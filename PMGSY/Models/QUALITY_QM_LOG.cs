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
    
    public partial class QUALITY_QM_LOG
    {
        public int LOG_ID { get; set; }
        public int ADMIN_QM_CODE { get; set; }
        public string MOBILE_NO { get; set; }
        public string IMEI_NO { get; set; }
        public string OS_VERSION { get; set; }
        public string MODEL_NAME { get; set; }
        public string NETWORK_PROVIDER { get; set; }
        public Nullable<System.DateTime> LOGIN_DATE_TIME { get; set; }
        public Nullable<System.DateTime> LOGOUT_DATE_TIME { get; set; }
        public string APP_VERSION { get; set; }
        public string LOG_MODE { get; set; }
        public string IP_ADDRESS { get; set; }
    
        public virtual ADMIN_QUALITY_MONITORS ADMIN_QUALITY_MONITORS { get; set; }
    }
}
