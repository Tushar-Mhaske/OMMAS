using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using System.Text.RegularExpressions;
using PMGSY.Common;

namespace PMGSY.Models.Feedback
{
    public class SearchFeedback
    {
        //[Required(ErrorMessage = "Token Number Required")]
        //[TokenValidation("FBToken", "contactDetails", ErrorMessage = "Please Enter/Select Token or Contact Details")]
        [RegularExpression(@"^([a-zA-Z0-9 ]+)$", ErrorMessage = "Token Number should be Alpha-Numeric")]
        public string FBToken { get; set; }

        [Display(Name = "View/Search Feedback")]
        public string Title { get; set; }

        //[Range(1, int.MaxValue, ErrorMessage = "Please Select Search Details")]
        [TokenValidation("FBToken", "contactDetails", ErrorMessage = "Please Enter/Select Token or Contact Details")]
        public string searchDetails { get; set; }

        public string hdnsearchDetails { get; set; }

        public string sType { get; set; }
        public List<SelectListItem> searchDetailsList { get; set; }


        public string tokendetails { get; set; }

        //[TokenValidation("FBToken", "contactDetails", ErrorMessage = "Please Enter/Select Token or Contact Details")]Please Enter either Alphabets or Numbers
        [Display(Name="Enter Contact Details")]
        [RegularExpression(@"^([a-zA-Z0-9@._ ]+)$", ErrorMessage = "Invalid characters entered")]
        public string contactDetails { get; set; }

        [Display(Name = "Enter Mobile Number")]
        public string mob { get; set; }

        [Display(Name="Enter Telephone Number")]
        public string telno { get; set; }

        [Display(Name="Enter Email Id")]
        public string email { get; set; }

        public string RepStatus { get; set; }
        public string FinalRep { get; set; }

        public List<SearchFeedReply> statusList { get; set; }

        public string feedsub { get; set; }
        public string feedcomm { get; set; }
        public string feeddate { get; set; }

        public string feedstatus { get; set; }

        public int feedbackId { get; set; }
        public List<feedListing> feedList { get; set; }

        #region
        //[Display(Name = "Feedback Type")]
        public string FB_Type { set; get; }

        //[Display(Name = "Name")]
        public string FName { get; set; }

        [Display(Name = "Telephone")]
        public string FTel { get; set; }

        [Display(Name = "Mobile")]
        public string FMob { get; set; }

        [Display(Name = "Email")]
        public string FEmail { get; set; }

        [Display(Name = "Date")]
        public string FDate { get; set; }

        [Display(Name = "Category")]
        public string FCategory { get; set; }

        [Display(Name = "Against")]
        public string FAgainst { get; set; }

        [Display(Name = "State")]
        public string FState { get; set; }

        [Display(Name = "District")]
        public string FDistrict { get; set; }

        [Display(Name = "Block")]
        public string FBlock { get; set; }

        [Display(Name = "For")]
        public string FFor { get; set; }

        [Display(Name = "Comments")]
        public string FComments { get; set; }

        [Display(Name = "PMGSY Roads")]
        public string PMGSYRoads { get; set; }

        [Display(Name = "Village Name")]
        public string VillageName { get; set; }

        [Display(Name = "Nearest Habitation")]
        public string NearestHabitation { get; set; }

        [Display(Name = "Road Name")]
        public string RoadName { get; set; }

        public string hdnfeedId { get; set; }

        public string RH_Name { get; set; }
        #endregion
    }

    public class feedListing
    {
        public string feedToken { get; set; }
        public string feedsubject { get; set; }
        public int feedId { get; set; }
        [Display(Name="Sr.No.")]
        public string SrNo { get; set; }

        public string name { get; set; }

        ////[LocalizedDisplayNewsFeedbackName("News Upload Date")]
        public string Date { get; set; }

        public string fbAgainst { get; set; }
        public string fbStatus { get; set; }
        public string feedComment { get; set; }
    }

    public class SearchFeedReply
    {
        public string feedsub { get; set; }
        public string feedcomm { get; set; }

        public string repstat { get; set; }
        public string repdate { get; set; }
        public string repcomment { get; set; }
    }

    #region Custom Validation for two fields
    //public class TokenValidation : ValidationAttribute , IClientValidatable
    //{
    //    private readonly string txtToken;
    //    private readonly string txtContact;
    //    //private readonly string WorkStatus;

    //    public TokenValidation(string txtToken, string txtContact)
    //    {
    //        this.txtToken = txtToken;
    //        this.txtContact = txtContact;
    //        // this.WorkStatus = workStatus;
    //    }

