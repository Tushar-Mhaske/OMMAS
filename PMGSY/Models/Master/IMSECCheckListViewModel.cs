using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PMGSY.Common;
using PMGSY.Extensions;
using System.Web.Mvc;
using System.ComponentModel.DataAnnotations;
using PMGSY.DAL.Master;

namespace PMGSY.Models.Master
{
    public class IMSECCheckListViewModel
    {
        public IMSECCheckListViewModel()
        {
            PMGSYEntities dbContext = new PMGSYEntities(); 
            CommonFunctions commonFunctions = new CommonFunctions();
            MasterDAL objDal = new MasterDAL();
            StateList = new List<SelectListItem>();
            StateName = PMGSYSession.Current.StateCode == 0 ? "0" : PMGSYSession.Current.StateName.Trim();
            StateList = commonFunctions.PopulateStates(true);
            // StateList.Insert(0, (new SelectListItem { Text = "All States", Value = "0"}));
            StateCode = PMGSYSession.Current.StateCode == 0 ? 0 : PMGSYSession.Current.StateCode;
            StateList.Find(x => x.Value == StateCode.ToString()).Selected = true;
            BatchList = commonFunctions.PopulateBatch(false);
            BatchList.Find(x => x.Value == "-1").Value = "0";
            Mast_AgencyList = new List<SelectListItem>();
            if (StateCode > 0)
            {
                if (PMGSYSession.Current.AdminNdCode > 0)
                {
                    Mast_AgencyList = objDal.PopulateAgenciesByDepartementWise(StateCode, PMGSYSession.Current.AdminNdCode, false);
                    var query = (from ma in dbContext.MASTER_AGENCY
                                 join md in dbContext.ADMIN_DEPARTMENT on ma.MAST_AGENCY_CODE equals md.MAST_AGENCY_CODE
                                 where md.MAST_STATE_CODE == PMGSYSession.Current.StateCode &&
                                 md.MAST_ND_TYPE == "S" &&
                                 md.ADMIN_ND_CODE == PMGSYSession.Current.AdminNdCode
                                 select new
                                 {
                                     ma.MAST_AGENCY_CODE
                                 }).FirstOrDefault();
                   Mast_Agency = query == null ? 0 : query.MAST_AGENCY_CODE;
                }
                else
                {
                    Mast_AgencyList = objDal.PopulateAgencies(StateCode, false);
                }
            }
            else
            {
                Mast_AgencyList.Insert(0, (new SelectListItem { Text = "Select Agency", Value = "0" }));
            }
           // Mast_AgencyList.Find(x => x.Value == "-1").Value = "0";
            PhaseYear = DateTime.Now.Year;
            PhaseYearList = new SelectList(commonFunctions.PopulateFinancialYear(false, false), "Value", "Text").ToList();
            RoleCodeStringSRRDA =Convert.ToString(PMGSYSession.Current.RoleCode)=="2"?"true":"false";
            RoleCodeStringMORD = Convert.ToString(PMGSYSession.Current.RoleCode) == "25" ? "true" : "false";          
           
        }

        public string StateName { get; set; }
        public string Agency_Name { get; set; }
        public string BatchName { get; set; }
        public string yearName { get; set; }
        public string HdRoleTypeEntry { get; set; }
        [UIHint("hidden")]
        public string EncryptedMastEcId { get; set; }
        public string RoleCodeStringSRRDA { get; set; }
        public string RoleCodeStringMORD { get; set; }
        public int MAST_EC_ID { get; set; }

        [Display(Name = "State")]
        [Required(ErrorMessage = "Please select State. ")]
        [Range(1, int.MaxValue, ErrorMessage = "Please select  State.")]
        public int StateCode { get; set; }
        public List<SelectListItem> StateList { get; set; }

        [Display(Name = "Year")]
        [Range(1, 2090, ErrorMessage = "Please select Year.")]
        [Required(ErrorMessage = "Please select Year.")]
        [RegularExpression("^([0-9]+)$", ErrorMessage = "Year must be valid number.")]
        public int PhaseYear { get; set; }
        public List<SelectListItem> PhaseYearList { get; set; }

        [Display(Name = "Batch")]
        [Range(1, 10, ErrorMessage = "Please select Batch.")]
        [Required(ErrorMessage = "Please select Batch.")]
        [RegularExpression("^([0-9]+)$", ErrorMessage = "Batch must be valid number.")]
        public int Batch { get; set; }
        public List<SelectListItem> BatchList { get; set; }

