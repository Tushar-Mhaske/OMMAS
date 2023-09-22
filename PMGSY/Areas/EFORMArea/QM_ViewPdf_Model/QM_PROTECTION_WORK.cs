using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PMGSY.Areas.EFORMArea.QM_ViewPdf_Model
{
    public class QM_PROTECTION_WORK
    {
        public string IS_DPR_PROVISION { get; set; }
        public string IS_RETAINING_WALL { get; set; }
        public string IS_BREAST_WALL { get; set; }
        public string IS_PARAPET_WALL { get; set; }
        public string IS_ANY_OTHER_TYPE { get; set; }
        public Nullable<decimal> RETAINING_WALL_LENGTH { get; set; }
        public Nullable<decimal> BREAST_WALL_LENGTH { get; set; }
        public Nullable<decimal> PARAPET_WALL_LENGTH { get; set; }
        public string OTHER_TYPE1_NAME { get; set; }
        public string OTHER_TYPE2_NAME { get; set; }
        public string OTHER_TYPE3_NAME { get; set; }
        public Nullable<decimal> OTHER_A_WALL_LENGTH { get; set; }
        public Nullable<decimal> OTHER_B_WALL_LENGTH { get; set; }
        public Nullable<decimal> OTHER_C_WALL_LENGTH { get; set; }
        public Nullable<decimal> TOT_LENGTH { get; set; }
        public string IS_STONE_MEASONRY_ACCEPTABLE { get; set; }
        public string IS_BOND_STONE_PROVIDED { get; set; }
        public string ITEM_GRADING_15 { get; set; }
        public string IMPROVE_SUGGESTIONS { get; set; }
    }
}