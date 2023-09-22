/*----------------------------------------------------------------------------------------
 * Project Id              :

 * Project Name            :OMMAS-II

 * File Name               :MasterAdminQualityMonitorViewModel.cs
 
 * Author                  : Abhishek Kamble.

 * Creation Date           :14/May/2013

 * Desc                    :This class is used to declare the variables, lists that are used in the Details form.
 * ---------------------------------------------------------------------------------------*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel.DataAnnotations;
using PMGSY.DAL.Master;
using System.Web.Mvc;
using System.Collections;
using System.Web;

namespace PMGSY.Models.Master
{
    public class MasterAdminQualityMonitorViewModel
    {
        public MasterAdminQualityMonitorViewModel()
        {
            ADMIN_SERVICE_TYPE = "D";
        }


        public string PAN_DOWNLOAD { get; set; } //12-01-2023
        public string PAN_DELETE { get; set; } //12-01-2023

        public string FILE_NAME { get; set; }

        public string FILE_PATH { get; set; }

        [Display(Name = "Upload PAN File : ")]
        //[Required(ErrorMessage = "Pdf file is Required")]
        [DataType(DataType.Upload)]
        public HttpPostedFileBase File { get; set; }



        public string STATE_NAME { get; set; }
        public string DISTRICT_NAME { get; set; }
        public string DESIGNATION_NAME { get; set; }

        [UIHint("hidden")]
        public string EncryptedQmCode { get; set; }

        public int ADMIN_QM_CODE { get; set; }

        [Display(Name = "Quality Monitor Type")]
        [RegularExpression("[IS]", ErrorMessage = "Please select Quality Monitor Type.")]
        public string ADMIN_QM_TYPE { get; set; }



        [Display(Name = "First Name")]
        [Required(ErrorMessage = "First Name is required.")]
        [RegularExpression(@"^([a-zA-Z _.]+)$", ErrorMessage = "First Name is not in valid format.")]
        [StringLength(50, ErrorMessage = "First Name must be less than 50 characters.")]
        public string ADMIN_QM_FNAME { get; set; }

        [Display(Name = "Middle Name")]
        [RegularExpression(@"^([a-zA-Z _.]+)$", ErrorMessage = "Middle Name is not in valid format.")]
        [StringLength(50, ErrorMessage = "Middle Name must be less than 50 characters.")]
        public string ADMIN_QM_MNAME { get; set; }


        [Display(Name = "Last Name")]
        [RegularExpression(@"^([a-zA-Z _.]+)$", ErrorMessage = "Last Name is not in valid format.")]
        [StringLength(50, ErrorMessage = "Last Name must be less than 50 characters.")]
        public string ADMIN_QM_LNAME { get; set; }

        [Display(Name = "Designation")]
        [Range(1, 2147483647, ErrorMessage = "Please select Designation.")]
        public Nullable<int> ADMIN_QM_DESG { get; set; }

        [Display(Name = "Address1")]
        [Required(ErrorMessage = "Address1 is required.")]
        [RegularExpression("[a-zA-Z0-9- _.&,/()]{1,255}", ErrorMessage = "Address is not in valid format.")]
        [StringLength(255, ErrorMessage = "Address must be less than 255 characters.")]
        public string ADMIN_QM_ADDRESS1 { get; set; }

        [Display(Name = "Address2")]
        [RegularExpression("[a-zA-Z0-9- _.&,/()]{1,255}", ErrorMessage = "Address is not in valid format.")]
        [StringLength(255, ErrorMessage = "Address must be less than 255 characters.")]
        public string ADMIN_QM_ADDRESS2 { get; set; }

        [Display(Name = "Home District")]
        [Range(1, Int32.MaxValue, ErrorMessage = "Please select District.")]
        public Nullable<int> MAST_DISTRICT_CODE { get; set; }


        [Display(Name = "PIN Code")]
        [RegularExpression("[0-9]{6,6}", ErrorMessage = "Enter 6 digits PIN Code.")]
        [StringLength(6, MinimumLength = 6, ErrorMessage = "PIN Code must be 6 digits only.")]
        public string ADMIN_QM_PIN { get; set; }


        [RegularExpression("[0-9]{3,5}", ErrorMessage = "STD Code should contains 3 to 5 digits only")]
        [StringLength(5, MinimumLength = 3, ErrorMessage = "STD Code must be 3 to 5 digits only.")]
        public string ADMIN_QM_STD1 { get; set; }

        [RegularExpression("[0-9]{3,5}", ErrorMessage = "STD Code should contains 3 to 5 digits only")]
        [StringLength(5, MinimumLength = 3, ErrorMessage = "STD code must be 3 to 5 digits only.")]
        public string ADMIN_QM_STD2 { get; set; }

        [Display(Name = "Phone1")]
        [RegularExpression("[0-9]{6,8}", ErrorMessage = "Phone Number should contains 6 to 8 digits only.")]
        [StringLength(8, MinimumLength = 6, ErrorMessage = "Phone Number must be 6 to 8 digits only.")]
        public string ADMIN_QM_PHONE1 { get; set; }

        [Display(Name = "Phone2")]
        [RegularExpression("[0-9]{6,8}", ErrorMessage = "Phone Number should contains 6 to 8 digits only.")]
        [StringLength(8, MinimumLength = 6, ErrorMessage = "Phone Number must be 6 to 8 digits only.")]
        public string ADMIN_QM_PHONE2 { get; set; }


        [Display(Name = "Std Fax")]
        [RegularExpression("[0-9]{3,5}", ErrorMessage = "STD Code should contains 3 to 5 digits only")]
        [StringLength(5, MinimumLength = 3, ErrorMessage = "STD Code must be 3 to 5 digits only.")]
        public string ADMIN_QM_STD_FAX { get; set; }

        [Display(Name = "Fax")]
        [RegularExpression("[0-9]{7,8}", ErrorMessage = "Fax Number should contains 7 or 8 digits only.")]
        [StringLength(8, MinimumLength = 7, ErrorMessage = "Fax Number must be 7 to 8 digits only.")]
        public string ADMIN_QM_FAX { get; set; }


        [Display(Name = "Mobile1")]
        [Required(ErrorMessage = "Mobile1 is required.")]
        [RegularExpression(@"([0-9]+)$", ErrorMessage = "Invalid Mobile number")]
        [StringLength(10, MinimumLength = 10, ErrorMessage = "Mobile number must be 10 digits only.")]
        public string ADMIN_QM_MOBILE1 { get; set; }

        [Display(Name = "Mobile2")]
        [RegularExpression(@"^([0-9]+)$", ErrorMessage = "Invalid Mobile number")]
        [StringLength(10, MinimumLength = 10, ErrorMessage = "Mobile number must be 10 digits only.")]
        public string ADMIN_QM_MOBILE2 { get; set; }

        [Display(Name = "Email")]
        [EmailAddress(ErrorMessage = "Email is not in valid format.")]
        [StringLength(50, ErrorMessage = "Email address must be less than 50 characters.")]
        [Required(ErrorMessage = "Email address is required.")]
        public string ADMIN_QM_EMAIL { get; set; }

        [Display(Name = "PAN")]
        [Required(ErrorMessage = "PAN is required.")]
        [StringLength(10, MinimumLength = 10, ErrorMessage = "PAN must be 10 alphanumeric characters only.")]
        [RegularExpression("^([a-zA-Z]){5}([0-9]){4}([a-zA-Z]){1}?$", ErrorMessage = "PAN is not in valid format.")]
        public string ADMIN_QM_PAN { get; set; }

        [RegularExpression("([0-9]{1,12})", ErrorMessage = "Aadhaar Card No. is not in valid format.")]
        [MaxLength(12, ErrorMessage = "Aadhaar Card No. is of 12 digit Number.")]
        //   [Required(ErrorMessage = "Aadhaar Card No is required.")]
        [StringLength(12, MinimumLength = 12, ErrorMessage = "Adhar must be 12 digit numeric characters only.")]
        [Display(Name = "Aadhaar Card No.")]
        public string ADMIN_QM_AADHAR_NO { get; set; }

        [Display(Name = "Designation")]
        [RegularExpression("[YN]", ErrorMessage = "Please select Designation.")]
        public string ADMIN_QM_DEG { get; set; }


        [Display(Name = "Empanelled")]
        [RegularExpression("[YN]", ErrorMessage = "Please select Empanelled.")]
        public string ADMIN_QM_EMPANELLED { get; set; }


        [Display(Name = "Empanelled Year")]
        [RequredEmpanelledYear("ADMIN_QM_EMPANELLED", ErrorMessage = "Please select empanelled year")]
        public Nullable<int> ADMIN_QM_EMPANELLED_YEAR { get; set; }

        [Display(Name = "Image Path")]
        [StringLength(50, ErrorMessage = "Image Path must be less than 50 characters.")]
        [RegularExpression(@"^([a-zA-Z0-9 _.]+)$", ErrorMessage = "Invalid image path.")]
        public string ADMIN_QM_IMAGE { get; set; }

        [Display(Name = "Document Path")]
        [StringLength(50, ErrorMessage = "Document Path must be less than 50 characters.")]
        //[RegularExpression("^([a-zA-Z0-9 :.\\]){1,50}?$", ErrorMessage = "Invalid document path.")]
        [RegularExpression(@"^([a-zA-Z0-9 _.]+)$", ErrorMessage = "Invalid document path.")]
        public string ADMIN_QM_DOCPATH { get; set; }


        [Display(Name = "Remark")]
        [StringLength(255, ErrorMessage = "Remark must be less than 255 characters.")]
        [RegularExpression(@"^([a-zA-Z0-9 ._',\r\n&()-]+)$", ErrorMessage = "Remark is not in valid format.")]
        //[RegularExpression(@"^([a-zA-Z0-9 ._',\r\n&()-]+)$", ErrorMessage = "Remark is not in valid format.")]
        public string ADMIN_QM_REMARKS { get; set; }

        [Required(ErrorMessage = "Please Enter Birth Date")]
        [Display(Name = "Birth Date")]
        [RegularExpression(@"^(((0[1-9]|[12]\d|3[01])\/(0[13578]|1[02])\/((19|[2-9]\d)\d{2}))|((0[1-9]|[12]\d|30)\/(0[13456789]|1[012])\/((19|[2-9]\d)\d{2}))|((0[1-9]|1\d|2[0-8])\/02\/((19|[2-9]\d)\d{2}))|(29\/02\/((1[6-9]|[2-9]\d)(0[48]|[2468][048]|[13579][26])|((16|[2468][048]|[3579][26])00))))$", ErrorMessage = "Inspection Date must be in dd/mm/yyyy format.")]
        public string ADMIN_QM_BIRTH_DATE { get; set; }

        [Required(ErrorMessage = "Please Select NQM Service Type")]
        [Display(Name = "NQM Service Type")]
        [RegularExpression(@"^[SCAD]", ErrorMessage = "Select Service Type.")]
        public string ADMIN_SERVICE_TYPE { get; set; }

        [Display(Name = "Empanelled Month")]
        [Required(ErrorMessage = "Empanelled Month is required.")]
        [Range(1, 12, ErrorMessage = "Select valid Month.")]//Added by deendayal on 21/06/2017
        public Nullable<int> ADMIN_QM_EMPANELLED_MONTH { get; set; }

        [Display(Name = "State")]
        [Range(0, Int32.MaxValue, ErrorMessage = "Please select State.")]
        public Nullable<int> MAST_STATE_CODE { get; set; }

        [Display(Name = "Home State")]
        [Range(1, Int32.MaxValue, ErrorMessage = "Please select State.")]               //Uncommented on 16-11-2022
        public Nullable<int> MAST_STATE_CODE_ADDR { get; set; }

        [Display(Name = "Cadre State")]
        //[Range(0, Int32.MaxValue, ErrorMessage = "Please select Cadre State.")]
        public int[] MAST_CADRE_STATE_CODE { get; set; }

        [RegularExpression(@"^[A-Za-z1-9 ]{0,2}$", ErrorMessage = "Please select DeEmpanelled Reason")]
        public string empanelledRemove { get; set; } //change 0-1-23
        public List<SelectListItem> EMPANELLED_REMOVE_LIST { get; set; } //change 0-1-23



        public string ADMIN_QM_EMPANELLED_REASON { get; set; }              //Shreyas


        [Display(Name = "Date of DeEmpanelled")]
        [Required(ErrorMessage = "Please Enter Date of DeEmpanelled.")]
        [RegularExpression(@"^(((0[1-9]|[12]\d|3[01])\/(0[13578]|1[02])\/((19|[2-9]\d)\d{2}))|((0[1-9]|[12]\d|30)\/(0[13456789]|1[012])\/((19|[2-9]\d)\d{2}))|((0[1-9]|1\d|2[0-8])\/02\/((19|[2-9]\d)\d{2}))|(29\/02\/((1[6-9]|[2-9]\d)(0[48]|[2468][048]|[13579][26])|((16|[2468][048]|[3579][26])00))))$", ErrorMessage = "Inspection Date must be in dd/mm/yyyy format.")]
        public string ADMIN_QM_DEMPANELLED_DATE { get; set; }

        #region Quality Monitor Profile Information to NQM / SQM as Present in CQC / SQC
        //Added by Hrishikesh to provide Quality Monito Profile Information to NQM/SQM as Present in CQC/SQC --start
        public string Service_Type { get; set; }
        public string Cadre_State { get; set; }
        public string Home_State { get; set; }
        public string District_Name { get; set; }
        public string Empanelled_Month { get; set; }
        public string Designation { get; set; }
        // ---end
        #endregion

        /// <summary>
        /// To get the NQM Service Type
        /// </summary>
        public SelectList ServiceTypeList
        {
            get
            {

                List<ServiceType> serviceList = new List<ServiceType>
                {
                    new ServiceType {ServiceCode="0",ServiceName="Select Service" },
                    new ServiceType {ServiceCode="S",ServiceName="State Government" },
                    new ServiceType {ServiceCode="C",ServiceName="Central Government" },
                    new ServiceType {ServiceCode="A",ServiceName="Central Agency" }
                };
                System.Reflection.FieldInfo[] fields = (new ServiceType()).GetType().GetFields();
                return new SelectList(serviceList, "ServiceCode", "ServiceName", this.ADMIN_SERVICE_TYPE);

            }

        }

        public SelectList MonthList
        {
            get
            {
                PMGSY.Common.CommonFunctions obj = new PMGSY.Common.CommonFunctions();
                return new SelectList(obj.PopulateMonths(true), "Value", "Text");
            }
        }

        /// <summary>
        /// To get the Cadre State Names 
        /// </summary>
        public MultiSelectList CadreStates
        {
            get
            {

                List<MASTER_STATE> stateList = new List<MASTER_STATE>();

                IMasterDAL objDAL = new MasterDAL();
                stateList = objDAL.GetAllStateNames();
                return new SelectList(stateList, "MAST_STATE_CODE", "MAST_STATE_NAME", this.MAST_STATE_CODE);

            }
        }

        /// <summary>
        /// To get the State Names 
        /// </summary>
        public SelectList States
        {
            get
            {

                List<MASTER_STATE> stateList = new List<MASTER_STATE>();

                IMasterDAL objDAL = new MasterDAL();
                stateList = objDAL.GetAllStateNames();

                stateList.Insert(0, new MASTER_STATE() { MAST_STATE_CODE = 0, MAST_STATE_NAME = "--Select--" });

                return new SelectList(stateList, "MAST_STATE_CODE", "MAST_STATE_NAME", this.MAST_STATE_CODE);

            }

        }

        /// <summary>
        /// To get the Districts
        /// </summary>
        public SelectList Districts
        {
            get
            {

                List<MASTER_DISTRICT> districtList = new List<MASTER_DISTRICT>();
                IMasterDAL objDAL = new MasterDAL();
                districtList = objDAL.GetAllDistrictByStateCode(Convert.ToInt32(this.MAST_STATE_CODE_ADDR));
                districtList.Insert(0, new MASTER_DISTRICT() { MAST_DISTRICT_CODE = 0, MAST_DISTRICT_NAME = "--Select--" });
                return new SelectList(districtList, "MAST_DISTRICT_CODE", "MAST_DISTRICT_NAME", this.MAST_DISTRICT_CODE);
            }
        }

        /// <summary>
        /// To get the Designations lists
        /// </summary>
        public SelectList QmDesignations
        {
            get
            {
                List<MASTER_DESIGNATION> designationList = new List<MASTER_DESIGNATION>();
                IMasterDAL objDAL = new MasterDAL();

                designationList = objDAL.GetAllQmDesignation();
                designationList.Insert(0, new MASTER_DESIGNATION() { MAST_DESIG_CODE = 0, MAST_DESIG_NAME = "--Select--" });

                return new SelectList(designationList, "MAST_DESIG_CODE", "MAST_DESIG_NAME");
            }
        }

        /// <summary>
        /// To get the list of years .
        /// </summary>
        public SelectList Years
        {

            get
            {
                List<SelectListItem> yearlst = new List<SelectListItem>();

                IMasterDAL objDAL = new MasterDAL();
                yearlst = objDAL.GetYears();
                return new SelectList(yearlst, "Value", "Text");
            }
        }

        public virtual MASTER_DESIGNATION MASTER_DESIGNATION { get; set; }
        public virtual MASTER_DISTRICT MASTER_DISTRICT { get; set; }
        public virtual MASTER_STATE MASTER_STATE { get; set; }
        public virtual MASTER_STATE MASTER_STATE1 { get; set; }

        //added by abhinav pathak 0n 17 july 2019
        [Display(Name = "DeEmpanelled Remark")]
        [StringLength(255, ErrorMessage = "Remark must be less than 255 characters.")]
        [RegularExpression(@"^([a-zA-Z0-9 ._',\r\n&()-]+)$", ErrorMessage = "Remark is not in valid format.")]
        [Required(ErrorMessage = "Please Enter DeEmpanlled Remark.")]
        public string DeEmpanelledRemark { get; set; }

        public string isOpEdit { get; set; }

        //[Display(Name = "Department")]
        //[Range(1 , int.MaxValue , ErrorMessage = "Please Select Department")] 
        //public int admin_nd_code { get; set; }

        public List<SelectListItem> AdminDepartmentList { get; set; }
    }

    public class RequredEmpanelledYearAttribute : ValidationAttribute, IClientValidatable
    {
        // private const string _defaultErrorMessage = "Start date must be less than end date.";
        private string _basePropertyName;

        public RequredEmpanelledYearAttribute(string basePropertyName)
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

            var IsEmpanelled = basePropertyInfo.GetValue(validationContext.ObjectInstance, null);
            int EmpanelledYear = (int)value;

            //if (sDate != null && eDate != null)
            //{

            //Actual Validation 
            if ((IsEmpanelled.ToString() == "Y") && (EmpanelledYear == 0))
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
                ValidationType = "empanelledyearvalidator"
            };
            rule.ValidationParameters["date"] = this._basePropertyName;
            yield return rule;

        }

    }

    public class ServiceType
    {
        public string ServiceCode { get; set; }
        public string ServiceName { get; set; }
    }


    public class Month
    {
        public string MonthCode { get; set; }
        public string MonthName { get; set; }
    }
}























///*----------------------------------------------------------------------------------------
// * Project Id              :

// * Project Name            :OMMAS-II

// * File Name               :MasterAdminQualityMonitorViewModel.cs

// * Author                  : Abhishek Kamble.

// * Creation Date           :14/May/2013

// * Desc                    :This class is used to declare the variables, lists that are used in the Details form.
// * ---------------------------------------------------------------------------------------*/
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.ComponentModel.DataAnnotations;
//using PMGSY.DAL.Master;
//using System.Web.Mvc;


