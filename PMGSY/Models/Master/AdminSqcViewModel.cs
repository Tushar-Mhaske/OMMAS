/*----------------------------------------------------------------------------------------
 * Project Id     :

 * Project Name   :OMMAS-II

 * File Name      :AdminSqcViewModel.cs
 * 
 * Author         :Ashish Markande.

 * Creation Date  :14/May/2013

 * Desc           :This class is used to declare the variables, lists that are used in the Details form.
 * ---------------------------------------------------------------------------------------*/
using PMGSY.DAL.Master;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.ComponentModel.DataAnnotations;

namespace PMGSY.Models.Master
{
    public class AdminSqcViewModel
    {
        public string EncryptedSqcCode { get; set; }

        
        public int ADMIN_QC_CODE { get; set; }

        public string ADMIN_ND_NAME { get; set; }

        [Display(Name = "Department")]
        [Required(ErrorMessage="Please Select Department")]
        [Range(1,2147483647,ErrorMessage="Please Select Department")]
        public int ADMIN_ND_CODE { get; set; }

        public List<SelectListItem> depatmentList { get; set; }


      
        [Required]
        [Range(1, 2147483647, ErrorMessage = "Please select State.")]
        [Display(Name="State")]
        public int MAST_STATE_CODE { get; set; }


        [Display(Name="Quality Controller Name")]
        [RegularExpression(@"^([a-zA-Z0-9 ._]+)$", ErrorMessage = "Quality Controller Name is not in valid format.")]
        [Required(ErrorMessage = "Quality Controller Name is required.")]
        [StringLength(255, ErrorMessage = "Quality Controller Name must be less than 255 chareacters.")]
        public string ADMIN_QC_NAME { get; set; }


        [RegularExpression("([0-9]{1,12})", ErrorMessage = "Aadhaar Card No. is not in valid format.")]
        [MaxLength(12, ErrorMessage = "Aadhaar Card No. is of 12 digit Number.")]
        [Display(Name = "Aadhaar Card No.")]
        public string ADMIN__QC_AADHAR_NO { get; set; }

        [Display(Name="Designation")]
        [Range(1, 2147483647, ErrorMessage = "Please select Designation.")]
        public Nullable<int> ADMIN_QC_DESG { get; set; }

        [Display(Name="Address")]
        [RegularExpression("([a-zA-Z 0-9 _(),#$-.:;/]{1,255})", ErrorMessage = "Address is not in valid format.")]
        [StringLength(255,ErrorMessage="Address must be less than 255 characters.")]
        public string ADMIN_QC_ADDRESS1 { get; set; }

        [Display(Name = "Address2")]
        [RegularExpression("([a-zA-Z 0-9 _(),#$-.:;/]{1,255})", ErrorMessage = "Address is not in valid format.")]
        [StringLength(255, ErrorMessage = "Address must be less than 255 characters.")]
        public string ADMIN_QC_ADDRESS2 { get; set; }

        [Display(Name="District")]
        public Nullable<int> MAST_DISTRICT_CODE { get; set; }
        public Nullable<int> MAST_STATE_CODE_ADDR { get; set; }

       [Display(Name = "PIN Code")]
       [RegularExpression("^([0-9]{6})?$", ErrorMessage = "PIN Code should contains 6 digits only.")]
       [StringLength(6, ErrorMessage = "PIN Code must be 6 digits only.")]
       public string ADMIN_QC_PIN { get; set; }

       [Display(Name = "Phone1")]
       [RegularExpression("^([0-9]+)$", ErrorMessage = "STD Code is not in valid format.")]
       [StringLength(5, ErrorMessage = "STD Code must be 3 to 5 digits.", MinimumLength = 3)]       
       public string ADMIN_QC_STD1 { get; set; }

       [Display(Name = "Phone2")]
       [RegularExpression("^([0-9]+)$", ErrorMessage = "STD Code is not in valid format.")]
       [StringLength(5, ErrorMessage = "STD Code must be 3 to 5 digits.", MinimumLength = 3)]
        public string ADMIN_QC_STD2 { get; set; }

       [Display(Name = "-")]
       [RegularExpression("^([0-9]+)$", ErrorMessage = "Phone Number should contains digits only.")]
       [StringLength(8, ErrorMessage = "Phone Number must be 6 to 8 digits.", MinimumLength = 6)]   
       public string ADMIN_QC_PHONE1 { get; set; }

       [Display(Name = "-")]
       [RegularExpression("^([0-9]+)$", ErrorMessage = "Phone Number should contains digits only.")]
       [StringLength(8, ErrorMessage = "Phone Number must be 6 to 8 digits.", MinimumLength = 6)]
        public string ADMIN_QC_PHONE2 { get; set; }

       [Display(Name = "Fax")]
       [RegularExpression("^([0-9 ]+)$", ErrorMessage = "STD Code is not in valid format.")]
       [StringLength(5, ErrorMessage = "STD Code must be 3 to 5 digits.")]
        public string ADMIN_QC_STD_FAX { get; set; }

       [Display(Name = "Fax")]
       [RegularExpression("^([0-9 ]+)$", ErrorMessage = "Fax Number should contains digits only.")]
       [StringLength(8, ErrorMessage = "Fax Number must be 6 to 8 digits.", MinimumLength = 6)]
        public string ADMIN_QC_FAX { get; set; }


       [Display(Name = "Mobile")]
       [RegularExpression("^[0-9]{10}", ErrorMessage = "Enter 10 digits Mobile number.")]
       [Required(ErrorMessage = "Mobile no. is required")]
       public string ADMIN_QC_MOBILE { get; set; }

