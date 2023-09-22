using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PMGSY.Models.EAuthorization
{
    public class EAuthorizationMasterModel
    {

        
        
         
        [Display(Name = "EAuthorization Number")]
        [Required(ErrorMessage = "EAuthorization Number is Required")]
        [MaxLength(50, ErrorMessage = "EAuthorization Number can be atmost 50 digit long")]
        [RegularExpression(@"^[a-zA-Z0-9-/]+$", ErrorMessage = "Only Alphanumeric, Space and '-','/' Allowed")]
        public string EAUTHORIZATION_NO { get; set; }

        [Display(Name = "Total Bank Authorization Available(In Lakhs.)")]
        public string TOTAL_BANK_AUTHORIZATION_AVAILABLE { get; set; }


        [Display(Name = "EAuthorization Date")]
         [Required(ErrorMessage = "EAuthorization Date is Required")]
        //[RegularExpression(@"^(((0[1-9]|[12]\d|3[01])\/(0[13578]|1[02])\/((19|[2-9]\d)\d{2}))|((0[1-9]|[12]\d|30)\/(0[13456789]|1[012])\/((19|[2-9]\d)\d{2}))|((0[1-9]|1\d|2[0-8])\/02\/((19|[2-9]\d)\d{2}))|(29\/02\/((1[6-9]|[2-9]\d)(0[48]|[2468][048]|[13579][26])|((16|[2468][048]|[3579][26])00))))$", ErrorMessage = "Please Enter valid Voucher date")]
        //[DataType(DataType.Date, ErrorMessage = "Invalid Date")]
        //[IsDateAfter("CURRENT_DATE", true, ErrorMessage = "Voucher Date must be less than or equal to today's date")]
        //[IsValidDate("BILL_MONTH", "BILL_YEAR", "CHQ_EPAY", ErrorMessage = "Voucher Date must be within selected month and year")]
        //[IsValidChqBookIssueDate("ChequeBookIssueDate", "CHQ_Book_ID", "CHQ_EPAY", ErrorMessage = "Voucher Date must be greater than or equal to Cheque book issue date")]
         public String EAUTHORIZATION_DATE { get; set; }


        //[RegularExpression(@"^\d{0,11}(\.\d{0,2})?$", ErrorMessage = "Only 11 Digit Whole number and 2 Decimal Allowed")]
        //[Display(Name = "Request Amount")]
        //[Range(0.01, Double.MaxValue, ErrorMessage = "Please Enter Valid EAuthorization Request Amount")]
        
        //[Required(ErrorMessage = "Amount is Required")]
        public Nullable<decimal> EAUTHORIZATION_AMOUNT { get; set; }



        [Required(ErrorMessage = "Month is Required")]
        public short BILL_MONTH { get; set; }
        public List<SelectListItem> BILL_YEAR_List { get; set; }

        [Required(ErrorMessage = "Year is Required")]
        public short BILL_YEAR { get; set; }
        public List<SelectListItem> BILL_MONTH_List { get; set; }
        
        public String BILL_DATE { get; set; }

        public string CURRENT_DATE { get; set; }

        public string FundType { get; set; }
        public short LevelID { get; set; }
        public int AdminNDCode { get; set; }
        public int UserID { get; set; }

        public string Operation { get; set; }
        public Int32 Auth_Id { get; set; }

        public bool status{get;set;}
        public string StatusMessage { get; set; }




        //*******************************************************
        [Display(Name = "Company Name (Contractor)")]
        [RegularExpression(@"^[0-9]+$", ErrorMessage = "invalid Company (Contractor)")]
        [Range(0, Int32.MaxValue, ErrorMessage = "Invalid Company (Contractor)")]
        public Nullable<int> MAST_CON_ID_CON { get; set; }
        public List<SelectListItem> mast_CON_ID_CON1 { get; set; }

        public long BILL_ID { get; set; }
        public short TXN_NO { get; set; }
        public string HEAD_ID { get; set; }
        public Nullable<decimal> AMOUNT { get; set; }

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

        [Display(Name = "Cheque Amount")]
        [RegularExpression(@"^\d{0,11}(\.\d{0,2})?$", ErrorMessage = "Only 11 Digit Whole number and 2 Decimal Allowed")]
        public Nullable<decimal> AMOUNT_Q { get; set; }

        [Display(Name = "Cash Amount")]
        [RegularExpression(@"^\d{0,11}(\.\d{0,2})?$", ErrorMessage = "Only 11 Digit Whole number and 2 Decimal Allowed")]
        public Nullable<decimal> AMOUNT_C { get; set; }

        public string CREDIT_DEBIT { get; set; }

        public string CASH_CHQ { get; set; }

        [Display(Name = "Sanction Year")]
        public int SANCTION_YEAR { get; set; }
        public List<SelectListItem> IMS_SANCTION_YEAR_List { get; set; }


        [Display(Name = "Sanction Packages")]
        public String SANCTION_PACKAGE { get; set; }
        public List<SelectListItem> IMS_SANCTION_PACKAGE_List { get; set; }


        [Display(Name = "Narration")]
        // [Required(ErrorMessage = "Narration is Required")]
        [StringLength(255, ErrorMessage = "Maximum 255 Characters Allowed")]
        //[RegularExpression(@"^[a-zA-Z0-9-/()., ]+$", ErrorMessage = "Only Alphanumeric, Space and '-','/','.',',','(',')' Allowed")]

        [RegularExpression("^([a-zA-Z0-9 -.,/\r\n()]+)", ErrorMessage = "Only Alphanumeric, Space and '-','/','.','(',')',',' Allowed")]//added by abhishek kamble 11-nov-2013

        public string NARRATION_P { get; set; }


        [Display(Name = "Narration")]
        // [Required(ErrorMessage = "Narration is Required")]
        [StringLength(255, ErrorMessage = "Maximum 255 Characters Allowed")]
        [RegularExpression(@"^[a-zA-Z0-9-/()., ]+$", ErrorMessage = "Only Alphanumeric, Space and '-','/','.',',','(',')' Allowed")]
        public string NARRATION_D { get; set; }

        [Display(Name = "Road ")]
        public Nullable<int> IMS_PR_ROAD_CODE { get; set; }
        public List<SelectListItem> IMS_PR_ROAD_CODEList { get; set; }

        [Display(Name = "Agreement Name")]
        //[Required(ErrorMessage = "Agreement Name is Required")]
        //[Range(1, 2147483647, ErrorMessage = "Please Select Agreement")]
        public Nullable<int> IMS_AGREEMENT_CODE { get; set; }


        [Display(Name = "Agreement Name (Supplier)")]
        public Nullable<int> IMS_AGREEMENT_CODE_S { get; set; }
        public List<SelectListItem> AGREEMENT_S { get; set; }

        [Display(Name = "Agreement Name (Contractor) ")]
        public Nullable<int> IMS_AGREEMENT_CODE_C { get; set; }
        public List<SelectListItem> AGREEMENT_C { get; set; }

        [Display(Name = " Implementing Agency / Department ")]
        public Nullable<int> MAST_DPIU_CODE { get; set; }
        public List<SelectListItem> MAST_DPIU_CODEList { get; set; }

        [Display(Name = "District ")]
        public Nullable<int> MAST_DISTRICT_CODE { get; set; }
        public List<SelectListItem> MAST_DISTRICT_CODEList { get; set; }



        [Display(Name = "Is Final Payment ")]
        public Nullable<bool> FINAL_PAYMENT { get; set; }

        public List<SelectListItem> final_pay { get; set; }

        public Nullable<int> ND_CODE { get; set; }

        public Nullable<int> CON_ID { get; set; }


        [Display(Name = "Agreement Name (Deduction)")]
        public Nullable<int> IMS_AGREEMENT_CODE_DED { get; set; }
        public List<SelectListItem> AGREEMENT_DED { get; set; }

        public string CON_REQ { get; set; }
        public string AGREEMENT_REQ { get; set; }
        public string ROAD_REQ { get; set; }
        public string PACKAGE_REQ { get; set; }
        public string PIU_REQ { get; set; }
        public string SUPPLIER_REQ { get; set; }
        public string FUND_TYPE { get; set; }
        public short LVL_ID { get; set; }
        public string BILL_TYPE { get; set; }

        public Nullable<int> CONTRACTOR_ID { get; set; }


        public virtual ACC_BILL_MASTER ACC_BILL_MASTER { get; set; }
        public virtual ACC_MASTER_HEAD ACC_MASTER_HEAD { get; set; }
        public virtual MASTER_CONTRACTOR MASTER_CONTRACTOR { get; set; }

        

        //************************************************************


    }
}