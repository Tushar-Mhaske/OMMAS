using PMGSY.Common;
using PMGSY.Extensions;
using PMGSY.Models;
using PMGSY.Areas.WronglyMappedHabs.Models;
using PMGSY.Areas.WronglyMappedHabs.BAL;
using PMGSY.Models.Master;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Validation;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Transactions;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace PMGSY.Areas.WronglyMappedHabs.DAL
{
    public class WronglyMappedHabsDAL : IWronglyMappedHabsDAL
    {
        PMGSYEntities dbContext = null;
        Dictionary<string, string> decryptedParameters = null;
        string[] encryptedParameters = null;

        public Array ListHabsDAL(int stateCode, int? page, int? rows, string sidx, string sord, out long totalRecords)
        {

            try
            {
                dbContext = new PMGSYEntities();
                var lstMasterAgencyDetails = dbContext.USP_WRONG_MAPPED_HABS(stateCode).ToList();
                                             
                totalRecords = lstMasterAgencyDetails.Count();

                if (sidx.Trim() != string.Empty)
                {
                    if (sord.ToString() == "asc")
                    {
                        switch (sidx)
                        {
                            case "IMS_ROAD_NAME":
                                lstMasterAgencyDetails = lstMasterAgencyDetails.OrderBy(x => x.IMS_ROAD_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                            case "State":
                                lstMasterAgencyDetails = lstMasterAgencyDetails.OrderBy(x => x.State).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                            case "District":
                                lstMasterAgencyDetails = lstMasterAgencyDetails.OrderBy(x => x.District).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                            case "Block":
                                lstMasterAgencyDetails = lstMasterAgencyDetails.OrderBy(x => x.Block).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                            case "IMS_YEAR":
                                lstMasterAgencyDetails = lstMasterAgencyDetails.OrderBy(x => x.IMS_YEAR).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                            case "IMS_BATCH":
                                lstMasterAgencyDetails = lstMasterAgencyDetails.OrderBy(x => x.IMS_BATCH).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                            case "IMS_PACKAGE_ID":
                                lstMasterAgencyDetails = lstMasterAgencyDetails.OrderBy(x => x.IMS_PACKAGE_ID).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                            default:
                                
                                break;
                        }
                    }
                    else
                    {
                        switch (sidx)
                        {
                            case "IMS_ROAD_NAME":
                                lstMasterAgencyDetails = lstMasterAgencyDetails.OrderByDescending(x => x.IMS_ROAD_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                            case "State":
                                lstMasterAgencyDetails = lstMasterAgencyDetails.OrderBy(x => x.State).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                            case "District":
                                lstMasterAgencyDetails = lstMasterAgencyDetails.OrderBy(x => x.District).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                            case "Block":
                                lstMasterAgencyDetails = lstMasterAgencyDetails.OrderBy(x => x.Block).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                            case "IMS_YEAR":
                                lstMasterAgencyDetails = lstMasterAgencyDetails.OrderByDescending(x => x.IMS_YEAR).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                            case "IMS_BATCH":
                                lstMasterAgencyDetails = lstMasterAgencyDetails.OrderByDescending(x => x.IMS_BATCH).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                            case "IMS_PACKAGE_ID":
                                lstMasterAgencyDetails = lstMasterAgencyDetails.OrderByDescending(x => x.IMS_PACKAGE_ID).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                            default:
                                break;
                        }
                    }
                }
                else
                {
                    lstMasterAgencyDetails = lstMasterAgencyDetails.OrderBy(x => x.IMS_ROAD_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                }

                var result = lstMasterAgencyDetails.Select(item => new
                {

                    item.State,
                    item.District,
                    item.Block,
                    item.IMS_ROAD_NAME,
                    item.IMS_YEAR,
                    item.IMS_BATCH,
                    item.IMS_PACKAGE_ID,
                    item.Stage,
                    item.StagePhase,
                    item.UpgradeConnect,
                    item.Habitation,
                    item.HabPop,
                    item.HabConnect, 
                    item.MAST_HAB_CODE,
                    item.IMS_PR_ROAD_CODE

                }).ToArray();



                return result.Select(item => new
                {
                    cell = new[]{
                                     item.State,
                                     item.District,
                                     item.Block,
                                     item.IMS_ROAD_NAME.Trim(),
                                     item.IMS_YEAR.ToString(),
                                     item.IMS_BATCH.ToString(),
                                     item.IMS_PACKAGE_ID,
                                     item.Stage,
                                     item.StagePhase,
                                     item.UpgradeConnect,
                                     item.Habitation,
                                     item.HabPop.ToString(),
                                     item.HabConnect,                                 
                                   URLEncrypt.EncryptParameters1(new string[]{"habCode="+item.MAST_HAB_CODE.ToString().Trim(), "roadCode="+item
                                   .IMS_PR_ROAD_CODE.ToString().Trim()}
                                   
                                   )  
                      
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

        public Boolean DeleteHabsDAL(int habCode, int roadCode)
        {
            try
            {
                dbContext = new PMGSYEntities();
                IMS_BENEFITED_HABS HabsDetails = dbContext.IMS_BENEFITED_HABS.Where(m => m.IMS_PR_ROAD_CODE == roadCode && m.MAST_HAB_CODE == habCode).FirstOrDefault();

                dbContext.IMS_BENEFITED_HABS.Remove(HabsDetails);
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

    }
   
    public interface IWronglyMappedHabsDAL
     {
        //ListHabsDAL
        Array ListHabsDAL(int stateCode, int? page, int? rows, string sidx, string sord, out long totalRecords);
        Boolean DeleteHabsDAL(int habCode, int roadCode);
    }



    
}