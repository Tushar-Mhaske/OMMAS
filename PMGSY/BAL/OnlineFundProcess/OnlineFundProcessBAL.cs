using PMGSY.DAL.OnlineFundProcess;
using PMGSY.Models.OnlineFundRequest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PMGSY.BAL.OnlineFundProcess
{
    public class OnlineFundProcessBAL: IOnlineFundProcessBAL
    {
        private IOnlineFundProcessDAL objProcessDAL = new OnlineFundProcessDAL();

        /// <summary>
        /// returns the list of requests according to the parameters supplied
        /// </summary>
        /// <param name="page"></param>
        /// <param name="rows"></param>
        /// <param name="sidx"></param>
        /// <param name="sord"></param>
        /// <param name="totalRecords"></param>
        /// <param name="State"></param>
        /// <param name="Year"></param>
        /// <param name="Batch"></param>
        /// <param name="Collaboration"></param>
        /// <param name="Agency"></param>
        /// <param name="Scheme"></param>
        /// <returns></returns>
        public Array GetListOfOnlineFundRequestsBAL(int page, int rows, string sidx, string sord, out long totalRecords, int State, int Year, int Batch, int Collaboration, int Agency, int Scheme)
        {
            return objProcessDAL.GetListOfOnlineFundRequestsDAL(page, rows, sidx, sord, out totalRecords, State, Year, Batch, Collaboration, Agency, Scheme);
        }

        /// <summary>
        /// saves the fund request details
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public bool AddOnlineFundRequestBAL(OnlineFundRequestViewModel model,ref string message)
        {
            return objProcessDAL.AddOnlineFundRequestDAL(model, ref message);
        }

        /// <summary>
        /// updates the details of online fund request
        /// </summary>
        /// <param name="model"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public bool UpdateOnlineFundRequestBAL(OnlineFundRequestViewModel model, ref string message)
        {
            return objProcessDAL.UpdateOnlineFundRequestDAL(model, ref message);
        }

        /// <summary>
        /// deletes the entry of fund request.
        /// </summary>
        /// <param name="requestId"></param>
        /// <returns></returns>
        public bool DeleteOnlineFundRequestBAL(int requestId)
        {
            return objProcessDAL.DeleteOnlineFundRequestDAL(requestId);
        }

        /// <summary>
        /// returns the details of online fund request
        /// </summary>
        /// <param name="requestId"></param>
        /// <returns></returns>
        public OnlineFundRequestViewModel GetOnlineFundRequestDetailsBAL(int requestId)
        {
            return objProcessDAL.GetOnlineFundRequestDetailsDAL(requestId);
        }

        /// <summary>
        /// finalizes the request.
        /// </summary>
        /// <param name="requestId"></param>
        /// <returns></returns>
        public bool FinalizeRequestDetailsBAL(int requestId,out string message)
        {
            return objProcessDAL.FinalizeRequestDetailsDAL(requestId,out message);
        }

        /// <summary>
        /// returns the list of 
        /// </summary>
        /// <param name="page"></param>
        /// <param name="rows"></param>
        /// <param name="sidx"></param>
        /// <param name="sord"></param>
        /// <param name="totalRecords"></param>
        /// <param name="requestId"></param>
        /// <returns></returns>
        public Array GetProposalListBAL(int page, int rows, string sidx, string sord, out long totalRecords, int requestId)
        {
            return objProcessDAL.GetProposalListDAL(page, rows, sidx, sord, out totalRecords, requestId);
        }

        /// <summary>
        /// saves the details of observation 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public bool AddObservationDetailsBAL(RequestApprovalViewModel model)
        {
            return objProcessDAL.AddObservationDetailsDAL(model);
        }

        /// <summary>
        /// saves the uploaded file details
        /// </summary>
        /// <param name="lstModels"></param>
        /// <returns></returns>
        public bool AddDocumentDetailsBAL(List<DocumentUploadViewModel> lstModels)
        {
            return objProcessDAL.AddDocumentDetailsDAL(lstModels);
        }

        /// <summary>
        /// returns the list of documenst uploaded
        /// </summary>
        /// <param name="page"></param>
        /// <param name="rows"></param>
        /// <param name="sidx"></param>
        /// <param name="sord"></param>
        /// <param name="totalRecords"></param>
        /// <param name="requestId"></param>
        /// <returns></returns>
        public Array GetListOfDocumentsUploadedBAL(int page, int rows, string sidx, string sord, out long totalRecords, int requestId)
        {
            return objProcessDAL.GetListOfDocumentsUploadedDAL(page,rows,sidx,sord,out totalRecords,requestId);
        }

        /// <summary>
        /// returns the list of observations
        /// </summary>
        /// <param name="page"></param>
        /// <param name="rows"></param>
        /// <param name="sidx"></param>
        /// <param name="sord"></param>
        /// <param name="totalRecords"></param>
        /// <param name="requestId"></param>
        /// <returns></returns>
        public Array GetListofObservationDetailsBAL(int page, int rows, string sidx, string sord, out long totalRecords, int requestId)
        {
            return objProcessDAL.GetListofObservationDetailsDAL(page, rows, sidx, sord, out totalRecords, requestId);
        }

        /// <summary>
        /// returns the list of action required list
        /// </summary>
        /// <param name="page"></param>
        /// <param name="rows"></param>
        /// <param name="sidx"></param>
        /// <param name="sord"></param>
        /// <param name="totalRecords"></param>
        /// <returns></returns>
        public Array GetActionRequiredRequestListBAL(int page, int rows, string sidx, string sord, out long totalRecords)
        {
            return objProcessDAL.GetActionRequiredRequestListDAL(page, rows, sidx, sord, out totalRecords);
        }

        /// <summary>
        /// returns the list of in progress request list
        /// </summary>
        /// <param name="page"></param>
        /// <param name="rows"></param>
        /// <param name="sidx"></param>
        /// <param name="sord"></param>
        /// <param name="totalRecords"></param>
        /// <returns></returns>
        public Array GetInProgressRequestListBAL(int page, int rows, string sidx, string sord, out long totalRecords)
        {
            return objProcessDAL.GetInProgressRequestListDAL(page, rows, sidx, sord, out totalRecords);
        }
        
        /// <summary>
        /// returns the list of completed request list
        /// </summary>
        /// <param name="page"></param>
        /// <param name="rows"></param>
        /// <param name="sidx"></param>
        /// <param name="sord"></param>
        /// <param name="totalRecords"></param>
        /// <returns></returns>
        public Array GetCompletedRequestListBAL(int page, int rows, string sidx, string sord, out long totalRecords)
        {
            return objProcessDAL.GetCompletedRequestListDAL(page, rows, sidx, sord, out totalRecords);
        }

        /// <summary>
        /// adds the condition reply details
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public bool AddConditionReplyBAL(ConditionReplyViewModel model)
        {
            return objProcessDAL.AddConditionReplyDAL(model);
        }

        /// <summary>
        /// saves the details of UO number details
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public bool AddUODetailsBAL(OnlineFundRequestViewModel model)
        {
            return objProcessDAL.AddUODetailsDAL(model);
        }

        /// <summary>
        /// returns the list of approved request list
        /// </summary>
        /// <param name="page"></param>
        /// <param name="rows"></param>
        /// <param name="sidx"></param>
        /// <param name="sord"></param>
        /// <param name="totalRecords"></param>
        /// <returns></returns>
        public Array GetApprovedRequestListBAL(int page, int rows, string sidx, string sord, out long totalRecords)
        {
            return objProcessDAL.GetApprovedRequestListDAL(page, rows, sidx, sord, out totalRecords);
        }

        /// <summary>
        /// deletes the document details
        /// </summary>
        /// <param name="requestDocumentId"></param>
        /// <returns></returns>
        public bool DeleteDocumentDetailsBAL(int requestDocumentId)
        {
            return objProcessDAL.DeleteDocumentDetailsDAL(requestDocumentId);
        }

        /// <summary>
        /// returns the list of condition imposed list
        /// </summary>
        /// <param name="page"></param>
        /// <param name="rows"></param>
        /// <param name="sidx"></param>
        /// <param name="sord"></param>
        /// <param name="requestCode"></param>
        /// <param name="totalRecords"></param>
        /// <returns></returns>
        public Array GetConditionImposedListBAL(int page, int rows, string sidx, string sord, int requestCode, out long totalRecords)
        {
            return objProcessDAL.GetConditionImposedListDAL(page, rows, sidx, sord, requestCode, out totalRecords);
        }

    }

    public interface IOnlineFundProcessBAL
    {
        Array GetListOfOnlineFundRequestsBAL(int page, int rows, string sidx, string sord, out long totalRecords, int State, int Year, int Batch, int Collaboration, int Agency, int Scheme);
        bool AddOnlineFundRequestBAL(OnlineFundRequestViewModel model, ref string message);
        bool UpdateOnlineFundRequestBAL(OnlineFundRequestViewModel model, ref string message);
        bool DeleteOnlineFundRequestBAL(int requestId);
        OnlineFundRequestViewModel GetOnlineFundRequestDetailsBAL(int requestId);
        bool FinalizeRequestDetailsBAL(int requestId,out string message);
        Array GetProposalListBAL(int page, int rows, string sidx, string sord, out long totalRecords, int requestId);
        bool AddObservationDetailsBAL(RequestApprovalViewModel model);
        bool AddDocumentDetailsBAL(List<DocumentUploadViewModel> lstModels);
        Array GetListOfDocumentsUploadedBAL(int page, int rows, string sidx, string sord, out long totalRecords, int requestId);
        Array GetListofObservationDetailsBAL(int page, int rows, string sidx, string sord, out long totalRecords, int requestId);
        Array GetActionRequiredRequestListBAL(int page, int rows, string sidx, string sord, out long totalRecords);
        Array GetInProgressRequestListBAL(int page, int rows, string sidx, string sord, out long totalRecords);
        Array GetCompletedRequestListBAL(int page, int rows, string sidx, string sord, out long totalRecords);
        bool AddConditionReplyBAL(ConditionReplyViewModel model);
        bool AddUODetailsBAL(OnlineFundRequestViewModel model);
        Array GetApprovedRequestListBAL(int page, int rows, string sidx, string sord, out long totalRecords);
        bool DeleteDocumentDetailsBAL(int requestDocumentId);
        Array GetConditionImposedListBAL(int page, int rows, string sidx, string sord, int requestCode,out long totalRecords);
    }
}