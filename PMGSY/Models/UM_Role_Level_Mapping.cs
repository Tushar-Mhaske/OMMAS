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
    
    public partial class UM_Role_Level_Mapping
    {
        public int ID { get; set; }
        public short RoleID { get; set; }
        public short LevelID { get; set; }
    
        public virtual UM_Level_Master UM_Level_Master { get; set; }
        public virtual UM_Role_Master UM_Role_Master { get; set; }
    }
}
