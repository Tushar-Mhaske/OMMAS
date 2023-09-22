using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PMGSY.Areas.EFORMArea.QM_ViewPdf_Model
{
    public class BITUMINOUS_BASECOURSE_ViewModel
    {
        public int LAYER_ID { get; set; }
        public int EFORM_ID { get; set; }
        public int IMS_PR_ROAD_CODE { get; set; }
        public string PROVISION_IN_DPR { get; set; }
        public string ITEM_EXECUTION_STATUS { get; set; }
        public string ACTUAL_EXECUTION { get; set; }
        public string IS_NEW_TECH_USED { get; set; }
        public string NEW_TECH_NAME { get; set; }
        public string NEW_TECH_PROVIDER { get; set; }
        public string NEW_TECH_MATERIAL_NAME { get; set; }
        public string NEW_TECH_QTY_MATERIAL_ASPER_DPR { get; set; }
        public string NEW_TECH_QTY_MATERIAL_USED { get; set; }
        public Nullable<decimal> NEW_TECH_LOC_FROM_ROAD { get; set; }
        public Nullable<decimal> NEW_TECH_LOC_FROM_TO { get; set; }
        public string GRADING_NEW_TECH_EVALUATION { get; set; }
        public decimal THICKNESS_ASPER_DPR { get; set; }
        public string TYPE_GRADE_BINDER { get; set; }
        public string BRAND_NAME_BITUMEN { get; set; }
        public string IS_BITUMEN_USED { get; set; }
        public string INVOICE_INSUFFICIENT_REASON { get; set; }
        public decimal PERCENT_BITUMEN_CONTENT { get; set; }
        public string IS_PRIME_COAT_USED { get; set; }
        public decimal LAYING_TEMP { get; set; }
        public string IS_MIX_DESIGN { get; set; }
        public Nullable<decimal> MARSHAL_STAB { get; set; }
        public Nullable<decimal> DESIGN_DENSITY { get; set; }
        public string IS_HOT_MIX_DONE { get; set; }
        public string INSP_DATE { get; set; }
        public string ITEM_GRADING_10 { get; set; }
        public string IMPROVE_SUGGESTION { get; set; }
       
    }
}