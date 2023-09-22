using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using System.Text;


namespace PMGSY.Models.MasterDataEntry
{
    public class HabitationDetails
    {
        [UIHint("Hidden")]
        //[Required(ErrorMessage = "Habitation Code is required.")]
        public string EncryptedHabitationCode_OtherDetails { get; set; }

        [UIHint("Hidden")]
        //[Required(ErrorMessage = "Habitation Code is required.")]
        public string EncryptedHabitationDetailsCode { get; set; }

        //private Int16 _YearID { get; set; }
        [Display(Name = "Census Year")]
        //[Range(1, short.MaxValue, ErrorMessage = "Please select Census Year.")] 
        public Int16 YearID { get; set; }


        [Display(Name = "Total Population")]
        [Range(0, 2147483647, ErrorMessage = "Total Population must be valid number.")] 
        [Required(ErrorMessage = "Total Population is required.")]
        [RegularExpression(@"^([0-9]+)$", ErrorMessage = "Total Population must be valid number.")]
        [CompareFieldHabitationPopValidator("MAST_HAB_SCST_POP", ErrorMessage = "Total Population must be greater or equal to SC/ST Population.")]
        public Int32? MAST_HAB_TOT_POP { get; set; }

         [Display(Name = "SC/ST Population")]
         [Range(0, 2147483647, ErrorMessage = "SC/ST Population must be valid number.")]
         [Required(ErrorMessage = "SC/ST Population is required.")]
         [RegularExpression(@"^([0-9]+)$", ErrorMessage = "SC/ST Population must be valid number.")]
        public Int32? MAST_HAB_SCST_POP { get; set; }

        [Display(Name = "Is Habitation Connected")]
        public string MAST_HAB_CONNECTED { get; set; }

        [Display(Name = "Is Scheme")]
        public string MAST_SCHEME { get; set; }
        
        [Display(Name = "Primary School")]
        public string MAST_PRIMARY_SCHOOL { get; set; }

        [Display(Name = "Middle School")]
        public string MAST_MIDDLE_SCHOOL { get; set; }

        [Display(Name = "High School")]
        public string MAST_HIGH_SCHOOL { get; set; }

        [Display(Name = "Intermediate School")]
        public string MAST_INTERMEDIATE_SCHOOL { get; set; }

        [Display(Name = "ITI")]
        public string MAST_ITI { get; set; }

        [Display(Name = "Degree College")]
        public string MAST_DEGREE_COLLEGE { get; set; }

        [Display(Name = "Health Service")]
        public string MAST_HEALTH_SERVICE { get; set; }

        [Display(Name = "Dispensary")]
        public string MAST_DISPENSARY { get; set; }
        [Display(Name = "MCW Centres")]
        public string MAST_MCW_CENTERS { get; set; }

        [Display(Name = "PHCS")]
        public string MAST_PHCS { get; set; }
        [Display(Name = "Veterinary")]
        public string MAST_VETNARY_HOSPITAL { get; set; }

        [Display(Name = "Telegraph Office")]
        public string MAST_TELEGRAPH_OFFICE { get; set; }
        [Display(Name = "Telephone Connection")]
        public string MAST_TELEPHONE_CONNECTION { get; set; }

        [Display(Name = "Mining")]
        public string MAST_Mining { get; set; }

        [IsBooleanValidator(ErrorMessage = "Please select valid option for Mining")]
        public bool HasMining { get; set; }

        [Display(Name = "Bus Service")]
        public string MAST_BUS_SERVICE { get; set; }
        [Display(Name = "Railway Station")]
        public string MAST_RAILWAY_STATION { get; set; }
        [Display(Name = "Electrification")]
        public string MAST_ELECTRICTY { get; set; }
        [Display(Name = "Is Panchyat Head Quater")]
        public string MAST_PANCHAYAT_HQ { get; set; }
        [Display(Name = "Is Tourist Place")]
        public string MAST_TOURIST_PLACE { get; set; }

        [Display(Name = "One Diesel/Petrol Authorized Outlet")]
        public string MAST_PETROL_PUMP { get; set; }

        [Display(Name = "Additional Authorized Diesel Outlet")]
        public string MAST_PUMP_ADD { get; set; }

