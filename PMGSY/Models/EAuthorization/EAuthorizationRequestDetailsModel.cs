using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;


namespace PMGSY.Models.EAuthorization
{
    public class EAuthorizationRequestDetailsModel
    {
        public EAuthorizationRequestDetailsModel()
        {

        }

        public Int32 AdminND_Code { get; set; }
        public long EAUTH_ID { get; set; }
        public short TXN_NO { get; set; }
        public short TXN_ID { get; set; }
        public short HEAD_ID { get; set; }
        public decimal AMOUNT { get; set; }
        public string CREDIT_DEBIT { get; set; }
        public string CASH_CHQ { get; set; }
        public string ErrMessage { get; set; }
        public string ApprovalStatus { get; set; }
        public Nullable<int> MAST_CON_ID { get; set; }
        public int count { get; set; }

        public Nullable<int> IMS_AGREEMENT_CODE { get; set; }
        public Nullable<int> MAS_FA_CODE { get; set; }
        public Boolean IsForUpdate { get; set; }

        public Boolean status { get; set; }

        public string EAUTH_NO { get; set; }

        public string IMS_PACKAGE_ID { get; set; }

        [Display(Name = "Payee Name ")]
        [Required(ErrorMessage = "Payee Name is Required")]
        public string PAYEE_NAME { get; set; }


        [Display(Name = "Sub Transaction Type (Payment)")]
        //[Required(ErrorMessage = "Sub Transaction is Required")]
        public string HEAD_ID_P { get; set; }
        public List<SelectListItem> HeadId_P { get; set; }

        [Display(Name = "Sub Transaction Type (Deduction)")]
        //[Required(ErrorMessage = "Sub Transaction is Required")]
        public string HEAD_ID_D { get; set; }
        public List<SelectListItem> HeadId_D { get; set; }

        [Display(Name = "Deduction Amount")]
        // [Required(ErrorMessage = "Deduction Amount is Required")]
        [RegularExpression(@"^\d{0,11}(\.\d{0,2})?$", ErrorMessage = "Only 11 Digit Whole number and 2 Decimal Allowed")]
        public Nullable<decimal> AMOUNT_D { get; set; }


        //1
        [Display(Name = "Agreement Amount(In Lakhs.)")]
        [RegularExpression(@"^\d{0,11}(\.\d{0,2})?$", ErrorMessage = "Only 11 Digit Whole number and 2 Decimal Allowed")]
        public Nullable<decimal> AGREEMENT_AMOUNT { get; set; }

        [Display(Name = "Expenditure Amount(In Lakhs.)")]
        public Nullable<decimal> EXPENDITURE_AMOUNT { get; set; }


        //2
        [Display(Name = "Amount already Authorized(In Lakhs.)")]
        [Required(ErrorMessage = "Amount already Authorized is Required")]
        //[RegularExpression(@"^\d{0,11}(\.\d{0,2})?$", ErrorMessage = "Only 11 Digit Whole number and 2 Decimal Allowed")]
        public Nullable<decimal> ALREADY_AUTHORISED_AMOUNT { get; set; }


        //3
        [Display(Name = "Authorization Requested Amount(In Lakhs.)")]
        [Required(ErrorMessage = "Authorization Requested Amount is Required")]
        [RegularExpression(@"^\d{0,11}(\.\d{0,2})?$", ErrorMessage = "Onl  11 Digit Whole number and 2 Decimal Allowed")]
        public Nullable<decimal> AMOUNT_Q { get; set; }

        

        



        [Display(Name = "Cash Amount")]
        [RegularExpression(@"^\d{0,11}(\.\d{0,2})?$", ErrorMessage = "Only 11 Digit Whole number and 2 Decimal Allowed")]
        public Nullable<decimal> AMOUNT_C { get; set; }

        [Display(Name = "Sanction Year")]
        public int IMS_SANCTION_YEAR { get; set; }
        public List<SelectListItem> IMS_SANCTION_YEAR_List { get; set; }


        [Display(Name = "Package Number")]
        [Required(ErrorMessage = "Package Number is Required")]
        public String IMS_SANCTION_PACKAGE { get; set; }
        public List<SelectListItem> IMS_SANCTION_PACKAGE_List { get; set; }


        [Display(Name = "Narration")]
        // [Required(ErrorMessage = "Narration is Required")]
        [StringLength(255, ErrorMessage = "Maximum 255 Characters Allowed")]
        [RegularExpression(@"^[a-zA-Z0-9-/()&., ]+$", ErrorMessage = "Only Alphanumeric, Space and '-','/','.',',','(',')' Allowed")]
        public string NARRATION_P { get; set; }


        [Display(Name = "Narration")]
        // [Required(ErrorMessage = "Narration is Required")]
        [StringLength(255, ErrorMessage = "Maximum 255 Characters Allowed")]
        [RegularExpression(@"^[a-zA-Z0-9-/()&., ]+$", ErrorMessage = "Only Alphanumeric, Space and '-','/','.',',','(',')' Allowed")]
        public string NARRATION_D { get; set; }

        [Display(Name = "Road ")]
        public Nullable<int> IMS_PR_ROAD_CODE { get; set; }
        public List<SelectListItem> IMS_PR_ROAD_CODEList { get; set; }


        [Display(Name = "Agreement Number ")]
        public Nullable<int> IMS_AGREEMENT_CODE_C { get; set; }
        public List<SelectListItem> AGREEMENT_C { get; set; }

        [Display(Name = "DPIU ")]
        public Nullable<int> MAST_DPIU_CODE { get; set; }
        public List<SelectListItem> MAST_DPIU_CODEList { get; set; }




        [Display(Name = "Is Final Payment ")]
        public Nullable<bool> FINAL_PAYMENT { get; set; }
        public List<SelectListItem> final_pay { get; set; }


        [Display(Name = "Agreement Name (Deduction)")]
        public Nullable<int> IMS_AGREEMENT_CODE_DED { get; set; }
        public List<SelectListItem> AGREEMENT_DED { get; set; }

        public Int32 CONTRACTOR_ID { get; set; }

        public virtual ACC_MASTER_HEAD ACC_MASTER_HEAD { get; set; }
        public virtual ACC_MASTER_TXN ACC_MASTER_TXN { get; set; }
        public virtual IMS_SANCTIONED_PROJECTS IMS_SANCTIONED_PROJECTS { get; set; }
        public virtual MASTER_CONTRACTOR MASTER_CONTRACTOR { get; set; }
        public virtual MASTER_STREAMS MASTER_STREAMS { get; set; }
        public virtual TEND_AGREEMENT_MASTER TEND_AGREEMENT_MASTER { get; set; }


        [Display(Name = "Company Name (Contractor)")]
        [Required(ErrorMessage = "Company Name (Contractor) is Required")]
        [RegularExpression(@"^[0-9]+$", ErrorMessage = "invalid Company (Contractor)")]
        [Range(0, Int32.MaxValue, ErrorMessage = "Invalid Company (Contractor)")]
        public Nullable<int> MAST_CON_ID_C { get; set; }
        public List<SelectListItem> MAST_CON_ID_C1 { get; set; }


    }
}