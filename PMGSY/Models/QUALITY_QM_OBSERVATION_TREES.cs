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
    
    public partial class QUALITY_QM_OBSERVATION_TREES
    {
        public int QM_OBSERVATION_ID { get; set; }
        public string QM_TREES_VERIFIED { get; set; }
        public string QM_REMARKS { get; set; }
        public Nullable<int> USERID { get; set; }
        public string IPADD { get; set; }
    
        public virtual QUALITY_QM_OBSERVATION_MASTER QUALITY_QM_OBSERVATION_MASTER { get; set; }
        public virtual UM_User_Master UM_User_Master { get; set; }
    }
}
