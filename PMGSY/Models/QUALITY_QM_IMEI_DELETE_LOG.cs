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
    
    public partial class QUALITY_QM_IMEI_DELETE_LOG
    {
        public int UserID { get; set; }
        public int IMEI_ID { get; set; }
        public string IMEI_NO { get; set; }
    
        public virtual UM_User_Master UM_User_Master { get; set; }
    }
}
