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
    
    public partial class QUALITY_QM_DIRECTORS
    {
        public int ID { get; set; }
        public int DIR_ID { get; set; }
        public string DIR_ORDER_NO { get; set; }
        public System.DateTime DIR_ORDER_DATE { get; set; }
        public byte[] DIR_SCAN_SIGN { get; set; }
        public bool IS_ACTIVE { get; set; }
    
        public virtual MASTER_INFO MASTER_INFO { get; set; }
    }
}
