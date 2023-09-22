/*----------------------------------------------------------------------------------------
 * Project Id          :

 * Project Name        :OMMAS-II

 * File Name           :MasterContractorViewModel.cs
  
 * Author              :Vikram Nandanwar

 * Creation Date       :01/May/2013

 * Desc                :This class is used to declare the variables, lists that are used in the Details form.
 
 * ---------------------------------------------------------------------------------------*/
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PMGSY.Common;
using PMGSY.DAL;
using PMGSY.DAL.Master;
using System.Text.RegularExpressions;

namespace PMGSY.Models.Master
{
    public class MasterContractorViewModel
    {
        [UIHint("Hidden")]
        public string EncryptedContractorCode { get; set; }

        [Display(Name = "Contractor ID")]
        public int MAST_CON_ID { get; set; }

        [Display(Name = "Supplier Flag")]
        public string MAST_CON_SUP_FLAG { get; set; }

        //Added By Abhishek kamble
        [Display(Name = "Contractor Status")]
        [CheckMastContStatus("EncryptedContractorCode", "MAST_CON_LEGAL_HEIR_FNAME", "MAST_CON_LEGAL_HEIR_MNAME", "MAST_CON_LEGAL_HEIR_LNAME", ErrorMessage = "Please enter Contractor / Supplier Heir Name.")]
        public string Mast_Con_Status_Flag { get; set; }

        [Required(ErrorMessage = "Company Name is required.")]
        //[RegularExpression(@"^([a-zA-Z0-9 ._-]+)$", ErrorMessage = "Company Name is not in valid format.")]
        [RegularExpression(@"^(?![0-9]*$)[a-zA-Z0-9.,-/& ]+$", ErrorMessage = "Company Name is not in valid format.")] //working   
        [StringLength(255, ErrorMessage = "Company Name must be less than 255 characters.")]
        [Display(Name = "Company Name")]
        public string MAST_CON_COMPANY_NAME { get; set; }

        [Required(ErrorMessage = "First Name is required.")]
        [Display(Name = "First Name")]
        [StringLength(50, ErrorMessage = "First Name must be less than 50 characters.")]
        [RegularExpression(@"^([a-zA-Z0-9 ._]+)$", ErrorMessage = "First Name is not in valid format.")]
        public string MAST_CON_FNAME { get; set; }

        [Display(Name = "Middle Name")]
        [StringLength(50, ErrorMessage = "Middle Name must be less than 50 characters.")]
        [RegularExpression(@"^([a-zA-Z0-9 ._]+)$", ErrorMessage = "Middle Name is not in valid format.")]
        public string MAST_CON_MNAME { get; set; }

        [Display(Name = "Last Name")]
        [StringLength(50, ErrorMessage = "Last Name must be less than 50 characters.")]
        [RegularExpression(@"^([a-zA-Z0-9 ._]+)$", ErrorMessage = "Last Name is not in valid format.")]
        public string MAST_CON_LNAME { get; set; }

        [Display(Name = "Contact Address")]
        [RegularExpression("([a-zA-Z 0-9 _(),#$-.]{1,255})", ErrorMessage = "Address is not in valid format.")]
        [StringLength(255, ErrorMessage = "Address must be less than 255 characters.")]
        [Required(ErrorMessage="Address is required.")]
        public string MAST_CON_ADDR1 { get; set; }

        [RegularExpression("([a-zA-Z 0-9 _(),#$-.]{1,255})", ErrorMessage = "Address is not in valid format.")]
        [StringLength(255, ErrorMessage = "Address must be less than 255 characters.")]
        public string MAST_CON_ADDR2 { get; set; }

        [Required(ErrorMessage = "Please select District.")]
        [Range(1, 2147483647, ErrorMessage = "Please select District.")]
        [Display(Name = "District")]
        public int MAST_DISTRICT_CODE_ADDR { get; set; }

        [Required(ErrorMessage = "Please select State.")]
        [Range(1, 2147483647, ErrorMessage = "Please select State.")]
        [Display(Name = "State")]
        public int MAST_STATE_CODE_ADDR { get; set; }

        [Required(ErrorMessage = "Please select Pin Code.")]
        [Display(Name = "PIN Code")]
        [RegularExpression("^([0-9]{6})?$", ErrorMessage = "Enter 6 digit PIN Code.")]
        [StringLength(6, ErrorMessage = "PIN Code must be 6 digits only.")]
        public string MAST_CON_PIN { get; set; }

        //[Required(ErrorMessage = "Please Enter STD Code.")]
        [Display(Name = "Phone1")]
        [RegularExpression("^([0-9]+)$", ErrorMessage = "STD Code is not in valid format.")]
        [StringLength(6, ErrorMessage = "STD Code must be 3 to 5 digits.", MinimumLength = 3)]
        public string MAST_CON_STD1 { get; set; }