//namespace PMGSY.Models.Master
//{
//    public class MasterAdminQualityMonitorViewModel
//    {
//        public string STATE_NAME { get; set; }
//        public string DISTRICT_NAME { get; set; }
//        public string DESIGNATION_NAME { get; set; }

//        [UIHint("hidden")]
//        public string EncryptedQmCode { get; set; }

//        public int ADMIN_QM_CODE { get; set; }        

//        [Display(Name="Quality Monitor Type")]
//        [RegularExpression("[IS]", ErrorMessage = "Please select Quality Monitor Type.")]
//        public string ADMIN_QM_TYPE { get; set; }

//        [Display(Name="State")]
//        [Range(1, Int32.MaxValue, ErrorMessage = "Please select State.")] 
//        public Nullable<int> MAST_STATE_CODE { get; set; }

//        [Display(Name="First Name")]
//        [Required(ErrorMessage="First Name is required.")]
//        [RegularExpression(@"^([a-zA-Z _.]+)$", ErrorMessage = "First Name is not in valid format.")]
//        [StringLength(50, ErrorMessage = "First Name must be less than 50 characters.")]
//        public string ADMIN_QM_FNAME { get; set; }

//        [Display(Name = "Middle Name")]
//        [RegularExpression(@"^([a-zA-Z _.]+)$", ErrorMessage = "Middle Name is not in valid format.")]
//        [StringLength(50, ErrorMessage = "Middle Name must be less than 50 characters.")]       
//        public string ADMIN_QM_MNAME { get; set; }

