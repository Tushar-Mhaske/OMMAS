using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PMGSY.Models.Execution
{
    public class AddATRByPIU
    {

        #region PIU



        [RegularExpression("[SRU]", ErrorMessage = "Please Select valid Grade")]
        public string GradeCode { get; set; }
        public List<SelectListItem> GradeList { get; set; }

        public int RSAId { get; set; }

        [RegularExpression("[YN]", ErrorMessage = "Please Select Acceptance of recommendation")]
        public string AccpetCode { get; set; }
        public List<SelectListItem> AccpetList { get; set; }


        [Required(ErrorMessage = "Please select ATR Upload Date")]
        [RegularExpression(@"^(((0[1-9]|[12]\d|3[01])\/(0[13578]|1[02])\/((19|[2-9]\d)\d{2}))|((0[1-9]|[12]\d|30)\/(0[13456789]|1[012])\/((19|[2-9]\d)\d{2}))|((0[1-9]|1\d|2[0-8])\/02\/((19|[2-9]\d)\d{2}))|(29\/02\/((1[6-9]|[2-9]\d)(0[48]|[2468][048]|[13579][26])|((16|[2468][048]|[3579][26])00))))$", ErrorMessage = "Date is not in valid format")]
        public string ATRUploadDate { get; set; }


        [Display(Name = "Description of Action Taken By PIU")]
        [RegularExpression(@"^[a-zA-Z0-9 ,.()-]+$", ErrorMessage = "Invalid ATR , Can only contains AlphaNumeric values and [,.()-]")]
        [Required(ErrorMessage = "Please enter Description of Action Taken.")]
        //[CheckATRRegradeWiseRejectValidation("ATR_REGRADE_STATUS", ErrorMessage = "Please enter Safety Issue details.")]
        public string ATR_By_PIU { get; set; }

        #endregion
    }
}