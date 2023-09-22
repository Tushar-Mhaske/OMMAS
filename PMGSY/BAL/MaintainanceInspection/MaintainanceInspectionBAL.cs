using PMGSY.DAL.MaintainanceInspection;
using PMGSY.Models;
using PMGSY.Models.MaintainanceInspection;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Web;

namespace PMGSY.BAL.MaintainanceInspection
{
    public class MaintainanceInspectionBAL : IMaintenanceInspectionBAL
    {
        IMaintenanceInspectionDAL maintenanceInspectionDAL = new MaintainanceInspectionDAL();

        public Array GetFilesListBAL(int page, int rows, string sidx, string sord, out Int32 totalRecords, int IMS_PR_ROAD_CODE)
        { 
            return maintenanceInspectionDAL.GetFilesListDAL(page, rows, sidx, sord, out totalRecords, IMS_PR_ROAD_CODE);
        }

        public string AddFileUploadDetailsBAL(List<FileUploadViewModel> lstFileUploadViewModel)
        {
            List<MAINTENANCE_FILES> lst_maintenance_files = new List<MAINTENANCE_FILES>();

            // Image Upload
            foreach (FileUploadViewModel model in lstFileUploadViewModel)
            {
                lst_maintenance_files.Add(
                    new MAINTENANCE_FILES()
                    {
                        IMS_PR_ROAD_CODE = model.IMS_PR_ROAD_CODE,
                        MAINTENANCE_UPLOAD_DATE = DateTime.Now,
                        MAINTENANCE_FILE_NAME = model.name,
                        MAINTENANCE_FILE_DESC = model.Image_Description,
                        MAINTENANCE_FILE_TYPE = model.file_type,
                        MAINTENANCE_STATUS = model.status,
                        MAINTENANCE_LATITUDE = model.Latitude,
                        MAINTENANCE_LONGITUDE = model.Longitude,
                        MAINTENANCE_STAGE = model.HeadItem
                    }
               );
            }

            return maintenanceInspectionDAL.AddFileUploadDetailsDAL(lst_maintenance_files);
        }
        public string UpdateImageDetailsBAL(FileUploadViewModel fileuploadViewModel)
        {
            MAINTENANCE_FILES maintenance_files = new MAINTENANCE_FILES();

            maintenance_files.MAINTENANCE_FILE_ID = Convert.ToInt32(fileuploadViewModel.MAINTENANCE_FILE_ID);
            maintenance_files.IMS_PR_ROAD_CODE = fileuploadViewModel.IMS_PR_ROAD_CODE;
            maintenance_files.MAINTENANCE_FILE_DESC = fileuploadViewModel.Image_Description;
            return maintenanceInspectionDAL.UpdateImageDetailsDAL(maintenance_files);
        }
        public string DeleteFileDetails(int EXEC_FILE_ID, int IMS_PR_ROAD_CODE, string EXEC_FILE_NAME)
        {
            PMGSYEntities dbContext = new PMGSYEntities();
            try
            {

                MAINTENANCE_FILES maintenance_files = dbContext.MAINTENANCE_FILES.Where(
                    a => a.MAINTENANCE_FILE_ID == EXEC_FILE_ID &&
                    a.IMS_PR_ROAD_CODE == IMS_PR_ROAD_CODE &&
                    a.MAINTENANCE_FILE_NAME == EXEC_FILE_NAME).FirstOrDefault();

                return maintenanceInspectionDAL.DeleteFileDetailsDAL(maintenance_files);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                return "There is an error while processing request";
            }
            finally
            {
                dbContext.Dispose();
            }
        }
        public void CompressImage(HttpPostedFileBase httpPostedFileBase, string DestinitionPath, string ThumbnailPath)
        {
            HttpPostedFileBase newHttpPostedFileBase = httpPostedFileBase;
            Image image = Image.FromStream(httpPostedFileBase.InputStream, true, true);
            httpPostedFileBase.InputStream.Seek(0, System.IO.SeekOrigin.Begin);
            // For Thumbnail Image            
            ImageResizer.ImageJob ThumbnailJob = new ImageResizer.ImageJob(newHttpPostedFileBase, ThumbnailPath,
                new ImageResizer.ResizeSettings("width=100;height=100;format=jpg;mode=max"));
            ThumbnailJob.ResetSourceStream = true;
            ThumbnailJob.Build();
            Image thumbnailImage = Image.FromFile(ThumbnailPath);
            foreach (var item in image.PropertyItems)
            {
                thumbnailImage.SetPropertyItem(item);
            }

            //thumbnailImage.Save(ThumbnailPath);



            // For Original Image
            ImageResizer.ImageJob job = new ImageResizer.ImageJob(newHttpPostedFileBase, DestinitionPath,
                new ImageResizer.ResizeSettings("width=1024;height=768;format=jpg;mode=max"));

            job.ResetSourceStream = true;
            job.Build();

            Image DestinationImage = Image.FromFile(DestinitionPath);
            foreach (var item in image.PropertyItems)
            {
                DestinationImage.SetPropertyItem(item);
            }
            //return maintenanceInspectionDAL.CompressImage(httpPostedFileBase, DestinitionPath, ThumbnailPath)
        }

