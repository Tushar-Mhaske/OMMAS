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
    
    public partial class MAST_TECH_LAYER_MAPPING
    {
        public int MAST_TECH_LAYER_ID { get; set; }
        public int MAST_TECH_CODE { get; set; }
        public int MAST_HEAD_CODE { get; set; }
    
        public virtual MASTER_EXECUTION_ITEM MASTER_EXECUTION_ITEM { get; set; }
        public virtual MASTER_TECHNOLOGY MASTER_TECHNOLOGY { get; set; }
    }
}