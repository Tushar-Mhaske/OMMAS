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
    
    public partial class MASTER_ER_CBR_VALUE
    {
        public int MAST_ER_ROAD_CODE { get; set; }
        public int MAST_SEGMENT_NO { get; set; }
        public decimal MAST_STR_CHAIN { get; set; }
        public decimal MAST_END_CHAIN { get; set; }
        public int MAST_CBR_VALUE { get; set; }
        public Nullable<int> USERID { get; set; }
        public string IPADD { get; set; }
    
        public virtual MASTER_EXISTING_ROADS MASTER_EXISTING_ROADS { get; set; }
        public virtual UM_User_Master UM_User_Master { get; set; }
    }
}
