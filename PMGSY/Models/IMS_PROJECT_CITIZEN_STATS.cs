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
    
    public partial class IMS_PROJECT_CITIZEN_STATS
    {
        public int MAST_STATE_CODE { get; set; }
        public int MAST_DISTRICT_CODE { get; set; }
        public int MAST_BLOCK_CODE { get; set; }
        public int MAST_PMGSY_SCHEME { get; set; }
        public int MAST_TOT_NEW_ROADS { get; set; }
        public decimal MAST_TOT_NEW_LEN { get; set; }
        public decimal MAST_TOT_NEW_COST { get; set; }
        public int MAST_TOT_NEW_COMPLETE { get; set; }
        public int MAST_TOT_NEW_PROGRESS { get; set; }
        public decimal MAST_TOT_NEW_COMP_EXP { get; set; }
        public decimal MAST_TOT_NEW_EXP { get; set; }
        public int MAST_TOT_UPGRADE_ROADS { get; set; }
        public decimal MAST_TOT_UPGRADE_LEN { get; set; }
        public decimal MAST_TOT_UPGRADE_COST { get; set; }
        public int MAST_TOT_UPGRADE_COMPLETE { get; set; }
        public int MAST_TOT_UPGRADE_PROGRESS { get; set; }
        public decimal MAST_TOT_UPGRADE_COMP_EXP { get; set; }
        public decimal MAST_TOT_UPGRADE_EXP { get; set; }
        public Nullable<int> MAST_TOT_NEW_LSB { get; set; }
        public Nullable<int> MAST_TOT_UPGRADE_LSB { get; set; }
        public Nullable<int> MAST_TOT_NEW_COMP_LSB { get; set; }
        public Nullable<int> MAST_TOT_UPGRADE_COMP_LSB { get; set; }
    
        public virtual MASTER_BLOCK MASTER_BLOCK { get; set; }
        public virtual MASTER_DISTRICT MASTER_DISTRICT { get; set; }
        public virtual MASTER_STATE MASTER_STATE { get; set; }
    }
}
