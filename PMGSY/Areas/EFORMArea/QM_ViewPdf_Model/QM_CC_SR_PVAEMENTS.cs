using System;
using System.Collections.Generic;

namespace PMGSY.Areas.EFORMArea.QM_ViewPdf_Model
{   
    public class QM_CC_SR_PVAEMENTS
    {    
        public string ITEM_EXEC_STATUS { get; set; }
        public string CCP_TYPE { get; set; }
        public string CONCRETE_GRADE_ASPER_DPR { get; set; }
        public Nullable<decimal> CC_SR_PROPOSED_LENGTH { get; set; }
        public Nullable<decimal> CC_SR_EXECUTED_LENGTH { get; set; }
        public string IS_CC_CORE_STRENGTH_ACCEPTABLE { get; set; }
        public string IS_EXPANS_CONCTRUCT_PROVIDED { get; set; }
        public string IS_CUTS_JOINTS_ACCEPTABLE { get; set; }
        public string IS_JOINTS_FILLED { get; set; }
        public string IS_SURFACE_TEXTURE_ACCEPTABLE { get; set; }
        public string IS_EDGES_FREE { get; set; }
        public string IS_CAMBER_PROVIDED { get; set; }
        public string IS_CC_PAVEMENT_EXIST { get; set; }
        public string ITEM_GRADING_18 { get; set; }
        public string IMPROVE_SUGGESTIONS { get; set; }
   
    }
}
