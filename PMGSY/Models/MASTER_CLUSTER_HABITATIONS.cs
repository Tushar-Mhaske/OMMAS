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
    
    public partial class MASTER_CLUSTER_HABITATIONS
    {
        public int MAST_CLUSTER_CODE { get; set; }
        public int MAST_HAB_CODE { get; set; }
        public Nullable<int> USERID { get; set; }
        public string IPADD { get; set; }
    
        public virtual MASTER_CLUSTER MASTER_CLUSTER { get; set; }
        public virtual MASTER_HABITATIONS MASTER_HABITATIONS { get; set; }
    }
}