        [Display(Name="Electric Sub Station above 11 KVA")]
        public string MAST_ELECTRICITY_ADD { get; set; }

        [Display(Name="Mandi (Based on Turnover)")]
        public string MAST_MANDI { get; set; }

        [Display(Name="Warehouse/Cold Storage")]
        public string MAST_WAREHOUSE { get; set; }

        [Display(Name="Retail Shops Selling")]
        public string MAST_RETAIL_SHOP { get; set; }

        [Display(Name="Sub Tehsil")]
        public string MAST_SUB_TEHSIL { get; set; }

        [Display(Name = "Tehsil/Block Headquarter")]
        public string MAST_BLOCK_HQ { get; set; }

        [IsBooleanValidator(ErrorMessage = "Please select valid option")]
        public bool HasHabConnected { get; set; }
        [IsBooleanValidator(ErrorMessage = "Please select valid option")]
        public bool ISScheme { get; set; }
        [IsBooleanValidator(ErrorMessage = "Please select valid option")]
        public bool HasPrimarySchool { get; set; }
        [IsBooleanValidator(ErrorMessage = "Please select valid option")]
        public bool HasMiddleSchool { get; set; }
        [IsBooleanValidator(ErrorMessage = "Please select valid option")]
        public bool HasHighSchool { get; set; }
        [IsBooleanValidator(ErrorMessage = "Please select valid option")]
        public bool HasIntermediateSchool { get; set; }
        [IsBooleanValidator(ErrorMessage = "Please select valid option")]
        public bool HasITI { get; set; }
        [IsBooleanValidator(ErrorMessage = "Please select valid option")]
        public bool HasDegreeCollege { get; set; }
        [IsBooleanValidator(ErrorMessage = "Please select valid option")]
        public bool HasHealthService { get; set; }
        [IsBooleanValidator(ErrorMessage = "Please select valid option")]
        public bool HasDespensary { get; set; }
        [IsBooleanValidator(ErrorMessage = "Please select valid option")]
        public bool HasMCWCenters { get; set; }
        [IsBooleanValidator(ErrorMessage = "Please select valid option")]
        public bool HasPHCS { get; set; }
        [IsBooleanValidator(ErrorMessage = "Please select valid option")]
        public bool HasVetnaryHospital { get; set; }
        [IsBooleanValidator(ErrorMessage = "Please select valid option")]
        public bool HasTelegraphOffice { get; set; }
        [IsBooleanValidator(ErrorMessage = "Please select valid option")]
        public bool HasTelephoneConnection { get; set; }
        [IsBooleanValidator(ErrorMessage = "Please select valid option")]
        public bool HasBusService { get; set; }
        [IsBooleanValidator(ErrorMessage = "Please select valid option")]
        public bool HasRailwayStation { get; set; }
        [IsBooleanValidator(ErrorMessage = "Please select valid option")]
        public bool HasElectricity { get; set; }
        [IsBooleanValidator(ErrorMessage = "Please select valid option")]
        public bool ISPanchayatHQ { get; set; }
        [IsBooleanValidator(ErrorMessage = "Please select valid option")]
        public bool IsTouristPlace { get; set; }
        [IsBooleanValidator(ErrorMessage = "Please select valid option")]

        public bool HasPetrolPump { get; set; }
        [IsBooleanValidator(ErrorMessage = "Please select valid option")]
        public bool HasAdditionalPetrolPump { get; set; }
        [IsBooleanValidator(ErrorMessage = "Please select valid option")]
        public bool HasAdditionalElectricity { get; set; }
        [IsBooleanValidator(ErrorMessage = "Please select valid option")]
        public bool HasMandi { get; set; }
        [IsBooleanValidator(ErrorMessage = "Please select valid option")]
        public bool HasWarehouse { get; set; }
        [IsBooleanValidator(ErrorMessage = "Please select valid option")]
        public bool HasRetailShop { get; set; }
        [IsBooleanValidator(ErrorMessage = "Please select valid option")]
        public bool HasSubTehsil { get; set; }
        [IsBooleanValidator(ErrorMessage = "Please select valid option")]
        public bool HasBlockHeadquarter { get; set; }

        //Added By Abhishek kamble 24-Feb-2014
        public int totalRemainingPopulation { get; set; }
        public int totalRemainingSCSTPopulation { get; set; }
        public bool IsVillagePopulationDetailsExist { get; set; }
        public string ErrMessageForIsVillagePopulationExist { get; set; }

