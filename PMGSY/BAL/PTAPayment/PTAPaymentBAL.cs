using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PMGSY.Models;
using PMGSY.Extensions;
using System.Web.Mvc;
//using System.Data.Entity.Core.Objects;
using System.Data.Entity.Core.Objects;
using PMGSY.Models.PTAPayment;
using PMGSY.DAL.PTAPayment;

namespace PMGSY.BAL.PTAPayment
{
    public class PTAPaymentBAL : IPTAPaymentBAL
    {
        IPTAPaymentDAL objPtaPayment;
        public Array GetPTAPaymentListBAL(int page, int rows, string sidx, string sord, out Int32 totalRecords, int MAST_STATE_CODE, int IMS_YEAR, int IMS_BATCH, int IMS_STREAMS, string ProposalType, decimal TOT_HON_MIN, int PMGSY_SCHEME, out PTAPaymentTotalModel model)
        {
            objPtaPayment = new PTAPaymentDAL();
            return objPtaPayment.GetPTAPaymentListDAL(page, rows, sidx, sord, out  totalRecords, MAST_STATE_CODE, IMS_YEAR, IMS_BATCH, IMS_STREAMS, ProposalType, TOT_HON_MIN, PMGSY_SCHEME, out model);
        }

        public Array GetPTAInvoiceListBAL(int page, int rows, string sidx, string sord, out Int32 totalRecords, int MAST_STATE_CODE, Int16 IMS_YEAR, int IMS_BATCH, int IMS_STREAMS, string IMS_PROPOSAL_TYPE, string PTA_SANCTIONED_BY, string PTA_INSTITUTE_NAME, decimal HON_AMOUNT,int PMGSY_SCHEME, out PTAPaymentTotalViewModel model)
        {
            objPtaPayment = new PTAPaymentDAL();
            return objPtaPayment.GetPTAInvoiceListDAL(page, rows, sidx, sord, out  totalRecords, MAST_STATE_CODE, IMS_YEAR, IMS_BATCH, IMS_STREAMS, IMS_PROPOSAL_TYPE, PTA_SANCTIONED_BY, PTA_INSTITUTE_NAME, HON_AMOUNT,PMGSY_SCHEME, out model);
        }

        public string AddPtaInvoiceDetailsBAL(PTAInvoiceViewModel ptaPaymentViewModel)
        {
            IMS_GENERATED_INVOICE_PTA ims_generated_invoice = new IMS_GENERATED_INVOICE_PTA();
            objPtaPayment = new PTAPaymentDAL();
            ims_generated_invoice.MAST_STATE_CODE = ptaPaymentViewModel.MAST_STATE_CODE;
            ims_generated_invoice.IMS_YEAR = ptaPaymentViewModel.IMS_YEAR;
            ims_generated_invoice.IMS_BATCH = ptaPaymentViewModel.IMS_BATCH;
            ims_generated_invoice.IMS_STREAM = ptaPaymentViewModel.IMS_STREAM;
            ims_generated_invoice.IMS_PROPOSAL_TYPE = ptaPaymentViewModel.IMS_PROPOSAL_TYPE;
            ims_generated_invoice.PTA_SANCTIONED_BY = ptaPaymentViewModel.PTA_SANCTIONED_BY;
            ims_generated_invoice.SAS_ABBREVATION = ptaPaymentViewModel.SAS_ABBREVATION;
            ims_generated_invoice.HONORARIUM_AMOUNT = ptaPaymentViewModel.HONORARIUM_AMOUNT;
            ims_generated_invoice.PENALTY_AMOUNT = ptaPaymentViewModel.PENALTY_AMOUNT;
            ims_generated_invoice.MAST_TDS_ID = ptaPaymentViewModel.MAST_TDS_ID;
            ims_generated_invoice.TDS_AMOUNT = ptaPaymentViewModel.TDS_AMOUNT;
            ims_generated_invoice.SC_AMOUNT = ptaPaymentViewModel.SC_AMOUNT;
            ims_generated_invoice.SERVICE_TAX_AMOUNT = ptaPaymentViewModel.SERVICE_TAX_AMOUNT;
            ims_generated_invoice.TOTAL_AMOUNT = ptaPaymentViewModel.Amount_To_Be_Paid;
            ims_generated_invoice.GENERATION_DATE = DateTime.Now;
            ims_generated_invoice.INVOICE_FILE_NO = ptaPaymentViewModel.INVOICE_FILE_NO;
            ims_generated_invoice.MAST_PMGSY_SCHEME = Convert.ToByte(ptaPaymentViewModel.PMGSY_SCHEME);
            return objPtaPayment.AddPtaInvoiceDetailsDAL(ims_generated_invoice);
        }


