/*----------------------------------------------------------------------------------------

 * Project Id:

 * Project Name:OMMAS2

 * File Name: TeoDetailsModelForTOB.cs

 * Author : Koustubh Nakate

 * Creation Date :26/Aug/2013

 * Desc :This is model for TEO details view 
 * ---------------------------------------------------------------------------------------*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace PMGSY.Models.TransferEntryOrder
{
    public class TeoDetailsModelForTOB
    {

        public long BILL_ID { get; set; }
        public short TXN_NO { get; set; }
        public short TXN_ID { get; set; }

        [Display(Name = "Account Head")]
        [Required(ErrorMessage = "Account Head Required")]
        [Range(1, Int16.MaxValue, ErrorMessage = "Account Head Required")]
        public short HEAD_ID_C { get; set; }

        [Display(Name = "Amount")]
        [Required(ErrorMessage = "Amount is Required")]
        [RegularExpression(@"^\d{0,11}(\.\d{0,2})?$", ErrorMessage = "Only 11 Digit Whole number and 2 Decimal Allowed")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Invalid Amount")]
        public Nullable<decimal> AMOUNT_C { get; set; }

        [Display(Name = "Narration")]
        [Required(ErrorMessage = "Narration is Required")]
        [StringLength(255, ErrorMessage = "Maximum 255 Characters Allowed")]
        [RegularExpression(@"^[a-zA-Z0-9-/. ]+$", ErrorMessage = "Only Alphanumeric, Space and '-','/','.' Allowed")]
        public string NARRATION_C { get; set; }

        [Display(Name = "PIU Name")]
        public Nullable<int> ADMIN_ND_CODE_C { get; set; }

        [Display(Name = "Company Name")]
        public Nullable<int> MAST_CON_ID_C { get; set; }

        [Display(Name = "Company Name")]
        public Nullable<int> MAST_CON_ID_TRANS_C { get; set; }


        [Display(Name = "Road Name")]
        public Nullable<int> IMS_PR_ROAD_CODE_C { get; set; }

        [Display(Name = "Agreement Name")]
        public Nullable<int> IMS_AGREEMENT_CODE_C { get; set; }

        [Display(Name = "District Name")]
        public Nullable<int> MAST_DISTRICT_CODE_C { get; set; }

        [Display(Name = "Package")]
        public string IMS_PACKAGE_ID_C { get; set; }

        [Display(Name = "Sanction Year")]
        public Nullable<int> SANC_YEAR_C { get; set; }

        public string CREDIT_DEBIT { get; set; }
        public string CASH_CHQ { get; set; }

        public Nullable<int> MAS_FA_CODE_C { get; set; }

        public Nullable<int> MAS_FA_CODE_D { get; set; }

        //public Nullable<int> MAS_FA_CODE { get; set; }
        [Display(Name = "Is Final Payment")]
        public bool FINAL_PAYMENT_C { get; set; }



        //for debit
        [Display(Name = "Account Head")]
        [Required(ErrorMessage = "Account Head Required")]
        [Range(1, Int16.MaxValue, ErrorMessage = "Account Head Required")]
        public short HEAD_ID_D { get; set; }

        [Display(Name = "Amount")]
        [Required(ErrorMessage = "Amount is Required")]
        [RegularExpression(@"^\d{0,11}(\.\d{0,2})?$", ErrorMessage = "Only 11 Digit Whole number and 2 Decimal Allowed")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Invalid Amount")]
        public Nullable<decimal> AMOUNT_D { get; set; }

        [Display(Name = "Narration")]
        [Required(ErrorMessage = "Narration is Required")]
        [StringLength(255, ErrorMessage = "Maximum 255 Characters Allowed")]
        [RegularExpression(@"^[a-zA-Z0-9-/. ]+$", ErrorMessage = "Only Alphanumeric, Space and '-','/','.' Allowed")]
        public string NARRATION_D { get; set; }

        [Display(Name = "PIU Name")]
        public Nullable<int> ADMIN_ND_CODE_D { get; set; }

        [Display(Name = "Company Name")]
        public Nullable<int> MAST_CON_ID_D { get; set; }

        [Display(Name = "Company Name")]
        public Nullable<int> MAST_CON_ID_TRANS_D { get; set; }


        [Display(Name = "Road Name")]
        public Nullable<int> IMS_PR_ROAD_CODE_D { get; set; }

        [Display(Name = "Agreement Name")]
        public Nullable<int> IMS_AGREEMENT_CODE_D { get; set; }

        [Display(Name = "District Name")]
        public Nullable<int> MAST_DISTRICT_CODE_D { get; set; }

        [Display(Name = "Package")]
        public string IMS_PACKAGE_ID_D { get; set; }

        [Display(Name = "Sanction Year")]
        public Nullable<int> SANC_YEAR_D { get; set; }
 
        [Display(Name = "Is Final Payment")]
        public bool FINAL_PAYMENT_D { get; set; }


        //Added By Abhishek kamble to populate State drop down   1 Oct 2014 start
        [Display(Name = "State Name")]
        [Required(ErrorMessage="Please select State")]
        [Range(1,int.MaxValue,ErrorMessage="Please select State")]
        public int MAST_STATE_CODE_C { get; set; }


        [Display(Name = "State Name")]
        [Range(1,int.MaxValue,ErrorMessage="Please select State")]        
        [Required(ErrorMessage = "Please select State")]
        public int MAST_STATE_CODE_D { get; set; }
        //Added By Abhishek kamble to populate State drop down   1 Oct 2014 end     
  

        //Added by Abhishek kamble 9-oct-2014 for state balance transfer start

        public int _ParentTxnID { get; set; }

        //Added by Abhishek kamble 9-oct-2014 for state balance transfer start


        
    }
}