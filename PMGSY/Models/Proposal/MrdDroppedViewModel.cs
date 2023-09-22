using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;


namespace PMGSY.Models.Proposal
{
    public class SearchDroppedViewModel
    {
        public string User_Action { get; set; }

        public string clrDate { get; set; }

        public string Agency_Name { get; set; }
        public string Batch_Name { get; set; }
        public string Year_Name { get; set; }
        public string Collaboration_Name { get; set; }

        [UIHint("hidden")]
        public string EncryptedClearanceCode { get; set; }

        public string hdnStage { get; set; }
        public string StateName { get; set; }

        [Display(Name = "State")]
        [Required(ErrorMessage = "Please select State. ")]
        [Range(0, int.MaxValue, ErrorMessage = "Please select  State.")]
        public int StateCode { get; set; }
        public List<SelectListItem> StateList { get; set; }

        [Display(Name = "Sanctioned Year")]
        [Range(0, 2090, ErrorMessage = "Please select Sanctioned Year.")]
        [Required(ErrorMessage = "Please select Year.")]
        [RegularExpression("^([0-9]+)$", ErrorMessage = "Sanctioned Year must be valid number.")]
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


        [Display(Name = "Collaboration")]
        public Nullable<int> IMS_COLLABORATION { get; set; }
        public List<SelectListItem> COLLABORATIONS_List { get; set; }

    }

    public class MrdDroppedViewModel
    {
        public int hdnClrTotRoads { get; set; }
        public int hdnClrTotBridge { get; set; }
        public decimal hdnClrRoadLen { get; set; }
        public decimal hdnClrBridgeLen { get; set; }
        public decimal hdnClrRoadMrdShare { get; set; }
        public decimal hdnClrBridgeMrdShare { get; set; }
        public decimal hdnClrRoadStateShare { get; set; }
        public decimal hdnClrBridgeStateShare { get; set; }
        public int hdnClrHAB1000 { get; set; }
        public int hdnClrHAB500 { get; set; }
        public int hdnClrHAB250 { get; set; }
        public int hdnClrHAB100 { get; set; }

        public int hdnDropTotRoads { get; set; }
        public int hdnDropTotBridge { get; set; }
        public decimal hdnDropRoadLen { get; set; }
        public decimal hdnDropBridgeLen { get; set; }
        public decimal hdnDropRoadMrdShare { get; set; }
        public decimal hdnDropBridgeMrdShare { get; set; }
        public decimal hdnDropRoadStateShare { get; set; }
        public decimal hdnDropBridgeStateShare { get; set; }
        public int hdnDropHAB1000 { get; set; }
        public int hdnDropHAB500 { get; set; }
        public int hdnDropHAB250 { get; set; }
        public int hdnDropHAB100 { get; set; }

        public int MAST_EC_ID { get; set; }

        public string hdnStage { get; set; }
        public string StateName { get; set; }

        [Display(Name = "State")]
        [Required(ErrorMessage = "Please select State. ")]
        [Range(0, int.MaxValue, ErrorMessage = "Please select  State.")]
        public int StateCode { get; set; }
        public List<SelectListItem> StateList { get; set; }

        [Display(Name = "Sanctioned Year")]
        [Range(0, 2090, ErrorMessage = "Please select Sanctioned Year.")]
        [Required(ErrorMessage = "Please select Year.")]
        [RegularExpression("^([0-9]+)$", ErrorMessage = "Sanctioned Year must be valid number.")]
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


        [Display(Name = "Collaboration")]
        public Nullable<int> IMS_COLLABORATION { get; set; }
        public List<SelectListItem> COLLABORATIONS_List { get; set; }
        
        [UIHint("hidden")]
        public string EncryptedClearanceCode { get; set; }

        [UIHint("hidden")]
        public string EncryptedDroppedCode { get; set; }

        public int MRD_DROPPED_CODE { get; set; }

        [RegularExpression(@"^([NU]+)$", ErrorMessage = "Invalid New/Upgrade selected")]
        public string UPGRADE_CONNECT { get; set; }

        public string STAGE_COMPLETE { get; set; }

        public string clrDate { get; set; }

        public string Agency_Name { get; set; }
        public string Batch_Name { get; set; }
        public string Year_Name { get; set; }
        public string Collaboration_Name { get; set; }

        public int MRD_CLEARANCE_CODE { get; set; }

