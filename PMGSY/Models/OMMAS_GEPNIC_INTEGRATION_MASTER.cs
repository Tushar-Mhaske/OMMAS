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
    
    public partial class OMMAS_GEPNIC_INTEGRATION_MASTER
    {
        public int MASTER_ID { get; set; }
        public long REF_NO { get; set; }
        public int MAST_STATE_CODE { get; set; }
        public int MAST_YEAR { get; set; }
        public System.DateTime DATE_OF_INSERTION { get; set; }
        public int NO_OF_RECORDS_INSERTED { get; set; }
        public System.DateTime DATE_OF_RESPONSE { get; set; }
        public string RESPONSE_MESSAGE { get; set; }
        public bool RESPONSE_TYPE { get; set; }
        public string GEPNIC_NREGA { get; set; }
    
        public virtual MASTER_STATE MASTER_STATE { get; set; }
        public virtual MASTER_YEAR MASTER_YEAR { get; set; }
    }
}
