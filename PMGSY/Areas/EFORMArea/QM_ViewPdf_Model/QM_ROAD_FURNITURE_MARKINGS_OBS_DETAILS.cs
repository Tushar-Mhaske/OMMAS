using System;
using System.Collections.Generic;

namespace PMGSY.Areas.EFORMArea.QM_ViewPdf_Model
{ 
    public class QM_ROAD_FURNITURE_MARKINGS_OBS_DETAILS
    {
        public short ITEM_ID { get; set; }
        public Nullable<decimal> FURNITURE_QTY_TOBE_PROVIDED { get; set; }
        public Nullable<decimal> FURNITURE_QTY_PROVIDED_AT_SIDE { get; set; }
        public string IS_PROVIDED_FURNITURE_ADEQUATE { get; set; }
        public string GRADING_FURNITURE_QUALITY { get; set; }
        
    }
}
