using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PMGSY.Models.NQMTourClaimModels
{
    public class NQM_TOUR_DISTRICT_DETAILS_MODEL
    {
        public int ADMIN_QM_CODE { get; set; }

        public int ADMIN_SCHEDULE_CODE { get; set; }

        public int DISTRICT_DETAILS_ID { get; set; }

        [Display(Name = "State Name")]
        [Required(ErrorMessage = "Please select State.")]
        [Range(1, 2147483647, ErrorMessage = "Please select State.")]
        public int STATE_CODE { get; set; }

        public List<SelectListItem> lstStates { set; get; }

        [RegularExpression(@"^(((0[1-9]|[12]\d|3[01])\/(0[13578]|1[02])\/((19|[2-9]\d)\d{2}))|((0[1-9]|[12]\d|30)\/(0[13456789]|1[012])\/((19|[2-9]\d)\d{2}))|((0[1-9]|1\d|2[0-8])\/02\/((19|[2-9]\d)\d{2}))|(29\/02\/((1[6-9]|[2-9]\d)(0[48]|[2468][048]|[13579][26])|((16|[2468][048]|[3579][26])00))))$", ErrorMessage = "Date of claim must be in dd/mm/yyyy format.")]
        public System.DateTime DATE_OF_CLAIM { get; set; }

        public List<SelectListItem> lstDistricts { set; get; }

        [Display(Name = "District Name")]
        [Required(ErrorMessage = "Please select Distict.")]
        [Range(1, 2147483647, ErrorMessage = "Please select Distict.")]
        public int DISTRICT_CODE { get; set; }

        [Display(Name = "Date From")]
        [Required(ErrorMessage = "Date from is required")]
        [DataType(DataType.Date)]
        [RegularExpression(@"^(((0[1-9]|[12]\d|3[01])\/(0[13578]|1[02])\/((19|[2-9]\d)\d{2}))|((0[1-9]|[12]\d|30)\/(0[13456789]|1[012])\/((19|[2-9]\d)\d{2}))|((0[1-9]|1\d|2[0-8])\/02\/((19|[2-9]\d)\d{2}))|(29\/02\/((1[6-9]|[2-9]\d)(0[48]|[2468][048]|[13579][26])|((16|[2468][048]|[3579][26])00))))$", ErrorMessage = "Date from must be in dd/mm/yyyy format.")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        public System.DateTime DATE_FROM { get; set; }

        [Display(Name = "Date To")]
        [Required(ErrorMessage = "Date To is required")]
        [DataType(DataType.Date)]
        [RegularExpression(@"^(((0[1-9]|[12]\d|3[01])\/(0[13578]|1[02])\/((19|[2-9]\d)\d{2}))|((0[1-9]|[12]\d|30)\/(0[13456789]|1[012])\/((19|[2-9]\d)\d{2}))|((0[1-9]|1\d|2[0-8])\/02\/((19|[2-9]\d)\d{2}))|(29\/02\/((1[6-9]|[2-9]\d)(0[48]|[2468][048]|[13579][26])|((16|[2468][048]|[3579][26])00))))$", ErrorMessage = "Date to must be in dd/mm/yyyy format.")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        public System.DateTime DATE_TO { get; set; }

        public int TOUR_CLAIM_ID { get; set; }

        public Nullable<int> USERID { get; set; }
        public string IPADD { get; set; }

        public int ADD_EDIT { get; set; }
    }
}