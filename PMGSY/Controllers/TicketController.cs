/*----------------------------------------------------------------------------------------

 * Project Id:

 * Project Name:OMMAS2

 * File Name: TicketController.cs

 * Author : Pradip Patil

 * Creation Date :09-02-2018

 * Desc : Ticketing functionality
 * ---------------------------------------------------------------------------------------*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PMGSY.Models.Ticket;
using PMGSY.BAL.Ticket;
using PMGSY.Common;
using System.IO;
using System.Configuration;
using System.Transactions;
using PMGSY.Models;
using PMGSY.Extensions;

namespace PMGSY.Controllers
{
    [RequiredAuthentication]
    [RequiredAuthorization]
    public class TicketController : Controller
    {
        Dictionary<String,String> dectrypedParams;
        ITicketBAL objTicketBAL;
        // GET: /Ticket/
     #region Data Entry 09-02-2018
        [NonAction]
        private List<Int32> GetActivityUserRole(Operation operation)
        {   
            List<int>  roles =null;
            if (operation == Operation.TicketEntry)
            {
                roles = new List<int> { 22,2,3,15,7,6,8,36,21,26,39,58}; //piu/srrda/sta/pta/sqm/nqm/sqc/ac-piu/acc-srrda/Auth sign/NRRDA
            }
            else if (operation == Operation.Approval)
            { 
            
            }
            else if (operation == Operation.Reply)
            { 
            
            }
            return roles;
        }
        [HttpGet]
        public ActionResult TicketingLayout()
        {
            objTicketBAL = new TicketingBAL();
            ViewData["RoleType"] = objTicketBAL.GetUserRoleInfo();

            //added by rohit on 30-08-2022 // to show list if role MORD then only for Director IT (mord) user
            if (PMGSYSession.Current.RoleCode == 25 && PMGSYSession.Current.UserId != 6892)
            {
                ViewData["RoleType"] = "N";
            }
            return View();
        }

        [HttpPost]
        public JsonResult GetTicketList(int? page, int? rows, String sidx, String sord,string filters)
        {
            objTicketBAL = new TicketingBAL();
            long totalrecords;
            try
            {
                var JsonData = new
                {
                    rows = objTicketBAL.GetTicketListBAL(page - 1, rows, sidx, sord, out totalrecords,filters),
                    total = totalrecords <= Convert.ToInt32(rows) ? 1 : (totalrecords % Convert.ToInt32(rows) == 0 ? totalrecords / Convert.ToInt32(rows) : totalrecords / Convert.ToInt32(rows) + 1),
                    page = Convert.ToInt32(page),
                    records = totalrecords,
                    userTyepe= objTicketBAL.GetUserRoleInfo()
                };

                //added by rohit on 30-08-2022 // to show list if role MORD then only for Director IT (mord) user
                if (PMGSYSession.Current.RoleCode == 25 && PMGSYSession.Current.UserId != 6892)
                {
                    var JsonDataList = new
                    {
                        rows = 0,//objTicketBAL.GetTicketListBAL(page - 1, rows, sidx, sord, out totalrecords, filters),
                        total = 0,//totalrecords <= Convert.ToInt32(rows) ? 1 : (totalrecords % Convert.ToInt32(rows) == 0 ? totalrecords / Convert.ToInt32(rows) : totalrecords / Convert.ToInt32(rows) + 1),
                        page = 0,
                        records = 0,
                        userTyepe = "N"
                    };
                    return Json(JsonDataList);
                }

                return Json(JsonData);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "TicketController.GetTicketList()");
                return null;
            }
        }



        [HttpPost]
        public JsonResult GetAllTicketList(int? page, int? rows, String sidx, String sord, string filters)
        {
            objTicketBAL = new TicketingBAL();
            long totalrecords;
            try
            {
                var JsonData = new
                {
                    rows = objTicketBAL.GetALLTicketListBAL(page - 1, rows, sidx, sord, out totalrecords, filters),
                    total = totalrecords <= Convert.ToInt32(rows) ? 1 : (totalrecords % Convert.ToInt32(rows) == 0 ? totalrecords / Convert.ToInt32(rows) : totalrecords / Convert.ToInt32(rows) + 1),
                    page = Convert.ToInt32(page),
                    records = totalrecords,
                    userTyepe = objTicketBAL.GetUserRoleInfo()
                };
                //added by rohit on 30-08-2022 // to show list if role MORD then only for Director IT (mord) user
                if (PMGSYSession.Current.RoleCode == 25 && PMGSYSession.Current.UserId != 6892)
                {
                    var JsonDataList = new
                    {
                        rows = 0,//objTicketBAL.GetTicketListBAL(page - 1, rows, sidx, sord, out totalrecords, filters),
                        total = 0,//totalrecords <= Convert.ToInt32(rows) ? 1 : (totalrecords % Convert.ToInt32(rows) == 0 ? totalrecords / Convert.ToInt32(rows) : totalrecords / Convert.ToInt32(rows) + 1),
                        page = 0,
                        records = 0,
                        userTyepe = "N"
                    };
                    return Json(JsonDataList);
                }

                return Json(JsonData);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "TicketController.GetTicketList()");
                return null;
            }
        }


        // changed by rohit borse on 14-07-2022
        [HttpGet]
        public ViewResult AddTicket()
        {
            objTicketBAL = new TicketingBAL();
            TicketViewModel model = new TicketViewModel();
            try
            {
                model.LstModule = objTicketBAL.GetModule();

                // changed by rohit borse on 14-07-2022
                // model.lstCategory = objTicketBAL.GetCategory();

                List<SelectListItem> listCategory = new List<SelectListItem>();
                listCategory = objTicketBAL.GetCategory();
                listCategory.Remove(listCategory.Find(s=>s.Value == "5")); 
                listCategory.Remove(listCategory.Find(s => s.Value == "6"));
                model.lstCategory = listCategory;
                return View(model);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "TicketController.AddTicket()");
                return View(model);
            }

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult AddTicketDetails(TicketViewModel model)
        {
            String message = String.Empty;
            Boolean Status = false;
            try
            {
                objTicketBAL = new TicketingBAL();
                if (ModelState.IsValid)
                {
                    Status = objTicketBAL.SaveTicketDetailsBAL(model);
                }
                if(Status)
                  return Json(new { success = true,message="Ticket details saved successfully."});
               return Json(new { success = false });
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "TicketController.GetTicketList()");
                return Json(new { success = false});
            }
        }

        [HttpGet]
        public ViewResult TicketFileUpload(String parameter, String hash, string key)
        {
            try
            {
                dectrypedParams = URLEncrypt.DecryptParameters1(new String[] { parameter , hash , key });
                if (dectrypedParams.Count > 0)
                {
                    ViewBag.TicketNo = parameter+"/"+hash+"/"+key;
                    
                }
                return View();

            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "TicketController.TicketFileUpload()");
                return null;
            }
            
        }

        [HttpPost]
        public JsonResult GetTicketFileList(int? page, int? rows, String sidx, String sord, string TicketNo)
        {
            objTicketBAL = new TicketingBAL();
            Dictionary<String, String> decryptedParameters;
            long totalrecords;
            try
            {
                string[] encParam = TicketNo.Split('/');
                decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { encParam[0], encParam[1], encParam[2] });
                int tktNo = Convert.ToInt32(decryptedParameters["TicketNo"]);

                var JsonData = new
                {
                    rows = objTicketBAL.GetTicketFileListBAL(page - 1, rows, sidx, sord, tktNo, out totalrecords),
                    total = totalrecords <= Convert.ToInt32(rows) ? 1 : (totalrecords % Convert.ToInt32(rows) == 0 ? totalrecords / Convert.ToInt32(rows) : totalrecords / Convert.ToInt32(rows) + 1),
                    page = Convert.ToInt32(page),
                    records = totalrecords,
                    userTyepe = objTicketBAL.GetUserRoleInfo()
                };
                return Json(JsonData);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "TicketController.GetTicketFileList()");
                return null;
            }
        }



        [HttpPost]
        public JsonResult UploadTicketFile(FormCollection formCollection)
        {
            CommonFunctions commonFunction = new CommonFunctions();
            Dictionary<String, String> decryptedParameters = null;
            String ticketNo = formCollection["ticketNo"] as String;
            Int32 ticketNumber;
            Boolean status = false;
            String message =string.Empty;
            ITicketBAL ticketBAL = new TicketingBAL();
            try
            {
                string[] encParam = ticketNo.Split('/');
                decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { encParam[0], encParam[1], encParam[2] });
               
                if (Request.Files.Count == 0)
                {
                    return Json(new { success = false, message = "No file selected. Please select file" });
                }
                else
                {
                    String[] fileTypes = new String[] { "pdf", "jpg", "jpeg", "png" };
                    HttpPostedFileBase postedFile = Request.Files[0];

                    if (postedFile.FileName.Split('.').Length > 2)
                    {
                        return Json(new { success = false, message = "invalid file name. Please upload only valid file." });
                    
                    }

                    string ext = postedFile.FileName.Substring(postedFile.FileName.LastIndexOf('.') + 1).Trim().ToLower();

                    int maxSize = 1024 * 1024 * 4;
                    if (!fileTypes.Contains(ext))
                    {
                        return Json(new { success = false, message = "invalid file. (."+ext+") files are not allowed." });
                    }

                    if (postedFile.ContentLength > maxSize)
                    {
                        return Json(new { success = false, message = "invalid file size. Please upload file upto 4 MB." });
                    }

                    if (decryptedParameters.Count > 0)
                    {
                        ticketNumber = Convert.ToInt32(decryptedParameters["TicketNo"]);
                        status = ticketBAL.SaveTicketFile(ticketNumber, postedFile, out message);
                    }
                    return Json(new { success = status, message = message });
                }
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "TicketController.UploadTicketFile()");
                return Json(new { message="error occured while processing your request."});
            }

        }


        public FileContentResult GetTicketFile(string id)
        {
            try
            {
                String FileName = String.Empty;
                string[] encParam = id.Split('/');
                dectrypedParams = URLEncrypt.DecryptParameters1(new String[] { encParam[0], encParam[1], encParam[2] });
                if (dectrypedParams.Count > 0)
                {
                    FileName = dectrypedParams["TKTFile"];
                }
                String Path = ConfigurationManager.AppSettings["TICKET_UPLOAD_MAIN"].ToString();

                // inline change due to file view rather than download
                // added by rohit borse on 14-07-2022 
                //var cd = new System.Net.Mime.ContentDisposition { FileName = FileName, Inline = false };

                var cd = new System.Net.Mime.ContentDisposition { FileName = FileName, Inline = true };

                Response.AppendHeader("Content-Disposition", cd.ToString());

                return File(System.IO.File.ReadAllBytes(Path+FileName),System.Web.MimeMapping.GetMimeMapping(FileName));
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "GetTicketFile()");
                return null;
            }

        }


        public JsonResult DeleteTicketFile(string id)
        {
            PMGSYEntities dbContext = new PMGSYEntities();

            try
            {
                int FileId = 0;
                
                bool fileDeleted = false;
                string message = string.Empty;

                FileId = id != null ? Convert.ToInt32(id) : 0;
                              
                if (FileId > 0)
                {
                    using (var scope = new TransactionScope())
                    {
                        
                        var filePresent = dbContext.TKT_TICKET_FILES.Where(F => F.TICKET_FILE_ID == FileId).Any() ? dbContext.TKT_TICKET_FILES.Where(F => F.TICKET_FILE_ID == FileId).First() : null;
                        if (filePresent != null)
                        {
                            dbContext.TKT_TICKET_FILES.Remove(filePresent);
                            int DeleteCount =  dbContext.SaveChanges();                            
                            scope.Complete();

                            String DirectoryPath = ConfigurationManager.AppSettings["TICKET_UPLOAD_MAIN"].ToString();
                            string path = Path.Combine(DirectoryPath + filePresent.TICKET_FILE_NAME);

                            FileInfo file = new FileInfo(path);
                            if (file.Exists) //check file exsit or not  
                            {
                                file.Delete();
                                message = "File deleted successfully";
                                fileDeleted = true;
                            }
                            else
                            {
                                fileDeleted = false;
                                message = "This file does not exists..!!";
                            }
                        }
                    }
                    if (fileDeleted == true)
                    {
                        return Json(new { success = true, message = message },JsonRequestBehavior.AllowGet);
                    }                   
                }
                return Json(new { success = false, message = message }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "DeleteTicketFile()");
                return Json(new { success = false, message = "error occured while file deleting..." }, JsonRequestBehavior.AllowGet);
            }
            finally
            {
                if (dbContext != null)
                    dbContext.Dispose();
            }
        }

        [HttpGet]
        public ViewResult ViewTicketDetails(String parameter, String hash, string key)
        {
               objTicketBAL = new TicketingBAL();
            try
            {
                dectrypedParams = URLEncrypt.DecryptParameters1(new String[] { parameter, hash, key });
                Int32 ticketNo = 0;
                Boolean IsApproved = false;
                TicketAcceptModel model=null;
                if (dectrypedParams.Count > 0)
                {
                    ticketNo = Convert.ToInt32(dectrypedParams["TicketNo"]);
                    ViewBag.TicketNo = parameter + "/" + hash + "/" + key;
                    model = objTicketBAL.GetTicketAcceptDetailBAL(ticketNo, out IsApproved);
                    model.IsApproved = IsApproved;
                }
                return View(model);

            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "TicketController.ViewTicketDetails()");
                return null;
            }

        }

        

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult SaveApproveDetails(TicketAcceptModel model)
        {
            String message = String.Empty;
            Boolean Status = false;
            try
            {
                objTicketBAL = new TicketingBAL();

                if (model.AcceptReject == 2)
                    ModelState.Remove("ForwardTo");

                if (ModelState.IsValid)
                {
                    Status = objTicketBAL.SaveApproveDetailsBAL(model,out message);
                }
                if(Status)
                    return Json(new { success = true, message = message });
                return Json(new { success = false, message=message });
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "TicketController.SaveApproveDetails()");
                return Json(new { success = false,message="Error occured while processing your reuest."});
            }
        }


        [HttpGet]
        public ViewResult ReplyTicketDetails(String parameter, String hash, string key)
        {
               objTicketBAL = new TicketingBAL();
            try
            {
                dectrypedParams = URLEncrypt.DecryptParameters1(new String[] { parameter, hash, key });
                Int32 ticketNo = 0;
                TicketReplyModel model=null;
                if (dectrypedParams.Count > 0)
                {
                    ticketNo = Convert.ToInt32(dectrypedParams["TicketNo"]);
                    ViewBag.TicketNo = parameter + "/" + hash + "/" + key;
                    model = objTicketBAL.GetTicketReplyDetailBAL(ticketNo);
                }
                return View(model);

            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "TicketController.ReplyTicketDetails()");
                return null;
            }

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult SaveTicketReplyDetails(TicketReplyModel model)
        {
            String message = "Error occured while processing your request";
            Boolean Status = false;
            try
            {
                objTicketBAL = new TicketingBAL();

                // changed by rohit borse on 20-07-2022
                if (model.CurrentStatus == 4)
                {
                    ModelState.Remove("ForwardTo");                   
                }
                  

                if (ModelState.IsValid)
                {
                    if (Request.Files.Count > 0)
                    {
                        String[] fileTypes = new String[] { "pdf", "jpg", "jpeg", "png" };
                        HttpPostedFileBase postedFile = Request.Files[0];

                        if (postedFile.FileName.Split('.').Length > 2)
                        {
                            return Json(new { success = false, message = "invalid file. Please upload only valid file." });
                        }

                        string ext = postedFile.FileName.Substring(postedFile.FileName.LastIndexOf('.') + 1).Trim().ToLower();

                        int maxSize = 1024 * 1024 * 4;
                        if (!fileTypes.Contains(ext))
                        {
                            return Json(new { success = false, message = "invalid file. (." + ext + ") files are not allowed." });
                        }

                        if (postedFile.ContentLength > maxSize)
                        {
                            return Json(new { success = false, message = "invalid file size. Please upload file upto 4 MB." });
                        }
                    }

                    Status = objTicketBAL.SaveTicketReplyDetailsBAL(model, out message);
                    if (Status)
                        return Json(new { success = true, message = message });
                }
                return Json(new { success = false, message = message });
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "TicketController.SaveTicketReplyDetails()");
                return Json(new { success = false, message = "Error occured while processing your request" });
            }
        
        }

        //added by rohit borse on 14-07-2022
        [HttpGet]
        public JsonResult GetForwardToDetails(string id ) {
            objTicketBAL = new TicketingBAL();
            int moduleID = Convert.ToInt32(id);
            try
            {
                if (moduleID > 0)
                {
                    string data = objTicketBAL.GetForwardToDetailsBAL(moduleID);
                    if(data!=null)
                        return Json(data, JsonRequestBehavior.AllowGet);
                    else
                        return Json(new { success = false, message = "Error occured while processing your request" });
                }
                return null;
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "TicketController.GetForwardToDetails()");
                return Json(new { success = false, message = "Error occured while processing your request" });
            }          

        }

        public JsonResult DeleteTicketById(String parameter, String hash, string key)
        {
            PMGSYEntities dbContext = new PMGSYEntities();
            try
            {
                dectrypedParams = URLEncrypt.DecryptParameters1(new String[] { parameter, hash, key });
                int ticketNo = 0;
                Boolean IsApproved = false;
                TicketAcceptModel model = null;
                if (dectrypedParams.Count > 0)
                {
                    ticketNo = Convert.ToInt32(dectrypedParams["TicketNo"]);

                    if (dbContext.TKT_TICKET_MASTER.Where(x=>x.TICKET_ID == ticketNo).Any())
                    {
                        using (var scope = new TransactionScope())
                        {
                            TKT_TICKET_MASTER objtTicketmaster = new TKT_TICKET_MASTER();
                            List<TKT_TICKET_FILES> filePresentList = new List<TKT_TICKET_FILES>();


                            objtTicketmaster = dbContext.TKT_TICKET_MASTER.Where(x => x.TICKET_ID == ticketNo).First();
                            dbContext.TKT_TICKET_MASTER.Remove(objtTicketmaster);
                            //dbContext.SaveChanges();

                           filePresentList = dbContext.TKT_TICKET_FILES.Where(f=>f.TICKET_ID == ticketNo).Any() ? dbContext.TKT_TICKET_FILES.Where(a=>a.TICKET_ID == ticketNo).ToList<TKT_TICKET_FILES>() : null;
                            if (filePresentList != null)
                            {
                                String DirectoryPath = ConfigurationManager.AppSettings["TICKET_UPLOAD_MAIN"].ToString();

                                foreach (var item in filePresentList)
                                {
                                    dbContext.TKT_TICKET_FILES.Remove(item);

                                    
                                    string path = Path.Combine(DirectoryPath + item.TICKET_FILE_NAME);

                                    FileInfo file = new FileInfo(path);
                                    if (file.Exists) //check file exsit or not  
                                    {
                                        file.Delete();
                                    }
                                }                                
                                dbContext.SaveChanges();                                                             
                            }                            
                            scope.Complete();
                        }
                        return Json(new { success = true, message = "Record deleted successfully.. " }, JsonRequestBehavior.AllowGet);
                    }
                }

                return Json(new { success = false, message = "Record cannot delete..!!" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "TicketController.DeleteTicketById()");
                return Json(new { success = false, message = "Error occured while processing your request" }, JsonRequestBehavior.AllowGet);
            }
            finally 
            {
                if (dbContext != null)
                    dbContext.Dispose();
            }
        }

        #endregion

        #region Report  23/02/2018

        [HttpGet]
        public ViewResult TicketingReportLayout()
        {
            try
            {

                CommonFunctions commonObj = new CommonFunctions();
                objTicketBAL = new TicketingBAL();
                TicketReportViewModel model = new TicketReportViewModel();
                model.LstModule = objTicketBAL.GetModule();
                model.LstModule.FirstOrDefault(s => s.Value == "0").Text = "All";
                model.lstCategory = objTicketBAL.GetCategory();
                model.lstCategory.FirstOrDefault(s => s.Value == "0").Text = "All";
                model.stateList = commonObj.PopulateStates(false);
                model.DesignationList = new List<SelectListItem> { new SelectListItem { Value = "0", Text = "All" } };
                objTicketBAL.GetReportStatisticsBAL(model);
                model.Level = "0";


                //added by rohit on 30-08-2022 // to show dashboard if role MORD then only for Director IT (mord) user
                if (PMGSYSession.Current.RoleCode == 25 && PMGSYSession.Current.UserId != 6892)
                {
                    model.LstModule.Insert(0, new SelectListItem { Value = "0", Text = "Select Module" });
                    model.lstCategory.Insert(0, new SelectListItem { Value = "0", Text = "All" });
                    model.stateList.Insert(0, new SelectListItem { Value = "0", Text = "Select State" });
                    model.DesignationList.Insert(0, new SelectListItem { Value = "0", Text = "Select Designation" });
                    model.TotalTicket = "0";
                    model.TotalApprovedTicket = "0";
                    model.TotalClosedTicket = "0";
                    model.TotalNotClosedTicket = "0";
                    model.PartialClosedTicket = "0";
                    model.InProgressTicket = "0";
                    model.OpenedTicket = "0";
                    model.NotOpenedTicket = "0";
                    model.Level = "0";
                }

                return View(model);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "TicketController.TicketingReportLayout()");
                return null;
            }
        }

        [HttpPost]
        public JsonResult GetDesignation(String Level)
        {
            List<SelectListItem> DesigntioList = new List<SelectListItem>();
            DesigntioList.Add(new SelectListItem { Value = "0", Text = "All", Selected = true });
            if (Level.Equals("1")) //state
            {
                //DesigntioList.Add(new SelectListItem { Value = "2", Text = "SRRDA" });
                //DesigntioList.Add(new SelectListItem { Value = "8", Text = "SQC" });
                DesigntioList.Add(new SelectListItem { Value = "7", Text = "SQM" });
                //DesigntioList.Add(new SelectListItem { Value = "36", Text = "ITNO" });
                //DesigntioList.Add(new SelectListItem { Value = "3", Text = "STA" });
            }
            else if (Level.Equals("2")) //national
            {
                //DesigntioList.Add(new SelectListItem { Value = "15", Text = "PTA" });
                DesigntioList.Add(new SelectListItem { Value = "6", Text = "NQM" });
                DesigntioList.Add(new SelectListItem { Value = "39", Text = "NRRDA" });
            }
            return Json(DesigntioList);
        }

        [HttpPost]
        public JsonResult GetStatisticDetails(TicketReportViewModel model)
        {
           
            try
            {
                objTicketBAL = new TicketingBAL();
                // added by rohit borse on 20-07-2022 for report on select level all validation
                if (model.Level == "0")
                {
                    model.state = 0;
                }
                objTicketBAL.GetReportStatisticsBAL(model);

                //added by rohit on 30-08-2022 // to show dashboard if role MORD then only for Director IT (mord) user
                if (PMGSYSession.Current.RoleCode == 25 && PMGSYSession.Current.UserId != 6892)
                {                    
                    model.TotalTicket = "0";
                    model.TotalApprovedTicket = "0";
                    model.TotalClosedTicket = "0";
                    model.TotalNotClosedTicket = "0";
                    model.PartialClosedTicket = "0";
                    model.InProgressTicket = "0";
                    model.OpenedTicket = "0";
                    model.NotOpenedTicket = "0";                  
                }

                return Json(model);
            }
            catch (Exception ex)
            {
               ErrorLog.LogError(ex, "TicketController.GetStatisticDetails()");
                return null;
            }
        }

        [HttpPost]
        public JsonResult GetTicketReportList(int? page, int? rows, String sidx, String sord, string filters, string Level, String Designation, String Category, String Module,String State)
        {
            objTicketBAL = new TicketingBAL();
            long totalrecords;
            try
            {
                // added by rohit borse on 20-07-2022 for report on select level all validation
                if(Level == "0")
                {
                    State = "0";
                }

                var JsonData = new
                {
                    rows = objTicketBAL.GetTicketReportListBAL(page - 1, rows, sidx, sord, out totalrecords, filters,Level,Designation,Category,Module,State),
                    total = totalrecords <= Convert.ToInt32(rows) ? 1 : (totalrecords % Convert.ToInt32(rows) == 0 ? totalrecords / Convert.ToInt32(rows) : totalrecords / Convert.ToInt32(rows) + 1),
                    page = Convert.ToInt32(page),
                    records = totalrecords,
                    userTyepe = objTicketBAL.GetUserRoleInfo()
                };

                //added by rohit on 30-08-2022 // to show dashboard if role MORD then only for Director IT (mord) user
                if (PMGSYSession.Current.RoleCode == 25 && PMGSYSession.Current.UserId != 6892)
                {
                    var JsonDetail = new
                    {
                        rows = 10,
                        total = 0,
                        page = 0,
                        records = 0,
                        userTyepe = "N"
                    };
                    return Json(JsonDetail);
                }

                return Json(JsonData);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "TicketController.GetTicketReportList()");
                return null;
            }
        }

     #endregion

    }
}
