//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace PMGSY.Models
{
    using System;
    
    public partial class SP_ACC_RPT_LEDGER_CONTRACTOR_WORK_Result
    {
        public string BILL_NO { get; set; }
        public string Bill_Date { get; set; }
        public decimal AdvancePayment { get; set; }
        public decimal SecuredAdvance { get; set; }
        public decimal MobilisationAdvance { get; set; }
        public decimal Machineryadvance { get; set; }
        public decimal Materialsissued { get; set; }
        public string NameOfWork { get; set; }
        public string NARRATION { get; set; }
        public Nullable<decimal> GrossDebit { get; set; }
        public Nullable<decimal> GrossCredit { get; set; }
        public Nullable<decimal> TotalValue { get; set; }
        public string Remarks { get; set; }
    }
}