       [Display(Name = "Email")]
       [RegularExpression(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$", ErrorMessage = "Email address is not in valid format.")]
       [Required(ErrorMessage = "Email is required")]
       public string ADMIN_QC_EMAIL { get; set; }

        public string ADMIN_ACTIVE_STATUS { get; set; }

        [Display(Name = "Is Active")]
        public bool IsActive { get; set; }

        [Display(Name = "Valid Upto")]
        [RegularExpression(@"^(((0[1-9]|[12]\d|3[01])\/(0[13578]|1[02])\/((19|[2-9]\d)\d{2}))|((0[1-9]|[12]\d|30)\/(0[13456789]|1[012])\/((19|[2-9]\d)\d{2}))|((0[1-9]|1\d|2[0-8])\/02\/((19|[2-9]\d)\d{2}))|(29\/02\/((1[6-9]|[2-9]\d)(0[48]|[2468][048]|[13579][26])|((16|[2468][048]|[3579][26])00))))$", ErrorMessage = "Expiry date must be in dd/mm/yyyy format.")]
        [CustomValidationRequired("IsActive","EncryptedSqcCode", ErrorMessage = "Valid Upto Date is required.")]
        public string ADMIN_ACTIVE_ENDDATE { get; set; }

        [Display(Name = "Remarks")]
        [StringLength(255, ErrorMessage = "Remark must be less than 255 characters.")]
        [RegularExpression(@"^([a-zA-Z0-9 ._',\r\n&()-]+)$", ErrorMessage = "Remark is not in valid format.")]
            public string ADMIN_QC_REMARKS { get; set; }

        public virtual MASTER_DESIGNATION MASTER_DESIGNATION { get; set; }
        public virtual MASTER_DISTRICT MASTER_DISTRICT { get; set; }
        public virtual MASTER_STATE MASTER_STATE { get; set; }
        public virtual MASTER_STATE MASTER_STATE1 { get; set; }

        /// <summary>
        /// To get the Designation Names 
        /// </summary>
        public SelectList Designation
        {
            get
            {
                List<MASTER_DESIGNATION> stateList = new List<MASTER_DESIGNATION>();
                IMasterDAL objDAL = new MasterDAL();

                stateList = objDAL.GetDesignation();
                stateList.Insert(0, new MASTER_DESIGNATION { MAST_DESIG_CODE = 0, MAST_DESIG_NAME = "--Select--" });
                return new SelectList(stateList, "MAST_DESIG_CODE", "MAST_DESIG_NAME");
            }

        }
        /// <summary>
        /// To get the States Names
        /// </summary>
        public SelectList States
        {
            get
            {
                List<MASTER_STATE> stateList = new List<MASTER_STATE>();
                IMasterDAL objDAL = new MasterDAL();

                stateList = objDAL.GetStates();
                stateList.Insert(0, new MASTER_STATE { MAST_STATE_CODE= 0, MAST_STATE_NAME = "--Select--" });
                return new SelectList(stateList, "MAST_STATE_CODE", "MAST_STATE_NAME");
            }

        }

        /// <summary>
        /// To get the District Names 
        /// </summary>
        public SelectList Districts
        {
            get
            {
                List<MASTER_DISTRICT> stateList = new List<MASTER_DISTRICT>();
                IMasterDAL objDAL = new MasterDAL();

                stateList = objDAL.GetDistrictName(this.MAST_STATE_CODE);
                stateList.Insert(0, new MASTER_DISTRICT {MAST_DISTRICT_CODE= 0 , MAST_DISTRICT_NAME="--Select--" });
                return new SelectList(stateList, "MAST_DISTRICT_CODE", "MAST_DISTRICT_NAME");
            }

        }
    }

    /// <summary>
    /// To Get the status
    /// </summary>
    public class Status
    {
        public string StatusID { get; set; }
        public string StatusDescription { get; set; }

        public static readonly Dictionary<string, string> lstStatus = new Dictionary<string, string>() { { "A", "All" }, { "Y", "Active" }, { "N", "InActive" } };
    }

    //class for custom validation
    public class CustomValidationRequiredAttribute : ValidationAttribute, IClientValidatable
    {
        public string Property { get; set; }
        public string OtherProperty { get; set; }

        public CustomValidationRequiredAttribute(string Property, string OtherProperty)
        {
            this.Property = Property;
            this.OtherProperty = OtherProperty; 
        }


        public override string FormatErrorMessage(string name)
        {
            return string.Format(ErrorMessageString, name, Property);
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {

            System.Reflection.PropertyInfo PropertyInfo = validationContext.ObjectInstance.GetType().GetProperty(Property);
            System.Reflection.PropertyInfo OtherPropertyInfo = validationContext.ObjectInstance.GetType().GetProperty(OtherProperty);

            if (PropertyInfo == null)
            {
                return new ValidationResult(string.Format("Property '{0}' is undefined.", Property));
            }


            object PropertyValue = PropertyInfo.GetValue(validationContext.ObjectInstance, null);

            if (PropertyValue != null)
            {
                bool propertyValue = Convert.ToBoolean(PropertyInfo.GetValue(validationContext.ObjectInstance, null).ToString());
                string otherPropertyValue = OtherPropertyInfo.GetValue(validationContext.ObjectInstance, null)==null?null:OtherPropertyInfo.GetValue(validationContext.ObjectInstance, null).ToString() ;
               
                if (!string.IsNullOrEmpty(otherPropertyValue) && propertyValue == false && value == null)
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
                ValidationType = "customvalidationrequired"
            };

        }
    }//end custom validation
}