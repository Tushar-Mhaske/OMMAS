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
    
    public partial class IMS_COST_COMPONENT
    {
        public int IMS_COMPONENT_ID { get; set; }
        public int IMS_PR_ROAD_CODE { get; set; }
        public decimal IMS_CLEARING { get; set; }
        public decimal IMS_EXCAVATION { get; set; }
        public decimal IMS_FILLING { get; set; }
        public decimal IMS_SUB_GRADE { get; set; }
        public decimal IMS_SHOULDER { get; set; }
        public decimal IMS_GRANULAR_SUB_BASE { get; set; }
        public decimal IMS_SOIL_AGGREGATE { get; set; }
        public decimal IMS_WBM_GRADE_II { get; set; }
        public decimal IMS_WBM_GRADE_III { get; set; }
        public decimal IMS_WMM { get; set; }
        public decimal IMS_PRIME_COAT { get; set; }
        public decimal IMS_TACK_COAT { get; set; }
        public decimal IMS_BM_DBM { get; set; }
        public decimal IMS_OGPC_SDBC_BC { get; set; }
        public decimal IMS_SEAL_COAT { get; set; }
        public decimal IMS_SURFACE_DRESSING { get; set; }
        public decimal IMS_DRY_LEAN_CONCRETE { get; set; }
        public decimal IMS_CONCRETE_PAVEMENT { get; set; }
        public Nullable<int> USERID { get; set; }
        public string IPADD { get; set; }
    
        public virtual UM_User_Master UM_User_Master { get; set; }
        public virtual IMS_SANCTIONED_PROJECTS IMS_SANCTIONED_PROJECTS { get; set; }
    }
}