        [Display(Name = "Phone2")]
        [RegularExpression("^([0-9]+)$", ErrorMessage = "STD Code is not in valid format.")]
        [StringLength(5, ErrorMessage = "STD Code must be 3 to 5 digits.", MinimumLength = 3)]
        public string MAST_CON_STD2 { get; set; }

        //[Required(ErrorMessage = "Please Enter Phone Number.")]
        [Display(Name = "-")]
        [RegularExpression("^([0-9]+)$", ErrorMessage = "Phone Number should contains digits only.")]
        [StringLength(8, ErrorMessage = "Phone Number must be 6 to 8 digits.", MinimumLength = 6)]
        public string MAST_CON_PHONE1 { get; set; }

        [Display(Name = "-")]
        [RegularExpression("^([0-9]+)$", ErrorMessage = "Phone Number should contains digits only.")]
        [StringLength(8, ErrorMessage = "Phone Number must be 6 to 8 digits.", MinimumLength = 6)]
        public string MAST_CON_PHONE2 { get; set; }

        [Display(Name = "Fax")]
        [RegularExpression("^([0-9]+)$", ErrorMessage = "STD Code is not in valid format.")]
        [StringLength(5, ErrorMessage = "STD Code must be 3-5 digits")]
        public string MAST_CON_STD_FAX { get; set; }

        [Display(Name = "-")]
        [RegularExpression("^([0-9 ]+)$", ErrorMessage = "Fax Number should contains digits only.")]
        [StringLength(30, ErrorMessage = "Fax number is too long")]
        public string MAST_CON_FAX { get; set; }

        [Display(Name = "Mobile")]
        [RegularExpression("^[0-9]{10,11}", ErrorMessage = "Enter 10 digit mobile number.")]
        public string MAST_CON_MOBILE { get; set; }