        [Display(Name = "Agency")]
        [Range(1, int.MaxValue, ErrorMessage = "Please select Agency.")]
        [Required(ErrorMessage = "Please select Agency.")]
        public int Mast_Agency { get; set; }
        public List<SelectListItem> Mast_AgencyList { get; set; }

        [Display(Name = "IMS_CN_READY")]
        [RegularExpression(@"^([YN]+)$", ErrorMessage = "Please select Yes or No.")]
        [CheckRoleWiseValidation("RoleCodeStringSRRDA", ErrorMessage = "Please select Yes or No.")]
        public string IMS_CN_READY { get; set; }

        [Display(Name = "IMS_DRRP_OMMAS")]
        [RegularExpression(@"^([YN]+)$", ErrorMessage = "Please select Yes or No.")]
        [CheckRoleWiseValidation("RoleCodeStringSRRDA", ErrorMessage = "Please select Yes or No.")]        
        public string IMS_DRRP_OMMAS { get; set; }

        [Display(Name = "IMS_CNCUPL_READY")]
        [RegularExpression(@"^([YN]+)$", ErrorMessage = "Please select Yes or No.")]
        [CheckRoleWiseValidation("RoleCodeStringSRRDA", ErrorMessage = "Please select Yes or No.")]
        public string IMS_CNCUPL_READY { get; set; }

        [Display(Name = "IMS_DP_APPROVAL")]
        [RegularExpression(@"^([YN]+)$", ErrorMessage = "Please select Yes or No.")]
        [CheckRoleWiseValidation("RoleCodeStringSRRDA", ErrorMessage = "Please select Yes or No.")]
        public string IMS_DP_APPROVAL { get; set; }

        [Display(Name = "IMS_SLSC_PROCEEDING")]
        [RegularExpression(@"^([YN]+)$", ErrorMessage = "Please select Yes or No.")]
        [CheckRoleWiseValidation("RoleCodeStringSRRDA", ErrorMessage = "Please select Yes or No.")]
        public string IMS_SLSC_PROCEEDING { get; set; }

        [Display(Name = "IMS_DPR_SCRUTINY")]
        [RegularExpression(@"^([YN]+)$", ErrorMessage = "Please select Yes or No.")]
        [CheckRoleWiseValidation("RoleCodeStringSRRDA", ErrorMessage = "Please select Yes or No.")]
        public string IMS_DPR_SCRUTINY { get; set; }

        [Display(Name = "IMS_PCI_REGISTER")]
        [RegularExpression(@"^([YN]+)$", ErrorMessage = "Please select Yes or No.")]
        [CheckRoleWiseValidation("RoleCodeStringSRRDA", ErrorMessage = "Please select Yes or No.")]
        public string IMS_PCI_REGISTER { get; set; }

        [Display(Name = "IMS_UNSEALED")]
        [RegularExpression(@"^([YN]+)$", ErrorMessage = "Please select Yes or No.")]
        [CheckRoleWiseValidation("RoleCodeStringSRRDA", ErrorMessage = "Please select Yes or No.")]
        public string IMS_UNSEALED { get; set; }

        [Display(Name = "IMS_MP_DATA")]
        [RegularExpression(@"^([YN]+)$", ErrorMessage = "Please select Yes or No.")]
        [CheckRoleWiseValidation("RoleCodeStringSRRDA", ErrorMessage = "Please select Yes or No.")]
        public string IMS_MP_DATA { get; set; }

        [Display(Name = "IMS_MAINT_YEARWISE")]
        [RegularExpression(@"^([YN]+)$", ErrorMessage = "Please select Yes or No.")]
        [CheckRoleWiseValidation("RoleCodeStringSRRDA", ErrorMessage = "Please select Yes or No.")]
        public string IMS_MAINT_YEARWISE { get; set; }

        [Display(Name = "IMS_ESTIMATE_SSR")]
        [RegularExpression(@"^([YN]+)$", ErrorMessage = "Please select Yes or No.")]
        [CheckRoleWiseValidation("RoleCodeStringSRRDA", ErrorMessage = "Please select Yes or No.")]
        public string IMS_ESTIMATE_SSR { get; set; }

