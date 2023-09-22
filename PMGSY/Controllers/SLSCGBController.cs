using PMGSY.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PMGSY.Extensions;
using PMGSY.Models.SLSCGB;
using PMGSY.BAL.SLSCGB;
using System.Configuration;
using System.IO;
using PMGSY.Areas.MeetingReport;
using PMGSY.DAL.SLSCGB;
using PMGSY.Models;

namespace PMGSY.Controllers
{
    public class SLSCGBController : Controller
    {
        SLSCGBBAL objBAL = new SLSCGBBAL();
        //
        // GET: /SLSCGB/

        public ActionResult AddSLSCGBDetails()
        {
            CommonFunctions comm = new CommonFunctions();
            try
            {
                SLSCGBViewModel model = new SLSCGBViewModel();
                if (PMGSYSession.Current.RoleCode == 25)///MORD
                {
                    model.stateList = comm.PopulateStates(true);
                    model.stateList.Find(x => x.Value == "0").Value = "-1";
                }
                else if (PMGSYSession.Current.RoleCode == 2)///SRRDA
                {
                    model.stateList = new List<SelectListItem>();
                    model.stateList.Insert(0, new SelectListItem() { Text = PMGSYSession.Current.StateName, Value = Convert.ToString(PMGSYSession.Current.StateCode) });
                }



                return View(model);
            }
            catch (Exception ex)
            {
                return null;
            }
            finally
            {

            }
        }


