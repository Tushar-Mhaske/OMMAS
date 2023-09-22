#region HEADER
/*
 * Project Id:

 * Project Name:OMMAS-II

 * File Name: ExecutionBAL.cs

 * Author : Vikram Nandanwar
 
 * Creation Date :19/June/2013

 * Desc : This class is used as BAL to call methods present in the DAL for Save,Edit,Update,Delete and listing of Execution screens.  
 
 */
#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PMGSY.Models.Execution;
using PMGSY.DAL.Execution;
using PMGSY.Models.Execution;
using PMGSY.Models;
using System.Data.Entity;
using System.Drawing;
using System.Web.Mvc;
using PMGSY.Common;
namespace PMGSY.BAL.Execution
{
    public class ExecutionBAL : IExecutionBAL
    {
        ExecutionDAL objDAL = new ExecutionDAL();
        PMGSYEntities dbContext = null;
        public Array GetProposalsBAL(int page, int rows, string sidx, string sord, out Int32 totalRecords, ProposalFilterViewModel proposalFilterViewModel)
        {
            return objDAL.GetProposalsDAL(page, rows, sidx, sord, out totalRecords, proposalFilterViewModel);
        }

        public bool UpdateRoadProgressDetailsITNO(ProposalFilterForITNOViewModel progressModel, ref string message)
        {
            try
            {
                return objDAL.UpdateRoadProgressDetailsITNO(progressModel, ref message);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                message = "Error Occured while processing your request.";
                return false;
            }
        }

        #region Work Program BAL Definition

        /// <summary>
        /// list of proposal details
        /// </summary>
        /// <param name="page">no. of pages</param>
        /// <param name="rows">no. of rows</param>
        /// <param name="sidx">sort column name</param>
        /// <param name="sord">sort order</param>
        /// <param name="totalRecords">total no. of records</param>
        /// <param name="proposalFilterViewModel">data containing filter details</param>
        /// <returns></returns>
        public Array GetWorkProgramList(int page, int rows, string sidx, string sord, out Int32 totalRecords, int IMS_PR_ROAD_CODE)
        {
            return objDAL.GetWorkProgramList(page, rows, sidx, sord, out totalRecords, IMS_PR_ROAD_CODE);
        }

        /// <summary>
        /// saves the Work program details
        /// </summary>
        /// <param name="workProgramViewModel"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public bool AddWorkProgramDetails(WorkProgramViewModel workProgramViewModel, ref string message)
        {
            return objDAL.AddWorkProgramDetails(workProgramViewModel, ref message);
        }

        /// <summary>
        /// updates the work program details
        /// </summary>
        /// <param name="workProgramViewModel">model containing the updates work program details</param>
        /// <param name="message">response message</param>
        /// <returns></returns>    
        public bool EditWorkProgramDetails(WorkProgramViewModel workProgramViewModel, ref string message)
        {
            return objDAL.EditWorkProgramDetails(workProgramViewModel, ref message);
        }

        /// <summary>
        /// deletes the Work Program details
        /// </summary>
        /// <param name="imsPrRoadCode">proposal id</param>
        /// <param name="headCode">head id</param>
        /// <param name="message">response message</param>
        /// <returns></returns>
        public bool DeleteWorkProgramDetails(int imsPrRoadCode, int headCode, ref string message)
        {
            return objDAL.DeleteWorkProgramDetails(imsPrRoadCode, headCode, ref message);
        }

        public WorkProgramViewModel GetWorkProgramDetails(int imsPrRoadCode, int headCode)
        {
            return objDAL.GetWorkProgramDetails(imsPrRoadCode, headCode);
        }

        public WorkProgramViewModel GetWorkProgramInformation(int IMS_PR_ROAD_CODE)
        {
            return objDAL.GetWorkProgramInformation(IMS_PR_ROAD_CODE);
        }



        #endregion Work Program BAL Definition

        #region PaymentSchedule Definition

        public Array GetPaymentScheduleList(int page, int rows, string sidx, string sord, out Int32 totalRecords, int IMS_PR_ROAD_CODE)
        {
            return objDAL.GetPaymentScheduleList(page, rows, sidx, sord, out totalRecords, IMS_PR_ROAD_CODE);
        }

        public bool AddPaymentScheduleDetails(PaymentScheduleViewModel paymentScheduleViewModel, ref string message)
        {
            return objDAL.AddPaymentScheduleDetails(paymentScheduleViewModel, ref message);
        }

        public bool EditPaymentScheduleDetails(PaymentScheduleViewModel paymentScheduleViewModel, ref string message)
        {
            return objDAL.EditPaymentScheduleDetails(paymentScheduleViewModel, ref message);
        }

        public bool DeletePaymentScheduleDetails(int imsPrRoadCode, int month, int year, ref string message)
        {
            return objDAL.DeletePaymentScheduleDetails(imsPrRoadCode, month, year, ref message);
        }

        public PaymentScheduleViewModel GetPaymentScheduleDetails(int imsPrRoadCode, int month, int year)
        {
            return objDAL.GetPaymentScheduleDetails(imsPrRoadCode, month, year);
        }

        #endregion PaymentSchedule Definition

        #region PROGRESS

        public Array GetExecutionList(int yearCode, int blockCode, string packageCode, string proposalCode, string upgradationType, int page, int rows, string sidx, string sord, out long totalRecords)
        {
            try
            {
                return objDAL.GetExecutionList(yearCode, blockCode, packageCode, proposalCode, upgradationType, page, rows, sidx, sord, out totalRecords);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                totalRecords = 0;
                return null;
            }
        }

        public Array GetRoadPhysicalProgressList(int page, int rows, string sidx, string sord, out long totalRecords, int proposalCode)
        {
            try
            {
                return objDAL.GetRoadPhysicalProgressList(page, rows, sidx, sord, out totalRecords, proposalCode);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                totalRecords = 0;
                return null;
            }

        }

        public ExecutionRoadStatusViewModel GetPhysicalRoadDetails(int proposalCode, int monthCode, int yearCode)
        {
            try
            {
                return objDAL.GetPhysicalRoadDetails(proposalCode, monthCode, yearCode);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                return null;
            }
        }

        public bool AddPhysicalProgressDetails(ExecutionRoadStatusViewModel progressModel, ref string message)
        {
            try
            {
                return objDAL.AddPhysicalProgressDetails(progressModel, ref message);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                message = "Error Occured while processing your request.";
                return false;
            }
        }

        public bool EditPhysicalRoadDetails(ExecutionRoadStatusViewModel progressModel, ref string message)
        {
            try
            {
                return objDAL.EditPhysicalRoadDetails(progressModel, ref message);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                message = "Error Occured while processing your request.";
                return false;
            }
        }

        public bool AddLSBPhysicalProgressDetails(ExecutionLSBStatusViewModel progressModel, ref string message)
        {
            try
            {
                return objDAL.AddLSBPhysicalProgressDetails(progressModel, ref message);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                message = "Error Occured while processing your request.";
                return false;
            }
        }

        public bool EditLSBPhysicalRoadDetails(ExecutionLSBStatusViewModel progressModel, ref string message)
        {
            try
            {
                return objDAL.EditLSBPhysicalRoadDetails(progressModel, ref message);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                message = "Error Occured while processing your request.";
                return false;
            }

        }

