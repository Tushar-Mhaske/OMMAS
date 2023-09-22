using PMGSY.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PMGSY.Models.Payment
{
    public class HoldingSDALayoutModel
    {
        public HoldingSDALayoutModel()
        {

            CommonFunctions objCommon = new CommonFunctions();
            this.MONTH = Convert.ToInt16(DateTime.Now.Month);
            this.YEAR = Convert.ToInt16(DateTime.Now.Year);
        }

        [Display(Name = "DPIU")]
        [Range(1, Int32.MaxValue, ErrorMessage = "Please select DPIU")]
        public int ADMIN_ND_CODE { get; set; }
        public List<SelectListItem> Dpiu_List { get; set; }


        [Range(1, Int16.MaxValue, ErrorMessage = "Please Select Month")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Please Select Month")]
        public short MONTH { get; set; }
        public List<SelectListItem> YEAR_LIST { get; set; }


        [Range(1, Int16.MaxValue, ErrorMessage = "Please Select Year")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Please Select Year")]
        public short YEAR { get; set; }
        public List<SelectListItem> MONTH_LIST { get; set; }

        [Range(1, Int16.MaxValue, ErrorMessage = "Please Select Account Type")]
        public string ACCOUNT_TYPE { get; set; }
        public List<SelectListItem> ACCOUNT_TYPE_LIST { get; set; }

    }
}