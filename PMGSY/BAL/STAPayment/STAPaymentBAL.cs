using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PMGSY.Models;
using PMGSY.Extensions;
using System.Web.Mvc;
using System.Data.Entity.Core.Objects;
using PMGSY.Models.STAPayment;
using PMGSY.DAL.STAPayment;

namespace PMGSY.BAL.STAPayment
{
    public class STAPaymentBAL : ISTAPaymentBAL
    {
        ISTAPaymentDAL objStaPayment;
        public Array GetSTAPaymentListBAL(int page, int rows, string sidx, string sord, out Int32 totalRecords, int MAST_STATE_CODE, int IMS_YEAR, int IMS_BATCH, int IMS_STREAMS, string ProposalType, decimal TOT_HON_MIN, int PMGSY_SCHEME, out STAPaymentTotalModel model)
        {
            objStaPayment = new STAPaymentDAL();
            return objStaPayment.GetSTAPaymentListDAL(page, rows, sidx, sord, out  totalRecords, MAST_STATE_CODE, IMS_YEAR, IMS_BATCH, IMS_STREAMS, ProposalType, TOT_HON_MIN, PMGSY_SCHEME, out model);
        }

        public Array GetSTAInvoiceListBAL(int page, int rows, string sidx, string sord, out Int32 totalRecords, int MAST_STATE_CODE, Int16 IMS_YEAR, int IMS_BATCH, int IMS_STREAMS, string IMS_PROPOSAL_TYPE, string STA_SANCTIONED_BY, string STA_INSTITUTE_NAME, decimal HON_AMOUNT,int PMGSY_SCHEME, out STAPaymentTotalViewModel model)
        {
            objStaPayment = new STAPaymentDAL();
            return objStaPayment.GetSTAInvoiceListDAL(page, rows, sidx, sord, out  totalRecords, MAST_STATE_CODE, IMS_YEAR, IMS_BATCH, IMS_STREAMS, IMS_PROPOSAL_TYPE, STA_SANCTIONED_BY, STA_INSTITUTE_NAME, HON_AMOUNT,PMGSY_SCHEME, out model);
        }

        public string AddStaInvoiceDetailsBAL(STAInvoiceViewModel staPaymentViewModel)
        {
            IMS_GENERATED_INVOICE ims_generated_invoice = new IMS_GENERATED_INVOICE();
            objStaPayment = new STAPaymentDAL();
            ims_generated_invoice.MAST_STATE_CODE = staPaymentViewModel.MAST_STATE_CODE;
            ims_generated_invoice.IMS_YEAR = staPaymentViewModel.IMS_YEAR;
            ims_generated_invoice.IMS_BATCH = staPaymentViewModel.IMS_BATCH;
            ims_generated_invoice.IMS_STREAM = staPaymentViewModel.IMS_STREAM;
            ims_generated_invoice.IMS_PROPOSAL_TYPE = staPaymentViewModel.IMS_PROPOSAL_TYPE;
            ims_generated_invoice.STA_SANCTIONED_BY = staPaymentViewModel.STA_SANCTIONED_BY;
            ims_generated_invoice.SAS_ABBREVATION = staPaymentViewModel.SAS_ABBREVATION;
            ims_generated_invoice.HONORARIUM_AMOUNT = staPaymentViewModel.HONORARIUM_AMOUNT;
            ims_generated_invoice.PENALTY_AMOUNT = staPaymentViewModel.PENALTY_AMOUNT;
            ims_generated_invoice.MAST_TDS_ID = staPaymentViewModel.MAST_TDS_ID;
            ims_generated_invoice.TDS_AMOUNT = staPaymentViewModel.TDS_AMOUNT;
            ims_generated_invoice.SC_AMOUNT = staPaymentViewModel.SC_AMOUNT;
            ims_generated_invoice.SERVICE_TAX_AMOUNT = staPaymentViewModel.SERVICE_TAX_AMOUNT;
            ims_generated_invoice.TOTAL_AMOUNT = staPaymentViewModel.Amount_To_Be_Paid;
            ims_generated_invoice.GENERATION_DATE = DateTime.Now;
            ims_generated_invoice.INVOICE_FILE_NO = staPaymentViewModel.INVOICE_FILE_NO;
            ims_generated_invoice.MAST_PMGSY_SCHEME = Convert.ToByte(staPaymentViewModel.PMGSY_SCHEME);
            return objStaPayment.AddStaInvoiceDetailsDAL(ims_generated_invoice);
        }


