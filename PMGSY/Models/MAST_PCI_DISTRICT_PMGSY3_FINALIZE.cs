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
    
    public partial class MAST_PCI_DISTRICT_PMGSY3_FINALIZE
    {
        public int MAST_PCI_DISTRICT_FIN_CODE { get; set; }
        public int MAST_DISTRICT_CODE { get; set; }
        public System.DateTime MAST_PCI_DISTRICT_FINALIZE_DATE { get; set; }
        public string IS_FINALIZED { get; set; }
        public Nullable<int> USERID { get; set; }
        public string IPADD { get; set; }
    
        public virtual MASTER_DISTRICT MASTER_DISTRICT { get; set; }
        public virtual UM_User_Master UM_User_Master { get; set; }
    }
}