//        [RegularExpression("([0-9]{1,12})", ErrorMessage = "Aadhaar Card No. is not in valid format.")]
//        [MaxLength(12, ErrorMessage = "Aadhaar Card No. is of 12 digit Number.")]
//        [Display(Name = "Aadhaar Card No.")]
//        public string ADMIN_QM_AADHAR_NO { get; set; }


//        [Display(Name = "Last Name")]
//        [RegularExpression(@"^([a-zA-Z _.]+)$", ErrorMessage = "Last Name is not in valid format.")]
//        [StringLength(50, ErrorMessage = "Last Name must be less than 50 characters.")]        
//        public string ADMIN_QM_LNAME { get; set; }

//        [Display(Name="Designation")]
//        [Range(1,2147483647,ErrorMessage="Please select Designation.")]
//        public Nullable<int> ADMIN_QM_DESG { get; set; }

//        [Display(Name="Address1")]
//        [Required(ErrorMessage = "Address1 is required.")]
//        [RegularExpression("[a-zA-Z0-9- _.&,/()]{1,255}", ErrorMessage = "Address is not in valid format.")]
//        [StringLength(255,ErrorMessage="Address must be less than 255 characters.")]
//        public string ADMIN_QM_ADDRESS1 { get; set; }

//        [Display(Name = "Address2")]
//        [RegularExpression("[a-zA-Z0-9- _.&,/()]{1,255}", ErrorMessage = "Address is not in valid format.")]
//        [StringLength(255, ErrorMessage = "Address must be less than 255 characters.")]
//        public string ADMIN_QM_ADDRESS2 { get; set; }

