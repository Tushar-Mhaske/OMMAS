using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using static PMGSY.Areas.EFORMArea.Common.CommonEnum;
using System.ComponentModel.DataAnnotations;
using PMGSY.Areas.EFORMArea.Model;

namespace PMGSY.Areas.EFORMArea.TestReportPreviewModel
{
    public class EFORM_TR_TYPEA_SUMMARY_PAGE23_SRM_54_VIEW
    {
        public int TYPEA_SUMM_ID { get; set; }
        public int MAIN_ITEM_ID { get; set; }
        public int SUBITEM_ID { get; set; }
        public int EFORM_ID { get; set; }
        public int TABLE_ID { get; set; }
        public string IS_HEAVY_COMPACTION { get; set; }
        public string MDD_CH1 { get; set; }
        public string MDD_CH2 { get; set; }
        public string MDD_CH3 { get; set; }
        public string SAMPLE_AGE { get; set; }
        public string LAYER_NAME { get; set; }

        public string CC_PAVEMENT_TYPE_CH1_LAYER_NAME { get; set; }

        public string CC_PAVEMENT_TYPE_CH2_LAYER_NAME { get; set; }

        public string CC_PAVEMENT_TYPE_CH3_LAYER_NAME { get; set; }
        public string DESIGN_STRENGTH { get; set; }
        public string TESTING_DATE { get; set; }
        public string MOISTURE_CONTENT_METHOD { get; set; }
        public string REMARK { get; set; }
        public string COMMENT { get; set; }
        public string TESTED_BY_PIU { get; set; }
        public string TEST_CONDUCTED_IN_PRESENCE { get; set; }
        public string B_CONTENT { get; set; }
    }
}