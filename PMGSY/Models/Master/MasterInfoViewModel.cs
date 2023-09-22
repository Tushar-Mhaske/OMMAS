using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace PMGSY.Models.Master
{
    public class MasterInfoViewModel
    {
        [UIHint("hidden")]
        public string EncryptedInfoCode { get; set; }

        public int MAST_INFO_CODE { get; set; }

        [Display(Name="Name")]
        [Required(ErrorMessage="Please enter name.")]
        [RegularExpression(@"^([a-zA-Z .-]+)$",ErrorMessage="Only Alphabets are allowed")]
        [StringLength(255, ErrorMessage = "Name must be less than 255 characters.")]
        public string MAST_INFO_NAME { get; set; }

        [Display(Name = "Designation")]
        [Required(ErrorMessage = "Please enter designation.")]        
        [RegularExpression(@"^([a-zA-Z ._&(),-]+)$", ErrorMessage = "Only Alphabets and .()-& characters are allowed.")]        
        [StringLength(255, MinimumLength = 1, ErrorMessage = "Designation must be less than 255 characters.")]
        public string MAST_INFO_DESIGNATION { get; set; }

        [Display(Name = "Office")]
        [Required(ErrorMessage = "Please enter office.")]        
        [RegularExpression(@"^([a-zA-Z0-9 ._&(),-]+)$", ErrorMessage = "Only Alphanumeric and .()-& characters are allowed.")]                
        [StringLength(255,ErrorMessage = "Office must be less than 255 characters.")]
        public string MAST_INFO_OFFICE { get; set; }

        [Display(Name = "State")]        
        [RegularExpression(@"^([0-9]+)$", ErrorMessage = "Invalid State.")]
        [RequredInfoState("MAST_INFO_TYPE", ErrorMessage = "Please select state.")]      
        public Nullable<int> MAST_STATE_CODE { get; set; }


        [Display(Name = "Tel. Office")]        
        [RegularExpression(@"^([a-zA-Z0-9 ,-]+)$", ErrorMessage = "Please enter correct office telephone number.")]        

        public string MAST_INFO_TELE_OFF { get; set; }

        [Display(Name = "Tel. Residential")]        
        [RegularExpression(@"^([0-9-]+)$", ErrorMessage = "Please enter correct residential telephone number.")]        
        public string MAST_INFO_TELE_RES { get; set; }

        [Display(Name = "Mobile")]        
        [RegularExpression(@"^([0-9-]+)$", ErrorMessage = "Please enter correct mobile number.")]        
        [StringLength(13, ErrorMessage = "Incorrect Phone Number.")]                
        public string MAST_INFO_MOBILE { get; set; }

        [Display(Name = "Fax")]        
        [RegularExpression(@"^([0-9-]+)$", ErrorMessage = "Invalid Fax Number.")]        
        [StringLength(13,ErrorMessage = "Fax Number must be 10 to 13 digits only.")]                           
        public string MAST_INFO_FAX { get; set; }
                
        [Display(Name = "Email")]
        [EmailAddress(ErrorMessage = "Email is not in valid format.")]
        [StringLength(100, ErrorMessage = "Email address must be less than 100 characters.")]        
        public string MAST_INFO_EMAIL { get; set; }

        [Display(Name = "Type")]
        [RegularExpression("[MNTA]", ErrorMessage = "Please select Type.")]        
        public string MAST_INFO_TYPE { get; set; }

        [Display(Name = "Status")]        
        public string MAST_INFO_ACTIVE { get; set; }

        [Display(Name = "Sort Order")]
        [Required(ErrorMessage = "Please enter sort order.")]
        [RegularExpression(@"^([0-9]+)$", ErrorMessage = "Please enter correct sort order number.")]       
        public int MAST_SORT_ORDER { get; set; }


        /// <summary>
        /// TO Get the State Names
        /// </summary>
        public SelectList States
        {
            get {
                List<MASTER_STATE> stateList = new List<MASTER_STATE>();
                PMGSY.DAL.Master.IMasterDAL objDAL = new PMGSY.DAL.Master.MasterDAL();
                stateList = objDAL.GetAllStateNames();                                                         
                stateList.Insert(0,new MASTER_STATE { MAST_STATE_CODE = 0, MAST_STATE_NAME = "Select state" });
                
                return new SelectList(stateList, "MAST_STATE_CODE", "MAST_STATE_NAME");
            }
        }

        /// <summary>
        /// Populate All States.
        /// </summary>
        public SelectList AllStates
        {
            get
            {
                List<MASTER_STATE> stateList = new List<MASTER_STATE>();
                PMGSY.DAL.Master.IMasterDAL objDAL = new PMGSY.DAL.Master.MasterDAL();
                stateList = objDAL.GetAllStateNames();
                stateList.Insert(0, new MASTER_STATE { MAST_STATE_CODE = 0, MAST_STATE_NAME = "All states" });

                return new SelectList(stateList, "MAST_STATE_CODE", "MAST_STATE_NAME");
            }
        }
        public SelectList InfoTypes
        {
            get {
                PMGSY.DAL.Master.IMasterDAL objDAL = new PMGSY.DAL.Master.MasterDAL();
                return  objDAL.PopulateInfoTypes();
            }        
        }

        public virtual MASTER_STATE MASTER_STATE { get; set; }
    }


    public class RequredInfoStateAttribute : ValidationAttribute, IClientValidatable
    {
        // private const string _defaultErrorMessage = "Start date must be less than end date.";
        private string _basePropertyName;

        public RequredInfoStateAttribute(string basePropertyName)
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

            var InfoType = basePropertyInfo.GetValue(validationContext.ObjectInstance, null);
            int? StateCode = (int?)value;

            //Actual Validation 
            if (( InfoType.ToString() == "T") && (StateCode==0))
            {
                var message = FormatErrorMessage(validationContext.DisplayName);
                return new ValidationResult(message);
            }

            return ValidationResult.Success;
        }


        public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
        {
            var rule = new ModelClientValidationRule
            {
                ErrorMessage = FormatErrorMessage(metadata.DisplayName),
                ValidationType = "infostatevalidator"
            };
            rule.ValidationParameters["date"] = this._basePropertyName;
            yield return rule;

        }

    }
}