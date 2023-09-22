using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace PMGSY.Models.EAuthorization
{
    public class EAuthorizationListModel
    {
        [Display(Name = "From Date")]
        [RegularExpression(@"^(((0[1-9]|[12]\d|3[01])\/(0[13578]|1[02])\/((19|[2-9]\d)\d{2}))|((0[1-9]|[12]\d|30)\/(0[13456789]|1[012])\/((19|[2-9]\d)\d{2}))|((0[1-9]|1\d|2[0-8])\/02\/((19|[2-9]\d)\d{2}))|(29\/02\/((1[6-9]|[2-9]\d)(0[48]|[2468][048]|[13579][26])|((16|[2468][048]|[3579][26])00))))$", ErrorMessage = "Please Enter valid From Date")]
        //[IsDateAfterSearchValidation("toDate", true, ErrorMessage = "From Date must be less than or equal to To Date")]
        public String fromDate { get; set; }


        [Display(Name = "To Date")]
        [RegularExpression(@"^(((0[1-9]|[12]\d|3[01])\/(0[13578]|1[02])\/((19|[2-9]\d)\d{2}))|((0[1-9]|[12]\d|30)\/(0[13456789]|1[012])\/((19|[2-9]\d)\d{2}))|((0[1-9]|1\d|2[0-8])\/02\/((19|[2-9]\d)\d{2}))|(29\/02\/((1[6-9]|[2-9]\d)(0[48]|[2468][048]|[13579][26])|((16|[2468][048]|[3579][26])00))))$", ErrorMessage = "Please Enter valid To date")]
        //[IsDateAfterSearchValidation("CURRENT_DATE", true, ErrorMessage = "To Date must be less than or equal to today's date")]
        public String toDate { get; set; }

        public string CURRENT_DATE { get; set; }

        //[Display(Name = "Transaction Type")]
        //public String TXN_ID { get; set; }

        //[Display(Name = "Cheque/EpayNumber")]
        //[RegularExpression(@"^[a-zA-Z0-9-/ ]+$", ErrorMessage = "Only Alphanumeric, Space and '-','/' Allowed")]
        //[MaxLength(30, ErrorMessage = "Cheque/EpayNumber can be atmost 30 characters long")]
        //public String Chq_Epay { get; set; }

        [Display(Name = "Authorization Status")]
        [RegularExpression(@"^[a-zA-Z]+$", ErrorMessage = "Only Alphabets Allowed")]
        [MaxLength(30, ErrorMessage = "Authorization Status can be atmost 30 characters long")]
        public String AUTH_STATUS { get; set; }


        public int SrNo { get; set; }
        public string EAUTHORIZATION_NO { get; set; }
        public String EAUTHORIZATION_DATE { get; set; }
        public Nullable<decimal> EAUTHORIZATION_AMOUNT { get; set; }
        public string EAUTHORIZATION_STATUS { get; set; }
        public string Edit { get; set; }
        public string Delete { get; set; }
        public string Finalize { get; set; }







    }
}