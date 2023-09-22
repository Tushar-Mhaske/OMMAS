#region File Header
/*
        * Project Id    :
        * Project Name  :   OMMAS II
        * Name          :   QMScheduleViewModel.cs        
        * Description   :   Includes different model classes for scheduling, filling observations, correcting  grades process in quality module
        * Author        :   Shyam Yadav 
        * Creation Date :   10/Jun/2013
 **/
#endregion

using PMGSY.BLL.Common;
using PMGSY.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PMGSY.Models.QualityMonitoring
{
    public class QMScheduleViewModel
    {
        public QMScheduleViewModel(){}

        public string OPERATION { get; set; }
        public int ADMIN_SCHEDULE_CODE { get; set; }

        [Display(Name = "Monitor")]
        [Required]
        public int ADMIN_QM_CODE { get; set; }
        public List<SelectListItem> MONITORS { set; get; }


        [Display(Name = "Year")]
        [Required]
        public int ADMIN_IM_YEAR { get; set; }
        public List<SelectListItem> YEARS_LIST { set; get; }


        [Display(Name = "Month")]
        [Required]
        public int ADMIN_IM_MONTH { get; set; }
        public List<SelectListItem> MONTHS_LIST { set; get; }


        public List<SelectListItem> DISTRICTS { set; get; }

        [Display(Name = "State")]
        [Required]
        [Range(1, Int16.MaxValue, ErrorMessage = "State field is required")]
        public int MAST_STATE_CODE { get; set; }
        public List<SelectListItem> STATES { get; set; }

        //For Multiselect
        public int DISTRICT_LIST { get; set; }
        public string ASSIGNED_DISTRICT_LIST { get; set; }

        public int MAST_DISTRICT_CODE { get; set; }
        public Nullable<int> MAST_DISTRICT_CODE2 { get; set; }
        public Nullable<int> MAST_DISTRICT_CODE3 { get; set; }
        public Nullable<System.DateTime> SCHEDULE_DATE { get; set; }
        public string ADMIN_IS_ENQUIRY { get; set; }
        public string FINALIZE_FLAG { get; set; }
        public string INSP_STATUS_FLAG { get; set; }


    
        public virtual ADMIN_QUALITY_MONITORS ADMIN_QUALITY_MONITORS { get; set; }
        public virtual MASTER_DISTRICT MASTER_DISTRICT { get; set; }
        public virtual MASTER_DISTRICT MASTER_DISTRICT1 { get; set; }
        public virtual MASTER_DISTRICT MASTER_DISTRICT2 { get; set; }
        public virtual MASTER_STATE MASTER_STATE { get; set; }
        public virtual ICollection<QUALITY_QM_OBSERVATION_MASTER> QUALITY_QM_OBSERVATION_MASTER { get; set; }
        public virtual ICollection<QUALITY_QM_SCHEDULE_DETAILS> QUALITY_QM_SCHEDULE_DETAILS { get; set; }
        
    }



    public class QMCQCAddDistrictModel
    {
        public int ADMIN_SCHEDULE_CODE { get; set; }
        
        [Display(Name = "District")]
        [Required]
        [Range(1, Int16.MaxValue, ErrorMessage = "District field is required")]
        public int MAST_DISTRICT_CODE { get; set; }

        public List<SelectListItem> DISTRICTS { get; set; }
    }


    public class QMAssignRoadsModel
    {

        public int MAST_STATE_CODE { get; set; }
        public int SCHEDULE_MONTH { get; set; }
        public int SCHEDULE_YEAR { get; set; }
        public int ADMIN_QM_CODE { get; set; }

        [Display(Name = "District")]
        [Required]
        [Range(1, Int16.MaxValue, ErrorMessage = "District field is required")]
        public int MAST_DISTRICT_CODE { get; set; }
        public List<SelectListItem> DISTRICTS { get; set; }


        [Display(Name = "Sanction Year")]
        [Required]
        [Range(1, Int16.MaxValue, ErrorMessage = "Sanction Year field is required")]
        public int IMS_YEAR { get; set; }
        public List<SelectListItem> YEARS { get; set; }

        [Display(Name = "Road Status")]
        public string ROAD_STATUS { get; set; }
        public List<SelectListItem> ROAD_STATUS_LIST { get; set; }

        public int ADMIN_SCHEDULE_CODE { get; set; }
        public int IMS_PR_ROAD_CODE { get; set; }
        public string DEVICE_FLAG { get; set; }
        public string FINALIZE_FLAG { get; set; }
        public string SCHEDULE_ASSIGNED { get; set; }
        public string INSP_STATUS_FLAG { get; set; }
        public string CQC_FORWARD_FLAG { get; set; }
        public string IS_SCHEDULE_DOWNLOAD { get; set; }
        public Nullable<int> TOTAL_IMAGE_COUNT { get; set; }

        public virtual IMS_SANCTIONED_PROJECTS IMS_SANCTIONED_PROJECTS { get; set; }
        public virtual QUALITY_QM_SCHEDULE QUALITY_QM_SCHEDULE { get; set; }

        
    }

    public class QMDistrictwiseSchDetailsModel
    {
        public int ADMIN_SCHEDULE_CODE { get; set; }
        public string INSP_STATUS_FLAG { get; set; }
    }

    public class QMPreviousScheduleModel
    {
        [Display(Name = "Monitor")]
        [Required]
        public int ADMIN_QM_CODE { get; set; }
        public List<SelectListItem> MONITORS { set; get; }

        [Display(Name = "Year")]
        public int ADMIN_IM_YEAR { get; set; }
        public List<SelectListItem> YEARS_LIST { set; get; }


        [Display(Name = "Month")]
        public int ADMIN_IM_MONTH { get; set; }
        public List<SelectListItem> MONTHS_LIST { set; get; }

        public string QM_TYPE { get; set; }
    }

    public class QMObsSchListModel
    {
        [Display(Name = "Year")]
        public int ADMIN_IM_YEAR { get; set; }
        public List<SelectListItem> YEARS_LIST { set; get; }

        [Display(Name = "Month")]
        public int ADMIN_IM_MONTH { get; set; }
        public List<SelectListItem> MONTHS_LIST { set; get; }
    }



    public class QMFillObservationModel
    {
        public QMFillObservationModel()
        {
            CommonFunctions objCommon = new CommonFunctions();
            this.CURRENT_DATE = objCommon.GetDateTimeToString(DateTime.Now);
        }

        public int LSBOverallCorrectedGrade { get; set; }

        public int ADMIN_SCHEDULE_CODE { get; set; }
        public int IMS_PR_ROAD_CODE { get; set; }
        public int QM_OBSERVATION_ID { get; set; }
        public bool IS_3RD_TIER_SQC { get; set; }
        public string SCHEDULE_MONTH_YEAR_START_DATE { get; set; }
        public string SCHEDULE_MONTH_YEAR_END_DATE { get; set; }// Added by deendayal on 15/9/2017 to restrict the monitor to select any other month's day

        public int MAST_PCI_GRADE { get; set; }
        public List<qm_monitor_master_grade_items_Result> MASTER_GRADE_ITEM_LIST { get; set; }
        public List<qm_observation_grading_detail_Result> GRADE_DETAILS_LIST { get; set; }
        public List<qm_observation_grading_detail_for_atr_Result> GRADE_DETAILS_LIST_ATR { get; set; }
        public List<qm_observation_grading_detail_correction_Result> GRADE_DETAILS_LIST_CORRECTION { get; set; }
        public List<MANE_IMS_PCI_INDEX> PCI_LIST { get; set; }

        public int MAX_MAIN_ITEM_COUNT { get; set; }

        //added by sachin 13 june 2020 for bridge inspection format change 
        [Display(Name = "Type Of Bearing")]
        [Required(ErrorMessage = "Please select Bearing Type ")]
        public int BearingCode { get; set; }
        public List<SelectListItem> BearingTypeList { get; set; }

        [Display(Name = "Monitor")]
        public string MONITOR_NAME { get; set; }

        [Display(Name = "State")]
        public string STATE_NAME { get; set; }

        [Display(Name = "District")]
        public string DISTRICT_NAME { get; set; }

        [Display(Name = "Month & Year Of Visit")]
        public string SCHEDULE_MONTH_YEAR { get; set; }

        [Display(Name = "Package")]
        public string IMS_PACKAGE_ID { get; set; }

        [Display(Name = "Sanction Year")]
        public string IMS_YEAR { get; set; }

        [Display(Name = "Length (Km.)")]
        public decimal IMS_PAV_LENGTH { get; set; }

        [Display(Name = "Road Name")]
        public string IMS_ROAD_NAME { get; set; }

        [Display(Name = "Bridge Name")]
        public string IMS_BRIDGE_NAME { get; set; }

        [Display(Name = "Length (Mtrs.)")]
        public decimal? IMS_BRIDGE_LENGTH { get; set; }

        [Display(Name = "Start Chainage (Km.)")]
        [Required]
        [RegularExpression(@"^\s*(?=.*[0-9])\d*(?:\.\d{1,3})?\s*$", ErrorMessage = "Start Chainage can only contains Numeric values and 3 digits after decimal place")]
        [Range(0, 9999.999, ErrorMessage = "Invalid Start Chainage")]
        // Commented on 08/ 12/2020 as per suggestion from Pankaj Sir.
      //  [IsValidStartChainage("IMS_PAV_LENGTH", "TO_ROAD_LENGTH", "IMS_PROPOSAL_TYPE", "IMS_ISCOMPLETED", ErrorMessage = "For In-Progress or Completed Road, Start Chainage should be greater than or equal to 0 and less than Road Length. Maximum chainage difference must be 3.000. For Maintenance Road, Start Chainage should be 0.")]
        public decimal FROM_ROAD_LENGTH { get; set; }




        [Display(Name = "End Chainage (Km.)")]
        [Required]
        [RegularExpression(@"^\s*(?=.*[0-9])\d*(?:\.\d{1,3})?\s*$", ErrorMessage = "Invalid End Chainage, can only contains Numeric values and 3 digits after decimal place")]
        [Range(0, 9999.999, ErrorMessage = "Invalid End Chainage")]
        // Commented on 08/ 12/2020 as per suggestion from Pankaj Sir.
      // [IsValidEndChainage("IMS_PAV_LENGTH", "FROM_ROAD_LENGTH", "IMS_PROPOSAL_TYPE", "IMS_ISCOMPLETED", ErrorMessage = "For In-Progress or Completed Road, End Chainage should be greater than 0 and less than or equal to Road Length. Maximum chainage difference must be 3.000. For Maintenance Road, End Chainage should be equal to Road Length.")]
        public decimal TO_ROAD_LENGTH { get; set; }

        [Display(Name = "Road Status")]
        public string IMS_ISCOMPLETED { get; set; }

        [Display(Name = "Completion Date")]
        public string COMPLETION_DATE { get; set; }

        [Display(Name = "Inspection Date")]
        [Required]
        [IsValidInspectionDate("SCHEDULE_MONTH_YEAR_START_DATE", ErrorMessage = "Inspection Date must be of scheduled month's and should not greater than today's date.")]
        public string QM_INSPECTION_DATE { get; set; }

        public string CURRENT_DATE { get; set; }

        public Nullable<int> NO_OF_ITEM { get; set; }
        public Nullable<int> NO_OF_SUB_ITEM { get; set; }
        public Nullable<int> MAST_ITEM_NO_OVERALL_GRADE { get; set; }

        public bool IsLatLongAvailable { get; set; }

        public int MAST_ITEM_NO { get; set; }
        public string MAST_QM_TYPE { get; set; }
        public int MAST_ITEM_CODE { get; set; }
        public int MAST_SUB_ITEM_CODE { get; set; }
        public string MAST_ITEM_NAME { get; set; }
        public string MAST_ITEM_ACTIVE { get; set; }
        public System.DateTime MAST_ITEM_ACTIVATION_DATE { get; set; }
        public Nullable<System.DateTime> MAST_ITEM_DEACTIVATION_DATE { get; set; }
        public string MAST_ITEM_STATUS { get; set; }
        public Nullable<int> MAST_GRADE_CODE { get; set; }

        public string IMS_PROPOSAL_TYPE { get; set; }

        [Display(Name = "Remarks")]
        public string GRADE_REMARKS { get; set; }

        public virtual MASTER_GRADE_TYPE MASTER_GRADE_TYPE { get; set; }
        public virtual ICollection<QUALITY_QM_OBSERVATION_DETAIL> QUALITY_QM_OBSERVATION_DETAIL { get; set; }

        public string IS_ATR_PAGE { get; set; }
    }


   
    public class QMATRDetailsModel
    {
        public List<qm_inspection_list_atr_Result> OBS_ATR_LIST { get; set; }
        public List<qm_inspection_list_atrr_Result> OBSS_ATR_LIST { get; set; }
        public List<QMATRModel> ATR_LIST { get; set; }
        public List<QMObsATRModel> OBS_LIST { get; set; }
        public string ERROR { get; set; }
    }



    #region 2 tier atr vikky
    public class QMATR2TierDetailsModel
    {
        public List<qm_inspection_list_atr_Result> OBS_ATR_LIST { get; set; }
        public List<qm_inspection_list_2_Tier_atrr_Result> OBSS_ATR_LIST { get; set; }
        public List<QMATRModel> ATR_LIST { get; set; }
        public List<QMObsATRModel> OBS_LIST { get; set; }

        public string SQC_OR_PIU { get; set; }
        public string ERROR { get; set; }
    }



    public class QM2TierATRInspdetailsModel
    {
        public List<qm_inspection_list_Against_Road_sqmATR_Result> INSP_LIST { get; set; }

        public List<QMATRINSPModel> INSP_AGAINST_ROAD_LIST { get; set; }

        public string ERROR { get; set; }
    }
    #endregion

    public class QMATRInspdetailsModel
    {
        public List<qm_inspection_list_Against_Road_Result> INSP_LIST { get; set; }

        public List<QMATRINSPModel> INSP_AGAINST_ROAD_LIST { get; set; }

        public string ERROR { get; set; }
    }

    #region FDR tech model vikky
    public class TechDetailsAgainstRoadModel
    {
        public List<USP_FDR_TECH_DETAILS_AGAINST_ROAD_Result> TECH_LIST { get; set; }

        public List<FDR_TECH_DETAILS> TECH_DETAILS_AGAINST_ROAD_LIST { get; set; }

        public string ERROR { get; set; }
    }

    public class FDR_TECH_DETAILS
    {
        public int IMS_PR_ROAD_CODE { get; set; }
        public int MAST_STATE_CODE { get; set; }
        public string STATE_NAME { get; set; }
        public int MAST_DISTRICT_CODE { get; set; }
        public string DISTRICT_NAME { get; set; }
        public int MAST_BLOCK_CODE { get; set; }
        public string BLOCK_NAME { get; set; }
        public string IMS_PACKAGE_ID { get; set; }
        public int IMS_YEAR { get; set; }
        public string IMS_ROAD_NAME { get; set; }
        public string IMS_PROPOSAL_TYPE { get; set; }
        public string PMGSY_SCHEME { get; set; }
        public string TECHNLOGY { get; set; }
        public string ALL_ADDITIVE_NAME { get; set; }
        public string STRETCH_CONSTR_DATE { get; set; }
        public Nullable<decimal> EXEC_LENGTH_STAB_BC { get; set; }

    }



    #endregion

    //public class QMObsATRModel
    //{
    //    public int QM_OBSERVATION_ID { get; set; }
    //    public string MONITOR_NAME { get; set; }
    //    public string STATE_NAME { get; set; }
    //    public string DISTRICT_NAME { get; set; }
    //    public string BLOCK_NAME { get; set; }
    //    public string IMS_PACKAGE_ID { get; set; }
    //    public string IMS_YEAR { get; set; }
    //    public string IMS_ROAD_NAME { get; set; }
    //    public decimal QM_INSPECTED_START_CHAINAGE { get; set; }
    //    public decimal QM_INSPECTED_END_CHAINAGE { get; set; }
    //    public Nullable<decimal> QM_INSPECTED_END_CHAINAGE_BRIDGE { get; set; }
    //    public Nullable<decimal> WORK_LENGTH { get; set; }
    //    public string QM_INSPECTION_DATE { get; set; }
    //    public string IMS_ISCOMPLETED { get; set; }
    //    public string OVERALL_GRADE { get; set; }
    //    public Nullable<int> NO_OF_PHOTO_UPLOADED { get; set; }
    //    public string QM_ATR_STATUS { get; set; }
    //    public string ADMIN_IS_ENQUIRY { get; set; }
    //    public string IMS_PROPOSAL_TYPE { get; set; }
    //    public string SHOW_OBS_LINK { get; set; }
    //    public string PMGSY_SCHEME { get; set; }
    //    public string IMS_ISLABUPLOADED { get; set; }
    //}




    public class QMObsATRModel
    {
        public int QM_OBSERVATION_ID { get; set; }
        public string MONITOR_NAME { get; set; }
        public string STATE_NAME { get; set; }
        public string DISTRICT_NAME { get; set; }
        public string BLOCK_NAME { get; set; }
        public string IMS_PACKAGE_ID { get; set; }
        public string IMS_YEAR { get; set; }
        public string IMS_ROAD_NAME { get; set; }
        public decimal QM_INSPECTED_START_CHAINAGE { get; set; }
        public decimal QM_INSPECTED_END_CHAINAGE { get; set; }
        public Nullable<decimal> QM_INSPECTED_END_CHAINAGE_BRIDGE { get; set; }
        public Nullable<decimal> WORK_LENGTH { get; set; }
        public string QM_INSPECTION_DATE { get; set; }
        public string IMS_ISCOMPLETED { get; set; }
        public string OVERALL_GRADE { get; set; }
        public Nullable<int> NO_OF_PHOTO_UPLOADED { get; set; }
        public string QM_ATR_STATUS { get; set; }
        public string ADMIN_IS_ENQUIRY { get; set; }
        public string IMS_PROPOSAL_TYPE { get; set; }
        public string SHOW_OBS_LINK { get; set; }
        public string PMGSY_SCHEME { get; set; }
        public string IMS_ISLABUPLOADED { get; set; }
        public string VIEW_INSPECTION_REPORT_LINK { get; set; }

        public string EXEC_COMPLETION_DATE { get; set; }

        public string INSP_REPORT { get; set; }
        public string EFORM_PDF_VIEW { get; set; }
        public string EFORM_PDF_PREVIEW { get; set; }

        public int ATR_ELIGIBILITY { get; set; }

        // Added By Chandra Darshan Agrawal
        public string IMS_IS_STAGED { get; set; }
        public string STAGE_PHASE { get; set; }
    }





    public class QMATRModel
    {
        public int QM_OBSERVATION_ID { get; set; }
        public Nullable<int> QM_ATR_ID { get; set; }
        public string ATR_ENTRY_DATE { get; set; }
        public string ATR_REGRADE_STATUS { get; set; }
        public string ATR_REGRADE_REMARKS { get; set; }
        public string ATR_REGRADE_DATE { get; set; }
        public string ATR_IS_DELETED { get; set; }
        public string QM_ATR_STATUS { get; set; }

        public string IS_SUBMITTED { get; set; }
        public string ATR_UPLOAD_VIEW_LINK { get; set; }
        public string ATR_ACCEPTANCE_LINK { get; set; }
        public string ATR_REGRADE_LINK { get; set; }
        public string ATR_DELETE_LINK { get; set; }

        public int ATR_ELIGIBILITY { get; set; }

        //----------------- ADDED BY ROHIT BORSE for ATR MARK VERIFICATION
        public string VERIFICATION_ATR_CODE { get; set; }
    }

    public class QMATRINSPModel
    {
        public int QM_OBSERVATION_ID { get; set; }
        public Nullable<int> ADMIN_SCHEDULE_CODE { get; set; }
        public int IMS_PR_ROAD_CODE { get; set; }
        public int ADMIN_QM_CODE { get; set; }
        public string MONITOR_NAME { get; set; }
        public int MAST_STATE_CODE { get; set; }
        public string STATE_NAME { get; set; }
        public int MAST_DISTRICT_CODE { get; set; }
        public string DISTRICT_NAME { get; set; }
        public int MAST_BLOCK_CODE { get; set; }
        public string BLOCK_NAME { get; set; }
        public string IMS_PACKAGE_ID { get; set; }
        public int IMS_YEAR { get; set; }
        public string IMS_ROAD_NAME { get; set; }
        public decimal QM_INSPECTED_START_CHAINAGE { get; set; }
        public decimal QM_INSPECTED_END_CHAINAGE { get; set; }
        public string QM_INSPECTION_DATE { get; set; }
        public string QM_SCHEDULE_DATE { get; set; }
        public int ADMIN_IM_YEAR { get; set; }
        public int ADMIN_IM_MONTH { get; set; }
        public string QM_OBS_UPLOAD_DATE { get; set; }
        public string IMS_ISCOMPLETED { get; set; }
        public string OVERALL_GRADE { get; set; }
        public Nullable<int> NO_OF_PHOTO_UPLOADED { get; set; }
        public string QM_ATR_STATUS { get; set; }
        public string UPLOAD_BY { get; set; }
        public string ADMIN_IS_ENQUIRY { get; set; }
        public string IMS_PROPOSAL_TYPE { get; set; }
        public string PMGSY_SCHEME { get; set; }
        public string IMS_PAV_LENGTH { get; set; }
        public Nullable<decimal> IMS_BRIDGE_LENGTH { get; set; }
        public string ADMIN_QM_TYPE { get; set; }
        public string ATR_VERIFICATION_FINALIZED { get; set; }

        public string INSPECTION_REPORT_LINK { get; set; }
        public string OBS_LINK { get; set; }

    }

    //QMScheduleViewModel

    public class QMATRRegradeModel
    {
        public int ADMIN_SCHEDULE_CODE { get; set; }
        public int IMS_PR_ROAD_CODE { get; set; }
        public int QM_OBSERVATION_ID { get; set; }
        public string PACKAGE_ID { get; set; }
        public string ROAD_NAME { get; set; }
        public string StateName { get; set; }
        public string DistrictName { get; set; }
        public string BlockName { get; set; }


        public string ATR_REGRADE_STATUS { get; set; }
        public DateTime ATR_REGRADE_DATE { get; set; }
        //[a-zA-Z0-9!-?_.&,/()@!$%^&*]
        [Display(Name = "Remarks")]
        [RegularExpression("[a-zA-Z0-9- _.@?;$*&,/():'+#%!=]{1,500}", ErrorMessage = "Invalid Remarks")]
        [Required(ErrorMessage = "Please enter Remarks.")]
        //[CheckATRRegradeWiseRejectValidation("ATR_REGRADE_STATUS", ErrorMessage = "Please enter Remarks.")]
        public string ATR_REGRADE_REMARKS { get; set; }


        [Display(Name = "Reasons")]
        // [Range(1, int.MaxValue, ErrorMessage = "Please select a valid reason")]
        public Nullable<int> reasonCode { get; set; }
        public List<SelectListItem> lstReasons { set; get; }
    }









    public class QMLetterModel
    {
        public string FILE_NAME { get; set; }
        public int QC_CODE { get; set; }
        public string QC_TYPE { get; set; }
        public int INSP_MONTH { get; set; }
        public int INSP_YEAR { get; set; }
        public string MONTH_TEXT { get; set; }
        public string YEAR_TEXT { get; set; }
        public int SQC_STATE_CODE { get; set; }
        public int SCHEDULE_CODE { get; set; }
        public int LETTER_ID { get; set; }
    }

    public class QMUnsatisfactoryWorkModel
    {
        public List<USP_QM_UNSATISFACTORY_WORKS_FOR_STATE_Result> ROAD_OBS_LIST { get; set; }
        public List<QMUnsatisfactoryRoadModel> ROAD_LIST { get; set; }
        public List<QMUnsatisfactoryObsModel> OBS_LIST { get; set; }
        public string ERROR { get; set; }

        public string STATE_NAME { get; set; }
        public string QC_NAME { get; set; }
        public string QC_PHONE { get; set; }
        public string CEO_NAME { get; set; }
        public string CEO_PHONE { get; set; }
    }

    public class QMUnsatisfactoryRoadModel
    {
        public int IMS_PR_ROAD_CODE { get; set; }
        public int MAST_STATE_CODE { get; set; }
        public string STATE_NAME { get; set; }
        public int MAST_DISTRICT_CODE { get; set; }
        public string DISTRICT_NAME { get; set; }
        public int MAST_BLOCK_CODE { get; set; }
        public string BLOCK_NAME { get; set; }
        public string IMS_PACKAGE_ID { get; set; }
        public string IMS_YEAR { get; set; }
        public string IMS_ROAD_NAME { get; set; }
        public string IMS_PROPOSAL_TYPE { get; set; }
    }


    public class QMUnsatisfactoryObsModel
    {
        public int IMS_PR_ROAD_CODE { get; set; }
        public int QM_OBSERVATION_ID { get; set; }
        public Nullable<int> ADMIN_SCHEDULE_CODE { get; set; }
        public int ADMIN_QM_CODE { get; set; }
        public string ADMIN_QM_TYPE { get; set; }
        public string MONITOR_NAME { get; set; }
        public string QM_INSPECTION_DATE { get; set; }
        public string IMS_ISCOMPLETED { get; set; }
        public string OVERALL_GRADE { get; set; }
        public string OBS_LINK { get; set; }
    }


    public class QMCommencedWorkModel
    {
        public List<USP_QM_COMMENCED_WORKS_Result> WORK_LIST { get; set; }
        public List<USP_QM_COMMENCED_INSP_DETAILS_Result> INSP_LIST { get; set; }
        public List<USP_QM_COMMENCED_WORKS_DETAILS_Result> COMMENCEMENT_LIST { get; set; }
        public string STATE_NAME { get; set; }
        public string DURATION { get; set; }
        public string ERROR { get; set; }
    }

    public class QMCompletedWorkModel
    {
        public List<USP_QM_COMPLETED_WORKS_Result> WORK_LIST { get; set; }
        public List<USP_QM_COMPLETED_INSP_DETAILS_Result> INSP_LIST { get; set; }
        public string WORK_NAME { get; set; }

        [Display(Name = "From Date")]
        public string FROM_DATE { get; set; }
        [Display(Name = "To Date")]
        public string TO_DATE { get; set; }
        public string ERROR { get; set; }
    }

    /// <summary>
    /// Check for valid start chainage
    /// </summary>
    public class IsValidStartChainage : ValidationAttribute, IClientValidatable
    {
        private readonly string PropertyName;
        private readonly string EndChainage;
        private readonly string ProposalType;
        private readonly string RoadStatus;

        public IsValidStartChainage(string propertyName, string endChainage, string propType, string roadStatus)
        {
            this.PropertyName = propertyName;
            this.EndChainage = endChainage;
            this.ProposalType = propType;
            this.RoadStatus = roadStatus;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var propertyTestedInfo = validationContext.ObjectType.GetProperty(this.PropertyName);
            var endChainageInfo = validationContext.ObjectType.GetProperty(this.EndChainage);
            var proposalTypeInfo = validationContext.ObjectType.GetProperty(this.ProposalType);
            var roadStatusInfo = validationContext.ObjectType.GetProperty(this.RoadStatus);
            if (propertyTestedInfo == null)
            {
                return new ValidationResult(string.Format("unknown property {0}", this.PropertyName));
            }

            var roadlength = propertyTestedInfo.GetValue(validationContext.ObjectInstance, null);
            var EndChaiangeVal = endChainageInfo.GetValue(validationContext.ObjectInstance, null);
            var ProposalTypeVal = proposalTypeInfo.GetValue(validationContext.ObjectInstance, null);
            var roadStatusVal = roadStatusInfo.GetValue(validationContext.ObjectInstance, null);
            decimal Difference = Convert.ToDecimal(EndChaiangeVal) - Convert.ToDecimal(value);
            if (ProposalTypeVal.Equals("P"))    //Check only for Road (Not for Bridge)
            {
                if (roadStatusVal.Equals("M"))
                {
                    if (Convert.ToDecimal(value) == 0)
                    {
                        return ValidationResult.Success;
                    }
                }
                else if (Convert.ToDecimal(value) >= 0 && (Convert.ToDecimal(value) < Convert.ToDecimal(roadlength)) && Difference <= Convert.ToDecimal(3.00) && Convert.ToDecimal(value) < Convert.ToDecimal(EndChaiangeVal))
                {
                    return ValidationResult.Success;
                }
            }
            else
            {
                return ValidationResult.Success;
            }

            return new ValidationResult(FormatErrorMessage(validationContext.DisplayName));
        }

        public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
        {
            var rule = new ModelClientValidationRule
            {
                ErrorMessage = this.ErrorMessageString,
                ValidationType = "isvalidstartchainage"
            };
            rule.ValidationParameters["pavlength"] = this.PropertyName;
            yield return rule;
        }
    }

    
    /// <summary>
    /// Check for valid end chainage
    /// </summary>
    public class IsValidEndChainage : ValidationAttribute, IClientValidatable
    {
        private readonly string PropertyName;
        private readonly string StartChainage;
        private readonly string ProposalType;
        private readonly string RoadStatus;

        public IsValidEndChainage(string propertyName, string startChainage, string propType, string roadStatus)
        {
            this.PropertyName = propertyName;
            this.StartChainage = startChainage;
            this.ProposalType = propType;
            this.RoadStatus = roadStatus;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var propertyTestedInfo = validationContext.ObjectType.GetProperty(this.PropertyName);
            var startChainageInfo = validationContext.ObjectType.GetProperty(this.StartChainage);
            var proposalTypeInfo = validationContext.ObjectType.GetProperty(this.ProposalType);
            var roadStatusInfo = validationContext.ObjectType.GetProperty(this.RoadStatus);
            if (propertyTestedInfo == null)
            {
                return new ValidationResult(string.Format("unknown property {0}", this.PropertyName));
            }

            var roadlength = propertyTestedInfo.GetValue(validationContext.ObjectInstance, null);
            var StartChaiangeVal = startChainageInfo.GetValue(validationContext.ObjectInstance, null);
            var ProposalTypeVal = proposalTypeInfo.GetValue(validationContext.ObjectInstance, null);
            var roadStatusVal = roadStatusInfo.GetValue(validationContext.ObjectInstance, null);
            decimal Difference = Convert.ToDecimal(value) - Convert.ToDecimal(StartChaiangeVal);

            if (ProposalTypeVal.Equals("P"))    //Check only for Road (Not for Bridge)
            {
                if (roadStatusVal.Equals("M"))
                {
                    if (Convert.ToDecimal(value) == Convert.ToDecimal(roadlength))
                    {
                        return ValidationResult.Success;
                    }
                }
                else if (Convert.ToDecimal(value) > 0 && (Convert.ToDecimal(value) <= Convert.ToDecimal(roadlength)) && Difference <= Convert.ToDecimal(3.00) && Convert.ToDecimal(StartChaiangeVal) < Convert.ToDecimal(value))
                {
                    return ValidationResult.Success;
                }
            }
            else
            {
                return ValidationResult.Success;
            }

            return new ValidationResult(FormatErrorMessage(validationContext.DisplayName));
        }

        public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
        {
            var rule = new ModelClientValidationRule
            {
                ErrorMessage = this.ErrorMessageString,
                ValidationType = "isvalidendchainage"
            };
            rule.ValidationParameters["pavlength"] = this.PropertyName;
            yield return rule;
        }
    }



    public class IsValidInspectionDate : ValidationAttribute, IClientValidatable
    {
        // private const string _defaultErrorMessage = "Start date must be less than end date.";
        private string _scheduleDate;
        CommonFunctions objCommonFun = new CommonFunctions();
        public IsValidInspectionDate(string scheduleDate)
        {
            _scheduleDate = scheduleDate;
        }

        //Override IsValid  
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            //Get PropertyInfo Object  
            var scheduleDatePropertyInfo = validationContext.ObjectType.GetProperty(_scheduleDate);
            var startDate = scheduleDatePropertyInfo.GetValue(validationContext.ObjectInstance, null);
            var endDate = ConvertStringToDate(DateTime.Now.ToString("dd/MM/yyyy"));
            var inspDate = value;

            if (startDate != null && endDate != null && inspDate != null)
            {
                var scheduleDate = objCommonFun.GetStringToDateTime(startDate.ToString());
                var inspectionDate = objCommonFun.GetStringToDateTime(inspDate.ToString());

                //Actual comparision  
                if (inspectionDate < scheduleDate || inspectionDate > endDate)
                {
                    var message = FormatErrorMessage(validationContext.DisplayName);
                    return new ValidationResult(message);
                }
            }

            //Default return - This means there were no validation error  
            return ValidationResult.Success;
        }


        public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
        {
            var rule = new ModelClientValidationRule
            {
                ErrorMessage = this.ErrorMessageString,
                ValidationType = "isvalidinspdate"
            };
            rule.ValidationParameters["scheduledate"] = this._scheduleDate;
            yield return rule;

        }


        public DateTime? ConvertStringToDate(string dateToConvert)
        {
            DateTime MyDateTime;
            MyDateTime = new DateTime();
            MyDateTime = DateTime.ParseExact(dateToConvert, "dd/MM/yyyy", null);
            return MyDateTime;
        }
    }


    public class CheckATRRegradeWiseRejectValidationAttribute : ValidationAttribute, IClientValidatable
    {
        // private const string _defaultErrorMessage = "Start date must be less than end date.";
        private string _basePropertyName;

        public CheckATRRegradeWiseRejectValidationAttribute(string basePropertyName)
        //: base(_defaultErrorMessage)
        {
            _basePropertyName = basePropertyName;
        }

        //Override default FormatErrorMessage Method  
        public override string FormatErrorMessage(string name)
        {
            return string.Format(ErrorMessageString, name, _basePropertyName);
        }

        //Override IsValid  
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            //Get PropertyInfo Object  
            var basePropertyInfo = validationContext.ObjectType.GetProperty(_basePropertyName);

            var IsRejectedFlag = basePropertyInfo.GetValue(validationContext.ObjectInstance, null); // check value is null or empty
            string RejectedText = (string)value;

            //if (sDate != null && eDate != null)
            //{

            //Actual Validation 
            if (IsRejectedFlag == "R" && (string.IsNullOrEmpty(RejectedText)))
            {
                var message = FormatErrorMessage(validationContext.DisplayName);
                return new ValidationResult(message);
            }

            return ValidationResult.Success;
        }


        public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
        {

            var rule = new ModelClientValidationRule
            {
                ErrorMessage = FormatErrorMessage(metadata.DisplayName),
                ValidationType = "checkatrregradewiserejectvalidationattribute"
            };
            //rule.ValidationParameters["date"] = this._basePropertyName;
            rule.ValidationParameters.Add("previousval", this._basePropertyName);

            //yield return rule;
            return new[] { rule };

        }

    }

    #region Technical Expert Review
    public class TEQMFillObservationModel
    {
        public TEQMFillObservationModel()
        {
            CommonFunctions objCommon = new CommonFunctions();
            this.CURRENT_DATE = objCommon.GetDateTimeToString(DateTime.Now);
        }

        public int LSBOverallCorrectedGrade { get; set; }

        public int ADMIN_SCHEDULE_CODE { get; set; }
        public int IMS_PR_ROAD_CODE { get; set; }
        public int QM_OBSERVATION_ID { get; set; }
        public bool IS_3RD_TIER_SQC { get; set; }
        public string SCHEDULE_MONTH_YEAR_START_DATE { get; set; }
        public string SCHEDULE_MONTH_YEAR_END_DATE { get; set; }// Added by deendayal on 15/9/2017 to restrict the monitor to select any other month's day

        public int MAST_PCI_GRADE { get; set; }

        public List<TE_QM_OBSERVATION_GRADING_DETAIL> GRADE_DETAILS_LIST { get; set; }

        public List<MANE_IMS_PCI_INDEX> PCI_LIST { get; set; }

        public int MAX_MAIN_ITEM_COUNT { get; set; }

        //added by sachin 13 june 2020 for bridge inspection format change 
        [Display(Name = "Type Of Bearing")]
        [Required(ErrorMessage = "Please select Bearing Type ")]
        public int BearingCode { get; set; }
        public List<SelectListItem> BearingTypeList { get; set; }

        [Display(Name = "Monitor")]
        public string MONITOR_NAME { get; set; }

        [Display(Name = "State")]
        public string STATE_NAME { get; set; }

        [Display(Name = "District")]
        public string DISTRICT_NAME { get; set; }

        [Display(Name = "Month & Year Of Visit")]
        public string SCHEDULE_MONTH_YEAR { get; set; }

        [Display(Name = "Package")]
        public string IMS_PACKAGE_ID { get; set; }

        [Display(Name = "Sanction Year")]
        public string IMS_YEAR { get; set; }

        [Display(Name = "Length (Km.)")]
        public decimal IMS_PAV_LENGTH { get; set; }

        [Display(Name = "Road Name")]
        public string IMS_ROAD_NAME { get; set; }

        [Display(Name = "Bridge Name")]
        public string IMS_BRIDGE_NAME { get; set; }

        [Display(Name = "Length (Mtrs.)")]
        public decimal? IMS_BRIDGE_LENGTH { get; set; }

        [Display(Name = "Start Chainage (Km.)")]
        [Required]
        [RegularExpression(@"^\s*(?=.*[0-9])\d*(?:\.\d{1,3})?\s*$", ErrorMessage = "Start Chainage can only contains Numeric values and 3 digits after decimal place")]
        [Range(0, 9999.999, ErrorMessage = "Invalid Start Chainage")]
        // Commented on 08/ 12/2020 as per suggestion from Pankaj Sir.
        //  [IsValidStartChainage("IMS_PAV_LENGTH", "TO_ROAD_LENGTH", "IMS_PROPOSAL_TYPE", "IMS_ISCOMPLETED", ErrorMessage = "For In-Progress or Completed Road, Start Chainage should be greater than or equal to 0 and less than Road Length. Maximum chainage difference must be 3.000. For Maintenance Road, Start Chainage should be 0.")]
        public decimal FROM_ROAD_LENGTH { get; set; }




        [Display(Name = "End Chainage (Km.)")]
        [Required]
        [RegularExpression(@"^\s*(?=.*[0-9])\d*(?:\.\d{1,3})?\s*$", ErrorMessage = "Invalid End Chainage, can only contains Numeric values and 3 digits after decimal place")]
        [Range(0, 9999.999, ErrorMessage = "Invalid End Chainage")]
        // Commented on 08/ 12/2020 as per suggestion from Pankaj Sir.
        // [IsValidEndChainage("IMS_PAV_LENGTH", "FROM_ROAD_LENGTH", "IMS_PROPOSAL_TYPE", "IMS_ISCOMPLETED", ErrorMessage = "For In-Progress or Completed Road, End Chainage should be greater than 0 and less than or equal to Road Length. Maximum chainage difference must be 3.000. For Maintenance Road, End Chainage should be equal to Road Length.")]
        public decimal TO_ROAD_LENGTH { get; set; }

        [Display(Name = "Road Status")]
        public string IMS_ISCOMPLETED { get; set; }

        [Display(Name = "Completion Date")]
        public string COMPLETION_DATE { get; set; }

        [Display(Name = "Inspection Date")]
        [Required]
        [IsValidInspectionDate("SCHEDULE_MONTH_YEAR_START_DATE", ErrorMessage = "Inspection Date must be of scheduled month's and should not greater than today's date.")]
        public string QM_INSPECTION_DATE { get; set; }

        public string CURRENT_DATE { get; set; }

        public Nullable<int> NO_OF_ITEM { get; set; }
        public Nullable<int> NO_OF_SUB_ITEM { get; set; }
        public Nullable<int> MAST_ITEM_NO_OVERALL_GRADE { get; set; }

        public bool IsLatLongAvailable { get; set; }

        public int MAST_ITEM_NO { get; set; }
        public string MAST_QM_TYPE { get; set; }
        public int MAST_ITEM_CODE { get; set; }
        public int MAST_SUB_ITEM_CODE { get; set; }
        public string MAST_ITEM_NAME { get; set; }
        public string MAST_ITEM_ACTIVE { get; set; }
        public System.DateTime MAST_ITEM_ACTIVATION_DATE { get; set; }
        public Nullable<System.DateTime> MAST_ITEM_DEACTIVATION_DATE { get; set; }
        public string MAST_ITEM_STATUS { get; set; }
        public Nullable<int> MAST_GRADE_CODE { get; set; }

        public string IMS_PROPOSAL_TYPE { get; set; }

        [Display(Name = "Remarks")]
        public string GRADE_REMARKS { get; set; }

        public string IS_ATR_PAGE { get; set; }

        [StringLength(255, ErrorMessage = "Maximum 255 characters are allowed")]
        [RegularExpression(@"^[a-zA-Z0-9-/. ]+$", ErrorMessage = "Only Alphanumeric, Space and '-','/','.' Allowed")]
        [Required(ErrorMessage = "Please enter remark.")]
        public string GENERALREMARKS { get; set; }
    }

    public class TE_QM_OBSERVATION_GRADING_DETAIL
    {
        public int MAST_ITEM_NO { get; set; }
        public int MAST_ITEM_CODE { get; set; }
        public int MAST_SUB_ITEM_CODE { get; set; }
        public string MAST_ITEM_NAME { get; set; }
        public Nullable<int> MAST_GRADE_CODE { get; set; }
        public Nullable<int> ASSIGNED_GRADE_CODE { get; set; }
        public string MAST_GRADE_NAME { get; set; }
        public Nullable<int> NO_OF_ITEM { get; set; }
        public Nullable<int> NO_OF_SUB_ITEM { get; set; }
        public string REMARKS { get; set; }

        [StringLength(255, ErrorMessage = "Maximum 255 characters are allowed")]
        [RegularExpression(@"^[a-zA-Z0-9-/. ]+$", ErrorMessage = "Only Alphanumeric, Space and '-','/','.' Allowed")]
        public string TEREMARKS { get; set; }

        [StringLength(255, ErrorMessage = "Maximum 255 characters are allowed")]
        [RegularExpression(@"^[a-zA-Z0-9-/. ]+$", ErrorMessage = "Only Alphanumeric, Space and '-','/','.' Allowed")]
        [Required(ErrorMessage = "Please enter remark.")]
        public string NQMREMARKS { get; set; }
    }
    #endregion
}