using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PMGSY.Areas.PMIS.Models
{
    public class ListPMISRoadDetailsViewModel
    {
        [Display(Name = "State")]
        [Required(ErrorMessage = "Please Select State")]
        [RegularExpression(@"^([0-9]+)$", ErrorMessage = "Please select State.")]
        public int StateCode { get; set; }
        public List<SelectListItem> StateList { get; set; }


        [Display(Name = "District")]
        [Required(ErrorMessage = "Please Select District")]
        [RegularExpression(@"^([0-9]+)$", ErrorMessage = "Please select District.")]
        public int DistrictCode { get; set; }
        public List<SelectListItem> DistrictList { get; set; }

        [Display(Name = "Block")]
        [Required(ErrorMessage = "Please Select Block")]
        [RegularExpression(@"^([0-9]+)$", ErrorMessage = "Please select Block.")]
        public int BlockCode { get; set; }
        public List<SelectListItem> BlockList { get; set; }

        [Display(Name = "Batch")]
        [Range(0, 10, ErrorMessage = "Please select Batch.")]
        [Required(ErrorMessage = "Please select Batch.")]
        [RegularExpression("^([0-9]+)$", ErrorMessage = "Batch must be valid number.")]
        public int Batch { get; set; }
        public List<SelectListItem> BatchList { get; set; }

        [Display(Name = "Year")]
        [Range(0, int.MaxValue, ErrorMessage = "Please select valid Year.")]
        public int Sanction_Year { get; set; }
        public List<SelectListItem> Sanction_Year_List { get; set; }

    }

    public class PMISRoadDAL
    {
        public int PlanId;
        public string StateName;
        public string DistrictName;
        public string BlockName;
        public string BatchName;
        public string SanctionYear;
        public string SanctionDate;
        public string PackageName;
        public string SanctionLength;
        public string AgreementNo;
        public string AgreementCost;
        public string MordShare;
        public string StateShare;
        public string TotalSanctionedCost;
        public string AgreementStartDate;
        public string AgreementEndDate;
        public string RoadName;
        //Change By Hrishikesh PMIS To Add Technology Name in Grid -12-06-2023
        public string TECHNOLOGY_NAME;
        public string IMS_PR_RoadCode;
        public string IsPlanAvaliable;
        public string IsFinalize;
        public string IsRevisePlan;
        public string IsActualsAvaliable;
        // ON 01-01-2022
        public string ProgressStatus;
        public string ActualLock;
        public string IsFinalized;
        // ADDED FOR PMISDAL

        // ADDED BY ROHIT BORSE TO TO GET COUNT FOR LOCK_STATUS on 19-05-2023
        public bool Is_roadProgress_Entered { get; set; }
        public bool Is_roadQCR_Entered_andFinalize { get; set; }
        public bool IsWork_Freeze_RoadCodeAvailable { get; set; }
        public bool IsGPSVTS_Installed_OnWork { get; set; }

        //Added By Rohit Borse on 26-08-2023 for GPS VTS Freeze/Unfreeze Validation
        public bool IsVTSWorkUnfreeze { get; set; }

        //Added By Hrishikesh To Add "Trail Strech For FDR" --05-06-2023
        public bool IsFdrTechUsed { get; set; }
        public bool isFDR_SAMI_UPLOADED { get; set; }
        public bool isTrialStretchUploadedFinalized { get; set; }
        public bool IsFDRFilled { get; set; }

    }

    public class PMISRoadDALDetails
    {
        public string StateName;
        public string DistrictName;
        public string BlockName;
        public string BatchName;
        public string SanctionYear;
        public string SanctionDate;
        public string PackageName;
        public string SanctionLength;
        public string AgreementNo;
        public string AgreementCost;
        public string MordShare;
        public string StateShare;
        public string TotalSanctionedCost;
        public string AgreementStartDate;
        public string AgreementEndDate;
        public string RoadName;
        public string IMS_PR_RoadCode;
        public string IsPlanAvaliable;
        public string IsFinalize;
        public string IsRevisePlan;
        public string IsActualsAvaliable;

      
        public string PlanId;
    }

    public class AddPlanPMISViewModel
    {
        public string IMS_ROAD_NAME { get; set; }
        public int IMS_PR_ROAD_CODE { get; set; }    
        public int SORT_ORDER { get; set; }  // Trial Stretch Change
        public decimal IMS_PAV_LENGTH { get; set; }//SanctionedLength
        public string IMS_YEAR { get; set; }        //SanctionedYear

        public string StateName { get; set; }
        public string DistrictName { get; set; }

        public string IMS_PACKAGE_ID { get; set; }
        public int MAST_DISTRICT_CODE { get; set; }
        public Nullable<decimal> AGREEMENT_VALUE { get; set; }
        public string ROAD_TYPE { get; set; }
        public string StateShare { get; set; }
        public string MordShare { get; set; }
        public string TotalSanctionedCost { get; set; }

        public string[] Activity_Desc_List { get; set; }
        public string[] Activity_Unit_List { get; set; }
        public string ACTIVITY_DESC { get; set; }
        public string ACTIVITY_UNIT { get; set; }
        public int PLAN_ID { get; set; }
        public int ACTIVITY_ID { get; set; }
        public Nullable<decimal> QUANTITY { get; set; }
        [RegularExpression(@"^\s*(?=.*[0-9])\d*(?:\.\d{1,2})?\s*$", ErrorMessage = "Cost,Can only contains Numeric values and 2 digits after decimal place")]
        //[Range(0, 9999999999.99, ErrorMessage = "Invalid Agreement Cost.")]
        public Nullable<decimal> AGREEMENT_COST { get; set; }
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        //[DataType(DataType.Date)]
        public Nullable<DateTime> PLANNED_START_DATE { get; set; }
        public string View_Planned_Start_Date { get; set; }
        public Nullable<int> PLANNED_DURATION { get; set; }
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        // [DataType(DataType.Date)]
        public Nullable<DateTime> PLANNED_COMPLETION_DATE { get; set; }
        public string View_Planned_Completion_Date { get; set; }

        public string[] QUANTITY_APPL { get; set; }
        public string[] AGRCOST_APPL { get; set; }
        public string[] PLANNED_START_DATE_APPL { get; set; }
        public string[] PLANNED_DURATION_APPL { get; set; }
        public string[] PLANNED_COMPLETION_DATE_APPL { get; set; }

        public string QUANTITY_APPL_U { get; set; }
        public string AGRCOST_APPL_U { get; set; }
        public string PLANNED_START_DATE_APPL_U { get; set; }
        public string PLANNED_DURATION_APPL_U { get; set; }
        public string PLANNED_COMPLETION_DATE_APPL_U { get; set; }

        public Nullable<decimal> TotalAgreementCost { get; set; }
        [DataType(DataType.Date)]
        public Nullable<DateTime> TotalPlannedStartDate { get; set; }
        public Nullable<int> TotalPlannedDuration { get; set; }
        [DataType(DataType.Date)]
        public Nullable<DateTime> TotalPlannedCompletion { get; set; }

        public string IS_LATEST { get; set; }
        public Nullable<int> BASELINE_NO { get; set; }
        public string IS_FINALISED { get; set; }
    }

    public class ListPMISBridgeDetailsViewModel
    {
        [Display(Name = "State")]
        [Required(ErrorMessage = "Please Select State")]
        [RegularExpression(@"^([0-9]+)$", ErrorMessage = "Please select State.")]
        public int StateCode { get; set; }
        public List<SelectListItem> StateList { get; set; }


        [Display(Name = "District")]
        [Required(ErrorMessage = "Please Select District")]
        [RegularExpression(@"^([0-9]+)$", ErrorMessage = "Please select District.")]
        public int DistrictCode { get; set; }
        public List<SelectListItem> DistrictList { get; set; }

        [Display(Name = "Block")]
        [Required(ErrorMessage = "Please Select Block")]
        [RegularExpression(@"^([0-9]+)$", ErrorMessage = "Please select Block.")]
        public int BlockCode { get; set; }
        public List<SelectListItem> BlockList { get; set; }

        [Display(Name = "Batch")]
        [Range(0, 10, ErrorMessage = "Please select Batch.")]
        [Required(ErrorMessage = "Please select Batch.")]
        [RegularExpression("^([0-9]+)$", ErrorMessage = "Batch must be valid number.")]
        public int Batch { get; set; }
        public List<SelectListItem> BatchList { get; set; }

        [Display(Name = "Year")]
        [Range(0, int.MaxValue, ErrorMessage = "Please select valid Year.")]
        public int Sanction_Year { get; set; }
        public List<SelectListItem> Sanction_Year_List { get; set; }

    }

    public class PMISBridgeDAL
    {
        public int PlanId;
        public string StateName;
        public string DistrictName;
        public string BlockName;
        public string BatchName;
        public string SanctionYear;
        public string SanctionDate;
        public string PackageName;
        public string SanctionLength;
        public string AgreementNo;
        public string AgreementCost;
        public string MordShare;
        public string StateShare;
        public string TotalSanctionedCost;
        public string AgreementStartDate;
        public string AgreementEndDate;
        public string LSBName;
        public string IMS_PR_RoadCode;
        public string IsPlanAvaliable;
        public string IsFinalize;
        public string IsRevisePlan;
        public string IsActualsAvaliable;
        public string ProgressStatus;
        public string ActualLock;
        public string IsFinalized;

        // ADDED BY ROHIT BORSE TO TO GET COUNT FOR LOCK_STATUS on 19-05-2023
        public bool Is_bridgeProgress_Entered { get; set; }
        public bool Is_bridgeQCR_Entered_andFinalize { get; set; }
        
    }

    public class AddPlanPMISViewModelBridge
    {
        public string IMS_BRIDGE_NAME { get; set; }
        public int IMS_PR_ROAD_CODE { get; set; }

         public string ReviseStatus { get; set; }
        
        public decimal IMS_BRIDGE_LENGTH { get; set; }//SanctionedLength
        public string IMS_YEAR { get; set; }        //SanctionedYear

        public string StateName { get; set; }
        public string DistrictName { get; set; }

        public string IMS_PACKAGE_ID { get; set; }
        public int MAST_DISTRICT_CODE { get; set; }
        public Nullable<decimal> AGREEMENT_VALUE { get; set; }
        public string ROAD_TYPE { get; set; }
        public string StateShare { get; set; }
        public string MordShare { get; set; }
        public string TotalSanctionedCost { get; set; }

        public string[] Activity_Desc_List { get; set; }
        public string[] Activity_Unit_List { get; set; }
        public string ACTIVITY_DESC { get; set; }
        public string ACTIVITY_UNIT { get; set; }
        public int PLAN_ID { get; set; }
        public int ACTIVITY_ID { get; set; }
        public Nullable<decimal> QUANTITY { get; set; }
        [RegularExpression(@"^\s*(?=.*[0-9])\d*(?:\.\d{1,2})?\s*$", ErrorMessage = "Cost,Can only contains Numeric values and 2 digits after decimal place")]
        //[Range(0, 9999999999.99, ErrorMessage = "Invalid Agreement Cost.")]
        public Nullable<decimal> AGREEMENT_COST { get; set; }
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        //[DataType(DataType.Date)]
        public Nullable<DateTime> PLANNED_START_DATE { get; set; }
        public string View_Planned_Start_Date { get; set; }
        public Nullable<int> PLANNED_DURATION { get; set; }
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        // [DataType(DataType.Date)]
        public Nullable<DateTime> PLANNED_COMPLETION_DATE { get; set; }
        public string View_Planned_Completion_Date { get; set; }

        public string[] QUANTITY_APPL { get; set; }
        public string[] AGRCOST_APPL { get; set; }
        public string[] PLANNED_START_DATE_APPL { get; set; }
        public string[] PLANNED_DURATION_APPL { get; set; }
        public string[] PLANNED_COMPLETION_DATE_APPL { get; set; }

        public string QUANTITY_APPL_U { get; set; }
        public string AGRCOST_APPL_U { get; set; }
        public string PLANNED_START_DATE_APPL_U { get; set; }
        public string PLANNED_DURATION_APPL_U { get; set; }
        public string PLANNED_COMPLETION_DATE_APPL_U { get; set; }

        public Nullable<decimal> TotalAgreementCost { get; set; }
        [DataType(DataType.Date)]
        public Nullable<DateTime> TotalPlannedStartDate { get; set; }
        public Nullable<int> TotalPlannedDuration { get; set; }
        [DataType(DataType.Date)]
        public Nullable<DateTime> TotalPlannedCompletion { get; set; }

        public string IS_LATEST { get; set; }
        public Nullable<int> BASELINE_NO { get; set; }
        public string IS_FINALISED { get; set; }
    }


}