        public Nullable<int> MRD_ORG_CLEARANCE_CODE { get; set; }

        public string MRD_CLEARANCE_STATUS { get; set; }

        [Display(Name = "Clearance Date")]
        [RegularExpression(@"^(((0[1-9]|[12]\d|3[01])\/(0[13578]|1[02])\/((19|[2-9]\d)\d{2}))|((0[1-9]|[12]\d|30)\/(0[13456789]|1[012])\/((19|[2-9]\d)\d{2}))|((0[1-9]|1\d|2[0-8])\/02\/((19|[2-9]\d)\d{2}))|(29\/02\/((1[6-9]|[2-9]\d)(0[48]|[2468][048]|[13579][26])|((16|[2468][048]|[3579][26])00))))$", ErrorMessage = "Clearance Date must be in dd/mm/yyyy format.")]
        [Required(ErrorMessage = "Please select Clearance Date.")]
        public string MRD_CLEARANCE_DATE { get; set; }

        [Display(Name = "Revision Date")]
        [RegularExpression(@"^(((0[1-9]|[12]\d|3[01])\/(0[13578]|1[02])\/((19|[2-9]\d)\d{2}))|((0[1-9]|[12]\d|30)\/(0[13456789]|1[012])\/((19|[2-9]\d)\d{2}))|((0[1-9]|1\d|2[0-8])\/02\/((19|[2-9]\d)\d{2}))|(29\/02\/((1[6-9]|[2-9]\d)(0[48]|[2468][048]|[13579][26])|((16|[2468][048]|[3579][26])00))))$", ErrorMessage = "Revision Date must be in dd/mm/yyyy format.")]
        [Required(ErrorMessage = "Please select Revision Date.")]
        public string MRD_REVISION_DATE { get; set; }

        public string MRD_REVISION_NUMBER { get; set; }

        [Display(Name = "Clearance Number")]
        [Required(ErrorMessage = "Please enter Clearance Number.")]
        [RegularExpression(@"^([a-zA-Z0-9-._/() ]+)$", ErrorMessage = "Clearance Number must be valid number.")]
        public string MRD_CLEARANCE_NUMBER { get; set; }

        [Display(Name = "Number of Roads")]
        [Range(0, 2147483647, ErrorMessage = "Number of Roads must be valid number.")]
        [Required(ErrorMessage = "Number of Roads is required.")]
        [RegularExpression(@"^([0-9]+)$", ErrorMessage = "Number of Roads must be valid number.")]
        [ComparePrevValue("hdnClrTotRoads", "hdnDropTotRoads", ErrorMessage = "Total No. of Roads for Dropped Letter cannot be greater than Total No. of Roads for Clearance Letter.")]
        public int MRD_TOTAL_ROADS { get; set; }

        [Display(Name = "Number of Bridges")]
        [Range(0, 2147483647, ErrorMessage = "Number of Bridges must be valid number.")]
        [Required(ErrorMessage = "Number of Bridges is required.")]
        [RegularExpression(@"^([0-9]+)$", ErrorMessage = "Number of Bridges must be valid number.")]
        [ComparePrevValue("hdnClrTotBridge", "hdnDropTotBridge", ErrorMessage = "Total No. of Bridges for Dropped Letter cannot be greater than Total No. of Bridges for Clearance Letter.")]
        public int MRD_TOTAL_LSB { get; set; }


        [Display(Name = "Road MoRD share")]
        [RegularExpression(@"^\s*(?=.*[0-9])\d*(?:\.\d{1,4})?\s*$", ErrorMessage = "Invalid Road MoRD share,Can only contains Numeric values and 4 digit after decimal place")]
        [Range(0, 999999999999.9999, ErrorMessage = "Invalid Road MoRD Share,Can only contains Numeric values and 4 digit after decimal place")]
        [Required(ErrorMessage = "Please enter Road MoRD Share.")]
        [ComparePrevValue("hdnClrRoadMrdShare", "hdnDropRoadMrdShare", ErrorMessage = "Total MoRD Road Share for Dropped Letter cannot be greater than Total MoRD Road Share for Clearance Letter.")]
        public decimal MRD_ROAD_MORD_SHARE_AMT { get; set; }


