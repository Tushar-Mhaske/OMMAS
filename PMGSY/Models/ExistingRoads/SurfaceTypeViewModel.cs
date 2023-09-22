using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using PMGSY.DAL.ExistingRoads;
                                                                                 
namespace PMGSY.Models.ExistingRoads
{
    public class SurfaceTypeViewModel
    {

        [UIHint("hidden")]
        public string EncryptedRoadCode { get; set; }

        // U- Update , A-Add
        public string Operation { get; set; }

        public int MAST_ER_ROAD_CODE { get; set; }

        public int MAST_SURFACE_SEG_NO { get; set; }

        [Display(Name = "Surface Type")]
        [Range(1, 2147483647, ErrorMessage = "Please select Surface Type.")]
        public int MAST_SURFACE_CODE { get; set; }
                      
        [Display(Name = "Start Chainage(in Kms.)")]
        [RegularExpression(@"^\s*(?=.*[0-9])\d*(?:\.\d{1,3})?\s*$", ErrorMessage = "Invalid Start Chainage ,Can only contains Numeric values and 3 digits after decimal place")]
        [Range(0, 9999.999, ErrorMessage = "Invalid Start Chainage.")]
        public Nullable<decimal> MAST_ER_STR_CHAIN { get; set; }

        [Display(Name = "End Chainage(in Kms.)")]
        [Required(ErrorMessage = "End Chainage is required.")]
        [RegularExpression(@"^\s*(?=.*[0-9])\d*(?:\.\d{1,3})?\s*$", ErrorMessage = "Invalid End Chainage ,Can only contains Numeric values and 3 digits after decimal place")]
        [Range(0, 9999.999, ErrorMessage = "Invalid End Chainage.")]
        [CompareChainageValidation("MAST_ER_STR_CHAIN", ErrorMessage = "End Chainage must be greater than start chainage.")]
        //[CompareSurfaceChainage("MAST_ER_STR_CHAIN", "Remaining_Length","EncryptedRoadCode", ErrorMessage = "Surface Length exceeds the Remaining Length.")]
        [CompareEndChainageValidation("MAST_ER_STR_CHAIN","Remaining_Length","EncryptedRoadCode",ErrorMessage="Surface Length exceeds the Remaining Length.")]
        public Nullable<decimal> MAST_ER_END_CHAIN { get; set; }


        [Display(Name = "Road Condition")]
        [RegularExpression(@"^[GBF]+$", ErrorMessage = "Select Proper Road Condition.")]
        [Required(ErrorMessage = "Please select Road Condition.")]
        public string MAST_ER_SURFACE_CONDITION { get; set; }

        [Display(Name = "Surface Length(in Kms.)")]
        [Range(0, 9999.999, ErrorMessage = "Invalid Surface Length.")]        
        [RegularExpression(@"^\s*(?=.*[0-9])\d*(?:\.\d{1,3})?\s*$", ErrorMessage = "Surface Length Can only contains Numeric values and 3 digits after decimal place.")]
        public decimal MAST_ER_SURFACE_LENGTH { get; set; }


        [Display(Name = "Road Number")]
        public string RoadNumber { get; set; }

        [Display(Name = "Road Name")]
        public string RoadName { get; set; }


        [Display(Name = "Start Chainage of Road")]
        public decimal StartChainageOfRoad { get; set; }



        [Display(Name = "End Chainage of Road")]
        public decimal EndChainageOfRoad { get; set; }

        [Display(Name = "Total Length Of Road")]
        public decimal SumOfAllSurfaceLength { get; set; }

        //[Display(Name = "Sum of all Surface Length should be")]
        public decimal? EditModeEndChainage{ get; set; }

        
        [Display(Name="Total Entered Surface Length")]
        public decimal SurfaceLenghEntered { get; set; }
        
        [Display(Name="Remaining Length")]
        //[CompareEndChainageValidation("MAST_ER_SURFACE_LENGTH", ErrorMessage = "Surface Length exceeds the Remaining Length.")]
        public decimal Remaining_Length { get; set; }        

        public SelectList SurfaceType
        {
            get
            {
                List<MASTER_SURFACE> surfaceList = new List<MASTER_SURFACE>();

                IExistingRoadsDAL objDAL = new ExistingRoadsDAL();

                surfaceList = objDAL.GetAllSurface();

                surfaceList.Insert(0, new MASTER_SURFACE() { MAST_SURFACE_CODE= 0, MAST_SURFACE_NAME= "--Select Surface Type--" });

                return new SelectList(surfaceList, "MAST_SURFACE_CODE", "MAST_SURFACE_NAME");
            }
        }

        public SelectList RoadCondition
        {
            get
            {
                List<SelectListItem> roadConditionList = new List<SelectListItem>();

                IExistingRoadsDAL objDAL = new ExistingRoadsDAL();

                roadConditionList = objDAL.GetRoadCondition();

                return new SelectList(roadConditionList, "Value", "Text");
            }
        }

        public virtual MASTER_EXISTING_ROADS MASTER_EXISTING_ROADS { get; set; }
        public virtual MASTER_SURFACE MASTER_SURFACE { get; set; }
        public virtual PLAN_ROAD PLAN_ROAD { get; set; }

    }


    public class CompareSurfaceChainage : ValidationAttribute, IClientValidatable
    {
        private readonly string PropertyStartChainage;
        private readonly string PropertyRemainingLength;
        private readonly string PropertyOperation;