        [Display(Name = "IMS_SSR_DATE")]
        //[Required(ErrorMessage = "Please select Date.")]      
        [RegularExpression(@"^(((0[1-9]|[12]\d|3[01])\/(0[13578]|1[02])\/((19|[2-9]\d)\d{2}))|((0[1-9]|[12]\d|30)\/(0[13456789]|1[012])\/((19|[2-9]\d)\d{2}))|((0[1-9]|1\d|2[0-8])\/02\/((19|[2-9]\d)\d{2}))|(29\/02\/((1[6-9]|[2-9]\d)(0[48]|[2468][048]|[13579][26])|((16|[2468][048]|[3579][26])00))))$", ErrorMessage = "SSR Date must be in dd/mm/yyyy format.")]
        [CheckRoleWiseValidation("RoleCodeStringSRRDA", ErrorMessage = "Please enter SSR Date.")]
        public string IMS_SSR_DATE { get; set; }
      

        [Display(Name = "IMS_DPR_STA")]
        [RegularExpression(@"^([YN]+)$", ErrorMessage = "Please select Yes or No.")]
        [CheckRoleWiseValidation("RoleCodeStringSRRDA", ErrorMessage = "Please select Yes or No.")]
        public string IMS_DPR_STA { get; set; }

        [Display(Name = "IMS_NIT_UPLOADED")]
        [RegularExpression(@"^([YN]+)$", ErrorMessage = "Please select Yes or No.")]
        [CheckRoleWiseValidation("RoleCodeStringSRRDA", ErrorMessage = "Please select Yes or No.")]
        public string IMS_NIT_UPLOADED { get; set; }

        [Display(Name = "IMS_WORK_CAPACITY")]
        [RegularExpression(@"^([YN]+)$", ErrorMessage = "Please select Yes or No.")]
        [CheckRoleWiseValidation("RoleCodeStringSRRDA", ErrorMessage = "Please select Yes or No.")]
        public string IMS_WORK_CAPACITY { get; set; }

        [Display(Name = "IMS_IPAI_ACCOUNTS")]
        [RegularExpression(@"^([YN]+)$", ErrorMessage = "Please select Yes or No.")]
        [CheckRoleWiseValidation("RoleCodeStringSRRDA", ErrorMessage = "Please select Yes or No.")]
        public string IMS_IPAI_ACCOUNTS { get; set; }

        [Display(Name = "IMS_LWE_MHA")]
        [RegularExpression(@"^([YN]+)$", ErrorMessage = "Please select Yes or No.")]
        [CheckRoleWiseValidation("RoleCodeStringSRRDA", ErrorMessage = "Please select Yes or No.")]
        public string IMS_LWE_MHA { get; set; }

        [Display(Name = "IMS_WB_BATCH_SIZE")]
        [RegularExpression(@"^([YN]+)$", ErrorMessage = "Please select Yes or No.")]
        [CheckRoleWiseValidation("RoleCodeStringSRRDA", ErrorMessage = "Please select Yes or No.")]
        public string IMS_WB_BATCH_SIZE { get; set; }

        [Display(Name = "IMS_WB_ECOP")]
        [RegularExpression(@"^([YN]+)$", ErrorMessage = "Please select Yes or No.")]
        [CheckRoleWiseValidation("RoleCodeStringSRRDA", ErrorMessage = "Please select Yes or No.")]
        public string IMS_WB_ECOP { get; set; }

        [Display(Name = "IMS_WB_STA_CLEARED")]
        [RegularExpression(@"^([YN]+)$", ErrorMessage = "Please select Yes or No.")]
        [CheckRoleWiseValidation("RoleCodeStringSRRDA", ErrorMessage = "Please select Yes or No.")]
        public string IMS_WB_STA_CLEARED { get; set; }




        [Display(Name = "IMS_CN_APPROVAL_DATE")]
        [RegularExpression(@"^(((0[1-9]|[12]\d|3[01])\/(0[13578]|1[02])\/((19|[2-9]\d)\d{2}))|((0[1-9]|[12]\d|30)\/(0[13456789]|1[012])\/((19|[2-9]\d)\d{2}))|((0[1-9]|1\d|2[0-8])\/02\/((19|[2-9]\d)\d{2}))|(29\/02\/((1[6-9]|[2-9]\d)(0[48]|[2468][048]|[13579][26])|((16|[2468][048]|[3579][26])00))))$", ErrorMessage = "CN Approval Date must be in dd/mm/yyyy format.")]
       [Required(ErrorMessage="Please enter CN approval Date")]
        public string IMS_CN_APPROVAL_DATE { get; set; }


