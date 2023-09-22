using PMGSY.Areas.EFORMArea.PiuBridgeViewPdfModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PMGSY.Areas.EFORMArea.QMBridgeViewPdfModel
{
    public class EFORM_BRIDGE_CQC_PREVIEW_MODEL
    {   
        public EFORM_BRIDGE_PIU_PREVIEW_MODEL PIU_BRIDGE_VIEWMODEL { get; set; }

        public EFORM_BRIDGE_QM_PREVIEW_MODEL QM_BRIDGE_VIEWMODEL { get; set; }
    }
}