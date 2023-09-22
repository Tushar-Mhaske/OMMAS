using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Configuration;
using System.Drawing;


using ImageResizer;
using System.IO;
using System.Drawing;
using System.Web.Mvc;

//using System.Data.Entity.Core.Objects;
using System.Data.Entity.Core.Objects;

namespace PMGSY.BAL.NewsDetails
{
    public class NewsBAL
    {
        PMGSY.DAL.NewsDetails.NewsDAL newsDAL = new DAL.NewsDetails.NewsDAL();

        #region Upload File Details

        /// <summary>
        ///  Lists the Files
        /// </summary>
        /// <param name="page"></param>
        /// <param name="rows"></param>
        /// <param name="sidx"></param>
        /// <param name="sord"></param>
        /// <param name="totalRecords"></param>
        /// <param name="IMS_PR_ROAD_CODE"></param>
        /// <returns></returns>
        //public Array GetFilesListBAL(int page, int rows, string sidx, string sord, out Int32 totalRecords, int IMS_PR_ROAD_CODE)
        //{
        //    return objProposalDAL.GetFilesListDAL(page, rows, sidx, sord, out totalRecords, IMS_PR_ROAD_CODE);
        //}

        /// <summary>
        /// Get the PDF Files List
        /// </summary>
        /// <param name="page"></param>
        /// <param name="rows"></param>
        /// <param name="sidx"></param>
        /// <param name="sord"></param>
        /// <param name="totalRecords"></param>
        /// <param name="IMS_PR_ROAD_CODE"></param>
        /// <returns></returns>
        //public Array GetPDFFilesListBAL(int page, int rows, string sidx, string sord, out Int32 totalRecords, int IMS_PR_ROAD_CODE)
        //{
        //    return objProposalDAL.GetPDFFilesListDAL(page, rows, sidx, sord, out totalRecords, IMS_PR_ROAD_CODE);
        //}

        /// <summary>
        /// Add File Upload Details
        /// </summary>
        /// <param name="lstFileUploadViewModel"></param>
        /// <param name="ISPF_TYPE"></param>
        /// <returns></returns>
        public string AddFileUploadDetailsBAL(List<PMGSY.Models.NewsDetails.NewsPDFUpload> lstFileUploadViewModel, string ISPF_TYPE)
        {
            List<PMGSY.Models.ADMIN_NEWS_FILES> lst_ims_news_files = new List<PMGSY.Models.ADMIN_NEWS_FILES>();

            // Image Upload
            if (ISPF_TYPE.ToUpper() == "P")
            {

                foreach (PMGSY.Models.NewsDetails.NewsPDFUpload model in lstFileUploadViewModel)
                {
                    lst_ims_news_files.Add(
                        new PMGSY.Models.ADMIN_NEWS_FILES()
                        {
                            NEWS_ID = model.News_Id,
                            FILE_ID = model.NEWS_FILE_ID,
                            FILE_NAME = model.name,
                            //CHAINAGE = model.chainage,
                            FILE_DESC = model.PdfDescription,
                            FILE_UPLOAD_DATE = System.DateTime.Now,//ISPF_IS_ACTIVE = "Y",
                            FILE_TYPE = ISPF_TYPE.Trim()
                        }
                   );
                }

            }
             //Image File Upload
            else if (ISPF_TYPE.ToUpper() == "I")
            {

                foreach (PMGSY.Models.NewsDetails.NewsPDFUpload model in lstFileUploadViewModel)
                {
                    lst_ims_news_files.Add(
                        new PMGSY.Models.ADMIN_NEWS_FILES()
                        {
                            NEWS_ID = model.News_Id,
                            FILE_ID = model.NEWS_FILE_ID,
                            FILE_NAME = model.name,
                            //CHAINAGE = model.chainage,
                            FILE_DESC = model.Image_Description,
                            FILE_UPLOAD_DATE = System.DateTime.Now,//ISPF_IS_ACTIVE = "Y",
                            FILE_TYPE = ISPF_TYPE.Trim()
                        }
                   );
                }
            }
            return newsDAL.AddFileUploadDetailsDAL(lst_ims_news_files);
        }

        /// <summary>
        /// Update the Image File Details
        /// </summary>
        /// <param name="fileuploadViewModel"></param>
        /// <returns></returns>
        //public string UpdateImageDetailsBAL(FileUploadViewModel fileuploadViewModel)
        //{
        //    IMS_PROPOSAL_FILES ims_proposal_files = new IMS_PROPOSAL_FILES();

        //    ims_proposal_files.IMS_FILE_ID = Convert.ToInt32(fileuploadViewModel.IMS_FILE_ID);
        //    ims_proposal_files.IMS_PR_ROAD_CODE = fileuploadViewModel.IMS_PR_ROAD_CODE;
        //    ims_proposal_files.ISPF_TYPE = "I";
        //    ims_proposal_files.CHAINAGE = fileuploadViewModel.chainage;
        //    ims_proposal_files.ISPF_FILE_REMARK = fileuploadViewModel.Image_Description;

        //    return objProposalDAL.UpdateImageDetailsDAL(ims_proposal_files);
        //}

