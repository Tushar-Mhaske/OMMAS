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
    
    public partial class MASTER_ARRR_ITEMS_MASTER
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public MASTER_ARRR_ITEMS_MASTER()
        {
            this.MASTER_ARRR_ITEM_TAXES = new HashSet<MASTER_ARRR_ITEM_TAXES>();
            this.MASTER_ARRR_ITEMS = new HashSet<MASTER_ARRR_ITEMS>();
        }
    
        public int MAST_ITEM_CODE { get; set; }
        public int MAST_HEAD_CODE { get; set; }
        public int MAST_ITEM { get; set; }
        public Nullable<int> MAST_MAJOR_SUBITEM_CODE { get; set; }
        public Nullable<int> MAST_MINOR_SUBITEM_CODE { get; set; }
        public string MAST_ITEM_NAME { get; set; }
        public string MAST_ITEM_DESC { get; set; }
        public int MAST_ITEM_PARENT { get; set; }
        public int MAST_ITEM_UNIT { get; set; }
        public string MAST_ITEM_ACTIVE { get; set; }
        public string MAST_MORD_REF { get; set; }
        public string MAST_ITEM_USER_CODE { get; set; }
        public Nullable<int> USERID { get; set; }
        public string IPADD { get; set; }
    
        public virtual MASTER_ARRR_ITEM_HEAD MASTER_ARRR_ITEM_HEAD { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<MASTER_ARRR_ITEM_TAXES> MASTER_ARRR_ITEM_TAXES { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<MASTER_ARRR_ITEMS> MASTER_ARRR_ITEMS { get; set; }
        public virtual UM_User_Master UM_User_Master { get; set; }
    }
}
