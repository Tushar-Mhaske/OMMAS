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
    
    public partial class ACC_RPT_REPORT_PROPERTY
    {
        public short RPT_ID { get; set; }
        public int PROP_ID { get; set; }
        public string PROP_DESC { get; set; }
        public string PROP_VALUE { get; set; }
    
        public virtual ACC_RPT_REPORT_MASTER ACC_RPT_REPORT_MASTER { get; set; }
    }
}