        /// <summary>
        /// saves the inserted details of SLSC GB
        /// </summary>
        /// <param name="model">contains the data of Core Network details</param>
        /// <returns>response message along with status</returns>
        [HttpPost]
        [Audit]
        public ActionResult AddSLSCGB(SLSCGBViewModel model)
        {
            //FormCollection frmCollection
            //return View();
            //SLSCGBViewModel model = null;
            bool status = false;
            string message = string.Empty;

            bool flag = false;
            string[] arr = null;

            bool fileExt = false;

            HttpPostedFileBase file = null;
            string fileTypes = string.Empty;

            string fileId = string.Empty;
            int fileSize = 0;

            string filePath = string.Empty;
            string fileSaveExt = string.Empty;
            int fileValidSize = 0;
            //string status = string.Empty;

            try
            {
                if (ModelState.IsValid)
                {
                    //model = new SLSCGBViewModel();

                    //model.state = Convert.ToInt32(frmCollection["state"]);
                    //model.meetingDate = Convert.ToString(frmCollection["meetingDate"]).Trim();
                    //model.meetingFlag = Convert.ToString(frmCollection["meetingFlag"]).Trim();


                    #region File Upload
                    if (Request.Files.AllKeys.Count() > 0)
                    {
                        file = Request.Files[0];
                        if (file != null)
                        {
                            //ModelState.Add("tourReport");
                            fileTypes = ConfigurationManager.AppSettings["SLSC_GB_FILE_VALID_FORMAT"];
                            filePath = ConfigurationManager.AppSettings["SLSC_GB_FILE_UPLOAD"];
                            fileSize = Convert.ToInt32(ConfigurationManager.AppSettings["SLSC_GB_FILE_MAX_SIZE"]);

                            fileExt = true;
                            fileSaveExt = Path.GetExtension(file.FileName);//file.ContentType;
                            fileSaveExt = fileSaveExt.Contains('.') ? fileSaveExt.Trim().Remove(0, 1) : fileSaveExt;
                            fileValidSize = file.ContentLength;

                            if (fileValidSize > fileSize)
                            {
                                message = message == string.Empty ? "Pdf/doc files upto " + fileSize / (1024 * 1024) + " MB are allowed." : message;
                                return Json(new { success = false, message = message });
                            }

                            arr = fileTypes.Split('$');
                            for (int i = 0; i < arr.Length; i++)
                            {
                                if (fileSaveExt == arr[i])
                                {
                                    flag = true;
                                }
                            }
                            if (flag)
                            {
                                model.FileName = file.FileName;
                                model.fileType = fileSaveExt;

                                model.FileName = (model.meetingFlag == "S" ? "SLSC" : "GB") + "-" + DateTime.Now.Year.ToString() + DateTime.Now.Month.ToString() + DateTime.Now.Day.ToString() + DateTime.Now.Hour.ToString() + DateTime.Now.Minute.ToString() + DateTime.Now.Second.ToString() + "." + fileSaveExt;
                                if (objBAL.AddSLSCGBBAL(model, ref message))
                                {
                                    if (Request.Files.AllKeys.Count() > 0)
                                    {

                                        Request.Files[0].SaveAs(Path.Combine(filePath, model.FileName));
                                    }
                                    message = message == string.Empty ? "SLSC/GB details added successfully" : message;
                                    return Json(new { success = true, message = message }, JsonRequestBehavior.AllowGet);
                                }
                                else
                                {
                                    message = message == string.Empty ? "SLSC/GB details not saved" : message;
                                    return Json(new { success = false, message = message });
                                }
                            }
                            else
                            {
                                message = message == string.Empty ? "Invalid file format, only pdf/doc files allowed" : message;
                                return Json(new { success = false, message = message });
                            }
                        }
                        else
                        {
                            message = message == string.Empty ? "SLSC/GB details not saved, please select a pdf/doc file" : message;
                            return Json(new { success = false, message = message });
                        }
                    }
                    else
                    {
                        message = message == string.Empty ? "SLSC/GB details not saved, please select a pdf/doc file" : message;
                        return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
                    }
                    #endregion
                }
                else
                {
                    return Json(new { success = false, message = new CommonFunctions().FormatErrorMessage(ModelState) });
                }
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);

                return Json(new { success = false, message = "Error occurred while adding SLSC/GB details" });
                //return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// returns the list of Technology details
        /// </summary>
        /// <param name="page"></param>
        /// <param name="rows"></param>
        /// <param name="sidx"></param>
        /// <param name="sord"></param>
        /// <returns></returns>
        [HttpPost]
        [Audit]
        public ActionResult GetMeetingList(int? page, int? rows, string sidx, string sord)
        {
            //Added By Abhishek kamble 30-Apr-2014 start
            using (CommonFunctions commonFunction = new CommonFunctions())
            {
                if (!commonFunction.ValidateGridParameters(new GridParams(page, rows, sidx, sord, Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                {
                    return null;
                }
            }
            //Added By SAMMED PATIL 3-NOV-2016 end
            SLSCGBBAL objBAL = new SLSCGBBAL();
            long totalRecords = 0;
            int meetingCode = 0;
            try
            {
                using (CommonFunctions commonFunction = new CommonFunctions())
                {
                    if (!commonFunction.ValidateGridParameters(new GridParams(page, rows, sidx, sord, Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                    {
                        return null;
                    }
                }

                var jsonData = new
                {
                    rows = objBAL.GetMeetingListBAL(Convert.ToInt32(page) - 1, Convert.ToInt32(rows), sidx, sord, out totalRecords),
                    total = totalRecords <= Convert.ToInt32(rows) ? 1 : totalRecords / Convert.ToInt32(rows) + 1,
                    page = Convert.ToInt32(page),
                    records = totalRecords,
                };
                return Json(jsonData, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return Json(new { success = false, message = "Error occurred while processing your request." });
            }
        }

        [HttpGet]
        [Audit]
        public ActionResult DownloadMeetingFile(String parameter, String hash, String key)
        {
            try
            {
                string FileName = string.Empty;
                string FullFileLogicalPath = string.Empty;
                string FullfilePhysicalPath = string.Empty;
                string FileExtension = string.Empty;

                if (!String.IsNullOrEmpty(parameter) && !String.IsNullOrEmpty(hash) && !String.IsNullOrEmpty(key))
                {
                    String[] urlParams = URLEncrypt.DecryptParameters(new String[] { parameter, hash, key });
                    if (urlParams.Length >= 1)
                    {
                        String[] urlSplitParams = urlParams[0].Split('$');
                        FileName = (urlSplitParams[0]);
                    }
                }
                FileExtension = Path.GetExtension(FileName).ToLower();

                if (FileExtension == ".pdf" || FileExtension == ".doc" || FileExtension == ".docx")
                {
                    FullFileLogicalPath = Path.Combine(ConfigurationManager.AppSettings["SLSC_GB_FILE_UPLOAD_VIRTUAL_DIR_PATH"], FileName);
                    FullfilePhysicalPath = Path.Combine(ConfigurationManager.AppSettings["SLSC_GB_FILE_UPLOAD"], FileName);
                }

                string name = Path.GetFileName(FileName);
                string ext = Path.GetExtension(FileName);

                string type = string.Empty;

                if (ext != null)
                {
                    switch (ext.ToLower())
                    {
                        case ".pdf":
                            type = "Application/pdf";
                            break;
                        case ".doc":
                        case ".docx":
                            type = "Application/msword";
                            break;
                        case ".jpg":
                        case ".bmp":
                        case ".tiff":
                        case ".png":
                        case ".gif":
                        case ".jpeg":
                            type = "image/png";
                            break;
                        default:
                            type = "Application";
                            break;
                    }
                }

                if (System.IO.File.Exists(FullfilePhysicalPath))
                {
                    return File(FullfilePhysicalPath, type, DateTime.Now.ToShortDateString() + "_" + new Random().Next(1000000000) + FileExtension);
                }
                else
                {
                    return Json(new { Success = "false", ErrorMessage = "File Not Found." }, JsonRequestBehavior.AllowGet);
                }
            }
            catch
            {
                return Json(new { Success = "false", ErrorMessage = "Error While Processing Your Request." }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        [Audit]
        public JsonResult DeleteMeetingDetails(string parameter, string hash, string key)
        {
            SLSCGBBAL objBAL = new SLSCGBBAL();

            string status = string.Empty;

            String PhysicalPath = string.Empty;
            String ThumbnailPath = string.Empty;

            //string[] arrParam = param.Split('$');
            string fileName = string.Empty;
            int meetingCode = 0;

            Dictionary<string, string> decryptedParameters = null;

            try
            {
                decryptedParameters = URLEncrypt.DecryptParameters1(new string[] { parameter, hash, key });

                if (decryptedParameters != null)
                {
                    meetingCode = Convert.ToInt32(decryptedParameters["MeetingCode"]);
                    fileName = decryptedParameters["File_Name"];
                }

                PhysicalPath = ConfigurationManager.AppSettings["SLSC_GB_FILE_UPLOAD"];
                ThumbnailPath = Path.Combine(PhysicalPath, fileName);

                //string[] arrParam = Request.Params["IMS_PR_ROAD_CODE"].Split('$');

                //int EXEC_FILE_ID = Convert.ToInt32(arrParam[0]);
                //int IMS_PR_ROAD_CODE = Convert.ToInt32(arrParam[1]);

                PhysicalPath = Path.Combine(PhysicalPath, fileName);

                if (!System.IO.File.Exists(PhysicalPath) && !System.IO.File.Exists(ThumbnailPath))
                {
                    //return Json(new { Success = false, ErrorMessage = "file Not Found." });
                }

                status = objBAL.DeleteMeetingDetailsBAL(meetingCode);

                if (status == string.Empty)
                {
                    try
                    {
                        System.IO.File.Delete(PhysicalPath);
                        System.IO.File.Delete(ThumbnailPath);
                    }
                    catch (Exception ex)
                    {
                        Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                        return Json(new { Success = true, ErrorMessage = ex.Message });
                    }
                    return Json(new { Success = true, ErrorMessage = "There is an error while processing your request." });
                }
                else
                {
                    return Json(new { Success = false, ErrorMessage = status });
                }
            }
            catch (Exception ex)
            {
                return Json(new { Success = false, ErrorMessage = status });
            }
            finally
            { 
                
            }
        }

        #region Meeting Report
        public ActionResult MeetingDetails()
        {
            MeetingModel MasterReportViewModel = new MeetingModel();
            SLSCGBDAL objDAL = new SLSCGBDAL();
            List<SelectListItem> stateList = objDAL.PopulateStates();

            //  stateDd.Find(x => x.Value == "1").Selected = true;

            int stateCode = PMGSYSession.Current.StateCode;

            if (stateCode > 0)
            {
                //  stateDd.Find(x => x.Value == stateCode.ToString()).Selected = true;
                var dbContext = new PMGSYEntities();
                MASTER_STATE state = dbContext.MASTER_STATE.Where(s => s.MAST_STATE_CODE == stateCode).SingleOrDefault();
                stateList.Clear();
                SelectListItem item = new SelectListItem { Text = state.MAST_STATE_NAME, Value = state.MAST_STATE_CODE.ToString(), Selected = true };
                stateList.Add(item);

            }
            ViewData["STATE"] = stateList;

            SelectListItem all = new SelectListItem
            {
                Text = "All",
                Value = "%"
            };

            SelectListItem slsc = new SelectListItem
            {
                Text = "SLSC",
                Value = "S"
            };

            SelectListItem gb = new SelectListItem
            {
                Text = "GB",
                Value = "G"
            };

            List<SelectListItem> MeetingType = new List<SelectListItem>();
            MeetingType.Add(all);
            MeetingType.Add(slsc);
            MeetingType.Add(gb);

            ViewData["MEETING"] = MeetingType;

            return View(MasterReportViewModel);
        }

        public ActionResult MeetingDetailsListing(MeetingModel model)
        {
            model.level = 1;
            return View(model);
        }

        public ActionResult MeetingFileDownloadPdf(string FileName)
        {
            try
            {
                PMGSY.Models.PMGSYEntities dbContext = new PMGSY.Models.PMGSYEntities();

                string FullfilePhysicalPath = string.Empty;
                string FileExtension = string.Empty;
                string PhysicalPath = PhysicalPath = ConfigurationManager.AppSettings["SLSC_GB_FILE_UPLOAD"];

                FileExtension = Path.GetExtension(FileName).ToLower();
                FullfilePhysicalPath = Path.Combine(PhysicalPath, FileName);
                if (System.IO.File.Exists(FullfilePhysicalPath))
                {
                    return File(FullfilePhysicalPath, "Application/pdf", DateTime.Now.ToShortDateString() + "_" + new Random().Next(1000000000) + FileExtension);

                }
                else
                {
                    return Json(new { Success = "false", ErrorMessage = "File Not Found." }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                return Json(new { Success = "false", ErrorMessage = "Error While Processing Your Request." }, JsonRequestBehavior.AllowGet);
            }
        }
        #endregion
    }
}