        public Array GetLSBPhysicalProgressList(int page, int rows, string sidx, string sord, out long totalRecords, int proposalCode)
        {
            try
            {
                return objDAL.GetLSBPhysicalProgressList(page, rows, sidx, sord, out totalRecords, proposalCode);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                totalRecords = 0;
                return null;
            }
        }

        public ExecutionLSBStatusViewModel GetPhysicalLSBDetails(int proposalCode, int yearCode, int monthCode)
        {
            try
            {
                return objDAL.GetPhysicalLSBDetails(proposalCode, yearCode, monthCode);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                return null;
            }
        }

        public Array GetFinancialProgressList(int page, int rows, string sidx, string sord, out long totalRecords, int proposalCode, string progressType)
        {
            try
            {
                return objDAL.GetFinancialProgressList(page, rows, sidx, sord, out totalRecords, proposalCode, progressType);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                totalRecords = 0;
                return null;
            }
        }

        public ExecutionProgressViewModel GetFinancialDetails(int proposalCode, int yearCode, int monthCode)
        {
            try
            {
                return objDAL.GetFinancialDetails(proposalCode, yearCode, monthCode);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                return null;
            }
        }

        public bool EditFinancialProgress(ExecutionProgressViewModel progressModel, ref string message)
        {
            try
            {
                return objDAL.EditFinancialProgress(progressModel, ref message);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                message = "Error Occurred While Processing Your Request.";
                return false;
            }
        }

        public bool AddFinancialProgress(ExecutionProgressViewModel progressModel, ref string message)
        {
            try
            {
                return objDAL.AddFinancialProgress(progressModel, ref message);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                message = "Error Occurred While Processing Your Request.";
                return false;
            }
        }

        public bool AddCDWorksDetails(ExecutionCDWorksViewModel cdWorksModel, ref string message)
        {
            try
            {
                return objDAL.AddCDWorksDetails(cdWorksModel, ref message);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                message = "Error Occurred while processing your request.";
                return false;
            }
        }

        public bool EditCDWorksDetails(ExecutionCDWorksViewModel cdWorksModel, ref string message)
        {
            try
            {
                return objDAL.EditCDWorksDetails(cdWorksModel, ref message);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                message = "Error Occurred while processing your request.";
                return false;
            }
        }

        public bool DeleteCDWorksDetails(int proposalCode, int cdWorksCode, ref string message)
        {
            try
            {
                return objDAL.DeleteCDWorksDetails(proposalCode, cdWorksCode, ref message);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                message = "Error Occurred while processing your request.";
                return false;
            }
        }

        public Array GetCDWorksList(int page, int rows, string sidx, string sord, out long totalRecords, int proposalCode)
        {
            try
            {
                return objDAL.GetCDWorksList(page, rows, sidx, sord, out totalRecords, proposalCode);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                totalRecords = 0;
                return null;
            }
        }

        public ExecutionCDWorksViewModel GetCdWorksDetails(int proposalCode, int cdWorksCode)
        {
            try
            {
                return objDAL.GetCdWorksDetails(proposalCode, cdWorksCode);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                return null;
            }
        }

        public bool DeletePhysicalRoadDetails(int proposalCode, int yearCode, int monthCode, ref string message)
        {
            try
            {
                return objDAL.DeletePhysicalRoadDetails(proposalCode, yearCode, monthCode, ref message);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                message = "Error Occurred while processing your request.";
                return false;
            }
        }

        public bool DeletePhysicalLSBDetails(int proposalCode, int yearCode, int monthCode, ref string message)
        {
            try
            {
                return objDAL.DeletePhysicalLSBDetails(proposalCode, yearCode, monthCode, ref message);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                message = "Error Occurred while processing your request.";
                return false;
            }
        }

        public bool DeleteFinancialRoadDetails(int proposalCode, int yearCode, int monthCode, ref string message)
        {
            try
            {
                return objDAL.DeleteFinancialRoadDetails(proposalCode, yearCode, monthCode, ref message);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                message = "Error Occurred while processing your request.";
                return false;
            }
        }

        public bool AddProgressRemarks(ProposalRemarksViewModel model, ref string message)
        {
            try
            {
                return objDAL.AddProgressRemarks(model, ref message);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                message = "Error occurred while processing your request.";
                return false;
            }
        }

