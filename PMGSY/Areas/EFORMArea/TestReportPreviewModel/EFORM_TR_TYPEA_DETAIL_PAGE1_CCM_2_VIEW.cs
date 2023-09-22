using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using PMGSY.Areas.EFORMArea.Model;
using System.ComponentModel.DataAnnotations;
using static PMGSY.Areas.EFORMArea.Common.CommonEnum;
namespace PMGSY.Areas.EFORMArea.TestReportPreviewModel
{
    public class EFORM_TR_TYPEA_DETAIL_PAGE1_CCM_2_VIEW
    {
        public int TYPEA_DETAIL_ID { get; set; }
        public int MAIN_ITEM_ID { get; set; }
        public int SUBITEM_ID { get; set; }
        public int DETAIL_ITEM_ID { get; set; }
        public int TYPEA_SUMM_ID { get; set; }
        public int EFORM_ID { get; set; }
        public int ROAD_ID { get; set; }
        public string CH1_VALUE { get; set; }
        public string CH2_VALUE { get; set; }
        public string CH3_VALUE { get; set; }
        public string CH4_VALUE { get; set; }
    }
}