using PMGSY.Common;
using PMGSY.Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PMGSY.Areas.AnalysisSSRSReport.Models
{
    public class AnalysisAverageLengthModel
    {

        public AnalysisAverageLengthModel()
        {
            CommonFunctions commonFunctions = new CommonFunctions();
            StateList = new List<SelectListItem>();
            DistrictList = new List<SelectListItem>();
            BlockList = new List<SelectListItem>();

            StateName = PMGSYSession.Current.StateCode == 0 ? "0" : PMGSYSession.Current.StateName.Trim();
            DistName = PMGSYSession.Current.DistrictCode == 0 ? "0" : PMGSYSession.Current.DistrictName.Trim();

            Mast_State_Code = PMGSYSession.Current.StateCode;
            Mast_District_Code = PMGSYSession.Current.DistrictCode;

            //LevelCode = PMGSYSession.Current.BlockCode > 0 ? 3 : PMGSYSession.Current.DistrictCode > 0 ? 2 : 1;
            LevelCode = PMGSYSession.Current.DistrictCode > 0 ? 2 : 1;

            StateList = commonFunctions.PopulateStates(false);
            StateList.Insert(0, (new SelectListItem { Text = "All States", Value = "0" }));
            StateCode = PMGSYSession.Current.StateCode;//== 0 ? 1 : PMGSYSession.Current.StateCode;
            StateList.Find(x => x.Value == StateCode.ToString()).Selected = true;

            DistrictList = new List<SelectListItem>();
            if (StateCode == 0)
            {
                DistrictList.Insert(0, (new SelectListItem { Text = "All Districts", Value = "0", Selected = true }));
            }
            else
            {
                DistrictList = commonFunctions.PopulateDistrict(StateCode, true);
                DistrictCode = PMGSYSession.Current.DistrictCode == 0 ? 0 : PMGSYSession.Current.DistrictCode;
                DistrictList.Find(x => x.Value == "-1").Value = "0";
                DistrictList.Find(x => x.Value == DistrictCode.ToString()).Selected = true;

            }
            BlockList = new List<SelectListItem>();
            if (DistrictCode == 0)
            {
                BlockList.Insert(0, (new SelectListItem { Text = "All Blocks", Value = "0", Selected = true }));
            }
            else
            {
                BlockList = commonFunctions.PopulateBlocks(DistrictCode, true);
                BlockList.Find(x => x.Value == "-1").Value = "0";

                //BlockCode = PMGSYSession.Current.BlockCode == 0 ? 0 : PMGSYSession.Current.BlockCode;
                //BlockList.Find(x => x.Value == BlockCode.ToString()).Selected = true;
            }

            //PhaseYear = DateTime.Now.Year;
            PhaseYearList = new SelectList(commonFunctions.PopulateFinancialYear(true, true), "Value", "Text").ToList();
            BatchList = commonFunctions.PopulateBatch(true);
            FundingAgencyList = commonFunctions.PopulateFundingAgency(true);
            FundingAgencyList.Find(x => x.Value == "-1").Value = "0";
            StatusList = new List<SelectListItem>();
            StatusList.Add(new SelectListItem { Text = "All Proposals", Value = "%" });
            StatusList.Add(new SelectListItem { Text = "Scrutinized", Value = "Y" });
            StatusList.Add(new SelectListItem { Text = "Yet to be Scrutinized", Value = "N" });
            Status = "%";
            ReportTypeList = new List<SelectListItem>();
            ReportTypeList.Add(new SelectListItem { Text = "Road wise", Value = "R" });
            ReportTypeList.Add(new SelectListItem { Text = "Habitation wise ", Value = "H" });
            ReportType = "R";
            RoadWise = false;


        }

        public int LevelCode { get; set; }
        public string StateName { get; set; }
        public string DistName { get; set; }
        public string BlockName { get; set; }
        public int Mast_State_Code { get; set; }
        public int Mast_Block_Code { get; set; }
        public int Mast_District_Code { get; set; }
        public string FundingAgencyName { get; set; }
        public string StatusName { get; set; }
        public string BatchName { get; set; }
        public string YearName { get; set; }

        [Display(Name = "State")]
        [Required(ErrorMessage = "Please select State. ")]
        [Range(0, int.MaxValue, ErrorMessage = "Please select  State.")]
        public int StateCode { get; set; }
        public List<SelectListItem> StateList { get; set; }

        [Display(Name = "District")]
        [Required(ErrorMessage = "Please select District.")]
        [Range(0, int.MaxValue, ErrorMessage = "Please select District.")]
        public int DistrictCode { get; set; }
        public List<SelectListItem> DistrictList { get; set; }

        [Display(Name = "Block")]
        [Required(ErrorMessage = "Please select Block.")]
        [Range(0, int.MaxValue, ErrorMessage = "Please select Block.")]
        public int BlockCode { get; set; }
        public List<SelectListItem> BlockList { get; set; }

        [Display(Name = "Collaboration")]
        [Range(0, int.MaxValue, ErrorMessage = "Please select Collaboration.")]
        [Required(ErrorMessage = "Please select Collaboration.")]
        [RegularExpression(@"^([0-9]+)$", ErrorMessage = "Collaboration must be valid number.")]
        public int FundingAgency { get; set; }
        public List<SelectListItem> FundingAgencyList { get; set; }

        [Display(Name = "Year")]
        [Range(0, 2090, ErrorMessage = "Please select Phase.")]
        [Required(ErrorMessage = "Please select Phase.")]
        [RegularExpression("^([0-9]+)$", ErrorMessage = "Year must be valid number.")]
        public int PhaseYear { get; set; }
        public List<SelectListItem> PhaseYearList { get; set; }
        // public  SelectList PhaseYearList { get; set; }

        [Display(Name = "Batch")]
        [Range(0, 10, ErrorMessage = "Please select Batch.")]
        [Required(ErrorMessage = "Please select Batch.")]
        [RegularExpression("^([0-9]+)$", ErrorMessage = "Batch must be valid number.")]
        public int Batch { get; set; }
        public List<SelectListItem> BatchList { get; set; }

        [Display(Name = "STA Status")]
        public string Status { get; set; }
        public List<SelectListItem> StatusList { get; set; }
        [Display(Name = "Report Type")]
        public string ReportType { get; set; }
        public List<SelectListItem> ReportTypeList { get; set; }

        [Display(Name = "Roadwise")]
        public bool RoadWise { get; set; }


    }
}