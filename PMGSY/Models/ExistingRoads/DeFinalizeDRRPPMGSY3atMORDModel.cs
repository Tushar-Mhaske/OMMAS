using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace PMGSY.Models.ExistingRoads
{
    public class DeFinalizeDRRPPMGSY3atMORDModel
    {
        //[Required(ErrorMessage = "Please select District.")]
        //[Range(1, int.MaxValue, ErrorMessage = "Please select District.")]
        //public int districtCode { get; set; }
        //public List<SelectListItem> lstDistricts { get; set; }

        //[Required(ErrorMessage = "Please select Block.")]
        //[Range(1, int.MaxValue, ErrorMessage = "Please select Block.")]
        //public int blockCode { get; set; }
        //public List<SelectListItem> lstBlocks { get; set; }

        //[RegularExpression(@"^(((0[1-9]|[12]\d|3[01])\/(0[13578]|1[02])\/((19|[2-9]\d)\d{2}))|((0[1-9]|[12]\d|30)\/(0[13456789]|1[012])\/((19|[2-9]\d)\d{2}))|((0[1-9]|1\d|2[0-8])\/02\/((19|[2-9]\d)\d{2}))|(29\/02\/((1[6-9]|[2-9]\d)(0[48]|[2468][048]|[13579][26])|((16|[2468][048]|[3579][26])00))))$", ErrorMessage = "Final Payment Date must be in dd/mm/yyyy format.")]
        //public string finalizationDate { get; set; }

        //public bool isFinalized { get; set; }

        //public int statecode { get; set; }
        //public List<SelectListItem> statelist { get; set; }

        public int LevelCode { get; set; }
        public string StateName { get; set; }
        public string DistName { get; set; }
      
        public int Mast_State_Code { get; set; }
     
        public int Mast_District_Code { get; set; }

   

        [Display(Name = "State : ")]
        [Required(ErrorMessage = "Please select State. ")]
        [Range(1, int.MaxValue, ErrorMessage = "Please select  State.")]

        public int StateCode { get; set; }
        public List<SelectListItem> StateList { get; set; }




        [Display(Name = "District : ")]
        [Required(ErrorMessage = "Please select District.")]
        [Range(0, int.MaxValue, ErrorMessage = "Please select District.")]

        public int DistrictCode { get; set; }
        public List<SelectListItem> DistrictList { get; set; }

 

    }
}