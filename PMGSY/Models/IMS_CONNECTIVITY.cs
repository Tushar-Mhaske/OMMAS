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
    
    public partial class IMS_CONNECTIVITY
    {
        public int MAST_STATE_CODE { get; set; }
        public int MAST_DISTRICT_CODE { get; set; }
        public byte MAST_PMGSY_SCHEME { get; set; }
        public int IMS_YEAR { get; set; }
        public int IMS_TNH_1000 { get; set; }
        public int IMS_TNH_250 { get; set; }
        public int IMS_TNH_500 { get; set; }
        public int IMS_TNH_EL_499 { get; set; }
        public int IMS_TNH_EL_249 { get; set; }
        public int IMS_TNH { get; set; }
        public int IMS_TNCH_1000 { get; set; }
        public int IMS_TNCH_250 { get; set; }
        public int IMS_TNCH_500 { get; set; }
        public int IMS_TNCH_EL_499 { get; set; }
        public int IMS_TNCH_EL_249 { get; set; }
        public int IMS_TNCH { get; set; }
        public int IMS_H2000_1000 { get; set; }
        public int IMS_H2000_250 { get; set; }
        public int IMS_H2000_500 { get; set; }
        public int IMS_H2000_EL_499 { get; set; }
        public int IMS_H2000_EL_249 { get; set; }
        public int IMS_H2000 { get; set; }
        public int IMS_H2001_1000 { get; set; }
        public int IMS_H2001_250 { get; set; }
        public int IMS_H2001_500 { get; set; }
        public int IMS_H2001_EL_499 { get; set; }
        public int IMS_H2001_EL_249 { get; set; }
        public int IMS_H2001 { get; set; }
    
        public virtual MASTER_DISTRICT MASTER_DISTRICT { get; set; }
        public virtual MASTER_STATE MASTER_STATE { get; set; }
    }
}
