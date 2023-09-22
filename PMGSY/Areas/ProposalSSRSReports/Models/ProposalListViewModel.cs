using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.ComponentModel.DataAnnotations;

namespace PMGSY.Areas.ProposalSSRSReports.Models
{
    public class ProposalListViewModel
    {
        [Display(Name = "Year")]
        [Range(0, int.MaxValue, ErrorMessage = "Please select Year.")]
        public int Year { get; set; }
        public List<SelectListItem> YearList { get; set; }

        [Display(Name = "Batch")]
        [Range(0, int.MaxValue, ErrorMessage = "Please select Batch.")]
        public int Batch { get; set; }
        public List<SelectListItem> BatchList { get; set; }

        [Display(Name = "Collaboration")]
        [Range(0, int.MaxValue, ErrorMessage = "Please select Collaboration.")]
        public int Collaboration { get; set; }
        public List<SelectListItem> CollabList { get; set; }

        [Display(Name = "Status")]
        //[Range(0, int.MaxValue, ErrorMessage = "Please select Status.")]
        [RegularExpression(@"^([NYURDSA]+)$", ErrorMessage = "Invalid Status selected")]
        public string Status { get; set; }
        public List<SelectListItem> StatusList { get; set; }

        public int Level { get; set; }
        public int State { get; set; }
        public int District { get; set; }
        public int Block { get; set; }
        public int PMGSYScheme { get; set; }

        public string StateName { get; set; }
        public string DistrictName { get; set; }
        public string BlockName { get; set; }
        public string CollabName { get; set; }
        public string StatusName { get; set; }
    }

    public class ProposalScrutinyViewModel
    {
        //public ProposalReportsModel()
        //{
        //    STATE_NAME = PMGSYSession.Current.StateCode == 0 ? string.Empty : PMGSYSession.Current.StateName;
        //    DISTRICT_NAME = PMGSYSession.Current.DistrictCode == 0 ? string.Empty : PMGSYSession.Current.DistrictName;
        //    BLOCK_NAME = string.Empty;

        //}

        public string STATE_NAME { get; set; }
        public string DISTRICT_NAME { get; set; }
        public string BLOCK_NAME { get; set; }


        //[Display(Name = "State")]
        //public Nullable<int> MAST_STATE_CODE { get; set; }
        //public List<SelectListItem> STATES { get; set; }

        //[Display(Name = "District")]
        //public Nullable<int> MAST_DISTRICT_CODE { get; set; }
        //public List<SelectListItem> DISTRICTS { get; set; }

        //[Display(Name = "Block")]
        //public int MAST_BLOCK_CODE { get; set; }
        //public List<SelectListItem> BLOCKS { get; set; }

        [Display(Name = "State")]
        [Range(0, int.MaxValue, ErrorMessage = "Please select State.")]
        public int State { get; set; }
        public List<SelectListItem> StateList { get; set; }

        [Display(Name = "District")]
        [Range(0, int.MaxValue, ErrorMessage = "Please select District.")]
        public int District { get; set; }
        public List<SelectListItem> DistrictList { get; set; }

        [Display(Name = "Type")]
        [RegularExpression(@"^([LP]+)$", ErrorMessage = "Invalid Type selected")]
        public string Type { get; set; }
        public List<SelectListItem> TypeList { get; set; }

        [Display(Name = "Type")]
        [RegularExpression(@"^([SP]+)$", ErrorMessage = "Invalid Type selected")]
        public string TechType { get; set; }
        public List<SelectListItem> TechTypeList { get; set; }

        [Display(Name = "Agency")]
        [Range(0, int.MaxValue, ErrorMessage = "Please select Agency.")]
        public int Agency { get; set; }
        public List<SelectListItem> AgencyList { get; set; }

        [Display(Name = "Year")]
        [Range(0, int.MaxValue, ErrorMessage = "Please select Year.")]
        public int Year { get; set; }
        public List<SelectListItem> YearList { get; set; }

        [Display(Name = "Batch")]
        [Range(0, int.MaxValue, ErrorMessage = "Please select Batch.")]
        public int Batch { get; set; }
        public List<SelectListItem> BatchList { get; set; }

