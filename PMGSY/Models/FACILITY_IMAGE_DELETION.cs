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
    
    public partial class FACILITY_IMAGE_DELETION
    {
        public int ID { get; set; }
        public int MAST_FACILITY_ID { get; set; }
        public string FILE_NAME { get; set; }
        public string FILE_DELETION_REMARKS { get; set; }
        public System.DateTime FILE_DELETION_DATE { get; set; }
        public Nullable<int> USERID { get; set; }
        public string IPADD { get; set; }
    
        public virtual MASTER_FACILITY MASTER_FACILITY { get; set; }
    }
}
