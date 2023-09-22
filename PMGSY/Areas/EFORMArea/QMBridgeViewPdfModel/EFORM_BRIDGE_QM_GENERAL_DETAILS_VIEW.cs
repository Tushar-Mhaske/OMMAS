using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PMGSY.Areas.EFORMArea.QMBridgeViewPdfModel
{
    public class EFORM_BRIDGE_QM_GENERAL_DETAILS_VIEW
    {
        public string INSPECTION_DATE { get; set; }
        public string IS_WORK_STAT_LAYOUT { get; set; }
        public string IS_WORK_STAT_FOUNDATION { get; set; }
        public string IS_WORK_STAT_SUBSTRUCTURE { get; set; }
        public string IS_WORK_STAT_SUPERSTRUCTURE { get; set; }
        public string IS_WORK_STAT_PROT_WORK { get; set; }
        public string IS_WORK_STAT_APPROACH { get; set; }
        public string IS_WORK_STAT_FINISHING_STAGE { get; set; }
        public string IS_VIDEO_RECORDS { get; set; }

        public string TemplateVersion { get; set; }
    }
}