        [Display(Name = "IMS_CE_REMARKS")]
        [CheckRoleWiseValidation("RoleCodeStringSRRDA", ErrorMessage = "Please enter SRRDA Remark.")]      
        [RegularExpression(@"^([a-zA-Z0-9 ._';\r\n,&()-]+)$", ErrorMessage = "Remark is not in valid format.")]
        [StringLength(255, ErrorMessage = "Remark must be less than 255 characters.")]
        public string IMS_CE_REMARKS { get; set; }

      

        public string IMS_EC_TYPE { get; set; }

        //NRRDA 
        [Display(Name = "IMS_NRRDA_REMARKS")]
        [CheckRoleWiseValidation("RoleCodeStringMORD", ErrorMessage = "Please enter NRRDA Remark.")]
        [RegularExpression(@"^([a-zA-Z0-9 ._';\r\n,&()-]+)$", ErrorMessage = "Remark is not in valid format.")]
        [StringLength(255, ErrorMessage = "Remark must be less than 255 characters.")]
        public string IMS_NRRDA_REMARKS { get; set; }
   
        [Display(Name = "IMS_CN_READY_NRRDA")]
        [RegularExpression(@"^([YN]+)$", ErrorMessage = "Please select Yes or No.")]
        [CheckRoleWiseValidation("RoleCodeStringMORD", ErrorMessage = "Please select Yes or No.")]
        public string IMS_CN_READY_NRRDA { get; set; }

        [Display(Name = "IMS_DRRP_OMMAS_NRRDA")]
        [RegularExpression(@"^([YN]+)$", ErrorMessage = "Please select Yes or No.")]
        [CheckRoleWiseValidation("RoleCodeStringMORD", ErrorMessage = "Please select Yes or No.")]
        public string IMS_DRRP_OMMAS_NRRDA { get; set; }

        [Display(Name = "IMS_CNCUPL_READY_NRRDA")]
        [RegularExpression(@"^([YN]+)$", ErrorMessage = "Please select Yes or No.")]
        [CheckRoleWiseValidation("RoleCodeStringMORD", ErrorMessage = "Please select Yes or No.")]
        public string IMS_CNCUPL_READY_NRRDA { get; set; }

        [Display(Name = "IMS_DP_APPROVAL_NRRDA")]
        [RegularExpression(@"^([YN]+)$", ErrorMessage = "Please select Yes or No.")]
        [CheckRoleWiseValidation("RoleCodeStringMORD", ErrorMessage = "Please select Yes or No.")]
        public string IMS_DP_APPROVAL_NRRDA { get; set; }

        [Display(Name = "IMS_SLSC_PROCEEDING_NRRDA")]
        [RegularExpression(@"^([YN]+)$", ErrorMessage = "Please select Yes or No.")]
        [CheckRoleWiseValidation("RoleCodeStringMORD", ErrorMessage = "Please select Yes or No.")]
        public string IMS_SLSC_PROCEEDING_NRRDA { get; set; }

        [Display(Name = "IMS_DPR_SCRUTINY_NRRDA")]
        [RegularExpression(@"^([YN]+)$", ErrorMessage = "Please select Yes or No.")]
        [CheckRoleWiseValidation("RoleCodeStringMORD", ErrorMessage = "Please select Yes or No.")]
        public string IMS_DPR_SCRUTINY_NRRDA { get; set; }

        [Display(Name = "IMS_PCI_REGISTER_NRRDA")]
        [RegularExpression(@"^([YN]+)$", ErrorMessage = "Please select Yes or No.")]
        [CheckRoleWiseValidation("RoleCodeStringMORD", ErrorMessage = "Please select Yes or No.")]
        public string IMS_PCI_REGISTER_NRRDA { get; set; }

        [Display(Name = "IMS_UNSEALED_NRRDA")]
        [RegularExpression(@"^([YN]+)$", ErrorMessage = "Please select Yes or No.")]
        [CheckRoleWiseValidation("RoleCodeStringMORD", ErrorMessage = "Please select Yes or No.")]
        public string IMS_UNSEALED_NRRDA { get; set; }

        [Display(Name = "IMS_MP_DATA_NRRDA")]
        [RegularExpression(@"^([YN]+)$", ErrorMessage = "Please select Yes or No.")]
        [CheckRoleWiseValidation("RoleCodeStringMORD", ErrorMessage = "Please select Yes or No.")]
        public string IMS_MP_DATA_NRRDA { get; set; }

