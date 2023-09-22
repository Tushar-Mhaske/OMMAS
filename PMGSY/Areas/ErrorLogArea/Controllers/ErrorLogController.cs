using PMGSY.Areas.ErrorLogArea.Models;
using PMGSY.Common;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Xml.Linq;

namespace PMGSY.Areas.ErrorLogArea.Controllers
{
    public class ErrorLogController : Controller
    {
        
        public ActionResult Index()
        {
            #region retrieve ErrorLog Key and Path value

            LogXmlPathModel logdata;
            List<LogXmlPathModel> logXmlPathslist = new List<LogXmlPathModel>();

            if (System.IO.File.Exists(System.Web.HttpContext.Current.Server.MapPath("~/ErrorLogMaster.xml")))
            { // Create a file to write to   
                XDocument doc = XDocument.Load(System.Web.HttpContext.Current.Server.MapPath("~/ErrorLogMaster.xml"));

                foreach (XElement element in doc.Descendants("ErrorLogModules")
                    .Descendants("ModuleLogPath"))
                {
                    logdata = new LogXmlPathModel();

                    logdata.moduleId = Convert.ToString(element.Element("ModuleID").Value);
                    logdata.LogKey = Convert.ToString(element.Element("LogKey").Value);
                    logdata.LogPath = Convert.ToString(element.Element("LogPath").Value);
                    logXmlPathslist.Add(logdata);
                }
            }

            ErrorLogModel model = new ErrorLogModel();
            List<SelectListItem> logItemList = new List<SelectListItem>();
            SelectListItem Listitem;
            Listitem = new SelectListItem();
            Listitem.Text = "Select Log Key";
            Listitem.Value = "0";
            Listitem.Selected = true;
            logItemList.Add(Listitem);

            foreach (var item in logXmlPathslist)
            {
                Listitem = new SelectListItem();
                Listitem.Text = item.LogKey.Trim();
                Listitem.Value = item.moduleId.Trim();
                logItemList.Add(Listitem);
            }
            model.lstModule = logItemList;
           // lstYears.Insert(0, (new SelectListItem { Text = "Select Year", Value = "0", Selected = true }));

            #endregion



            return View(model);
        }

        [HttpPost]
        public ActionResult GetLogContent(string moduleID, string logDate)
        {
            try
            {
                string mODULEid = string.Empty;
                string dateLog = string.Empty;
                string KeyvaluePath = string.Empty;
                string filePath = string.Empty;
               
                mODULEid = moduleID.Trim();
                dateLog = logDate.Replace('/', '_');

                LogXmlPathModel logdata;
                List<LogXmlPathModel> logXmlPathslist = new List<LogXmlPathModel>();

                if (System.IO.File.Exists(System.Web.HttpContext.Current.Server.MapPath("~/ErrorLogMaster.xml")))
                { // Create a file to write to   
                    XDocument doc = XDocument.Load(System.Web.HttpContext.Current.Server.MapPath("~/ErrorLogMaster.xml"));

                    foreach (XElement element in doc.Descendants("ErrorLogModules")
                        .Descendants("ModuleLogPath"))
                    {
                        logdata = new LogXmlPathModel();

                        logdata.moduleId = Convert.ToString(element.Element("ModuleID").Value);
                        logdata.LogKey = Convert.ToString(element.Element("LogKey").Value);
                        logdata.LogPath = Convert.ToString(element.Element("LogPath").Value);
                        logdata.FileAlias = Convert.ToString(element.Element("FileNameAlias").Value);
                        logXmlPathslist.Add(logdata);
                    }
                }

                foreach(var item in logXmlPathslist)
                {
                    if(mODULEid == item.moduleId.Trim())
                    {
                        KeyvaluePath = ConfigurationManager.AppSettings[item.LogKey];
                        if (!Directory.Exists(KeyvaluePath))
                        {
                            string logContent = "Directory Not Exists.";
                            return Content(logContent, "text/plain");
                        }                           
                        filePath = Path.Combine(KeyvaluePath + "\\"+ item.FileAlias + dateLog + ".txt", "");
                        FileInfo file = new FileInfo(filePath);
                        if (file.Exists)
                        {
                            string logContent = System.IO.File.ReadAllText(filePath);
                            return Content(logContent, "text/plain");
                        }
                        else
                        {
                            string logContent = "File Not Exists.";
                            return Content(logContent, "text/plain");
                        }
                    }
                }

                //if (mODULEid == "1")
                //{
                //    KeyvaluePath = ConfigurationManager.AppSettings["OMMASErrorLogPath"];
                //    filePath = Path.Combine(KeyvaluePath + "\\OMMASErrorLog_" + dateLog + ".txt", "");
                //    FileInfo file = new FileInfo(filePath);
                //    if (file.Exists)
                //    {
                //        string logContent = System.IO.File.ReadAllText(filePath);
                //        return Content(logContent, "text/plain");
                //    }
                //    else
                //    {
                //        string logContent = "File Not Exists.";
                //        return Content(logContent, "text/plain");
                //    }

                //}
                //else
                //{
                //    string logContent = "InCorrect Module Id.";
                //    return Content(logContent, "text/plain");
                //}

                return null;
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "GetLogContent()");
                string logContent = "Error Occurred While Processing Request.";
                return Content(logContent, "text/plain");
            }
            finally
            {

            }
        }


    }
}
