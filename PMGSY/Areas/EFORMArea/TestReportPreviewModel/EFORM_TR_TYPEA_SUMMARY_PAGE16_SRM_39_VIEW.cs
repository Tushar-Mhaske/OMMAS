using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PMGSY.Areas.EFORMArea.Model;
using System.ComponentModel.DataAnnotations;
using static PMGSY.Areas.EFORMArea.Common.CommonEnum;

namespace PMGSY.Areas.EFORMArea.TestReportPreviewModel
{
    public class EFORM_TR_TYPEA_SUMMARY_PAGE16_SRM_39_VIEW
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
        public int? LAYER_ID { get; set; }
        public string CEMENT_GRADE { get; set; }
        public string TESTING_DATE { get; set; }
        public string MOISTURE_CONTENT_METHOD { get; set; }
        public string REMARK { get; set; }
        public string COMMENT { get; set; }
        public string TESTED_BY_PIU { get; set; }
        public string TEST_CONDUCTED_IN_PRESENCE { get; set; }
        public string B_CONTENT { get; set; }
    }
}