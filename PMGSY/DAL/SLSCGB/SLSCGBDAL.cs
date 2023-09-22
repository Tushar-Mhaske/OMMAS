using PMGSY.Common;
using PMGSY.Extensions;
using PMGSY.Models;
using PMGSY.Models.SLSCGB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using System.Web;
using System.Web.Mvc;

namespace PMGSY.DAL.SLSCGB
{
    public class SLSCGBDAL : ISLSCGBDAL
    {
        PMGSYEntities dbContext = null;
        /// <summary>
        /// save the SLSC GB details
        /// </summary>
        /// <param name="progressModel">model containing the physical road details</param>
        /// <param name="message">returns the status of save operation</param>
        /// <returns></returns>
        public bool AddSLSCGBDAL(SLSCGBViewModel model, ref string message)
        {
            dbContext = new PMGSYEntities();
            CommonFunctions comm = new CommonFunctions();
            int maxId = 0;
            try
            {
                using (TransactionScope ts = new TransactionScope())
                {
                    //if (dbContext.MASTER_SLSC_GB_MEETING.Any(m => m.IMS_PR_ROAD_CODE == model.IMS_PR_ROAD_CODE))
                    //{
                    //    message = "Physical Road Progress Details Already Exist.";
                    //    return false;
                    //}

                    MASTER_SLSC_GB_MEETING master_slsc_gb_meeting = new MASTER_SLSC_GB_MEETING();

                    master_slsc_gb_meeting.SLSC_GB_CODE = dbContext.MASTER_SLSC_GB_MEETING.Any() ? dbContext.MASTER_SLSC_GB_MEETING.Select(x => x.SLSC_GB_CODE).Max() + 1 : 1;
                    master_slsc_gb_meeting.MAST_STATE_CODE = model.state;
                    master_slsc_gb_meeting.SLSC_GB = model.meetingFlag;
                    master_slsc_gb_meeting.MEETING_DATE = comm.GetStringToDateTime(model.meetingDate);
                    master_slsc_gb_meeting.FILE_TYPE = model.fileType == "pdf" ? "P" : "D";
                    master_slsc_gb_meeting.FILE_NAME = model.FileName.Trim();
                    master_slsc_gb_meeting.FILE_UPLOAD_DATE = DateTime.Now;

                    master_slsc_gb_meeting.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                    master_slsc_gb_meeting.USERID = PMGSYSession.Current.UserId;

                    dbContext.MASTER_SLSC_GB_MEETING.Add(master_slsc_gb_meeting);
                    dbContext.SaveChanges();
                    ts.Complete();
                }

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
            finally
            {

            }
        }

        /// <summary>
        /// For Listing the Images
        /// </summary>
        /// <param name="page">no. of pages</param>
        /// <param name="rows">no. of rows</param>
        /// <param name="sidx">sort column name</param>
        /// <param name="sord">sort order</param>
        /// <param name="totalRecords">total no. of records</param>
        /// <param name="IMS_PR_ROAD_CODE">id of proposal</param>
        /// <returns></returns>
        public Array GetMeetingListDAL(int page, int rows, string sidx, string sord, out long totalRecords)
        {
            CommonFunctions comm = new CommonFunctions();
            try
            {
                dbContext = new PMGSYEntities();
                var query = (from meeting in dbContext.MASTER_SLSC_GB_MEETING
                             where PMGSYSession.Current.RoleCode == 25 ? (1 == 1) : (meeting.MAST_STATE_CODE == PMGSYSession.Current.StateCode)
                             select new
                             {
                                 meeting.SLSC_GB_CODE,
                                 meeting.MAST_STATE_CODE,
                                 meeting.MASTER_STATE.MAST_STATE_NAME,
                                 meeting.SLSC_GB,
                                 meeting.MEETING_DATE,
                                 meeting.FILE_NAME,
                                 meeting.FILE_TYPE,
                             });
                totalRecords = query.Count();

                if (sidx.Trim() != string.Empty)
                {
                    if (sord.ToString() == "asc")
                    {
                        switch (sidx)
                        {
                            case "State":
                                query = query.OrderBy(m => m.MAST_STATE_CODE).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                                break;
                            case "MeetingDate":
                                query = query.OrderBy(m => m.MEETING_DATE).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                                break;
                            default:
                                query = query.OrderBy(x => x.SLSC_GB_CODE).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                                break;
                        }
                    }
                    else
                    {
                        switch (sidx)
                        {
                            case "State":
                                query = query.OrderByDescending(m => m.MAST_STATE_CODE).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                                break;
                            case "MeetingDate":
                                query = query.OrderByDescending(m => m.MEETING_DATE).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                                break;
                            default:
                                query = query.OrderByDescending(x => x.SLSC_GB_CODE).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                                break;
                        }
                    }
                }
                else
                {
                    query = query.OrderBy(x => x.SLSC_GB_CODE).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                }

                var result = query.Select(m => new
                {
                    m.SLSC_GB_CODE,
                    m.MAST_STATE_NAME,
                    m.SLSC_GB,
                    m.MEETING_DATE,
                    m.FILE_NAME,
                    m.FILE_TYPE,
                }).ToArray();

                return result.Select(meetingDetails => new
                {
                    id = meetingDetails.SLSC_GB_CODE,
                    cell = new[] {   
                                   meetingDetails.MAST_STATE_NAME.Trim(),
                                   comm.GetDateTimeToString(meetingDetails.MEETING_DATE),
                                   meetingDetails.SLSC_GB == "S" ? "SLSC" : "GB",
                                   //meetingDetails.FILE_NAME.Trim(),
                                   //"<a href='#' onclick=downloadMeetingFileFromAction('/SLSCGB/DownloadMeetingFile/" + URLEncrypt.EncryptParameters(new string[] { meetingDetails.FILE_NAME  }) + "'); return false;>" + meetingDetails.FILE_TYPE == "P" ? "<img style='height:16px;width:16px' height='16' width='16' border=0 src='../../Content/images/PDF.ico' />" : "<img style='height:16px;width:16px' height='16' width='16' border=0 src='../../Content/images/Doc.ico' /> </a>",
                                   meetingDetails.FILE_TYPE == "P" ? "<a href='#' title='Click here to Download PDF File' onclick=downloadMeetingFileFromAction('/SLSCGB/DownloadMeetingFile/" + URLEncrypt.EncryptParameters(new string[] { meetingDetails.FILE_NAME  }) + "'); return false;> <img style='height:16px;width:16px' height='16' width='16' border=0 src='../../Content/images/PDF.ico' /> </a>" : "<a href='#' title='Click here to Download Document File' onclick=downloadMeetingFileFromAction('/SLSCGB/DownloadMeetingFile/" + URLEncrypt.EncryptParameters(new string[] { meetingDetails.FILE_NAME  }) + "'); return false;> <img style='height:16px;width:16px' height='16' width='16' border=0 src='../../Content/images/Doc.ico' /> </a>",
                                   PMGSYSession.Current.RoleCode == 25 ? "<a href='#' title='Delete Meeting Details' class='ui-icon ui-icon-trash ui-align-center' onClick=DeleteMeetingDetails('" + URLEncrypt.EncryptParameters1(new string[]{"MeetingCode =" + meetingDetails.SLSC_GB_CODE.ToString().Trim(), "File_Name=" + meetingDetails.FILE_NAME.Trim() }) +"'); return false;'>Delete Meeting Details</a>" : "-"
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

        /// <summary>
        /// Delete Meeting Files
        /// </summary>
        /// <param name="execution_files">file along with details</param>
        /// <returns></returns>
        public string DeleteMeetingDetailsDAL(int meetingCode)
        {
            try
            {
                dbContext = new PMGSYEntities();
                MASTER_SLSC_GB_MEETING master_slsc_gb_meeting = dbContext.MASTER_SLSC_GB_MEETING.Where(
                    a => a.SLSC_GB_CODE == meetingCode).FirstOrDefault();

                dbContext.MASTER_SLSC_GB_MEETING.Remove(master_slsc_gb_meeting);
                dbContext.SaveChanges();
                return string.Empty;
            }
            catch
            {
                return "There is an error while deleting meeting.";
            }
            finally
            {
                dbContext.Dispose();
            }
        }

        #region Meeting Report
        public List<SelectListItem> PopulateStates()
        {
            List<SelectListItem> StatesList = new List<SelectListItem>();
            SelectListItem item = new SelectListItem();
            item.Text = "All States";
            item.Value = "0";
            item.Selected = true;
            StatesList.Add(item);

            var dbContext = new PMGSYEntities();
            try
            {

                var query = (from c in dbContext.MASTER_STATE
                             where c.MAST_STATE_ACTIVE == "Y"
                             select new
                             {
                                 Text = c.MAST_STATE_NAME,
                                 Value = c.MAST_STATE_CODE
                             }).OrderBy(c => c.Text).ToList();

                foreach (var data in query)
                {
                    item = new SelectListItem();
                    item.Text = data.Text;
                    item.Value = data.Value.ToString();
                    StatesList.Add(item);
                }
                return StatesList;
            }
            catch
            {
                return StatesList;
            }
            finally
            {
                dbContext.Dispose();
            }
        }
        #endregion
    }

    public interface ISLSCGBDAL
    {
        bool AddSLSCGBDAL(SLSCGBViewModel model, ref string message);
        Array GetMeetingListDAL(int page, int rows, string sidx, string sord, out long totalRecords);
        string DeleteMeetingDetailsDAL(int meetingCode);
    }
}