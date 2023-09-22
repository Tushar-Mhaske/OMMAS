using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PMGSY.Areas.MPRFileDownload.Models;
using PMGSY.Extensions;
using PMGSY.Common;
using PMGSY.Models;
using PMGSY.Controllers;
using System.IO;
using System.Configuration;
using PMGSY.Areas.MPRFileDownload.DAL;
using PMGSY.Areas.MPRFileDownload.BAL;

namespace PMGSY.Areas.MPRFileDownload.Controllers
{
    [RequiredAuthentication]
    [RequiredAuthorization]
    public class MPRFileDownloadsController : Controller
    {
        //
        // GET: /MPRFileDownload/MPRFileDownloads/
        CommonFunctions common = new CommonFunctions();

        string message = String.Empty;
        private IMPRFileUploadDAL objDAL = null;
        private IMPRFileUploadBAL objBAL = null;
        Dictionary<string, string> decryptedParameters = null;
        String[] encryptedParameters = null;
        int outParam = 0;

        public ActionResult Index()
        {
            return View();
        }
        [RequiredAuthentication]
        [RequiredAuthorization]

        [HttpGet]
        public ActionResult LoadFilter()
        {
            CommonFunctions objCommonFunctions = new CommonFunctions();
            MPRFileUploadViewModel FiltersModel = new MPRFileUploadViewModel();
            try
            {
                

                if (PMGSYSession.Current.RoleCode == 2)// SRRDA
                {
                    FiltersModel.StateList = new List<SelectListItem>();


                    FiltersModel.StateList.Insert(0, (new SelectListItem { Text = PMGSYSession.Current.StateName, Value = PMGSYSession.Current.StateCode.ToString(), Selected = true }));
                }
                else // MRD
                {
                    FiltersModel.StateList = objCommonFunctions.PopulateStates(false);
                    FiltersModel.StateList.Insert(0, (new SelectListItem { Text = "Select State", Value = "0", Selected = true }));
                }
            
                return View(FiltersModel);
            }
            catch
            {
                return null;
            }
        }

        //Get List of MPRFileUploadGetGridView
        public ActionResult MPRFileUploadGetGridView(int? page, int? rows, string sidx, string sord)
        {
            String searchParameters = String.Empty;
            long totalRecords;
            string agencyType = String.Empty;
            objBAL = new MPRFileUploadBAL();

            using (CommonFunctions commonFunction = new CommonFunctions())
            {
                if (!commonFunction.ValidateGridParameters(new GridParams(page, rows, sidx, sord, Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                {
                    return null;
                }
            }

            var jsonData = new
            {
                rows = objBAL.ListMPRFileUpload( Convert.ToInt32(Request.Params["stateCode"]), Convert.ToInt32(page) - 1, Convert.ToInt32(rows), sidx, sord, out totalRecords),


                total = totalRecords <= Convert.ToInt32(rows) ? 1 : totalRecords / Convert.ToInt32(rows) + 1,
                page = Convert.ToInt32(page),
                records = totalRecords
            };

            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }


        [HttpGet]
        [Audit]
        public ActionResult DownloadFiles(String id)
        {
            try
            {
                string FileName = id;
                string FullFileLogicalPath = string.Empty;
                string FullfilePhysicalPath = string.Empty;
                string FileExtension = string.Empty;

               // String[] urlParam = id.Split('/');

                //if (!String.IsNullOrEmpty(urlParam[0]) && !String.IsNullOrEmpty(urlParam[1]) && !String.IsNullOrEmpty(urlParam[2]))
                //{
                //    String[] urlParams = URLEncrypt.DecryptParameters(new String[] { urlParam[0].Replace(' ','%'), urlParam[1], urlParam[2] });
                //    if (urlParams.Length >= 1)
                //    {
                //        String[] urlSplitParams = urlParams[0].Split('$');
                //        FileName = (urlSplitParams[0]);

                //    }
                //}
                FileExtension = Path.GetExtension(FileName).ToLower();

              
                    FullFileLogicalPath = Path.Combine(PMGSYSession.Current.PMGSYScheme == 1 ? ConfigurationManager.AppSettings["MPR_FILE_DOWNLOAD_VIRTUAL_DIR_PATH"] : ConfigurationManager.AppSettings["MPR_FILE_DOWNLOAD_VIRTUAL_DIR_PATH"], FileName);
                    FullfilePhysicalPath = Path.Combine(PMGSYSession.Current.PMGSYScheme == 1 ? ConfigurationManager.AppSettings["MPR_FILE_DOWNLOAD"] : ConfigurationManager.AppSettings["MPR_FILE_DOWNLOAD"], FileName);
               


                string name = Path.GetFileName(FileName);
                string ext = Path.GetExtension(FileName);
                
                string type = string.Empty;

                if (ext != null)
                {
                    switch (ext.ToLower())
                    {
                     
                        case ".xls":
                            type = "Application/excel";
                            break;

                        case ".xlsx":
                             type = "Application/excel";
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

    }
}
