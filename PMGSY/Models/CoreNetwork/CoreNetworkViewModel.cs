using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PMGSY.DAL.Core_Network;
using PMGSY.Extensions;

namespace PMGSY.Models.CoreNetwork
{
    public class CoreNetworkViewModel : IValidatableObject
    {
        public char MAST_SCHEDULE5 { get; set; }

        [UIHint("Hidden")]
        public string EncryptedRoadCode { get; set; }

        public int PLAN_CN_ROAD_CODE { get; set; }

        [Required(ErrorMessage = "Please Select Road Name.")]
        [Range(1, 2147483647, ErrorMessage = "Please select road name.")]
        [Display(Name = "Road Name")]
        public int MAST_ER_ROAD_CODE { get; set; }

        [Required(ErrorMessage = "Please Select Road Number.")]
        [Display(Name = "  Road Number")]
        [StringLength(10, ErrorMessage = "  Road number must be less than 10 characters.")]
        [RegularExpression(@"^([MRLT0-9]+)$", ErrorMessage = "Select correct road number.")]
        public string PLAN_CN_ROAD_NUMBER { get; set; }

        [Display(Name = "State")]
        public int MAST_STATE_CODE { get; set; }

        [Display(Name = "District")]
        public int MAST_DISTRICT_CODE { get; set; }

        [Display(Name = " Previous Block Name")]
        [Range(1, Int32.MaxValue, ErrorMessage = "Please select block")]
        public int MAST_BLOCK_CODE { get; set; }

        [Display(Name = "Road Name")]
        [RegularExpression(@"^([a-zA-Z0-9 ()-.,]+)$", ErrorMessage = "Enter correct Road Name.")]
        [StringLength(200, ErrorMessage = "Road name must be less than 200 characters.")]
        public string PLAN_RD_NAME { get; set; }

        [Required(ErrorMessage = "  Please Select Route Type.")]
        [Display(Name = "Route Type")]
        //[RegularExpression(@"^([(L)|(T)|(M)|(ML)]+)$", ErrorMessage = "Please select route type.")]
        //[RegularExpression("^([L|M|T|ML]+)$", ErrorMessage = "Please select valid route type")]
        [RegularExpression("^([LMTN]+)$", ErrorMessage = "Please select valid route type")]
        public string PLAN_RD_ROUTE { get; set; }

        [Required(ErrorMessage = "  Please Enter Start Chainage.")]
        [Display(Name = "Start Chainage( in Km)")]
        [RegularExpression(@"^\s*(?=.*[0-9])\d*(?:\.\d{1,3})?\s*$", ErrorMessage = "  Enter correct value of start chainage.")]
        [CompareChainage("PLAN_RD_TO_CHAINAGE", ErrorMessage = "Start Chainage must be less than End Chainage")]
        [CompareStartChainage("ExistStartChainage", "PLAN_RD_LENG", ErrorMessage = "Start chainage must be greater than existing start chainage.")]
        public double PLAN_RD_FROM_CHAINAGE { get; set; }

        [Required(ErrorMessage = "  Please Enter End Chainage.")]
        [Display(Name = "End Chainage(in Km)")]
        [RegularExpression(@"^\s*(?=.*[0-9])\d*(?:\.\d{1,3})?\s*$", ErrorMessage = "  Enter correct value of end chainage.")]
        //[Range(0.001, 9999.999, ErrorMessage = "End chainage must be less than existing end chainage")]
        [CompareEndChainage("ExistEndChainage", "PLAN_RD_LENG", ErrorMessage = "End chainage must be less than existing end chainage")]
        [CompareValidation("PLAN_RD_FROM_CHAINAGE", ErrorMessage = "Start Chainage must be less than end chainage.")]
        public double PLAN_RD_TO_CHAINAGE { get; set; }

        [Required(ErrorMessage = "Please select length of road.")]
        [Display(Name = "Length of Road")]
        [RegularExpression(@"^([PF]+)$", ErrorMessage = " Select proper length of road.")]
        public string PLAN_RD_LENG { get; set; }

        [Display(Name = "Road Length(in K.Ms)")]
        [RegularExpression(@"^[0-9]*(?:\.[0-9]*)?$", ErrorMessage = "  Enter correct value of length.")]
        public double PLAN_RD_LENGTH { get; set; }

