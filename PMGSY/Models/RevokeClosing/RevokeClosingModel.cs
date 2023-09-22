using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PMGSY.Models.RevokeClosing
{
    public class RevokeClosingModel
    {

        [Required(ErrorMessage = "Month is Required")]
        [Range(1, 12, ErrorMessage = "Please Select Month")]
        public int StartMonth { get; set; }


        [Required(ErrorMessage = "Year is Required")]
        [Range(1, int.MaxValue, ErrorMessage = "Please Select Year")]
        public int StartYear { get; set; }

        public int Month { get; set; }
        public int Year { get; set; }

        [Required(ErrorMessage = "Monthly or Yearly selection is Required")]
        public string durationFlag { get; set; }


        [Required(ErrorMessage = "Month is Required")]
        [Range(1, 12, ErrorMessage = "Please Select Month")]
        public int ToMonth { get; set; }


        [Required(ErrorMessage = "Year is Required")]
        [Range(1, int.MaxValue, ErrorMessage = "Please Select Year")]
        public int ToYear { get; set; }


        public Int32 AdminNdCode { get; set; }

        [RegularExpression("[PMA]", ErrorMessage = "Please select Fund Type.")]
        public String FundType { get; set; }
        public String MonthClosed { get; set; }
        public Int16 LevelID { get; set; }


        //Added by Abhishek kamble to populate PIU for month Revoke 9-sep-2014
        [Display(Name = "DPIU")]
        [Range(1, int.MaxValue, ErrorMessage = "Please Select DPIU")]
        public int DPIU_CODE { get; set; }
        public List<SelectListItem> DPIU_LIST { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Please Select SRRDA")]
        public int SRRDA_CODE { get; set; }
        public List<SelectListItem> SRRDA_LIST { get; set; }

        //use for radio button O-SRRDA D-DPIU
        public string OwnDPIUFlag { get; set; }

        [Display(Name = "Remark")]
        [Required(ErrorMessage = "Please enter Remark.")]
        [StringLength(250, ErrorMessage = "Remark must be less than 250 characters.")]
        [RegularExpression(@"^([a-zA-Z0-9 ._,\r\n&()-]+)$", ErrorMessage = "Remark is not in valid format.")]
        public string Remark
        {
            get;
            set;
        }
    }


    public class FinalizeBalanceSheetModel
    {
        [Display(Name = "Year")]
        [Required(ErrorMessage = "Year is Required")]
        [Range(1, int.MaxValue, ErrorMessage = "Please Select Year")]
        public Int16 Year { get; set; }

        public String FinalizedYear { get; set; }
        // public List<SelectList> lstYear { get; set; }

        [Display(Name = "Audit Date")]
        [RegularExpression(@"^(((0[1-9]|[12]\d|3[01])\/(0[13578]|1[02])\/((19|[2-9]\d)\d{2}))|((0[1-9]|[12]\d|30)\/(0[13456789]|1[012])\/((19|[2-9]\d)\d{2}))|((0[1-9]|1\d|2[0-8])\/02\/((19|[2-9]\d)\d{2}))|(29\/02\/((1[6-9]|[2-9]\d)(0[48]|[2468][048]|[13579][26])|((16|[2468][048]|[3579][26])00))))$", ErrorMessage = "Start Date is not in valid format")]
        [Required(ErrorMessage = "Audit Date is required.")]
        public String AuditDate { get; set; }
    }

    public class DefinalizeBalanceSheetModel
    {
        [Display(Name = "Agency")]
        //[Required(ErrorMessage="Please select Agency")]
        [Range(1, int.MaxValue, ErrorMessage = "Please select Agency.")]
        public Int32 AdminNdCode { get; set; }
        public List<SelectListItem> lstAgency { get; set; }

        [Display(Name = "Year")]
        [Range(1, int.MaxValue, ErrorMessage = "Please select Year.")]
        public int Year { get; set; }
        public List<SelectListItem> lstYear { get; set; }

        [Display(Name = "Fund Type")]
        //[Required(ErrorMessage="Please select Fund Type.")]
        [RegularExpression("[PAM]", ErrorMessage = "Please select Fund Type.")]
        public String FundType { get; set; }
        public List<SelectListItem> lstFundType { get; set; }

        //public String FinalizedYear { get; set; }

    }
}