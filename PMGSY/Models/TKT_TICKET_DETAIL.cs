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
    
    public partial class TKT_TICKET_DETAIL
    {
        public long DETAIL_ID { get; set; }
        public int TICKET_ID { get; set; }
        public Nullable<int> FORWARDED_BY_USERID { get; set; }
        public Nullable<System.DateTime> FORWARDED_DATE_TIME { get; set; }
        public Nullable<short> FORWARDED_TO_ROLEID { get; set; }
        public Nullable<int> ACTION_BY_USERID { get; set; }
        public string ACTION_TAKEN { get; set; }
        public Nullable<System.DateTime> ACTION_DATE_TIME { get; set; }
        public int CURRENT_STATUS { get; set; }
        public Nullable<int> INSERTED_USERID { get; set; }
        public System.DateTime INSERTED_DATETIME { get; set; }
    
        public virtual UM_Role_Master UM_Role_Master { get; set; }
        public virtual UM_User_Master UM_User_Master { get; set; }
        public virtual UM_User_Master UM_User_Master1 { get; set; }
        public virtual TKT_TICKET_MASTER TKT_TICKET_MASTER { get; set; }
    }
}