        public Array GetCompletedRoadListBAL(int stateCode, int districtCode, int blockCode, int sanctionedYear, int adminNDCode,string packageID,int batch,int collaboration,string upgradationType, int page, int rows, string sidx, string sord, out long totalRecords)
        {
            return maintenanceInspectionDAL.GetCompletedRoadListBAL(stateCode, districtCode, blockCode, sanctionedYear, adminNDCode, packageID, batch, collaboration, upgradationType, page, rows, sidx, sord, out totalRecords);
        }

        public Array GetInspectionRoadList(int proposalCode, int page, int rows, string sidx, string sord, out long totalRecords)
        {
            return maintenanceInspectionDAL.GetInspectionRoadList(proposalCode, page, rows, sidx, sord, out totalRecords);
        }

        public bool SaveInspectionDetails(MaintainanceInspectionViewModel inspectionModel, ref string message)
        {
            return maintenanceInspectionDAL.SaveInspectionDetails(inspectionModel, ref message);
        }

        public bool EditInspectionDetails(MaintainanceInspectionViewModel inspectionModel, ref string message)
        {
            return maintenanceInspectionDAL.EditInspectionDetails(inspectionModel, ref message);
        }

        public bool DeleteInspectionDetails(int prRoadCode,int inspectionCode)
        {
            return maintenanceInspectionDAL.DeleteInspectionDetails(prRoadCode, inspectionCode);
        }

        public MaintainanceInspectionViewModel GetMaintainanceInspection_ByRoadCode(int prRoadCode, int inspectionCode)
        {
            return maintenanceInspectionDAL.GetMaintainanceInspection_ByRoadCode(prRoadCode, inspectionCode);
        }

        public Array GetFinancialProgressList(int page, int rows, string sidx, string sord, out long totalRecords, int proposalCode, string progressType,int contractCode)
        {
            try
            {
                return maintenanceInspectionDAL.GetFinancialProgressList(page, rows, sidx, sord, out totalRecords, proposalCode, progressType,contractCode);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);      
                totalRecords = 0;
                return null;
            }
        }

