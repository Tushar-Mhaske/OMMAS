using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Text;

using System.Data.Entity.Validation;
using PMGSY.Common;
using System.Text.RegularExpressions;
using System.Web.Script.Serialization;
using System.IO;
using System.Configuration;
using System.Net;

namespace PMGSY.Controllers
{
    [RequiredAuthentication]
    [RequiredAuthorization]
    public class NewsDetailsController : Controller
    {
        //
        // GET: /NewsDetails/
        Dictionary<string, string> decryptedParameters = null;

        Feedback.DAL.FeedbackDAL fbDAL = new Feedback.DAL.FeedbackDAL();
        PMGSY.DAL.NewsDetails.NewsDAL newsDAL = new DAL.NewsDetails.NewsDAL();
        PMGSY.BAL.NewsDetails.NewsBAL newsBAL = new BAL.NewsDetails.NewsBAL();
        private PMGSY.Models.PMGSYEntities dbContext = new Models.PMGSYEntities();
        Common.CommonFunctions comm = new Common.CommonFunctions();
        int outParam = 0;

        public ActionResult DetailsNews()
        {
            PMGSY.Models.NewsDetails.DetailsNews vwNwDtls = new Models.NewsDetails.DetailsNews();

            FeedbackDetailsController fb = new FeedbackDetailsController();

            vwNwDtls.Years_List = fb.PopulateYear(System.DateTime.Now.Year, true, true);


            vwNwDtls.Months_List = comm.PopulateMonths(System.DateTime.Now.Month);

            vwNwDtls.MONTHs = System.DateTime.Now.Month;
            vwNwDtls.YEARs = System.DateTime.Now.Year;

            vwNwDtls.Status_List = fillStatus("N");

            if (PMGSY.Extensions.PMGSYSession.Current.RoleCode == 25)
            {
                vwNwDtls.rdbNS = "N";
                vwNwDtls.State_List = newsDAL.PopulateNrrdaStates("-1");
                vwNwDtls.Nrrda_List = newsDAL.PopulateNRRDAList("0");
                vwNwDtls.Srrda_List = newsDAL.PopulateSRRDAList("-1");
                vwNwDtls.Dpiu_List = newsDAL.PopulateDPIUList("-1");
            }
            else if (PMGSY.Extensions.PMGSYSession.Current.RoleCode == 2)
            {
                vwNwDtls.rdbNS = "S";
                vwNwDtls.State_List = newsDAL.PopulateNrrdaStates("-1");
                vwNwDtls.Nrrda_List = newsDAL.PopulateNRRDAList("-1");
                vwNwDtls.Srrda_List = newsDAL.PopulateSRRDAList("0");
                vwNwDtls.Dpiu_List = newsDAL.PopulateDPIUList("-1");
            }
            else
            {
                //vwNwDtls.State_List = newsDAL.PopulateNrrdaStates("-1");
                vwNwDtls.Srrda_List = newsDAL.PopulateSRRDAList("0");
                vwNwDtls.Dpiu_List = newsDAL.PopulateDPIUList(Convert.ToString(PMGSY.Extensions.PMGSYSession.Current.AdminNdCode).Trim());
            }


            vwNwDtls.Approved = "0";
            vwNwDtls.Status = "0";

            vwNwDtls.hdnRole = PMGSY.Extensions.PMGSYSession.Current.RoleCode;

            return View("DetailsNews", vwNwDtls);
        }

        public JsonResult fillDDLStatus(string approval)
        {
            PMGSY.Models.NewsDetails.DetailsNews vwNwDtls = new Models.NewsDetails.DetailsNews();
            try
            {
                List<SelectListItem> statusLst = new List<SelectListItem>();

                statusLst = fillStatus(approval);

                return Json(statusLst);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return Json(string.Empty);
            }
        }

        public List<SelectListItem> fillStatus(string appr)
        {
            List<SelectListItem> statusLst = new List<SelectListItem>();
            SelectListItem item;
            if (appr == "N" || appr == "0")
            {
                statusLst.Insert(0, new SelectListItem() { Text = "Archived", Value = "A" });
                statusLst.Insert(0, new SelectListItem() { Text = "Published", Value = "P" });
                statusLst.Insert(0, new SelectListItem() { Text = "New", Value = "N" });
                statusLst.Insert(0, new SelectListItem() { Text = "All", Value = "0" });
            }
            else
            {
                statusLst.Insert(0, new SelectListItem() { Text = "Archived", Value = "A" });
                statusLst.Insert(0, new SelectListItem() { Text = "Published", Value = "P" });
                statusLst.Insert(0, new SelectListItem() { Text = "All", Value = "0" });
            }
            return statusLst;
        }

        public JsonResult fillDDLSRRDA(string Code)
        {
            PMGSY.Models.NewsDetails.DetailsNews vwNwDtls = new Models.NewsDetails.DetailsNews();
            //int outParam = 0;
            try
            {
                if (!int.TryParse(Code, out outParam))
                {
                    return Json(false);
                }
                List<SelectListItem> srrdaLst = new List<SelectListItem>();

                srrdaLst = newsDAL.PopulateSRRDAList(Code.Trim());

                //return Json(new SelectList(vwNwDtls.Srrda_List, "ADMIN_ND_NAME", "ADMIN_ND_CODE"));
                return Json(srrdaLst);

            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return Json(string.Empty);
            }
        }