        /// <summary>
        ///  Delete File and File Details
        /// </summary>
        /// <param name="IMS_FILE_ID"></param>
        /// <param name="IMS_PR_ROAD_CODE"></param>
        /// <param name="IMS_FILE_NAME"></param>
        /// <param name="ISPF_TYPE"></param>
        /// <returns></returns>
        //public string DeleteFileDetails(int IMS_FILE_ID, int IMS_PR_ROAD_CODE, string IMS_FILE_NAME, string ISPF_TYPE)
        //{
        //    IMS_PROPOSAL_FILES ims_proposal_files = db.IMS_PROPOSAL_FILES.Where(
        //        a => a.IMS_FILE_ID == IMS_FILE_ID &&
        //        a.IMS_PR_ROAD_CODE == IMS_PR_ROAD_CODE &&
        //        a.ISPF_TYPE.ToUpper() == ISPF_TYPE &&
        //        a.IMS_FILE_NAME == IMS_FILE_NAME).FirstOrDefault();

        //    return objProposalDAL.DeleteFileDetailsDAL(ims_proposal_files);
        //}

        /// <summary>
        /// Update the PDF File Details
        /// </summary>
        /// <param name="fileuploadViewModel"></param>
        /// <returns></returns>
        //public string UpdatePDFDetailsBAL(FileUploadViewModel fileuploadViewModel)
        //{
        //    IMS_PROPOSAL_FILES ims_proposal_files = new IMS_PROPOSAL_FILES();

        //    ims_proposal_files.IMS_FILE_ID = Convert.ToInt32(fileuploadViewModel.IMS_FILE_ID);
        //    ims_proposal_files.IMS_PR_ROAD_CODE = fileuploadViewModel.IMS_PR_ROAD_CODE;
        //    ims_proposal_files.ISPF_TYPE = "C";
        //    ims_proposal_files.ISPF_FILE_REMARK = fileuploadViewModel.PdfDescription;

        //    return objProposalDAL.UpdatePDFDetailsDAL(ims_proposal_files);
        //}

        /// <summary>
        /// This Compresses Image and Creates the Thumbnail
        /// </summary>
        /// <param name="httpPostedFileBase"></param>
        /// <param name="DestinitionPath"></param>
        /// <param name="ThumbnailPath"></param>
        public void CompressImage(HttpPostedFileBase httpPostedFileBase, string DestinitionPath, string ThumbnailPath)
        {
            // For Thumbnail Image            
            ImageResizer.ImageJob ThumbnailJob = new ImageResizer.ImageJob(httpPostedFileBase, ThumbnailPath,
                new ImageResizer.ResizeSettings("width=100;height=75;format=jpg;mode=max"));

            ThumbnailJob.Build();

            HttpPostedFileBase ForResizeConditions = httpPostedFileBase;

            Image image = Image.FromStream(ForResizeConditions.InputStream);
            if (image.Height < 768 || image.Width < 1024)
            {
                httpPostedFileBase.InputStream.Seek(0, SeekOrigin.Begin);
                // For Original Image
                ImageResizer.ImageJob job = new ImageResizer.ImageJob(httpPostedFileBase, DestinitionPath,
                    new ImageResizer.ResizeSettings("width=" + image.Width + ";height=" + image.Height + ";format=jpg;mode=min"));

                job.Build();
            }
            else
            {
                httpPostedFileBase.InputStream.Seek(0, SeekOrigin.Begin);
                // For Original Image
                ImageResizer.ImageJob job = new ImageResizer.ImageJob(httpPostedFileBase, DestinitionPath,
                    new ImageResizer.ResizeSettings("width=1024;height=768;format=jpg;mode=max"));

                job.Build();
            }
        }


        /// <summary>
        /// Validates the PDF File
        /// </summary>
        /// <param name="FileSize"></param>
        /// <param name="FileExtension"></param>
        /// <returns></returns>
        public string ValidatePDFFile(int FileSize, string FileExtension)
        {
            if (FileExtension.ToUpper() != ".PDF")
            {
                return "File is not PDF File";
            }
            if (FileSize > Convert.ToInt32(ConfigurationManager.AppSettings["NEWS_PDF_FILE_MAX_SIZE"]))
            {
                return "File Size Exceed the Maximum File Limit";
            }

            return string.Empty;
        }

        /// <summary>
        /// Validates the Image File
        /// </summary>
        /// <param name="FileSize"></param>
        /// <param name="FileExtension"></param>
        /// <returns></returns>
        public string ValidateImageFile(int FileSize, string FileExtension)
        {
            string ValidExtensions = ConfigurationManager.AppSettings["NEWS_IMAGE_VALID_FORMAT"];
            string[] arrValidFormats = ValidExtensions.Split('$');


            if (!arrValidFormats.Contains(FileExtension.ToLower()))
            {
                return "File is not Valid Image File";
            }
            if (FileSize > Convert.ToInt32(ConfigurationManager.AppSettings["NEWS_IMAGE_FILE_MAX_SIZE"]))
            {
                return "File Size Exceed the Maximum File Limit";
            }

            return string.Empty;
        }
        #endregion
    }
}