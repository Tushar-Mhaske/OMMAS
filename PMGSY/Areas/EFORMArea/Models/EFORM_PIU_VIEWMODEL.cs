using PMGSY.Areas.EFORMArea.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PMGSY.Areas.EFORMArea.Model
{
    public class EFORM_PIU_VIEWMODEL
    {

        #region Create/Added By Rohit Borse on 22-03-2022
        public EFORM_PREFILLED_PIU Prefilled_Details { get; set; }
        public List<EFORM_WORK_ITEM> Work_Item_List { get; set; }

        #endregion
        public EFORM_GENERAL_INFO_PIU_View GENERAL_INFO_PIU_view { get; set; }
        //public List<EFORM_GENERAL_INFO_PIU> GENERAL_INFO_PIU { get; set; }
        public EFORM_GENERAL_INFO_PIU GENERAL_INFO_PIU { get; set; }
        public  EFORM_NEW_TECH_PIU new_tech_piu { get; set; }

        public List<EFORM_NEW_TECHNOLOGY_DETAILS_PIU> NEW_TECHNOLOGY_DETAILS_LIST { get; set; }
        
        public EFORM_QC_DETAILS_PIU_View QC_DETAILS_PIU_view { get; set; }
        public List<EFORM_PRGS_DETAILS_PIU> Physical_Progress_List { get; set; }
        public EFORM_QC_DETAILS_PIU QC_DETAILS_PIU { get; set; }
        public List<EFORM_QC_OFFICIAL_DETAILS_PIU> QC_OFFICIAL_DETAILS_LIST { get; set; }
        public List<EFORM_MIX_DESIGN_DETAILS_PIU> MIX_DESIGN_DETAILS_LIST { get; set; }
         public List<EFORM_PREVIOUS_INSP_DETAILS_PIU> PREVIOUS_INSP_DETAILS_LIST { get; set; }

        public List<EFORM_PREVIOUS_INSP_DETAILS_PIU_Temp3_0> PREVIOUS_INSP_DETAILS_LIST_temp3_0 { get; set; }  //add on 01-08-2022

        public List<NewTechPIU_ViewModel> NewTechnology_PIU_List { get; set; }

        public bool ErrorOccured { get; set; }

        public List<string> ErrorList { get; set; }
    }
}