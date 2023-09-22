using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using PMGSY.DAL.ExistingRoads;


namespace PMGSY.Models.ExistingRoads
{


    public class ExistingRoadsViewModel
    {
        public ExistingRoadsViewModel()
        {
            this.MANE_ER_PCI_INDEX = new HashSet<MANE_ER_PCI_INDEX>();
            this.MASTER_ER_CBR_VALUE = new HashSet<MASTER_ER_CBR_VALUE>();
            this.MASTER_ER_CDWORKS_ROAD = new HashSet<MASTER_ER_CDWORKS_ROAD>();
            this.MASTER_ER_HABITATION_ROAD = new HashSet<MASTER_ER_HABITATION_ROAD>();
            this.MASTER_ER_SURFACE_TYPES = new HashSet<MASTER_ER_SURFACE_TYPES>();
            this.MASTER_ER_TRAFFIC_INTENSITY = new HashSet<MASTER_ER_TRAFFIC_INTENSITY>();
            this.PLAN_ROAD = new HashSet<PLAN_ROAD>();
        }

        [UIHint("hidden")]
        public string EncryptedRoadCode { get; set; }

        public int hdnRoadCategoryCode { get; set; }
        public string hdn_MAST_ER_ROAD_NUMBER { get; set; }
        public int MAST_ER_ROAD_CODE_PMGSY1 { get; set; }

        public string CategoryOfRoadName { get; set; }
        public string OwnerOfRoadName { get; set; }
        public string SoilTypeName { get; set; }
        public string TerrainTypeName { get; set; }

        public string BlockName { get; set; }

        public int isSurfaceCbrDetails { get; set; }


        public int MAST_ER_ROAD_CODE { get; set; }

        [Display(Name = "State")]
        public int MAST_STATE_CODE { get; set; }

        [Display(Name = "District")]
        public int MAST_DISTRICT_CODE { get; set; }

        [Display(Name = "Block")]
        public int MAST_BLOCK_CODE { get; set; }

        public string MAST_ER_SHORT_DESC { get; set; }

        [Display(Name = "Road Number")]
        [Required(ErrorMessage = " Road Code is required.")]
        [StringLength(15, ErrorMessage = "Road number must be less than 15 characters.")]
        [RegularExpression("[A-Za-z0-9 ./()-]{1,15}", ErrorMessage = "Only alphanumeric characters are allowed.")]
        public string MAST_ER_ROAD_NUMBER { get; set; }

        [Display(Name = "Category of Road")]
        [Range(1, 2147483647, ErrorMessage = " Please select road category.")]
        public int MAST_ROAD_CAT_CODE { get; set; }

        [Display(Name = "Road Name")]
        [Required(ErrorMessage = "Road Name is required.")]
        [RegularExpression("^([,A-Za-z0-9 ()-./]{1,60})$", ErrorMessage = "Enter valid Road Name.Only alphanumeric characters are allowed.")]
        [StringLength(200, ErrorMessage = "Road name must be less than 200 characters.")]
        public string MAST_ER_ROAD_NAME { get; set; }

        [Display(Name = "Start Chainage")]
        [Required(ErrorMessage = " Road Start Chainage is required.")]
        [RegularExpression(@"^\s*(?=.*[0-9])\d*(?:\.\d{1,3})?\s*$", ErrorMessage = "Invalid Start Chainage ,Can only contains Numeric values and 3 digits after decimal place")]
        [Range(0.000, 9999.999, ErrorMessage = "Invalid Start Chainage")]
        public decimal MAST_ER_ROAD_STR_CHAIN { get; set; }

        [CompareValidation("MAST_ER_ROAD_STR_CHAIN", ErrorMessage = "Road end chainage should be greater than Road start chainage.")]
        [Display(Name = "End Chainage")]
        [Required(ErrorMessage = " Road End Chainage is required.")]
        [RegularExpression(@"^\s*(?=.*[0-9])\d*(?:\.\d{1,3})?\s*$", ErrorMessage = "Invalid End Chainage ,Can only contains Numeric values and 3 digits after decimal place")]
        [Range(0.000, 9999.999, ErrorMessage = "Invalid End Chainage")]
        public decimal MAST_ER_ROAD_END_CHAIN { get; set; }

        [CompareValidation("MAST_ER_ROAD_F_WIDTH", ErrorMessage = "Land Width should be greater than or equal to Formation Width.")]
        [Display(Name = "Land Width")]
        [RegularExpression(@"^\s*(?=.*[0-9])\d*(?:\.\d{1,3})?\s*$", ErrorMessage = "Invalid Land Width,Can only contains Numeric values and 3 digits after decimal place")]
        [Range(0.001, 9999.999, ErrorMessage = "Invalid Land Width.")]
        [Required(ErrorMessage = "Land Width is required.")]
        public Nullable<decimal> MAST_ER_ROAD_L_WIDTH { get; set; }

