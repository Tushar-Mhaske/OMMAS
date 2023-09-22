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
    
    public partial class MASTER_BLOCK
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public MASTER_BLOCK()
        {
            this.ADMIN_FEEDBACK = new HashSet<ADMIN_FEEDBACK>();
            this.CUPL_PMGSY3 = new HashSet<CUPL_PMGSY3>();
            this.FACILITY_HABITATION_MAPPING = new HashSet<FACILITY_HABITATION_MAPPING>();
            this.IMS_PROJECT_CITIZEN_STATS = new HashSet<IMS_PROJECT_CITIZEN_STATS>();
            this.IMS_PROJECTS_STATISTICS = new HashSet<IMS_PROJECTS_STATISTICS>();
            this.IMS_PROPOSAL_TRACKING = new HashSet<IMS_PROPOSAL_TRACKING>();
            this.MAST_CN_BLOCK_PMGSY3_FINALIZE = new HashSet<MAST_CN_BLOCK_PMGSY3_FINALIZE>();
            this.MAST_DRRP_BLOCK_PMGSY3_FINALIZE = new HashSet<MAST_DRRP_BLOCK_PMGSY3_FINALIZE>();
            this.MAST_FACILITY_BLOCK_PMGSY3_FINALIZE = new HashSet<MAST_FACILITY_BLOCK_PMGSY3_FINALIZE>();
            this.MAST_HAB_DETAILS_CSV_PMGSY3 = new HashSet<MAST_HAB_DETAILS_CSV_PMGSY3>();
            this.MAST_PCI_BLOCK_PMGSY3_FINALIZE = new HashSet<MAST_PCI_BLOCK_PMGSY3_FINALIZE>();
            this.MAST_TRACE_DRRP_SCORE_PMGSY3 = new HashSet<MAST_TRACE_DRRP_SCORE_PMGSY3>();
            this.MAST_TRACEFILE_PMGSY3 = new HashSet<MAST_TRACEFILE_PMGSY3>();
            this.MASTER_CLUSTER = new HashSet<MASTER_CLUSTER>();
            this.MASTER_EXISTING_ROADS_LOG = new HashSet<MASTER_EXISTING_ROADS_LOG>();
            this.MASTER_EXISTING_ROADS = new HashSet<MASTER_EXISTING_ROADS>();
            this.MASTER_EXISTING_ROADS_Temp = new HashSet<MASTER_EXISTING_ROADS_Temp>();
            this.MASTER_MLA_BLOCKS = new HashSet<MASTER_MLA_BLOCKS>();
            this.MASTER_MP_BLOCKS = new HashSet<MASTER_MP_BLOCKS>();
            this.MASTER_PANCHAYAT = new HashSet<MASTER_PANCHAYAT>();
            this.MASTER_VILLAGE = new HashSet<MASTER_VILLAGE>();
            this.PLAN_ROAD_LOG = new HashSet<PLAN_ROAD_LOG>();
            this.PLAN_ROAD_LOG1 = new HashSet<PLAN_ROAD_LOG>();
            this.PLAN_ROAD_LOG2 = new HashSet<PLAN_ROAD_LOG>();
            this.UM_Citizen_User_Master = new HashSet<UM_Citizen_User_Master>();
            this.EMARG_ROAD_WISE_BALANCE_WORK_PACKAGE_DETAILS = new HashSet<EMARG_ROAD_WISE_BALANCE_WORK_PACKAGE_DETAILS>();
            this.PLAN_ROAD = new HashSet<PLAN_ROAD>();
            this.PLAN_ROAD1 = new HashSet<PLAN_ROAD>();
            this.PLAN_ROAD2 = new HashSet<PLAN_ROAD>();
            this.IMS_SANCTIONED_PROJECTS = new HashSet<IMS_SANCTIONED_PROJECTS>();
            this.IMS_UNLOCK_DETAILS = new HashSet<IMS_UNLOCK_DETAILS>();
        }
    
        public int MAST_BLOCK_CODE { get; set; }
        public string MAST_BLOCK_NAME { get; set; }
        public int MAST_DISTRICT_CODE { get; set; }
        public Nullable<int> MAST_BLOCK_ID { get; set; }
        public string MAST_IS_DESERT { get; set; }
        public string MAST_IS_TRIBAL { get; set; }
        public string MAST_PMGSY_INCLUDED { get; set; }
        public string MAST_SCHEDULE5 { get; set; }
        public string MAST_IAP_BLOCK { get; set; }
        public string MAST_IS_BADB { get; set; }
        public string MAST_BLOCK_ACTIVE { get; set; }
        public int MAST_NIC_STATE_CODE { get; set; }
        public Nullable<int> MAST_NIC_DISTRICT_CODE { get; set; }
        public Nullable<int> MAST_NIC_BLOCK_CODE { get; set; }
        public string MAST_LOCK_STATUS { get; set; }
        public Nullable<int> USERID { get; set; }
        public string IPADD { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ADMIN_FEEDBACK> ADMIN_FEEDBACK { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CUPL_PMGSY3> CUPL_PMGSY3 { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<FACILITY_HABITATION_MAPPING> FACILITY_HABITATION_MAPPING { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<IMS_PROJECT_CITIZEN_STATS> IMS_PROJECT_CITIZEN_STATS { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<IMS_PROJECTS_STATISTICS> IMS_PROJECTS_STATISTICS { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<IMS_PROPOSAL_TRACKING> IMS_PROPOSAL_TRACKING { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<MAST_CN_BLOCK_PMGSY3_FINALIZE> MAST_CN_BLOCK_PMGSY3_FINALIZE { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<MAST_DRRP_BLOCK_PMGSY3_FINALIZE> MAST_DRRP_BLOCK_PMGSY3_FINALIZE { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<MAST_FACILITY_BLOCK_PMGSY3_FINALIZE> MAST_FACILITY_BLOCK_PMGSY3_FINALIZE { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<MAST_HAB_DETAILS_CSV_PMGSY3> MAST_HAB_DETAILS_CSV_PMGSY3 { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<MAST_PCI_BLOCK_PMGSY3_FINALIZE> MAST_PCI_BLOCK_PMGSY3_FINALIZE { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<MAST_TRACE_DRRP_SCORE_PMGSY3> MAST_TRACE_DRRP_SCORE_PMGSY3 { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<MAST_TRACEFILE_PMGSY3> MAST_TRACEFILE_PMGSY3 { get; set; }
        public virtual MASTER_DISTRICT MASTER_DISTRICT { get; set; }
        public virtual UM_User_Master UM_User_Master { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<MASTER_CLUSTER> MASTER_CLUSTER { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<MASTER_EXISTING_ROADS_LOG> MASTER_EXISTING_ROADS_LOG { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<MASTER_EXISTING_ROADS> MASTER_EXISTING_ROADS { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<MASTER_EXISTING_ROADS_Temp> MASTER_EXISTING_ROADS_Temp { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<MASTER_MLA_BLOCKS> MASTER_MLA_BLOCKS { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<MASTER_MP_BLOCKS> MASTER_MP_BLOCKS { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<MASTER_PANCHAYAT> MASTER_PANCHAYAT { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<MASTER_VILLAGE> MASTER_VILLAGE { get; set; }
        public virtual OMMAS_LDG_BLOCK_MAPPING OMMAS_LDG_BLOCK_MAPPING { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PLAN_ROAD_LOG> PLAN_ROAD_LOG { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PLAN_ROAD_LOG> PLAN_ROAD_LOG1 { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PLAN_ROAD_LOG> PLAN_ROAD_LOG2 { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<UM_Citizen_User_Master> UM_Citizen_User_Master { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<EMARG_ROAD_WISE_BALANCE_WORK_PACKAGE_DETAILS> EMARG_ROAD_WISE_BALANCE_WORK_PACKAGE_DETAILS { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PLAN_ROAD> PLAN_ROAD { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PLAN_ROAD> PLAN_ROAD1 { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PLAN_ROAD> PLAN_ROAD2 { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<IMS_SANCTIONED_PROJECTS> IMS_SANCTIONED_PROJECTS { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<IMS_UNLOCK_DETAILS> IMS_UNLOCK_DETAILS { get; set; }
    }
}