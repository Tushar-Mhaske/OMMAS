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
    
    public partial class ACC_MAP_STATES_FA
    {
        public int FA_CODE { get; set; }
        public int MAST_STATE_CODE { get; set; }
        public int MAST_AGENCY_CODE { get; set; }
    
        public virtual MASTER_AGENCY MASTER_AGENCY { get; set; }
        public virtual MASTER_FUNDING_AGENCY MASTER_FUNDING_AGENCY { get; set; }
        public virtual MASTER_STATE MASTER_STATE { get; set; }
    }
}