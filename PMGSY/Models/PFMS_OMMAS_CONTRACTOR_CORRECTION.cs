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
    
    public partial class PFMS_OMMAS_CONTRACTOR_CORRECTION
    {
        public int POCC_ID { get; set; }
        public int MAST_CON_ID { get; set; }
        public int MAST_ACCOUNT_ID { get; set; }
        public string ACTION { get; set; }
        public System.DateTime ACTION_DATE { get; set; }
        public Nullable<int> USERID { get; set; }
        public string IPADD { get; set; }
    
        public virtual MASTER_CONTRACTOR MASTER_CONTRACTOR { get; set; }
        public virtual UM_User_Master UM_User_Master { get; set; }
    }
}