        public MaintenanceProgressViewModel GetFinancialDetails(int proposalCode, int contractCode,int yearCode, int monthCode)
        {
            try
            {
                return maintenanceInspectionDAL.GetFinancialDetails(proposalCode, contractCode, yearCode, monthCode);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);          
           
                return null;
            }
        }

        public bool EditFinancialProgress(MaintenanceProgressViewModel progressModel, ref string message)
        {
            try
            {
                return maintenanceInspectionDAL.EditFinancialProgress(progressModel, ref message);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                message = "Error Occurred While Processing Your Request.";
                return false;
            }
        }

        public bool AddFinancialProgress(MaintenanceProgressViewModel progressModel, ref string message)
        {
            try
            {
                return maintenanceInspectionDAL.AddFinancialProgress(progressModel, ref message);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                message = "Error Occurred While Processing Your Request.";
                return false;
            }
        }

        public bool DeleteFinancialRoadDetails(int proposalCode, int contractCode, int yearCode, int monthCode, ref string message)
        {
            try
            {
                return maintenanceInspectionDAL.DeleteFinancialRoadDetails(proposalCode, contractCode, yearCode, monthCode, ref message);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                message = "Error Occurred while processing your request.";
                return false;
            }
        }

        public bool CheckSanctionValue(int proposalCode, decimal valueOfWork, decimal valueOfPayment, string opearation,int contractCode)
        {
            try
            {
                return maintenanceInspectionDAL.CheckSanctionValue(proposalCode, valueOfWork, valueOfPayment, opearation,contractCode);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                return false;
            }
        }

        public bool CheckMonthYear(int proposalCode,int month,int year,int contractCode)
        {
            PMGSYEntities dbContext = new PMGSYEntities();
            try
            {
                if (dbContext.MANE_IMS_CONTRACT.Any(m => m.IMS_PR_ROAD_CODE == proposalCode && m.MANE_PR_CONTRACT_CODE == contractCode))
                {
                    //int maxContractCode = dbContext.MANE_IMS_CONTRACT.Where(m => m.IMS_PR_ROAD_CODE == proposalCode && m.MANE_PR_CONTRACT_CODE == contractCode).OrderByDescending(m => m.MANE_PR_CONTRACT_CODE).Select(m => m.MANE_PR_CONTRACT_CODE).FirstOrDefault();
                    if (dbContext.MANE_IMS_PROGRESS.Any(m=>m.MANE_PROG_MONTH == month && m.MANE_PROG_YEAR == year && m.IMS_PR_ROAD_CODE == proposalCode && m.MANE_MAINTENANCE_NUMBER == contractCode))
                    {
                        return false;
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                return false;
            }
            finally
            {
                if (dbContext != null)
                {
                    dbContext.Dispose();
                }
            }
        }

        public Array GetAgreementDetailsList(int page, int rows, string sidx, string sord, out long totalRecords, int proposalCode, string progressType, int contractCode)
        {
            try
            {
                return maintenanceInspectionDAL.GetAgreementDetailsList(page, rows, sidx, sord, out totalRecords, proposalCode, progressType, contractCode);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                totalRecords = 0;
                return null;
            }
        }

        #region Photo Upload

        public string AddFileUploadDetailsBAL(List<PMGSY.Models.MaintainanceInspection.FileUploadViewModelProgress> lstFileUploadViewModel)
        {
            // IMaintenanceAgreementDAL maintenanceAgreementDAL = new MaintenanceAgreementDAL();
            PMGSY.Models.PMGSYEntities dbContext = new PMGSY.Models.PMGSYEntities();

            try
            {
                // Image Upload
                PMGSY.Models.MANE_IMS_PROGRESS_FILES qualityQMInspectionFile = new PMGSY.Models.MANE_IMS_PROGRESS_FILES();





                foreach (PMGSY.Models.MaintainanceInspection.FileUploadViewModelProgress model in lstFileUploadViewModel)
                {
                    qualityQMInspectionFile.IMS_PR_ROAD_CODE = model.IMS_PR_ROAD_CODE;
                    qualityQMInspectionFile.FILE_NAME = model.name.Trim();
                    qualityQMInspectionFile.FILE_DESC = model.Image_Description.Trim();
                    qualityQMInspectionFile.LATITUDE = null;
                    qualityQMInspectionFile.LONGITUDE = null;
                    qualityQMInspectionFile.START_CHAINAGE = model.Startchainage; // Added on 19 Jan 2020
                    qualityQMInspectionFile.END_CHAINAGE = model.Endchainage;// Added on 19 Jan 2020

                    qualityQMInspectionFile.SEGMENT_LENGTH = model.Endchainage - model.Startchainage;

                    qualityQMInspectionFile.FILE_UPLOAD_DATE = DateTime.Now;
                    qualityQMInspectionFile.USERID = PMGSY.Extensions.PMGSYSession.Current.UserId;
                    qualityQMInspectionFile.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                }

                return maintenanceInspectionDAL.AddFileUploadDetailsDAL(qualityQMInspectionFile);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                return ("Error Occurred While Processing Request.");
            }
            finally
            {
                dbContext.Dispose();
            }
        }




        public Array GetFilesListBALProgress(int page, int rows, string sidx, string sord, out Int32 totalRecords, int obsId)
        {
            // maintenanceInspectionDAL maintenanceAgreementDAL = new MaintenanceAgreementDAL();
            return maintenanceInspectionDAL.GetFilesListDALProgress(page, rows, sidx, sord, out totalRecords, obsId);
        }
        public string DeleteFileDetailsProgress(int QM_FILE_ID)
        {
            //  maintenanceInspectionDAL maintenanceAgreementDAL = new MaintenanceAgreementDAL();
            return maintenanceInspectionDAL.DeleteFileDetailsDALProgress(QM_FILE_ID);
        }

        #endregion

    }
    public interface IMaintenanceInspectionBAL
    {
        Array GetFilesListBAL(int page, int rows, string sidx, string sord, out Int32 totalRecords, int IMS_PR_ROAD_CODE);
        string AddFileUploadDetailsBAL(List<FileUploadViewModel> lstFileUploadViewModel);
        string UpdateImageDetailsBAL(FileUploadViewModel fileuploadViewModel);
        string DeleteFileDetails(int EXEC_FILE_ID, int IMS_PR_ROAD_CODE, string EXEC_FILE_NAME);
        void CompressImage(HttpPostedFileBase httpPostedFileBase, string DestinitionPath, string ThumbnailPath);

        Array GetCompletedRoadListBAL(int stateCode, int districtCode, int blockCode, int sanctionedYear, int adminNDCode,string packageID,int batch,int collaboration,string upgradationType, int page, int rows, string sidx, string sord, out long totalRecords);

        Array GetInspectionRoadList(int proposalCode, int page, int rows, string sidx, string sord, out long totalRecords);
        bool SaveInspectionDetails(MaintainanceInspectionViewModel inspectionModel, ref string message);
        bool EditInspectionDetails(MaintainanceInspectionViewModel inspectionModel, ref string message);
        bool DeleteInspectionDetails(int prRoadCode, int inspectionCode);
        MaintainanceInspectionViewModel GetMaintainanceInspection_ByRoadCode(int prRoadCode, int inspectionCode);

        Array GetFinancialProgressList(int page, int rows, string sidx, string sord, out long totalRecords, int proposalCode, string progressType,int contractCode);
        MaintenanceProgressViewModel GetFinancialDetails(int proposalCode, int contractCode, int yearCode, int monthCode);
        bool EditFinancialProgress(MaintenanceProgressViewModel progressModel, ref string message);
        bool AddFinancialProgress(MaintenanceProgressViewModel progressModel, ref string message);
        bool DeleteFinancialRoadDetails(int proposalCode, int contractCode, int yearCode, int monthCode, ref string message);
        bool CheckSanctionValue(int proposalCode, decimal valueOfWork, decimal valueOfPayment, string opearation,int contractCode);
        bool CheckMonthYear(int proposalCode, int month, int year, int contractCode);
        Array GetAgreementDetailsList(int page, int rows, string sidx, string sord, out long totalRecords, int proposalCode, string progressType, int contractCode);


        //Array GetAgreementDetailsListBAL_Proposal(int IMSPRRoadCode, int page, int rows, string sidx, string sord, out long totalRecords);

        ////bool SaveAgreementDetailsBAL_Proposal(MaintenanceAgreementDetails details_agreement, ref string message);

        //bool GetExistingAgreementDetails_BAL(int IMSPRRoadCode, ref string agreementDetails, ref decimal? year1, ref decimal? year2, ref decimal? year3, ref decimal? year4, ref decimal? year5);

        #region Photo Upload

        string AddFileUploadDetailsBAL(List<PMGSY.Models.MaintainanceInspection.FileUploadViewModelProgress> lstFileUploadViewModel);

        Array GetFilesListBALProgress(int page, int rows, string sidx, string sord, out Int32 totalRecords, int obsId);


        string DeleteFileDetailsProgress(int QM_FILE_ID);
        #endregion
    }
}