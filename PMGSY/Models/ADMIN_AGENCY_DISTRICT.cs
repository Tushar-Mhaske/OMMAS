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
    
    public partial class ADMIN_AGENCY_DISTRICT
    {
        public int ADMIN_AGENCY_DISTRICT_ID { get; set; }
        public int ADMIN_ND_CODE { get; set; }
        public int MAST_DISTRICT_CODE { get; set; }
        public Nullable<int> USERID { get; set; }
        public string IPADD { get; set; }
    
        public virtual ADMIN_DEPARTMENT ADMIN_DEPARTMENT { get; set; }
        public virtual MASTER_DISTRICT MASTER_DISTRICT { get; set; }
        public virtual UM_User_Master UM_User_Master { get; set; }
    }
}
