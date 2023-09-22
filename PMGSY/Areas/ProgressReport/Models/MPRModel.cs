using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PMGSY.Areas.ProgressReport.Models
{
    public class MPRModel
    {
    }

    public class WorkDetails
    {
        [Display(Name = "Connectivity")]
        public string ConnectivityCode { get; set; }
        public List<SelectListItem> ConnectivityList { get; set; }

        [Display(Name = "Scheme")]
        public int SchemeCode { get; set; }
        public List<SelectListItem> SchemeList { get; set; }


        [Display(Name = "Proposal Type")]
        public string ProposalTypeCode { get; set; }
        public List<SelectListItem> ProposalTypeList { get; set; }

        [Required(ErrorMessage = "Month required")]
        [Range(0, int.MaxValue, ErrorMessage = "Invalid Month")]
        public int Month { get; set; }
        public List<SelectListItem> MonthList { get; set; }

        [Required(ErrorMessage = "Year required")]
        [Range(0, int.MaxValue, ErrorMessage = "Invalid Year")]
        public int Year { get; set; }
        public List<SelectListItem> YearList { get; set; }

        [Required(ErrorMessage = "State required")]
        [Range(0, int.MaxValue, ErrorMessage = "Select State")]
        [Display(Name = "State")]
        public int StateCode { get; set; }
        public List<SelectListItem> StateList { get; set; }



 
        public string StateName { get; set; }
   
        public int Mast_State_Code { get; set; }



        //public int Mast_Block_Code { get; set; }
        //public int Mast_District_Code { get; set; }

    

        //[Required(ErrorMessage = "Please select District.")]
        //[Range(0, int.MaxValue, ErrorMessage = "Please select District.")]
        ////[RegularExpression(@"^([0-9]+)$", ErrorMessage = "District must be valid number.")]
        //[Display(Name = "District")]
        //public int DistrictCode { get; set; }
        //public List<SelectListItem> DistrictList { get; set; }

        //[Required(ErrorMessage = "Please select Block.")]
        //[Range(0, int.MaxValue, ErrorMessage = "Please select Block.")]
        //[Display(Name = "Block")]
        //public int BlockCode { get; set; }
        //public List<SelectListItem> BlockList { get; set; }

        //[Required(ErrorMessage = "Please select Collaboration.")]
        //[Range(0, int.MaxValue, ErrorMessage = "Please select Collaboration.")]
        //[Display(Name = "Collaboration")]
        //public int CollabCode { get; set; }
        //public List<SelectListItem> CollabList { get; set; }

      
    }

    public class MPRTargetModel
    {
        [Required(ErrorMessage = "Month required")]
        [Range(0, int.MaxValue, ErrorMessage = "Invalid Month")]
        public int Month { get; set; }
        public List<SelectListItem> MonthList { get; set; }

        [Required(ErrorMessage = "Year required")]
        [Range(0, int.MaxValue, ErrorMessage = "Invalid year")]
        public int Year { get; set; }
        public List<SelectListItem> YearList { get; set; }

        [Required(ErrorMessage = "Localized value required")]
        [RegularExpression(@"^(\b(en|hi)\b+)$", ErrorMessage = "Invalid Localized Value")]
        public string localizedValue { get; set; }

        public int LevelCode { get; set; }
        public string StateName { get; set; }
        public string DistName { get; set; }
        public string BlockName { get; set; }
        public string CollabName { get; set; }
        public int Mast_State_Code { get; set; }
        public int Mast_Block_Code { get; set; }
        public int Mast_District_Code { get; set; }

        [Required(ErrorMessage = "State required")]
        [Range(0, int.MaxValue, ErrorMessage = "Invalid state")]
        [Display(Name = "State")]
        public int StateCode { get; set; }
        public List<SelectListItem> StateList { get; set; }

        [Required(ErrorMessage = "Please select District.")]
        [Range(0, int.MaxValue, ErrorMessage = "Please select District.")]
        //[RegularExpression(@"^([0-9]+)$", ErrorMessage = "District must be valid number.")]
        [Display(Name = "District")]
        public int DistrictCode { get; set; }
        public List<SelectListItem> DistrictList { get; set; }

        [Required(ErrorMessage = "Please select Block.")]
        [Range(0, int.MaxValue, ErrorMessage = "Please select Block.")]
        [Display(Name = "Block")]
        public int BlockCode { get; set; }
        public List<SelectListItem> BlockList { get; set; }

        [Required(ErrorMessage = "Please select Collaboration.")]
        [Range(0, int.MaxValue, ErrorMessage = "Please select Collaboration.")]
        [Display(Name = "Collaboration")]
        public int CollabCode { get; set; }
        public List<SelectListItem> CollabList { get; set; }

        public string QuarterName { get; set; }
        public string YearName { get; set; }
    }
}