//        [Display(Name = "District")]
//        [Range(1, Int32.MaxValue, ErrorMessage = "Please select District.")] 
//        public Nullable<int> MAST_DISTRICT_CODE { get; set; }

//        public Nullable<int> MAST_STATE_CODE_ADDR { get; set; }

//        [Display(Name="PIN Code")]
//        [RegularExpression("[0-9]{6,6}", ErrorMessage = "Enter 6 digits PIN Code.")]
//        [StringLength(6,MinimumLength=6,ErrorMessage="PIN Code must be 6 digits only.")]
//        public string ADMIN_QM_PIN { get; set; }


//        [RegularExpression("[0-9]{3,5}", ErrorMessage = "STD Code should contains 3 to 5 digits only")] 
//        [StringLength(5, MinimumLength = 3, ErrorMessage = "STD Code must be 3 to 5 digits only.")]        
//        public string ADMIN_QM_STD1 { get; set; }

//        [RegularExpression("[0-9]{3,5}", ErrorMessage = "STD Code should contains 3 to 5 digits only")]
//        [StringLength(5, MinimumLength = 3, ErrorMessage = "STD code must be 3 to 5 digits only.")]                
//        public string ADMIN_QM_STD2 { get; set; }

//        [Display(Name="Phone1")]
//        [RegularExpression("[0-9]{6,8}", ErrorMessage = "Phone Number should contains 6 to 8 digits only.")]
//        [StringLength(8, MinimumLength = 6, ErrorMessage = "Phone Number must be 6 to 8 digits only.")]                
//        public string ADMIN_QM_PHONE1 { get; set; }