        [Display(Name = "Road State share")]
        [RegularExpression(@"^\s*(?=.*[0-9])\d*(?:\.\d{1,4})?\s*$", ErrorMessage = "Invalid Road State share,Can only contains Numeric values and 4 digit after decimal place")]
        [Range(0, 999999999999.9999, ErrorMessage = "Invalid Road State share ,Can only contains Numeric values and 4 digit after decimal place")]
        [Required(ErrorMessage = "Please enter Road State share.")]
        [ComparePrevValue("hdnClrRoadStateShare", "hdnDropRoadStateShare", ErrorMessage = "Total Road State Share for Dropped Letter cannot be greater than Total Road State Share for Clearance Letter.")]
        public decimal MRD_ROAD_STATE_SHARE_AMT { get; set; }


        [Display(Name = "Total Road Sanctioned Amount")]
        [RegularExpression(@"^\s*(?=.*[0-9])\d*(?:\.\d{1,4})?\s*$", ErrorMessage = "Invalid Total Road Sanctioned Amount ,Can only contains Numeric values and 4 digit after decimal place")]
        [Range(0, 999999999999.9999, ErrorMessage = "Invalid Total Road Sanctioned Amount ,Can only contains Numeric values and 4 digit after decimal place")]
        [Required(ErrorMessage = "Please enter Total Road Sanctioned Amount.")]
        public decimal MRD_ROAD_TOTAL_AMT { get; set; }

        [Display(Name = "Bridge MoRD share")]
        [RegularExpression(@"^\s*(?=.*[0-9])\d*(?:\.\d{1,4})?\s*$", ErrorMessage = "Invalid Bridge  MoRD share, Can only contains Numeric values and 4 digit after decimal place")]
        [Range(0, 999999999999.9999, ErrorMessage = "Invalid Bridge  MoRD share, Can only contains Numeric values and 4 digit after decimal place")]
        [Required(ErrorMessage = "Please enter Bridge MoRD share.")]
        [ComparePrevValue("hdnClrBridgeMrdShare", "hdnDropBridgeMrdShare", ErrorMessage = "Total Bridge MoRD Share for Dropped Letter cannot be greater than Total Bridge MoRD Share for Clearance Letter.")]
        public decimal MRD_LSB_MORD_SHARE_AMT { get; set; }

        [Display(Name = "Bridge State share")]
        [RegularExpression(@"^\s*(?=.*[0-9])\d*(?:\.\d{1,4})?\s*$", ErrorMessage = "Invalid LSB State share ,Can only contains Numeric values and 4 digit after decimal place")]
        [Range(0, 999999999999.9999, ErrorMessage = "Invalid Bridge State share ,Can only contains Numeric values and 4 digit after decimal place")]
        [Required(ErrorMessage = "Please enter Bridge State share.")]
        [ComparePrevValue("hdnClrBridgeStateShare", "hdnDropBridgeStateShare", ErrorMessage = "Total State Bridge Share for Dropped Letter cannot be greater than Total State Bridge Share for Clearance Letter.")]
        public decimal MRD_LSB_STATE_SHARE_AMT { get; set; }

        [Display(Name = "Total Bridge Sanctioned Amount")]
        [RegularExpression(@"^\s*(?=.*[0-9])\d*(?:\.\d{1,4})?\s*$", ErrorMessage = "Invalid Total Bridge Sanctioned Amount ,Can only contains Numeric values and 4 digit after decimal place")]
        [Range(0, 999999999999.9999, ErrorMessage = "Invalid Total Bridge Sanctioned Amount ,Can only contains Numeric values and 4 digit after decimal place")]
        [Required(ErrorMessage = "Please enter Total Bridge Sanctioned Amount.")]
        public decimal MRD_LSB_TOTAL_AMT { get; set; }

        [Display(Name = "Total MoRD share")]
        [RegularExpression(@"^\s*(?=.*[0-9])\d*(?:\.\d{1,4})?\s*$", ErrorMessage = "Invalid Total MoRD share,Can only contains Numeric values and 4 digit after decimal place")]
        [Range(0, 999999999999.9999, ErrorMessage = "Invalid Total MoRD share,Can only contains Numeric values and 4 digit after decimal place")]
        [Required(ErrorMessage = "Please enter Total MoRD share.")]
        public decimal MRD_TOTAL_MORD_SHARE_AMT { get; set; }

