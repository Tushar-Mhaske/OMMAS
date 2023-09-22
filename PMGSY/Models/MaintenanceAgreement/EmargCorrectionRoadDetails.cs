using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;


namespace PMGSY.Models.MaintenanceAgreement
{
    public class EmargCorrectionRoadDetails
    {

        [UIHint("Hidden")]
        public string EncryptedRoadCode { get; set; }

        public string SaveOrUpdate { get; set; }

        public int Emarg_ID { get; set; }

        [Required(ErrorMessage = "Traffice Type is required")]
        [Range(1, int.MaxValue, ErrorMessage = "Select Traffice Type")]
        [Display(Name = "Traffice Type")]
        public int TrafficeTypeCode { get; set; }
        public List<SelectListItem> TrafficeTypeList { get; set; }

        [Required(ErrorMessage = "Carriage Width is required")]
        [Range(1, int.MaxValue, ErrorMessage = "Select Carriage Width")]
        [Display(Name = "Carriage Width")]
        public int CarriageWidthCode { get; set; }
        public List<SelectListItem> CarriageWidthList { get; set; }

        public string TrafficeTypeCodeText { get; set; }
        public decimal CarriageWidthCodeText { get; set; }


        public string RoadName { get; set; }
        public string PackageName { get; set; }
        public string StateName { get; set; }
        public string DistrictName { get; set; }
        public string BlockName { get; set; }
        public string ContractorName { get; set; }
        public string PanNumberText { get; set; }

        public string MANE_CONTRACT_FINALIZED { get; set; }
        public string MANE_LOCK_STATUS { get; set; }
        public string MANE_AGREEMENT_TYPE { get; set; }
        public string MANE_CONTRACT_STATUS { get; set; }

        //public int MANE_CONTRACT_NUMBER { get; set; }
        public Nullable<int> MANE_CONTRACT_NUMBER { get; set; }

        //public int MANE_CONTRACT_ID { get; set; }
        public Nullable<int> MANE_CONTRACT_ID { get; set; }

        public int IMS_PR_ROAD_CODE { get; set; }
        public int MANE_PR_CONTRACT_CODE { get; set; }
        public int MAST_CON_ID { get; set; }

        //[Display(Name = "Agreement Number")]
        //[RegularExpression(@"^[a-zA-Z0-9]+[a-zA-Z0-9-/()._ ]*$", ErrorMessage = "Agreement Number should contains at least one character or number and starts with alphanumeric value.")]
        //[Required(ErrorMessage = "Agreement Number is required.")]
        //[StringLength(100, ErrorMessage = "Agreement Number must be less than 100 characters.")]
        public string MANE_AGREEMENT_NUMBER { get; set; }

        //[Display(Name = "Agreement Date")]
        //[Required(ErrorMessage = "Agreement Date is required.")]
        //[RegularExpression(@"^(((0[1-9]|[12]\d|3[01])\/(0[13578]|1[02])\/((19|[2-9]\d)\d{2}))|((0[1-9]|[12]\d|30)\/(0[13456789]|1[012])\/((19|[2-9]\d)\d{2}))|((0[1-9]|1\d|2[0-8])\/02\/((19|[2-9]\d)\d{2}))|(29\/02\/((1[6-9]|[2-9]\d)(0[48]|[2468][048]|[13579][26])|((16|[2468][048]|[3579][26])00))))$", ErrorMessage = "Agreement Date must be in dd/mm/yyyy format.")]
        //public DateTime MANE_AGREEMENT_DATE { get; set; }


