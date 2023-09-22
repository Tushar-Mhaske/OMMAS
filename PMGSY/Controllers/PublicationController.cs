


using Mvc.Mailer;
//using PMGSY.BAL.Proposal;
using PMGSY.BAL.QualityMonitoring;
using PMGSY.Common;
using PMGSY.Extensions;
using PMGSY.Mailers;
using PMGSY.Models;
using PMGSY.Models.Common;
using PMGSY.Models.Master;
using PMGSY.Models.Publication;
//using PMGSY.Models.Proposal;
//using PMGSY.Models.QualityMonitoring;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity.Validation;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;


namespace PMGSY.Controllers
{
    [RequiredAuthentication]
    [RequiredAuthorization]
    public class PublicationController : Controller
    {
        PMGSY.BAL.Publication.IPublicationBAL publicationBAL;
        //
        // GET: /Publication/
        public PublicationController()
        {
            PMGSYSession.Current.ModuleName = "Publication";
            publicationBAL = new PMGSY.BAL.Publication.PublicationBAL();
        }

        [Audit]
        public ActionResult Index()
        {
            return View();
        }

        [Audit]
        public ActionResult Publication(int ? id)
        {
            PMGSY.Models.Publication.PublicationViewModel pubViewModel=new Models.Publication.PublicationViewModel();
            ViewBag.PublicationCategory = pubViewModel.PublicationCategoryList;
           
            return View();
        }

