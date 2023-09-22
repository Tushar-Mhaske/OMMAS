using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PMGSY.Models.QualityMonitoring
{
    public class InspectionTargetModel
    {
        public bool flg { get; set; }

        //[Display(Name = "State")]
        //[Required(ErrorMessage = "State is Required")]
        //public Int32 STATEID { get; set; }
        //public List<SelectListItem> STATE_LIST { get; set; }


        //[Display(Name = "Month")]
        //[Required(ErrorMessage = "Month is Required")]
        //public Int32 MONTHID { get; set; }
        //public List<SelectListItem> MONTH_LIST { get; set; }


        //[Display(Name = "Year")]
        //[Required(ErrorMessage = "Year is Required")]
        //public Int32 YEARID { get; set; }
        //public List<SelectListItem> YEAR_LIST { get; set; }


        //[Display(Name = "Year")]
        //[Required(ErrorMessage = "Year is Required")]
        //public string NUMBER_OF_INSPECTION { get; set; }


        [Required]
        [Display(Name = "State")]
        [Range(1, Int32.MaxValue, ErrorMessage = "Please select State.")]
        public Int32 STATEID { get; set; }
        public List<SelectListItem> STATE_LIST { get; set; }


        [Required]
        [Display(Name = "Month")]
        [Range(1, Int32.MaxValue, ErrorMessage = "Please select Month.")]
        public Int32 MONTHID { get; set; }
        public List<SelectListItem> MONTH_LIST { get; set; }



                

        [Required]
        [Display(Name = "Year")]
        [Range(1, Int32.MaxValue, ErrorMessage = "Please select Year.")]
        public Int32 YEARID { get; set; }
        public List<SelectListItem> YEAR_LIST { get; set; }


        
        [Required(ErrorMessage = "Number of Inspection is Required")]
        [Display(Name = "Number of Inspection")]
        [RegularExpression(@"^[0-9]+$", ErrorMessage = "Only Numbers are Allowed")]
        public string NUMBER_OF_INSPECTION { get; set; }


        public bool IsForUpdate { get; set; }
        public Int32 INSPECTION_TARGET_ID { get; set; }
        public bool status { get; set; }
        public string message { get; set; }
        public string EncINSPECTION_TARGET_ID { get; set; }

        public Int32 page { get; set; }
        public Int32 rows { get; set; }
        public String sidx { get; set; }
        public String sord { get; set; }


    }
}