﻿using PMGSY.Areas.EFORMArea.Model;
using PMGSY.Areas.EFORMArea.PiuBridgeModel;
using PMGSY.Areas.EFORMArea.QM_ViewPdf_Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PMGSY.Areas.EFORMArea.QMBridgeViewPdfModel
{
    public class EFORM_BRIDGE_QM_PREVIEW_MODEL
    {
        public List<EFORM_WORK_ITEM> WorkItemlistViewModel  { get; set; }

        public ApplicableCheck ApplicableCheck { get; set; }

        #region  vikky 9-15

        public PIU_BRIDGE_GET_PREFILLRD_DETAILS BRIDGE_PIU_PREFILLED_VIEW { get; set; }

        public EFORM_BRIDGE_QM_GENERAL_DETAILS_VIEW BRIDGE_QM_GENERAL_DETAILS_VIEW { get; set; }

        public EFORM_BRIDGE_QM_ARRANGEMENTS_OBS_DETAILS_VIEW BRIDGE_QM_ARRANGEMENT_OBS_DETAIL_VIEW { get; set; }

        public EFORM_BRIDGE_QM_QUALITY_ATTENTION_VIEW BRIDGE_QM_QUALITY_ATTENTION_VIEW { get; set; }

        public List<EFORM_BRIDGE_QM_QC_TEST_DETAILS_VIEW> BRIDGE_QM_QC_TEST_DETAILS_LIST_VIEW { get; set; }

        public List<EFORM_BRIDGE_QM_TEST_RESULT_VERIFICATION_DETAILS_VIEW> BRIDGE_QM_TEST_RESULT_VERIFICATION_DETAILS_LIST_VIEW { get; set; }

        public EFORM_BRIDGE_QM_FOUNDATION_VIEW BRIDGE_QM_FOUNDATION_VIEW { get; set; }

        public EFORM_BRIDGE_QM_ONGOING_FOUNDATION_VIEW BRIDGE_QM_ONGOING_FOUNDATION_VIEW { get; set; }

        public List<EFORM_BRIDGE_QM_CHILD_ON_QOM_FOUNDATION_VIEW> BRIDGE_QM_CHILD_ON_QOM_FOUNDATION_LIST_VIEW { get; set; }
        public List<EFORM_BRIDGE_QM_CHILD_ON_WORKMENSHIP_FOUNDATION_VIEW> BRIDGE_QM_CHILD_ON_WORKMENSHIP_FOUNDATION_LIST_VIEW { get; set; }
        public List<EFORM_BRIDGE_QM_CHILD_COMPL_QOM_FOUNDATION_VIEW> BRIDGE_QM_CHILD_COMPL_QOM_FOUNDATION_LIST_VIEW { get; set; }
        public EFORM_BRIDGE_QM_COMPLETED_FOUNDATION_VIEW BRIDGE_QM_COMPLETED_FOUNDATION_VIEW { get; set; }


        #endregion

        #region saurabh 15-22
        public EFORM_BRIDGE_QM_SUBSTRUCTURE_VIEW BRIDGE_QM_SUBSTRUCTURE_VIEW { get; set; }

        public EFORM_BRIDGE_QM_ONGOING_SUBSTRUCTURE_VIEW BRIDGE_QM_ONGOING_SUBSTRUCTURE_VIEW { get; set; }

        public List<EFORM_BRIDGE_QM_CHILD_ON_QOM_SUBSTRUCTURE_VIEW> BRIDGE_QM_CHILD_ON_QOM_SUBSTRUCTURE_LIST_VIEW { get; set; }

        public List<EFORM_BRIDGE_QM_CHILD_ON_WORKMENSHIP_SUBSTRUCTURE_VIEW> BRIDGE_QM_CHILD_ON_WORKMENSHIP_SUBSTRUCTURE_LIST_VIEW { get; set; }

        public EFORM_BRIDGE_QM_COMPLETED_SUBSTRUCTURE_VIEW BRIDGE_QM_COMPLETED_SUBSTRUCTURE_VIEW { get; set; }

        public List<EFORM_BRIDGE_QM_CHILD_COMPLETED_SUBSTRUCTURE_VIEW> BRIDGE_QM_CHILD_COMPLETED_SUBSTRUCTURE_LIST_VIEW { get; set; }


        // SUPERSTRUCTURE
        public EFORM_BRIDGE_QM_SUPERSTRUCTURE_VIEW BRIDGE_QM_SUPERSTRUCTURE_VIEW { get; set; }

        public EFORM_BRIDGE_QM_CHILD_RCC_SUPERSTRUCTURE_ONGOING_VIEW BRIDGE_QM_CHILD_RCC_SUPERSTRUCTURE_VIEW { get; set; }

        public EFORM_BRIDGE_QM_CHILD_RCC_SUPERSTRUCTURE_COMPLETED_VIEW BRIDGE_QM_CHILD_RCC_SUPERSTRUCTURE_COMPLETED_VIEW { get; set; }

        public List<EFORM_BRIDGE_QM_CHILD_ON_QOM_RCC_SUPERSTRUCTURE_VIEW> BRIDGE_QM_CHILD_ON_QOM_RCC_SUPERSTRUCTURE_LIST_VIEW { get; set; }

        public List<EFORM_BRIDGE_QM_CHILD_COMPL_ACTIVITY_RCC_SUPERSTRUCTURE_VIEW> BRIDGE_QM_CHILD_COMPL_ACTIVITY_RCC_SUPERSTRUCTURE_LIST_VIEW { get; set; } // COMPLETED



        public EFORM_BRIDGE_QM_CHILD_STEEL_SUPERSTRUCTURE_ONGOING_VIEW BRIDGE_QM_CHILD_STEEL_SUPERSTRUCTURE_VIEW { get; set; }

        public EFORM_BRIDGE_QM_CHILD_STEEL_SUPERSTRUCTURE_COMPLETED_VIEW BRIDGE_QM_CHILD_STEEL_SUPERSTRUCTURE_COMPLETED_VIEW { get; set; }

        public List<EFORM_BRIDGE_QM_CHILD_ON_QOM_STEEL_SUPERSTRUCTURE_VIEW> BRIDGE_QM_CHILD_ON_QOM_STEEL_SUPERSTRUCTURE_LIST_VIEW { get; set; }

        public List<EFORM_BRIDGE_QM_CHILD_CO_MATERIAL_STEEL_SUPERSTRUCTURE_VIEW> BRIDGE_QM_CHILD_CO_MATERIAL_STEEL_SUPERSTRUCTURE_LIST_VIEW { get; set; }


        public EFORM_BRIDGE_QM_CHILD_BAILEY_BRIDGE_SUPERSTRUCTURE_ONGOING_VIEW BRIDGE_QM_CHILD_BAILEY_BRIDGE_SUPERSTRUCTURE_VIEW { get; set; }

        public EFORM_BRIDGE_QM_CHILD_BAILEY_BRIDGE_SUPERSTRUCTURE_COMPLETED_VIEW BRIDGE_QM_CHILD_BAILEY_BRIDGE_SUPERSTRUCTURE_COMPLETED_VIEW { get; set; }

        public List<EFORM_BRIDGE_QM_CHILD_ON_QOM_BAILEY_SUPERSTRUCTURE_VIEW> BRIDGE_QM_CHILD_ON_QOM_BAILEY_SUPERSTRUCTURE_LIST_VIEW { get; set; }

        public List<EFORM_BRIDGE_QM_CHILD_CO_MATERIAL_BAILEY_SUPERSTRUCTURE_VIEW> BRIDGE_QM_CHILD_CO_MATERIAL_BAILEY_SUPERSTRUCTURE_LIST_VIEW { get; set; }

        #endregion

        #region vikky 23-28
        public EFORM_BRIDGE_QM_LOAD_TEST_VIEW BRIDGE_QM_LOAD_TEST_VIEW { get; set; }

        public EFORM_BRIDGE_QM_BEARING_VIEW BRIDGE_QM_BEARING_VIEW { get; set; }

        public EFORM_BRIDGE_QM_CHILD_BEARING_TYPE BRIDGE_QM_CHILD_BEARING_TYPE_VIEW { get; set; }


        public EFORM_BRIDGE_QM_EXPANSION_VIEW BRIDGE_QM_EXPANSION_VIEW { get; set; }

        public EFORM_BRIDGE_QM_APPROACH_VIEW BRIDGE_QM_APPROACH_VIEW { get; set; }

        public List<EFORM_BRIDGE_QM_CHILD_EMBANKMENT_APPROACH_VIEW> BRIDGE_QM_CHILD_EMBANKMENT_APPROACH_LIST_VIEW { get; set; }

        public List<EFORM_BRIDGE_QM_CHILD_SUB_BASE_APPROACH_VIEW> BRIDGE_QM_CHILD_SUB_BASE_APPROACH_LIST_VIEW { get; set; }

        public List<EFORM_BRIDGE_QM_CHILD_BASE_COURSE_APPROACH_VIEW> BRIDGE_QM_CHILD_BASE_COURSE_APPROACH_LIST_VIEW { get; set; }

        public List<EFORM_BRIDGE_QM_CHILD_WEARING_COURSE_APPROACH_VIEW> BRIDGE_QM_CHILD_WEARING_COURSE_APPROACH_LIST_VIEW { get; set; }


        public EFORM_BRIDGE_QM_CHILD_PROTECH_APPROACH_VIEW BRIDGE_QM_CHILD_PROTECH_APPROACH_VIEW { get; set; }

        public List<EFORM_BRIDGE_QM_CHILD_QOM_APPROACH_VIEW> BRIDGE_QM_CHILD_QOM_APPROACH_LIST_VIEW { get; set; }

        public List<EFORM_BRIDGE_QM_CHILD_WORKMENSHIP_APPROACH_VIEW> BRIDGE_QM_CHILD_WORKMENSHIP_APPROACH_LIST_VIEW { get; set; }

        #endregion

        #region Bhushan 29-35
        public EFORM_BRIDGE_QM_FURNITURE_MARKINGS_VIEW BRIDGE_QM_FURNITURE_MARKINGS_VIEW { get; set; }
        public EFORM_BRIDGE_QM_CHILD_OBS_FURNITURE_MARKING_VIEW BRIDGE_QM_CHILD_OBS_FURNITURE_MARKING_VIEW { get; set; }
        public EFORM_BRIDGE_QM_DEFICIENCY_VIEW BRIDGE_QM_DEFICIENCY_VIEW { get; set; }
        public EFORM_BRIDGE_QM_WORK_COMPLETED_IN_PROGRESS_ASPER_PROG_VIEW BRIDGE_QM_WORK_COMPLETED_IN_PROGRESS_ASPER_PROG_VIEW { get; set; }
        public EFORM_BRIDGE_QM_WORK_COMPLETED_IN_PROGRESS_ASPER_COST_VIEW BRIDGE_QM_WORK_COMPLETED_IN_PROGRESS_ASPER_COST_VIEW { get; set; }
        public List<EFORM_BRIDGE_QM_ACTION_TAKEN_PIU_VIEW> BRIDGE_QM_ACTION_TAKEN_PIU_LIST_VIEW { get; set; }
        public EFORM_BRIDGE_QM_DIFFERENCE_IN_OBSERVATION_VIEW BRIDGE_QM_DIFFERENCE_IN_OBSERVATION_VIEW { get; set; }
        public EFORM_BRIDGE_QM_OVERALL_GRADING_VIEW BRIDGE_QM_OVERALL_GRADING_VIEW { get; set; }
        public EFORM_BRIDGE_QM_QUALITY_GRADING_VIEW BRIDGE_QM_QUALITY_GRADING_VIEW { get; set; }
        #endregion

    }
}