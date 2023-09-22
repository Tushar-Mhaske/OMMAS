using PMGSY.Common;
using PMGSY.Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PMGSY.Models.Proposal
{

    public class MrdClearenceViewModel
    {
        public MrdClearenceViewModel()
        {
            CommonFunctions commonFunctions = new CommonFunctions();

            StateList = new List<SelectListItem>();
            StateList = commonFunctions.PopulateStates(true);
            // StateList.Insert(0, (new SelectListItem { Text = "All States", Value = "0"}));
            Mast_State_Code = PMGSYSession.Current.StateCode == 0 ? 0 : PMGSYSession.Current.StateCode;
            StateList.Find(x => x.Value == Mast_State_Code.ToString()).Selected = true;
            BatchList = commonFunctions.PopulateBatch(false);
            BatchList.Find(x => x.Value == "-1").Value = "0";
            Mast_AgencyList = new List<SelectListItem>();
            if (Mast_State_Code > 0)
            {
                Mast_AgencyList = commonFunctions.PopulateAgenciesByStateAndDepartmentwise(Mast_State_Code, PMGSYSession.Current.AdminNdCode, false);
            }
            else
            {
                Mast_AgencyList.Insert(0, (new SelectListItem { Text = "Select Agency", Value = "0" }));
            }
            // Mast_AgencyList.Find(x => x.Value == "-1").Value = "0";
            PhaseYear = DateTime.Now.Year;
            //PhaseYearList = new SelectList(commonFunctions.PopulateYears(true), "Value", "Text").ToList();
            PhaseYearList = new SelectList(commonFunctions.PopulateFinancialYear(true, false), "Value", "Text").ToList();
            COLLABORATIONS_List = commonFunctions.PopulateFundingAgency(false);

            //ImsFileTypeList = new List<SelectListItem>();
            //ImsFileTypeList.Add(new SelectListItem { Text = "Select File Type", Value = "%" });
            //ImsFileTypeList.Add(new SelectListItem { Text = "Audit Report", Value = "A" });
            //ImsFileTypeList.Add(new SelectListItem { Text = "Utilization Certificate", Value = "U" });
            //ImsFileTypeList.Add(new SelectListItem { Text = "Others", Value = "O" });

        }

        public string State_Name { get; set; }
        public string Agency_Name { get; set; }
        public string Batch_Name { get; set; }
        public string Year_Name { get; set; }
        public string Collaboration_Name { get; set; }

        [UIHint("hidden")]
        public string EncryptedClearanceCode { get; set; }


        public int MRD_CLEARANCE_CODE { get; set; }

        public Nullable <int> MRD_ORG_CLEARANCE_CODE { get; set; }

        public string MRD_CLEARANCE_STATUS { get; set; }

        [Display(Name = "State")]
        [Required(ErrorMessage = "Please select State. ")]
        [Range(1, int.MaxValue, ErrorMessage = "Please select  State.")]
        public int Mast_State_Code { get; set; }
        public List<SelectListItem> StateList { get; set; }

        [Display(Name = "Sanctioned Year")]
        [Range(1, 2090, ErrorMessage = "Please select Sanctioned Year.")]
        [Required(ErrorMessage = "Please select Sanctioned Year.")]
        [RegularExpression("^([0-9]+)$", ErrorMessage = "Sanctioned Year must be valid number.")]
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


        [Display(Name = "Collaboration")]
        [Range(1, 10, ErrorMessage = "Please select Collaboration.")]
        [Required(ErrorMessage = "Please select Collaboration.")]
        [RegularExpression("^([0-9]+)$", ErrorMessage = "Collaboration must be valid number.")]
        public int IMS_COLLABORATION { get; set; }
        public List<SelectListItem> COLLABORATIONS_List { get; set; }

        //[Display(Name = "Type")]
        //[RegularExpression(@"^([AUO]+)$", ErrorMessage = "Please select File Type.")]
        //public string ImsFileType { get; set; }
        //public List<SelectListItem> ImsFileTypeList { get; set; }      


        [Display(Name = "Clearance Date")]
        [RegularExpression(@"^(((0[1-9]|[12]\d|3[01])\/(0[13578]|1[02])\/((19|[2-9]\d)\d{2}))|((0[1-9]|[12]\d|30)\/(0[13456789]|1[012])\/((19|[2-9]\d)\d{2}))|((0[1-9]|1\d|2[0-8])\/02\/((19|[2-9]\d)\d{2}))|(29\/02\/((1[6-9]|[2-9]\d)(0[48]|[2468][048]|[13579][26])|((16|[2468][048]|[3579][26])00))))$", ErrorMessage = "Clearance Date must be in dd/mm/yyyy format.")]
        [Required(ErrorMessage = "Please select Clearance Date.")]
        public string MRD_CLEARANCE_DATE { get; set; }

        //[Display(Name = "Revision Date")]
        //[RegularExpression(@"^(((0[1-9]|[12]\d|3[01])\/(0[13578]|1[02])\/((19|[2-9]\d)\d{2}))|((0[1-9]|[12]\d|30)\/(0[13456789]|1[012])\/((19|[2-9]\d)\d{2}))|((0[1-9]|1\d|2[0-8])\/02\/((19|[2-9]\d)\d{2}))|(29\/02\/((1[6-9]|[2-9]\d)(0[48]|[2468][048]|[13579][26])|((16|[2468][048]|[3579][26])00))))$", ErrorMessage = "Revision Date must be in dd/mm/yyyy format.")]
        //[Required(ErrorMessage = "Please select Revision Date.")]
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
        public int MRD_TOTAL_ROADS { get; set; }

        [Display(Name = "Number of Bridges")]
        [Range(0, 2147483647, ErrorMessage = "Number of Bridges must be valid number.")]
        [Required(ErrorMessage = "Number of Bridges is required.")]
        [RegularExpression(@"^([0-9]+)$", ErrorMessage = "Number of Bridges must be valid number.")]
        public int MRD_TOTAL_LSB { get; set; }


        [Display(Name = "Road MoRD share")]
        [RegularExpression(@"^\s*(?=.*[0-9])\d*(?:\.\d{1,4})?\s*$", ErrorMessage = "Invalid Road MoRD share,Can only contains Numeric values and 4 digit after decimal place")]
        [Range(0, 999999999999.9999, ErrorMessage = "Invalid Road MoRD Share,Can only contains Numeric values and 4 digit after decimal place")]
        [Required(ErrorMessage = "Please enter Road MoRD Share.")]
        public decimal MRD_ROAD_MORD_SHARE_AMT { get; set; }


        [Display(Name = "Road State share")]
        [RegularExpression(@"^\s*(?=.*[0-9])\d*(?:\.\d{1,4})?\s*$", ErrorMessage = "Invalid Road State share,Can only contains Numeric values and 4 digit after decimal place")]
        [Range(0,999999999999.9999, ErrorMessage = "Invalid Road State share ,Can only contains Numeric values and 4 digit after decimal place")]
        [Required(ErrorMessage = "Please enter Road State share.")]
        public decimal MRD_ROAD_STATE_SHARE_AMT { get; set; }


        [Display(Name = "Total Road Sanctioned Amount")]
        [RegularExpression(@"^\s*(?=.*[0-9])\d*(?:\.\d{1,4})?\s*$", ErrorMessage = "Invalid Total Road Sanctioned Amount ,Can only contains Numeric values and 4 digit after decimal place")]
        [Range(0,999999999999.9999, ErrorMessage = "Invalid Total Road Sanctioned Amount ,Can only contains Numeric values and 4 digit after decimal place")]
        [Required(ErrorMessage = "Please enter Total Road Sanctioned Amount.")]
        public decimal MRD_ROAD_TOTAL_AMT { get; set; }

        [Display(Name = "Bridge  MoRD share")]
        [RegularExpression(@"^\s*(?=.*[0-9])\d*(?:\.\d{1,4})?\s*$", ErrorMessage = "Invalid Bridge  MoRD share, Can only contains Numeric values and 4 digit after decimal place")]
        [Range(0,999999999999.9999, ErrorMessage = "Invalid Bridge  MoRD share, Can only contains Numeric values and 4 digit after decimal place")]
        [Required(ErrorMessage = "Please enter Bridge MoRD share.")]
        public decimal MRD_LSB_MORD_SHARE_AMT { get; set; }

        [Display(Name = "Bridge State share")]
        [RegularExpression(@"^\s*(?=.*[0-9])\d*(?:\.\d{1,4})?\s*$", ErrorMessage = "Invalid LSB State share ,Can only contains Numeric values and 4 digit after decimal place")]
        [Range(0,999999999999.9999, ErrorMessage = "Invalid Bridge State share ,Can only contains Numeric values and 4 digit after decimal place")]
        [Required(ErrorMessage = "Please enter Bridge State share.")]
        public decimal MRD_LSB_STATE_SHARE_AMT { get; set; }

        [Display(Name = "Total Bridge Sanctioned Amount")]
        [RegularExpression(@"^\s*(?=.*[0-9])\d*(?:\.\d{1,4})?\s*$", ErrorMessage = "Invalid Total Bridge Sanctioned Amount ,Can only contains Numeric values and 4 digit after decimal place")]
        [Range(0,999999999999.9999, ErrorMessage = "Invalid Total Bridge Sanctioned Amount ,Can only contains Numeric values and 4 digit after decimal place")]
        [Required(ErrorMessage = "Please enter Total Bridge Sanctioned Amount.")]
        public decimal MRD_LSB_TOTAL_AMT { get; set; }

        [Display(Name = "Total MoRD share")]
        [RegularExpression(@"^\s*(?=.*[0-9])\d*(?:\.\d{1,4})?\s*$", ErrorMessage = "Invalid Total MoRD share,Can only contains Numeric values and 4 digit after decimal place")]
        [Range(0,999999999999.9999, ErrorMessage = "Invalid Total MoRD share,Can only contains Numeric values and 4 digit after decimal place")]
        [Required(ErrorMessage = "Please enter Total MoRD share.")]
        public decimal MRD_TOTAL_MORD_SHARE_AMT { get; set; }

        [Display(Name = "Total State share")]
        [RegularExpression(@"^\s*(?=.*[0-9])\d*(?:\.\d{1,4})?\s*$", ErrorMessage = "Invalid Total State share, Can only contains Numeric values and 4 digit after decimal place")]
        [Range(0,999999999999.9999, ErrorMessage = "Invalid Total State share, Can only contains Numeric values and 4 digit after decimal place")]
        [Required(ErrorMessage = "Please enter Total State share.")]
        public decimal MRD_TOTAL_STATE_SHARE_AMT { get; set; }

        [Display(Name = "Total Sanctioned Amount")]
        [RegularExpression(@"^\s*(?=.*[0-9])\d*(?:\.\d{1,4})?\s*$", ErrorMessage = "Invalid Total Sanctioned Amount ,Can only contains Numeric values and 4 digit after decimal place")]
        [Range(0,999999999999.9999, ErrorMessage = "Invalid Total Sanctioned Amount ,Can only contains Numeric values and 4 digit after decimal place")]
        [Required(ErrorMessage = "Please enter Total Sanctioned Amount.")]
        public decimal MRD_TOTAL_SANCTIONED_AMT { get; set; }

        [Display(Name = "Total Road Length")]
        [RegularExpression(@"^\s*(?=.*[0-9])\d*(?:\.\d{1,4})?\s*$", ErrorMessage = "Invalid Total Road Length ,Can only contains Numeric values and 4 digit after decimal place")]
        [Range(0,999999999999.9999, ErrorMessage = "Invalid Total Road Length ,Can only contains Numeric values and 4 digit after decimal place")]
        [Required(ErrorMessage = "Please enter Total Road Length.")]
        public decimal MRD_TOTAL_ROAD_LENGTH { get; set; }

        [Display(Name = "Total Bridge Length")]
        [RegularExpression(@"^\s*(?=.*[0-9])\d*(?:\.\d{1,4})?\s*$", ErrorMessage = "Invalid Total Bridge Length ,Can only contains Numeric values and 4 digit after decimal place")]
        [Range(0,999999999999.9999, ErrorMessage = "Invalid Total Bridge Length ,Can only contains Numeric values and 4 digit after decimal place")]
        [Required(ErrorMessage = "Please enter Total Bridge Length.")]
        public decimal MRD_TOTAL_LSB_LENGTH { get; set; }

        [Display(Name = "Hab >1000")]
        // [Range(1000, 2147483647, ErrorMessage = "Habitation must be greater than equal to 1000.")]
        [Range(0,2147483647, ErrorMessage = "Habitation must be valid number.")]
        [Required(ErrorMessage = "Please enter Habitation.")]
        [RegularExpression(@"^([0-9]+)$", ErrorMessage = "Habitation must be valid number.")]
        public int MRD_HAB_1000 { get; set; }

        [Display(Name = "Hab >500")]
        //[Range(500, 999, ErrorMessage = "Habitation must be greater than equal to 500 and less than 1000.")]
        [Range(0,2147483647, ErrorMessage = "Habitation must be valid number.")]
        [Required(ErrorMessage = "Please enter Habitation.")]
        [RegularExpression(@"^([0-9]+)$", ErrorMessage = "Habitation must be valid number.")]
        public int MRD_HAB_500 { get; set; }

        [Display(Name = "Hab >250")]
        //[Range(250, 499, ErrorMessage = "Habitation must be greater than equal to 250 and less than 500.")]
        [Range(0,2147483647, ErrorMessage = "Habitation must be valid number.")]
        [Required(ErrorMessage = "Please enter Habitation.")]
        [RegularExpression(@"^([0-9]+)$", ErrorMessage = "Habitation must be valid number.")]
        public int MRD_HAB_250_ELIGIBLE { get; set; }

        [Display(Name = "Hab >100")]
        // [Range(250, 499, ErrorMessage = "Habitation must be greater than equal to 100 and less than 249.")]
        [Range(0,2147483647, ErrorMessage = "Habitation must be valid number.")]
        [Required(ErrorMessage = "Please enter Habitation.")]
        [RegularExpression(@"^([0-9]+)$", ErrorMessage = "Habitation must be valid number.")]
        public int MRD_HAB_100_ELIGIBLE { get; set; }


        public string MRD_CLEARANCE_PDF_FILE { get; set; }
        public string MRD_CLEARANCE_REVISED_PDF_FILE { get; set; }
        public string MRD_ROAD_PDF_FILE { get; set; }
        public string MRD_ROAD_REVISED_PDF_FILE { get; set; }
        public string MRD_ROAD_EXCEL_FILE { get; set; }
        public string MRD_ROAD_REVISED_EXCEL_FILE { get; set; }
        public string User_Action { get; set; }

        public string Temp_MRD_CLEARANCE_PDF_FILE { get; set; }
        public string Temp_MRD_ROAD_PDF_FILE { get; set; }
        public string Temp_MRD_ROAD_EXCEL_FILE { get; set; }

        [Required]
        [Display(Name="New / Upgradation")]
        [RegularExpression(@"^[NU]",ErrorMessage="Please select New / Upgradation")]
        public string UPGRADE_CONNECT { get; set; }

        [Display(Name="Stage Construction")]
        //[RegularExpression(@"^[12S]")]
        public string STAGE_COMPLETE { get; set; }

        [Display(Name = "Remarks")]
        [RegularExpression(@"^[a-zA-Z0-9 ,./()-]+$", ErrorMessage = "Invalid Remarks,Can only contains AlphaNumeric values and [,.()-]")]
        public string MRD_CLEARANCE_REMARKS { get; set; }

    }

    public class MrdClearenceSearchViewModel
    {
        public MrdClearenceSearchViewModel()
        {
            CommonFunctions commonFunctions = new CommonFunctions();
            StateList = new List<SelectListItem>();
            StateName = PMGSYSession.Current.StateCode == 0 ? "0" : PMGSYSession.Current.StateName.Trim();
            StateList = commonFunctions.PopulateStates(false);
            StateList.Insert(0, (new SelectListItem { Text = "All States", Value = "0" }));
            StateCode = PMGSYSession.Current.StateCode == 0 ? 0 : PMGSYSession.Current.StateCode;
            StateList.Find(x => x.Value == StateCode.ToString()).Selected = true;
            BatchList = commonFunctions.PopulateBatch(true);
            Mast_AgencyList = new List<SelectListItem>();
            if (StateCode > 0)
            {
                Mast_AgencyList = commonFunctions.PopulateAgenciesByStateAndDepartmentwise(StateCode, PMGSYSession.Current.AdminNdCode, true);
            }
            else
            {
                Mast_AgencyList.Insert(0, (new SelectListItem { Text = "All Agencies", Value = "0" }));
            }
            PhaseYear = DateTime.Now.Year;
            //PhaseYearList = new SelectList(commonFunctions.PopulateYears(false), "Value", "Text").ToList();
            //PhaseYearList.Insert(0, (new SelectListItem { Text = "All Year", Value = "0" }));
            PhaseYearList = new SelectList(commonFunctions.PopulateFinancialYear(true, true), "Value", "Text").ToList();
            COLLABORATIONS_List = commonFunctions.PopulateFundingAgency(true);

        }


        public string StateName { get; set; }

        public int MAST_EC_ID { get; set; }

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

    public class MrdClearenceViewModelImageUploadViewModel
    {
        [UIHint("hidden")]
        public string EncryptedFileId { get; set; }
        public int MAST_File_ID { get; set; }

        public int? NumberofFiles { get; set; }

        public string url { get; set; }
        public string thumbnail_url { get; set; }
        public string name { get; set; }


        public string type { get; set; } //n
        public int size { get; set; }
        public string delete_url { get; set; }
        public string delete_type { get; set; } //n


        public string file_type { get; set; }
        public string ImsFileName { get; set; }
        public string ImsFilePath { get; set; }

        //public string QMName { get; set; }
        //public string QMDesignation { get; set; }
        //public string QMType { get; set; }
        //public string QMState { get; set; }

        //Added By Abhishek kamble 26-Apr-2014
        public string ErrorMessage { get; set; }

    }

    public class MrdClearenceRevisionViewModel
    {
        public MrdClearenceRevisionViewModel()
        {
            CommonFunctions commonFunctions = new CommonFunctions();

            StateList = new List<SelectListItem>();
            State_Name = PMGSYSession.Current.StateCode == 0 ? "0" : PMGSYSession.Current.StateName.Trim();
            StateList = commonFunctions.PopulateStates(true);
            // StateList.Insert(0, (new SelectListItem { Text = "All States", Value = "0"}));
            Mast_State_Code = PMGSYSession.Current.StateCode == 0 ? 0 : PMGSYSession.Current.StateCode;
            StateList.Find(x => x.Value == Mast_State_Code.ToString()).Selected = true;
            BatchList = commonFunctions.PopulateBatch(false);
            BatchList.Find(x => x.Value == "-1").Value = "0";
            Mast_AgencyList = new List<SelectListItem>();
            if (Mast_State_Code > 0)
            {
                Mast_AgencyList = commonFunctions.PopulateAgenciesByStateAndDepartmentwise(Mast_State_Code, PMGSYSession.Current.AdminNdCode, false);
            }
            else
            {
                Mast_AgencyList.Insert(0, (new SelectListItem { Text = "Select Agency", Value = "0" }));
            }
            // Mast_AgencyList.Find(x => x.Value == "-1").Value = "0";
            PhaseYear = DateTime.Now.Year;
            //PhaseYearList = new SelectList(commonFunctions.PopulateYears(true), "Value", "Text").ToList();
            PhaseYearList = new SelectList(commonFunctions.PopulateFinancialYear(true, false), "Value", "Text").ToList();
            COLLABORATIONS_List = commonFunctions.PopulateFundingAgency(false);

            //ImsFileTypeList = new List<SelectListItem>();
            //ImsFileTypeList.Add(new SelectListItem { Text = "Select File Type", Value = "%" });
            //ImsFileTypeList.Add(new SelectListItem { Text = "Audit Report", Value = "A" });
            //ImsFileTypeList.Add(new SelectListItem { Text = "Utilization Certificate", Value = "U" });
            //ImsFileTypeList.Add(new SelectListItem { Text = "Others", Value = "O" });
        }
        public string State_Name { get; set; }
        public string Agency_Name { get; set; }
        public string Batch_Name { get; set; }
        public string Year_Name { get; set; }
        public string Collaboration_Name { get; set; }
        public string Clerance_Number { get; set; }
        //[UIHint("hidden")]
        //public string EncryptedClearanceCode { get; set; }
        //public int MRD_CLEARANCE_CODE { get; set; }

        //public int MRD_ORG_CLEARANCE_CODE { get; set; }
        //public string MRD_CLEARANCE_STATUS { get; set; }
        [UIHint("hidden")]
        public string EncryptedClearanceCode { get; set; }

        [UIHint("hidden")]
        public string EncryptedClearanceRevisionCode { get; set; }

        public int MRD_Revision_CODE { get; set; }


        public int MRD_CLEARANCE_CODE { get; set; }

        public Nullable<int> MRD_ORG_CLEARANCE_CODE { get; set; }

        public string MRD_CLEARANCE_STATUS { get; set; }

        public string MRD_CLEARANCE_NUMBER { get; set; }
        public string MRD_CLEARANCE_DATE { get; set; }

        [Display(Name = "State")]
        [Required(ErrorMessage = "Please select State. ")]
        [Range(1, int.MaxValue, ErrorMessage = "Please select  State.")]
        public int Mast_State_Code { get; set; }
        public List<SelectListItem> StateList { get; set; }

        [Display(Name = "Sanctioned Year")]
        [Range(1, 2090, ErrorMessage = "Please select Sanctioned Year.")]
        [Required(ErrorMessage = "Please select Sanctioned Year.")]
        [RegularExpression("^([0-9]+)$", ErrorMessage = "Sanctioned Year must be valid number.")]
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


        [Display(Name = "Collaboration")]
        [Range(1, 10, ErrorMessage = "Please select Collaboration.")]
        [Required(ErrorMessage = "Please select Collaboration.")]
        [RegularExpression("^([0-9]+)$", ErrorMessage = "Collaboration must be valid number.")]
        public int IMS_COLLABORATION { get; set; }
        public List<SelectListItem> COLLABORATIONS_List { get; set; }


        [Display(Name = "Revision Date")]
        [RegularExpression(@"^(((0[1-9]|[12]\d|3[01])\/(0[13578]|1[02])\/((19|[2-9]\d)\d{2}))|((0[1-9]|[12]\d|30)\/(0[13456789]|1[012])\/((19|[2-9]\d)\d{2}))|((0[1-9]|1\d|2[0-8])\/02\/((19|[2-9]\d)\d{2}))|(29\/02\/((1[6-9]|[2-9]\d)(0[48]|[2468][048]|[13579][26])|((16|[2468][048]|[3579][26])00))))$", ErrorMessage = "Revision Date must be in dd/mm/yyyy format.")]
        [Required(ErrorMessage = "Please select Revision Date.")]
        public string MRD_REVISION_DATE { get; set; }

        [Display(Name = "Revision Number")]
        [Required(ErrorMessage = "Please enter Revision Number.")]
        [RegularExpression(@"^([a-zA-Z0-9-._/() ]+)$", ErrorMessage = "Revision Number must be valid number.")]
        public string MRD_REVISION_NUMBER { get; set; }


        [Display(Name = "Number of Roads")]
        [Range(0, 2147483647, ErrorMessage = "Number of Roads must be valid number.")]
        [Required(ErrorMessage = "Number of Roads is required.")]
        [RegularExpression(@"^([0-9]+)$", ErrorMessage = "Number of Roads must be valid number.")]
        public int MRD_TOTAL_ROADS { get; set; }

        [Display(Name = "Number of Bridges")]
        [Range(0, 2147483647, ErrorMessage = "Number of Bridges must be valid number.")]
        [Required(ErrorMessage = "Number of Bridges is required.")]
        [RegularExpression(@"^([0-9]+)$", ErrorMessage = "Number of Bridges must be valid number.")]
        public int MRD_TOTAL_LSB { get; set; }


        [Display(Name = "Road MoRD share")]
        [RegularExpression(@"^\s*(?=.*[0-9])\d*(?:\.\d{1,4})?\s*$", ErrorMessage = "Invalid Road MoRD share,Can only contains Numeric values and 4 digit after decimal place")]
        [Range(0, 999999999999.9999, ErrorMessage = "Invalid Road MoRD Share,Can only contains Numeric values and 4 digit after decimal place")]
        [Required(ErrorMessage = "Please enter Road MoRD Share.")]
        public decimal MRD_ROAD_MORD_SHARE_AMT { get; set; }


        [Display(Name = "Road State share")]
        [RegularExpression(@"^\s*(?=.*[0-9])\d*(?:\.\d{1,4})?\s*$", ErrorMessage = "Invalid Road State share,Can only contains Numeric values and 4 digit after decimal place")]
        [Range(0,999999999999.9999, ErrorMessage = "Invalid Road State share ,Can only contains Numeric values and 4 digit after decimal place")]
        [Required(ErrorMessage = "Please enter Road State share.")]
        public decimal MRD_ROAD_STATE_SHARE_AMT { get; set; }


        [Display(Name = "Total Road Sanctioned Amount")]
        [RegularExpression(@"^\s*(?=.*[0-9])\d*(?:\.\d{1,4})?\s*$", ErrorMessage = "Invalid Total Road Sanctioned Amount ,Can only contains Numeric values and 4 digit after decimal place")]
        [Range(0, 999999999999.9999, ErrorMessage = "Invalid Total Road Sanctioned Amount ,Can only contains Numeric values and 4 digit after decimal place")]
        [Required(ErrorMessage = "Please enter Total Road Sanctioned Amount.")]
        public decimal MRD_ROAD_TOTAL_AMT { get; set; }

        [Display(Name = "Bridge  MoRD share")]
        [RegularExpression(@"^\s*(?=.*[0-9])\d*(?:\.\d{1,4})?\s*$", ErrorMessage = "Invalid Bridge  MoRD share, Can only contains Numeric values and 4 digit after decimal place")]
        [Range(0,999999999999.9999, ErrorMessage = "Invalid Bridge  MoRD share, Can only contains Numeric values and 4 digit after decimal place")]
        [Required(ErrorMessage = "Please enter Bridge MoRD share.")]
        public decimal MRD_LSB_MORD_SHARE_AMT { get; set; }

        [Display(Name = "Bridge State share")]
        [RegularExpression(@"^\s*(?=.*[0-9])\d*(?:\.\d{1,4})?\s*$", ErrorMessage = "Invalid LSB State share ,Can only contains Numeric values and 4 digit after decimal place")]
        [Range(0,999999999999.9999, ErrorMessage = "Invalid Bridge State share ,Can only contains Numeric values and 4 digit after decimal place")]
        [Required(ErrorMessage = "Please enter Bridge State share.")]
        public decimal MRD_LSB_STATE_SHARE_AMT { get; set; }

        [Display(Name = "Total Bridge Sanctioned Amount")]
        [RegularExpression(@"^\s*(?=.*[0-9])\d*(?:\.\d{1,4})?\s*$", ErrorMessage = "Invalid Total Bridge Sanctioned Amount ,Can only contains Numeric values and 4 digit after decimal place")]
        [Range(0,999999999999.9999, ErrorMessage = "Invalid Total Bridge Sanctioned Amount ,Can only contains Numeric values and 4 digit after decimal place")]
        [Required(ErrorMessage = "Please enter Total Bridge Sanctioned Amount.")]
        public decimal MRD_LSB_TOTAL_AMT { get; set; }

        [Display(Name = "Total MoRD share")]
        [RegularExpression(@"^\s*(?=.*[0-9])\d*(?:\.\d{1,4})?\s*$", ErrorMessage = "Invalid Total MoRD share,Can only contains Numeric values and 4 digit after decimal place")]
        [Range(0,999999999999.9999, ErrorMessage = "Invalid Total MoRD share,Can only contains Numeric values and 4 digit after decimal place")]
        [Required(ErrorMessage = "Please enter Total MoRD share.")]
        public decimal MRD_TOTAL_MORD_SHARE_AMT { get; set; }

        [Display(Name = "Total State share")]
        [RegularExpression(@"^\s*(?=.*[0-9])\d*(?:\.\d{1,4})?\s*$", ErrorMessage = "Invalid Total State share, Can only contains Numeric values and 4 digit after decimal place")]
        [Range(0,999999999999.9999, ErrorMessage = "Invalid Total State share, Can only contains Numeric values and 4 digit after decimal place")]
        [Required(ErrorMessage = "Please enter Total State share.")]
        public decimal MRD_TOTAL_STATE_SHARE_AMT { get; set; }

        [Display(Name = "Total Sanctioned Amount")]
        [RegularExpression(@"^\s*(?=.*[0-9])\d*(?:\.\d{1,4})?\s*$", ErrorMessage = "Invalid Total Sanctioned Amount ,Can only contains Numeric values and 4 digit after decimal place")]
        [Range(0,999999999999.9999, ErrorMessage = "Invalid Total Sanctioned Amount ,Can only contains Numeric values and 4 digit after decimal place")]
        [Required(ErrorMessage = "Please enter Total Sanctioned Amount.")]
        public decimal MRD_TOTAL_SANCTIONED_AMT { get; set; }

        [Display(Name = "Total Road Length")]
        [RegularExpression(@"^\s*(?=.*[0-9])\d*(?:\.\d{1,4})?\s*$", ErrorMessage = "Invalid Total Road Length ,Can only contains Numeric values and 4 digit after decimal place")]
        [Range(0,999999999999.9999, ErrorMessage = "Invalid Total Road Length ,Can only contains Numeric values and 4 digit after decimal place")]
        [Required(ErrorMessage = "Please enter Total Road Length.")]
        public decimal MRD_TOTAL_ROAD_LENGTH { get; set; }

        [Display(Name = "Total Bridge Length")]
        [RegularExpression(@"^\s*(?=.*[0-9])\d*(?:\.\d{1,4})?\s*$", ErrorMessage = "Invalid Total Bridge Length ,Can only contains Numeric values and 4 digit after decimal place")]
        [Range(0,999999999999.9999, ErrorMessage = "Invalid Total Bridge Length ,Can only contains Numeric values and 4 digit after decimal place")]
        [Required(ErrorMessage = "Please enter Total Bridge Length.")]
        public decimal MRD_TOTAL_LSB_LENGTH { get; set; }

        [Display(Name = "Hab >1000")]
        // [Range(1000, 2147483647, ErrorMessage = "Habitation must be greater than equal to 1000.")]
        [Range(0,2147483647, ErrorMessage = "Habitation must be valid number.")]
        [Required(ErrorMessage = "Please enter Habitation.")]
        [RegularExpression(@"^([0-9]+)$", ErrorMessage = "Habitation must be valid number.")]
        public int MRD_HAB_1000 { get; set; }

        [Display(Name = "Hab >500")]
        //[Range(500, 999, ErrorMessage = "Habitation must be greater than equal to 500 and less than 1000.")]
        [Range(0,2147483647, ErrorMessage = "Habitation must be valid number.")]
        [Required(ErrorMessage = "Please enter Habitation.")]
        [RegularExpression(@"^([0-9]+)$", ErrorMessage = "Habitation must be valid number.")]
        public int MRD_HAB_500 { get; set; }

        [Display(Name = "Hab >250")]
        //[Range(250, 499, ErrorMessage = "Habitation must be greater than equal to 250 and less than 500.")]
        [Range(0,2147483647, ErrorMessage = "Habitation must be valid number.")]
        [Required(ErrorMessage = "Please enter Habitation.")]
        [RegularExpression(@"^([0-9]+)$", ErrorMessage = "Habitation must be valid number.")]
        public int MRD_HAB_250_ELIGIBLE { get; set; }

        [Display(Name = "Hab >100")]
        // [Range(250, 499, ErrorMessage = "Habitation must be greater than equal to 100 and less than 249.")]
        [Range(0,2147483647,ErrorMessage = "Habitation must be valid number.")]
        [Required(ErrorMessage = "Please enter Habitation.")]
        [RegularExpression(@"^([0-9]+)$", ErrorMessage = "Habitation must be valid number.")]
        public int MRD_HAB_100_ELIGIBLE { get; set; }

        [Required]
        [Display(Name = "New / Upgradation")]
        [RegularExpression(@"^[NU]", ErrorMessage = "Please select New / Upgradation")]
        public string UPGRADE_CONNECT { get; set; }

        [Display(Name = "Stage Construction")]
        public string STAGE_COMPLETE { get; set; }

        [Display(Name = "Remarks")]
        [RegularExpression(@"^[a-zA-Z0-9 ,./()-]+$", ErrorMessage = "Invalid Remarks,Can only contains AlphaNumeric values and [,.()-]")]
        public string MRD_CLEARANCE_REMARKS { get; set; }

        public string MRD_CLEARANCE_REVISED_PDF_FILE { get; set; }
        public string MRD_ROAD_REVISED_PDF_FILE { get; set; }
        public string MRD_ROAD_REVISED_EXCEL_FILE { get; set; }

        public string User_Action { get; set; }

        public string Temp_MRD_REVISED_CLEARANCE_PDF_FILE { get; set; }
        public string Temp_MRD_ROAD_REVISED_PDF_FILE { get; set; }
        public string Temp_MRD_ROAD_REVISED_EXCEL_FILE { get; set; }

    }




}