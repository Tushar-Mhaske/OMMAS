using PMGSY.Common;
using PMGSY.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PMGSY.Extensions;
using PMGSY.Models.MaintainanceInspection;
using System.Data.Entity;
using System.Transactions;
using System.Configuration;
using System.IO;
using System.Data.Entity.Core;


/*
 * MaintainanceInspectionController:Purpose 1)Add/Edit/Delete/List view of 
 * MaintainanceInspection Details
 * 
 */

namespace PMGSY.DAL.MaintainanceInspection
{
    public class MaintainanceInspectionDAL : IMaintenanceInspectionDAL
    {
        private readonly Dictionary<String, String> AgreementStatus = new Dictionary<string, string>() { { "P", "In Progress" }, { "I", "Incomplete" }, { "C", "Agreement Completed" } };

        #region Maintenance Inspection
        public Array GetFilesListDAL(int page, int rows, string sidx, string sord, out Int32 totalRecords, int IMS_PR_ROAD_CODE)
        {
            PMGSYEntities dbContext = new PMGSYEntities();
            try
            {
                List<MAINTENANCE_FILES> listExecutionFiles = dbContext.MAINTENANCE_FILES.Where(p => p.IMS_PR_ROAD_CODE == IMS_PR_ROAD_CODE && p.MAINTENANCE_FILE_TYPE == 0).ToList();
                IQueryable<MAINTENANCE_FILES> query = listExecutionFiles.AsQueryable<MAINTENANCE_FILES>();
                totalRecords = listExecutionFiles.Count();

                if (sidx.Trim() != string.Empty)
                {
                    if (sord.ToString() == "asc")
                    {
                        query = query.OrderBy(x => x.IMS_PR_ROAD_CODE).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                    }
                    else
                    {
                        query = query.OrderByDescending(x => x.IMS_PR_ROAD_CODE).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                    }
                }
                else
                {
                    query = query.OrderBy(x => x.IMS_PR_ROAD_CODE).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                }

                //string VirtualDirectoryUrl = Path.Combine(ConfigurationManager.AppSettings["PROPOSAL_FILE_UPLOAD_VIRTUAL_DIR_PATH"], "thumbnails");
                string VirtualDirectoryUrl = Path.Combine(ConfigurationManager.AppSettings["MAINTENANCE_PRGRESS_FILE_UPLOAD_VIRTUAL_DIR_PATH"], "thumbnails");
                //string PhysicalPath = ConfigurationManager.AppSettings["PROPOSAL_FILE_UPLOAD"];
                string PhysicalPath = ConfigurationManager.AppSettings["MAINTENANCE_PRGRESS_FILE_UPLOAD"];

                //string imageUrl = Path.Combine(VirtualDirectoryUrl, query.Select(c => c.IMS_FILE_NAME).First().ToString()).ToString().Replace(@"\\",@"//").Replace(@"\",@"/");

                return query.Select(fileDetails => new
                {
                    id = fileDetails.MAINTENANCE_FILE_ID + "$" + fileDetails.IMS_PR_ROAD_CODE,
                    cell = new[] {   
                                   // @"file/://"  + Path.Combine(PhysicalPath, fileDetails.IMS_FILE_NAME.ToString()).ToString().Replace(@"\\",@"//").Replace(@"\",@"/"),
                                    Path.Combine(VirtualDirectoryUrl, fileDetails.MAINTENANCE_FILE_NAME.ToString()).ToString().Replace(@"\\",@"//").Replace(@"\",@"/"),
                                    //fileDetails.CHAINAGE.ToString(),
                                    fileDetails.MAINTENANCE_FILE_DESC,
                                    dbContext.MASTER_EXECUTION_ITEM.Where(m=>m.MAST_HEAD_CODE == fileDetails.MAINTENANCE_STAGE).Select(m=>m.MAST_HEAD_DESC).FirstOrDefault(),
                                    "<a href='#' title='Click here to Download an Image' class='ui-icon ui-icon-arrowthickstop-1-s ui-align-center' onClick=DownLoadImage(\"" + URLEncrypt.EncryptParameters(new string[] { fileDetails.MAINTENANCE_FILE_NAME  }) +"\"); return false;'>Download</a>" ,
                                    "<a href='#' title='Click here to Edit the File Details' class='ui-icon ui-icon-pencil ui-align-center' onClick=EditImageDetails('" +  fileDetails.MAINTENANCE_FILE_ID.ToString().Trim()  + "$" + fileDetails.IMS_PR_ROAD_CODE.ToString().Trim() +"'); return false;>Edit</a>",
                                    "<a href='#' title='Click here to delete the File Details' class='ui-icon ui-icon-trash ui-align-center' onClick=DeleteFileDetails('" + fileDetails.MAINTENANCE_FILE_ID.ToString().Trim()  + "$" + fileDetails.IMS_PR_ROAD_CODE.ToString().Trim() + "','" + fileDetails.MAINTENANCE_FILE_NAME + "'); return false;>Delete</a>",                                    
                                    "<center><table><tr><td style='border-color:white'><a href='#' style='float:left' id='btnSave"+  fileDetails.MAINTENANCE_FILE_ID.ToString().Trim()  + "$" + fileDetails.IMS_PR_ROAD_CODE.ToString().Trim()  +"' title='Click here to Save the File Details' class='ui-icon ui-icon-disk ui-align-center' onClick=SaveFileDetails('" +  fileDetails.MAINTENANCE_FILE_ID.ToString().Trim() + "$" + fileDetails.IMS_PR_ROAD_CODE.ToString().Trim() + "');></a><a href='#' style='float:right' id='btnCancel" +  fileDetails.MAINTENANCE_FILE_ID.ToString().Trim()  + "$" + fileDetails.IMS_PR_ROAD_CODE.ToString().Trim()  +"' title='Click here to Cancel the File Edit' class='ui-icon ui-icon-closethick ui-align-center' onClick= CancelSaveFileDetails('" +  fileDetails.MAINTENANCE_FILE_ID.ToString().Trim() + "$" + fileDetails.IMS_PR_ROAD_CODE.ToString().Trim() + "');></a></td></tr></table></center>"
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

        /// <summary>
        /// Add Image Files
        /// </summary>
        /// <param name="lst_execution_files">list of files along with file details</param>
        /// <returns></returns>
        public string AddFileUploadDetailsDAL(List<MAINTENANCE_FILES> lst_maintenance_files)
        {
            PMGSYEntities dbContext = new PMGSYEntities();
            try
            {
                
                Int32? MaxID;
                foreach (MAINTENANCE_FILES fileModel in lst_maintenance_files)
                {
                    if (dbContext.MAINTENANCE_FILES.Count() == 0)
                    {
                        MaxID = 0;
                    }
                    else
                    {
                        if (dbContext.MAINTENANCE_FILES.Where(s => s.IMS_PR_ROAD_CODE == fileModel.IMS_PR_ROAD_CODE).Any())
                        {
                            MaxID = (from c in dbContext.MAINTENANCE_FILES.Where(s => s.IMS_PR_ROAD_CODE == fileModel.IMS_PR_ROAD_CODE) select (Int32?)c.MAINTENANCE_FILE_ID ?? 0).Max();
                        }
                        else
                        {
                            MaxID = 0;
                        }
                    }
                    if (fileModel.MAINTENANCE_LATITUDE == 0)
                    {
                        fileModel.MAINTENANCE_LATITUDE = null;
                    }
                    if (fileModel.MAINTENANCE_LONGITUDE == 0)
                    {
                        fileModel.MAINTENANCE_LONGITUDE = null;
                    }
                    ++MaxID;
                    //fileModel.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                    //fileModel.USERID = PMGSYSession.Current.UserId; 
                    fileModel.MAINTENANCE_FILE_ID = Convert.ToInt32(MaxID);
                    dbContext.MAINTENANCE_FILES.Add(fileModel);
                }
                dbContext.SaveChanges();
                return string.Empty;
            }
            catch (System.Data.Entity.Infrastructure.DbUpdateException)
            {
                return ("An Error Occurred While Your Processing Request.");
            }
            catch (OptimisticConcurrencyException)
            {
                return ("An Error Occurred While Your Processing Request.");
            }
            catch (Exception)
            {
                return ("Error Occurred While Processing Request.");
            }
            finally
            {
                dbContext.Dispose();
            }
        }

        /// <summary>
        /// update image Details
        /// </summary>
        /// <param name="execution_files">model data containing the updated file details</param>
        /// <returns></returns>
        public string UpdateImageDetailsDAL(MAINTENANCE_FILES maintenance_files)
        {
            PMGSYEntities dbContext = new PMGSYEntities();
            try
            {
                MAINTENANCE_FILES db_maintenance_files = dbContext.MAINTENANCE_FILES.Where(
                    a => a.MAINTENANCE_FILE_ID == maintenance_files.MAINTENANCE_FILE_ID &&
                    a.IMS_PR_ROAD_CODE == maintenance_files.IMS_PR_ROAD_CODE
                    ).FirstOrDefault();

                db_maintenance_files.MAINTENANCE_FILE_DESC = db_maintenance_files.MAINTENANCE_FILE_DESC;

                dbContext.Entry(db_maintenance_files).State = System.Data.Entity.EntityState.Modified;
                dbContext.SaveChanges();

                return string.Empty;
            }
            catch (System.Data.Entity.Infrastructure.DbUpdateException)
            {
                return ("An Error Occurred While Your Processing Request.");
            }
            catch (OptimisticConcurrencyException)
            {
                return ("An Error Occurred While Your Processing Request.");
            }
            catch (Exception)
            {
                return ("Error Occurred While Processing Request.");
            }
            finally
            {
                dbContext.Dispose();
            }
        }

        /// <summary>
        /// Delete Image Files
        /// </summary>
        /// <param name="execution_files">file along with details</param>
        /// <returns></returns>
        public string DeleteFileDetailsDAL(MAINTENANCE_FILES maintenance_files)
        {
            PMGSYEntities dbContext = new PMGSYEntities();
            try
            {
                MAINTENANCE_FILES obj_maintenance_files = dbContext.MAINTENANCE_FILES.Where(
                    a => a.IMS_PR_ROAD_CODE == maintenance_files.IMS_PR_ROAD_CODE &&
                    a.MAINTENANCE_FILE_ID == maintenance_files.MAINTENANCE_FILE_ID
                     &&
                    a.MAINTENANCE_FILE_NAME == maintenance_files.MAINTENANCE_FILE_NAME).FirstOrDefault();

                dbContext.MAINTENANCE_FILES.Remove(obj_maintenance_files);
                dbContext.SaveChanges();
                return string.Empty;
            }
            catch
            {
                return "There is an error while processing request.";
            }
            finally
            {
                dbContext.Dispose();
            }
        }
        #endregion

        #region MAINTENANCE_INSPECTION

        /// <summary>
        /// This function is used to calculated max code
        /// </summary>
        /// <param name="module"> MasterDataEntryModules object</param>
        /// <returns> MaxCode</returns>

        private Int32 GetMaxCode(MaintainanceInspectionViewModel module)
        {
            Int64? maxCode = 0;
            PMGSYEntities dbContext = new PMGSYEntities();
            try
            {
                maxCode = (from IMSInspection in dbContext.MANE_IMS_INSPECTION where IMSInspection.IMS_PR_ROAD_CODE == module.IMS_PR_ROAD_CODE select (Int64?)IMSInspection.IMS_INSPECTION_CODE).Max();
                if (maxCode == null)
                {
                    maxCode = 1;
                }
                else
                {
                    maxCode = maxCode + 1;
                }

                return (Int32)maxCode;
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


        public Array GetCompletedRoadListBAL(int stateCode, int districtCode, int blockCode, int sanctionedYear, int adminNDCode, string packageID, int batch, int collaboration, string upgradationType, int page, int rows, string sidx, string sord, out long totalRecords)
        {
            PMGSYEntities dbContext = new PMGSYEntities();
            try
            {
                if (packageID.Contains("All"))
                {
                    packageID = "All Packages";
                }

                var query = (from imsSanctionedProjectDetails in dbContext.IMS_SANCTIONED_PROJECTS
                             join execRoads in dbContext.EXEC_ROADS_MONTHLY_STATUS
                             on imsSanctionedProjectDetails.IMS_PR_ROAD_CODE equals execRoads.IMS_PR_ROAD_CODE
                             join blockDetails in dbContext.MASTER_BLOCK
                             on imsSanctionedProjectDetails.MAST_BLOCK_CODE equals blockDetails.MAST_BLOCK_CODE
                             join districtDetails in dbContext.MASTER_DISTRICT
                             on imsSanctionedProjectDetails.MAST_DISTRICT_CODE equals districtDetails.MAST_DISTRICT_CODE
                             join stateDetails in dbContext.MASTER_STATE
                             on imsSanctionedProjectDetails.MAST_STATE_CODE equals stateDetails.MAST_STATE_CODE
                             join adminDetails in dbContext.ADMIN_DEPARTMENT
                             on imsSanctionedProjectDetails.MAST_DPIU_CODE equals adminDetails.ADMIN_ND_CODE
                             join fundingAgency in dbContext.MASTER_FUNDING_AGENCY
                             on imsSanctionedProjectDetails.IMS_COLLABORATION equals fundingAgency.MAST_FUNDING_AGENCY_CODE into agencies
                             from fundingAgency in agencies.DefaultIfEmpty()
                             //new change by Vikram
                             join contractDetails in dbContext.MANE_IMS_CONTRACT
                             on imsSanctionedProjectDetails.IMS_PR_ROAD_CODE equals contractDetails.IMS_PR_ROAD_CODE
                             where
                             imsSanctionedProjectDetails.MAST_STATE_CODE == stateCode &&
                             imsSanctionedProjectDetails.MAST_DISTRICT_CODE == districtCode &&
                             imsSanctionedProjectDetails.MAST_DPIU_CODE == adminNDCode &&
                             imsSanctionedProjectDetails.IMS_SANCTIONED == "Y" &&
                             imsSanctionedProjectDetails.IMS_PROPOSAL_TYPE == "P" &&
                             execRoads.EXEC_ISCOMPLETED == "C" &&
                             contractDetails.MANE_CONTRACT_FINALIZED == "Y" &&
                             (sanctionedYear == 0 ? 1 : imsSanctionedProjectDetails.IMS_YEAR) == (sanctionedYear == 0 ? 1 : sanctionedYear) &&
                             (blockCode == -1 ? 1 : imsSanctionedProjectDetails.MAST_BLOCK_CODE) == (blockCode == -1 ? 1 : blockCode) &&
                             (packageID == "All Packages" ? "%" : imsSanctionedProjectDetails.IMS_PACKAGE_ID.ToUpper()) == (packageID == "All Packages" ? "%" : packageID.ToUpper()) &&
                                 //new filters added by Vikram 
                             (batch == 0 ? 1 : imsSanctionedProjectDetails.IMS_BATCH) == (batch == 0 ? 1 : batch) &&
                             (collaboration <= 0 ? 1 : imsSanctionedProjectDetails.IMS_COLLABORATION) == (collaboration <= 0 ? 1 : collaboration) &&
                             (upgradationType == "0" ? "%" : imsSanctionedProjectDetails.IMS_UPGRADE_CONNECT) == (upgradationType == "0" ? "%" : upgradationType) &&
                                 //end of change
                             imsSanctionedProjectDetails.MAST_PMGSY_SCHEME == PMGSYSession.Current.PMGSYScheme //new change done by Vikram on 10 Feb 2014
                             && imsSanctionedProjectDetails.IMS_DPR_STATUS == "N" //new change done by Vikram
                             select new
                             {
                                 imsSanctionedProjectDetails.IMS_PR_ROAD_CODE,
                                 imsSanctionedProjectDetails.IMS_ROAD_NAME,
                                 imsSanctionedProjectDetails.IMS_ROAD_FROM,
                                 imsSanctionedProjectDetails.IMS_ROAD_TO,
                                 imsSanctionedProjectDetails.IMS_YEAR,
                                 imsSanctionedProjectDetails.IMS_SANCTIONED_PAV_AMT,
                                 imsSanctionedProjectDetails.IMS_SANCTIONED_CD_AMT,
                                 imsSanctionedProjectDetails.IMS_SANCTIONED_PW_AMT,
                                 imsSanctionedProjectDetails.IMS_SANCTIONED_OW_AMT,
                                 imsSanctionedProjectDetails.IMS_SANCTIONED_BW_AMT,
                                 imsSanctionedProjectDetails.IMS_SANCTIONED_RS_AMT,
                                 imsSanctionedProjectDetails.IMS_SANCTIONED_BS_AMT,

                                 imsSanctionedProjectDetails.IMS_SANCTIONED_HS_AMT,
                                 imsSanctionedProjectDetails.IMS_SANCTIONED_FC_AMT,

                                 imsSanctionedProjectDetails.IMS_SANCTIONED_MAN_AMT1,
                                 imsSanctionedProjectDetails.IMS_SANCTIONED_MAN_AMT2,
                                 imsSanctionedProjectDetails.IMS_SANCTIONED_MAN_AMT3,
                                 imsSanctionedProjectDetails.IMS_SANCTIONED_MAN_AMT4,
                                 imsSanctionedProjectDetails.IMS_SANCTIONED_MAN_AMT5,
                                 imsSanctionedProjectDetails.IMS_LOCK_STATUS,
                                 imsSanctionedProjectDetails.IMS_SANCTIONED,
                                 imsSanctionedProjectDetails.IMS_PACKAGE_ID,
                                 imsSanctionedProjectDetails.IMS_PROPOSAL_TYPE,
                                 imsSanctionedProjectDetails.IMS_PAV_LENGTH,
                                 fundingAgency.MAST_FUNDING_AGENCY_NAME,
                                 imsSanctionedProjectDetails.IMS_BATCH,
                                 imsSanctionedProjectDetails.MASTER_BLOCK.MAST_BLOCK_NAME
                             }).Distinct();



                totalRecords = query == null ? 0 : query.Count();


                if (sidx.Trim() != string.Empty)
                {
                    if (sord.ToString() == "asc")
                    {
                        switch (sidx)
                        {
                            case "RoadName":
                                query = query.OrderBy(x => x.IMS_ROAD_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows));
                                break;
                            case "SanctionedYear":
                                query = query.OrderBy(x => x.IMS_YEAR).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows));
                                break;
                            case "RoadLength":
                                query = query.OrderBy(x => x.IMS_PAV_LENGTH).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows));
                                break;
                            case "Collaboration":
                                query = query.OrderBy(x => x.MAST_FUNDING_AGENCY_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows));
                                break;
                            case "Package":
                                query = query.OrderBy(x => x.IMS_PACKAGE_ID).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows));
                                break;
                            default:
                                query = query.OrderBy(x => x.IMS_YEAR).ThenBy(x => x.IMS_PACKAGE_ID).ThenBy(x => x.IMS_ROAD_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows));
                                break;

                        }


                    }
                    else
                    {
                        switch (sidx)
                        {
                            case "RoadName":
                                query = query.OrderByDescending(x => x.IMS_ROAD_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows));
                                break;
                            case "SanctionedYear":
                                query = query.OrderByDescending(x => x.IMS_YEAR).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows));
                                break;
                            case "RoadLength":
                                query = query.OrderByDescending(x => x.IMS_PAV_LENGTH).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows));
                                break;
                            case "Collaboration":
                                query = query.OrderByDescending(x => x.MAST_FUNDING_AGENCY_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows));
                                break;
                            case "Package":
                                query = query.OrderByDescending(x => x.IMS_PACKAGE_ID).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows));
                                break;
                            default:
                                query = query.OrderByDescending(x => x.IMS_ROAD_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows));
                                break;
                        }

                    }
                }
                else
                {
                    query = query.OrderBy(x => x.IMS_YEAR).ThenBy(x => x.IMS_PACKAGE_ID).ThenBy(x => x.IMS_ROAD_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows));
                }

                var result = query.Select(imsSanctionedProjectDetails => new
                {
                    imsSanctionedProjectDetails.IMS_PR_ROAD_CODE,
                    imsSanctionedProjectDetails.IMS_ROAD_NAME,
                    imsSanctionedProjectDetails.IMS_ROAD_FROM,
                    imsSanctionedProjectDetails.IMS_ROAD_TO,
                    imsSanctionedProjectDetails.IMS_YEAR,
                    imsSanctionedProjectDetails.IMS_SANCTIONED_PAV_AMT,
                    imsSanctionedProjectDetails.IMS_SANCTIONED_CD_AMT,
                    imsSanctionedProjectDetails.IMS_SANCTIONED_PW_AMT,
                    imsSanctionedProjectDetails.IMS_SANCTIONED_OW_AMT,
                    imsSanctionedProjectDetails.IMS_SANCTIONED_BW_AMT,
                    imsSanctionedProjectDetails.IMS_SANCTIONED_RS_AMT,
                    imsSanctionedProjectDetails.IMS_SANCTIONED_BS_AMT,

                    imsSanctionedProjectDetails.IMS_SANCTIONED_HS_AMT,
                    imsSanctionedProjectDetails.IMS_SANCTIONED_FC_AMT,

                    imsSanctionedProjectDetails.IMS_SANCTIONED_MAN_AMT1,
                    imsSanctionedProjectDetails.IMS_SANCTIONED_MAN_AMT2,
                    imsSanctionedProjectDetails.IMS_SANCTIONED_MAN_AMT3,
                    imsSanctionedProjectDetails.IMS_SANCTIONED_MAN_AMT4,
                    imsSanctionedProjectDetails.IMS_SANCTIONED_MAN_AMT5,
                    imsSanctionedProjectDetails.IMS_LOCK_STATUS,
                    imsSanctionedProjectDetails.IMS_SANCTIONED,
                    imsSanctionedProjectDetails.IMS_PACKAGE_ID,
                    imsSanctionedProjectDetails.IMS_PROPOSAL_TYPE,
                    imsSanctionedProjectDetails.IMS_PAV_LENGTH,
                    imsSanctionedProjectDetails.MAST_FUNDING_AGENCY_NAME,
                    imsSanctionedProjectDetails.IMS_BATCH,
                    imsSanctionedProjectDetails.MAST_BLOCK_NAME
                }).ToArray();


                return result.Select(imsSanctionedProjectDetails => new
                {

                    cell = new[] {                                                                               
                                    imsSanctionedProjectDetails.MAST_BLOCK_NAME == null ? "-" :imsSanctionedProjectDetails.MAST_BLOCK_NAME.ToString(),            
                                    imsSanctionedProjectDetails.IMS_YEAR.ToString()+"-"+(imsSanctionedProjectDetails.IMS_YEAR+1).ToString() ,
                                    imsSanctionedProjectDetails.IMS_BATCH == null ? "-" : ("Batch -" + imsSanctionedProjectDetails.IMS_BATCH).ToString(),
                                    imsSanctionedProjectDetails.IMS_PACKAGE_ID,                
                                    imsSanctionedProjectDetails.IMS_ROAD_NAME,
                                    imsSanctionedProjectDetails.IMS_PAV_LENGTH.ToString(),
                                    imsSanctionedProjectDetails.MAST_FUNDING_AGENCY_NAME==null?"NA":imsSanctionedProjectDetails.MAST_FUNDING_AGENCY_NAME.Trim(),                                                              
                                    //imsSanctionedProjectDetails.IMS_PROPOSAL_TYPE=="P"? 
                                    //(imsSanctionedProjectDetails.IMS_SANCTIONED_PAV_AMT+imsSanctionedProjectDetails.IMS_SANCTIONED_CD_AMT+imsSanctionedProjectDetails.IMS_SANCTIONED_PW_AMT+
                                    //imsSanctionedProjectDetails.IMS_SANCTIONED_OW_AMT+imsSanctionedProjectDetails.IMS_SANCTIONED_RS_AMT).ToString() : (imsSanctionedProjectDetails.IMS_SANCTIONED_BW_AMT+imsSanctionedProjectDetails.IMS_SANCTIONED_BS_AMT).ToString(),    

                                    ///Change made by SAMMED PATIL on 29MAR2016 
                                    PMGSYSession.Current.PMGSYScheme == 1 ? 
                                                      ((imsSanctionedProjectDetails.IMS_SANCTIONED_PAV_AMT == null ? 0 : imsSanctionedProjectDetails.IMS_SANCTIONED_PAV_AMT) + (imsSanctionedProjectDetails.IMS_SANCTIONED_PW_AMT == null ? 0 : imsSanctionedProjectDetails.IMS_SANCTIONED_PW_AMT) + (imsSanctionedProjectDetails.IMS_SANCTIONED_OW_AMT == null ? 0 : imsSanctionedProjectDetails.IMS_SANCTIONED_OW_AMT) + (imsSanctionedProjectDetails.IMS_SANCTIONED_CD_AMT == null ? 0 : imsSanctionedProjectDetails.IMS_SANCTIONED_CD_AMT) + (imsSanctionedProjectDetails.IMS_SANCTIONED_BW_AMT == null ? 0 : imsSanctionedProjectDetails.IMS_SANCTIONED_BW_AMT) + (imsSanctionedProjectDetails.IMS_SANCTIONED_BS_AMT == null ? 0 : imsSanctionedProjectDetails.IMS_SANCTIONED_BS_AMT) + (imsSanctionedProjectDetails.IMS_SANCTIONED_RS_AMT == null ? 0 : imsSanctionedProjectDetails.IMS_SANCTIONED_RS_AMT)).ToString()
                                                    : ((imsSanctionedProjectDetails.IMS_SANCTIONED_PAV_AMT == null ? 0 : imsSanctionedProjectDetails.IMS_SANCTIONED_PAV_AMT) + (imsSanctionedProjectDetails.IMS_SANCTIONED_PW_AMT == null ? 0 : imsSanctionedProjectDetails.IMS_SANCTIONED_PW_AMT) + (imsSanctionedProjectDetails.IMS_SANCTIONED_OW_AMT == null ? 0 : imsSanctionedProjectDetails.IMS_SANCTIONED_OW_AMT) + (imsSanctionedProjectDetails.IMS_SANCTIONED_CD_AMT == null ? 0 : imsSanctionedProjectDetails.IMS_SANCTIONED_CD_AMT) + (imsSanctionedProjectDetails.IMS_SANCTIONED_FC_AMT == null ? 0 : imsSanctionedProjectDetails.IMS_SANCTIONED_FC_AMT) + (imsSanctionedProjectDetails.IMS_SANCTIONED_HS_AMT == null ? 0 : imsSanctionedProjectDetails.IMS_SANCTIONED_HS_AMT) + (imsSanctionedProjectDetails.IMS_SANCTIONED_BW_AMT == null ? 0 : imsSanctionedProjectDetails.IMS_SANCTIONED_BW_AMT) + (imsSanctionedProjectDetails.IMS_SANCTIONED_BS_AMT == null ? 0 : imsSanctionedProjectDetails.IMS_SANCTIONED_BS_AMT)).ToString(),

                                    (imsSanctionedProjectDetails.IMS_SANCTIONED_MAN_AMT1+imsSanctionedProjectDetails.IMS_SANCTIONED_MAN_AMT2+imsSanctionedProjectDetails.IMS_SANCTIONED_MAN_AMT3+imsSanctionedProjectDetails.IMS_SANCTIONED_MAN_AMT4+imsSanctionedProjectDetails.IMS_SANCTIONED_MAN_AMT5).ToString(),                                    
                                    URLEncrypt.EncryptParameters1(new string[] { "IMSPRRoadCode =" + imsSanctionedProjectDetails.IMS_PR_ROAD_CODE.ToString(),"IMSRoadName =" + imsSanctionedProjectDetails.IMS_ROAD_NAME.ToString().Replace('/','_'),"SanctionedYear =" +  imsSanctionedProjectDetails.IMS_YEAR.ToString()+"-"+(imsSanctionedProjectDetails.IMS_YEAR+1).ToString(),"Package="+imsSanctionedProjectDetails.IMS_PACKAGE_ID   }),
                                    URLEncrypt.EncryptParameters1(new string[] { "ProposalCode =" + imsSanctionedProjectDetails.IMS_PR_ROAD_CODE.ToString(),"Operation="+"M"}),
                                    imsSanctionedProjectDetails.IMS_PR_ROAD_CODE.ToString(),
                                    //"RoadLength="+imsSanctionedProjectDetails.IMS_PAV_LENGTH.ToString()
                                    URLEncrypt.EncryptParameters1(new string[] { "ProposalCode =" + imsSanctionedProjectDetails.IMS_PR_ROAD_CODE.ToString() }),
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


        public Array GetAgreementDetailsListBAL_Proposal(int IMSPRRoadCode, int page, int rows, string sidx, string sord, out long totalRecords)
        {
            PMGSYEntities dbContext = new PMGSYEntities();

            try
            {
                int stateCode = PMGSY.Extensions.PMGSYSession.Current.StateCode;
                int districtCode = PMGSY.Extensions.PMGSYSession.Current.DistrictCode;

                var query = from IMSContract in dbContext.MANE_IMS_CONTRACT
                            join contractorDetails in dbContext.MASTER_CONTRACTOR
                            on IMSContract.MAST_CON_ID equals contractorDetails.MAST_CON_ID //into contractors
                            //from contractorDetails in contractors.DefaultIfEmpty()
                            where
                            IMSContract.IMS_PR_ROAD_CODE == IMSPRRoadCode &&
                            IMSContract.MANE_CONTRACT_STATUS != "I"
                            select new
                            {

                                IMSContract.IMS_PR_ROAD_CODE,
                                IMSContract.MANE_PR_CONTRACT_CODE,
                                contractorDetails.MAST_CON_COMPANY_NAME,
                                IMSContract.MANE_AGREEMENT_NUMBER,
                                IMSContract.MANE_AGREEMENT_DATE,
                                IMSContract.MANE_MAINTENANCE_START_DATE,
                                IMSContract.MANE_YEAR1_AMOUNT,
                                IMSContract.MANE_YEAR2_AMOUNT,
                                IMSContract.MANE_YEAR3_AMOUNT,
                                IMSContract.MANE_YEAR4_AMOUNT,
                                IMSContract.MANE_YEAR5_AMOUNT,
                                IMSContract.MANE_CONTRACT_FINALIZED,
                                IMSContract.MANE_LOCK_STATUS,
                                IMSContract.MANE_CONTRACT_STATUS


                            };



                totalRecords = query == null ? 0 : query.Count();


                if (sidx.Trim() != string.Empty)
                {
                    if (sord.ToString() == "asc")
                    {
                        switch (sidx)
                        {
                            case "ContractorName":
                                query = query.OrderBy(x => x.MAST_CON_COMPANY_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows));
                                break;
                            case "MaintenanceDate":
                                query = query.OrderBy(x => x.MANE_MAINTENANCE_START_DATE).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows));
                                break;
                            case "AgreementNumber":
                                query = query.OrderBy(x => x.MANE_AGREEMENT_NUMBER).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows));
                                break;
                            case "AgreementDate":
                                query = query.OrderBy(x => x.MANE_AGREEMENT_DATE).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows));
                                break;
                            default:
                                query = query.OrderBy(x => x.MAST_CON_COMPANY_NAME).ThenBy(x => x.MANE_AGREEMENT_NUMBER).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows));
                                break;

                        }


                    }
                    else
                    {
                        switch (sidx)
                        {
                            case "ContractorName":
                                query = query.OrderByDescending(x => x.MAST_CON_COMPANY_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows));
                                break;
                            case "MaintenanceDate":
                                query = query.OrderByDescending(x => x.MANE_MAINTENANCE_START_DATE).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows));
                                break;
                            case "AgreementNumber":
                                query = query.OrderByDescending(x => x.MANE_AGREEMENT_NUMBER).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows));
                                break;
                            case "AgreementDate":
                                query = query.OrderByDescending(x => x.MANE_AGREEMENT_DATE).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows));
                                break;
                            default:
                                query = query.OrderByDescending(x => x.MAST_CON_COMPANY_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows));
                                break;

                        }

                    }
                }
                else
                {
                    query = query.OrderBy(x => x.MAST_CON_COMPANY_NAME).ThenBy(x => x.MANE_AGREEMENT_NUMBER).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows));
                }

                var result = query.Select(IMSContract => new
                {

                    IMSContract.IMS_PR_ROAD_CODE,
                    IMSContract.MANE_PR_CONTRACT_CODE,
                    IMSContract.MAST_CON_COMPANY_NAME,
                    IMSContract.MANE_AGREEMENT_NUMBER,
                    IMSContract.MANE_AGREEMENT_DATE,
                    IMSContract.MANE_MAINTENANCE_START_DATE,
                    IMSContract.MANE_YEAR1_AMOUNT,
                    IMSContract.MANE_YEAR2_AMOUNT,
                    IMSContract.MANE_YEAR3_AMOUNT,
                    IMSContract.MANE_YEAR4_AMOUNT,
                    IMSContract.MANE_YEAR5_AMOUNT,
                    IMSContract.MANE_CONTRACT_FINALIZED,
                    IMSContract.MANE_LOCK_STATUS,
                    IMSContract.MANE_CONTRACT_STATUS

                }).ToArray();


                return result.Select(IMSContract => new
                {
                    //id = tendAgreementMaster.TEND_AGREEMENT_CODE.ToString().Trim(),
                    cell = new[] {                                                                               
                                                                  
                                    IMSContract.MANE_AGREEMENT_NUMBER.ToString(),
                                    IMSContract.MAST_CON_COMPANY_NAME==null?"NA":IMSContract.MAST_CON_COMPANY_NAME.ToString().Trim(),                            
                                    Convert.ToDateTime(IMSContract.MANE_AGREEMENT_DATE).ToString("dd/MM/yyyy"),
                                    Convert.ToDateTime(IMSContract.MANE_MAINTENANCE_START_DATE).ToString("dd/MM/yyyy"),
                                       
                                    ((IMSContract.MANE_YEAR1_AMOUNT==null?0:IMSContract.MANE_YEAR1_AMOUNT)+
                                       (IMSContract.MANE_YEAR2_AMOUNT==null?0:IMSContract.MANE_YEAR2_AMOUNT)+
                                       (IMSContract.MANE_YEAR3_AMOUNT==null?0:IMSContract.MANE_YEAR3_AMOUNT)+
                                       (IMSContract.MANE_YEAR4_AMOUNT==null?0:IMSContract.MANE_YEAR4_AMOUNT)+
                                       (IMSContract.MANE_YEAR5_AMOUNT==null?0:IMSContract.MANE_YEAR5_AMOUNT)
                                    ).ToString(),
                                    AgreementStatus[IMSContract.MANE_CONTRACT_STATUS].ToString(),
                                    (IMSContract.MANE_CONTRACT_STATUS =="I")?string.Empty:URLEncrypt.EncryptParameters1(new string[] { "IMSPRRoadCode =" + IMSContract.IMS_PR_ROAD_CODE.ToString(),"PRContractCode =" + IMSContract.MANE_PR_CONTRACT_CODE.ToString() }),
                                    (IMSContract.MANE_CONTRACT_FINALIZED=="N" && IMSContract.MANE_LOCK_STATUS=="N") ?"<center><table><tr><td  style='border-color:white'><span class='ui-icon ui-icon-unlocked' title='Finalize Agreement' onClick ='FinalizeAgreement(\"" + URLEncrypt.EncryptParameters1(new string[]{"IMSPRRoadCode="+IMSContract.IMS_PR_ROAD_CODE.ToString(),"PRContractCode =" + IMSContract.MANE_PR_CONTRACT_CODE.ToString()  }) + "\");' ></span></td></tr></table></center>": "<center><table><tr><td  style='border-color:white'><span class='ui-icon ui-icon-locked' title='Finalized'></span></td></tr></table></center>",
                                    (IMSContract.MANE_CONTRACT_FINALIZED=="Y"||IMSContract.MANE_LOCK_STATUS=="Y")?string.Empty:URLEncrypt.EncryptParameters1(new string[] { "IMSPRRoadCode =" + IMSContract.IMS_PR_ROAD_CODE.ToString(),"PRContractCode =" + IMSContract.MANE_PR_CONTRACT_CODE.ToString()  }),
                                    (IMSContract.MANE_CONTRACT_FINALIZED=="Y"||IMSContract.MANE_LOCK_STATUS=="Y")?string.Empty:URLEncrypt.EncryptParameters1(new string[] { "IMSPRRoadCode =" + IMSContract.IMS_PR_ROAD_CODE.ToString(),"PRContractCode =" + IMSContract.MANE_PR_CONTRACT_CODE.ToString()})
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

        public bool SaveInspectionDetails(MaintainanceInspectionViewModel inspectionModel, ref string message)
        {
            PMGSYEntities dbContext = new PMGSYEntities();
            try
            {
                MANE_IMS_INSPECTION inspectionDetails = new MANE_IMS_INSPECTION();

                CommonFunctions obj = new CommonFunctions();

                if (obj.GetStringToDateTime(inspectionModel.MANE_INSP_DATE) < getInspectionDate(inspectionModel.IMS_PR_ROAD_CODE))
                {
                    message = "Inspection date should be greater than maintenance start date.";
                    return false;
                }
                //if (dbContext.MANE_IMS_INSPECTION.Where(m => m.IMS_PR_ROAD_CODE == inspectionModel.IMS_PR_ROAD_CODE).Any())
                //{

                //    DateTime previousDate = dbContext.MANE_IMS_INSPECTION.Where(m => m.IMS_PR_ROAD_CODE == inspectionModel.IMS_PR_ROAD_CODE).Max(m => m.MANE_INSP_DATE);
                //    inspectionModel.InspectionDate =ConvertDateToString(previousDate);

                //    if (previousDate == ConvertStringToDate(inspectionModel.MANE_INSP_DATE))
                //    {
                //        message = "Please select inspection date grater than last inspection date.";
                //        return false;
                //    }    
                //}
                inspectionDetails.IMS_PR_ROAD_CODE = inspectionModel.IMS_PR_ROAD_CODE;
                inspectionDetails.IMS_INSPECTION_CODE = GetMaxCode(inspectionModel);
                inspectionDetails.MAST_OFFICER_CODE = inspectionModel.MAST_OFFICER_CODE;
                inspectionDetails.MANE_INSP_DATE = (System.DateTime)ConvertStringToDate(inspectionModel.MANE_INSP_DATE);
                inspectionDetails.MANE_RECTIFICATION_DATE = ConvertStringToDate(inspectionModel.MANE_RECTIFICATION_DATE);
                inspectionDetails.MANE_RECTIFICATION_STATUS = "P";
                inspectionDetails.MANE_REMARKS = inspectionModel.MANE_REMARKS;
                dbContext.MANE_IMS_INSPECTION.Add(inspectionDetails);
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

        public string LastInspectionDate(int prRoadCode)
        {
            PMGSYEntities dbContext = new PMGSYEntities();
            //MaintainanceInspectionViewModel inspectionModel = null;
            try
            {
                MANE_IMS_INSPECTION inspectionDetails = new MANE_IMS_INSPECTION();
                //DateTime previousDate=;
                if (dbContext.MANE_IMS_INSPECTION.Where(m => m.IMS_PR_ROAD_CODE == prRoadCode).Any())
                {
                    DateTime previousDate = dbContext.MANE_IMS_INSPECTION.Where(m => m.IMS_PR_ROAD_CODE == prRoadCode).Max(m => m.MANE_INSP_DATE).AddDays(1);

                    string nextDate = ConvertDateToString(previousDate);
                    return nextDate;

                }

                return "";
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

        public bool EditInspectionDetails(MaintainanceInspectionViewModel inspectionModel, ref string message)
        {

            PMGSYEntities dbContext = new PMGSYEntities();
            CommonFunctions obj = new CommonFunctions();
            try
            {
                MANE_IMS_INSPECTION inspectionDetails = dbContext.MANE_IMS_INSPECTION.Find(inspectionModel.IMS_PR_ROAD_CODE, inspectionModel.IMS_INSPECTION_CODE);
                if (obj.GetStringToDateTime(inspectionModel.MANE_INSP_DATE) < getInspectionDate(inspectionModel.IMS_PR_ROAD_CODE))
                {
                    message = "Inspection date should be greater than maintenance start date.";
                    return false;
                }
                //if (dbContext.MANE_IMS_INSPECTION.Where(m => m.IMS_PR_ROAD_CODE == inspectionModel.IMS_PR_ROAD_CODE).Any())
                //{

                //    DateTime previousDate = dbContext.MANE_IMS_INSPECTION.Where(m => m.IMS_PR_ROAD_CODE == inspectionModel.IMS_PR_ROAD_CODE).Max(m => m.MANE_INSP_DATE);
                //    inspectionModel.InspectionDate = ConvertDateToString(previousDate);

                //    if (previousDate > ConvertStringToDate(inspectionModel.MANE_INSP_DATE))
                //    {
                //        message = "Please select inspection date grater than last inspection date.";
                //        return false;
                //    }
                //}
                inspectionDetails.IMS_PR_ROAD_CODE = inspectionModel.IMS_PR_ROAD_CODE;
                inspectionDetails.IMS_INSPECTION_CODE = inspectionModel.IMS_INSPECTION_CODE;
                inspectionDetails.MAST_OFFICER_CODE = inspectionModel.MAST_OFFICER_CODE;
                inspectionDetails.MANE_INSP_DATE = (System.DateTime)ConvertStringToDate(inspectionModel.MANE_INSP_DATE);
                inspectionDetails.MANE_RECTIFICATION_DATE = ConvertStringToDate(inspectionModel.MANE_RECTIFICATION_DATE);

                //modofied by abhishek kamble 21-nov-2013
                if (inspectionModel.statusFlag == "true")
                {
                    inspectionDetails.MANE_RECTIFICATION_STATUS = "C";
                }
                else
                {
                    inspectionDetails.MANE_RECTIFICATION_STATUS = inspectionModel.MANE_RECTIFICATION_STATUS;
                    //inspectionDetails.MANE_RECTIFICATION_STATUS = "P";
                }
                inspectionDetails.MANE_REMARKS = inspectionModel.MANE_REMARKS;
                dbContext.Entry(inspectionDetails).State = System.Data.Entity.EntityState.Modified;
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

        public bool DeleteInspectionDetails(int prRoadCode, int inspectionCode)
        {
            PMGSYEntities dbContext = new PMGSYEntities();
            try
            {
                MANE_IMS_INSPECTION inspectiondetails = new MANE_IMS_INSPECTION();
                inspectiondetails = dbContext.MANE_IMS_INSPECTION.Where(m => m.IMS_PR_ROAD_CODE == prRoadCode && m.IMS_INSPECTION_CODE == inspectionCode).FirstOrDefault();
                if (inspectiondetails == null)
                {
                    return false;
                }
                dbContext.MANE_IMS_INSPECTION.Remove(inspectiondetails);
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

        public MaintainanceInspectionViewModel GetMaintainanceInspection_ByRoadCode(int prRoadCode, int inspectionCode)
        {
            PMGSYEntities dbContext = new PMGSYEntities();
            try
            {
                MANE_IMS_INSPECTION inspectionDetails = new MANE_IMS_INSPECTION();
                inspectionDetails = dbContext.MANE_IMS_INSPECTION.FirstOrDefault(m => m.IMS_PR_ROAD_CODE == prRoadCode && m.IMS_INSPECTION_CODE == inspectionCode);



                MaintainanceInspectionViewModel maintainanceInspection = new MaintainanceInspectionViewModel();

                IMS_SANCTIONED_PROJECTS sactionedProjects = dbContext.IMS_SANCTIONED_PROJECTS.Where(m => m.IMS_PR_ROAD_CODE == prRoadCode).FirstOrDefault();

                var designation = dbContext.ADMIN_NODAL_OFFICERS.Where(m => m.ADMIN_NO_OFFICER_CODE == inspectionDetails.MAST_OFFICER_CODE).Select(m => m.ADMIN_NO_DESIGNATION).FirstOrDefault();
                maintainanceInspection.RoadName = sactionedProjects.IMS_ROAD_NAME;
                maintainanceInspection.SactionedYear = sactionedProjects.IMS_YEAR.ToString();
                maintainanceInspection.Package = sactionedProjects.IMS_PACKAGE_ID;
                if (inspectionDetails != null)
                {
                    // maintainanceInspection.EncryptedIMSPRRoadCode = URLEncrypt.EncryptParameters1(new string[] { encryptedParameters[0], encryptedParameters[1], encryptedParameters[2] });
                    maintainanceInspection.IMS_PR_ROAD_CODE = prRoadCode;
                    maintainanceInspection.IMS_INSPECTION_CODE = inspectionCode;
                    maintainanceInspection.MANE_RECTIFICATION_STATUS = inspectionDetails.MANE_RECTIFICATION_STATUS;
                    maintainanceInspection.MAST_OFFICER_CODE = inspectionDetails.MAST_OFFICER_CODE;
                    maintainanceInspection.Designation = designation.ToString();
                    maintainanceInspection.MANE_REMARKS = inspectionDetails.MANE_REMARKS;
                    maintainanceInspection.MANE_INSP_DATE = ConvertDateToString(inspectionDetails.MANE_INSP_DATE);
                    maintainanceInspection.MANE_RECTIFICATION_DATE = inspectionDetails.MANE_RECTIFICATION_DATE == null ? string.Empty : ConvertDateToString(inspectionDetails.MANE_RECTIFICATION_DATE);
                }
                return maintainanceInspection;

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

        public DateTime? ConvertStringToDate(string dateToConvert)
        {

            if (dateToConvert != null)
            {

                DateTime MyDateTime;
                MyDateTime = new DateTime();
                MyDateTime = DateTime.ParseExact(dateToConvert, "dd/MM/yyyy",
                                                 null);

                return MyDateTime;
            }
            else
            {
                return null;
            }
        }

        public string ConvertDateToString(DateTime? date)
        {
            return Convert.ToDateTime(date).ToString("dd/MM/yyyy");
        }


        public List<SelectListItem> GetAdminNdName(string desigCode)
        {
            PMGSYEntities dbContext = new PMGSYEntities();
            try
            {
                int designationCode = Convert.ToInt32(desigCode);

                // adminNdCode = PMGSYSession.Current.AdminNdCode;
                List<SelectListItem> lstadminNames = new List<SelectListItem>();

                var list = (from AdminName in dbContext.ADMIN_NODAL_OFFICERS
                            //where AdminName.ADMIN_NO_DESIGNATION == designationCode && AdminName.ADMIN_ND_CODE == PMGSYSession.Current.AdminNdCode && AdminName.ADMIN_ACTIVE_STATUS == "Y"
                            where AdminName.ADMIN_NO_DESIGNATION == designationCode && AdminName.ADMIN_ND_CODE == PMGSYSession.Current.ParentNDCode && AdminName.ADMIN_ACTIVE_STATUS == "Y"
                            select new
                            {
                                AdminName.ADMIN_NO_FNAME,
                                AdminName.ADMIN_NO_MNAME,
                                AdminName.ADMIN_NO_LNAME,
                                AdminName.ADMIN_ND_CODE,
                                AdminName.ADMIN_NO_OFFICER_CODE
                            });
                foreach (var item in list)
                {
                    lstadminNames.Add(new SelectListItem { Value = item.ADMIN_NO_OFFICER_CODE.ToString().Trim(), Text = item.ADMIN_NO_FNAME.Trim() + " " + item.ADMIN_NO_MNAME.Trim() + " " + item.ADMIN_NO_LNAME.Trim() });

                }
                lstadminNames.Insert(0, (new SelectListItem { Text = "--Select--", Value = "0", Selected = true }));
                return lstadminNames;


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

        public MaintainanceInspectionViewModel GetInspectionDetails(int proposalCode)
        {
            PMGSYEntities dbContext = new PMGSYEntities();
            try
            {
                MaintainanceInspectionViewModel model = new MaintainanceInspectionViewModel();
                IMS_SANCTIONED_PROJECTS master = dbContext.IMS_SANCTIONED_PROJECTS.Find(proposalCode);
                if (master != null)
                {
                    model.BlockName = (dbContext.MASTER_BLOCK.Where(m => m.MAST_BLOCK_CODE == master.MAST_BLOCK_CODE).Select(m => m.MAST_BLOCK_NAME).FirstOrDefault());
                    model.Package = master.IMS_PACKAGE_ID.ToString();
                    model.RoadName = master.IMS_ROAD_NAME;
                    model.SactionedYear = master.IMS_YEAR + "-" + ((master.IMS_YEAR) + 1);
                    //  model.Sanction_Cost = Convert.ToDouble(master.IMS_SANCTIONED_BS_AMT + master.IMS_SANCTIONED_BW_AMT + master.IMS_SANCTIONED_CD_AMT + master.IMS_SANCTIONED_OW_AMT + master.IMS_SANCTIONED_PAV_AMT + master.IMS_SANCTIONED_PW_AMT + master.IMS_SANCTIONED_RS_AMT);
                    //  model.Sanction_length = master.IMS_PAV_LENGTH;
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
        public Array GetInspectionRoadList(int proposalCode, int page, int rows, string sidx, string sord, out long totalRecords)
        {
            PMGSYEntities dbContext = new PMGSYEntities();
            try
            {
                var list = (from item in dbContext.MANE_IMS_INSPECTION
                            // join item1 in dbContext.ADMIN_NODAL_OFFICERS on item.MAST_OFFICER_CODE equals item1.ADMIN_ND_CODE
                            where item.IMS_PR_ROAD_CODE == proposalCode

                            select item
                                ).ToList();

                totalRecords = list.Count();
                if (sidx.Trim() != string.Empty)
                {
                    if (sord.ToString() == "asc")
                    {
                        switch (sidx)
                        {
                            case "Name":
                                list = list.OrderBy(m => m.MAST_OFFICER_CODE).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                                break;
                            case "InspectionDate":
                                list = list.OrderBy(m => m.MANE_INSP_DATE).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                                break;
                            case "RectificationDate":
                                list = list.OrderBy(m => m.MANE_RECTIFICATION_DATE).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                                break;
                            case "Status":
                                list = list.OrderBy(m => m.MANE_RECTIFICATION_STATUS).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                                break;
                            case "Designation":
                                list = list.OrderBy(m => m.MAST_OFFICER_CODE).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                                break;
                        }

                    }
                    else
                    {
                        switch (sidx)
                        {
                            case "Name":
                                list = list.OrderByDescending(m => m.MAST_OFFICER_CODE).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                                break;
                            case "InspectionDate":
                                list = list.OrderByDescending(m => m.MANE_INSP_DATE).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                                break;
                            case "RectificationDate":
                                list = list.OrderByDescending(m => m.MANE_RECTIFICATION_DATE).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                                break;
                            case "Status":
                                list = list.OrderByDescending(m => m.MANE_RECTIFICATION_STATUS).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                                break;
                            case "Designation":
                                list = list.OrderByDescending(m => m.MAST_OFFICER_CODE).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                                break;
                        }
                    }
                }

                return list.Select(imsSanctionedProjectDetails => new
                {

                    cell = new[] {                                                                               
                                    imsSanctionedProjectDetails.ADMIN_NODAL_OFFICERS.MASTER_DESIGNATION.MAST_DESIG_NAME,
                                    imsSanctionedProjectDetails.ADMIN_NODAL_OFFICERS.ADMIN_NO_FNAME+""+imsSanctionedProjectDetails.ADMIN_NODAL_OFFICERS.ADMIN_NO_MNAME+""+imsSanctionedProjectDetails.ADMIN_NODAL_OFFICERS.ADMIN_NO_LNAME,
                                    Convert.ToDateTime(imsSanctionedProjectDetails.MANE_INSP_DATE).ToString("dd/MM/yyyy"),
                                    imsSanctionedProjectDetails.MANE_RECTIFICATION_DATE==null?"NA": Convert.ToDateTime(imsSanctionedProjectDetails.MANE_RECTIFICATION_DATE).ToString("dd/MM/yyyy"),
                                    imsSanctionedProjectDetails.MANE_RECTIFICATION_STATUS=="P"?"Progress": (imsSanctionedProjectDetails.MANE_RECTIFICATION_STATUS=="C"?"Completed":string.Empty),
                                    URLEncrypt.EncryptParameters1(new string[] { "IMSPRRoadCode =" + imsSanctionedProjectDetails.IMS_PR_ROAD_CODE.ToString(),"IMSInspectionCode =" + imsSanctionedProjectDetails.IMS_INSPECTION_CODE.ToString(),"Flag="+string.Empty}),//"RoadLength="+imsSanctionedProjectDetails.IMS_PAV_LENGTH.ToString()
                                    URLEncrypt.EncryptParameters1(new string[] { "IMSPRRoadCode =" + imsSanctionedProjectDetails.IMS_PR_ROAD_CODE.ToString(),"IMSInspectionCode =" + imsSanctionedProjectDetails.IMS_INSPECTION_CODE.ToString(),"Flag="+string.Empty}),
                                    URLEncrypt.EncryptParameters1(new string[] { "IMSPRRoadCode =" + imsSanctionedProjectDetails.IMS_PR_ROAD_CODE.ToString(),"IMSInspectionCode =" + imsSanctionedProjectDetails.IMS_INSPECTION_CODE.ToString(),"Flag="+"true"})

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

        public List<SelectListItem> PopulateDesignation(bool isPopulateFirstItem = true)
        {
            PMGSYEntities dbContext = new PMGSYEntities();
            try
            {
                List<SelectListItem> lstDesignation = new SelectList(dbContext.MASTER_DESIGNATION.Where(m => m.MAST_DESIG_CODE == 118 || m.MAST_DESIG_CODE == 23 || m.MAST_DESIG_CODE == 85 || m.MAST_DESIG_CODE == 170 || m.MAST_DESIG_CODE == 37), "MAST_DESIG_CODE", "MAST_DESIG_NAME").ToList();
                if (isPopulateFirstItem)
                {
                    lstDesignation.Insert(0, (new SelectListItem { Text = "--Select--", Value = "0", Selected = true }));
                }
                return lstDesignation;
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

        public DateTime? getInspectionDate(int PRRoadCode)
        {
            PMGSYEntities dbContext = new PMGSYEntities();
            try
            {
                System.DateTime objContract = dbContext.MANE_IMS_CONTRACT.Where(m => m.IMS_PR_ROAD_CODE == PRRoadCode).OrderByDescending(m => m.MANE_MAINTENANCE_START_DATE).Select(m => m.MANE_MAINTENANCE_START_DATE).FirstOrDefault();
                DateTime StartDate = objContract.AddDays(1);
                return StartDate;
            }
            catch (Exception ex)
            {
                return null;
            }
            finally
            {
                dbContext.Dispose();
            }

        }

        #endregion

        #region MAINTENANCE_FINANCIAL_DETAILS

        /// <summary>
        /// returns the progress list of financial details
        /// </summary>
        /// <param name="page">no. of pages in list</param>
        /// <param name="rows">no. of rows per page</param>
        /// <param name="sidx">sort column index</param>
        /// <param name="sord">sort order</param>
        /// <param name="totalRecords">total no. of records</param>
        /// <param name="proposalCode">list of particular id</param>
        /// <returns></returns>
        public Array GetFinancialProgressList(int page, int rows, string sidx, string sord, out long totalRecords, int proposalCode, string progressType, int contractCode)
        {
            PMGSYEntities dbContext = new PMGSYEntities();
            try
            {
                int? monthCode = 0;
                int? yearCode = 0;
                if ((dbContext.MANE_IMS_PROGRESS.Any(m => m.IMS_PR_ROAD_CODE == proposalCode)))
                {
                    yearCode = (dbContext.MANE_IMS_PROGRESS.Where(m => m.IMS_PR_ROAD_CODE == proposalCode && m.MANE_MAINTENANCE_NUMBER == contractCode).Max(y => (Int32?)y.MANE_PROG_YEAR));
                    monthCode = (dbContext.MANE_IMS_PROGRESS.Where(m => m.IMS_PR_ROAD_CODE == proposalCode && m.MANE_PROG_YEAR == yearCode && m.MANE_MAINTENANCE_NUMBER == contractCode).Max(s => (Int32?)s.MANE_PROG_MONTH));
                }

                var lstFinancialProgress = (from item in dbContext.MANE_IMS_PROGRESS
                                            where item.IMS_PR_ROAD_CODE == proposalCode &&
                                            (contractCode == 0 ? 1 : item.MANE_MAINTENANCE_NUMBER) == (contractCode == 0 ? 1 : contractCode)
                                            select new
                                            {
                                                item.MANE_FINAL_PAYMENT_DATE,
                                                item.MANE_FINAL_PAYMENT_FLAG,
                                                item.MANE_PAYMENT_LASTMONTH,
                                                item.MANE_PAYMENT_THISMONTH,
                                                item.MANE_PROG_MONTH,
                                                item.MANE_PROG_YEAR,
                                                //item.EXEC_PROGRESS_TYPE,
                                                item.MANE_TYPE,
                                                item.MANE_VALUEOFWORK_LASTMONTH,
                                                item.MANE_VALUEOFWORK_THISMONTH,
                                                item.IMS_PR_ROAD_CODE,
                                                item.MANE_MAINTENANCE_NUMBER
                                            });

                totalRecords = lstFinancialProgress.Count();

                if (sidx.Trim() != string.Empty)
                {
                    if (sord.ToString() == "asc")
                    {
                        switch (sidx)
                        {
                            case "EXEC_FINAL_PAYMENT_DATE":
                                lstFinancialProgress = lstFinancialProgress.OrderBy(m => m.MANE_FINAL_PAYMENT_DATE).ThenBy(m => m.MANE_PROG_MONTH).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                                break;
                            case "EXEC_FINAL_PAYMENT_FLAG":
                                lstFinancialProgress = lstFinancialProgress.OrderBy(m => m.MANE_FINAL_PAYMENT_FLAG).ThenBy(m => m.MANE_PROG_MONTH).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                                break;
                            case "EXEC_PAYMENT_LASTMONTH":
                                lstFinancialProgress = lstFinancialProgress.OrderBy(m => m.MANE_PAYMENT_LASTMONTH).ThenBy(m => m.MANE_PROG_MONTH).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                                break;
                            case "EXEC_PAYMENT_THISMONTH":
                                lstFinancialProgress = lstFinancialProgress.OrderBy(m => m.MANE_PAYMENT_THISMONTH).ThenBy(m => m.MANE_PROG_MONTH).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                                break;
                            case "EXEC_PROG_MONTH":
                                lstFinancialProgress = lstFinancialProgress.OrderBy(m => m.MANE_PROG_MONTH).ThenBy(m => m.MANE_PROG_MONTH).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                                break;
                            case "EXEC_PROGRAM_YEAR":
                                lstFinancialProgress = lstFinancialProgress.OrderBy(m => m.MANE_PROG_YEAR).ThenBy(m => m.MANE_PROG_MONTH).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                                break;
                            //case "EXEC_PROGRESS_TYPE":
                            //    lstFinancialProgress = lstFinancialProgress.OrderBy(m => m.EXEC_PROGRESS_TYPE).ThenBy(m => m.EXEC_PROG_MONTH).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                            //    break;
                            case "EXEC_VALUEOFWORK_LASTMONTH":
                                lstFinancialProgress = lstFinancialProgress.OrderBy(m => m.MANE_VALUEOFWORK_LASTMONTH).ThenBy(m => m.MANE_PROG_MONTH).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                                break;
                            case "EXEC_VALUEOFWORK_THISMONTH":
                                lstFinancialProgress = lstFinancialProgress.OrderBy(m => m.MANE_VALUEOFWORK_THISMONTH).ThenBy(m => m.MANE_PROG_MONTH).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                                break;
                            default:
                                lstFinancialProgress = lstFinancialProgress.OrderByDescending(m => m.MANE_PROG_YEAR).ThenByDescending(m => m.MANE_PROG_MONTH).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                                break;
                        }
                    }
                    else
                    {
                        switch (sidx)
                        {
                            case "EXEC_FINAL_PAYMENT_DATE":
                                lstFinancialProgress = lstFinancialProgress.OrderByDescending(m => m.MANE_FINAL_PAYMENT_DATE).ThenBy(m => m.MANE_PROG_MONTH).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                                break;
                            case "EXEC_FINAL_PAYMENT_FLAG":
                                lstFinancialProgress = lstFinancialProgress.OrderByDescending(m => m.MANE_FINAL_PAYMENT_FLAG).ThenBy(m => m.MANE_PROG_MONTH).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                                break;
                            case "EXEC_PAYMENT_LASTMONTH":
                                lstFinancialProgress = lstFinancialProgress.OrderByDescending(m => m.MANE_PAYMENT_LASTMONTH).ThenBy(m => m.MANE_PROG_MONTH).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                                break;
                            case "EXEC_PAYMENT_THISMONTH":
                                lstFinancialProgress = lstFinancialProgress.OrderByDescending(m => m.MANE_PAYMENT_THISMONTH).ThenBy(m => m.MANE_PROG_MONTH).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                                break;
                            case "EXEC_PROG_MONTH":
                                lstFinancialProgress = lstFinancialProgress.OrderByDescending(m => m.MANE_PROG_MONTH).ThenBy(m => m.MANE_PROG_MONTH).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                                break;
                            case "EXEC_PROGRAM_YEAR":
                                lstFinancialProgress = lstFinancialProgress.OrderByDescending(m => m.MANE_PROG_YEAR).ThenBy(m => m.MANE_PROG_MONTH).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                                break;
                            //case "EXEC_PROGRESS_TYPE":
                            //    lstFinancialProgress = lstFinancialProgress.OrderByDescending(m => m.EXEC_PROGRESS_TYPE).ThenBy(m => m.EXEC_PROG_MONTH).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                            //    break;
                            case "EXEC_VALUEOFWORK_LASTMONTH":
                                lstFinancialProgress = lstFinancialProgress.OrderByDescending(m => m.MANE_VALUEOFWORK_LASTMONTH).ThenBy(m => m.MANE_PROG_MONTH).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                                break;
                            case "EXEC_VALUEOFWORK_THISMONTH":
                                lstFinancialProgress = lstFinancialProgress.OrderByDescending(m => m.MANE_VALUEOFWORK_THISMONTH).ThenBy(m => m.MANE_PROG_MONTH).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                                break;
                            default:
                                lstFinancialProgress = lstFinancialProgress.OrderByDescending(m => m.MANE_PROG_YEAR).ThenByDescending(m => m.MANE_PROG_MONTH).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                                break;
                        }


                        //lstFinancialProgress = lstFinancialProgress.OrderByDescending(m => m.EXEC_PROG_YEAR).ThenBy(m => m.EXEC_PROG_MONTH).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                    }
                }
                else
                {
                    lstFinancialProgress = lstFinancialProgress.OrderByDescending(m => m.MANE_PROG_YEAR).ThenByDescending(m => m.MANE_PROG_MONTH).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                }

                var gridData = lstFinancialProgress.Select(progressDetails => new
                {
                    progressDetails.MANE_FINAL_PAYMENT_DATE,
                    progressDetails.MANE_FINAL_PAYMENT_FLAG,
                    progressDetails.MANE_PAYMENT_LASTMONTH,
                    progressDetails.MANE_PAYMENT_THISMONTH,
                    progressDetails.MANE_PROG_MONTH,
                    progressDetails.MANE_PROG_YEAR,
                    //progressDetails.EXEC_PROGRESS_TYPE,
                    progressDetails.MANE_TYPE,
                    progressDetails.MANE_VALUEOFWORK_LASTMONTH,
                    progressDetails.MANE_VALUEOFWORK_THISMONTH,
                    progressDetails.IMS_PR_ROAD_CODE,
                    progressDetails.MANE_MAINTENANCE_NUMBER
                }).ToArray();

                return gridData.Select(m => new
                {
                    id = m.IMS_PR_ROAD_CODE.ToString(),
                    cell = new[]
                    {
                        m.MANE_TYPE == "P" ? "Periodic" : "Routine",
                        m.MANE_PROG_YEAR.ToString(),
                        m.MANE_PROG_MONTH == 1?"January":(m.MANE_PROG_MONTH == 2?"February":(m.MANE_PROG_MONTH == 3?"March":(m.MANE_PROG_MONTH == 4?"April":(m.MANE_PROG_MONTH == 5?"May":(m.MANE_PROG_MONTH == 6?"June":(m.MANE_PROG_MONTH == 7?"July":m.MANE_PROG_MONTH == 8?"August":(m.MANE_PROG_MONTH == 9?"September":(m.MANE_PROG_MONTH == 10?"October":(m.MANE_PROG_MONTH == 11?"November":"December"))))))))),
                        m.MANE_VALUEOFWORK_LASTMONTH.ToString(),
                        m.MANE_VALUEOFWORK_THISMONTH.ToString(),
                        (m.MANE_VALUEOFWORK_LASTMONTH + m.MANE_VALUEOFWORK_THISMONTH).ToString(),
                        m.MANE_PAYMENT_LASTMONTH.ToString(),
                        m.MANE_PAYMENT_THISMONTH.ToString(),
                        (m.MANE_PAYMENT_LASTMONTH + m.MANE_PAYMENT_THISMONTH).ToString(),
                        m.MANE_FINAL_PAYMENT_FLAG=="Y"?"Yes":"No",
                        m.MANE_FINAL_PAYMENT_DATE==null?"-":Convert.ToDateTime(m.MANE_FINAL_PAYMENT_DATE).ToString("dd/MM/yyyy"),
                        (m.MANE_PROG_MONTH==monthCode && m.MANE_PROG_YEAR==yearCode)?"<a href='#' title='Click here to edit Financial Details' class='ui-icon ui-icon-pencil ui-align-center' onClick=EditFinancialProgress('" + URLEncrypt.EncryptParameters1(new string[]{"ProposalCode="+m.IMS_PR_ROAD_CODE.ToString().Trim(),"Month="+m.MANE_PROG_MONTH.ToString().Trim(),"Year="+m.MANE_PROG_YEAR.ToString().Trim(),"ContractCode="+m.MANE_MAINTENANCE_NUMBER.ToString().Trim()}) +"'); return false;'>Add Remarks</a>":"<span class='ui-icon ui-icon-locked ui-align-center'></span>",
                        (m.MANE_PROG_MONTH==monthCode && m.MANE_PROG_YEAR==yearCode)?"<a href='#' title='Click here to delete Financial Details' class='ui-icon ui-icon-trash ui-align-center' onClick=DeleteFinancialProgress('" + URLEncrypt.EncryptParameters1(new string[]{"ProposalCode="+m.IMS_PR_ROAD_CODE.ToString().Trim(),"Month="+m.MANE_PROG_MONTH.ToString().Trim(),"Year="+m.MANE_PROG_YEAR.ToString().Trim(),"ContractCode="+m.MANE_MAINTENANCE_NUMBER.ToString().Trim()}) +"'); return false;'>Add Remarks</a>":"<span class='ui-icon ui-icon-locked ui-align-center'></span>",
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
        /// returns the data for updation
        /// </summary>
        /// <param name="proposalCode">proposal code </param>
        /// <param name="yearCode">year of data</param>
        /// <param name="monthCode">month of data</param>
        /// <returns></returns>
        public MaintenanceProgressViewModel GetFinancialDetails(int proposalCode, int contractCode, int yearCode, int monthCode)
        {
            PMGSYEntities dbContext = new PMGSYEntities();
            try
            {
                CommonFunctions objCommon = new CommonFunctions();
                MANE_IMS_PROGRESS progressMaster = dbContext.MANE_IMS_PROGRESS.Find(proposalCode, contractCode, yearCode, monthCode);
                MaintenanceProgressViewModel progressModel = new MaintenanceProgressViewModel();
                if (progressMaster != null)
                {
                    progressModel.EncyptedProgressCode = URLEncrypt.EncryptParameters1(new string[] { "ProposalCode=" + progressMaster.IMS_PR_ROAD_CODE.ToString().Trim() });
                    if (progressMaster.MANE_FINAL_PAYMENT_DATE != null)
                    {
                        progressModel.FinalPaymentDate = objCommon.GetDateTimeToString(progressMaster.MANE_FINAL_PAYMENT_DATE.Value);
                    }
                    else
                    {
                        progressModel.FinalPaymentDate = null;
                    }
                    progressModel.FinalPaymentFlag = progressMaster.MANE_FINAL_PAYMENT_FLAG;
                    progressModel.PaymentLastMonth = progressMaster.MANE_PAYMENT_LASTMONTH;
                    progressModel.PaymentThisMonth = progressMaster.MANE_PAYMENT_THISMONTH;
                    progressModel.ProgramMonth = progressMaster.MANE_PROG_MONTH;
                    progressModel.ProgramYear = progressMaster.MANE_PROG_YEAR;
                    progressModel.ValueOfWorkLastMonth = progressMaster.MANE_VALUEOFWORK_LASTMONTH;
                    progressModel.ValueOfWorkThisMonth = progressMaster.MANE_VALUEOFWORK_THISMONTH;
                    progressModel.ProposalCode = progressMaster.IMS_PR_ROAD_CODE;
                    progressModel.ProposalContractCode = progressMaster.MANE_MAINTENANCE_NUMBER;
                    ///Added by SAMMED A. PATIL on 19JUNE2017 for new field Maintenance Type
                    progressModel.maintenanceType = progressMaster.MANE_TYPE;
                    progressModel.Operation = "E";

                    if (dbContext.MANE_IMS_PROGRESS.Any(m => m.IMS_PR_ROAD_CODE == proposalCode))
                    {
                        int? year = (dbContext.MANE_IMS_PROGRESS.Where(m => m.IMS_PR_ROAD_CODE == proposalCode && m.MANE_MAINTENANCE_NUMBER == contractCode).OrderByDescending(y => y.MANE_PROG_YEAR).Select(y => y.MANE_PROG_YEAR).FirstOrDefault());
                        int? month = (dbContext.MANE_IMS_PROGRESS.Where(m => m.IMS_PR_ROAD_CODE == proposalCode && m.MANE_PROG_YEAR == year && m.MANE_MAINTENANCE_NUMBER == contractCode).Max(m => m.MANE_PROG_MONTH));
                        MANE_IMS_PROGRESS masterRoad = (dbContext.MANE_IMS_PROGRESS.Where(m => m.MANE_PROG_YEAR == year && m.IMS_PR_ROAD_CODE == proposalCode && m.MANE_MAINTENANCE_NUMBER == contractCode).OrderByDescending(m => m.MANE_PROG_MONTH).FirstOrDefault());

                        if (masterRoad != null)
                        {
                            progressModel.PreviousMonth = masterRoad.MANE_PROG_MONTH;
                            progressModel.PreviousYear = masterRoad.MANE_PROG_YEAR;
                            progressModel.TotalValueofwork = (dbContext.MANE_IMS_PROGRESS.Where(m => m.MANE_PROG_MONTH == month && m.MANE_PROG_YEAR == year && m.IMS_PR_ROAD_CODE == proposalCode).Select(m => m.MANE_VALUEOFWORK_LASTMONTH).FirstOrDefault()) + (dbContext.MANE_IMS_PROGRESS.Where(m => m.MANE_PROG_MONTH == month && m.MANE_PROG_YEAR == year && m.IMS_PR_ROAD_CODE == proposalCode).Select(m => m.MANE_VALUEOFWORK_THISMONTH).FirstOrDefault());
                            progressModel.TotalPayment = (dbContext.MANE_IMS_PROGRESS.Where(m => m.MANE_PROG_MONTH == month && m.MANE_PROG_YEAR == year && m.IMS_PR_ROAD_CODE == proposalCode).Select(m => m.MANE_PAYMENT_LASTMONTH).FirstOrDefault()) + (dbContext.MANE_IMS_PROGRESS.Where(m => m.MANE_PROG_MONTH == month && m.MANE_PROG_YEAR == year && m.IMS_PR_ROAD_CODE == proposalCode).Select(m => m.MANE_PAYMENT_THISMONTH).FirstOrDefault());
                            progressModel.LastPaymentValue = (dbContext.MANE_IMS_PROGRESS.Where(m => m.MANE_PROG_MONTH == month && m.MANE_PROG_YEAR == year && m.IMS_PR_ROAD_CODE == proposalCode).Select(m => m.MANE_PAYMENT_LASTMONTH).FirstOrDefault());
                            progressModel.LastMonthValue = (dbContext.MANE_IMS_PROGRESS.Where(m => m.MANE_PROG_MONTH == month && m.MANE_PROG_YEAR == year && m.IMS_PR_ROAD_CODE == proposalCode).Select(m => m.MANE_VALUEOFWORK_LASTMONTH).FirstOrDefault());
                            progressModel.PaymentLastMonth = progressModel.LastPaymentValue;
                            progressModel.ValueOfWorkLastMonth = progressModel.LastMonthValue;
                            string status = (from item in dbContext.MANE_IMS_PROGRESS
                                             where item.IMS_PR_ROAD_CODE == proposalCode &&
                                             item.MANE_FINAL_PAYMENT_FLAG == "Y"
                                             select item.MANE_FINAL_PAYMENT_FLAG).FirstOrDefault();

                            if (status == "Y")
                            {
                                progressModel.CompleteStatus = "C";
                            }
                            else
                            {
                                progressModel.CompleteStatus = "N";
                            }
                        }

                        if (dbContext.MANE_IMS_PROGRESS.Any(m => m.MANE_MAINTENANCE_NUMBER == contractCode && m.IMS_PR_ROAD_CODE == proposalCode && m.MANE_PROG_MONTH == progressModel.PreviousMonth && m.MANE_PROG_YEAR == progressModel.PreviousYear && m.MANE_FINAL_PAYMENT_FLAG == "Y"))
                        {
                            progressModel.IsFinalPaymentBefore = "Y";
                        }

                    }
                    progressModel.AgreementTotal = (dbContext.TEND_AGREEMENT_DETAIL.Where(m => m.IMS_PR_ROAD_CODE == proposalCode).Select(m => m.TEND_AGREEMENT_AMOUNT).FirstOrDefault());

                    MANE_IMS_CONTRACT contractMaster = dbContext.MANE_IMS_CONTRACT.Where(m => m.IMS_PR_ROAD_CODE == proposalCode && m.MANE_CONTRACT_NUMBER == contractCode).FirstOrDefault();
                    progressModel.AgreementCost = contractMaster.MANE_YEAR1_AMOUNT + contractMaster.MANE_YEAR2_AMOUNT + contractMaster.MANE_YEAR3_AMOUNT + contractMaster.MANE_YEAR4_AMOUNT + contractMaster.MANE_YEAR5_AMOUNT;
                    int agreementCode = dbContext.TEND_AGREEMENT_DETAIL.Where(m => m.IMS_PR_ROAD_CODE == proposalCode).OrderBy(m => m.TEND_AGREEMENT_ID).Select(m => m.TEND_AGREEMENT_CODE).FirstOrDefault();
                    //progressModel.AgreementCost = dbContext.TEND_AGREEMENT_MASTER.Where(m => m.TEND_AGREEMENT_CODE == agreementCode).Select(m => m.TEND_AGREEMENT_AMOUNT).FirstOrDefault();
                    DateTime? agreementDate = dbContext.TEND_AGREEMENT_MASTER.Where(m => m.TEND_AGREEMENT_CODE == agreementCode).Select(m => m.TEND_DATE_OF_AGREEMENT).FirstOrDefault();
                    progressModel.AgreementDate = objCommon.GetDateTimeToString(agreementDate.Value);
                    progressModel.AgreementMonth = agreementDate.Value.Month;
                    progressModel.AgreementYear = agreementDate.Value.Year;
                    ////IMS_SANCTIONED_PROJECTS master = dbContext.IMS_SANCTIONED_PROJECTS.Find(proposalCode);
                    ////if (master != null)
                    ////{
                    ////    progressModel.BlockName = (dbContext.MASTER_BLOCK.Where(m => m.MAST_BLOCK_CODE == master.MAST_BLOCK_CODE).Select(m => m.MAST_BLOCK_NAME).FirstOrDefault());
                    ////    progressModel.Package = master.IMS_PACKAGE_ID.ToString();
                    ////    progressModel.RoadName = master.IMS_ROAD_NAME;
                    ////    progressModel.Sanction_Cost = Convert.ToDouble(master.IMS_SANCTIONED_BS_AMT + master.IMS_SANCTIONED_BW_AMT + master.IMS_SANCTIONED_CD_AMT + master.IMS_SANCTIONED_OW_AMT + master.IMS_SANCTIONED_PAV_AMT + master.IMS_SANCTIONED_PW_AMT + master.IMS_SANCTIONED_RS_AMT);
                    ////    progressModel.Sanction_length = master.IMS_PAV_LENGTH;
                    ////}

                    return progressModel;
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
                if (dbContext != null)
                {
                    dbContext.Dispose();
                }
            }
        }

        /// <summary>
        /// save the data of financial details
        /// </summary>
        /// <param name="progressModel">model containing data </param>
        /// <param name="message">returns the error message</param>
        /// <returns></returns>
        public bool AddFinancialProgress(MaintenanceProgressViewModel progressModel, ref string message)
        {
            PMGSYEntities dbContext = new PMGSYEntities();
            using (TransactionScope ts = new TransactionScope())
            {
                try
                {
                    DateTime? finalPaymentDate = null;
                    MANE_IMS_PROGRESS progreeMaster = new MANE_IMS_PROGRESS();
                    CommonFunctions objCommon = new CommonFunctions();
                    if (progressModel.FinalPaymentDate != null)
                    {
                        finalPaymentDate = objCommon.GetStringToDateTime(progressModel.FinalPaymentDate);
                        progreeMaster.MANE_FINAL_PAYMENT_DATE = finalPaymentDate;
                    }
                    //progreeMaster.MANE_PR_CONTRACT_CODE = dbContext.MANE_IMS_CONTRACT.Where(m => m.IMS_PR_ROAD_CODE == progressModel.ProposalCode).OrderByDescending(m => m.MANE_PR_CONTRACT_CODE).Select(m => m.MANE_PR_CONTRACT_CODE).FirstOrDefault();
                    progreeMaster.MANE_MAINTENANCE_NUMBER = progressModel.ProposalContractCode;
                    progreeMaster.MANE_FINAL_PAYMENT_FLAG = progressModel.FinalPaymentFlag;
                    progreeMaster.MANE_PAYMENT_LASTMONTH = progressModel.PaymentLastMonth;
                    progreeMaster.MANE_PAYMENT_THISMONTH = progressModel.PaymentThisMonth;
                    progreeMaster.MANE_PROG_MONTH = progressModel.ProgramMonth;
                    progreeMaster.MANE_PROG_YEAR = progressModel.ProgramYear;
                    progreeMaster.MANE_VALUEOFWORK_LASTMONTH = progressModel.ValueOfWorkLastMonth;
                    progreeMaster.MANE_VALUEOFWORK_THISMONTH = progressModel.ValueOfWorkThisMonth;
                    progreeMaster.IMS_PR_ROAD_CODE = progressModel.ProposalCode;
                    
                    ///Added by SAMMED A. PATIL on 19JUNE2017 for new field Maintenance Type
                    progreeMaster.MANE_TYPE = progressModel.maintenanceType;

                    progreeMaster.USERID = PMGSYSession.Current.UserId;
                    progreeMaster.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                    dbContext.MANE_IMS_PROGRESS.Add(progreeMaster);
                    dbContext.SaveChanges();
                    ts.Complete();
                    message = "Financial Details added successfully.";
                    return true;
                }
                catch (Exception ex)
                {
                    Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                    message = "Error Occurred while processing your request.";
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
        /// updates the financial details
        /// </summary>
        /// <param name="progressModel">model containing the updated data</param>
        /// <param name="message">indicates the response message</param>
        /// <returns>returns status along with the response message</returns>
        public bool EditFinancialProgress(MaintenanceProgressViewModel progressModel, ref string message)
        {
            PMGSYEntities dbContext = new PMGSYEntities();
            try
            {
                MANE_IMS_PROGRESS master = dbContext.MANE_IMS_PROGRESS.Find(progressModel.ProposalCode, progressModel.ProposalContractCode, progressModel.ProgramYear, progressModel.ProgramMonth);
                if (master != null)
                {
                    if (progressModel.FinalPaymentFlag == "Y")
                    {
                        master.MANE_FINAL_PAYMENT_DATE = new CommonFunctions().GetStringToDateTime(progressModel.FinalPaymentDate);
                    }
                    else
                    {
                        master.MANE_FINAL_PAYMENT_DATE = null;
                    }
                    master.MANE_FINAL_PAYMENT_FLAG = progressModel.FinalPaymentFlag;
                    master.MANE_PAYMENT_THISMONTH = progressModel.PaymentThisMonth;
                    master.MANE_PROG_MONTH = progressModel.ProgramMonth;
                    master.MANE_PROG_YEAR = progressModel.ProgramYear;
                    master.MANE_VALUEOFWORK_THISMONTH = progressModel.ValueOfWorkThisMonth;
                    master.USERID = PMGSYSession.Current.UserId;
                    master.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];

                    ///Added by SAMMED A. PATIL on 19JUNE2017 for new field Maintenance Type
                    master.MANE_TYPE = progressModel.maintenanceType;

                    dbContext.Entry(master).State = System.Data.Entity.EntityState.Modified;
                    dbContext.SaveChanges();
                    message = "Financial Details Updated successfully.";
                    return true;
                }
                else
                {
                    message = "Error Occurred while processing your request.";
                    return false;
                }
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                message = "Error Occurred while processing your request.";
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
        /// deletes the financial details
        /// </summary>
        /// <param name="proposalCode">road proposal code</param>
        /// <param name="progressType">progress type of financial details </param>
        /// <param name="yearCode">year of financial details </param>
        /// <param name="monthCode">month of financial details</param>
        /// <param name="message">response message</param>
        /// <returns>status of operation along with response message</returns>
        public bool DeleteFinancialRoadDetails(int proposalCode, int contractCode, int yearCode, int monthCode, ref string message)
        {
            PMGSYEntities dbContext = new PMGSYEntities();
            try
            {
                MANE_IMS_PROGRESS master = dbContext.MANE_IMS_PROGRESS.Find(proposalCode, contractCode, yearCode, monthCode);
                if (master != null)
                {
                    master.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                    master.USERID = PMGSYSession.Current.UserId;
                    dbContext.Entry(master).State = System.Data.Entity.EntityState.Modified;
                    dbContext.SaveChanges();

                    dbContext.MANE_IMS_PROGRESS.Remove(master);
                    dbContext.SaveChanges();
                    message = "Financial Progress details deleted successfully.";
                    return true;
                }
                else
                {
                    message = "Financial Progress details is in use and can not be deleted.";
                    return false;
                }
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                message = "Error Occurred while processing your request.";
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
        /// returns the financial details
        /// </summary>
        /// <param name="proposalCode">road proposal code</param>
        /// <returns>financial details</returns>
        public MaintenanceProgressViewModel GetFinancialAddDetails(int proposalCode, int contractCode = 0)
        {
            PMGSYEntities dbContext = new PMGSYEntities();
            try
            {
                MaintenanceProgressViewModel model = new MaintenanceProgressViewModel();
                if (dbContext.MANE_IMS_PROGRESS.Any(m => m.IMS_PR_ROAD_CODE == proposalCode))
                {
                    int? year = (dbContext.MANE_IMS_PROGRESS.Where(m => m.IMS_PR_ROAD_CODE == proposalCode && m.MANE_MAINTENANCE_NUMBER == contractCode).OrderByDescending(y => y.MANE_PROG_YEAR).Select(y => y.MANE_PROG_YEAR).FirstOrDefault());
                    int? month = (dbContext.MANE_IMS_PROGRESS.Where(m => m.IMS_PR_ROAD_CODE == proposalCode && m.MANE_PROG_YEAR == year && m.MANE_MAINTENANCE_NUMBER == contractCode).OrderByDescending(y => y.MANE_PROG_MONTH).Select(y => y.MANE_PROG_MONTH).FirstOrDefault());
                    MANE_IMS_PROGRESS masterRoad = (dbContext.MANE_IMS_PROGRESS.Where(m => m.MANE_PROG_YEAR == year && m.IMS_PR_ROAD_CODE == proposalCode && m.MANE_MAINTENANCE_NUMBER == contractCode).OrderByDescending(m => m.MANE_PROG_MONTH).FirstOrDefault());

                    if (masterRoad != null)
                    {
                        model.PreviousMonth = masterRoad.MANE_PROG_MONTH;
                        model.PreviousYear = masterRoad.MANE_PROG_YEAR;
                        model.TotalValueofwork = (dbContext.MANE_IMS_PROGRESS.Where(m => m.MANE_PROG_MONTH == month && m.MANE_PROG_YEAR == year && m.IMS_PR_ROAD_CODE == proposalCode && m.MANE_MAINTENANCE_NUMBER == contractCode).Select(m => m.MANE_VALUEOFWORK_LASTMONTH).FirstOrDefault()) + (dbContext.MANE_IMS_PROGRESS.Where(m => m.MANE_PROG_MONTH == month && m.MANE_PROG_YEAR == year && m.IMS_PR_ROAD_CODE == proposalCode && m.MANE_MAINTENANCE_NUMBER == contractCode).Select(m => m.MANE_VALUEOFWORK_THISMONTH).FirstOrDefault());
                        model.TotalPayment = (dbContext.MANE_IMS_PROGRESS.Where(m => m.MANE_PROG_MONTH == month && m.MANE_PROG_YEAR == year && m.IMS_PR_ROAD_CODE == proposalCode && m.MANE_MAINTENANCE_NUMBER == contractCode).Select(m => m.MANE_PAYMENT_LASTMONTH).FirstOrDefault()) + (dbContext.MANE_IMS_PROGRESS.Where(m => m.MANE_PROG_MONTH == month && m.MANE_PROG_YEAR == year && m.IMS_PR_ROAD_CODE == proposalCode && m.MANE_MAINTENANCE_NUMBER == contractCode).Select(m => m.MANE_PAYMENT_THISMONTH).FirstOrDefault());
                        model.LastPaymentValue = (dbContext.MANE_IMS_PROGRESS.Where(m => m.MANE_PROG_MONTH == month && m.MANE_PROG_YEAR == year && m.IMS_PR_ROAD_CODE == proposalCode && m.MANE_MAINTENANCE_NUMBER == contractCode).Select(m => m.MANE_PAYMENT_LASTMONTH).FirstOrDefault());
                        model.LastMonthValue = (dbContext.MANE_IMS_PROGRESS.Where(m => m.MANE_PROG_MONTH == month && m.MANE_PROG_YEAR == year && m.IMS_PR_ROAD_CODE == proposalCode && m.MANE_MAINTENANCE_NUMBER == contractCode).Select(m => m.MANE_VALUEOFWORK_LASTMONTH).FirstOrDefault());
                        model.PaymentLastMonth = model.LastPaymentValue;
                        model.ValueOfWorkLastMonth = model.LastMonthValue;
                        string status = (from item in dbContext.MANE_IMS_PROGRESS
                                         where item.IMS_PR_ROAD_CODE == proposalCode &&
                                         item.MANE_FINAL_PAYMENT_FLAG == "Y"
                                         select item.MANE_FINAL_PAYMENT_FLAG).FirstOrDefault();

                        if (status == "Y")
                        {
                            model.CompleteStatus = "C";
                        }
                        else
                        {
                            model.CompleteStatus = "N";
                        }
                    }
                    else
                    {
                        model.TotalValueofwork = 0;
                        model.TotalPayment = 0;
                        model.LastPaymentValue = 0;
                        model.LastMonthValue = 0;
                        model.PaymentLastMonth = 0;
                        model.ValueOfWorkLastMonth = 0;
                    }
                }
                else
                {
                    model.TotalValueofwork = 0;
                    model.TotalPayment = 0;
                    model.LastPaymentValue = 0;
                    model.LastMonthValue = 0;
                    model.PaymentLastMonth = 0;
                    model.ValueOfWorkLastMonth = 0;
                }
                if (contractCode != 0)
                {
                    if (dbContext.MANE_IMS_PROGRESS.Any(m => m.IMS_PR_ROAD_CODE == proposalCode && m.MANE_MAINTENANCE_NUMBER == contractCode && m.MANE_FINAL_PAYMENT_FLAG == "Y"))
                    {
                        model.IsFinalPaymentBefore = "Y";
                    }
                }
                int agreementId = dbContext.TEND_AGREEMENT_DETAIL.Where(m => m.IMS_PR_ROAD_CODE == proposalCode).OrderBy(m => m.TEND_AGREEMENT_ID).Select(m => m.TEND_AGREEMENT_CODE).FirstOrDefault();
                //TEND_AGREEMENT_MASTER tendMaster = dbContext.TEND_AGREEMENT_MASTER.Find(agreementId);
                //model.AgreementDate = new CommonFunctions().GetDateTimeToString(tendMaster.TEND_DATE_OF_AGREEMENT);
                //model.AgreementMonth = tendMaster.TEND_DATE_OF_AGREEMENT.Month;
                //model.AgreementYear = tendMaster.TEND_DATE_OF_AGREEMENT.Year;

                //MANE_IMS_CONTRACT contractMaster = dbContext.MANE_IMS_CONTRACT.Where(m => m.IMS_PR_ROAD_CODE == proposalCode).OrderByDescending(m => m.MANE_PR_CONTRACT_CODE).ThenByDescending(m => m.MANE_CONTRACT_NUMBER).FirstOrDefault();
                
                
                
                
                MANE_IMS_CONTRACT contractMaster = contractCode == 0 ? dbContext.MANE_IMS_CONTRACT.Where(m => m.IMS_PR_ROAD_CODE == proposalCode && m.MANE_CONTRACT_NUMBER == 1 && m.MANE_AGREEMENT_TYPE == "R").FirstOrDefault() : dbContext.MANE_IMS_CONTRACT.Where(m => m.IMS_PR_ROAD_CODE == proposalCode && m.MANE_CONTRACT_NUMBER == contractCode && m.MANE_AGREEMENT_TYPE == "R").FirstOrDefault();

                model.AgreementDate = contractMaster == null ? (new CommonFunctions().GetDateTimeToString(System.DateTime.Now)) : (new CommonFunctions().GetDateTimeToString(contractMaster.MANE_MAINTENANCE_START_DATE));
                model.AgreementMonth = contractMaster == null ? DateTime.Now.Month : (contractMaster.MANE_MAINTENANCE_START_DATE.Month);
                model.AgreementYear = contractMaster == null ? DateTime.Now.Year : (contractMaster.MANE_MAINTENANCE_START_DATE.Year);
                model.AgreementCost = contractMaster == null ? 0 : (contractMaster.MANE_YEAR1_AMOUNT + contractMaster.MANE_YEAR2_AMOUNT + contractMaster.MANE_YEAR3_AMOUNT + contractMaster.MANE_YEAR4_AMOUNT + contractMaster.MANE_YEAR5_AMOUNT);

                //model.AgreementDate = new CommonFunctions().GetDateTimeToString(contractMaster.MANE_MAINTENANCE_START_DATE);
                //model.AgreementMonth = contractMaster.MANE_MAINTENANCE_START_DATE.Month;
                //model.AgreementYear = contractMaster.MANE_MAINTENANCE_START_DATE.Year;
                //model.AgreementCost = contractMaster.MANE_YEAR1_AMOUNT + contractMaster.MANE_YEAR2_AMOUNT + contractMaster.MANE_YEAR3_AMOUNT + contractMaster.MANE_YEAR4_AMOUNT + contractMaster.MANE_YEAR5_AMOUNT;

                var ContractNumbers = dbContext.MANE_IMS_CONTRACT.Where(m => m.IMS_PR_ROAD_CODE == proposalCode && m.MANE_AGREEMENT_TYPE == "R").OrderByDescending(m => m.MANE_PR_CONTRACT_CODE).ThenByDescending(m => m.MANE_CONTRACT_NUMBER).FirstOrDefault();
                List<SelectListItem> lstNumbers = new List<SelectListItem>();
                for (int i = 1; i <= ContractNumbers.MANE_CONTRACT_NUMBER; i++)
                {
                    lstNumbers.Add(new SelectListItem { Value = i.ToString(), Text = i.ToString() });
                }
                model.lstMaintenanceNo = lstNumbers;
                //model.ContractNumber = contractMaster.MANE_CONTRACT_NUMBER;
                model.OverallCost = dbContext.MANE_IMS_CONTRACT.Where(m => m.IMS_PR_ROAD_CODE == proposalCode && m.MANE_CONTRACT_STATUS == "I" && m.MANE_AGREEMENT_TYPE == "R").Sum(m => m.MANE_VALUE_WORK_DONE);
                if (model.OverallCost == null)
                {
                    model.OverallCost = 0;
                }
                model.OverallCost = model.OverallCost + model.AgreementCost;
                IMS_SANCTIONED_PROJECTS imsMaster = dbContext.IMS_SANCTIONED_PROJECTS.Find(proposalCode);
                model.RoadName = imsMaster.IMS_ROAD_NAME;
                model.Package = imsMaster.IMS_PACKAGE_ID;
                model.SanctionLength = imsMaster.IMS_PAV_LENGTH;
                model.BlockName = (dbContext.MASTER_BLOCK.Where(m => m.MAST_BLOCK_CODE == imsMaster.MAST_BLOCK_CODE).Select(m => m.MAST_BLOCK_NAME).FirstOrDefault());

                //new change done on 20-11-2013

                var lstAgreements = dbContext.MANE_IMS_CONTRACT.Where(m => m.IMS_PR_ROAD_CODE == proposalCode && m.MANE_AGREEMENT_TYPE == "R").ToList();
                List<SelectListItem> _lstContracts = new List<SelectListItem>();
                if (lstAgreements != null)
                {
                    foreach (var item in lstAgreements)
                    {
                        _lstContracts.Add(new SelectListItem { Value = item.MANE_PR_CONTRACT_CODE.ToString(), Text = item.MANE_AGREEMENT_NUMBER.ToString() });
                    }
                }
                model.lstMANEAgreements = _lstContracts;
                // end of change


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
        /// validates the agreement cost
        /// </summary>
        /// <param name="proposalCode">proposal id</param>
        /// <param name="valueOfWork">value of work during this month</param>
        /// <param name="valueOfPayment">value of payment during this month</param>
        /// <param name="operation">operation (add or edit)</param>
        /// <returns></returns>
        //public bool CheckSanctionValue(int proposalCode, decimal valueOfWork, decimal valueOfPayment, string operation,int contractCode)
        //{
        //    PMGSYEntities dbContext = new PMGSYEntities();
        //    decimal? TotalPayment = 0;
        //    decimal? TotalValue = 0;
        //    try
        //    {
        //        IMS_SANCTIONED_PROJECTS master = dbContext.IMS_SANCTIONED_PROJECTS.Find(proposalCode);
        //        if (operation == "E")
        //        {
        //            TotalPayment = dbContext.MANE_IMS_PROGRESS.Where(m => m.IMS_PR_ROAD_CODE == proposalCode && m.MANE_MAINTENANCE_NUMBER == contractCode).OrderByDescending(m => m.MANE_PROG_YEAR).ThenByDescending(m => m.MANE_PROG_MONTH).Select(m => m.MANE_PAYMENT_LASTMONTH).FirstOrDefault();
        //            TotalValue = dbContext.MANE_IMS_PROGRESS.Where(m => m.IMS_PR_ROAD_CODE == proposalCode && m.MANE_MAINTENANCE_NUMBER == contractCode).OrderByDescending(m => m.MANE_PROG_YEAR).ThenByDescending(m => m.MANE_PROG_MONTH).Select(m => m.MANE_VALUEOFWORK_LASTMONTH).FirstOrDefault();
        //        }
        //        else
        //        {
        //            TotalPayment = dbContext.MANE_IMS_PROGRESS.Where(m => m.IMS_PR_ROAD_CODE == proposalCode && m.MANE_MAINTENANCE_NUMBER == contractCode).OrderByDescending(m => m.MANE_PROG_YEAR).ThenByDescending(m => m.MANE_PROG_MONTH).Select(m => m.MANE_PAYMENT_LASTMONTH).FirstOrDefault() + dbContext.MANE_IMS_PROGRESS.Where(m => m.IMS_PR_ROAD_CODE == proposalCode).OrderByDescending(m => m.MANE_PROG_YEAR).ThenByDescending(m => m.MANE_PROG_MONTH).Select(m => m.MANE_PAYMENT_THISMONTH).FirstOrDefault();
        //            TotalValue = dbContext.MANE_IMS_PROGRESS.Where(m => m.IMS_PR_ROAD_CODE == proposalCode && m.MANE_MAINTENANCE_NUMBER == contractCode).OrderByDescending(m => m.MANE_PROG_YEAR).ThenByDescending(m => m.MANE_PROG_MONTH).Select(m => m.MANE_VALUEOFWORK_LASTMONTH).FirstOrDefault() + dbContext.MANE_IMS_PROGRESS.Where(m => m.IMS_PR_ROAD_CODE == proposalCode).OrderByDescending(m => m.MANE_PROG_YEAR).ThenByDescending(m => m.MANE_PROG_MONTH).Select(m => m.MANE_VALUEOFWORK_THISMONTH).FirstOrDefault();
        //        }

        //        //int agreementCode = dbContext.TEND_AGREEMENT_DETAIL.Where(m => m.IMS_PR_ROAD_CODE == proposalCode).OrderByDescending(m => m.TEND_AGREEMENT_ID).Select(m => m.TEND_AGREEMENT_CODE).FirstOrDefault();
        //        var listAgreements = dbContext.MANE_IMS_CONTRACT.Where(m => m.IMS_PR_ROAD_CODE == proposalCode).OrderByDescending(m => m.MANE_PR_CONTRACT_CODE).Select(m => m.MANE_PR_CONTRACT_CODE);
        //        decimal? agreementCost = null;
        //        foreach (var item in listAgreements)
        //        {
        //            if (dbContext.MANE_IMS_CONTRACT.Any(m => m.MANE_CONTRACT_STATUS == "I" && m.MANE_PR_CONTRACT_CODE == item && m.IMS_PR_ROAD_CODE == proposalCode))
        //            {
        //                agreementCost = dbContext.MANE_IMS_CONTRACT.Where(m => m.MANE_PR_CONTRACT_CODE == item && m.IMS_PR_ROAD_CODE == proposalCode).Sum(m => m.MANE_VALUE_WORK_DONE);
        //            }
        //        }

        //        //int contractCode = dbContext.MANE_IMS_CONTRACT.Where(m => m.IMS_PR_ROAD_CODE == proposalCode && m.MANE_CONTRACT_STATUS == "P").OrderBy(m => m.MANE_PR_CONTRACT_CODE).Select(m => m.MANE_PR_CONTRACT_CODE).FirstOrDefault();
        //        MANE_IMS_CONTRACT firstContractMaster = dbContext.MANE_IMS_CONTRACT.Where(m => m.MANE_PR_CONTRACT_CODE == contractCode && m.IMS_PR_ROAD_CODE == proposalCode).FirstOrDefault();
        //        decimal? firstAgreementCost = firstContractMaster.MANE_YEAR1_AMOUNT + firstContractMaster.MANE_YEAR2_AMOUNT + firstContractMaster.MANE_YEAR3_AMOUNT + firstContractMaster.MANE_YEAR4_AMOUNT + firstContractMaster.MANE_YEAR5_AMOUNT;
        //        if (agreementCost == null)
        //        {
        //            agreementCost = 0;
        //            agreementCost = agreementCost + valueOfWork;
        //        }

        //        if (agreementCost < (firstAgreementCost + firstAgreementCost / 10))
        //        {
        //            return true;
        //        }
        //        else
        //        {
        //            return false;
        //        }


        //        if (TotalPayment == null)
        //        {
        //            TotalPayment = 0;
        //        }

        //        if (TotalValue == null)
        //        {
        //            TotalValue = 0;
        //        }
        //        TotalPayment = TotalPayment + valueOfPayment;
        //        TotalValue = TotalValue + valueOfWork;

        //        //if (TotalPayment <= (agreementCost + (agreementCost / 10)) && TotalValue <= (agreementCost + (agreementCost / 10)))
        //        if (TotalPayment <= (agreementCost + (agreementCost / 10)))
        //        {
        //            return true;
        //        }
        //        else
        //        {
        //            return false;
        //        }
        //    }
        //    catch (Exception)
        //    {
        //        return false;
        //    }
        //}
        public bool CheckSanctionValue(int proposalCode, decimal valueOfWork, decimal valueOfPayment, string operation, int contractCode)
        {
            PMGSYEntities dbContext = new PMGSYEntities();
            decimal? TotalPayment = 0;
            decimal? TotalValue = 0;
            try
            {
                IMS_SANCTIONED_PROJECTS master = dbContext.IMS_SANCTIONED_PROJECTS.Find(proposalCode);
                if (operation == "E")
                {
                    TotalPayment = dbContext.MANE_IMS_PROGRESS.Where(m => m.IMS_PR_ROAD_CODE == proposalCode && m.MANE_MAINTENANCE_NUMBER == contractCode).OrderByDescending(m => m.MANE_PROG_YEAR).ThenByDescending(m => m.MANE_PROG_MONTH).Select(m => m.MANE_PAYMENT_LASTMONTH).FirstOrDefault();
                    TotalValue = dbContext.MANE_IMS_PROGRESS.Where(m => m.IMS_PR_ROAD_CODE == proposalCode && m.MANE_MAINTENANCE_NUMBER == contractCode).OrderByDescending(m => m.MANE_PROG_YEAR).ThenByDescending(m => m.MANE_PROG_MONTH).Select(m => m.MANE_VALUEOFWORK_LASTMONTH).FirstOrDefault();
                }
                else
                {
                    TotalPayment = dbContext.MANE_IMS_PROGRESS.Where(m => m.IMS_PR_ROAD_CODE == proposalCode && m.MANE_MAINTENANCE_NUMBER == contractCode).OrderByDescending(m => m.MANE_PROG_YEAR).ThenByDescending(m => m.MANE_PROG_MONTH).Select(m => m.MANE_PAYMENT_LASTMONTH).FirstOrDefault() + dbContext.MANE_IMS_PROGRESS.Where(m => m.IMS_PR_ROAD_CODE == proposalCode).OrderByDescending(m => m.MANE_PROG_YEAR).ThenByDescending(m => m.MANE_PROG_MONTH).Select(m => m.MANE_PAYMENT_THISMONTH).FirstOrDefault();
                    TotalValue = dbContext.MANE_IMS_PROGRESS.Where(m => m.IMS_PR_ROAD_CODE == proposalCode && m.MANE_MAINTENANCE_NUMBER == contractCode).OrderByDescending(m => m.MANE_PROG_YEAR).ThenByDescending(m => m.MANE_PROG_MONTH).Select(m => m.MANE_VALUEOFWORK_LASTMONTH).FirstOrDefault() + dbContext.MANE_IMS_PROGRESS.Where(m => m.IMS_PR_ROAD_CODE == proposalCode).OrderByDescending(m => m.MANE_PROG_YEAR).ThenByDescending(m => m.MANE_PROG_MONTH).Select(m => m.MANE_VALUEOFWORK_THISMONTH).FirstOrDefault();
                }

                List<MANE_IMS_CONTRACT> lstContracts = dbContext.MANE_IMS_CONTRACT.Where(m => m.IMS_PR_ROAD_CODE == proposalCode && m.MANE_CONTRACT_NUMBER == contractCode && m.MANE_CONTRACT_STATUS != "I").ToList();
                decimal? costToAdd = 0;
                if (lstContracts != null)
                {
                    foreach (var item in lstContracts)
                    {
                        costToAdd = costToAdd + item.MANE_YEAR1_AMOUNT + item.MANE_YEAR2_AMOUNT + item.MANE_YEAR3_AMOUNT + item.MANE_YEAR4_AMOUNT + item.MANE_YEAR5_AMOUNT;
                    }
                }

                decimal? agreementCost = (dbContext.MANE_IMS_CONTRACT.Where(m => m.IMS_PR_ROAD_CODE == proposalCode && m.MANE_CONTRACT_STATUS == "I").Sum(m => (Decimal?)m.MANE_VALUE_WORK_DONE) == null ? 0 : dbContext.MANE_IMS_CONTRACT.Where(m => m.IMS_PR_ROAD_CODE == proposalCode && m.MANE_CONTRACT_STATUS == "I").Sum(m => (Decimal?)m.MANE_VALUE_WORK_DONE)) + costToAdd;

                if (TotalPayment == null)
                {
                    TotalPayment = 0;
                }

                if (TotalValue == null)
                {
                    TotalValue = 0;
                }

                TotalPayment = TotalPayment + valueOfPayment;
                TotalValue = TotalValue + valueOfWork;

                if (TotalPayment <= (agreementCost + (agreementCost / 10)))
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

        /// <summary>
        /// returns the maintenance no associated with the proposal
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public List<SelectListItem> PopulateMaintenanceNo(string id)
        {
            PMGSYEntities dbContext = new PMGSYEntities();
            try
            {
                string[] parameters = id.Split('$');
                List<SelectListItem> lstNos = new List<SelectListItem>();
                int proposalCode = Convert.ToInt32(parameters[0]);
                int contractCode = Convert.ToInt32(parameters[1]);
                var lstNumbers = dbContext.MANE_IMS_CONTRACT.Where(m => m.IMS_PR_ROAD_CODE == proposalCode && m.MANE_PR_CONTRACT_CODE == contractCode).ToList();
                if (lstNumbers != null)
                {
                    foreach (var item in lstNumbers)
                    {
                        lstNos.Add(new SelectListItem { Value = item.MANE_PR_CONTRACT_CODE.ToString(), Text = item.MANE_CONTRACT_NUMBER.ToString() });
                    }
                }
                return lstNos;
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
        /// returns the array of agreements for populating the list of agreements of particular road.
        /// </summary>
        /// <param name="page"></param>
        /// <param name="rows"></param>
        /// <param name="sidx"></param>
        /// <param name="sord"></param>
        /// <param name="totalRecords"></param>
        /// <param name="proposalCode"></param>
        /// <param name="progressType"></param>
        /// <param name="contractCode"></param>
        /// <returns></returns>
        public Array GetAgreementDetailsList(int page, int rows, string sidx, string sord, out long totalRecords, int proposalCode, string progressType, int contractCode)
        {
            PMGSYEntities dbContext = new PMGSYEntities();
            try
            {
                var lstAggDetails = (from sanc_projects in dbContext.IMS_SANCTIONED_PROJECTS
                                     join contract in dbContext.MANE_IMS_CONTRACT on sanc_projects.IMS_PR_ROAD_CODE equals contract.IMS_PR_ROAD_CODE
                                     join splitWork in dbContext.IMS_PROPOSAL_WORK on contract.IMS_WORK_CODE equals splitWork.IMS_WORK_CODE into splitDetails
                                     from sd in splitDetails.DefaultIfEmpty()
                                     where sanc_projects.IMS_PR_ROAD_CODE == proposalCode && contract.MANE_CONTRACT_NUMBER == contractCode
                                     select new
                                     {
                                         contract.MANE_PR_CONTRACT_CODE,
                                         contract.MANE_AGREEMENT_NUMBER,
                                         contract.MANE_AGREEMENT_DATE,
                                         AGREEMENT_AMOUNT = (contract.MANE_YEAR1_AMOUNT + contract.MANE_YEAR2_AMOUNT + contract.MANE_YEAR3_AMOUNT + contract.MANE_YEAR4_AMOUNT + contract.MANE_YEAR5_AMOUNT),
                                         SPLIT_WORK = (sd.IMS_WORK_DESC == null ? "-" : sd.IMS_WORK_DESC),
                                         SPLIT_COST = (sd.IMS_PAV_EST_COST == null ? 0 : sd.IMS_PAV_EST_COST),
                                         SPLIT_LENGTH = (sd.IMS_PAV_LENGTH == null ? 0 : sd.IMS_PAV_LENGTH),
                                         contract.MANE_CONTRACT_STATUS,
                                         contract.MANE_VALUE_WORK_DONE
                                     }).OrderByDescending(m => m.MANE_PR_CONTRACT_CODE).ThenBy(m => m.MANE_AGREEMENT_DATE).ToList();

                totalRecords = lstAggDetails.Count();

                if (sidx.Trim() != string.Empty)
                {
                    if (sord.ToString() == "asc")
                    {
                        switch (sidx)
                        {
                            case "MANE_AGREEMENT_NUMBER":
                                lstAggDetails = lstAggDetails.OrderBy(m => m.MANE_AGREEMENT_NUMBER).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                            case "MANE_AGREEMENT_DATE":
                                lstAggDetails = lstAggDetails.OrderBy(m => m.MANE_AGREEMENT_DATE).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                            case "AGREEMENT_AMOUNT":
                                lstAggDetails = lstAggDetails.OrderBy(m => m.AGREEMENT_AMOUNT).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                            case "IMS_PAV_LENGTH":
                                lstAggDetails = lstAggDetails.OrderBy(m => m.SPLIT_LENGTH).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                            case "IMS_PAV_EST_COST":
                                lstAggDetails = lstAggDetails.OrderBy(m => m.SPLIT_COST).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                            default:
                                lstAggDetails = lstAggDetails.OrderBy(m => m.MANE_AGREEMENT_NUMBER).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                        }
                    }
                    else
                    {
                        switch (sidx)
                        {
                            case "MANE_AGREEMENT_NUMBER":
                                lstAggDetails = lstAggDetails.OrderByDescending(m => m.MANE_AGREEMENT_NUMBER).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                            case "MANE_AGREEMENT_DATE":
                                lstAggDetails = lstAggDetails.OrderByDescending(m => m.MANE_AGREEMENT_DATE).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                            case "AGREEMENT_AMOUNT":
                                lstAggDetails = lstAggDetails.OrderByDescending(m => m.AGREEMENT_AMOUNT).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                            case "IMS_PAV_LENGTH":
                                lstAggDetails = lstAggDetails.OrderByDescending(m => m.SPLIT_LENGTH).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                            case "IMS_PAV_EST_COST":
                                lstAggDetails = lstAggDetails.OrderByDescending(m => m.SPLIT_COST).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                            default:
                                lstAggDetails = lstAggDetails.OrderByDescending(m => m.MANE_AGREEMENT_NUMBER).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                        }
                    }
                }
                else
                {
                    lstAggDetails = lstAggDetails.OrderByDescending(m => m.MANE_AGREEMENT_NUMBER).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                }

                var gridData = lstAggDetails.Select(progressDetails => new
                {
                    progressDetails.MANE_AGREEMENT_NUMBER,
                    progressDetails.MANE_AGREEMENT_DATE,
                    progressDetails.AGREEMENT_AMOUNT,
                    progressDetails.MANE_CONTRACT_STATUS,
                    progressDetails.MANE_VALUE_WORK_DONE,
                    progressDetails.SPLIT_WORK,
                    progressDetails.SPLIT_LENGTH,
                    progressDetails.SPLIT_COST
                }).ToArray();

                return gridData.Select(m => new
                {
                    cell = new[]
                    {
                        m.MANE_AGREEMENT_NUMBER.ToString(),
                        m.SPLIT_WORK == null?"-":m.SPLIT_WORK.ToString(),
                        m.SPLIT_LENGTH == 0?"-":m.SPLIT_LENGTH.ToString(),
                        m.SPLIT_COST == 0?"-":m.SPLIT_COST.ToString(),
                        Convert.ToDateTime(m.MANE_AGREEMENT_DATE).ToString("dd/MM/yyyy"),
                        m.AGREEMENT_AMOUNT.ToString(),
                        m.MANE_CONTRACT_STATUS =="P"?"In Progress":(m.MANE_CONTRACT_STATUS == "C"?"Completed":(m.MANE_CONTRACT_STATUS == "M"?"In Maintenance":"Incomplete")),
                        m.MANE_VALUE_WORK_DONE == null?"-":m.MANE_VALUE_WORK_DONE.ToString()
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

        #region Photo Upload


        public string AddFileUploadDetailsDAL(MANE_IMS_PROGRESS_FILES lst_inspection_files)
        {
            try
            {
                PMGSYEntities dbContext = new PMGSYEntities();
                Int32? MaxID;
                if (dbContext.MANE_IMS_PROGRESS_FILES.Count() == 0)
                {
                    MaxID = 0;
                }
                else
                {
                    MaxID = (from c in dbContext.MANE_IMS_PROGRESS_FILES select (Int32?)c.MANE_IMS_FILE_ID ?? 0).Max();
                }
                ++MaxID;

                lst_inspection_files.MANE_IMS_FILE_ID = Convert.ToInt32(MaxID);

                decimal workLength = dbContext.IMS_SANCTIONED_PROJECTS.Where(m => m.IMS_PR_ROAD_CODE == lst_inspection_files.IMS_PR_ROAD_CODE).Select(m => m.IMS_PAV_LENGTH).FirstOrDefault();
                decimal? entredsegmentLength = dbContext.MANE_IMS_PROGRESS_FILES.Where(m => m.IMS_PR_ROAD_CODE == lst_inspection_files.IMS_PR_ROAD_CODE).Select(m => m.SEGMENT_LENGTH).Sum();

                decimal? entredsegmentLength_Final = entredsegmentLength == null ? 0 : entredsegmentLength;

                decimal? currentSegment = (lst_inspection_files.END_CHAINAGE - lst_inspection_files.START_CHAINAGE);
                decimal? FinalSengmentlength = entredsegmentLength_Final + currentSegment;


                if (lst_inspection_files.END_CHAINAGE <= lst_inspection_files.START_CHAINAGE)
                {
                    return "End Chainage must be greater than Start Chainage";
                }


                if (FinalSengmentlength > workLength)
                {
                    return "Entered Chainage is exceeding maximum Segment Length for this Work.";
                }
                else
                {
                    dbContext.MANE_IMS_PROGRESS_FILES.Add(lst_inspection_files);
                    dbContext.SaveChanges();
                    return string.Empty;
                }
            }
            catch (System.Data.Entity.Infrastructure.DbUpdateException ex)
            {
                ErrorLog.LogError(ex, "MaintainanceInspectionDAL().AddFileUploadDetailsDAL() [DbUpdateException]");
                return ("An Error Occurred While Processing Your Request.");
            }
            catch (OptimisticConcurrencyException ex)
            {
                ErrorLog.LogError(ex, "MaintainanceInspectionDAL().AddFileUploadDetailsDAL() [OptimisticConcurrencyException]");
                return ("An Error Occurred While Processing Your Request.");
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "MaintainanceInspectionDAL().AddFileUploadDetailsDAL() [Exception]");
                return ("Error Occurred While Processing Your Request.");
            }
            finally
            {
                //  dbContext.Dispose();
            }
        }


        public Array GetFilesListDALProgress(int page, int rows, string sidx, string sord, out int totalRecords, int RoadCode)
        {
            try
            {
                PMGSYEntities dbContext = new PMGSYEntities();
                CommonFunctions objCommonFunction = new CommonFunctions();
                List<MANE_IMS_PROGRESS_FILES> listQMFiles = dbContext.MANE_IMS_PROGRESS_FILES.Where(p => p.IMS_PR_ROAD_CODE == RoadCode).ToList();

                IQueryable<MANE_IMS_PROGRESS_FILES> query = listQMFiles.AsQueryable<MANE_IMS_PROGRESS_FILES>();
                totalRecords = listQMFiles.Count();


                string VirtualDirectoryUrl = string.Empty;
                string PhysicalPath = string.Empty;
                string VirtualDirectoryUrl_OMMAS4 = string.Empty;

                VirtualDirectoryUrl = System.IO.Path.Combine(System.Configuration.ConfigurationManager.AppSettings["MANE_PROGRESS_FILE_UPLOAD_VIRTUAL_DIR_PATH"], "thumbnails");

                PhysicalPath = System.Configuration.ConfigurationManager.AppSettings["MANE_PROGRESS_FILE_UPLOAD"];


                //For self Reference
                File.Exists(System.IO.Path.Combine(PhysicalPath, HttpUtility.UrlEncode("1983635_1.jpg")).ToString().Replace(@"\\", @"//").Replace(@"\", @"/"));

                return query.Select(fileDetails => new
                {
                    id = fileDetails.MANE_IMS_FILE_ID,
                    cell = new[] {   
                                    // Added for look into OMMAS4 also
                                    (File.Exists(System.IO.Path.Combine(PhysicalPath, HttpUtility.UrlEncode(fileDetails.FILE_NAME.ToString())).ToString().Replace(@"\\",@"//").Replace(@"\",@"/")))
                                     ?
                                    Path.Combine(VirtualDirectoryUrl, HttpUtility.UrlEncode(fileDetails.FILE_NAME.ToString())).ToString().Replace(@"\\",@"//").Replace(@"\",@"/") + 
                                            "$$$" + (fileDetails.LATITUDE == null ? "0" : fileDetails.LATITUDE.ToString()) + "$$" +
                                            (fileDetails.LONGITUDE == null ? "0" : fileDetails.LONGITUDE.ToString()) +
                                            "$$$" + (objCommonFunction.GetDateTimeToString( Convert.ToDateTime(fileDetails.FILE_UPLOAD_DATE) ))
                                    :   Path.Combine(VirtualDirectoryUrl_OMMAS4, HttpUtility.UrlEncode(fileDetails.FILE_NAME.ToString())).ToString().Replace(@"\\",@"//").Replace(@"\",@"/") + 
                                            "$$$" + (fileDetails.LATITUDE == null ? "0" : fileDetails.LATITUDE.ToString()) + "$$" +
                                            (fileDetails.LONGITUDE == null ? "0" : fileDetails.LONGITUDE.ToString()) +
                                            "$$$" + (objCommonFunction.GetDateTimeToString( Convert.ToDateTime(fileDetails.FILE_UPLOAD_DATE) )) ,
                                    
                                    fileDetails.FILE_DESC,
                                    fileDetails.START_CHAINAGE == null? "-":fileDetails.START_CHAINAGE.ToString(),
                                    fileDetails.END_CHAINAGE == null? "-":fileDetails.END_CHAINAGE.ToString(),
                                     fileDetails.SEGMENT_LENGTH == null? "-":fileDetails.SEGMENT_LENGTH.ToString(),

                                                                          "<a href='#' title='Click here to Download an Image' class='ui-icon ui-icon-arrowthickstop-1-s ui-align-center' onClick=DownLoadProgressImageByPIU(\"" + URLEncrypt.EncryptParameters(new string[] { fileDetails.FILE_NAME + "$" + fileDetails.MANE_IMS_FILE_ID.ToString().Trim() }) +"\"); return false;'>Download</a>" ,
                                   // "<a href='#' title='Click here to Download an Image' class='ui-icon ui-icon-arrowthickstop-1-s ui-align-center' onClick=DownLoadImage(\"" + URLEncrypt.EncryptParameters(new string[] { fileDetails.FILE_NAME + "$" + fileDetails.IMS_PR_ROAD_CODE.ToString().Trim() }) +"\"); return false;'>Download</a>" ,
                      
                                     "<a href='#' title='Click here to Delete an Image' class='ui-icon ui-icon-trash ui-align-center' onClick=DeleteByPIUDetailsofProgress(\"" + URLEncrypt.EncryptParameters(new string[] { fileDetails.FILE_NAME + "$" + fileDetails.MANE_IMS_FILE_ID.ToString().Trim()}) +"\"); return false;'>Delete</a>" 
                                  //    "<a href='#' title='Click here to Delete an Image' class='ui-icon ui-icon-trash ui-align-center' onClick=DeleteFileDetailsForProgress(\"" + URLEncrypt.EncryptParameters(new string[] { fileDetails.IMS_PR_ROAD_CODE.ToString().Trim()+"$"+fileDetails.MANE_IMS_FILE_ID.ToString().Trim() }) +"\"); return false;'>Delete</a>" 
                                   // "<a href='#' title='Click here to Delete an Image' class='ui-icon ui-icon-trash ui-align-center' onClick=DeleteFileDetailsForProgress(\"" + URLEncrypt.EncryptParameters(new string[] { fileDetails.FILE_NAME + "$" + fileDetails.IMS_PR_ROAD_CODE.ToString().Trim()+"$"+fileDetails.MANE_IMS_FILE_ID.ToString().Trim() }) +"\"); return false;'>Delete</a>" 
                                              
                                                             
                                 
                    }
                }).ToArray();
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "MaintainanceInspectionDAL().GetFilesListDALProgress()");
                totalRecords = 0;
                return null;
            }
            finally
            {
                //  dbContext.Dispose();
            }
        }


        public string DeleteFileDetailsDALProgress(int QM_FILE_ID)
        {
            try
            {
                PMGSYEntities dbContext = new PMGSYEntities();

                // Delete only latest added Record.


                int roadCode = dbContext.MANE_IMS_PROGRESS_FILES.Where(m => m.MANE_IMS_FILE_ID == QM_FILE_ID).Select(m => m.IMS_PR_ROAD_CODE).FirstOrDefault();

                int MaxFileID = dbContext.MANE_IMS_PROGRESS_FILES.Where(a => a.IMS_PR_ROAD_CODE == roadCode).Max(a => a.MANE_IMS_FILE_ID);

                if (QM_FILE_ID == MaxFileID)
                {

                    MANE_IMS_PROGRESS_FILES db_qm_inspection_files = dbContext.MANE_IMS_PROGRESS_FILES.Find(QM_FILE_ID);
                    db_qm_inspection_files.USERID = PMGSYSession.Current.UserId;
                    db_qm_inspection_files.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];

                    dbContext.Entry(db_qm_inspection_files).State = System.Data.Entity.EntityState.Modified;
                    dbContext.SaveChanges();

                    dbContext.MANE_IMS_PROGRESS_FILES.Remove(db_qm_inspection_files);
                    dbContext.SaveChanges();
                    return string.Empty;
                }
                else
                {
                    return "Only last added Chainage Details can be deleted.";

                }
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "MaintainanceInspectionDAL().DeleteFileDetailsDAL()");
                return ("Error Occurred While Processing Your Request.");
            }
            finally
            {
                // dbContext.Dispose();
            }
        }

        #endregion
    }

    public interface IMaintenanceInspectionDAL
    {
        #region Upload File Details
        string AddFileUploadDetailsDAL(List<MAINTENANCE_FILES> lst_maintenance_files);
        Array GetFilesListDAL(int page, int rows, string sidx, string sord, out Int32 totalRecords, int IMS_PR_ROAD_CODE);
        string UpdateImageDetailsDAL(MAINTENANCE_FILES maintenance_files);
        string DeleteFileDetailsDAL(MAINTENANCE_FILES maintenance_files);
        #endregion

        #region MAINTENANCE_INSPECTION

        Array GetCompletedRoadListBAL(int stateCode, int districtCode, int blockCode, int sanctionedYear, int adminNDCode, string packageID, int batch, int collaboration, string upgradationType, int page, int rows, string sidx, string sord, out long totalRecords);
        Array GetInspectionRoadList(int proposalCode, int page, int rows, string sidx, string sord, out long totalRecords);
        bool SaveInspectionDetails(MaintainanceInspectionViewModel inspectionModel, ref string message);
        bool EditInspectionDetails(MaintainanceInspectionViewModel inspectionModel, ref string message);
        bool DeleteInspectionDetails(int prRoadCode, int inspectionCode);
        MaintainanceInspectionViewModel GetMaintainanceInspection_ByRoadCode(int prRoadCode, int inspectionCode);
        DateTime? getInspectionDate(int PRRoadCode);
        #endregion

        #region MAINTENANCE_FINANCIAL_INSPECTION

        Array GetFinancialProgressList(int page, int rows, string sidx, string sord, out long totalRecords, int proposalCode, string progressType, int contractCode);

        MaintenanceProgressViewModel GetFinancialDetails(int proposalCode, int contractCode, int yearCode, int monthCode);

        bool AddFinancialProgress(MaintenanceProgressViewModel progressModel, ref string message);

        bool EditFinancialProgress(MaintenanceProgressViewModel progressModel, ref string message);

        bool DeleteFinancialRoadDetails(int proposalCode, int contractCode, int yearCode, int monthCode, ref string message);

        MaintenanceProgressViewModel GetFinancialAddDetails(int proposalCode, int contractCode);

        bool CheckSanctionValue(int proposalCode, decimal valueOfWork, decimal valueOfPayment, string operation, int contractCode);

        List<SelectListItem> PopulateDesignation(bool isPopulateFirstItem = true);

        List<SelectListItem> PopulateMaintenanceNo(string id);

        Array GetAgreementDetailsList(int page, int rows, string sidx, string sord, out long totalRecords, int proposalCode, string progressType, int contractCode);

        #endregion

        #region Photo Upload
        string AddFileUploadDetailsDAL(MANE_IMS_PROGRESS_FILES lst_inspection_files);

        Array GetFilesListDALProgress(int page, int rows, string sidx, string sord, out int totalRecords, int obsId);

        string DeleteFileDetailsDALProgress(int QM_FILE_ID);
        #endregion
    }
}