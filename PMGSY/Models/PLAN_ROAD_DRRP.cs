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
    
    public partial class PLAN_ROAD_DRRP
    {
        public int PLAN_CN_ROAD_CODE { get; set; }
        public int MAST_ER_ROAD_CODE { get; set; }
        public string PLAN_RD_LENG { get; set; }
        public Nullable<decimal> PLAN_RD_FROM_CHAINAGE { get; set; }
        public Nullable<decimal> PLAN_RD_TO_CHAINAGE { get; set; }
        public Nullable<decimal> PLAN_RD_LENGTH { get; set; }
        public string PLAN_LOCK_STATUS { get; set; }
        public Nullable<int> USERID { get; set; }
        public string IPADD { get; set; }
    
        public virtual MASTER_EXISTING_ROADS MASTER_EXISTING_ROADS { get; set; }
        public virtual UM_User_Master UM_User_Master { get; set; }
        public virtual PLAN_ROAD PLAN_ROAD { get; set; }
    }
}
