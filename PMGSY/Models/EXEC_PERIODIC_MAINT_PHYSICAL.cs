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
    
    public partial class EXEC_PERIODIC_MAINT_PHYSICAL
    {
        public int MAINT_ID { get; set; }
        public int IMS_PR_ROAD_CODE { get; set; }
        public int EXEC_PROG_YEAR { get; set; }
        public int EXEC_PROG_MONTH { get; set; }
        public Nullable<decimal> EXEC_RENWAL_LENGTH { get; set; }
        public Nullable<int> MAST_TECH_CODE { get; set; }
        public string EXEC_ISCOMPLETED { get; set; }
        public Nullable<System.DateTime> EXEC_COMPLETION_DATE { get; set; }
        public int EXEC_IS_RENWAL { get; set; }
        public int MANE_RENEWAL_TYPE { get; set; }
        public Nullable<int> USERID { get; set; }
        public string IPADD { get; set; }
    
        public virtual MASTER_TECHNOLOGY MASTER_TECHNOLOGY { get; set; }
        public virtual MASTER_RENEWAL_TYPE MASTER_RENEWAL_TYPE { get; set; }
        public virtual IMS_SANCTIONED_PROJECTS IMS_SANCTIONED_PROJECTS { get; set; }
    }
}