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
    
    public partial class IMS_EC_PDF_GENERATED
    {
        public int MAST_EC_ID { get; set; }
        public string IMS_FILE_NAME { get; set; }
        public string IMS_FILE_PATH { get; set; }
        public System.DateTime IMS_PDF_GENERATION_DATE { get; set; }
        public Nullable<int> USERID { get; set; }
        public string IPADD { get; set; }
    
        public virtual IMS_EC_CHECKLIST IMS_EC_CHECKLIST { get; set; }
        public virtual UM_User_Master UM_User_Master { get; set; }
    }
}
