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
    
    public partial class MAST_FACILITY_DISTRICT_PMGSY3_FINALIZE_LOG
    {
        public int MAST_FACILITY_DISTRICT_FIN_CODE { get; set; }
        public int MAST_DISTRICT_CODE { get; set; }
        public System.DateTime MAST_FACILITY_DISTRICT_FINALIZE_DATE { get; set; }
        public string IS_FINALIZED { get; set; }
        public Nullable<int> USERID { get; set; }
        public string IPADD { get; set; }
        public int UNLOCKED_BY_USERID { get; set; }
        public System.DateTime UNLOCKED_BY_DATETIME { get; set; }
        public string UNLOCKED_BY_IPADD { get; set; }
    }
}
