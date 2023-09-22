#region File Header
/*
        * Project Id    :
        * Project Name  :   OMMAS II
        * Name          :   QMPhaseInspectionViewModel.cs        
        * Description   :   All filters will be loaded using this model
        * Author        :   Rohit Jadhav.
        * Creation Date :   01/Sept/2014
 **/
#endregion


using PMGSY.Common;
using PMGSY.Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PMGSY.Models.QualityMonitoring
{
    public class QMPhaseInspectionViewModel
    {
        public QMPhaseInspectionViewModel()
        {
            CommonFunctions commonFunctions = new CommonFunctions();

            StateList = new List<SelectListItem>();
            DistrictList = new List<SelectListItem>();
            BlockList = new List<SelectListItem>();
            AgencyList = new List<SelectListItem>();

            StateName = PMGSYSession.Current.StateCode == 0 ? "0" : PMGSYSession.Current.StateName.Trim();
            DistName = PMGSYSession.Current.DistrictCode == 0 ? "0" : PMGSYSession.Current.DistrictName.Trim();

            Mast_State_Code = PMGSYSession.Current.StateCode;
            Mast_District_Code = PMGSYSession.Current.DistrictCode;

            StateList = commonFunctions.PopulateStates(true);
            
            StateCode = PMGSYSession.Current.StateCode == 0 ? 0 : PMGSYSession.Current.StateCode;
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
            }

            AgencyList = new List<SelectListItem>();
            if (StateCode == 0)
            {
                AgencyList.Insert(0, (new SelectListItem {Text="All Agencies",Value="0",Selected=true }));
            
            }
            else
            {
                AgencyList = commonFunctions.PopulateAgencies(StateCode, true);
                //AgencyList.Find(x => x.Value == "-1").Value = "0";
            }


            //FundingAgencyList = commonFunctions.PopulateFundingAgency(true);
            //FundingAgencyList.Find(x => x.Value == "-1").Value = "0";

            Level = 1;
            YearList = commonFunctions.PopulateFinancialYear(true, true).ToList();
            YearList.Find(x => x.Value == "0").Selected = true;

            qmTypeList = new List<SelectListItem>();
            qmTypeList = commonFunctions.PopulateMonitorTypes();

            RoadStatusList = new List<SelectListItem>();
            RoadStatusList.Insert(0, (new SelectListItem { Text = "All", Value = "0", Selected = true }));
            RoadStatusList.Insert(1, (new SelectListItem { Text = "Completed", Value = "C" }));
            RoadStatusList.Insert(2, (new SelectListItem { Text = "In Progress", Value = "P" }));

            StatusList = new List<SelectListItem>();
            StatusList.Insert(0, (new SelectListItem { Text = "TOTAL WORK SANCTIONED", Value = "0", Selected = true }));
            StatusList.Insert(1, (new SelectListItem { Text = "TOTAL AWARDED PROGRESS WORK", Value = "1" }));
            StatusList.Insert(2, (new SelectListItem { Text = "TOTAL UNAWARDED PROGRESS WORK", Value = "2" }));
        }

        public string StateName { get; set; }
        public string DistName { get; set; }
        public string BlockName { get; set; }
        public string FundingAgencyName { get; set; }
        public string AgencyName { get; set;}
        public int Mast_State_Code { get; set; }
        public int Mast_Block_Code { get; set; }
        public int Mast_District_Code { get; set; }
        public int Mast_Funding_Agency_Code { get; set; }
        public int Mast_Agency_Code { get; set; }

     
        [Display(Name = "State")]
        [Required(ErrorMessage = "Please select State. ")]
        [Range(1, int.MaxValue, ErrorMessage = "Please select  State.")]
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
        public int FundingAgencyCode { get; set; }
        public List<SelectListItem> FundingAgencyList { get; set; }

        [Display(Name = "Agency")]
        [Range(0, int.MaxValue, ErrorMessage = "Please select Agency.")]
        [Required(ErrorMessage = "Please select Agency.")]
        public int AgencyCode { get; set;}
        public List<SelectListItem> AgencyList { get; set;}

        public int Level { get; set; }

        [Display(Name = "Quality Monitor Type")]
        [RegularExpression(@"^([SI]+)$", ErrorMessage = "Invalid QM Type selected")]
        public string qmType { get; set; }
        public List<SelectListItem> qmTypeList { get; set; }

        [Display(Name = "Road Status")]
        [RegularExpression(@"^([0CP]+)$", ErrorMessage = "Invalid Road Status selected")]
        public string RoadStatus { get; set; }
        public List<SelectListItem> RoadStatusList { get; set; }

        [Display(Name = "Status")]
        [RegularExpression(@"^([012]+)$", ErrorMessage = "Invalid Status selected")]
        public string Status { get; set; }
        public List<SelectListItem> StatusList { get; set; }

        [Display(Name = "Year")]
        [Range(0, int.MaxValue, ErrorMessage = "Please select valid Sanction Year.")]
        public int Year { get; set; }
        public List<SelectListItem> YearList { get; set; }
    }
}