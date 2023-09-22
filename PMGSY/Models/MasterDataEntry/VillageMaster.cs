using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace PMGSY.Models.MasterDataEntry
{
    public class VillageMaster
    {
        [UIHint("Hidden")]
      //  [Required(ErrorMessage = "Village Code is required.")]
        public string EncryptedVillageCode { get; set; }


        public int MAST_VILLAGE_CODE { get; set; }

        [Display(Name = "Village Name")]
        [Required(ErrorMessage = "Village Name is required.")]
        [StringLength(50, ErrorMessage = "Village Name is not greater than 50 characters.")]
       // [RegularExpression(@"^[a-zA-Z][a-zA-Z- _().]+$", ErrorMessage = "Village Name is not in valid format.")]
        [RegularExpression(@"^(?![0-9]*$)[a-zA-Z0-9 ._()]+$", ErrorMessage = "Village Name is not in valid format.")]   
        public string MAST_VILLAGE_NAME { get; set; }


        [Display(Name = "State")]
        [Range(1, 2147483647, ErrorMessage = "Please select state.")]
        public int MAST_STATE_CODE { get; set; }

        [Display(Name = "District")]
        [Range(1, 2147483647, ErrorMessage = "Please select district.")]
        public int MAST_DISTRICT_CODE { get; set; }


        [Display(Name = "Block")]
        [Range(1, 2147483647, ErrorMessage = "Please select block.")]
        public int MAST_BLOCK_CODE { get; set; }


        public string MAST_VILLAGE_ACTIVE { get; set; }
        public Nullable<System.DateTime> MAST_ENTRY_DATE { get; set; }
        public int MAST_NIC_STATE_CODE { get; set; }

        [Display(Name = "Total Population")]
        [Range(0, 2147483647, ErrorMessage = "Total Population must be valid number.")]
        [Required(ErrorMessage = "Total Population is required.")]
        [RegularExpression(@"^([0-9]+)$", ErrorMessage = "Total Population must be valid number.")]
        [CompareFieldVillagePopValidator("MAST_VILLAGE_SCST_POP", ErrorMessage = "Total Population must be greater than or equal to SC/ST Population.")]
        public Int32? MAST_VILLAGE_TOT_POP { get; set; }

        [Display(Name = "SC/ST Population")]
        [Range(0, 2147483647, ErrorMessage = "SC/ST Population must be valid number.")]
        [Required(ErrorMessage = "SC/ST Population is required.")]
        [RegularExpression(@"^([0-9]+)$", ErrorMessage = "Total Population must be valid number.")]
        public Int32? MAST_VILLAGE_SCST_POP { get; set; }

        [Display(Name = "Is Schedule5")]
        public string MAST_SCHEDULE5 { get; set; }

         [IsBooleanValidator(ErrorMessage = "Please select valid option")]
         public bool IsSchedule5 { get; set; }

         [Display(Name = "Active")]
         [IsBooleanValidator(ErrorMessage = "Please select valid Status")]
         public bool IsActive { get; set; }

        //To get state Name 
        /// <summary>
        /// Master list of state details
        /// </summary>
        public SelectList States
        {
            get
            {
                List<PMGSY.Models.MASTER_STATE> stateList = new List<PMGSY.Models.MASTER_STATE>();

                PMGSY.DAL.MasterDataEntryDAL masterDataEntryDAL = new PMGSY.DAL.MasterDataEntryDAL();

                stateList = masterDataEntryDAL.GetAllStates(false);

                stateList.Insert(0, new PMGSY.Models.MASTER_STATE() { MAST_STATE_CODE = 0, MAST_STATE_NAME = "--Select--" });

                return new SelectList(stateList, "MAST_STATE_CODE", "MAST_STATE_NAME", this.MAST_STATE_CODE);
            }
        }

        public SelectList Districts
        {
            get
            {
                List<PMGSY.Models.MASTER_DISTRICT> districtList = new List<PMGSY.Models.MASTER_DISTRICT>();

                PMGSY.DAL.MasterDataEntryDAL masterDataEntryDAL = new PMGSY.DAL.MasterDataEntryDAL();

                districtList = masterDataEntryDAL.GetAllDistrictsByStateCode(this.MAST_STATE_CODE,false);

                return new SelectList(districtList, "MAST_DISTRICT_CODE", "MAST_DISTRICT_NAME", this.MAST_DISTRICT_CODE);
            }
        }


        public SelectList Blocks
        {
            get
            {
                List<MASTER_BLOCK> blockList = new List<MASTER_BLOCK>();

                PMGSY.DAL.MasterDataEntryDAL masterDataEntryDAL = new PMGSY.DAL.MasterDataEntryDAL();

                blockList = masterDataEntryDAL.GetAllBlocksByDistrictCode(this.MAST_DISTRICT_CODE, false);

                return new SelectList(blockList, "MAST_BLOCK_CODE", "MAST_BLOCK_NAME", this.MAST_BLOCK_CODE);
            }
        }


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
    }
    //class for custom validation
    public class CompareFieldVillagePopValidator : ValidationAttribute, IClientValidatable
    {
        //public string Value { get; set; }
        public string ComparingValue { get; set; }

        public CompareFieldVillagePopValidator(string ComparingValue)
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
                ValidationType = "comparefieldvillagepopvalidator"
            };

        }
    }//end custom validation
}