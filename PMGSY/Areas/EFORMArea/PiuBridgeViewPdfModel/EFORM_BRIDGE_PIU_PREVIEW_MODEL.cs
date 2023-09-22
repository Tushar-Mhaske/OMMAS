using PMGSY.Areas.EFORMArea.PiuBridgeModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PMGSY.Areas.EFORMArea.PiuBridgeViewPdfModel
{
    public class EFORM_BRIDGE_PIU_PREVIEW_MODEL
    {
        

     public List<EFORM_BRIDGE_MASTER_WORK_ITEM_VIEW> BRIDGE_MASTER_WORK_ITEM_VIEW { get; set; }
        public EFORM_PDF_UPLOAD_DETAIL_VIEW PDF_UPLOAD_DETAIL_VIEW { get; set; }
        public PIU_BRIDGE_GET_PREFILLRD_DETAILS BRIDGE_PIU_PREFILLED_VIEW { get; set;}
        public EFORM_BRIDGE_PIU_GENERAL_INFO_VIEW BRIDGE_PIU_GENERAL_INFO_VIEW { get; set; }

        public List<EFORM_BRIDGE_PIU_MIX_DESIGN_DETAILS_VIEW> BRIDGE_PIU_MIX_DESIGN_DETAIL_VIEW { get; set; }

        public EFORM_BRIDGE_PIU_PARTICULARS_VIEW BRIDGE_PIU_PARTICULAR_VIEW { get; set; }

        public List<EFORM_BRIDGE_PIU_PREVIOUS_INSP_DETAILS_VIEW> BRIDGE_PIU_PREVIOUS_INSP_DETAIL_VIEW { get; set; }

        public List<EFORM_BRIDGE_PIU_PRGS_DETAILS_VIEW> BRIDGE_PIU_PRGS_DETAILS_VIEW { get; set; }

        public EFORM_BRIDGE_PIU_QC_DETAILS_VIEW BRIDGE_PIU_QC_DETAIL_VIEW { get; set; }

        public List<EFORM_BRIDGE_QC_OFFICIAL_DETAILS_VIEW> BRIDGE_QC_OFFICIAL_DETAILS_VIEW { get; set; }
    }
}