        [CompareValidation("MAST_ER_ROAD_C_WIDTH", ErrorMessage = "Formation Width should be greater than or equal to Carriageway Width.")]
        [Display(Name = "Formation Width")]
        [RegularExpression(@"^\s*(?=.*[0-9])\d*(?:\.\d{1,3})?\s*$", ErrorMessage = "Invalid Formation Width,Can only contains Numeric values and 3 digits after decimal place")]
        [Range(0.001, 9999.999, ErrorMessage = "Invalid Formation Width.")]
        [Required(ErrorMessage = "Formation Width is required.")]
        public Nullable<decimal> MAST_ER_ROAD_F_WIDTH { get; set; }

        [Display(Name = "Carriageway Width")]
        [RegularExpression(@"^\s*(?=.*[0-9])\d*(?:\.\d{1,3})?\s*$", ErrorMessage = "Invalid Carriageway Width,Can only contains Numeric values and 3 digits after decimal place")]
        [Range(0.001, 9999.999, ErrorMessage = "Invalid Carriageway Width.")]
        [Required(ErrorMessage = "Carriageway Width is required.")]
        public Nullable<decimal> MAST_ER_ROAD_C_WIDTH { get; set; }

        [Display(Name = "Road Type")]
        [Required(ErrorMessage = "Please select Road Type")]
        [RegularExpression(@"^([AF]+)$", ErrorMessage = "Please select Road Type.")]
        public string MAST_ER_ROAD_TYPE { get; set; }

        [Display(Name = "Soil Type")]
        [Range(1, 2147483647, ErrorMessage = "Please select Soil Type.")]
        public Nullable<int> MAST_SOIL_TYPE_CODE { get; set; }

        [Display(Name = "Terrain Type")]
        [Range(1, 2147483647, ErrorMessage = "Please select Terrain Type.")]
        public Nullable<int> MAST_TERRAIN_TYPE_CODE { get; set; }

        [Required(ErrorMessage = "Please select Core Network(y/n) (Try after some time..)")]
        [RegularExpression(@"^([YN]+)$", ErrorMessage = "Please select whether included in Core Network or not.")]
        [Display(Name = "Is Included in Core Network ?")]
        public string MAST_CORE_NETWORK { get; set; }

        [Display(Name = "  Select If No Benefited Habitations")]
        public string MAST_IS_BENEFITTED_HAB { get; set; }

        [Display(Name = "Reason")]
        [CompareReason("MAST_IS_BENEFITTED_HAB", ErrorMessage = "Please select reason.")]
        public Nullable<int> MAST_NOHABS_REASON { get; set; }

        [Display(Name = "Road Owner")]
        [Range(1, 2147483647, ErrorMessage = "Please select Road Owner.")]
        public int MAST_ER_ROAD_OWNER { get; set; }

        [Display(Name = "Year of Construction")]
        [Range(1950, 2050, ErrorMessage = "Please select proper construction year.")]
        public Nullable<int> MAST_CONS_YEAR { get; set; }

        [Display(Name = "Year of Last Periodic Renewal")]
        [Range(1950, 2050, ErrorMessage = "Please select proper renewal year.")]
        //[CompareYear("MAST_CONS_YEAR", "MAST_ROAD_CAT_CODE", ErrorMessage = "Renewal Year must be greater than Construction Year.")]
        public Nullable<int> MAST_RENEW_YEAR { get; set; }

        [Display(Name = "CD Works Number")]
        public Nullable<int> MAST_CD_WORKS_NUM { get; set; }

        [Display(Name = "Lock Status")]
        public string MAST_LOCK_STATUS { get; set; }

        public string LockUnlockFlag { get; set; }

        public SelectList RoadCategory
        {
            get;
            set;
        }


        public SelectList GovOwners
        {
            get
            {

                List<MASTER_AGENCY> govOwnerList = new List<MASTER_AGENCY>();

                IExistingRoadsDAL objDAL = new ExistingRoadsDAL();

                govOwnerList = objDAL.GetAllGovOwner();
                //new code added by Vikram - in edit mode provide Others in dropdown of Road Owner.
                if (this.EncryptedRoadCode != null)
                {
                    govOwnerList.Add(new MASTER_AGENCY() { MAST_AGENCY_CODE = 29, MAST_AGENCY_NAME = "OTHERS" });
                }
                //end of change
                govOwnerList.Insert(0, new MASTER_AGENCY() { MAST_AGENCY_CODE = 0, MAST_AGENCY_NAME = "-- Select Road Owner --" });

                return new SelectList(govOwnerList, "MAST_AGENCY_CODE", "MAST_AGENCY_NAME");
            }
        }


