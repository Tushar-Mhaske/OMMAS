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
    
    public partial class MAST_TRACE_DRRP_SCORE_PMGSY3
    {
        public int MAST_TRACE_DRRP_ID { get; set; }
        public int MAST_BLOCK_CODE { get; set; }
        public int MAST_TRACEFILE_ID { get; set; }
        public Nullable<int> MAST_ER_ROAD_CODE { get; set; }
        public decimal MAST_ER_ROAD_LEN { get; set; }
        public decimal MAST_ER_ROAD_SCORE { get; set; }
        public int MAST_ER_POP_BEN { get; set; }
        public Nullable<int> USERID { get; set; }
        public string IPADD { get; set; }
    
        public virtual MASTER_BLOCK MASTER_BLOCK { get; set; }
        public virtual MAST_TRACEFILE_PMGSY3 MAST_TRACEFILE_PMGSY3 { get; set; }
        public virtual MASTER_EXISTING_ROADS MASTER_EXISTING_ROADS { get; set; }
        public virtual UM_User_Master UM_User_Master { get; set; }
    }
}