using PMGSY.Common;
/*----------------------------------------------------------------------------------------
 * Project Id     :

 * Project Name   :OMMAS-II

 * File Name      :AdminDepartmentViewModel.cs
 * 
 * Author         :Koustubh Nakate
 
 * Creation Date  :14/May/2013

 * Desc           : This class is used to declare the variables, lists that are used in the Details form.
 * ---------------------------------------------------------------------------------------*/
using PMGSY.DAL.Master;
using PMGSY.Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PMGSY.Models.Master
{
    public class AdminDepartmentViewModel
    {

        [UIHint("hidden")]
        public string EncryptedAdminCode { get; set; }

        [UIHint("hidden")]
        public string EncryptedMapAdminCode { get; set; }

       
        public string Mast_Parent_ND_Name { get; set; }
       

        [Display]
        public int ADMIN_ND_CODE { get; set; }

        [Required(ErrorMessage = "Please select SRRDA.")]
        [Range(1, 2147483647, ErrorMessage = "Please select SRRDA.")]
        [Display(Name = "SRRDA")]
        public int? MAST_PARENT_ND_CODE { get; set; }
        public List<SelectListItem> MAST_PARENT_ND_CODE_List { get; set; }

        [Required(ErrorMessage = "Please select State.")]
        [Range(1, 2147483647, ErrorMessage = "Please select State.")]
        [Display(Name = "State")]
        public int MAST_STATE_CODE { get; set; }

        [Required(ErrorMessage = "Please select Agency.")]
        [Range(1, 2147483647, ErrorMessage = "Please select Agency.")]
        [Display(Name = "Agency Name")]
        public int MAST_AGENCY_CODE { get; set; }

      //  public Nullable<int> MAST_PARENT_ND_CODE { get; set; }

        public string MAST_ND_TYPE { get; set; }

        [Required(ErrorMessage = "Name field is required.")]
        [RegularExpression(@"^([a-zA-Z0-9 _(),-./]+)$", ErrorMessage = "SRRDA/DPIU Name is not in valid format.")]
        [StringLength(100, ErrorMessage = "SRRDA/DPIU Name must be less than 100 characters.")]
        [Display(Name = "Admin Name")]
        public string ADMIN_ND_NAME { get; set; }

        [Display(Name = "Address")]
        [RegularExpression("([a-zA-Z 0-9 _(),#$-.:;/]{1,255})", ErrorMessage = "Address is not in valid format.")]
        [StringLength(255, ErrorMessage = "Address must be less than 255 characters.")]
        public string ADMIN_ND_ADDRESS1 { get; set; }

        [Display(Name = "Address2")]
        [RegularExpression("([a-zA-Z 0-9 _(),#$-.:;/]{1,255})", ErrorMessage = "Address is not in valid format.")]
        [StringLength(255, ErrorMessage = "Address must be less than 255 characters.")]
        public string ADMIN_ND_ADDRESS2 { get; set; }

        [Display(Name = "District")]
        [Required(ErrorMessage = "Please select district.")]
        [Range(1, 2147483647, ErrorMessage = "Please select district.")]
        public Nullable<int> MAST_DISTRICT_CODE { get; set; }

        [Display(Name = "PIN Code")]
        [RegularExpression("^([0-9]{6})?$", ErrorMessage = "Enter 6 digits PIN Code.")]
        [StringLength(6, ErrorMessage = "PIN Code must be 6 digits.")]
        public string ADMIN_ND_PIN { get; set; }

        [Display(Name = "Phone Number 1")]
        [RegularExpression(@"^([0-9]+)$", ErrorMessage = "STD Code is not in valid format.")]
        [StringLength(5, ErrorMessage = "STD code must be between 3 to 5 digits.")]
        public string ADMIN_ND_STD1 { get; set; }

        [Display(Name = "Phone Number 2")]
        [RegularExpression(@"^([0-9]+)$", ErrorMessage = "STD Code is not in valid format.")]
        [StringLength(5, ErrorMessage = "STD code must be between 3 to 5 digits.")]
        public string ADMIN_ND_STD2 { get; set; }

        [Display(Name = "-")]
        [RegularExpression(@"^([0-9]+)$", ErrorMessage = "Phone Number should contains digits only.")]
        [StringLength(30, ErrorMessage = "Phone no. exceeds 30 digits")]
        public string ADMIN_ND_PHONE1 { get; set; }

        [Display(Name = "-")]
        [RegularExpression(@"^([0-9]+)$", ErrorMessage = "Phone Number should contains digits only.")]
        [StringLength(30, ErrorMessage = "Phone no. exceeds 30 digits.")]
        public string ADMIN_ND_PHONE2 { get; set; }

        [Display(Name = "Fax Number")]
        [RegularExpression(@"^([0-9]+)$", ErrorMessage = "STD Code is not in valid format.")]
        [StringLength(5, ErrorMessage = "STD code exceeds 5 digits.")]
        public string ADMIN_ND_STD_FAX { get; set; }

        [Display(Name = "-")]
        [RegularExpression(@"^([0-9]+)$", ErrorMessage = "Fax Number should contains digits only.")]
        [StringLength(30, ErrorMessage = "Fax no. exceeds 30 digits.", MinimumLength = 6)]
        public string ADMIN_ND_FAX { get; set; }

        [Display(Name = "Mobile")]
        [RegularExpression("^[0-9]{10,11}", ErrorMessage = "Enter 10 digits Mobile number.")]
        [StringLength(10, ErrorMessage = "Mobile number must be 10 digits.")]
        public string ADMIN_ND_MOBILE_NO { get; set; }

        [Display(Name = "Email")]
        [RegularExpression(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$", ErrorMessage = "Email address is not in valid format.")]
        public string ADMIN_ND_EMAIL { get; set; }

        [Display(Name = "Remark")]
        [RegularExpression(@"^([a-zA-Z0-9 ._';\r\n,&()-]+)$", ErrorMessage = "Remark is not in valid format.")]
        [StringLength(255, ErrorMessage = "Remark must be less than 255 characters.")]
        public string ADMIN_ND_REMARKS { get; set; }

        [Display(Name = "SBD Date")]
        [RegularExpression(@"^(((0[1-9]|[12]\d|3[01])\/(0[13578]|1[02])\/((19|[2-9]\d)\d{2}))|((0[1-9]|[12]\d|30)\/(0[13456789]|1[012])\/((19|[2-9]\d)\d{2}))|((0[1-9]|1\d|2[0-8])\/02\/((19|[2-9]\d)\d{2}))|(29\/02\/((1[6-9]|[2-9]\d)(0[48]|[2468][048]|[13579][26])|((16|[2468][048]|[3579][26])00))))$", ErrorMessage = "SBD date must be in dd/mm/yyyy format.")]
        public string ADMIN_SBD_DATE { get; set; }

        [Display(Name = "PDF Key")]
        public string ADMIN_PDF_KEY { get; set; }

        [Display(Name = "Email CC")]

        [RegularExpression(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$", ErrorMessage = "Email address is not in valid format.")]
        public string ADMIN_EMAIL_CC { get; set; }

        [Display(Name = "TAN")]
        [RegularExpression("^([a-zA-Z]){4}([0-9]){5}([a-zA-Z]){1}?$", ErrorMessage = "TAN is not in valid format.")]
        [StringLength(10, ErrorMessage = "TAN exceeds 10 digits.")]
        public string ADMIN_ND_TAN_NO { get; set; }

        [RegularExpression(@"^([YN]+)$", ErrorMessage = "Please select Yes or No.")]
        public string ADMIN_BANK_AUTH_ENABLED { get; set; }

        [Display(Name = "BA Enable Date")]
        [CustomValidation("ADMIN_BANK_AUTH_ENABLED", "MAST_ND_TYPE", ErrorMessage = "BA Enable Date is required.")]
        [RegularExpression(@"^(((0[1-9]|[12]\d|3[01])\/(0[13578]|1[02])\/((19|[2-9]\d)\d{2}))|((0[1-9]|[12]\d|30)\/(0[13456789]|1[012])\/((19|[2-9]\d)\d{2}))|((0[1-9]|1\d|2[0-8])\/02\/((19|[2-9]\d)\d{2}))|(29\/02\/((1[6-9]|[2-9]\d)(0[48]|[2468][048]|[13579][26])|((16|[2468][048]|[3579][26])00))))$", ErrorMessage = "BA Enable Date must be in dd/mm/yyyy format.")]
        public string ADMIN_BA_ENABLE_DATE { get; set; }

        [Display(Name = "Epay Email")]
        [RegularExpression(@"^([YN]+)$", ErrorMessage = "Please select Yes or No.")]
        public string ADMIN_EPAY_MAIL { get; set; }

        [Display(Name = "Epay Enable Date")]
        [CustomValidation("ADMIN_EPAY_MAIL", "MAST_ND_TYPE", ErrorMessage = "Epay Enable Date is required.")]
        [RegularExpression(@"^(((0[1-9]|[12]\d|3[01])\/(0[13578]|1[02])\/((19|[2-9]\d)\d{2}))|((0[1-9]|[12]\d|30)\/(0[13456789]|1[012])\/((19|[2-9]\d)\d{2}))|((0[1-9]|1\d|2[0-8])\/02\/((19|[2-9]\d)\d{2}))|(29\/02\/((1[6-9]|[2-9]\d)(0[48]|[2468][048]|[13579][26])|((16|[2468][048]|[3579][26])00))))$", ErrorMessage = "Epay Enable Date must be in dd/mm/yyyy format.")]
        public string ADMIN_EPAY_ENABLE_DATE { get; set; }

        [RegularExpression(@"^([YN]+)$", ErrorMessage = " Please select Yes or No.")]
        public string ADMIN_EREMITTANCE_ENABLED { get; set; }


        [Display(Name = "Remittance Enable Date")]
        [CustomValidation("ADMIN_EREMITTANCE_ENABLED", "MAST_ND_TYPE", ErrorMessage = "Remittance Enable Date is required.")]
        [RegularExpression(@"^(((0[1-9]|[12]\d|3[01])\/(0[13578]|1[02])\/((19|[2-9]\d)\d{2}))|((0[1-9]|[12]\d|30)\/(0[13456789]|1[012])\/((19|[2-9]\d)\d{2}))|((0[1-9]|1\d|2[0-8])\/02\/((19|[2-9]\d)\d{2}))|(29\/02\/((1[6-9]|[2-9]\d)(0[48]|[2468][048]|[13579][26])|((16|[2468][048]|[3579][26])00))))$", ErrorMessage = "Remittance Enable Date must be in dd/mm/yyyy format.")]
        public string ADMIN_EREMIT_ENABLED_DATE { get; set; }


        //New 
        [Display(Name = "PIU Active")]
        [RegularExpression(@"^([YN]+)$", ErrorMessage = "Please select Yes or No.")]
        public string PIUActive { get; set; }

        [Display(Name = "PIU Close Date")]
        [CustomPIUCloseDateValidation("PIUActive", "MAST_ND_TYPE", "EncryptedAdminCode", ErrorMessage = "PIU Close Date is required.")]
        [RegularExpression(@"^(((0[1-9]|[12]\d|3[01])\/(0[13578]|1[02])\/((19|[2-9]\d)\d{2}))|((0[1-9]|[12]\d|30)\/(0[13456789]|1[012])\/((19|[2-9]\d)\d{2}))|((0[1-9]|1\d|2[0-8])\/02\/((19|[2-9]\d)\d{2}))|(29\/02\/((1[6-9]|[2-9]\d)(0[48]|[2468][048]|[13579][26])|((16|[2468][048]|[3579][26])00))))$", ErrorMessage = "PIU Close Date must be in dd/mm/yyyy format.")]
        public string PIU_Close_DATE { get; set; }

        public string STATE_NAME { get; set; }

        public string District_Name { get; set; }

        public string Agency_Name { get; set; }
        /// <summary>
        /// To Get the state Names
        /// </summary>
        public SelectList States
        {
            get
            {
                List<MASTER_STATE> stateList = new List<MASTER_STATE>();

                MasterDAL objMaster = new MasterDAL();

                stateList = objMaster.GetAllStates();

                stateList.Insert(0, new PMGSY.Models.MASTER_STATE() { MAST_STATE_CODE = 0, MAST_STATE_NAME = "--Select--" });

                return new SelectList(stateList, "MAST_STATE_CODE", "MAST_STATE_NAME", this.MAST_STATE_CODE);
            }
        }
        /// <summary>
        /// To get the Districts.
        /// </summary>
        public SelectList Districts
        {
            get
            {
                List<PMGSY.Models.MASTER_DISTRICT> districtList = new List<PMGSY.Models.MASTER_DISTRICT>();

                MasterDAL masterDataEntryDAL = new MasterDAL();

                districtList = masterDataEntryDAL.GetAllDistricts(this.MAST_STATE_CODE);

                districtList.Insert(0, new PMGSY.Models.MASTER_DISTRICT() { MAST_DISTRICT_CODE = 0, MAST_DISTRICT_NAME = "--Select--" });

                return new SelectList(districtList, "MAST_DISTRICT_CODE", "MAST_DISTRICT_NAME", this.MAST_DISTRICT_CODE);
            }
        }
        /// <summary>
        /// To get the Agency list 
        /// </summary>
        public SelectList Agency
        {
            get
            {

                if (PMGSYSession.Current.RoleCode != 47 && PMGSYSession.Current.RoleCode != 36)//RoleCode=ITNO=36 and ITNOOA=47
                {
                    List<MASTER_AGENCY> agencyList = new List<MASTER_AGENCY>();

                    MasterDAL objDAL = new MasterDAL();

                    agencyList = objDAL.GetAgencyNames();

                    agencyList.Insert(0, new PMGSY.Models.MASTER_AGENCY() { MAST_AGENCY_CODE = 0, MAST_AGENCY_NAME = "--Select--" });

                    return new SelectList(agencyList, "MAST_AGENCY_CODE", "MAST_AGENCY_NAME", this.MAST_AGENCY_CODE);
                }
                else
                {
                    CommonFunctions objCommon = new CommonFunctions();
                    List<SelectListItem> lstAgencyDeptwise = new List<SelectListItem>();
                    lstAgencyDeptwise = objCommon.PopulateAgenciesByStateAndDepartmentwise(PMGSYSession.Current.StateCode, PMGSYSession.Current.AdminNdCode, true);
                    lstAgencyDeptwise.RemoveAt(0);
                    return new SelectList(lstAgencyDeptwise, "Value", "Text");
      
                }
            }
        }

        /// <summary>
        /// To get the Departmenets 
        /// </summary>
        public SelectList Department
        {
            get
            {
                List<SelectListItem> lstDepartment = new List<SelectListItem>();

                lstDepartment.Add(new SelectListItem { Value = "", Text = "All", Selected = true });
                lstDepartment.Add(new SelectListItem { Value = "S", Text = "SRRDA" });
                lstDepartment.Add(new SelectListItem { Value = "D", Text = "DPIU" });
                return new SelectList(lstDepartment, "Value", "Text");
            }
        }

      
    }

    //class for custom validation
    public class CustomValidationAttribute : ValidationAttribute, IClientValidatable
    {
        public string Property { get; set; }
        public string Property_NDType { get; set; }

        public CustomValidationAttribute(string Property, string Property_NDType)
        {
            this.Property = Property;
            this.Property_NDType = Property_NDType;
        }


        public override string FormatErrorMessage(string name)
        {
            return string.Format(ErrorMessageString, name, Property);
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {

            System.Reflection.PropertyInfo otherPropertyInfo = validationContext.ObjectInstance.GetType().GetProperty(Property);
            System.Reflection.PropertyInfo otherPropertyInfo_NDType = validationContext.ObjectInstance.GetType().GetProperty(Property_NDType);

            if (otherPropertyInfo == null)
            {
                return new ValidationResult(string.Format("Property '{0}' is undefined.", Property));
            }

            string otherPropertyValue_NDType = otherPropertyInfo_NDType.GetValue(validationContext.ObjectInstance, null).ToString();

            if (otherPropertyValue_NDType.Equals("D"))
            {
                string otherPropertyValue = otherPropertyInfo.GetValue(validationContext.ObjectInstance, null).ToString();

                if (otherPropertyValue == "Y" && value == null)
                {
                    return new ValidationResult(this.FormatErrorMessage(validationContext.DisplayName));
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
                ValidationType = "customvalidation"
            };

        }
    }//end custom validation

    //class for custom validation
    public class CustomPIUCloseDateValidationAttribute : ValidationAttribute, IClientValidatable
    {
        public string Property { get; set; }
        public string Property_NDType { get; set; }
        public string EncryptedAdminCode { get; set; }

        public CustomPIUCloseDateValidationAttribute(string Property, string Property_NDType, string EncryptedAdminCode)
        {
            this.Property = Property;
            this.Property_NDType = Property_NDType;
            this.EncryptedAdminCode = EncryptedAdminCode;
        }


        public override string FormatErrorMessage(string name)
        {
            return string.Format(ErrorMessageString, name, Property);
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {

            System.Reflection.PropertyInfo otherPropertyInfo = validationContext.ObjectInstance.GetType().GetProperty(Property);
            System.Reflection.PropertyInfo otherPropertyInfo_NDType = validationContext.ObjectInstance.GetType().GetProperty(Property_NDType);
            System.Reflection.PropertyInfo otherEncryptedAdminCode = validationContext.ObjectInstance.GetType().GetProperty(EncryptedAdminCode);

            if (otherPropertyInfo == null)
            {
                return new ValidationResult(string.Format("Property '{0}' is undefined.", Property));
            }

            string otherPropertyValue_NDType = otherPropertyInfo_NDType.GetValue(validationContext.ObjectInstance, null).ToString();
            var other_EncryptedAdminCodeValue = otherEncryptedAdminCode.GetValue(validationContext.ObjectInstance, null);

            if (other_EncryptedAdminCodeValue != null)
            {
                if (otherPropertyValue_NDType.Equals("D"))
                {
                    string otherPropertyValue = otherPropertyInfo.GetValue(validationContext.ObjectInstance, null).ToString();

                    if (otherPropertyValue == "N" && value == null)
                    {
                        return new ValidationResult(this.FormatErrorMessage(validationContext.DisplayName));
                    }
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
                ValidationType = "customvalidationpiuclosedate"
            };

        }
    }//end custom PiuClose validation
}