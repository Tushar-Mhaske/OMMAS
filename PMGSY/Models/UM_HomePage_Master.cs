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
    
    public partial class UM_HomePage_Master
    {
        public int Id { get; set; }
        public int RoleId { get; set; }
        public int HomePageId { get; set; }
    
        public virtual UM_Action_Master UM_Action_Master { get; set; }
    }
}
