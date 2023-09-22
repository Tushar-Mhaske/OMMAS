using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PMGSY.Models.MonthlyClosing
{

    public class MaxMonthYearClosedModel {

        public short? CLOSING_YEAR { get; set; }
        public short? CLOSING_MONTH { get; set; }
        public long ADMIN_ND_CODE { get; set; }
    }


    public class monthlyClosingResultModel
    {
        public int ERR_NUMBER { get; set; }
        public int ERR_LINE_NUMBER { get; set; }
        public String ERR_MESSAGE { get; set; }
    }

    public class MonthlyClosingModel
    {

        public MonthlyClosingModel()
        {
            this.FROM_MONTH = Convert.ToInt16(DateTime.Now.Year);
            this.FROM_YEAR = Convert.ToInt16(DateTime.Now.Month);
            this.CURRENT_YEAR = DateTime.Now.Year;
            this.CURRENT_MONTH = DateTime.Now.Month;
        }

        [Display(Name = "Month")]
        [Required(ErrorMessage = "Month is Required")]
        [Range(1, 12, ErrorMessage = "Please Select Month")]
        //[RegularExpression(@"^([0-9]+)$", ErrorMessage = "Invalid month")]
        public short FROM_MONTH { get; set; } 
       
        public List<SelectListItem> FROM_MONTH_LIST { get; set; }

        [Display(Name = "Year")]
        [Required(ErrorMessage = "Year is Required")]
        [Range(0, Int16.MaxValue, ErrorMessage = "Please Select Year")]
       // [RegularExpression(@"^([1-9]+)$", ErrorMessage = "Invalid year")]
        public short FROM_YEAR { get; set; }
        
        public List<SelectListItem> FROM_YEAR_LIST { get; set; }
        
        [Display(Name = "Month")]
       
        [Range(0, 12, ErrorMessage = "Please Select Month")]
        public short TO_MONTH { get; set; }
        public List<SelectListItem> TO_MONTH_LIST { get; set; }

        [Display(Name = "Year")]            
        [Range(0, Int16.MaxValue, ErrorMessage = "Please Select Year")]
        public short TO_YEAR { get; set; }
        public List<SelectListItem> TO_YEAR_LIST { get; set; }
                                                            
        [MaxLength(1, ErrorMessage = "Invalid Month Closing option")]
        public String CLOSE_MONTH_TYPE { get; set; }


        public Int32 CURRENT_MONTH { get; set; }
        public Int32 CURRENT_YEAR{ get; set; }

        //Added by Abhishek kamble to populate PIU for month close 28-Aug-2014
        [Display(Name="DPIU")]        
        public int DPIU_CODE { get; set; }
        public List<SelectListItem> DPIU_LIST { get; set; }

        //use for radio button O-SRRDA D-DPIU
        public string OwnDPIUFlag { get; set; }

    }


    //Added By Abhishek 18-July-2014
    public class ListPIUNames {
        //public List<PIUNames> lstPiuNames { get; set; }
        public List<USP_ACC_VERIFY_PIUS_CHEQUEACK_Result> USP_ACC_VERIFY_PIUS_CHEQUEACK_Model { get; set; }
    }                   
    public class PIUNames {
        public String ADMIN_ND_NAME { get; set; }
        public String Month { get; set; }
        public String Year { get; set; }        
    }
    //Added By Abhishek 18-July-2014
}