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
    
    public partial class ADMIN_DEPARTMENT
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public ADMIN_DEPARTMENT()
        {
            this.ACC_AUTH_REQUEST_MASTER = new HashSet<ACC_AUTH_REQUEST_MASTER>();
            this.ACC_BILL_DETAILS = new HashSet<ACC_BILL_DETAILS>();
            this.ACC_BILL_MASTER = new HashSet<ACC_BILL_MASTER>();
            this.ACC_CANCELLED_CHEQUES = new HashSet<ACC_CANCELLED_CHEQUES>();
            this.ACC_EAUTH_MASTER = new HashSet<ACC_EAUTH_MASTER>();
            this.ACC_EPAY_MAIL_DETAILS = new HashSet<ACC_EPAY_MAIL_DETAILS>();
            this.ACC_EPAY_MAIL_OTHER = new HashSet<ACC_EPAY_MAIL_OTHER>();
            this.ACC_NOTIFICATION_DETAILS = new HashSet<ACC_NOTIFICATION_DETAILS>();
            this.ACC_NOTIFICATION_DETAILS1 = new HashSet<ACC_NOTIFICATION_DETAILS>();
            this.ACC_RPT_FINAL_BILL_PAYMENT_PENDING = new HashSet<ACC_RPT_FINAL_BILL_PAYMENT_PENDING>();
            this.ACC_RPT_MONTHWISE_SUMMARY = new HashSet<ACC_RPT_MONTHWISE_SUMMARY>();
            this.ACC_RPT_MONTHWISE_SUMMARY_REVOKE_FINALIZE = new HashSet<ACC_RPT_MONTHWISE_SUMMARY_REVOKE_FINALIZE>();
            this.ACC_TXN_BANK = new HashSet<ACC_TXN_BANK>();
            this.ACC_VOUCHER_NUMBER_MASTER = new HashSet<ACC_VOUCHER_NUMBER_MASTER>();
            this.ADMIN_AGENCY_DISTRICT = new HashSet<ADMIN_AGENCY_DISTRICT>();
            this.ADMIN_NEWS = new HashSet<ADMIN_NEWS>();
            this.PAYMENT_EXEC_VALIDATION_CONFIGURATION = new HashSet<PAYMENT_EXEC_VALIDATION_CONFIGURATION>();
            this.REAT_CONTRACTOR_DETAILS = new HashSet<REAT_CONTRACTOR_DETAILS>();
            this.REAT_DATA_SEND_DETAILS = new HashSet<REAT_DATA_SEND_DETAILS>();
            this.ADMIN_NODAL_OFFICERS = new HashSet<ADMIN_NODAL_OFFICERS>();
            this.EMARG_AUTHORIZATION_DETAILS = new HashSet<EMARG_AUTHORIZATION_DETAILS>();
            this.MASTER_TEND_EMPLOYMENT = new HashSet<MASTER_TEND_EMPLOYMENT>();
            this.MASTER_TEND_EQUIPMENT = new HashSet<MASTER_TEND_EQUIPMENT>();
            this.MRD_CLEARANCE_LETTERS = new HashSet<MRD_CLEARANCE_LETTERS>();
            this.MRD_DROPPED_LETTERS = new HashSet<MRD_DROPPED_LETTERS>();
            this.MRD_FUND_ALLOCATION = new HashSet<MRD_FUND_ALLOCATION>();
            this.MRD_FUND_RELEASE = new HashSet<MRD_FUND_RELEASE>();
            this.OFP_REQUEST_MASTER = new HashSet<OFP_REQUEST_MASTER>();
            this.PFMS_DATA_SEND_DETAILS = new HashSet<PFMS_DATA_SEND_DETAILS>();
            this.PFMS_DATA_SEND_DETAILS_TEMP = new HashSet<PFMS_DATA_SEND_DETAILS_TEMP>();
            this.PFMS_INITIATING_PARTY_MASTER = new HashSet<PFMS_INITIATING_PARTY_MASTER>();
            this.PFMS_OMMAS_DSC_MAPPING = new HashSet<PFMS_OMMAS_DSC_MAPPING>();
            this.PFMS_OMMS_BANK_MAPPING = new HashSet<PFMS_OMMS_BANK_MAPPING>();
            this.QUALITY_QM_MP_VISIT = new HashSet<QUALITY_QM_MP_VISIT>();
            this.RCTRC_UM_User_Master = new HashSet<RCTRC_UM_User_Master>();
            this.REAT_FUND_RECEIPT_CONFIGURATION = new HashSet<REAT_FUND_RECEIPT_CONFIGURATION>();
            this.REAT_INITIATING_PARTY_MASTER = new HashSet<REAT_INITIATING_PARTY_MASTER>();
            this.REAT_OB_DETAILS = new HashSet<REAT_OB_DETAILS>();
            this.REAT_OMMAS_DSC_MAPPING = new HashSet<REAT_OMMAS_DSC_MAPPING>();
            this.REAT_RECEIPT_DETAILS = new HashSet<REAT_RECEIPT_DETAILS>();
            this.TEND_CHECKLIST_DETAIL = new HashSet<TEND_CHECKLIST_DETAIL>();
            this.TEND_NIT_MASTER = new HashSet<TEND_NIT_MASTER>();
            this.TEND_TENDER_PRICE = new HashSet<TEND_TENDER_PRICE>();
            this.UM_User_Master1 = new HashSet<UM_User_Master>();
            this.EFORM_PIU_GENERAL_INFO = new HashSet<EFORM_PIU_GENERAL_INFO>();
            this.EFORM_PIU_MIX_DESIGN_DETAILS = new HashSet<EFORM_PIU_MIX_DESIGN_DETAILS>();
            this.EFORM_PIU_PRGS_DETAILS = new HashSet<EFORM_PIU_PRGS_DETAILS>();
            this.EFORM_PIU_QC_DETAILS = new HashSet<EFORM_PIU_QC_DETAILS>();
            this.EMARG_ROAD_WISE_BALANCE_WORK_PACKAGE_DETAILS = new HashSet<EMARG_ROAD_WISE_BALANCE_WORK_PACKAGE_DETAILS>();
            this.EFORM_BRIDGE_PIU_GENERAL_INFO = new HashSet<EFORM_BRIDGE_PIU_GENERAL_INFO>();
            this.EFORM_BRIDGE_PIU_GENERAL_INFO1 = new HashSet<EFORM_BRIDGE_PIU_GENERAL_INFO>();
            this.EFORM_BRIDGE_PIU_MIX_DESIGN_DETAILS = new HashSet<EFORM_BRIDGE_PIU_MIX_DESIGN_DETAILS>();
            this.EFORM_BRIDGE_PIU_PRGS_DETAILS = new HashSet<EFORM_BRIDGE_PIU_PRGS_DETAILS>();
            this.EFORM_BRIDGE_PIU_QC_DETAILS = new HashSet<EFORM_BRIDGE_PIU_QC_DETAILS>();
            this.EFORM_BRIDGE_PIU_PARTICULARS = new HashSet<EFORM_BRIDGE_PIU_PARTICULARS>();
            this.ACC_CHQ_BOOK_DETAILS = new HashSet<ACC_CHQ_BOOK_DETAILS>();
            this.ACC_BANK_DETAILS = new HashSet<ACC_BANK_DETAILS>();
            this.IMS_SANCTIONED_PROJECTS = new HashSet<IMS_SANCTIONED_PROJECTS>();
        }
    
        public int ADMIN_ND_CODE { get; set; }
        public int MAST_STATE_CODE { get; set; }
        public int MAST_AGENCY_CODE { get; set; }
        public Nullable<int> MAST_PARENT_ND_CODE { get; set; }
        public string MAST_ND_TYPE { get; set; }
        public string ADMIN_ND_NAME { get; set; }
        public string ADMIN_ND_ADDRESS1 { get; set; }
        public string ADMIN_ND_ADDRESS2 { get; set; }
        public Nullable<int> MAST_DISTRICT_CODE { get; set; }
        public string ADMIN_ND_PIN { get; set; }
        public string ADMIN_ND_STD1 { get; set; }
        public string ADMIN_ND_STD2 { get; set; }
        public string ADMIN_ND_PHONE1 { get; set; }
        public string ADMIN_ND_PHONE2 { get; set; }
        public string ADMIN_ND_STD_FAX { get; set; }
        public string ADMIN_ND_FAX { get; set; }
        public string ADMIN_ND_MOBILE_NO { get; set; }
        public string ADMIN_ND_EMAIL { get; set; }
        public string ADMIN_ND_REMARKS { get; set; }
        public Nullable<System.DateTime> ADMIN_SBD_DATE { get; set; }
        public string ADMIN_PDF_KEY { get; set; }
        public string ADMIN_EMAIL_CC { get; set; }
        public string ADMIN_ND_TAN_NO { get; set; }
        public string ADMIN_BANK_AUTH_ENABLED { get; set; }
        public Nullable<System.DateTime> ADMIN_BA_ENABLE_DATE { get; set; }
        public string ADMIN_EPAY_MAIL { get; set; }
        public Nullable<System.DateTime> ADMIN_EPAY_ENABLE_DATE { get; set; }
        public string ADMIN_EREMITTANCE_ENABLED { get; set; }
        public Nullable<System.DateTime> ADMIN_EREMIT_ENABLED_DATE { get; set; }
        public Nullable<int> ADMIN_EPAY_DPIU_CODE { get; set; }
        public string ADMIN_ND_ACTIVE { get; set; }
        public Nullable<System.DateTime> ADMIN_ND_CLOSE_DATE { get; set; }
        public Nullable<int> USERID { get; set; }
        public string IPADD { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ACC_AUTH_REQUEST_MASTER> ACC_AUTH_REQUEST_MASTER { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ACC_BILL_DETAILS> ACC_BILL_DETAILS { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ACC_BILL_MASTER> ACC_BILL_MASTER { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ACC_CANCELLED_CHEQUES> ACC_CANCELLED_CHEQUES { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ACC_EAUTH_MASTER> ACC_EAUTH_MASTER { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ACC_EPAY_MAIL_DETAILS> ACC_EPAY_MAIL_DETAILS { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ACC_EPAY_MAIL_OTHER> ACC_EPAY_MAIL_OTHER { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ACC_NOTIFICATION_DETAILS> ACC_NOTIFICATION_DETAILS { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ACC_NOTIFICATION_DETAILS> ACC_NOTIFICATION_DETAILS1 { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ACC_RPT_FINAL_BILL_PAYMENT_PENDING> ACC_RPT_FINAL_BILL_PAYMENT_PENDING { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ACC_RPT_MONTHWISE_SUMMARY> ACC_RPT_MONTHWISE_SUMMARY { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ACC_RPT_MONTHWISE_SUMMARY_REVOKE_FINALIZE> ACC_RPT_MONTHWISE_SUMMARY_REVOKE_FINALIZE { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ACC_TXN_BANK> ACC_TXN_BANK { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ACC_VOUCHER_NUMBER_MASTER> ACC_VOUCHER_NUMBER_MASTER { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ADMIN_AGENCY_DISTRICT> ADMIN_AGENCY_DISTRICT { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ADMIN_NEWS> ADMIN_NEWS { get; set; }
        public virtual MASTER_AGENCY MASTER_AGENCY { get; set; }
        public virtual MASTER_STATE MASTER_STATE { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PAYMENT_EXEC_VALIDATION_CONFIGURATION> PAYMENT_EXEC_VALIDATION_CONFIGURATION { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<REAT_CONTRACTOR_DETAILS> REAT_CONTRACTOR_DETAILS { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<REAT_DATA_SEND_DETAILS> REAT_DATA_SEND_DETAILS { get; set; }
        public virtual UM_User_Master UM_User_Master { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ADMIN_NODAL_OFFICERS> ADMIN_NODAL_OFFICERS { get; set; }
        public virtual ADMIN_SQC ADMIN_SQC { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<EMARG_AUTHORIZATION_DETAILS> EMARG_AUTHORIZATION_DETAILS { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<MASTER_TEND_EMPLOYMENT> MASTER_TEND_EMPLOYMENT { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<MASTER_TEND_EQUIPMENT> MASTER_TEND_EQUIPMENT { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<MRD_CLEARANCE_LETTERS> MRD_CLEARANCE_LETTERS { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<MRD_DROPPED_LETTERS> MRD_DROPPED_LETTERS { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<MRD_FUND_ALLOCATION> MRD_FUND_ALLOCATION { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<MRD_FUND_RELEASE> MRD_FUND_RELEASE { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<OFP_REQUEST_MASTER> OFP_REQUEST_MASTER { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PFMS_DATA_SEND_DETAILS> PFMS_DATA_SEND_DETAILS { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PFMS_DATA_SEND_DETAILS_TEMP> PFMS_DATA_SEND_DETAILS_TEMP { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PFMS_INITIATING_PARTY_MASTER> PFMS_INITIATING_PARTY_MASTER { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PFMS_OMMAS_DSC_MAPPING> PFMS_OMMAS_DSC_MAPPING { get; set; }
        public virtual PFMS_OMMAS_PAYMENT_SUCCESS PFMS_OMMAS_PAYMENT_SUCCESS { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PFMS_OMMS_BANK_MAPPING> PFMS_OMMS_BANK_MAPPING { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<QUALITY_QM_MP_VISIT> QUALITY_QM_MP_VISIT { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<RCTRC_UM_User_Master> RCTRC_UM_User_Master { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<REAT_FUND_RECEIPT_CONFIGURATION> REAT_FUND_RECEIPT_CONFIGURATION { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<REAT_INITIATING_PARTY_MASTER> REAT_INITIATING_PARTY_MASTER { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<REAT_OB_DETAILS> REAT_OB_DETAILS { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<REAT_OMMAS_DSC_MAPPING> REAT_OMMAS_DSC_MAPPING { get; set; }
        public virtual REAT_OMMAS_PAYMENT_SUCCESS REAT_OMMAS_PAYMENT_SUCCESS { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<REAT_RECEIPT_DETAILS> REAT_RECEIPT_DETAILS { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TEND_CHECKLIST_DETAIL> TEND_CHECKLIST_DETAIL { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TEND_NIT_MASTER> TEND_NIT_MASTER { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TEND_TENDER_PRICE> TEND_TENDER_PRICE { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<UM_User_Master> UM_User_Master1 { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<EFORM_PIU_GENERAL_INFO> EFORM_PIU_GENERAL_INFO { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<EFORM_PIU_MIX_DESIGN_DETAILS> EFORM_PIU_MIX_DESIGN_DETAILS { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<EFORM_PIU_PRGS_DETAILS> EFORM_PIU_PRGS_DETAILS { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<EFORM_PIU_QC_DETAILS> EFORM_PIU_QC_DETAILS { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<EMARG_ROAD_WISE_BALANCE_WORK_PACKAGE_DETAILS> EMARG_ROAD_WISE_BALANCE_WORK_PACKAGE_DETAILS { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<EFORM_BRIDGE_PIU_GENERAL_INFO> EFORM_BRIDGE_PIU_GENERAL_INFO { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<EFORM_BRIDGE_PIU_GENERAL_INFO> EFORM_BRIDGE_PIU_GENERAL_INFO1 { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<EFORM_BRIDGE_PIU_MIX_DESIGN_DETAILS> EFORM_BRIDGE_PIU_MIX_DESIGN_DETAILS { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<EFORM_BRIDGE_PIU_PRGS_DETAILS> EFORM_BRIDGE_PIU_PRGS_DETAILS { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<EFORM_BRIDGE_PIU_QC_DETAILS> EFORM_BRIDGE_PIU_QC_DETAILS { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<EFORM_BRIDGE_PIU_PARTICULARS> EFORM_BRIDGE_PIU_PARTICULARS { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ACC_CHQ_BOOK_DETAILS> ACC_CHQ_BOOK_DETAILS { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ACC_BANK_DETAILS> ACC_BANK_DETAILS { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<IMS_SANCTIONED_PROJECTS> IMS_SANCTIONED_PROJECTS { get; set; }
    }
}