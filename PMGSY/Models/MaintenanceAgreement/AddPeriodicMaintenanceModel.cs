using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace PMGSY.Models.MaintenanceAgreement
{
    public class AddPeriodicMaintenanceModel
    {

        public AddPeriodicMaintenanceModel()
        {
            this.RenewalTypeList = new List<SelectListItem>();
            this.TechnologyList = new List<SelectListItem>();
            this.PerFormanceYearList = new List<SelectListItem>();
            this.MANE_MAIN_MONTH_LIST = new List<SelectListItem>();
        }

        [Display(Name = "Maintenance year")]
        [Required(ErrorMessage = "Please select maintenance year")]
        [Range(1, int.MaxValue, ErrorMessage = "Please select maintenance year")]
        public int MANE_MAIN_YEAR { get; set; }
        public List<SelectListItem> MANE_MAIN_YEAR_LIST { get; set; }

        [Display(Name = "Maintenance Month")]
        [Required(ErrorMessage = "Please select maintenance month")]
        [Range(1, int.MaxValue, ErrorMessage = "Please select maintenance month")]
        public int MANE_MAIN_MONTH { get; set; }
        public List<SelectListItem> MANE_MAIN_MONTH_LIST { get; set; }

        [Display(Name = "Maintenance Month")]
        public String MonthName { get; set; }

        [Display(Name = "Start Chainage (in Kms.)")]
        [Required(ErrorMessage="Start Chainage is require")]
        [RegularExpression(@"^\s*(?=.*[0-9])\d*(?:\.\d{1,3})?\s*$", ErrorMessage = "Invalid Length,Can only contains Numeric values and 3 digits after decimal place")]
        [Range(0.000, 9999.999, ErrorMessage = "Invalid Length,can be upto 4 digits and 3 digits after decimal place")]
        [DisplayFormat(DataFormatString = "{0:0.000}", ApplyFormatInEditMode = true)]
        public decimal StartChainage { get; set; }

        [Display(Name = "End Chainage (in Kms.)")]
        [Required(ErrorMessage="End chainage is required.")]
        [RegularExpression(@"^\s*(?=.*[0-9])\d*(?:\.\d{1,3})?\s*$", ErrorMessage = "Invalid Length,Can only contains Numeric values and 3 digits after decimal place")]
        [Range(0.001, 9999.999, ErrorMessage = "Invalid Length,can be upto 4 digits and 3 digits after decimal place")]
        [DisplayFormat(DataFormatString = "{0:0.000}", ApplyFormatInEditMode = true)]
        public decimal EndChainage { get; set; }

        [Display(Name = "Length( Chainage) in Kms.")]
        [Required]
        [RegularExpression(@"^\s*(?=.*[0-9])\d*(?:\.\d{1,3})?\s*$", ErrorMessage = "Invalid Length,Can only contains Numeric values and 3 digits after decimal place")]
        [Range(0, 999999.999, ErrorMessage = "Invalid Length")]
        [DisplayFormat(DataFormatString = "{0:0.000}", ApplyFormatInEditMode = true)]
        public decimal Length { get; set; }

        [Display(Name = "Profile Correction Cost")]
        [Required(ErrorMessage = "Profile Correction is required.")]
        [RegularExpression(@"^\d*(\.\d{1,2})?", ErrorMessage = "Profile Correction is not in valid format. ")]
        [Range(0, 999.99, ErrorMessage = "Invalid Cost,Profile Correction cost can be upto 3 digits and 2 digits after decimal place")]
        public decimal ProfileCorrectionCost { get; set; }


        [Display(Name="Renewal Type")]
        [Required(ErrorMessage="Please select renewal type")]
        [Range(1,int.MaxValue,ErrorMessage="Please select renewal type")]
        public int RenewalType { get; set; }
        public List<SelectListItem> RenewalTypeList { get; set; }


        [Display(Name = "Technology")]
        [Required(ErrorMessage = "Please select technology")]
        [Range(1, int.MaxValue, ErrorMessage = "Please select a valid technology")]
        public int Technology { get; set; }
        public List<SelectListItem> TechnologyList { get; set; }
        
        [Display(Name = "Periodic maintenance Cost")]
        [Required(ErrorMessage = "Maintenance Cost is required.")]
        [RegularExpression(@"^\d*(\.\d{1,2})?", ErrorMessage = "Periodic maintenance Cost  is not in valid format. ")]
        [Range(1, 999.99, ErrorMessage = "Invalid Cost,Periodic maintenance cost can be upto 3 digits and 2 digits after decimal place.")]
        public decimal MaintanenaceCost { get; set; }

        [Display(Name = "Other Cost")]
        [RegularExpression(@"^\d*(\.\d{1,2})?", ErrorMessage = "Other Cost is not in valid format. ")]
        [Range(0, 999.99, ErrorMessage = "Invalid Cost,Other Cost can be  upto 3 digit and 2 digits after decimal place.")]
        public decimal OtherCost { get; set; }

        [Display(Name = "Total Cost (First periodic maintenance)")]
        [Required(ErrorMessage = "Total Maintenance Cost is required.")]
        [RegularExpression(@"^\d*(\.\d{1,2})?", ErrorMessage = "Total Maintenance cost is not in valid format. ")]
        [Range(0, 999.99, ErrorMessage = "Invalid Cost,Total Maintenance cost can be  upto 3 digit and 2 digits after decimal place.")]
        public decimal TotalMaintenanceCost { get; set; }

        [Display(Name="Maintenance Completion Date")]
        [Required(ErrorMessage="Maintenance completion date is required")]
        [RegularExpression(@"^(((0[1-9]|[12]\d|3[01])\/(0[13578]|1[02])\/((19|[2-9]\d)\d{2}))|((0[1-9]|[12]\d|30)\/(0[13456789]|1[012])\/((19|[2-9]\d)\d{2}))|((0[1-9]|1\d|2[0-8])\/02\/((19|[2-9]\d)\d{2}))|(29\/02\/((1[6-9]|[2-9]\d)(0[48]|[2468][048]|[13579][26])|((16|[2468][048]|[3579][26])00))))$", ErrorMessage = "Maintenance completion date must be in dd/mm/yyyy format.")]
        public String MaintenanceCompleteDate { get; set; }

        [Display(Name="Performance incentive")]
        public String IsPerformaceIncentive { get;set;}

        [Display(Name = "Is Periodic Maintenance Completed")]
        public String Iscompleted { get; set; }

        [Display(Name="Performance Incentive Year")]
        [Range(1, int.MaxValue, ErrorMessage = "Please select Performance Incentive year")]
        public int PerformanceIntensiveYear { get; set; }
        public List<SelectListItem> PerFormanceYearList { get; set; }

        [Display(Name = "Maintenance Cost Year1 ")]
       // [Required(ErrorMessage = "Maintenance Cost Year1 is required.")]
        [RegularExpression(@"^\d*(\.\d{1,2})?", ErrorMessage = "Maintenance Cost Year1 is not in valid format. ")]
        [Range(0, 999999.99, ErrorMessage = "Maintenance Cost Year1 is exceeding the limit.can be  upto 6 digit.")]
        public decimal MANE_YEAR1_AMOUNT { get; set; }

        [Display(Name = "Maintenance Cost Year2")]
      //  [Required(ErrorMessage = "Maintenance Cost Year2 is required.")]
        [RegularExpression(@"^\d*(\.\d{1,2})?", ErrorMessage = "Maintenance Cost Year2 is not in valid format. ")]
        [Range(0, 999999.99, ErrorMessage = "Maintenance Cost Year2 is exceeding the limit.can be  upto 6 digit.")]
        public decimal MANE_YEAR2_AMOUNT { get; set; }

        [Display(Name = "Maintenance Cost Year3")]
      //  [Required(ErrorMessage = "Maintenance Cost Year3 is required.")]
        [RegularExpression(@"^\d*(\.\d{1,2})?", ErrorMessage = "Maintenance Cost Year3 is not in valid format. ")]
        [Range(0, 999999.99, ErrorMessage = "Maintenance Cost Year3 is exceeding the limit.can be  upto 6 digit.")]
        public decimal MANE_YEAR3_AMOUNT { get; set; }

        [Display(Name = "Maintenance Cost Year4")]
      //  [Required(ErrorMessage = "Maintenance Cost Year4 is required.")]
        [RegularExpression(@"^\d*(\.\d{1,2})?", ErrorMessage = "Maintenance Cost Year4 is not in valid format. ")]
        [Range(0, 999999.99, ErrorMessage = "Maintenance Cost Year4 is exceeding the limit.can be  upto 6 digit.")]
        public decimal MANE_YEAR4_AMOUNT { get; set; }

        [Display(Name = "Maintenance Cost Year5")]
     //   [Required(ErrorMessage = "Maintenance Cost Year5 is required.")]
        [RegularExpression(@"^\d*(\.\d{1,2})?", ErrorMessage = "Maintenance Cost Year5 is not in valid format. ")]
        [Range(0, 999999.99, ErrorMessage = "Maintenance Cost Year5 is exceeding the limit.can be  upto 6 digit.")]
        public decimal MANE_YEAR5_AMOUNT { get; set; }

        [Display(Name = "Total Cost")]
        //[CustomMaintenanceRequired("PMGSYScheme", ErrorMessage = "Renewal Cost is required.")]
        [RegularExpression(@"^\d*(\.\d{1,2})?", ErrorMessage = "Renewal Cost is not in valid format. ")]
        //[Range(0, 9999999.99, ErrorMessage = "Renewal Cost is exceeding the limit.can be  upto 7 digit.")]
        [Range(0, 9999999.99, ErrorMessage = "Renewal Cost can be upto 7 digit and greater than 0.")]
        public decimal MANE_TOTAL_AMOUNT { get; set; }


        public string Operation { get; set; }

        public Int32 ImdRoadCode { get;set;}

        public string IsSecondPeriodic { get; set; }

        public string technologyName { get; set; }

        public string RenewalName { get; set; }

        public int intMaxValue { get; set; }
    }
}