using PMGSY.Common;
using PMGSY.Controllers;
using PMGSY.Extensions;
using PMGSY.Models;
using PMGSY.Models.Ticket;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Transactions;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace PMGSY.DAL.Ticket
{
    public interface ITicketDAL
    {
        Array GetTicketListDAL(int? page, int? rows, String sidx, String sord, out long totalrecords, string filters);
        Array GetAllTicketListDAL(int? page, int? rows, String sidx, String sord, out long totalrecords, string filters);

        List<SelectListItem> GetCategory(bool disposing=false);
        List<SelectListItem> GetModule();
        string GetUserRoleInfo();

        Boolean SaveTicketDetailsDAL(TicketViewModel model);
        Boolean SaveTicketFile(int ticketNo, HttpPostedFileBase file, out String message);
        Array GetTicketFileListDAL(int? page, int? rows, String sidx, String sord, int tktno, out long totalrecords);
        TicketAcceptModel GetTicketAcceptDetailDAL(int ticketNo,out Boolean IsApproved);
        TicketReplyModel GetTicketReplyDetailDAL(int ticketNo);
        bool SaveApproveDetailsDAL(TicketAcceptModel model,out string message);
        Boolean SaveTicketReplyDetailsDAL(TicketReplyModel model,out string message);

        void GetReportStatisticsDAL(TicketReportViewModel model);
        Array GetTicketReportListDAL(int? page, int? rows, string sidx, string sord, out long totalrecords, string filters, string Level, String Designation, String Category, String Module,String State);
       
        // added by rohit borse on 14-07-2022
        string GetForwardToDetailsDAL(int moduleId);
    }





    public class TicketingDAL : ITicketDAL
    {
        PMGSYEntities dbContext = null;
         #region common method
        private String GetTicketStatus(int StatusCode)
        {
            switch (StatusCode)
            {
                case 1: return "Opened";
                case 2: return "In Progress";
                case 3: return "Partial Closed";
                case 4: return "Closed";
                default:
                    return "Opened";
            }
        }

        // changed by rohit borse on 20-07-2022
        private List<SelectListItem> GetForwardingAuthority()
        {
            dbContext = new PMGSYEntities();

            List<SelectListItem> AuthorityList = new List<SelectListItem>();
            //AuthorityList.Add(new SelectListItem { Value = "0", Text = "Select user", Selected = true });
            //AuthorityList.Add(new SelectListItem { Value = "9", Text = "CQC" });
            //AuthorityList.Add(new SelectListItem { Value = "14", Text = "Director F&A" });
            //AuthorityList.Add(new SelectListItem { Value = "16", Text = "Technical Director" });
            //AuthorityList.Add(new SelectListItem { Value = "68", Text = "Ommas Team" });
            //AuthorityList.Add(new SelectListItem { Value = "67", Text = "Director  Progress" });
            //AuthorityList.Add(new SelectListItem { Value = "39", Text = "NRRDA(Ticket Admin)" });
            //AuthorityList.Add(new SelectListItem { Value = "99", Text = "Ticket Generator" });
            //return AuthorityList;
                        
            //New Code
            List<int?> ListRoleId = dbContext.TKT_ROLE_TYPE_FORWARDING.Where(s=>s.ROLE_ID == PMGSYSession.Current.RoleCode).Select(s => s.FORWARD_TO_ROLE_ID).Distinct().ToList();
            
            AuthorityList = new SelectList(dbContext.UM_Role_Master
                        .Where(x => ListRoleId.Contains(x.RoleID))
                        .Select(x => new { x.RoleID, x.RoleName })
                        .ToList(), "RoleId", "RoleName").ToList<SelectListItem>();
            
            foreach (var item in AuthorityList)
            {
                if (item.Value == "39")
                {
                    item.Text = "Ticket Admin";
                }

                if (item.Value == "25")
                {
                    item.Text = "Director IT";
                }
            }

            AuthorityList.Add(new SelectListItem { Value = "-2", Text = "Select user" });

            return AuthorityList;
        }

        private string GetStatus(int StatusCode)
        {
            switch (StatusCode)
	        {
                case 1: return "Opened"; break;
                case 2: return "In-Progress"; break;
                case 3: return "Partially closed"; break;
                case 4: return "Closed"; break;
                default: return "Not Opened";
	        }
        }

     #endregion

        public Array GetTicketListDAL(int? page, int? rows, string sidx, string sord, out long totalrecords, string filters)
        {
            dbContext = new PMGSYEntities();
            SearchJsonString test = new SearchJsonString();
            string state = string.Empty;
            //List<TKT_TICKET_MASTER> lstAllrecords = null;
            List<USP_TKT_GET_PENDING_TKTLIST_SELFLEVEL_Result> lstAllrecords = null;
            try
            {
                if (filters != null)
                {
                   var js = new JavaScriptSerializer();
                    test = js.Deserialize<SearchJsonString>(filters);

                    foreach (SearchRules item in test.rules)
                    {
                        switch (item.field)
                        {
                            case "State": state = item.data;
                                break;
                            default:
                                break;
                        }
                    }
                }

                #region OLD CODE
                /*
                 
                           lstAllrecords = dbContext.TKT_TICKET_MASTER.OrderByDescending(s=>s.REPORTED_DATE_TIME).ToList();

                if (PMGSYSession.Current.RoleCode == 39 && PMGSYSession.Current.UserName == "ticketadmin")  //NRDA Appoving authority
                  {
                      lstAllrecords = lstAllrecords.Where(p => !dbContext.TKT_TICKET_APPROVAL.Any(p2 => p2.TICKET_ID == p.TICKET_ID)).ToList();
                      //lstAllrecords.AddRange(dbContext.TKT_TICKET_MASTER.Where(s => s.REPORTED_USERID == PMGSYSession.Current.UserId)); //entered by approve authority
                  }
                    else if (PMGSYSession.Current.RoleCode == 9 || PMGSYSession.Current.RoleCode == 14 || PMGSYSession.Current.RoleCode == 16 || PMGSYSession.Current.RoleCode == 25 || PMGSYSession.Current.RoleCode == 66 || PMGSYSession.Current.RoleCode == 67)  //Replying Autority[CQC, Director F&A, Techical Director,ommasteam/mord,ommasteam/mord,director progress],
                  {

                      lstAllrecords = (from master in lstAllrecords
                                       join approve in dbContext.TKT_TICKET_APPROVAL.ToList()
                                       on master.TICKET_ID equals approve.TICKET_ID
                                       where approve.PENDING_AT_ROLEID == PMGSYSession.Current.RoleCode && approve.STATUS !=4 //closed
                                       select master).ToList();  //pending at user
                  }
                  else
                  {
                      //lstAllrecords = lstAllrecords.ToList();
                     lstAllrecords = lstAllrecords.Where(s => s.REPORTED_USERID == PMGSYSession.Current.UserId).ToList();
                  }


                totalrecords = lstAllrecords.Count;

                if (sidx.Trim() != String.Empty)
                {
                    if (sord == "asc")
                    {
                        switch (sidx)
                        {
                            case "TicketNo":
                                lstAllrecords = lstAllrecords.OrderBy(s => s.TICKET_ID).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                                break;
                            default:
                                lstAllrecords = lstAllrecords.OrderBy(s => s.TICKET_ID).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                                break;
                        }
                    }
                    else
                    {
                        switch (sidx)
                        {
                            case "TicketNo":
                                lstAllrecords = lstAllrecords.OrderByDescending(s => s.TICKET_ID).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                                break;
                            default:
                                lstAllrecords = lstAllrecords.OrderByDescending(s => s.TICKET_ID).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                                break;
                        }
                    }
                }

                var result = lstAllrecords.Select(x => new
                {     
                    TicketNo = x.TICKET_ID,
                    State = dbContext.UM_User_Master.FirstOrDefault(s => s.UserID == x.REPORTED_USERID).Mast_State_Code == null ? -1 : dbContext.UM_User_Master.FirstOrDefault(s => s.UserID == x.REPORTED_USERID).Mast_State_Code.Value,
                    RoleCode = dbContext.UM_User_Master.FirstOrDefault(s => s.UserID == x.REPORTED_USERID).DefaultRoleID,
                    Category = x.MASTER_TKT_CATEGORY.MAST_CATEGORY_NAME,
                    Subject = x.REPORTED_SUBJECT,
                    ReportedBy = x.REPORTED_BY,
                    ReportedDate = x.REPORTED_DATE_TIME,
                    Status = dbContext.TKT_TICKET_APPROVAL.FirstOrDefault(app => app.TICKET_ID == x.TICKET_ID) == null ? -1 : dbContext.TKT_TICKET_APPROVAL.FirstOrDefault(p => p.TICKET_ID == x.TICKET_ID).STATUS,
                    //PendingAt = dbContext.TKT_TICKET_APPROVAL.FirstOrDefault(app => app.TICKET_ID == x.TICKET_ID) == null ? -1 : dbContext.TKT_TICKET_APPROVAL.FirstOrDefault(p => p.TICKET_ID == x.TICKET_ID).UM_User_Master1.DefaultRoleID,
                    PendingAt = dbContext.TKT_TICKET_APPROVAL.FirstOrDefault(app => app.TICKET_ID == x.TICKET_ID) == null ? -1 : dbContext.TKT_TICKET_APPROVAL.FirstOrDefault(p => p.TICKET_ID == x.TICKET_ID).PENDING_AT_ROLEID??-1,
                    FirstForwardTo = dbContext.TKT_TICKET_DETAIL.FirstOrDefault(t => t.TICKET_ID == x.TICKET_ID) == null ? -1 : dbContext.TKT_TICKET_DETAIL.FirstOrDefault(t => t.TICKET_ID == x.TICKET_ID).FORWARDED_TO_ROLEID??-1,
                    Forwarddate = dbContext.TKT_TICKET_DETAIL.FirstOrDefault(t => t.TICKET_ID == x.TICKET_ID) == null ? null : dbContext.TKT_TICKET_DETAIL.FirstOrDefault(t => t.TICKET_ID == x.TICKET_ID).FORWARDED_DATE_TIME,
                    ClosingDate = dbContext.TKT_TICKET_APPROVAL.FirstOrDefault(app => app.TICKET_ID == x.TICKET_ID) == null ? null : dbContext.TKT_TICKET_APPROVAL.FirstOrDefault(app => app.TICKET_ID == x.TICKET_ID).CLOSED_DATE_TIME
                }).ToList();

                var FinalResult = result.Select(x => new
                  {
                      TicketNo = x.TicketNo,
                      State = x.State,
                      StateName =x.State!= -1? dbContext.MASTER_STATE.FirstOrDefault(st => st.MAST_STATE_CODE == x.State).MAST_STATE_NAME : "-",
                      RoleName = dbContext.UM_Role_Master.FirstOrDefault(s=>s.RoleID==x.RoleCode).RoleName,
                      Category = x.Category,
                      Subject = x.Subject,
                      ReportedBy = x.ReportedBy,
                      ReportedDate = x.ReportedDate,
                      Status = x.Status,
                      PendingAt = x.PendingAt,
                      FirstForwardTo = x.FirstForwardTo,
                      Forwarddate =x.Forwarddate,
                      ClosingDate = x.ClosingDate
                  }).ToList();

                if (state != string.Empty) //filtering logic
                    FinalResult = FinalResult.Where(f => f.StateName.ToLower().Contains(state.ToLower())).ToList();

              return FinalResult.Select(s => new
                 {
                    id = s.TicketNo,
                    cell = new[]
                   {  
                    s.TicketNo.ToString(),
                    s.StateName,
                    s.Category,
                    s.Subject,
                    s.ReportedBy+"("+s.RoleName+")",
                    s.ReportedDate.ToString("dd/MM/yyyy"),
                    s.Status == -1?"-":GetTicketStatus(s.Status),
                    s.PendingAt==-1?"-":dbContext.UM_Role_Master.FirstOrDefault(x=>x.RoleID==s.PendingAt).RoleName,
                    s.FirstForwardTo == -1 ? "-" :dbContext.UM_Role_Master.FirstOrDefault(x=>x.RoleID==s.FirstForwardTo).RoleName,
                    s.Forwarddate==null ? "-" : s.Forwarddate.Value.ToString("dd/MM/yyyy"),
                    s.ClosingDate==null ? "-" : s.ClosingDate.Value.ToString("dd/MM/yyyy"),
                    //dbContext.TKT_TICKET_FILES.Any(x=>x.TICKET_ID==s.TicketNo)?"<center><a href='#' class='ui-icon ui-icon-locked  ui-align-center'</a></center>": "<center><a href='#' class='ui-icon ui-icon-arrowthickstop-1-n  ui-align-center' onclick='UploadFile(\"" +URLEncrypt.EncryptParameters1(new String[]{ "TicketNo=" +s.TicketNo })+ "\");return false;'>Upload File</a></center>",
                    dbContext.TKT_TICKET_APPROVAL.Any(f=>f.TICKET_ID==s.TicketNo ) ?"<center><a href='#' class='ui-icon ui-icon-locked  ui-align-center'>Locked</a></center>": "<center><a href='#' class='ui-icon ui-icon-arrowthickstop-1-n  ui-align-center' onclick='UploadFile(\"" +URLEncrypt.EncryptParameters1(new String[]{ "TicketNo=" +s.TicketNo })+ "\");return false;'>Upload File</a></center>",
                    "<center><a href='#' class='ui-icon ui-icon-zoomin  ui-align-center' onclick='ViewAndAcceptTicket(\"" +URLEncrypt.EncryptParameters1(new String[]{ "TicketNo=" +s.TicketNo })+ "\");return false;'>View </a></center>",
                    dbContext.TKT_TICKET_APPROVAL.Any(x=>x.TICKET_ID==s.TicketNo &&x.PENDING_AT_ROLEID==PMGSYSession.Current.RoleCode && x.STATUS != 4)?"<center><a href='#' class='ui-icon  ui-icon-arrowreturn-1-w  ui-align-center' onclick='ReplyDetails(\"" +URLEncrypt.EncryptParameters1(new String[]{ "TicketNo=" +s.TicketNo })+ "\");return false;'>Reply</a></center>":"<center><a href='#' class='ui-icon ui-icon-locked  ui-align-center'>Locked</a></center>"
                  }
                }).ToArray();

                 */


                #endregion

                lstAllrecords = dbContext.USP_TKT_GET_PENDING_TKTLIST_SELFLEVEL(PMGSYSession.Current.RoleCode,PMGSYSession.Current.UserId).ToList();

                //if (PMGSYSession.Current.RoleCode == 39 && PMGSYSession.Current.UserName == "ticketadmin")  //NRDA Appoving authority
                //  {
                //      lstAllrecords = lstAllrecords.Where(p => !dbContext.TKT_TICKET_APPROVAL.Any(p2 => p2.TICKET_ID == p.TICKET_ID)).ToList();
                //      //lstAllrecords.AddRange(dbContext.TKT_TICKET_MASTER.Where(s => s.REPORTED_USERID == PMGSYSession.Current.UserId)); //entered by approve authority
                //  }
                //    else if (PMGSYSession.Current.RoleCode == 9 || PMGSYSession.Current.RoleCode == 14 || PMGSYSession.Current.RoleCode == 16 || PMGSYSession.Current.RoleCode == 25 || PMGSYSession.Current.RoleCode == 66 || PMGSYSession.Current.RoleCode == 67)  //Replying Autority[CQC, Director F&A, Techical Director,ommasteam/mord,ommasteam/mord,director progress],
                //  {

                //      lstAllrecords = (from master in lstAllrecords
                //                       join approve in dbContext.TKT_TICKET_APPROVAL.ToList()
                //                       on master.TICKET_ID equals approve.TICKET_ID
                //                       where approve.PENDING_AT_ROLEID == PMGSYSession.Current.RoleCode && approve.STATUS !=4 //closed
                //                       select master).ToList();  //pending at user
                //  }
                //  else
                //  {
                //      //lstAllrecords = lstAllrecords.ToList();
                //     lstAllrecords = lstAllrecords.Where(s => s.REPORTED_USERID == PMGSYSession.Current.UserId).ToList();
                //  }


                totalrecords = lstAllrecords.Count;

                if (sidx.Trim() != String.Empty)
                {
                    if (sord == "asc")
                    {
                        switch (sidx)
                        {
                            case "TicketNo":
                                lstAllrecords = lstAllrecords.OrderBy(s => s.TICKET_NO).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                                break;
                            default:
                                lstAllrecords = lstAllrecords.OrderBy(s => s.TICKET_NO).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                                break;
                        }
                    }
                    else
                    {
                        switch (sidx)
                        {
                            case "TicketNo":
                                lstAllrecords = lstAllrecords.OrderByDescending(s => s.TICKET_NO).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                                break;
                            default:
                                lstAllrecords = lstAllrecords.OrderByDescending(s => s.TICKET_NO).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                                break;
                        }
                    }
                }

                var FinalResult = lstAllrecords.Select(x => new
                  {
                      TicketNo = x.TICKET_NO,
                      StateName =x.STATE_NAME,
                      Category = x.TKT_CATEGORY,
                      Module = x.TKT_MODULE,
                      Subject = x.REPORTED_SUBJECT,
                      ReportedBy = x.REPORTED_BY,
                      ReportedDate = x.REPORTED_DATE_TIME,
                      ApprovalDate = x.APPROVAL_DATE_TIME,
                      Status = x.TKT_STATUS,
                      PendingAt = x.CURRENTLY_PENDING_AT,
                      FirstForwardTo = x.FIRST_FORWARDED_TO,  
                      
                      //added by rohit borse on 20-07-2022 for delete
                      isApproved = dbContext.TKT_TICKET_APPROVAL.Where(s => s.TICKET_ID == x.TICKET_NO).Any() ? true : false,
                      Roletype = dbContext.TKT_ROLE_TYPE_FORWARDING.Where(a => a.ROLE_ID == PMGSYSession.Current.RoleCode).Any() ? dbContext.TKT_ROLE_TYPE_FORWARDING.Where(a => a.ROLE_ID == PMGSYSession.Current.RoleCode).Select(a=> a.ROLE_TYPE).First() : "-", 
                }).ToList();

                
              return FinalResult.Select(s => new
                 {
                    id = s.TicketNo,
                    cell = new[]
                   {  
                    s.TicketNo.ToString(),
                    s.StateName,
                    s.Category,
                    s.Module,
                    s.Subject,
                    s.ReportedBy,
                    s.ReportedDate.ToString("dd/MM/yyyy hh:mm tt"),
                    s.ApprovalDate==null ? "-" : s.ApprovalDate.Value.ToString("dd/MM/yyyy hh:mm tt"),
                    s.FirstForwardTo == null ? "-" : s.FirstForwardTo.ToLower().Equals("mord") ? "Director IT" : s.FirstForwardTo,
                    s.Status ,
                    s.PendingAt == null ? "-" : s.PendingAt.ToLower().Equals("mord") ? "Director IT" : s.PendingAt,
                   // dbContext.TKT_TICKET_APPROVAL.Any(f=>f.TICKET_ID==s.TicketNo) ?"<center><a href='#' class='ui-icon ui-icon-locked  ui-align-center'>Locked</a></center>": "<center><a href='#' class='ui-icon ui-icon-arrowthickstop-1-n  ui-align-center' onclick='UploadFile(\"" +URLEncrypt.EncryptParameters1(new String[]{ "TicketNo=" +s.TicketNo })+ "\");return false;'>Upload File</a></center>",
                    (dbContext.TKT_TICKET_MASTER.Any(f=>f.TICKET_ID==s.TicketNo &&f.REPORTED_USERID==PMGSYSession.Current.UserId) && dbContext.TKT_TICKET_APPROVAL.Any(a=>a.TICKET_ID==s.TicketNo)==false) ? "<center><a href='#' class='ui-icon ui-icon-arrowthickstop-1-n  ui-align-center' onclick='UploadFile(\"" +URLEncrypt.EncryptParameters1(new String[]{ "TicketNo=" +s.TicketNo })+ "\");return false;'>Upload File</a></center>":"<center><a href='#' class='ui-icon ui-icon-locked  ui-align-center'>Locked</a></center>",
                    "<center><a href='#' class='ui-icon ui-icon-zoomin  ui-align-center' onclick='ViewAndAcceptTicket(\"" +URLEncrypt.EncryptParameters1(new String[]{ "TicketNo=" +s.TicketNo })+ "\");return false;'>View </a></center>",
                    dbContext.TKT_TICKET_APPROVAL.Any(x=>x.TICKET_ID==s.TicketNo &&x.PENDING_AT_ROLEID==PMGSYSession.Current.RoleCode && x.STATUS != 4)?"<center><a href='#' class='ui-icon  ui-icon-arrowreturn-1-w  ui-align-center' onclick='ReplyDetails(\"" +URLEncrypt.EncryptParameters1(new String[]{ "TicketNo=" +s.TicketNo })+ "\");return false;'>Reply</a></center>":"<center><a href='#' class='ui-icon ui-icon-locked  ui-align-center'>Locked</a></center>",
                   
                    // added by rohit borse on 20-07-2022
                   s.isApproved == false && s.Roletype == "G"
                   ? "<center><a href='#' class='ui-icon ui-icon-trash  ui-align-center' onclick='DeleteTicketMaster(\"" +URLEncrypt.EncryptParameters1(new String[]{ "TicketNo=" +s.TicketNo })+ "\");return false;'>View </a></center>": "-",

                  }
                }).ToArray();
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "TicketDAL.GetGetTicketListDAL()");
                totalrecords = 0;
                return null;
            }
            finally
            {
                if (dbContext != null)
                    dbContext.Dispose();
            }
        }

        public Array GetAllTicketListDAL(int? page, int? rows, string sidx, string sord, out long totalrecords, string filters)
        {
            dbContext = new PMGSYEntities();
            SearchJsonString test = new SearchJsonString();
            string state = string.Empty;
            string category = string.Empty;
            string module = string.Empty;
            string subject = string.Empty;
            string reportedby = string.Empty;
            string reporteddate = string.Empty;
            string apprvoaldate = string.Empty;
            string firstfwdto = string.Empty;
            string status = string.Empty;
            string curpendingAt = string.Empty;
            
            List<USP_TKT_GET_ALL_TKTLIST_SELFLEVEL_Result> lstAllrecords = null;
            try
            {
                if (filters != null)
                {
                    var js = new JavaScriptSerializer();
                    test = js.Deserialize<SearchJsonString>(filters);

                    foreach (SearchRules item in test.rules)
                    {
                        switch (item.field)
                        {
                            case "State": state = item.data;
                                break;
                            case "Category": category = item.data;
                                break;
                            case "Module": module = item.data;
                                break;
                            case "Subject": subject = item.data;
                                break;
                            case "ReportedBy": reportedby = item.data;
                                break;
                            case "ReportedDate": reporteddate = item.data;
                                break;
                            case "ApprovalDate": apprvoaldate = item.data;
                                break;
                            case "ForwardTo": firstfwdto = item.data;
                                break;
                            case "Status": status = item.data;
                                break;
                            case "PendingAt": curpendingAt = item.data;
                                break;
                            default:
                                break;
                        }
                    }
                }

            

                lstAllrecords = dbContext.USP_TKT_GET_ALL_TKTLIST_SELFLEVEL(PMGSYSession.Current.RoleCode, PMGSYSession.Current.UserId,0,0,0).ToList();


                // changed by rohit borse on 20-07-2022
                //lstAllrecords = lstAllrecords.Where(x => x.STATE_NAME.ToLower().Contains(state.Equals(string.Empty) ? "" : state.ToLower()) &&
                //                                                     x.TKT_CATEGORY.ToLower().Contains(category.Equals(string.Empty) ? "" : category.ToLower()) &&
                //                                                     x.TKT_MODULE.ToLower().Contains(module.Equals(string.Empty) ? "" : module.ToLower()) &&
                //                                                     x.REPORTED_SUBJECT.ToLower().Contains(subject.Equals(string.Empty) ? "" : subject.ToLower()) &&
                //                                                     x.REPORTED_BY.ToLower().Contains(reportedby.Equals(string.Empty) ? "" : reportedby.ToLower()) &&
                //                                                     x.FIRST_FORWARDED_TO.ToLower().Contains(firstfwdto.Equals(string.Empty) ? "" : firstfwdto.ToLower()) &&
                //                                                     x.CURRENTLY_PENDING_AT.ToLower().Contains(curpendingAt.Equals(string.Empty) ? "" : curpendingAt.ToLower()) &&
                //                                                     x.TKT_STATUS.ToLower().Contains(status.Equals(string.Empty) ? "" : status.ToLower())
                //                                                     ).OrderByDescending(x => x.TICKET_NO).ToList();

                lstAllrecords = lstAllrecords.Where(x => x.STATE_NAME.ToLower().Contains(state.Equals(string.Empty) ? "" : state.ToLower()) &&
                                                                     x.TKT_CATEGORY.ToLower().Contains(category.Equals(string.Empty) ? "" : category.ToLower()) &&
                                                                     x.TKT_MODULE.ToLower().Contains(module.Equals(string.Empty) ? "" : module.ToLower()) &&
                                                                     x.REPORTED_SUBJECT.ToLower().Contains(subject.Equals(string.Empty) ? "" : subject.ToLower()) &&
                                                                     x.REPORTED_BY.ToLower().Contains(reportedby.Equals(string.Empty) ? "" : reportedby.ToLower()) &&
                                                                     x.FIRST_FORWARDED_TO.ToLower().Contains(firstfwdto.Equals(string.Empty) ? "" : firstfwdto.ToLower()) &&
                                                                     x.CURRENTLY_PENDING_AT.ToLower().Contains(curpendingAt.Equals(string.Empty) ? "" : curpendingAt.ToLower()) &&
                                                                     x.TKT_STATUS.ToLower().Contains(status.Equals(string.Empty) ? "" : status.ToLower())
                                                                     ).OrderBy(x => x.TICKET_NO).ToList();

                totalrecords = lstAllrecords.Count;

                if (sidx.Trim() != String.Empty)
                {
                    if (sord == "asc")
                    {
                        switch (sidx)
                        {
                            case "TicketNo":
                                lstAllrecords = lstAllrecords.OrderBy(s => s.TICKET_NO).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                                break;
                            default:
                                lstAllrecords = lstAllrecords.OrderBy(s => s.TICKET_NO).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                                break;
                        }
                    }
                    else
                    {
                        switch (sidx)
                        {
                            case "TicketNo":
                                lstAllrecords = lstAllrecords.OrderByDescending(s => s.TICKET_NO).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                                break;
                            default:
                                lstAllrecords = lstAllrecords.OrderByDescending(s => s.TICKET_NO).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                                break;
                        }
                    }
                }

                // changed by rohit borse on 20-07-2022
                var FinalResult = lstAllrecords.Select(x => new
                {
                    TicketNo = x.TICKET_NO,
                    StateName = x.STATE_NAME,
                    Category = x.TKT_CATEGORY,
                    Module = x.TKT_MODULE,
                    Subject = x.REPORTED_SUBJECT,
                    ReportedBy = x.REPORTED_BY,
                    ReportedDate = x.REPORTED_DATE_TIME,
                    ApprovalDate = x.APPROVAL_DATE_TIME,
                    Status = x.TKT_STATUS,
                    PendingAt = x.CURRENTLY_PENDING_AT,
                    FirstForwardTo = x.FIRST_FORWARDED_TO,
                    ClosingDate = x.CLOSED_DATE_TIME
                }).ToList();


                return FinalResult.Select(s => new
                {
                    id = s.TicketNo,
                    cell = new[]
                   {  
                    s.TicketNo.ToString(),
                    s.StateName,
                    s.Category,
                    s.Module,
                    s.Subject,
                    s.ReportedBy,
                    s.ReportedDate.ToString("dd/MM/yyyy hh:mm tt"),
                    s.ApprovalDate==null ? "-" : s.ApprovalDate.Value.ToString("dd/MM/yyyy hh:mm tt"),                    
                    s.FirstForwardTo == null ? "-" : s.FirstForwardTo.ToLower().Equals("mord") ? "Director IT" : s.FirstForwardTo,
                    s.Status ,
                    s.PendingAt == null ? "-" : s.PendingAt.ToLower().Equals("mord") ? "Director IT" : s.PendingAt,
                    s.ClosingDate==null ? "-" : s.ClosingDate.Value.ToString("dd/MM/yyyy hh:mm tt"),
                    "<center><a href='#' class='ui-icon ui-icon-zoomin  ui-align-center' onclick='ViewAndAcceptTicket(\"" +URLEncrypt.EncryptParameters1(new String[]{ "TicketNo=" +s.TicketNo })+ "\");return false;'>View </a></center>",
                  }
                }).ToArray();
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "TicketDAL.GetGetTicketListDAL()");
                totalrecords = 0;
                return null;
            }
            finally
            {
                if (dbContext != null)
                    dbContext.Dispose();
            }
        }





        public List<SelectListItem> GetCategory(bool disposing=true)
        {
            dbContext = new PMGSYEntities();
            List<SelectListItem> lstCategory = new List<SelectListItem>();

            try
            {
                lstCategory =new SelectList(dbContext.MASTER_TKT_CATEGORY.ToList(),"MAST_CATEGORY_CODE","MAST_CATEGORY_NAME").ToList<SelectListItem>();
                     
                lstCategory.Insert(0, new SelectListItem { Value = "0", Text = "Select Category" });
                return lstCategory;
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "TicketingDAL.GetCategory()");
                return new List<SelectListItem>() { new SelectListItem { Text = "select category", Value = "0" } };
            }
            finally
            {
                if (disposing)
                {
                    if (dbContext != null)
                        dbContext.Dispose();
                }
            }

        }

        public List<SelectListItem> GetModule()
        {
            dbContext = new PMGSYEntities();
            List<SelectListItem> lstModule = null;

            try
            {
                // change by rohit borse on 20-07-2022
                //var lst = dbContext.MASTER_MODULE.ToList();
                var moduleCodelist = dbContext.TKT_TG_MODULE_MAPPING.Where(x=>x.ROLEID == PMGSYSession.Current.RoleCode).Select(s=>s.MAST_MODULE_CODE).ToList();
                if (moduleCodelist.Count > 0)
                {
                    var lst = dbContext.MASTER_MODULE.Where(x => moduleCodelist.Contains(x.MAST_MODULE_CODE)).ToList();

                    lstModule = new SelectList(lst, "MAST_MODULE_CODE", "MAST_MODULE_NAME").ToList<SelectListItem>();
                    lstModule.Insert(0, new SelectListItem { Value = "0", Text = "Select Module" });
                    return lstModule;
                }
                else {
                    var lst = dbContext.MASTER_MODULE.ToList();

                    lstModule = new SelectList(lst, "MAST_MODULE_CODE", "MAST_MODULE_NAME").ToList<SelectListItem>();
                    lstModule.Insert(0, new SelectListItem { Value = "0", Text = "Select Module" });
                    return lstModule;
                }
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "TicketingDAL.GetModule()");
                return new List<SelectListItem>() { new SelectListItem { Text = "select Module", Value = "0" } };
            }
            finally
            {
                if (dbContext != null)
                   dbContext.Dispose();
            }

        }

        public string GetUserRoleInfo()
        {
            dbContext = new PMGSYEntities();
            try
            {
                var obj = dbContext.TKT_ROLE_TYPE_FORWARDING.Where(s => s.ROLE_ID == PMGSYSession.Current.RoleCode).FirstOrDefault();
                if (obj != null)
                  return obj.ROLE_TYPE;
                return String.Empty;
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "TicketingDAL.GetUserRoleInfo()");
                return String.Empty;
            }
            finally
            {
                if(dbContext != null)
                  dbContext.Dispose();
            }

        }


        #region commented / changed by rohit borse on 14-07-2022

        public bool SaveTicketDetailsDAL(TicketViewModel model)
        {
            dbContext = new PMGSYEntities();
            try
            {
                TKT_TICKET_MASTER dbModel = new TKT_TICKET_MASTER();
                dbModel.TICKET_ID = dbContext.TKT_TICKET_MASTER.Any() ? dbContext.TKT_TICKET_MASTER.Max(x => x.TICKET_ID) + 1 : 1;
                dbModel.MAST_MODULE_CODE = model.ModuleID;
                dbModel.REPORTED_USERID = PMGSYSession.Current.UserId;
                dbModel.REPORTED_BY = model.Name;
                dbModel.REPORTED_CONTACT = model.Contact;
                dbModel.REPORTED_EMAIL = model.Email;
                dbModel.REPORTED_SUBJECT = model.Subject;
                dbModel.REPORTED_DESCRIPTION = model.Remarks;
                dbModel.REPORTED_DATE_TIME = DateTime.Now;
                dbModel.MAST_CATEGORY_CODE = model.Category;
                dbContext.TKT_TICKET_MASTER.Add(dbModel);
                return dbContext.SaveChanges() > 0 ? true : false;
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "TicketingDAL.SaveTicketDetailsDAL()");
                return false;
            }
            finally
            {
                if (dbContext != null)
                    dbContext.Dispose();
            }
        }

        //public bool SaveTicketDetailsDAL(TicketViewModel viewmodel)
        //{
        //    dbContext = new PMGSYEntities();
        //    bool isSaveChangesDone = false;

        //    try
        //    {
        //         using (var scope = new TransactionScope())
        //        { 
        //            int newTicketId = dbContext.TKT_TICKET_MASTER.Any() ? dbContext.TKT_TICKET_MASTER.Max(x => x.TICKET_ID) + 1 : 1;

        //            TKT_TICKET_MASTER dbModel = new TKT_TICKET_MASTER();
        //            dbModel.TICKET_ID = newTicketId;
        //            dbModel.MAST_MODULE_CODE = viewmodel.ModuleID;
        //            dbModel.REPORTED_USERID = PMGSYSession.Current.UserId;
        //            dbModel.REPORTED_BY = viewmodel.Name;
        //            dbModel.REPORTED_CONTACT = viewmodel.Contact;
        //            dbModel.REPORTED_EMAIL = viewmodel.Email;
        //            dbModel.REPORTED_SUBJECT = viewmodel.Subject;
        //            dbModel.REPORTED_DESCRIPTION = viewmodel.Remarks;
        //            dbModel.REPORTED_DATE_TIME = DateTime.Now;
        //            dbModel.MAST_CATEGORY_CODE = viewmodel.Category;
        //            dbContext.TKT_TICKET_MASTER.Add(dbModel);

        //            List<short> ForwardingAuthority = dbContext.TKT_ROLE_TYPE_FORWARDING.Select(s => s.ROLE_ID).ToList<short>();
        //            int? forwardToRole =  dbContext.MASTER_MODULE.Where(m => m.MAST_MODULE_CODE == viewmodel.ModuleID).Select(m => m.FORWARD_TO_ROLE_ID).First();
        //            string RoleName = dbContext.UM_Role_Master.Where(s => s.RoleID == forwardToRole).Select(s => s.RoleName).First();

        //            TicketAcceptModel Acceptmodel = new TicketAcceptModel();
        //            Acceptmodel.AcceptReject = 1;   //ticket accepted by default 
        //            Acceptmodel.ForwardTo = forwardToRole == null ? 0 : (int)forwardToRole;
        //            Acceptmodel.ActionTakenRemark = string.Concat("Forwarded to " + RoleName);

        //            if (Acceptmodel.AcceptReject != 2)
        //            {
        //                if (!ForwardingAuthority.Contains(((short)Acceptmodel.ForwardTo)))
        //                {
        //                    //message = "Invalid Forwarding authority is selected.";
        //                    return false;
        //                }
        //            }                    

        //            TKT_TICKET_APPROVAL apporveModel = new TKT_TICKET_APPROVAL();
        //            apporveModel.TICKET_ID = newTicketId;
        //            apporveModel.IS_APPROVED = Acceptmodel.AcceptReject == 1 ? 1 : 0;
        //            apporveModel.APPROVED_BY_USERID = PMGSYSession.Current.UserId;
        //            apporveModel.APPROVAL_DATE_TIME = DateTime.Now;
        //            apporveModel.REMARKS = Acceptmodel.ActionTakenRemark;
        //            apporveModel.STATUS = 0; //Acceptmodel.AcceptReject == 1 ? 1 : 4;    //1=opened,2=in-progress,3=partial closed,4=closed
        //            if (Acceptmodel.AcceptReject == 1)
        //            {
        //               apporveModel.PENDING_AT_ROLEID = (short)Acceptmodel.ForwardTo;  //if 2 rejeted then leave null;
        //            }                    
        //            dbContext.TKT_TICKET_APPROVAL.Add(apporveModel);


        //            TKT_TICKET_DETAIL tktdetailObj = new TKT_TICKET_DETAIL();
        //            tktdetailObj.DETAIL_ID = dbContext.TKT_TICKET_DETAIL.Any() ? dbContext.TKT_TICKET_DETAIL.Max(x => x.DETAIL_ID) + 1 : 1;
        //            tktdetailObj.TICKET_ID = newTicketId;
        //            if (Acceptmodel.AcceptReject == 1)
        //            {
        //                tktdetailObj.FORWARDED_BY_USERID = PMGSYSession.Current.UserId;
        //                tktdetailObj.FORWARDED_DATE_TIME = DateTime.Now;

        //                tktdetailObj.FORWARDED_TO_ROLEID = (short)Acceptmodel.ForwardTo; //if 2 rejeted then leave null;

        //            }

        //            tktdetailObj.CURRENT_STATUS = Acceptmodel.AcceptReject == 1 ? 0 : 4; //1=opened,2=in-progress,3=partial closed,4=closed [if accept reject 2 then close it (4)]
        //            tktdetailObj.INSERTED_DATETIME = DateTime.Now;
        //            tktdetailObj.INSERTED_USERID = PMGSYSession.Current.UserId;
        //            dbContext.TKT_TICKET_DETAIL.Add(tktdetailObj);

        //            isSaveChangesDone =  dbContext.SaveChanges() > 0 ? true : false;
        //            scope.Complete();               
        //            return isSaveChangesDone;
        //        }

        //    }
        //    catch (Exception ex)
        //    {
        //        ErrorLog.LogError(ex, "TicketingDAL.SaveTicketDetailsDAL()");
        //        return false;
        //    }
        //    finally
        //    {
        //        if (dbContext != null)
        //            dbContext.Dispose();
        //    }
        //}

        #endregion

        public bool SaveTicketFile(int ticketNo, HttpPostedFileBase file, out string message)
        {
            dbContext = new PMGSYEntities();
            try
            {
                using (var scope = new TransactionScope())
                {
                    string ext = file.FileName.Substring(file.FileName.LastIndexOf('.') + 1).Trim().ToLower();

                    String Basepath = ConfigurationManager.AppSettings["TICKET_UPLOAD_MAIN"].ToString();
                    if (!Directory.Exists(Basepath))
                        Directory.CreateDirectory(Basepath);  //if Directory not created creat it;

                    TKT_TICKET_FILES fileObj = new TKT_TICKET_FILES();
                    fileObj.TICKET_FILE_ID = dbContext.TKT_TICKET_FILES.Any() ? dbContext.TKT_TICKET_FILES.Max(x => x.TICKET_FILE_ID) + 1 : 1;
                    fileObj.TICKET_FILE_NAME = "TKT_" + ticketNo+"_"+fileObj.TICKET_FILE_ID + "_" + DateTime.Now.ToString("dd_MM_yyyy") + "." + ext;
                    fileObj.TICKET_FILE_TYPE = "I"; //issued
                    fileObj.TICKET_FILE_UPLOAD_DATE = DateTime.Now;
                    fileObj.TICKET_ID = ticketNo;
                    fileObj.TICKET_UPLOADED_BY = PMGSYSession.Current.UserId;
                    dbContext.TKT_TICKET_FILES.Add(fileObj);

                    //==
                    file.SaveAs(System.IO.Path.Combine(Basepath, fileObj.TICKET_FILE_NAME));  //save file
                    //==
                    dbContext.SaveChanges();
                    scope.Complete();
                }
                message="Ticket File Saved Successfully";
                return true;
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "TicketingDAL.SaveTicketFile()");
                message = "Error occured while processing your request.";
                return false;
            }
            finally
            {
                if (dbContext != null)
                    dbContext.Dispose();
            }
        }


        public Array GetTicketFileListDAL(int? page, int? rows, string sidx, string sord, int tktno, out long totalrecords)
        {
            dbContext = new PMGSYEntities();
            try
            {
                List<TKT_TICKET_FILES> lstAllrecords = dbContext.TKT_TICKET_FILES.Where(x => x.TICKET_ID == tktno && x.TICKET_FILE_TYPE=="I").ToList();

                totalrecords = lstAllrecords.Count;

                if (sidx.Trim() != String.Empty)
                {
                    if (sord == "asc")
                    {
                        switch (sidx)
                        {
                            case "FileName":
                                lstAllrecords = lstAllrecords.OrderBy(s => s.TICKET_ID).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                                break;
                            default:
                                lstAllrecords = lstAllrecords.OrderBy(s => s.TICKET_ID).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                                break;
                        }
                    }
                    else
                    {
                        switch (sidx)
                        {
                            case "FileName":
                                lstAllrecords = lstAllrecords.OrderByDescending(s => s.TICKET_ID).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                                break;
                            default:
                                lstAllrecords = lstAllrecords.OrderByDescending(s => s.TICKET_ID).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                                break;
                        }
                   }
                }
                return lstAllrecords.Select(x => new
                {
                    id = x.TICKET_FILE_ID,
                    cell = new[]
                        {
                         x.TICKET_FILE_NAME,
                         x.TICKET_FILE_UPLOAD_DATE.ToString("dd/MM/yyyy"),
                         dbContext.UM_Role_Master.FirstOrDefault(s=>s.RoleID==x.UM_User_Master.DefaultRoleID).RoleName,
                         "<a href='/Ticket/GetTicketFile?id="+URLEncrypt.EncryptParameters1(new String[]{"TKTFile="+x.TICKET_FILE_NAME})+"' title='Click here to ticket details' class='ui-icon ui-icon-arrowthickstop-1-s  ui-align-center' target=_blank></a>",
                         x.TICKET_FILE_ID > 0 ? "<span id='btnDelete' value='Delete File' onClick='deleteUploadedFile(\""+ x.TICKET_FILE_ID.ToString()+"\");'class='ui-icon ui-icon-trash ui-align-center' />" : "-",
                         //"<a href='/Ticket/DeleteTicketFile?id="+URLEncrypt.EncryptParameters1(new String[]{"TKTFile="+x.TICKET_FILE_NAME,"TKTFileId="+x.TICKET_FILE_ID })+"' title='Click here to delete file' class='ui-icon ui-icon-trash  ui-align-center'></a>",                         
                        }

                }).ToArray();
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "TicketingDAL.GetTicketFileListDAL()");
                totalrecords = 0;
                return null;
            }
            finally
            {
                if (dbContext != null)
                    dbContext.Dispose();
            }
        }


        public TicketAcceptModel GetTicketAcceptDetailDAL(int ticketNo, out Boolean IsApproved)
        {
            dbContext = new PMGSYEntities();
            try
            {
                TicketAcceptModel model = new TicketAcceptModel();
                TKT_TICKET_MASTER masterObj = dbContext.TKT_TICKET_MASTER.FirstOrDefault(s => s.TICKET_ID == ticketNo);

                UM_User_Master user = dbContext.UM_User_Master.FirstOrDefault(x => x.UserID == masterObj.REPORTED_USERID);
                String Role = dbContext.UM_Role_Master.FirstOrDefault(r => r.RoleID == user.DefaultRoleID).RoleName;
                String state = (user.Mast_State_Code == null || user.Mast_State_Code == 0) ? " " : dbContext.MASTER_STATE.FirstOrDefault(st => st.MAST_STATE_CODE == user.Mast_State_Code).MAST_STATE_NAME;


                model.Category = masterObj.MAST_CATEGORY_CODE;

                TicketMatserDetails detailsModel = new TicketMatserDetails();
                detailsModel.TicketNo = masterObj.TICKET_ID.ToString();
                detailsModel.Name = masterObj.REPORTED_BY + " ( " + Role + " " + state + ")";
                detailsModel.Contact = masterObj.REPORTED_CONTACT;
                detailsModel.Email = masterObj.REPORTED_EMAIL;
                detailsModel.ModuleName = masterObj.MASTER_MODULE.MAST_MODULE_NAME;
                detailsModel.CategoryName = masterObj.MASTER_TKT_CATEGORY.MAST_CATEGORY_NAME;
                detailsModel.Subject = masterObj.REPORTED_SUBJECT;
                detailsModel.Description = masterObj.REPORTED_DESCRIPTION;
                List<String> filesnames = dbContext.TKT_TICKET_FILES.Where(s => s.TICKET_ID == ticketNo && s.TICKET_FILE_TYPE=="I").Select(n => n.TICKET_FILE_NAME).ToList();
                if (filesnames.Count > 0)
                {
                    foreach (string name in filesnames)
                    {
                        detailsModel.FilesUrls.Add("<a href='/Ticket/GetTicketFile?id=" + URLEncrypt.EncryptParameters1(new String[] { "TKTFile=" + name }) + "' title='Click here to ticket details' class='' target=_blank>" + name + "</a>");
                    }
                }

                IsApproved = dbContext.TKT_TICKET_APPROVAL.Any(s => s.TICKET_ID == ticketNo); //is approved 

                if (PMGSYSession.Current.RoleCode == 39 && IsApproved==false)  //approving autority and Yet to be approved
                {
                    // changed by rohit borse on 20-07-2022
                    //model.lstCategory = GetCategory(false);
                    if (IsApproved == false)
                    {
                        List<SelectListItem> listCategory = new List<SelectListItem>();
                        listCategory = GetCategory(false);
                        listCategory.Remove(listCategory.Find(s => s.Value == "5"));
                        listCategory.Remove(listCategory.Find(s => s.Value == "6"));
                        model.lstCategory = listCategory;
                    }
                    // added by rohit borse on 20-07-2022
                    int ? forwardToRolecode = dbContext.MASTER_MODULE.Where(s => s.MAST_MODULE_CODE == masterObj.MAST_MODULE_CODE).Select(s => s.FORWARD_TO_ROLE_ID).First();
                    model.ForwardTo = forwardToRolecode == null ? -2 : (int)forwardToRolecode;

                    model.ForwardToList = GetForwardingAuthority();
                }
                else   //Ticket entry 
                {
                   var relymodelList = (from detail in dbContext.TKT_TICKET_DETAIL
                                        join approve in dbContext.TKT_TICKET_APPROVAL
                                        on detail.TICKET_ID equals approve.TICKET_ID
                                        //// where approve.STATUS != 4 && detail.ACTION_BY != null && detail.ACTION_DATE_TIME != null //closed
                                        where detail.TICKET_ID==ticketNo && approve.IS_APPROVED !=0 && detail.ACTION_BY_USERID != null && detail.ACTION_DATE_TIME != null //closed
                                        select new 
                                        {    
                                            DetailActionBy = detail.ACTION_BY_USERID,
                                            ReplyByRoleId = dbContext.UM_User_Master.FirstOrDefault(u=>u.UserID==detail.ACTION_BY_USERID).DefaultRoleID,
                                            ForwardedTo = dbContext.UM_Role_Master.FirstOrDefault(a => a.RoleID == detail.FORWARDED_TO_ROLEID).RoleName,
                                            Reply = detail.ACTION_TAKEN == null ? "NA" : detail.ACTION_TAKEN, 
                                            ReplyDate = detail.ACTION_DATE_TIME,
                                            CurrentStatus = detail.CURRENT_STATUS
                                            //FilesUrls = dbContext.TKT_TICKET_FILES.Any(s => s.TICKET_FILE_TYPE == "R" && s.TICKET_FILE_NAME.Contains(FileNameStarts)) ? dbContext.TKT_TICKET_FILES.Where(s => s.TICKET_FILE_TYPE == "R" && s.TICKET_FILE_NAME.Contains(FileNameStarts)).Select(f => f.TICKET_FILE_NAME) : null,
                                        }).ToList();


                                            var replymodelListWithFileName = relymodelList.Select(s => new ViewReplyModel
                                            {
                                                //added by rohit on 30-08-2022 change for 'directorit' login

                                                //ReplyBy = dbContext.UM_Role_Master.FirstOrDefault(r => r.RoleID == s.ReplyByRoleId).RoleName,
                                                //ForwardedTo = s.ForwardedTo,
                                                ReplyBy = s.ReplyByRoleId == 25 ? "Director IT" : dbContext.UM_Role_Master.FirstOrDefault(r => r.RoleID == s.ReplyByRoleId).RoleName,
                                                ForwardedTo = s.ForwardedTo == null ? "NA" : s.ForwardedTo.Equals("mord") ? "Director IT" : s.ForwardedTo,

                                                Reply = s.Reply,
                                                ReplyDate = s.ReplyDate,
                                                FilesUrls = dbContext.TKT_TICKET_FILES.Any(f => f.TICKET_UPLOADED_BY == s.DetailActionBy && f.TICKET_ID == ticketNo && f.TICKET_FILE_TYPE == "R") ? dbContext.TKT_TICKET_FILES.FirstOrDefault(x => x.TICKET_UPLOADED_BY == s.DetailActionBy && x.TICKET_ID == ticketNo && x.TICKET_FILE_TYPE == "R").TICKET_FILE_NAME : "" ,
                                                Status = GetStatus(s.CurrentStatus),
                                            }).ToList();

                                           model.listReplymodel = replymodelListWithFileName.Select(s => new ViewReplyModel
                                            {
                                                ReplyBy = s.ReplyBy,
                                                ForwardedTo = s.ForwardedTo,
                                                Reply = s.Reply,
                                                ReplyDate = s.ReplyDate,
                                                FilesUrls = s.FilesUrls != "" ?  "<a href='/Ticket/GetTicketFile?id=" + URLEncrypt.EncryptParameters1(new String[] { "TKTFile=" + s.FilesUrls }) + "' title='Click here to ticket details' class='' target=_blank>" + s.FilesUrls + "</a>" : null,
                                                Status = s.Status
                                            }).ToList();

                   TKT_TICKET_APPROVAL approvemodel = dbContext.TKT_TICKET_APPROVAL.FirstOrDefault(s => s.TICKET_ID == ticketNo);
                   if (approvemodel != null)
                   {
                        // added by rohit borse on 20-07-2022 for showing in internal forwarding -- forwarded to 
                        short? getRoleIdForForwardedNames = dbContext.TKT_TICKET_DETAIL.Where(s => s.TICKET_ID == ticketNo && s.CURRENT_STATUS != 4).Select(s => s.FORWARDED_TO_ROLEID).Any()
                                                           ? dbContext.TKT_TICKET_DETAIL.Where(s => s.TICKET_ID == ticketNo && s.CURRENT_STATUS != 4).Select(s => s.FORWARDED_TO_ROLEID).First()
                                                           : 0;

                        ViewReplyModel replymodel = new ViewReplyModel
                        {
                            ReplyBy = dbContext.UM_Role_Master.FirstOrDefault(s => s.RoleID == approvemodel.UM_User_Master.DefaultRoleID).RoleName + "- Ticket Admin ",
                            Reply = approvemodel.REMARKS == null ? "NA" : approvemodel.REMARKS,  
                            // changed by rohit borse on 20-07-2022
                            //ForwardedTo = approvemodel.PENDING_AT_ROLEID == null ? "-" : dbContext.UM_Role_Master.FirstOrDefault(s => s.RoleID == approvemodel.PENDING_AT_ROLEID).RoleName,
                            ForwardedTo = getRoleIdForForwardedNames == 0 ? "NA" : (getRoleIdForForwardedNames == 25 ? "Director IT" : dbContext.UM_Role_Master.FirstOrDefault(s => s.RoleID == getRoleIdForForwardedNames).RoleName),
                            ReplyDate = approvemodel.APPROVAL_DATE_TIME,
                            FilesUrls = null,
                            Status = GetStatus(dbContext.TKT_TICKET_DETAIL.FirstOrDefault(s=>s.TICKET_ID==ticketNo ).CURRENT_STATUS)
                        };
                       model.listReplymodel.Insert(0, replymodel);
                   }
                      model.listReplymodel.Reverse();
                }
                model.listTicketMatserDetail.Add(detailsModel);
                return model;
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "TicketingDAL.GetTicketAcceptDetailDAL()");
                IsApproved = false;
                return null;
            }
            finally
            {
                if (dbContext != null)
                    dbContext.Dispose();
            }
        }


        public bool SaveApproveDetailsDAL(TicketAcceptModel model, out string message)
        {

            dbContext = new PMGSYEntities();
            List<SelectListItem> lstCategory = new List<SelectListItem>();
            try
            {
                string[] encparam = model.TicketNo.Split('/');
                Dictionary<String, String> decParams = URLEncrypt.DecryptParameters1(new String[] { encparam[0], encparam[1], encparam[2] });

                if (decParams.Count > 0)
                {
                    int TicketNumber = Convert.ToInt32(decParams["TicketNo"]);
                    using (var scope = new TransactionScope())
                    {
                        // changed by rohit borse on 20-07-2022
                        //List<int> ForwardingAuthority = new List<int> { 9, 14, 16, 25, 66, 67, 2,39,99};  // CQC,Director F&A,Technical Director,mord(ommasdteam dual role),Ommas Team,Director  Progress,SRRDA,NRRDA, ticket generator
                        List<int?> ForwardingAuthority = dbContext.TKT_ROLE_TYPE_FORWARDING.Select(s=>s.FORWARD_TO_ROLE_ID).ToList();  // CQC,Director F&A,Technical Director,mord(ommasdteam dual role),Ommas Team,Director  Progress,SRRDA,NRRDA, ticket generator

                        if (model.AcceptReject != 2)
                        {
                            // change by rohit borse on 20-07-2022
                            //if (!ForwardingAuthority.Contains(model.ForwardTo))
                            if (!ForwardingAuthority.Contains(model.ForwardTo))
                            {
                                message = "Invalid Forwarding authority is selected.";
                                return false;
                            }
                        }
                        if (model.AcceptReject > 2 || model.AcceptReject < 1 )
                        {
                            message = "Invalid Action. Please select valid action";
                            return false;
                        }
                        
                        TKT_TICKET_APPROVAL apporveModel = new TKT_TICKET_APPROVAL();
                        apporveModel.TICKET_ID = TicketNumber;
                        apporveModel.IS_APPROVED = model.AcceptReject == 1 ? 1 : 0;
                        apporveModel.APPROVED_BY_USERID = PMGSYSession.Current.UserId;
                        apporveModel.APPROVAL_DATE_TIME = DateTime.Now;
                        apporveModel.REMARKS = model.ActionTakenRemark;
                        apporveModel.STATUS = model.AcceptReject == 1 ? 1 : 4;    //1=opened,2=in-progress,3=partial closed,4=closed
                        if (model.AcceptReject == 1)
                        {
                            
                            // change by rohit borse on 20-07-2022
                            if (model.ForwardTo == -1) //generator                            
                            {
                                int userId = dbContext.TKT_TICKET_MASTER.FirstOrDefault(s => s.TICKET_ID == TicketNumber).REPORTED_USERID;
                                int roleId = dbContext.UM_User_Master.FirstOrDefault(s => s.UserID == userId).DefaultRoleID;
                                apporveModel.PENDING_AT_ROLEID = (short)roleId;
                            }
                            else
                            {
                                apporveModel.PENDING_AT_ROLEID = (short)model.ForwardTo;  //if 2 rejeted then leave null;
                            }
                        }
                        if (model.AcceptReject == 2) //only if rejected
                        {
                         apporveModel.CLOSED_BY_USERID = PMGSYSession.Current.UserId;
                         apporveModel.CLOSED_DATE_TIME = DateTime.Now;
                        }

                        dbContext.TKT_TICKET_APPROVAL.Add(apporveModel);


                        TKT_TICKET_DETAIL tktdetailObj = new TKT_TICKET_DETAIL();
                        tktdetailObj.DETAIL_ID = dbContext.TKT_TICKET_DETAIL.Any() ? dbContext.TKT_TICKET_DETAIL.Max(x => x.DETAIL_ID) + 1 : 1;
                        tktdetailObj.TICKET_ID = TicketNumber;
                        if (model.AcceptReject == 1)
                        {
                            tktdetailObj.FORWARDED_BY_USERID = PMGSYSession.Current.UserId;
                            tktdetailObj.FORWARDED_DATE_TIME = DateTime.Now;

                            // changed by rohit borse on 20-07-2022
                            if (model.ForwardTo == -1) //generator                            
                            {
                                int userId = dbContext.TKT_TICKET_MASTER.FirstOrDefault(s => s.TICKET_ID == TicketNumber).REPORTED_USERID;
                                int roleId = dbContext.UM_User_Master.FirstOrDefault(s => s.UserID == userId).DefaultRoleID;
                                tktdetailObj.FORWARDED_TO_ROLEID = (short)roleId;
                            }
                            else
                            {
                                tktdetailObj.FORWARDED_TO_ROLEID = (short)model.ForwardTo; //if 2 rejeted then leave null;
                            }
                        }

                        if (model.AcceptReject == 2)
                        {
                            tktdetailObj.ACTION_BY_USERID = PMGSYSession.Current.UserId;
                            tktdetailObj.ACTION_TAKEN = model.ActionTakenRemark;
                            tktdetailObj.ACTION_DATE_TIME = DateTime.Now;
                        }

                        tktdetailObj.CURRENT_STATUS = model.AcceptReject == 1 ? 1 : 4;//1=opened,2=in-progress,3=partial closed,4=closed [if accept reject 2 then close it (4)]
                        tktdetailObj.INSERTED_DATETIME = DateTime.Now;
                        tktdetailObj.INSERTED_USERID = PMGSYSession.Current.UserId;
                        dbContext.TKT_TICKET_DETAIL.Add(tktdetailObj);

                        // changed by rohit borse on 14-07-2022
                        //if (model.Category > 0 && model.Category < 5)
                        if (model.Category > 0 && model.Category < 9)
                        { 
                            TKT_TICKET_MASTER masterTkt = dbContext.TKT_TICKET_MASTER.FirstOrDefault(x => x.TICKET_ID == TicketNumber);
                            masterTkt.MAST_CATEGORY_CODE = model.Category;
                            dbContext.TKT_TICKET_MASTER.Attach(masterTkt);
                            dbContext.Entry(masterTkt).Property(x => x.MAST_CATEGORY_CODE).IsModified = true;
                        }

                        dbContext.SaveChanges();
                        scope.Complete();
                         message = "Ticket approve details saved successfully";
                       return true;
                    }
                }
                else {
                    message = "Invalid ticket,kindly contact administrator";
                    return false;
                }

                
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "TicketingDAL.SaveApproveDetailsDAL()");
                message = "Error occurred while processing your request.";
                return false;
            }
            finally
            {
               if (dbContext != null)
                 dbContext.Dispose();
            }
        }


        public TicketReplyModel GetTicketReplyDetailDAL(int ticketNo)
        {
            dbContext = new PMGSYEntities();
            try
            {
                TicketReplyModel model = new TicketReplyModel();
                TKT_TICKET_MASTER masterObj = dbContext.TKT_TICKET_MASTER.FirstOrDefault(s => s.TICKET_ID == ticketNo);

                UM_User_Master user = dbContext.UM_User_Master.FirstOrDefault(x => x.UserID == masterObj.REPORTED_USERID);
                String Role = dbContext.UM_Role_Master.FirstOrDefault(r => r.RoleID == user.DefaultRoleID).RoleName;
                String state = (user.Mast_State_Code==null || user.Mast_State_Code== 0)?" ":dbContext.MASTER_STATE.FirstOrDefault(st=>st.MAST_STATE_CODE==user.Mast_State_Code).MAST_STATE_NAME;

                model.MasterTicketModel = new TicketMatserDetails();
                model.MasterTicketModel.TicketNo = masterObj.TICKET_ID.ToString();
                model.MasterTicketModel.Name = masterObj.REPORTED_BY+" ( "+Role+" "+state+")";
                model.MasterTicketModel.Contact = masterObj.REPORTED_CONTACT;
                model.MasterTicketModel.Email = masterObj.REPORTED_EMAIL;
                model.MasterTicketModel.ModuleName = masterObj.MASTER_MODULE.MAST_MODULE_NAME;
                model.MasterTicketModel.CategoryName = masterObj.MASTER_TKT_CATEGORY.MAST_CATEGORY_NAME;
                model.MasterTicketModel.Subject = masterObj.REPORTED_SUBJECT;
                model.MasterTicketModel.Description = masterObj.REPORTED_DESCRIPTION;
                
                //added by rohit borse on 14-07-2022
                model.Category = masterObj.MAST_CATEGORY_CODE;
                model.lstCategory = GetCategory(false);

                List<String> filesnames = dbContext.TKT_TICKET_FILES.Where(s => s.TICKET_ID == ticketNo && s.TICKET_FILE_TYPE=="I").Select(n => n.TICKET_FILE_NAME).ToList();
                if (filesnames.Count > 0)
                {
                    foreach (string name in filesnames)
                    {
                        model.MasterTicketModel.FilesUrls.Add("<a href='/Ticket/GetTicketFile?id=" + URLEncrypt.EncryptParameters1(new String[] { "TKTFile=" + name }) + "' title='Click here to ticket details' class='' target=_blank>" + name + "</a>");
                    }
                }

                // added by rohit borse on 20-07-2022
                int? forwardToRolecode = dbContext.MASTER_MODULE.Where(s => s.MAST_MODULE_CODE == masterObj.MAST_MODULE_CODE).Select(s => s.FORWARD_TO_ROLE_ID).First();
                model.ForwardTo = forwardToRolecode == null ? -2 : (int)forwardToRolecode;

                model.ForwardToList = GetForwardingAuthority();
                    //remove the current Role Name
                   model.ForwardToList.Remove(model.ForwardToList.FirstOrDefault(x=>x.Value==PMGSYSession.Current.RoleCode+""));

                   
                    var relymodelList = (from detail in dbContext.TKT_TICKET_DETAIL
                                         join approve in dbContext.TKT_TICKET_APPROVAL
                                         on detail.TICKET_ID equals approve.TICKET_ID
                                         // where approve.STATUS != 4 && detail.ACTION_BY != null && detail.ACTION_DATE_TIME != null //closed
                                         where detail.TICKET_ID == ticketNo && approve.IS_APPROVED!=0&&detail.ACTION_BY_USERID != null && detail.ACTION_DATE_TIME != null //closed
                                         select new
                                         {
                                             DetailActionBy = detail.ACTION_BY_USERID,
                                             ReplyByRoleId = dbContext.UM_User_Master.FirstOrDefault(u => u.UserID == detail.ACTION_BY_USERID).DefaultRoleID,
                                             ForwardedTo = dbContext.UM_Role_Master.FirstOrDefault(a => a.RoleID == detail.FORWARDED_TO_ROLEID).RoleName,
                                             Reply = detail.ACTION_TAKEN,
                                             ReplyDate = detail.ACTION_DATE_TIME,
                                             CurrentStatus = detail.CURRENT_STATUS
                                             //FilesUrls = dbContext.TKT_TICKET_FILES.Any(s => s.TICKET_FILE_TYPE == "R" && s.TICKET_FILE_NAME.Contains(FileNameStarts)) ? dbContext.TKT_TICKET_FILES.Where(s => s.TICKET_FILE_TYPE == "R" && s.TICKET_FILE_NAME.Contains(FileNameStarts)).Select(f => f.TICKET_FILE_NAME) : null,
                                         }).ToList();

                    var replymodelListWithFileName = relymodelList.Select(s => new ViewReplyModel
                    {
                        //added by rohit on 30-08-2022 change for 'directorit' login
                       
                        //ReplyBy = dbContext.UM_Role_Master.FirstOrDefault(r=>r.RoleID==s.ReplyByRoleId).RoleName,
                        //ForwardedTo = s.ForwardedTo,
                        ReplyBy = s.ReplyByRoleId == 25 ? "Director IT" : dbContext.UM_Role_Master.FirstOrDefault(r => r.RoleID == s.ReplyByRoleId).RoleName,
                        ForwardedTo = s.ForwardedTo.Equals("mord") ? "Director IT" : s.ForwardedTo,

                        Reply = s.Reply,
                        ReplyDate = s.ReplyDate,
                        FilesUrls = dbContext.TKT_TICKET_FILES.Any(f => f.TICKET_UPLOADED_BY == s.DetailActionBy && f.TICKET_ID == ticketNo && f.TICKET_FILE_TYPE == "R") ? dbContext.TKT_TICKET_FILES.FirstOrDefault(x => x.TICKET_UPLOADED_BY == s.DetailActionBy && x.TICKET_ID == ticketNo && x.TICKET_FILE_TYPE == "R").TICKET_FILE_NAME : "",
                        Status = GetStatus(s.CurrentStatus)
                    }).ToList();

                    model.listReplymodel = replymodelListWithFileName.Select(s => new ViewReplyModel
                    {
                        ReplyBy = s.ReplyBy,
                        ForwardedTo = s.ForwardedTo,
                        Reply = s.Reply,
                        ReplyDate = s.ReplyDate,
                        FilesUrls = s.FilesUrls != "" ? "<a href='/Ticket/GetTicketFile?id=" + URLEncrypt.EncryptParameters1(new String[] { "TKTFile=" + s.FilesUrls }) + "' title='Click here to ticket details' class='' target=_blank>" + s.FilesUrls + "</a>" : null,
                        Status=s.Status
                    }).ToList();
                 

                    //get approve autority comment
                    TKT_TICKET_APPROVAL approvemodel = dbContext.TKT_TICKET_APPROVAL.FirstOrDefault(s => s.TICKET_ID == ticketNo);

                    if (approvemodel != null)
                    {
                        // added by rohit borse on 20-07-2022 for showing in internal forwarding -- forwarded to 
                        short? getRoleIdForForwardedNames = dbContext.TKT_TICKET_DETAIL.Where(s => s.TICKET_ID == ticketNo && s.CURRENT_STATUS != 4).Select(s => s.FORWARDED_TO_ROLEID).First() == null ? 0 : dbContext.TKT_TICKET_DETAIL.Where(s => s.TICKET_ID == ticketNo && s.CURRENT_STATUS != 4).Select(s => s.FORWARDED_TO_ROLEID).First();

                    ViewReplyModel replymodel = new ViewReplyModel
                                                  {
                                                      ReplyBy = dbContext.UM_Role_Master.FirstOrDefault(s => s.RoleID == approvemodel.UM_User_Master.DefaultRoleID).RoleName + "- Ticket Admin",
                                                      Reply = approvemodel.REMARKS,
                                                      // changed by rohit borse on 20-07-2022
                                                      //ForwardedTo = approvemodel.PENDING_AT_ROLEID == null ? "-" : dbContext.UM_Role_Master.FirstOrDefault(s => s.RoleID == approvemodel.PENDING_AT_ROLEID).RoleName,
                                                      ForwardedTo = getRoleIdForForwardedNames == 25 ? "Director IT" :dbContext.UM_Role_Master.FirstOrDefault(s => s.RoleID == getRoleIdForForwardedNames).RoleName,
                                                      ReplyDate = approvemodel.APPROVAL_DATE_TIME,
                                                      FilesUrls = null,
                                                      Status = GetStatus(dbContext.TKT_TICKET_DETAIL.FirstOrDefault(s => s.TICKET_ID == ticketNo).CURRENT_STATUS) // status when aprooving authority accept the ticket
                                                  };
                        model.listReplymodel.Insert(0, replymodel);
                    }
                    model.listReplymodel.Reverse();

                return model;
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "TicketingDAL.GetTicketReplyDetailDAL()");
                return null;
            }
            finally
            {
                if (dbContext != null)
                    dbContext.Dispose();
            }
        }
    

        public bool SaveTicketReplyDetailsDAL(TicketReplyModel model,out string message)
        {
 	      dbContext = new PMGSYEntities();
            List<SelectListItem> lstCategory = new List<SelectListItem>();
            try
            {
                string[] encparam = model.TicketNo.Split('/');
                Dictionary<String, String> decParams = URLEncrypt.DecryptParameters1(new String[] { encparam[0], encparam[1], encparam[2] });
                  if(decParams.Count>0)
                  {
                     int TicketNumber = Convert.ToInt32(decParams["TicketNo"]);
                   using(var scope = new TransactionScope())
	                {
		             TKT_TICKET_DETAIL tktdetailObj = new TKT_TICKET_DETAIL();
                        //int MaxStatusId = dbContext.TKT_TICKET_DETAIL.Where(t=>t.TICKET_ID==TicketNumber).Max(x=>x.CURRENT_STATUS);

                        // added by rohit borse on 20-07-2022 if forwraded to Ticket Generator get dynamic role id check
                        if (model.CurrentStatus == 4)
                        {
                            model.ForwardTo = null;                            
                        }                        
                        string TicketGeneratorRoletype = dbContext.TKT_ROLE_TYPE_FORWARDING.Where(s => s.ROLE_ID == model.ForwardTo).Any() ? dbContext.TKT_ROLE_TYPE_FORWARDING.Where(s => s.FORWARD_TO_ROLE_ID == model.ForwardTo).Select(s => s.ROLE_TYPE).First() : null;

                        // changed by rohit borse on 20-07-2022
                        //List<int> ForwardingAuthority = new List<int> { 9, 14, 16, 25, 66, 67, 2,39,99 };  // CQC,Director F&A,Technical Director,mord(ommasdteam dual role),Ommas Team,Director  Progress,SRRDA,NRRDA,ticket generator

                        List<int?> ForwardingAuthority = dbContext.TKT_ROLE_TYPE_FORWARDING.Select(s => s.FORWARD_TO_ROLE_ID).ToList();

                    if (model.CurrentStatus != 4)
                    {
                        // change by rohit borse on 20-07-2022
                        //if (!ForwardingAuthority.Contains(model.ForwardTo))
                        if (!ForwardingAuthority.Contains(model.ForwardTo))
                        {
                            message = "Invalid Forwarding autority is selected.";
                            return false;
                        }
                    }
                    //if(model.CurrentStatus <= MaxStatusId )
                    //{
                    //    message="Ticket status is earlier than current status.";
                    //   return false;
                    //}

                    if(model.CurrentStatus > 4  )
                    {
                      message="Ticket status is invalid.";
                       return false;
                    }
                    if(model.CurrentStatus < 1  )
                    {
                      message="Ticket status is invalid.";
                       return false;
                    }

                    tktdetailObj.DETAIL_ID = dbContext.TKT_TICKET_DETAIL.Any() ? dbContext.TKT_TICKET_DETAIL.Max(x => x.DETAIL_ID) + 1 : 1;
                    tktdetailObj.TICKET_ID = TicketNumber;

                    if (model.CurrentStatus != 4) //if not closed
                    {
                        tktdetailObj.FORWARDED_BY_USERID = PMGSYSession.Current.UserId;
                        tktdetailObj.FORWARDED_DATE_TIME = DateTime.Now;

                        // change by rohit borse on 20-07-2022
                        if (model.ForwardTo == -1) //generator                       
                        {
                            int userId = dbContext.TKT_TICKET_MASTER.FirstOrDefault(s => s.TICKET_ID == TicketNumber).REPORTED_USERID;
                            int roleId = dbContext.UM_User_Master.FirstOrDefault(s => s.UserID == userId).DefaultRoleID;
                            tktdetailObj.FORWARDED_TO_ROLEID = (short)roleId;
                        }
                        else
                        {
                            tktdetailObj.FORWARDED_TO_ROLEID = (Int16)model.ForwardTo; //if 4 closed rejeted then leave null;
                        }
                    }

                   
                    tktdetailObj.ACTION_BY_USERID = PMGSYSession.Current.UserId;  
                    tktdetailObj.ACTION_TAKEN = model.TicketReply;
                    tktdetailObj.ACTION_DATE_TIME = DateTime.Now;
                    tktdetailObj.CURRENT_STATUS = model.CurrentStatus;//1=opened,2=in-progress,3=partial closed,4=closed
                    tktdetailObj.INSERTED_DATETIME = DateTime.Now;
                    tktdetailObj.INSERTED_USERID = PMGSYSession.Current.UserId;
                    dbContext.TKT_TICKET_DETAIL.Add(tktdetailObj);

                    //added by rohit borse on 14-07-2022                       
                    TKT_TICKET_MASTER tktMasterObj = new TKT_TICKET_MASTER();
                    tktMasterObj = dbContext.TKT_TICKET_MASTER.Where(t => t.TICKET_ID == TicketNumber).FirstOrDefault();
                    tktMasterObj.CLOSING_MAST_CATEGORY_CODE = model.Category;
                    dbContext.Entry(tktMasterObj).State = System.Data.Entity.EntityState.Modified;
                        
                    TKT_TICKET_APPROVAL apporveModel = dbContext.TKT_TICKET_APPROVAL.FirstOrDefault(x=>x.TICKET_ID==TicketNumber);
                    dbContext.TKT_TICKET_APPROVAL.Attach(apporveModel);
                    apporveModel.STATUS = model.CurrentStatus;

                    if (model.CurrentStatus != 4)
                    {
                        // change by rohit borse on 20-07-2022
                        if (model.ForwardTo == -1) //generator                    
                        {
                            int userId = dbContext.TKT_TICKET_MASTER.FirstOrDefault(s => s.TICKET_ID == TicketNumber).REPORTED_USERID;
                            int roleId = dbContext.UM_User_Master.FirstOrDefault(s => s.UserID == userId).DefaultRoleID;
                            apporveModel.PENDING_AT_ROLEID = (short)roleId;
                        }
                        else
                        {
                            apporveModel.PENDING_AT_ROLEID = (short)model.ForwardTo; // if not closed then enter forward id else NULL
                        }
                    }
                    if (model.CurrentStatus == 4)
                    {
                        apporveModel.PENDING_AT_ROLEID = null;   // if not closed then enter forward id else NULL
                        apporveModel.CLOSED_BY_USERID = PMGSYSession.Current.UserId;  
                        apporveModel.CLOSED_DATE_TIME = DateTime.Now;
                    }

                    if(model.ReplyFile != null)
                    {
                    string ext = model.ReplyFile.FileName.Substring(model.ReplyFile.FileName.LastIndexOf('.') + 1).Trim().ToLower();

                    String Basepath = ConfigurationManager.AppSettings["TICKET_UPLOAD_MAIN"].ToString();
                    if (!Directory.Exists(Basepath))
                        Directory.CreateDirectory(Basepath);  //if Directory not created creat it;

                    TKT_TICKET_FILES fileObj = new TKT_TICKET_FILES();
                    fileObj.TICKET_FILE_ID = dbContext.TKT_TICKET_FILES.Any() ? dbContext.TKT_TICKET_FILES.Max(x => x.TICKET_FILE_ID) + 1 : 1;
                    fileObj.TICKET_FILE_NAME = "RPL_" + TicketNumber+"_"+fileObj.TICKET_FILE_ID + "_" + DateTime.Now.ToString("dd_MM_yyyy") + "." + ext;
                    fileObj.TICKET_FILE_TYPE = "R"; //replied
                    fileObj.TICKET_FILE_UPLOAD_DATE = DateTime.Now;
                    fileObj.TICKET_ID = TicketNumber;
                    fileObj.TICKET_UPLOADED_BY = PMGSYSession.Current.UserId;
                    dbContext.TKT_TICKET_FILES.Add(fileObj);

                     //==
                     model.ReplyFile.SaveAs(System.IO.Path.Combine(Basepath, fileObj.TICKET_FILE_NAME));  //save file
                     //==
                   }
                    dbContext.SaveChanges();
                    scope.Complete();
                    message = "Reply details saved successfully";
                    return true;
	                }
                 }
                else
                  {
                   message = "Invalid ticket number.";
                    return true;
                  }
               
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "TicketingDAL.SaveTicketReplyDetailsDAL()");
                message="Error occured while processing your request";
                return false;
            }
            finally
            {
               if (dbContext != null)
                 dbContext.Dispose();
            }
        }

                 
        public void GetReportStatisticsDAL(TicketReportViewModel model)
        {
            dbContext = new PMGSYEntities();
            try
            {
                USP_TKT_RPT_GET_TICKET_COUNT_Result Dbmodel = null;
                if (model.Level == null) //first
                {
                    Dbmodel = dbContext.USP_TKT_RPT_GET_TICKET_COUNT(0, 0, 0, 0, 0, 0).FirstOrDefault();                    
                }
                else
                {
                    Dbmodel = dbContext.USP_TKT_RPT_GET_TICKET_COUNT(Convert.ToInt32(model.Level), model.state, model.Designation, model.Category, model.ModuleID, PMGSYSession.Current.UserId).FirstOrDefault();
                }

                // need to greater and equal on 10-08-2022
                if (Dbmodel.TOTAL_TICKETS.Value >= 0)
                {
                    model.TotalTicket = Dbmodel.TOTAL_TICKETS.Value + "";
                    model.TotalApprovedTicket = Dbmodel.TOTAL_TICKETS_APPROVED.Value + "";
                    model.TotalClosedTicket = Dbmodel.TOTAL_TICKETS.Value > 0
                                              ? Dbmodel.TOTAL_TICKETS_CLOSED.Value + "<br/><small>(" + Math.Round(((double)Dbmodel.TOTAL_TICKETS_CLOSED.Value / (double)Dbmodel.TOTAL_TICKETS.Value) * 100, 2) + " %)</small>"
                                              : "0";
                }
                model.TotalNotClosedTicket = Dbmodel.TOTAL_TICKETS_NOT_CLOSED.Value + "";

                // need to greater and equal on 10-08-2022
                if (Dbmodel.TOTAL_TICKETS_NOT_CLOSED.Value >= 0)
                {
                    model.PartialClosedTicket = Dbmodel.TOTAL_TICKETS_PARTIALLY_CLOSED.Value + "";
                    model.InProgressTicket = Dbmodel.TOTAL_TICKETS_INPROGRESS.Value + "";
                    model.OpenedTicket = Dbmodel.TOTAL_TICKETS_OPENED.Value + "";
                    model.NotOpenedTicket = Dbmodel.TOTAL_TICKETS_NOT_OPENED.Value + "";
                }
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "TicketingDAL.SaveTicketReplyDetailsDAL()");
            }
            finally
            {
                if (dbContext != null)
                    dbContext.Dispose();
            }
        }

        public Array GetTicketReportListDAL(int? page, int? rows, string sidx, string sord, out long totalrecords, string filters, string Level, String Designation, String Category, String Module,String State)
        {
            dbContext = new PMGSYEntities();
            SearchJsonString test = new SearchJsonString();

            int roleCode = Convert.ToInt32(Designation);
           int levelId = Convert.ToInt32(Level);
           int categoryid = Convert.ToInt32(Category);
           int moduleId = Convert.ToInt32(Module);
           int stateCode=0;
            if(!String.IsNullOrEmpty(State))
                stateCode=Convert.ToInt32(State);

            string ticketno = string.Empty;
            string state = string.Empty;
            string category = string.Empty;
            string module = string.Empty;
            string subject = string.Empty;
            string reportedby = string.Empty;
            string reporteddate = string.Empty;
            string apprvoaldate = string.Empty;
            string firstfwdto = string.Empty;
            string status = string.Empty;
            string curpendingAt = string.Empty;
            string PendingSince = string.Empty;
            List<USP_TKT_RPT_GET_ALL_TKTLIST_1_Result> lstAllrecords = null;
            try
            {

               
                if (filters != null)
                {
                    var js = new JavaScriptSerializer();
                    test = js.Deserialize<SearchJsonString>(filters);

                    foreach (SearchRules item in test.rules)
                    {
                        switch (item.field)
                        {
                            case "State": state = item.data;
                                break;
                            case "TicketNo": ticketno = item.data;
                                break;
                            case "Category": category = item.data;
                                break;
                            case "Module": module = item.data;
                                break;
                            case "Subject": subject = item.data;
                                break;
                            case "ReportedBy": reportedby = item.data;
                                break;
                            case "ReportedDate": reporteddate = item.data;
                                break;
                            case "ApprovalDate": apprvoaldate = item.data;
                                break;
                            case "ForwardTo": firstfwdto = item.data;
                                break;
                            case "Status": status = item.data;
                                break;
                            case "PendingAt": curpendingAt = item.data;
                                break;
                            case "Pendingdays":PendingSince = item.data;
                                 break;
                            default:
                                break;
                        }
                    }
                }



                lstAllrecords = dbContext.USP_TKT_RPT_GET_ALL_TKTLIST_1(levelId,stateCode,roleCode,categoryid,moduleId,PMGSYSession.Current.UserId,0).ToList();



                lstAllrecords = lstAllrecords.Where(x => x.STATE_NAME.ToLower().Contains(state.Equals(string.Empty) ? "" : state.ToLower()) &&
                                                                     //x.NOT_CLOSED_FOR_DAYS.ToString().Contains(PendingSince.Equals(string.Empty) ? "" : PendingSince) &&
                                                                     x.TICKET_NO.ToString().Contains(ticketno.Equals(string.Empty) ? "" : ticketno) &&
                                                                     x.TKT_CATEGORY.ToLower().Contains(category.Equals(string.Empty) ? "" : category.ToLower()) &&
                                                                     x.TKT_MODULE.ToLower().Contains(module.Equals(string.Empty) ? "" : module.ToLower()) &&
                                                                     x.REPORTED_SUBJECT.ToLower().Contains(subject.Equals(string.Empty) ? "" : subject.ToLower()) &&
                                                                     x.REPORTED_BY.ToLower().Contains(reportedby.Equals(string.Empty) ? "" : reportedby.ToLower()) &&
                                                                     x.FIRST_FORWARDED_TO.ToLower().Contains(firstfwdto.Equals(string.Empty) ? "" : firstfwdto.ToLower()) &&
                                                                     x.CURRENTLY_PENDING_AT.ToLower().Contains(curpendingAt.Equals(string.Empty) ? "" : curpendingAt.ToLower()) &&
                                                                     x.TKT_STATUS.ToLower().Contains(status.Equals(string.Empty) ? "" : status.ToLower())
                                                                     ).OrderByDescending(x => x.TICKET_NO).ToList();

                if (!String.IsNullOrEmpty(PendingSince))
                {
                    int NotClosedFor = Convert.ToInt32(PendingSince);
                    lstAllrecords = lstAllrecords.Where(s => s.NOT_CLOSED_FOR_DAYS >= NotClosedFor).ToList();
                }

                totalrecords = lstAllrecords.Count;

                if (sidx.Trim() != String.Empty)
                {
                    if (sord == "asc")
                    {
                        switch (sidx)
                        {
                            case "TicketNo":
                                lstAllrecords = lstAllrecords.OrderBy(s => s.TICKET_NO).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                                break;
                            default:
                                lstAllrecords = lstAllrecords.OrderBy(s => s.TICKET_NO).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                                break;
                        }
                    }
                    else
                    {
                        switch (sidx)
                        {
                            case "TicketNo":
                                lstAllrecords = lstAllrecords.OrderByDescending(s => s.TICKET_NO).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                                break;
                            default:
                                lstAllrecords = lstAllrecords.OrderByDescending(s => s.TICKET_NO).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                                break;
                        }
                    }
                }

                var FinalResult = lstAllrecords.Select(x => new
                {
                    TicketNo = x.TICKET_NO,
                    StateName = x.STATE_NAME,
                    Category = x.TKT_CATEGORY,
                    Module = x.TKT_MODULE,
                    Subject = x.REPORTED_SUBJECT,
                    ReportedBy = x.REPORTED_BY,
                    ReportedDate = x.REPORTED_DATE_TIME,
                    ApprovalDate = x.APPROVAL_DATE_TIME,
                    Status = x.TKT_STATUS,
                    PendingAt = x.CURRENTLY_PENDING_AT,
                    FirstForwardTo = x.FIRST_FORWARDED_TO,
                    ClosingDate = x.CLOSED_DATE_TIME,
                    PendingDays = x.NOT_CLOSED_FOR_DAYS
                }).Reverse().ToList();


                return FinalResult.Select(s => new
                {
                    id = s.TicketNo,
                    cell = new[]
                   {  
                    s.TicketNo.ToString(),
                    s.StateName,
                    s.Category,
                    s.Module,
                    s.Subject,
                    s.ReportedBy,
                    s.ReportedDate.ToString("dd/MM/yyyy hh:mm tt"),
                    s.ApprovalDate==null ? "-" : s.ApprovalDate.Value.ToString("dd/MM/yyyy hh:mm tt"),
                    s.FirstForwardTo == null ? "-" : s.FirstForwardTo.ToLower().Equals("mord") ? "Director IT" : s.FirstForwardTo,
                    s.Status ,
                    s.PendingAt == null ? "-" : s.PendingAt.ToLower().Equals("mord") ? "Director IT" : s.PendingAt,
                    s.ClosingDate==null ? "-" : s.ClosingDate.Value.ToString("dd/MM/yyyy hh:mm tt"),
                    s.PendingDays.ToString(),
                    "<center><a href='#' class='ui-icon ui-icon-zoomin  ui-align-center' onclick='ViewTicket(\"" +URLEncrypt.EncryptParameters1(new String[]{ "TicketNo=" +s.TicketNo })+ "\");return false;'>View </a></center>",
                  }
                }).ToArray();
                 
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "TicketDAL.GetTicketReportListDAL()");
                totalrecords = 0;
                return null;
            }
            finally
            {
                if (dbContext != null)
                    dbContext.Dispose();
            }
        }

        // added by rohit borse on 14-07-2022
        public string GetForwardToDetailsDAL(int moduleId)
        {
            try
            {
                dbContext = new PMGSYEntities();

                int? RoleId = dbContext.MASTER_MODULE.Where(m => m.MAST_MODULE_CODE == moduleId).Select(m => m.FORWARD_TO_ROLE_ID).First();
                if (RoleId != null)
                {
                    string forwardTo = dbContext.UM_Role_Master.Where(r => r.RoleID == RoleId).Select(r => r.RoleName).First();
                    return forwardTo;
                }
                return null;
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "TicketDAL.GetForwardToDetailsDAL()");
                return null;
            }
        }
    }
}