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
    
    public partial class MAST_FACILITY_BLOCK_PMGSY3_FINALIZE
    {
        public int MAST_FACILITY_BLOCK_FIN_CODE { get; set; }
        public int MAST_BLOCK_CODE { get; set; }
        public System.DateTime MAST_FACILITY_BLOCK_FINALIZE_DATE { get; set; }
        public string IS_FINALIZED { get; set; }
        public Nullable<int> USERID { get; set; }
        public string IPADD { get; set; }
    
        public virtual MASTER_BLOCK MASTER_BLOCK { get; set; }
        public virtual UM_User_Master UM_User_Master { get; set; }
    }
}
