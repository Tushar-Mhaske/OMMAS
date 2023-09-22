#region File Header
/*
        * Project Id    :
        * Project Name  :   OMMAS II
        * Name          :   WorkProgramViewModel.cs
        * Description   :   This View Model is Used in CBR Views WorkProgramAddEdit.cshtml
        * Author        :   Vikram Nandanwar        
        * Creation Date :   19/June/2013
 **/
#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace PMGSY.Models.Execution
{
    public class WorkProgramViewModel
    {
        public string Operation { get; set; }

        [Display(Name="District")]
        public string District { get; set; }
        [Display(Name="Year")]
        public string Year { get; set; }
        
        [Display(Name="Block")]
        public string Block { get; set; }
        [Display(Name = "Package Number")]
        public string PackageNumber { get; set; }
        [Display(Name = "Road Name")]
        public string RoadName { get; set; }

        [Display(Name = "Sanction Cost (in Rs. lakh)")]
        public string SanctionedCost { get; set; }

        [Display(Name = "Agreement Amount (in Rs. lakh)")]
        public string AgreementAmount { get; set; }

        [Display(Name = "Sactioned Length (in Km)")]
        public string SactionedLength { get; set; }

        public string ProposalType { get; set; }

        public string Ims_Proposal_Type { get; set; }

        public string AgreementStartDate { get; set; }
        public string AgreementEndDate { get; set; }

        public String AgreementDate { get; set; }

        public int IMS_PR_ROAD_CODE { get; set; }

        [Display(Name = "Head Items")]
        [Range(1, 2147483647, ErrorMessage = " Please select head item.")]
        public int MAST_HEAD_CODE { get; set; }

        [RegularExpression(@"^(((0[1-9]|[12]\d|3[01])\/(0[13578]|1[02])\/((19|[2-9]\d)\d{2}))|((0[1-9]|[12]\d|30)\/(0[13456789]|1[012])\/((19|[2-9]\d)\d{2}))|((0[1-9]|1\d|2[0-8])\/02\/((19|[2-9]\d)\d{2}))|(29\/02\/((1[6-9]|[2-9]\d)(0[48]|[2468][048]|[13579][26])|((16|[2468][048]|[3579][26])00))))$", ErrorMessage = "Start date is not in valid format")]
        [Display(Name = "Start Date")]
        [Required(ErrorMessage="Start Date is required")]
        public string EXEC_START_DATE { get; set; }

         [RegularExpression(@"^(((0[1-9]|[12]\d|3[01])\/(0[13578]|1[02])\/((19|[2-9]\d)\d{2}))|((0[1-9]|[12]\d|30)\/(0[13456789]|1[012])\/((19|[2-9]\d)\d{2}))|((0[1-9]|1\d|2[0-8])\/02\/((19|[2-9]\d)\d{2}))|(29\/02\/((1[6-9]|[2-9]\d)(0[48]|[2468][048]|[13579][26])|((16|[2468][048]|[3579][26])00))))$", ErrorMessage = "End date is not in valid format")]
        [Display(Name = "End Date")]
        [Required(ErrorMessage = "End Date is required")]
        [DateValidationVST("EXEC_START_DATE", ErrorMessage = "End Date must be greater than start date.")]
        public string EXEC_END_DATE { get; set; }
        
        public virtual IMS_SANCTIONED_PROJECTS IMS_SANCTIONED_PROJECTS { get; set; }
        public virtual MASTER_EXECUTION_ITEM MASTER_EXECUTION_ITEM { get; set; }
    }//end of model



    public class DateValidationVSTAttribute : ValidationAttribute, IClientValidatable
    {
        // private const string _defaultErrorMessage = "Start date must be less than end date."
        private string _basePropertyName;

        public DateValidationVSTAttribute(string basePropertyName)
        //: base(_defaultErrorMessage)
        {
            _basePropertyName = basePropertyName;
        }

        //Override default FormatErrorMessage Method  
        public override string FormatErrorMessage(string name)
        {
            return string.Format(ErrorMessageString, name, _basePropertyName);
        }

        //Override IsValid  
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            //Get PropertyInfo Object  
            var basePropertyInfo = validationContext.ObjectType.GetProperty(_basePropertyName);

            //Get Value of the property  
            var sDate = basePropertyInfo.GetValue(validationContext.ObjectInstance, null);
            var eDate = value;

            if (sDate != null && eDate != null)
            {
                var startDate = ConvertStringToDate(sDate.ToString());
                var thisDate = ConvertStringToDate(eDate.ToString());

                //Actual comparision  
                if (thisDate <= startDate)
                {
                    var message = FormatErrorMessage(validationContext.DisplayName);
                    return new ValidationResult(message);
                }
            }
            return ValidationResult.Success;
        }
        public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
        {   
            yield return new ModelClientValidationRule
            {
                ErrorMessage = FormatErrorMessage(metadata.DisplayName),
                //This is the name of the method aaded to the jQuery validator method (must be lower case)
                ValidationType = "datecomparefieldvalidator"
            };
        }

        public DateTime? ConvertStringToDate(string dateToConvert)
        {
            DateTime MyDateTime;
            MyDateTime = new DateTime();
            MyDateTime = DateTime.ParseExact(dateToConvert, "dd/MM/yyyy", null);
            return MyDateTime;
        }
    }
}