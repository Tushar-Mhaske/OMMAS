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
    
    public partial class OMMAS_LDG_STATE_MAPPING
    {
        public int MAST_STATE_CODE { get; set; }
        public int MAST_STATE_LDG_CODE { get; set; }
        public int MAST_STATE_LDG_CODE_REAT { get; set; }
    
        public virtual MASTER_STATE MASTER_STATE { get; set; }
    }
}
