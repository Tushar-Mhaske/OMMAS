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
    
    public partial class SP_ACC_ASSET_LIST_DETAILS_Result
    {
        public string Voucher_No { get; set; }
        public string Voucher_date { get; set; }
        public string Head_Desc { get; set; }
        public string Cheque_No { get; set; }
        public string Cheque_Date { get; set; }
        public decimal Asset_Amount { get; set; }
        public Nullable<decimal> Entered_Asset_Amount { get; set; }
        public string Payee_Name { get; set; }
        public int Admin_nd_code { get; set; }
        public short Head_Id { get; set; }
        public long Bill_ID { get; set; }
    }
}
