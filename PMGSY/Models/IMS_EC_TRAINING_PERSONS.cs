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
    
    public partial class IMS_EC_TRAINING_PERSONS
    {
        public int MAST_TRAINING_ID { get; set; }
        public int MAST_PERSON_ID { get; set; }
        public int MAST_DESIG_CODE { get; set; }
        public string IMS_PERSON_NAME { get; set; }
        public Nullable<int> USERID { get; set; }
        public string IPADD { get; set; }
    
        public virtual IMS_EC_TRAININGS IMS_EC_TRAININGS { get; set; }
        public virtual MASTER_DESIGNATION MASTER_DESIGNATION { get; set; }
        public virtual UM_User_Master UM_User_Master { get; set; }
    }
}
