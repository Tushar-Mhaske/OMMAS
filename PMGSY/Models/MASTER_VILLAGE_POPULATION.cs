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
    
    public partial class MASTER_VILLAGE_POPULATION
    {
        public int MAST_VILLAGE_CODE { get; set; }
        public int MAST_CENSUS_YEAR { get; set; }
        public int MAST_VILLAGE_TOT_POP { get; set; }
        public int MAST_VILLAGE_SCST_POP { get; set; }
        public string MAST_LOCK_STATUS { get; set; }
        public Nullable<int> USERID { get; set; }
        public string IPADD { get; set; }
    
        public virtual MASTER_CENSUS_YEAR MASTER_CENSUS_YEAR { get; set; }
        public virtual MASTER_VILLAGE MASTER_VILLAGE { get; set; }
        public virtual UM_User_Master UM_User_Master { get; set; }
    }
}