        //[Display(Name = "Agreement Date")]
        //[Required(ErrorMessage = "Agreement Date is required.")]
        //[RegularExpression(@"^(((0[1-9]|[12]\d|3[01])\/(0[13578]|1[02])\/((19|[2-9]\d)\d{2}))|((0[1-9]|[12]\d|30)\/(0[13456789]|1[012])\/((19|[2-9]\d)\d{2}))|((0[1-9]|1\d|2[0-8])\/02\/((19|[2-9]\d)\d{2}))|(29\/02\/((1[6-9]|[2-9]\d)(0[48]|[2468][048]|[13579][26])|((16|[2468][048]|[3579][26])00))))$", ErrorMessage = "Agreement Date must be in dd/mm/yyyy format.")]
        //  [DateValidationVST("CurrentDate", ErrorMessage = "Tender Form Issue Start Date must be greater than or equal to current date.")]
        public string MANE_AGREEMENT_DATE { get; set; }


        //[Display(Name = "Construction Completion Date")]
        //[Required(ErrorMessage = "Construction Completion Date is required.")]
        //[RegularExpression(@"^(((0[1-9]|[12]\d|3[01])\/(0[13578]|1[02])\/((19|[2-9]\d)\d{2}))|((0[1-9]|[12]\d|30)\/(0[13456789]|1[012])\/((19|[2-9]\d)\d{2}))|((0[1-9]|1\d|2[0-8])\/02\/((19|[2-9]\d)\d{2}))|(29\/02\/((1[6-9]|[2-9]\d)(0[48]|[2468][048]|[13579][26])|((16|[2468][048]|[3579][26])00))))$", ErrorMessage = "Construction Completion Date must be in dd/mm/yyyy format.")]
        //public DateTime MANE_CONSTR_COMP_DATE { get; set; }

        [Display(Name = "Construction Completion Date")]
        [Required(ErrorMessage = "Construction Completion Date is required.")]
        [RegularExpression(@"^(((0[1-9]|[12]\d|3[01])\/(0[13578]|1[02])\/((19|[2-9]\d)\d{2}))|((0[1-9]|[12]\d|30)\/(0[13456789]|1[012])\/((19|[2-9]\d)\d{2}))|((0[1-9]|1\d|2[0-8])\/02\/((19|[2-9]\d)\d{2}))|(29\/02\/((1[6-9]|[2-9]\d)(0[48]|[2468][048]|[13579][26])|((16|[2468][048]|[3579][26])00))))$", ErrorMessage = "Construction Completion Date must be in dd/mm/yyyy format.")]
        //  [DateValidationVST("CurrentDate", ErrorMessage = "Tender Form Issue Start Date must be greater than or equal to current date.")]
        public string MANE_CONSTR_COMP_DATE { get; set; }


        [Display(Name = "Maintenance Start Date")]
        [Required(ErrorMessage = "Maintenance Start Date is required.")]
        [RegularExpression(@"^(((0[1-9]|[12]\d|3[01])\/(0[13578]|1[02])\/((19|[2-9]\d)\d{2}))|((0[1-9]|[12]\d|30)\/(0[13456789]|1[012])\/((19|[2-9]\d)\d{2}))|((0[1-9]|1\d|2[0-8])\/02\/((19|[2-9]\d)\d{2}))|(29\/02\/((1[6-9]|[2-9]\d)(0[48]|[2468][048]|[13579][26])|((16|[2468][048]|[3579][26])00))))$", ErrorMessage = "Maintenance Start Date must be in dd/mm/yyyy format.")]
        //  [DateValidationVST("CurrentDate", ErrorMessage = "Tender Form Issue Start Date must be greater than or equal to current date.")]
        public string MANE_MAINTENANCE_START_DATE { get; set; }


        //[Display(Name = "Maintenance Start Date")]
        //[Required(ErrorMessage = "Maintenance Start Date is required.")]
        //[RegularExpression(@"^(((0[1-9]|[12]\d|3[01])\/(0[13578]|1[02])\/((19|[2-9]\d)\d{2}))|((0[1-9]|[12]\d|30)\/(0[13456789]|1[012])\/((19|[2-9]\d)\d{2}))|((0[1-9]|1\d|2[0-8])\/02\/((19|[2-9]\d)\d{2}))|(29\/02\/((1[6-9]|[2-9]\d)(0[48]|[2468][048]|[13579][26])|((16|[2468][048]|[3579][26])00))))$", ErrorMessage = "Maintenance Start Date must be in dd/mm/yyyy format.")]
        //public DateTime MANE_MAINTENANCE_START_DATE { get; set; }