        public JsonResult fillSRRDAstate(string Code)
        {
            PMGSY.Models.NewsDetails.DetailsNews vwNwDtls = new Models.NewsDetails.DetailsNews();
            //int outParam = 0;
            try
            {
                if (!int.TryParse(Code, out outParam))
                {
                    return Json(false);
                }
                List<SelectListItem> srrdastateLst = new List<SelectListItem>();

                srrdastateLst = newsDAL.PopulateNrrdaStates(Code.Trim());

                return Json(srrdastateLst);

            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return Json(string.Empty);
            }
        }

        public JsonResult fillDDLDPIU(string Code)
        {
            PMGSY.Models.NewsDetails.DetailsNews vwNwDtls = new Models.NewsDetails.DetailsNews();
            //int outParam = 0;
            try
            {
                if (!int.TryParse(Code, out outParam))
                {
                    return Json(false);
                }
                List<SelectListItem> dpiuLst = new List<SelectListItem>();

                dpiuLst = newsDAL.PopulateDPIUList(Code.Trim());

                return Json(dpiuLst);

            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return Json(string.Empty);
            }
        }

        [HttpPost]
        public ActionResult NewsDetailsList(FormCollection formCollection)
        {
            int totalRecords;
            try
            {
                using (PMGSY.Common.CommonFunctions commonFunction = new PMGSY.Common.CommonFunctions())
                {
                    if (!commonFunction.ValidateGridParameters(new PMGSY.Common.GridParams(Convert.ToInt32(formCollection["page"]), Convert.ToInt32(formCollection["rows"]), formCollection["sidx"], formCollection["sord"], Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                    {
                        return null;
                    }
                }
                var jsonData = new
                {
                    rows = newsDAL.NewsDetailsReportDAL(Convert.ToInt32(formCollection["page"]) - 1, Convert.ToInt32(formCollection["rows"]),
                                                            formCollection["sidx"], formCollection["sord"], out totalRecords,
                                                            Convert.ToInt32(formCollection["FOR_MONTH"]), Convert.ToInt32(formCollection["FOR_YEAR"]),
                                                            Convert.ToString(formCollection["APPR"]), Convert.ToString(formCollection["STATUS"]),
                                                            Convert.ToString(formCollection["NRRDA"]), Convert.ToString(formCollection["State"]),
                                                            Convert.ToString(formCollection["SRRDA"]), Convert.ToString(formCollection["DPIU"])
                                                            ),
                    total = totalRecords <= Convert.ToInt32(formCollection["rows"]) ? 1 : (totalRecords % Convert.ToInt32(formCollection["rows"]) == 0 ? totalRecords / Convert.ToInt32(formCollection["rows"]) : (totalRecords / Convert.ToInt32(formCollection["rows"]) + 1)),
                    page = Convert.ToInt32(formCollection["page"]),
                    records = totalRecords
                };
                return Json(jsonData, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return Json(String.Empty);
            }

        }

        public ActionResult CreateNews(String parameter, String hash, String key/*string id*/)
        {
            dbContext = new Models.PMGSYEntities();
            PMGSY.Models.NewsDetails.CreateNews crNews = new Models.NewsDetails.CreateNews();
            string id = string.Empty;
            if (parameter != null && hash != null && key != null)
            {
                decryptedParameters = PMGSY.Common.URLEncrypt.DecryptParameters1(new String[] { parameter, hash, key });
                id = Convert.ToString(decryptedParameters["NewsCode"]).Trim();
            }
            else
            {
                id = "0";
            }

            //if (decryptedParameters.Count > 0)
            {

                if (!int.TryParse(id, out outParam))
                {
                    return Json(false);
                }
                int nId = Convert.ToInt32(id.Trim());

                if (id == "0")
                {
                    crNews.newsDBOpr = "I";
                }
                else
                {

                    var q = dbContext.ADMIN_NEWS.Where(n => n.NEWS_ID == nId).DefaultIfEmpty().ToList();
                    if (q.Count > 0)
                    {
                        foreach (var item in q)
                        {
                            crNews.hdnewsId = item.NEWS_ID;
                            crNews.newsTitle = item.NEWS_TITLE.Trim();
                            crNews.news_Upload_Date = comm.GetDateTimeToString(item.NEWS_UPLOAD_DATE).Trim();
                            crNews.news_Desc = item.NEWS_DESCRIPTION.Trim();
                            crNews.newa_Pub_Start_Date = comm.GetDateTimeToString(item.NEWS_PUB_START_DATE).Trim();

                            if (item.NEWS_PUB_END_DATE != null)
                            {
                                crNews.newa_Pub_End_Date = comm.GetDateTimeToString((DateTime)item.NEWS_PUB_END_DATE).Trim();
                            }
                            else
                            {
                                crNews.newa_Pub_End_Date = string.Empty;
                            }
                            crNews.newsDBOpr = "M";
                        }
                    }
                }
            }
            return View("CreateNews", crNews);
        }

        public JsonResult NewsFinalization(String parameter, String hash, String key)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    decryptedParameters = PMGSY.Common.URLEncrypt.DecryptParameters1(new String[] { parameter, hash, key });

                    if (decryptedParameters.Count < 0)
                    {
                        return Json(false);
                    }
                    string id = Convert.ToString(decryptedParameters["NewsCode"]).Trim();
                    if (!int.TryParse(id, out outParam))
                    {
                        return Json(false);
                    }

                    int flag = newsDAL.FinalizeNews(id);

                    return Json(new { status = flag }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    string message = ModelState.Values.Where(x => x.Errors.Count == 1).FirstOrDefault().Errors[0].ErrorMessage;
                    return Json(string.Empty);
                }
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return null;
            }
        }

        public JsonResult NewsUnfinalization(String parameter, String hash, String key)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    decryptedParameters = PMGSY.Common.URLEncrypt.DecryptParameters1(new String[] { parameter, hash, key });

                    if (decryptedParameters.Count < 0)
                    {
                        return Json(false);
                    }
                    string id = Convert.ToString(decryptedParameters["NewsCode"]).Trim();
                    if (!int.TryParse(id, out outParam))
                    {
                        return Json(false);
                    }

                    int flag = newsDAL.UnfinalizeNews(id);

                    return Json(new { status = flag }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    string message = ModelState.Values.Where(x => x.Errors.Count == 1).FirstOrDefault().Errors[0].ErrorMessage;
                    return Json(string.Empty);
                }
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return null;
            }
        }

