using PMGSY.BAL.AssetDetails;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PMGSY.Models.AssetDetails
{
    public class AssetDetailsViewModel
    {

        public AssetDetailsViewModel()
        {
            ddlAssets = new List<SelectListItem>();
        }

        public int Details_Id { get; set; }
        public long Bill_Id { get; set; }
        public short Head_Id { get; set; }
        public string EncryptedDetailsId { get; set; }

        [Required(ErrorMessage="Please select Asset.")]
        [Display(Name="Asset")]
        [Range(1,Int16.MaxValue,ErrorMessage="Please select Asset.")]
        public short Asset_Id { get; set; }

        [Display(Name="Serial No")]
        [RegularExpression(@"^([a-zA-Z0-9/]+){1,30}$", ErrorMessage = "Please Enter Valid Serial No.")]
        public string Serial_No { get; set; }

        [Display(Name="Model No")]
        [RegularExpression(@"^([a-zA-Z0-9/]+){1,30}$", ErrorMessage = "Please Enter Valid Model No.")]
        public string Model_No { get; set; }

        [Required(ErrorMessage="Quantity is required.")]
        [Display(Name="Quantity")]
        [Range(1,Int16.MaxValue,ErrorMessage="Please enter valid quantity.")]
        public short Quantity { get; set; }

        [Required(ErrorMessage="Rate is required.")]
        [RegularExpression(@"^\s*(?=.*[0-9])\d*(?:\.\d{1,2})?\s*$", ErrorMessage = "Invalid Value of rate,Can only contains 24 Numeric digits and 2 digit after decimal place")]
        [Display(Name="Rate")]
        [Range(0.01,9999999999999999999999.99,ErrorMessage="Please enter valid Rate.")]
        public decimal Rate { get; set; }

        [Required(ErrorMessage="Total Amount is required.")]
        [Display(Name="Total Amount")]
        [Range(0.01, 9999999999999999999999.99, ErrorMessage = "Total Amount is invalid.")]
        [RegularExpression(@"^\s*(?=.*[0-9])\d*(?:\.\d{1,2})?\s*$", ErrorMessage = "Invalid Value of Total Amount,Can only contains 24 Numeric digits and 2 digit after decimal place")]
        public decimal Total_Amount { get; set; }

        [Display(Name="Assigned Id")]
        [RegularExpression(@"^([a-zA-Z0-9/]+){1,30}$", ErrorMessage = "Please Enter Valid Assigned Id.")]
        public string Assigned_Id { get; set; }

        [Display(Name="Disposal Date")]
        [RegularExpression(@"^(((0[1-9]|[12]\d|3[01])\/(0[13578]|1[02])\/((19|[2-9]\d)\d{2}))|((0[1-9]|[12]\d|30)\/(0[13456789]|1[012])\/((19|[2-9]\d)\d{2}))|((0[1-9]|1\d|2[0-8])\/02\/((19|[2-9]\d)\d{2}))|(29\/02\/((1[6-9]|[2-9]\d)(0[48]|[2468][048]|[13579][26])|((16|[2468][048]|[3579][26])00))))$", ErrorMessage = "Disposal Date must be in dd/mm/yyyy format.")]
        public string Disposal_Date { get; set; }

        public bool IsFinalize { get; set; }

        public string Operation { get; set; }

        public List<SelectListItem> ddlAssets { get; set; }

        public string Urlparameter { get; set; }

        [CompareTotalAmount("Head_Id", "Bill_Id", "Total_Amount", "Operation", "EncryptedDetailsId", ErrorMessage = "Total Amount of asset details is more than Total Asset Amount.")]
        public decimal TotalAssetAmount { get; set; }

        public decimal? TotalRemainingAmount { get; set; }

        public string VoucherDate { get; set; }

        public string VoucherNo { get; set; }

        public string HeadDesc { get; set; }

    }


    public class CompareTotalAmount : ValidationAttribute, IClientValidatable
    {
        private readonly string headId;
        private readonly string billId;
        private readonly string totalAmount;
        private readonly string operation;
        private readonly string detailsId;

        public CompareTotalAmount(string headId, string billId,string totalAmount,string operation,string detailsId)
        {
            this.headId = headId;
            this.billId = billId;
            this.totalAmount = totalAmount;
            this.operation = operation;
            this.detailsId = detailsId;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var propertyTestedHeadId = validationContext.ObjectType.GetProperty(this.headId);
            var propertyTestedBiliId = validationContext.ObjectType.GetProperty(this.billId);
            var propertyTestedTotalAmount = validationContext.ObjectType.GetProperty(this.totalAmount);
            var propertyTestedOperation = validationContext.ObjectType.GetProperty(this.operation);
            var propertyTestedDetailsId = validationContext.ObjectType.GetProperty(this.detailsId);

            if (propertyTestedHeadId == null)
            {
                return new ValidationResult(string.Format("unknown property {0}", this.headId));
            }

            var headId = Convert.ToInt32(propertyTestedHeadId.GetValue(validationContext.ObjectInstance, null));
            var billId = Convert.ToInt32(propertyTestedBiliId.GetValue(validationContext.ObjectInstance, null));
            var latestTotalAmount = Convert.ToDecimal(propertyTestedTotalAmount.GetValue(validationContext.ObjectInstance, null));
            var totalAmount = Convert.ToDecimal(value);
            var operation = propertyTestedOperation.GetValue(validationContext.ObjectInstance, null);
            var detailsId = propertyTestedDetailsId.GetValue(validationContext.ObjectInstance, null);
            if(detailsId == null)
            {
                detailsId = string.Empty;
            }

            IAssetDetailsBAL objBAL = new AssetDetailsBAL();
            bool status = objBAL.CheckTotalAmount(headId, billId, totalAmount,latestTotalAmount,operation.ToString(),detailsId.ToString());
            if (status)
            {
                return ValidationResult.Success;
            }
            else
            {
                return new ValidationResult(FormatErrorMessage(validationContext.DisplayName));
            }
        }

        public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
        {
            var rule = new ModelClientValidationRule
            {
                ErrorMessage = this.ErrorMessageString,
                ValidationType = "comparetotalamount"
            };
            yield return rule;
        }

    }

}