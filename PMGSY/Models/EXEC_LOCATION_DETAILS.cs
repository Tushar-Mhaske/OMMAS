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
    
    public partial class EXEC_LOCATION_DETAILS
    {
        public int EXEC_LOC_ID { get; set; }
        public int IMS_PR_ROAD_CODE { get; set; }
        public string EXEC_LOC_FLAG { get; set; }
        public string EXEC_LOC_LONG { get; set; }
        public string EXEC_LOC_LAT { get; set; }
        public System.DateTime EXEC_UPLOAD_DATETIME { get; set; }
        public int USERID { get; set; }
    
        public virtual UM_User_Master UM_User_Master { get; set; }
        public virtual IMS_SANCTIONED_PROJECTS IMS_SANCTIONED_PROJECTS { get; set; }
    }
}