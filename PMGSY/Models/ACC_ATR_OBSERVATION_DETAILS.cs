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
    
    public partial class ACC_ATR_OBSERVATION_DETAILS
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public ACC_ATR_OBSERVATION_DETAILS()
        {
            this.ACC_ATR_OBSERVATION_DETAILS1 = new HashSet<ACC_ATR_OBSERVATION_DETAILS>();
        }
    
        public int ACC_ATR_OBSERVATIONS_ID { get; set; }
        public int ACC_ATR_MASTER_ID { get; set; }
        public Nullable<int> PARENT_OBSERVATION_ID { get; set; }
        public string OBSERVATION_SUBJECT { get; set; }
        public string OBSERVATIONS { get; set; }
        public string OBSERVATION_STATUS { get; set; }
        public int OBSERVATION_ORDER { get; set; }
        public System.DateTime OBSERVATION_DATE { get; set; }
        public string OBSERVATION_BY { get; set; }
        public bool IS_ACTIVE_OBSERVATION { get; set; }
        public int USERID { get; set; }
        public string IPADD { get; set; }
    
        public virtual ACC_ATR_MASTER ACC_ATR_MASTER { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ACC_ATR_OBSERVATION_DETAILS> ACC_ATR_OBSERVATION_DETAILS1 { get; set; }
        public virtual ACC_ATR_OBSERVATION_DETAILS ACC_ATR_OBSERVATION_DETAILS2 { get; set; }
    }
}