        public SelectList SoilTypes
        {
            get
            {
                List<MASTER_SOIL_TYPE> soilTypeList = new List<MASTER_SOIL_TYPE>();

                IExistingRoadsDAL objDAL = new ExistingRoadsDAL();

                soilTypeList = objDAL.GetAllSoilTypes();

                soilTypeList.Insert(0, new MASTER_SOIL_TYPE() { MAST_SOIL_TYPE_CODE = 0, MAST_SOIL_TYPE_NAME = "-- Select Soil Type --" });

                return new SelectList(soilTypeList, "MAST_SOIL_TYPE_CODE", "MAST_SOIL_TYPE_NAME");
            }
        }

        public SelectList TerrainTypes
        {
            get
            {
                List<MASTER_TERRAIN_TYPE> TerrainTypeList = new List<MASTER_TERRAIN_TYPE>();

                IExistingRoadsDAL objDAL = new ExistingRoadsDAL();

                TerrainTypeList = objDAL.GetAllTerrainTypes();
                TerrainTypeList.Insert(0, new MASTER_TERRAIN_TYPE() { MAST_TERRAIN_TYPE_CODE = 0, MAST_TERRAIN_TYPE_NAME = "-- Select Terrain Type --" });
                return new SelectList(TerrainTypeList, "MAST_TERRAIN_TYPE_CODE", "MAST_TERRAIN_TYPE_NAME");
            }
        }

        public SelectList RoadConstuctionYears
        {
            get
            {
                List<SelectListItem> yearList = new List<SelectListItem>();

                IExistingRoadsDAL objDAL = new ExistingRoadsDAL();

                yearList = objDAL.GetConstructionYears();
                if (PMGSY.Extensions.PMGSYSession.Current.PMGSYScheme == 2)
                {
                    yearList.Find(x => x.Value == "0").Value = "";
                }
                return new SelectList(yearList, "Value", "Text");
            }
        }

        public SelectList RoadPeriodicRenewalYears
        {
            get
            {
                List<SelectListItem> yearList = new List<SelectListItem>();

                IExistingRoadsDAL objDAL = new ExistingRoadsDAL();

                yearList = objDAL.GetPeriodicRenewalYears();
                if (PMGSY.Extensions.PMGSYSession.Current.PMGSYScheme == 2)
                {
                    yearList.Find(x => x.Value == "0").Value = "";
                }
                return new SelectList(yearList, "Value", "Text");
            }
        }

        public SelectList Reason
        {
            get
            {

                List<MASTER_REASON> reasonList = new List<MASTER_REASON>();

                IExistingRoadsDAL objDAL = new ExistingRoadsDAL();

                reasonList = objDAL.GetAllReasons();

                return new SelectList(reasonList, "MAST_REASON_CODE", "MAST_REASON_NAME");
            }
        }

        public virtual ICollection<MANE_ER_PCI_INDEX> MANE_ER_PCI_INDEX { get; set; }
        public virtual MASTER_AGENCY MASTER_AGENCY { get; set; }
        public virtual MASTER_BLOCK MASTER_BLOCK { get; set; }
        public virtual MASTER_DISTRICT MASTER_DISTRICT { get; set; }
        public virtual ICollection<MASTER_ER_CBR_VALUE> MASTER_ER_CBR_VALUE { get; set; }
        public virtual ICollection<MASTER_ER_CDWORKS_ROAD> MASTER_ER_CDWORKS_ROAD { get; set; }
        public virtual ICollection<MASTER_ER_HABITATION_ROAD> MASTER_ER_HABITATION_ROAD { get; set; }
        public virtual ICollection<MASTER_ER_SURFACE_TYPES> MASTER_ER_SURFACE_TYPES { get; set; }
        public virtual ICollection<MASTER_ER_TRAFFIC_INTENSITY> MASTER_ER_TRAFFIC_INTENSITY { get; set; }
        public virtual MASTER_REASON MASTER_REASON { get; set; }
        public virtual MASTER_ROAD_CATEGORY MASTER_ROAD_CATEGORY { get; set; }
        public virtual MASTER_SOIL_TYPE MASTER_SOIL_TYPE { get; set; }
        public virtual MASTER_STATE MASTER_STATE { get; set; }
        public virtual MASTER_TERRAIN_TYPE MASTER_TERRAIN_TYPE { get; set; }
        public virtual ICollection<PLAN_ROAD> PLAN_ROAD { get; set; }
    }