        [Required(ErrorMessage = "  Please Select Road From.")]
        [RegularExpression(@"^([a-zA-Z1-9]+)$", ErrorMessage = "  Select correct Road From.")]
        [Display(Name = "Road From")]
        public string PLAN_RD_FROM_TYPE { get; set; }

        [Required(ErrorMessage = "  Please Select Road From.")]
        [RegularExpression(@"^([a-zA-Z1-9]+)$", ErrorMessage = "  Select correct Road To.")]
        [Display(Name = "Road To")]
        public string PLAN_RD_TO_TYPE { get; set; }

        [Display(Name = "Habitation")]
        [CompareHabitationStatusFrom("PLAN_RD_FROM_TYPE", ErrorMessage = "Please select Habitation From")]
        public Nullable<int> PLAN_RD_FROM_HAB { get; set; }

        [Display(Name = "Habitation")]
        [CompareHabitationStatusTo("PLAN_RD_TO_TYPE", ErrorMessage = "Please select Habitation To.")]
        public Nullable<int> PLAN_RD_TO_HAB { get; set; }

        [Range(1, Int32.MaxValue, ErrorMessage = "  Please Select Block.")]
        [Display(Name = "Block")]
        [RegularExpression(@"^([0-9]+)$", ErrorMessage = "Select correct start block.")]
        public Nullable<int> PLAN_RD_BLOCK_FROM_CODE { get; set; }

        [Range(1, Int32.MaxValue, ErrorMessage = "  Please Select Block.")]
        [Display(Name = "Block")]
        [RegularExpression(@"^([0-9]+)$", ErrorMessage = "Select Correct End Block.")]
        public Nullable<int> PLAN_RD_BLOCK_TO_CODE { get; set; }

        [Display(Name = "Start At:")]
        [RegularExpression(@"^([a-zA-Z0-9 ()]+)$", ErrorMessage = "Select correct start at.")]
        //[Range(1, Int32.MaxValue, ErrorMessage = "Please select Road.")]
        [CompareRoadStatusFrom("PLAN_RD_FROM_TYPE", ErrorMessage = "Please select Road From")]
        public Nullable<int> PLAN_RD_NUM_FROM { get; set; }

        [Display(Name = "End At:")]
        [RegularExpression(@"^([a-zA-Z0-9 ()]+)$", ErrorMessage = "Select correct end at.")]
        //[Range(1, Int32.MaxValue, ErrorMessage = "Please select Road.")]
        [CompareRoadStatusTo("PLAN_RD_TO_TYPE", ErrorMessage = "Please select Road To")]
        public Nullable<int> PLAN_RD_NUM_TO { get; set; }

        public string PLAN_LOCK_STATUS { get; set; }

        [Display(Name = "Road From")]
        [RegularExpression(@"^([a-zA-Z0-9 ()]+)$", ErrorMessage = "Select correct start.")]
        [StringLength(200, ErrorMessage = "Road name must be less than 200 characters.")]
        public string PLAN_RD_FROM { get; set; }

        [Display(Name = "Road To")]
        [RegularExpression(@"^([a-zA-Z0-9 ()]+)$", ErrorMessage = "Select correct end.")]
        [StringLength(200, ErrorMessage = "Road name must be less than 200 characters.")]
        public string PLAN_RD_TO { get; set; }

        //new field related to Candidate Road
        [Display(Name = "Total Length of Candidate Road")]
        //[IsRequired(ErrorMessage = "Total Length of Candidate Road is required.")]
        ///[RegularExpression(@"^\s*(?=.*[0-9])\d*(?:\.\d{1,3})?\s*$", ErrorMessage = "  Enter correct value of Total Length.")]
        [RegularExpression(@"^\s*(?=.*[0-9])\d*(?:\.\d{1,3})?\s*$", ErrorMessage = "Decimal values upto 3 decimals are allowed for Total Length")]
        public Decimal? TotalLengthOfCandidate { get; set; }