        public bool CheckSplitWork(int proposalCode)
        {
            dbContext = new PMGSYEntities();
            try
            {
                if (dbContext.TEND_AGREEMENT_DETAIL.Any(m => m.IMS_PR_ROAD_CODE == proposalCode && (m.TEND_AGREEMENT_STATUS == "P" || m.TEND_AGREEMENT_STATUS == "C")))
                {
                    return true;
                }

                int agreementCode = dbContext.TEND_AGREEMENT_DETAIL.Where(m => m.IMS_PR_ROAD_CODE == proposalCode).OrderByDescending(m => m.TEND_AGREEMENT_ID).Select(m => m.TEND_AGREEMENT_CODE).FirstOrDefault();
                if ((dbContext.TEND_AGREEMENT_MASTER.Where(m => m.TEND_AGREEMENT_CODE == agreementCode && m.TEND_AGREEMENT_TYPE == "C").Select(m => m.TEND_IS_AGREEMENT_FINALIZED).FirstOrDefault() == "N"))
                {
                    return false;
                }

                if (dbContext.TEND_AGREEMENT_DETAIL.Where(m => m.TEND_AGREEMENT_CODE == agreementCode && m.IMS_PR_ROAD_CODE == proposalCode).All(m => m.TEND_AGREEMENT_STATUS == "W"))
                {
                    return false;
                }

                var lstSplitWork = dbContext.IMS_PROPOSAL_WORK.Where(m => m.IMS_PR_ROAD_CODE == proposalCode).Select(m => m.IMS_WORK_CODE);
                foreach (var item in lstSplitWork)
                {
                    if (dbContext.TEND_AGREEMENT_DETAIL.Any(m => m.IMS_WORK_CODE == item))
                    {
                        if (dbContext.TEND_AGREEMENT_DETAIL.Any(m => m.IMS_WORK_CODE == item && (m.TEND_AGREEMENT_STATUS == "P" || (m.TEND_AGREEMENT_STATUS == "C"))))
                        {
                            int agrmntCode = dbContext.TEND_AGREEMENT_DETAIL.Where(m => m.IMS_PR_ROAD_CODE == proposalCode && m.IMS_WORK_CODE == item && (m.TEND_AGREEMENT_STATUS == "C" || m.TEND_AGREEMENT_STATUS == "P")).Select(m => m.TEND_AGREEMENT_CODE).FirstOrDefault();
                            if ((dbContext.TEND_AGREEMENT_MASTER.Where(m => m.TEND_AGREEMENT_CODE == agrmntCode).Select(m => m.TEND_IS_AGREEMENT_FINALIZED).FirstOrDefault() == "N"))
                            {
                                return false;
                            }
                            continue;
                        }
                        else
                        {
                            return false;
                        }

                        //var agreementCount = (from details in dbContext.TEND_AGREEMENT_DETAIL
                        //                     join master in dbContext.TEND_AGREEMENT_MASTER on details.TEND_AGREEMENT_CODE equals master.TEND_AGREEMENT_CODE
                        //                     where details.IMS_WORK_CODE == item 
                        //                     select details).OrderByDescending(m=>m.TEND_AGREEMENT_CODE).FirstOrDefault();
                        //if (agreementCount.TEND_AGREEMENT_STATUS == "W")
                        //{
                        //    return false;
                        //}
                    }
                    else //this will be when there is no agreement present against the split work
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
                dbContext.Dispose();
            }
        }

        public Array GetRemarksList(int page, int rows, string sidx, string sord, out long totalRecords, int proposalCode)
        {
            try
            {
                return objDAL.GetRemarksList(page, rows, sidx, sord, out totalRecords, proposalCode);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                totalRecords = 0;
                return null;
            }
        }

        public bool DeleteRemark(int proposalCode, ref string message)
        {
            try
            {
                return objDAL.DeleteRemark(proposalCode, ref message);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                message = "Error occured while processing the request.";
                return false;
            }
        }

        public bool EditRemark(ProposalRemarksViewModel remarkModel, ref string message)
        {
            try
            {
                return objDAL.EditRemark(remarkModel, ref message);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                message = "Error occured while processing the request.";
                return false;
            }
        }

        public bool CheckSanctionValue(int proposalCode, decimal valueOfWork, decimal valueOfPayment, string opearation)
        {
            try
            {
                return objDAL.CheckSanctionValue(proposalCode, valueOfWork, valueOfPayment, opearation);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                return false;
            }
        }

        public bool CheckCDWorksCount(int proposalCode, string operation)
        {
            try
            {
                return objDAL.CheckCDWorksCount(proposalCode, operation);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                return false;
            }
        }

        public DateTime? GetAgreementDate(int proposalCode)
        {
            return objDAL.GetAgreementDate(proposalCode);
        }

        public bool CheckPreviousCompletedLength(int monthCode, int yearCode, int proposalCode, decimal? lengthToCompare)
        {
            dbContext = new PMGSYEntities();
            try
            {
                if (dbContext.EXEC_ROADS_MONTHLY_STATUS.Any(m => m.EXEC_PROG_YEAR == yearCode && m.IMS_PR_ROAD_CODE == proposalCode))
                {
                    var monthCount = dbContext.EXEC_ROADS_MONTHLY_STATUS.Where(m => m.EXEC_PROG_YEAR == yearCode && m.IMS_PR_ROAD_CODE == proposalCode).Select(m => m.EXEC_PROG_MONTH);
                    if (monthCount.Count() > 1)
                    {
                        EXEC_ROADS_MONTHLY_STATUS roadMaster = dbContext.EXEC_ROADS_MONTHLY_STATUS.Where(m => m.EXEC_PROG_YEAR == yearCode && m.IMS_PR_ROAD_CODE == proposalCode && m.EXEC_PROG_MONTH < monthCode).OrderByDescending(m => m.EXEC_PROG_MONTH).FirstOrDefault();
                        if (roadMaster.EXEC_COMPLETED > lengthToCompare)
                        {
                            return false;
                        }
                        else
                        {
                            return true;
                        }
                    }
                    else
                    {
                        if (dbContext.EXEC_ROADS_MONTHLY_STATUS.Any(m => m.IMS_PR_ROAD_CODE == proposalCode && m.EXEC_PROG_YEAR < yearCode))
                        {
                            int year = dbContext.EXEC_ROADS_MONTHLY_STATUS.Where(m => m.IMS_PR_ROAD_CODE == proposalCode && m.EXEC_PROG_YEAR < yearCode).OrderByDescending(m => m.EXEC_PROG_YEAR).Select(m => m.EXEC_PROG_YEAR).FirstOrDefault();
                            EXEC_ROADS_MONTHLY_STATUS roadMaster = dbContext.EXEC_ROADS_MONTHLY_STATUS.Where(m => m.IMS_PR_ROAD_CODE == proposalCode && m.EXEC_PROG_YEAR == year).OrderByDescending(m => m.EXEC_PROG_MONTH).FirstOrDefault();
                            if (roadMaster.EXEC_COMPLETED > lengthToCompare)
                            {
                                return false;
                            }
                            else
                            {
                                return true;
                            }
                        }
                    }
                }


                if (dbContext.EXEC_ROADS_MONTHLY_STATUS.Any(m => m.EXEC_PROG_YEAR < yearCode && m.EXEC_PROG_MONTH < monthCode && m.IMS_PR_ROAD_CODE == proposalCode))
                {
                    decimal? length = dbContext.EXEC_ROADS_MONTHLY_STATUS.Where(m => m.EXEC_PROG_MONTH <= monthCode && m.EXEC_PROG_YEAR <= yearCode && m.IMS_PR_ROAD_CODE == proposalCode).Select(m => m.EXEC_COMPLETED).FirstOrDefault();
                    if (length < lengthToCompare)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    return true;
                }
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                return false;
            }
            finally
            {

            }
        }

        public Array GetRoadAgreementDetailsList(int page, int rows, string sidx, string sord, out long totalRecords, int proposalCode)
        {
            try
            {
                return objDAL.GetRoadAgreementDetailsList(page, rows, sidx, sord, out totalRecords, proposalCode);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                totalRecords = 0;
                return null;
            }
        }

        public decimal? CalculateTotalValueOfWorkDone(int proposalCode)
        {
            dbContext = new PMGSYEntities();
            decimal? totalValueofWorkDone = 0;
            try
            {
                if (dbContext.EXEC_PROGRESS.Any(m => m.IMS_PR_ROAD_CODE == proposalCode))
                {
                    totalValueofWorkDone = dbContext.EXEC_PROGRESS.Where(m => m.IMS_PR_ROAD_CODE == proposalCode).OrderByDescending(m => m.EXEC_PROG_YEAR).ThenByDescending(m => m.EXEC_PROG_MONTH).Select(m => m.EXEC_VALUEOFWORK_LASTMONTH).FirstOrDefault() + dbContext.EXEC_PROGRESS.Where(m => m.IMS_PR_ROAD_CODE == proposalCode).OrderByDescending(m => m.EXEC_PROG_YEAR).ThenByDescending(m => m.EXEC_PROG_MONTH).Select(m => m.EXEC_VALUEOFWORK_THISMONTH).FirstOrDefault();
                }
                return totalValueofWorkDone;
            }
            catch (Exception)
            {
                return null;
            }
            finally
            {
                if (dbContext != null)
                {
                    dbContext.Dispose();
                }
            }
        }

        public decimal? CalculateTotalValueOfPayment(int proposalCode)
        {
            dbContext = new PMGSYEntities();
            decimal? totalValueofPayment = 0;
            try
            {
                if (dbContext.EXEC_PROGRESS.Any(m => m.IMS_PR_ROAD_CODE == proposalCode))
                {
                    totalValueofPayment = dbContext.EXEC_PROGRESS.Where(m => m.IMS_PR_ROAD_CODE == proposalCode).OrderByDescending(m => m.EXEC_PROG_YEAR).ThenByDescending(m => m.EXEC_PROG_MONTH).Select(m => m.EXEC_PAYMENT_LASTMONTH).FirstOrDefault() + dbContext.EXEC_PROGRESS.Where(m => m.IMS_PR_ROAD_CODE == proposalCode).OrderByDescending(m => m.EXEC_PROG_YEAR).ThenByDescending(m => m.EXEC_PROG_MONTH).Select(m => m.EXEC_PAYMENT_THISMONTH).FirstOrDefault();
                }
                return totalValueofPayment;
            }
            catch (Exception)
            {
                return null;
            }
            finally
            {
                if (dbContext != null)
                {
                    dbContext.Dispose();
                }
            }
        }

        #region Upload File Details

        //grid data
        public Array GetFilesListBAL(int page, int rows, string sidx, string sord, out Int32 totalRecords, int IMS_PR_ROAD_CODE)
        {
            return objDAL.GetFilesListDAL(page, rows, sidx, sord, out totalRecords, IMS_PR_ROAD_CODE);
        }

        public string AddFileUploadDetailsBAL(List<FileUploadViewModel> lstFileUploadViewModel)
        {
            List<EXEC_FILES> lst_execution_files = new List<EXEC_FILES>();

            // Image Upload
            foreach (FileUploadViewModel model in lstFileUploadViewModel)
            {
                lst_execution_files.Add(
                    new EXEC_FILES()
                    {
                        IMS_PR_ROAD_CODE = model.IMS_PR_ROAD_CODE,
                        EXEC_UPLOAD_DATE = DateTime.Now,
                        EXEC_FILE_NAME = model.name,
                        EXEC_FILE_DESC = model.Image_Description,
                        EXEC_FILE_TYPE = model.file_type,
                        EXEC_STATUS = model.status,
                        EXEC_LATITUDE = model.Latitude,
                        EXEC_LONGITUDE = model.Longitude,
                        EXEC_STAGE = model.HeadItem
                    }
               );
            }
            return objDAL.AddFileUploadDetailsDAL(lst_execution_files);
        }

        public string UpdateImageDetailsBAL(FileUploadViewModel fileuploadViewModel)
        {
            EXEC_FILES execution_files = new EXEC_FILES();

            execution_files.EXEC_FILE_ID = Convert.ToInt32(fileuploadViewModel.EXEC_FILE_ID);
            execution_files.IMS_PR_ROAD_CODE = fileuploadViewModel.IMS_PR_ROAD_CODE;
            execution_files.EXEC_FILE_DESC = fileuploadViewModel.Image_Description;

            return objDAL.UpdateImageDetailsDAL(execution_files);
        }

        public string DeleteFileDetails(int EXEC_FILE_ID, int IMS_PR_ROAD_CODE, string EXEC_FILE_NAME)
        {
            try
            {
                dbContext = new PMGSYEntities();
                EXEC_FILES execution_files = dbContext.EXEC_FILES.Where(
                    a => a.EXEC_FILE_ID == EXEC_FILE_ID &&
                    a.IMS_PR_ROAD_CODE == IMS_PR_ROAD_CODE &&
                    a.EXEC_FILE_NAME == EXEC_FILE_NAME).FirstOrDefault();

                return objDAL.DeleteFileDetailsDAL(execution_files);
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

            //DestinationImage.Save(DestinitionPath);
        }

        //Video Upload

        public string AddVideoUploadDetailsBAL(List<FileUploadViewModel> list)
        {
            List<EXEC_FILES> files = new List<EXEC_FILES>();
            foreach (FileUploadViewModel model in list)
            {
                files.Add(
                    new EXEC_FILES()
                    {
                        IMS_PR_ROAD_CODE = model.IMS_PR_ROAD_CODE,
                        EXEC_FILE_DESC = model.Image_Description,
                        EXEC_FILE_NAME = model.name,
                        //PLAN_START_CHAINAGE = model.PLAN_START_CHAINAGE,
                        //PLAN_END_CHAINAGE = model.PLAN_END_CHAINAGE,
                        EXEC_FILE_TYPE = 1,
                        EXEC_UPLOAD_DATE = DateTime.Now,
                        EXEC_STATUS = model.status
                    }
               );
            }
            return objDAL.AddFileUploadDetailsDAL(files);
        }

        public Array GetVideoFilesListBAL(int page, int rows, string sidx, string sord, out Int32 totalRecords, int IMS_PR_ROAD_CODE)
        {
            return objDAL.GetVideoFilesListDAL(page, rows, sidx, sord, out totalRecords, IMS_PR_ROAD_CODE);
        }

        public bool CheckProposalType(int proposalCode)
        {
            return objDAL.CheckProposalType(proposalCode);
        }


        #endregion

        #region TECHNOLOGY PROGRESS
        public Array GetTechnologyProgressDetailsListBAL(int? page, int? rows, string sidx, string sord, out long totalRecords, int proposalCode)
        {
            return objDAL.GetTechnologyProgressDetailsListDAL(page, rows, sidx, sord, out totalRecords, proposalCode);
        }

        public Array GetExecTechnologyProgressDetailsListBAL(int? page, int? rows, string sidx, string sord, out long totalRecords, int proposalCode, int technologyCode, int layerCode)
        {
            return objDAL.GetExecTechnologyProgressDetailsListDAL(page, rows, sidx, sord, out totalRecords, proposalCode, technologyCode, layerCode);
        }

        public string AddExecTechnologyProgressDetailsBAL(TechnologyDetailsViewModel model)
        {
            return objDAL.AddExecTechnologyProgressDetailsDAL(model);
        }
        public bool EditExecTechnologyProgressDetailsBAL(TechnologyDetailsViewModel model, ref string message)
        {
            return objDAL.EditExecTechnologyProgressDetailsDAL(model, ref message);
        }
        public bool DeleteExecTechnologyProgressDetailsBAL(int techMonhtlyCode, ref string message)
        {
            return objDAL.DeleteExecTechnologyProgressDetailsDAL(techMonhtlyCode, ref message);
        }
        #endregion

        #endregion

        #region Executing Officer

        public Array GetExecutingOfficerListBAL(int page, int rows, string sidx, string sord, out long totalRecords, int proposalCode)
        {
            try
            {
                return objDAL.GetExecutingOfficerListDAL(page, rows, sidx, sord, out totalRecords, proposalCode);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                totalRecords = 0;
                return null;
            }
        }

        public ExecutingOfficerViewModel GetExecutingOfficerDetails(int proposalCode, int ExecutingOfficerCode)
        {
            try
            {
                return objDAL.GetExecutingOfficerDetails(proposalCode, ExecutingOfficerCode);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                return null;
            }
        }

        public bool EditExecutingOfficerDetails(ExecutingOfficerViewModel executingOfficerViewModel, ref string message)
        {
            try
            {
                return objDAL.EditExecutingOfficerDetails(executingOfficerViewModel, ref message);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                message = "Error Occurred While Processing Your Request.";
                return false;
            }
        }

        public bool AddExecutingOfficerDetails(ExecutingOfficerViewModel executingOfficerViewModel, ref string message)
        {
            try
            {
                return objDAL.AddExecutingOfficerDetails(executingOfficerViewModel, ref message);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                message = "Error Occurred While Processing Your Request.";
                return false;
            }
        }

        public bool DeleteExecutingOfficerDetails(int proposalCode, int ExecutingOfficerCode, ref string message)
        {
            try
            {
                return objDAL.DeleteExecutingOfficerDetails(proposalCode, ExecutingOfficerCode, ref message);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                message = "Error Occurred while processing your request.";
                return false;
            }
        }

        #endregion

        public Array GetTechnologyProgressListBAL(int page, int rows, string sidx, string sord, out long totalRecords, int proposalCode)
        {
            try
            {
                return objDAL.GetTechnologyProgressListDAL(page, rows, sidx, sord, out totalRecords, proposalCode);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                totalRecords = 0;
                return null;
            }
        }

        #region PROPOSAL_RELATED_DETAILS

        public Array GetRoadProposalExecutionList(int page, int rows, string sidx, string sord, out long totalRecords, int proposalCode)
        {
            try
            {
                return objDAL.GetRoadProposalExecutionList(page, rows, sidx, sord, out totalRecords, proposalCode);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                totalRecords = 0;
                return null;
            }

        }

        public Array GetLSBProposalExecutionList(int page, int rows, string sidx, string sord, out long totalRecords, int proposalCode)
        {
            try
            {
                return objDAL.GetLSBProposalExecutionList(page, rows, sidx, sord, out totalRecords, proposalCode);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                totalRecords = 0;
                return null;
            }

        }

        public Array GetProposalFinancialList(int page, int rows, string sidx, string sord, out long totalRecords, int proposalCode)
        {
            try
            {
                return objDAL.GetProposalFinancialList(page, rows, sidx, sord, out totalRecords, proposalCode);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                totalRecords = 0;
                return null;
            }

        }



        #endregion

        #region PHYSICAL_PROGRESS_FOR_ITNO

        public Array GetExecutionListForITNO(int districtCode, int yearCode, int blockCode, string packageCode, string proposalCode, int page, int rows, string sidx, string sord, out long totalRecords)
        {
            try
            {
                return objDAL.GetExecutionListForITNO(districtCode, yearCode, blockCode, packageCode, proposalCode, page, rows, sidx, sord, out totalRecords);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                totalRecords = 0;
                return null;
            }
        }

        public Array GetRoadPhysicalProgressListForITNO(int page, int rows, string sidx, string sord, out long totalRecords, int proposalCode)
        {
            try
            {
                return objDAL.GetRoadPhysicalProgressListForITNO(page, rows, sidx, sord, out totalRecords, proposalCode);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                totalRecords = 0;
                return null;
            }

        }

        public Array GetLSBPhysicalProgressListForITNO(int page, int rows, string sidx, string sord, out long totalRecords, int proposalCode)
        {
            try
            {
                return objDAL.GetLSBPhysicalProgressListForITNO(page, rows, sidx, sord, out totalRecords, proposalCode);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                totalRecords = 0;
                return null;
            }
        }

        public Array GetFinancialProgressListForITNO(int page, int rows, string sidx, string sord, out long totalRecords, int proposalCode)
        {
            try
            {
                return objDAL.GetFinancialProgressListForITNO(page, rows, sidx, sord, out totalRecords, proposalCode);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                totalRecords = 0;
                return null;
            }
        }

        #endregion

        #region Habitation Details
        public Array GetHabitationListToMap(int roadCode, int page, int rows, string sidx, string sord, out long totalRecords)
        {
            try
            {
                return objDAL.GetHabitationListToMap(roadCode, page, rows, sidx, sord, out totalRecords);
            }
            catch (Exception ex)
            {
                totalRecords = 0;
                return null;
            }
        }

        public Array GetHabitationList(int roadCode, string flag, int page, int rows, string sidx, string sord, out long totalRecords)
        { 
            try
            {
                return objDAL.GetHabitationList(roadCode, flag, page, rows, sidx, sord, out totalRecords);
            }
            catch (Exception ex)
            {
                totalRecords = 0;
                return null;
            }
        }

        public bool MapHabitationToRoad(string encryptedHabCodes, string roadName, string MappingDate)
        {
            try
            {
                return objDAL.MapHabitationToRoad(encryptedHabCodes, roadName, MappingDate);
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public Array GetMappedHabitationList(int roadCode, string flag, int page, int rows, string sidx, string sord, out long totalRecords)
        { 
            try
            {
                return objDAL.GetMappedHabitationList(roadCode, flag, page, rows, sidx, sord, out totalRecords);
            }
            catch (Exception ex)
            {
                totalRecords = 0;
                return null;
            }
        }

        public bool MapClusterToRoad(int roadCode, int clusterCode)
        {
            try
            {
                return objDAL.MapClusterToRoad(roadCode, clusterCode);
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool DeleteHabitaion(int HabCode, out SelectListItem deleted) 
        {
            try
            {
                return objDAL.DeleteHabitaion(HabCode,out deleted);
            }
            catch (Exception ex)
            {
                deleted = null;
                return false;
            }
        }

        #endregion Habitation Details Ends

        #region Road Safety
        public bool AddRoadSafetyBAL(RoadSafetyViewModel model, ref string message)
        {
            return objDAL.AddRoadSafetyDetails(model, ref message);
        }

        public Array GetRoadSafetyListBAL(int roadCode, int page, int rows, string sidx, string sord, out long totalRecords)
        {
            try
            {
                return objDAL.GetRoadSafetyListDAL(roadCode, page, rows, sidx, sord, out totalRecords);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "GetMappedHabitationListBAL()");
                totalRecords = 0;
                return null;
            }
        }
        #endregion

        #region Exec Tech File Upload

        //grid data
        public Array GetExecTechFilesListBAL(int page, int rows, string sidx, string sord, out Int32 totalRecords, int IMS_PR_ROAD_CODE)
        {
            return objDAL.GetExecTechFilesListDAL(page, rows, sidx, sord, out totalRecords, IMS_PR_ROAD_CODE);
        }

        public string AddExecTechFileUploadDetailsBAL(List<FileUploadViewModel> lstFileUploadViewModel)
        {
            List<EXEC_TECH_FILES> lst_execution_files = new List<EXEC_TECH_FILES>();

            // Image Upload
            foreach (FileUploadViewModel model in lstFileUploadViewModel)
            {
                lst_execution_files.Add(
                    new EXEC_TECH_FILES()
                    {
                        IMS_PR_ROAD_CODE = model.IMS_PR_ROAD_CODE,
                        EXEC_UPLOAD_DATE = DateTime.Now,
                        EXEC_FILE_NAME = model.name,
                        EXEC_FILE_DESC = model.Image_Description,
                        //EXEC_FILE_TYPE = model.file_type,
                        EXEC_STATUS = model.status,
                        EXEC_LATITUDE = model.Latitude,
                        EXEC_LONGITUDE = model.Longitude,
                        EXEC_STAGE = model.HeadItem,
                        REMARKS = model.Remarks,
                    }
               );
            }
            return objDAL.AddExecTechFileUploadDetailsDAL(lst_execution_files);
        }

        public string UpdateExecTechImageDetailsBAL(FileUploadViewModel fileuploadViewModel)
        {
            EXEC_TECH_FILES execution_files = new EXEC_TECH_FILES();

            execution_files.EXEC_FILE_ID = Convert.ToInt32(fileuploadViewModel.EXEC_FILE_ID);
            execution_files.IMS_PR_ROAD_CODE = fileuploadViewModel.IMS_PR_ROAD_CODE;
            execution_files.EXEC_FILE_DESC = fileuploadViewModel.Image_Description;
            execution_files.REMARKS = fileuploadViewModel.Remarks;

            return objDAL.UpdateExecTechImageDetailsDAL(execution_files);
        }

        public string DeleteExecTechFileDetails(int EXEC_FILE_ID, int IMS_PR_ROAD_CODE, string EXEC_FILE_NAME)
        {
            try
            {
                dbContext = new PMGSYEntities();
                EXEC_TECH_FILES execution_files = dbContext.EXEC_TECH_FILES.Where(
                    a => a.EXEC_FILE_ID == EXEC_FILE_ID &&
                    a.IMS_PR_ROAD_CODE == IMS_PR_ROAD_CODE &&
                    a.EXEC_FILE_NAME == EXEC_FILE_NAME).FirstOrDefault();

                return objDAL.DeleteExecTechFileDetailsDAL(execution_files);
            }
            catch (Exception ex)
            {
                //Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                ErrorLog.LogError(ex, "DeleteExecTechFileDetails().BAL");
                return "There is an error while processing request";
            }
            finally
            {
                dbContext.Dispose();
            }
        }
        #endregion

        #region Execution Progress MRD
        public bool AddPhysicalProgressDetailsMRDBAL(ExecutionRoadStatusViewModelMRD progressModel, ref string message)
        { 
            return objDAL.AddPhysicalProgressDetailsMRDDAL(progressModel, ref message);
        }

        public Array GetExecutionListMRDBAL(int yearCode, int districtCode, int blockCode, string packageCode, string proposalCode, string upgradationType, int page, int rows, string sidx, string sord, out long totalRecords)
        { 
            return objDAL.GetExecutionListMRDDAL(yearCode, districtCode, blockCode, packageCode, proposalCode, upgradationType, page, rows, sidx, sord, out totalRecords);
        }

        public Array GetRoadPhysicalProgressListMRDBAL(int page, int rows, string sidx, string sord, out long totalRecords, int proposalCode)
        {
            try
            {
                return objDAL.GetRoadPhysicalProgressListMRDDAL(page, rows, sidx, sord, out totalRecords, proposalCode);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                totalRecords = 0;
                return null;
            }
        }

        public bool EditPhysicalRoadDetailsMRDBAL(ExecutionRoadStatusViewModelMRD progressModel, ref string message)
        {
            return objDAL.EditPhysicalRoadDetailsMRDDAL(progressModel, ref message);
        }
        #endregion

        #region Execution Change Work Status
        public Array GetRoadListBAL(int yearCode, int districtCode, int blockCode, int batchCode, int streamCode, string packageCode, string proposalCode, string upgradationType, int page, int rows, string sidx, string sord, out long totalRecords)
        {
            return objDAL.GetRoadList(yearCode, districtCode, blockCode, batchCode, streamCode, packageCode, proposalCode, upgradationType, page, rows, sidx, sord, out totalRecords);
        }
        /// <summary>
        /// Get road details such as road name,Length, package.
        /// </summary>
        /// <param name="IMS_PR_ROAD_CODE"></param>
        /// <returns></returns>
        public AdditionalRoadDetailsViewModel GetRoadDetails(int IMS_PR_ROAD_CODE)
        {
            return objDAL.GetCompleteRoadDetails(IMS_PR_ROAD_CODE);
        }
        public bool AddAdditionalRoadDetailsBAL(ExecutionAdditionalRoadDetails executionAdditionalRoadDetails, ref string message)
        {
            return objDAL.AddAdditionalRoadDetailsDAL(executionAdditionalRoadDetails, ref message);
        }
        //public Array GetAdditionalRoadListBAL(int roadCode, int page, int rows, string sidx, string sord, out long totalRecords)
        //{
        //    return objDAL.GetAdditionalRoadListDAL(roadCode, page, rows, sidx, sord, out totalRecords);
        //}
        public bool EditAdditionalRoadDetailsBAL(int IMS_PR_RODE_CODE)
        {
            return objDAL.EditAdditionalRoadDetailsDAL(IMS_PR_RODE_CODE);
        }
        #endregion

        #region Road Safety ATR
        public Array RSABALListSubmitted(int districtCode, int yearCode, int blockCode, string packageCode, string proposalCode, string upgradationType, int page, int rows, string sidx, string sord, out long totalRecords)
        {
            try
            {
                return objDAL.RSADALListSubmitted(districtCode, yearCode, blockCode, packageCode, proposalCode, upgradationType, page, rows, sidx, sord, out totalRecords);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                totalRecords = 0;
                return null;
            }
        }

        public Array RSABALList(int districtCode, int yearCode, int blockCode, string packageCode, string proposalCode, string upgradationType, int page, int rows, string sidx, string sord, out long totalRecords)
        {
            try
            {
                return objDAL.RSADALList(districtCode, yearCode, blockCode, packageCode, proposalCode, upgradationType, page, rows, sidx, sord, out totalRecords);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                totalRecords = 0;
                return null;
            }
        }


        public bool AddRSABAL(RSAInspectionDetails model, ref string message)
        {
            return objDAL.AddRSADAL(model, ref message);
        }

        public bool AddRSAdetailsBAL(RSAInspectionDetails model, ref string message)
        {
            return objDAL.AddRSADetailsDAL(model, ref message);
        }

        public Array GetInspectionDetailsBALList(int page, int rows, string sidx, string sord, out long totalRecords, int proposalCode)
        {
            try
            {
                return objDAL.GetInspectionDetailsDALList(page, rows, sidx, sord, out totalRecords, proposalCode);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                totalRecords = 0;
                return null;
            }
        }


        public bool AddATRByPIUBAL(RSAInspectionDetails model, ref string message)
        {
            return objDAL.AddATRByPIUDAL(model, ref message);
        }

        public bool AddATRBySQCBAL(RSAInspectionDetails model, ref string message)
        {
            return objDAL.AddATRBySQCDAL(model, ref message);
        }


        public Array GetFilesListBALByAuditor(int page, int rows, string sidx, string sord, out Int32 totalRecords, int obsId)
        {

            return objDAL.GetFilesListDALByAuditor(page, rows, sidx, sord, out totalRecords, obsId);
        }


        public string AddFileUploadDetailsBALByAuditorBAL(int IMS_PR_ROAD_CODE, string FileName, string desc)
        {

            return objDAL.AddFileUploadDetailsBALByAuditorDAL(IMS_PR_ROAD_CODE, FileName, desc);
        }


        public string DeleteFileDetailsByAuditor(int RSAId)
        {

            return objDAL.DeleteFileDetailsByAuditorDAL(RSAId);
        }



        public Array GetFilesListBALByPIU(int page, int rows, string sidx, string sord, out Int32 totalRecords, int obsId)
        {

            return objDAL.GetFilesListDALByPIU(page, rows, sidx, sord, out totalRecords, obsId);
        }


        public string AddFileUploadDetailsBALByPIUBAL(int IMS_PR_ROAD_CODE, string FileName, string desc)
        {

            return objDAL.AddFileUploadDetailsBALByPIUDAL(IMS_PR_ROAD_CODE, FileName, desc);
        }


        public string DeleteFileDetailsByPIU(int RSAId)
        {

            return objDAL.DeleteFileDetailsByPIUDAL(RSAId);
        }

        public string FinalizeRSAATRBAL(int ATRId)
        {

            return objDAL.FinalizeRSAATRDAL(ATRId);
        }

        public string FinalizeDetailsByAuditor(int ProposalCode)
        {

            return objDAL.FinalizeDetailsByAuditorDAL(ProposalCode); //FinalizeDetailsByAuditor  FinalizeRSAATRDAL
        }
        //DeleteByAuditorBAL


        public string FinalizeDetailsByPIU(int ProposalCode)
        {

            return objDAL.FinalizeDetailsByPIUDAL(ProposalCode); //FinalizeDetailsByAuditor  FinalizeRSAATRDAL
        }

        public string DeleteByAuditorBAL(int RSACode)
        {

            return objDAL.DeleteByAuditorDAL(RSACode); //FinalizeDetailsByAuditor  FinalizeRSAATRDAL
        }


        public string AddPADFByAuditorBAL(int IMS_PR_ROAD_CODE, string FileName, string desc)
        {

            return objDAL.AddPADFByAuditorDAL(IMS_PR_ROAD_CODE, FileName, desc);
        }


        public string DeletePDFDetailsByAuditor(int RSAId)
        {

            return objDAL.DeletePDFDetailsByAuditorDAL(RSAId);
        }



        public Array GetPDFListBALByAuditor(int page, int rows, string sidx, string sord, out Int32 totalRecords, int obsId)
        {

            return objDAL.GetPDFsListDALByAuditor(page, rows, sidx, sord, out totalRecords, obsId);
        }

        #endregion
    }

    public interface IExecutionBAL
    {
        #region Proposal List

        Array GetProposalsBAL(int page, int rows, string sidx, string sord, out Int32 totalRecords, ProposalFilterViewModel proposalFilterViewModel);

        #endregion Proposal List

        #region Work Program BAL Declaration

        Array GetWorkProgramList(int page, int rows, string sidx, string sord, out Int32 totalRecords, int IMS_PR_ROAD_CODE);
        bool AddWorkProgramDetails(WorkProgramViewModel workProgramViewModel, ref string message);
        bool EditWorkProgramDetails(WorkProgramViewModel workProgramViewModel, ref string message);
        bool DeleteWorkProgramDetails(int imsPrRoadCode, int headCode, ref string message);
        WorkProgramViewModel GetWorkProgramDetails(int imsPrRoadCode, int headCode);
        WorkProgramViewModel GetWorkProgramInformation(int IMS_PR_ROAD_CODE);
        #endregion Work Program BAL Declaration

        #region PaymentSchedule Declaration

        bool AddPaymentScheduleDetails(PaymentScheduleViewModel paymentScheduleViewModel, ref string message);
        bool EditPaymentScheduleDetails(PaymentScheduleViewModel paymentScheduleViewModel, ref string message);
        bool DeletePaymentScheduleDetails(int imsPrRoadCode, int month, int year, ref string message);
        PaymentScheduleViewModel GetPaymentScheduleDetails(int imsPrRoadCode, int month, int year);
        Array GetPaymentScheduleList(int page, int rows, string sidx, string sord, out Int32 totalRecords, int IMS_PR_ROAD_CODE);

        #endregion Payment Schedule Declaration

        #region PROGRESS

        Array GetExecutionList(int yearCode, int blockCode, string packageCode, string proposalCode, string upgradationType, int page, int rows, string sidx, string sord, out long totalRecords);
        Array GetRoadPhysicalProgressList(int page, int rows, string sidx, string sord, out long totalRecords, int proposalCode);
        ExecutionRoadStatusViewModel GetPhysicalRoadDetails(int proposalCode, int monthCode, int yearCode);
        bool AddPhysicalProgressDetails(ExecutionRoadStatusViewModel progressModel, ref string message);
        bool EditPhysicalRoadDetails(ExecutionRoadStatusViewModel progressModel, ref string message);
        bool AddLSBPhysicalProgressDetails(ExecutionLSBStatusViewModel progressModel, ref string message);
        bool EditLSBPhysicalRoadDetails(ExecutionLSBStatusViewModel progressModel, ref string message);
        Array GetLSBPhysicalProgressList(int page, int rows, string sidx, string sord, out long totalRecords, int proposalCode);
        ExecutionLSBStatusViewModel GetPhysicalLSBDetails(int proposalCode, int yearCode, int monthCode);
        Array GetFinancialProgressList(int page, int rows, string sidx, string sord, out long totalRecords, int proposalCode, string progressType);
        ExecutionProgressViewModel GetFinancialDetails(int proposalCode, int yearCode, int monthCode);
        bool EditFinancialProgress(ExecutionProgressViewModel progressModel, ref string message);
        bool AddFinancialProgress(ExecutionProgressViewModel progressModel, ref string message);
        bool AddCDWorksDetails(ExecutionCDWorksViewModel cdWorksModel, ref string message);
        bool EditCDWorksDetails(ExecutionCDWorksViewModel cdWorksModel, ref string message);
        bool DeleteCDWorksDetails(int proposalCode, int cdWorksCode, ref string message);
        Array GetCDWorksList(int page, int rows, string sidx, string sord, out long totalRecords, int proposalCode);
        ExecutionCDWorksViewModel GetCdWorksDetails(int proposalCode, int cdWorksCode);
        bool DeletePhysicalRoadDetails(int proposalCode, int yearCode, int monthCode, ref string message);
        bool DeletePhysicalLSBDetails(int proposalCode, int yearCode, int monthCode, ref string message);
        bool DeleteFinancialRoadDetails(int proposalCode, int yearCode, int monthCode, ref string message);
        bool AddProgressRemarks(ProposalRemarksViewModel model, ref string message);
        Array GetRemarksList(int page, int rows, string sidx, string sord, out long totalRecords, int proposalCode);
        bool DeleteRemark(int proposalCode, ref string message);
        bool EditRemark(ProposalRemarksViewModel remarkModel, ref string message);
        bool CheckSanctionValue(int proposalCode, decimal valueOfWork, decimal valueOfPayment, string operation);
        bool CheckCDWorksCount(int proposalCode, string operation);
        bool CheckProposalType(int proposalCode);
        bool CheckPreviousCompletedLength(int monthCode, int yearCode, int proposalCode, decimal? lengthToCompare);
        Array GetRoadAgreementDetailsList(int page, int rows, string sidx, string sord, out long totalRecords, int proposalCode);



        #region Upload File Details
        Array GetFilesListBAL(int page, int rows, string sidx, string sord, out Int32 totalRecords, int IMS_PR_ROAD_CODE);
        string AddFileUploadDetailsBAL(List<FileUploadViewModel> lstFileUploadViewModel);
        string UpdateImageDetailsBAL(FileUploadViewModel fileuploadViewModel);
        string DeleteFileDetails(int EXEC_FILE_ID, int IMS_PR_ROAD_CODE, string EXEC_FILE_NAME);
        void CompressImage(HttpPostedFileBase httpPostedFileBase, string DestinitionPath, string ThumbnailPath);
        Array GetVideoFilesListBAL(int page, int rows, string sidx, string sord, out Int32 totalRecords, int IMS_PR_ROAD_CODE);

        #endregion Upload File Details

        #region TECHNOLOGY PROGRESS
        Array GetTechnologyProgressDetailsListBAL(int? page, int? rows, string sidx, string sord, out long totalRecords, int proposalCode);
        Array GetExecTechnologyProgressDetailsListBAL(int? page, int? rows, string sidx, string sord, out long totalRecords, int proposalCode, int technologyCode, int layerCode);
        string AddExecTechnologyProgressDetailsBAL(TechnologyDetailsViewModel model);
        bool EditExecTechnologyProgressDetailsBAL(TechnologyDetailsViewModel model, ref string message);
        bool DeleteExecTechnologyProgressDetailsBAL(int techMonhtlyCode, ref string message);
        #endregion

        #endregion

        #region Executing Officer

        Array GetExecutingOfficerListBAL(int page, int rows, string sidx, string sord, out long totalRecords, int proposalCode);
        ExecutingOfficerViewModel GetExecutingOfficerDetails(int proposalCode, int ExecutingOfficerCode);
        bool EditExecutingOfficerDetails(ExecutingOfficerViewModel executingOfficerViewModel, ref string message);
        bool AddExecutingOfficerDetails(ExecutingOfficerViewModel executingOfficerViewModel, ref string message);
        bool DeleteExecutingOfficerDetails(int proposalCode, int ExecutingOfficerCode, ref string message);

        #endregion Execution Officer

        Array GetTechnologyProgressListBAL(int page, int rows, string sidx, string sord, out long totalRecords, int proposalCode);

        #region PROPOSAL_RELATED_DETAILS

        Array GetRoadProposalExecutionList(int page, int rows, string sidx, string sord, out long totalRecords, int proposalCode);

        Array GetLSBProposalExecutionList(int page, int rows, string sidx, string sord, out long totalRecords, int proposalCode);

        Array GetProposalFinancialList(int page, int rows, string sidx, string sord, out long totalRecords, int proposalCode);

        #endregion

        #region PHYSICAL_PROGRESS_FOR_ITNO

        Array GetExecutionListForITNO(int districtCode, int yearCode, int blockCode, string packageCode, string proposalCode, int page, int rows, string sidx, string sord, out long totalRecords);
        Array GetRoadPhysicalProgressListForITNO(int page, int rows, string sidx, string sord, out long totalRecords, int proposalCode);
        Array GetLSBPhysicalProgressListForITNO(int page, int rows, string sidx, string sord, out long totalRecords, int proposalCode);
        Array GetFinancialProgressListForITNO(int page, int rows, string sidx, string sord, out long totalRecords, int proposalCode);

        #endregion

        bool UpdateRoadProgressDetailsITNO(ProposalFilterForITNOViewModel progressModel, ref string message);

        #region Habitation Details
        Array GetHabitationListToMap(int roadCode, int page, int rows, string sidx, string sord, out long totalRecords);
        Array GetHabitationList(int roadCode, string flag, int page, int rows, string sidx, string sord, out long totalRecords);
        bool MapHabitationToRoad(string encryptedHabCodes, string roadName, string MappingDate);
        Array GetMappedHabitationList(int roadCode, string flag, int page, int rows, string sidx, string sord, out long totalRecords);
        bool MapClusterToRoad(int roadCode, int clusterCode);
        bool DeleteHabitaion(int HabCode, out SelectListItem deleted);
        #endregion Habitation Details Ends

        #region Road Safety
        bool AddRoadSafetyBAL(RoadSafetyViewModel model, ref string message);
        Array GetRoadSafetyListBAL(int roadCode, int page, int rows, string sidx, string sord, out long totalRecords);
        #endregion

        #region Exec Tech File Upload
        Array GetExecTechFilesListBAL(int page, int rows, string sidx, string sord, out int totalRecords, int IMS_PR_ROAD_CODE);
        string AddExecTechFileUploadDetailsBAL(List<FileUploadViewModel> lstFileUploadViewModel);
        string UpdateExecTechImageDetailsBAL(FileUploadViewModel fileuploadViewModel);
        string DeleteExecTechFileDetails(int EXEC_FILE_ID, int IMS_PR_ROAD_CODE, string EXEC_FILE_NAME);
        #endregion

        #region Execution Progress MRD
        bool AddPhysicalProgressDetailsMRDBAL(ExecutionRoadStatusViewModelMRD progressModel, ref string message);
        Array GetExecutionListMRDBAL(int yearCode, int districtCode, int blockCode, string packageCode, string proposalCode, string upgradationType, int page, int rows, string sidx, string sord, out long totalRecords);
        Array GetRoadPhysicalProgressListMRDBAL(int page, int rows, string sidx, string sord, out long totalRecords, int proposalCode);
        bool EditPhysicalRoadDetailsMRDBAL(ExecutionRoadStatusViewModelMRD progressModel, ref string message);
        #endregion

        #region Execution Change Work Status
        Array GetRoadListBAL(int yearCode, int districtCode, int blockCode, int batchCode, int streamCode, string packageCode, string proposalCode, string upgradationType, int page, int rows, string sidx, string sord, out long totalRecords);
        AdditionalRoadDetailsViewModel GetRoadDetails(int IMS_PR_ROAD_CODE);
        bool AddAdditionalRoadDetailsBAL(ExecutionAdditionalRoadDetails executionAdditionalRoadDetails, ref string message);
        //Array GetAdditionalRoadListBAL(int roadCode, int page, int rows, string sidx, string sord, out long totalRecords);
        bool EditAdditionalRoadDetailsBAL(int IMS_PR_RODE_CODE);
        #endregion

        #region Road Safety ATR
        Array RSABALListSubmitted(int districtCode, int yearCode, int blockCode, string packageCode, string proposalCode, string upgradationType, int page, int rows, string sidx, string sord, out long totalRecords);
        Array RSABALList(int districtCode, int yearCode, int blockCode, string packageCode, string proposalCode, string upgradationType, int page, int rows, string sidx, string sord, out long totalRecords);
        bool AddRSABAL(RSAInspectionDetails model, ref string message);
        bool AddRSAdetailsBAL(RSAInspectionDetails model, ref string message);
        Array GetInspectionDetailsBALList(int page, int rows, string sidx, string sord, out long totalRecords, int proposalCode);
        bool AddATRByPIUBAL(RSAInspectionDetails model, ref string message);

        bool AddATRBySQCBAL(RSAInspectionDetails model, ref string message);

        Array GetFilesListBALByAuditor(int page, int rows, string sidx, string sord, out Int32 totalRecords, int obsId);

        string AddFileUploadDetailsBALByAuditorBAL(int IMS_PR_ROAD_CODE, string FileName, string desc);


        string DeleteFileDetailsByAuditor(int RSAId);


        #region
        Array GetFilesListBALByPIU(int page, int rows, string sidx, string sord, out Int32 totalRecords, int obsId);
        string AddFileUploadDetailsBALByPIUBAL(int IMS_PR_ROAD_CODE, string FileName, string desc);
        string DeleteFileDetailsByPIU(int RSAId);
        #endregion


        string FinalizeRSAATRBAL(int ATRId);

        string FinalizeDetailsByAuditor(int ProposalCode);

        string FinalizeDetailsByPIU(int ProposalCode);

        string DeleteByAuditorBAL(int RSACode);

        string AddPADFByAuditorBAL(int IMS_PR_ROAD_CODE, string FileName, string desc);

        string DeletePDFDetailsByAuditor(int RSAId);

        Array GetPDFListBALByAuditor(int page, int rows, string sidx, string sord, out Int32 totalRecords, int obsId);
        #endregion
    }

}