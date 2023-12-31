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
    
    public partial class EFORM_BRIDGE_QM_CHILD_ON_QOM_BAILEY_SUPERSTRUCTURE
    {
        public int BAILY_QOM_ID { get; set; }
        public int BAILEY_ID { get; set; }
        public int STRUCTURE_ID { get; set; }
        public int EFORM_ID { get; set; }
        public int PR_ROAD_CODE { get; set; }
        public string RD_LOC { get; set; }
        public string TYPES_OF_BAILY_BRIDGE { get; set; }
        public string IS_ALL_COMPONENT_AVAILABLE { get; set; }
        public string IS_COMPONENT_CORROSION_FREE { get; set; }
        public string IS_LAUNCHING_ROLLER_AVAILABLE { get; set; }
        public string IS_PANEL_PINS_PLACED_PROPERLY { get; set; }
        public string IS_PAINTING_PROPER { get; set; }
        public string IS_HFL_CLEARANCE_ADEQUATE { get; set; }
        public int USER_ID { get; set; }
        public string IPADD { get; set; }
    
        public virtual EFORM_BRIDGE_QM_CHILD_BAILEY_BRIDGE_SUPERSTRUCTURE EFORM_BRIDGE_QM_CHILD_BAILEY_BRIDGE_SUPERSTRUCTURE { get; set; }
        public virtual EFORM_BRIDGE_QM_SUPERSTRUCTURE EFORM_BRIDGE_QM_SUPERSTRUCTURE { get; set; }
        public virtual EFORM_MASTER EFORM_MASTER { get; set; }
        public virtual UM_User_Master UM_User_Master { get; set; }
        public virtual IMS_SANCTIONED_PROJECTS IMS_SANCTIONED_PROJECTS { get; set; }
    }
}
