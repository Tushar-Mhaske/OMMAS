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
    
    public partial class TEND_NIT_DOCUMENT
    {
        public int TEND_NIT_ID { get; set; }
        public int TEND_DOC_UNIQ_ID { get; set; }
        public string TEND_DOC_DISP_NAME { get; set; }
        public string TEND_DOC_TYPE { get; set; }
        public Nullable<int> USERID { get; set; }
        public string IPADD { get; set; }
    
        public virtual TEND_NIT_DETAILS TEND_NIT_DETAILS { get; set; }
        public virtual UM_User_Master UM_User_Master { get; set; }
    }
}
