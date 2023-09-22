using PMGSY.Common;
using PMGSY.Extensions;
using PMGSY.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web;

namespace PMGSY.Areas.MPRFileDownload.DAL
{
    public class MPRFileUploadDAL : IMPRFileUploadDAL
    {
        PMGSYEntities dbContext = null;
        Dictionary<string, string> decryptedParameters = null;
        string[] encryptedParameters = null;

        public Array ListMPRFileUploadDAL(int stateCode, int? page, int? rows, string sidx, string sord, out long totalRecords)
        {  try
            {
                dbContext = new PMGSYEntities();
                var lstMasterAgencyDetails = dbContext.MPR_FILE_UPLOADED.Where(m => m.MAST_STATE_CODE == stateCode).OrderByDescending(m => m.MPR_FILE_UPLOAD_DATE).ToList();

                totalRecords = lstMasterAgencyDetails.Count();
            

                if (sidx.Trim() != string.Empty)
                {
                    if (sord == "asc")
                    {
                        switch (sidx)
                        {
                            case "MPR_FILE_UPLOAD_DATE":
                                lstMasterAgencyDetails = lstMasterAgencyDetails.OrderBy(m => m.MPR_FILE_UPLOAD_DATE).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                                break;
                            case "MPR_FILE_NAME":
                                lstMasterAgencyDetails = lstMasterAgencyDetails.OrderBy(m => m.MPR_FILE_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                                break;
                            case "MPR_FILE_DESC":
                                lstMasterAgencyDetails = lstMasterAgencyDetails.OrderBy(m => m.MPR_FILE_DESC).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                                break;
                            case "MPR_FILE_TYPE":
                                lstMasterAgencyDetails = lstMasterAgencyDetails.OrderBy(m => m.MPR_FILE_TYPE).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                                break;
                        }
                    }
                    else
                    {
                        switch (sidx)
                        {
                            case "MPR_FILE_UPLOAD_DATE":
                                lstMasterAgencyDetails = lstMasterAgencyDetails.OrderByDescending(m => m.MPR_FILE_UPLOAD_DATE).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                                break;
                            case "MPR_FILE_NAME":
                                lstMasterAgencyDetails = lstMasterAgencyDetails.OrderByDescending(m => m.MPR_FILE_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                                break;
                            case "MPR_FILE_DESC":
                                lstMasterAgencyDetails = lstMasterAgencyDetails.OrderByDescending(m => m.MPR_FILE_DESC).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                                break;
                            case "MPR_FILE_TYPE":
                                lstMasterAgencyDetails = lstMasterAgencyDetails.OrderByDescending(m => m.MPR_FILE_TYPE).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                                break;
                        }
                    }

                }
               

                return lstMasterAgencyDetails.Select(item => new
                {
                    cell = new[]{ 
                                     item.MPR_FILE_UPLOAD_DATE==null?"-":Convert.ToDateTime(item.MPR_FILE_UPLOAD_DATE).ToString("dd/MM/yyyy"),
                                     //"<a href='#' title='Click here to Download this file.'  onClick=DownLoadFile(\"" + URLEncrypt.EncryptParameters(new string[] { item.MPR_FILE_NAME  }) +"\"); return false;'>"+Path.GetFileNameWithoutExtension(item.MPR_FILE_NAME)+"</a>" ,
                                      System.IO.File.Exists(Path.Combine(ConfigurationManager.AppSettings["MPR_FILE_DOWNLOAD"], item.MPR_FILE_NAME))?"<a href='#' title='Click here to Download this file.'  onClick=DownLoadFile(\""+(item.MPR_FILE_NAME.Replace(' ','#'))+"\"); return false;'>"+Path.GetFileNameWithoutExtension(item.MPR_FILE_NAME)+"</a>":"<a href='#' title='Click here to Download this file.'  onClick=DownLoadFileNotExist(); return false;'>"+Path.GetFileNameWithoutExtension(item.MPR_FILE_NAME)+"</a>" ,

                                     item.MPR_FILE_DESC,
                                     item.MPR_FILE_TYPE==1?"Monthly Format-I":  item.MPR_FILE_TYPE==2?"Quarterly Format":item.MPR_FILE_TYPE==3?"Half-Yearly Format":item.MPR_FILE_TYPE==4?"Annual Reporting Format":item.MPR_FILE_TYPE==5?"Monthly Format-II":"",
                                               
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

    }
    public interface IMPRFileUploadDAL
    {
        //ListHabsDAL
        Array ListMPRFileUploadDAL(int stateCode, int? page, int? rows, string sidx, string sord, out long totalRecords);
      
    }
}