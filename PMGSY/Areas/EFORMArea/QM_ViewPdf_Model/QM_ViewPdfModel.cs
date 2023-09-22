using PMGSY.Areas.EFORMArea.Model;
using PMGSY.Areas.EFORMArea.QMModels;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PMGSY.Areas.EFORMArea.QM_ViewPdf_Model
{
    public class QM_ViewPdfModel
    {
        #region 1 to 10
        public string ErrorName { get; set; }

        public ApplicableCheck ApplicableCheck { get; set; }

        public Prefilled_Details_ViewModel PrefillDetailsViewModel { get; set; }
        
        public General_Details_ViewModel GeneralDetailsViewModel { get; set; }
        
        public List<Present_Work_Details_ViewModel> PresentWorkDetailsList { get; set; }
               
        public ArrangementsOBSDetails_ViewModel ArrangementObsDetailsViewModel { get; set; }

        public AttentionToQuality_ViewModel AttentionToqualityViewModel { get; set; }

        public List<AttentionQualityControl_ViewModel> AttentionQualityControlList { get; set; }

        public List<AttentionQualityTestResult_ViewModel> AttentionQualityTestResultList { get; set; }

        public Geometric_ViewModel GeometricViewModel { get; set; }        
                
        public List<EFORM_GEOMETRICS_OBS_DETAILS_QM> GeometricObsDetailList { get; set; }
        public EarthworkNewTechnology_ViewModel EarthworkNewTechnologyViewModel { get; set; }

        public List<EarthworkUCSDetail> EarthworkUCSList { get; set; }

        public List<EarthworkCBRDetail> EarthworkCBRList { get; set; }

        public List<EFORM_WORK_ITEM> WorkItemlistViewModel { get; set; }

        public QOM_Embankment_ViewModel QomEmbankmentModel { get; set; }

        public List<Group_Symbol_Soil_ViewModel> GroupSymbolSoilList { get; set; }

        public COMPAQ_EMBANKMENT_ViewModel CompqEmbankmentModel { get; set; }

        public List<DEGREE_OF_COMPAQ_ViewModel> DegreeofCompaqModelList { get; set; }

        public SIDE_SLOPES_ViewModel SideSlopeModel { get; set; }

        public List<SIDE_SLOPE_LIST_ViewModel> SideSlopeModelList { get; set; }

        public List<CUT_SLOPE_LIST_ViewModel> CutSlopeModelList { get; set; }

        public GRANULAR_SUBBASE_ViewModel GranularSubbaseViewModel { get; set; }

        public List<GRANULAR_SUBBASE_UCS_DETAILS_ViewModel> GranularSubaseUCSList { get; set; }

        public List<GRANULAR_QOM_WORKMENSHIP_ViewModel> GranularQOMWorkmenshipList { get; set; }

        public BASE_COURSE_I_View_Model BaseCourse1ViewModel { get; set; }

        public BASE_COURSE_II_ViewModel BaseCourse2viewModel { get; set; }

        public BASE_COURSE_III_ViewModel BaseCourse3viewModel { get; set; }

        public List<BASECOURSE_UCS_DETAILS_LAYER1_ViewModel> BaseCourse1_UCS_List { get; set; }

        public List<BASECOURSE_UCS_DETAILS_LAYER2_ViewModel> Basecourse2_UCSList { get; set; }

        public List<BASECOURSE_UCS_DETAILS_LAYER3_ViewModel> Basecourse3_UCSList { get; set; }

        public List<BASE_COURSE_OBSERVATION_WORKMANSHIP_LAYER1_View_Model> BaseCourse1_ObsWorkmanship_List { get; set; }

        public List<BASE_COURSE_OBSERVATION_WORKMANSHIP_LAYER2_View_Model> BaseCourse2_ObsWorkmanship_List { get; set; }

        public List<BASE_COURSE_OBSERVATION_WORKMANSHIP_LAYER3_ViewModel> BaseCourse3_ObsWorkmanship_List { get; set; }

        public BITUMINOUS_BASECOURSE_ViewModel BituminousBaseCourseViewModel { get; set; }

        public List<BITUMINOUS_BASECOURSE_OBSERVATION_ViewModel> BituminousBaseCourse_OBS_List { get; set; }

        #endregion

        #region 11 to 23

        public BITUMINOUS_SURFACE_ViewModel Bituminous_Surface_Course { get; set;  }

        public List<BITUMINOUS_SURFACE_COARSE_DETAILS> Bituminous_Surface_CourseList { get; set; }

        public QM_SHOULDERS QMShoulders { get; set; }

        public List<QM_SHOULDERS_UCS_DETAILS> QM_SHOULDERS_UCS_List { get; set; }

        public List<QM_SHOULDERS_MATERIAL_DETAILS> QM_SHOULDERS_MATERIAL_List { get; set; }

        public QM_PIPE_CULVERTS Pipeculvert { get; set; }

        public List<QM_PIPE_CULVERTS_DETAILS> PipeCulvertList { get; set; }

        public QM_SLAB_CULVERTS SlabCulvert { get; set; }

        public List<QM_SLAB_CULVERTS_DETAILS> SlabCulvertList { get; set; }

        public QM_PROTECTION_WORK QMProtectionWork { get; set; }

        public List<QM_PROTECTION_WORKS_QOM_DETAILS> ProtectionWorkQOMList { get; set; }

        public List<QM_PROTECTION_WORKS_WORKMANSHIP_DETAILS> ProtectionWorkWorkmanshipList { get; set; }

        public QM_CRASH_BARRIERS_ROAD_SAFETY CrashBarriersRoadSafetyModel { get; set; }

        public List<QM_CRASH_BARRIERS_OBSERVATION_DETAILS> CrashBarriersOBSList { get; set; }

        public QM_SIDE_AND_CATCH_DRAINS_EARTHEN SideCatchDrainModel { get; set; }

        public List<QM_SIDE_AND_CATCH_WT_DRAINS_DETAILS>  SideCatchDrainList { get; set; }

        public QM_CC_SR_PVAEMENTS CCSR_PavementsModel  { get; set; }

        public List<QM_CC_AND_SR_PAVEMENTS_OBS_DETAILS> CCSR_PavementsOBS_List { get; set; }

        public QM_CC_PUCCA_DRAINS CC_PuccaDrains  { get; set; }

        public List<QM_CC_PUCCA_DRAINS_OBS_DETAILS> CC_PuccaDrain_OBS_List { get; set; }

        public QM_ROAD_FURNITURE_MARKINGS FurnitureMarkingModel { get; set; }

        public List<QM_ROAD_FURNITURE_MARKINGS_OBS_DETAILS> FurnitureMarkingOBS_List { get; set; }

        public QM_DEFICIENCY_PREPARATION DeficiencyPreparationModel { get; set; }
        
        public QM_WORK_COMPLETED_IN_PROGRESS_ASPER_PROG  WorkInProgress_AsPerProgModel { get; set; }

        public QM_WORK_COMPLETED_IN_PROGRESS_ASPER_COST WorkInProgress_AsPerCostModel { get; set; }

        public List<QM_ACTION_TAKEN_PIU> ActionTakenByPIU_List { get; set; }

        public QM_DIFFEENCE_IN_OBSERVATION QMDiffObservationsModel { get; set; }

        public QM_QUALITY_GRADING QualityGradingModel { get; set; }
       
        public QM_OVERALL_GRADING OverallGradingModel  { get; set; }

        #endregion


    }
}