//        [Display(Name = "Phone2")]
//        [RegularExpression("[0-9]{6,8}", ErrorMessage = "Phone Number should contains 6 to 8 digits only.")]
//        [StringLength(8, MinimumLength = 6, ErrorMessage = "Phone Number must be 6 to 8 digits only.")]                
//        public string ADMIN_QM_PHONE2 { get; set; }


//        [Display(Name = "Std Fax")]
//        [RegularExpression("[0-9]{3,5}", ErrorMessage = "STD Code should contains 3 to 5 digits only")]
//        [StringLength(5, MinimumLength = 3, ErrorMessage = "STD Code must be 3 to 5 digits only.")]                
//        public string ADMIN_QM_STD_FAX { get; set; }

//        [Display(Name = "Fax")]
//        [RegularExpression("[0-9]{7,8}", ErrorMessage = "Fax Number should contains 7 or 8 digits only.")]
//        [StringLength(8, MinimumLength = 7, ErrorMessage = "Fax Number must be 7 to 8 digits only.")]                       
//        public string ADMIN_QM_FAX { get; set; }


//        [Display(Name = "Mobile1")]
//        [Required(ErrorMessage = "Mobile1 is required.")]
//        [RegularExpression(@"([0-9]+)$", ErrorMessage = "Invalid Mobile number")]
//        [StringLength(10, MinimumLength = 10, ErrorMessage = "Mobile number must be 10 digits only.")]   
//        public string ADMIN_QM_MOBILE1 { get; set; }

