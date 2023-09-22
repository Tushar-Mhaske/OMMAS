#region HEADER
/*
 * Project Id:

 * Project Name:OMMAS-II

 * File Name: ExecutionDAL.cs

 * Author : Abhishek Kamble (Modified By: Vikram Nandanwar)

 * Creation Date :30/May/2013

 * Desc : This class is used as data access layer to perform Save,Edit,Delete and listing of Existing Roads screens.  
*/

#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using PMGSY.Common;
using PMGSY.Controllers;
using PMGSY.Extensions;
using PMGSY.Models;
using PMGSY.Models.ExistingRoads;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Diagnostics;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using System.Data.Entity.Infrastructure;
using System.Transactions;
using System.IO;
using Microsoft.VisualBasic.FileIO;
using System.Data.SqlClient;
using System.Configuration;
using System.Data.Entity.Core;


namespace PMGSY.DAL.ExistingRoads
{
    public class ExistingRoadsDAL : IExistingRoadsDAL
    {
        PMGSYEntities dbContext;
        Dictionary<string, string> decryptedParameters = null;
        string[] encryptedParameters = null;

        private int ExistingRoadCode = 0; //encryped Primary key (MAST_ER_ROAD_CODE)

        ///Changed by SAMMED A. PATIL for provision to update Road Category upwards
        public SelectList PopulateRoadCategoriesforEditDAL(int roadCategory)
        {
            PMGSYEntities dbContext = new PMGSYEntities();
            try
            {
                List<MASTER_ROAD_CATEGORY> lstRoadCategory = null;
                if (PMGSYSession.Current.RoleCode == 36 && PMGSYSession.Current.PMGSYScheme == 2)
                {
                    lstRoadCategory = dbContext.MASTER_ROAD_CATEGORY.OrderBy(m => m.MAST_ROAD_CAT_CODE).ToList<MASTER_ROAD_CATEGORY>();
                }
                else
                {
                    lstRoadCategory = dbContext.MASTER_ROAD_CATEGORY.Where(x => x.MAST_ROAD_CAT_CODE <= roadCategory && x.MAST_ROAD_CAT_CODE > 0).OrderBy(m => m.MAST_ROAD_CAT_CODE).ToList<MASTER_ROAD_CATEGORY>();
                }
                lstRoadCategory.Insert(0, new MASTER_ROAD_CATEGORY() { MAST_ROAD_CAT_CODE = 0, MAST_ROAD_CAT_NAME = "-- Select Road Category --" });
                return new SelectList(lstRoadCategory, "MAST_ROAD_CAT_CODE", "MAST_ROAD_CAT_NAME");
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "PopulateRoadCategoriesforEditDAL()");
                return null;
            }
            finally
            {
                dbContext.Dispose();
            }
        }

        public bool CheckUnlockedDAL(int blockCode)
        {
            PMGSYEntities dbContext = new PMGSYEntities();
            bool flg = false;
            try
            {
                short roleCode = dbContext.UM_User_Master.Where(m => m.UserID == PMGSYSession.Current.UserId).Select(m => m.DefaultRoleID).FirstOrDefault();

                var count = dbContext.UDF_IMS_UNLOCK_STATUS(PMGSYSession.Current.StateCode, PMGSYSession.Current.DistrictCode, blockCode, 0, 0, 0, 0, 0, "ER", PMGSYSession.Current.PMGSYScheme, roleCode).Select(c => c.UNLOCK_COUNT).FirstOrDefault();
                flg = dbContext.MASTER_PMGSY3.Any(x => x.MAST_STATE_CODE == PMGSYSession.Current.StateCode);
                if (Convert.ToInt32(count) > 0 /*&& !flg*/)
                {
                    //message = "Habitations are already mapped to this road.So please delete the habitation to change the status.";
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                //Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                ErrorLog.LogError(ex, "CheckUnlockedDAL()");
                return false;
            }
            finally
            {
                dbContext.Dispose();
            }
        }

        #region Existing Roads DAL Defination

        /// <summary>
        /// for saving the Existing Road details
        /// </summary>
        /// <param name="existingRoadsViewModel">contains the existing road details</param>
        /// <param name="message">response message</param>
        /// <returns>response message along with status</returns>
        public bool AddExistingRoads(ExistingRoadsViewModel existingRoadsViewModel, ref string message)
        {
            dbContext = new PMGSYEntities();
            try
            {

                int roadCategoryExist = dbContext.MASTER_EXISTING_ROADS.Where(m => m.MAST_STATE_CODE == existingRoadsViewModel.MAST_STATE_CODE &&
                    m.MAST_DISTRICT_CODE == existingRoadsViewModel.MAST_DISTRICT_CODE &&
                    m.MAST_BLOCK_CODE == existingRoadsViewModel.MAST_BLOCK_CODE &&
                    m.MAST_ROAD_CAT_CODE == existingRoadsViewModel.MAST_ROAD_CAT_CODE &&
                    m.MAST_ER_ROAD_NUMBER == existingRoadsViewModel.MAST_ER_SHORT_DESC + existingRoadsViewModel.MAST_ER_ROAD_NUMBER &&
                    m.MAST_ER_ROAD_STR_CHAIN == existingRoadsViewModel.MAST_ER_ROAD_STR_CHAIN &&
                    m.MAST_ER_ROAD_END_CHAIN == existingRoadsViewModel.MAST_ER_ROAD_END_CHAIN
                    && m.MAST_PMGSY_SCHEME == PMGSYSession.Current.PMGSYScheme    //Modified By Abhishek kamble 4-feb-2014
                    ).Count();

                List<MASTER_EXISTING_ROADS> lstMaster = dbContext.MASTER_EXISTING_ROADS.Where(m => m.MAST_STATE_CODE == existingRoadsViewModel.MAST_STATE_CODE && m.MAST_DISTRICT_CODE == existingRoadsViewModel.MAST_DISTRICT_CODE && m.MAST_BLOCK_CODE == existingRoadsViewModel.MAST_BLOCK_CODE && m.MAST_ROAD_CAT_CODE == existingRoadsViewModel.MAST_ROAD_CAT_CODE && m.MAST_ER_ROAD_NUMBER == existingRoadsViewModel.MAST_ER_SHORT_DESC + existingRoadsViewModel.MAST_ER_ROAD_NUMBER && m.MAST_PMGSY_SCHEME == PMGSYSession.Current.PMGSYScheme).ToList();//Modification for PMGSY II By Abhishek kamble 4-feb-2014
                foreach (var item in lstMaster)
                {
                    if (item.MAST_ER_ROAD_STR_CHAIN < existingRoadsViewModel.MAST_ER_ROAD_STR_CHAIN && existingRoadsViewModel.MAST_ER_ROAD_STR_CHAIN < item.MAST_ER_ROAD_END_CHAIN)
                    {
                        message = "Entered Start Chainage or End Chaiange is already exist for this road.";
                        return false;
                    }

                    if (item.MAST_ER_ROAD_STR_CHAIN < existingRoadsViewModel.MAST_ER_ROAD_END_CHAIN && existingRoadsViewModel.MAST_ER_ROAD_END_CHAIN < item.MAST_ER_ROAD_END_CHAIN)
                    {
                        message = "Entered Start Chainage or End Chainage is already exist for this road.";
                        return false;
                    }
                    if (existingRoadsViewModel.MAST_ER_ROAD_STR_CHAIN < item.MAST_ER_ROAD_STR_CHAIN && item.MAST_ER_ROAD_STR_CHAIN < existingRoadsViewModel.MAST_ER_ROAD_END_CHAIN)
                    {
                        message = "Entered Start Chainage or End Chaiange is already exist for this road.";
                        return false;
                    }

                    if (existingRoadsViewModel.MAST_ER_ROAD_STR_CHAIN < item.MAST_ER_ROAD_END_CHAIN && item.MAST_ER_ROAD_END_CHAIN < existingRoadsViewModel.MAST_ER_ROAD_END_CHAIN)
                    {
                        message = "Entered Start Chainage or End Chainage is already exist for this road.";
                        return false;
                    }
                }


                if (roadCategoryExist != 0)
                {
                    message = "Existing Road details for this chainage is already exist.";
                    return false;
                }

                bool flagExistingRoadsAddEdit = true;

                MASTER_EXISTING_ROADS ExistingRoadsModel = CloneExistingRoadsModel(existingRoadsViewModel, flagExistingRoadsAddEdit);
                ExistingRoadsModel.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                ExistingRoadsModel.USERID = PMGSYSession.Current.UserId;
                dbContext = new PMGSYEntities();
                dbContext.MASTER_EXISTING_ROADS.Add(ExistingRoadsModel);

                dbContext.SaveChanges();
                return true;
            }
            catch (DbUpdateException ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                message = "An Error Occurred While Your Processing Request.";
                return false;
            }
            catch (OptimisticConcurrencyException ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                message = "An Error Occurred While Your Processing Request.";
                return false;
            }

            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                message = "An Error Occurred While Your Processing Request.";
                return false;
            }
            finally
            {
                dbContext.Dispose();
            }
        }

        /// <summary>
        /// for updating the Existing Road details
        /// </summary>
        /// <param name="existingRoadsViewModel">contains the updated road details</param>
        /// <param name="message">response message along with status</param>
        /// <returns></returns>
        public bool EditExistingRoads(ExistingRoadsViewModel existingRoadsViewModel, ref string message)
        {
            try
            {
                dbContext = new PMGSYEntities();
                encryptedParameters = existingRoadsViewModel.EncryptedRoadCode.Split('/');

                if (!(encryptedParameters.Length == 3))
                {
                    return false;
                }

                decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { encryptedParameters[0], encryptedParameters[1], encryptedParameters[2] });

                ExistingRoadCode = Convert.ToInt32(decryptedParameters["RoadCode"].ToString());

                Int32 recordCount = dbContext.MASTER_EXISTING_ROADS.Where(m => m.MAST_ER_ROAD_CODE != ExistingRoadCode && m.MAST_STATE_CODE == existingRoadsViewModel.MAST_STATE_CODE &&
                   m.MAST_DISTRICT_CODE == existingRoadsViewModel.MAST_DISTRICT_CODE &&
                   m.MAST_BLOCK_CODE == existingRoadsViewModel.MAST_BLOCK_CODE &&
                   m.MAST_ROAD_CAT_CODE == existingRoadsViewModel.MAST_ROAD_CAT_CODE &&
                   m.MAST_ER_ROAD_NUMBER == existingRoadsViewModel.MAST_ER_SHORT_DESC + existingRoadsViewModel.MAST_ER_ROAD_NUMBER &&
                   m.MAST_ER_ROAD_STR_CHAIN == existingRoadsViewModel.MAST_ER_ROAD_STR_CHAIN &&
                   m.MAST_ER_ROAD_END_CHAIN == existingRoadsViewModel.MAST_ER_ROAD_END_CHAIN
                   && m.MAST_PMGSY_SCHEME == PMGSYSession.Current.PMGSYScheme //Modification for PMGSY II By Abhishek kamble 4-feb-2014
                   ).Count();

                List<MASTER_EXISTING_ROADS> lstMaster = dbContext.MASTER_EXISTING_ROADS.Where(m => m.MAST_STATE_CODE == existingRoadsViewModel.MAST_STATE_CODE && m.MAST_DISTRICT_CODE == existingRoadsViewModel.MAST_DISTRICT_CODE && m.MAST_BLOCK_CODE == existingRoadsViewModel.MAST_BLOCK_CODE && m.MAST_ROAD_CAT_CODE == existingRoadsViewModel.MAST_ROAD_CAT_CODE && m.MAST_ER_ROAD_NUMBER == existingRoadsViewModel.MAST_ER_ROAD_NUMBER && m.MAST_ER_ROAD_CODE != ExistingRoadCode && m.MAST_PMGSY_SCHEME == PMGSYSession.Current.PMGSYScheme).ToList(); //Modification for PMGSY II By Abhishek kamble 4-feb-2014
                foreach (var item in lstMaster)
                {
                    if (item.MAST_ER_ROAD_STR_CHAIN < existingRoadsViewModel.MAST_ER_ROAD_STR_CHAIN && existingRoadsViewModel.MAST_ER_ROAD_STR_CHAIN < item.MAST_ER_ROAD_END_CHAIN)
                    {
                        message = "Entered Start Chainage or End Chaiange is already exist for this road.";
                        return false;
                    }

                    if (item.MAST_ER_ROAD_STR_CHAIN < existingRoadsViewModel.MAST_ER_ROAD_END_CHAIN && existingRoadsViewModel.MAST_ER_ROAD_END_CHAIN < item.MAST_ER_ROAD_END_CHAIN)
                    {
                        message = "Entered Start Chainage or End Chainage is already exist for this road.";
                        return false;
                    }
                    if (existingRoadsViewModel.MAST_ER_ROAD_STR_CHAIN < item.MAST_ER_ROAD_STR_CHAIN && item.MAST_ER_ROAD_STR_CHAIN < existingRoadsViewModel.MAST_ER_ROAD_END_CHAIN)
                    {
                        message = "Entered Start Chainage or End Chaiange is already exist for this road.";
                        return false;
                    }

                    if (existingRoadsViewModel.MAST_ER_ROAD_STR_CHAIN < item.MAST_ER_ROAD_END_CHAIN && item.MAST_ER_ROAD_END_CHAIN < existingRoadsViewModel.MAST_ER_ROAD_END_CHAIN)
                    {
                        message = "Entered Start Chainage or End Chainage is already exist for this road.";
                        return false;
                    }
                }

                if (recordCount > 0)
                {
                    message = "Existing Road details for this chainage is already exist.";
                    return false;
                }
                bool flagExistingRoadsAddEdit = false;

                existingRoadsViewModel.MAST_ER_ROAD_CODE = ExistingRoadCode;

                MASTER_EXISTING_ROADS ExistingRoadsModel = CloneExistingRoadsModel(existingRoadsViewModel, flagExistingRoadsAddEdit);
                ExistingRoadsModel.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                ExistingRoadsModel.USERID = PMGSYSession.Current.UserId;
                dbContext.Entry(ExistingRoadsModel).State = System.Data.Entity.EntityState.Modified;
                dbContext.SaveChanges();
                return true;
            }
            catch (OptimisticConcurrencyException ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                message = "An Error Occurred While Processing Your Request.";
                return false;
            }
            catch (UpdateException ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                message = "An Error Occurred While Processing Your Request.";
                return false;
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                message = "An Error Occurred While Processing Your Request.";
                return false;
            }
            finally
            {
                dbContext.Dispose();
            }
        }

        /// <summary>
        /// for deleting the Existing Road details 
        /// </summary>
        /// <param name="_roadCode">id of Existing Road </param>
        /// <param name="message">response message</param>
        /// <returns>response message along with status</returns>
        public Boolean DeleteExistingRoads(int _roadCode, ref string message)
        {
            try
            {
                dbContext = new PMGSYEntities();
                int cnRoadCode = 0;

                using (TransactionScope ts = new TransactionScope())
                {
                    if (dbContext.IMS_UNLOCK_DETAILS.Any(m => m.MAST_ER_ROAD_CODE == _roadCode))
                    {
                        IMS_UNLOCK_DETAILS unlockDetails = dbContext.IMS_UNLOCK_DETAILS.Where(m => m.MAST_ER_ROAD_CODE == _roadCode).FirstOrDefault();
                        if (unlockDetails != null)
                        {
                            dbContext.IMS_UNLOCK_DETAILS.Remove(unlockDetails);
                            dbContext.SaveChanges();
                        }
                    }

                    MASTER_EXISTING_ROADS existingRoadsModel = dbContext.MASTER_EXISTING_ROADS.Find(_roadCode);

                    if (existingRoadsModel == null)
                    {
                        return false;
                    }
                    if (dbContext.PLAN_ROAD.Any(m => m.MAST_ER_ROAD_CODE == _roadCode))
                    {
                        message = "Road is already mapped with the core network and hence can not be deleted.";
                        return false;
                    }

                    #region
                    //if (PMGSYSession.Current.PMGSYScheme == 2)
                    //{
                    //    if (dbContext.PLAN_ROAD.Any(m => m.MAST_ER_ROAD_CODE == _roadCode))
                    //    {
                    //        message = "Road is already mapped with the core network and hence can not be deleted.";
                    //        return false;
                    //    }
                    //}
                    //else
                    //{
                    //    if (dbContext.PLAN_ROAD.Any(m => m.MAST_ER_ROAD_CODE == _roadCode))
                    //    {
                    //        cnRoadCode = dbContext.PLAN_ROAD.Where(m => m.MAST_ER_ROAD_CODE == _roadCode).Select(x => x.PLAN_CN_ROAD_CODE).FirstOrDefault();
                    //        if (cnRoadCode > 0)
                    //        {
                    //            if (dbContext.IMS_SANCTIONED_PROJECTS.Any(m => m.PLAN_CN_ROAD_CODE == cnRoadCode))
                    //            {
                    //                message = "Road is already used in Sanctioned Project hence can not be deleted.";
                    //                return false;
                    //            }
                    //        }
                    //    }
                    //}
                    #endregion

                    existingRoadsModel.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                    existingRoadsModel.USERID = PMGSYSession.Current.UserId;
                    dbContext.Entry(existingRoadsModel).State = System.Data.Entity.EntityState.Modified;
                    dbContext.SaveChanges();

                    dbContext.MASTER_EXISTING_ROADS.Remove(existingRoadsModel);
                    dbContext.SaveChanges();
                    ts.Complete();
                    return true;
                }
            }
            catch (System.Data.Entity.Infrastructure.DbUpdateException ex)
            {
                ErrorLog.LogError(ex, "DeleteExistingRoads().DbUpdateException");
                message = "Existing Roads details can not be deleted because other details for road are entered.";
                return false;
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "DeleteExistingRoads()");
                message = "An Error Occurred While Your Processing Request.";
                return false;
            }
            finally
            {
                dbContext.Dispose();
            }
        }

        /// <summary>
        /// populating the list of Existing Road details
        /// </summary>
        /// <param name="blockCode">id of block</param>
        /// <param name="categoryCode">code of Road Category</param>
        /// <param name="ownerCode">code to determine owner</param>
        /// <param name="page">no. of pages</param>
        /// <param name="rows">no. of rows</param>
        /// <param name="sidx">sort column name</param>
        /// <param name="sord">sort order</param>
        /// <param name="totalRecords">total no. of records</param>
        /// <param name="filters">filter toolbar string </param>
        /// <returns>list of existing road details satisfying the filter criteria</returns>
        public Array ListExistingRoads(int stateCode, int districtCode, int blockCode, int categoryCode, int ownerCode, int? page, int? rows, string sidx, string sord, out long totalRecords, string filters)
        {
            bool isPMGSY3 = false;
            try
            {
                JavaScriptSerializer js = null;
                PMGSY.Common.CommonFunctions.SearchJson test = new PMGSY.Common.CommonFunctions.SearchJson();
                string roadName = string.Empty;
                int erRoadCode = 0;

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
                            case "ERCode": erRoadCode = Convert.ToInt32(item.data);
                                break;
                            default:
                                break;
                        }
                    }
                }

                dbContext = new PMGSYEntities();
                #region Old Logic
                //var lstExistingRoadsDetails = (from existingRoads in dbContext.MASTER_EXISTING_ROADS
                //                               join roadCatCode in dbContext.MASTER_ROAD_CATEGORY
                //                               on existingRoads.MAST_ROAD_CAT_CODE equals roadCatCode.MAST_ROAD_CAT_CODE
                //                               join agency in dbContext.MASTER_AGENCY
                //                               on existingRoads.MAST_ER_ROAD_OWNER equals agency.MAST_AGENCY_CODE

                //                               where ((blockCode == 0 ? 1 : existingRoads.MAST_BLOCK_CODE) == (blockCode == 0 ? 1 : blockCode)) &&
                //                               ((categoryCode == 0 ? 1 : existingRoads.MAST_ROAD_CAT_CODE) == (categoryCode == 0 ? 1 : categoryCode))
                //                               &&(roadName == string.Empty ? "%" : existingRoads.MAST_ER_ROAD_NAME).StartsWith(roadName == string.Empty ? "%" : roadName)     
                //                               select new
                //                               {
                //                                   existingRoads.MAST_ER_ROAD_CODE,
                //                                   existingRoads.MAST_ER_ROAD_NUMBER,
                //                                   existingRoads.MAST_ER_ROAD_NAME,
                //                                   existingRoads.MAST_CORE_NETWORK,
                //                                   agency.MAST_AGENCY_NAME,
                //                                   existingRoads.MAST_ER_ROAD_OWNER,
                //                                   existingRoads.MAST_LOCK_STATUS,

                //                               }).ToList();
                //totalRecords = lstExistingRoadsDetails.Count();

                //if (sidx.Trim() != string.Empty)
                //{
                //    if (sord.ToString() == "asc")
                //    {
                //        switch (sidx)
                //        {
                //            case "MAST_ER_ROAD_NUMBER":
                //                lstExistingRoadsDetails = lstExistingRoadsDetails.OrderBy(x => x.MAST_ER_ROAD_NUMBER).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                //                break;

                //            case "MAST_ER_ROAD_NAME":
                //                lstExistingRoadsDetails = lstExistingRoadsDetails.OrderBy(x => x.MAST_ER_ROAD_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                //                break;
                //            case "MAST_ER_ROAD_OWNER":
                //                lstExistingRoadsDetails = lstExistingRoadsDetails.OrderBy(x => x.MAST_AGENCY_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                //                break;
                //            case "MAST_CORE_NETWORK":
                //                lstExistingRoadsDetails = lstExistingRoadsDetails.OrderBy(x => x.MAST_CORE_NETWORK).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                //                break;
                //            case "MAST_ER_ROAD_CODE":
                //                lstExistingRoadsDetails = lstExistingRoadsDetails.OrderBy(x => x.MAST_ER_ROAD_CODE).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                //                break;
                //            default:
                //                lstExistingRoadsDetails = lstExistingRoadsDetails.OrderBy(x => x.MAST_ER_ROAD_CODE).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                //                break;
                //        }
                //    }
                //    else
                //    {

                //        switch (sidx)
                //        {
                //            case "MAST_ER_ROAD_NUMBER":
                //                lstExistingRoadsDetails = lstExistingRoadsDetails.OrderByDescending(x => x.MAST_ER_ROAD_NUMBER).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                //                break;

                //            case "MAST_ER_ROAD_NAME":
                //                lstExistingRoadsDetails = lstExistingRoadsDetails.OrderByDescending(x => x.MAST_ER_ROAD_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                //                break;

                //            case "MAST_ER_ROAD_OWNER":
                //                lstExistingRoadsDetails = lstExistingRoadsDetails.OrderByDescending(x => x.MAST_AGENCY_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                //                break;
                //            case "MAST_CORE_NETWORK":
                //                lstExistingRoadsDetails = lstExistingRoadsDetails.OrderByDescending(x => x.MAST_CORE_NETWORK).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                //                break;
                //            case "MAST_ER_ROAD_CODE":
                //                lstExistingRoadsDetails = lstExistingRoadsDetails.OrderByDescending(x => x.MAST_ER_ROAD_CODE).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                //                break;
                //            default:
                //                lstExistingRoadsDetails = lstExistingRoadsDetails.OrderByDescending(x => x.MAST_ER_ROAD_CODE).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                //                break;
                //        }
                //    }
                //}
                //else
                //{
                //    lstExistingRoadsDetails = lstExistingRoadsDetails.OrderByDescending(x => x.MAST_ER_ROAD_CODE).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();

                //}

                //return lstExistingRoadsDetails.Select(item => new
                //{
                //    id = item.MAST_ER_ROAD_CODE,
                //    cell = new[]{                          

                //                  item.MAST_ER_ROAD_NUMBER,
                //                  item.MAST_ER_ROAD_NAME,
                //                  item.MAST_AGENCY_NAME,
                //                  item.MAST_CORE_NETWORK.ToString().Trim().ToUpper()=="Y"?"Yes":"No",

                //                  item.MAST_LOCK_STATUS.ToString().ToUpper()=="Y"?string.Empty:URLEncrypt.EncryptParameters1(new string[]{"RoadCode="+item.MAST_ER_ROAD_CODE.ToString().Trim()}),


                //                   item.MAST_LOCK_STATUS.ToString().ToUpper()=="Y"?string.Empty:URLEncrypt.EncryptParameters1(new string[]{"RoadCode="+item.MAST_ER_ROAD_CODE.ToString().Trim()}),   
                //                   item.MAST_LOCK_STATUS.ToString().ToUpper()=="Y"?string.Empty:URLEncrypt.EncryptParameters1(new string[]{"RoadCode="+item.MAST_ER_ROAD_CODE.ToString().Trim()}),
                //                   item.MAST_LOCK_STATUS.ToString().ToUpper()=="Y"?string.Empty:URLEncrypt.EncryptParameters1(new string[]{"RoadCode="+item.MAST_ER_ROAD_CODE.ToString().Trim()}),
                //                   item.MAST_LOCK_STATUS.ToString().ToUpper()=="Y"?string.Empty:URLEncrypt.EncryptParameters1(new string[]{"RoadCode="+item.MAST_ER_ROAD_CODE.ToString().Trim()}),    
                //                   URLEncrypt.EncryptParameters1(new string[]{"RoadCode="+item.MAST_ER_ROAD_CODE.ToString().Trim()}),
                //                   item.MAST_LOCK_STATUS.ToString().ToUpper()=="Y"?string.Empty:URLEncrypt.EncryptParameters1(new string[]{"RoadCode="+item.MAST_ER_ROAD_CODE.ToString().Trim()}),    
                //                   item.MAST_LOCK_STATUS.ToString().ToUpper()=="Y"?string.Empty:URLEncrypt.EncryptParameters1(new string[]{"RoadCode="+item.MAST_ER_ROAD_CODE.ToString().Trim()})

                //            }
                //}).ToArray();
                #endregion

                if (PMGSYSession.Current.RoleCode == 2)
                {
                    stateCode = PMGSYSession.Current.StateCode;
                }
                else if (PMGSYSession.Current.RoleCode == 22 || PMGSYSession.Current.RoleCode == 54)///Changes for RCPLWE
                {
                    stateCode = PMGSYSession.Current.StateCode;
                    districtCode = PMGSYSession.Current.DistrictCode;
                }
                ///PMGSY3
                isPMGSY3 = dbContext.MASTER_PMGSY3.Any(x => x.MAST_STATE_CODE == stateCode && x.MAST_PMGSY3_ACTIVE == "Y");

                short roleCode = dbContext.UM_User_Master.Where(m => m.UserID == PMGSYSession.Current.UserId).Select(m => m.DefaultRoleID).FirstOrDefault();
                //var lstExistingRoadsDetails = dbContext.GET_EXISTING_ROADS((stateCode <= 0 ? 0 : stateCode), (districtCode <= 0 ? 0 : districtCode), (blockCode <= 0 ? 0 : blockCode), categoryCode, roadName, PMGSYSession.Current.PMGSYScheme, roleCode).ToList();
                var lstExistingRoadsDetails = dbContext.GET_EXISTING_ROADS((stateCode <= 0 ? 0 : stateCode), (districtCode <= 0 ? 0 : districtCode), (blockCode <= 0 ? 0 : blockCode), categoryCode, roadName, (PMGSYSession.Current.PMGSYScheme == 3 ? (byte)1 : PMGSYSession.Current.PMGSYScheme), (roleCode == 54 ? (short)22 : roleCode)).ToList();///Changes for RCPLWE

                if (erRoadCode > 0)
                {
                    lstExistingRoadsDetails = lstExistingRoadsDetails.Where(x => x.MAST_ER_ROAD_CODE == erRoadCode).ToList();
                }

                totalRecords = lstExistingRoadsDetails.Count();

                if (sidx.Trim() != string.Empty)
                {
                    if (sord.ToString() == "asc")
                    {
                        switch (sidx)
                        {
                            case "MAST_ER_ROAD_CODE":
                                lstExistingRoadsDetails = lstExistingRoadsDetails.OrderBy(x => x.MAST_ER_ROAD_CODE).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                            case "MAST_ER_ROAD_NUMBER":
                                lstExistingRoadsDetails = lstExistingRoadsDetails.OrderBy(x => x.MAST_ER_ROAD_NUMBER).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                            case "MAST_ER_ROAD_NAME":
                                lstExistingRoadsDetails = lstExistingRoadsDetails.OrderBy(x => x.MAST_ER_ROAD_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                            case "MAST_ER_ROAD_OWNER":
                                lstExistingRoadsDetails = lstExistingRoadsDetails.OrderBy(x => x.AGENCY_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                            case "MAST_CORE_NETWORK":
                                lstExistingRoadsDetails = lstExistingRoadsDetails.OrderBy(x => x.MAST_CORE_NETWORK).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                            //case "MAST_ER_ROAD_CODE":
                            //    lstExistingRoadsDetails = lstExistingRoadsDetails.OrderBy(x => x.MAST_ER_ROAD_CODE).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                            //break;
                            case "MAST_ROAD_SHORT_DESC":
                                lstExistingRoadsDetails = lstExistingRoadsDetails.OrderBy(x => x.MAST_ROAD_SHORT_DESC).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                            case "MAST_ER_ROAD_TYPE":
                                lstExistingRoadsDetails = lstExistingRoadsDetails.OrderBy(x => x.MAST_ER_ROAD_TYPE).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                            default:
                                lstExistingRoadsDetails = lstExistingRoadsDetails.OrderBy(x => x.MAST_ER_ROAD_CODE).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                        }
                    }
                    else
                    {

                        switch (sidx)
                        {
                            case "MAST_ER_ROAD_CODE":
                                lstExistingRoadsDetails = lstExistingRoadsDetails.OrderByDescending(x => x.MAST_ER_ROAD_CODE).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                            case "MAST_ER_ROAD_NUMBER":
                                lstExistingRoadsDetails = lstExistingRoadsDetails.OrderByDescending(x => x.MAST_ER_ROAD_NUMBER).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                            case "MAST_ER_ROAD_NAME":
                                lstExistingRoadsDetails = lstExistingRoadsDetails.OrderByDescending(x => x.MAST_ER_ROAD_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                            case "MAST_ER_ROAD_OWNER":
                                lstExistingRoadsDetails = lstExistingRoadsDetails.OrderByDescending(x => x.AGENCY_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                            case "MAST_CORE_NETWORK":
                                lstExistingRoadsDetails = lstExistingRoadsDetails.OrderByDescending(x => x.MAST_CORE_NETWORK).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                            //case "MAST_ER_ROAD_CODE":
                            //    lstExistingRoadsDetails = lstExistingRoadsDetails.OrderByDescending(x => x.MAST_ER_ROAD_CODE).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                            //    break;
                            case "MAST_ROAD_SHORT_DESC":
                                lstExistingRoadsDetails = lstExistingRoadsDetails.OrderByDescending(x => x.MAST_ROAD_SHORT_DESC).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                            case "MAST_ER_ROAD_TYPE":
                                lstExistingRoadsDetails = lstExistingRoadsDetails.OrderByDescending(x => x.MAST_ER_ROAD_TYPE).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                            default:
                                lstExistingRoadsDetails = lstExistingRoadsDetails.OrderByDescending(x => x.MAST_ER_ROAD_CODE).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                        }
                    }
                }
                else
                {
                    lstExistingRoadsDetails = lstExistingRoadsDetails.OrderByDescending(x => x.MAST_ER_ROAD_CODE).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                }

                var result = lstExistingRoadsDetails.Select(roadDetails => new
                {
                    roadDetails.MAST_ER_ROAD_CODE,
                    roadDetails.MAST_ROAD_SHORT_DESC,
                    roadDetails.MAST_ER_ROAD_NUMBER,
                    roadDetails.MAST_ER_ROAD_NAME,
                    roadDetails.MAST_ER_ROAD_TYPE,
                    roadDetails.AGENCY_NAME,
                    roadDetails.MAST_CORE_NETWORK,
                    roadDetails.UNLOCK_BY_MORD,
                    //MAST_ER_ROAD_CODE_PMGSY1 = dbContext.MASTER_EXISTING_ROADS.Where(m => m.MAST_ER_ROAD_CODE == roadDetails.MAST_ER_ROAD_CODE).Select(m => m.MAST_ER_ROAD_CODE_PMGSY1).FirstOrDefault(),
                    roadDetails.MAST_ER_ROAD_CODE_PMGSY1,
                    roadDetails.MAST_ROAD_CAT_CODE_PMGSY2,
                    roadDetails.MAST_ROAD_CAT_CODE_PMGSY1,
                }).ToArray();

                return result.Select(item => new
                {

                    id = item.MAST_ER_ROAD_CODE,
                    cell = new[]
                    {       
                        item.MAST_ER_ROAD_CODE.ToString(),
                        item.MAST_ROAD_SHORT_DESC, //Road Category Short Desc
                        item.MAST_ER_ROAD_NUMBER,
                        item.MAST_ER_ROAD_NAME,
                         item.MAST_ER_ROAD_TYPE, //Road Type
                        (dbContext.MASTER_EXISTING_ROADS.Where(m=>m.MAST_ER_ROAD_CODE == item.MAST_ER_ROAD_CODE).Select(m=>m.MAST_ER_ROAD_END_CHAIN).FirstOrDefault() - dbContext.MASTER_EXISTING_ROADS.Where(m=>m.MAST_ER_ROAD_CODE == item.MAST_ER_ROAD_CODE).Select(m=>m.MAST_ER_ROAD_STR_CHAIN).FirstOrDefault()).ToString(),
                        item.AGENCY_NAME,
                        item.MAST_CORE_NETWORK.ToString().Trim().ToUpper()=="Y"?"Yes":"No",
                        
                        #region
                        /*dbContext.MASTER_EXISTING_ROADS.Where(m=>m.MAST_ER_ROAD_CODE == item.MAST_ER_ROAD_CODE).Select(m=>m.MAST_LOCK_STATUS).FirstOrDefault() == "N"?"<a href='#' title='Click here to add CDWorks details' class='ui-icon ui-icon-plusthick ui-align-center' onClick =CDWorks('"+URLEncrypt.EncryptParameters1(new string[]{"RoadCode =" + item.MAST_ER_ROAD_CODE.ToString().Trim()})+"'); return false;'></a>":item.UNLOCK_BY_MORD == "M"?"<a href='#' title='Click here to add CDWorks details' class='ui-icon ui-icon-unlocked ui-align-center' onClick =CDWorks('"+URLEncrypt.EncryptParameters1(new string[]{"RoadCode =" + item.MAST_ER_ROAD_CODE.ToString().Trim()})+"'); return false;'></a>":item.UNLOCK_BY_MORD.ToString().ToUpper()=="Y"?"<span class='ui-icon ui-icon-locked ui-align-center'></span>":"<a href='#' title='Click here to add CDWorks details' class='ui-icon ui-icon-plusthick ui-align-center' onClick =CDWorks('"+URLEncrypt.EncryptParameters1(new string[]{"RoadCode =" + item.MAST_ER_ROAD_CODE.ToString().Trim()})+"'); return false;'></a>",   
                        dbContext.MASTER_EXISTING_ROADS.Where(m=>m.MAST_ER_ROAD_CODE == item.MAST_ER_ROAD_CODE).Select(m=>m.MAST_LOCK_STATUS).FirstOrDefault() == "N"?"<a href='#' title='Click here to add surface type details' class='ui-icon ui-icon-plusthick ui-align-center' onClick =SurfaceTypes('"+URLEncrypt.EncryptParameters1(new string[]{"RoadCode =" + item.MAST_ER_ROAD_CODE.ToString().Trim()})+"'); return false;'></a>":item.UNLOCK_BY_MORD == "M"?"<a href='#' title='Click here to add surface type details' class='ui-icon ui-icon-unlocked ui-align-center' onClick =SurfaceTypes('"+URLEncrypt.EncryptParameters1(new string[]{"RoadCode =" + item.MAST_ER_ROAD_CODE.ToString().Trim()})+"'); return false;'></a>":item.UNLOCK_BY_MORD.ToString().ToUpper()=="Y"?"<span class='ui-icon ui-icon-locked ui-align-center'></span>":"<a href='#' title='Click here to add surface type details' class='ui-icon ui-icon-plusthick ui-align-center' onClick =SurfaceTypes('"+URLEncrypt.EncryptParameters1(new string[]{"RoadCode =" + item.MAST_ER_ROAD_CODE.ToString().Trim()})+"'); return false;'></a>",   
                        dbContext.MASTER_EXISTING_ROADS.Where(m=>m.MAST_ER_ROAD_CODE == item.MAST_ER_ROAD_CODE).Select(m=>m.MAST_LOCK_STATUS).FirstOrDefault() == "N"?"<a href='#' title='Click here to add habitation details' class='ui-icon ui-icon-plusthick ui-align-center' onClick =HabitationsMapped('"+URLEncrypt.EncryptParameters1(new string[]{"RoadCode =" + item.MAST_ER_ROAD_CODE.ToString().Trim()})+"'); return false;'></a>":item.UNLOCK_BY_MORD == "M"?"<a href='#' title='Click here to add Habitation details' class='ui-icon ui-icon-unlocked ui-align-center' onClick =HabitationsMapped('"+URLEncrypt.EncryptParameters1(new string[]{"RoadCode =" + item.MAST_ER_ROAD_CODE.ToString().Trim()})+"'); return false;'></a>":item.UNLOCK_BY_MORD.ToString().ToUpper()=="Y"?"<span class='ui-icon ui-icon-locked ui-align-center'></span>":"<a href='#' title='Click here to add habitation details' class='ui-icon ui-icon-plusthick ui-align-center' onClick =HabitationsMapped('"+URLEncrypt.EncryptParameters1(new string[]{"RoadCode =" + item.MAST_ER_ROAD_CODE.ToString().Trim()})+"'); return false;'></a>",   
                        dbContext.MASTER_EXISTING_ROADS.Where(m=>m.MAST_ER_ROAD_CODE == item.MAST_ER_ROAD_CODE).Select(m=>m.MAST_LOCK_STATUS).FirstOrDefault() == "N"?"<a href='#' title='Click here to add traffic intensity details' class='ui-icon ui-icon-plusthick ui-align-center' onClick =TrafficIntensity('"+URLEncrypt.EncryptParameters1(new string[]{"RoadCode =" + item.MAST_ER_ROAD_CODE.ToString().Trim()})+"'); return false;'></a>":item.UNLOCK_BY_MORD == "M"?"<a href='#' title='Click here to add Traffic Intensity details' class='ui-icon ui-icon-unlocked ui-align-center' onClick =TrafficIntensity('"+URLEncrypt.EncryptParameters1(new string[]{"RoadCode =" + item.MAST_ER_ROAD_CODE.ToString().Trim()})+"'); return false;'></a>":item.UNLOCK_BY_MORD.ToString().ToUpper()=="Y"?"<span class='ui-icon ui-icon-locked ui-align-center'></span>":"<a href='#' title='Click here to add traffic intensity details' class='ui-icon ui-icon-plusthick ui-align-center' onClick =TrafficIntensity('"+URLEncrypt.EncryptParameters1(new string[]{"RoadCode =" + item.MAST_ER_ROAD_CODE.ToString().Trim()})+"'); return false;'></a>",   
                        dbContext.MASTER_EXISTING_ROADS.Where(m=>m.MAST_ER_ROAD_CODE == item.MAST_ER_ROAD_CODE).Select(m=>m.MAST_LOCK_STATUS).FirstOrDefault() == "N"?"<a href='#' title='Click here to add CBR details' class='ui-icon ui-icon-plusthick ui-align-center' onClick =CBRValue('"+URLEncrypt.EncryptParameters1(new string[]{"RoadCode =" + item.MAST_ER_ROAD_CODE.ToString().Trim()})+"'); return false;'></a>":item.UNLOCK_BY_MORD == "M"?"<a href='#' title='Click here to add CBR details' class='ui-icon ui-icon-unlocked ui-align-center' onClick =CBRValue('"+URLEncrypt.EncryptParameters1(new string[]{"RoadCode =" + item.MAST_ER_ROAD_CODE.ToString().Trim()})+"'); return false;'></a>":item.UNLOCK_BY_MORD.ToString().ToUpper()=="Y"?"<span class='ui-icon ui-icon-locked ui-align-center'></span>":"<a href='#' title='Click here to add CBR details' class='ui-icon ui-icon-plusthick ui-align-center' onClick =CBRValue('"+URLEncrypt.EncryptParameters1(new string[]{"RoadCode =" + item.MAST_ER_ROAD_CODE.ToString().Trim()})+"'); return false;'></a>",   
                        "<a href='#' title='Click here to view existing roads details' class='ui-icon ui-icon-zoomin ui-align-center' onClick =ShowDetails('"+URLEncrypt.EncryptParameters1(new string[]{"RoadCode =" + item.MAST_ER_ROAD_CODE.ToString().Trim()})+"'); return false;'></a>",
                        
                        ///Changed by SAMMED A. PATIL on 15FEB2017 for Map PMGSY1 DRRP Roads
                        //(PMGSYSession.Current.PMGSYScheme == 2 && (PMGSYSession.Current.RoleCode == 22 || PMGSYSession.Current.RoleCode == 25)) 
                        //? //(dbContext.MASTER_EXISTING_ROADS.Where(m=>m.MAST_ER_ROAD_CODE == item.MAST_ER_ROAD_CODE && m.MAST_ER_ROAD_CODE_PMGSY1 != null).Any() 
                        //((item.MAST_ER_ROAD_CODE_PMGSY1 != 0 && (item.MAST_ROAD_CAT_CODE_PMGSY2 != item.MAST_ROAD_CAT_CODE_PMGSY1)) ? dbContext.MASTER_EXISTING_ROADS.Where(m=>m.MAST_ER_ROAD_CODE == item.MAST_ER_ROAD_CODE_PMGSY1).Select(m=>m.MAST_ER_ROAD_NUMBER).FirstOrDefault().ToString() : item.MAST_ER_ROAD_CODE_PMGSY1 != 0
                        //    ? "<a href='#' onClick =MapDRRPPMGSY1('"+URLEncrypt.EncryptParameters1(new string[]{"BlockCode =" + blockCode.ToString().Trim(), "RoadCode =" + item.MAST_ER_ROAD_CODE.ToString().Trim(), "RoadCategory=" + item.MAST_ROAD_CAT_CODE_PMGSY2.ToString().Trim()})+"'); return false;'>"+ dbContext.MASTER_EXISTING_ROADS.Where(m=>m.MAST_ER_ROAD_CODE == item.MAST_ER_ROAD_CODE_PMGSY1).Select(m=>m.MAST_ER_ROAD_NUMBER).FirstOrDefault().ToString() +"</a>" 
                        //    : "<a href='#' title='Click here to Map DRRP PMGSY - I Road' class='ui-icon ui-icon-plusthick ui-align-center' onClick =MapDRRPPMGSY1('"+URLEncrypt.EncryptParameters1(new string[]{"BlockCode =" + blockCode.ToString().Trim(), "RoadCode =" + item.MAST_ER_ROAD_CODE.ToString().Trim(), "RoadCategory=" + item.MAST_ROAD_CAT_CODE_PMGSY2.ToString().Trim()})+"'); return false;'></a>") 
                        //: "-",
                        
                        ((PMGSYSession.Current.PMGSYScheme == 2 && (PMGSYSession.Current.RoleCode == 22 || PMGSYSession.Current.RoleCode == 25))
                        ? (item.MAST_ER_ROAD_CODE_PMGSY1 == 0) 
                            ? "<a href='#' title='Click here to Map DRRP PMGSY - I Road' class='ui-icon ui-icon-plusthick ui-align-center' onClick =MapDRRPPMGSY1('"+URLEncrypt.EncryptParameters1(new string[]{"BlockCode =" + blockCode.ToString().Trim(), "RoadCode =" + item.MAST_ER_ROAD_CODE.ToString().Trim(), "RoadCategory=" + item.MAST_ROAD_CAT_CODE_PMGSY2.ToString().Trim()})+"'); return false;'></a>" 
                            : //(dbContext.MASTER_EXISTING_ROADS.Where(m=>m.MAST_ER_ROAD_CODE == item.MAST_ER_ROAD_CODE_PMGSY1).Select(m=>m.MAST_ER_ROAD_NUMBER).FirstOrDefault())
                              "<a href='#' onClick =MapDRRPPMGSY1('"+URLEncrypt.EncryptParameters1(new string[]{"BlockCode =" + blockCode.ToString().Trim(), "RoadCode =" + item.MAST_ER_ROAD_CODE.ToString().Trim(), "RoadCategory=" + item.MAST_ROAD_CAT_CODE_PMGSY2.ToString().Trim()})+"'); return false;'>"+ dbContext.MASTER_EXISTING_ROADS.Where(m=>m.MAST_ER_ROAD_CODE == item.MAST_ER_ROAD_CODE_PMGSY1).Select(m=>m.MAST_ER_ROAD_NUMBER).FirstOrDefault() +"</a>" 
                        : "-"),

                        //dbContext.MASTER_EXISTING_ROADS.Where(m=>m.MAST_ER_ROAD_CODE == item.MAST_ER_ROAD_CODE).Select(m=>m.MAST_LOCK_STATUS).FirstOrDefault() == "N"?"<a href='#' title='Click here to edit details' class='ui-icon ui-icon-pencil ui-align-center' onClick =EditDetails('"+URLEncrypt.EncryptParameters1(new string[]{"RoadCode =" + item.MAST_ER_ROAD_CODE.ToString().Trim()})+"'); return false;'></a>":item.UNLOCK_BY_MORD == "M"?"<a href='#' title='Click here to edit details' class='ui-icon ui-icon-unlocked ui-align-center' onClick =EditDetails('"+URLEncrypt.EncryptParameters1(new string[]{"RoadCode =" + item.MAST_ER_ROAD_CODE.ToString().Trim(),"UnlockFlag =" + item.UNLOCK_BY_MORD.ToString().Trim()})+"'); return false;'></a>":item.UNLOCK_BY_MORD.ToString().ToUpper()=="Y"?"<span class='ui-icon ui-icon-locked ui-align-center'></span>":"<a href='#' title='Click here to edit details' class='ui-icon ui-icon-pencil ui-align-center' onClick =EditDetails('"+URLEncrypt.EncryptParameters1(new string[]{"RoadCode =" + item.MAST_ER_ROAD_CODE.ToString().Trim()})+"'); return false;'></a>",   
                        ///Changes by SAMMED A. PATIL on 03 OCTOBER 2017 to edit DRRP for MORD
                        PMGSYSession.Current.RoleCode == 25 
                            ? "<a href='#' title='Click here to edit details' class='ui-icon ui-icon-pencil ui-align-center' onClick =EditDetails('"+URLEncrypt.EncryptParameters1(new string[]{"RoadCode =" + item.MAST_ER_ROAD_CODE.ToString().Trim()})+"'); return false;'></a>"
                            : (dbContext.MASTER_EXISTING_ROADS.Where(m=>m.MAST_ER_ROAD_CODE == item.MAST_ER_ROAD_CODE).Select(m=>m.MAST_LOCK_STATUS).FirstOrDefault() == "N"?"<a href='#' title='Click here to edit details' class='ui-icon ui-icon-pencil ui-align-center' onClick =EditDetails('"+URLEncrypt.EncryptParameters1(new string[]{"RoadCode =" + item.MAST_ER_ROAD_CODE.ToString().Trim()})+"'); return false;'></a>":item.UNLOCK_BY_MORD == "M"?"<a href='#' title='Click here to edit details' class='ui-icon ui-icon-unlocked ui-align-center' onClick =EditDetails('"+URLEncrypt.EncryptParameters1(new string[]{"RoadCode =" + item.MAST_ER_ROAD_CODE.ToString().Trim(),"UnlockFlag =" + item.UNLOCK_BY_MORD.ToString().Trim()})+"'); return false;'></a>":item.UNLOCK_BY_MORD.ToString().ToUpper()=="Y"?"<span class='ui-icon ui-icon-locked ui-align-center'></span>":"<a href='#' title='Click here to edit details' class='ui-icon ui-icon-pencil ui-align-center' onClick =EditDetails('"+URLEncrypt.EncryptParameters1(new string[]{"RoadCode =" + item.MAST_ER_ROAD_CODE.ToString().Trim()})+"'); return false;'></a>"),

                        PMGSYSession.Current.PMGSYScheme == 2 
                        ? 
                                (dbContext.MASTER_EXISTING_ROADS.Where(m=>m.MAST_ER_ROAD_CODE == item.MAST_ER_ROAD_CODE).Select(m=>m.MAST_LOCK_STATUS).FirstOrDefault() == "N" || item.UNLOCK_BY_MORD == "M")
                                ? ("<a href='#' title='Click here to delete details' class='ui-icon ui-icon-trash ui-align-center' onClick =DeleteDetails('"+URLEncrypt.EncryptParameters1(new string[]{"RoadCode =" + item.MAST_ER_ROAD_CODE.ToString().Trim                                   ()})+"'); return false;'></a>")
                                : "<span class='ui-icon ui-icon-locked ui-align-center'></span>"
                        : dbContext.MASTER_EXISTING_ROADS.Where(m=>m.MAST_ER_ROAD_CODE == item.MAST_ER_ROAD_CODE).Select(m=>m.MAST_LOCK_STATUS).FirstOrDefault() == "N"?"<a href='#' title='Click here to delete details' class='ui-icon ui-icon-trash ui-align-center' onClick =DeleteDetails('"+URLEncrypt.EncryptParameters1(new string[]{"RoadCode =" + item.MAST_ER_ROAD_CODE.ToString().Trim()})+"'); return false;'></a>":item.UNLOCK_BY_MORD == "M"?"<a href='#' title='Click here to delete details' class='ui-icon ui-icon-trash ui-align-center' onClick =DeleteDetails('"+URLEncrypt.EncryptParameters1(new string[]{"RoadCode =" + item.MAST_ER_ROAD_CODE.ToString().Trim()})+"'); return false;'></a>":item.UNLOCK_BY_MORD.ToString().ToUpper()=="Y"?"<span class='ui-icon ui-icon-locked ui-align-center'></span>":"<a href='#' title='Click here to delete details' class='ui-icon ui-icon-trash ui-align-center' onClick =DeleteDetails('"+URLEncrypt.EncryptParameters1(new string[]{"RoadCode =" + item.MAST_ER_ROAD_CODE.ToString().Trim()})+"'); return false;'></a>", */  
#endregion

                        //CD Works
                        (isPMGSY3 == true && PMGSYSession.Current.PMGSYScheme == 2)
                        ? "<span class='ui-icon ui-icon-locked ui-align-center'></span>"
                        : (dbContext.MASTER_EXISTING_ROADS.Where(m=>m.MAST_ER_ROAD_CODE == item.MAST_ER_ROAD_CODE).Select(m=>m.MAST_LOCK_STATUS).FirstOrDefault() == "N"?"<a href='#' title='Click here to add CDWorks details' class='ui-icon ui-icon-plusthick ui-align-center' onClick =CDWorks('"+URLEncrypt.EncryptParameters1(new string[]{"RoadCode =" + item.MAST_ER_ROAD_CODE.ToString().Trim()})+"'); return false;'></a>":item.UNLOCK_BY_MORD == "M"?"<a href='#' title='Click here to add CDWorks details' class='ui-icon ui-icon-unlocked ui-align-center' onClick =CDWorks('"+URLEncrypt.EncryptParameters1(new string[]{"RoadCode =" + item.MAST_ER_ROAD_CODE.ToString().Trim()})+"'); return false;'></a>":item.UNLOCK_BY_MORD.ToString().ToUpper()=="Y"?"<span class='ui-icon ui-icon-locked ui-align-center'></span>":"<a href='#' title='Click here to add CDWorks details' class='ui-icon ui-icon-plusthick ui-align-center' onClick =CDWorks('"+URLEncrypt.EncryptParameters1(new string[]{"RoadCode =" + item.MAST_ER_ROAD_CODE.ToString().Trim()})+"'); return false;'></a>"),   

                        //Surface Types
                        (isPMGSY3 == true && PMGSYSession.Current.PMGSYScheme == 2)
                        ? "<span class='ui-icon ui-icon-locked ui-align-center'></span>"
                        : (dbContext.MASTER_EXISTING_ROADS.Where(m=>m.MAST_ER_ROAD_CODE == item.MAST_ER_ROAD_CODE).Select(m=>m.MAST_LOCK_STATUS).FirstOrDefault() == "N"?"<a href='#' title='Click here to add surface type details' class='ui-icon ui-icon-plusthick ui-align-center' onClick =SurfaceTypes('"+URLEncrypt.EncryptParameters1(new string[]{"RoadCode =" + item.MAST_ER_ROAD_CODE.ToString().Trim()})+"'); return false;'></a>":item.UNLOCK_BY_MORD == "M"?"<a href='#' title='Click here to add surface type details' class='ui-icon ui-icon-unlocked ui-align-center' onClick =SurfaceTypes('"+URLEncrypt.EncryptParameters1(new string[]{"RoadCode =" + item.MAST_ER_ROAD_CODE.ToString().Trim()})+"'); return false;'></a>":item.UNLOCK_BY_MORD.ToString().ToUpper()=="Y"?"<span class='ui-icon ui-icon-locked ui-align-center'></span>":"<a href='#' title='Click here to add surface type details' class='ui-icon ui-icon-plusthick ui-align-center' onClick =SurfaceTypes('"+URLEncrypt.EncryptParameters1(new string[]{"RoadCode =" + item.MAST_ER_ROAD_CODE.ToString().Trim()})+"'); return false;'></a>"),   
                        
                        //Habitations
                        (isPMGSY3 == true && PMGSYSession.Current.PMGSYScheme == 2)
                        ? "<span class='ui-icon ui-icon-locked ui-align-center'></span>"
                        : (dbContext.MASTER_EXISTING_ROADS.Where(m=>m.MAST_ER_ROAD_CODE == item.MAST_ER_ROAD_CODE).Select(m=>m.MAST_LOCK_STATUS).FirstOrDefault() == "N"?"<a href='#' title='Click here to add habitation details' class='ui-icon ui-icon-plusthick ui-align-center' onClick =HabitationsMapped('"+URLEncrypt.EncryptParameters1(new string[]{"RoadCode =" + item.MAST_ER_ROAD_CODE.ToString().Trim()})+"'); return false;'></a>":item.UNLOCK_BY_MORD == "M"?"<a href='#' title='Click here to add Habitation details' class='ui-icon ui-icon-unlocked ui-align-center' onClick =HabitationsMapped('"+URLEncrypt.EncryptParameters1(new string[]{"RoadCode =" + item.MAST_ER_ROAD_CODE.ToString().Trim()})+"'); return false;'></a>":item.UNLOCK_BY_MORD.ToString().ToUpper()=="Y"?"<span class='ui-icon ui-icon-locked ui-align-center'></span>":"<a href='#' title='Click here to add habitation details' class='ui-icon ui-icon-plusthick ui-align-center' onClick =HabitationsMapped('"+URLEncrypt.EncryptParameters1(new string[]{"RoadCode =" + item.MAST_ER_ROAD_CODE.ToString().Trim()})+"'); return false;'></a>"),   

                        //Traffic
                        (isPMGSY3 == true && PMGSYSession.Current.PMGSYScheme == 2)
                        ? "<span class='ui-icon ui-icon-locked ui-align-center'></span>"
                        : (dbContext.MASTER_EXISTING_ROADS.Where(m=>m.MAST_ER_ROAD_CODE == item.MAST_ER_ROAD_CODE).Select(m=>m.MAST_LOCK_STATUS).FirstOrDefault() == "N"?"<a href='#' title='Click here to add traffic intensity details' class='ui-icon ui-icon-plusthick ui-align-center' onClick =TrafficIntensity('"+URLEncrypt.EncryptParameters1(new string[]{"RoadCode =" + item.MAST_ER_ROAD_CODE.ToString().Trim()})+"'); return false;'></a>":item.UNLOCK_BY_MORD == "M"?"<a href='#' title='Click here to add Traffic Intensity details' class='ui-icon ui-icon-unlocked ui-align-center' onClick =TrafficIntensity('"+URLEncrypt.EncryptParameters1(new string[]{"RoadCode =" + item.MAST_ER_ROAD_CODE.ToString().Trim()})+"'); return false;'></a>":item.UNLOCK_BY_MORD.ToString().ToUpper()=="Y"?"<span class='ui-icon ui-icon-locked ui-align-center'></span>":"<a href='#' title='Click here to add traffic intensity details' class='ui-icon ui-icon-plusthick ui-align-center' onClick =TrafficIntensity('"+URLEncrypt.EncryptParameters1(new string[]{"RoadCode =" + item.MAST_ER_ROAD_CODE.ToString().Trim()})+"'); return false;'></a>"),   
                        
                        //CBR
                        (isPMGSY3 == true && PMGSYSession.Current.PMGSYScheme == 2)
                        ? "<span class='ui-icon ui-icon-locked ui-align-center'></span>"
                        : (dbContext.MASTER_EXISTING_ROADS.Where(m=>m.MAST_ER_ROAD_CODE == item.MAST_ER_ROAD_CODE).Select(m=>m.MAST_LOCK_STATUS).FirstOrDefault() == "N"?"<a href='#' title='Click here to add CBR details' class='ui-icon ui-icon-plusthick ui-align-center' onClick =CBRValue('"+URLEncrypt.EncryptParameters1(new string[]{"RoadCode =" + item.MAST_ER_ROAD_CODE.ToString().Trim()})+"'); return false;'></a>":item.UNLOCK_BY_MORD == "M"?"<a href='#' title='Click here to add CBR details' class='ui-icon ui-icon-unlocked ui-align-center' onClick =CBRValue('"+URLEncrypt.EncryptParameters1(new string[]{"RoadCode =" + item.MAST_ER_ROAD_CODE.ToString().Trim()})+"'); return false;'></a>":item.UNLOCK_BY_MORD.ToString().ToUpper()=="Y"?"<span class='ui-icon ui-icon-locked ui-align-center'></span>":"<a href='#' title='Click here to add CBR details' class='ui-icon ui-icon-plusthick ui-align-center' onClick =CBRValue('"+URLEncrypt.EncryptParameters1(new string[]{"RoadCode =" + item.MAST_ER_ROAD_CODE.ToString().Trim()})+"'); return false;'></a>"),   


                        // Shift DRRP TO  new District and Block 27 Jan 2021 
                        "<a href='#' title='Click here to shift existing roads details' class='ui-icon ui-icon-unlocked ui-align-center' onClick =ShiftDRRPToNewBlockAndDistrict('"+URLEncrypt.EncryptParameters1(new string[]{"RoadCode =" + item.MAST_ER_ROAD_CODE.ToString().Trim()})+"'); return false;'></a>",


                        ///View
                        "<a href='#' title='Click here to view existing roads details' class='ui-icon ui-icon-zoomin ui-align-center' onClick =ShowDetails('"+URLEncrypt.EncryptParameters1(new string[]{"RoadCode =" + item.MAST_ER_ROAD_CODE.ToString().Trim()})+"'); return false;'></a>",
                        
                        ///Changed by SAMMED A. PATIL on 15FEB2017 for Map PMGSY1 DRRP Roads
                        //(PMGSYSession.Current.PMGSYScheme == 2 && (PMGSYSession.Current.RoleCode == 22 || PMGSYSession.Current.RoleCode == 25)) 
                        //? //(dbContext.MASTER_EXISTING_ROADS.Where(m=>m.MAST_ER_ROAD_CODE == item.MAST_ER_ROAD_CODE && m.MAST_ER_ROAD_CODE_PMGSY1 != null).Any() 
                        //((item.MAST_ER_ROAD_CODE_PMGSY1 != 0 && (item.MAST_ROAD_CAT_CODE_PMGSY2 != item.MAST_ROAD_CAT_CODE_PMGSY1)) ? dbContext.MASTER_EXISTING_ROADS.Where(m=>m.MAST_ER_ROAD_CODE == item.MAST_ER_ROAD_CODE_PMGSY1).Select(m=>m.MAST_ER_ROAD_NUMBER).FirstOrDefault().ToString() : item.MAST_ER_ROAD_CODE_PMGSY1 != 0
                        //    ? "<a href='#' onClick =MapDRRPPMGSY1('"+URLEncrypt.EncryptParameters1(new string[]{"BlockCode =" + blockCode.ToString().Trim(), "RoadCode =" + item.MAST_ER_ROAD_CODE.ToString().Trim(), "RoadCategory=" + item.MAST_ROAD_CAT_CODE_PMGSY2.ToString().Trim()})+"'); return false;'>"+ dbContext.MASTER_EXISTING_ROADS.Where(m=>m.MAST_ER_ROAD_CODE == item.MAST_ER_ROAD_CODE_PMGSY1).Select(m=>m.MAST_ER_ROAD_NUMBER).FirstOrDefault().ToString() +"</a>" 
                        //    : "<a href='#' title='Click here to Map DRRP PMGSY - I Road' class='ui-icon ui-icon-plusthick ui-align-center' onClick =MapDRRPPMGSY1('"+URLEncrypt.EncryptParameters1(new string[]{"BlockCode =" + blockCode.ToString().Trim(), "RoadCode =" + item.MAST_ER_ROAD_CODE.ToString().Trim(), "RoadCategory=" + item.MAST_ROAD_CAT_CODE_PMGSY2.ToString().Trim()})+"'); return false;'></a>") 
                        //: "-",
                        
                        //Map DRRP 
                        (isPMGSY3 == true && PMGSYSession.Current.PMGSYScheme == 2)
                        ? "<span class='ui-icon ui-icon-locked ui-align-center'></span>"
                        : (
                            ((PMGSYSession.Current.PMGSYScheme == 2 && (PMGSYSession.Current.RoleCode == 22 || PMGSYSession.Current.RoleCode == 25))
                            ? (item.MAST_ER_ROAD_CODE_PMGSY1 == 0) 
                                ? "<a href='#' title='Click here to Map DRRP PMGSY - I Road' class='ui-icon ui-icon-plusthick ui-align-center' onClick =MapDRRPPMGSY1('"+URLEncrypt.EncryptParameters1(new string[]{"BlockCode =" + blockCode.ToString().Trim(), "RoadCode =" + item.MAST_ER_ROAD_CODE.ToString().Trim(), "RoadCategory=" + item.MAST_ROAD_CAT_CODE_PMGSY2.ToString().Trim()})+"'); return false;'></a>" 
                                : //(dbContext.MASTER_EXISTING_ROADS.Where(m=>m.MAST_ER_ROAD_CODE == item.MAST_ER_ROAD_CODE_PMGSY1).Select(m=>m.MAST_ER_ROAD_NUMBER).FirstOrDefault())
                                  "<a href='#' onClick =MapDRRPPMGSY1('"+URLEncrypt.EncryptParameters1(new string[]{"BlockCode =" + blockCode.ToString().Trim(), "RoadCode =" + item.MAST_ER_ROAD_CODE.ToString().Trim(), "RoadCategory=" + item.MAST_ROAD_CAT_CODE_PMGSY2.ToString().Trim()})+"'); return false;'>"+ dbContext.MASTER_EXISTING_ROADS.Where(m=>m.MAST_ER_ROAD_CODE == item.MAST_ER_ROAD_CODE_PMGSY1).Select(m=>m.MAST_ER_ROAD_NUMBER).FirstOrDefault() +"</a>" 
                            : "-")
                           ),

                        //dbContext.MASTER_EXISTING_ROADS.Where(m=>m.MAST_ER_ROAD_CODE == item.MAST_ER_ROAD_CODE).Select(m=>m.MAST_LOCK_STATUS).FirstOrDefault() == "N"?"<a href='#' title='Click here to edit details' class='ui-icon ui-icon-pencil ui-align-center' onClick =EditDetails('"+URLEncrypt.EncryptParameters1(new string[]{"RoadCode =" + item.MAST_ER_ROAD_CODE.ToString().Trim()})+"'); return false;'></a>":item.UNLOCK_BY_MORD == "M"?"<a href='#' title='Click here to edit details' class='ui-icon ui-icon-unlocked ui-align-center' onClick =EditDetails('"+URLEncrypt.EncryptParameters1(new string[]{"RoadCode =" + item.MAST_ER_ROAD_CODE.ToString().Trim(),"UnlockFlag =" + item.UNLOCK_BY_MORD.ToString().Trim()})+"'); return false;'></a>":item.UNLOCK_BY_MORD.ToString().ToUpper()=="Y"?"<span class='ui-icon ui-icon-locked ui-align-center'></span>":"<a href='#' title='Click here to edit details' class='ui-icon ui-icon-pencil ui-align-center' onClick =EditDetails('"+URLEncrypt.EncryptParameters1(new string[]{"RoadCode =" + item.MAST_ER_ROAD_CODE.ToString().Trim()})+"'); return false;'></a>",   
                        
                        ///Changes by SAMMED A. PATIL on 03 OCTOBER 2017 to edit DRRP for MORD
                        
                        //Edit
                        (isPMGSY3 == true && PMGSYSession.Current.PMGSYScheme == 2)
                        ? "<span class='ui-icon ui-icon-locked ui-align-center'></span>"
                        : (PMGSYSession.Current.RoleCode == 25 
                            ? "<a href='#' title='Click here to edit details' class='ui-icon ui-icon-pencil ui-align-center' onClick =EditDetails('"+URLEncrypt.EncryptParameters1(new string[]{"RoadCode =" + item.MAST_ER_ROAD_CODE.ToString().Trim()})+"'); return false;'></a>"
                            : (dbContext.MASTER_EXISTING_ROADS.Where(m=>m.MAST_ER_ROAD_CODE == item.MAST_ER_ROAD_CODE).Select(m=>m.MAST_LOCK_STATUS).FirstOrDefault() == "N"?"<a href='#' title='Click here to edit details' class='ui-icon ui-icon-pencil ui-align-center' onClick =EditDetails('"+URLEncrypt.EncryptParameters1(new string[]{"RoadCode =" + item.MAST_ER_ROAD_CODE.ToString().Trim()})+"'); return false;'></a>":item.UNLOCK_BY_MORD == "M"?"<a href='#' title='Click here to edit details' class='ui-icon ui-icon-unlocked ui-align-center' onClick =EditDetails('"+URLEncrypt.EncryptParameters1(new string[]{"RoadCode =" + item.MAST_ER_ROAD_CODE.ToString().Trim(),"UnlockFlag =" + item.UNLOCK_BY_MORD.ToString().Trim()})+"'); return false;'></a>":item.UNLOCK_BY_MORD.ToString().ToUpper()=="Y"?"<span class='ui-icon ui-icon-locked ui-align-center'></span>":"<a href='#' title='Click here to edit details' class='ui-icon ui-icon-pencil ui-align-center' onClick =EditDetails('"+URLEncrypt.EncryptParameters1(new string[]{"RoadCode =" + item.MAST_ER_ROAD_CODE.ToString().Trim()})+"'); return false;'></a>")),

                        //Delete
                        (isPMGSY3 == true && PMGSYSession.Current.PMGSYScheme == 2)
                        ? "<span class='ui-icon ui-icon-locked ui-align-center'></span>"
                        : (PMGSYSession.Current.PMGSYScheme == 2 
                        ? /*(dbContext.MASTER_EXISTING_ROADS.Where(m=>m.MAST_ER_ROAD_CODE == item.MAST_ER_ROAD_CODE && m.MAST_ER_ROAD_CODE_PMGSY1 != null).Any() 
                            ? "<span class='ui-icon ui-icon-locked ui-align-center'></span>"
                            : (*/
                                (dbContext.MASTER_EXISTING_ROADS.Where(m=>m.MAST_ER_ROAD_CODE == item.MAST_ER_ROAD_CODE).Select(m=>m.MAST_LOCK_STATUS).FirstOrDefault() == "N" || item.UNLOCK_BY_MORD == "M")
                                ? ("<a href='#' title='Click here to delete details' class='ui-icon ui-icon-trash ui-align-center' onClick =DeleteDetails('"+URLEncrypt.EncryptParameters1(new string[]{"RoadCode =" + item.MAST_ER_ROAD_CODE.ToString().Trim                                   ()})+"'); return false;'></a>")
                                : "<span class='ui-icon ui-icon-locked ui-align-center'></span>"
                               /*)
                           )*/
                        : dbContext.MASTER_EXISTING_ROADS.Where(m=>m.MAST_ER_ROAD_CODE == item.MAST_ER_ROAD_CODE).Select(m=>m.MAST_LOCK_STATUS).FirstOrDefault() == "N"?"<a href='#' title='Click here to delete details' class='ui-icon ui-icon-trash ui-align-center' onClick =DeleteDetails('"+URLEncrypt.EncryptParameters1(new string[]{"RoadCode =" + item.MAST_ER_ROAD_CODE.ToString().Trim()})+"'); return false;'></a>":item.UNLOCK_BY_MORD == "M"?"<a href='#' title='Click here to delete details' class='ui-icon ui-icon-trash ui-align-center' onClick =DeleteDetails('"+URLEncrypt.EncryptParameters1(new string[]{"RoadCode =" + item.MAST_ER_ROAD_CODE.ToString().Trim()})+"'); return false;'></a>":item.UNLOCK_BY_MORD.ToString().ToUpper()=="Y"?"<span class='ui-icon ui-icon-locked ui-align-center'></span>":"<a href='#' title='Click here to delete details' class='ui-icon ui-icon-trash ui-align-center' onClick =DeleteDetails('"+URLEncrypt.EncryptParameters1(new string[]{"RoadCode =" + item.MAST_ER_ROAD_CODE.ToString().Trim()})+"'); return false;'></a>"),  
                    }
                }).ToArray();
            }
            catch (Exception ex)
            {
                //Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                ErrorLog.LogError(ex, "ListExistingRoads().DAL");
                totalRecords = 0;
                return null;
            }
            finally
            {
                dbContext.Dispose();
            }
        }

        /// <summary>
        /// details of the existing road 
        /// </summary>
        /// <param name="_roadCode">id of existing road</param>
        /// <returns>model containing the existing road details</returns>
        public ExistingRoadsViewModel GetExistingRoads_ByRoadCode(int _roadCode)
        {
            try
            {
                dbContext = new PMGSYEntities();
                MASTER_EXISTING_ROADS existingRoadsModel = dbContext.MASTER_EXISTING_ROADS.FirstOrDefault(m => m.MAST_ER_ROAD_CODE == _roadCode);

                MASTER_BLOCK blockModel = dbContext.MASTER_BLOCK.Where(m => m.MAST_BLOCK_CODE == existingRoadsModel.MAST_BLOCK_CODE).FirstOrDefault();

                ExistingRoadsViewModel existingRoadsViewModel = null;
                if (existingRoadsModel != null)
                {
                    existingRoadsViewModel = CloneExistingRoadsObject(existingRoadsModel);
                }

                existingRoadsViewModel.BlockName = blockModel.MAST_BLOCK_NAME;

                return existingRoadsViewModel;
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                return null;
            }
            finally { dbContext.Dispose(); }
        }

        //Added By Abhishek kamble 7-feb-2014

        public bool IsExistingRoadIsMappedWithCN_CR(ExistingRoadsViewModel model, int ErRoadCode, out decimal? CN_CRRoadLength, out decimal TotalERRoadLength)
        {
            dbContext = new PMGSYEntities();
            try
            {

                //MASTER_EXISTING_ROADS existingRoadModel=dbContext.

                if (PMGSYSession.Current.PMGSYScheme == 1)
                {
                    var PlanRoadDetails = dbContext.PLAN_ROAD.Where(m => m.MAST_PMGSY_SCHEME == 1 && m.MAST_ER_ROAD_CODE == ErRoadCode && m.MAST_STATE_CODE == model.MAST_STATE_CODE && m.MAST_DISTRICT_CODE == model.MAST_DISTRICT_CODE && m.MAST_BLOCK_CODE == model.MAST_BLOCK_CODE).ToList();

                    foreach (var planRoad in PlanRoadDetails)
                    {
                        decimal TatalRoadLenght = model.MAST_ER_ROAD_END_CHAIN - model.MAST_ER_ROAD_STR_CHAIN;
                        if (TatalRoadLenght < planRoad.PLAN_RD_LENGTH)
                        {
                            CN_CRRoadLength = planRoad.PLAN_RD_LENGTH;
                            TotalERRoadLength = TatalRoadLenght;
                            return false;
                        }
                    }
                }
                else if (PMGSYSession.Current.PMGSYScheme == 2)
                {
                    //checked in PLAN_ROAD 
                    var PlanRoadDetails = dbContext.PLAN_ROAD.Where(m => m.MAST_PMGSY_SCHEME == 2 && m.MAST_ER_ROAD_CODE == ErRoadCode && m.MAST_STATE_CODE == model.MAST_STATE_CODE && m.MAST_DISTRICT_CODE == model.MAST_DISTRICT_CODE && m.MAST_BLOCK_CODE == model.MAST_BLOCK_CODE).ToList();

                    decimal? PlanRoadLength = PlanRoadDetails.Max(m => (decimal?)m.PLAN_RD_LENGTH);
                    if (PlanRoadLength != null)
                    {
                        decimal TatalRoadLenght = model.MAST_ER_ROAD_END_CHAIN - model.MAST_ER_ROAD_STR_CHAIN;
                        if (TatalRoadLenght < PlanRoadLength)
                        {
                            CN_CRRoadLength = PlanRoadLength;
                            TotalERRoadLength = TatalRoadLenght;
                            return false;
                        }
                    }

                    //foreach (var planRoad in PlanRoadDetails)
                    //{
                    //    decimal TatalRoadLenght = model.MAST_ER_ROAD_END_CHAIN - model.MAST_ER_ROAD_STR_CHAIN;
                    //    if (TatalRoadLenght < planRoad.PLAN_RD_LENGTH)
                    //    {
                    //        CN_CRRoadLength = planRoad.PLAN_RD_LENGTH;
                    //        TotalERRoadLength = TatalRoadLenght;
                    //        return false;
                    //    }
                    //}

                    //Checked in PLAN_ROAD_DRRP

                    var PlanRoadDrrpDetails = dbContext.PLAN_ROAD_DRRP.Where(m => m.MAST_ER_ROAD_CODE == ErRoadCode).ToList();

                    decimal? PlanRoadDrrpLength = PlanRoadDrrpDetails.Max(m => (decimal?)m.PLAN_RD_LENGTH);
                    if (PlanRoadDrrpLength != null)
                    {
                        decimal TatalRoadLenght = model.MAST_ER_ROAD_END_CHAIN - model.MAST_ER_ROAD_STR_CHAIN;
                        if (TatalRoadLenght < PlanRoadDrrpLength)
                        {
                            CN_CRRoadLength = PlanRoadDrrpLength;
                            TotalERRoadLength = TatalRoadLenght;
                            return false;
                        }
                    }

                    //foreach (var PlanRoadDrrp in PlanRoadDrrpDetails)
                    //{
                    //    decimal TatalRoadLenght = model.MAST_ER_ROAD_END_CHAIN - model.MAST_ER_ROAD_STR_CHAIN;
                    //    if (TatalRoadLenght < PlanRoadDrrp.PLAN_RD_LENGTH)
                    //    {
                    //        CN_CRRoadLength = PlanRoadDrrp.PLAN_RD_LENGTH;
                    //        TotalERRoadLength = TatalRoadLenght;
                    //        return false;
                    //    }
                    //}
                }
                CN_CRRoadLength = 0;
                TotalERRoadLength = 0;
                return true;
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                CN_CRRoadLength = 0;
                TotalERRoadLength = 0;
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
        /// view details of Existing Road
        /// </summary>
        /// <param name="_roadCode">id of Existing Road</param>
        /// <returns></returns>
        public ExistingRoadsViewModel GetExistingRoads_ForViewDetails(int _roadCode)
        {
            try
            {
                dbContext = new PMGSYEntities();
                MASTER_EXISTING_ROADS existingRoadsModel = dbContext.MASTER_EXISTING_ROADS.FirstOrDefault(m => m.MAST_ER_ROAD_CODE == _roadCode);

                ExistingRoadsViewModel existingRoadsViewModel = new ExistingRoadsViewModel();

                if (existingRoadsModel != null)
                {
                    existingRoadsViewModel.MAST_ER_ROAD_NUMBER = existingRoadsModel.MAST_ER_ROAD_NUMBER;
                    existingRoadsViewModel.MAST_ROAD_CAT_CODE = existingRoadsModel.MAST_ROAD_CAT_CODE;
                    existingRoadsViewModel.MAST_ER_ROAD_NAME = existingRoadsModel.MAST_ER_ROAD_NAME;
                    existingRoadsViewModel.MAST_ER_ROAD_STR_CHAIN = existingRoadsModel.MAST_ER_ROAD_STR_CHAIN;
                    existingRoadsViewModel.MAST_ER_ROAD_END_CHAIN = existingRoadsModel.MAST_ER_ROAD_END_CHAIN;
                    existingRoadsViewModel.MAST_ER_ROAD_C_WIDTH = existingRoadsModel.MAST_ER_ROAD_C_WIDTH;
                    existingRoadsViewModel.MAST_ER_ROAD_F_WIDTH = existingRoadsModel.MAST_ER_ROAD_F_WIDTH;
                    existingRoadsViewModel.MAST_ER_ROAD_L_WIDTH = existingRoadsModel.MAST_ER_ROAD_L_WIDTH;

                    if ((existingRoadsModel.MAST_ER_ROAD_TYPE.ToUpper()) == "A")
                    {
                        existingRoadsViewModel.MAST_ER_ROAD_TYPE = "All Weather";
                    }
                    else
                    {
                        existingRoadsViewModel.MAST_ER_ROAD_TYPE = "Fair Weather";
                    }

                    existingRoadsViewModel.MAST_SOIL_TYPE_CODE = existingRoadsModel.MAST_SOIL_TYPE_CODE;
                    existingRoadsViewModel.MAST_TERRAIN_TYPE_CODE = existingRoadsModel.MAST_TERRAIN_TYPE_CODE;

                    if ((existingRoadsModel.MAST_CORE_NETWORK.ToUpper()) == "Y")
                    {
                        existingRoadsViewModel.MAST_CORE_NETWORK = "Yes";
                    }
                    else
                    {
                        existingRoadsViewModel.MAST_CORE_NETWORK = "No";
                    }

                    existingRoadsViewModel.MAST_ER_ROAD_CODE = existingRoadsModel.MAST_ER_ROAD_CODE;
                    existingRoadsViewModel.MAST_ER_ROAD_OWNER = existingRoadsModel.MAST_ER_ROAD_OWNER;
                    existingRoadsViewModel.MAST_CONS_YEAR = existingRoadsModel.MAST_CONS_YEAR;
                    existingRoadsViewModel.MAST_RENEW_YEAR = existingRoadsModel.MAST_RENEW_YEAR;

                    existingRoadsViewModel.CategoryOfRoadName = existingRoadsModel.MASTER_ROAD_CATEGORY == null ? "NA" : existingRoadsModel.MASTER_ROAD_CATEGORY.MAST_ROAD_CAT_NAME;

                    existingRoadsViewModel.OwnerOfRoadName = existingRoadsModel.MASTER_AGENCY == null ? "NA" : existingRoadsModel.MASTER_AGENCY.MAST_AGENCY_NAME;

                    existingRoadsViewModel.SoilTypeName = existingRoadsModel.MASTER_SOIL_TYPE == null ? "NA" : existingRoadsModel.MASTER_SOIL_TYPE.MAST_SOIL_TYPE_NAME;

                    existingRoadsViewModel.TerrainTypeName = existingRoadsModel.MASTER_TERRAIN_TYPE == null ? "NA" : existingRoadsModel.MASTER_TERRAIN_TYPE.MAST_TERRAIN_TYPE_NAME;

                    existingRoadsViewModel.MAST_LOCK_STATUS = existingRoadsModel.MAST_LOCK_STATUS;
                    existingRoadsViewModel.MAST_IS_BENEFITTED_HAB = existingRoadsModel.MAST_IS_BENEFITTED_HAB == "Y" ? "Yes" : "No";
                }
                return existingRoadsViewModel;
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
        /// populates the Surface details list
        /// </summary>
        /// <param name="page">no of pages</param>
        /// <param name="rows">no of rows</param>
        /// <param name="sidx">sort column name</param>
        /// <param name="sord">sort order</param>
        /// <param name="totalRecords">total no. of records</param>
        /// <param name="roadCode">id of Existing Road</param>
        /// <returns>list of Surface details of existing roads</returns>
        public Array GetSurfaceList(int? page, int? rows, string sidx, string sord, out Int32 totalRecords, int roadCode)
        {
            try
            {
                dbContext = new PMGSYEntities();

                var lstSurfaceDetails = (from surfaceTypes in dbContext.MASTER_ER_SURFACE_TYPES
                                         join existingRoads in dbContext.MASTER_EXISTING_ROADS
                                       on surfaceTypes.MAST_ER_ROAD_CODE equals existingRoads.MAST_ER_ROAD_CODE
                                         join surface in dbContext.MASTER_SURFACE
                                         on surfaceTypes.MAST_SURFACE_CODE equals surface.MAST_SURFACE_CODE
                                         where existingRoads.MAST_ER_ROAD_CODE == roadCode

                                         select new
                                         {
                                             surfaceTypes.MAST_ER_ROAD_CODE,
                                             surfaceTypes.MAST_SURFACE_SEG_NO,
                                             surface.MAST_SURFACE_NAME,
                                             surfaceTypes.MAST_ER_STR_CHAIN,
                                             surfaceTypes.MAST_ER_END_CHAIN,
                                             surfaceTypes.MAST_ER_SURFACE_CONDITION,
                                             surfaceTypes.MAST_ER_SURFACE_LENGTH,
                                         }).ToList();


                totalRecords = lstSurfaceDetails.Count();
                int recCount = totalRecords;

                if (sidx.Trim() != string.Empty)
                {
                    if (sord.ToString() == "asc")
                    {
                        switch (sidx)
                        {
                            case "SurfaceName":
                                lstSurfaceDetails = lstSurfaceDetails.OrderBy(x => x.MAST_SURFACE_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                            case "StartChainage":
                                lstSurfaceDetails = lstSurfaceDetails.OrderBy(x => x.MAST_ER_STR_CHAIN).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                            case "EndChainage":
                                lstSurfaceDetails = lstSurfaceDetails.OrderBy(x => x.MAST_ER_END_CHAIN).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                            case "SurfaceCondition":
                                lstSurfaceDetails = lstSurfaceDetails.OrderBy(x => x.MAST_ER_SURFACE_CONDITION).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                            case "SurfaceLength":
                                lstSurfaceDetails = lstSurfaceDetails.OrderBy(x => x.MAST_ER_SURFACE_LENGTH).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                        }
                    }
                    else
                    {
                        switch (sidx)
                        {
                            case "SurfaceName":
                                lstSurfaceDetails = lstSurfaceDetails.OrderByDescending(x => x.MAST_SURFACE_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                            case "StartChainage":
                                lstSurfaceDetails = lstSurfaceDetails.OrderByDescending(x => x.MAST_ER_STR_CHAIN).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                            case "EndChainage":
                                lstSurfaceDetails = lstSurfaceDetails.OrderByDescending(x => x.MAST_ER_END_CHAIN).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                            case "SurfaceCondition":
                                lstSurfaceDetails = lstSurfaceDetails.OrderByDescending(x => x.MAST_ER_SURFACE_CONDITION).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                            case "SurfaceLength":
                                lstSurfaceDetails = lstSurfaceDetails.OrderByDescending(x => x.MAST_ER_SURFACE_LENGTH).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                        }
                    }
                }
                else
                {
                    lstSurfaceDetails = lstSurfaceDetails.OrderBy(x => x.MAST_ER_END_CHAIN).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                }


                return lstSurfaceDetails.Select(surfaceDetails => new
                {
                    cell = new[] {         
                                    surfaceDetails.MAST_SURFACE_NAME,
                                    surfaceDetails.MAST_ER_STR_CHAIN==null?"-": surfaceDetails.MAST_ER_STR_CHAIN.ToString(),
                                    surfaceDetails.MAST_ER_END_CHAIN==null?"-":surfaceDetails.MAST_ER_END_CHAIN.ToString(),
                                    surfaceDetails.MAST_ER_SURFACE_CONDITION.ToUpper().Trim() == "G" ? "Good" : surfaceDetails.MAST_ER_SURFACE_CONDITION.ToUpper().Trim() == "F" ? "Fair" : "Bad",
                                    surfaceDetails.MAST_ER_SURFACE_LENGTH.ToString(),

                                    URLEncrypt.EncryptParameters1(new string[]{"MAST_ER_ROAD_CODE="+surfaceDetails.MAST_ER_ROAD_CODE.ToString().Trim(),"MAST_SURFACE_SEG_NO="+surfaceDetails.MAST_SURFACE_SEG_NO.ToString().Trim()}),

                                    recCount==1?URLEncrypt.EncryptParameters1(new string[]{"MAST_ER_ROAD_CODE="+surfaceDetails.MAST_ER_ROAD_CODE.ToString().Trim(),"MAST_SURFACE_SEG_NO="+surfaceDetails.MAST_SURFACE_SEG_NO.ToString().Trim()}):recCount==surfaceDetails.MAST_SURFACE_SEG_NO?URLEncrypt.EncryptParameters1(new string[]{"MAST_ER_ROAD_CODE="+surfaceDetails.MAST_ER_ROAD_CODE.ToString().Trim(),"MAST_SURFACE_SEG_NO="+surfaceDetails.MAST_SURFACE_SEG_NO.ToString().Trim()}):String.Empty,

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
        /// populates the CBR Details list
        /// </summary>
        /// <param name="page">no of pages</param>
        /// <param name="rows">no of rows</param>
        /// <param name="sidx">sort column name</param>
        /// <param name="sord">sort order</param>
        /// <param name="totalRecords">total no. of records</param>
        /// <param name="roadCode">id of Existing Road</param>
        /// <returns>list of CBR details of existing roads</returns>
        public Array GetCBRListDAL(int? page, int? rows, string sidx, string sord, out Int32 totalRecords, int roadCode)
        {
            try
            {
                dbContext = new PMGSYEntities();
                List<MASTER_ER_CBR_VALUE> listCBR = (from CrbValue in dbContext.MASTER_ER_CBR_VALUE
                                                     where
                                                    CrbValue.MAST_ER_ROAD_CODE == roadCode
                                                     select CrbValue
                                                             ).OrderBy(c => c.MAST_SEGMENT_NO).ToList<MASTER_ER_CBR_VALUE>();


                IQueryable<MASTER_ER_CBR_VALUE> query = listCBR.AsQueryable<MASTER_ER_CBR_VALUE>();
                totalRecords = listCBR.Count();
                int recCount = totalRecords;

                if (sidx.Trim() != string.Empty)
                {
                    if (sord.ToString() == "asc")
                    {
                        switch (sidx)
                        {
                            case "StartChainage":
                                query = query.OrderBy(x => x.MAST_STR_CHAIN).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                                break;
                            case "EndChainage":
                                query = query.OrderBy(x => x.MAST_END_CHAIN).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                                break;
                            case "CBRValue":
                                query = query.OrderBy(x => x.MAST_CBR_VALUE).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                                break;

                        }

                    }
                    else
                    {
                        switch (sidx)
                        {
                            case "StartChainage":
                                query = query.OrderByDescending(x => x.MAST_STR_CHAIN).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                                break;
                            case "EndChainage":
                                query = query.OrderByDescending(x => x.MAST_END_CHAIN).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                                break;
                            case "CBRValue":
                                query = query.OrderByDescending(x => x.MAST_CBR_VALUE).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                                break;

                        }
                    }
                }
                else
                {
                    query = query.OrderBy(x => x.MAST_SEGMENT_NO).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                }

                return query.Select(CBRDetails => new
                {

                    cell = new[] {                                      

                                    CBRDetails.MAST_STR_CHAIN.ToString(),
                                    CBRDetails.MAST_END_CHAIN.ToString(),
                                    (Convert.ToDecimal(CBRDetails.MAST_END_CHAIN - CBRDetails.MAST_STR_CHAIN)).ToString(),
                                    CBRDetails.MAST_CBR_VALUE.ToString(),

                                    URLEncrypt.EncryptParameters1(new string[]{"MAST_ER_ROAD_CODE="+CBRDetails.MAST_ER_ROAD_CODE.ToString().Trim(),"MAST_SEGMENT_NO="+CBRDetails.MAST_SEGMENT_NO.ToString().Trim()}),

                                    recCount==1?URLEncrypt.EncryptParameters1(new string[]{"MAST_ER_ROAD_CODE="+CBRDetails.MAST_ER_ROAD_CODE.ToString().Trim(),"MAST_SEGMENT_NO="+CBRDetails.MAST_SEGMENT_NO.ToString().Trim()}):recCount==CBRDetails.MAST_SEGMENT_NO?URLEncrypt.EncryptParameters1(new string[]{"MAST_ER_ROAD_CODE="+CBRDetails.MAST_ER_ROAD_CODE.ToString().Trim(),"MAST_SEGMENT_NO="+CBRDetails.MAST_SEGMENT_NO.ToString().Trim()}):String.Empty,

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
        /// populates the Traffic Details list
        /// </summary>
        /// <param name="page">no of pages</param>
        /// <param name="rows">no of rows</param>
        /// <param name="sidx">sort column name</param>
        /// <param name="sord">sort order</param>
        /// <param name="totalRecords">total no. of records</param>
        /// <param name="roadCode">id of Existing Road</param>
        /// <returns>list of Traffic details of existing roads</returns>
        public Array GetTrafficListDAL(int? page, int? rows, string sidx, string sord, out Int32 totalRecords, int roadCode)
        {
            try
            {
                dbContext = new PMGSYEntities();
                List<MASTER_ER_TRAFFIC_INTENSITY> listTrafficIntensity = (from c in dbContext.MASTER_ER_TRAFFIC_INTENSITY
                                                                          where
                                                                          c.MAST_ER_ROAD_CODE == roadCode
                                                                          select c
                                                             ).OrderBy(c => c.MAST_TI_YEAR).ToList<MASTER_ER_TRAFFIC_INTENSITY>();

                IQueryable<MASTER_ER_TRAFFIC_INTENSITY> query = listTrafficIntensity.AsQueryable<MASTER_ER_TRAFFIC_INTENSITY>();

                totalRecords = listTrafficIntensity.Count();

                if (sidx.Trim() != string.Empty)
                {
                    if (sord.ToString() == "asc")
                    {
                        switch (sidx)
                        {

                            case "Year":
                                query = query.OrderBy(x => x.MAST_TI_YEAR).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                                break;
                            case "TotalMotarisedTrafficday":
                                query = query.OrderBy(x => x.MAST_TOTAL_TI).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                                break;
                            case "CommercialVehicleTrafficDay":
                                query = query.OrderBy(x => x.MAST_COMM_TI).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                                break;
                        }

                    }
                    else
                    {
                        switch (sidx)
                        {

                            case "Year":
                                query = query.OrderByDescending(x => x.MAST_TI_YEAR).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                                break;
                            case "TotalMotarisedTrafficday":
                                query = query.OrderByDescending(x => x.MAST_TOTAL_TI).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                                break;
                            case "CommercialVehicleTrafficDay":
                                query = query.OrderByDescending(x => x.MAST_COMM_TI).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                                break;
                        }

                    }
                }
                else
                {
                    query = query.OrderBy(x => x.MAST_TI_YEAR).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                }


                return query.Select(trafficDetails => new
                {
                    ID = trafficDetails.MAST_ER_ROAD_CODE,
                    cell = new[] {       
                                    trafficDetails.MAST_TI_YEAR.ToString()+"-"+(trafficDetails.MAST_TI_YEAR+1),
                                    trafficDetails.MAST_TOTAL_TI.ToString(),
                                    trafficDetails.MAST_COMM_TI.ToString (),
                                    URLEncrypt.EncryptParameters1(new string[]{"MAST_ER_ROAD_CODE="+trafficDetails.MAST_ER_ROAD_CODE.ToString().Trim(),"MAST_IT_YEAR="+trafficDetails.MAST_TI_YEAR.ToString().Trim()}),

                                    URLEncrypt.EncryptParameters1(new string[]{"MAST_ER_ROAD_CODE="+trafficDetails.MAST_ER_ROAD_CODE.ToString().Trim(),"MAST_IT_YEAR="+trafficDetails.MAST_TI_YEAR.ToString().Trim()})
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
        /// populates the CDWorks list
        /// </summary>
        /// <param name="page">no of pages</param>
        /// <param name="rows">no of rows</param>
        /// <param name="sidx">sort column name</param>
        /// <param name="sord">sort order</param>
        /// <param name="totalRecords">total no. of records</param>
        /// <param name="roadCode">id of Existing Road</param>
        /// <returns>list of CDWorks details of existing roads</returns>
        public Array GetCdWorkList(int? page, int? rows, string sidx, string sord, out Int32 totalRecords, int roadCode)
        {
            try
            {
                dbContext = new PMGSYEntities();
                var cdWorksDetails = (from cdWorks in dbContext.MASTER_ER_CDWORKS_ROAD
                                      join cdWorkTypeConstruction in dbContext.MASTER_CDWORKS_TYPE_CONSTRUCTION
                                     on cdWorks.MAST_CDWORKS_CODE equals cdWorkTypeConstruction.MAST_CDWORKS_CODE
                                      where cdWorks.MAST_ER_ROAD_CODE == roadCode
                                      select new
                                      {
                                          cdWorks.MAST_ER_ROAD_CODE,
                                          cdWorks.MAST_CD_NUMBER,
                                          cdWorkTypeConstruction.MAST_CDWORKS_NAME,
                                          cdWorks.MAST_CD_LENGTH,
                                          cdWorks.MAST_CD_DISCHARGE,
                                          cdWorks.MAST_CD_CHAINAGE,
                                          cdWorks.MAST_CONSTRUCT_YEAR,
                                          cdWorks.MAST_REHAB_YEAR,
                                          cdWorks.MAST_ER_SPAN,
                                          cdWorks.MAST_CARRIAGE_WAY,
                                          cdWorks.MAST_IS_FOOTPATH,
                                      }
                          ).ToList();

                totalRecords = cdWorksDetails.Count();

                if (sidx.Trim() != string.Empty)
                {
                    if (sord.ToString() == "asc")
                    {
                        switch (sidx)
                        {
                            case "CDWorksNumber":
                                cdWorksDetails = cdWorksDetails.OrderBy(x => x.MAST_CD_NUMBER).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                            case "CDWorksType":
                                cdWorksDetails = cdWorksDetails.OrderBy(x => x.MAST_CDWORKS_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                            case "CDWorksLength":
                                cdWorksDetails = cdWorksDetails.OrderBy(x => x.MAST_CD_LENGTH).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                            case "CDWorksDischarge":
                                cdWorksDetails = cdWorksDetails.OrderBy(x => x.MAST_CD_DISCHARGE).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;

                            case "CDWorksChainage":
                                cdWorksDetails = cdWorksDetails.OrderBy(x => x.MAST_CD_CHAINAGE).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;


                            case "ConstructionYear":
                                cdWorksDetails = cdWorksDetails.OrderBy(x => x.MAST_CONSTRUCT_YEAR).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;

                            case "RehabilitationYear":
                                cdWorksDetails = cdWorksDetails.OrderBy(x => x.MAST_REHAB_YEAR).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;

                            case "Span":
                                cdWorksDetails = cdWorksDetails.OrderBy(x => x.MAST_ER_SPAN).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;

                            case "CarriageWay":
                                cdWorksDetails = cdWorksDetails.OrderBy(x => x.MAST_CARRIAGE_WAY).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                            case "FootPath":
                                cdWorksDetails = cdWorksDetails.OrderBy(x => x.MAST_IS_FOOTPATH).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;

                        }
                    }
                    else
                    {
                        switch (sidx)
                        {
                            case "CDWorksNumber":
                                cdWorksDetails = cdWorksDetails.OrderByDescending(x => x.MAST_CD_NUMBER).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                            case "CDWorksType":
                                cdWorksDetails = cdWorksDetails.OrderByDescending(x => x.MAST_CDWORKS_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                            case "CDWorksLength":
                                cdWorksDetails = cdWorksDetails.OrderByDescending(x => x.MAST_CD_LENGTH).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                            case "CDWorksDischarge":
                                cdWorksDetails = cdWorksDetails.OrderByDescending(x => x.MAST_CD_DISCHARGE).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;

                            case "CDWorksChainage":
                                cdWorksDetails = cdWorksDetails.OrderByDescending(x => x.MAST_CD_CHAINAGE).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;

                            case "ConstructionYear":
                                cdWorksDetails = cdWorksDetails.OrderByDescending(x => x.MAST_CONSTRUCT_YEAR).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;

                            case "RehabilitationYear":
                                cdWorksDetails = cdWorksDetails.OrderByDescending(x => x.MAST_REHAB_YEAR).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;

                            case "Span":
                                cdWorksDetails = cdWorksDetails.OrderByDescending(x => x.MAST_ER_SPAN).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;

                            case "CarriageWay":
                                cdWorksDetails = cdWorksDetails.OrderByDescending(x => x.MAST_CARRIAGE_WAY).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                            case "FootPath":
                                cdWorksDetails = cdWorksDetails.OrderByDescending(x => x.MAST_IS_FOOTPATH).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;

                        }
                    }
                }
                else
                {
                    cdWorksDetails = cdWorksDetails.OrderBy(x => x.MAST_CD_NUMBER).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                }


                return cdWorksDetails.Select(cdWorksRoadDetails => new
                {
                    cell = new[] {         
                        cdWorksRoadDetails.MAST_CDWORKS_NAME,
                        cdWorksRoadDetails.MAST_CD_LENGTH==null?"-":cdWorksRoadDetails.MAST_CD_LENGTH.ToString(),
                        cdWorksRoadDetails.MAST_CD_DISCHARGE==null?"-":cdWorksRoadDetails.MAST_CD_DISCHARGE.ToString(),
                        cdWorksRoadDetails.MAST_CD_CHAINAGE==null?"-":cdWorksRoadDetails.MAST_CD_CHAINAGE.ToString(),
                        cdWorksRoadDetails.MAST_CONSTRUCT_YEAR==null?"-":cdWorksRoadDetails.MAST_CONSTRUCT_YEAR.ToString(),
                        cdWorksRoadDetails.MAST_REHAB_YEAR==null?"-":cdWorksRoadDetails.MAST_REHAB_YEAR.ToString(),
                        cdWorksRoadDetails.MAST_ER_SPAN==null?"-":cdWorksRoadDetails.MAST_ER_SPAN.ToString(),
                        cdWorksRoadDetails.MAST_CARRIAGE_WAY==null?"-":cdWorksRoadDetails.MAST_CARRIAGE_WAY.ToString(),
                        cdWorksRoadDetails.MAST_IS_FOOTPATH.ToString().ToUpper().Trim()=="Y"?"Yes":"No",

                                    URLEncrypt.EncryptParameters1(new string[]{"MAST_ER_ROAD_CODE="+cdWorksRoadDetails.MAST_ER_ROAD_CODE.ToString().Trim(),"MAST_CD_NUMBER="+cdWorksRoadDetails.MAST_CD_NUMBER.ToString().Trim()}),

                                    URLEncrypt.EncryptParameters1(new string[]{"MAST_ER_ROAD_CODE="+cdWorksRoadDetails.MAST_ER_ROAD_CODE.ToString().Trim(),"MAST_CD_NUMBER="+cdWorksRoadDetails.MAST_CD_NUMBER.ToString().Trim()})


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
        /// populates the Habitation list
        /// </summary>
        /// <param name="page">no of pages</param>
        /// <param name="rows">no of rows</param>
        /// <param name="sidx">sort column name</param>
        /// <param name="sord">sort order</param>
        /// <param name="totalRecords">total no. of records</param>
        /// <param name="roadCode">id of Existing Road</param>
        /// <returns>list of Habitation details of existing roads</returns>
        public Array GetHabitationList(int? page, int? rows, string sidx, string sord, out Int32 totalRecords, int roadCode)
        {
            try
            {
                dbContext = new PMGSYEntities();
                var habitationDetails = (from existingRoadHab in dbContext.MASTER_ER_HABITATION_ROAD
                                         join habitation in dbContext.MASTER_HABITATIONS
                                        on existingRoadHab.MAST_HAB_CODE equals habitation.MAST_HAB_CODE
                                         join habDetails in dbContext.MASTER_HABITATIONS_DETAILS
                                         on habitation.MAST_HAB_CODE equals habDetails.MAST_HAB_CODE
                                         join village in dbContext.MASTER_VILLAGE
                                         on habitation.MAST_VILLAGE_CODE equals village.MAST_VILLAGE_CODE
                                         where existingRoadHab.MAST_ER_ROAD_CODE == roadCode
                                         &&
                                         (PMGSYSession.Current.PMGSYScheme == 1 ? habDetails.MAST_YEAR == 2001 : habDetails.MAST_YEAR == 2011)
                                         select new
                                         {
                                             existingRoadHab.MAST_ER_ROAD_CODE,
                                             habitation.MAST_HAB_CODE,
                                             habitation.MAST_HAB_NAME,
                                             habDetails.MAST_HAB_TOT_POP,
                                             habDetails.MAST_HAB_SCST_POP,
                                             habitation.MASTER_VILLAGE.MAST_VILLAGE_NAME
                                         }
                          ).ToList();

                totalRecords = habitationDetails.Count();

                if (sidx.Trim() != string.Empty)
                {
                    if (sord.ToString() == "asc")
                    {
                        switch (sidx)
                        {
                            case "habitationName":
                                habitationDetails = habitationDetails.OrderBy(x => x.MAST_HAB_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                            case "totalPopulation":
                                habitationDetails = habitationDetails.OrderBy(x => x.MAST_HAB_TOT_POP).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                            case "totalSCSTPopulation":
                                habitationDetails = habitationDetails.OrderBy(x => x.MAST_HAB_SCST_POP).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                        }
                    }
                    else
                    {
                        switch (sidx)
                        {
                            case "habitationName":
                                habitationDetails = habitationDetails.OrderByDescending(x => x.MAST_HAB_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                            case "totalPopulation":
                                habitationDetails = habitationDetails.OrderByDescending(x => x.MAST_HAB_TOT_POP).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                            case "totalSCSTPopulation":
                                habitationDetails = habitationDetails.OrderByDescending(x => x.MAST_HAB_SCST_POP).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                        }
                    }
                }
                else
                {
                    habitationDetails = habitationDetails.OrderBy(x => x.MAST_HAB_CODE).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                }
                return habitationDetails.Select(habDetails => new
                {
                    cell = new[] {         
                        habDetails.MAST_HAB_CODE.ToString(),
                        habDetails.MAST_HAB_NAME,
                        habDetails.MAST_VILLAGE_NAME,
                        habDetails.MAST_HAB_SCST_POP == null?"":habDetails.MAST_HAB_SCST_POP.ToString(),
                        habDetails.MAST_HAB_TOT_POP == null?"":habDetails.MAST_HAB_TOT_POP.ToString(),
                        
                       "<a href='#' title='Click here to edit the Mapped Habitation Details' class='ui-icon ui-icon-pencil ui-align-center' onClick='EditMappedHabitation(" + roadCode.ToString().Trim()  + "," + habDetails.MAST_HAB_CODE+"); return false;'>Edit</a>",

                                    "<a href='#' title='Click here to delete the CD Works Details' class='ui-icon ui-icon-trash ui-align-center' onClick='DeleteMappedHabitation(" + habDetails.MAST_ER_ROAD_CODE.ToString().Trim() + "," + habDetails.MAST_HAB_CODE+"); return false;'>Delete</a>"
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
        /// copying the model details into entity object of Existing road
        /// </summary>
        /// <param name="existingRoadsViewModel">model containing form data</param>
        /// <param name="flagExistingRoadsAddEdit">flag to determine operation(add/edit)</param>
        /// <returns>entity object of Existing Road</returns>
        public MASTER_EXISTING_ROADS CloneExistingRoadsModel(ExistingRoadsViewModel existingRoadsViewModel, bool flagExistingRoadsAddEdit)
        {
            MASTER_EXISTING_ROADS existingRoadsModel = new MASTER_EXISTING_ROADS();

            if (flagExistingRoadsAddEdit)
            {
                existingRoadsModel.MAST_ER_ROAD_CODE = GetMaxExistingRoadCode();
                existingRoadsModel.MAST_ER_ROAD_NUMBER = existingRoadsViewModel.MAST_ER_SHORT_DESC + existingRoadsViewModel.MAST_ER_ROAD_NUMBER;

                if (existingRoadsViewModel.MAST_NOHABS_REASON == 0)
                {
                    existingRoadsModel.MAST_NOHABS_REASON = null;
                    existingRoadsModel.MAST_IS_BENEFITTED_HAB = existingRoadsViewModel.MAST_IS_BENEFITTED_HAB;
                }
                else
                {
                    existingRoadsModel.MAST_NOHABS_REASON = existingRoadsViewModel.MAST_NOHABS_REASON;
                    existingRoadsModel.MAST_IS_BENEFITTED_HAB = existingRoadsViewModel.MAST_IS_BENEFITTED_HAB;
                }
            }
            else
            {
                if (existingRoadsViewModel.MAST_IS_BENEFITTED_HAB == "N")
                {
                    existingRoadsModel.MAST_IS_BENEFITTED_HAB = existingRoadsViewModel.MAST_IS_BENEFITTED_HAB;
                    existingRoadsModel.MAST_NOHABS_REASON = existingRoadsViewModel.MAST_NOHABS_REASON;
                }
                else
                {
                    existingRoadsModel.MAST_IS_BENEFITTED_HAB = existingRoadsViewModel.MAST_IS_BENEFITTED_HAB;
                    existingRoadsModel.MAST_NOHABS_REASON = null;
                }



                existingRoadsModel.MAST_ER_ROAD_CODE = ExistingRoadCode;
                existingRoadsModel.MAST_ER_ROAD_NUMBER = existingRoadsViewModel.MAST_ER_ROAD_NUMBER;
            }

            //Modification for PMGSY II By Abhishek kamble 4-feb-2014                        
            //existingRoadsModel.MAST_PMGSY_SCHEME = Convert.ToByte(PMGSYSession.Current.PMGSYScheme);

            ///PMGSY3
            if (PMGSYSession.Current.PMGSYScheme == 4 || PMGSYSession.Current.PMGSYScheme == 5)
            {
                existingRoadsModel.MAST_PMGSY_SCHEME = 2;
            }
            else
            {
                /*if (flagExistingRoadsAddEdit)
                {
                    //Add
                    existingRoadsModel.MAST_PMGSY_SCHEME = (CheckPMGSY3DAL(PMGSYSession.Current.StateCode) == true) ? (byte)2 : Convert.ToByte(PMGSYSession.Current.PMGSYScheme);
                }
                else
                {
                    //Edit
                    existingRoadsModel.MAST_PMGSY_SCHEME = Convert.ToByte(PMGSYSession.Current.PMGSYScheme);
                }*/
                existingRoadsModel.MAST_PMGSY_SCHEME = Convert.ToByte(PMGSYSession.Current.PMGSYScheme);
            }

            existingRoadsModel.MAST_STATE_CODE = existingRoadsViewModel.MAST_STATE_CODE;
            existingRoadsModel.MAST_DISTRICT_CODE = existingRoadsViewModel.MAST_DISTRICT_CODE;
            existingRoadsModel.MAST_BLOCK_CODE = existingRoadsViewModel.MAST_BLOCK_CODE;

            existingRoadsModel.MAST_ROAD_CAT_CODE = existingRoadsViewModel.MAST_ROAD_CAT_CODE;
            existingRoadsModel.MAST_ER_ROAD_NAME = existingRoadsViewModel.MAST_ER_ROAD_NAME;
            existingRoadsModel.MAST_ER_ROAD_STR_CHAIN = existingRoadsViewModel.MAST_ER_ROAD_STR_CHAIN;
            existingRoadsModel.MAST_ER_ROAD_END_CHAIN = existingRoadsViewModel.MAST_ER_ROAD_END_CHAIN;
            existingRoadsModel.MAST_ER_ROAD_C_WIDTH = existingRoadsViewModel.MAST_ER_ROAD_C_WIDTH;
            existingRoadsModel.MAST_ER_ROAD_F_WIDTH = existingRoadsViewModel.MAST_ER_ROAD_F_WIDTH;
            existingRoadsModel.MAST_ER_ROAD_L_WIDTH = existingRoadsViewModel.MAST_ER_ROAD_L_WIDTH;
            existingRoadsModel.MAST_ER_ROAD_TYPE = existingRoadsViewModel.MAST_ER_ROAD_TYPE;
            existingRoadsModel.MAST_SOIL_TYPE_CODE = existingRoadsViewModel.MAST_SOIL_TYPE_CODE;
            existingRoadsModel.MAST_TERRAIN_TYPE_CODE = existingRoadsViewModel.MAST_TERRAIN_TYPE_CODE;
            existingRoadsModel.MAST_CORE_NETWORK = existingRoadsViewModel.MAST_CORE_NETWORK;

            existingRoadsModel.MAST_ER_ROAD_OWNER = existingRoadsViewModel.MAST_ER_ROAD_OWNER;

            if (existingRoadsViewModel.MAST_ROAD_CAT_CODE == 6)
            {
                existingRoadsModel.MAST_CONS_YEAR = null;
                existingRoadsModel.MAST_RENEW_YEAR = null;
            }
            else
            {
                existingRoadsModel.MAST_CONS_YEAR = existingRoadsViewModel.MAST_CONS_YEAR == 0 ? null : existingRoadsViewModel.MAST_CONS_YEAR;
                existingRoadsModel.MAST_RENEW_YEAR = existingRoadsViewModel.MAST_RENEW_YEAR == 0 ? null : existingRoadsViewModel.MAST_RENEW_YEAR;
            }

            existingRoadsModel.MAST_CD_WORKS_NUM = existingRoadsViewModel.MAST_CD_WORKS_NUM;
            existingRoadsModel.MAST_LOCK_STATUS = existingRoadsViewModel.MAST_LOCK_STATUS;
            ///Changed by SAMMED A. PATIL for provision to update Road Category upwards
            existingRoadsModel.MAST_ER_ROAD_CODE_PMGSY1 = existingRoadsViewModel.MAST_ER_ROAD_CODE_PMGSY1 > 0 ? (int?)existingRoadsViewModel.MAST_ER_ROAD_CODE_PMGSY1 : null;

            return existingRoadsModel;
        }

        /// <summary>
        /// copying the entity object data into view model of existing road
        /// </summary>
        /// <param name="ExistingRoadsModel">contains the details of Existing road</param>
        /// <returns>view model containing Existing Road details</returns>
        public ExistingRoadsViewModel CloneExistingRoadsObject(MASTER_EXISTING_ROADS ExistingRoadsModel)
        {
            ExistingRoadsViewModel existingRoadsViewModel = new ExistingRoadsViewModel();

            existingRoadsViewModel.EncryptedRoadCode = URLEncrypt.EncryptParameters1(new string[] { "RoadCode =" + ExistingRoadsModel.MAST_ER_ROAD_CODE.ToString().Trim() });

            existingRoadsViewModel.MAST_STATE_CODE = ExistingRoadsModel.MAST_STATE_CODE;
            existingRoadsViewModel.MAST_DISTRICT_CODE = ExistingRoadsModel.MAST_DISTRICT_CODE;
            existingRoadsViewModel.MAST_BLOCK_CODE = ExistingRoadsModel.MAST_BLOCK_CODE;

            existingRoadsViewModel.MAST_ER_ROAD_NUMBER = ExistingRoadsModel.MAST_ER_ROAD_NUMBER;
            existingRoadsViewModel.MAST_ROAD_CAT_CODE = ExistingRoadsModel.MAST_ROAD_CAT_CODE;
            existingRoadsViewModel.MAST_ER_ROAD_NAME = ExistingRoadsModel.MAST_ER_ROAD_NAME;
            existingRoadsViewModel.MAST_ER_ROAD_STR_CHAIN = ExistingRoadsModel.MAST_ER_ROAD_STR_CHAIN;
            existingRoadsViewModel.MAST_ER_ROAD_END_CHAIN = ExistingRoadsModel.MAST_ER_ROAD_END_CHAIN;
            existingRoadsViewModel.MAST_ER_ROAD_C_WIDTH = ExistingRoadsModel.MAST_ER_ROAD_C_WIDTH;
            existingRoadsViewModel.MAST_ER_ROAD_F_WIDTH = ExistingRoadsModel.MAST_ER_ROAD_F_WIDTH;
            existingRoadsViewModel.MAST_ER_ROAD_L_WIDTH = ExistingRoadsModel.MAST_ER_ROAD_L_WIDTH;
            existingRoadsViewModel.MAST_ER_ROAD_TYPE = ExistingRoadsModel.MAST_ER_ROAD_TYPE;
            existingRoadsViewModel.MAST_SOIL_TYPE_CODE = ExistingRoadsModel.MAST_SOIL_TYPE_CODE;
            existingRoadsViewModel.MAST_TERRAIN_TYPE_CODE = ExistingRoadsModel.MAST_TERRAIN_TYPE_CODE;
            existingRoadsViewModel.MAST_CORE_NETWORK = ExistingRoadsModel.MAST_CORE_NETWORK;

            existingRoadsViewModel.MAST_IS_BENEFITTED_HAB = ExistingRoadsModel.MAST_IS_BENEFITTED_HAB;
            existingRoadsViewModel.MAST_NOHABS_REASON = ExistingRoadsModel.MAST_NOHABS_REASON;

            existingRoadsViewModel.MAST_ER_ROAD_OWNER = ExistingRoadsModel.MAST_ER_ROAD_OWNER;
            existingRoadsViewModel.MAST_CONS_YEAR = ExistingRoadsModel.MAST_CONS_YEAR;
            existingRoadsViewModel.MAST_RENEW_YEAR = ExistingRoadsModel.MAST_RENEW_YEAR;

            existingRoadsViewModel.MAST_CD_WORKS_NUM = ExistingRoadsModel.MAST_CD_WORKS_NUM;
            existingRoadsViewModel.MAST_LOCK_STATUS = ExistingRoadsModel.MAST_LOCK_STATUS;

            ///Changed by SAMMED A. PATIL for provision to update Road Category upwards
            existingRoadsViewModel.hdnRoadCategoryCode = ExistingRoadsModel.MAST_ROAD_CAT_CODE;
            existingRoadsViewModel.hdn_MAST_ER_ROAD_NUMBER = ExistingRoadsModel.MAST_ER_ROAD_NUMBER;
            existingRoadsViewModel.MAST_ER_ROAD_CODE_PMGSY1 = ExistingRoadsModel.MAST_ER_ROAD_CODE_PMGSY1.HasValue ? ExistingRoadsModel.MAST_ER_ROAD_CODE_PMGSY1.Value : 0;

            return existingRoadsViewModel;
        }

        /// <summary>
        /// calculates the max id of Existing Road
        /// </summary>
        /// <returns>the maximum id of Existing Road</returns>
        public int GetMaxExistingRoadCode()
        {
            int? maxCode = null;
            try
            {
                dbContext = new PMGSYEntities();
                if (dbContext.MASTER_EXISTING_ROADS.Any())
                {
                    maxCode = dbContext.MASTER_EXISTING_ROADS.Max(m => m.MAST_ER_ROAD_CODE) + 1;
                }
                else
                {
                    maxCode = 1;
                }

                //if (maxCode == null)
                //{
                //    maxCode = 1;
                //}
                //else
                //{
                //    maxCode = maxCode + 1;
                //}
                return (int)maxCode;
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);

                return (int)maxCode;
            }
            finally
            {
                dbContext.Dispose();
            }
        }

        /// <summary>
        /// populates the Block dropdown
        /// </summary>
        /// <param name="MAST_DISTRICT_CODE">district code</param>
        /// <returns>list of blocks </returns>
        public List<MASTER_BLOCK> GetAllBlockNames(int MAST_DISTRICT_CODE)
        {
            try
            {
                dbContext = new PMGSYEntities();
                return (from blocks in dbContext.MASTER_BLOCK
                        where blocks.MAST_DISTRICT_CODE == MAST_DISTRICT_CODE &&
                        blocks.MAST_BLOCK_ACTIVE == "Y"
                        select blocks
                             ).ToList<MASTER_BLOCK>();
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
        /// populates the Road Category dropdown
        /// </summary>
        /// <returns>list of Road Category dropdown</returns>
        public List<MASTER_ROAD_CATEGORY> GetAllRoadCategory()
        {
            try
            {
                dbContext = new PMGSYEntities();
                List<MASTER_ROAD_CATEGORY> roadCategoriList = dbContext.MASTER_ROAD_CATEGORY.OrderBy(m => m.MAST_ROAD_CAT_NAME).ToList<MASTER_ROAD_CATEGORY>();

                return roadCategoriList;
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
        /// populates the Owner dropdown
        /// </summary>
        /// <returns>list of Owner dropdown</returns>
        public List<MASTER_AGENCY> GetAllGovOwner()
        {
            try
            {
                dbContext = new PMGSYEntities();
                List<MASTER_AGENCY> govOwnerList = dbContext.MASTER_AGENCY.Where(m => m.MAST_AGENCY_TYPE.ToUpper() == "G" && m.MAST_AGENCY_CODE != 26).OrderBy(m => m.MAST_AGENCY_NAME).ToList<MASTER_AGENCY>();
                return govOwnerList;
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
        /// populates the road owner
        /// </summary>
        /// <returns></returns>
        public List<SelectListItem> PopulateBroRoadOwner()
        {
            try
            {
                dbContext = new PMGSYEntities();
                List<SelectListItem> lstBroRoadOwner = null;
                lstBroRoadOwner = new SelectList(dbContext.MASTER_AGENCY.Where(m => m.MAST_AGENCY_TYPE == "G" && m.MAST_AGENCY_CODE == 26), "MAST_AGENCY_CODE", "MAST_AGENCY_NAME").ToList();

                return lstBroRoadOwner;
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

        /// <summary>
        /// populates the Road Owners
        /// </summary>
        /// <returns></returns>
        public List<SelectListItem> PopulateRoadOwners()
        {
            try
            {
                dbContext = new PMGSYEntities();
                List<SelectListItem> lstRoadOwner = null;
                lstRoadOwner = new SelectList(dbContext.MASTER_AGENCY.Where(m => m.MAST_AGENCY_TYPE == "G" && m.MAST_AGENCY_CODE != 26).OrderBy(m => m.MAST_AGENCY_NAME), "MAST_AGENCY_CODE", "MAST_AGENCY_NAME").ToList();
                lstRoadOwner.Insert(0, (new SelectListItem { Text = "-- Select Road Owner --", Value = "0", Selected = true }));
                return lstRoadOwner;
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

        /// <summary>
        /// populates the Soil type dropdown
        /// </summary>
        /// <returns>list of Soil Type</returns>
        public List<MASTER_SOIL_TYPE> GetAllSoilTypes()
        {
            try
            {
                dbContext = new PMGSYEntities();
                List<MASTER_SOIL_TYPE> soilTypelist = dbContext.MASTER_SOIL_TYPE.OrderBy(m => m.MAST_SOIL_TYPE_NAME).ToList<MASTER_SOIL_TYPE>();
                return soilTypelist;
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
        /// populates the Terrain type dropdown
        /// </summary>
        /// <returns>list of Terrain Type dropdown</returns>
        public List<MASTER_TERRAIN_TYPE> GetAllTerrainTypes()
        {
            try
            {
                dbContext = new PMGSYEntities();
                List<MASTER_TERRAIN_TYPE> terrainTypelist = dbContext.MASTER_TERRAIN_TYPE.OrderBy(m => m.MAST_TERRAIN_TYPE_NAME).ToList<MASTER_TERRAIN_TYPE>();
                return terrainTypelist;
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
        /// returns Road Category Name
        /// </summary>
        /// <param name="roadCategoryCode">id of Road Category</param>
        /// <returns></returns>
        public String GetRoadShortName(int roadCategoryCode)
        {
            try
            {
                dbContext = new PMGSYEntities();
                var data = dbContext.MASTER_ROAD_CATEGORY.Where(m => m.MAST_ROAD_CAT_CODE == roadCategoryCode).ToList();

                string roadShortName = "";
                foreach (var item in data)
                {
                    roadShortName = item.MAST_ROAD_SHORT_DESC;
                }
                return roadShortName;
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
        /// finalize Existing Road
        /// </summary>
        /// <param name="MAST_ER_ROAD_CODE">id of Existing Road</param>
        /// <returns></returns>
        public String FinalizeExistingRoad(int MAST_ER_ROAD_CODE)
        {
            try
            {
                dbContext = new PMGSYEntities();

                MASTER_EXISTING_ROADS masterExistingRoadModel = dbContext.MASTER_EXISTING_ROADS.Find(MAST_ER_ROAD_CODE);

                masterExistingRoadModel.MAST_LOCK_STATUS = "Y";
                masterExistingRoadModel.USERID = PMGSYSession.Current.UserId;
                masterExistingRoadModel.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                dbContext.Entry(masterExistingRoadModel).State = System.Data.Entity.EntityState.Modified;
                dbContext.SaveChanges();
                return string.Empty;
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                return "An Error occured while your request processing";
            }
            finally
            {
                dbContext.Dispose();
            }
        }

        public Array GetCoreNetworkList(int? page, int? rows, string sidx, string sord, out Int32 totalRecords, int roadCode)
        {
            try
            {
                dbContext = new PMGSYEntities();
                var lstPlanRoads = (from item in dbContext.PLAN_ROAD
                                    where item.MAST_ER_ROAD_CODE == roadCode
                                    select new
                                    {
                                        item.PLAN_CN_ROAD_CODE,
                                        item.PLAN_CN_ROAD_NUMBER,
                                        item.PLAN_RD_NAME,
                                        item.PLAN_RD_FROM_TYPE,
                                        item.PLAN_RD_TO_TYPE,
                                        item.PLAN_RD_BLOCK_FROM_CODE,
                                        item.PLAN_RD_BLOCK_TO_CODE,
                                        item.PLAN_RD_FROM_HAB,
                                        item.PLAN_RD_TO_HAB,
                                        item.PLAN_RD_FROM_CHAINAGE,
                                        item.PLAN_RD_TO_CHAINAGE,
                                        item.PLAN_RD_NUM_FROM,
                                        item.PLAN_RD_NUM_TO,
                                        item.PLAN_RD_LENG,
                                        item.PLAN_RD_LENGTH,
                                        item.PLAN_RD_ROUTE,
                                        item.PLAN_RD_FROM,
                                        item.PLAN_RD_TO
                                    }).ToList();


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


                return lstPlanRoads.Select(roadDetails => new
                {
                    cell = new[]
                            {
                                roadDetails.PLAN_CN_ROAD_NUMBER == null?string.Empty:roadDetails.PLAN_CN_ROAD_NUMBER.ToString(),
                                roadDetails.PLAN_RD_NAME == null?string.Empty:roadDetails.PLAN_RD_NAME.ToString(),
                                roadDetails.PLAN_RD_FROM_TYPE=="B"?"Block("+(dbContext.MASTER_BLOCK.Where(item=>item.MAST_BLOCK_CODE == roadDetails.PLAN_RD_BLOCK_FROM_CODE).Select(m=>m.MAST_BLOCK_NAME).FirstOrDefault())+")":(roadDetails.PLAN_RD_FROM_TYPE=="H"?"Habitation("+(dbContext.MASTER_HABITATIONS.Where(item=>item.MAST_HAB_CODE == roadDetails.PLAN_RD_FROM_HAB).Select(m=>m.MAST_HAB_NAME).FirstOrDefault())+")":(roadDetails.PLAN_RD_FROM==null?string.Empty:roadDetails.PLAN_RD_FROM.ToString())),
                                roadDetails.PLAN_RD_TO_TYPE=="B"?"Block("+(dbContext.MASTER_BLOCK.Where(item=>item.MAST_BLOCK_CODE == roadDetails.PLAN_RD_BLOCK_TO_CODE).Select(m=>m.MAST_BLOCK_NAME).FirstOrDefault())+")":(roadDetails.PLAN_RD_TO_TYPE=="H"?"Habitation("+(dbContext.MASTER_HABITATIONS.Where(item=>item.MAST_HAB_CODE == roadDetails.PLAN_RD_TO_HAB).Select(m=>m.MAST_HAB_NAME).FirstOrDefault())+")":(roadDetails.PLAN_RD_TO==null?string.Empty:roadDetails.PLAN_RD_TO.ToString())),
                                roadDetails.PLAN_RD_FROM_CHAINAGE == null?string.Empty:roadDetails.PLAN_RD_FROM_CHAINAGE.ToString(),
                                roadDetails.PLAN_RD_TO_CHAINAGE == null?string.Empty:roadDetails.PLAN_RD_TO_CHAINAGE.ToString(),
                                roadDetails.PLAN_RD_LENGTH == null?string.Empty:roadDetails.PLAN_RD_LENGTH.ToString(),
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

        #endregion Existing Roads DAL Defination

        /// <summary>
        /// populates the Construction Year Dropdown
        /// </summary>
        /// <returns>list of Construction Years</returns>
        public List<SelectListItem> GetConstructionYears()
        {
            List<SelectListItem> yearList = new List<SelectListItem>();

            yearList.Add(
                new SelectListItem()
                {
                    Text = "-- Select Year --",
                    Value = "0"
                }
                );

            for (int i = DateTime.Now.Year; i >= 1950; i--)
            {

                yearList.Add(
                    new SelectListItem()
                    {
                        Text = i.ToString(),
                        Value = i.ToString()
                    }
                    );
            }
            return yearList;
        }

        /// <summary>
        /// Populates the Periodic renewal years
        /// </summary>
        /// <returns></returns>
        public List<SelectListItem> GetPeriodicRenewalYears()
        {
            List<SelectListItem> yearList = new List<SelectListItem>();

            yearList.Add(
                new SelectListItem()
                {
                    Text = "-- Select Year --",
                    Value = "0"
                }
                );

            for (int i = DateTime.Now.Year; i >= 1950; i--)
            {

                yearList.Add(
                    new SelectListItem()
                    {
                        Text = i.ToString(),
                        Value = i.ToString()
                    }
                    );
            }
            return yearList;
        }

        #region Traffic Intensity DAL Defination

        /// <summary>
        /// Save the Traffic Intensity details
        /// </summary>
        /// <param name="trafficViewModel">contains the Traffic details</param>
        /// <param name="message"></param>
        /// <returns></returns>
        public bool AddTrafficIntensity(TrafficViewModel trafficViewModel, ref string message)
        {
            try
            {
                dbContext = new PMGSYEntities();
                MASTER_ER_TRAFFIC_INTENSITY TrafficIntensityModel = CloneTrafficIntensityModel(trafficViewModel);
                TrafficIntensityModel.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                TrafficIntensityModel.USERID = PMGSYSession.Current.UserId;

                dbContext.MASTER_ER_TRAFFIC_INTENSITY.Add(TrafficIntensityModel);

                dbContext.SaveChanges();
                return true;
            }

            catch (OptimisticConcurrencyException ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                message = "An Error Occurred While Processing Your Request.";
                return false;
            }
            catch (UpdateException ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                message = "An Error Occurred While Processing Your Request.";
                return false;
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                message = "An Error Occurred While Processing Your Request.";
                return false;
            }
            finally
            {
                dbContext.Dispose();
            }
        }

        /// <summary>
        /// updates the Traffic Intensity details
        /// </summary>
        /// <param name="trafficViewModel">contains the traffic details</param>
        /// <param name="message">response message</param>
        /// <returns></returns>
        public bool EditTrafficIntensity(TrafficViewModel trafficViewModel, ref string message)
        {
            try
            {
                dbContext = new PMGSYEntities();
                encryptedParameters = trafficViewModel.EncryptedErRoadCode.Split('/');

                if (!(encryptedParameters.Length == 3))
                {
                    return false;
                }

                decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { encryptedParameters[0], encryptedParameters[1], encryptedParameters[2] });

                ExistingRoadCode = Convert.ToInt32(decryptedParameters["RoadCode"].ToString());


                trafficViewModel.MAST_ER_ROAD_CODE = ExistingRoadCode;

                MASTER_ER_TRAFFIC_INTENSITY trafficIntensityModel = CloneTrafficIntensityModel(trafficViewModel);
                trafficIntensityModel.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                trafficIntensityModel.USERID = PMGSYSession.Current.UserId;
                dbContext.Entry(trafficIntensityModel).State = System.Data.Entity.EntityState.Modified;
                dbContext.SaveChanges();
                return true;
            }
            catch (OptimisticConcurrencyException ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                message = "An Error Occurred While Processing Your Request.";
                return false;
            }
            catch (UpdateException ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                message = "An Error Occurred While Processing Your Request.";
                return false;
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                message = "An Error Occurred While Processing Your Request.";
                return false;
            }
            finally
            {
                dbContext.Dispose();
            }
        }

        /// <summary>
        /// deleting the Traffic intensity details
        /// </summary>
        /// <param name="MAST_ER_ROAD_CODE">existing road code</param>
        /// <param name="MAST_TI_YEAR">year code</param>
        /// <param name="message">response message</param>
        /// <returns></returns>
        public Boolean DeleteTrafficIntensity(int MAST_ER_ROAD_CODE, int MAST_TI_YEAR, ref string message)
        {
            using (TransactionScope ts = new TransactionScope())
            {
                try
                {
                    dbContext = new PMGSYEntities();
                    MASTER_ER_TRAFFIC_INTENSITY trafficIntensityModel = dbContext.MASTER_ER_TRAFFIC_INTENSITY.Where(m => m.MAST_ER_ROAD_CODE == MAST_ER_ROAD_CODE && m.MAST_TI_YEAR == MAST_TI_YEAR).FirstOrDefault();

                    if (trafficIntensityModel == null)
                    {
                        return false;
                    }

                    trafficIntensityModel.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                    trafficIntensityModel.USERID = PMGSYSession.Current.UserId;
                    dbContext.Entry(trafficIntensityModel).State = System.Data.Entity.EntityState.Modified;
                    dbContext.SaveChanges();

                    dbContext.MASTER_ER_TRAFFIC_INTENSITY.Remove(trafficIntensityModel);

                    dbContext.SaveChanges();
                    ts.Complete();
                    return true;
                }
                catch (System.Data.Entity.Infrastructure.DbUpdateException ex)
                {
                    ts.Dispose();
                    Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                    message = "You can not delete this Traffic Intensity details because this details are in use.";
                    return false;
                }
                catch (Exception ex)
                {
                    ts.Dispose();
                    Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                    message = "An Error Occurred While Processing Your Request.";
                    return false;
                }
                finally
                {
                    dbContext.Dispose();
                }
            }
        }

        /// <summary>
        /// details of Traffic Intensity 
        /// </summary>
        /// <param name="MAST_ER_ROAD_CODE">Existing Road Code</param>
        /// <param name="MAST_TI_YEAR">year code</param>
        /// <returns></returns>
        public TrafficViewModel GetTrafficIntensity_ByRoadCode(int MAST_ER_ROAD_CODE, int MAST_TI_YEAR)
        {
            try
            {
                dbContext = new PMGSYEntities();

                MASTER_ER_TRAFFIC_INTENSITY trafficIntensityModel = dbContext.MASTER_ER_TRAFFIC_INTENSITY.FirstOrDefault(m => m.MAST_ER_ROAD_CODE == MAST_ER_ROAD_CODE && m.MAST_TI_YEAR == MAST_TI_YEAR);

                TrafficViewModel trafficViewModel = null;
                if (trafficIntensityModel != null)
                {
                    trafficViewModel = CloneTrafficIntensityObject(trafficIntensityModel);
                }
                return trafficViewModel;
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
        /// copying the Traffic Intensity model data into entity object
        /// </summary>
        /// <param name="trafficViewModel">contains the traffic intensity details</param>
        /// <returns></returns>
        public MASTER_ER_TRAFFIC_INTENSITY CloneTrafficIntensityModel(TrafficViewModel trafficViewModel)
        {
            try
            {
                MASTER_ER_TRAFFIC_INTENSITY trafficIntensityModel = new MASTER_ER_TRAFFIC_INTENSITY();

                trafficIntensityModel.MAST_ER_ROAD_CODE = trafficViewModel.MAST_ER_ROAD_CODE;
                trafficIntensityModel.MAST_TI_YEAR = trafficViewModel.MAST_TI_YEAR;
                trafficIntensityModel.MAST_TOTAL_TI = trafficViewModel.MAST_TOTAL_TI;
                trafficIntensityModel.MAST_COMM_TI = trafficViewModel.MAST_COMM_TI;

                return trafficIntensityModel;
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                return null;
            }

        }

        /// <summary>
        /// copying the entity object data into view model
        /// </summary>
        /// <param name="trafficIntensityModel">entity object containing traffic intensity details</param>
        /// <returns></returns>
        public TrafficViewModel CloneTrafficIntensityObject(MASTER_ER_TRAFFIC_INTENSITY trafficIntensityModel)
        {
            TrafficViewModel trafficIntensityViewModel = new TrafficViewModel();

            trafficIntensityViewModel.EncryptedErRoadCode = URLEncrypt.EncryptParameters1(new string[] { "RoadCode =" + trafficIntensityModel.MAST_ER_ROAD_CODE.ToString().Trim() });

            trafficIntensityViewModel.MAST_ER_ROAD_CODE = trafficIntensityModel.MAST_ER_ROAD_CODE;
            trafficIntensityViewModel.MAST_TI_YEAR = trafficIntensityModel.MAST_TI_YEAR;
            trafficIntensityViewModel.MAST_TOTAL_TI = trafficIntensityModel.MAST_TOTAL_TI;
            trafficIntensityViewModel.MAST_COMM_TI = trafficIntensityModel.MAST_COMM_TI;

            trafficIntensityViewModel.RoadName = trafficIntensityModel.MASTER_EXISTING_ROADS.MAST_ER_ROAD_NAME;
            trafficIntensityViewModel.RoadNumber = trafficIntensityModel.MASTER_EXISTING_ROADS.MAST_ER_ROAD_NUMBER;

            return trafficIntensityViewModel;
        }

        /// <summary>
        /// populating traffic intensity years
        /// </summary>
        /// <param name="MAST_ER_ROAD_CODE">existing road code</param>
        /// <returns></returns>
        public List<SelectListItem> PopulateTrafficIntensityYears(int MAST_ER_ROAD_CODE)
        {
            try
            {

                //all years from 2000
                List<SelectListItem> AllYears = PopulateYear(MAST_ER_ROAD_CODE);
                dbContext = new PMGSYEntities();
                var query = (from c in dbContext.MASTER_ER_TRAFFIC_INTENSITY
                             where c.MAST_ER_ROAD_CODE == MAST_ER_ROAD_CODE
                             select new
                             {
                                 Value = c.MAST_TI_YEAR
                             }).ToList();

                foreach (var data in query)
                {
                    AllYears.Remove(AllYears.Where(c => c.Value == data.Value.ToString()).Single());
                }
                return AllYears;
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
        /// populating the year dropdown
        /// </summary>
        /// <param name="MAST_ER_ROAD_CODE">id of existing road</param>
        /// <returns></returns>
        public List<SelectListItem> PopulateYear(int MAST_ER_ROAD_CODE)
        {
            try
            {
                dbContext = new PMGSYEntities();
                List<SelectListItem> lstYears = new List<SelectListItem>();
                SelectListItem item = new SelectListItem();

                for (int i = 2000; i < DateTime.Now.Year; i++)
                {
                    item = new SelectListItem();
                    item.Text = i + "-" + (i + 1);
                    item.Value = i.ToString();
                    lstYears.Add(item);
                }
                return lstYears;
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

        #endregion Traffic Intensity DAL Defination

        #region CBR DAL Defination

        /// <summary>
        /// save the CBR Details
        /// </summary>
        /// <param name="CbrViewModel">contains the CBR details</param>
        /// <param name="message">response message</param>
        /// <returns></returns>
        public bool AddCbrValue(CBRViewModel CbrViewModel, ref string message)
        {
            try
            {
                bool flagCbrAddEdit = true;
                //decimal? totalChainage = dbContext.MASTER_ER_CBR_VALUE.Where(m => m.MAST_ER_ROAD_CODE == CbrViewModel.MAST_ER_ROAD_CODE).Sum(m=>m.);
                MASTER_ER_CBR_VALUE CBRModel = CloneCBRModel(CbrViewModel, flagCbrAddEdit);
                dbContext = new PMGSYEntities();
                CBRModel.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                CBRModel.USERID = PMGSYSession.Current.UserId;
                dbContext.MASTER_ER_CBR_VALUE.Add(CBRModel);
                dbContext.SaveChanges();
                return true;
            }

            catch (OptimisticConcurrencyException ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                message = "An Error Occurred While Processing Your Request.";
                return false;
            }
            catch (UpdateException ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                message = "An Error Occurred While Processing Your Request.";
                return false;
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                message = "An Error Occurred While Processing Your Request.";
                return false;
            }
            finally
            {
                dbContext.Dispose();
            }
        }

        /// <summary>
        /// updates the CBR details
        /// </summary>
        /// <param name="CbrViewModel">contains the updated CBR details</param>
        /// <param name="message">reposnse message</param>
        /// <returns></returns>
        public bool EditCbrValue(CBRViewModel CbrViewModel, ref string message)
        {
            try
            {
                dbContext = new PMGSYEntities();
                bool flagCbrAddEdit = false;

                MASTER_ER_CBR_VALUE CRBModel = CloneCBRModel(CbrViewModel, flagCbrAddEdit);
                CRBModel.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                CRBModel.USERID = PMGSYSession.Current.UserId;
                dbContext.Entry(CRBModel).State = System.Data.Entity.EntityState.Modified;
                dbContext.SaveChanges();
                return true;
            }
            catch (OptimisticConcurrencyException ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                message = "An Error Occurred While Processing Your Request.";
                return false;
            }
            catch (UpdateException ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                message = "An Error Occurred While Processing Your Request.";
                return false;
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                message = "An Error Occurred While Processing Your Request.";
                return false;
            }
            finally
            {
                dbContext.Dispose();
            }
        }

        /// <summary>
        /// delete operation of CBR details
        /// </summary>
        /// <param name="MAST_ER_ROAD_CODE">existing road code</param>
        /// <param name="MAST_SEGMENT_NO">segment id</param>
        /// <param name="message">response message</param>
        /// <returns></returns>
        public Boolean DeleteCbrValue(int MAST_ER_ROAD_CODE, int MAST_SEGMENT_NO, ref string message)
        {
            using (TransactionScope ts = new TransactionScope())
            {
                try
                {
                    dbContext = new PMGSYEntities();
                    MASTER_ER_CBR_VALUE CbrModel = dbContext.MASTER_ER_CBR_VALUE.Where(m => m.MAST_ER_ROAD_CODE == MAST_ER_ROAD_CODE && m.MAST_SEGMENT_NO == MAST_SEGMENT_NO).FirstOrDefault();

                    if (CbrModel == null)
                    {
                        return false;
                    }
                    CbrModel.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                    CbrModel.USERID = PMGSYSession.Current.UserId;
                    dbContext.Entry(CbrModel).State = System.Data.Entity.EntityState.Modified;
                    dbContext.SaveChanges();


                    dbContext.MASTER_ER_CBR_VALUE.Remove(CbrModel);
                    dbContext.SaveChanges();
                    ts.Complete();
                    return true;
                }
                catch (System.Data.Entity.Infrastructure.DbUpdateException ex)
                {
                    ts.Dispose();
                    Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                    message = "You can not delete this CBR details because this details are in use.";
                    return false;
                }
                catch (Exception ex)
                {
                    ts.Dispose();
                    Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                    message = "An Error Occurred While Processing Your Request.";
                    return false;
                }
                finally
                {
                    dbContext.Dispose();
                }
            }
        }

        /// <summary>
        /// returns the CBR details
        /// </summary>
        /// <param name="MAST_ER_ROAD_CODE">Existing road code</param>
        /// <param name="MAST_SEGMENT_NO">segment code</param>
        /// <returns></returns>
        public CBRViewModel GetCBRDetails(int MAST_ER_ROAD_CODE, int MAST_SEGMENT_NO)
        {
            try
            {
                dbContext = new PMGSYEntities();

                MASTER_ER_CBR_VALUE CBRModel = dbContext.MASTER_ER_CBR_VALUE.FirstOrDefault(m => m.MAST_ER_ROAD_CODE == MAST_ER_ROAD_CODE && m.MAST_SEGMENT_NO == MAST_SEGMENT_NO);

                CBRViewModel cbrViewModel = null;
                if (CBRModel != null)
                {
                    cbrViewModel = CloneCBRObject(CBRModel);
                }
                return cbrViewModel;
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
        /// copying CBR details into entity object
        /// </summary>
        /// <param name="CbrViewModel">contains the CBR details</param>
        /// <param name="flagCbrAddEdit">to determine operation</param>
        /// <returns></returns>
        public MASTER_ER_CBR_VALUE CloneCBRModel(CBRViewModel CbrViewModel, bool flagCbrAddEdit)
        {

            MASTER_ER_CBR_VALUE CBRModel = new MASTER_ER_CBR_VALUE();

            if (flagCbrAddEdit)
            {
                CBRModel.MAST_SEGMENT_NO = GetMaxCbrSegmentCode(CbrViewModel.MAST_ER_ROAD_CODE);
                CBRModel.MAST_END_CHAIN = CbrViewModel.MAST_END_CHAIN;
            }
            else
            {
                CBRModel.MAST_SEGMENT_NO = CbrViewModel.MAST_SEGMENT_NO;
                CBRModel.MAST_END_CHAIN = CbrViewModel.EndChainage;
            }

            CBRModel.MAST_ER_ROAD_CODE = CbrViewModel.MAST_ER_ROAD_CODE;
            CBRModel.MAST_STR_CHAIN = CbrViewModel.MAST_STR_CHAIN;

            CBRModel.MAST_CBR_VALUE = CbrViewModel.MAST_CBR_VALUE;

            return CBRModel;
        }

        /// <summary>
        /// calculating the Max segment code
        /// </summary>
        /// <param name="MAST_ER_ROAD_CODE">id of existing road</param>
        /// <returns></returns>
        public int GetMaxCbrSegmentCode(int MAST_ER_ROAD_CODE)
        {
            int? maxCode = null;

            try
            {
                dbContext = new PMGSYEntities();
                maxCode = dbContext.MASTER_ER_CBR_VALUE.Where(m => m.MAST_ER_ROAD_CODE == MAST_ER_ROAD_CODE).Count();

                if (maxCode == 0)
                {
                    maxCode = 1;
                }
                else
                {
                    maxCode = dbContext.MASTER_ER_CBR_VALUE.Where(m => m.MAST_ER_ROAD_CODE == MAST_ER_ROAD_CODE).Max(m => m.MAST_SEGMENT_NO);

                    if (maxCode == null)
                    {
                        maxCode = 1;
                    }
                    else
                    {

                        maxCode = maxCode + 1;
                    }
                }

                return (int)maxCode;
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                return (int)maxCode;
            }
            finally
            {
                dbContext.Dispose();
            }
        }

        /// <summary>
        /// copying the CBR details into CBR view model
        /// </summary>
        /// <param name="CBRModel">entity object containing CBR details</param>
        /// <returns></returns>
        public CBRViewModel CloneCBRObject(MASTER_ER_CBR_VALUE CBRModel)
        {
            CBRViewModel CbrViewModel = new CBRViewModel();

            CbrViewModel.EncryptedCBRCode = CBRModel.MAST_ER_ROAD_CODE.ToString();
            CbrViewModel.MAST_ER_ROAD_CODE = CBRModel.MAST_ER_ROAD_CODE;
            CbrViewModel.MAST_SEGMENT_NO = CBRModel.MAST_SEGMENT_NO;
            CbrViewModel.MAST_STR_CHAIN = CBRModel.MAST_STR_CHAIN;
            CbrViewModel.MAST_END_CHAIN = CBRModel.MAST_END_CHAIN;

            CbrViewModel.EndChainage = CBRModel.MAST_END_CHAIN;

            CbrViewModel.MAST_CBR_VALUE = CBRModel.MAST_CBR_VALUE;

            CbrViewModel.RoadName = CBRModel.MASTER_EXISTING_ROADS.MAST_ER_ROAD_NAME;
            CbrViewModel.RoadID = CBRModel.MASTER_EXISTING_ROADS.MAST_ER_ROAD_NUMBER;

            CbrViewModel.Segment_Length = CBRModel.MAST_END_CHAIN - CBRModel.MAST_STR_CHAIN;

            return CbrViewModel;
        }

        #endregion Traffic Intensity DAL Defination

        #region CdWorks DAL Defination

        /// <summary>
        /// save the CDWorks details
        /// </summary>
        /// <param name="cdWorksViewModel">contains the CDWorks details</param>
        /// <param name="message">response message</param>
        /// <returns></returns>
        public bool AddCDWorksDetails(CdWorksViewModel cdWorksViewModel, ref string message)
        {
            using (TransactionScope objTransactionScope = new TransactionScope())
            {

                try
                {
                    bool flagCdWorksAddEdit = true;
                    MASTER_ER_CDWORKS_ROAD CdWorksModel = CloneCDWorkModel(cdWorksViewModel, flagCdWorksAddEdit);

                    dbContext = new PMGSYEntities();
                    CdWorksModel.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                    CdWorksModel.USERID = PMGSYSession.Current.UserId;
                    dbContext.MASTER_ER_CDWORKS_ROAD.Add(CdWorksModel);
                    dbContext.SaveChanges();

                    MASTER_EXISTING_ROADS existingRoadsModel = dbContext.MASTER_EXISTING_ROADS.Where(m => m.MAST_ER_ROAD_CODE == cdWorksViewModel.MAST_ER_ROAD_CODE).FirstOrDefault();

                    existingRoadsModel.MAST_CD_WORKS_NUM = existingRoadsModel.MAST_CD_WORKS_NUM + 1;
                    existingRoadsModel.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                    existingRoadsModel.USERID = PMGSYSession.Current.UserId;
                    dbContext.Entry(existingRoadsModel).State = System.Data.Entity.EntityState.Modified;
                    dbContext.SaveChanges();
                    objTransactionScope.Complete();
                    return true;
                }

                catch (OptimisticConcurrencyException ex)
                {
                    objTransactionScope.Dispose();
                    Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                    message = "An Error Occurred While Processing Your Request.";
                    return false;
                }
                catch (UpdateException ex)
                {
                    objTransactionScope.Dispose();
                    Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                    message = "An Error Occurred While Processing Your Request.";
                    return false;
                }
                catch (Exception ex)
                {
                    objTransactionScope.Dispose();
                    Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                    message = "An Error Occurred While Processing Your Request.";
                    return false;
                }
                finally
                {
                    dbContext.Dispose();
                }
            }
        }

        /// <summary>
        /// updates the CdWorks details
        /// </summary>
        /// <param name="cdWorksViewModel">contains the updated CDWorks details</param>
        /// <param name="message">response message</param>
        /// <returns></returns>
        public bool EditCDWorksDetails(CdWorksViewModel cdWorksViewModel, ref string message)
        {
            try
            {
                dbContext = new PMGSYEntities();
                bool flagCdWorksAddEdit = false;

                MASTER_ER_CDWORKS_ROAD CdWorksModel = CloneCDWorkModel(cdWorksViewModel, flagCdWorksAddEdit);
                CdWorksModel.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                CdWorksModel.USERID = PMGSYSession.Current.UserId;
                dbContext.Entry(CdWorksModel).State = System.Data.Entity.EntityState.Modified;
                dbContext.SaveChanges();
                return true;
            }
            catch (OptimisticConcurrencyException ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                message = "An Error Occurred While Processing Your Request.";
                return false;
            }
            catch (UpdateException ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                message = "An Error Occurred While Processing Your Request.";
                return false;
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                message = "An Error Occurred While Processing Your Request.";
                return false;
            }
            finally
            {
                dbContext.Dispose();
            }
        }

        /// <summary>
        /// deletes the CDWorks details
        /// </summary>
        /// <param name="MAST_ER_ROAD_CODE">Existing Road code</param>
        /// <param name="MAST_CD_NUMBER">CDWorks code</param>
        /// <param name="message">response message</param>
        /// <returns></returns>
        public Boolean DeleteCDWorksDetails(int MAST_ER_ROAD_CODE, int MAST_CD_NUMBER, ref string message)
        {

            using (TransactionScope ts = new TransactionScope())
            {
                try
                {
                    dbContext = new PMGSYEntities();
                    MASTER_ER_CDWORKS_ROAD CdWorksModel = dbContext.MASTER_ER_CDWORKS_ROAD.Where(m => m.MAST_ER_ROAD_CODE == MAST_ER_ROAD_CODE && m.MAST_CD_NUMBER == MAST_CD_NUMBER).FirstOrDefault();

                    if (CdWorksModel == null)
                    {
                        return false;
                    }

                    CdWorksModel.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                    CdWorksModel.USERID = PMGSYSession.Current.UserId;
                    dbContext.Entry(CdWorksModel).State = System.Data.Entity.EntityState.Modified;
                    dbContext.SaveChanges();



                    dbContext.MASTER_ER_CDWORKS_ROAD.Remove(CdWorksModel);
                    dbContext.SaveChanges();


                    MASTER_EXISTING_ROADS existingRoadsModel = dbContext.MASTER_EXISTING_ROADS.Where(m => m.MAST_ER_ROAD_CODE == MAST_ER_ROAD_CODE).FirstOrDefault();

                    existingRoadsModel.MAST_CD_WORKS_NUM = existingRoadsModel.MAST_CD_WORKS_NUM - 1;
                    existingRoadsModel.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                    existingRoadsModel.USERID = PMGSYSession.Current.UserId;
                    dbContext.Entry(existingRoadsModel).State = System.Data.Entity.EntityState.Modified;
                    dbContext.SaveChanges();
                    ts.Complete();
                    return true;
                }
                catch (System.Data.Entity.Infrastructure.DbUpdateException ex)
                {
                    ts.Dispose();
                    Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                    message = "You can not delete this CdWorks details because this details are in use.";
                    return false;
                }
                catch (Exception ex)
                {
                    ts.Dispose();
                    Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                    message = "An Error Occurred While Processing Your Request.";
                    return false;
                }
                finally
                {
                    dbContext.Dispose();
                }
            }
        }

        /// <summary>
        /// returns the CDWorks details
        /// </summary>
        /// <param name="MAST_ER_ROAD_CODE">existing road code</param>
        /// <param name="MAST_CD_NUMBER">CdWorks code</param>
        /// <returns></returns>
        public CdWorksViewModel GetCDWorksDetails(int MAST_ER_ROAD_CODE, int MAST_CD_NUMBER)
        {
            try
            {
                dbContext = new PMGSYEntities();
                MASTER_ER_CDWORKS_ROAD CdWorksModel = dbContext.MASTER_ER_CDWORKS_ROAD.FirstOrDefault(m => m.MAST_ER_ROAD_CODE == MAST_ER_ROAD_CODE && m.MAST_CD_NUMBER == MAST_CD_NUMBER);

                CdWorksViewModel CdWorksViewModel = null;
                if (CdWorksModel != null)
                {
                    CdWorksViewModel = CloneCDWorkObject(CdWorksModel);
                }
                return CdWorksViewModel;
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
        /// copying the view model data into Entity object
        /// </summary>
        /// <param name="cdWorksViewModel">view model containing CdWorks details</param>
        /// <param name="flagCdWorksAddEdit">determines the add/edit operation</param>
        /// <returns></returns>
        public MASTER_ER_CDWORKS_ROAD CloneCDWorkModel(CdWorksViewModel cdWorksViewModel, bool flagCdWorksAddEdit)
        {

            MASTER_ER_CDWORKS_ROAD CdWorksModel = new MASTER_ER_CDWORKS_ROAD();

            if (flagCdWorksAddEdit)
            {
                CdWorksModel.MAST_CD_NUMBER = GetMaxCdWorksCode(cdWorksViewModel.MAST_ER_ROAD_CODE);
            }
            else
            {
                CdWorksModel.MAST_CD_NUMBER = cdWorksViewModel.MAST_CD_NUMBER;
            }

            CdWorksModel.MAST_ER_ROAD_CODE = cdWorksViewModel.MAST_ER_ROAD_CODE;
            CdWorksModel.MAST_CDWORKS_CODE = cdWorksViewModel.MAST_CDWORKS_CODE;
            CdWorksModel.MAST_CD_LENGTH = cdWorksViewModel.MAST_CD_LENGTH;
            CdWorksModel.MAST_CD_DISCHARGE = cdWorksViewModel.MAST_CD_DISCHARGE;
            CdWorksModel.MAST_CD_CHAINAGE = cdWorksViewModel.MAST_CD_CHAINAGE;
            CdWorksModel.MAST_CONSTRUCT_YEAR = cdWorksViewModel.MAST_CONSTRUCT_YEAR;
            CdWorksModel.MAST_REHAB_YEAR = cdWorksViewModel.MAST_REHAB_YEAR;
            CdWorksModel.MAST_ER_SPAN = cdWorksViewModel.MAST_ER_SPAN;
            CdWorksModel.MAST_CARRIAGE_WAY = cdWorksViewModel.MAST_CARRIAGE_WAY;
            CdWorksModel.MAST_IS_FOOTPATH = cdWorksViewModel.MAST_IS_FOOTPATH;
            return CdWorksModel;
        }

        /// <summary>
        /// copying the CDWorks details into view model
        /// </summary>
        /// <param name="CDWorksModel">entity object containing existing road details</param>
        /// <returns></returns>
        public CdWorksViewModel CloneCDWorkObject(MASTER_ER_CDWORKS_ROAD CDWorksModel)
        {
            CdWorksViewModel CDWorksViewModel = new CdWorksViewModel();

            CDWorksViewModel.MAST_ER_ROAD_CODE = CDWorksModel.MAST_ER_ROAD_CODE;
            CDWorksViewModel.MAST_CD_NUMBER = CDWorksModel.MAST_CD_NUMBER;
            CDWorksViewModel.MAST_CDWORKS_CODE = CDWorksModel.MAST_CDWORKS_CODE;

            CDWorksViewModel.MAST_CD_LENGTH = CDWorksModel.MAST_CD_LENGTH;
            CDWorksViewModel.MAST_CD_DISCHARGE = CDWorksModel.MAST_CD_DISCHARGE;
            CDWorksViewModel.MAST_CD_CHAINAGE = CDWorksModel.MAST_CD_CHAINAGE;

            CDWorksViewModel.MAST_CONSTRUCT_YEAR = CDWorksModel.MAST_CONSTRUCT_YEAR;
            CDWorksViewModel.MAST_REHAB_YEAR = CDWorksModel.MAST_REHAB_YEAR;
            CDWorksViewModel.MAST_ER_SPAN = CDWorksModel.MAST_ER_SPAN;


            CDWorksViewModel.MAST_CARRIAGE_WAY = CDWorksModel.MAST_CARRIAGE_WAY;
            CDWorksViewModel.MAST_IS_FOOTPATH = CDWorksModel.MAST_IS_FOOTPATH;

            CDWorksViewModel.RoadName = CDWorksModel.MASTER_EXISTING_ROADS.MAST_ER_ROAD_NAME;
            CDWorksViewModel.RoadNumber = CDWorksModel.MASTER_EXISTING_ROADS.MAST_ER_ROAD_NUMBER;

            return CDWorksViewModel;
        }

        /// <summary>
        /// determines the max code of existing road
        /// </summary>
        /// <param name="MAST_ER_ROAD_CODE"></param>
        /// <returns></returns>
        public int GetMaxCdWorksCode(int MAST_ER_ROAD_CODE)
        {
            int? maxCode = null;

            try
            {
                dbContext = new PMGSYEntities();
                maxCode = dbContext.MASTER_ER_CDWORKS_ROAD.Where(m => m.MAST_ER_ROAD_CODE == MAST_ER_ROAD_CODE).Count();

                if (maxCode == 0)
                {
                    maxCode = 1;
                }
                else
                {
                    maxCode = dbContext.MASTER_ER_CDWORKS_ROAD.Where(m => m.MAST_ER_ROAD_CODE == MAST_ER_ROAD_CODE).Max(m => m.MAST_CD_NUMBER);

                    if (maxCode == null)
                    {
                        maxCode = 1;
                    }
                    else
                    {

                        maxCode = maxCode + 1;
                    }
                }

                return (int)maxCode;
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                return (int)maxCode;
            }
            finally
            {
                dbContext.Dispose();
            }
        }

        /// <summary>
        /// populates the CDWorks dropdown
        /// </summary>
        /// <returns></returns>
        public List<MASTER_CDWORKS_TYPE_CONSTRUCTION> GetCDWorkTypes()
        {
            try
            {
                dbContext = new PMGSYEntities();
                List<MASTER_CDWORKS_TYPE_CONSTRUCTION> cdWorksList = dbContext.MASTER_CDWORKS_TYPE_CONSTRUCTION.OrderBy(m => m.MAST_CDWORKS_NAME).ToList<MASTER_CDWORKS_TYPE_CONSTRUCTION>();

                return cdWorksList;
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
        /// get block name from block code
        /// </summary>
        /// <param name="blockCode">block id</param>
        /// <returns>block name</returns>
        public string GetBlockName(int blockCode)
        {
            try
            {
                dbContext = new PMGSYEntities();
                return dbContext.MASTER_BLOCK.Where(m => m.MAST_BLOCK_CODE == blockCode).Select(m => m.MAST_BLOCK_NAME).FirstOrDefault(); ;
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

        public MASTER_EXISTING_ROADS GetRoadDetails(int roadCode)
        {
            dbContext = new PMGSYEntities();
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
            finally
            {
                dbContext.Dispose();
            }
        }

        public MASTER_ER_CBR_VALUE GetCBRDetails(int roadCode)
        {
            dbContext = new PMGSYEntities();
            try
            {
                MASTER_ER_CBR_VALUE master = dbContext.MASTER_ER_CBR_VALUE.Where(a => a.MAST_ER_ROAD_CODE == roadCode && a.MAST_SEGMENT_NO == (dbContext.MASTER_ER_CBR_VALUE.Where(m => m.MAST_ER_ROAD_CODE == roadCode).Max(m => m.MAST_SEGMENT_NO))).FirstOrDefault();
                return master;
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


        #endregion CdWorks DAL Defination

        #region Surface Type DAL Defination

        /// <summary>
        /// save the Surface details
        /// </summary>
        /// <param name="SurfaceViewModel">contains surface details</param>
        /// <param name="message"></param>
        /// <returns></returns>
        public bool AddSurfaceDetails(SurfaceTypeViewModel SurfaceViewModel, ref string message)
        {
            using (TransactionScope ts = new TransactionScope())
            {
                dbContext = new PMGSYEntities();
                try
                {
                    bool flagSufaceAddEdit = true;
                    decimal? totalLength = dbContext.MASTER_ER_SURFACE_TYPES.Where(m => m.MAST_ER_ROAD_CODE == SurfaceViewModel.MAST_ER_ROAD_CODE).Sum(m => (Decimal?)m.MAST_ER_SURFACE_LENGTH);
                    if (totalLength == null)
                    {
                        totalLength = 0;
                    }
                    totalLength = SurfaceViewModel.MAST_ER_SURFACE_LENGTH + totalLength;
                    MASTER_EXISTING_ROADS existMaster = dbContext.MASTER_EXISTING_ROADS.Where(m => m.MAST_ER_ROAD_CODE == SurfaceViewModel.MAST_ER_ROAD_CODE).FirstOrDefault();
                    decimal? chainageLength = existMaster.MAST_ER_ROAD_END_CHAIN - existMaster.MAST_ER_ROAD_STR_CHAIN;
                    if (chainageLength < totalLength)
                    {
                        message = "The Remaining Length is less than the entered chainage length.";
                        return false;
                    }
                    MASTER_ER_SURFACE_TYPES SurfaceModel = CloneSurfaceModel(SurfaceViewModel, flagSufaceAddEdit);

                    dbContext = new PMGSYEntities();
                    SurfaceModel.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                    SurfaceModel.USERID = PMGSYSession.Current.UserId;
                    dbContext.MASTER_ER_SURFACE_TYPES.Add(SurfaceModel);
                    dbContext.SaveChanges();
                    ts.Complete();
                    return true;
                }

                catch (OptimisticConcurrencyException ex)
                {
                    ts.Dispose();
                    Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                    message = "An Error Occurred While Processing Your Request.";
                    return false;
                }
                catch (UpdateException ex)
                {
                    ts.Dispose();
                    Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                    message = "An Error Occurred While Processing Your Request.";
                    return false;
                }
                catch (Exception ex)
                {
                    ts.Dispose();
                    Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                    message = "An Error Occurred While Processing Your Request.";
                    return false;
                }
                finally
                {
                    dbContext.Dispose();
                }
            }
        }

        /// <summary>
        /// updates the Surface details
        /// </summary>
        /// <param name="SurfaceViewModel">contains the updates surface details</param>
        /// <param name="message"></param>
        /// <returns></returns>
        public bool EditSurfaceDetails(SurfaceTypeViewModel SurfaceViewModel, ref string message)
        {
            try
            {
                dbContext = new PMGSYEntities();
                bool flagSufaceAddEdit = false;

                MASTER_ER_SURFACE_TYPES SurfaceModel = CloneSurfaceModel(SurfaceViewModel, flagSufaceAddEdit);
                SurfaceModel.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                SurfaceModel.USERID = PMGSYSession.Current.UserId;
                dbContext.Entry(SurfaceModel).State = System.Data.Entity.EntityState.Modified;
                dbContext.SaveChanges();
                return true;
            }
            catch (OptimisticConcurrencyException ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                message = "An Error Occurred While Processing Your Request.";
                return false;
            }
            catch (UpdateException ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                message = "An Error Occurred While Processing Your Request.";
                return false;
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                message = "An Error Occurred While Processing Your Request.";
                return false;
            }
            finally
            {
                dbContext.Dispose();
            }
        }

        /// <summary>
        /// deleting the surface details
        /// </summary>
        /// <param name="MAST_ER_ROAD_CODE">existing road code</param>
        /// <param name="MAST_SURFACE_SEG_NO"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public Boolean DeleteSurfaceDetails(int MAST_ER_ROAD_CODE, int MAST_SURFACE_SEG_NO, ref string message)
        {

            using (TransactionScope ts = new TransactionScope())
            {
                try
                {
                    dbContext = new PMGSYEntities();
                    MASTER_ER_SURFACE_TYPES SurfaceModel = dbContext.MASTER_ER_SURFACE_TYPES.Where(m => m.MAST_ER_ROAD_CODE == MAST_ER_ROAD_CODE && m.MAST_SURFACE_SEG_NO == MAST_SURFACE_SEG_NO).FirstOrDefault();

                    if (SurfaceModel == null)
                    {
                        return false;
                    }

                    SurfaceModel.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                    SurfaceModel.USERID = PMGSYSession.Current.UserId;
                    dbContext.Entry(SurfaceModel).State = System.Data.Entity.EntityState.Modified;
                    dbContext.SaveChanges();



                    dbContext.MASTER_ER_SURFACE_TYPES.Remove(SurfaceModel);
                    dbContext.SaveChanges();
                    ts.Complete();
                    return true;
                }
                catch (System.Data.Entity.Infrastructure.DbUpdateException ex)
                {
                    ts.Dispose();
                    Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                    message = "You can not delete this Surface details because this details are in use";
                    return false;
                }
                catch (Exception ex)
                {
                    ts.Dispose();
                    Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                    message = "An Error Occurred While Processing Your Request.";
                    return false;
                }
                finally
                {
                    dbContext.Dispose();
                }
            }
        }

        /// <summary>
        /// returns the surface details 
        /// </summary>
        /// <param name="MAST_ER_ROAD_CODE"></param>
        /// <param name="MAST_SURFACE_SEG_NO"></param>
        /// <returns></returns>
        public SurfaceTypeViewModel GetSurfaceDetails(int MAST_ER_ROAD_CODE, int MAST_SURFACE_SEG_NO)
        {
            try
            {
                dbContext = new PMGSYEntities();
                MASTER_ER_SURFACE_TYPES SurfaceModel = dbContext.MASTER_ER_SURFACE_TYPES.FirstOrDefault(m => m.MAST_ER_ROAD_CODE == MAST_ER_ROAD_CODE && m.MAST_SURFACE_SEG_NO == MAST_SURFACE_SEG_NO);

                SurfaceTypeViewModel SurfaceViewModel = null;
                if (SurfaceModel != null)
                {
                    SurfaceViewModel = CloneSurfaceObject(SurfaceModel);
                }
                return SurfaceViewModel;
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
        /// copying the Surface details into entity object
        /// </summary>
        /// <param name="SurfaceViewModel">contains the surface details</param>
        /// <param name="flagSufaceAddEdit"></param>
        /// <returns></returns>
        public MASTER_ER_SURFACE_TYPES CloneSurfaceModel(SurfaceTypeViewModel SurfaceViewModel, bool flagSufaceAddEdit)
        {

            MASTER_ER_SURFACE_TYPES SurfaceModel = new MASTER_ER_SURFACE_TYPES();

            if (flagSufaceAddEdit)
            {
                SurfaceModel.MAST_SURFACE_SEG_NO = GetMaxSurfaceSegmentNumber(SurfaceViewModel.MAST_ER_ROAD_CODE);

            }
            else
            {
                SurfaceModel.MAST_SURFACE_SEG_NO = SurfaceViewModel.MAST_SURFACE_SEG_NO;
                //SurfaceModel.MAST_ER_END_CHAIN = SurfaceViewModel.EditModeEndChainage;
            }
            SurfaceModel.MAST_ER_END_CHAIN = SurfaceViewModel.MAST_ER_END_CHAIN;
            SurfaceModel.MAST_ER_ROAD_CODE = SurfaceViewModel.MAST_ER_ROAD_CODE;
            SurfaceModel.MAST_SURFACE_CODE = SurfaceViewModel.MAST_SURFACE_CODE;
            SurfaceModel.MAST_ER_STR_CHAIN = SurfaceViewModel.MAST_ER_STR_CHAIN;

            SurfaceModel.MAST_ER_SURFACE_CONDITION = SurfaceViewModel.MAST_ER_SURFACE_CONDITION;
            SurfaceModel.MAST_ER_SURFACE_LENGTH = Convert.ToDecimal(SurfaceViewModel.MAST_ER_END_CHAIN - SurfaceViewModel.MAST_ER_STR_CHAIN);

            return SurfaceModel;
        }

        /// <summary>
        /// returns max surface code
        /// </summary>
        /// <param name="MAST_ER_ROAD_CODE"></param>
        /// <returns></returns>
        public int GetMaxSurfaceSegmentNumber(int MAST_ER_ROAD_CODE)
        {
            int? maxCode = null;
            dbContext = new PMGSYEntities();
            try
            {
                maxCode = dbContext.MASTER_ER_SURFACE_TYPES.Where(m => m.MAST_ER_ROAD_CODE == MAST_ER_ROAD_CODE).Count();

                if (maxCode == 0)
                {
                    maxCode = 1;
                }
                else
                {
                    maxCode = dbContext.MASTER_ER_SURFACE_TYPES.Where(m => m.MAST_ER_ROAD_CODE == MAST_ER_ROAD_CODE).Max(m => m.MAST_SURFACE_SEG_NO);

                    if (maxCode == null)
                    {
                        maxCode = 1;
                    }
                    else
                    {

                        maxCode = maxCode + 1;
                    }
                }

                return (int)maxCode;
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                return (int)maxCode;
            }
            finally
            {
                dbContext.Dispose();
            }
        }

        /// <summary>
        /// copying the entity object data into view model
        /// </summary>
        /// <param name="SurfaceModel">details of surface</param>
        /// <returns></returns>
        public SurfaceTypeViewModel CloneSurfaceObject(MASTER_ER_SURFACE_TYPES SurfaceModel)
        {
            SurfaceTypeViewModel SurfaceViewModel = new SurfaceTypeViewModel();

            //check updated
            MASTER_EXISTING_ROADS planRoad = dbContext.MASTER_EXISTING_ROADS.Where(m => m.MAST_ER_ROAD_CODE == SurfaceModel.MAST_ER_ROAD_CODE).FirstOrDefault();

            if (planRoad != null)
            {
                SurfaceViewModel.StartChainageOfRoad = Convert.ToDecimal(planRoad.MAST_ER_ROAD_STR_CHAIN);
                SurfaceViewModel.EndChainageOfRoad = Convert.ToDecimal(planRoad.MAST_ER_ROAD_END_CHAIN);
                SurfaceViewModel.SumOfAllSurfaceLength = Convert.ToDecimal(planRoad.MAST_ER_ROAD_END_CHAIN - planRoad.MAST_ER_ROAD_STR_CHAIN);
            }

            SurfaceViewModel.EncryptedRoadCode = SurfaceModel.MAST_ER_ROAD_CODE.ToString();

            SurfaceViewModel.MAST_ER_ROAD_CODE = SurfaceModel.MAST_ER_ROAD_CODE;
            SurfaceViewModel.MAST_SURFACE_SEG_NO = SurfaceModel.MAST_SURFACE_SEG_NO;
            SurfaceViewModel.MAST_SURFACE_CODE = SurfaceModel.MAST_SURFACE_CODE;
            SurfaceViewModel.MAST_ER_STR_CHAIN = SurfaceModel.MAST_ER_STR_CHAIN;

            SurfaceViewModel.MAST_ER_END_CHAIN = SurfaceModel.MAST_ER_END_CHAIN;
            SurfaceViewModel.MAST_ER_SURFACE_CONDITION = SurfaceModel.MAST_ER_SURFACE_CONDITION;
            SurfaceViewModel.MAST_ER_SURFACE_LENGTH = SurfaceModel.MAST_ER_SURFACE_LENGTH;


            SurfaceViewModel.RoadName = SurfaceModel.MASTER_EXISTING_ROADS.MAST_ER_ROAD_NAME;
            SurfaceViewModel.RoadNumber = SurfaceModel.MASTER_EXISTING_ROADS.MAST_ER_ROAD_NUMBER;

            SurfaceViewModel.EditModeEndChainage = SurfaceModel.MAST_ER_END_CHAIN;

            return SurfaceViewModel;
        }

        /// <summary>
        /// populates the Surface list
        /// </summary>
        /// <returns></returns>
        public List<MASTER_SURFACE> GetAllSurface()
        {
            try
            {
                dbContext = new PMGSYEntities();
                List<MASTER_SURFACE> surfaceList = dbContext.MASTER_SURFACE.OrderBy(m => m.MAST_SURFACE_NAME).ToList<MASTER_SURFACE>();
                return surfaceList;
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
        /// populates the Road Condition dropdown
        /// </summary>
        /// <returns></returns>
        public List<SelectListItem> GetRoadCondition()
        {
            List<SelectListItem> roadConditionList = new List<SelectListItem>();



            roadConditionList.Add(

                new SelectListItem()
                {
                    Text = "Good",
                    Value = "G"
                }
                );

            roadConditionList.Add(
                new SelectListItem()
                {
                    Text = "Bad",
                    Value = "B"
                }
                );
            roadConditionList.Add(
                new SelectListItem()
                {
                    Text = "Fair",
                    Value = "F"
                }
                );

            roadConditionList.Insert(0,
                new SelectListItem()
                {
                    Text = "--Select Road Condition--",
                    Value = ""
                }
                );


            return roadConditionList.OrderBy(m => m.Value).ToList();

        }

        #endregion Surface Type DAL Defination

        #region Habitation DAL

        /// <summary>
        /// returns the list of available habitations to map
        /// </summary>
        /// <param name="roadCode">Existing Road id</param>
        /// <param name="page">no. of pages</param>
        /// <param name="rows">no. of rows</param>
        /// <param name="sidx">sort column name</param>
        /// <param name="sord">sort order</param>
        /// <param name="totalRecords">total no. of records</param>
        /// <returns></returns>
        public Array GetHabitationListToMap(int roadCode, int blockCode, int page, int rows, string sidx, string sord, out long totalRecords)
        {
            totalRecords = 0;
            try
            {
                dbContext = new PMGSYEntities();
                MASTER_EXISTING_ROADS masterRoad = dbContext.MASTER_EXISTING_ROADS.Where(m => m.MAST_ER_ROAD_CODE == roadCode).FirstOrDefault();

                ////get total habitation details   old code5-feb-2014
                //var lstHabitations = (from item in dbContext.MASTER_VILLAGE
                //                      join habitation in dbContext.MASTER_HABITATIONS on item.MAST_VILLAGE_CODE equals habitation.MAST_VILLAGE_CODE
                //                      join habitationDetails in dbContext.MASTER_HABITATIONS_DETAILS on habitation.MAST_HAB_CODE equals habitationDetails.MAST_HAB_CODE

                //                      where item.MAST_BLOCK_CODE == masterRoad.MAST_BLOCK_CODE &&
                //                      habitationDetails.MAST_HAB_CONNECTED == "Y"
                //                      select new
                //                      {
                //                          habitation.MAST_HAB_NAME,
                //                          habitation.MAST_HAB_CODE,

                //                      });


                //get total habitation details

                var lstHabitations = (from item in dbContext.MASTER_VILLAGE
                                      join habitation in dbContext.MASTER_HABITATIONS on item.MAST_VILLAGE_CODE equals habitation.MAST_VILLAGE_CODE
                                      join habitationDetails in dbContext.MASTER_HABITATIONS_DETAILS on habitation.MAST_HAB_CODE equals habitationDetails.MAST_HAB_CODE
                                      //join villagePopulation in dbContext.MASTER_VILLAGE_POPULATION on item.MAST_VILLAGE_CODE equals villagePopulation.MAST_VILLAGE_CODE
                                      where item.MAST_BLOCK_CODE == blockCode &&//masterRoad.MAST_BLOCK_CODE && commented by Vikram to allow habitations from another block in the block to map.
                                      habitation.MAST_HABITATION_ACTIVE == "Y" &&  //new condition added by Vikram and below line commented as per suggestion from Dev Sir
                                          //(habitationDetails.MAST_HAB_CONNECTED == "Y" || habitationDetails.MAST_HAB_CONNECTED == "N") &&//the habitation available for mapping may be connected or unconnected as suggested by Dev sir
                                      (PMGSYSession.Current.PMGSYScheme == 1 ? (habitationDetails.MAST_YEAR == 2001) : (habitationDetails.MAST_YEAR == 2011))
                                      select new
                                      {
                                          habitation.MAST_HAB_NAME,
                                          habitation.MAST_HAB_CODE,

                                      });


                //Modified by Abhishek kamble for PMGSY II 5-feb-2014 start

                //Modified by Abhishek kamble for PMGSY II 5-feb-2014 end

                //get mapped habitations details
                //List<int> mapHabitations = (from item in dbContext.MASTER_ER_HABITATION_ROAD
                //                            join data in dbContext.MASTER_EXISTING_ROADS on item.MAST_ER_ROAD_CODE equals data.MAST_ER_ROAD_CODE
                //                            where data.MAST_BLOCK_CODE == masterRoad.MAST_BLOCK_CODE
                //                            && data.MAST_PMGSY_SCHEME==PMGSYSession.Current.PMGSYScheme//added by abhishek kamble 5-feb-2014                                            
                //                            select item.MAST_HAB_CODE).Distinct().ToList<int>();

                List<int> mapHabitations = null;
                if (PMGSYSession.Current.PMGSYScheme == 1)
                {
                    mapHabitations = (from item in dbContext.MASTER_ER_HABITATION_ROAD
                                      join data in dbContext.MASTER_EXISTING_ROADS on item.MAST_ER_ROAD_CODE equals data.MAST_ER_ROAD_CODE
                                      where data.MAST_BLOCK_CODE == masterRoad.MAST_BLOCK_CODE
                                      && data.MAST_PMGSY_SCHEME == PMGSYSession.Current.PMGSYScheme//added by abhishek kamble 5-feb-2014                                            
                                      select item.MAST_HAB_CODE).Distinct().ToList<int>();

                }
                else if (PMGSYSession.Current.PMGSYScheme == 2)
                {
                    mapHabitations = (from item in dbContext.MASTER_ER_HABITATION_ROAD
                                      join data in dbContext.MASTER_EXISTING_ROADS on item.MAST_ER_ROAD_CODE equals data.MAST_ER_ROAD_CODE
                                      where data.MAST_BLOCK_CODE == masterRoad.MAST_BLOCK_CODE
                                      && data.MAST_PMGSY_SCHEME == PMGSYSession.Current.PMGSYScheme//added by abhishek kamble 5-feb-2014                                            
                                      && item.MAST_ER_ROAD_CODE == roadCode
                                      select item.MAST_HAB_CODE).Distinct().ToList<int>();
                }

                var listHab = (from item in lstHabitations
                               where !mapHabitations.Contains(item.MAST_HAB_CODE)
                               select item.MAST_HAB_CODE).Distinct();

                var mappingList = (from item in dbContext.MASTER_HABITATIONS_DETAILS
                                   join habitation in dbContext.MASTER_HABITATIONS on item.MAST_HAB_CODE equals habitation.MAST_HAB_CODE
                                   where
                                   (PMGSYSession.Current.PMGSYScheme == 1 ? (item.MAST_YEAR == 2001) : (item.MAST_YEAR == 2011))//added by abhishek kamble 5-feb-2014
                                       //&&
                                       //listHab.Contains(item.MAST_HAB_CODE)
                                   && habitation.MASTER_VILLAGE.MAST_BLOCK_CODE == blockCode
                                   && habitation.MAST_HABITATION_ACTIVE == "Y"
                                   select new
                                   {
                                       item.MAST_HAB_CODE,
                                       habitation.MAST_HAB_NAME,
                                       item.MAST_HAB_TOT_POP,
                                       habitation.MASTER_VILLAGE.MAST_VILLAGE_NAME
                                   }).Distinct();

                totalRecords = mappingList.Count();

                if (sidx.Trim() != string.Empty)
                {
                    if (sord.ToString() == "asc")
                    {
                        mappingList = mappingList.OrderBy(x => x.MAST_HAB_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                    }
                    else
                    {
                        mappingList = mappingList.OrderByDescending(x => x.MAST_HAB_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                    }
                }
                else
                {
                    mappingList = mappingList.OrderBy(x => x.MAST_HAB_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
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
        /// list of mapped habitations 
        /// </summary>
        /// <param name="roadCode">Existing Road id</param>
        /// <param name="page">no. of pages</param>
        /// <param name="rows">no. of rows</param>
        /// <param name="sidx">sort column name</param>
        /// <param name="sord">sort order</param>
        /// <param name="totalRecords">total no. of records</param>
        /// <returns></returns>
        public Array GetAllHabitationList(int roadCode, int page, int rows, string sidx, string sord, out long totalRecords)
        {
            dbContext = new PMGSYEntities();
            try
            {

                var lstHabitations = from item in dbContext.MASTER_ER_HABITATION_ROAD
                                     join habitation in dbContext.MASTER_HABITATIONS on item.MAST_HAB_CODE equals habitation.MAST_HAB_CODE
                                     join habCode in dbContext.MASTER_HABITATIONS_DETAILS on item.MAST_HAB_CODE equals habCode.MAST_HAB_CODE
                                     join roadPlan in dbContext.MASTER_EXISTING_ROADS on item.MAST_ER_ROAD_CODE equals roadPlan.MAST_ER_ROAD_CODE
                                     where item.MAST_ER_ROAD_CODE == roadCode
                                     && roadPlan.MAST_PMGSY_SCHEME == PMGSYSession.Current.PMGSYScheme //added by abhishek kamble 5-feb-2014
                                     && (PMGSYSession.Current.PMGSYScheme == 1 ? (habCode.MAST_YEAR == 2001) : (habCode.MAST_YEAR == 2011))  //added by abhishek kamble 5-feb-2014

                                     select new
                                     {

                                         habitation.MAST_HAB_NAME,
                                         habCode.MAST_HAB_CODE,
                                         habitation.MASTER_VILLAGE.MAST_VILLAGE_NAME,
                                         habCode.MAST_HAB_TOT_POP,

                                     };


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
                    habDetails.MAST_HAB_NAME,
                    habDetails.MAST_HAB_CODE,
                    habDetails.MAST_HAB_TOT_POP,
                    habDetails.MAST_VILLAGE_NAME

                }).ToArray();


                return result.Select(habDetails => new
                {

                    cell = new[]
                {   
                    habDetails.MAST_HAB_CODE.ToString(),
                    habDetails.MAST_HAB_NAME == null?string.Empty:habDetails.MAST_HAB_NAME.ToString(),
                    habDetails.MAST_VILLAGE_NAME == null ? string.Empty :habDetails.MAST_VILLAGE_NAME.ToString(),
                     habDetails.MAST_HAB_TOT_POP.ToString(),
                         URLEncrypt.EncryptParameters1(new string[]{"HabCode =" + habDetails.MAST_HAB_CODE.ToString().Trim()}),
                }

                }).ToArray();
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                totalRecords = 0;
                return null;
            }
            finally { dbContext.Dispose(); }

        }

        /// <summary>
        /// map operation for habitation to road
        /// </summary>
        /// <param name="encryptedHabCodes">encrypted habitation codes</param>
        /// <param name="roadName"></param>
        /// <returns></returns>
        public bool MapHabitationToRoad(string encryptedHabCodes, string roadName)
        {

            using (TransactionScope objTransactionScope = new TransactionScope())
            {
                try
                {
                    dbContext = new PMGSYEntities();
                    String[] habCodes = null;
                    int roadCode = Convert.ToInt32(roadName);
                    int habCode = 0;

                    //Added By Abhishek kamble 10-feb-2014 start
                    MASTER_EXISTING_ROADS existingRoadModel = dbContext.MASTER_EXISTING_ROADS.Where(m => m.MAST_ER_ROAD_CODE == roadCode).FirstOrDefault();
                    //Added By Abhishek kamble 10-feb-2014 end


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

                        MASTER_ER_HABITATION_ROAD master = new MASTER_ER_HABITATION_ROAD();
                        if (dbContext.MASTER_ER_HABITATION_ROAD.Any())
                        {
                            master.MAST_ER_ROAD_ID = (from item1 in dbContext.MASTER_ER_HABITATION_ROAD select item1.MAST_ER_ROAD_ID).Max() + 1;
                        }
                        else
                        {
                            master.MAST_ER_ROAD_ID = 1;
                        }
                        master.MAST_ER_ROAD_CODE = roadCode;
                        master.MAST_HAB_CODE = habCode;
                        master.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                        master.USERID = PMGSYSession.Current.UserId;
                        dbContext.MASTER_ER_HABITATION_ROAD.Add(master);
                        dbContext.SaveChanges();

                        //Added By Abhishek kamble 6-feb-2014 start

                        if (PMGSYSession.Current.PMGSYScheme == 2)
                        {
                            //Add Habitation For Candidate Roads start
                            var candidateRoadCodes = dbContext.PLAN_ROAD.Where(m => m.MAST_PMGSY_SCHEME == 2 && m.MAST_ER_ROAD_CODE == roadCode && m.MAST_STATE_CODE == existingRoadModel.MAST_STATE_CODE && m.MAST_DISTRICT_CODE == existingRoadModel.MAST_DISTRICT_CODE && m.MAST_BLOCK_CODE == existingRoadModel.MAST_BLOCK_CODE).Select(s => s.PLAN_CN_ROAD_CODE);
                            if (candidateRoadCodes != null)
                            {
                                //get PlanCNRoadCode
                                foreach (var planCnRoadCode in candidateRoadCodes)
                                {
                                    //Added Habitation For PlanCnRoadCode In PLAN ROAD Habitaiton
                                    int? HabId = null;
                                    PLAN_ROAD_HABITATION planRoadHabitationModel = new PLAN_ROAD_HABITATION();

                                    if (!(dbContext.PLAN_ROAD_HABITATION.Where(m => m.PLAN_CN_ROAD_CODE == planCnRoadCode && m.MAST_HAB_CODE == habCode).Any()))
                                    {
                                        //HabId = dbContext.PLAN_ROAD_HABITATION..Max(s => (int?)s.PLAN_CN_ROAD_HAB_ID);
                                        if (dbContext.PLAN_ROAD_HABITATION.Any())
                                        {
                                            HabId = dbContext.PLAN_ROAD_HABITATION.Max(s => (int?)s.PLAN_CN_ROAD_HAB_ID) + 1;
                                        }
                                        else
                                        {
                                            HabId = 1;
                                        }
                                        planRoadHabitationModel.PLAN_CN_ROAD_HAB_ID = HabId.Value;
                                        planRoadHabitationModel.PLAN_CN_ROAD_CODE = planCnRoadCode;
                                        planRoadHabitationModel.MAST_HAB_CODE = habCode;
                                        planRoadHabitationModel.USERID = PMGSYSession.Current.UserId;
                                        planRoadHabitationModel.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                                        dbContext.PLAN_ROAD_HABITATION.Add(planRoadHabitationModel);
                                        dbContext.SaveChanges();
                                    }
                                }
                            }
                            //Add Habitation For Candidate Roads end

                            //Add Habitation For Other Candidate Roads start
                            var otherCandidateRoadCodes = dbContext.PLAN_ROAD_DRRP.Where(m => m.MAST_ER_ROAD_CODE == roadCode).Select(s => s.PLAN_CN_ROAD_CODE);
                            if (otherCandidateRoadCodes != null)
                            {
                                //get PlanCNRoadCode
                                foreach (var planCnRoadCode in otherCandidateRoadCodes)
                                {
                                    //Added Habitation For PlanCnRoadCode In PLAN ROAD Habitaiton
                                    int? HabId = null;
                                    PLAN_ROAD_HABITATION planRoadHabitationModel = new PLAN_ROAD_HABITATION();

                                    if (!(dbContext.PLAN_ROAD_HABITATION.Where(m => m.PLAN_CN_ROAD_CODE == planCnRoadCode && m.MAST_HAB_CODE == habCode).Any()))
                                    {
                                        //HabId = dbContext.PLAN_ROAD_HABITATION..Max(s => (int?)s.PLAN_CN_ROAD_HAB_ID);
                                        if (dbContext.PLAN_ROAD_HABITATION.Any())
                                        {
                                            HabId = dbContext.PLAN_ROAD_HABITATION.Max(s => (int?)s.PLAN_CN_ROAD_HAB_ID) + 1;
                                        }
                                        else
                                        {
                                            HabId = 1;
                                        }
                                        planRoadHabitationModel.PLAN_CN_ROAD_HAB_ID = HabId.Value;
                                        planRoadHabitationModel.PLAN_CN_ROAD_CODE = planCnRoadCode;
                                        planRoadHabitationModel.MAST_HAB_CODE = habCode;
                                        planRoadHabitationModel.USERID = PMGSYSession.Current.UserId;
                                        planRoadHabitationModel.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                                        dbContext.PLAN_ROAD_HABITATION.Add(planRoadHabitationModel);
                                        dbContext.SaveChanges();
                                    }
                                }
                            }
                            //Add Habitation For Other Candidate Roads end
                        }

                        //Added By Abhishek kamble 6-feb-2014 end
                    }

                    objTransactionScope.Complete();
                    return true;
                }
                catch (OptimisticConcurrencyException ex)
                {
                    objTransactionScope.Dispose();
                    Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                    return false;
                }
                catch (UpdateException ex)
                {
                    objTransactionScope.Dispose();
                    Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                    return false;
                }
                catch (Exception ex)
                {
                    objTransactionScope.Dispose();
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
        /// deleting habitation against the road
        /// </summary>
        /// <param name="habitationCode">habitation id</param>
        /// <param name="roadCode">existing road code</param>
        /// <returns></returns>
        public bool DeleteMapHabitation(int habitationCode, int roadCode, out string message)
        {
            using (TransactionScope ts = new TransactionScope())
            {
                try
                {
                    dbContext = new PMGSYEntities();

                    //Added By Abhishek kamble 6-feb-2014 start
                    //var PlanCNRoadCode = dbContext.PLAN_ROAD_DRRP.Where(m => m.MAST_ER_ROAD_CODE == roadCode).Select(s => s.PLAN_CN_ROAD_CODE).FirstOrDefault();

                    if (PMGSYSession.Current.PMGSYScheme == 2)
                    {
                        ///Old Code commented by SAMMED A. PATIL on 28APR2017 to correct the logic

                        //Existing Road details
                        MASTER_EXISTING_ROADS ExistingRoadModel = dbContext.MASTER_EXISTING_ROADS.Where(m => m.MAST_ER_ROAD_CODE == roadCode).FirstOrDefault();

                        /*if (dbContext.PLAN_ROAD.Where(m => m.MAST_PMGSY_SCHEME == PMGSYSession.Current.PMGSYScheme && m.MAST_ER_ROAD_CODE == roadCode && m.MAST_STATE_CODE == ExistingRoadModel.MAST_STATE_CODE && m.MAST_DISTRICT_CODE == ExistingRoadModel.MAST_DISTRICT_CODE && m.MAST_BLOCK_CODE == ExistingRoadModel.MAST_BLOCK_CODE).Any())
                        {
                            message = "Habitation can not be deleted because this habitation is mapped with candidate road.";
                            return false;
                        }
                        if (dbContext.PLAN_ROAD_DRRP.Where(m => m.MAST_ER_ROAD_CODE == roadCode).Any())
                        {
                            message = "Habitation can not be deleted because this habitation is mapped with other candidate road.";
                            return false;
                        }*/

                        int cnRoadCode = dbContext.PLAN_ROAD.Where(x => x.MAST_ER_ROAD_CODE == roadCode).Select(x => x.PLAN_CN_ROAD_CODE).FirstOrDefault();

                        if (dbContext.PLAN_ROAD_HABITATION.Where(m => m.MAST_HAB_CODE == habitationCode && m.PLAN_ROAD.MAST_PMGSY_SCHEME == PMGSYSession.Current.PMGSYScheme && m.PLAN_ROAD.PLAN_CN_ROAD_CODE == cnRoadCode && m.PLAN_ROAD.MAST_STATE_CODE == ExistingRoadModel.MAST_STATE_CODE && m.PLAN_ROAD.MAST_DISTRICT_CODE == ExistingRoadModel.MAST_DISTRICT_CODE && m.PLAN_ROAD.MAST_BLOCK_CODE == ExistingRoadModel.MAST_BLOCK_CODE).Any())
                        {
                            message = "Habitation can not be deleted because this habitation is mapped with other candidate road.";
                            return false;
                        }
                    }

                    //Added By Abhishek kamble 6-feb-2014 end

                    MASTER_ER_HABITATION_ROAD master = dbContext.MASTER_ER_HABITATION_ROAD.Where(m => m.MAST_HAB_CODE == habitationCode && m.MAST_ER_ROAD_CODE == roadCode).FirstOrDefault();

                    master.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                    master.USERID = PMGSYSession.Current.UserId;
                    dbContext.Entry(master).State = System.Data.Entity.EntityState.Modified;
                    dbContext.SaveChanges();

                    dbContext.MASTER_ER_HABITATION_ROAD.Remove(master);
                    dbContext.SaveChanges();
                    message = String.Empty;
                    ts.Complete();
                    return true;
                }
                catch (DbEntityValidationException ex)
                {
                    ts.Dispose();
                    Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                    message = String.Empty;
                    return false;
                }
                catch (Exception ex)
                {
                    ts.Dispose();
                    Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                    message = String.Empty;
                    return false;
                }
                finally
                {
                    dbContext.Dispose();
                }
            }
        }

        /// <summary>
        /// populates the Reason dropdown
        /// </summary>
        /// <returns></returns>
        public List<MASTER_REASON> GetAllReasons()
        {
            try
            {
                dbContext = new PMGSYEntities();
                List<MASTER_REASON> reasonList = dbContext.MASTER_REASON.OrderBy(m => m.MAST_REASON_NAME).Where(m => m.MAST_REASON_TYPE == "H").ToList<MASTER_REASON>();

                reasonList.Insert(0, new MASTER_REASON() { MAST_REASON_CODE = 0, MAST_REASON_NAME = "-- Select Reason --" });

                return reasonList;
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

        public bool CheckMapHabitation(int roadCode, ref string message)
        {
            PMGSYEntities dbContext = new PMGSYEntities();
            try
            {
                if (dbContext.MASTER_ER_HABITATION_ROAD.Any(m => m.MAST_ER_ROAD_CODE == roadCode))
                {
                    message = "Habitations are already mapped to this road.So please delete the habitation to change the status.";
                    return false;
                }
                else
                {
                    message = "";
                    return true;
                }
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                message = "";
                return false;
            }
            finally
            {
                dbContext.Dispose();
            }
        }

        //public bool CheckRangeChainage(decimal startChainage,decimal endChainage,decimal value)
        //{
        //    try
        //    {
        //        if (startChainage <= value && value <= endChainage)
        //        {
        //            return false;
        //        }
        //    }
        //    catch (Exception)
        //    {
        //        return false;
        //    }
        //}


        #endregion Habitation DAL

        #region Map DRRP PMGSY 1 Roads Ends

        public bool checkIsRoadMapped(int erRoadCode)
        {
            PMGSYEntities dbContext = new PMGSYEntities();
            try
            {
                int? mappedDRRP = dbContext.MASTER_EXISTING_ROADS.Where(m => m.MAST_ER_ROAD_CODE == erRoadCode).Select(x => x.MAST_ER_ROAD_CODE_PMGSY1).FirstOrDefault();
                return (mappedDRRP == null ? false : true);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "checkIsRoadMappedDAL()");
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
        public List<SelectListItem> GetDRRPPMGSY1RoadsToMap(int block, int roadCategory)
        {
            SelectListItem item;
            List<SelectListItem> lstDRRPRoads = new List<SelectListItem>();
            PMGSYEntities dbContext = new PMGSYEntities();
            try
            {
                var pmgsy1Roads = dbContext.MASTER_EXISTING_ROADS.Where(b => b.MAST_BLOCK_CODE == block && b.MAST_PMGSY_SCHEME == 1).Select(x => x.MAST_ER_ROAD_CODE).Distinct().ToList();
                var mappedRoads = dbContext.MASTER_EXISTING_ROADS.Where(b => b.MAST_BLOCK_CODE == block && b.MAST_PMGSY_SCHEME == 2 && pmgsy1Roads.Contains(b.MAST_ER_ROAD_CODE_PMGSY1.Value)).Select(x => x.MAST_ER_ROAD_CODE_PMGSY1).Distinct().ToList();

                //var lstPlanRoad = dbContext.MASTER_EXISTING_ROADS.Where(b => b.MAST_BLOCK_CODE == block && b.MAST_PMGSY_SCHEME == 1).ToList();
                var lstPlanRoad = (from drrp in dbContext.MASTER_EXISTING_ROADS
                                   where drrp.MAST_BLOCK_CODE == block && drrp.MAST_PMGSY_SCHEME == 1
                                       //&& drrp.MAST_ROAD_CAT_CODE == roadCategory
                                         && drrp.MAST_ROAD_CAT_CODE <= roadCategory
                                         && !(mappedRoads.Contains(drrp.MAST_ER_ROAD_CODE))
                                   orderby drrp.MAST_ER_ROAD_NAME
                                   select new
                                   {
                                       drrp.MAST_ER_ROAD_CODE,
                                       drrp.MAST_ER_ROAD_NAME,
                                       drrp.MAST_ER_ROAD_NUMBER,
                                       drrp.MASTER_ROAD_CATEGORY.MAST_ROAD_SHORT_DESC,
                                   }).ToList();
                if (lstPlanRoad != null)
                {
                    lstDRRPRoads.Insert(0, new SelectListItem { Text = "Select DRRP Road", Value = "-1" });
                    foreach (var itm in lstPlanRoad)
                    {
                        item = new SelectListItem();
                        item.Text = itm.MAST_ER_ROAD_NAME + " (" + itm.MAST_ER_ROAD_NUMBER + "-" + itm.MAST_ROAD_SHORT_DESC + ")";
                        item.Value = itm.MAST_ER_ROAD_CODE.ToString();
                        lstDRRPRoads.Add(item);
                    }
                }
                return lstDRRPRoads;
            }
            catch (Exception ex)
            {
                //Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                ErrorLog.LogError(ex, "GetDRRPPMGSY1RoadsToMapDAL()");
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

        public bool MapDRRPPMGSY1RoadsDAL(int erRoadCode, int erRoadCode1, ref string message)
        {
            PMGSYEntities dbContext = new PMGSYEntities();
            try
            {

                int recordCount = dbContext.MASTER_EXISTING_ROADS.Where(m => m.MAST_ER_ROAD_CODE_PMGSY1 == erRoadCode1).Count();

                if (recordCount > 0)
                {
                    message = "DRRP Road already mapped.";
                    return false;
                }

                ExistingRoadsViewModel model = new ExistingRoadsViewModel();
                #region
                /*MASTER_EXISTING_ROADS master_existing_roads1 = new MASTER_EXISTING_ROADS();

                MASTER_EXISTING_ROADS master_existing_roads = dbContext.MASTER_EXISTING_ROADS.Where(x => x.MAST_ER_ROAD_CODE == erRoadCode).FirstOrDefault();
                if (master_existing_roads != null)
                {
                    master_existing_roads1.MAST_ER_ROAD_CODE = dbContext.MASTER_EXISTING_ROADS.Any() ? (from item in dbContext.MASTER_EXISTING_ROADS select item.MAST_ER_ROAD_CODE).Max() + 1 : 1;

                    master_existing_roads1.MAST_PMGSY_SCHEME = PMGSYSession.Current.PMGSYScheme;

                    master_existing_roads1.MAST_STATE_CODE = master_existing_roads.MAST_STATE_CODE;
                    master_existing_roads1.MAST_DISTRICT_CODE = master_existing_roads.MAST_DISTRICT_CODE;
                    master_existing_roads1.MAST_BLOCK_CODE = master_existing_roads.MAST_BLOCK_CODE;
                    master_existing_roads1.MAST_ER_ROAD_NUMBER = master_existing_roads.MAST_ER_ROAD_NUMBER;
                    master_existing_roads1.MAST_ROAD_CAT_CODE = master_existing_roads.MAST_ROAD_CAT_CODE;
                    master_existing_roads1.MAST_ER_ROAD_NAME = master_existing_roads.MAST_ER_ROAD_NAME;
                    master_existing_roads1.MAST_ER_ROAD_STR_CHAIN = master_existing_roads.MAST_ER_ROAD_STR_CHAIN;
                    master_existing_roads1.MAST_ER_ROAD_END_CHAIN = master_existing_roads.MAST_ER_ROAD_END_CHAIN;
                    master_existing_roads1.MAST_ER_ROAD_C_WIDTH = master_existing_roads.MAST_ER_ROAD_C_WIDTH;
                    master_existing_roads1.MAST_ER_ROAD_F_WIDTH = master_existing_roads.MAST_ER_ROAD_F_WIDTH;
                    master_existing_roads1.MAST_ER_ROAD_L_WIDTH = master_existing_roads.MAST_ER_ROAD_L_WIDTH;
                    master_existing_roads1.MAST_ER_ROAD_TYPE = master_existing_roads.MAST_ER_ROAD_TYPE;
                    master_existing_roads1.MAST_SOIL_TYPE_CODE = master_existing_roads.MAST_SOIL_TYPE_CODE;
                    master_existing_roads1.MAST_TERRAIN_TYPE_CODE = master_existing_roads.MAST_TERRAIN_TYPE_CODE;
                    master_existing_roads1.MAST_CORE_NETWORK = master_existing_roads.MAST_CORE_NETWORK;
                    master_existing_roads1.MAST_IS_BENEFITTED_HAB = master_existing_roads.MAST_IS_BENEFITTED_HAB;
                    master_existing_roads1.MAST_NOHABS_REASON = master_existing_roads.MAST_NOHABS_REASON;
                    master_existing_roads1.MAST_ER_ROAD_OWNER = master_existing_roads.MAST_ER_ROAD_OWNER;
                    master_existing_roads1.MAST_CONS_YEAR = master_existing_roads.MAST_CONS_YEAR;
                    master_existing_roads1.MAST_RENEW_YEAR = master_existing_roads.MAST_RENEW_YEAR;
                    master_existing_roads1.MAST_CD_WORKS_NUM = master_existing_roads.MAST_CD_WORKS_NUM;
                    master_existing_roads1.MAST_LOCK_STATUS = "N";//master_existing_roads.MAST_LOCK_STATUS;

                    master_existing_roads1.USERID = PMGSYSession.Current.UserId;
                    master_existing_roads1.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                    master_existing_roads1.MAST_ER_ROAD_CODE_PMGSY1 = erRoadCode;

                    dbContext.MASTER_EXISTING_ROADS.Add(master_existing_roads1);
                    dbContext.SaveChanges();
                    return true;
                }*/
                #endregion
                MASTER_EXISTING_ROADS master_existing_roads = dbContext.MASTER_EXISTING_ROADS.Where(x => x.MAST_ER_ROAD_CODE == erRoadCode).FirstOrDefault();
                if (master_existing_roads != null)
                {
                    master_existing_roads.USERID = PMGSYSession.Current.UserId;
                    master_existing_roads.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                    master_existing_roads.MAST_ER_ROAD_CODE_PMGSY1 = erRoadCode1;

                    dbContext.Entry(master_existing_roads).State = System.Data.Entity.EntityState.Modified;
                    dbContext.SaveChanges();
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                //objLog.WriteErrorMessage(ex.Message);
                //Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                ErrorLog.LogError(ex, "MapDRRPPMGSY1RoadsDAL()");
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
        /// list of mapped habitations 
        /// </summary>
        /// <param name="roadCode">Existing Road id</param>
        /// <param name="page">no. of pages</param>
        /// <param name="rows">no. of rows</param>
        /// <param name="sidx">sort column name</param>
        /// <param name="sord">sort order</param>
        /// <param name="totalRecords">total no. of records</param>
        /// <returns></returns>
        public Array GetMappedDRRPPmgsy1ListDAL(int block, int erRoadCode, int page, int rows, string sidx, string sord, out long totalRecords)
        {
            dbContext = new PMGSYEntities();
            try
            {
                //var pmgsy1Roads = dbContext.MASTER_EXISTING_ROADS.Where(b => b.MAST_BLOCK_CODE == block && b.MAST_PMGSY_SCHEME == 1).Select(x => x.MAST_ER_ROAD_CODE).Distinct().ToList();
                //var mappedRoads = dbContext.MASTER_EXISTING_ROADS.Where(b => b.MAST_BLOCK_CODE == block && b.MAST_PMGSY_SCHEME == 2 && pmgsy1Roads.Contains(b.MAST_ER_ROAD_CODE_PMGSY1.Value)).Select(x => x.MAST_ER_ROAD_CODE_PMGSY1).Distinct().ToList();
                int? erRoadCode1 = dbContext.MASTER_EXISTING_ROADS.Where(x => x.MAST_ER_ROAD_CODE == erRoadCode).Select(x => x.MAST_ER_ROAD_CODE_PMGSY1).FirstOrDefault();

                var lstPlanRoad = (from drrp in dbContext.MASTER_EXISTING_ROADS
                                   where drrp.MAST_ER_ROAD_CODE == erRoadCode1
                                   //drrp.MAST_BLOCK_CODE == block && drrp.MAST_PMGSY_SCHEME == 1
                                   //&& (mappedRoads.Contains(drrp.MAST_ER_ROAD_CODE))
                                   select new
                                   {
                                       drrp.MAST_ER_ROAD_CODE,
                                       drrp.MASTER_ROAD_CATEGORY.MAST_ROAD_SHORT_DESC,
                                       drrp.MAST_ER_ROAD_NUMBER,
                                       drrp.MAST_ER_ROAD_NAME,
                                       drrp.MAST_ER_ROAD_TYPE, //Road Type
                                       drrp.MASTER_AGENCY.MAST_AGENCY_NAME,
                                       drrp.MAST_CORE_NETWORK
                                   });

                totalRecords = lstPlanRoad.Count();

                if (sidx.Trim() != string.Empty)
                {
                    if (sord.ToString() == "asc")
                    {
                        lstPlanRoad = lstPlanRoad.OrderBy(x => x.MAST_ER_ROAD_CODE).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                    }
                    //else
                    //{
                    //    lstPlanRoad = lstPlanRoad.OrderByDescending(x => x.MAST_HAB_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                    //}
                }
                else
                {
                    lstPlanRoad = lstPlanRoad.OrderBy(x => x.MAST_ER_ROAD_CODE).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                }

                var result = lstPlanRoad.Select(drrp => new
                {
                    drrp.MAST_ER_ROAD_CODE,
                    drrp.MAST_ROAD_SHORT_DESC,
                    drrp.MAST_ER_ROAD_NUMBER,
                    drrp.MAST_ER_ROAD_NAME,
                    drrp.MAST_ER_ROAD_TYPE,
                    drrp.MAST_AGENCY_NAME,
                    drrp.MAST_CORE_NETWORK
                }).ToArray();

                return result.Select(drrp => new
                {
                    cell = new[]
                {   
                    drrp.MAST_ER_ROAD_CODE.ToString(),
                    drrp.MAST_ROAD_SHORT_DESC,
                    drrp.MAST_ER_ROAD_NUMBER,
                    drrp.MAST_ER_ROAD_NAME,
                    (dbContext.MASTER_EXISTING_ROADS.Where(m=>m.MAST_ER_ROAD_CODE == drrp.MAST_ER_ROAD_CODE).Select(m=>m.MAST_ER_ROAD_END_CHAIN).FirstOrDefault() - dbContext.MASTER_EXISTING_ROADS.Where(m=>m.MAST_ER_ROAD_CODE == drrp.MAST_ER_ROAD_CODE).Select(m=>m.MAST_ER_ROAD_STR_CHAIN).FirstOrDefault()).ToString(),
                    drrp.MAST_ER_ROAD_TYPE == "A" ? "All Weather" : "Full Weather" ,
                    drrp.MAST_AGENCY_NAME,
                    drrp.MAST_CORE_NETWORK.Trim().ToUpper()=="Y"?"Yes":"No",
                    "<a href='#' title='Click here to delete details' class='ui-icon ui-icon-trash ui-align-center' onClick =UnMapDRRPPMGSY1Road('"+URLEncrypt.EncryptParameters1(new string[]{"RoadCode =" + erRoadCode.ToString().Trim()})+"'); return false;'></a>"
                }

                }).ToArray();
            }
            catch (Exception ex)
            {
                //Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                ErrorLog.LogError(ex, "GetMappedDRRPPmgsy1ListDAL()");
                totalRecords = 0;
                return null;
            }
            finally { dbContext.Dispose(); }
        }

        /// <summary>
        /// for deleting the Existing Road details 
        /// </summary>
        /// <param name="_roadCode">id of Existing Road </param>
        /// <param name="message">response message</param>
        /// <returns>response message along with status</returns>
        public bool UnMapDRRPPMGSY1RoadsDAL(int roadCode, ref string message)
        {
            try
            {
                dbContext = new PMGSYEntities();

                using (TransactionScope ts = new TransactionScope())
                {
                    /*if (dbContext.IMS_UNLOCK_DETAILS.Any(m => m.MAST_ER_ROAD_CODE == roadCode))
                    {
                        IMS_UNLOCK_DETAILS unlockDetails = dbContext.IMS_UNLOCK_DETAILS.Where(m => m.MAST_ER_ROAD_CODE == roadCode).FirstOrDefault();
                        if (unlockDetails != null)
                        {
                            dbContext.IMS_UNLOCK_DETAILS.Remove(unlockDetails);
                            dbContext.SaveChanges();
                        }
                    }*/

                    MASTER_EXISTING_ROADS existingRoadsModel = dbContext.MASTER_EXISTING_ROADS.Where(x => x.MAST_ER_ROAD_CODE == roadCode).FirstOrDefault();

                    if (existingRoadsModel == null)
                    {
                        return false;
                    }

                    /*if (dbContext.PLAN_ROAD.Any(m => m.MAST_ER_ROAD_CODE == roadCode))
                    {
                        message = "Road is already mapped with the core network and hence can not be deleted.";
                        return false;
                    }*/

                    existingRoadsModel.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                    existingRoadsModel.USERID = PMGSYSession.Current.UserId;
                    existingRoadsModel.MAST_ER_ROAD_CODE_PMGSY1 = null;

                    dbContext.Entry(existingRoadsModel).State = System.Data.Entity.EntityState.Modified;
                    dbContext.SaveChanges();

                    ts.Complete();
                    return true;
                }
            }
            catch (System.Data.Entity.Infrastructure.DbUpdateException ex)
            {
                //Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                ErrorLog.LogError(ex, "GetMappedDRRPPmgsy1ListDAL().DbUpdateException");
                message = "Existing Roads details can not be unmapped because other details for road are entered.";
                return false;
            }
            catch (Exception ex)
            {
                //Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                ErrorLog.LogError(ex, "GetMappedDRRPPmgsy1ListDAL()");
                message = "An Error Occurred While Your Processing Request.";
                return false;
            }
            finally
            {
                dbContext.Dispose();
            }
        }
        #endregion

        #region PMGSY 3
        ///DRRP 
        public bool CheckPMGSY3DAL(int stateCode)
        {
            PMGSYEntities dbContext = new PMGSYEntities();
            bool flag = false;
            try
            {
                //short roleCode = dbContext.UM_User_Master.Where(m => m.UserID == PMGSYSession.Current.UserId).Select(m => m.DefaultRoleID).FirstOrDefault();

                flag = dbContext.MASTER_PMGSY3.Any(x => x.MAST_STATE_CODE == stateCode && x.MAST_PMGSY3_ACTIVE == "Y");

                if (flag)
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
                //Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                ErrorLog.LogError(ex, "CheckPMGSY3DAL()");
                return false;
            }
            finally
            {
                dbContext.Dispose();
            }
        }

        public bool CheckUnlockedPMGSY3DAL(int blockCode)
        {
            PMGSYEntities dbContext = new PMGSYEntities();
            try
            {
                short roleCode = dbContext.UM_User_Master.Where(m => m.UserID == PMGSYSession.Current.UserId).Select(m => m.DefaultRoleID).FirstOrDefault();

                var count = dbContext.UDF_IMS_UNLOCK_STATUS(PMGSYSession.Current.StateCode, PMGSYSession.Current.DistrictCode, blockCode, 0, 0, 0, 0, 0, "ER", PMGSYSession.Current.PMGSYScheme, roleCode).Select(c => c.UNLOCK_COUNT).FirstOrDefault();

                if (Convert.ToInt32(count) > 0 &&
                      (
                        !(dbContext.MAST_DRRP_BLOCK_PMGSY3_FINALIZE.Any(x => x.MAST_BLOCK_CODE == blockCode && x.IS_FINALIZED == "Y"))
                        &&
                    //!(dbContext.MAST_DRRP_BLOCK_PMGSY3_FINALIZE.Include("MASTER_BLOCK").Any(x => x.MASTER_BLOCK.MASTER_DISTRICT.MAST_DRRP_DISTRICT_PMGSY3_FINALIZE
                    //    .Any(c => c.MAST_DISTRICT_CODE == x.MASTER_BLOCK.MAST_DISTRICT_CODE && c.IS_FINALIZED == "Y")))
                        !(dbContext.MAST_DRRP_DISTRICT_PMGSY3_FINALIZE.Any(x => x.MAST_DISTRICT_CODE == PMGSYSession.Current.DistrictCode && x.IS_FINALIZED == "Y"))
                      ) || (PMGSYSession.Current.StateCode ==3 || PMGSYSession.Current.StateCode == 30 || PMGSYSession.Current.StateCode == 34 )
                   )
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
                ErrorLog.LogError(ex, "CheckUnlockedPMGSY3DAL()");
                return false;
            }
            finally
            {
                dbContext.Dispose();
            }
        }

        public Array ListExistingRoadsPMGSY3DAL(int stateCode, int districtCode, int blockCode, int categoryCode, int ownerCode, int? page, int? rows, string sidx, string sord, out long totalRecords, string filters)
        {
            try
            {
                JavaScriptSerializer js = null;
                PMGSY.Common.CommonFunctions.SearchJson test = new PMGSY.Common.CommonFunctions.SearchJson();
                string roadName = string.Empty;
                int erRoadCode = 0;

                if (filters != null)
                {
                    js = new JavaScriptSerializer();
                    test = js.Deserialize<PMGSY.Common.CommonFunctions.SearchJson>(filters);

                    foreach (PMGSY.Common.CommonFunctions.rules item in test.rules)
                    {
                        switch (item.field)
                        {
                            case "MAST_ER_ROAD_NAME":
                                roadName = item.data;
                                break;
                            case "ERCode":
                                erRoadCode = Convert.ToInt32(item.data);
                                break;
                            default:
                                break;
                        }
                    }
                }

                dbContext = new PMGSYEntities();

                //else if (PMGSYSession.Current.RoleCode == 22 || PMGSYSession.Current.RoleCode == 54)///Changes for RCPLWE
                if (PMGSYSession.Current.StateCode > 0 && PMGSYSession.Current.DistrictCode > 0)//PIU
                {
                    stateCode = PMGSYSession.Current.StateCode;
                    districtCode = PMGSYSession.Current.DistrictCode;
                }
                //if (PMGSYSession.Current.RoleCode == 2)
                else if (PMGSYSession.Current.StateCode > 0)//SRRDA
                {
                    stateCode = PMGSYSession.Current.StateCode;
                }

                short roleCode = dbContext.UM_User_Master.Where(m => m.UserID == PMGSYSession.Current.UserId).Select(m => m.DefaultRoleID).FirstOrDefault();


                ///Get Exisiting Roads for Scheme 2 in case of PMGSY3
                //var lstExistingRoadsDetails = dbContext.GET_EXISTING_ROADS((stateCode <= 0 ? 0 : stateCode), (districtCode <= 0 ? 0 : districtCode), (blockCode <= 0 ? 0 : blockCode), categoryCode, roadName, PMGSYSession.Current.PMGSYScheme, (roleCode == 54 ? (short)22 : roleCode)).ToList();

                //Changes by Shreyas for Scheme 5 (Village Vibrent Scheme) on 22-06-23
                List<GET_EXISTING_ROADS_Result> lstExistingRoadsDetails = new List<GET_EXISTING_ROADS_Result>();
                if (PMGSYSession.Current.PMGSYScheme == 5)
                {
                    lstExistingRoadsDetails = dbContext.GET_EXISTING_ROADS((stateCode <= 0 ? 0 : stateCode), (districtCode <= 0 ? 0 : districtCode), (blockCode <= 0 ? 0 : blockCode), categoryCode, roadName, 4 , (roleCode == 54 ? (short)22 : roleCode)).ToList();
                }
                else
                {
                    lstExistingRoadsDetails = dbContext.GET_EXISTING_ROADS((stateCode <= 0 ? 0 : stateCode), (districtCode <= 0 ? 0 : districtCode), (blockCode <= 0 ? 0 : blockCode), categoryCode, roadName, PMGSYSession.Current.PMGSYScheme, (roleCode == 54 ? (short)22 : roleCode)).ToList();
                }



                if (erRoadCode > 0)
                {
                    lstExistingRoadsDetails = lstExistingRoadsDetails.Where(x => x.MAST_ER_ROAD_CODE == erRoadCode).ToList();
                }

                totalRecords = lstExistingRoadsDetails.Count();

                if (sidx.Trim() != string.Empty)
                {
                    if (sord.ToString() == "asc")
                    {
                        switch (sidx)
                        {
                            case "MAST_ER_ROAD_CODE":
                                lstExistingRoadsDetails = lstExistingRoadsDetails.OrderBy(x => x.MAST_ER_ROAD_CODE).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                            case "MAST_ER_ROAD_NUMBER":
                                lstExistingRoadsDetails = lstExistingRoadsDetails.OrderBy(x => x.MAST_ER_ROAD_NUMBER).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                            case "MAST_ER_ROAD_NAME":
                                lstExistingRoadsDetails = lstExistingRoadsDetails.OrderBy(x => x.MAST_ER_ROAD_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                            case "MAST_ER_ROAD_OWNER":
                                lstExistingRoadsDetails = lstExistingRoadsDetails.OrderBy(x => x.AGENCY_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                            case "MAST_CORE_NETWORK":
                                lstExistingRoadsDetails = lstExistingRoadsDetails.OrderBy(x => x.MAST_CORE_NETWORK).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                            case "MAST_ROAD_SHORT_DESC":
                                lstExistingRoadsDetails = lstExistingRoadsDetails.OrderBy(x => x.MAST_ROAD_SHORT_DESC).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                            case "MAST_ER_ROAD_TYPE":
                                lstExistingRoadsDetails = lstExistingRoadsDetails.OrderBy(x => x.MAST_ER_ROAD_TYPE).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                            default:
                                lstExistingRoadsDetails = lstExistingRoadsDetails.OrderBy(x => x.MAST_ER_ROAD_CODE).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                        }
                    }
                    else
                    {

                        switch (sidx)
                        {
                            case "MAST_ER_ROAD_CODE":
                                lstExistingRoadsDetails = lstExistingRoadsDetails.OrderByDescending(x => x.MAST_ER_ROAD_CODE).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                            case "MAST_ER_ROAD_NUMBER":
                                lstExistingRoadsDetails = lstExistingRoadsDetails.OrderByDescending(x => x.MAST_ER_ROAD_NUMBER).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                            case "MAST_ER_ROAD_NAME":
                                lstExistingRoadsDetails = lstExistingRoadsDetails.OrderByDescending(x => x.MAST_ER_ROAD_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                            case "MAST_ER_ROAD_OWNER":
                                lstExistingRoadsDetails = lstExistingRoadsDetails.OrderByDescending(x => x.AGENCY_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                            case "MAST_CORE_NETWORK":
                                lstExistingRoadsDetails = lstExistingRoadsDetails.OrderByDescending(x => x.MAST_CORE_NETWORK).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                            case "MAST_ROAD_SHORT_DESC":
                                lstExistingRoadsDetails = lstExistingRoadsDetails.OrderByDescending(x => x.MAST_ROAD_SHORT_DESC).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                            case "MAST_ER_ROAD_TYPE":
                                lstExistingRoadsDetails = lstExistingRoadsDetails.OrderByDescending(x => x.MAST_ER_ROAD_TYPE).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                            default:
                                lstExistingRoadsDetails = lstExistingRoadsDetails.OrderByDescending(x => x.MAST_ER_ROAD_CODE).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                        }
                    }
                }
                else
                {
                    lstExistingRoadsDetails = lstExistingRoadsDetails.OrderByDescending(x => x.MAST_ER_ROAD_CODE).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                }

                var result = lstExistingRoadsDetails.Select(roadDetails => new
                {
                    roadDetails.MAST_ER_ROAD_CODE,
                    roadDetails.MAST_ROAD_SHORT_DESC,
                    roadDetails.MAST_ER_ROAD_NUMBER,
                    roadDetails.MAST_ER_ROAD_NAME,
                    roadDetails.MAST_ER_ROAD_TYPE,
                    roadDetails.AGENCY_NAME,
                    roadDetails.MAST_CORE_NETWORK,
                    roadDetails.UNLOCK_BY_MORD,
                    //MAST_ER_ROAD_CODE_PMGSY1 = dbContext.MASTER_EXISTING_ROADS.Where(m => m.MAST_ER_ROAD_CODE == roadDetails.MAST_ER_ROAD_CODE).Select(m => m.MAST_ER_ROAD_CODE_PMGSY1).FirstOrDefault(),
                    roadDetails.MAST_ER_ROAD_CODE_PMGSY1,
                    roadDetails.MAST_ROAD_CAT_CODE_PMGSY2,
                    roadDetails.MAST_ROAD_CAT_CODE_PMGSY1,
                }).ToArray();

                return result.Select(item => new
                {

                    id = item.MAST_ER_ROAD_CODE,
                    cell = new[]
                    {
                        item.MAST_ER_ROAD_CODE.ToString(),
                        item.MAST_ROAD_SHORT_DESC, //Road Category Short Desc
                        item.MAST_ER_ROAD_NUMBER,
                        item.MAST_ER_ROAD_NAME,
                         item.MAST_ER_ROAD_TYPE, //Road Type
                        (dbContext.MASTER_EXISTING_ROADS.Where(m=>m.MAST_ER_ROAD_CODE == item.MAST_ER_ROAD_CODE).Select(m=>m.MAST_ER_ROAD_END_CHAIN).FirstOrDefault() - dbContext.MASTER_EXISTING_ROADS.Where(m=>m.MAST_ER_ROAD_CODE == item.MAST_ER_ROAD_CODE).Select(m=>m.MAST_ER_ROAD_STR_CHAIN).FirstOrDefault()).ToString(),
                        item.AGENCY_NAME,
                        item.MAST_CORE_NETWORK.ToString().Trim().ToUpper()=="Y"?"Yes":"No",
                        
                        #region
                        /*//CD Works
                        dbContext.MASTER_EXISTING_ROADS.Where(m=>m.MAST_ER_ROAD_CODE == item.MAST_ER_ROAD_CODE).Select(m=>m.MAST_LOCK_STATUS_PMGSY3).FirstOrDefault() == "N"
                        ?   "<a href='#' title='Click here to add CDWorks details' class='ui-icon ui-icon-plusthick ui-align-center' onClick =CDWorks('"+URLEncrypt.EncryptParameters1(new string[]{"RoadCode =" + item.MAST_ER_ROAD_CODE.ToString().Trim()})                              +"'); return false;'></a>"
                        : "<span class='ui-icon ui-icon-locked ui-align-center'></span>",  

                        //Surface Type
                        dbContext.MASTER_EXISTING_ROADS.Where(m=>m.MAST_ER_ROAD_CODE == item.MAST_ER_ROAD_CODE).Select(m=>m.MAST_LOCK_STATUS_PMGSY3).FirstOrDefault() == "N"
                        ?   "<a href='#' title='Click here to add surface type details' class='ui-icon ui-icon-plusthick ui-align-center' onClick =SurfaceTypes('"+URLEncrypt.EncryptParameters1(new string[]{"RoadCode =" + item.MAST_ER_ROAD_CODE.ToString                                ().Trim()})+"'); return false;'></a>"
                        : "<span class='ui-icon ui-icon-locked ui-align-center'></span>",   
                        //Habitation
                        dbContext.MASTER_EXISTING_ROADS.Where(m=>m.MAST_ER_ROAD_CODE == item.MAST_ER_ROAD_CODE).Select(m=>m.MAST_LOCK_STATUS_PMGSY3).FirstOrDefault() == "N"
                        ?   "<a href='#' title='Click here to add habitation details' class='ui-icon ui-icon-plusthick ui-align-center' onClick =HabitationsMapped('"+URLEncrypt.EncryptParameters1(new string[]{"RoadCode =" + item.MAST_ER_ROAD_CODE.ToString().Trim()})+"'); return false;'></a>"
                        : "<span class='ui-icon ui-icon-locked ui-align-center'></span>",   
                        //Traffic Intensity
                        dbContext.MASTER_EXISTING_ROADS.Where(m=>m.MAST_ER_ROAD_CODE == item.MAST_ER_ROAD_CODE).Select(m=>m.MAST_LOCK_STATUS_PMGSY3).FirstOrDefault() == "N"
                        ?   "<a href='#' title='Click here to add traffic intensity details' class='ui-icon ui-icon-plusthick ui-align-center' onClick =TrafficIntensity('"+URLEncrypt.EncryptParameters1(new string[]{"RoadCode =" +                                                       item.MAST_ER_ROAD_CODE.ToString().Trim()})+"'); return false;'></a>"
                        :   "<span class='ui-icon ui-icon-locked ui-align-center'></span>",   
                        //CBR
                        dbContext.MASTER_EXISTING_ROADS.Where(m=>m.MAST_ER_ROAD_CODE == item.MAST_ER_ROAD_CODE).Select(m=>m.MAST_LOCK_STATUS_PMGSY3).FirstOrDefault() == "N"
                        ?   "<a href='#' title='Click here to add CBR details' class='ui-icon ui-icon-plusthick ui-align-center' onClick =CBRValue('"+URLEncrypt.EncryptParameters1(new string[]{"RoadCode =" + item.MAST_ER_ROAD_CODE.ToString().Trim()})                                  +"'); return false;'></a>"
                        :   "<span class='ui-icon ui-icon-locked ui-align-center'></span>",   
                        "<a href='#' title='Click here to view existing roads details' class='ui-icon ui-icon-zoomin ui-align-center' onClick =ShowDetails('"+URLEncrypt.EncryptParameters1(new string[]{"RoadCode =" + item.MAST_ER_ROAD_CODE.ToString().Trim()})+"'); return false;'></a>",
                        
                        ///Changed by SAMMED A. PATIL on 15FEB2017 for Map PMGSY1 DRRP Roads
                        ((PMGSYSession.Current.PMGSYScheme == 2 && (PMGSYSession.Current.RoleCode == 22 || PMGSYSession.Current.RoleCode == 25))
                        ? (item.MAST_ER_ROAD_CODE_PMGSY1 == 0) 
                            ? "<a href='#' title='Click here to Map DRRP PMGSY - I Road' class='ui-icon ui-icon-plusthick ui-align-center' onClick =MapDRRPPMGSY1('"+URLEncrypt.EncryptParameters1(new string[]{"BlockCode =" + blockCode.ToString().Trim(), "RoadCode =" + item.MAST_ER_ROAD_CODE.ToString().Trim(), "RoadCategory=" + item.MAST_ROAD_CAT_CODE_PMGSY2.ToString().Trim()})+"'); return false;'></a>" 
                            : //(dbContext.MASTER_EXISTING_ROADS.Where(m=>m.MAST_ER_ROAD_CODE == item.MAST_ER_ROAD_CODE_PMGSY1).Select(m=>m.MAST_ER_ROAD_NUMBER).FirstOrDefault())
                              "<a href='#' onClick =MapDRRPPMGSY1('"+URLEncrypt.EncryptParameters1(new string[]{"BlockCode =" + blockCode.ToString().Trim(), "RoadCode =" + item.MAST_ER_ROAD_CODE.ToString().Trim(), "RoadCategory=" + item.MAST_ROAD_CAT_CODE_PMGSY2.ToString().Trim()})+"'); return false;'>"+ dbContext.MASTER_EXISTING_ROADS.Where(m=>m.MAST_ER_ROAD_CODE == item.MAST_ER_ROAD_CODE_PMGSY1).Select(m=>m.MAST_ER_ROAD_NUMBER).FirstOrDefault() +"</a>" 
                        : "-"),

                        ///Changes by SAMMED A. PATIL on 03 OCTOBER 2017 to edit DRRP for MORD
                        (dbContext.MASTER_EXISTING_ROADS.Where(m=>m.MAST_ER_ROAD_CODE == item.MAST_ER_ROAD_CODE).Select(m=>m.MAST_LOCK_STATUS_PMGSY3).FirstOrDefault() == "N"
                        ?   "<a href='#' title='Click here to edit details' class='ui-icon ui-icon-pencil ui-align-center' onClick =EditDetails('"+URLEncrypt.EncryptParameters1(new string[]{"RoadCode =" + item.MAST_ER_ROAD_CODE.ToString().Trim()})+"');                                return false;'></a>"
                        :   "<span class='ui-icon ui-icon-locked ui-align-center'></span>"),

                        //Delete
                                (dbContext.MASTER_EXISTING_ROADS.Where(m=>m.MAST_ER_ROAD_CODE == item.MAST_ER_ROAD_CODE).Select(m=>m.MAST_LOCK_STATUS).FirstOrDefault() == "N" || item.UNLOCK_BY_MORD == "M")
                                ? ("<a href='#' title='Click here to delete details' class='ui-icon ui-icon-trash ui-align-center' onClick =DeleteDetails('"+URLEncrypt.EncryptParameters1(new string[]{"RoadCode =" + item.MAST_ER_ROAD_CODE.ToString                                              ().Trim()})+"'); return false;'></a>")
                                : "<span class='ui-icon ui-icon-locked ui-align-center'></span>"
                               ,   */
                        #endregion

                        dbContext.MASTER_EXISTING_ROADS.Where(m=>m.MAST_ER_ROAD_CODE == item.MAST_ER_ROAD_CODE).Select(m=>m.MAST_LOCK_STATUS).FirstOrDefault() == "N"?"<a href='#' title='Click here to add CDWorks details' class='ui-icon ui-icon-plusthick ui-align-center' onClick =CDWorks('"+URLEncrypt.EncryptParameters1(new string[]{"RoadCode =" + item.MAST_ER_ROAD_CODE.ToString().Trim()})+"'); return false;'></a>":item.UNLOCK_BY_MORD == "M"?"<a href='#' title='Click here to add CDWorks details' class='ui-icon ui-icon-unlocked ui-align-center' onClick =CDWorks('"+URLEncrypt.EncryptParameters1(new string[]{"RoadCode =" + item.MAST_ER_ROAD_CODE.ToString().Trim()})+"'); return false;'></a>":item.UNLOCK_BY_MORD.ToString().ToUpper()=="Y"?"<span class='ui-icon ui-icon-locked ui-align-center'></span>":"<a href='#' title='Click here to add CDWorks details' class='ui-icon ui-icon-plusthick ui-align-center' onClick =CDWorks('"+URLEncrypt.EncryptParameters1(new string[]{"RoadCode =" + item.MAST_ER_ROAD_CODE.ToString().Trim()})+"'); return false;'></a>",
                        dbContext.MASTER_EXISTING_ROADS.Where(m=>m.MAST_ER_ROAD_CODE == item.MAST_ER_ROAD_CODE).Select(m=>m.MAST_LOCK_STATUS).FirstOrDefault() == "N"?"<a href='#' title='Click here to add surface type details' class='ui-icon ui-icon-plusthick ui-align-center' onClick =SurfaceTypes('"+URLEncrypt.EncryptParameters1(new string[]{"RoadCode =" + item.MAST_ER_ROAD_CODE.ToString().Trim()})+"'); return false;'></a>":item.UNLOCK_BY_MORD == "M"?"<a href='#' title='Click here to add surface type details' class='ui-icon ui-icon-unlocked ui-align-center' onClick =SurfaceTypes('"+URLEncrypt.EncryptParameters1(new string[]{"RoadCode =" + item.MAST_ER_ROAD_CODE.ToString().Trim()})+"'); return false;'></a>":item.UNLOCK_BY_MORD.ToString().ToUpper()=="Y"?"<span class='ui-icon ui-icon-locked ui-align-center'></span>":"<a href='#' title='Click here to add surface type details' class='ui-icon ui-icon-plusthick ui-align-center' onClick =SurfaceTypes('"+URLEncrypt.EncryptParameters1(new string[]{"RoadCode =" + item.MAST_ER_ROAD_CODE.ToString().Trim()})+"'); return false;'></a>",
                        dbContext.MASTER_EXISTING_ROADS.Where(m=>m.MAST_ER_ROAD_CODE == item.MAST_ER_ROAD_CODE).Select(m=>m.MAST_LOCK_STATUS).FirstOrDefault() == "N"?"<a href='#' title='Click here to add habitation details' class='ui-icon ui-icon-plusthick ui-align-center' onClick =HabitationsMapped('"+URLEncrypt.EncryptParameters1(new string[]{"RoadCode =" + item.MAST_ER_ROAD_CODE.ToString().Trim()})+"'); return false;'></a>":item.UNLOCK_BY_MORD == "M"?"<a href='#' title='Click here to add Habitation details' class='ui-icon ui-icon-unlocked ui-align-center' onClick =HabitationsMapped('"+URLEncrypt.EncryptParameters1(new string[]{"RoadCode =" + item.MAST_ER_ROAD_CODE.ToString().Trim()})+"'); return false;'></a>":item.UNLOCK_BY_MORD.ToString().ToUpper()=="Y"?"<span class='ui-icon ui-icon-locked ui-align-center'></span>":"<a href='#' title='Click here to add habitation details' class='ui-icon ui-icon-plusthick ui-align-center' onClick =HabitationsMapped('"+URLEncrypt.EncryptParameters1(new string[]{"RoadCode =" + item.MAST_ER_ROAD_CODE.ToString().Trim()})+"'); return false;'></a>",
                        dbContext.MASTER_EXISTING_ROADS.Where(m=>m.MAST_ER_ROAD_CODE == item.MAST_ER_ROAD_CODE).Select(m=>m.MAST_LOCK_STATUS).FirstOrDefault() == "N"?"<a href='#' title='Click here to add traffic intensity details' class='ui-icon ui-icon-plusthick ui-align-center' onClick =TrafficIntensity('"+URLEncrypt.EncryptParameters1(new string[]{"RoadCode =" + item.MAST_ER_ROAD_CODE.ToString().Trim()})+"'); return false;'></a>":item.UNLOCK_BY_MORD == "M"?"<a href='#' title='Click here to add Traffic Intensity details' class='ui-icon ui-icon-unlocked ui-align-center' onClick =TrafficIntensity('"+URLEncrypt.EncryptParameters1(new string[]{"RoadCode =" + item.MAST_ER_ROAD_CODE.ToString().Trim()})+"'); return false;'></a>":item.UNLOCK_BY_MORD.ToString().ToUpper()=="Y"?"<span class='ui-icon ui-icon-locked ui-align-center'></span>":"<a href='#' title='Click here to add traffic intensity details' class='ui-icon ui-icon-plusthick ui-align-center' onClick =TrafficIntensity('"+URLEncrypt.EncryptParameters1(new string[]{"RoadCode =" + item.MAST_ER_ROAD_CODE.ToString().Trim()})+"'); return false;'></a>",
                        dbContext.MASTER_EXISTING_ROADS.Where(m=>m.MAST_ER_ROAD_CODE == item.MAST_ER_ROAD_CODE).Select(m=>m.MAST_LOCK_STATUS).FirstOrDefault() == "N"?"<a href='#' title='Click here to add CBR details' class='ui-icon ui-icon-plusthick ui-align-center' onClick =CBRValue('"+URLEncrypt.EncryptParameters1(new string[]{"RoadCode =" + item.MAST_ER_ROAD_CODE.ToString().Trim()})+"'); return false;'></a>":item.UNLOCK_BY_MORD == "M"?"<a href='#' title='Click here to add CBR details' class='ui-icon ui-icon-unlocked ui-align-center' onClick =CBRValue('"+URLEncrypt.EncryptParameters1(new string[]{"RoadCode =" + item.MAST_ER_ROAD_CODE.ToString().Trim()})+"'); return false;'></a>":item.UNLOCK_BY_MORD.ToString().ToUpper()=="Y"?"<span class='ui-icon ui-icon-locked ui-align-center'></span>":"<a href='#' title='Click here to add CBR details' class='ui-icon ui-icon-plusthick ui-align-center' onClick =CBRValue('"+URLEncrypt.EncryptParameters1(new string[]{"RoadCode =" + item.MAST_ER_ROAD_CODE.ToString().Trim()})+"'); return false;'></a>",   
                       
                        
                        // Added on 27 Jan 2021
                        
                         // Shift DRRP TO  new District and Block 27 Jan 2021 
                        "<a href='#' title='Click here to shift existing roads details' class='ui-icon ui-icon-unlocked ui-align-center' onClick =ShiftDRRPToNewBlockAndDistrictPMGSY3('"+URLEncrypt.EncryptParameters1(new string[]{"RoadCode =" + item.MAST_ER_ROAD_CODE.ToString().Trim()})+"'); return false;'></a>",



                        "<a href='#' title='Click here to view existing roads details' class='ui-icon ui-icon-zoomin ui-align-center' onClick =ShowDetails('"+URLEncrypt.EncryptParameters1(new string[]{"RoadCode =" + item.MAST_ER_ROAD_CODE.ToString().Trim()})+"'); return false;'></a>",
                        
                        ///Changed by SAMMED A. PATIL on 15FEB2017 for Map PMGSY1 DRRP Roads
                        //(PMGSYSession.Current.PMGSYScheme == 2 && (PMGSYSession.Current.RoleCode == 22 || PMGSYSession.Current.RoleCode == 25)) 
                        //? //(dbContext.MASTER_EXISTING_ROADS.Where(m=>m.MAST_ER_ROAD_CODE == item.MAST_ER_ROAD_CODE && m.MAST_ER_ROAD_CODE_PMGSY1 != null).Any() 
                        //((item.MAST_ER_ROAD_CODE_PMGSY1 != 0 && (item.MAST_ROAD_CAT_CODE_PMGSY2 != item.MAST_ROAD_CAT_CODE_PMGSY1)) ? dbContext.MASTER_EXISTING_ROADS.Where(m=>m.MAST_ER_ROAD_CODE == item.MAST_ER_ROAD_CODE_PMGSY1).Select(m=>m.MAST_ER_ROAD_NUMBER).FirstOrDefault().ToString() : item.MAST_ER_ROAD_CODE_PMGSY1 != 0
                        //    ? "<a href='#' onClick =MapDRRPPMGSY1('"+URLEncrypt.EncryptParameters1(new string[]{"BlockCode =" + blockCode.ToString().Trim(), "RoadCode =" + item.MAST_ER_ROAD_CODE.ToString().Trim(), "RoadCategory=" + item.MAST_ROAD_CAT_CODE_PMGSY2.ToString().Trim()})+"'); return false;'>"+ dbContext.MASTER_EXISTING_ROADS.Where(m=>m.MAST_ER_ROAD_CODE == item.MAST_ER_ROAD_CODE_PMGSY1).Select(m=>m.MAST_ER_ROAD_NUMBER).FirstOrDefault().ToString() +"</a>" 
                        //    : "<a href='#' title='Click here to Map DRRP PMGSY - I Road' class='ui-icon ui-icon-plusthick ui-align-center' onClick =MapDRRPPMGSY1('"+URLEncrypt.EncryptParameters1(new string[]{"BlockCode =" + blockCode.ToString().Trim(), "RoadCode =" + item.MAST_ER_ROAD_CODE.ToString().Trim(), "RoadCategory=" + item.MAST_ROAD_CAT_CODE_PMGSY2.ToString().Trim()})+"'); return false;'></a>") 
                        //: "-",
                        
                        ((PMGSYSession.Current.PMGSYScheme == 2 && (PMGSYSession.Current.RoleCode == 22 || PMGSYSession.Current.RoleCode == 25))
                        ? (item.MAST_ER_ROAD_CODE_PMGSY1 == 0)
                            ? "<a href='#' title='Click here to Map DRRP PMGSY - I Road' class='ui-icon ui-icon-plusthick ui-align-center' onClick =MapDRRPPMGSY1('"+URLEncrypt.EncryptParameters1(new string[]{"BlockCode =" + blockCode.ToString().Trim(), "RoadCode =" + item.MAST_ER_ROAD_CODE.ToString().Trim(), "RoadCategory=" + item.MAST_ROAD_CAT_CODE_PMGSY2.ToString().Trim()})+"'); return false;'></a>"
                            : //(dbContext.MASTER_EXISTING_ROADS.Where(m=>m.MAST_ER_ROAD_CODE == item.MAST_ER_ROAD_CODE_PMGSY1).Select(m=>m.MAST_ER_ROAD_NUMBER).FirstOrDefault())
                              "<a href='#' onClick =MapDRRPPMGSY1('"+URLEncrypt.EncryptParameters1(new string[]{"BlockCode =" + blockCode.ToString().Trim(), "RoadCode =" + item.MAST_ER_ROAD_CODE.ToString().Trim(), "RoadCategory=" + item.MAST_ROAD_CAT_CODE_PMGSY2.ToString().Trim()})+"'); return false;'>"+ dbContext.MASTER_EXISTING_ROADS.Where(m=>m.MAST_ER_ROAD_CODE == item.MAST_ER_ROAD_CODE_PMGSY1).Select(m=>m.MAST_ER_ROAD_NUMBER).FirstOrDefault() +"</a>"
                        : "-"),

                        //dbContext.MASTER_EXISTING_ROADS.Where(m=>m.MAST_ER_ROAD_CODE == item.MAST_ER_ROAD_CODE).Select(m=>m.MAST_LOCK_STATUS).FirstOrDefault() == "N"?"<a href='#' title='Click here to edit details' class='ui-icon ui-icon-pencil ui-align-center' onClick =EditDetails('"+URLEncrypt.EncryptParameters1(new string[]{"RoadCode =" + item.MAST_ER_ROAD_CODE.ToString().Trim()})+"'); return false;'></a>":item.UNLOCK_BY_MORD == "M"?"<a href='#' title='Click here to edit details' class='ui-icon ui-icon-unlocked ui-align-center' onClick =EditDetails('"+URLEncrypt.EncryptParameters1(new string[]{"RoadCode =" + item.MAST_ER_ROAD_CODE.ToString().Trim(),"UnlockFlag =" + item.UNLOCK_BY_MORD.ToString().Trim()})+"'); return false;'></a>":item.UNLOCK_BY_MORD.ToString().ToUpper()=="Y"?"<span class='ui-icon ui-icon-locked ui-align-center'></span>":"<a href='#' title='Click here to edit details' class='ui-icon ui-icon-pencil ui-align-center' onClick =EditDetails('"+URLEncrypt.EncryptParameters1(new string[]{"RoadCode =" + item.MAST_ER_ROAD_CODE.ToString().Trim()})+"'); return false;'></a>",   
                        ///Changes by SAMMED A. PATIL on 03 OCTOBER 2017 to edit DRRP for MORD
                        PMGSYSession.Current.RoleCode == 25
                            ? "<a href='#' title='Click here to edit details' class='ui-icon ui-icon-pencil ui-align-center' onClick =EditDetails('"+URLEncrypt.EncryptParameters1(new string[]{"RoadCode =" + item.MAST_ER_ROAD_CODE.ToString().Trim()})+"'); return false;'></a>"
                            : (dbContext.MASTER_EXISTING_ROADS.Where(m=>m.MAST_ER_ROAD_CODE == item.MAST_ER_ROAD_CODE).Select(m=>m.MAST_LOCK_STATUS).FirstOrDefault() == "N"?"<a href='#' title='Click here to edit details' class='ui-icon ui-icon-pencil ui-align-center' onClick =EditDetails('"+URLEncrypt.EncryptParameters1(new string[]{"RoadCode =" + item.MAST_ER_ROAD_CODE.ToString().Trim()})+"'); return false;'></a>":item.UNLOCK_BY_MORD == "M"?"<a href='#' title='Click here to edit details' class='ui-icon ui-icon-unlocked ui-align-center' onClick =EditDetails('"+URLEncrypt.EncryptParameters1(new string[]{"RoadCode =" + item.MAST_ER_ROAD_CODE.ToString().Trim(),"UnlockFlag =" + item.UNLOCK_BY_MORD.ToString().Trim()})+"'); return false;'></a>":item.UNLOCK_BY_MORD.ToString().ToUpper()=="Y"?"<span class='ui-icon ui-icon-locked ui-align-center'></span>":"<a href='#' title='Click here to edit details' class='ui-icon ui-icon-pencil ui-align-center' onClick =EditDetails('"+URLEncrypt.EncryptParameters1(new string[]{"RoadCode =" + item.MAST_ER_ROAD_CODE.ToString().Trim()})+"'); return false;'></a>"),

                        //PMGSYSession.Current.PMGSYScheme == 2 
                        //?
                        //    (dbContext.MASTER_EXISTING_ROADS.Where(m=>m.MAST_ER_ROAD_CODE == item.MAST_ER_ROAD_CODE).Select(m=>m.MAST_LOCK_STATUS).FirstOrDefault() == "N" || item.UNLOCK_BY_MORD == "M")
                        //        ? ("<a href='#' title='Click here to delete details' class='ui-icon ui-icon-trash ui-align-center' onClick =DeleteDetails('"+URLEncrypt.EncryptParameters1(new string[]{"RoadCode =" + item.MAST_ER_ROAD_CODE.ToString().Trim                                   ()})+"'); return false;'></a>")
                        //        : "<span class='ui-icon ui-icon-locked ui-align-center'></span>"
                               
                        //: dbContext.MASTER_EXISTING_ROADS.Where(m=>m.MAST_ER_ROAD_CODE == item.MAST_ER_ROAD_CODE).Select(m=>m.MAST_LOCK_STATUS).FirstOrDefault() == "N"?"<a href='#' title='Click here to delete details' class='ui-icon ui-icon-trash ui-align-center' onClick =DeleteDetails('"+URLEncrypt.EncryptParameters1(new string[]{"RoadCode =" + item.MAST_ER_ROAD_CODE.ToString().Trim()})+"'); return false;'></a>":item.UNLOCK_BY_MORD == "M"?"<a href='#' title='Click here to delete details' class='ui-icon ui-icon-trash ui-align-center' onClick =DeleteDetails('"+URLEncrypt.EncryptParameters1(new string[]{"RoadCode =" + item.MAST_ER_ROAD_CODE.ToString().Trim()})+"'); return false;'></a>":item.UNLOCK_BY_MORD.ToString().ToUpper()=="Y"?"<span class='ui-icon ui-icon-locked ui-align-center'></span>":"<a href='#' title='Click here to delete details' class='ui-icon ui-icon-trash ui-align-center' onClick =DeleteDetails('"+URLEncrypt.EncryptParameters1(new string[]{"RoadCode =" + item.MAST_ER_ROAD_CODE.ToString().Trim()})+"'); return false;'></a>",   

                        ///Delete
                        /*dbContext.MASTER_EXISTING_ROADS.Where(m=>m.MAST_ER_ROAD_CODE == item.MAST_ER_ROAD_CODE).Select(m=>m.MAST_LOCK_STATUS).FirstOrDefault() == "N"
                            ? "<a href='#' title='Click here to delete details' class='ui-icon ui-icon-trash ui-align-center' onClick =DeleteDetails('"+URLEncrypt.EncryptParameters1(new string[]{"RoadCode =" + item.MAST_ER_ROAD_CODE.ToString().Trim()})                                +"'); return false;'></a>"
                            : "<span class='ui-icon ui-icon-locked ui-align-center'></span>",*/
                        (dbContext.MASTER_EXISTING_ROADS.Where(m=>m.MAST_ER_ROAD_CODE == item.MAST_ER_ROAD_CODE).Select(m=>m.MAST_LOCK_STATUS).FirstOrDefault() == "N" || item.UNLOCK_BY_MORD == "M")
                            ? "<a href='#' title='Click here to delete details' class='ui-icon ui-icon-trash ui-align-center' onClick =DeleteDetails('"+URLEncrypt.EncryptParameters1(new string[]{"RoadCode =" + item.MAST_ER_ROAD_CODE.ToString().Trim()})                                +"'); return false;'></a>"
                            : "<span class='ui-icon ui-icon-locked ui-align-center'></span>"
                    }
                }).ToArray();
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "ListExistingRoadsPMGSY3DAL()");
                totalRecords = 0;
                return null;
            }
            finally
            {
                dbContext.Dispose();
            }
        }

        public string GetExistingRoadCheckPMGSY3DAL(int MAST_ER_ROAD_CODE)
        {
            try
            {
                dbContext = new PMGSYEntities();

                if (dbContext.MASTER_EXISTING_ROADS.Where(m => m.MAST_ER_ROAD_CODE == MAST_ER_ROAD_CODE).Select(m => m.MAST_IS_BENEFITTED_HAB).First() == "Y")
                {
                    if (!dbContext.MASTER_ER_HABITATION_ROAD_PMGSY3.Where(a => a.MAST_ER_ROAD_CODE == MAST_ER_ROAD_CODE).Any())
                    {
                        return "Habitation Details are not added against Existing Road, Please Add Habitation Details.";
                    }
                }


                // For PMGSY III , do not appy below validations for finalizing DRRP

                if (PMGSYSession.Current.PMGSYScheme != 4)
                {


                    // check for Traffic Intensity 
                    if (!dbContext.MASTER_ER_TRAFFIC_INTENSITY_PMGSY3.Where(m => m.MAST_ER_ROAD_CODE == MAST_ER_ROAD_CODE).Any())
                    {
                        return "Traffic Intensity Details are not added against Existing Road, Please Add Traffic Intensity Details.";
                    }

                    //Check For CBR Details
                    decimal EnteredSegmentLength = 0;

                    decimal TotalLengthOfRoad = dbContext.MASTER_EXISTING_ROADS.Where(m => m.MAST_ER_ROAD_CODE == MAST_ER_ROAD_CODE).Select(m => m.MAST_ER_ROAD_END_CHAIN - m.MAST_ER_ROAD_STR_CHAIN).First();


                    if (dbContext.MASTER_ER_CBR_VALUE_PMGSY3.Where(m => m.MAST_ER_ROAD_CODE == MAST_ER_ROAD_CODE).Any())
                    {

                        EnteredSegmentLength = dbContext.MASTER_ER_CBR_VALUE_PMGSY3.Where(m => m.MAST_ER_ROAD_CODE == MAST_ER_ROAD_CODE).AsEnumerable().Sum(m => m.MAST_END_CHAIN - m.MAST_STR_CHAIN);
                        if (EnteredSegmentLength < TotalLengthOfRoad)
                        {
                            return " Please Enter Complete CBR Details.\n Total Entered Segment Length in CBR Details : " + EnteredSegmentLength + " Kms.\n Total Length Of Road : " + TotalLengthOfRoad + " Kms.";
                        }
                    }
                    else
                    {
                        return "CBR Details are not added against Existing Road, Please Add CBR Details.";
                    }


                    //Check For Surface Type Details
                    decimal EnteredSurfaceLength = 0;

                    if (dbContext.MASTER_ER_SURFACE_TYPES_PMGSY3.Where(m => m.MAST_ER_ROAD_CODE == MAST_ER_ROAD_CODE).Any())
                    {
                        EnteredSurfaceLength = Convert.ToDecimal(dbContext.MASTER_ER_SURFACE_TYPES_PMGSY3.Where(m => m.MAST_ER_ROAD_CODE == MAST_ER_ROAD_CODE).AsEnumerable().Sum(m => m.MAST_ER_SURFACE_LENGTH));

                        if (EnteredSurfaceLength < TotalLengthOfRoad)
                        {
                            return " Please Enter Complete Surface Type Details.\n Total entered Surface Length In Surface Details : " + EnteredSurfaceLength + " Kms.\nTotal Length of Road : " + TotalLengthOfRoad + " Kms.";
                        }
                    }
                    else
                    {
                        return "Surface Type Details are not added against Existing Road, Please Add Surface Type Details.";
                    }

                }







                return String.Empty;





            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "GetExistingRoadCheckDAL()");
                return "An error occured while proccessing your request";
            }
            finally
            {
                dbContext.Dispose();
            }
        }

        ///CD Works
        public Array GetCdWorkListPMGSY3DAL(int? page, int? rows, string sidx, string sord, out Int32 totalRecords, int roadCode)
        {
            try
            {
                dbContext = new PMGSYEntities();
                var cdWorksDetails = (from cdWorks in dbContext.MASTER_ER_CDWORKS_ROAD_PMGSY3
                                      join cdWorkTypeConstruction in dbContext.MASTER_CDWORKS_TYPE_CONSTRUCTION
                                     on cdWorks.MAST_CDWORKS_CODE equals cdWorkTypeConstruction.MAST_CDWORKS_CODE
                                      where cdWorks.MAST_ER_ROAD_CODE == roadCode
                                      select new
                                      {
                                          cdWorks.MAST_ER_ROAD_CODE,
                                          cdWorks.MAST_CD_NUMBER,
                                          cdWorkTypeConstruction.MAST_CDWORKS_NAME,
                                          cdWorks.MAST_CD_LENGTH,
                                          cdWorks.MAST_CD_DISCHARGE,
                                          cdWorks.MAST_CD_CHAINAGE,
                                          cdWorks.MAST_CONSTRUCT_YEAR,
                                          cdWorks.MAST_REHAB_YEAR,
                                          cdWorks.MAST_ER_SPAN,
                                          cdWorks.MAST_CARRIAGE_WAY,
                                          cdWorks.MAST_IS_FOOTPATH,
                                      }
                          ).ToList();

                totalRecords = cdWorksDetails.Count();

                if (sidx.Trim() != string.Empty)
                {
                    if (sord.ToString() == "asc")
                    {
                        switch (sidx)
                        {
                            case "CDWorksNumber":
                                cdWorksDetails = cdWorksDetails.OrderBy(x => x.MAST_CD_NUMBER).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                            case "CDWorksType":
                                cdWorksDetails = cdWorksDetails.OrderBy(x => x.MAST_CDWORKS_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                            case "CDWorksLength":
                                cdWorksDetails = cdWorksDetails.OrderBy(x => x.MAST_CD_LENGTH).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                            case "CDWorksDischarge":
                                cdWorksDetails = cdWorksDetails.OrderBy(x => x.MAST_CD_DISCHARGE).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;

                            case "CDWorksChainage":
                                cdWorksDetails = cdWorksDetails.OrderBy(x => x.MAST_CD_CHAINAGE).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;


                            case "ConstructionYear":
                                cdWorksDetails = cdWorksDetails.OrderBy(x => x.MAST_CONSTRUCT_YEAR).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;

                            case "RehabilitationYear":
                                cdWorksDetails = cdWorksDetails.OrderBy(x => x.MAST_REHAB_YEAR).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;

                            case "Span":
                                cdWorksDetails = cdWorksDetails.OrderBy(x => x.MAST_ER_SPAN).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;

                            case "CarriageWay":
                                cdWorksDetails = cdWorksDetails.OrderBy(x => x.MAST_CARRIAGE_WAY).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                            case "FootPath":
                                cdWorksDetails = cdWorksDetails.OrderBy(x => x.MAST_IS_FOOTPATH).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;

                        }
                    }
                    else
                    {
                        switch (sidx)
                        {
                            case "CDWorksNumber":
                                cdWorksDetails = cdWorksDetails.OrderByDescending(x => x.MAST_CD_NUMBER).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                            case "CDWorksType":
                                cdWorksDetails = cdWorksDetails.OrderByDescending(x => x.MAST_CDWORKS_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                            case "CDWorksLength":
                                cdWorksDetails = cdWorksDetails.OrderByDescending(x => x.MAST_CD_LENGTH).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                            case "CDWorksDischarge":
                                cdWorksDetails = cdWorksDetails.OrderByDescending(x => x.MAST_CD_DISCHARGE).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;

                            case "CDWorksChainage":
                                cdWorksDetails = cdWorksDetails.OrderByDescending(x => x.MAST_CD_CHAINAGE).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;

                            case "ConstructionYear":
                                cdWorksDetails = cdWorksDetails.OrderByDescending(x => x.MAST_CONSTRUCT_YEAR).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;

                            case "RehabilitationYear":
                                cdWorksDetails = cdWorksDetails.OrderByDescending(x => x.MAST_REHAB_YEAR).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;

                            case "Span":
                                cdWorksDetails = cdWorksDetails.OrderByDescending(x => x.MAST_ER_SPAN).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;

                            case "CarriageWay":
                                cdWorksDetails = cdWorksDetails.OrderByDescending(x => x.MAST_CARRIAGE_WAY).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                            case "FootPath":
                                cdWorksDetails = cdWorksDetails.OrderByDescending(x => x.MAST_IS_FOOTPATH).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;

                        }
                    }
                }
                else
                {
                    cdWorksDetails = cdWorksDetails.OrderBy(x => x.MAST_CD_NUMBER).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                }


                return cdWorksDetails.Select(cdWorksRoadDetails => new
                {
                    cell = new[] {         
                        cdWorksRoadDetails.MAST_CDWORKS_NAME,
                        cdWorksRoadDetails.MAST_CD_LENGTH==null?"-":cdWorksRoadDetails.MAST_CD_LENGTH.ToString(),
                        cdWorksRoadDetails.MAST_CD_DISCHARGE==null?"-":cdWorksRoadDetails.MAST_CD_DISCHARGE.ToString(),
                        cdWorksRoadDetails.MAST_CD_CHAINAGE==null?"-":cdWorksRoadDetails.MAST_CD_CHAINAGE.ToString(),
                        cdWorksRoadDetails.MAST_CONSTRUCT_YEAR==null?"-":cdWorksRoadDetails.MAST_CONSTRUCT_YEAR.ToString(),
                        cdWorksRoadDetails.MAST_REHAB_YEAR==null?"-":cdWorksRoadDetails.MAST_REHAB_YEAR.ToString(),
                        cdWorksRoadDetails.MAST_ER_SPAN==null?"-":cdWorksRoadDetails.MAST_ER_SPAN.ToString(),
                        cdWorksRoadDetails.MAST_CARRIAGE_WAY==null?"-":cdWorksRoadDetails.MAST_CARRIAGE_WAY.ToString(),
                        cdWorksRoadDetails.MAST_IS_FOOTPATH.ToString().ToUpper().Trim()=="Y"?"Yes":"No",

                                    URLEncrypt.EncryptParameters1(new string[]{"MAST_ER_ROAD_CODE="+cdWorksRoadDetails.MAST_ER_ROAD_CODE.ToString().Trim(),"MAST_CD_NUMBER="+cdWorksRoadDetails.MAST_CD_NUMBER.ToString().Trim()}),

                                    URLEncrypt.EncryptParameters1(new string[]{"MAST_ER_ROAD_CODE="+cdWorksRoadDetails.MAST_ER_ROAD_CODE.ToString().Trim(),"MAST_CD_NUMBER="+cdWorksRoadDetails.MAST_CD_NUMBER.ToString().Trim()})


                    }
                }).ToArray();

            }
            catch (Exception ex)
            {
                //Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                ErrorLog.LogError(ex, "GetCdWorkListPMGSY3DAL()");
                totalRecords = 0;
                return null;
            }
            finally
            {
                dbContext.Dispose();
            }
        }

        public bool AddCDWorksDetailsPMGSY3DAL(CdWorksViewModel cdWorksViewModel, ref string message)
        {
            using (TransactionScope objTransactionScope = new TransactionScope())
            {
                try
                {
                    dbContext = new PMGSYEntities();

                    bool flagCdWorksAddEdit = true;
                    MASTER_ER_CDWORKS_ROAD_PMGSY3 CdWorksModel = new MASTER_ER_CDWORKS_ROAD_PMGSY3(); //CloneCDWorkModel(cdWorksViewModel, flagCdWorksAddEdit);

                    //CdWorksModel.MAST_CD_NUMBER = (GetMaxCdWorksCode(cdWorksViewModel.MAST_ER_ROAD_CODE)) + 1;

                    CdWorksModel.MAST_CD_NUMBER = (dbContext.MASTER_ER_CDWORKS_ROAD_PMGSY3.Any(c => c.MAST_ER_ROAD_CODE == cdWorksViewModel.MAST_ER_ROAD_CODE) ? dbContext.MASTER_ER_CDWORKS_ROAD_PMGSY3.Where(c => c.MAST_ER_ROAD_CODE == cdWorksViewModel.MAST_ER_ROAD_CODE).Max(x => x.MAST_CD_NUMBER) : 0) + 1;


                    CdWorksModel.MAST_ER_ROAD_CODE = cdWorksViewModel.MAST_ER_ROAD_CODE;
                    CdWorksModel.MAST_CDWORKS_CODE = cdWorksViewModel.MAST_CDWORKS_CODE;
                    CdWorksModel.MAST_CD_LENGTH = cdWorksViewModel.MAST_CD_LENGTH;
                    CdWorksModel.MAST_CD_DISCHARGE = cdWorksViewModel.MAST_CD_DISCHARGE;
                    CdWorksModel.MAST_CD_CHAINAGE = cdWorksViewModel.MAST_CD_CHAINAGE;
                    CdWorksModel.MAST_CONSTRUCT_YEAR = cdWorksViewModel.MAST_CONSTRUCT_YEAR;
                    CdWorksModel.MAST_REHAB_YEAR = cdWorksViewModel.MAST_REHAB_YEAR;
                    CdWorksModel.MAST_ER_SPAN = cdWorksViewModel.MAST_ER_SPAN;
                    CdWorksModel.MAST_CARRIAGE_WAY = cdWorksViewModel.MAST_CARRIAGE_WAY;
                    CdWorksModel.MAST_IS_FOOTPATH = cdWorksViewModel.MAST_IS_FOOTPATH;


                    CdWorksModel.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                    CdWorksModel.USERID = PMGSYSession.Current.UserId;
                    dbContext.MASTER_ER_CDWORKS_ROAD_PMGSY3.Add(CdWorksModel);
                    dbContext.SaveChanges();

                    MASTER_EXISTING_ROADS existingRoadsModel = dbContext.MASTER_EXISTING_ROADS.Where(m => m.MAST_ER_ROAD_CODE == cdWorksViewModel.MAST_ER_ROAD_CODE).FirstOrDefault();

                    existingRoadsModel.MAST_CD_WORKS_NUM = existingRoadsModel.MAST_CD_WORKS_NUM + 1;
                    existingRoadsModel.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                    existingRoadsModel.USERID = PMGSYSession.Current.UserId;
                    dbContext.Entry(existingRoadsModel).State = System.Data.Entity.EntityState.Modified;
                    dbContext.SaveChanges();
                    objTransactionScope.Complete();
                    return true;
                }
                catch (DbEntityValidationException e)
                {
                    //Elmah.ErrorSignal.FromCurrentContext().Raise(e, HttpContext.Current);
                    ErrorLog.LogError(e, "AddCDWorksDetailsPMGSY3DAL().DbEntityValidationException");
                    ModelStateDictionary modelstate = new ModelStateDictionary();

                    foreach (var eve in e.EntityValidationErrors)
                    {
                        foreach (var ve in eve.ValidationErrors)
                        {
                            modelstate.AddModelError(ve.ErrorMessage, ve.ErrorMessage);
                        }
                    }
                    ErrorLog.LogError(e, "AddCDWorksDetailsPMGSY3DAL().DbEntityValidationException()");
                    using (System.IO.StreamWriter sw = System.IO.File.AppendText(System.IO.Path.Combine("D:\\ErrorLog\\OMMASErrorLog_" + System.DateTime.Now.Date.ToString("dd_MM_yyyy") + ".txt", "")))
                    {
                        sw.WriteLine("Date :" + DateTime.Now.ToString());
                        sw.WriteLine("AddCDWorksDetailsPMGSY3DAL().DbEntityValidationException() : " + "Application_Error()");

                        sw.WriteLine("Exception : " + new CommonFunctions().FormatErrorMessage(modelstate));
                        sw.WriteLine("---------------------------------------------------------------------------------------");
                        sw.Close();
                    }

                    //return new CommonFunctions().FormatErrorMessage(modelstate);
                    return false;
                }
                catch (OptimisticConcurrencyException ex)
                {
                    objTransactionScope.Dispose();
                    //Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                    ErrorLog.LogError(ex, "AddCDWorksDetailsPMGSY3DAL().OptimisticConcurrencyException");
                    message = "An Error Occurred While Processing Your Request.";
                    return false;
                }
                catch (UpdateException ex)
                {
                    objTransactionScope.Dispose();
                    //Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                    ErrorLog.LogError(ex, "AddCDWorksDetailsPMGSY3DAL().UpdateException");
                    message = "An Error Occurred While Processing Your Request.";
                    return false;
                }
                catch (Exception ex)
                {
                    objTransactionScope.Dispose();
                    //Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                    ErrorLog.LogError(ex, "AddCDWorksDetailsPMGSY3DAL()");
                    message = "An Error Occurred While Processing Your Request.";
                    return false;
                }
                finally
                {
                    dbContext.Dispose();
                }
            }
        }

        public CdWorksViewModel GetCDWorksDetailsPMGSY3DAL(int MAST_ER_ROAD_CODE, int MAST_CD_NUMBER)
        {
            try
            {
                dbContext = new PMGSYEntities();
                MASTER_ER_CDWORKS_ROAD_PMGSY3 CdWorksModel = dbContext.MASTER_ER_CDWORKS_ROAD_PMGSY3.FirstOrDefault(m => m.MAST_ER_ROAD_CODE == MAST_ER_ROAD_CODE && m.MAST_CD_NUMBER == MAST_CD_NUMBER);

                CdWorksViewModel cdWorksViewModel = new CdWorksViewModel();
                if (CdWorksModel != null)
                {
                    //MASTER_ER_CDWORKS_ROAD_PMGSY3 CdWorksModel = dbContext.MASTER_ER_CDWORKS_ROAD_PMGSY3.Where(x => x.MAST_ER_ROAD_CODE == cdWorksViewModel.MAST_ER_ROAD_CODE && x.MAST_CD_NUMBER == cdWorksViewModel.MAST_CD_NUMBER).FirstOrDefault();

                    cdWorksViewModel.MAST_ER_ROAD_CODE = CdWorksModel.MAST_ER_ROAD_CODE;
                    cdWorksViewModel.MAST_CD_NUMBER = CdWorksModel.MAST_CD_NUMBER;

                    cdWorksViewModel.MAST_CDWORKS_CODE = CdWorksModel.MAST_CDWORKS_CODE;
                    cdWorksViewModel.MAST_CD_LENGTH = CdWorksModel.MAST_CD_LENGTH;
                    cdWorksViewModel.MAST_CD_DISCHARGE = CdWorksModel.MAST_CD_DISCHARGE;
                    cdWorksViewModel.MAST_CD_CHAINAGE = CdWorksModel.MAST_CD_CHAINAGE;
                    cdWorksViewModel.MAST_CONSTRUCT_YEAR = CdWorksModel.MAST_CONSTRUCT_YEAR;
                    cdWorksViewModel.MAST_REHAB_YEAR = CdWorksModel.MAST_REHAB_YEAR;
                    cdWorksViewModel.MAST_ER_SPAN = CdWorksModel.MAST_ER_SPAN;
                    cdWorksViewModel.MAST_CARRIAGE_WAY = CdWorksModel.MAST_CARRIAGE_WAY;
                    cdWorksViewModel.MAST_IS_FOOTPATH = CdWorksModel.MAST_IS_FOOTPATH;
                }
                return cdWorksViewModel;
            }
            catch (Exception ex)
            {
                //Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                ErrorLog.LogError(ex, "GetCDWorksDetailsPMGSY3DAL()");
                return null;
            }
            finally
            {
                dbContext.Dispose();
            }
        }

        public bool EditCDWorksDetailsPMGSY3DAL(CdWorksViewModel cdWorksViewModel, ref string message)
        {
            try
            {
                dbContext = new PMGSYEntities();
                bool flagCdWorksAddEdit = false;

                MASTER_ER_CDWORKS_ROAD_PMGSY3 CdWorksModel = dbContext.MASTER_ER_CDWORKS_ROAD_PMGSY3.Where(x => x.MAST_ER_ROAD_CODE == cdWorksViewModel.MAST_ER_ROAD_CODE && x.MAST_CD_NUMBER == cdWorksViewModel.MAST_CD_NUMBER).FirstOrDefault();

                CdWorksModel.MAST_CDWORKS_CODE = cdWorksViewModel.MAST_CDWORKS_CODE;
                CdWorksModel.MAST_CD_LENGTH = cdWorksViewModel.MAST_CD_LENGTH;
                CdWorksModel.MAST_CD_DISCHARGE = cdWorksViewModel.MAST_CD_DISCHARGE;
                CdWorksModel.MAST_CD_CHAINAGE = cdWorksViewModel.MAST_CD_CHAINAGE;
                CdWorksModel.MAST_CONSTRUCT_YEAR = cdWorksViewModel.MAST_CONSTRUCT_YEAR;
                CdWorksModel.MAST_REHAB_YEAR = cdWorksViewModel.MAST_REHAB_YEAR;
                CdWorksModel.MAST_ER_SPAN = cdWorksViewModel.MAST_ER_SPAN;
                CdWorksModel.MAST_CARRIAGE_WAY = cdWorksViewModel.MAST_CARRIAGE_WAY;
                CdWorksModel.MAST_IS_FOOTPATH = cdWorksViewModel.MAST_IS_FOOTPATH;

                CdWorksModel.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                CdWorksModel.USERID = PMGSYSession.Current.UserId;
                dbContext.Entry(CdWorksModel).State = System.Data.Entity.EntityState.Modified;
                dbContext.SaveChanges();
                return true;
            }
            catch (DbEntityValidationException e)
            {
                //Elmah.ErrorSignal.FromCurrentContext().Raise(e, HttpContext.Current);
                ErrorLog.LogError(e, "EditCDWorksDetailsPMGSY3DAL().DbEntityValidationException");
                ModelStateDictionary modelstate = new ModelStateDictionary();

                foreach (var eve in e.EntityValidationErrors)
                {
                    foreach (var ve in eve.ValidationErrors)
                    {
                        modelstate.AddModelError(ve.ErrorMessage, ve.ErrorMessage);
                    }
                }
                ErrorLog.LogError(e, "EditCDWorksDetailsPMGSY3DAL().DbEntityValidationException()");
                using (System.IO.StreamWriter sw = System.IO.File.AppendText(System.IO.Path.Combine("D:\\ErrorLog\\OMMASErrorLog_" + System.DateTime.Now.Date.ToString("dd_MM_yyyy") + ".txt", "")))
                {
                    sw.WriteLine("Date :" + DateTime.Now.ToString());
                    sw.WriteLine("EditCDWorksDetailsPMGSY3DAL().DbEntityValidationException() : " + "Application_Error()");

                    sw.WriteLine("Exception : " + new CommonFunctions().FormatErrorMessage(modelstate));
                    sw.WriteLine("---------------------------------------------------------------------------------------");
                    sw.Close();
                }

                //return new CommonFunctions().FormatErrorMessage(modelstate);
                return false;
            }
            catch (OptimisticConcurrencyException ex)
            {
                //Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                ErrorLog.LogError(ex, "EditCDWorksDetailsPMGSY3DAL().OptimisticConcurrencyException()");
                message = "An Error Occurred While Processing Your Request.";
                return false;
            }
            catch (UpdateException ex)
            {
                //Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                ErrorLog.LogError(ex, "EditCDWorksDetailsPMGSY3DAL().UpdateException()");
                message = "An Error Occurred While Processing Your Request.";
                return false;
            }
            catch (Exception ex)
            {
                //Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                ErrorLog.LogError(ex, "EditCDWorksDetailsPMGSY3DAL()");
                message = "An Error Occurred While Processing Your Request.";
                return false;
            }
            finally
            {
                dbContext.Dispose();
            }
        }

        public Boolean DeleteCDWorksDetailsPMGSY3DAL(int MAST_ER_ROAD_CODE, int MAST_CD_NUMBER, ref string message)
        {

            using (TransactionScope ts = new TransactionScope())
            {
                try
                {
                    dbContext = new PMGSYEntities();
                    MASTER_ER_CDWORKS_ROAD_PMGSY3 CdWorksModel = dbContext.MASTER_ER_CDWORKS_ROAD_PMGSY3.Where(m => m.MAST_ER_ROAD_CODE == MAST_ER_ROAD_CODE && m.MAST_CD_NUMBER == MAST_CD_NUMBER).FirstOrDefault();

                    if (CdWorksModel == null)
                    {
                        return false;
                    }

                    CdWorksModel.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                    CdWorksModel.USERID = PMGSYSession.Current.UserId;
                    dbContext.Entry(CdWorksModel).State = System.Data.Entity.EntityState.Modified;
                    dbContext.SaveChanges();



                    dbContext.MASTER_ER_CDWORKS_ROAD_PMGSY3.Remove(CdWorksModel);
                    dbContext.SaveChanges();


                    MASTER_EXISTING_ROADS existingRoadsModel = dbContext.MASTER_EXISTING_ROADS.Where(m => m.MAST_ER_ROAD_CODE == MAST_ER_ROAD_CODE).FirstOrDefault();

                    existingRoadsModel.MAST_CD_WORKS_NUM = existingRoadsModel.MAST_CD_WORKS_NUM - 1;
                    existingRoadsModel.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                    existingRoadsModel.USERID = PMGSYSession.Current.UserId;
                    dbContext.Entry(existingRoadsModel).State = System.Data.Entity.EntityState.Modified;
                    dbContext.SaveChanges();
                    ts.Complete();
                    return true;
                }
                catch (DbEntityValidationException e)
                {
                    //Elmah.ErrorSignal.FromCurrentContext().Raise(e, HttpContext.Current);
                    ErrorLog.LogError(e, "DeleteCDWorksDetailsPMGSY3DAL().DbEntityValidationException");
                    ModelStateDictionary modelstate = new ModelStateDictionary();

                    foreach (var eve in e.EntityValidationErrors)
                    {
                        foreach (var ve in eve.ValidationErrors)
                        {
                            modelstate.AddModelError(ve.ErrorMessage, ve.ErrorMessage);
                        }
                    }
                    ErrorLog.LogError(e, "DeleteCDWorksDetailsPMGSY3DAL().DbEntityValidationException()");
                    using (System.IO.StreamWriter sw = System.IO.File.AppendText(System.IO.Path.Combine("D:\\ErrorLog\\OMMASErrorLog_" + System.DateTime.Now.Date.ToString("dd_MM_yyyy") + ".txt", "")))
                    {
                        sw.WriteLine("Date :" + DateTime.Now.ToString());
                        sw.WriteLine("DeleteCDWorksDetailsPMGSY3DAL().DbEntityValidationException() : " + "Application_Error()");

                        sw.WriteLine("Exception : " + new CommonFunctions().FormatErrorMessage(modelstate));
                        sw.WriteLine("---------------------------------------------------------------------------------------");
                        sw.Close();
                    }

                    //return new CommonFunctions().FormatErrorMessage(modelstate);
                    return false;
                }
                catch (OptimisticConcurrencyException ex)
                {
                    //Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                    ErrorLog.LogError(ex, "EditCDWorksDetailsPMGSY3DAL().OptimisticConcurrencyException()");
                    message = "An Error Occurred While Processing Your Request.";
                    return false;
                }
                catch (System.Data.Entity.Infrastructure.DbUpdateException ex)
                {
                    ts.Dispose();
                    //Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                    ErrorLog.LogError(ex, "DeleteCDWorksDetailsPMGSY3DAL().DbUpdateException");
                    message = "You can not delete this CdWorks details because this details are in use.";
                    return false;
                }
                catch (Exception ex)
                {
                    ts.Dispose();
                    //Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                    ErrorLog.LogError(ex, "DeleteCDWorksDetailsPMGSY3DAL");
                    message = "An Error Occurred While Processing Your Request.";
                    return false;
                }
                finally
                {
                    dbContext.Dispose();
                }
            }
        }

        ///Surface Types
        public Array GetSurfaceListPMGSY3DAL(int? page, int? rows, string sidx, string sord, out Int32 totalRecords, int roadCode)
        {
            try
            {
                dbContext = new PMGSYEntities();

                var lstSurfaceDetails = (from surfaceTypes in dbContext.MASTER_ER_SURFACE_TYPES_PMGSY3
                                         join existingRoads in dbContext.MASTER_EXISTING_ROADS
                                       on surfaceTypes.MAST_ER_ROAD_CODE equals existingRoads.MAST_ER_ROAD_CODE
                                         join surface in dbContext.MASTER_SURFACE
                                         on surfaceTypes.MAST_SURFACE_CODE equals surface.MAST_SURFACE_CODE
                                         where existingRoads.MAST_ER_ROAD_CODE == roadCode

                                         select new
                                         {
                                             surfaceTypes.MAST_ER_ROAD_CODE,
                                             surfaceTypes.MAST_SURFACE_SEG_NO,
                                             surface.MAST_SURFACE_NAME,
                                             surfaceTypes.MAST_ER_STR_CHAIN,
                                             surfaceTypes.MAST_ER_END_CHAIN,
                                             surfaceTypes.MAST_ER_SURFACE_CONDITION,
                                             surfaceTypes.MAST_ER_SURFACE_LENGTH,
                                         }).ToList();


                totalRecords = lstSurfaceDetails.Count();
                int recCount = totalRecords;

                if (sidx.Trim() != string.Empty)
                {
                    if (sord.ToString() == "asc")
                    {
                        switch (sidx)
                        {
                            case "SurfaceName":
                                lstSurfaceDetails = lstSurfaceDetails.OrderBy(x => x.MAST_SURFACE_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                            case "StartChainage":
                                lstSurfaceDetails = lstSurfaceDetails.OrderBy(x => x.MAST_ER_STR_CHAIN).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                            case "EndChainage":
                                lstSurfaceDetails = lstSurfaceDetails.OrderBy(x => x.MAST_ER_END_CHAIN).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                            case "SurfaceCondition":
                                lstSurfaceDetails = lstSurfaceDetails.OrderBy(x => x.MAST_ER_SURFACE_CONDITION).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                            case "SurfaceLength":
                                lstSurfaceDetails = lstSurfaceDetails.OrderBy(x => x.MAST_ER_SURFACE_LENGTH).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                        }
                    }
                    else
                    {
                        switch (sidx)
                        {
                            case "SurfaceName":
                                lstSurfaceDetails = lstSurfaceDetails.OrderByDescending(x => x.MAST_SURFACE_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                            case "StartChainage":
                                lstSurfaceDetails = lstSurfaceDetails.OrderByDescending(x => x.MAST_ER_STR_CHAIN).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                            case "EndChainage":
                                lstSurfaceDetails = lstSurfaceDetails.OrderByDescending(x => x.MAST_ER_END_CHAIN).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                            case "SurfaceCondition":
                                lstSurfaceDetails = lstSurfaceDetails.OrderByDescending(x => x.MAST_ER_SURFACE_CONDITION).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                            case "SurfaceLength":
                                lstSurfaceDetails = lstSurfaceDetails.OrderByDescending(x => x.MAST_ER_SURFACE_LENGTH).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                        }
                    }
                }
                else
                {
                    lstSurfaceDetails = lstSurfaceDetails.OrderBy(x => x.MAST_ER_END_CHAIN).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                }


                return lstSurfaceDetails.Select(surfaceDetails => new
                {
                    cell = new[] {         
                                    surfaceDetails.MAST_SURFACE_NAME,
                                    surfaceDetails.MAST_ER_STR_CHAIN==null?"-": surfaceDetails.MAST_ER_STR_CHAIN.ToString(),
                                    surfaceDetails.MAST_ER_END_CHAIN==null?"-":surfaceDetails.MAST_ER_END_CHAIN.ToString(),
                                    surfaceDetails.MAST_ER_SURFACE_CONDITION.ToUpper().Trim() == "G" ? "Good" : surfaceDetails.MAST_ER_SURFACE_CONDITION.ToUpper().Trim() == "F" ? "Fair" : "Bad",
                                    surfaceDetails.MAST_ER_SURFACE_LENGTH.ToString(),

                                    URLEncrypt.EncryptParameters1(new string[]{"MAST_ER_ROAD_CODE="+surfaceDetails.MAST_ER_ROAD_CODE.ToString().Trim(),"MAST_SURFACE_SEG_NO="+surfaceDetails.MAST_SURFACE_SEG_NO.ToString().Trim()}),

                                    recCount==1?URLEncrypt.EncryptParameters1(new string[]{"MAST_ER_ROAD_CODE="+surfaceDetails.MAST_ER_ROAD_CODE.ToString().Trim(),"MAST_SURFACE_SEG_NO="+surfaceDetails.MAST_SURFACE_SEG_NO.ToString().Trim()}):recCount==surfaceDetails.MAST_SURFACE_SEG_NO?URLEncrypt.EncryptParameters1(new string[]{"MAST_ER_ROAD_CODE="+surfaceDetails.MAST_ER_ROAD_CODE.ToString().Trim(),"MAST_SURFACE_SEG_NO="+surfaceDetails.MAST_SURFACE_SEG_NO.ToString().Trim()}):String.Empty,

                    }
                }).ToArray();
            }
            catch (Exception ex)
            {
                //Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                ErrorLog.LogError(ex, "GetSurfaceListPMGSY3DAL()");
                totalRecords = 0;
                return null;
            }
            finally
            {

                dbContext.Dispose();
            }
        }

        /// <summary>
        /// SurfaceStartChainageCalculation() this action is used to calcutate start chainage of Surface
        /// </summary>
        /// <param name="MAST_ER_ROAD_CODE"></param>
        /// <returns>Returns start chainage of the surface</returns>
        //[Audit]
        public SurfaceTypeViewModel SurfaceStartChainageCalculationPMGSY3DAL(int MAST_ER_ROAD_CODE)
        {
            try
            {
                dbContext = new PMGSYEntities();

                MASTER_EXISTING_ROADS masterExistingRoads = dbContext.MASTER_EXISTING_ROADS.Find(MAST_ER_ROAD_CODE);


                SurfaceTypeViewModel SurfaceViewModel = new SurfaceTypeViewModel();
                //set last entered start chainage as first to start chainage
                MASTER_ER_SURFACE_TYPES_PMGSY3 SurfaceModel = dbContext.MASTER_ER_SURFACE_TYPES_PMGSY3.Where(a => a.MAST_ER_ROAD_CODE == MAST_ER_ROAD_CODE && a.MAST_SURFACE_SEG_NO == (dbContext.MASTER_ER_SURFACE_TYPES_PMGSY3.Where(m => m.MAST_ER_ROAD_CODE == MAST_ER_ROAD_CODE).Max(m => m.MAST_SURFACE_SEG_NO))).FirstOrDefault();

                //set start chainage
                if (SurfaceModel == null)
                {
                    SurfaceViewModel.MAST_ER_STR_CHAIN = masterExistingRoads.MAST_ER_ROAD_STR_CHAIN;
                }
                else
                {
                    SurfaceViewModel.MAST_ER_STR_CHAIN = SurfaceModel.MAST_ER_END_CHAIN == null ? 0 : SurfaceModel.MAST_ER_END_CHAIN;
                }

                return SurfaceViewModel;
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "SurfaceStartChainageCalculationPMGSY3DAL()");
                return null;
            }
            finally
            {
                dbContext.Dispose();
            }
        }

        /// <summary>
        /// save the Surface details
        /// </summary>
        /// <param name="SurfaceViewModel">contains surface details</param>
        /// <param name="message"></param>
        /// <returns></returns>
        public bool AddSurfaceDetailsPMGSY3DAL(SurfaceTypeViewModel SurfaceViewModel, ref string message)
        {
            using (TransactionScope ts = new TransactionScope())
            {
                dbContext = new PMGSYEntities();
                try
                {
                    bool flagSufaceAddEdit = true;
                    decimal? totalLength = dbContext.MASTER_ER_SURFACE_TYPES_PMGSY3.Where(m => m.MAST_ER_ROAD_CODE == SurfaceViewModel.MAST_ER_ROAD_CODE).Sum(m => (Decimal?)m.MAST_ER_SURFACE_LENGTH);
                    if (totalLength == null)
                    {
                        totalLength = 0;
                    }
                    totalLength = SurfaceViewModel.MAST_ER_SURFACE_LENGTH + totalLength;
                    MASTER_EXISTING_ROADS existMaster = dbContext.MASTER_EXISTING_ROADS.Where(m => m.MAST_ER_ROAD_CODE == SurfaceViewModel.MAST_ER_ROAD_CODE).FirstOrDefault();
                    decimal? chainageLength = existMaster.MAST_ER_ROAD_END_CHAIN - existMaster.MAST_ER_ROAD_STR_CHAIN;
                    if (chainageLength < totalLength)
                    {
                        message = "The Remaining Length is less than the entered chainage length.";
                        return false;
                    }
                    //MASTER_ER_SURFACE_TYPES_PMGSY3 SurfaceModel = CloneSurfaceModel(SurfaceViewModel, flagSufaceAddEdit);

                    MASTER_ER_SURFACE_TYPES_PMGSY3 SurfaceModel = new MASTER_ER_SURFACE_TYPES_PMGSY3();

                    SurfaceModel.MAST_SURFACE_SEG_NO = (dbContext.MASTER_ER_SURFACE_TYPES_PMGSY3.Any(m => m.MAST_ER_ROAD_CODE == SurfaceViewModel.MAST_ER_ROAD_CODE) ? dbContext.MASTER_ER_SURFACE_TYPES_PMGSY3.Where(m => m.MAST_ER_ROAD_CODE == SurfaceViewModel.MAST_ER_ROAD_CODE).Max(m => m.MAST_SURFACE_SEG_NO) : 0) + 1;

                    SurfaceModel.MAST_ER_END_CHAIN = SurfaceViewModel.MAST_ER_END_CHAIN;
                    SurfaceModel.MAST_ER_ROAD_CODE = SurfaceViewModel.MAST_ER_ROAD_CODE;
                    SurfaceModel.MAST_SURFACE_CODE = SurfaceViewModel.MAST_SURFACE_CODE;
                    SurfaceModel.MAST_ER_STR_CHAIN = SurfaceViewModel.MAST_ER_STR_CHAIN;

                    SurfaceModel.MAST_ER_SURFACE_CONDITION = SurfaceViewModel.MAST_ER_SURFACE_CONDITION;
                    SurfaceModel.MAST_ER_SURFACE_LENGTH = Convert.ToDecimal(SurfaceViewModel.MAST_ER_END_CHAIN - SurfaceViewModel.MAST_ER_STR_CHAIN);

                    dbContext = new PMGSYEntities();
                    SurfaceModel.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                    SurfaceModel.USERID = PMGSYSession.Current.UserId;
                    dbContext.MASTER_ER_SURFACE_TYPES_PMGSY3.Add(SurfaceModel);
                    dbContext.SaveChanges();
                    ts.Complete();
                    return true;
                }
                catch (DbEntityValidationException e)
                {
                    //Elmah.ErrorSignal.FromCurrentContext().Raise(e, HttpContext.Current);
                    ErrorLog.LogError(e, "AddSurfaceDetailsPMGSY3DAL().DbEntityValidationException");
                    ModelStateDictionary modelstate = new ModelStateDictionary();

                    foreach (var eve in e.EntityValidationErrors)
                    {
                        foreach (var ve in eve.ValidationErrors)
                        {
                            modelstate.AddModelError(ve.ErrorMessage, ve.ErrorMessage);
                        }
                    }
                    ErrorLog.LogError(e, "AddSurfaceDetailsPMGSY3DAL().DbEntityValidationException()");
                    using (System.IO.StreamWriter sw = System.IO.File.AppendText(System.IO.Path.Combine("D:\\ErrorLog\\OMMASErrorLog_" + System.DateTime.Now.Date.ToString("dd_MM_yyyy") + ".txt", "")))
                    {
                        sw.WriteLine("Date :" + DateTime.Now.ToString());
                        sw.WriteLine("AddSurfaceDetailsPMGSY3DAL().DbEntityValidationException() : " + "Application_Error()");

                        sw.WriteLine("Exception : " + new CommonFunctions().FormatErrorMessage(modelstate));
                        sw.WriteLine("---------------------------------------------------------------------------------------");
                        sw.Close();
                    }
                    return false;
                }
                catch (OptimisticConcurrencyException ex)
                {
                    ts.Dispose();
                    //Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                    ErrorLog.LogError(ex, "AddSurfaceDetailsPMGSY3DAL().OptimisticConcurrencyException");
                    message = "An Error Occurred While Processing Your Request.";
                    return false;
                }
                catch (UpdateException ex)
                {
                    ts.Dispose();
                    //Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                    ErrorLog.LogError(ex, "AddSurfaceDetailsPMGSY3DAL().UpdateException");
                    message = "An Error Occurred While Processing Your Request.";
                    return false;
                }
                catch (Exception ex)
                {
                    ts.Dispose();
                    //Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                    ErrorLog.LogError(ex, "AddSurfaceDetailsPMGSY3DAL()");
                    message = "An Error Occurred While Processing Your Request.";
                    return false;
                }
                finally
                {
                    dbContext.Dispose();
                }
            }
        }

        public SurfaceTypeViewModel GetSurfaceDetailsPMGSY3(int MAST_ER_ROAD_CODE, int MAST_SURFACE_SEG_NO)
        {
            try
            {
                dbContext = new PMGSYEntities();
                MASTER_ER_SURFACE_TYPES_PMGSY3 SurfaceModel = dbContext.MASTER_ER_SURFACE_TYPES_PMGSY3.FirstOrDefault(m => m.MAST_ER_ROAD_CODE == MAST_ER_ROAD_CODE && m.MAST_SURFACE_SEG_NO == MAST_SURFACE_SEG_NO);

                SurfaceTypeViewModel SurfaceViewModel = new SurfaceTypeViewModel();
                if (SurfaceModel != null)
                {
                    //check updated
                    MASTER_EXISTING_ROADS planRoad = dbContext.MASTER_EXISTING_ROADS.Where(m => m.MAST_ER_ROAD_CODE == SurfaceModel.MAST_ER_ROAD_CODE).FirstOrDefault();

                    if (planRoad != null)
                    {
                        SurfaceViewModel.StartChainageOfRoad = Convert.ToDecimal(planRoad.MAST_ER_ROAD_STR_CHAIN);
                        SurfaceViewModel.EndChainageOfRoad = Convert.ToDecimal(planRoad.MAST_ER_ROAD_END_CHAIN);
                        SurfaceViewModel.SumOfAllSurfaceLength = Convert.ToDecimal(planRoad.MAST_ER_ROAD_END_CHAIN - planRoad.MAST_ER_ROAD_STR_CHAIN);
                    }

                    SurfaceViewModel.EncryptedRoadCode = SurfaceModel.MAST_ER_ROAD_CODE.ToString();

                    SurfaceViewModel.MAST_ER_ROAD_CODE = SurfaceModel.MAST_ER_ROAD_CODE;
                    SurfaceViewModel.MAST_SURFACE_SEG_NO = SurfaceModel.MAST_SURFACE_SEG_NO;
                    SurfaceViewModel.MAST_SURFACE_CODE = SurfaceModel.MAST_SURFACE_CODE;
                    SurfaceViewModel.MAST_ER_STR_CHAIN = SurfaceModel.MAST_ER_STR_CHAIN;

                    SurfaceViewModel.MAST_ER_END_CHAIN = SurfaceModel.MAST_ER_END_CHAIN;
                    SurfaceViewModel.MAST_ER_SURFACE_CONDITION = SurfaceModel.MAST_ER_SURFACE_CONDITION;
                    SurfaceViewModel.MAST_ER_SURFACE_LENGTH = Convert.ToDecimal(SurfaceModel.MAST_ER_SURFACE_LENGTH);


                    SurfaceViewModel.RoadName = SurfaceModel.MASTER_EXISTING_ROADS.MAST_ER_ROAD_NAME;
                    SurfaceViewModel.RoadNumber = SurfaceModel.MASTER_EXISTING_ROADS.MAST_ER_ROAD_NUMBER;

                    SurfaceViewModel.EditModeEndChainage = SurfaceModel.MAST_ER_END_CHAIN;

                    //remaining length
                    SurfaceViewModel.SurfaceLenghEntered = Convert.ToDecimal(dbContext.MASTER_ER_SURFACE_TYPES_PMGSY3.Where(m => m.MAST_ER_ROAD_CODE == MAST_ER_ROAD_CODE).Sum(m => m.MAST_ER_SURFACE_LENGTH));
                    SurfaceViewModel.Remaining_Length = SurfaceViewModel.SumOfAllSurfaceLength - SurfaceViewModel.SurfaceLenghEntered;
                }
                return SurfaceViewModel;
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "");
                return null;
            }
            finally
            {
                dbContext.Dispose();
            }
        }

        public bool EditSurfaceDetailsPMGSY3DAL(SurfaceTypeViewModel SurfaceViewModel, ref string message)
        {
            try
            {
                dbContext = new PMGSYEntities();
                bool flagSufaceAddEdit = false;

                //MASTER_ER_SURFACE_TYPES_PMGSY3 SurfaceModel = CloneSurfaceModel(SurfaceViewModel, flagSufaceAddEdit);

                //SurfaceModel.MAST_SURFACE_SEG_NO = SurfaceViewModel.MAST_SURFACE_SEG_NO;
                //SurfaceModel.MAST_ER_END_CHAIN = SurfaceViewModel.EditModeEndChainage;

                MASTER_ER_SURFACE_TYPES_PMGSY3 SurfaceModel = dbContext.MASTER_ER_SURFACE_TYPES_PMGSY3.FirstOrDefault(m => m.MAST_ER_ROAD_CODE == SurfaceViewModel.MAST_ER_ROAD_CODE && m.MAST_SURFACE_SEG_NO == SurfaceViewModel.MAST_SURFACE_SEG_NO);

                SurfaceModel.MAST_ER_END_CHAIN = SurfaceViewModel.MAST_ER_END_CHAIN;
                SurfaceModel.MAST_ER_ROAD_CODE = SurfaceViewModel.MAST_ER_ROAD_CODE;
                SurfaceModel.MAST_SURFACE_CODE = SurfaceViewModel.MAST_SURFACE_CODE;
                SurfaceModel.MAST_ER_STR_CHAIN = SurfaceViewModel.MAST_ER_STR_CHAIN;

                SurfaceModel.MAST_ER_SURFACE_CONDITION = SurfaceViewModel.MAST_ER_SURFACE_CONDITION;
                SurfaceModel.MAST_ER_SURFACE_LENGTH = Convert.ToDecimal(SurfaceViewModel.MAST_ER_END_CHAIN - SurfaceViewModel.MAST_ER_STR_CHAIN);

                SurfaceModel.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                SurfaceModel.USERID = PMGSYSession.Current.UserId;
                dbContext.Entry(SurfaceModel).State = System.Data.Entity.EntityState.Modified;
                dbContext.SaveChanges();
                return true;
            }
            catch (DbEntityValidationException e)
            {
                //Elmah.ErrorSignal.FromCurrentContext().Raise(e, HttpContext.Current);
                ErrorLog.LogError(e, "EditSurfaceDetailsPMGSY3DAL().DbEntityValidationException");
                ModelStateDictionary modelstate = new ModelStateDictionary();

                foreach (var eve in e.EntityValidationErrors)
                {
                    foreach (var ve in eve.ValidationErrors)
                    {
                        modelstate.AddModelError(ve.ErrorMessage, ve.ErrorMessage);
                    }
                }
                ErrorLog.LogError(e, "EditSurfaceDetailsPMGSY3DAL().DbEntityValidationException()");
                using (System.IO.StreamWriter sw = System.IO.File.AppendText(System.IO.Path.Combine("D:\\ErrorLog\\OMMASErrorLog_" + System.DateTime.Now.Date.ToString("dd_MM_yyyy") + ".txt", "")))
                {
                    sw.WriteLine("Date :" + DateTime.Now.ToString());
                    sw.WriteLine("EditSurfaceDetailsPMGSY3DAL().DbEntityValidationException() : " + "Application_Error()");

                    sw.WriteLine("Exception : " + new CommonFunctions().FormatErrorMessage(modelstate));
                    sw.WriteLine("---------------------------------------------------------------------------------------");
                    sw.Close();
                }
                return false;
            }
            catch (OptimisticConcurrencyException ex)
            {
                //Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                ErrorLog.LogError(ex, "EditSurfaceDetailsPMGSY3DAL().OptimisticConcurrencyException");
                message = "An Error Occurred While Processing Your Request.";
                return false;
            }
            catch (UpdateException ex)
            {
                //Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                ErrorLog.LogError(ex, "EditSurfaceDetailsPMGSY3DAL().UpdateException");
                message = "An Error Occurred While Processing Your Request.";
                return false;
            }
            catch (Exception ex)
            {
                //Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                ErrorLog.LogError(ex, "EditSurfaceDetailsPMGSY3DAL()");
                message = "An Error Occurred While Processing Your Request.";
                return false;
            }
            finally
            {
                dbContext.Dispose();
            }
        }

        public Boolean DeleteSurfaceDetailsPMGSY3DAL(int MAST_ER_ROAD_CODE, int MAST_SURFACE_SEG_NO, ref string message)
        {
            using (TransactionScope ts = new TransactionScope())
            {
                try
                {
                    dbContext = new PMGSYEntities();
                    MASTER_ER_SURFACE_TYPES_PMGSY3 SurfaceModel = dbContext.MASTER_ER_SURFACE_TYPES_PMGSY3.Where(m => m.MAST_ER_ROAD_CODE == MAST_ER_ROAD_CODE && m.MAST_SURFACE_SEG_NO == MAST_SURFACE_SEG_NO).FirstOrDefault();

                    if (SurfaceModel == null)
                    {
                        return false;
                    }

                    SurfaceModel.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                    SurfaceModel.USERID = PMGSYSession.Current.UserId;
                    dbContext.Entry(SurfaceModel).State = System.Data.Entity.EntityState.Modified;
                    dbContext.SaveChanges();



                    dbContext.MASTER_ER_SURFACE_TYPES_PMGSY3.Remove(SurfaceModel);
                    dbContext.SaveChanges();
                    ts.Complete();
                    return true;
                }
                catch (DbEntityValidationException e)
                {
                    //Elmah.ErrorSignal.FromCurrentContext().Raise(e, HttpContext.Current);
                    ErrorLog.LogError(e, "DeleteSurfaceDetailsPMGSY3DAL().DbEntityValidationException");
                    ModelStateDictionary modelstate = new ModelStateDictionary();

                    foreach (var eve in e.EntityValidationErrors)
                    {
                        foreach (var ve in eve.ValidationErrors)
                        {
                            modelstate.AddModelError(ve.ErrorMessage, ve.ErrorMessage);
                        }
                    }
                    ErrorLog.LogError(e, "DeleteSurfaceDetailsPMGSY3DAL().DbEntityValidationException()");
                    using (System.IO.StreamWriter sw = System.IO.File.AppendText(System.IO.Path.Combine("D:\\ErrorLog\\OMMASErrorLog_" + System.DateTime.Now.Date.ToString("dd_MM_yyyy") + ".txt", "")))
                    {
                        sw.WriteLine("Date :" + DateTime.Now.ToString());
                        sw.WriteLine("DeleteSurfaceDetailsPMGSY3DAL().DbEntityValidationException() : " + "Application_Error()");

                        sw.WriteLine("Exception : " + new CommonFunctions().FormatErrorMessage(modelstate));
                        sw.WriteLine("---------------------------------------------------------------------------------------");
                        sw.Close();
                    }

                    //return new CommonFunctions().FormatErrorMessage(modelstate);
                    return false;
                }
                catch (System.Data.Entity.Infrastructure.DbUpdateException ex)
                {
                    ts.Dispose();
                    //Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                    ErrorLog.LogError(ex, "DeleteSurfaceDetailsPMGSY3DAL().DbUpdateException");
                    message = "You can not delete this Surface details because this details are in use";
                    return false;
                }
                catch (Exception ex)
                {
                    ts.Dispose();
                    //Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                    ErrorLog.LogError(ex, "DeleteSurfaceDetailsPMGSY3DAL()");
                    message = "An Error Occurred While Processing Your Request.";
                    return false;
                }
                finally
                {
                    dbContext.Dispose();
                }
            }
        }

        ///Habitation
        public Array GetHabitationListToMapPMGSY3DAL(int roadCode, int blockCode, int page, int rows, string sidx, string sord, out long totalRecords)
        {
            totalRecords = 0;
            try
            {
                dbContext = new PMGSYEntities();
                MASTER_EXISTING_ROADS masterRoad = dbContext.MASTER_EXISTING_ROADS.Where(m => m.MAST_ER_ROAD_CODE == roadCode).FirstOrDefault();

                var lstHabitations = (from item in dbContext.MASTER_VILLAGE
                                      join habitation in dbContext.MASTER_HABITATIONS on item.MAST_VILLAGE_CODE equals habitation.MAST_VILLAGE_CODE
                                      join habitationDetails in dbContext.MASTER_HABITATIONS_DETAILS on habitation.MAST_HAB_CODE equals habitationDetails.MAST_HAB_CODE
                                      //join villagePopulation in dbContext.MASTER_VILLAGE_POPULATION on item.MAST_VILLAGE_CODE equals villagePopulation.MAST_VILLAGE_CODE
                                      where item.MAST_BLOCK_CODE == blockCode &&//masterRoad.MAST_BLOCK_CODE && commented by Vikram to allow habitations from another block in the block to map.
                                      habitation.MAST_HABITATION_ACTIVE == "Y" &&  //new condition added by Vikram and below line commented as per suggestion from Dev Sir
                                          //(habitationDetails.MAST_HAB_CONNECTED == "Y" || habitationDetails.MAST_HAB_CONNECTED == "N") &&//the habitation available for mapping may be connected or unconnected as suggested by Dev sir
                                          //(PMGSYSession.Current.PMGSYScheme == 1 ? (habitationDetails.MAST_YEAR == 2001) : (habitationDetails.MAST_YEAR == 2011))
                                      habitationDetails.MAST_YEAR == 2011
                                      select new
                                      {
                                          habitation.MAST_HAB_NAME,
                                          habitation.MAST_HAB_CODE,
                                      });

                List<int> mapHabitations = null;
                //if (PMGSYSession.Current.PMGSYScheme == 1)
                //{
                //    mapHabitations = (from item in dbContext.MASTER_ER_HABITATION_ROAD
                //                      join data in dbContext.MASTER_EXISTING_ROADS on item.MAST_ER_ROAD_CODE equals data.MAST_ER_ROAD_CODE
                //                      where data.MAST_BLOCK_CODE == masterRoad.MAST_BLOCK_CODE
                //                      && data.MAST_PMGSY_SCHEME == PMGSYSession.Current.PMGSYScheme//added by abhishek kamble 5-feb-2014                                            
                //                      select item.MAST_HAB_CODE).Distinct().ToList<int>();

                //}
                //else
                if (PMGSYSession.Current.PMGSYScheme == 4)
                {
                    mapHabitations = (from item in dbContext.MASTER_ER_HABITATION_ROAD_PMGSY3
                                      join data in dbContext.MASTER_EXISTING_ROADS on item.MAST_ER_ROAD_CODE equals data.MAST_ER_ROAD_CODE
                                      where data.MAST_BLOCK_CODE == masterRoad.MAST_BLOCK_CODE
                                      && data.MAST_PMGSY_SCHEME == 2//added by abhishek kamble 5-feb-2014                                            
                                      && item.MAST_ER_ROAD_CODE == roadCode
                                      select item.MAST_HAB_CODE).Distinct().ToList<int>();
                }

                var listHab = (from item in lstHabitations
                               where !mapHabitations.Contains(item.MAST_HAB_CODE)
                               select item.MAST_HAB_CODE).Distinct();

                var mappingList = (from item in dbContext.MASTER_HABITATIONS_DETAILS
                                   join habitation in dbContext.MASTER_HABITATIONS on item.MAST_HAB_CODE equals habitation.MAST_HAB_CODE
                                   where
                                       //(PMGSYSession.Current.PMGSYScheme == 1 ? (item.MAST_YEAR == 2001) : (item.MAST_YEAR == 2011))//added by abhishek kamble 5-feb-2014
                                       //&&
                                       //listHab.Contains(item.MAST_HAB_CODE)
                                   item.MAST_YEAR == 2011
                                   && habitation.MASTER_VILLAGE.MAST_BLOCK_CODE == blockCode
                                   && habitation.MAST_HABITATION_ACTIVE == "Y"
                                   select new
                                   {
                                       item.MAST_HAB_CODE,
                                       habitation.MAST_HAB_NAME,
                                       item.MAST_HAB_TOT_POP,
                                       habitation.MASTER_VILLAGE.MAST_VILLAGE_NAME
                                   }).Distinct();

                totalRecords = mappingList.Count();

                if (sidx.Trim() != string.Empty)
                {
                    if (sord.ToString() == "asc")
                    {
                        mappingList = mappingList.OrderBy(x => x.MAST_HAB_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                    }
                    else
                    {
                        mappingList = mappingList.OrderByDescending(x => x.MAST_HAB_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                    }
                }
                else
                {
                    mappingList = mappingList.OrderBy(x => x.MAST_HAB_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
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
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "GetHabitationListToMapPMGSY3DAL()");
                return null;
            }
            finally
            {
                dbContext.Dispose();
            }
        }

        public Array GetAllHabitationListPMGSY3DAL(int roadCode, int page, int rows, string sidx, string sord, out long totalRecords)
        {
            dbContext = new PMGSYEntities();
            try
            {

                var lstHabitations = from item in dbContext.MASTER_ER_HABITATION_ROAD_PMGSY3
                                     join habitation in dbContext.MASTER_HABITATIONS on item.MAST_HAB_CODE equals habitation.MAST_HAB_CODE
                                     join habCode in dbContext.MASTER_HABITATIONS_DETAILS on item.MAST_HAB_CODE equals habCode.MAST_HAB_CODE
                                     join roadPlan in dbContext.MASTER_EXISTING_ROADS on item.MAST_ER_ROAD_CODE equals roadPlan.MAST_ER_ROAD_CODE
                                     where item.MAST_ER_ROAD_CODE == roadCode
                                         //&& roadPlan.MAST_PMGSY_SCHEME == PMGSYSession.Current.PMGSYScheme //added by abhishek kamble 5-feb-2014
                                         //&& (PMGSYSession.Current.PMGSYScheme == 1 ? (habCode.MAST_YEAR == 2001) : (habCode.MAST_YEAR == 2011))  //added by abhishek kamble 5-feb-2014
                                     && roadPlan.MAST_PMGSY_SCHEME == 2
                                     && habCode.MAST_YEAR == 2011
                                     select new
                                     {
                                         habitation.MAST_HAB_NAME,
                                         habCode.MAST_HAB_CODE,
                                         habitation.MASTER_VILLAGE.MAST_VILLAGE_NAME,
                                         habCode.MAST_HAB_TOT_POP,
                                         MAST_HAB_CODE_DIRECT = item.MAST_HAB_CODE_DIRECT.Trim() == "Y" ? "Yes" : "No",
                                         MAST_HAB_CODE_VERIFIED = item.MAST_HAB_CODE_VERIFIED.Trim() == "Y" ? "Yes" : "No",
                                     };

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
                    habDetails.MAST_HAB_NAME,
                    habDetails.MAST_HAB_CODE,
                    habDetails.MAST_HAB_TOT_POP,
                    habDetails.MAST_VILLAGE_NAME,
                    habDetails.MAST_HAB_CODE_DIRECT,
                    habDetails.MAST_HAB_CODE_VERIFIED
                }).ToArray();

                return result.Select(habDetails => new
                {
                    cell = new[]
                    {   
                        habDetails.MAST_HAB_CODE.ToString(),
                        habDetails.MAST_HAB_NAME == null?string.Empty:habDetails.MAST_HAB_NAME.ToString(),
                        habDetails.MAST_VILLAGE_NAME == null ? string.Empty :habDetails.MAST_VILLAGE_NAME.ToString(),
                        habDetails.MAST_HAB_TOT_POP.ToString(),
                        habDetails.MAST_HAB_CODE_DIRECT.Trim(),
                        habDetails.MAST_HAB_CODE_VERIFIED.Trim(),
                        URLEncrypt.EncryptParameters1(new string[]{"HabCode =" + habDetails.MAST_HAB_CODE.ToString().Trim()}),
                    }
                }).ToArray();
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "GetAllHabitationListPMGSY3DAL()");
                totalRecords = 0;
                return null;
            }
            finally { dbContext.Dispose(); }
        }

        public bool MapHabitationToRoadPMGSY3DAL(string encryptedHabCodes, string roadName, string habDirect)
        {
            using (TransactionScope objTransactionScope = new TransactionScope())
            {
                try
                {
                    dbContext = new PMGSYEntities();
                    String[] habCodes = null;
                    int roadCode = Convert.ToInt32(roadName);
                    int habCode = 0;

                    //Added By Abhishek kamble 10-feb-2014 start
                    MASTER_EXISTING_ROADS existingRoadModel = dbContext.MASTER_EXISTING_ROADS.Where(m => m.MAST_ER_ROAD_CODE == roadCode).FirstOrDefault();

                    //Added By Abhishek kamble 10-feb-2014 end
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

                        MASTER_ER_HABITATION_ROAD_PMGSY3 master = new MASTER_ER_HABITATION_ROAD_PMGSY3();
                        if (dbContext.MASTER_ER_HABITATION_ROAD_PMGSY3.Any())
                        {
                            master.MAST_ER_ROAD_ID = (from item1 in dbContext.MASTER_ER_HABITATION_ROAD_PMGSY3 select item1.MAST_ER_ROAD_ID).Max() + 1;
                        }
                        else
                        {
                            master.MAST_ER_ROAD_ID = 1;
                        }
                        master.MAST_ER_ROAD_CODE = roadCode;
                        master.MAST_HAB_CODE = habCode;

                        master.MAST_HAB_CODE_DIRECT = habDirect.Trim();
                        master.MAST_HAB_CODE_VERIFIED = "N";

                        master.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                        master.USERID = PMGSYSession.Current.UserId;
                        dbContext.MASTER_ER_HABITATION_ROAD_PMGSY3.Add(master);
                        dbContext.SaveChanges();

                        //Added By Abhishek kamble 6-feb-2014 start
                        #region Commented
                        /*if (PMGSYSession.Current.PMGSYScheme == 4)
                        {
                            //Add Habitation For Candidate Roads start
                            var candidateRoadCodes = dbContext.PLAN_ROAD.Where(m => m.MAST_PMGSY_SCHEME == 2 && m.MAST_ER_ROAD_CODE == roadCode && m.MAST_STATE_CODE == existingRoadModel.MAST_STATE_CODE && m.MAST_DISTRICT_CODE == existingRoadModel.MAST_DISTRICT_CODE && m.MAST_BLOCK_CODE == existingRoadModel.MAST_BLOCK_CODE).Select(s => s.PLAN_CN_ROAD_CODE);
                            if (candidateRoadCodes != null)
                            {
                                //get PlanCNRoadCode
                                foreach (var planCnRoadCode in candidateRoadCodes)
                                {
                                    //Added Habitation For PlanCnRoadCode In PLAN ROAD Habitaiton
                                    int? HabId = null;
                                    PLAN_ROAD_HABITATION_PMGSY3 planRoadHabitationModel = new PLAN_ROAD_HABITATION_PMGSY3();

                                    if (!(dbContext.PLAN_ROAD_HABITATION_PMGSY3.Where(m => m.PLAN_CN_ROAD_CODE == planCnRoadCode && m.MAST_HAB_CODE == habCode).Any()))
                                    {
                                        //HabId = dbContext.PLAN_ROAD_HABITATION..Max(s => (int?)s.PLAN_CN_ROAD_HAB_ID);
                                        if (dbContext.PLAN_ROAD_HABITATION_PMGSY3.Any())
                                        {
                                            HabId = dbContext.PLAN_ROAD_HABITATION_PMGSY3.Max(s => (int?)s.PLAN_CN_ROAD_HAB_ID) + 1;
                                        }
                                        else
                                        {
                                            HabId = 1;
                                        }
                                        planRoadHabitationModel.PLAN_CN_ROAD_HAB_ID = HabId.Value;
                                        planRoadHabitationModel.PLAN_CN_ROAD_CODE = planCnRoadCode;
                                        planRoadHabitationModel.MAST_HAB_CODE = habCode;

                                        planRoadHabitationModel.MAST_HAB_CODE_DIRECT = habDirect.Trim();
                                        planRoadHabitationModel.MAST_HAB_CODE_VERIFIED = "N";

                                        planRoadHabitationModel.USERID = PMGSYSession.Current.UserId;
                                        planRoadHabitationModel.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                                        dbContext.PLAN_ROAD_HABITATION_PMGSY3.Add(planRoadHabitationModel);
                                        dbContext.SaveChanges();
                                    }
                                }
                            }
                            //Add Habitation For Candidate Roads end

                            //Add Habitation For Other Candidate Roads start
                            var otherCandidateRoadCodes = dbContext.PLAN_ROAD_DRRP.Where(m => m.MAST_ER_ROAD_CODE == roadCode).Select(s => s.PLAN_CN_ROAD_CODE);
                            if (otherCandidateRoadCodes != null)
                            {
                                //get PlanCNRoadCode
                                foreach (var planCnRoadCode in otherCandidateRoadCodes)
                                {
                                    //Added Habitation For PlanCnRoadCode In PLAN ROAD Habitaiton
                                    int? HabId = null;
                                    PLAN_ROAD_HABITATION_PMGSY3 planRoadHabitationModel = new PLAN_ROAD_HABITATION_PMGSY3();

                                    if (!(dbContext.PLAN_ROAD_HABITATION_PMGSY3.Where(m => m.PLAN_CN_ROAD_CODE == planCnRoadCode && m.MAST_HAB_CODE == habCode).Any()))
                                    {
                                        //HabId = dbContext.PLAN_ROAD_HABITATION..Max(s => (int?)s.PLAN_CN_ROAD_HAB_ID);
                                        if (dbContext.PLAN_ROAD_HABITATION_PMGSY3.Any())
                                        {
                                            HabId = dbContext.PLAN_ROAD_HABITATION_PMGSY3.Max(s => (int?)s.PLAN_CN_ROAD_HAB_ID) + 1;
                                        }
                                        else
                                        {
                                            HabId = 1;
                                        }
                                        planRoadHabitationModel.PLAN_CN_ROAD_HAB_ID = HabId.Value;
                                        planRoadHabitationModel.PLAN_CN_ROAD_CODE = planCnRoadCode;
                                        planRoadHabitationModel.MAST_HAB_CODE = habCode;

                                        planRoadHabitationModel.MAST_HAB_CODE_DIRECT = habDirect.Trim();
                                        planRoadHabitationModel.MAST_HAB_CODE_VERIFIED = "N";

                                        planRoadHabitationModel.USERID = PMGSYSession.Current.UserId;
                                        planRoadHabitationModel.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                                        dbContext.PLAN_ROAD_HABITATION_PMGSY3.Add(planRoadHabitationModel);
                                        dbContext.SaveChanges();
                                    }
                                }
                            }
                            //Add Habitation For Other Candidate Roads end
                        }*/
                        #endregion
                        //Added By Abhishek kamble 6-feb-2014 end
                    }

                    objTransactionScope.Complete();
                    return true;
                }
                catch (DbEntityValidationException e)
                {
                    ErrorLog.LogError(e, "MapHabitationToRoadPMGSY3DAL().DbEntityValidationException");
                    ModelStateDictionary modelstate = new ModelStateDictionary();

                    foreach (var eve in e.EntityValidationErrors)
                    {
                        foreach (var ve in eve.ValidationErrors)
                        {
                            modelstate.AddModelError(ve.ErrorMessage, ve.ErrorMessage);
                        }
                    }
                    ErrorLog.LogError(e, "MapHabitationToRoadPMGSY3DAL().OptimisticConcurrencyException");
                    using (System.IO.StreamWriter sw = System.IO.File.AppendText(System.IO.Path.Combine("D:\\ErrorLog\\OMMASErrorLog_" + System.DateTime.Now.Date.ToString("dd_MM_yyyy") + ".txt", "")))
                    {
                        sw.WriteLine("Date :" + DateTime.Now.ToString());
                        sw.WriteLine("DbEntityValidationException().DbEntityValidationException() : " + "Application_Error()");

                        sw.WriteLine("Exception : " + new CommonFunctions().FormatErrorMessage(modelstate));
                        sw.WriteLine("---------------------------------------------------------------------------------------");
                        sw.Close();
                    }
                    //return new CommonFunctions().FormatErrorMessage(modelstate);
                    objTransactionScope.Dispose();
                    return false;
                }
                catch (OptimisticConcurrencyException ex)
                {
                    objTransactionScope.Dispose();
                    ErrorLog.LogError(ex, "MapHabitationToRoadPMGSY3DAL().OptimisticConcurrencyException");
                    return false;
                }
                catch (UpdateException ex)
                {
                    objTransactionScope.Dispose();
                    ErrorLog.LogError(ex, "MapHabitationToRoadPMGSY3DAL().UpdateException");
                    return false;
                }
                catch (Exception ex)
                {
                    objTransactionScope.Dispose();
                    ErrorLog.LogError(ex, "MapHabitationToRoadPMGSY3DAL()");
                    return false;
                }
                finally
                {
                    objTransactionScope.Dispose();
                    dbContext.Dispose();
                }
            }
        }

        public bool DeleteMapHabitationPMGSY3DAL(int habitationCode, int roadCode, out string message)
        {
            using (TransactionScope ts = new TransactionScope())
            {
                try
                {
                    dbContext = new PMGSYEntities();

                    if (PMGSYSession.Current.PMGSYScheme == 4)
                    {
                        //Existing Road details
                        MASTER_EXISTING_ROADS ExistingRoadModel = dbContext.MASTER_EXISTING_ROADS.Where(m => m.MAST_ER_ROAD_CODE == roadCode).FirstOrDefault();

                        int cnRoadCode = dbContext.PLAN_ROAD.Where(x => x.MAST_ER_ROAD_CODE == roadCode).Select(x => x.PLAN_CN_ROAD_CODE).FirstOrDefault();

                        if (dbContext.PLAN_ROAD_HABITATION_PMGSY3.Where(m => m.MAST_HAB_CODE == habitationCode && m.PLAN_ROAD.MAST_PMGSY_SCHEME == PMGSYSession.Current.PMGSYScheme && m.PLAN_ROAD.PLAN_CN_ROAD_CODE == cnRoadCode && m.PLAN_ROAD.MAST_STATE_CODE == ExistingRoadModel.MAST_STATE_CODE && m.PLAN_ROAD.MAST_DISTRICT_CODE == ExistingRoadModel.MAST_DISTRICT_CODE && m.PLAN_ROAD.MAST_BLOCK_CODE == ExistingRoadModel.MAST_BLOCK_CODE).Any())
                        {
                            message = "Habitation can not be deleted because this habitation is mapped with other candidate road.";
                            return false;
                        }
                    }

                    //Added By Abhishek kamble 6-feb-2014 end

                    MASTER_ER_HABITATION_ROAD_PMGSY3 master = dbContext.MASTER_ER_HABITATION_ROAD_PMGSY3.Where(m => m.MAST_HAB_CODE == habitationCode && m.MAST_ER_ROAD_CODE == roadCode).FirstOrDefault();

                    master.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                    master.USERID = PMGSYSession.Current.UserId;
                    dbContext.Entry(master).State = System.Data.Entity.EntityState.Modified;
                    dbContext.SaveChanges();

                    dbContext.MASTER_ER_HABITATION_ROAD_PMGSY3.Remove(master);
                    dbContext.SaveChanges();
                    message = String.Empty;
                    ts.Complete();
                    return true;
                }
                catch (DbEntityValidationException e)
                {
                    ErrorLog.LogError(e, "MapHabitationToRoadPMGSY3DAL().DbEntityValidationException");
                    ModelStateDictionary modelstate = new ModelStateDictionary();

                    foreach (var eve in e.EntityValidationErrors)
                    {
                        foreach (var ve in eve.ValidationErrors)
                        {
                            modelstate.AddModelError(ve.ErrorMessage, ve.ErrorMessage);
                        }
                    }
                    ErrorLog.LogError(e, "MapHabitationToRoadPMGSY3DAL().OptimisticConcurrencyException");
                    using (System.IO.StreamWriter sw = System.IO.File.AppendText(System.IO.Path.Combine("D:\\ErrorLog\\OMMASErrorLog_" + System.DateTime.Now.Date.ToString("dd_MM_yyyy") + ".txt", "")))
                    {
                        sw.WriteLine("Date :" + DateTime.Now.ToString());
                        sw.WriteLine("DbEntityValidationException().DbEntityValidationException() : " + "Application_Error()");

                        sw.WriteLine("Exception : " + new CommonFunctions().FormatErrorMessage(modelstate));
                        sw.WriteLine("---------------------------------------------------------------------------------------");
                        sw.Close();
                    }
                    //return new CommonFunctions().FormatErrorMessage(modelstate);
                    message = String.Empty;
                    return false;
                }
                catch (OptimisticConcurrencyException ex)
                {
                    ts.Dispose();
                    ErrorLog.LogError(ex, "DeleteMapHabitationPMGSY3DAL().OptimisticConcurrencyException");
                    message = String.Empty;
                    return false;
                }
                catch (UpdateException ex)
                {
                    ts.Dispose();
                    ErrorLog.LogError(ex, "DeleteMapHabitationPMGSY3DAL().UpdateException");
                    message = String.Empty;
                    return false;
                }
                catch (Exception ex)
                {
                    ts.Dispose();
                    ErrorLog.LogError(ex, "DeleteMapHabitationPMGSY3DAL()");
                    message = String.Empty;
                    return false;
                }
                finally
                {
                    ts.Dispose();
                    dbContext.Dispose();
                }
            }
        }

        public bool CheckMapHabitationPMGSY3DAL(int roadCode, ref string message)
        {
            PMGSYEntities dbContext = new PMGSYEntities();
            try
            {
                if (dbContext.MASTER_ER_HABITATION_ROAD_PMGSY3.Any(m => m.MAST_ER_ROAD_CODE == roadCode))
                {
                    message = "Habitations are already mapped to this road.So please delete the habitation to change the status.";
                    return false;
                }
                else
                {
                    message = "";
                    return true;
                }
            }
            catch (DbEntityValidationException e)
            {
                ErrorLog.LogError(e, "CheckMapHabitationPMGSY3DAL().DbEntityValidationException");
                ModelStateDictionary modelstate = new ModelStateDictionary();

                foreach (var eve in e.EntityValidationErrors)
                {
                    foreach (var ve in eve.ValidationErrors)
                    {
                        modelstate.AddModelError(ve.ErrorMessage, ve.ErrorMessage);
                    }
                }
                ErrorLog.LogError(e, "CheckMapHabitationPMGSY3DAL().DbEntityValidationException");
                using (System.IO.StreamWriter sw = System.IO.File.AppendText(System.IO.Path.Combine("D:\\ErrorLog\\OMMASErrorLog_" + System.DateTime.Now.Date.ToString("dd_MM_yyyy") + ".txt", "")))
                {
                    sw.WriteLine("Date :" + DateTime.Now.ToString());
                    sw.WriteLine("CheckMapHabitationPMGSY3DAL().DbEntityValidationException() : " + "Application_Error()");

                    sw.WriteLine("Exception : " + new CommonFunctions().FormatErrorMessage(modelstate));
                    sw.WriteLine("---------------------------------------------------------------------------------------");
                    sw.Close();
                }
                //return new CommonFunctions().FormatErrorMessage(modelstate);
                message = String.Empty;
                return false;
            }
            catch (OptimisticConcurrencyException ex)
            {
                ErrorLog.LogError(ex, "CheckMapHabitationPMGSY3DAL().OptimisticConcurrencyException");
                message = String.Empty;
                return false;
            }
            catch (UpdateException ex)
            {
                ErrorLog.LogError(ex, "CheckMapHabitationPMGSY3DAL().UpdateException");
                message = String.Empty;
                return false;
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "CheckMapHabitationPMGSY3DAL()");
                message = "";
                return false;
            }
            finally
            {
                dbContext.Dispose();
            }
        }

        ///Traffic
        public Array GetTrafficListPMGSY3DAL(int? page, int? rows, string sidx, string sord, out Int32 totalRecords, int roadCode)
        {
            try
            {
                dbContext = new PMGSYEntities();
                List<MASTER_ER_TRAFFIC_INTENSITY_PMGSY3> listTrafficIntensity = (from c in dbContext.MASTER_ER_TRAFFIC_INTENSITY_PMGSY3
                                                                                 where
                                                                                 c.MAST_ER_ROAD_CODE == roadCode
                                                                                 select c
                                                             ).OrderBy(c => c.MAST_TI_YEAR).ToList<MASTER_ER_TRAFFIC_INTENSITY_PMGSY3>();

                IQueryable<MASTER_ER_TRAFFIC_INTENSITY_PMGSY3> query = listTrafficIntensity.AsQueryable<MASTER_ER_TRAFFIC_INTENSITY_PMGSY3>();

                totalRecords = listTrafficIntensity.Count();

                if (sidx.Trim() != string.Empty)
                {
                    if (sord.ToString() == "asc")
                    {
                        switch (sidx)
                        {

                            case "Year":
                                query = query.OrderBy(x => x.MAST_TI_YEAR).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                                break;
                            case "TotalMotarisedTrafficday":
                                query = query.OrderBy(x => x.MAST_TOTAL_TI).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                                break;
                            case "CommercialVehicleTrafficDay":
                                query = query.OrderBy(x => x.MAST_COMM_TI).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                                break;
                        }
                    }
                    else
                    {
                        switch (sidx)
                        {

                            case "Year":
                                query = query.OrderByDescending(x => x.MAST_TI_YEAR).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                                break;
                            case "TotalMotarisedTrafficday":
                                query = query.OrderByDescending(x => x.MAST_TOTAL_TI).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                                break;
                            case "CommercialVehicleTrafficDay":
                                query = query.OrderByDescending(x => x.MAST_COMM_TI).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                                break;
                        }
                    }
                }
                else
                {
                    query = query.OrderBy(x => x.MAST_TI_YEAR).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                }

                return query.Select(trafficDetails => new
                {
                    ID = trafficDetails.MAST_ER_ROAD_CODE,
                    cell = new[] {       
                                    trafficDetails.MAST_TI_YEAR.ToString()+"-"+(trafficDetails.MAST_TI_YEAR+1),
                                    trafficDetails.MAST_TOTAL_TI.ToString(),
                                    trafficDetails.MAST_COMM_TI.ToString (),
                                    URLEncrypt.EncryptParameters1(new string[]{"MAST_ER_ROAD_CODE="+trafficDetails.MAST_ER_ROAD_CODE.ToString().Trim(),"MAST_IT_YEAR="+trafficDetails.MAST_TI_YEAR.ToString().Trim()}),

                                    URLEncrypt.EncryptParameters1(new string[]{"MAST_ER_ROAD_CODE="+trafficDetails.MAST_ER_ROAD_CODE.ToString().Trim(),"MAST_IT_YEAR="+trafficDetails.MAST_TI_YEAR.ToString().Trim()})
                    }
                }).ToArray();


            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "GetTrafficListPMGSY3DAL()");
                totalRecords = 0;
                return null;
            }
            finally
            {
                dbContext.Dispose();
            }
        }

        public List<SelectListItem> PopulateTrafficIntensityYearsPMGSY3DAL(int MAST_ER_ROAD_CODE)
        {
            try
            {

                //all years from 2000
                List<SelectListItem> AllYears = PopulateYear(MAST_ER_ROAD_CODE);
                dbContext = new PMGSYEntities();
                var query = (from c in dbContext.MASTER_ER_TRAFFIC_INTENSITY_PMGSY3
                             where c.MAST_ER_ROAD_CODE == MAST_ER_ROAD_CODE
                             select new
                             {
                                 Value = c.MAST_TI_YEAR
                             }).ToList();

                foreach (var data in query)
                {
                    AllYears.Remove(AllYears.Where(c => c.Value == data.Value.ToString()).Single());
                }
                return AllYears;
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "PopulateTrafficIntensityYearsPMGSY3DAL");
                return null;
            }
            finally
            {
                dbContext.Dispose();
            }
        }

        public bool AddTrafficIntensityPMGSY3DAL(TrafficViewModel trafficViewModel, ref string message)
        {
            try
            {
                dbContext = new PMGSYEntities();
                MASTER_ER_TRAFFIC_INTENSITY_PMGSY3 TrafficIntensityModel = new MASTER_ER_TRAFFIC_INTENSITY_PMGSY3();

                TrafficIntensityModel.MAST_ER_ROAD_CODE = trafficViewModel.MAST_ER_ROAD_CODE;
                TrafficIntensityModel.MAST_TI_YEAR = trafficViewModel.MAST_TI_YEAR;
                TrafficIntensityModel.MAST_TOTAL_TI = trafficViewModel.MAST_TOTAL_TI;
                TrafficIntensityModel.MAST_COMM_TI = trafficViewModel.MAST_COMM_TI;

                TrafficIntensityModel.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                TrafficIntensityModel.USERID = PMGSYSession.Current.UserId;

                dbContext.MASTER_ER_TRAFFIC_INTENSITY_PMGSY3.Add(TrafficIntensityModel);

                dbContext.SaveChanges();
                return true;
            }
            catch (DbEntityValidationException e)
            {
                //Elmah.ErrorSignal.FromCurrentContext().Raise(e, HttpContext.Current);
                ErrorLog.LogError(e, "AddTrafficIntensityPMGSY3DAL().DbEntityValidationException");
                ModelStateDictionary modelstate = new ModelStateDictionary();

                foreach (var eve in e.EntityValidationErrors)
                {
                    foreach (var ve in eve.ValidationErrors)
                    {
                        modelstate.AddModelError(ve.ErrorMessage, ve.ErrorMessage);
                    }
                }
                ErrorLog.LogError(e, "AddTrafficIntensityPMGSY3DAL().DbEntityValidationException()");
                using (System.IO.StreamWriter sw = System.IO.File.AppendText(System.IO.Path.Combine("D:\\ErrorLog\\OMMASErrorLog_" + System.DateTime.Now.Date.ToString("dd_MM_yyyy") + ".txt", "")))
                {
                    sw.WriteLine("Date :" + DateTime.Now.ToString());
                    sw.WriteLine("AddTrafficIntensityPMGSY3DAL().DbEntityValidationException() : " + "Application_Error()");

                    sw.WriteLine("Exception : " + new CommonFunctions().FormatErrorMessage(modelstate));
                    sw.WriteLine("---------------------------------------------------------------------------------------");
                    sw.Close();
                }
                return false;
            }
            catch (OptimisticConcurrencyException ex)
            {
                ErrorLog.LogError(ex, "AddTrafficIntensityPMGSY3DAL().DbEntityValidationException");
                message = "An Error Occurred While Processing Your Request.";
                return false;
            }
            catch (UpdateException ex)
            {
                ErrorLog.LogError(ex, "AddTrafficIntensityPMGSY3DAL().DbEntityValidationException");
                message = "An Error Occurred While Processing Your Request.";
                return false;
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "AddTrafficIntensityPMGSY3DAL().DbEntityValidationException");
                message = "An Error Occurred While Processing Your Request.";
                return false;
            }
            finally
            {
                dbContext.Dispose();
            }
        }

        public TrafficViewModel GetTrafficIntensity_ByRoadCodePMGSY3DAL(int MAST_ER_ROAD_CODE, int MAST_TI_YEAR)
        {
            try
            {
                dbContext = new PMGSYEntities();

                MASTER_ER_TRAFFIC_INTENSITY_PMGSY3 trafficIntensityModel = dbContext.MASTER_ER_TRAFFIC_INTENSITY_PMGSY3.FirstOrDefault(m => m.MAST_ER_ROAD_CODE == MAST_ER_ROAD_CODE && m.MAST_TI_YEAR == MAST_TI_YEAR);

                TrafficViewModel trafficViewModel = new TrafficViewModel();
                if (trafficIntensityModel != null)
                {
                    //trafficViewModel = CloneTrafficIntensityObject(trafficIntensityModel);
                    //TrafficViewModel trafficIntensityViewModel = new TrafficViewModel();

                    trafficViewModel.EncryptedErRoadCode = URLEncrypt.EncryptParameters1(new string[] { "RoadCode =" + trafficIntensityModel.MAST_ER_ROAD_CODE.ToString().Trim() });

                    trafficViewModel.MAST_ER_ROAD_CODE = trafficIntensityModel.MAST_ER_ROAD_CODE;
                    trafficViewModel.MAST_TI_YEAR = trafficIntensityModel.MAST_TI_YEAR;
                    trafficViewModel.MAST_TOTAL_TI = trafficIntensityModel.MAST_TOTAL_TI;
                    trafficViewModel.MAST_COMM_TI = trafficIntensityModel.MAST_COMM_TI;

                    trafficViewModel.RoadName = trafficIntensityModel.MASTER_EXISTING_ROADS.MAST_ER_ROAD_NAME;
                    trafficViewModel.RoadNumber = trafficIntensityModel.MASTER_EXISTING_ROADS.MAST_ER_ROAD_NUMBER;
                }
                return trafficViewModel;
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "GetTrafficIntensity_ByRoadCodePMGSY3DAL()");
                return null;
            }
            finally
            {
                dbContext.Dispose();
            }
        }

        public bool EditTrafficIntensityPMGSY3DAL(TrafficViewModel trafficViewModel, ref string message)
        {
            try
            {
                dbContext = new PMGSYEntities();
                encryptedParameters = trafficViewModel.EncryptedErRoadCode.Split('/');

                if (!(encryptedParameters.Length == 3))
                {
                    return false;
                }

                decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { encryptedParameters[0], encryptedParameters[1], encryptedParameters[2] });

                ExistingRoadCode = Convert.ToInt32(decryptedParameters["RoadCode"].ToString());

                trafficViewModel.MAST_ER_ROAD_CODE = ExistingRoadCode;

                MASTER_ER_TRAFFIC_INTENSITY_PMGSY3 trafficIntensityModel = new MASTER_ER_TRAFFIC_INTENSITY_PMGSY3();

                trafficIntensityModel.MAST_ER_ROAD_CODE = trafficViewModel.MAST_ER_ROAD_CODE;
                trafficIntensityModel.MAST_TI_YEAR = trafficViewModel.MAST_TI_YEAR;
                trafficIntensityModel.MAST_TOTAL_TI = trafficViewModel.MAST_TOTAL_TI;
                trafficIntensityModel.MAST_COMM_TI = trafficViewModel.MAST_COMM_TI;

                trafficIntensityModel.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                trafficIntensityModel.USERID = PMGSYSession.Current.UserId;

                dbContext.Entry(trafficIntensityModel).State = System.Data.Entity.EntityState.Modified;
                dbContext.SaveChanges();
                return true;
            }
            catch (DbEntityValidationException e)
            {
                //Elmah.ErrorSignal.FromCurrentContext().Raise(e, HttpContext.Current);
                ErrorLog.LogError(e, "EditTrafficIntensityPMGSY3DAL().DbEntityValidationException");
                ModelStateDictionary modelstate = new ModelStateDictionary();

                foreach (var eve in e.EntityValidationErrors)
                {
                    foreach (var ve in eve.ValidationErrors)
                    {
                        modelstate.AddModelError(ve.ErrorMessage, ve.ErrorMessage);
                    }
                }
                ErrorLog.LogError(e, "EditTrafficIntensityPMGSY3DAL().DbEntityValidationException()");
                using (System.IO.StreamWriter sw = System.IO.File.AppendText(System.IO.Path.Combine("D:\\ErrorLog\\OMMASErrorLog_" + System.DateTime.Now.Date.ToString("dd_MM_yyyy") + ".txt", "")))
                {
                    sw.WriteLine("Date :" + DateTime.Now.ToString());
                    sw.WriteLine("EditTrafficIntensityPMGSY3DAL().DbEntityValidationException() : " + "Application_Error()");

                    sw.WriteLine("Exception : " + new CommonFunctions().FormatErrorMessage(modelstate));
                    sw.WriteLine("---------------------------------------------------------------------------------------");
                    sw.Close();
                }
                return false;
            }
            catch (OptimisticConcurrencyException ex)
            {
                ErrorLog.LogError(ex, "EditTrafficIntensityPMGSY3DAL().OptimisticConcurrencyException");
                message = "An Error Occurred While Processing Your Request.";
                return false;
            }
            catch (UpdateException ex)
            {
                ErrorLog.LogError(ex, "EditTrafficIntensityPMGSY3DAL().UpdateException");
                message = "An Error Occurred While Processing Your Request.";
                return false;
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "EditTrafficIntensityPMGSY3DAL().DbEntityValidationException");
                message = "An Error Occurred While Processing Your Request.";
                return false;
            }
            finally
            {
                dbContext.Dispose();
            }
        }

        /// <summary>
        /// deleting the Traffic intensity details
        /// </summary>
        /// <param name="MAST_ER_ROAD_CODE">existing road code</param>
        /// <param name="MAST_TI_YEAR">year code</param>
        /// <param name="message">response message</param>
        /// <returns></returns>
        public Boolean DeleteTrafficIntensityPMGSY3DAL(int MAST_ER_ROAD_CODE, int MAST_TI_YEAR, ref string message)
        {
            using (TransactionScope ts = new TransactionScope())
            {
                try
                {
                    dbContext = new PMGSYEntities();
                    MASTER_ER_TRAFFIC_INTENSITY_PMGSY3 trafficIntensityModel = dbContext.MASTER_ER_TRAFFIC_INTENSITY_PMGSY3.Where(m => m.MAST_ER_ROAD_CODE == MAST_ER_ROAD_CODE && m.MAST_TI_YEAR == MAST_TI_YEAR).FirstOrDefault();

                    if (trafficIntensityModel == null)
                    {
                        return false;
                    }

                    trafficIntensityModel.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                    trafficIntensityModel.USERID = PMGSYSession.Current.UserId;
                    dbContext.Entry(trafficIntensityModel).State = System.Data.Entity.EntityState.Modified;
                    dbContext.SaveChanges();

                    dbContext.MASTER_ER_TRAFFIC_INTENSITY_PMGSY3.Remove(trafficIntensityModel);

                    dbContext.SaveChanges();
                    ts.Complete();
                    return true;
                }
                catch (System.Data.Entity.Infrastructure.DbUpdateException ex)
                {
                    ts.Dispose();
                    ErrorLog.LogError(ex, "DeleteTrafficIntensityPMGSY3DAL().DbEntityValidationException");
                    message = "You can not delete this Traffic Intensity details because this details are in use.";
                    return false;
                }
                catch (Exception ex)
                {
                    ts.Dispose();
                    ErrorLog.LogError(ex, "DeleteTrafficIntensityPMGSY3DAL().DbEntityValidationException");
                    message = "An Error Occurred While Processing Your Request.";
                    return false;
                }
                finally
                {
                    dbContext.Dispose();
                }
            }
        }

        ///CBR
        public MASTER_ER_CBR_VALUE_PMGSY3 GetCBRDetailsPMGSY3DAL(int roadCode)
        {
            dbContext = new PMGSYEntities();
            try
            {
                MASTER_ER_CBR_VALUE_PMGSY3 master = dbContext.MASTER_ER_CBR_VALUE_PMGSY3.Where(a => a.MAST_ER_ROAD_CODE == roadCode && a.MAST_SEGMENT_NO == (dbContext.MASTER_ER_CBR_VALUE_PMGSY3.Where(m => m.MAST_ER_ROAD_CODE == roadCode).Max(m => m.MAST_SEGMENT_NO))).FirstOrDefault();
                return master;
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "GetCBRDetailsPMGSY3DAL()");
                return null;
            }
            finally
            {
                dbContext.Dispose();
            }
        }

        public CBRViewModel GetCBRDetailsPMGSY3DAL(int MAST_ER_ROAD_CODE, int MAST_SEGMENT_NO)
        {
            try
            {
                dbContext = new PMGSYEntities();

                MASTER_ER_CBR_VALUE_PMGSY3 CBRModel = dbContext.MASTER_ER_CBR_VALUE_PMGSY3.FirstOrDefault(m => m.MAST_ER_ROAD_CODE == MAST_ER_ROAD_CODE && m.MAST_SEGMENT_NO == MAST_SEGMENT_NO);

                CBRViewModel CbrViewModel = new CBRViewModel();
                if (CBRModel != null)
                {
                    //cbrViewModel = CloneCBRObject(CBRModel);
                    //CBRViewModel CbrViewModel = new CBRViewModel();

                    CbrViewModel.EncryptedCBRCode = CBRModel.MAST_ER_ROAD_CODE.ToString();
                    CbrViewModel.MAST_ER_ROAD_CODE = CBRModel.MAST_ER_ROAD_CODE;
                    CbrViewModel.MAST_SEGMENT_NO = CBRModel.MAST_SEGMENT_NO;
                    CbrViewModel.MAST_STR_CHAIN = CBRModel.MAST_STR_CHAIN;
                    CbrViewModel.MAST_END_CHAIN = CBRModel.MAST_END_CHAIN;

                    CbrViewModel.EndChainage = CBRModel.MAST_END_CHAIN;

                    CbrViewModel.MAST_CBR_VALUE = CBRModel.MAST_CBR_VALUE;

                    CbrViewModel.RoadName = CBRModel.MASTER_EXISTING_ROADS.MAST_ER_ROAD_NAME;
                    CbrViewModel.RoadID = CBRModel.MASTER_EXISTING_ROADS.MAST_ER_ROAD_NUMBER;

                    CbrViewModel.Segment_Length = CBRModel.MAST_END_CHAIN - CBRModel.MAST_STR_CHAIN;

                }
                return CbrViewModel;
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "GetCBRDetailsPMGSY3DAL(int MAST_ER_ROAD_CODE, int MAST_SEGMENT_NO)");
                return null;
            }
            finally
            {
                dbContext.Dispose();
            }
        }

        public Array GetCBRListPMGSY3DAL(int? page, int? rows, string sidx, string sord, out Int32 totalRecords, int roadCode)
        {
            try
            {
                dbContext = new PMGSYEntities();
                List<MASTER_ER_CBR_VALUE_PMGSY3> listCBR = (from CrbValue in dbContext.MASTER_ER_CBR_VALUE_PMGSY3
                                                            where
                                                           CrbValue.MAST_ER_ROAD_CODE == roadCode
                                                            select CrbValue
                                                             ).OrderBy(c => c.MAST_SEGMENT_NO).ToList<MASTER_ER_CBR_VALUE_PMGSY3>();

                IQueryable<MASTER_ER_CBR_VALUE_PMGSY3> query = listCBR.AsQueryable<MASTER_ER_CBR_VALUE_PMGSY3>();
                totalRecords = listCBR.Count();
                int recCount = totalRecords;

                if (sidx.Trim() != string.Empty)
                {
                    if (sord.ToString() == "asc")
                    {
                        switch (sidx)
                        {
                            case "StartChainage":
                                query = query.OrderBy(x => x.MAST_STR_CHAIN).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                                break;
                            case "EndChainage":
                                query = query.OrderBy(x => x.MAST_END_CHAIN).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                                break;
                            case "CBRValue":
                                query = query.OrderBy(x => x.MAST_CBR_VALUE).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                                break;
                        }
                    }
                    else
                    {
                        switch (sidx)
                        {
                            case "StartChainage":
                                query = query.OrderByDescending(x => x.MAST_STR_CHAIN).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                                break;
                            case "EndChainage":
                                query = query.OrderByDescending(x => x.MAST_END_CHAIN).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                                break;
                            case "CBRValue":
                                query = query.OrderByDescending(x => x.MAST_CBR_VALUE).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                                break;
                        }
                    }
                }
                else
                {
                    query = query.OrderBy(x => x.MAST_SEGMENT_NO).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                }

                return query.Select(CBRDetails => new
                {

                    cell = new[] {                                      

                                    CBRDetails.MAST_STR_CHAIN.ToString(),
                                    CBRDetails.MAST_END_CHAIN.ToString(),
                                    (Convert.ToDecimal(CBRDetails.MAST_END_CHAIN - CBRDetails.MAST_STR_CHAIN)).ToString(),
                                    CBRDetails.MAST_CBR_VALUE.ToString(),

                                    URLEncrypt.EncryptParameters1(new string[]{"MAST_ER_ROAD_CODE="+CBRDetails.MAST_ER_ROAD_CODE.ToString().Trim(),"MAST_SEGMENT_NO="+CBRDetails.MAST_SEGMENT_NO.ToString().Trim()}),

                                    recCount==1?URLEncrypt.EncryptParameters1(new string[]{"MAST_ER_ROAD_CODE="+CBRDetails.MAST_ER_ROAD_CODE.ToString().Trim(),"MAST_SEGMENT_NO="+CBRDetails.MAST_SEGMENT_NO.ToString().Trim()}):recCount==CBRDetails.MAST_SEGMENT_NO?URLEncrypt.EncryptParameters1(new string[]{"MAST_ER_ROAD_CODE="+CBRDetails.MAST_ER_ROAD_CODE.ToString().Trim(),"MAST_SEGMENT_NO="+CBRDetails.MAST_SEGMENT_NO.ToString().Trim()}):String.Empty,
                    }
                }).ToArray();
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "GetCBRListPMGSY3DAL()");
                totalRecords = 0;
                return null;
            }
            finally
            {
                dbContext.Dispose();
            }
        }

        public bool AddCbrValuePMGSY3DAL(CBRViewModel CbrViewModel, ref string message)
        {
            try
            {
                dbContext = new PMGSYEntities();
                MASTER_ER_CBR_VALUE_PMGSY3 CBRModel = new MASTER_ER_CBR_VALUE_PMGSY3();


                CBRModel.MAST_SEGMENT_NO = (dbContext.MASTER_ER_CBR_VALUE_PMGSY3.Any(m => m.MAST_ER_ROAD_CODE == CbrViewModel.MAST_ER_ROAD_CODE) ? dbContext.MASTER_ER_CBR_VALUE_PMGSY3.Where(m => m.MAST_ER_ROAD_CODE == CbrViewModel.MAST_ER_ROAD_CODE).Max(m => m.MAST_SEGMENT_NO) : 0) + 1;
                CBRModel.MAST_END_CHAIN = CbrViewModel.MAST_END_CHAIN;


                CBRModel.MAST_ER_ROAD_CODE = CbrViewModel.MAST_ER_ROAD_CODE;
                CBRModel.MAST_STR_CHAIN = CbrViewModel.MAST_STR_CHAIN;

                CBRModel.MAST_CBR_VALUE = CbrViewModel.MAST_CBR_VALUE;

                dbContext = new PMGSYEntities();
                CBRModel.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                CBRModel.USERID = PMGSYSession.Current.UserId;
                dbContext.MASTER_ER_CBR_VALUE_PMGSY3.Add(CBRModel);
                dbContext.SaveChanges();
                return true;
            }
            catch (DbEntityValidationException e)
            {
                //Elmah.ErrorSignal.FromCurrentContext().Raise(e, HttpContext.Current);
                ErrorLog.LogError(e, "AddCbrValuePMGSY3DAL().DbEntityValidationException");
                ModelStateDictionary modelstate = new ModelStateDictionary();

                foreach (var eve in e.EntityValidationErrors)
                {
                    foreach (var ve in eve.ValidationErrors)
                    {
                        modelstate.AddModelError(ve.ErrorMessage, ve.ErrorMessage);
                    }
                }
                ErrorLog.LogError(e, "AddCbrValuePMGSY3DAL().DbEntityValidationException()");
                using (System.IO.StreamWriter sw = System.IO.File.AppendText(System.IO.Path.Combine("D:\\ErrorLog\\OMMASErrorLog_" + System.DateTime.Now.Date.ToString("dd_MM_yyyy") + ".txt", "")))
                {
                    sw.WriteLine("Date :" + DateTime.Now.ToString());
                    sw.WriteLine("AddCbrValuePMGSY3DAL().DbEntityValidationException() : " + "Application_Error()");

                    sw.WriteLine("Exception : " + new CommonFunctions().FormatErrorMessage(modelstate));
                    sw.WriteLine("---------------------------------------------------------------------------------------");
                    sw.Close();
                }
                return false;
            }
            catch (OptimisticConcurrencyException ex)
            {
                ErrorLog.LogError(ex, "AddCbrValuePMGSY3DAL().OptimisticConcurrencyException");
                message = "An Error Occurred While Processing Your Request.";
                return false;
            }
            catch (UpdateException ex)
            {
                ErrorLog.LogError(ex, "AddCbrValuePMGSY3DAL().UpdateException");
                message = "An Error Occurred While Processing Your Request.";
                return false;
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "AddCbrValuePMGSY3DAL()");
                message = "An Error Occurred While Processing Your Request.";
                return false;
            }
            finally
            {
                dbContext.Dispose();
            }
        }

        /// <summary>
        /// updates the CBR details
        /// </summary>
        /// <param name="CbrViewModel">contains the updated CBR details</param>
        /// <param name="message">reposnse message</param>
        /// <returns></returns>
        public bool EditCbrValuePMGSY3(CBRViewModel CbrViewModel, ref string message)
        {
            try
            {
                dbContext = new PMGSYEntities();
                bool flagCbrAddEdit = false;

                MASTER_ER_CBR_VALUE_PMGSY3 CBRModel = new MASTER_ER_CBR_VALUE_PMGSY3();

                CBRModel.MAST_SEGMENT_NO = CbrViewModel.MAST_SEGMENT_NO;
                CBRModel.MAST_END_CHAIN = CbrViewModel.EndChainage;


                CBRModel.MAST_ER_ROAD_CODE = CbrViewModel.MAST_ER_ROAD_CODE;
                CBRModel.MAST_STR_CHAIN = CbrViewModel.MAST_STR_CHAIN;

                CBRModel.MAST_CBR_VALUE = CbrViewModel.MAST_CBR_VALUE;

                CBRModel.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                CBRModel.USERID = PMGSYSession.Current.UserId;
                dbContext.Entry(CBRModel).State = System.Data.Entity.EntityState.Modified;
                dbContext.SaveChanges();
                return true;
            }
            catch (DbEntityValidationException e)
            {
                //Elmah.ErrorSignal.FromCurrentContext().Raise(e, HttpContext.Current);
                ErrorLog.LogError(e, "EditCbrValuePMGSY().DbEntityValidationException");
                ModelStateDictionary modelstate = new ModelStateDictionary();

                foreach (var eve in e.EntityValidationErrors)
                {
                    foreach (var ve in eve.ValidationErrors)
                    {
                        modelstate.AddModelError(ve.ErrorMessage, ve.ErrorMessage);
                    }
                }
                ErrorLog.LogError(e, "EditCbrValuePMGSY().DbEntityValidationException()");
                using (System.IO.StreamWriter sw = System.IO.File.AppendText(System.IO.Path.Combine("D:\\ErrorLog\\OMMASErrorLog_" + System.DateTime.Now.Date.ToString("dd_MM_yyyy") + ".txt", "")))
                {
                    sw.WriteLine("Date :" + DateTime.Now.ToString());
                    sw.WriteLine("EditCbrValuePMGSY().DbEntityValidationException() : " + "Application_Error()");

                    sw.WriteLine("Exception : " + new CommonFunctions().FormatErrorMessage(modelstate));
                    sw.WriteLine("---------------------------------------------------------------------------------------");
                    sw.Close();
                }
                return false;
            }
            catch (OptimisticConcurrencyException ex)
            {
                ErrorLog.LogError(ex, "EditCbrValuePMGSY().OptimisticConcurrencyException");
                message = "An Error Occurred While Processing Your Request.";
                return false;
            }
            catch (UpdateException ex)
            {
                ErrorLog.LogError(ex, "EditCbrValuePMGSY().UpdateException");
                message = "An Error Occurred While Processing Your Request.";
                return false;
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "EditCbrValuePMGSY()");
                message = "An Error Occurred While Processing Your Request.";
                return false;
            }
            finally
            {
                dbContext.Dispose();
            }
        }

        /// <summary>
        /// delete operation of CBR details
        /// </summary>
        /// <param name="MAST_ER_ROAD_CODE">existing road code</param>
        /// <param name="MAST_SEGMENT_NO">segment id</param>
        /// <param name="message">response message</param>
        /// <returns></returns>
        public Boolean DeleteCbrValuePMGSY3(int MAST_ER_ROAD_CODE, int MAST_SEGMENT_NO, ref string message)
        {
            using (TransactionScope ts = new TransactionScope())
            {
                try
                {
                    dbContext = new PMGSYEntities();
                    MASTER_ER_CBR_VALUE_PMGSY3 CbrModel = dbContext.MASTER_ER_CBR_VALUE_PMGSY3.Where(m => m.MAST_ER_ROAD_CODE == MAST_ER_ROAD_CODE && m.MAST_SEGMENT_NO == MAST_SEGMENT_NO).FirstOrDefault();

                    if (CbrModel == null)
                    {
                        return false;
                    }
                    CbrModel.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                    CbrModel.USERID = PMGSYSession.Current.UserId;
                    dbContext.Entry(CbrModel).State = System.Data.Entity.EntityState.Modified;
                    dbContext.SaveChanges();


                    dbContext.MASTER_ER_CBR_VALUE_PMGSY3.Remove(CbrModel);
                    dbContext.SaveChanges();
                    ts.Complete();
                    return true;
                }
                catch (System.Data.Entity.Infrastructure.DbUpdateException ex)
                {
                    ts.Dispose();
                    ErrorLog.LogError(ex, "EditCbrValuePMGSY().DbUpdateException");
                    message = "You can not delete this CBR details because this details are in use.";
                    return false;
                }
                catch (Exception ex)
                {
                    ts.Dispose();
                    ErrorLog.LogError(ex, "EditCbrValuePMGSY()");
                    message = "An Error Occurred While Processing Your Request.";
                    return false;
                }
                finally
                {
                    dbContext.Dispose();
                }
            }
        }
        #endregion

        #region DRRP - II PMGSY-I Mapping

        public Array GetProposalsForDRRPMappingUnderPMGSY3DAL(int? page, int? rows, string sidx, string sord, out long totalRecords, int stateCode, int districtCode, int IMS_YEAR, int MAST_BLOCK_ID, int IMS_BATCH, int IMS_STREAMS, string IMS_PROPOSAL_TYPE, int adminCode, string IMS_PROPOSAL_STATUS, string IMS_UPGRADE_CONNECT)
        {
            dbContext = new Models.PMGSYEntities();
            try
            {
                int state_Code = stateCode == -1 ? 0 : stateCode;
                int district_Code = districtCode == -1 ? 0 : districtCode;
                int YEAR_Code = IMS_YEAR == -1 ? 0 : IMS_YEAR;
                int BLOCK_Code = MAST_BLOCK_ID == -1 ? 0 : MAST_BLOCK_ID;
                int BATCH_Code = IMS_BATCH == -1 ? 0 : IMS_BATCH;
                int STREAMS_Code = IMS_STREAMS == -1 ? 0 : IMS_STREAMS;
                string UPGRADE_CONNECT = IMS_UPGRADE_CONNECT.Equals("0") ? "%" : IMS_UPGRADE_CONNECT;
                dbContext = new Models.PMGSYEntities();
                List<USP_DRRP_ROAD_LIST_MAPPING_Result> itemList = new List<USP_DRRP_ROAD_LIST_MAPPING_Result>();

                itemList = dbContext.USP_DRRP_ROAD_LIST_MAPPING(state_Code, district_Code, BLOCK_Code, YEAR_Code, BATCH_Code, STREAMS_Code, UPGRADE_CONNECT).ToList<USP_DRRP_ROAD_LIST_MAPPING_Result>();
                totalRecords = itemList.Count();

                if (sidx.Trim() != string.Empty)
                {
                    if (sord.ToString() == "asc")
                    {
                        itemList = itemList.OrderBy(x => x.WORK_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                    }
                    else
                    {
                        itemList = itemList.OrderByDescending(x => x.WORK_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                    }
                }
                else
                {
                    itemList = itemList.OrderBy(x => x.WORK_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                }


                return itemList.Select(propDetails => new
                {
                    id = propDetails.SANCTIONED_ROAD_CODE.ToString(),
                    cell = new[] {     

                                    propDetails.MAST_STATE_NAME.Trim(),
                                    propDetails.MAST_DISTRICT_NAME.Trim(),
                                    propDetails.MAST_BLOCK_NAME.Trim(),
                                    propDetails.WORK_NAME.Trim(),
                                    propDetails.PACKAGE.ToString(),     
                                    propDetails.IMS_PAV_LENGTH.ToString(), 
                                    (propDetails.SANCTIONED_YEAR + " - " + (propDetails.SANCTIONED_YEAR + 1)).ToString(),
                                    propDetails.IMS_UPGRADE_CONNECT == "N" ? "New" : "Upgradation",
                                    ((propDetails.DRRP_NAME_MAPPED_UNDER_PMGSY_3==null || propDetails.DRRP_NAME_MAPPED_UNDER_PMGSY_3==string.Empty)?"":propDetails.DRRP_NAME_MAPPED_UNDER_PMGSY_3.ToString()),
                                    ((propDetails.MAST_ER_ROAD_NAME==null || propDetails.MAST_ER_ROAD_NAME==string.Empty || string.IsNullOrEmpty(propDetails.DRRP_NAME_MAPPED_UNDER_PMGSY_3))?(  propDetails.DRRP_IS_FINALIZED=="Y"?"<center><table><tr><td style='border:none'><a href='#' title='Locked' class='ui-icon ui-icon-locked  ui-align-center'>Locked</a></td></tr></table></center>": "<a href='#' title='Click here to map DRRP details' class='ui-icon ui-icon-plusthick ui-align-center' onClick='MapDRRP(\"" + URLEncrypt.EncryptParameters(new string[] {propDetails.SANCTIONED_ROAD_CODE.ToString()  })  +"\"); return false;'>Map Details</a>") :propDetails.MAST_ER_ROAD_NAME.ToString()),
                                    (propDetails.DRRP_IS_FINALIZED=="N"?"<a href='#' title='Click here to Finalize DRRP details' class='ui-icon ui-icon-unlocked ui-align-center' onClick='FinalizeDRRP(\"" + propDetails.SANCTIONED_ROAD_CODE.ToString()+"\"); return false;'>Finalize Details</a>":(PMGSYSession.Current.RoleCode==2?"<a href='#' title='Click here to Finalize DRRP details' class='ui-icon ui-icon-locked ui-align-center' onClick='DeFinalizeDRRP(\"" + propDetails.SANCTIONED_ROAD_CODE.ToString()+"\"); return false;'>DeFinalize Details</a>":"-"))
                
                    }
                }).ToArray();



            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "ExistingRoadDAL.GetProposalsForDRRPMappingUnderPMGSY3DAL()");
                totalRecords = 0;
                return null;
            }
            finally
            {
                dbContext.Dispose();
            }
        }

        public MapDRRPUnderPMGSY3 GetProposalDetails(int proposalCode)
        {
            dbContext = new PMGSYEntities();
            try
            {
                MapDRRPUnderPMGSY3 model = new MapDRRPUnderPMGSY3();
                IMS_SANCTIONED_PROJECTS imsMaster = dbContext.IMS_SANCTIONED_PROJECTS.Find(proposalCode);
                model.WorkName = imsMaster.IMS_ROAD_NAME;
                model.PackageName = imsMaster.IMS_PACKAGE_ID;
                model.ProposalCode = proposalCode;
                model.ProposalType = imsMaster.IMS_PROPOSAL_TYPE;
                model.UpgradeConnect = imsMaster.IMS_UPGRADE_CONNECT;
                model.Block = imsMaster.MAST_BLOCK_CODE;
                return model;
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "ExistingRoadDAL.GetProposalDetails()");
                return null;
            }
            finally
            {
                dbContext.Dispose();
            }
        }

        public string FinalizeProposalDAL(int prRoadCode)
        {
            var dbContext = new PMGSYEntities();
            try
            {
                MASTER_ER_MAPROAD_PMGSY3 details = dbContext.MASTER_ER_MAPROAD_PMGSY3.Where(m => m.IMS_PR_ROAD_CODE == prRoadCode).FirstOrDefault();

                if (details == null)
                {
                    return "DRRP details are not mapped yet.";
                }
                else
                {
                    details.IS_FINALIZED = "Y";
                    details.DATE_FINALIZATION = System.DateTime.Now;
                    details.USERID = PMGSYSession.Current.UserId;
                    details.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                    dbContext.Entry(details).State = System.Data.Entity.EntityState.Modified;
                    dbContext.SaveChanges();
                    return string.Empty;
                }
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "ExistingRoadDAL.FinalizeProposalDAL()");
                return "Error occurred while processing your request.";
            }
            finally
            {
                dbContext.Dispose();
            }
        }

        public string DeFinalizeProposalDAL(int prRoadCode)
        {
            var dbContext = new PMGSYEntities();
            try
            {
                MASTER_ER_MAPROAD_PMGSY3 details = dbContext.MASTER_ER_MAPROAD_PMGSY3.Where(m => m.IMS_PR_ROAD_CODE == prRoadCode).FirstOrDefault();

                if (details == null)
                {
                    return "DRRP details are not mapped yet.";
                }
                else
                {
                    details.IS_FINALIZED = "N";
                    details.DATE_FINALIZATION = System.DateTime.Now;
                    details.USERID = PMGSYSession.Current.UserId;
                    details.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                    dbContext.Entry(details).State = System.Data.Entity.EntityState.Modified;
                    dbContext.SaveChanges();
                    return string.Empty;
                }
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "ExistingRoadDAL.DeFinalizeProposalDAL()");
                return "Error occurred while processing your request.";
            }
            finally
            {
                dbContext.Dispose();
            }
        }

        public bool MapDRRPDetailsDAL(MapDRRPUnderPMGSY3 model)
        {
            dbContext = new PMGSYEntities();
            try
            {
                using (TransactionScope ts = new TransactionScope())
                {
                    if (dbContext.MASTER_ER_MAPROAD_PMGSY3.Where(m => m.MAST_ER_ROAD_CODE == model.CnCode && m.IMS_PR_ROAD_CODE == model.ProposalCode).Any())
                    {// Duplicate Check 
                        return false;
                    }

                    if (dbContext.MASTER_ER_MAPROAD_PMGSY3.Where(m => m.IMS_PR_ROAD_CODE == model.ProposalCode).Any())
                    {
                        MASTER_ER_MAPROAD_PMGSY3 masterModel = dbContext.MASTER_ER_MAPROAD_PMGSY3.Where(m => m.IMS_PR_ROAD_CODE == model.ProposalCode).FirstOrDefault();

                        if (masterModel != null)
                        {
                            // Remove Already added record
                            dbContext.MASTER_ER_MAPROAD_PMGSY3.Remove(masterModel);
                            dbContext.SaveChanges();

                            // update / add new road and drrp mapping details
                            MASTER_ER_MAPROAD_PMGSY3 masterTable = new MASTER_ER_MAPROAD_PMGSY3();
                            string DRRP_NAME = dbContext.MASTER_EXISTING_ROADS.Where(m => m.MAST_ER_ROAD_CODE == model.CnCode).Select(m => m.MAST_ER_ROAD_NAME).FirstOrDefault();
                            masterTable.MAST_ER_ROAD_CODE = model.CnCode; // This is DRRP Code
                            masterTable.IMS_PR_ROAD_CODE = model.ProposalCode;
                            masterTable.IS_FINALIZED = "N";
                            masterTable.DATE_FINALIZATION = System.DateTime.Now;
                            masterTable.USERID = PMGSYSession.Current.UserId;
                            masterTable.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                            masterTable.DRRP_NAME = DRRP_NAME.Trim();
                            dbContext.MASTER_ER_MAPROAD_PMGSY3.Add(masterTable);
                            dbContext.SaveChanges();

                            ts.Complete();
                            return true;
                        }
                        else
                        {
                            ts.Complete();
                            return true;
                        }
                    }
                    else
                    { // To Newly Map DPPP To Road

                        MASTER_ER_MAPROAD_PMGSY3 masterTable = new MASTER_ER_MAPROAD_PMGSY3();

                        string DRRP_NAME = dbContext.MASTER_EXISTING_ROADS.Where(m => m.MAST_ER_ROAD_CODE == model.CnCode).Select(m => m.MAST_ER_ROAD_NAME).FirstOrDefault();

                        masterTable.MAST_ER_ROAD_CODE = model.CnCode; // This is DRRP Code
                        masterTable.IMS_PR_ROAD_CODE = model.ProposalCode;
                        masterTable.IS_FINALIZED = "N";
                        masterTable.DATE_FINALIZATION = System.DateTime.Now;
                        masterTable.USERID = PMGSYSession.Current.UserId;
                        masterTable.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                        masterTable.DRRP_NAME = DRRP_NAME.Trim();
                        dbContext.MASTER_ER_MAPROAD_PMGSY3.Add(masterTable);
                        dbContext.SaveChanges();
                        ts.Complete();
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {

                ErrorLog.LogError(ex, "ExistingRoadDAL.MapDRRPDetailsDAL()");
                return false;
            }
            finally
            {
                dbContext.Dispose();
            }
        }

        #endregion

        #region Finalize DRRP PMGSY3 BLOCK/DISTRICT
        public Array GetBlockListPMGSY3DAL(int districtCode, int? page, int? rows, string sidx, string sord, out long totalRecords, string filters, ref bool isAllBlockFinalized)
        {
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

                var lstDRRPFinalizedBlocks = dbContext.MAST_DRRP_BLOCK_PMGSY3_FINALIZE.Where(z => z.MASTER_BLOCK.MAST_DISTRICT_CODE == districtCode && z.MASTER_BLOCK.MAST_BLOCK_ACTIVE == "Y" && z.IS_FINALIZED == "Y").Select(x => new { MAST_BLOCK_CODE = x.MAST_BLOCK_CODE, MAST_BLOCK_NAME = x.MASTER_BLOCK.MAST_BLOCK_NAME }).Distinct().ToList();

                ///Get Block Names
                var lstBlock = dbContext.MASTER_BLOCK.Where(c => c.MAST_DISTRICT_CODE == districtCode && c.MAST_BLOCK_ACTIVE == "Y").Select(x => new { MAST_BLOCK_CODE = x.MAST_BLOCK_CODE, MAST_BLOCK_NAME = x.MAST_BLOCK_NAME.Trim() }).OrderBy(z => z.MAST_BLOCK_CODE).Distinct().ToList();

                totalRecords = lstBlock.Count();

                if (lstBlock.Count() == lstDRRPFinalizedBlocks.Count() && (!dbContext.MAST_DRRP_DISTRICT_PMGSY3_FINALIZE.Any(z => z.MAST_DISTRICT_CODE == districtCode && z.IS_FINALIZED == "Y") || !dbContext.MAST_DRRP_DISTRICT_PMGSY3_FINALIZE.Any(z => z.MAST_DISTRICT_CODE == districtCode)))
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

                return lstBlock.Select(item => new
                {
                    //id = item.MAST_ER_ROAD_CODE,
                    cell = new[]
                    {       
                        item.MAST_BLOCK_NAME.ToString(),
                        
                        (dbContext.MAST_DRRP_BLOCK_PMGSY3_FINALIZE.Where(z=>z.MAST_BLOCK_CODE == item.MAST_BLOCK_CODE && z.IS_FINALIZED == "Y").Any())
                        ? "<span class='ui-icon ui-icon-locked ui-align-center' title='Locked'></span>"
                        : (dbContext.MASTER_EXISTING_ROADS.Where(m=>m.MAST_DISTRICT_CODE == districtCode && m.MAST_BLOCK_CODE == item.MAST_BLOCK_CODE && m.MAST_LOCK_STATUS == "N" && m.MAST_PMGSY_SCHEME == 2).Any()) 
                                ? "All DRRP roads not finalized for the block"
                                : (dbContext.MAST_DRRP_BLOCK_PMGSY3_FINALIZE.Where(z=>z.MAST_BLOCK_CODE == item.MAST_BLOCK_CODE && z.IS_FINALIZED == "N").Any() || !(dbContext.MAST_DRRP_BLOCK_PMGSY3_FINALIZE.Where(z=>z.MAST_BLOCK_CODE == item.MAST_BLOCK_CODE).Any()))
                                    ? "<a href='#' title='Click here to finalize details' class='ui-icon ui-icon-unlocked ui-align-center' onClick =FinalizeDRRPBlock('"
                                            +URLEncrypt.EncryptParameters1(new string[]{"BlockCode =" + item.MAST_BLOCK_CODE.ToString().Trim()}) +"'); return false;'></a>"
                            
                                    : ""
                    }
                }).ToArray();
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "ListExistingRoadsPMGSY3DAL()");
                totalRecords = 0;
                return null;
            }
            finally
            {
                dbContext.Dispose();
            }
        }

        public bool FinalizeDRRPBlockPMGSY3DAL(int blockCode, ref string message)
        {
            PMGSYEntities dbContext = new PMGSYEntities();
            try
            {
                MAST_DRRP_BLOCK_PMGSY3_FINALIZE DRRPPmgsy3Model = new MAST_DRRP_BLOCK_PMGSY3_FINALIZE();
                dbContext = new PMGSYEntities();

                DRRPPmgsy3Model.MAST_ER_BLOCK_FIN_CODE = (dbContext.MAST_DRRP_BLOCK_PMGSY3_FINALIZE.Any() ? dbContext.MAST_DRRP_BLOCK_PMGSY3_FINALIZE.Max(z => z.MAST_ER_BLOCK_FIN_CODE) : 0) + 1;
                DRRPPmgsy3Model.MAST_BLOCK_CODE = blockCode;
                DRRPPmgsy3Model.MAST_DRRP_BLOCK_FINALIZE_DATE = DateTime.Now;
                DRRPPmgsy3Model.IS_FINALIZED = "Y";

                DRRPPmgsy3Model.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                DRRPPmgsy3Model.USERID = PMGSYSession.Current.UserId;
                dbContext.MAST_DRRP_BLOCK_PMGSY3_FINALIZE.Add(DRRPPmgsy3Model);
                dbContext.SaveChanges();
                message = "Block finalized successfully";
                return true;
            }
            catch (DbEntityValidationException e)
            {
                ErrorLog.LogError(e, "FinalizeDRRPBlockPMGSY3DAL().DbEntityValidationException");

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
                    sw.WriteLine("FinalizeDRRPBlockPMGSY3DAL().DbEntityValidationException()");
                    sw.WriteLine("Exception : " + new CommonFunctions().FormatErrorMessage(modelstate));
                    sw.WriteLine("---------------------------------------------------------------------------------------");
                    sw.Close();
                }
                return false;
            }
            catch (OptimisticConcurrencyException ex)
            {
                ErrorLog.LogError(ex, "FinalizeDRRPBlockPMGSY3DAL().OptimisticConcurrencyException");

                message = "An Error Occurred While Processing Your Request.";
                return false;
            }
            catch (UpdateException ex)
            {
                ErrorLog.LogError(ex, "FinalizeDRRPBlockPMGSY3DAL().UpdateException");
                message = "An Error Occurred While Processing Your Request.";
                return false;
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "FinalizeDRRPBlockPMGSY3DAL()");
                message = "An Error Occurred While Processing Your Request.";
                return false;
            }
            finally
            {
                dbContext.Dispose();
            }
        }

        public bool FinalizeDRRPDistrictPMGSY3DAL(int districtCode, ref string message)
        {
            PMGSYEntities dbContext = new PMGSYEntities();
            try
            {
                MAST_DRRP_DISTRICT_PMGSY3_FINALIZE DRRPPmgsy3Model = new MAST_DRRP_DISTRICT_PMGSY3_FINALIZE();
                dbContext = new PMGSYEntities();

                DRRPPmgsy3Model.MAST_ER_DISTRICT_FIN_CODE = (dbContext.MAST_DRRP_DISTRICT_PMGSY3_FINALIZE.Any() ? dbContext.MAST_DRRP_DISTRICT_PMGSY3_FINALIZE.Max(z => z.MAST_ER_DISTRICT_FIN_CODE) : 0) + 1;
                DRRPPmgsy3Model.MAST_DISTRICT_CODE = districtCode;
                DRRPPmgsy3Model.MAST_DRRP_DISTRICT_FINALIZE_DATE = DateTime.Now;
                DRRPPmgsy3Model.IS_FINALIZED = "Y";

                DRRPPmgsy3Model.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                DRRPPmgsy3Model.USERID = PMGSYSession.Current.UserId;
                dbContext.MAST_DRRP_DISTRICT_PMGSY3_FINALIZE.Add(DRRPPmgsy3Model);
                dbContext.SaveChanges();
                message = "District finalized successfully";
                return true;
            }
            catch (OptimisticConcurrencyException ex)
            {
                ErrorLog.LogError(ex, "FinalizeDRRPDistrictPMGSY3DAL().OptimisticConcurrencyException");
                message = "An Error Occurred While Processing Your Request.";
                return false;
            }
            catch (UpdateException ex)
            {
                ErrorLog.LogError(ex, "FinalizeDRRPDistrictPMGSY3DAL().UpdateException");
                message = "An Error Occurred While Processing Your Request.";
                return false;
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "FinalizeDRRPDistrictPMGSY3DAL()");
                message = "An Error Occurred While Processing Your Request.";
                return false;
            }
            finally
            {
                dbContext.Dispose();
            }
        }
        #endregion

        #region Trace Maps

        public Array GetDistrictListDAL(int page, int rows, string sidx, string sord, out int totalRecords, int districtcode)
        {
            PMGSYEntities dbContext = new PMGSYEntities();

            //var BlockList = new CommonFunctions().PopulateBlocks(blockcode);
            //BlockList.Remove(BlockList.Find(x => x.Text.Equals("Select Block")));
            var BlockList = (from c in dbContext.MASTER_BLOCK
                             where
                             c.MAST_DISTRICT_CODE == districtcode && c.MAST_BLOCK_ACTIVE == "Y"
                             select new
                             {
                                 c.MAST_BLOCK_NAME,
                                 c.MAST_BLOCK_CODE,
                                 c.MASTER_DISTRICT.MAST_DISTRICT_NAME
                             }).OrderBy(x => x.MAST_DISTRICT_NAME).ToList();

            try
            {
                totalRecords = BlockList.Count();
                var isFinalizedcheck = (from item in dbContext.MAST_FACILITY_DISTRICT_PMGSY3_FINALIZE
                                        join MB in dbContext.MASTER_BLOCK on item.MAST_DISTRICT_CODE
                                        equals MB.MAST_DISTRICT_CODE
                                        select new
                                        {
                                            MB.MAST_DISTRICT_CODE,
                                            MB.MAST_BLOCK_CODE,
                                            item.IS_FINALIZED
                                        }).ToList();


                BlockList = BlockList.OrderBy(x => x.MAST_BLOCK_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                return BlockList.Select(obj => new
                {
                    cell = new[]
                    {
                        obj.MAST_DISTRICT_NAME.ToString(),
                        obj.MAST_BLOCK_NAME.ToString(),
                        
                        //(dbContext.FACILITY_HABITATION_MAPPING.Any(x => x.MASTER_BLOCK_CODE == obj.MAST_BLOCK_CODE))?
                        //(isFinalizedcheck.Where(x => x.MAST_BLOCK_CODE == obj.MAST_BLOCK_CODE && x.IS_FINALIZED == "Y").Any())?
                        (dbContext.MAST_FACILITY_BLOCK_PMGSY3_FINALIZE.Where(x => x.MAST_BLOCK_CODE == obj.MAST_BLOCK_CODE && x.IS_FINALIZED == "Y").Any())?
                        "<a href='#' title='Click here to Download Facility List' class='ui-icon  ui-icon-arrowthickstop-1-s ui-align-center' onClick='DownloadFacilityList(\"" + 
                        obj.MAST_BLOCK_CODE + "\"); return false;'>Download</a>" : "<span style='color:red;'>Facility details not finalized</span>",
                        
                        #region for PDF upload condtions
                        (PMGSYSession.Current.RoleCode == 2 || PMGSYSession.Current.RoleCode == 36)?

                        //(! dbContext.FACILITY_HABITATION_MAPPING.Any(x => x.MASTER_BLOCK_CODE == obj.MAST_BLOCK_CODE))? 
                        //(!isFinalizedcheck.Where(x => x.MAST_BLOCK_CODE == obj.MAST_BLOCK_CODE && x.IS_FINALIZED == "Y").Any())? 
                        (!dbContext.MAST_FACILITY_BLOCK_PMGSY3_FINALIZE.Where(x => x.MAST_BLOCK_CODE == obj.MAST_BLOCK_CODE && x.IS_FINALIZED == "Y").Any())?
                        "<span style='color:red;'>File cannot be uploaded as Facility details not finalized</span>"
                        :

                        (!dbContext.MAST_DRRP_BLOCK_PMGSY3_FINALIZE.Where(objBlock => objBlock.MAST_BLOCK_CODE == obj.MAST_BLOCK_CODE).Any()) ? "<span style='color:red;'>Block is not finalized under DRRP</span>" 
                        :
                        (dbContext.MAST_TRACEFILE_PMGSY3.Where(x => x.MAST_BLOCK_CODE == obj.MAST_BLOCK_CODE).Select(x => x.TRACEFILE_NAME_PDF).FirstOrDefault() == null) ?

                        "<a href='#' title='Click here to upload PDF' class='ui-icon ui-icon-plusthick ui-align-center' onClick='UploadPDF(\"" + obj.MAST_BLOCK_CODE + "\"); return false;'>Upload</a>"
                        :"<a href='#' title='Click here to view PDF' class='ui-icon ui-icon-zoomin ui-align-center' onClick='UploadPDF(\"" + obj.MAST_BLOCK_CODE + "\"); return false;'>Upload</a>"
                        :
                        (dbContext.MAST_TRACEFILE_PMGSY3.Where(x => x.MAST_BLOCK_CODE == obj.MAST_BLOCK_CODE).Select(x => x.TRACEFILE_NAME_PDF).FirstOrDefault() != null) ?
                        "<a href='#' title='Click here to view PDF' class='ui-icon ui-icon-zoomin ui-align-center' onClick='UploadPDF(\"" + obj.MAST_BLOCK_CODE + "\"); return false;'>Upload</a>": "<span style='color:red;'>PDF File not uploaded</span>" ,
                        #endregion
                        
                        #region for CSV upload condtions
                        (PMGSYSession.Current.RoleCode == 2 || PMGSYSession.Current.RoleCode == 36) ?
                        //(! dbContext.FACILITY_HABITATION_MAPPING.Any(x => x.MASTER_BLOCK_CODE == obj.MAST_BLOCK_CODE))? 
                        //(!isFinalizedcheck.Where(x => x.MAST_BLOCK_CODE == obj.MAST_BLOCK_CODE && x.IS_FINALIZED == "Y").Any())? 
                        (!dbContext.MAST_FACILITY_BLOCK_PMGSY3_FINALIZE.Where(x => x.MAST_BLOCK_CODE == obj.MAST_BLOCK_CODE && x.IS_FINALIZED == "Y").Any())?
                        "<span style='color:red;'>File cannot be uploaded as Facility details not finalized</span>"
                        :
                        (!dbContext.MAST_DRRP_BLOCK_PMGSY3_FINALIZE.Where(objBlock => objBlock.MAST_BLOCK_CODE == obj.MAST_BLOCK_CODE).Any()) ? "<span style='color:red;'>Block is not finalized under DRRP</span>" 
                        :
                        (dbContext.MAST_TRACEFILE_PMGSY3.Where(x => x.MAST_BLOCK_CODE == obj.MAST_BLOCK_CODE).Select(x => x.TRACEFILE_NAME_CSV).FirstOrDefault() == null) ?
                        "<a href='#' title='Click here to upload CSV' class='ui-icon ui-icon-plusthick ui-align-center' onClick='UploadCSV(\"" + obj.MAST_BLOCK_CODE + "\"); return false;'>Upload</a>"
                        :"<a href='#' title='Click here to view CSV' class='ui-icon ui-icon-zoomin ui-align-center' onClick='UploadCSV(\"" + obj.MAST_BLOCK_CODE + "\"); return false;'>Upload</a>"
                        :
                        (dbContext.MAST_TRACEFILE_PMGSY3.Where(x => x.MAST_BLOCK_CODE == obj.MAST_BLOCK_CODE).Select(x => x.TRACEFILE_NAME_CSV).FirstOrDefault() != null) ?
                        "<a href='#' title='Click here to view CSV' class='ui-icon ui-icon-zoomin ui-align-center' onClick='UploadCSV(\"" + obj.MAST_BLOCK_CODE + "\"); return false;'>Upload</a>" : "<span style='color:red;'>CSV File not uploaded</span>",
                        #endregion

                    //     "<a href='#' title='Click here to upload Habitation CSV' class='ui-icon ui-icon-plusthick ui-align-center' onClick='UploadHabCSV(\"" + obj.MAST_BLOCK_CODE + "\"); return false;'>Upload</a>",


                         #region Hab CSV Upload 01 May 2020
                        (PMGSYSession.Current.RoleCode == 2 || PMGSYSession.Current.RoleCode == 36) ?
          
                        (!dbContext.MAST_FACILITY_BLOCK_PMGSY3_FINALIZE.Where(x => x.MAST_BLOCK_CODE == obj.MAST_BLOCK_CODE && x.IS_FINALIZED == "Y").Any())?
                        "<span style='color:red;'>File cannot be uploaded as Facility details not finalized</span>"
                        :
                        (!dbContext.MAST_DRRP_BLOCK_PMGSY3_FINALIZE.Where(objBlock => objBlock.MAST_BLOCK_CODE == obj.MAST_BLOCK_CODE).Any()) ? "<span style='color:red;'>Block is not finalized under DRRP</span>" 
                        :
                        (dbContext.MAST_HAB_CSV_PMGSY3.Where(x => x.MAST_BLOCK_CODE == obj.MAST_BLOCK_CODE).Select(x => x.MAST_HAB_CSV_FILE_NAME).FirstOrDefault() == null) ?
                        "<a href='#' title='Click here to upload Habitation CSV' class='ui-icon ui-icon-plusthick ui-align-center' onClick='UploadHabCSV(\"" + obj.MAST_BLOCK_CODE + "\"); return false;'>Upload</a>"
                        :"<a href='#' title='Click here to view Habitation CSV' class='ui-icon ui-icon-zoomin ui-align-center' onClick='UploadHabCSV(\"" + obj.MAST_BLOCK_CODE + "\"); return false;'>Upload</a>"
                        :
                        (dbContext.MAST_HAB_CSV_PMGSY3.Where(x => x.MAST_BLOCK_CODE == obj.MAST_BLOCK_CODE).Select(x => x.MAST_HAB_CSV_FILE_NAME).FirstOrDefault() != null) ?
                        "<a href='#' title='Click here to view Habitation CSV' class='ui-icon ui-icon-zoomin ui-align-center' onClick='UploadHabCSV(\"" + obj.MAST_BLOCK_CODE + "\"); return false;'>Upload</a>" : "<span style='color:red;'>CSV File not uploaded</span>",
                        #endregion


                       #region Finalization
                        (PMGSYSession.Current.RoleCode == 22 && dbContext.MAST_TRACEFILE_PMGSY3.Where(x => x.MAST_BLOCK_CODE == obj.MAST_BLOCK_CODE 
                            && x.TRACEFILE_NAME_PDF !=null 
                            && x.TRACEFILE_NAME_CSV != null 
                            && x.TRACEFILE_FINALIZE == "N").Any()) ? (dbContext.MAST_HAB_CSV_PMGSY3.Where(x => x.MAST_BLOCK_CODE == obj.MAST_BLOCK_CODE && x.MAST_HAB_CSV_FILE_FINALIZED == "Y").Any()?

                                      "<a href='#' title='Click here to Finalize the File Details' class='ui-icon ui-icon-plusthick ui-align-center' onClick=FinalisePDFDetails('" +obj.MAST_BLOCK_CODE+ "'); return false;>Finalise</a>"
                                      :"Habitation CSV is not Finalized By PIU"
                                      )
                                      
                                      //: (PMGSYSession.Current.RoleCode == 36 ? "Trace Map not yet Finalized" : "-"),
                                        : (PMGSYSession.Current.RoleCode == 36 && dbContext.MAST_TRACEFILE_PMGSY3.Where(x => x.MAST_BLOCK_CODE == obj.MAST_BLOCK_CODE && x.TRACEFILE_FINALIZE == "Y").Any() ? "Trace Map Finalized" : 
                                        dbContext.MAST_TRACEFILE_PMGSY3.Where(x => x.MAST_BLOCK_CODE == obj.MAST_BLOCK_CODE && x.TRACEFILE_FINALIZE == "Y").Any() ? "Trace Map Finalized" : 
                                        "Trace Map not yet Finalized")
#endregion

                    }


                }).ToArray();

            }
            catch (Exception ex)
            {
                totalRecords = BlockList.Count();
                ErrorLog.LogError(ex, "ExistingRoadsDAL/GetDistrictListDAL");
                return null;
            }
            finally
            {
                if (dbContext != null)
                    dbContext.Dispose();
            }
        }

        public string TraceMapsSaveFileDetailsDAL(List<FileUploadModel> lst_files, ref string filename)
        {
            try
            {
                var itemToInsert = lst_files.FirstOrDefault();
                MAST_TRACEFILE_PMGSY3 DBModel = new MAST_TRACEFILE_PMGSY3();
                dbContext = new PMGSYEntities();

                var isEntryPresent = dbContext.MAST_TRACEFILE_PMGSY3.Where(x => x.MAST_BLOCK_CODE == itemToInsert.BlockCode).FirstOrDefault();

                if (isEntryPresent != null)
                {
                    filename = isEntryPresent.MAST_BLOCK_CODE + "_" + isEntryPresent.MAST_TRACEFILE_ID + Path.GetExtension(itemToInsert.name).ToString();
                    isEntryPresent.TRACEFILE_NAME_PDF = filename;
                    isEntryPresent.TRACEFILE_PDF_UPLOAD_DATE = System.DateTime.Now;
                    dbContext.Entry(isEntryPresent).State = System.Data.Entity.EntityState.Modified;
                }
                else
                {
                    Int32? MaxID;
                    if (dbContext.MAST_TRACEFILE_PMGSY3.Count() == 0)
                    {
                        MaxID = 0;
                    }
                    else
                    {
                        MaxID = (from c in dbContext.MAST_TRACEFILE_PMGSY3 select (Int32?)c.MAST_TRACEFILE_ID ?? 0).Max();
                    }
                    ++MaxID;


                    DBModel.MAST_TRACEFILE_ID = Convert.ToInt32(MaxID);
                    DBModel.MAST_BLOCK_CODE = itemToInsert.BlockCode;
                    DBModel.TRACEFILE_NAME_PDF = itemToInsert.name;
                    DBModel.TRACEFILE_NAME_CSV = null;
                    DBModel.TRACEFILE_PDF_UPLOAD_DATE = System.DateTime.Now;
                    DBModel.TRACEFILE_FINALIZE = "N";
                    DBModel.USERID = PMGSYSession.Current.UserId;
                    DBModel.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                    dbContext = new PMGSYEntities();
                    dbContext.MAST_TRACEFILE_PMGSY3.Add(DBModel);

                }
                dbContext.SaveChanges();
                return string.Empty;
            }
            catch (DbUpdateException ex)
            {
                return ("An Error Occurred While Processing Your Request.");
            }
            catch (OptimisticConcurrencyException ex)
            {
                return ("An Error Occurred While Processing Your Request.");
            }
            catch (Exception ex)
            {
                return ("Error Occurred While Processing Request.");
            }
            finally
            {
                dbContext.Dispose();
            }
        }

        public Array GetTraceMapsFilesListDAL(int page, int rows, string sidx, string sord, out int totalRecords, int blockcode)
        {
            dbContext = new PMGSYEntities();
            var record = dbContext.MAST_TRACEFILE_PMGSY3.Where(x => x.MAST_BLOCK_CODE == blockcode && x.TRACEFILE_NAME_PDF != null).ToList();

            totalRecords = record.Count();

            string VirtualDirectoryUrl = string.Empty;
            string PhysicalPath = string.Empty;

            VirtualDirectoryUrl = ConfigurationManager.AppSettings["TRACE_MAPS_FILE_UPLOAD_PDF_VIRTUAL_DIR_PATH"];
            PhysicalPath = ConfigurationManager.AppSettings["TRACE_MAPS_FILE_UPLOAD_PDF"];

            return record.Select(fileDetails => new
            {
                id = fileDetails.MAST_TRACEFILE_ID + "$" + fileDetails.MAST_BLOCK_CODE,
                cell = new[] {       
                    
                                       URLEncrypt.EncryptParameters(new string[] { fileDetails.TRACEFILE_NAME_PDF + "$" +  fileDetails.MAST_TRACEFILE_ID}),                                                                                                      
                                       (fileDetails.TRACEFILE_FINALIZE == "N" && (PMGSYSession.Current.RoleCode == 2 || PMGSYSession.Current.RoleCode == 36)) ?
                                       "<a href='#' title='Click here to delete the File and File Details' class='ui-icon ui-icon-trash ui-align-center' onClick=DeletePDFFileDetails('" +             fileDetails.MAST_TRACEFILE_ID.ToString().Trim() + "$" +  fileDetails.MAST_BLOCK_CODE +"'); return false;'>Delete</a>"
                                       : "<a href='#' class='ui-icon ui-icon-locked ui-align-center'>",

                                       fileDetails.TRACEFILE_FINALIZE == "N" ? "No" : "Yes",
                                       
                                      (PMGSYSession.Current.RoleCode == 22) ? 
                                      "<a href='#' title='Click here to Finalize the File Details' class='ui-icon ui-icon-plusthick ui-align-center' onClick=FinalisePDFDetails('" +  fileDetails.MAST_TRACEFILE_ID.ToString                                            ().Trim()+ "'); return false;>Finalise</a>"
                                        : "-",
            }
            }).ToArray();

        }

        public Array GetTraceCSVFilesListDAL(int page, int rows, string sidx, string sord, out int totalRecords, int blockcode)
        {
            dbContext = new PMGSYEntities();
            var record = dbContext.MAST_TRACEFILE_PMGSY3.Where(x => x.MAST_BLOCK_CODE == blockcode && x.TRACEFILE_NAME_CSV != null).ToList();

            totalRecords = record.Count();

            string VirtualDirectoryUrl = string.Empty;
            string PhysicalPath = string.Empty;

            VirtualDirectoryUrl = ConfigurationManager.AppSettings["TRACE_MAPS_FILE_UPLOAD_PDF_VIRTUAL_DIR_PATH"];
            PhysicalPath = ConfigurationManager.AppSettings["DRRP_TRACE_CSV_PATH"];

            return record.Select(fileDetails => new
            {
                id = fileDetails.MAST_TRACEFILE_ID + "$" + fileDetails.MAST_BLOCK_CODE,
                cell = new[] {       
                    
                                       URLEncrypt.EncryptParameters(new string[] { fileDetails.TRACEFILE_NAME_CSV + "$" +  fileDetails.MAST_TRACEFILE_ID}),      
                                       
                                                         
                                       (fileDetails.TRACEFILE_FINALIZE == "N" && (PMGSYSession.Current.RoleCode == 2 || PMGSYSession.Current.RoleCode == 36)) ?
                                       "<a href='#' title='Click here to delete the File and File Details' class='ui-icon ui-icon-trash ui-align-center' onClick=DeleteCSVFileDetails('" + fileDetails.MAST_TRACEFILE_ID.ToString().Trim() + "&" +                                                fileDetails.MAST_BLOCK_CODE +"'); return false;'>Delete</a>"
                                       : "<a href='#' class='ui-icon ui-icon-locked ui-align-center'>",





                                       fileDetails.TRACEFILE_FINALIZE == "N" ? "No" : "Yes",
                                      
                                       (PMGSYSession.Current.RoleCode == 22) ? 
                                      "<a href='#' title='Click here to Finalize the File Details' class='ui-icon ui-icon-plusthick ui-align-center' onClick=FinaliseCSVDetails('" +        fileDetails.MAST_TRACEFILE_ID.ToString().Trim()+ "'); return false;>Finalise</a>"
                                        : "-",
            }
            }).ToArray();

        }

        public bool UploadCSVDAL(HttpPostedFileBase fileSrc, string sep, ref string message, int blockcode)
        {
            System.IO.Stream MyStream;
            bool isBulkCoiped = false;
            bool flag = false;
            bool header = true;
            SqlConnection cn = null;
            //int traceFileId = 0;
            MAST_TRACEFILE_PMGSY3 DBModel = new MAST_TRACEFILE_PMGSY3();
            int MAST_TRACE_DRRP_ID = 0;
            try
            {
                var TraceFileID = 0;
                dbContext = new PMGSYEntities();

                TransactionOptions transactionOptions = new TransactionOptions();
                transactionOptions.IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted;
                transactionOptions.Timeout = TimeSpan.MaxValue;

                using (TransactionScope ts = new TransactionScope(TransactionScopeOption.Required, transactionOptions))
                {
                    #region To save file details in omms.MAST_TRACEFILE_PMGSY3

                    var isEntryPresent = dbContext.MAST_TRACEFILE_PMGSY3.Where(x => x.MAST_BLOCK_CODE == blockcode).FirstOrDefault();

                    var filename = string.Empty;

                    if (isEntryPresent != null)
                    {
                        TraceFileID = isEntryPresent.MAST_TRACEFILE_ID;
                        filename = blockcode + "_" + isEntryPresent.MAST_TRACEFILE_ID + Path.GetExtension(fileSrc.FileName).ToString();
                        isEntryPresent.TRACEFILE_NAME_CSV = filename;
                        isEntryPresent.TRACEFILE_CSV_UPLOAD_DATE1 = System.DateTime.Now;
                        dbContext.Entry(isEntryPresent).State = System.Data.Entity.EntityState.Modified;
                    }
                    else
                    {
                        Int32? MaxID;
                        if (dbContext.MAST_TRACEFILE_PMGSY3.Count() == 0)
                        {
                            MaxID = 0;
                        }
                        else
                        {
                            MaxID = (from c in dbContext.MAST_TRACEFILE_PMGSY3 select (Int32?)c.MAST_TRACEFILE_ID ?? 0).Max();
                        }
                        ++MaxID;
                        TraceFileID = Convert.ToInt32(MaxID);
                        filename = blockcode + "_" + MaxID + Path.GetExtension(fileSrc.FileName).ToString();
                        DBModel.MAST_TRACEFILE_ID = Convert.ToInt32(MaxID);
                        DBModel.MAST_BLOCK_CODE = blockcode;
                        DBModel.TRACEFILE_NAME_PDF = null;
                        DBModel.TRACEFILE_NAME_CSV = filename;
                        DBModel.TRACEFILE_CSV_UPLOAD_DATE1 = System.DateTime.Now;
                        DBModel.TRACEFILE_FINALIZE = "N";
                        DBModel.USERID = PMGSYSession.Current.UserId;
                        DBModel.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                        //dbContext = new PMGSYEntities();
                        dbContext.MAST_TRACEFILE_PMGSY3.Add(DBModel);
                    }

                    #endregion // to save file details in omms.MAST_TRACEFILE_PMGSY3

                    //traceFileId = 1;//Hardcoded temporary

                    MAST_TRACE_DRRP_ID = (dbContext.MAST_TRACE_DRRP_SCORE_PMGSY3.Any() ? dbContext.MAST_TRACE_DRRP_SCORE_PMGSY3.Max(x => x.MAST_TRACE_DRRP_ID) : 0) + 1;

                    MyStream = fileSrc.InputStream;
                    List<int> listERRoadCode = new List<int>();
                    using (TextFieldParser parser = new TextFieldParser(MyStream))
                    {
                        #region Bulk Upload
                        //DataTable dt = new DataTable("TempTable");
                        //dt.Columns.AddRange(new DataColumn[9] 
                        //                {   
                        //                    new DataColumn("MAST_TRACE_DRRP_ID", typeof(int)),
                        //                    new DataColumn("MAST_BLOCK_CODE", typeof(int)),
                        //                    new DataColumn("MAST_TRACEFILE_ID", typeof(int)),
                        //                    new DataColumn("MAST_ER_ROAD_CODE", typeof(int)),  
                        //                    new DataColumn("MAST_ER_ROAD_LEN", typeof(string)),  
                        //                    new DataColumn("MAST_ER_ROAD_SCORE", typeof(string)),  
                        //                    new DataColumn("MAST_ER_POP_BEN", typeof(string)),
                        //                    new DataColumn("USERID", typeof(string)),
                        //                    new DataColumn("IPADD", typeof(string)),
                        //                });
                        #endregion
                        parser.TextFieldType = Microsoft.VisualBasic.FileIO.FieldType.Delimited;
                        //specify the delimiter  
                        parser.SetDelimiters(sep);
                        while (!parser.EndOfData)
                        {
                            //read Fileds  
                            string[] fields = parser.ReadFields();
                            //Processing row  
                            if (header == true)
                            { //escape first line  
                                header = false;
                            }
                            else
                            {
                                #region Bulk Upload
                                //dt.Rows.Add();
                                //int i = 0;

                                //dt.Rows[dt.Rows.Count - 1][i] = MAST_TRACE_DRRP_ID;
                                //dt.Rows[dt.Rows.Count - 1][i + 1] = string.IsNullOrEmpty(fields[1]) ? 0 : Convert.ToInt32(fields[1]);//MAST_BLOCK_CODE
                                //dt.Rows[dt.Rows.Count - 1][i + 2] = TraceFileID;
                                //dt.Rows[dt.Rows.Count - 1][i + 3] = string.IsNullOrEmpty(fields[0]) || Convert.ToInt32(fields[0]) < 0 ? 0 : Convert.ToInt32(fields[0]);//MAST_ER_ROAD_CODE
                                //dt.Rows[dt.Rows.Count - 1][i + 4] = string.IsNullOrEmpty(fields[3]) ? 0 : Convert.ToDecimal(fields[3]);//MAST_ER_ROAD_LEN
                                //dt.Rows[dt.Rows.Count - 1][i + 5] = string.IsNullOrEmpty(fields[6]) ? 0 : Convert.ToInt32(fields[6]);//MAST_ER_ROAD_SCORE
                                //dt.Rows[dt.Rows.Count - 1][i + 6] = string.IsNullOrEmpty(fields[5]) ? 0 : Convert.ToInt32(fields[5]);//MAST_ER_POP_BEN
                                //dt.Rows[dt.Rows.Count - 1][i + 7] = PMGSYSession.Current.UserId;
                                //dt.Rows[dt.Rows.Count - 1][i + 8] = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];

                                //MAST_TRACE_DRRP_ID++;
                                //listERRoadCode.Add(string.IsNullOrEmpty(fields[0]) ? 0 : Convert.ToInt32(fields[1]));
                                #endregion

                                listERRoadCode.Add(string.IsNullOrEmpty(fields[1]) ? 0 : Convert.ToInt32(fields[1]));

                                MAST_TRACE_DRRP_SCORE_PMGSY3 mast_trace_drrp_score_pmgsy3 = new MAST_TRACE_DRRP_SCORE_PMGSY3();
                                mast_trace_drrp_score_pmgsy3.MAST_TRACE_DRRP_ID = MAST_TRACE_DRRP_ID;
                                mast_trace_drrp_score_pmgsy3.MAST_BLOCK_CODE = string.IsNullOrEmpty(fields[1]) ? 0 : Convert.ToInt32(fields[1]);
                                mast_trace_drrp_score_pmgsy3.MAST_TRACEFILE_ID = TraceFileID;
                                //mast_trace_drrp_score_pmgsy3.MAST_ER_ROAD_CODE = string.IsNullOrEmpty(fields[0]) ? null ? 0 : Convert.ToInt32(fields[0]) <=0 ? null : Convert.ToInt32(fields[0]);
                                if (string.IsNullOrEmpty(fields[0]))
                                {
                                    mast_trace_drrp_score_pmgsy3.MAST_ER_ROAD_CODE = null;
                                }
                                else
                                {
                                    if ((Convert.ToInt32(fields[0]) <= 0))
                                    {
                                        mast_trace_drrp_score_pmgsy3.MAST_ER_ROAD_CODE = null;
                                    }
                                    else
                                    {
                                        mast_trace_drrp_score_pmgsy3.MAST_ER_ROAD_CODE = Convert.ToInt32(fields[0]);
                                    }
                                }
                                mast_trace_drrp_score_pmgsy3.MAST_ER_ROAD_LEN = string.IsNullOrEmpty(fields[3]) ? 0 : Convert.ToDecimal(fields[3]);//MAST_ER_ROAD_LEN
                                mast_trace_drrp_score_pmgsy3.MAST_ER_ROAD_SCORE = string.IsNullOrEmpty(fields[6]) ? 0 : Convert.ToDecimal(fields[6]);//MAST_ER_ROAD_SCORE
                                mast_trace_drrp_score_pmgsy3.MAST_ER_POP_BEN = string.IsNullOrEmpty(fields[5]) ? 0 : Convert.ToInt32(fields[5].Contains('.') ? fields[5].Substring(0, fields[5].IndexOf('.')) : fields[5]);//MAST_ER_POP_BEN
                                mast_trace_drrp_score_pmgsy3.USERID = PMGSYSession.Current.UserId;
                                mast_trace_drrp_score_pmgsy3.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                                dbContext.MAST_TRACE_DRRP_SCORE_PMGSY3.Add(mast_trace_drrp_score_pmgsy3);
                                MAST_TRACE_DRRP_ID++;
                            }
                        }
                        #region Bulk Upload
                        //string connString = ConfigurationManager.ConnectionStrings["PMGSYConnection"].ConnectionString;
                        //cn = new SqlConnection(connString);
                        //if (cn.State == ConnectionState.Closed)
                        //{
                        //    cn.Open();
                        //}
                        #endregion

                        bool isRoadCodePresent = false;
                        foreach (var item in listERRoadCode)
                        {
                            isRoadCodePresent = !(item == blockcode);
                            if (isRoadCodePresent)
                            {
                                flag = false;
                                message = "Invalid File, Please upload valid file for the selected block";
                                return false;
                            }
                        }

                        #region  Bulk Upload
                        //if (!dbContext.MAST_TRACEFILE_PMGSY3.Any(x => x.MAST_TRACEFILE_ID == TraceFileID))
                        //{
                        //    message = "No records present against the File.";
                        //    return false;
                        //}

                        //SqlBulkCopy sbc = new SqlBulkCopy(cn);
                        //sbc.DestinationTableName = "OMMS.MAST_TRACE_DRRP_SCORE_PMGSY3";
                        //sbc.WriteToServer(dt);
                        //isBulkCoiped = true;
                        #endregion

                        dbContext.SaveChanges();

                        if (fileSrc.ContentLength > 0)
                        {
                            fileSrc.SaveAs(Path.Combine(ConfigurationManager.AppSettings["DRRP_TRACE_CSV_PATH"], filename));
                            flag = true;
                            message = "PFMS Contractor mapped successfully";
                        }
                        else
                        {
                            message = "File Content invalid";
                        }
                        ts.Complete();
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "UploadCSVDAL");
                message = "Error occurred while CSV upload";
            }
            finally
            {
                if (cn != null)
                {
                    cn.Dispose();
                }
            }
            return flag;
        }
        #endregion

        #region List ER ITNO
        public Array ListExistingRoadsITNO(int stateCode, int districtCode, int blockCode, int categoryCode, int ownerCode, int? page, int? rows, string sidx, string sord, out long totalRecords, string filters)
        {
            bool isPMGSY3 = false;
            try
            {
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
                #region Old Logic
                //var lstExistingRoadsDetails = (from existingRoads in dbContext.MASTER_EXISTING_ROADS
                //                               join roadCatCode in dbContext.MASTER_ROAD_CATEGORY
                //                               on existingRoads.MAST_ROAD_CAT_CODE equals roadCatCode.MAST_ROAD_CAT_CODE
                //                               join agency in dbContext.MASTER_AGENCY
                //                               on existingRoads.MAST_ER_ROAD_OWNER equals agency.MAST_AGENCY_CODE

                //                               where ((blockCode == 0 ? 1 : existingRoads.MAST_BLOCK_CODE) == (blockCode == 0 ? 1 : blockCode)) &&
                //                               ((categoryCode == 0 ? 1 : existingRoads.MAST_ROAD_CAT_CODE) == (categoryCode == 0 ? 1 : categoryCode))
                //                               &&(roadName == string.Empty ? "%" : existingRoads.MAST_ER_ROAD_NAME).StartsWith(roadName == string.Empty ? "%" : roadName)     
                //                               select new
                //                               {
                //                                   existingRoads.MAST_ER_ROAD_CODE,
                //                                   existingRoads.MAST_ER_ROAD_NUMBER,
                //                                   existingRoads.MAST_ER_ROAD_NAME,
                //                                   existingRoads.MAST_CORE_NETWORK,
                //                                   agency.MAST_AGENCY_NAME,
                //                                   existingRoads.MAST_ER_ROAD_OWNER,
                //                                   existingRoads.MAST_LOCK_STATUS,

                //                               }).ToList();
                //totalRecords = lstExistingRoadsDetails.Count();

                //if (sidx.Trim() != string.Empty)
                //{
                //    if (sord.ToString() == "asc")
                //    {
                //        switch (sidx)
                //        {
                //            case "MAST_ER_ROAD_NUMBER":
                //                lstExistingRoadsDetails = lstExistingRoadsDetails.OrderBy(x => x.MAST_ER_ROAD_NUMBER).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                //                break;

                //            case "MAST_ER_ROAD_NAME":
                //                lstExistingRoadsDetails = lstExistingRoadsDetails.OrderBy(x => x.MAST_ER_ROAD_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                //                break;
                //            case "MAST_ER_ROAD_OWNER":
                //                lstExistingRoadsDetails = lstExistingRoadsDetails.OrderBy(x => x.MAST_AGENCY_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                //                break;
                //            case "MAST_CORE_NETWORK":
                //                lstExistingRoadsDetails = lstExistingRoadsDetails.OrderBy(x => x.MAST_CORE_NETWORK).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                //                break;
                //            case "MAST_ER_ROAD_CODE":
                //                lstExistingRoadsDetails = lstExistingRoadsDetails.OrderBy(x => x.MAST_ER_ROAD_CODE).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                //                break;
                //            default:
                //                lstExistingRoadsDetails = lstExistingRoadsDetails.OrderBy(x => x.MAST_ER_ROAD_CODE).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                //                break;
                //        }
                //    }
                //    else
                //    {

                //        switch (sidx)
                //        {
                //            case "MAST_ER_ROAD_NUMBER":
                //                lstExistingRoadsDetails = lstExistingRoadsDetails.OrderByDescending(x => x.MAST_ER_ROAD_NUMBER).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                //                break;

                //            case "MAST_ER_ROAD_NAME":
                //                lstExistingRoadsDetails = lstExistingRoadsDetails.OrderByDescending(x => x.MAST_ER_ROAD_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                //                break;

                //            case "MAST_ER_ROAD_OWNER":
                //                lstExistingRoadsDetails = lstExistingRoadsDetails.OrderByDescending(x => x.MAST_AGENCY_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                //                break;
                //            case "MAST_CORE_NETWORK":
                //                lstExistingRoadsDetails = lstExistingRoadsDetails.OrderByDescending(x => x.MAST_CORE_NETWORK).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                //                break;
                //            case "MAST_ER_ROAD_CODE":
                //                lstExistingRoadsDetails = lstExistingRoadsDetails.OrderByDescending(x => x.MAST_ER_ROAD_CODE).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                //                break;
                //            default:
                //                lstExistingRoadsDetails = lstExistingRoadsDetails.OrderByDescending(x => x.MAST_ER_ROAD_CODE).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                //                break;
                //        }
                //    }
                //}
                //else
                //{
                //    lstExistingRoadsDetails = lstExistingRoadsDetails.OrderByDescending(x => x.MAST_ER_ROAD_CODE).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();

                //}

                //return lstExistingRoadsDetails.Select(item => new
                //{
                //    id = item.MAST_ER_ROAD_CODE,
                //    cell = new[]{                          

                //                  item.MAST_ER_ROAD_NUMBER,
                //                  item.MAST_ER_ROAD_NAME,
                //                  item.MAST_AGENCY_NAME,
                //                  item.MAST_CORE_NETWORK.ToString().Trim().ToUpper()=="Y"?"Yes":"No",

                //                  item.MAST_LOCK_STATUS.ToString().ToUpper()=="Y"?string.Empty:URLEncrypt.EncryptParameters1(new string[]{"RoadCode="+item.MAST_ER_ROAD_CODE.ToString().Trim()}),


                //                   item.MAST_LOCK_STATUS.ToString().ToUpper()=="Y"?string.Empty:URLEncrypt.EncryptParameters1(new string[]{"RoadCode="+item.MAST_ER_ROAD_CODE.ToString().Trim()}),   
                //                   item.MAST_LOCK_STATUS.ToString().ToUpper()=="Y"?string.Empty:URLEncrypt.EncryptParameters1(new string[]{"RoadCode="+item.MAST_ER_ROAD_CODE.ToString().Trim()}),
                //                   item.MAST_LOCK_STATUS.ToString().ToUpper()=="Y"?string.Empty:URLEncrypt.EncryptParameters1(new string[]{"RoadCode="+item.MAST_ER_ROAD_CODE.ToString().Trim()}),
                //                   item.MAST_LOCK_STATUS.ToString().ToUpper()=="Y"?string.Empty:URLEncrypt.EncryptParameters1(new string[]{"RoadCode="+item.MAST_ER_ROAD_CODE.ToString().Trim()}),    
                //                   URLEncrypt.EncryptParameters1(new string[]{"RoadCode="+item.MAST_ER_ROAD_CODE.ToString().Trim()}),
                //                   item.MAST_LOCK_STATUS.ToString().ToUpper()=="Y"?string.Empty:URLEncrypt.EncryptParameters1(new string[]{"RoadCode="+item.MAST_ER_ROAD_CODE.ToString().Trim()}),    
                //                   item.MAST_LOCK_STATUS.ToString().ToUpper()=="Y"?string.Empty:URLEncrypt.EncryptParameters1(new string[]{"RoadCode="+item.MAST_ER_ROAD_CODE.ToString().Trim()})

                //            }
                //}).ToArray();
                #endregion

                if (PMGSYSession.Current.RoleCode == 2)
                {
                    stateCode = PMGSYSession.Current.StateCode;
                }
                else if (PMGSYSession.Current.RoleCode == 22 || PMGSYSession.Current.RoleCode == 54)///Changes for RCPLWE
                {
                    stateCode = PMGSYSession.Current.StateCode;
                    districtCode = PMGSYSession.Current.DistrictCode;
                }
                ///PMGSY3
                isPMGSY3 = dbContext.MASTER_PMGSY3.Any(x => x.MAST_STATE_CODE == stateCode && x.MAST_PMGSY3_ACTIVE == "Y");

                short roleCode = dbContext.UM_User_Master.Where(m => m.UserID == PMGSYSession.Current.UserId).Select(m => m.DefaultRoleID).FirstOrDefault();
                //var lstExistingRoadsDetails = dbContext.GET_EXISTING_ROADS((stateCode <= 0 ? 0 : stateCode), (districtCode <= 0 ? 0 : districtCode), (blockCode <= 0 ? 0 : blockCode), categoryCode, roadName, PMGSYSession.Current.PMGSYScheme, roleCode).ToList();
                var lstExistingRoadsDetails = dbContext.GET_EXISTING_ROADS((stateCode <= 0 ? 0 : stateCode), (districtCode <= 0 ? 0 : districtCode), (blockCode <= 0 ? 0 : blockCode), categoryCode, roadName, (PMGSYSession.Current.PMGSYScheme == 3 ? (byte)1 : PMGSYSession.Current.PMGSYScheme), (roleCode == 54 ? (short)22 : roleCode)).ToList();///Changes for RCPLWE
                int unlockCount = dbContext.IMS_UNLOCK_DETAILS.Where(x => x.IMS_UNLOCK_TABLE == "ER" && (x.IMS_UNLOCK_LEVEL == "S" || x.IMS_UNLOCK_LEVEL == "D") && x.MAST_PMGSY_SCHEME == 2 && (x.MAST_STATE_CODE == PMGSYSession.Current.StateCode || x.MAST_DISTRICT_CODE == PMGSYSession.Current.DistrictCode) && x.IMS_UNLOCK_STATUS == "Y" && x.IMS_UNLOCK_END_DATE >= DateTime.Now).Count();

                totalRecords = lstExistingRoadsDetails.Count();

                if (sidx.Trim() != string.Empty)
                {
                    if (sord.ToString() == "asc")
                    {
                        switch (sidx)
                        {
                            case "MAST_ER_ROAD_CODE":
                                lstExistingRoadsDetails = lstExistingRoadsDetails.OrderBy(x => x.MAST_ER_ROAD_CODE).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                            case "MAST_ER_ROAD_NUMBER":
                                lstExistingRoadsDetails = lstExistingRoadsDetails.OrderBy(x => x.MAST_ER_ROAD_NUMBER).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                            case "MAST_ER_ROAD_NAME":
                                lstExistingRoadsDetails = lstExistingRoadsDetails.OrderBy(x => x.MAST_ER_ROAD_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                            case "MAST_ER_ROAD_OWNER":
                                lstExistingRoadsDetails = lstExistingRoadsDetails.OrderBy(x => x.AGENCY_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                            case "MAST_CORE_NETWORK":
                                lstExistingRoadsDetails = lstExistingRoadsDetails.OrderBy(x => x.MAST_CORE_NETWORK).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                            //case "MAST_ER_ROAD_CODE":
                            //    lstExistingRoadsDetails = lstExistingRoadsDetails.OrderBy(x => x.MAST_ER_ROAD_CODE).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                            //break;
                            case "MAST_ROAD_SHORT_DESC":
                                lstExistingRoadsDetails = lstExistingRoadsDetails.OrderBy(x => x.MAST_ROAD_SHORT_DESC).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                            case "MAST_ER_ROAD_TYPE":
                                lstExistingRoadsDetails = lstExistingRoadsDetails.OrderBy(x => x.MAST_ER_ROAD_TYPE).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                            default:
                                lstExistingRoadsDetails = lstExistingRoadsDetails.OrderBy(x => x.MAST_ER_ROAD_CODE).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                        }
                    }
                    else
                    {

                        switch (sidx)
                        {
                            case "MAST_ER_ROAD_CODE":
                                lstExistingRoadsDetails = lstExistingRoadsDetails.OrderByDescending(x => x.MAST_ER_ROAD_CODE).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                            case "MAST_ER_ROAD_NUMBER":
                                lstExistingRoadsDetails = lstExistingRoadsDetails.OrderByDescending(x => x.MAST_ER_ROAD_NUMBER).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                            case "MAST_ER_ROAD_NAME":
                                lstExistingRoadsDetails = lstExistingRoadsDetails.OrderByDescending(x => x.MAST_ER_ROAD_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                            case "MAST_ER_ROAD_OWNER":
                                lstExistingRoadsDetails = lstExistingRoadsDetails.OrderByDescending(x => x.AGENCY_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                            case "MAST_CORE_NETWORK":
                                lstExistingRoadsDetails = lstExistingRoadsDetails.OrderByDescending(x => x.MAST_CORE_NETWORK).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                            //case "MAST_ER_ROAD_CODE":
                            //    lstExistingRoadsDetails = lstExistingRoadsDetails.OrderByDescending(x => x.MAST_ER_ROAD_CODE).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                            //    break;
                            case "MAST_ROAD_SHORT_DESC":
                                lstExistingRoadsDetails = lstExistingRoadsDetails.OrderByDescending(x => x.MAST_ROAD_SHORT_DESC).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                            case "MAST_ER_ROAD_TYPE":
                                lstExistingRoadsDetails = lstExistingRoadsDetails.OrderByDescending(x => x.MAST_ER_ROAD_TYPE).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                            default:
                                lstExistingRoadsDetails = lstExistingRoadsDetails.OrderByDescending(x => x.MAST_ER_ROAD_CODE).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                        }
                    }
                }
                else
                {
                    lstExistingRoadsDetails = lstExistingRoadsDetails.OrderByDescending(x => x.MAST_ER_ROAD_CODE).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                }

                var result = lstExistingRoadsDetails.Select(roadDetails => new
                {
                    roadDetails.MAST_ER_ROAD_CODE,
                    roadDetails.MAST_ROAD_SHORT_DESC,
                    roadDetails.MAST_ER_ROAD_NUMBER,
                    roadDetails.MAST_ER_ROAD_NAME,
                    roadDetails.MAST_ER_ROAD_TYPE,
                    roadDetails.AGENCY_NAME,
                    roadDetails.MAST_CORE_NETWORK,
                    roadDetails.UNLOCK_BY_MORD,
                    //MAST_ER_ROAD_CODE_PMGSY1 = dbContext.MASTER_EXISTING_ROADS.Where(m => m.MAST_ER_ROAD_CODE == roadDetails.MAST_ER_ROAD_CODE).Select(m => m.MAST_ER_ROAD_CODE_PMGSY1).FirstOrDefault(),
                    roadDetails.MAST_ER_ROAD_CODE_PMGSY1,
                    roadDetails.MAST_ROAD_CAT_CODE_PMGSY2,
                    roadDetails.MAST_ROAD_CAT_CODE_PMGSY1,
                }).ToArray();

                return result.Select(item => new
                {

                    id = item.MAST_ER_ROAD_CODE,
                    cell = new[]
                    {       
                        item.MAST_ER_ROAD_CODE.ToString(),
                        item.MAST_ROAD_SHORT_DESC, //Road Category Short Desc
                        item.MAST_ER_ROAD_NUMBER,
                        item.MAST_ER_ROAD_NAME,
                         item.MAST_ER_ROAD_TYPE, //Road Type
                        (dbContext.MASTER_EXISTING_ROADS.Where(m=>m.MAST_ER_ROAD_CODE == item.MAST_ER_ROAD_CODE).Select(m=>m.MAST_ER_ROAD_END_CHAIN).FirstOrDefault() - dbContext.MASTER_EXISTING_ROADS.Where(m=>m.MAST_ER_ROAD_CODE == item.MAST_ER_ROAD_CODE).Select(m=>m.MAST_ER_ROAD_STR_CHAIN).FirstOrDefault()).ToString(),
                        item.AGENCY_NAME,
                        item.MAST_CORE_NETWORK.ToString().Trim().ToUpper()=="Y"?"Yes":"No",
                        
                        #region
                        dbContext.MASTER_EXISTING_ROADS.Where(m=>m.MAST_ER_ROAD_CODE == item.MAST_ER_ROAD_CODE).Select(m=>m.MAST_LOCK_STATUS).FirstOrDefault() == "N"?"<a href='#' title='Click here to add CDWorks details' class='ui-icon ui-icon-plusthick ui-align-center' onClick =CDWorks('"+URLEncrypt.EncryptParameters1(new string[]{"RoadCode =" + item.MAST_ER_ROAD_CODE.ToString().Trim()})+"'); return false;'></a>":item.UNLOCK_BY_MORD == "M"?"<a href='#' title='Click here to add CDWorks details' class='ui-icon ui-icon-unlocked ui-align-center' onClick =CDWorks('"+URLEncrypt.EncryptParameters1(new string[]{"RoadCode =" + item.MAST_ER_ROAD_CODE.ToString().Trim()})+"'); return false;'></a>":item.UNLOCK_BY_MORD.ToString().ToUpper()=="Y"?"<span class='ui-icon ui-icon-locked ui-align-center'></span>":"<a href='#' title='Click here to add CDWorks details' class='ui-icon ui-icon-plusthick ui-align-center' onClick =CDWorks('"+URLEncrypt.EncryptParameters1(new string[]{"RoadCode =" + item.MAST_ER_ROAD_CODE.ToString().Trim()})+"'); return false;'></a>",   
                        
                        dbContext.MASTER_EXISTING_ROADS.Where(m=>m.MAST_ER_ROAD_CODE == item.MAST_ER_ROAD_CODE).Select(m=>m.MAST_LOCK_STATUS).FirstOrDefault() == "N"?"<a href='#' title='Click here to add surface type details' class='ui-icon ui-icon-plusthick ui-align-center' onClick =SurfaceTypes('"+URLEncrypt.EncryptParameters1(new string[]{"RoadCode =" + item.MAST_ER_ROAD_CODE.ToString().Trim()})+"'); return false;'></a>":item.UNLOCK_BY_MORD == "M"?"<a href='#' title='Click here to add surface type details' class='ui-icon ui-icon-unlocked ui-align-center' onClick =SurfaceTypes('"+URLEncrypt.EncryptParameters1(new string[]{"RoadCode =" + item.MAST_ER_ROAD_CODE.ToString().Trim()})+"'); return false;'></a>":item.UNLOCK_BY_MORD.ToString().ToUpper()=="Y"?"<span class='ui-icon ui-icon-locked ui-align-center'></span>":"<a href='#' title='Click here to add surface type details' class='ui-icon ui-icon-plusthick ui-align-center' onClick =SurfaceTypes('"+URLEncrypt.EncryptParameters1(new string[]{"RoadCode =" + item.MAST_ER_ROAD_CODE.ToString().Trim()})+"'); return false;'></a>",   
                        
                        dbContext.MASTER_EXISTING_ROADS.Where(m=>m.MAST_ER_ROAD_CODE == item.MAST_ER_ROAD_CODE).Select(m=>m.MAST_LOCK_STATUS).FirstOrDefault() == "N"?"<a href='#' title='Click here to add habitation details' class='ui-icon ui-icon-plusthick ui-align-center' onClick =HabitationsMapped('"+URLEncrypt.EncryptParameters1(new string[]{"RoadCode =" + item.MAST_ER_ROAD_CODE.ToString().Trim()})+"'); return false;'></a>":item.UNLOCK_BY_MORD == "M"?"<a href='#' title='Click here to add Habitation details' class='ui-icon ui-icon-unlocked ui-align-center' onClick =HabitationsMapped('"+URLEncrypt.EncryptParameters1(new string[]{"RoadCode =" + item.MAST_ER_ROAD_CODE.ToString().Trim()})+"'); return false;'></a>":item.UNLOCK_BY_MORD.ToString().ToUpper()=="Y"?"<span class='ui-icon ui-icon-locked ui-align-center'></span>":"<a href='#' title='Click here to add habitation details' class='ui-icon ui-icon-plusthick ui-align-center' onClick =HabitationsMapped('"+URLEncrypt.EncryptParameters1(new string[]{"RoadCode =" + item.MAST_ER_ROAD_CODE.ToString().Trim()})+"'); return false;'></a>",   
                       
                        dbContext.MASTER_EXISTING_ROADS.Where(m=>m.MAST_ER_ROAD_CODE == item.MAST_ER_ROAD_CODE).Select(m=>m.MAST_LOCK_STATUS).FirstOrDefault() == "N"?"<a href='#' title='Click here to add traffic intensity details' class='ui-icon ui-icon-plusthick ui-align-center' onClick =TrafficIntensity('"+URLEncrypt.EncryptParameters1(new string[]{"RoadCode =" + item.MAST_ER_ROAD_CODE.ToString().Trim()})+"'); return false;'></a>":item.UNLOCK_BY_MORD == "M"?"<a href='#' title='Click here to add Traffic Intensity details' class='ui-icon ui-icon-unlocked ui-align-center' onClick =TrafficIntensity('"+URLEncrypt.EncryptParameters1(new string[]{"RoadCode =" + item.MAST_ER_ROAD_CODE.ToString().Trim()})+"'); return false;'></a>":item.UNLOCK_BY_MORD.ToString().ToUpper()=="Y"?"<span class='ui-icon ui-icon-locked ui-align-center'></span>":"<a href='#' title='Click here to add traffic intensity details' class='ui-icon ui-icon-plusthick ui-align-center' onClick =TrafficIntensity('"+URLEncrypt.EncryptParameters1(new string[]{"RoadCode =" + item.MAST_ER_ROAD_CODE.ToString().Trim()})+"'); return false;'></a>",   
                        
                        dbContext.MASTER_EXISTING_ROADS.Where(m=>m.MAST_ER_ROAD_CODE == item.MAST_ER_ROAD_CODE).Select(m=>m.MAST_LOCK_STATUS).FirstOrDefault() == "N"?"<a href='#' title='Click here to add CBR details' class='ui-icon ui-icon-plusthick ui-align-center' onClick =CBRValue('"+URLEncrypt.EncryptParameters1(new string[]{"RoadCode =" + item.MAST_ER_ROAD_CODE.ToString().Trim()})+"'); return false;'></a>":item.UNLOCK_BY_MORD == "M"?"<a href='#' title='Click here to add CBR details' class='ui-icon ui-icon-unlocked ui-align-center' onClick =CBRValue('"+URLEncrypt.EncryptParameters1(new string[]{"RoadCode =" + item.MAST_ER_ROAD_CODE.ToString().Trim()})+"'); return false;'></a>":item.UNLOCK_BY_MORD.ToString().ToUpper()=="Y"?"<span class='ui-icon ui-icon-locked ui-align-center'></span>":"<a href='#' title='Click here to add CBR details' class='ui-icon ui-icon-plusthick ui-align-center' onClick =CBRValue('"+URLEncrypt.EncryptParameters1(new string[]{"RoadCode =" + item.MAST_ER_ROAD_CODE.ToString().Trim()})+"'); return false;'></a>",   
                        "<a href='#' title='Click here to view existing roads details' class='ui-icon ui-icon-zoomin ui-align-center' onClick =ShowDetails('"+URLEncrypt.EncryptParameters1(new string[]{"RoadCode =" + item.MAST_ER_ROAD_CODE.ToString().Trim()})+"'); return false;'></a>",
                        
                        ///Changed by SAMMED A. PATIL on 15FEB2017 for Map PMGSY1 DRRP Roads
                        //(PMGSYSession.Current.PMGSYScheme == 2 && (PMGSYSession.Current.RoleCode == 22 || PMGSYSession.Current.RoleCode == 25)) 
                        //? //(dbContext.MASTER_EXISTING_ROADS.Where(m=>m.MAST_ER_ROAD_CODE == item.MAST_ER_ROAD_CODE && m.MAST_ER_ROAD_CODE_PMGSY1 != null).Any() 
                        //((item.MAST_ER_ROAD_CODE_PMGSY1 != 0 && (item.MAST_ROAD_CAT_CODE_PMGSY2 != item.MAST_ROAD_CAT_CODE_PMGSY1)) ? dbContext.MASTER_EXISTING_ROADS.Where(m=>m.MAST_ER_ROAD_CODE == item.MAST_ER_ROAD_CODE_PMGSY1).Select(m=>m.MAST_ER_ROAD_NUMBER).FirstOrDefault().ToString() : item.MAST_ER_ROAD_CODE_PMGSY1 != 0
                        //    ? "<a href='#' onClick =MapDRRPPMGSY1('"+URLEncrypt.EncryptParameters1(new string[]{"BlockCode =" + blockCode.ToString().Trim(), "RoadCode =" + item.MAST_ER_ROAD_CODE.ToString().Trim(), "RoadCategory=" + item.MAST_ROAD_CAT_CODE_PMGSY2.ToString().Trim()})+"'); return false;'>"+ dbContext.MASTER_EXISTING_ROADS.Where(m=>m.MAST_ER_ROAD_CODE == item.MAST_ER_ROAD_CODE_PMGSY1).Select(m=>m.MAST_ER_ROAD_NUMBER).FirstOrDefault().ToString() +"</a>" 
                        //    : "<a href='#' title='Click here to Map DRRP PMGSY - I Road' class='ui-icon ui-icon-plusthick ui-align-center' onClick =MapDRRPPMGSY1('"+URLEncrypt.EncryptParameters1(new string[]{"BlockCode =" + blockCode.ToString().Trim(), "RoadCode =" + item.MAST_ER_ROAD_CODE.ToString().Trim(), "RoadCategory=" + item.MAST_ROAD_CAT_CODE_PMGSY2.ToString().Trim()})+"'); return false;'></a>") 
                        //: "-",
                        
                        ((PMGSYSession.Current.PMGSYScheme == 2 && (PMGSYSession.Current.RoleCode == 22 || PMGSYSession.Current.RoleCode == 25 || PMGSYSession.Current.RoleCode == 25 ))
                        ? (item.MAST_ER_ROAD_CODE_PMGSY1 == 0) 
                            ? "<a href='#' title='Click here to Map DRRP PMGSY - I Road' class='ui-icon ui-icon-plusthick ui-align-center' onClick =MapDRRPPMGSY1('"+URLEncrypt.EncryptParameters1(new string[]{"BlockCode =" + blockCode.ToString().Trim(), "RoadCode =" + item.MAST_ER_ROAD_CODE.ToString().Trim(), "RoadCategory=" + item.MAST_ROAD_CAT_CODE_PMGSY2.ToString().Trim()})+"'); return false;'></a>" 
                            : //(dbContext.MASTER_EXISTING_ROADS.Where(m=>m.MAST_ER_ROAD_CODE == item.MAST_ER_ROAD_CODE_PMGSY1).Select(m=>m.MAST_ER_ROAD_NUMBER).FirstOrDefault())
                              "<a href='#' onClick =MapDRRPPMGSY1('"+URLEncrypt.EncryptParameters1(new string[]{"BlockCode =" + blockCode.ToString().Trim(), "RoadCode =" + item.MAST_ER_ROAD_CODE.ToString().Trim(), "RoadCategory=" + item.MAST_ROAD_CAT_CODE_PMGSY2.ToString().Trim()})+"'); return false;'>"+ dbContext.MASTER_EXISTING_ROADS.Where(m=>m.MAST_ER_ROAD_CODE == item.MAST_ER_ROAD_CODE_PMGSY1).Select(m=>m.MAST_ER_ROAD_NUMBER).FirstOrDefault() +"</a>" 
                        : "-"),

                        //dbContext.MASTER_EXISTING_ROADS.Where(m=>m.MAST_ER_ROAD_CODE == item.MAST_ER_ROAD_CODE).Select(m=>m.MAST_LOCK_STATUS).FirstOrDefault() == "N"?"<a href='#' title='Click here to edit details' class='ui-icon ui-icon-pencil ui-align-center' onClick =EditDetails('"+URLEncrypt.EncryptParameters1(new string[]{"RoadCode =" + item.MAST_ER_ROAD_CODE.ToString().Trim()})+"'); return false;'></a>":item.UNLOCK_BY_MORD == "M"?"<a href='#' title='Click here to edit details' class='ui-icon ui-icon-unlocked ui-align-center' onClick =EditDetails('"+URLEncrypt.EncryptParameters1(new string[]{"RoadCode =" + item.MAST_ER_ROAD_CODE.ToString().Trim(),"UnlockFlag =" + item.UNLOCK_BY_MORD.ToString().Trim()})+"'); return false;'></a>":item.UNLOCK_BY_MORD.ToString().ToUpper()=="Y"?"<span class='ui-icon ui-icon-locked ui-align-center'></span>":"<a href='#' title='Click here to edit details' class='ui-icon ui-icon-pencil ui-align-center' onClick =EditDetails('"+URLEncrypt.EncryptParameters1(new string[]{"RoadCode =" + item.MAST_ER_ROAD_CODE.ToString().Trim()})+"'); return false;'></a>",   
                        ///Changes by SAMMED A. PATIL on 03 OCTOBER 2017 to edit DRRP for MORD
                        PMGSYSession.Current.RoleCode == 25 
                            ? "<a href='#' title='Click here to edit details' class='ui-icon ui-icon-pencil ui-align-center' onClick =EditDetails('"+URLEncrypt.EncryptParameters1(new string[]{"RoadCode =" + item.MAST_ER_ROAD_CODE.ToString().Trim()})+"'); return false;'></a>"
                            : (dbContext.MASTER_EXISTING_ROADS.Where(m=>m.MAST_ER_ROAD_CODE == item.MAST_ER_ROAD_CODE).Select(m=>m.MAST_LOCK_STATUS).FirstOrDefault() == "N"?"<a href='#' title='Click here to edit details' class='ui-icon ui-icon-pencil ui-align-center' onClick =EditDetails('"+URLEncrypt.EncryptParameters1(new string[]{"RoadCode =" + item.MAST_ER_ROAD_CODE.ToString().Trim()})+"'); return false;'></a>":item.UNLOCK_BY_MORD == "M"?"<a href='#' title='Click here to edit details' class='ui-icon ui-icon-unlocked ui-align-center' onClick =EditDetails('"+URLEncrypt.EncryptParameters1(new string[]{"RoadCode =" + item.MAST_ER_ROAD_CODE.ToString().Trim(),"UnlockFlag =" + item.UNLOCK_BY_MORD.ToString().Trim()})+"'); return false;'></a>":item.UNLOCK_BY_MORD.ToString().ToUpper()=="Y"?"<span class='ui-icon ui-icon-locked ui-align-center'></span>":"<a href='#' title='Click here to edit details' class='ui-icon ui-icon-pencil ui-align-center' onClick =EditDetails('"+URLEncrypt.EncryptParameters1(new string[]{"RoadCode =" + item.MAST_ER_ROAD_CODE.ToString().Trim()})+"'); return false;'></a>"),

                        PMGSYSession.Current.PMGSYScheme == 2 
                        ? 
                                (dbContext.MASTER_EXISTING_ROADS.Where(m=>m.MAST_ER_ROAD_CODE == item.MAST_ER_ROAD_CODE).Select(m=>m.MAST_LOCK_STATUS).FirstOrDefault() == "N" || item.UNLOCK_BY_MORD == "M")
                                ? ("<a href='#' title='Click here to delete details' class='ui-icon ui-icon-trash ui-align-center' onClick =DeleteDetailsITNO('"+URLEncrypt.EncryptParameters1(new string[]{"RoadCode =" + item.MAST_ER_ROAD_CODE.ToString().Trim                                   ()})+"'); return false;'></a>")
                                : "<span class='ui-icon ui-icon-locked ui-align-center'></span>"
                        : dbContext.MASTER_EXISTING_ROADS.Where(m=>m.MAST_ER_ROAD_CODE == item.MAST_ER_ROAD_CODE).Select(m=>m.MAST_LOCK_STATUS).FirstOrDefault() == "N"?"<a href='#' title='Click here to delete details' class='ui-icon ui-icon-trash ui-align-center' onClick =DeleteDetailsITNO('"+URLEncrypt.EncryptParameters1(new string[]{"RoadCode =" + item.MAST_ER_ROAD_CODE.ToString().Trim()})+"'); return false;'></a>":item.UNLOCK_BY_MORD == "M"?"<a href='#' title='Click here to delete details' class='ui-icon ui-icon-trash ui-align-center' onClick =DeleteDetailsITNO('"+URLEncrypt.EncryptParameters1(new string[]{"RoadCode =" + item.MAST_ER_ROAD_CODE.ToString().Trim()})+"'); return false;'></a>":item.UNLOCK_BY_MORD.ToString().ToUpper()=="Y"?"<span class='ui-icon ui-icon-locked ui-align-center'></span>":"<a href='#' title='Click here to delete details' class='ui-icon ui-icon-trash ui-align-center' onClick =DeleteDetailsITNO('"+URLEncrypt.EncryptParameters1(new string[]{"RoadCode =" + item.MAST_ER_ROAD_CODE.ToString().Trim()})+"'); return false;'></a>", 
#endregion

#region

                       /* //CD Works
                        (isPMGSY3 == true && PMGSYSession.Current.PMGSYScheme == 2)
                        ? "<span class='ui-icon ui-icon-locked ui-align-center'></span>"
                        : (dbContext.MASTER_EXISTING_ROADS.Where(m=>m.MAST_ER_ROAD_CODE == item.MAST_ER_ROAD_CODE).Select(m=>m.MAST_LOCK_STATUS).FirstOrDefault() == "N"?"<a href='#' title='Click here to add CDWorks details' class='ui-icon ui-icon-plusthick ui-align-center' onClick =CDWorks('"+URLEncrypt.EncryptParameters1(new string[]{"RoadCode =" + item.MAST_ER_ROAD_CODE.ToString().Trim()})+"'); return false;'></a>":item.UNLOCK_BY_MORD == "M"?"<a href='#' title='Click here to add CDWorks details' class='ui-icon ui-icon-unlocked ui-align-center' onClick =CDWorks('"+URLEncrypt.EncryptParameters1(new string[]{"RoadCode =" + item.MAST_ER_ROAD_CODE.ToString().Trim()})+"'); return false;'></a>":item.UNLOCK_BY_MORD.ToString().ToUpper()=="Y"?"<span class='ui-icon ui-icon-locked ui-align-center'></span>":"<a href='#' title='Click here to add CDWorks details' class='ui-icon ui-icon-plusthick ui-align-center' onClick =CDWorks('"+URLEncrypt.EncryptParameters1(new string[]{"RoadCode =" + item.MAST_ER_ROAD_CODE.ToString().Trim()})+"'); return false;'></a>"),   

                        //Surface Types
                        (isPMGSY3 == true && PMGSYSession.Current.PMGSYScheme == 2)
                        ? "<span class='ui-icon ui-icon-locked ui-align-center'></span>"
                        : (dbContext.MASTER_EXISTING_ROADS.Where(m=>m.MAST_ER_ROAD_CODE == item.MAST_ER_ROAD_CODE).Select(m=>m.MAST_LOCK_STATUS).FirstOrDefault() == "N"?"<a href='#' title='Click here to add surface type details' class='ui-icon ui-icon-plusthick ui-align-center' onClick =SurfaceTypes('"+URLEncrypt.EncryptParameters1(new string[]{"RoadCode =" + item.MAST_ER_ROAD_CODE.ToString().Trim()})+"'); return false;'></a>":item.UNLOCK_BY_MORD == "M"?"<a href='#' title='Click here to add surface type details' class='ui-icon ui-icon-unlocked ui-align-center' onClick =SurfaceTypes('"+URLEncrypt.EncryptParameters1(new string[]{"RoadCode =" + item.MAST_ER_ROAD_CODE.ToString().Trim()})+"'); return false;'></a>":item.UNLOCK_BY_MORD.ToString().ToUpper()=="Y"?"<span class='ui-icon ui-icon-locked ui-align-center'></span>":"<a href='#' title='Click here to add surface type details' class='ui-icon ui-icon-plusthick ui-align-center' onClick =SurfaceTypes('"+URLEncrypt.EncryptParameters1(new string[]{"RoadCode =" + item.MAST_ER_ROAD_CODE.ToString().Trim()})+"'); return false;'></a>"),   
                        
                        //Habitations
                        (isPMGSY3 == true && PMGSYSession.Current.PMGSYScheme == 2)
                        ? "<span class='ui-icon ui-icon-locked ui-align-center'></span>"
                        : (dbContext.MASTER_EXISTING_ROADS.Where(m=>m.MAST_ER_ROAD_CODE == item.MAST_ER_ROAD_CODE).Select(m=>m.MAST_LOCK_STATUS).FirstOrDefault() == "N"?"<a href='#' title='Click here to add habitation details' class='ui-icon ui-icon-plusthick ui-align-center' onClick =HabitationsMapped('"+URLEncrypt.EncryptParameters1(new string[]{"RoadCode =" + item.MAST_ER_ROAD_CODE.ToString().Trim()})+"'); return false;'></a>":item.UNLOCK_BY_MORD == "M"?"<a href='#' title='Click here to add Habitation details' class='ui-icon ui-icon-unlocked ui-align-center' onClick =HabitationsMapped('"+URLEncrypt.EncryptParameters1(new string[]{"RoadCode =" + item.MAST_ER_ROAD_CODE.ToString().Trim()})+"'); return false;'></a>":item.UNLOCK_BY_MORD.ToString().ToUpper()=="Y"?"<span class='ui-icon ui-icon-locked ui-align-center'></span>":"<a href='#' title='Click here to add habitation details' class='ui-icon ui-icon-plusthick ui-align-center' onClick =HabitationsMapped('"+URLEncrypt.EncryptParameters1(new string[]{"RoadCode =" + item.MAST_ER_ROAD_CODE.ToString().Trim()})+"'); return false;'></a>"),   

                        //Traffic
                        (isPMGSY3 == true && PMGSYSession.Current.PMGSYScheme == 2)
                        ? "<span class='ui-icon ui-icon-locked ui-align-center'></span>"
                        : (dbContext.MASTER_EXISTING_ROADS.Where(m=>m.MAST_ER_ROAD_CODE == item.MAST_ER_ROAD_CODE).Select(m=>m.MAST_LOCK_STATUS).FirstOrDefault() == "N"?"<a href='#' title='Click here to add traffic intensity details' class='ui-icon ui-icon-plusthick ui-align-center' onClick =TrafficIntensity('"+URLEncrypt.EncryptParameters1(new string[]{"RoadCode =" + item.MAST_ER_ROAD_CODE.ToString().Trim()})+"'); return false;'></a>":item.UNLOCK_BY_MORD == "M"?"<a href='#' title='Click here to add Traffic Intensity details' class='ui-icon ui-icon-unlocked ui-align-center' onClick =TrafficIntensity('"+URLEncrypt.EncryptParameters1(new string[]{"RoadCode =" + item.MAST_ER_ROAD_CODE.ToString().Trim()})+"'); return false;'></a>":item.UNLOCK_BY_MORD.ToString().ToUpper()=="Y"?"<span class='ui-icon ui-icon-locked ui-align-center'></span>":"<a href='#' title='Click here to add traffic intensity details' class='ui-icon ui-icon-plusthick ui-align-center' onClick =TrafficIntensity('"+URLEncrypt.EncryptParameters1(new string[]{"RoadCode =" + item.MAST_ER_ROAD_CODE.ToString().Trim()})+"'); return false;'></a>"),   
                        
                        //CBR
                        (isPMGSY3 == true && PMGSYSession.Current.PMGSYScheme == 2)
                        ? "<span class='ui-icon ui-icon-locked ui-align-center'></span>"
                        : (dbContext.MASTER_EXISTING_ROADS.Where(m=>m.MAST_ER_ROAD_CODE == item.MAST_ER_ROAD_CODE).Select(m=>m.MAST_LOCK_STATUS).FirstOrDefault() == "N"?"<a href='#' title='Click here to add CBR details' class='ui-icon ui-icon-plusthick ui-align-center' onClick =CBRValue('"+URLEncrypt.EncryptParameters1(new string[]{"RoadCode =" + item.MAST_ER_ROAD_CODE.ToString().Trim()})+"'); return false;'></a>":item.UNLOCK_BY_MORD == "M"?"<a href='#' title='Click here to add CBR details' class='ui-icon ui-icon-unlocked ui-align-center' onClick =CBRValue('"+URLEncrypt.EncryptParameters1(new string[]{"RoadCode =" + item.MAST_ER_ROAD_CODE.ToString().Trim()})+"'); return false;'></a>":item.UNLOCK_BY_MORD.ToString().ToUpper()=="Y"?"<span class='ui-icon ui-icon-locked ui-align-center'></span>":"<a href='#' title='Click here to add CBR details' class='ui-icon ui-icon-plusthick ui-align-center' onClick =CBRValue('"+URLEncrypt.EncryptParameters1(new string[]{"RoadCode =" + item.MAST_ER_ROAD_CODE.ToString().Trim()})+"'); return false;'></a>"),   

                        ///View
                        "<a href='#' title='Click here to view existing roads details' class='ui-icon ui-icon-zoomin ui-align-center' onClick =ShowDetails('"+URLEncrypt.EncryptParameters1(new string[]{"RoadCode =" + item.MAST_ER_ROAD_CODE.ToString().Trim()})+"'); return false;'></a>",
                        
                        ///Changed by SAMMED A. PATIL on 15FEB2017 for Map PMGSY1 DRRP Roads
                        //(PMGSYSession.Current.PMGSYScheme == 2 && (PMGSYSession.Current.RoleCode == 22 || PMGSYSession.Current.RoleCode == 25)) 
                        //? //(dbContext.MASTER_EXISTING_ROADS.Where(m=>m.MAST_ER_ROAD_CODE == item.MAST_ER_ROAD_CODE && m.MAST_ER_ROAD_CODE_PMGSY1 != null).Any() 
                        //((item.MAST_ER_ROAD_CODE_PMGSY1 != 0 && (item.MAST_ROAD_CAT_CODE_PMGSY2 != item.MAST_ROAD_CAT_CODE_PMGSY1)) ? dbContext.MASTER_EXISTING_ROADS.Where(m=>m.MAST_ER_ROAD_CODE == item.MAST_ER_ROAD_CODE_PMGSY1).Select(m=>m.MAST_ER_ROAD_NUMBER).FirstOrDefault().ToString() : item.MAST_ER_ROAD_CODE_PMGSY1 != 0
                        //    ? "<a href='#' onClick =MapDRRPPMGSY1('"+URLEncrypt.EncryptParameters1(new string[]{"BlockCode =" + blockCode.ToString().Trim(), "RoadCode =" + item.MAST_ER_ROAD_CODE.ToString().Trim(), "RoadCategory=" + item.MAST_ROAD_CAT_CODE_PMGSY2.ToString().Trim()})+"'); return false;'>"+ dbContext.MASTER_EXISTING_ROADS.Where(m=>m.MAST_ER_ROAD_CODE == item.MAST_ER_ROAD_CODE_PMGSY1).Select(m=>m.MAST_ER_ROAD_NUMBER).FirstOrDefault().ToString() +"</a>" 
                        //    : "<a href='#' title='Click here to Map DRRP PMGSY - I Road' class='ui-icon ui-icon-plusthick ui-align-center' onClick =MapDRRPPMGSY1('"+URLEncrypt.EncryptParameters1(new string[]{"BlockCode =" + blockCode.ToString().Trim(), "RoadCode =" + item.MAST_ER_ROAD_CODE.ToString().Trim(), "RoadCategory=" + item.MAST_ROAD_CAT_CODE_PMGSY2.ToString().Trim()})+"'); return false;'></a>") 
                        //: "-",
                        
                        //Map DRRP 
                        (isPMGSY3 == true && PMGSYSession.Current.PMGSYScheme == 2)
                        ? "<span class='ui-icon ui-icon-locked ui-align-center'></span>"
                        : (
                            ((PMGSYSession.Current.PMGSYScheme == 2 && (PMGSYSession.Current.RoleCode == 22 || PMGSYSession.Current.RoleCode == 25))
                            ? (item.MAST_ER_ROAD_CODE_PMGSY1 == 0) 
                                ? "<a href='#' title='Click here to Map DRRP PMGSY - I Road' class='ui-icon ui-icon-plusthick ui-align-center' onClick =MapDRRPPMGSY1('"+URLEncrypt.EncryptParameters1(new string[]{"BlockCode =" + blockCode.ToString().Trim(), "RoadCode =" + item.MAST_ER_ROAD_CODE.ToString().Trim(), "RoadCategory=" + item.MAST_ROAD_CAT_CODE_PMGSY2.ToString().Trim()})+"'); return false;'></a>" 
                                : //(dbContext.MASTER_EXISTING_ROADS.Where(m=>m.MAST_ER_ROAD_CODE == item.MAST_ER_ROAD_CODE_PMGSY1).Select(m=>m.MAST_ER_ROAD_NUMBER).FirstOrDefault())
                                  "<a href='#' onClick =MapDRRPPMGSY1('"+URLEncrypt.EncryptParameters1(new string[]{"BlockCode =" + blockCode.ToString().Trim(), "RoadCode =" + item.MAST_ER_ROAD_CODE.ToString().Trim(), "RoadCategory=" + item.MAST_ROAD_CAT_CODE_PMGSY2.ToString().Trim()})+"'); return false;'>"+ dbContext.MASTER_EXISTING_ROADS.Where(m=>m.MAST_ER_ROAD_CODE == item.MAST_ER_ROAD_CODE_PMGSY1).Select(m=>m.MAST_ER_ROAD_NUMBER).FirstOrDefault() +"</a>" 
                            : "-")
                           ),

                        //dbContext.MASTER_EXISTING_ROADS.Where(m=>m.MAST_ER_ROAD_CODE == item.MAST_ER_ROAD_CODE).Select(m=>m.MAST_LOCK_STATUS).FirstOrDefault() == "N"?"<a href='#' title='Click here to edit details' class='ui-icon ui-icon-pencil ui-align-center' onClick =EditDetails('"+URLEncrypt.EncryptParameters1(new string[]{"RoadCode =" + item.MAST_ER_ROAD_CODE.ToString().Trim()})+"'); return false;'></a>":item.UNLOCK_BY_MORD == "M"?"<a href='#' title='Click here to edit details' class='ui-icon ui-icon-unlocked ui-align-center' onClick =EditDetails('"+URLEncrypt.EncryptParameters1(new string[]{"RoadCode =" + item.MAST_ER_ROAD_CODE.ToString().Trim(),"UnlockFlag =" + item.UNLOCK_BY_MORD.ToString().Trim()})+"'); return false;'></a>":item.UNLOCK_BY_MORD.ToString().ToUpper()=="Y"?"<span class='ui-icon ui-icon-locked ui-align-center'></span>":"<a href='#' title='Click here to edit details' class='ui-icon ui-icon-pencil ui-align-center' onClick =EditDetails('"+URLEncrypt.EncryptParameters1(new string[]{"RoadCode =" + item.MAST_ER_ROAD_CODE.ToString().Trim()})+"'); return false;'></a>",   
                        
                        ///Changes by SAMMED A. PATIL on 03 OCTOBER 2017 to edit DRRP for MORD
                        
                        //Edit
                        (isPMGSY3 == true && PMGSYSession.Current.PMGSYScheme == 2)
                        ? "<span class='ui-icon ui-icon-locked ui-align-center'></span>"
                        : (PMGSYSession.Current.RoleCode == 25 
                            ? "<a href='#' title='Click here to edit details' class='ui-icon ui-icon-pencil ui-align-center' onClick =EditDetails('"+URLEncrypt.EncryptParameters1(new string[]{"RoadCode =" + item.MAST_ER_ROAD_CODE.ToString().Trim()})+"'); return false;'></a>"
                            : (dbContext.MASTER_EXISTING_ROADS.Where(m=>m.MAST_ER_ROAD_CODE == item.MAST_ER_ROAD_CODE).Select(m=>m.MAST_LOCK_STATUS).FirstOrDefault() == "N"?"<a href='#' title='Click here to edit details' class='ui-icon ui-icon-pencil ui-align-center' onClick =EditDetails('"+URLEncrypt.EncryptParameters1(new string[]{"RoadCode =" + item.MAST_ER_ROAD_CODE.ToString().Trim()})+"'); return false;'></a>":item.UNLOCK_BY_MORD == "M"?"<a href='#' title='Click here to edit details' class='ui-icon ui-icon-unlocked ui-align-center' onClick =EditDetails('"+URLEncrypt.EncryptParameters1(new string[]{"RoadCode =" + item.MAST_ER_ROAD_CODE.ToString().Trim(),"UnlockFlag =" + item.UNLOCK_BY_MORD.ToString().Trim()})+"'); return false;'></a>":item.UNLOCK_BY_MORD.ToString().ToUpper()=="Y"?"<span class='ui-icon ui-icon-locked ui-align-center'></span>":"<a href='#' title='Click here to edit details' class='ui-icon ui-icon-pencil ui-align-center' onClick =EditDetails('"+URLEncrypt.EncryptParameters1(new string[]{"RoadCode =" + item.MAST_ER_ROAD_CODE.ToString().Trim()})+"'); return false;'></a>")),

                        //Delete
                        (isPMGSY3 == true && PMGSYSession.Current.PMGSYScheme == 2)
                        ? "<span class='ui-icon ui-icon-locked ui-align-center'></span>"
                        : (PMGSYSession.Current.PMGSYScheme == 2 
                        ? /*(dbContext.MASTER_EXISTING_ROADS.Where(m=>m.MAST_ER_ROAD_CODE == item.MAST_ER_ROAD_CODE && m.MAST_ER_ROAD_CODE_PMGSY1 != null).Any() 
                            ? "<span class='ui-icon ui-icon-locked ui-align-center'></span>"
                            : (
                                (dbContext.MASTER_EXISTING_ROADS.Where(m=>m.MAST_ER_ROAD_CODE == item.MAST_ER_ROAD_CODE).Select(m=>m.MAST_LOCK_STATUS).FirstOrDefault() == "N" || item.UNLOCK_BY_MORD == "M")
                                ? ("<a href='#' title='Click here to delete details' class='ui-icon ui-icon-trash ui-align-center' onClick =DeleteDetails('"+URLEncrypt.EncryptParameters1(new string[]{"RoadCode =" + item.MAST_ER_ROAD_CODE.ToString().Trim                                   ()})+"'); return false;'></a>")
                                : "<span class='ui-icon ui-icon-locked ui-align-center'></span>"
                               /*)
                           )
                        : dbContext.MASTER_EXISTING_ROADS.Where(m=>m.MAST_ER_ROAD_CODE == item.MAST_ER_ROAD_CODE).Select(m=>m.MAST_LOCK_STATUS).FirstOrDefault() == "N"?"<a href='#' title='Click here to delete details' class='ui-icon ui-icon-trash ui-align-center' onClick =DeleteDetails('"+URLEncrypt.EncryptParameters1(new string[]{"RoadCode =" + item.MAST_ER_ROAD_CODE.ToString().Trim()})+"'); return false;'></a>":item.UNLOCK_BY_MORD == "M"?"<a href='#' title='Click here to delete details' class='ui-icon ui-icon-trash ui-align-center' onClick =DeleteDetails('"+URLEncrypt.EncryptParameters1(new string[]{"RoadCode =" + item.MAST_ER_ROAD_CODE.ToString().Trim()})+"'); return false;'></a>":item.UNLOCK_BY_MORD.ToString().ToUpper()=="Y"?"<span class='ui-icon ui-icon-locked ui-align-center'></span>":"<a href='#' title='Click here to delete details' class='ui-icon ui-icon-trash ui-align-center' onClick =DeleteDetails('"+URLEncrypt.EncryptParameters1(new string[]{"RoadCode =" + item.MAST_ER_ROAD_CODE.ToString().Trim()})+"'); return false;'></a>"), */
#endregion
                            unlockCount > 0
                            ?URLEncrypt.EncryptParameters1(new string[] {  "ERCode =" + item.MAST_ER_ROAD_CODE.ToString() })
                            : "",
                    }
                }).ToArray();
            }
            catch (Exception ex)
            {
                //Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                ErrorLog.LogError(ex, "ListExistingRoads().DAL");
                totalRecords = 0;
                return null;
            }
            finally
            {
                dbContext.Dispose();
            }
        }

        public Boolean DeleteExistingRoadsITNO(int _roadCode, ref string message)
        {
            try
            {
                dbContext = new PMGSYEntities();
                int cnRoadCode = 0;

                using (TransactionScope ts = new TransactionScope())
                {
                    if (dbContext.IMS_SANCTIONED_PROJECTS.Any(m => m.PLAN_CN_ROAD_CODE == _roadCode))
                    {
                        message = "Road is already mapped in Proposal hence can not be deleted.";
                        return false;
                    }

                    if (dbContext.PLAN_ROAD_DRRP.Where(m => m.PLAN_CN_ROAD_CODE == _roadCode).Any())
                    {
                        PLAN_ROAD_DRRP PlanRoadDRRP = dbContext.PLAN_ROAD_DRRP.Where(m => m.PLAN_CN_ROAD_CODE == _roadCode).FirstOrDefault();
                        if (PlanRoadDRRP != null)
                        {
                            dbContext.Entry(PlanRoadDRRP).State = EntityState.Deleted;
                            dbContext.PLAN_ROAD_DRRP.Remove(PlanRoadDRRP);
                        }
                    }

                    if (dbContext.PLAN_ROAD.Where(m => m.PLAN_CN_ROAD_CODE == _roadCode).Any())
                    {
                        PLAN_ROAD PlanRoad = dbContext.PLAN_ROAD.Where(m => m.PLAN_CN_ROAD_CODE == _roadCode).FirstOrDefault();
                        if (PlanRoad != null)
                        {
                            dbContext.Entry(PlanRoad).State = EntityState.Deleted;
                            dbContext.PLAN_ROAD.Remove(PlanRoad);
                        }
                    }

                    #region
                    //if (PMGSYSession.Current.PMGSYScheme == 2)
                    //{
                    //    if (dbContext.PLAN_ROAD.Any(m => m.MAST_ER_ROAD_CODE == _roadCode))
                    //    {
                    //        message = "Road is already mapped with the core network and hence can not be deleted.";
                    //        return false;
                    //    }
                    //}
                    //else
                    //{
                    //    if (dbContext.PLAN_ROAD.Any(m => m.MAST_ER_ROAD_CODE == _roadCode))
                    //    {
                    //        cnRoadCode = dbContext.PLAN_ROAD.Where(m => m.MAST_ER_ROAD_CODE == _roadCode).Select(x => x.PLAN_CN_ROAD_CODE).FirstOrDefault();
                    //        if (cnRoadCode > 0)
                    //        {
                    //            if (dbContext.IMS_SANCTIONED_PROJECTS.Any(m => m.PLAN_CN_ROAD_CODE == cnRoadCode))
                    //            {
                    //                message = "Road is already used in Sanctioned Project hence can not be deleted.";
                    //                return false;
                    //            }
                    //        }
                    //    }
                    //}
                    #endregion

                    dbContext.SaveChanges();
                    ts.Complete();
                    return true;
                }
            }
            catch (System.Data.Entity.Infrastructure.DbUpdateException ex)
            {
                ErrorLog.LogError(ex, "DeleteExistingRoads().DbUpdateException");
                message = "Candidate Road details can not be deleted because other details/PCI index for road are entered.";
                return false;
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "ExistingRoads/DeleteExistingRoadsITNO()");
                message = "An Error Occurred While Your Processing Request.";
                return false;
            }
            finally
            {
                dbContext.Dispose();
            }
        }


        public Boolean DeleteExistingRoadsMainITNO(int _roadCode, ref string message)
        {
            try
            {
                dbContext = new PMGSYEntities();
                int cnRoadCode = 0;

                using (TransactionScope ts = new TransactionScope())
                {
                    if (dbContext.IMS_UNLOCK_DETAILS.Any(m => m.MAST_ER_ROAD_CODE == _roadCode))
                    {
                        IMS_UNLOCK_DETAILS unlockDetails = dbContext.IMS_UNLOCK_DETAILS.Where(m => m.MAST_ER_ROAD_CODE == _roadCode).FirstOrDefault();
                        if (unlockDetails != null)
                        {
                            dbContext.IMS_UNLOCK_DETAILS.Remove(unlockDetails);
                            dbContext.SaveChanges();
                        }
                    }

                    MASTER_EXISTING_ROADS existingRoadsModel = dbContext.MASTER_EXISTING_ROADS.Find(_roadCode);

                    if (existingRoadsModel == null)
                    {
                        return false;
                    }
                    if (dbContext.PLAN_ROAD.Any(m => m.MAST_ER_ROAD_CODE == _roadCode))
                    {
                        message = "Road is already mapped with the core network and hence can not be deleted.";
                        return false;
                    }

                    #region
                    //if (PMGSYSession.Current.PMGSYScheme == 2)
                    //{
                    //    if (dbContext.PLAN_ROAD.Any(m => m.MAST_ER_ROAD_CODE == _roadCode))
                    //    {
                    //        message = "Road is already mapped with the core network and hence can not be deleted.";
                    //        return false;
                    //    }
                    //}
                    //else
                    //{
                    //    if (dbContext.PLAN_ROAD.Any(m => m.MAST_ER_ROAD_CODE == _roadCode))
                    //    {
                    //        cnRoadCode = dbContext.PLAN_ROAD.Where(m => m.MAST_ER_ROAD_CODE == _roadCode).Select(x => x.PLAN_CN_ROAD_CODE).FirstOrDefault();
                    //        if (cnRoadCode > 0)
                    //        {
                    //            if (dbContext.IMS_SANCTIONED_PROJECTS.Any(m => m.PLAN_CN_ROAD_CODE == cnRoadCode))
                    //            {
                    //                message = "Road is already used in Sanctioned Project hence can not be deleted.";
                    //                return false;
                    //            }
                    //        }
                    //    }
                    //}
                    #endregion

                    existingRoadsModel.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                    existingRoadsModel.USERID = PMGSYSession.Current.UserId;
                    dbContext.Entry(existingRoadsModel).State = System.Data.Entity.EntityState.Modified;
                    dbContext.SaveChanges();

                    dbContext.MASTER_EXISTING_ROADS.Remove(existingRoadsModel);
                    dbContext.SaveChanges();
                    ts.Complete();
                    return true;
                }
            }
            catch (System.Data.Entity.Infrastructure.DbUpdateException ex)
            {
                ErrorLog.LogError(ex, "DeleteExistingRoads().DbUpdateException");
                message = "Existing Roads details can not be deleted because other details for road are entered.";
                return false;
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "DeleteExistingRoads()");
                message = "An Error Occurred While Your Processing Request.";
                return false;
            }
            finally
            {
                dbContext.Dispose();
            }
        }

        public Array GetCoreNetworkListITNO(int? page, int? rows, string sidx, string sord, out Int32 totalRecords, int roadCode)
        {
            try
            {
                dbContext = new PMGSYEntities();
                var lstPlanRoads = (from item in dbContext.PLAN_ROAD
                                    where item.MAST_ER_ROAD_CODE == roadCode
                                    select new
                                    {
                                        item.PLAN_CN_ROAD_CODE,
                                        item.PLAN_CN_ROAD_NUMBER,
                                        item.PLAN_RD_NAME,
                                        item.PLAN_RD_FROM_TYPE,
                                        item.PLAN_RD_TO_TYPE,
                                        item.PLAN_RD_BLOCK_FROM_CODE,
                                        item.PLAN_RD_BLOCK_TO_CODE,
                                        item.PLAN_RD_FROM_HAB,
                                        item.PLAN_RD_TO_HAB,
                                        item.PLAN_RD_FROM_CHAINAGE,
                                        item.PLAN_RD_TO_CHAINAGE,
                                        item.PLAN_RD_NUM_FROM,
                                        item.PLAN_RD_NUM_TO,
                                        item.PLAN_RD_LENG,
                                        item.PLAN_RD_LENGTH,
                                        item.PLAN_RD_ROUTE,
                                        item.PLAN_RD_FROM,
                                        item.PLAN_RD_TO
                                    }).ToList();


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


                return lstPlanRoads.Select(roadDetails => new
                {

                    cell = new[]
                            {
                                roadDetails.PLAN_CN_ROAD_CODE.ToString(),            
                                roadDetails.PLAN_CN_ROAD_NUMBER == null?string.Empty:roadDetails.PLAN_CN_ROAD_NUMBER.ToString(),
                                roadDetails.PLAN_RD_NAME == null?string.Empty:roadDetails.PLAN_RD_NAME.ToString(),
                                roadDetails.PLAN_RD_FROM_TYPE=="B"?"Block("+(dbContext.MASTER_BLOCK.Where(item=>item.MAST_BLOCK_CODE == roadDetails.PLAN_RD_BLOCK_FROM_CODE).Select(m=>m.MAST_BLOCK_NAME).FirstOrDefault())+")":(roadDetails.PLAN_RD_FROM_TYPE=="H"?"Habitation("+(dbContext.MASTER_HABITATIONS.Where(item=>item.MAST_HAB_CODE == roadDetails.PLAN_RD_FROM_HAB).Select(m=>m.MAST_HAB_NAME).FirstOrDefault())+")":(roadDetails.PLAN_RD_FROM==null?string.Empty:roadDetails.PLAN_RD_FROM.ToString())),
                                roadDetails.PLAN_RD_TO_TYPE=="B"?"Block("+(dbContext.MASTER_BLOCK.Where(item=>item.MAST_BLOCK_CODE == roadDetails.PLAN_RD_BLOCK_TO_CODE).Select(m=>m.MAST_BLOCK_NAME).FirstOrDefault())+")":(roadDetails.PLAN_RD_TO_TYPE=="H"?"Habitation("+(dbContext.MASTER_HABITATIONS.Where(item=>item.MAST_HAB_CODE == roadDetails.PLAN_RD_TO_HAB).Select(m=>m.MAST_HAB_NAME).FirstOrDefault())+")":(roadDetails.PLAN_RD_TO==null?string.Empty:roadDetails.PLAN_RD_TO.ToString())),
                                roadDetails.PLAN_RD_FROM_CHAINAGE == null?string.Empty:roadDetails.PLAN_RD_FROM_CHAINAGE.ToString(),
                                roadDetails.PLAN_RD_TO_CHAINAGE == null?string.Empty:roadDetails.PLAN_RD_TO_CHAINAGE.ToString(),
                                roadDetails.PLAN_RD_LENGTH == null?string.Empty:roadDetails.PLAN_RD_LENGTH.ToString(),


                                //added by abhinav pathak on 09-10-2019

                                "<a href='#' title='Click here to delete details' class='ui-icon ui-icon-trash ui-align-center' onClick =DeleteDetails('"+URLEncrypt.EncryptParameters1(new string[]{"RoadCode =" + roadDetails.PLAN_CN_ROAD_CODE.ToString().Trim()})+"'); return false;'></a>"
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

        #endregion//commented by abhinav

        #region TraceMaps For MORD

        public Array GetDistrictListMORDDAL(int page, int rows, string sidx, string sord, out int totalRecords, int statecode, int districtcode)
        {
            PMGSYEntities dbContext = new PMGSYEntities();

            var BlockList = (from c in dbContext.MASTER_BLOCK
                             where
                             c.MAST_DISTRICT_CODE == (districtcode == 0 ? c.MAST_DISTRICT_CODE : districtcode)
                             && c.MASTER_DISTRICT.MAST_STATE_CODE == (districtcode == 0 ? statecode : c.MASTER_DISTRICT.MAST_STATE_CODE)
                             && c.MAST_BLOCK_ACTIVE == "Y"
                             select new
                             {
                                 c.MAST_BLOCK_NAME,
                                 c.MAST_BLOCK_CODE,
                                 c.MASTER_DISTRICT.MAST_DISTRICT_NAME
                             }).OrderBy(x => x.MAST_DISTRICT_NAME).ToList();

            try
            {
                totalRecords = BlockList.Count();
                var isFinalizedcheck = (from item in dbContext.MAST_FACILITY_DISTRICT_PMGSY3_FINALIZE
                                        join MB in dbContext.MASTER_BLOCK on item.MAST_DISTRICT_CODE
                                        equals MB.MAST_DISTRICT_CODE
                                        select new
                                        {
                                            MB.MAST_DISTRICT_CODE,
                                            MB.MAST_BLOCK_CODE,
                                            item.IS_FINALIZED
                                        }).ToList();


                BlockList = BlockList.OrderBy(x => x.MAST_DISTRICT_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                return BlockList.Select(obj => new
                {
                    cell = new[]
                    {
                        obj.MAST_DISTRICT_NAME.ToString(),

                        obj.MAST_BLOCK_NAME.ToString(),
                        
                        (dbContext.MAST_FACILITY_BLOCK_PMGSY3_FINALIZE.Where(x => x.MAST_BLOCK_CODE == obj.MAST_BLOCK_CODE && x.IS_FINALIZED == "Y").Any())?
                        "<a href='#' title='Click here to Download Facility List' class='ui-icon  ui-icon-arrowthickstop-1-s ui-align-center' onClick='DownloadFacilityList(\"" + 
                        obj.MAST_BLOCK_CODE + "\"); return false;'>Download</a>" : "<span style='color:red;'>Facility details not finalized</span>",
                        
                        #region for PDF upload condtions
                        (dbContext.MAST_TRACEFILE_PMGSY3.Where(x => x.MAST_BLOCK_CODE == obj.MAST_BLOCK_CODE).Select(x => x.TRACEFILE_NAME_PDF).FirstOrDefault() != null) ?
                        "<a href='#' title='Click here to view PDF' class='ui-icon ui-icon-zoomin ui-align-center' onClick='UploadPDF(\"" + obj.MAST_BLOCK_CODE + "\"); return false;'>Upload</a>": "<span style='color:red;'>PDF File not uploaded</                           span>" ,
                        #endregion
                        
                        #region for CSV upload condtions
                        (dbContext.MAST_TRACEFILE_PMGSY3.Where(x => x.MAST_BLOCK_CODE == obj.MAST_BLOCK_CODE).Select(x => x.TRACEFILE_NAME_CSV).FirstOrDefault() != null) ?
                        "<a href='#' title='Click here to view CSV' class='ui-icon ui-icon-zoomin ui-align-center' onClick='UploadCSV(\"" + obj.MAST_BLOCK_CODE + "\"); return false;'>Upload</a>" : "<span style='color:red;'>CSV File not uploaded</                          span>",
                        #endregion


                         #region for Hab CSV upload at MORD  condtions
                        (dbContext.MAST_HAB_CSV_PMGSY3.Where(x => x.MAST_BLOCK_CODE == obj.MAST_BLOCK_CODE).Select(x => x.MAST_HAB_CSV_FILE_NAME).FirstOrDefault() != null) ?
                        "<a href='#' title='Click here to view Habitation CSV' class='ui-icon ui-icon-zoomin ui-align-center' onClick='UploadHabCSV(\"" + obj.MAST_BLOCK_CODE + "\"); return false;'>Upload</a>" : "<span style='color:red;'>Habitation CSV File not uploaded</span>",
                        #endregion


                        (PMGSYSession.Current.RoleCode == 22 && dbContext.MAST_TRACEFILE_PMGSY3.Where(x => x.MAST_BLOCK_CODE == obj.MAST_BLOCK_CODE 
                            && x.TRACEFILE_NAME_PDF !=null 
                            && x.TRACEFILE_NAME_CSV != null 
                            && x.TRACEFILE_FINALIZE == "N").Any()) ? 
                                      "<a href='#' title='Click here to Finalize the File Details' class='ui-icon ui-icon-plusthick ui-align-center' onClick=FinalisePDFDetails('" +                                                                                                                obj.MAST_BLOCK_CODE+ "'); return false;>Finalise</a>"
                                        //: (PMGSYSession.Current.RoleCode == 36 ? "Trace Map not yet Finalized" : "-"),
                                        : (PMGSYSession.Current.RoleCode == 36 && dbContext.MAST_TRACEFILE_PMGSY3.Where(x => x.MAST_BLOCK_CODE == obj.MAST_BLOCK_CODE && x.TRACEFILE_FINALIZE == "Y").Any() ? "Trace Map Finalized" : 
                                        dbContext.MAST_TRACEFILE_PMGSY3.Where(x => x.MAST_BLOCK_CODE == obj.MAST_BLOCK_CODE && x.TRACEFILE_FINALIZE == "Y").Any() ? "Trace Map Finalized" : 
                                        "Trace Map not yet Finalized"),
                        

                        
                        //dbContext.PLAN_ROAD.Where(x => x.MAST_BLOCK_CODE == obj.MAST_BLOCK_CODE && x.MAST_PMGSY_SCHEME == 4 && x.PLAN_LOCK_STATUS == "Y").Any() ? 
                        //"Candidate Roads Entered"
                        //: "B"
                        //"<a href='#' title='Click here to Definalize the File Details' class='ui-icon ui-icon-unlocked ui-align-center' onClick=DefinalizeTraceMapDetails('" +                                                                obj.MAST_BLOCK_CODE+ "'); return false;>Finalise</a>",


                        
                        dbContext.MAST_TRACEFILE_PMGSY3.Where(x => x.MAST_BLOCK_CODE == obj.MAST_BLOCK_CODE && x.TRACEFILE_FINALIZE == "Y").Any() ?
                        (dbContext.MAST_CN_BLOCK_PMGSY3_FINALIZE.Where(x => x.MAST_BLOCK_CODE == obj.MAST_BLOCK_CODE && x.IS_FINALIZED == "Y").Any() ? 
                        "<span style='color:red;'>Block is Finalized</span>"
                        :
                        "<a href='#' title='Click here to Definalize the File Details' class='ui-icon ui-icon-unlocked ui-align-center' onClick=DefinalizeTraceMapDetails('" +                                                                                                  obj.MAST_BLOCK_CODE+ "'); return false;>Finalise</a>") : "<span style='color:red;'>Not Available</span>"

                    }


                }).ToArray();

            }
            catch (Exception ex)
            {
                totalRecords = BlockList.Count();
                ErrorLog.LogError(ex, "ExistingRoadsDAL/GetDistrictListDAL");
                return null;
            }
            finally
            {
                if (dbContext != null)
                    dbContext.Dispose();
            }
        }

        public Array GetTraceMapsFilesListMORDDAL(int page, int rows, string sidx, string sord, out int totalRecords, int blockcode)
        {
            dbContext = new PMGSYEntities();
            var record = dbContext.MAST_TRACEFILE_PMGSY3.Where(x => x.MAST_BLOCK_CODE == blockcode && x.TRACEFILE_NAME_PDF != null).ToList();

            totalRecords = record.Count();

            string VirtualDirectoryUrl = string.Empty;
            string PhysicalPath = string.Empty;

            VirtualDirectoryUrl = ConfigurationManager.AppSettings["TRACE_MAPS_FILE_UPLOAD_PDF_VIRTUAL_DIR_PATH"];
            PhysicalPath = ConfigurationManager.AppSettings["TRACE_MAPS_FILE_UPLOAD_PDF"];

            return record.Select(fileDetails => new
            {
                id = fileDetails.MAST_TRACEFILE_ID + "$" + fileDetails.MAST_BLOCK_CODE,
                cell = new[] {       
                    
                                       URLEncrypt.EncryptParameters(new string[] { fileDetails.TRACEFILE_NAME_PDF + "$" +  fileDetails.MAST_TRACEFILE_ID}),                                                                                                      
                                       (fileDetails.TRACEFILE_FINALIZE == "N" && (PMGSYSession.Current.RoleCode == 2 || PMGSYSession.Current.RoleCode == 36)) ?
                                       "<a href='#' title='Click here to delete the File and File Details' class='ui-icon ui-icon-trash ui-align-center' onClick=DeletePDFFileDetails('" + fileDetails.MAST_TRACEFILE_ID.ToString().Trim() + "$" +  fileDetails.MAST_BLOCK_CODE +"'); return false;'>Delete</a>"
                                       : "<a href='#' class='ui-icon ui-icon-locked ui-align-center'>",

                                       fileDetails.TRACEFILE_FINALIZE == "N" ? "No" : "Yes",
                                       
                                      (PMGSYSession.Current.RoleCode == 22) ? 
                                      "<a href='#' title='Click here to Finalize the File Details' class='ui-icon ui-icon-plusthick ui-align-center' onClick=FinalisePDFDetails('" +  fileDetails.MAST_TRACEFILE_ID.ToString                                            ().Trim()+ "'); return false;>Finalise</a>"
                                        : "-",
            }
            }).ToArray();

        }

        public Array GetTraceCSVFilesListMORDDAL(int page, int rows, string sidx, string sord, out int totalRecords, int blockcode)
        {
            dbContext = new PMGSYEntities();
            var record = dbContext.MAST_TRACEFILE_PMGSY3.Where(x => x.MAST_BLOCK_CODE == blockcode && x.TRACEFILE_NAME_CSV != null).ToList();

            totalRecords = record.Count();

            string VirtualDirectoryUrl = string.Empty;
            string PhysicalPath = string.Empty;

            VirtualDirectoryUrl = ConfigurationManager.AppSettings["TRACE_MAPS_FILE_UPLOAD_PDF_VIRTUAL_DIR_PATH"];
            PhysicalPath = ConfigurationManager.AppSettings["DRRP_TRACE_CSV_PATH"];

            return record.Select(fileDetails => new
            {
                id = fileDetails.MAST_TRACEFILE_ID + "$" + fileDetails.MAST_BLOCK_CODE,
                cell = new[] {       
                    
                                       URLEncrypt.EncryptParameters(new string[] { fileDetails.TRACEFILE_NAME_CSV + "$" +  fileDetails.MAST_TRACEFILE_ID}),                                                                                                      
                                       (fileDetails.TRACEFILE_FINALIZE == "N" && (PMGSYSession.Current.RoleCode == 2 || PMGSYSession.Current.RoleCode == 36)) ?
                                       "<a href='#' title='Click here to delete the File and File Details' class='ui-icon ui-icon-trash ui-align-center' onClick=DeleteCSVFileDetails('" + fileDetails.MAST_TRACEFILE_ID.ToString().Trim() + "&" +                                                fileDetails.MAST_BLOCK_CODE +"'); return false;'>Delete</a>"
                                       : "<a href='#' class='ui-icon ui-icon-locked ui-align-center'>",

                                       fileDetails.TRACEFILE_FINALIZE == "N" ? "No" : "Yes",
                                      
                                       (PMGSYSession.Current.RoleCode == 22) ? 
                                      "<a href='#' title='Click here to Finalize the File Details' class='ui-icon ui-icon-plusthick ui-align-center' onClick=FinaliseCSVDetails('" +        fileDetails.MAST_TRACEFILE_ID.ToString().Trim()+ "'); return false;>Finalise</a>"
                                        : "-",
            }
            }).ToArray();

        }
        #endregion

        #region ITNO for inactive Blocks
        public Array ListExistingRoadsITNOForInactiveBlocksDAL(int stateCode, int districtCode, int blockCode, int categoryCode, int ownerCode, int? page, int? rows, string sidx, string sord, out long totalRecords, string filters)
        {
            bool isPMGSY3 = false;
            try
            {
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
                #region Old Logic
                //var lstExistingRoadsDetails = (from existingRoads in dbContext.MASTER_EXISTING_ROADS
                //                               join roadCatCode in dbContext.MASTER_ROAD_CATEGORY
                //                               on existingRoads.MAST_ROAD_CAT_CODE equals roadCatCode.MAST_ROAD_CAT_CODE
                //                               join agency in dbContext.MASTER_AGENCY
                //                               on existingRoads.MAST_ER_ROAD_OWNER equals agency.MAST_AGENCY_CODE

                //                               where ((blockCode == 0 ? 1 : existingRoads.MAST_BLOCK_CODE) == (blockCode == 0 ? 1 : blockCode)) &&
                //                               ((categoryCode == 0 ? 1 : existingRoads.MAST_ROAD_CAT_CODE) == (categoryCode == 0 ? 1 : categoryCode))
                //                               &&(roadName == string.Empty ? "%" : existingRoads.MAST_ER_ROAD_NAME).StartsWith(roadName == string.Empty ? "%" : roadName)     
                //                               select new
                //                               {
                //                                   existingRoads.MAST_ER_ROAD_CODE,
                //                                   existingRoads.MAST_ER_ROAD_NUMBER,
                //                                   existingRoads.MAST_ER_ROAD_NAME,
                //                                   existingRoads.MAST_CORE_NETWORK,
                //                                   agency.MAST_AGENCY_NAME,
                //                                   existingRoads.MAST_ER_ROAD_OWNER,
                //                                   existingRoads.MAST_LOCK_STATUS,

                //                               }).ToList();
                //totalRecords = lstExistingRoadsDetails.Count();

                //if (sidx.Trim() != string.Empty)
                //{
                //    if (sord.ToString() == "asc")
                //    {
                //        switch (sidx)
                //        {
                //            case "MAST_ER_ROAD_NUMBER":
                //                lstExistingRoadsDetails = lstExistingRoadsDetails.OrderBy(x => x.MAST_ER_ROAD_NUMBER).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                //                break;

                //            case "MAST_ER_ROAD_NAME":
                //                lstExistingRoadsDetails = lstExistingRoadsDetails.OrderBy(x => x.MAST_ER_ROAD_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                //                break;
                //            case "MAST_ER_ROAD_OWNER":
                //                lstExistingRoadsDetails = lstExistingRoadsDetails.OrderBy(x => x.MAST_AGENCY_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                //                break;
                //            case "MAST_CORE_NETWORK":
                //                lstExistingRoadsDetails = lstExistingRoadsDetails.OrderBy(x => x.MAST_CORE_NETWORK).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                //                break;
                //            case "MAST_ER_ROAD_CODE":
                //                lstExistingRoadsDetails = lstExistingRoadsDetails.OrderBy(x => x.MAST_ER_ROAD_CODE).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                //                break;
                //            default:
                //                lstExistingRoadsDetails = lstExistingRoadsDetails.OrderBy(x => x.MAST_ER_ROAD_CODE).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                //                break;
                //        }
                //    }
                //    else
                //    {

                //        switch (sidx)
                //        {
                //            case "MAST_ER_ROAD_NUMBER":
                //                lstExistingRoadsDetails = lstExistingRoadsDetails.OrderByDescending(x => x.MAST_ER_ROAD_NUMBER).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                //                break;

                //            case "MAST_ER_ROAD_NAME":
                //                lstExistingRoadsDetails = lstExistingRoadsDetails.OrderByDescending(x => x.MAST_ER_ROAD_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                //                break;

                //            case "MAST_ER_ROAD_OWNER":
                //                lstExistingRoadsDetails = lstExistingRoadsDetails.OrderByDescending(x => x.MAST_AGENCY_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                //                break;
                //            case "MAST_CORE_NETWORK":
                //                lstExistingRoadsDetails = lstExistingRoadsDetails.OrderByDescending(x => x.MAST_CORE_NETWORK).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                //                break;
                //            case "MAST_ER_ROAD_CODE":
                //                lstExistingRoadsDetails = lstExistingRoadsDetails.OrderByDescending(x => x.MAST_ER_ROAD_CODE).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                //                break;
                //            default:
                //                lstExistingRoadsDetails = lstExistingRoadsDetails.OrderByDescending(x => x.MAST_ER_ROAD_CODE).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                //                break;
                //        }
                //    }
                //}
                //else
                //{
                //    lstExistingRoadsDetails = lstExistingRoadsDetails.OrderByDescending(x => x.MAST_ER_ROAD_CODE).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();

                //}

                //return lstExistingRoadsDetails.Select(item => new
                //{
                //    id = item.MAST_ER_ROAD_CODE,
                //    cell = new[]{                          

                //                  item.MAST_ER_ROAD_NUMBER,
                //                  item.MAST_ER_ROAD_NAME,
                //                  item.MAST_AGENCY_NAME,
                //                  item.MAST_CORE_NETWORK.ToString().Trim().ToUpper()=="Y"?"Yes":"No",

                //                  item.MAST_LOCK_STATUS.ToString().ToUpper()=="Y"?string.Empty:URLEncrypt.EncryptParameters1(new string[]{"RoadCode="+item.MAST_ER_ROAD_CODE.ToString().Trim()}),


                //                   item.MAST_LOCK_STATUS.ToString().ToUpper()=="Y"?string.Empty:URLEncrypt.EncryptParameters1(new string[]{"RoadCode="+item.MAST_ER_ROAD_CODE.ToString().Trim()}),   
                //                   item.MAST_LOCK_STATUS.ToString().ToUpper()=="Y"?string.Empty:URLEncrypt.EncryptParameters1(new string[]{"RoadCode="+item.MAST_ER_ROAD_CODE.ToString().Trim()}),
                //                   item.MAST_LOCK_STATUS.ToString().ToUpper()=="Y"?string.Empty:URLEncrypt.EncryptParameters1(new string[]{"RoadCode="+item.MAST_ER_ROAD_CODE.ToString().Trim()}),
                //                   item.MAST_LOCK_STATUS.ToString().ToUpper()=="Y"?string.Empty:URLEncrypt.EncryptParameters1(new string[]{"RoadCode="+item.MAST_ER_ROAD_CODE.ToString().Trim()}),    
                //                   URLEncrypt.EncryptParameters1(new string[]{"RoadCode="+item.MAST_ER_ROAD_CODE.ToString().Trim()}),
                //                   item.MAST_LOCK_STATUS.ToString().ToUpper()=="Y"?string.Empty:URLEncrypt.EncryptParameters1(new string[]{"RoadCode="+item.MAST_ER_ROAD_CODE.ToString().Trim()}),    
                //                   item.MAST_LOCK_STATUS.ToString().ToUpper()=="Y"?string.Empty:URLEncrypt.EncryptParameters1(new string[]{"RoadCode="+item.MAST_ER_ROAD_CODE.ToString().Trim()})

                //            }
                //}).ToArray();
                #endregion

                if (PMGSYSession.Current.RoleCode == 2)
                {
                    stateCode = PMGSYSession.Current.StateCode;
                }
                else if (PMGSYSession.Current.RoleCode == 22 || PMGSYSession.Current.RoleCode == 54)///Changes for RCPLWE
                {
                    stateCode = PMGSYSession.Current.StateCode;
                    districtCode = PMGSYSession.Current.DistrictCode;
                }
                ///PMGSY3
                isPMGSY3 = dbContext.MASTER_PMGSY3.Any(x => x.MAST_STATE_CODE == stateCode && x.MAST_PMGSY3_ACTIVE == "Y");

                short roleCode = dbContext.UM_User_Master.Where(m => m.UserID == PMGSYSession.Current.UserId).Select(m => m.DefaultRoleID).FirstOrDefault();
                //var lstExistingRoadsDetails = dbContext.GET_EXISTING_ROADS((stateCode <= 0 ? 0 : stateCode), (districtCode <= 0 ? 0 : districtCode), (blockCode <= 0 ? 0 : blockCode), categoryCode, roadName, PMGSYSession.Current.PMGSYScheme, roleCode).ToList();
                var lstExistingRoadsDetails = dbContext.GET_EXISTING_ROADS((stateCode <= 0 ? 0 : stateCode), (districtCode <= 0 ? 0 : districtCode), (blockCode <= 0 ? 0 : blockCode), categoryCode, roadName, (PMGSYSession.Current.PMGSYScheme == 3 ? (byte)1 : PMGSYSession.Current.PMGSYScheme), (roleCode == 54 ? (short)22 : roleCode)).ToList();///Changes for RCPLWE

                var PLAN_ROAD = dbContext.PLAN_ROAD.Where(x => x.MAST_DISTRICT_CODE == districtCode && x.MAST_BLOCK_CODE == blockCode).Select(x => x.MAST_ER_ROAD_CODE).ToList();
                List<GET_EXISTING_ROADS_Result> templist = new List<GET_EXISTING_ROADS_Result>();
                foreach (var item in lstExistingRoadsDetails)
                {
                    if (PLAN_ROAD.Contains(item.MAST_ER_ROAD_CODE))
                    {
                        templist.Add(item);
                    }
                }
                lstExistingRoadsDetails = lstExistingRoadsDetails.Except(templist).ToList();
                lstExistingRoadsDetails = lstExistingRoadsDetails.Where(x => x.MAST_ER_ROAD_CODE != x.MAST_ER_ROAD_CODE_PMGSY1).ToList();

                totalRecords = lstExistingRoadsDetails.Count();

                if (sidx.Trim() != string.Empty)
                {
                    if (sord.ToString() == "asc")
                    {
                        switch (sidx)
                        {
                            case "MAST_ER_ROAD_CODE":
                                lstExistingRoadsDetails = lstExistingRoadsDetails.OrderBy(x => x.MAST_ER_ROAD_CODE).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                            case "MAST_ER_ROAD_NUMBER":
                                lstExistingRoadsDetails = lstExistingRoadsDetails.OrderBy(x => x.MAST_ER_ROAD_NUMBER).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                            case "MAST_ER_ROAD_NAME":
                                lstExistingRoadsDetails = lstExistingRoadsDetails.OrderBy(x => x.MAST_ER_ROAD_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                            case "MAST_ER_ROAD_OWNER":
                                lstExistingRoadsDetails = lstExistingRoadsDetails.OrderBy(x => x.AGENCY_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                            case "MAST_CORE_NETWORK":
                                lstExistingRoadsDetails = lstExistingRoadsDetails.OrderBy(x => x.MAST_CORE_NETWORK).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                            //case "MAST_ER_ROAD_CODE":
                            //    lstExistingRoadsDetails = lstExistingRoadsDetails.OrderBy(x => x.MAST_ER_ROAD_CODE).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                            //break;
                            case "MAST_ROAD_SHORT_DESC":
                                lstExistingRoadsDetails = lstExistingRoadsDetails.OrderBy(x => x.MAST_ROAD_SHORT_DESC).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                            case "MAST_ER_ROAD_TYPE":
                                lstExistingRoadsDetails = lstExistingRoadsDetails.OrderBy(x => x.MAST_ER_ROAD_TYPE).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                            default:
                                lstExistingRoadsDetails = lstExistingRoadsDetails.OrderBy(x => x.MAST_ER_ROAD_CODE).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                        }
                    }
                    else
                    {

                        switch (sidx)
                        {
                            case "MAST_ER_ROAD_CODE":
                                lstExistingRoadsDetails = lstExistingRoadsDetails.OrderByDescending(x => x.MAST_ER_ROAD_CODE).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                            case "MAST_ER_ROAD_NUMBER":
                                lstExistingRoadsDetails = lstExistingRoadsDetails.OrderByDescending(x => x.MAST_ER_ROAD_NUMBER).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                            case "MAST_ER_ROAD_NAME":
                                lstExistingRoadsDetails = lstExistingRoadsDetails.OrderByDescending(x => x.MAST_ER_ROAD_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                            case "MAST_ER_ROAD_OWNER":
                                lstExistingRoadsDetails = lstExistingRoadsDetails.OrderByDescending(x => x.AGENCY_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                            case "MAST_CORE_NETWORK":
                                lstExistingRoadsDetails = lstExistingRoadsDetails.OrderByDescending(x => x.MAST_CORE_NETWORK).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                            //case "MAST_ER_ROAD_CODE":
                            //    lstExistingRoadsDetails = lstExistingRoadsDetails.OrderByDescending(x => x.MAST_ER_ROAD_CODE).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                            //    break;
                            case "MAST_ROAD_SHORT_DESC":
                                lstExistingRoadsDetails = lstExistingRoadsDetails.OrderByDescending(x => x.MAST_ROAD_SHORT_DESC).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                            case "MAST_ER_ROAD_TYPE":
                                lstExistingRoadsDetails = lstExistingRoadsDetails.OrderByDescending(x => x.MAST_ER_ROAD_TYPE).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                            default:
                                lstExistingRoadsDetails = lstExistingRoadsDetails.OrderByDescending(x => x.MAST_ER_ROAD_CODE).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                        }
                    }
                }
                else
                {
                    lstExistingRoadsDetails = lstExistingRoadsDetails.OrderByDescending(x => x.MAST_ER_ROAD_CODE).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                }

                var result = lstExistingRoadsDetails.Select(roadDetails => new
                {
                    roadDetails.MAST_ER_ROAD_CODE,
                    roadDetails.MAST_ROAD_SHORT_DESC,
                    roadDetails.MAST_ER_ROAD_NUMBER,
                    roadDetails.MAST_ER_ROAD_NAME,
                    roadDetails.MAST_ER_ROAD_TYPE,
                    roadDetails.AGENCY_NAME,
                    roadDetails.MAST_CORE_NETWORK,
                    roadDetails.UNLOCK_BY_MORD,
                    //MAST_ER_ROAD_CODE_PMGSY1 = dbContext.MASTER_EXISTING_ROADS.Where(m => m.MAST_ER_ROAD_CODE == roadDetails.MAST_ER_ROAD_CODE).Select(m => m.MAST_ER_ROAD_CODE_PMGSY1).FirstOrDefault(),
                    roadDetails.MAST_ER_ROAD_CODE_PMGSY1,
                    roadDetails.MAST_ROAD_CAT_CODE_PMGSY2,
                    roadDetails.MAST_ROAD_CAT_CODE_PMGSY1,

                }).ToArray();

                return result.Select(item => new
                {

                    id = item.MAST_ER_ROAD_CODE,
                    cell = new[]
                    {       
                        item.MAST_ER_ROAD_CODE.ToString(),
                        item.MAST_ROAD_SHORT_DESC, //Road Category Short Desc
                        item.MAST_ER_ROAD_NUMBER,
                        item.MAST_ER_ROAD_NAME,
                         item.MAST_ER_ROAD_TYPE, //Road Type
                        (dbContext.MASTER_EXISTING_ROADS.Where(m=>m.MAST_ER_ROAD_CODE == item.MAST_ER_ROAD_CODE).Select(m=>m.MAST_ER_ROAD_END_CHAIN).FirstOrDefault() - dbContext.MASTER_EXISTING_ROADS.Where(m=>m.MAST_ER_ROAD_CODE == item.MAST_ER_ROAD_CODE).Select(m=>m.MAST_ER_ROAD_STR_CHAIN).FirstOrDefault()).ToString(),
                        item.AGENCY_NAME,
                        item.MAST_CORE_NETWORK.ToString().Trim().ToUpper()=="Y"?"Yes":"No",
                        
                        #region
                        dbContext.MASTER_EXISTING_ROADS.Where(m=>m.MAST_ER_ROAD_CODE == item.MAST_ER_ROAD_CODE).Select(m=>m.MAST_LOCK_STATUS).FirstOrDefault() == "N"?"<a href='#' title='Click here to add CDWorks details' class='ui-icon ui-icon-plusthick ui-align-center' onClick =CDWorks('"+URLEncrypt.EncryptParameters1(new string[]{"RoadCode =" + item.MAST_ER_ROAD_CODE.ToString().Trim()})+"'); return false;'></a>":item.UNLOCK_BY_MORD == "M"?"<a href='#' title='Click here to add CDWorks details' class='ui-icon ui-icon-unlocked ui-align-center' onClick =CDWorks('"+URLEncrypt.EncryptParameters1(new string[]{"RoadCode =" + item.MAST_ER_ROAD_CODE.ToString().Trim()})+"'); return false;'></a>":item.UNLOCK_BY_MORD.ToString().ToUpper()=="Y"?"<span class='ui-icon ui-icon-locked ui-align-center'></span>":"<a href='#' title='Click here to add CDWorks details' class='ui-icon ui-icon-plusthick ui-align-center' onClick =CDWorks('"+URLEncrypt.EncryptParameters1(new string[]{"RoadCode =" + item.MAST_ER_ROAD_CODE.ToString().Trim()})+"'); return false;'></a>",   
                        
                        dbContext.MASTER_EXISTING_ROADS.Where(m=>m.MAST_ER_ROAD_CODE == item.MAST_ER_ROAD_CODE).Select(m=>m.MAST_LOCK_STATUS).FirstOrDefault() == "N"?"<a href='#' title='Click here to add surface type details' class='ui-icon ui-icon-plusthick ui-align-center' onClick =SurfaceTypes('"+URLEncrypt.EncryptParameters1(new string[]{"RoadCode =" + item.MAST_ER_ROAD_CODE.ToString().Trim()})+"'); return false;'></a>":item.UNLOCK_BY_MORD == "M"?"<a href='#' title='Click here to add surface type details' class='ui-icon ui-icon-unlocked ui-align-center' onClick =SurfaceTypes('"+URLEncrypt.EncryptParameters1(new string[]{"RoadCode =" + item.MAST_ER_ROAD_CODE.ToString().Trim()})+"'); return false;'></a>":item.UNLOCK_BY_MORD.ToString().ToUpper()=="Y"?"<span class='ui-icon ui-icon-locked ui-align-center'></span>":"<a href='#' title='Click here to add surface type details' class='ui-icon ui-icon-plusthick ui-align-center' onClick =SurfaceTypes('"+URLEncrypt.EncryptParameters1(new string[]{"RoadCode =" + item.MAST_ER_ROAD_CODE.ToString().Trim()})+"'); return false;'></a>",   
                        
                        dbContext.MASTER_EXISTING_ROADS.Where(m=>m.MAST_ER_ROAD_CODE == item.MAST_ER_ROAD_CODE).Select(m=>m.MAST_LOCK_STATUS).FirstOrDefault() == "N"?"<a href='#' title='Click here to add habitation details' class='ui-icon ui-icon-plusthick ui-align-center' onClick =HabitationsMapped('"+URLEncrypt.EncryptParameters1(new string[]{"RoadCode =" + item.MAST_ER_ROAD_CODE.ToString().Trim()})+"'); return false;'></a>":item.UNLOCK_BY_MORD == "M"?"<a href='#' title='Click here to add Habitation details' class='ui-icon ui-icon-unlocked ui-align-center' onClick =HabitationsMapped('"+URLEncrypt.EncryptParameters1(new string[]{"RoadCode =" + item.MAST_ER_ROAD_CODE.ToString().Trim()})+"'); return false;'></a>":item.UNLOCK_BY_MORD.ToString().ToUpper()=="Y"?"<span class='ui-icon ui-icon-locked ui-align-center'></span>":"<a href='#' title='Click here to add habitation details' class='ui-icon ui-icon-plusthick ui-align-center' onClick =HabitationsMapped('"+URLEncrypt.EncryptParameters1(new string[]{"RoadCode =" + item.MAST_ER_ROAD_CODE.ToString().Trim()})+"'); return false;'></a>",   
                       
                        dbContext.MASTER_EXISTING_ROADS.Where(m=>m.MAST_ER_ROAD_CODE == item.MAST_ER_ROAD_CODE).Select(m=>m.MAST_LOCK_STATUS).FirstOrDefault() == "N"?"<a href='#' title='Click here to add traffic intensity details' class='ui-icon ui-icon-plusthick ui-align-center' onClick =TrafficIntensity('"+URLEncrypt.EncryptParameters1(new string[]{"RoadCode =" + item.MAST_ER_ROAD_CODE.ToString().Trim()})+"'); return false;'></a>":item.UNLOCK_BY_MORD == "M"?"<a href='#' title='Click here to add Traffic Intensity details' class='ui-icon ui-icon-unlocked ui-align-center' onClick =TrafficIntensity('"+URLEncrypt.EncryptParameters1(new string[]{"RoadCode =" + item.MAST_ER_ROAD_CODE.ToString().Trim()})+"'); return false;'></a>":item.UNLOCK_BY_MORD.ToString().ToUpper()=="Y"?"<span class='ui-icon ui-icon-locked ui-align-center'></span>":"<a href='#' title='Click here to add traffic intensity details' class='ui-icon ui-icon-plusthick ui-align-center' onClick =TrafficIntensity('"+URLEncrypt.EncryptParameters1(new string[]{"RoadCode =" + item.MAST_ER_ROAD_CODE.ToString().Trim()})+"'); return false;'></a>",   
                        
                        dbContext.MASTER_EXISTING_ROADS.Where(m=>m.MAST_ER_ROAD_CODE == item.MAST_ER_ROAD_CODE).Select(m=>m.MAST_LOCK_STATUS).FirstOrDefault() == "N"?"<a href='#' title='Click here to add CBR details' class='ui-icon ui-icon-plusthick ui-align-center' onClick =CBRValue('"+URLEncrypt.EncryptParameters1(new string[]{"RoadCode =" + item.MAST_ER_ROAD_CODE.ToString().Trim()})+"'); return false;'></a>":item.UNLOCK_BY_MORD == "M"?"<a href='#' title='Click here to add CBR details' class='ui-icon ui-icon-unlocked ui-align-center' onClick =CBRValue('"+URLEncrypt.EncryptParameters1(new string[]{"RoadCode =" + item.MAST_ER_ROAD_CODE.ToString().Trim()})+"'); return false;'></a>":item.UNLOCK_BY_MORD.ToString().ToUpper()=="Y"?"<span class='ui-icon ui-icon-locked ui-align-center'></span>":"<a href='#' title='Click here to add CBR details' class='ui-icon ui-icon-plusthick ui-align-center' onClick =CBRValue('"+URLEncrypt.EncryptParameters1(new string[]{"RoadCode =" + item.MAST_ER_ROAD_CODE.ToString().Trim()})+"'); return false;'></a>",   
                        "<a href='#' title='Click here to view existing roads details' class='ui-icon ui-icon-zoomin ui-align-center' onClick =ShowDetails('"+URLEncrypt.EncryptParameters1(new string[]{"RoadCode =" + item.MAST_ER_ROAD_CODE.ToString().Trim()})+"'); return false;'></a>",
                        
                        ///Changed by SAMMED A. PATIL on 15FEB2017 for Map PMGSY1 DRRP Roads
                        //(PMGSYSession.Current.PMGSYScheme == 2 && (PMGSYSession.Current.RoleCode == 22 || PMGSYSession.Current.RoleCode == 25)) 
                        //? //(dbContext.MASTER_EXISTING_ROADS.Where(m=>m.MAST_ER_ROAD_CODE == item.MAST_ER_ROAD_CODE && m.MAST_ER_ROAD_CODE_PMGSY1 != null).Any() 
                        //((item.MAST_ER_ROAD_CODE_PMGSY1 != 0 && (item.MAST_ROAD_CAT_CODE_PMGSY2 != item.MAST_ROAD_CAT_CODE_PMGSY1)) ? dbContext.MASTER_EXISTING_ROADS.Where(m=>m.MAST_ER_ROAD_CODE == item.MAST_ER_ROAD_CODE_PMGSY1).Select(m=>m.MAST_ER_ROAD_NUMBER).FirstOrDefault().ToString() : item.MAST_ER_ROAD_CODE_PMGSY1 != 0
                        //    ? "<a href='#' onClick =MapDRRPPMGSY1('"+URLEncrypt.EncryptParameters1(new string[]{"BlockCode =" + blockCode.ToString().Trim(), "RoadCode =" + item.MAST_ER_ROAD_CODE.ToString().Trim(), "RoadCategory=" + item.MAST_ROAD_CAT_CODE_PMGSY2.ToString().Trim()})+"'); return false;'>"+ dbContext.MASTER_EXISTING_ROADS.Where(m=>m.MAST_ER_ROAD_CODE == item.MAST_ER_ROAD_CODE_PMGSY1).Select(m=>m.MAST_ER_ROAD_NUMBER).FirstOrDefault().ToString() +"</a>" 
                        //    : "<a href='#' title='Click here to Map DRRP PMGSY - I Road' class='ui-icon ui-icon-plusthick ui-align-center' onClick =MapDRRPPMGSY1('"+URLEncrypt.EncryptParameters1(new string[]{"BlockCode =" + blockCode.ToString().Trim(), "RoadCode =" + item.MAST_ER_ROAD_CODE.ToString().Trim(), "RoadCategory=" + item.MAST_ROAD_CAT_CODE_PMGSY2.ToString().Trim()})+"'); return false;'></a>") 
                        //: "-",
                        
                        ((PMGSYSession.Current.PMGSYScheme == 2 && (PMGSYSession.Current.RoleCode == 22 || PMGSYSession.Current.RoleCode == 25 || PMGSYSession.Current.RoleCode == 25 ))
                        ? (item.MAST_ER_ROAD_CODE_PMGSY1 == 0) 
                            ? "<a href='#' title='Click here to Map DRRP PMGSY - I Road' class='ui-icon ui-icon-plusthick ui-align-center' onClick =MapDRRPPMGSY1('"+URLEncrypt.EncryptParameters1(new string[]{"BlockCode =" + blockCode.ToString().Trim(), "RoadCode =" + item.MAST_ER_ROAD_CODE.ToString().Trim(), "RoadCategory=" + item.MAST_ROAD_CAT_CODE_PMGSY2.ToString().Trim()})+"'); return false;'></a>" 
                            : //(dbContext.MASTER_EXISTING_ROADS.Where(m=>m.MAST_ER_ROAD_CODE == item.MAST_ER_ROAD_CODE_PMGSY1).Select(m=>m.MAST_ER_ROAD_NUMBER).FirstOrDefault())
                              "<a href='#' onClick =MapDRRPPMGSY1('"+URLEncrypt.EncryptParameters1(new string[]{"BlockCode =" + blockCode.ToString().Trim(), "RoadCode =" + item.MAST_ER_ROAD_CODE.ToString().Trim(), "RoadCategory=" + item.MAST_ROAD_CAT_CODE_PMGSY2.ToString().Trim()})+"'); return false;'>"+ dbContext.MASTER_EXISTING_ROADS.Where(m=>m.MAST_ER_ROAD_CODE == item.MAST_ER_ROAD_CODE_PMGSY1).Select(m=>m.MAST_ER_ROAD_NUMBER).FirstOrDefault() +"</a>" 
                        : "-"),

                        //dbContext.MASTER_EXISTING_ROADS.Where(m=>m.MAST_ER_ROAD_CODE == item.MAST_ER_ROAD_CODE).Select(m=>m.MAST_LOCK_STATUS).FirstOrDefault() == "N"?"<a href='#' title='Click here to edit details' class='ui-icon ui-icon-pencil ui-align-center' onClick =EditDetails('"+URLEncrypt.EncryptParameters1(new string[]{"RoadCode =" + item.MAST_ER_ROAD_CODE.ToString().Trim()})+"'); return false;'></a>":item.UNLOCK_BY_MORD == "M"?"<a href='#' title='Click here to edit details' class='ui-icon ui-icon-unlocked ui-align-center' onClick =EditDetails('"+URLEncrypt.EncryptParameters1(new string[]{"RoadCode =" + item.MAST_ER_ROAD_CODE.ToString().Trim(),"UnlockFlag =" + item.UNLOCK_BY_MORD.ToString().Trim()})+"'); return false;'></a>":item.UNLOCK_BY_MORD.ToString().ToUpper()=="Y"?"<span class='ui-icon ui-icon-locked ui-align-center'></span>":"<a href='#' title='Click here to edit details' class='ui-icon ui-icon-pencil ui-align-center' onClick =EditDetails('"+URLEncrypt.EncryptParameters1(new string[]{"RoadCode =" + item.MAST_ER_ROAD_CODE.ToString().Trim()})+"'); return false;'></a>",   
                        ///Changes by SAMMED A. PATIL on 03 OCTOBER 2017 to edit DRRP for MORD
                        PMGSYSession.Current.RoleCode == 25 
                            ? "<a href='#' title='Click here to edit details' class='ui-icon ui-icon-pencil ui-align-center' onClick =EditDetails('"+URLEncrypt.EncryptParameters1(new string[]{"RoadCode =" + item.MAST_ER_ROAD_CODE.ToString().Trim()})+"'); return false;'></a>"
                            : (dbContext.MASTER_EXISTING_ROADS.Where(m=>m.MAST_ER_ROAD_CODE == item.MAST_ER_ROAD_CODE).Select(m=>m.MAST_LOCK_STATUS).FirstOrDefault() == "N"?"<a href='#' title='Click here to edit details' class='ui-icon ui-icon-pencil ui-align-center' onClick =EditDetails('"+URLEncrypt.EncryptParameters1(new string[]{"RoadCode =" + item.MAST_ER_ROAD_CODE.ToString().Trim()})+"'); return false;'></a>":item.UNLOCK_BY_MORD == "M"?"<a href='#' title='Click here to edit details' class='ui-icon ui-icon-unlocked ui-align-center' onClick =EditDetails('"+URLEncrypt.EncryptParameters1(new string[]{"RoadCode =" + item.MAST_ER_ROAD_CODE.ToString().Trim(),"UnlockFlag =" + item.UNLOCK_BY_MORD.ToString().Trim()})+"'); return false;'></a>":item.UNLOCK_BY_MORD.ToString().ToUpper()=="Y"?"<span class='ui-icon ui-icon-locked ui-align-center'></span>":"<a href='#' title='Click here to edit details' class='ui-icon ui-icon-pencil ui-align-center' onClick =EditDetails('"+URLEncrypt.EncryptParameters1(new string[]{"RoadCode =" + item.MAST_ER_ROAD_CODE.ToString().Trim()})+"'); return false;'></a>"),

                        PMGSYSession.Current.PMGSYScheme == 2 
                        ? 
                                (dbContext.MASTER_EXISTING_ROADS.Where(m=>m.MAST_ER_ROAD_CODE == item.MAST_ER_ROAD_CODE).Select(m=>m.MAST_LOCK_STATUS).FirstOrDefault() == "N" || item.UNLOCK_BY_MORD == "M")
                                ? ("<a href='#' title='Click here to delete details' class='ui-icon ui-icon-trash ui-align-center' onClick =DeleteDetailsITNO('"+URLEncrypt.EncryptParameters1(new string[]{"RoadCode =" + item.MAST_ER_ROAD_CODE.ToString().Trim                                   ()})+"'); return false;'></a>")
                                : "<span class='ui-icon ui-icon-locked ui-align-center'></span>"
                        : dbContext.MASTER_EXISTING_ROADS.Where(m=>m.MAST_ER_ROAD_CODE == item.MAST_ER_ROAD_CODE).Select(m=>m.MAST_LOCK_STATUS).FirstOrDefault() == "N"?"<a href='#' title='Click here to delete details' class='ui-icon ui-icon-trash ui-align-center' onClick =DeleteDetailsITNO('"+URLEncrypt.EncryptParameters1(new string[]{"RoadCode =" + item.MAST_ER_ROAD_CODE.ToString().Trim()})+"'); return false;'></a>":item.UNLOCK_BY_MORD == "M"?"<a href='#' title='Click here to delete details' class='ui-icon ui-icon-trash ui-align-center' onClick =DeleteDetailsITNO('"+URLEncrypt.EncryptParameters1(new string[]{"RoadCode =" + item.MAST_ER_ROAD_CODE.ToString().Trim()})+"'); return false;'></a>":item.UNLOCK_BY_MORD.ToString().ToUpper()=="Y"?"<span class='ui-icon ui-icon-locked ui-align-center'></span>":"<a href='#' title='Click here to delete details' class='ui-icon ui-icon-trash ui-align-center' onClick =DeleteDetailsITNO('"+URLEncrypt.EncryptParameters1(new string[]{"RoadCode =" + item.MAST_ER_ROAD_CODE.ToString().Trim()})+"'); return false;'></a>", 


                        URLEncrypt.EncryptParameters1(new string[] {  "ERCode =" + item.MAST_ER_ROAD_CODE.ToString() })
                      
#endregion

#region

                       /* //CD Works
                        (isPMGSY3 == true && PMGSYSession.Current.PMGSYScheme == 2)
                        ? "<span class='ui-icon ui-icon-locked ui-align-center'></span>"
                        : (dbContext.MASTER_EXISTING_ROADS.Where(m=>m.MAST_ER_ROAD_CODE == item.MAST_ER_ROAD_CODE).Select(m=>m.MAST_LOCK_STATUS).FirstOrDefault() == "N"?"<a href='#' title='Click here to add CDWorks details' class='ui-icon ui-icon-plusthick ui-align-center' onClick =CDWorks('"+URLEncrypt.EncryptParameters1(new string[]{"RoadCode =" + item.MAST_ER_ROAD_CODE.ToString().Trim()})+"'); return false;'></a>":item.UNLOCK_BY_MORD == "M"?"<a href='#' title='Click here to add CDWorks details' class='ui-icon ui-icon-unlocked ui-align-center' onClick =CDWorks('"+URLEncrypt.EncryptParameters1(new string[]{"RoadCode =" + item.MAST_ER_ROAD_CODE.ToString().Trim()})+"'); return false;'></a>":item.UNLOCK_BY_MORD.ToString().ToUpper()=="Y"?"<span class='ui-icon ui-icon-locked ui-align-center'></span>":"<a href='#' title='Click here to add CDWorks details' class='ui-icon ui-icon-plusthick ui-align-center' onClick =CDWorks('"+URLEncrypt.EncryptParameters1(new string[]{"RoadCode =" + item.MAST_ER_ROAD_CODE.ToString().Trim()})+"'); return false;'></a>"),   

                        //Surface Types
                        (isPMGSY3 == true && PMGSYSession.Current.PMGSYScheme == 2)
                        ? "<span class='ui-icon ui-icon-locked ui-align-center'></span>"
                        : (dbContext.MASTER_EXISTING_ROADS.Where(m=>m.MAST_ER_ROAD_CODE == item.MAST_ER_ROAD_CODE).Select(m=>m.MAST_LOCK_STATUS).FirstOrDefault() == "N"?"<a href='#' title='Click here to add surface type details' class='ui-icon ui-icon-plusthick ui-align-center' onClick =SurfaceTypes('"+URLEncrypt.EncryptParameters1(new string[]{"RoadCode =" + item.MAST_ER_ROAD_CODE.ToString().Trim()})+"'); return false;'></a>":item.UNLOCK_BY_MORD == "M"?"<a href='#' title='Click here to add surface type details' class='ui-icon ui-icon-unlocked ui-align-center' onClick =SurfaceTypes('"+URLEncrypt.EncryptParameters1(new string[]{"RoadCode =" + item.MAST_ER_ROAD_CODE.ToString().Trim()})+"'); return false;'></a>":item.UNLOCK_BY_MORD.ToString().ToUpper()=="Y"?"<span class='ui-icon ui-icon-locked ui-align-center'></span>":"<a href='#' title='Click here to add surface type details' class='ui-icon ui-icon-plusthick ui-align-center' onClick =SurfaceTypes('"+URLEncrypt.EncryptParameters1(new string[]{"RoadCode =" + item.MAST_ER_ROAD_CODE.ToString().Trim()})+"'); return false;'></a>"),   
                        
                        //Habitations
                        (isPMGSY3 == true && PMGSYSession.Current.PMGSYScheme == 2)
                        ? "<span class='ui-icon ui-icon-locked ui-align-center'></span>"
                        : (dbContext.MASTER_EXISTING_ROADS.Where(m=>m.MAST_ER_ROAD_CODE == item.MAST_ER_ROAD_CODE).Select(m=>m.MAST_LOCK_STATUS).FirstOrDefault() == "N"?"<a href='#' title='Click here to add habitation details' class='ui-icon ui-icon-plusthick ui-align-center' onClick =HabitationsMapped('"+URLEncrypt.EncryptParameters1(new string[]{"RoadCode =" + item.MAST_ER_ROAD_CODE.ToString().Trim()})+"'); return false;'></a>":item.UNLOCK_BY_MORD == "M"?"<a href='#' title='Click here to add Habitation details' class='ui-icon ui-icon-unlocked ui-align-center' onClick =HabitationsMapped('"+URLEncrypt.EncryptParameters1(new string[]{"RoadCode =" + item.MAST_ER_ROAD_CODE.ToString().Trim()})+"'); return false;'></a>":item.UNLOCK_BY_MORD.ToString().ToUpper()=="Y"?"<span class='ui-icon ui-icon-locked ui-align-center'></span>":"<a href='#' title='Click here to add habitation details' class='ui-icon ui-icon-plusthick ui-align-center' onClick =HabitationsMapped('"+URLEncrypt.EncryptParameters1(new string[]{"RoadCode =" + item.MAST_ER_ROAD_CODE.ToString().Trim()})+"'); return false;'></a>"),   

                        //Traffic
                        (isPMGSY3 == true && PMGSYSession.Current.PMGSYScheme == 2)
                        ? "<span class='ui-icon ui-icon-locked ui-align-center'></span>"
                        : (dbContext.MASTER_EXISTING_ROADS.Where(m=>m.MAST_ER_ROAD_CODE == item.MAST_ER_ROAD_CODE).Select(m=>m.MAST_LOCK_STATUS).FirstOrDefault() == "N"?"<a href='#' title='Click here to add traffic intensity details' class='ui-icon ui-icon-plusthick ui-align-center' onClick =TrafficIntensity('"+URLEncrypt.EncryptParameters1(new string[]{"RoadCode =" + item.MAST_ER_ROAD_CODE.ToString().Trim()})+"'); return false;'></a>":item.UNLOCK_BY_MORD == "M"?"<a href='#' title='Click here to add Traffic Intensity details' class='ui-icon ui-icon-unlocked ui-align-center' onClick =TrafficIntensity('"+URLEncrypt.EncryptParameters1(new string[]{"RoadCode =" + item.MAST_ER_ROAD_CODE.ToString().Trim()})+"'); return false;'></a>":item.UNLOCK_BY_MORD.ToString().ToUpper()=="Y"?"<span class='ui-icon ui-icon-locked ui-align-center'></span>":"<a href='#' title='Click here to add traffic intensity details' class='ui-icon ui-icon-plusthick ui-align-center' onClick =TrafficIntensity('"+URLEncrypt.EncryptParameters1(new string[]{"RoadCode =" + item.MAST_ER_ROAD_CODE.ToString().Trim()})+"'); return false;'></a>"),   
                        
                        //CBR
                        (isPMGSY3 == true && PMGSYSession.Current.PMGSYScheme == 2)
                        ? "<span class='ui-icon ui-icon-locked ui-align-center'></span>"
                        : (dbContext.MASTER_EXISTING_ROADS.Where(m=>m.MAST_ER_ROAD_CODE == item.MAST_ER_ROAD_CODE).Select(m=>m.MAST_LOCK_STATUS).FirstOrDefault() == "N"?"<a href='#' title='Click here to add CBR details' class='ui-icon ui-icon-plusthick ui-align-center' onClick =CBRValue('"+URLEncrypt.EncryptParameters1(new string[]{"RoadCode =" + item.MAST_ER_ROAD_CODE.ToString().Trim()})+"'); return false;'></a>":item.UNLOCK_BY_MORD == "M"?"<a href='#' title='Click here to add CBR details' class='ui-icon ui-icon-unlocked ui-align-center' onClick =CBRValue('"+URLEncrypt.EncryptParameters1(new string[]{"RoadCode =" + item.MAST_ER_ROAD_CODE.ToString().Trim()})+"'); return false;'></a>":item.UNLOCK_BY_MORD.ToString().ToUpper()=="Y"?"<span class='ui-icon ui-icon-locked ui-align-center'></span>":"<a href='#' title='Click here to add CBR details' class='ui-icon ui-icon-plusthick ui-align-center' onClick =CBRValue('"+URLEncrypt.EncryptParameters1(new string[]{"RoadCode =" + item.MAST_ER_ROAD_CODE.ToString().Trim()})+"'); return false;'></a>"),   

                        ///View
                        "<a href='#' title='Click here to view existing roads details' class='ui-icon ui-icon-zoomin ui-align-center' onClick =ShowDetails('"+URLEncrypt.EncryptParameters1(new string[]{"RoadCode =" + item.MAST_ER_ROAD_CODE.ToString().Trim()})+"'); return false;'></a>",
                        
                        ///Changed by SAMMED A. PATIL on 15FEB2017 for Map PMGSY1 DRRP Roads
                        //(PMGSYSession.Current.PMGSYScheme == 2 && (PMGSYSession.Current.RoleCode == 22 || PMGSYSession.Current.RoleCode == 25)) 
                        //? //(dbContext.MASTER_EXISTING_ROADS.Where(m=>m.MAST_ER_ROAD_CODE == item.MAST_ER_ROAD_CODE && m.MAST_ER_ROAD_CODE_PMGSY1 != null).Any() 
                        //((item.MAST_ER_ROAD_CODE_PMGSY1 != 0 && (item.MAST_ROAD_CAT_CODE_PMGSY2 != item.MAST_ROAD_CAT_CODE_PMGSY1)) ? dbContext.MASTER_EXISTING_ROADS.Where(m=>m.MAST_ER_ROAD_CODE == item.MAST_ER_ROAD_CODE_PMGSY1).Select(m=>m.MAST_ER_ROAD_NUMBER).FirstOrDefault().ToString() : item.MAST_ER_ROAD_CODE_PMGSY1 != 0
                        //    ? "<a href='#' onClick =MapDRRPPMGSY1('"+URLEncrypt.EncryptParameters1(new string[]{"BlockCode =" + blockCode.ToString().Trim(), "RoadCode =" + item.MAST_ER_ROAD_CODE.ToString().Trim(), "RoadCategory=" + item.MAST_ROAD_CAT_CODE_PMGSY2.ToString().Trim()})+"'); return false;'>"+ dbContext.MASTER_EXISTING_ROADS.Where(m=>m.MAST_ER_ROAD_CODE == item.MAST_ER_ROAD_CODE_PMGSY1).Select(m=>m.MAST_ER_ROAD_NUMBER).FirstOrDefault().ToString() +"</a>" 
                        //    : "<a href='#' title='Click here to Map DRRP PMGSY - I Road' class='ui-icon ui-icon-plusthick ui-align-center' onClick =MapDRRPPMGSY1('"+URLEncrypt.EncryptParameters1(new string[]{"BlockCode =" + blockCode.ToString().Trim(), "RoadCode =" + item.MAST_ER_ROAD_CODE.ToString().Trim(), "RoadCategory=" + item.MAST_ROAD_CAT_CODE_PMGSY2.ToString().Trim()})+"'); return false;'></a>") 
                        //: "-",
                        
                        //Map DRRP 
                        (isPMGSY3 == true && PMGSYSession.Current.PMGSYScheme == 2)
                        ? "<span class='ui-icon ui-icon-locked ui-align-center'></span>"
                        : (
                            ((PMGSYSession.Current.PMGSYScheme == 2 && (PMGSYSession.Current.RoleCode == 22 || PMGSYSession.Current.RoleCode == 25))
                            ? (item.MAST_ER_ROAD_CODE_PMGSY1 == 0) 
                                ? "<a href='#' title='Click here to Map DRRP PMGSY - I Road' class='ui-icon ui-icon-plusthick ui-align-center' onClick =MapDRRPPMGSY1('"+URLEncrypt.EncryptParameters1(new string[]{"BlockCode =" + blockCode.ToString().Trim(), "RoadCode =" + item.MAST_ER_ROAD_CODE.ToString().Trim(), "RoadCategory=" + item.MAST_ROAD_CAT_CODE_PMGSY2.ToString().Trim()})+"'); return false;'></a>" 
                                : //(dbContext.MASTER_EXISTING_ROADS.Where(m=>m.MAST_ER_ROAD_CODE == item.MAST_ER_ROAD_CODE_PMGSY1).Select(m=>m.MAST_ER_ROAD_NUMBER).FirstOrDefault())
                                  "<a href='#' onClick =MapDRRPPMGSY1('"+URLEncrypt.EncryptParameters1(new string[]{"BlockCode =" + blockCode.ToString().Trim(), "RoadCode =" + item.MAST_ER_ROAD_CODE.ToString().Trim(), "RoadCategory=" + item.MAST_ROAD_CAT_CODE_PMGSY2.ToString().Trim()})+"'); return false;'>"+ dbContext.MASTER_EXISTING_ROADS.Where(m=>m.MAST_ER_ROAD_CODE == item.MAST_ER_ROAD_CODE_PMGSY1).Select(m=>m.MAST_ER_ROAD_NUMBER).FirstOrDefault() +"</a>" 
                            : "-")
                           ),

                        //dbContext.MASTER_EXISTING_ROADS.Where(m=>m.MAST_ER_ROAD_CODE == item.MAST_ER_ROAD_CODE).Select(m=>m.MAST_LOCK_STATUS).FirstOrDefault() == "N"?"<a href='#' title='Click here to edit details' class='ui-icon ui-icon-pencil ui-align-center' onClick =EditDetails('"+URLEncrypt.EncryptParameters1(new string[]{"RoadCode =" + item.MAST_ER_ROAD_CODE.ToString().Trim()})+"'); return false;'></a>":item.UNLOCK_BY_MORD == "M"?"<a href='#' title='Click here to edit details' class='ui-icon ui-icon-unlocked ui-align-center' onClick =EditDetails('"+URLEncrypt.EncryptParameters1(new string[]{"RoadCode =" + item.MAST_ER_ROAD_CODE.ToString().Trim(),"UnlockFlag =" + item.UNLOCK_BY_MORD.ToString().Trim()})+"'); return false;'></a>":item.UNLOCK_BY_MORD.ToString().ToUpper()=="Y"?"<span class='ui-icon ui-icon-locked ui-align-center'></span>":"<a href='#' title='Click here to edit details' class='ui-icon ui-icon-pencil ui-align-center' onClick =EditDetails('"+URLEncrypt.EncryptParameters1(new string[]{"RoadCode =" + item.MAST_ER_ROAD_CODE.ToString().Trim()})+"'); return false;'></a>",   
                        
                        ///Changes by SAMMED A. PATIL on 03 OCTOBER 2017 to edit DRRP for MORD
                        
                        //Edit
                        (isPMGSY3 == true && PMGSYSession.Current.PMGSYScheme == 2)
                        ? "<span class='ui-icon ui-icon-locked ui-align-center'></span>"
                        : (PMGSYSession.Current.RoleCode == 25 
                            ? "<a href='#' title='Click here to edit details' class='ui-icon ui-icon-pencil ui-align-center' onClick =EditDetails('"+URLEncrypt.EncryptParameters1(new string[]{"RoadCode =" + item.MAST_ER_ROAD_CODE.ToString().Trim()})+"'); return false;'></a>"
                            : (dbContext.MASTER_EXISTING_ROADS.Where(m=>m.MAST_ER_ROAD_CODE == item.MAST_ER_ROAD_CODE).Select(m=>m.MAST_LOCK_STATUS).FirstOrDefault() == "N"?"<a href='#' title='Click here to edit details' class='ui-icon ui-icon-pencil ui-align-center' onClick =EditDetails('"+URLEncrypt.EncryptParameters1(new string[]{"RoadCode =" + item.MAST_ER_ROAD_CODE.ToString().Trim()})+"'); return false;'></a>":item.UNLOCK_BY_MORD == "M"?"<a href='#' title='Click here to edit details' class='ui-icon ui-icon-unlocked ui-align-center' onClick =EditDetails('"+URLEncrypt.EncryptParameters1(new string[]{"RoadCode =" + item.MAST_ER_ROAD_CODE.ToString().Trim(),"UnlockFlag =" + item.UNLOCK_BY_MORD.ToString().Trim()})+"'); return false;'></a>":item.UNLOCK_BY_MORD.ToString().ToUpper()=="Y"?"<span class='ui-icon ui-icon-locked ui-align-center'></span>":"<a href='#' title='Click here to edit details' class='ui-icon ui-icon-pencil ui-align-center' onClick =EditDetails('"+URLEncrypt.EncryptParameters1(new string[]{"RoadCode =" + item.MAST_ER_ROAD_CODE.ToString().Trim()})+"'); return false;'></a>")),

                        //Delete
                        (isPMGSY3 == true && PMGSYSession.Current.PMGSYScheme == 2)
                        ? "<span class='ui-icon ui-icon-locked ui-align-center'></span>"
                        : (PMGSYSession.Current.PMGSYScheme == 2 
                        ? /*(dbContext.MASTER_EXISTING_ROADS.Where(m=>m.MAST_ER_ROAD_CODE == item.MAST_ER_ROAD_CODE && m.MAST_ER_ROAD_CODE_PMGSY1 != null).Any() 
                            ? "<span class='ui-icon ui-icon-locked ui-align-center'></span>"
                            : (
                                (dbContext.MASTER_EXISTING_ROADS.Where(m=>m.MAST_ER_ROAD_CODE == item.MAST_ER_ROAD_CODE).Select(m=>m.MAST_LOCK_STATUS).FirstOrDefault() == "N" || item.UNLOCK_BY_MORD == "M")
                                ? ("<a href='#' title='Click here to delete details' class='ui-icon ui-icon-trash ui-align-center' onClick =DeleteDetails('"+URLEncrypt.EncryptParameters1(new string[]{"RoadCode =" + item.MAST_ER_ROAD_CODE.ToString().Trim                                   ()})+"'); return false;'></a>")
                                : "<span class='ui-icon ui-icon-locked ui-align-center'></span>"
                               /*)
                           )
                        : dbContext.MASTER_EXISTING_ROADS.Where(m=>m.MAST_ER_ROAD_CODE == item.MAST_ER_ROAD_CODE).Select(m=>m.MAST_LOCK_STATUS).FirstOrDefault() == "N"?"<a href='#' title='Click here to delete details' class='ui-icon ui-icon-trash ui-align-center' onClick =DeleteDetails('"+URLEncrypt.EncryptParameters1(new string[]{"RoadCode =" + item.MAST_ER_ROAD_CODE.ToString().Trim()})+"'); return false;'></a>":item.UNLOCK_BY_MORD == "M"?"<a href='#' title='Click here to delete details' class='ui-icon ui-icon-trash ui-align-center' onClick =DeleteDetails('"+URLEncrypt.EncryptParameters1(new string[]{"RoadCode =" + item.MAST_ER_ROAD_CODE.ToString().Trim()})+"'); return false;'></a>":item.UNLOCK_BY_MORD.ToString().ToUpper()=="Y"?"<span class='ui-icon ui-icon-locked ui-align-center'></span>":"<a href='#' title='Click here to delete details' class='ui-icon ui-icon-trash ui-align-center' onClick =DeleteDetails('"+URLEncrypt.EncryptParameters1(new string[]{"RoadCode =" + item.MAST_ER_ROAD_CODE.ToString().Trim()})+"'); return false;'></a>"), */
#endregion
                    }
                }).ToArray();
            }
            catch (Exception ex)
            {
                //Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                ErrorLog.LogError(ex, "ListExistingRoads().DAL");
                totalRecords = 0;
                return null;
            }
            finally
            {
                dbContext.Dispose();
            }
        }


        public bool ShiftDetailsDAL(string encryptedEReCode, string newBlockCode, string newDistrictCode, string ERCode)
        {
            Models.PMGSYEntities dbContext = new Models.PMGSYEntities();
            try
            {

                using (TransactionScope ts = new TransactionScope())
                {
                    int blockCode = 0;
                    int NewBlockCode = Convert.ToInt32(newBlockCode);
                    int NewDistrictCode = Convert.ToInt32(newDistrictCode);
                    int ERCODE = Convert.ToInt32(ERCode);

                    MASTER_EXISTING_ROADS oldmaster = new MASTER_EXISTING_ROADS();
                    MASTER_EXISTING_ROADS_Temp newMaster = new MASTER_EXISTING_ROADS_Temp();

                    if (dbContext.MASTER_EXISTING_ROADS_Temp.Where(m => m.MAST_ER_ROAD_CODE == ERCODE).Any())
                    {
                        return false;
                    }

                    List<MASTER_ER_HABITATION_ROAD> master = new List<MASTER_ER_HABITATION_ROAD>();

                    if (dbContext.MASTER_ER_HABITATION_ROAD.Where(m => m.MAST_ER_ROAD_CODE == ERCODE).Any())
                    {
                        master = dbContext.MASTER_ER_HABITATION_ROAD.Where(m => m.MAST_ER_ROAD_CODE == ERCODE).ToList();
                    }
                    dbContext.MASTER_ER_HABITATION_ROAD.DeleteMany(master);
                    dbContext.SaveChanges();
                    // Multiple Delete from 


                    oldmaster = dbContext.MASTER_EXISTING_ROADS.Find(ERCODE);

                    newMaster.MAST_ER_ROAD_CODE = oldmaster.MAST_ER_ROAD_CODE;
                    newMaster.MAST_PMGSY_SCHEME = oldmaster.MAST_PMGSY_SCHEME;
                    newMaster.MAST_STATE_CODE = oldmaster.MAST_STATE_CODE;
                    newMaster.MAST_DISTRICT_CODE = oldmaster.MAST_DISTRICT_CODE;
                    newMaster.MAST_BLOCK_CODE = oldmaster.MAST_BLOCK_CODE;
                    newMaster.MAST_ER_ROAD_NUMBER = oldmaster.MAST_ER_ROAD_NUMBER;
                    newMaster.MAST_ROAD_CAT_CODE = oldmaster.MAST_ROAD_CAT_CODE;
                    newMaster.MAST_ER_ROAD_NAME = oldmaster.MAST_ER_ROAD_NAME;
                    newMaster.MAST_ER_ROAD_STR_CHAIN = oldmaster.MAST_ER_ROAD_STR_CHAIN;
                    newMaster.MAST_ER_ROAD_END_CHAIN = oldmaster.MAST_ER_ROAD_END_CHAIN;
                    newMaster.MAST_ER_ROAD_C_WIDTH = oldmaster.MAST_ER_ROAD_C_WIDTH;
                    newMaster.MAST_ER_ROAD_F_WIDTH = oldmaster.MAST_ER_ROAD_F_WIDTH;
                    newMaster.MAST_ER_ROAD_L_WIDTH = oldmaster.MAST_ER_ROAD_L_WIDTH;
                    newMaster.MAST_ER_ROAD_TYPE = oldmaster.MAST_ER_ROAD_TYPE;
                    newMaster.MAST_SOIL_TYPE_CODE = oldmaster.MAST_SOIL_TYPE_CODE;
                    newMaster.MAST_TERRAIN_TYPE_CODE = oldmaster.MAST_TERRAIN_TYPE_CODE;
                    newMaster.MAST_CORE_NETWORK = oldmaster.MAST_CORE_NETWORK;
                    newMaster.MAST_IS_BENEFITTED_HAB = oldmaster.MAST_IS_BENEFITTED_HAB;
                    newMaster.MAST_NOHABS_REASON = oldmaster.MAST_NOHABS_REASON;
                    newMaster.MAST_ER_ROAD_OWNER = oldmaster.MAST_ER_ROAD_OWNER;
                    newMaster.MAST_CONS_YEAR = oldmaster.MAST_CONS_YEAR;
                    newMaster.MAST_RENEW_YEAR = oldmaster.MAST_RENEW_YEAR;
                    newMaster.MAST_CD_WORKS_NUM = oldmaster.MAST_CD_WORKS_NUM;
                    newMaster.MAST_LOCK_STATUS = oldmaster.MAST_LOCK_STATUS;
                    newMaster.MAST_ER_ROAD_CODE_PMGSY1 = oldmaster.MAST_ER_ROAD_CODE_PMGSY1;
                    newMaster.MAST_DISTRICT_CODE_NEW = NewDistrictCode;
                    newMaster.MAST_BLOCK_CODE_NEW = NewBlockCode;
                    newMaster.USERID = PMGSYSession.Current.UserId;
                    newMaster.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];


                    dbContext.MASTER_EXISTING_ROADS_Temp.Add(newMaster);

                    oldmaster.MAST_DISTRICT_CODE = NewDistrictCode;
                    oldmaster.MAST_BLOCK_CODE = NewBlockCode;


                    dbContext.Entry(oldmaster).State = System.Data.Entity.EntityState.Modified;
                    dbContext.SaveChanges();

                    ts.Complete();
                    return true;

                }

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

        #endregion


        #region Not Feasible Roads under PMGSY III
        public Array GetNotFeasibleRoadsListDAL(int? page, int? rows, string sidx, string sord, out long totalRecords, int stateCode, int districtCode, int IMS_YEAR, int MAST_BLOCK_ID, int IMS_BATCH, int IMS_STREAMS, string IMS_PROPOSAL_TYPE, int adminCode, string IMS_PROPOSAL_STATUS, string IMS_UPGRADE_CONNECT)
        {
            int maxBatch = 0;
            dbContext = new Models.PMGSYEntities();
            try
            {
                int state_Code = stateCode == -1 ? 0 : stateCode;
                int district_Code = districtCode == -1 ? 0 : districtCode;
                int YEAR_Code = IMS_YEAR == -1 ? 0 : IMS_YEAR;
                int BLOCK_Code = MAST_BLOCK_ID == -1 ? 0 : MAST_BLOCK_ID;
                int BATCH_Code = IMS_BATCH == -1 ? 0 : IMS_BATCH;
                int STREAMS_Code = IMS_STREAMS == -1 ? 0 : IMS_STREAMS;

                dbContext = new Models.PMGSYEntities();
                var itemList = dbContext.USP_GET_CUPL_PMGSY3_LIST(district_Code, BLOCK_Code, YEAR_Code, BATCH_Code).OrderBy(m => m.CUPL_RANK).ToList();

                IQueryable<USP_GET_CUPL_PMGSY3_LIST_Result> query = itemList.AsQueryable<USP_GET_CUPL_PMGSY3_LIST_Result>();

                totalRecords = itemList.Count;

                if (sidx.Trim() != string.Empty)
                {
                    if (sord.ToString() == "asc")
                    {

                        //  itemList = itemList.OrderBy(x => x.MAST_ER_ROAD_NAME).Take(Convert.ToInt16(rows)).Skip(Convert.ToInt32((page - 1) * rows)).ToList();
                        itemList = itemList.OrderBy(x => x.CUPL_RANK).Skip(Convert.ToInt32((page - 1) * rows)).ToList();
                    }
                    else
                    {
                        // itemList = itemList.OrderByDescending(x => x.MAST_ER_ROAD_NAME).Take(Convert.ToInt16(rows)).Skip(Convert.ToInt32((page - 1) * rows)).ToList();
                        itemList = itemList.OrderByDescending(x => x.CUPL_RANK).Skip(Convert.ToInt32((page - 1) * rows)).ToList();
                    }
                }
                else
                {
                    //  itemList = itemList.OrderBy(x => x.MAST_ER_ROAD_NAME).Take(Convert.ToInt16(rows)).Skip(Convert.ToInt32((page - 1) * rows)).ToList();
                    itemList = itemList.OrderBy(x => x.CUPL_RANK).Skip(Convert.ToInt32((page - 1) * rows)).ToList();
                }
                maxBatch = dbContext.CUPL_PMGSY3.Where(z => z.MAST_BLOCK_CODE == BLOCK_Code && z.IMS_YEAR == YEAR_Code).Select(x => x.IMS_BATCH).Max();
                return itemList.Select(propDetails => new
                {
                    id = propDetails.CUPL_PMGSY3_ID.ToString(), // here PLAN_CN_ROAD_CODE is used.  see SP USP_GET_CUPL_PMGSY3_LIST for more clarification
                    cell = new[] {     

                                    propDetails.CUPL_RANK.ToString(),
                                    propDetails.MAST_DISTRICT_NAME.Trim(),
                                    propDetails.MAST_BLOCK_NAME.Trim(),
                                    propDetails.PLAN_CN_ROAD_NUMBER.Trim(),
                                    propDetails.PLAN_RD_LENGTH.ToString(),     
                                    propDetails.PLAN_RD_LENGTH<5?"Not Eligible":"Eligible",
                                    propDetails.ELIGIBLE_CANDIDATE_LENGTH.ToString(),
                                    propDetails.PLAN_RD_NAME.ToString(),

                                    //propDetails.SCORE_PER_UNIT_LENGTH.ToString(),
                                   // propDetails.MAST_ER_ROAD_SCORE.ToString(),

                                   (propDetails.REQUEST_REMARKS!=null)?("Request Remarks : "+propDetails.REQUEST_REMARKS.ToString() +", Reason : "+propDetails.REASON.ToString() ):"-",
                                   
                                   (dbContext.IMS_SANCTIONED_PROJECTS.Where(z=>z.PLAN_CN_ROAD_CODE == propDetails.PLAN_CN_ROAD_CODE).Any())
                                   ? "Proposed"
                                   : (BATCH_Code < maxBatch) 
                                        ? "CUPL Generated for Batch " + maxBatch
                                        :  ((propDetails.REQUEST_REMARKS!=null) ? "Approved"
                                                                       : ((propDetails.APPROVAL.Equals("Y"))?"<a href='#' title='Click here to Add details' class='ui-icon ui-icon-plusthick ui-align-center' onClick='MapDetails(\"" + URLEncrypt.EncryptParameters(new string[] {propDetails.CUPL_PMGSY3_ID.ToString() + "$" + YEAR_Code.ToString() + "$" + BATCH_Code.ToString()   })  +"\"); return false;'>Map</a>":"-"))
                
                    }
                }).ToArray();



            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "ExistingRoadDAL.GetNotFeasibleRoadsListDAL()");
                totalRecords = 0;
                return null;
            }
            finally
            {
                dbContext.Dispose();
            }
        }

        public bool MapRoadDetailsDAL(MapNotFeasibleRoads model)
        {
            dbContext = new PMGSYEntities();
            try
            {
                //using (TransactionScope ts = new TransactionScope())
                {
                    TR_MRL_EXEMPTION masterTable = new TR_MRL_EXEMPTION();

                    //var listDetails = dbContext.CUPL_PMGSY3.Where(m => m.PLAN_CN_ROAD_CODE == model.PLAN_CN_ROAD_CODE).Select(m => m.MAST_ER_ROAD_SCORE).ToList();
                    //decimal minScore = listDetails.Min();
                    //int CUPL_ID = dbContext.CUPL_PMGSY3.Where(m => m.PLAN_CN_ROAD_CODE == model.PLAN_CN_ROAD_CODE && m.MAST_ER_ROAD_SCORE == minScore).Select(m => m.CUPL_PMGSY3_ID).FirstOrDefault();

                    var listDetails = dbContext.CUPL_PMGSY3.Where(m => m.PLAN_CN_ROAD_CODE == model.PLAN_CN_ROAD_CODE && m.IMS_YEAR == model.Year && m.IMS_BATCH == model.Batch).ToList();
                    var FinalList = listDetails.OrderBy(m => m.MAST_ER_ROAD_SCORE).ThenBy(m => m.CUPL_PMGSY3_ID);
                    int CUPL_ID = FinalList.Select(m => m.CUPL_PMGSY3_ID).FirstOrDefault();
                    if (dbContext.TR_MRL_EXEMPTION.Where(m => m.PLAN_CN_ROAD_CODE == model.PLAN_CN_ROAD_CODE && m.CUPL_PMGSY3_ID == CUPL_ID).Any())
                    {// Duplicate Check
                        return false;
                    }
                    //masterTable.TR_MRL_EXEMPTION_ID = dbContext.TR_MRL_EXEMPTION.Max(cp => (Int32?)cp.TR_MRL_EXEMPTION_ID) == null ? 1 : (Int32)dbContext.TR_MRL_EXEMPTION.Max(cp => (Int32?)cp.TR_MRL_EXEMPTION_ID) + 1;
                    masterTable.CUPL_PMGSY3_ID = CUPL_ID;
                    masterTable.REASON_FOR_NON_INCLUSION = model.ReasonCode;
                    masterTable.PLAN_CN_ROAD_CODE = model.PLAN_CN_ROAD_CODE;

                    masterTable.DATE_OF_NON_INCLUSION = System.DateTime.Now;
                    masterTable.USERID = PMGSYSession.Current.UserId;
                    masterTable.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                    masterTable.REQUEST_REMARKS_EXEMPTION = model.REQUEST_REMARKS_EXEMPTION.Trim();

                    masterTable.APPROVED_DATE = System.DateTime.Now;
                    masterTable.APPROVAL = "Y";
                    masterTable.APPROVAL_REMARKS = null;
                    dbContext.TR_MRL_EXEMPTION.Add(masterTable);
                    dbContext.SaveChanges();
                    //ts.Complete();
                    return true;
                }
            }
            catch (Exception ex)
            {

                ErrorLog.LogError(ex, "ExistingRoadDAL.MapRoadDetailsDAL()");
                return false;
            }
            finally
            {
                dbContext.Dispose();
            }
        }

        #endregion


        #region Existing Road Shift
        public Array ListExistingRoadsForShift(int stateCode, int districtCode, int blockCode, int SchemeCode, int ownerCode, int? page, int? rows, string sidx, string sord, out long totalRecords, string filters)
        {
            bool isPMGSY3 = false;
            try
            {
                JavaScriptSerializer js = null;
                PMGSY.Common.CommonFunctions.SearchJson test = new PMGSY.Common.CommonFunctions.SearchJson();
                string roadName = string.Empty;


                dbContext = new PMGSYEntities();

                var lstExistingRoadsDetails = dbContext.GET_EXISTING_ROADS_FOR_SHIFTING((blockCode <= 0 ? 0 : blockCode), SchemeCode).ToList();///Changes for RCPLWE

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
                        var finalList = lstExistingRoadsDetails.Where(o => o.MAST_ER_ROAD_NAME.Contains(roadName)).ToList();
                        lstExistingRoadsDetails = finalList;
                    }
                }


                totalRecords = lstExistingRoadsDetails.Count();

                if (sidx.Trim() != string.Empty)
                {
                    if (sord.ToString() == "asc")
                    {
                        switch (sidx)
                        {
                            case "MAST_ER_ROAD_CODE":
                                lstExistingRoadsDetails = lstExistingRoadsDetails.OrderBy(x => x.MAST_ER_ROAD_CODE).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                            case "MAST_ER_ROAD_NUMBER":
                                lstExistingRoadsDetails = lstExistingRoadsDetails.OrderBy(x => x.MAST_ER_ROAD_NUMBER).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                            case "MAST_ER_ROAD_NAME":
                                lstExistingRoadsDetails = lstExistingRoadsDetails.OrderBy(x => x.MAST_ER_ROAD_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                            //case "MAST_ER_ROAD_OWNER":
                            //    lstExistingRoadsDetails = lstExistingRoadsDetails.OrderBy(x => x.AGENCY_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                            //    break;
                            case "MAST_CORE_NETWORK":
                                lstExistingRoadsDetails = lstExistingRoadsDetails.OrderBy(x => x.MAST_CORE_NETWORK).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                            //case "MAST_ER_ROAD_CODE":
                            //    lstExistingRoadsDetails = lstExistingRoadsDetails.OrderBy(x => x.MAST_ER_ROAD_CODE).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                            //break;
                            case "MAST_ROAD_SHORT_DESC":
                                lstExistingRoadsDetails = lstExistingRoadsDetails.OrderBy(x => x.MAST_ROAD_SHORT_DESC).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                            case "MAST_ER_ROAD_TYPE":
                                lstExistingRoadsDetails = lstExistingRoadsDetails.OrderBy(x => x.MAST_ER_ROAD_TYPE).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                            default:
                                lstExistingRoadsDetails = lstExistingRoadsDetails.OrderBy(x => x.MAST_ER_ROAD_CODE).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                        }
                    }
                    else
                    {

                        switch (sidx)
                        {
                            case "MAST_ER_ROAD_CODE":
                                lstExistingRoadsDetails = lstExistingRoadsDetails.OrderByDescending(x => x.MAST_ER_ROAD_CODE).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                            case "MAST_ER_ROAD_NUMBER":
                                lstExistingRoadsDetails = lstExistingRoadsDetails.OrderByDescending(x => x.MAST_ER_ROAD_NUMBER).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                            case "MAST_ER_ROAD_NAME":
                                lstExistingRoadsDetails = lstExistingRoadsDetails.OrderByDescending(x => x.MAST_ER_ROAD_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                            //case "MAST_ER_ROAD_OWNER":
                            //    lstExistingRoadsDetails = lstExistingRoadsDetails.OrderByDescending(x => x.AGENCY_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                            //    break;
                            case "MAST_CORE_NETWORK":
                                lstExistingRoadsDetails = lstExistingRoadsDetails.OrderByDescending(x => x.MAST_CORE_NETWORK).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                            //case "MAST_ER_ROAD_CODE":
                            //    lstExistingRoadsDetails = lstExistingRoadsDetails.OrderByDescending(x => x.MAST_ER_ROAD_CODE).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                            //    break;
                            case "MAST_ROAD_SHORT_DESC":
                                lstExistingRoadsDetails = lstExistingRoadsDetails.OrderByDescending(x => x.MAST_ROAD_SHORT_DESC).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                            case "MAST_ER_ROAD_TYPE":
                                lstExistingRoadsDetails = lstExistingRoadsDetails.OrderByDescending(x => x.MAST_ER_ROAD_TYPE).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                            default:
                                lstExistingRoadsDetails = lstExistingRoadsDetails.OrderByDescending(x => x.MAST_ER_ROAD_CODE).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                        }
                    }
                }
                else
                {
                    lstExistingRoadsDetails = lstExistingRoadsDetails.OrderByDescending(x => x.MAST_ER_ROAD_CODE).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                }

                var result = lstExistingRoadsDetails.Select(roadDetails => new
                {
                    roadDetails.MAST_ER_ROAD_CODE,
                    roadDetails.MAST_ROAD_SHORT_DESC,
                    roadDetails.MAST_ER_ROAD_NUMBER,
                    roadDetails.MAST_PMGSY_SCHEME,
                    roadDetails.MAST_ER_ROAD_NAME,
                    roadDetails.MAST_ER_ROAD_TYPE,
                    roadDetails.MAST_ER_ROAD_LENGTH,
                    roadDetails.MAST_ER_ROAD_OWNER,
                    roadDetails.MAST_CORE_NETWORK

                }).ToArray();

                return result.Select(item => new
                {

                    id = item.MAST_ER_ROAD_CODE,
                    cell = new[]
                    {       
                        item.MAST_ER_ROAD_CODE.ToString(),
                        item.MAST_ROAD_SHORT_DESC.ToString().Trim(), //Road Category Short Desc
                        item.MAST_ER_ROAD_NUMBER.ToString().Trim(),
                        item.MAST_PMGSY_SCHEME.ToString().Trim(),
                        item.MAST_ER_ROAD_NAME.ToString().Trim(),
                        item.MAST_ER_ROAD_TYPE.ToString().Trim(), //Road Type
                        item.MAST_ER_ROAD_LENGTH.ToString().Trim(),// (dbContext.MASTER_EXISTING_ROADS.Where(m=>m.MAST_ER_ROAD_CODE == item.MAST_ER_ROAD_CODE).Select(m=>m.MAST_ER_ROAD_END_CHAIN).FirstOrDefault() - dbContext.MASTER_EXISTING_ROADS.Where(m=>m.MAST_ER_ROAD_CODE == item.MAST_ER_ROAD_CODE).Select(m=>m.MAST_ER_ROAD_STR_CHAIN).FirstOrDefault()).ToString(),
                        item.MAST_ER_ROAD_OWNER.ToString().Trim(),
                        item.MAST_CORE_NETWORK.ToString().Trim().ToUpper()=="Y"?"Yes":"No",
                      
                        URLEncrypt.EncryptParameters1(new string[] {  "ERCode =" + item.MAST_ER_ROAD_CODE.ToString() })
                      
                    }
                }).ToArray();
            }
            catch (Exception ex)
            {

                ErrorLog.LogError(ex, "ListExistingRoads().DAL().ListExistingRoadsForShift");
                totalRecords = 0;
                return null;
            }
            finally
            {
                dbContext.Dispose();
            }
        }


        public bool ShiftERDetailsDAL(string encryptedEReCode, string newBlockCode, string newDistrictCode, string ERCode)
        {
            Models.PMGSYEntities dbContext = new Models.PMGSYEntities();
            try
            {
                using (TransactionScope ts = new TransactionScope())
                {
                    int blockCode = 0;
                    int NewBlockCode = Convert.ToInt32(newBlockCode);
                    int NewDistrictCode = Convert.ToInt32(newDistrictCode);
                    int ERCODE = Convert.ToInt32(ERCode);

                    MASTER_EXISTING_ROADS oldmaster = new MASTER_EXISTING_ROADS();
                    MASTER_EXISTING_ROADS_LOG newMaster = new MASTER_EXISTING_ROADS_LOG();



                    if (dbContext.PLAN_ROAD.Where(m => m.MAST_ER_ROAD_CODE == ERCODE).Any())
                    {
                        PLAN_ROAD oldPlanRoad = new PLAN_ROAD();
                        PLAN_ROAD_LOG newPlanRoad = new PLAN_ROAD_LOG();

                        oldPlanRoad = dbContext.PLAN_ROAD.Where(m => m.MAST_ER_ROAD_CODE == ERCODE).FirstOrDefault();

                        newPlanRoad.PLAN_CN_ROAD_CODE = oldPlanRoad.PLAN_CN_ROAD_CODE;
                        newPlanRoad.MAST_PMGSY_SCHEME = oldPlanRoad.MAST_PMGSY_SCHEME;
                        newPlanRoad.MAST_ER_ROAD_CODE = oldPlanRoad.MAST_ER_ROAD_CODE;
                        newPlanRoad.PLAN_CN_ROAD_NUMBER = oldPlanRoad.PLAN_CN_ROAD_NUMBER;
                        newPlanRoad.MAST_STATE_CODE = oldPlanRoad.MAST_STATE_CODE;
                        newPlanRoad.MAST_DISTRICT_CODE = oldPlanRoad.MAST_DISTRICT_CODE;
                        newPlanRoad.MAST_BLOCK_CODE = oldPlanRoad.MAST_BLOCK_CODE;
                        newPlanRoad.PLAN_RD_NAME = oldPlanRoad.PLAN_RD_NAME;
                        newPlanRoad.PLAN_RD_ROUTE = oldPlanRoad.PLAN_RD_ROUTE;
                        newPlanRoad.PLAN_RD_FROM_CHAINAGE = oldPlanRoad.PLAN_RD_FROM_CHAINAGE;

                        newPlanRoad.PLAN_RD_TO_CHAINAGE = oldPlanRoad.PLAN_RD_TO_CHAINAGE;
                        newPlanRoad.PLAN_RD_LENG = oldPlanRoad.PLAN_RD_LENG;
                        newPlanRoad.PLAN_RD_LENGTH = oldPlanRoad.PLAN_RD_LENGTH;
                        newPlanRoad.PLAN_RD_TOTAL_LEN = oldPlanRoad.PLAN_RD_TOTAL_LEN;
                        newPlanRoad.PLAN_RD_FROM_TYPE = oldPlanRoad.PLAN_RD_FROM_TYPE;
                        newPlanRoad.PLAN_RD_TO_TYPE = oldPlanRoad.PLAN_RD_TO_TYPE;
                        newPlanRoad.PLAN_RD_FROM_HAB = oldPlanRoad.PLAN_RD_FROM_HAB;
                        newPlanRoad.PLAN_RD_TO_HAB = oldPlanRoad.PLAN_RD_TO_HAB;
                        newPlanRoad.PLAN_RD_BLOCK_FROM_CODE = oldPlanRoad.PLAN_RD_BLOCK_FROM_CODE;
                        newPlanRoad.PLAN_RD_BLOCK_TO_CODE = oldPlanRoad.PLAN_RD_BLOCK_TO_CODE;
                        newPlanRoad.PLAN_RD_NUM_FROM = oldPlanRoad.PLAN_RD_NUM_FROM;

                        newPlanRoad.PLAN_RD_NUM_TO = oldPlanRoad.PLAN_RD_NUM_TO;
                        newPlanRoad.PLAN_RD_FROM = oldPlanRoad.PLAN_RD_FROM;
                        newPlanRoad.PLAN_RD_TO = oldPlanRoad.PLAN_RD_TO;
                        newPlanRoad.PLAN_LOCK_STATUS = oldPlanRoad.PLAN_LOCK_STATUS;

                        newPlanRoad.NEW_MAST_DISTRICT_CODE = NewDistrictCode;
                        newPlanRoad.NEW_MAST_BLOCK_CODE = NewBlockCode;


                        newPlanRoad.USERID = PMGSYSession.Current.UserId;
                        newPlanRoad.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                        dbContext.PLAN_ROAD_LOG.Add(newPlanRoad);

                        oldPlanRoad.MAST_DISTRICT_CODE = NewDistrictCode;
                        oldPlanRoad.MAST_BLOCK_CODE = NewBlockCode;
                        dbContext.Entry(oldPlanRoad).State = System.Data.Entity.EntityState.Modified;
                        dbContext.SaveChanges();

                    }
                    if (dbContext.MASTER_EXISTING_ROADS_Temp.Where(m => m.MAST_ER_ROAD_CODE == ERCODE).Any())
                    {
                        return false;
                    }

                    List<MASTER_ER_HABITATION_ROAD> master = new List<MASTER_ER_HABITATION_ROAD>();

                    if (dbContext.MASTER_ER_HABITATION_ROAD.Where(m => m.MAST_ER_ROAD_CODE == ERCODE).Any())
                    {
                        master = dbContext.MASTER_ER_HABITATION_ROAD.Where(m => m.MAST_ER_ROAD_CODE == ERCODE).ToList();
                    }
                    //Code commented to enable displaying habitations in shifted block
                    //dbContext.MASTER_ER_HABITATION_ROAD.DeleteMany(master);
                    dbContext.SaveChanges();
                    // Multiple Delete from 


                    oldmaster = dbContext.MASTER_EXISTING_ROADS.Find(ERCODE);

                    newMaster.MAST_ER_ROAD_CODE = oldmaster.MAST_ER_ROAD_CODE;
                    newMaster.MAST_PMGSY_SCHEME = oldmaster.MAST_PMGSY_SCHEME;
                    newMaster.MAST_STATE_CODE = oldmaster.MAST_STATE_CODE;
                    newMaster.MAST_DISTRICT_CODE = oldmaster.MAST_DISTRICT_CODE;
                    newMaster.MAST_BLOCK_CODE = oldmaster.MAST_BLOCK_CODE;
                    newMaster.MAST_ER_ROAD_NUMBER = oldmaster.MAST_ER_ROAD_NUMBER;
                    newMaster.MAST_ROAD_CAT_CODE = oldmaster.MAST_ROAD_CAT_CODE;
                    newMaster.MAST_ER_ROAD_NAME = oldmaster.MAST_ER_ROAD_NAME;
                    newMaster.MAST_ER_ROAD_STR_CHAIN = oldmaster.MAST_ER_ROAD_STR_CHAIN;
                    newMaster.MAST_ER_ROAD_END_CHAIN = oldmaster.MAST_ER_ROAD_END_CHAIN;
                    newMaster.MAST_ER_ROAD_C_WIDTH = oldmaster.MAST_ER_ROAD_C_WIDTH;
                    newMaster.MAST_ER_ROAD_F_WIDTH = oldmaster.MAST_ER_ROAD_F_WIDTH;
                    newMaster.MAST_ER_ROAD_L_WIDTH = oldmaster.MAST_ER_ROAD_L_WIDTH;
                    newMaster.MAST_ER_ROAD_TYPE = oldmaster.MAST_ER_ROAD_TYPE;
                    newMaster.MAST_SOIL_TYPE_CODE = oldmaster.MAST_SOIL_TYPE_CODE;
                    newMaster.MAST_TERRAIN_TYPE_CODE = oldmaster.MAST_TERRAIN_TYPE_CODE;
                    newMaster.MAST_CORE_NETWORK = oldmaster.MAST_CORE_NETWORK;
                    newMaster.MAST_IS_BENEFITTED_HAB = oldmaster.MAST_IS_BENEFITTED_HAB;
                    newMaster.MAST_NOHABS_REASON = oldmaster.MAST_NOHABS_REASON;
                    newMaster.MAST_ER_ROAD_OWNER = oldmaster.MAST_ER_ROAD_OWNER;
                    newMaster.MAST_CONS_YEAR = oldmaster.MAST_CONS_YEAR;
                    newMaster.MAST_RENEW_YEAR = oldmaster.MAST_RENEW_YEAR;
                    newMaster.MAST_CD_WORKS_NUM = oldmaster.MAST_CD_WORKS_NUM;
                    newMaster.MAST_LOCK_STATUS = oldmaster.MAST_LOCK_STATUS;
                    newMaster.MAST_ER_ROAD_CODE_PMGSY1 = oldmaster.MAST_ER_ROAD_CODE_PMGSY1;
                    newMaster.MAST_DISTRICT_CODE_NEW = NewDistrictCode;
                    newMaster.MAST_BLOCK_CODE_NEW = NewBlockCode;
                    newMaster.USERID = PMGSYSession.Current.UserId;
                    newMaster.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                    //Commented to stop shifting only once
                    //dbContext.MASTER_EXISTING_ROADS_LOG.Add(newMaster);

                    //Code Added to Allow Shifting multiple times
                    if (dbContext.MASTER_EXISTING_ROADS_LOG.Find(ERCODE) != null)
                    {
                        MASTER_EXISTING_ROADS_LOG existing = new MASTER_EXISTING_ROADS_LOG();

                        existing = dbContext.MASTER_EXISTING_ROADS_LOG.Find(ERCODE);

                        ((IObjectContextAdapter)dbContext).ObjectContext.Detach(existing);

                        dbContext.Entry(newMaster).State = System.Data.Entity.EntityState.Modified;
                    }
                    else
                    {
                        dbContext.MASTER_EXISTING_ROADS_LOG.Add(newMaster);
                    }
                    oldmaster.MAST_DISTRICT_CODE = NewDistrictCode;
                    oldmaster.MAST_BLOCK_CODE = NewBlockCode;

                    dbContext.Entry(oldmaster).State = System.Data.Entity.EntityState.Modified;
                    dbContext.SaveChanges();

                    ts.Complete();
                    return true;

                }

            }
            catch (OptimisticConcurrencyException ex)
            {
                ErrorLog.LogError(ex, "ListExistingRoads().DAL().ShiftERDetailsDAL [OptimisticConcurrencyException]");
                return false;
            }
            catch (UpdateException ex)
            {
                ErrorLog.LogError(ex, "ListExistingRoads().DAL().ShiftERDetailsDAL [UpdateException]");
                return false;
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "ListExistingRoads().DAL().ShiftERDetailsDAL [Exception]");
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


        #region Delete / Unlock Exumpted Roads
        public Array GetNotFeasibleRoadsListDALforDeletion(int? page, int? rows, string sidx, string sord, out long totalRecords, int stateCode, int districtCode, int IMS_YEAR, int MAST_BLOCK_ID, int IMS_BATCH, int IMS_STREAMS, string IMS_PROPOSAL_TYPE, int adminCode, string IMS_PROPOSAL_STATUS, string IMS_UPGRADE_CONNECT)
        {
            dbContext = new Models.PMGSYEntities();
            try
            {
                int state_Code = stateCode == -1 ? 0 : stateCode;
                int district_Code = districtCode == -1 ? 0 : districtCode;
                int YEAR_Code = IMS_YEAR == -1 ? 0 : IMS_YEAR;
                int BLOCK_Code = MAST_BLOCK_ID == -1 ? 0 : MAST_BLOCK_ID;
                int BATCH_Code = IMS_BATCH == -1 ? 0 : IMS_BATCH;
                int STREAMS_Code = IMS_STREAMS == -1 ? 0 : IMS_STREAMS;

                dbContext = new Models.PMGSYEntities();
                //var itemList = dbContext.USP_GET_CUPL_PMGSY3_EXEMPTION_LIST_FOR_DELETION(district_Code, BLOCK_Code, YEAR_Code, BATCH_Code).OrderBy(m => m.CUPL_RANK).ToList();
                var itemList = dbContext.USP_GET_CUPL_PMGSY3_EXEMPTION_LIST(district_Code, BLOCK_Code, YEAR_Code, BATCH_Code).OrderBy(m => m.CUPL_RANK).ToList();
                //IQueryable<USP_GET_CUPL_PMGSY3_EXEMPTION_LIST_FOR_DELETION_Result> query = itemList.AsQueryable<USP_GET_CUPL_PMGSY3_EXEMPTION_LIST_FOR_DELETION_Result>();
                IQueryable<USP_GET_CUPL_PMGSY3_EXEMPTION_LIST_Result> query = itemList.AsQueryable<USP_GET_CUPL_PMGSY3_EXEMPTION_LIST_Result>();
                totalRecords = itemList.Count;
                if (sidx.Trim() != string.Empty)
                {
                    if (sord.ToString() == "asc")
                    {
                        itemList = itemList.OrderBy(x => x.CUPL_RANK).Skip(Convert.ToInt32((page - 1) * rows)).ToList();
                    }
                    else
                    {
                        itemList = itemList.OrderByDescending(x => x.CUPL_RANK).Skip(Convert.ToInt32((page - 1) * rows)).ToList();
                    }
                }
                else
                {
                    itemList = itemList.OrderBy(x => x.CUPL_RANK).Skip(Convert.ToInt32((page - 1) * rows)).ToList();
                }

                return itemList.Select(propDetails => new
                {
                    id = propDetails.CUPL_PMGSY3_ID.ToString(), // here PLAN_CN_ROAD_CODE is used.  see SP USP_GET_CUPL_PMGSY3_LIST for more clarification
                    cell = new[] {     
                                    propDetails.CUPL_RANK.ToString(),
                                    propDetails.MAST_DISTRICT_NAME.Trim(),
                                    propDetails.MAST_BLOCK_NAME.Trim(),
                                    propDetails.PLAN_CN_ROAD_NUMBER.Trim(),
                                    propDetails.PLAN_RD_LENGTH.ToString(),     
                                    propDetails.PLAN_RD_LENGTH<5?"Not Eligible":"Eligible",
                                    propDetails.ELIGIBLE_CANDIDATE_LENGTH.ToString(),
                                    propDetails.PLAN_RD_NAME.ToString(),
                                    (propDetails.REQUEST_REMARKS!=null)?("Request Remarks : "+propDetails.REQUEST_REMARKS.ToString() +", Reason : "+propDetails.REASON.ToString() ):"-",
                                    (propDetails.REQUEST_REMARKS!=null)?"Approved":((propDetails.APPROVAL.Equals("Y"))?"<a href='#' title='Click here to Add details' class='ui-icon ui-icon-plusthick ui-align-center' onClick='Map(\"" + URLEncrypt.EncryptParameters(new string[] {propDetails.CUPL_PMGSY3_ID.ToString()  })  +"\"); return false;'>Map</a>":"-"),
                                     "<a href='#' title='Click here to delete details' class='ui-icon ui-icon-trash ui-align-center' onClick =DeleteExumptedDetails('"+URLEncrypt.EncryptParameters1(new string[]{"TRMRLCode =" + propDetails.TR_MRL_EXEMPTION_ID.ToString().Trim()})+"'); return false;'></a>"

                    }
                }).ToArray();
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "ExistingRoadDAL.GetNotFeasibleRoadsListDALforDeletion()");
                totalRecords = 0;
                return null;
            }
            finally
            {
                dbContext.Dispose();
            }
        }

        // DeleteTRMRLDetailsDAL
        public bool DeleteTRMRLDetailsDAL(int CNCode, out string message)
        {
            using (TransactionScope ts = new TransactionScope())
            {
                try
                {
                    dbContext = new PMGSYEntities();
                    TR_MRL_EXEMPTION master = dbContext.TR_MRL_EXEMPTION.Where(m => m.PLAN_CN_ROAD_CODE == CNCode).FirstOrDefault();

                    if (master != null)
                    {
                        List<TR_MRL_EXEMPTION> master1 = new List<TR_MRL_EXEMPTION>();

                        if (dbContext.TR_MRL_EXEMPTION.Where(m => m.PLAN_CN_ROAD_CODE == CNCode).Any())
                        {
                            master1 = dbContext.TR_MRL_EXEMPTION.Where(m => m.PLAN_CN_ROAD_CODE == CNCode).ToList();
                        }
                        foreach (var item in master1)
                        {

                            TR_MRL_EXEMPTION_LOG logmaster = new TR_MRL_EXEMPTION_LOG();
                            int Primary_Key = 0;

                            if (dbContext.TR_MRL_EXEMPTION_LOG.Any())
                            {
                                Primary_Key = dbContext.TR_MRL_EXEMPTION_LOG.Max(s => (int)s.SR_NO) + 1;
                            }
                            else
                            {
                                Primary_Key = 1;
                            }
                            logmaster.SR_NO = Primary_Key;
                            logmaster.TR_MRL_EXEMPTION_ID = master.TR_MRL_EXEMPTION_ID;
                            logmaster.CUPL_PMGSY3_ID = master.CUPL_PMGSY3_ID;
                            logmaster.PLAN_CN_ROAD_CODE = master.PLAN_CN_ROAD_CODE;
                            logmaster.REASON_FOR_NON_INCLUSION = master.REASON_FOR_NON_INCLUSION;
                            logmaster.DATE_OF_NON_INCLUSION = master.DATE_OF_NON_INCLUSION;
                            logmaster.REQUEST_REMARKS_EXEMPTION = master.REQUEST_REMARKS_EXEMPTION;
                            logmaster.APPROVED_DATE = master.APPROVED_DATE;
                            logmaster.APPROVAL = master.APPROVAL;
                            logmaster.APPROVAL_REMARKS = master.APPROVAL_REMARKS;
                            logmaster.USERID = PMGSYSession.Current.UserId;
                            logmaster.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                            dbContext.TR_MRL_EXEMPTION_LOG.Add(logmaster);
                            dbContext.SaveChanges();

                        }

                        // Delete From Original Table.
                        dbContext.TR_MRL_EXEMPTION.DeleteMany(master1);
                        dbContext.SaveChanges();
                        // Multiple Delete from 

                        message = String.Empty;
                        ts.Complete();
                        return true;

                        //dbContext.TR_MRL_EXEMPTION.Remove(master);
                        //dbContext.SaveChanges();
                        //message = String.Empty;
                        //ts.Complete();
                        //return true;
                    }
                    else
                    {
                        message = String.Empty;
                        return false;
                    }

                }
                catch (DbEntityValidationException ex)
                {
                    ts.Dispose();
                    ErrorLog.LogError(ex, "ExistingRoadDAL.DeleteTRMRLDetailsDAL()");
                    message = String.Empty;
                    return false;
                }
                catch (Exception ex)
                {
                    ts.Dispose();
                    ErrorLog.LogError(ex, "ExistingRoadDAL.DeleteTRMRLDetailsDAL()");
                    message = String.Empty;
                    return false;
                }
                finally
                {
                    dbContext.Dispose();
                }
            }
        }

        public bool DeleteTRMRLExemptedDetailsDAL(int TRMRLCode, out string message)
        {
            using (TransactionScope ts = new TransactionScope())
            {
                try
                {
                    dbContext = new PMGSYEntities();
                    TR_MRL_EXEMPTION master = dbContext.TR_MRL_EXEMPTION.Where(m => m.TR_MRL_EXEMPTION_ID == TRMRLCode).FirstOrDefault();

                    if (master != null)
                    {

                        // Delete From Original Table.
                        //dbContext.TR_MRL_EXEMPTION.DeleteMany(master);
                        //dbContext.SaveChanges();

                        //message = String.Empty;
                        //ts.Complete();
                        //return true;

                        dbContext.TR_MRL_EXEMPTION.Remove(master);
                        dbContext.SaveChanges();
                        message = String.Empty;
                        ts.Complete();
                        return true;
                    }
                    else
                    {
                        message = String.Empty;
                        return false;
                    }

                }
                catch (DbEntityValidationException ex)
                {
                    ts.Dispose();
                    ErrorLog.LogError(ex, "ExistingRoadDAL.DeleteTRMRLExemptedDetailsDAL()");
                    message = String.Empty;
                    return false;
                }
                catch (Exception ex)
                {
                    ts.Dispose();
                    ErrorLog.LogError(ex, "ExistingRoadDAL.DeleteTRMRLExemptedDetailsDAL()");
                    message = String.Empty;
                    return false;
                }
                finally
                {
                    dbContext.Dispose();
                }
            }
        }
        #endregion

        #region Delete CUPL Roads
        public Array UnlockCUPLListDAL(int? page, int? rows, string sidx, string sord, out long totalRecords, int stateCode, int districtCode, int IMS_YEAR, int MAST_BLOCK_ID, int IMS_BATCH, int IMS_STREAMS, string IMS_PROPOSAL_TYPE, int adminCode, string IMS_PROPOSAL_STATUS, string IMS_UPGRADE_CONNECT)
        {
            dbContext = new Models.PMGSYEntities();
            try
            {
                int state_Code = stateCode == -1 ? 0 : stateCode;
                int district_Code = districtCode == -1 ? 0 : districtCode;
                int YEAR_Code = IMS_YEAR == -1 ? 0 : IMS_YEAR;
                int BLOCK_Code = MAST_BLOCK_ID == -1 ? 0 : MAST_BLOCK_ID;
                int BATCH_Code = IMS_BATCH == -1 ? 0 : IMS_BATCH;
                int STREAMS_Code = IMS_STREAMS == -1 ? 0 : IMS_STREAMS;

                dbContext = new Models.PMGSYEntities();
                var itemList = dbContext.USP_GET_CUPL_PMGSY3_DETAILS_FOR_DELETION(district_Code, BLOCK_Code, YEAR_Code, BATCH_Code).OrderBy(m => m.CUPL_RANK).ToList();
                IQueryable<USP_GET_CUPL_PMGSY3_DETAILS_FOR_DELETION_Result> query = itemList.AsQueryable<USP_GET_CUPL_PMGSY3_DETAILS_FOR_DELETION_Result>();
                totalRecords = itemList.Count;
                if (sidx.Trim() != string.Empty)
                {
                    if (sord.ToString() == "asc")
                    {
                        itemList = itemList.OrderBy(x => x.CUPL_RANK).Skip(Convert.ToInt32((page - 1) * rows)).ToList();
                    }
                    else
                    {
                        itemList = itemList.OrderByDescending(x => x.CUPL_RANK).Skip(Convert.ToInt32((page - 1) * rows)).ToList();
                    }
                }
                else
                {
                    itemList = itemList.OrderBy(x => x.CUPL_RANK).Skip(Convert.ToInt32((page - 1) * rows)).ToList();
                }

                return itemList.Select(propDetails => new
                {
                    id = propDetails.CUPL_PMGSY3_ID.ToString(), // here PLAN_CN_ROAD_CODE is used.  see SP USP_GET_CUPL_PMGSY3_LIST for more clarification
                    cell = new[] {     
                                    propDetails.CUPL_RANK.ToString(),
                                    propDetails.MAST_DISTRICT_NAME.Trim(),
                                    propDetails.MAST_BLOCK_NAME.Trim(),
                                    propDetails.PLAN_CN_ROAD_NUMBER.Trim(),
                                    propDetails.PLAN_RD_LENGTH.ToString(),     
                                    propDetails.PLAN_RD_LENGTH<5?"Not Eligible":"Eligible",
                                    propDetails.ELIGIBLE_CANDIDATE_LENGTH.ToString(),
                                    propDetails.PLAN_RD_NAME.ToString(),
                                  //  (propDetails.REQUEST_REMARKS!=null)?("Request Remarks : "+propDetails.REQUEST_REMARKS.ToString() +", Reason : "+propDetails.REASON.ToString() ):"-",
                                    //(propDetails.REQUEST_REMARKS!=null)?"Approved":((propDetails.APPROVAL.Equals("Y"))?"<a href='#' title='Click here to Add details' class='ui-icon ui-icon-plusthick ui-align-center' onClick='Map(\"" + URLEncrypt.EncryptParameters(new string[] {propDetails.CUPL_PMGSY3_ID.ToString()  })  +"\"); return false;'>Map</a>":"-"),
                                     "<a href='#' title='Click here to delete details' class='ui-icon ui-icon-trash ui-align-center' onClick =DeleteExumptedDetails('"+URLEncrypt.EncryptParameters1(new string[]{"CNCode =" + propDetails.CUPL_PMGSY3_ID.ToString().Trim()})+"'); return false;'></a>"

                    }
                }).ToArray();
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "ExistingRoadDAL.UnlockCUPLListDAL()");
                totalRecords = 0;
                return null;
            }
            finally
            {
                dbContext.Dispose();
            }
        }

        // DeleteTRMRLDetailsDAL
        public bool DeleteCUPLDetailsDAL(int BlockCode, int YearCode, int BatchCode, out string message)
        {
            //using (TransactionScope ts = new TransactionScope())
            //{
            try
            {
                dbContext = new PMGSYEntities();
                //  CUPL_PMGSY3 master = dbContext.CUPL_PMGSY3.Where(m => m.MAST_BLOCK_CODE == BlockCode && m.IMS_YEAR == YearCode && m.IMS_BATCH == BatchCode).FirstOrDefault();


                // Checking if the Road is used in Proposal or Not. If it is used in Proposal then can not be deleted. 
                List<CUPL_PMGSY3> masterDetails = new List<CUPL_PMGSY3>();
                masterDetails = dbContext.CUPL_PMGSY3.Where(m => m.MAST_BLOCK_CODE == BlockCode && m.IMS_YEAR == YearCode && m.IMS_BATCH == BatchCode).ToList();

                foreach (var item in masterDetails)
                {
                    if (dbContext.IMS_SANCTIONED_PROJECTS.Where(m => m.PLAN_CN_ROAD_CODE == item.PLAN_CN_ROAD_CODE && m.MAST_BLOCK_CODE == item.MAST_BLOCK_CODE && m.IMS_YEAR == item.IMS_YEAR && m.IMS_BATCH == item.IMS_BATCH).Any())
                    {
                        message = item.PLAN_RD_NAME + ", This CUPL Road is already used in Proposal. These details can not be unlocked.";
                        return false;
                    }
                }

                // Checking if the Road is Exempted or Not. If it is exempted then it can not be deleted until we remove record from exempted table.
                foreach (var item in masterDetails)
                {
                    if (dbContext.TR_MRL_EXEMPTION.Where(m => m.CUPL_PMGSY3_ID == item.CUPL_PMGSY3_ID).Any())
                    {
                        message = item.PLAN_RD_NAME + ", This CUPL Road is exempted. Please contact MORD for its deletion so as to unlock CUPL details for selected Block.";
                        return false;
                    }
                }

                // Check on deleting records only for latest batch.
                foreach (var item in masterDetails)
                {
                    int batch = dbContext.CUPL_PMGSY3.Where(m => m.MAST_BLOCK_CODE == item.MAST_BLOCK_CODE && m.IMS_YEAR == YearCode).Max(m => m.IMS_BATCH);
                    if (batch > item.IMS_BATCH)
                    {
                        message = "Batch " + batch + " records are already entred for block " + item.MAST_BLOCK_NAME + ". Hence details for Batch " + item.IMS_BATCH + " in block " + item.MAST_BLOCK_NAME + " can not be deleted.";
                        return false;
                    }

                }


                if (masterDetails != null)
                {

                    #region Log Entry
                    //foreach (var item in master1)
                    //{
                    //    CUPL_PMGSY3_LOG_ON_DELETE logdetails = new CUPL_PMGSY3_LOG_ON_DELETE();
                    //    int Primary_Key = 0;
                    //    if (dbContext.CUPL_PMGSY3_LOG_ON_DELETE.Any())
                    //    {
                    //        Primary_Key = dbContext.CUPL_PMGSY3_LOG_ON_DELETE.Max(s => (int)s.SR_NO) + 1;
                    //    }
                    //    else
                    //    {
                    //        Primary_Key = 1;
                    //    }
                    //    logdetails.SR_NO = Primary_Key;
                    //    logdetails.CUPL_PMGSY3_ID = item.CUPL_PMGSY3_ID;
                    //    logdetails.CUPL_RANK = item.CUPL_RANK;
                    //    logdetails.MAST_DISTRICT_CODE = item.MAST_DISTRICT_CODE;
                    //    logdetails.MAST_DISTRICT_NAME = item.MAST_DISTRICT_NAME;
                    //    logdetails.MAST_BLOCK_CODE = item.MAST_BLOCK_CODE;
                    //    logdetails.MAST_BLOCK_NAME = item.MAST_BLOCK_NAME;
                    //    logdetails.PLAN_CN_ROAD_CODE = item.PLAN_CN_ROAD_CODE;
                    //    logdetails.PLAN_CN_ROAD_NUMBER = item.PLAN_CN_ROAD_NUMBER;
                    //    logdetails.PLAN_RD_LENGTH = item.PLAN_RD_LENGTH;
                    //    logdetails.ELIGIBLE_CANDIDATE_LENGTH = Convert.ToDecimal(item.ELIGIBLE_CANDIDATE_LENGTH);
                    //    logdetails.PLAN_RD_NAME = item.PLAN_RD_NAME;
                    //    logdetails.SCORE = item.SCORE;
                    //    logdetails.SCORE_PER_UNIT_LENGTH = item.SCORE_PER_UNIT_LENGTH;
                    //    logdetails.MIN_SCORE = item.MIN_SCORE;
                    //    logdetails.MAST_ER_ROAD_CODE = item.MAST_ER_ROAD_CODE;
                    //    logdetails.MAST_ER_ROAD_NAME = item.MAST_ER_ROAD_NAME;
                    //    logdetails.MAST_ER_ROAD_NUMBER = item.MAST_ER_ROAD_NUMBER;
                    //    logdetails.MAST_ER_ROAD_OWNER = item.MAST_ER_ROAD_OWNER;
                    //    logdetails.PLAN_RD_FROM_CHAINAGE = item.PLAN_RD_FROM_CHAINAGE;
                    //    logdetails.PLAN_RD_TO_CHAINAGE = item.PLAN_RD_TO_CHAINAGE;
                    //    logdetails.MAST_ER_ROAD_SCORE = item.MAST_ER_ROAD_SCORE;
                    //    logdetails.MEANPCI = item.MEANPCI;
                    //    logdetails.PCI_ELIGIBILITY = item.PCI_ELIGIBILITY;
                    //    logdetails.ELIGIBILE_LENGTH = item.ELIGIBILE_LENGTH;
                    //    logdetails.COMPLETED = item.COMPLETED;
                    //    logdetails.ELIGIBLE = item.ELIGIBLE;
                    //    logdetails.DATE_DIFFETENCR = item.DATE_DIFFETENCR;
                    //    logdetails.TOTAL_HAB_SERVER = item.TOTAL_HAB_SERVER;
                    //    logdetails.IMS_YEAR = item.IMS_YEAR;
                    //    logdetails.IMS_BATCH = item.IMS_BATCH;
                    //    logdetails.FINALIZED_DATE = item.FINALIZED_DATE;
                    //    logdetails.APPROVAL = item.APPROVAL;
                    //    logdetails.DELETION_DATE = DateTime.Now;
                    //    logdetails.USERID = PMGSYSession.Current.UserId;
                    //    logdetails.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                    //    dbContext.CUPL_PMGSY3_LOG_ON_DELETE.Add(logdetails);
                    //    dbContext.SaveChanges();
                    //}
                    #endregion Log Entry

                    // Delete From Original Table.
                    dbContext.CUPL_PMGSY3.DeleteMany(masterDetails);
                    dbContext.SaveChanges();
                    // Multiple Delete from 
                    message = String.Empty;
                    // ts.Complete();
                    return true;
                }
                else
                {
                    message = String.Empty;
                    return false;
                }

            }
            catch (UpdateException ex)
            {
                //  ts.Dispose();
                ErrorLog.LogError(ex, "ExistingRoadDAL.DeleteCUPLDetailsDAL() [UpdateException]");
                message = "[UpdateException] : This Record can not be deleted as it is being used in Proposal or Exempted Records.";
                return false;
            }
            catch (System.Data.Entity.Infrastructure.DbUpdateException ex)
            {
                // ts.Dispose();
                ErrorLog.LogError(ex, "ExistingRoadDAL.DeleteCUPLDetailsDAL() [DbUpdateException]");
                message = "[DbUpdateException] : This Record can not be deleted as it is being used in Proposal or Exempted Records.";
                return false;
            }
            catch (DbEntityValidationException ex)
            {
                // ts.Dispose();
                ErrorLog.LogError(ex, "ExistingRoadDAL.DeleteCUPLDetailsDAL() [DbEntityValidationException]");
                message = "[DbEntityValidationException] : This Record can not be deleted as it is being used in Proposal or Exempted Records.";
                return false;
            }
            catch (Exception ex)
            {
                //ts.Dispose();
                ErrorLog.LogError(ex, "ExistingRoadDAL.DeleteCUPLDetailsDAL() [Exception]");
                message = "[Exception] : This Record can not be deleted as it is being used in Proposal or Exempted Records.";
                return false;
            }
            finally
            {
                dbContext.Dispose();
            }
            // }
        }
        #endregion

        #region Definalize PCI Details
        public Array GetListForDefinalizePCIatMORDunderPMGSYIII(int districtCode, int? page, int? rows, string sidx, string sord, out long totalRecords, string filters, ref bool isAllBlockFinalized, int statecode)
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

                var lstDRRPFinalizedBlocks = dbContext.MAST_PCI_BLOCK_PMGSY3_FINALIZE.Where(z => z.MASTER_BLOCK.MAST_DISTRICT_CODE == districtCode && z.MASTER_BLOCK.MAST_BLOCK_ACTIVE == "Y" && z.IS_FINALIZED == "Y").Select(x => new { MAST_BLOCK_CODE = x.MAST_BLOCK_CODE, MAST_BLOCK_NAME = x.MASTER_BLOCK.MAST_BLOCK_NAME }).Distinct().ToList();

                ///Get Block Names
                ///

                //var lstBlock = dbContext.MASTER_BLOCK.Where(c => c.MAST_BLOCK_ACTIVE == "Y"
                //    && c.MAST_DISTRICT_CODE == (districtCode == 0 ? c.MAST_DISTRICT_CODE : districtCode)
                //    && c.MASTER_DISTRICT.MAST_STATE_CODE == statecode).Select(x => new { MAST_BLOCK_CODE = x.MAST_BLOCK_CODE, MAST_BLOCK_NAME = x.MAST_BLOCK_NAME.Trim(), MAST_DISTRICT_CODE = x.MAST_DISTRICT_CODE, MAST_DISTRICT_NAME = x.MASTER_DISTRICT.MAST_DISTRICT_NAME }).OrderBy(x => x.MAST_DISTRICT_NAME).ToList();



                var lstBlock = dbContext.MASTER_BLOCK.Where(c => c.MAST_BLOCK_ACTIVE == "Y" && c.MAST_DISTRICT_CODE == districtCode).Select(x => new { MAST_BLOCK_CODE = x.MAST_BLOCK_CODE, MAST_BLOCK_NAME = x.MAST_BLOCK_NAME.Trim(), MAST_DISTRICT_CODE = x.MAST_DISTRICT_CODE, MAST_DISTRICT_NAME = x.MASTER_DISTRICT.MAST_DISTRICT_NAME }).OrderBy(x => x.MAST_DISTRICT_NAME).ToList();

                if (lstBlock.Count() == lstDRRPFinalizedBlocks.Count() && (dbContext.MAST_PCI_DISTRICT_PMGSY3_FINALIZE.Any(z => z.MAST_DISTRICT_CODE == districtCode && z.IS_FINALIZED == "Y") || dbContext.MAST_PCI_DISTRICT_PMGSY3_FINALIZE.Any(z => z.MAST_DISTRICT_CODE == districtCode)))
                {
                    isAllBlockFinalized = true;
                }

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



                return lstBlock.Select(item => new
                {
                    //id = item.MAST_ER_ROAD_CODE,
                    cell = new[]
                    {    
                        item.MAST_DISTRICT_NAME.ToString(),
                        item.MAST_BLOCK_NAME.ToString(),
                        
                        (dbContext.MAST_PCI_BLOCK_PMGSY3_FINALIZE.Any(z=>z.MAST_BLOCK_CODE == item.MAST_BLOCK_CODE && z.IS_FINALIZED == "Y"))
                        ? "<span class='ui-icon ui-icon-locked ui-align-center'></span>"
                        : (dbContext.PLAN_ROAD.Where(m=>m.MAST_BLOCK_CODE == item.MAST_BLOCK_CODE && m.PLAN_LOCK_STATUS == "N" && m.MAST_PMGSY_SCHEME == 4).Any()) 
                                ? "All DRRP roads not finalized for the block"
                                : (dbContext.MAST_PCI_BLOCK_PMGSY3_FINALIZE.Where(z=>z.MAST_BLOCK_CODE == item.MAST_BLOCK_CODE && z.IS_FINALIZED == "N").Any() || !(dbContext.MAST_PCI_BLOCK_PMGSY3_FINALIZE.Where(z=>z.MAST_BLOCK_CODE == item.MAST_BLOCK_CODE).Any()))
                                    ? "<a href='#' title='Click here to delete details' class='ui-icon ui-icon-unlocked ui-align-center' onClick =FinalizeMRLBlock('"
                                            +URLEncrypt.EncryptParameters1(new string[]{"BlockCode =" + item.MAST_BLOCK_CODE.ToString().Trim()}) +"'); return false;'></a>"
                                    : "",

                        //DeFinalize
                        (dbContext.MAST_PCI_BLOCK_PMGSY3_FINALIZE.Any(z=>z.MAST_BLOCK_CODE == item.MAST_BLOCK_CODE && z.IS_FINALIZED == "Y") && !dbContext.MAST_PCI_DISTRICT_PMGSY3_FINALIZE.Any(z=>z.MAST_DISTRICT_CODE == districtCode && z.IS_FINALIZED =="Y"))   
                        ? "<a href='#' title='Click here to delete details' class='ui-icon ui-icon-locked ui-align-center' onClick =DeFinalizeDRRPBlock('"+URLEncrypt.EncryptParameters1(new string[]{"BlockCode =" + item.MAST_BLOCK_CODE.ToString().Trim()}) +"'); return false;'></a>"
                        : (dbContext.MAST_PCI_BLOCK_PMGSY3_FINALIZE.Any(z=>z.MAST_BLOCK_CODE == item.MAST_BLOCK_CODE && z.IS_FINALIZED == "Y") && dbContext.MAST_PCI_DISTRICT_PMGSY3_FINALIZE.Any(z=>z.MAST_DISTRICT_CODE == districtCode && z.IS_FINALIZED== "Y"))
                            ? "District is finalized"
                            : "<span style='color:red;'>Block not yet finalized</span>"
                    }
                }).ToArray();
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "ExistingRoadDAL().GetListForDefinalizePCIatMORDunderPMGSYIII()");
                totalRecords = 0;
                return null;
            }
            finally
            {
                dbContext.Dispose();
            }
        }

        public bool DeFinalizePCIPMGSY3DAL(int blockCode, ref string message)
        {
            PMGSYEntities dbContext = new PMGSYEntities();
            int DistrictCode = dbContext.MASTER_BLOCK.Where(z => z.MAST_BLOCK_CODE == blockCode).Select(z => z.MAST_DISTRICT_CODE).FirstOrDefault();
            try
            {

                if (dbContext.MAST_PCI_DISTRICT_PMGSY3_FINALIZE.Where(z => z.MAST_DISTRICT_CODE == DistrictCode).Any())
                {
                    message = " This District is finalized for PCI. Please definalize district first.";
                    return false;
                }
                else
                {

                    dbContext = new PMGSYEntities();
                    MAST_PCI_BLOCK_PMGSY3_FINALIZE pciBlock = dbContext.MAST_PCI_BLOCK_PMGSY3_FINALIZE.Where(z => z.MAST_BLOCK_CODE == blockCode).FirstOrDefault();
                    dbContext.MAST_PCI_BLOCK_PMGSY3_FINALIZE.Remove(pciBlock);
                    dbContext.SaveChanges();
                    message = "PCI Block definalized successfully";
                    return true;
                }
            }
            catch (DbEntityValidationException e)
            {
                ErrorLog.LogError(e, "ExistingRoadDAL().DeFinalizePCIPMGSY3DAL [DbEntityValidationException]");

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
                    sw.WriteLine("ExistingRoadDAL().DeFinalizePCIPMGSY3DAL [DbEntityValidationException]");
                    sw.WriteLine("Exception : " + new CommonFunctions().FormatErrorMessage(modelstate));
                    sw.WriteLine("---------------------------------------------------------------------------------------");
                    sw.Close();
                }
                return false;
            }
            catch (OptimisticConcurrencyException ex)
            {
                ErrorLog.LogError(ex, "ExistingRoadDAL().DeFinalizePCIPMGSY3DAL [OptimisticConcurrencyException]");

                message = "An Error Occurred While Processing Your Request.";
                return false;
            }
            catch (UpdateException ex)
            {
                ErrorLog.LogError(ex, "ExistingRoadDAL().DeFinalizePCIPMGSY3DAL [UpdateException]");
                message = "An Error Occurred While Processing Your Request.";
                return false;
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "ExistingRoadDAL().DeFinalizePCIPMGSY3DAL [Exception]");
                message = "An Error Occurred While Processing Your Request.";
                return false;
            }
            finally
            {
                dbContext.Dispose();
            }
        }

        public bool DeFinalizePCIDistrictPMGSY3DAL(int districtCode, ref string message)
        {
            PMGSYEntities dbContext = new PMGSYEntities();
            try
            {
                dbContext = new PMGSYEntities();
                if (dbContext.CUPL_PMGSY3.Where(z => z.MAST_DISTRICT_CODE == districtCode).Any())
                {
                    message = " CUPL is generated against the Block of This District. Hence This District can not be definalized here.";
                    return false;
                }
                else
                {
                    MAST_PCI_DISTRICT_PMGSY3_FINALIZE pciDistrictCode = dbContext.MAST_PCI_DISTRICT_PMGSY3_FINALIZE.Where(z => z.MAST_DISTRICT_CODE == districtCode).FirstOrDefault();
                    if (pciDistrictCode != null)
                    {
                        dbContext.MAST_PCI_DISTRICT_PMGSY3_FINALIZE.Remove(pciDistrictCode);
                        dbContext.SaveChanges();
                        message = " PCI District definalized successfully.";
                        return true;
                    }
                    else
                    {
                        message = "District is already definalized successfully.";
                        return true;
                    }
                }
            }

            catch (DbEntityValidationException e)
            {
                ErrorLog.LogError(e, "ExistingRoadDAL().DeFinalizePCIDistrictPMGSY3DAL [DbEntityValidationException]");

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
                    sw.WriteLine("ExistingRoadDAL().DeFinalizePCIDistrictPMGSY3DAL [DbEntityValidationException]");
                    sw.WriteLine("Exception : " + new CommonFunctions().FormatErrorMessage(modelstate));
                    sw.WriteLine("---------------------------------------------------------------------------------------");
                    sw.Close();
                }
                return false;
            }
            catch (OptimisticConcurrencyException ex)
            {
                ErrorLog.LogError(ex, "ExistingRoadDAL().DeFinalizePCIDistrictPMGSY3DAL [OptimisticConcurrencyException]");
                message = "An Error Occurred While Processing Your Request.";
                return false;
            }
            catch (UpdateException ex)
            {
                ErrorLog.LogError(ex, "ExistingRoadDAL().DeFinalizePCIDistrictPMGSY3DAL [UpdateException]");
                message = "An Error Occurred While Processing Your Request.";
                return false;
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "ExistingRoadDAL().DeFinalizePCIDistrictPMGSY3DAL [Exception]");
                message = "An Error Occurred While Processing Your Request.";
                return false;
            }
            finally
            {
                dbContext.Dispose();
            }
        }
        #endregion


        #region Hab CSV Upload

        public bool UploadHabCSVDAL(HttpPostedFileBase fileSrc, string sep, ref string message, int blockcode)
        {
            System.IO.Stream MyStream;
            bool isBulkCoiped = false;
            bool flag = false;
            bool header = true;
            // SqlConnection cn = null;
            //int traceFileId = 0;
            MAST_HAB_CSV_PMGSY3 DBModel = new MAST_HAB_CSV_PMGSY3();
            int MAST_HAB_CSV_ID = 0;
            try
            {
                var TraceFileID = 0;
                dbContext = new PMGSYEntities();

                TransactionOptions transactionOptions = new TransactionOptions();
                transactionOptions.IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted;
                transactionOptions.Timeout = TimeSpan.MaxValue;

                using (TransactionScope ts = new TransactionScope(TransactionScopeOption.Required, transactionOptions))
                {
                    #region To save file details in omms.MAST_HAB_CSV_PMGSY3

                    var isEntryPresent = dbContext.MAST_HAB_CSV_PMGSY3.Where(x => x.MAST_BLOCK_CODE == blockcode).FirstOrDefault();

                    var filename = string.Empty;

                    if (isEntryPresent != null)
                    {
                        TraceFileID = isEntryPresent.MAST_HAB_CSV_FILE_ID;
                        filename = blockcode + "_" + isEntryPresent.MAST_HAB_CSV_FILE_ID + Path.GetExtension(fileSrc.FileName).ToString();
                        isEntryPresent.MAST_HAB_CSV_FILE_NAME = filename;
                        isEntryPresent.MAST_HAB_CSV_FILE_UPLOAD_DATE = System.DateTime.Now;
                        dbContext.Entry(isEntryPresent).State = System.Data.Entity.EntityState.Modified;
                    }
                    else
                    {
                        Int32? MaxID;
                        if (dbContext.MAST_HAB_CSV_PMGSY3.Count() == 0)
                        {
                            MaxID = 0;
                        }
                        else
                        {
                            MaxID = (from c in dbContext.MAST_HAB_CSV_PMGSY3 select (Int32?)c.MAST_HAB_CSV_FILE_ID ?? 0).Max();
                        }
                        ++MaxID;
                        TraceFileID = Convert.ToInt32(MaxID);
                        filename = blockcode + "_" + MaxID + Path.GetExtension(fileSrc.FileName).ToString();
                        DBModel.MAST_HAB_CSV_FILE_ID = Convert.ToInt32(MaxID);
                        DBModel.MAST_BLOCK_CODE = blockcode;
                        // DBModel.TRACEFILE_NAME_PDF = null;
                        DBModel.MAST_HAB_CSV_FILE_NAME = filename;
                        DBModel.MAST_HAB_CSV_FILE_UPLOAD_DATE = System.DateTime.Now;
                        DBModel.MAST_HAB_CSV_FILE_FINALIZED = "N";
                        DBModel.USERID = PMGSYSession.Current.UserId;
                        DBModel.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                        //dbContext = new PMGSYEntities();
                        dbContext.MAST_HAB_CSV_PMGSY3.Add(DBModel);
                    }

                    #endregion // to save file details in omms.MAST_TRACEFILE_PMGSY3


                    MAST_HAB_CSV_ID = (dbContext.MAST_HAB_DETAILS_CSV_PMGSY3.Any() ? dbContext.MAST_HAB_DETAILS_CSV_PMGSY3.Max(x => x.MAST_HAB_CSV_ID) : 0) + 1;



                    MyStream = fileSrc.InputStream;
                    List<int> listERRoadCode = new List<int>();

                    dbContext.Configuration.AutoDetectChangesEnabled = false;

                    using (TextFieldParser parser = new TextFieldParser(MyStream))
                    {
                        #region Bulk Upload
                        //DataTable dt = new DataTable("TempTable");
                        //dt.Columns.AddRange(new DataColumn[7] 
                        //                {   
                        //                    new DataColumn("MAST_HAB_CSV_ID", typeof(int)),
                        //                    new DataColumn("MAST_HAB_CSV_FILE_ID", typeof(int)),

                        //                    new DataColumn("MAST_HAB_CSV_ER_CODE", typeof(int)),
                        //                    new DataColumn("MAST_HAB_CSV_HAB_CODE", typeof(int)),  

                        //                    new DataColumn("MAST_HAB_CSV_BLOCK_CODE", typeof(int)),  

                        //                    new DataColumn("USERID", typeof(string)),
                        //                    new DataColumn("IPADD", typeof(string)),
                        //                });
                        #endregion
                        parser.TextFieldType = Microsoft.VisualBasic.FileIO.FieldType.Delimited;
                        //specify the delimiter  
                        parser.SetDelimiters(sep);
                        while (!parser.EndOfData)
                        {
                            //read Fileds  
                            string[] fields = parser.ReadFields();
                            //Processing row  
                            if (header == true)
                            { //escape first line  
                                header = false;
                            }
                            else
                            {
                                #region Bulk Upload
                                //dt.Rows.Add();
                                //int i = 0;

                                //dt.Rows[dt.Rows.Count - 1][i] = MAST_HAB_CSV_ID;
                                //dt.Rows[dt.Rows.Count - 1][i + 1] = TraceFileID;
                                //dt.Rows[dt.Rows.Count - 1][i + 2] = Convert.ToInt32(fields[0]);// Hab Code
                                //dt.Rows[dt.Rows.Count - 1][i + 3] = Convert.ToInt32(fields[1]);// ER Code
                                //dt.Rows[dt.Rows.Count - 1][i + 4] = Convert.ToInt32(fields[2]);// Block Code
                                //dt.Rows[dt.Rows.Count - 1][i + 5] = PMGSYSession.Current.UserId;
                                //dt.Rows[dt.Rows.Count - 1][i + 6] = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];

                                //MAST_HAB_CSV_ID++;


                                #endregion


                                if (blockcode != Convert.ToInt32(fields[2]))
                                {
                                    message = "Block Code in CSV is Invalid.";
                                    return false;
                                }

                                MAST_HAB_DETAILS_CSV_PMGSY3 mast_trace_drrp_score_pmgsy3 = new MAST_HAB_DETAILS_CSV_PMGSY3();
                                mast_trace_drrp_score_pmgsy3.MAST_HAB_CSV_ID = MAST_HAB_CSV_ID;
                                mast_trace_drrp_score_pmgsy3.MAST_HAB_CSV_FILE_ID = TraceFileID;

                                mast_trace_drrp_score_pmgsy3.MAST_HAB_CSV_HAB_CODE = Convert.ToInt32(fields[0]);// Hab Code
                                mast_trace_drrp_score_pmgsy3.MAST_HAB_CSV_ER_CODE = Convert.ToInt32(fields[1]);// ER Code
                                mast_trace_drrp_score_pmgsy3.MAST_HAB_CSV_BLOCK_CODE = Convert.ToInt32(fields[2]);// Block Code

                                mast_trace_drrp_score_pmgsy3.USERID = PMGSYSession.Current.UserId;
                                mast_trace_drrp_score_pmgsy3.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                                dbContext.MAST_HAB_DETAILS_CSV_PMGSY3.Add(mast_trace_drrp_score_pmgsy3);
                                MAST_HAB_CSV_ID++;
                            }
                        }
                        #region Bulk Upload

                        //string constr = ConfigurationManager.AppSettings["HAB_CSV_UPLOAD_CONNECTION_STRING"];// "data source=10.208.23.208;initial catalog=OMMAS_23JAN2020;user id=sa;password=sa@sql2k12;MultipleActiveResultSets=True;App=EntityFramework";


                        //using (SqlConnection destinationConnection = new SqlConnection(constr))
                        //{
                        //    destinationConnection.Open();

                        //    using (SqlBulkCopy bulkCopy = new SqlBulkCopy(destinationConnection))
                        //    {
                        //        try
                        //        {
                        //            bulkCopy.DestinationTableName = "OMMS.MAST_HAB_DETAILS_CSV_PMGSY3";
                        //            bulkCopy.WriteToServer(dt);
                        //            isBulkCoiped = true;
                        //        }
                        //        catch (Exception ex)
                        //        {
                        //            Console.WriteLine(ex.Message);
                        //        }
                        //        finally
                        //        {
                        //            destinationConnection.Close();
                        //        }
                        //    }
                        //}




                        #endregion


                        #region  Bulk Upload

                        ////if (!dbContext.MAST_HAB_CSV_PMGSY3.Any(x => x.MAST_HAB_CSV_FILE_ID == TraceFileID))
                        ////{
                        ////    message = "No records present against the File.";
                        ////    return false;
                        ////}


                        #endregion


                        dbContext.SaveChanges();
                        dbContext.Configuration.AutoDetectChangesEnabled = true;

                        if (fileSrc.ContentLength > 0)
                        {
                            fileSrc.SaveAs(Path.Combine(ConfigurationManager.AppSettings["DRRP_HAB_TRACE_CSV_PATH"], filename));
                            flag = true;
                            message = "Mapped successfully";
                            //  message = "PFMS Contractor mapped successfully";
                        }
                        else
                        {
                            message = "File Content invalid";
                        }
                        ts.Complete();
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "UploadHabCSVDAL");
                message = "Error occurred while CSV upload";
            }
            finally
            {
                //if (cn != null)
                //{
                //    cn.Dispose();
                //}
                dbContext.Configuration.AutoDetectChangesEnabled = true;
            }
            return flag;
        }


        public Array GetTraceHabCSVFilesListDAL(int page, int rows, string sidx, string sord, out int totalRecords, int blockcode)
        {
            dbContext = new PMGSYEntities();
            var record = dbContext.MAST_HAB_CSV_PMGSY3.Where(x => x.MAST_BLOCK_CODE == blockcode && x.MAST_HAB_CSV_FILE_NAME != null).ToList();

            totalRecords = record.Count();

            string VirtualDirectoryUrl = string.Empty;
            string PhysicalPath = string.Empty;


            //Virtual path is not changed
            VirtualDirectoryUrl = ConfigurationManager.AppSettings["DRRP_HAB_TRACE_CSV_PATH_VIRTUAL_DIR_PATH"];// value="http://egovmsdb:82/Files/QM/NQM/" />
            PhysicalPath = ConfigurationManager.AppSettings["DRRP_HAB_TRACE_CSV_PATH"]; // value="D:\OmmasFiles\TRACEMAPSHAB\CSV"

            return record.Select(fileDetails => new
            {
                id = fileDetails.MAST_HAB_CSV_FILE_ID + "$" + fileDetails.MAST_BLOCK_CODE,
                cell = new[] {       
                    
                                       URLEncrypt.EncryptParameters(new string[] { fileDetails.MAST_HAB_CSV_FILE_NAME + "$" +  fileDetails.MAST_HAB_CSV_FILE_ID}),       
                                       
                                       
                                          //  SRRDA = 2 and ITNO= 36
                                       (fileDetails.MAST_HAB_CSV_FILE_FINALIZED == "N" && (PMGSYSession.Current.RoleCode == 2 || PMGSYSession.Current.RoleCode == 36)) ?


                                       "<a href='#' title='Click here to delete the File and File Details' class='ui-icon ui-icon-trash ui-align-center' onClick=DeleteHabCSVFileDetails('" + fileDetails.MAST_HAB_CSV_FILE_ID.ToString().Trim() + "&" +   fileDetails.MAST_BLOCK_CODE +"'); return false;'>Delete</a>"
                                       
                                       
                                       : ((fileDetails.MAST_HAB_CSV_FILE_FINALIZED == "Y" && dbContext.MAST_TRACEFILE_PMGSY3.Any(m=>m.MAST_BLOCK_CODE==blockcode && m.TRACEFILE_FINALIZE=="N") &&(PMGSYSession.Current.RoleCode == 2 || PMGSYSession.Current.RoleCode == 36)) 
                                      ? "<a href='#' title='Click here to delete the File and File Details as Tracefile is not finalized. (i.e. Tracefile is unlocked.)' class='ui-icon ui-icon-trash ui-align-center' onClick=DeleteHabCSVFileDetails('" + fileDetails.MAST_HAB_CSV_FILE_ID.ToString().Trim() + "&" +   fileDetails.MAST_BLOCK_CODE +"'); return false;'>Delete</a>"
                                      : "<a href='#' class='ui-icon ui-icon-locked ui-align-center'>"),
                                       
                                       
                                       fileDetails.MAST_HAB_CSV_FILE_FINALIZED == "N" ? "No" : "Yes",
                                      
                                       (PMGSYSession.Current.RoleCode == 22) ? 
                                      "<a href='#' title='Click here to Finalize the File Details' class='ui-icon ui-icon-plusthick ui-align-center' onClick=FinaliseHabCSVDetails('" +        fileDetails.MAST_HAB_CSV_FILE_ID.ToString().Trim()+ "'); return false;>Finalise</a>"
                                        : "-",
            }
            }).ToArray();

        }


        public String FinaliseHabCSVDAL(Int32 FileID)
        {
            try
            {
                dbContext = new PMGSYEntities();

                MAST_HAB_CSV_PMGSY3 masterExistingRoadModel = dbContext.MAST_HAB_CSV_PMGSY3.Find(FileID);

                masterExistingRoadModel.MAST_HAB_CSV_FILE_FINALIZED = "Y";
                masterExistingRoadModel.MAST_HAB_CSV_FILE_FINALIZED_DATE = System.DateTime.Now;
                masterExistingRoadModel.MAST_HAB_CSV_HAB_FINALIZED_BY = PMGSYSession.Current.UserId;

                masterExistingRoadModel.USERID = PMGSYSession.Current.UserId;
                masterExistingRoadModel.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                dbContext.Entry(masterExistingRoadModel).State = System.Data.Entity.EntityState.Modified;
                dbContext.SaveChanges();
                return string.Empty;
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "ExistingRoadsDAL().FinaliseHabCSVDAL()");
                return "An Error occured while your request processing";
            }
            finally
            {
                dbContext.Dispose();
            }
        }
        #endregion


        #region Map Unmap Hab
        public Array MapUnmapHabGetDistrictListMORDDAL(int page, int rows, string sidx, string sord, out int totalRecords, int statecode, int districtcode)
        {
            PMGSYEntities dbContext = new PMGSYEntities();

            var BlockList = (from c in dbContext.MASTER_BLOCK
                             where
                             c.MAST_DISTRICT_CODE == (districtcode == 0 ? c.MAST_DISTRICT_CODE : districtcode)
                             && c.MASTER_DISTRICT.MAST_STATE_CODE == (districtcode == 0 ? statecode : c.MASTER_DISTRICT.MAST_STATE_CODE)
                             && c.MAST_BLOCK_ACTIVE == "Y"
                             select new
                             {
                                 c.MAST_BLOCK_NAME,
                                 c.MAST_BLOCK_CODE,
                                 c.MASTER_DISTRICT.MAST_DISTRICT_NAME
                             }).OrderBy(x => x.MAST_DISTRICT_NAME).ToList();

            try
            {
                totalRecords = BlockList.Count();
                var isFinalizedcheck = (from item in dbContext.MAST_FACILITY_DISTRICT_PMGSY3_FINALIZE
                                        join MB in dbContext.MASTER_BLOCK on item.MAST_DISTRICT_CODE
                                        equals MB.MAST_DISTRICT_CODE
                                        select new
                                        {
                                            MB.MAST_DISTRICT_CODE,
                                            MB.MAST_BLOCK_CODE,
                                            item.IS_FINALIZED
                                        }).ToList();


                BlockList = BlockList.OrderBy(x => x.MAST_DISTRICT_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                return BlockList.Select(obj => new
                {
                    cell = new[]
                    {
                        obj.MAST_DISTRICT_NAME.ToString(),

                        obj.MAST_BLOCK_NAME.ToString(),
                        
                        (dbContext.MAST_FACILITY_BLOCK_PMGSY3_FINALIZE.Where(x => x.MAST_BLOCK_CODE == obj.MAST_BLOCK_CODE && x.IS_FINALIZED == "Y").Any())?
                        "<a href='#' title='Click here to Download Facility List' class='ui-icon  ui-icon-arrowthickstop-1-s ui-align-center' onClick='DownloadFacilityList(\"" + 
                        obj.MAST_BLOCK_CODE + "\"); return false;'>Download</a>" : "<span style='color:red;'>Facility details not finalized</span>",
                        
                        #region for PDF upload condtions
                        (dbContext.MAST_TRACEFILE_PMGSY3.Where(x => x.MAST_BLOCK_CODE == obj.MAST_BLOCK_CODE).Select(x => x.TRACEFILE_NAME_PDF).FirstOrDefault() != null) ?
                        "<a href='#' title='Click here to view PDF' class='ui-icon ui-icon-zoomin ui-align-center' onClick='UploadPDF(\"" + obj.MAST_BLOCK_CODE + "\"); return false;'>Upload</a>": "<span style='color:red;'>PDF File not uploaded</                           span>" ,
                        #endregion
                        
                        #region for CSV upload condtions
                        (dbContext.MAST_TRACEFILE_PMGSY3.Where(x => x.MAST_BLOCK_CODE == obj.MAST_BLOCK_CODE).Select(x => x.TRACEFILE_NAME_CSV).FirstOrDefault() != null) ?
                        "<a href='#' title='Click here to view CSV' class='ui-icon ui-icon-zoomin ui-align-center' onClick='UploadCSV(\"" + obj.MAST_BLOCK_CODE + "\"); return false;'>Upload</a>" : "<span style='color:red;'>CSV File not uploaded</                          span>",
                        #endregion


                         #region for Hab CSV upload at MORD  condtions
                        (dbContext.MAST_HAB_CSV_PMGSY3.Where(x => x.MAST_BLOCK_CODE == obj.MAST_BLOCK_CODE).Select(x => x.MAST_HAB_CSV_FILE_NAME).FirstOrDefault() != null) ?
                        "<a href='#' title='Click here to view Habitation CSV' class='ui-icon ui-icon-zoomin ui-align-center' onClick='UploadHabCSV(\"" + obj.MAST_BLOCK_CODE + "\"); return false;'>Upload</a>" : "<span style='color:red;'>Habitation CSV File not uploaded</span>",
                        #endregion

                      
                        //
                        dbContext.MAST_HAB_CSV_PMGSY3.Where(x => x.MAST_BLOCK_CODE == obj.MAST_BLOCK_CODE && x.MAST_HAB_CSV_FILE_FINALIZED == "Y" && x.MAST_MAP_UNMAP=="Y").Any()?"Habitations are mapped from CSV": "<a href='#' title='Click here to delete already mapped Habitations and Map Habitations from CSV.' class='ui-icon ui-icon-plusthick ui-align-center' onClick=MapUnmapHabDetails('" +obj.MAST_BLOCK_CODE+ "'); return false;>MapUnmapHabDetails</a>",
                        //



                        (PMGSYSession.Current.RoleCode == 22 && dbContext.MAST_TRACEFILE_PMGSY3.Where(x => x.MAST_BLOCK_CODE == obj.MAST_BLOCK_CODE 
                            && x.TRACEFILE_NAME_PDF !=null 
                            && x.TRACEFILE_NAME_CSV != null 
                            && x.TRACEFILE_FINALIZE == "N").Any()) ? 
                                      "<a href='#' title='Click here to Finalize the File Details' class='ui-icon ui-icon-plusthick ui-align-center' onClick=FinalisePDFDetails('" +                                                                                                                obj.MAST_BLOCK_CODE+ "'); return false;>Finalise</a>"
                                        //: (PMGSYSession.Current.RoleCode == 36 ? "Trace Map not yet Finalized" : "-"),
                                        : (PMGSYSession.Current.RoleCode == 36 && dbContext.MAST_TRACEFILE_PMGSY3.Where(x => x.MAST_BLOCK_CODE == obj.MAST_BLOCK_CODE && x.TRACEFILE_FINALIZE == "Y").Any() ? "Trace Map Finalized" : 
                                        dbContext.MAST_TRACEFILE_PMGSY3.Where(x => x.MAST_BLOCK_CODE == obj.MAST_BLOCK_CODE && x.TRACEFILE_FINALIZE == "Y").Any() ? "Trace Map Finalized" : 
                                        "Trace Map not yet Finalized"),
                        

                        
                        //dbContext.PLAN_ROAD.Where(x => x.MAST_BLOCK_CODE == obj.MAST_BLOCK_CODE && x.MAST_PMGSY_SCHEME == 4 && x.PLAN_LOCK_STATUS == "Y").Any() ? 
                        //"Candidate Roads Entered"
                        //: "B"
                        //"<a href='#' title='Click here to Definalize the File Details' class='ui-icon ui-icon-unlocked ui-align-center' onClick=DefinalizeTraceMapDetails('" +                                                                obj.MAST_BLOCK_CODE+ "'); return false;>Finalise</a>",


                        
                        dbContext.MAST_TRACEFILE_PMGSY3.Where(x => x.MAST_BLOCK_CODE == obj.MAST_BLOCK_CODE && x.TRACEFILE_FINALIZE == "Y").Any() ?
                        (dbContext.MAST_CN_BLOCK_PMGSY3_FINALIZE.Where(x => x.MAST_BLOCK_CODE == obj.MAST_BLOCK_CODE && x.IS_FINALIZED == "Y").Any() ? 
                        "<span style='color:red;'>Block is Finalized</span>"
                        :
                        "<a href='#' title='Click here to Definalize the File Details' class='ui-icon ui-icon-unlocked ui-align-center' onClick=DefinalizeTraceMapDetails('" +                                                                                                  obj.MAST_BLOCK_CODE+ "'); return false;>Finalise</a>") : "<span style='color:red;'>Not Available</span>"

                    }


                }).ToArray();

            }
            catch (Exception ex)
            {
                totalRecords = BlockList.Count();
                ErrorLog.LogError(ex, "ExistingRoadsDAL/MapUnmapHabGetDistrictListMORDDAL");
                return null;
            }
            finally
            {
                if (dbContext != null)
                    dbContext.Dispose();
            }
        }



        public Array MapUnmapHabGetDistrictListDAL(int page, int rows, string sidx, string sord, out int totalRecords, int districtcode)
        {
            PMGSYEntities dbContext = new PMGSYEntities();

            //var BlockList = new CommonFunctions().PopulateBlocks(blockcode);
            //BlockList.Remove(BlockList.Find(x => x.Text.Equals("Select Block")));
            var BlockList = (from c in dbContext.MASTER_BLOCK
                             where
                             c.MAST_DISTRICT_CODE == districtcode && c.MAST_BLOCK_ACTIVE == "Y"
                             select new
                             {
                                 c.MAST_BLOCK_NAME,
                                 c.MAST_BLOCK_CODE,
                                 c.MASTER_DISTRICT.MAST_DISTRICT_NAME
                             }).OrderBy(x => x.MAST_DISTRICT_NAME).ToList();

            try
            {
                totalRecords = BlockList.Count();
                var isFinalizedcheck = (from item in dbContext.MAST_FACILITY_DISTRICT_PMGSY3_FINALIZE
                                        join MB in dbContext.MASTER_BLOCK on item.MAST_DISTRICT_CODE
                                        equals MB.MAST_DISTRICT_CODE
                                        select new
                                        {
                                            MB.MAST_DISTRICT_CODE,
                                            MB.MAST_BLOCK_CODE,
                                            item.IS_FINALIZED
                                        }).ToList();


                BlockList = BlockList.OrderBy(x => x.MAST_BLOCK_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                return BlockList.Select(obj => new
                {
                    cell = new[]
                    {
                        obj.MAST_DISTRICT_NAME.ToString(),
                        obj.MAST_BLOCK_NAME.ToString(),
                        
                        //(dbContext.FACILITY_HABITATION_MAPPING.Any(x => x.MASTER_BLOCK_CODE == obj.MAST_BLOCK_CODE))?
                        //(isFinalizedcheck.Where(x => x.MAST_BLOCK_CODE == obj.MAST_BLOCK_CODE && x.IS_FINALIZED == "Y").Any())?
                        (dbContext.MAST_FACILITY_BLOCK_PMGSY3_FINALIZE.Where(x => x.MAST_BLOCK_CODE == obj.MAST_BLOCK_CODE && x.IS_FINALIZED == "Y").Any())?
                        "<a href='#' title='Click here to Download Facility List' class='ui-icon  ui-icon-arrowthickstop-1-s ui-align-center' onClick='DownloadFacilityList(\"" + 
                        obj.MAST_BLOCK_CODE + "\"); return false;'>Download</a>" : "<span style='color:red;'>Facility details not finalized</span>",
                        
                        #region for PDF upload condtions
                        (PMGSYSession.Current.RoleCode == 2 || PMGSYSession.Current.RoleCode == 36)?

                        //(! dbContext.FACILITY_HABITATION_MAPPING.Any(x => x.MASTER_BLOCK_CODE == obj.MAST_BLOCK_CODE))? 
                        //(!isFinalizedcheck.Where(x => x.MAST_BLOCK_CODE == obj.MAST_BLOCK_CODE && x.IS_FINALIZED == "Y").Any())? 
                        (!dbContext.MAST_FACILITY_BLOCK_PMGSY3_FINALIZE.Where(x => x.MAST_BLOCK_CODE == obj.MAST_BLOCK_CODE && x.IS_FINALIZED == "Y").Any())?
                        "<span style='color:red;'>File cannot be uploaded as Facility details not finalized</span>"
                        :

                        (!dbContext.MAST_DRRP_BLOCK_PMGSY3_FINALIZE.Where(objBlock => objBlock.MAST_BLOCK_CODE == obj.MAST_BLOCK_CODE).Any()) ? "<span style='color:red;'>Block is not finalized under DRRP</span>" 
                        :
                        (dbContext.MAST_TRACEFILE_PMGSY3.Where(x => x.MAST_BLOCK_CODE == obj.MAST_BLOCK_CODE).Select(x => x.TRACEFILE_NAME_PDF).FirstOrDefault() == null) ?

                        "<a href='#' title='Click here to upload PDF' class='ui-icon ui-icon-plusthick ui-align-center' onClick='UploadPDF(\"" + obj.MAST_BLOCK_CODE + "\"); return false;'>Upload</a>"
                        :"<a href='#' title='Click here to view PDF' class='ui-icon ui-icon-zoomin ui-align-center' onClick='UploadPDF(\"" + obj.MAST_BLOCK_CODE + "\"); return false;'>Upload</a>"
                        :
                        (dbContext.MAST_TRACEFILE_PMGSY3.Where(x => x.MAST_BLOCK_CODE == obj.MAST_BLOCK_CODE).Select(x => x.TRACEFILE_NAME_PDF).FirstOrDefault() != null) ?
                        "<a href='#' title='Click here to view PDF' class='ui-icon ui-icon-zoomin ui-align-center' onClick='UploadPDF(\"" + obj.MAST_BLOCK_CODE + "\"); return false;'>Upload</a>": "<span style='color:red;'>PDF File not uploaded</span>" ,
                        #endregion
                        
                        #region for CSV upload condtions
                        (PMGSYSession.Current.RoleCode == 2 || PMGSYSession.Current.RoleCode == 36) ?
                        //(! dbContext.FACILITY_HABITATION_MAPPING.Any(x => x.MASTER_BLOCK_CODE == obj.MAST_BLOCK_CODE))? 
                        //(!isFinalizedcheck.Where(x => x.MAST_BLOCK_CODE == obj.MAST_BLOCK_CODE && x.IS_FINALIZED == "Y").Any())? 
                        (!dbContext.MAST_FACILITY_BLOCK_PMGSY3_FINALIZE.Where(x => x.MAST_BLOCK_CODE == obj.MAST_BLOCK_CODE && x.IS_FINALIZED == "Y").Any())?
                        "<span style='color:red;'>File cannot be uploaded as Facility details not finalized</span>"
                        :
                        (!dbContext.MAST_DRRP_BLOCK_PMGSY3_FINALIZE.Where(objBlock => objBlock.MAST_BLOCK_CODE == obj.MAST_BLOCK_CODE).Any()) ? "<span style='color:red;'>Block is not finalized under DRRP</span>" 
                        :
                        (dbContext.MAST_TRACEFILE_PMGSY3.Where(x => x.MAST_BLOCK_CODE == obj.MAST_BLOCK_CODE).Select(x => x.TRACEFILE_NAME_CSV).FirstOrDefault() == null) ?
                        "<a href='#' title='Click here to upload CSV' class='ui-icon ui-icon-plusthick ui-align-center' onClick='UploadCSV(\"" + obj.MAST_BLOCK_CODE + "\"); return false;'>Upload</a>"
                        :"<a href='#' title='Click here to view CSV' class='ui-icon ui-icon-zoomin ui-align-center' onClick='UploadCSV(\"" + obj.MAST_BLOCK_CODE + "\"); return false;'>Upload</a>"
                        :
                        (dbContext.MAST_TRACEFILE_PMGSY3.Where(x => x.MAST_BLOCK_CODE == obj.MAST_BLOCK_CODE).Select(x => x.TRACEFILE_NAME_CSV).FirstOrDefault() != null) ?
                        "<a href='#' title='Click here to view CSV' class='ui-icon ui-icon-zoomin ui-align-center' onClick='UploadCSV(\"" + obj.MAST_BLOCK_CODE + "\"); return false;'>Upload</a>" : "<span style='color:red;'>CSV File not uploaded</span>",
                        #endregion

                    //     "<a href='#' title='Click here to upload Habitation CSV' class='ui-icon ui-icon-plusthick ui-align-center' onClick='UploadHabCSV(\"" + obj.MAST_BLOCK_CODE + "\"); return false;'>Upload</a>",


                         #region Hab CSV Upload 01 May 2020
                         ((PMGSYSession.Current.RoleCode == 2 || PMGSYSession.Current.RoleCode == 36) ?
          
                        (!dbContext.MAST_FACILITY_BLOCK_PMGSY3_FINALIZE.Where(x => x.MAST_BLOCK_CODE == obj.MAST_BLOCK_CODE && x.IS_FINALIZED == "Y").Any())?
                        "<span style='color:red;'>File cannot be uploaded as Facility details not finalized</span>"
                        :
                        (!dbContext.MAST_DRRP_BLOCK_PMGSY3_FINALIZE.Where(objBlock => objBlock.MAST_BLOCK_CODE == obj.MAST_BLOCK_CODE).Any()) ? "<span style='color:red;'>Block is not finalized under DRRP</span>" 
                        :
                        (dbContext.MAST_HAB_CSV_PMGSY3.Where(x => x.MAST_BLOCK_CODE == obj.MAST_BLOCK_CODE).Select(x => x.MAST_HAB_CSV_FILE_NAME).FirstOrDefault() == null) ?

                       // This is Newly added Line : Needs to added at ITNO and PIU Code in Hab Mapping
                        (dbContext.MAST_TRACEFILE_PMGSY3.Where(x => x.MAST_BLOCK_CODE == obj.MAST_BLOCK_CODE && x.TRACEFILE_FINALIZE == "Y").Any() ? "Trace Map is already Finalized" :"-")
                       
                        :"<a href='#' title='Click here to view Habitation CSV' class='ui-icon ui-icon-zoomin ui-align-center' onClick='UploadHabCSV(\"" + obj.MAST_BLOCK_CODE + "\"); return false;'>Upload</a>"
                        :
                        (dbContext.MAST_HAB_CSV_PMGSY3.Where(x => x.MAST_BLOCK_CODE == obj.MAST_BLOCK_CODE).Select(x => x.MAST_HAB_CSV_FILE_NAME).FirstOrDefault() != null) ?
                        "<a href='#' title='Click here to view Habitation CSV' class='ui-icon ui-icon-zoomin ui-align-center' onClick='UploadHabCSV(\"" + obj.MAST_BLOCK_CODE + "\"); return false;'>Upload</a>" : "<span style='color:red;'>CSV File not uploaded</span>"),
                        #endregion



                        //
                        dbContext.MAST_HAB_CSV_PMGSY3.Where(x => x.MAST_BLOCK_CODE == obj.MAST_BLOCK_CODE && x.MAST_HAB_CSV_FILE_FINALIZED == "Y" && x.MAST_MAP_UNMAP=="Y").Any()?"Habitations are mapped from CSV": "<a href='#' title='Click here to delete already mapped Habitations and Map Habitations from CSV.' class='ui-icon ui-icon-plusthick ui-align-center' onClick=MapUnmapHabDetails('" +obj.MAST_BLOCK_CODE+ "'); return false;>MapUnmapHabDetails</a>",
                        //







                       #region Finalization
                        (PMGSYSession.Current.RoleCode == 22 && dbContext.MAST_TRACEFILE_PMGSY3.Where(x => x.MAST_BLOCK_CODE == obj.MAST_BLOCK_CODE 
                            && x.TRACEFILE_NAME_PDF !=null 
                            && x.TRACEFILE_NAME_CSV != null 
                            && x.TRACEFILE_FINALIZE == "N").Any()) ? (dbContext.MAST_HAB_CSV_PMGSY3.Where(x => x.MAST_BLOCK_CODE == obj.MAST_BLOCK_CODE && x.MAST_HAB_CSV_FILE_FINALIZED == "Y").Any()?

                                      "<a href='#' title='Click here to Finalize the File Details' class='ui-icon ui-icon-plusthick ui-align-center' onClick=FinalisePDFDetails('" +obj.MAST_BLOCK_CODE+ "'); return false;>Finalise</a>"
                                      :"Habitation CSV is not Finalized By PIU"
                                      )
                                      
                                      //: (PMGSYSession.Current.RoleCode == 36 ? "Trace Map not yet Finalized" : "-"),
                                        : (PMGSYSession.Current.RoleCode == 36 && dbContext.MAST_TRACEFILE_PMGSY3.Where(x => x.MAST_BLOCK_CODE == obj.MAST_BLOCK_CODE && x.TRACEFILE_FINALIZE == "Y").Any() ? "Trace Map Finalized" : 
                                        dbContext.MAST_TRACEFILE_PMGSY3.Where(x => x.MAST_BLOCK_CODE == obj.MAST_BLOCK_CODE && x.TRACEFILE_FINALIZE == "Y").Any() ? "Trace Map Finalized" : 
                                        "Trace Map not yet Finalized")
#endregion

                    }


                }).ToArray();

            }
            catch (Exception ex)
            {
                totalRecords = BlockList.Count();
                ErrorLog.LogError(ex, "ExistingRoadsDAL/GetDistrictListDAL");
                return null;
            }
            finally
            {
                if (dbContext != null)
                    dbContext.Dispose();
            }
        }
        #endregion


        #region Delete DRRP in PMGSY III
        public Array DeleteListExistingRoadsPMGSY3DAL(int stateCode, int districtCode, int blockCode, int categoryCode, int ownerCode, int? page, int? rows, string sidx, string sord, out long totalRecords, string filters)
        {
            try
            {
                JavaScriptSerializer js = null;
                PMGSY.Common.CommonFunctions.SearchJson test = new PMGSY.Common.CommonFunctions.SearchJson();
                string roadName = string.Empty;
                int erRoadCode = 0;

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
                            case "ERCode": erRoadCode = Convert.ToInt32(item.data);
                                break;
                            default:
                                break;
                        }
                    }
                }

                dbContext = new PMGSYEntities();


                stateCode = PMGSYSession.Current.StateCode;


                short roleCode = dbContext.UM_User_Master.Where(m => m.UserID == PMGSYSession.Current.UserId).Select(m => m.DefaultRoleID).FirstOrDefault();

                ///Get Exisiting Roads for Scheme 2 in case of PMGSY3
                var lstExistingRoadsDetails = dbContext.GET_EXISTING_ROADS((stateCode <= 0 ? 0 : stateCode), (districtCode <= 0 ? 0 : districtCode), (blockCode <= 0 ? 0 : blockCode), categoryCode, roadName, PMGSYSession.Current.PMGSYScheme, (roleCode == 54 ? (short)22 : roleCode)).ToList();

                if (erRoadCode > 0)
                {
                    lstExistingRoadsDetails = lstExistingRoadsDetails.Where(x => x.MAST_ER_ROAD_CODE == erRoadCode).ToList();
                }

                totalRecords = lstExistingRoadsDetails.Count();

                if (sidx.Trim() != string.Empty)
                {
                    if (sord.ToString() == "asc")
                    {
                        switch (sidx)
                        {
                            case "MAST_ER_ROAD_CODE":
                                lstExistingRoadsDetails = lstExistingRoadsDetails.OrderBy(x => x.MAST_ER_ROAD_CODE).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                            case "MAST_ER_ROAD_NUMBER":
                                lstExistingRoadsDetails = lstExistingRoadsDetails.OrderBy(x => x.MAST_ER_ROAD_NUMBER).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                            case "MAST_ER_ROAD_NAME":
                                lstExistingRoadsDetails = lstExistingRoadsDetails.OrderBy(x => x.MAST_ER_ROAD_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                            case "MAST_ER_ROAD_OWNER":
                                lstExistingRoadsDetails = lstExistingRoadsDetails.OrderBy(x => x.AGENCY_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                            case "MAST_CORE_NETWORK":
                                lstExistingRoadsDetails = lstExistingRoadsDetails.OrderBy(x => x.MAST_CORE_NETWORK).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                            case "MAST_ROAD_SHORT_DESC":
                                lstExistingRoadsDetails = lstExistingRoadsDetails.OrderBy(x => x.MAST_ROAD_SHORT_DESC).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                            case "MAST_ER_ROAD_TYPE":
                                lstExistingRoadsDetails = lstExistingRoadsDetails.OrderBy(x => x.MAST_ER_ROAD_TYPE).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                            default:
                                lstExistingRoadsDetails = lstExistingRoadsDetails.OrderBy(x => x.MAST_ER_ROAD_CODE).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                        }
                    }
                    else
                    {

                        switch (sidx)
                        {
                            case "MAST_ER_ROAD_CODE":
                                lstExistingRoadsDetails = lstExistingRoadsDetails.OrderByDescending(x => x.MAST_ER_ROAD_CODE).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                            case "MAST_ER_ROAD_NUMBER":
                                lstExistingRoadsDetails = lstExistingRoadsDetails.OrderByDescending(x => x.MAST_ER_ROAD_NUMBER).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                            case "MAST_ER_ROAD_NAME":
                                lstExistingRoadsDetails = lstExistingRoadsDetails.OrderByDescending(x => x.MAST_ER_ROAD_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                            case "MAST_ER_ROAD_OWNER":
                                lstExistingRoadsDetails = lstExistingRoadsDetails.OrderByDescending(x => x.AGENCY_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                            case "MAST_CORE_NETWORK":
                                lstExistingRoadsDetails = lstExistingRoadsDetails.OrderByDescending(x => x.MAST_CORE_NETWORK).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                            case "MAST_ROAD_SHORT_DESC":
                                lstExistingRoadsDetails = lstExistingRoadsDetails.OrderByDescending(x => x.MAST_ROAD_SHORT_DESC).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                            case "MAST_ER_ROAD_TYPE":
                                lstExistingRoadsDetails = lstExistingRoadsDetails.OrderByDescending(x => x.MAST_ER_ROAD_TYPE).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                            default:
                                lstExistingRoadsDetails = lstExistingRoadsDetails.OrderByDescending(x => x.MAST_ER_ROAD_CODE).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                        }
                    }
                }
                else
                {
                    lstExistingRoadsDetails = lstExistingRoadsDetails.OrderByDescending(x => x.MAST_ER_ROAD_CODE).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                }

                var result = lstExistingRoadsDetails.Select(roadDetails => new
                {
                    roadDetails.MAST_ER_ROAD_CODE,
                    roadDetails.MAST_ROAD_SHORT_DESC,
                    roadDetails.MAST_ER_ROAD_NUMBER,
                    roadDetails.MAST_ER_ROAD_NAME,
                    roadDetails.MAST_ER_ROAD_TYPE,
                    roadDetails.AGENCY_NAME,
                    roadDetails.MAST_CORE_NETWORK,
                    roadDetails.UNLOCK_BY_MORD,
                    //MAST_ER_ROAD_CODE_PMGSY1 = dbContext.MASTER_EXISTING_ROADS.Where(m => m.MAST_ER_ROAD_CODE == roadDetails.MAST_ER_ROAD_CODE).Select(m => m.MAST_ER_ROAD_CODE_PMGSY1).FirstOrDefault(),
                    roadDetails.MAST_ER_ROAD_CODE_PMGSY1,
                    roadDetails.MAST_ROAD_CAT_CODE_PMGSY2,
                    roadDetails.MAST_ROAD_CAT_CODE_PMGSY1,
                    roadDetails.MAST_BLOCK_NAME,
                    PLAN_CN_ROAD_CODE = (dbContext.PLAN_ROAD.Any(m => m.MAST_ER_ROAD_CODE == roadDetails.MAST_ER_ROAD_CODE))
                                         ? dbContext.PLAN_ROAD.Where(m => m.MAST_ER_ROAD_CODE == roadDetails.MAST_ER_ROAD_CODE).Select(m => m.PLAN_CN_ROAD_CODE).FirstOrDefault()
                                         : (dbContext.PLAN_ROAD_DRRP.Any(m => m.MAST_ER_ROAD_CODE == roadDetails.MAST_ER_ROAD_CODE) ? dbContext.PLAN_ROAD_DRRP.Where(m => m.MAST_ER_ROAD_CODE == roadDetails.MAST_ER_ROAD_CODE).Select(m => m.PLAN_CN_ROAD_CODE).FirstOrDefault() : dbContext.PLAN_ROAD_MRL_PMGSY3.Where(m => m.MAST_ER_ROAD_CODE == roadDetails.MAST_ER_ROAD_CODE).Select(m => m.PLAN_CN_ROAD_CODE).FirstOrDefault())


                }).ToArray();

                return result.Select(item => new
                {

                    id = item.MAST_ER_ROAD_CODE,
                    cell = new[]
                    {       
                        item.MAST_ER_ROAD_CODE.ToString(),
                        item.MAST_ROAD_SHORT_DESC, //Road Category Short Desc
                        item.MAST_ER_ROAD_NUMBER,
                        item.MAST_ER_ROAD_NAME,
                         item.MAST_ER_ROAD_TYPE, //Road Type
                        (dbContext.MASTER_EXISTING_ROADS.Where(m=>m.MAST_ER_ROAD_CODE == item.MAST_ER_ROAD_CODE).Select(m=>m.MAST_ER_ROAD_END_CHAIN).FirstOrDefault() - dbContext.MASTER_EXISTING_ROADS.Where(m=>m.MAST_ER_ROAD_CODE == item.MAST_ER_ROAD_CODE).Select(m=>m.MAST_ER_ROAD_STR_CHAIN).FirstOrDefault()).ToString(),
                        item.AGENCY_NAME,
                        

                        item.MAST_BLOCK_NAME,
                        dbContext.PLAN_ROAD.Any(m=>m.PLAN_CN_ROAD_CODE==item.PLAN_CN_ROAD_CODE)? dbContext.PLAN_ROAD.Where(m=>m.PLAN_CN_ROAD_CODE==item.PLAN_CN_ROAD_CODE).Select(m=>m.PLAN_RD_NAME).FirstOrDefault()+", ( "+item.PLAN_CN_ROAD_CODE+" )":"-",

                       IsPCI(item.MAST_ER_ROAD_CODE) ?"<a href='#' title='Click here to delete PCI details' class='ui-icon ui-icon-trash ui-align-center' onClick =DeletePCIDAL('"+URLEncrypt.EncryptParameters1(new string[]{"RoadCode =" + item.MAST_ER_ROAD_CODE.ToString().Trim()})+"'); return false;'></a>":"-",
                       IsCBR(item.MAST_ER_ROAD_CODE) ?"<a href='#' title='Click here to delete CBR details' class='ui-icon ui-icon-trash ui-align-center' onClick =DeleteCBRDAL('"+URLEncrypt.EncryptParameters1(new string[]{"RoadCode =" + item.MAST_ER_ROAD_CODE.ToString().Trim()})+"'); return false;'></a>":"-",
                       IsCDWorks(item.MAST_ER_ROAD_CODE)?"<a href='#' title='Click here to delete CD Works details' class='ui-icon ui-icon-trash ui-align-center' onClick =DeleteCDWorksDAL('"+URLEncrypt.EncryptParameters1(new string[]{"RoadCode =" + item.MAST_ER_ROAD_CODE.ToString().Trim()})+"'); return false;'></a>":"-",
                       
                       IsHab(item.MAST_ER_ROAD_CODE)?"<a href='#' title='Click here to delete Habitations details' class='ui-icon ui-icon-trash ui-align-center' onClick =DeleteHabitationsDAL('"+URLEncrypt.EncryptParameters1(new string[]{"RoadCode =" + item.MAST_ER_ROAD_CODE.ToString().Trim()})+"'); return false;'></a>":"-",
                       IsSurface(item.MAST_ER_ROAD_CODE)?"<a href='#' title='Click here to delete Surface Types details' class='ui-icon ui-icon-trash ui-align-center' onClick =DeleteSurfaceTypesDAL('"+URLEncrypt.EncryptParameters1(new string[]{"RoadCode =" + item.MAST_ER_ROAD_CODE.ToString().Trim()})+"'); return false;'></a>":"-",
                       IsTraffice(item.MAST_ER_ROAD_CODE)? "<a href='#' title='Click here to delete Traffice Intensity details' class='ui-icon ui-icon-trash ui-align-center' onClick =DeleteTrafficeIntensityDAL('"+URLEncrypt.EncryptParameters1(new string[]{"RoadCode =" + item.MAST_ER_ROAD_CODE.ToString().Trim()})+"'); return false;'></a>":"-",
                 
                       true? "<a href='#' title='Click here to delete DRRP / MRL details' class='ui-icon ui-icon-trash ui-align-center' onClick =DeleteDRRPandMRLDAL('"+URLEncrypt.EncryptParameters1(new string[]{"RoadCode =" + item.MAST_ER_ROAD_CODE.ToString().Trim()})+"'); return false;'></a>":"-",

                      
                      
                      "<a href='#' title='Click here to view existing roads details' class='ui-icon ui-icon-zoomin ui-align-center' onClick =ShowDetails('"+URLEncrypt.EncryptParameters1(new string[]{"RoadCode =" + item.MAST_ER_ROAD_CODE.ToString().Trim()})+"'); return false;'></a>",
                      

                       "<a href='#' title='Click here to delete details' class='ui-icon ui-icon-trash ui-align-center' onClick =DeletePMGSY3DRRPDetails('"+URLEncrypt.EncryptParameters1(new string[]{"RoadCode =" + item.MAST_ER_ROAD_CODE.ToString().Trim()})+"'); return false;'></a>"
                        
                    }
                }).ToArray();
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "ListExistingRoadsPMGSY3DAL()");
                totalRecords = 0;
                return null;
            }
            finally
            {
                dbContext.Dispose();
            }
        }




        public Boolean DeletePCIDAL(int _roadCode, ref string message)
        {
            try
            {
                dbContext = new PMGSYEntities();
                int cnRoadCode = 0;

                using (TransactionScope ts = new TransactionScope())
                {
                    #region Validation for Non Deletion

                    string message1 = String.Empty;
                    if (!CheckIfUsedinSanctionedProjects(_roadCode, ref message1))
                    {
                        message = message1;
                        return false;
                    }


                    if (!CheckIfUsedinPlanRoad(_roadCode, ref message1))
                    {
                        message = message1;
                        return false;
                    }


                    #endregion

                    #region Deletion From Tables

                    //List<CUPL_PMGSY3> masterModel1 = new List<CUPL_PMGSY3>();
                    //if (dbContext.CUPL_PMGSY3.Where(m => m.MAST_ER_ROAD_CODE == _roadCode).Any())
                    //{
                    //    masterModel1 = dbContext.CUPL_PMGSY3.Where(m => m.MAST_ER_ROAD_CODE == _roadCode).ToList();
                    //    dbContext.CUPL_PMGSY3.DeleteMany(masterModel1);
                    //    dbContext.SaveChanges();
                    //}

                    List<IMS_LOCK_DETAILS> masterModel2 = new List<IMS_LOCK_DETAILS>();
                    if (dbContext.IMS_LOCK_DETAILS.Where(m => m.MAST_ER_ROAD_CODE == _roadCode).Any())
                    {
                        masterModel2 = dbContext.IMS_LOCK_DETAILS.Where(m => m.MAST_ER_ROAD_CODE == _roadCode).ToList();
                        dbContext.IMS_LOCK_DETAILS.DeleteMany(masterModel2);
                        dbContext.SaveChanges();
                    }

                    List<IMS_UNLOCK_DETAILS> masterModel3 = new List<IMS_UNLOCK_DETAILS>();
                    if (dbContext.IMS_UNLOCK_DETAILS.Where(m => m.MAST_ER_ROAD_CODE == _roadCode).Any())
                    {
                        masterModel3 = dbContext.IMS_UNLOCK_DETAILS.Where(m => m.MAST_ER_ROAD_CODE == _roadCode).ToList();
                        dbContext.IMS_UNLOCK_DETAILS.DeleteMany(masterModel3);
                        dbContext.SaveChanges();
                    }



                    List<MANE_PCI_IMAGE_MAPPING_PMGSY3> listSubDetailsPIC = new List<MANE_PCI_IMAGE_MAPPING_PMGSY3>();
                    List<MANE_CN_PCI_INDEX_PMGSY3> listMasterPCI = new List<MANE_CN_PCI_INDEX_PMGSY3>();
                    if (dbContext.MANE_CN_PCI_INDEX_PMGSY3.Where(m => m.MAST_ER_ROAD_CODE == _roadCode).Any())
                    {
                        listMasterPCI = dbContext.MANE_CN_PCI_INDEX_PMGSY3.Where(m => m.MAST_ER_ROAD_CODE == _roadCode).ToList();

                        foreach (var masterPciItem in listMasterPCI)
                        {
                            listSubDetailsPIC = dbContext.MANE_PCI_IMAGE_MAPPING_PMGSY3.Where(m => m.PCI_ID == masterPciItem.PCI_ID).ToList();
                            if (listSubDetailsPIC.Count > 0)
                            {
                                dbContext.MANE_PCI_IMAGE_MAPPING_PMGSY3.DeleteMany(listSubDetailsPIC);
                                dbContext.SaveChanges();
                            }
                        }

                    }


                    List<MANE_CN_PCI_INDEX_PMGSY3> masterModel4 = new List<MANE_CN_PCI_INDEX_PMGSY3>();
                    if (dbContext.MANE_CN_PCI_INDEX_PMGSY3.Where(m => m.MAST_ER_ROAD_CODE == _roadCode).Any())
                    {
                        masterModel4 = dbContext.MANE_CN_PCI_INDEX_PMGSY3.Where(m => m.MAST_ER_ROAD_CODE == _roadCode).ToList();
                        dbContext.MANE_CN_PCI_INDEX_PMGSY3.DeleteMany(masterModel4);
                        dbContext.SaveChanges();
                    }

                    List<MANE_ER_PCI_INDEX> masterModel5 = new List<MANE_ER_PCI_INDEX>();
                    if (dbContext.MANE_ER_PCI_INDEX.Where(m => m.MAST_ER_ROAD_CODE == _roadCode).Any())
                    {
                        masterModel5 = dbContext.MANE_ER_PCI_INDEX.Where(m => m.MAST_ER_ROAD_CODE == _roadCode).ToList();
                        dbContext.MANE_ER_PCI_INDEX.DeleteMany(masterModel5);
                        dbContext.SaveChanges();
                    }



                    #endregion

                    ts.Complete();
                    return true;
                }

            }
            catch (System.Data.Entity.Infrastructure.DbUpdateException ex)
            {
                ErrorLog.LogError(ex, "DeletePCIDAL().DbUpdateException");
                message = "Existing Roads details can not be deleted because other details for road are entered. DeletePCIDAL()";
                return false;
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "DeletePCIDAL()");
                message = "An Error Occurred While Your Processing Request.DeletePCIDAL()";
                return false;
            }
            finally
            {
                dbContext.Dispose();
            }
        }

        public Boolean DeleteCBRDAL(int _roadCode, ref string message)
        {
            try
            {
                dbContext = new PMGSYEntities();
                int cnRoadCode = 0;

                using (TransactionScope ts = new TransactionScope())
                {
                    #region Validation for Non Deletion

                    string message1 = String.Empty;
                    if (!CheckIfUsedinSanctionedProjects(_roadCode, ref message1))
                    {
                        message = message1;
                        return false;
                    }


                    if (!CheckIfUsedinPlanRoad(_roadCode, ref message1))
                    {
                        message = message1;
                        return false;
                    }


                    #endregion

                    #region Deletion From Tables





                    List<MAST_TRACE_DRRP_SCORE_PMGSY3> masterModel6 = new List<MAST_TRACE_DRRP_SCORE_PMGSY3>();
                    if (dbContext.MAST_TRACE_DRRP_SCORE_PMGSY3.Where(m => m.MAST_ER_ROAD_CODE == _roadCode).Any())
                    {
                        masterModel6 = dbContext.MAST_TRACE_DRRP_SCORE_PMGSY3.Where(m => m.MAST_ER_ROAD_CODE == _roadCode).ToList();
                        dbContext.MAST_TRACE_DRRP_SCORE_PMGSY3.DeleteMany(masterModel6);
                        dbContext.SaveChanges();
                    }

                    List<MASTER_ER_CBR_VALUE> masterModel7 = new List<MASTER_ER_CBR_VALUE>();
                    if (dbContext.MASTER_ER_CBR_VALUE.Where(m => m.MAST_ER_ROAD_CODE == _roadCode).Any())
                    {
                        masterModel7 = dbContext.MASTER_ER_CBR_VALUE.Where(m => m.MAST_ER_ROAD_CODE == _roadCode).ToList();
                        dbContext.MASTER_ER_CBR_VALUE.DeleteMany(masterModel7);
                        dbContext.SaveChanges();
                    }


                    List<MASTER_ER_CBR_VALUE_PMGSY3> masterModel8 = new List<MASTER_ER_CBR_VALUE_PMGSY3>();
                    if (dbContext.MASTER_ER_CBR_VALUE_PMGSY3.Where(m => m.MAST_ER_ROAD_CODE == _roadCode).Any())
                    {
                        masterModel8 = dbContext.MASTER_ER_CBR_VALUE_PMGSY3.Where(m => m.MAST_ER_ROAD_CODE == _roadCode).ToList();
                        dbContext.MASTER_ER_CBR_VALUE_PMGSY3.DeleteMany(masterModel8);
                        dbContext.SaveChanges();
                    }




                    #endregion

                    ts.Complete();
                    return true;
                }

            }
            catch (System.Data.Entity.Infrastructure.DbUpdateException ex)
            {
                ErrorLog.LogError(ex, "DeleteCBRDAL().DbUpdateException");
                message = "Existing Roads details can not be deleted because other details for road are entered. DeleteCBRDAL()";
                return false;
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "DeleteCBRDAL()");
                message = "An Error Occurred While Your Processing Request. DeleteCBRDAL()";
                return false;
            }
            finally
            {
                dbContext.Dispose();
            }
        }


        public Boolean DeleteCDWorksDAL(int _roadCode, ref string message)
        {
            try
            {
                dbContext = new PMGSYEntities();
                int cnRoadCode = 0;

                using (TransactionScope ts = new TransactionScope())
                {
                    #region Validation for Non Deletion

                    string message1 = String.Empty;
                    if (!CheckIfUsedinSanctionedProjects(_roadCode, ref message1))
                    {
                        message = message1;
                        return false;
                    }


                    if (!CheckIfUsedinPlanRoad(_roadCode, ref message1))
                    {
                        message = message1;
                        return false;
                    }


                    #endregion


                    #region Deletion From Tables

                    List<MASTER_ER_CDWORKS_ROAD> masterModel9 = new List<MASTER_ER_CDWORKS_ROAD>();
                    if (dbContext.MASTER_ER_CDWORKS_ROAD.Where(m => m.MAST_ER_ROAD_CODE == _roadCode).Any())
                    {
                        masterModel9 = dbContext.MASTER_ER_CDWORKS_ROAD.Where(m => m.MAST_ER_ROAD_CODE == _roadCode).ToList();
                        dbContext.MASTER_ER_CDWORKS_ROAD.DeleteMany(masterModel9);
                        dbContext.SaveChanges();
                    }

                    List<MASTER_ER_CDWORKS_ROAD_PMGSY3> masterModel10 = new List<MASTER_ER_CDWORKS_ROAD_PMGSY3>();
                    if (dbContext.MASTER_ER_CDWORKS_ROAD_PMGSY3.Where(m => m.MAST_ER_ROAD_CODE == _roadCode).Any())
                    {
                        masterModel10 = dbContext.MASTER_ER_CDWORKS_ROAD_PMGSY3.Where(m => m.MAST_ER_ROAD_CODE == _roadCode).ToList();
                        dbContext.MASTER_ER_CDWORKS_ROAD_PMGSY3.DeleteMany(masterModel10);
                        dbContext.SaveChanges();
                    }


                    #endregion

                    ts.Complete();
                    return true;
                }

            }
            catch (System.Data.Entity.Infrastructure.DbUpdateException ex)
            {
                ErrorLog.LogError(ex, "DeleteCDWorksDAL().DbUpdateException");
                message = "Existing Roads details can not be deleted because other details for road are entered. DeleteCDWorksDAL()";
                return false;
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "DeleteCDWorksDAL()");
                message = "An Error Occurred While Your Processing Request. DeleteCDWorksDAL()";
                return false;
            }
            finally
            {
                dbContext.Dispose();
            }
        }


        public Boolean DeleteHabitationsDAL(int _roadCode, ref string message)
        {
            try
            {
                dbContext = new PMGSYEntities();
                int cnRoadCode = 0;

                using (TransactionScope ts = new TransactionScope())
                {
                    #region Validation for Non Deletion

                    string message1 = String.Empty;
                    if (!CheckIfUsedinSanctionedProjects(_roadCode, ref message1))
                    {
                        message = message1;
                        return false;
                    }


                    if (!CheckIfUsedinPlanRoad(_roadCode, ref message1))
                    {
                        message = message1;
                        return false;
                    }


                    #endregion

                    #region Deletion From Tables

                    List<MASTER_ER_HABITATION_ROAD> masterModel11 = new List<MASTER_ER_HABITATION_ROAD>();
                    if (dbContext.MASTER_ER_HABITATION_ROAD.Where(m => m.MAST_ER_ROAD_CODE == _roadCode).Any())
                    {
                        masterModel11 = dbContext.MASTER_ER_HABITATION_ROAD.Where(m => m.MAST_ER_ROAD_CODE == _roadCode).ToList();
                        dbContext.MASTER_ER_HABITATION_ROAD.DeleteMany(masterModel11);
                        dbContext.SaveChanges();
                    }

                    List<MASTER_ER_HABITATION_ROAD_PMGSY3> masterModel12 = new List<MASTER_ER_HABITATION_ROAD_PMGSY3>();
                    if (dbContext.MASTER_ER_HABITATION_ROAD_PMGSY3.Where(m => m.MAST_ER_ROAD_CODE == _roadCode).Any())
                    {
                        masterModel12 = dbContext.MASTER_ER_HABITATION_ROAD_PMGSY3.Where(m => m.MAST_ER_ROAD_CODE == _roadCode).ToList();
                        dbContext.MASTER_ER_HABITATION_ROAD_PMGSY3.DeleteMany(masterModel12);
                        dbContext.SaveChanges();
                    }

                    List<MASTER_ER_MAPROAD_PMGSY3> masterModel13 = new List<MASTER_ER_MAPROAD_PMGSY3>();
                    if (dbContext.MASTER_ER_MAPROAD_PMGSY3.Where(m => m.MAST_ER_ROAD_CODE == _roadCode).Any())
                    {
                        masterModel13 = dbContext.MASTER_ER_MAPROAD_PMGSY3.Where(m => m.MAST_ER_ROAD_CODE == _roadCode).ToList();
                        dbContext.MASTER_ER_MAPROAD_PMGSY3.DeleteMany(masterModel13);
                        dbContext.SaveChanges();
                    }



                    #endregion

                    ts.Complete();
                    return true;
                }

            }
            catch (System.Data.Entity.Infrastructure.DbUpdateException ex)
            {
                ErrorLog.LogError(ex, "DeleteHabitationsDAL().DbUpdateException");
                message = "Existing Roads details can not be deleted because other details for road are entered. DeleteHabitationsDAL()";
                return false;
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "DeleteHabitationsDAL()");
                message = "An Error Occurred While Your Processing Request. DeleteHabitationsDAL()";
                return false;
            }
            finally
            {
                dbContext.Dispose();
            }
        }


        public Boolean DeleteSurfaceTypesDAL(int _roadCode, ref string message)
        {
            try
            {
                dbContext = new PMGSYEntities();
                int cnRoadCode = 0;

                using (TransactionScope ts = new TransactionScope())
                {
                    #region Validation for Non Deletion

                    string message1 = String.Empty;
                    if (!CheckIfUsedinSanctionedProjects(_roadCode, ref message1))
                    {
                        message = message1;
                        return false;
                    }


                    if (!CheckIfUsedinPlanRoad(_roadCode, ref message1))
                    {
                        message = message1;
                        return false;
                    }


                    #endregion


                    #region Deletion From Tables
                    List<MASTER_ER_SURFACE_TYPES> masterModel14 = new List<MASTER_ER_SURFACE_TYPES>();
                    if (dbContext.MASTER_ER_SURFACE_TYPES.Where(m => m.MAST_ER_ROAD_CODE == _roadCode).Any())
                    {
                        masterModel14 = dbContext.MASTER_ER_SURFACE_TYPES.Where(m => m.MAST_ER_ROAD_CODE == _roadCode).ToList();
                        dbContext.MASTER_ER_SURFACE_TYPES.DeleteMany(masterModel14);
                        dbContext.SaveChanges();
                    }

                    List<MASTER_ER_SURFACE_TYPES_PMGSY3> masterModel15 = new List<MASTER_ER_SURFACE_TYPES_PMGSY3>();
                    if (dbContext.MASTER_ER_SURFACE_TYPES_PMGSY3.Where(m => m.MAST_ER_ROAD_CODE == _roadCode).Any())
                    {
                        masterModel15 = dbContext.MASTER_ER_SURFACE_TYPES_PMGSY3.Where(m => m.MAST_ER_ROAD_CODE == _roadCode).ToList();
                        dbContext.MASTER_ER_SURFACE_TYPES_PMGSY3.DeleteMany(masterModel15);
                        dbContext.SaveChanges();
                    }



                    #endregion

                    ts.Complete();
                    return true;
                }

            }
            catch (System.Data.Entity.Infrastructure.DbUpdateException ex)
            {
                ErrorLog.LogError(ex, "DeleteSurfaceTypesDAL().DbUpdateException");
                message = "Existing Roads details can not be deleted because other details for road are entered. DeleteSurfaceTypesDAL()";
                return false;
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "DeleteSurfaceTypesDAL()");
                message = "An Error Occurred While Your Processing Request. DeleteSurfaceTypesDAL()";
                return false;
            }
            finally
            {
                dbContext.Dispose();
            }
        }

        public Boolean DeleteTrafficeIntensityDAL(int _roadCode, ref string message)
        {
            try
            {
                dbContext = new PMGSYEntities();
                int cnRoadCode = 0;

                using (TransactionScope ts = new TransactionScope())
                {
                    #region Validation for Non Deletion

                    string message1 = String.Empty;
                    if (!CheckIfUsedinSanctionedProjects(_roadCode, ref message1))
                    {
                        message = message1;
                        return false;
                    }


                    if (!CheckIfUsedinPlanRoad(_roadCode, ref message1))
                    {
                        message = message1;
                        return false;
                    }


                    #endregion

                    #region Deletion From Tables


                    List<MASTER_ER_TRAFFIC_INTENSITY> masterModel16 = new List<MASTER_ER_TRAFFIC_INTENSITY>();
                    if (dbContext.MASTER_ER_TRAFFIC_INTENSITY.Where(m => m.MAST_ER_ROAD_CODE == _roadCode).Any())
                    {
                        masterModel16 = dbContext.MASTER_ER_TRAFFIC_INTENSITY.Where(m => m.MAST_ER_ROAD_CODE == _roadCode).ToList();
                        dbContext.MASTER_ER_TRAFFIC_INTENSITY.DeleteMany(masterModel16);
                        dbContext.SaveChanges();
                    }

                    List<MASTER_ER_TRAFFIC_INTENSITY_PMGSY3> masterModel17 = new List<MASTER_ER_TRAFFIC_INTENSITY_PMGSY3>();
                    if (dbContext.MASTER_ER_TRAFFIC_INTENSITY_PMGSY3.Where(m => m.MAST_ER_ROAD_CODE == _roadCode).Any())
                    {
                        masterModel17 = dbContext.MASTER_ER_TRAFFIC_INTENSITY_PMGSY3.Where(m => m.MAST_ER_ROAD_CODE == _roadCode).ToList();
                        dbContext.MASTER_ER_TRAFFIC_INTENSITY_PMGSY3.DeleteMany(masterModel17);
                        dbContext.SaveChanges();
                    }



                    #endregion

                    ts.Complete();
                    return true;
                }

            }
            catch (System.Data.Entity.Infrastructure.DbUpdateException ex)
            {
                ErrorLog.LogError(ex, "DeleteTrafficeIntensityDAL().DbUpdateException");
                message = "Existing Roads details can not be deleted because other details for road are entered. DeleteTrafficeIntensityDAL()";
                return false;
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "DeleteTrafficeIntensityDAL()");
                message = "An Error Occurred While Your Processing Request. DeleteTrafficeIntensityDAL()";
                return false;
            }
            finally
            {
                dbContext.Dispose();
            }
        }


        // Not Used Currently.
        public Boolean DeleteDRRPandMRLDAL(int _roadCode, ref string message)
        {
            try
            {
                dbContext = new PMGSYEntities();
                int cnRoadCode = 0;

                using (TransactionScope ts = new TransactionScope())
                {
                    #region Validation for Non Deletion

                    string message1 = String.Empty;
                    if (!CheckIfUsedinSanctionedProjects(_roadCode, ref message1))
                    {
                        message = message1;
                        return false;
                    }


                    if (!CheckIfUsedinPlanRoad(_roadCode, ref message1))
                    {
                        message = message1;
                        return false;
                    }


                    #endregion

                    #region Deletion From Tables



                    List<PLAN_ROAD_DRRP> masterModel18 = new List<PLAN_ROAD_DRRP>();
                    if (dbContext.PLAN_ROAD_DRRP.Where(m => m.MAST_ER_ROAD_CODE == _roadCode).Any())
                    {
                        masterModel18 = dbContext.PLAN_ROAD_DRRP.Where(m => m.MAST_ER_ROAD_CODE == _roadCode).ToList();
                        dbContext.PLAN_ROAD_DRRP.DeleteMany(masterModel18);
                        dbContext.SaveChanges();
                    }

                    List<PLAN_ROAD_MRL_PMGSY3> masterModel19 = new List<PLAN_ROAD_MRL_PMGSY3>();
                    if (dbContext.PLAN_ROAD_MRL_PMGSY3.Where(m => m.MAST_ER_ROAD_CODE == _roadCode).Any())
                    {
                        masterModel19 = dbContext.PLAN_ROAD_MRL_PMGSY3.Where(m => m.MAST_ER_ROAD_CODE == _roadCode).ToList();
                        dbContext.PLAN_ROAD_MRL_PMGSY3.DeleteMany(masterModel19);
                        dbContext.SaveChanges();
                    }



                    #endregion

                    ts.Complete();
                    return true;
                }

            }
            catch (System.Data.Entity.Infrastructure.DbUpdateException ex)
            {
                ErrorLog.LogError(ex, "DeleteDRRPandMRLDAL().DbUpdateException");
                message = "Existing Roads details can not be deleted because other details for road are entered. DeleteDRRPandMRLDAL()";
                return false;
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "DeleteDRRPandMRLDAL()");
                message = "An Error Occurred While Your Processing Request. DeleteDRRPandMRLDAL()";
                return false;
            }
            finally
            {
                dbContext.Dispose();
            }
        }


        // Main Master Record Deletion.
        public Boolean DeleteExistingRoadsDALDRRPPMGSY3(int _roadCode, ref string message)
        {
            try
            {
                dbContext = new PMGSYEntities();
                int cnRoadCode = 0;
                bool status = false;

                using (TransactionScope ts = new TransactionScope())
                {
                    #region Validation for Non Deletion

                    string message1 = String.Empty;
                    if (!CheckIfUsedinSanctionedProjects(_roadCode, ref message1))
                    {
                        message = message1;
                        return false;
                    }


                    if (!CheckIfUsedinPlanRoad(_roadCode, ref message1))
                    {
                        message = message1;
                        return false;
                    }




                    #endregion

                    #region PCI Validation


                    List<MANE_PCI_IMAGE_MAPPING_PMGSY3> listSubDetailsPIC = new List<MANE_PCI_IMAGE_MAPPING_PMGSY3>();
                    List<MANE_CN_PCI_INDEX_PMGSY3> listMasterPCI = new List<MANE_CN_PCI_INDEX_PMGSY3>();

                    if (dbContext.MANE_CN_PCI_INDEX_PMGSY3.Where(m => m.MAST_ER_ROAD_CODE == _roadCode).Any())
                    {
                        message = "Please delete PCI Details.";
                        return false;
                        listMasterPCI = dbContext.MANE_CN_PCI_INDEX_PMGSY3.Where(m => m.MAST_ER_ROAD_CODE == _roadCode).ToList();

                        foreach (var masterPciItem in listMasterPCI)
                        {
                            listSubDetailsPIC = dbContext.MANE_PCI_IMAGE_MAPPING_PMGSY3.Where(m => m.PCI_ID == masterPciItem.PCI_ID).ToList();
                            if (listSubDetailsPIC.Count > 0)
                            {
                                message = "Please delete PCI Details.";
                                return false;

                            }
                        }
                    }


                    List<MANE_CN_PCI_INDEX_PMGSY3> masterModel4 = new List<MANE_CN_PCI_INDEX_PMGSY3>();
                    if (dbContext.MANE_CN_PCI_INDEX_PMGSY3.Where(m => m.MAST_ER_ROAD_CODE == _roadCode).Any())
                    {
                        message = "Please delete PCI Details.";
                        return false;

                    }

                    List<MANE_ER_PCI_INDEX> masterModel5 = new List<MANE_ER_PCI_INDEX>();
                    if (dbContext.MANE_ER_PCI_INDEX.Where(m => m.MAST_ER_ROAD_CODE == _roadCode).Any())
                    {
                        message = "Please delete PCI Details.";
                        return false;
                    }


                    List<CUPL_PMGSY3> masterModel1 = new List<CUPL_PMGSY3>();
                    if (dbContext.CUPL_PMGSY3.Where(m => m.MAST_ER_ROAD_CODE == _roadCode).Any())
                    {
                        message = "Please delete PCI Details.";
                        return false;
                    }

                    List<IMS_LOCK_DETAILS> masterModel2 = new List<IMS_LOCK_DETAILS>();
                    if (dbContext.IMS_LOCK_DETAILS.Where(m => m.MAST_ER_ROAD_CODE == _roadCode).Any())
                    {
                        message = "Please delete PCI Details.";
                        return false;
                    }

                    List<IMS_UNLOCK_DETAILS> masterModel3 = new List<IMS_UNLOCK_DETAILS>();
                    if (dbContext.IMS_UNLOCK_DETAILS.Where(m => m.MAST_ER_ROAD_CODE == _roadCode).Any())
                    {
                        message = "Please delete PCI Details.";
                        return false;
                    }

                    #endregion

                    #region  CBR Validation
                    List<MAST_TRACE_DRRP_SCORE_PMGSY3> masterModel6 = new List<MAST_TRACE_DRRP_SCORE_PMGSY3>();
                    if (dbContext.MAST_TRACE_DRRP_SCORE_PMGSY3.Where(m => m.MAST_ER_ROAD_CODE == _roadCode).Any())
                    {
                        message = "Please delete CBR Details.";
                        return false;
                    }

                    List<MASTER_ER_CBR_VALUE> masterModel7 = new List<MASTER_ER_CBR_VALUE>();
                    if (dbContext.MASTER_ER_CBR_VALUE.Where(m => m.MAST_ER_ROAD_CODE == _roadCode).Any())
                    {
                        message = "Please delete CBR Details.";
                        return false;
                    }


                    List<MASTER_ER_CBR_VALUE_PMGSY3> masterModel8 = new List<MASTER_ER_CBR_VALUE_PMGSY3>();
                    if (dbContext.MASTER_ER_CBR_VALUE_PMGSY3.Where(m => m.MAST_ER_ROAD_CODE == _roadCode).Any())
                    {
                        message = "Please delete CBR Details.";
                        return false;
                    }


                    #endregion

                    #region  CD Works Validation


                    List<MASTER_ER_CDWORKS_ROAD> masterModel9 = new List<MASTER_ER_CDWORKS_ROAD>();
                    if (dbContext.MASTER_ER_CDWORKS_ROAD.Where(m => m.MAST_ER_ROAD_CODE == _roadCode).Any())
                    {
                        message = "Please delete CD Works Details.";
                        return false;
                    }

                    List<MASTER_ER_CDWORKS_ROAD_PMGSY3> masterModel10 = new List<MASTER_ER_CDWORKS_ROAD_PMGSY3>();
                    if (dbContext.MASTER_ER_CDWORKS_ROAD_PMGSY3.Where(m => m.MAST_ER_ROAD_CODE == _roadCode).Any())
                    {
                        message = "Please delete CD Works Details.";
                        return false;
                    }
                    #endregion

                    #region  Habitation Validation

                    List<MASTER_ER_HABITATION_ROAD> masterModel11 = new List<MASTER_ER_HABITATION_ROAD>();
                    if (dbContext.MASTER_ER_HABITATION_ROAD.Where(m => m.MAST_ER_ROAD_CODE == _roadCode).Any())
                    {

                        message = "Please delete Habitation Details.";
                        return false;
                    }

                    List<MASTER_ER_HABITATION_ROAD_PMGSY3> masterModel12 = new List<MASTER_ER_HABITATION_ROAD_PMGSY3>();
                    if (dbContext.MASTER_ER_HABITATION_ROAD_PMGSY3.Where(m => m.MAST_ER_ROAD_CODE == _roadCode).Any())
                    {
                        message = "Please delete Habitation Details.";
                        return false;
                    }

                    List<MASTER_ER_MAPROAD_PMGSY3> masterModel13 = new List<MASTER_ER_MAPROAD_PMGSY3>();
                    if (dbContext.MASTER_ER_MAPROAD_PMGSY3.Where(m => m.MAST_ER_ROAD_CODE == _roadCode).Any())
                    {
                        message = "Please delete Habitation Details.";
                        return false;
                    }

                    #endregion

                    #region  Surface Types Validation
                    List<MASTER_ER_SURFACE_TYPES> masterModel14 = new List<MASTER_ER_SURFACE_TYPES>();
                    if (dbContext.MASTER_ER_SURFACE_TYPES.Where(m => m.MAST_ER_ROAD_CODE == _roadCode).Any())
                    {
                        message = "Please delete Surface Types Details.";
                        return false;
                    }

                    List<MASTER_ER_SURFACE_TYPES_PMGSY3> masterModel15 = new List<MASTER_ER_SURFACE_TYPES_PMGSY3>();
                    if (dbContext.MASTER_ER_SURFACE_TYPES_PMGSY3.Where(m => m.MAST_ER_ROAD_CODE == _roadCode).Any())
                    {
                        message = "Please delete Surface Types Details.";
                        return false;
                    }

                    #endregion

                    #region  Traffice Intensity Validation
                    List<MASTER_ER_TRAFFIC_INTENSITY> masterModel16 = new List<MASTER_ER_TRAFFIC_INTENSITY>();
                    if (dbContext.MASTER_ER_TRAFFIC_INTENSITY.Where(m => m.MAST_ER_ROAD_CODE == _roadCode).Any())
                    {
                        message = "Please delete Traffice Intensity Details.";
                        return false;
                    }

                    List<MASTER_ER_TRAFFIC_INTENSITY_PMGSY3> masterModel17 = new List<MASTER_ER_TRAFFIC_INTENSITY_PMGSY3>();
                    if (dbContext.MASTER_ER_TRAFFIC_INTENSITY_PMGSY3.Where(m => m.MAST_ER_ROAD_CODE == _roadCode).Any())
                    {
                        message = "Please delete Traffice Intensity Details.";
                        return false;
                    }
                    #endregion

                    #region  DRRP / MRL  Validation
                    List<PLAN_ROAD_DRRP> masterModel18 = new List<PLAN_ROAD_DRRP>();
                    if (dbContext.PLAN_ROAD_DRRP.Where(m => m.MAST_ER_ROAD_CODE == _roadCode).Any())
                    {
                        message = "Please delete DRRP / MRL  Details.";
                        return false;
                    }

                    List<PLAN_ROAD_MRL_PMGSY3> masterModel19 = new List<PLAN_ROAD_MRL_PMGSY3>();
                    if (dbContext.PLAN_ROAD_MRL_PMGSY3.Where(m => m.MAST_ER_ROAD_CODE == _roadCode).Any())
                    {
                        message = "Please delete DRRP / MRL  Details.";
                        return false;
                    }

                    #endregion

                    List<MASTER_EXISTING_ROADS> masterModel20 = new List<MASTER_EXISTING_ROADS>();
                    if (dbContext.MASTER_EXISTING_ROADS.Where(m => m.MAST_ER_ROAD_CODE == _roadCode).Any())
                    {
                        masterModel20 = dbContext.MASTER_EXISTING_ROADS.Where(m => m.MAST_ER_ROAD_CODE == _roadCode).ToList();
                        dbContext.MASTER_EXISTING_ROADS.DeleteMany(masterModel20);
                        dbContext.SaveChanges();
                    }

                    ts.Complete();
                    return true;

                    #region Commented Code

                    //List<CUPL_PMGSY3> masterModel1 = new List<CUPL_PMGSY3>();
                    //if (dbContext.CUPL_PMGSY3.Where(m => m.MAST_ER_ROAD_CODE == _roadCode).Any())
                    //{
                    //    masterModel1 = dbContext.CUPL_PMGSY3.Where(m => m.MAST_ER_ROAD_CODE == _roadCode).ToList();
                    //    dbContext.CUPL_PMGSY3.DeleteMany(masterModel1);
                    //    dbContext.SaveChanges();
                    //}

                    //List<IMS_LOCK_DETAILS> masterModel2 = new List<IMS_LOCK_DETAILS>();
                    //if (dbContext.IMS_LOCK_DETAILS.Where(m => m.MAST_ER_ROAD_CODE == _roadCode).Any())
                    //{
                    //    masterModel2 = dbContext.IMS_LOCK_DETAILS.Where(m => m.MAST_ER_ROAD_CODE == _roadCode).ToList();
                    //    dbContext.IMS_LOCK_DETAILS.DeleteMany(masterModel2);
                    //    dbContext.SaveChanges();
                    //}

                    //List<IMS_UNLOCK_DETAILS> masterModel3 = new List<IMS_UNLOCK_DETAILS>();
                    //if (dbContext.IMS_UNLOCK_DETAILS.Where(m => m.MAST_ER_ROAD_CODE == _roadCode).Any())
                    //{
                    //    masterModel3 = dbContext.IMS_UNLOCK_DETAILS.Where(m => m.MAST_ER_ROAD_CODE == _roadCode).ToList();
                    //    dbContext.IMS_UNLOCK_DETAILS.DeleteMany(masterModel3);
                    //    dbContext.SaveChanges();
                    //}



                    //List<MANE_PCI_IMAGE_MAPPING_PMGSY3> listSubDetailsPIC = new List<MANE_PCI_IMAGE_MAPPING_PMGSY3>();
                    //List<MANE_CN_PCI_INDEX_PMGSY3> listMasterPCI = new List<MANE_CN_PCI_INDEX_PMGSY3>();
                    //if (dbContext.MANE_CN_PCI_INDEX_PMGSY3.Where(m => m.MAST_ER_ROAD_CODE == _roadCode).Any())
                    //{
                    //    listMasterPCI = dbContext.MANE_CN_PCI_INDEX_PMGSY3.Where(m => m.MAST_ER_ROAD_CODE == _roadCode).ToList();

                    //    foreach (var masterPciItem in listMasterPCI)
                    //    {
                    //        listSubDetailsPIC = dbContext.MANE_PCI_IMAGE_MAPPING_PMGSY3.Where(m => m.PCI_ID == masterPciItem.PCI_ID).ToList();
                    //        if (listSubDetailsPIC.Count > 0)
                    //        {
                    //            dbContext.MANE_PCI_IMAGE_MAPPING_PMGSY3.DeleteMany(listSubDetailsPIC);
                    //            dbContext.SaveChanges();
                    //        }
                    //    }

                    //}


                    //List<MANE_CN_PCI_INDEX_PMGSY3> masterModel4 = new List<MANE_CN_PCI_INDEX_PMGSY3>();
                    //if (dbContext.MANE_CN_PCI_INDEX_PMGSY3.Where(m => m.MAST_ER_ROAD_CODE == _roadCode).Any())
                    //{
                    //    masterModel4 = dbContext.MANE_CN_PCI_INDEX_PMGSY3.Where(m => m.MAST_ER_ROAD_CODE == _roadCode).ToList();
                    //    dbContext.MANE_CN_PCI_INDEX_PMGSY3.DeleteMany(masterModel4);
                    //    dbContext.SaveChanges();
                    //}

                    //List<MANE_ER_PCI_INDEX> masterModel5 = new List<MANE_ER_PCI_INDEX>();
                    //if (dbContext.MANE_ER_PCI_INDEX.Where(m => m.MAST_ER_ROAD_CODE == _roadCode).Any())
                    //{
                    //    masterModel5 = dbContext.MANE_ER_PCI_INDEX.Where(m => m.MAST_ER_ROAD_CODE == _roadCode).ToList();
                    //    dbContext.MANE_ER_PCI_INDEX.DeleteMany(masterModel5);
                    //    dbContext.SaveChanges();
                    //}

                    //List<MAST_TRACE_DRRP_SCORE_PMGSY3> masterModel6 = new List<MAST_TRACE_DRRP_SCORE_PMGSY3>();
                    //if (dbContext.MAST_TRACE_DRRP_SCORE_PMGSY3.Where(m => m.MAST_ER_ROAD_CODE == _roadCode).Any())
                    //{
                    //    masterModel6 = dbContext.MAST_TRACE_DRRP_SCORE_PMGSY3.Where(m => m.MAST_ER_ROAD_CODE == _roadCode).ToList();
                    //    dbContext.MAST_TRACE_DRRP_SCORE_PMGSY3.DeleteMany(masterModel6);
                    //    dbContext.SaveChanges();
                    //}

                    //List<MASTER_ER_CBR_VALUE> masterModel7 = new List<MASTER_ER_CBR_VALUE>();
                    //if (dbContext.MASTER_ER_CBR_VALUE.Where(m => m.MAST_ER_ROAD_CODE == _roadCode).Any())
                    //{
                    //    masterModel7 = dbContext.MASTER_ER_CBR_VALUE.Where(m => m.MAST_ER_ROAD_CODE == _roadCode).ToList();
                    //    dbContext.MASTER_ER_CBR_VALUE.DeleteMany(masterModel7);
                    //    dbContext.SaveChanges();
                    //}


                    //List<MASTER_ER_CBR_VALUE_PMGSY3> masterModel8 = new List<MASTER_ER_CBR_VALUE_PMGSY3>();
                    //if (dbContext.MASTER_ER_CBR_VALUE_PMGSY3.Where(m => m.MAST_ER_ROAD_CODE == _roadCode).Any())
                    //{
                    //    masterModel8 = dbContext.MASTER_ER_CBR_VALUE_PMGSY3.Where(m => m.MAST_ER_ROAD_CODE == _roadCode).ToList();
                    //    dbContext.MASTER_ER_CBR_VALUE_PMGSY3.DeleteMany(masterModel8);
                    //    dbContext.SaveChanges();
                    //}

                    //List<MASTER_ER_CDWORKS_ROAD> masterModel9 = new List<MASTER_ER_CDWORKS_ROAD>();
                    //if (dbContext.MASTER_ER_CDWORKS_ROAD.Where(m => m.MAST_ER_ROAD_CODE == _roadCode).Any())
                    //{
                    //    masterModel9 = dbContext.MASTER_ER_CDWORKS_ROAD.Where(m => m.MAST_ER_ROAD_CODE == _roadCode).ToList();
                    //    dbContext.MASTER_ER_CDWORKS_ROAD.DeleteMany(masterModel9);
                    //    dbContext.SaveChanges();
                    //}

                    //List<MASTER_ER_CDWORKS_ROAD_PMGSY3> masterModel10 = new List<MASTER_ER_CDWORKS_ROAD_PMGSY3>();
                    //if (dbContext.MASTER_ER_CDWORKS_ROAD_PMGSY3.Where(m => m.MAST_ER_ROAD_CODE == _roadCode).Any())
                    //{
                    //    masterModel10 = dbContext.MASTER_ER_CDWORKS_ROAD_PMGSY3.Where(m => m.MAST_ER_ROAD_CODE == _roadCode).ToList();
                    //    dbContext.MASTER_ER_CDWORKS_ROAD_PMGSY3.DeleteMany(masterModel10);
                    //    dbContext.SaveChanges();
                    //}

                    //List<MASTER_ER_HABITATION_ROAD> masterModel11 = new List<MASTER_ER_HABITATION_ROAD>();
                    //if (dbContext.MASTER_ER_HABITATION_ROAD.Where(m => m.MAST_ER_ROAD_CODE == _roadCode).Any())
                    //{
                    //    masterModel11 = dbContext.MASTER_ER_HABITATION_ROAD.Where(m => m.MAST_ER_ROAD_CODE == _roadCode).ToList();
                    //    dbContext.MASTER_ER_HABITATION_ROAD.DeleteMany(masterModel11);
                    //    dbContext.SaveChanges();
                    //}

                    //List<MASTER_ER_HABITATION_ROAD_PMGSY3> masterModel12 = new List<MASTER_ER_HABITATION_ROAD_PMGSY3>();
                    //if (dbContext.MASTER_ER_HABITATION_ROAD_PMGSY3.Where(m => m.MAST_ER_ROAD_CODE == _roadCode).Any())
                    //{
                    //    masterModel12 = dbContext.MASTER_ER_HABITATION_ROAD_PMGSY3.Where(m => m.MAST_ER_ROAD_CODE == _roadCode).ToList();
                    //    dbContext.MASTER_ER_HABITATION_ROAD_PMGSY3.DeleteMany(masterModel12);
                    //    dbContext.SaveChanges();
                    //}

                    //List<MASTER_ER_MAPROAD_PMGSY3> masterModel13 = new List<MASTER_ER_MAPROAD_PMGSY3>();
                    //if (dbContext.MASTER_ER_MAPROAD_PMGSY3.Where(m => m.MAST_ER_ROAD_CODE == _roadCode).Any())
                    //{
                    //    masterModel13 = dbContext.MASTER_ER_MAPROAD_PMGSY3.Where(m => m.MAST_ER_ROAD_CODE == _roadCode).ToList();
                    //    dbContext.MASTER_ER_MAPROAD_PMGSY3.DeleteMany(masterModel13);
                    //    dbContext.SaveChanges();
                    //}


                    //List<MASTER_ER_SURFACE_TYPES> masterModel14 = new List<MASTER_ER_SURFACE_TYPES>();
                    //if (dbContext.MASTER_ER_SURFACE_TYPES.Where(m => m.MAST_ER_ROAD_CODE == _roadCode).Any())
                    //{
                    //    masterModel14 = dbContext.MASTER_ER_SURFACE_TYPES.Where(m => m.MAST_ER_ROAD_CODE == _roadCode).ToList();
                    //    dbContext.MASTER_ER_SURFACE_TYPES.DeleteMany(masterModel14);
                    //    dbContext.SaveChanges();
                    //}

                    //List<MASTER_ER_SURFACE_TYPES_PMGSY3> masterModel15 = new List<MASTER_ER_SURFACE_TYPES_PMGSY3>();
                    //if (dbContext.MASTER_ER_SURFACE_TYPES_PMGSY3.Where(m => m.MAST_ER_ROAD_CODE == _roadCode).Any())
                    //{
                    //    masterModel15 = dbContext.MASTER_ER_SURFACE_TYPES_PMGSY3.Where(m => m.MAST_ER_ROAD_CODE == _roadCode).ToList();
                    //    dbContext.MASTER_ER_SURFACE_TYPES_PMGSY3.DeleteMany(masterModel15);
                    //    dbContext.SaveChanges();
                    //}

                    //List<MASTER_ER_TRAFFIC_INTENSITY> masterModel16 = new List<MASTER_ER_TRAFFIC_INTENSITY>();
                    //if (dbContext.MASTER_ER_TRAFFIC_INTENSITY.Where(m => m.MAST_ER_ROAD_CODE == _roadCode).Any())
                    //{
                    //    masterModel16 = dbContext.MASTER_ER_TRAFFIC_INTENSITY.Where(m => m.MAST_ER_ROAD_CODE == _roadCode).ToList();
                    //    dbContext.MASTER_ER_TRAFFIC_INTENSITY.DeleteMany(masterModel16);
                    //    dbContext.SaveChanges();
                    //}

                    //List<MASTER_ER_TRAFFIC_INTENSITY_PMGSY3> masterModel17 = new List<MASTER_ER_TRAFFIC_INTENSITY_PMGSY3>();
                    //if (dbContext.MASTER_ER_TRAFFIC_INTENSITY_PMGSY3.Where(m => m.MAST_ER_ROAD_CODE == _roadCode).Any())
                    //{
                    //    masterModel17 = dbContext.MASTER_ER_TRAFFIC_INTENSITY_PMGSY3.Where(m => m.MAST_ER_ROAD_CODE == _roadCode).ToList();
                    //    dbContext.MASTER_ER_TRAFFIC_INTENSITY_PMGSY3.DeleteMany(masterModel17);
                    //    dbContext.SaveChanges();
                    //}

                    //List<PLAN_ROAD_DRRP> masterModel18 = new List<PLAN_ROAD_DRRP>();
                    //if (dbContext.PLAN_ROAD_DRRP.Where(m => m.MAST_ER_ROAD_CODE == _roadCode).Any())
                    //{
                    //    masterModel18 = dbContext.PLAN_ROAD_DRRP.Where(m => m.MAST_ER_ROAD_CODE == _roadCode).ToList();
                    //    dbContext.PLAN_ROAD_DRRP.DeleteMany(masterModel18);
                    //    dbContext.SaveChanges();
                    //}

                    //List<PLAN_ROAD_MRL_PMGSY3> masterModel19 = new List<PLAN_ROAD_MRL_PMGSY3>();
                    //if (dbContext.PLAN_ROAD_MRL_PMGSY3.Where(m => m.MAST_ER_ROAD_CODE == _roadCode).Any())
                    //{
                    //    masterModel19 = dbContext.PLAN_ROAD_MRL_PMGSY3.Where(m => m.MAST_ER_ROAD_CODE == _roadCode).ToList();
                    //    dbContext.PLAN_ROAD_MRL_PMGSY3.DeleteMany(masterModel19);
                    //    dbContext.SaveChanges();
                    //}
                    #endregion

                }

            }
            catch (System.Data.Entity.Infrastructure.DbUpdateException ex)
            {
                ErrorLog.LogError(ex, "DeleteExistingRoadsDALDRRPPMGSY3().DbUpdateException");
                message = "Existing Roads details can not be deleted because other details for road are entered.";
                return false;
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "DeleteExistingRoadsDALDRRPPMGSY3()");
                message = "An Error Occurred While Your Processing Request.";
                return false;
            }
            finally
            {
                dbContext.Dispose();
            }
        }


        public Boolean CheckIfUsedinSanctionedProjects(int _roadCode, ref string message)
        {
            try
            {
                dbContext = new PMGSYEntities();
                // int cnRoadCode = 0;

                using (TransactionScope ts = new TransactionScope())
                {
                    #region Validation for Non Deletion



                    if (dbContext.PLAN_ROAD_DRRP.Any(m => m.MAST_ER_ROAD_CODE == _roadCode))
                    {
                        List<PLAN_ROAD_DRRP> planRoaddrrp = dbContext.PLAN_ROAD_DRRP.Where(m => m.MAST_ER_ROAD_CODE == _roadCode).ToList();

                        foreach (var item in planRoaddrrp)
                        {
                            if (dbContext.IMS_SANCTIONED_PROJECTS.Any(m => m.PLAN_CN_ROAD_CODE == item.PLAN_CN_ROAD_CODE))
                            {
                                IMS_SANCTIONED_PROJECTS isp = dbContext.IMS_SANCTIONED_PROJECTS.Where(m => m.PLAN_CN_ROAD_CODE == item.PLAN_CN_ROAD_CODE).FirstOrDefault();
                                PLAN_ROAD planRoadDetails = dbContext.PLAN_ROAD.Where(m => m.PLAN_CN_ROAD_CODE == item.PLAN_CN_ROAD_CODE).FirstOrDefault();

                                message = "This Road is used in Proposal (" + isp.IMS_ROAD_NAME + ", ROADE CODE : " + isp.IMS_PR_ROAD_CODE + " ), ( " + planRoadDetails.PLAN_RD_NAME + ", PLAN CN ROAD CODE : " + planRoadDetails.PLAN_CN_ROAD_CODE + " ). Hence can not be deleted. (omms.PLAN_ROAD_DRRP)";
                                return false;
                            }
                        }
                    }


                    if (dbContext.PLAN_ROAD_MRL_PMGSY3.Any(m => m.MAST_ER_ROAD_CODE == _roadCode))
                    {
                        List<PLAN_ROAD_MRL_PMGSY3> planRoaddrrp = dbContext.PLAN_ROAD_MRL_PMGSY3.Where(m => m.MAST_ER_ROAD_CODE == _roadCode).ToList();

                        foreach (var item in planRoaddrrp)
                        {
                            if (dbContext.IMS_SANCTIONED_PROJECTS.Any(m => m.PLAN_CN_ROAD_CODE == item.PLAN_CN_ROAD_CODE))
                            {
                                IMS_SANCTIONED_PROJECTS isp = dbContext.IMS_SANCTIONED_PROJECTS.Where(m => m.PLAN_CN_ROAD_CODE == item.PLAN_CN_ROAD_CODE).FirstOrDefault();
                                PLAN_ROAD planRoadDetails = dbContext.PLAN_ROAD.Where(m => m.PLAN_CN_ROAD_CODE == item.PLAN_CN_ROAD_CODE).FirstOrDefault();

                                message = "This Road is used in Proposal (" + isp.IMS_ROAD_NAME + ", ROADE CODE : " + isp.IMS_PR_ROAD_CODE + " ), ( " + planRoadDetails.PLAN_RD_NAME + ", PLAN CN ROAD CODE : " + planRoadDetails.PLAN_CN_ROAD_CODE + " ). Hence can not be deleted. (omms.PLAN_ROAD_MRL_PMGSY3)";
                                return false;
                            }
                        }
                    }


                    if (dbContext.PLAN_ROAD.Any(m => m.MAST_ER_ROAD_CODE == _roadCode))
                    {
                        List<PLAN_ROAD> planRoaddrrp = dbContext.PLAN_ROAD.Where(m => m.MAST_ER_ROAD_CODE == _roadCode).ToList();

                        foreach (var item in planRoaddrrp)
                        {
                            if (dbContext.IMS_SANCTIONED_PROJECTS.Any(m => m.PLAN_CN_ROAD_CODE == item.PLAN_CN_ROAD_CODE))
                            {
                                IMS_SANCTIONED_PROJECTS isp = dbContext.IMS_SANCTIONED_PROJECTS.Where(m => m.PLAN_CN_ROAD_CODE == item.PLAN_CN_ROAD_CODE).FirstOrDefault();
                                PLAN_ROAD planRoadDetails = dbContext.PLAN_ROAD.Where(m => m.PLAN_CN_ROAD_CODE == item.PLAN_CN_ROAD_CODE).FirstOrDefault();

                                message = "This Road is used in Proposal (" + isp.IMS_ROAD_NAME + ", ROADE CODE : " + isp.IMS_PR_ROAD_CODE + " ), ( " + planRoadDetails.PLAN_RD_NAME + ", PLAN CN ROAD CODE : " + planRoadDetails.PLAN_CN_ROAD_CODE + " ). Hence can not be deleted. (omms.PLAN_ROAD)";
                                return false;
                            }
                        }
                    }



                    #endregion


                    ts.Complete();
                    message = "Allow to Delete";
                    return true;
                }

            }
            catch (System.Data.Entity.Infrastructure.DbUpdateException ex)
            {
                ErrorLog.LogError(ex, "CheckIfUsedinSanctionedProjects().DbUpdateException");
                message = "Existing Roads details can not be deleted because other details for road are entered. CheckIfUsedinSanctionedProjects()";
                return false;
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "CheckIfUsedinSanctionedProjects()");
                message = "An Error Occurred While Your Processing Request.CheckIfUsedinSanctionedProjects()";
                return false;
            }
            finally
            {
                //   dbContext.Dispose();
            }
        }

        public Boolean CheckIfUsedinPlanRoad(int _roadCode, ref string message)
        {
            try
            {
                dbContext = new PMGSYEntities();
                // int cnRoadCode = 0;

                using (TransactionScope ts = new TransactionScope())
                {
                    #region Validation for Non Deletion



                    if (dbContext.PLAN_ROAD_DRRP.Any(m => m.MAST_ER_ROAD_CODE == _roadCode))
                    {
                        List<PLAN_ROAD_DRRP> planRoaddrrp = dbContext.PLAN_ROAD_DRRP.Where(m => m.MAST_ER_ROAD_CODE == _roadCode).ToList();

                        foreach (var item in planRoaddrrp)
                        {

                            //  IMS_SANCTIONED_PROJECTS isp = dbContext.IMS_SANCTIONED_PROJECTS.Where(m => m.PLAN_CN_ROAD_CODE == item.PLAN_CN_ROAD_CODE).FirstOrDefault();
                            PLAN_ROAD planRoadDetails = dbContext.PLAN_ROAD.Where(m => m.PLAN_CN_ROAD_CODE == item.PLAN_CN_ROAD_CODE).FirstOrDefault();

                            message = "This Road is used in to create TR / MRL :  " + planRoadDetails.PLAN_RD_NAME + ", PLAN CN ROAD CODE : " + planRoadDetails.PLAN_CN_ROAD_CODE + " . Hence can not be deleted. (omms.PLAN_ROAD_DRRP)";
                            return false;

                        }
                    }


                    if (dbContext.PLAN_ROAD_MRL_PMGSY3.Any(m => m.MAST_ER_ROAD_CODE == _roadCode))
                    {
                        List<PLAN_ROAD_MRL_PMGSY3> planRoaddrrp = dbContext.PLAN_ROAD_MRL_PMGSY3.Where(m => m.MAST_ER_ROAD_CODE == _roadCode).ToList();

                        foreach (var item in planRoaddrrp)
                        {
                            PLAN_ROAD planRoadDetails = dbContext.PLAN_ROAD.Where(m => m.PLAN_CN_ROAD_CODE == item.PLAN_CN_ROAD_CODE).FirstOrDefault();

                            message = "This Road is used in to create TR / MRL :  " + planRoadDetails.PLAN_RD_NAME + ", PLAN CN ROAD CODE : " + planRoadDetails.PLAN_CN_ROAD_CODE + " . Hence can not be deleted. (omms.PLAN_ROAD_MRL_PMGSY3)";
                            return false;
                        }
                    }

                    if (dbContext.PLAN_ROAD.Any(m => m.MAST_ER_ROAD_CODE == _roadCode))
                    {
                        List<PLAN_ROAD> planRoaddrrp = dbContext.PLAN_ROAD.Where(m => m.MAST_ER_ROAD_CODE == _roadCode).ToList();

                        foreach (var item in planRoaddrrp)
                        {

                            PLAN_ROAD planRoadDetails = dbContext.PLAN_ROAD.Where(m => m.PLAN_CN_ROAD_CODE == item.PLAN_CN_ROAD_CODE).FirstOrDefault();

                            message = "This Road is used in to create TR / MRL :  " + planRoadDetails.PLAN_RD_NAME + ", PLAN CN ROAD CODE : " + planRoadDetails.PLAN_CN_ROAD_CODE + " . Hence can not be deleted. (omms.PLAN_ROAD_MRL_PMGSY3)";
                            return false;
                        }
                    }



                    List<CUPL_PMGSY3> masterModel1 = new List<CUPL_PMGSY3>();
                    if (dbContext.CUPL_PMGSY3.Where(m => m.MAST_ER_ROAD_CODE == _roadCode).Any())
                    {
                        message = "This Existing Road is used for CUPL generation. Hence it can not be deleted.";
                        return false;
                    }

                    #endregion
                    ts.Complete();
                    message = "Allow to Delete";
                    return true;
                }

            }
            catch (System.Data.Entity.Infrastructure.DbUpdateException ex)
            {
                ErrorLog.LogError(ex, "CheckIfUsedinPlanRoad().DbUpdateException");
                message = "Existing Roads details can not be deleted because other details for road are entered. CheckIfUsedinPlanRoad()";
                return false;
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "CheckIfUsedinPlanRoad()");
                message = "An Error Occurred While Your Processing Request.CheckIfUsedinPlanRoad()";
                return false;
            }
            finally
            {
                //  dbContext.Dispose();
            }
        }




        #region

        public bool IsPCI(int _roadCode)
        {

            try
            {

                dbContext = new PMGSYEntities();
                dbContext.Configuration.AutoDetectChangesEnabled = false;
                using (TransactionScope ts = new TransactionScope())
                {
                    List<IMS_LOCK_DETAILS> masterModel2 = new List<IMS_LOCK_DETAILS>();
                    if (dbContext.IMS_LOCK_DETAILS.Where(m => m.MAST_ER_ROAD_CODE == _roadCode).Any())
                    {
                        return true;
                    }

                    List<IMS_UNLOCK_DETAILS> masterModel3 = new List<IMS_UNLOCK_DETAILS>();
                    if (dbContext.IMS_UNLOCK_DETAILS.Where(m => m.MAST_ER_ROAD_CODE == _roadCode).Any())
                    {
                        return true;
                    }



                    List<MANE_PCI_IMAGE_MAPPING_PMGSY3> listSubDetailsPIC = new List<MANE_PCI_IMAGE_MAPPING_PMGSY3>();
                    List<MANE_CN_PCI_INDEX_PMGSY3> listMasterPCI = new List<MANE_CN_PCI_INDEX_PMGSY3>();
                    if (dbContext.MANE_CN_PCI_INDEX_PMGSY3.Where(m => m.MAST_ER_ROAD_CODE == _roadCode).Any())
                    {

                        return true;
                    }


                    List<MANE_CN_PCI_INDEX_PMGSY3> masterModel4 = new List<MANE_CN_PCI_INDEX_PMGSY3>();
                    if (dbContext.MANE_CN_PCI_INDEX_PMGSY3.Where(m => m.MAST_ER_ROAD_CODE == _roadCode).Any())
                    {
                        return true;
                    }

                    List<MANE_ER_PCI_INDEX> masterModel5 = new List<MANE_ER_PCI_INDEX>();
                    if (dbContext.MANE_ER_PCI_INDEX.Where(m => m.MAST_ER_ROAD_CODE == _roadCode).Any())
                    {
                        return true;
                    }


                    ts.Complete();
                    return false;
                }
            }
            catch (Exception ex)
            {
                return false;
            }
            finally
            {
                dbContext.Configuration.AutoDetectChangesEnabled = true;
            }
        }

        public bool IsCBR(int _roadCode)
        {

            try
            {

                dbContext = new PMGSYEntities();
                dbContext.Configuration.AutoDetectChangesEnabled = false;
                using (TransactionScope ts = new TransactionScope())
                {

                    List<MAST_TRACE_DRRP_SCORE_PMGSY3> masterModel6 = new List<MAST_TRACE_DRRP_SCORE_PMGSY3>();
                    if (dbContext.MAST_TRACE_DRRP_SCORE_PMGSY3.Where(m => m.MAST_ER_ROAD_CODE == _roadCode).Any())
                    {
                        return true;
                    }

                    List<MASTER_ER_CBR_VALUE> masterModel7 = new List<MASTER_ER_CBR_VALUE>();
                    if (dbContext.MASTER_ER_CBR_VALUE.Where(m => m.MAST_ER_ROAD_CODE == _roadCode).Any())
                    {
                        return true;
                    }


                    List<MASTER_ER_CBR_VALUE_PMGSY3> masterModel8 = new List<MASTER_ER_CBR_VALUE_PMGSY3>();
                    if (dbContext.MASTER_ER_CBR_VALUE_PMGSY3.Where(m => m.MAST_ER_ROAD_CODE == _roadCode).Any())
                    {
                        return true;
                    }




                    ts.Complete();
                    return false;
                }
            }
            catch (Exception ex)
            {
                return false;
            }
            finally
            {
                dbContext.Configuration.AutoDetectChangesEnabled = true;
            }
        }


        public bool IsCDWorks(int _roadCode)
        {

            try
            {

                dbContext = new PMGSYEntities();
                dbContext.Configuration.AutoDetectChangesEnabled = false;
                using (TransactionScope ts = new TransactionScope())
                {

                    List<MASTER_ER_CDWORKS_ROAD> masterModel9 = new List<MASTER_ER_CDWORKS_ROAD>();
                    if (dbContext.MASTER_ER_CDWORKS_ROAD.Where(m => m.MAST_ER_ROAD_CODE == _roadCode).Any())
                    {
                        return true;
                    }

                    List<MASTER_ER_CDWORKS_ROAD_PMGSY3> masterModel10 = new List<MASTER_ER_CDWORKS_ROAD_PMGSY3>();
                    if (dbContext.MASTER_ER_CDWORKS_ROAD_PMGSY3.Where(m => m.MAST_ER_ROAD_CODE == _roadCode).Any())
                    {
                        return true;
                    }




                    ts.Complete();
                    return false;
                }
            }
            catch (Exception ex)
            {
                return false;
            }
            finally
            {
                dbContext.Configuration.AutoDetectChangesEnabled = true;
            }
        }

        public bool IsHab(int _roadCode)
        {

            try
            {

                dbContext = new PMGSYEntities();
                dbContext.Configuration.AutoDetectChangesEnabled = false;
                using (TransactionScope ts = new TransactionScope())
                {

                    List<MASTER_ER_HABITATION_ROAD> masterModel11 = new List<MASTER_ER_HABITATION_ROAD>();
                    if (dbContext.MASTER_ER_HABITATION_ROAD.Where(m => m.MAST_ER_ROAD_CODE == _roadCode).Any())
                    {
                        return true;
                    }

                    List<MASTER_ER_HABITATION_ROAD_PMGSY3> masterModel12 = new List<MASTER_ER_HABITATION_ROAD_PMGSY3>();
                    if (dbContext.MASTER_ER_HABITATION_ROAD_PMGSY3.Where(m => m.MAST_ER_ROAD_CODE == _roadCode).Any())
                    {
                        return true;
                    }

                    List<MASTER_ER_MAPROAD_PMGSY3> masterModel13 = new List<MASTER_ER_MAPROAD_PMGSY3>();
                    if (dbContext.MASTER_ER_MAPROAD_PMGSY3.Where(m => m.MAST_ER_ROAD_CODE == _roadCode).Any())
                    {
                        return true;
                    }




                    ts.Complete();
                    return false;
                }
            }
            catch (Exception ex)
            {
                return false;
            }
            finally
            {
                dbContext.Configuration.AutoDetectChangesEnabled = true;
            }
        }


        public bool IsSurface(int _roadCode)
        {
            dbContext = new PMGSYEntities();
            try
            {


                dbContext.Configuration.AutoDetectChangesEnabled = false;
                using (TransactionScope ts = new TransactionScope())
                {
                    List<MASTER_ER_SURFACE_TYPES> masterModel14 = new List<MASTER_ER_SURFACE_TYPES>();
                    if (dbContext.MASTER_ER_SURFACE_TYPES.Where(m => m.MAST_ER_ROAD_CODE == _roadCode).Any())
                    {
                        return true;
                    }

                    List<MASTER_ER_SURFACE_TYPES_PMGSY3> masterModel15 = new List<MASTER_ER_SURFACE_TYPES_PMGSY3>();
                    if (dbContext.MASTER_ER_SURFACE_TYPES_PMGSY3.Where(m => m.MAST_ER_ROAD_CODE == _roadCode).Any())
                    {
                        return true;
                    }



                    ts.Complete();
                    return false;
                }
            }
            catch (Exception ex)
            {
                return false;
            }
            finally
            {
                dbContext.Configuration.AutoDetectChangesEnabled = true;
            }
        }


        public bool IsTraffice(int _roadCode)
        {

            try
            {

                dbContext = new PMGSYEntities();
                dbContext.Configuration.AutoDetectChangesEnabled = false;
                using (TransactionScope ts = new TransactionScope())
                {

                    List<MASTER_ER_TRAFFIC_INTENSITY> masterModel16 = new List<MASTER_ER_TRAFFIC_INTENSITY>();
                    if (dbContext.MASTER_ER_TRAFFIC_INTENSITY.Where(m => m.MAST_ER_ROAD_CODE == _roadCode).Any())
                    {
                        return true;
                    }

                    List<MASTER_ER_TRAFFIC_INTENSITY_PMGSY3> masterModel17 = new List<MASTER_ER_TRAFFIC_INTENSITY_PMGSY3>();
                    if (dbContext.MASTER_ER_TRAFFIC_INTENSITY_PMGSY3.Where(m => m.MAST_ER_ROAD_CODE == _roadCode).Any())
                    {
                        return true;
                    }

                    ts.Complete();
                    return false;
                }
            }
            catch (Exception ex)
            {
                return false;
            }
            finally
            {
                dbContext.Configuration.AutoDetectChangesEnabled = true;
            }
        }

        #endregion
        #endregion


        #region Shift DRRP 27 Jan 2021

        public bool ShiftDRRPToNewBlock(string encryptedDRRPCode, string newBlockCode)
        {
            Models.PMGSYEntities dbContext = new Models.PMGSYEntities();
            try
            {
                int blockCode = 0;
                //int oldBlockCode = 0;
                int drrpCode = 0;

                encryptedParameters = encryptedDRRPCode.Split('/');

                if (!(encryptedParameters.Length == 3))
                {
                    return false;
                }
                decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { encryptedParameters[0], encryptedParameters[1], encryptedParameters[2] });
                drrpCode = Convert.ToInt32(decryptedParameters["RoadCode"].ToString());

                blockCode = Convert.ToInt32(newBlockCode); // new Block Code.
                int newDistCode = dbContext.MASTER_BLOCK.Where(m => m.MAST_BLOCK_CODE == blockCode).Select(m => m.MAST_DISTRICT_CODE).FirstOrDefault();


                if (dbContext.PLAN_ROAD.Where(m => m.MAST_ER_ROAD_CODE == drrpCode).Any())
                {
                    List<int> PlanCnRoadCodes = dbContext.PLAN_ROAD.Where(m => m.MAST_ER_ROAD_CODE == drrpCode).Select(m => m.PLAN_CN_ROAD_CODE).ToList<int>();
                    foreach (var CNCode in PlanCnRoadCodes)
                    {
                        if (dbContext.IMS_SANCTIONED_PROJECTS.Where(m => m.PLAN_CN_ROAD_CODE == CNCode && (m.IMS_SANCTIONED == "Y" || m.IMS_SANCTIONED == "D")).Any())
                        {
                            return false;

                        }
                    }

                }

                if (dbContext.PLAN_ROAD_DRRP.Where(m => m.MAST_ER_ROAD_CODE == drrpCode).Any())
                {
                    List<int> PlanCnRoadCodes1 = dbContext.PLAN_ROAD_DRRP.Where(m => m.MAST_ER_ROAD_CODE == drrpCode).Select(m => m.PLAN_CN_ROAD_CODE).ToList<int>();
                    foreach (var CNCode1 in PlanCnRoadCodes1)
                    {
                        if (dbContext.IMS_SANCTIONED_PROJECTS.Where(m => m.PLAN_CN_ROAD_CODE == CNCode1 && (m.IMS_SANCTIONED == "Y" || m.IMS_SANCTIONED == "D")).Any())
                        {
                            return false;

                        }
                    }

                }




                using (System.IO.StreamWriter sw = System.IO.File.AppendText(System.IO.Path.Combine("D:\\ErrorLog\\OMMASErrorLog_" + System.DateTime.Now.Date.ToString("dd_MM_yyyy") + ".txt", "")))
                {
                    sw.WriteLine("Date :" + DateTime.Now.ToString());
                    sw.WriteLine("Method : " + "ShiftDRRPToNewBlock()");

                    sw.WriteLine("newblockCode : " + blockCode.ToString());
                    sw.WriteLine("UserId : " + PMGSYSession.Current.UserId.ToString());
                    sw.WriteLine("IPADD : " + HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"].ToString());
                    sw.WriteLine("---------------------------------------------------------------------------------------");
                    sw.Close();
                }
                PMGSY.Models.MASTER_EXISTING_ROADS masterDRRP = new Models.MASTER_EXISTING_ROADS();

                masterDRRP = dbContext.MASTER_EXISTING_ROADS.Where(m => m.MAST_ER_ROAD_CODE == drrpCode).FirstOrDefault();

                if (masterDRRP != null)
                {
                    masterDRRP.MAST_BLOCK_CODE = blockCode; // New Block Code is assigned here.
                    masterDRRP.MAST_DISTRICT_CODE = newDistCode;
                    dbContext.Entry(masterDRRP).State = System.Data.Entity.EntityState.Modified;
                    dbContext.SaveChanges();
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (OptimisticConcurrencyException ex)
            {
                ErrorLog.LogError(ex, "ExistingRoadsDAL.ShiftDRRPToNewBlock.OptimisticConcurrencyException");
                return false;
            }
            catch (UpdateException ex)
            {
                ErrorLog.LogError(ex, "ExistingRoadsDAL.ShiftDRRPToNewBlock.UpdateException");
                return false;
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "ExistingRoadsDAL.ShiftDRRPToNewBlock");
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

        public bool ShiftDRRPToNewBlockPMGSY3(string encryptedDRRPCode, string newBlockCode)
        {
            Models.PMGSYEntities dbContext = new Models.PMGSYEntities();
            try
            {
                int blockCode = 0;
                //int oldBlockCode = 0;
                int drrpCode = 0;

                encryptedParameters = encryptedDRRPCode.Split('/');

                if (!(encryptedParameters.Length == 3))
                {
                    return false;
                }
                decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { encryptedParameters[0], encryptedParameters[1], encryptedParameters[2] });
                drrpCode = Convert.ToInt32(decryptedParameters["RoadCode"].ToString());

                blockCode = Convert.ToInt32(newBlockCode); // new Block Code.
                int newDistCode = dbContext.MASTER_BLOCK.Where(m => m.MAST_BLOCK_CODE == blockCode).Select(m => m.MAST_DISTRICT_CODE).FirstOrDefault();


                if (dbContext.PLAN_ROAD.Where(m => m.MAST_ER_ROAD_CODE == drrpCode).Any())
                {
                    List<int> PlanCnRoadCodes = dbContext.PLAN_ROAD.Where(m => m.MAST_ER_ROAD_CODE == drrpCode).Select(m => m.PLAN_CN_ROAD_CODE).ToList<int>();
                    foreach (var CNCode in PlanCnRoadCodes)
                    {
                        if (dbContext.IMS_SANCTIONED_PROJECTS.Where(m => m.PLAN_CN_ROAD_CODE == CNCode && (m.IMS_SANCTIONED == "Y" || m.IMS_SANCTIONED == "D")).Any())
                        {
                            return false;

                        }
                    }

                }

                if (dbContext.PLAN_ROAD_MRL_PMGSY3.Where(m => m.MAST_ER_ROAD_CODE == drrpCode).Any())
                {
                    List<int> PlanCnRoadCodes1 = dbContext.PLAN_ROAD_MRL_PMGSY3.Where(m => m.MAST_ER_ROAD_CODE == drrpCode).Select(m => m.PLAN_CN_ROAD_CODE).ToList<int>();
                    foreach (var CNCode1 in PlanCnRoadCodes1)
                    {
                        if (dbContext.IMS_SANCTIONED_PROJECTS.Where(m => m.PLAN_CN_ROAD_CODE == CNCode1 && (m.IMS_SANCTIONED == "Y" || m.IMS_SANCTIONED == "D")).Any())
                        {
                            return false;

                        }
                    }

                }




                using (System.IO.StreamWriter sw = System.IO.File.AppendText(System.IO.Path.Combine("D:\\ErrorLog\\OMMASErrorLog_" + System.DateTime.Now.Date.ToString("dd_MM_yyyy") + ".txt", "")))
                {
                    sw.WriteLine("Date :" + DateTime.Now.ToString());
                    sw.WriteLine("Method : " + "ShiftDRRPToNewBlock()");

                    sw.WriteLine("newblockCode : " + blockCode.ToString());
                    sw.WriteLine("UserId : " + PMGSYSession.Current.UserId.ToString());
                    sw.WriteLine("IPADD : " + HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"].ToString());
                    sw.WriteLine("---------------------------------------------------------------------------------------");
                    sw.Close();
                }
                PMGSY.Models.MASTER_EXISTING_ROADS masterDRRP = new Models.MASTER_EXISTING_ROADS();

                masterDRRP = dbContext.MASTER_EXISTING_ROADS.Where(m => m.MAST_ER_ROAD_CODE == drrpCode).FirstOrDefault();

                if (masterDRRP != null)
                {
                    masterDRRP.MAST_BLOCK_CODE = blockCode; // New Block Code is assigned here.
                    masterDRRP.MAST_DISTRICT_CODE = newDistCode;
                    dbContext.Entry(masterDRRP).State = System.Data.Entity.EntityState.Modified;
                    dbContext.SaveChanges();
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (OptimisticConcurrencyException ex)
            {
                ErrorLog.LogError(ex, "ExistingRoadsDAL.ShiftDRRPToNewBlock.OptimisticConcurrencyException");
                return false;
            }
            catch (UpdateException ex)
            {
                ErrorLog.LogError(ex, "ExistingRoadsDAL.ShiftDRRPToNewBlock.UpdateException");
                return false;
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "ExistingRoadsDAL.ShiftDRRPToNewBlock");
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
    }

    public interface IExistingRoadsDAL
    {
        bool CheckUnlockedDAL(int blockCode);

        #region Existing Roads DAL declaration


        bool IsExistingRoadIsMappedWithCN_CR(ExistingRoadsViewModel model, int ErRoadCode, out decimal? CN_CRRoadLength, out decimal TotalERRoadLength);

        bool AddExistingRoads(ExistingRoadsViewModel existingRoadsViewModel, ref string message);

        bool EditExistingRoads(ExistingRoadsViewModel existingRoadsViewModel, ref string message);

        Boolean DeleteExistingRoads(int _roadCode, ref string message);

        Array ListExistingRoads(int stateCode, int districtCode, int blockCode, int categoryCode, int ownerCode, int? page, int? rows, string sidx, string sord, out long totalRecords, string filters);

        ExistingRoadsViewModel GetExistingRoads_ByRoadCode(int _roadCode);
        ExistingRoadsViewModel GetExistingRoads_ForViewDetails(int _roadCode);

        Array GetSurfaceList(int? page, int? rows, string sidx, string sord, out Int32 totalRecords, int roadCode);

        Array GetTrafficListDAL(int? page, int? rows, string sidx, string sord, out Int32 totalRecords, int roadCode);

        Array GetCBRListDAL(int? page, int? rows, string sidx, string sord, out Int32 totalRecords, int roadCode);

        Array GetCdWorkList(int? page, int? rows, string sidx, string sord, out Int32 totalRecords, int roadCode);

        Array GetHabitationList(int? page, int? rows, string sidx, string sord, out Int32 totalRecords, int roadCode);

        MASTER_EXISTING_ROADS CloneExistingRoadsModel(ExistingRoadsViewModel existingRoadsViewModel, bool flagExistingRoadsAddEdit);

        ExistingRoadsViewModel CloneExistingRoadsObject(MASTER_EXISTING_ROADS ExistingRoadsModel);

        int GetMaxExistingRoadCode();

        List<MASTER_BLOCK> GetAllBlockNames(int MAST_DISTRICT_CODE);

        List<MASTER_ROAD_CATEGORY> GetAllRoadCategory();

        List<MASTER_AGENCY> GetAllGovOwner();

        List<MASTER_SOIL_TYPE> GetAllSoilTypes();

        List<MASTER_TERRAIN_TYPE> GetAllTerrainTypes();

        List<SelectListItem> GetConstructionYears();

        List<SelectListItem> GetPeriodicRenewalYears();

        String GetRoadShortName(int roadCategoryCode);

        String FinalizeExistingRoad(int MAST_ER_ROAD_CODE);

        Array GetCoreNetworkList(int? page, int? rows, string sidx, string sord, out Int32 totalRecords, int roadCode);

        #endregion Existing Roads DAL declaration

        #region Traffic Intensity DAL

        bool AddTrafficIntensity(TrafficViewModel trafficViewModel, ref string message);
        bool EditTrafficIntensity(TrafficViewModel trafficViewModel, ref string message);
        Boolean DeleteTrafficIntensity(int MAST_ER_ROAD_CODE, int MAST_TI_YEAR, ref string message);
        TrafficViewModel GetTrafficIntensity_ByRoadCode(int MAST_ER_ROAD_CODE, int MAST_TI_YEAR);

        MASTER_ER_TRAFFIC_INTENSITY CloneTrafficIntensityModel(TrafficViewModel trafficViewModel);
        TrafficViewModel CloneTrafficIntensityObject(MASTER_ER_TRAFFIC_INTENSITY trafficIntensityModel);

        List<SelectListItem> PopulateTrafficIntensityYears(int MAST_ER_ROAD_CODE);
        List<SelectListItem> PopulateYear(int MAST_ER_ROAD_CODE);
        #endregion Traffic Intensity DAL

        #region CBR Value Intensity DAL

        bool AddCbrValue(CBRViewModel CbrViewModel, ref string message);
        bool EditCbrValue(CBRViewModel CbrViewModel, ref string message);
        Boolean DeleteCbrValue(int MAST_ER_ROAD_CODE, int MAST_SEGMENT_NO, ref string message);
        CBRViewModel GetCBRDetails(int MAST_ER_ROAD_CODE, int MAST_SEGMENT_NO);
        MASTER_ER_CBR_VALUE CloneCBRModel(CBRViewModel CbrViewModel, bool flagCbrAddEdit);
        CBRViewModel CloneCBRObject(MASTER_ER_CBR_VALUE CBRModel);

        #endregion CBR Value Intensity DAL

        #region CDWorks DAL

        bool AddCDWorksDetails(CdWorksViewModel cdWorksViewModel, ref string message);
        bool EditCDWorksDetails(CdWorksViewModel cdWorksViewModel, ref string message);
        Boolean DeleteCDWorksDetails(int MAST_ER_ROAD_CODE, int MAST_CD_NUMBER, ref string message);
        CdWorksViewModel GetCDWorksDetails(int MAST_ER_ROAD_CODE, int MAST_CD_NUMBER);
        MASTER_ER_CDWORKS_ROAD CloneCDWorkModel(CdWorksViewModel cdWorksViewModel, bool flagCdWorksAddEdit);
        CdWorksViewModel CloneCDWorkObject(MASTER_ER_CDWORKS_ROAD CDWorksModel);

        List<MASTER_CDWORKS_TYPE_CONSTRUCTION> GetCDWorkTypes();

        #endregion CDWorks DAL

        #region Surface Type DAL

        bool AddSurfaceDetails(SurfaceTypeViewModel SurfaceViewModel, ref string message);
        bool EditSurfaceDetails(SurfaceTypeViewModel SurfaceViewModel, ref string message);
        Boolean DeleteSurfaceDetails(int MAST_ER_ROAD_CODE, int MAST_SURFACE_SEG_NO, ref string message);
        SurfaceTypeViewModel GetSurfaceDetails(int MAST_ER_ROAD_CODE, int MAST_SURFACE_SEG_NO);
        MASTER_ER_SURFACE_TYPES CloneSurfaceModel(SurfaceTypeViewModel SurfaceViewModel, bool flagSufaceAddEdit);
        SurfaceTypeViewModel CloneSurfaceObject(MASTER_ER_SURFACE_TYPES SurfaceModel);
        List<MASTER_SURFACE> GetAllSurface();
        List<SelectListItem> GetRoadCondition();

        #endregion Surface Type DAL

        #region Habitation DAL
        Array GetHabitationListToMap(int roadCode, int blockCode, int page, int rows, string sidx, string sord, out long totalRecords);
        Array GetAllHabitationList(int habCode, int page, int rows, string sidx, string sord, out long totalRecords);
        bool MapHabitationToRoad(string habCode, string roadCode);
        bool DeleteMapHabitation(int habitationCode, int roadCode, out string message);
        List<MASTER_REASON> GetAllReasons();
        #endregion Habitation DAL

        #region Map DRRP for PMGSY 1

        bool MapDRRPPMGSY1RoadsDAL(int erRoadCode, int erRoadCode1, ref string message);
        Array GetMappedDRRPPmgsy1ListDAL(int block, int erRoadCode, int page, int rows, string sidx, string sord, out long totalRecords);
        bool UnMapDRRPPMGSY1RoadsDAL(int roadCode, ref string message);
        #endregion

        #region DRRP - II PMGSY-I Mapping

        Array GetProposalsForDRRPMappingUnderPMGSY3DAL(int? page, int? rows, string sidx, string sord, out long totalRecords, int stateCode, int districtCode, int IMS_YEAR, int MAST_BLOCK_ID, int IMS_BATCH, int IMS_STREAMS, string IMS_PROPOSAL_TYPE, int adminCode, string IMS_PROPOSAL_STATUS, string IMS_UPGRADE_CONNECT);
        bool MapDRRPDetailsDAL(MapDRRPUnderPMGSY3 model);

        #endregion

        #region Trace Maps
        Array GetDistrictListDAL(int page, int rows, string sidx, string sord, out int totalRecords, int blockcode);
        string TraceMapsSaveFileDetailsDAL(List<FileUploadModel> lst_files, ref string filename);
        Array GetTraceMapsFilesListDAL(int page, int rows, string sidx, string sord, out int totalRecords, int blockcode);
        Array GetTraceCSVFilesListDAL(int page, int rows, string sidx, string sord, out int totalRecords, int blockcode);

        #endregion

        #region List ER ITNO
        Array ListExistingRoadsITNO(int stateCode, int districtCode, int blockCode, int categoryCode, int ownerCode, int? page, int? rows, string sidx, string sord, out long totalRecords, string filters);
        Boolean DeleteExistingRoadsITNO(int _roadCode, ref string message);
        Boolean DeleteExistingRoadsMainITNO(int _roadCode, ref string message);
        Array GetCoreNetworkListITNO(int? page, int? rows, string sidx, string sord, out Int32 totalRecords, int roadCode);
        #endregion

        #region ITNO ER for Inactive Blocks
        Array ListExistingRoadsITNOForInactiveBlocksDAL(int stateCode, int districtCode, int blockCode, int categoryCode, int ownerCode, int? page, int? rows, string sidx, string sord, out long totalRecords, string filters);
        bool ShiftDetailsDAL(string encryptedVillageCode, string newBlockCode, string newDistrictCode, string ERCode);
        #endregion

        #region Not Feasible Roads under PMGSY III
        //GetNotFeasibleRoadsListDAL
        Array GetNotFeasibleRoadsListDAL(int? page, int? rows, string sidx, string sord, out long totalRecords, int stateCode, int districtCode, int IMS_YEAR, int MAST_BLOCK_ID, int IMS_BATCH, int IMS_STREAMS, string IMS_PROPOSAL_TYPE, int adminCode, string IMS_PROPOSAL_STATUS, string IMS_UPGRADE_CONNECT);
        // MapRoadDetailsDAL

        bool MapRoadDetailsDAL(MapNotFeasibleRoads model);

        #endregion


        #region  Existing Road Shift
        Array ListExistingRoadsForShift(int stateCode, int districtCode, int blockCode, int SchemeCode, int ownerCode, int? page, int? rows, string sidx, string sord, out long totalRecords, string filters);

        bool ShiftERDetailsDAL(string encryptedVillageCode, string newBlockCode, string newDistrictCode, string ERCode);
        #endregion
    }
}