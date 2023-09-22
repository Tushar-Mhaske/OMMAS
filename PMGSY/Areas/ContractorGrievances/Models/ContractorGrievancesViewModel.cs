using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PMGSY.Areas.ContractorGrievances.Models
{
    public class ContractorGrievancesViewModel
    {
        public int REGISTRATION_ID {get; set;}
        public string CON_FIRM_NAME {get; set;}
        public string EMAIL_ID { get; set; }
        public string MOBILE_NO { get; set; }
        public int USER_ID {get; set;}

        [Display(Name = "Bank Name")]
        [Required(ErrorMessage = "Bank Name is required")]
        [RegularExpression(@"^([a-zA-Z ,_()&-;'.]+)$", ErrorMessage = "Bank Name is not in valid format.")]
        [StringLength(100, ErrorMessage = "Bank Name must be less than 100 characters.")]
        public string BANK_NAME { get; set; }
        public List<SelectListItem> lstBankNames { get; set; }

        [Display(Name = "Branch Name")]
        [Required(ErrorMessage = "Branch Name is required")]
        [RegularExpression(@"^([a-zA-Z ,_()&-;'.]+)$", ErrorMessage = "Branch Name is not in valid format.")]
        [StringLength(100, ErrorMessage = "Branch Name must be less than 100 characters.")]
        public string BRANCH_NAME { get; set; }

        [Display(Name = "IFSC Code")]
        [RegularExpression(@"^([A-Z|a-z]{4}[0][A-Z|a-z|0-9]{6})$", ErrorMessage = "IFSC Code is not in valid format.")]
        [StringLength(11, ErrorMessage = "IFSC Code must be 11 characters only.")]
        public string IFSC_CODE { get; set; }

        [Display(Name = "Account Number")]
        [Required(ErrorMessage = "Account Number is required")]
        [RegularExpression("^([0-9]{5,17})$", ErrorMessage = "Account Number must be minimum 5 digits amd maximum 17 digits only.")]
        [StringLength(17, ErrorMessage = "Account Number must be 17 digits only.")]
        public string BANK_ACCOUNT_NO { get; set; }

        public string PAN_NO { get; set; }
    }

    public class RegisterGrievanceViewModel
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

        [Display(Name = "Agreement Year")]
        [Range(0, int.MaxValue, ErrorMessage = "Please select valid Year.")]
        public int Agreement_Year { get; set; }
        public List<SelectListItem> Agreement_Year_List { get; set; }
    }

    public class AgreementDetailsModel
    {
        public int Detail_Id { get; set; }
        public int Agreement_Code { get; set; }
        public string Agreement_Number { get; set; }
        public string State { get; set; }
        public int StateCode { get; set; }
        public string District { get; set; }
        public int DistrictCode { get; set; }
        [DataType(DataType.Date)]
        public DateTime Agreement_Date { get; set; }
        public decimal Agreement_Amount { get; set; }
        public bool Feedback_Complaint { get; set; }

        [Required(ErrorMessage = "Grievance Type required")]
        [Range(1, 3, ErrorMessage = "Please select a valid Grievance Type")]
        public int Grievance_Type { get; set; }
        public List<SelectListItem> Grievance_Type_List { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Please select a valid Grievance Sub-Type")]
        [Required(ErrorMessage = "Grievance Sub-Type required")]
        public int Grievance_SubType { get; set; }
        public List<SelectListItem> Grievance_SubType_List { get; set; }

        [Display(Name = "Grievance Description")]
        [Required(ErrorMessage = "Enter Grievance Description")]
        [MaxLength(250, ErrorMessage = "Remarks can be atmost 250 characters long")]
        [StringLength(250, ErrorMessage = "Maximum 255 Characters Allowed")]
        [RegularExpression(@"^[a-zA-Z0-9-/. ]+$", ErrorMessage = "Only Alphanumeric, Space and '-','/','.' Allowed")]
        public string Grievance_Description { get; set; }

        public string Package_Number { get; set; }
        public string Road_Name { get; set; }
        public int IMS_PR_ROAD_CODE { get; set; }
        public HttpPostedFileBase[] files { get; set; }
        public int Tend_Agreement_Code { get; set; }
    }

    public class GrievanceDetailsModel
    {
        public int DETAIL_ID { get; set; }
        public string Grievance_ID { get; set; }
        [DataType(DataType.Date)]
        public DateTime? Grievance_SubmittedOn { get; set; }
        public string Agreement_Number { get; set; }
        public string Grievance_Type { get; set; }
        public string Grievance_Category { get; set; }
        public string Grievance_Status { get; set; }
        public string Is_Finalized { get; set; }
        public string Is_Closed { get; set; }
        public string Is_Reopened { get; set; }
        public string Filename { get; set; }
        public int FileId { get; set; }
        public DateTime? FileUploadDate { get; set; }
        public string FileUploadedBy { get; set; }
        public string Contractor_FirmName { get; set; }
        public string RoadName { get; set; }
        public string State { get; set; }
        public string District { get; set; }
        public int Agreement_Year { get; set; }
        public string Finalization_Date { get; set; }
        public string Finalized_By_Piu { get; set; }
        public bool Forwarded_To_Piu { get; set; }
        public bool Action_Taken_By_Piu { get; set; }
    }

    public class TrackingDetailsModel
    {
        public int TRACK_ID { get; set; }
        public int DETAIL_ID { get; set; }
        public string FORWARD_TO_PIU { get; set; }
        [DataType(DataType.Date)]
        public Nullable<System.DateTime> FORWARD_DATE { get; set; }
        [Display(Name = "SRRDS Remarks")]
        public string SRRDA_REMARKS { get; set; }
        public string FINALIZED_BY_PIU { get; set; }
        public Nullable<System.DateTime> PIU_FINALIZATION_DATE { get; set; }
        [Display(Name = "PIU Remarks")]
        public string PIU_REMARKS { get; set; }
        public Nullable<int> ROUND_CNT { get; set; }
        [Display(Name = "Latest Grievance")]
        public string IS_LATEST { get; set; }
        public int SRRDA_USER_ID { get; set; }
        public string SRRDA_IPADDR { get; set; }
        public Nullable<int> PIU_USER_ID { get; set; }
        public string PIU_IPADDR { get; set; }
        public HttpPostedFileBase[] files { get; set; }
        [Display(Name = "Contractor Remarks")]
        public string Contractor_Remarks { get; set; }
        [Display(Name = "Grievance Sub-Category")]
        public string Contractor_Grievance_SubCat { get; set; }
        [Display(Name = "Grievance Category")]
        public string Contractor_Grievance_Category { get; set; }
        [DataType(DataType.Date)]
        public Nullable<System.DateTime> SUBMITTED_ON { get; set; }
    }
}