        [Display(Name = "Email")]
        [RegularExpression(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$", ErrorMessage = "Email is not in valid format.")]
        public string MAST_CON_EMAIL { get; set; }

        [Required(ErrorMessage = "PAN/TAN is required.")]
        [Display(Name = "PAN")]
        [PanTanNumberValidationAttribute("MAST_CON_SUP_FLAG", ErrorMessage = "Entered number is not in valid format.")]
        public string MAST_CON_PAN { get; set; }

        [Display(Name = "Heir First Name")]
        [RegularExpression(@"^([a-zA-Z ._]+)$", ErrorMessage = "First Name is not in valid format.")]
        public string MAST_CON_LEGAL_HEIR_FNAME { get; set; }

        [Display(Name = "Heir Middle Name")]
        [RegularExpression(@"^([a-zA-Z ._]+)$", ErrorMessage = "Middle Name is not in valid format.")]
        public string MAST_CON_LEGAL_HEIR_MNAME { get; set; }

        [Display(Name = "Heir Last Name")]
        [RegularExpression(@"^([a-zA-Z ._]+)$", ErrorMessage = "Last Name is not in valid format.")]
        public string MAST_CON_LEGAL_HEIR_LNAME { get; set; }

        [Display(Name = "Expired Date")]//:(dd/mm/yyyy)]
        // [RegularExpression(@"^(((0[1-9]|[12]\d|3[01])\/(0[13578]|1[02])\/((19|[2-9]\d)\d{2}))|((0[1-9]|[12]\d|30)\/(0[13456789]|1[012])\/((19|[2-9]\d)\d{2}))|((0[1-9]|1\d|2[0-8])\/02\/((19|[2-9]\d)\d{2}))|(29\/02\/((1[6-9]|[2-9]\d)(0[48]|[2468][048]|[13579][26])|((16|[2468][048]|[3579][26])00))))$", ErrorMessage = "Expired Date must be in dd/mm/yyyy format.")]
        [DataType(DataType.Date, ErrorMessage = "Expired Date is not in valid format.")]
        public string MAST_CON_EXPIRY_DATE { get; set; }
        [Display(Name = "Remark")]
        [StringLength(255, ErrorMessage = "Remark must be less than 255 characters.")]
        [RegularExpression(@"^([a-zA-Z0-9 ._',&()-]+)$", ErrorMessage = "Remark is not in valid format.")]
        public string MAST_CON_REMARKS { get; set; }

        [Display(Name = "Contractor Status")]

        public string MAST_CON_STATUS { get; set; }

        public string Pan_Tan { get; set; }

        public string DistrictName { get; set; }

        public string StateName { get; set; }

        //Added by vikky [06-01-2022]
        public bool xmlRelaxationStatus { get; set; }

        //Added by Pradip Patil [11/05/2017 06:27 PM]
        [Display(Name = "Aadhar No.")]
        [RegularExpression(@"^([0-9]{12})", ErrorMessage = "Invalid Aadhar number.")]
        //[Required(ErrorMessage="Aadhar Number is required")]
        public String AadharNumber { get; set; }

        public List<SelectListItem> lstState { set; get; }

        /// <summary>
        /// To Get the States
        /// </summary>
        public SelectList States
        {
            get
            {
                List<MASTER_STATE> stateList = new List<MASTER_STATE>();

                MasterDAL objMaster = new MasterDAL();

                stateList = objMaster.GetAllStates();

                stateList.Insert(0, new PMGSY.Models.MASTER_STATE() { MAST_STATE_CODE = 0, MAST_STATE_NAME = "--Select--" });

                return new SelectList(stateList, "MAST_STATE_CODE", "MAST_STATE_NAME", this.MAST_STATE_CODE_ADDR);
            }
            set
            { 
            
            }
        }
        /// <summary>
        /// To Get the Districts.
        /// </summary>
        public SelectList Districts
        {
            get
            {
                List<PMGSY.Models.MASTER_DISTRICT> districtList = new List<PMGSY.Models.MASTER_DISTRICT>();

                MasterDAL masterDataEntryDAL = new MasterDAL();

                districtList = masterDataEntryDAL.GetAllDistricts(this.MAST_STATE_CODE_ADDR);

                districtList.Insert(0, new MASTER_DISTRICT { MAST_DISTRICT_CODE = 0, MAST_DISTRICT_NAME = "--Select--" });

                return new SelectList(districtList, "MAST_DISTRICT_CODE", "MAST_DISTRICT_NAME", this.MAST_DISTRICT_CODE_ADDR);
            }
        }
        /// <summary>
        /// To Get the Status
        /// </summary>
        public SelectList Status
        {
            get
            {
                List<SelectListItem> list = new List<SelectListItem>();
                list.Insert(0, new SelectListItem { Value = "A", Text = "Active" });
                list.Insert(1, new SelectListItem { Value = "I", Text = "Inactive" });
                list.Insert(2, new SelectListItem { Value = "E", Text = "Expired" });
                return new SelectList(list, "Value", "Text");
            }
        }

        public virtual ICollection<ACC_AUTH_REQUEST_MASTER> ACC_AUTH_REQUEST_MASTER { get; set; }
        public virtual ICollection<ACC_BILL_DETAILS> ACC_BILL_DETAILS { get; set; }
        public virtual ICollection<ACC_BILL_MASTER> ACC_BILL_MASTER { get; set; }
        public virtual ICollection<MASTER_CONTRACTOR_BANK> MASTER_CONTRACTOR_BANK { get; set; }
        public virtual MASTER_DISTRICT MASTER_DISTRICT { get; set; }
        public virtual MASTER_STATE MASTER_STATE { get; set; }
        public virtual ICollection<MASTER_CONTRACTOR_REGISTRATION> MASTER_CONTRACTOR_REGISTRATION { get; set; }
    }



    public class PanTanNumberValidationAttribute : ValidationAttribute, IClientValidatable
    {

        //private const string _defaultErrorMessage = "Start date must be less than end date.";
        private string _basePropertyName;

        public PanTanNumberValidationAttribute(string basePropertyName)
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
            //var startDate = (DateTime)basePropertyInfo.GetValue(validationContext.ObjectInstance, null);            
            //var thisDate = (DateTime)value;  

            var ConSupplierType = basePropertyInfo.GetValue(validationContext.ObjectInstance, null);
            var PanTanNumber = value;

            if (ConSupplierType.ToString() == "C")
            {
                //Actual validation 
                //"^([a-zA-Z]){5}([0-9]){4}([a-zA-Z]){1}?$"
                //,@"^[\w]{3}(p|P|c|C|h|H|f|F|a|A|t|T|b|B|l|L|j|J|g|G)[\w][\d]{4}[\w]$")
                //if (!Regex.IsMatch(PanTanNumber.ToString(), @"^[\w]{3}(p|P|c|C|h|H|f|F|a|A|t|T|b|B|l|L|j|J|g|G)[\w][\d]{4}[\w]$"))
                //{
                //    //var message = FormatErrorMessage(validationContext.DisplayName);
                //    return new ValidationResult("PAN is not in valid format.");
                //}

                // added on 22-12-2021 by Srishti Tyagi
                // Validations of PAN for Contractor
                if (!Regex.IsMatch(PanTanNumber.ToString(), "^([a-zA-Z]){5}([0-9]){4}([a-zA-Z]){1}?$"))
                {
                    if (!Regex.IsMatch(PanTanNumber.ToString(), @"^[\w]{3}(p|P|c|C|h|H|f|F|a|A|t|T|b|B|l|L|j|J|g|G)[\w][\d]{4}[\w]$"))
              
                    //var message = FormatErrorMessage(validationContext.DisplayName);
                    return new ValidationResult("PAN is not in valid format.");
                }
            }
            else
            {
                 //Actual validation   
               // Validations for TAN and PAN
                if (!Regex.IsMatch(PanTanNumber.ToString(), "^([a-zA-Z]){4}([0-9]){5}([a-zA-Z]){1}?$")) //|| ((!Regex.IsMatch(PanTanNumber.ToString(), "^([a-zA-Z]){5}([0-9]){4}([a-zA-Z]){1}?$")) && (!Regex.IsMatch(PanTanNumber.ToString(), @"^[\w]{3}(p|P|c|C|h|H|f|F|a|A|t|T|b|B|l|L|j|J|g|G)[\w][\d]{4}[\w]$"))))
                {
                    // added on 28-12-2021 by Srishti Tyagi
                    if (!Regex.IsMatch(PanTanNumber.ToString(), "^([a-zA-Z]){5}([0-9]){4}([a-zA-Z]){1}?$"))
                    {
                        if (!Regex.IsMatch(PanTanNumber.ToString(), @"^[\w]{3}(p|P|c|C|h|H|f|F|a|A|t|T|b|B|l|L|j|J|g|G)[\w][\d]{4}[\w]$"))

                            //var message = FormatErrorMessage(validationContext.DisplayName);
                            return new ValidationResult("PAN is not in valid format.");
                    }
                    else
                        return ValidationResult.Success;

                    return new ValidationResult("TAN is not in valid format.");
                }
                // added on 22-12-2021 by Srishti Tyagi
                // Validations for PAN
                //else if (!Regex.IsMatch(PanTanNumber.ToString(), "^([a-zA-Z]){5}([0-9]){4}([a-zA-Z]){1}?$"))
                //{
                //    if (!Regex.IsMatch(PanTanNumber.ToString(), @"^[\w]{3}(p|P|c|C|h|H|f|F|a|A|t|T|b|B|l|L|j|J|g|G)[\w][\d]{4}[\w]$"))

                //        //var message = FormatErrorMessage(validationContext.DisplayName);
                //        return new ValidationResult("PAN is not in valid format.");
                //}
            }
            //Default return - This means there were no validation error  
            //return null;
            return ValidationResult.Success;
        }

        public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
        {
            yield return new ModelClientValidationRule
            {
                ErrorMessage = FormatErrorMessage(metadata.DisplayName),
                //This is the name of the method aaded to the jQuery validator method (must be lower case)
                ValidationType = "pantanfieldvalidator"
            };

        }
    }


