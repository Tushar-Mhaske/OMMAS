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
    
    public partial class ACC_MONTHLY_CLOSING_MASTER
    {
        public long CLOSING_ID { get; set; }
        public int ADMIN_ND_CODE { get; set; }
        public byte CLOSING_MONTH { get; set; }
        public short CLOSING_YEAR { get; set; }
        public System.DateTime DATE_OF_CLOSING { get; set; }
        public string FUND_TYPE { get; set; }
        public short LVL_ID { get; set; }
    
        public virtual ACC_MASTER_FUND_TYPE ACC_MASTER_FUND_TYPE { get; set; }
        public virtual ACC_MASTER_LEVEL ACC_MASTER_LEVEL { get; set; }
    }
}
