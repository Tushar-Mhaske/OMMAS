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
    
    public partial class IMS_EC_TRAININGS
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public IMS_EC_TRAININGS()
        {
            this.IMS_EC_TRAINING_PERSONS = new HashSet<IMS_EC_TRAINING_PERSONS>();
        }
    
        public int MAST_TRAINING_ID { get; set; }
        public int MAST_STATE_CODE { get; set; }
        public int IMS_YEAR { get; set; }
        public int MAST_DESIG_CODE { get; set; }
        public int IMS_TOTAL_PERSONS { get; set; }
        public Nullable<int> USERID { get; set; }
        public string IPADD { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<IMS_EC_TRAINING_PERSONS> IMS_EC_TRAINING_PERSONS { get; set; }
        public virtual MASTER_DESIGNATION MASTER_DESIGNATION { get; set; }
        public virtual MASTER_STATE MASTER_STATE { get; set; }
        public virtual UM_User_Master UM_User_Master { get; set; }
    }
}