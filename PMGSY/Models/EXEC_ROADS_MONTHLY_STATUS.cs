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
    
    public partial class EXEC_ROADS_MONTHLY_STATUS
    {
        public int IMS_PR_ROAD_CODE { get; set; }
        public int EXEC_PROG_YEAR { get; set; }
        public int EXEC_PROG_MONTH { get; set; }
        public Nullable<decimal> EXEC_PREPARATORY_WORK { get; set; }
        public Nullable<decimal> EXEC_EARTHWORK_SUBGRADE { get; set; }
        public Nullable<decimal> EXEC_SUBBASE_PREPRATION { get; set; }
        public Nullable<decimal> EXEC_BASE_COURSE { get; set; }
        public Nullable<decimal> EXEC_COMPLETED { get; set; }
        public Nullable<decimal> EXEC_SURFACE_COURSE { get; set; }
        public Nullable<decimal> EXEC_SIGNS_STONES { get; set; }
        public Nullable<decimal> EXEC_CD_WORKS { get; set; }
        public Nullable<decimal> EXEC_LSB_WORKS { get; set; }
        public Nullable<decimal> EXEC_MISCELANEOUS { get; set; }
        public string EXEC_ISCOMPLETED { get; set; }
        public Nullable<System.DateTime> EXEC_COMPLETION_DATE { get; set; }
        public Nullable<int> USERID { get; set; }
        public string IPADD { get; set; }
    
        public virtual UM_User_Master UM_User_Master { get; set; }
        public virtual IMS_SANCTIONED_PROJECTS IMS_SANCTIONED_PROJECTS { get; set; }
    }
}