        public CompareSurfaceChainage(string propertyEndChainage, string propertyRemainingLength, string operation)
        {
            this.PropertyStartChainage = propertyEndChainage;
            this.PropertyRemainingLength = propertyRemainingLength;
            this.PropertyOperation = operation;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var propertyTestedStartChainage = validationContext.ObjectType.GetProperty(this.PropertyStartChainage);
            var propertyTestedRemainingLength = validationContext.ObjectType.GetProperty(this.PropertyRemainingLength);
            var propertyTestedOperation = validationContext.ObjectType.GetProperty(this.PropertyOperation);
            if (propertyTestedStartChainage == null)
            {
                return new ValidationResult(string.Format("unknown property {0}", this.PropertyStartChainage));
            }

            var startChainage = Convert.ToDecimal(propertyTestedStartChainage.GetValue(validationContext.ObjectInstance, null));
            var remainingLength = Convert.ToDecimal(propertyTestedRemainingLength.GetValue(validationContext.ObjectInstance, null));
            var operation = propertyTestedOperation.GetValue(validationContext.ObjectInstance, null);
            if (operation != string.Empty)
            {
                return ValidationResult.Success;
            }

            if (value == null)
            {
                return new ValidationResult(FormatErrorMessage(validationContext.DisplayName));
            }
            var endChainage = Convert.ToDecimal(value);

            var remaining = endChainage - startChainage;

            if (remaining < remainingLength)
            {
                return ValidationResult.Success;
            }
            else
            {
                return new ValidationResult(FormatErrorMessage(validationContext.DisplayName));
            }
        }

        public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
        {
            var rule = new ModelClientValidationRule
            {
                ErrorMessage = this.ErrorMessageString,
                ValidationType = "comparechainage"
            };
            yield return rule;
        }
    }


    public class CompareChainageValidationAttribute : ValidationAttribute, IClientValidatable
    {

        // private const string _defaultErrorMessage = "Road Renewal Year must be greater than Road Costruction Year";  
        private string _basePropertyName;

        public CompareChainageValidationAttribute(string basePropertyName) //: base(_defaultErrorMessage)
        {
            _basePropertyName = basePropertyName;
        }


        //Override IsValid  
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            //Get PropertyInfo Object  
            var basePropertyInfo = validationContext.ObjectType.GetProperty(_basePropertyName);

            var getStartChainage = basePropertyInfo.GetValue(validationContext.ObjectInstance, null);
            var getEndChainage = value;

            if (getStartChainage != null && getEndChainage != null)
            {
                var startChainage = Convert.ToDecimal(getStartChainage);
                var endChainage = Convert.ToDecimal(getEndChainage);

                //Actual comparision  
                if (startChainage>= endChainage)
                {
                    var message = FormatErrorMessage(validationContext.DisplayName);
                    return new ValidationResult(message);
                }
            }
            //Default return - This means there were no validation error  
            return null;
        }

        public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
        {
            var rule = new ModelClientValidationRule
            {
                ErrorMessage = this.ErrorMessageString,
                ValidationType = "comparechainagevalidation"
            };
            yield return rule;
        }

    }


    //added by abhishek kamble 01-oct-2013
    public class CompareEndChainageValidationAttribute : ValidationAttribute, IClientValidatable
    {

        private readonly string PropertyStartChainage;
        private readonly string PropoertyRemainingLength;
        private readonly string PropertyOperation;

        // private const string _defaultErrorMessage = "Road Renewal Year must be greater than Road Costruction Year";  
        //private string _basePropertyName;
        public CompareEndChainageValidationAttribute(string propertStartChainage,string propertyRemainingLength,string propertyOperation) //: base(_defaultErrorMessage)
        {
            this.PropertyStartChainage = propertStartChainage;
            this.PropoertyRemainingLength = propertyRemainingLength;
            this.PropertyOperation = propertyOperation;
        }
        //Override IsValid  
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {

            //Get PropertyInfo Object  
            var basePropertyStartChainage = validationContext.ObjectType.GetProperty(this.PropertyStartChainage);
            var basePropertyRemainingLength = validationContext.ObjectType.GetProperty(this.PropoertyRemainingLength);
            var basePropertyOperation = validationContext.ObjectType.GetProperty(this.PropertyOperation);

            if (basePropertyStartChainage == null)
            {
                return new ValidationResult(string.Format("unknown property{0}",this.PropertyStartChainage));
            }               
            var startChainage = Convert.ToDecimal(basePropertyStartChainage.GetValue(validationContext.ObjectInstance,null));
            var remainingLEngth = Convert.ToDecimal(basePropertyRemainingLength.GetValue(validationContext.ObjectInstance, null));
            var operation = basePropertyOperation.GetValue(validationContext.ObjectInstance, null);

            if (operation != null)
            {
                return ValidationResult.Success;
            }

            if (value == null)
            {
                return new ValidationResult(FormatErrorMessage(validationContext.DisplayName));
            }

            var endChainage = Convert.ToDecimal(value);
            //var endChainage = Convert.ToDecimal(60);

            var remaining = endChainage - startChainage;

            if ((remaining <= remainingLEngth) && (remaining != 0))
            {
                return ValidationResult.Success;
            }
            else {
                return new ValidationResult(FormatErrorMessage(validationContext.DisplayName));
            }
        }

        public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
        {
            var rule = new ModelClientValidationRule
            {
                ErrorMessage = this.ErrorMessageString,
                ValidationType = "compareendchainagevalidation"
            };
            yield return rule;
        }

    }


}