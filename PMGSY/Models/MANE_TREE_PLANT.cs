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
    
    public partial class MANE_TREE_PLANT
    {
        public int TREE_PLANT_ID { get; set; }
        public int IMS_PR_ROAD_CODE { get; set; }
        public int TREE_PLANT_YEAR { get; set; }
        public int TREE_PLANT_MONTH { get; set; }
        public int TREE_PLANT_NEW { get; set; }
        public int TREE_PLANT_OLD { get; set; }
    
        public virtual IMS_SANCTIONED_PROJECTS IMS_SANCTIONED_PROJECTS { get; set; }
        public virtual IMS_SANCTIONED_PROJECTS IMS_SANCTIONED_PROJECTS1 { get; set; }
    }
}