        [Display(Name = "Total State share")]
        [RegularExpression(@"^\s*(?=.*[0-9])\d*(?:\.\d{1,4})?\s*$", ErrorMessage = "Invalid Total State share, Can only contains Numeric values and 4 digit after decimal place")]
        [Range(0, 999999999999.9999, ErrorMessage = "Invalid Total State share, Can only contains Numeric values and 4 digit after decimal place")]
        [Required(ErrorMessage = "Please enter Total State share.")]
        public decimal MRD_TOTAL_STATE_SHARE_AMT { get; set; }

        [Display(Name = "Total Sanctioned Amount")]
        [RegularExpression(@"^\s*(?=.*[0-9])\d*(?:\.\d{1,4})?\s*$", ErrorMessage = "Invalid Total Sanctioned Amount ,Can only contains Numeric values and 4 digit after decimal place")]
        [Range(0, 999999999999.9999, ErrorMessage = "Invalid Total Sanctioned Amount ,Can only contains Numeric values and 4 digit after decimal place")]
        [Required(ErrorMessage = "Please enter Total Sanctioned Amount.")]
        public decimal MRD_TOTAL_SANCTIONED_AMT { get; set; }

        [Display(Name = "Total Road Length")]
        [RegularExpression(@"^\s*(?=.*[0-9])\d*(?:\.\d{1,4})?\s*$", ErrorMessage = "Invalid Total Road Length ,Can only contains Numeric values and 4 digit after decimal place")]
        [Range(0, 999999999999.9999, ErrorMessage = "Invalid Total Road Length ,Can only contains Numeric values and 4 digit after decimal place")]
        [Required(ErrorMessage = "Please enter Total Road Length.")]
        [ComparePrevValue("hdnClrRoadLen", "hdnDropRoadLen", ErrorMessage = "Total Road Length for Dropped Letter cannot be greater than Total Road Length of Bridges for Clearance Letter.")]
        public decimal MRD_TOTAL_ROAD_LENGTH { get; set; }

        [Display(Name = "Total Bridge Length")]
        [RegularExpression(@"^\s*(?=.*[0-9])\d*(?:\.\d{1,4})?\s*$", ErrorMessage = "Invalid Total Bridge Length ,Can only contains Numeric values and 4 digit after decimal place")]
        [Range(0, 999999999999.9999, ErrorMessage = "Invalid Total Bridge Length ,Can only contains Numeric values and 4 digit after decimal place")]
        [Required(ErrorMessage = "Please enter Total Bridge Length.")]
        [ComparePrevValue("hdnClrBridgeLen", "hdnDropBridgeLen", ErrorMessage = "Total Bridge Length for Dropped Letter cannot be greater than Total Bridge Length for Clearance Letter.")]
        public decimal MRD_TOTAL_LSB_LENGTH { get; set; }

        [Display(Name = "Hab >1000")]
        //[Range(1000, 2147483647, ErrorMessage = "Habitation must be greater than equal to 1000.")]
        [Range(0, 2147483647, ErrorMessage = "Habitation must be valid number.")]
        [Required(ErrorMessage = "Please enter Habitation.")]
        [RegularExpression(@"^([0-9]+)$", ErrorMessage = "Habitation must be valid number.")]
        [ComparePrevValue("hdnClrHAB1000", "hdnDropHAB1000", ErrorMessage = "Total Habs 1000 for Dropped Letter cannot be greater than Total Habs 1000 Roads for Clearance Letter.")]
        public int MRD_HAB_1000 { get; set; }

        [Display(Name = "Hab >500")]
        //[Range(500, 999, ErrorMessage = "Habitation must be greater than equal to 500 and less than 1000.")]
        [Range(0, 2147483647, ErrorMessage = "Habitation must be valid number.")]
        [Required(ErrorMessage = "Please enter Habitation.")]
        [RegularExpression(@"^([0-9]+)$", ErrorMessage = "Habitation must be valid number.")]
        [ComparePrevValue("hdnClrHAB500", "hdnDropHAB500", ErrorMessage = "Total Habs 500 for Dropped Letter cannot be greater than Total Habs 500 Roads for Clearance Letter.")]
        public int MRD_HAB_500 { get; set; }

        [Display(Name = "Hab >250")]
        //[Range(250, 499, ErrorMessage = "Habitation must be greater than equal to 250 and less than 500.")]
        [Range(0, 2147483647, ErrorMessage = "Habitation must be valid number.")]
        [Required(ErrorMessage = "Please enter Habitation.")]
        [RegularExpression(@"^([0-9]+)$", ErrorMessage = "Habitation must be valid number.")]
        [ComparePrevValue("hdnClrHAB250", "hdnDropHAB250", ErrorMessage = "Total Habs 250 for Dropped Letter cannot be greater than Total Habs 250 Roads for Clearance Letter.")]
        public int MRD_HAB_250_ELIGIBLE { get; set; }

