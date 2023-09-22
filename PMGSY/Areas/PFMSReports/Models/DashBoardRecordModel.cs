using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PMGSY.Areas.PFMSReports.Models
{
    public class DashBoardRecordModel
    {
        public int Total_Payment { get; set; }
        public decimal Total_Payment_Amt { get; set; }
        public int ACK_PFMS { get; set; }
        public decimal ACK_PFMS_AMT { get; set; }
        public int PFMS_RESPONSE_PENDING { get; set; }
        public decimal PFMS_RESPONSE_PENDING_AMT { get; set; }
        public Nullable<int> REJECTED_PFMS { get; set; }
        public Nullable<decimal> REJECTED_PFMS_AMT { get; set; }
        public Nullable<int> ACK_BANK { get; set; }
        public Nullable<decimal> ACK_BANK_AMT { get; set; }
        public Nullable<int> BANK_RESPONSE_PENDING { get; set; }
        public Nullable<decimal> BANK_RESPONSE_PENDING_AMT { get; set; }
        public Nullable<int> REJECTED_BANK { get; set; }
        public Nullable<int> REJECTED_BANK_AMT { get; set; }
        public Nullable<decimal> PAYMENT_MADE_BY_BANK { get; set; }
        public Nullable<decimal> PAYMENT_MADE_BY_BANK_AMT { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
    }
}