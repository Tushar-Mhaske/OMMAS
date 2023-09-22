using PMGSY.BLL.Common;
using PMGSY.Common;
using PMGSY.Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PMGSY.Models.ChequeBook
{
    public class ChequeBookDetailsViewModel
    {
        public ChequeBookDetailsViewModel()
        {
            lstBankAccType = new List<SelectListItem>(); // Added by Saurabh
        }


        // added by saurabh
        [Required(ErrorMessage = "Please select Account Type.")]
        [RegularExpression(@"^[SHD]?", ErrorMessage = "Please select valid Account Type.")]
        //[RegularExpression(@"^([SHD]$", ErrorMessage = "Please select valid Account Type.")]
        [Display(Name = "Account Type :")]
        public string BANK_ACC_TYPE { get; set; }
        public List<SelectListItem> lstBankAccType { get; set; }


        [UIHint("hidden")]
        public string EncryptedChequeBookCode { get; set; }
        public int CHQ_BOOK_ID { get; set; }

        [Display(Name = "Start Leaf")]
        [Required(ErrorMessage="Start Leaf is mandatory")]
        [RegularExpression(@"^\d{1,6}?$", ErrorMessage = "Only Numeric Allowed upto six digits")]
        public string LEAF_START { get; set; }

        [Display(Name = "End Leaf")]
        [Required(ErrorMessage="End Leaf is mandatory")]
        [RegularExpression(@"^\d{1,6}?$", ErrorMessage = "Only Numeric Allowed upto six digits")]
        public string LEAF_END { get; set; }

        public string FUND_TYPE { get; set; }

        [Display(Name = "Cheque Issue Date")]
        [Required(ErrorMessage = "Cheque Issue Date is Required")]
        [RegularExpression(@"^(((0[1-9]|[12]\d|3[01])\/(0[13578]|1[02])\/((19|[2-9]\d)\d{2}))|((0[1-9]|[12]\d|30)\/(0[13456789]|1[012])\/((19|[2-9]\d)\d{2}))|((0[1-9]|1\d|2[0-8])\/02\/((19|[2-9]\d)\d{2}))|(29\/02\/((1[6-9]|[2-9]\d)(0[48]|[2468][048]|[13579][26])|((16|[2468][048]|[3579][26])00))))$", ErrorMessage = "Please Enter valid Cheque Issue date")]
        public string ISSUE_DATE { get; set; }

        public short BANK_CODE { get; set; }

        [Display(Name = "DPIU")]
        [Range(1, Int32.MaxValue, ErrorMessage = "Please select DPIU")]
        public int ADMIN_ND_CODE { get; set; }

        public Int16 LVL_ID { get; set; }

        public string CURRENT_DATE { get; set; }

        public string ACC_OPEN_DATE { get; set; }
        // SRRDA and Admin Fund 
        public String IsSRRDADpiu { get; set; }
     
        public SelectList PopulateDPIU
        {
            get
            {
                CommonFunctions objComm = new CommonFunctions();
                List<SelectListItem> lstDPIU = objComm.PopulateDPIUOfSRRDA(PMGSYSession.Current.AdminNdCode);
                return new SelectList(lstDPIU, "Value", "Text");
            }
        }

    }
}