        [Display(Name = "Scheme")]
        [Range(0, int.MaxValue, ErrorMessage = "Please select Scheme.")]
        public int Collaboration { get; set; }
        public List<SelectListItem> CollabList { get; set; }

        public int PMGSYScheme { get; set; }

        public string StateName { get; set; }
        public string DistrictName { get; set; }
        public string BatchName { get; set; }
        public string CollabName { get; set; }
        public string AgencyName { get; set; }

        public string TAName { get; set; }
    }

    public class ProposalAnalysisViewModel
    {
        //public ProposalReportsModel()
        //{
        //    STATE_NAME = PMGSYSession.Current.StateCode == 0 ? string.Empty : PMGSYSession.Current.StateName;
        //    DISTRICT_NAME = PMGSYSession.Current.DistrictCode == 0 ? string.Empty : PMGSYSession.Current.DistrictName;
        //    BLOCK_NAME = string.Empty;

        //}

        [Display(Name = "State")]
        [Range(0, int.MaxValue, ErrorMessage = "Please select State.")]
        public int State { get; set; }
        public List<SelectListItem> StateList { get; set; }

        [Display(Name = "Scrutiny")]
        [RegularExpression(@"^([%NY]+)$", ErrorMessage = "Invalid Scrutiny selected")]
        public string Scrutiny { get; set; }
        public List<SelectListItem> ScrutinyList { get; set; }

        [Display(Name = "Sanctioned")]
        [RegularExpression(@"^([%NY]+)$", ErrorMessage = "Invalid Sanctioned Type selected")]
        public string Sanctioned { get; set; }
        public List<SelectListItem> SanctionedList { get; set; }

        [Display(Name = "Proposal")]
        [RegularExpression(@"^([PL]+)$", ErrorMessage = "Invalid Proposal Type selected")]
        public string Proposal { get; set; }
        public List<SelectListItem> ProposalList { get; set; }

        public string StateName { get; set; }
        public string ScrutinyName { get; set; }
        public string SanctionName { get; set; }
        public string ProposalType { get; set; }
    }

    public class PendingWorksViewModel
    {
        [Display(Name = "State")]
        [Range(0, int.MaxValue, ErrorMessage = "Please select State.")]
        public int State { get; set; }
        public List<SelectListItem> StateList { get; set; }

        [Display(Name = "Reason")]
        [RegularExpression(@"^([%ALF]+)$", ErrorMessage = "Invalid Reason selected")]
        public string Reason { get; set; }
        public List<SelectListItem> ReasonList { get; set; }

        public string StateName { get; set; }
        public string ReasonName { get; set; }
    }

    public class PCIAnalysisViewModel
    {
        [Display(Name = "Route")]
        [RegularExpression(@"^([%LT]+)$", ErrorMessage = "Invalid Route selected")]
        public string Route { get; set; }
        public List<SelectListItem> RouteList { get; set; }


        public int State { get; set; }
        public int District { get; set; }
        public int Block { get; set; }

        public string flag { get; set; }
        public string StateName { get; set; }
        public string DistName { get; set; }
        public string BlockName { get; set; }
        public string RouteName { get; set; }
    }

    public class ExecutionFinancialProgressViewModel
    {
        [Display(Name = "State")]
        [Range(0, int.MaxValue, ErrorMessage = "Please select State.")]
        public int State { get; set; }
        public List<SelectListItem> StateList { get; set; }

        [Display(Name = "District")]
        [Range(0, int.MaxValue, ErrorMessage = "Please select District.")]
        public int District { get; set; }
        public List<SelectListItem> DistrictList { get; set; }

        [Display(Name = "Block")]
        [Range(0, int.MaxValue, ErrorMessage = "Please select Block.")]
        public int Block { get; set; }
        public List<SelectListItem> BlockList { get; set; }

        [Display(Name = "Type")]
        [RegularExpression(@"^([%SP]+)$", ErrorMessage = "Invalid Type selected")]
        public string Type { get; set; }
        public List<SelectListItem> TypeList { get; set; }

        [Display(Name = "Sanction Year")]
        [Range(0, int.MaxValue, ErrorMessage = "Please select Year.")]
        public int Year { get; set; }
        public List<SelectListItem> YearList { get; set; }