        public PTAInvoiceViewModel PtaPaymentReportBAL(int invoiceId)
        {
            objPtaPayment = new PTAPaymentDAL();
            return objPtaPayment.PtaPaymentReportDAL(invoiceId);
        }
        public Array GetPTAPaymenInoviceListBAL(int page, int rows, string sidx, string sord, out int totalRecords, int MAST_STATE_CODE, int IMS_YEAR, int IMS_BATCH, int IMS_STREAMS, string IMS_PROPOSAL_TYPE, int Scheme)
        {
            objPtaPayment = new PTAPaymentDAL();
            return objPtaPayment.GetPTAPaymenInoviceListDAL(page, rows, sidx, sord, out  totalRecords, MAST_STATE_CODE, IMS_YEAR, IMS_BATCH, IMS_STREAMS, IMS_PROPOSAL_TYPE, Scheme);
        }
        public Array GetPTAPaymentListBAL(int invoiceCode, int page, int rows, string sidx, string sord, out long totalRecords)
        {
            objPtaPayment = new PTAPaymentDAL();
            return objPtaPayment.GetPTAPaymentListDAL(invoiceCode, page, rows, sidx, sord, out  totalRecords);
        }
        public bool AddPTAPaymentDetailsBAL(PTAPayemntInvoiceModel ptaPaymentViewModel, ref string message)
        {
            objPtaPayment = new PTAPaymentDAL();
            return objPtaPayment.AddPTAPaymentDetailsDAL(ptaPaymentViewModel, ref message);
        }
        public PTAPayemntInvoiceModel EditPTAPaymentDetailsBAL(int PayemntCode, int IMSInvoiceCode)
        {
            objPtaPayment = new PTAPaymentDAL();
            return objPtaPayment.EditPTAPaymentDetailsDAL(PayemntCode, IMSInvoiceCode);
        }
        public bool UpdatePTAPaymentDetailsBAL(PTAPayemntInvoiceModel ptaPaymentViewModel, ref string message)
        {
            objPtaPayment = new PTAPaymentDAL();
            return objPtaPayment.UpdatePTAPaymentDetailsDAL(ptaPaymentViewModel, ref message);
        }
        public bool DeletePTAPaymentDetailsBAL(int PayemntCode, int IMSInvoiceCode, ref string message)
        {
            objPtaPayment = new PTAPaymentDAL();
            return objPtaPayment.DeletePTAPaymentDetailsDAL(PayemntCode, IMSInvoiceCode,ref message);
        }
        public bool FinalizePTAPaymentDetailsBAL(int paymentCode, int imsInvoiceCode, ref string message)
        {
            objPtaPayment = new PTAPaymentDAL();
            return objPtaPayment.FinalizePTAPaymentDetailsDAL(paymentCode, imsInvoiceCode, ref message);
        }
    }

    public interface IPTAPaymentBAL
    {
        Array GetPTAPaymentListBAL(int page, int rows, string sidx, string sord, out Int32 totalRecords, int MAST_STATE_CODE, int IMS_YEAR, int IMS_BATCH, int IMS_STREAMS, string ProposalType, decimal TOT_HON_MIN, int PMGSY_SCHEME, out PTAPaymentTotalModel model);
        Array GetPTAInvoiceListBAL(int page, int rows, string sidx, string sord, out Int32 totalRecords, int MAST_STATE_CODE, Int16 IMS_YEAR, int IMS_BATCH, int IMS_STREAMS, string IMS_PROPOSAL_TYPE, string PTA_SANCTIONED_BY, string PTA_INSTITUTE_NAME, decimal HON_AMOUNT, int PMGSY_SCHEME, out PTAPaymentTotalViewModel model);
        string AddPtaInvoiceDetailsBAL(PTAInvoiceViewModel ptaPaymentViewModel);
        PTAInvoiceViewModel PtaPaymentReportBAL(int invoiceId);
        #region PTA Payment
        Array GetPTAPaymenInoviceListBAL(int page, int rows, string sidx, string sord, out int totalRecords, int MAST_STATE_CODE, int IMS_YEAR, int IMS_BATCH, int IMS_STREAMS, string IMS_PROPOSAL_TYPE, int Scheme);
        Array GetPTAPaymentListBAL(int invoiceCode, int page, int rows, string sidx, string sord, out long totalRecords);
        bool AddPTAPaymentDetailsBAL(PTAPayemntInvoiceModel ptaPaymentViewModel, ref string message);
        PTAPayemntInvoiceModel EditPTAPaymentDetailsBAL(int PayemntCode, int IMSInvoiceCode);
        bool UpdatePTAPaymentDetailsBAL(PTAPayemntInvoiceModel ptaPaymentViewModel, ref string message);
        bool DeletePTAPaymentDetailsBAL(int paymentCode, int imsInvoiceCode, ref string message);
        bool FinalizePTAPaymentDetailsBAL(int paymentCode, int imsInvoiceCode, ref string message);
        #endregion
    }
}