//        [Display(Name = "Mobile2")]
//        [RegularExpression(@"^([0-9]+)$", ErrorMessage = "Invalid Mobile number")]
//        [StringLength(10, MinimumLength = 10, ErrorMessage = "Mobile number must be 10 digits only.")]   
//        public string ADMIN_QM_MOBILE2 { get; set; }

//        [Display(Name = "Email")]
//        [EmailAddress(ErrorMessage = "Email is not in valid format.")]
//        [StringLength(50,ErrorMessage = "Email address must be less than 50 characters.")]                                   
//        [Required(ErrorMessage = "Email address is required.")]
//        public string ADMIN_QM_EMAIL { get; set; }

//        [Display(Name = "PAN")]
//        [Required(ErrorMessage = "PAN is required.")]
//        [StringLength(10,MinimumLength=10,ErrorMessage = "PAN must be 10 alphanumeric characters only.")]
//        [RegularExpression("^([a-zA-Z]){5}([0-9]){4}([a-zA-Z]){1}?$", ErrorMessage = "PAN is not in valid format.")]
//        public string ADMIN_QM_PAN { get; set; }

//        [Display(Name = "Designation")]
//        [RegularExpression("[YN]", ErrorMessage = "Please select Designation.")]
//        public string ADMIN_QM_DEG { get; set; }


