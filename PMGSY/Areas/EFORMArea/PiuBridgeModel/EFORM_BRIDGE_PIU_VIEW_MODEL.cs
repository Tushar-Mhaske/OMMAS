using PMGSY.Areas.EFORMArea.PiuBridgeModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PMGSY.Areas.EFORMArea.PiuBridgeModel
{
    public class EFORM_BRIDGE_PIU_VIEW_MODEL
    {

        public EFORM_BRIDGE_PIU_GENERAL_INFO BRIDGE_PIU_GENERAL_INFO { get; set; }

        public List<EFORM_BRIDGE_PIU_MIX_DESIGN_DETAILS> BRIDGE_PIU_MIX_DESIGN_DETAIL { get; set; }

        public EFORM_BRIDGE_PIU_PARTICULARS BRIDGE_PIU_PARTICULAR  { get; set; }

        public List<EFORM_BRIDGE_PIU_PREVIOUS_INSP_DETAILS> BRIDGE_PIU_PREVIOUS_INSP_DETAIL { get; set; }

        public List<EFORM_BRIDGE_PIU_PRGS_DETAILS> BRIDGE_PIU_PRGS_DETAILS { get; set; }

        public EFORM_BRIDGE_PIU_QC_DETAILS BRIDGE_PIU_QC_DETAIL { get; set; }

        public List<EFORM_BRIDGE_QC_OFFICIAL_DETAILS> BRIDGE_QC_OFFICIAL_DETAILS { get; set; }


        public bool ErrorOccured { get; set; }

        public List<string> ErrorList { get; set; }
    }
}