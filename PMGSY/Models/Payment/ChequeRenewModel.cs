using PMGSY.BLL.Common;
using PMGSY.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PMGSY.Models.Payment
{

    public class ChequeCancellModel
    {

        public ChequeCancellModel()
        {

            CommonFunctions objCommon = new CommonFunctions();
            this.CURRENT_DATE = objCommon.GetDateTimeToString(DateTime.Now);
                   

        }
        
        [Display(Name = "Cheque/Advice Cancellation Date")]
        [Required(ErrorMessage = "Cheque/Advice Cancellation Date is Required")]
        [RegularExpression(@"^(((0[1-9]|[12]\d|3[01])\/(0[13578]|1[02])\/((19|[2-9]\d)\d{2}))|((0[1-9]|[12]\d|30)\/(0[13456789]|1[012])\/((19|[2-9]\d)\d{2}))|((0[1-9]|1\d|2[0-8])\/02\/((19|[2-9]\d)\d{2}))|(29\/02\/((1[6-9]|[2-9]\d)(0[48]|[2468][048]|[13579][26])|((16|[2468][048]|[3579][26])00))))$", ErrorMessage = "Please Enter valid Cheque Cancellation Date")]
        [DataType(DataType.Date, ErrorMessage = "Invalid Date")]
        [IsDateAfter("CURRENT_DATE", true, ErrorMessage = "Cheque/Advice Cancellation Date must be less than or equal to today's date")]
        public String CHEQUE_CANCEL_DATE { get; set; }


        [Display(Name = "Reason for Cancellation of cheque/Advice")]
        [Required(ErrorMessage = "Reason for Cancellation of cheque/Advice is Required")]
        [StringLength(255, ErrorMessage = "Maximum 255 Characters Allowed")]
        // [RegularExpression(@"^[a-zA-Z0-9 ,-)/(. ]+$", ErrorMessage = "Only Alphanumeric, Space and '-','/','.','(',')',',' Allowed")] //added some special characters by koustubh nakate on 18/07/2013 
        [RegularExpression(@"^([a-zA-Z0-9 ./,()-]+)$", ErrorMessage = "Only Alphanumeric, Space and '-','/','.','(',')',',' Allowed")]
        public string CHEQUE_CANCEL_NARRATION { get; set; }

        [Display(Name = "Fund Type")]
       [Required(ErrorMessage = "Please select Fund Type")]
       [Range(0, Int32.MaxValue, ErrorMessage = "Invalid Fund Type")]
        public Nullable<int> CANCEL_FUND_TYPE { get; set; }
        public List<SelectListItem> CANCEL_FUND_TYPE_List { get; set; }


        //[Display(Name = "Operation")]
        //[Required(ErrorMessage = "Operation Mode is Required")]
        //[MaxLength(1, ErrorMessage = "Operation Mode can be atmost 1 characters long")]
        //public string CHQ_CANCEL_RENEW_C { get; set; }

        public string CURRENT_DATE { get; set; }

    }

    public class ChequeRenewModel
    {

        public ChequeRenewModel()
        {

            CommonFunctions objCommon = new CommonFunctions();
            this.CURRENT_DATE = objCommon.GetDateTimeToString(DateTime.Now);
            BILL_DATE = this.CURRENT_DATE;
            CHQ_DATE = this.CURRENT_DATE;
            this.CHQ_EPAY = "Q";

        }

        public string CURRENT_DATE { get; set; }

        public long BILL_ID { get; set; }

        public Nullable<short> BANK_CODE { get; set; }

        public bool IS_CHQ_ENCASHED_NA { get; set; }

        public Nullable<long> NA_BILL_ID { get; set; }

        public bool IS_CHQ_RECONCILE_BANK { get; set; }

        public Nullable<System.DateTime> CHQ_RECONCILE_DATE { get; set; }

        public string CHQ_RECONCILE_REMARKS { get; set; }

        public string CHEQUE_STATUS { get; set; }

        //[Display(Name = "ChequeBook Series")]
        //[Range(0, Int16.MaxValue, ErrorMessage = "Invalid ChequeBook Series")]
        //public string CHQ_SERIES { get; set; }
        //public List<SelectListItem> CHQ_SERIES_LIST { get; set; }

        public string PAYEE_NAME { get; set; }

        [Range(1, Int16.MaxValue, ErrorMessage = "Invalid Month")]
        public short BILL_MONTH { get; set; }

        public string CHQ_EPAY { get; set; }


        [Range(1, Int16.MaxValue, ErrorMessage = "Invalid Year")]
        public short BILL_YEAR { get; set; }

        [Display(Name = "Cheque Series")]
        //[Required(ErrorMessage = "Cheque Series is Required")]
        [Range(0, Int32.MaxValue, ErrorMessage = "Invalid Cheque Series")]
        public Nullable<int> CHQ_Book_ID { get; set; }
        public List<SelectListItem> CHQ_Book_ID_List { get; set; }


        [Display(Name = "Operation")]
        [Required(ErrorMessage = "Operation Mode is Required")]
        [MaxLength(1, ErrorMessage = "Operation Mode can be atmost 1 characters long")]
        public string CHQ_CANCEL_RENEW { get; set; }



        [Display(Name = "Voucher Date")]
        [Required(ErrorMessage = "Voucher Date is Required")]
        [RegularExpression(@"^(((0[1-9]|[12]\d|3[01])\/(0[13578]|1[02])\/((19|[2-9]\d)\d{2}))|((0[1-9]|[12]\d|30)\/(0[13456789]|1[012])\/((19|[2-9]\d)\d{2}))|((0[1-9]|1\d|2[0-8])\/02\/((19|[2-9]\d)\d{2}))|(29\/02\/((1[6-9]|[2-9]\d)(0[48]|[2468][048]|[13579][26])|((16|[2468][048]|[3579][26])00))))$", ErrorMessage = "Please Enter valid Voucher date")]
        [DataType(DataType.Date, ErrorMessage = "Invalid Date")]
        [IsDateAfter("CURRENT_DATE", true, ErrorMessage = "Voucher Date must be less than or equal to today's date")]
        [IsValidChqBookIssueDate("ChequeBookIssueDate", "CHQ_Book_ID", "CHQ_EPAY", ErrorMessage = "Voucher Date must be greater than or equal to Cheque book issue date")]
        public String BILL_DATE { get; set; }

        //added by Abhishek kamble chq issue date validation 10Mar2015
        public String ChequeBookIssueDate { get; set; }


        [Display(Name = "Voucher Number")]
        [Required(ErrorMessage = "Voucher Number is Required")]
        [RegularExpression(@"^(?=.*[0-9])[a-zA-Z0-9-/]+$", ErrorMessage = "Only Alphanumeric with atleast one number,'-','/' Allowed")]
        [StringLength(50, ErrorMessage = "Maximum {1} and Minimum {2} character Allowed", MinimumLength = 1)]
        [IsValidVoucherBillNo("BILL_MONTH", "BILL_YEAR", ErrorMessage = "Voucher Number already exists")]
        public string BILL_NO { get; set; }


        //[Display(Name = "Cheque Number")]
        //[Range(0, Int32.MaxValue, ErrorMessage = "Invalid Cheque")]
        //public string CHQ_NO_SELECTION { get; set; }
        //public List<SelectListItem> CHQ_NO_SELECTION_LIST { get; set; }


        [Display(Name = "Cheque/Advice Number")]
        [Required(ErrorMessage = "Cheque Number is Required")]
        [RegularExpression(@"^[0-9]+$", ErrorMessage = "Only Numbers Allowed")]
        //[MaxLength(6, ErrorMessage = "Cheque Numer must be 6 digit long")]
        //[MinLength(6,ErrorMessage = "Cheque Numer must be 6 digit long")]
        [StringLength(6, ErrorMessage = "Cheque Numer must be 6 digit long", MinimumLength = 6)]
        public string CHQ_NO { get; set; }



        [Display(Name = "Cheque/Advice Date")]
        [Required(ErrorMessage = "Cheque Date is Required")]
        [RegularExpression(@"^(((0[1-9]|[12]\d|3[01])\/(0[13578]|1[02])\/((19|[2-9]\d)\d{2}))|((0[1-9]|[12]\d|30)\/(0[13456789]|1[012])\/((19|[2-9]\d)\d{2}))|((0[1-9]|1\d|2[0-8])\/02\/((19|[2-9]\d)\d{2}))|(29\/02\/((1[6-9]|[2-9]\d)(0[48]|[2468][048]|[13579][26])|((16|[2468][048]|[3579][26])00))))$", ErrorMessage = "Please Enter valid cheque date")]
        [IsCheque("CHQ_EPAY", ErrorMessage = "Cheque/Epay Date is Required")]
        [IsDateAfter("CURRENT_DATE", true, ErrorMessage = "Voucher Date must be less than or equal to today's date")]
        [IsValidDate("BILL_MONTH", "BILL_YEAR", "CHQ_EPAY", ErrorMessage = "Cheque/Epay Date must be within {0} month and {1} year")]
        public String CHQ_DATE { get; set; }


        [Display(Name = "Reason")]
        [Required(ErrorMessage = "Reason is Required")]
        [StringLength(255, ErrorMessage = "Maximum 255 Characters Allowed")]
        // [RegularExpression(@"^[a-zA-Z0-9 -,/).( ]+$", ErrorMessage = "Only Alphanumeric, Space and '-','/','.' Allowed")]
        //[RegularExpression(@"^[a-zA-Z0-9 ,-)/(. ]+$", ErrorMessage = "Only Alphanumeric, Space and '-','/','.','(',')',',' Allowed")] //added some special characters by koustubh nakate on 18/07/2013  
        [RegularExpression(@"^([a-zA-Z0-9 ./,()-]+)$", ErrorMessage = "Only Alphanumeric, Space and '-','/','.','(',')',',' Allowed")]
        public string NARRATION { get; set; }

       

        public virtual ACC_BANK_DETAILS ACC_BANK_DETAILS { get; set; }
        public virtual ACC_BILL_MASTER ACC_BILL_MASTER { get; set; }

         //Added By Abhishek for Advice No 6Apr2015
        //public string CHALAN_NO { get; set; }

    }
}