        public STAInvoiceViewModel StaPaymentReportBAL(int invoiceId)
        {
            objStaPayment = new STAPaymentDAL();
            return objStaPayment.StaPaymentReportDAL(invoiceId);
        }
        public Array GetSTAPaymenInoviceListBAL(int page, int rows, string sidx, string sord, out int totalRecords, int MAST_STATE_CODE, int IMS_YEAR, int IMS_BATCH, int IMS_STREAMS, string IMS_PROPOSAL_TYPE, int Scheme)
        {
            objStaPayment = new STAPaymentDAL();
            return objStaPayment.GetSTAPaymenInoviceListDAL(page, rows, sidx, sord, out  totalRecords, MAST_STATE_CODE, IMS_YEAR, IMS_BATCH, IMS_STREAMS, IMS_PROPOSAL_TYPE, Scheme);
        }
        public Array GetSTAPaymentListBAL(int invoiceCode, int page, int rows, string sidx, string sord, out long totalRecords)
        {
            objStaPayment = new STAPaymentDAL();
            return objStaPayment.GetSTAPaymentListDAL(invoiceCode, page, rows, sidx, sord, out  totalRecords);
        }
        public bool AddSTAPaymentDetailsBAL(STAPayemntInvoiceModel staPaymentViewModel, ref string message)
        {
            objStaPayment = new STAPaymentDAL();
            return objStaPayment.AddSTAPaymentDetailsDAL(staPaymentViewModel, ref message);
        }
        public STAPayemntInvoiceModel EditSTAPaymentDetailsBAL(int PayemntCode, int IMSInvoiceCode)
        {
            objStaPayment = new STAPaymentDAL();
            return objStaPayment.EditSTAPaymentDetailsDAL(PayemntCode, IMSInvoiceCode);
        }
        public bool UpdateSTAPaymentDetailsBAL(STAPayemntInvoiceModel staPaymentViewModel, ref string message)
        {
            objStaPayment = new STAPaymentDAL();
            return objStaPayment.UpdateSTAPaymentDetailsDAL(staPaymentViewModel, ref message);
        }
        public bool DeleteSTAPaymentDetailsBAL(int PayemntCode, int IMSInvoiceCode, ref string message)
        {
            objStaPayment = new STAPaymentDAL();
            return objStaPayment.DeleteSTAPaymentDetailsDAL(PayemntCode, IMSInvoiceCode,ref message);
        }
        public bool FinalizeSTAPaymentDetailsBAL(int paymentCode, int imsInvoiceCode, ref string message)
        {
            objStaPayment = new STAPaymentDAL();
            return objStaPayment.FinalizeSTAPaymentDetailsDAL(paymentCode, imsInvoiceCode, ref message);
        }
    }

    public interface ISTAPaymentBAL
    {
        Array GetSTAPaymentListBAL(int page, int rows, string sidx, string sord, out Int32 totalRecords, int MAST_STATE_CODE, int IMS_YEAR, int IMS_BATCH, int IMS_STREAMS, string ProposalType, decimal TOT_HON_MIN, int PMGSY_SCHEME, out STAPaymentTotalModel model);
        Array GetSTAInvoiceListBAL(int page, int rows, string sidx, string sord, out Int32 totalRecords, int MAST_STATE_CODE, Int16 IMS_YEAR, int IMS_BATCH, int IMS_STREAMS, string IMS_PROPOSAL_TYPE, string STA_SANCTIONED_BY, string STA_INSTITUTE_NAME, decimal HON_AMOUNT, int PMGSY_SCHEME, out STAPaymentTotalViewModel model);
        string AddStaInvoiceDetailsBAL(STAInvoiceViewModel staPaymentViewModel);
        STAInvoiceViewModel StaPaymentReportBAL(int invoiceId);
        #region STA Payment
        Array GetSTAPaymenInoviceListBAL(int page, int rows, string sidx, string sord, out int totalRecords, int MAST_STATE_CODE, int IMS_YEAR, int IMS_BATCH, int IMS_STREAMS, string IMS_PROPOSAL_TYPE, int Scheme);
        Array GetSTAPaymentListBAL(int invoiceCode, int page, int rows, string sidx, string sord, out long totalRecords);
        bool AddSTAPaymentDetailsBAL(STAPayemntInvoiceModel staPaymentViewModel, ref string message);
        STAPayemntInvoiceModel EditSTAPaymentDetailsBAL(int PayemntCode, int IMSInvoiceCode);
        bool UpdateSTAPaymentDetailsBAL(STAPayemntInvoiceModel staPaymentViewModel, ref string message);
        bool DeleteSTAPaymentDetailsBAL(int paymentCode, int imsInvoiceCode, ref string message);
        bool FinalizeSTAPaymentDetailsBAL(int paymentCode, int imsInvoiceCode, ref string message);
        #endregion
    }
}