        public decimal TotalLength { get; set; }
        public Nullable<int> NUM_FROM { get; set; }
        public Nullable<int> NUM_TO { get; set; }
        public string FROM_TYPE { get; set; }
        public string TO_TYPE { get; set; }
        public string RD_FROM { get; set; }
        public string RD_TO { get; set; }
        public string ROAD_CODE { get; set; }
        public string BLOCK_NAME { get; set; }
        public double ExistStartChainage { get; set; }
        public double ExistEndChainage { get; set; }
        public int RoadCatCode { get; set; }
        public string RoadShortCode { get; set; }
        public string LockUnlockFlag { get; set; }

        public List<SelectListItem> RoadCodeList { get; set; }

        public List<SelectListItem> RouteType { get; set; }

        public SelectList RoadFrom
        {
            get
            {
                List<SelectListItem> lstRoadFrom = new List<SelectListItem>();
                lstRoadFrom.Add(new SelectListItem { Value = "0", Text = "--Select Road--", Selected = true });
                lstRoadFrom.Add(new SelectListItem { Value = "H", Text = "Habitation" });
                if (PMGSYSession.Current.PMGSYScheme == 1)
                {
                    lstRoadFrom.Add(new SelectListItem { Value = "L", Text = "Link Route" });
                }
                else if (PMGSYSession.Current.PMGSYScheme == 2)
                {
                    lstRoadFrom.Add(new SelectListItem { Value = "M", Text = "Major Rural Link" });
                }

                else if (PMGSYSession.Current.PMGSYScheme == 4)
                {

                    //lstRoadFrom.Add(new SelectListItem { Value = "T", Text = "Through Route" });
                    lstRoadFrom.Add(new SelectListItem { Value = "D", Text = "Major District Road" });
                    lstRoadFrom.Add(new SelectListItem { Value = "N", Text = "National Highway" });
                    lstRoadFrom.Add(new SelectListItem { Value = "O", Text = "Others" });
                    lstRoadFrom.Add(new SelectListItem { Value = "R", Text = "Rural Road(Other District Roads)" });
                    lstRoadFrom.Add(new SelectListItem { Value = "Z", Text = "Rural Road(Track)" });
                    lstRoadFrom.Add(new SelectListItem { Value = "V", Text = "Rural Road(Village Roads)" });
                    lstRoadFrom.Add(new SelectListItem { Value = "S", Text = "State Highway" });
                }
                else
                {
                    lstRoadFrom.Add(new SelectListItem { Value = "T", Text = "Through Route" });
                    lstRoadFrom.Add(new SelectListItem { Value = "D", Text = "Major District Road" });
                    lstRoadFrom.Add(new SelectListItem { Value = "N", Text = "National Highway" });
                    lstRoadFrom.Add(new SelectListItem { Value = "R", Text = "Rural Road(Other District Roads)" });
                    lstRoadFrom.Add(new SelectListItem { Value = "Z", Text = "Rural Road(Track)" });
                    lstRoadFrom.Add(new SelectListItem { Value = "V", Text = "Rural Road(Village Roads)" });
                    lstRoadFrom.Add(new SelectListItem { Value = "S", Text = "State Highway" });
                    lstRoadFrom.Add(new SelectListItem { Value = "O", Text = "Others" });
                }
                return new SelectList(lstRoadFrom, "Value", "Text");
            }
        }

        public SelectList RoadTo
        {
            get
            {

                List<SelectListItem> lstRoadTo = new List<SelectListItem>();
                if (PMGSYSession.Current.PMGSYScheme == 4)
                {
                    lstRoadTo.Add(new SelectListItem { Value = "0", Text = "--Select Road--", Selected = true });
                    //lstRoadTo.Add(new SelectListItem { Value="B",Text="Block Boundry"});
                    lstRoadTo.Add(new SelectListItem { Value = "H", Text = "Habitation" });
                    lstRoadTo.Add(new SelectListItem { Value = "D", Text = "Major District Road" });
                    lstRoadTo.Add(new SelectListItem { Value = "N", Text = "National Highway" });
                    lstRoadTo.Add(new SelectListItem { Value = "O", Text = "Others" });
                    lstRoadTo.Add(new SelectListItem { Value = "R", Text = "Rural Road(Other District Roads)" });
                    lstRoadTo.Add(new SelectListItem { Value = "Z", Text = "Rural Road(Track)" });
                    lstRoadTo.Add(new SelectListItem { Value = "V", Text = "Rural Road(Village Roads)" });
                    lstRoadTo.Add(new SelectListItem { Value = "S", Text = "State Highway" });
                }
                else
                {
                    lstRoadTo.Add(new SelectListItem { Value = "0", Text = "--Select Road--", Selected = true });
                    //lstRoadTo.Add(new SelectListItem { Value="B",Text="Block Boundry"});
                    lstRoadTo.Add(new SelectListItem { Value = "H", Text = "Habitation" });
                    lstRoadTo.Add(new SelectListItem { Value = "R", Text = "Rural Road(Other District Roads)" });
                    lstRoadTo.Add(new SelectListItem { Value = "Z", Text = "Rural Road(Track)" });
                    lstRoadTo.Add(new SelectListItem { Value = "V", Text = "Rural Road(Village Roads)" });
                }
                return new SelectList(lstRoadTo, "Value", "Text");

            }
        }

