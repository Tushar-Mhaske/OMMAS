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
    
    public partial class MASTER_TEND_EMPLOYMENT
    {
        public int MAST_STATE_CODE { get; set; }
        public int ADMIN_ND_CODE { get; set; }
        public int TEND_EMPLOYMENT_ID { get; set; }
        public int MAST_CHECKLIST_POINTID { get; set; }
        public int TEND_QUALIFICATION_ID { get; set; }
        public int TEND_EMPLOYMENT_NUMBER { get; set; }
        public Nullable<int> USERID { get; set; }
        public string IPADD { get; set; }
    
        public virtual ADMIN_DEPARTMENT ADMIN_DEPARTMENT { get; set; }
        public virtual MASTER_CHECKLIST_POINTS MASTER_CHECKLIST_POINTS { get; set; }
        public virtual MASTER_QUALIFICATION MASTER_QUALIFICATION { get; set; }
        public virtual MASTER_STATE MASTER_STATE { get; set; }
        public virtual UM_User_Master UM_User_Master { get; set; }
    }
}
