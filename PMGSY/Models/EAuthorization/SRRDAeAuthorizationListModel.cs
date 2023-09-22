﻿using PMGSY.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PMGSY.Models.EAuthorization
{
    public class SRRDAeAuthorizationListModel
    {
        public SRRDAeAuthorizationListModel()
        {
            CommonFunctions objCommon = new CommonFunctions();
            this.CURRENT_DATE = objCommon.GetDateTimeToString(DateTime.Now);
        }


        [Display(Name = "Status")]
        //[Range(1, 12, ErrorMessage = "Please select Month")]
        public Int16 EAUTH_STATUS1 { get; set; }

        public List<SelectListItem> EAUTH_STATUS_LIST { get; set; }
    

        public string FUND_TYPE { get; set; }
        public string EAUTH_NO { get; set; }
        public string EAUTH_DATE { get; set; }
        public string REQUEST_AMOUNT { get; set; }
        public string TOTAL_APPROVE_AMOUNT { get; set; }

        public Int32 PIU_ADMIN_ND_CODE { get; set; }
        public Int32 EO_ADMIN_ND_CODE { get; set; }
        

        [Display(Name = "DPIU Name")]
        //[Range(1,Int32.MaxValue,ErrorMessage="Please select DPIU")]   no validation on selection 
        public Int32 ADMIN_ND_CODE { get; set; }

        [Display(Name = "Month")]
        //[Range(1, 12, ErrorMessage = "Please select Month")]
        public Int16 EAUTH_MONTH { get; set; }
        
        public List<SelectListItem> EAUTH_MONTH_LIST { get; set; }

        [Display(Name = "Year")]
        //[Range(2000, Int32.MaxValue, ErrorMessage = "Please select Year")]
        public Int16 EAUTH_YEAR { get; set; }
        public List<SelectListItem> EAUTH_YEAR_LIST { get; set; }
        public long REQUEST_ID { get; set; }
        public long EAUTH_ID { get; set; }
        public string EAUTH_STATUS { get; set; }

        public string EncryptedEAuthID { get; set; }
        public Int32 StateCode { get; set; }


        [Display(Name = "Date of Operation")]
        [Required(ErrorMessage = "Approval/Rejection Date is Required")]
        [RegularExpression(@"^(((0[1-9]|[12]\d|3[01])\/(0[13578]|1[02])\/((19|[2-9]\d)\d{2}))|((0[1-9]|[12]\d|30)\/(0[13456789]|1[012])\/((19|[2-9]\d)\d{2}))|((0[1-9]|1\d|2[0-8])\/02\/((19|[2-9]\d)\d{2}))|(29\/02\/((1[6-9]|[2-9]\d)(0[48]|[2468][048]|[13579][26])|((16|[2468][048]|[3579][26])00))))$", ErrorMessage = "Please Enter valid date")]
        [DataType(DataType.Date, ErrorMessage = "Invalid Date")]
        //[IsDateAfter("CURRENT_DATE", true, ErrorMessage = "Date must be less than or equal to today's date")]
        public String DATE_OF_OPERATION { get; set; }
        
        [Display(Name = "Remarks")]
        [Required(ErrorMessage = "Remarks is Required")]
        [StringLength(255, ErrorMessage = "Maximum 255 Characters Allowed")]
        [RegularExpression(@"^[a-zA-Z0-9-/.() ]+$", ErrorMessage = "Only Alphanumeric, Space and '-','/','.','(',')' Allowed")]
        public String REMARKS { get; set; }

        public String CURRENT_DATE { get; set; }

        public String REQUEST_ID_LIST { get; set; }
        
        //public virtual ACC_AUTH_REQUEST_MASTER ACC_AUTH_REQUEST_MASTER { get; set; }
        public Int64 EAuthID { get; set; }
    }
}