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
    
    public partial class EFORM_TR_SUBITEM_OPTIONS_SELECTED_DETAIL
    {
        public int SUB_SELECT_ID { get; set; }
        public int MAIN_ITEM_ID { get; set; }
        public int SUBITEM_ID { get; set; }
        public int EFORM_ID { get; set; }
        public int OPTION_ID { get; set; }
        public int USER_ID { get; set; }
        public string IPADD { get; set; }
    
        public virtual EFORM_MASTER EFORM_MASTER { get; set; }
        public virtual EFORM_TR_ITEM_OPTIONS_MASTER EFORM_TR_ITEM_OPTIONS_MASTER { get; set; }
        public virtual EFORM_TR_MAIN_ITEM_MASTER EFORM_TR_MAIN_ITEM_MASTER { get; set; }
        public virtual EFORM_TR_SUBITEM_MASTER EFORM_TR_SUBITEM_MASTER { get; set; }
        public virtual UM_User_Master UM_User_Master { get; set; }
    }
}