using PMGSY.Common;
using PMGSY.Extensions;
using PMGSY.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Transactions;
using System.Web;
using System.Web.Mvc;

namespace PMGSY.DAL.TourClaim
{
    public class TourClaimDAL : ITourClaimDAL
    {
        PMGSYEntities dbContext = new PMGSYEntities();

        #region NQM Tour Claim

        public Array GetNQMCurrScheduleListDAL(int month, int year, int? page, int? rows, string sidx, string sord, out Int32 totalRecords)
        {
            dbContext = new Models.PMGSYEntities();
            try
            {
                //get Current Logged in MonitorID 
                var monitorDetails = (from aqm in dbContext.ADMIN_QUALITY_MONITORS
                                      where aqm.ADMIN_USER_ID == PMGSYSession.Current.UserId
                                      select aqm).First();

                List<qm_monitor_schedule_current_month_onwards_Result> scheduleItemList = new List<qm_monitor_schedule_current_month_onwards_Result>();
                string qmType = string.Empty;
                month = month == 0 ? DateTime.Now.Month : month;
                year = year == 0 ? DateTime.Now.Year : year;

                scheduleItemList = dbContext.qm_monitor_schedule_current_month_onwards(monitorDetails.ADMIN_QM_CODE, monitorDetails.ADMIN_QM_TYPE, month, year).ToList<qm_monitor_schedule_current_month_onwards_Result>();

                totalRecords = scheduleItemList.Count();

                if (sidx.Trim() != string.Empty)
                {
                    if (sord.ToString() == "asc")
                    {
                        scheduleItemList = scheduleItemList.OrderBy(x => x.STATE_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                    }
                    else
                    {
                        scheduleItemList = scheduleItemList.OrderByDescending(x => x.STATE_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                    }
                }
                else
                {
                    scheduleItemList = scheduleItemList.OrderBy(x => x.STATE_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                }

                scheduleItemList = scheduleItemList.OrderBy(x => x.STATE_NAME).ToList();
                return scheduleItemList.Select(schDetails => new
                {
                    id = schDetails.ADMIN_SCHEDULE_CODE.ToString().Trim(),
                    cell = new[] {
                                        (new CommonFunctions().getMonthText(Convert.ToInt16(schDetails.ADMIN_IM_MONTH)) + " " + schDetails.ADMIN_IM_YEAR),
                                        schDetails.STATE_NAME,
                                        schDetails.DISTRICT_NAME,
                                        schDetails.DISTRICT_NAME2,
                                        schDetails.DISTRICT_NAME3,
                                        schDetails.INSP_STATUS_FLAG,
                                        dbContext.QUALITY_QM_OBSERVATION_MASTER.Where(x => x.ADMIN_SCHEDULE_CODE == schDetails.ADMIN_SCHEDULE_CODE).Any()
                                        ? dbContext.NQM_TOUR_CLAIM_MASTER.Where(x => x.ADMIN_SCHEDULE_CODE == schDetails.ADMIN_SCHEDULE_CODE).Any()
                                            ? dbContext.NQM_TOUR_CLAIM_MASTER.Where(x => x.ADMIN_SCHEDULE_CODE == schDetails.ADMIN_SCHEDULE_CODE && (x.FINALIZE_FLAG >= 1)).Any()
                                                ? "<a href='#' title='Click here to view tour details' class='ui-icon ui-icon-zoomin ui-align-center' onClick='ViewTourDetails(\"" +  schDetails.ADMIN_SCHEDULE_CODE.ToString().Trim() +"\"); return false;'></a>"
                                                : "<a href='#' title='Click here to add tour details' class='ui-icon ui-icon-plus ui-align-center' onClick='AddAllTourDetails(\"" +  schDetails.ADMIN_SCHEDULE_CODE.ToString().Trim()  +"\"); return false;'></a>"
                                            : dbContext.ADMIN_QUALITY_MONITORS_BANK.Where(x => x.ADMIN_QM_CODE == monitorDetails.ADMIN_QM_CODE).Any()
                                                ? "<a href='#' title='Click here to add tour details' class='ui-icon ui-icon-plus ui-align-center' onClick='AddNewTourDetails(\"" +  schDetails.ADMIN_SCHEDULE_CODE.ToString().Trim()  +"\"); return false;'></a>"
                                                : "<a href='#' title='Click here to add tour details' class='ui-icon ui-icon-plus ui-align-center' onClick='AddAlert(); return false;'></a>"
                                        : "--"
                        }
                }).ToArray();
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "TourClaimDAL.GetNQMCurrScheduleListDAL()");
                totalRecords = 0;
                return null;
            }
            finally
            {
                dbContext.Dispose();
            }
        }

        public bool InsertTourClaimDetailsDAL(FormCollection formCollection, out String IsValid)
        {
            int status = 0;
            NQM_TOUR_CLAIM_MASTER bankModel = new NQM_TOUR_CLAIM_MASTER();
            NQM_TOUR_MASTER_LETTER_NUMBER letter = new NQM_TOUR_MASTER_LETTER_NUMBER();

            status = dbContext.SaveChanges();
            try
            {
                using (var scope = new TransactionScope())
                {
                    bankModel.TOUR_CLAIM_ID = dbContext.NQM_TOUR_CLAIM_MASTER.Max(cp => (Int32?)cp.TOUR_CLAIM_ID) == null ? 1 : (Int32)dbContext.NQM_TOUR_CLAIM_MASTER.Max(cp => (Int32?)cp.TOUR_CLAIM_ID) + 1;
                    bankModel.ADMIN_QM_CODE = Convert.ToInt32(formCollection["ADMIN_QM_CODE"]);
                    bankModel.ADMIN_SCHEDULE_CODE = Convert.ToInt32(formCollection["ADMIN_SCHEDULE_CODE"]);
                    bankModel.DATE_OF_CLAIM = Convert.ToDateTime(formCollection["DATE_OF_CLAIM"]);
                    bankModel.MONTH_OF_INSPECTION = DateTime.ParseExact(formCollection["MONTH_OF_INSPECTION"], "MMMM", CultureInfo.CurrentCulture).Month;
                    bankModel.YEAR_OF_INSPECTION = Convert.ToInt32(formCollection["YEAR_OF_INSPECTION"]);
                    bankModel.NRRDA_LETTER_NUMBER = formCollection["NRRDA_LETTER_NUMBER"];
                    bankModel.FINALIZE_FLAG = 0;
                    bankModel.ROUNDS_SEQUENCE = 0;
                    bankModel.USERID = PMGSYSession.Current.UserId;
                    bankModel.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];

                    dbContext.NQM_TOUR_CLAIM_MASTER.Add(bankModel);
                    dbContext.SaveChanges();

                    string letterNumber = bankModel.NRRDA_LETTER_NUMBER;
                    string[] letterDetails = letterNumber.Split('/');
                    letter.LETTER_ID = dbContext.NQM_TOUR_MASTER_LETTER_NUMBER.Max(x => (Int32?)x.LETTER_ID) == null ? 1 : (Int32)dbContext.NQM_TOUR_MASTER_LETTER_NUMBER.Max(x => (Int32?)x.LETTER_ID) + 1;
                    letter.QM_TYPE = letterDetails[2];
                    letter.FINANCIAL_YEAR = letterDetails[1];
                    letter.RUNNING_NUM = Convert.ToInt32(letterDetails[3]);

                    dbContext.NQM_TOUR_MASTER_LETTER_NUMBER.Add(letter);
                    dbContext.SaveChanges();

                    status = 1;

                    scope.Complete();
                }
                if (status > 0)
                {
                    IsValid = "Tour claim details saved successfully.";
                    return true;
                }

                IsValid = "Tour claim details saved successfully.";
                return true;
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "TourClaimDAL.InsertTourClaimDetailsDAL()");
                IsValid = "Tour claim details not saved.";
                return false;
            }
        }

        public bool FinalizeTourDetailDAL(int scheduleCode, out String IsValid)
        {
            NQM_TOUR_CLAIM_MASTER tourModel = new NQM_TOUR_CLAIM_MASTER();
            NQM_TOUR_DISTRICT_DETAILS districtModel = new NQM_TOUR_DISTRICT_DETAILS();
            NQM_TRAVEL_CLAIM_DETAILS travelModel = new NQM_TRAVEL_CLAIM_DETAILS();
            NQM_LODGE_AND_DAILY_CLAIM_DETAILS lodgeModel = new NQM_LODGE_AND_DAILY_CLAIM_DETAILS();
            NQM_TOUR_HONORARIUM_INSPECTION_OF_ROADS inspModel = new NQM_TOUR_HONORARIUM_INSPECTION_OF_ROADS();
            NQM_TOUR_HONORARIUM_MEETING_WITH_PIU meetingModel = new NQM_TOUR_HONORARIUM_MEETING_WITH_PIU();
            NQM_TOUR_MISCELLANEOUS misModel = new NQM_TOUR_MISCELLANEOUS();

            int status = 0;
            status = dbContext.SaveChanges();
            try
            {
                using (var scope = new TransactionScope())
                {
                    int tourClaimId = dbContext.NQM_TOUR_CLAIM_MASTER.Where(x => x.ADMIN_SCHEDULE_CODE == scheduleCode).Select(y => y.TOUR_CLAIM_ID).FirstOrDefault();

                    List<int> districtIds = new List<int>();

                    districtIds = dbContext.NQM_TOUR_DISTRICT_DETAILS.Where(x => x.TOUR_CLAIM_ID == tourClaimId).Select(y => y.DISTRICT_DETAILS_ID).ToList();

                    if (districtIds.Count < 1)
                    {
                        IsValid = "Please add atleast 1 district before finalizing.";
                        return false;
                    }

                    List<int> travelIds = new List<int>();

                    travelIds = dbContext.NQM_TRAVEL_CLAIM_DETAILS.Where(x => x.TOUR_CLAIM_ID == tourClaimId).Select(y => y.TRAVEL_CLAIM_ID).ToList();

                    if (travelIds.Count < 1)
                    {
                        IsValid = "Please add atleast 1 travel claim before finalizing.";
                        return false;
                    }

                    List<int> lodgeIds = new List<int>();

                    lodgeIds = dbContext.NQM_LODGE_AND_DAILY_CLAIM_DETAILS.Where(x => x.TOUR_CLAIM_ID == tourClaimId).Select(y => y.LODGE_CLAIM_ID).ToList();

                    if (lodgeIds.Count < 1)
                    {
                        IsValid = "Please add atleast 1 lodge or daily claim before finalizing.";
                        return false;
                    }

                    List<int> inspIds = new List<int>();

                    inspIds = dbContext.NQM_TOUR_HONORARIUM_INSPECTION_OF_ROADS.Where(x => x.TOUR_CLAIM_ID == tourClaimId).Select(y => y.HONORARIUM_INSPECTION_ID).ToList();

                    if (inspIds.Count < 1)
                    {
                        IsValid = "Please add atleast 1 inspection of roads allowance before finalizing.";
                        return false;
                    }

                    List<int> meetingIds = new List<int>();

                    meetingIds = dbContext.NQM_TOUR_HONORARIUM_MEETING_WITH_PIU.Where(x => x.TOUR_CLAIM_ID == tourClaimId).Select(y => y.HONORARIUM_MEETING_ID).ToList();

                    if (meetingIds.Count < 1)
                    {
                        IsValid = "Please add atleast 1 meeting with PIU allowance before finalizing.";
                        return false;
                    }

                    tourModel = dbContext.NQM_TOUR_CLAIM_MASTER.Where(x => x.TOUR_CLAIM_ID == tourClaimId).FirstOrDefault();
                    tourModel.FINALIZE_FLAG = 1;
                    tourModel.NQM_FINALIZATION_DATE = DateTime.Now;

                    dbContext.Entry(tourModel).State = System.Data.Entity.EntityState.Modified;
                    dbContext.SaveChanges();

                    status = 1;

                    scope.Complete();
                }
                if (status > 0)
                {
                    IsValid = "Tour details finalized successfully.";
                    return true;
                }

                IsValid = "Tour details finalized successfully.";
                return true;
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "TourClaimDAL.FinalizeTourDetailDAL()");
                IsValid = "Tour details not finalized.";
                return false;
            }
        }

