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
    
    public partial class ADMIN_ALERTS
    {
        public int ALERT_ID { get; set; }
        public string ALERT_HEADING { get; set; }
        public string ALERT_TEXT { get; set; }
        public System.DateTime INSERTION_DATE { get; set; }
        public System.DateTime DISPLAY_START_DATE { get; set; }
        public Nullable<System.DateTime> DISPLAY_END_DATE { get; set; }
        public string ALERT_STATUS { get; set; }
        public string ALERT_HEADING_HI { get; set; }
        public string ALERT_TEXT_HI { get; set; }
    }
}