        [Display(Name = "Batch")]
        [Range(0, int.MaxValue, ErrorMessage = "Please select Batch.")]
        public int Batch { get; set; }
        public List<SelectListItem> BatchList { get; set; }

        [Display(Name = "Collaboration")]
        [Range(0, int.MaxValue, ErrorMessage = "Please select Collaboration.")]
        public int Collaboration { get; set; }
        public List<SelectListItem> CollabList { get; set; }

        public string Progress { get; set; }
        public string StateName { get; set; }
        public string DistName { get; set; }
        public string BlockName { get; set; }
        public string CollabName { get; set; }
    }

    public class MaintenanceAgreementViewModel
    {
        [Display(Name = "State")]
        [Range(0, int.MaxValue, ErrorMessage = "Please select State.")]
        public int State { get; set; }
        public List<SelectListItem> StateList { get; set; }

        [Display(Name = "District")]
        [Range(0, int.MaxValue, ErrorMessage = "Please select District.")]
        public int District { get; set; }
        public List<SelectListItem> DistrictList { get; set; }

        [Display(Name = "Block")]
        [Range(0, int.MaxValue, ErrorMessage = "Please select Block.")]
        public int Block { get; set; }
        public List<SelectListItem> BlockList { get; set; }

        [Display(Name = "Status")]
        [RegularExpression(@"^([%NDYUR]+)$", ErrorMessage = "Invalid Status selected")]
        public string Status { get; set; }
        public List<SelectListItem> StatusList { get; set; }

        [Display(Name = "Year")]
        [Range(0, int.MaxValue, ErrorMessage = "Please select Year.")]
        public int Year { get; set; }
        public List<SelectListItem> YearList { get; set; }

        [Display(Name = "Batch")]
        [Range(0, int.MaxValue, ErrorMessage = "Please select Batch.")]
        public int Batch { get; set; }
        public List<SelectListItem> BatchList { get; set; }

        [Display(Name = "Collaboration")]
        [Range(0, int.MaxValue, ErrorMessage = "Please select Collaboration.")]
        public int Collaboration { get; set; }
        public List<SelectListItem> CollabList { get; set; }

        public string StatusName { get; set; }
        public string StateName { get; set; }
        public string DistName { get; set; }
        public string BlockName { get; set; }
        public string CollabName { get; set; }
    }

    public class FundSanctionReleaseViewModel
    {
        [Display(Name = "State")]
        [Range(0, int.MaxValue, ErrorMessage = "Please select State.")]
        public int State { get; set; }
        public List<SelectListItem> StateList { get; set; }

        [Display(Name = "District")]
        [Range(0, int.MaxValue, ErrorMessage = "Please select District.")]
        public int District { get; set; }
        public List<SelectListItem> DistrictList { get; set; }

        [Display(Name = "Block")]
        [Range(0, int.MaxValue, ErrorMessage = "Please select Block.")]
        public int Block { get; set; }
        public List<SelectListItem> BlockList { get; set; }

        [Display(Name = "Type")]
        [RegularExpression(@"^([%SP]+)$", ErrorMessage = "Invalid Type selected")]
        public string Type { get; set; }
        public List<SelectListItem> TypeList { get; set; }

        [Display(Name = "Year")]
        [Range(0, int.MaxValue, ErrorMessage = "Please select Year.")]
        public int Year { get; set; }
        public List<SelectListItem> YearList { get; set; }

        [Display(Name = "Batch")]
        [Range(0, int.MaxValue, ErrorMessage = "Please select Batch.")]
        public int Batch { get; set; }
        public List<SelectListItem> BatchList { get; set; }

        [Display(Name = "Collaboration")]
        [Range(0, int.MaxValue, ErrorMessage = "Please select Collaboration.")]
        public int Collaboration { get; set; }
        public List<SelectListItem> CollabList { get; set; }

        public string Progress { get; set; }
        public string StateName { get; set; }
        public string DistName { get; set; }
        public string BlockName { get; set; }
        public string CollabName { get; set; }
    }