        public SelectList RoadNumFromList
        {
            get
            {
                List<SelectListItem> lstRoadNumFrom = new List<SelectListItem>();
                lstRoadNumFrom.Add(new SelectListItem { Value = "0", Text = "--Select Road--", Selected = true });
                return new SelectList(lstRoadNumFrom, "Value", "Text");
            }
        }

        public SelectList RoadNumToList
        {
            get
            {
                List<SelectListItem> lstRoadNumTo = new List<SelectListItem>();
                lstRoadNumTo.Add(new SelectListItem { Value = "0", Text = "--Select Road--", Selected = true });
                return new SelectList(lstRoadNumTo, "Value", "Text");
            }
        }

        public SelectList RoadCategory
        {
            get
            {
                List<MASTER_ROAD_CATEGORY> lsRoadCategory = new List<MASTER_ROAD_CATEGORY>();
                CoreNetworkDAL objDAL = new CoreNetworkDAL();
                lsRoadCategory = objDAL.GetAllRoadCategories();
                return new SelectList(lsRoadCategory, "MAST_ROAD_CAT_CODE", "MAST_ROAD_CAT_NAME");
            }
        }

        public SelectList RoadCode
        {
            get
            {
                //if (this.EncryptedRoadCode != null)
                //{
                //    //CoreNetworkDAL objDAL = new CoreNetworkDAL();
                //    //List<MASTER_EXISTING_ROADS> lstRoads = objDAL.GetRoadNamesByRoadCode(this.RoadCatCode, this.MAST_BLOCK_CODE);//new
                //    //lstRoads.Insert(0, new MASTER_EXISTING_ROADS { MAST_ER_ROAD_CODE = 0, MAST_ER_ROAD_NAME = "-Select Road Name-" });
                //    //return new SelectList(lstRoads, "Value", "Text");
                //}
                //else
                //{
                //    List<SelectListItem> lsRoadCode = new List<SelectListItem>();
                //    lsRoadCode.Add(new SelectListItem { Value = "0", Text = "--Select Road--", Selected = true });
                //    return new SelectList(lsRoadCode, "Value", "Text");
                //}
                List<SelectListItem> lsRoadCode = new List<SelectListItem>();
                lsRoadCode.Add(new SelectListItem { Value = "0", Text = "--Select Road--", Selected = true });
                return new SelectList(lsRoadCode, "Value", "Text");
            }
        }

        //public SelectList RouteType
        //{
        //    get
        //    {
        //        List<SelectListItem> lstRoute = new List<SelectListItem>();
        //        lstRoute.Add(new SelectListItem { Value="0",Text="--Select Route--",Selected=true});
        //        lstRoute.Add(new SelectListItem { Value = "T", Text = "Through Route" });
        //        if (PMGSYSession.Current.PMGSYScheme == 1)
        //        {
        //            lstRoute.Add(new SelectListItem { Value = "L", Text = "Link Route" });
        //        }
        //        else if (PMGSYSession.Current.PMGSYScheme == 2)
        //        {
        //            lstRoute.Add(new SelectListItem { Value = "M", Text = "Major Rural Link" });
        //        }
        //        return new SelectList(lstRoute,"Value","Text");
        //    }
        //}