    public class CompareValidationAttribute : ValidationAttribute, IClientValidatable
    {

        // private const string _defaultErrorMessage = "Road Renewal Year must be greater than Road Costruction Year";  
        private string _basePropertyName;

        public CompareValidationAttribute(string basePropertyName) //: base(_defaultErrorMessage)
        {
            _basePropertyName = basePropertyName;
        }


        //Override IsValid  
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            //Get PropertyInfo Object  
            var basePropertyInfo = validationContext.ObjectType.GetProperty(_basePropertyName);

            var constructionYear = basePropertyInfo.GetValue(validationContext.ObjectInstance, null);
            var renewalYear = value;

            if (constructionYear != null && renewalYear != null)
            {
                int RoadConstructionYear = Convert.ToInt32(constructionYear);
                var RoadRenewalYear = Convert.ToInt32(renewalYear);

                //Actual comparision  
                if (RoadConstructionYear > RoadRenewalYear)
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
                ValidationType = "comparevalidation"
            };
            yield return rule;
        }

    }

    public class CompareYear : ValidationAttribute, IClientValidatable
    {
        private readonly string PropertyYearConstruction;
        private readonly string PropertyRoadCategory;

        public CompareYear(string propertyYearConstruction, string propertyRoadCategory)
        {
            this.PropertyYearConstruction = propertyYearConstruction;
            this.PropertyRoadCategory = propertyRoadCategory;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var propertyTestedConsYear = validationContext.ObjectType.GetProperty(this.PropertyYearConstruction);

            var propertyTestedRoadCategory = validationContext.ObjectType.GetProperty(this.PropertyRoadCategory);

            var roadCode = Convert.ToInt32(propertyTestedRoadCategory.GetValue(validationContext.ObjectInstance, null));

            if (roadCode == 6)
            {
                return ValidationResult.Success;
            }

            if (propertyTestedConsYear == null)
            {
                return new ValidationResult(string.Format("unknown property {0}", this.PropertyYearConstruction));
            }

            var constructionYear = Convert.ToInt32(propertyTestedConsYear.GetValue(validationContext.ObjectInstance, null));
            if (value == null)
            {
                return new ValidationResult(FormatErrorMessage(validationContext.DisplayName));
            }

            var renewalYear = Convert.ToInt32(value);

            if (constructionYear <= renewalYear)
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
                ValidationType = "compareyear"
            };
            yield return rule;
        }
    }

    public class CompareReason : ValidationAttribute, IClientValidatable
    {
        private readonly string PropertyHabitation;

        public CompareReason(string propertyHabitation)
        {
            this.PropertyHabitation = propertyHabitation;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var propertyTestedHabitation = validationContext.ObjectType.GetProperty(this.PropertyHabitation);

            if (propertyTestedHabitation == null)
            {
                return new ValidationResult(string.Format("unknown property {0}", this.PropertyHabitation));
            }

            var isHabitation = propertyTestedHabitation.GetValue(validationContext.ObjectInstance, null);

            if (isHabitation.ToString().ToLower() == "n")
            {
                if (Convert.ToInt32(value) == 0)
                {
                    return new ValidationResult(FormatErrorMessage(validationContext.DisplayName));
                }
            }

            return ValidationResult.Success;
        }

        public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
        {
            var rule = new ModelClientValidationRule
            {
                ErrorMessage = this.ErrorMessageString,
                ValidationType = "comparereason"
            };
            yield return rule;
        }
    }

    public class CheckRoadType : ValidationAttribute, IClientValidatable
    {
        private readonly string PropertyYear1;
        private readonly string PropertyYear2;

        public CheckRoadType(string propertyYear1, string propertyYear2)
        {
            this.PropertyYear1 = propertyYear1;
            this.PropertyYear2 = propertyYear2;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var propertyTestedYear1 = validationContext.ObjectType.GetProperty(this.PropertyYear1);
            var propertyTestedYear2 = validationContext.ObjectType.GetProperty(this.PropertyYear2);

            var year1 = Convert.ToInt32(propertyTestedYear1.GetValue(validationContext.ObjectInstance, null));
            var year2 = Convert.ToInt32(propertyTestedYear2.GetValue(validationContext.ObjectInstance, null));

            if (Convert.ToInt32(value) == 6)
            {
                if (year1 == 0)
                {
                    return ValidationResult.Success;
                }

                if (year2 == 0)
                {
                    return ValidationResult.Success;
                }
            }
            return ValidationResult.Success;
        }

        public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
        {
            var rule = new ModelClientValidationRule
            {
                ErrorMessage = this.ErrorMessageString,
                ValidationType = "checkroadtype"
            };
            yield return rule;
        }
    }


}