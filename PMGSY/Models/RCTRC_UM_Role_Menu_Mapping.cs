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
    
    public partial class RCTRC_UM_Role_Menu_Mapping
    {
        public int MenuID { get; set; }
        public short RoleID { get; set; }
        public Nullable<bool> isAdd { get; set; }
        public Nullable<bool> isEdit { get; set; }
        public Nullable<bool> isDelete { get; set; }
    
        public virtual RCTRC_UM_Menu_Master RCTRC_UM_Menu_Master { get; set; }
        public virtual RCTRC_UM_Role_Master RCTRC_UM_Role_Master { get; set; }
    }
}