    public class PropAnalysisViewModel
    {
        //public ProposalReportsModel()
        //{
        //    STATE_NAME = PMGSYSession.Current.StateCode == 0 ? string.Empty : PMGSYSession.Current.StateName;
        //    DISTRICT_NAME = PMGSYSession.Current.DistrictCode == 0 ? string.Empty : PMGSYSession.Current.DistrictName;
        //    BLOCK_NAME = string.Empty;

        //}

        public string STATE_NAME { get; set; }
        public string DISTRICT_NAME { get; set; }
        public string BLOCK_NAME { get; set; }


        [Display(Name = "State")]
        public Nullable<int> MAST_STATE_CODE { get; set; }
        public List<SelectListItem> STATES { get; set; }

        [Display(Name = "District")]
        public Nullable<int> MAST_DISTRICT_CODE { get; set; }
        public List<SelectListItem> DISTRICTS { get; set; }

        [Display(Name = "Block")]
        public int MAST_BLOCK_CODE { get; set; }
        public List<SelectListItem> BLOCKS { get; set; }
    }

    public class TechnologyViewModel
    {
        [Display(Name = "State")]
        [Range(0, int.MaxValue, ErrorMessage = "Please select valid State.")]
        public int State { get; set; }
        public List<SelectListItem> StateList { get; set; }

        [Display(Name = "District")]
        [Range(0, int.MaxValue, ErrorMessage = "Please select valid District.")]
        public int District { get; set; }
        public List<SelectListItem> DistrictList { get; set; }

        [Display(Name = "Block")]
        [Range(0, int.MaxValue, ErrorMessage = "Please select valid Block.")]
        public int Block { get; set; }
        public List<SelectListItem> BlockList { get; set; }

        [Display(Name = "Year")]
        [Range(0, int.MaxValue, ErrorMessage = "Please select valid Year.")]
        public int Year { get; set; }
        public List<SelectListItem> YearList { get; set; }

        [Display(Name = "Accredited Type")]
        [RegularExpression(@"^([%AEN]+)$", ErrorMessage = "Invalid Accredited Type selected")]
        public string AggregatedType { get; set; }
        public List<SelectListItem> AggregatedTypeList { get; set; }

        [Display(Name = "STA Status")]
        [RegularExpression(@"^([%YN]+)$", ErrorMessage = "Invalid STA Status selected")]
        public string STAStatus { get; set; }
        public List<SelectListItem> STAStatusList { get; set; }

        [Display(Name = "MRD Status")]
        [RegularExpression(@"^([%YN]+)$", ErrorMessage = "Invalid MRD Status selected")]
        public string MRDStatus { get; set; }
        public List<SelectListItem> MRDStatusList { get; set; }

        [Display(Name = "Connectivity")]
        [RegularExpression(@"^([%NU]+)$", ErrorMessage = "Invalid Connectivity selected")]
        public string Connectivity { get; set; }
        public List<SelectListItem> ConnectivityList { get; set; }

        [Display(Name = "Report Type")]
        [RegularExpression(@"^([ST]+)$", ErrorMessage = "Invalid Report Type selected")]
        public string ReportType { get; set; }
        public List<SelectListItem> ReportTypeList { get; set; }

        [Display(Name = "Batch")]
        [Range(0, int.MaxValue, ErrorMessage = "Please select valid Batch.")]
        public int Batch { get; set; }
        public List<SelectListItem> BatchList { get; set; }

        [Display(Name = "Collaboration")]
        [Range(0, int.MaxValue, ErrorMessage = "Please select valid Collaboration.")]
        public int Collaboration { get; set; }
        public List<SelectListItem> CollabList { get; set; }

        public string StateName { get; set; }
        public string DistName { get; set; }
        public string BlockName { get; set; }
        public string BatchName { get; set; }
        public string CollabName { get; set; }
        public string YearName { get; set; }

        public int Mast_State_Code { get; set; }
        public int Mast_District_Code { get; set; }
        public int Mast_Block_Code { get; set; }

        [Display(Name = "Technology")]
        [Range(0, int.MaxValue, ErrorMessage = "Please select valid Technology.")]
        public int Technology { get; set; }
        public List<SelectListItem> TechList { get; set; }

        public string TechName { get; set; }
    }
}