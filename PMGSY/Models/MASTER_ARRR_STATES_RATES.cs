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
    
    public partial class MASTER_ARRR_STATES_RATES
    {
        public int MAST_ARRR_RATE_CODE { get; set; }
        public int MAST_STATE_CODE { get; set; }
        public int MAST_LMM_TYPE_CODE { get; set; }
        public int MAST_LMM_TYPE { get; set; }
        public int MAST_ARRR_YEAR { get; set; }
        public decimal MAST_ARRR_RATE { get; set; }
        public string MAST_ARRR_RATE_IS_FINAL { get; set; }
        public string ACTIVE_FLAG { get; set; }
        public Nullable<int> USERID { get; set; }
        public string IPADD { get; set; }
        public string MAST_ARRR_FILENAME { get; set; }
        public Nullable<System.DateTime> MAST_ARRR_FILE_UPLOAD_DATE { get; set; }
    
        public virtual MASTER_ARRR_LMM_TYPES MASTER_ARRR_LMM_TYPES { get; set; }
        public virtual MASTER_STATE MASTER_STATE { get; set; }
        public virtual UM_User_Master UM_User_Master { get; set; }
    }
}
