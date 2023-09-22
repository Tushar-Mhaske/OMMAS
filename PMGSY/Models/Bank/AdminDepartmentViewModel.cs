using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PMGSY.Models.Bank
{
    public class AdminDepartmentViewModel
    {
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
        //public Nullable<System.DateTime> ADMIN_BA_ENABLE_DATE { get; set; }
        public string ADMIN_BA_ENABLE_DATE { get; set; }
        public string ADMIN_EPAY_MAIL { get; set; }
        //public Nullable<System.DateTime> ADMIN_EPAY_ENABLE_DATE { get; set; }
        public string ADMIN_EPAY_ENABLE_DATE { get; set; }
        public string ADMIN_EREMITTANCE_ENABLED { get; set; }
        //public Nullable<System.DateTime> ADMIN_EREMIT_ENABLED_DATE { get; set; }
        public string ADMIN_EREMIT_ENABLED_DATE { get; set; }
        public Nullable<int> ADMIN_EPAY_DPIU_CODE { get; set; }
    }
}