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
    
    public partial class MASTER_CONTRACTOR_LEGAL_HEIR
    {
        public int MAST_HEIR_ID { get; set; }
        public int MAST_CON_ID { get; set; }
        public string MAST_HEIR_FNAME { get; set; }
        public string MAST_HEIR_MNAME { get; set; }
        public string MAST_HEIR_LNAME { get; set; }
        public string MAST_HEIR_COMPANY_NAME { get; set; }
        public string MAST_HEIR_ADDR { get; set; }
        public string MAST_HEIR_PIN { get; set; }
        public string MAST_HEIR_MOBILE { get; set; }
        public string MAST_HEIR_EMAIL { get; set; }
        public string MAST_HEIR_PAN { get; set; }
        public string MAST_HEIR_AADHAR { get; set; }
        public int MAST_ACCOUNT_ID { get; set; }
        public int MAST_LGD_STATE_CODE { get; set; }
        public int MAST_LGD_DISTRICT_CODE { get; set; }
        public string MAST_ACCOUNT_NUMBER { get; set; }
        public string MAST_BANK_NAME { get; set; }
        public string MAST_IFSC_CODE { get; set; }
        public string MAST_CON_STATUS { get; set; }
        public Nullable<int> MAST_FC_USERID { get; set; }
        public string MAST_LOCK_STATUS { get; set; }
        public Nullable<int> USERID { get; set; }
        public string IPADD { get; set; }
        public string MAST_CON_STD1 { get; set; }
        public string MAST_CON_STD2 { get; set; }
        public string MAST_HEIR_PHONE1 { get; set; }
        public string MAST_HEIR_PHONE2 { get; set; }
        public string MAST_CON_DEATH_CERTIFICATE { get; set; }
        public string CONTRACTOR_NAME_PAN { get; set; }
    
        public virtual MASTER_CONTRACTOR MASTER_CONTRACTOR { get; set; }
        public virtual UM_User_Master UM_User_Master { get; set; }
        public virtual UM_User_Master UM_User_Master1 { get; set; }
    }
}