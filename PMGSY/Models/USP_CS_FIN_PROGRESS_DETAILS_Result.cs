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
    
    public partial class USP_CS_FIN_PROGRESS_DETAILS_Result
    {
        public int IMS_PR_ROAD_CODE { get; set; }
        public int EXEC_PROG_YEAR { get; set; }
        public string EXEC_PROG_MONTH { get; set; }
        public Nullable<decimal> VALUE_OF_WORK { get; set; }
        public Nullable<decimal> PAYMENT_MADE { get; set; }
        public string EXEC_FINAL_PAYMENT_FLAG { get; set; }
        public Nullable<System.DateTime> EXEC_FINAL_PAYMENT_DATE { get; set; }
    }
}
