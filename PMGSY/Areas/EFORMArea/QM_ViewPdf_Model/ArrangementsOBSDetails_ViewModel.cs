using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PMGSY.Areas.EFORMArea.QM_ViewPdf_Model
{
    public class ArrangementsOBSDetails_ViewModel
    {       
        public int EFORM_ID { get; set; }       
        public int PR_ROAD_CODE { get; set; }
        public string IS_FIELD_LAB_ESTD { get; set; }
        public string IS_LAB_LOC_SAME { get; set; }
        public string IS_EQUIP_AVAILABLE { get; set; }
        public string IS_EQUIP_USED { get; set; }
        public string IS_EQUIP_AVAIL_VERIFY { get; set; }
        public string IS_ENGG_AVAILABLE { get; set; }
        public string IS_ALT_ENGG_ARR_SATISFIED { get; set; }
        public string IS_LAB_TECH_AVAILABLE { get; set; }
        public string ITEM_GRADING_2 { get; set; }
        public string IMPROVEMENT_REMARK { get; set; }
       
    }
}