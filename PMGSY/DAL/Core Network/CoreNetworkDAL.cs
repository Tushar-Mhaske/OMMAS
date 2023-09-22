#region HEADER
/*
 * Project Id:

 * Project Name:OMMAS-II

 * File Name: CoreNetworkDAL.cs

 * Author : Vikram Nandanwar

 * Creation Date :30/May/2013

 * Desc : This class is used as data access layer to perform Save,Edit,Delete and listing of Core Network screens.  
*/

#endregion


using PMGSY.BAL.Core_Network;
using PMGSY.Common;
using PMGSY.Controllers;
using PMGSY.Extensions;
using PMGSY.Models;
using PMGSY.Models.CoreNetwork;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Validation;
//using System.Data.Entity.Core.Objects;
using System.Data.Entity.Core.Objects;
using System.IO;
using System.Linq;
using System.Transactions;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using System.Data.Entity.Core;

namespace PMGSY.DAL.Core_Network
{
    public class CoreNetworkDAL : ICoreNetworkDAL
    {
        Dictionary<string, string> decryptedParameters = null;
        string[] encryptedParameters = null;


        #region CORE_NETWORKS

        public bool checkIsLocked(int block)
        {
            PMGSYEntities dbContext = new PMGSYEntities();
            bool flag = false;
            try
            {
                var query = dbContext.IMS_UNLOCK_DETAILS.Where(x => x.MAST_PMGSY_SCHEME == 1 &&
                                    ((x.MAST_BLOCK_CODE == block && x.IMS_UNLOCK_LEVEL == "B") || (x.IMS_UNLOCK_LEVEL == "D") || (x.IMS_UNLOCK_LEVEL == "S"))
                                    && (x.IMS_UNLOCK_START_DATE >= DateTime.Now && x.IMS_UNLOCK_END_DATE >= DateTime.Now)).Any();

                flag = Convert.ToInt32(query) > 0 ? true : false;

                return flag;

            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "CoreNetworkDAL.checkIsLocked()");
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
        /// Returns the list of Core Network roads
        /// </summary>
        /// <param name="blockCode"></param>
        /// <param name="roadType"></param>
        /// <param name="roadCode"></param>
        /// <param name="roadName"></param>
        /// <param name="page"></param>
        /// <param name="rows"></param>
        /// <param name="sidx"></param>
        /// <param name="sord"></param>
        /// <param name="totalRecords"></param>
        /// <returns></returns>
        public Array GetCoreNetWorksList(int stateCode, int districtCode, int blockCode, string roadType, int roadCode, string roadName, int page, int rows, string sidx, string sord, out long totalRecords, int CnCode)
        {
            string filters = string.Empty;
            string nameSearch = string.Empty;
            PMGSYEntities dbContext = new PMGSYEntities();

            try
            {
                // new change with data from stored procedure
                if (roadType == "0")
                {
                    roadType = null;
                }

                if (PMGSYSession.Current.RoleCode == 2)
                {
                    stateCode = PMGSYSession.Current.StateCode;
                }
                else if (PMGSYSession.Current.RoleCode == 22)
                {
                    stateCode = PMGSYSession.Current.StateCode;
                    districtCode = PMGSYSession.Current.DistrictCode;
                }

                short roleCode = dbContext.UM_User_Master.Where(m => m.UserID == PMGSYSession.Current.UserId).Select(m => m.DefaultRoleID).FirstOrDefault();

                var lstPlanRoads = dbContext.GET_CORE_NETWORKS((stateCode <= 0 ? 0 : stateCode), (districtCode <= 0 ? 0 : districtCode), (blockCode <= 0 ? 0 : blockCode), roadType, roadCode, roadName, roleCode).ToList();

                lstPlanRoads = lstPlanRoads.Where(m => m.MAST_PMGSY_SCHEME == PMGSYSession.Current.PMGSYScheme).ToList();

                if (CnCode > 0)
                {
                    lstPlanRoads = lstPlanRoads.Where(m => m.PLAN_CN_ROAD_CODE == CnCode).ToList();
                }

                totalRecords = lstPlanRoads.Count();

                if (sidx.Trim() != string.Empty)
                {
                    if (sord.ToString() == "asc")
                    {
                        switch (sidx)
                        {
                            case "PLAN_CN_ROAD_CODE":
                                lstPlanRoads = lstPlanRoads.OrderBy(x => x.PLAN_CN_ROAD_CODE).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                            case "PLAN_CN_ROAD_NUMBER":
                                lstPlanRoads = lstPlanRoads.OrderBy(x => x.PLAN_CN_ROAD_NUMBER).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                            //case "MAST_ER_ROAD_CODE":
                            //    lstPlanRoads = lstPlanRoads.OrderBy(x => x.MAST_ER_ROAD_CODE).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                            //    break;
                            case "PLAN_RD_NAME":
                                lstPlanRoads = lstPlanRoads.OrderBy(x => x.PLAN_RD_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                            case "PLAN_RD_FROM":
                                lstPlanRoads = lstPlanRoads.OrderBy(x => x.PLAN_RD_FROM).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                            case "PLAN_RD_TO":
                                lstPlanRoads = lstPlanRoads.OrderBy(x => x.PLAN_RD_TO).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                            case "PLAN_RD_FROM_CHAINAGE":
                                lstPlanRoads = lstPlanRoads.OrderBy(x => x.PLAN_RD_FROM_CHAINAGE).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                            case "PLAN_RD_TO_CHAINAGE":
                                lstPlanRoads = lstPlanRoads.OrderBy(x => x.PLAN_RD_TO_CHAINAGE).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                            case "PLAN_RD_LENGTH":
                                lstPlanRoads = lstPlanRoads.OrderBy(x => x.PLAN_RD_LENGTH).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                            case "PLAN_RD_TOTAL_LENGTH":
                                lstPlanRoads = lstPlanRoads.OrderBy(x => x.PLAN_RD_TOTAL_LEN).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                            case "PLAN_RD_ROUTE":
                                lstPlanRoads = lstPlanRoads.OrderBy(x => x.PLAN_RD_ROUTE).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                            default:
                                lstPlanRoads = lstPlanRoads.OrderBy(x => x.PLAN_CN_ROAD_CODE).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                        }
                    }
                    else
                    {
                        switch (sidx)
                        {
                            case "PLAN_CN_ROAD_CODE":
                                lstPlanRoads = lstPlanRoads.OrderByDescending(x => x.PLAN_CN_ROAD_CODE).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                            case "PLAN_CN_ROAD_NUMBER":
                                lstPlanRoads = lstPlanRoads.OrderByDescending(x => x.PLAN_CN_ROAD_NUMBER).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                            //case "MAST_ER_ROAD_CODE":
                            //    lstPlanRoads = lstPlanRoads.OrderByDescending(x => x.MAST_ER_ROAD_CODE).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                            //    break;
                            case "PLAN_RD_NAME":
                                lstPlanRoads = lstPlanRoads.OrderByDescending(x => x.PLAN_RD_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                            case "PLAN_RD_FROM":
                                lstPlanRoads = lstPlanRoads.OrderByDescending(x => x.PLAN_RD_FROM).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                            case "PLAN_RD_TO":
                                lstPlanRoads = lstPlanRoads.OrderByDescending(x => x.PLAN_RD_TO).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                            case "PLAN_RD_FROM_CHAINAGE":
                                lstPlanRoads = lstPlanRoads.OrderByDescending(x => x.PLAN_RD_FROM_CHAINAGE).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                            case "PLAN_RD_TO_CHAINAGE":
                                lstPlanRoads = lstPlanRoads.OrderByDescending(x => x.PLAN_RD_TO_CHAINAGE).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                            case "PLAN_RD_LENGTH":
                                lstPlanRoads = lstPlanRoads.OrderByDescending(x => x.PLAN_RD_LENGTH).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                            case "PLAN_RD_TOTAL_LENGTH":
                                lstPlanRoads = lstPlanRoads.OrderByDescending(x => x.PLAN_RD_TOTAL_LEN).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                            case "PLAN_RD_ROUTE":
                                lstPlanRoads = lstPlanRoads.OrderByDescending(x => x.PLAN_RD_ROUTE).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                            default:
                                lstPlanRoads = lstPlanRoads.OrderByDescending(x => x.PLAN_CN_ROAD_CODE).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                        }
                    }
                }
                else
                {
                    lstPlanRoads = lstPlanRoads.OrderByDescending(x => x.PLAN_CN_ROAD_CODE).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                }

                var result = lstPlanRoads.Select(roadDetails => new
                {
                    //roadDetails.MAST_ER_ROAD_NUMBER,
                    roadDetails.BLOCK_NAME,
                    roadDetails.DISTRICT_NAME,
                    //roadDetails.ER,
                    roadDetails.STATE_NAME,
                    roadDetails.PLAN_CN_ROAD_CODE,
                    roadDetails.PLAN_CN_ROAD_NUMBER,
                    //roadDetails.PLAN_LOCK_STATUS,
                    roadDetails.PLAN_RD_BLOCK_FROM_CODE,
                    roadDetails.PLAN_RD_BLOCK_TO_CODE,
                    roadDetails.PLAN_RD_FROM_CHAINAGE,
                    roadDetails.PLAN_RD_FROM_HAB,
                    roadDetails.PLAN_RD_FROM_TYPE,
                    roadDetails.PLAN_RD_LENG,
                    roadDetails.PLAN_RD_LENGTH,
                    roadDetails.PLAN_RD_NAME,
                    roadDetails.PLAN_RD_NUM_FROM,
                    roadDetails.PLAN_RD_NUM_TO,
                    roadDetails.PLAN_RD_ROUTE,
                    roadDetails.PLAN_RD_TO_CHAINAGE,
                    roadDetails.PLAN_RD_TO_HAB,
                    roadDetails.PLAN_RD_TO_TYPE,
                    roadDetails.PLAN_RD_FROM,
                    roadDetails.PLAN_RD_TO,
                    roadDetails.UNLOCK_BY_MORD,
                    roadDetails.MAST_ER_ROAD_NUMBER,
                    roadDetails.PLAN_RD_TOTAL_LEN
                }).ToArray();

                return result.Select(roadDetails => new
                {
                    id = URLEncrypt.EncryptParameters1(new string[] { "NetworkCode=" + roadDetails.PLAN_CN_ROAD_CODE.ToString() }),
                    cell = new[]
                {
                    roadDetails.PLAN_CN_ROAD_CODE.ToString().Trim(),
                    roadDetails.PLAN_RD_ROUTE == null?string.Empty:roadDetails.PLAN_RD_ROUTE.ToString(),
                    roadDetails.PLAN_CN_ROAD_NUMBER == null?string.Empty:roadDetails.PLAN_CN_ROAD_NUMBER.ToString(),
                    roadDetails.PLAN_RD_NAME == null?string.Empty:roadDetails.PLAN_RD_NAME.ToString(),
                    roadDetails.MAST_ER_ROAD_NUMBER.ToString(),
                    roadDetails.PLAN_RD_FROM_TYPE=="B"?"Block("+(dbContext.MASTER_BLOCK.Where(item=>item.MAST_BLOCK_CODE == roadDetails.PLAN_RD_BLOCK_FROM_CODE).Select(m=>m.MAST_BLOCK_NAME).FirstOrDefault())+")":(roadDetails.PLAN_RD_FROM_TYPE=="H"?"Habitation("+(dbContext.MASTER_HABITATIONS.Where(item=>item.MAST_HAB_CODE == roadDetails.PLAN_RD_FROM_HAB).Select(m=>m.MAST_HAB_NAME).FirstOrDefault())+")":(roadDetails.PLAN_RD_FROM==null?string.Empty:roadDetails.PLAN_RD_FROM.ToString())),
                    roadDetails.PLAN_RD_TO_TYPE=="B"?"Block("+(dbContext.MASTER_BLOCK.Where(item=>item.MAST_BLOCK_CODE == roadDetails.PLAN_RD_BLOCK_TO_CODE).Select(m=>m.MAST_BLOCK_NAME).FirstOrDefault())+")":(roadDetails.PLAN_RD_TO_TYPE=="H"?"Habitation("+(dbContext.MASTER_HABITATIONS.Where(item=>item.MAST_HAB_CODE == roadDetails.PLAN_RD_TO_HAB).Select(m=>m.MAST_HAB_NAME).FirstOrDefault())+")":(roadDetails.PLAN_RD_TO==null?string.Empty:roadDetails.PLAN_RD_TO.ToString())),

                    roadDetails.PLAN_RD_FROM_CHAINAGE == null?string.Empty:roadDetails.PLAN_RD_FROM_CHAINAGE.ToString(),
                    roadDetails.PLAN_RD_TO_CHAINAGE == null?string.Empty:roadDetails.PLAN_RD_TO_CHAINAGE.ToString(),
                    roadDetails.PLAN_RD_LENGTH == null?string.Empty:roadDetails.PLAN_RD_LENGTH.ToString(),
                    PMGSYSession.Current.PMGSYScheme == 2 ? (roadDetails.PLAN_RD_TOTAL_LEN == null ? Convert.ToString(roadDetails.PLAN_RD_TO_CHAINAGE - roadDetails.PLAN_RD_FROM_CHAINAGE) :  roadDetails.PLAN_RD_TOTAL_LEN.ToString()) : "",
                    roadDetails.UNLOCK_BY_MORD == "M"?"<a href='#' title='Click here to map habitation Details' class='ui-icon ui-icon-unlocked ui-align-center' onClick =editHabitationDetails('"+URLEncrypt.EncryptParameters1(new string[]{"NetworkCode =" + roadDetails.PLAN_CN_ROAD_CODE.ToString().Trim(),"UnlockFlag =" + roadDetails.UNLOCK_BY_MORD.ToString().Trim()})+"'); return false;'>Delete</a>":roadDetails.UNLOCK_BY_MORD=="N"?"<a href='#' title='Click here to map habitation Details' class='ui-icon ui-icon-plusthick ui-align-center' onClick =editHabitationDetails('"+URLEncrypt.EncryptParameters1(new string[]{"NetworkCode =" + roadDetails.PLAN_CN_ROAD_CODE.ToString().Trim(),"UnlockFlag =" + roadDetails.UNLOCK_BY_MORD.ToString().Trim()})+"'); return false;'>Delete</a>":"<span class='ui-icon ui-icon-locked ui-align-center'></span>",
                    ///Changes by SAMMED A. PATIL on 27JULY2017 to lock Habitation mapping if CoreNetwork is locked uncommented above line
                    //"<a href='#' title='Click here to map habitation Details' class='ui-icon ui-icon-plusthick ui-align-center' onClick =editHabitationDetails('"+URLEncrypt.EncryptParameters1(new string[]{"NetworkCode =" + roadDetails.PLAN_CN_ROAD_CODE.ToString().Trim(),"UnlockFlag =" + roadDetails.UNLOCK_BY_MORD.ToString().Trim()})+"'); return false;'>Delete</a>",
                    (roadDetails.UNLOCK_BY_MORD == "M"?"<a href='#' title='Click here to upload file details' class='ui-icon ui-icon-unlocked ui-align-center' onClick =UploadFile('"+URLEncrypt.EncryptParameters1(new string[]{"NetworkCode =" + roadDetails.PLAN_CN_ROAD_CODE.ToString().Trim()})+"'); return false;'>Delete</a>":roadDetails.UNLOCK_BY_MORD=="N"?"<a href='#' title='Click here to upload file details' class='ui-icon ui-icon-plusthick ui-align-center' onClick =UploadFile('"+URLEncrypt.EncryptParameters1(new string[]{"NetworkCode =" + roadDetails.PLAN_CN_ROAD_CODE.ToString().Trim()})+"'); return false;'>Delete</a>":"<span class='ui-icon ui-icon-locked ui-align-center'></span></center>"),
                    PMGSYSession.Current.PMGSYScheme == 1?"":roadDetails.UNLOCK_BY_MORD == "M"?("<a href='#' title='Click here to map other candidate road' class='ui-icon ui-icon-unlocked ui-align-center' onClick =MapOtherCandidateRoad('"+URLEncrypt.EncryptParameters1(new string[]{"NetworkCode =" + roadDetails.PLAN_CN_ROAD_CODE.ToString().Trim()})+"'); return false;'>Map Other DRRP Road</a>"):roadDetails.UNLOCK_BY_MORD == "Y"?"<span class='ui-icon ui-icon-locked ui-align-center'></span>":"<a href='#' title='Click here to map other candidate road' class='ui-icon ui-icon-plusthick ui-align-center' onClick =MapOtherCandidateRoad('"+URLEncrypt.EncryptParameters1(new string[]{"NetworkCode =" + roadDetails.PLAN_CN_ROAD_CODE.ToString().Trim()})+"'); return false;'>Map Other DRRP Road</a>",
                    "<a href='#' title='Click here to view core network details' class='ui-icon ui-icon-zoomin ui-align-center' onClick =detailsCoreNetwork('"+URLEncrypt.EncryptParameters1(new string[]{"NetworkCode =" + roadDetails.PLAN_CN_ROAD_CODE.ToString().Trim()})+"'); return false;'>Delete</a>",
                    //roadDetails.UNLOCK_BY_MORD == "M"?"<a href='#' title='Click here to edit core network details' class='ui-icon ui-icon-unlocked ui-align-center' onClick =editNetworkData('"+URLEncrypt.EncryptParameters1(new string[]{"NetworkCode =" + roadDetails.PLAN_CN_ROAD_CODE.ToString().Trim(),"UnlockFlag =" + roadDetails.UNLOCK_BY_MORD.ToString().Trim()})+"'); return false;'>Delete</a>":roadDetails.UNLOCK_BY_MORD=="N"?"<a href='#' title='Click here to view core network details' class='ui-icon ui-icon-pencil ui-align-center' onClick =editNetworkData('"+URLEncrypt.EncryptParameters1(new string[]{"NetworkCode =" + roadDetails.PLAN_CN_ROAD_CODE.ToString().Trim()})+"'); return false;'>Delete</a>":"<span class='ui-icon ui-icon-locked ui-align-center'></span></center>",
                    
                    ///Changes by SAMMED A. PATIL on 03 OCTOBER 2017 to edit DRRP for MORD
                    PMGSYSession.Current.RoleCode == 25 
                        ? "<a href='#' title='Click here to view core network details' class='ui-icon ui-icon-pencil ui-align-center' onClick =editNetworkData('"+URLEncrypt.EncryptParameters1(new string[]{"NetworkCode =" + roadDetails.PLAN_CN_ROAD_CODE.ToString().Trim()})+"'); return false;'>Edit</a>"
                        : (roadDetails.UNLOCK_BY_MORD == "M"?"<a href='#' title='Click here to edit core network details' class='ui-icon ui-icon-unlocked ui-align-center' onClick =editNetworkData('"+URLEncrypt.EncryptParameters1(new string[]{"NetworkCode =" + roadDetails.PLAN_CN_ROAD_CODE.ToString().Trim(),"UnlockFlag =" + roadDetails.UNLOCK_BY_MORD.ToString().Trim()})+"'); return false;'>Delete</a>":roadDetails.UNLOCK_BY_MORD=="N"?"<a href='#' title='Click here to view core network details' class='ui-icon ui-icon-pencil ui-align-center' onClick =editNetworkData('"+URLEncrypt.EncryptParameters1(new string[]{"NetworkCode =" + roadDetails.PLAN_CN_ROAD_CODE.ToString().Trim()})+"'); return false;'>Edit</a>":"<span class='ui-icon ui-icon-locked ui-align-center'></span></center>"),
                    
                    roadDetails.UNLOCK_BY_MORD == "M"?"<a href='#' title='Click here to delete core network details' class='ui-icon ui-icon-trash ui-align-center' onClick =deleteNetworkData('"+URLEncrypt.EncryptParameters1(new string[]{"NetworkCode =" + roadDetails.PLAN_CN_ROAD_CODE.ToString().Trim()})+"'); return false;'>Delete</a>":roadDetails.UNLOCK_BY_MORD=="N"?"<a href='#' title='Click here to delete core network details' class='ui-icon ui-icon-trash ui-align-center' onClick =deleteNetworkData('"+URLEncrypt.EncryptParameters1(new string[]{"NetworkCode =" + roadDetails.PLAN_CN_ROAD_CODE.ToString().Trim()})+"'); return false;'>Delete</a>":"<span class='ui-icon ui-icon-locked ui-align-center'></span></center>",
                }
                }).ToArray();


                //end of change


                //lstPlanRoads = lstPlanRoads.GroupBy(rl => rl.PLAN_CN_ROAD_CODE).Select(rl => rl.FirstOrDefault());
            }
            catch (Exception ex)
            {
                //Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                ErrorLog.LogError(ex, "GetCoreNetWorksList().DAL");
                totalRecords = 0;
                dbContext.Dispose();
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
        /// saves the core network details
        /// </summary>
        /// <param name="model"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public bool AddCoreNetworks(CoreNetworkViewModel model, ref string message)
        {
            PMGSYEntities dbContext = new PMGSYEntities();
            try
            {

                decimal chainageFrom = Convert.ToDecimal(model.PLAN_RD_FROM_CHAINAGE);
                decimal chainageTo = Convert.ToDecimal(model.PLAN_RD_TO_CHAINAGE);
                int recordCount = dbContext.PLAN_ROAD.Where(m => m.MAST_STATE_CODE == model.MAST_STATE_CODE && m.MAST_DISTRICT_CODE == model.MAST_DISTRICT_CODE && m.MAST_BLOCK_CODE == model.MAST_BLOCK_CODE && m.PLAN_CN_ROAD_NUMBER == model.PLAN_CN_ROAD_NUMBER && m.PLAN_RD_FROM_CHAINAGE == chainageFrom && m.PLAN_RD_TO_CHAINAGE == chainageTo && m.MAST_PMGSY_SCHEME == PMGSYSession.Current.PMGSYScheme).Count();

                if (recordCount > 0)
                {
                    if (PMGSYSession.Current.PMGSYScheme == 1)
                    {
                        message = "Core Network information already exist.";
                    }
                    // added by rohit for vibrant village prog on 20-07-2023
                    else if (PMGSYSession.Current.PMGSYScheme == 2)
                    {
                        message = "Candidate Road details already present.";
                    }
                    return false;
                }

                PLAN_ROAD master = new PLAN_ROAD();
                master = CloneModelToObject(model);
                master.USERID = PMGSYSession.Current.UserId;
                master.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                master.PLAN_CN_ROAD_CODE = dbContext.PLAN_ROAD.Any() ? (from item in dbContext.PLAN_ROAD select item.PLAN_CN_ROAD_CODE).Max() + 1 : 1;
                dbContext.PLAN_ROAD.Add(master);
                dbContext.SaveChanges();
                //new change done by Vikram on 04 Feb 2014 for PMGSY Scheme 2
                if (PMGSYSession.Current.PMGSYScheme == 2)
                {
                    List<int> lstHabCodes = dbContext.MASTER_ER_HABITATION_ROAD.Where(m => m.MAST_ER_ROAD_CODE == master.MAST_ER_ROAD_CODE).Select(m => m.MAST_HAB_CODE).ToList();
                    if (lstHabCodes != null)
                    {
                        foreach (var item in lstHabCodes)
                        {
                            PLAN_ROAD_HABITATION mappingMaster = new PLAN_ROAD_HABITATION();
                            mappingMaster.MAST_HAB_CODE = item;
                            mappingMaster.PLAN_CN_ROAD_CODE = master.PLAN_CN_ROAD_CODE;
                            mappingMaster.PLAN_CN_ROAD_HAB_ID = dbContext.PLAN_ROAD_HABITATION.Any() ? dbContext.PLAN_ROAD_HABITATION.Max(m => m.PLAN_CN_ROAD_HAB_ID) + 1 : 1;
                            mappingMaster.USERID = PMGSYSession.Current.UserId;
                            mappingMaster.IPADD = master.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                            dbContext.PLAN_ROAD_HABITATION.Add(mappingMaster);
                            dbContext.SaveChanges();
                        }
                    }
                }
                //end of change


                return true;
            }
            catch (Exception ex)
            {
                //objLog.WriteErrorMessage(ex.Message);
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

        /// <summary>
        /// update the core network details
        /// </summary>
        /// <param name="model"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public bool EditCoreNetworks(CoreNetworkViewModel model, ref string message)
        {
            PMGSYEntities dbContext = new PMGSYEntities();
            try
            {
                int networkCode = 0;
                encryptedParameters = model.EncryptedRoadCode.Split('/');
                if (!(encryptedParameters.Length == 3))
                {
                    return false;
                }

                decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { encryptedParameters[0], encryptedParameters[1], encryptedParameters[2] });
                networkCode = Convert.ToInt32(decryptedParameters["NetworkCode"].ToString());

                decimal chainageFrom = Convert.ToDecimal(model.PLAN_RD_FROM_CHAINAGE);
                decimal chainageTo = Convert.ToDecimal(model.PLAN_RD_TO_CHAINAGE);

                // added by rohit for vibrant village prog on 20-07-2023
                if (PMGSYSession.Current.PMGSYScheme == 2 || PMGSYSession.Current.PMGSYScheme == 5)
                {
                    if (dbContext.PLAN_ROAD_DRRP.Any(m => m.PLAN_CN_ROAD_CODE == networkCode))
                    {
                        if (model.TotalLengthOfCandidate < (dbContext.PLAN_ROAD_DRRP.Where(m => m.PLAN_CN_ROAD_CODE == networkCode).Sum(m => m.PLAN_RD_LENGTH) + Convert.ToDecimal(model.PLAN_RD_LENGTH)))
                        {
                            message = "Total length of Candidate Road should not be less than the sum of candidate road length and other DRRP road length mapped to it.";
                            return false;
                        }
                    }
                }

                int recordCount = dbContext.PLAN_ROAD.Where(m => m.MAST_STATE_CODE == model.MAST_STATE_CODE && m.MAST_DISTRICT_CODE == model.MAST_DISTRICT_CODE && m.MAST_BLOCK_CODE == model.MAST_BLOCK_CODE && m.PLAN_CN_ROAD_NUMBER == model.PLAN_CN_ROAD_NUMBER && m.PLAN_RD_FROM_CHAINAGE == chainageFrom && m.PLAN_RD_TO_CHAINAGE == chainageTo && m.PLAN_CN_ROAD_CODE != networkCode && m.MAST_PMGSY_SCHEME == PMGSYSession.Current.PMGSYScheme).Count();
                if (recordCount > 0)
                {
                    if (PMGSYSession.Current.PMGSYScheme == 1)
                    {
                        message = "Core Network information already exist.";
                    }
                    // added by rohit for vibrant village prog on 20-07-2023
                    else if (PMGSYSession.Current.PMGSYScheme == 2 || PMGSYSession.Current.PMGSYScheme == 5)
                    {
                        message = "Candidate Road details already present.";
                    }
                    return false;
                }

                PLAN_ROAD master = dbContext.PLAN_ROAD.Where(m => m.PLAN_CN_ROAD_CODE == networkCode).FirstOrDefault();
                if (master != null)
                {
                    master = CloneModelToObject(model);
                    master.PLAN_CN_ROAD_CODE = networkCode;
                    master.USERID = PMGSYSession.Current.UserId;
                    master.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                    //dbContext.Entry(master).State = System.Data.Entity.EntityState.Modified;
                    var currentProduct = dbContext.PLAN_ROAD.Where(m => m.PLAN_CN_ROAD_CODE == networkCode).FirstOrDefault(); ;
                    dbContext.Entry(currentProduct).CurrentValues.SetValues(master);
                    dbContext.SaveChanges();
                    return true;
                }
                else
                {
                    return false;
                }

            }
            catch (Exception ex)
            {
                //objLog.WriteErrorMessage(ex.Message);
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

        /// <summary>
        /// returns the core network details for updation
        /// </summary>
        /// <param name="networkCode"></param>
        /// <returns></returns>
        public CoreNetworkViewModel GetCoreNetworkDetails(int networkCode)
        {
            PMGSYEntities dbContext = new PMGSYEntities();
            try
            {
                PLAN_ROAD master = dbContext.PLAN_ROAD.Where(m => m.PLAN_CN_ROAD_CODE == networkCode).FirstOrDefault();
                CoreNetworkViewModel model = null;
                if (master != null)
                {
                    model = new CoreNetworkViewModel()
                    {
                        EncryptedRoadCode = URLEncrypt.EncryptParameters1(new string[] { "NetworkCode=" + master.PLAN_CN_ROAD_CODE.ToString().Trim() }),
                        MAST_BLOCK_CODE = master.MAST_BLOCK_CODE,
                        MAST_DISTRICT_CODE = master.MAST_DISTRICT_CODE,
                        MAST_ER_ROAD_CODE = master.MAST_ER_ROAD_CODE,
                        MAST_STATE_CODE = master.MAST_STATE_CODE,
                        PLAN_CN_ROAD_CODE = master.PLAN_CN_ROAD_CODE,
                        PLAN_CN_ROAD_NUMBER = master.PLAN_CN_ROAD_NUMBER,
                        PLAN_LOCK_STATUS = master.PLAN_LOCK_STATUS,
                        PLAN_RD_BLOCK_FROM_CODE = master.PLAN_RD_BLOCK_FROM_CODE,
                        PLAN_RD_BLOCK_TO_CODE = master.PLAN_RD_BLOCK_TO_CODE,
                        PLAN_RD_FROM = master.PLAN_RD_FROM_TYPE == "B" ? "Block(" + (dbContext.MASTER_BLOCK.Where(item => item.MAST_BLOCK_CODE == master.PLAN_RD_BLOCK_FROM_CODE).Select(m => m.MAST_BLOCK_NAME).FirstOrDefault()) + ")" : (master.PLAN_RD_FROM_TYPE == "H" ? "Habitation(" + (dbContext.MASTER_HABITATIONS.Where(item => item.MAST_HAB_CODE == master.PLAN_RD_FROM_HAB).Select(m => m.MAST_HAB_NAME).FirstOrDefault()) + ")" : (master.PLAN_RD_FROM == null ? string.Empty : master.PLAN_RD_FROM)),
                        PLAN_RD_FROM_CHAINAGE = Convert.ToDouble(master.PLAN_RD_FROM_CHAINAGE),
                        PLAN_RD_FROM_HAB = master.PLAN_RD_FROM_HAB,
                        PLAN_RD_FROM_TYPE = master.PLAN_RD_FROM_TYPE,
                        PLAN_RD_LENG = master.PLAN_RD_LENG,
                        PLAN_RD_LENGTH = (double)master.PLAN_RD_LENGTH,
                        PLAN_RD_NAME = master.PLAN_RD_NAME,
                        PLAN_RD_NUM_FROM = master.PLAN_RD_NUM_FROM,
                        PLAN_RD_NUM_TO = master.PLAN_RD_NUM_TO,
                        PLAN_RD_ROUTE = master.PLAN_RD_ROUTE,
                        PLAN_RD_TO = master.PLAN_RD_TO_TYPE == "B" ? "Block(" + (dbContext.MASTER_BLOCK.Where(item => item.MAST_BLOCK_CODE == master.PLAN_RD_BLOCK_TO_CODE).Select(m => m.MAST_BLOCK_NAME).FirstOrDefault()) + ")" : (master.PLAN_RD_TO_TYPE == "H" ? "Habitation(" + (dbContext.MASTER_HABITATIONS.Where(item => item.MAST_HAB_CODE == master.PLAN_RD_TO_HAB).Select(m => m.MAST_HAB_NAME).FirstOrDefault()) + ")" : (master.PLAN_RD_TO == null ? string.Empty : master.PLAN_RD_TO)),
                        PLAN_RD_TO_CHAINAGE = (double)master.PLAN_RD_TO_CHAINAGE,
                        PLAN_RD_TO_HAB = master.PLAN_RD_TO_HAB,
                        PLAN_RD_TO_TYPE = master.PLAN_RD_TO_TYPE,
                        TotalLengthOfCandidate = master.PLAN_RD_TOTAL_LEN,
                        ExistStartChainage = Convert.ToDouble(dbContext.MASTER_EXISTING_ROADS.Where(m => m.MAST_ER_ROAD_CODE == master.MAST_ER_ROAD_CODE).Select(m => m.MAST_ER_ROAD_STR_CHAIN).FirstOrDefault()),
                        ExistEndChainage = Convert.ToDouble(dbContext.MASTER_EXISTING_ROADS.Where(m => m.MAST_ER_ROAD_CODE == master.MAST_ER_ROAD_CODE).Select(m => m.MAST_ER_ROAD_END_CHAIN).FirstOrDefault()),
                        //RoadCatCode = dbContext.MASTER_EXISTING_ROADS.Where(m => m.MAST_ER_ROAD_CODE == model.MAST_ER_ROAD_CODE).Select(r => r.MAST_ROAD_CAT_CODE).FirstOrDefault()
                        //RoadShortCode = (from plan in dbContext.PLAN_ROAD
                        //                 join extroads in dbContext.MASTER_EXISTING_ROADS
                        //                 on plan.MAST_ER_ROAD_CODE equals extroads.MAST_ER_ROAD_CODE
                        //                 join roadcat in dbContext.MASTER_ROAD_CATEGORY
                        //                 on extroads.MAST_ROAD_CAT_CODE equals roadcat.MAST_ROAD_CAT_CODE
                        //                 where (extroads.MAST_ER_ROAD_CODE == master.MAST_ER_ROAD_CODE)
                        //                 select new { roadcat.MAST_ROAD_SHORT_DESC }).FirstOrDefault().ToString(),
                        RoadShortCode = dbContext.MASTER_EXISTING_ROADS.Where(r => r.MAST_ER_ROAD_CODE == master.MAST_ER_ROAD_CODE).Select(a => a.MASTER_ROAD_CATEGORY.MAST_ROAD_SHORT_DESC).FirstOrDefault(),
                    };
                }
                return model;
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
        /// delete the core network details
        /// </summary>
        /// <param name="networkCode"></param>
        /// <returns></returns>
        public bool DeleteCoreNetworks(int networkCode)
        {
            PMGSYEntities dbContext = new PMGSYEntities();
            try
            {
                using (TransactionScope ts = new TransactionScope())
                {
                    if (dbContext.IMS_UNLOCK_DETAILS.Any(m => m.PLAN_CN_ROAD_CODE == networkCode && m.MAST_PMGSY_SCHEME == PMGSYSession.Current.PMGSYScheme))
                    {
                        IMS_UNLOCK_DETAILS unlockDetails = dbContext.IMS_UNLOCK_DETAILS.Where(m => m.PLAN_CN_ROAD_CODE == networkCode && m.MAST_PMGSY_SCHEME == PMGSYSession.Current.PMGSYScheme).FirstOrDefault();
                        if (unlockDetails != null)
                        {
                            unlockDetails.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                            unlockDetails.USERID = PMGSYSession.Current.UserId;
                            dbContext.Entry(unlockDetails).State = System.Data.Entity.EntityState.Modified;
                            dbContext.SaveChanges();

                            dbContext.IMS_UNLOCK_DETAILS.Remove(unlockDetails);
                            dbContext.SaveChanges();
                        }

                    }

                    PLAN_ROAD master = dbContext.PLAN_ROAD.Where(m => m.PLAN_CN_ROAD_CODE == networkCode && m.MAST_PMGSY_SCHEME == PMGSYSession.Current.PMGSYScheme).FirstOrDefault();
                    master.USERID = PMGSYSession.Current.UserId;
                    master.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                    dbContext.Entry(master).State = System.Data.Entity.EntityState.Modified;
                    dbContext.SaveChanges();

                    // added by rohit for vibrant village prog on 20-07-2023
                    if (PMGSYSession.Current.PMGSYScheme == 2 || PMGSYSession.Current.PMGSYScheme == 5)
                    {
                        List<PLAN_ROAD_HABITATION> lstRoads = dbContext.PLAN_ROAD_HABITATION.Where(m => m.PLAN_CN_ROAD_CODE == networkCode && m.PLAN_ROAD.MAST_PMGSY_SCHEME == PMGSYSession.Current.PMGSYScheme).ToList();
                        if (lstRoads.Count > 0)
                        {
                            foreach (var item in lstRoads)
                            {
                                dbContext.PLAN_ROAD_HABITATION.Remove(item);
                            }
                        }
                    }
                    dbContext.PLAN_ROAD.Remove(master);
                    dbContext.SaveChanges();
                    ts.Complete();
                }
                return true;
            }
            catch (DbUpdateException ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                return false;
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

        /// <summary>
        /// returns road category list
        /// </summary>
        /// <returns></returns>
        public List<MASTER_ROAD_CATEGORY> GetAllRoadCategories()
        {
            PMGSYEntities dbContext = new PMGSYEntities();
            try
            {
                List<MASTER_ROAD_CATEGORY> lstRoadCategory = dbContext.MASTER_ROAD_CATEGORY.OrderBy(m => m.MAST_ROAD_CAT_NAME).ToList<MASTER_ROAD_CATEGORY>();
                lstRoadCategory.Insert(0, new MASTER_ROAD_CATEGORY { MAST_ROAD_CAT_CODE = 0, MAST_ROAD_CAT_NAME = "--Select Category--" });
                return lstRoadCategory;
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
        /// returns road name according to the road code
        /// </summary>
        /// <param name="roadCode"></param>
        /// <param name="blockCode"></param>
        /// <returns></returns>
        public List<SelectListItem> GetRoadNamesByRoadCode(int roadCode, int blockCode)
        {
            PMGSYEntities dbContext = new PMGSYEntities();
            try
            {
                //var  roadList = dbContext.MASTER_EXISTING_ROADS.Where(d => d.MAST_ROAD_CAT_CODE == roadCode && d.MAST_BLOCK_CODE == blockCode && d.MAST_CORE_NETWORK == "Y" && d.MAST_PMGSY_SCHEME == PMGSYSession.Current.PMGSYScheme).OrderBy(d => d.MAST_ER_ROAD_NAME).ToList<Models.MASTER_EXISTING_ROADS>();

                var roadList = (from item in dbContext.MASTER_EXISTING_ROADS
                                where item.MAST_ROAD_CAT_CODE == roadCode &&
                                item.MAST_BLOCK_CODE == blockCode &&
                                item.MAST_CORE_NETWORK == "Y" &&
                                item.MAST_PMGSY_SCHEME == (PMGSYSession.Current.PMGSYScheme == 3 ? (byte)1 : PMGSYSession.Current.PMGSYScheme) ///Changes for RCPLWE, PMGSY3
                                select new
                                {
                                    MAST_ROAD_CODE = item.MAST_ER_ROAD_CODE,
                                    MAST_ROAD_NAME = (item.MAST_ER_ROAD_NUMBER + " - " + item.MAST_ER_ROAD_NAME)
                                }).OrderBy(m => m.MAST_ROAD_NAME).ToList();



                return new SelectList(roadList.ToList(), "MAST_ROAD_CODE", "MAST_ROAD_NAME").ToList();

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
        /// returns the list of blocks according to the district with current block excluding 
        /// </summary>
        /// <param name="districtCode"></param>
        /// <param name="blockCode"></param>
        /// <returns></returns>
        public List<MASTER_BLOCK> GetBlocksByDistrictCode(int districtCode, int blockCode)
        {
            PMGSYEntities dbContext = new PMGSYEntities();
            try
            {
                List<MASTER_BLOCK> lstBlocks = dbContext.MASTER_BLOCK.Where(b => b.MAST_DISTRICT_CODE == districtCode && b.MAST_BLOCK_CODE != blockCode).Distinct<MASTER_BLOCK>().ToList<MASTER_BLOCK>();
                return lstBlocks;
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
        /// returns blocks according to the district
        /// </summary>
        /// <param name="districtCode"></param>
        /// <returns></returns>
        public List<MASTER_BLOCK> GetBlocksByDistrictCode(int districtCode)
        {
            PMGSYEntities dbContext = new PMGSYEntities();
            try
            {
                List<MASTER_BLOCK> lstBlocks = dbContext.MASTER_BLOCK.Where(b => b.MAST_DISTRICT_CODE == districtCode && b.MAST_BLOCK_ACTIVE == "Y").Distinct<MASTER_BLOCK>().ToList<MASTER_BLOCK>();
                return lstBlocks;
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
        /// returns the previous block names for dropdown
        /// </summary>
        /// <param name="blockCode"></param>
        /// <returns></returns>
        public List<PLAN_ROAD> GetPreviousBlockByBlockCode(int blockCode)
        {
            PMGSYEntities dbContext = new PMGSYEntities();
            try
            {
                List<PLAN_ROAD> lstPreviousBlock = dbContext.PLAN_ROAD.Where(m => m.MAST_BLOCK_CODE == blockCode).OrderBy(m => m.PLAN_CN_ROAD_NUMBER).ToList<PLAN_ROAD>();
                return lstPreviousBlock;

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
        /// returns the list of habitation according to the block
        /// </summary>
        /// <param name="blockCode"></param>
        /// <returns></returns>
        public List<SelectListItem> GetHabitationsByBlockCode(int blockCode)
        {
            PMGSYEntities dbContext = new PMGSYEntities();
            try
            {
                var lstHabitations = (from item in dbContext.MASTER_VILLAGE
                                      join habitation in dbContext.MASTER_HABITATIONS on item.MAST_VILLAGE_CODE equals habitation.MAST_VILLAGE_CODE
                                      join habitationDetails in dbContext.MASTER_HABITATIONS_DETAILS on habitation.MAST_HAB_CODE equals habitationDetails.MAST_HAB_CODE
                                      where item.MAST_BLOCK_CODE == blockCode
                                      select new
                                      {
                                          habitation.MAST_HAB_NAME,
                                          item.MAST_VILLAGE_NAME,
                                          habitation.MAST_HAB_CODE,
                                          habitationDetails.MAST_YEAR,
                                          habitationDetails.MAST_HAB_TOT_POP
                                      }).ToList();
                if (PMGSYSession.Current.PMGSYScheme == 2 || PMGSYSession.Current.PMGSYScheme == 4)///Changes for PMGSY3
                {
                    lstHabitations = lstHabitations.Where(m => m.MAST_YEAR == 2011).ToList();
                }
                else if (PMGSYSession.Current.PMGSYScheme == 1 || PMGSYSession.Current.PMGSYScheme == 3)///Changes for RCPLWE
                {
                    lstHabitations = lstHabitations.Where(m => m.MAST_YEAR == 2001).ToList();
                }

                var list = lstHabitations.ToList().Select(m => new SelectListItem { Text = m.MAST_HAB_NAME + " ( Village : " + m.MAST_VILLAGE_NAME + " , Population : " + m.MAST_HAB_TOT_POP.ToString() + " )", Value = m.MAST_HAB_CODE.ToString() }).OrderBy(m => m.Text);
                //list.ToList<SelectListItem>().Insert(0,new SelectListItem { Value="0",Text="--Select--"});
                return list.ToList<SelectListItem>();
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
        /// returns the habitation dropdown list
        /// </summary>
        /// <param name="networkCode"></param>
        /// <returns></returns>
        public List<SelectListItem> GetHabitationCodeList(int networkCode)
        {
            PMGSYEntities dbContext = new PMGSYEntities();
            try
            {
                int recordCount = 0;

                PLAN_ROAD masterRoad = dbContext.PLAN_ROAD.Find(networkCode);

                var lstHabitations = (from item in dbContext.MASTER_VILLAGE
                                      join habitation in dbContext.MASTER_HABITATIONS on item.MAST_VILLAGE_CODE equals habitation.MAST_VILLAGE_CODE
                                      join habitationDetails in dbContext.MASTER_HABITATIONS_DETAILS on habitation.MAST_HAB_CODE equals habitationDetails.MAST_HAB_CODE
                                      where item.MAST_BLOCK_CODE == masterRoad.MAST_BLOCK_CODE &&
                                      habitationDetails.MAST_HAB_CONNECTED == "N"   //new change done after discussion.
                                      select new
                                      {
                                          habitation.MAST_HAB_NAME,
                                          habitation.MAST_HAB_CODE
                                      });

                List<int> mapHabitations = (from item in dbContext.PLAN_ROAD_HABITATION
                                            join data in dbContext.PLAN_ROAD on item.PLAN_CN_ROAD_CODE equals data.PLAN_CN_ROAD_CODE
                                            where data.MAST_BLOCK_CODE == masterRoad.MAST_BLOCK_CODE
                                            select item.MAST_HAB_CODE).Distinct().ToList<int>();

                var listHab = (from item in lstHabitations
                               where !mapHabitations.Contains(item.MAST_HAB_CODE)
                               select new
                               {
                                   item.MAST_HAB_CODE,
                                   item.MAST_HAB_NAME
                               });

                recordCount = listHab.Count();
                if (recordCount == 0)
                {
                    List<SelectListItem> listHabitation = new List<SelectListItem>();
                    listHabitation.Insert(0, new SelectListItem { Value = "0", Text = "No Habitations to be Benefitted" });
                    return listHabitation;
                }
                else
                {
                    var listHabitation = listHab.ToList().Select(m => new SelectListItem { Text = m.MAST_HAB_NAME, Value = m.MAST_HAB_CODE.ToString() });
                    listHabitation.ToList<SelectListItem>().Insert(0, new SelectListItem { Value = "0", Text = "--Select Habitation--" });
                    return listHabitation.Distinct().ToList<SelectListItem>();
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

        /// <summary>
        /// populates the habitations according to the core network code
        /// </summary>
        /// <param name="roadCode">code network code</param>
        /// <returns></returns>
        public Array GetHabitationsByRoadCode(int roadCode)
        {
            PMGSYEntities dbContext = new PMGSYEntities();
            try
            {
                var lstHabitations = (from item in dbContext.PLAN_ROAD_HABITATION
                                      join habitation in dbContext.MASTER_HABITATIONS on item.MAST_HAB_CODE equals habitation.MAST_HAB_CODE
                                      join habCode in dbContext.MASTER_HABITATIONS_DETAILS on item.MAST_HAB_CODE equals habCode.MAST_HAB_CODE
                                      join roadPlan in dbContext.PLAN_ROAD on item.PLAN_CN_ROAD_CODE equals roadPlan.PLAN_CN_ROAD_CODE
                                      where item.PLAN_CN_ROAD_CODE == roadCode
                                      select new HabitationList
                                      {

                                          MAST_HAB_CODE = habCode.MAST_HAB_CODE,
                                          MAST_HAB_NAME = habitation.MAST_HAB_NAME,
                                          MAST_HAB_TOT_POP = habCode.MAST_HAB_TOT_POP
                                      }).ToArray();

                return lstHabitations;
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
        /// copies the value of view model in entity object
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public PLAN_ROAD CloneModelToObject(CoreNetworkViewModel model)
        {
            PMGSYEntities dbContext = new PMGSYEntities();
            try
            {
                PLAN_ROAD master = new PLAN_ROAD();
                master.MAST_BLOCK_CODE = model.MAST_BLOCK_CODE;
                master.MAST_DISTRICT_CODE = model.MAST_DISTRICT_CODE;
                master.MAST_ER_ROAD_CODE = model.MAST_ER_ROAD_CODE;
                master.MAST_STATE_CODE = model.MAST_STATE_CODE;
                master.PLAN_CN_ROAD_NUMBER = model.PLAN_CN_ROAD_NUMBER;
                master.PLAN_LOCK_STATUS = model.PLAN_LOCK_STATUS == null ? "N" : model.PLAN_LOCK_STATUS;
                master.PLAN_RD_BLOCK_FROM_CODE = model.PLAN_RD_BLOCK_FROM_CODE;
                master.PLAN_RD_BLOCK_TO_CODE = model.PLAN_RD_BLOCK_TO_CODE;
                master.PLAN_RD_FROM_CHAINAGE = Convert.ToDecimal(model.PLAN_RD_FROM_CHAINAGE);
                master.PLAN_RD_FROM_HAB = model.PLAN_RD_FROM_HAB;
                master.PLAN_RD_FROM_TYPE = model.PLAN_RD_FROM_TYPE;
                master.PLAN_RD_LENG = model.PLAN_RD_LENG;

                // added by rohit for vibrant village prog on 20-07-2023
                //master.PLAN_RD_LENGTH = Convert.ToDecimal(model.PLAN_RD_LENGTH);
                master.PLAN_RD_LENGTH = Convert.ToDecimal(model.PLAN_RD_TO_CHAINAGE - model.PLAN_RD_FROM_CHAINAGE);

                master.PLAN_RD_NAME = (model.PLAN_RD_NAME == null ? dbContext.MASTER_EXISTING_ROADS.Where(m => m.MAST_ER_ROAD_CODE == model.MAST_ER_ROAD_CODE).Select(m => m.MAST_ER_ROAD_NAME).FirstOrDefault() : model.PLAN_RD_NAME); //model.PLAN_RD_NAME;
                master.PLAN_RD_ROUTE = model.PLAN_RD_ROUTE;
                master.PLAN_RD_TO_CHAINAGE = Convert.ToDecimal(model.PLAN_RD_TO_CHAINAGE);
                master.PLAN_RD_TO_HAB = model.PLAN_RD_TO_HAB;
                master.PLAN_RD_TO_TYPE = model.PLAN_RD_TO_TYPE;
                switch (model.PLAN_RD_FROM_TYPE)
                {
                    case "B":
                        master.PLAN_RD_FROM = "Block(" + dbContext.MASTER_BLOCK.Where(m => m.MAST_BLOCK_CODE == model.PLAN_RD_BLOCK_FROM_CODE).Select(m => m.MAST_BLOCK_NAME).FirstOrDefault() + ")";
                        master.PLAN_RD_NUM_FROM = model.PLAN_RD_NUM_FROM;
                        master.PLAN_RD_FROM_HAB = null;
                        break;
                    case "H":
                        master.PLAN_RD_FROM = "Habitation(" + (dbContext.MASTER_HABITATIONS.Where(m => m.MAST_HAB_CODE == model.PLAN_RD_FROM_HAB).Select(m => m.MAST_HAB_NAME).FirstOrDefault()) + ")";
                        master.PLAN_RD_NUM_FROM = null;
                        master.PLAN_RD_BLOCK_FROM_CODE = null;
                        break;
                    case "L":
                        master.PLAN_RD_FROM = dbContext.PLAN_ROAD.Where(m => m.PLAN_CN_ROAD_CODE == model.PLAN_RD_NUM_FROM).Select(m => m.PLAN_CN_ROAD_NUMBER).FirstOrDefault();
                        master.PLAN_RD_NUM_FROM = model.PLAN_RD_NUM_FROM;
                        master.PLAN_RD_BLOCK_FROM_CODE = null;
                        master.PLAN_RD_FROM_HAB = null;
                        break;
                    case "M":
                        master.PLAN_RD_FROM = dbContext.PLAN_ROAD.Where(m => m.PLAN_CN_ROAD_CODE == model.PLAN_RD_NUM_FROM).Select(m => m.PLAN_CN_ROAD_NUMBER).FirstOrDefault();
                        master.PLAN_RD_NUM_FROM = model.PLAN_RD_NUM_FROM;
                        master.PLAN_RD_BLOCK_FROM_CODE = null;
                        master.PLAN_RD_FROM_HAB = null;
                        break;
                    case "T":
                        master.PLAN_RD_FROM = dbContext.PLAN_ROAD.Where(m => m.PLAN_CN_ROAD_CODE == model.PLAN_RD_NUM_FROM).Select(m => m.PLAN_CN_ROAD_NUMBER).FirstOrDefault();
                        master.PLAN_RD_NUM_FROM = model.PLAN_RD_NUM_FROM;
                        master.PLAN_RD_BLOCK_FROM_CODE = null;
                        master.PLAN_RD_FROM_HAB = null;
                        break;
                    default:
                        master.PLAN_RD_FROM = dbContext.MASTER_EXISTING_ROADS.Where(m => m.MAST_ER_ROAD_CODE == model.PLAN_RD_NUM_FROM).Select(m => m.MAST_ER_ROAD_NUMBER).FirstOrDefault();
                        master.PLAN_RD_NUM_FROM = model.PLAN_RD_NUM_FROM;
                        master.PLAN_RD_BLOCK_FROM_CODE = null;
                        master.PLAN_RD_FROM_HAB = null;
                        break;
                }

                switch (model.PLAN_RD_TO_TYPE)
                {
                    case "B":
                        master.PLAN_RD_TO = "Block(" + dbContext.MASTER_BLOCK.Where(m => m.MAST_BLOCK_CODE == model.PLAN_RD_BLOCK_TO_CODE).Select(m => m.MAST_BLOCK_NAME).FirstOrDefault() + ")";
                        master.PLAN_RD_NUM_TO = model.PLAN_RD_NUM_TO;
                        master.PLAN_RD_TO_HAB = null;
                        break;
                    case "H":
                        master.PLAN_RD_TO = "Habitation(" + (dbContext.MASTER_HABITATIONS.Where(m => m.MAST_HAB_CODE == model.PLAN_RD_TO_HAB).Select(m => m.MAST_HAB_NAME).FirstOrDefault()) + ")";
                        master.PLAN_RD_NUM_TO = null;
                        master.PLAN_RD_BLOCK_TO_CODE = null;
                        break;
                    case "L":
                        master.PLAN_RD_TO = dbContext.PLAN_ROAD.Where(m => m.PLAN_CN_ROAD_CODE == model.PLAN_RD_NUM_TO).Select(m => m.PLAN_CN_ROAD_NUMBER).FirstOrDefault();
                        master.PLAN_RD_NUM_TO = model.PLAN_RD_NUM_TO;
                        master.PLAN_RD_BLOCK_TO_CODE = null;
                        master.PLAN_RD_TO_HAB = null;
                        break;
                    case "M":
                        master.PLAN_RD_TO = dbContext.PLAN_ROAD.Where(m => m.PLAN_CN_ROAD_CODE == model.PLAN_RD_NUM_TO).Select(m => m.PLAN_CN_ROAD_NUMBER).FirstOrDefault();
                        master.PLAN_RD_NUM_TO = model.PLAN_RD_NUM_TO;
                        master.PLAN_RD_BLOCK_TO_CODE = null;
                        master.PLAN_RD_TO_HAB = null;
                        break;
                    case "T":
                        master.PLAN_RD_TO = dbContext.PLAN_ROAD.Where(m => m.PLAN_CN_ROAD_CODE == model.PLAN_RD_NUM_TO).Select(m => m.PLAN_CN_ROAD_NUMBER).FirstOrDefault();
                        master.PLAN_RD_NUM_TO = model.PLAN_RD_NUM_TO;
                        master.PLAN_RD_BLOCK_TO_CODE = null;
                        master.PLAN_RD_TO_HAB = null;
                        break;
                    default:
                        master.PLAN_RD_TO = dbContext.MASTER_EXISTING_ROADS.Where(m => m.MAST_ER_ROAD_CODE == model.PLAN_RD_NUM_TO).Select(m => m.MAST_ER_ROAD_NUMBER).FirstOrDefault();
                        master.PLAN_RD_BLOCK_TO_CODE = null;
                        master.PLAN_RD_NUM_TO = model.PLAN_RD_NUM_TO;
                        master.PLAN_RD_TO_HAB = null;
                        break;
                }

                // added by rohit for vibrant village prog on 20-07-2023
                if (PMGSYSession.Current.PMGSYScheme == 2 || PMGSYSession.Current.PMGSYScheme == 5)
                {
                    master.PLAN_RD_TOTAL_LEN = model.TotalLengthOfCandidate;
                    master.MAST_PMGSY_SCHEME = PMGSYSession.Current.PMGSYScheme;
                }
                else
                {
                    master.PLAN_RD_TOTAL_LEN = null;
                    master.MAST_PMGSY_SCHEME = PMGSYSession.Current.PMGSYScheme;
                }

                return master;
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
        /// copies the value of entity object into view model
        /// </summary>
        /// <param name="master"></param>
        /// <returns></returns>
        public CoreNetworkViewModel CloneObjectToModel(PLAN_ROAD master)
        {
            PMGSYEntities dbContext = new PMGSYEntities();
            try
            {
                CoreNetworkViewModel model = new CoreNetworkViewModel();
                model.MAST_BLOCK_CODE = master.MAST_BLOCK_CODE;
                model.MAST_DISTRICT_CODE = master.MAST_DISTRICT_CODE;
                model.MAST_ER_ROAD_CODE = master.MAST_ER_ROAD_CODE;
                model.MAST_STATE_CODE = master.MAST_ER_ROAD_CODE;
                model.PLAN_CN_ROAD_CODE = master.PLAN_CN_ROAD_CODE;
                model.PLAN_CN_ROAD_NUMBER = master.PLAN_CN_ROAD_NUMBER;
                model.PLAN_LOCK_STATUS = master.PLAN_LOCK_STATUS;
                model.PLAN_RD_BLOCK_FROM_CODE = master.PLAN_RD_BLOCK_FROM_CODE;
                model.PLAN_RD_BLOCK_TO_CODE = master.PLAN_RD_BLOCK_TO_CODE;
                model.PLAN_RD_FROM = master.PLAN_RD_FROM;
                model.PLAN_RD_FROM_CHAINAGE = Convert.ToDouble(master.PLAN_RD_FROM_CHAINAGE);
                model.PLAN_RD_FROM_HAB = master.PLAN_RD_FROM_HAB;
                model.PLAN_RD_FROM_TYPE = master.PLAN_RD_FROM_TYPE;
                model.PLAN_RD_LENG = master.PLAN_RD_LENG;
                model.PLAN_RD_LENGTH = Convert.ToDouble(master.PLAN_RD_LENGTH);
                model.PLAN_RD_NAME = master.PLAN_RD_NAME;
                model.PLAN_RD_NUM_FROM = master.PLAN_RD_NUM_FROM;
                model.PLAN_RD_NUM_TO = master.PLAN_RD_NUM_TO;
                model.PLAN_RD_ROUTE = master.PLAN_RD_ROUTE;
                model.PLAN_RD_TO = master.PLAN_RD_TO;
                model.PLAN_RD_TO_CHAINAGE = Convert.ToDouble(master.PLAN_RD_TO_CHAINAGE);
                model.PLAN_RD_TO_HAB = master.PLAN_RD_TO_HAB;
                model.PLAN_RD_TO_TYPE = master.PLAN_RD_TO_TYPE;
                return model;
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
        /// returns the list for road number dropdown
        /// </summary>
        /// <param name="roadFrom"></param>
        /// <param name="blockCode"></param>
        /// <returns></returns>
        public List<MASTER_EXISTING_ROADS> GetRoadNumFromByRoadFrom(string roadFrom, int blockCode)
        {
            PMGSYEntities dbContext = new PMGSYEntities();
            try
            {
                List<MASTER_EXISTING_ROADS> lstRoads = null;
                int catCode = 0;
                switch (roadFrom)
                {
                    /*
                     case "D":
                         catCode = (from item in dbContext.MASTER_ROAD_CATEGORY where item.MAST_ROAD_SHORT_DESC == "MDR" select item.MAST_ROAD_CAT_CODE).FirstOrDefault();
                         //lstRoads = dbContext.MASTER_EXISTING_ROADS.Where(item => item.MAST_ROAD_CAT_CODE == catCode && item.MAST_BLOCK_CODE == blockCode && item.MAST_PMGSY_SCHEME == PMGSYSession.Current.PMGSYScheme).OrderBy(m => m.MAST_ER_ROAD_NAME).ToList<MASTER_EXISTING_ROADS>();
                         ///Changes for RCPLWE/PMGSY3
                         lstRoads = dbContext.MASTER_EXISTING_ROADS.Where(item => item.MAST_ROAD_CAT_CODE == catCode && item.MAST_BLOCK_CODE == blockCode && item.MAST_PMGSY_SCHEME == (PMGSYSession.Current.PMGSYScheme == 3 ? (byte)1 : PMGSYSession.Current.PMGSYScheme == 4 ? (byte)2 : PMGSYSession.Current.PMGSYScheme)).OrderBy(m => m.MAST_ER_ROAD_NAME).ToList<MASTER_EXISTING_ROADS>();
                         break;
                     case "N":
                         catCode = (from item in dbContext.MASTER_ROAD_CATEGORY where item.MAST_ROAD_SHORT_DESC == "NH" select item.MAST_ROAD_CAT_CODE).FirstOrDefault();
                         //lstRoads = dbContext.MASTER_EXISTING_ROADS.Where(item => item.MAST_ROAD_CAT_CODE == catCode && item.MAST_BLOCK_CODE == blockCode && item.MAST_PMGSY_SCHEME == PMGSYSession.Current.PMGSYScheme).OrderBy(m => m.MAST_ER_ROAD_NAME).ToList<MASTER_EXISTING_ROADS>();
                         ///Changes for RCPLWE/PMGSY3
                         lstRoads = dbContext.MASTER_EXISTING_ROADS.Where(item => item.MAST_ROAD_CAT_CODE == catCode && item.MAST_BLOCK_CODE == blockCode && item.MAST_PMGSY_SCHEME == (PMGSYSession.Current.PMGSYScheme == 3 ? (byte)1 : PMGSYSession.Current.PMGSYScheme == 4 ? (byte)2 : PMGSYSession.Current.PMGSYScheme)).OrderBy(m => m.MAST_ER_ROAD_NAME).ToList<MASTER_EXISTING_ROADS>();
                         break;
                     case "R":
                         catCode = (from item in dbContext.MASTER_ROAD_CATEGORY where item.MAST_ROAD_SHORT_DESC == "RR(ODR)" select item.MAST_ROAD_CAT_CODE).FirstOrDefault();
                         //lstRoads = dbContext.MASTER_EXISTING_ROADS.Where(item => item.MAST_ROAD_CAT_CODE == catCode && item.MAST_BLOCK_CODE == blockCode && item.MAST_PMGSY_SCHEME == PMGSYSession.Current.PMGSYScheme).OrderBy(m => m.MAST_ER_ROAD_NAME).ToList<MASTER_EXISTING_ROADS>();
                         ///Changes for RCPLWE/PMGSY3
                         lstRoads = dbContext.MASTER_EXISTING_ROADS.Where(item => item.MAST_ROAD_CAT_CODE == catCode && item.MAST_BLOCK_CODE == blockCode && item.MAST_PMGSY_SCHEME == (PMGSYSession.Current.PMGSYScheme == 3 ? (byte)1 : PMGSYSession.Current.PMGSYScheme == 4 ? (byte)2 : PMGSYSession.Current.PMGSYScheme)).OrderBy(m => m.MAST_ER_ROAD_NAME).ToList<MASTER_EXISTING_ROADS>();
                         break;
                     case "Z":
                         catCode = (from item in dbContext.MASTER_ROAD_CATEGORY where item.MAST_ROAD_SHORT_DESC == "RR(TRACK)" select item.MAST_ROAD_CAT_CODE).FirstOrDefault();
                         //lstRoads = dbContext.MASTER_EXISTING_ROADS.Where(item => item.MAST_ROAD_CAT_CODE == catCode && item.MAST_BLOCK_CODE == blockCode && item.MAST_PMGSY_SCHEME == PMGSYSession.Current.PMGSYScheme).OrderBy(m => m.MAST_ER_ROAD_NAME).ToList<MASTER_EXISTING_ROADS>();
                         ///Changes for RCPLWE/PMGSY3
                         lstRoads = dbContext.MASTER_EXISTING_ROADS.Where(item => item.MAST_ROAD_CAT_CODE == catCode && item.MAST_BLOCK_CODE == blockCode && item.MAST_PMGSY_SCHEME == (PMGSYSession.Current.PMGSYScheme == 3 ? (byte)1 : PMGSYSession.Current.PMGSYScheme == 4 ? (byte)2 : PMGSYSession.Current.PMGSYScheme)).OrderBy(m => m.MAST_ER_ROAD_NAME).ToList<MASTER_EXISTING_ROADS>();
                         break;
                     case "S":
                         catCode = (from item in dbContext.MASTER_ROAD_CATEGORY where item.MAST_ROAD_SHORT_DESC == "SH" select item.MAST_ROAD_CAT_CODE).FirstOrDefault();
                         //lstRoads = dbContext.MASTER_EXISTING_ROADS.Where(item => item.MAST_ROAD_CAT_CODE == catCode && item.MAST_BLOCK_CODE == blockCode && item.MAST_PMGSY_SCHEME == PMGSYSession.Current.PMGSYScheme).OrderBy(m => m.MAST_ER_ROAD_NAME).ToList<MASTER_EXISTING_ROADS>();
                         ///Changes for RCPLWE/PMGSY3
                         lstRoads = dbContext.MASTER_EXISTING_ROADS.Where(item => item.MAST_ROAD_CAT_CODE == catCode && item.MAST_BLOCK_CODE == blockCode && item.MAST_PMGSY_SCHEME == (PMGSYSession.Current.PMGSYScheme == 3 ? (byte)1 : PMGSYSession.Current.PMGSYScheme == 4 ? (byte)2 : PMGSYSession.Current.PMGSYScheme)).OrderBy(m => m.MAST_ER_ROAD_NAME).ToList<MASTER_EXISTING_ROADS>();
                         break;
                     case "V":
                         catCode = (from item in dbContext.MASTER_ROAD_CATEGORY where item.MAST_ROAD_SHORT_DESC == "RR(VR)" select item.MAST_ROAD_CAT_CODE).FirstOrDefault();
                         //lstRoads = dbContext.MASTER_EXISTING_ROADS.Where(item => item.MAST_ROAD_CAT_CODE == catCode && item.MAST_BLOCK_CODE == blockCode && item.MAST_PMGSY_SCHEME == PMGSYSession.Current.PMGSYScheme).OrderBy(m => m.MAST_ER_ROAD_NAME).ToList<MASTER_EXISTING_ROADS>();
                         ///Changes for RCPLWE/PMGSY3
                         lstRoads = dbContext.MASTER_EXISTING_ROADS.Where(item => item.MAST_ROAD_CAT_CODE == catCode && item.MAST_BLOCK_CODE == blockCode && item.MAST_PMGSY_SCHEME == (PMGSYSession.Current.PMGSYScheme == 3 ? (byte)1 : PMGSYSession.Current.PMGSYScheme == 4 ? (byte)2 : PMGSYSession.Current.PMGSYScheme)).OrderBy(m => m.MAST_ER_ROAD_NAME).ToList<MASTER_EXISTING_ROADS>();
                         break;
                     case "O":
                         catCode = (from item in dbContext.MASTER_ROAD_CATEGORY where item.MAST_ROAD_SHORT_DESC == "OT" select item.MAST_ROAD_CAT_CODE).FirstOrDefault();
                         //lstRoads = dbContext.MASTER_EXISTING_ROADS.Where(item => item.MAST_ROAD_CAT_CODE == catCode && item.MAST_BLOCK_CODE == blockCode && item.MAST_PMGSY_SCHEME == PMGSYSession.Current.PMGSYScheme).OrderBy(m => m.MAST_ER_ROAD_NAME).ToList<MASTER_EXISTING_ROADS>();
                         ///Changes for RCPLWE/PMGSY3
                         lstRoads = dbContext.MASTER_EXISTING_ROADS.Where(item => item.MAST_ROAD_CAT_CODE == catCode && item.MAST_BLOCK_CODE == blockCode && item.MAST_PMGSY_SCHEME == (PMGSYSession.Current.PMGSYScheme == 3 ? (byte)1 : PMGSYSession.Current.PMGSYScheme == 4 ? (byte)2 : PMGSYSession.Current.PMGSYScheme)).OrderBy(m => m.MAST_ER_ROAD_NAME).ToList<MASTER_EXISTING_ROADS>();
                         break;

                     */

                    // added by rohit for vibrant village prog on 20-07-2023

                    case "D":
                        catCode = (from item in dbContext.MASTER_ROAD_CATEGORY where item.MAST_ROAD_SHORT_DESC == "MDR" select item.MAST_ROAD_CAT_CODE).FirstOrDefault();
                        //lstRoads = dbContext.MASTER_EXISTING_ROADS.Where(item => item.MAST_ROAD_CAT_CODE == catCode && item.MAST_BLOCK_CODE == blockCode && item.MAST_PMGSY_SCHEME == PMGSYSession.Current.PMGSYScheme).OrderBy(m => m.MAST_ER_ROAD_NAME).ToList<MASTER_EXISTING_ROADS>();
                        ///Changes for RCPLWE/PMGSY3
                        lstRoads = dbContext.MASTER_EXISTING_ROADS.Where(item => item.MAST_ROAD_CAT_CODE == catCode && item.MAST_BLOCK_CODE == blockCode && item.MAST_PMGSY_SCHEME == (PMGSYSession.Current.PMGSYScheme == 3 ? (byte)1 : (PMGSYSession.Current.PMGSYScheme == 4 || PMGSYSession.Current.PMGSYScheme == 5) ? (byte)2 : PMGSYSession.Current.PMGSYScheme)).OrderBy(m => m.MAST_ER_ROAD_NAME).ToList<MASTER_EXISTING_ROADS>();
                        break;
                    case "N":
                        catCode = (from item in dbContext.MASTER_ROAD_CATEGORY where item.MAST_ROAD_SHORT_DESC == "NH" select item.MAST_ROAD_CAT_CODE).FirstOrDefault();
                        //lstRoads = dbContext.MASTER_EXISTING_ROADS.Where(item => item.MAST_ROAD_CAT_CODE == catCode && item.MAST_BLOCK_CODE == blockCode && item.MAST_PMGSY_SCHEME == PMGSYSession.Current.PMGSYScheme).OrderBy(m => m.MAST_ER_ROAD_NAME).ToList<MASTER_EXISTING_ROADS>();
                        ///Changes for RCPLWE/PMGSY3
                        lstRoads = dbContext.MASTER_EXISTING_ROADS.Where(item => item.MAST_ROAD_CAT_CODE == catCode && item.MAST_BLOCK_CODE == blockCode && item.MAST_PMGSY_SCHEME == (PMGSYSession.Current.PMGSYScheme == 3 ? (byte)1 : (PMGSYSession.Current.PMGSYScheme == 4 || PMGSYSession.Current.PMGSYScheme == 5) ? (byte)2 : PMGSYSession.Current.PMGSYScheme)).OrderBy(m => m.MAST_ER_ROAD_NAME).ToList<MASTER_EXISTING_ROADS>();
                        break;
                    case "R":
                        catCode = (from item in dbContext.MASTER_ROAD_CATEGORY where item.MAST_ROAD_SHORT_DESC == "RR(ODR)" select item.MAST_ROAD_CAT_CODE).FirstOrDefault();
                        //lstRoads = dbContext.MASTER_EXISTING_ROADS.Where(item => item.MAST_ROAD_CAT_CODE == catCode && item.MAST_BLOCK_CODE == blockCode && item.MAST_PMGSY_SCHEME == PMGSYSession.Current.PMGSYScheme).OrderBy(m => m.MAST_ER_ROAD_NAME).ToList<MASTER_EXISTING_ROADS>();
                        ///Changes for RCPLWE/PMGSY3
                        lstRoads = dbContext.MASTER_EXISTING_ROADS.Where(item => item.MAST_ROAD_CAT_CODE == catCode && item.MAST_BLOCK_CODE == blockCode && item.MAST_PMGSY_SCHEME == (PMGSYSession.Current.PMGSYScheme == 3 ? (byte)1 : (PMGSYSession.Current.PMGSYScheme == 4 || PMGSYSession.Current.PMGSYScheme == 5) ? (byte)2 : PMGSYSession.Current.PMGSYScheme)).OrderBy(m => m.MAST_ER_ROAD_NAME).ToList<MASTER_EXISTING_ROADS>();
                        break;
                    case "Z":
                        catCode = (from item in dbContext.MASTER_ROAD_CATEGORY where item.MAST_ROAD_SHORT_DESC == "RR(TRACK)" select item.MAST_ROAD_CAT_CODE).FirstOrDefault();
                        //lstRoads = dbContext.MASTER_EXISTING_ROADS.Where(item => item.MAST_ROAD_CAT_CODE == catCode && item.MAST_BLOCK_CODE == blockCode && item.MAST_PMGSY_SCHEME == PMGSYSession.Current.PMGSYScheme).OrderBy(m => m.MAST_ER_ROAD_NAME).ToList<MASTER_EXISTING_ROADS>();
                        ///Changes for RCPLWE/PMGSY3
                        lstRoads = dbContext.MASTER_EXISTING_ROADS.Where(item => item.MAST_ROAD_CAT_CODE == catCode && item.MAST_BLOCK_CODE == blockCode && item.MAST_PMGSY_SCHEME == (PMGSYSession.Current.PMGSYScheme == 3 ? (byte)1 : (PMGSYSession.Current.PMGSYScheme == 4 || PMGSYSession.Current.PMGSYScheme == 5) ? (byte)2 : PMGSYSession.Current.PMGSYScheme)).OrderBy(m => m.MAST_ER_ROAD_NAME).ToList<MASTER_EXISTING_ROADS>();
                        break;
                    case "S":
                        catCode = (from item in dbContext.MASTER_ROAD_CATEGORY where item.MAST_ROAD_SHORT_DESC == "SH" select item.MAST_ROAD_CAT_CODE).FirstOrDefault();
                        //lstRoads = dbContext.MASTER_EXISTING_ROADS.Where(item => item.MAST_ROAD_CAT_CODE == catCode && item.MAST_BLOCK_CODE == blockCode && item.MAST_PMGSY_SCHEME == PMGSYSession.Current.PMGSYScheme).OrderBy(m => m.MAST_ER_ROAD_NAME).ToList<MASTER_EXISTING_ROADS>();
                        ///Changes for RCPLWE/PMGSY3
                        lstRoads = dbContext.MASTER_EXISTING_ROADS.Where(item => item.MAST_ROAD_CAT_CODE == catCode && item.MAST_BLOCK_CODE == blockCode && item.MAST_PMGSY_SCHEME == (PMGSYSession.Current.PMGSYScheme == 3 ? (byte)1 : (PMGSYSession.Current.PMGSYScheme == 4 || PMGSYSession.Current.PMGSYScheme == 5) ? (byte)2 : PMGSYSession.Current.PMGSYScheme)).OrderBy(m => m.MAST_ER_ROAD_NAME).ToList<MASTER_EXISTING_ROADS>();
                        break;
                    case "V":
                        catCode = (from item in dbContext.MASTER_ROAD_CATEGORY where item.MAST_ROAD_SHORT_DESC == "RR(VR)" select item.MAST_ROAD_CAT_CODE).FirstOrDefault();
                        //lstRoads = dbContext.MASTER_EXISTING_ROADS.Where(item => item.MAST_ROAD_CAT_CODE == catCode && item.MAST_BLOCK_CODE == blockCode && item.MAST_PMGSY_SCHEME == PMGSYSession.Current.PMGSYScheme).OrderBy(m => m.MAST_ER_ROAD_NAME).ToList<MASTER_EXISTING_ROADS>();
                        ///Changes for RCPLWE/PMGSY3
                        lstRoads = dbContext.MASTER_EXISTING_ROADS.Where(item => item.MAST_ROAD_CAT_CODE == catCode && item.MAST_BLOCK_CODE == blockCode && item.MAST_PMGSY_SCHEME == (PMGSYSession.Current.PMGSYScheme == 3 ? (byte)1 : (PMGSYSession.Current.PMGSYScheme == 4 || PMGSYSession.Current.PMGSYScheme == 5) ? (byte)2 : PMGSYSession.Current.PMGSYScheme)).OrderBy(m => m.MAST_ER_ROAD_NAME).ToList<MASTER_EXISTING_ROADS>();
                        break;
                    case "O":
                        catCode = (from item in dbContext.MASTER_ROAD_CATEGORY where item.MAST_ROAD_SHORT_DESC == "OT" select item.MAST_ROAD_CAT_CODE).FirstOrDefault();
                        //lstRoads = dbContext.MASTER_EXISTING_ROADS.Where(item => item.MAST_ROAD_CAT_CODE == catCode && item.MAST_BLOCK_CODE == blockCode && item.MAST_PMGSY_SCHEME == PMGSYSession.Current.PMGSYScheme).OrderBy(m => m.MAST_ER_ROAD_NAME).ToList<MASTER_EXISTING_ROADS>();
                        ///Changes for RCPLWE/PMGSY3
                        lstRoads = dbContext.MASTER_EXISTING_ROADS.Where(item => item.MAST_ROAD_CAT_CODE == catCode && item.MAST_BLOCK_CODE == blockCode && item.MAST_PMGSY_SCHEME == (PMGSYSession.Current.PMGSYScheme == 3 ? (byte)1 : (PMGSYSession.Current.PMGSYScheme == 4 || PMGSYSession.Current.PMGSYScheme == 5) ? (byte)2 : PMGSYSession.Current.PMGSYScheme)).OrderBy(m => m.MAST_ER_ROAD_NAME).ToList<MASTER_EXISTING_ROADS>();
                        break;

                }
                return lstRoads;
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "GetRoadNumFromByRoadFrom().DAL");
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
        /// returns through routes and link routes of current block
        /// </summary>
        /// <param name="roadFrom"></param>
        /// <param name="blockCode"></param>
        /// <returns></returns>
        public List<PLAN_ROAD> GetRoadNumFromThroughList(string roadFrom, int blockCode)
        {
            PMGSYEntities dbContext = new PMGSYEntities();
            try
            {
                List<PLAN_ROAD> lstThroughRoads = null;
                switch (roadFrom)
                {
                    /*
                    case "T":
                        //lstThroughRoads = dbContext.PLAN_ROAD.Where(item => item.PLAN_RD_ROUTE == "T" && item.MAST_BLOCK_CODE == blockCode && item.MAST_PMGSY_SCHEME == PMGSYSession.Current.PMGSYScheme).OrderBy(m => m.PLAN_CN_ROAD_NUMBER).ToList<PLAN_ROAD>();
                        ///Changes for RCPLWE/PMGSY3
                        lstThroughRoads = dbContext.PLAN_ROAD.Where(item => item.PLAN_RD_ROUTE == "T" && item.MAST_BLOCK_CODE == blockCode && item.MAST_PMGSY_SCHEME == (PMGSYSession.Current.PMGSYScheme == 3 ? (byte)1 :
                            PMGSYSession.Current.PMGSYScheme == 4 ? (byte)2 : PMGSYSession.Current.PMGSYScheme)).OrderBy(m => m.PLAN_CN_ROAD_NUMBER).ToList<PLAN_ROAD>();
                        break;
                    case "L":
                        //lstThroughRoads = dbContext.PLAN_ROAD.Where(item => item.PLAN_RD_ROUTE == "L" && item.MAST_BLOCK_CODE == blockCode && item.MAST_PMGSY_SCHEME == PMGSYSession.Current.PMGSYScheme).OrderBy(m => m.PLAN_CN_ROAD_NUMBER).ToList<PLAN_ROAD>();
                        ///Changes for RCPLWE/PMGSY3
                        lstThroughRoads = dbContext.PLAN_ROAD.Where(item => item.PLAN_RD_ROUTE == "L" && item.MAST_BLOCK_CODE == blockCode && item.MAST_PMGSY_SCHEME == (PMGSYSession.Current.PMGSYScheme == 3 ? (byte)1 :
                            PMGSYSession.Current.PMGSYScheme == 4 ? (byte)2 : PMGSYSession.Current.PMGSYScheme)).OrderBy(m => m.PLAN_CN_ROAD_NUMBER).ToList<PLAN_ROAD>();
                        break;
                    case "M":
                        //lstThroughRoads = dbContext.PLAN_ROAD.Where(item => item.PLAN_RD_ROUTE == "M" && item.MAST_BLOCK_CODE == blockCode && item.MAST_PMGSY_SCHEME == PMGSYSession.Current.PMGSYScheme).OrderBy(m => m.PLAN_CN_ROAD_NUMBER).ToList<PLAN_ROAD>();
                        ///Changes for RCPLWE/PMGSY3
                        lstThroughRoads = dbContext.PLAN_ROAD.Where(item => item.PLAN_RD_ROUTE == "M" && item.MAST_BLOCK_CODE == blockCode && item.MAST_PMGSY_SCHEME == (PMGSYSession.Current.PMGSYScheme == 3 ? (byte)1 :
                            PMGSYSession.Current.PMGSYScheme == 4 ? (byte)2 : PMGSYSession.Current.PMGSYScheme)).OrderBy(m => m.PLAN_CN_ROAD_NUMBER).ToList<PLAN_ROAD>();
                        break;
                    default:
                        break;
                   
                    */

                    // new code added by rohit for vibrant village prog on 20-07-2023
                    case "T":
                        //lstThroughRoads = dbContext.PLAN_ROAD.Where(item => item.PLAN_RD_ROUTE == "T" && item.MAST_BLOCK_CODE == blockCode && item.MAST_PMGSY_SCHEME == PMGSYSession.Current.PMGSYScheme).OrderBy(m => m.PLAN_CN_ROAD_NUMBER).ToList<PLAN_ROAD>();
                        ///Changes for RCPLWE/PMGSY3
                        lstThroughRoads = dbContext.PLAN_ROAD.Where(item => item.PLAN_RD_ROUTE == "T" && item.MAST_BLOCK_CODE == blockCode && item.MAST_PMGSY_SCHEME == (PMGSYSession.Current.PMGSYScheme == 3 ? (byte)1 :
                            (PMGSYSession.Current.PMGSYScheme == 4 || PMGSYSession.Current.PMGSYScheme == 5) ? (byte)2 : PMGSYSession.Current.PMGSYScheme)).OrderBy(m => m.PLAN_CN_ROAD_NUMBER).ToList<PLAN_ROAD>();
                        break;
                    case "L":
                        //lstThroughRoads = dbContext.PLAN_ROAD.Where(item => item.PLAN_RD_ROUTE == "L" && item.MAST_BLOCK_CODE == blockCode && item.MAST_PMGSY_SCHEME == PMGSYSession.Current.PMGSYScheme).OrderBy(m => m.PLAN_CN_ROAD_NUMBER).ToList<PLAN_ROAD>();
                        ///Changes for RCPLWE/PMGSY3
                        lstThroughRoads = dbContext.PLAN_ROAD.Where(item => item.PLAN_RD_ROUTE == "L" && item.MAST_BLOCK_CODE == blockCode && item.MAST_PMGSY_SCHEME == (PMGSYSession.Current.PMGSYScheme == 3 ? (byte)1 :
                            (PMGSYSession.Current.PMGSYScheme == 4 || PMGSYSession.Current.PMGSYScheme == 5) ? (byte)2 : PMGSYSession.Current.PMGSYScheme)).OrderBy(m => m.PLAN_CN_ROAD_NUMBER).ToList<PLAN_ROAD>();
                        break;
                    case "M":
                        //lstThroughRoads = dbContext.PLAN_ROAD.Where(item => item.PLAN_RD_ROUTE == "M" && item.MAST_BLOCK_CODE == blockCode && item.MAST_PMGSY_SCHEME == PMGSYSession.Current.PMGSYScheme).OrderBy(m => m.PLAN_CN_ROAD_NUMBER).ToList<PLAN_ROAD>();
                        ///Changes for RCPLWE/PMGSY3
                        lstThroughRoads = dbContext.PLAN_ROAD.Where(item => item.PLAN_RD_ROUTE == "M" && item.MAST_BLOCK_CODE == blockCode && item.MAST_PMGSY_SCHEME == (PMGSYSession.Current.PMGSYScheme == 3 ? (byte)1 :
                            (PMGSYSession.Current.PMGSYScheme == 4 || PMGSYSession.Current.PMGSYScheme == 5) ? (byte)2 : PMGSYSession.Current.PMGSYScheme)).OrderBy(m => m.PLAN_CN_ROAD_NUMBER).ToList<PLAN_ROAD>();
                        break;
                    default:
                        break;

                }

                return lstThroughRoads;
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "GetRoadNumFromThroughList().DAL");
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
        /// returns the value to be selected when editing the old data
        /// </summary>
        /// <param name="roadFrom">road route type</param>
        /// <param name="blockCode">block code</param>
        /// <param name="roadCode">core network id</param>
        /// <returns></returns>
        public List<SelectListItem> GetRoadNumFromThroughEditList(string roadFrom, int blockCode, int roadCode)
        {
            PMGSYEntities dbContext = new PMGSYEntities();
            try
            {
                List<SelectListItem> lstThroughRoads = null;
                switch (roadFrom)
                {
                    case "T":
                        //lstThroughRoads = dbContext.PLAN_ROAD.Where(item => item.PLAN_CN_ROAD_NUMBER.StartsWith(roadFrom) && item.MAST_BLOCK_CODE == blockCode).OrderBy(m => m.PLAN_CN_ROAD_NUMBER).ToList<SelectListItem>();
                        lstThroughRoads = new SelectList(dbContext.PLAN_ROAD.Where(m => m.PLAN_RD_ROUTE == "T" && m.MAST_BLOCK_CODE == blockCode && m.MAST_PMGSY_SCHEME == PMGSYSession.Current.PMGSYScheme), "PLAN_CN_ROAD_CODE", "PLAN_CN_ROAD_NUMBER").ToList();
                        break;
                    case "L":
                        lstThroughRoads = new SelectList(dbContext.PLAN_ROAD.Where(m => m.PLAN_RD_ROUTE == "L" && m.MAST_BLOCK_CODE == blockCode && m.MAST_PMGSY_SCHEME == PMGSYSession.Current.PMGSYScheme), "PLAN_CN_ROAD_CODE", "PLAN_CN_ROAD_NUMBER").ToList();
                        break;
                    case "M":
                        lstThroughRoads = new SelectList(dbContext.PLAN_ROAD.Where(m => m.PLAN_RD_ROUTE == "M" && m.MAST_BLOCK_CODE == blockCode && m.MAST_PMGSY_SCHEME == PMGSYSession.Current.PMGSYScheme), "PLAN_CN_ROAD_CODE", "PLAN_CN_ROAD_NUMBER").ToList();
                        break;
                    default:
                        break;
                }

                PLAN_ROAD master = dbContext.PLAN_ROAD.Find(roadCode);
                if (master.PLAN_RD_FROM_TYPE == "T" && master.PLAN_RD_NUM_FROM == null)
                {
                    lstThroughRoads.Add(new SelectListItem { Value = "1", Text = master.PLAN_RD_FROM, Selected = true });
                }

                if (master.PLAN_RD_TO_TYPE == "T" && master.PLAN_RD_NUM_TO == null)
                {
                    lstThroughRoads.Add(new SelectListItem { Value = "1", Text = master.PLAN_RD_TO, Selected = true });
                }

                if (master.PLAN_RD_TO_TYPE == "M" && master.PLAN_RD_NUM_TO == null)
                {
                    lstThroughRoads.Add(new SelectListItem { Value = "1", Text = master.PLAN_RD_TO, Selected = true });
                }

                return lstThroughRoads.ToList<SelectListItem>();
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
        /// returns the road category name according to the road code
        /// </summary>
        /// <param name="roadCode"></param>
        /// <returns></returns>
        public string GetRoadCategory(int roadCode)
        {
            PMGSYEntities dbContext = new PMGSYEntities();
            try
            {
                MASTER_EXISTING_ROADS master = dbContext.MASTER_EXISTING_ROADS.Find(roadCode);
                MASTER_ROAD_CATEGORY roadMaster = dbContext.MASTER_ROAD_CATEGORY.Find(master.MAST_ROAD_CAT_CODE);
                return roadMaster.MAST_ROAD_CAT_NAME;
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
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

        /// <summary>
        /// returns the list of habitations for populating the grid data
        /// </summary>
        /// <param name="roadCode"></param>
        /// <param name="page"></param>
        /// <param name="rows"></param>
        /// <param name="sidx"></param>
        /// <param name="sord"></param>
        /// <param name="totalRecords"></param>
        /// <returns></returns>
        public Array GetHabitationList(int roadCode, string flag, int page, int rows, string sidx, string sord, out long totalRecords)
        {
            PMGSYEntities dbContext = new PMGSYEntities();
            try
            {
                var lstHabitations = (from item in dbContext.PLAN_ROAD_HABITATION
                                      join habitation in dbContext.MASTER_HABITATIONS on item.MAST_HAB_CODE equals habitation.MAST_HAB_CODE
                                      join habCode in dbContext.MASTER_HABITATIONS_DETAILS on item.MAST_HAB_CODE equals habCode.MAST_HAB_CODE
                                      join roadPlan in dbContext.PLAN_ROAD on item.PLAN_CN_ROAD_CODE equals roadPlan.PLAN_CN_ROAD_CODE
                                      where item.PLAN_CN_ROAD_CODE == roadCode &&
                                      habCode.MAST_YEAR == (PMGSYSession.Current.PMGSYScheme == 1 ? 2001 : 2011)
                                      select new
                                      {
                                          roadPlan.PLAN_CN_ROAD_NUMBER,
                                          roadPlan.PLAN_LOCK_STATUS,
                                          habitation.MAST_HAB_NAME,
                                          habCode.MAST_HAB_CODE,
                                          habCode.MAST_BUS_SERVICE,
                                          habCode.MAST_DEGREE_COLLEGE,
                                          habCode.MAST_DISPENSARY,
                                          habCode.MAST_ELECTRICTY,
                                          habCode.MAST_HAB_CONNECTED,
                                          habCode.MAST_HAB_SCST_POP,
                                          habCode.MAST_HAB_TOT_POP,
                                          habCode.MAST_HEALTH_SERVICE,
                                          habCode.MAST_HIGH_SCHOOL,
                                          habCode.MAST_INTERMEDIATE_SCHOOL,
                                          habCode.MAST_MCW_CENTERS,
                                          habCode.MAST_MIDDLE_SCHOOL,
                                          habCode.MAST_PANCHAYAT_HQ,
                                          habCode.MAST_PHCS,
                                          habCode.MAST_PRIMARY_SCHOOL,
                                          habCode.MAST_RAILWAY_STATION,
                                          habCode.MAST_SCHEME,
                                          habCode.MAST_TELEGRAPH_OFFICE,
                                          habCode.MAST_TELEPHONE_CONNECTION,
                                          habCode.MAST_TOURIST_PLACE,
                                          habCode.MAST_VETNARY_HOSPITAL,
                                          habCode.MAST_YEAR,
                                          roadPlan.MAST_BLOCK_CODE,
                                          habitation.MASTER_VILLAGE.MAST_VILLAGE_NAME,
                                          roadPlan.PLAN_CN_ROAD_CODE,
                                          habitation.MASTER_VILLAGE.MASTER_BLOCK.MAST_BLOCK_NAME
                                          //roadPlan.MASTER_BLOCK.MAST_BLOCK_NAME
                                      }).Distinct().GroupBy(m => m.MAST_HAB_CODE).Where(g => g.Count() == 1 || g.Count() > 1)
                                        .Select(g => g.FirstOrDefault());


                totalRecords = lstHabitations.Count();




                if (sidx.Trim() != string.Empty)
                {
                    if (sord.ToString() == "asc")
                    {
                        lstHabitations = lstHabitations.OrderBy(x => x.MAST_HAB_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                    }
                    else
                    {
                        lstHabitations = lstHabitations.OrderByDescending(x => x.MAST_HAB_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                    }
                }
                else
                {
                    lstHabitations = lstHabitations.OrderBy(x => x.MAST_HAB_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                }



                var result = lstHabitations.Select(habDetails => new
                {

                    habDetails.PLAN_CN_ROAD_NUMBER,
                    habDetails.MAST_HAB_NAME,
                    habDetails.MAST_BUS_SERVICE,
                    habDetails.MAST_DEGREE_COLLEGE,
                    habDetails.MAST_HAB_CODE,
                    habDetails.MAST_DISPENSARY,
                    habDetails.MAST_ELECTRICTY,
                    habDetails.MAST_HAB_CONNECTED,
                    habDetails.MAST_HAB_SCST_POP,
                    habDetails.MAST_HAB_TOT_POP,
                    habDetails.MAST_HEALTH_SERVICE,
                    habDetails.MAST_HIGH_SCHOOL,
                    habDetails.MAST_INTERMEDIATE_SCHOOL,
                    habDetails.MAST_MCW_CENTERS,
                    habDetails.MAST_MIDDLE_SCHOOL,
                    habDetails.MAST_PANCHAYAT_HQ,
                    habDetails.MAST_PHCS,
                    habDetails.MAST_PRIMARY_SCHOOL,
                    habDetails.MAST_RAILWAY_STATION,
                    habDetails.MAST_SCHEME,
                    habDetails.MAST_TELEGRAPH_OFFICE,
                    habDetails.MAST_TELEPHONE_CONNECTION,
                    habDetails.MAST_TOURIST_PLACE,
                    habDetails.MAST_VETNARY_HOSPITAL,
                    habDetails.MAST_YEAR,
                    habDetails.MAST_BLOCK_NAME,
                    habDetails.MAST_VILLAGE_NAME,
                    habDetails.PLAN_LOCK_STATUS,
                    habDetails.MAST_BLOCK_CODE,
                    habDetails.PLAN_CN_ROAD_CODE
                }).ToArray();

                short roleCode = dbContext.UM_User_Master.Where(m => m.UserID == PMGSYSession.Current.UserId).Select(m => m.DefaultRoleID).FirstOrDefault();
                return result.Select(habDetails => new
                {

                    cell = new[]
                {
                    habDetails.MAST_HAB_CODE.ToString(),
                    habDetails.MAST_HAB_NAME == null?string.Empty:habDetails.MAST_HAB_NAME.ToString(),
                    habDetails.MAST_BLOCK_NAME == null?string.Empty:habDetails.MAST_BLOCK_NAME.ToString(),
                    habDetails.MAST_VILLAGE_NAME == null?string.Empty:habDetails.MAST_VILLAGE_NAME.ToString(),
                    habDetails.PLAN_CN_ROAD_NUMBER == null?string.Empty:habDetails.PLAN_CN_ROAD_NUMBER.ToString(),
                    habDetails.MAST_HAB_TOT_POP==null?"0":habDetails.MAST_HAB_TOT_POP.ToString(),
                    habDetails.MAST_HAB_SCST_POP==null?"0":habDetails.MAST_HAB_SCST_POP.ToString(),//New SC/ST Population
                    //dbContext.UDF_IMS_UNLOCK_STATUS(PMGSYSession.Current.StateCode,PMGSYSession.Current.DistrictCode,habDetails.MAST_BLOCK_CODE,0,0,habDetails.PLAN_CN_ROAD_CODE,0,0,"CN",PMGSYSession.Current.PMGSYScheme).Select(m=>m.UNLOCK_COUNT).FirstOrDefault().Value > 0 ? "<center><span  class='ui-icon ui-icon-trash' title='Enter Habitation Details' onClick ='deleteHabitationDetails(\"" + URLEncrypt.EncryptParameters1(new string[]{"HabCode =" + habDetails.MAST_HAB_CODE.ToString().Trim(),"Flag ="+flag.ToString().Trim()}) + "\");'></span></center>" : habDetails.PLAN_LOCK_STATUS == "Y" ? "<span class='ui-icon ui-icon-locked ui-align-center'></span>" : "<center><span  class='ui-icon ui-icon-trash' title='Enter Habitation Details' onClick ='deleteHabitationDetails(\"" + URLEncrypt.EncryptParameters1(new string[]{"HabCode =" + habDetails.MAST_HAB_CODE.ToString().Trim(),"Flag ="+flag.ToString().Trim()}) + "\");'></span></center>",
                    dbContext.UDF_IMS_UNLOCK_STATUS(PMGSYSession.Current.StateCode,PMGSYSession.Current.DistrictCode,habDetails.MAST_BLOCK_CODE,0,0,habDetails.PLAN_CN_ROAD_CODE,0,0,"CN",PMGSYSession.Current.PMGSYScheme,roleCode).Select(m=>m.UNLOCK_COUNT).FirstOrDefault().Value > 0 ? "<center><span  class='ui-icon ui-icon-trash' title='Enter Habitation Details' onClick ='deleteHabitationDetails(\"" + URLEncrypt.EncryptParameters1(new string[]{"HabCode =" + habDetails.MAST_HAB_CODE.ToString().Trim(),"Flag ="+flag.ToString().Trim()}) + "\");'></span></center>" : habDetails.PLAN_LOCK_STATUS == "Y" ? "<span class='ui-icon ui-icon-locked ui-align-center'></span>" : "<center><span  class='ui-icon ui-icon-trash' title='Enter Habitation Details' onClick ='deleteHabitationDetails(\"" + URLEncrypt.EncryptParameters1(new string[]{"HabCode =" + habDetails.MAST_HAB_CODE.ToString().Trim(),"Flag ="+flag.ToString().Trim()}) + "\");'></span></center>",
                    
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

        /// <summary>
        /// populateds the route type dropdown
        /// </summary>
        /// <returns></returns>
        public SelectList GetAllRoutes()
        {
            try
            {
                List<SelectListItem> lstRoutes = new List<SelectListItem>();
                lstRoutes.Add(new SelectListItem { Value = "0", Text = "--All--", Selected = true });
                if (PMGSYSession.Current.PMGSYScheme == 1)
                {
                    lstRoutes.Add(new SelectListItem { Value = "T", Text = "Through Route" });
                    lstRoutes.Add(new SelectListItem { Value = "L", Text = "Link Route" });
                }
                else
                {
                    lstRoutes.Add(new SelectListItem { Value = "T", Text = "Through Route" });
                    lstRoutes.Add(new SelectListItem { Value = "L", Text = "Link Route" });
                    lstRoutes.Add(new SelectListItem { Value = "M", Text = "Major Rural Link" });
                }
                return new SelectList(lstRoutes, "Value", "Text");
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
            }
            return null;

        }

        /// <summary>
        /// populates the road category dropdown in search view
        /// </summary>
        /// <returns></returns>
        public SelectList GetCategoryForSearch()
        {
            PMGSYEntities dbContext = new PMGSYEntities();
            try
            {
                List<MASTER_ROAD_CATEGORY> lstRoad = dbContext.MASTER_ROAD_CATEGORY.OrderBy(m => m.MAST_ROAD_CAT_NAME).ToList<MASTER_ROAD_CATEGORY>();
                return new SelectList(lstRoad, "MAST_ROAD_CAT_CODE", "MAST_ROAD_CAT_NAME");
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
        /// map the habitation to the road
        /// </summary>
        /// <param name="habitationCode">habitation id</param>
        /// <param name="roadCode">core network id</param>
        /// <param name="message">response message</param>
        /// <returns></returns>
        public bool AddHabitation(int habitationCode, int roadCode, ref string message)
        {
            PMGSYEntities dbContext = new PMGSYEntities();
            try
            {
                PLAN_ROAD_HABITATION master = new PLAN_ROAD_HABITATION();
                master.PLAN_CN_ROAD_HAB_ID = dbContext.PLAN_ROAD_HABITATION.Any() ? (from item in dbContext.PLAN_ROAD_HABITATION select item.PLAN_CN_ROAD_HAB_ID).Max() + 1 : 1;
                master.PLAN_CN_ROAD_CODE = roadCode;
                master.MAST_HAB_CODE = habitationCode;
                master.USERID = PMGSYSession.Current.UserId;
                master.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                dbContext.PLAN_ROAD_HABITATION.Add(master);
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
                if (dbContext != null)
                {
                    dbContext.Dispose();
                }
            }

        }

        /// <summary>
        /// delete the habitation map details from the road
        /// </summary>
        /// <param name="habitationCode">corresponding habitation id</param>
        /// <param name="roadCode">core network code</param>
        /// <returns></returns>
        public bool DeleteMapHabitation(int habitationCode, string flag, int roadCode)
        {
            PMGSYEntities dbContext = new PMGSYEntities();
            try
            {
                ///Changed by SAMMED A. PATIL on 24 APRIL 2017 to skip validation for PMGSY-II
                if (dbContext.IMS_BENEFITED_HABS.Any(m => m.MAST_HAB_CODE == habitationCode && m.IMS_SANCTIONED_PROJECTS.PLAN_CN_ROAD_CODE == roadCode && m.IMS_SANCTIONED_PROJECTS.MAST_PMGSY_SCHEME == PMGSYSession.Current.PMGSYScheme))
                {
                    return false;
                }

                PLAN_ROAD_HABITATION master = dbContext.PLAN_ROAD_HABITATION.Where(m => m.MAST_HAB_CODE == habitationCode && m.PLAN_CN_ROAD_CODE == roadCode).FirstOrDefault();
                master.USERID = PMGSYSession.Current.UserId;
                master.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                dbContext.Entry(master).State = System.Data.Entity.EntityState.Modified;
                dbContext.SaveChanges();

                dbContext.PLAN_ROAD_HABITATION.Remove(master);
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
                if (dbContext != null)
                {
                    dbContext.Dispose();
                }
            }

        }

        /// <summary>
        /// populates the Road Number Dropdown
        /// </summary>
        /// <param name="roadFrom">road type</param>
        /// <param name="blockCode">block code</param>
        /// <returns>list of Road Number</returns>
        public SelectList GetRoadNumber(string roadFrom, int blockCode)
        {
            List<PLAN_ROAD> lstPlanRoad = new List<PLAN_ROAD>();
            try
            {
                switch (roadFrom)
                {
                    case "T":
                        lstPlanRoad = GetRoadNumFromThroughList(roadFrom, blockCode);
                        return new SelectList(lstPlanRoad, "PLAN_CN_ROAD_CODE", "PLAN_CN_ROAD_NUMBER");
                    case "L":
                        lstPlanRoad = GetRoadNumFromThroughList(roadFrom, blockCode);
                        return new SelectList(lstPlanRoad, "PLAN_CN_ROAD_CODE", "PLAN_CN_ROAD_NUMBER");
                    default:
                        List<MASTER_EXISTING_ROADS> list = GetRoadNumFromByRoadFrom(roadFrom, blockCode);
                        if (list.Count() == 0)
                        {
                            list.Insert(0, new MASTER_EXISTING_ROADS { MAST_ER_ROAD_CODE = 0, MAST_ER_ROAD_NAME = "-No Roads Found-" });
                            return new SelectList(list, "MAST_ER_ROAD_CODE", "MAST_ER_ROAD_NAME");
                        }
                        else
                        {
                            return new SelectList(list, "MAST_ER_ROAD_CODE", "MAST_ER_ROAD_NAME");
                        }
                }
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                return null;
            }
        }

        /// <summary>
        /// populates the availabe list of habitations to be mapped 
        /// </summary>
        /// <param name="roadCode">core network code</param>
        /// <param name="page">no. of pages</param>
        /// <param name="rows">no of rows</param>
        /// <param name="sidx">sort column</param>
        /// <param name="sord">sort order</param>
        /// <param name="totalRecords">no. of totalRecords</param>
        /// <returns>list of availabel Habitation list</returns>
        public Array GetHabitationListToMap(int roadCode, int blockCode, int erRoadCode, int page, int rows, string sidx, string sord, out long totalRecords)
        {
            PMGSYEntities dbContext = new PMGSYEntities();
            totalRecords = 0;
            try
            {
                PLAN_ROAD masterRoad = dbContext.PLAN_ROAD.Find(roadCode);

                var lstHabitations = (from item in dbContext.MASTER_VILLAGE
                                      join habitation in dbContext.MASTER_HABITATIONS on item.MAST_VILLAGE_CODE equals habitation.MAST_VILLAGE_CODE
                                      join habDetails in dbContext.MASTER_HABITATIONS_DETAILS on habitation.MAST_HAB_CODE equals habDetails.MAST_HAB_CODE
                                      where item.MAST_BLOCK_CODE == blockCode &&//masterRoad.MAST_BLOCK_CODE && commented by Vikram in order to map habitations of other blocks also
                                          //(habitation.MAST_HAB_STATUS == "U" || habitation.MAST_HAB_STATUS == "C")
                                      habitation.MAST_HABITATION_ACTIVE == "Y" //new condition added by Vikram and above line commented as per suggestion from Dev Sir
                                      select new
                                      {
                                          habitation.MAST_HAB_NAME,
                                          item.MAST_VILLAGE_NAME,
                                          habDetails.MAST_HAB_TOT_POP,
                                          habitation.MAST_HAB_CODE,

                                      });

                List<int> mapHabitations = (from item in dbContext.PLAN_ROAD_HABITATION
                                            join data in dbContext.PLAN_ROAD on item.PLAN_CN_ROAD_CODE equals data.PLAN_CN_ROAD_CODE
                                            where
                                            data.MAST_BLOCK_CODE == masterRoad.MAST_BLOCK_CODE &&
                                            data.MAST_PMGSY_SCHEME == (PMGSYSession.Current.PMGSYScheme)
                                            select item.MAST_HAB_CODE).Distinct().ToList<int>();

                if (PMGSYSession.Current.PMGSYScheme == 2)
                {
                    lstHabitations = (from item in dbContext.MASTER_VILLAGE
                                      join habitation in dbContext.MASTER_HABITATIONS on item.MAST_VILLAGE_CODE equals habitation.MAST_VILLAGE_CODE
                                      join habDetails in dbContext.MASTER_HABITATIONS_DETAILS on habitation.MAST_HAB_CODE equals habDetails.MAST_HAB_CODE
                                      join erHabs in dbContext.MASTER_ER_HABITATION_ROAD on habitation.MAST_HAB_CODE equals erHabs.MAST_HAB_CODE
                                      where erHabs.MAST_ER_ROAD_CODE == erRoadCode &&//masterRoad.MAST_BLOCK_CODE && commented by Vikram in order to map habitations of other blocks also
                                          //(habitation.MAST_HAB_STATUS == "U" || habitation.MAST_HAB_STATUS == "C")
                                      habitation.MAST_HABITATION_ACTIVE == "Y" //new condition added by Vikram and above line commented as per suggestion from Dev Sir
                                      && habDetails.MAST_YEAR == 2011
                                      select new
                                      {
                                          habitation.MAST_HAB_NAME,
                                          item.MAST_VILLAGE_NAME,
                                          habDetails.MAST_HAB_TOT_POP,
                                          habitation.MAST_HAB_CODE,

                                      });

                    mapHabitations = (from item in dbContext.PLAN_ROAD_HABITATION
                                      //join data in dbContext.PLAN_ROAD on item.PLAN_CN_ROAD_CODE equals data.PLAN_CN_ROAD_CODE
                                      where
                                      item.PLAN_CN_ROAD_CODE == roadCode &&
                                      item.PLAN_ROAD.MAST_PMGSY_SCHEME == (PMGSYSession.Current.PMGSYScheme)
                                      select item.MAST_HAB_CODE).Distinct().ToList<int>();
                }

                var listHab = (from item in lstHabitations
                               where !mapHabitations.Contains(item.MAST_HAB_CODE)
                               select item.MAST_HAB_CODE).Distinct();


                //dynamic mappingList = null;

                var route = dbContext.PLAN_ROAD.Where(x => x.PLAN_CN_ROAD_CODE == roadCode).Select(x => x.PLAN_RD_ROUTE).FirstOrDefault();
                if (route == "N")
                {
                    var mappingList = (from item in dbContext.MASTER_HABITATIONS
                                       join habitation in dbContext.MASTER_ER_HABITATION_ROAD on item.MAST_HAB_CODE equals habitation.MAST_HAB_CODE
                                       join planHab in dbContext.MASTER_ER_HABITATION_ROAD on item.MAST_HAB_CODE equals planHab.MAST_HAB_CODE
                                       join habDetails1 in dbContext.MASTER_HABITATIONS_DETAILS on item.MAST_HAB_CODE equals habDetails1.MAST_HAB_CODE
                                       join planRoad in dbContext.PLAN_ROAD on planHab.MAST_ER_ROAD_CODE equals planRoad.MAST_ER_ROAD_CODE
                                       where planRoad.PLAN_CN_ROAD_CODE == roadCode
                                       && item.MASTER_VILLAGE.MAST_BLOCK_CODE == blockCode
                                       && habDetails1.MAST_YEAR == (PMGSYSession.Current.PMGSYScheme == 1 ? 2001 : 2011)
                                       && item.MAST_HABITATION_ACTIVE == "Y"
                                       select new
                                       {
                                           item.MAST_HAB_CODE,
                                           item.MAST_HAB_NAME,
                                           item.MASTER_VILLAGE.MAST_VILLAGE_NAME,
                                           habDetails1.MAST_HAB_TOT_POP
                                       }).Distinct().ToList();

                    totalRecords = mappingList.Count();

                    if (sidx.Trim() != string.Empty)
                    {
                        if (sord.ToString() == "asc")
                        {
                            mappingList = mappingList.OrderBy(x => x.MAST_HAB_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                        }
                        else
                        {
                            mappingList = mappingList.OrderByDescending(x => x.MAST_HAB_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                        }
                    }
                    else
                    {
                        mappingList = mappingList.OrderBy(x => x.MAST_HAB_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                    }

                    var result = mappingList.Select(habDetails => new
                    {
                        habDetails.MAST_HAB_CODE,
                        habDetails.MAST_HAB_NAME,
                        habDetails.MAST_VILLAGE_NAME,
                        habDetails.MAST_HAB_TOT_POP
                    }).ToArray();



                    return result.Select(habDetails => new
                    {
                        id = URLEncrypt.EncryptParameters1(new string[] { "HabCode =" + habDetails.MAST_HAB_CODE.ToString().Trim() }),
                        cell = new[] {
                        
                        //habDetails.MAST_HAB_CODE.ToString(),    
                        habDetails.MAST_HAB_NAME.ToString(),
                        habDetails.MAST_VILLAGE_NAME.ToString(),
                        habDetails.MAST_HAB_TOT_POP.ToString()
                    }
                    }).ToArray();

                }
                else
                {
                    var mappingList = (from item in dbContext.MASTER_HABITATIONS_DETAILS
                                       join habitation in dbContext.MASTER_HABITATIONS on item.MAST_HAB_CODE equals habitation.MAST_HAB_CODE
                                       join village in dbContext.MASTER_VILLAGE on habitation.MAST_VILLAGE_CODE equals village.MAST_VILLAGE_CODE
                                       where listHab.Contains(item.MAST_HAB_CODE) &&
                                       item.MAST_YEAR == (PMGSYSession.Current.PMGSYScheme == 1 ? 2001 : 2011)
                                       select new
                                       {
                                           item.MAST_HAB_CODE,
                                           habitation.MAST_HAB_NAME,
                                           village.MAST_VILLAGE_NAME,
                                           item.MAST_HAB_TOT_POP
                                       }).Distinct().ToList();

                    totalRecords = mappingList.Count();

                    if (sidx.Trim() != string.Empty)
                    {
                        if (sord.ToString() == "asc")
                        {
                            mappingList = mappingList.OrderBy(x => x.MAST_HAB_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                        }
                        else
                        {
                            mappingList = mappingList.OrderByDescending(x => x.MAST_HAB_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                        }
                    }
                    else
                    {
                        mappingList = mappingList.OrderBy(x => x.MAST_HAB_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                    }

                    var result = mappingList.Select(habDetails => new
                    {
                        habDetails.MAST_HAB_CODE,
                        habDetails.MAST_HAB_NAME,
                        habDetails.MAST_VILLAGE_NAME,
                        habDetails.MAST_HAB_TOT_POP
                    }).ToArray();



                    return result.Select(habDetails => new
                    {
                        id = URLEncrypt.EncryptParameters1(new string[] { "HabCode =" + habDetails.MAST_HAB_CODE.ToString().Trim() }),
                        cell = new[] {
                        
                        //habDetails.MAST_HAB_CODE.ToString(),    
                        habDetails.MAST_HAB_NAME.ToString(),
                        habDetails.MAST_VILLAGE_NAME.ToString(),
                        habDetails.MAST_HAB_TOT_POP.ToString()
                    }
                    }).ToArray();
                }


                //totalRecords = mappingList.Count();

                //if (sidx.Trim() != string.Empty)
                //{
                //    if (sord.ToString() == "asc")
                //    {
                //        mappingList = mappingList.OrderBy(x => x.MAST_HAB_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                //    }
                //    else
                //    {
                //        mappingList = mappingList.OrderByDescending(x => x.MAST_HAB_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                //    }
                //}
                //else
                //{
                //    mappingList = mappingList.OrderBy(x => x.MAST_HAB_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                //}

                //var result = mappingList.Select(habDetails => new
                //{
                //    habDetails.MAST_HAB_CODE,
                //    habDetails.MAST_HAB_NAME,
                //    habDetails.MAST_VILLAGE_NAME,
                //    habDetails.MAST_HAB_TOT_POP
                //}).ToArray();



                //return result.Select(habDetails => new
                //{
                //    id = URLEncrypt.EncryptParameters1(new string[] { "HabCode =" + habDetails.MAST_HAB_CODE.ToString().Trim() }),
                //    cell = new[] {

                //        //habDetails.MAST_HAB_CODE.ToString(),    
                //        habDetails.MAST_HAB_NAME.ToString(),
                //        habDetails.MAST_VILLAGE_NAME.ToString(),
                //        habDetails.MAST_HAB_TOT_POP.ToString()
                //    }
                //}).ToArray();
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
        /// maps the habitations to the current road
        /// </summary>
        /// <param name="encryptedHabCodes">encrypted habitation codes</param>
        /// <param name="roadName">core network code</param>
        /// <returns></returns>
        public bool MapHabitationToRoad(string encryptedHabCodes, string roadName)
        {
            PMGSYEntities dbContext = new PMGSYEntities();
            try
            {
                String[] habCodes = null;
                int roadCode = Convert.ToInt32(roadName);
                int habCode = 0;

                habCodes = encryptedHabCodes.Split(',');
                if (habCodes.Count() == 0)
                {
                    return false;
                }
                foreach (String item in habCodes)
                {
                    encryptedParameters = null;
                    encryptedParameters = item.Split('/');
                    decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { encryptedParameters[0], encryptedParameters[1], encryptedParameters[2] });
                    habCode = Convert.ToInt32(decryptedParameters["HabCode"].ToString());

                    PLAN_ROAD_HABITATION master = new PLAN_ROAD_HABITATION();
                    master.PLAN_CN_ROAD_HAB_ID = dbContext.PLAN_ROAD_HABITATION.Any() ? (from item1 in dbContext.PLAN_ROAD_HABITATION select item1.PLAN_CN_ROAD_HAB_ID).Max() + 1 : 1;
                    master.PLAN_CN_ROAD_CODE = roadCode;
                    master.MAST_HAB_CODE = habCode;
                    master.USERID = PMGSYSession.Current.UserId;
                    master.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                    dbContext.PLAN_ROAD_HABITATION.Add(master);
                    dbContext.SaveChanges();
                }
                return true;
            }
            catch (OptimisticConcurrencyException ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);

                return false;
            }
            catch (UpdateException ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                return false;
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

        /// <summary>
        /// finalizes the core network details
        /// </summary>
        /// <param name="roadCode">core network code</param>
        /// <returns></returns>
        public string FinalizeCoreNetworkDAL(int roadCode)
        {
            PMGSYEntities dbContext = new PMGSYEntities();
            try
            {
                PLAN_ROAD planRoad = dbContext.PLAN_ROAD.Find(roadCode);
                planRoad.PLAN_LOCK_STATUS = "Y";
                planRoad.USERID = PMGSYSession.Current.UserId;
                planRoad.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                dbContext.Entry(planRoad).State = System.Data.Entity.EntityState.Modified;
                dbContext.SaveChanges();
                return string.Empty;
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                return "An Error Occurred While Your Processing Request.";
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
        /// returns blockname according to the block code
        /// </summary>
        /// <param name="blockCode"></param>
        /// <returns></returns>
        public string GetBlockName(int blockCode)
        {
            PMGSYEntities dbContext = new PMGSYEntities();
            try
            {
                return dbContext.MASTER_BLOCK.Where(m => m.MAST_BLOCK_CODE == blockCode).Select(m => m.MAST_BLOCK_NAME).FirstOrDefault();
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
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

        public List<string> MLRoadList(int blockCode)
        {
            //List<SelectListItem> roadList = new List<SelectListItem>();
            //SelectListItem item;
            PMGSYEntities dbContext = new PMGSYEntities();
            try
            {
                //var query = (from c in dbContext.PLAN_ROAD
                //             where c.MAST_BLOCK_CODE == blockCode && c.PLAN_RD_ROUTE == "N"
                //             select new
                //             {
                //                 Text = c.PLAN_CN_ROAD_NUMBER,
                //             }).OrderBy(c => c.Text).ToList();

                //foreach (var data in query)
                //{
                //    item = new SelectListItem();
                //    item.Text = data.Text;
                //    item.Value = data.Value.ToString();
                //    roadList.Add(item);
                //}
                //return roadList;
                List<string> roadList = (from item in dbContext.PLAN_ROAD
                                         where item.MAST_BLOCK_CODE == blockCode && item.PLAN_RD_ROUTE == "N" && item.MAST_PMGSY_SCHEME == PMGSYSession.Current.PMGSYScheme
                                         select item.PLAN_CN_ROAD_NUMBER).Distinct().ToList<string>();
                return roadList;
            }
            catch
            {
                return null;
            }
            finally
            {
                dbContext.Dispose();
            }
        }

        public bool checkSchedule5DAL(int blockCode)
        {
            PMGSYEntities dbContext = new PMGSYEntities();
            try
            {
                return dbContext.MASTER_BLOCK.Where(m => m.MAST_BLOCK_CODE == blockCode).Select(m => m.MAST_IAP_BLOCK).FirstOrDefault() == "Y" ? true : false;
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

        /// <summary>
        /// returns the list of allocated through routes
        /// </summary>
        /// <param name="blockCode"></param>
        /// <returns></returns>
        public List<string> GetThroughRoutes(int blockCode, int cnCode)
        {
            PMGSYEntities dbContext = new PMGSYEntities();
            try
            {
                List<string> filter = (from item in dbContext.PLAN_ROAD
                                       where item.MAST_BLOCK_CODE == blockCode && item.PLAN_RD_ROUTE == "T" && item.MAST_PMGSY_SCHEME == PMGSYSession.Current.PMGSYScheme//item.PLAN_CN_ROAD_NUMBER.StartsWith("T")
                                       && (cnCode == 0 ? (0 == 0) : (item.PLAN_CN_ROAD_CODE != cnCode))
                                       select item.PLAN_CN_ROAD_NUMBER).Distinct().ToList<string>();
                return filter;
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
        /// returns the list of allocated through routes
        /// </summary>
        /// <param name="blockCode"></param>
        /// <returns></returns>
        public List<string> GetMRLRoutes(int blockCode, int cnCode)
        {
            PMGSYEntities dbContext = new PMGSYEntities();
            try
            {
                List<string> filter = (from item in dbContext.PLAN_ROAD
                                       where item.MAST_BLOCK_CODE == blockCode && item.PLAN_RD_ROUTE == "M" && item.MAST_PMGSY_SCHEME == PMGSYSession.Current.PMGSYScheme//item.PLAN_CN_ROAD_NUMBER.StartsWith("T")
                                       && (cnCode == 0 ? (0 == 0) : (item.PLAN_CN_ROAD_CODE != cnCode))
                                       select item.PLAN_CN_ROAD_NUMBER).Distinct().ToList<string>();
                return filter;
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
        /// returns allocated link routes
        /// </summary>
        /// <param name="blockCode"></param>
        /// <returns></returns>
        public List<string> GetLinkRoutes(int blockCode, int cnCode)
        {
            PMGSYEntities dbContext = new PMGSYEntities();
            try
            {
                List<string> filter = (from item in dbContext.PLAN_ROAD
                                       where item.MAST_BLOCK_CODE == blockCode && item.MAST_PMGSY_SCHEME == PMGSYSession.Current.PMGSYScheme && item.PLAN_RD_ROUTE == "L"//item.PLAN_CN_ROAD_NUMBER.StartsWith("L")
                                       && (cnCode == 0 ? (0 == 0) : (item.PLAN_CN_ROAD_CODE != cnCode))
                                       select item.PLAN_CN_ROAD_NUMBER).Distinct().ToList<string>();
                return filter;
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
        /// get the existing road details
        /// </summary>
        /// <param name="roadCode">existing road id</param>
        /// <returns>entity object containing existing road details</returns>
        public MASTER_EXISTING_ROADS GetRoadDetails(int roadCode)
        {
            PMGSYEntities dbContext = new PMGSYEntities();
            try
            {
                MASTER_EXISTING_ROADS master = dbContext.MASTER_EXISTING_ROADS.Find(roadCode);
                return master;
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                return null;
            }
        }

        /// <summary>
        /// get the Habitation details by habitation code
        /// </summary>
        /// <param name="habCode">habitation code</param>
        public MASTER_HABITATIONS_DETAILS GetHabitationDetails(int habCode)
        {
            PMGSYEntities dbContext = new PMGSYEntities();
            try
            {
                MASTER_HABITATIONS_DETAILS master = dbContext.MASTER_HABITATIONS_DETAILS.Where(m => m.MAST_HAB_CODE == habCode).FirstOrDefault();
                if (master != null)
                {
                    return master;
                }
                else
                {
                    return null;
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
        /// returns road name by plan road code
        /// </summary>
        /// <param name="roadCode">core network code</param>
        /// <returns></returns>
        public string GetRoadName(int roadCode)
        {
            PMGSYEntities dbContext = new PMGSYEntities();
            try
            {
                string roadName = (from item in dbContext.PLAN_ROAD
                                   where item.PLAN_CN_ROAD_CODE == roadCode
                                   select item.PLAN_RD_NAME).FirstOrDefault().ToString();
                return roadName;
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                return string.Empty;
            }
            finally
            {
                dbContext.Dispose();
            }
        }

        /// <summary>
        /// returns the Proposal list associated with this core network
        /// </summary>
        /// <param name="roadCode"></param>
        /// <param name="page"></param>
        /// <param name="rows"></param>
        /// <param name="sidx"></param>
        /// <param name="sord"></param>
        /// <param name="totalRecords"></param>
        /// <returns></returns>
        public Array ListProposals(int roadCode, int page, int rows, string sidx, string sord, out long totalRecords)
        {
            PMGSYEntities dbContext = new PMGSYEntities();
            try
            {
                var lstProposals = (from item in dbContext.IMS_SANCTIONED_PROJECTS
                                    where
                                    item.PLAN_CN_ROAD_CODE == roadCode
                                    select new
                                    {
                                        item.IMS_PR_ROAD_CODE,
                                        item.IMS_PROPOSAL_TYPE,
                                        WORK_NAME = item.IMS_PROPOSAL_TYPE == "P" ? item.IMS_ROAD_NAME : item.IMS_BRIDGE_NAME,
                                        item.IMS_YEAR,
                                        item.IMS_BATCH,
                                        item.IMS_PACKAGE_ID,
                                        item.IMS_UPGRADE_CONNECT,
                                        WORK_COST = item.IMS_PROPOSAL_TYPE == "P" ? item.IMS_PAV_EST_COST : item.IMS_BRIDGE_WORKS_EST_COST,
                                        item.IMS_ISCOMPLETED,
                                        item.IMS_PAV_LENGTH,
                                    }).ToList();

                totalRecords = lstProposals.Count();

                if (sidx.Trim() != string.Empty)
                {
                    if (sord.ToString() == "asc")
                    {
                        switch (sidx)
                        {
                            case "IMS_PROPOSAL_TYPE":
                                lstProposals = lstProposals.OrderBy(x => x.IMS_PROPOSAL_TYPE).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                            case "WORK_NAME":
                                lstProposals = lstProposals.OrderBy(x => x.WORK_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                            case "IMS_YEAR":
                                lstProposals = lstProposals.OrderBy(x => x.IMS_YEAR).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                            case "IMS_BATCH":
                                lstProposals = lstProposals.OrderBy(x => x.IMS_BATCH).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                            case "IMS_PACKAGE_ID":
                                lstProposals = lstProposals.OrderBy(x => x.IMS_PACKAGE_ID).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                            case "IMS_UPGRADE_CONNECT":
                                lstProposals = lstProposals.OrderBy(x => x.IMS_UPGRADE_CONNECT).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                            case "WORK_COST":
                                lstProposals = lstProposals.OrderBy(x => x.WORK_COST).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                            case "IMS_ISCOMPLETED":
                                lstProposals = lstProposals.OrderBy(x => x.IMS_ISCOMPLETED).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                            default:
                                lstProposals = lstProposals.OrderBy(x => x.IMS_PR_ROAD_CODE).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                        }
                    }
                    else
                    {
                        switch (sidx)
                        {
                            case "IMS_PROPOSAL_TYPE":
                                lstProposals = lstProposals.OrderByDescending(x => x.IMS_PROPOSAL_TYPE).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                            case "WORK_NAME":
                                lstProposals = lstProposals.OrderByDescending(x => x.WORK_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                            case "IMS_YEAR":
                                lstProposals = lstProposals.OrderByDescending(x => x.IMS_YEAR).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                            case "IMS_BATCH":
                                lstProposals = lstProposals.OrderByDescending(x => x.IMS_BATCH).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                            case "IMS_PACKAGE_ID":
                                lstProposals = lstProposals.OrderByDescending(x => x.IMS_PACKAGE_ID).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                            case "IMS_UPGRADE_CONNECT":
                                lstProposals = lstProposals.OrderByDescending(x => x.IMS_UPGRADE_CONNECT).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                            case "WORK_COST":
                                lstProposals = lstProposals.OrderByDescending(x => x.WORK_COST).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                            case "IMS_ISCOMPLETED":
                                lstProposals = lstProposals.OrderByDescending(x => x.IMS_ISCOMPLETED).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                            default:
                                lstProposals = lstProposals.OrderByDescending(x => x.IMS_PR_ROAD_CODE).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                        }
                    }
                }
                else
                {
                    lstProposals = lstProposals.OrderByDescending(x => x.IMS_PR_ROAD_CODE).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                }


                return lstProposals.Select(roadDetails => new
                {
                    cell = new[]
                        {
                            roadDetails.IMS_PROPOSAL_TYPE == "P"?"Road":(roadDetails.IMS_PROPOSAL_TYPE=="L"?"Bridge":"-"),
                            roadDetails.WORK_NAME.ToString(),
                            roadDetails.IMS_YEAR.ToString(),
                            roadDetails.IMS_BATCH.ToString(),
                            roadDetails.IMS_PACKAGE_ID.ToString(),
                            roadDetails.IMS_UPGRADE_CONNECT == "N"?"New":(roadDetails.IMS_UPGRADE_CONNECT == "U"?"Upgrade":"-"),
                            roadDetails.WORK_COST.ToString(),
                            roadDetails.IMS_ISCOMPLETED == "H"?"Habitation Finalized":(roadDetails.IMS_ISCOMPLETED == "D"?"PIU Finalized":(roadDetails.IMS_ISCOMPLETED == "S"?"STA Scrutinized":(roadDetails.IMS_ISCOMPLETED == "M"?"MORD Finalized":(roadDetails.IMS_ISCOMPLETED == "A"?"Agreement Made":(roadDetails.IMS_ISCOMPLETED == "P"?"Execution Progress":(roadDetails.IMS_ISCOMPLETED == "X"?"In Maintenance":(roadDetails.IMS_ISCOMPLETED == "C"?"Completed":(roadDetails.IMS_ISCOMPLETED == "E"?"Proposal Entered":"NA")))))))),
                            roadDetails.IMS_PAV_LENGTH.ToString(),
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
        /// returns the list of mapped candidate road with the particular Candidate Road
        /// </summary>
        /// <param name="roadCode">id of DRRP</param>
        /// <param name="page"></param>
        /// <param name="rows"></param>
        /// <param name="sidx"></param>
        /// <param name="sord"></param>
        /// <param name="totalRecords"></param>
        /// <returns></returns>
        public Array ListCandidateRoads(int roadCode, int page, int rows, string sidx, string sord, out long totalRecords, out string IsFinalized)
        {
            PMGSYEntities dbContext = new PMGSYEntities();
            try
            {
                var lstMappedRoads = (from item in dbContext.PLAN_ROAD_DRRP
                                      where item.PLAN_CN_ROAD_CODE == roadCode
                                      select new
                                      {
                                          item.MASTER_EXISTING_ROADS.MASTER_BLOCK.MAST_BLOCK_NAME,
                                          item.MASTER_EXISTING_ROADS.MASTER_ROAD_CATEGORY.MAST_ROAD_CAT_NAME,
                                          item.MASTER_EXISTING_ROADS.MAST_ER_ROAD_NAME,
                                          item.PLAN_RD_LENGTH,
                                          item.PLAN_RD_LENG,
                                          item.MAST_ER_ROAD_CODE,
                                          item.PLAN_LOCK_STATUS,
                                          item.PLAN_CN_ROAD_CODE,
                                          item.MASTER_EXISTING_ROADS.MAST_ER_ROAD_NUMBER
                                      }).ToList();

                totalRecords = lstMappedRoads.Count();

                IsFinalized = String.Empty;
                if (dbContext.PLAN_ROAD_DRRP.Any(m => m.PLAN_CN_ROAD_CODE == roadCode && m.PLAN_LOCK_STATUS == "N"))
                {
                    IsFinalized = "N";
                }
                else if (dbContext.PLAN_ROAD_DRRP.Any(m => m.PLAN_CN_ROAD_CODE == roadCode && m.PLAN_LOCK_STATUS == "Y"))
                {
                    IsFinalized = "Y";
                }

                if (sidx.Trim() != string.Empty)
                {
                    if (sord.ToString() == "asc")
                    {
                        switch (sidx)
                        {
                            case "MAST_BLOCK_NAME":
                                lstMappedRoads = lstMappedRoads.OrderBy(x => x.MAST_BLOCK_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                            case "MAST_ROAD_CAT_NAME":
                                lstMappedRoads = lstMappedRoads.OrderBy(x => x.MAST_ROAD_CAT_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                            case "MAST_ER_ROAD_NAME":
                                lstMappedRoads = lstMappedRoads.OrderBy(x => x.MAST_ER_ROAD_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                            case "PLAN_RD_LENGTH":
                                lstMappedRoads = lstMappedRoads.OrderBy(x => x.PLAN_RD_LENGTH).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                            case "PLAN_RD_LENG":
                                lstMappedRoads = lstMappedRoads.OrderBy(x => x.PLAN_RD_LENG).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                            default:
                                lstMappedRoads = lstMappedRoads.OrderBy(x => x.MAST_ER_ROAD_CODE).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                        }
                    }
                    else
                    {
                        switch (sidx)
                        {
                            case "MAST_BLOCK_NAME":
                                lstMappedRoads = lstMappedRoads.OrderByDescending(x => x.MAST_BLOCK_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                            case "MAST_ROAD_CAT_NAME":
                                lstMappedRoads = lstMappedRoads.OrderByDescending(x => x.MAST_ROAD_CAT_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                            case "MAST_ER_ROAD_NAME":
                                lstMappedRoads = lstMappedRoads.OrderByDescending(x => x.MAST_ER_ROAD_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                            case "PLAN_RD_LENGTH":
                                lstMappedRoads = lstMappedRoads.OrderByDescending(x => x.PLAN_RD_LENGTH).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                            case "PLAN_RD_LENG":
                                lstMappedRoads = lstMappedRoads.OrderByDescending(x => x.PLAN_RD_LENG).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                            default:
                                lstMappedRoads = lstMappedRoads.OrderByDescending(x => x.MAST_ER_ROAD_CODE).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                        }
                    }
                }
                else
                {
                    lstMappedRoads = lstMappedRoads.OrderByDescending(x => x.MAST_ER_ROAD_CODE).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                }

                return lstMappedRoads.Select(roadDetails => new
                {
                    cell = new[]
                        {
                            roadDetails.MAST_BLOCK_NAME == null?"-":roadDetails.MAST_BLOCK_NAME.ToString(),
                            roadDetails.MAST_ROAD_CAT_NAME == null?"-":roadDetails.MAST_ROAD_CAT_NAME.ToString(),
                            roadDetails.MAST_ER_ROAD_NAME == null?"-":(roadDetails.MAST_ER_ROAD_NUMBER.ToString() +" - "+ roadDetails.MAST_ER_ROAD_NAME.ToString()),
                            roadDetails.PLAN_RD_LENGTH == null?"0":roadDetails.PLAN_RD_LENGTH.ToString(),
                            roadDetails.PLAN_RD_LENG == null?"-":(roadDetails.PLAN_RD_LENG == "P"?"Partial":"Full"),
                            roadDetails.PLAN_LOCK_STATUS == "Y"?"<span class='ui-icon ui-icon-locked ui-align-center'></span>":"<a href='#' title='Click here to edit candidate road details' class='ui-icon ui-icon-pencil ui-align-center' onClick =EditCandidateRoad('"+URLEncrypt.EncryptParameters1(new string[]{"DRRPCode =" + roadDetails.MAST_ER_ROAD_CODE.ToString().Trim(),"CNCode ="+roadDetails.PLAN_CN_ROAD_CODE})+"'); return false;'>Edit</a>",
                            roadDetails.PLAN_LOCK_STATUS == "Y"?"<span class='ui-icon ui-icon-locked ui-align-center'></span>":"<a href='#' title='Click here to delete candidate road details' class='ui-icon ui-icon-trash ui-align-center' onClick =DeleteCandidateRoad('"+URLEncrypt.EncryptParameters1(new string[]{"DRRPCode =" + roadDetails.MAST_ER_ROAD_CODE.ToString().Trim(),"CNCode ="+roadDetails.PLAN_CN_ROAD_CODE})+"'); return false;'>Delete</a>",
                            roadDetails.PLAN_LOCK_STATUS == "Y"?"<span class='ui-icon ui-icon-locked ui-align-center'></span>":"<a href='#' title='Click here to Map/Delete Habitation details' class='ui-icon ui-icon-plusthick ui-align-center' onClick =MapOrDeleteHabitations('"+URLEncrypt.EncryptParameters1(new string[]{"DRRPCode =" + roadDetails.MAST_ER_ROAD_CODE.ToString().Trim(),"CNCode ="+roadDetails.PLAN_CN_ROAD_CODE})+"'); return false;'>Delete</a>",
                        }
                }).ToArray();

            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                totalRecords = 0;
                IsFinalized = String.Empty;
                return null;
            }
            finally
            {
                dbContext.Dispose();
            }
        }

        /// <summary>
        /// maps the DRRP details with the candidate road
        /// </summary>
        /// <param name="model">contains the mapping details</param>
        /// <param name="message"></param>
        /// <returns></returns>
        public bool MapCandidateRoad(CandidateRoadViewModel model, ref string message)
        {
            PMGSYEntities dbContext = new PMGSYEntities();
            try
            {
                using (TransactionScope ts = new TransactionScope())
                {

                    //check whether the road is already mapped with the same Candidate Road or not
                    if (dbContext.PLAN_ROAD_DRRP.Any(m => m.PLAN_CN_ROAD_CODE == model.CNCode && m.MAST_ER_ROAD_CODE == model.DRRPCode))
                    {
                        message = "DRRP is already mapped with the Candidate Road.";
                        return false;
                    }

                    //check whether the DRRP on which candidate  road is made is mapped with the same Candidate Road
                    if (dbContext.PLAN_ROAD.Any(m => m.PLAN_CN_ROAD_CODE == model.CNCode && m.MAST_ER_ROAD_CODE == model.DRRPCode && m.MAST_PMGSY_SCHEME == PMGSYSession.Current.PMGSYScheme))
                    {
                        message = "DRRP on which the candidate road is made can not be mapped with the same candidate road.";
                        return false;
                    }

                    //if (dbContext.PLAN_ROAD_DRRP.Any(m => m.MAST_ER_ROAD_CODE == model.DRRPCode))
                    //{
                    //    decimal? mappedDRRPLength = dbContext.PLAN_ROAD_DRRP.Where(m=>m.MAST_ER_ROAD_CODE == model.DRRPCode).Sum(m => m.PLAN_RD_LENGTH);
                    //    decimal? drrpLength = (dbContext.MASTER_EXISTING_ROADS.Where(m => m.MAST_ER_ROAD_CODE == model.DRRPCode).Select(m => m.MAST_ER_ROAD_END_CHAIN).FirstOrDefault() - dbContext.MASTER_EXISTING_ROADS.Where(m => m.MAST_ER_ROAD_CODE == model.DRRPCode).Select(m => m.MAST_ER_ROAD_STR_CHAIN).FirstOrDefault());
                    //    if (dbContext.MASTER_EXISTING_ROADS.Any(m => m.MAST_ER_ROAD_CODE == model.DRRPCode))
                    //    {
                    //        decimal? allowedDRRPLength =  drrpLength - mappedDRRPLength;
                    //        if (allowedDRRPLength < model.LengthOfRoad)
                    //        {
                    //            message = "The Total Length of DRRP ( "+drrpLength+" )  should be equal to the total of DRRP Length mapped to the Candidate Roads.";
                    //            return false;
                    //        }
                    //    }
                    //}


                    //validation for checking the Total Candidate Road length with the mapped road length
                    //decimal? totalCandidateRoadLength = dbContext.PLAN_ROAD.Where(m=>m.PLAN_CN_ROAD_CODE == model.CNCode).Select(m=>m.PLAN_RD_TOTAL_LEN).FirstOrDefault();
                    PLAN_ROAD roadMaster = dbContext.PLAN_ROAD.Where(m => m.PLAN_CN_ROAD_CODE == model.CNCode).FirstOrDefault();
                    decimal? totalMappedRoadLength = 0;
                    if (dbContext.PLAN_ROAD_DRRP.Any(m => m.PLAN_CN_ROAD_CODE == model.CNCode))
                    {
                        totalMappedRoadLength = dbContext.PLAN_ROAD_DRRP.Where(m => m.PLAN_CN_ROAD_CODE == model.CNCode).Sum(m => m.PLAN_RD_LENGTH);
                        if (totalMappedRoadLength == null)
                        {
                            totalMappedRoadLength = 0;
                        }
                        totalMappedRoadLength = totalMappedRoadLength + model.LengthOfRoad + roadMaster.PLAN_RD_LENGTH;
                    }
                    else
                    {
                        totalMappedRoadLength = model.LengthOfRoad + roadMaster.PLAN_RD_LENGTH;
                    }

                    if (roadMaster.PLAN_RD_TOTAL_LEN < totalMappedRoadLength)
                    {
                        ///Changes for RCPLWE
                        message = (PMGSYSession.Current.PMGSYScheme == 2) ? "Total of mapped DRRP road length exceeding the total length of Candidate Road." : "Total of mapped DRRP road length exceeding the total length of RCPLWE Road.";
                        return false;
                    }


                    PLAN_ROAD_DRRP mappingMaster = new PLAN_ROAD_DRRP();
                    mappingMaster.PLAN_CN_ROAD_CODE = model.CNCode;
                    mappingMaster.MAST_ER_ROAD_CODE = model.DRRPCode;
                    mappingMaster.PLAN_LOCK_STATUS = "N";
                    mappingMaster.PLAN_RD_FROM_CHAINAGE = model.StartChainage;
                    mappingMaster.PLAN_RD_TO_CHAINAGE = model.EndChainage;
                    mappingMaster.PLAN_RD_LENG = model.LengthTypeOfRoad;
                    mappingMaster.PLAN_RD_LENGTH = model.LengthOfRoad;
                    mappingMaster.USERID = PMGSYSession.Current.UserId;
                    mappingMaster.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                    dbContext.PLAN_ROAD_DRRP.Add(mappingMaster);
                    dbContext.SaveChanges();

                    //mapping DRRP Habitations
                    var lstDRRPHabs = dbContext.MASTER_ER_HABITATION_ROAD.Where(m => m.MAST_ER_ROAD_CODE == model.DRRPCode).ToList();
                    foreach (var item in lstDRRPHabs)
                    {
                        if (!dbContext.PLAN_ROAD_HABITATION.Any(m => m.PLAN_CN_ROAD_CODE == model.CNCode && m.MAST_HAB_CODE == item.MAST_HAB_CODE))
                        {
                            PLAN_ROAD_HABITATION cnHabMapping = new PLAN_ROAD_HABITATION();
                            cnHabMapping.MAST_HAB_CODE = item.MAST_HAB_CODE;
                            cnHabMapping.PLAN_CN_ROAD_CODE = model.CNCode;
                            cnHabMapping.PLAN_CN_ROAD_HAB_ID = (dbContext.PLAN_ROAD_HABITATION.Any() ? dbContext.PLAN_ROAD_HABITATION.Max(m => m.PLAN_CN_ROAD_HAB_ID) + 1 : 1);
                            cnHabMapping.USERID = PMGSYSession.Current.UserId;
                            cnHabMapping.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                            dbContext.PLAN_ROAD_HABITATION.Add(cnHabMapping);
                            dbContext.SaveChanges();
                        }
                    }
                    ts.Complete();
                }
                message = "DRRP Road mapped successfully";
                return true;
            }
            catch (Exception ex)
            {
                //Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                ErrorLog.LogError(ex, "MapCandidateRoad.DAL()");
                return false;
            }
            finally
            {
                dbContext.Dispose();
            }
        }

        /// <summary>
        /// deletes the mapping details of DRRP with associated Candidate Road
        /// </summary>
        /// <param name="DRRPCode">Existing Road Id</param>
        /// <param name="CNCode">Candidate Road Code</param>
        /// <returns></returns>
        public bool DeleteMappedDRRPDetails(int DRRPCode, int CNCode)
        {
            PMGSYEntities dbContext = new PMGSYEntities();
            try
            {
                using (TransactionScope ts = new TransactionScope())
                {
                    PLAN_ROAD_DRRP mappedMaster = dbContext.PLAN_ROAD_DRRP.Where(m => m.PLAN_CN_ROAD_CODE == CNCode && m.MAST_ER_ROAD_CODE == DRRPCode).FirstOrDefault();
                    if (mappedMaster != null)
                    {
                        dbContext.PLAN_ROAD_DRRP.Remove(mappedMaster);
                        dbContext.SaveChanges();

                        //removing the mapped habitations of current DRRP
                        var drrpMappedHabs = dbContext.MASTER_ER_HABITATION_ROAD.Where(m => m.MAST_ER_ROAD_CODE == DRRPCode && m.MASTER_EXISTING_ROADS.MAST_PMGSY_SCHEME == 2).Select(m => m.MAST_HAB_CODE).Distinct().ToList();

                        var mappedDRRPHabList = (from item in dbContext.PLAN_ROAD_DRRP
                                                 join
                                                 erHab in dbContext.MASTER_ER_HABITATION_ROAD
                                                 on item.MAST_ER_ROAD_CODE equals erHab.MAST_ER_ROAD_CODE
                                                 where
                                                 item.MAST_ER_ROAD_CODE != DRRPCode &&
                                                 item.PLAN_CN_ROAD_CODE == CNCode
                                                 select new
                                                 {
                                                     erHab.MAST_HAB_CODE
                                                 }).Distinct().GroupBy(m => m.MAST_HAB_CODE).Select(m => m.FirstOrDefault()).ToList();


                        var lstHabsPlanRoad = (from item in dbContext.PLAN_ROAD
                                               join
                                               erHab in dbContext.MASTER_ER_HABITATION_ROAD
                                               on item.MAST_ER_ROAD_CODE equals erHab.MAST_ER_ROAD_CODE
                                               where
                                               item.MAST_ER_ROAD_CODE != DRRPCode &&
                                               item.PLAN_CN_ROAD_CODE == CNCode
                                               select new
                                               {
                                                   erHab.MAST_HAB_CODE
                                               }).Distinct().GroupBy(m => m.MAST_HAB_CODE).Select(m => m.FirstOrDefault()).ToList();

                        mappedDRRPHabList = mappedDRRPHabList.Union(lstHabsPlanRoad).ToList();

                        List<int> lstSameHabs = null;

                        if (mappedDRRPHabList.Any(m => drrpMappedHabs.Contains(m.MAST_HAB_CODE)))
                        {
                            lstSameHabs = mappedDRRPHabList.Where(m => drrpMappedHabs.Contains(m.MAST_HAB_CODE)).Select(m => m.MAST_HAB_CODE).ToList();
                        }

                        var habsToRemove = (lstSameHabs == null ? drrpMappedHabs : drrpMappedHabs.Where(m => !lstSameHabs.Contains(m)).ToList());

                        if (habsToRemove != null)
                        {
                            foreach (var item in habsToRemove)
                            {
                                PLAN_ROAD_HABITATION cnHabMapping = dbContext.PLAN_ROAD_HABITATION.Where(m => m.PLAN_CN_ROAD_CODE == CNCode && m.MAST_HAB_CODE == item).FirstOrDefault();
                                if (cnHabMapping != null)
                                {
                                    dbContext.PLAN_ROAD_HABITATION.Remove(cnHabMapping);
                                    dbContext.SaveChanges();
                                }
                            }
                        }
                        ts.Complete();
                        return true;
                    }
                    else
                    {
                        return false;
                    }

                }
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

        /// <summary>
        /// finalizes the mapped DRRP details 
        /// </summary>
        /// <param name="CNCode"></param>
        /// <returns></returns>
        public bool FinalizeMappedDRRPDetails(int CNCode)
        {
            PMGSYEntities dbContext = new PMGSYEntities();
            try
            {
                var lstMappedDetails = dbContext.PLAN_ROAD_DRRP.Where(m => m.PLAN_CN_ROAD_CODE == CNCode).ToList();
                if (lstMappedDetails != null)
                {
                    lstMappedDetails.ForEach(m => m.PLAN_LOCK_STATUS = "Y");
                    dbContext.SaveChanges();
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
                dbContext.Dispose();
            }
        }

        /// <summary>
        /// definalizes the mapped DRRP details 
        /// </summary>
        /// <param name="CNCode"></param>
        /// <returns></returns>
        public bool DeFinalizeMappedDRRPDetails(int CNCode)
        {
            PMGSYEntities dbContext = new PMGSYEntities();
            try
            {
                var lstMappedDetails = dbContext.PLAN_ROAD_DRRP.Where(m => m.PLAN_CN_ROAD_CODE == CNCode).ToList();
                if (lstMappedDetails != null)
                {
                    lstMappedDetails.ForEach(m => m.PLAN_LOCK_STATUS = "N");
                    dbContext.SaveChanges();
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
                dbContext.Dispose();
            }
        }

        /// <summary>
        /// returns the list of Habitations mapped with the DRRP
        /// </summary>
        /// <param name="DRRPCode"></param>
        /// <param name="CNCode"></param>
        /// <param name="page"></param>
        /// <param name="rows"></param>
        /// <param name="sidx"></param>
        /// <param name="sord"></param>
        /// <param name="totalRecords"></param>
        /// <returns></returns>
        public Array ListDRRPMappedHabitations(int DRRPCode, int CNCode, int page, int rows, string sidx, string sord, out long totalRecords)
        {
            PMGSYEntities dbContext = new PMGSYEntities();
            try
            {
                var lstHabitations = (from item in dbContext.MASTER_ER_HABITATION_ROAD
                                      join habDetails in dbContext.MASTER_HABITATIONS_DETAILS
                                      on item.MAST_HAB_CODE equals habDetails.MAST_HAB_CODE
                                      join candidateDetails in dbContext.PLAN_ROAD_HABITATION
                                      on habDetails.MAST_HAB_CODE equals candidateDetails.MAST_HAB_CODE
                                      join cnDRRPMapping in dbContext.PLAN_ROAD_DRRP
                                      on
                                      candidateDetails.PLAN_CN_ROAD_CODE equals cnDRRPMapping.PLAN_CN_ROAD_CODE
                                      where
                                      item.MAST_ER_ROAD_CODE == DRRPCode
                                      select new
                                      {
                                          habDetails.MAST_HAB_CODE,
                                          habDetails.MASTER_HABITATIONS.MAST_HAB_NAME,
                                          item.MASTER_EXISTING_ROADS.MASTER_BLOCK.MAST_BLOCK_NAME,
                                          habDetails.MAST_YEAR,
                                          habDetails.MAST_HAB_TOT_POP,
                                          habDetails.MAST_HAB_SCST_POP,
                                          item.MASTER_EXISTING_ROADS.MAST_ER_ROAD_NUMBER,
                                          item.MASTER_EXISTING_ROADS.MAST_ER_ROAD_CODE
                                      }).Distinct().ToList();

                totalRecords = lstHabitations.Count();

                if (sidx.Trim() != string.Empty)
                {
                    if (sord.ToString() == "asc")
                    {
                        switch (sidx)
                        {
                            case "MAST_HAB_NAME":
                                lstHabitations = lstHabitations.OrderBy(x => x.MAST_HAB_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                            case "MASTER_CENSUS_YEAR":
                                lstHabitations = lstHabitations.OrderBy(x => x.MAST_YEAR).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                            case "MAST_BLOCK_NAME":
                                lstHabitations = lstHabitations.OrderBy(x => x.MAST_BLOCK_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                            case "MAST_ER_ROAD_NUMBER":
                                lstHabitations = lstHabitations.OrderBy(x => x.MAST_ER_ROAD_NUMBER).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                            case "MAST_HAB_TOT_POP":
                                lstHabitations = lstHabitations.OrderBy(x => x.MAST_HAB_TOT_POP).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                            case "MAST_HAB_SCST_POP":
                                lstHabitations = lstHabitations.OrderBy(x => x.MAST_HAB_SCST_POP).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                            default:
                                lstHabitations = lstHabitations.OrderBy(x => x.MAST_HAB_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                        }
                    }
                    else
                    {
                        switch (sidx)
                        {
                            case "MAST_HAB_NAME":
                                lstHabitations = lstHabitations.OrderByDescending(x => x.MAST_HAB_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                            case "MASTER_CENSUS_YEAR":
                                lstHabitations = lstHabitations.OrderByDescending(x => x.MAST_YEAR).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                            case "MAST_BLOCK_NAME":
                                lstHabitations = lstHabitations.OrderByDescending(x => x.MAST_BLOCK_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                            case "MAST_ER_ROAD_NUMBER":
                                lstHabitations = lstHabitations.OrderByDescending(x => x.MAST_ER_ROAD_NUMBER).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                            case "MAST_HAB_TOT_POP":
                                lstHabitations = lstHabitations.OrderByDescending(x => x.MAST_HAB_TOT_POP).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                            case "MAST_HAB_SCST_POP":
                                lstHabitations = lstHabitations.OrderByDescending(x => x.MAST_HAB_SCST_POP).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                            default:
                                lstHabitations = lstHabitations.OrderByDescending(x => x.MAST_HAB_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                        }
                    }
                }
                else
                {
                    lstHabitations = lstHabitations.OrderByDescending(x => x.MAST_HAB_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                }

                return lstHabitations.Select(habDetails => new
                {
                    cell = new[]
                        {
                            habDetails.MAST_HAB_NAME == null?"-":habDetails.MAST_HAB_NAME.ToString(),
                            habDetails.MAST_BLOCK_NAME == null?"-":habDetails.MAST_BLOCK_NAME.ToString(),
                            habDetails.MAST_ER_ROAD_NUMBER == null?"-":habDetails.MAST_ER_ROAD_NUMBER.ToString(),
                            habDetails.MAST_YEAR == null?"-":habDetails.MAST_YEAR.ToString(),
                            habDetails.MAST_HAB_TOT_POP == null?"-":habDetails.MAST_HAB_TOT_POP.ToString(),
                            habDetails.MAST_HAB_SCST_POP == null?"-":habDetails.MAST_HAB_SCST_POP.ToString(),
                            "<a href='#' title='Click here to delete Habitation details' class='ui-icon ui-icon-trash ui-align-center' onClick =DeleteHabitation('"+URLEncrypt.EncryptParameters1(new string[]{"HabitationCode =" + habDetails.MAST_HAB_CODE.ToString().Trim(),"DRRPCode ="+habDetails.MAST_ER_ROAD_CODE,"CNCode ="+CNCode})+"'); return false;'>Delete</a>",
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
        /// deletes the entry of habitation mapped with DRRP
        /// </summary>
        /// <param name="DRRPCode"></param>
        /// <param name="CNCode"></param>
        /// <param name="habCode"></param>
        /// <returns></returns>
        public bool DeleteMappedDRRPHabitation(int DRRPCode, int CNCode, int habCode)
        {
            PMGSYEntities dbContext = new PMGSYEntities();
            try
            {

                //if (dbContext.IMS_BENEFITED_HABS.Any(m => m.MAST_HAB_CODE == habCode))
                //{
                //    return false;
                //}

                var habContainingList = (from item in dbContext.PLAN_ROAD_DRRP
                                         join drrpHabs in dbContext.MASTER_ER_HABITATION_ROAD
                                         on item.MAST_ER_ROAD_CODE equals drrpHabs.MAST_ER_ROAD_CODE
                                         where item.MAST_ER_ROAD_CODE != DRRPCode
                                         &&
                                         drrpHabs.MAST_HAB_CODE == habCode
                                         select new
                                         {
                                             drrpHabs.MAST_ER_ROAD_ID
                                         }).ToList();

                if (habContainingList.Count() > 0)
                {
                    return true;
                }
                else
                {
                    PLAN_ROAD_HABITATION cnHabMapping = dbContext.PLAN_ROAD_HABITATION.Where(m => m.MAST_HAB_CODE == habCode && m.PLAN_CN_ROAD_CODE == CNCode).FirstOrDefault();
                    dbContext.PLAN_ROAD_HABITATION.Remove(cnHabMapping);
                    dbContext.SaveChanges();
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

        /// <summary>
        /// returns the lock status of DRRP Road
        /// </summary>
        /// <param name="cnCode"></param>
        /// <returns></returns>
        public String GetLockStatusOfCandidateRoad(int cnCode)
        {
            PMGSYEntities dbContext = new PMGSYEntities();
            try
            {
                if (dbContext.PLAN_ROAD_DRRP.Any(m => m.PLAN_CN_ROAD_CODE == cnCode && m.PLAN_LOCK_STATUS == "N"))
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
                return "";
            }
            finally
            {
                dbContext.Dispose();
            }
        }

        /// <summary>
        /// returns the Mapped DRRP details 
        /// </summary>
        /// <param name="cnCode"></param>
        /// <param name="drrpCode"></param>
        /// <returns></returns>
        public CandidateRoadViewModel GetDRRPDetails(int cnCode, int drrpCode)
        {
            PMGSYEntities dbContext = new PMGSYEntities();
            CandidateRoadViewModel model = new CandidateRoadViewModel();
            try
            {
                if (dbContext.PLAN_ROAD_DRRP.Any(m => m.MAST_ER_ROAD_CODE == drrpCode && m.PLAN_CN_ROAD_CODE == cnCode))
                {
                    PLAN_ROAD planRoadDetails = dbContext.PLAN_ROAD.Where(m => m.PLAN_CN_ROAD_CODE == cnCode).FirstOrDefault();
                    PLAN_ROAD_DRRP drrpDetails = dbContext.PLAN_ROAD_DRRP.Where(m => m.PLAN_CN_ROAD_CODE == cnCode && m.MAST_ER_ROAD_CODE == drrpCode).FirstOrDefault();
                    MASTER_EXISTING_ROADS drrpMaster = dbContext.MASTER_EXISTING_ROADS.Where(m => m.MAST_ER_ROAD_CODE == drrpCode).FirstOrDefault();
                    model.BlockCode = drrpMaster.MAST_BLOCK_CODE;
                    model.CNCode = drrpDetails.PLAN_CN_ROAD_CODE;
                    model.DRRPCode = drrpDetails.MAST_ER_ROAD_CODE;
                    model.EndChainage = drrpDetails.PLAN_RD_TO_CHAINAGE.Value;
                    model.ExistStartChainage = drrpMaster.MAST_ER_ROAD_STR_CHAIN;
                    model.ExistEndChainage = drrpMaster.MAST_ER_ROAD_END_CHAIN;
                    model.LengthOfRoad = drrpDetails.PLAN_RD_LENGTH.Value;
                    model.LengthTypeOfRoad = drrpDetails.PLAN_RD_LENG;
                    return model;
                }
                else
                {
                    return model;
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


        #endregion

        #region FILE_UPLOAD

        /// <summary>
        /// save operation of file details
        /// </summary>
        /// <param name="list">list of file details</param>
        /// <returns>response message</returns>
        public string AddFileUploadDetailsDAL(List<PLAN_ROAD_UPLOAD_FILE> list)
        {
            PMGSYEntities dbContext = new PMGSYEntities();
            try
            {
                using (TransactionScope ts = new TransactionScope())
                {
                    Int32? MaxID;
                    foreach (PLAN_ROAD_UPLOAD_FILE fileModel in list)
                    {
                        //if (dbContext.PLAN_ROAD.Any(m => m.PLAN_CN_ROAD_CODE == fileModel.PLAN_CN_ROAD_CODE))
                        //{
                        //    PLAN_ROAD planRoad = dbContext.PLAN_ROAD.Where(m => m.PLAN_CN_ROAD_CODE == fileModel.PLAN_CN_ROAD_CODE).FirstOrDefault();
                        //    decimal? totalLenth = planRoad.PLAN_RD_LENGTH;
                        //    if (totalLenth < (fileModel.PLAN_END_CHAINAGE - fileModel.PLAN_START_CHAINAGE))
                        //    {
                        //        return "Chainage must be less than or equal to the road length.";
                        //    }
                        //}

                        if (dbContext.PLAN_ROAD_UPLOAD_FILE.Count() == 0)
                        {
                            MaxID = 0;
                        }
                        else
                        {
                            MaxID = (from c in dbContext.PLAN_ROAD_UPLOAD_FILE select (Int32?)c.PLAN_FILE_ID ?? 0).Max();
                        }
                        ++MaxID;
                        fileModel.PLAN_FILE_ID = Convert.ToInt32(MaxID);
                        //fileModel.USERID = PMGSYSession.Current.UserId;
                        //fileModel.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                        dbContext.PLAN_ROAD_UPLOAD_FILE.Add(fileModel);
                    }
                    dbContext.SaveChanges();
                    ts.Complete();
                }

                return string.Empty;
            }
            catch (DbUpdateException ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);

                return ("An Error Occurred While Your Processing Request.");
            }
            catch (OptimisticConcurrencyException ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                return ("An Error Occurred While Your Processing Request.");
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                return ("Error Occurred While Processing Request.");
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
        /// populates the list of Files 
        /// </summary>
        /// <param name="page">no. of pages</param>
        /// <param name="rows">no. of rows</param>
        /// <param name="sidx">sort column</param>
        /// <param name="sord">sort order</param>
        /// <param name="totalRecords">no of total records</param>
        /// <param name="roadCode">core network code</param>
        /// <returns></returns>
        public Array GetFilesListDAL(int page, int rows, string sidx, string sord, out long totalRecords, int roadCode)
        {
            PMGSYEntities dbContext = new PMGSYEntities();
            try
            {
                List<PLAN_ROAD_UPLOAD_FILE> listFiles = dbContext.PLAN_ROAD_UPLOAD_FILE.Where(p => p.PLAN_CN_ROAD_CODE == roadCode).ToList();
                IQueryable<PLAN_ROAD_UPLOAD_FILE> query = listFiles.AsQueryable<PLAN_ROAD_UPLOAD_FILE>();
                totalRecords = listFiles.Count();

                if (sidx.Trim() != string.Empty)
                {
                    if (sord.ToString() == "asc")
                    {
                        query = query.OrderBy(x => x.PLAN_CN_ROAD_CODE).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                    }
                    else
                    {
                        query = query.OrderByDescending(x => x.PLAN_CN_ROAD_CODE).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                    }
                }
                else
                {
                    query = query.OrderBy(x => x.PLAN_CN_ROAD_CODE).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                }

                return query.Select(fileDetails => new
                {
                    id = fileDetails.PLAN_FILE_ID + "$" + fileDetails.PLAN_CN_ROAD_CODE,
                    cell = new[] {                                       
                                    URLEncrypt.EncryptParameters(new string[] { fileDetails.PLAN_FILE_NAME}),                                    
                                    fileDetails.PLAN_START_CHAINAGE.ToString(),
                                    fileDetails.PLAN_END_CHAINAGE.ToString(),
                                    Convert.ToDateTime(fileDetails.PLAN_UPLOAD_DATE).ToString("dd/MM/yyyy"),
                                    "<a href='#' title='Click here to Edit the File Details' class='ui-icon ui-icon-pencil ui-align-center' onClick=EditFileDetails('" +  fileDetails.PLAN_FILE_ID.ToString().Trim()  + "$" + fileDetails.PLAN_CN_ROAD_CODE.ToString().Trim() +"'); return false;>Edit</a>",
                                    "<a href='#' title='Click here to delete the File and File Details' class='ui-icon ui-icon-trash ui-align-center' onClick=DeleteFileDetails('" + fileDetails.PLAN_FILE_ID.ToString().Trim()  + "$" + fileDetails.PLAN_CN_ROAD_CODE.ToString().Trim() + "','" + fileDetails.PLAN_FILE_NAME +"'); return false;'>Delete</a>",
                                    "<center><table><tr><td style='border-color:white'><a href='#' style='float:left' id='btnSave"+  fileDetails.PLAN_FILE_ID.ToString().Trim()  + "$" + fileDetails.PLAN_CN_ROAD_CODE.ToString().Trim()  +"' title='Click here to Save the File Details' class='ui-icon ui-icon-disk ui-align-center' onClick=SaveDetails('" +  fileDetails.PLAN_FILE_ID.ToString().Trim() + "$" + fileDetails.PLAN_CN_ROAD_CODE.ToString().Trim() + "');></a><a href='#' style='float:right' id='btnCancel" +  fileDetails.PLAN_FILE_ID.ToString().Trim()  + "$" + fileDetails.PLAN_CN_ROAD_CODE.ToString().Trim()  +"' title='Click here to Cancel the File Edit' class='ui-icon ui-icon-closethick ui-align-center' onClick= CancelSaveDetails('" +  fileDetails.PLAN_FILE_ID.ToString().Trim() + "$" + fileDetails.PLAN_CN_ROAD_CODE.ToString().Trim() + "');></a></td></tr></table></center>"
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

        /// <summary>
        /// Update the details associated with the file
        /// </summary>
        /// <param name="model">contains the updated file details </param>
        /// <returns></returns>
        public string UpdateFileDetailsDAL(PLAN_ROAD_UPLOAD_FILE model, ref string message)
        {
            PMGSYEntities dbContext = new PMGSYEntities();
            try
            {
                //if (dbContext.PLAN_ROAD.Any(m => m.PLAN_CN_ROAD_CODE == model.PLAN_CN_ROAD_CODE))
                //{
                //    PLAN_ROAD planRoad = dbContext.PLAN_ROAD.Where(m => m.PLAN_CN_ROAD_CODE == model.PLAN_CN_ROAD_CODE).FirstOrDefault();
                //    decimal? totalLenth = planRoad.PLAN_RD_LENGTH;
                //    if (totalLenth < (model.PLAN_END_CHAINAGE - model.PLAN_START_CHAINAGE))
                //    {
                //        message = "Chainage must be less than or equal to the road length.";
                //        return message;
                //    }
                //}

                PLAN_ROAD_UPLOAD_FILE files = dbContext.PLAN_ROAD_UPLOAD_FILE.Where(
                    a => a.PLAN_FILE_ID == model.PLAN_FILE_ID &&
                    a.PLAN_CN_ROAD_CODE == model.PLAN_CN_ROAD_CODE
                    ).FirstOrDefault();

                files.PLAN_START_CHAINAGE = model.PLAN_START_CHAINAGE;
                files.PLAN_END_CHAINAGE = model.PLAN_END_CHAINAGE;
                //files.USERID = PMGSYSession.Current.UserId;
                //files.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                dbContext.Entry(files).State = System.Data.Entity.EntityState.Modified;
                dbContext.SaveChanges();

                return string.Empty;
            }
            catch (DbUpdateException ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);

                return ("An Error Occurred While Your Processing Request.");
            }
            catch (OptimisticConcurrencyException ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                return ("An Error Occurred While Your Processing Request.");
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                return ("Error Occurred While Processing Request.");
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
        /// delete file operation
        /// </summary>
        /// <param name="files">model containing file details </param>
        /// <returns>message of operation</returns>
        public string DeleteFileDetails(PLAN_ROAD_UPLOAD_FILE files)
        {
            PMGSYEntities dbContext = new PMGSYEntities();
            try
            {
                PLAN_ROAD_UPLOAD_FILE listfiles = dbContext.PLAN_ROAD_UPLOAD_FILE.Where(
                    a => a.PLAN_CN_ROAD_CODE == files.PLAN_CN_ROAD_CODE &&
                    a.PLAN_FILE_ID == files.PLAN_FILE_ID &&
                    a.PLAN_FILE_NAME == files.PLAN_FILE_NAME).FirstOrDefault();


                //listfiles.USERID = PMGSYSession.Current.UserId;
                //listfiles.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                dbContext.Entry(listfiles).State = System.Data.Entity.EntityState.Modified;
                dbContext.SaveChanges();


                dbContext.PLAN_ROAD_UPLOAD_FILE.Remove(listfiles);
                dbContext.SaveChanges();
                return string.Empty;
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                return "There is an error while processing request.";
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

        #region RCPLWE
        public Array GetRCPLWEList(int stateCode, int districtCode, int blockCode, string roadType, int roadCode, string roadName, int page, int rows, string sidx, string sord, out long totalRecords, int CnCode)
        {
            string filters = string.Empty;
            string nameSearch = string.Empty;
            PMGSYEntities dbContext = new PMGSYEntities();
            try
            {
                // new change with data from stored procedure
                if (roadType == "0")
                {
                    roadType = null;
                }

                if (PMGSYSession.Current.RoleCode == 2)
                {
                    stateCode = PMGSYSession.Current.StateCode;
                }
                else if (PMGSYSession.Current.RoleCode == 22 || PMGSYSession.Current.RoleCode == 54)
                {
                    stateCode = PMGSYSession.Current.StateCode;
                    districtCode = PMGSYSession.Current.DistrictCode;
                }

                short roleCode = dbContext.UM_User_Master.Where(m => m.UserID == PMGSYSession.Current.UserId).Select(m => m.DefaultRoleID).FirstOrDefault();
                var lstPlanRoads = dbContext.GET_CORE_NETWORKS((stateCode <= 0 ? 0 : stateCode), (districtCode <= 0 ? 0 : districtCode), (blockCode <= 0 ? 0 : blockCode), roadType, roadCode, roadName, roleCode).ToList();

                lstPlanRoads = lstPlanRoads.Where(m => m.MAST_PMGSY_SCHEME == 3).ToList();

                if (CnCode > 0)
                {
                    lstPlanRoads = lstPlanRoads.Where(m => m.PLAN_CN_ROAD_CODE == CnCode).ToList();
                }

                totalRecords = lstPlanRoads.Count();

                if (sidx.Trim() != string.Empty)
                {
                    if (sord.ToString() == "asc")
                    {
                        switch (sidx)
                        {
                            case "PLAN_CN_ROAD_CODE":
                                lstPlanRoads = lstPlanRoads.OrderBy(x => x.PLAN_CN_ROAD_CODE).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                            case "PLAN_CN_ROAD_NUMBER":
                                lstPlanRoads = lstPlanRoads.OrderBy(x => x.PLAN_CN_ROAD_NUMBER).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                            //case "MAST_ER_ROAD_CODE":
                            //    lstPlanRoads = lstPlanRoads.OrderBy(x => x.MAST_ER_ROAD_CODE).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                            //    break;
                            case "PLAN_RD_NAME":
                                lstPlanRoads = lstPlanRoads.OrderBy(x => x.PLAN_RD_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                            case "PLAN_RD_FROM":
                                lstPlanRoads = lstPlanRoads.OrderBy(x => x.PLAN_RD_FROM).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                            case "PLAN_RD_TO":
                                lstPlanRoads = lstPlanRoads.OrderBy(x => x.PLAN_RD_TO).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                            case "PLAN_RD_FROM_CHAINAGE":
                                lstPlanRoads = lstPlanRoads.OrderBy(x => x.PLAN_RD_FROM_CHAINAGE).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                            case "PLAN_RD_TO_CHAINAGE":
                                lstPlanRoads = lstPlanRoads.OrderBy(x => x.PLAN_RD_TO_CHAINAGE).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                            case "PLAN_RD_LENGTH":
                                lstPlanRoads = lstPlanRoads.OrderBy(x => x.PLAN_RD_LENGTH).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                            case "PLAN_RD_TOTAL_LENGTH":
                                lstPlanRoads = lstPlanRoads.OrderBy(x => x.PLAN_RD_TOTAL_LEN).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                            case "PLAN_RD_ROUTE":
                                lstPlanRoads = lstPlanRoads.OrderBy(x => x.PLAN_RD_ROUTE).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                            default:
                                lstPlanRoads = lstPlanRoads.OrderBy(x => x.PLAN_CN_ROAD_CODE).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                        }
                    }
                    else
                    {
                        switch (sidx)
                        {
                            case "PLAN_CN_ROAD_CODE":
                                lstPlanRoads = lstPlanRoads.OrderByDescending(x => x.PLAN_CN_ROAD_CODE).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                            case "PLAN_CN_ROAD_NUMBER":
                                lstPlanRoads = lstPlanRoads.OrderByDescending(x => x.PLAN_CN_ROAD_NUMBER).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                            //case "MAST_ER_ROAD_CODE":
                            //    lstPlanRoads = lstPlanRoads.OrderByDescending(x => x.MAST_ER_ROAD_CODE).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                            //    break;
                            case "PLAN_RD_NAME":
                                lstPlanRoads = lstPlanRoads.OrderByDescending(x => x.PLAN_RD_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                            case "PLAN_RD_FROM":
                                lstPlanRoads = lstPlanRoads.OrderByDescending(x => x.PLAN_RD_FROM).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                            case "PLAN_RD_TO":
                                lstPlanRoads = lstPlanRoads.OrderByDescending(x => x.PLAN_RD_TO).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                            case "PLAN_RD_FROM_CHAINAGE":
                                lstPlanRoads = lstPlanRoads.OrderByDescending(x => x.PLAN_RD_FROM_CHAINAGE).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                            case "PLAN_RD_TO_CHAINAGE":
                                lstPlanRoads = lstPlanRoads.OrderByDescending(x => x.PLAN_RD_TO_CHAINAGE).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                            case "PLAN_RD_LENGTH":
                                lstPlanRoads = lstPlanRoads.OrderByDescending(x => x.PLAN_RD_LENGTH).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                            case "PLAN_RD_TOTAL_LENGTH":
                                lstPlanRoads = lstPlanRoads.OrderByDescending(x => x.PLAN_RD_TOTAL_LEN).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                            case "PLAN_RD_ROUTE":
                                lstPlanRoads = lstPlanRoads.OrderByDescending(x => x.PLAN_RD_ROUTE).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                            default:
                                lstPlanRoads = lstPlanRoads.OrderByDescending(x => x.PLAN_CN_ROAD_CODE).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                        }
                    }
                }
                else
                {
                    lstPlanRoads = lstPlanRoads.OrderByDescending(x => x.PLAN_CN_ROAD_CODE).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                }

                var result = lstPlanRoads.Select(roadDetails => new
                {
                    //roadDetails.MAST_ER_ROAD_NUMBER,
                    roadDetails.BLOCK_NAME,
                    roadDetails.DISTRICT_NAME,
                    //roadDetails.ER,
                    roadDetails.STATE_NAME,
                    roadDetails.PLAN_CN_ROAD_CODE,
                    roadDetails.PLAN_CN_ROAD_NUMBER,
                    //roadDetails.PLAN_LOCK_STATUS,
                    roadDetails.PLAN_RD_BLOCK_FROM_CODE,
                    roadDetails.PLAN_RD_BLOCK_TO_CODE,
                    roadDetails.PLAN_RD_FROM_CHAINAGE,
                    roadDetails.PLAN_RD_FROM_HAB,
                    roadDetails.PLAN_RD_FROM_TYPE,
                    roadDetails.PLAN_RD_LENG,
                    roadDetails.PLAN_RD_LENGTH,
                    roadDetails.PLAN_RD_NAME,
                    roadDetails.PLAN_RD_NUM_FROM,
                    roadDetails.PLAN_RD_NUM_TO,
                    roadDetails.PLAN_RD_ROUTE,
                    roadDetails.PLAN_RD_TO_CHAINAGE,
                    roadDetails.PLAN_RD_TO_HAB,
                    roadDetails.PLAN_RD_TO_TYPE,
                    roadDetails.PLAN_RD_FROM,
                    roadDetails.PLAN_RD_TO,
                    roadDetails.UNLOCK_BY_MORD,
                    roadDetails.MAST_ER_ROAD_NUMBER,
                    roadDetails.PLAN_RD_TOTAL_LEN
                }).ToArray();

                return result.Select(roadDetails => new
                {
                    id = URLEncrypt.EncryptParameters1(new string[] { "NetworkCode=" + roadDetails.PLAN_CN_ROAD_CODE.ToString() }),
                    cell = new[]
                {
                    roadDetails.PLAN_CN_ROAD_CODE.ToString().Trim(),
                    roadDetails.PLAN_RD_ROUTE == null?string.Empty:roadDetails.PLAN_RD_ROUTE.ToString(),
                    roadDetails.PLAN_CN_ROAD_NUMBER == null?string.Empty:roadDetails.PLAN_CN_ROAD_NUMBER.ToString(),
                    roadDetails.PLAN_RD_NAME == null?string.Empty:roadDetails.PLAN_RD_NAME.ToString(),
                    roadDetails.MAST_ER_ROAD_NUMBER.ToString(),
                    roadDetails.PLAN_RD_FROM_TYPE=="B"?"Block("+(dbContext.MASTER_BLOCK.Where(item=>item.MAST_BLOCK_CODE == roadDetails.PLAN_RD_BLOCK_FROM_CODE).Select(m=>m.MAST_BLOCK_NAME).FirstOrDefault())+")":(roadDetails.PLAN_RD_FROM_TYPE=="H"?"Habitation("+(dbContext.MASTER_HABITATIONS.Where(item=>item.MAST_HAB_CODE == roadDetails.PLAN_RD_FROM_HAB).Select(m=>m.MAST_HAB_NAME).FirstOrDefault())+")":(roadDetails.PLAN_RD_FROM==null?string.Empty:roadDetails.PLAN_RD_FROM.ToString())),
                    roadDetails.PLAN_RD_TO_TYPE=="B"?"Block("+(dbContext.MASTER_BLOCK.Where(item=>item.MAST_BLOCK_CODE == roadDetails.PLAN_RD_BLOCK_TO_CODE).Select(m=>m.MAST_BLOCK_NAME).FirstOrDefault())+")":(roadDetails.PLAN_RD_TO_TYPE=="H"?"Habitation("+(dbContext.MASTER_HABITATIONS.Where(item=>item.MAST_HAB_CODE == roadDetails.PLAN_RD_TO_HAB).Select(m=>m.MAST_HAB_NAME).FirstOrDefault())+")":(roadDetails.PLAN_RD_TO==null?string.Empty:roadDetails.PLAN_RD_TO.ToString())),

                    roadDetails.PLAN_RD_FROM_CHAINAGE == null?string.Empty:roadDetails.PLAN_RD_FROM_CHAINAGE.ToString(),
                    roadDetails.PLAN_RD_TO_CHAINAGE == null?string.Empty:roadDetails.PLAN_RD_TO_CHAINAGE.ToString(),
                    roadDetails.PLAN_RD_LENGTH == null?string.Empty:roadDetails.PLAN_RD_LENGTH.ToString(),
                    //PMGSYSession.Current.PMGSYScheme == 2?roadDetails.PLAN_RD_TOTAL_LEN.ToString():"",
                    roadDetails.PLAN_RD_TOTAL_LEN.ToString(),
                    roadDetails.UNLOCK_BY_MORD == "M"?"<a href='#' title='Click here to map habitation Details' class='ui-icon ui-icon-unlocked ui-align-center' onClick =editHabitationDetails('"+URLEncrypt.EncryptParameters1(new string[]{"NetworkCode =" + roadDetails.PLAN_CN_ROAD_CODE.ToString().Trim(),"UnlockFlag =" + roadDetails.UNLOCK_BY_MORD.ToString().Trim()})+"'); return false;'>Delete</a>":roadDetails.UNLOCK_BY_MORD=="N"?"<a href='#' title='Click here to map habitation Details' class='ui-icon ui-icon-plusthick ui-align-center' onClick =editHabitationDetails('"+URLEncrypt.EncryptParameters1(new string[]{"NetworkCode =" + roadDetails.PLAN_CN_ROAD_CODE.ToString().Trim(),"UnlockFlag =" + roadDetails.UNLOCK_BY_MORD.ToString().Trim()})+"'); return false;'>Delete</a>":"<span class='ui-icon ui-icon-locked ui-align-center'></span>",
                    //"<a href='#' title='Click here to map habitation Details' class='ui-icon ui-icon-plusthick ui-align-center' onClick =editHabitationDetails('"+URLEncrypt.EncryptParameters1(new string[]{"NetworkCode =" + roadDetails.PLAN_CN_ROAD_CODE.ToString().Trim(),"UnlockFlag =" + roadDetails.UNLOCK_BY_MORD.ToString().Trim()})+"'); return false;'>Delete</a>",
                    (roadDetails.UNLOCK_BY_MORD == "M"?"<a href='#' title='Click here to upload file details' class='ui-icon ui-icon-unlocked ui-align-center' onClick =UploadFile('"+URLEncrypt.EncryptParameters1(new string[]{"NetworkCode =" + roadDetails.PLAN_CN_ROAD_CODE.ToString().Trim()})+"'); return false;'>Delete</a>":roadDetails.UNLOCK_BY_MORD=="N"?"<a href='#' title='Click here to upload file details' class='ui-icon ui-icon-plusthick ui-align-center' onClick =UploadFile('"+URLEncrypt.EncryptParameters1(new string[]{"NetworkCode =" + roadDetails.PLAN_CN_ROAD_CODE.ToString().Trim()})+"'); return false;'>Delete</a>":"<span class='ui-icon ui-icon-locked ui-align-center'></span></center>"),
                    //PMGSYSession.Current.PMGSYScheme == 1
                    //?"":
                    //roadDetails.UNLOCK_BY_MORD == "M"?("<a href='#' title='Click here to map other candidate road' class='ui-icon ui-icon-unlocked ui-align-center' onClick =MapOtherCandidateRoad('"+URLEncrypt.EncryptParameters1(new string[]{"NetworkCode =" + roadDetails.PLAN_CN_ROAD_CODE.ToString().Trim()})+"'); return false;'>Map Other DRRP Road</a>")
                    //:roadDetails.UNLOCK_BY_MORD == "Y"?"<span class='ui-icon ui-icon-locked ui-align-center'></span>":
                    roadDetails.UNLOCK_BY_MORD == "N" ? "<a href='#' title='Click here to map other RCPLWE road' class='ui-icon ui-icon-plusthick ui-align-center' onClick =MapOtherCandidateRoad('"+URLEncrypt.EncryptParameters1(new string[]{"NetworkCode =" + roadDetails.PLAN_CN_ROAD_CODE.ToString().Trim()})+"'); return false;'>Map Other DRRP Road</a>" :
                    roadDetails.UNLOCK_BY_MORD == "M" ? "<a href='#' title='Click here to map other RCPLWE road' class='ui-icon ui-icon-unlocked ui-align-center' onClick =MapOtherCandidateRoad('"+URLEncrypt.EncryptParameters1(new string[]{"NetworkCode =" + roadDetails.PLAN_CN_ROAD_CODE.ToString().Trim()})+"'); return false;'>Map Other DRRP Road</a>"
                     : "<span class='ui-icon ui-icon-locked ui-align-center'></span>",
                    
                    "<a href='#' title='Click here to view RCPLWE details' class='ui-icon ui-icon-zoomin ui-align-center' onClick =detailsCoreNetwork('"+URLEncrypt.EncryptParameters1(new string[]{"NetworkCode =" + roadDetails.PLAN_CN_ROAD_CODE.ToString().Trim()})+"'); return false;'>View RCPLWE Details</a>",
                    
                    roadDetails.UNLOCK_BY_MORD == "N" ? "<a href='#' title='Click here to edit RCPLWE Road details' class='ui-icon ui-icon-pencil ui-align-center' onClick =editNetworkData('"+URLEncrypt.EncryptParameters1(new string[]{"NetworkCode =" + roadDetails.PLAN_CN_ROAD_CODE.ToString().Trim(),"UnlockFlag =" + roadDetails.UNLOCK_BY_MORD.ToString().Trim()})+"'); return false;'>Edit</a>" :
                    roadDetails.UNLOCK_BY_MORD == "M" ? "<a href='#' title='Click here to edit RCPLWE Road details' class='ui-icon ui-icon-unlocked ui-align-center' onClick =editNetworkData('"+URLEncrypt.EncryptParameters1(new string[]{"NetworkCode =" + roadDetails.PLAN_CN_ROAD_CODE.ToString().Trim(),"UnlockFlag =" + roadDetails.UNLOCK_BY_MORD.ToString().Trim()})+"'); return false;'>Edit</a>" : "<span class='ui-icon ui-icon-locked ui-align-center'></span>",
                    
                    (roadDetails.UNLOCK_BY_MORD == "N" || roadDetails.UNLOCK_BY_MORD == "M") ? "<a href='#' title='Click here to delete core network details' class='ui-icon ui-icon-trash ui-align-center' onClick =deleteNetworkData('"+URLEncrypt.EncryptParameters1(new string[]{"NetworkCode =" + roadDetails.PLAN_CN_ROAD_CODE.ToString().Trim()})+"'); return false;'>Delete</a>" : "<span class='ui-icon ui-icon-locked ui-align-center'></span>"
                    
                }
                }).ToArray();


                //end of change


                //lstPlanRoads = lstPlanRoads.GroupBy(rl => rl.PLAN_CN_ROAD_CODE).Select(rl => rl.FirstOrDefault());
            }
            catch (Exception ex)
            {
                //Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                totalRecords = 0;
                dbContext.Dispose();
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

        public bool AddRCPLWE(CoreNetworkViewModel model, ref string message)
        {
            PMGSYEntities dbContext = new PMGSYEntities();
            try
            {

                decimal chainageFrom = Convert.ToDecimal(model.PLAN_RD_FROM_CHAINAGE);
                decimal chainageTo = Convert.ToDecimal(model.PLAN_RD_TO_CHAINAGE);
                int recordCount = dbContext.PLAN_ROAD.Where(m => m.MAST_STATE_CODE == model.MAST_STATE_CODE && m.MAST_DISTRICT_CODE == model.MAST_DISTRICT_CODE && m.MAST_BLOCK_CODE == model.MAST_BLOCK_CODE && m.PLAN_CN_ROAD_NUMBER == model.PLAN_CN_ROAD_NUMBER && m.PLAN_RD_FROM_CHAINAGE == chainageFrom && m.PLAN_RD_TO_CHAINAGE == chainageTo && m.MAST_PMGSY_SCHEME == PMGSYSession.Current.PMGSYScheme).Count();

                if (recordCount > 0)
                {
                    //if (PMGSYSession.Current.PMGSYScheme == 3)
                    {
                        message = "RCPLWE information already exist.";
                    }
                    return false;
                }

                PLAN_ROAD master = new PLAN_ROAD();
                master = CloneModelToObjectRCPLWE(model);

                master.PLAN_RD_ROUTE = "O";
                master.PLAN_CN_ROAD_NUMBER = "O";

                master.USERID = PMGSYSession.Current.UserId;
                master.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                master.PLAN_CN_ROAD_CODE = dbContext.PLAN_ROAD.Any() ? (from item in dbContext.PLAN_ROAD select item.PLAN_CN_ROAD_CODE).Max() + 1 : 1;
                dbContext.PLAN_ROAD.Add(master);
                dbContext.SaveChanges();
                //new change done by Vikram on 04 Feb 2014 for PMGSY Scheme 2
                /*if (PMGSYSession.Current.PMGSYScheme == 2)
                {
                    List<int> lstHabCodes = dbContext.MASTER_ER_HABITATION_ROAD.Where(m => m.MAST_ER_ROAD_CODE == master.MAST_ER_ROAD_CODE).Select(m => m.MAST_HAB_CODE).ToList();
                    if (lstHabCodes != null)
                    {
                        foreach (var item in lstHabCodes)
                        {
                            PLAN_ROAD_HABITATION mappingMaster = new PLAN_ROAD_HABITATION();
                            mappingMaster.MAST_HAB_CODE = item;
                            mappingMaster.PLAN_CN_ROAD_CODE = master.PLAN_CN_ROAD_CODE;
                            mappingMaster.PLAN_CN_ROAD_HAB_ID = dbContext.PLAN_ROAD_HABITATION.Any() ? dbContext.PLAN_ROAD_HABITATION.Max(m => m.PLAN_CN_ROAD_HAB_ID) + 1 : 1;
                            mappingMaster.USERID = PMGSYSession.Current.UserId;
                            mappingMaster.IPADD = master.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                            dbContext.PLAN_ROAD_HABITATION.Add(mappingMaster);
                            dbContext.SaveChanges();
                        }
                    }
                }*/
                //end of change


                return true;
            }
            catch (Exception ex)
            {
                //objLog.WriteErrorMessage(ex.Message);
                //Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
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
        /// copies the value of view model in entity object
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public PLAN_ROAD CloneModelToObjectRCPLWE(CoreNetworkViewModel model)
        {
            PMGSYEntities dbContext = new PMGSYEntities();
            try
            {
                PLAN_ROAD master = new PLAN_ROAD();
                master.MAST_BLOCK_CODE = model.MAST_BLOCK_CODE;
                master.MAST_DISTRICT_CODE = model.MAST_DISTRICT_CODE;
                master.MAST_ER_ROAD_CODE = model.MAST_ER_ROAD_CODE;
                master.MAST_STATE_CODE = model.MAST_STATE_CODE;
                master.PLAN_CN_ROAD_NUMBER = model.PLAN_CN_ROAD_NUMBER;
                master.PLAN_LOCK_STATUS = model.PLAN_LOCK_STATUS == null ? "N" : model.PLAN_LOCK_STATUS;
                master.PLAN_RD_BLOCK_FROM_CODE = model.PLAN_RD_BLOCK_FROM_CODE;
                master.PLAN_RD_BLOCK_TO_CODE = model.PLAN_RD_BLOCK_TO_CODE;
                master.PLAN_RD_FROM_CHAINAGE = Convert.ToDecimal(model.PLAN_RD_FROM_CHAINAGE);
                master.PLAN_RD_FROM_HAB = model.PLAN_RD_FROM_HAB;
                master.PLAN_RD_FROM_TYPE = model.PLAN_RD_FROM_TYPE;
                master.PLAN_RD_LENG = model.PLAN_RD_LENG;
                master.PLAN_RD_LENGTH = Convert.ToDecimal(model.PLAN_RD_LENGTH);
                master.PLAN_RD_NAME = (model.PLAN_RD_NAME == null ? dbContext.MASTER_EXISTING_ROADS.Where(m => m.MAST_ER_ROAD_CODE == model.MAST_ER_ROAD_CODE).Select(m => m.MAST_ER_ROAD_NAME).FirstOrDefault() : model.PLAN_RD_NAME); //model.PLAN_RD_NAME;
                master.PLAN_RD_ROUTE = model.PLAN_RD_ROUTE;
                master.PLAN_RD_TO_CHAINAGE = Convert.ToDecimal(model.PLAN_RD_TO_CHAINAGE);
                master.PLAN_RD_TO_HAB = model.PLAN_RD_TO_HAB;
                master.PLAN_RD_TO_TYPE = model.PLAN_RD_TO_TYPE;
                switch (model.PLAN_RD_FROM_TYPE)
                {
                    case "B":
                        master.PLAN_RD_FROM = "Block(" + dbContext.MASTER_BLOCK.Where(m => m.MAST_BLOCK_CODE == model.PLAN_RD_BLOCK_FROM_CODE).Select(m => m.MAST_BLOCK_NAME).FirstOrDefault() + ")";
                        master.PLAN_RD_NUM_FROM = model.PLAN_RD_NUM_FROM;
                        master.PLAN_RD_FROM_HAB = null;
                        break;
                    case "H":
                        master.PLAN_RD_FROM = "Habitation(" + (dbContext.MASTER_HABITATIONS.Where(m => m.MAST_HAB_CODE == model.PLAN_RD_FROM_HAB).Select(m => m.MAST_HAB_NAME).FirstOrDefault()) + ")";
                        master.PLAN_RD_NUM_FROM = null;
                        master.PLAN_RD_BLOCK_FROM_CODE = null;
                        break;
                    case "L":
                        master.PLAN_RD_FROM = dbContext.PLAN_ROAD.Where(m => m.PLAN_CN_ROAD_CODE == model.PLAN_RD_NUM_FROM).Select(m => m.PLAN_CN_ROAD_NUMBER).FirstOrDefault();
                        master.PLAN_RD_NUM_FROM = model.PLAN_RD_NUM_FROM;
                        master.PLAN_RD_BLOCK_FROM_CODE = null;
                        master.PLAN_RD_FROM_HAB = null;
                        break;
                    case "M":
                        master.PLAN_RD_FROM = dbContext.PLAN_ROAD.Where(m => m.PLAN_CN_ROAD_CODE == model.PLAN_RD_NUM_FROM).Select(m => m.PLAN_CN_ROAD_NUMBER).FirstOrDefault();
                        master.PLAN_RD_NUM_FROM = model.PLAN_RD_NUM_FROM;
                        master.PLAN_RD_BLOCK_FROM_CODE = null;
                        master.PLAN_RD_FROM_HAB = null;
                        break;
                    case "T":
                        master.PLAN_RD_FROM = dbContext.PLAN_ROAD.Where(m => m.PLAN_CN_ROAD_CODE == model.PLAN_RD_NUM_FROM).Select(m => m.PLAN_CN_ROAD_NUMBER).FirstOrDefault();
                        master.PLAN_RD_NUM_FROM = model.PLAN_RD_NUM_FROM;
                        master.PLAN_RD_BLOCK_FROM_CODE = null;
                        master.PLAN_RD_FROM_HAB = null;
                        break;
                    default:
                        master.PLAN_RD_FROM = dbContext.MASTER_EXISTING_ROADS.Where(m => m.MAST_ER_ROAD_CODE == model.PLAN_RD_NUM_FROM).Select(m => m.MAST_ER_ROAD_NUMBER).FirstOrDefault();
                        master.PLAN_RD_NUM_FROM = model.PLAN_RD_NUM_FROM;
                        master.PLAN_RD_BLOCK_FROM_CODE = null;
                        master.PLAN_RD_FROM_HAB = null;
                        break;
                }

                switch (model.PLAN_RD_TO_TYPE)
                {
                    case "B":
                        master.PLAN_RD_TO = "Block(" + dbContext.MASTER_BLOCK.Where(m => m.MAST_BLOCK_CODE == model.PLAN_RD_BLOCK_TO_CODE).Select(m => m.MAST_BLOCK_NAME).FirstOrDefault() + ")";
                        master.PLAN_RD_NUM_TO = model.PLAN_RD_NUM_TO;
                        master.PLAN_RD_TO_HAB = null;
                        break;
                    case "H":
                        master.PLAN_RD_TO = "Habitation(" + (dbContext.MASTER_HABITATIONS.Where(m => m.MAST_HAB_CODE == model.PLAN_RD_TO_HAB).Select(m => m.MAST_HAB_NAME).FirstOrDefault()) + ")";
                        master.PLAN_RD_NUM_TO = null;
                        master.PLAN_RD_BLOCK_TO_CODE = null;
                        break;
                    case "L":
                        master.PLAN_RD_TO = dbContext.PLAN_ROAD.Where(m => m.PLAN_CN_ROAD_CODE == model.PLAN_RD_NUM_TO).Select(m => m.PLAN_CN_ROAD_NUMBER).FirstOrDefault();
                        master.PLAN_RD_NUM_TO = model.PLAN_RD_NUM_TO;
                        master.PLAN_RD_BLOCK_TO_CODE = null;
                        master.PLAN_RD_TO_HAB = null;
                        break;
                    case "M":
                        master.PLAN_RD_TO = dbContext.PLAN_ROAD.Where(m => m.PLAN_CN_ROAD_CODE == model.PLAN_RD_NUM_TO).Select(m => m.PLAN_CN_ROAD_NUMBER).FirstOrDefault();
                        master.PLAN_RD_NUM_TO = model.PLAN_RD_NUM_TO;
                        master.PLAN_RD_BLOCK_TO_CODE = null;
                        master.PLAN_RD_TO_HAB = null;
                        break;
                    case "T":
                        master.PLAN_RD_TO = dbContext.PLAN_ROAD.Where(m => m.PLAN_CN_ROAD_CODE == model.PLAN_RD_NUM_TO).Select(m => m.PLAN_CN_ROAD_NUMBER).FirstOrDefault();
                        master.PLAN_RD_NUM_TO = model.PLAN_RD_NUM_TO;
                        master.PLAN_RD_BLOCK_TO_CODE = null;
                        master.PLAN_RD_TO_HAB = null;
                        break;
                    default:
                        master.PLAN_RD_TO = dbContext.MASTER_EXISTING_ROADS.Where(m => m.MAST_ER_ROAD_CODE == model.PLAN_RD_NUM_TO).Select(m => m.MAST_ER_ROAD_NUMBER).FirstOrDefault();
                        master.PLAN_RD_BLOCK_TO_CODE = null;
                        master.PLAN_RD_NUM_TO = model.PLAN_RD_NUM_TO;
                        master.PLAN_RD_TO_HAB = null;
                        break;
                }

                master.PLAN_RD_TOTAL_LEN = model.TotalLengthOfCandidate;
                ///For RCPLWE insert PMGSY SCHEME=3
                master.MAST_PMGSY_SCHEME = 3;// PMGSYSession.Current.PMGSYScheme;

                return master;
            }
            catch (Exception ex)
            {
                //Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
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

        public bool EditRCPLWE(CoreNetworkViewModel model, ref string message)
        {
            PMGSYEntities dbContext = new PMGSYEntities();
            try
            {
                int networkCode = 0;
                encryptedParameters = model.EncryptedRoadCode.Split('/');
                if (!(encryptedParameters.Length == 3))
                {
                    return false;
                }

                decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { encryptedParameters[0], encryptedParameters[1], encryptedParameters[2] });
                networkCode = Convert.ToInt32(decryptedParameters["NetworkCode"].ToString());

                decimal chainageFrom = Convert.ToDecimal(model.PLAN_RD_FROM_CHAINAGE);
                decimal chainageTo = Convert.ToDecimal(model.PLAN_RD_TO_CHAINAGE);

                /*if (PMGSYSession.Current.PMGSYScheme == 2)
                {
                    if (dbContext.PLAN_ROAD_DRRP.Any(m => m.PLAN_CN_ROAD_CODE == networkCode))
                    {
                        if (model.TotalLengthOfCandidate < (dbContext.PLAN_ROAD_DRRP.Where(m => m.PLAN_CN_ROAD_CODE == networkCode).Sum(m => m.PLAN_RD_LENGTH) + Convert.ToDecimal(model.PLAN_RD_LENGTH)))
                        {
                            message = "Total length of RCPLWE should not be less than the sum of candidate road length and other DRRP road length mapped to it.";
                            return false;
                        }
                    }
                }*/

                int recordCount = dbContext.PLAN_ROAD.Where(m => m.MAST_STATE_CODE == model.MAST_STATE_CODE && m.MAST_DISTRICT_CODE == model.MAST_DISTRICT_CODE && m.MAST_BLOCK_CODE == model.MAST_BLOCK_CODE && m.PLAN_CN_ROAD_NUMBER == model.PLAN_CN_ROAD_NUMBER && m.PLAN_RD_FROM_CHAINAGE == chainageFrom && m.PLAN_RD_TO_CHAINAGE == chainageTo && m.PLAN_CN_ROAD_CODE != networkCode && m.MAST_PMGSY_SCHEME == PMGSYSession.Current.PMGSYScheme).Count();
                if (recordCount > 0)
                {
                    //if (PMGSYSession.Current.PMGSYScheme == 1)
                    {
                        message = "RCPLWE information already exist.";
                    }
                    return false;
                }

                PLAN_ROAD master = dbContext.PLAN_ROAD.Where(m => m.PLAN_CN_ROAD_CODE == networkCode).FirstOrDefault();
                if (master != null)
                {
                    master = CloneModelToObjectRCPLWE(model);
                    master.PLAN_CN_ROAD_CODE = networkCode;
                    master.PLAN_CN_ROAD_NUMBER = "O";
                    master.PLAN_RD_ROUTE = "O";

                    master.USERID = PMGSYSession.Current.UserId;
                    master.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                    //dbContext.Entry(master).State = System.Data.Entity.EntityState.Modified;
                    var currentProduct = dbContext.PLAN_ROAD.Where(m => m.PLAN_CN_ROAD_CODE == networkCode).FirstOrDefault(); ;
                    dbContext.Entry(currentProduct).CurrentValues.SetValues(master);
                    dbContext.SaveChanges();
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                //objLog.WriteErrorMessage(ex.Message);
                //Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);

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

        public bool DeleteRCPLWE(int networkCode)
        {
            PMGSYEntities dbContext = new PMGSYEntities();
            try
            {
                using (TransactionScope ts = new TransactionScope())
                {
                    if (dbContext.IMS_UNLOCK_DETAILS.Any(m => m.PLAN_CN_ROAD_CODE == networkCode && m.MAST_PMGSY_SCHEME == PMGSYSession.Current.PMGSYScheme))
                    {
                        IMS_UNLOCK_DETAILS unlockDetails = dbContext.IMS_UNLOCK_DETAILS.Where(m => m.PLAN_CN_ROAD_CODE == networkCode && m.MAST_PMGSY_SCHEME == PMGSYSession.Current.PMGSYScheme).FirstOrDefault();
                        if (unlockDetails != null)
                        {
                            unlockDetails.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                            unlockDetails.USERID = PMGSYSession.Current.UserId;
                            dbContext.Entry(unlockDetails).State = System.Data.Entity.EntityState.Modified;
                            dbContext.SaveChanges();

                            dbContext.IMS_UNLOCK_DETAILS.Remove(unlockDetails);
                            dbContext.SaveChanges();
                        }
                    }

                    PLAN_ROAD master = dbContext.PLAN_ROAD.Where(m => m.PLAN_CN_ROAD_CODE == networkCode && m.MAST_PMGSY_SCHEME == 3).FirstOrDefault();
                    master.USERID = PMGSYSession.Current.UserId;
                    master.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                    dbContext.Entry(master).State = System.Data.Entity.EntityState.Modified;
                    dbContext.SaveChanges();

                    /*
                    List<PLAN_ROAD_HABITATION> lstRoads = dbContext.PLAN_ROAD_HABITATION.Where(m => m.PLAN_CN_ROAD_CODE == networkCode && m.PLAN_ROAD.MAST_PMGSY_SCHEME == 3).ToList();
                    if (lstRoads.Count > 0)
                    {
                        foreach (var item in lstRoads)
                        {
                            dbContext.PLAN_ROAD_HABITATION.Remove(item);
                        }
                    }*/

                    dbContext.PLAN_ROAD.Remove(master);
                    dbContext.SaveChanges();
                    ts.Complete();
                }
                return true;
            }
            catch (DbUpdateException ex)
            {
                //Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                return false;
            }
            catch (Exception ex)
            {
                //Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
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
        /// returns blocks according to the district
        /// </summary>
        /// <param name="districtCode"></param>
        /// <returns></returns>
        public List<MASTER_BLOCK> GetBlocksByDistrictCodeRCPLWE(int districtCode)
        {
            PMGSYEntities dbContext = new PMGSYEntities();
            try
            {
                List<MASTER_BLOCK> lstBlocks = dbContext.MASTER_BLOCK.Where(b => b.MAST_DISTRICT_CODE == districtCode && b.MAST_BLOCK_ACTIVE == "Y" /*&& b.MAST_IAP_BLOCK == "Y"*/).Distinct<MASTER_BLOCK>().ToList<MASTER_BLOCK>();
                return lstBlocks;
            }
            catch (Exception ex)
            {
                //Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
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
        /// returns blocks according to the district RCPLWE
        /// </summary>
        /// <param name="districtCode"></param>
        /// <returns></returns>
        public List<SelectListItem> GetRoadsByCNCodeRCPLWE(int networkCode)
        {
            SelectListItem item;
            PMGSYEntities dbContext = new PMGSYEntities();
            try
            {
                List<SelectListItem> lstRoads = new List<SelectListItem>();
                lstRoads.Insert(0, new SelectListItem { Text = "Select Road", Value = "-1" });
                //List<SelectListItem> lstRoads = (from PR in dbContext.PLAN_ROAD where PR.PLAN_CN_ROAD_CODE == networkCode select PR.MAST_ER_ROAD_CODE).Union(from PR in dbContext.PLAN_ROAD where PR.PLAN_CN_ROAD_CODE == networkCode select PR.MAST_ER_ROAD_CODE);
                var lstPlanRoad = (from PR in dbContext.PLAN_ROAD where PR.PLAN_CN_ROAD_CODE == networkCode select new { PR.MAST_ER_ROAD_CODE, PR.MASTER_EXISTING_ROADS.MAST_ER_ROAD_NAME });
                var lstPlanRoadDRRP = (from PRD in dbContext.PLAN_ROAD_DRRP where PRD.PLAN_CN_ROAD_CODE == networkCode select new { PRD.MAST_ER_ROAD_CODE, PRD.MASTER_EXISTING_ROADS.MAST_ER_ROAD_NAME });

                var lstquery = lstPlanRoad.Union(lstPlanRoadDRRP);
                if (lstquery != null)
                {
                    foreach (var itm in lstquery)
                    {
                        item = new SelectListItem();
                        item.Text = itm.MAST_ER_ROAD_NAME;
                        item.Value = itm.MAST_ER_ROAD_CODE.ToString();
                        lstRoads.Add(item);
                    }
                }

                return lstRoads;
            }
            catch (Exception ex)
            {
                //Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
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
        /// populates the availabe list of habitations to be mapped 
        /// </summary>
        /// <param name="roadCode">core network code</param>
        /// <param name="page">no. of pages</param>
        /// <param name="rows">no of rows</param>
        /// <param name="sidx">sort column</param>
        /// <param name="sord">sort order</param>
        /// <param name="totalRecords">no. of totalRecords</param>
        /// <returns>list of availabel Habitation list</returns>
        public Array GetHabitationListToMapRCPLWE(int cnRoadCode, int erRoadCode, int page, int rows, string sidx, string sord, out long totalRecords)
        {
            PMGSYEntities dbContext = new PMGSYEntities();
            totalRecords = 0;
            try
            {
                //PLAN_ROAD masterRoad = dbContext.PLAN_ROAD.Find(erRoadCode);
                //cnRoadCode = dbContext.PLAN_ROAD.Where(x => x.MAST_ER_ROAD_CODE == erRoadCode).Select(x => x.PLAN_CN_ROAD_CODE).FirstOrDefault();

                var lstHabitations = (from item in dbContext.MASTER_ER_HABITATION_ROAD
                                      join habDetails in dbContext.MASTER_HABITATIONS_DETAILS on item.MASTER_HABITATIONS.MAST_HAB_CODE equals habDetails.MAST_HAB_CODE
                                      where item.MAST_ER_ROAD_CODE == erRoadCode && item.MASTER_HABITATIONS.MAST_HABITATION_ACTIVE == "Y" && habDetails.MAST_YEAR == 2001
                                      select new
                                      {
                                          item.MASTER_HABITATIONS.MAST_HAB_NAME,
                                          item.MASTER_HABITATIONS.MASTER_VILLAGE.MAST_VILLAGE_NAME,
                                          habDetails.MAST_HAB_TOT_POP,
                                          item.MAST_HAB_CODE,
                                      });

                List<int> mapHabitations = (from item in dbContext.PLAN_ROAD_HABITATION
                                            join data in dbContext.PLAN_ROAD on item.PLAN_CN_ROAD_CODE equals data.PLAN_CN_ROAD_CODE
                                            where
                                                //data.MAST_BLOCK_CODE == masterRoad.MAST_BLOCK_CODE &&
                                            data.PLAN_CN_ROAD_CODE == cnRoadCode
                                            select item.MAST_HAB_CODE).Distinct().ToList<int>();

                var listHab = (from item in lstHabitations
                               where !mapHabitations.Contains(item.MAST_HAB_CODE)
                               select item.MAST_HAB_CODE).Distinct();


                //dynamic mappingList = null;

                var route = dbContext.PLAN_ROAD.Where(x => x.PLAN_CN_ROAD_CODE == cnRoadCode).Select(x => x.PLAN_RD_ROUTE).FirstOrDefault();
                if (route == "N")
                {
                    var mappingList = (from item in dbContext.MASTER_HABITATIONS
                                       join habitation in dbContext.MASTER_ER_HABITATION_ROAD on item.MAST_HAB_CODE equals habitation.MAST_HAB_CODE
                                       join planHab in dbContext.MASTER_ER_HABITATION_ROAD on item.MAST_HAB_CODE equals planHab.MAST_HAB_CODE
                                       join habDetails1 in dbContext.MASTER_HABITATIONS_DETAILS on item.MAST_HAB_CODE equals habDetails1.MAST_HAB_CODE
                                       join planRoad in dbContext.PLAN_ROAD on planHab.MAST_ER_ROAD_CODE equals planRoad.MAST_ER_ROAD_CODE
                                       where planRoad.PLAN_CN_ROAD_CODE == cnRoadCode
                                           //&& item.MASTER_VILLAGE.MAST_BLOCK_CODE == blockCode
                                       && habDetails1.MAST_YEAR == ((PMGSYSession.Current.PMGSYScheme == 1 || PMGSYSession.Current.PMGSYScheme == 3) ? 2001 : 2011)
                                       && item.MAST_HABITATION_ACTIVE == "Y"
                                       select new
                                       {
                                           item.MAST_HAB_CODE,
                                           item.MAST_HAB_NAME,
                                           item.MASTER_VILLAGE.MAST_VILLAGE_NAME,
                                           habDetails1.MAST_HAB_TOT_POP
                                       }).Distinct().ToList();

                    totalRecords = mappingList.Count();

                    if (sidx.Trim() != string.Empty)
                    {
                        if (sord.ToString() == "asc")
                        {
                            mappingList = mappingList.OrderBy(x => x.MAST_HAB_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                        }
                        else
                        {
                            mappingList = mappingList.OrderByDescending(x => x.MAST_HAB_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                        }
                    }
                    else
                    {
                        mappingList = mappingList.OrderBy(x => x.MAST_HAB_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                    }

                    var result = mappingList.Select(habDetails => new
                    {
                        habDetails.MAST_HAB_CODE,
                        habDetails.MAST_HAB_NAME,
                        habDetails.MAST_VILLAGE_NAME,
                        habDetails.MAST_HAB_TOT_POP
                    }).ToArray();

                    return result.Select(habDetails => new
                    {
                        id = URLEncrypt.EncryptParameters1(new string[] { "HabCode =" + habDetails.MAST_HAB_CODE.ToString().Trim() }),
                        cell = new[] {
                        
                        //habDetails.MAST_HAB_CODE.ToString(),    
                        habDetails.MAST_HAB_NAME.ToString(),
                        habDetails.MAST_VILLAGE_NAME.ToString(),
                        habDetails.MAST_HAB_TOT_POP.ToString()
                    }
                    }).ToArray();
                }
                else
                {
                    var mappingList = (from item in dbContext.MASTER_HABITATIONS_DETAILS
                                       join habitation in dbContext.MASTER_HABITATIONS on item.MAST_HAB_CODE equals habitation.MAST_HAB_CODE
                                       join village in dbContext.MASTER_VILLAGE on habitation.MAST_VILLAGE_CODE equals village.MAST_VILLAGE_CODE
                                       where listHab.Contains(item.MAST_HAB_CODE) &&
                                       item.MAST_YEAR == ((PMGSYSession.Current.PMGSYScheme == 1 || PMGSYSession.Current.PMGSYScheme == 3) ? 2001 : 2011)
                                       select new
                                       {
                                           item.MAST_HAB_CODE,
                                           habitation.MAST_HAB_NAME,
                                           village.MAST_VILLAGE_NAME,
                                           item.MAST_HAB_TOT_POP
                                       }).Distinct().ToList();

                    totalRecords = mappingList.Count();

                    if (sidx.Trim() != string.Empty)
                    {
                        if (sord.ToString() == "asc")
                        {
                            mappingList = mappingList.OrderBy(x => x.MAST_HAB_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                        }
                        else
                        {
                            mappingList = mappingList.OrderByDescending(x => x.MAST_HAB_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                        }
                    }
                    else
                    {
                        mappingList = mappingList.OrderBy(x => x.MAST_HAB_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                    }

                    var result = mappingList.Select(habDetails => new
                    {
                        habDetails.MAST_HAB_CODE,
                        habDetails.MAST_HAB_NAME,
                        habDetails.MAST_VILLAGE_NAME,
                        habDetails.MAST_HAB_TOT_POP
                    }).ToArray();



                    return result.Select(habDetails => new
                    {
                        id = URLEncrypt.EncryptParameters1(new string[] { "HabCode =" + habDetails.MAST_HAB_CODE.ToString().Trim() }),
                        cell = new[] {
                        
                        //habDetails.MAST_HAB_CODE.ToString(),    
                        habDetails.MAST_HAB_NAME.ToString(),
                        habDetails.MAST_VILLAGE_NAME.ToString(),
                        habDetails.MAST_HAB_TOT_POP.ToString()
                    }
                    }).ToArray();
                }
            }
            catch (Exception ex)
            {
                //Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
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
        /// returns the list of habitations for populating the grid data RCPLWE
        /// </summary>
        /// <param name="roadCode"></param>
        /// <param name="page"></param>
        /// <param name="rows"></param>
        /// <param name="sidx"></param>
        /// <param name="sord"></param>
        /// <param name="totalRecords"></param>
        /// <returns></returns>
        public Array GetHabitationListRCPLWE(int roadCode, string flag, int page, int rows, string sidx, string sord, out long totalRecords)
        {
            PMGSYEntities dbContext = new PMGSYEntities();
            try
            {
                //int roadCode = dbContext.PLAN_ROAD.Where(x => x.MAST_ER_ROAD_CODE == erRoadCode).Select(x => x.PLAN_CN_ROAD_CODE).FirstOrDefault();

                var lstHabitations = (from item in dbContext.PLAN_ROAD_HABITATION
                                      join habitation in dbContext.MASTER_HABITATIONS on item.MAST_HAB_CODE equals habitation.MAST_HAB_CODE
                                      join habCode in dbContext.MASTER_HABITATIONS_DETAILS on item.MAST_HAB_CODE equals habCode.MAST_HAB_CODE
                                      join roadPlan in dbContext.PLAN_ROAD on item.PLAN_CN_ROAD_CODE equals roadPlan.PLAN_CN_ROAD_CODE

                                      //join planDRRP in dbContext.PLAN_ROAD_DRRP on roadPlan.PLAN_CN_ROAD_CODE equals planDRRP.PLAN_CN_ROAD_CODE

                                      where item.PLAN_CN_ROAD_CODE == roadCode &&
                                      habCode.MAST_YEAR == ((PMGSYSession.Current.PMGSYScheme == 1 || PMGSYSession.Current.PMGSYScheme == 3) ? 2001 : 2011)
                                      select new
                                      {
                                          //roadPlan.PLAN_CN_ROAD_NUMBER,
                                          roadPlan.MASTER_EXISTING_ROADS.MAST_ER_ROAD_NAME,
                                          //MAST_ER_ROAD_NAME = dbContext.MASTER_ER_HABITATION_ROAD.Where(x => x.MAST_ER_ROAD_CODE == planDRRP.MAST_ER_ROAD_CODE).Select(x => x.MASTER_EXISTING_ROADS.MAST_ER_ROAD_NAME).FirstOrDefault(),
                                          roadPlan.PLAN_LOCK_STATUS,
                                          habitation.MAST_HAB_NAME,
                                          habCode.MAST_HAB_CODE,
                                          habCode.MAST_BUS_SERVICE,
                                          habCode.MAST_DEGREE_COLLEGE,
                                          habCode.MAST_DISPENSARY,
                                          habCode.MAST_ELECTRICTY,
                                          habCode.MAST_HAB_CONNECTED,
                                          habCode.MAST_HAB_SCST_POP,
                                          habCode.MAST_HAB_TOT_POP,
                                          habCode.MAST_HEALTH_SERVICE,
                                          habCode.MAST_HIGH_SCHOOL,
                                          habCode.MAST_INTERMEDIATE_SCHOOL,
                                          habCode.MAST_MCW_CENTERS,
                                          habCode.MAST_MIDDLE_SCHOOL,
                                          habCode.MAST_PANCHAYAT_HQ,
                                          habCode.MAST_PHCS,
                                          habCode.MAST_PRIMARY_SCHOOL,
                                          habCode.MAST_RAILWAY_STATION,
                                          habCode.MAST_SCHEME,
                                          habCode.MAST_TELEGRAPH_OFFICE,
                                          habCode.MAST_TELEPHONE_CONNECTION,
                                          habCode.MAST_TOURIST_PLACE,
                                          habCode.MAST_VETNARY_HOSPITAL,
                                          habCode.MAST_YEAR,
                                          roadPlan.MAST_BLOCK_CODE,
                                          habitation.MASTER_VILLAGE.MAST_VILLAGE_NAME,
                                          roadPlan.PLAN_CN_ROAD_CODE,
                                          habitation.MASTER_VILLAGE.MASTER_BLOCK.MAST_BLOCK_NAME
                                          //roadPlan.MASTER_BLOCK.MAST_BLOCK_NAME
                                      }).Distinct().GroupBy(m => m.MAST_HAB_CODE).Where(g => g.Count() == 1 || g.Count() > 1)
                                        .Select(g => g.FirstOrDefault());


                totalRecords = lstHabitations.Count();




                if (sidx.Trim() != string.Empty)
                {
                    if (sord.ToString() == "asc")
                    {
                        lstHabitations = lstHabitations.OrderBy(x => x.MAST_HAB_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                    }
                    else
                    {
                        lstHabitations = lstHabitations.OrderByDescending(x => x.MAST_HAB_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                    }
                }
                else
                {
                    lstHabitations = lstHabitations.OrderBy(x => x.MAST_HAB_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                }



                var result = lstHabitations.Select(habDetails => new
                {

                    //habDetails.PLAN_CN_ROAD_NUMBER,
                    habDetails.MAST_ER_ROAD_NAME,
                    habDetails.MAST_HAB_NAME,
                    habDetails.MAST_BUS_SERVICE,
                    habDetails.MAST_DEGREE_COLLEGE,
                    habDetails.MAST_HAB_CODE,
                    habDetails.MAST_DISPENSARY,
                    habDetails.MAST_ELECTRICTY,
                    habDetails.MAST_HAB_CONNECTED,
                    habDetails.MAST_HAB_SCST_POP,
                    habDetails.MAST_HAB_TOT_POP,
                    habDetails.MAST_HEALTH_SERVICE,
                    habDetails.MAST_HIGH_SCHOOL,
                    habDetails.MAST_INTERMEDIATE_SCHOOL,
                    habDetails.MAST_MCW_CENTERS,
                    habDetails.MAST_MIDDLE_SCHOOL,
                    habDetails.MAST_PANCHAYAT_HQ,
                    habDetails.MAST_PHCS,
                    habDetails.MAST_PRIMARY_SCHOOL,
                    habDetails.MAST_RAILWAY_STATION,
                    habDetails.MAST_SCHEME,
                    habDetails.MAST_TELEGRAPH_OFFICE,
                    habDetails.MAST_TELEPHONE_CONNECTION,
                    habDetails.MAST_TOURIST_PLACE,
                    habDetails.MAST_VETNARY_HOSPITAL,
                    habDetails.MAST_YEAR,
                    habDetails.MAST_BLOCK_NAME,
                    habDetails.MAST_VILLAGE_NAME,
                    habDetails.PLAN_LOCK_STATUS,
                    habDetails.MAST_BLOCK_CODE,
                    habDetails.PLAN_CN_ROAD_CODE
                }).ToArray();

                short roleCode = dbContext.UM_User_Master.Where(m => m.UserID == PMGSYSession.Current.UserId).Select(m => m.DefaultRoleID).FirstOrDefault();
                return result.Select(habDetails => new
                {

                    cell = new[]
                {
                    habDetails.MAST_HAB_CODE.ToString(),
                    habDetails.MAST_HAB_NAME == null?string.Empty:habDetails.MAST_HAB_NAME.ToString(),
                    habDetails.MAST_BLOCK_NAME == null?string.Empty:habDetails.MAST_BLOCK_NAME.ToString(),
                    habDetails.MAST_VILLAGE_NAME == null?string.Empty:habDetails.MAST_VILLAGE_NAME.ToString(),
                    //habDetails.PLAN_CN_ROAD_NUMBER == null?string.Empty:habDetails.PLAN_CN_ROAD_NUMBER.ToString(),
                    habDetails.MAST_ER_ROAD_NAME== null?string.Empty:habDetails.MAST_ER_ROAD_NAME.ToString(),
                    habDetails.MAST_HAB_TOT_POP==null?"0":habDetails.MAST_HAB_TOT_POP.ToString(),
                    habDetails.MAST_HAB_SCST_POP==null?"0":habDetails.MAST_HAB_SCST_POP.ToString(),//New SC/ST Population
                    //dbContext.UDF_IMS_UNLOCK_STATUS(PMGSYSession.Current.StateCode,PMGSYSession.Current.DistrictCode,habDetails.MAST_BLOCK_CODE,0,0,habDetails.PLAN_CN_ROAD_CODE,0,0,"CN",PMGSYSession.Current.PMGSYScheme).Select(m=>m.UNLOCK_COUNT).FirstOrDefault().Value > 0 ? "<center><span  class='ui-icon ui-icon-trash' title='Enter Habitation Details' onClick ='deleteHabitationDetails(\"" + URLEncrypt.EncryptParameters1(new string[]{"HabCode =" + habDetails.MAST_HAB_CODE.ToString().Trim(),"Flag ="+flag.ToString().Trim()}) + "\");'></span></center>" : habDetails.PLAN_LOCK_STATUS == "Y" ? "<span class='ui-icon ui-icon-locked ui-align-center'></span>" : "<center><span  class='ui-icon ui-icon-trash' title='Enter Habitation Details' onClick ='deleteHabitationDetails(\"" + URLEncrypt.EncryptParameters1(new string[]{"HabCode =" + habDetails.MAST_HAB_CODE.ToString().Trim(),"Flag ="+flag.ToString().Trim()}) + "\");'></span></center>",
                    dbContext.UDF_IMS_UNLOCK_STATUS(PMGSYSession.Current.StateCode,PMGSYSession.Current.DistrictCode,habDetails.MAST_BLOCK_CODE,0,0,habDetails.PLAN_CN_ROAD_CODE,0,0,"CN",PMGSYSession.Current.PMGSYScheme,roleCode).Select(m=>m.UNLOCK_COUNT).FirstOrDefault().Value > 0 ? "<center><span  class='ui-icon ui-icon-trash' title='Enter Habitation Details' onClick ='deleteHabitationDetails(\"" + URLEncrypt.EncryptParameters1(new string[]{"HabCode =" + habDetails.MAST_HAB_CODE.ToString().Trim(),"Flag ="+flag.ToString().Trim()}) + "\");'></span></center>" : habDetails.PLAN_LOCK_STATUS == "Y" ? "<span class='ui-icon ui-icon-locked ui-align-center'></span>" : "<center><span  class='ui-icon ui-icon-trash' title='Enter Habitation Details' onClick ='deleteHabitationDetails(\"" + URLEncrypt.EncryptParameters1(new string[]{"HabCode =" + habDetails.MAST_HAB_CODE.ToString().Trim(),"Flag ="+flag.ToString().Trim()}) + "\");'></span></center>",
                }

                }).ToArray();
            }
            catch (Exception ex)
            {
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

        /// <summary>
        /// maps the habitations to the current road
        /// </summary>
        /// <param name="encryptedHabCodes">encrypted habitation codes</param>
        /// <param name="roadName">core network code</param>
        /// <returns></returns>
        public bool MapHabitationToRoadRCPLWE(string encryptedHabCodes, string roadName)
        {
            PMGSYEntities dbContext = new PMGSYEntities();
            try
            {
                String[] habCodes = null;
                int roadCode = Convert.ToInt32(roadName);
                //int roadCode = dbContext.PLAN_ROAD.Where(x => x.MAST_ER_ROAD_CODE == erRoadCode).Select(x => x.PLAN_CN_ROAD_CODE).FirstOrDefault();
                int habCode = 0;

                habCodes = encryptedHabCodes.Split(',');
                if (habCodes.Count() == 0)
                {
                    return false;
                }
                foreach (String item in habCodes)
                {
                    encryptedParameters = null;
                    encryptedParameters = item.Split('/');
                    decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { encryptedParameters[0], encryptedParameters[1], encryptedParameters[2] });
                    habCode = Convert.ToInt32(decryptedParameters["HabCode"].ToString());

                    PLAN_ROAD_HABITATION master = new PLAN_ROAD_HABITATION();
                    master.PLAN_CN_ROAD_HAB_ID = dbContext.PLAN_ROAD_HABITATION.Any() ? (from item1 in dbContext.PLAN_ROAD_HABITATION select item1.PLAN_CN_ROAD_HAB_ID).Max() + 1 : 1;
                    master.PLAN_CN_ROAD_CODE = roadCode;
                    master.MAST_HAB_CODE = habCode;
                    master.USERID = PMGSYSession.Current.UserId;
                    master.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                    dbContext.PLAN_ROAD_HABITATION.Add(master);
                    dbContext.SaveChanges();
                }
                return true;
            }
            catch (OptimisticConcurrencyException ex)
            {
                //Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);

                return false;
            }
            catch (UpdateException ex)
            {
                //Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                return false;
            }
            catch (Exception ex)
            {
                //Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
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
        /// finalizes the core network details
        /// </summary>
        /// <param name="roadCode">core network code</param>
        /// <returns></returns>
        public string FinalizeCoreNetworkRCPLWEDAL(int roadCode)
        {
            PMGSYEntities dbContext = new PMGSYEntities();
            try
            {
                PLAN_ROAD planRoad = dbContext.PLAN_ROAD.Find(roadCode);
                //PLAN_ROAD roadMaster = dbContext.PLAN_ROAD.Where(m => m.PLAN_CN_ROAD_CODE == model.CNCode).FirstOrDefault();
                decimal? totalMappedRoadLength = 0;
                if (dbContext.PLAN_ROAD_DRRP.Any(m => m.PLAN_CN_ROAD_CODE == roadCode))
                {
                    totalMappedRoadLength = dbContext.PLAN_ROAD_DRRP.Where(m => m.PLAN_CN_ROAD_CODE == roadCode).Sum(m => m.PLAN_RD_LENGTH);
                    if (totalMappedRoadLength == null)
                    {
                        totalMappedRoadLength = 0;
                    }
                    totalMappedRoadLength = totalMappedRoadLength + planRoad.PLAN_RD_LENGTH;
                }
                else
                {
                    totalMappedRoadLength = planRoad.PLAN_RD_LENGTH;
                }
                string[] totLength = Convert.ToString(planRoad.PLAN_RD_TOTAL_LEN).Split('.');
                string[] mappedLength = Convert.ToString(totalMappedRoadLength).Split('.');

                //if (planRoad.PLAN_RD_TOTAL_LEN == totalMappedRoadLength)
                //if (Convert.ToInt32(totLength[0]) <= Convert.ToInt32(mappedLength[0]))
                if (planRoad.PLAN_RD_LENG == "F" ? Convert.ToInt32(totLength[0]) == Convert.ToInt32(mappedLength[0]) : Convert.ToInt32(totLength[0]) <= Convert.ToInt32(mappedLength[0]))
                {
                    planRoad.PLAN_LOCK_STATUS = "Y";
                    planRoad.USERID = PMGSYSession.Current.UserId;
                    planRoad.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                    dbContext.Entry(planRoad).State = System.Data.Entity.EntityState.Modified;
                    dbContext.SaveChanges();
                    return string.Empty;
                }
                else
                {
                    //return "Total of mapped DRRP road length and the total length of RCPLWE Road must be equal.";
                    return "Total of mapped DRRP road length must be less than or equal to the total length of RCPLWE Road.";
                }
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                return "An Error Occurred While Your Processing Request.";
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

        /// <summary>
        /// returns roads according to the Candidate Road
        /// </summary>
        /// <param name="CNCode"></param>
        /// <returns></returns>
        public List<SelectListItem> GetRoadsByCNCodeCandidate(int networkCode)
        {
            SelectListItem item;
            PMGSYEntities dbContext = new PMGSYEntities();
            try
            {
                List<SelectListItem> lstRoads = new List<SelectListItem>();
                lstRoads.Insert(0, new SelectListItem { Text = "Select Road", Value = "-1" });
                //List<SelectListItem> lstRoads = (from PR in dbContext.PLAN_ROAD where PR.PLAN_CN_ROAD_CODE == networkCode select PR.MAST_ER_ROAD_CODE).Union(from PR in dbContext.PLAN_ROAD where PR.PLAN_CN_ROAD_CODE == networkCode select PR.MAST_ER_ROAD_CODE);
                var lstPlanRoad = (from PR in dbContext.PLAN_ROAD where PR.PLAN_CN_ROAD_CODE == networkCode select new { PR.MAST_ER_ROAD_CODE, PR.MASTER_EXISTING_ROADS.MAST_ER_ROAD_NAME });
                var lstPlanRoadDRRP = (from PRD in dbContext.PLAN_ROAD_DRRP where PRD.PLAN_CN_ROAD_CODE == networkCode select new { PRD.MAST_ER_ROAD_CODE, PRD.MASTER_EXISTING_ROADS.MAST_ER_ROAD_NAME });

                var lstquery = lstPlanRoad.Union(lstPlanRoadDRRP);
                if (lstquery != null)
                {
                    foreach (var itm in lstquery)
                    {
                        item = new SelectListItem();
                        item.Text = itm.MAST_ER_ROAD_NAME;
                        item.Value = itm.MAST_ER_ROAD_CODE.ToString();
                        lstRoads.Add(item);
                    }
                }

                return lstRoads;
            }
            catch (Exception ex)
            {
                //Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
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

        #region PMGSY3
        
        public bool checkIsLockedPMGSY3(int block)
        {
            PMGSYEntities dbContext = new PMGSYEntities();
            bool flag = false;
            bool isBlockFinalised = false;
            try
            {
                //var query = dbContext.IMS_UNLOCK_DETAILS.Where(x => x.MAST_PMGSY_SCHEME == PMGSYSession.Current.PMGSYScheme &&
                //                    ((x.MAST_BLOCK_CODE == block && x.IMS_UNLOCK_LEVEL == "B") || (x.IMS_UNLOCK_LEVEL == "D") || (x.IMS_UNLOCK_LEVEL == "S"))
                //                    && (x.IMS_UNLOCK_START_DATE >= DateTime.Now && x.IMS_UNLOCK_END_DATE >= DateTime.Now)).Any();
                //flag = Convert.ToInt32(query) > 0 ? true : false;

                short roleCode = dbContext.UM_User_Master.Where(m => m.UserID == PMGSYSession.Current.UserId).Select(m => m.DefaultRoleID).FirstOrDefault();
                var count = dbContext.UDF_IMS_UNLOCK_STATUS(PMGSYSession.Current.StateCode, PMGSYSession.Current.DistrictCode, block, 0, 0, 0, 0, 0, "CN", PMGSYSession.Current.PMGSYScheme, roleCode).Select(c => c.UNLOCK_COUNT).FirstOrDefault();

                isBlockFinalised = dbContext.MAST_CN_BLOCK_PMGSY3_FINALIZE.Where(z => z.MAST_BLOCK_CODE == block && z.IS_FINALIZED == "Y").Any();

                if ((Convert.ToInt32(count) > 0) || (Convert.ToInt32(count) == 0 && isBlockFinalised == false))
                {
                    flag = true;
                }
                else
                {
                    flag = false;
                }

                return flag;

            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "CoreNetworkDAL.checkIsLocked()");
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

        public bool CheckLockofTraceMapDAL(int block)
        {
            PMGSYEntities dbContext = new PMGSYEntities();
            bool flag = false;
            //  bool isBlockFinalised = false;
            try
            {

                if (dbContext.MAST_TRACEFILE_PMGSY3.Where(x => x.MAST_BLOCK_CODE == block && x.TRACEFILE_FINALIZE == "Y").Any())
                {
                    flag = true;
                    if (dbContext.MAST_CN_BLOCK_PMGSY3_FINALIZE.Where(x => x.MAST_BLOCK_CODE == block && x.IS_FINALIZED == "Y").Any())
                    {
                        Int32 StateCode = PMGSYSession.Current.StateCode;
                        Int32 DistrictCode = PMGSYSession.Current.DistrictCode;
                        DateTime currentDate = System.DateTime.Now;

                        //State
                        if (dbContext.IMS_UNLOCK_DETAILS.Where(x => x.MAST_PMGSY_SCHEME == 4 && x.IMS_UNLOCK_TABLE == "CN" && x.IMS_UNLOCK_LEVEL == "S" && x.IMS_UNLOCK_STATUS == "Y" && x.MAST_STATE_CODE == StateCode && x.MAST_DISTRICT_CODE == null && x.MAST_BLOCK_CODE == null && x.IMS_UNLOCK_END_DATE >= currentDate).Any())
                        {
                            flag = true; // Show Add Button
                        }
                        //District 
                        else if (dbContext.IMS_UNLOCK_DETAILS.Where(x => x.MAST_PMGSY_SCHEME == 4 && x.IMS_UNLOCK_TABLE == "CN" && x.IMS_UNLOCK_LEVEL == "D" && x.IMS_UNLOCK_STATUS == "Y" && x.MAST_STATE_CODE == StateCode && x.MAST_DISTRICT_CODE == DistrictCode && x.MAST_BLOCK_CODE == null && x.IMS_UNLOCK_END_DATE >= currentDate).Any())
                        {
                            flag = true;  // Show Add Button
                        }
                        // Block
                        else if (dbContext.IMS_UNLOCK_DETAILS.Where(x => x.MAST_PMGSY_SCHEME == 4 && x.IMS_UNLOCK_TABLE == "CN" && x.IMS_UNLOCK_LEVEL == "B" && x.IMS_UNLOCK_STATUS == "Y" && x.MAST_STATE_CODE == StateCode && x.MAST_DISTRICT_CODE == null && x.MAST_BLOCK_CODE == block && x.IMS_UNLOCK_END_DATE >= currentDate).Any())
                        {
                            flag = true;   // Show Add Button
                        }
                        else
                        {
                            flag = false; // hide Add Button

                        }

                    }
                    else
                    {
                        flag = true; // Show Add Button
                    }


                }
                else
                {
                    flag = false; // hide Add Button
                }

                return flag;

            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "CoreNetworkDAL.CheckLockofTraceMapDAL()");
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

        public bool CheckPMGSY3FinalizedTRMRLDAL(int blockCode)
        {
            PMGSYEntities dbContext = new PMGSYEntities();
            bool flag = false;
            int districtCode = 0;
            try
            {
                districtCode = dbContext.MASTER_BLOCK.Where(z => z.MAST_BLOCK_CODE == blockCode).Select(c => c.MAST_DISTRICT_CODE).FirstOrDefault();

                if (
                    dbContext.MAST_DRRP_BLOCK_PMGSY3_FINALIZE.Any(x => x.MAST_BLOCK_CODE == blockCode && x.IS_FINALIZED == "Y")
                    && !dbContext.MAST_FACILITY_DISTRICT_PMGSY3_FINALIZE.Any(x => x.MAST_DISTRICT_CODE == districtCode && x.IS_FINALIZED == "N")
                    && !dbContext.MAST_FACILITY_BLOCK_PMGSY3_FINALIZE.Any(x => x.MASTER_BLOCK.MAST_DISTRICT_CODE == districtCode && x.IS_FINALIZED == "N")
                    && dbContext.MAST_TRACEFILE_PMGSY3.Any(x => x.MAST_BLOCK_CODE == blockCode && x.TRACEFILE_FINALIZE == "Y")
                    )
                {
                    flag = true;
                }
                return flag;
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "CoreNetwork.CheckPMGSY3FinalizedTRMRLDAL()");
                return false;
            }
        }

        public List<SelectListItem> GetDRRPBlocksByDistrictCode(int districtCode)
        {
            PMGSYEntities dbContext = new PMGSYEntities();
            List<SelectListItem> lstBlocks = null;
            SelectListItem item = null;
            try
            {
                var query = dbContext.MAST_DRRP_BLOCK_PMGSY3_FINALIZE.Where(b => b.MASTER_BLOCK.MAST_DISTRICT_CODE == districtCode && b.MASTER_BLOCK.MAST_BLOCK_ACTIVE == "Y" && b.IS_FINALIZED == "Y" && b.MASTER_BLOCK.MAST_TRACEFILE_PMGSY3.Select(c => c.TRACEFILE_FINALIZE).FirstOrDefault() == "Y").Select(x => new { x.MAST_BLOCK_CODE, x.MASTER_BLOCK.MAST_BLOCK_NAME }).Distinct().ToList();

                if (query != null)
                {
                    lstBlocks = new List<SelectListItem>();
                    foreach (var data in query)
                    {
                        item = new SelectListItem();
                        item.Text = data.MAST_BLOCK_NAME.Trim();
                        item.Value = data.MAST_BLOCK_CODE.ToString();
                        lstBlocks.Add(item);
                    }
                }
                return lstBlocks;
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "CoreNetworkDAL.GetDRRPBlocksByDistrictCode()");
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

        public bool CheckPMGSY3FinalizedDAL(int districtCode)
        {
            PMGSYEntities dbContext = new PMGSYEntities();
            bool flag = false;
            try
            {
                if (dbContext.MAST_DRRP_DISTRICT_PMGSY3_FINALIZE.Any(x => x.MAST_DISTRICT_CODE == districtCode && x.IS_FINALIZED == "Y")
                    && !dbContext.MAST_DRRP_BLOCK_PMGSY3_FINALIZE.Any(x => x.MASTER_BLOCK.MAST_DISTRICT_CODE == districtCode && x.IS_FINALIZED == "N")
                    && dbContext.MAST_FACILITY_DISTRICT_PMGSY3_FINALIZE.Any(x => x.MAST_DISTRICT_CODE == districtCode && x.IS_FINALIZED == "Y")
                    && !dbContext.MAST_FACILITY_BLOCK_PMGSY3_FINALIZE.Any(x => x.MASTER_BLOCK.MAST_DISTRICT_CODE == districtCode && x.IS_FINALIZED == "N")
                    && !dbContext.MAST_TRACEFILE_PMGSY3.Any(x => x.MASTER_BLOCK.MAST_DISTRICT_CODE == districtCode && x.TRACEFILE_FINALIZE == "N")
                    )
                //if (!dbContext.MAST_DRRP_BLOCK_PMGSY3_FINALIZE.Any(x => x.MAST_BLOCK_CODE == blockCode && x.IS_FINALIZED == "N")
                //    && !dbContext.MAST_FACILITY_BLOCK_PMGSY3_FINALIZE.Any(x => x.MAST_BLOCK_CODE == blockCode && x.IS_FINALIZED == "N")
                //    && !dbContext.MAST_TRACEFILE_PMGSY3.Any(x => x.MAST_BLOCK_CODE == blockCode && x.TRACEFILE_FINALIZE == "N")
                //    )
                {
                    flag = true;
                }
                return flag;
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "CoreNetwork.CheckPMGSY3FinalizedDAL()");
                return false;
            }
        }

        public Array GetCoreNetWorksListPMGSY3DAL(int stateCode, int districtCode, int blockCode, string roadType, int roadCode, string roadName, int page, int rows, string sidx, string sord, out long totalRecords, int CnCode)
        {
            bool isLocked = false;
            bool isUnlocked = false;
            string filters = string.Empty;
            string nameSearch = string.Empty;
            PMGSYEntities dbContext = new PMGSYEntities();
            try
            {
                // new change with data from stored procedure
                if (roadType == "0")
                {
                    roadType = null;
                }

                if (PMGSYSession.Current.RoleCode == 2)
                {
                    stateCode = PMGSYSession.Current.StateCode;
                }
                else if (PMGSYSession.Current.RoleCode == 22)
                {
                    stateCode = PMGSYSession.Current.StateCode;
                    districtCode = PMGSYSession.Current.DistrictCode;
                }

                short roleCode = dbContext.UM_User_Master.Where(m => m.UserID == PMGSYSession.Current.UserId).Select(m => m.DefaultRoleID).FirstOrDefault();

                var lstPlanRoads = dbContext.GET_CORE_NETWORKS((stateCode <= 0 ? 0 : stateCode), (districtCode <= 0 ? 0 : districtCode), (blockCode <= 0 ? 0 : blockCode), roadType, roadCode, roadName, roleCode).ToList();

                lstPlanRoads = lstPlanRoads.Where(m => m.MAST_PMGSY_SCHEME == 4).ToList();

                if (CnCode > 0)
                {
                    lstPlanRoads = lstPlanRoads.Where(m => m.PLAN_CN_ROAD_CODE == CnCode).ToList();
                }

                totalRecords = lstPlanRoads.Count();

                isLocked = (/*checkIsLocked(blockCode) == true && */dbContext.MAST_CN_BLOCK_PMGSY3_FINALIZE.Where(z => z.MAST_BLOCK_CODE == blockCode && z.IS_FINALIZED == "Y").Any()
                    //&& dbContext.MAST_CN_DISTRICT_PMGSY3_FINALIZE.Where(z => z.MAST_DISTRICT_CODE == districtCode && z.IS_FINALIZED == "Y").Any()
                    );

                // Added By Rohit On 13APR2020
                isUnlocked = checkIsUnlocked(stateCode, districtCode, blockCode);

                #region TRMRLHabs    1
                List<int> lstBlocks = new List<int>();

                //lstBlocks.Add(51);
                //lstBlocks.Add(1341);
                //lstBlocks.Add(2166);
                //lstBlocks.Add(2437);
                //lstBlocks.Add(3566);
                //lstBlocks.Add(5138);
                //lstBlocks.Add(5931);
                //lstBlocks.Add(6347);
                //lstBlocks.Add(7745);
                //lstBlocks.Add(664);
                //lstBlocks.Add(766);
                //lstBlocks.Add(3777);
                //lstBlocks.Add(4221);
                //lstBlocks.Add(4740);
                //lstBlocks.Add(5067);
                //lstBlocks.Add(5351);
                //lstBlocks.Add(5880);
                //lstBlocks.Add(7746);
                //lstBlocks.Add(1762);
                //lstBlocks.Add(3174);
                //lstBlocks.Add(152);
                //lstBlocks.Add(653);
                //lstBlocks.Add(2935);
                //lstBlocks.Add(2996);
                //lstBlocks.Add(5670);
                //lstBlocks.Add(5);
                //lstBlocks.Add(1781);
                //lstBlocks.Add(1783);
                //lstBlocks.Add(1911);
                //lstBlocks.Add(2087);
                //lstBlocks.Add(2381);
                //lstBlocks.Add(3020);
                //lstBlocks.Add(3574);
                //lstBlocks.Add(3615);
                //lstBlocks.Add(6484);
                //lstBlocks.Add(660);
                //lstBlocks.Add(1513);
                //lstBlocks.Add(1585);
                //lstBlocks.Add(1637);
                //lstBlocks.Add(1770);
                //lstBlocks.Add(2079);
                //lstBlocks.Add(2580);
                //lstBlocks.Add(2615);
                //lstBlocks.Add(4895);
                //lstBlocks.Add(5593);
                //lstBlocks.Add(7747);
                //lstBlocks.Add(892);
                //lstBlocks.Add(1464);
                //lstBlocks.Add(1876);
                //lstBlocks.Add(2107);
                //lstBlocks.Add(2245);
                //lstBlocks.Add(2246);
                //lstBlocks.Add(3532);
                //lstBlocks.Add(3960);
                //lstBlocks.Add(5881);
                //lstBlocks.Add(5893);
                //lstBlocks.Add(16);
                //lstBlocks.Add(872);
                //lstBlocks.Add(2388);
                //lstBlocks.Add(2389);
                //lstBlocks.Add(3432);
                //lstBlocks.Add(4102);
                //lstBlocks.Add(4357);
                //lstBlocks.Add(4733);
                //lstBlocks.Add(5217);
                //lstBlocks.Add(5512);
                //lstBlocks.Add(7748);
                //lstBlocks.Add(1609);
                //lstBlocks.Add(2749);
                //lstBlocks.Add(4045);
                //lstBlocks.Add(4724);
                //lstBlocks.Add(5798);
                //lstBlocks.Add(1486);
                //lstBlocks.Add(1666);
                //lstBlocks.Add(2347);
                //lstBlocks.Add(2957);
                //lstBlocks.Add(3441);
                //lstBlocks.Add(3442);
                //lstBlocks.Add(3458);
                //lstBlocks.Add(4435);
                //lstBlocks.Add(5328);
                //lstBlocks.Add(5611);
                //lstBlocks.Add(5780);
                //lstBlocks.Add(7749);
                //lstBlocks.Add(7750);
                //lstBlocks.Add(847);
                //lstBlocks.Add(1059);
                //lstBlocks.Add(2506);
                //lstBlocks.Add(3704);
                //lstBlocks.Add(5413);
                //lstBlocks.Add(379);
                //lstBlocks.Add(3149);
                //lstBlocks.Add(3876);
                //lstBlocks.Add(3877);
                //lstBlocks.Add(4298);
                //lstBlocks.Add(3148);
                //lstBlocks.Add(3370);
                //lstBlocks.Add(3606);
                //lstBlocks.Add(3965);
                //lstBlocks.Add(310);
                //lstBlocks.Add(434);
                //lstBlocks.Add(502);
                //lstBlocks.Add(4254);
                //lstBlocks.Add(5420);
                //lstBlocks.Add(891);
                //lstBlocks.Add(1913);
                //lstBlocks.Add(4042);
                //lstBlocks.Add(4620);
                //lstBlocks.Add(4628);
                //lstBlocks.Add(5002);
                //lstBlocks.Add(5315);
                //lstBlocks.Add(5379);
                //lstBlocks.Add(174);
                //lstBlocks.Add(1134);
                //lstBlocks.Add(3924);
                //lstBlocks.Add(4359);
                //lstBlocks.Add(5197);
                //lstBlocks.Add(113);
                //lstBlocks.Add(183);
                //lstBlocks.Add(832);
                //lstBlocks.Add(1622);
                //lstBlocks.Add(3405);
                //lstBlocks.Add(3588);
                //lstBlocks.Add(3589);
                //lstBlocks.Add(5363);
                //lstBlocks.Add(5561);
                //lstBlocks.Add(5806);
                //lstBlocks.Add(6946);
                //lstBlocks.Add(6947);
                //lstBlocks.Add(6948);
                //lstBlocks.Add(6949);
                //lstBlocks.Add(6950);
                //lstBlocks.Add(6951);
                //lstBlocks.Add(6952);
                //lstBlocks.Add(6953);
                //lstBlocks.Add(6954);
                //lstBlocks.Add(6955);
                //   lstBlocks.Add(1532);locked (commented) on 9 march 2020

                #endregion

                if (sidx.Trim() != string.Empty)
                {
                    if (sord.ToString() == "asc")
                    {
                        switch (sidx)
                        {
                            case "PLAN_CN_ROAD_CODE":
                                lstPlanRoads = lstPlanRoads.OrderBy(x => x.PLAN_CN_ROAD_CODE).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                            case "PLAN_CN_ROAD_NUMBER":
                                lstPlanRoads = lstPlanRoads.OrderBy(x => x.PLAN_CN_ROAD_NUMBER).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                            //case "MAST_ER_ROAD_CODE":
                            //    lstPlanRoads = lstPlanRoads.OrderBy(x => x.MAST_ER_ROAD_CODE).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                            //    break;
                            case "PLAN_RD_NAME":
                                lstPlanRoads = lstPlanRoads.OrderBy(x => x.PLAN_RD_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                            case "PLAN_RD_FROM":
                                lstPlanRoads = lstPlanRoads.OrderBy(x => x.PLAN_RD_FROM).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                            case "PLAN_RD_TO":
                                lstPlanRoads = lstPlanRoads.OrderBy(x => x.PLAN_RD_TO).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                            case "PLAN_RD_FROM_CHAINAGE":
                                lstPlanRoads = lstPlanRoads.OrderBy(x => x.PLAN_RD_FROM_CHAINAGE).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                            case "PLAN_RD_TO_CHAINAGE":
                                lstPlanRoads = lstPlanRoads.OrderBy(x => x.PLAN_RD_TO_CHAINAGE).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                            case "PLAN_RD_LENGTH":
                                lstPlanRoads = lstPlanRoads.OrderBy(x => x.PLAN_RD_LENGTH).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                            case "PLAN_RD_TOTAL_LENGTH":
                                lstPlanRoads = lstPlanRoads.OrderBy(x => x.PLAN_RD_TOTAL_LEN).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                            case "PLAN_RD_ROUTE":
                                lstPlanRoads = lstPlanRoads.OrderBy(x => x.PLAN_RD_ROUTE).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                            default:
                                lstPlanRoads = lstPlanRoads.OrderBy(x => x.PLAN_CN_ROAD_CODE).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                        }
                    }
                    else
                    {
                        switch (sidx)
                        {
                            case "PLAN_CN_ROAD_CODE":
                                lstPlanRoads = lstPlanRoads.OrderByDescending(x => x.PLAN_CN_ROAD_CODE).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                            case "PLAN_CN_ROAD_NUMBER":
                                lstPlanRoads = lstPlanRoads.OrderByDescending(x => x.PLAN_CN_ROAD_NUMBER).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                            //case "MAST_ER_ROAD_CODE":
                            //    lstPlanRoads = lstPlanRoads.OrderByDescending(x => x.MAST_ER_ROAD_CODE).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                            //    break;
                            case "PLAN_RD_NAME":
                                lstPlanRoads = lstPlanRoads.OrderByDescending(x => x.PLAN_RD_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                            case "PLAN_RD_FROM":
                                lstPlanRoads = lstPlanRoads.OrderByDescending(x => x.PLAN_RD_FROM).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                            case "PLAN_RD_TO":
                                lstPlanRoads = lstPlanRoads.OrderByDescending(x => x.PLAN_RD_TO).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                            case "PLAN_RD_FROM_CHAINAGE":
                                lstPlanRoads = lstPlanRoads.OrderByDescending(x => x.PLAN_RD_FROM_CHAINAGE).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                            case "PLAN_RD_TO_CHAINAGE":
                                lstPlanRoads = lstPlanRoads.OrderByDescending(x => x.PLAN_RD_TO_CHAINAGE).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                            case "PLAN_RD_LENGTH":
                                lstPlanRoads = lstPlanRoads.OrderByDescending(x => x.PLAN_RD_LENGTH).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                            case "PLAN_RD_TOTAL_LENGTH":
                                lstPlanRoads = lstPlanRoads.OrderByDescending(x => x.PLAN_RD_TOTAL_LEN).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                            case "PLAN_RD_ROUTE":
                                lstPlanRoads = lstPlanRoads.OrderByDescending(x => x.PLAN_RD_ROUTE).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                            default:
                                lstPlanRoads = lstPlanRoads.OrderByDescending(x => x.PLAN_CN_ROAD_CODE).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                        }
                    }
                }
                else
                {
                    lstPlanRoads = lstPlanRoads.OrderByDescending(x => x.PLAN_CN_ROAD_CODE).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                }

                var result = lstPlanRoads.Select(roadDetails => new
                {
                    //roadDetails.MAST_ER_ROAD_NUMBER,
                    roadDetails.BLOCK_NAME,
                    roadDetails.DISTRICT_NAME,
                    //roadDetails.ER,
                    roadDetails.STATE_NAME,
                    roadDetails.PLAN_CN_ROAD_CODE,
                    roadDetails.PLAN_CN_ROAD_NUMBER,
                    //roadDetails.PLAN_LOCK_STATUS,
                    roadDetails.PLAN_RD_BLOCK_FROM_CODE,
                    roadDetails.PLAN_RD_BLOCK_TO_CODE,
                    roadDetails.PLAN_RD_FROM_CHAINAGE,
                    roadDetails.PLAN_RD_FROM_HAB,
                    roadDetails.PLAN_RD_FROM_TYPE,
                    roadDetails.PLAN_RD_LENG,
                    roadDetails.PLAN_RD_LENGTH,
                    roadDetails.PLAN_RD_NAME,
                    roadDetails.PLAN_RD_NUM_FROM,
                    roadDetails.PLAN_RD_NUM_TO,
                    roadDetails.PLAN_RD_ROUTE,
                    roadDetails.PLAN_RD_TO_CHAINAGE,
                    roadDetails.PLAN_RD_TO_HAB,
                    roadDetails.PLAN_RD_TO_TYPE,
                    roadDetails.PLAN_RD_FROM,
                    roadDetails.PLAN_RD_TO,
                    roadDetails.UNLOCK_BY_MORD,
                    roadDetails.MAST_ER_ROAD_NUMBER,
                    roadDetails.PLAN_RD_TOTAL_LEN
                }).ToArray();

                return result.Select(roadDetails => new
                {
                    id = URLEncrypt.EncryptParameters1(new string[] { "NetworkCode=" + roadDetails.PLAN_CN_ROAD_CODE.ToString() }),
                    cell = new[]
                {
                    roadDetails.PLAN_CN_ROAD_CODE.ToString().Trim(),
                    roadDetails.PLAN_RD_ROUTE == null?string.Empty:roadDetails.PLAN_RD_ROUTE.ToString(),
                    roadDetails.PLAN_CN_ROAD_NUMBER == null?string.Empty:roadDetails.PLAN_CN_ROAD_NUMBER.ToString(),
                    roadDetails.PLAN_RD_NAME == null?string.Empty:roadDetails.PLAN_RD_NAME.ToString(),
                    roadDetails.MAST_ER_ROAD_NUMBER.ToString(),
                    roadDetails.PLAN_RD_FROM_TYPE=="B"?"Block("+(dbContext.MASTER_BLOCK.Where(item=>item.MAST_BLOCK_CODE == roadDetails.PLAN_RD_BLOCK_FROM_CODE).Select(m=>m.MAST_BLOCK_NAME).FirstOrDefault())+")":(roadDetails.PLAN_RD_FROM_TYPE=="H"?"Habitation("+(dbContext.MASTER_HABITATIONS.Where(item=>item.MAST_HAB_CODE == roadDetails.PLAN_RD_FROM_HAB).Select(m=>m.MAST_HAB_NAME).FirstOrDefault())+")":(roadDetails.PLAN_RD_FROM==null?string.Empty:roadDetails.PLAN_RD_FROM.ToString())),
                    roadDetails.PLAN_RD_TO_TYPE=="B"?"Block("+(dbContext.MASTER_BLOCK.Where(item=>item.MAST_BLOCK_CODE == roadDetails.PLAN_RD_BLOCK_TO_CODE).Select(m=>m.MAST_BLOCK_NAME).FirstOrDefault())+")":(roadDetails.PLAN_RD_TO_TYPE=="H"?"Habitation("+(dbContext.MASTER_HABITATIONS.Where(item=>item.MAST_HAB_CODE == roadDetails.PLAN_RD_TO_HAB).Select(m=>m.MAST_HAB_NAME).FirstOrDefault())+")":(roadDetails.PLAN_RD_TO==null?string.Empty:roadDetails.PLAN_RD_TO.ToString())),

                    roadDetails.PLAN_RD_FROM_CHAINAGE == null?string.Empty:roadDetails.PLAN_RD_FROM_CHAINAGE.ToString(),
                    roadDetails.PLAN_RD_TO_CHAINAGE == null?string.Empty:roadDetails.PLAN_RD_TO_CHAINAGE.ToString(),
                    roadDetails.PLAN_RD_LENGTH == null?string.Empty:roadDetails.PLAN_RD_LENGTH.ToString(),
                     (roadDetails.PLAN_RD_TOTAL_LEN == null ? Convert.ToString(roadDetails.PLAN_RD_TO_CHAINAGE - roadDetails.PLAN_RD_FROM_CHAINAGE) :  roadDetails.PLAN_RD_TOTAL_LEN.ToString()),

                     //Map Habitation
                     //(PMGSYSession.Current.StateCode == 29 /*|| PMGSYSession.Current.StateCode == 17 || PMGSYSession.Current.StateCode == 13*/) 
                     //   ? "<a href='#' title='Click here to map habitation Details' class='ui-icon ui-icon-plusthick ui-align-center' onClick =editHabitationDetails('"+URLEncrypt.EncryptParameters1(new string[]{"NetworkCode =" +                                                    roadDetails.PLAN_CN_ROAD_CODE.ToString().Trim(),"UnlockFlag =" + roadDetails.UNLOCK_BY_MORD.ToString().Trim()})+"'); return false;'>Delete</a>"
                     //   : 



                     //TRMRLHabs    2  This line is commented on 16 April 2020
                       // (((roadDetails.UNLOCK_BY_MORD == "M" && isLocked == false) || lstBlocks.Contains(blockCode))?"<a href='#' title='Click here to map habitation Details' class='ui-icon ui-icon-unlocked ui-align-center' onClick =editHabitationDetails('"+URLEncrypt.EncryptParameters1(new string[]{"NetworkCode =" + roadDetails.PLAN_CN_ROAD_CODE.ToString().Trim(),"UnlockFlag =" + roadDetails.UNLOCK_BY_MORD.ToString().Trim()})+"'); return false;'>Delete</a>":roadDetails.UNLOCK_BY_MORD=="N"?"<a href='#' title='Click here to map habitation Details' class='ui-icon ui-icon-plusthick ui-align-center' onClick =editHabitationDetails('"+URLEncrypt.EncryptParameters1(new string[]{"NetworkCode =" + roadDetails.PLAN_CN_ROAD_CODE.ToString().Trim(),"UnlockFlag =" + roadDetails.UNLOCK_BY_MORD.ToString().Trim()})+"'); return false;'>Delete</a>":"<span class='ui-icon ui-icon-locked ui-align-center'></span>"),
                    


                          // Added By Rohit On 13APR2020
                          ((isUnlocked==true||lstBlocks.Contains(blockCode))?"<a href='#' title='Click here to map habitation Details' class='ui-icon ui-icon-unlocked ui-align-center' onClick =editHabitationDetails('"+URLEncrypt.EncryptParameters1(new string[]{"NetworkCode =" + roadDetails.PLAN_CN_ROAD_CODE.ToString().Trim(),"UnlockFlag =" + roadDetails.UNLOCK_BY_MORD.ToString().Trim()})+"'); return false;'>Delete</a>":roadDetails.UNLOCK_BY_MORD=="N"?"<a href='#' title='Click here to map habitation Details' class='ui-icon ui-icon-plusthick ui-align-center' onClick =editHabitationDetails('"+URLEncrypt.EncryptParameters1(new string[]{"NetworkCode =" + roadDetails.PLAN_CN_ROAD_CODE.ToString().Trim(),"UnlockFlag =" + roadDetails.UNLOCK_BY_MORD.ToString().Trim()})+"'); return false;'>Delete</a>":"<span class='ui-icon ui-icon-locked ui-align-center'></span>"),
                    





                    //Upload
                    ///Changes by SAMMED A. PATIL on 27JULY2017 to lock Habitation mapping if CoreNetwork is locked uncommented above line
                    //"<a href='#' title='Click here to map habitation Details' class='ui-icon ui-icon-plusthick ui-align-center' onClick =editHabitationDetails('"+URLEncrypt.EncryptParameters1(new string[]{"NetworkCode =" + roadDetails.PLAN_CN_ROAD_CODE.ToString().Trim(),"UnlockFlag =" + roadDetails.UNLOCK_BY_MORD.ToString().Trim()})+"'); return false;'>Delete</a>",
                    (
                    (roadDetails.UNLOCK_BY_MORD == "M" && isLocked == false)?"<a href='#' title='Click here to upload file details' class='ui-icon ui-icon-unlocked ui-align-center' onClick =UploadFile('"+URLEncrypt.EncryptParameters1(new string[]{"NetworkCode =" + roadDetails.PLAN_CN_ROAD_CODE.ToString().Trim()})+"'); return false;'>Delete</a>":
                    roadDetails.UNLOCK_BY_MORD=="N"?"<a href='#' title='Click here to upload file details' class='ui-icon ui-icon-plusthick ui-align-center' onClick =UploadFile('"+URLEncrypt.EncryptParameters1(new string[]{"NetworkCode =" + roadDetails.PLAN_CN_ROAD_CODE.ToString().Trim()})+"'); return false;'>Delete</a>":"<span class='ui-icon ui-icon-locked ui-align-center'></span></center>"),
                    
                    //Map DRRP
                    (PMGSYSession.Current.PMGSYScheme == 4 && dbContext.MANE_CN_PCI_INDEX_PMGSY3.Where(z=>z.PLAN_CN_ROAD_CODE == roadDetails.PLAN_CN_ROAD_CODE).Any())
                    ?   "PCI already entered" 
                    : (PMGSYSession.Current.PMGSYScheme == 1?"":
                    (roadDetails.UNLOCK_BY_MORD == "M" && isLocked == false)?("<a href='#' title='Click here to map other TR/MRL road' class='ui-icon ui-icon-unlocked ui-align-center' onClick =MapOtherCandidateRoad('"+URLEncrypt.EncryptParameters1(new string[]{"NetworkCode =" + roadDetails.PLAN_CN_ROAD_CODE.ToString().Trim()})+"'); return false;'>Map Other DRRP Road</a>"):
                    roadDetails.UNLOCK_BY_MORD == "N"? "<a href='#' title='Click here to map other TR/MRL road' class='ui-icon ui-icon-plusthick ui-align-center' onClick =MapOtherCandidateRoad('"+URLEncrypt.EncryptParameters1(new string[]{"NetworkCode =" + roadDetails.PLAN_CN_ROAD_CODE.ToString().Trim()})+"'); return false;'>Map Other DRRP Road</a>" : "<span class='ui-icon ui-icon-locked ui-align-center'></span>"),

                    //MapInterDistrict
                    ((isUnlocked==true||lstBlocks.Contains(blockCode))?"<a href='#' title='Click here to map inter district habitation Details' class='ui-icon ui-icon-plusthick ui-align-center' onClick =mapInterDistrictHabitationDetails('"+URLEncrypt.EncryptParameters1(new string[]{"NetworkCode =" + roadDetails.PLAN_CN_ROAD_CODE.ToString().Trim(),"UnlockFlag =" + roadDetails.UNLOCK_BY_MORD.ToString().Trim()})+"'); return false;'>Delete</a>":roadDetails.UNLOCK_BY_MORD=="N"?"<a href='#' title='Click here to map habitation Details' class='ui-icon ui-icon-plusthick ui-align-center' onClick =editHabitationDetails('"+URLEncrypt.EncryptParameters1(new string[]{"NetworkCode =" + roadDetails.PLAN_CN_ROAD_CODE.ToString().Trim(),"UnlockFlag =" + roadDetails.UNLOCK_BY_MORD.ToString().Trim()})+"'); return false;'>Delete</a>":"<span class='ui-icon ui-icon-locked ui-align-center'></span>"),
        


                    //View
                    "<a href='#' title='Click here to view core network details' class='ui-icon ui-icon-zoomin ui-align-center' onClick =detailsCoreNetwork('"+URLEncrypt.EncryptParameters1(new string[]{"NetworkCode =" + roadDetails.PLAN_CN_ROAD_CODE.ToString().Trim()})+"'); return false;'>Delete</a>",
                    
                    ///Changes by SAMMED A. PATIL on 03 OCTOBER 2017 to edit DRRP for MORD
                    (PMGSYSession.Current.PMGSYScheme == 4 && dbContext.MANE_CN_PCI_INDEX_PMGSY3.Where(z=>z.PLAN_CN_ROAD_CODE == roadDetails.PLAN_CN_ROAD_CODE).Any())
                    ?   "PCI already entered" 
                    : (PMGSYSession.Current.RoleCode == 25 
                        ? "<a href='#' title='Click here to view core network details' class='ui-icon ui-icon-pencil ui-align-center' onClick =editNetworkData('"+URLEncrypt.EncryptParameters1(new string[]{"NetworkCode =" + roadDetails.PLAN_CN_ROAD_CODE.ToString().Trim()})+"'); return false;'>Edit</a>"
                        : (
                        (roadDetails.UNLOCK_BY_MORD == "M" && isLocked == false)?"<a href='#' title='Click here to edit core network details' class='ui-icon ui-icon-unlocked ui-align-center' onClick =editNetworkData('"+URLEncrypt.EncryptParameters1(new string[]{"NetworkCode =" + roadDetails.PLAN_CN_ROAD_CODE.ToString().Trim(),"UnlockFlag =" + roadDetails.UNLOCK_BY_MORD.ToString().Trim()})+"'); return false;'>Delete</a>":
                        roadDetails.UNLOCK_BY_MORD=="N"?"<a href='#' title='Click here to view core network details' class='ui-icon ui-icon-pencil ui-align-center' onClick =editNetworkData('"+URLEncrypt.EncryptParameters1(new string[]{"NetworkCode =" + roadDetails.PLAN_CN_ROAD_CODE.ToString().Trim()})+"'); return false;'>Edit</a>":"<span class='ui-icon ui-icon-locked ui-align-center'></span></center>")),
                    //Delete
                    (roadDetails.UNLOCK_BY_MORD == "M" && isLocked == false)?"<a href='#' title='Click here to delete core network details' class='ui-icon ui-icon-trash ui-align-center' onClick =deleteNetworkData('"+URLEncrypt.EncryptParameters1(new string[]{"NetworkCode =" + roadDetails.PLAN_CN_ROAD_CODE.ToString().Trim()})+"'); return false;'>Delete</a>":
                    roadDetails.UNLOCK_BY_MORD=="N"?"<a href='#' title='Click here to delete core network details' class='ui-icon ui-icon-trash ui-align-center' onClick =deleteNetworkData('"+URLEncrypt.EncryptParameters1(new string[]{"NetworkCode =" + roadDetails.PLAN_CN_ROAD_CODE.ToString().Trim()})+"'); return false;'>Delete</a>":"<span class='ui-icon ui-icon-locked ui-align-center'></span></center>",
                }
                }).ToArray();
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "GetCoreNetWorksListPMGSY3DAL()");
                totalRecords = 0;
                dbContext.Dispose();
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

        public List<SelectListItem> GetRoadNamesByRoadCodePMGSY3DAL(int roadCode, int blockCode)
        {
            PMGSYEntities dbContext = new PMGSYEntities();
            try
            {
                //var  roadList = dbContext.MASTER_EXISTING_ROADS.Where(d => d.MAST_ROAD_CAT_CODE == roadCode && d.MAST_BLOCK_CODE == blockCode && d.MAST_CORE_NETWORK == "Y" && d.MAST_PMGSY_SCHEME == PMGSYSession.Current.PMGSYScheme).OrderBy(d => d.MAST_ER_ROAD_NAME).ToList<Models.MASTER_EXISTING_ROADS>();

                var roadList = (from item in dbContext.MASTER_EXISTING_ROADS
                                where item.MAST_ROAD_CAT_CODE == roadCode &&
                                item.MAST_BLOCK_CODE == blockCode &&
                                    //item.MAST_CORE_NETWORK == "Y" &&
                                item.MAST_PMGSY_SCHEME == 2
                                && item.MAST_LOCK_STATUS == "Y"
                                select new
                                {
                                    MAST_ROAD_CODE = item.MAST_ER_ROAD_CODE,
                                    MAST_ROAD_NAME = (item.MAST_ER_ROAD_NUMBER + " - " + item.MAST_ER_ROAD_NAME)
                                }).OrderBy(m => m.MAST_ROAD_NAME).ToList();
                return new SelectList(roadList.ToList(), "MAST_ROAD_CODE", "MAST_ROAD_NAME").ToList();
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "GetCoreNetWorksListPMGSY3DAL()");
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

        public PLAN_ROAD CloneModelToObjectPMGSY3(CoreNetworkViewModelPMGSY3 model)
        {
            PMGSYEntities dbContext = new PMGSYEntities();
            try
            {
                PLAN_ROAD master = new PLAN_ROAD();
                master.MAST_BLOCK_CODE = model.MAST_BLOCK_CODE;
                master.MAST_DISTRICT_CODE = model.MAST_DISTRICT_CODE;
                master.MAST_ER_ROAD_CODE = model.MAST_ER_ROAD_CODE;
                master.MAST_STATE_CODE = model.MAST_STATE_CODE;
                master.PLAN_CN_ROAD_NUMBER = model.PLAN_CN_ROAD_NUMBER;
                master.PLAN_LOCK_STATUS = model.PLAN_LOCK_STATUS == null ? "N" : model.PLAN_LOCK_STATUS;
                master.PLAN_RD_BLOCK_FROM_CODE = model.PLAN_RD_BLOCK_FROM_CODE;
                master.PLAN_RD_BLOCK_TO_CODE = model.PLAN_RD_BLOCK_TO_CODE;
                master.PLAN_RD_FROM_CHAINAGE = Convert.ToDecimal(model.PLAN_RD_FROM_CHAINAGE);
                master.PLAN_RD_FROM_HAB = model.PLAN_RD_FROM_HAB;
                master.PLAN_RD_FROM_TYPE = model.PLAN_RD_FROM_TYPE;
                master.PLAN_RD_LENG = model.PLAN_RD_LENG;
                master.PLAN_RD_LENGTH = Convert.ToDecimal(model.PLAN_RD_LENGTH);
                master.PLAN_RD_NAME = (model.PLAN_RD_NAME == null ? dbContext.MASTER_EXISTING_ROADS.Where(m => m.MAST_ER_ROAD_CODE == model.MAST_ER_ROAD_CODE).Select(m => m.MAST_ER_ROAD_NAME).FirstOrDefault() : model.PLAN_RD_NAME); //model.PLAN_RD_NAME;
                master.PLAN_RD_ROUTE = model.PLAN_RD_ROUTE;
                master.PLAN_RD_TO_CHAINAGE = Convert.ToDecimal(model.PLAN_RD_TO_CHAINAGE);
                master.PLAN_RD_TO_HAB = model.PLAN_RD_TO_HAB;
                master.PLAN_RD_TO_TYPE = model.PLAN_RD_TO_TYPE;
                switch (model.PLAN_RD_FROM_TYPE)
                {
                    case "B":
                        master.PLAN_RD_FROM = "Block(" + dbContext.MASTER_BLOCK.Where(m => m.MAST_BLOCK_CODE == model.PLAN_RD_BLOCK_FROM_CODE).Select(m => m.MAST_BLOCK_NAME).FirstOrDefault() + ")";
                        master.PLAN_RD_NUM_FROM = model.PLAN_RD_NUM_FROM;
                        master.PLAN_RD_FROM_HAB = null;
                        break;
                    case "H":
                        master.PLAN_RD_FROM = "Habitation(" + (dbContext.MASTER_HABITATIONS.Where(m => m.MAST_HAB_CODE == model.PLAN_RD_FROM_HAB).Select(m => m.MAST_HAB_NAME).FirstOrDefault()) + ")";
                        master.PLAN_RD_NUM_FROM = null;
                        master.PLAN_RD_BLOCK_FROM_CODE = null;
                        break;
                    case "L":
                        master.PLAN_RD_FROM = dbContext.PLAN_ROAD.Where(m => m.PLAN_CN_ROAD_CODE == model.PLAN_RD_NUM_FROM).Select(m => m.PLAN_CN_ROAD_NUMBER).FirstOrDefault();
                        master.PLAN_RD_NUM_FROM = model.PLAN_RD_NUM_FROM;
                        master.PLAN_RD_BLOCK_FROM_CODE = null;
                        master.PLAN_RD_FROM_HAB = null;
                        break;
                    case "M":
                        master.PLAN_RD_FROM = dbContext.PLAN_ROAD.Where(m => m.PLAN_CN_ROAD_CODE == model.PLAN_RD_NUM_FROM).Select(m => m.PLAN_CN_ROAD_NUMBER).FirstOrDefault();
                        master.PLAN_RD_NUM_FROM = model.PLAN_RD_NUM_FROM;
                        master.PLAN_RD_BLOCK_FROM_CODE = null;
                        master.PLAN_RD_FROM_HAB = null;
                        break;
                    case "T":
                        master.PLAN_RD_FROM = dbContext.PLAN_ROAD.Where(m => m.PLAN_CN_ROAD_CODE == model.PLAN_RD_NUM_FROM).Select(m => m.PLAN_CN_ROAD_NUMBER).FirstOrDefault();
                        master.PLAN_RD_NUM_FROM = model.PLAN_RD_NUM_FROM;
                        master.PLAN_RD_BLOCK_FROM_CODE = null;
                        master.PLAN_RD_FROM_HAB = null;
                        break;
                    default:
                        master.PLAN_RD_FROM = dbContext.MASTER_EXISTING_ROADS.Where(m => m.MAST_ER_ROAD_CODE == model.PLAN_RD_NUM_FROM).Select(m => m.MAST_ER_ROAD_NUMBER).FirstOrDefault();
                        master.PLAN_RD_NUM_FROM = model.PLAN_RD_NUM_FROM;
                        master.PLAN_RD_BLOCK_FROM_CODE = null;
                        master.PLAN_RD_FROM_HAB = null;
                        break;
                }

                switch (model.PLAN_RD_TO_TYPE)
                {
                    case "B":
                        master.PLAN_RD_TO = "Block(" + dbContext.MASTER_BLOCK.Where(m => m.MAST_BLOCK_CODE == model.PLAN_RD_BLOCK_TO_CODE).Select(m => m.MAST_BLOCK_NAME).FirstOrDefault() + ")";
                        master.PLAN_RD_NUM_TO = model.PLAN_RD_NUM_TO;
                        master.PLAN_RD_TO_HAB = null;
                        break;
                    case "H":
                        master.PLAN_RD_TO = "Habitation(" + (dbContext.MASTER_HABITATIONS.Where(m => m.MAST_HAB_CODE == model.PLAN_RD_TO_HAB).Select(m => m.MAST_HAB_NAME).FirstOrDefault()) + ")";
                        master.PLAN_RD_NUM_TO = null;
                        master.PLAN_RD_BLOCK_TO_CODE = null;
                        break;
                    case "L":
                        master.PLAN_RD_TO = dbContext.PLAN_ROAD.Where(m => m.PLAN_CN_ROAD_CODE == model.PLAN_RD_NUM_TO).Select(m => m.PLAN_CN_ROAD_NUMBER).FirstOrDefault();
                        master.PLAN_RD_NUM_TO = model.PLAN_RD_NUM_TO;
                        master.PLAN_RD_BLOCK_TO_CODE = null;
                        master.PLAN_RD_TO_HAB = null;
                        break;
                    case "M":
                        master.PLAN_RD_TO = dbContext.PLAN_ROAD.Where(m => m.PLAN_CN_ROAD_CODE == model.PLAN_RD_NUM_TO).Select(m => m.PLAN_CN_ROAD_NUMBER).FirstOrDefault();
                        master.PLAN_RD_NUM_TO = model.PLAN_RD_NUM_TO;
                        master.PLAN_RD_BLOCK_TO_CODE = null;
                        master.PLAN_RD_TO_HAB = null;
                        break;
                    case "T":
                        master.PLAN_RD_TO = dbContext.PLAN_ROAD.Where(m => m.PLAN_CN_ROAD_CODE == model.PLAN_RD_NUM_TO).Select(m => m.PLAN_CN_ROAD_NUMBER).FirstOrDefault();
                        master.PLAN_RD_NUM_TO = model.PLAN_RD_NUM_TO;
                        master.PLAN_RD_BLOCK_TO_CODE = null;
                        master.PLAN_RD_TO_HAB = null;
                        break;
                    default:
                        master.PLAN_RD_TO = dbContext.MASTER_EXISTING_ROADS.Where(m => m.MAST_ER_ROAD_CODE == model.PLAN_RD_NUM_TO).Select(m => m.MAST_ER_ROAD_NUMBER).FirstOrDefault();
                        master.PLAN_RD_BLOCK_TO_CODE = null;
                        master.PLAN_RD_NUM_TO = model.PLAN_RD_NUM_TO;
                        master.PLAN_RD_TO_HAB = null;
                        break;
                }

                master.PLAN_RD_TOTAL_LEN = model.TotalLengthOfCandidate;
                master.MAST_PMGSY_SCHEME = PMGSYSession.Current.PMGSYScheme;
                return master;
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "CloneModelToObjectPMGSY3");
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

        public bool AddCoreNetworksPMGSY3DAL(CoreNetworkViewModelPMGSY3 model, ref string message)
        {
            PMGSYEntities dbContext = new PMGSYEntities();
            try
            {
                decimal chainageFrom = Convert.ToDecimal(model.PLAN_RD_FROM_CHAINAGE);
                decimal chainageTo = Convert.ToDecimal(model.PLAN_RD_TO_CHAINAGE);
                //using (TransactionScope ts = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions
                //{
                //    IsolationLevel = System.Transactions.IsolationLevel.ReadUncommitted
                //}))


                //using (TransactionScope ts = new TransactionScope())
                //{
                    int recordCount = dbContext.PLAN_ROAD.Where(m => m.MAST_STATE_CODE == model.MAST_STATE_CODE && m.MAST_DISTRICT_CODE == model.MAST_DISTRICT_CODE && m.MAST_BLOCK_CODE == model.MAST_BLOCK_CODE && m.PLAN_CN_ROAD_NUMBER == model.PLAN_CN_ROAD_NUMBER && m.PLAN_RD_FROM_CHAINAGE == chainageFrom && m.PLAN_RD_TO_CHAINAGE == chainageTo && m.MAST_PMGSY_SCHEME == PMGSYSession.Current.PMGSYScheme).Count();

                    if (recordCount > 0)
                    {
                        message = "TR/MRL Road details already present.";
                        return false;
                    }

                    PLAN_ROAD master = new PLAN_ROAD();
                    // master = CloneModelToObjectPMGSY3(model);
               
                    #region From Clone
                    master.MAST_BLOCK_CODE = model.MAST_BLOCK_CODE;
                    master.MAST_DISTRICT_CODE = model.MAST_DISTRICT_CODE;
                    master.MAST_ER_ROAD_CODE = model.MAST_ER_ROAD_CODE;
                    master.MAST_STATE_CODE = model.MAST_STATE_CODE;
                    master.PLAN_CN_ROAD_NUMBER = model.PLAN_CN_ROAD_NUMBER;
                    master.PLAN_LOCK_STATUS = model.PLAN_LOCK_STATUS == null ? "N" : model.PLAN_LOCK_STATUS;
                    master.PLAN_RD_BLOCK_FROM_CODE = model.PLAN_RD_BLOCK_FROM_CODE;
                    master.PLAN_RD_BLOCK_TO_CODE = model.PLAN_RD_BLOCK_TO_CODE;
                    master.PLAN_RD_FROM_CHAINAGE = Convert.ToDecimal(model.PLAN_RD_FROM_CHAINAGE);
                    master.PLAN_RD_FROM_HAB = model.PLAN_RD_FROM_HAB;
                    master.PLAN_RD_FROM_TYPE = model.PLAN_RD_FROM_TYPE;
                    master.PLAN_RD_LENG = model.PLAN_RD_LENG;
                    master.PLAN_RD_LENGTH = Convert.ToDecimal(model.PLAN_RD_LENGTH);
                    master.PLAN_RD_NAME = (model.PLAN_RD_NAME == null ? dbContext.MASTER_EXISTING_ROADS.Where(m => m.MAST_ER_ROAD_CODE == model.MAST_ER_ROAD_CODE).Select(m => m.MAST_ER_ROAD_NAME).FirstOrDefault() : model.PLAN_RD_NAME); //model.PLAN_RD_NAME;
                    master.PLAN_RD_ROUTE = model.PLAN_RD_ROUTE;
                    master.PLAN_RD_TO_CHAINAGE = Convert.ToDecimal(model.PLAN_RD_TO_CHAINAGE);
                    master.PLAN_RD_TO_HAB = model.PLAN_RD_TO_HAB;
                    master.PLAN_RD_TO_TYPE = model.PLAN_RD_TO_TYPE;
                    switch (model.PLAN_RD_FROM_TYPE)
                    {
                        case "B":
                            master.PLAN_RD_FROM = "Block(" + dbContext.MASTER_BLOCK.Where(m => m.MAST_BLOCK_CODE == model.PLAN_RD_BLOCK_FROM_CODE).Select(m => m.MAST_BLOCK_NAME).FirstOrDefault() + ")";
                            master.PLAN_RD_NUM_FROM = model.PLAN_RD_NUM_FROM;
                            master.PLAN_RD_FROM_HAB = null;
                            break;
                        case "H":
                            master.PLAN_RD_FROM = "Habitation(" + (dbContext.MASTER_HABITATIONS.Where(m => m.MAST_HAB_CODE == model.PLAN_RD_FROM_HAB).Select(m => m.MAST_HAB_NAME).FirstOrDefault()) + ")";
                            master.PLAN_RD_NUM_FROM = null;
                            master.PLAN_RD_BLOCK_FROM_CODE = null;
                            break;
                        case "L":
                            master.PLAN_RD_FROM = dbContext.PLAN_ROAD.Where(m => m.PLAN_CN_ROAD_CODE == model.PLAN_RD_NUM_FROM).Select(m => m.PLAN_CN_ROAD_NUMBER).FirstOrDefault();
                            master.PLAN_RD_NUM_FROM = model.PLAN_RD_NUM_FROM;
                            master.PLAN_RD_BLOCK_FROM_CODE = null;
                            master.PLAN_RD_FROM_HAB = null;
                            break;
                        case "M":
                            master.PLAN_RD_FROM = dbContext.PLAN_ROAD.Where(m => m.PLAN_CN_ROAD_CODE == model.PLAN_RD_NUM_FROM).Select(m => m.PLAN_CN_ROAD_NUMBER).FirstOrDefault();
                            master.PLAN_RD_NUM_FROM = model.PLAN_RD_NUM_FROM;
                            master.PLAN_RD_BLOCK_FROM_CODE = null;
                            master.PLAN_RD_FROM_HAB = null;
                            break;
                        case "T":
                            master.PLAN_RD_FROM = dbContext.PLAN_ROAD.Where(m => m.PLAN_CN_ROAD_CODE == model.PLAN_RD_NUM_FROM).Select(m => m.PLAN_CN_ROAD_NUMBER).FirstOrDefault();
                            master.PLAN_RD_NUM_FROM = model.PLAN_RD_NUM_FROM;
                            master.PLAN_RD_BLOCK_FROM_CODE = null;
                            master.PLAN_RD_FROM_HAB = null;
                            break;
                        default:
                            master.PLAN_RD_FROM = dbContext.MASTER_EXISTING_ROADS.Where(m => m.MAST_ER_ROAD_CODE == model.PLAN_RD_NUM_FROM).Select(m => m.MAST_ER_ROAD_NUMBER).FirstOrDefault();
                            master.PLAN_RD_NUM_FROM = model.PLAN_RD_NUM_FROM;
                            master.PLAN_RD_BLOCK_FROM_CODE = null;
                            master.PLAN_RD_FROM_HAB = null;
                            break;
                    }

                    switch (model.PLAN_RD_TO_TYPE)
                    {
                        case "B":
                            master.PLAN_RD_TO = "Block(" + dbContext.MASTER_BLOCK.Where(m => m.MAST_BLOCK_CODE == model.PLAN_RD_BLOCK_TO_CODE).Select(m => m.MAST_BLOCK_NAME).FirstOrDefault() + ")";
                            master.PLAN_RD_NUM_TO = model.PLAN_RD_NUM_TO;
                            master.PLAN_RD_TO_HAB = null;
                            break;
                        case "H":
                            master.PLAN_RD_TO = "Habitation(" + (dbContext.MASTER_HABITATIONS.Where(m => m.MAST_HAB_CODE == model.PLAN_RD_TO_HAB).Select(m => m.MAST_HAB_NAME).FirstOrDefault()) + ")";
                            master.PLAN_RD_NUM_TO = null;
                            master.PLAN_RD_BLOCK_TO_CODE = null;
                            break;
                        case "L":
                            master.PLAN_RD_TO = dbContext.PLAN_ROAD.Where(m => m.PLAN_CN_ROAD_CODE == model.PLAN_RD_NUM_TO).Select(m => m.PLAN_CN_ROAD_NUMBER).FirstOrDefault();
                            master.PLAN_RD_NUM_TO = model.PLAN_RD_NUM_TO;
                            master.PLAN_RD_BLOCK_TO_CODE = null;
                            master.PLAN_RD_TO_HAB = null;
                            break;
                        case "M":
                            master.PLAN_RD_TO = dbContext.PLAN_ROAD.Where(m => m.PLAN_CN_ROAD_CODE == model.PLAN_RD_NUM_TO).Select(m => m.PLAN_CN_ROAD_NUMBER).FirstOrDefault();
                            master.PLAN_RD_NUM_TO = model.PLAN_RD_NUM_TO;
                            master.PLAN_RD_BLOCK_TO_CODE = null;
                            master.PLAN_RD_TO_HAB = null;
                            break;
                        case "T":
                            master.PLAN_RD_TO = dbContext.PLAN_ROAD.Where(m => m.PLAN_CN_ROAD_CODE == model.PLAN_RD_NUM_TO).Select(m => m.PLAN_CN_ROAD_NUMBER).FirstOrDefault();
                            master.PLAN_RD_NUM_TO = model.PLAN_RD_NUM_TO;
                            master.PLAN_RD_BLOCK_TO_CODE = null;
                            master.PLAN_RD_TO_HAB = null;
                            break;
                        default:
                            master.PLAN_RD_TO = dbContext.MASTER_EXISTING_ROADS.Where(m => m.MAST_ER_ROAD_CODE == model.PLAN_RD_NUM_TO).Select(m => m.MAST_ER_ROAD_NUMBER).FirstOrDefault();
                            master.PLAN_RD_BLOCK_TO_CODE = null;
                            master.PLAN_RD_NUM_TO = model.PLAN_RD_NUM_TO;
                            master.PLAN_RD_TO_HAB = null;
                            break;
                    }

                    master.PLAN_RD_TOTAL_LEN = model.TotalLengthOfCandidate;
                    master.MAST_PMGSY_SCHEME = PMGSYSession.Current.PMGSYScheme;
                    master.USERID = PMGSYSession.Current.UserId;
                    master.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                    master.PLAN_CN_ROAD_CODE = dbContext.PLAN_ROAD.Any() ? (from item in dbContext.PLAN_ROAD select item.PLAN_CN_ROAD_CODE).Max() + 1 : 1;

                  ObjectParameter outputParam = new ObjectParameter("OutputParameter" , 0);
                  dbContext.USP_INSERT_CN_PMGSY3(PMGSYSession.Current.PMGSYScheme, master.MAST_ER_ROAD_CODE, master.PLAN_CN_ROAD_NUMBER, master.MAST_STATE_CODE, master.MAST_DISTRICT_CODE, master.MAST_BLOCK_CODE, master.PLAN_RD_NAME, master.PLAN_RD_ROUTE, master.PLAN_RD_FROM_CHAINAGE, master.PLAN_RD_TO_CHAINAGE, master.PLAN_RD_LENG, master.PLAN_RD_LENGTH, master.PLAN_RD_TOTAL_LEN, master.PLAN_RD_FROM_TYPE, master.PLAN_RD_TO_TYPE, master.PLAN_RD_FROM_HAB, master.PLAN_RD_TO_HAB, master.PLAN_RD_BLOCK_FROM_CODE, master.PLAN_RD_BLOCK_TO_CODE, master.PLAN_RD_NUM_FROM, master.PLAN_RD_NUM_TO, master.PLAN_RD_FROM, master.PLAN_RD_TO, master.PLAN_LOCK_STATUS, master.USERID, master.IPADD, outputParam);
                
                  if (outputParam.Value.Equals(1))
                    {
                        return true;
                    }
                    else
                    {
                        message = "An Error Occured While proccessing your request";
                        return false;
                    }
                    //dbContext.PLAN_ROAD.Add(master);
                    //dbContext.SaveChanges();
                     #endregion
                     


                    #region Hab Mapping OLD Code Commented on 09 May 2020

                    //var lstHabCodes = dbContext.MASTER_ER_HABITATION_ROAD_PMGSY3.Where(m => m.MAST_ER_ROAD_CODE == master.MAST_ER_ROAD_CODE).Select(m => new { m.MAST_HAB_CODE, m.MAST_HAB_CODE_DIRECT, m.MAST_HAB_CODE_VERIFIED }).ToList();
                    //    if (lstHabCodes != null)
                    //    {
                    //        foreach (var item in lstHabCodes)
                    //        {
                    //            PLAN_ROAD_HABITATION_PMGSY3 mappingMaster = new PLAN_ROAD_HABITATION_PMGSY3();

                    //            mappingMaster.MAST_HAB_CODE = item.MAST_HAB_CODE;
                    //            mappingMaster.MAST_HAB_CODE_DIRECT = item.MAST_HAB_CODE_DIRECT;
                    //            mappingMaster.MAST_HAB_CODE_VERIFIED = item.MAST_HAB_CODE_VERIFIED;

                    //            mappingMaster.PLAN_CN_ROAD_CODE = master.PLAN_CN_ROAD_CODE;
                    //            mappingMaster.PLAN_CN_ROAD_HAB_ID = dbContext.PLAN_ROAD_HABITATION_PMGSY3.Any() ? dbContext.PLAN_ROAD_HABITATION_PMGSY3.Max(m => m.PLAN_CN_ROAD_HAB_ID) + 1 : 1;
                    //            mappingMaster.USERID = PMGSYSession.Current.UserId;
                    //            mappingMaster.IPADD = master.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                    //            dbContext.PLAN_ROAD_HABITATION_PMGSY3.Add(mappingMaster);
                    //            dbContext.SaveChanges();
                    //        }
                    //    }

                    #endregion


                    #region Hab Mapping New Code Developed on 09 May 2020
                   
                    //var lstHabCodes = dbContext.MAST_HAB_DETAILS_CSV_PMGSY3.Where(m => m.MAST_HAB_CSV_ER_CODE == master.MAST_ER_ROAD_CODE).Select(m => new { m.MAST_HAB_CSV_HAB_CODE }).ToList();

                    //Int32 CNHabId = (dbContext.PLAN_ROAD_HABITATION_PMGSY3.Any() ? dbContext.PLAN_ROAD_HABITATION_PMGSY3.Max(m => m.PLAN_CN_ROAD_HAB_ID) + 1 : 1);

                    //if (lstHabCodes != null)
                    //{
                    //    foreach (var item in lstHabCodes)
                    //    {
                    //        PLAN_ROAD_HABITATION_PMGSY3 mappingMaster = new PLAN_ROAD_HABITATION_PMGSY3();

                    //        if (!dbContext.PLAN_ROAD_HABITATION_PMGSY3.Any(m => m.PLAN_CN_ROAD_CODE == master.PLAN_CN_ROAD_CODE && m.MAST_HAB_CODE == item.MAST_HAB_CSV_HAB_CODE))
                    //        {
                    //            mappingMaster.MAST_HAB_CODE = item.MAST_HAB_CSV_HAB_CODE;
                    //            mappingMaster.MAST_HAB_CODE_DIRECT = "Y";
                    //            mappingMaster.MAST_HAB_CODE_VERIFIED = "Y";

                    //            mappingMaster.PLAN_CN_ROAD_CODE = master.PLAN_CN_ROAD_CODE;
                    //            mappingMaster.PLAN_CN_ROAD_HAB_ID = CNHabId;
                    //            mappingMaster.USERID = PMGSYSession.Current.UserId;
                    //            mappingMaster.IPADD = master.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                    //            dbContext.PLAN_ROAD_HABITATION_PMGSY3.Add(mappingMaster);

                    //            using (System.IO.StreamWriter sw = System.IO.File.AppendText(System.IO.Path.Combine("D:\\ErrorLog\\OMMASErrorLog_" + System.DateTime.Now.Date.ToString("dd_MM_yyyy") + ".txt", "")))
                    //            {
                    //                sw.WriteLine("Date :" + DateTime.Now.ToString());
                    //                sw.WriteLine("Method : " + "AddCoreNetworksPMGSY3DAL()");

                    //                sw.WriteLine(" .AST_HAB_CODE :" + mappingMaster.MAST_HAB_CODE);
                    //                sw.WriteLine("mappingMaster.PLAN_CN_ROAD_CODE :" + mappingMaster.PLAN_CN_ROAD_CODE);
                    //                sw.WriteLine("mappingMaster.PLAN_CN_ROAD_HAB_ID :" + mappingMaster.PLAN_CN_ROAD_HAB_ID);
                    //                sw.WriteLine("mappingMaster.USERID :" + mappingMaster.USERID);



                    //                sw.WriteLine("---------------------------------------------------------------------------------------");
                    //                sw.Close();
                    //            }
                    //            dbContext.SaveChanges();
                    //                }

                    //        CNHabId++;
                    //    }
                    //}
                  

                    #endregion


                //    ts.Complete();
                //}
                //end of change
                
            }
            catch (DbEntityValidationException e)
            {
                // dbContext.Configuration.AutoDetectChangesEnabled = true;
                ErrorLog.LogError(e, "AddCoreNetworksPMGSY3DAL().DbEntityValidationException");

                ModelStateDictionary modelstate = new ModelStateDictionary();
                foreach (var eve in e.EntityValidationErrors)
                {
                    foreach (var ve in eve.ValidationErrors)
                    {
                        modelstate.AddModelError(ve.ErrorMessage, ve.ErrorMessage);
                    }
                }

                ErrorLog.LogError(e, "AddCoreNetworksPMGSY3DAL().DbEntityValidationException()");
                using (System.IO.StreamWriter sw = System.IO.File.AppendText(System.IO.Path.Combine("D:\\ErrorLog\\OMMASErrorLog_" + System.DateTime.Now.Date.ToString("dd_MM_yyyy") + ".txt", "")))
                {
                    sw.WriteLine("Date :" + DateTime.Now.ToString());
                    sw.WriteLine("AddCoreNetworksPMGSY3DAL().DbEntityValidationException() : " + "Application_Error()");

                    sw.WriteLine("Exception : " + new CommonFunctions().FormatErrorMessage(modelstate));
                    sw.WriteLine("---------------------------------------------------------------------------------------");
                    sw.Close();
                }
                return false;
            }
            catch (UpdateException ex)
            {
                // dbContext.Configuration.AutoDetectChangesEnabled = true;
                ErrorLog.LogError(ex, "AddCoreNetworksPMGSY3DAL.UpdateException()");
                message = "An Error Occured While proccessing your request";
                return false;
            }
            catch (Exception ex)
            {
                //  dbContext.Configuration.AutoDetectChangesEnabled = true;
                ErrorLog.LogError(ex, "AddCoreNetworksPMGSY3DAL()");
                message = "An Error Occured While proccessing your request";
                return false;
            }
            finally
            {
                //   dbContext.Configuration.AutoDetectChangesEnabled = true; 
                if (dbContext != null)
                {
                    dbContext.Dispose();
                }
            }
        }

        public CoreNetworkViewModelPMGSY3 GetCoreNetworkDetailsPMGSY3(int networkCode)
        {
            PMGSYEntities dbContext = new PMGSYEntities();
            try
            {
                PLAN_ROAD master = dbContext.PLAN_ROAD.Where(m => m.PLAN_CN_ROAD_CODE == networkCode).FirstOrDefault();
                CoreNetworkViewModelPMGSY3 model = null;
                if (master != null)
                {
                    model = new CoreNetworkViewModelPMGSY3()
                    {
                        EncryptedRoadCode = URLEncrypt.EncryptParameters1(new string[] { "NetworkCode=" + master.PLAN_CN_ROAD_CODE.ToString().Trim() }),
                        MAST_BLOCK_CODE = master.MAST_BLOCK_CODE,
                        MAST_DISTRICT_CODE = master.MAST_DISTRICT_CODE,
                        MAST_ER_ROAD_CODE = master.MAST_ER_ROAD_CODE,
                        MAST_STATE_CODE = master.MAST_STATE_CODE,
                        PLAN_CN_ROAD_CODE = master.PLAN_CN_ROAD_CODE,
                        PLAN_CN_ROAD_NUMBER = master.PLAN_CN_ROAD_NUMBER,
                        PLAN_LOCK_STATUS = master.PLAN_LOCK_STATUS,
                        PLAN_RD_BLOCK_FROM_CODE = master.PLAN_RD_BLOCK_FROM_CODE,
                        PLAN_RD_BLOCK_TO_CODE = master.PLAN_RD_BLOCK_TO_CODE,
                        PLAN_RD_FROM = master.PLAN_RD_FROM_TYPE == "B" ? "Block(" + (dbContext.MASTER_BLOCK.Where(item => item.MAST_BLOCK_CODE == master.PLAN_RD_BLOCK_FROM_CODE).Select(m => m.MAST_BLOCK_NAME).FirstOrDefault()) + ")" : (master.PLAN_RD_FROM_TYPE == "H" ? "Habitation(" + (dbContext.MASTER_HABITATIONS.Where(item => item.MAST_HAB_CODE == master.PLAN_RD_FROM_HAB).Select(m => m.MAST_HAB_NAME).FirstOrDefault()) + ")" : (master.PLAN_RD_FROM == null ? string.Empty : master.PLAN_RD_FROM)),
                        PLAN_RD_FROM_CHAINAGE = Convert.ToDouble(master.PLAN_RD_FROM_CHAINAGE),
                        PLAN_RD_FROM_HAB = master.PLAN_RD_FROM_HAB,
                        PLAN_RD_FROM_TYPE = master.PLAN_RD_FROM_TYPE,
                        PLAN_RD_LENG = master.PLAN_RD_LENG,
                        PLAN_RD_LENGTH = (double)master.PLAN_RD_LENGTH,
                        PLAN_RD_NAME = master.PLAN_RD_NAME,
                        PLAN_RD_NUM_FROM = master.PLAN_RD_NUM_FROM,
                        PLAN_RD_NUM_TO = master.PLAN_RD_NUM_TO,
                        PLAN_RD_ROUTE = master.PLAN_RD_ROUTE,
                        PLAN_RD_TO = master.PLAN_RD_TO_TYPE == "B" ? "Block(" + (dbContext.MASTER_BLOCK.Where(item => item.MAST_BLOCK_CODE == master.PLAN_RD_BLOCK_TO_CODE).Select(m => m.MAST_BLOCK_NAME).FirstOrDefault()) + ")" : (master.PLAN_RD_TO_TYPE == "H" ? "Habitation(" + (dbContext.MASTER_HABITATIONS.Where(item => item.MAST_HAB_CODE == master.PLAN_RD_TO_HAB).Select(m => m.MAST_HAB_NAME).FirstOrDefault()) + ")" : (master.PLAN_RD_TO == null ? string.Empty : master.PLAN_RD_TO)),
                        PLAN_RD_TO_CHAINAGE = (double)master.PLAN_RD_TO_CHAINAGE,
                        PLAN_RD_TO_HAB = master.PLAN_RD_TO_HAB,
                        PLAN_RD_TO_TYPE = master.PLAN_RD_TO_TYPE,
                        TotalLengthOfCandidate = Convert.ToDecimal(master.PLAN_RD_TOTAL_LEN),
                        ExistStartChainage = Convert.ToDouble(dbContext.MASTER_EXISTING_ROADS.Where(m => m.MAST_ER_ROAD_CODE == master.MAST_ER_ROAD_CODE).Select(m => m.MAST_ER_ROAD_STR_CHAIN).FirstOrDefault()),
                        ExistEndChainage = Convert.ToDouble(dbContext.MASTER_EXISTING_ROADS.Where(m => m.MAST_ER_ROAD_CODE == master.MAST_ER_ROAD_CODE).Select(m => m.MAST_ER_ROAD_END_CHAIN).FirstOrDefault()),

                        RoadShortCode = dbContext.MASTER_EXISTING_ROADS.Where(r => r.MAST_ER_ROAD_CODE == master.MAST_ER_ROAD_CODE).Select(a => a.MASTER_ROAD_CATEGORY.MAST_ROAD_SHORT_DESC).FirstOrDefault(),
                    };
                }
                return model;
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "GetCoreNetworkDetailsPMGSY3().DAL");
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

        public bool EditCoreNetworksPMGSY3DAL(CoreNetworkViewModelPMGSY3 model, ref string message)
        {
            PMGSYEntities dbContext = new PMGSYEntities();
            try
            {
                int networkCode = 0;
                encryptedParameters = model.EncryptedRoadCode.Split('/');
                if (!(encryptedParameters.Length == 3))
                {
                    return false;
                }

                decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { encryptedParameters[0], encryptedParameters[1], encryptedParameters[2] });
                networkCode = Convert.ToInt32(decryptedParameters["NetworkCode"].ToString());

                decimal chainageFrom = Convert.ToDecimal(model.PLAN_RD_FROM_CHAINAGE);
                decimal chainageTo = Convert.ToDecimal(model.PLAN_RD_TO_CHAINAGE);

                using (TransactionScope ts = new TransactionScope())
                {
                    if (dbContext.PLAN_ROAD_MRL_PMGSY3.Any(m => m.PLAN_CN_ROAD_CODE == networkCode))
                    {
                        if (model.TotalLengthOfCandidate < (dbContext.PLAN_ROAD_MRL_PMGSY3.Where(m => m.PLAN_CN_ROAD_CODE == networkCode).Sum(m => m.PLAN_RD_LENGTH) + Convert.ToDecimal(model.PLAN_RD_LENGTH)))
                        {
                            message = "Total length of TR/MRL Road should not be less than the sum of TR/MRL road length and other DRRP road length mapped to it.";
                            return false;
                        }
                    }

                    int recordCount = dbContext.PLAN_ROAD.Where(m => m.MAST_STATE_CODE == model.MAST_STATE_CODE && m.MAST_DISTRICT_CODE == model.MAST_DISTRICT_CODE && m.MAST_BLOCK_CODE == model.MAST_BLOCK_CODE && m.PLAN_CN_ROAD_NUMBER == model.PLAN_CN_ROAD_NUMBER && m.PLAN_RD_FROM_CHAINAGE == chainageFrom && m.PLAN_RD_TO_CHAINAGE == chainageTo && m.PLAN_CN_ROAD_CODE != networkCode && m.MAST_PMGSY_SCHEME == PMGSYSession.Current.PMGSYScheme).Count();
                    if (recordCount > 0)
                    {
                        message = "TR/MRL Road details already present.";

                        return false;
                    }

                    PLAN_ROAD master = dbContext.PLAN_ROAD.Where(m => m.PLAN_CN_ROAD_CODE == networkCode).FirstOrDefault();

                    if (!dbContext.PLAN_ROAD_MRL_PMGSY3.Where(m => m.PLAN_CN_ROAD_CODE == networkCode && m.PLAN_LOCK_STATUS == "N").Any())
                    {
                        if (master.PLAN_RD_TOTAL_LEN != model.TotalLengthOfCandidate)
                        {
                            message = "Cannot change Total length of TR/MRL road as all mapped DRRP raods are finalized.";
                            return false;
                        }
                    }

                    if (master != null)
                    {
                        master = CloneModelToObjectPMGSY3(model);
                        master.PLAN_CN_ROAD_CODE = networkCode;
                        master.USERID = PMGSYSession.Current.UserId;
                        master.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                        //dbContext.Entry(master).State = System.Data.Entity.EntityState.Modified;
                        var currentProduct = dbContext.PLAN_ROAD.Where(m => m.PLAN_CN_ROAD_CODE == networkCode).FirstOrDefault(); ;
                        dbContext.Entry(currentProduct).CurrentValues.SetValues(master);
                        dbContext.SaveChanges();
                        ts.Complete();
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            catch (DbEntityValidationException e)
            {
                ErrorLog.LogError(e, "EditCoreNetworksPMGSY3DAL().DbEntityValidationException()");
                ModelStateDictionary modelstate = new ModelStateDictionary();
                foreach (var eve in e.EntityValidationErrors)
                {
                    foreach (var ve in eve.ValidationErrors)
                    {
                        modelstate.AddModelError(ve.ErrorMessage, ve.ErrorMessage);
                    }
                }

                using (System.IO.StreamWriter sw = System.IO.File.AppendText(System.IO.Path.Combine("D:\\ErrorLog\\OMMASErrorLog_" + System.DateTime.Now.Date.ToString("dd_MM_yyyy") + ".txt", "")))
                {
                    sw.WriteLine("Date :" + DateTime.Now.ToString());
                    sw.WriteLine("EditCoreNetworksPMGSY3DAL().DbEntityValidationException() : " + "Application_Error()");

                    sw.WriteLine("Exception : " + new CommonFunctions().FormatErrorMessage(modelstate));
                    sw.WriteLine("---------------------------------------------------------------------------------------");
                    sw.Close();
                }
                return false;
            }
            catch (UpdateException ex)
            {
                ErrorLog.LogError(ex, "EditCoreNetworksPMGSY3DAL.UpdateException()");
                message = "An Error Occured While proccessing your request";
                return false;
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "EditCoreNetworksPMGSY3DAL()");
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

        public bool DeleteCoreNetworksPMGSY3DAL(int networkCode)
        {
            PMGSYEntities dbContext = new PMGSYEntities();
            try
            {
                using (TransactionScope ts = new TransactionScope())
                {
                    if (dbContext.IMS_UNLOCK_DETAILS.Any(m => m.PLAN_CN_ROAD_CODE == networkCode && m.MAST_PMGSY_SCHEME == PMGSYSession.Current.PMGSYScheme))
                    {
                        IMS_UNLOCK_DETAILS unlockDetails = dbContext.IMS_UNLOCK_DETAILS.Where(m => m.PLAN_CN_ROAD_CODE == networkCode && m.MAST_PMGSY_SCHEME == PMGSYSession.Current.PMGSYScheme).FirstOrDefault();
                        if (unlockDetails != null)
                        {
                            unlockDetails.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                            unlockDetails.USERID = PMGSYSession.Current.UserId;
                            dbContext.Entry(unlockDetails).State = System.Data.Entity.EntityState.Modified;
                            dbContext.SaveChanges();

                            dbContext.IMS_UNLOCK_DETAILS.Remove(unlockDetails);
                            dbContext.SaveChanges();
                        }

                    }

                    PLAN_ROAD master = dbContext.PLAN_ROAD.Where(m => m.PLAN_CN_ROAD_CODE == networkCode && m.MAST_PMGSY_SCHEME == PMGSYSession.Current.PMGSYScheme).FirstOrDefault();
                    master.USERID = PMGSYSession.Current.UserId;
                    master.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                    dbContext.Entry(master).State = System.Data.Entity.EntityState.Modified;
                    dbContext.SaveChanges();

                    List<PLAN_ROAD_HABITATION_PMGSY3> lstRoads = dbContext.PLAN_ROAD_HABITATION_PMGSY3.Where(m => m.PLAN_CN_ROAD_CODE == networkCode && m.PLAN_ROAD.MAST_PMGSY_SCHEME == PMGSYSession.Current.PMGSYScheme).ToList();
                    if (lstRoads.Count > 0)
                    {
                        foreach (var item in lstRoads)
                        {
                            dbContext.PLAN_ROAD_HABITATION_PMGSY3.Remove(item);
                        }
                    }
                    dbContext.PLAN_ROAD.Remove(master);
                    dbContext.SaveChanges();
                    ts.Complete();
                }
                return true;
            }
            catch (DbUpdateException ex)
            {
                ErrorLog.LogError(ex, "DeleteCoreNetworksPMGSY3DAL().DbUpdateException()");
                return false;
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "DeleteCoreNetworksPMGSY3DAL");
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


        //getDistrictByState
        public List<SelectListItem> GetDistrictsByStateCodePMGSY3(int stateCode)
        {

            PMGSYEntities dbContext = new PMGSYEntities();
            try
            {

                var lstDistrictsByState = dbContext.MASTER_DISTRICT.Where(m => m.MAST_STATE_CODE == stateCode && m.MAST_DISTRICT_ACTIVE == "Y").OrderBy(a => a.MAST_DISTRICT_NAME).ToList();
                return new SelectList(lstDistrictsByState.ToList(), "MAST_DISTRICT_CODE", "MAST_DISTRICT_NAME").ToList();

            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "GetDistrictsByState()");
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

        //inter District Habitation Mapping
        public List<SelectListItem> GetRoadsByBlockCode(int blockCode, int districtCode)
        {
            SelectListItem item;
            PMGSYEntities dbContext = new PMGSYEntities();
            try
            {
                List<SelectListItem> lstRoads = new List<SelectListItem>();
                lstRoads.Insert(0, new SelectListItem { Value = "0", Text = "--Select Road--" });
                var lstExistingRoad = (from PR in dbContext.MASTER_EXISTING_ROADS where (PR.MAST_PMGSY_SCHEME == 2 && PR.MAST_BLOCK_CODE == blockCode && PR.MAST_DISTRICT_CODE == districtCode) select new { PR.MAST_ER_ROAD_CODE, PR.MAST_ER_ROAD_NAME });

                if (lstExistingRoad != null)
                {
                    foreach (var itm in lstExistingRoad)
                    {
                        item = new SelectListItem();
                        item.Text = itm.MAST_ER_ROAD_NAME;
                        item.Value = itm.MAST_ER_ROAD_CODE.ToString();
                        lstRoads.Add(item);
                    }
                }

                return lstRoads;
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "GetRoadsByRoadCodePMGSY3()");
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


        public Array GetHabitationListToMapPMGSY3DAL_InterDistricthabitation(int roadCode, string habDirect, int erRoadCode, int page, int rows, string sidx, string sord, out long totalRecords)
        {

            List<int> lstBlocks = new List<int>();
            PMGSYEntities dbContext = new PMGSYEntities();
            totalRecords = 0;


            try
            {
                PLAN_ROAD masterRoad = dbContext.PLAN_ROAD.Find(roadCode);

                if (erRoadCode > 0)
                {
                    var lstHabitations = (from item in dbContext.MASTER_VILLAGE
                                          join habitation in dbContext.MASTER_HABITATIONS on item.MAST_VILLAGE_CODE equals habitation.MAST_VILLAGE_CODE
                                          join habDetails in dbContext.MASTER_HABITATIONS_DETAILS on habitation.MAST_HAB_CODE equals habDetails.MAST_HAB_CODE
                                          join erHabs in dbContext.MASTER_ER_HABITATION_ROAD_PMGSY3 on habitation.MAST_HAB_CODE equals erHabs.MAST_HAB_CODE
                                          where erHabs.MAST_ER_ROAD_CODE == erRoadCode
                                            && habitation.MAST_HABITATION_ACTIVE == "Y"
                                            && habDetails.MAST_YEAR == 2011




                                          select new
                                          {
                                              habitation.MAST_HAB_NAME,
                                              item.MAST_VILLAGE_NAME,
                                              habDetails.MAST_HAB_TOT_POP,
                                              habitation.MAST_HAB_CODE

                                          });


                    List<int> mapHabitations = (from item in dbContext.PLAN_ROAD_HABITATION_PMGSY3
                                                where
                                                item.PLAN_CN_ROAD_CODE == roadCode
                                                select item.MAST_HAB_CODE).Distinct().ToList<int>();


                    var listHab = (from item in lstHabitations
                                   where !mapHabitations.Contains(item.MAST_HAB_CODE)
                                   select item.MAST_HAB_CODE).Distinct();



                    var mappingList = (from item in dbContext.MASTER_HABITATIONS_DETAILS
                                       join habitation in dbContext.MASTER_HABITATIONS on item.MAST_HAB_CODE equals habitation.MAST_HAB_CODE
                                       join village in dbContext.MASTER_VILLAGE on habitation.MAST_VILLAGE_CODE equals village.MAST_VILLAGE_CODE
                                       where listHab.Contains(item.MAST_HAB_CODE) && item.MAST_YEAR == 2011
                                       select new
                                       {
                                           item.MAST_HAB_CODE,
                                           habitation.MAST_HAB_NAME,
                                           village.MAST_VILLAGE_NAME,
                                           item.MAST_HAB_TOT_POP
                                       }).Distinct().ToList();
                    using (System.IO.StreamWriter sw = System.IO.File.AppendText(System.IO.Path.Combine("D:\\ErrorLog\\OMMASErrorLog_" + System.DateTime.Now.Date.ToString("dd_MM_yyyy") + ".txt", "")))
                    {
                        sw.WriteLine("Date :" + DateTime.Now.ToString());
                        sw.WriteLine("Mapping List : When route is not N Before habDirect");
                        foreach (var item in mappingList)
                        {
                            sw.WriteLine("MAST_HAB_CODE :::" + item.MAST_HAB_CODE);
                            sw.WriteLine("MAST_HAB_NAME :::" + item.MAST_HAB_NAME);
                            sw.WriteLine("MAST_VILLAGE_NAME :::" + item.MAST_VILLAGE_NAME);
                            sw.WriteLine("MAST_HAB_TOT_POP :::" + item.MAST_HAB_TOT_POP);
                        }
                        sw.WriteLine("---------------------------------------------------------------------------------------");
                        sw.Close();
                    }


                    totalRecords = mappingList.Count();

                    if (sidx.Trim() != string.Empty)
                    {
                        if (sord.ToString() == "asc")
                        {
                            mappingList = mappingList.OrderBy(x => x.MAST_HAB_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                        }
                        else
                        {
                            mappingList = mappingList.OrderByDescending(x => x.MAST_HAB_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                        }
                    }
                    else
                    {
                        mappingList = mappingList.OrderBy(x => x.MAST_HAB_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                    }

                    var result = mappingList.Select(habDetails => new
                    {
                        habDetails.MAST_HAB_CODE,
                        habDetails.MAST_HAB_NAME,
                        habDetails.MAST_VILLAGE_NAME,
                        habDetails.MAST_HAB_TOT_POP
                    }).ToArray();

                    return result.Select(habDetails => new
                    {
                        id = URLEncrypt.EncryptParameters1(new string[] { "HabCode =" + habDetails.MAST_HAB_CODE.ToString().Trim() }),
                        cell = new[] {
                            
                        habDetails.MAST_HAB_NAME.ToString(),
                        habDetails.MAST_VILLAGE_NAME.ToString(),
                        habDetails.MAST_HAB_TOT_POP.ToString()
                    }
                    }).ToArray();
                }
                else
                {
                    return null;

                }


            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "GetHabitationListToMapPMGSY3DAL()");
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






        //Habitation Mapping
        public List<SelectListItem> GetRoadsByCNCodePMGSY3(int networkCode)
        {
            SelectListItem item;
            PMGSYEntities dbContext = new PMGSYEntities();
            try
            {
                List<SelectListItem> lstRoads = new List<SelectListItem>();
                lstRoads.Insert(0, new SelectListItem { Text = "All Roads", Value = "0" }); // Commented on 11 May 2020 to populate All Road Option
                //  lstRoads.Insert(0, new SelectListItem { Text = "Select Road", Value = "-1" }); // Commented on 11 May 2020 to populate All Road Option
                //List<SelectListItem> lstRoads = (from PR in dbContext.PLAN_ROAD where PR.PLAN_CN_ROAD_CODE == networkCode select PR.MAST_ER_ROAD_CODE).Union(from PR in dbContext.PLAN_ROAD where PR.PLAN_CN_ROAD_CODE == networkCode select PR.MAST_ER_ROAD_CODE);
                var lstPlanRoad = (from PR in dbContext.PLAN_ROAD where PR.PLAN_CN_ROAD_CODE == networkCode select new { PR.MAST_ER_ROAD_CODE, PR.MASTER_EXISTING_ROADS.MAST_ER_ROAD_NAME });
                var lstPlanRoadDRRP = (from PRD in dbContext.PLAN_ROAD_MRL_PMGSY3 where PRD.PLAN_CN_ROAD_CODE == networkCode select new { PRD.MAST_ER_ROAD_CODE, PRD.MASTER_EXISTING_ROADS.MAST_ER_ROAD_NAME });

                var lstquery = lstPlanRoad.Union(lstPlanRoadDRRP);
                if (lstquery != null)
                {
                    foreach (var itm in lstquery)
                    {
                        item = new SelectListItem();
                        item.Text = itm.MAST_ER_ROAD_NAME;
                        item.Value = itm.MAST_ER_ROAD_CODE.ToString();
                        lstRoads.Add(item);
                    }
                }
                return lstRoads;
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "GetRoadsByCNCodePMGSY3()");
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

        public Array GetHabitationListPMGSY3DAL(int roadCode, string flag, int page, int rows, string sidx, string sord, out long totalRecords, out bool isHabFinalized)
        {
            PMGSYEntities dbContext = new PMGSYEntities();

            #region TRMRLHabs     3
            List<int> lstBlocks = new List<int>();

            //lstBlocks.Add(51);
            //lstBlocks.Add(1341);
            //lstBlocks.Add(2166);
            //lstBlocks.Add(2437);
            //lstBlocks.Add(3566);
            //lstBlocks.Add(5138);
            //lstBlocks.Add(5931);
            //lstBlocks.Add(6347);
            //lstBlocks.Add(7745);
            //lstBlocks.Add(664);
            //lstBlocks.Add(766);
            //lstBlocks.Add(3777);
            //lstBlocks.Add(4221);
            //lstBlocks.Add(4740);
            //lstBlocks.Add(5067);
            //lstBlocks.Add(5351);
            //lstBlocks.Add(5880);
            //lstBlocks.Add(7746);
            //lstBlocks.Add(1762);
            //lstBlocks.Add(3174);
            //lstBlocks.Add(152);
            //lstBlocks.Add(653);
            //lstBlocks.Add(2935);
            //lstBlocks.Add(2996);
            //lstBlocks.Add(5670);
            //lstBlocks.Add(5);
            //lstBlocks.Add(1781);
            //lstBlocks.Add(1783);
            //lstBlocks.Add(1911);
            //lstBlocks.Add(2087);
            //lstBlocks.Add(2381);
            //lstBlocks.Add(3020);
            //lstBlocks.Add(3574);
            //lstBlocks.Add(3615);
            //lstBlocks.Add(6484);
            //lstBlocks.Add(660);
            //lstBlocks.Add(1513);
            //lstBlocks.Add(1585);
            //lstBlocks.Add(1637);
            //lstBlocks.Add(1770);
            //lstBlocks.Add(2079);
            //lstBlocks.Add(2580);
            //lstBlocks.Add(2615);
            //lstBlocks.Add(4895);
            //lstBlocks.Add(5593);
            //lstBlocks.Add(7747);
            //lstBlocks.Add(892);
            //lstBlocks.Add(1464);
            //lstBlocks.Add(1876);
            //lstBlocks.Add(2107);
            //lstBlocks.Add(2245);
            //lstBlocks.Add(2246);
            //lstBlocks.Add(3532);
            //lstBlocks.Add(3960);
            //lstBlocks.Add(5881);
            //lstBlocks.Add(5893);
            //lstBlocks.Add(16);
            //lstBlocks.Add(872);
            //lstBlocks.Add(2388);
            //lstBlocks.Add(2389);
            //lstBlocks.Add(3432);
            //lstBlocks.Add(4102);
            //lstBlocks.Add(4357);
            //lstBlocks.Add(4733);
            //lstBlocks.Add(5217);
            //lstBlocks.Add(5512);
            //lstBlocks.Add(7748);
            //lstBlocks.Add(1609);
            //lstBlocks.Add(2749);
            //lstBlocks.Add(4045);
            //lstBlocks.Add(4724);
            //lstBlocks.Add(5798);
            //lstBlocks.Add(1486);
            //lstBlocks.Add(1666);
            //lstBlocks.Add(2347);
            //lstBlocks.Add(2957);
            //lstBlocks.Add(3441);
            //lstBlocks.Add(3442);
            //lstBlocks.Add(3458);
            //lstBlocks.Add(4435);
            //lstBlocks.Add(5328);
            //lstBlocks.Add(5611);
            //lstBlocks.Add(5780);
            //lstBlocks.Add(7749);
            //lstBlocks.Add(7750);
            //lstBlocks.Add(847);
            //lstBlocks.Add(1059);
            //lstBlocks.Add(2506);
            //lstBlocks.Add(3704);
            //lstBlocks.Add(5413);
            //lstBlocks.Add(379);
            //lstBlocks.Add(3149);
            //lstBlocks.Add(3876);
            //lstBlocks.Add(3877);
            //lstBlocks.Add(4298);
            //lstBlocks.Add(3148);
            //lstBlocks.Add(3370);
            //lstBlocks.Add(3606);
            //lstBlocks.Add(3965);
            //lstBlocks.Add(310);
            //lstBlocks.Add(434);
            //lstBlocks.Add(502);
            //lstBlocks.Add(4254);
            //lstBlocks.Add(5420);
            //lstBlocks.Add(891);
            //lstBlocks.Add(1913);
            //lstBlocks.Add(4042);
            //lstBlocks.Add(4620);
            //lstBlocks.Add(4628);
            //lstBlocks.Add(5002);
            //lstBlocks.Add(5315);
            //lstBlocks.Add(5379);
            //lstBlocks.Add(174);
            //lstBlocks.Add(1134);
            //lstBlocks.Add(3924);
            //lstBlocks.Add(4359);
            //lstBlocks.Add(5197);
            //lstBlocks.Add(113);
            //lstBlocks.Add(183);
            //lstBlocks.Add(832);
            //lstBlocks.Add(1622);
            //lstBlocks.Add(3405);
            //lstBlocks.Add(3588);
            //lstBlocks.Add(3589);
            //lstBlocks.Add(5363);
            //lstBlocks.Add(5561);
            //lstBlocks.Add(5806);
            //lstBlocks.Add(6946);
            //lstBlocks.Add(6947);
            //lstBlocks.Add(6948);
            //lstBlocks.Add(6949);
            //lstBlocks.Add(6950);
            //lstBlocks.Add(6951);
            //lstBlocks.Add(6952);
            //lstBlocks.Add(6953);
            //lstBlocks.Add(6954);
            //lstBlocks.Add(6955);
            //   lstBlocks.Add(1532);locked (commented) on 9 march 2020


            #endregion

            try
            {
                var lstHabitations = (from item in dbContext.PLAN_ROAD_HABITATION_PMGSY3
                                      join habitation in dbContext.MASTER_HABITATIONS on item.MAST_HAB_CODE equals habitation.MAST_HAB_CODE
                                      join habCode in dbContext.MASTER_HABITATIONS_DETAILS on item.MAST_HAB_CODE equals habCode.MAST_HAB_CODE
                                      join roadPlan in dbContext.PLAN_ROAD on item.PLAN_CN_ROAD_CODE equals roadPlan.PLAN_CN_ROAD_CODE
                                      where item.PLAN_CN_ROAD_CODE == roadCode &&
                                      habCode.MAST_YEAR == 2011
                                      select new
                                      {
                                          roadPlan.PLAN_CN_ROAD_NUMBER,
                                          roadPlan.PLAN_LOCK_STATUS,
                                          habitation.MAST_HAB_NAME,
                                          habCode.MAST_HAB_CODE,
                                          habCode.MAST_BUS_SERVICE,
                                          habCode.MAST_DEGREE_COLLEGE,
                                          habCode.MAST_DISPENSARY,
                                          habCode.MAST_ELECTRICTY,
                                          habCode.MAST_HAB_CONNECTED,
                                          habCode.MAST_HAB_SCST_POP,
                                          habCode.MAST_HAB_TOT_POP,
                                          habCode.MAST_HEALTH_SERVICE,
                                          habCode.MAST_HIGH_SCHOOL,
                                          habCode.MAST_INTERMEDIATE_SCHOOL,
                                          habCode.MAST_MCW_CENTERS,
                                          habCode.MAST_MIDDLE_SCHOOL,
                                          habCode.MAST_PANCHAYAT_HQ,
                                          habCode.MAST_PHCS,
                                          habCode.MAST_PRIMARY_SCHOOL,
                                          habCode.MAST_RAILWAY_STATION,
                                          habCode.MAST_SCHEME,
                                          habCode.MAST_TELEGRAPH_OFFICE,
                                          habCode.MAST_TELEPHONE_CONNECTION,
                                          habCode.MAST_TOURIST_PLACE,
                                          habCode.MAST_VETNARY_HOSPITAL,
                                          habCode.MAST_YEAR,
                                          roadPlan.MAST_BLOCK_CODE,
                                          habitation.MASTER_VILLAGE.MAST_VILLAGE_NAME,
                                          roadPlan.PLAN_CN_ROAD_CODE,
                                          habitation.MASTER_VILLAGE.MASTER_BLOCK.MAST_BLOCK_NAME,
                                          MAST_HAB_CODE_DIRECT = item.MAST_HAB_CODE_DIRECT == "Y" ? "Yes" : "No",
                                          MAST_HAB_CODE_VERIFIED = item.MAST_HAB_CODE_VERIFIED == "Y" ? "Yes" : "No",
                                          item.PLAN_CN_HAB_FINALIZED

                                          //roadPlan.MASTER_BLOCK.MAST_BLOCK_NAME
                                      }).Distinct().GroupBy(m => m.MAST_HAB_CODE).Where(g => g.Count() == 1 || g.Count() > 1)
                                        .Select(g => g.FirstOrDefault());

                totalRecords = lstHabitations.Count();

                isHabFinalized = dbContext.PLAN_ROAD_HABITATION_PMGSY3.Where(z => z.PLAN_CN_ROAD_CODE == roadCode && z.PLAN_CN_HAB_FINALIZED == "Y").Any();

                if (sidx.Trim() != string.Empty)
                {
                    if (sord.ToString() == "asc")
                    {
                        lstHabitations = lstHabitations.OrderBy(x => x.MAST_HAB_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                    }
                    else
                    {
                        lstHabitations = lstHabitations.OrderByDescending(x => x.MAST_HAB_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                    }
                }
                else
                {
                    lstHabitations = lstHabitations.OrderBy(x => x.MAST_HAB_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                }

                var result = lstHabitations.Select(habDetails => new
                {

                    habDetails.PLAN_CN_ROAD_NUMBER,
                    habDetails.MAST_HAB_NAME,
                    habDetails.MAST_BUS_SERVICE,
                    habDetails.MAST_DEGREE_COLLEGE,
                    habDetails.MAST_HAB_CODE,
                    habDetails.MAST_DISPENSARY,
                    habDetails.MAST_ELECTRICTY,
                    habDetails.MAST_HAB_CONNECTED,
                    habDetails.MAST_HAB_SCST_POP,
                    habDetails.MAST_HAB_TOT_POP,
                    habDetails.MAST_HEALTH_SERVICE,
                    habDetails.MAST_HIGH_SCHOOL,
                    habDetails.MAST_INTERMEDIATE_SCHOOL,
                    habDetails.MAST_MCW_CENTERS,
                    habDetails.MAST_MIDDLE_SCHOOL,
                    habDetails.MAST_PANCHAYAT_HQ,
                    habDetails.MAST_PHCS,
                    habDetails.MAST_PRIMARY_SCHOOL,
                    habDetails.MAST_RAILWAY_STATION,
                    habDetails.MAST_SCHEME,
                    habDetails.MAST_TELEGRAPH_OFFICE,
                    habDetails.MAST_TELEPHONE_CONNECTION,
                    habDetails.MAST_TOURIST_PLACE,
                    habDetails.MAST_VETNARY_HOSPITAL,
                    habDetails.MAST_YEAR,
                    habDetails.MAST_BLOCK_NAME,
                    habDetails.MAST_VILLAGE_NAME,
                    habDetails.PLAN_LOCK_STATUS,
                    habDetails.MAST_BLOCK_CODE,
                    habDetails.PLAN_CN_ROAD_CODE,
                    habDetails.MAST_HAB_CODE_DIRECT,
                    habDetails.MAST_HAB_CODE_VERIFIED,
                    PLAN_CN_HAB_FINALIZED = string.IsNullOrEmpty(habDetails.PLAN_CN_HAB_FINALIZED) ? "N" : habDetails.PLAN_CN_HAB_FINALIZED
                }).ToArray();

                short roleCode = dbContext.UM_User_Master.Where(m => m.UserID == PMGSYSession.Current.UserId).Select(m => m.DefaultRoleID).FirstOrDefault();
                return result.Select(habDetails => new
                {
                    cell = new[]
                {
                    habDetails.MAST_HAB_CODE.ToString(),
                    habDetails.MAST_HAB_NAME == null?string.Empty:habDetails.MAST_HAB_NAME.ToString(),
                    habDetails.MAST_BLOCK_NAME == null?string.Empty:habDetails.MAST_BLOCK_NAME.ToString(),
                    habDetails.MAST_VILLAGE_NAME == null?string.Empty:habDetails.MAST_VILLAGE_NAME.ToString(),
                    habDetails.PLAN_CN_ROAD_NUMBER == null?string.Empty:habDetails.PLAN_CN_ROAD_NUMBER.ToString(),
                    habDetails.MAST_HAB_TOT_POP==null?"0":habDetails.MAST_HAB_TOT_POP.ToString(),
                    habDetails.MAST_HAB_SCST_POP==null?"0":habDetails.MAST_HAB_SCST_POP.ToString(),//New SC/ST Population,
                    habDetails.MAST_HAB_CODE_DIRECT,
                    habDetails.MAST_HAB_CODE_VERIFIED,
                    //(PMGSYSession.Current.StateCode == 29 /*|| PMGSYSession.Current.StateCode == 17 || PMGSYSession.Current.StateCode == 13*/) 
                    //?   "<center><span  class='ui-icon ui-icon-trash' title='Enter Habitation Details' onClick ='deleteHabitationDetails(\"" + URLEncrypt.EncryptParameters1(new string[]{"HabCode =" + habDetails.MAST_HAB_CODE.ToString().Trim(),"Flag                               ="+flag.ToString().Trim()}) + "\");'></span></center>"
                    //:
                    //TRMRLHabs     4
                    (
                    // Below line is commented on 17 April 20202
                       // (dbContext.UDF_IMS_UNLOCK_STATUS(PMGSYSession.Current.StateCode,PMGSYSession.Current.DistrictCode,habDetails.MAST_BLOCK_CODE,0,0,habDetails.PLAN_CN_ROAD_CODE,0,0,"CN",PMGSYSession.Current.PMGSYScheme,roleCode).Select(m=>m.UNLOCK_COUNT).FirstOrDefault().Value > 0 || lstBlocks.Contains(habDetails.MAST_BLOCK_CODE))
                   (checkIsUnlocked(PMGSYSession.Current.StateCode, PMGSYSession.Current.DistrictCode, habDetails.MAST_BLOCK_CODE)==true || lstBlocks.Contains(habDetails.MAST_BLOCK_CODE))
                        
                        
                        
                        ? "<center><span  class='ui-icon ui-icon-trash' title='Enter Habitation Details' onClick ='deleteHabitationDetails(\"" + URLEncrypt.EncryptParameters1(new string[]{"HabCode =" + habDetails.MAST_HAB_CODE.ToString().Trim(),"Flag                                  ="+flag.ToString().Trim()}) + "\");'></span></center>" 
                    : habDetails.PLAN_LOCK_STATUS == "Y" 
                        ? "<span class='ui-icon ui-icon-locked ui-align-center'></span>" 
                        : habDetails.PLAN_CN_HAB_FINALIZED == "Y" 
                            ? "<span class='ui-icon ui-icon-locked ui-align-center'></span>" 
                            : "<center><span  class='ui-icon ui-icon-trash' title='Enter Habitation Details' onClick ='deleteHabitationDetails(\"" + URLEncrypt.EncryptParameters1(new string[]{"HabCode =" + habDetails.MAST_HAB_CODE.ToString().Trim(),"Flag                                  ="+flag.ToString().Trim()}) + "\");'></span></center>")
                }
                }).ToArray();
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "GetHabitationListPMGSY3DAL()");
                totalRecords = 0;
                isHabFinalized = false;
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


        //HabMap
        public Array GetHabitationListToMapPMGSY3DAL(int roadCode, string habDirect, int erRoadCode, int page, int rows, string sidx, string sord, out long totalRecords, out bool isHabFinalized)
        {
            #region TRMRLHabs   5
            List<int> lstBlocks = new List<int>();
            //lstBlocks.Add(51);
            //  lstBlocks.Add(1532);locked (commented) on 9 march 2020
            #endregion
            PMGSYEntities dbContext = new PMGSYEntities();
            totalRecords = 0;
            isHabFinalized = false;

            try
            {
                PLAN_ROAD masterRoad = dbContext.PLAN_ROAD.Find(roadCode);
                List<int> lstErCodesFromPlanRoad;
                List<int> lstErCodesFromPlanRoadMRL;


                if (erRoadCode == 0)
                {
                    lstErCodesFromPlanRoad = dbContext.PLAN_ROAD.Where(m => m.PLAN_CN_ROAD_CODE == roadCode).Select(m => m.MAST_ER_ROAD_CODE).ToList<int>();

                    lstErCodesFromPlanRoadMRL = dbContext.PLAN_ROAD_MRL_PMGSY3.Where(m => m.PLAN_CN_ROAD_CODE == roadCode).Select(m => m.MAST_ER_ROAD_CODE).ToList<int>();
                }
                else
                {
                    lstErCodesFromPlanRoad = dbContext.PLAN_ROAD.Where(m => m.MAST_ER_ROAD_CODE == erRoadCode).Select(m => m.MAST_ER_ROAD_CODE).ToList<int>();

                    lstErCodesFromPlanRoadMRL = dbContext.PLAN_ROAD_MRL_PMGSY3.Where(m => m.MAST_ER_ROAD_CODE == erRoadCode).Select(m => m.MAST_ER_ROAD_CODE).ToList<int>();

                }

                var lstFinalErCodes = lstErCodesFromPlanRoad.Union(lstErCodesFromPlanRoadMRL);


                var lstHabitations = (from item in dbContext.MASTER_VILLAGE
                                      join habitation in dbContext.MASTER_HABITATIONS on item.MAST_VILLAGE_CODE equals habitation.MAST_VILLAGE_CODE
                                      join habDetails in dbContext.MASTER_HABITATIONS_DETAILS on habitation.MAST_HAB_CODE equals habDetails.MAST_HAB_CODE
                                      join erHabs in dbContext.MAST_HAB_DETAILS_CSV_PMGSY3 on habitation.MAST_HAB_CODE equals erHabs.MAST_HAB_CSV_HAB_CODE
                                      //join erHabs in dbContext.MASTER_ER_HABITATION_ROAD_PMGSY3 on habitation.MAST_HAB_CODE equals erHabs.MAST_HAB_CODE
                                      where lstFinalErCodes.Contains(erHabs.MAST_HAB_CSV_ER_CODE) && //== erRoadCode &&
                                      habitation.MAST_HABITATION_ACTIVE == "Y" //new condition added by Vikram and above line commented as per suggestion from Dev Sir
                                      && habDetails.MAST_YEAR == 2011
                                      select new
                                      {
                                          habitation.MAST_HAB_NAME,
                                          item.MAST_VILLAGE_NAME,
                                          habDetails.MAST_HAB_TOT_POP,
                                          habitation.MAST_HAB_CODE

                                      });



                // Added on 29 May 2020
                var lstHabitations1 = (from item in dbContext.MASTER_VILLAGE
                                       join habitation in dbContext.MASTER_HABITATIONS on item.MAST_VILLAGE_CODE equals habitation.MAST_VILLAGE_CODE
                                       join habDetails in dbContext.MASTER_HABITATIONS_DETAILS on habitation.MAST_HAB_CODE equals habDetails.MAST_HAB_CODE
                                       //  join erHabs in dbContext.MAST_HAB_DETAILS_CSV_PMGSY3 on habitation.MAST_HAB_CODE equals erHabs.MAST_HAB_CSV_HAB_CODE
                                       join erHabs in dbContext.MASTER_ER_HABITATION_ROAD_PMGSY3 on habitation.MAST_HAB_CODE equals erHabs.MAST_HAB_CODE
                                       where lstFinalErCodes.Contains(erHabs.MAST_ER_ROAD_CODE) && //== erRoadCode &&
                                       habitation.MAST_HABITATION_ACTIVE == "Y"
                                       && habDetails.MAST_YEAR == 2011
                                       select new
                                       {
                                           habitation.MAST_HAB_NAME,
                                           item.MAST_VILLAGE_NAME,
                                           habDetails.MAST_HAB_TOT_POP,
                                           habitation.MAST_HAB_CODE

                                       });


                var lstHabitations2 = (from item in dbContext.MASTER_VILLAGE
                                       join habitation in dbContext.MASTER_HABITATIONS on item.MAST_VILLAGE_CODE equals habitation.MAST_VILLAGE_CODE
                                       join habDetails in dbContext.MASTER_HABITATIONS_DETAILS on habitation.MAST_HAB_CODE equals habDetails.MAST_HAB_CODE
                                       //  join erHabs in dbContext.MAST_HAB_DETAILS_CSV_PMGSY3 on habitation.MAST_HAB_CODE equals erHabs.MAST_HAB_CSV_HAB_CODE
                                       join erHabs in dbContext.MASTER_ER_HABITATION_ROAD on habitation.MAST_HAB_CODE equals erHabs.MAST_HAB_CODE
                                       where lstFinalErCodes.Contains(erHabs.MAST_ER_ROAD_CODE) && //== erRoadCode &&
                                       habitation.MAST_HABITATION_ACTIVE == "Y"
                                       && habDetails.MAST_YEAR == 2011
                                       select new
                                       {
                                           habitation.MAST_HAB_NAME,
                                           item.MAST_VILLAGE_NAME,
                                           habDetails.MAST_HAB_TOT_POP,
                                           habitation.MAST_HAB_CODE

                                       });


               lstHabitations = lstHabitations.Union(lstHabitations1.Union(lstHabitations2));


                // This If Else is added on 29 May 2020
                if (lstHabitations.Count() == 0)
                {
                    #region CODE

                    List<int> mapHabitations = (from item in dbContext.PLAN_ROAD_HABITATION_PMGSY3
                                                where
                                                item.PLAN_CN_ROAD_CODE == roadCode &&
                                                item.PLAN_ROAD.MAST_PMGSY_SCHEME == (PMGSYSession.Current.PMGSYScheme)
                                                select item.MAST_HAB_CODE).Distinct().ToList<int>();


                    var listHab1 = (from item in lstHabitations1
                                    where !mapHabitations.Contains(item.MAST_HAB_CODE)
                                    select item.MAST_HAB_CODE).Distinct();


                    //dynamic mappingList = null;

                    isHabFinalized = dbContext.PLAN_ROAD_HABITATION_PMGSY3.Where(z => z.PLAN_CN_ROAD_CODE == roadCode && z.PLAN_CN_HAB_FINALIZED == "Y").Any();
                    //TRMRLHabs      6

                    // Below Line is commented on 17 April 2020
                    //  isHabFinalized = lstBlocks.Contains(masterRoad.MAST_BLOCK_CODE) ? false : isHabFinalized;

                    isHabFinalized = (lstBlocks.Contains(masterRoad.MAST_BLOCK_CODE) || checkIsUnlocked(PMGSYSession.Current.StateCode, PMGSYSession.Current.DistrictCode, masterRoad.MAST_BLOCK_CODE) == true) ? false : isHabFinalized;


                    var route = dbContext.PLAN_ROAD.Where(x => x.PLAN_CN_ROAD_CODE == roadCode).Select(x => x.PLAN_RD_ROUTE).FirstOrDefault();




                    if (route == "N")
                    {
                        var mappingList = (from item in dbContext.MASTER_HABITATIONS

                                           join planHab in dbContext.MAST_HAB_DETAILS_CSV_PMGSY3 on item.MAST_HAB_CODE equals planHab.MAST_HAB_CSV_HAB_CODE
                                           //   join planHab in dbContext.MASTER_ER_HABITATION_ROAD_PMGSY3 on item.MAST_HAB_CODE equals planHab.MAST_HAB_CODE
                                           join habDetails1 in dbContext.MASTER_HABITATIONS_DETAILS on item.MAST_HAB_CODE equals habDetails1.MAST_HAB_CODE
                                           join planRoad in dbContext.PLAN_ROAD on planHab.MAST_HAB_CSV_ER_CODE equals planRoad.MAST_ER_ROAD_CODE
                                           where planRoad.PLAN_CN_ROAD_CODE == roadCode
                                           && item.MASTER_VILLAGE.MAST_BLOCK_CODE == masterRoad.MAST_BLOCK_CODE//blockCode
                                           && habDetails1.MAST_YEAR == (PMGSYSession.Current.PMGSYScheme == 1 ? 2001 : 2011)
                                           && item.MAST_HABITATION_ACTIVE == "Y"
                                           select new
                                           {
                                               item.MAST_HAB_CODE,
                                               item.MAST_HAB_NAME,
                                               item.MASTER_VILLAGE.MAST_VILLAGE_NAME,
                                               habDetails1.MAST_HAB_TOT_POP
                                           }).Distinct().ToList();

                        totalRecords = mappingList.Count();

                        if (sidx.Trim() != string.Empty)
                        {
                            if (sord.ToString() == "asc")
                            {
                                mappingList = mappingList.OrderBy(x => x.MAST_HAB_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                            }
                            else
                            {
                                mappingList = mappingList.OrderByDescending(x => x.MAST_HAB_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                            }
                        }
                        else
                        {
                            mappingList = mappingList.OrderBy(x => x.MAST_HAB_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                        }

                        var result = mappingList.Select(habDetails => new
                        {
                            habDetails.MAST_HAB_CODE,
                            habDetails.MAST_HAB_NAME,
                            habDetails.MAST_VILLAGE_NAME,
                            habDetails.MAST_HAB_TOT_POP
                        }).ToArray();

                        return result.Select(habDetails => new
                        {
                            id = URLEncrypt.EncryptParameters1(new string[] { "HabCode =" + habDetails.MAST_HAB_CODE.ToString().Trim() }),
                            cell = new[] {
                        
                        //habDetails.MAST_HAB_CODE.ToString(),    
                        habDetails.MAST_HAB_NAME.ToString(),
                        habDetails.MAST_VILLAGE_NAME.ToString(),
                        habDetails.MAST_HAB_TOT_POP.ToString()
                    }
                        }).ToArray();

                    }
                    else
                    {
                        var mappingList = (from item in dbContext.MASTER_HABITATIONS_DETAILS
                                           join habitation in dbContext.MASTER_HABITATIONS on item.MAST_HAB_CODE equals habitation.MAST_HAB_CODE
                                           join village in dbContext.MASTER_VILLAGE on habitation.MAST_VILLAGE_CODE equals village.MAST_VILLAGE_CODE
                                           where listHab1.Contains(item.MAST_HAB_CODE) &&
                                           item.MAST_YEAR == (PMGSYSession.Current.PMGSYScheme == 1 ? 2001 : 2011)
                                           select new
                                           {
                                               item.MAST_HAB_CODE,
                                               habitation.MAST_HAB_NAME,
                                               village.MAST_VILLAGE_NAME,
                                               item.MAST_HAB_TOT_POP
                                           }).Distinct().ToList();

                        if (habDirect.Trim() == "N")
                        {
                            #region Commented on 9 May 2020
                            MASTER_EXISTING_ROADS drrpRoad = dbContext.MASTER_EXISTING_ROADS.Find(erRoadCode);

                            mappingList = (from item in dbContext.MASTER_HABITATIONS_DETAILS
                                           join habitation in dbContext.MASTER_HABITATIONS on item.MAST_HAB_CODE equals habitation.MAST_HAB_CODE
                                           join village in dbContext.MASTER_VILLAGE on habitation.MAST_VILLAGE_CODE equals village.MAST_VILLAGE_CODE
                                           where listHab1.Contains(item.MAST_HAB_CODE) &&
                                           item.MAST_YEAR == (PMGSYSession.Current.PMGSYScheme == 1 ? 2001 : 2011)
                                           select new
                                           {
                                               item.MAST_HAB_CODE,
                                               habitation.MAST_HAB_NAME,
                                               village.MAST_VILLAGE_NAME,
                                               item.MAST_HAB_TOT_POP
                                           }).Distinct().ToList();



                            //(from item in dbContext.MASTER_HABITATIONS_DETAILS
                            //           join habitation in dbContext.MASTER_HABITATIONS on item.MAST_HAB_CODE equals habitation.MAST_HAB_CODE
                            //           join village in dbContext.MASTER_VILLAGE on habitation.MAST_VILLAGE_CODE equals village.MAST_VILLAGE_CODE
                            //           where
                            //           village.MAST_BLOCK_CODE == drrpRoad.MAST_BLOCK_CODE &&
                            //           item.MAST_YEAR == 2011
                            //           && !mapHabitations.Contains(item.MAST_HAB_CODE)

                            //           select new
                            //           {
                            //               item.MAST_HAB_CODE,
                            //               habitation.MAST_HAB_NAME,
                            //               village.MAST_VILLAGE_NAME,
                            //               item.MAST_HAB_TOT_POP
                            //           }).Distinct().ToList();
                            #endregion


                        }

                        totalRecords = mappingList.Count();

                        if (sidx.Trim() != string.Empty)
                        {
                            if (sord.ToString() == "asc")
                            {
                                mappingList = mappingList.OrderBy(x => x.MAST_HAB_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                            }
                            else
                            {
                                mappingList = mappingList.OrderByDescending(x => x.MAST_HAB_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                            }
                        }
                        else
                        {
                            mappingList = mappingList.OrderBy(x => x.MAST_HAB_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                        }

                        var result = mappingList.Select(habDetails => new
                        {
                            habDetails.MAST_HAB_CODE,
                            habDetails.MAST_HAB_NAME,
                            habDetails.MAST_VILLAGE_NAME,
                            habDetails.MAST_HAB_TOT_POP
                        }).ToArray();

                        return result.Select(habDetails => new
                        {
                            id = URLEncrypt.EncryptParameters1(new string[] { "HabCode =" + habDetails.MAST_HAB_CODE.ToString().Trim() }),
                            cell = new[] {
                        
                        //habDetails.MAST_HAB_CODE.ToString(),    
                        habDetails.MAST_HAB_NAME.ToString(),
                        habDetails.MAST_VILLAGE_NAME.ToString(),
                        habDetails.MAST_HAB_TOT_POP.ToString()
                    }
                        }).ToArray();
                    }




                    #endregion CODE Ends
                }
                else
                {
                    #region CODE

                    List<int> mapHabitations = (from item in dbContext.PLAN_ROAD_HABITATION_PMGSY3
                                                where
                                                item.PLAN_CN_ROAD_CODE == roadCode &&
                                                item.PLAN_ROAD.MAST_PMGSY_SCHEME == (PMGSYSession.Current.PMGSYScheme)
                                                select item.MAST_HAB_CODE).Distinct().ToList<int>();


                    var listHab = (from item in lstHabitations
                                   where !mapHabitations.Contains(item.MAST_HAB_CODE)
                                   select item.MAST_HAB_CODE).Distinct();


                    //dynamic mappingList = null;

                    isHabFinalized = dbContext.PLAN_ROAD_HABITATION_PMGSY3.Where(z => z.PLAN_CN_ROAD_CODE == roadCode && z.PLAN_CN_HAB_FINALIZED == "Y").Any();
                    //TRMRLHabs      6

                    // Below Line is commented on 17 April 2020
                    //  isHabFinalized = lstBlocks.Contains(masterRoad.MAST_BLOCK_CODE) ? false : isHabFinalized;

                    isHabFinalized = (lstBlocks.Contains(masterRoad.MAST_BLOCK_CODE) || checkIsUnlocked(PMGSYSession.Current.StateCode, PMGSYSession.Current.DistrictCode, masterRoad.MAST_BLOCK_CODE) == true) ? false : isHabFinalized;


                    var route = dbContext.PLAN_ROAD.Where(x => x.PLAN_CN_ROAD_CODE == roadCode).Select(x => x.PLAN_RD_ROUTE).FirstOrDefault();




                    if (route == "N")
                    {

                        //LogLog
                        var mappingList = (from item in dbContext.MASTER_HABITATIONS

                                           join planHab in dbContext.MAST_HAB_DETAILS_CSV_PMGSY3 on item.MAST_HAB_CODE equals planHab.MAST_HAB_CSV_HAB_CODE
                                           //   join planHab in dbContext.MASTER_ER_HABITATION_ROAD_PMGSY3 on item.MAST_HAB_CODE equals planHab.MAST_HAB_CODE
                                           join habDetails1 in dbContext.MASTER_HABITATIONS_DETAILS on item.MAST_HAB_CODE equals habDetails1.MAST_HAB_CODE
                                           join planRoad in dbContext.PLAN_ROAD on planHab.MAST_HAB_CSV_ER_CODE equals planRoad.MAST_ER_ROAD_CODE
                                           where planRoad.PLAN_CN_ROAD_CODE == roadCode
                                           && item.MASTER_VILLAGE.MAST_BLOCK_CODE == masterRoad.MAST_BLOCK_CODE//blockCode
                                           && habDetails1.MAST_YEAR == (PMGSYSession.Current.PMGSYScheme == 1 ? 2001 : 2011)
                                           && item.MAST_HABITATION_ACTIVE == "Y"
                                           select new
                                           {
                                               item.MAST_HAB_CODE,
                                               item.MAST_HAB_NAME,
                                               item.MASTER_VILLAGE.MAST_VILLAGE_NAME,
                                               habDetails1.MAST_HAB_TOT_POP
                                           }).Distinct().ToList();

                        using (System.IO.StreamWriter sw = System.IO.File.AppendText(System.IO.Path.Combine("D:\\ErrorLog\\OMMASErrorLog_" + System.DateTime.Now.Date.ToString("dd_MM_yyyy") + ".txt", "")))
                        {
                            sw.WriteLine("Date :" + DateTime.Now.ToString());
                            sw.WriteLine("Mapping List : When route == N");
                            foreach (var item in mappingList)
                            {
                                sw.WriteLine("MAST_HAB_CODE ::" + item.MAST_HAB_CODE);
                                sw.WriteLine("MAST_HAB_NAME ::" + item.MAST_HAB_NAME);
                                sw.WriteLine("MAST_VILLAGE_NAME ::" + item.MAST_VILLAGE_NAME);
                                sw.WriteLine("MAST_HAB_TOT_POP ::" + item.MAST_HAB_TOT_POP);
                            }
                            sw.WriteLine("---------------------------------------------------------------------------------------");
                            sw.Close();
                        }
                        totalRecords = mappingList.Count();

                        if (sidx.Trim() != string.Empty)
                        {
                            if (sord.ToString() == "asc")
                            {
                                mappingList = mappingList.OrderBy(x => x.MAST_HAB_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                            }
                            else
                            {
                                mappingList = mappingList.OrderByDescending(x => x.MAST_HAB_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                            }
                        }
                        else
                        {
                            mappingList = mappingList.OrderBy(x => x.MAST_HAB_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                        }

                        var result = mappingList.Select(habDetails => new
                        {
                            habDetails.MAST_HAB_CODE,
                            habDetails.MAST_HAB_NAME,
                            habDetails.MAST_VILLAGE_NAME,
                            habDetails.MAST_HAB_TOT_POP
                        }).ToArray();

                        return result.Select(habDetails => new
                        {
                            id = URLEncrypt.EncryptParameters1(new string[] { "HabCode =" + habDetails.MAST_HAB_CODE.ToString().Trim() }),
                            cell = new[] {
                        
                        //habDetails.MAST_HAB_CODE.ToString(),    
                        habDetails.MAST_HAB_NAME.ToString(),
                        habDetails.MAST_VILLAGE_NAME.ToString(),
                        habDetails.MAST_HAB_TOT_POP.ToString()
                    }
                        }).ToArray();

                    }
                    else
                    {


                        //
                        var mappingList = (from item in dbContext.MASTER_HABITATIONS_DETAILS
                                           join habitation in dbContext.MASTER_HABITATIONS on item.MAST_HAB_CODE equals habitation.MAST_HAB_CODE
                                           join village in dbContext.MASTER_VILLAGE on habitation.MAST_VILLAGE_CODE equals village.MAST_VILLAGE_CODE
                                           where listHab.Contains(item.MAST_HAB_CODE) &&
                                           item.MAST_YEAR == (PMGSYSession.Current.PMGSYScheme == 1 ? 2001 : 2011)
                                           select new
                                           {
                                               item.MAST_HAB_CODE,
                                               habitation.MAST_HAB_NAME,
                                               village.MAST_VILLAGE_NAME,
                                               item.MAST_HAB_TOT_POP
                                           }).Distinct().ToList();
                        using (System.IO.StreamWriter sw = System.IO.File.AppendText(System.IO.Path.Combine("D:\\ErrorLog\\OMMASErrorLog_" + System.DateTime.Now.Date.ToString("dd_MM_yyyy") + ".txt", "")))
                        {
                            sw.WriteLine("Date :" + DateTime.Now.ToString());
                            sw.WriteLine("Mapping List : When route is not N Before habDirect");
                            foreach (var item in mappingList)
                            {
                                sw.WriteLine("MAST_HAB_CODE :::" + item.MAST_HAB_CODE);
                                sw.WriteLine("MAST_HAB_NAME :::" + item.MAST_HAB_NAME);
                                sw.WriteLine("MAST_VILLAGE_NAME :::" + item.MAST_VILLAGE_NAME);
                                sw.WriteLine("MAST_HAB_TOT_POP :::" + item.MAST_HAB_TOT_POP);
                            }
                            sw.WriteLine("---------------------------------------------------------------------------------------");
                            sw.Close();
                        }
                        if (habDirect.Trim() == "N")
                        {
                            #region Commented on 9 May 2020


                            MASTER_EXISTING_ROADS drrpRoad = dbContext.MASTER_EXISTING_ROADS.Find(erRoadCode);

                            mappingList = (from item in dbContext.MASTER_HABITATIONS_DETAILS
                                           join habitation in dbContext.MASTER_HABITATIONS on item.MAST_HAB_CODE equals habitation.MAST_HAB_CODE
                                           join village in dbContext.MASTER_VILLAGE on habitation.MAST_VILLAGE_CODE equals village.MAST_VILLAGE_CODE
                                           where listHab.Contains(item.MAST_HAB_CODE) &&
                                           item.MAST_YEAR == (PMGSYSession.Current.PMGSYScheme == 1 ? 2001 : 2011)
                                           select new
                                           {
                                               item.MAST_HAB_CODE,
                                               habitation.MAST_HAB_NAME,
                                               village.MAST_VILLAGE_NAME,
                                               item.MAST_HAB_TOT_POP
                                           }).Distinct().ToList();
                            using (System.IO.StreamWriter sw = System.IO.File.AppendText(System.IO.Path.Combine("D:\\ErrorLog\\OMMASErrorLog_" + System.DateTime.Now.Date.ToString("dd_MM_yyyy") + ".txt", "")))
                            {
                                sw.WriteLine("Date :" + DateTime.Now.ToString());
                                sw.WriteLine("Mapping List : When route is not N Inside habDirect");
                                foreach (var item in mappingList)
                                {
                                    sw.WriteLine("MAST_HAB_CODE ::::" + item.MAST_HAB_CODE);
                                    sw.WriteLine("MAST_HAB_NAME ::::" + item.MAST_HAB_NAME);
                                    sw.WriteLine("MAST_VILLAGE_NAME ::::" + item.MAST_VILLAGE_NAME);
                                    sw.WriteLine("MAST_HAB_TOT_POP ::::" + item.MAST_HAB_TOT_POP);
                                }
                                sw.WriteLine("---------------------------------------------------------------------------------------");
                                sw.Close();
                            }


                            //(from item in dbContext.MASTER_HABITATIONS_DETAILS
                            //           join habitation in dbContext.MASTER_HABITATIONS on item.MAST_HAB_CODE equals habitation.MAST_HAB_CODE
                            //           join village in dbContext.MASTER_VILLAGE on habitation.MAST_VILLAGE_CODE equals village.MAST_VILLAGE_CODE
                            //           where
                            //           village.MAST_BLOCK_CODE == drrpRoad.MAST_BLOCK_CODE &&
                            //           item.MAST_YEAR == 2011
                            //           && !mapHabitations.Contains(item.MAST_HAB_CODE)

                            //           select new
                            //           {
                            //               item.MAST_HAB_CODE,
                            //               habitation.MAST_HAB_NAME,
                            //               village.MAST_VILLAGE_NAME,
                            //               item.MAST_HAB_TOT_POP
                            //           }).Distinct().ToList();
                            #endregion


                        }

                        totalRecords = mappingList.Count();

                        if (sidx.Trim() != string.Empty)
                        {
                            if (sord.ToString() == "asc")
                            {
                                mappingList = mappingList.OrderBy(x => x.MAST_HAB_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                            }
                            else
                            {
                                mappingList = mappingList.OrderByDescending(x => x.MAST_HAB_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                            }
                        }
                        else
                        {
                            mappingList = mappingList.OrderBy(x => x.MAST_HAB_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                        }

                        var result = mappingList.Select(habDetails => new
                        {
                            habDetails.MAST_HAB_CODE,
                            habDetails.MAST_HAB_NAME,
                            habDetails.MAST_VILLAGE_NAME,
                            habDetails.MAST_HAB_TOT_POP
                        }).ToArray();

                        return result.Select(habDetails => new
                        {
                            id = URLEncrypt.EncryptParameters1(new string[] { "HabCode =" + habDetails.MAST_HAB_CODE.ToString().Trim() }),
                            cell = new[] {
                        
                        //habDetails.MAST_HAB_CODE.ToString(),    
                        habDetails.MAST_HAB_NAME.ToString(),
                        habDetails.MAST_VILLAGE_NAME.ToString(),
                        habDetails.MAST_HAB_TOT_POP.ToString()
                    }
                        }).ToArray();
                    }




                    #endregion CODE Ends
                }



            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "GetHabitationListToMapPMGSY3DAL()");
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






        public bool MapHabitationToRoadPMGSY3DAL(string encryptedHabCodes, string roadName, string habDirect)
        {
            PMGSYEntities dbContext = new PMGSYEntities();
            try
            {
                String[] habCodes = null;
                int roadCode = Convert.ToInt32(roadName);
                int habCode = 0;
                int habId = 0;
                habCodes = encryptedHabCodes.Split(',');
                if (habCodes.Count() == 0)
                {
                    return false;
                }
                using (TransactionScope ts = new TransactionScope())
                {
                    dbContext.Configuration.AutoDetectChangesEnabled = false;
                    habId = dbContext.PLAN_ROAD_HABITATION_PMGSY3.Any() ? (from item1 in dbContext.PLAN_ROAD_HABITATION_PMGSY3 select item1.PLAN_CN_ROAD_HAB_ID).Max() + 1 : 1;
                    foreach (String item in habCodes)
                    {
                        encryptedParameters = null;
                        encryptedParameters = item.Split('/');
                        decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { encryptedParameters[0], encryptedParameters[1], encryptedParameters[2] });
                        habCode = Convert.ToInt32(decryptedParameters["HabCode"].ToString());

                        PLAN_ROAD_HABITATION_PMGSY3 master = new PLAN_ROAD_HABITATION_PMGSY3();
                        master.PLAN_CN_ROAD_HAB_ID = habId;
                        master.PLAN_CN_ROAD_CODE = roadCode;
                        master.MAST_HAB_CODE = habCode;

                        master.MAST_HAB_CODE_DIRECT = habDirect.Trim();
                        master.MAST_HAB_CODE_VERIFIED = "N";

                        master.USERID = PMGSYSession.Current.UserId;
                        master.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                        dbContext.PLAN_ROAD_HABITATION_PMGSY3.Add(master);
                        habId++;
                    }
                    dbContext.SaveChanges();
                    dbContext.Configuration.AutoDetectChangesEnabled = true;
                    ts.Complete();
                }
                return true;
            }
            catch (OptimisticConcurrencyException ex)
            {
                dbContext.Configuration.AutoDetectChangesEnabled = true;
                ErrorLog.LogError(ex, "MapHabitationToRoadPMGSY3DAL().OptimisticConcurrencyException");
                return false;
            }
            catch (UpdateException ex)
            {
                dbContext.Configuration.AutoDetectChangesEnabled = true;
                ErrorLog.LogError(ex, "MapHabitationToRoadPMGSY3DAL().UpdateException");
                return false;
            }
            catch (Exception ex)
            {
                dbContext.Configuration.AutoDetectChangesEnabled = true;
                ErrorLog.LogError(ex, "MapHabitationToRoadPMGSY3DAL()");
                return false;
            }
            finally
            {
                dbContext.Configuration.AutoDetectChangesEnabled = true;
                if (dbContext != null)
                {
                    dbContext.Dispose();
                }
            }
        }

        public bool DeleteMapHabitationPMGSY3DAL(int habitationCode, string flag, int roadCode)
        {
            PMGSYEntities dbContext = new PMGSYEntities();
            try
            {
                ///Changed by SAMMED A. PATIL on 24 APRIL 2017 to skip validation for PMGSY-II
                if (dbContext.IMS_BENEFITED_HABS.Any(m => m.MAST_HAB_CODE == habitationCode && m.IMS_SANCTIONED_PROJECTS.PLAN_CN_ROAD_CODE == roadCode && m.IMS_SANCTIONED_PROJECTS.MAST_PMGSY_SCHEME == PMGSYSession.Current.PMGSYScheme))
                {
                    return false;
                }

                // PLAN_ROAD_HABITATION_PMGSY3 master = dbContext.PLAN_ROAD_HABITATION_PMGSY3.Where(m => m.MAST_HAB_CODE == habitationCode && m.PLAN_CN_ROAD_CODE == roadCode).FirstOrDefault();

                // dbContext.PLAN_ROAD_HABITATION_PMGSY3.Remove(master);
                // dbContext.SaveChanges();

                //return true;


                List<PLAN_ROAD_HABITATION_PMGSY3> master1 = new List<PLAN_ROAD_HABITATION_PMGSY3>();

                master1 = dbContext.PLAN_ROAD_HABITATION_PMGSY3.Where(m => m.MAST_HAB_CODE == habitationCode && m.PLAN_CN_ROAD_CODE == roadCode).ToList();

                dbContext.PLAN_ROAD_HABITATION_PMGSY3.DeleteMany(master1);
                dbContext.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "DeleteMapHabitationPMGSY3DAL()");
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

        //Map DRRP to CN
        public Array ListCandidateRoadsPMGSY3DAL(int roadCode, int page, int rows, string sidx, string sord, out long totalRecords, out string IsFinalized)
        {
            int erRoadCode = 0;
            PMGSYEntities dbContext = new PMGSYEntities();
            try
            {
                var lstDRRP = dbContext.PLAN_ROAD.Where(c => c.PLAN_CN_ROAD_CODE == roadCode).Select(x => new
                {
                    x.MASTER_BLOCK.MAST_BLOCK_NAME,
                    x.MASTER_EXISTING_ROADS.MASTER_ROAD_CATEGORY.MAST_ROAD_CAT_NAME,
                    x.MASTER_EXISTING_ROADS.MAST_ER_ROAD_NAME,
                    x.PLAN_RD_LENGTH,
                    x.PLAN_RD_LENG,
                    x.MAST_ER_ROAD_CODE,
                    x.PLAN_LOCK_STATUS,
                    x.PLAN_CN_ROAD_CODE,
                    x.MASTER_EXISTING_ROADS.MAST_ER_ROAD_NUMBER
                }).ToList();
                erRoadCode = lstDRRP.Where(z => z.PLAN_CN_ROAD_CODE == roadCode).Select(x => x.MAST_ER_ROAD_CODE).FirstOrDefault();

                var lstMappedRoads = (from item in dbContext.PLAN_ROAD_MRL_PMGSY3
                                      where item.PLAN_CN_ROAD_CODE == roadCode
                                      select new
                                      {
                                          item.MASTER_EXISTING_ROADS.MASTER_BLOCK.MAST_BLOCK_NAME,
                                          item.MASTER_EXISTING_ROADS.MASTER_ROAD_CATEGORY.MAST_ROAD_CAT_NAME,
                                          item.MASTER_EXISTING_ROADS.MAST_ER_ROAD_NAME,
                                          item.PLAN_RD_LENGTH,
                                          item.PLAN_RD_LENG,
                                          item.MAST_ER_ROAD_CODE,
                                          item.PLAN_LOCK_STATUS,
                                          item.PLAN_CN_ROAD_CODE,
                                          item.MASTER_EXISTING_ROADS.MAST_ER_ROAD_NUMBER
                                      }).ToList().Union(lstDRRP);

                totalRecords = lstMappedRoads.Count();

                IsFinalized = String.Empty;
                if (dbContext.PLAN_ROAD_MRL_PMGSY3.Any(m => m.PLAN_CN_ROAD_CODE == roadCode && m.PLAN_LOCK_STATUS == "N"))
                {
                    IsFinalized = "N";
                }
                else if (dbContext.PLAN_ROAD_MRL_PMGSY3.Any(m => m.PLAN_CN_ROAD_CODE == roadCode && m.PLAN_LOCK_STATUS == "Y"))
                {
                    IsFinalized = "Y";
                }

                if (sidx.Trim() != string.Empty)
                {
                    if (sord.ToString() == "asc")
                    {
                        switch (sidx)
                        {
                            case "MAST_BLOCK_NAME":
                                lstMappedRoads = lstMappedRoads.OrderBy(x => x.MAST_BLOCK_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                            case "MAST_ROAD_CAT_NAME":
                                lstMappedRoads = lstMappedRoads.OrderBy(x => x.MAST_ROAD_CAT_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                            case "MAST_ER_ROAD_NAME":
                                lstMappedRoads = lstMappedRoads.OrderBy(x => x.MAST_ER_ROAD_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                            case "PLAN_RD_LENGTH":
                                lstMappedRoads = lstMappedRoads.OrderBy(x => x.PLAN_RD_LENGTH).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                            case "PLAN_RD_LENG":
                                lstMappedRoads = lstMappedRoads.OrderBy(x => x.PLAN_RD_LENG).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                            default:
                                lstMappedRoads = lstMappedRoads.OrderBy(x => x.MAST_ER_ROAD_CODE).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                        }
                    }
                    else
                    {
                        switch (sidx)
                        {
                            case "MAST_BLOCK_NAME":
                                lstMappedRoads = lstMappedRoads.OrderByDescending(x => x.MAST_BLOCK_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                            case "MAST_ROAD_CAT_NAME":
                                lstMappedRoads = lstMappedRoads.OrderByDescending(x => x.MAST_ROAD_CAT_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                            case "MAST_ER_ROAD_NAME":
                                lstMappedRoads = lstMappedRoads.OrderByDescending(x => x.MAST_ER_ROAD_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                            case "PLAN_RD_LENGTH":
                                lstMappedRoads = lstMappedRoads.OrderByDescending(x => x.PLAN_RD_LENGTH).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                            case "PLAN_RD_LENG":
                                lstMappedRoads = lstMappedRoads.OrderByDescending(x => x.PLAN_RD_LENG).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                            default:
                                lstMappedRoads = lstMappedRoads.OrderByDescending(x => x.MAST_ER_ROAD_CODE).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                        }
                    }
                }
                else
                {
                    lstMappedRoads = lstMappedRoads.OrderByDescending(x => x.MAST_ER_ROAD_CODE).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                }

                return lstMappedRoads.Select(roadDetails => new
                {
                    cell = new[]
                        {
                            roadDetails.MAST_BLOCK_NAME == null?"-":roadDetails.MAST_BLOCK_NAME.ToString(),
                            roadDetails.MAST_ROAD_CAT_NAME == null?"-":roadDetails.MAST_ROAD_CAT_NAME.ToString(),
                            roadDetails.MAST_ER_ROAD_NAME == null?"-":(roadDetails.MAST_ER_ROAD_NUMBER.ToString() +" - "+ roadDetails.MAST_ER_ROAD_NAME.ToString()),
                            roadDetails.PLAN_RD_LENGTH == null?"0":roadDetails.PLAN_RD_LENGTH.ToString(),
                            roadDetails.PLAN_RD_LENG == null?"-":(roadDetails.PLAN_RD_LENG == "P"?"Partial":"Full"),
                            roadDetails.PLAN_LOCK_STATUS == "Y"?"<span class='ui-icon ui-icon-locked ui-align-center'></span>":"<a href='#' title='Click here to edit TR/MRL road details' class='ui-icon ui-icon-pencil ui-align-center' onClick =EditCandidateRoad('"+URLEncrypt.EncryptParameters1(new string[]{"DRRPCode =" + roadDetails.MAST_ER_ROAD_CODE.ToString().Trim(),"CNCode ="+roadDetails.PLAN_CN_ROAD_CODE})+"'); return false;'>Edit</a>",
                            roadDetails.MAST_ER_ROAD_CODE == erRoadCode ? "-" :
                            roadDetails.PLAN_LOCK_STATUS == "Y"?"<span class='ui-icon ui-icon-locked ui-align-center'></span>":"<a href='#' title='Click here to delete TR/MRL road details' class='ui-icon ui-icon-trash ui-align-center' onClick =DeleteCandidateRoad('"+URLEncrypt.EncryptParameters1(new string[]{"DRRPCode =" + roadDetails.MAST_ER_ROAD_CODE.ToString().Trim(),"CNCode ="+roadDetails.PLAN_CN_ROAD_CODE})+"'); return false;'>Delete</a>",
                            roadDetails.PLAN_LOCK_STATUS == "Y"?"<span class='ui-icon ui-icon-locked ui-align-center'></span>":"<a href='#' title='Click here to Map/Delete Habitation details' class='ui-icon ui-icon-plusthick ui-align-center' onClick =MapOrDeleteHabitations('"+URLEncrypt.EncryptParameters1(new string[]{"DRRPCode =" + roadDetails.MAST_ER_ROAD_CODE.ToString().Trim(),"CNCode ="+roadDetails.PLAN_CN_ROAD_CODE})+"'); return false;'>Delete</a>",
                        }
                }).ToArray();

            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "ListCandidateRoadsPMGSY3DAL()");
                totalRecords = 0;
                IsFinalized = String.Empty;
                return null;
            }
            finally
            {
                dbContext.Dispose();
            }
        }

        public bool MapCandidateRoadPMGSY3DAL(CandidateRoadViewModel model, ref string message)
        {
            PMGSYEntities dbContext = new PMGSYEntities();
            int CNHabId = 0;
            try
            {
                //using (TransactionScope ts = new TransactionScope())
                //{

                    //check whether the road is already mapped with the same Candidate Road or not
                    if (dbContext.PLAN_ROAD_MRL_PMGSY3.Any(m => m.PLAN_CN_ROAD_CODE == model.CNCode && m.MAST_ER_ROAD_CODE == model.DRRPCode))
                    {
                        message = "DRRP is already mapped with the TR/MRL Road.";
                        return false;
                    }

                    //check whether the DRRP on which TR/MRL  road is made is mapped with the same TR/MRL Road
                    if (dbContext.PLAN_ROAD.Any(m => m.PLAN_CN_ROAD_CODE == model.CNCode && m.MAST_ER_ROAD_CODE == model.DRRPCode && m.MAST_PMGSY_SCHEME == PMGSYSession.Current.PMGSYScheme))
                    {
                        message = "DRRP on which the TR/MRL road is made can not be mapped with the same TR/MRL road.";
                        return false;
                    }

                    //validation for checking the Total TR/MRL Road length with the mapped road length
                    //decimal? totalCandidateRoadLength = dbContext.PLAN_ROAD.Where(m=>m.PLAN_CN_ROAD_CODE == model.CNCode).Select(m=>m.PLAN_RD_TOTAL_LEN).FirstOrDefault();
                    PLAN_ROAD roadMaster = dbContext.PLAN_ROAD.Where(m => m.PLAN_CN_ROAD_CODE == model.CNCode).FirstOrDefault();
                    decimal? totalMappedRoadLength = 0;
                    if (dbContext.PLAN_ROAD_MRL_PMGSY3.Any(m => m.PLAN_CN_ROAD_CODE == model.CNCode))
                    {
                        totalMappedRoadLength = dbContext.PLAN_ROAD_MRL_PMGSY3.Where(m => m.PLAN_CN_ROAD_CODE == model.CNCode).Sum(m => m.PLAN_RD_LENGTH);
                        if (totalMappedRoadLength == null)
                        {
                            totalMappedRoadLength = 0;
                        }
                        totalMappedRoadLength = totalMappedRoadLength + model.LengthOfRoad + roadMaster.PLAN_RD_LENGTH;
                    }
                    else
                    {
                        totalMappedRoadLength = model.LengthOfRoad + roadMaster.PLAN_RD_LENGTH;
                    }

                    if (roadMaster.PLAN_RD_TOTAL_LEN < totalMappedRoadLength)
                    {
                        ///Changes for PMGSY3
                        message = "Total of mapped DRRP road length exceeding the total length of TR/MRL Road.";
                        return false;
                    }


                    PLAN_ROAD_MRL_PMGSY3 mappingMaster = new PLAN_ROAD_MRL_PMGSY3();
                    mappingMaster.PLAN_CN_ROAD_CODE = model.CNCode;
                    string PlanCNRoadCode = Convert.ToString(mappingMaster.PLAN_CN_ROAD_CODE);
                    mappingMaster.MAST_ER_ROAD_CODE = model.DRRPCode;
                    mappingMaster.PLAN_LOCK_STATUS = "N";
                    mappingMaster.PLAN_RD_FROM_CHAINAGE = model.StartChainage;
                    mappingMaster.PLAN_RD_TO_CHAINAGE = model.EndChainage;
                    mappingMaster.PLAN_RD_LENG = model.LengthTypeOfRoad;
                    mappingMaster.PLAN_RD_LENGTH = model.LengthOfRoad;
                    mappingMaster.USERID = PMGSYSession.Current.UserId;
                    mappingMaster.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];


                    ObjectParameter outputParam = new ObjectParameter("OutputParameter", 0);
                    dbContext.USP_INSERT_PLAN_ROAD_MRL_PMGSY3(PlanCNRoadCode, mappingMaster.MAST_ER_ROAD_CODE, mappingMaster.PLAN_RD_LENG, mappingMaster.PLAN_RD_FROM_CHAINAGE, mappingMaster.PLAN_RD_TO_CHAINAGE, mappingMaster.PLAN_RD_LENGTH, mappingMaster.PLAN_LOCK_STATUS, mappingMaster.USERID, mappingMaster.IPADD, outputParam);
                    if (outputParam.Value.Equals(1))
                    {
                        message = "DRRP Road mapped successfully";
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                
              



                    //dbContext.PLAN_ROAD_MRL_PMGSY3.Add(mappingMaster);
                   // dbContext.SaveChanges();


                    #region OLD Logic of Habitation Mapping. Commented on 09 May 2020

                    //var lstDRRPHabs = dbContext.MASTER_ER_HABITATION_ROAD_PMGSY3.Where(m => m.MAST_ER_ROAD_CODE == model.DRRPCode).ToList();
                    //CNHabId = (dbContext.PLAN_ROAD_HABITATION_PMGSY3.Any() ? dbContext.PLAN_ROAD_HABITATION_PMGSY3.Max(m => m.PLAN_CN_ROAD_HAB_ID) + 1 : 1);

                    //foreach (var item in lstDRRPHabs)
                    //{
                    //    if (!dbContext.PLAN_ROAD_HABITATION_PMGSY3.Any(m => m.PLAN_CN_ROAD_CODE == model.CNCode && m.MAST_HAB_CODE == item.MAST_HAB_CODE))
                    //    {
                    //        PLAN_ROAD_HABITATION_PMGSY3 cnHabMapping = new PLAN_ROAD_HABITATION_PMGSY3();
                    //        cnHabMapping.MAST_HAB_CODE = item.MAST_HAB_CODE;
                    //        cnHabMapping.PLAN_CN_ROAD_CODE = model.CNCode;

                    //        cnHabMapping.MAST_HAB_CODE_DIRECT = item.MAST_HAB_CODE_DIRECT;
                    //        cnHabMapping.MAST_HAB_CODE_VERIFIED = item.MAST_HAB_CODE_VERIFIED;

                    //        cnHabMapping.PLAN_CN_ROAD_HAB_ID = CNHabId;
                    //        cnHabMapping.USERID = PMGSYSession.Current.UserId;
                    //        cnHabMapping.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                    //        dbContext.PLAN_ROAD_HABITATION_PMGSY3.Add(cnHabMapping);
                    //        CNHabId++;
                    //    }

                    //}
                    #endregion

                    #region New Logic of Habitation Mapping. Developed on 09 May 2020

                   

                    //var lstDRRPHabs = dbContext.MAST_HAB_DETAILS_CSV_PMGSY3.Where(m => m.MAST_HAB_CSV_ER_CODE == model.DRRPCode).ToList();
                    //CNHabId = (dbContext.PLAN_ROAD_HABITATION_PMGSY3.Any() ? dbContext.PLAN_ROAD_HABITATION_PMGSY3.Max(m => m.PLAN_CN_ROAD_HAB_ID) + 1 : 1);

                    //foreach (var item in lstDRRPHabs)
                    //{
                    //    if (!dbContext.PLAN_ROAD_HABITATION_PMGSY3.Any(m => m.PLAN_CN_ROAD_CODE == model.CNCode && m.MAST_HAB_CODE == item.MAST_HAB_CSV_HAB_CODE))
                    //    {
                    //        PLAN_ROAD_HABITATION_PMGSY3 cnHabMapping = new PLAN_ROAD_HABITATION_PMGSY3();
                    //        cnHabMapping.MAST_HAB_CODE = item.MAST_HAB_CSV_HAB_CODE;
                    //        cnHabMapping.PLAN_CN_ROAD_CODE = model.CNCode;

                    //        cnHabMapping.MAST_HAB_CODE_DIRECT = "Y";
                    //        cnHabMapping.MAST_HAB_CODE_VERIFIED = "Y";

                    //        cnHabMapping.PLAN_CN_ROAD_HAB_ID = CNHabId;
                    //        cnHabMapping.USERID = PMGSYSession.Current.UserId;
                    //        cnHabMapping.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                    //        dbContext.PLAN_ROAD_HABITATION_PMGSY3.Add(cnHabMapping);

                    //        using (System.IO.StreamWriter sw = System.IO.File.AppendText(System.IO.Path.Combine("D:\\ErrorLog\\OMMASErrorLog_" + System.DateTime.Now.Date.ToString("dd_MM_yyyy") + ".txt", "")))
                    //        {
                    //            sw.WriteLine("Date :" + DateTime.Now.ToString());
                    //            sw.WriteLine("Method : " + "MapCandidateRoadPMGSY3DAL()");

                    //            sw.WriteLine("cnHabMapping.MAST_HAB_CODE :" + cnHabMapping.MAST_HAB_CODE);
                    //            sw.WriteLine("cnHabMapping.PLAN_CN_ROAD_CODE :" + cnHabMapping.PLAN_CN_ROAD_CODE);
                    //            sw.WriteLine("cnHabMapping.PLAN_CN_ROAD_HAB_ID :" + cnHabMapping.PLAN_CN_ROAD_HAB_ID);
                    //            sw.WriteLine("cnHabMapping.USERID :" + cnHabMapping.USERID);



                    //            sw.WriteLine("---------------------------------------------------------------------------------------");
                    //            sw.Close();
                    //        }

                    //        CNHabId++;

                    //    }
                    //}

                    //dbContext.SaveChanges();
                    
                    #endregion


                //    ts.Complete();
                //}

               
            }
            catch (Exception ex)
            {

                //   dbContext.Configuration.AutoDetectChangesEnabled = true;
                ErrorLog.LogError(ex, "MapCandidateRoad.DAL()");
                return false;
            }
            finally
            {
                // dbContext.Configuration.AutoDetectChangesEnabled = true;
                dbContext.Dispose();
            }
        }


        public bool DeleteMappedDRRPDetailsPMGSY3DAL(int DRRPCode, int CNCode)
        {
            PMGSYEntities dbContext = new PMGSYEntities();
            try
            {
                using (TransactionScope ts = new TransactionScope())
                {
                    PLAN_ROAD_MRL_PMGSY3 mappedMaster = dbContext.PLAN_ROAD_MRL_PMGSY3.Where(m => m.PLAN_CN_ROAD_CODE == CNCode && m.MAST_ER_ROAD_CODE == DRRPCode).FirstOrDefault();
                    if (mappedMaster != null)
                    {

                        var mappedAllHabitations = dbContext.PLAN_ROAD_HABITATION_PMGSY3.Where(m => m.PLAN_CN_ROAD_CODE == CNCode).Select(m => m.MAST_HAB_CODE).ToList<int>();

                        var mappedHabitationsAgainstDRRPTakenFromCSV = dbContext.MAST_HAB_DETAILS_CSV_PMGSY3.Where(m => m.MAST_HAB_CSV_ER_CODE == DRRPCode).Select(m => m.MAST_HAB_CSV_HAB_CODE).ToList<int>();

                        var habsToRemove = mappedAllHabitations.Intersect(mappedHabitationsAgainstDRRPTakenFromCSV).ToList<int>();


                        if (habsToRemove != null)
                        {
                            foreach (var item in habsToRemove)
                            {
                                PLAN_ROAD_HABITATION_PMGSY3 cnHabMapping = dbContext.PLAN_ROAD_HABITATION_PMGSY3.Where(m => m.PLAN_CN_ROAD_CODE == CNCode && m.MAST_HAB_CODE == item).FirstOrDefault();
                                if (cnHabMapping != null)
                                {
                                    dbContext.PLAN_ROAD_HABITATION_PMGSY3.Remove(cnHabMapping);
                                    dbContext.SaveChanges();
                                }
                            }
                        }

                        dbContext.PLAN_ROAD_MRL_PMGSY3.Remove(mappedMaster);
                        dbContext.SaveChanges();



                        //var lstHabsPlanRoad = (from item in dbContext.PLAN_ROAD
                        //                       join
                        //                       erHab in dbContext.MAST_HAB_DETAILS_CSV_PMGSY3
                        //                       on item.MAST_ER_ROAD_CODE equals erHab.MAST_HAB_CSV_ER_CODE
                        //                       where
                        //                       item.MAST_ER_ROAD_CODE != DRRPCode &&
                        //                       item.PLAN_CN_ROAD_CODE == CNCode
                        //                       select new
                        //                       {
                        //                           erHab.MAST_HAB_CSV_HAB_CODE
                        //                       }).Distinct().GroupBy(m => m.MAST_HAB_CSV_HAB_CODE).Select(m => m.FirstOrDefault()).ToList();


                        //removing the mapped habitations of current DRRP
                        // var drrpMappedHabs = dbContext.MASTER_ER_HABITATION_ROAD_PMGSY3.Where(m => m.MAST_ER_ROAD_CODE == DRRPCode && m.MASTER_EXISTING_ROADS.MAST_PMGSY_SCHEME == 2).Select(m => m.MAST_HAB_CODE).Distinct().ToList();



                        //#region Commented on 26 June 2020
                        //var drrpMappedHabs = dbContext.MAST_HAB_DETAILS_CSV_PMGSY3.Where(m => m.MAST_HAB_CSV_ER_CODE == DRRPCode).Select(m => m.MAST_HAB_CSV_HAB_CODE).Distinct().ToList();

                        //var mappedDRRPHabList = (from item in dbContext.PLAN_ROAD_MRL_PMGSY3
                        //                         join
                        //                         erHab in dbContext.MAST_HAB_DETAILS_CSV_PMGSY3
                        //                         on item.MAST_ER_ROAD_CODE equals erHab.MAST_HAB_CSV_ER_CODE
                        //                         where
                        //                         item.MAST_ER_ROAD_CODE != DRRPCode &&
                        //                         item.PLAN_CN_ROAD_CODE == CNCode
                        //                         select new
                        //                         {
                        //                             erHab.MAST_HAB_CSV_HAB_CODE
                        //                         }).Distinct().GroupBy(m => m.MAST_HAB_CSV_HAB_CODE).Select(m => m.FirstOrDefault()).ToList();


                        //var lstHabsPlanRoad = (from item in dbContext.PLAN_ROAD
                        //                       join
                        //                       erHab in dbContext.MAST_HAB_DETAILS_CSV_PMGSY3
                        //                       on item.MAST_ER_ROAD_CODE equals erHab.MAST_HAB_CSV_ER_CODE
                        //                       where
                        //                       item.MAST_ER_ROAD_CODE != DRRPCode &&
                        //                       item.PLAN_CN_ROAD_CODE == CNCode
                        //                       select new
                        //                       {
                        //                           erHab.MAST_HAB_CSV_HAB_CODE
                        //                       }).Distinct().GroupBy(m => m.MAST_HAB_CSV_HAB_CODE).Select(m => m.FirstOrDefault()).ToList();

                        //mappedDRRPHabList = mappedDRRPHabList.Union(lstHabsPlanRoad).ToList();

                        //List<int> lstSameHabs = null;

                        //if (mappedDRRPHabList.Any(m => drrpMappedHabs.Contains(m.MAST_HAB_CSV_HAB_CODE)))
                        //{
                        //    lstSameHabs = mappedDRRPHabList.Where(m => drrpMappedHabs.Contains(m.MAST_HAB_CSV_HAB_CODE)).Select(m => m.MAST_HAB_CSV_HAB_CODE).ToList();
                        //}

                        //var habsToRemove = (lstSameHabs == null ? drrpMappedHabs : drrpMappedHabs.Where(m => !lstSameHabs.Contains(m)).ToList());

                        //if (habsToRemove != null)
                        //{
                        //    foreach (var item in habsToRemove)
                        //    {
                        //        PLAN_ROAD_HABITATION_PMGSY3 cnHabMapping = dbContext.PLAN_ROAD_HABITATION_PMGSY3.Where(m => m.PLAN_CN_ROAD_CODE == CNCode && m.MAST_HAB_CODE == item).FirstOrDefault();
                        //        if (cnHabMapping != null)
                        //        {
                        //            dbContext.PLAN_ROAD_HABITATION_PMGSY3.Remove(cnHabMapping);
                        //            dbContext.SaveChanges();
                        //        }
                        //    }
                        //}
        #endregion Commented on 26 June 2020
                        ts.Complete();
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "DeleteMappedDRRPDetailsPMGSY3DAL()");
                return false;
            }
            finally
            {
                dbContext.Dispose();
            }
        }


        public bool FinalizeMappedDRRPDetailsPMGSY3DAL(int CNCode, ref string message)
        {
            PMGSYEntities dbContext = new PMGSYEntities();
            try
            {
                PLAN_ROAD planRoad = dbContext.PLAN_ROAD.Find(CNCode);
                decimal? totalMappedRoadLength = 0;
                if (dbContext.PLAN_ROAD_MRL_PMGSY3.Any(m => m.PLAN_CN_ROAD_CODE == CNCode))
                {
                    totalMappedRoadLength = dbContext.PLAN_ROAD_MRL_PMGSY3.Where(m => m.PLAN_CN_ROAD_CODE == CNCode).Sum(m => m.PLAN_RD_LENGTH);
                    if (totalMappedRoadLength == null)
                    {
                        totalMappedRoadLength = 0;
                    }
                    totalMappedRoadLength = totalMappedRoadLength + planRoad.PLAN_RD_LENGTH;
                }
                else
                {
                    totalMappedRoadLength = planRoad.PLAN_RD_LENGTH;
                }
                string[] totLength = Convert.ToString(planRoad.PLAN_RD_TOTAL_LEN).Split('.');
                string[] mappedLength = Convert.ToString(totalMappedRoadLength).Split('.');


                //if (planRoad.PLAN_RD_LENG == "F" ? Convert.ToInt32(totLength[0]) == Convert.ToInt32(mappedLength[0]) : Convert.ToInt32(totLength[0]) <= Convert.ToInt32(mappedLength[0]))
                if (Convert.ToInt32(totLength[0]) == Convert.ToInt32(mappedLength[0]))
                {
                    var lstMappedDetails = dbContext.PLAN_ROAD_MRL_PMGSY3.Where(m => m.PLAN_CN_ROAD_CODE == CNCode).ToList();
                    if (lstMappedDetails != null)
                    {
                        lstMappedDetails.ForEach(m => m.PLAN_LOCK_STATUS = "Y");
                        dbContext.SaveChanges();
                        message = "DRRP details finalized succesfully.";
                        return true;
                    }
                    else
                    {
                        message = "DRRP details could not be finalized.";
                        return false;
                    }
                }
                else
                {
                    message = "Sum of all mapped DRRP roads should be equal to total length of the TR/MRL road.";
                    return false;
                }
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "FinalizeMappedDRRPDetailsPMGSY3DAL()");
                message = "Error occurred while mapping DRRP road";
                return false;
            }
            finally
            {
                dbContext.Dispose();
            }
        }

        public bool DeFinalizeMappedDRRPDetailsPMGSY3DAL(int CNCode)
        {
            PMGSYEntities dbContext = new PMGSYEntities();
            try
            {
                var lstMappedDetails = dbContext.PLAN_ROAD_MRL_PMGSY3.Where(m => m.PLAN_CN_ROAD_CODE == CNCode).ToList();
                if (lstMappedDetails != null)
                {
                    lstMappedDetails.ForEach(m => m.PLAN_LOCK_STATUS = "N");
                    dbContext.SaveChanges();
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "DeFinalizeMappedDRRPDetailsPMGSY3DAL()");
                return false;
            }
            finally
            {
                dbContext.Dispose();
            }
        }

        public string GetCoreNetworkChecksPMGSY3DAL(int roadCode)
        {
            PMGSYEntities dbContext = new PMGSYEntities();
            try
            {
                decimal? TotalCandidateRoadLength = dbContext.PLAN_ROAD.Where(m => m.PLAN_CN_ROAD_CODE == roadCode).Select(m => m.PLAN_RD_TOTAL_LEN).FirstOrDefault();
                decimal? TotalDRRPRoadLength = dbContext.PLAN_ROAD_MRL_PMGSY3.Where(m => m.PLAN_CN_ROAD_CODE == roadCode).Sum(m => m.PLAN_RD_LENGTH);
                decimal? CandidateRoadLength = dbContext.PLAN_ROAD.Where(m => m.PLAN_CN_ROAD_CODE == roadCode).Select(m => m.PLAN_RD_LENGTH).FirstOrDefault();

                decimal Length1 = TotalDRRPRoadLength == null ? 0 : Convert.ToDecimal(TotalDRRPRoadLength);
                decimal Length2 = CandidateRoadLength == null ? 0 : Convert.ToDecimal(CandidateRoadLength);


                decimal? FinalRoadLengthToCompare = Length1 + Length2;

                if (TotalCandidateRoadLength != FinalRoadLengthToCompare)
                {
                    return "Entered TR / MRL Road Length is not equal to Sum of Lengths of all the mapped DRRP Roads.";
                }



                //Allow finalization of CN without habitation finalization
                if (dbContext.PLAN_ROAD.Where(a => a.PLAN_CN_ROAD_CODE == roadCode && a.PLAN_RD_ROUTE == "N").Any())
                {
                    if (dbContext.PLAN_ROAD_HABITATION_PMGSY3.Where(a => a.PLAN_CN_ROAD_CODE == roadCode).Count() >= 2)
                        return "";
                    else
                        return "Map atleast 2 Habitations for Finalization of Missing Link";
                }
                // Check whether habitations are added 
                if (!dbContext.PLAN_ROAD_HABITATION_PMGSY3.Where(a => a.PLAN_CN_ROAD_CODE == roadCode).Any())
                {
                    return "Habitation Details are not added against TR/MRL Road, Please Add Habitation Details.";
                }
                //if (!dbContext.PLAN_ROAD_UPLOAD_FILE.Where(a => a.PLAN_CN_ROAD_CODE == roadCode).Any())
                //{
                //    return "File Details are not added against Core Network, Please Add File Details.";
                //}

                //Check if DRRP finalized
                if (dbContext.PLAN_ROAD_MRL_PMGSY3.Where(a => a.PLAN_CN_ROAD_CODE == roadCode && a.PLAN_LOCK_STATUS == "N").Any())
                {
                    return "Mapped DRRP road details are not finalized.";
                }
                //Check if Habitations are finalised
                if (!dbContext.PLAN_ROAD_HABITATION_PMGSY3.Where(a => a.PLAN_CN_ROAD_CODE == roadCode && a.PLAN_CN_HAB_FINALIZED == "Y").Any())
                {
                    return "All Habitations are not finalized against TR/MRL Road, Please fnalize Habitation Details.";
                }
                return string.Empty;
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "GetCoreNetworkChecksPMGSY3DAL()");
                return "An Error Occurred While Your Processing Request.";
            }
            finally
            {
                dbContext.Dispose();
            }
        }

        public List<string> GetCNDetailsPMGSY3DAL(int CNCode)
        {
            PMGSYEntities dbContext = new PMGSYEntities();
            List<string> lst = new List<string>();
            try
            {
                var query = dbContext.PLAN_ROAD.Where(z => z.PLAN_CN_ROAD_CODE == CNCode).FirstOrDefault();
                if (query != null)
                {
                    lst.Add(query.PLAN_RD_NAME.Trim());
                    lst.Add(Convert.ToString(query.PLAN_RD_TOTAL_LEN));
                    lst.Add(Convert.ToString(query.PLAN_RD_TOTAL_LEN - (query.PLAN_ROAD_MRL_PMGSY3.Sum(x => x.PLAN_RD_LENGTH))));
                }

                return lst;
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "GetCNLengthPMGSY3DAL()");
                return null;
            }
            finally
            {
                dbContext.Dispose();
            }
        }

        public bool FinalizeCNHabitationDetailsPMGSY3DAL(int CNCode)
        {
            PMGSYEntities dbContext = new PMGSYEntities();
            try
            {
                var lstMappedDetails = dbContext.PLAN_ROAD_HABITATION_PMGSY3.Where(m => m.PLAN_CN_ROAD_CODE == CNCode).ToList();
                if (lstMappedDetails != null)
                {
                    lstMappedDetails.ForEach(m => { m.PLAN_CN_HAB_FINALIZED = "Y"; m.PLAN_CN_HAB_FIN_DATE = DateTime.Now; });
                    dbContext.SaveChanges();
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "DeFinalizeMappedDRRPDetailsPMGSY3DAL()");
                return false;
            }
            finally
            {
                dbContext.Dispose();
            }
        }

       

        #region PMGSY3 CN Finalize/ Definalize BLOCK/DISTRICT




        public Array GetBlockListMRLPMGSY3DAL(int districtCode, int? page, int? rows, string sidx, string sord, out long totalRecords, string filters, ref bool isAllBlockFinalized)
        {



            PMGSYEntities dbContext = new PMGSYEntities();
            try
            {
                isAllBlockFinalized = false;

                JavaScriptSerializer js = null;
                PMGSY.Common.CommonFunctions.SearchJson test = new PMGSY.Common.CommonFunctions.SearchJson();
                string roadName = string.Empty;

                if (filters != null)
                {
                    js = new JavaScriptSerializer();
                    test = js.Deserialize<PMGSY.Common.CommonFunctions.SearchJson>(filters);

                    foreach (PMGSY.Common.CommonFunctions.rules item in test.rules)
                    {
                        switch (item.field)
                        {
                            case "MAST_ER_ROAD_NAME": roadName = item.data;
                                break;
                            default:
                                break;
                        }
                    }
                }

                dbContext = new PMGSYEntities();

                var lstMRLFinalizedBlocks = dbContext.MAST_CN_BLOCK_PMGSY3_FINALIZE.Where(z => z.MASTER_BLOCK.MAST_DISTRICT_CODE == districtCode && z.MASTER_BLOCK.MAST_BLOCK_ACTIVE == "Y" && z.IS_FINALIZED == "Y").Select(x => new { MAST_BLOCK_CODE = x.MAST_BLOCK_CODE, MAST_BLOCK_NAME = x.MASTER_BLOCK.MAST_BLOCK_NAME }).Distinct().ToList();

                ///Get Block Names
                var lstBlock = dbContext.MASTER_BLOCK.Where(c => c.MAST_DISTRICT_CODE == districtCode && c.MAST_BLOCK_ACTIVE == "Y").Select(x => new { MAST_STATE_CODE = x.MASTER_DISTRICT.MASTER_STATE.MAST_STATE_CODE, MAST_BLOCK_CODE = x.MAST_BLOCK_CODE, MAST_BLOCK_NAME = x.MAST_BLOCK_NAME.Trim() }).OrderBy(z => z.MAST_BLOCK_CODE).Distinct().ToList();

                totalRecords = lstBlock.Count();

                if (sidx.Trim() != string.Empty)
                {
                    if (sord.ToString() == "asc")
                    {
                        lstBlock = lstBlock.OrderBy(x => x.MAST_BLOCK_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                    }
                    else
                    {
                        lstBlock = lstBlock.OrderByDescending(x => x.MAST_BLOCK_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                    }
                }
                else
                {
                    lstBlock = lstBlock.OrderBy(x => x.MAST_BLOCK_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                }

                if (lstBlock.Count() == lstMRLFinalizedBlocks.Count() && (!dbContext.MAST_CN_DISTRICT_PMGSY3_FINALIZE.Any(z => z.MAST_DISTRICT_CODE == districtCode && z.IS_FINALIZED == "Y") || !dbContext.MAST_CN_DISTRICT_PMGSY3_FINALIZE.Any(z => z.MAST_DISTRICT_CODE == districtCode)))
                {
                    isAllBlockFinalized = true;
                }

                //var lstScore = (dbContext.MAST_TRACE_DRRP_SCORE_PMGSY3.Where(z=>z.MAST_ER_ROAD_SCORE > 0 && z.MAST_ER_ROAD_SCORE <=15.000))
                //var lst = (dbContext.PLAN_ROAD.Where(x => x.MAST_PMGSY_SCHEME == 4 && x.MAST_STATE_CODE == 20 && x.MAST_DISTRICT_CODE == districtCode && x.MAST_BLOCK_CODE == 730 && x.MAST_ER_ROAD_CODE));

                //var lstBlocks = lstBlock.Select(m => new
                //{
                //    m.MAST_BLOCK_CODE,
                //    m.MAST_BLOCK_NAME,
                //    m.MAST_STATE_CODE,
                //    score = ()
                //    }).ToArray();

               // Int32 CNT = ;
                return lstBlock.Select(item => new
                {
                    //id = item.MAST_ER_ROAD_CODE,
                    cell = new[]
                    {       
                        item.MAST_BLOCK_NAME.ToString(),

                        (dbContext.MAST_CN_BLOCK_PMGSY3_FINALIZE.Any(z=>z.MAST_BLOCK_CODE == item.MAST_BLOCK_CODE && z.IS_FINALIZED == "Y"))? "<span class='ui-icon ui-icon-locked ui-align-center'></span>" :
                        ((dbContext.PLAN_ROAD.Where(m=> m.MAST_DISTRICT_CODE == districtCode && m.MAST_BLOCK_CODE == item.MAST_BLOCK_CODE && m.PLAN_LOCK_STATUS == "N" && m.MAST_PMGSY_SCHEME == 4).Any())
                        ? "All TR/MRL roads not finalized for the block"
                        : ((dbContext.PLAN_ROAD.Where(m=> m.MAST_DISTRICT_CODE == districtCode && m.MAST_BLOCK_CODE == item.MAST_BLOCK_CODE && m.MAST_PMGSY_SCHEME == 4).Any())?
                        
                        (
                        CheckCounts(item.MAST_BLOCK_CODE)
                        ?"<a href='#' title='Click here to finalize details' class='ui-icon ui-icon-unlocked ui-align-center' onClick =FinalizeMRLBlock('"+URLEncrypt.EncryptParameters1(new string[]{"BlockCode =" + item.MAST_BLOCK_CODE.ToString().Trim()}) +"'); return false;'></a>"
                        :"Top 15 or Top Eligible TR/ MRLs are not included in Trace Map."
                        )
                        
                        :"TR/MRL roads are not entered")
                        
                        ),
                        
                        
                        
                        


                        //(dbContext.MAST_CN_BLOCK_PMGSY3_FINALIZE.Any(z=>z.MAST_BLOCK_CODE == item.MAST_BLOCK_CODE && z.IS_FINALIZED == "Y"))? "<span class='ui-icon ui-icon-locked ui-align-center'></span>"
                        //: (
                        //           (dbContext.PLAN_ROAD.Where(m=> m.MAST_DISTRICT_CODE == districtCode && m.MAST_BLOCK_CODE == item.MAST_BLOCK_CODE && m.MAST_PMGSY_SCHEME == 4).Any()) // Part1
                                   

                        //           ?    // Part2
                         
                        //           (
                                   
                        //           (dbContext.PLAN_ROAD.Where(m=> m.MAST_DISTRICT_CODE == districtCode && m.MAST_BLOCK_CODE == item.MAST_BLOCK_CODE && m.PLAN_LOCK_STATUS == "N" && m.MAST_PMGSY_SCHEME == 4).Any()) 
                        //                 ? "All TR/MRL roads not finalized for the block"
                        //                 :  "<a href='#' title='Click here to finalize details' class='ui-icon ui-icon-unlocked ui-align-center' onClick =FinalizeMRLBlock('"+URLEncrypt.EncryptParameters1(new string[]{"BlockCode =" + item.MAST_BLOCK_CODE.ToString().Trim()}) +"'); return false;'></a>"
                                    
                                    
                                    
                        //            )
                                    
                                    
                        //            : " TR/MRL roads are not entered."   // Part3

                        // ),





                        //(dbContext.PLAN_ROAD.Where(m=>m.MAST_BLOCK_CODE == item.MAST_BLOCK_CODE && m.PLAN_LOCK_STATUS == "N" && m.MAST_PMGSY_SCHEME == 4 && m.MASTER_EXISTING_ROADS.MAST_TRACE_DRRP_SCORE_PMGSY3.Where(z=>z.MAST_ER_ROAD_SCORE >0 && z.MAST_ER_ROAD_SCORE<=15 && z.MAST_BLOCK_CODE == item.MAST_BLOCK_CODE).Select(c=>c.MAST_ER_ROAD_CODE).Distinct().Contains(m.MAST_ER_ROAD_CODE)).Union(dbContext.PLAN_ROAD_MRL_PMGSY3.Where(m=>m.MAST_BLOCK_CODE == item.MAST_BLOCK_CODE && m.PLAN_LOCK_STATUS == "N" && m.MAST_PMGSY_SCHEME == 4 && m.MASTER_EXISTING_ROADS.MAST_TRACE_DRRP_SCORE_PMGSY3.Where(z=>z.MAST_ER_ROAD_SCORE >0 && z.MAST_ER_ROAD_SCORE<=15 && z.MAST_BLOCK_CODE == item.MAST_BLOCK_CODE).Select(c=>c.MAST_ER_ROAD_CODE).Distinct().Contains(m.MAST_ER_ROAD_CODE)))),
                        //DeFinalize
                        (dbContext.MAST_CN_BLOCK_PMGSY3_FINALIZE.Any(z=>z.MAST_BLOCK_CODE == item.MAST_BLOCK_CODE && z.IS_FINALIZED == "Y") && !dbContext.MAST_CN_DISTRICT_PMGSY3_FINALIZE.Any(z=>z.MAST_DISTRICT_CODE == districtCode && z.IS_FINALIZED ==                                     "Y"))   
                        ? "<a href='#' title='Click here to definalize details' class='ui-icon ui-icon-unlocked ui-align-center' onClick =DeFinalizeMRLBlock('"
                                            +URLEncrypt.EncryptParameters1(new string[]{"BlockCode =" + item.MAST_BLOCK_CODE.ToString().Trim()}) +"'); return false;'></a>"
                        : (dbContext.MAST_CN_BLOCK_PMGSY3_FINALIZE.Any(z=>z.MAST_BLOCK_CODE == item.MAST_BLOCK_CODE && z.IS_FINALIZED == "Y") && dbContext.MAST_CN_DISTRICT_PMGSY3_FINALIZE.Any(z=>z.MAST_DISTRICT_CODE == districtCode && z.IS_FINALIZED                                      == "Y"))
                            ? "District is finalized"
                            : "-"
                    }
                }).ToArray();
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "GetBlockListMRLPMGSY3DAL()");
                totalRecords = 0;
                return null;
            }
            finally
            {
                dbContext.Dispose();
            }
        }

        public bool CheckCounts(int block)
        {
            PMGSYEntities dbContext = new PMGSYEntities();
            bool flag = false;

            try
            {

                //CNT : Number of TR / MRL in Trace Map
                //select ISNULL( Count(MAST_TRACE_DRRP_ID),0) AS CNT 
                //from omms.MAST_TRACE_DRRP_SCORE_PMGSY3 
                //where MAST_ER_ROAD_SCORE >0 and MAST_BLOCK_CODE=730 and MAST_ER_ROAD_SCORE<=15

                //CNT1 : TR / MRL included in Trace Map
                //select ISNULL(count(PLAN_CN_ROAD_CODE),0) AS CNT1 
                //from omms.PLAN_ROAD 
                //where MAST_ER_ROAD_CODE in (select MAST_TRACE_DRRP_ID from omms.MAST_TRACE_DRRP_SCORE_PMGSY3 
                //where MAST_ER_ROAD_SCORE >0 and MAST_BLOCK_CODE=730 and MAST_ER_ROAD_SCORE<=15)

              Int32 CNT=  dbContext.MAST_TRACE_DRRP_SCORE_PMGSY3.Where(m => m.MAST_ER_ROAD_SCORE > 0 && m.MAST_ER_ROAD_SCORE <= 15 && m.MAST_BLOCK_CODE == block).Count();
              var MAST_TRACE_DRRP_IDs = dbContext.MAST_TRACE_DRRP_SCORE_PMGSY3.Where(m => m.MAST_BLOCK_CODE == block && m.MAST_ER_ROAD_SCORE > 0 && m.MAST_ER_ROAD_SCORE <= 15).Select(m => m.MAST_TRACE_DRRP_ID).ToList();
             
              //Int32 CNT1 = dbContext.PLAN_ROAD.Where(m => MAST_TRACE_DRRP_IDs.Contains(m.MAST_ER_ROAD_CODE)).Count();
              var countFromSP = dbContext.USP_CANDIDATE_ROAD_FINALIZATION_(block).FirstOrDefault();
              Int32 CNT1 = Convert.ToInt16(countFromSP);

              if (CNT == CNT1)
              {
                  return true; // Allow to Finalize Block
              }
              else
              {
                  return false; // Dont allow to Finalize Block
              
              }
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "CoreNetworkDAL.CheckCounts()");
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


        public bool FinalizeMRLBlockPMGSY3DAL(int blockCode, ref string message)
        {
            PMGSYEntities dbContext = new PMGSYEntities();
            try
            {
                // Commented on 19 March 2021 

                //string sqlQuery = "SELECT [omms].[UDF_PRIORITY_ROAD_COUNT] ({0})";
                //Object[] parameters = { blockCode };
                //int priorityCount = dbContext.Database.SqlQuery<int>(sqlQuery, parameters).FirstOrDefault();

                //var TRMRLRoads = dbContext.PLAN_ROAD.Where(z => z.MAST_PMGSY_SCHEME == 4 && z.MAST_BLOCK_CODE == blockCode).ToList();
                //var TRMRLRoadsFinalized = dbContext.PLAN_ROAD.Where(z => z.MAST_PMGSY_SCHEME == 4 && z.MAST_BLOCK_CODE == blockCode && z.PLAN_LOCK_STATUS == "Y").ToList();
                //var erScoreList = dbContext.MAST_TRACE_DRRP_SCORE_PMGSY3.Where(z => z.MAST_BLOCK_CODE == blockCode).Select(x => x.MAST_ER_ROAD_SCORE).Distinct().ToList();
                //if (priorityCount < 15)
                //{
                //    if ((erScoreList.Count() == TRMRLRoadsFinalized.Count()))
                //    {
                //        //continue;
                //    }
                //    else if (!(erScoreList.Count() == TRMRLRoadsFinalized.Count()))
                //    {
                //        message = "Block cannot be finalized as all roads are not finalized";
                //        return false;
                //    }
                //    else
                //    {
                //        message = "Block cannot be finalized as Priority Road count is less than 15";
                //        return false;
                //    }
                //}
                //var CNCodes = TRMRLRoads.Select(z => z.PLAN_CN_ROAD_CODE).ToList();
                ////var mappedHabs = dbContext.PLAN_ROAD_HABITATION_PMGSY3.Where(z => CNCodes.Contains(z.PLAN_CN_ROAD_CODE) && z.MAST_HAB_CODE_DIRECT == "N").ToList();
                //var mappedHabs = dbContext.PLAN_ROAD_HABITATION_PMGSY3.Where(z => z.PLAN_ROAD.MAST_BLOCK_CODE == blockCode && z.PLAN_ROAD.MAST_PMGSY_SCHEME == 4 && z.MAST_HAB_CODE_DIRECT == "N").ToList().Distinct();
                //if (mappedHabs.Count() < 10)
                //{
                //    // Below validation is removed as per Pankaj Sir's approval on 10 June 2020
                //    //  message = "Indirect Habitations mapped unsatisfactorily, please contact NRIDA.";
                //    //  return false;
                //}

                MAST_CN_BLOCK_PMGSY3_FINALIZE MRLPmgsy3Model = new MAST_CN_BLOCK_PMGSY3_FINALIZE();
                dbContext = new PMGSYEntities();

                MRLPmgsy3Model.MAST_CN_BLOCK_FIN_CODE = (dbContext.MAST_CN_BLOCK_PMGSY3_FINALIZE.Any() ? dbContext.MAST_CN_BLOCK_PMGSY3_FINALIZE.Max(z => z.MAST_CN_BLOCK_FIN_CODE) : 0) + 1;
                MRLPmgsy3Model.MAST_BLOCK_CODE = blockCode;
                MRLPmgsy3Model.MAST_CN_BLOCK_FINALIZE_DATE = DateTime.Now;
                MRLPmgsy3Model.IS_FINALIZED = "Y";

                MRLPmgsy3Model.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                MRLPmgsy3Model.USERID = PMGSYSession.Current.UserId;
                dbContext.MAST_CN_BLOCK_PMGSY3_FINALIZE.Add(MRLPmgsy3Model);
                dbContext.SaveChanges();
                message = "Block finalized successfully";
                return true;
            }
            catch (DbEntityValidationException e)
            {
                ErrorLog.LogError(e, "FinalizeMRLBlockPMGSY3DAL().DbEntityValidationException");

                ModelStateDictionary modelstate = new ModelStateDictionary();
                foreach (var eve in e.EntityValidationErrors)
                {
                    foreach (var ve in eve.ValidationErrors)
                    {
                        modelstate.AddModelError(ve.ErrorMessage, ve.ErrorMessage);
                    }
                }

                using (System.IO.StreamWriter sw = System.IO.File.AppendText(System.IO.Path.Combine("D:\\ErrorLog\\OMMASErrorLog_" + System.DateTime.Now.Date.ToString("dd_MM_yyyy") + ".txt", "")))
                {
                    sw.WriteLine("Date :" + DateTime.Now.ToString());
                    sw.WriteLine("FinalizeMRLBlockPMGSY3DAL().DbEntityValidationException()");
                    sw.WriteLine("Exception : " + new CommonFunctions().FormatErrorMessage(modelstate));
                    sw.WriteLine("---------------------------------------------------------------------------------------");
                    sw.Close();
                }
                return false;
            }
            catch (OptimisticConcurrencyException ex)
            {
                ErrorLog.LogError(ex, "FinalizeMRLBlockPMGSY3DAL().OptimisticConcurrencyException");

                message = "An Error Occurred While Processing Your Request.";
                return false;
            }
            catch (UpdateException ex)
            {
                ErrorLog.LogError(ex, "FinalizeMRLBlockPMGSY3DAL().UpdateException");
                message = "An Error Occurred While Processing Your Request.";
                return false;
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "FinalizeMRLBlockPMGSY3DAL()");
                message = "An Error Occurred While Processing Your Request.";
                return false;
            }
            finally
            {
                dbContext.Dispose();
            }
        }

        public bool FinalizeMRLDistrictPMGSY3DAL(int districtCode, ref string message)
        {
            PMGSYEntities dbContext = new PMGSYEntities();
            try
            {
                MAST_CN_DISTRICT_PMGSY3_FINALIZE MRLPmgsy3Model = new MAST_CN_DISTRICT_PMGSY3_FINALIZE();
                dbContext = new PMGSYEntities();

                MRLPmgsy3Model.MAST_CN_DISTRICT_FIN_CODE = (dbContext.MAST_CN_DISTRICT_PMGSY3_FINALIZE.Any() ? dbContext.MAST_CN_DISTRICT_PMGSY3_FINALIZE.Max(z => z.MAST_CN_DISTRICT_FIN_CODE) : 0) + 1;
                MRLPmgsy3Model.MAST_DISTRICT_CODE = districtCode;
                MRLPmgsy3Model.MAST_CN_DISTRICT_FINALIZE_DATE = DateTime.Now;
                MRLPmgsy3Model.IS_FINALIZED = "Y";

                MRLPmgsy3Model.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                MRLPmgsy3Model.USERID = PMGSYSession.Current.UserId;
                dbContext.MAST_CN_DISTRICT_PMGSY3_FINALIZE.Add(MRLPmgsy3Model);
                dbContext.SaveChanges();
                message = "District finalized successfully";
                return true;
            }
            catch (OptimisticConcurrencyException ex)
            {
                ErrorLog.LogError(ex, "FinalizeMRLDistrictPMGSY3DAL().OptimisticConcurrencyException");
                message = "An Error Occurred While Processing Your Request.";
                return false;
            }
            catch (UpdateException ex)
            {
                ErrorLog.LogError(ex, "FinalizeMRLDistrictPMGSY3DAL().UpdateException");
                message = "An Error Occurred While Processing Your Request.";
                return false;
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "FinalizeMRLDistrictPMGSY3DAL()");
                message = "An Error Occurred While Processing Your Request.";
                return false;
            }
            finally
            {
                dbContext.Dispose();
            }
        }

        public bool DeFinalizeMRLBlockPMGSY3DAL(int blockCode, ref string message)
        {

            PMGSYEntities dbContext = new PMGSYEntities();
            bool isBlockFinalizedForPCI = false;
            bool isDistrictFinalizedForTRMRL = false;
            try
            {
                int distcode = dbContext.MASTER_BLOCK.Where(x => x.MAST_BLOCK_CODE == blockCode).Select(m => m.MAST_DISTRICT_CODE).FirstOrDefault();

                isDistrictFinalizedForTRMRL = dbContext.MAST_CN_DISTRICT_PMGSY3_FINALIZE.Where(x => x.MAST_DISTRICT_CODE == distcode && x.IS_FINALIZED == "Y").Any();

                isBlockFinalizedForPCI = dbContext.MAST_PCI_BLOCK_PMGSY3_FINALIZE.Where(x => x.MAST_BLOCK_CODE == blockCode && x.IS_FINALIZED == "Y").Any();

                if (isBlockFinalizedForPCI)
                {
                    message = "This block is already finalized for PCI Details. Hence it can not be definalized here.";
                    return false;
                }
                else if (isDistrictFinalizedForTRMRL)
                {
                    message = "This District is already finalized for TR / MRL Details. Hence first definalize this district.";
                    return false;

                }
                else
                {
                    dbContext = new PMGSYEntities();
                    MAST_CN_BLOCK_PMGSY3_FINALIZE MRLPmgsy3Model = dbContext.MAST_CN_BLOCK_PMGSY3_FINALIZE.Where(z => z.MAST_BLOCK_CODE == blockCode).FirstOrDefault();
                    dbContext.MAST_CN_BLOCK_PMGSY3_FINALIZE.Remove(MRLPmgsy3Model);
                    dbContext.SaveChanges();
                    message = "Block definalized successfully";
                    return true;
                }
            }
            catch (DbEntityValidationException e)
            {
                ErrorLog.LogError(e, "DeFinalizeMRLBlockPMGSY3DAL().DbEntityValidationException");

                ModelStateDictionary modelstate = new ModelStateDictionary();
                foreach (var eve in e.EntityValidationErrors)
                {
                    foreach (var ve in eve.ValidationErrors)
                    {
                        modelstate.AddModelError(ve.ErrorMessage, ve.ErrorMessage);
                    }
                }

                using (System.IO.StreamWriter sw = System.IO.File.AppendText(System.IO.Path.Combine("D:\\ErrorLog\\OMMASErrorLog_" + System.DateTime.Now.Date.ToString("dd_MM_yyyy") + ".txt", "")))
                {
                    sw.WriteLine("Date :" + DateTime.Now.ToString());
                    sw.WriteLine("DeFinalizeMRLBlockPMGSY3DAL().DbEntityValidationException()");
                    sw.WriteLine("Exception : " + new CommonFunctions().FormatErrorMessage(modelstate));
                    sw.WriteLine("---------------------------------------------------------------------------------------");
                    sw.Close();
                }
                return false;
            }
            catch (OptimisticConcurrencyException ex)
            {
                ErrorLog.LogError(ex, "DeFinalizeMRLBlockPMGSY3DAL().OptimisticConcurrencyException");

                message = "An Error Occurred While Processing Your Request.";
                return false;
            }
            catch (UpdateException ex)
            {
                ErrorLog.LogError(ex, "DeFinalizeMRLBlockPMGSY3DAL().UpdateException");
                message = "An Error Occurred While Processing Your Request.";
                return false;
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "FinalizeMRLBlockPMGSY3DAL()");
                message = "An Error Occurred While Processing Your Request.";
                return false;
            }
            finally
            {
                dbContext.Dispose();
            }
        }

        public bool DefinalizeMRLDistrictPMGSY3DAL(int districtCode, ref string message) //Added by Aditi on 5 June 2020
        {
            PMGSYEntities dbContext = new PMGSYEntities();
            try
            {
                MAST_CN_DISTRICT_PMGSY3_FINALIZE_LOG MRLPmgsy3Model = new MAST_CN_DISTRICT_PMGSY3_FINALIZE_LOG();

                MAST_CN_DISTRICT_PMGSY3_FINALIZE FinalizeDetails = dbContext.MAST_CN_DISTRICT_PMGSY3_FINALIZE.Where(x => x.MAST_DISTRICT_CODE == districtCode).FirstOrDefault();

                using (TransactionScope ts = new TransactionScope())
                {
                    if (FinalizeDetails != null)
                    {
                        MRLPmgsy3Model.MAST_CN_DISTRICT_FIN_CODE = (dbContext.MAST_CN_DISTRICT_PMGSY3_FINALIZE_LOG.Any() ? dbContext.MAST_CN_DISTRICT_PMGSY3_FINALIZE_LOG.Max(z => z.MAST_CN_DISTRICT_FIN_CODE) : 0) + 1;
                        MRLPmgsy3Model.MAST_DISTRICT_CODE = districtCode;
                        MRLPmgsy3Model.MAST_CN_DISTRICT_FINALIZE_DATE = FinalizeDetails.MAST_CN_DISTRICT_FINALIZE_DATE;
                        MRLPmgsy3Model.IS_FINALIZED = "N";
                        MRLPmgsy3Model.IPADD = FinalizeDetails.IPADD;
                        MRLPmgsy3Model.USERID = FinalizeDetails.USERID;

                        MRLPmgsy3Model.UNLOCKED_BY_IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                        MRLPmgsy3Model.UNLOCKED_BY_DATETIME = DateTime.Now;
                        MRLPmgsy3Model.UNLOCKED_BY_USERID = PMGSYSession.Current.UserId;

                        dbContext.MAST_CN_DISTRICT_PMGSY3_FINALIZE_LOG.Add(MRLPmgsy3Model);
                        dbContext.SaveChanges();
                        //To delete finalized record after definalizing it
                        dbContext.Entry(FinalizeDetails).State = EntityState.Deleted;
                        dbContext.SaveChanges();
                        message = "District Definalized successfully";
                        ts.Complete();
                        return true;
                    }
                    else
                    {
                        message = "No records found to definalize. District already definalized";
                        ts.Complete();
                        return false;
                    }
                }
            }
            catch (OptimisticConcurrencyException ex)
            {
                ErrorLog.LogError(ex, "DefinalizeMRLDistrictPMGSY3DAL().OptimisticConcurrencyException");
                message = "An Error Occurred While Processing Your Request.";
                return false;
            }
            catch (UpdateException ex)
            {
                ErrorLog.LogError(ex, "DefinalizeMRLDistrictPMGSY3DAL().UpdateException");
                message = "An Error Occurred While Processing Your Request.";
                return false;
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "DefinalizeMRLDistrictPMGSY3DAL()");
                message = "An Error Occurred While Processing Your Request.";
                return false;
            }
            finally
            {
                dbContext.Dispose();
            }
        }



        #endregion

        #region PCI Entry

        public Array GetPmgsyRoadsDAL(int page, int rows, string sidx, string sord, out Int32 totalRecords, int MAST_STATE_CODE, int MAST_DISTRICT_CODE, int ADMIN_ND_CODE, int IMS_YEAR, int MAST_BLOCK_CODE)
        {
            PMGSYEntities dbContext = new Models.PMGSYEntities();

            try
            {
                List<IMS_SANCTIONED_PROJECTS> list_ims_sanctioned_project = (
                                                                                from c in dbContext.IMS_SANCTIONED_PROJECTS
                                                                                join d in dbContext.EXEC_ROADS_MONTHLY_STATUS
                                                                                on c.IMS_PR_ROAD_CODE equals d.IMS_PR_ROAD_CODE
                                                                                where
                                                                                    c.IMS_PR_ROAD_CODE == d.IMS_PR_ROAD_CODE &&
                                                                                    c.MAST_STATE_CODE == MAST_STATE_CODE &&
                                                                                    c.MAST_DISTRICT_CODE == MAST_DISTRICT_CODE &&
                                                                                    c.MAST_DPIU_CODE == ADMIN_ND_CODE &&
                                                                                    (MAST_BLOCK_CODE > 0 ? c.MAST_BLOCK_CODE : 1) == (MAST_BLOCK_CODE > 0 ? MAST_BLOCK_CODE : 1) &&
                                                                                    c.IMS_YEAR == IMS_YEAR &&
                                                                                    c.IMS_PROPOSAL_TYPE == "P" &&
                                                                                    (c.IMS_ISCOMPLETED == "C" || c.IMS_ISCOMPLETED == "X") &&
                                                                                    d.EXEC_ISCOMPLETED == "C" &&
                                                                                    c.MAST_PMGSY_SCHEME == PMGSYSession.Current.PMGSYScheme //new change done by Vikram on 10 Feb 2014
                                                                                select c
                                                                            ).ToList<IMS_SANCTIONED_PROJECTS>();
                totalRecords = list_ims_sanctioned_project.Count();
                if (sidx.Trim() != string.Empty)
                {
                    if (sord.ToString() == "asc")
                    {
                        list_ims_sanctioned_project = list_ims_sanctioned_project.OrderBy(x => x.IMS_ROAD_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                    }
                    else
                    {
                        list_ims_sanctioned_project = list_ims_sanctioned_project.OrderByDescending(x => x.IMS_ROAD_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                    }
                }
                else
                {
                    list_ims_sanctioned_project = list_ims_sanctioned_project.OrderBy(x => x.IMS_ROAD_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                }
                return list_ims_sanctioned_project.Select(propDetails => new
                {
                    id = URLEncrypt.EncryptParameters(new string[] { propDetails.IMS_PR_ROAD_CODE.ToString() }),
                    cell = new[] {                         
                                    (propDetails.IMS_ROAD_NAME == null || propDetails.IMS_ROAD_NAME == "" )? "NA" :  propDetails.IMS_ROAD_NAME,
                                    propDetails.IMS_PACKAGE_ID,
                                    propDetails.IMS_PAV_LENGTH.ToString(),                                                               
                                    (propDetails.MANE_IMS_PCI_INDEX.Where(a=> a.IMS_PR_ROAD_CODE == propDetails.IMS_PR_ROAD_CODE).Any()) ? propDetails.MANE_IMS_PCI_INDEX.Max(a=> a.MANE_PCI_YEAR).ToString() : "-",
                                    "<a href='#' class='ui-icon ui-icon-plusthick ui-align-center' onclick='AddPCIIndexForPmgsyRoad(\"" + URLEncrypt.EncryptParameters(new string[] { propDetails.IMS_PR_ROAD_CODE.ToString().Trim() }) + "\"); return false;'>CBR Details</a>" 
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

        //public Array GetCNRoadsDAL(int page, int rows, string sidx, string sord, out int totalRecords, int MAST_STATE_CODE, int MAST_DISTRICT_CODE, int ADMIN_ND_CODE, int IMS_BLOCK_ID , int IMS_YEAR)
        //{

        //    PMGSYEntities dbContext = new Models.PMGSYEntities();

        //    try
        //    {
        //        List<int> RoadCatagoryFilter = new List<int>();
        //        RoadCatagoryFilter.Add(1); // National Highway
        //        RoadCatagoryFilter.Add(2); // State Highway
        //        RoadCatagoryFilter.Add(3); // Major District Road

        //        var ListCNRoads = (
        //                                            from c in dbContext.PLAN_ROAD
        //                                            join e in dbContext.MASTER_EXISTING_ROADS
        //                                            on c.MAST_ER_ROAD_CODE equals e.MAST_ER_ROAD_CODE
        //                                            join f in dbContext.MASTER_ROAD_CATEGORY
        //                                            on e.MAST_ROAD_CAT_CODE equals f.MAST_ROAD_CAT_CODE

        //                                            join PRM in dbContext.PLAN_ROAD_MRL_PMGSY3 on
        //                                            c.PLAN_CN_ROAD_CODE equals PRM.PLAN_CN_ROAD_CODE
        //                                            into joinedtable from mappingTable in joinedtable.DefaultIfEmpty()

        //                                            where
        //                                            c.MAST_STATE_CODE == MAST_STATE_CODE &&
        //                                            c.MAST_DISTRICT_CODE == MAST_DISTRICT_CODE &&
        //                                            (IMS_BLOCK_ID > 0 ? c.MAST_BLOCK_CODE : 1) == (IMS_BLOCK_ID > 0 ? IMS_BLOCK_ID : 1) 
        //                                            //&&
        //                                            //!RoadCatagoryFilter.Contains(f.MAST_ROAD_CAT_CODE)
        //                                            && c.MAST_PMGSY_SCHEME == PMGSYSession.Current.PMGSYScheme //as per the change done in application for scheme 
        //                                            && e.MAST_CONS_YEAR == (IMS_YEAR == 0 ? e.MAST_CONS_YEAR : IMS_YEAR)

        //                                            select new 
        //                                            {
        //                                                c.PLAN_CN_ROAD_CODE,
        //                                                c.PLAN_CN_ROAD_NUMBER,
        //                                                c.PLAN_RD_NAME,
        //                                                c.MASTER_EXISTING_ROADS,
        //                                                c.PLAN_RD_FROM_CHAINAGE,
        //                                                c.PLAN_RD_TO_CHAINAGE,
        //                                            } 

        //                                       ).OrderBy(c => c.PLAN_CN_ROAD_NUMBER).ToList();

        //        var PLAN_ROAD_MRL_PMGSY3List = (from item in dbContext.PLAN_ROAD_MRL_PMGSY3
        //                                        where item.PLAN_ROAD.MAST_BLOCK_CODE == IMS_BLOCK_ID
        //                                        select new
        //                                        {
        //                                            PLAN_CN_ROAD_CODE = item.PLAN_CN_ROAD_CODE,
        //                                            PLAN_CN_ROAD_NUMBER = item.PLAN_ROAD.PLAN_CN_ROAD_NUMBER,
        //                                            PLAN_RD_NAME = item.MASTER_EXISTING_ROADS.MAST_ER_ROAD_NAME,
        //                                            MASTER_EXISTING_ROADS = item.MASTER_EXISTING_ROADS,
        //                                            PLAN_RD_FROM_CHAINAGE = item.PLAN_RD_FROM_CHAINAGE,
        //                                            PLAN_RD_TO_CHAINAGE = item.PLAN_RD_TO_CHAINAGE
        //                                        }).ToList();




        //        var UnionList = ListCNRoads.Union(PLAN_ROAD_MRL_PMGSY3List).OrderBy(x => x.PLAN_CN_ROAD_CODE).ToList();

        //        totalRecords = UnionList.Count();

        //        if (sidx.Trim() != string.Empty)
        //        {
        //            if (sord.ToString() == "asc")
        //            {
        //                UnionList = UnionList.OrderBy(c => c.PLAN_CN_ROAD_NUMBER).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
        //            }
        //            else
        //            {
        //                UnionList = UnionList.OrderByDescending(c => c.PLAN_CN_ROAD_NUMBER).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
        //            }
        //        }
        //        else
        //        {
        //            UnionList = UnionList.OrderBy(c => c.PLAN_CN_ROAD_NUMBER).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
        //        }

        //        UnionList.Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));

        //        return UnionList.Select(CNDetails => new
        //        {
        //            id = URLEncrypt.EncryptParameters(new string[] { CNDetails.PLAN_CN_ROAD_CODE.ToString() }),
        //            cell = new[] {    
        //                            CNDetails.PLAN_CN_ROAD_CODE.ToString(),
        //                            (CNDetails.PLAN_RD_NAME == null || CNDetails.PLAN_RD_NAME == "" ) ? "NA" :  CNDetails.PLAN_RD_NAME,
        //                            CNDetails.PLAN_CN_ROAD_NUMBER.ToString(),
        //                            CNDetails.MASTER_EXISTING_ROADS.MAST_ER_ROAD_NAME.Trim() + " (" + CNDetails.MASTER_EXISTING_ROADS.MAST_ER_ROAD_CODE.ToString() + ")",
        //                            CNDetails.MASTER_EXISTING_ROADS.MASTER_ROAD_CATEGORY.MAST_ROAD_CAT_NAME,
        //                            CNDetails.PLAN_RD_FROM_CHAINAGE .ToString(),
        //                            CNDetails.PLAN_RD_TO_CHAINAGE.ToString(),

        //                            (dbContext.MANE_CN_PCI_INDEX_PMGSY3.Where(a=> a.PLAN_CN_ROAD_CODE == CNDetails.PLAN_CN_ROAD_CODE).Any()) ? dbContext.MANE_CN_PCI_INDEX_PMGSY3.Max(a => a.MANE_PCI_YEAR).ToString() : "-",      

        //                            (dbContext.PLAN_ROAD_MRL_PMGSY3.AsEnumerable().Where(xx => xx.MAST_ER_ROAD_CODE.ToString().Equals(CNDetails.MASTER_EXISTING_ROADS.MAST_ER_ROAD_NAME.Trim()) 
        //                                && xx.PLAN_CN_ROAD_CODE.ToString().Equals(CNDetails.PLAN_CN_ROAD_CODE.ToString().Trim()) 
        //                                && xx.PLAN_LOCK_STATUS == "Y").Any()  ||


        //                            dbContext.MAST_CN_BLOCK_PMGSY3_FINALIZE.Where(xy => xy.MAST_BLOCK_CODE == IMS_BLOCK_ID && xy.IS_FINALIZED == "Y").Any())?
        //                            "<a href='#' class='ui-icon ui-icon-plusthick ui-align-center' onclick='AddPCIIndexForCNRoad(\"" + URLEncrypt.EncryptParameters(new string[] { CNDetails.PLAN_CN_ROAD_CODE.ToString() + "$" +                                                                CNDetails.MASTER_EXISTING_ROADS.MAST_ER_ROAD_CODE.ToString()}) + "\"); return false;'>PCI Index</a>" : "Road under TR/MRL or Block has not been finalized." ,

        //                            dbContext.MANE_CN_PCI_INDEX_PMGSY3.Where(x => x.PLAN_CN_ROAD_CODE == CNDetails.PLAN_CN_ROAD_CODE && x.IS_FINALIZED != null).Any() ? "Y" : "N"
        //                        }
        //        }).ToArray();
        //    }
        //    catch (Exception ex)
        //    {
        //        totalRecords = 0;
        //        return null;
        //    }
        //    finally
        //    {
        //        dbContext.Dispose();
        //    }
        //}

        //copy
        public Array GetCNRoadsDAL(int page, int rows, string sidx, string sord, out int totalRecords, int MAST_STATE_CODE, int MAST_DISTRICT_CODE, int ADMIN_ND_CODE, int IMS_BLOCK_ID, int IMS_YEAR)
        {

            PMGSYEntities dbContext = new Models.PMGSYEntities();

            try
            {
                List<int> RoadCatagoryFilter = new List<int>();
                RoadCatagoryFilter.Add(1); // National Highway
                RoadCatagoryFilter.Add(2); // State Highway
                RoadCatagoryFilter.Add(3); // Major District Road

                //var ListCNRoads = (
                //                                    from c in dbContext.PLAN_ROAD
                //                                    join e in dbContext.MASTER_EXISTING_ROADS
                //                                    on c.MAST_ER_ROAD_CODE equals e.MAST_ER_ROAD_CODE
                //                                    join f in dbContext.MASTER_ROAD_CATEGORY
                //                                    on e.MAST_ROAD_CAT_CODE equals f.MAST_ROAD_CAT_CODE

                //                                    join PRM in dbContext.PLAN_ROAD_MRL_PMGSY3 on
                //                                    c.PLAN_CN_ROAD_CODE equals PRM.PLAN_CN_ROAD_CODE
                //                                    into joinedtable
                //                                    from mappingTable in joinedtable.DefaultIfEmpty()

                //                                    where
                //                                    c.MAST_STATE_CODE == MAST_STATE_CODE &&
                //                                    c.MAST_DISTRICT_CODE == MAST_DISTRICT_CODE &&
                //                                    (IMS_BLOCK_ID > 0 ? c.MAST_BLOCK_CODE : 1) == (IMS_BLOCK_ID > 0 ? IMS_BLOCK_ID : 1)
                //                                        //&&
                //                                        //!RoadCatagoryFilter.Contains(f.MAST_ROAD_CAT_CODE)
                //                                    && c.MAST_PMGSY_SCHEME == PMGSYSession.Current.PMGSYScheme //as per the change done in application for scheme 
                //                                    && e.MAST_CONS_YEAR == (IMS_YEAR == 0 ? e.MAST_CONS_YEAR : IMS_YEAR)

                //                                    select new
                //                                    {
                //                                        c.PLAN_CN_ROAD_CODE,
                //                                        c.PLAN_CN_ROAD_NUMBER,
                //                                        c.PLAN_RD_NAME,
                //                                        c.MASTER_EXISTING_ROADS,
                //                                        c.PLAN_RD_FROM_CHAINAGE,
                //                                        c.PLAN_RD_TO_CHAINAGE,
                //                                    }

                //                               ).OrderBy(c => c.PLAN_CN_ROAD_NUMBER).ToList();

                //var PLAN_ROAD_MRL_PMGSY3List = (from item in dbContext.PLAN_ROAD_MRL_PMGSY3
                //                                where item.PLAN_ROAD.MAST_BLOCK_CODE == IMS_BLOCK_ID
                //                                select new
                //                                {
                //                                    PLAN_CN_ROAD_CODE = item.PLAN_CN_ROAD_CODE,
                //                                    PLAN_CN_ROAD_NUMBER = item.PLAN_ROAD.PLAN_CN_ROAD_NUMBER,
                //                                    PLAN_RD_NAME = item.MASTER_EXISTING_ROADS.MAST_ER_ROAD_NAME,
                //                                    MASTER_EXISTING_ROADS = item.MASTER_EXISTING_ROADS,
                //                                    PLAN_RD_FROM_CHAINAGE = item.PLAN_RD_FROM_CHAINAGE,
                //                                    PLAN_RD_TO_CHAINAGE = item.PLAN_RD_TO_CHAINAGE
                //                                }).ToList();




                //var UnionList = ListCNRoads.Union(PLAN_ROAD_MRL_PMGSY3List).OrderBy(x => x.PLAN_CN_ROAD_CODE).ToList();

                var ListCNRoads = dbContext.USP_PMGSY3_CN_PCI_ROADLIST(MAST_STATE_CODE, MAST_DISTRICT_CODE, IMS_BLOCK_ID, PMGSYSession.Current.PMGSYScheme).ToList();

                totalRecords = ListCNRoads.Count();

                if (sidx.Trim() != string.Empty)
                {
                    if (sord.ToString() == "asc")
                    {
                        ListCNRoads = ListCNRoads.OrderBy(c => c.PLAN_CN_ROAD_NUMBER).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                    }
                    else
                    {
                        ListCNRoads = ListCNRoads.OrderByDescending(c => c.PLAN_CN_ROAD_NUMBER).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                    }
                }
                else
                {
                    ListCNRoads = ListCNRoads.OrderBy(c => c.PLAN_CN_ROAD_NUMBER).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                }

                ListCNRoads.Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));

                return ListCNRoads.Select(CNDetails => new
                {
                    id = URLEncrypt.EncryptParameters(new string[] { CNDetails.PLAN_CN_ROAD_CODE.ToString() }),
                    cell = new[] {    
                                    CNDetails.PLAN_CN_ROAD_CODE.ToString(),
                                    (CNDetails.PLAN_RD_NAME == null || CNDetails.PLAN_RD_NAME == "" ) ? "NA" :  CNDetails.PLAN_RD_NAME,
                                    CNDetails.PLAN_CN_ROAD_NUMBER.ToString(),
                                    CNDetails.MAST_ER_ROAD_NAME.Trim() + " (" + CNDetails.MAST_ER_ROAD_CODE.ToString() + ")",
                                    CNDetails.MAST_ROAD_CAT_NAME,
                                    CNDetails.DRRP_START_CHAINAGE.ToString(),
                                    CNDetails.DRRP_END_CHAINAGE.ToString(),

                                    (dbContext.MANE_CN_PCI_INDEX_PMGSY3.Where(a=> a.PLAN_CN_ROAD_CODE == CNDetails.PLAN_CN_ROAD_CODE).Any()) ? dbContext.MANE_CN_PCI_INDEX_PMGSY3.Max(a => a.MANE_PCI_YEAR).ToString() : "-",      
                              
                                    (dbContext.PLAN_ROAD_MRL_PMGSY3.AsEnumerable().Where(xx => xx.MAST_ER_ROAD_CODE.ToString().Equals(CNDetails.MAST_ER_ROAD_CODE.ToString().Trim()) 
                                        && xx.PLAN_CN_ROAD_CODE.ToString().Equals(CNDetails.PLAN_CN_ROAD_CODE.ToString().Trim()) 
                                        && xx.PLAN_LOCK_STATUS == "Y").Any()  ||
                                    
                                    
                                    dbContext.MAST_CN_BLOCK_PMGSY3_FINALIZE.Where(xy => xy.MAST_BLOCK_CODE == IMS_BLOCK_ID && xy.IS_FINALIZED == "Y").Any()
                                    && dbContext.MAST_DRRP_BLOCK_PMGSY3_FINALIZE.Where(xy => xy.MAST_BLOCK_CODE == IMS_BLOCK_ID && xy.IS_FINALIZED == "Y").Any()
                                    )?
                                    "<a href='#' class='ui-icon ui-icon-plusthick ui-align-center' onclick='AddPCIIndexForCNRoad(\"" + URLEncrypt.EncryptParameters(new string[] { CNDetails.PLAN_CN_ROAD_CODE.ToString() + "$" +CNDetails.MAST_ER_ROAD_CODE.ToString()}) + "\"); return false;'>PCI Index</a>" : "Road under TR/MRL or Block has not been finalized." ,

                                    dbContext.MANE_CN_PCI_INDEX_PMGSY3.Where(x => x.PLAN_CN_ROAD_CODE == CNDetails.PLAN_CN_ROAD_CODE && x.MAST_ER_ROAD_CODE == CNDetails.MAST_ER_ROAD_CODE && x.IS_FINALIZED != null && x.MANE_PCI_YEAR==IMS_YEAR).Any() ? "Finalized for "+ IMS_YEAR+"-"+(IMS_YEAR+1)
                                    : "Not Finalized for "+IMS_YEAR+"-"+(IMS_YEAR+1)
                                }
                }).ToArray();
            }
            catch (Exception ex)
            {
                totalRecords = 0;
                return null;
            }
            finally
            {
                dbContext.Dispose();
            }
        }

        public string SavePciForCNRoadDAL(PCIIndexViewModel pciIndexViewModel)
        {
            PMGSYEntities DbContext = new PMGSYEntities();

            try
            {

                PLAN_ROAD planCNRoad = DbContext.PLAN_ROAD.Where(x => x.PLAN_CN_ROAD_CODE == pciIndexViewModel.PLAN_CN_ROAD_CODE).FirstOrDefault();

                decimal chainage = 0;

                if (DbContext.MANE_CN_PCI_INDEX_PMGSY3.Any(m => m.PLAN_CN_ROAD_CODE == pciIndexViewModel.PLAN_CN_ROAD_CODE && m.MANE_PCI_YEAR == pciIndexViewModel.MANE_PCI_YEAR)) // Added  m.MANE_PCI_YEAR == pciIndexViewModel.MANE_PCI_YEAR
                {
                    foreach (var item in DbContext.MANE_CN_PCI_INDEX_PMGSY3.Where(m => m.PLAN_CN_ROAD_CODE == pciIndexViewModel.PLAN_CN_ROAD_CODE && m.MANE_PCI_YEAR == pciIndexViewModel.MANE_PCI_YEAR).ToList())
                    {
                        chainage += (item.MANE_END_CHAIN - item.MANE_STR_CHAIN);
                    }
                }

                if (chainage == null)
                {
                    chainage = 0;
                }

                chainage = chainage + (pciIndexViewModel.MANE_END_CHAIN - pciIndexViewModel.MANE_STR_CHAIN);
                //if (planCNRoad != null && planCNRoad.PLAN_RD_TOTAL_LEN != null && planCNRoad.PLAN_RD_TOTAL_LEN != 0)
                //{
                //    if (chainage > planCNRoad.PLAN_RD_TOTAL_LEN.Value)
                //    {
                //        return "Sum of Chainage is exceeding the Road Length.";
                //    }
                //}
                //else
                //{
                //    if (chainage > (DbContext.PLAN_ROAD.Where(m => m.PLAN_CN_ROAD_CODE == pciIndexViewModel.PLAN_CN_ROAD_CODE).Select(m => m.PLAN_RD_TO_CHAINAGE).FirstOrDefault() - DbContext.PLAN_ROAD.Where(m => m.PLAN_CN_ROAD_CODE == pciIndexViewModel.PLAN_CN_ROAD_CODE).Select(m => m.PLAN_RD_FROM_CHAINAGE).FirstOrDefault()))
                //    {
                //        return "Sum of Chainage is exceeding the Road Length.";
                //    }
                //}

                int MaxSegmentNumber = 0;
                int MaxID = 0;

                if (DbContext.MANE_CN_PCI_INDEX_PMGSY3.Where(a => a.PLAN_CN_ROAD_CODE == pciIndexViewModel.PLAN_CN_ROAD_CODE && a.MANE_PCI_YEAR == pciIndexViewModel.MANE_PCI_YEAR).Any())
                {
                    MaxSegmentNumber = DbContext.MANE_CN_PCI_INDEX_PMGSY3.Where(a => a.PLAN_CN_ROAD_CODE == pciIndexViewModel.PLAN_CN_ROAD_CODE && a.MANE_PCI_YEAR == pciIndexViewModel.MANE_PCI_YEAR).Select(a => a.MANE_SEGMENT_NO).Max();
                }

                if (DbContext.MANE_CN_PCI_INDEX_PMGSY3.Any())
                {
                    MaxID = DbContext.MANE_CN_PCI_INDEX_PMGSY3.Select(a => a.PCI_ID).Max();
                }
                MaxSegmentNumber++;
                MaxID++;

                MANE_CN_PCI_INDEX_PMGSY3 mane_cn_pci_index = new MANE_CN_PCI_INDEX_PMGSY3();
                mane_cn_pci_index.PCI_ID = MaxID;
                mane_cn_pci_index.PLAN_CN_ROAD_CODE = pciIndexViewModel.PLAN_CN_ROAD_CODE;
                mane_cn_pci_index.MAST_ER_ROAD_CODE = pciIndexViewModel.ER_ROAD_CODE;
                mane_cn_pci_index.MANE_SEGMENT_NO = MaxSegmentNumber;
                mane_cn_pci_index.MANE_PCI_YEAR = pciIndexViewModel.MANE_PCI_YEAR;
                mane_cn_pci_index.MANE_PCIINDEX = pciIndexViewModel.MANE_PCIINDEX;
                mane_cn_pci_index.MANE_STR_CHAIN = pciIndexViewModel.MANE_STR_CHAIN;
                mane_cn_pci_index.MANE_END_CHAIN = pciIndexViewModel.MANE_END_CHAIN;
                mane_cn_pci_index.MANE_SURFACE_TYPE = pciIndexViewModel.MANE_SURFACE_TYPE;
                mane_cn_pci_index.MANE_PCI_DATE = Convert.ToDateTime(pciIndexViewModel.MANE_PCI_DATE);

                mane_cn_pci_index.USERID = PMGSYSession.Current.UserId;
                mane_cn_pci_index.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                DbContext.MANE_CN_PCI_INDEX_PMGSY3.Add(mane_cn_pci_index);
                DbContext.SaveChanges();
                return string.Empty;
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "CoreNetworkDAL/SavePciForCNRoadDAL");
                return "Error Occured while processing your request.";
            }
            finally
            {
                DbContext.Dispose();
            }
        }

        public Array GetPCIListForCNRoadDAL(int page, int rows, string sidx, string sord, out Int32 totalRecords, int PLAN_CN_ROAD_CODE, int ER_ROAD_CODE)
        {
            PMGSYEntities dbContext = new Models.PMGSYEntities();

            try
            {
                var lstPciIndex = (from c in dbContext.MANE_CN_PCI_INDEX_PMGSY3
                                   where c.MAST_ER_ROAD_CODE == ER_ROAD_CODE
                                   && c.PLAN_CN_ROAD_CODE == PLAN_CN_ROAD_CODE
                                   select new
                                   {
                                       PciID = c.PCI_ID,
                                       Plan_cn_road_code = (Nullable<int>)c.PLAN_CN_ROAD_CODE,
                                       SegmentNo = (Nullable<int>)c.MANE_SEGMENT_NO,
                                       Mane_Pci_Year = (Nullable<int>)c.MANE_PCI_YEAR,
                                       Mane_Str_Chain = (Decimal?)c.MANE_STR_CHAIN,
                                       Mane_End_Chain = (Decimal?)c.MANE_END_CHAIN,
                                       Mane_Pci_Index = c.MANE_PCIINDEX,
                                       Mane_Surface_Type = c.MANE_SURFACE_TYPE,
                                       Mane_Pci_Date = c.MANE_PCI_DATE,
                                       isFinalized = c.IS_FINALIZED,
                                       FinalizedDate = c.FINALIZED_DATE,
                                       UserID = c.USERID,
                                       IPADD = c.IPADD,
                                       MasterSurfaceCode = c.MASTER_SURFACE.MAST_SURFACE_CODE,
                                       MasterSurfaceName = c.MASTER_SURFACE.MAST_SURFACE_NAME
                                   }).OrderByDescending(x => x.Mane_Pci_Year).ThenBy(x=>x.SegmentNo).ToList(); // ThenBy is added on 01 DEC 2020


                totalRecords = lstPciIndex.Count();

                //lstPciIndex.OrderByDescending(x => x.MANE_PCI_YEAR).Skip(Convert.ToInt32(page * rows)).Take(rows).ToList<MANE_CN_PCI_INDEX>();

                if (sidx.Trim() != string.Empty)
                {
                    if (sord.ToString() == "asc")
                    {
                        lstPciIndex = lstPciIndex.OrderBy(x => x.Mane_Pci_Year).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                    }
                    else
                    {
                        lstPciIndex = lstPciIndex.OrderByDescending(x => x.Mane_Pci_Year).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                    }
                }
                else
                {
                    lstPciIndex = lstPciIndex.OrderBy(x => x.Mane_Pci_Year).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                }



                return lstPciIndex.Select(PciDetails => new
                {
                    id = URLEncrypt.EncryptParameters(new string[] { PciDetails.Plan_cn_road_code.ToString() + "$" + PciDetails.SegmentNo + "$" + PciDetails.Mane_Pci_Year }),
                    cell = new[] {                         
                                    PciDetails.Mane_Pci_Year.ToString(),
                                    PciDetails.SegmentNo.ToString(),
                                    Convert.ToString(PciDetails.Mane_Str_Chain),
                                    Convert.ToString(PciDetails.Mane_End_Chain),
                                    PciDetails.Mane_Pci_Index.ToString(),
                                    PciDetails.Mane_Surface_Type == null ? "" : PciDetails.MasterSurfaceName,
                                    PciDetails.Mane_Pci_Date != null ? Convert.ToDateTime(PciDetails.Mane_Pci_Date).ToString("dd-MMM-yyyy") : "NA",                                  
                                    
                                    dbContext.MANE_CN_PCI_INDEX_PMGSY3.Where(a=> a.PLAN_CN_ROAD_CODE == PciDetails.Plan_cn_road_code && a.IS_FINALIZED != null  && a.MANE_PCI_YEAR == PciDetails.Mane_Pci_Year).Any() ? "N/A" :
                                    
                                    (dbContext.MANE_PCI_IMAGE_MAPPING_PMGSY3.Where(x => x.PCI_ID == PciDetails.PciID).Any() ? "Photograph(s) against the chainage are uploaded" :
                                   
                                    //((dbContext.MANE_CN_PCI_INDEX_PMGSY3.Where(a=> a.PLAN_CN_ROAD_CODE == PciDetails.Plan_cn_road_code && a.MANE_PCI_YEAR == PciDetails.Mane_Pci_Year).Count(obj => obj.IS_FINALIZED == null)  == 1) ? 
                                    ((dbContext.MANE_CN_PCI_INDEX_PMGSY3.Where(a=> a.PLAN_CN_ROAD_CODE == PciDetails.Plan_cn_road_code && a.MANE_PCI_YEAR == PciDetails.Mane_Pci_Year && a.IS_FINALIZED == null).Any()) ? 
                                    (dbContext.MANE_CN_PCI_INDEX_PMGSY3.Where(a=> a.PLAN_CN_ROAD_CODE == PciDetails.Plan_cn_road_code && a.MANE_PCI_YEAR == PciDetails.Mane_Pci_Year && a.MAST_ER_ROAD_CODE == ER_ROAD_CODE).Select(a=> a.MANE_END_CHAIN).Max() == PciDetails.Mane_End_Chain)  ?  
                                    ("<a href='#' title='Click here to delete Road Details' class='ui-icon ui-icon-trash ui-align-center' onClick='DeletePciForCNRoadDetails(\"" +
                                    URLEncrypt.EncryptParameters1(new string[] { "PCIID =" + PciDetails.PciID.ToString()}) +"\"); return false;'>Show Details</a>") : 
                                    
                                    (dbContext.MANE_CN_PCI_INDEX_PMGSY3.Where(a=> a.PLAN_CN_ROAD_CODE == PciDetails.Plan_cn_road_code && a.MANE_PCI_YEAR == PciDetails.Mane_Pci_Year).Select(a=> a.MANE_END_CHAIN).Max() == PciDetails.Mane_End_Chain )  ?                                     
                                    "<a href='#' title='Click here to delete Road Details' class='ui-icon ui-icon-trash ui-align-center' onClick='DeletePciForCNRoadDetails(\"" +
                                    URLEncrypt.EncryptParameters1(new string[] { "PCIID =" + PciDetails.PciID.ToString()}) +"\"); return false;'>Show Details</a>"   :  "<a href='#' title='N/A' class='ui-icon ui-icon-locked ui-align-center'                                                 onClick='return false;'" : "-")),
                                    
                    "<center><span class='ui-icon ui-icon-image' title='Click here to upload photograph' onClick ='UploadChainagePhotoGraph(\"" + URLEncrypt.EncryptParameters1(new string[] { "PCIID =" +                                                                  PciDetails.PciID.ToString()}) + 
                    "\");'></span></center>"
                   }
                }).ToArray();
            }
            catch (Exception ex)
            {

                totalRecords = 0;
                return null;
            }
            finally
            {
                if (dbContext != null)
                    dbContext.Dispose();
            }
        }

        public bool SavePhotoGraphDAL(int PCIID, string FileName, HttpPostedFileBase filebase, string remark)
        {
            PMGSY.Models.PMGSYEntities dbcontext = new Models.PMGSYEntities();
            try
            {
                MANE_PCI_IMAGE_MAPPING_PMGSY3 dbmodel = new MANE_PCI_IMAGE_MAPPING_PMGSY3();
                int maxid = 0;
                if (dbcontext.MANE_PCI_IMAGE_MAPPING_PMGSY3.Any())
                    maxid = dbcontext.MANE_PCI_IMAGE_MAPPING_PMGSY3.Select(x => x.FILE_ID).Max() + 1;

                else
                    maxid = 1;

                var Entry = dbcontext.MANE_CN_PCI_INDEX_PMGSY3.Where(x => x.PCI_ID == PCIID).FirstOrDefault();
                var RoadCode = Entry.PLAN_CN_ROAD_CODE;
                var ErCode = Entry.MAST_ER_ROAD_CODE;

                if (dbmodel != null)
                {

                    var timestampArray = System.DateTime.Now.ToString("MM/dd/yyyy hh:mm:ss.fff").Replace('.', '_').Split(' ');
                    string timestamp = timestampArray[0].ToString() + timestampArray[1].ToString();
                    FileName = ErCode.ToString() + "_" + timestamp.Replace('/', '_').Replace(':', '_') + "_" + RoadCode.ToString() + ".jpeg";
                    bool isEntrySaved = false;
                    using (TransactionScope ts = new TransactionScope())
                    {
                        dbmodel.FILE_ID = maxid;
                        dbmodel.PCI_ID = PCIID;
                        dbmodel.FILE_NAME = FileName;
                        dbmodel.FILE_UPLOAD_DATE = System.DateTime.Now;
                        dbmodel.REMARKS = remark;
                        dbmodel.USERID = PMGSYSession.Current.UserId;
                        dbmodel.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];

                        dbcontext.Entry(dbmodel).State = EntityState.Added;

                        dbcontext.SaveChanges();
                        ts.Complete();
                        isEntrySaved = true;
                    }

                    if (isEntrySaved)
                        filebase.SaveAs(Path.Combine(ConfigurationManager.AppSettings["PCI_INDEX_CHAINAGE_PHOTO"], FileName));

                    return true;

                }
                return false;
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "MasterEntryDAL/SavePhotoGraph");
                return false;
            }
        }

        public Array GetImageFilesListDAL(int page, int rows, string sidx, string sord, out int totalRecords, int PCIid)
        {
            PMGSYEntities dbContext = new PMGSYEntities();
            try
            {
                var ImageEntryList = dbContext.MANE_PCI_IMAGE_MAPPING_PMGSY3.Where(x => x.PCI_ID == PCIid).ToList();

                totalRecords = ImageEntryList.Count();

                if (sidx.Trim() != string.Empty)
                {
                    if (sord.ToString() == "asc")
                    {
                        ImageEntryList = ImageEntryList.OrderBy(x => x.FILE_ID).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                    }
                    else
                    {
                        ImageEntryList = ImageEntryList.OrderByDescending(x => x.FILE_ID).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                    }
                }
                else
                {
                    ImageEntryList = ImageEntryList.OrderBy(x => x.FILE_ID).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                }

                string VirtualDirectoryUrl = string.Empty;
                string PhysicalPath = string.Empty;

                VirtualDirectoryUrl = ConfigurationManager.AppSettings["PCI_INDEX_CHAINAGE_PHOTO_VIRTUAL_PATH"];
                PhysicalPath = ConfigurationManager.AppSettings["PCI_INDEX_CHAINAGE_PHOTO"];

                return ImageEntryList.Select(fileDetails => new
                {
                    id = fileDetails.FILE_ID,
                    cell = new[] {                                       
                                    //URLEncrypt.EncryptParameters(new string[] { fileDetails.FILE_NAME + "$" +  fileDetails.FILE_ID}),                                                                                                      
                                    Path.Combine(VirtualDirectoryUrl , fileDetails.FILE_NAME),
                                    fileDetails.REMARKS,
                                  "<a href='#' title='Click here to Edit the File Details' class='ui-icon ui-icon-pencil ui-align-center' onClick=EditPDFDetails('" + URLEncrypt.EncryptParameters1(new string[] { "FileID =" +                                                                  fileDetails.FILE_ID.ToString()}) +"'); return false;>Edit</a>",
                                  
                                  dbContext.MANE_CN_PCI_INDEX_PMGSY3.Where(a=> a.PCI_ID == fileDetails.PCI_ID && a.IS_FINALIZED != null).Any()? "N/A" :
                                  
                                  "<a href='#' title='Click here to delete the File and File Details' class='ui-icon ui-icon-trash ui-align-center' onClick=DeletePDFFileDetails('" + URLEncrypt.EncryptParameters1(new string[] { "FileID =" +                                                                  fileDetails.FILE_ID.ToString()}) +"'); return false;'>Delete</a>",
                                      
                                    
                                  "<center><table><tr><td style='border-color:white'><a href='#' style='float:left' id='btnSave"+  fileDetails.FILE_ID  +"' title='Click here to Save the File Details' class='ui-icon ui-icon-disk ui-align-center' onClick=SavePDFDetails('" +  URLEncrypt.EncryptParameters1(new string[] { "FileID =" +  fileDetails.FILE_ID.ToString()}) + "');></a><a href='#' style='float:right' id='btnCancel" + fileDetails.FILE_ID.ToString().Trim() +"' title='Click here to Cancel the File Edit' class='ui-icon ui-icon-closethick ui-align-center' onClick= CancelSavePDFDetails('" + URLEncrypt.EncryptParameters1(new string[] { "FileID =" +                                                                  fileDetails.FILE_ID.ToString()})  + "');></a></td></tr></table></center>"
                                        
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

        #endregion

        #region Finalize PCI PMGSY3 BLOCK/DISTRICT
        public Array GetBlockListPMGSY3DAL(int districtCode, int? page, int? rows, string sidx, string sord, out long totalRecords, string filters, ref bool isAllBlockFinalized)
        {
            PMGSYEntities dbContext = new PMGSYEntities();
            try
            {
                isAllBlockFinalized = false;

                JavaScriptSerializer js = null;
                PMGSY.Common.CommonFunctions.SearchJson test = new PMGSY.Common.CommonFunctions.SearchJson();
                string roadName = string.Empty;

                if (filters != null)
                {
                    js = new JavaScriptSerializer();
                    test = js.Deserialize<PMGSY.Common.CommonFunctions.SearchJson>(filters);

                    foreach (PMGSY.Common.CommonFunctions.rules item in test.rules)
                    {
                        switch (item.field)
                        {
                            case "MAST_ER_ROAD_NAME": roadName = item.data;
                                break;
                            default:
                                break;
                        }
                    }
                }


                var lstFacilityFinalizedBlocks = dbContext.MAST_PCI_BLOCK_PMGSY3_FINALIZE.Where(z => z.MASTER_BLOCK.MAST_DISTRICT_CODE == districtCode && z.MASTER_BLOCK.MAST_BLOCK_ACTIVE == "Y" && z.IS_FINALIZED == "Y").Select(x => new { MAST_BLOCK_CODE = x.MAST_BLOCK_CODE, MAST_BLOCK_NAME = x.MASTER_BLOCK.MAST_BLOCK_NAME }).Distinct().ToList();

                ///Get Block Names
                //var lstBlock = dbContext.MASTER_BLOCK.Where(c => c.MAST_DISTRICT_CODE == districtCode && c.MAST_BLOCK_ACTIVE == "Y").Select(x => new { MAST_BLOCK_CODE = x.MAST_BLOCK_CODE, MAST_BLOCK_NAME = x.MAST_BLOCK_NAME.Trim() }).OrderBy(z => z.MAST_BLOCK_CODE).Distinct().ToList();
                var lstBlock = dbContext.USP_CN_PCI_FINALIZE(PMGSYSession.Current.StateCode, districtCode, PMGSYSession.Current.PMGSYScheme).ToList();

                totalRecords = lstBlock.Count();

                //if (lstBlock.Count() == lstFacilityFinalizedBlocks.Count() && (!dbContext.MAST_PCI_DISTRICT_PMGSY3_FINALIZE.Any(z => z.MAST_DISTRICT_CODE == districtCode && z.IS_FINALIZED == "Y") ||
                //    !dbContext.MAST_PCI_DISTRICT_PMGSY3_FINALIZE.Any(z => z.MAST_DISTRICT_CODE == districtCode)))
                var lstDistrictFin = (from block in dbContext.MASTER_BLOCK
                                      join pci in dbContext.MAST_PCI_BLOCK_PMGSY3_FINALIZE on new { p1 = block.MAST_BLOCK_CODE, p2 = block.MAST_DISTRICT_CODE } equals new { p1 = (int)pci.MAST_BLOCK_CODE, p2 = (int)districtCode }
                                      into c
                                      from d in c.DefaultIfEmpty()
                                      where block.MAST_DISTRICT_CODE == districtCode && block.MAST_BLOCK_ACTIVE == "Y" && d.MAST_BLOCK_CODE == null && d.IS_FINALIZED == null
                                      //select new { Block_Code = block.MAST_BLOCK_CODE, PCI_Code = pci.MAST_BLOCK_CODE, pci.IS_FINALIZED }
                                      select new { block.MAST_BLOCK_CODE }
                                          ).Count();

                if ((!dbContext.MAST_PCI_DISTRICT_PMGSY3_FINALIZE.Any(z => z.MAST_DISTRICT_CODE == districtCode && z.IS_FINALIZED == "Y") ||
                    !dbContext.MAST_PCI_DISTRICT_PMGSY3_FINALIZE.Any(z => z.MAST_DISTRICT_CODE == districtCode)) && lstDistrictFin == 0)
                {
                    isAllBlockFinalized = true;
                }

                if (sidx.Trim() != string.Empty)
                {
                    if (sord.ToString() == "asc")
                    {
                        lstBlock = lstBlock.OrderBy(x => x.MAST_BLOCK_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                    }
                    else
                    {
                        lstBlock = lstBlock.OrderByDescending(x => x.MAST_BLOCK_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                    }
                }
                else
                {
                    lstBlock = lstBlock.OrderBy(x => x.MAST_BLOCK_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                }

                return lstBlock.Select((item, i) => new
                {
                    cell = new[]
                    {       
                        item.MAST_BLOCK_NAME.ToString(),
                        item.TOTAL_RECORDS.ToString(),
                        item.FINALIZED_COUNT.ToString(),
                        item.NON_FINALIZED_COUNT.ToString(),
                        item.IS_ELIGIBLE == "Y" ? "<a href='#' title='Click here to finalize details' class='ui-icon ui-icon-unlocked ui-align-center' onClick =FinalizeFacilityBlock('" +URLEncrypt.EncryptParameters1(new string[]{"BlockCode =" +                                                      item.MAST_BLOCK_CODE.ToString().Trim()}) +"'); return false;'></a>" : "-"

                        /*!dbContext.MAST_PCI_BLOCK_PMGSY3_FINALIZE.Where(x => x.MAST_BLOCK_CODE == item.MAST_BLOCK_CODE).Any() ?
                        (//(EntryListFromPCI.ElementAt(i) &&
                         //(!dbContext.PLAN_ROAD.Where(m=>m.MAST_BLOCK_CODE == item.MAST_BLOCK_CODE && m.PLAN_LOCK_STATUS == "N" && m.MAST_PMGSY_SCHEME == 4).Any())

                        ) ? 
                        "<a href='#' title='Click here to finalize details' class='ui-icon ui-icon-unlocked ui-align-center' onClick =FinalizeFacilityBlock('" +URLEncrypt.EncryptParameters1(new string[]{"BlockCode =" +                                                      item.MAST_BLOCK_CODE.ToString().Trim()}) +"'); return false;'></a>" 
                        : "PCI Entry against the Block is Pending") : "Finalized"*/
                    }
                }).ToArray();
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "GetBlockListPMGSY3DAL()");
                totalRecords = 0;
                return null;
            }
            finally
            {
                dbContext.Dispose();
            }
        }

        public bool FinalizePCIBlockPMGSY3DAL(int blockCode, ref string message)
        {
            PMGSYEntities dbContext = new PMGSYEntities();
            try
            {
                MAST_PCI_BLOCK_PMGSY3_FINALIZE FacilityPmgsy3Model = new MAST_PCI_BLOCK_PMGSY3_FINALIZE();
                dbContext = new PMGSYEntities();

                FacilityPmgsy3Model.MAST_PCI_BLOCK_FIN_CODE = (dbContext.MAST_PCI_BLOCK_PMGSY3_FINALIZE.Any() ? dbContext.MAST_PCI_BLOCK_PMGSY3_FINALIZE.Max(z => z.MAST_PCI_BLOCK_FIN_CODE) : 0) + 1;
                FacilityPmgsy3Model.MAST_BLOCK_CODE = blockCode;
                FacilityPmgsy3Model.MAST_PCI_BLOCK_FINALIZE_DATE = DateTime.Now;
                FacilityPmgsy3Model.IS_FINALIZED = "Y";

                FacilityPmgsy3Model.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                FacilityPmgsy3Model.USERID = PMGSYSession.Current.UserId;
                dbContext.MAST_PCI_BLOCK_PMGSY3_FINALIZE.Add(FacilityPmgsy3Model);
                dbContext.SaveChanges();
                message = "Block finalized successfully";
                return true;
            }
            catch (DbEntityValidationException e)
            {
                ErrorLog.LogError(e, "FinalizeFacilityBlockPMGSY3DAL().DbEntityValidationException");

                ModelStateDictionary modelstate = new ModelStateDictionary();
                foreach (var eve in e.EntityValidationErrors)
                {
                    foreach (var ve in eve.ValidationErrors)
                    {
                        modelstate.AddModelError(ve.ErrorMessage, ve.ErrorMessage);
                    }
                }

                using (System.IO.StreamWriter sw = System.IO.File.AppendText(System.IO.Path.Combine("D:\\ErrorLog\\OMMASErrorLog_" + System.DateTime.Now.Date.ToString("dd_MM_yyyy") + ".txt", "")))
                {
                    sw.WriteLine("Date :" + DateTime.Now.ToString());
                    sw.WriteLine("FinalizeFacilityBlockPMGSY3DAL().DbEntityValidationException()");
                    sw.WriteLine("Exception : " + new CommonFunctions().FormatErrorMessage(modelstate));
                    sw.WriteLine("---------------------------------------------------------------------------------------");
                    sw.Close();
                }
                return false;
            }
            catch (OptimisticConcurrencyException ex)
            {
                ErrorLog.LogError(ex, "FinalizeFacilityBlockPMGSY3DAL().OptimisticConcurrencyException");

                message = "An Error Occurred While Processing Your Request.";
                return false;
            }
            catch (UpdateException ex)
            {
                ErrorLog.LogError(ex, "FinalizeFacilityBlockPMGSY3DAL().UpdateException");
                message = "An Error Occurred While Processing Your Request.";
                return false;
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "FinalizeFacilityBlockPMGSY3DAL()");
                message = "An Error Occurred While Processing Your Request.";
                return false;
            }
            finally
            {
                dbContext.Dispose();
            }
        }

        public bool FinalizePCIDistrictPMGSY3DAL(int districtCode, ref string message)
        {
            PMGSYEntities dbContext = new PMGSYEntities();
            try
            {
                MAST_PCI_DISTRICT_PMGSY3_FINALIZE FacilityPmgsy3Model = new MAST_PCI_DISTRICT_PMGSY3_FINALIZE();
                dbContext = new PMGSYEntities();

                FacilityPmgsy3Model.MAST_PCI_DISTRICT_FIN_CODE = (dbContext.MAST_PCI_DISTRICT_PMGSY3_FINALIZE.Any() ? dbContext.MAST_PCI_DISTRICT_PMGSY3_FINALIZE.Max(z => z.MAST_PCI_DISTRICT_FIN_CODE) : 0) + 1;
                FacilityPmgsy3Model.MAST_DISTRICT_CODE = districtCode;
                FacilityPmgsy3Model.MAST_PCI_DISTRICT_FINALIZE_DATE = DateTime.Now;
                FacilityPmgsy3Model.IS_FINALIZED = "Y";

                FacilityPmgsy3Model.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                FacilityPmgsy3Model.USERID = PMGSYSession.Current.UserId;
                dbContext.MAST_PCI_DISTRICT_PMGSY3_FINALIZE.Add(FacilityPmgsy3Model);
                dbContext.SaveChanges();
                message = "District finalized successfully";
                return true;
            }
            catch (DbEntityValidationException e)
            {
                ErrorLog.LogError(e, "FinalizeFacilityDistrictPMGSY3DAL().DbEntityValidationException");

                ModelStateDictionary modelstate = new ModelStateDictionary();
                foreach (var eve in e.EntityValidationErrors)
                {
                    foreach (var ve in eve.ValidationErrors)
                    {
                        modelstate.AddModelError(ve.ErrorMessage, ve.ErrorMessage);
                    }
                }

                using (System.IO.StreamWriter sw = System.IO.File.AppendText(System.IO.Path.Combine("D:\\ErrorLog\\OMMASErrorLog_" + System.DateTime.Now.Date.ToString("dd_MM_yyyy") + ".txt", "")))
                {
                    sw.WriteLine("Date :" + DateTime.Now.ToString());
                    sw.WriteLine("FinalizeFacilityDistrictPMGSY3DAL().DbEntityValidationException()");
                    sw.WriteLine("Exception : " + new CommonFunctions().FormatErrorMessage(modelstate));
                    sw.WriteLine("---------------------------------------------------------------------------------------");
                    sw.Close();
                }
                return false;
            }
            catch (OptimisticConcurrencyException ex)
            {
                ErrorLog.LogError(ex, "FinalizeFacilityDistrictPMGSY3DAL().OptimisticConcurrencyException");
                message = "An Error Occurred While Processing Your Request.";
                return false;
            }
            catch (UpdateException ex)
            {
                ErrorLog.LogError(ex, "FinalizeFacilityDistrictPMGSY3DAL().UpdateException");
                message = "An Error Occurred While Processing Your Request.";
                return false;
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "FinalizeFacilityDistrictPMGSY3DAL()");
                message = "An Error Occurred While Processing Your Request.";
                return false;
            }
            finally
            {
                dbContext.Dispose();
            }
        }
        #endregion

        #region PCI Definalization ITNO

        public Array GetPmgsyRoadsDALITNO(int page, int rows, string sidx, string sord, out Int32 totalRecords, int MAST_STATE_CODE, int MAST_DISTRICT_CODE, int ADMIN_ND_CODE, int IMS_YEAR, int MAST_BLOCK_CODE)
        {
            PMGSYEntities dbContext = new Models.PMGSYEntities();

            try
            {
                List<IMS_SANCTIONED_PROJECTS> list_ims_sanctioned_project = (
                                                                                from c in dbContext.IMS_SANCTIONED_PROJECTS
                                                                                join d in dbContext.EXEC_ROADS_MONTHLY_STATUS
                                                                                on c.IMS_PR_ROAD_CODE equals d.IMS_PR_ROAD_CODE
                                                                                where
                                                                                    c.IMS_PR_ROAD_CODE == d.IMS_PR_ROAD_CODE &&
                                                                                    c.MAST_STATE_CODE == MAST_STATE_CODE &&
                                                                                    c.MAST_DISTRICT_CODE == MAST_DISTRICT_CODE &&
                                                                                    c.MAST_DPIU_CODE == ADMIN_ND_CODE &&
                                                                                    (MAST_BLOCK_CODE > 0 ? c.MAST_BLOCK_CODE : 1) == (MAST_BLOCK_CODE > 0 ? MAST_BLOCK_CODE : 1) &&
                                                                                    c.IMS_YEAR == IMS_YEAR &&
                                                                                    c.IMS_PROPOSAL_TYPE == "P" &&
                                                                                    (c.IMS_ISCOMPLETED == "C" || c.IMS_ISCOMPLETED == "X") &&
                                                                                    d.EXEC_ISCOMPLETED == "C" &&
                                                                                    c.MAST_PMGSY_SCHEME == PMGSYSession.Current.PMGSYScheme //new change done by Vikram on 10 Feb 2014
                                                                                select c
                                                                            ).ToList<IMS_SANCTIONED_PROJECTS>();
                totalRecords = list_ims_sanctioned_project.Count();
                if (sidx.Trim() != string.Empty)
                {
                    if (sord.ToString() == "asc")
                    {
                        list_ims_sanctioned_project = list_ims_sanctioned_project.OrderBy(x => x.IMS_ROAD_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                    }
                    else
                    {
                        list_ims_sanctioned_project = list_ims_sanctioned_project.OrderByDescending(x => x.IMS_ROAD_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                    }
                }
                else
                {
                    list_ims_sanctioned_project = list_ims_sanctioned_project.OrderBy(x => x.IMS_ROAD_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                }
                return list_ims_sanctioned_project.Select(propDetails => new
                {
                    id = URLEncrypt.EncryptParameters(new string[] { propDetails.IMS_PR_ROAD_CODE.ToString() }),
                    cell = new[] {                         
                                    (propDetails.IMS_ROAD_NAME == null || propDetails.IMS_ROAD_NAME == "" )? "NA" :  propDetails.IMS_ROAD_NAME,
                                    propDetails.IMS_PACKAGE_ID,
                                    propDetails.IMS_PAV_LENGTH.ToString(),                                                               
                                    (propDetails.MANE_IMS_PCI_INDEX.Where(a=> a.IMS_PR_ROAD_CODE == propDetails.IMS_PR_ROAD_CODE).Any()) ? propDetails.MANE_IMS_PCI_INDEX.Max(a=> a.MANE_PCI_YEAR).ToString() : "-",
                                    "<a href='#' class='ui-icon ui-icon-plusthick ui-align-center' onclick='AddPCIIndexForPmgsyRoad(\"" + URLEncrypt.EncryptParameters(new string[] { propDetails.IMS_PR_ROAD_CODE.ToString().Trim() }) + "\"); return false;'>CBR Details</a>" 
                   }
                }).ToArray();

            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "CoreNetworkDAL.GetPmgsyRoadsDALITNO()");
                totalRecords = 0;
                return null;
            }
            finally
            {
                dbContext.Dispose();
            }
        }

        //public Array GetCNRoadsDAL(int page, int rows, string sidx, string sord, out int totalRecords, int MAST_STATE_CODE, int MAST_DISTRICT_CODE, int ADMIN_ND_CODE, int IMS_BLOCK_ID , int IMS_YEAR)
        //{

        //    PMGSYEntities dbContext = new Models.PMGSYEntities();

        //    try
        //    {
        //        List<int> RoadCatagoryFilter = new List<int>();
        //        RoadCatagoryFilter.Add(1); // National Highway
        //        RoadCatagoryFilter.Add(2); // State Highway
        //        RoadCatagoryFilter.Add(3); // Major District Road

        //        var ListCNRoads = (
        //                                            from c in dbContext.PLAN_ROAD
        //                                            join e in dbContext.MASTER_EXISTING_ROADS
        //                                            on c.MAST_ER_ROAD_CODE equals e.MAST_ER_ROAD_CODE
        //                                            join f in dbContext.MASTER_ROAD_CATEGORY
        //                                            on e.MAST_ROAD_CAT_CODE equals f.MAST_ROAD_CAT_CODE

        //                                            join PRM in dbContext.PLAN_ROAD_MRL_PMGSY3 on
        //                                            c.PLAN_CN_ROAD_CODE equals PRM.PLAN_CN_ROAD_CODE
        //                                            into joinedtable from mappingTable in joinedtable.DefaultIfEmpty()

        //                                            where
        //                                            c.MAST_STATE_CODE == MAST_STATE_CODE &&
        //                                            c.MAST_DISTRICT_CODE == MAST_DISTRICT_CODE &&
        //                                            (IMS_BLOCK_ID > 0 ? c.MAST_BLOCK_CODE : 1) == (IMS_BLOCK_ID > 0 ? IMS_BLOCK_ID : 1) 
        //                                            //&&
        //                                            //!RoadCatagoryFilter.Contains(f.MAST_ROAD_CAT_CODE)
        //                                            && c.MAST_PMGSY_SCHEME == PMGSYSession.Current.PMGSYScheme //as per the change done in application for scheme 
        //                                            && e.MAST_CONS_YEAR == (IMS_YEAR == 0 ? e.MAST_CONS_YEAR : IMS_YEAR)

        //                                            select new 
        //                                            {
        //                                                c.PLAN_CN_ROAD_CODE,
        //                                                c.PLAN_CN_ROAD_NUMBER,
        //                                                c.PLAN_RD_NAME,
        //                                                c.MASTER_EXISTING_ROADS,
        //                                                c.PLAN_RD_FROM_CHAINAGE,
        //                                                c.PLAN_RD_TO_CHAINAGE,
        //                                            } 

        //                                       ).OrderBy(c => c.PLAN_CN_ROAD_NUMBER).ToList();

        //        var PLAN_ROAD_MRL_PMGSY3List = (from item in dbContext.PLAN_ROAD_MRL_PMGSY3
        //                                        where item.PLAN_ROAD.MAST_BLOCK_CODE == IMS_BLOCK_ID
        //                                        select new
        //                                        {
        //                                            PLAN_CN_ROAD_CODE = item.PLAN_CN_ROAD_CODE,
        //                                            PLAN_CN_ROAD_NUMBER = item.PLAN_ROAD.PLAN_CN_ROAD_NUMBER,
        //                                            PLAN_RD_NAME = item.MASTER_EXISTING_ROADS.MAST_ER_ROAD_NAME,
        //                                            MASTER_EXISTING_ROADS = item.MASTER_EXISTING_ROADS,
        //                                            PLAN_RD_FROM_CHAINAGE = item.PLAN_RD_FROM_CHAINAGE,
        //                                            PLAN_RD_TO_CHAINAGE = item.PLAN_RD_TO_CHAINAGE
        //                                        }).ToList();




        //        var UnionList = ListCNRoads.Union(PLAN_ROAD_MRL_PMGSY3List).OrderBy(x => x.PLAN_CN_ROAD_CODE).ToList();

        //        totalRecords = UnionList.Count();

        //        if (sidx.Trim() != string.Empty)
        //        {
        //            if (sord.ToString() == "asc")
        //            {
        //                UnionList = UnionList.OrderBy(c => c.PLAN_CN_ROAD_NUMBER).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
        //            }
        //            else
        //            {
        //                UnionList = UnionList.OrderByDescending(c => c.PLAN_CN_ROAD_NUMBER).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
        //            }
        //        }
        //        else
        //        {
        //            UnionList = UnionList.OrderBy(c => c.PLAN_CN_ROAD_NUMBER).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
        //        }

        //        UnionList.Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));

        //        return UnionList.Select(CNDetails => new
        //        {
        //            id = URLEncrypt.EncryptParameters(new string[] { CNDetails.PLAN_CN_ROAD_CODE.ToString() }),
        //            cell = new[] {    
        //                            CNDetails.PLAN_CN_ROAD_CODE.ToString(),
        //                            (CNDetails.PLAN_RD_NAME == null || CNDetails.PLAN_RD_NAME == "" ) ? "NA" :  CNDetails.PLAN_RD_NAME,
        //                            CNDetails.PLAN_CN_ROAD_NUMBER.ToString(),
        //                            CNDetails.MASTER_EXISTING_ROADS.MAST_ER_ROAD_NAME.Trim() + " (" + CNDetails.MASTER_EXISTING_ROADS.MAST_ER_ROAD_CODE.ToString() + ")",
        //                            CNDetails.MASTER_EXISTING_ROADS.MASTER_ROAD_CATEGORY.MAST_ROAD_CAT_NAME,
        //                            CNDetails.PLAN_RD_FROM_CHAINAGE .ToString(),
        //                            CNDetails.PLAN_RD_TO_CHAINAGE.ToString(),

        //                            (dbContext.MANE_CN_PCI_INDEX_PMGSY3.Where(a=> a.PLAN_CN_ROAD_CODE == CNDetails.PLAN_CN_ROAD_CODE).Any()) ? dbContext.MANE_CN_PCI_INDEX_PMGSY3.Max(a => a.MANE_PCI_YEAR).ToString() : "-",      

        //                            (dbContext.PLAN_ROAD_MRL_PMGSY3.AsEnumerable().Where(xx => xx.MAST_ER_ROAD_CODE.ToString().Equals(CNDetails.MASTER_EXISTING_ROADS.MAST_ER_ROAD_NAME.Trim()) 
        //                                && xx.PLAN_CN_ROAD_CODE.ToString().Equals(CNDetails.PLAN_CN_ROAD_CODE.ToString().Trim()) 
        //                                && xx.PLAN_LOCK_STATUS == "Y").Any()  ||


        //                            dbContext.MAST_CN_BLOCK_PMGSY3_FINALIZE.Where(xy => xy.MAST_BLOCK_CODE == IMS_BLOCK_ID && xy.IS_FINALIZED == "Y").Any())?
        //                            "<a href='#' class='ui-icon ui-icon-plusthick ui-align-center' onclick='AddPCIIndexForCNRoad(\"" + URLEncrypt.EncryptParameters(new string[] { CNDetails.PLAN_CN_ROAD_CODE.ToString() + "$" +                                                                CNDetails.MASTER_EXISTING_ROADS.MAST_ER_ROAD_CODE.ToString()}) + "\"); return false;'>PCI Index</a>" : "Road under TR/MRL or Block has not been finalized." ,

        //                            dbContext.MANE_CN_PCI_INDEX_PMGSY3.Where(x => x.PLAN_CN_ROAD_CODE == CNDetails.PLAN_CN_ROAD_CODE && x.IS_FINALIZED != null).Any() ? "Y" : "N"
        //                        }
        //        }).ToArray();
        //    }
        //    catch (Exception ex)
        //    {
        //        totalRecords = 0;
        //        return null;
        //    }
        //    finally
        //    {
        //        dbContext.Dispose();
        //    }
        //}

        //copy
        public Array GetCNRoadsDALITNO(int page, int rows, string sidx, string sord, out int totalRecords, int MAST_STATE_CODE, int MAST_DISTRICT_CODE, int ADMIN_ND_CODE, int IMS_BLOCK_ID, int IMS_YEAR)
        {

            PMGSYEntities dbContext = new Models.PMGSYEntities();

            try
            {
                List<int> RoadCatagoryFilter = new List<int>();
                RoadCatagoryFilter.Add(1); // National Highway
                RoadCatagoryFilter.Add(2); // State Highway
                RoadCatagoryFilter.Add(3); // Major District Road

                var ListCNRoads = dbContext.USP_PMGSY3_CN_PCI_ROADLIST(MAST_STATE_CODE, MAST_DISTRICT_CODE, IMS_BLOCK_ID, PMGSYSession.Current.PMGSYScheme).ToList();

                totalRecords = ListCNRoads.Count();

                if (sidx.Trim() != string.Empty)
                {
                    if (sord.ToString() == "asc")
                    {
                        ListCNRoads = ListCNRoads.OrderBy(c => c.PLAN_CN_ROAD_NUMBER).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                    }
                    else
                    {
                        ListCNRoads = ListCNRoads.OrderByDescending(c => c.PLAN_CN_ROAD_NUMBER).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                    }
                }
                else
                {
                    ListCNRoads = ListCNRoads.OrderBy(c => c.PLAN_CN_ROAD_NUMBER).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                }

                ListCNRoads.Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));

                return ListCNRoads.Select(CNDetails => new
                {
                    id = URLEncrypt.EncryptParameters(new string[] { CNDetails.PLAN_CN_ROAD_CODE.ToString() }),
                    cell = new[] {    
                                    CNDetails.PLAN_CN_ROAD_CODE.ToString(),
                                    (CNDetails.PLAN_RD_NAME == null || CNDetails.PLAN_RD_NAME == "" ) ? "NA" :  CNDetails.PLAN_RD_NAME,
                                    CNDetails.PLAN_CN_ROAD_NUMBER.ToString(),
                                    CNDetails.MAST_ER_ROAD_NAME.Trim() + " (" + CNDetails.MAST_ER_ROAD_CODE.ToString() + ")",
                                    CNDetails.MAST_ROAD_CAT_NAME,
                                    CNDetails.DRRP_START_CHAINAGE.ToString(),
                                    CNDetails.DRRP_END_CHAINAGE.ToString(),

                                    (dbContext.MANE_CN_PCI_INDEX_PMGSY3.Where(a=> a.PLAN_CN_ROAD_CODE == CNDetails.PLAN_CN_ROAD_CODE).Any()) ? dbContext.MANE_CN_PCI_INDEX_PMGSY3.Max(a => a.MANE_PCI_YEAR).ToString() : "-",      
                              
                                    (dbContext.PLAN_ROAD_MRL_PMGSY3.AsEnumerable().Where(xx => xx.MAST_ER_ROAD_CODE.ToString().Equals(CNDetails.MAST_ER_ROAD_CODE.ToString().Trim()) 
                                        && xx.PLAN_CN_ROAD_CODE.ToString().Equals(CNDetails.PLAN_CN_ROAD_CODE.ToString().Trim()) 
                                        && xx.PLAN_LOCK_STATUS == "Y").Any()  ||
                                    
                                    
                                    dbContext.MAST_CN_BLOCK_PMGSY3_FINALIZE.Where(xy => xy.MAST_BLOCK_CODE == IMS_BLOCK_ID && xy.IS_FINALIZED == "Y").Any()
                                    && dbContext.MAST_DRRP_BLOCK_PMGSY3_FINALIZE.Where(xy => xy.MAST_BLOCK_CODE == IMS_BLOCK_ID && xy.IS_FINALIZED == "Y").Any())?
                                    "<a href='#' title='Click Here to view PCI Entry' class='ui-icon ui-icon ui-icon-zoomin ui-align-center' onclick='AddPCIIndexForCNRoad(\"" + URLEncrypt.EncryptParameters(new string[]                                                                  { CNDetails.PLAN_CN_ROAD_CODE.ToString() + "$" + CNDetails.MAST_ER_ROAD_CODE.ToString()}) + "\"); return false;'>PCI Index</a>" : "Road under TR/MRL/DRRP or Block has not been finalized." ,

                                    //dbContext.MANE_CN_PCI_INDEX_PMGSY3.Where(x => x.PLAN_CN_ROAD_CODE == CNDetails.PLAN_CN_ROAD_CODE && x.IS_FINALIZED != null).Any() ? "Finalized" : "Not Finalized"
                                     dbContext.MANE_CN_PCI_INDEX_PMGSY3.Where(x => x.PLAN_CN_ROAD_CODE == CNDetails.PLAN_CN_ROAD_CODE && x.MAST_ER_ROAD_CODE == CNDetails.MAST_ER_ROAD_CODE && x.IS_FINALIZED != null).Any() ? "Finalized" 
                                    : "Not Finalized"
                                }
                }).ToArray();
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "CoreNetworkDAL.GetCNRoadsDALITNO()");
                totalRecords = 0;
                return null;
            }
            finally
            {
                dbContext.Dispose();
            }
        }

        public string SavePciForCNRoadDALITNO(PCIIndexViewModel pciIndexViewModel)
        {
            PMGSYEntities DbContext = new PMGSYEntities();

            try
            {

                PLAN_ROAD planCNRoad = DbContext.PLAN_ROAD.Where(x => x.PLAN_CN_ROAD_CODE == pciIndexViewModel.PLAN_CN_ROAD_CODE).FirstOrDefault();

                decimal chainage = 0;

                if (DbContext.MANE_CN_PCI_INDEX_PMGSY3.Any(m => m.PLAN_CN_ROAD_CODE == pciIndexViewModel.PLAN_CN_ROAD_CODE))
                {
                    foreach (var item in DbContext.MANE_CN_PCI_INDEX_PMGSY3.Where(m => m.PLAN_CN_ROAD_CODE == pciIndexViewModel.PLAN_CN_ROAD_CODE && m.MANE_PCI_YEAR == pciIndexViewModel.MANE_PCI_YEAR).ToList())
                    {
                        chainage += (item.MANE_END_CHAIN - item.MANE_STR_CHAIN);
                    }
                }

                if (chainage == null)
                {
                    chainage = 0;
                }

                chainage = chainage + (pciIndexViewModel.MANE_END_CHAIN - pciIndexViewModel.MANE_STR_CHAIN);
                //if (planCNRoad != null && planCNRoad.PLAN_RD_TOTAL_LEN != null && planCNRoad.PLAN_RD_TOTAL_LEN != 0)
                //{
                //    if (chainage > planCNRoad.PLAN_RD_TOTAL_LEN.Value)
                //    {
                //        return "Sum of Chainage is exceeding the Road Length.";
                //    }
                //}
                //else
                //{
                //    if (chainage > (DbContext.PLAN_ROAD.Where(m => m.PLAN_CN_ROAD_CODE == pciIndexViewModel.PLAN_CN_ROAD_CODE).Select(m => m.PLAN_RD_TO_CHAINAGE).FirstOrDefault() - DbContext.PLAN_ROAD.Where(m => m.PLAN_CN_ROAD_CODE == pciIndexViewModel.PLAN_CN_ROAD_CODE).Select(m => m.PLAN_RD_FROM_CHAINAGE).FirstOrDefault()))
                //    {
                //        return "Sum of Chainage is exceeding the Road Length.";
                //    }
                //}

                int MaxSegmentNumber = 0;
                int MaxID = 0;

                if (DbContext.MANE_CN_PCI_INDEX_PMGSY3.Where(a => a.PLAN_CN_ROAD_CODE == pciIndexViewModel.PLAN_CN_ROAD_CODE && a.MANE_PCI_YEAR == pciIndexViewModel.MANE_PCI_YEAR).Any())
                {
                    MaxSegmentNumber = DbContext.MANE_CN_PCI_INDEX_PMGSY3.Where(a => a.PLAN_CN_ROAD_CODE == pciIndexViewModel.PLAN_CN_ROAD_CODE && a.MANE_PCI_YEAR == pciIndexViewModel.MANE_PCI_YEAR).Select(a => a.MANE_SEGMENT_NO).Max();
                }

                if (DbContext.MANE_CN_PCI_INDEX_PMGSY3.Any())
                {
                    MaxID = DbContext.MANE_CN_PCI_INDEX_PMGSY3.Select(a => a.PCI_ID).Max();
                }
                MaxSegmentNumber++;
                MaxID++;

                MANE_CN_PCI_INDEX_PMGSY3 mane_cn_pci_index = new MANE_CN_PCI_INDEX_PMGSY3();
                mane_cn_pci_index.PCI_ID = MaxID;
                mane_cn_pci_index.PLAN_CN_ROAD_CODE = pciIndexViewModel.PLAN_CN_ROAD_CODE;
                mane_cn_pci_index.MAST_ER_ROAD_CODE = pciIndexViewModel.ER_ROAD_CODE;
                mane_cn_pci_index.MANE_SEGMENT_NO = MaxSegmentNumber;
                mane_cn_pci_index.MANE_PCI_YEAR = pciIndexViewModel.MANE_PCI_YEAR;
                mane_cn_pci_index.MANE_PCIINDEX = pciIndexViewModel.MANE_PCIINDEX;
                mane_cn_pci_index.MANE_STR_CHAIN = pciIndexViewModel.MANE_STR_CHAIN;
                mane_cn_pci_index.MANE_END_CHAIN = pciIndexViewModel.MANE_END_CHAIN;
                mane_cn_pci_index.MANE_SURFACE_TYPE = pciIndexViewModel.MANE_SURFACE_TYPE;
                mane_cn_pci_index.MANE_PCI_DATE = Convert.ToDateTime(pciIndexViewModel.MANE_PCI_DATE);

                mane_cn_pci_index.USERID = PMGSYSession.Current.UserId;
                mane_cn_pci_index.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                DbContext.MANE_CN_PCI_INDEX_PMGSY3.Add(mane_cn_pci_index);
                DbContext.SaveChanges();
                return string.Empty;
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "CoreNetworkDAL/SavePciForCNRoadDAL");
                return "Error Occured while processing your request.";
            }
            finally
            {
                DbContext.Dispose();
            }
        }

        public Array GetPCIListForCNRoadDALITNO(int page, int rows, string sidx, string sord, out Int32 totalRecords, int PLAN_CN_ROAD_CODE, int ER_ROAD_CODE)
        {
            PMGSYEntities dbContext = new Models.PMGSYEntities();

            try
            {
                var lstPciIndex = (from c in dbContext.MANE_CN_PCI_INDEX_PMGSY3
                                   where c.MAST_ER_ROAD_CODE == ER_ROAD_CODE
                                   && c.PLAN_CN_ROAD_CODE == PLAN_CN_ROAD_CODE
                                   select new
                                   {
                                       PciID = c.PCI_ID,
                                       Plan_cn_road_code = (Nullable<int>)c.PLAN_CN_ROAD_CODE,
                                       SegmentNo = (Nullable<int>)c.MANE_SEGMENT_NO,
                                       Mane_Pci_Year = (Nullable<int>)c.MANE_PCI_YEAR,
                                       Mane_Str_Chain = (Decimal?)c.MANE_STR_CHAIN,
                                       Mane_End_Chain = (Decimal?)c.MANE_END_CHAIN,
                                       Mane_Pci_Index = c.MANE_PCIINDEX,
                                       Mane_Surface_Type = c.MANE_SURFACE_TYPE,
                                       Mane_Pci_Date = c.MANE_PCI_DATE,
                                       isFinalized = c.IS_FINALIZED,
                                       FinalizedDate = c.FINALIZED_DATE,
                                       UserID = c.USERID,
                                       IPADD = c.IPADD,
                                       MasterSurfaceCode = c.MASTER_SURFACE.MAST_SURFACE_CODE,
                                       MasterSurfaceName = c.MASTER_SURFACE.MAST_SURFACE_NAME
                                   }).OrderByDescending(x => x.Mane_Pci_Year).ToList();


                totalRecords = lstPciIndex.Count();

                //lstPciIndex.OrderByDescending(x => x.MANE_PCI_YEAR).Skip(Convert.ToInt32(page * rows)).Take(rows).ToList<MANE_CN_PCI_INDEX>();

                if (sidx.Trim() != string.Empty)
                {
                    if (sord.ToString() == "asc")
                    {
                        lstPciIndex = lstPciIndex.OrderBy(x => x.Mane_Pci_Year).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                    }
                    else
                    {
                        lstPciIndex = lstPciIndex.OrderByDescending(x => x.Mane_Pci_Year).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                    }
                }
                else
                {
                    lstPciIndex = lstPciIndex.OrderBy(x => x.Mane_Pci_Year).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                }



                return lstPciIndex.Select(PciDetails => new
                {
                    id = URLEncrypt.EncryptParameters(new string[] { PciDetails.Plan_cn_road_code.ToString() + "$" + PciDetails.SegmentNo + "$" + PciDetails.Mane_Pci_Year }),
                    cell = new[] {                         
                                    PciDetails.Mane_Pci_Year.ToString(),
                                    PciDetails.SegmentNo.ToString(),
                                    Convert.ToString(PciDetails.Mane_Str_Chain),
                                    Convert.ToString(PciDetails.Mane_End_Chain),
                                    PciDetails.Mane_Pci_Index.ToString(),
                                    PciDetails.Mane_Surface_Type == null ? "" : PciDetails.MasterSurfaceName,
                                    PciDetails.Mane_Pci_Date != null ? Convert.ToDateTime(PciDetails.Mane_Pci_Date).ToString("dd-MMM-yyyy") : "NA",                                  
                                    
                                    dbContext.MANE_CN_PCI_INDEX_PMGSY3.Where(a=> a.PLAN_CN_ROAD_CODE == PciDetails.Plan_cn_road_code && a.IS_FINALIZED != null).Any() ? "N/A" :
                                    
                                    (dbContext.MANE_PCI_IMAGE_MAPPING_PMGSY3.Where(x => x.PCI_ID == PciDetails.PciID).Any() ? "Photograph(s) against the chainage are uploaded" :
                                   
                                    ((dbContext.MANE_CN_PCI_INDEX_PMGSY3.Where(a=> a.PLAN_CN_ROAD_CODE == PciDetails.Plan_cn_road_code && a.MANE_PCI_YEAR == PciDetails.Mane_Pci_Year).Count(obj => obj.IS_FINALIZED == null)  == 1) ? 
                                    ("<a href='#' title='Click here to delete Road Details' class='ui-icon ui-icon-trash ui-align-center' onClick='DeletePciForCNRoadDetails(\"" +
                                    URLEncrypt.EncryptParameters1(new string[] { "PCIID =" + PciDetails.PciID.ToString()}) +"\"); return false;'>Show Details</a>") : 
                                    
                                    (dbContext.MANE_CN_PCI_INDEX_PMGSY3.Where(a=> a.PLAN_CN_ROAD_CODE == PciDetails.Plan_cn_road_code && a.MANE_PCI_YEAR == PciDetails.Mane_Pci_Year).Select(a=> a.MANE_END_CHAIN).Max() == PciDetails.Mane_End_Chain )  ?                                     
                                    "<a href='#' title='Click here to delete Road Details' class='ui-icon ui-icon-trash ui-align-center' onClick='DeletePciForCNRoadDetails(\"" +
                                    URLEncrypt.EncryptParameters1(new string[] { "PCIID =" + PciDetails.PciID.ToString()}) +"\"); return false;'>Show Details</a>"   :  "<a href='#' title='N/A' class='ui-icon ui-icon-locked ui-align-center'                                                 onClick='return false;'")),
                                    
                    "<center><span class='ui-icon ui-icon-image' title='Click here to upload photograph' onClick ='UploadChainagePhotoGraph(\"" + URLEncrypt.EncryptParameters1(new string[] { "PCIID =" +                                                                  PciDetails.PciID.ToString()}) + 
                    "\");'></span></center>"
                   }
                }).ToArray();
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "CoreNetworkDAL.GetPCIListForCNRoadDALITNO");
                totalRecords = 0;
                return null;
            }
            finally
            {
                if (dbContext != null)
                    dbContext.Dispose();
            }
        }

        public bool SavePhotoGraphDALITNO(int PCIID, string FileName, HttpPostedFileBase filebase, string remark)
        {
            PMGSY.Models.PMGSYEntities dbcontext = new Models.PMGSYEntities();
            try
            {
                MANE_PCI_IMAGE_MAPPING_PMGSY3 dbmodel = new MANE_PCI_IMAGE_MAPPING_PMGSY3();
                int maxid = 0;
                if (dbcontext.MANE_PCI_IMAGE_MAPPING_PMGSY3.Any())
                    maxid = dbcontext.MANE_PCI_IMAGE_MAPPING_PMGSY3.Select(x => x.FILE_ID).Max() + 1;

                else
                    maxid = 1;

                var Entry = dbcontext.MANE_CN_PCI_INDEX_PMGSY3.Where(x => x.PCI_ID == PCIID).FirstOrDefault();
                var RoadCode = Entry.PLAN_CN_ROAD_CODE;
                var ErCode = Entry.MAST_ER_ROAD_CODE;

                if (dbmodel != null)
                {

                    var timestampArray = System.DateTime.Now.ToString("MM/dd/yyyy hh:mm:ss.fff").Replace('.', '_').Split(' ');
                    string timestamp = timestampArray[0].ToString() + timestampArray[1].ToString();
                    FileName = ErCode.ToString() + "_" + timestamp.Replace('/', '_').Replace(':', '_') + "_" + RoadCode.ToString() + ".jpeg";
                    bool isEntrySaved = false;
                    using (TransactionScope ts = new TransactionScope())
                    {
                        dbmodel.FILE_ID = maxid;
                        dbmodel.PCI_ID = PCIID;
                        dbmodel.FILE_NAME = FileName;
                        dbmodel.FILE_UPLOAD_DATE = System.DateTime.Now;
                        dbmodel.REMARKS = remark;
                        dbmodel.USERID = PMGSYSession.Current.UserId;
                        dbmodel.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];

                        dbcontext.Entry(dbmodel).State = EntityState.Added;

                        dbcontext.SaveChanges();
                        ts.Complete();
                        isEntrySaved = true;
                    }

                    if (isEntrySaved)
                        filebase.SaveAs(Path.Combine(ConfigurationManager.AppSettings["PCI_INDEX_CHAINAGE_PHOTO"], FileName));

                    return true;

                }
                return false;
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "MasterEntryDAL/SavePhotoGraph");
                return false;
            }
        }

        public Array GetImageFilesListDALITNO(int page, int rows, string sidx, string sord, out int totalRecords, int PCIid)
        {
            PMGSYEntities dbContext = new PMGSYEntities();
            try
            {
                var ImageEntryList = dbContext.MANE_PCI_IMAGE_MAPPING_PMGSY3.Where(x => x.PCI_ID == PCIid).ToList();

                totalRecords = ImageEntryList.Count();

                if (sidx.Trim() != string.Empty)
                {
                    if (sord.ToString() == "asc")
                    {
                        ImageEntryList = ImageEntryList.OrderBy(x => x.FILE_ID).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                    }
                    else
                    {
                        ImageEntryList = ImageEntryList.OrderByDescending(x => x.FILE_ID).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                    }
                }
                else
                {
                    ImageEntryList = ImageEntryList.OrderBy(x => x.FILE_ID).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                }

                string VirtualDirectoryUrl = string.Empty;
                string PhysicalPath = string.Empty;

                VirtualDirectoryUrl = ConfigurationManager.AppSettings["PCI_INDEX_CHAINAGE_PHOTO_VIRTUAL_PATH"];
                PhysicalPath = ConfigurationManager.AppSettings["PCI_INDEX_CHAINAGE_PHOTO"];

                return ImageEntryList.Select(fileDetails => new
                {
                    id = fileDetails.FILE_ID,
                    cell = new[] {                                       
                                    //URLEncrypt.EncryptParameters(new string[] { fileDetails.FILE_NAME + "$" +  fileDetails.FILE_ID}),                                                                                                      
                                    Path.Combine(VirtualDirectoryUrl , fileDetails.FILE_NAME),
                                    fileDetails.REMARKS,
                                  "<a href='#' title='Click here to Edit the File Details' class='ui-icon ui-icon-pencil ui-align-center' onClick=EditPDFDetails('" + URLEncrypt.EncryptParameters1(new string[] { "FileID =" +                                                                  fileDetails.FILE_ID.ToString()}) +"'); return false;>Edit</a>",
                                  
                                  dbContext.MANE_CN_PCI_INDEX_PMGSY3.Where(a=> a.PCI_ID == fileDetails.PCI_ID && a.IS_FINALIZED != null).Any()? "N/A" :
                                  
                                  "<a href='#' title='Click here to delete the File and File Details' class='ui-icon ui-icon-trash ui-align-center' onClick=DeletePDFFileDetails('" + URLEncrypt.EncryptParameters1(new string[] { "FileID =" +                                                                  fileDetails.FILE_ID.ToString()}) +"'); return false;'>Delete</a>",
                                      
                                    
                                  "<center><table><tr><td style='border-color:white'><a href='#' style='float:left' id='btnSave"+  fileDetails.FILE_ID  +"' title='Click here to Save the File Details' class='ui-icon ui-icon-disk ui-align-center' onClick=SavePDFDetails('" +  URLEncrypt.EncryptParameters1(new string[] { "FileID =" +  fileDetails.FILE_ID.ToString()}) + "');></a><a href='#' style='float:right' id='btnCancel" + fileDetails.FILE_ID.ToString().Trim() +"' title='Click here to Cancel the File Edit' class='ui-icon ui-icon-closethick ui-align-center' onClick= CancelSavePDFDetails('" + URLEncrypt.EncryptParameters1(new string[] { "FileID =" +                                                                  fileDetails.FILE_ID.ToString()})  + "');></a></td></tr></table></center>"
                                        
                                  }
                }).ToArray();
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "CoreNetworkDAL.GetImageFilesListDALITNO");
                totalRecords = 0;
                return null;
            }
            finally
            {
                dbContext.Dispose();
            }

        }

        #endregion

        #region Definalization TR/MRL ITNO
        public Array GetCoreNetWorksListPMGSY3DALITNO(int stateCode, int districtCode, int blockCode, string roadType, int roadCode, string roadName, int page, int rows, string sidx, string sord, out long totalRecords)
        {
            string filters = string.Empty;
            string nameSearch = string.Empty;
            PMGSYEntities dbContext = new PMGSYEntities();
            try
            {
                // new change with data from stored procedure
                if (roadType == "0")
                {
                    roadType = null;
                }

                if (PMGSYSession.Current.RoleCode == 2)
                {
                    stateCode = PMGSYSession.Current.StateCode;
                }
                else if (PMGSYSession.Current.RoleCode == 22)
                {
                    stateCode = PMGSYSession.Current.StateCode;
                    districtCode = PMGSYSession.Current.DistrictCode;
                }

                short roleCode = dbContext.UM_User_Master.Where(m => m.UserID == PMGSYSession.Current.UserId).Select(m => m.DefaultRoleID).FirstOrDefault();

                var lstPlanRoads = dbContext.GET_CORE_NETWORKS((stateCode <= 0 ? 0 : stateCode), (districtCode <= 0 ? 0 : districtCode), (blockCode <= 0 ? 0 : blockCode), roadType, roadCode, roadName, roleCode).ToList();

                lstPlanRoads = lstPlanRoads.Where(m => m.MAST_PMGSY_SCHEME == 4).ToList();

                totalRecords = lstPlanRoads.Count();

                if (sidx.Trim() != string.Empty)
                {
                    if (sord.ToString() == "asc")
                    {
                        switch (sidx)
                        {
                            case "PLAN_CN_ROAD_CODE":
                                lstPlanRoads = lstPlanRoads.OrderBy(x => x.PLAN_CN_ROAD_CODE).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                            case "PLAN_CN_ROAD_NUMBER":
                                lstPlanRoads = lstPlanRoads.OrderBy(x => x.PLAN_CN_ROAD_NUMBER).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                            //case "MAST_ER_ROAD_CODE":
                            //    lstPlanRoads = lstPlanRoads.OrderBy(x => x.MAST_ER_ROAD_CODE).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                            //    break;
                            case "PLAN_RD_NAME":
                                lstPlanRoads = lstPlanRoads.OrderBy(x => x.PLAN_RD_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                            case "PLAN_RD_FROM":
                                lstPlanRoads = lstPlanRoads.OrderBy(x => x.PLAN_RD_FROM).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                            case "PLAN_RD_TO":
                                lstPlanRoads = lstPlanRoads.OrderBy(x => x.PLAN_RD_TO).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                            case "PLAN_RD_FROM_CHAINAGE":
                                lstPlanRoads = lstPlanRoads.OrderBy(x => x.PLAN_RD_FROM_CHAINAGE).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                            case "PLAN_RD_TO_CHAINAGE":
                                lstPlanRoads = lstPlanRoads.OrderBy(x => x.PLAN_RD_TO_CHAINAGE).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                            case "PLAN_RD_LENGTH":
                                lstPlanRoads = lstPlanRoads.OrderBy(x => x.PLAN_RD_LENGTH).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                            case "PLAN_RD_TOTAL_LENGTH":
                                lstPlanRoads = lstPlanRoads.OrderBy(x => x.PLAN_RD_TOTAL_LEN).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                            case "PLAN_RD_ROUTE":
                                lstPlanRoads = lstPlanRoads.OrderBy(x => x.PLAN_RD_ROUTE).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                            default:
                                lstPlanRoads = lstPlanRoads.OrderBy(x => x.PLAN_CN_ROAD_CODE).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                        }
                    }
                    else
                    {
                        switch (sidx)
                        {
                            case "PLAN_CN_ROAD_CODE":
                                lstPlanRoads = lstPlanRoads.OrderByDescending(x => x.PLAN_CN_ROAD_CODE).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                            case "PLAN_CN_ROAD_NUMBER":
                                lstPlanRoads = lstPlanRoads.OrderByDescending(x => x.PLAN_CN_ROAD_NUMBER).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                            //case "MAST_ER_ROAD_CODE":
                            //    lstPlanRoads = lstPlanRoads.OrderByDescending(x => x.MAST_ER_ROAD_CODE).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                            //    break;
                            case "PLAN_RD_NAME":
                                lstPlanRoads = lstPlanRoads.OrderByDescending(x => x.PLAN_RD_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                            case "PLAN_RD_FROM":
                                lstPlanRoads = lstPlanRoads.OrderByDescending(x => x.PLAN_RD_FROM).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                            case "PLAN_RD_TO":
                                lstPlanRoads = lstPlanRoads.OrderByDescending(x => x.PLAN_RD_TO).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                            case "PLAN_RD_FROM_CHAINAGE":
                                lstPlanRoads = lstPlanRoads.OrderByDescending(x => x.PLAN_RD_FROM_CHAINAGE).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                            case "PLAN_RD_TO_CHAINAGE":
                                lstPlanRoads = lstPlanRoads.OrderByDescending(x => x.PLAN_RD_TO_CHAINAGE).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                            case "PLAN_RD_LENGTH":
                                lstPlanRoads = lstPlanRoads.OrderByDescending(x => x.PLAN_RD_LENGTH).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                            case "PLAN_RD_TOTAL_LENGTH":
                                lstPlanRoads = lstPlanRoads.OrderByDescending(x => x.PLAN_RD_TOTAL_LEN).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                            case "PLAN_RD_ROUTE":
                                lstPlanRoads = lstPlanRoads.OrderByDescending(x => x.PLAN_RD_ROUTE).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                            default:
                                lstPlanRoads = lstPlanRoads.OrderByDescending(x => x.PLAN_CN_ROAD_CODE).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                        }
                    }
                }
                else
                {
                    lstPlanRoads = lstPlanRoads.OrderByDescending(x => x.PLAN_CN_ROAD_CODE).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                }

                var result = lstPlanRoads.Select(roadDetails => new
                {
                    //roadDetails.MAST_ER_ROAD_NUMBER,
                    roadDetails.BLOCK_NAME,
                    roadDetails.DISTRICT_NAME,
                    //roadDetails.ER,
                    roadDetails.STATE_NAME,
                    roadDetails.PLAN_CN_ROAD_CODE,
                    roadDetails.PLAN_CN_ROAD_NUMBER,
                    //roadDetails.PLAN_LOCK_STATUS,
                    roadDetails.PLAN_RD_BLOCK_FROM_CODE,
                    roadDetails.PLAN_RD_BLOCK_TO_CODE,
                    roadDetails.PLAN_RD_FROM_CHAINAGE,
                    roadDetails.PLAN_RD_FROM_HAB,
                    roadDetails.PLAN_RD_FROM_TYPE,
                    roadDetails.PLAN_RD_LENG,
                    roadDetails.PLAN_RD_LENGTH,
                    roadDetails.PLAN_RD_NAME,
                    roadDetails.PLAN_RD_NUM_FROM,
                    roadDetails.PLAN_RD_NUM_TO,
                    roadDetails.PLAN_RD_ROUTE,
                    roadDetails.PLAN_RD_TO_CHAINAGE,
                    roadDetails.PLAN_RD_TO_HAB,
                    roadDetails.PLAN_RD_TO_TYPE,
                    roadDetails.PLAN_RD_FROM,
                    roadDetails.PLAN_RD_TO,
                    roadDetails.UNLOCK_BY_MORD,
                    roadDetails.MAST_ER_ROAD_NUMBER,
                    roadDetails.PLAN_RD_TOTAL_LEN
                }).OrderBy(x => x.DISTRICT_NAME).ToArray();

                return result.Select(roadDetails => new
                {
                    id = URLEncrypt.EncryptParameters1(new string[] { "NetworkCode=" + roadDetails.PLAN_CN_ROAD_CODE.ToString() }),
                    cell = new[]
                {
                    roadDetails.PLAN_CN_ROAD_CODE.ToString().Trim(),
                    roadDetails.DISTRICT_NAME,
                    roadDetails.BLOCK_NAME,
                    roadDetails.PLAN_RD_ROUTE == null?string.Empty:roadDetails.PLAN_RD_ROUTE.ToString(),
                    roadDetails.PLAN_CN_ROAD_NUMBER == null?string.Empty:roadDetails.PLAN_CN_ROAD_NUMBER.ToString(),
                    roadDetails.PLAN_RD_NAME == null?string.Empty:roadDetails.PLAN_RD_NAME.ToString(),
                    roadDetails.MAST_ER_ROAD_NUMBER.ToString(),
                    roadDetails.PLAN_RD_FROM_TYPE=="B"?"Block("+(dbContext.MASTER_BLOCK.Where(item=>item.MAST_BLOCK_CODE == roadDetails.PLAN_RD_BLOCK_FROM_CODE).Select(m=>m.MAST_BLOCK_NAME).FirstOrDefault())+")":(roadDetails.PLAN_RD_FROM_TYPE=="H"?"Habitation("+(dbContext.MASTER_HABITATIONS.Where(item=>item.MAST_HAB_CODE == roadDetails.PLAN_RD_FROM_HAB).Select(m=>m.MAST_HAB_NAME).FirstOrDefault())+")":(roadDetails.PLAN_RD_FROM==null?string.Empty:roadDetails.PLAN_RD_FROM.ToString())),
                    roadDetails.PLAN_RD_TO_TYPE=="B"?"Block("+(dbContext.MASTER_BLOCK.Where(item=>item.MAST_BLOCK_CODE == roadDetails.PLAN_RD_BLOCK_TO_CODE).Select(m=>m.MAST_BLOCK_NAME).FirstOrDefault())+")":(roadDetails.PLAN_RD_TO_TYPE=="H"?"Habitation("+(dbContext.MASTER_HABITATIONS.Where(item=>item.MAST_HAB_CODE == roadDetails.PLAN_RD_TO_HAB).Select(m=>m.MAST_HAB_NAME).FirstOrDefault())+")":(roadDetails.PLAN_RD_TO==null?string.Empty:roadDetails.PLAN_RD_TO.ToString())),

                    roadDetails.PLAN_RD_FROM_CHAINAGE == null?string.Empty:roadDetails.PLAN_RD_FROM_CHAINAGE.ToString(),
                    roadDetails.PLAN_RD_TO_CHAINAGE == null?string.Empty:roadDetails.PLAN_RD_TO_CHAINAGE.ToString(),
                    roadDetails.PLAN_RD_LENGTH == null?string.Empty:roadDetails.PLAN_RD_LENGTH.ToString(),
                     (roadDetails.PLAN_RD_TOTAL_LEN == null ? Convert.ToString(roadDetails.PLAN_RD_TO_CHAINAGE - roadDetails.PLAN_RD_FROM_CHAINAGE) :  roadDetails.PLAN_RD_TOTAL_LEN.ToString()),

                     //Map Habitation
                    roadDetails.UNLOCK_BY_MORD == "M"?"<a href='#' title='Click here to map habitation Details' class='ui-icon ui-icon-unlocked ui-align-center' onClick =editHabitationDetails('"+URLEncrypt.EncryptParameters1(new string[]{"NetworkCode =" + roadDetails.PLAN_CN_ROAD_CODE.ToString().Trim(),"UnlockFlag =" + roadDetails.UNLOCK_BY_MORD.ToString().Trim()})+"'); return false;'>Delete</a>":roadDetails.UNLOCK_BY_MORD=="N"?"<a href='#' title='Click here to map habitation Details' class='ui-icon ui-icon-plusthick ui-align-center' onClick =editHabitationDetails('"+URLEncrypt.EncryptParameters1(new string[]{"NetworkCode =" + roadDetails.PLAN_CN_ROAD_CODE.ToString().Trim(),"UnlockFlag =" + roadDetails.UNLOCK_BY_MORD.ToString().Trim()})+"'); return false;'>Delete</a>":"<span class='ui-icon ui-icon-locked ui-align-center'></span>",
                    
                    //Upload
                    ///Changes by SAMMED A. PATIL on 27JULY2017 to lock Habitation mapping if CoreNetwork is locked uncommented above line
                    //"<a href='#' title='Click here to map habitation Details' class='ui-icon ui-icon-plusthick ui-align-center' onClick =editHabitationDetails('"+URLEncrypt.EncryptParameters1(new string[]{"NetworkCode =" + roadDetails.PLAN_CN_ROAD_CODE.ToString().Trim(),"UnlockFlag =" + roadDetails.UNLOCK_BY_MORD.ToString().Trim()})+"'); return false;'>Delete</a>",
                    (roadDetails.UNLOCK_BY_MORD == "M"?"<a href='#' title='Click here to upload file details' class='ui-icon ui-icon-unlocked ui-align-center' onClick =UploadFile('"+URLEncrypt.EncryptParameters1(new string[]{"NetworkCode =" + roadDetails.PLAN_CN_ROAD_CODE.ToString().Trim()})+"'); return false;'>Delete</a>":roadDetails.UNLOCK_BY_MORD=="N"?"<a href='#' title='Click here to upload file details' class='ui-icon ui-icon-plusthick ui-align-center' onClick =UploadFile('"+URLEncrypt.EncryptParameters1(new string[]{"NetworkCode =" + roadDetails.PLAN_CN_ROAD_CODE.ToString().Trim()})+"'); return false;'>Delete</a>":"<span class='ui-icon ui-icon-locked ui-align-center'></span></center>"),
                    
                    //Edit
                    //PMGSYSession.Current.PMGSYScheme == 1?"":roadDetails.UNLOCK_BY_MORD == "M"?("<a href='#' title='Click here to map other TR/MRL road' class='ui-icon ui-icon-unlocked ui-align-center' onClick =MapOtherCandidateRoad('"+URLEncrypt.EncryptParameters1(new string[]{"NetworkCode =" + roadDetails.PLAN_CN_ROAD_CODE.ToString().Trim()})+"'); return false;'>Map Other DRRP Road</a>"):roadDetails.UNLOCK_BY_MORD == "Y"?"<span class='ui-icon ui-icon-locked ui-align-center'></span>":"<a href='#' title='Click here to map other TR/MRL road' class='ui-icon ui-icon-plusthick ui-align-center' onClick =MapOtherCandidateRoad('"+URLEncrypt.EncryptParameters1(new string[]{"NetworkCode =" + roadDetails.PLAN_CN_ROAD_CODE.ToString().Trim()})+"'); return false;'>Map Other DRRP Road</a>",
                    
                    ("<a href='#' title='Click here to map other TR/MRL road' class='ui-icon ui-icon-unlocked ui-align-center' onClick =MapOtherCandidateRoad('"+URLEncrypt.EncryptParameters1(new string[]{"NetworkCode =" +                                               roadDetails.PLAN_CN_ROAD_CODE.ToString().Trim()})+"'); return false;'>Map Other DRRP Road</a>"),

                    //Delete
                    "<a href='#' title='Click here to view core network details' class='ui-icon ui-icon-zoomin ui-align-center' onClick =detailsCoreNetwork('"+URLEncrypt.EncryptParameters1(new string[]{"NetworkCode =" + roadDetails.PLAN_CN_ROAD_CODE.ToString().Trim()})+"'); return false;'>Delete</a>",
                    
                    ///Changes by SAMMED A. PATIL on 03 OCTOBER 2017 to edit DRRP for MORD
                    PMGSYSession.Current.RoleCode == 25 
                        ? "<a href='#' title='Click here to view core network details' class='ui-icon ui-icon-pencil ui-align-center' onClick =editNetworkData('"+URLEncrypt.EncryptParameters1(new string[]{"NetworkCode =" + roadDetails.PLAN_CN_ROAD_CODE.ToString().Trim()})+"'); return false;'>Edit</a>"
                        : (roadDetails.UNLOCK_BY_MORD == "M"?"<a href='#' title='Click here to edit core network details' class='ui-icon ui-icon-unlocked ui-align-center' onClick =editNetworkData('"+URLEncrypt.EncryptParameters1(new string[]{"NetworkCode =" + roadDetails.PLAN_CN_ROAD_CODE.ToString().Trim(),"UnlockFlag =" + roadDetails.UNLOCK_BY_MORD.ToString().Trim()})+"'); return false;'>Delete</a>":roadDetails.UNLOCK_BY_MORD=="N"?"<a href='#' title='Click here to view core network details' class='ui-icon ui-icon-pencil ui-align-center' onClick =editNetworkData('"+URLEncrypt.EncryptParameters1(new string[]{"NetworkCode =" + roadDetails.PLAN_CN_ROAD_CODE.ToString().Trim()})+"'); return false;'>Edit</a>":"<span class='ui-icon ui-icon-locked ui-align-center'></span></center>"),
                    
                    roadDetails.UNLOCK_BY_MORD == "M"?"<a href='#' title='Click here to delete core network details' class='ui-icon ui-icon-trash ui-align-center' onClick =deleteNetworkData('"+URLEncrypt.EncryptParameters1(new string[]{"NetworkCode =" + roadDetails.PLAN_CN_ROAD_CODE.ToString().Trim()})+"'); return false;'>Delete</a>":roadDetails.UNLOCK_BY_MORD=="N"?"<a href='#' title='Click here to delete core network details' class='ui-icon ui-icon-trash ui-align-center' onClick =deleteNetworkData('"+URLEncrypt.EncryptParameters1(new string[]{"NetworkCode =" + roadDetails.PLAN_CN_ROAD_CODE.ToString().Trim()})+"'); return false;'>Delete</a>":"<span class='ui-icon ui-icon-locked ui-align-center'></span></center>",
                }
                }).ToArray();
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "GetCoreNetWorksListPMGSY3DAL()");
                totalRecords = 0;
                dbContext.Dispose();
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

        public Array ListCandidateRoadsPMGSY3DALITNO(int roadCode, int page, int rows, string sidx, string sord, out long totalRecords, out string IsFinalized)
        {
            int erRoadCode = 0;
            PMGSYEntities dbContext = new PMGSYEntities();
            try
            {
                var lstDRRP = dbContext.PLAN_ROAD.Where(c => c.PLAN_CN_ROAD_CODE == roadCode).Select(x => new
                {
                    x.MASTER_BLOCK.MAST_BLOCK_NAME,
                    x.MASTER_EXISTING_ROADS.MASTER_ROAD_CATEGORY.MAST_ROAD_CAT_NAME,
                    x.MASTER_EXISTING_ROADS.MAST_ER_ROAD_NAME,
                    x.PLAN_RD_LENGTH,
                    x.PLAN_RD_LENG,
                    x.MAST_ER_ROAD_CODE,
                    x.PLAN_LOCK_STATUS,
                    x.PLAN_CN_ROAD_CODE,
                    x.MASTER_EXISTING_ROADS.MAST_ER_ROAD_NUMBER
                }).ToList();
                erRoadCode = lstDRRP.Where(z => z.PLAN_CN_ROAD_CODE == roadCode).Select(x => x.MAST_ER_ROAD_CODE).FirstOrDefault();

                var lstMappedRoads = (from item in dbContext.PLAN_ROAD_MRL_PMGSY3
                                      where item.PLAN_CN_ROAD_CODE == roadCode
                                      select new
                                      {
                                          item.MASTER_EXISTING_ROADS.MASTER_BLOCK.MAST_BLOCK_NAME,
                                          item.MASTER_EXISTING_ROADS.MASTER_ROAD_CATEGORY.MAST_ROAD_CAT_NAME,
                                          item.MASTER_EXISTING_ROADS.MAST_ER_ROAD_NAME,
                                          item.PLAN_RD_LENGTH,
                                          item.PLAN_RD_LENG,
                                          item.MAST_ER_ROAD_CODE,
                                          item.PLAN_LOCK_STATUS,
                                          item.PLAN_CN_ROAD_CODE,
                                          item.MASTER_EXISTING_ROADS.MAST_ER_ROAD_NUMBER
                                      }).ToList().Union(lstDRRP);

                totalRecords = lstMappedRoads.Count();

                IsFinalized = String.Empty;
                if (dbContext.PLAN_ROAD.Any(m => m.PLAN_CN_ROAD_CODE == roadCode && m.PLAN_LOCK_STATUS == "N"))
                {
                    IsFinalized = "N";
                }
                else if (dbContext.PLAN_ROAD.Any(m => m.PLAN_CN_ROAD_CODE == roadCode && m.PLAN_LOCK_STATUS == "Y"))
                {
                    IsFinalized = "Y";
                }

                if (dbContext.MANE_CN_PCI_INDEX_PMGSY3.Where(x => x.PLAN_CN_ROAD_CODE == roadCode).Any())
                {
                    IsFinalized = "PCI";
                }

                if (sidx.Trim() != string.Empty)
                {
                    if (sord.ToString() == "asc")
                    {
                        switch (sidx)
                        {
                            case "MAST_BLOCK_NAME":
                                lstMappedRoads = lstMappedRoads.OrderBy(x => x.MAST_BLOCK_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                            case "MAST_ROAD_CAT_NAME":
                                lstMappedRoads = lstMappedRoads.OrderBy(x => x.MAST_ROAD_CAT_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                            case "MAST_ER_ROAD_NAME":
                                lstMappedRoads = lstMappedRoads.OrderBy(x => x.MAST_ER_ROAD_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                            case "PLAN_RD_LENGTH":
                                lstMappedRoads = lstMappedRoads.OrderBy(x => x.PLAN_RD_LENGTH).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                            case "PLAN_RD_LENG":
                                lstMappedRoads = lstMappedRoads.OrderBy(x => x.PLAN_RD_LENG).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                            default:
                                lstMappedRoads = lstMappedRoads.OrderBy(x => x.MAST_ER_ROAD_CODE).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                        }
                    }
                    else
                    {
                        switch (sidx)
                        {
                            case "MAST_BLOCK_NAME":
                                lstMappedRoads = lstMappedRoads.OrderByDescending(x => x.MAST_BLOCK_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                            case "MAST_ROAD_CAT_NAME":
                                lstMappedRoads = lstMappedRoads.OrderByDescending(x => x.MAST_ROAD_CAT_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                            case "MAST_ER_ROAD_NAME":
                                lstMappedRoads = lstMappedRoads.OrderByDescending(x => x.MAST_ER_ROAD_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                            case "PLAN_RD_LENGTH":
                                lstMappedRoads = lstMappedRoads.OrderByDescending(x => x.PLAN_RD_LENGTH).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                            case "PLAN_RD_LENG":
                                lstMappedRoads = lstMappedRoads.OrderByDescending(x => x.PLAN_RD_LENG).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                            default:
                                lstMappedRoads = lstMappedRoads.OrderByDescending(x => x.MAST_ER_ROAD_CODE).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                        }
                    }
                }
                else
                {
                    lstMappedRoads = lstMappedRoads.OrderByDescending(x => x.MAST_ER_ROAD_CODE).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                }

                return lstMappedRoads.Select(roadDetails => new
                {
                    cell = new[]
                        {
                            roadDetails.MAST_BLOCK_NAME == null?"-":roadDetails.MAST_BLOCK_NAME.ToString(),
                            roadDetails.MAST_ROAD_CAT_NAME == null?"-":roadDetails.MAST_ROAD_CAT_NAME.ToString(),
                            roadDetails.MAST_ER_ROAD_NAME == null?"-":(roadDetails.MAST_ER_ROAD_NUMBER.ToString() +" - "+ roadDetails.MAST_ER_ROAD_NAME.ToString()),
                            roadDetails.PLAN_RD_LENGTH == null?"0":roadDetails.PLAN_RD_LENGTH.ToString(),
                            roadDetails.PLAN_RD_LENG == null?"-":(roadDetails.PLAN_RD_LENG == "P"?"Partial":"Full"),
                            roadDetails.PLAN_LOCK_STATUS == "Y"?"<span class='ui-icon ui-icon-locked ui-align-center'></span>":"<a href='#' title='Click here to edit TR/MRL road details' class='ui-icon ui-icon-pencil ui-align-center' onClick =EditCandidateRoad('"+URLEncrypt.EncryptParameters1(new string[]{"DRRPCode =" + roadDetails.MAST_ER_ROAD_CODE.ToString().Trim(),"CNCode ="+roadDetails.PLAN_CN_ROAD_CODE})+"'); return false;'>Edit</a>",
                            roadDetails.MAST_ER_ROAD_CODE == erRoadCode ? "-" :
                            roadDetails.PLAN_LOCK_STATUS == "Y"?"<span class='ui-icon ui-icon-locked ui-align-center'></span>":"<a href='#' title='Click here to delete TR/MRL road details' class='ui-icon ui-icon-trash ui-align-center' onClick =DeleteCandidateRoad('"+URLEncrypt.EncryptParameters1(new string[]{"DRRPCode =" + roadDetails.MAST_ER_ROAD_CODE.ToString().Trim(),"CNCode ="+roadDetails.PLAN_CN_ROAD_CODE})+"'); return false;'>Delete</a>",
                            roadDetails.PLAN_LOCK_STATUS == "Y"?"<span class='ui-icon ui-icon-locked ui-align-center'></span>":"<a href='#' title='Click here to Map/Delete Habitation details' class='ui-icon ui-icon-plusthick ui-align-center' onClick =MapOrDeleteHabitations('"+URLEncrypt.EncryptParameters1(new string[]{"DRRPCode =" + roadDetails.MAST_ER_ROAD_CODE.ToString().Trim(),"CNCode ="+roadDetails.PLAN_CN_ROAD_CODE})+"'); return false;'>Delete</a>",
                        }
                }).ToArray();

            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "ListCandidateRoadsPMGSY3DALITNO()");
                totalRecords = 0;
                IsFinalized = String.Empty;
                return null;
            }
            finally
            {
                dbContext.Dispose();
            }
        }

        public bool DeFinalizeMappedDRRPDetailsPMGSY3DALITNO(int CNCode)
        {
            PMGSYEntities dbContext = new PMGSYEntities();
            try
            {
                var lstMappedDetails = dbContext.PLAN_ROAD.Where(m => m.PLAN_CN_ROAD_CODE == CNCode && m.MAST_PMGSY_SCHEME == 4).FirstOrDefault();
                if (lstMappedDetails != null)
                {
                    lstMappedDetails.PLAN_LOCK_STATUS = "N";

                    #region Drfinalize Habs
                    var lstCNHabs = dbContext.PLAN_ROAD_HABITATION_PMGSY3.Where(m => m.PLAN_CN_ROAD_CODE == CNCode && m.PLAN_CN_HAB_FINALIZED == "Y").ToList();
                    if (lstCNHabs != null)
                    {
                        if (lstCNHabs.Count() > 0)
                        {
                            lstCNHabs.ForEach(m => m.PLAN_CN_HAB_FINALIZED = null);
                            lstCNHabs.ForEach(m => m.PLAN_CN_HAB_FIN_DATE = null);
                        }
                    }
                    #endregion

                    dbContext.SaveChanges();
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "DeFinalizeMappedDRRPDetailsPMGSY3DALITNO()");
                return false;
            }
            finally
            {
                dbContext.Dispose();
            }
        }
        #endregion

        #region Definalization TR/MRL Blocks mord
        public Array GetBlockListMRLPMGSY3DALMORD(int districtCode, int? page, int? rows, string sidx, string sord, out long totalRecords, string filters, ref bool isAllBlockFinalized, ref bool isDistrictFinalizedForTRMRL, int statecode)
        {
            PMGSYEntities dbContext = new PMGSYEntities();
            try
            {
                isAllBlockFinalized = false;

                JavaScriptSerializer js = null;
                PMGSY.Common.CommonFunctions.SearchJson test = new PMGSY.Common.CommonFunctions.SearchJson();
                string roadName = string.Empty;

                if (filters != null)
                {
                    js = new JavaScriptSerializer();
                    test = js.Deserialize<PMGSY.Common.CommonFunctions.SearchJson>(filters);

                    foreach (PMGSY.Common.CommonFunctions.rules item in test.rules)
                    {
                        switch (item.field)
                        {
                            case "MAST_ER_ROAD_NAME": roadName = item.data;
                                break;
                            default:
                                break;
                        }
                    }
                }

                dbContext = new PMGSYEntities();

                var lstMRLFinalizedBlocks = dbContext.MAST_CN_BLOCK_PMGSY3_FINALIZE.Where(z => z.MASTER_BLOCK.MAST_DISTRICT_CODE == districtCode && z.MASTER_BLOCK.MAST_BLOCK_ACTIVE == "Y" && z.IS_FINALIZED == "Y").Select(x => new { MAST_BLOCK_CODE = x.MAST_BLOCK_CODE, MAST_BLOCK_NAME = x.MASTER_BLOCK.MAST_BLOCK_NAME }).Distinct().ToList();

                ///Get Block Names
                var lstBlock = dbContext.MASTER_BLOCK.Where(c => c.MAST_BLOCK_ACTIVE == "Y"
                    && c.MAST_DISTRICT_CODE == (districtCode == 0 ? c.MAST_DISTRICT_CODE : districtCode)
                    && c.MASTER_DISTRICT.MAST_STATE_CODE == statecode)

                    .Select(x => new { MAST_BLOCK_CODE = x.MAST_BLOCK_CODE, MAST_BLOCK_NAME = x.MAST_BLOCK_NAME.Trim(), MAST_DISTRICT_CODE = x.MAST_DISTRICT_CODE, MAST_DISTRICT_NAME = x.MASTER_DISTRICT.MAST_DISTRICT_NAME }).OrderBy(x => x.MAST_DISTRICT_NAME).ToList();

                totalRecords = lstBlock.Count();

                if (sidx.Trim() != string.Empty)
                {
                    if (sord.ToString() == "asc")
                    {
                        lstBlock = lstBlock.OrderBy(x => x.MAST_DISTRICT_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                    }
                    else
                    {
                        lstBlock = lstBlock.OrderByDescending(x => x.MAST_DISTRICT_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                    }
                }
                else
                {
                    lstBlock = lstBlock.OrderBy(x => x.MAST_DISTRICT_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                }

                if (lstBlock.Count() == lstMRLFinalizedBlocks.Count() && (!dbContext.MAST_CN_DISTRICT_PMGSY3_FINALIZE.Any(z => z.MAST_DISTRICT_CODE == districtCode && z.IS_FINALIZED == "Y") || !dbContext.MAST_CN_DISTRICT_PMGSY3_FINALIZE.Any(z => z.MAST_DISTRICT_CODE == districtCode)))
                {
                    isAllBlockFinalized = true;
                }
                isDistrictFinalizedForTRMRL = dbContext.MAST_CN_DISTRICT_PMGSY3_FINALIZE.Where(x => x.MAST_DISTRICT_CODE == districtCode && x.IS_FINALIZED == "Y").Any();

                return lstBlock.Select(item => new
                {
                    //id = item.MAST_ER_ROAD_CODE,
                    cell = new[]
                    {    
                        item.MAST_DISTRICT_NAME.ToString(),
                        item.MAST_BLOCK_NAME.ToString(),
                        
                        (dbContext.MAST_CN_BLOCK_PMGSY3_FINALIZE.Any(z=>z.MAST_BLOCK_CODE == item.MAST_BLOCK_CODE && z.IS_FINALIZED == "Y"))
                        ? "<span class='ui-icon ui-icon-locked ui-align-center'></span>"
                        : (dbContext.PLAN_ROAD.Where(m=>m.MAST_BLOCK_CODE == item.MAST_BLOCK_CODE && m.PLAN_LOCK_STATUS == "N" && m.MAST_PMGSY_SCHEME == 4).Any()) 
                                ? "All TR/MRL roads not finalized for the block"
                                : (dbContext.MAST_CN_BLOCK_PMGSY3_FINALIZE.Where(z=>z.MAST_BLOCK_CODE == item.MAST_BLOCK_CODE && z.IS_FINALIZED == "N").Any() || !(dbContext.MAST_CN_BLOCK_PMGSY3_FINALIZE.Where(z=>z.MAST_BLOCK_CODE == item.MAST_BLOCK_CODE).Any()))
                                    ? "<a href='#' title='Click here to delete details' class='ui-icon ui-icon-unlocked ui-align-center' onClick =FinalizeMRLBlock('"
                                            +URLEncrypt.EncryptParameters1(new string[]{"BlockCode =" + item.MAST_BLOCK_CODE.ToString().Trim()}) +"'); return false;'></a>"
                                    : "",
                        //DeFinalize
                        (dbContext.MAST_CN_BLOCK_PMGSY3_FINALIZE.Any(z=>z.MAST_BLOCK_CODE == item.MAST_BLOCK_CODE && z.IS_FINALIZED == "Y") && !dbContext.MAST_CN_DISTRICT_PMGSY3_FINALIZE.Any(z=>z.MAST_DISTRICT_CODE == districtCode && z.IS_FINALIZED ==                                     "Y"))   
                        ? "<a href='#' title='Click here to delete details' class='ui-icon ui-icon-unlocked ui-align-center' onClick =DeFinalizeMRLBlock('"
                                            +URLEncrypt.EncryptParameters1(new string[]{"BlockCode =" + item.MAST_BLOCK_CODE.ToString().Trim()}) +"'); return false;'></a>"
                        : (dbContext.MAST_CN_BLOCK_PMGSY3_FINALIZE.Any(z=>z.MAST_BLOCK_CODE == item.MAST_BLOCK_CODE && z.IS_FINALIZED == "Y") && dbContext.MAST_CN_DISTRICT_PMGSY3_FINALIZE.Any(z=>z.MAST_DISTRICT_CODE == districtCode && z.IS_FINALIZED                                      == "Y"))
                            ? "District is finalized"
                            : "<span style='color:red;'>Block not yet finalized</span>"
                    }
                }).ToArray();
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "GetBlockListMRLPMGSY3DAL()");
                totalRecords = 0;
                return null;
            }
            finally
            {
                dbContext.Dispose();
            }
        }
        #endregion

        #region CUPL PMGSY3
        public Array GetBlockListCUPLPMGSY3DAL(int districtCode, int Year, int Batch, int? page, int? rows, string sidx, string sord, out long totalRecords)
        {
            PMGSYEntities dbContext = new PMGSYEntities();
            try
            {
                string roadName = string.Empty;

                dbContext = new PMGSYEntities();

                ///Get Block Names
                //var lstBlock = dbContext.MASTER_BLOCK.Where(c => c.MAST_DISTRICT_CODE == districtCode && c.MAST_BLOCK_ACTIVE == "Y").Select(x => new { MAST_BLOCK_CODE = x.MAST_BLOCK_CODE, MAST_BLOCK_NAME = x.MAST_BLOCK_NAME.Trim() }).OrderBy(z => z.MAST_BLOCK_CODE).Distinct().ToList();
                var lstBlock = dbContext.USP_BLOCK_CUPL_PMGSY3_NOT_ELIGIBILITY(PMGSYSession.Current.StateCode, districtCode, 0, Year, Batch, 0, "%").ToList();

                totalRecords = lstBlock.Count();

                short roleCode = dbContext.UM_User_Master.Where(m => m.UserID == PMGSYSession.Current.UserId).Select(m => m.DefaultRoleID).FirstOrDefault();


                if (sidx.Trim() != string.Empty)
                {
                    if (sord.ToString() == "asc")
                    {
                        lstBlock = lstBlock.OrderBy(x => x.MAST_BLOCK_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                    }
                    else
                    {
                        lstBlock = lstBlock.OrderByDescending(x => x.MAST_BLOCK_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                    }
                }
                else
                {
                    lstBlock = lstBlock.OrderBy(x => x.MAST_BLOCK_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                }

                return lstBlock.Select(item => new
                {
                    //id = item.MAST_ER_ROAD_CODE,
                    cell = new[]
                    {       
                        item.MAST_BLOCK_NAME.ToString(),
                         //View CUPL Report
                        (dbContext.UDF_IMS_UNLOCK_STATUS(PMGSYSession.Current.StateCode, PMGSYSession.Current.DistrictCode, item.MAST_BLOCK_CODE, 0, 0, 0, 0, 0, "CN", PMGSYSession.Current.PMGSYScheme, roleCode).Select(c => c.UNLOCK_COUNT).FirstOrDefault() > 0) ? "CUPL cannot be generated as TR/MRL is unlocked. Please contact NRIDA"
                        : ("<a href='#' title='Click here to view CUPL report' class='ui-icon ui-icon-zoomin ui-align-center' onClick =LoadCUPLReport('"+ URLEncrypt.EncryptParameters1(new string[]{"BlockCode =" + item.MAST_BLOCK_CODE.ToString().Trim(), "BlockName=" + item.MAST_BLOCK_NAME.Trim() }) +"'); return false;'></a>"),
                        //Generate CUPL 
                        (dbContext.UDF_IMS_UNLOCK_STATUS(PMGSYSession.Current.StateCode, PMGSYSession.Current.DistrictCode, item.MAST_BLOCK_CODE, 0, 0, 0, 0, 0, "CN", PMGSYSession.Current.PMGSYScheme, roleCode).Select(c => c.UNLOCK_COUNT).FirstOrDefault() > 0) ? "CUPL cannot be generated as TR/MRL is unlocked. Please contact NRIDA"
                        : ((dbContext.CUPL_PMGSY3.Where(z=>z.MAST_DISTRICT_CODE == districtCode && z.MAST_BLOCK_CODE == item.MAST_BLOCK_CODE && z.IMS_YEAR == Year && z.IMS_BATCH == Batch).Any())
                        ? "<a href='#' title='Click here to view details' class='ui-icon ui-icon-zoomin ui-align-center' onClick =ViewCUPLDetails('"+ URLEncrypt.EncryptParameters1(new string[]{"BlockCode =" + item.MAST_BLOCK_CODE.ToString().Trim(),"BlockName=" + item.MAST_BLOCK_NAME.Trim(),"BatchCode="+Batch,"YearCode="+Year}) +"'); return false;'></a>"
                        :   (PMGSYSession.Current.DistrictCode > 0)
                            ? "<a href='#' title='Click here to generate details' class='ui-icon ui-icon-plusthick ui-align-center' onClick =GenerateCUPLModal('"+ URLEncrypt.EncryptParameters1(new string[]{"BlockCode =" + item.MAST_BLOCK_CODE.ToString().Trim(),                                       "BlockName=" + item.MAST_BLOCK_NAME.Trim()  }) +"'); return false;'></a>"
                            : "-"
                         ),
                        //Copy TR/MRL
                        (dbContext.UDF_IMS_UNLOCK_STATUS(PMGSYSession.Current.StateCode, PMGSYSession.Current.DistrictCode, item.MAST_BLOCK_CODE, 0, 0, 0, 0, 0, "CN", PMGSYSession.Current.PMGSYScheme, roleCode).Select(c => c.UNLOCK_COUNT).FirstOrDefault() > 0) ? "CUPL cannot be generated as TR/MRL is unlocked. Please contact NRIDA"
                        : ((dbContext.CUPL_PMGSY3.Where(z=>z.MAST_DISTRICT_CODE == districtCode && z.MAST_BLOCK_CODE == item.MAST_BLOCK_CODE && z.IMS_YEAR == Year && z.IMS_BATCH == Batch).Any() && Batch != 1)
                        ? (from cupl in dbContext.CUPL_PMGSY3 join exm in dbContext.TR_MRL_EXEMPTION on cupl.CUPL_PMGSY3_ID equals exm.CUPL_PMGSY3_ID where cupl.MAST_BLOCK_CODE == item.MAST_BLOCK_CODE && cupl.IMS_YEAR == Year && cupl.IMS_BATCH == 1                                select cupl.CUPL_PMGSY3_ID).Any()
                            && !(from cupl in dbContext.CUPL_PMGSY3 join exm in dbContext.TR_MRL_EXEMPTION on cupl.CUPL_PMGSY3_ID equals exm.CUPL_PMGSY3_ID where cupl.MAST_BLOCK_CODE == item.MAST_BLOCK_CODE && cupl.IMS_YEAR == Year && cupl.IMS_BATCH == 2                                select cupl.CUPL_PMGSY3_ID).Any()
                            ? "<a href='#' title='Click here to generate details' class='ui-icon ui-icon-copy ui-align-center' onClick=CopyTRMRLExemptionBatch2('"+ URLEncrypt.EncryptParameters1(new string[]{"block =" + item.MAST_BLOCK_CODE.ToString().Trim(), "year=" + Year, "batch=" + Batch }) +"'); return false;'></a>"
                            : "TR/MRL Exemption copied for Batch2"
                        :"-"
                        )
                    }
                }).ToArray();
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "GetBlockListMRLPMGSY3DAL()");
                totalRecords = 0;
                return null;
            }
            finally
            {
                dbContext.Dispose();
            }
        }

        public bool GeneratePMGSY3DAL(CUPLPMGSY3ViewModel model, ref string message)
        {
            PMGSYEntities dbContext = new PMGSYEntities();
            int maxId = 0;
            CUPL_PMGSY3 cupl_pmgsy3 = null;
            CommonFunctions comm = new CommonFunctions();
            try
            {
                //using (TransactionScope ts = new TransactionScope())
                //{
                // Added By Rohit On 13APR2020
                var isStateOrDisrtictOrBlockUnlockedForHabitationChanges = checkIsUnlocked(model.stateCode, model.districtCode, model.blockCode);
                if (isStateOrDisrtictOrBlockUnlockedForHabitationChanges == true)
                {
                    message = "Habitations are unlocked in this Block for some duration. Hence CUPL can not be generated.";
                    return false;
                }

                #region Modified Code
                maxId = (dbContext.CUPL_PMGSY3.Any() ? dbContext.CUPL_PMGSY3.Select(x => x.CUPL_PMGSY3_ID).Max() : 0) + 1;
                var query = dbContext.USP_CUPL_PCI_PMGSY3(model.stateCode, model.districtCode, model.blockCode);
                if (query != null)
                {
                    foreach (var itm in query)
                    {
                        if (string.IsNullOrEmpty(Convert.ToString(itm.MEANPCI)))
                        {
                            message = "Mean PCI value is null";
                            return false;
                        }
                        cupl_pmgsy3 = new CUPL_PMGSY3();

                        cupl_pmgsy3.CUPL_PMGSY3_ID = maxId;
                        cupl_pmgsy3.CUPL_RANK = Convert.ToInt32(itm.CUPL_RANK);
                        cupl_pmgsy3.MAST_DISTRICT_CODE = Convert.ToInt32(itm.MAST_DISTRICT_CODE);
                        cupl_pmgsy3.MAST_DISTRICT_NAME = itm.MAST_DISTRICT_NAME;
                        cupl_pmgsy3.MAST_BLOCK_CODE = Convert.ToInt32(itm.MAST_BLOCK_CODE);
                        cupl_pmgsy3.MAST_BLOCK_NAME = itm.MAST_BLOCK_NAME;
                        cupl_pmgsy3.PLAN_CN_ROAD_CODE = itm.PLAN_CN_ROAD_CODE;
                        cupl_pmgsy3.PLAN_CN_ROAD_NUMBER = itm.PLAN_CN_ROAD_NUMBER;
                        cupl_pmgsy3.PLAN_RD_LENGTH = Convert.ToDecimal(itm.PLAN_RD_LENGTH);
                        cupl_pmgsy3.ELIGIBLE_CANDIDATE_LENGTH = Convert.ToDecimal(itm.ELIGIBLE_CANDIDATE_LENGTH);
                        cupl_pmgsy3.PLAN_RD_NAME = itm.PLAN_RD_NAME;
                        cupl_pmgsy3.SCORE = Convert.ToInt32(itm.SCORE);
                        cupl_pmgsy3.SCORE_PER_UNIT_LENGTH = Convert.ToDecimal(itm.SCORE_PER_UNIT_LENGTH);
                        cupl_pmgsy3.MAST_ER_ROAD_CODE = Convert.ToInt32(itm.MAST_ER_ROAD_CODE);
                        cupl_pmgsy3.MAST_ER_ROAD_NAME = itm.MAST_ER_ROAD_NAME;
                        cupl_pmgsy3.MAST_ER_ROAD_NUMBER = itm.MAST_ER_ROAD_NUMBER;
                        cupl_pmgsy3.MAST_ER_ROAD_OWNER = itm.MAST_ER_ROAD_OWNER.HasValue ? itm.MAST_ER_ROAD_OWNER.Value : 0;
                        cupl_pmgsy3.PLAN_RD_FROM_CHAINAGE = Convert.ToDecimal(itm.PLAN_RD_FROM_CHAINAGE);
                        cupl_pmgsy3.PLAN_RD_TO_CHAINAGE = Convert.ToDecimal(itm.PLAN_RD_TO_CHAINAGE);
                        cupl_pmgsy3.MIN_SCORE = Convert.ToDecimal(itm.MIN_SCORE);
                        cupl_pmgsy3.MAST_ER_ROAD_SCORE = itm.MAST_ER_ROAD_SCORE;
                        cupl_pmgsy3.MEANPCI = Convert.ToDecimal(itm.MEANPCI);
                        cupl_pmgsy3.PCI_ELIGIBILITY = itm.PCI_ELIGIBILITY;
                        cupl_pmgsy3.ELIGIBILE_LENGTH = Convert.ToDecimal(itm.ELIGIBILE_LENGTH);
                        cupl_pmgsy3.COMPLETED = itm.COMPLETED;
                        cupl_pmgsy3.ELIGIBLE = itm.ELIGIBLE;
                        cupl_pmgsy3.DATE_DIFFETENCR = itm.DATE_DIFFETENCR;
                        cupl_pmgsy3.TOTAL_HAB_SERVER = null;//itm.TOTAL_HAB_SERVER;     Currently insert as null
                        cupl_pmgsy3.IMS_YEAR = model.Year;
                        cupl_pmgsy3.IMS_BATCH = model.Batch;
                        cupl_pmgsy3.FINALIZED_DATE = comm.GetStringToDateTime(model.generationDate);
                        cupl_pmgsy3.APPROVAL = (itm.ELIGIBLE_CANDIDATE_LENGTH > 0 && itm.PLAN_RD_LENGTH >= 5) ? "Y" : "N";

                        cupl_pmgsy3.USERID = PMGSYSession.Current.UserId;
                        cupl_pmgsy3.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                        dbContext.CUPL_PMGSY3.Add(cupl_pmgsy3);
                        maxId++;
                    }
                    dbContext.SaveChanges();
                }
                #endregion
                // ts.Complete();
                //}
                //ObjectParameter name = new ObjectParameter("outputMessage", typeof(String));
                var query2 = dbContext.USP_EXEMPTION_INSERT_STATE(model.stateCode, model.districtCode, model.blockCode, model.Year, model.Batch);
                //dbContext.USP_EXEMPTION_INSERT_STATE(model.stateCode, model.districtCode, model.blockCode, model.Year, model.Batch,name);

                //end of change
                return true;
            }
            catch (DbEntityValidationException e)
            {
                ErrorLog.LogError(e, "GeneratePMGSY3DAL().DbEntityValidationException");

                ModelStateDictionary modelstate = new ModelStateDictionary();
                foreach (var eve in e.EntityValidationErrors)
                {
                    foreach (var ve in eve.ValidationErrors)
                    {
                        modelstate.AddModelError(ve.ErrorMessage, ve.ErrorMessage);
                    }
                }

                ErrorLog.LogError(e, "GeneratePMGSY3DAL().DbEntityValidationException()");
                using (System.IO.StreamWriter sw = System.IO.File.AppendText(System.IO.Path.Combine("D:\\ErrorLog\\OMMASErrorLog_" + System.DateTime.Now.Date.ToString("dd_MM_yyyy") + ".txt", "")))
                {
                    sw.WriteLine("Date :" + DateTime.Now.ToString());
                    sw.WriteLine("GeneratePMGSY3DAL().DbEntityValidationException() : " + "Application_Error()");

                    sw.WriteLine("Exception : " + new CommonFunctions().FormatErrorMessage(modelstate));
                    sw.WriteLine("---------------------------------------------------------------------------------------");
                    sw.Close();
                }
                return false;
            }
            catch (UpdateException ex)
            {
                ErrorLog.LogError(ex, "GeneratePMGSY3DAL.UpdateException()");
                message = "An Error Occured While proccessing your request";
                return false;
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "GeneratePMGSY3DAL()");
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

        public Array GetCUPLPMGSY3ListDAL(int districtCode, int blockCode, int? page, int? rows, string sidx, string sord, out long totalRecords)
        {
            PMGSYEntities dbContext = new PMGSYEntities();
            try
            {
                string roadName = string.Empty;

                dbContext = new PMGSYEntities();

                ///Get Block Names
                //var query = dbContext.USP_CUPL_PCI_PMGSY3(model.stateCode, model.districtCode, model.blockCode).ToList();

                var query = dbContext.CUPL_PMGSY3.Where(x => x.MAST_DISTRICT_CODE == districtCode && x.MAST_BLOCK_CODE == blockCode).Select(z => new
                {
                    z.CUPL_RANK,
                    z.MAST_DISTRICT_NAME,
                    z.MAST_BLOCK_NAME,
                    z.PLAN_CN_ROAD_NUMBER,
                    z.PLAN_RD_LENGTH,
                    z.ELIGIBLE_CANDIDATE_LENGTH,
                    z.PLAN_RD_NAME,
                    z.SCORE,
                    z.SCORE_PER_UNIT_LENGTH,
                    z.MIN_SCORE,
                    z.MAST_ER_ROAD_NAME,
                    z.MAST_ER_ROAD_NUMBER,
                    z.MAST_ER_ROAD_OWNER,
                    z.PLAN_RD_FROM_CHAINAGE,
                    z.PLAN_RD_TO_CHAINAGE,
                    z.MAST_ER_ROAD_SCORE,
                    z.MEANPCI,
                    z.PCI_ELIGIBILITY,
                    z.ELIGIBILE_LENGTH,
                    z.COMPLETED,
                    z.ELIGIBLE,
                    z.DATE_DIFFETENCR,
                    //z.TRACE_MAP_RANK,
                    z.TOTAL_HAB_SERVER,
                    z.IMS_YEAR,
                    z.IMS_BATCH,
                    z.FINALIZED_DATE //SqlFunctions.DatePart("date",z.FINALIZED_DATE)
                }).ToList();

                totalRecords = query.Count();

                if (sidx.Trim() != string.Empty)
                {
                    if (sord.ToString() == "asc")
                    {
                        query = query.OrderBy(x => x.MAST_BLOCK_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                    }
                    else
                    {
                        query = query.OrderByDescending(x => x.MAST_BLOCK_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                    }
                }
                else
                {
                    query = query.OrderBy(x => x.MAST_BLOCK_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                }

                return query.Select(item => new
                {
                    //id = item.MAST_ER_ROAD_CODE,
                    cell = new[]
                    {       
                        item.CUPL_RANK.ToString(),
                        item.MAST_DISTRICT_NAME,
                        item.MAST_BLOCK_NAME,
                        item.PLAN_CN_ROAD_NUMBER,
                        item.PLAN_RD_LENGTH.ToString(),
                        item.ELIGIBLE_CANDIDATE_LENGTH.ToString(),
                        item.PLAN_RD_NAME,
                        item.SCORE.ToString(),
                        item.SCORE_PER_UNIT_LENGTH.ToString(),
                        item.MAST_ER_ROAD_NAME,
                        item.MAST_ER_ROAD_NUMBER,
                        item.PLAN_RD_FROM_CHAINAGE.ToString(),
                        item.PLAN_RD_TO_CHAINAGE.ToString(),
                        item.MAST_ER_ROAD_SCORE.ToString(),
                        item.MIN_SCORE.ToString(),
                        item.MEANPCI.ToString(),
                        item.PCI_ELIGIBILITY,
                        item.ELIGIBILE_LENGTH.ToString(),
                        string.IsNullOrEmpty(item.COMPLETED) ? "-" : item.COMPLETED,
                        string.IsNullOrEmpty(item.ELIGIBLE) ? "-" : item.ELIGIBLE,
                        string.IsNullOrEmpty(Convert.ToString(item.DATE_DIFFETENCR)) ? "-" : Convert.ToString(item.DATE_DIFFETENCR),
                        //Convert.ToString(item.TRACE_MAP_RANK),
                        string.IsNullOrEmpty(Convert.ToString(item.TOTAL_HAB_SERVER)) ? "-" : Convert.ToString(item.TOTAL_HAB_SERVER),
                        new CommonFunctions().GetDateTimeToString(item.FINALIZED_DATE)
                    }
                }).ToArray();
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "GetBlockListMRLPMGSY3DAL()");
                totalRecords = 0;
                return null;
            }
            finally
            {
                dbContext.Dispose();
            }
        }

        public List<SelectListItem> PopulateCUPLBatch(int district, int block, int year)
        {
            PMGSYEntities dbContext = new PMGSYEntities();
            CommonFunctions comm = new CommonFunctions();
            List<SelectListItem> batchList = new List<SelectListItem>();
            SelectListItem item;
            int batch = 0;
            int tmp = 0;
            try
            {

                var lstBatch = (block > 0) ? dbContext.CUPL_PMGSY3.Where(z => z.IMS_YEAR == year && z.MAST_BLOCK_CODE == block).Select(x => x.IMS_BATCH).Distinct().ToList()
                                           : dbContext.CUPL_PMGSY3.Where(z => z.IMS_YEAR == year && z.MAST_DISTRICT_CODE == district).Select(x => x.IMS_BATCH).Distinct().ToList();
                if (lstBatch.Count() > 0)
                {
                    if (block == 0)
                    {
                        foreach (var data in lstBatch)
                        {
                            //batch = item;
                            item = new SelectListItem();
                            item.Text = "Batch " + data.ToString();
                            item.Value = data.ToString();
                            if (data <= 2)
                            {
                                batchList.Add(item);
                            }
                            tmp = data;
                        }
                        //batch++;
                        if (tmp < 2)
                        {
                            item = new SelectListItem();
                            item.Text = "Batch " + (tmp + 1).ToString();
                            item.Value = (tmp + 1).ToString();
                            if ((tmp + 1) <= 2)
                            {
                                batchList.Add(item);
                            }
                        }
                    }
                    else
                    {
                        foreach (var data in lstBatch)
                        {
                            tmp = data;
                        }
                        //batch++;
                        if (tmp < 2)
                        {
                            item = new SelectListItem();
                            item.Text = "Batch " + (tmp + 1).ToString();
                            item.Value = (tmp + 1).ToString();
                            if ((tmp + 1) <= 2)
                            {
                                batchList.Add(item);
                            }
                        }
                    }
                }
                else
                {
                    item = new SelectListItem();
                    item.Text = "Batch " + (batch + 1).ToString();
                    item.Value = (batch + 1).ToString();
                    if ((tmp + 1) <= 2)
                    {
                        batchList.Add(item);
                    }
                }
                //end of change
                //batch = batch > 6 ? 6 : batch;
                return batchList;
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "CoreNetworkDAL().PopulateCUPLBatch()");
                //message = "An Error Occured While proccessing your request";
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

        public bool CopyTRMRLExemptiontoBatch2DAL(int block, int year, ref string message)
        {
            PMGSYEntities dbContext = new PMGSYEntities();
            int maxId = 0;
            TR_MRL_EXEMPTION tr_mrl_exemption = null;
            CommonFunctions comm = new CommonFunctions();
            try
            {
                //using (TransactionScope ts = new TransactionScope())
                {
                    maxId = (dbContext.TR_MRL_EXEMPTION.Any() ? dbContext.TR_MRL_EXEMPTION.Select(x => x.TR_MRL_EXEMPTION_ID).Max() : 0) + 1;
                    var query = (from cupl in dbContext.CUPL_PMGSY3 join exm in dbContext.TR_MRL_EXEMPTION on cupl.CUPL_PMGSY3_ID equals exm.CUPL_PMGSY3_ID where cupl.MAST_BLOCK_CODE == block && cupl.IMS_YEAR == year && cupl.IMS_BATCH == 1 select exm).ToList();
                    if (query.Count() > 0)
                    {
                        foreach (var itm in query)
                        {
                            if (dbContext.CUPL_PMGSY3.Where(x => x.IMS_BATCH == 2 && x.PLAN_CN_ROAD_CODE == itm.PLAN_CN_ROAD_CODE).Any())
                            {
                                tr_mrl_exemption = new TR_MRL_EXEMPTION();

                                tr_mrl_exemption.TR_MRL_EXEMPTION_ID = maxId;
                                tr_mrl_exemption.CUPL_PMGSY3_ID = dbContext.CUPL_PMGSY3.Where(x => x.IMS_BATCH == 2 && x.PLAN_CN_ROAD_CODE == itm.PLAN_CN_ROAD_CODE).Select(c => c.CUPL_PMGSY3_ID).FirstOrDefault();//itm.CUPL_PMGSY3_ID;
                                tr_mrl_exemption.PLAN_CN_ROAD_CODE = itm.PLAN_CN_ROAD_CODE;
                                tr_mrl_exemption.REASON_FOR_NON_INCLUSION = itm.REASON_FOR_NON_INCLUSION;
                                tr_mrl_exemption.DATE_OF_NON_INCLUSION = itm.DATE_OF_NON_INCLUSION;
                                tr_mrl_exemption.REQUEST_REMARKS_EXEMPTION = itm.REQUEST_REMARKS_EXEMPTION;
                                tr_mrl_exemption.APPROVED_DATE = itm.APPROVED_DATE;//comm.GetStringToDateTime(Convert.ToString(itm.APPROVED_DATE));
                                tr_mrl_exemption.APPROVAL = itm.APPROVAL;
                                tr_mrl_exemption.APPROVAL_REMARKS = itm.APPROVAL_REMARKS;
                                tr_mrl_exemption.USERID = PMGSYSession.Current.UserId;
                                tr_mrl_exemption.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];

                                dbContext.TR_MRL_EXEMPTION.Add(tr_mrl_exemption);
                                maxId++;
                            }
                        }
                        dbContext.SaveChanges();
                    }
                    // ts.Complete();
                }
                //end of change
                return true;
            }
            catch (DbEntityValidationException e)
            {
                ErrorLog.LogError(e, "GeneratePMGSY3DAL().DbEntityValidationException");

                ModelStateDictionary modelstate = new ModelStateDictionary();
                foreach (var eve in e.EntityValidationErrors)
                {
                    foreach (var ve in eve.ValidationErrors)
                    {
                        modelstate.AddModelError(ve.ErrorMessage, ve.ErrorMessage);
                    }
                }

                ErrorLog.LogError(e, "GeneratePMGSY3DAL().DbEntityValidationException()");
                using (System.IO.StreamWriter sw = System.IO.File.AppendText(System.IO.Path.Combine("D:\\ErrorLog\\OMMASErrorLog_" + System.DateTime.Now.Date.ToString("dd_MM_yyyy") + ".txt", "")))
                {
                    sw.WriteLine("Date :" + DateTime.Now.ToString());
                    sw.WriteLine("GeneratePMGSY3DAL().DbEntityValidationException() : " + "Application_Error()");

                    sw.WriteLine("Exception : " + new CommonFunctions().FormatErrorMessage(modelstate));
                    sw.WriteLine("---------------------------------------------------------------------------------------");
                    sw.Close();
                }
                return false;
            }
            catch (UpdateException ex)
            {
                ErrorLog.LogError(ex, "GeneratePMGSY3DAL.UpdateException()");
                message = "An Error Occured While proccessing your request";
                return false;
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "GeneratePMGSY3DAL()");
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
        #endregion

        #region Unlock Method
        // Added By Rohit On 13APR2020
        public bool checkIsUnlocked(int state, int district, int block)
        {
            PMGSYEntities dbContext = new PMGSYEntities();
            bool flag = false;
            try
            {
                // State 
                if (dbContext.IMS_UNLOCK_DETAILS.Where(x => x.MAST_PMGSY_SCHEME == 4 && x.IMS_UNLOCK_LEVEL == "S" && x.IMS_UNLOCK_TABLE == "CH" && x.IMS_UNLOCK_STATUS == "Y" && x.MAST_STATE_CODE == state && x.MAST_DISTRICT_CODE == null && x.MAST_BLOCK_CODE == null && (DateTime.Now >= x.IMS_UNLOCK_START_DATE && DateTime.Now <= x.IMS_UNLOCK_END_DATE)).Any())
                {
                    flag = true;
                    return flag;
                }// District
                else if (dbContext.IMS_UNLOCK_DETAILS.Where(x => x.MAST_PMGSY_SCHEME == 4 && x.IMS_UNLOCK_LEVEL == "D" && x.IMS_UNLOCK_TABLE == "CH" && x.IMS_UNLOCK_STATUS == "Y" && x.MAST_STATE_CODE == state && x.MAST_DISTRICT_CODE == district && x.MAST_BLOCK_CODE == null && (DateTime.Now >= x.IMS_UNLOCK_START_DATE && DateTime.Now <= x.IMS_UNLOCK_END_DATE)).Any())
                {
                    flag = true;
                    return flag;
                }// Block
                else if (dbContext.IMS_UNLOCK_DETAILS.Where(x => x.MAST_PMGSY_SCHEME == 4 && x.IMS_UNLOCK_LEVEL == "B" && x.IMS_UNLOCK_TABLE == "CH" && x.IMS_UNLOCK_STATUS == "Y" && x.MAST_STATE_CODE == state && x.MAST_DISTRICT_CODE == null && x.MAST_BLOCK_CODE == block && (DateTime.Now >= x.IMS_UNLOCK_START_DATE && DateTime.Now <= x.IMS_UNLOCK_END_DATE)).Any())
                {
                    flag = true;
                    return flag;
                }
                else
                {
                    flag = false;
                    return flag;
                }

                // flag = Convert.ToInt32(query) > 0 ? true : false;

                //  return flag;

            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "CoreNetworkDAL.checkIsUnlocked()");
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

        //public bool checkIsTRMRLIsUnlocked(int state, int district, int block)
        //{
        //    PMGSYEntities dbContext = new PMGSYEntities();
        //    bool flag = false;
        //    try
        //    {
        //        // State 
        //        if (dbContext.IMS_UNLOCK_DETAILS.Where(x => x.MAST_PMGSY_SCHEME == 4 && x.IMS_UNLOCK_LEVEL == "S" && x.IMS_UNLOCK_TABLE == "CN" && x.IMS_UNLOCK_STATUS == "Y" && x.MAST_STATE_CODE == state && x.MAST_DISTRICT_CODE == null && x.MAST_BLOCK_CODE == null && (DateTime.Now >= x.IMS_UNLOCK_START_DATE && DateTime.Now <= x.IMS_UNLOCK_END_DATE)).Any())
        //        {
        //            flag = true;
        //            return flag;
        //        }// District
        //        else if (dbContext.IMS_UNLOCK_DETAILS.Where(x => x.MAST_PMGSY_SCHEME == 4 && x.IMS_UNLOCK_LEVEL == "D" && x.IMS_UNLOCK_TABLE == "CN" && x.IMS_UNLOCK_STATUS == "Y" && x.MAST_STATE_CODE == state && x.MAST_DISTRICT_CODE == district && x.MAST_BLOCK_CODE == null && (DateTime.Now >= x.IMS_UNLOCK_START_DATE && DateTime.Now <= x.IMS_UNLOCK_END_DATE)).Any())
        //        {
        //            flag = true;
        //            return flag;
        //        }// Block
        //        else if (dbContext.IMS_UNLOCK_DETAILS.Where(x => x.MAST_PMGSY_SCHEME == 4 && x.IMS_UNLOCK_LEVEL == "B" && x.IMS_UNLOCK_TABLE == "CN" && x.IMS_UNLOCK_STATUS == "Y" && x.MAST_STATE_CODE == state && x.MAST_DISTRICT_CODE == null && x.MAST_BLOCK_CODE == block && (DateTime.Now >= x.IMS_UNLOCK_START_DATE && DateTime.Now <= x.IMS_UNLOCK_END_DATE)).Any())
        //        {
        //            flag = true;
        //            return flag;
        //        }
        //        else
        //        {
        //            flag = false;
        //            return flag;
        //        }

        //        // flag = Convert.ToInt32(query) > 0 ? true : false;

        //        //  return flag;

        //    }
        //    catch (Exception ex)
        //    {
        //        ErrorLog.LogError(ex, "CoreNetworkDAL.checkIsUnlocked()");
        //        return false;
        //    }
        //    finally
        //    {
        //        if (dbContext != null)
        //        {
        //            dbContext.Dispose();
        //        }
        //    }
        //}
        #endregion

        #region Village Vibrant Programme

        /// <summary>
        /// Returns the list of Core Network roads
        /// </summary>
        /// <param name="blockCode"></param>
        /// <param name="roadType"></param>
        /// <param name="roadCode"></param>
        /// <param name="roadName"></param>
        /// <param name="page"></param>
        /// <param name="rows"></param>
        /// <param name="sidx"></param>
        /// <param name="sord"></param>
        /// <param name="totalRecords"></param>
        /// <returns></returns>
        public Array GetCoreNetWorksListVVP(int stateCode, int districtCode, int blockCode, string roadType, int roadCode, string roadName, int page, int rows, string sidx, string sord, out long totalRecords, int CnCode)
        {
            string filters = string.Empty;
            string nameSearch = string.Empty;
            PMGSYEntities dbContext = new PMGSYEntities();

            try
            {
                // new change with data from stored procedure
                if (roadType == "0")
                {
                    roadType = null;
                }

                if (PMGSYSession.Current.RoleCode == 2)
                {
                    stateCode = PMGSYSession.Current.StateCode;
                }
                else if (PMGSYSession.Current.RoleCode == 22)
                {
                    stateCode = PMGSYSession.Current.StateCode;
                    districtCode = PMGSYSession.Current.DistrictCode;
                }

                short roleCode = dbContext.UM_User_Master.Where(m => m.UserID == PMGSYSession.Current.UserId).Select(m => m.DefaultRoleID).FirstOrDefault();

                var lstPlanRoads = dbContext.GET_CORE_NETWORKS((stateCode <= 0 ? 0 : stateCode), (districtCode <= 0 ? 0 : districtCode), (blockCode <= 0 ? 0 : blockCode), roadType, roadCode, roadName, roleCode).ToList();

                lstPlanRoads = lstPlanRoads.Where(m => m.MAST_PMGSY_SCHEME == PMGSYSession.Current.PMGSYScheme).ToList();

                if (CnCode > 0)
                {
                    lstPlanRoads = lstPlanRoads.Where(m => m.PLAN_CN_ROAD_CODE == CnCode).ToList();
                }

                totalRecords = lstPlanRoads.Count();

                if (sidx.Trim() != string.Empty)
                {
                    if (sord.ToString() == "asc")
                    {
                        switch (sidx)
                        {
                            case "PLAN_CN_ROAD_CODE":
                                lstPlanRoads = lstPlanRoads.OrderBy(x => x.PLAN_CN_ROAD_CODE).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                            case "PLAN_CN_ROAD_NUMBER":
                                lstPlanRoads = lstPlanRoads.OrderBy(x => x.PLAN_CN_ROAD_NUMBER).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                            //case "MAST_ER_ROAD_CODE":
                            //    lstPlanRoads = lstPlanRoads.OrderBy(x => x.MAST_ER_ROAD_CODE).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                            //    break;
                            case "PLAN_RD_NAME":
                                lstPlanRoads = lstPlanRoads.OrderBy(x => x.PLAN_RD_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                            case "PLAN_RD_FROM":
                                lstPlanRoads = lstPlanRoads.OrderBy(x => x.PLAN_RD_FROM).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                            case "PLAN_RD_TO":
                                lstPlanRoads = lstPlanRoads.OrderBy(x => x.PLAN_RD_TO).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                            case "PLAN_RD_FROM_CHAINAGE":
                                lstPlanRoads = lstPlanRoads.OrderBy(x => x.PLAN_RD_FROM_CHAINAGE).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                            case "PLAN_RD_TO_CHAINAGE":
                                lstPlanRoads = lstPlanRoads.OrderBy(x => x.PLAN_RD_TO_CHAINAGE).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                            case "PLAN_RD_LENGTH":
                                lstPlanRoads = lstPlanRoads.OrderBy(x => x.PLAN_RD_LENGTH).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                            case "PLAN_RD_TOTAL_LENGTH":
                                lstPlanRoads = lstPlanRoads.OrderBy(x => x.PLAN_RD_TOTAL_LEN).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                            case "PLAN_RD_ROUTE":
                                lstPlanRoads = lstPlanRoads.OrderBy(x => x.PLAN_RD_ROUTE).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                            default:
                                lstPlanRoads = lstPlanRoads.OrderBy(x => x.PLAN_CN_ROAD_CODE).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                        }
                    }
                    else
                    {
                        switch (sidx)
                        {
                            case "PLAN_CN_ROAD_CODE":
                                lstPlanRoads = lstPlanRoads.OrderByDescending(x => x.PLAN_CN_ROAD_CODE).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                            case "PLAN_CN_ROAD_NUMBER":
                                lstPlanRoads = lstPlanRoads.OrderByDescending(x => x.PLAN_CN_ROAD_NUMBER).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                            //case "MAST_ER_ROAD_CODE":
                            //    lstPlanRoads = lstPlanRoads.OrderByDescending(x => x.MAST_ER_ROAD_CODE).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                            //    break;
                            case "PLAN_RD_NAME":
                                lstPlanRoads = lstPlanRoads.OrderByDescending(x => x.PLAN_RD_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                            case "PLAN_RD_FROM":
                                lstPlanRoads = lstPlanRoads.OrderByDescending(x => x.PLAN_RD_FROM).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                            case "PLAN_RD_TO":
                                lstPlanRoads = lstPlanRoads.OrderByDescending(x => x.PLAN_RD_TO).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                            case "PLAN_RD_FROM_CHAINAGE":
                                lstPlanRoads = lstPlanRoads.OrderByDescending(x => x.PLAN_RD_FROM_CHAINAGE).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                            case "PLAN_RD_TO_CHAINAGE":
                                lstPlanRoads = lstPlanRoads.OrderByDescending(x => x.PLAN_RD_TO_CHAINAGE).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                            case "PLAN_RD_LENGTH":
                                lstPlanRoads = lstPlanRoads.OrderByDescending(x => x.PLAN_RD_LENGTH).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                            case "PLAN_RD_TOTAL_LENGTH":
                                lstPlanRoads = lstPlanRoads.OrderByDescending(x => x.PLAN_RD_TOTAL_LEN).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                            case "PLAN_RD_ROUTE":
                                lstPlanRoads = lstPlanRoads.OrderByDescending(x => x.PLAN_RD_ROUTE).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                            default:
                                lstPlanRoads = lstPlanRoads.OrderByDescending(x => x.PLAN_CN_ROAD_CODE).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                        }
                    }
                }
                else
                {
                    lstPlanRoads = lstPlanRoads.OrderByDescending(x => x.PLAN_CN_ROAD_CODE).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                }

                var result = lstPlanRoads.Select(roadDetails => new
                {
                    //roadDetails.MAST_ER_ROAD_NUMBER,
                    roadDetails.BLOCK_NAME,
                    roadDetails.DISTRICT_NAME,
                    //roadDetails.ER,
                    roadDetails.STATE_NAME,
                    roadDetails.PLAN_CN_ROAD_CODE,
                    roadDetails.PLAN_CN_ROAD_NUMBER,
                    //roadDetails.PLAN_LOCK_STATUS,
                    roadDetails.PLAN_RD_BLOCK_FROM_CODE,
                    roadDetails.PLAN_RD_BLOCK_TO_CODE,
                    roadDetails.PLAN_RD_FROM_CHAINAGE,
                    roadDetails.PLAN_RD_FROM_HAB,
                    roadDetails.PLAN_RD_FROM_TYPE,
                    roadDetails.PLAN_RD_LENG,
                    roadDetails.PLAN_RD_LENGTH,
                    roadDetails.PLAN_RD_NAME,
                    roadDetails.PLAN_RD_NUM_FROM,
                    roadDetails.PLAN_RD_NUM_TO,
                    roadDetails.PLAN_RD_ROUTE,
                    roadDetails.PLAN_RD_TO_CHAINAGE,
                    roadDetails.PLAN_RD_TO_HAB,
                    roadDetails.PLAN_RD_TO_TYPE,
                    roadDetails.PLAN_RD_FROM,
                    roadDetails.PLAN_RD_TO,
                    roadDetails.UNLOCK_BY_MORD,
                    roadDetails.MAST_ER_ROAD_NUMBER,
                    roadDetails.PLAN_RD_TOTAL_LEN
                }).ToArray();

                return result.Select(roadDetails => new
                {
                    id = URLEncrypt.EncryptParameters1(new string[] { "NetworkCode=" + roadDetails.PLAN_CN_ROAD_CODE.ToString() }),
                    cell = new[]
                    {
                        roadDetails.PLAN_CN_ROAD_CODE.ToString().Trim(),
                        roadDetails.PLAN_RD_ROUTE == null?string.Empty:roadDetails.PLAN_RD_ROUTE.ToString(),
                        roadDetails.PLAN_CN_ROAD_NUMBER == null?string.Empty:roadDetails.PLAN_CN_ROAD_NUMBER.ToString(),
                        roadDetails.PLAN_RD_NAME == null?string.Empty:roadDetails.PLAN_RD_NAME.ToString(),
                        roadDetails.MAST_ER_ROAD_NUMBER.ToString(),
                        roadDetails.PLAN_RD_FROM_TYPE=="B"?"Block("+(dbContext.MASTER_BLOCK.Where(item=>item.MAST_BLOCK_CODE == roadDetails.PLAN_RD_BLOCK_FROM_CODE).Select(m=>m.MAST_BLOCK_NAME).FirstOrDefault())+")":(roadDetails.PLAN_RD_FROM_TYPE=="H"?"Habitation("+(dbContext.MASTER_HABITATIONS.Where(item=>item.MAST_HAB_CODE == roadDetails.PLAN_RD_FROM_HAB).Select(m=>m.MAST_HAB_NAME).FirstOrDefault())+")":(roadDetails.PLAN_RD_FROM==null?string.Empty:roadDetails.PLAN_RD_FROM.ToString())),
                        roadDetails.PLAN_RD_TO_TYPE=="B"?"Block("+(dbContext.MASTER_BLOCK.Where(item=>item.MAST_BLOCK_CODE == roadDetails.PLAN_RD_BLOCK_TO_CODE).Select(m=>m.MAST_BLOCK_NAME).FirstOrDefault())+")":(roadDetails.PLAN_RD_TO_TYPE=="H"?"Habitation("+(dbContext.MASTER_HABITATIONS.Where(item=>item.MAST_HAB_CODE == roadDetails.PLAN_RD_TO_HAB).Select(m=>m.MAST_HAB_NAME).FirstOrDefault())+")":(roadDetails.PLAN_RD_TO==null?string.Empty:roadDetails.PLAN_RD_TO.ToString())),

                        roadDetails.PLAN_RD_FROM_CHAINAGE == null?string.Empty:roadDetails.PLAN_RD_FROM_CHAINAGE.ToString(),
                        roadDetails.PLAN_RD_TO_CHAINAGE == null?string.Empty:roadDetails.PLAN_RD_TO_CHAINAGE.ToString(),
                        roadDetails.PLAN_RD_LENGTH == null?string.Empty:roadDetails.PLAN_RD_LENGTH.ToString(),
                        (PMGSYSession.Current.PMGSYScheme == 2 || PMGSYSession.Current.PMGSYScheme == 5) ? (roadDetails.PLAN_RD_TOTAL_LEN == null ? Convert.ToString(roadDetails.PLAN_RD_TO_CHAINAGE - roadDetails.PLAN_RD_FROM_CHAINAGE) :  roadDetails.PLAN_RD_TOTAL_LEN.ToString()) : "",

                        // HABITATION MAPPING COLUMN
                        roadDetails.UNLOCK_BY_MORD == "M" ? "<a href='#' title='Click here to map habitation Details' class='ui-icon ui-icon-unlocked ui-align-center' onClick =editHabitationDetails('"+URLEncrypt.EncryptParameters1(new string[]{"NetworkCode =" + roadDetails.PLAN_CN_ROAD_CODE.ToString().Trim(),"UnlockFlag =" + roadDetails.UNLOCK_BY_MORD.ToString().Trim()})+"'); return false;'>Delete</a>"
                                                          : roadDetails.UNLOCK_BY_MORD=="N" ? "<a href='#' title='Click here to map habitation Details' class='ui-icon ui-icon-plusthick ui-align-center' onClick =editHabitationDetails('"+URLEncrypt.EncryptParameters1(new string[]{"NetworkCode =" + roadDetails.PLAN_CN_ROAD_CODE.ToString().Trim(),"UnlockFlag =" + roadDetails.UNLOCK_BY_MORD.ToString().Trim()})+"'); return false;'>Delete</a>"
                                                                                            :"<span class='ui-icon ui-icon-locked ui-align-center'></span>",

                        // UPLOAD FILES COLUMN
                        (roadDetails.UNLOCK_BY_MORD == "M" ? "<a href='#' title='Click here to upload file details' class='ui-icon ui-icon-unlocked ui-align-center' onClick =UploadFile('"+URLEncrypt.EncryptParameters1(new string[]{"NetworkCode =" + roadDetails.PLAN_CN_ROAD_CODE.ToString().Trim()})+"'); return false;'>Delete</a>"
                                                           : roadDetails.UNLOCK_BY_MORD=="N" ? "<a href='#' title='Click here to upload file details' class='ui-icon ui-icon-plusthick ui-align-center' onClick =UploadFile('"+URLEncrypt.EncryptParameters1(new string[]{"NetworkCode =" + roadDetails.PLAN_CN_ROAD_CODE.ToString().Trim()})+"'); return false;'>Delete</a>"
                                                                                             : "<span class='ui-icon ui-icon-locked ui-align-center'></span></center>"),

                        // MAP DRRP ROAD COLUMN
                        PMGSYSession.Current.PMGSYScheme == 1 ? ""
                                                              : roadDetails.UNLOCK_BY_MORD == "M" ? ("<a href='#' title='Click here to map other candidate road' class='ui-icon ui-icon-unlocked ui-align-center' onClick =MapOtherCandidateRoad('"+URLEncrypt.EncryptParameters1(new string[]{"NetworkCode =" + roadDetails.PLAN_CN_ROAD_CODE.ToString().Trim()})+"'); return false;'>Map Other DRRP Road</a>")
                                                                                                  : roadDetails.UNLOCK_BY_MORD == "Y" ? "<span class='ui-icon ui-icon-locked ui-align-center'></span>"
                                                                                                                                      : "<a href='#' title='Click here to map other candidate road' class='ui-icon ui-icon-plusthick ui-align-center' onClick =MapOtherCandidateRoad('"+URLEncrypt.EncryptParameters1(new string[]{"NetworkCode =" + roadDetails.PLAN_CN_ROAD_CODE.ToString().Trim()})+"'); return false;'>Map Other DRRP Road</a>",

                        // VIEW CORE NETWORK DETAIL COLUMN
                        "<a href='#' title='Click here to view core network details' class='ui-icon ui-icon-zoomin ui-align-center' onClick =detailsCoreNetwork('"+URLEncrypt.EncryptParameters1(new string[]{"NetworkCode =" + roadDetails.PLAN_CN_ROAD_CODE.ToString().Trim()})+"'); return false;'></a>",
                    
                        // EDIT
                        ///Changes by SAMMED A. PATIL on 03 OCTOBER 2017 to edit DRRP for MORD
                        /*
                          PMGSYSession.Current.RoleCode == 25
                            ? "<a href='#' title='Click here to view core network details' class='ui-icon ui-icon-pencil ui-align-center' onClick =editNetworkData('"+URLEncrypt.EncryptParameters1(new string[]{"NetworkCode =" + roadDetails.PLAN_CN_ROAD_CODE.ToString().Trim()})+"'); return false;'>Edit</a>"
                            : (roadDetails.UNLOCK_BY_MORD == "M"
                                                                ? "<a href='#' title='Click here to edit core network details' class='ui-icon ui-icon-unlocked ui-align-center' onClick =editNetworkData('"+URLEncrypt.EncryptParameters1(new string[]{"NetworkCode =" + roadDetails.PLAN_CN_ROAD_CODE.ToString().Trim(),"UnlockFlag =" + roadDetails.UNLOCK_BY_MORD.ToString().Trim()})+"'); return false;'>Delete</a>"
                                                                : roadDetails.UNLOCK_BY_MORD=="N"
                                                                                                ? "<a href='#' title='Click here to view core network details' class='ui-icon ui-icon-pencil ui-align-center' onClick =editNetworkData('"+URLEncrypt.EncryptParameters1(new string[]{"NetworkCode =" + roadDetails.PLAN_CN_ROAD_CODE.ToString().Trim()})+"'); return false;'>Edit</a>"
                                                                                                :"<span class='ui-icon ui-icon-locked ui-align-center'></span></center>"),
                        */

                        (roadDetails.UNLOCK_BY_MORD == "M" ? "<a href='#' title='Click here to edit core network details' class='ui-icon ui-icon-unlocked ui-align-center' onClick =editNetworkData('"+URLEncrypt.EncryptParameters1(new string[]{"NetworkCode =" + roadDetails.PLAN_CN_ROAD_CODE.ToString().Trim(),"UnlockFlag =" + roadDetails.UNLOCK_BY_MORD.ToString().Trim()})+"'); return false;'>Delete</a>"
                                                           : roadDetails.UNLOCK_BY_MORD=="N" ? "<a href='#' title='Click here to view core network details' class='ui-icon ui-icon-pencil ui-align-center' onClick =editNetworkData('"+URLEncrypt.EncryptParameters1(new string[]{"NetworkCode =" + roadDetails.PLAN_CN_ROAD_CODE.ToString().Trim()})+"'); return false;'>Edit</a>"
                                                                                             :"<span class='ui-icon ui-icon-locked ui-align-center'></span></center>"),

                        
                        // DELETE
                        /*
                        roadDetails.UNLOCK_BY_MORD == "M"   
                                                        ? "<a href='#' title='Click here to delete core network details' class='ui-icon ui-icon-trash ui-align-center' onClick =deleteNetworkData('"+URLEncrypt.EncryptParameters1(new string[]{"NetworkCode =" + roadDetails.PLAN_CN_ROAD_CODE.ToString().Trim()})+"'); return false;'>Delete</a>"
                                                        : roadDetails.UNLOCK_BY_MORD=="N"
                                                                                        ? "<a href='#' title='Click here to delete core network details' class='ui-icon ui-icon-trash ui-align-center' onClick =deleteNetworkData('"+URLEncrypt.EncryptParameters1(new string[]{"NetworkCode =" + roadDetails.PLAN_CN_ROAD_CODE.ToString().Trim()})+"'); return false;'>Delete</a>"
                                                                                        : "<span class='ui-icon ui-icon-locked ui-align-center'></span></center>",
                        */
                        roadDetails.UNLOCK_BY_MORD=="N" ? "<a href='#' title='Click here to delete core network details' class='ui-icon ui-icon-trash ui-align-center' onClick =deleteNetworkData('"+URLEncrypt.EncryptParameters1(new string[]{"NetworkCode =" + roadDetails.PLAN_CN_ROAD_CODE.ToString().Trim()})+"'); return false;'>Delete</a>"
                                                        : "<span class='ui-icon ui-icon-locked ui-align-center'></span></center>",
                    }
                }).ToArray();

            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "GetCoreNetWorksList().DAL");
                totalRecords = 0;
                dbContext.Dispose();
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
        /// populates the availabe list of habitations to be mapped 
        /// </summary>
        /// <param name="roadCode">core network code</param>
        /// <param name="page">no. of pages</param>
        /// <param name="rows">no of rows</param>
        /// <param name="sidx">sort column</param>
        /// <param name="sord">sort order</param>
        /// <param name="totalRecords">no. of totalRecords</param>
        /// <returns>list of availabel Habitation list</returns>
        public Array GetHabitationListToMapVVP(int roadCode, int blockCode, int erRoadCode, int page, int rows, string sidx, string sord, out long totalRecords)
        {
            PMGSYEntities dbContext = new PMGSYEntities();
            totalRecords = 0;
            try
            {
                PLAN_ROAD masterRoad = dbContext.PLAN_ROAD.Find(roadCode);

                var lstHabitations = (from item in dbContext.MASTER_VILLAGE
                                      join habitation in dbContext.MASTER_HABITATIONS on item.MAST_VILLAGE_CODE equals habitation.MAST_VILLAGE_CODE
                                      join habDetails in dbContext.MASTER_HABITATIONS_DETAILS on habitation.MAST_HAB_CODE equals habDetails.MAST_HAB_CODE
                                      where item.MAST_BLOCK_CODE == blockCode &&//masterRoad.MAST_BLOCK_CODE && commented by Vikram in order to map habitations of other blocks also
                                                                                //(habitation.MAST_HAB_STATUS == "U" || habitation.MAST_HAB_STATUS == "C")
                                      habitation.MAST_HABITATION_ACTIVE == "Y" //new condition added by Vikram and above line commented as per suggestion from Dev Sir
                                      select new
                                      {
                                          habitation.MAST_HAB_NAME,
                                          item.MAST_VILLAGE_NAME,
                                          habDetails.MAST_HAB_TOT_POP,
                                          habitation.MAST_HAB_CODE,

                                      }).ToList();

                List<int> mapHabitations = (from item in dbContext.PLAN_ROAD_HABITATION
                                            join data in dbContext.PLAN_ROAD on item.PLAN_CN_ROAD_CODE equals data.PLAN_CN_ROAD_CODE
                                            where
                                            data.MAST_BLOCK_CODE == masterRoad.MAST_BLOCK_CODE &&
                                            data.MAST_PMGSY_SCHEME == (PMGSYSession.Current.PMGSYScheme)
                                            select item.MAST_HAB_CODE).Distinct().ToList<int>();

                if (PMGSYSession.Current.PMGSYScheme == 2 || PMGSYSession.Current.PMGSYScheme == 5)
                {
                    lstHabitations = (from item in dbContext.MASTER_VILLAGE
                                      join habitation in dbContext.MASTER_HABITATIONS on item.MAST_VILLAGE_CODE equals habitation.MAST_VILLAGE_CODE
                                      join habDetails in dbContext.MASTER_HABITATIONS_DETAILS on habitation.MAST_HAB_CODE equals habDetails.MAST_HAB_CODE
                                      join erHabs in dbContext.MASTER_ER_HABITATION_ROAD_PMGSY3 on habitation.MAST_HAB_CODE equals erHabs.MAST_HAB_CODE
                                      where erHabs.MAST_ER_ROAD_CODE == erRoadCode &&
                                      habitation.MAST_HABITATION_ACTIVE == "Y" 
                                      && habDetails.MAST_YEAR == 2011
                                      select new
                                      {
                                          habitation.MAST_HAB_NAME,
                                          item.MAST_VILLAGE_NAME,
                                          habDetails.MAST_HAB_TOT_POP,
                                          habitation.MAST_HAB_CODE,

                                      }).ToList();

                    mapHabitations = (from item in dbContext.PLAN_ROAD_HABITATION                                          
                                      where
                                      item.PLAN_CN_ROAD_CODE == roadCode &&
                                      item.PLAN_ROAD.MAST_PMGSY_SCHEME == (PMGSYSession.Current.PMGSYScheme)
                                      select item.MAST_HAB_CODE).Distinct().ToList<int>();
                }

                var listHab = (from item in lstHabitations
                               where !mapHabitations.Contains(item.MAST_HAB_CODE)
                               select item.MAST_HAB_CODE).Distinct();


                //dynamic mappingList = null;

                var route = dbContext.PLAN_ROAD.Where(x => x.PLAN_CN_ROAD_CODE == roadCode).Select(x => x.PLAN_RD_ROUTE).FirstOrDefault();
                if (route == "N")
                {
                    var mappingList = (from item in dbContext.MASTER_HABITATIONS
                                       join habitation in dbContext.MASTER_ER_HABITATION_ROAD on item.MAST_HAB_CODE equals habitation.MAST_HAB_CODE
                                       join planHab in dbContext.MASTER_ER_HABITATION_ROAD on item.MAST_HAB_CODE equals planHab.MAST_HAB_CODE
                                       join habDetails1 in dbContext.MASTER_HABITATIONS_DETAILS on item.MAST_HAB_CODE equals habDetails1.MAST_HAB_CODE
                                       join planRoad in dbContext.PLAN_ROAD on planHab.MAST_ER_ROAD_CODE equals planRoad.MAST_ER_ROAD_CODE
                                       where planRoad.PLAN_CN_ROAD_CODE == roadCode
                                       && item.MASTER_VILLAGE.MAST_BLOCK_CODE == blockCode
                                       && habDetails1.MAST_YEAR == (PMGSYSession.Current.PMGSYScheme == 1 ? 2001 : 2011)
                                       && item.MAST_HABITATION_ACTIVE == "Y"
                                       select new
                                       {
                                           item.MAST_HAB_CODE,
                                           item.MAST_HAB_NAME,
                                           item.MASTER_VILLAGE.MAST_VILLAGE_NAME,
                                           habDetails1.MAST_HAB_TOT_POP
                                       }).Distinct().ToList();

                    totalRecords = mappingList.Count();

                    if (sidx.Trim() != string.Empty)
                    {
                        if (sord.ToString() == "asc")
                        {
                            mappingList = mappingList.OrderBy(x => x.MAST_HAB_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                        }
                        else
                        {
                            mappingList = mappingList.OrderByDescending(x => x.MAST_HAB_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                        }
                    }
                    else
                    {
                        mappingList = mappingList.OrderBy(x => x.MAST_HAB_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                    }

                    var result = mappingList.Select(habDetails => new
                    {
                        habDetails.MAST_HAB_CODE,
                        habDetails.MAST_HAB_NAME,
                        habDetails.MAST_VILLAGE_NAME,
                        habDetails.MAST_HAB_TOT_POP
                    }).ToArray();



                    return result.Select(habDetails => new
                    {
                        id = URLEncrypt.EncryptParameters1(new string[] { "HabCode =" + habDetails.MAST_HAB_CODE.ToString().Trim() }),
                        cell = new[] {
                        
                        //habDetails.MAST_HAB_CODE.ToString(),    
                        habDetails.MAST_HAB_NAME.ToString(),
                        habDetails.MAST_VILLAGE_NAME.ToString(),
                        habDetails.MAST_HAB_TOT_POP.ToString()
                    }
                    }).ToArray();

                }
                else
                {
                    var mappingList = (from item in dbContext.MASTER_HABITATIONS_DETAILS
                                       join habitation in dbContext.MASTER_HABITATIONS on item.MAST_HAB_CODE equals habitation.MAST_HAB_CODE
                                       join village in dbContext.MASTER_VILLAGE on habitation.MAST_VILLAGE_CODE equals village.MAST_VILLAGE_CODE
                                       where listHab.Contains(item.MAST_HAB_CODE) &&
                                       item.MAST_YEAR == (PMGSYSession.Current.PMGSYScheme == 1 ? 2001 : 2011)
                                       select new
                                       {
                                           item.MAST_HAB_CODE,
                                           habitation.MAST_HAB_NAME,
                                           village.MAST_VILLAGE_NAME,
                                           item.MAST_HAB_TOT_POP
                                       }).Distinct().ToList();

                    totalRecords = mappingList.Count();

                    if (sidx.Trim() != string.Empty)
                    {
                        if (sord.ToString() == "asc")
                        {
                            mappingList = mappingList.OrderBy(x => x.MAST_HAB_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                        }
                        else
                        {
                            mappingList = mappingList.OrderByDescending(x => x.MAST_HAB_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                        }
                    }
                    else
                    {
                        mappingList = mappingList.OrderBy(x => x.MAST_HAB_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                    }

                    var result = mappingList.Select(habDetails => new
                    {
                        habDetails.MAST_HAB_CODE,
                        habDetails.MAST_HAB_NAME,
                        habDetails.MAST_VILLAGE_NAME,
                        habDetails.MAST_HAB_TOT_POP
                    }).ToArray();



                    return result.Select(habDetails => new
                    {
                        id = URLEncrypt.EncryptParameters1(new string[] { "HabCode =" + habDetails.MAST_HAB_CODE.ToString().Trim() }),
                        cell = new[] {
                        
                        //habDetails.MAST_HAB_CODE.ToString(),    
                        habDetails.MAST_HAB_NAME.ToString(),
                        habDetails.MAST_VILLAGE_NAME.ToString(),
                        habDetails.MAST_HAB_TOT_POP.ToString()
                    }
                    }).ToArray();
                }


                //totalRecords = mappingList.Count();

                //if (sidx.Trim() != string.Empty)
                //{
                //    if (sord.ToString() == "asc")
                //    {
                //        mappingList = mappingList.OrderBy(x => x.MAST_HAB_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                //    }
                //    else
                //    {
                //        mappingList = mappingList.OrderByDescending(x => x.MAST_HAB_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                //    }
                //}
                //else
                //{
                //    mappingList = mappingList.OrderBy(x => x.MAST_HAB_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                //}

                //var result = mappingList.Select(habDetails => new
                //{
                //    habDetails.MAST_HAB_CODE,
                //    habDetails.MAST_HAB_NAME,
                //    habDetails.MAST_VILLAGE_NAME,
                //    habDetails.MAST_HAB_TOT_POP
                //}).ToArray();



                //return result.Select(habDetails => new
                //{
                //    id = URLEncrypt.EncryptParameters1(new string[] { "HabCode =" + habDetails.MAST_HAB_CODE.ToString().Trim() }),
                //    cell = new[] {

                //        //habDetails.MAST_HAB_CODE.ToString(),    
                //        habDetails.MAST_HAB_NAME.ToString(),
                //        habDetails.MAST_VILLAGE_NAME.ToString(),
                //        habDetails.MAST_HAB_TOT_POP.ToString()
                //    }
                //}).ToArray();
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

        public bool DeleteMappedDRRPDetailsVVP(int DRRPCode, int CNCode)
        {
            PMGSYEntities dbContext = new PMGSYEntities();
            try
            {
                using (TransactionScope ts = new TransactionScope())
                {
                    PLAN_ROAD_DRRP mappedMaster = dbContext.PLAN_ROAD_DRRP.Where(m => m.PLAN_CN_ROAD_CODE == CNCode && m.MAST_ER_ROAD_CODE == DRRPCode).FirstOrDefault();
                    if (mappedMaster != null)
                    {
                        dbContext.PLAN_ROAD_DRRP.Remove(mappedMaster);
                        dbContext.SaveChanges();

                        //removing the mapped habitations of current DRRP
                        var drrpMappedHabs = dbContext.MASTER_ER_HABITATION_ROAD_PMGSY3.Where(m => m.MAST_ER_ROAD_CODE == DRRPCode).Select(m => m.MAST_HAB_CODE).Distinct().ToList();

                        var mappedDRRPHabList = (from item in dbContext.PLAN_ROAD_DRRP
                                                 join
                                                 erHab in dbContext.MASTER_ER_HABITATION_ROAD_PMGSY3
                                                 on item.MAST_ER_ROAD_CODE equals erHab.MAST_ER_ROAD_CODE
                                                 where
                                                 item.MAST_ER_ROAD_CODE != DRRPCode &&
                                                 item.PLAN_CN_ROAD_CODE == CNCode
                                                 select new
                                                 {
                                                     erHab.MAST_HAB_CODE
                                                 }).Distinct().GroupBy(m => m.MAST_HAB_CODE).Select(m => m.FirstOrDefault()).ToList();


                        var lstHabsPlanRoad = (from item in dbContext.PLAN_ROAD
                                               join
                                               erHab in dbContext.MASTER_ER_HABITATION_ROAD_PMGSY3
                                               on item.MAST_ER_ROAD_CODE equals erHab.MAST_ER_ROAD_CODE
                                               where
                                               item.MAST_ER_ROAD_CODE != DRRPCode &&
                                               item.PLAN_CN_ROAD_CODE == CNCode
                                               select new
                                               {
                                                   erHab.MAST_HAB_CODE
                                               }).Distinct().GroupBy(m => m.MAST_HAB_CODE).Select(m => m.FirstOrDefault()).ToList();

                        mappedDRRPHabList = mappedDRRPHabList.Union(lstHabsPlanRoad).ToList();

                        List<int> lstSameHabs = null;

                        if (mappedDRRPHabList.Any(m => drrpMappedHabs.Contains(m.MAST_HAB_CODE)))
                        {
                            lstSameHabs = mappedDRRPHabList.Where(m => drrpMappedHabs.Contains(m.MAST_HAB_CODE)).Select(m => m.MAST_HAB_CODE).ToList();
                        }

                        var habsToRemove = (lstSameHabs == null ? drrpMappedHabs : drrpMappedHabs.Where(m => !lstSameHabs.Contains(m)).ToList());

                        if (habsToRemove != null)
                        {
                            foreach (var item in habsToRemove)
                            {
                                PLAN_ROAD_HABITATION cnHabMapping = dbContext.PLAN_ROAD_HABITATION.Where(m => m.PLAN_CN_ROAD_CODE == CNCode && m.MAST_HAB_CODE == item).FirstOrDefault();
                                if (cnHabMapping != null)
                                {
                                    dbContext.PLAN_ROAD_HABITATION.Remove(cnHabMapping);
                                    dbContext.SaveChanges();
                                }
                            }
                        }
                        ts.Complete();
                        return true;
                    }
                    else
                    {
                        return false;
                    }

                }
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


        /// <summary>
        /// saves the core network details
        /// </summary>
        /// <param name="model"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public bool AddCoreNetworksVVP(CoreNetworkViewModel model, ref string message)
        {
            PMGSYEntities dbContext = new PMGSYEntities();
            try
            {

                decimal chainageFrom = Convert.ToDecimal(model.PLAN_RD_FROM_CHAINAGE);
                decimal chainageTo = Convert.ToDecimal(model.PLAN_RD_TO_CHAINAGE);
                int recordCount = dbContext.PLAN_ROAD.Where(m => m.MAST_STATE_CODE == model.MAST_STATE_CODE && m.MAST_DISTRICT_CODE == model.MAST_DISTRICT_CODE && m.MAST_BLOCK_CODE == model.MAST_BLOCK_CODE && m.PLAN_CN_ROAD_NUMBER == model.PLAN_CN_ROAD_NUMBER && m.PLAN_RD_FROM_CHAINAGE == chainageFrom && m.PLAN_RD_TO_CHAINAGE == chainageTo && m.MAST_PMGSY_SCHEME == PMGSYSession.Current.PMGSYScheme).Count();

                if (recordCount > 0)
                {
                    if (PMGSYSession.Current.PMGSYScheme == 1)
                    {
                        message = "Core Network information already exist.";
                    }
                    // added by rohit for vibrant village prog on 20-07-2023
                    else if (PMGSYSession.Current.PMGSYScheme == 5)
                    {
                        message = "Candidate Road details already present.";
                    }
                    return false;
                }

                PLAN_ROAD master = new PLAN_ROAD();
                master = CloneModelToObject(model);
                master.USERID = PMGSYSession.Current.UserId;
                master.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                master.PLAN_CN_ROAD_CODE = dbContext.PLAN_ROAD.Any() ? (from item in dbContext.PLAN_ROAD select item.PLAN_CN_ROAD_CODE).Max() + 1 : 1;
                dbContext.PLAN_ROAD.Add(master);
                dbContext.SaveChanges();
                
                // added by rohit for vibrant village prog on 20-07-2023
                if (PMGSYSession.Current.PMGSYScheme == 5)
                {
                    List<int> lstHabCodes = dbContext.MASTER_ER_HABITATION_ROAD_PMGSY3.Where(m => m.MAST_ER_ROAD_CODE == master.MAST_ER_ROAD_CODE).Select(m => m.MAST_HAB_CODE).ToList();
                    if (lstHabCodes != null)
                    {
                        foreach (var item in lstHabCodes)
                        {
                            PLAN_ROAD_HABITATION mappingMaster = new PLAN_ROAD_HABITATION();
                            mappingMaster.MAST_HAB_CODE = item;
                            mappingMaster.PLAN_CN_ROAD_CODE = master.PLAN_CN_ROAD_CODE;
                            mappingMaster.PLAN_CN_ROAD_HAB_ID = dbContext.PLAN_ROAD_HABITATION.Any() ? dbContext.PLAN_ROAD_HABITATION.Max(m => m.PLAN_CN_ROAD_HAB_ID) + 1 : 1;
                            mappingMaster.USERID = PMGSYSession.Current.UserId;
                            mappingMaster.IPADD = master.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                            dbContext.PLAN_ROAD_HABITATION.Add(mappingMaster);
                            dbContext.SaveChanges();
                        }
                    }
                }
                //end of change


                return true;
            }
            catch (Exception ex)
            {
                //objLog.WriteErrorMessage(ex.Message);
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


        /// <summary>
        /// update the core network details
        /// </summary>
        /// <param name="model"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public bool EditCoreNetworksVVP(CoreNetworkViewModel model, ref string message)
        {
            PMGSYEntities dbContext = new PMGSYEntities();
            try
            {
                int networkCode = 0;
                encryptedParameters = model.EncryptedRoadCode.Split('/');
                if (!(encryptedParameters.Length == 3))
                {
                    return false;
                }

                decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { encryptedParameters[0], encryptedParameters[1], encryptedParameters[2] });
                networkCode = Convert.ToInt32(decryptedParameters["NetworkCode"].ToString());

                decimal chainageFrom = Convert.ToDecimal(model.PLAN_RD_FROM_CHAINAGE);
                decimal chainageTo = Convert.ToDecimal(model.PLAN_RD_TO_CHAINAGE);

                // added by rohit for vibrant village prog on 20-07-2023
                if (PMGSYSession.Current.PMGSYScheme == 2 || PMGSYSession.Current.PMGSYScheme == 5)
                {
                    if (dbContext.PLAN_ROAD_DRRP.Any(m => m.PLAN_CN_ROAD_CODE == networkCode))
                    {
                        string DRRP_lockStatus = dbContext.PLAN_ROAD_DRRP.Where(m => m.PLAN_CN_ROAD_CODE == networkCode).Select(m => m.PLAN_LOCK_STATUS).FirstOrDefault();
                        if (DRRP_lockStatus.Trim().Equals("Y"))
                        {
                            if (model.TotalLengthOfCandidate > (dbContext.PLAN_ROAD_DRRP.Where(m => m.PLAN_CN_ROAD_CODE == networkCode).Sum(m => m.PLAN_RD_LENGTH) + Convert.ToDecimal(model.PLAN_RD_LENGTH)))
                            {
                                message = "Total length of Candidate Road should not be greater than the sum of candidate road length and other DRRP road length mapped to it.";
                                return false;
                            }
                        }

                        if (model.TotalLengthOfCandidate < (dbContext.PLAN_ROAD_DRRP.Where(m => m.PLAN_CN_ROAD_CODE == networkCode).Sum(m => m.PLAN_RD_LENGTH) + Convert.ToDecimal(model.PLAN_RD_LENGTH)))
                        {
                            message = "Total length of Candidate Road should not be less than the sum of candidate road length and other DRRP road length mapped to it.";
                            return false;
                        }
                    }
                }

                int recordCount = dbContext.PLAN_ROAD.Where(m => m.MAST_STATE_CODE == model.MAST_STATE_CODE && m.MAST_DISTRICT_CODE == model.MAST_DISTRICT_CODE && m.MAST_BLOCK_CODE == model.MAST_BLOCK_CODE && m.PLAN_CN_ROAD_NUMBER == model.PLAN_CN_ROAD_NUMBER && m.PLAN_RD_FROM_CHAINAGE == chainageFrom && m.PLAN_RD_TO_CHAINAGE == chainageTo && m.PLAN_CN_ROAD_CODE != networkCode && m.MAST_PMGSY_SCHEME == PMGSYSession.Current.PMGSYScheme).Count();
                if (recordCount > 0)
                {
                    if (PMGSYSession.Current.PMGSYScheme == 1)
                    {
                        message = "Core Network information already exist.";
                    }
                    // added by rohit for vibrant village prog on 20-07-2023
                    //else if (PMGSYSession.Current.PMGSYScheme == 2)
                    else if (PMGSYSession.Current.PMGSYScheme == 2 || PMGSYSession.Current.PMGSYScheme == 5)
                    {
                        message = "Candidate Road details already present.";
                    }
                    return false;
                }

                PLAN_ROAD master = dbContext.PLAN_ROAD.Where(m => m.PLAN_CN_ROAD_CODE == networkCode).FirstOrDefault();
                if (master != null)
                {
                    master = CloneModelToObject(model);
                    master.PLAN_CN_ROAD_CODE = networkCode;
                    master.USERID = PMGSYSession.Current.UserId;
                    master.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                    //dbContext.Entry(master).State = System.Data.Entity.EntityState.Modified;
                    var currentProduct = dbContext.PLAN_ROAD.Where(m => m.PLAN_CN_ROAD_CODE == networkCode).FirstOrDefault(); ;
                    dbContext.Entry(currentProduct).CurrentValues.SetValues(master);
                    dbContext.SaveChanges();
                    return true;
                }
                else
                {
                    return false;
                }

            }
            catch (Exception ex)
            {
                //objLog.WriteErrorMessage(ex.Message);
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


        /// <summary>
        /// maps the DRRP details with the candidate road
        /// </summary>
        /// <param name="model">contains the mapping details</param>
        /// <param name="message"></param>
        /// <returns></returns>
        public bool MapCandidateRoadVVP(CandidateRoadViewModel model, ref string message)
        {
            PMGSYEntities dbContext = new PMGSYEntities();
            try
            {
                using (TransactionScope ts = new TransactionScope())
                {

                    //check whether the road is already mapped with the same Candidate Road or not
                    if (dbContext.PLAN_ROAD_DRRP.Any(m => m.PLAN_CN_ROAD_CODE == model.CNCode && m.MAST_ER_ROAD_CODE == model.DRRPCode))
                    {
                        message = "DRRP is already mapped with the Candidate Road.";
                        return false;
                    }

                    //check whether the DRRP on which candidate  road is made is mapped with the same Candidate Road
                    if (dbContext.PLAN_ROAD.Any(m => m.PLAN_CN_ROAD_CODE == model.CNCode && m.MAST_ER_ROAD_CODE == model.DRRPCode && m.MAST_PMGSY_SCHEME == PMGSYSession.Current.PMGSYScheme))
                    {
                        message = "DRRP on which the candidate road is made can not be mapped with the same candidate road.";
                        return false;
                    }

                    //if (dbContext.PLAN_ROAD_DRRP.Any(m => m.MAST_ER_ROAD_CODE == model.DRRPCode))
                    //{
                    //    decimal? mappedDRRPLength = dbContext.PLAN_ROAD_DRRP.Where(m=>m.MAST_ER_ROAD_CODE == model.DRRPCode).Sum(m => m.PLAN_RD_LENGTH);
                    //    decimal? drrpLength = (dbContext.MASTER_EXISTING_ROADS.Where(m => m.MAST_ER_ROAD_CODE == model.DRRPCode).Select(m => m.MAST_ER_ROAD_END_CHAIN).FirstOrDefault() - dbContext.MASTER_EXISTING_ROADS.Where(m => m.MAST_ER_ROAD_CODE == model.DRRPCode).Select(m => m.MAST_ER_ROAD_STR_CHAIN).FirstOrDefault());
                    //    if (dbContext.MASTER_EXISTING_ROADS.Any(m => m.MAST_ER_ROAD_CODE == model.DRRPCode))
                    //    {
                    //        decimal? allowedDRRPLength =  drrpLength - mappedDRRPLength;
                    //        if (allowedDRRPLength < model.LengthOfRoad)
                    //        {
                    //            message = "The Total Length of DRRP ( "+drrpLength+" )  should be equal to the total of DRRP Length mapped to the Candidate Roads.";
                    //            return false;
                    //        }
                    //    }
                    //}


                    //validation for checking the Total Candidate Road length with the mapped road length
                    //decimal? totalCandidateRoadLength = dbContext.PLAN_ROAD.Where(m=>m.PLAN_CN_ROAD_CODE == model.CNCode).Select(m=>m.PLAN_RD_TOTAL_LEN).FirstOrDefault();
                    PLAN_ROAD roadMaster = dbContext.PLAN_ROAD.Where(m => m.PLAN_CN_ROAD_CODE == model.CNCode).FirstOrDefault();
                    decimal? totalMappedRoadLength = 0;
                    if (dbContext.PLAN_ROAD_DRRP.Any(m => m.PLAN_CN_ROAD_CODE == model.CNCode))
                    {
                        totalMappedRoadLength = dbContext.PLAN_ROAD_DRRP.Where(m => m.PLAN_CN_ROAD_CODE == model.CNCode).Sum(m => m.PLAN_RD_LENGTH);
                        if (totalMappedRoadLength == null)
                        {
                            totalMappedRoadLength = 0;
                        }
                        totalMappedRoadLength = totalMappedRoadLength + model.LengthOfRoad + roadMaster.PLAN_RD_LENGTH;
                    }
                    else
                    {
                        totalMappedRoadLength = model.LengthOfRoad + roadMaster.PLAN_RD_LENGTH;
                    }

                    if (roadMaster.PLAN_RD_TOTAL_LEN < totalMappedRoadLength)
                    {
                        ///Changes for RCPLWE
                        /// added by rohit for vibrant village prog on 20-07-2023
                        message = (PMGSYSession.Current.PMGSYScheme == 2 || PMGSYSession.Current.PMGSYScheme == 5) ? "Total of mapped DRRP road length exceeding the total length of Candidate Road." : "Total of mapped DRRP road length exceeding the total length of RCPLWE Road.";
                        return false;
                    }


                    PLAN_ROAD_DRRP mappingMaster = new PLAN_ROAD_DRRP();
                    mappingMaster.PLAN_CN_ROAD_CODE = model.CNCode;
                    mappingMaster.MAST_ER_ROAD_CODE = model.DRRPCode;
                    mappingMaster.PLAN_LOCK_STATUS = "N";
                    mappingMaster.PLAN_RD_FROM_CHAINAGE = model.StartChainage;
                    mappingMaster.PLAN_RD_TO_CHAINAGE = model.EndChainage;
                    mappingMaster.PLAN_RD_LENG = model.LengthTypeOfRoad;
                    mappingMaster.PLAN_RD_LENGTH = model.LengthOfRoad;
                    mappingMaster.USERID = PMGSYSession.Current.UserId;
                    mappingMaster.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                    dbContext.PLAN_ROAD_DRRP.Add(mappingMaster);
                    dbContext.SaveChanges();

                    //mapping DRRP Habitations
                    var lstDRRPHabs = dbContext.MASTER_ER_HABITATION_ROAD_PMGSY3.Where(m => m.MAST_ER_ROAD_CODE == model.DRRPCode).ToList();
                    foreach (var item in lstDRRPHabs)
                    {
                        if (!dbContext.PLAN_ROAD_HABITATION.Any(m => m.PLAN_CN_ROAD_CODE == model.CNCode && m.MAST_HAB_CODE == item.MAST_HAB_CODE))
                        {
                            PLAN_ROAD_HABITATION cnHabMapping = new PLAN_ROAD_HABITATION();
                            cnHabMapping.MAST_HAB_CODE = item.MAST_HAB_CODE;
                            cnHabMapping.PLAN_CN_ROAD_CODE = model.CNCode;
                            cnHabMapping.PLAN_CN_ROAD_HAB_ID = (dbContext.PLAN_ROAD_HABITATION.Any() ? dbContext.PLAN_ROAD_HABITATION.Max(m => m.PLAN_CN_ROAD_HAB_ID) + 1 : 1);
                            cnHabMapping.USERID = PMGSYSession.Current.UserId;
                            cnHabMapping.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                            dbContext.PLAN_ROAD_HABITATION.Add(cnHabMapping);
                            dbContext.SaveChanges();
                        }
                    }
                    ts.Complete();
                }
                message = "DRRP Road mapped successfully";
                return true;
            }
            catch (Exception ex)
            {
                //Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                ErrorLog.LogError(ex, "MapCandidateRoadVVP.DAL()");
                return false;
            }
            finally
            {
                dbContext.Dispose();
            }
        }


        #endregion

    }

    public interface ICoreNetworkDAL
    {
        #region CORE_NETWORKS

        Array GetCoreNetWorksList(int stateCode, int districtCode, int blockCode, string roadType, int roadCode, string roadName, int page, int rows, string sidx, string sord, out long totalRecords, int CnCode);
        bool AddCoreNetworks(CoreNetworkViewModel model, ref string message);
        bool EditCoreNetworks(CoreNetworkViewModel model, ref string message);
        CoreNetworkViewModel GetCoreNetworkDetails(int networkCode);
        bool DeleteCoreNetworks(int networkCode);
        string GetRoadCategory(int roadCode);
        Array GetHabitationList(int habCode, string flag, int page, int rows, string sidx, string sord, out long totalRecords);
        bool AddHabitation(int habitationCode, int roadCode, ref string message);
        bool DeleteMapHabitation(int habitationCode, string flag, int roadCode);
        Array GetHabitationListToMap(int roadCode, int blockCode, int erRoadCode, int page, int rows, string sidx, string sord, out long totalRecords);
        bool MapHabitationToRoad(string habCode, string roadCode);
        Array ListProposals(int roadCode, int page, int rows, string sidx, string sord, out long totalRecords);
        Array ListCandidateRoads(int roadCode, int page, int rows, string sidx, string sord, out long totalRecords, out string IsFinalized);
        bool MapCandidateRoad(CandidateRoadViewModel model, ref string message);
        bool DeleteMappedDRRPDetails(int DRRPCode, int CNCode);
        bool FinalizeMappedDRRPDetails(int CNCode);
        Array ListDRRPMappedHabitations(int DRRPCode, int CNCode, int page, int rows, string sidx, string sord, out long totalRecords);
        bool DeleteMappedDRRPHabitation(int DRRPCode, int CNCode, int habCode);

        string AddFileUploadDetailsDAL(List<PLAN_ROAD_UPLOAD_FILE> list);
        Array GetFilesListDAL(int page, int rows, string sidx, string sord, out long totalRecords, int roadCode);
        string UpdateFileDetailsDAL(PLAN_ROAD_UPLOAD_FILE model, ref string message);
        string DeleteFileDetails(PLAN_ROAD_UPLOAD_FILE model);
        string FinalizeCoreNetworkDAL(int roadCode);
        CandidateRoadViewModel GetDRRPDetails(int cnCode, int drrpCode);

        #endregion

        #region RCPLWE
        Array GetRCPLWEList(int stateCode, int districtCode, int blockCode, string roadType, int roadCode, string roadName, int page, int rows, string sidx, string sord, out long totalRecords, int CnCode);
        bool AddRCPLWE(CoreNetworkViewModel model, ref string message);
        bool EditRCPLWE(CoreNetworkViewModel model, ref string message);
        bool DeleteRCPLWE(int networkCode);

        Array GetHabitationListToMapRCPLWE(int cnRoadCode, int erRoadCode, int page, int rows, string sidx, string sord, out long totalRecords);
        Array GetHabitationListRCPLWE(int roadCode, string flag, int page, int rows, string sidx, string sord, out long totalRecords);
        bool MapHabitationToRoadRCPLWE(string encryptedHabCodes, string roadName);

        string FinalizeCoreNetworkRCPLWEDAL(int roadCode);
        #endregion

        #region PCI Entry

        Array GetCNRoadsDAL(int page, int rows, string sidx, string sord, out int totalRecords, int MAST_STATE_CODE, int MAST_DISTRICT_CODE, int ADMIN_ND_CODE, int IMS_BLOCK_ID, int IMS_YEAR);
        Array GetPmgsyRoadsDAL(int page, int rows, string sidx, string sord, out Int32 totalRecords, int MAST_STATE_CODE, int MAST_DISTRICT_CODE, int ADMIN_ND_CODE, int IMS_YEAR, int MAST_BLOCK_CODE);
        string SavePciForCNRoadDAL(PCIIndexViewModel pciIndexViewModel);
        Array GetPCIListForCNRoadDAL(int page, int rows, string sidx, string sord, out Int32 totalRecords, int PLAN_CN_ROAD_CODE, int ER_ROAD_CODE);
        bool SavePhotoGraphDAL(int PCIID, string FileName, HttpPostedFileBase filebase, string remark);
        Array GetImageFilesListDAL(int page, int rows, string sidx, string sord, out int totalRecords, int obsId);
        #endregion


        #region Village Vibrant Programme
        Array GetCoreNetWorksListVVP(int stateCode, int districtCode, int blockCode, string roadType, int roadCode, string roadName, int page, int rows, string sidx, string sord, out long totalRecords, int cnCode);

        Array GetHabitationListToMapVVP(int roadCode, int blockCode, int erRoadCode, int page, int rows, string sidx, string sord, out long totalRecords);

        bool DeleteMappedDRRPDetailsVVP(int DRRPCode, int CNCode);

        bool AddCoreNetworksVVP(CoreNetworkViewModel model, ref string message);

        bool EditCoreNetworksVVP(CoreNetworkViewModel model, ref string message);

        bool MapCandidateRoadVVP(CandidateRoadViewModel model, ref string message);

        #endregion
    }
}
