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
    
    public partial class IMS_TEST_RESULT
    {
        public int IMS_RESULT_CODE { get; set; }
        public int IMS_PR_ROAD_CODE { get; set; }
        public int IMS_TEST_CODE { get; set; }
        public decimal IMS_CHAINAGE { get; set; }
        public int IMS_SAMPLE_ID { get; set; }
        public decimal IMS_TEST_RESULT1 { get; set; }
        public Nullable<int> USERID { get; set; }
        public string IPADD { get; set; }
    
        public virtual MASTER_TEST MASTER_TEST { get; set; }
        public virtual UM_User_Master UM_User_Master { get; set; }
        public virtual IMS_SANCTIONED_PROJECTS IMS_SANCTIONED_PROJECTS { get; set; }
    }
}