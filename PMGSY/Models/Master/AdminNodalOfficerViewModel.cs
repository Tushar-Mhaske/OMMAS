using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using PMGSY.DAL.Master;
using PMGSY.Extensions;

namespace PMGSY.Models.Master
{
    public class AdminNodalOfficerViewModel
    {
       

        [UIHint("Hidden")]
        public string EncryptedOfficerCode { get; set; }

        
        public int ADMIN_NO_OFFICER_CODE { get; set; }

        [Required (ErrorMessage=" Please select Office.")]
        [Range(1,2147483647,ErrorMessage=" Please select Office.")]
        [Display(Name = "Office")]
        public int ADMIN_ND_CODE { get; set; }

        [Required(ErrorMessage="First Name is required.")]
        [RegularExpression("([a-zA-Z _.]{1,25})", ErrorMessage = "First Name is not in valid format.")]
        [StringLength(25,ErrorMessage="First Name must be less than 25 characters.")]
        [Display(Name="First Name")]
        public string ADMIN_NO_FNAME { get; set; }

        [RegularExpression("([a-zA-Z _.]{1,25})", ErrorMessage = "Middle Name is not in valid format.")]
        [StringLength(25, ErrorMessage = "Middle Name must be less than 25 characters.")]
        [Display(Name="Middle Name")]
        public string ADMIN_NO_MNAME { get; set; }




        [RegularExpression("([a-zA-Z _.]{1,25})", ErrorMessage = "Last Name is not in valid format.")]
        [StringLength(25, ErrorMessage = "Last Name must be less than 25 characters.")]
        [Display(Name="Last Name")]
        public string ADMIN_NO_LNAME { get; set; }


       
        [Display(Name = "Aadhaar Card No.")]
        [RegularExpression("([0-9]{1,12})", ErrorMessage = "Aadhaar Card No. is not in valid format.")]
        public string ADMIN_AADHAR_NO { get; set; }


        [Required(ErrorMessage="Please select Designation.")]
        [Range(1,2147483647,ErrorMessage=" Please select Designation.")]
        [Display(Name="Designation")]
        public Nullable<int> ADMIN_NO_DESIGNATION { get; set; }

        [RegularExpression("([a-zA-Z 0-9 &_(),-.:;/]{1,255})", ErrorMessage = "Address is not in valid format.")]
        [StringLength(255,ErrorMessage="Address must be less than 255 characters.")]
        [Display(Name="Address")]
        public string ADMIN_NO_ADDRESS1 { get; set; }

        [RegularExpression("([a-zA-Z 0-9 &_(),-.:;/]{1,255})", ErrorMessage = "Address is not in valid format.")]
        [StringLength(255, ErrorMessage = "Address must be less than 255 characters.")]
        public string ADMIN_NO_ADDRESS2 { get; set; }

        [Display(Name="District")]
        public Nullable<int> MAST_DISTRICT_CODE { get; set; }

        [Display(Name="PIN")]
        [RegularExpression("^([0-9]{6})?$", ErrorMessage = "PIN Code should contains 6 digit.")]
        [StringLength(6, ErrorMessage = "PIN Code must be 6 digits only.")]
        public string ADMIN_NO_PIN { get; set; }

        [Display(Name="Residence Phone")]
        [RegularExpression("^([0-9]+)$", ErrorMessage = "STD Code is not in valid format.")]
        [StringLength(5, ErrorMessage = "STD  Code should contains 3 to 5 digits only.", MinimumLength = 3)]
        public string ADMIN_NO_RESIDENCE_STD { get; set; }

        [Display(Name="Phone")]
        [RegularExpression("^([0-9]+)$", ErrorMessage = "Phone Number should contains digits only.")]
        [StringLength(8, ErrorMessage = "Phone Number must be 6 to 8 digits only.", MinimumLength = 6)]
        public string ADMIN_NO_RESIDENCE_PHONE { get; set; }

        [Display(Name="Office Phone")]
        [RegularExpression("^([0-9]+)$", ErrorMessage = "STD Code is not in valid format.")]
        [StringLength(5, ErrorMessage = "STD Code must be 3 to 5 digits only.", MinimumLength = 3)]
        public string ADMIN_NO_OFFICE_STD { get; set; }


