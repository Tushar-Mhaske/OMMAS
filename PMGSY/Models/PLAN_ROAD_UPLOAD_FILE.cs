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
    
    public partial class PLAN_ROAD_UPLOAD_FILE
    {
        public int PLAN_CN_ROAD_CODE { get; set; }
        public int PLAN_FILE_ID { get; set; }
        public string PLAN_FILE_NAME { get; set; }
        public Nullable<int> PLAN_FILE_SIZE { get; set; }
        public System.DateTime PLAN_UPLOAD_DATE { get; set; }
        public decimal PLAN_START_CHAINAGE { get; set; }
        public decimal PLAN_END_CHAINAGE { get; set; }
    
        public virtual PLAN_ROAD PLAN_ROAD { get; set; }
    }
}
