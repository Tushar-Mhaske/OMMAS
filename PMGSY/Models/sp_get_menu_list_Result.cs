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
    
    public partial class sp_get_menu_list_Result
    {
        public int MenuID { get; set; }
        public string MenuName { get; set; }
        public int ParentID { get; set; }
        public string ParentName { get; set; }
        public short Sequence { get; set; }
        public short VerticalLevel { get; set; }
        public string IsActive { get; set; }
        public string MenucombinationCode { get; set; }
    }
}