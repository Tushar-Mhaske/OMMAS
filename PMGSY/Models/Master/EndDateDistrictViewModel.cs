using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace PMGSY.Models.Master
{
    public class EndDateDistrictViewModel
    {
        [UIHint("hidden")]
        public string EncryptedAdminId { get; set; }

     
        [Display(Name = "End Date")]
        [Required(ErrorMessage="End Date is required.")]
        [RegularExpression(@"^(((0[1-9]|[12]\d|3[01])\/(0[13578]|1[02])\/((19|[2-9]\d)\d{2}))|((0[1-9]|[12]\d|30)\/(0[13456789]|1[012])\/((19|[2-9]\d)\d{2}))|((0[1-9]|1\d|2[0-8])\/02\/((19|[2-9]\d)\d{2}))|(29\/02\/((1[6-9]|[2-9]\d)(0[48]|[2468][048]|[13579][26])|((16|[2468][048]|[3579][26])00))))$", ErrorMessage = "End date must be in dd/mm/yyyy format.")]
        public string EndDateDistrict { get; set; }

        [Display(Name = "District Name")]
        public string DistrictName { get; set; }

        [Display(Name = "Start Date")]
        public string StartDateDistrict { get; set; }
    }
}