//        [Display(Name = "Empanelled")]
//        [RegularExpression("[YN]", ErrorMessage = "Please select Empanelled.")]
//        public string ADMIN_QM_EMPANELLED { get; set; }


//        [Display(Name = "Empanelled Year")]
//        [RequredEmpanelledYear("ADMIN_QM_EMPANELLED", ErrorMessage = "Please select empanelled year")]
//        public Nullable<int> ADMIN_QM_EMPANELLED_YEAR { get; set; }

//        [Display(Name = "Image Path")]
//        [StringLength(50, ErrorMessage = "Image Path must be less than 50 characters.")]
//        [RegularExpression(@"^([a-zA-Z0-9 _.]+)$", ErrorMessage = "Invalid image path.")]     
//        public string ADMIN_QM_IMAGE { get; set; }

//        [Display(Name = "Document Path")]
//        [StringLength(50, ErrorMessage = "Document Path must be less than 50 characters.")]
//        //[RegularExpression("^([a-zA-Z0-9 :.\\]){1,50}?$", ErrorMessage = "Invalid document path.")]
//        [RegularExpression(@"^([a-zA-Z0-9 _.]+)$", ErrorMessage = "Invalid document path.")]  
//        public string ADMIN_QM_DOCPATH { get; set; }


//        [Display(Name = "Remark")]
//        [StringLength(255, ErrorMessage = "Remark must be less than 255 characters.")]
//        [RegularExpression(@"^([a-zA-Z0-9 ._',\r\n&()-]+)$", ErrorMessage = "Remark is not in valid format.")]  
//        //[RegularExpression(@"^([a-zA-Z0-9 ._',\r\n&()-]+)$", ErrorMessage = "Remark is not in valid format.")]
//        public string ADMIN_QM_REMARKS { get; set; }





//        /// <summary>
//        /// To get the State Names 
//        /// </summary>
//        public SelectList States
//        {
//            get {

//                List<MASTER_STATE> stateList = new List<MASTER_STATE>();

//                IMasterDAL objDAL = new MasterDAL();
//                stateList=objDAL.GetAllStateNames();

//                stateList.Insert(0, new MASTER_STATE() { MAST_STATE_CODE = 0, MAST_STATE_NAME = "--Select--" });

