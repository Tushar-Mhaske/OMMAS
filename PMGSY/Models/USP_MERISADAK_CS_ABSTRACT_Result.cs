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
    
    public partial class USP_MERISADAK_CS_ABSTRACT_Result
    {
        public int MAST_DISTRICT_CODE { get; set; }
        public string MAST_DISTRICT_NAME { get; set; }
        public int MAST_BLOCK_CODE { get; set; }
        public string MAST_BLOCK_NAME { get; set; }
        public Nullable<int> NEW_ROADS { get; set; }
        public Nullable<int> UPGRADE_ROADS { get; set; }
        public Nullable<int> NEW_ROADS_COMPLETED { get; set; }
        public Nullable<int> UPGRADE_ROADS_COMPLETED { get; set; }
        public Nullable<int> NEW_ROADS_INPROGRESS { get; set; }
        public Nullable<int> UPGRADE_ROADS_INPROGRESS { get; set; }
    }
}