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
    
    public partial class RCTRC_CONTACT_APPLICATION_PROFECIENCY
    {
        public int RCTRC_CONTACT_APPL_ID { get; set; }
        public int RCTRC_Contact_Id { get; set; }
        public int RCTRC_APPLICATION_ID { get; set; }
        public string RCTRC_CONTACT_APPL_PROF { get; set; }
        public System.DateTime RCTRC_CONTACT_APPL_DATE { get; set; }
        public Nullable<int> USERID { get; set; }
    
        public virtual UM_User_Master UM_User_Master { get; set; }
    }
}