        [Display(Name = "IMS_MAINT_YEARWISE_NRRDA")]
        [RegularExpression(@"^([YN]+)$", ErrorMessage = "Please select Yes or No.")]
        [CheckRoleWiseValidation("RoleCodeStringMORD", ErrorMessage = "Please select Yes or No.")]
        public string IMS_MAINT_YEARWISE_NRRDA { get; set; }

        [Display(Name = "IMS_ESTIMATE_SSR_NRRDA")]
        [RegularExpression(@"^([YN]+)$", ErrorMessage = "Please select Yes or No.")]
        [CheckRoleWiseValidation("RoleCodeStringMORD", ErrorMessage = "Please select Yes or No.")]
        public string IMS_ESTIMATE_SSR_NRRDA { get; set; }

        [Display(Name = "IMS_SSR_DATE_NRRDA")]
        //[Required(ErrorMessage = "Please select Date.")]      
        [RegularExpression(@"^(((0[1-9]|[12]\d|3[01])\/(0[13578]|1[02])\/((19|[2-9]\d)\d{2}))|((0[1-9]|[12]\d|30)\/(0[13456789]|1[012])\/((19|[2-9]\d)\d{2}))|((0[1-9]|1\d|2[0-8])\/02\/((19|[2-9]\d)\d{2}))|(29\/02\/((1[6-9]|[2-9]\d)(0[48]|[2468][048]|[13579][26])|((16|[2468][048]|[3579][26])00))))$", ErrorMessage = "SSR Date must be in dd/mm/yyyy format.")]
        [CheckRoleWiseValidation("RoleCodeStringMORD", ErrorMessage = "Please enter SSR Date.")]
        public string IMS_SSR_DATE_NRRDA { get; set; }


        [Display(Name = "IMS_DPR_STA_NRRDA")]
        [RegularExpression(@"^([YN]+)$", ErrorMessage = "Please select Yes or No.")]
        [CheckRoleWiseValidation("RoleCodeStringMORD", ErrorMessage = "Please select Yes or No.")]
        public string IMS_DPR_STA_NRRDA { get; set; }

        [Display(Name = "IMS_NIT_UPLOADED_NRRDA")]
        [RegularExpression(@"^([YN]+)$", ErrorMessage = "Please select Yes or No.")]
        [CheckRoleWiseValidation("RoleCodeStringMORD", ErrorMessage = "Please select Yes or No.")]
        public string IMS_NIT_UPLOADED_NRRDA { get; set; }

        [Display(Name = "IMS_WORK_CAPACITY_NRRDA")]
        [RegularExpression(@"^([YN]+)$", ErrorMessage = "Please select Yes or No.")]
        [CheckRoleWiseValidation("RoleCodeStringMORD", ErrorMessage = "Please select Yes or No .")]
        public string IMS_WORK_CAPACITY_NRRDA { get; set; }
     
        [Display(Name = "IMS_IPAI_ACCOUNTS_NRRDA")]
        [RegularExpression(@"^([YN]+)$", ErrorMessage = "Please select Yes or No.")]
        [CheckRoleWiseValidation("RoleCodeStringMORD", ErrorMessage = "Please select Yes or No .")]
        public string IMS_IPAI_ACCOUNTS_NRRDA { get; set; }

        [Display(Name = "IMS_LWE_MHA_NRRDA")]
        [RegularExpression(@"^([YN]+)$", ErrorMessage = "Please select Yes or No.")]
        [CheckRoleWiseValidation("RoleCodeStringMORD", ErrorMessage = "Please select Yes or No.")]
        public string IMS_LWE_MHA_NRRDA { get; set; }

        [Display(Name = "IMS_WB_BATCH_SIZE_NRRDA")]
        [RegularExpression(@"^([YN]+)$", ErrorMessage = "Please select Yes or No.")]
        [CheckRoleWiseValidation("RoleCodeStringMORD", ErrorMessage = "Please select Yes or No.")]
        public string IMS_WB_BATCH_SIZE_NRRDA { get; set; }

        [Display(Name = "IMS_WB_ECOP_NRRDA")]
        [RegularExpression(@"^([YN]+)$", ErrorMessage = "Please select Yes or No.")]
        [CheckRoleWiseValidation("RoleCodeStringMORD", ErrorMessage = "Please select Yes or No.")]
        public string IMS_WB_ECOP_NRRDA { get; set; }

