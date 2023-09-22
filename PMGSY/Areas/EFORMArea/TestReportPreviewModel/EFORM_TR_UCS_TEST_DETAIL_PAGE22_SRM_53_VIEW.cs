using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using static PMGSY.Areas.EFORMArea.Common.CommonEnum;
using System.ComponentModel.DataAnnotations;
using PMGSY.Areas.EFORMArea.Model;

namespace PMGSY.Areas.EFORMArea.TestReportPreviewModel
{
    public class EFORM_TR_UCS_TEST_DETAIL_PAGE22_SRM_53_VIEW
    {
        public int DETAILS_ID { get; set; }
        public int MAIN_ITEM_ID { get; set; }
        public int SUBITEM_ID { get; set; }
        public int TYPEA_SUMM_ID { get; set; }
        public int EFORM_ID { get; set; }
        public int ROAD_ID { get; set; }
        public string CHAINAGE { get; set; }
        public string SAMPLE_WT { get; set; }
        public string DENSITY { get; set; }
        public string TESTING_DATE { get; set; }
        public string SAMPLE_VOL { get; set; }
        public string LOAD_KN { get; set; }
        public string COMPR_STREANGTH { get; set; }
        public string IS_STD_CONFIRM { get; set; }

    }
}