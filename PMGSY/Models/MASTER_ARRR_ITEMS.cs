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
    
    public partial class MASTER_ARRR_ITEMS
    {
        public int MAST_ARRR_CODE { get; set; }
        public int MAST_ITEM_CODE { get; set; }
        public int MAST_LMM_TYPE_CODE { get; set; }
        public decimal MAST_ARRR_QTY { get; set; }
        public string MAST_ARRR_FINALIZED { get; set; }
        public string MAST_ARRR_MORD_APPROVED { get; set; }
        public Nullable<int> MAST_ARRR_ITEM_DEACTIVE_FROM { get; set; }
        public Nullable<System.DateTime> MAST_ARRR_ITEM_DEACTIVE_DATE { get; set; }
        public Nullable<int> USERID { get; set; }
        public string IPADD { get; set; }
    
        public virtual MASTER_ARRR_ITEMS_MASTER MASTER_ARRR_ITEMS_MASTER { get; set; }
        public virtual MASTER_ARRR_LMM_TYPES MASTER_ARRR_LMM_TYPES { get; set; }
        public virtual UM_User_Master UM_User_Master { get; set; }
    }
}