        [Display(Name = "IMS_WB_STA_CLEARED_NRRDA")]
        [RegularExpression(@"^([YN]+)$", ErrorMessage = "Please select Yes or No.")]
        [CheckRoleWiseValidation("RoleCodeStringMORD", ErrorMessage = "Please select Yes or No.")]
        public string IMS_WB_STA_CLEARED_NRRDA { get; set; }

        public string IMS_EC_File_Name { get; set; }
        public string IMS_EC_File_Path { get; set; }
      

    }

    public class IMSECCheckListSearchViewModel
    {
         
        public IMSECCheckListSearchViewModel()
        {
            
            CommonFunctions commonFunctions = new CommonFunctions();
            MasterDAL objDal = new MasterDAL();
            StateList = new List<SelectListItem>();
            StateName = PMGSYSession.Current.StateCode == 0 ? "0" : PMGSYSession.Current.StateName.Trim();
            StateList = commonFunctions.PopulateStates(false);
            StateList.Insert(0, (new SelectListItem { Text = "All States", Value = "0"}));
            StateCode = PMGSYSession.Current.StateCode == 0 ? 0 : PMGSYSession.Current.StateCode;
            StateList.Find(x => x.Value == StateCode.ToString()).Selected = true;
            BatchList = commonFunctions.PopulateBatch(true);
            Mast_AgencyList = new List<SelectListItem>();
            if (StateCode > 0)
            {
                if (PMGSYSession.Current.AdminNdCode > 0)
                {
                    Mast_AgencyList = objDal.PopulateAgenciesByDepartementWise(StateCode, PMGSYSession.Current.AdminNdCode, true);
                   
               
              
                }
                else
                {
                    Mast_AgencyList = objDal.PopulateAgencies(StateCode, true);
                }
               
            }
            else
            {
                Mast_AgencyList.Insert(0, (new SelectListItem { Text = "All Agencies", Value = "0" }));
            }
          
          
            PhaseYear = DateTime.Now.Year;
            PhaseYearList = new SelectList(commonFunctions.PopulateFinancialYear(true, true), "Value", "Text").ToList();

        }


        public string StateName { get; set; }   

        public int MAST_EC_ID { get; set; }

        [Display(Name = "State")]
        [Required(ErrorMessage = "Please select State. ")]
        [Range(0, int.MaxValue, ErrorMessage = "Please select  State.")]
        public int StateCode { get; set; }
        public List<SelectListItem> StateList { get; set; }

        [Display(Name = "Year")]
        [Range(0, 2090, ErrorMessage = "Please select Year.")]
        [Required(ErrorMessage = "Please select Year.")]
        [RegularExpression("^([0-9]+)$", ErrorMessage = "Year must be valid number.")]
        public int PhaseYear { get; set; }
        public List<SelectListItem> PhaseYearList { get; set; }

        [Display(Name = "Batch")]
        [Range(0, 10, ErrorMessage = "Please select Batch.")]
        [Required(ErrorMessage = "Please select Batch.")]
        [RegularExpression("^([0-9]+)$", ErrorMessage = "Batch must be valid number.")]
        public int Batch { get; set; }
        public List<SelectListItem> BatchList { get; set; }

        [Display(Name = "Agency")]
        [Range(0, int.MaxValue, ErrorMessage = "Please select Agency.")]
        [Required(ErrorMessage = "Please select Agency.")]
        public int Mast_Agency { get; set; }
        public List<SelectListItem> Mast_AgencyList { get; set; }
      

    }

    public class CheckRoleWiseValidationAttribute : ValidationAttribute, IClientValidatable
    {
        // private const string _defaultErrorMessage = "Start date must be less than end date.";
        private string _basePropertyName;

        public CheckRoleWiseValidationAttribute(string basePropertyName)
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

            var IsEmpanelled = basePropertyInfo.GetValue(validationContext.ObjectInstance, null); // check value is null or empty
            string EmpanelledYear = (string)value;

            //if (sDate != null && eDate != null)
            //{

            //Actual Validation 
            if ((IsEmpanelled.ToString() == "true") && (EmpanelledYear == null))
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
                ValidationType = "checkrolewisevalidationattribute"
            };
            //rule.ValidationParameters["date"] = this._basePropertyName;
            rule.ValidationParameters.Add("previousval", this._basePropertyName);
            
            //yield return rule;
            return new[] { rule };

        }

    }
}