        [Display(Name = "Hab >100")]
        //[Range(250, 499, ErrorMessage = "Habitation must be greater than equal to 100 and less than 249.")]
        [Range(0, 2147483647, ErrorMessage = "Habitation must be valid number.")]
        [Required(ErrorMessage = "Please enter Habitation.")]
        [RegularExpression(@"^([0-9]+)$", ErrorMessage = "Habitation must be valid number.")]
        [ComparePrevValue("hdnClrHAB100", "hdnDropHAB100", ErrorMessage = "Total Habs 100 for Dropped Letter cannot be greater than Total Habs 100 Roads for Clearance Letter.")]
        public int MRD_HAB_100_ELIGIBLE { get; set; }

        [Display(Name = "Remarks")]
        [StringLength(255, ErrorMessage = "Remarks should not be greater than 255 characters.")]
        [RegularExpression(@"^([-0-9a-zA-Z'.,: ]+)$", ErrorMessage = "Invalid Remarks.")]
        public string MRD_DROPPED_REMARKS { get; set; }

        public string MRD_DROPPED_PDF_FILE { get; set; }

        public string MRD_CLEARANCE_PDF_FILE { get; set; }
        public string MRD_CLEARANCE_REVISED_PDF_FILE { get; set; }
        public string MRD_ROAD_PDF_FILE { get; set; }
        public string MRD_ROAD_REVISED_PDF_FILE { get; set; }
        public string MRD_ROAD_EXCEL_FILE { get; set; }
        public string MRD_ROAD_REVISED_EXCEL_FILE { get; set; }
        public string User_Action { get; set; }

        public string Temp_MRD_DROPPED_PDF_FILE { get; set; }

        public string Temp_MRD_CLEARANCE_PDF_FILE { get; set; }
        public string Temp_MRD_ROAD_PDF_FILE { get; set; }
        public string Temp_MRD_ROAD_EXCEL_FILE { get; set; }

    }

    public class ComparePrevValueAttribute : ValidationAttribute, IClientValidatable
    {
        private readonly string PropertyPreviousCValue;
        private readonly string PropertyPreviousDValue;

        public ComparePrevValueAttribute(string previousCValue, string previousDValue)
        {
            this.PropertyPreviousCValue = previousCValue;
            this.PropertyPreviousDValue = previousDValue;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var PrevCValue = validationContext.ObjectType.GetProperty(this.PropertyPreviousCValue);
            var PrevDValue = validationContext.ObjectType.GetProperty(this.PropertyPreviousDValue);
            var currValue = PrevDValue.GetValue(validationContext.ObjectInstance, null);

            //if (currValue.ToString().ToLower() == "f" || currValue.ToString().ToLower() == "a" || currValue.ToString().ToLower() == "l")
            //{
            //    return ValidationResult.Success;
            //}

            //if (propertyPrevValue == null)
            //{
            //    return new ValidationResult(string.Format("unknown property {0}", this.PropertyPreviousCValue));
            //}

            //var existingValue = Convert.ToInt32(propertyPrevValue.GetValue(validationContext.ObjectInstance, null));

            //var newLength = Convert.ToInt32(value);

            //if (newLength == null)
            //{
            //    return new ValidationResult(FormatErrorMessage(validationContext.DisplayName));
            //}

            //if (newLength < existingValue)
            //{
            //    return new ValidationResult(FormatErrorMessage(validationContext.DisplayName));
            //}

            int calcValue = Convert.ToInt32(PrevDValue) + Convert.ToInt32(currValue);
            if (calcValue > Convert.ToInt32(PrevCValue))
            {
                return new ValidationResult(FormatErrorMessage(validationContext.DisplayName));
            }

            return ValidationResult.Success;
        }

        public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
        {
            var rule = new ModelClientValidationRule
            {
                ErrorMessage = this.ErrorMessageString,
                ValidationType = "compareprevvalue"
            };
            rule.ValidationParameters.Add("oldroadclerancecount", this.PropertyPreviousCValue);
            rule.ValidationParameters.Add("oldroaddroppedcount", this.PropertyPreviousDValue);
            return new[] { rule };
        }
    }
}