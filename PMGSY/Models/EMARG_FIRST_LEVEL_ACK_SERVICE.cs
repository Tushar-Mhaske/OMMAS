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
    using System.Collections.Generic;
    
    public partial class EMARG_FIRST_LEVEL_ACK_SERVICE
    {
        public int EMARG_ACK_ID { get; set; }
        public Nullable<int> EID { get; set; }
        public string PACKAGE_NO { get; set; }
        public string ACK_DATE { get; set; }
        public string SUCCESS_STATUS { get; set; }
        public string REJECT_STATUS { get; set; }
        public string REJECT_CODE { get; set; }
        public string REJECT_REASON { get; set; }
        public Nullable<System.DateTime> ACK_MESSAGE_DATE_TIME { get; set; }
        public string ACK_STATUS { get; set; }
        public string ACK_REJECT_CODE { get; set; }
        public string ACK_REMARKS { get; set; }
    }
}