        [Audit]
        public ActionResult PublicationAddEdit(int? id)
        {
            CommonFunctions commonFunctions = new CommonFunctions();
            PublicationViewModel pubViewModel;
            pubViewModel = new PublicationViewModel();
           

            if(id.HasValue)
            {
                
                pubViewModel = publicationBAL.GetPublicationBAL(id.Value);
                pubViewModel.Action = "E";
            }
            else
            {               
                pubViewModel.Action = "A";

                
            }

        


            return View(pubViewModel);
        }

        
        [HttpPost]
        [Audit]
        public ActionResult PublicationAddEdit(Models.Publication.PublicationViewModel pubViewModel)
        {
            string message = string.Empty;
            try
            {
                if (ModelState.IsValid)
                {

                    if (publicationBAL.PublicationAddEditBAL(pubViewModel, ref message))
                    {
                        if (pubViewModel.Action == "A")
                        {
                            message = message == string.Empty ? "Publication details saved successfully." : message;
                        }
                        else if (pubViewModel.Action == "E")
                        {
                            message = message == string.Empty ? "Publication details updated successfully." : message;
                        }

                        return Json(new { success = true, message = message });
                    }
                    else
                    {
                       
                        if (pubViewModel.Action == "A")
                        {
                            message = message == string.Empty ? "Publication details not saved." : message;
                        }
                        else if (pubViewModel.Action == "E")
                        {
                            message = message == string.Empty ? "Publication details not updated." : message;
                        }
                        return Json(new { success = false, message = message });
                    }
                }
                else
                {

                    //string messages = string.Join("; ", ModelState.Values
                    //                       .SelectMany(x => x.Errors)
                    //                       .Select(x => x.ErrorMessage));

                    return Json(new { success = false, message = new CommonFunctions().FormatErrorMessage(ModelState) }, JsonRequestBehavior.AllowGet);
                }
                return Json(new { success = false, message = message }, JsonRequestBehavior.AllowGet);
            }
            catch (DbEntityValidationException ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                foreach (var eve in ex.EntityValidationErrors)
                {
                    ModelState.AddModelError("", eve.ValidationErrors.ToString());
                    message += eve.ValidationErrors.ToString();
                }
                return Json(new { success = false, message = message }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                message = "Publication details not saved because " + ex.Message;
                return Json(new { success = false, message = message }, JsonRequestBehavior.AllowGet);
            }        

          
        }
       

        [HttpPost]
        [Audit]
        public ActionResult PublicationAction(int pubid,string action)
        {
            string message=string.Empty;
            publicationBAL.PublicationActionBAL(pubid, action, ref message);
            return Json(new { Success = message });
        }

        [HttpPost]
        [Audit]
        public ActionResult PublicationGetJQGrid(int publication, string published, string finalized,int? page, int? rows, string sidx, string sord)
        {
            String searchParameters = String.Empty;
            int totalRecords;           

            Dictionary<string, string> parameters = new Dictionary<string, string>();

            
            using (CommonFunctions commonFunction = new CommonFunctions())
            {
                if (!commonFunction.ValidateGridParameters(new GridParams(page, rows, sidx, sord, Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                {
                    return null;
                }
            }
            var jsonData = new
            {
                rows = publicationBAL.GetPublicationListBAL(publication, published, finalized, Convert.ToInt32(page) - 1, Convert.ToInt32(rows), sidx, sord, out totalRecords),
                total = totalRecords <= Convert.ToInt32(rows) ? 1 : totalRecords / Convert.ToInt32(rows) + 1,
                page = Convert.ToInt32(page),
                records = totalRecords
            };
            //int totalRecords=0;
            //var jsonData = new
            //{
            //    rows = publicationBAL.GetPublicationListBAL(publication, published, finalized,Convert.ToInt32(page) - 1, Convert.ToInt32(rows), sidx, sord, out totalRecords),
            //    total = totalRecords <= Convert.ToInt32(rows) ? 1 : totalRecords / Convert.ToInt32(rows) + 1,
            //    page = Convert.ToInt32(page),
            //    records = totalRecords
            //};
            return Json(jsonData);
            
        }


        [Audit]
        public ActionResult PublicationUpload(int id)
        {

            var publication = publicationBAL.GetPublicationBAL(id);

            PMGSY.Models.Publication.PublicationUploadViewModel publicationUploadViewModel = new Models.Publication.PublicationUploadViewModel
            { 
                publicationCode=publication.publicationCode,
                pubAuthor=publication.publicationAuther,
                pubTitle = publication.publicationTitle,
                pubVolume = publication.publicationVolume,
                publicationName = publication.publicationName,
                pubDate = publication.publicationDate,
                publicationFinalized=publication.publicationFinalized
            };



            return View(publicationUploadViewModel);
        }

        [Audit]
        public ActionResult ListPublicationFile(int id, int? page, int? rows, string sidx, string sord)
        {

            int totalRecords = 0;
            var jsonData = new
            {
                rows = publicationBAL.ListPublicationFileBAL(id, Convert.ToInt32(page) - 1, Convert.ToInt32(rows), sidx, sord, out totalRecords),
                total = totalRecords <= Convert.ToInt32(rows) ? 1 : totalRecords / Convert.ToInt32(rows) + 1,
                page = Convert.ToInt32(page),
                records = totalRecords
            };
            return Json(jsonData);
        }

        
        [HttpPost]
        [Audit]
        public ActionResult PdfFileUpload(PMGSY.Models.Publication.PublicationUploadViewModel pubUploadViewModel)
        {
           
            PMGSYEntities dbContext = new PMGSYEntities();
            
            try
            {
                CommonFunctions objCommonFunc = new CommonFunctions();
                if (!(objCommonFunc.ValidateIsPdf(ConfigurationManager.AppSettings["PUBLICATION_FILE_UPLOAD"], Request)))
                {
                    pubUploadViewModel.ErrorMessage = "File Type is Not Allowed.";
                    return View("PdfFileUpload", pubUploadViewModel.ErrorMessage);
                }

                foreach (string file in Request.Files)
                {
                    string status = ValidatePDFFile(Request.Files[0].ContentLength, Path.GetExtension(Request.Files[0].FileName));
                    if (status != string.Empty)
                    {
                        pubUploadViewModel.ErrorMessage = status;
                        return View("PdfFileUpload", pubUploadViewModel.ErrorMessage);
                    }
                }

                var fileData = new List<PMGSY.Models.Publication.PublicationUploadViewModel>();

                Int32 publicationCode = 0;
                if (pubUploadViewModel.publicationCode != 0)
                {
                    publicationCode = pubUploadViewModel.publicationCode.Value;
                }
                else
                {
                    try
                    {
                        publicationCode = Convert.ToInt32(Request["publicationCode"]);
                    }
                    catch (Exception ex)
                    {
                        Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                        if (Request["publicationCode"].Contains(','))
                        {
                            publicationCode = Convert.ToInt32(Request["publicationCode"].Split(',')[0]);
                        }
                        if (Request["ADMIN_SCHEDULE_CODE"].Contains(','))
                        {
                            publicationCode = Convert.ToInt32(Request["publicationCode"].Split(',')[0]);
                        }
                    }
                }

                foreach (string file in Request.Files)
                {
                    UploadPDFFile(Request, fileData, publicationCode);
                }

                if (dbContext.MRD_PUBLICATION_FILES.Where(a => a.PUBLICATION_CODE == publicationCode).Any())
                {
                    pubUploadViewModel.NumberofFiles = dbContext.MRD_PUBLICATION_FILES.Where(a => a.PUBLICATION_CODE == publicationCode).Count();
                }
                else
                {
                    pubUploadViewModel.NumberofFiles = 0;
                }

                var serializer = new JavaScriptSerializer();
                serializer.MaxJsonLength = Int32.MaxValue;

                var result = new ContentResult
                {
                    Content = "{\"files\":" + serializer.Serialize(fileData) + "}",
                };
                return result;
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);

                return null;
            }
            finally
            {
                dbContext.Dispose();
            }


        }



        /// <summary>
        /// This Method Uploads PDF File
        /// </summary>
        /// <param name="request"></param>
        /// <param name="statuses"></param>
        /// <param name="IMS_PR_ROAD_CODE"></param>
        [Audit]
        public void UploadPDFFile(HttpRequestBase request, List<PMGSY.Models.Publication.PublicationUploadViewModel> statuses, int publicationCode)
        {
            String StorageRoot = ConfigurationManager.AppSettings["PUBLICATION_FILE_UPLOAD"];
            PMGSYEntities dbContext = new PMGSYEntities();
            
            int MaxCount = 0;

            for (int i = 0; i < request.Files.Count; i++)
            {
                HttpPostedFileBase file = request.Files[i];
                //var fileId = IMS_PR_ROAD_CODE;
                MaxCount = dbContext.MRD_PUBLICATION_FILES.Max(cp => (Int32?)cp.PUBLICATION_FILE_CODE) == null ? 1 : (Int32)dbContext.MRD_PUBLICATION_FILES.Max(cp => (Int32?)cp.PUBLICATION_FILE_CODE) + 1;                
               // MaxCount = dbContext.MRD_PUBLICATION_FILES.Select(c => c.PUBLICATION_FILE_CODE).Max();
                var fileId = MaxCount;

                var fileName = request.Files[i].FileName.ToString();
                var fullPath = Path.Combine(StorageRoot, fileName);

                statuses.Add(new PMGSY.Models.Publication.PublicationUploadViewModel()
                {
                    url = fullPath,
                    thumbnail_url = fullPath,
                    publicationName = fileId.ToString() + ".pdf",
                    type = file.ContentType,
                    size = file.ContentLength,
                    delete_url = "",
                    delete_type = "DELETE",

                    //PdfDescription = request.Params["PdfDescription[]"],

                    publicationCode = publicationCode
                });

                string status = publicationBAL.AddPublicationFileUploadBAL(statuses);
                if (status == string.Empty)
                {

                    //file.SaveAs(fullPath);
                    file.SaveAs(Path.Combine(ConfigurationManager.AppSettings["PUBLICATION_FILE_UPLOAD"], fileId.ToString() + ".pdf"));//File Will be saved as FileId.pdf
                }
                else
                {
                    // show an error over here
                }
            }

        }

        /// <summary>
        /// Validates the PDF File
        /// </summary>
        /// <param name="FileSize"></param>
        /// <param name="FileExtension"></param>
        /// <returns></returns>
        [Audit]
        public string ValidatePDFFile(int FileSize, string FileExtension)
        {
            if (FileExtension.ToUpper() != ".PDF")
            {
                return "File is not PDF File";
            }
            if (FileSize > Convert.ToInt32(ConfigurationManager.AppSettings["PROPOSAL_PDF_FILE_MAX_SIZE"]))
            {
                return "File Size Exceed the Maximum File Limit";
            }

            return string.Empty;
        }

        [HttpGet]
        [Audit]
        public ActionResult DownloadFile(int id,String parameter, String hash, String key)
        {
            string FileName = id+".pdf";
            //string FullFileLogicalPathName = string.Empty;
            string FullfilePhysicalPathName = string.Empty;
           // string FullFileLogicalPathId = string.Empty;
            string FullfilePhysicalPathId = string.Empty;
            string FileExtension = string.Empty;
            Int32 publicationId = id;
            PMGSYEntities dbContext = new PMGSYEntities();
            try
            {
                if (!String.IsNullOrEmpty(parameter) && !String.IsNullOrEmpty(hash) && !String.IsNullOrEmpty(key))
                {
                    String[] urlParams = URLEncrypt.DecryptParameters(new String[] { parameter, hash, key });
                    if (urlParams.Length >= 1)
                    {
                        String[] urlSplitParams = urlParams[0].Split('$');
                        FileName = (urlSplitParams[0]);
                        publicationId = Convert.ToInt32(urlSplitParams[1]);
                    }
                }
                FileExtension = Path.GetExtension(FileName).ToLower();

                if (FileExtension == ".pdf")
                {
                    //In case of if File With Name not Found then find with Id, This is case particularly for ATR
                    //FullFileLogicalPathId = Path.Combine(ConfigurationManager.AppSettings["PUBLICATION_FILE_UPLOAD_VIRTUAL_DIR_PATH"], publicationId.ToString() + ".pdf");
                    FullfilePhysicalPathId = Path.Combine(ConfigurationManager.AppSettings["PUBLICATION_FILE_UPLOAD"], publicationId.ToString() + ".pdf");

                    //FullFileLogicalPathName = Path.Combine(ConfigurationManager.AppSettings["PUBLICATION_FILE_UPLOAD_VIRTUAL_DIR_PATH"], FileName);
                    FullfilePhysicalPathName = Path.Combine(ConfigurationManager.AppSettings["PUBLICATION_FILE_UPLOAD"], FileName);
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

                if (System.IO.File.Exists(FullfilePhysicalPathName))
                {
                    return File(FullfilePhysicalPathName, type, DateTime.Now.ToShortDateString() + "_" + new Random().Next(1000000000) + FileExtension);
                }
                else if (System.IO.File.Exists(FullfilePhysicalPathId))
                {
                    return File(FullfilePhysicalPathId, type, DateTime.Now.ToShortDateString() + "_" + new Random().Next(1000000000) + FileExtension);
                }
                else
                {
                    return Json(new { Success = "false", Error = "File Not Exist" }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return Json(new { Success = "false" }, JsonRequestBehavior.AllowGet);
            }
            finally
            {
                dbContext.Dispose();
            }
        }

    }
}