        [RegularExpression("^([0-9]+)$", ErrorMessage = "Phone Number should contains digits only.")]
        [StringLength(8, ErrorMessage = "Phone Number must be 6 to 8 digits only.", MinimumLength = 6)]
        public string ADMIN_NO_OFFICE_PHONE { get; set; }

        [Display(Name="Fax")]
        [RegularExpression("^([0-9]+)$", ErrorMessage = "STD Code is not in valid format.")]
        [StringLength(5, ErrorMessage = "STD Code must be 3 to 5 digits only.", MinimumLength = 3)]
        public string ADMIN_NO_STD_FAX { get; set; }

        [RegularExpression("^([0-9]+)$", ErrorMessage = "FAX Number should contains digits only")]
        [StringLength(30, ErrorMessage = "FAX Number must be 6 to 8 digits only.", MinimumLength = 6)]
        public string ADMIN_NO_FAX { get; set; }

        [Display(Name="Mobile")]
        [RegularExpression("^[0-9]{10,11}", ErrorMessage = "Enter 10 digits Mobile number.")]
        public string ADMIN_NO_MOBILE { get; set; }

        [Display(Name="Email")]
        [RegularExpression(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$", ErrorMessage = "Email address is not in valid format.")]
        public string ADMIN_NO_EMAIL { get; set; }

        [Display (Name="Mail")]
        public string ADMIN_NO_MAIL_FLAG { get; set; }

        [Required(ErrorMessage = " Please select Type.")]
        [Range(1, 2147483647, ErrorMessage = " Please select Type.")]
        [Display(Name = "Type")]
        public int ADMIN_NO_TYPE { get; set; }

        [Display(Name="Authentication Code")]
        public string ADMIN_AUTH_CODE { get; set; }

       
        public string ADMIN_NO_LEVEL { get; set; }
        public string ADMIN_ACTIVE_STATUS { get; set; }
        public string ADMIN_MODULE { get; set; }
        public string ADMIN_NAME { get; set; }
        public string ADMIN_TYPE { get; set; }
        public string ADMIN_DESIGNATION { get; set; }
        public string ADMIN_DISTRICT { get; set; }
     
        [Required]
        [Display(Name="Start Date")]
        [RegularExpression(@"^(((0[1-9]|[12]\d|3[01])\/(0[13578]|1[02])\/((19|[2-9]\d)\d{2}))|((0[1-9]|[12]\d|30)\/(0[13456789]|1[012])\/((19|[2-9]\d)\d{2}))|((0[1-9]|1\d|2[0-8])\/02\/((19|[2-9]\d)\d{2}))|(29\/02\/((1[6-9]|[2-9]\d)(0[48]|[2468][048]|[13579][26])|((16|[2468][048]|[3579][26])00))))$", ErrorMessage = "Start date must be in dd/mm/yyyy format")]
        public string ADMIN_ACTIVE_START_DATE { get; set; }
        
        [Display(Name="End Date")]
        [RequiredEndDate("ADMIN_ACTIVE_STATUS", ErrorMessage = "The End Date is required.")]
       // [RegularExpression(@"^(((0[1-9]|[12]\d|3[01])\/(0[13578]|1[02])\/((19|[2-9]\d)\d{2}))|((0[1-9]|[12]\d|30)\/(0[13456789]|1[012])\/((19|[2-9]\d)\d{2}))|((0[1-9]|1\d|2[0-8])\/02\/((19|[2-9]\d)\d{2}))|(29\/02\/((1[6-9]|[2-9]\d)(0[48]|[2468][048]|[13579][26])|((16|[2468][048]|[3579][26])00))))$", ErrorMessage = "End date must be in dd/mm/yyyy format")]      
       // [DateValidationVST("ADMIN_ACTIVE_START_DATE", ErrorMessage = "End Date must be greater than or equal to start date.")]       
        public string ADMIN_ACTIVE_END_DATE { get; set; }

        [Display(Name="Remark")]
        [StringLength(255, ErrorMessage = "Remark must be less than 255 characters.")]
        [RegularExpression(@"^([a-zA-Z0-9 ._',&()-]+)$", ErrorMessage = "Remark is not in valid format.")]
        public string ADMIN_NO_REMARKS { get; set; }
    
        public virtual ADMIN_DEPARTMENT ADMIN_DEPARTMENT { get; set; }
        public virtual MASTER_DESIGNATION MASTER_DESIGNATION { get; set; }
        public virtual MASTER_DISTRICT MASTER_DISTRICT { get; set; }
        public virtual ICollection<MANE_IMS_INSPECTION> MANE_IMS_INSPECTION { get; set; }
       /// <summary>
       /// To get the Admin Names.
       /// </summary>
        public SelectList GetAdminNdName
        {
            get
            {
                List<ADMIN_DEPARTMENT> lstDepartment = new List<ADMIN_DEPARTMENT>();
                IMasterDAL objDAL = new MasterDAL();
                lstDepartment = objDAL.GetAdminNdCode();
                lstDepartment.Insert(0, new ADMIN_DEPARTMENT { ADMIN_ND_CODE = 0, ADMIN_ND_NAME = "--Select--" });
                return new SelectList(lstDepartment, "ADMIN_ND_CODE", "ADMIN_ND_NAME");   
            }
        }
   /// <summary>
   /// To get The Destination Names .
   /// </summary>
        public SelectList GetDesignation
        {
            get
            {
                List<MASTER_DESIGNATION> lstDesignation = new List<MASTER_DESIGNATION>();
                IMasterDAL objDAL = new MasterDAL();
                lstDesignation = objDAL.GetNodalDesignation();
                lstDesignation.Insert(0, new MASTER_DESIGNATION { MAST_DESIG_CODE = 0, MAST_DESIG_NAME = "--Select--" });
                return new SelectList(lstDesignation,"MAST_DESIG_CODE","MAST_DESIG_NAME");
            }
        }
        
        /// <summary>
        /// To get the District Names
        /// </summary>

        // public SelectList GetDistrict
        //{
        //    get
        //    {
        //        List<MASTER_DISTRICT> lstDistrict= new List<MASTER_DISTRICT>();
        //        IMasterDAL objDAL = new MasterDAL();
        //        lstDistrict = objDAL.GetDistrictName();
        //        if (PMGSYSession.Current.RoleCode == 22)
        //        {
        //            return new SelectList(lstDistrict.Where(m=>m.MAST_DISTRICT_CODE == PMGSYSession.Current.DistrictCode), "MAST_DISTRICT_CODE", "MAST_DISTRICT_NAME");
        //        }
        //        lstDistrict.Insert(0, new MASTER_DISTRICT { MAST_DISTRICT_CODE = 0, MAST_DISTRICT_NAME = "--Select--" });
        //        return new SelectList(lstDistrict,"MAST_DISTRICT_CODE","MAST_DISTRICT_NAME");
        //    }
        //}

         public SelectList GetAdminNoType
         {
             get
             {
                 List<MASTER_PROFILE> lstNoType = new List<MASTER_PROFILE>();
                 IMasterDAL objDAL = new MasterDAL();
                 lstNoType= objDAL.GetAdminNoType();
                 lstNoType.Insert(0, new MASTER_PROFILE { MAST_PROFILE_CODE = 0, MAST_PROFILE_NAME = "--Select--" });
                 return new SelectList(lstNoType, "MAST_PROFILE_CODE", "MAST_PROFILE_NAME");
             }
         }

    }

    public class RequiredEndDateAttribute : ValidationAttribute, IClientValidatable
    {
        // private const string _defaultErrorMessage = "Start date must be less than end date.";
        private string _basePropertyName;

        public RequiredEndDateAttribute(string basePropertyName)
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

            var IsDeactive = basePropertyInfo.GetValue(validationContext.ObjectInstance, null);
            var EndDate = value;

            //if (sDate != null && eDate != null)
            //{

            //Actual Validation 
            if ((IsDeactive == null) && (EndDate == null))
            {
                return ValidationResult.Success;
            }
            if ((IsDeactive.ToString() == "N") && (EndDate == null))
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
                ValidationType = "enddatevalidator"
            };
            rule.ValidationParameters["date"] = this._basePropertyName;
            yield return rule;

        }

    }
}