        [Display(Name = "Maintenance End Date")]
        [Required(ErrorMessage = "Maintenance End Date is required.")]
        [RegularExpression(@"^(((0[1-9]|[12]\d|3[01])\/(0[13578]|1[02])\/((19|[2-9]\d)\d{2}))|((0[1-9]|[12]\d|30)\/(0[13456789]|1[012])\/((19|[2-9]\d)\d{2}))|((0[1-9]|1\d|2[0-8])\/02\/((19|[2-9]\d)\d{2}))|(29\/02\/((1[6-9]|[2-9]\d)(0[48]|[2468][048]|[13579][26])|((16|[2468][048]|[3579][26])00))))$", ErrorMessage = "Maintenance End Date must be in dd/mm/yyyy format.")]
        //  [DateValidationVST("CurrentDate", ErrorMessage = "Tender Form Issue Start Date must be greater than or equal to current date.")]
        public string MANE_MAINTENANCE_END_DATE { get; set; }


        //[Display(Name = "Maintenance End Date")]
        //[Required(ErrorMessage = "Maintenance End Date is required.")]
        //[RegularExpression(@"^(((0[1-9]|[12]\d|3[01])\/(0[13578]|1[02])\/((19|[2-9]\d)\d{2}))|((0[1-9]|[12]\d|30)\/(0[13456789]|1[012])\/((19|[2-9]\d)\d{2}))|((0[1-9]|1\d|2[0-8])\/02\/((19|[2-9]\d)\d{2}))|(29\/02\/((1[6-9]|[2-9]\d)(0[48]|[2468][048]|[13579][26])|((16|[2468][048]|[3579][26])00))))$", ErrorMessage = "Maintenance End Date must be in dd/mm/yyyy format.")]
        //public DateTime? MANE_MAINTENANCE_END_DATE { get; set; }

        [Display(Name = "Maintenance Cost Year1 (Rs in Lakhs)")]
        [Required(ErrorMessage = "Maintenance Cost Year1 is required.")]
        //[RegularExpression(@"^\d*(\.\d{1,2})?", ErrorMessage = "Maintenance Cost Year1 is not in valid format. ")]
        [RegularExpression(@"^\d*(\.\d{1,2})?", ErrorMessage = "Valid Decimal Number with maximum 2 decimal places. ")]
        [Range(0, 99999.99, ErrorMessage = "Maintenance Cost Year1 is not in valid format.")]
        public Nullable<decimal> MANE_YEAR1_AMOUNT { get; set; }
        //public decimal MANE_YEAR1_AMOUNT { get; set; }

        [Display(Name = "Maintenance Cost Year2 (Rs in Lakhs)")]
        [Required(ErrorMessage = "Maintenance Cost Year2 is required.")]
        [RegularExpression(@"^\d*(\.\d{1,2})?", ErrorMessage = "Valid Decimal Number with maximum 2 decimal places. ")]
        [Range(0, 99999.99, ErrorMessage = "Maintenance Cost Year2 is not in valid format.")]
        public Nullable<decimal> MANE_YEAR2_AMOUNT { get; set; }
        //public decimal MANE_YEAR2_AMOUNT { get; set; }

        [Display(Name = "Maintenance Cost Year3 (Rs in Lakhs)")]
        [Required(ErrorMessage = "Maintenance Cost Year3 is required.")]
        [RegularExpression(@"^\d*(\.\d{1,2})?", ErrorMessage = "Valid Decimal Number with maximum 2 decimal places. ")]
        [Range(0, 99999.99, ErrorMessage = "Maintenance Cost Year3 is not in valid format.")]
        public Nullable<decimal> MANE_YEAR3_AMOUNT { get; set; }
        //public decimal MANE_YEAR3_AMOUNT { get; set; }

