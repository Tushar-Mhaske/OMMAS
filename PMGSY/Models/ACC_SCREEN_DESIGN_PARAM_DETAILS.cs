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
    
    public partial class ACC_SCREEN_DESIGN_PARAM_DETAILS
    {
        public short TXN_ID { get; set; }
        public string CON_REQ { get; set; }
        public string AGREEMENT_REQ { get; set; }
        public string ROAD_REQ { get; set; }
        public string PIU_REQ { get; set; }
        public string SUPPLIER_REQ { get; set; }
        public string YEAR_REQ { get; set; }
        public string PKG_REQ { get; set; }
    
        public virtual ACC_MASTER_TXN ACC_MASTER_TXN { get; set; }
    }
}