        public int totalVillagePopulation { get; set; }
        public Int64 totalVillagePopulation20Per { get; set; }
        public int totalVillageSCSTPopulation { get; set; }
        

        /// <summary>
        /// Census year according to selected PMGSY Scheme
        /// </summary>
        public string Years
        {
            get
            {
                List<PMGSY.Models.MASTER_CENSUS_YEAR> yearList = new List<PMGSY.Models.MASTER_CENSUS_YEAR>();

                /*foreach (var item in Year.lstYear)
                {
                    yearList.Add(new Year() { YearID = (Int16)item.Key, YearDesc = item.Value });
                }*/


                PMGSY.DAL.MasterDataEntryDAL masterDataEntryDAL = new PMGSY.DAL.MasterDataEntryDAL();

                yearList = masterDataEntryDAL.GetCensusYears(false);
                
                string censusYear = yearList.Select(s => s.MAST_CENSUS_YEAR.ToString()).FirstOrDefault();
                
                //yearList.Insert(0, new PMGSY.Models.MASTER_CENSUS_YEAR() { MAST_CENSUS_YEAR = 0, MAST_YEAR = "--Select--" });
               
                
                //return new SelectList(yearList, "MAST_CENSUS_YEAR", "MAST_YEAR");
                return censusYear;
            }
        }

       
              

        //public HabitationDetails()
        //{
        //    HasHabConnected = false;
        //    ISScheme = true;
        //    HasPrimarySchool = false;
        //    HasMiddleSchool = false;
        //    HasHighSchool = false;
        //    HasIntermediateSchool = false;
        //    HasDegreeCollege = false;
        //    HasHealthService = false;
        //    HasDespensary = false;
        //    HasMCWCenters = false;
        //    HasPHCS = false;
        //    HasVetnaryHospital = false;
        //    HasTelegraphOffice = false;
        //    HasTelephoneConnection = false;
        //    HasBusService = false;
        //    HasRailwayStation = false;
        //    HasElectricity = false;
        //    ISPanchayatHQ = false;
        //    IsTouristPlace = false;

        //    HasPetrolPump = false;
        //    HasAdditionalPetrolPump = false;
        //    HasAdditionalElectricity = false;
        //    HasMandi = false;
        //    HasWarehouse = false;
        //    HasRetailShop = false;
        //    HasSubTehsil = false;
        //    HasBlockHeadquarter = false;
            
           
        //}



    }

    public class Year
    {
       
        public Int16 YearID { get; set; }
        public string YearDesc { get; set; }
        public static readonly Dictionary<Int16,string> lstYear = new Dictionary<Int16,string>() 
                                                                  { 
                                                                    {2000,"--Select--"}, 
                                                                    {2001,"2001"},
                                                                    {2011,"2011"} 
                                                                  };
    }

    //class for custom validation
    public class CompareFieldHabitationPopValidator : ValidationAttribute, IClientValidatable
    {
        //public string Value { get; set; }
        public string ComparingValue { get; set; }

        public CompareFieldHabitationPopValidator(string ComparingValue)
        {
            //this.Value = Value;
            this.ComparingValue = ComparingValue;
        }

        public override string FormatErrorMessage(string name)
        {
            return string.Format(ErrorMessageString, name, ComparingValue);
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {

            // System.Reflection.PropertyInfo PropertyInfo = validationContext.ObjectInstance.GetType().GetProperty(Value);
            System.Reflection.PropertyInfo comparingPropertyInfo = validationContext.ObjectInstance.GetType().GetProperty(ComparingValue);

            if (comparingPropertyInfo == null)
            {
                return new ValidationResult(string.Format("Property '{0}' is undefined.", ComparingValue));
            }

            object comparingValue = comparingPropertyInfo.GetValue(validationContext.ObjectInstance, null);

            if (value != null && comparingValue != null)
            {
                //if (Convert.ToInt32(value)  <= Convert.ToInt32(comparingValue))
                if (Convert.ToDouble(value) < Convert.ToDouble(comparingValue))
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
                ValidationType = "comparefieldhabitationpopvalidator"
            };

        }
    }//end custom validation
}