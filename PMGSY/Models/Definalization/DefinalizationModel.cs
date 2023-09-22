using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PMGSY.Models.Definalization
{
    public class DefinalizationModel
    {

        public DefinalizationModel()
        {
            this.YEAR =Convert.ToInt16(DateTime.Now.Year);
            this.MONTH = Convert.ToInt16(DateTime.Now.Month);
        }

          [Display(Name = "Month")]
        [Required(ErrorMessage = "Month is Required")]
        [Range(1, Int16.MaxValue, ErrorMessage = "Please Select Month")]
        public short MONTH { get; set; }
        public List<SelectListItem> MONTH_LIST { get; set; }

         [Display(Name = "Year")]
        [Required(ErrorMessage = "Year is Required")]
        [Range(1, Int16.MaxValue, ErrorMessage = "Please Select Year")]
        public short YEAR { get; set; }
        public List<SelectListItem> YEAR_LIST { get; set; }

        // [Display(Name = "Fund Type")]
        //[Required(ErrorMessage = "Fund Type is Required")]
        //[MaxLength(1,ErrorMessage="Invalid Fund Type")]
        //public String FUND_TYPE { get; set; }
        public List<SelectListItem> FUND_TYPE_LIST { get; set; }

         [Display(Name = "Voucher Type")]
        [Required(ErrorMessage = "Voucher Type is Required")]
        [MaxLength(1, ErrorMessage = "Invalid Voucher Type ")]
        public String VOUCHER_TYPE { get; set; }
        public List<SelectListItem> VOUCHER_TYPE_LIST { get; set; }

        [Required(ErrorMessage = "level is Required")]
        [MaxLength(1, ErrorMessage = "Invalid level")]
        public String LEVEL { get; set; }

        
        public short DPIU { get; set; }
        public List<SelectListItem> DPIU_LIST { get; set; }

        //Added by Abhishek kamble 5Jan2015
        public List<SelectListItem> SRRDA_LIST { get; set; }
        public Int32 SRRDAAdminNdCode { get; set; }

        // added by rohit borse on 06-06-2022
        [Display(Name = "Fund Type")]
        [Required(ErrorMessage = "Fund Type is required")]
        public string fundType { get; set; }

    }

    public class VoucherFilterModel
    {
        public Int16 Month { get; set; }
        public Int16 Year { get; set; }
        public Int32 page { get; set; }
        public Int32 rows { get; set; }
        public String sidx { get; set; }
        public String sord { get; set; }
        public String FundType { get; set; }
        public Int32 AdminNdCode { get; set; }
        public Int16 LevelId { get; set; }
        public Int64 BillId { get; set; }
        public String Bill_type { get; set; }

        public Int32 SRRDAAdminNdCode { get; set; }
       
    }

    
}