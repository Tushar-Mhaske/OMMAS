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
    
    public partial class REAT_PAYMENT_CONFIGURATION_TABLE
    {
        public int ADMIN_ND_CODE { get; set; }
        public int MAST_CON_ID { get; set; }
        public int MAST_ACCOUNT_ID { get; set; }
        public string MAST_CON_COMPANY_NAME { get; set; }
        public string MAST_BANK_NAME { get; set; }
        public string MAST_IFSC_CODE { get; set; }
        public string MAST_ACCOUNT_NUMBER { get; set; }
        public string REAT_CON_ID { get; set; }
        public Nullable<bool> IS_ENABLE { get; set; }
        public Nullable<bool> IS_XML_GENERATED { get; set; }
        public string FUND_TYPE { get; set; }
    }
}
