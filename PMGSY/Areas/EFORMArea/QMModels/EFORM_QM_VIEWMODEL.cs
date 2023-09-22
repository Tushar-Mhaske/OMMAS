using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PMGSY.Areas.EFORMArea.QMModels
{
    public class EFORM_QM_VIEWMODEL
    {

        #region ---- pages from 6-18-------


        //-----Saurabh-------
        public EFORM_GENERAL_DETAILS_QM general_details_model { get; set; }
        public EFORM_QM_ARRANGEMENTS_OBS_DETAILS QM_ARRANGEMENT_OBS_DETAIL { get; set; }

        public EFORM_QM_QUALITY_ATTENTION QM_QUALITY_ATTENTION { get; set; }

        public List<EFORM_QM_QC_TEST_DETAILS> QM_QC_TEST_DETAILS { get; set; }

        public List<EFORM_QM_QC_TEST_DETAILS_Temp2_0> QM_QC_TEST_DETAILS_Temp2_0 { get; set; }   //add on 28-07-2022

        public List<EFORM_QM_TEST_RESULT_VERIFICATION_DETAILS> QM_TEST_RESULT_VERIFICATION_DETAILS_List { get; set; }

        public List<EFORM_QM_TEST_RESULT_VERIFICATION_DETAILS_Temp2_0> QM_TEST_RESULT_VERIFICATION_DETAILS_List_Temp_2_0 { get; set; }  //add on 28-07-2022

        public EFORM_QM_GEOMETRICS_DETAILS QM_GEOMETRICS_DETAILS { get; set; }

        public List<EFORM_QM_GEOMETRICS_OBS_DETAILS> QM_GEOMETRICS_OBS_DETAILS_List { get; set; }

        public List<EFORM_QM_PRESENT_WORK_DETAILS> QM_PRESENT_WORK_DETAILS { get; set; }

        public EFORM_QM_SIDE_SLOPES QM_SIDE_SLOPES { get; set; }

        public List<EFORM_QM_CHILD_SIDE_SLOPE_DETAIL> QM_CHILD_SIDE_SLOPE_DETAIL_List { get; set; }

        public List<EFORM_QM_CHILD_CUT_SLOPE_DETAIL> QM_CHILD_CUT_SLOPE_DETAIL_List { get; set; }


        //---Bhushan------------
        public EFORM_QM_NEW_TECHNOLOGY_DETAILS QM_NEW_TECHNOLOGY_DETAILS { get; set; }

        public List<EFORM_QM_CHILD_EARTHWORK_SUBGRADE_UCS_DETAILS> QM_CHILD_UCS_DETAILS { get; set; }

        public List<EFORM_QM_CHILD_EARTHWORK_SUBGRADE_CBR_DETAILS> QM_CHILD_CBR_DETAILS { get; set; }

        public EFORM_QM_QOM_EMBANKMENT QM_QOM_EMBANKMENT { get; set; }

        public List<EFORM_QM_CHILD_GROUP_SYMBOL_SOIL> QM_CHILD_GROUP_SYMBOL_SOIL { get; set; }

        public EFORM_QM_COMPAQ_EMBANKMENT QM_COMPAQ_EMBANKMENT { get; set; }

        public List<EFORM_QM_CHILD_DEGREE_OF_COMPAQ> QM_CHILD_DEGREE_OF_COMPAQ { get; set; }



        public EFORM_GRANULAR_SUBBASE_QM granular_subbase_mod { get; set; }


        public List<EFORM_CHILD_GRANULAR_UCS_DETAILS_QM> child_granular_UCS_List { get; set; }
        public List<EFORM_CHILD_GRANULAR_QOM_OBS_DETAILS_QM> child_granular_QOM_OBS_list { get; set; }

        public EFORM_BASE_COURSE_I_QM base_course_1_details { get; set; }
        public List<EFORM_CHILD_BASECOURSE_UCS_DETAILS_LAYER1_QM> child_base_coarse_l1_ucs_list { get; set; }
        public List<EFORM_CHILD_OBSERVATION_WORKMANSHIP_LAYER1_QM> child_base_coarse_l1_workmanship_list { get; set; }

        public EFORM_BASE_COURSE_2_QM base_course_2_details { get; set; }
        public List<EFORM_CHILD_BASECOURSE_UCS_DETAILS_LAYER2_QM> child_base_coarse_l2_ucs_list { get; set; }
        public List<EFORM_CHILD_OBSERVATION_WORKMANSHIP_LAYER2_QM> child_base_coarse_l2_workmanship_list { get; set; }

        public EFORM_BASE_COURSE_3_QM base_course_3_details { get; set; }
        public List<EFORM_CHILD_BASECOURSE_UCS_DETAILS_LAYER3_QM> child_base_coarse_l3_ucs_list { get; set; }
        public List<EFORM_CHILD_OBSERVATION_WORKMANSHIP_LAYER3_QM> child_base_coarse_l3_workmanship_list { get; set; }













        #endregion

        #region  ---vikky pages 23-27----

        public EFORM_CDWORKS_PIPE_CULVERTS_QM CDWORKS_PIPE_CULVERTS_details { get; set; }

        public List<EFORM_CHILD_CDWORKS_PIPE_CULVERTS_DETAILS_QM> CHILD_CDWORKS_PIPE_CULVERTS_list { get; set; }

        public EFORM_CDWORKS_SLAB_CULVERTS_QM CDWORKS_SLAB_CULVERTS_details { get; set; }

        public List<EFORM_CHILD_CDWORKS_SLAB_CULVERTS_DETAILS_QM> CHILD_CDWORKS_SLAB_CULVERTS_list { get; set; }
        public EFORM_PROTECTION_WORK_QM PROTECTION_WORK_details { get; set; }

        public List<EFORM_CHILD_PROT_WORKS_QOM_DETAILS_QM> CHILD_PROT_WORKS_QOM_list { get; set; }


        public List<EFORM_CHILD_PROT_WORKS_WORKMANSHIP_OF_RS_DETAILS_QM> CHILD_PROT_WORKS_WORKMANSHIP_OF_RS_list { get; set; }



        public EFORM_CRASH_BARRIERS_ROAD_SAFETY_QM CRASH_BARRIERS_ROAD_SAFETY_details { get; set; }

        public List<EFORM_CHILD_CRASH_BARRIERS_OBSERVATION_DETAILS_QM> CHILD_CRASH_BARRIERS_OBSERVATION_list { get; set; }

        public EFORM_SIDE_AND_CATCH_DRAINS_EARTHEN_QM SIDE_AND_CATCH_DRAINS_EARTHEN_details { get; set; }

        public List<EFORM_CHILD_SD_AND_CW_DRAINS_DETAILS_QM> CHILD_SD_AND_CW_DRAINS_list { get; set; }



        #endregion

        #region --bhushan 27-30-----
        public EFORM_QM_CC_SR_PVAEMENTS QM_CC_SR_PVAEMENTS { get; set; }

        public List<EFORM_QM_CHILD_CC_AND_SR_PAVEMENTS_OBSERVATION_DETAILS> QM_CHILD_CC_AND_SR_PAVEMENTS_OBSERVATION_DETAILS { get; set; }

        public EFORM_QM_CC_PUCCA_DRAINS QM_CC_PUCCA_DRAINS { get; set; }

        public List<EFORM_QM_CHILD_CC_PUCCA_DRAINS_OBSERVATION_DETAILS> QM_CHILD_CC_PUCCA_DRAINS_OBSERVATION_DETAILS { get; set; }

        public EFORM_QM_ROAD_FURNITURE_MARKINGS QM_ROAD_FURNITURE_MARKINGS { get; set; }

        public List<EFORM_QM_CHILD_ROAD_FURNITURE_MARKINGS_OBSERVATION_DETAILS> QM_CHILD_ROAD_FURNITURE_MARKINGS_OBSERVATION_DETAILS { get; set; }




        #endregion

        #region --saurabh 30-33----
        public EFORM_DEFICIENCY_PREPARATION_QM EFORM_DEFICIENCY_PREPARATION_QM { get; set; }

        public EFORM_WORK_COMPLETED_IN_PROGRESS_ASPER_PROG_QM EFORM_WORK_COMPLETED_IN_PROGRESS_ASPER_PROG_QM { get; set; }


        public EFORM_WORK_COMPLETED_IN_PROGRESS_ASPER_COST_QM EFORM_WORK_COMPLETED_IN_PROGRESS_ASPER_COST_QM { get; set; }

        public List<EFORM_ACTION_TAKEN_PIU_QM> EFORM_ACTION_TAKEN_PIU_QM_List { get; set; }

        public EFORM_DIFFEENCE_IN_OBSERVATION_QM EFORM_DIFFEENCE_IN_OBSERVATION_QM { get; set; }


        #endregion

        #region --- Srishti Page 19-20 34-38 ---

        public EFORM_QM_BITUMINOUS_BASE_COURSE QM_BITUMINOUS_BASE_COURSE { get; set; }
        public List<EFORM_QM_CHILD_BITUMINOUS_OBSERVATION_DETAILS> QM_CHILD_BITUMINOUS_OBSERVATION_DETAILS { get; set; }
        public EFORM_QM_BITUMINOUS_SURFACE_COURSE QM_BITUMINOUS_SURFACE_COURSE { get; set; }
        public List<EFORM_QM_CHILD_BITUMINOUS_SURFACE_COARSE_DETAILS> QM_CHILD_BITUMINOUS_SURFACE_COARSE_DETAILS { get; set; }
        public EFORM_QM_SHOULDERS QM_SHOULDERS { get; set; }
        public List<EFORM_QM_CHILD_SHOULDERS_UCS_DETAILS> QM_CHILD_SHOULDERS_UCS_DETAILS { get; set; }
        public List<EFORM_QM_CHILD_SHOULDERS_MATERIAL_DETAILS> QM_CHILD_SHOULDERS_MATERIAL_DETAILS { get; set; }
        public EFORM_QM_QUALITY_GRADING QM_QUALITY_GRADING { get; set; }
        public EFORM_QM_OVERALL_GRADING QM_OVERALL_GRADING { get; set; }

        #endregion

        public bool ErrorOccured { get; set; }

        public List<string> ErrorList { get; set; }


        
    }
}