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
    
    public partial class USP_QM_HELPDESK_GET_LOG_DETAILS_Result
    {
        public int ADMIN_QM_CODE { get; set; }
        public int LOG_ID { get; set; }
        public string MOBILE_NO { get; set; }
        public string IMEI_NO { get; set; }
        public string OS_VERSION { get; set; }
        public string MODEL_NAME { get; set; }
        public string NETWORK_PROVIDER { get; set; }
        public System.DateTime LOGIN_DATE_TIME { get; set; }
        public System.DateTime LOGOUT_DATE_TIME { get; set; }
        public string APP_VERSION { get; set; }
        public string LOG_MODE { get; set; }
    }
}