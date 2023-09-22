using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PMGSY.Areas.EFORMArea.PiuBridgeViewPdfModel
{
    public class EFORM_BRIDGE_PIU_QC_DETAILS_VIEW
    {
        public string LAB_LOCATION { get; set; }
        public string PHOTO_UPLOAD_DATE { get; set; }
        public string ESTB_DELAY_REASON { get; set; }
        public string LAB_EQUIP_AVBL { get; set; }
        public string EQUIP_WORKING { get; set; }
        public string EQUIP_NOT_WORKING { get; set; }
        public string LAB_EQUIP_NOT_AVBL { get; set; }
        public string REASON_LAB_EQUIP_NOT_AVBL { get; set; }
        public string CALIBRATION_DETAILS { get; set; }
        public string DOCUMENT_FOR_QM { get; set; }

    }
}