//                return new SelectList(stateList, "MAST_STATE_CODE", "MAST_STATE_NAME", this.MAST_STATE_CODE);

//            }

//        }

//        /// <summary>
//        /// To get the Districts
//        /// </summary>
//        public SelectList Districts
//        {
//            get {

//                List<MASTER_DISTRICT> districtList = new List<MASTER_DISTRICT>();
//                IMasterDAL objDAL = new MasterDAL();
//                districtList = objDAL.GetAllDistrictByStateCode(Convert.ToInt32(this.MAST_STATE_CODE));
//                districtList.Insert(0, new MASTER_DISTRICT() { MAST_DISTRICT_CODE = 0, MAST_DISTRICT_NAME = "--Select--" });                
//                return new SelectList(districtList, "MAST_DISTRICT_CODE", "MAST_DISTRICT_NAME", this.MAST_DISTRICT_CODE);            
//            }
//        }

//        /// <summary>
//        /// To get the Designations lists
//        /// </summary>
//        public SelectList QmDesignations
//        {
//            get {
//                List<MASTER_DESIGNATION> designationList = new List<MASTER_DESIGNATION>();
//                IMasterDAL objDAL = new MasterDAL();

//                designationList=objDAL.GetAllQmDesignation();
//                designationList.Insert(0, new MASTER_DESIGNATION() { MAST_DESIG_CODE = 0, MAST_DESIG_NAME = "--Select--" });

//                return new SelectList(designationList, "MAST_DESIG_CODE", "MAST_DESIG_NAME");
//            }        
//        }

//        /// <summary>
//        /// To get the list of years .
//        /// </summary>
//        public SelectList Years
//        {

//            get {
//                List<SelectListItem> yearlst = new List<SelectListItem>();

//                IMasterDAL objDAL = new MasterDAL();
//                yearlst = objDAL.GetYears();
//                return new SelectList(yearlst, "Value","Text" );            
//            }        
//        }

//        public virtual MASTER_DESIGNATION MASTER_DESIGNATION { get; set; }
//        public virtual MASTER_DISTRICT MASTER_DISTRICT { get; set; }
//        public virtual MASTER_STATE MASTER_STATE { get; set; }
//        public virtual MASTER_STATE MASTER_STATE1 { get; set; }


//        //public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
//        //{
//        //    if ((this.ADMIN_QM_EMPANELLED == "Y") && (this.ADMIN_QM_EMPANELLED_YEAR == 0))
//        //    {
//        //        yield return new ValidationResult("Please select empanelled year", new[] { "ADMIN_QM_EMPANELLED_YEAR" });
//        //    }
//        //}


//    }

//    public class RequredEmpanelledYearAttribute : ValidationAttribute, IClientValidatable
//    {
//        // private const string _defaultErrorMessage = "Start date must be less than end date.";
//        private string _basePropertyName;

//        public RequredEmpanelledYearAttribute(string basePropertyName)
//        //: base(_defaultErrorMessage)
//        {
//            _basePropertyName = basePropertyName;
//        }

//        //Override default FormatErrorMessage Method  
//        public override string FormatErrorMessage(string name)
//        {
//            return string.Format(ErrorMessageString, name, _basePropertyName);
//        }

//        //Override IsValid  
//        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
//        {
//            //Get PropertyInfo Object  
//            var basePropertyInfo = validationContext.ObjectType.GetProperty(_basePropertyName);

//            var IsEmpanelled = basePropertyInfo.GetValue(validationContext.ObjectInstance, null);
//            int EmpanelledYear = (int)value;

//            //if (sDate != null && eDate != null)
//            //{

//            //Actual Validation 
//            if ((IsEmpanelled.ToString() == "Y") && (EmpanelledYear == 0))
//            {   
//                var message = FormatErrorMessage(validationContext.DisplayName);
//                return new ValidationResult(message);
//            }

//            return ValidationResult.Success;
//        }


//        public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
//        {

//            var rule = new ModelClientValidationRule
//            {
//                ErrorMessage = FormatErrorMessage(metadata.DisplayName),
//                ValidationType = "empanelledyearvalidator"
//            };
//            rule.ValidationParameters["date"] = this._basePropertyName;
//            yield return rule;

//        }

//    }

//}