        public JsonResult NewsCreation(PMGSY.Models.NewsDetails.CreateNews crNews)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    bool flag = newsDAL.saveNews(crNews);

                    return Json(new { status = flag }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    string message = ModelState.Values.Where(x => x.Errors.Count == 1).FirstOrDefault().Errors[0].ErrorMessage;
                    return Json(string.Empty);
                }
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return null;
            }
        }

        public JsonResult NewsUpdation(PMGSY.Models.NewsDetails.CreateNews crNews)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    int flag = newsDAL.updateNews(crNews);

                    return Json(new { status = flag }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    string message = ModelState.Values.Where(x => x.Errors.Count == 1).FirstOrDefault().Errors[0].ErrorMessage;
                    return Json(string.Empty);
                }
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return null;
            }
        }

        public JsonResult NewsDeletion(String parameter, String hash, String key/*string id*/)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    decryptedParameters = PMGSY.Common.URLEncrypt.DecryptParameters1(new String[] { parameter, hash, key });

                    if (decryptedParameters.Count < 0)
                    {
                        return Json(false);
                    }
                    string id = Convert.ToString(decryptedParameters["NewsCode"]).Trim();
                    if (!int.TryParse(id, out outParam))
                    {
                        return Json(false);
                    }

                    int flag = newsDAL.deleteNews(id);

                    return Json(new { status = flag }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    string message = ModelState.Values.Where(x => x.Errors.Count == 1).FirstOrDefault().Errors[0].ErrorMessage;
                    return Json(string.Empty);
                }
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return null;
            }
        }

        public JsonResult NewspublishArchive(String parameter, String hash, String key/*string id*/)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    decryptedParameters = PMGSY.Common.URLEncrypt.DecryptParameters1(new String[] { parameter, hash, key });

                    if (decryptedParameters.Count < 0)
                    {
                        return Json(false);
                    }
                    string id = Convert.ToString(decryptedParameters["NewsCode"]).Trim();
                    if (!int.TryParse(id, out outParam))
                    {
                        return Json(false);
                    }

                    int flag = newsDAL.PublishArchiveNews(id);

                    return Json(new { status = flag }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    string message = ModelState.Values.Where(x => x.Errors.Count == 1).FirstOrDefault().Errors[0].ErrorMessage;
                    return Json(string.Empty);
                }
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return null;
            }
        }

        public JsonResult NewsApproval(String parameter, String hash, String key/*string id*/)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    decryptedParameters = PMGSY.Common.URLEncrypt.DecryptParameters1(new String[] { parameter, hash, key });

                    if (decryptedParameters.Count < 0)
                    {
                        return Json(false);
                    }
                    string id = Convert.ToString(decryptedParameters["NewsCode"]).Trim();
                    if (!int.TryParse(id, out outParam))
                    {
                        return Json(false);
                    }

                    int flag = newsDAL.ApproveNews(id);

                    return Json(new { status = flag }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    string message = ModelState.Values.Where(x => x.Errors.Count == 1).FirstOrDefault().Errors[0].ErrorMessage;
                    return Json(string.Empty);
                }
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return null;
            }
        }

        public ActionResult NewsUpload(String parameter, String hash, String key/*string newsId*/)
        {
            decryptedParameters = PMGSY.Common.URLEncrypt.DecryptParameters1(new String[] { parameter, hash, key });

            if (decryptedParameters.Count < 0)
            {
                return Json(false);
            }
            string newsId = Convert.ToString(decryptedParameters["NewsCode"]).Trim();
            if (!int.TryParse(newsId, out outParam))
            {
                return Json(false);
            }

            PMGSY.Models.NewsDetails.NewsUpload nwsUpl = new Models.NewsDetails.NewsUpload();
            nwsUpl.hdnnewsId = Convert.ToInt32(newsId.Trim());

            return View("NewsUpload", nwsUpl);
        }

        public JsonResult ListFiles(FormCollection formCollection)
        {
            try
            {
                if (!int.TryParse(Convert.ToString(Request["News_Id"]), out outParam))
                {
                    return Json(false);
                }

                int newsId = Convert.ToInt32(Request["News_Id"]);
                int totalRecords;
                using (PMGSY.Common.CommonFunctions commonFunction = new PMGSY.Common.CommonFunctions())
                {
                    if (!commonFunction.ValidateGridParameters(new PMGSY.Common.GridParams(Convert.ToInt32(formCollection["page"]), Convert.ToInt32(formCollection["rows"]), formCollection["sidx"], formCollection["sord"], Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                    {
                        return null;
                    }
                }

                var jsonData = new
                {
                    rows = newsDAL.NewsFileList(Convert.ToInt32(formCollection["page"]) - 1, Convert.ToInt32(formCollection["rows"]), formCollection["sidx"], formCollection["sord"], out totalRecords, newsId),
                    total = totalRecords <= Convert.ToInt32(formCollection["rows"]) ? 1 : totalRecords / Convert.ToInt32(formCollection["rows"]) + 1,
                    page = Convert.ToInt32(formCollection["page"]),
                    records = totalRecords
                };
                return Json(jsonData);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return null;
            }
        }

        public ActionResult NewsPDFUpload(string newsId)
        {
            if (!int.TryParse(newsId, out outParam))
            {
                return Json(false);
            }

            PMGSY.Models.NewsDetails.NewsPDFUpload nwspdfUpl = new Models.NewsDetails.NewsPDFUpload();
            nwspdfUpl.News_Id = Convert.ToInt32(newsId.Trim());
            //nwspdfUpl.News_Id = 2;

            dbContext = new Models.PMGSYEntities();

            var s = dbContext.ADMIN_NEWS_FILES.Where(a => a.NEWS_ID == nwspdfUpl.News_Id && a.FILE_TYPE == "P").Count();
            nwspdfUpl.NumberofPdfs = Convert.ToInt32(s);

            return View("NewsPDFUpload", nwspdfUpl);
        }

        public ActionResult NewsImageUpload(string newsId)
        {
            if (!int.TryParse(newsId, out outParam))
            {
                return Json(false);
            }

            PMGSY.Models.NewsDetails.NewsPDFUpload nwspdfUpl = new Models.NewsDetails.NewsPDFUpload();
            nwspdfUpl.News_Id = Convert.ToInt32(newsId.Trim());

            var s = dbContext.ADMIN_NEWS_FILES.Where(a => a.NEWS_ID == nwspdfUpl.News_Id && a.FILE_TYPE == "I").Count();
            nwspdfUpl.NumberofImages = Convert.ToInt32(s);

            return View("NewsImageUpload", nwspdfUpl);
        }


        /// <summary>
        /// Get the PDF Files List
        /// </summary>
        /// <param name="formCollection"></param>
        /// <returns></returns>
        //[Audit]
        public JsonResult ListImageFiles(FormCollection formCollection)
        {
            if (!int.TryParse(Convert.ToString(Request["News_Id"]), out outParam))
            {
                return Json(false);
            }

            int newsId = Convert.ToInt32(Request["News_Id"]);
            int totalRecords;
            using (PMGSY.Common.CommonFunctions commonFunction = new PMGSY.Common.CommonFunctions())
            {
                if (!commonFunction.ValidateGridParameters(new PMGSY.Common.GridParams(Convert.ToInt32(formCollection["page"]), Convert.ToInt32(formCollection["rows"]), formCollection["sidx"], formCollection["sord"], Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                {
                    return null;
                }
            }

            var jsonData = new
            {
                rows = newsDAL.NewsImageFileList(Convert.ToInt32(formCollection["page"]) - 1, Convert.ToInt32(formCollection["rows"]), formCollection["sidx"], formCollection["sord"], out totalRecords, newsId),
                total = totalRecords <= Convert.ToInt32(formCollection["rows"]) ? 1 : totalRecords / Convert.ToInt32(formCollection["rows"]) + 1,
                page = Convert.ToInt32(formCollection["page"]),
                records = totalRecords
            };
            return Json(jsonData);
        }


        /// <summary>
        /// Get Main File Upload View
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        //[HttpGet]
        //[Audit]
        //public ActionResult FileUpload(string id)
        //{
        //    FileUploadViewModel fileUploadViewModel = new FileUploadViewModel();
        //    fileUploadViewModel.IMS_PR_ROAD_CODE = Convert.ToInt32(id);

        //    IMS_SANCTIONED_PROJECTS ims_sanctioned_project = db.IMS_SANCTIONED_PROJECTS.Where(a => a.IMS_PR_ROAD_CODE == fileUploadViewModel.IMS_PR_ROAD_CODE).First();
        //    fileUploadViewModel.IMS_YEAR = ims_sanctioned_project.IMS_YEAR;
        //    fileUploadViewModel.IMS_BATCH = ims_sanctioned_project.IMS_BATCH;
        //    fileUploadViewModel.IMS_PACKAGE_ID = ims_sanctioned_project.IMS_PACKAGE_ID;
        //    fileUploadViewModel.IMS_ROAD_NAME = ims_sanctioned_project.IMS_ROAD_NAME;
        //    fileUploadViewModel.IMS_PAV_LENGTH = Convert.ToDecimal(ims_sanctioned_project.IMS_PAV_LENGTH);
        //    return View("FileUpload", fileUploadViewModel);
        //}

        /// <summary>
        ///  Get the Image Upload View
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        //[HttpGet]
        //[Audit]
        //public ActionResult ImageUpload(string id)
        //{
        //    FileUploadViewModel fileUploadViewModel = new FileUploadViewModel();
        //    fileUploadViewModel.IMS_PR_ROAD_CODE = Convert.ToInt32(id);
        //    if (db.IMS_PROPOSAL_FILES.Where(a => a.IMS_PR_ROAD_CODE == fileUploadViewModel.IMS_PR_ROAD_CODE && a.ISPF_TYPE.ToUpper() == "I").Any())
        //    {
        //        fileUploadViewModel.NumberofImages = db.IMS_PROPOSAL_FILES.Where(a => a.IMS_PR_ROAD_CODE == fileUploadViewModel.IMS_PR_ROAD_CODE && a.ISPF_TYPE.ToUpper() == "I").Count();
        //    }
        //    else
        //    {
        //        fileUploadViewModel.NumberofImages = 0;
        //    }
        //    fileUploadViewModel.ErrorMessage = string.Empty;
        //    return View("ImageUpload", fileUploadViewModel);
        //}

        /// <summary>
        /// Post Method for Uploading IMAGE File
        /// </summary>
        /// <param name="fileUploadViewModel"></param>
        /// <returns></returns>
        [HttpPost]
        //[AcceptVerbs(HttpVerbs.Post)]
        //[Audit]
        public ActionResult Uploads(PMGSY.Models.NewsDetails.NewsPDFUpload newsUploadViewModel)
        {
            //Added By Abhishek kamble 26-Apr-2014 to validate File Type
            CommonFunctions objCommonFunc = new CommonFunctions();
            //Array of File Types to Validate             
            String[] fileTypes = new String[] { "jpeg", String.Empty, String.Empty };
            if (!(objCommonFunc.IsValidImageFile(ConfigurationManager.AppSettings["NEWS_IMAGE_FILE_UPLOAD"], Request, fileTypes)))
            {
                newsUploadViewModel.ErrorMessage = "File Type is Not Allowed.";
                return View("ImageUpload", newsUploadViewModel.ErrorMessage);
            }
            foreach (string file in Request.Files)
            {
                string status = newsBAL.ValidateImageFile(Request.Files[0].ContentLength, Path.GetExtension(Request.Files[0].FileName));
                if (status != string.Empty)
                {
                    newsUploadViewModel.ErrorMessage = status;
                    return View("ImageUpload", newsUploadViewModel.ErrorMessage);
                }
            }

            var fileData = new List<PMGSY.Models.NewsDetails.NewsPDFUpload>();

            int newsID = 0;
            if (newsUploadViewModel.News_Id != 0)
            {
                newsID = newsUploadViewModel.News_Id;
            }
            else
            {
                try
                {
                    newsID = Convert.ToInt32(Request["News_Id"]);
                }
                catch
                {
                    if (Request["News_Id"].Contains(','))
                    {
                        newsID = Convert.ToInt32(Request["News_Id"].Split(',')[0]);
                    }
                }
            }
            foreach (string file in Request.Files)
            {
                UploadImageFile(Request, fileData, newsID);
            }
            var serializer = new JavaScriptSerializer();
            serializer.MaxJsonLength = Int32.MaxValue;

            var result = new ContentResult
            {
                Content = "{\"files\":" + serializer.Serialize(fileData) + "}",
            };
            return result;
        }



        /// <summary>
        /// Post Method for Uploading PDF File
        /// </summary>
        /// <param name="fileUploadViewModel"></param>
        /// <returns></returns>
        [HttpPost]
        //[Audit]
        //[ValidateAntiForgeryToken]
        public ActionResult NewsPdfFileUpload(PMGSY.Models.NewsDetails.NewsPDFUpload newspdfUploadModel)
        {
            //Added By Abhishek kamble 26-Apr-2014
            CommonFunctions objCommonFunc = new CommonFunctions();
            if (!(objCommonFunc.ValidateIsPdf(ConfigurationManager.AppSettings["NEWS_PDF_FILE_UPLOAD"], Request)))
            {
                newspdfUploadModel.ErrorMessage = "File Type is Not Allowed.";
                return View("PdfFileUpload", newspdfUploadModel.ErrorMessage);
            }            

            foreach (string file in Request.Files)
            {
                string status = newsBAL.ValidatePDFFile(Request.Files[0].ContentLength, Path.GetExtension(Request.Files[0].FileName));
                if (status != string.Empty)
                {
                    newspdfUploadModel.ErrorMessage = status;
                    return View("PdfFileUpload", newspdfUploadModel.ErrorMessage);
                }
            }

            var fileData = new List<PMGSY.Models.NewsDetails.NewsPDFUpload>();

            int newsId = 0;
            if (newspdfUploadModel.News_Id != 0)
            {
                newsId = newspdfUploadModel.News_Id;
            }
            else
            {
                try
                {
                    newsId = Convert.ToInt32(Request["News_Id"]);
                }
                catch
                {
                    if (Request["News_Id"].Contains(','))
                    {
                        newsId = Convert.ToInt32(Request["News_Id"].Split(',')[0]);
                    }
                }
            }
            foreach (string file in Request.Files)
            {
                UploadPDFFile(Request, fileData, newsId);
            }

            if (dbContext.ADMIN_NEWS_FILES.Where(a => a.NEWS_ID == newsId).Any())
            {
                newspdfUploadModel.NumberofPdfs = dbContext.ADMIN_NEWS_FILES.Where(a => a.NEWS_ID == newspdfUploadModel.News_Id).Count();
            }
            else
            {
                newspdfUploadModel.NumberofPdfs = 0;
            }

            var serializer = new JavaScriptSerializer();
            serializer.MaxJsonLength = Int32.MaxValue;

            var result = new ContentResult
            {
                Content = "{\"files\":" + serializer.Serialize(fileData) + "}",
            };
            return result;
            //return null;
        }

        /// <summary>
        /// This Method Uploads PDF File
        /// </summary>
        /// <param name="request"></param>
        /// <param name="statuses"></param>
        /// <param name="IMS_PR_ROAD_CODE"></param>
        //[Audit]
        public void UploadPDFFile(HttpRequestBase request, List<PMGSY.Models.NewsDetails.NewsPDFUpload> statuses, int newsID)
        {
            String StorageRoot = ConfigurationManager.AppSettings["NEWS_PDF_FILE_UPLOAD"];

            int MaxCount = 0;

            for (int i = 0; i < request.Files.Count; i++)
            {
                HttpPostedFileBase file = request.Files[i];
                var fileId = newsID;
                if (dbContext.ADMIN_NEWS_FILES.Where(a => a.NEWS_ID == newsID).Any())
                {
                    MaxCount = dbContext.ADMIN_NEWS_FILES.Where(a => a.NEWS_ID == newsID).Count();
                }
                MaxCount++;

                var fileName = newsID + "-" + MaxCount + Path.GetExtension(request.Files[i].FileName).ToString();
                var fullPath = Path.Combine(StorageRoot, fileName);

                statuses.Add(new PMGSY.Models.NewsDetails.NewsPDFUpload()
                {
                    url = fullPath,
                    thumbnail_url = fullPath,
                    name = fileName,
                    type = file.ContentType,
                    size = file.ContentLength,
                    delete_url = "",
                    delete_type = "DELETE",

                    PdfDescription = request.Params["PdfDescription[]"],

                    News_Id = newsID

                });

                string status = newsBAL.AddFileUploadDetailsBAL(statuses, "P");
                if (status == string.Empty)
                {

                    //file.SaveAs(fullPath);
                    file.SaveAs(Path.Combine(ConfigurationManager.AppSettings["NEWS_PDF_FILE_UPLOAD"], fileName));
                }
                else
                {
                    // show an error over here
                }
            }
        }

        /// <summary>
        /// Downloads the File
        /// </summary>
        /// <param name="parameter"></param>
        /// <param name="hash"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        [HttpGet]
        //[Audit]
        public ActionResult DownloadFile(String parameter, String hash, String key)
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

            if (FileExtension == ".pdf")
            {
                FullFileLogicalPath = Path.Combine(ConfigurationManager.AppSettings["NEWS_PDF_FILE_UPLOAD_VIRTUAL_DIR_PATH"], FileName);
                FullfilePhysicalPath = Path.Combine(ConfigurationManager.AppSettings["NEWS_PDF_FILE_UPLOAD"], FileName);
            }
            else if (FileExtension == ".jpg" || FileExtension == ".jpeg" || FileExtension == ".bmp" || FileExtension == ".png" || FileExtension == ".gif" || FileExtension == ".tiff")
            {
                FullFileLogicalPath = Path.Combine(ConfigurationManager.AppSettings["NEWS_IMAGE_FILE_UPLOAD_VIRTUAL_DIR_PATH"], FileName);
                FullfilePhysicalPath = Path.Combine(ConfigurationManager.AppSettings["NEWS_IMAGE_FILE_UPLOAD"], FileName);
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
                return Json(new { Success = "false" });
            }
        }

        /// <summary>
        /// Update the Image File Details
        /// </summary>
        /// <param name="formCollection"></param>
        /// <returns></returns>
        [HttpPost]
        //[Audit]
        public JsonResult UpdateImageDetails(FormCollection formCollection)
        {
            string[] arrKey = formCollection["id"].Split('$');
            PMGSY.Models.NewsDetails.NewsPDFUpload newsfileupload = new PMGSY.Models.NewsDetails.NewsPDFUpload();
            newsfileupload.News_Id = Convert.ToInt32(arrKey[0]);
            newsfileupload.NEWS_FILE_ID = Convert.ToInt32(arrKey[1]);

            //try
            //{
            //    Regex regexChainage = new Regex((@"^\s*(?=.*[0-9])\d*(?:\.\d{1,3})?\s*$"));
            //    if (regexChainage.IsMatch(formCollection["chainage"]) && formCollection["chainage"].Trim().Length != 0)
            //    {
            //        //fileuploadViewModel.chainage = Convert.ToDecimal(formCollection["chainage"]);
            //    }
            //    else
            //    {
            //        return Json("Invalid chainage,Can only contains Numeric values and Total 7 Digits and 3 digits after decimal place");
            //    }
            //}
            //catch
            //{
            //    return Json("Please Enter Valid Chainage.");
            //}
            Regex regex = new Regex(@"^[a-zA-Z0-9 ,.()-]+$");

            if (regex.IsMatch(formCollection["Description"]) && formCollection["Description"].Trim().Length != 0)
            {
                newsfileupload.Image_Description = formCollection["Description"];
            }
            else
            {
                return Json("Invalid Image Description, Only Alphabets, Numbers and  [,.()-] value are allowed");
            }

            PMGSY.Models.ADMIN_NEWS_FILES news_files = dbContext.ADMIN_NEWS_FILES.Where(
                 a => a.NEWS_ID == newsfileupload.News_Id &&
                    a.FILE_ID == newsfileupload.NEWS_FILE_ID
                //a.ISPF_TYPE.ToUpper() == ISPF_TYPE &&
                //a.IMS_FILE_NAME == IMS_FILE_NAME
            ).FirstOrDefault();
            news_files.FILE_DESC = newsfileupload.Image_Description;
            string status = newsDAL.UpdateImageDetailsDAL(news_files);

            if (status == string.Empty)
                return Json(true);
            else
                return Json("There is an error occured while processing your request.");
        }

        /// <summary>
        /// Update the PDF File Details
        /// </summary>
        /// <param name="formCollection"></param>
        /// <returns></returns>
        [HttpPost]
        //[Audit]
        public JsonResult UpdatePDFDetails(FormCollection formCollection)
        {
            string[] arrKey = formCollection["id"].Split('$');
            PMGSY.Models.NewsDetails.NewsPDFUpload newsfileupload = new PMGSY.Models.NewsDetails.NewsPDFUpload();
            newsfileupload.News_Id = Convert.ToInt32(arrKey[0]);
            newsfileupload.NEWS_FILE_ID = Convert.ToInt32(arrKey[1]);

            Regex regex = new Regex(@"^[a-zA-Z0-9 ,.()-]+$");
            if (regex.IsMatch(formCollection["Description"]) && formCollection["Description"].Trim().Length != 0)
            {
                newsfileupload.PdfDescription = formCollection["Description"];
            }
            else
            {
                return Json("Invalid PDF Description, Only Alphabets,Numbers and [,.()-] are allowed");
            }

            PMGSY.Models.ADMIN_NEWS_FILES news_files = new PMGSY.Models.ADMIN_NEWS_FILES();

            news_files.FILE_ID = Convert.ToInt32(newsfileupload.NEWS_FILE_ID);
            news_files.NEWS_ID = newsfileupload.News_Id;
            //news_files.ISPF_TYPE = "C";
            //news_files.ISPF_FILE_REMARK = fileuploadViewModel.PdfDescription;
            news_files.FILE_DESC = newsfileupload.PdfDescription;

            //string status = objProposalBAL.UpdatePDFDetailsBAL(fileuploadViewModel);
            string status = newsDAL.UpdatePDFDetailsDAL(news_files);

            if (status == string.Empty)
                return Json(true);
            else
                return Json("There is an error occured while processing your request.");
        }

        /// <summary>
        /// Delete File and File Details
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        //[Audit]
        public JsonResult DeleteFileDetails(string id)
        {
            String PhysicalPath = string.Empty;
            String ThumbnailPath = string.Empty;
            string NEWS_IMAGE_FILE_NAME = Request.Params["NEWS_FILE_NAME"];



            string[] encoded = Request.Params["News_Id"].Split('/');
            decryptedParameters = PMGSY.Common.URLEncrypt.DecryptParameters1(new String[] { encoded[0], encoded[1], encoded[2] });

            string[] sep = { "'", "," };
            string[] decParam = decryptedParameters["NewsCode"].Split(sep, StringSplitOptions.RemoveEmptyEntries);//Request.Params["News_Id"].Split('$');

            if (NEWS_IMAGE_FILE_NAME == null)
            {
                NEWS_IMAGE_FILE_NAME = Convert.ToString(decParam[1]).Trim();
            }

            if (Request.Params["ISPF_TYPE"].ToUpper() == "I")
            {
                PhysicalPath = ConfigurationManager.AppSettings["NEWS_IMAGE_FILE_UPLOAD"];
                ThumbnailPath = Path.Combine(Path.Combine(PhysicalPath, "thumbnails"), NEWS_IMAGE_FILE_NAME);
            }
            else if (Request.Params["ISPF_TYPE"].ToUpper() == "P")
            {
                PhysicalPath = ConfigurationManager.AppSettings["NEWS_PDF_FILE_UPLOAD"];
            }

            string[] arrParam = decParam[0].Split('$');

            //string[] arrParam = Request.Params["News_Id"].Split('$');

            int NEWS_ID = Convert.ToInt32(arrParam[0]);
            int FILE_ID = Convert.ToInt32(arrParam[1]);

            PhysicalPath = Path.Combine(PhysicalPath, NEWS_IMAGE_FILE_NAME);

            if (!System.IO.File.Exists(PhysicalPath) && !System.IO.File.Exists(ThumbnailPath))
            {
                return Json(new { Success = false, ErrorMessage = "File Not Found." });
            }

            PMGSY.Models.ADMIN_NEWS_FILES news_files = dbContext.ADMIN_NEWS_FILES.Where(
                 a => a.NEWS_ID == NEWS_ID &&
                    a.FILE_ID == FILE_ID
                //a.ISPF_TYPE.ToUpper() == ISPF_TYPE &&
                //a.IMS_FILE_NAME == IMS_FILE_NAME
            ).FirstOrDefault();

            //string status = newsDAL.DeleteFileDetailsDAL(FILE_ID, NEWS_ID, NEWS_IMAGE_FILE_NAME, Request.Params["ISPF_TYPE"]);
            string status = newsDAL.DeleteFileDetailsDAL(news_files);

            if (status == string.Empty)
            {
                try
                {
                    System.IO.File.Delete(PhysicalPath);
                    if (Request.Params["ISPF_TYPE"].ToUpper() == "I")
                    {
                        System.IO.File.Delete(ThumbnailPath);
                    }
                }
                catch (Exception ex)
                {
                    Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                    return Json(new { Success = true, ErrorMessage = ex.Message });
                }
                return Json(new { Success = true, ErrorMessage = status });
            }
            else
            {
                return Json(new { Success = false, ErrorMessage = status });
            }
        }

        /// <summary>
        /// Uploads the Image File
        /// </summary>
        /// <param name="request"></param>
        /// <param name="statuses"></param>
        /// <param name="IMS_PR_ROAD_CODE"></param>
        public void UploadImageFile(HttpRequestBase request, List<PMGSY.Models.NewsDetails.NewsPDFUpload> statuses, int newsID)
        {       
           
            String StorageRoot = ConfigurationManager.AppSettings["NEWS_IMAGE_FILE_UPLOAD"];
            int MaxCount = 0;
            for (int i = 0; i < request.Files.Count; i++)
            {
                HttpPostedFileBase file = request.Files[i];
                var fileId = newsID;
                if (dbContext.ADMIN_NEWS_FILES.Where(a => a.NEWS_ID == newsID).Any())
                {
                    //Commented Max Count Logic By Shyam, it fails when user deletes intermediate file.
                    //MaxCount = db.IMS_PROPOSAL_FILES.Where(a => a.IMS_PR_ROAD_CODE == IMS_PR_ROAD_CODE).Count(); 

                    //Take Max File Id respective to IMS_PR_ROAD_CODE
                    MaxCount = dbContext.ADMIN_NEWS_FILES.Where(a => a.NEWS_ID == newsID).Select(a => a.FILE_ID).Max();
                }
                MaxCount++;

                var fileName = newsID + "-" + MaxCount + Path.GetExtension(request.Files[i].FileName).ToString();
                var fullPath = Path.Combine(StorageRoot, fileName);

                var ThumbnailPath = Path.Combine(StorageRoot, "thumbnails");
                var FullThumbnailPath = Path.Combine(ThumbnailPath, fileName);

                statuses.Add(new PMGSY.Models.NewsDetails.NewsPDFUpload()
                {
                    url = fullPath,
                    thumbnail_url = fullPath,
                    name = fileName,
                    type = file.ContentType,
                    size = file.ContentLength,
                    delete_url = "",
                    delete_type = "DELETE",

                    chainage = Convert.ToDecimal(request.Params["chainageValue[]"]),
                    Image_Description = request.Params["remark[]"],

                    News_Id = newsID
                });

                string status = newsBAL.AddFileUploadDetailsBAL(statuses, "I");
                if (status == string.Empty)
                {
                    newsBAL.CompressImage(request.Files[0], fullPath, FullThumbnailPath);
                    file.SaveAs(fullPath);
                }
                else
                {
                    // show an error over here
                }
            }
        }

        public ActionResult DisplayNewsFiles(String parameter, String hash, String key/*string nId*/)
        {
            PMGSY.Models.NewsDetails.DisplayNewsFiles dsplnewsfiles = new Models.NewsDetails.DisplayNewsFiles();

            dbContext = new Models.PMGSYEntities();

            decryptedParameters = PMGSY.Common.URLEncrypt.DecryptParameters1(new String[] { parameter, hash, key });
            if (decryptedParameters.Count > 0)
            {
                string nId = Convert.ToString(decryptedParameters["NewsCode"]).Trim();
                if (!int.TryParse(nId, out outParam))
                {
                    return Json(false);
                }
                string FullfilePhysicalPath = string.Empty;

                SelectListItem item;
                //if (nId == null)
                //{
                //    nId = "1";
                //}
                int NewsId = Convert.ToInt32(nId.Trim());



                var s = dbContext.ADMIN_NEWS_FILES.Where(a => a.NEWS_ID == NewsId).OrderBy(a => a.FILE_ID).ToList();

                if (s.Count > 0)
                {

                    dsplnewsfiles.path = new List<SelectListItem>();
                    foreach (var a in s)
                    {
                        //switch(a.ADMIN_NEWS.ADMIN_DEPARTMENT.)
                        {
                            //  case 
                            dsplnewsfiles.IssuedBy = a.ADMIN_NEWS.UM_User_Master.UserName.Trim();//a.ADMIN_NEWS.NEWS_USER_ID.ToString();
                            dsplnewsfiles.IssuedDate = a.ADMIN_NEWS.NEWS_UPLOAD_DATE.ToString("dd/MM/yyyy hh:mm tt");
                            dsplnewsfiles.Title = a.ADMIN_NEWS.NEWS_TITLE.Trim();
                            dsplnewsfiles.Description = a.ADMIN_NEWS.NEWS_DESCRIPTION;
                        }
                        item = new SelectListItem();
                        if (a.FILE_TYPE == "P")
                        {
                            //FullFileLogicalPath = Path.Combine(ConfigurationManager.AppSettings["NEWS_PDF_FILE_UPLOAD_VIRTUAL_DIR_PATH"], FileName);
                            //FullfilePhysicalPath = Path.Combine(ConfigurationManager.AppSettings["NEWS_PDF_FILE_UPLOAD"], a.FILE_NAME.Trim());
                            FullfilePhysicalPath = Path.Combine(ConfigurationManager.AppSettings["NEWS_PDF_FILE_UPLOAD_VIRTUAL_DIR_PATH"], a.FILE_NAME.Trim());
                        }
                        else
                        {
                            //FullFileLogicalPath = Path.Combine(ConfigurationManager.AppSettings["NEWS_IMAGE_FILE_UPLOAD_VIRTUAL_DIR_PATH"], FileName);
                            //FullfilePhysicalPath = Path.Combine(ConfigurationManager.AppSettings["NEWS_IMAGE_FILE_UPLOAD"], a.FILE_NAME.Trim());
                            FullfilePhysicalPath = Path.Combine(ConfigurationManager.AppSettings["NEWS_IMAGE_FILE_UPLOAD_VIRTUAL_DIR_PATH"], a.FILE_NAME.Trim());
                        }
                        item.Text = FullfilePhysicalPath.Trim();
                        item.Value = a.FILE_TYPE.Trim();
                        dsplnewsfiles.path.Add(item);
                    }
                }
            }
            return View("DisplayNewsFiles", dsplnewsfiles);
        }

    }
}
