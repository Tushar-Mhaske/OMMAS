using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace PMGSY.Models.Feedback
{
    public class FBApprovalDetails
    {
        [Display(Name = "Approve Date")]
        public string Appr_Date { set; get; }

        //[Range(typeof(bool), "true", "true", ErrorMessage = "Please select Yes for Approval")]
        //[BooleanRequired(ErrorMessage = "Please select Yes for Approval")]
        public bool ApprValue { get; set; }

        [Display(Name = "Approval")]
        //[Required(ErrorMessage="Please select Yes for Approval")]
        //[RegularExpression("[Y]", ErrorMessage = "Please select Yes for Approval")]
        //[Range(typeof(bool), "true", "true")]

        [RegularExpression("[YN]", ErrorMessage = "Please Select Yes or No")]
        public string Approval { get; set; }

        public string Repstat { get; set; }

        public string ApprovalDisplay { get; set; }

        public int hdnfeedId { get; set; }

        [Display(Name = "State")]
        public int State { get; set; }
        public List<SelectListItem> State_List { set; get; }

        [Display(Name = "District")]
        public int District { get; set; }
        public List<SelectListItem> District_List { set; get; }

        //[Required(ErrorMessage = "Please enter comments for Reply")]
        [StringLength(8000, ErrorMessage = "Reason is not greater than 8000 characters.")]
        [RegularExpression(@"^([-0-9a-zA-Z,.@()/ ]+)$", ErrorMessage = "Reason has some invalid characters.")]
        public string Rep_ApprComments { get; set; }

        public long? CitizenId { get; set; }

        public bool IS_PMGSY_ROAD { get; set; }
    }

    public class BooleanRequiredAttribute : ValidationAttribute, IClientValidatable
    {
        public override bool IsValid(object value)
        {
            if (value is bool)
                return (bool)value;
            else
                return true;
        }

        public IEnumerable<ModelClientValidationRule> GetClientValidationRules(
            ModelMetadata metadata,
            ControllerContext context)
        {
            yield return new ModelClientValidationRule
            {
                ErrorMessage = FormatErrorMessage(metadata.GetDisplayName()),
                ValidationType = "booleanrequired"
            };
        }
    }

}