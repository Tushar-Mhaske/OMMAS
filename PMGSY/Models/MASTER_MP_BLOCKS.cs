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
    
    public partial class MASTER_MP_BLOCKS
    {
        public int MAST_MP_BLOCK_ID { get; set; }
        public int MAST_MP_CONST_CODE { get; set; }
        public int MAST_BLOCK_CODE { get; set; }
        public string MAST_MP_BLOCK_ACTIVE { get; set; }
        public string MAST_LOCK_STATUS { get; set; }
        public Nullable<int> USERID { get; set; }
        public string IPADD { get; set; }
    
        public virtual MASTER_BLOCK MASTER_BLOCK { get; set; }
        public virtual MASTER_MP_CONSTITUENCY MASTER_MP_CONSTITUENCY { get; set; }
        public virtual UM_User_Master UM_User_Master { get; set; }
    }
}