    //    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    //    {
    //        var txtToken = validationContext.ObjectType.GetProperty(this.txtToken);
    //        var txtContact = validationContext.ObjectType.GetProperty(this.txtContact);

    //        var txtTokenValue = txtToken.GetValue(validationContext.ObjectInstance, null);
    //        var txtContactValue = txtContact.GetValue(validationContext.ObjectInstance, null);

    //        //var comparisonValue = value;

    //        if (txtToken == null && txtContact == null)
    //        {
    //            return new ValidationResult("Please Enter Token or Contact Details");
    //        }
    //        else
    //        {
    //            return ValidationResult.Success;
    //        }
    //    }

    //    public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
    //    {
    //        yield return new ModelClientValidationRule
    //        {
    //            ErrorMessage = this.ErrorMessage,
    //            ValidationType = "customvalidator"
    //        };
    //    }
    //}
    #endregion

    public class TokenValidation : ValidationAttribute, IClientValidatable
    {
        private readonly string txtToken;
        private readonly string txtContact;
        //private readonly string WorkStatus;

        public TokenValidation(string txtToken, string txtContact)
        {
            this.txtToken = txtToken;
            this.txtContact = txtContact;
            // this.WorkStatus = workStatus;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var txtToken = validationContext.ObjectType.GetProperty(this.txtToken);
            var txtContact = validationContext.ObjectType.GetProperty(this.txtContact);

            var txtTokenValue = txtToken.GetValue(validationContext.ObjectInstance, null);
            var txtContactValue = txtContact.GetValue(validationContext.ObjectInstance, null);

            var comparisonValue = value;

            if (txtTokenValue == null && txtContactValue == null)
            {
                return new ValidationResult("Please Enter Token or Contact Details");
            }
            else if (txtTokenValue == null && txtContactValue != null)
            {
                if (Convert.ToString(comparisonValue) == "1")
                {
                    Regex regex = new Regex(@"^([0-9 ]+)$");

                    if (!regex.IsMatch(Convert.ToString(txtContactValue).Trim()) && (Convert.ToString(txtContactValue).Trim().Length != 10))
                    {
                        return new ValidationResult("Mobile Number should be a 10 digit number");
                    }
                }
                else if (Convert.ToString(comparisonValue) == "2")
                {
                    Regex regex = new Regex(@"^([0-9 ]+)$");

                    if (!regex.IsMatch(Convert.ToString(txtContactValue).Trim()) && (Convert.ToString(txtContactValue).Trim().Length < 9 && Convert.ToString(txtContactValue).Trim().Length > 13))
                    {
                        return new ValidationResult("Telephone number should be 9 to 13 digit number");
                    }
                }
                //if (Convert.ToString(comparisonValue) == "1")
                else if (Convert.ToString(comparisonValue) == "3")
                {
                    //Regex regex = new Regex(@"^([a-zA-Z0-9@, ]{}+)$");
                    Regex regex = new Regex(@"^[a-zA-Z0-9._-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,4}$");

                    if (!regex.IsMatch(Convert.ToString(txtContactValue).Trim()))
                    {
                        return new ValidationResult("Please enter email id in proper format");
                    }
                }
                ///Added for Name and Feedback
                else if (Convert.ToString(comparisonValue) == "4")
                {
                    //Regex regex = new Regex(@"^([a-zA-Z0-9@, ]{}+)$");
                    Regex regex = new Regex(@"^([a-zA-Z'. ]+)$");

                    if (!regex.IsMatch(Convert.ToString(txtContactValue).Trim()))
                    {
                        return new ValidationResult("Name is not in valid format.");
                    }
                    if (Convert.ToString(txtContactValue).Trim().Length > 50)
                    {
                        return new ValidationResult("Name should not be greater than 50 characters.");
                    }
                }
                else if (Convert.ToString(comparisonValue) == "5")
                {
                    //Regex regex = new Regex(@"^([a-zA-Z0-9@, ]{}+)$");
                    Regex regex = new Regex(@"^([-0-9a-zA-Z'.,:\n\r ]+)$");

                    if (!regex.IsMatch(Convert.ToString(txtContactValue).Trim()))
                    {
                        return new ValidationResult("Feedback is not in valid format.");
                    }
                    if (Convert.ToString(txtContactValue).Trim().Length > 8000)
                    {
                        return new ValidationResult("Feedback should not be greater than 8000 characters.");
                    }
                }
            }
            else
            {
                return ValidationResult.Success;
            }
            return ValidationResult.Success;
        }

        public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
        {
            yield return new ModelClientValidationRule
            {
                ErrorMessage = this.ErrorMessage,
                ValidationType = "customvalidator"
            };
        }
    }
}