        public bool InsertDistrictDetailsDAL(FormCollection formCollection, out String IsValid)
        {
            int status = 0;
            NQM_TOUR_DISTRICT_DETAILS tourModel = new NQM_TOUR_DISTRICT_DETAILS();

            status = dbContext.SaveChanges();
            try
            {
                using (var scope = new TransactionScope())
                {
                    tourModel.DISTRICT_DETAILS_ID = dbContext.NQM_TOUR_DISTRICT_DETAILS.Max(cp => (Int32?)cp.DISTRICT_DETAILS_ID) == null ? 1 : (Int32)dbContext.NQM_TOUR_DISTRICT_DETAILS.Max(cp => (Int32?)cp.DISTRICT_DETAILS_ID) + 1;
                    tourModel.TOUR_CLAIM_ID = Convert.ToInt32(formCollection["TOUR_CLAIM_ID"]);
                    tourModel.DATE_OF_ENTRY = DateTime.Now;
                    tourModel.DISTRICT = Convert.ToInt32(formCollection["DISTRICT_CODE"]);
                    tourModel.DATE_FROM = Convert.ToDateTime(formCollection["DATE_FROM"]);
                    tourModel.DATE_TO = Convert.ToDateTime(formCollection["DATE_TO"]);
                    tourModel.USERID = PMGSYSession.Current.UserId;
                    tourModel.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];

                    dbContext.NQM_TOUR_DISTRICT_DETAILS.Add(tourModel);
                    status = dbContext.SaveChanges();
                    status = 1;

                    scope.Complete();
                }
                if (status > 0)
                {
                    IsValid = "District details saved successfully.";
                    return true;
                }

                IsValid = "District details saved successfully.";
                return true;
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "TourClaimDAL.InsertDistrictDetailsDAL()");
                IsValid = "District details not saved.";
                return false;
            }
        }

        public Array GetTourDistrictListDAL(int scheduleCode, int page, int rows, string sidx, string sord, out long totalRecords)
        {
            try
            {
                var lstExecution = (from item in dbContext.NQM_TOUR_DISTRICT_DETAILS
                                    join master in dbContext.NQM_TOUR_CLAIM_MASTER on item.TOUR_CLAIM_ID equals master.TOUR_CLAIM_ID
                                    join district in dbContext.MASTER_DISTRICT on item.DISTRICT equals district.MAST_DISTRICT_CODE
                                    where
                                    master.ADMIN_SCHEDULE_CODE == scheduleCode
                                    select new
                                    {
                                        item.TOUR_CLAIM_ID,
                                        master.ADMIN_SCHEDULE_CODE,
                                        item.DISTRICT_DETAILS_ID,
                                        district.MAST_DISTRICT_NAME,
                                        item.DATE_FROM,
                                        item.DATE_TO,
                                        master.FINALIZE_FLAG,
                                        item.DISTRICT
                                    }).Distinct();

                totalRecords = lstExecution.Count();

                if (sidx.Trim() != string.Empty)
                {
                    if (sord.ToString() == "asc")
                    {
                        switch (sidx)
                        {
                            case "DATE_FROM":
                                lstExecution = lstExecution.OrderBy(m => m.DATE_FROM).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                                break;
                            default:
                                lstExecution = lstExecution.OrderBy(m => m.DATE_FROM).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                                break;
                        }
                    }
                    else
                    {
                        switch (sidx)
                        {
                            case "DATE_FROM":
                                lstExecution = lstExecution.OrderBy(m => m.DATE_FROM).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                                break;
                            default:
                                lstExecution = lstExecution.OrderBy(m => m.DATE_FROM).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                                break;
                        }
                    }
                }
                else
                {
                    lstExecution = lstExecution.OrderByDescending(m => m.DATE_FROM).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                }

                var result = lstExecution.Select(executionDetails => new
                {
                    executionDetails.TOUR_CLAIM_ID,
                    executionDetails.ADMIN_SCHEDULE_CODE,
                    executionDetails.DISTRICT_DETAILS_ID,
                    executionDetails.MAST_DISTRICT_NAME,
                    executionDetails.DATE_FROM,
                    executionDetails.DATE_TO,
                    executionDetails.FINALIZE_FLAG,
                    executionDetails.DISTRICT
                }).ToArray();

                return result.Select(m => new
                {
                    id = m.TOUR_CLAIM_ID.ToString(),
                    cell = new[]
                    {
                        m.TOUR_CLAIM_ID.ToString(),
                        m.MAST_DISTRICT_NAME,
                        m.DATE_FROM.ToShortDateString(),
                        m.DATE_TO.ToShortDateString(),
                        //"<center><table><tr><td  style='border-color:white'><span class='ui-icon ui-icon-zoomin' title='Click here to View Quick Response Sheet' onClick ='ViewQuickResponseSheetReport(\"" + m.DISTRICT_DETAILS_ID + "\");' ></span></td></tr></table></center>",
                        (m.FINALIZE_FLAG >= 1)
                        ? "<a href='#' class='ui-icon ui-icon-locked ui-align-center'>"
                        : "<center><table><tr><td  style='border-color:white'><span class='ui-icon ui-icon-trash' title='Click here to Delete Details' onClick ='DeleteDistrictDetails(\"" + m.DISTRICT_DETAILS_ID + "$" + m.ADMIN_SCHEDULE_CODE + "\");' ></span></td></tr></table></center>",
                    }
                }).ToArray();
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "TourClaimDAL.GetTourDistrictListDAL()");
                totalRecords = 0;
                return null;
            }
            finally
            {
                dbContext.Dispose();
            }
        }

        public bool InsertTravelClaimDetailsDAL(FormCollection formCollection, HttpPostedFileBase boardingPass, HttpPostedFileBase travelTicket, out String IsValid)
        {
            int status = 0;
            NQM_TRAVEL_CLAIM_DETAILS travelModel = new NQM_TRAVEL_CLAIM_DETAILS();

            status = dbContext.SaveChanges();
            try
            {
                using (var scope = new TransactionScope())
                {
                    travelModel.TRAVEL_CLAIM_ID = dbContext.NQM_TRAVEL_CLAIM_DETAILS.Max(cp => (Int32?)cp.TRAVEL_CLAIM_ID) == null ? 1 : (Int32)dbContext.NQM_TRAVEL_CLAIM_DETAILS.Max(cp => (Int32?)cp.TRAVEL_CLAIM_ID) + 1;
                    travelModel.TOUR_CLAIM_ID = Convert.ToInt32(formCollection["TOUR_CLAIM_ID"]);
                    travelModel.DATE_OF_CLAIM = DateTime.Now;
                    travelModel.START_DATE_OF_TRAVEL = Convert.ToDateTime(formCollection["START_DATE_OF_TRAVEL"]);
                    travelModel.START_HOURS = Convert.ToInt32(formCollection["START_HOURS"]);
                    travelModel.START_MINUTES = Convert.ToInt32(formCollection["START_MINUTES"]);
                    travelModel.END_DATE_OF_TRAVEL = Convert.ToDateTime(formCollection["END_DATE_OF_TRAVEL"]);
                    travelModel.END_HOURS = Convert.ToInt32(formCollection["END_HOURS"]);
                    travelModel.END_MINUTES = Convert.ToInt32(formCollection["END_MINUTES"]);
                    travelModel.DEPARTURE_FROM = formCollection["DEPARTURE_FROM"];
                    travelModel.ARRIVAL_AT = formCollection["ARRIVAL_AT"];
                    travelModel.MODE_OF_TRAVEL = Convert.ToInt32(formCollection["MODE_OF_TRAVEL"]);
                    travelModel.TRAVEL_CLASS = formCollection["TRAVEL_CLASS"];
                    travelModel.AMOUNT_CLAIMED = Convert.ToDecimal(formCollection["AMOUNT_CLAIMED"]);
                    travelModel.AMOUNT_PASSED_CQC = Convert.ToDecimal(formCollection["AMOUNT_CLAIMED"]);
                    travelModel.USERID = PMGSYSession.Current.UserId;
                    travelModel.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];

                    string currYear = DateTime.Now.Year.ToString();
                    String FilePath = ConfigurationManager.AppSettings["NQM_TOUR_CLAIM_PATH"];
                    var fullPath = Path.Combine(FilePath, "TravelClaim", currYear);

                    if (!Directory.Exists(fullPath))
                    {
                        Directory.CreateDirectory(fullPath);
                    }

                    DateTime d = DateTime.Now;
                    String format = "d_MMM_yyyy_HH_mm_ss";
                    String currentTime = d.ToString(format);
                    String FileNameBP = string.Empty;
                    String FileName = string.Empty;

                    if (Convert.ToInt32(formCollection["MODE_OF_TRAVEL"]) == 2)
                    {
                        if (boardingPass.FileName.Substring(boardingPass.FileName.LastIndexOf('.') + 1).Trim().ToLower() == "jpg")
                            FileNameBP = currYear + "_TC_BP_" + formCollection["TOUR_CLAIM_ID"] + "_" + travelModel.TRAVEL_CLAIM_ID + "_" + currentTime + ".jpg";
                        else if (boardingPass.FileName.Substring(boardingPass.FileName.LastIndexOf('.') + 1).Trim().ToLower() == "jpeg")
                            FileNameBP = currYear + "_TC_BP_" + formCollection["TOUR_CLAIM_ID"] + "_" + travelModel.TRAVEL_CLAIM_ID + "_" + currentTime + ".jpeg";
                        else if (boardingPass.FileName.Substring(boardingPass.FileName.LastIndexOf('.') + 1).Trim().ToLower() == "png")
                            FileNameBP = currYear + "_TC_BP_" + formCollection["TOUR_CLAIM_ID"] + "_" + travelModel.TRAVEL_CLAIM_ID + "_" + currentTime + ".png";
                        else if (boardingPass.FileName.Substring(boardingPass.FileName.LastIndexOf('.') + 1).Trim().ToLower() == "pdf")
                            FileNameBP = currYear + "_TC_BP_" + formCollection["TOUR_CLAIM_ID"] + "_" + travelModel.TRAVEL_CLAIM_ID + "_" + currentTime + ".pdf";

                        boardingPass.SaveAs(System.IO.Path.Combine(fullPath, FileNameBP));
                        travelModel.BOARDING_PASS_PATH = fullPath;
                        travelModel.BOARDING_PASS_NAME = FileNameBP;

                        if (travelTicket.FileName.Substring(travelTicket.FileName.LastIndexOf('.') + 1).Trim().ToLower() == "jpg")
                            FileName = currYear + "_TC_" + formCollection["TOUR_CLAIM_ID"] + "_" + travelModel.TRAVEL_CLAIM_ID + "_" + currentTime + ".jpg";
                        else if (travelTicket.FileName.Substring(travelTicket.FileName.LastIndexOf('.') + 1).Trim().ToLower() == "jpeg")
                            FileName = currYear + "_TC_" + formCollection["TOUR_CLAIM_ID"] + "_" + travelModel.TRAVEL_CLAIM_ID + "_" + currentTime + ".jpeg";
                        else if (travelTicket.FileName.Substring(travelTicket.FileName.LastIndexOf('.') + 1).Trim().ToLower() == "png")
                            FileName = currYear + "_TC_" + formCollection["TOUR_CLAIM_ID"] + "_" + travelModel.TRAVEL_CLAIM_ID + "_" + currentTime + ".png";
                        else if (travelTicket.FileName.Substring(travelTicket.FileName.LastIndexOf('.') + 1).Trim().ToLower() == "pdf")
                            FileName = currYear + "_TC_" + formCollection["TOUR_CLAIM_ID"] + "_" + travelModel.TRAVEL_CLAIM_ID + "_" + currentTime + ".pdf";

                        travelTicket.SaveAs(System.IO.Path.Combine(fullPath, FileName));
                        travelModel.UPLOADED_TICKET_PATH = fullPath;
                        travelModel.UPLOADED_TICKET_NAME = FileName;
                    }
                    else if (Convert.ToInt32(formCollection["MODE_OF_TRAVEL"]) != 5)
                    {
                        if (travelTicket.FileName.Substring(travelTicket.FileName.LastIndexOf('.') + 1).Trim().ToLower() == "jpg")
                            FileName = currYear + "_TC_" + formCollection["TOUR_CLAIM_ID"] + "_" + travelModel.TRAVEL_CLAIM_ID + "_" + currentTime + ".jpg";
                        else if (travelTicket.FileName.Substring(travelTicket.FileName.LastIndexOf('.') + 1).Trim().ToLower() == "jpeg")
                            FileName = currYear + "_TC_" + formCollection["TOUR_CLAIM_ID"] + "_" + travelModel.TRAVEL_CLAIM_ID + "_" + currentTime + ".jpeg";
                        else if (travelTicket.FileName.Substring(travelTicket.FileName.LastIndexOf('.') + 1).Trim().ToLower() == "png")
                            FileName = currYear + "_TC_" + formCollection["TOUR_CLAIM_ID"] + "_" + travelModel.TRAVEL_CLAIM_ID + "_" + currentTime + ".png";
                        else if (travelTicket.FileName.Substring(travelTicket.FileName.LastIndexOf('.') + 1).Trim().ToLower() == "pdf")
                            FileName = currYear + "_TC_" + formCollection["TOUR_CLAIM_ID"] + "_" + travelModel.TRAVEL_CLAIM_ID + "_" + currentTime + ".pdf";

                        travelTicket.SaveAs(System.IO.Path.Combine(fullPath, FileName));
                        travelModel.UPLOADED_TICKET_PATH = fullPath;
                        travelModel.UPLOADED_TICKET_NAME = FileName;
                    }

                    dbContext.NQM_TRAVEL_CLAIM_DETAILS.Add(travelModel);
                    status = dbContext.SaveChanges();
                    status = 1;

                    scope.Complete();
                }
                if (status > 0)
                {
                    IsValid = "Travel Claim details saved successfully.";
                    return true;
                }

                IsValid = "Travel Claim details saved successfully.";
                return true;
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "TourClaimDAL.InsertTravelClaimDetailsDAL()");
                IsValid = "Travel Claim details not saved.";
                return false;
            }
        }

        public Array GetTravelClaimListDAL(int scheduleCode, int page, int rows, string sidx, string sord, out long totalRecords)
        {
            try
            {
                var lstExecution = (from travel in dbContext.NQM_TRAVEL_CLAIM_DETAILS
                                    join master in dbContext.NQM_TOUR_CLAIM_MASTER on travel.TOUR_CLAIM_ID equals master.TOUR_CLAIM_ID
                                    join modeOfTravel in dbContext.NQM_TOUR_CLAIM_MODE_OF_TRAVEL on travel.MODE_OF_TRAVEL equals modeOfTravel.ID
                                    where
                                    master.ADMIN_SCHEDULE_CODE == scheduleCode
                                    select new
                                    {
                                        travel.TOUR_CLAIM_ID,
                                        travel.TRAVEL_CLAIM_ID,
                                        master.ADMIN_SCHEDULE_CODE,
                                        travel.START_DATE_OF_TRAVEL,
                                        travel.START_HOURS,
                                        travel.START_MINUTES,
                                        travel.END_DATE_OF_TRAVEL,
                                        travel.END_HOURS,
                                        travel.END_MINUTES,
                                        travel.DEPARTURE_FROM,
                                        travel.ARRIVAL_AT,
                                        modeOfTravel.MODE_OF_TRAVEL,
                                        travel.TRAVEL_CLASS,
                                        travel.AMOUNT_CLAIMED,
                                        travel.AMOUNT_PASSED_CQC,
                                        travel.DATE_OF_MODIFICATION_CQC,
                                        master.FINALIZE_FLAG,
                                        travel.UPLOADED_TICKET_NAME,
                                        travel.BOARDING_PASS_NAME,
                                        master.CQC_APPROVE_DATE
                                    }).Distinct();

                totalRecords = lstExecution.Count();

                if (sidx.Trim() != string.Empty)
                {
                    if (sord.ToString() == "asc")
                    {
                        switch (sidx)
                        {
                            case "START_DATE_OF_TRAVEL":
                                lstExecution = lstExecution.OrderBy(m => m.START_DATE_OF_TRAVEL).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                                break;
                            default:
                                lstExecution = lstExecution.OrderBy(m => m.START_DATE_OF_TRAVEL).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                                break;
                        }
                    }
                    else
                    {
                        switch (sidx)
                        {
                            case "START_DATE_OF_TRAVEL":
                                lstExecution = lstExecution.OrderBy(m => m.START_DATE_OF_TRAVEL).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                                break;
                            default:
                                lstExecution = lstExecution.OrderBy(m => m.START_DATE_OF_TRAVEL).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                                break;
                        }
                    }
                }
                else
                {
                    lstExecution = lstExecution.OrderBy(m => m.START_DATE_OF_TRAVEL).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                }

                var result = lstExecution.Select(executionDetails => new
                {
                    executionDetails.TOUR_CLAIM_ID,
                    executionDetails.TRAVEL_CLAIM_ID,
                    executionDetails.ADMIN_SCHEDULE_CODE,
                    executionDetails.START_DATE_OF_TRAVEL,
                    executionDetails.START_HOURS,
                    executionDetails.START_MINUTES,
                    executionDetails.END_DATE_OF_TRAVEL,
                    executionDetails.END_HOURS,
                    executionDetails.END_MINUTES,
                    executionDetails.DEPARTURE_FROM,
                    executionDetails.ARRIVAL_AT,
                    executionDetails.MODE_OF_TRAVEL,
                    executionDetails.TRAVEL_CLASS,
                    executionDetails.AMOUNT_CLAIMED,
                    executionDetails.AMOUNT_PASSED_CQC,
                    executionDetails.DATE_OF_MODIFICATION_CQC,
                    executionDetails.FINALIZE_FLAG,
                    executionDetails.UPLOADED_TICKET_NAME,
                    executionDetails.BOARDING_PASS_NAME,
                    executionDetails.CQC_APPROVE_DATE
                }).ToArray();

                return result.Select(m => new
                {
                    id = m.TOUR_CLAIM_ID.ToString(),
                    cell = new[]
                    {
                        m.TOUR_CLAIM_ID.ToString(),
                        m.TRAVEL_CLAIM_ID.ToString(),
                        m.START_DATE_OF_TRAVEL.ToShortDateString() + " " + string.Format("{0:00}", m.START_HOURS) + ":" + string.Format("{0:00}", m.START_MINUTES),
                        m.END_DATE_OF_TRAVEL.ToShortDateString() + " " + string.Format("{0:00}", m.END_HOURS) + ":" + string.Format("{0:00}", m.END_MINUTES),
                        m.DEPARTURE_FROM,
                        m.ARRIVAL_AT,
                        m.MODE_OF_TRAVEL,
                        m.TRAVEL_CLASS == null ? "--" : m.TRAVEL_CLASS,
                        m.AMOUNT_CLAIMED.ToString(),
                        m.FINALIZE_FLAG == 5 ? ((m.AMOUNT_PASSED_CQC.ToString() == null || m.AMOUNT_PASSED_CQC.ToString() == string.Empty) ? "--" : m.AMOUNT_PASSED_CQC.ToString()) : "--",
                        m.FINALIZE_FLAG == 5 ? ((m.CQC_APPROVE_DATE.ToString() == null || m.CQC_APPROVE_DATE.ToString() == string.Empty) ? "--" : string.Format("{0:d}", m.CQC_APPROVE_DATE)) : "--",
                        m.MODE_OF_TRAVEL == "Own Car" ? "--" : "<a href='/TourClaim/ViewUploadedTravelTicket?id1=" +URLEncrypt.EncryptParameters1(new String[]{"FileName ="+m.UPLOADED_TICKET_NAME})+"' title='Click here to view uploaded file' class='ui-icon ui-icon-search  ui-align-center' target=_blank></a>",
                        m.MODE_OF_TRAVEL == "Flight" ? "<a href='/TourClaim/ViewUploadedBoardingPass?id1=" +URLEncrypt.EncryptParameters1(new String[]{"FileName ="+m.BOARDING_PASS_NAME})+"' title='Click here to view uploaded boarding pass' class='ui-icon 	ui-icon-search  ui-align-center' target=_blank></a>" : "--",
                        (m.FINALIZE_FLAG >= 1)
                        ? "<a href='#' class='ui-icon ui-icon-locked ui-align-center'>"
                        : "<center><table><tr><td  style='border-color:white'><span class='ui-icon ui-icon-pencil' title='Click here to Edit Details' onClick ='EditTravelDetails(\"" + m.TRAVEL_CLAIM_ID + "\");' ></span></td></tr></table></center>",
                        (m.FINALIZE_FLAG >= 1)
                        ? "<a href='#' class='ui-icon ui-icon-locked ui-align-center'>"
                        : "<center><table><tr><td  style='border-color:white'><span class='ui-icon ui-icon-trash' title='Click here to Delete Details' onClick ='DeleteTravelDetails(\"" + m.TRAVEL_CLAIM_ID + "$" + m.ADMIN_SCHEDULE_CODE + "\");' ></span></td></tr></table></center>",
                    }
                }).ToArray();
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "TourClaimDAL.GetTravelClaimListDAL()");
                totalRecords = 0;
                return null;
            }
            finally
            {
                dbContext.Dispose();
            }
        }

        public bool UpdateTravelClaimDetailsDAL(FormCollection formCollection, HttpPostedFileBase boardingPass, HttpPostedFileBase travelTicket, out String IsValid)
        {
            int status = 0;
            NQM_TRAVEL_CLAIM_DETAILS travelModel = new NQM_TRAVEL_CLAIM_DETAILS();
            int travelId = Convert.ToInt32(formCollection["TRAVEL_CLAIM_ID"]);
            int modeOfTravel;

            status = dbContext.SaveChanges();
            try
            {
                using (var scope = new TransactionScope())
                {
                    travelModel = dbContext.NQM_TRAVEL_CLAIM_DETAILS.Where(x => x.TRAVEL_CLAIM_ID == travelId).FirstOrDefault();
                    modeOfTravel = travelModel.MODE_OF_TRAVEL;
                    travelModel.DATE_OF_CLAIM = DateTime.Now;
                    travelModel.START_HOURS = Convert.ToInt32(formCollection["START_HOURS"]);
                    travelModel.START_MINUTES = Convert.ToInt32(formCollection["START_MINUTES"]);
                    travelModel.END_HOURS = Convert.ToInt32(formCollection["END_HOURS"]);
                    travelModel.END_MINUTES = Convert.ToInt32(formCollection["END_MINUTES"]);
                    travelModel.START_DATE_OF_TRAVEL = Convert.ToDateTime(formCollection["START_DATE_OF_TRAVEL"]);
                    travelModel.END_DATE_OF_TRAVEL = Convert.ToDateTime(formCollection["END_DATE_OF_TRAVEL"]);
                    travelModel.DEPARTURE_FROM = formCollection["DEPARTURE_FROM"];
                    travelModel.ARRIVAL_AT = formCollection["ARRIVAL_AT"];
                    travelModel.TRAVEL_CLASS = formCollection["TRAVEL_CLASS"] == "-1" ? null : formCollection["TRAVEL_CLASS"];
                    travelModel.AMOUNT_CLAIMED = Convert.ToDecimal(formCollection["AMOUNT_CLAIMED"]);
                    travelModel.AMOUNT_PASSED_CQC = Convert.ToDecimal(formCollection["AMOUNT_CLAIMED"]);

                    String oldBpName = string.Empty;
                    String bpPath = string.Empty;
                    String fullPathBP = string.Empty;
                    string NewBpName = string.Empty;

                    String oldTicketName = string.Empty;
                    String ticketPath = string.Empty;
                    String fullPathTicket = string.Empty;
                    string NewTicketName = string.Empty;

                    if (modeOfTravel == 2)
                    {
                        if (boardingPass.ContentLength != 0)
                        {
                            // First delete the existing file
                            oldBpName = travelModel.BOARDING_PASS_NAME;
                            bpPath = travelModel.BOARDING_PASS_PATH;
                            fullPathBP = Path.Combine(bpPath, oldBpName);
                            FileInfo file = new FileInfo(fullPathBP);

                            if (file.Exists)
                            {
                                file.Delete();
                            }

                            // Create new file name with old file and replace the extention.
                            string fileName = oldBpName.Split('.')[0];

                            // Create new name with corrrect extention.
                            if (boardingPass.FileName.Substring(boardingPass.FileName.LastIndexOf('.') + 1).Trim().ToLower() == "jpg")
                                NewBpName = fileName + ".jpg";
                            else if (boardingPass.FileName.Substring(boardingPass.FileName.LastIndexOf('.') + 1).Trim().ToLower() == "jpeg")
                                NewBpName = fileName + ".jpeg";
                            else if (boardingPass.FileName.Substring(boardingPass.FileName.LastIndexOf('.') + 1).Trim().ToLower() == "png")
                                NewBpName = fileName + ".png";
                            else if (boardingPass.FileName.Substring(boardingPass.FileName.LastIndexOf('.') + 1).Trim().ToLower() == "pdf")
                                NewBpName = fileName + ".pdf";

                            // Save new file with same and same path.
                            boardingPass.SaveAs(System.IO.Path.Combine(bpPath, NewBpName));
                            travelModel.BOARDING_PASS_NAME = NewBpName;
                        }

                        if (travelTicket.ContentLength != 0)
                        {
                            // First delete the existing file
                            oldTicketName = travelModel.UPLOADED_TICKET_NAME;
                            ticketPath = travelModel.UPLOADED_TICKET_PATH;
                            fullPathTicket = Path.Combine(ticketPath, oldTicketName);
                            FileInfo file = new FileInfo(fullPathTicket);

                            if (file.Exists)
                            {
                                file.Delete();
                            }

                            // Create new file name with old file and replace the extention.
                            string fileName = oldTicketName.Split('.')[0];

                            // Create new name with corrrect extention.
                            if (travelTicket.FileName.Substring(travelTicket.FileName.LastIndexOf('.') + 1).Trim().ToLower() == "jpg")
                                NewTicketName = fileName + ".jpg";
                            else if (travelTicket.FileName.Substring(travelTicket.FileName.LastIndexOf('.') + 1).Trim().ToLower() == "jpeg")
                                NewTicketName = fileName + ".jpeg";
                            else if (travelTicket.FileName.Substring(travelTicket.FileName.LastIndexOf('.') + 1).Trim().ToLower() == "png")
                                NewTicketName = fileName + ".png";
                            else if (travelTicket.FileName.Substring(travelTicket.FileName.LastIndexOf('.') + 1).Trim().ToLower() == "pdf")
                                NewTicketName = fileName + ".pdf";

                            // Save new file with same and same path.
                            travelTicket.SaveAs(System.IO.Path.Combine(ticketPath, NewTicketName));
                            travelModel.UPLOADED_TICKET_NAME = NewTicketName;
                        }
                    }
                    else if (modeOfTravel != 5)
                    {
                        if (travelTicket.ContentLength != 0)
                        {
                            // First delete the existing file
                            oldTicketName = travelModel.UPLOADED_TICKET_NAME;
                            ticketPath = travelModel.UPLOADED_TICKET_PATH;
                            fullPathTicket = Path.Combine(ticketPath, oldTicketName);
                            FileInfo file = new FileInfo(fullPathTicket);

                            if (file.Exists)
                            {
                                file.Delete();
                            }

                            // Create new file name with old file and replace the extention.
                            string fileName = oldTicketName.Split('.')[0];

                            // Create new name with corrrect extention.
                            if (travelTicket.FileName.Substring(travelTicket.FileName.LastIndexOf('.') + 1).Trim().ToLower() == "jpg")
                                NewTicketName = fileName + ".jpg";
                            else if (travelTicket.FileName.Substring(travelTicket.FileName.LastIndexOf('.') + 1).Trim().ToLower() == "jpeg")
                                NewTicketName = fileName + ".jpeg";
                            else if (travelTicket.FileName.Substring(travelTicket.FileName.LastIndexOf('.') + 1).Trim().ToLower() == "png")
                                NewTicketName = fileName + ".png";
                            else if (travelTicket.FileName.Substring(travelTicket.FileName.LastIndexOf('.') + 1).Trim().ToLower() == "pdf")
                                NewTicketName = fileName + ".pdf";

                            // Save new file with same and same path.
                            travelTicket.SaveAs(System.IO.Path.Combine(ticketPath, NewTicketName));
                            travelModel.UPLOADED_TICKET_NAME = NewTicketName;
                        }
                    }

                    dbContext.Entry(travelModel).State = System.Data.Entity.EntityState.Modified;
                    dbContext.SaveChanges();
                    status = 1;

                    scope.Complete();
                }
                if (status > 0)
                {
                    IsValid = "Travel Claim details updated successfully.";
                    return true;
                }

                IsValid = "Travel Claim details updated successfully.";
                return true;
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "TourClaimDAL.UpdateTravelClaimDetailsDAL()");
                IsValid = "Travel Claim details not updated.";
                return false;
            }
        }

        public bool InsertLodgeClaimDetailsDAL(FormCollection formCollection, HttpPostedFileBase bill, HttpPostedFileBase receipt, HttpPostedFileBase gBill, out String IsValid)
        {
            int status = 0;
            NQM_LODGE_AND_DAILY_CLAIM_DETAILS lodgeModel = new NQM_LODGE_AND_DAILY_CLAIM_DETAILS();
            NQM_TOUR_ALLOWANCE_MASTER allowanceModel = new NQM_TOUR_ALLOWANCE_MASTER();
            string currYear = DateTime.Now.Year.ToString();

            status = dbContext.SaveChanges();
            try
            {
                using (var scope = new TransactionScope())
                {
                    lodgeModel.LODGE_CLAIM_ID = dbContext.NQM_LODGE_AND_DAILY_CLAIM_DETAILS.Max(cp => (Int32?)cp.LODGE_CLAIM_ID) == null ? 1 : (Int32)dbContext.NQM_LODGE_AND_DAILY_CLAIM_DETAILS.Max(cp => (Int32?)cp.LODGE_CLAIM_ID) + 1;
                    lodgeModel.TOUR_CLAIM_ID = Convert.ToInt32(formCollection["TOUR_CLAIM_ID"]);
                    lodgeModel.DATE_OF_CLAIM = DateTime.Now;
                    lodgeModel.DATE_FROM = Convert.ToDateTime(formCollection["DATE_FROM"]);
                    lodgeModel.DATE_TO = Convert.ToDateTime(formCollection["DATE_TO"]);
                    lodgeModel.TYPE_OF_CLAIM = formCollection["TYPE_OF_CLAIM"];

                    TimeSpan tSpan = Convert.ToDateTime(formCollection["DATE_TO"]).Subtract(Convert.ToDateTime(formCollection["DATE_FROM"]));
                    int noOfDays = tSpan.Days + 1;

                    List<NQM_LODGE_AND_DAILY_CLAIM_DETAILS> model = new List<NQM_LODGE_AND_DAILY_CLAIM_DETAILS>();
                    int tourId = Convert.ToInt32(formCollection["TOUR_CLAIM_ID"]);
                    model = dbContext.NQM_LODGE_AND_DAILY_CLAIM_DETAILS.Where(x => x.TOUR_CLAIM_ID == tourId).ToList();

                    foreach (var item in model)
                    {
                        if (item.DATE_TO == Convert.ToDateTime(formCollection["DATE_FROM"]))
                            noOfDays = noOfDays - 1;
                    }

                    if (lodgeModel.TYPE_OF_CLAIM.Equals("H"))
                    {
                        lodgeModel.HOTEL_NAME = formCollection["HOTEL_NAME"];
                        lodgeModel.AMOUNT_CLAIMED_HOTEL = Convert.ToInt32(formCollection["AMOUNT_CLAIMED_HOTEL"]);
                        lodgeModel.AMOUNT_PASSED_HOTEL_CQC = Convert.ToInt32(formCollection["AMOUNT_CLAIMED_HOTEL"]);
                        lodgeModel.AMOUNT_CLAIMED_DAILY = noOfDays * dbContext.NQM_TOUR_ALLOWANCE_MASTER.Where(x => x.ALLOWANCE_TYPE.Equals("Daily for Hotel") && x.IS_ACTIVE == "Y").Select(y => y.AMOUNT).FirstOrDefault();
                        lodgeModel.AMOUNT_PASSED_DAILY = lodgeModel.AMOUNT_CLAIMED_DAILY;

                        String FilePath = ConfigurationManager.AppSettings["NQM_TOUR_CLAIM_PATH"];
                        var fullPath = Path.Combine(FilePath, "LodgeClaim", currYear);

                        if (!Directory.Exists(fullPath))
                        {
                            Directory.CreateDirectory(fullPath);
                        }

                        DateTime d = DateTime.Now;
                        String format = "d_MMM_yyyy_HH_mm_ss";
                        String currentTime = d.ToString(format);
                        String FileNameBill = string.Empty;
                        String FileNameReceipt = string.Empty;

                        if (bill.FileName.Substring(bill.FileName.LastIndexOf('.') + 1).Trim().ToLower() == "jpg")
                            FileNameBill = currYear + "_LodgeClaim_Bill_" + formCollection["TOUR_CLAIM_ID"] + "_" + lodgeModel.LODGE_CLAIM_ID + "_" + currentTime + ".jpg";
                        else if (bill.FileName.Substring(bill.FileName.LastIndexOf('.') + 1).Trim().ToLower() == "jpeg")
                            FileNameBill = currYear + "_LodgeClaim_Bill_" + formCollection["TOUR_CLAIM_ID"] + "_" + lodgeModel.LODGE_CLAIM_ID + "_" + currentTime + ".jpeg";
                        else if (bill.FileName.Substring(bill.FileName.LastIndexOf('.') + 1).Trim().ToLower() == "png")
                            FileNameBill = currYear + "_LodgeClaim_Bill_" + formCollection["TOUR_CLAIM_ID"] + "_" + lodgeModel.LODGE_CLAIM_ID + "_" + currentTime + ".png";
                        else if (bill.FileName.Substring(bill.FileName.LastIndexOf('.') + 1).Trim().ToLower() == "pdf")
                            FileNameBill = currYear + "_LodgeClaim_Bill_" + formCollection["TOUR_CLAIM_ID"] + "_" + lodgeModel.LODGE_CLAIM_ID + "_" + currentTime + ".pdf";

                        bill.SaveAs(System.IO.Path.Combine(fullPath, FileNameBill));
                        lodgeModel.UPLOADED_BILL_PATH = fullPath;
                        lodgeModel.UPLOADED_BILL_NAME = FileNameBill;

                        if (receipt.FileName.Substring(receipt.FileName.LastIndexOf('.') + 1).Trim().ToLower() == "jpg")
                            FileNameReceipt = currYear + "_LodgeClaim_Receipt_" + formCollection["TOUR_CLAIM_ID"] + "_" + lodgeModel.LODGE_CLAIM_ID + "_" + currentTime + ".jpg";
                        else if (receipt.FileName.Substring(receipt.FileName.LastIndexOf('.') + 1).Trim().ToLower() == "jpeg")
                            FileNameReceipt = currYear + "_LodgeClaim_Receipt_" + formCollection["TOUR_CLAIM_ID"] + "_" + lodgeModel.LODGE_CLAIM_ID + "_" + currentTime + ".jpeg";
                        else if (receipt.FileName.Substring(receipt.FileName.LastIndexOf('.') + 1).Trim().ToLower() == "png")
                            FileNameReceipt = currYear + "_LodgeClaim_Receipt_" + formCollection["TOUR_CLAIM_ID"] + "_" + lodgeModel.LODGE_CLAIM_ID + "_" + currentTime + ".png";
                        else if (receipt.FileName.Substring(receipt.FileName.LastIndexOf('.') + 1).Trim().ToLower() == "pdf")
                            FileNameReceipt = currYear + "_LodgeClaim_Receipt_" + formCollection["TOUR_CLAIM_ID"] + "_" + lodgeModel.LODGE_CLAIM_ID + "_" + currentTime + ".pdf";

                        receipt.SaveAs(System.IO.Path.Combine(fullPath, FileNameReceipt));
                        lodgeModel.UPLOADED_RECEIPT_PATH = fullPath;
                        lodgeModel.UPLOADED_RECEIPT_NAME = FileNameReceipt;
                    }
                    else if (lodgeModel.TYPE_OF_CLAIM.Equals("G"))
                    {
                        lodgeModel.HOTEL_NAME = formCollection["GUEST_HOUSE_NAME"];
                        lodgeModel.AMOUNT_CLAIMED_HOTEL = Convert.ToInt32(formCollection["AMOUNT_CLAIMED_GUEST"]);
                        lodgeModel.AMOUNT_PASSED_HOTEL_CQC = Convert.ToInt32(formCollection["AMOUNT_CLAIMED_GUEST"]);
                        lodgeModel.AMOUNT_CLAIMED_DAILY = noOfDays * dbContext.NQM_TOUR_ALLOWANCE_MASTER.Where(x => x.ALLOWANCE_TYPE.Equals("Daily for Hotel") && x.IS_ACTIVE == "Y").Select(y => y.AMOUNT).FirstOrDefault();
                        lodgeModel.AMOUNT_PASSED_DAILY = lodgeModel.AMOUNT_CLAIMED_DAILY;

                        String FilePath = ConfigurationManager.AppSettings["NQM_TOUR_CLAIM_PATH"];
                        var fullPath = Path.Combine(FilePath, "LodgeClaim", currYear);

                        if (!Directory.Exists(fullPath))
                        {
                            Directory.CreateDirectory(fullPath);
                        }

                        DateTime d = DateTime.Now;
                        String format = "d_MMM_yyyy_HH_mm_ss";
                        String currentTime = d.ToString(format);
                        String FileNameBill = string.Empty;

                        if (gBill.FileName.Substring(gBill.FileName.LastIndexOf('.') + 1).Trim().ToLower() == "jpg")
                            FileNameBill = currYear + "_LodgeClaim_Guest_Bill_" + formCollection["TOUR_CLAIM_ID"] + "_" + lodgeModel.LODGE_CLAIM_ID + "_" + currentTime + ".jpg";
                        else if (gBill.FileName.Substring(gBill.FileName.LastIndexOf('.') + 1).Trim().ToLower() == "jpeg")
                            FileNameBill = currYear + "_LodgeClaim_Guest_Bill_" + formCollection["TOUR_CLAIM_ID"] + "_" + lodgeModel.LODGE_CLAIM_ID + "_" + currentTime + ".jpeg";
                        else if (gBill.FileName.Substring(gBill.FileName.LastIndexOf('.') + 1).Trim().ToLower() == "png")
                            FileNameBill = currYear + "_LodgeClaim_Guest_Bill_" + formCollection["TOUR_CLAIM_ID"] + "_" + lodgeModel.LODGE_CLAIM_ID + "_" + currentTime + ".png";
                        else if (gBill.FileName.Substring(gBill.FileName.LastIndexOf('.') + 1).Trim().ToLower() == "pdf")
                            FileNameBill = currYear + "_LodgeClaim_Guest_Bill_" + formCollection["TOUR_CLAIM_ID"] + "_" + lodgeModel.LODGE_CLAIM_ID + "_" + currentTime + ".pdf";

                        gBill.SaveAs(System.IO.Path.Combine(fullPath, FileNameBill));
                        lodgeModel.UPLOADED_BILL_PATH = fullPath;
                        lodgeModel.UPLOADED_BILL_NAME = FileNameBill;
                    }
                    else
                    {
                        lodgeModel.AMOUNT_CLAIMED_DAILY = noOfDays * dbContext.NQM_TOUR_ALLOWANCE_MASTER.Where(x => x.ALLOWANCE_TYPE.Equals("Daily for Self") && x.IS_ACTIVE == "Y").Select(y => y.AMOUNT).FirstOrDefault();
                        lodgeModel.AMOUNT_PASSED_DAILY = lodgeModel.AMOUNT_CLAIMED_DAILY;
                        lodgeModel.AMOUNT_PASSED_HOTEL_CQC = Convert.ToDecimal(0.00);
                    }

                    lodgeModel.USERID = PMGSYSession.Current.UserId;
                    lodgeModel.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];

                    dbContext.NQM_LODGE_AND_DAILY_CLAIM_DETAILS.Add(lodgeModel);
                    status = dbContext.SaveChanges();
                    status = 1;

                    scope.Complete();
                }
                if (status > 0)
                {
                    IsValid = "Lodge claim details saved successfully.";
                    return true;
                }

                IsValid = "Lodge claim details saved successfully.";
                return true;
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "TourClaimDAL.InsertLodgeClaimDetailsDAL()");
                IsValid = "Lodge claim details not saved.";
                return false;
            }
        }

        public Array GetLodgeClaimListDAL(int scheduleCode, int page, int rows, string sidx, string sord, out long totalRecords)
        {
            try
            {
                var lstExecution = (from lodge in dbContext.NQM_LODGE_AND_DAILY_CLAIM_DETAILS
                                    join master in dbContext.NQM_TOUR_CLAIM_MASTER on lodge.TOUR_CLAIM_ID equals master.TOUR_CLAIM_ID
                                    where
                                    master.ADMIN_SCHEDULE_CODE == scheduleCode
                                    select new
                                    {
                                        lodge.LODGE_CLAIM_ID,
                                        lodge.TOUR_CLAIM_ID,
                                        master.ADMIN_SCHEDULE_CODE,
                                        lodge.DATE_FROM,
                                        lodge.DATE_TO,
                                        lodge.TYPE_OF_CLAIM,
                                        lodge.HOTEL_NAME,
                                        lodge.AMOUNT_CLAIMED_DAILY,
                                        lodge.AMOUNT_CLAIMED_HOTEL,
                                        lodge.AMOUNT_PASSED_HOTEL_CQC,
                                        lodge.AMOUNT_PASSED_DAILY,
                                        lodge.DATE_OF_MODIFICATION_CQC,
                                        master.FINALIZE_FLAG,
                                        lodge.UPLOADED_RECEIPT_NAME,
                                        lodge.UPLOADED_BILL_NAME,
                                        master.CQC_APPROVE_DATE
                                    }).Distinct();

                totalRecords = lstExecution.Count();

                if (sidx.Trim() != string.Empty)
                {
                    if (sord.ToString() == "asc")
                    {
                        switch (sidx)
                        {
                            case "DATE_FROM":
                                lstExecution = lstExecution.OrderBy(m => m.DATE_FROM).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                                break;
                            default:
                                lstExecution = lstExecution.OrderBy(m => m.DATE_FROM).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                                break;
                        }
                    }
                    else
                    {
                        switch (sidx)
                        {
                            case "DATE_FROM":
                                lstExecution = lstExecution.OrderBy(m => m.DATE_FROM).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                                break;
                            default:
                                lstExecution = lstExecution.OrderBy(m => m.DATE_FROM).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                                break;
                        }
                    }
                }
                else
                {
                    lstExecution = lstExecution.OrderBy(m => m.DATE_FROM).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                }

                var result = lstExecution.Select(executionDetails => new
                {
                    executionDetails.LODGE_CLAIM_ID,
                    executionDetails.TOUR_CLAIM_ID,
                    executionDetails.ADMIN_SCHEDULE_CODE,
                    executionDetails.DATE_FROM,
                    executionDetails.DATE_TO,
                    executionDetails.HOTEL_NAME,
                    executionDetails.TYPE_OF_CLAIM,
                    executionDetails.AMOUNT_CLAIMED_DAILY,
                    executionDetails.AMOUNT_CLAIMED_HOTEL,
                    executionDetails.AMOUNT_PASSED_DAILY,
                    executionDetails.AMOUNT_PASSED_HOTEL_CQC,
                    executionDetails.DATE_OF_MODIFICATION_CQC,
                    executionDetails.FINALIZE_FLAG,
                    executionDetails.UPLOADED_RECEIPT_NAME,
                    executionDetails.UPLOADED_BILL_NAME,
                    executionDetails.CQC_APPROVE_DATE
                }).ToArray();

                return result.Select(m => new
                {
                    id = m.TOUR_CLAIM_ID.ToString(),
                    cell = new[]
                    {
                        m.LODGE_CLAIM_ID.ToString(),
                        m.TOUR_CLAIM_ID.ToString(),
                        m.DATE_FROM.ToShortDateString(),
                        m.DATE_TO.ToShortDateString(),
                        (m.TYPE_OF_CLAIM == "H") ? "Hotel Claim" : (m.TYPE_OF_CLAIM == "S") ? "Self Accomodation" : (m.TYPE_OF_CLAIM == "G") ? "Guest House" : "--",
                        (m.TYPE_OF_CLAIM == "H") || (m.TYPE_OF_CLAIM == "G") ? m.HOTEL_NAME : "--",
                        m.AMOUNT_CLAIMED_DAILY.ToString(),
                        (m.TYPE_OF_CLAIM == "H") || (m.TYPE_OF_CLAIM == "G") ? m.AMOUNT_CLAIMED_HOTEL.ToString() : "--",
                        m.FINALIZE_FLAG == 5 ? (m.AMOUNT_PASSED_DAILY + m.AMOUNT_PASSED_HOTEL_CQC).ToString() : "--",
                        m.FINALIZE_FLAG == 5 ? ((m.CQC_APPROVE_DATE.ToString() == null || m.CQC_APPROVE_DATE.ToString() == string.Empty) ? "--" : string.Format("{0:d}", m.CQC_APPROVE_DATE)) : "--",
                        (m.TYPE_OF_CLAIM == "H") || (m.TYPE_OF_CLAIM == "G") ? "<a href='/TourClaim/ViewUploadedLodgeBill?id1="+URLEncrypt.EncryptParameters1(new String[]{"FileName ="+m.UPLOADED_BILL_NAME})+"' title='Click here to view uploaded file' class='ui-icon ui-icon-search  ui-align-center' target=_blank></a>" : "--",
                        (m.TYPE_OF_CLAIM == "H") ? "<a href='/TourClaim/ViewUploadedLodgeBill?id1="+URLEncrypt.EncryptParameters1(new String[]{"FileName ="+m.UPLOADED_RECEIPT_NAME})+"' title='Click here to view uploaded file' class='ui-icon ui-icon-search  ui-align-center' target=_blank></a>" : "--",
                        (m.FINALIZE_FLAG >= 1)
                        ? "<a href='#' class='ui-icon ui-icon-locked ui-align-center'>"
                        : "<center><table><tr><td  style='border-color:white'><span class='ui-icon ui-icon-pencil' title='Click here to Edit Details' onClick ='EditLodgeDetails(\"" + m.LODGE_CLAIM_ID + "\");' ></span></td></tr></table></center>",
                        (m.FINALIZE_FLAG >= 1)
                        ? "<a href='#' class='ui-icon ui-icon-locked ui-align-center'>"
                        : "<center><table><tr><td  style='border-color:white'><span class='ui-icon ui-icon-trash' title='Click here to Delete Details' onClick ='DeleteLodgeDetails(\"" + m.LODGE_CLAIM_ID + "$" + m.ADMIN_SCHEDULE_CODE + "\");' ></span></td></tr></table></center>",
                    }
                }).ToArray();
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "TourClaimDAL.GetLodgeClaimListDAL()");
                totalRecords = 0;
                return null;
            }
            finally
            {
                dbContext.Dispose();
            }
        }

        public bool UpdateLodgeClaimDetailsDAL(FormCollection formCollection, HttpPostedFileBase bill, HttpPostedFileBase receipt, HttpPostedFileBase gBill, out String IsValid)
        {
            int status = 0;
            int lodgeId = Convert.ToInt32(formCollection["LODGE_CLAIM_ID"]);
            string typeOfClaim, oldBillName, oldReceiptName, billPath, receiptPath, fullBillPath, fullReceiptPath, newBillName = string.Empty, newReceiptName = string.Empty;
            NQM_LODGE_AND_DAILY_CLAIM_DETAILS lodgeModel = new NQM_LODGE_AND_DAILY_CLAIM_DETAILS();
            NQM_TOUR_ALLOWANCE_MASTER allowanceModel = new NQM_TOUR_ALLOWANCE_MASTER();
            List<NQM_LODGE_AND_DAILY_CLAIM_DETAILS> model = new List<NQM_LODGE_AND_DAILY_CLAIM_DETAILS>();

            status = dbContext.SaveChanges();
            try
            {
                using (var scope = new TransactionScope())
                {
                    lodgeModel = dbContext.NQM_LODGE_AND_DAILY_CLAIM_DETAILS.Where(x => x.LODGE_CLAIM_ID == lodgeId).FirstOrDefault();
                    typeOfClaim = lodgeModel.TYPE_OF_CLAIM;

                    lodgeModel.DATE_OF_CLAIM = DateTime.Now;
                    lodgeModel.DATE_FROM = Convert.ToDateTime(formCollection["DATE_FROM"]);
                    lodgeModel.DATE_TO = Convert.ToDateTime(formCollection["DATE_TO"]);

                    TimeSpan tSpan = Convert.ToDateTime(formCollection["DATE_TO"]).Subtract(Convert.ToDateTime(formCollection["DATE_FROM"]));
                    int noOfDays = tSpan.Days + 1;

                    int tourId = Convert.ToInt32(formCollection["TOUR_CLAIM_ID"]);
                    model = dbContext.NQM_LODGE_AND_DAILY_CLAIM_DETAILS.Where(x => x.TOUR_CLAIM_ID == tourId && x.LODGE_CLAIM_ID != lodgeId).ToList();

                    foreach (var item in model)
                    {
                        if (item.DATE_TO == Convert.ToDateTime(formCollection["DATE_FROM"]))
                            noOfDays = noOfDays - 1;
                    }

                    oldBillName = lodgeModel.UPLOADED_BILL_NAME;
                    oldReceiptName = lodgeModel.UPLOADED_RECEIPT_NAME;
                    billPath = lodgeModel.UPLOADED_BILL_PATH;
                    receiptPath = lodgeModel.UPLOADED_RECEIPT_PATH;

                    if (typeOfClaim.Equals("H"))
                    {
                        lodgeModel.HOTEL_NAME = formCollection["HOTEL_NAME"];
                        lodgeModel.AMOUNT_CLAIMED_HOTEL = Convert.ToDecimal(formCollection["AMOUNT_CLAIMED_HOTEL"]);
                        lodgeModel.AMOUNT_PASSED_HOTEL_CQC = Convert.ToDecimal(formCollection["AMOUNT_CLAIMED_HOTEL"]);
                        lodgeModel.AMOUNT_CLAIMED_DAILY = noOfDays * dbContext.NQM_TOUR_ALLOWANCE_MASTER.Where(x => x.ALLOWANCE_TYPE.Equals("Daily for Hotel") && x.IS_ACTIVE == "Y").Select(y => y.AMOUNT).FirstOrDefault();
                        lodgeModel.AMOUNT_PASSED_DAILY = lodgeModel.AMOUNT_CLAIMED_DAILY;

                        if (bill.ContentLength != 0)
                        {
                            // First delete the existing file
                            fullBillPath = Path.Combine(billPath, oldBillName);
                            FileInfo file = new FileInfo(fullBillPath);

                            if (file.Exists)
                            {
                                file.Delete();
                            }

                            // Create new file name with old file and replace the extention.
                            string fileName = oldBillName.Split('.')[0];

                            // Create new name with corrrect extention.
                            if (bill.FileName.Substring(bill.FileName.LastIndexOf('.') + 1).Trim().ToLower() == "jpg")
                                newBillName = fileName + ".jpg";
                            else if (bill.FileName.Substring(bill.FileName.LastIndexOf('.') + 1).Trim().ToLower() == "jpeg")
                                newBillName = fileName + ".jpeg";
                            else if (bill.FileName.Substring(bill.FileName.LastIndexOf('.') + 1).Trim().ToLower() == "png")
                                newBillName = fileName + ".png";
                            else if (bill.FileName.Substring(bill.FileName.LastIndexOf('.') + 1).Trim().ToLower() == "pdf")
                                newBillName = fileName + ".pdf";

                            // Save new file with same and same path.
                            bill.SaveAs(System.IO.Path.Combine(billPath, newBillName));
                            lodgeModel.UPLOADED_BILL_NAME = newBillName;
                        }

                        if (receipt.ContentLength != 0)
                        {
                            // First delete the existing file
                            fullReceiptPath = Path.Combine(receiptPath, oldReceiptName);
                            FileInfo file = new FileInfo(fullReceiptPath);

                            if (file.Exists)
                            {
                                file.Delete();
                            }

                            // Create new file name with old file and replace the extention.
                            string fileName = oldReceiptName.Split('.')[0];

                            // Create new name with corrrect extention.
                            if (receipt.FileName.Substring(receipt.FileName.LastIndexOf('.') + 1).Trim().ToLower() == "jpg")
                                newReceiptName = fileName + ".jpg";
                            else if (receipt.FileName.Substring(receipt.FileName.LastIndexOf('.') + 1).Trim().ToLower() == "jpeg")
                                newReceiptName = fileName + ".jpeg";
                            else if (receipt.FileName.Substring(receipt.FileName.LastIndexOf('.') + 1).Trim().ToLower() == "png")
                                newReceiptName = fileName + ".png";
                            else if (receipt.FileName.Substring(receipt.FileName.LastIndexOf('.') + 1).Trim().ToLower() == "pdf")
                                newReceiptName = fileName + ".pdf";

                            // Save new file with same and same path.
                            receipt.SaveAs(System.IO.Path.Combine(receiptPath, newReceiptName));
                            lodgeModel.UPLOADED_RECEIPT_NAME = newReceiptName;
                        }
                    }
                    else if (typeOfClaim.Equals("G"))
                    {
                        lodgeModel.HOTEL_NAME = formCollection["GUEST_HOUSE_NAME"];
                        lodgeModel.AMOUNT_CLAIMED_HOTEL = Convert.ToDecimal(formCollection["AMOUNT_CLAIMED_GUEST"]);
                        lodgeModel.AMOUNT_PASSED_HOTEL_CQC = Convert.ToDecimal(formCollection["AMOUNT_CLAIMED_GUEST"]);
                        lodgeModel.AMOUNT_CLAIMED_DAILY = noOfDays * dbContext.NQM_TOUR_ALLOWANCE_MASTER.Where(x => x.ALLOWANCE_TYPE.Equals("Daily for Hotel") && x.IS_ACTIVE == "Y").Select(y => y.AMOUNT).FirstOrDefault();
                        lodgeModel.AMOUNT_PASSED_DAILY = lodgeModel.AMOUNT_CLAIMED_DAILY;

                        if (gBill.ContentLength != 0)
                        {
                            // First delete the existing file
                            fullBillPath = Path.Combine(billPath, oldBillName);
                            FileInfo file = new FileInfo(fullBillPath);

                            if (file.Exists)
                            {
                                file.Delete();
                            }

                            // Create new file name with old file and replace the extention.
                            string fileName = oldBillName.Split('.')[0];

                            // Create new name with corrrect extention.
                            if (gBill.FileName.Substring(gBill.FileName.LastIndexOf('.') + 1).Trim().ToLower() == "jpg")
                                newBillName = fileName + ".jpg";
                            else if (gBill.FileName.Substring(gBill.FileName.LastIndexOf('.') + 1).Trim().ToLower() == "jpeg")
                                newBillName = fileName + ".jpeg";
                            else if (gBill.FileName.Substring(gBill.FileName.LastIndexOf('.') + 1).Trim().ToLower() == "png")
                                newBillName = fileName + ".png";
                            else if (gBill.FileName.Substring(gBill.FileName.LastIndexOf('.') + 1).Trim().ToLower() == "pdf")
                                newBillName = fileName + ".pdf";

                            // Save new file with same and same path.
                            gBill.SaveAs(System.IO.Path.Combine(billPath, newBillName));
                            lodgeModel.UPLOADED_BILL_NAME = newBillName;
                        }
                    }
                    else
                    {
                        lodgeModel.AMOUNT_CLAIMED_DAILY = noOfDays * dbContext.NQM_TOUR_ALLOWANCE_MASTER.Where(x => x.ALLOWANCE_TYPE.Equals("Daily for Self") && x.IS_ACTIVE == "Y").Select(y => y.AMOUNT).FirstOrDefault();
                        lodgeModel.AMOUNT_PASSED_DAILY = lodgeModel.AMOUNT_CLAIMED_DAILY;
                        lodgeModel.AMOUNT_PASSED_HOTEL_CQC = Convert.ToDecimal(0.00);
                    }

                    dbContext.Entry(lodgeModel).State = System.Data.Entity.EntityState.Modified;
                    dbContext.SaveChanges();
                    status = 1;

                    scope.Complete();
                }
                if (status > 0)
                {
                    IsValid = "Lodge claim details updated successfully.";
                    return true;
                }

                IsValid = "Lodge claim details updated successfully.";
                return true;
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "TourClaimDAL.UpdateLodgeClaimDetailsDAL()");
                IsValid = "Lodge claim details not updated.";
                return false;
            }
        }

        public bool InsertInspectionHonorariumDAL(FormCollection formCollection, out String IsValid)
        {
            int status = 0;
            NQM_TOUR_HONORARIUM_INSPECTION_OF_ROADS roadInspec = new NQM_TOUR_HONORARIUM_INSPECTION_OF_ROADS();

            status = dbContext.SaveChanges();
            try
            {
                using (var scope = new TransactionScope())
                {
                    roadInspec.HONORARIUM_INSPECTION_ID = dbContext.NQM_TOUR_HONORARIUM_INSPECTION_OF_ROADS.Max(cp => (Int32?)cp.HONORARIUM_INSPECTION_ID) == null ? 1 : (Int32)dbContext.NQM_TOUR_HONORARIUM_INSPECTION_OF_ROADS.Max(cp => (Int32?)cp.HONORARIUM_INSPECTION_ID) + 1;
                    roadInspec.TOUR_CLAIM_ID = Convert.ToInt32(formCollection["TOUR_CLAIM_ID"]);
                    roadInspec.DATE_OF_CLAIM = DateTime.Now;
                    roadInspec.DATE_OF_INSPECTION = Convert.ToDateTime(formCollection["DATE_OF_INSPECTION"]);
                    roadInspec.TYPE_OF_WORK = formCollection["TYPE_OF_WORK"];
                    roadInspec.TYPE_OF_WORK_OTHER = formCollection["TYPE_OF_WORK"] == "O" ?
                                                                                            formCollection["TYPE_OF_WORK_OTHER"] == String.Empty ? "Others" : formCollection["TYPE_OF_WORK_OTHER"]
                                                                                          : null;
                    roadInspec.AMOUNT_CLAIMED = Convert.ToDecimal(formCollection["AMOUNT_CLAIMED"]);
                    roadInspec.AMOUNT_PASSED_CQC = Convert.ToDecimal(formCollection["AMOUNT_CLAIMED"]);
                    roadInspec.USERID = PMGSYSession.Current.UserId;
                    roadInspec.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];

                    List<NQM_TOUR_HONORARIUM_INSPECTION_OF_ROADS> model = new List<NQM_TOUR_HONORARIUM_INSPECTION_OF_ROADS>();
                    int tourId = Convert.ToInt32(formCollection["TOUR_CLAIM_ID"]);
                    model = dbContext.NQM_TOUR_HONORARIUM_INSPECTION_OF_ROADS.Where(x => x.TOUR_CLAIM_ID == tourId).ToList();

                    foreach (var item in model)
                    {
                        if (item.DATE_OF_INSPECTION == Convert.ToDateTime(formCollection["DATE_OF_INSPECTION"]))
                        {
                            roadInspec.AMOUNT_CLAIMED = 0;
                        }
                    }

                    List<NQM_TOUR_HONORARIUM_MEETING_WITH_PIU> meetingModel = new List<NQM_TOUR_HONORARIUM_MEETING_WITH_PIU>();
                    meetingModel = dbContext.NQM_TOUR_HONORARIUM_MEETING_WITH_PIU.Where(x => x.TOUR_CLAIM_ID == tourId).ToList();

                    foreach (var item in meetingModel)
                    {
                        if (item.DATE_OF_MEETING == Convert.ToDateTime(formCollection["DATE_OF_INSPECTION"]))
                        {
                            item.AMOUNT_CLAIMED = 0;
                            dbContext.Entry(item).State = System.Data.Entity.EntityState.Modified;
                            dbContext.SaveChanges();
                        }
                    }

                    dbContext.NQM_TOUR_HONORARIUM_INSPECTION_OF_ROADS.Add(roadInspec);
                    status = dbContext.SaveChanges();
                    status = 1;

                    scope.Complete();
                }
                if (status > 0)
                {
                    IsValid = "Honorarium for inspection of roads details saved successfully.";
                    return true;
                }

                IsValid = "Honorarium for inspection of roads details saved successfully.";
                return true;
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "TourClaimDAL.InsertInspectionHonorariumDAL()");
                IsValid = "Honorarium for inspection of roads details not saved.";
                return false;
            }
        }

        public Array GetInspectionHonorariumListDAL(int scheduleCode, int page, int rows, string sidx, string sord, out long totalRecords)
        {
            try
            {
                var lstExecution = (from roadInsp in dbContext.NQM_TOUR_HONORARIUM_INSPECTION_OF_ROADS
                                    join master in dbContext.NQM_TOUR_CLAIM_MASTER on roadInsp.TOUR_CLAIM_ID equals master.TOUR_CLAIM_ID
                                    where
                                    master.ADMIN_SCHEDULE_CODE == scheduleCode
                                    select new
                                    {
                                        roadInsp.HONORARIUM_INSPECTION_ID,
                                        roadInsp.TOUR_CLAIM_ID,
                                        master.ADMIN_SCHEDULE_CODE,
                                        roadInsp.DATE_OF_INSPECTION,
                                        roadInsp.TYPE_OF_WORK,
                                        roadInsp.TYPE_OF_WORK_OTHER,
                                        roadInsp.AMOUNT_CLAIMED,
                                        roadInsp.AMOUNT_PASSED_CQC,
                                        roadInsp.DATE_OF_MODIFICATION_CQC,
                                        master.FINALIZE_FLAG,
                                        roadInsp.QRS_FLAG,
                                        master.CQC_APPROVE_DATE
                                    }).Distinct();

                totalRecords = lstExecution.Count();

                if (sidx.Trim() != string.Empty)
                {
                    if (sord.ToString() == "asc")
                    {
                        switch (sidx)
                        {
                            case "DATE_OF_INSPECTION":
                                lstExecution = lstExecution.OrderBy(m => m.DATE_OF_INSPECTION).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                                break;
                            default:
                                lstExecution = lstExecution.OrderBy(m => m.DATE_OF_INSPECTION).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                                break;
                        }
                    }
                    else
                    {
                        switch (sidx)
                        {
                            case "DATE_OF_INSPECTION":
                                lstExecution = lstExecution.OrderBy(m => m.DATE_OF_INSPECTION).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                                break;
                            default:
                                lstExecution = lstExecution.OrderBy(m => m.DATE_OF_INSPECTION).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                                break;
                        }
                    }
                }
                else
                {
                    lstExecution = lstExecution.OrderBy(m => m.DATE_OF_INSPECTION).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                }

                var result = lstExecution.Select(executionDetails => new
                {
                    executionDetails.HONORARIUM_INSPECTION_ID,
                    executionDetails.TOUR_CLAIM_ID,
                    executionDetails.ADMIN_SCHEDULE_CODE,
                    executionDetails.DATE_OF_INSPECTION,
                    executionDetails.TYPE_OF_WORK,
                    executionDetails.TYPE_OF_WORK_OTHER,
                    executionDetails.AMOUNT_CLAIMED,
                    executionDetails.AMOUNT_PASSED_CQC,
                    executionDetails.DATE_OF_MODIFICATION_CQC,
                    executionDetails.FINALIZE_FLAG,
                    executionDetails.QRS_FLAG,
                    executionDetails.CQC_APPROVE_DATE
                }).ToArray();

                return result.Select(m => new
                {

                    id = m.TOUR_CLAIM_ID.ToString(),
                    cell = new[]
                    {
                        m.HONORARIUM_INSPECTION_ID.ToString(),
                        m.TOUR_CLAIM_ID.ToString(),
                        m.DATE_OF_INSPECTION.ToShortDateString(),
                        m.TYPE_OF_WORK == "I" ? "Inspection" : m.TYPE_OF_WORK == "E" ? "Enquiry" : m.TYPE_OF_WORK == "A" ? "ATR Verification" : m.TYPE_OF_WORK == "O" ? m.TYPE_OF_WORK_OTHER : m.TYPE_OF_WORK == "N" ? "Normal" : "--",
                        m.AMOUNT_CLAIMED.ToString(),
                        m.FINALIZE_FLAG == 5 ? ((m.AMOUNT_PASSED_CQC.ToString() == null || m.AMOUNT_PASSED_CQC.ToString() == string.Empty) ? "--" : m.AMOUNT_PASSED_CQC.ToString()) : "--",
                        m.FINALIZE_FLAG == 5 ? ((m.CQC_APPROVE_DATE.ToString() == null || m.CQC_APPROVE_DATE.ToString() == string.Empty) ? "--" : string.Format("{0:d}", m.CQC_APPROVE_DATE)) : "--",
                        (m.FINALIZE_FLAG >= 1 || m.QRS_FLAG == 1)
                        ? "<a href='#' class='ui-icon ui-icon-locked ui-align-center'>"
                        : "<center><table><tr><td  style='border-color:white'><span class='ui-icon ui-icon-pencil' title='Click here to Edit Details' onClick ='EditInspectionDetails(\"" + m.HONORARIUM_INSPECTION_ID + "\");' ></span></td></tr></table></center>",
                        (m.FINALIZE_FLAG >= 1 || m.QRS_FLAG == 1)
                        ? "<a href='#' class='ui-icon ui-icon-locked ui-align-center'>"
                        : "<center><table><tr><td  style='border-color:white'><span class='ui-icon ui-icon-trash' title='Click here to Delete Details' onClick ='DeleteInspectionDetails(\"" + m.HONORARIUM_INSPECTION_ID + "$" + m.ADMIN_SCHEDULE_CODE + "\");' ></span></td></tr></table></center>",
                    }
                }).ToArray();
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "TourClaimDAL.GetInspectionHonorariumListDAL()");
                totalRecords = 0;
                return null;
            }
            finally
            {
                dbContext.Dispose();
            }
        }

        public bool UpdateInspectionHonorariumDAL(FormCollection formCollection, out String IsValid)
        {
            int status = 0;
            NQM_TOUR_HONORARIUM_INSPECTION_OF_ROADS roadInspec = new NQM_TOUR_HONORARIUM_INSPECTION_OF_ROADS();
            NQM_TOUR_HONORARIUM_INSPECTION_OF_ROADS inspModel = new NQM_TOUR_HONORARIUM_INSPECTION_OF_ROADS();
            NQM_TOUR_HONORARIUM_MEETING_WITH_PIU meetingModel = new NQM_TOUR_HONORARIUM_MEETING_WITH_PIU();
            List<NQM_TOUR_HONORARIUM_MEETING_WITH_PIU> meetingList = new List<NQM_TOUR_HONORARIUM_MEETING_WITH_PIU>();
            int inspId = Convert.ToInt32(formCollection["HONORARIUM_INSPECTION_ID"]);
            decimal amt = dbContext.NQM_TOUR_ALLOWANCE_MASTER.Where(x => x.ALLOWANCE_TYPE.Equals("Meeting") && x.IS_ACTIVE == "Y").Select(y => y.AMOUNT).FirstOrDefault();
            decimal inspAmt = dbContext.NQM_TOUR_ALLOWANCE_MASTER.Where(x => x.ALLOWANCE_TYPE.Equals("Inspection") && x.IS_ACTIVE == "Y").Select(y => y.AMOUNT).FirstOrDefault();
            DateTime prevInspDate, updatedInspDate;

            status = dbContext.SaveChanges();
            try
            {
                using (var scope = new TransactionScope())
                {
                    roadInspec = dbContext.NQM_TOUR_HONORARIUM_INSPECTION_OF_ROADS.Where(x => x.HONORARIUM_INSPECTION_ID == inspId).FirstOrDefault();
                    prevInspDate = roadInspec.DATE_OF_INSPECTION;
                    updatedInspDate = Convert.ToDateTime(formCollection["DATE_OF_INSPECTION"]);

                    if (prevInspDate == updatedInspDate)
                    {
                        roadInspec.DATE_OF_CLAIM = DateTime.Now;
                        roadInspec.TYPE_OF_WORK = formCollection["TYPE_OF_WORK"];
                        roadInspec.TYPE_OF_WORK_OTHER = formCollection["TYPE_OF_WORK"] == "O" ? formCollection["TYPE_OF_WORK_OTHER"] : null;

                        dbContext.Entry(roadInspec).State = System.Data.Entity.EntityState.Modified;
                        dbContext.SaveChanges();
                    }
                    else
                    {
                        if (dbContext.NQM_TOUR_HONORARIUM_INSPECTION_OF_ROADS.Where(x => x.TOUR_CLAIM_ID == roadInspec.TOUR_CLAIM_ID && x.DATE_OF_INSPECTION == prevInspDate && x.AMOUNT_CLAIMED == inspAmt && x.HONORARIUM_INSPECTION_ID != roadInspec.HONORARIUM_INSPECTION_ID).Any())
                        { }
                        else if (dbContext.NQM_TOUR_HONORARIUM_INSPECTION_OF_ROADS.Where(x => x.TOUR_CLAIM_ID == roadInspec.TOUR_CLAIM_ID && x.DATE_OF_INSPECTION == prevInspDate && x.HONORARIUM_INSPECTION_ID != roadInspec.HONORARIUM_INSPECTION_ID).Any())
                        {
                            inspModel = dbContext.NQM_TOUR_HONORARIUM_INSPECTION_OF_ROADS.Where(x => x.TOUR_CLAIM_ID == roadInspec.TOUR_CLAIM_ID && x.DATE_OF_INSPECTION == prevInspDate && x.HONORARIUM_INSPECTION_ID != roadInspec.HONORARIUM_INSPECTION_ID).FirstOrDefault();
                            inspModel.AMOUNT_CLAIMED = inspAmt;
                            inspModel.AMOUNT_PASSED_CQC = inspAmt;
                            dbContext.Entry(inspModel).State = System.Data.Entity.EntityState.Modified;
                            dbContext.SaveChanges();
                        }

                        if (dbContext.NQM_TOUR_HONORARIUM_INSPECTION_OF_ROADS.Where(x => x.TOUR_CLAIM_ID == roadInspec.TOUR_CLAIM_ID && x.DATE_OF_INSPECTION == updatedInspDate && x.AMOUNT_CLAIMED == inspAmt && x.HONORARIUM_INSPECTION_ID != roadInspec.HONORARIUM_INSPECTION_ID).Any())
                        {
                            roadInspec.AMOUNT_CLAIMED = 0;
                            roadInspec.AMOUNT_PASSED_CQC = 0;
                        }
                        else
                        {
                            roadInspec.AMOUNT_CLAIMED = inspAmt;
                            roadInspec.AMOUNT_PASSED_CQC = inspAmt;
                        }

                        roadInspec.DATE_OF_INSPECTION = Convert.ToDateTime(formCollection["DATE_OF_INSPECTION"]);
                    }

                    if (prevInspDate != updatedInspDate)
                    {
                        if ((dbContext.NQM_TOUR_HONORARIUM_INSPECTION_OF_ROADS.Where(x => x.TOUR_CLAIM_ID == roadInspec.TOUR_CLAIM_ID && x.DATE_OF_INSPECTION == prevInspDate && x.HONORARIUM_INSPECTION_ID != roadInspec.HONORARIUM_INSPECTION_ID).Any()) && (dbContext.NQM_TOUR_HONORARIUM_MEETING_WITH_PIU.Where(x => x.TOUR_CLAIM_ID == roadInspec.TOUR_CLAIM_ID && x.DATE_OF_MEETING == prevInspDate).Any()))
                        {
                            meetingList = dbContext.NQM_TOUR_HONORARIUM_MEETING_WITH_PIU.Where(x => x.TOUR_CLAIM_ID == roadInspec.TOUR_CLAIM_ID && x.DATE_OF_MEETING == prevInspDate).ToList();

                            foreach (var item in meetingList)
                            {
                                item.AMOUNT_CLAIMED = 0;
                                item.AMOUNT_PASSED_CQC = 0;
                                dbContext.Entry(item).State = System.Data.Entity.EntityState.Modified;
                                dbContext.SaveChanges();
                            }
                        }

                        if ((!dbContext.NQM_TOUR_HONORARIUM_INSPECTION_OF_ROADS.Where(x => x.TOUR_CLAIM_ID == roadInspec.TOUR_CLAIM_ID && x.DATE_OF_INSPECTION == prevInspDate && x.HONORARIUM_INSPECTION_ID != roadInspec.HONORARIUM_INSPECTION_ID).Any()) && (dbContext.NQM_TOUR_HONORARIUM_MEETING_WITH_PIU.Where(x => x.TOUR_CLAIM_ID == roadInspec.TOUR_CLAIM_ID && x.DATE_OF_MEETING == prevInspDate).Any()))
                        {
                            meetingList = dbContext.NQM_TOUR_HONORARIUM_MEETING_WITH_PIU.Where(x => x.TOUR_CLAIM_ID == roadInspec.TOUR_CLAIM_ID && x.DATE_OF_MEETING == prevInspDate).ToList();

                            foreach (var item in meetingList)
                            {
                                item.AMOUNT_CLAIMED = 0;
                                item.AMOUNT_PASSED_CQC = 0;
                                dbContext.Entry(item).State = System.Data.Entity.EntityState.Modified;
                                dbContext.SaveChanges();
                            }

                            meetingModel = dbContext.NQM_TOUR_HONORARIUM_MEETING_WITH_PIU.Where(x => x.TOUR_CLAIM_ID == roadInspec.TOUR_CLAIM_ID && x.DATE_OF_MEETING == prevInspDate).FirstOrDefault();

                            meetingModel.AMOUNT_CLAIMED = amt;
                            meetingModel.AMOUNT_PASSED_CQC = amt;
                            dbContext.Entry(meetingModel).State = System.Data.Entity.EntityState.Modified;
                            dbContext.SaveChanges();
                        }

                        if (dbContext.NQM_TOUR_HONORARIUM_MEETING_WITH_PIU.Where(x => x.TOUR_CLAIM_ID == roadInspec.TOUR_CLAIM_ID && x.DATE_OF_MEETING == updatedInspDate).Any())
                        {
                            meetingList = dbContext.NQM_TOUR_HONORARIUM_MEETING_WITH_PIU.Where(x => x.TOUR_CLAIM_ID == roadInspec.TOUR_CLAIM_ID && x.DATE_OF_MEETING == updatedInspDate).ToList();

                            foreach (var item in meetingList)
                            {
                                item.AMOUNT_CLAIMED = 0;
                                item.AMOUNT_PASSED_CQC = 0;
                                dbContext.Entry(item).State = System.Data.Entity.EntityState.Modified;
                                dbContext.SaveChanges();
                            }
                        }
                    }

                    dbContext.Entry(roadInspec).State = System.Data.Entity.EntityState.Modified;
                    dbContext.SaveChanges();
                    status = 1;

                    scope.Complete();
                }
                if (status > 0)
                {
                    IsValid = "Honorarium for inspection of roads details updated successfully.";
                    return true;
                }

                IsValid = "Honorarium for inspection of roads details updated successfully.";
                return true;
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "TourClaimDAL.UpdateInspectionHonorariumDAL()");
                IsValid = "Honorarium for inspection of roads details not updated.";
                return false;
            }
        }

        public bool InsertMeetingHonorariumDAL(FormCollection formCollection, HttpPostedFileBase postedBgFile, HttpPostedFileBase postedBgFile1, HttpPostedFileBase postedBgFile2, out String IsValid)
        {
            int status = 0;
            NQM_TOUR_HONORARIUM_MEETING_WITH_PIU meetingModel = new NQM_TOUR_HONORARIUM_MEETING_WITH_PIU();
            NQM_TOUR_HONORARIUM_INSPECTION_OF_ROADS inspModel = new NQM_TOUR_HONORARIUM_INSPECTION_OF_ROADS();
            string currYear = DateTime.Now.Year.ToString();

            int tourClaimId = Convert.ToInt32(formCollection["TOUR_CLAIM_ID"]);
            List<DateTime> lst = dbContext.NQM_TOUR_HONORARIUM_INSPECTION_OF_ROADS.Where(x => x.TOUR_CLAIM_ID == tourClaimId).Select(y => y.DATE_OF_INSPECTION).ToList();
            List<DateTime> lstMeeting = dbContext.NQM_TOUR_HONORARIUM_MEETING_WITH_PIU.Where(x => x.TOUR_CLAIM_ID == tourClaimId).Select(y => y.DATE_OF_MEETING).ToList();
            DateTime dateOfMeeting;
            var amt = Convert.ToDecimal(formCollection["AMOUNT_CLAIMED"]);

            status = dbContext.SaveChanges();
            try
            {
                using (var scope = new TransactionScope())
                {
                    meetingModel.HONORARIUM_MEETING_ID = dbContext.NQM_TOUR_HONORARIUM_MEETING_WITH_PIU.Max(cp => (Int32?)cp.HONORARIUM_MEETING_ID) == null ? 1 : (Int32)dbContext.NQM_TOUR_HONORARIUM_MEETING_WITH_PIU.Max(cp => (Int32?)cp.HONORARIUM_MEETING_ID) + 1;
                    meetingModel.TOUR_CLAIM_ID = Convert.ToInt32(formCollection["TOUR_CLAIM_ID"]);
                    meetingModel.DATE_OF_CLAIM = DateTime.Now;
                    meetingModel.DATE_OF_MEETING = Convert.ToDateTime(formCollection["DATE_OF_MEETING"]);

                    foreach (var item in lst)
                    {
                        dateOfMeeting = Convert.ToDateTime(formCollection["DATE_OF_MEETING"]);
                        if (dateOfMeeting == item)
                        {
                            amt = 0;
                            break;
                        }
                    }

                    foreach (var item in lstMeeting)
                    {
                        dateOfMeeting = Convert.ToDateTime(formCollection["DATE_OF_MEETING"]);
                        if (dateOfMeeting == item)
                        {
                            amt = 0;
                            break;
                        }
                    }

                    meetingModel.AMOUNT_CLAIMED = amt;
                    meetingModel.AMOUNT_PASSED_CQC = amt;
                    meetingModel.DISTRICT = Convert.ToInt32(formCollection["DISTRICT_CODE"]);
                    meetingModel.USERID = PMGSYSession.Current.UserId;
                    meetingModel.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];

                    String FilePath = ConfigurationManager.AppSettings["NQM_TOUR_CLAIM_PATH"];
                    var fullPath = Path.Combine(FilePath, "MeetingWithPIU", currYear);

                    if (!Directory.Exists(fullPath))
                    {
                        Directory.CreateDirectory(fullPath);
                    }

                    DateTime d = DateTime.Now;
                    String format = "d_MMM_yyyy_HH_mm_ss";
                    String currentTime = d.ToString(format);
                    String FileName = string.Empty;
                    String fileName = string.Empty;
                    String attendanceSheetName = string.Empty;

                    if (postedBgFile.FileName.Substring(postedBgFile.FileName.LastIndexOf('.') + 1).Trim().ToLower() == "jpg")
                        FileName = currYear + "_MP_" + formCollection["TOUR_CLAIM_ID"] + "_" + meetingModel.HONORARIUM_MEETING_ID + "_" + currentTime + ".jpg";
                    else if (postedBgFile.FileName.Substring(postedBgFile.FileName.LastIndexOf('.') + 1).Trim().ToLower() == "jpeg")
                        FileName = currYear + "_MP_" + formCollection["TOUR_CLAIM_ID"] + "_" + meetingModel.HONORARIUM_MEETING_ID + "_" + currentTime + ".jpeg";
                    else if (postedBgFile.FileName.Substring(postedBgFile.FileName.LastIndexOf('.') + 1).Trim().ToLower() == "png")
                        FileName = currYear + "_MP_" + formCollection["TOUR_CLAIM_ID"] + "_" + meetingModel.HONORARIUM_MEETING_ID + "_" + currentTime + ".png";
                    else if (postedBgFile.FileName.Substring(postedBgFile.FileName.LastIndexOf('.') + 1).Trim().ToLower() == "pdf")
                        FileName = currYear + "_MP_" + formCollection["TOUR_CLAIM_ID"] + "_" + meetingModel.HONORARIUM_MEETING_ID + "_" + currentTime + ".pdf";

                    if (postedBgFile1.FileName.Substring(postedBgFile1.FileName.LastIndexOf('.') + 1).Trim().ToLower() == "jpg")
                        fileName = currYear + "_GP_" + formCollection["TOUR_CLAIM_ID"] + "_" + meetingModel.HONORARIUM_MEETING_ID + "_" + currentTime + ".jpg";
                    else if (postedBgFile1.FileName.Substring(postedBgFile1.FileName.LastIndexOf('.') + 1).Trim().ToLower() == "jpeg")
                        fileName = currYear + "_GP_" + formCollection["TOUR_CLAIM_ID"] + "_" + meetingModel.HONORARIUM_MEETING_ID + "_" + currentTime + ".jpeg";
                    else if (postedBgFile1.FileName.Substring(postedBgFile1.FileName.LastIndexOf('.') + 1).Trim().ToLower() == "png")
                        fileName = currYear + "_GP_" + formCollection["TOUR_CLAIM_ID"] + "_" + meetingModel.HONORARIUM_MEETING_ID + "_" + currentTime + ".png";

                    if (postedBgFile2.FileName.Substring(postedBgFile2.FileName.LastIndexOf('.') + 1).Trim().ToLower() == "jpg")
                        attendanceSheetName = currYear + "_AS_" + formCollection["TOUR_CLAIM_ID"] + "_" + meetingModel.HONORARIUM_MEETING_ID + "_" + currentTime + ".jpg";
                    else if (postedBgFile2.FileName.Substring(postedBgFile2.FileName.LastIndexOf('.') + 1).Trim().ToLower() == "jpeg")
                        attendanceSheetName = currYear + "_AS_" + formCollection["TOUR_CLAIM_ID"] + "_" + meetingModel.HONORARIUM_MEETING_ID + "_" + currentTime + ".jpeg";
                    else if (postedBgFile2.FileName.Substring(postedBgFile2.FileName.LastIndexOf('.') + 1).Trim().ToLower() == "png")
                        attendanceSheetName = currYear + "_AS_" + formCollection["TOUR_CLAIM_ID"] + "_" + meetingModel.HONORARIUM_MEETING_ID + "_" + currentTime + ".png";
                    else if (postedBgFile2.FileName.Substring(postedBgFile2.FileName.LastIndexOf('.') + 1).Trim().ToLower() == "pdf")
                        attendanceSheetName = currYear + "_AS_" + formCollection["TOUR_CLAIM_ID"] + "_" + meetingModel.HONORARIUM_MEETING_ID + "_" + currentTime + ".pdf";

                    postedBgFile.SaveAs(System.IO.Path.Combine(fullPath, FileName));
                    postedBgFile1.SaveAs(System.IO.Path.Combine(fullPath, fileName));
                    postedBgFile2.SaveAs(System.IO.Path.Combine(fullPath, attendanceSheetName));
                    meetingModel.ATTENDANCE_SHEET_PATH = fullPath;
                    meetingModel.ATTENDANCE_SHEET_NAME = attendanceSheetName;
                    meetingModel.UPLOADED_FILE_PATH = fullPath;
                    meetingModel.UPLOADED_FILE_NAME = FileName;
                    meetingModel.PHOTO_NAME = fileName;
                    meetingModel.PHOTO_PATH = fullPath;

                    dbContext.NQM_TOUR_HONORARIUM_MEETING_WITH_PIU.Add(meetingModel);
                    status = dbContext.SaveChanges();
                    status = 1;

                    scope.Complete();
                }
                if (status > 0)
                {
                    IsValid = "Honorarium for Meeting with PIU details saved successfully.";
                    return true;
                }

                IsValid = "Honorarium for Meeting with PIU details saved successfully.";
                return true;
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "TourClaimDAL.InsertMeetingHonorariumDAL()");
                IsValid = "Honorarium for Meeting with PIU details not saved.";
                return false;
            }
        }

        public Array GetMeetingHonorariumListDAL(int scheduleCode, int page, int rows, string sidx, string sord, out long totalRecords)
        {
            try
            {
                var lstExecution = (from meeting in dbContext.NQM_TOUR_HONORARIUM_MEETING_WITH_PIU
                                    join master in dbContext.NQM_TOUR_CLAIM_MASTER on meeting.TOUR_CLAIM_ID equals master.TOUR_CLAIM_ID
                                    join district in dbContext.MASTER_DISTRICT on meeting.DISTRICT equals district.MAST_DISTRICT_CODE
                                    where
                                    master.ADMIN_SCHEDULE_CODE == scheduleCode
                                    select new
                                    {
                                        meeting.HONORARIUM_MEETING_ID,
                                        meeting.TOUR_CLAIM_ID,
                                        master.ADMIN_SCHEDULE_CODE,
                                        meeting.DATE_OF_MEETING,
                                        district.MAST_DISTRICT_NAME,
                                        meeting.AMOUNT_CLAIMED,
                                        meeting.AMOUNT_PASSED_CQC,
                                        meeting.DATE_OF_MODIFICATION_CQC,
                                        master.FINALIZE_FLAG,
                                        meeting.ATTENDANCE_SHEET_NAME,
                                        meeting.UPLOADED_FILE_NAME,
                                        meeting.PHOTO_NAME,
                                        master.CQC_APPROVE_DATE
                                    }).Distinct();

                totalRecords = lstExecution.Count();

                if (sidx.Trim() != string.Empty)
                {
                    if (sord.ToString() == "asc")
                    {
                        switch (sidx)
                        {
                            case "DATE_OF_MEETING":
                                lstExecution = lstExecution.OrderBy(m => m.DATE_OF_MEETING).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                                break;
                            default:
                                lstExecution = lstExecution.OrderBy(m => m.DATE_OF_MEETING).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                                break;
                        }
                    }
                    else
                    {
                        switch (sidx)
                        {
                            case "DATE_OF_MEETING":
                                lstExecution = lstExecution.OrderBy(m => m.DATE_OF_MEETING).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                                break;
                            default:
                                lstExecution = lstExecution.OrderBy(m => m.DATE_OF_MEETING).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                                break;
                        }
                    }
                }
                else
                {
                    lstExecution = lstExecution.OrderBy(m => m.DATE_OF_MEETING).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                }

                var result = lstExecution.Select(executionDetails => new
                {
                    executionDetails.HONORARIUM_MEETING_ID,
                    executionDetails.TOUR_CLAIM_ID,
                    executionDetails.ADMIN_SCHEDULE_CODE,
                    executionDetails.DATE_OF_MEETING,
                    executionDetails.MAST_DISTRICT_NAME,
                    executionDetails.AMOUNT_CLAIMED,
                    executionDetails.AMOUNT_PASSED_CQC,
                    executionDetails.DATE_OF_MODIFICATION_CQC,
                    executionDetails.FINALIZE_FLAG,
                    executionDetails.ATTENDANCE_SHEET_NAME,
                    executionDetails.UPLOADED_FILE_NAME,
                    executionDetails.PHOTO_NAME,
                    executionDetails.CQC_APPROVE_DATE
                }).ToArray();

                return result.Select(m => new
                {

                    id = m.TOUR_CLAIM_ID.ToString(),
                    cell = new[]
                    {
                        m.HONORARIUM_MEETING_ID.ToString(),
                        m.TOUR_CLAIM_ID.ToString(),
                        m.DATE_OF_MEETING.ToShortDateString(),
                        m.MAST_DISTRICT_NAME,
                        m.AMOUNT_CLAIMED.ToString(),
                        m.FINALIZE_FLAG == 5 ? ((m.AMOUNT_PASSED_CQC.ToString() == null || m.AMOUNT_PASSED_CQC.ToString() == string.Empty) ? "--" : m.AMOUNT_PASSED_CQC.ToString()) : "--",
                        m.FINALIZE_FLAG == 5 ? ((m.CQC_APPROVE_DATE.ToString() == null || m.CQC_APPROVE_DATE.ToString() == string.Empty) ? "--" : string.Format("{0:d}", m.CQC_APPROVE_DATE)) : "--",
                        "<a href='/TourClaim/ViewUploadedMeetingDetails?id1="+URLEncrypt.EncryptParameters1(new String[]{"FileName ="+m.ATTENDANCE_SHEET_NAME})+"' title='Click here to view uploaded attendance sheet' class='ui-icon 	ui-icon-search  ui-align-center' target=_blank></a>",
                        "<a href='/TourClaim/ViewUploadedMeetingDetails?id1="+URLEncrypt.EncryptParameters1(new String[]{"FileName ="+m.UPLOADED_FILE_NAME})+"' title='Click here to view uploaded file' class='ui-icon 	ui-icon-search  ui-align-center' target=_blank></a>",
                        "<a href='/TourClaim/ViewUploadedMeetingDetails?id1="+URLEncrypt.EncryptParameters1(new String[]{"FileName ="+m.PHOTO_NAME})+"' title='Click here to view uploaded file' class='ui-icon 	ui-icon-search  ui-align-center' target=_blank></a>",
                        (m.FINALIZE_FLAG >= 1)
                        ? "<a href='#' class='ui-icon ui-icon-locked ui-align-center'>"
                        : "<center><table><tr><td  style='border-color:white'><span class='ui-icon ui-icon-pencil' title='Click here to Edit Details' onClick ='EditMeetingDetails(\"" + m.HONORARIUM_MEETING_ID + "\");' ></span></td></tr></table></center>",
                        (m.FINALIZE_FLAG >= 1)
                        ? "<a href='#' class='ui-icon ui-icon-locked ui-align-center'>"
                        : "<center><table><tr><td  style='border-color:white'><span class='ui-icon ui-icon-trash' title='Click here to Delete Details' onClick ='DeleteMeetingDetails(\"" + m.HONORARIUM_MEETING_ID + "$" + m.ADMIN_SCHEDULE_CODE + "\");' ></span></td></tr></table></center>",
                    }
                }).ToArray();
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "TourClaimDAL.GetMeetingHonorariumListDAL()");
                totalRecords = 0;
                return null;
            }
            finally
            {
                dbContext.Dispose();
            }
        }

        public bool UpdateMeetingHonorariumDAL(FormCollection formCollection, HttpPostedFileBase postedBgFile, HttpPostedFileBase postedBgFile1, HttpPostedFileBase postedBgFile2, out String IsValid)
        {
            int tourClaimId = Convert.ToInt32(formCollection["TOUR_CLAIM_ID"]);
            int meetingId = Convert.ToInt32(formCollection["HONORARIUM_MEETING_ID"]);
            DateTime dateOfMeeting;
            int status = 0;

            NQM_TOUR_HONORARIUM_MEETING_WITH_PIU meetingModel = new NQM_TOUR_HONORARIUM_MEETING_WITH_PIU();
            NQM_TOUR_HONORARIUM_MEETING_WITH_PIU model = new NQM_TOUR_HONORARIUM_MEETING_WITH_PIU();
            List<NQM_TOUR_HONORARIUM_MEETING_WITH_PIU> meetingList = new List<NQM_TOUR_HONORARIUM_MEETING_WITH_PIU>();

            meetingList = dbContext.NQM_TOUR_HONORARIUM_MEETING_WITH_PIU.Where(x => x.TOUR_CLAIM_ID == tourClaimId).ToList();
            model = dbContext.NQM_TOUR_HONORARIUM_MEETING_WITH_PIU.Where(x => x.HONORARIUM_MEETING_ID == meetingId).FirstOrDefault();
            dateOfMeeting = Convert.ToDateTime(formCollection["DATE_OF_MEETING"]);
            decimal amt = dbContext.NQM_TOUR_ALLOWANCE_MASTER.Where(x => x.ALLOWANCE_TYPE.Equals("Meeting") && x.IS_ACTIVE == "Y").Select(y => y.AMOUNT).FirstOrDefault();

            status = dbContext.SaveChanges();
            try
            {
                using (var scope = new TransactionScope())
                {
                    model.DATE_OF_CLAIM = DateTime.Now;
                    model.DATE_OF_MEETING = Convert.ToDateTime(formCollection["DATE_OF_MEETING"]);
                    model.DISTRICT = Convert.ToInt32(formCollection["DISTRICT_CODE"]);

                    if (!dbContext.NQM_TOUR_HONORARIUM_MEETING_WITH_PIU.Where(x => x.TOUR_CLAIM_ID == tourClaimId && x.HONORARIUM_MEETING_ID != meetingId && x.DATE_OF_MEETING == dateOfMeeting).Any() && !dbContext.NQM_TOUR_HONORARIUM_INSPECTION_OF_ROADS.Where(x => x.TOUR_CLAIM_ID == tourClaimId && x.DATE_OF_INSPECTION == dateOfMeeting).Any())
                    {
                        model.AMOUNT_CLAIMED = amt;
                        model.AMOUNT_PASSED_CQC = amt;
                    }
                    else if (!dbContext.NQM_TOUR_HONORARIUM_MEETING_WITH_PIU.Where(x => x.TOUR_CLAIM_ID == tourClaimId && x.HONORARIUM_MEETING_ID != meetingId && x.DATE_OF_MEETING == dateOfMeeting).Any() && dbContext.NQM_TOUR_HONORARIUM_INSPECTION_OF_ROADS.Where(x => x.TOUR_CLAIM_ID == tourClaimId && x.DATE_OF_INSPECTION == dateOfMeeting).Any())
                    {
                        model.AMOUNT_CLAIMED = 0;
                        model.AMOUNT_PASSED_CQC = 0;
                    }

                    if (dbContext.NQM_TOUR_HONORARIUM_MEETING_WITH_PIU.Where(x => x.TOUR_CLAIM_ID == tourClaimId && x.HONORARIUM_MEETING_ID != meetingId && x.DATE_OF_MEETING == dateOfMeeting).Any() && dbContext.NQM_TOUR_HONORARIUM_INSPECTION_OF_ROADS.Where(x => x.TOUR_CLAIM_ID == tourClaimId && x.DATE_OF_INSPECTION == dateOfMeeting).Any())
                    {
                        foreach (var item in meetingList)
                        {
                            item.AMOUNT_CLAIMED = 0;
                            item.AMOUNT_PASSED_CQC = 0;
                            dbContext.Entry(item).State = System.Data.Entity.EntityState.Modified;
                            dbContext.SaveChanges();
                        }
                    }
                    else if (dbContext.NQM_TOUR_HONORARIUM_MEETING_WITH_PIU.Where(x => x.TOUR_CLAIM_ID == tourClaimId && x.HONORARIUM_MEETING_ID != meetingId && x.DATE_OF_MEETING == dateOfMeeting).Any() && !dbContext.NQM_TOUR_HONORARIUM_INSPECTION_OF_ROADS.Where(x => x.TOUR_CLAIM_ID == tourClaimId && x.DATE_OF_INSPECTION == dateOfMeeting).Any())
                    {
                        foreach (var item in meetingList)
                        {
                            item.AMOUNT_CLAIMED = 0;
                            item.AMOUNT_PASSED_CQC = 0;
                            dbContext.Entry(item).State = System.Data.Entity.EntityState.Modified;
                            dbContext.SaveChanges();
                        }

                        meetingModel = dbContext.NQM_TOUR_HONORARIUM_MEETING_WITH_PIU.Where(x => x.TOUR_CLAIM_ID == tourClaimId && x.DATE_OF_MEETING == dateOfMeeting).FirstOrDefault();
                        meetingModel.AMOUNT_CLAIMED = amt;
                        meetingModel.AMOUNT_PASSED_CQC = amt;
                        dbContext.Entry(meetingModel).State = System.Data.Entity.EntityState.Modified;
                        dbContext.SaveChanges();
                    }

                    String oldFilename, oldPhotoName, oldSheetName, filePath, photoPath, sheetPath, newFileName = string.Empty, newPhotoName = string.Empty, newSheetName = string.Empty;
                    String fullPathFile, fullPathSheet, fullPathPhoto;

                    if (postedBgFile.ContentLength != 0)
                    {
                        // First delete the existing file
                        oldFilename = model.UPLOADED_FILE_NAME;
                        filePath = model.UPLOADED_FILE_PATH;
                        fullPathFile = Path.Combine(filePath, oldFilename);
                        FileInfo file = new FileInfo(fullPathFile);

                        if (file.Exists)
                        {
                            file.Delete();
                        }

                        // Create new file name with old file and replace the extention.
                        string fileName = oldFilename.Split('.')[0];

                        // Create new name with corrrect extention.
                        if (postedBgFile.FileName.Substring(postedBgFile.FileName.LastIndexOf('.') + 1).Trim().ToLower() == "jpg")
                            newFileName = fileName + ".jpg";
                        else if (postedBgFile.FileName.Substring(postedBgFile.FileName.LastIndexOf('.') + 1).Trim().ToLower() == "jpeg")
                            newFileName = fileName + ".jpeg";
                        else if (postedBgFile.FileName.Substring(postedBgFile.FileName.LastIndexOf('.') + 1).Trim().ToLower() == "png")
                            newFileName = fileName + ".png";
                        else if (postedBgFile.FileName.Substring(postedBgFile.FileName.LastIndexOf('.') + 1).Trim().ToLower() == "pdf")
                            newFileName = fileName + ".pdf";

                        // Save new file with same and same path.
                        postedBgFile.SaveAs(System.IO.Path.Combine(filePath, newFileName));
                        model.UPLOADED_FILE_NAME = newFileName;
                    }

                    if (postedBgFile1.ContentLength != 0)
                    {
                        // First delete the existing file
                        oldPhotoName = model.PHOTO_NAME;
                        photoPath = model.PHOTO_PATH;
                        fullPathPhoto = Path.Combine(photoPath, oldPhotoName);
                        FileInfo file = new FileInfo(fullPathPhoto);

                        if (file.Exists)
                        {
                            file.Delete();
                        }

                        // Create new file name with old file and replace the extention.
                        string fileName = oldPhotoName.Split('.')[0];

                        // Create new name with corrrect extention.
                        if (postedBgFile1.FileName.Substring(postedBgFile1.FileName.LastIndexOf('.') + 1).Trim().ToLower() == "jpg")
                            newPhotoName = fileName + ".jpg";
                        else if (postedBgFile1.FileName.Substring(postedBgFile1.FileName.LastIndexOf('.') + 1).Trim().ToLower() == "jpeg")
                            newPhotoName = fileName + ".jpeg";
                        else if (postedBgFile1.FileName.Substring(postedBgFile1.FileName.LastIndexOf('.') + 1).Trim().ToLower() == "png")
                            newPhotoName = fileName + ".png";

                        // Save new file with same and same path.
                        postedBgFile1.SaveAs(System.IO.Path.Combine(photoPath, newPhotoName));
                        model.PHOTO_NAME = newPhotoName;
                    }

                    if (postedBgFile2.ContentLength != 0)
                    {
                        // First delete the existing file
                        oldSheetName = model.ATTENDANCE_SHEET_NAME;
                        sheetPath = model.ATTENDANCE_SHEET_PATH;
                        fullPathSheet = Path.Combine(sheetPath, oldSheetName);
                        FileInfo file = new FileInfo(fullPathSheet);

                        if (file.Exists)
                        {
                            file.Delete();
                        }

                        // Create new file name with old file and replace the extention.
                        string fileName = oldSheetName.Split('.')[0];

                        // Create new name with corrrect extention.
                        if (postedBgFile2.FileName.Substring(postedBgFile2.FileName.LastIndexOf('.') + 1).Trim().ToLower() == "jpg")
                            newSheetName = fileName + ".jpg";
                        else if (postedBgFile2.FileName.Substring(postedBgFile2.FileName.LastIndexOf('.') + 1).Trim().ToLower() == "jpeg")
                            newSheetName = fileName + ".jpeg";
                        else if (postedBgFile2.FileName.Substring(postedBgFile2.FileName.LastIndexOf('.') + 1).Trim().ToLower() == "png")
                            newSheetName = fileName + ".png";
                        else if (postedBgFile2.FileName.Substring(postedBgFile2.FileName.LastIndexOf('.') + 1).Trim().ToLower() == "pdf")
                            newSheetName = fileName + ".pdf";

                        // Save new file with same and same path.
                        postedBgFile2.SaveAs(System.IO.Path.Combine(sheetPath, newSheetName));
                        model.ATTENDANCE_SHEET_NAME = newSheetName;
                    }

                    dbContext.Entry(model).State = System.Data.Entity.EntityState.Modified;
                    dbContext.SaveChanges();
                    status = 1;

                    scope.Complete();
                }
                if (status > 0)
                {
                    IsValid = "Honorarium for Meeting with PIU details updated successfully.";
                    return true;
                }

                IsValid = "Honorarium for Meeting with PIU details updated successfully.";
                return true;
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "TourClaimDAL.UpdateMeetingHonorariumDAL()");
                IsValid = "Honorarium for Meeting with PIU details not updated.";
                return false;
            }
        }

        public bool InsertMiscellaneousClaimDetailsDAL(FormCollection formCollection, HttpPostedFileBase postedBgFile, out String IsValid)
        {
            int status = 0;
            NQM_TOUR_MISCELLANEOUS misModel = new NQM_TOUR_MISCELLANEOUS();
            string currYear = DateTime.Now.Year.ToString();

            status = dbContext.SaveChanges();
            try
            {
                using (var scope = new TransactionScope())
                {
                    misModel.MISCELLANEOUS_ID = dbContext.NQM_TOUR_MISCELLANEOUS.Max(cp => (Int32?)cp.MISCELLANEOUS_ID) == null ? 1 : (Int32)dbContext.NQM_TOUR_MISCELLANEOUS.Max(cp => (Int32?)cp.MISCELLANEOUS_ID) + 1;
                    misModel.TOUR_CLAIM_ID = Convert.ToInt32(formCollection["TOUR_CLAIM_ID"]);
                    misModel.DATE = Convert.ToDateTime(formCollection["DATE"]);
                    misModel.DATE_OF_CLAIM = DateTime.Now;
                    misModel.DESCRIPTION = formCollection["DESCRIPTION"];
                    misModel.AMOUNT_CLAIMED = Convert.ToDecimal(formCollection["AMOUNT_CLAIMED"]);
                    misModel.AMOUNT_PASSED_CQC = Convert.ToDecimal(formCollection["AMOUNT_CLAIMED"]);
                    misModel.USERID = PMGSYSession.Current.UserId;
                    misModel.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];

                    if (postedBgFile.ContentLength > 0)
                    {
                        String FilePath = ConfigurationManager.AppSettings["NQM_TOUR_CLAIM_PATH"];
                        var fullPath = Path.Combine(FilePath, "MisClaim", currYear);

                        if (!Directory.Exists(fullPath))
                        {
                            Directory.CreateDirectory(fullPath);
                        }

                        DateTime d = DateTime.Now;
                        String format = "d_MMM_yyyy_HH_mm_ss";
                        String currentTime = d.ToString(format);
                        String FileName = string.Empty;

                        if (postedBgFile.FileName.Substring(postedBgFile.FileName.LastIndexOf('.') + 1).Trim().ToLower() == "jpg")
                            FileName = currYear + "_Mis_" + formCollection["TOUR_CLAIM_ID"] + "_" + misModel.MISCELLANEOUS_ID + "_" + currentTime + ".jpg";
                        else if (postedBgFile.FileName.Substring(postedBgFile.FileName.LastIndexOf('.') + 1).Trim().ToLower() == "jpeg")
                            FileName = currYear + "_Mis_" + formCollection["TOUR_CLAIM_ID"] + "_" + misModel.MISCELLANEOUS_ID + "_" + currentTime + ".jpeg";
                        else if (postedBgFile.FileName.Substring(postedBgFile.FileName.LastIndexOf('.') + 1).Trim().ToLower() == "png")
                            FileName = currYear + "_Mis_" + formCollection["TOUR_CLAIM_ID"] + "_" + misModel.MISCELLANEOUS_ID + "_" + currentTime + ".png";
                        else if (postedBgFile.FileName.Substring(postedBgFile.FileName.LastIndexOf('.') + 1).Trim().ToLower() == "pdf")
                            FileName = currYear + "_Mis_" + formCollection["TOUR_CLAIM_ID"] + "_" + misModel.MISCELLANEOUS_ID + "_" + currentTime + ".pdf";

                        postedBgFile.SaveAs(System.IO.Path.Combine(fullPath, FileName));
                        misModel.UPLOADED_FILE_PATH = fullPath;
                        misModel.UPLOADED_FILE_NAME = FileName;
                    }

                    dbContext.NQM_TOUR_MISCELLANEOUS.Add(misModel);
                    status = dbContext.SaveChanges();
                    status = 1;

                    scope.Complete();
                }
                if (status > 0)
                {
                    IsValid = "Miscellaneous Claim Details saved successfully.";
                    return true;
                }

                IsValid = "Miscellaneous Claim Details saved successfully.";
                return true;
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "TourClaimDAL.InsertMiscellaneousClaimDetailsDAL()");
                IsValid = "Miscellaneous Claim Details not saved.";
                return false;
            }
        }

        public Array GetMiscellaneousClaimListDAL(int scheduleCode, int page, int rows, string sidx, string sord, out long totalRecords)
        {
            try
            {
                var lstExecution = (from mis in dbContext.NQM_TOUR_MISCELLANEOUS
                                    join master in dbContext.NQM_TOUR_CLAIM_MASTER on mis.TOUR_CLAIM_ID equals master.TOUR_CLAIM_ID
                                    where
                                    master.ADMIN_SCHEDULE_CODE == scheduleCode
                                    select new
                                    {
                                        mis.MISCELLANEOUS_ID,
                                        mis.TOUR_CLAIM_ID,
                                        master.ADMIN_SCHEDULE_CODE,
                                        mis.DATE,
                                        mis.DESCRIPTION,
                                        mis.AMOUNT_CLAIMED,
                                        mis.AMOUNT_PASSED_CQC,
                                        mis.DATE_OF_MODIFICATION_CQC,
                                        master.FINALIZE_FLAG,
                                        mis.UPLOADED_FILE_NAME,
                                        master.CQC_APPROVE_DATE
                                    }).Distinct();

                totalRecords = lstExecution.Count();

                if (sidx.Trim() != string.Empty)
                {
                    if (sord.ToString() == "asc")
                    {
                        switch (sidx)
                        {
                            case "TOUR_CLAIM_ID":
                                lstExecution = lstExecution.OrderByDescending(m => m.MISCELLANEOUS_ID).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                                break;
                            default:
                                lstExecution = lstExecution.OrderByDescending(m => m.MISCELLANEOUS_ID).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                                break;
                        }
                    }
                    else
                    {
                        switch (sidx)
                        {
                            case "TOUR_CLAIM_ID":
                                lstExecution = lstExecution.OrderByDescending(m => m.MISCELLANEOUS_ID).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                                break;
                            default:
                                lstExecution = lstExecution.OrderByDescending(m => m.MISCELLANEOUS_ID).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                                break;
                        }
                    }
                }
                else
                {
                    lstExecution = lstExecution.OrderByDescending(m => m.MISCELLANEOUS_ID).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                }

                var result = lstExecution.Select(executionDetails => new
                {
                    executionDetails.MISCELLANEOUS_ID,
                    executionDetails.TOUR_CLAIM_ID,
                    executionDetails.ADMIN_SCHEDULE_CODE,
                    executionDetails.DATE,
                    executionDetails.DESCRIPTION,
                    executionDetails.AMOUNT_CLAIMED,
                    executionDetails.AMOUNT_PASSED_CQC,
                    executionDetails.DATE_OF_MODIFICATION_CQC,
                    executionDetails.FINALIZE_FLAG,
                    executionDetails.UPLOADED_FILE_NAME,
                    executionDetails.CQC_APPROVE_DATE
                }).ToArray();

                return result.Select(m => new
                {
                    id = m.TOUR_CLAIM_ID.ToString(),
                    cell = new[]
                    {
                        m.MISCELLANEOUS_ID.ToString(),
                        m.TOUR_CLAIM_ID.ToString(),
                        m.DATE.ToShortDateString(),
                        m.DESCRIPTION,
                        m.AMOUNT_CLAIMED.ToString(),
                        m.FINALIZE_FLAG == 5 ? ((m.AMOUNT_PASSED_CQC.ToString() == null || m.AMOUNT_PASSED_CQC.ToString() == string.Empty) ? "--" : m.AMOUNT_PASSED_CQC.ToString()) : "--",
                        m.FINALIZE_FLAG == 5 ? ((m.CQC_APPROVE_DATE.ToString() == null || m.CQC_APPROVE_DATE.ToString() == string.Empty) ? "--" : string.Format("{0:d}", m.CQC_APPROVE_DATE)) : "--",
                        m.UPLOADED_FILE_NAME == null ? "--" : "<a href='/TourClaim/ViewUploadedMisDetails?id1="+URLEncrypt.EncryptParameters1(new String[]{"FileName ="+m.UPLOADED_FILE_NAME})+"' title='Click here to view uploaded file' class='ui-icon 	ui-icon-search  ui-align-center' target=_blank></a>",
                        (m.FINALIZE_FLAG >= 1)
                        ? "<a href='#' class='ui-icon ui-icon-locked ui-align-center'>"
                        : "<center><table><tr><td  style='border-color:white'><span class='ui-icon ui-icon-pencil' title='Click here to Edit Details' onClick ='EditMiscellaneousDetails(\"" + m.MISCELLANEOUS_ID + "$" + m.ADMIN_SCHEDULE_CODE + "\");' ></span></td></tr></table></center>",
                        (m.FINALIZE_FLAG >= 1)
                        ? "<a href='#' class='ui-icon ui-icon-locked ui-align-center'>"
                        : "<center><table><tr><td  style='border-color:white'><span class='ui-icon ui-icon-trash' title='Click here to Delete Details' onClick ='DeleteMiscellaneousDetails(\"" + m.MISCELLANEOUS_ID + "$" + m.ADMIN_SCHEDULE_CODE + "\");' ></span></td></tr></table></center>",
                    }
                }).ToArray();
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "TourClaimDAL.GetMiscellaneousClaimListDAL()");
                totalRecords = 0;
                return null;
            }
            finally
            {
                dbContext.Dispose();
            }
        }

        public bool UpdateMiscellaneousClaimDetailsDAL(FormCollection formCollection, HttpPostedFileBase postedBgFile, out String IsValid)
        {
            int status = 0;
            NQM_TOUR_MISCELLANEOUS misModel = new NQM_TOUR_MISCELLANEOUS();
            int misId = Convert.ToInt32(formCollection["MISCELLANEOUS_ID"]);
            string newFileName = string.Empty;

            status = dbContext.SaveChanges();
            try
            {
                using (var scope = new TransactionScope())
                {
                    misModel = dbContext.NQM_TOUR_MISCELLANEOUS.Where(x => x.MISCELLANEOUS_ID == misId).FirstOrDefault();

                    misModel.DATE_OF_CLAIM = DateTime.Now;
                    misModel.DATE = Convert.ToDateTime(formCollection["DATE"]);
                    misModel.DESCRIPTION = formCollection["DESCRIPTION"];
                    misModel.AMOUNT_CLAIMED = Convert.ToDecimal(formCollection["AMOUNT_CLAIMED"]);
                    misModel.AMOUNT_PASSED_CQC = Convert.ToDecimal(formCollection["AMOUNT_CLAIMED"]);
                    misModel.UPLOADED_FILE_PATH = null;
                    misModel.UPLOADED_FILE_NAME = null;

                    if (postedBgFile.ContentLength > 0)
                    {
                        string currYear = DateTime.Now.Year.ToString();
                        String FilePath = ConfigurationManager.AppSettings["NQM_TOUR_CLAIM_PATH"];
                        var fullPath = Path.Combine(FilePath, "MisClaim", currYear);

                        if (!Directory.Exists(fullPath))
                        {
                            Directory.CreateDirectory(fullPath);
                        }

                        DateTime d = DateTime.Now;
                        String format = "d_MMM_yyyy_HH_mm_ss";
                        String currentTime = d.ToString(format);
                        String FileName = string.Empty;

                        if (postedBgFile.FileName.Substring(postedBgFile.FileName.LastIndexOf('.') + 1).Trim().ToLower() == "jpg")
                            FileName = currYear + "_Mis_" + formCollection["TOUR_CLAIM_ID"] + "_" + misModel.MISCELLANEOUS_ID + "_" + currentTime + ".jpg";
                        else if (postedBgFile.FileName.Substring(postedBgFile.FileName.LastIndexOf('.') + 1).Trim().ToLower() == "jpeg")
                            FileName = currYear + "_Mis_" + formCollection["TOUR_CLAIM_ID"] + "_" + misModel.MISCELLANEOUS_ID + "_" + currentTime + ".jpeg";
                        else if (postedBgFile.FileName.Substring(postedBgFile.FileName.LastIndexOf('.') + 1).Trim().ToLower() == "png")
                            FileName = currYear + "_Mis_" + formCollection["TOUR_CLAIM_ID"] + "_" + misModel.MISCELLANEOUS_ID + "_" + currentTime + ".png";
                        else if (postedBgFile.FileName.Substring(postedBgFile.FileName.LastIndexOf('.') + 1).Trim().ToLower() == "pdf")
                            FileName = currYear + "_Mis_" + formCollection["TOUR_CLAIM_ID"] + "_" + misModel.MISCELLANEOUS_ID + "_" + currentTime + ".pdf";

                        postedBgFile.SaveAs(System.IO.Path.Combine(fullPath, FileName));
                        misModel.UPLOADED_FILE_PATH = fullPath;
                        misModel.UPLOADED_FILE_NAME = FileName;
                    }

                    dbContext.Entry(misModel).State = System.Data.Entity.EntityState.Modified;
                    dbContext.SaveChanges();
                    status = 1;

                    scope.Complete();
                }
                if (status > 0)
                {
                    IsValid = "Miscellaneous Claim Details updated successfully.";
                    return true;
                }

                IsValid = "Miscellaneous Claim Details updated successfully.";
                return true;
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "TourClaimDAL.UpdateMiscellaneousClaimDetailsDAL()");
                IsValid = "Miscellaneous Claim Details not updated.";
                return false;
            }
        }

        public bool InsertPermissionClaimDetailsDAL(FormCollection formCollection, HttpPostedFileBase postedBgFile, out String IsValid)
        {
            int status = 0;
            NQM_TOUR_PERMISSION_DETAILS perModel = new NQM_TOUR_PERMISSION_DETAILS();
            string currYear = DateTime.Now.Year.ToString();

            status = dbContext.SaveChanges();
            try
            {
                using (var scope = new TransactionScope())
                {
                    perModel.ID = dbContext.NQM_TOUR_PERMISSION_DETAILS.Max(cp => (Int32?)cp.ID) == null ? 1 : (Int32)dbContext.NQM_TOUR_PERMISSION_DETAILS.Max(cp => (Int32?)cp.ID) + 1;
                    perModel.TOUR_CLAIM_ID = Convert.ToInt32(formCollection["TOUR_CLAIM_ID"]);
                    perModel.DATE = Convert.ToDateTime(formCollection["DATE"]);
                    perModel.DATE_OF_CLAIM = DateTime.Now;
                    perModel.REMARK = formCollection["DESCRIPTION"];
                    perModel.USERID = PMGSYSession.Current.UserId;
                    perModel.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];

                    if (postedBgFile.ContentLength > 0)
                    {
                        String FilePath = ConfigurationManager.AppSettings["NQM_TOUR_CLAIM_PATH"];
                        var fullPath = Path.Combine(FilePath, "Permissions", currYear);

                        if (!Directory.Exists(fullPath))
                        {
                            Directory.CreateDirectory(fullPath);
                        }

                        DateTime d = DateTime.Now;
                        String format = "d_MMM_yyyy_HH_mm_ss";
                        String currentTime = d.ToString(format);
                        String FileName = string.Empty;

                        if (postedBgFile.FileName.Substring(postedBgFile.FileName.LastIndexOf('.') + 1).Trim().ToLower() == "jpg")
                            FileName = currYear + "_Per_" + formCollection["TOUR_CLAIM_ID"] + "_" + perModel.ID + "_" + currentTime + ".jpg";
                        else if (postedBgFile.FileName.Substring(postedBgFile.FileName.LastIndexOf('.') + 1).Trim().ToLower() == "jpeg")
                            FileName = currYear + "_Per_" + formCollection["TOUR_CLAIM_ID"] + "_" + perModel.ID + "_" + currentTime + ".jpeg";
                        else if (postedBgFile.FileName.Substring(postedBgFile.FileName.LastIndexOf('.') + 1).Trim().ToLower() == "png")
                            FileName = currYear + "_Per_" + formCollection["TOUR_CLAIM_ID"] + "_" + perModel.ID + "_" + currentTime + ".png";
                        else if (postedBgFile.FileName.Substring(postedBgFile.FileName.LastIndexOf('.') + 1).Trim().ToLower() == "pdf")
                            FileName = currYear + "_Per_" + formCollection["TOUR_CLAIM_ID"] + "_" + perModel.ID + "_" + currentTime + ".pdf";

                        postedBgFile.SaveAs(System.IO.Path.Combine(fullPath, FileName));
                        perModel.PERMISSION_DOCUMENT_PATH = fullPath;
                        perModel.PERMISSION_DOCUMENT_NAME = FileName;
                    }

                    dbContext.NQM_TOUR_PERMISSION_DETAILS.Add(perModel);
                    status = dbContext.SaveChanges();
                    status = 1;

                    scope.Complete();
                }
                if (status > 0)
                {
                    IsValid = "Permission Details saved successfully.";
                    return true;
                }

                IsValid = "Permission Details saved successfully.";
                return true;
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "TourClaimDAL.InsertPermissionClaimDetailsDAL()");
                IsValid = "Permission Details not saved.";
                return false;
            }
        }

        public Array GetPermissionClaimListDAL(int scheduleCode, int page, int rows, string sidx, string sord, out long totalRecords)
        {
            try
            {
                var lstExecution = (from per in dbContext.NQM_TOUR_PERMISSION_DETAILS
                                    join master in dbContext.NQM_TOUR_CLAIM_MASTER on per.TOUR_CLAIM_ID equals master.TOUR_CLAIM_ID
                                    where
                                    master.ADMIN_SCHEDULE_CODE == scheduleCode
                                    select new
                                    {
                                        per.ID,
                                        per.TOUR_CLAIM_ID,
                                        master.ADMIN_SCHEDULE_CODE,
                                        per.DATE,
                                        per.REMARK,
                                        master.FINALIZE_FLAG,
                                        per.PERMISSION_DOCUMENT_NAME,
                                        master.CQC_APPROVE_DATE
                                    }).Distinct();

                totalRecords = lstExecution.Count();

                if (sidx.Trim() != string.Empty)
                {
                    if (sord.ToString() == "asc")
                    {
                        switch (sidx)
                        {
                            case "TOUR_CLAIM_ID":
                                lstExecution = lstExecution.OrderByDescending(m => m.ID).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                                break;
                            default:
                                lstExecution = lstExecution.OrderByDescending(m => m.ID).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                                break;
                        }
                    }
                    else
                    {
                        switch (sidx)
                        {
                            case "TOUR_CLAIM_ID":
                                lstExecution = lstExecution.OrderByDescending(m => m.ID).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                                break;
                            default:
                                lstExecution = lstExecution.OrderByDescending(m => m.ID).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                                break;
                        }
                    }
                }
                else
                {
                    lstExecution = lstExecution.OrderByDescending(m => m.ID).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                }

                var result = lstExecution.Select(executionDetails => new
                {
                    executionDetails.ID,
                    executionDetails.TOUR_CLAIM_ID,
                    executionDetails.ADMIN_SCHEDULE_CODE,
                    executionDetails.DATE,
                    executionDetails.REMARK,
                    executionDetails.FINALIZE_FLAG,
                    executionDetails.PERMISSION_DOCUMENT_NAME,
                    executionDetails.CQC_APPROVE_DATE
                }).ToArray();

                return result.Select(m => new
                {
                    id = m.TOUR_CLAIM_ID.ToString(),
                    cell = new[]
                    {
                        m.ID.ToString(),
                        m.TOUR_CLAIM_ID.ToString(),
                        m.DATE.ToShortDateString(),
                        m.REMARK,
                        "<a href='/TourClaim/ViewUploadedPermissionDetails?id1="+URLEncrypt.EncryptParameters1(new String[]{"FileName ="+m.PERMISSION_DOCUMENT_NAME})+"' title='Click here to view uploaded file' class='ui-icon 	ui-icon-search  ui-align-center' target=_blank></a>",
                        (m.FINALIZE_FLAG >= 1)
                        ? "<a href='#' class='ui-icon ui-icon-locked ui-align-center'>"
                        : "<center><table><tr><td  style='border-color:white'><span class='ui-icon ui-icon-pencil' title='Click here to Edit Details' onClick ='EditPermissionDetails(\"" + m.ID + "$" + m.ADMIN_SCHEDULE_CODE + "\");' ></span></td></tr></table></center>",
                        (m.FINALIZE_FLAG >= 1)
                        ? "<a href='#' class='ui-icon ui-icon-locked ui-align-center'>"
                        : "<center><table><tr><td  style='border-color:white'><span class='ui-icon ui-icon-trash' title='Click here to Delete Details' onClick ='DeletePermissionDetails(\"" + m.ID + "$" + m.ADMIN_SCHEDULE_CODE + "\");' ></span></td></tr></table></center>",
                    }
                }).ToArray();
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "TourClaimDAL.GetPermissionClaimListDAL()");
                totalRecords = 0;
                return null;
            }
            finally
            {
                dbContext.Dispose();
            }
        }

        public bool UpdatePermissionClaimDetailsDAL(FormCollection formCollection, HttpPostedFileBase postedBgFile, out String IsValid)
        {
            int status = 0;
            NQM_TOUR_PERMISSION_DETAILS perModel = new NQM_TOUR_PERMISSION_DETAILS();
            int perId = Convert.ToInt32(formCollection["PERMISSION_ID"]);
            string newDocName = string.Empty;

            status = dbContext.SaveChanges();
            try
            {
                using (var scope = new TransactionScope())
                {
                    perModel = dbContext.NQM_TOUR_PERMISSION_DETAILS.Where(x => x.ID == perId).FirstOrDefault();

                    perModel.DATE_OF_CLAIM = DateTime.Now;
                    perModel.DATE = Convert.ToDateTime(formCollection["DATE"]);
                    perModel.REMARK = formCollection["DESCRIPTION"];

                    

                    if (postedBgFile.ContentLength > 0)
                    {
                        // First delete the existing file
                        string oldDocName = perModel.PERMISSION_DOCUMENT_NAME;
                        string docPath = perModel.PERMISSION_DOCUMENT_PATH;
                        string fullPathDoc = Path.Combine(docPath, oldDocName);
                        FileInfo file = new FileInfo(fullPathDoc);

                        if (file.Exists)
                        {
                            file.Delete();
                        }

                        // Create new file name with old file and replace the extention.
                        string fileName = oldDocName.Split('.')[0];

                        // Create new name with corrrect extention.
                        if (postedBgFile.FileName.Substring(postedBgFile.FileName.LastIndexOf('.') + 1).Trim().ToLower() == "jpg")
                            newDocName = fileName + ".jpg";
                        else if (postedBgFile.FileName.Substring(postedBgFile.FileName.LastIndexOf('.') + 1).Trim().ToLower() == "jpeg")
                            newDocName = fileName + ".jpeg";
                        else if (postedBgFile.FileName.Substring(postedBgFile.FileName.LastIndexOf('.') + 1).Trim().ToLower() == "png")
                            newDocName = fileName + ".png";
                        else if (postedBgFile.FileName.Substring(postedBgFile.FileName.LastIndexOf('.') + 1).Trim().ToLower() == "pdf")
                            newDocName = fileName + ".pdf";

                        // Save new file with same and same path.
                        postedBgFile.SaveAs(System.IO.Path.Combine(docPath, newDocName));
                        perModel.PERMISSION_DOCUMENT_NAME = newDocName;
                    }

                    dbContext.Entry(perModel).State = System.Data.Entity.EntityState.Modified;
                    dbContext.SaveChanges();
                    status = 1;

                    scope.Complete();
                }
                if (status > 0)
                {
                    IsValid = "Permission Details updated successfully.";
                    return true;
                }

                IsValid = "Permission Details updated successfully.";
                return true;
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "TourClaimDAL.UpdatePermissionClaimDetailsDAL()");
                IsValid = "Permission Details not updated.";
                return false;
            }
        }

        #endregion

        #region CQC Tour Claim

        public Array GetTourDistrictListCqcDAL(int scheduleCode, int page, int rows, string sidx, string sord, out long totalRecords)
        {
            try
            {
                var lstExecution = (from item in dbContext.NQM_TOUR_DISTRICT_DETAILS
                                    join master in dbContext.NQM_TOUR_CLAIM_MASTER on item.TOUR_CLAIM_ID equals master.TOUR_CLAIM_ID
                                    join district in dbContext.MASTER_DISTRICT on item.DISTRICT equals district.MAST_DISTRICT_CODE
                                    where
                                    master.ADMIN_SCHEDULE_CODE == scheduleCode
                                    select new
                                    {
                                        item.TOUR_CLAIM_ID,
                                        master.ADMIN_SCHEDULE_CODE,
                                        item.DISTRICT_DETAILS_ID,
                                        district.MAST_DISTRICT_NAME,
                                        item.DATE_FROM,
                                        item.DATE_TO
                                    }).Distinct();

                totalRecords = lstExecution.Count();

                if (sidx.Trim() != string.Empty)
                {
                    if (sord.ToString() == "asc")
                    {
                        switch (sidx)
                        {
                            case "DATE_FROM":
                                lstExecution = lstExecution.OrderBy(m => m.DATE_FROM).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                                break;
                            default:
                                lstExecution = lstExecution.OrderBy(m => m.DATE_FROM).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                                break;
                        }
                    }
                    else
                    {
                        switch (sidx)
                        {
                            case "DATE_FROM":
                                lstExecution = lstExecution.OrderBy(m => m.DATE_FROM).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                                break;
                            default:
                                lstExecution = lstExecution.OrderBy(m => m.DATE_FROM).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                                break;
                        }
                    }
                }
                else
                {
                    lstExecution = lstExecution.OrderBy(m => m.DATE_FROM).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                }

                var result = lstExecution.Select(executionDetails => new
                {
                    executionDetails.TOUR_CLAIM_ID,
                    executionDetails.ADMIN_SCHEDULE_CODE,
                    executionDetails.DISTRICT_DETAILS_ID,
                    executionDetails.MAST_DISTRICT_NAME,
                    executionDetails.DATE_FROM,
                    executionDetails.DATE_TO
                }).ToArray();

                return result.Select(m => new
                {
                    id = m.TOUR_CLAIM_ID.ToString(),
                    cell = new[]
                    {
                        m.TOUR_CLAIM_ID.ToString(),
                        m.MAST_DISTRICT_NAME,
                        m.DATE_FROM.ToShortDateString(),
                        m.DATE_TO.ToShortDateString(),
                        "<center><table><tr><td  style='border-color:white'><span class='ui-icon ui-icon-zoomin' title='Click here to view Quick Response Sheet' onClick ='ViewQuickResponseSheet(\"" + m.DISTRICT_DETAILS_ID + "\");' ></span></td></tr></table></center>",
                    }
                }).ToArray();
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "TourClaimDAL.GetTourDistrictListCqcDAL()");
                totalRecords = 0;
                return null;
            }
            finally
            {
                dbContext.Dispose();
            }
        }

        public bool FinalizeSanctionTourDetailsDAL(string id, out String IsValid)
        {
            NQM_TOUR_CLAIM_MASTER tourModel = new NQM_TOUR_CLAIM_MASTER();
            NQM_TOUR_DISTRICT_DETAILS districtModel = new NQM_TOUR_DISTRICT_DETAILS();
            NQM_TRAVEL_CLAIM_DETAILS travelModel = new NQM_TRAVEL_CLAIM_DETAILS();
            NQM_LODGE_AND_DAILY_CLAIM_DETAILS lodgeModel = new NQM_LODGE_AND_DAILY_CLAIM_DETAILS();
            NQM_TOUR_HONORARIUM_INSPECTION_OF_ROADS inspModel = new NQM_TOUR_HONORARIUM_INSPECTION_OF_ROADS();
            NQM_TOUR_HONORARIUM_MEETING_WITH_PIU meetingModel = new NQM_TOUR_HONORARIUM_MEETING_WITH_PIU();
            NQM_TOUR_MISCELLANEOUS misModel = new NQM_TOUR_MISCELLANEOUS();

            int status = 0;
            int scheduleCode = Convert.ToInt32(id.Split('$')[0]);
            string forwardRemark = string.Empty, approveRemark = string.Empty, officeAssistantName = string.Empty, directorName = string.Empty;

            if (id.Split('$')[1].Contains("FrwdRem"))
            {
                forwardRemark = id.Split('$')[2];
                officeAssistantName = id.Split('$')[4];
            }
            if (id.Split('$')[1].Contains("P3Name"))
            {
                officeAssistantName = id.Split('$')[2];
            }

            //string remark = id.Split('$')[1];

            status = dbContext.SaveChanges();
            try
            {
                using (var scope = new TransactionScope())
                {
                    int tourClaimId = dbContext.NQM_TOUR_CLAIM_MASTER.Where(x => x.ADMIN_SCHEDULE_CODE == scheduleCode).Select(y => y.TOUR_CLAIM_ID).FirstOrDefault();
                    int roundSeq = dbContext.NQM_TOUR_CLAIM_MASTER.Where(x => x.ADMIN_SCHEDULE_CODE == scheduleCode).Select(y => y.ROUNDS_SEQUENCE).FirstOrDefault();

                    if (roundSeq == 0)
                    {
                        // add amount to Finance, related to travel claims 
                        List<int> travelIds = new List<int>();

                        travelIds = dbContext.NQM_TRAVEL_CLAIM_DETAILS.Where(x => x.TOUR_CLAIM_ID == tourClaimId).Select(y => y.TRAVEL_CLAIM_ID).ToList();

                        foreach (int item in travelIds)
                        {
                            travelModel = dbContext.NQM_TRAVEL_CLAIM_DETAILS.Where(x => x.TRAVEL_CLAIM_ID == item).FirstOrDefault();

                            if (travelModel.AMOUNT_PASSED_CQC == null)
                            {
                                IsValid = "Please add amount for travel claims before finalize.";
                                return false;
                            }

                            travelModel.AMOUNT_PASSED_FIN1 = travelModel.AMOUNT_PASSED_CQC;

                            dbContext.Entry(travelModel).State = System.Data.Entity.EntityState.Modified;
                            dbContext.SaveChanges();
                        }

                        // finalize lodge and daily claims related to the tour
                        List<int> lodgeIds = new List<int>();

                        lodgeIds = dbContext.NQM_LODGE_AND_DAILY_CLAIM_DETAILS.Where(x => x.TOUR_CLAIM_ID == tourClaimId).Select(y => y.LODGE_CLAIM_ID).ToList();

                        foreach (int item in lodgeIds)
                        {
                            lodgeModel = dbContext.NQM_LODGE_AND_DAILY_CLAIM_DETAILS.Where(x => x.LODGE_CLAIM_ID == item).FirstOrDefault();

                            if (lodgeModel.AMOUNT_PASSED_DAILY == null && lodgeModel.AMOUNT_PASSED_HOTEL_CQC == null)
                            {
                                IsValid = "Please add amount for lodge or daily claims before finalize.";
                                return false;
                            }

                            lodgeModel.AMOUNT_PASSED_HOTEL_FIN1 = lodgeModel.AMOUNT_PASSED_HOTEL_CQC;

                            dbContext.Entry(lodgeModel).State = System.Data.Entity.EntityState.Modified;
                            dbContext.SaveChanges();
                        }

                        // finalize inspection claims related to the tour
                        List<int> inspIds = new List<int>();

                        inspIds = dbContext.NQM_TOUR_HONORARIUM_INSPECTION_OF_ROADS.Where(x => x.TOUR_CLAIM_ID == tourClaimId).Select(y => y.HONORARIUM_INSPECTION_ID).ToList();

                        foreach (int item in inspIds)
                        {
                            inspModel = dbContext.NQM_TOUR_HONORARIUM_INSPECTION_OF_ROADS.Where(x => x.HONORARIUM_INSPECTION_ID == item).FirstOrDefault();

                            if (inspModel.AMOUNT_PASSED_CQC == null)
                            {
                                IsValid = "Please add amount for inspection of roads allowance before finalize.";
                                return false;
                            }

                            inspModel.AMOUNT_PASSED_FIN1 = inspModel.AMOUNT_PASSED_CQC;

                            dbContext.Entry(inspModel).State = System.Data.Entity.EntityState.Modified;
                            dbContext.SaveChanges();
                        }

                        // finalize meeting claims related to the tour
                        List<int> meetingIds = new List<int>();

                        meetingIds = dbContext.NQM_TOUR_HONORARIUM_MEETING_WITH_PIU.Where(x => x.TOUR_CLAIM_ID == tourClaimId).Select(y => y.HONORARIUM_MEETING_ID).ToList();

                        foreach (int item in meetingIds)
                        {
                            meetingModel = dbContext.NQM_TOUR_HONORARIUM_MEETING_WITH_PIU.Where(x => x.HONORARIUM_MEETING_ID == item).FirstOrDefault();

                            if (meetingModel.AMOUNT_PASSED_CQC == null)
                            {
                                IsValid = "Please add amount for meeting with PIU allowance before finalize.";
                                return false;
                            }

                            meetingModel.AMOUNT_PASSED_FIN1 = meetingModel.AMOUNT_PASSED_CQC;

                            dbContext.Entry(meetingModel).State = System.Data.Entity.EntityState.Modified;
                            dbContext.SaveChanges();
                        }

                        // finalize miscellaneous claims related to the tour
                        List<int> misIds = new List<int>();

                        misIds = dbContext.NQM_TOUR_MISCELLANEOUS.Where(x => x.TOUR_CLAIM_ID == tourClaimId).Select(y => y.MISCELLANEOUS_ID).ToList();

                        foreach (int item in misIds)
                        {
                            misModel = dbContext.NQM_TOUR_MISCELLANEOUS.Where(x => x.MISCELLANEOUS_ID == item).FirstOrDefault();

                            if (misModel.AMOUNT_PASSED_CQC == null)
                            {
                                IsValid = "Please add amount for miscellaneous claims before finalize.";
                                return false;
                            }

                            misModel.AMOUNT_PASSED_FIN1 = misModel.AMOUNT_PASSED_CQC;

                            dbContext.Entry(misModel).State = System.Data.Entity.EntityState.Modified;
                            dbContext.SaveChanges();
                        }
                    }

                    // finalize tour details 
                    tourModel = dbContext.NQM_TOUR_CLAIM_MASTER.Where(x => x.TOUR_CLAIM_ID == tourClaimId).FirstOrDefault();
                    tourModel.FINALIZE_FLAG = 2;
                    if (roundSeq == 0)
                    {
                        tourModel.CQC_FINALIZATION_DATE = DateTime.Now;
                        tourModel.CQC_FINALIZE_OFFICE_ASSISTANT_P_III = officeAssistantName;
                    }
                    if (roundSeq == 1)
                    {
                        tourModel.CQC_FORWARDED_DATE = DateTime.Now;
                        tourModel.CQC_FORWARDED_REMARK = (forwardRemark == null || forwardRemark == string.Empty) ? null : forwardRemark;
                        tourModel.CQC_FINALIZE_OFFICE_ASSISTANT_P_III = officeAssistantName;
                    }

                    dbContext.Entry(tourModel).State = System.Data.Entity.EntityState.Modified;
                    dbContext.SaveChanges();

                    status = 1;

                    scope.Complete();
                }
                if (status > 0)
                {
                    IsValid = "Tour details finalized successfully.";
                    return true;
                }

                IsValid = "Tour details finalized successfully.";
                return true;
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "TourClaimDAL.FinalizeSanctionTourDetailsDAL()");
                IsValid = "Tour details not finalized.";
                return false;
            }
        }

        public bool ApproveTourDetailsDAL(string id, out String IsValid)
        {
            NQM_TOUR_CLAIM_MASTER tourModel = new NQM_TOUR_CLAIM_MASTER();

            int status = 0;
            int scheduleCode = Convert.ToInt32(id.Split('$')[0]);
            string remark = id.Split('$')[1];
            string directorName = id.Split('$')[2];
            status = dbContext.SaveChanges();
            try
            {
                using (var scope = new TransactionScope())
                {
                    int tourClaimId = dbContext.NQM_TOUR_CLAIM_MASTER.Where(x => x.ADMIN_SCHEDULE_CODE == scheduleCode).Select(y => y.TOUR_CLAIM_ID).FirstOrDefault();

                    // finalize tour details 
                    tourModel = dbContext.NQM_TOUR_CLAIM_MASTER.Where(x => x.TOUR_CLAIM_ID == tourClaimId).FirstOrDefault();
                    tourModel.FINALIZE_FLAG = 5;
                    tourModel.CQC_APPROVE_DATE = DateTime.Now;
                    tourModel.CQC_APPROVE_REMARK = (remark == null || remark == string.Empty) ? null : remark;
                    tourModel.CQC_APPROVE_DIRECTOR = directorName;

                    dbContext.Entry(tourModel).State = System.Data.Entity.EntityState.Modified;
                    dbContext.SaveChanges();

                    status = 1;

                    scope.Complete();
                }
                if (status > 0)
                {
                    IsValid = "Tour details approved successfully.";
                    return true;
                }

                IsValid = "Tour details approved successfully.";
                return true;
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "TourClaimDAL.ApproveTourDetailsDAL()");
                IsValid = "Tour details not approved.";
                return false;
            }
        }

        public Array GetTravelClaimListCqcDAL(int scheduleCode, int page, int rows, string sidx, string sord, out long totalRecords)
        {
            try
            {
                var lstExecution = (from travel in dbContext.NQM_TRAVEL_CLAIM_DETAILS
                                    join master in dbContext.NQM_TOUR_CLAIM_MASTER on travel.TOUR_CLAIM_ID equals master.TOUR_CLAIM_ID
                                    join modeOfTravel in dbContext.NQM_TOUR_CLAIM_MODE_OF_TRAVEL on travel.MODE_OF_TRAVEL equals modeOfTravel.ID
                                    where
                                    master.ADMIN_SCHEDULE_CODE == scheduleCode
                                    select new
                                    {
                                        travel.TOUR_CLAIM_ID,
                                        travel.TRAVEL_CLAIM_ID,
                                        master.ADMIN_SCHEDULE_CODE,
                                        travel.START_DATE_OF_TRAVEL,
                                        travel.START_HOURS,
                                        travel.START_MINUTES,
                                        travel.END_DATE_OF_TRAVEL,
                                        travel.END_HOURS,
                                        travel.END_MINUTES,
                                        travel.DEPARTURE_FROM,
                                        travel.ARRIVAL_AT,
                                        modeOfTravel.MODE_OF_TRAVEL,
                                        travel.TRAVEL_CLASS,
                                        travel.AMOUNT_CLAIMED,
                                        travel.AMOUNT_PASSED_CQC,
                                        master.FINALIZE_FLAG,
                                        travel.UPLOADED_TICKET_NAME,
                                        travel.BOARDING_PASS_NAME,
                                        master.ROUNDS_SEQUENCE,
                                        travel.AMOUNT_PASSED_FIN2,
                                        travel.AMOUNT_PASSED_FIN1,
                                        travel.REMARK_FIN1,
                                        travel.REMARK_FIN2,
                                        travel.REMARK_CQC,
                                    }).Distinct();

                totalRecords = lstExecution.Count();

                if (sidx.Trim() != string.Empty)
                {
                    if (sord.ToString() == "asc")
                    {
                        switch (sidx)
                        {
                            case "START_DATE_OF_TRAVEL":
                                lstExecution = lstExecution.OrderBy(m => m.START_DATE_OF_TRAVEL).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                                break;
                            default:
                                lstExecution = lstExecution.OrderBy(m => m.START_DATE_OF_TRAVEL).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                                break;
                        }
                    }
                    else
                    {
                        switch (sidx)
                        {
                            case "START_DATE_OF_TRAVEL":
                                lstExecution = lstExecution.OrderBy(m => m.START_DATE_OF_TRAVEL).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                                break;
                            default:
                                lstExecution = lstExecution.OrderBy(m => m.START_DATE_OF_TRAVEL).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                                break;
                        }
                    }
                }
                else
                {
                    lstExecution = lstExecution.OrderBy(m => m.START_DATE_OF_TRAVEL).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                }

                var result = lstExecution.Select(executionDetails => new
                {
                    executionDetails.TOUR_CLAIM_ID,
                    executionDetails.TRAVEL_CLAIM_ID,
                    executionDetails.ADMIN_SCHEDULE_CODE,
                    executionDetails.START_DATE_OF_TRAVEL,
                    executionDetails.START_HOURS,
                    executionDetails.START_MINUTES,
                    executionDetails.END_DATE_OF_TRAVEL,
                    executionDetails.END_HOURS,
                    executionDetails.END_MINUTES,
                    executionDetails.DEPARTURE_FROM,
                    executionDetails.ARRIVAL_AT,
                    executionDetails.MODE_OF_TRAVEL,
                    executionDetails.TRAVEL_CLASS,
                    executionDetails.AMOUNT_CLAIMED,
                    executionDetails.AMOUNT_PASSED_CQC,
                    executionDetails.FINALIZE_FLAG,
                    executionDetails.UPLOADED_TICKET_NAME,
                    executionDetails.BOARDING_PASS_NAME,
                    executionDetails.ROUNDS_SEQUENCE,
                    executionDetails.AMOUNT_PASSED_FIN2,
                    executionDetails.AMOUNT_PASSED_FIN1,
                    executionDetails.REMARK_FIN1,
                    executionDetails.REMARK_FIN2,
                    executionDetails.REMARK_CQC
                }).ToArray();

                return result.Select(m => new
                {
                    id = m.TOUR_CLAIM_ID.ToString(),
                    cell = new[]
                    {
                        m.TRAVEL_CLAIM_ID.ToString(),
                        m.TOUR_CLAIM_ID.ToString(),
                        m.ADMIN_SCHEDULE_CODE.ToString(),
                        m.START_DATE_OF_TRAVEL.ToShortDateString() + " " + string.Format("{0:00}", m.START_HOURS) + ":" + string.Format("{0:00}", m.START_MINUTES),
                        m.END_DATE_OF_TRAVEL.ToShortDateString() + " " + string.Format("{0:00}", m.END_HOURS) + ":" + string.Format("{0:00}", m.END_MINUTES),
                        m.DEPARTURE_FROM,
                        m.ARRIVAL_AT,
                        m.MODE_OF_TRAVEL,
                        m.TRAVEL_CLASS == null ? "--" : m.TRAVEL_CLASS,
                        m.MODE_OF_TRAVEL == "Own Car" ? "--" : "<a href='/TourClaim/ViewUploadedTravelTicket?id1=" +URLEncrypt.EncryptParameters1(new String[]{"FileName ="+m.UPLOADED_TICKET_NAME})+"' title='Click here to view uploaded file' class='ui-icon ui-icon-search  ui-align-center' target=_blank></a>",
                        m.MODE_OF_TRAVEL == "Flight" ? "<a href='/TourClaim/ViewUploadedBoardingPass?id1=" +URLEncrypt.EncryptParameters1(new String[]{"FileName ="+m.BOARDING_PASS_NAME})+"' title='Click here to view uploaded boarding pass' class='ui-icon 	ui-icon-search  ui-align-center' target=_blank></a>" : "--",
                        m.AMOUNT_CLAIMED.ToString(),
                        (m.FINALIZE_FLAG >= 4 || m.ROUNDS_SEQUENCE == 1) ? m.AMOUNT_PASSED_FIN1.ToString() : "--",
                        (m.FINALIZE_FLAG >= 4 || m.ROUNDS_SEQUENCE == 1) ? m.REMARK_FIN1 : "--",
                        (m.FINALIZE_FLAG >= 4 || m.ROUNDS_SEQUENCE == 1) ? m.AMOUNT_PASSED_FIN2.ToString() : "--",
                        (m.FINALIZE_FLAG >= 4 || m.ROUNDS_SEQUENCE == 1) ? m.REMARK_FIN2 : "--",
                        (m.AMOUNT_PASSED_CQC.ToString() == null || m.AMOUNT_PASSED_CQC.ToString() == string.Empty) ? ("$" + m.FINALIZE_FLAG.ToString()) : (m.AMOUNT_PASSED_CQC.ToString() + "$" + m.FINALIZE_FLAG.ToString()),
                        (m.REMARK_CQC == null || m.REMARK_CQC == string.Empty) ? ("$" + m.FINALIZE_FLAG.ToString()) : (m.REMARK_CQC + "$" + m.FINALIZE_FLAG.ToString()),
                        m.FINALIZE_FLAG.ToString() + "$" + m.AMOUNT_PASSED_CQC.ToString(),
                        m.FINALIZE_FLAG.ToString(),
                    }
                }).ToArray();
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "TourClaimDAL.GetTravelClaimListCqcDAL()");
                totalRecords = 0;
                return null;
            }
            finally
            {
                dbContext.Dispose();
            }
        }

        public Array GetLodgeClaimListCqcDAL(int scheduleCode, int page, int rows, string sidx, string sord, out long totalRecords)
        {
            try
            {
                var lstExecution = (from lodge in dbContext.NQM_LODGE_AND_DAILY_CLAIM_DETAILS
                                    join master in dbContext.NQM_TOUR_CLAIM_MASTER on lodge.TOUR_CLAIM_ID equals master.TOUR_CLAIM_ID
                                    where
                                    master.ADMIN_SCHEDULE_CODE == scheduleCode
                                    select new
                                    {
                                        lodge.LODGE_CLAIM_ID,
                                        lodge.TOUR_CLAIM_ID,
                                        master.ADMIN_SCHEDULE_CODE,
                                        lodge.DATE_FROM,
                                        lodge.DATE_TO,
                                        lodge.TYPE_OF_CLAIM,
                                        lodge.HOTEL_NAME,
                                        lodge.AMOUNT_CLAIMED_DAILY,
                                        lodge.AMOUNT_CLAIMED_HOTEL,
                                        lodge.AMOUNT_PASSED_HOTEL_CQC,
                                        lodge.AMOUNT_PASSED_DAILY,
                                        lodge.DATE_OF_MODIFICATION_CQC,
                                        master.FINALIZE_FLAG,
                                        lodge.UPLOADED_RECEIPT_NAME,
                                        lodge.UPLOADED_BILL_NAME,
                                        lodge.AMOUNT_PASSED_HOTEL_FIN2,
                                        master.ROUNDS_SEQUENCE,
                                        lodge.REMARK_CQC,
                                        lodge.AMOUNT_PASSED_HOTEL_FIN1,
                                        lodge.REMARK_FIN1,
                                        lodge.REMARK_FIN2
                                    }).Distinct();

                totalRecords = lstExecution.Count();

                if (sidx.Trim() != string.Empty)
                {
                    if (sord.ToString() == "asc")
                    {
                        switch (sidx)
                        {
                            case "DATE_FROM":
                                lstExecution = lstExecution.OrderBy(m => m.DATE_FROM).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                                break;
                            default:
                                lstExecution = lstExecution.OrderBy(m => m.DATE_FROM).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                                break;
                        }
                    }
                    else
                    {
                        switch (sidx)
                        {
                            case "DATE_FROM":
                                lstExecution = lstExecution.OrderBy(m => m.DATE_FROM).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                                break;
                            default:
                                lstExecution = lstExecution.OrderBy(m => m.DATE_FROM).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                                break;
                        }
                    }
                }
                else
                {
                    lstExecution = lstExecution.OrderBy(m => m.DATE_FROM).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                }

                var result = lstExecution.Select(executionDetails => new
                {
                    executionDetails.LODGE_CLAIM_ID,
                    executionDetails.TOUR_CLAIM_ID,
                    executionDetails.ADMIN_SCHEDULE_CODE,
                    executionDetails.DATE_FROM,
                    executionDetails.DATE_TO,
                    executionDetails.HOTEL_NAME,
                    executionDetails.TYPE_OF_CLAIM,
                    executionDetails.AMOUNT_CLAIMED_DAILY,
                    executionDetails.AMOUNT_CLAIMED_HOTEL,
                    executionDetails.AMOUNT_PASSED_DAILY,
                    executionDetails.AMOUNT_PASSED_HOTEL_CQC,
                    executionDetails.DATE_OF_MODIFICATION_CQC,
                    executionDetails.FINALIZE_FLAG,
                    executionDetails.UPLOADED_RECEIPT_NAME,
                    executionDetails.UPLOADED_BILL_NAME,
                    executionDetails.AMOUNT_PASSED_HOTEL_FIN2,
                    executionDetails.ROUNDS_SEQUENCE,
                    executionDetails.REMARK_CQC,
                    executionDetails.AMOUNT_PASSED_HOTEL_FIN1,
                    executionDetails.REMARK_FIN1,
                    executionDetails.REMARK_FIN2
                }).ToArray();

                return result.Select(m => new
                {
                    id = m.TOUR_CLAIM_ID.ToString(),
                    cell = new[]
                    {
                        m.LODGE_CLAIM_ID.ToString(),
                        m.TOUR_CLAIM_ID.ToString(),
                        m.ADMIN_SCHEDULE_CODE.ToString(),
                        m.DATE_FROM.ToShortDateString(),
                        m.DATE_TO.ToShortDateString(),
                        (m.TYPE_OF_CLAIM == "H") ? "Hotel Claim" : (m.TYPE_OF_CLAIM == "G") ? "Guest House" : "Self Accomodation",
                        (m.TYPE_OF_CLAIM == "H" || m.TYPE_OF_CLAIM == "G") ? m.HOTEL_NAME : "--",
                        (m.TYPE_OF_CLAIM == "H" || m.TYPE_OF_CLAIM == "G") ? "<a href='/TourClaim/ViewUploadedLodgeBill?id1="+URLEncrypt.EncryptParameters1(new String[]{"FileName ="+m.UPLOADED_BILL_NAME})+"' title='Click here to view uploaded file' class='ui-icon ui-icon-search  ui-align-center' target=_blank></a>" : "--",
                        (m.TYPE_OF_CLAIM == "H") ? "<a href='/TourClaim/ViewUploadedLodgeBill?id1="+URLEncrypt.EncryptParameters1(new String[]{"FileName ="+m.UPLOADED_RECEIPT_NAME})+"' title='Click here to view uploaded file' class='ui-icon 	ui-icon-search  ui-align-center' target=_blank></a>" : "--",
                        m.AMOUNT_CLAIMED_DAILY.ToString(),
                        (m.TYPE_OF_CLAIM == "H" || m.TYPE_OF_CLAIM == "G") ? m.AMOUNT_CLAIMED_HOTEL.ToString() : "--",
                        m.AMOUNT_PASSED_DAILY.ToString(),
                        (m.FINALIZE_FLAG >= 4 || m.ROUNDS_SEQUENCE == 1) ? m.AMOUNT_PASSED_HOTEL_FIN1.ToString() : "--",
                        (m.FINALIZE_FLAG >= 4 || m.ROUNDS_SEQUENCE == 1) ? m.REMARK_FIN1 : "--",
                        (m.FINALIZE_FLAG >= 4 || m.ROUNDS_SEQUENCE == 1) ? m.AMOUNT_PASSED_HOTEL_FIN2.ToString() : "--",
                        (m.FINALIZE_FLAG >= 4 || m.ROUNDS_SEQUENCE == 1) ? m.REMARK_FIN2 : "--",
                        (m.TYPE_OF_CLAIM == "H" || m.TYPE_OF_CLAIM == "G") ? (m.AMOUNT_PASSED_HOTEL_CQC.ToString() == null || m.AMOUNT_PASSED_HOTEL_CQC.ToString() == string.Empty) ? ("$" + m.FINALIZE_FLAG.ToString()) : (m.AMOUNT_PASSED_HOTEL_CQC.ToString() + "$" + m.FINALIZE_FLAG.ToString()) : ("--$" + m.FINALIZE_FLAG.ToString()),
                        (m.REMARK_CQC == null || m.REMARK_CQC == string.Empty) ? ("$" + m.FINALIZE_FLAG.ToString() + "$" + m.TYPE_OF_CLAIM) : (m.REMARK_CQC + "$" + m.FINALIZE_FLAG.ToString() + "$" + m.TYPE_OF_CLAIM),
                        m.FINALIZE_FLAG.ToString() + "$" + m.TYPE_OF_CLAIM + "$" + m.AMOUNT_PASSED_HOTEL_CQC.ToString(),
                        m.FINALIZE_FLAG.ToString()
                    }
                }).ToArray();
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "TourClaimDAL.GetLodgeClaimListCqcDAL()");
                totalRecords = 0;
                return null;
            }
            finally
            {
                dbContext.Dispose();
            }
        }

        public Array GetInspectionHonorariumListCqcDAL(int scheduleCode, int page, int rows, string sidx, string sord, out long totalRecords)
        {
            try
            {
                var lstExecution = (from roadInsp in dbContext.NQM_TOUR_HONORARIUM_INSPECTION_OF_ROADS
                                    join master in dbContext.NQM_TOUR_CLAIM_MASTER on roadInsp.TOUR_CLAIM_ID equals master.TOUR_CLAIM_ID
                                    where
                                    master.ADMIN_SCHEDULE_CODE == scheduleCode
                                    select new
                                    {
                                        roadInsp.HONORARIUM_INSPECTION_ID,
                                        roadInsp.TOUR_CLAIM_ID,
                                        master.ADMIN_SCHEDULE_CODE,
                                        roadInsp.DATE_OF_INSPECTION,
                                        roadInsp.TYPE_OF_WORK,
                                        roadInsp.TYPE_OF_WORK_OTHER,
                                        roadInsp.AMOUNT_CLAIMED,
                                        roadInsp.AMOUNT_PASSED_CQC,
                                        roadInsp.DATE_OF_MODIFICATION_CQC,
                                        master.FINALIZE_FLAG,
                                        roadInsp.AMOUNT_PASSED_FIN2,
                                        master.ROUNDS_SEQUENCE,
                                        roadInsp.REMARK_CQC,
                                        roadInsp.AMOUNT_PASSED_FIN1,
                                        roadInsp.REMARK_FIN1,
                                        roadInsp.REMARK_FIN2
                                    }).Distinct();

                totalRecords = lstExecution.Count();

                if (sidx.Trim() != string.Empty)
                {
                    if (sord.ToString() == "asc")
                    {
                        switch (sidx)
                        {
                            case "DATE_OF_INSPECTION":
                                lstExecution = lstExecution.OrderBy(m => m.DATE_OF_INSPECTION).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                                break;
                            default:
                                lstExecution = lstExecution.OrderBy(m => m.DATE_OF_INSPECTION).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                                break;
                        }
                    }
                    else
                    {
                        switch (sidx)
                        {
                            case "DATE_OF_INSPECTION":
                                lstExecution = lstExecution.OrderBy(m => m.DATE_OF_INSPECTION).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                                break;
                            default:
                                lstExecution = lstExecution.OrderBy(m => m.DATE_OF_INSPECTION).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                                break;
                        }
                    }
                }
                else
                {
                    lstExecution = lstExecution.OrderBy(m => m.DATE_OF_INSPECTION).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                }

                var result = lstExecution.Select(executionDetails => new
                {
                    executionDetails.HONORARIUM_INSPECTION_ID,
                    executionDetails.TOUR_CLAIM_ID,
                    executionDetails.ADMIN_SCHEDULE_CODE,
                    executionDetails.DATE_OF_INSPECTION,
                    executionDetails.TYPE_OF_WORK,
                    executionDetails.TYPE_OF_WORK_OTHER,
                    executionDetails.AMOUNT_CLAIMED,
                    executionDetails.AMOUNT_PASSED_CQC,
                    executionDetails.DATE_OF_MODIFICATION_CQC,
                    executionDetails.FINALIZE_FLAG,
                    executionDetails.ROUNDS_SEQUENCE,
                    executionDetails.AMOUNT_PASSED_FIN2,
                    executionDetails.REMARK_CQC,
                    executionDetails.AMOUNT_PASSED_FIN1,
                    executionDetails.REMARK_FIN1,
                    executionDetails.REMARK_FIN2
                }).ToArray();

                return result.Select(m => new
                {
                    id = m.TOUR_CLAIM_ID.ToString(),
                    cell = new[]
                    {
                        m.HONORARIUM_INSPECTION_ID.ToString(),
                        m.TOUR_CLAIM_ID.ToString(),
                        m.ADMIN_SCHEDULE_CODE.ToString(),
                        m.DATE_OF_INSPECTION.ToShortDateString(),
                        m.TYPE_OF_WORK == "I" ? "Inspection" : m.TYPE_OF_WORK == "E" ? "Enquiry" : m.TYPE_OF_WORK == "A" ? "ATR Verification" : m.TYPE_OF_WORK == "N" ? "Normal" : m.TYPE_OF_WORK == "O" ? m.TYPE_OF_WORK_OTHER : "--",
                        m.AMOUNT_CLAIMED.ToString(),
                        (m.FINALIZE_FLAG >= 4 || m.ROUNDS_SEQUENCE == 1) ? m.AMOUNT_PASSED_FIN1.ToString() : "--",
                        (m.FINALIZE_FLAG >= 4 || m.ROUNDS_SEQUENCE == 1) ? m.REMARK_FIN1 : "--",
                        (m.FINALIZE_FLAG >= 4 || m.ROUNDS_SEQUENCE == 1) ? m.AMOUNT_PASSED_FIN2.ToString() : "--",
                        (m.FINALIZE_FLAG >= 4 || m.ROUNDS_SEQUENCE == 1) ? m.REMARK_FIN2 : "--",
                        (m.AMOUNT_PASSED_CQC.ToString() == null || m.AMOUNT_PASSED_CQC.ToString() == string.Empty) ? ("$" + m.FINALIZE_FLAG.ToString()) : (m.AMOUNT_PASSED_CQC.ToString() + "$" + m.FINALIZE_FLAG.ToString()),
                        (m.REMARK_CQC == null || m.REMARK_CQC == string.Empty) ? ("$" + m.FINALIZE_FLAG.ToString()) : (m.REMARK_CQC + "$" + m.FINALIZE_FLAG.ToString()),
                        m.FINALIZE_FLAG.ToString() + "$" + m.AMOUNT_PASSED_CQC.ToString(),
                    }
                }).ToArray();
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "TourClaimDAL.GetInspectionHonorariumListCqcDAL()");
                totalRecords = 0;
                return null;
            }
            finally
            {
                dbContext.Dispose();
            }
        }

        public Array GetMeetingHonorariumListCqcDAL(int scheduleCode, int page, int rows, string sidx, string sord, out long totalRecords)
        {
            try
            {
                var lstExecution = (from meeting in dbContext.NQM_TOUR_HONORARIUM_MEETING_WITH_PIU
                                    join master in dbContext.NQM_TOUR_CLAIM_MASTER on meeting.TOUR_CLAIM_ID equals master.TOUR_CLAIM_ID
                                    join district in dbContext.MASTER_DISTRICT on meeting.DISTRICT equals district.MAST_DISTRICT_CODE
                                    where
                                    master.ADMIN_SCHEDULE_CODE == scheduleCode
                                    select new
                                    {
                                        meeting.HONORARIUM_MEETING_ID,
                                        meeting.TOUR_CLAIM_ID,
                                        master.ADMIN_SCHEDULE_CODE,
                                        meeting.DATE_OF_MEETING,
                                        district.MAST_DISTRICT_NAME,
                                        meeting.AMOUNT_CLAIMED,
                                        meeting.AMOUNT_PASSED_CQC,
                                        meeting.DATE_OF_MODIFICATION_CQC,
                                        master.FINALIZE_FLAG,
                                        meeting.UPLOADED_FILE_NAME,
                                        meeting.AMOUNT_PASSED_FIN2,
                                        master.ROUNDS_SEQUENCE,
                                        meeting.PHOTO_NAME,
                                        meeting.ATTENDANCE_SHEET_NAME,
                                        meeting.REMARK_CQC,
                                        meeting.AMOUNT_PASSED_FIN1,
                                        meeting.REMARK_FIN1,
                                        meeting.REMARK_FIN2
                                    }).Distinct();

                totalRecords = lstExecution.Count();

                if (sidx.Trim() != string.Empty)
                {
                    if (sord.ToString() == "asc")
                    {
                        switch (sidx)
                        {
                            case "DATE_OF_MEETING":
                                lstExecution = lstExecution.OrderBy(m => m.DATE_OF_MEETING).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                                break;
                            default:
                                lstExecution = lstExecution.OrderBy(m => m.DATE_OF_MEETING).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                                break;
                        }
                    }
                    else
                    {
                        switch (sidx)
                        {
                            case "DATE_OF_MEETING":
                                lstExecution = lstExecution.OrderBy(m => m.DATE_OF_MEETING).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                                break;
                            default:
                                lstExecution = lstExecution.OrderBy(m => m.DATE_OF_MEETING).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                                break;
                        }
                    }
                }
                else
                {
                    lstExecution = lstExecution.OrderBy(m => m.DATE_OF_MEETING).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                }

                var result = lstExecution.Select(executionDetails => new
                {
                    executionDetails.HONORARIUM_MEETING_ID,
                    executionDetails.TOUR_CLAIM_ID,
                    executionDetails.ADMIN_SCHEDULE_CODE,
                    executionDetails.DATE_OF_MEETING,
                    executionDetails.MAST_DISTRICT_NAME,
                    executionDetails.AMOUNT_CLAIMED,
                    executionDetails.AMOUNT_PASSED_CQC,
                    executionDetails.DATE_OF_MODIFICATION_CQC,
                    executionDetails.FINALIZE_FLAG,
                    executionDetails.UPLOADED_FILE_NAME,
                    executionDetails.ROUNDS_SEQUENCE,
                    executionDetails.AMOUNT_PASSED_FIN2,
                    executionDetails.PHOTO_NAME,
                    executionDetails.ATTENDANCE_SHEET_NAME,
                    executionDetails.REMARK_CQC,
                    executionDetails.AMOUNT_PASSED_FIN1,
                    executionDetails.REMARK_FIN1,
                    executionDetails.REMARK_FIN2
                }).ToArray();

                return result.Select(m => new
                {

                    id = m.TOUR_CLAIM_ID.ToString(),
                    cell = new[]
                    {
                        m.HONORARIUM_MEETING_ID.ToString(),
                        m.TOUR_CLAIM_ID.ToString(),
                        m.ADMIN_SCHEDULE_CODE.ToString(),
                        m.DATE_OF_MEETING.ToShortDateString(),
                        m.MAST_DISTRICT_NAME,
                        "<a href='/TourClaim/ViewUploadedMeetingDetails?id1="+URLEncrypt.EncryptParameters1(new String[]{"FileName ="+m.ATTENDANCE_SHEET_NAME})+"' title='Click here to view uploaded file' class='ui-icon 	ui-icon-search  ui-align-center' target=_blank></a>",
                        "<a href='/TourClaim/ViewUploadedMeetingDetails?id1="+URLEncrypt.EncryptParameters1(new String[]{"FileName ="+m.UPLOADED_FILE_NAME})+"' title='Click here to view uploaded file' class='ui-icon 	ui-icon-search  ui-align-center' target=_blank></a>",
                        "<a href='/TourClaim/ViewUploadedMeetingDetails?id1="+URLEncrypt.EncryptParameters1(new String[]{"FileName ="+m.PHOTO_NAME})+"' title='Click here to view uploaded file' class='ui-icon 	ui-icon-search  ui-align-center' target=_blank></a>",
                        m.AMOUNT_CLAIMED.ToString(),
                        (m.FINALIZE_FLAG >= 4 || m.ROUNDS_SEQUENCE == 1) ? m.AMOUNT_PASSED_FIN1.ToString() : "--",
                        (m.FINALIZE_FLAG >= 4 || m.ROUNDS_SEQUENCE == 1) ? m.REMARK_FIN1 : "--",
                        (m.FINALIZE_FLAG >= 4 || m.ROUNDS_SEQUENCE == 1) ? m.AMOUNT_PASSED_FIN2.ToString() : "--",
                        (m.FINALIZE_FLAG >= 4 || m.ROUNDS_SEQUENCE == 1) ? m.REMARK_FIN2 : "--",
                        (m.AMOUNT_PASSED_CQC.ToString() == null || m.AMOUNT_PASSED_CQC.ToString() == string.Empty) ? ("$" + m.FINALIZE_FLAG.ToString()) : (m.AMOUNT_PASSED_CQC.ToString() + "$" + m.FINALIZE_FLAG.ToString()),
                        (m.REMARK_CQC == null || m.REMARK_CQC == string.Empty) ? ("$" + m.FINALIZE_FLAG.ToString()) : (m.REMARK_CQC + "$" + m.FINALIZE_FLAG.ToString()),
                        m.FINALIZE_FLAG.ToString() + "$" + m.AMOUNT_PASSED_CQC.ToString(),
                        m.FINALIZE_FLAG.ToString()
                    }
                }).ToArray();
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "TourClaimDAL.GetMeetingHonorariumListCqcDAL()");
                totalRecords = 0;
                return null;
            }
            finally
            {
                dbContext.Dispose();
            }
        }

        public Array LoadMiscellaneousClaimListCqcDAL(int scheduleCode, int page, int rows, string sidx, string sord, out long totalRecords)
        {
            try
            {
                var lstExecution = (from mis in dbContext.NQM_TOUR_MISCELLANEOUS
                                    join master in dbContext.NQM_TOUR_CLAIM_MASTER on mis.TOUR_CLAIM_ID equals master.TOUR_CLAIM_ID
                                    where
                                    master.ADMIN_SCHEDULE_CODE == scheduleCode
                                    select new
                                    {
                                        mis.MISCELLANEOUS_ID,
                                        mis.TOUR_CLAIM_ID,
                                        master.ADMIN_SCHEDULE_CODE,
                                        mis.DATE,
                                        mis.DESCRIPTION,
                                        mis.AMOUNT_CLAIMED,
                                        mis.AMOUNT_PASSED_CQC,
                                        mis.DATE_OF_MODIFICATION_CQC,
                                        master.FINALIZE_FLAG,
                                        mis.UPLOADED_FILE_NAME,
                                        mis.AMOUNT_PASSED_FIN2,
                                        master.ROUNDS_SEQUENCE,
                                        mis.REMARK_CQC,
                                        mis.AMOUNT_PASSED_FIN1,
                                        mis.REMARK_FIN1,
                                        mis.REMARK_FIN2
                                    }).Distinct();

                totalRecords = lstExecution.Count();

                if (sidx.Trim() != string.Empty)
                {
                    if (sord.ToString() == "asc")
                    {
                        switch (sidx)
                        {
                            case "TOUR_CLAIM_ID":
                                lstExecution = lstExecution.OrderByDescending(m => m.MISCELLANEOUS_ID).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                                break;
                            default:
                                lstExecution = lstExecution.OrderByDescending(m => m.MISCELLANEOUS_ID).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                                break;
                        }
                    }
                    else
                    {
                        switch (sidx)
                        {
                            case "TOUR_CLAIM_ID":
                                lstExecution = lstExecution.OrderByDescending(m => m.MISCELLANEOUS_ID).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                                break;
                            default:
                                lstExecution = lstExecution.OrderByDescending(m => m.MISCELLANEOUS_ID).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                                break;
                        }
                    }
                }
                else
                {
                    lstExecution = lstExecution.OrderBy(m => m.TOUR_CLAIM_ID).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                }

                var result = lstExecution.Select(executionDetails => new
                {
                    executionDetails.MISCELLANEOUS_ID,
                    executionDetails.TOUR_CLAIM_ID,
                    executionDetails.ADMIN_SCHEDULE_CODE,
                    executionDetails.DATE,
                    executionDetails.DESCRIPTION,
                    executionDetails.AMOUNT_CLAIMED,
                    executionDetails.AMOUNT_PASSED_CQC,
                    executionDetails.DATE_OF_MODIFICATION_CQC,
                    executionDetails.FINALIZE_FLAG,
                    executionDetails.UPLOADED_FILE_NAME,
                    executionDetails.ROUNDS_SEQUENCE,
                    executionDetails.AMOUNT_PASSED_FIN2,
                    executionDetails.REMARK_CQC,
                    executionDetails.AMOUNT_PASSED_FIN1,
                    executionDetails.REMARK_FIN1,
                    executionDetails.REMARK_FIN2
                }).ToArray();

                return result.Select(m => new
                {
                    id = m.TOUR_CLAIM_ID.ToString(),
                    cell = new[]
                    {
                        m.MISCELLANEOUS_ID.ToString(),
                        m.TOUR_CLAIM_ID.ToString(),
                        m.ADMIN_SCHEDULE_CODE.ToString(),
                        m.DATE.ToShortDateString(),
                        m.DESCRIPTION,
                        m.UPLOADED_FILE_NAME == null ? "--" : "<a href='/TourClaim/ViewUploadedMisDetails?id1="+URLEncrypt.EncryptParameters1(new String[]{"FileName ="+m.UPLOADED_FILE_NAME})+"' title='Click here to view uploaded file' class='ui-icon 	ui-icon-search  ui-align-center' target=_blank></a>",
                        m.AMOUNT_CLAIMED.ToString(),
                        (m.FINALIZE_FLAG >= 4 || m.ROUNDS_SEQUENCE == 1) ? m.AMOUNT_PASSED_FIN1.ToString() : "--",
                        (m.FINALIZE_FLAG >= 4 || m.ROUNDS_SEQUENCE == 1) ? m.REMARK_FIN1 : "--",
                        (m.FINALIZE_FLAG >= 4 || m.ROUNDS_SEQUENCE == 1) ? m.AMOUNT_PASSED_FIN2.ToString() : "--",
                        (m.FINALIZE_FLAG >= 4 || m.ROUNDS_SEQUENCE == 1) ? m.REMARK_FIN2 : "--",
                        (m.AMOUNT_PASSED_CQC.ToString() == null || m.AMOUNT_PASSED_CQC.ToString() == string.Empty) ? ("$" + m.FINALIZE_FLAG.ToString()) : (m.AMOUNT_PASSED_CQC.ToString() + "$" + m.FINALIZE_FLAG.ToString()),
                        (m.REMARK_CQC == null || m.REMARK_CQC == string.Empty) ? ("$" + m.FINALIZE_FLAG.ToString()) : (m.REMARK_CQC + "$" + m.FINALIZE_FLAG.ToString()),
                        m.FINALIZE_FLAG.ToString() + "$" + m.AMOUNT_PASSED_CQC.ToString(),
                    }
                }).ToArray();
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "TourClaimDAL.LoadMiscellaneousClaimListCqcDAL()");
                totalRecords = 0;
                return null;
            }
            finally
            {
                dbContext.Dispose();
            }
        }

        public Array LoadPermissionClaimListCqcDAL(int scheduleCode, int page, int rows, string sidx, string sord, out long totalRecords)
        {
            try
            {
                var lstExecution = (from per in dbContext.NQM_TOUR_PERMISSION_DETAILS
                                    join master in dbContext.NQM_TOUR_CLAIM_MASTER on per.TOUR_CLAIM_ID equals master.TOUR_CLAIM_ID
                                    where
                                    master.ADMIN_SCHEDULE_CODE == scheduleCode
                                    select new
                                    {
                                        per.ID,
                                        per.TOUR_CLAIM_ID,
                                        master.ADMIN_SCHEDULE_CODE,
                                        per.DATE,
                                        per.REMARK,
                                        per.PERMISSION_DOCUMENT_NAME
                                    }).Distinct();

                totalRecords = lstExecution.Count();

                if (sidx.Trim() != string.Empty)
                {
                    if (sord.ToString() == "asc")
                    {
                        switch (sidx)
                        {
                            case "TOUR_CLAIM_ID":
                                lstExecution = lstExecution.OrderByDescending(m => m.ID).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                                break;
                            default:
                                lstExecution = lstExecution.OrderByDescending(m => m.ID).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                                break;
                        }
                    }
                    else
                    {
                        switch (sidx)
                        {
                            case "TOUR_CLAIM_ID":
                                lstExecution = lstExecution.OrderByDescending(m => m.ID).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                                break;
                            default:
                                lstExecution = lstExecution.OrderByDescending(m => m.ID).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                                break;
                        }
                    }
                }
                else
                {
                    lstExecution = lstExecution.OrderBy(m => m.TOUR_CLAIM_ID).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                }

                var result = lstExecution.Select(executionDetails => new
                {
                    executionDetails.ID,
                    executionDetails.TOUR_CLAIM_ID,
                    executionDetails.ADMIN_SCHEDULE_CODE,
                    executionDetails.DATE,
                    executionDetails.REMARK,
                    executionDetails.PERMISSION_DOCUMENT_NAME,
                }).ToArray();

                return result.Select(m => new
                {
                    id = m.TOUR_CLAIM_ID.ToString(),
                    cell = new[]
                    {
                        m.ID.ToString(),
                        m.TOUR_CLAIM_ID.ToString(),
                        m.ADMIN_SCHEDULE_CODE.ToString(),
                        m.DATE.ToShortDateString(),
                        m.REMARK,
                        m.PERMISSION_DOCUMENT_NAME == null ? "--" : "<a href='/TourClaim/ViewUploadedPermissionDetails?id1="+URLEncrypt.EncryptParameters1(new String[]{"FileName ="+m.PERMISSION_DOCUMENT_NAME})+"' title='Click here to view uploaded file' class='ui-icon 	ui-icon-search  ui-align-center' target=_blank></a>",
                    }
                }).ToArray();
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "TourClaimDAL.LoadPermissionClaimListCqcDAL()");
                totalRecords = 0;
                return null;
            }
            finally
            {
                dbContext.Dispose();
            }
        }

        #endregion

        #region Finance Tour Claim

        public Array GetFinanceMonitorListDAL(int month, int year, int? page, int? rows, string sidx, string sord, out Int32 totalRecords)
        {
            try
            {
                var tourDetails = (from tour in dbContext.NQM_TOUR_CLAIM_MASTER
                                   where
                                   (tour.FINALIZE_FLAG >= 2 || tour.ROUNDS_SEQUENCE == 1)
                                   && tour.MONTH_OF_INSPECTION == month
                                   && tour.YEAR_OF_INSPECTION == year
                                   select new
                                   {
                                       tour.TOUR_CLAIM_ID,
                                   }).Distinct().ToList();

                List<TOUR_CLAIM_CALCULATION_Result> lstExecution = new List<TOUR_CLAIM_CALCULATION_Result>();

                foreach (var item in tourDetails)
                {
                    lstExecution.Add(dbContext.TOUR_CLAIM_CALCULATION(item.TOUR_CLAIM_ID, month, year).FirstOrDefault());
                }

                totalRecords = lstExecution.Count();

                lstExecution = lstExecution.OrderByDescending(m => m.DATE_OF_CLAIM).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();

                return lstExecution.Select(m => new
                {
                    id = m.TOUR_CLAIM_ID.ToString(),
                    cell = new[]
                    {
                        m.TOUR_CLAIM_ID.ToString(),
                        dbContext.MASTER_MONTH.Where(x => x.MAST_MONTH_CODE == m.MONTH_OF_INSPECTION).Select(z => z.MAST_MONTH_FULL_NAME).FirstOrDefault() + " " + m.YEAR_OF_INSPECTION,
                        m.MONITOR_NAME,
                        m.DATE_OF_CLAIM.ToShortDateString(),
                        (m.DISTRICT_VISITED_ALLOWANCE + 200 + m.TOTAL_TRAVEL_CLAIM_AMOUNT + m.TOTAL_LODGE_CLAIM_AMOUNT + m.TOTAL_DAILY_CLAIM_AMOUNT + m.TOTAL_INSPECTION_CLAIM_AMOUNT + m.TOTAL_MEETING_CLAIM_AMOUNT + m.TOTAL_MISCELLANEOUS_CLAIM_AMOUNT).ToString(),
                        (m.DISTRICT_VISITED_ALLOWANCE + 200 + m.TOTAL_TRAVEL_SANCTIONED_AMOUNT + m.TOTAL_LODGE_SANCTIONED_AMOUNT + m.TOTAL_DAILY_SANCTIONED_AMOUNT + m.TOTAL_INSPECTION_SANCTIONED_AMOUNT + m.TOTAL_MEETING_SANCTIONED_AMOUNT + m.TOTAL_MISCELLANEOUS_SANCTIONED_AMOUNT).ToString(),
                        (m.FINALIZE_FLAG == 1 && m.ROUNDS_SEQUENCE == 1)
                        ? "Under process by CQC"
                        : (m.FINALIZE_FLAG == 3)
                        ? "Under process by Finance Approver"
                        : (m.FINALIZE_FLAG == 4)
                        ? "Approved by Finance"
                        : (m.FINALIZE_FLAG == 5)
                        ? "Approved by CQC"
                        : (m.FINALIZE_FLAG == 2 && m.ROUNDS_SEQUENCE == 1)
                        ? "Forwarded by CQC"
                        : "--",
                        (m.FINALIZE_FLAG == 2 && m.ROUNDS_SEQUENCE == 0)
                        ? "<center><table><tr><td  style='border-color:white'><span class='ui-icon ui-icon-pencil' title='Click here to Edit Details' onClick ='ViewEditFinance(" + m.TOUR_CLAIM_ID + ");' ></span></td></tr></table></center>"
                        : (m.FINALIZE_FLAG == 2 && m.ROUNDS_SEQUENCE == 1)
                        ? "<center><table><tr><td  style='border-color:white'><span class='ui-icon ui-icon-arrowthick-1-e' title='Click here to Edit Details' onClick ='ViewEditFinance(" + m.TOUR_CLAIM_ID + ");' ></span></td></tr></table></center>"
                        : (m.FINALIZE_FLAG == 3)
                        ? "<center><table><tr><td  style='border-color:white'><span class='ui-icon ui-icon-zoomin' title='Click here to View Details' onClick ='ViewEditFinance(" + m.TOUR_CLAIM_ID + ");' ></span></td></tr></table></center>"
                        : (m.ROUNDS_SEQUENCE == 1)
                        ? "<center><table><tr><td  style='border-color:white'><span class='ui-icon ui-icon-zoomin' title='Click here to View Details' onClick ='ViewEditFinance(" + m.TOUR_CLAIM_ID + ");' ></span></td></tr></table></center>"
                        : "--",
                    }
                }).ToArray();
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "TourClaimDAL.GetFinanceMonitorListDAL()");
                totalRecords = 0;
                return null;
            }
            finally
            {
                dbContext.Dispose();
            }
        }

        public Array GetTourDistrictListFin1DAL(int scheduleCode, int page, int rows, string sidx, string sord, out long totalRecords)
        {
            try
            {
                var lstExecution = (from item in dbContext.NQM_TOUR_DISTRICT_DETAILS
                                    join master in dbContext.NQM_TOUR_CLAIM_MASTER on item.TOUR_CLAIM_ID equals master.TOUR_CLAIM_ID
                                    join district in dbContext.MASTER_DISTRICT on item.DISTRICT equals district.MAST_DISTRICT_CODE
                                    where
                                    master.ADMIN_SCHEDULE_CODE == scheduleCode
                                    select new
                                    {
                                        item.TOUR_CLAIM_ID,
                                        master.ADMIN_SCHEDULE_CODE,
                                        item.DISTRICT_DETAILS_ID,
                                        district.MAST_DISTRICT_NAME,
                                        item.DATE_FROM,
                                        item.DATE_TO
                                    }).Distinct();

                totalRecords = lstExecution.Count();

                if (sidx.Trim() != string.Empty)
                {
                    if (sord.ToString() == "asc")
                    {
                        switch (sidx)
                        {
                            case "DATE_FROM":
                                lstExecution = lstExecution.OrderBy(m => m.DATE_FROM).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                                break;
                            default:
                                lstExecution = lstExecution.OrderBy(m => m.DATE_FROM).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                                break;
                        }
                    }
                    else
                    {
                        switch (sidx)
                        {
                            case "DATE_FROM":
                                lstExecution = lstExecution.OrderBy(m => m.DATE_FROM).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                                break;
                            default:
                                lstExecution = lstExecution.OrderBy(m => m.DATE_FROM).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                                break;
                        }
                    }
                }
                else
                {
                    lstExecution = lstExecution.OrderBy(m => m.DATE_FROM).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                }

                var result = lstExecution.Select(executionDetails => new
                {
                    executionDetails.TOUR_CLAIM_ID,
                    executionDetails.ADMIN_SCHEDULE_CODE,
                    executionDetails.DISTRICT_DETAILS_ID,
                    executionDetails.MAST_DISTRICT_NAME,
                    executionDetails.DATE_FROM,
                    executionDetails.DATE_TO
                }).ToArray();

                return result.Select(m => new
                {
                    id = m.TOUR_CLAIM_ID.ToString(),
                    cell = new[]
                    {
                        m.TOUR_CLAIM_ID.ToString(),
                        m.MAST_DISTRICT_NAME,
                        m.DATE_FROM.ToShortDateString(),
                        m.DATE_TO.ToShortDateString(),
                        "<center><table><tr><td  style='border-color:white'><span class='ui-icon ui-icon-zoomin' title='Click here to view Quick Response Sheet' onClick ='ViewQuickResponseSheet(\"" + m.DISTRICT_DETAILS_ID + "\");' ></span></td></tr></table></center>",
                    }
                }).ToArray();
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "TourClaimDAL.GetTourDistrictListFin1DAL()");
                totalRecords = 0;
                return null;
            }
            finally
            {
                dbContext.Dispose();
            }
        }

        public Array GetTravelClaimListFin1DAL(int scheduleCode, int page, int rows, string sidx, string sord, out long totalRecords)
        {
            try
            {
                var lstExecution = (from travel in dbContext.NQM_TRAVEL_CLAIM_DETAILS
                                    join master in dbContext.NQM_TOUR_CLAIM_MASTER on travel.TOUR_CLAIM_ID equals master.TOUR_CLAIM_ID
                                    join modeOfTravel in dbContext.NQM_TOUR_CLAIM_MODE_OF_TRAVEL on travel.MODE_OF_TRAVEL equals modeOfTravel.ID
                                    where
                                    master.ADMIN_SCHEDULE_CODE == scheduleCode
                                    select new
                                    {
                                        travel.TOUR_CLAIM_ID,
                                        travel.TRAVEL_CLAIM_ID,
                                        master.ADMIN_SCHEDULE_CODE,
                                        travel.START_DATE_OF_TRAVEL,
                                        travel.START_HOURS,
                                        travel.START_MINUTES,
                                        travel.END_DATE_OF_TRAVEL,
                                        travel.END_HOURS,
                                        travel.END_MINUTES,
                                        travel.DEPARTURE_FROM,
                                        travel.ARRIVAL_AT,
                                        modeOfTravel.MODE_OF_TRAVEL,
                                        travel.TRAVEL_CLASS,
                                        travel.AMOUNT_CLAIMED,
                                        travel.AMOUNT_PASSED_CQC,
                                        travel.AMOUNT_PASSED_FIN1,
                                        travel.AMOUNT_PASSED_FIN2,
                                        master.FINALIZE_FLAG,
                                        travel.UPLOADED_TICKET_NAME,
                                        travel.BOARDING_PASS_NAME,
                                        master.ROUNDS_SEQUENCE,
                                        travel.REMARK_CQC,
                                        travel.REMARK_FIN1,
                                        travel.REMARK_FIN2
                                    }).Distinct();

                totalRecords = lstExecution.Count();

                if (sidx.Trim() != string.Empty)
                {
                    if (sord.ToString() == "asc")
                    {
                        switch (sidx)
                        {
                            case "START_DATE_OF_TRAVEL":
                                lstExecution = lstExecution.OrderBy(m => m.START_DATE_OF_TRAVEL).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                                break;
                            default:
                                lstExecution = lstExecution.OrderBy(m => m.START_DATE_OF_TRAVEL).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                                break;
                        }
                    }
                    else
                    {
                        switch (sidx)
                        {
                            case "START_DATE_OF_TRAVEL":
                                lstExecution = lstExecution.OrderBy(m => m.START_DATE_OF_TRAVEL).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                                break;
                            default:
                                lstExecution = lstExecution.OrderBy(m => m.START_DATE_OF_TRAVEL).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                                break;
                        }
                    }
                }
                else
                {
                    lstExecution = lstExecution.OrderBy(m => m.START_DATE_OF_TRAVEL).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                }

                var result = lstExecution.Select(executionDetails => new
                {
                    executionDetails.TOUR_CLAIM_ID,
                    executionDetails.TRAVEL_CLAIM_ID,
                    executionDetails.ADMIN_SCHEDULE_CODE,
                    executionDetails.START_DATE_OF_TRAVEL,
                    executionDetails.START_HOURS,
                    executionDetails.START_MINUTES,
                    executionDetails.END_DATE_OF_TRAVEL,
                    executionDetails.END_HOURS,
                    executionDetails.END_MINUTES,
                    executionDetails.DEPARTURE_FROM,
                    executionDetails.ARRIVAL_AT,
                    executionDetails.MODE_OF_TRAVEL,
                    executionDetails.TRAVEL_CLASS,
                    executionDetails.AMOUNT_CLAIMED,
                    executionDetails.AMOUNT_PASSED_CQC,
                    executionDetails.AMOUNT_PASSED_FIN1,
                    executionDetails.AMOUNT_PASSED_FIN2,
                    executionDetails.FINALIZE_FLAG,
                    executionDetails.UPLOADED_TICKET_NAME,
                    executionDetails.BOARDING_PASS_NAME,
                    executionDetails.ROUNDS_SEQUENCE,
                    executionDetails.REMARK_CQC,
                    executionDetails.REMARK_FIN1,
                    executionDetails.REMARK_FIN2
                }).ToArray();

                return result.Select(m => new
                {
                    id = m.TOUR_CLAIM_ID.ToString(),
                    cell = new[]
                    {
                        m.TRAVEL_CLAIM_ID.ToString(),
                        m.TOUR_CLAIM_ID.ToString(),
                        m.ADMIN_SCHEDULE_CODE.ToString(),
                        m.START_DATE_OF_TRAVEL.ToShortDateString() + " " + string.Format("{0:00}", m.START_HOURS) + ":" + string.Format("{0:00}", m.START_MINUTES),
                        m.END_DATE_OF_TRAVEL.ToShortDateString() + " " + string.Format("{0:00}", m.END_HOURS) + ":" + string.Format("{0:00}", m.END_MINUTES),
                        m.DEPARTURE_FROM,
                        m.ARRIVAL_AT,
                        m.MODE_OF_TRAVEL,
                        m.TRAVEL_CLASS == null ? "--" : m.TRAVEL_CLASS,
                        m.MODE_OF_TRAVEL == "Own Car" ? "--" : "<a href='/TourClaim/ViewUploadedTravelTicket?id1=" +URLEncrypt.EncryptParameters1(new String[]{"FileName ="+m.UPLOADED_TICKET_NAME})+"' title='Click here to view uploaded file' class='ui-icon ui-icon-search  ui-align-center' target=_blank></a>",
                        m.MODE_OF_TRAVEL == "Flight" ? "<a href='/TourClaim/ViewUploadedBoardingPass?id1=" +URLEncrypt.EncryptParameters1(new String[]{"FileName ="+m.BOARDING_PASS_NAME})+"' title='Click here to view uploaded boarding pass' class='ui-icon 	ui-icon-search  ui-align-center' target=_blank></a>" : "--",
                        m.AMOUNT_CLAIMED.ToString(),
                        m.AMOUNT_PASSED_CQC.ToString(),
                        m.REMARK_CQC == null || m.REMARK_CQC == string.Empty ? "--" : m.REMARK_CQC,
                        (m.FINALIZE_FLAG >= 4 || m.ROUNDS_SEQUENCE == 1) ? m.AMOUNT_PASSED_FIN2.ToString() : "--",
                        (m.FINALIZE_FLAG >= 4 || m.ROUNDS_SEQUENCE == 1) ? m.REMARK_FIN2 : "--",
                        (m.AMOUNT_PASSED_FIN1.ToString() == null || m.AMOUNT_PASSED_FIN1.ToString() == string.Empty) ? ("$" + m.FINALIZE_FLAG.ToString() + "$" + m.ROUNDS_SEQUENCE.ToString()) : (m.AMOUNT_PASSED_FIN1.ToString() + "$" + m.FINALIZE_FLAG.ToString() + "$" + m.ROUNDS_SEQUENCE.ToString()),
                        (m.REMARK_FIN1 == null || m.REMARK_FIN1 == string.Empty) ? ("$" + m.FINALIZE_FLAG.ToString() + "$" + m.ROUNDS_SEQUENCE.ToString()) : (m.REMARK_FIN1 + "$" + m.FINALIZE_FLAG.ToString() + "$" + m.ROUNDS_SEQUENCE.ToString()),
                        m.FINALIZE_FLAG.ToString() + "$" + m.AMOUNT_PASSED_FIN1.ToString() + "$" + m.ROUNDS_SEQUENCE.ToString(),
                    }
                }).ToArray();
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "TourClaimDAL.GetTravelClaimListFin1DAL()");
                totalRecords = 0;
                return null;
            }
            finally
            {
                dbContext.Dispose();
            }
        }

        public Array GetLodgeClaimListFin1DAL(int scheduleCode, int page, int rows, string sidx, string sord, out long totalRecords)
        {
            try
            {
                var lstExecution = (from lodge in dbContext.NQM_LODGE_AND_DAILY_CLAIM_DETAILS
                                    join master in dbContext.NQM_TOUR_CLAIM_MASTER on lodge.TOUR_CLAIM_ID equals master.TOUR_CLAIM_ID
                                    where
                                    master.ADMIN_SCHEDULE_CODE == scheduleCode
                                    select new
                                    {
                                        lodge.LODGE_CLAIM_ID,
                                        lodge.TOUR_CLAIM_ID,
                                        master.ADMIN_SCHEDULE_CODE,
                                        lodge.DATE_FROM,
                                        lodge.DATE_TO,
                                        lodge.TYPE_OF_CLAIM,
                                        lodge.HOTEL_NAME,
                                        lodge.AMOUNT_CLAIMED_DAILY,
                                        lodge.AMOUNT_CLAIMED_HOTEL,
                                        lodge.AMOUNT_PASSED_HOTEL_CQC,
                                        lodge.AMOUNT_PASSED_HOTEL_FIN1,
                                        lodge.AMOUNT_PASSED_HOTEL_FIN2,
                                        lodge.REMARK_CQC,
                                        lodge.REMARK_FIN1,
                                        lodge.REMARK_FIN2,
                                        lodge.AMOUNT_PASSED_DAILY,
                                        lodge.DATE_OF_MODIFICATION_CQC,
                                        master.FINALIZE_FLAG,
                                        lodge.UPLOADED_RECEIPT_NAME,
                                        lodge.UPLOADED_BILL_NAME,
                                        master.ROUNDS_SEQUENCE
                                    }).Distinct();

                totalRecords = lstExecution.Count();

                if (sidx.Trim() != string.Empty)
                {
                    if (sord.ToString() == "asc")
                    {
                        switch (sidx)
                        {
                            case "DATE_FROM":
                                lstExecution = lstExecution.OrderBy(m => m.DATE_FROM).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                                break;
                            default:
                                lstExecution = lstExecution.OrderBy(m => m.DATE_FROM).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                                break;
                        }
                    }
                    else
                    {
                        switch (sidx)
                        {
                            case "DATE_FROM":
                                lstExecution = lstExecution.OrderBy(m => m.DATE_FROM).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                                break;
                            default:
                                lstExecution = lstExecution.OrderBy(m => m.DATE_FROM).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                                break;
                        }
                    }
                }
                else
                {
                    lstExecution = lstExecution.OrderBy(m => m.DATE_FROM).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                }

                var result = lstExecution.Select(executionDetails => new
                {
                    executionDetails.LODGE_CLAIM_ID,
                    executionDetails.TOUR_CLAIM_ID,
                    executionDetails.ADMIN_SCHEDULE_CODE,
                    executionDetails.DATE_FROM,
                    executionDetails.DATE_TO,
                    executionDetails.HOTEL_NAME,
                    executionDetails.TYPE_OF_CLAIM,
                    executionDetails.AMOUNT_CLAIMED_DAILY,
                    executionDetails.AMOUNT_CLAIMED_HOTEL,
                    executionDetails.AMOUNT_PASSED_DAILY,
                    executionDetails.AMOUNT_PASSED_HOTEL_CQC,
                    executionDetails.AMOUNT_PASSED_HOTEL_FIN1,
                    executionDetails.AMOUNT_PASSED_HOTEL_FIN2,
                    executionDetails.REMARK_CQC,
                    executionDetails.REMARK_FIN1,
                    executionDetails.REMARK_FIN2,
                    executionDetails.DATE_OF_MODIFICATION_CQC,
                    executionDetails.FINALIZE_FLAG,
                    executionDetails.UPLOADED_RECEIPT_NAME,
                    executionDetails.UPLOADED_BILL_NAME,
                    executionDetails.ROUNDS_SEQUENCE
                }).ToArray();

                return result.Select(m => new
                {
                    id = m.TOUR_CLAIM_ID.ToString(),
                    cell = new[]
                    {
                        m.LODGE_CLAIM_ID.ToString(),
                        m.TOUR_CLAIM_ID.ToString(),
                        m.ADMIN_SCHEDULE_CODE.ToString(),
                        m.DATE_FROM.ToShortDateString(),
                        m.DATE_TO.ToShortDateString(),
                        (m.TYPE_OF_CLAIM == "H") ? "Hotel Claim" : (m.TYPE_OF_CLAIM == "G") ? "Guest House" : "Self Accomodation",
                        (m.TYPE_OF_CLAIM == "H" || m.TYPE_OF_CLAIM == "G") ? m.HOTEL_NAME : "--",
                        (m.TYPE_OF_CLAIM == "H" || m.TYPE_OF_CLAIM == "G") ? "<a href='/TourClaim/ViewUploadedLodgeBill?id1="+URLEncrypt.EncryptParameters1(new String[]{"FileName ="+m.UPLOADED_BILL_NAME})+"' title='Click here to view uploaded file' class='ui-icon ui-icon-search  ui-align-center' target=_blank></a>" : "--",
                        (m.TYPE_OF_CLAIM == "H") ? "<a href='/TourClaim/ViewUploadedLodgeBill?id1="+URLEncrypt.EncryptParameters1(new String[]{"FileName ="+m.UPLOADED_RECEIPT_NAME})+"' title='Click here to view uploaded file' class='ui-icon 	ui-icon-search  ui-align-center' target=_blank></a>" : "--",
                        m.AMOUNT_CLAIMED_DAILY.ToString(),
                        (m.TYPE_OF_CLAIM == "H" || m.TYPE_OF_CLAIM == "G") ? m.AMOUNT_CLAIMED_HOTEL.ToString() : "--",
                        m.AMOUNT_PASSED_DAILY.ToString(),
                        (m.TYPE_OF_CLAIM == "H" || m.TYPE_OF_CLAIM == "G") ? m.AMOUNT_PASSED_HOTEL_CQC.ToString() : "--",
                        m.REMARK_CQC == null || m.REMARK_CQC == string.Empty ? "--" : m.REMARK_CQC,
                        (m.FINALIZE_FLAG >= 4 || m.ROUNDS_SEQUENCE == 1) ? m.AMOUNT_PASSED_HOTEL_FIN2.ToString() : "--",
                        (m.FINALIZE_FLAG >= 4 || m.ROUNDS_SEQUENCE == 1) ? m.REMARK_FIN2 : "--",
                        (m.TYPE_OF_CLAIM == "H" || m.TYPE_OF_CLAIM == "G") ? (m.AMOUNT_PASSED_HOTEL_FIN1.ToString() == null || m.AMOUNT_PASSED_HOTEL_FIN1.ToString() == string.Empty) ? ("$" + m.FINALIZE_FLAG.ToString() + "$" + m.ROUNDS_SEQUENCE.ToString()) : (m.AMOUNT_PASSED_HOTEL_FIN1.ToString() + "$" + m.FINALIZE_FLAG.ToString() + "$" + m.ROUNDS_SEQUENCE.ToString()) : ("--$" + m.FINALIZE_FLAG.ToString() + "$" + m.ROUNDS_SEQUENCE.ToString()),
                        (m.REMARK_FIN1 == null || m.REMARK_FIN1 == string.Empty) ? ("$" + m.FINALIZE_FLAG.ToString() + "$" + m.ROUNDS_SEQUENCE + "$" + m.TYPE_OF_CLAIM) : (m.REMARK_FIN1 + "$" + m.FINALIZE_FLAG.ToString() + "$" + m.ROUNDS_SEQUENCE + "$" + m.TYPE_OF_CLAIM),
                        m.FINALIZE_FLAG.ToString() + "$" + m.TYPE_OF_CLAIM + "$" + m.AMOUNT_PASSED_HOTEL_FIN1.ToString() + "$" + m.ROUNDS_SEQUENCE.ToString(),
                    }
                }).ToArray();
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "TourClaimDAL.GetLodgeClaimListFin1DAL()");
                totalRecords = 0;
                return null;
            }
            finally
            {
                dbContext.Dispose();
            }
        }

        public Array GetInspectionHonorariumListFin1DAL(int scheduleCode, int page, int rows, string sidx, string sord, out long totalRecords)
        {
            try
            {
                var lstExecution = (from roadInsp in dbContext.NQM_TOUR_HONORARIUM_INSPECTION_OF_ROADS
                                    join master in dbContext.NQM_TOUR_CLAIM_MASTER on roadInsp.TOUR_CLAIM_ID equals master.TOUR_CLAIM_ID
                                    where
                                    master.ADMIN_SCHEDULE_CODE == scheduleCode
                                    select new
                                    {
                                        roadInsp.HONORARIUM_INSPECTION_ID,
                                        roadInsp.TOUR_CLAIM_ID,
                                        master.ADMIN_SCHEDULE_CODE,
                                        roadInsp.DATE_OF_INSPECTION,
                                        roadInsp.TYPE_OF_WORK,
                                        roadInsp.TYPE_OF_WORK_OTHER,
                                        roadInsp.AMOUNT_CLAIMED,
                                        roadInsp.AMOUNT_PASSED_CQC,
                                        roadInsp.AMOUNT_PASSED_FIN1,
                                        roadInsp.AMOUNT_PASSED_FIN2,
                                        roadInsp.REMARK_CQC,
                                        roadInsp.REMARK_FIN1,
                                        roadInsp.REMARK_FIN2,
                                        roadInsp.DATE_OF_MODIFICATION_CQC,
                                        master.FINALIZE_FLAG,
                                        master.ROUNDS_SEQUENCE
                                    }).Distinct();

                totalRecords = lstExecution.Count();

                if (sidx.Trim() != string.Empty)
                {
                    if (sord.ToString() == "asc")
                    {
                        switch (sidx)
                        {
                            case "DATE_OF_INSPECTION":
                                lstExecution = lstExecution.OrderBy(m => m.DATE_OF_INSPECTION).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                                break;
                            default:
                                lstExecution = lstExecution.OrderBy(m => m.DATE_OF_INSPECTION).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                                break;
                        }
                    }
                    else
                    {
                        switch (sidx)
                        {
                            case "DATE_OF_INSPECTION":
                                lstExecution = lstExecution.OrderBy(m => m.DATE_OF_INSPECTION).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                                break;
                            default:
                                lstExecution = lstExecution.OrderBy(m => m.DATE_OF_INSPECTION).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                                break;
                        }
                    }
                }
                else
                {
                    lstExecution = lstExecution.OrderBy(m => m.DATE_OF_INSPECTION).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                }

                var result = lstExecution.Select(executionDetails => new
                {
                    executionDetails.HONORARIUM_INSPECTION_ID,
                    executionDetails.TOUR_CLAIM_ID,
                    executionDetails.ADMIN_SCHEDULE_CODE,
                    executionDetails.DATE_OF_INSPECTION,
                    executionDetails.TYPE_OF_WORK,
                    executionDetails.TYPE_OF_WORK_OTHER,
                    executionDetails.AMOUNT_CLAIMED,
                    executionDetails.AMOUNT_PASSED_CQC,
                    executionDetails.AMOUNT_PASSED_FIN1,
                    executionDetails.AMOUNT_PASSED_FIN2,
                    executionDetails.REMARK_CQC,
                    executionDetails.REMARK_FIN1,
                    executionDetails.REMARK_FIN2,
                    executionDetails.DATE_OF_MODIFICATION_CQC,
                    executionDetails.FINALIZE_FLAG,
                    executionDetails.ROUNDS_SEQUENCE
                }).ToArray();

                return result.Select(m => new
                {
                    id = m.TOUR_CLAIM_ID.ToString(),
                    cell = new[]
                    {
                        m.HONORARIUM_INSPECTION_ID.ToString(),
                        m.TOUR_CLAIM_ID.ToString(),
                        m.ADMIN_SCHEDULE_CODE.ToString(),
                        m.DATE_OF_INSPECTION.ToShortDateString(),
                        m.TYPE_OF_WORK == "I" ? "Inspection" : m.TYPE_OF_WORK == "E" ? "Enquiry" : m.TYPE_OF_WORK == "A" ? "ATR Verification" : m.TYPE_OF_WORK == "N" ? "Normal" : m.TYPE_OF_WORK == "O" ? m.TYPE_OF_WORK_OTHER : "--",
                        m.AMOUNT_CLAIMED.ToString(),
                        m.AMOUNT_PASSED_CQC.ToString(),
                        m.REMARK_CQC == null || m.REMARK_CQC == string.Empty ? "--" : m.REMARK_CQC,
                        (m.FINALIZE_FLAG >= 4 || m.ROUNDS_SEQUENCE == 1) ? m.AMOUNT_PASSED_FIN2.ToString() : "--",
                        (m.FINALIZE_FLAG >= 4 || m.ROUNDS_SEQUENCE == 1) ? m.REMARK_FIN2 : "--",
                        (m.AMOUNT_PASSED_FIN1.ToString() == null || m.AMOUNT_PASSED_FIN1.ToString() == string.Empty) ? ("$" + m.FINALIZE_FLAG.ToString() + "$" + m.ROUNDS_SEQUENCE.ToString()) : (m.AMOUNT_PASSED_FIN1.ToString() + "$" + m.FINALIZE_FLAG.ToString() + "$" + m.ROUNDS_SEQUENCE.ToString()),
                        (m.REMARK_FIN1 == null || m.REMARK_FIN1 == string.Empty) ? ("$" + m.FINALIZE_FLAG.ToString() + "$" + m.ROUNDS_SEQUENCE.ToString()) : (m.REMARK_FIN1 + "$" + m.FINALIZE_FLAG.ToString() + "$" + m.ROUNDS_SEQUENCE.ToString()),
                        m.FINALIZE_FLAG.ToString() + "$" + m.AMOUNT_PASSED_FIN1.ToString() + "$" + m.ROUNDS_SEQUENCE.ToString(),
                    }
                }).ToArray();
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "TourClaimDAL.GetInspectionHonorariumListFin1DAL()");
                totalRecords = 0;
                return null;
            }
            finally
            {
                dbContext.Dispose();
            }
        }

        public Array GetMeetingHonorariumListFin1DAL(int scheduleCode, int page, int rows, string sidx, string sord, out long totalRecords)
        {
            try
            {
                var lstExecution = (from meeting in dbContext.NQM_TOUR_HONORARIUM_MEETING_WITH_PIU
                                    join master in dbContext.NQM_TOUR_CLAIM_MASTER on meeting.TOUR_CLAIM_ID equals master.TOUR_CLAIM_ID
                                    join district in dbContext.MASTER_DISTRICT on meeting.DISTRICT equals district.MAST_DISTRICT_CODE
                                    where
                                    master.ADMIN_SCHEDULE_CODE == scheduleCode
                                    select new
                                    {
                                        meeting.HONORARIUM_MEETING_ID,
                                        meeting.TOUR_CLAIM_ID,
                                        master.ADMIN_SCHEDULE_CODE,
                                        meeting.DATE_OF_MEETING,
                                        district.MAST_DISTRICT_NAME,
                                        meeting.AMOUNT_CLAIMED,
                                        meeting.AMOUNT_PASSED_CQC,
                                        meeting.AMOUNT_PASSED_FIN1,
                                        meeting.AMOUNT_PASSED_FIN2,
                                        meeting.REMARK_CQC,
                                        meeting.REMARK_FIN1,
                                        meeting.REMARK_FIN2,
                                        meeting.DATE_OF_MODIFICATION_CQC,
                                        master.FINALIZE_FLAG,
                                        meeting.UPLOADED_FILE_NAME,
                                        master.ROUNDS_SEQUENCE,
                                        meeting.PHOTO_NAME,
                                        meeting.ATTENDANCE_SHEET_NAME
                                    }).Distinct();

                totalRecords = lstExecution.Count();

                if (sidx.Trim() != string.Empty)
                {
                    if (sord.ToString() == "asc")
                    {
                        switch (sidx)
                        {
                            case "DATE_OF_MEETING":
                                lstExecution = lstExecution.OrderBy(m => m.DATE_OF_MEETING).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                                break;
                            default:
                                lstExecution = lstExecution.OrderBy(m => m.DATE_OF_MEETING).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                                break;
                        }
                    }
                    else
                    {
                        switch (sidx)
                        {
                            case "DATE_OF_MEETING":
                                lstExecution = lstExecution.OrderBy(m => m.DATE_OF_MEETING).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                                break;
                            default:
                                lstExecution = lstExecution.OrderBy(m => m.DATE_OF_MEETING).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                                break;
                        }
                    }
                }
                else
                {
                    lstExecution = lstExecution.OrderBy(m => m.DATE_OF_MEETING).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                }

                var result = lstExecution.Select(executionDetails => new
                {
                    executionDetails.HONORARIUM_MEETING_ID,
                    executionDetails.TOUR_CLAIM_ID,
                    executionDetails.ADMIN_SCHEDULE_CODE,
                    executionDetails.DATE_OF_MEETING,
                    executionDetails.MAST_DISTRICT_NAME,
                    executionDetails.AMOUNT_CLAIMED,
                    executionDetails.AMOUNT_PASSED_CQC,
                    executionDetails.AMOUNT_PASSED_FIN1,
                    executionDetails.AMOUNT_PASSED_FIN2,
                    executionDetails.REMARK_CQC,
                    executionDetails.REMARK_FIN1,
                    executionDetails.REMARK_FIN2,
                    executionDetails.DATE_OF_MODIFICATION_CQC,
                    executionDetails.FINALIZE_FLAG,
                    executionDetails.UPLOADED_FILE_NAME,
                    executionDetails.ROUNDS_SEQUENCE,
                    executionDetails.PHOTO_NAME,
                    executionDetails.ATTENDANCE_SHEET_NAME
                }).ToArray();

                return result.Select(m => new
                {
                    id = m.TOUR_CLAIM_ID.ToString(),
                    cell = new[]
                    {
                        m.HONORARIUM_MEETING_ID.ToString(),
                        m.TOUR_CLAIM_ID.ToString(),
                        m.ADMIN_SCHEDULE_CODE.ToString(),
                        m.DATE_OF_MEETING.ToShortDateString(),
                        m.MAST_DISTRICT_NAME,
                        "<a href='/TourClaim/ViewUploadedMeetingDetails?id1="+URLEncrypt.EncryptParameters1(new String[]{"FileName ="+m.ATTENDANCE_SHEET_NAME})+"' title='Click here to view uploaded file' class='ui-icon 	ui-icon-search  ui-align-center' target=_blank></a>",
                        "<a href='/TourClaim/ViewUploadedMeetingDetails?id1="+URLEncrypt.EncryptParameters1(new String[]{"FileName ="+m.UPLOADED_FILE_NAME})+"' title='Click here to view uploaded file' class='ui-icon 	ui-icon-search  ui-align-center' target=_blank></a>",
                        "<a href='/TourClaim/ViewUploadedMeetingDetails?id1="+URLEncrypt.EncryptParameters1(new String[]{"FileName ="+m.PHOTO_NAME})+"' title='Click here to view uploaded file' class='ui-icon 	ui-icon-search  ui-align-center' target=_blank></a>",
                        m.AMOUNT_CLAIMED.ToString(),
                        m.AMOUNT_PASSED_CQC.ToString(),
                        m.REMARK_CQC == null || m.REMARK_CQC == string.Empty ? "--" : m.REMARK_CQC,
                        (m.FINALIZE_FLAG >= 4 || m.ROUNDS_SEQUENCE == 1) ? m.AMOUNT_PASSED_FIN2.ToString() : "--",
                        (m.FINALIZE_FLAG >= 4 || m.ROUNDS_SEQUENCE == 1) ? m.REMARK_FIN2 : "--",
                        (m.AMOUNT_PASSED_FIN1.ToString() == null || m.AMOUNT_PASSED_FIN1.ToString() == string.Empty) ? ("$" + m.FINALIZE_FLAG.ToString() + "$" + m.ROUNDS_SEQUENCE.ToString()) : (m.AMOUNT_PASSED_FIN1.ToString() + "$" + m.FINALIZE_FLAG.ToString() + "$" + m.ROUNDS_SEQUENCE.ToString()),
                        (m.REMARK_FIN1 == null || m.REMARK_FIN1 == string.Empty) ? ("$" + m.FINALIZE_FLAG.ToString() + "$" + m.ROUNDS_SEQUENCE.ToString()) : (m.REMARK_FIN1 + "$" + m.FINALIZE_FLAG.ToString() + "$" + m.ROUNDS_SEQUENCE.ToString()),
                        m.FINALIZE_FLAG.ToString() + "$" + m.AMOUNT_PASSED_FIN1.ToString() + "$" + m.ROUNDS_SEQUENCE.ToString(),
                    }
                }).ToArray();
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "TourClaimDAL.GetMeetingHonorariumListFin1DAL()");
                totalRecords = 0;
                return null;
            }
            finally
            {
                dbContext.Dispose();
            }
        }

        public Array LoadMiscellaneousClaimListFin1DAL(int scheduleCode, int page, int rows, string sidx, string sord, out long totalRecords)
        {
            try
            {
                var lstExecution = (from mis in dbContext.NQM_TOUR_MISCELLANEOUS
                                    join master in dbContext.NQM_TOUR_CLAIM_MASTER on mis.TOUR_CLAIM_ID equals master.TOUR_CLAIM_ID
                                    where
                                    master.ADMIN_SCHEDULE_CODE == scheduleCode
                                    select new
                                    {
                                        mis.MISCELLANEOUS_ID,
                                        mis.TOUR_CLAIM_ID,
                                        master.ADMIN_SCHEDULE_CODE,
                                        mis.DATE,
                                        mis.DESCRIPTION,
                                        mis.AMOUNT_CLAIMED,
                                        mis.AMOUNT_PASSED_CQC,
                                        mis.AMOUNT_PASSED_FIN1,
                                        mis.AMOUNT_PASSED_FIN2,
                                        mis.REMARK_CQC,
                                        mis.REMARK_FIN1,
                                        mis.REMARK_FIN2,
                                        mis.DATE_OF_MODIFICATION_CQC,
                                        master.FINALIZE_FLAG,
                                        mis.UPLOADED_FILE_NAME,
                                        master.ROUNDS_SEQUENCE
                                    }).Distinct();

                totalRecords = lstExecution.Count();

                if (sidx.Trim() != string.Empty)
                {
                    if (sord.ToString() == "asc")
                    {
                        switch (sidx)
                        {
                            case "TOUR_CLAIM_ID":
                                lstExecution = lstExecution.OrderByDescending(m => m.MISCELLANEOUS_ID).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                                break;
                            default:
                                lstExecution = lstExecution.OrderByDescending(m => m.MISCELLANEOUS_ID).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                                break;
                        }
                    }
                    else
                    {
                        switch (sidx)
                        {
                            case "TOUR_CLAIM_ID":
                                lstExecution = lstExecution.OrderByDescending(m => m.MISCELLANEOUS_ID).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                                break;
                            default:
                                lstExecution = lstExecution.OrderByDescending(m => m.MISCELLANEOUS_ID).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                                break;
                        }
                    }
                }
                else
                {
                    lstExecution = lstExecution.OrderBy(m => m.TOUR_CLAIM_ID).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                }

                var result = lstExecution.Select(executionDetails => new
                {
                    executionDetails.MISCELLANEOUS_ID,
                    executionDetails.TOUR_CLAIM_ID,
                    executionDetails.ADMIN_SCHEDULE_CODE,
                    executionDetails.DATE,
                    executionDetails.DESCRIPTION,
                    executionDetails.AMOUNT_CLAIMED,
                    executionDetails.AMOUNT_PASSED_CQC,
                    executionDetails.AMOUNT_PASSED_FIN1,
                    executionDetails.AMOUNT_PASSED_FIN2,
                    executionDetails.REMARK_CQC,
                    executionDetails.REMARK_FIN1,
                    executionDetails.REMARK_FIN2,
                    executionDetails.DATE_OF_MODIFICATION_CQC,
                    executionDetails.FINALIZE_FLAG,
                    executionDetails.UPLOADED_FILE_NAME,
                    executionDetails.ROUNDS_SEQUENCE
                }).ToArray();

                return result.Select(m => new
                {
                    id = m.TOUR_CLAIM_ID.ToString(),
                    cell = new[]
                    {
                        m.MISCELLANEOUS_ID.ToString(),
                        m.TOUR_CLAIM_ID.ToString(),
                        m.ADMIN_SCHEDULE_CODE.ToString(),
                        m.DATE.ToShortDateString(),
                        m.DESCRIPTION,
                        m.UPLOADED_FILE_NAME == null ? "--" : "<a href='/TourClaim/ViewUploadedMisDetails?id1="+URLEncrypt.EncryptParameters1(new String[]{"FileName ="+m.UPLOADED_FILE_NAME})+"' title='Click here to view uploaded file' class='ui-icon 	ui-icon-search  ui-align-center' target=_blank></a>",
                        m.AMOUNT_CLAIMED.ToString(),
                        m.AMOUNT_PASSED_CQC.ToString(),
                        m.REMARK_CQC == null || m.REMARK_CQC == string.Empty ? "--" : m.REMARK_CQC,
                        (m.FINALIZE_FLAG >= 4 || m.ROUNDS_SEQUENCE == 1) ? m.AMOUNT_PASSED_FIN2.ToString() : "--",
                        (m.FINALIZE_FLAG >= 4 || m.ROUNDS_SEQUENCE == 1) ? m.REMARK_FIN2 : "--",
                        (m.AMOUNT_PASSED_FIN1.ToString() == null || m.AMOUNT_PASSED_FIN1.ToString() == string.Empty) ? ("$" + m.FINALIZE_FLAG.ToString() + "$" + m.ROUNDS_SEQUENCE.ToString()) : (m.AMOUNT_PASSED_FIN1.ToString() + "$" + m.FINALIZE_FLAG.ToString() + "$" + m.ROUNDS_SEQUENCE.ToString()),
                        (m.REMARK_FIN1 == null || m.REMARK_FIN1 == string.Empty) ? ("$" + m.FINALIZE_FLAG.ToString() + "$" + m.ROUNDS_SEQUENCE.ToString()) : (m.REMARK_FIN1 + "$" + m.FINALIZE_FLAG.ToString() + "$" + m.ROUNDS_SEQUENCE.ToString()),
                        m.FINALIZE_FLAG.ToString() + "$" + m.AMOUNT_PASSED_FIN1.ToString() + "$" + m.ROUNDS_SEQUENCE.ToString(),
                    }
                }).ToArray();
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "TourClaimDAL.LoadMiscellaneousClaimListFin1DAL()");
                totalRecords = 0;
                return null;
            }
            finally
            {
                dbContext.Dispose();
            }
        }

        #endregion

        #region Finance 2 Tour Claim

        public Array GetFinance2MonitorListDAL(int month, int year, int? page, int? rows, string sidx, string sord, out Int32 totalRecords)
        {
            try
            {
                var tourDetails = (from tour in dbContext.NQM_TOUR_CLAIM_MASTER
                                   where
                                   (tour.FINALIZE_FLAG >= 3 || tour.ROUNDS_SEQUENCE == 1)
                                   && tour.MONTH_OF_INSPECTION == month
                                   && tour.YEAR_OF_INSPECTION == year
                                   select new
                                   {
                                       tour.TOUR_CLAIM_ID,
                                   }).Distinct().ToList();

                List<TOUR_CLAIM_CALCULATION_Result> lstExecution = new List<TOUR_CLAIM_CALCULATION_Result>();

                foreach (var item in tourDetails)
                {
                    lstExecution.Add(dbContext.TOUR_CLAIM_CALCULATION(item.TOUR_CLAIM_ID, month, year).FirstOrDefault());
                }

                totalRecords = lstExecution.Count();

                lstExecution = lstExecution.OrderByDescending(m => m.DATE_OF_CLAIM).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();

                return lstExecution.Select(m => new
                {
                    id = m.TOUR_CLAIM_ID.ToString(),
                    cell = new[]
                    {
                        m.TOUR_CLAIM_ID.ToString(),
                        dbContext.MASTER_MONTH.Where(x => x.MAST_MONTH_CODE == m.MONTH_OF_INSPECTION).Select(z => z.MAST_MONTH_FULL_NAME).FirstOrDefault() + " " + m.YEAR_OF_INSPECTION,
                        m.MONITOR_NAME,
                        m.DATE_OF_CLAIM.ToShortDateString(),
                        (m.DISTRICT_VISITED_ALLOWANCE + 200 + m.TOTAL_TRAVEL_CLAIM_AMOUNT + m.TOTAL_LODGE_CLAIM_AMOUNT + m.TOTAL_DAILY_CLAIM_AMOUNT + m.TOTAL_INSPECTION_CLAIM_AMOUNT + m.TOTAL_MEETING_CLAIM_AMOUNT + m.TOTAL_MISCELLANEOUS_CLAIM_AMOUNT).ToString(),
                        (m.DISTRICT_VISITED_ALLOWANCE + 200 + m.TOTAL_TRAVEL_SANCTIONED_AMOUNT + m.TOTAL_LODGE_SANCTIONED_AMOUNT + m.TOTAL_DAILY_SANCTIONED_AMOUNT + m.TOTAL_INSPECTION_SANCTIONED_AMOUNT + m.TOTAL_MEETING_SANCTIONED_AMOUNT + m.TOTAL_MISCELLANEOUS_SANCTIONED_AMOUNT).ToString(),
                        (m.DISTRICT_VISITED_ALLOWANCE + 200 + m.TOTAL_TRAVEL_PASSED_AMOUNT_FIN1 + m.TOTAL_LODGE_PASSED_AMOUNT_FIN1 + m.TOTAL_DAILY_SANCTIONED_AMOUNT + m.TOTAL_INSPECTION_PASSED_AMOUNT_FIN1 + m.TOTAL_MEETING_PASSED_AMOUNT_FIN1 + m.TOTAL_MISCELLANEOUS_PASSED_AMOUNT_FIN1).ToString(),
                        (m.FINALIZE_FLAG >= 4 || m.ROUNDS_SEQUENCE == 1)
                        ? "<center><table><tr><td  style='border-color:white'><span class='ui-icon ui-icon-zoomin' title='Click here to View Details' onClick ='ViewEditFinance2(" + m.TOUR_CLAIM_ID + ");' ></span></td></tr></table></center>"
                        : "<center><table><tr><td  style='border-color:white'><span class='ui-icon ui-icon-pencil' title='Click here to Edit Details' onClick ='ViewEditFinance2(" + m.TOUR_CLAIM_ID + ");' ></span></td></tr></table></center>",
                    }
                }).ToArray();
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "TourClaimDAL.GetFinance2MonitorListDAL()");
                totalRecords = 0;
                return null;
            }
            finally
            {
                dbContext.Dispose();
            }
        }

        public Array GetTourDistrictListFin2DAL(int scheduleCode, int page, int rows, string sidx, string sord, out long totalRecords)
        {
            try
            {
                var lstExecution = (from item in dbContext.NQM_TOUR_DISTRICT_DETAILS
                                    join master in dbContext.NQM_TOUR_CLAIM_MASTER on item.TOUR_CLAIM_ID equals master.TOUR_CLAIM_ID
                                    join district in dbContext.MASTER_DISTRICT on item.DISTRICT equals district.MAST_DISTRICT_CODE
                                    where
                                    master.ADMIN_SCHEDULE_CODE == scheduleCode
                                    select new
                                    {
                                        item.TOUR_CLAIM_ID,
                                        master.ADMIN_SCHEDULE_CODE,
                                        item.DISTRICT_DETAILS_ID,
                                        district.MAST_DISTRICT_NAME,
                                        item.DATE_FROM,
                                        item.DATE_TO
                                    }).Distinct();

                totalRecords = lstExecution.Count();

                if (sidx.Trim() != string.Empty)
                {
                    if (sord.ToString() == "asc")
                    {
                        switch (sidx)
                        {
                            case "DATE_FROM":
                                lstExecution = lstExecution.OrderBy(m => m.DATE_FROM).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                                break;
                            default:
                                lstExecution = lstExecution.OrderBy(m => m.DATE_FROM).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                                break;
                        }
                    }
                    else
                    {
                        switch (sidx)
                        {
                            case "DATE_FROM":
                                lstExecution = lstExecution.OrderBy(m => m.DATE_FROM).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                                break;
                            default:
                                lstExecution = lstExecution.OrderBy(m => m.DATE_FROM).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                                break;
                        }
                    }
                }
                else
                {
                    lstExecution = lstExecution.OrderBy(m => m.DATE_FROM).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                }

                var result = lstExecution.Select(executionDetails => new
                {
                    executionDetails.TOUR_CLAIM_ID,
                    executionDetails.ADMIN_SCHEDULE_CODE,
                    executionDetails.DISTRICT_DETAILS_ID,
                    executionDetails.MAST_DISTRICT_NAME,
                    executionDetails.DATE_FROM,
                    executionDetails.DATE_TO
                }).ToArray();

                return result.Select(m => new
                {
                    id = m.TOUR_CLAIM_ID.ToString(),
                    cell = new[]
                    {
                        m.TOUR_CLAIM_ID.ToString(),
                        m.MAST_DISTRICT_NAME,
                        m.DATE_FROM.ToShortDateString(),
                        m.DATE_TO.ToShortDateString(),
                        "<center><table><tr><td  style='border-color:white'><span class='ui-icon ui-icon-zoomin' title='Click here to view Quick Response Sheet' onClick ='ViewQuickResponseSheet(\"" + m.DISTRICT_DETAILS_ID + "\");' ></span></td></tr></table></center>",
                    }
                }).ToArray();
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "TourClaimDAL.GetTourDistrictListFin2DAL()");
                totalRecords = 0;
                return null;
            }
            finally
            {
                dbContext.Dispose();
            }
        }

        public Array GetTravelClaimListFin2DAL(int scheduleCode, int page, int rows, string sidx, string sord, out long totalRecords)
        {
            try
            {
                var lstExecution = (from travel in dbContext.NQM_TRAVEL_CLAIM_DETAILS
                                    join master in dbContext.NQM_TOUR_CLAIM_MASTER on travel.TOUR_CLAIM_ID equals master.TOUR_CLAIM_ID
                                    join modeOfTravel in dbContext.NQM_TOUR_CLAIM_MODE_OF_TRAVEL on travel.MODE_OF_TRAVEL equals modeOfTravel.ID
                                    where
                                    master.ADMIN_SCHEDULE_CODE == scheduleCode
                                    select new
                                    {
                                        travel.TOUR_CLAIM_ID,
                                        travel.TRAVEL_CLAIM_ID,
                                        master.ADMIN_SCHEDULE_CODE,
                                        travel.START_DATE_OF_TRAVEL,
                                        travel.START_HOURS,
                                        travel.START_MINUTES,
                                        travel.END_DATE_OF_TRAVEL,
                                        travel.END_HOURS,
                                        travel.END_MINUTES,
                                        travel.DEPARTURE_FROM,
                                        travel.ARRIVAL_AT,
                                        modeOfTravel.MODE_OF_TRAVEL,
                                        travel.TRAVEL_CLASS,
                                        travel.AMOUNT_CLAIMED,
                                        travel.AMOUNT_PASSED_CQC,
                                        travel.AMOUNT_PASSED_FIN1,
                                        travel.AMOUNT_PASSED_FIN2,
                                        master.FINALIZE_FLAG,
                                        travel.UPLOADED_TICKET_NAME,
                                        travel.BOARDING_PASS_NAME,
                                        master.ROUNDS_SEQUENCE,
                                        travel.REMARK_CQC,
                                        travel.REMARK_FIN1,
                                        travel.REMARK_FIN2
                                    }).Distinct();

                totalRecords = lstExecution.Count();

                if (sidx.Trim() != string.Empty)
                {
                    if (sord.ToString() == "asc")
                    {
                        switch (sidx)
                        {
                            case "START_DATE_OF_TRAVEL":
                                lstExecution = lstExecution.OrderBy(m => m.START_DATE_OF_TRAVEL).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                                break;
                            default:
                                lstExecution = lstExecution.OrderBy(m => m.START_DATE_OF_TRAVEL).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                                break;
                        }
                    }
                    else
                    {
                        switch (sidx)
                        {
                            case "START_DATE_OF_TRAVEL":
                                lstExecution = lstExecution.OrderBy(m => m.START_DATE_OF_TRAVEL).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                                break;
                            default:
                                lstExecution = lstExecution.OrderBy(m => m.START_DATE_OF_TRAVEL).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                                break;
                        }
                    }
                }
                else
                {
                    lstExecution = lstExecution.OrderBy(m => m.START_DATE_OF_TRAVEL).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                }

                var result = lstExecution.Select(executionDetails => new
                {
                    executionDetails.TOUR_CLAIM_ID,
                    executionDetails.TRAVEL_CLAIM_ID,
                    executionDetails.ADMIN_SCHEDULE_CODE,
                    executionDetails.START_DATE_OF_TRAVEL,
                    executionDetails.START_HOURS,
                    executionDetails.START_MINUTES,
                    executionDetails.END_DATE_OF_TRAVEL,
                    executionDetails.END_HOURS,
                    executionDetails.END_MINUTES,
                    executionDetails.DEPARTURE_FROM,
                    executionDetails.ARRIVAL_AT,
                    executionDetails.MODE_OF_TRAVEL,
                    executionDetails.TRAVEL_CLASS,
                    executionDetails.AMOUNT_CLAIMED,
                    executionDetails.AMOUNT_PASSED_CQC,
                    executionDetails.AMOUNT_PASSED_FIN1,
                    executionDetails.AMOUNT_PASSED_FIN2,
                    executionDetails.FINALIZE_FLAG,
                    executionDetails.UPLOADED_TICKET_NAME,
                    executionDetails.BOARDING_PASS_NAME,
                    executionDetails.ROUNDS_SEQUENCE,
                    executionDetails.REMARK_CQC,
                    executionDetails.REMARK_FIN1,
                    executionDetails.REMARK_FIN2
                }).ToArray();

                return result.Select(m => new
                {
                    id = m.TOUR_CLAIM_ID.ToString(),
                    cell = new[]
                    {
                        m.TRAVEL_CLAIM_ID.ToString(),
                        m.TOUR_CLAIM_ID.ToString(),
                        m.ADMIN_SCHEDULE_CODE.ToString(),
                        m.START_DATE_OF_TRAVEL.ToShortDateString() + " " + string.Format("{0:00}", m.START_HOURS) + ":" + string.Format("{0:00}", m.START_MINUTES),
                        m.END_DATE_OF_TRAVEL.ToShortDateString() + " " + string.Format("{0:00}", m.END_HOURS) + ":" + string.Format("{0:00}", m.END_MINUTES),
                        m.DEPARTURE_FROM,
                        m.ARRIVAL_AT,
                        m.MODE_OF_TRAVEL,
                        m.TRAVEL_CLASS == null ? "--" : m.TRAVEL_CLASS,
                        m.MODE_OF_TRAVEL == "Own Car" ? "--" : "<a href='/TourClaim/ViewUploadedTravelTicket?id1=" +URLEncrypt.EncryptParameters1(new String[]{"FileName ="+m.UPLOADED_TICKET_NAME})+"' title='Click here to view uploaded file' class='ui-icon ui-icon-search  ui-align-center' target=_blank></a>",
                        m.MODE_OF_TRAVEL == "Flight" ? "<a href='/TourClaim/ViewUploadedBoardingPass?id1=" +URLEncrypt.EncryptParameters1(new String[]{"FileName ="+m.BOARDING_PASS_NAME})+"' title='Click here to view uploaded boarding pass' class='ui-icon 	ui-icon-search  ui-align-center' target=_blank></a>" : "--",
                        m.AMOUNT_CLAIMED.ToString(),
                        m.AMOUNT_PASSED_CQC.ToString(),
                        m.REMARK_CQC == null || m.REMARK_CQC == string.Empty ? "--" : m.REMARK_CQC,
                        m.AMOUNT_PASSED_FIN1.ToString(),
                        m.REMARK_FIN1 == null || m.REMARK_FIN1 == string.Empty ? "--" : m.REMARK_FIN1,
                        (m.AMOUNT_PASSED_FIN2.ToString() == null || m.AMOUNT_PASSED_FIN2.ToString() == string.Empty) ? ("$" + m.FINALIZE_FLAG.ToString() + "$" + m.ROUNDS_SEQUENCE.ToString()) : (m.AMOUNT_PASSED_FIN2.ToString() + "$" + m.FINALIZE_FLAG.ToString() + "$" + m.ROUNDS_SEQUENCE.ToString()),
                        (m.REMARK_FIN2 == null || m.REMARK_FIN2 == string.Empty) ? ("$" + m.FINALIZE_FLAG.ToString() + "$" + m.ROUNDS_SEQUENCE.ToString()) : (m.REMARK_FIN2 + "$" + m.FINALIZE_FLAG.ToString() + "$" + m.ROUNDS_SEQUENCE.ToString()),
                        m.FINALIZE_FLAG.ToString() + "$" + m.AMOUNT_PASSED_FIN2.ToString() + "$" + m.ROUNDS_SEQUENCE.ToString(),
                    }
                }).ToArray();
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "TourClaimDAL.GetTravelClaimListFin2DAL()");
                totalRecords = 0;
                return null;
            }
            finally
            {
                dbContext.Dispose();
            }
        }

        public Array GetLodgeClaimListFin2DAL(int scheduleCode, int page, int rows, string sidx, string sord, out long totalRecords)
        {
            try
            {
                var lstExecution = (from lodge in dbContext.NQM_LODGE_AND_DAILY_CLAIM_DETAILS
                                    join master in dbContext.NQM_TOUR_CLAIM_MASTER on lodge.TOUR_CLAIM_ID equals master.TOUR_CLAIM_ID
                                    where
                                    master.ADMIN_SCHEDULE_CODE == scheduleCode
                                    select new
                                    {
                                        lodge.LODGE_CLAIM_ID,
                                        lodge.TOUR_CLAIM_ID,
                                        master.ADMIN_SCHEDULE_CODE,
                                        lodge.DATE_FROM,
                                        lodge.DATE_TO,
                                        lodge.TYPE_OF_CLAIM,
                                        lodge.HOTEL_NAME,
                                        lodge.AMOUNT_CLAIMED_DAILY,
                                        lodge.AMOUNT_CLAIMED_HOTEL,
                                        lodge.AMOUNT_PASSED_HOTEL_CQC,
                                        lodge.AMOUNT_PASSED_HOTEL_FIN1,
                                        lodge.AMOUNT_PASSED_HOTEL_FIN2,
                                        lodge.AMOUNT_PASSED_DAILY,
                                        lodge.DATE_OF_MODIFICATION_CQC,
                                        master.FINALIZE_FLAG,
                                        lodge.UPLOADED_RECEIPT_NAME,
                                        lodge.UPLOADED_BILL_NAME,
                                        master.ROUNDS_SEQUENCE,
                                        lodge.REMARK_CQC,
                                        lodge.REMARK_FIN1,
                                        lodge.REMARK_FIN2
                                    }).Distinct();

                totalRecords = lstExecution.Count();

                if (sidx.Trim() != string.Empty)
                {
                    if (sord.ToString() == "asc")
                    {
                        switch (sidx)
                        {
                            case "DATE_FROM":
                                lstExecution = lstExecution.OrderBy(m => m.DATE_FROM).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                                break;
                            default:
                                lstExecution = lstExecution.OrderBy(m => m.DATE_FROM).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                                break;
                        }
                    }
                    else
                    {
                        switch (sidx)
                        {
                            case "DATE_FROM":
                                lstExecution = lstExecution.OrderBy(m => m.DATE_FROM).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                                break;
                            default:
                                lstExecution = lstExecution.OrderBy(m => m.DATE_FROM).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                                break;
                        }
                    }
                }
                else
                {
                    lstExecution = lstExecution.OrderBy(m => m.DATE_FROM).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                }

                var result = lstExecution.Select(executionDetails => new
                {
                    executionDetails.LODGE_CLAIM_ID,
                    executionDetails.TOUR_CLAIM_ID,
                    executionDetails.ADMIN_SCHEDULE_CODE,
                    executionDetails.DATE_FROM,
                    executionDetails.DATE_TO,
                    executionDetails.HOTEL_NAME,
                    executionDetails.TYPE_OF_CLAIM,
                    executionDetails.AMOUNT_CLAIMED_DAILY,
                    executionDetails.AMOUNT_CLAIMED_HOTEL,
                    executionDetails.AMOUNT_PASSED_DAILY,
                    executionDetails.AMOUNT_PASSED_HOTEL_CQC,
                    executionDetails.AMOUNT_PASSED_HOTEL_FIN1,
                    executionDetails.AMOUNT_PASSED_HOTEL_FIN2,
                    executionDetails.DATE_OF_MODIFICATION_CQC,
                    executionDetails.FINALIZE_FLAG,
                    executionDetails.UPLOADED_RECEIPT_NAME,
                    executionDetails.UPLOADED_BILL_NAME,
                    executionDetails.ROUNDS_SEQUENCE,
                    executionDetails.REMARK_CQC,
                    executionDetails.REMARK_FIN1,
                    executionDetails.REMARK_FIN2
                }).ToArray();

                return result.Select(m => new
                {
                    id = m.TOUR_CLAIM_ID.ToString(),
                    cell = new[]
                    {
                        m.LODGE_CLAIM_ID.ToString(),
                        m.TOUR_CLAIM_ID.ToString(),
                        m.ADMIN_SCHEDULE_CODE.ToString(),
                        m.DATE_FROM.ToShortDateString(),
                        m.DATE_TO.ToShortDateString(),
                        (m.TYPE_OF_CLAIM == "H") ? "Hotel Claim" : (m.TYPE_OF_CLAIM == "G") ? "Guest House" : "Self Accomodation",
                        (m.TYPE_OF_CLAIM == "H" || m.TYPE_OF_CLAIM == "G") ? m.HOTEL_NAME : "--",
                        (m.TYPE_OF_CLAIM == "H" || m.TYPE_OF_CLAIM == "G") ? "<a href='/TourClaim/ViewUploadedLodgeBill?id1="+URLEncrypt.EncryptParameters1(new String[]{"FileName ="+m.UPLOADED_BILL_NAME})+"' title='Click here to view uploaded file' class='ui-icon ui-icon-search  ui-align-center' target=_blank></a>" : "--",
                        (m.TYPE_OF_CLAIM == "H") ? "<a href='/TourClaim/ViewUploadedLodgeBill?id1="+URLEncrypt.EncryptParameters1(new String[]{"FileName ="+m.UPLOADED_RECEIPT_NAME})+"' title='Click here to view uploaded file' class='ui-icon 	ui-icon-search  ui-align-center' target=_blank></a>" : "--",
                        m.AMOUNT_CLAIMED_DAILY.ToString(),
                        (m.TYPE_OF_CLAIM == "H" || m.TYPE_OF_CLAIM == "G") ? m.AMOUNT_CLAIMED_HOTEL.ToString() : "--",
                        m.AMOUNT_PASSED_DAILY.ToString(),
                        (m.TYPE_OF_CLAIM == "H" || m.TYPE_OF_CLAIM == "G") ? m.AMOUNT_PASSED_HOTEL_CQC.ToString() : "--",
                        m.REMARK_CQC == null || m.REMARK_CQC == string.Empty ? "--" : m.REMARK_CQC,
                        (m.TYPE_OF_CLAIM == "H" || m.TYPE_OF_CLAIM == "G") ? m.AMOUNT_PASSED_HOTEL_FIN1.ToString() : "--",
                        m.REMARK_FIN1 == null || m.REMARK_FIN1 == string.Empty ? "--" : m.REMARK_FIN1,
                        (m.TYPE_OF_CLAIM == "H" || m.TYPE_OF_CLAIM == "G") ? (m.AMOUNT_PASSED_HOTEL_FIN2.ToString() == null || m.AMOUNT_PASSED_HOTEL_FIN2.ToString() == string.Empty) ? ("$" + m.FINALIZE_FLAG.ToString() + "$" + m.ROUNDS_SEQUENCE.ToString()) : (m.AMOUNT_PASSED_HOTEL_FIN2.ToString() + "$" + m.FINALIZE_FLAG.ToString() + "$" + m.ROUNDS_SEQUENCE.ToString()) : ("--$" + m.FINALIZE_FLAG.ToString() + "$" + m.ROUNDS_SEQUENCE.ToString()),
                        (m.REMARK_FIN2 == null || m.REMARK_FIN2 == string.Empty) ? ("$" + m.FINALIZE_FLAG.ToString() + "$" + m.ROUNDS_SEQUENCE + "$" + m.TYPE_OF_CLAIM) : (m.REMARK_FIN2 + "$" + m.FINALIZE_FLAG.ToString() + "$" + m.ROUNDS_SEQUENCE + "$" + m.TYPE_OF_CLAIM),
                        m.FINALIZE_FLAG.ToString() + "$" + m.TYPE_OF_CLAIM + "$" + m.AMOUNT_PASSED_HOTEL_FIN2.ToString() + "$" + m.ROUNDS_SEQUENCE.ToString(),
                    }
                }).ToArray();
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "TourClaimDAL.GetLodgeClaimListFin2DAL()");
                totalRecords = 0;
                return null;
            }
            finally
            {
                dbContext.Dispose();
            }
        }

        public Array GetInspectionHonorariumListFin2DAL(int scheduleCode, int page, int rows, string sidx, string sord, out long totalRecords)
        {
            try
            {
                var lstExecution = (from roadInsp in dbContext.NQM_TOUR_HONORARIUM_INSPECTION_OF_ROADS
                                    join master in dbContext.NQM_TOUR_CLAIM_MASTER on roadInsp.TOUR_CLAIM_ID equals master.TOUR_CLAIM_ID
                                    where
                                    master.ADMIN_SCHEDULE_CODE == scheduleCode
                                    select new
                                    {
                                        roadInsp.HONORARIUM_INSPECTION_ID,
                                        roadInsp.TOUR_CLAIM_ID,
                                        master.ADMIN_SCHEDULE_CODE,
                                        roadInsp.DATE_OF_INSPECTION,
                                        roadInsp.TYPE_OF_WORK,
                                        roadInsp.TYPE_OF_WORK_OTHER,
                                        roadInsp.AMOUNT_CLAIMED,
                                        roadInsp.AMOUNT_PASSED_CQC,
                                        roadInsp.AMOUNT_PASSED_FIN1,
                                        roadInsp.AMOUNT_PASSED_FIN2,
                                        roadInsp.DATE_OF_MODIFICATION_CQC,
                                        master.FINALIZE_FLAG,
                                        master.ROUNDS_SEQUENCE,
                                        roadInsp.REMARK_CQC,
                                        roadInsp.REMARK_FIN1,
                                        roadInsp.REMARK_FIN2
                                    }).Distinct();

                totalRecords = lstExecution.Count();

                if (sidx.Trim() != string.Empty)
                {
                    if (sord.ToString() == "asc")
                    {
                        switch (sidx)
                        {
                            case "DATE_OF_INSPECTION":
                                lstExecution = lstExecution.OrderBy(m => m.DATE_OF_INSPECTION).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                                break;
                            default:
                                lstExecution = lstExecution.OrderBy(m => m.DATE_OF_INSPECTION).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                                break;
                        }
                    }
                    else
                    {
                        switch (sidx)
                        {
                            case "DATE_OF_INSPECTION":
                                lstExecution = lstExecution.OrderBy(m => m.DATE_OF_INSPECTION).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                                break;
                            default:
                                lstExecution = lstExecution.OrderBy(m => m.DATE_OF_INSPECTION).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                                break;
                        }
                    }
                }
                else
                {
                    lstExecution = lstExecution.OrderBy(m => m.DATE_OF_INSPECTION).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                }

                var result = lstExecution.Select(executionDetails => new
                {
                    executionDetails.HONORARIUM_INSPECTION_ID,
                    executionDetails.TOUR_CLAIM_ID,
                    executionDetails.ADMIN_SCHEDULE_CODE,
                    executionDetails.DATE_OF_INSPECTION,
                    executionDetails.TYPE_OF_WORK,
                    executionDetails.TYPE_OF_WORK_OTHER,
                    executionDetails.AMOUNT_CLAIMED,
                    executionDetails.AMOUNT_PASSED_CQC,
                    executionDetails.AMOUNT_PASSED_FIN1,
                    executionDetails.AMOUNT_PASSED_FIN2,
                    executionDetails.DATE_OF_MODIFICATION_CQC,
                    executionDetails.FINALIZE_FLAG,
                    executionDetails.ROUNDS_SEQUENCE,
                    executionDetails.REMARK_CQC,
                    executionDetails.REMARK_FIN1,
                    executionDetails.REMARK_FIN2
                }).ToArray();

                return result.Select(m => new
                {
                    id = m.TOUR_CLAIM_ID.ToString(),
                    cell = new[]
                    {
                        m.HONORARIUM_INSPECTION_ID.ToString(),
                        m.TOUR_CLAIM_ID.ToString(),
                        m.ADMIN_SCHEDULE_CODE.ToString(),
                        m.DATE_OF_INSPECTION.ToShortDateString(),
                        m.TYPE_OF_WORK == "I" ? "Inspection" : m.TYPE_OF_WORK == "E" ? "Enquiry" : m.TYPE_OF_WORK == "A" ? "ATR Verification" : m.TYPE_OF_WORK == "N" ? "Normal" : m.TYPE_OF_WORK == "O" ? m.TYPE_OF_WORK_OTHER : "--",
                        m.AMOUNT_CLAIMED.ToString(),
                        m.AMOUNT_PASSED_CQC.ToString(),
                        m.REMARK_CQC == null || m.REMARK_CQC == string.Empty ? "--" : m.REMARK_CQC,
                        m.AMOUNT_PASSED_FIN1.ToString(),
                        m.REMARK_FIN1 == null || m.REMARK_FIN1 == string.Empty ? "--" : m.REMARK_FIN1,
                        (m.AMOUNT_PASSED_FIN2.ToString() == null || m.AMOUNT_PASSED_FIN2.ToString() == string.Empty) ? ("$" + m.FINALIZE_FLAG.ToString() + "$" + m.ROUNDS_SEQUENCE.ToString()) : (m.AMOUNT_PASSED_FIN2.ToString() + "$" + m.FINALIZE_FLAG.ToString() + "$" + m.ROUNDS_SEQUENCE.ToString()),
                        (m.REMARK_FIN2 == null || m.REMARK_FIN2 == string.Empty) ? ("$" + m.FINALIZE_FLAG.ToString() + "$" + m.ROUNDS_SEQUENCE.ToString()) : (m.REMARK_FIN2 + "$" + m.FINALIZE_FLAG.ToString() + "$" + m.ROUNDS_SEQUENCE.ToString()),
                        m.FINALIZE_FLAG.ToString() + "$" + m.AMOUNT_PASSED_FIN2.ToString() + "$" + m.ROUNDS_SEQUENCE.ToString(),
                    }
                }).ToArray();
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "TourClaimDAL.GetInspectionHonorariumListFin2DAL()");
                totalRecords = 0;
                return null;
            }
            finally
            {
                dbContext.Dispose();
            }
        }

        public Array GetMeetingHonorariumListFin2DAL(int scheduleCode, int page, int rows, string sidx, string sord, out long totalRecords)
        {
            try
            {
                var lstExecution = (from meeting in dbContext.NQM_TOUR_HONORARIUM_MEETING_WITH_PIU
                                    join master in dbContext.NQM_TOUR_CLAIM_MASTER on meeting.TOUR_CLAIM_ID equals master.TOUR_CLAIM_ID
                                    join district in dbContext.MASTER_DISTRICT on meeting.DISTRICT equals district.MAST_DISTRICT_CODE
                                    where
                                    master.ADMIN_SCHEDULE_CODE == scheduleCode
                                    select new
                                    {
                                        meeting.HONORARIUM_MEETING_ID,
                                        meeting.TOUR_CLAIM_ID,
                                        master.ADMIN_SCHEDULE_CODE,
                                        meeting.DATE_OF_MEETING,
                                        district.MAST_DISTRICT_NAME,
                                        meeting.AMOUNT_CLAIMED,
                                        meeting.AMOUNT_PASSED_CQC,
                                        meeting.AMOUNT_PASSED_FIN1,
                                        meeting.AMOUNT_PASSED_FIN2,
                                        meeting.DATE_OF_MODIFICATION_CQC,
                                        master.FINALIZE_FLAG,
                                        meeting.UPLOADED_FILE_NAME,
                                        master.ROUNDS_SEQUENCE,
                                        meeting.PHOTO_NAME,
                                        meeting.ATTENDANCE_SHEET_NAME,
                                        meeting.REMARK_CQC,
                                        meeting.REMARK_FIN1,
                                        meeting.REMARK_FIN2
                                    }).Distinct();

                totalRecords = lstExecution.Count();

                if (sidx.Trim() != string.Empty)
                {
                    if (sord.ToString() == "asc")
                    {
                        switch (sidx)
                        {
                            case "DATE_OF_MEETING":
                                lstExecution = lstExecution.OrderBy(m => m.DATE_OF_MEETING).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                                break;
                            default:
                                lstExecution = lstExecution.OrderBy(m => m.DATE_OF_MEETING).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                                break;
                        }
                    }
                    else
                    {
                        switch (sidx)
                        {
                            case "DATE_OF_MEETING":
                                lstExecution = lstExecution.OrderBy(m => m.DATE_OF_MEETING).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                                break;
                            default:
                                lstExecution = lstExecution.OrderBy(m => m.DATE_OF_MEETING).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                                break;
                        }
                    }
                }
                else
                {
                    lstExecution = lstExecution.OrderBy(m => m.DATE_OF_MEETING).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                }

                var result = lstExecution.Select(executionDetails => new
                {
                    executionDetails.HONORARIUM_MEETING_ID,
                    executionDetails.TOUR_CLAIM_ID,
                    executionDetails.ADMIN_SCHEDULE_CODE,
                    executionDetails.DATE_OF_MEETING,
                    executionDetails.MAST_DISTRICT_NAME,
                    executionDetails.AMOUNT_CLAIMED,
                    executionDetails.AMOUNT_PASSED_CQC,
                    executionDetails.AMOUNT_PASSED_FIN1,
                    executionDetails.AMOUNT_PASSED_FIN2,
                    executionDetails.DATE_OF_MODIFICATION_CQC,
                    executionDetails.FINALIZE_FLAG,
                    executionDetails.UPLOADED_FILE_NAME,
                    executionDetails.ROUNDS_SEQUENCE,
                    executionDetails.PHOTO_NAME,
                    executionDetails.ATTENDANCE_SHEET_NAME,
                    executionDetails.REMARK_CQC,
                    executionDetails.REMARK_FIN1,
                    executionDetails.REMARK_FIN2
                }).ToArray();

                return result.Select(m => new
                {
                    id = m.TOUR_CLAIM_ID.ToString(),
                    cell = new[]
                    {
                        m.HONORARIUM_MEETING_ID.ToString(),
                        m.TOUR_CLAIM_ID.ToString(),
                        m.ADMIN_SCHEDULE_CODE.ToString(),
                        m.DATE_OF_MEETING.ToShortDateString(),
                        m.MAST_DISTRICT_NAME,
                        "<a href='/TourClaim/ViewUploadedMeetingDetails?id1="+URLEncrypt.EncryptParameters1(new String[]{"FileName ="+m.ATTENDANCE_SHEET_NAME})+"' title='Click here to view uploaded file' class='ui-icon 	ui-icon-search  ui-align-center' target=_blank></a>",
                        "<a href='/TourClaim/ViewUploadedMeetingDetails?id1="+URLEncrypt.EncryptParameters1(new String[]{"FileName ="+m.UPLOADED_FILE_NAME})+"' title='Click here to view uploaded file' class='ui-icon 	ui-icon-search  ui-align-center' target=_blank></a>",
                        "<a href='/TourClaim/ViewUploadedMeetingDetails?id1="+URLEncrypt.EncryptParameters1(new String[]{"FileName ="+m.PHOTO_NAME})+"' title='Click here to view uploaded file' class='ui-icon 	ui-icon-search  ui-align-center' target=_blank></a>",
                        m.AMOUNT_CLAIMED.ToString(),
                        m.AMOUNT_PASSED_CQC.ToString(),
                        m.REMARK_CQC == null || m.REMARK_CQC == string.Empty ? "--" : m.REMARK_CQC,
                        m.AMOUNT_PASSED_FIN1.ToString(),
                        m.REMARK_FIN1 == null || m.REMARK_FIN1 == string.Empty ? "--" : m.REMARK_FIN1,
                        (m.AMOUNT_PASSED_FIN2.ToString() == null || m.AMOUNT_PASSED_FIN2.ToString() == string.Empty) ? ("$" + m.FINALIZE_FLAG.ToString() + "$" + m.ROUNDS_SEQUENCE.ToString()) : (m.AMOUNT_PASSED_FIN2.ToString() + "$" + m.FINALIZE_FLAG.ToString() + "$" + m.ROUNDS_SEQUENCE.ToString()),
                        (m.REMARK_FIN2 == null || m.REMARK_FIN2 == string.Empty) ? ("$" + m.FINALIZE_FLAG.ToString() + "$" + m.ROUNDS_SEQUENCE.ToString()) : (m.REMARK_FIN2 + "$" + m.FINALIZE_FLAG.ToString() + "$" + m.ROUNDS_SEQUENCE.ToString()),
                        m.FINALIZE_FLAG.ToString() + "$" + m.AMOUNT_PASSED_FIN2.ToString() + "$" + m.ROUNDS_SEQUENCE.ToString(),
                    }
                }).ToArray();
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "TourClaimDAL.GetMeetingHonorariumListFin2DAL()");
                totalRecords = 0;
                return null;
            }
            finally
            {
                dbContext.Dispose();
            }
        }

        public Array LoadMiscellaneousClaimListFin2DAL(int scheduleCode, int page, int rows, string sidx, string sord, out long totalRecords)
        {
            try
            {
                var lstExecution = (from mis in dbContext.NQM_TOUR_MISCELLANEOUS
                                    join master in dbContext.NQM_TOUR_CLAIM_MASTER on mis.TOUR_CLAIM_ID equals master.TOUR_CLAIM_ID
                                    where
                                    master.ADMIN_SCHEDULE_CODE == scheduleCode
                                    select new
                                    {
                                        mis.MISCELLANEOUS_ID,
                                        mis.TOUR_CLAIM_ID,
                                        master.ADMIN_SCHEDULE_CODE,
                                        mis.DATE,
                                        mis.DESCRIPTION,
                                        mis.AMOUNT_CLAIMED,
                                        mis.AMOUNT_PASSED_CQC,
                                        mis.AMOUNT_PASSED_FIN1,
                                        mis.AMOUNT_PASSED_FIN2,
                                        mis.DATE_OF_MODIFICATION_CQC,
                                        master.FINALIZE_FLAG,
                                        mis.UPLOADED_FILE_NAME,
                                        master.ROUNDS_SEQUENCE,
                                        mis.REMARK_CQC,
                                        mis.REMARK_FIN1,
                                        mis.REMARK_FIN2
                                    }).Distinct();

                totalRecords = lstExecution.Count();

                if (sidx.Trim() != string.Empty)
                {
                    if (sord.ToString() == "asc")
                    {
                        switch (sidx)
                        {
                            case "TOUR_CLAIM_ID":
                                lstExecution = lstExecution.OrderByDescending(m => m.MISCELLANEOUS_ID).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                                break;
                            default:
                                lstExecution = lstExecution.OrderByDescending(m => m.MISCELLANEOUS_ID).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                                break;
                        }
                    }
                    else
                    {
                        switch (sidx)
                        {
                            case "TOUR_CLAIM_ID":
                                lstExecution = lstExecution.OrderByDescending(m => m.MISCELLANEOUS_ID).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                                break;
                            default:
                                lstExecution = lstExecution.OrderByDescending(m => m.MISCELLANEOUS_ID).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                                break;
                        }
                    }
                }
                else
                {
                    lstExecution = lstExecution.OrderBy(m => m.TOUR_CLAIM_ID).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                }

                var result = lstExecution.Select(executionDetails => new
                {
                    executionDetails.MISCELLANEOUS_ID,
                    executionDetails.TOUR_CLAIM_ID,
                    executionDetails.ADMIN_SCHEDULE_CODE,
                    executionDetails.DATE,
                    executionDetails.DESCRIPTION,
                    executionDetails.AMOUNT_CLAIMED,
                    executionDetails.AMOUNT_PASSED_CQC,
                    executionDetails.AMOUNT_PASSED_FIN1,
                    executionDetails.AMOUNT_PASSED_FIN2,
                    executionDetails.DATE_OF_MODIFICATION_CQC,
                    executionDetails.FINALIZE_FLAG,
                    executionDetails.UPLOADED_FILE_NAME,
                    executionDetails.ROUNDS_SEQUENCE,
                    executionDetails.REMARK_CQC,
                    executionDetails.REMARK_FIN1,
                    executionDetails.REMARK_FIN2
                }).ToArray();

                return result.Select(m => new
                {
                    id = m.TOUR_CLAIM_ID.ToString(),
                    cell = new[]
                    {
                        m.MISCELLANEOUS_ID.ToString(),
                        m.TOUR_CLAIM_ID.ToString(),
                        m.ADMIN_SCHEDULE_CODE.ToString(),
                        m.DATE.ToShortDateString(),
                        m.DESCRIPTION,
                        m.UPLOADED_FILE_NAME == null ? "--" : "<a href='/TourClaim/ViewUploadedMisDetails?id1="+URLEncrypt.EncryptParameters1(new String[]{"FileName ="+m.UPLOADED_FILE_NAME})+"' title='Click here to view uploaded file' class='ui-icon 	ui-icon-search  ui-align-center' target=_blank></a>",
                        m.AMOUNT_CLAIMED.ToString(),
                        m.AMOUNT_PASSED_CQC.ToString(),
                        m.REMARK_CQC == null || m.REMARK_CQC == string.Empty ? "--" : m.REMARK_CQC,
                        m.AMOUNT_PASSED_FIN1.ToString(),
                        m.REMARK_FIN1 == null || m.REMARK_FIN1 == string.Empty ? "--" : m.REMARK_FIN1,
                        (m.AMOUNT_PASSED_FIN2.ToString() == null || m.AMOUNT_PASSED_FIN2.ToString() == string.Empty) ? ("$" + m.FINALIZE_FLAG.ToString() + "$" + m.ROUNDS_SEQUENCE.ToString()) : (m.AMOUNT_PASSED_FIN2.ToString() + "$" + m.FINALIZE_FLAG.ToString() + "$" + m.ROUNDS_SEQUENCE.ToString()),
                        (m.REMARK_FIN2 == null || m.REMARK_FIN2 == string.Empty) ? ("$" + m.FINALIZE_FLAG.ToString() + "$" + m.ROUNDS_SEQUENCE.ToString()) : (m.REMARK_FIN2 + "$" + m.FINALIZE_FLAG.ToString() + "$" + m.ROUNDS_SEQUENCE.ToString()),
                        m.FINALIZE_FLAG.ToString() + "$" + m.AMOUNT_PASSED_FIN2.ToString() + "$" + m.ROUNDS_SEQUENCE.ToString(),
                    }
                }).ToArray();
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "TourClaimDAL.LoadMiscellaneousClaimListFin2DAL()");
                totalRecords = 0;
                return null;
            }
            finally
            {
                dbContext.Dispose();
            }
        }

        #endregion

    }

    public interface ITourClaimDAL
    {
        #region NQM Tour Claim
        Array GetNQMCurrScheduleListDAL(int month, int year, int? page, int? rows, string sidx, string sord, out Int32 totalRecords);
        bool InsertTourClaimDetailsDAL(FormCollection formCollection, out String IsValid);
        bool FinalizeTourDetailDAL(int scheduleCode, out String IsValid);
        bool InsertDistrictDetailsDAL(FormCollection formCollection, out String IsValid);
        Array GetTourDistrictListDAL(int scheduleCode, int page, int rows, string sidx, string sord, out long totalRecords);
        bool InsertTravelClaimDetailsDAL(FormCollection formCollection, HttpPostedFileBase boardingPass, HttpPostedFileBase travelTicket, out String IsValid);
        Array GetTravelClaimListDAL(int scheduleCode, int page, int rows, string sidx, string sord, out long totalRecords);
        bool UpdateTravelClaimDetailsDAL(FormCollection formCollection, HttpPostedFileBase boardingPass, HttpPostedFileBase travelTicket, out String IsValid);
        bool InsertLodgeClaimDetailsDAL(FormCollection formCollection, HttpPostedFileBase bill, HttpPostedFileBase receipt, HttpPostedFileBase gBill, out String IsValid);
        Array GetLodgeClaimListDAL(int scheduleCode, int page, int rows, string sidx, string sord, out long totalRecords);
        bool UpdateLodgeClaimDetailsDAL(FormCollection formCollection, HttpPostedFileBase bill, HttpPostedFileBase receipt, HttpPostedFileBase gBill, out String IsValid);
        bool InsertInspectionHonorariumDAL(FormCollection formCollection, out String IsValid);
        Array GetInspectionHonorariumListDAL(int scheduleCode, int page, int rows, string sidx, string sord, out long totalRecords);
        bool UpdateInspectionHonorariumDAL(FormCollection formCollection, out String IsValid);
        Array GetMeetingHonorariumListDAL(int scheduleCode, int page, int rows, string sidx, string sord, out long totalRecords);
        bool InsertMeetingHonorariumDAL(FormCollection formCollection, HttpPostedFileBase postedBgFile, HttpPostedFileBase postedBgFile1, HttpPostedFileBase postedBgFile2, out String IsValid);
        bool UpdateMeetingHonorariumDAL(FormCollection formCollection, HttpPostedFileBase postedBgFile, HttpPostedFileBase postedBgFile1, HttpPostedFileBase postedBgFile2, out String IsValid);
        bool InsertMiscellaneousClaimDetailsDAL(FormCollection formCollection, HttpPostedFileBase postedBgFile, out String IsValid);
        Array GetMiscellaneousClaimListDAL(int scheduleCode, int page, int rows, string sidx, string sord, out long totalRecords);
        bool UpdateMiscellaneousClaimDetailsDAL(FormCollection formCollection, HttpPostedFileBase postedBgFile, out String IsValid);
        bool InsertPermissionClaimDetailsDAL(FormCollection formCollection, HttpPostedFileBase postedBgFile, out String IsValid);
        Array GetPermissionClaimListDAL(int scheduleCode, int page, int rows, string sidx, string sord, out long totalRecords);
        bool UpdatePermissionClaimDetailsDAL(FormCollection formCollection, HttpPostedFileBase postedBgFile, out String IsValid);

        #endregion

        #region CQC Tour Claim

        Array GetTourDistrictListCqcDAL(int scheduleCode, int page, int rows, string sidx, string sord, out long totalRecords);
        Array GetTravelClaimListCqcDAL(int scheduleCode, int page, int rows, string sidx, string sord, out long totalRecords);
        Array GetLodgeClaimListCqcDAL(int scheduleCode, int page, int rows, string sidx, string sord, out long totalRecords);
        Array GetInspectionHonorariumListCqcDAL(int scheduleCode, int page, int rows, string sidx, string sord, out long totalRecords);
        Array GetMeetingHonorariumListCqcDAL(int scheduleCode, int page, int rows, string sidx, string sord, out long totalRecords);
        bool FinalizeSanctionTourDetailsDAL(string id, out String IsValid);
        bool ApproveTourDetailsDAL(string id, out String IsValid);
        Array LoadMiscellaneousClaimListCqcDAL(int scheduleCode, int page, int rows, string sidx, string sord, out long totalRecords);
        Array LoadPermissionClaimListCqcDAL(int scheduleCode, int page, int rows, string sidx, string sord, out long totalRecords);
        #endregion

        #region Finance Tour Claim

        Array GetFinanceMonitorListDAL(int month, int year, int? page, int? rows, string sidx, string sord, out Int32 totalRecords);
        Array GetTourDistrictListFin1DAL(int scheduleCode, int page, int rows, string sidx, string sord, out long totalRecords);
        Array GetTravelClaimListFin1DAL(int scheduleCode, int page, int rows, string sidx, string sord, out long totalRecords);
        Array GetLodgeClaimListFin1DAL(int scheduleCode, int page, int rows, string sidx, string sord, out long totalRecords);
        Array GetInspectionHonorariumListFin1DAL(int scheduleCode, int page, int rows, string sidx, string sord, out long totalRecords);
        Array GetMeetingHonorariumListFin1DAL(int scheduleCode, int page, int rows, string sidx, string sord, out long totalRecords);
        Array LoadMiscellaneousClaimListFin1DAL(int scheduleCode, int page, int rows, string sidx, string sord, out long totalRecords);

        #endregion

        #region Finance 2 Tour Claim

        Array GetFinance2MonitorListDAL(int month, int year, int? page, int? rows, string sidx, string sord, out Int32 totalRecords);
        Array GetTourDistrictListFin2DAL(int scheduleCode, int page, int rows, string sidx, string sord, out long totalRecords);
        Array GetTravelClaimListFin2DAL(int scheduleCode, int page, int rows, string sidx, string sord, out long totalRecords);
        Array GetLodgeClaimListFin2DAL(int scheduleCode, int page, int rows, string sidx, string sord, out long totalRecords);
        Array GetInspectionHonorariumListFin2DAL(int scheduleCode, int page, int rows, string sidx, string sord, out long totalRecords);
        Array GetMeetingHonorariumListFin2DAL(int scheduleCode, int page, int rows, string sidx, string sord, out long totalRecords);
        Array LoadMiscellaneousClaimListFin2DAL(int scheduleCode, int page, int rows, string sidx, string sord, out long totalRecords);

        #endregion
    }
}