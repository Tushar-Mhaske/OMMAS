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
    
    public partial class SP_ACC_BAR_LIST_DETAILS_Result
    {
        public long Auth_ID { get; set; }
        public string Auth_No { get; set; }
        public string Auth_Date { get; set; }
        public string Txn_Name { get; set; }
        public string Agreement_Number { get; set; }
        public string Package { get; set; }
        public string Work_Name { get; set; }
        public string Payee_Name { get; set; }
        public string Bank_Name { get; set; }
        public string Account_No { get; set; }
        public Nullable<decimal> Sanctioned_Amt { get; set; }
        public Nullable<decimal> Expn_Amt { get; set; }
        public decimal Payable_Amt { get; set; }
    }
}