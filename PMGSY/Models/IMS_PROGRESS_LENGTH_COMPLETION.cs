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
    
    public partial class IMS_PROGRESS_LENGTH_COMPLETION
    {
        public int IMS_PR_ROAD_CODE { get; set; }
        public int IMS_TRANSACTION_CODE { get; set; }
        public string IMS_CHANGE_TYPE { get; set; }
        public decimal IMS_CHANGED_LENGTH { get; set; }
        public decimal IMS_PERCENTAGE_CHANGE { get; set; }
        public System.DateTime IMS_LENGTH_CHANGE_REQUEST_DATE { get; set; }
        public string IMS_IS_MRD_APPROVED { get; set; }
        public Nullable<System.DateTime> IMS_MRD_APPROVED_DATE { get; set; }
        public Nullable<int> USERID { get; set; }
        public string IPADD { get; set; }
        public Nullable<int> APPROVE_USERID { get; set; }
        public string APPROVE_IPADD { get; set; }
    
        public virtual UM_User_Master UM_User_Master { get; set; }
        public virtual UM_User_Master UM_User_Master1 { get; set; }
        public virtual IMS_SANCTIONED_PROJECTS IMS_SANCTIONED_PROJECTS { get; set; }
    }
}