    //Added By Abhishek kamble 4-Apr-2014

    public class CheckMastContStatus : ValidationAttribute, IClientValidatable
    {
        private readonly string EncryptedCode;
        private readonly string ContSupHeirFirstName;
        private readonly string ContSupHeirMiddleName;
        private readonly string ContSupHeirLastName;


        public CheckMastContStatus(string encryptedCode, string contSupHeirFirstName, string contSupHeirMiddleName, string contSupHeirLastName)
        {
            this.EncryptedCode = encryptedCode;
            this.ContSupHeirFirstName = contSupHeirFirstName;
            this.ContSupHeirMiddleName = contSupHeirMiddleName;
            this.ContSupHeirLastName = contSupHeirLastName;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var propertyEncryptedCode = validationContext.ObjectType.GetProperty(this.EncryptedCode);
            var propertyContSupHeirFirstName = validationContext.ObjectType.GetProperty(this.ContSupHeirFirstName);
            var propertyContSupHeirMiddleName = validationContext.ObjectType.GetProperty(this.ContSupHeirMiddleName);
            var propertyContSupHeirLastName = validationContext.ObjectType.GetProperty(this.ContSupHeirLastName);



            //Validation start
            var encCode = propertyEncryptedCode.GetValue(validationContext.ObjectInstance, null);
            var conSupHeirFirstName = propertyContSupHeirFirstName.GetValue(validationContext.ObjectInstance, null);
            var conSupHeirMiddleName = propertyContSupHeirMiddleName.GetValue(validationContext.ObjectInstance, null);
            var conSupHeirLastName = propertyContSupHeirLastName.GetValue(validationContext.ObjectInstance, null);

            if (encCode != null)
            {
                if (value.ToString() == "E")
                {
                    if ((conSupHeirFirstName == String.Empty || conSupHeirFirstName == null) && (conSupHeirMiddleName == String.Empty || conSupHeirMiddleName == null) && (conSupHeirLastName == String.Empty || conSupHeirLastName == null))
                    {
                        //var msg= new ValidationResult(ErrorMessage);
                        return new ValidationResult(ErrorMessage);
                    }
                }
                else
                {
                    return ValidationResult.Success;
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
            var rule = new ModelClientValidationRule
            {
                ErrorMessage = this.ErrorMessageString,
                ValidationType = "checkmastcontstatus"
            };
            rule.ValidationParameters["status"] = this.ErrorMessage;
            yield return rule;
        }
    }

}