        [Display(Name = "Maintenance Cost Year4 (Rs in Lakhs)")]
        [Required(ErrorMessage = "Maintenance Cost Year4 is required.")]
        [RegularExpression(@"^\d*(\.\d{1,2})?", ErrorMessage = "Valid Decimal Number with maximum 2 decimal places. ")]
        [Range(0, 99999.99, ErrorMessage = "Maintenance Cost Year4 is not in valid format.")]
        public Nullable<decimal> MANE_YEAR4_AMOUNT { get; set; }
        //public decimal MANE_YEAR4_AMOUNT { get; set; }

        [Display(Name = "Maintenance Cost Year5 (Rs in Lakhs)")]
        [Required(ErrorMessage = "Maintenance Cost Year5 is required.")]
        [RegularExpression(@"^\d*(\.\d{1,2})?", ErrorMessage = "Valid Decimal Number with maximum 2 decimal places. ")]
        [Range(0, 99999.99, ErrorMessage = "Maintenance Cost Year5 is not in valid format.")]
        public Nullable<decimal> MANE_YEAR5_AMOUNT { get; set; }
        //public decimal MANE_YEAR5_AMOUNT { get; set; }

        [Display(Name = "Maintenance Cost Year6 (Rs in Lakhs)")]
        [Required(ErrorMessage = "Maintenance Cost Year6 is required.")]
        [RegularExpression(@"^\d*(\.\d{1,2})?", ErrorMessage = "Valid Decimal Number with maximum 2 decimal places. ")]
        [Range(0, 99999.99, ErrorMessage = "Maintenance Cost Year6 is not in valid format.")]
        public Nullable<decimal> MANE_YEAR6_AMOUNT { get; set; }
        //public decimal MANE_YEAR6_AMOUNT { get; set; }


        //[Display(Name = "Remarks")]
        //[RegularExpression(@"^[a-zA-Z0-9]+[a-zA-Z0-9-/()._ ]*$", ErrorMessage = "Remarks Number should contains at least one character or number and starts with alphanumeric value.")]
        ////[Required(ErrorMessage = "Agreement Number is required.")]
        //[StringLength(255, ErrorMessage = "Remarks must be less than 255 characters.")]
        //public string REMARKS { get; set; }


        //[Display(Name = "Incomplete Reason")]
        //[RegularExpression(@"^[a-zA-Z0-9]+[a-zA-Z0-9-/()._ ]*$", ErrorMessage = "Incomplete Reason should contains at least one character or number and starts with alphanumeric value.")]
        ////[Required(ErrorMessage = "Agreement Number is required.")]
        //[StringLength(255, ErrorMessage = "Incomplete Reason must be less than 255 characters.")]
        //public string MANE_INCOMPLETE_REASON { get; set; }


        [Display(Name = "Value of Work Done (Rs in Lakhs)")]
        [Required(ErrorMessage = "Value of Work Done is required.")]
        //[RegularExpression(@"^\d*(\.\d{1,3})?", ErrorMessage = "Value of Work Done is not in valid format. ")]
        [RegularExpression(@"^\d*(\.\d{1,3})?", ErrorMessage = "Valid Decimal Number with maximum 3 decimal places. ")]
        [Range(0, 99999.999, ErrorMessage = "Value of Work Done is not in valid format.")]
        public decimal? MANE_VALUE_WORK_DONE { get; set; }

        [Display(Name = "Completed Length (In Kms)")]
        [Required(ErrorMessage = "Completed Length (In Kms) is required.")]
        //[RegularExpression(@"^\d*(\.\d{1,3})?", ErrorMessage = "Completed Length (In Kms) is not in valid format. ")]
        [RegularExpression(@"^\d*(\.\d{1,3})?", ErrorMessage = "Valid Decimal Number with maximum 3 decimal places. ")]
        [Range(0, 99999.999, ErrorMessage = "Completed Length (In Kms) is not in valid format.")]
        public decimal? MANE_COMPLETED_LENGTH { get; set; }

    }
}