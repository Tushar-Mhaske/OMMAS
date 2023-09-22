using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PMGSY.Models.NQMTourClaimModels
{
    public class NQM_TOUR_HONORARIUM_INSPECTION_OF_ROADS_DETAILS_MODEL
    {
        public int HONORARIUM_INSPECTION_ID { get; set; }

        public int TOUR_CLAIM_ID { get; set; }

        public int ADMIN_SCHEDULE_CODE { get; set; }

        [Display(Name = "Date Of Inspection: ")]
        [Required(ErrorMessage = "Date Of Inspection is Required")]
        [RegularExpression("^((?!^1$).)*$", ErrorMessage = "Date Of Inspection is Required")]
        //[RegularExpression(@"^(((0[1-9]|[12]\d|3[01])\/(0[13578]|1[02])\/((19|[2-9]\d)\d{2}))|((0[1-9]|[12]\d|30)\/(0[13456789]|1[012])\/((19|[2-9]\d)\d{2}))|((0[1-9]|1\d|2[0-8])\/02\/((19|[2-9]\d)\d{2}))|(29\/02\/((1[6-9]|[2-9]\d)(0[48]|[2468][048]|[13579][26])|((16|[2468][048]|[3579][26])00))))$", ErrorMessage = "Date of inspection is required.")]
        public string DATE_OF_INSPECTION { get; set; }

        public List<SelectListItem> lstDateOfInsp { get; set; }

        [Display(Name = "Work: ")]
        [Required(ErrorMessage = "Type of work is required")]
        [RegularExpression(@"^[IEAO]?", ErrorMessage = "Type of work is required.")]
        public string TYPE_OF_WORK { get; set; }

        public string TYPE_OF_WORK_OTHER { get; set; }

        public List<SelectListItem> LST_WORK { get; set; }

        [Display(Name = "Amount Claim: ")]
        [Required(ErrorMessage = "Amount Claim is required.")]
        [RegularExpression(@"^\d*(\.\d{1,2})?", ErrorMessage = "Amount Claim is not in valid format. ")]
        [Range(0, 999999.99, ErrorMessage = "Amount Claim is not in valid format.")]
        public decimal AMOUNT_CLAIMED { get; set; }

        [Display(Name = "Date Of Claim: ")]
        [Required(ErrorMessage = "Date of Claim is required.")]
        [RegularExpression(@"^(((0[1-9]|[12]\d|3[01])\/(0[13578]|1[02])\/((19|[2-9]\d)\d{2}))|((0[1-9]|[12]\d|30)\/(0[13456789]|1[012])\/((19|[2-9]\d)\d{2}))|((0[1-9]|1\d|2[0-8])\/02\/((19|[2-9]\d)\d{2}))|(29\/02\/((1[6-9]|[2-9]\d)(0[48]|[2468][048]|[13579][26])|((16|[2468][048]|[3579][26])00))))$", ErrorMessage = "Date of claim must be in dd/mm/yyyy format.")]
        public string DATE_OF_CLAIM { get; set; }

        [Display(Name = "Amount Proposed by NRRDA: ")]
        [RegularExpression(@"^\d*(\.\d{1,2})?", ErrorMessage = "Proposed amount is not in valid format. ")]
        [Range(0, 999999.99, ErrorMessage = "Proposed amount is not in valid format.")]
        public Nullable<decimal> AMOUNT_PASSED { get; set; }

        [Display(Name = "Date of amount passed by NDDRA: ")]
        [RegularExpression(@"^(((0[1-9]|[12]\d|3[01])\/(0[13578]|1[02])\/((19|[2-9]\d)\d{2}))|((0[1-9]|[12]\d|30)\/(0[13456789]|1[012])\/((19|[2-9]\d)\d{2}))|((0[1-9]|1\d|2[0-8])\/02\/((19|[2-9]\d)\d{2}))|(29\/02\/((1[6-9]|[2-9]\d)(0[48]|[2468][048]|[13579][26])|((16|[2468][048]|[3579][26])00))))$", ErrorMessage = "Date of passing must be in dd/mm/yyyy format.")]
        public Nullable<System.DateTime> DATE_OF_PASSING { get; set; }

        [Display(Name = "Remark: ")]
        [StringLength(255, ErrorMessage = "Remark must be less than 255 characters.")]
        [RegularExpression(@"^([a-zA-Z0-9 ._',&()-]+)$", ErrorMessage = "Remark is not in valid format.")]
        public string REMARK { get; set; }

        public int ADD_EDIT { get; set; }

        public Nullable<int> USERID { get; set; }
        public string IPADD { get; set; }
    }
}