        public SelectList Road
        {
            get
            {
                List<SelectListItem> lstRoadFrom = new List<SelectListItem>();
                lstRoadFrom.Add(new SelectListItem { Value = "0", Text = "--Select Road--", Selected = true });
                //lstRoadFrom.Add(new SelectListItem { Value = "B", Text = "Block Boundry" });
                lstRoadFrom.Add(new SelectListItem { Value = "H", Text = "Habitation" });
                if (PMGSYSession.Current.PMGSYScheme == 1)
                {
                    lstRoadFrom.Add(new SelectListItem { Value = "L", Text = "Link Route" });
                }
                else if (PMGSYSession.Current.PMGSYScheme == 2)
                {
                    lstRoadFrom.Add(new SelectListItem { Value = "M", Text = "Major Rural Link" });
                }
                lstRoadFrom.Add(new SelectListItem { Value = "T", Text = "Through Route" });
                return new SelectList(lstRoadFrom, "Value", "Text");
            }
        }


        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (PMGSYSession.Current.PMGSYScheme == 2)
            {
                if (string.IsNullOrEmpty(Convert.ToString(TotalLengthOfCandidate)))
                {
                    yield return new ValidationResult("Total Length of candidate road is required.");
                }

                if (TotalLengthOfCandidate < (Convert.ToDecimal(PLAN_RD_TO_CHAINAGE) - Convert.ToDecimal(PLAN_RD_FROM_CHAINAGE)))
                {
                    yield return new ValidationResult("Total Length of candidate road should not be less than Road Length.");
                }

                if (TotalLengthOfCandidate > Convert.ToDecimal(9999.999) || TotalLengthOfCandidate < 0)
                {
                    yield return new ValidationResult("Candidate Road Length is invalid.");
                }
            }





        }
    }

    public class CompareValidationAttribute : ValidationAttribute
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

            var phaseYear = basePropertyInfo.GetValue(validationContext.ObjectInstance, null);
            var releaseYear = value;

            if (phaseYear != null && releaseYear != null)
            {
                var PhaseYear = Convert.ToInt32(phaseYear);
                var ReleaseYear = Convert.ToInt32(releaseYear);

                //Actual comparision  
                if (PhaseYear > ReleaseYear)
                {
                    var message = FormatErrorMessage(validationContext.DisplayName);
                    return new ValidationResult(message);
                }
            }

            //Default return - This means there were no validation error  
            return null;
        }

    }

    public class CompareStartChainage : ValidationAttribute, IClientValidatable
    {
        private readonly string PropertyName;
        private readonly string PropertyLength;

        public CompareStartChainage(string propertyName, string propertyLength)
        {
            this.PropertyName = propertyName;
            this.PropertyLength = propertyLength;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var propertyTestedInfo = validationContext.ObjectType.GetProperty(this.PropertyName);
            if (propertyTestedInfo == null)
            {
                return new ValidationResult(string.Format("unknown property {0}", this.PropertyName));
            }
            var propertyTestedLength = validationContext.ObjectType.GetProperty(this.PropertyLength);

            var startValue = Convert.ToDouble(propertyTestedInfo.GetValue(validationContext.ObjectInstance, null));
            var length = propertyTestedLength.GetValue(validationContext.ObjectInstance, null);

            if (length.ToString().ToLower() == "p")
            {
                if (startValue <= Convert.ToDouble(value))
                {
                    return ValidationResult.Success;
                }
                else
                {
                    return new ValidationResult(FormatErrorMessage(validationContext.DisplayName));
                }
            }
            else
            {
                return ValidationResult.Success;
            }
        }

        public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
        {
            var rule = new ModelClientValidationRule
            {
                ErrorMessage = this.ErrorMessageString,
                ValidationType = "comparestartchainge"
            };
            rule.ValidationParameters["startchainage"] = this.PropertyName;
            rule.ValidationParameters["lengthtype"] = this.PropertyLength;
            yield return rule;
        }
    }


    public class CompareEndChainage : ValidationAttribute, IClientValidatable
    {
        private readonly string PropertyName;
        private readonly string PropertyLength;

        public CompareEndChainage(string propertyName, string propertyLength)
        {
            this.PropertyName = propertyName;
            this.PropertyLength = propertyLength;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var propertyTestedInfo = validationContext.ObjectType.GetProperty(this.PropertyName);
            if (propertyTestedInfo == null)
            {
                return new ValidationResult(string.Format("unknown property {0}", this.PropertyName));
            }
            var propertyTestedLength = validationContext.ObjectType.GetProperty(this.PropertyLength);

            var endValue = Convert.ToDouble(propertyTestedInfo.GetValue(validationContext.ObjectInstance, null));
            var length = propertyTestedLength.GetValue(validationContext.ObjectInstance, null);

            if (length.ToString().ToLower() == "p")
            {
                if (Convert.ToDouble(value) <= endValue)
                {
                    return ValidationResult.Success;
                }
                else
                {
                    return new ValidationResult(FormatErrorMessage(validationContext.DisplayName));
                }
            }
            else
            {
                return ValidationResult.Success;
            }
        }

        public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
        {
            var rule = new ModelClientValidationRule
            {
                ErrorMessage = this.ErrorMessageString,
                ValidationType = "compareendchainge"
            };
            rule.ValidationParameters["endchainge"] = this.PropertyName;
            rule.ValidationParameters["lengthtype"] = this.PropertyLength;
            yield return rule;
        }
    }


    public class CompareChainage : ValidationAttribute, IClientValidatable
    {
        private readonly string PropertyEndChainage;

        public CompareChainage(string propertyEndChainage)
        {
            this.PropertyEndChainage = propertyEndChainage;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var propertyTestedEndChainage = validationContext.ObjectType.GetProperty(this.PropertyEndChainage);

            if (propertyTestedEndChainage == null)
            {
                return new ValidationResult(string.Format("unknown property {0}", this.PropertyEndChainage));
            }

            var endChainage = Convert.ToDecimal(propertyTestedEndChainage.GetValue(validationContext.ObjectInstance, null));
            if (value == null)
            {
                return new ValidationResult(FormatErrorMessage(validationContext.DisplayName));
            }
            var startChainage = Convert.ToDecimal(value);

            if (startChainage < endChainage)
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

    public class CompareHabitationStatusFrom : ValidationAttribute, IClientValidatable
    {
        private readonly string PropertyType;
        //private readonly string PropertyNumFrom;

        public CompareHabitationStatusFrom(string propertyType)//,string propertyNumFrom)
        {
            this.PropertyType = propertyType;
            //this.PropertyNumFrom = propertyNumFrom;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var propertyTestedInfo = validationContext.ObjectType.GetProperty(this.PropertyType);
            if (propertyTestedInfo == null)
            {
                return new ValidationResult(string.Format("unknown property {0}", this.PropertyType));
            }

            var type = propertyTestedInfo.GetValue(validationContext.ObjectInstance, null);
            var habCode = Convert.ToInt32(value);

            if (type.ToString().ToLower() == "h")
            {
                if (habCode == 0)
                {
                    return new ValidationResult(FormatErrorMessage(validationContext.DisplayName));
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
        }
        public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
        {
            var rule = new ModelClientValidationRule
            {
                ErrorMessage = this.ErrorMessageString,
                ValidationType = "comparehabitationstatusfrom"
            };
            yield return rule;
        }
    }

    public class CompareRoadStatusFrom : ValidationAttribute, IClientValidatable
    {
        private readonly string PropertyType;
        //private readonly string PropertyNumFrom;


        public CompareRoadStatusFrom(string propertyType)//, string numTo)
        {
            this.PropertyType = propertyType;
            //this.PropertyNumFrom = numTo;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var propertyTestedInfo = validationContext.ObjectType.GetProperty(this.PropertyType);
            //var propertyTestedNumTo = validationContext.ObjectType.GetProperty(this.PropertyNumFrom);

            if (propertyTestedInfo == null)
            {
                return new ValidationResult(string.Format("unknown property {0}", this.PropertyType));
            }

            var type = propertyTestedInfo.GetValue(validationContext.ObjectInstance, null);
            //int numFrom = Convert.ToInt32(propertyTestedNumTo.GetValue(validationContext.ObjectInstance, null));

            var numFrom = Convert.ToInt32(value);

            if (type.ToString().ToLower() != "h")
            {
                if (numFrom == 0)
                {
                    return new ValidationResult(FormatErrorMessage(validationContext.DisplayName));
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
        }
        public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
        {
            var rule = new ModelClientValidationRule
            {
                ErrorMessage = this.ErrorMessageString,
                ValidationType = "compareroadstatusfrom"
            };
            yield return rule;
        }

    }


    public class CompareHabitationStatusTo : ValidationAttribute, IClientValidatable
    {
        private readonly string PropertyType;
        //private readonly string PropertyNumTo;


        public CompareHabitationStatusTo(string propertyType)//,string numTo)
        {
            this.PropertyType = propertyType;
            //this.PropertyNumTo = numTo;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var propertyTestedInfo = validationContext.ObjectType.GetProperty(this.PropertyType);
            //var propertyTestedNumTo = validationContext.ObjectType.GetProperty(this.PropertyNumTo);

            if (propertyTestedInfo == null)
            {
                return new ValidationResult(string.Format("unknown property {0}", this.PropertyType));
            }

            var type = propertyTestedInfo.GetValue(validationContext.ObjectInstance, null);
            //int numTo = Convert.ToInt32(propertyTestedNumTo.GetValue(validationContext.ObjectInstance, null));

            var habCode = Convert.ToInt32(value);

            if (type.ToString().ToLower() == "h")
            {
                if (habCode == 0)
                {
                    return new ValidationResult(FormatErrorMessage(validationContext.DisplayName));
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
        }
        public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
        {
            var rule = new ModelClientValidationRule
            {
                ErrorMessage = this.ErrorMessageString,
                ValidationType = "comparehabitationstatusto"
            };
            yield return rule;
        }

    }

    public class CompareRoadStatusTo : ValidationAttribute, IClientValidatable
    {
        private readonly string PropertyType;
        //private readonly string PropertyNumTo;


        public CompareRoadStatusTo(string propertyType)//, string numTo)
        {
            this.PropertyType = propertyType;
            //this.PropertyNumTo = numTo;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var propertyTestedInfo = validationContext.ObjectType.GetProperty(this.PropertyType);
            //var propertyTestedNumTo = validationContext.ObjectType.GetProperty(this.PropertyNumTo);

            if (propertyTestedInfo == null)
            {
                return new ValidationResult(string.Format("unknown property {0}", this.PropertyType));
            }

            var type = propertyTestedInfo.GetValue(validationContext.ObjectInstance, null);
            //int numTo = Convert.ToInt32(propertyTestedNumTo.GetValue(validationContext.ObjectInstance, null));

            var numTo = Convert.ToInt32(value);

            if (type.ToString().ToLower() != "h")
            {
                if (numTo == 0)
                {
                    return new ValidationResult(FormatErrorMessage(validationContext.DisplayName));
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
        }
        public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
        {
            var rule = new ModelClientValidationRule
            {
                ErrorMessage = this.ErrorMessageString,
                ValidationType = "compareroadstatusto"
            };
            yield return rule;
        }

    }

    //public class IsRequired : ValidationAttribute, IClientValidatable
    //{
    //    private readonly byte PropertyName;

    //    public IsRequired(byte propertyName)
    //    {
    //        this.PropertyName = propertyName;
    //    }

    //    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    //    {
    //        var propertyTestedInfo = validationContext.ObjectType.GetProperty(this.PropertyName.ToString());
    //        if (propertyTestedInfo == null)
    //        {
    //            return new ValidationResult(string.Format("unknown property {0}", this.PropertyName));
    //        }

    //        var startValue = Convert.ToDouble(propertyTestedInfo.GetValue(validationContext.ObjectInstance, null));

    //        if(value == null)
    //        {
    //            return new ValidationResult(FormatErrorMessage(validationContext.DisplayName));
    //        }

    //        if (startValue.ToString() == "2")
    //        {
    //            if (value == null)
    //            {
    //                return new ValidationResult(FormatErrorMessage(validationContext.DisplayName));
    //            }
    //            else
    //            {
    //                return ValidationResult.Success;
    //            }
    //        }
    //        else
    //        {
    //            return ValidationResult.Success;
    //        }
    //    }

    //    public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
    //    {
    //        var rule = new ModelClientValidationRule
    //        {
    //            ErrorMessage = this.ErrorMessageString,
    //            ValidationType = "isrequired"
    //        };
    //        rule.ValidationParameters["totallengthofcandidate"] = this.PropertyName;
    //        yield return rule;
    //    }
    //}

}