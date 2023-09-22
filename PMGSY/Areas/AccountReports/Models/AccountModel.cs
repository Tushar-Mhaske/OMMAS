using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PMGSY.Common;
using PMGSY.Extensions;
using PMGSY.Models;
using PMGSY.Models.Report;

namespace PMGSY.Areas.AccountReports.Models
{
    public class AccountModel
    {

    }

    public class FPPModel
    {

        public FPPModel()
        {
          
            lstFundingAgency = new List<SelectListItem>();
            lstPeriod = new List<SelectListItem>();
      
        }

        public List<SelectListItem> lstPeriod { get; set; }
        [Display(Name = "Periodic Value")]
        public int PeriodCode { get; set; }


        public List<SelectListItem> lstFundingAgency { get; set; }
        [Display(Name = "Funding Agency")]
        public int FundingAgencyCode { get; set; }


        [Display(Name="State")]
        //[Range(1, int.MaxValue, ErrorMessage = "Please select State")]
        [RegularExpression(@"^([0-9]+)$", ErrorMessage = "Please select State.")]
        public int StateCode { get; set; }
        public List<SelectListItem> lstStates { get; set; }

        //[Display(Name = "Agency")]
        //[LocalizedDisplayName("lblAgency")]
        [Display(Name="Agency")]
        [RegularExpression(@"^([0-9]+)$", ErrorMessage = "Agency should contains digits only.")]
        //[Range(1, int.MaxValue, ErrorMessage = "Please select Agency")]
        public int Agency { get; set; }
        public List<SelectListItem> lstAgency { get; set; }



        [DisplayName("Select Year")]
        [Range(0, short.MaxValue, ErrorMessage = "Please Select Year")]
        public short Year { get; set; }
        public List<SelectListItem> ListYear { get; set; }



        

        public int levelId { get; set; }
      


        public string DisplayStateName { get; set; }
        public string DisplayAgencyName { get; set; }
        public string FundingAgencyName { get; set; }
        public string PeriodicName { get; set; }


    }

    public class UtilizationCertificate
    {


        [Range(1, 2147483647, ErrorMessage = "Please select state.")]
        [RegularExpression("^([0-9]+)$", ErrorMessage = "State must be valid number.")]
        [Display(Name = "State : ")]
        public int StateCode { get; set; }
        public List<SelectListItem> lstStates { get; set; }
        public String StateName { get; set; }

        public int LevelID { get; set; }



        [Range(1, 2147483647, ErrorMessage = "Please select Agency.")]
        [Display(Name = "Agency : ")]
        public int AgencyCode { get; set; }
        public List<SelectListItem> lstAgency { get; set; }
        public String AgencyName { get; set; }



        [Range(1, 2147483647, ErrorMessage = "Please select Financial Year.")]
        [Display(Name = "Financial Year : ")]
        public int YearCode { get; set; }
        public List<SelectListItem> lstYear { get; set; }
        public String YearName { get; set; }







    }
    public class BalanceSheet
    {
        public BalanceSheet()
        {
            // new change done on 29-Aug-2013
            DPIUList = new List<SelectListItem>();
            //new change done in absence of vss by Vikram
            NodalAgencyList = new List<SelectListItem>();

            MaintTypeList = new List<SelectListItem>();
        }

        [Display(Name = "Month")]
        public short Month { get; set; }
        public List<SelectListItem> MonthList { get; set; }

        //Report Id
        public int PINTRptID { get; set; }

        [Display(Name = "Fund Type")]
        [Required(ErrorMessage = "Please Select Fund Type")]
        [RegularExpression("[PAM]", ErrorMessage = "Please Select Fund Type")]
        public string FundType { get; set; }
        public List<SelectListItem> lstFundType { get; set; }

        [Display(Name = "Year")]
        [Range(1, short.MaxValue, ErrorMessage = "Please Select Year")]
        public short Year { get; set; }
        public List<SelectListItem> YearList { get; set; }

        // O (srrda and dpiu) , S--STATE , A-all piu
        public char ReportLevel { get; set; }

        public int IsAllPiu { get; set; }
        public int levelId { get; set; }


        //new change done on 29-Aug-2013
        //Dpiu Nd Code
        public int? AdminCode { get; set; }
        public List<SelectListItem> DPIUList { get; set; }

        //new change done in absence of vss by Vikram
        public List<SelectListItem> NodalAgencyList { get; set; } //list for population of Nodal Agency when MRD is in login

        public List<SelectListItem> MaintTypeList { get; set; }

        public int maintTypeCode { get; set; }

        //following parameters are used at MORD level
        public int NodalAdminCode { get; set; }
        public int StateAdminCode { get; set; }
        public int AllDPIUAdminCode { get; set; }

        //Added By Abhishek kamble 24-dec-2013
        public int? HeadCode { get; set; }
        public string HeadName { get; set; }

        public int SelectedMonth { get; set; }
        public int SelectedYear { get; set; }

    }

    public class MonthlyAccountModel
    {
        [DisplayName("Month")]
        [Range(1, 12, ErrorMessage = "Please Select month")]
        public Int16 Month { get; set; }

        [DisplayName("Year")]
        [Range(2000, Int32.MaxValue, ErrorMessage = "Please Select Year")]
        public Int16 Year { get; set; }


        [DisplayName("Balance")]
        [RegularExpression("[CD]", ErrorMessage = "Please Select Balance")]
        public String CreditDebit { get; set; }

        [DisplayName("Nodal Agency")]
        public String NodalAgency { get; set; }

        [Display(Name = "Fund Type")]
        [Required(ErrorMessage = "Please Select Fund Type")]
        [RegularExpression("[PAM]", ErrorMessage = "Please Select Fund Type")]
        public string FundType { get; set; }
        public List<SelectListItem> lstFundType { get; set; }

        public string monthlyStateSrrdaDpiu { get; set; }

        [DisplayName("STATE")]
        public Int32 State { get; set; }
        public List<SelectListItem> lstStates { get; set; }

        [DisplayName("SRRDA")]
        public Int16 Srrda { get; set; }
        [DisplayName("DPIU")]
        public Int16 Dpiu { get; set; }

        public int levelId { get; set; }
        public int AdminNdCode { get; set; }

        //Added 15-May-2014
        public int ISSelf { get; set; }
        public int AllPIU { get; set; }
    }


    public class CBSingleModel
    {
        [Display(Name = "Month")]
        [Required(ErrorMessage = "Please select month.")]
        [Range(1, 12, ErrorMessage = "Please select month.")]
        public Int16 Month { get; set; }

        [Display(Name = "Year")]
        [Required(ErrorMessage = "Please select year.")]
        [Range(1990, 2099, ErrorMessage = "Please select year.")]
        public Int16 Year { get; set; }

        //Radio Button
        public string SRRDA_DPIU { get; set; }

        [Display(Name = "SRRDA")]
        public int SRRDA { get; set; }

        [Display(Name = "DPIU")]
        //[Range(1,int.MaxValue,ErrorMessage="Please Select DPIU")]
        public int DPIU { get; set; }

        public int ADMIN_ND_CODE { get; set; }
        public int LvlId { get; set; }

        public string CashbookType { get; set; }
    }
    //validation for DPIU Required start
    //public class IsDPIURequiredAttribute : ValidationAttribute, IClientValidatable
    //{
    //    //S-SRRDA, D-DPIU
    //    private string SrrdaDpiu;

    //    public IsDPIURequiredAttribute(String srrdaDpiu)
    //    {
    //        this.SrrdaDpiu = srrdaDpiu;
    //    }
    //    //Override DefaultFormatErrorMessage Method
    //    public override String FormatErrorMessage(string name)
    //    {
    //        return string.Format(ErrorMessageString, SrrdaDpiu);
    //    }

    //    //Override Is Valid
    //    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    //    {
    //        //Get Property Context Object
    //        var SRRDA_DPIU = validationContext.ObjectType.GetProperty(this.SrrdaDpiu);

    //        var DPIUNdCode = value;

    //        return base.IsValid(value, validationContext);
    //    }

    //}
    //validation for DPIU Required end

    public class LedgerModel
    {
        public LedgerModel()
        {
            this.YEAR = Convert.ToInt16(DateTime.Now.Year);
            this.MONTH = Convert.ToInt16(DateTime.Now.Month);
        }

        [Display(Name = "Month")]
        [Range(1, Int16.MaxValue, ErrorMessage = "Please Select  Month")]
        public short MONTH { get; set; }
        public List<SelectListItem> YEAR_LIST { get; set; }

        [Display(Name = "Year")]
        [Range(1, Int16.MaxValue, ErrorMessage = "Please Select Year")]
        public short YEAR { get; set; }
        public List<SelectListItem> MONTH_LIST { get; set; }

        [Required(ErrorMessage = "Credit / Debit required ")]
        [MaxLength(1, ErrorMessage = "Invalid Credit/debit")]
        public string CREDIT_DEBIT { get; set; }

        [Display(Name = "Head")]
        [Required(ErrorMessage = "Head is Required")]
        [Range(1, Int16.MaxValue, ErrorMessage = "Please Select Head")]
        public String HEAD { get; set; }
        public List<SelectListItem> HEAD_LIST { get; set; }

        [Display(Name = "DPIU")]
        [Range(-1, Int16.MaxValue, ErrorMessage = "Please Select DPIU")]
        public short DPIU { get; set; }
        public List<SelectListItem> PIU_LIST { get; set; }


        public int SRRDA { get; set; }
        public string SRRDA_DPIU { get; set; }
        public int DPIULevel { get; set; }


        public int SelectedHead { get; set; }
        public bool isPiuLedger { get; set; }
        public short levelId { get; set; }
        public string RoadStatus { get; set; }
        public string FundType { get; set; }
        public String HeadDetails { get; set; }
        //C-Completed,L-Ledger Details,D-DPIU wise details.
        public String ReportType { get; set; }
        public int AdminNdCode { get; set; }
        public int HeadCode { get; set; }

        public String DistrictDepartment { get; set; }
        public string StateDepartment { get; set; }
    }

    /// <summary>
    /// Model used for report 1) Fund Received 2) Deposite Repayable
    /// </summary>
    public class SheduleReportModel
    {
        [DisplayName("Select Month")]
        //[Range(1, 12, ErrorMessage = "Please Select Month")]
        [RequiredMonth("ReportType", ErrorMessage = "Please select Month")]
        public short Month { get; set; }
        public List<SelectListItem> ListMonth { get; set; }

        [DisplayName("Select Year")]
        [Range(2000, short.MaxValue, ErrorMessage = "Please Select Year")]
        public short Year { get; set; }
        public List<SelectListItem> ListYear { get; set; }

        [DisplayName("Select PIU")]
        public int Piu { get; set; }
        public List<SelectListItem> ListPiu { get; set; }

        [DisplayName("Select Agency")]
        [Range(0, int.MaxValue, ErrorMessage = "Please Select Agency")]
        public int Agency { get; set; }
        public List<SelectListItem> ListAgency { get; set; }

        public int LevelId { get; set; }
        public int ReportType { get; set; }
        public int ReportLevel { get; set; }
        public int ReportID { get; set; }

        public string FundType { get; set; }
        public string SRRDA_DPIU { get; set; }
        public int? SrrdaNdCode { get; set; }
        public string RoadReportType { get; set; }

        //Report Header  Used in report (Incidental Fund)
        public string FormNumber { get; set; }
        public string ReportHeader { get; set; }
        public string Refference { get; set; }
        public string ScheduleNumber { get; set; }
    }


    public class ScheduleRoadModel
    {
        [DisplayName("Select Month")]
        [Range(1, 12, ErrorMessage = "Please Select Month")]
        public short Month { get; set; }
        public List<SelectListItem> ListMonth { get; set; }

        [DisplayName("Select Year")]
        [Range(2000, short.MaxValue, ErrorMessage = "Please Select Year")]
        public short Year { get; set; }
        public List<SelectListItem> ListYear { get; set; }

        [DisplayName("Select PIU")]
        //Added By Abhishek Kamble 2-dec-2013
        [Range(1, Int32.MaxValue, ErrorMessage = "Please select DPIU")]
        public int Piu { get; set; }
        public List<SelectListItem> ListPiu { get; set; }

        [DisplayName("Select State")]
        public int State { get; set; }
        public List<SelectListItem> ListState { get; set; }

        [DisplayName("Funding Agency")]
        public int FundingAgency { get; set; }
        public List<MASTER_FUNDING_AGENCY> lstFundingAgency { get; set; }

        [DisplayName("Head")]
        [Range(1, Int16.MaxValue, ErrorMessage = "Please Select Head")]
        public int HeadCode { get; set; }
        public List<ACC_MASTER_HEAD> lstHead { get; set; }

        //public List<USP_ACC_RPT_SCHEDULE_OF_ROADS_Result> lstSchedule { get; set; }
        public int LevelId { get; set; }
        public string FundType { get; set; }

        //public string Header { get; set; }
        //public string FormNumber { get; set; }
        //public string Paragraph1 { get; set; }
        //public string Paragraph2 { get; set; }
        //public bool IsYearly { get; set; }
        //public bool IsSrrdaPiu { get; set; }
        //public string PiuName { get; set; }
        public string HeadName { get; set; }
        //public string YearName { get; set; }
        public string AgencyName { get; set; }
        //public string MonthName { get; set; }
        //public string StateName { get; set; }
        //public string ScheduleName { get; set; }

        //added by abhishek kamlbe 20-dec-2013 
        public int ReportType { get; set; }
        public int ReportLevel { get; set; }
        public string ReportName { get; set; }

        public string RoadReportType { get; set; }
        public int SRRDANdCode { get; set; }

    }


    public class RequiredMonthAttribute : ValidationAttribute, IClientValidatable
    {
        // private const string _defaultErrorMessage = "Start date must be less than end date.";
        private string _basePropertyName;

        public RequiredMonthAttribute(string basePropertyName)
        //: base(_defaultErrorMessage)
        {
            _basePropertyName = basePropertyName;
        }

        //Override default FormatErrorMessage Method  
        public override string FormatErrorMessage(string name)
        {
            return string.Format(ErrorMessageString, name, _basePropertyName);
        }

        //Override IsValid  
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            //Get PropertyInfo Object  
            var basePropertyInfo = validationContext.ObjectType.GetProperty(_basePropertyName);

            var IsReportLevelMonthly = basePropertyInfo.GetValue(validationContext.ObjectInstance, null);
            //int month = (int)value;

            var month = value;

            //if (sDate != null && eDate != null)
            //{

            //Actual Validation 
            if ((Convert.ToInt32(IsReportLevelMonthly) == 2) && (Convert.ToInt32(month) == 0))
            {
                var message = FormatErrorMessage(validationContext.DisplayName);
                return new ValidationResult(message);
            }

            return ValidationResult.Success;
        }
        public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
        {
            var rule = new ModelClientValidationRule
            {
                ErrorMessage = FormatErrorMessage(metadata.DisplayName),
                ValidationType = "monthvalidator"
            };
            rule.ValidationParameters["month"] = this._basePropertyName;
            yield return rule;
        }
    }


    #region MaintenanceFundReport


    public class ScheduleOfRoadMFModel
    {
        [DisplayName("Select Month")]
        [Range(1, 12, ErrorMessage = "Please Select Month")]
        public short Month { get; set; }
        public List<SelectListItem> ListMonth { get; set; }

        [DisplayName("Select Year")]
        [Range(2000, short.MaxValue, ErrorMessage = "Please Select Year")]
        public short Year { get; set; }
        public List<SelectListItem> ListYear { get; set; }

        [DisplayName("Select PIU")]
        [Range(1, Int32.MaxValue, ErrorMessage = "Please select DPIU")]
        public int Piu { get; set; }
        public List<SelectListItem> ListPiu { get; set; }

        [DisplayName("Select State")]
        public int State { get; set; }
        public List<SelectListItem> ListState { get; set; }

        [DisplayName("Head")]
        [Range(1, Int16.MaxValue, ErrorMessage = "Please Select Head")]
        public int HeadCode { get; set; }
        List<SelectListItem> lstHead { get; set; }

        public int LevelId { get; set; }
        public string FundType { get; set; }

        public string HeadName { get; set; }

        public int ParentNdCode { get; set; }

        public String MapID { get; set; }

        public String ReportName { get; set; }
        public String ReportParagraphName { get; set; }
        public String ReportFormNo { get; set; }
    }


    #endregion Maintenance Fund Report

    #region Annual Accounts Report

    public class AnnualAccount
    {
        [DisplayName("Year")]
        [Required(ErrorMessage = "Please select year.")]
        [Range(1990, 2099, ErrorMessage = "Please select year")]
        public Int16 Year { get; set; }

        [DisplayName("Balance")]
        [RegularExpression("[CD]", ErrorMessage = "Please select balance.")]
        [Required(ErrorMessage = "Please select balance.")]

        public String CreditDebit { get; set; }

        [DisplayName("Nodal Agency")]
        [Required(ErrorMessage = "Please select nodal agency.")]
        [Range(1, Int64.MaxValue, ErrorMessage = "Please select nodal agency")]
        public String NodalAgency { get; set; }

        [DisplayName("Program Implementation Unit(PIU)")]
        //[Required(ErrorMessage = "Please select PIU.")]
        ////[RequiredDPIU("Selection",ErrorMessage="Please select PIU.")]
        //[Range(1, Int64.MaxValue, ErrorMessage = "Please select PIU")]
        public String PIU { get; set; }

        public List<USP_RPT_SHOW_YEARLY_ACCOUNT_SELF_Result> lstAnnualReport { get; set; }
        public List<USP_RPT_SHOW_YEARLY_ACCOUNT_ALLPIU_Result> lstReportDPIU { get; set; }

        public List<USP_RPT_SHOW_YEARLY_ACCOUNT_SELF_Result> lstReportSelfDPIU { get; set; }
        public Nullable<Decimal> TotalOpeningAmount { get; set; }

        public Nullable<Decimal> TotalCreditDebit { get; set; }


        public Nullable<Decimal> TotalOpeningAmountDPIU { get; set; }

        public Nullable<Decimal> TotalCreditDebitDPIU { get; set; }


        public Nullable<Decimal> TotalOpeningAmountSelf { get; set; }

        public Nullable<Decimal> TotalCreditDebitSelf { get; set; }

        public string FormNo { get; set; }

        public string ReportParaName { get; set; }

        public string ReportName { get; set; }

        public string FundType { get; set; }

        public List<SelectListItem> lstState { get; set; }
        public List<SelectListItem> lstSRRDA { get; set; }
        public List<SelectListItem> lstPIU { get; set; }

        public string Selection { get; set; }

        [RequiredState("Selection", ErrorMessage = "Please Select State")]
        public string State { get; set; }
        public string SRRDA { get; set; }
        public string DPIU { get; set; }

        public Int32 AdminNdCode { get; set; }
        public string ReportHeading { get; set; }
        public string Reference { get; set; }
        public string NodalAgencyName { get; set; }
        public string PIUName { get; set; }
        public string StateName { get; set; }
        public string YearName { get; set; }
        public string DpiuSelection { get; set; }
        public string FundTypeName { get; set; }
    }

    public class RequiredStateAttribute : ValidationAttribute, IClientValidatable
    {
        private string ReportSelection;
        public RequiredStateAttribute(string Selection)
        //: base(_defaultErrorMessage)
        {
            ReportSelection = Selection;
        }

        //Override default FormatErrorMessage Method  
        public override string FormatErrorMessage(string name)
        {
            return string.Format(ErrorMessageString, ReportSelection);
        }

        //Override IsValid  
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            //Get PropertyInfo Object  
            var selection = validationContext.ObjectType.GetProperty(this.ReportSelection);
            var state = value;

            //Actual Validation 
            if (PMGSYSession.Current.LevelId != 5)
            {
                string RptSelection = selection.GetValue(validationContext.ObjectInstance, null).ToString();
                if ((RptSelection == "S" || RptSelection == "R") && (Convert.ToInt32(value) == 0))
                {
                    var message = FormatErrorMessage(validationContext.DisplayName);
                    return new ValidationResult(message);
                }
            }
            return ValidationResult.Success;
        }
        public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
        {
            var rule = new ModelClientValidationRule
            {
                ErrorMessage = FormatErrorMessage(metadata.DisplayName),
                ValidationType = "reqiredstate"
            };
            rule.ValidationParameters["state"] = this.ReportSelection;
            yield return rule;
        }
    }
    #endregion

    #region Income & Expenditure
    public class ReportFormModel
    {
        [DisplayName("Select Month")]
        [Range(1, 12, ErrorMessage = "Please Select Month")]
        public short Month { get; set; }
        public List<SelectListItem> ListMonth { get; set; }

        [DisplayName("Select Year")]
        [Range(2000, short.MaxValue, ErrorMessage = "Please Select Year")]
        public short Year { get; set; }
        public List<SelectListItem> ListYear { get; set; }

        [DisplayName("Select PIU")]
        public int Piu { get; set; }
        public List<SelectListItem> ListPiu { get; set; }

        [DisplayName("Select Agency")]
        [Range(1, int.MaxValue, ErrorMessage = "Please Select Agency")]
        public int Agency { get; set; }
        public List<SelectListItem> ListAgency { get; set; }

        public int LevelId { get; set; }
        public int ReportType { get; set; }
        public int ReportLevel { get; set; }

        public int AdminNdCode { get; set; }
        //public int Month { get; set; }
        public string FundType { get; set; }
        public int isAllDPIU { get; set; }
        public string NodalAgencyName { get; set; }
        public string PIUName { get; set; }
        public string YearName { get; set; }
        public string FormName { get; set; }
        public string ReportHeading { get; set; }
        public string Paragraph { get; set; }
    }

    public class IncomeAndExpenditureModel
    {
        //[DisplayName("Select State")]
        //public int State { get; set; }
        //public List<SelectListItem> ListState { get; set; }

        //[DisplayName("Funding Agency")]
        //public int FundingAgency { get; set; }
        //public List<MASTER_FUNDING_AGENCY> lstFundingAgency { get; set; }

        //[DisplayName("Head")]
        //[Range(1, Int16.MaxValue, ErrorMessage = "Please Select Head")]
        //public int HeadCode { get; set; }
        //public List<ACC_MASTER_HEAD> lstHead { get; set; }

        public int LevelId { get; set; }
    }
    #endregion

    #region Cheque Issued

    public class CheckBookDetailsViewFilterModel
    {

        public CheckBookDetailsViewFilterModel()
        {
            lstYears = new List<SelectListItem>();
            lstMonths = new List<SelectListItem>();
        }

        [Display(Name = "SRRDA")]
        public int SRRDA { get; set; }

        [Display(Name = "Lower")]
        //[IsDPIURequired("LevelId", ErrorMessage = "Please Select DPIU")]
        public int DPIU { get; set; }

        public string UserLevel { get; set; }

        [Display(Name = "Month")]
        public short Month { get; set; }

        //[Display(Name = "Year")]

        public int LevelId { get; set; }

        [RequiredMonthYear("Month", "Year", "CheckbookSeries", ErrorMessage = "Please Select Month and Year")]
        public string CheckbookMonthYearWise { get; set; }

        public short Year { get; set; }

        [Display(Name = "Cheque Book Series")]
        public int CheckbookSeries { get; set; }

        [Display(Name = "Name Of State")]
        public string StateName { get; set; }

        [Display(Name = "Name Of DPIU")]
        public string PIUName { get; set; }

        [Display(Name = "Name Of Bank")]
        public string BankName { get; set; }

        [Display(Name = "Month")]
        public string MonthName { get; set; }

        [Display(Name = "Year")]
        public string YearName { get; set; }

        [Display(Name = "Cheque Book Series")]
        public string CheckbookSeriesName { get; set; }

        [Display(Name = "Name Of SRRDA")]
        public string NodalAgencyName { get; set; }

        [Display(Name = "Selection Type")]

        public string MonthlyOrChequebookWiseSelection { get; set; }

        public int totalRecords { get; set; }

        public List<SelectListItem> lstMonths { get; set; }

        public List<SelectListItem> lstYears { get; set; }

        //lstCheckbookDetails List contains cheque book details
        public List<SP_ACC_RPT_PIU_CHEQUE_ISSUED_DETAILS_Result> lstCheckbookDetails { get; set; }

        //lstChequeIssuedAbstract List contains Cheque book abstract details.
        public List<SP_ACC_RPT_PIU_CHEQUE_ISSUED_ABSTRACT_Result> lstChequeIssuedAbstract { get; set; }

        //lstChequeOutstandingDetails List contains Cheque book outstanding details.
        public List<SP_ACC_RPT_PIU_CHEQUE_OUTSTANDING_DETAILS_Result> lstChequeOutstandingDetails { get; set; }


        //Report Header parameter
        public string FundTypeName { get; set; }
        public string ReportName { get; set; }
        public string ReportParagraphName { get; set; }
        public string ReportFormNumber { get; set; }

        public int AdminNdCode { get; set; }
        public int SRRDANdCode { get; set; }
        public string FundType { get; set; }
        public string flag { get; set; }
        public string CBID { get; set; }
        public string SRRDAName { get; set; }
        public string DPIUName { get; set; }

        public string IsSRRDA_DPIU { get; set; }
    }

    public class RequiredMonthYearAttribute : ValidationAttribute, IClientValidatable
    {
        // private const string _defaultErrorMessage = "Start date must be less than end date.";
        private string Month;
        private string Year;
        private string chqDetails;
        public RequiredMonthYearAttribute(string month, string year, string chqSeries)
        //: base(_defaultErrorMessage)
        {
            Month = month;
            Year = year;
            chqDetails = chqSeries;

        }

        //Override default FormatErrorMessage Method  
        public override string FormatErrorMessage(string name)
        {
            return string.Format(ErrorMessageString, name, Month, Year);
        }

        //Override IsValid  
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            //Get PropertyInfo Object  
            var MonthInfo = validationContext.ObjectType.GetProperty(this.Month);
            var YearInfo = validationContext.ObjectType.GetProperty(this.Year);
            var ChequeInfo = validationContext.ObjectType.GetProperty(this.chqDetails);
            var IsMonthChqWise = value;


            int month = Convert.ToInt32(MonthInfo.GetValue(validationContext.ObjectInstance, null));
            int year = Convert.ToInt32(YearInfo.GetValue(validationContext.ObjectInstance, null));

            int cheque = Convert.ToInt32(ChequeInfo.GetValue(validationContext.ObjectInstance, null));
            //if (sDate != null && eDate != null)
            //{

            //Actual Validation 
            if ((value == null) && (month == 0) && (year == 0))
            {
                return ValidationResult.Success;
            }
            if ((value.ToString() == "M") && (month == 0) && (year == 0))
            {
                var message = FormatErrorMessage(validationContext.DisplayName);
                return new ValidationResult(message);
            }
            else if ((value.ToString() == "M") && (month > 0) && (year == 0))
            {
                var message = FormatErrorMessage(validationContext.DisplayName);
                return new ValidationResult("Please select year.");
            }
            else if ((value.ToString() == "M") && (month == 0) && (year > 0))
            {
                var message = FormatErrorMessage(validationContext.DisplayName);
                return new ValidationResult("Please select month.");
            }
            else if ((value.ToString() == "C") && (cheque == 0))
            {
                var message = FormatErrorMessage(validationContext.DisplayName);
                return new ValidationResult("Please select cheque book series");
            }

            return ValidationResult.Success;
        }


        public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
        {

            var rule = new ModelClientValidationRule
            {
                ErrorMessage = FormatErrorMessage(metadata.DisplayName),
                ValidationType = "reqiredmonthyear"
            };
            rule.ValidationParameters["month"] = this.Month;
            rule.ValidationParameters["year"] = this.Year;
            rule.ValidationParameters["cheque"] = this.chqDetails;
            yield return rule;

        }

    }

    //added by abhishek kamble 13-dec-2013
    public class IsDPIURequiredAttribute : ValidationAttribute, IClientValidatable
    {
        private string LevelID;
        public IsDPIURequiredAttribute(string levelID)
        //: base(_defaultErrorMessage)
        {
            LevelID = levelID;
        }

        //Override default FormatErrorMessage Method  
        public override string FormatErrorMessage(string name)
        {
            return string.Format(ErrorMessageString, LevelID);
        }

        //Override IsValid  
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            //Get PropertyInfo Object  
            var LvlId = validationContext.ObjectType.GetProperty(this.LevelID);
            var DPIU = value;

            int LevelID = Convert.ToInt32(LvlId.GetValue(validationContext.ObjectInstance, null));

            //Actual Validation 
            if (LevelID != 5)
            {
                if (Convert.ToInt32(DPIU) == 0)
                {
                    var message = FormatErrorMessage(validationContext.DisplayName);
                    return new ValidationResult(message);
                }
            }
            return ValidationResult.Success;
        }


        public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
        {
            var rule = new ModelClientValidationRule
            {
                ErrorMessage = FormatErrorMessage(metadata.DisplayName),
                ValidationType = "reqireddpiu"
            };
            rule.ValidationParameters["levelid"] = this.LevelID;
            yield return rule;
        }
    }

    #endregion

    #region Bill Details

    public class AccountBillViewModel
    {

        [Display(Name = "Own")]
        public string SRRDA { get; set; }

        [Display(Name = "DPIU")]
        //[RequiredDPIU("NodalAgency",ErrorMessage="Please Select DPIU")]
        public int DPIU { get; set; }

        [Display(Name = "Month")]
        [RequiredMonthForMonthly("rType", ErrorMessage = "Please select Month")]
        public short Month { get; set; }

        [Display(Name = "Year")]
        [RequiredYearForMonthly("rType", ErrorMessage = "Please select Year")]
        [RequiredYearForYearly("rType", ErrorMessage = "Please select Year")]
        public short Year { get; set; }

        [Display(Name = "Periodic")]
        public string Period { get; set; }

        [Display(Name = "Bill Type")]
        [RegularExpression("[0OPRJ]", ErrorMessage = "Please select Bill Type")]
        public string BillType { get; set; }

        [Display(Name = "Start Date")]
        [RegularExpression(@"^(((0[1-9]|[12]\d|3[01])\/(0[13578]|1[02])\/((19|[2-9]\d)\d{2}))|((0[1-9]|[12]\d|30)\/(0[13456789]|1[012])\/((19|[2-9]\d)\d{2}))|((0[1-9]|1\d|2[0-8])\/02\/((19|[2-9]\d)\d{2}))|(29\/02\/((1[6-9]|[2-9]\d)(0[48]|[2468][048]|[13579][26])|((16|[2468][048]|[3579][26])00))))$", ErrorMessage = "Start Date is not in valid format")]
        [RequiredStartDateForPeriodic("rType", ErrorMessage = "Start Date is required.")]
        public string StartDate { get; set; }

        [Display(Name = "End Date")]
        [RegularExpression(@"^(((0[1-9]|[12]\d|3[01])\/(0[13578]|1[02])\/((19|[2-9]\d)\d{2}))|((0[1-9]|[12]\d|30)\/(0[13456789]|1[012])\/((19|[2-9]\d)\d{2}))|((0[1-9]|1\d|2[0-8])\/02\/((19|[2-9]\d)\d{2}))|(29\/02\/((1[6-9]|[2-9]\d)(0[48]|[2468][048]|[13579][26])|((16|[2468][048]|[3579][26])00))))$", ErrorMessage = "End Date is not in valid format")]
        [AssetDateValidation("StartDate", ErrorMessage = "End Date must be greater than or equal to start date.")]
        [AssetCurrentDateValidation("StartDate", ErrorMessage = "End date must be less than or equals to todays date.")]
        [RequiredEndDateForPeriodic("rType", ErrorMessage = "End Date is required.")]
        public string EndDate { get; set; }


        public Int64 BillId { get; set; }
        public string rType { get; set; }

        public int AdminNdCode { get; set; }

        public string FundType { get; set; }

        public string BilltypeName { get; set; }
        public string MonthName { get; set; }
        public string DPIUName { get; set; }

        public int levelId { get; set; }

        public string NodalAgency { get; set; }
        public string DPIUByNO { get; set; }

        public int TotalRecords { get; set; }

        public string DPIUBySRRDA { get; set; }
        public List<SelectListItem> ddlDPIU { get; set; }

        public List<SelectListItem> ddlBillType { get; set; }

        public List<SelectListItem> GetBillType()
        {
            List<SelectListItem> lst = new List<SelectListItem>();
            //lst.Add(new SelectListItem { Text = "--Select--", Value = "O" });
            //Modifed for all txn by abhishek kamble 21-June-2014
            lst.Add(new SelectListItem { Text = "--All--", Value = "0" });
            lst.Add(new SelectListItem { Text = "Opening Balance", Value = "O" });
            lst.Add(new SelectListItem { Text = "Payment", Value = "P" });
            lst.Add(new SelectListItem { Text = "Receipts", Value = "R" });
            lst.Add(new SelectListItem { Text = "Transfer Entry Order", Value = "J" });
            return lst;
        }

        public AccountBillViewModel()
        {
            ddlBillType = new List<SelectListItem>();
        }

        public List<SP_ACC_RPT_DISPALY_Bill_DETAILS_Result> lstAccountBillDetails { get; set; }


        //Added By abhishek kamble 15-jan-2014
        public List<SelectListItem> MonthList { get; set; }
        public List<SelectListItem> YearList { get; set; }

        public int isAllPiu { get; set; }
        public string YearName { get; set; }
        public string StateName { get; set; }
        public string Selection { get; set; }
    }

    public class AssetDateValidationAttribute : ValidationAttribute, IClientValidatable
    {
        // private const string _defaultErrorMessage = "Start date must be less than end date.";
        private string _basePropertyName;

        public AssetDateValidationAttribute(string basePropertyName)
        //: base(_defaultErrorMessage)
        {
            _basePropertyName = basePropertyName;
        }

        //Override default FormatErrorMessage Method  
        public override string FormatErrorMessage(string name)
        {
            return string.Format(ErrorMessageString, name, _basePropertyName);
        }

        //Override IsValid  
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            //Get PropertyInfo Object  
            var basePropertyInfo = validationContext.ObjectType.GetProperty(_basePropertyName);

            //Get Value of the property  

            //var startDate = (DateTime)basePropertyInfo.GetValue(validationContext.ObjectInstance, null);            
            //var thisDate = (DateTime)value;  

            var sDate = basePropertyInfo.GetValue(validationContext.ObjectInstance, null);
            var eDate = value;

            if (sDate != null && eDate != null)
            {
                var startDate = ConvertStringToDate(sDate.ToString());
                var thisDate = ConvertStringToDate(eDate.ToString());

                //Actual comparision  
                if (thisDate < startDate)
                {
                    var message = FormatErrorMessage(validationContext.DisplayName);
                    return new ValidationResult(message);
                }

                //System.DateTime toDaysDate = System.DateTime.Now;

                //if (thisDate > toDaysDate)
                //{
                //    //var message = "To date must be less than or equal to today's date.";
                //    var message = FormatErrorMessage(validationContext.DisplayName);
                //    return new ValidationResult(message);
                //}
            }

            //Default return - This means there were no validation error  
            //return null;
            return ValidationResult.Success;
        }


        public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
        {
            //yield return new ModelClientValidationRule
            //{
            //    ErrorMessage = FormatErrorMessage(metadata.DisplayName), 
            //    ValidationType = "datecomparefieldvalidator"
            //};

            var rule = new ModelClientValidationRule
            {
                ErrorMessage = FormatErrorMessage(metadata.DisplayName),
                ValidationType = "datecomparefieldvalidator"
            };
            rule.ValidationParameters["date"] = this._basePropertyName;
            yield return rule;

        }


        public DateTime? ConvertStringToDate(string dateToConvert)
        {
            DateTime MyDateTime;
            MyDateTime = new DateTime();
            // MyDateTime = DateTime.Parse(dateToConvert);
            MyDateTime = DateTime.ParseExact(dateToConvert, "dd/MM/yyyy", null);
            //Convert.ToDateTime(dateToConvert);         //DateTime.ParseExact(dateToConver, "dd/MM/yyyy",null);
            return MyDateTime;
        }

    }

    public class AssetCurrentDateValidationAttribute : ValidationAttribute, IClientValidatable
    {

        // private const string _defaultErrorMessage = "Start date must be less than end date.";
        private string _basePropertyName;

        public AssetCurrentDateValidationAttribute(string basePropertyName)
        //: base(_defaultErrorMessage)
        {
            _basePropertyName = basePropertyName;
        }

        //Override default FormatErrorMessage Method  
        public override string FormatErrorMessage(string name)
        {
            return string.Format(ErrorMessageString, name, _basePropertyName);
        }

        //Override IsValid  
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            //Get PropertyInfo Object  
            var basePropertyInfo = validationContext.ObjectType.GetProperty(_basePropertyName);

            //Get Value of the property  

            //var startDate = (DateTime)basePropertyInfo.GetValue(validationContext.ObjectInstance, null);            
            //var thisDate = (DateTime)value;  

            var sDate = basePropertyInfo.GetValue(validationContext.ObjectInstance, null);
            var eDate = value;

            if (eDate != null)
            {
                var thisDate = ConvertStringToDate(eDate.ToString());

                System.DateTime toDaysDate = System.DateTime.Now;
                if (thisDate > toDaysDate)
                {
                    var message = FormatErrorMessage(validationContext.DisplayName);
                    return new ValidationResult(message);
                }
            }
            return ValidationResult.Success;
        }


        public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
        {
            //yield return new ModelClientValidationRule
            //{
            //    ErrorMessage = FormatErrorMessage(metadata.DisplayName), 
            //    ValidationType = "datecomparefieldvalidator"
            //};

            var rule = new ModelClientValidationRule
            {
                ErrorMessage = FormatErrorMessage(metadata.DisplayName),
                ValidationType = "currentdatefieldvalidator"
            };
            rule.ValidationParameters["date"] = this._basePropertyName;
            yield return rule;

        }


        public DateTime? ConvertStringToDate(string dateToConvert)
        {
            DateTime MyDateTime;
            MyDateTime = new DateTime();
            MyDateTime = DateTime.ParseExact(dateToConvert, "dd/MM/yyyy", null);
            return MyDateTime;
        }

    }



    //Added By Abhishek Kamble 12-Nov-2013

    public class RequiredMonthForMonthlyAttribute : ValidationAttribute, IClientValidatable
    {
        private string _baasePropertyName;

        public RequiredMonthForMonthlyAttribute(string basePropertyName)
        {
            _baasePropertyName = basePropertyName;
        }

        public override string FormatErrorMessage(string name)
        {
            return string.Format(ErrorMessageString, name, _baasePropertyName);
        }

        //Override IsValid()      
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {

            //Get Property Info Object
            var basePropertyInfo = validationContext.ObjectType.GetProperty(_baasePropertyName);
            var IsMonthly = basePropertyInfo.GetValue(validationContext.ObjectInstance, null);
            int Month = Convert.ToInt32(value);

            //Actual Validation
            if ((Convert.ToString(IsMonthly) == "M") && (Month == 0))
            {
                var message = FormatErrorMessage(validationContext.DisplayName);
                return new ValidationResult(message);
            }
            return ValidationResult.Success;
        }//end of IsValid()          

        public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
        {

            var rule = new ModelClientValidationRule
            {
                ErrorMessage = FormatErrorMessage(metadata.DisplayName),
                ValidationType = "requiredmonthformonthly"

            };
            rule.ValidationParameters["month"] = this._baasePropertyName;
            yield return rule;
        }
    }

    public class RequiredYearForMonthlyAttribute : ValidationAttribute, IClientValidatable
    {
        private string _baasePropertyName;

        public RequiredYearForMonthlyAttribute(string basePropertyName)
        {
            _baasePropertyName = basePropertyName;
        }

        public override string FormatErrorMessage(string name)
        {
            return string.Format(ErrorMessageString, name, _baasePropertyName);
        }

        //Override IsValid()      
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {

            //Get Property Info Object
            var basePropertyInfo = validationContext.ObjectType.GetProperty(_baasePropertyName);
            var IsMonthly = basePropertyInfo.GetValue(validationContext.ObjectInstance, null);
            int Year = Convert.ToInt32(value);

            //Actual Validation
            if ((Convert.ToString(IsMonthly) == "M") && (Year == 0))
            {
                var message = FormatErrorMessage(validationContext.DisplayName);
                return new ValidationResult(message);
            }
            return ValidationResult.Success;
        }//end of IsValid()          

        public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
        {

            var rule = new ModelClientValidationRule
            {
                ErrorMessage = FormatErrorMessage(metadata.DisplayName),
                ValidationType = "requiredyearformonthly"

            };
            rule.ValidationParameters["year"] = this._baasePropertyName;
            yield return rule;
        }
    }

    public class RequiredYearForYearlyAttribute : ValidationAttribute, IClientValidatable
    {
        private string _baasePropertyName;

        public RequiredYearForYearlyAttribute(string basePropertyName)
        {
            _baasePropertyName = basePropertyName;
        }

        public override string FormatErrorMessage(string name)
        {
            return string.Format(ErrorMessageString, name, _baasePropertyName);
        }

        //Override IsValid()      
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {

            //Get Property Info Object
            var basePropertyInfo = validationContext.ObjectType.GetProperty(_baasePropertyName);
            var IsYearly = basePropertyInfo.GetValue(validationContext.ObjectInstance, null);
            int Year = Convert.ToInt32(value);

            //Actual Validation
            if ((Convert.ToString(IsYearly) == "Y") && (Year == 0))
            {
                var message = FormatErrorMessage(validationContext.DisplayName);
                return new ValidationResult(message);
            }
            return ValidationResult.Success;
        }//end of IsValid()          

        public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
        {

            var rule = new ModelClientValidationRule
            {
                ErrorMessage = FormatErrorMessage(metadata.DisplayName),
                ValidationType = "requiredyearforyearly"

            };
            rule.ValidationParameters["year"] = this._baasePropertyName;
            yield return rule;
        }
    }

    public class RequiredStartDateForPeriodicAttribute : ValidationAttribute, IClientValidatable
    {
        private string _baasePropertyName;

        public RequiredStartDateForPeriodicAttribute(string basePropertyName)
        {
            _baasePropertyName = basePropertyName;
        }

        public override string FormatErrorMessage(string name)
        {
            return string.Format(ErrorMessageString, name, _baasePropertyName);
        }

        //Override IsValid()      
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {

            //Get Property Info Object
            var basePropertyInfo = validationContext.ObjectType.GetProperty(_baasePropertyName);
            var IsPeriodic = basePropertyInfo.GetValue(validationContext.ObjectInstance, null);
            // int Year = Convert.ToInt32(value);

            //Actual Validation
            if ((Convert.ToString(IsPeriodic) == "P") && (value == null))
            {
                var message = FormatErrorMessage(validationContext.DisplayName);
                return new ValidationResult(message);
            }
            return ValidationResult.Success;
        }//end of IsValid()          

        public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
        {
            var rule = new ModelClientValidationRule
            {
                ErrorMessage = FormatErrorMessage(metadata.DisplayName),
                ValidationType = "requiredstartdateforperiodic"

            };
            rule.ValidationParameters["startdate"] = this._baasePropertyName;
            yield return rule;
        }
    }

    public class RequiredEndDateForPeriodicAttribute : ValidationAttribute, IClientValidatable
    {
        private string _baasePropertyName;

        public RequiredEndDateForPeriodicAttribute(string basePropertyName)
        {
            _baasePropertyName = basePropertyName;
        }

        public override string FormatErrorMessage(string name)
        {
            return string.Format(ErrorMessageString, name, _baasePropertyName);
        }

        //Override IsValid()      
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {

            //Get Property Info Object
            var basePropertyInfo = validationContext.ObjectType.GetProperty(_baasePropertyName);
            var IsPeriodic = basePropertyInfo.GetValue(validationContext.ObjectInstance, null);
            // int Year = Convert.ToInt32(value);

            //Actual Validation
            if ((Convert.ToString(IsPeriodic) == "P") && (value == null))
            {
                var message = FormatErrorMessage(validationContext.DisplayName);
                return new ValidationResult(message);
            }
            return ValidationResult.Success;
        }//end of IsValid()          

        public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
        {
            var rule = new ModelClientValidationRule
            {
                ErrorMessage = FormatErrorMessage(metadata.DisplayName),
                ValidationType = "requiredenddateforperiodic"

            };
            rule.ValidationParameters["enddate"] = this._baasePropertyName;
            yield return rule;
        }
    }
    public class RequiredDPIUAttribute : ValidationAttribute, IClientValidatable
    {
        private string _baasePropertyName;

        public RequiredDPIUAttribute(string basePropertyName)
        {
            _baasePropertyName = basePropertyName;
        }

        public override string FormatErrorMessage(string name)
        {
            return string.Format(ErrorMessageString, name, _baasePropertyName);
        }

        //Override IsValid()      
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {

            //Get Property Info Object
            var basePropertyInfo = validationContext.ObjectType.GetProperty(_baasePropertyName);
            var IsDPIU = basePropertyInfo.GetValue(validationContext.ObjectInstance, null);
            int DPIU = Convert.ToInt32(value);

            //Actual Validation
            if (IsDPIU != null)
            {
                if ((Convert.ToString(IsDPIU)== "D") && (DPIU == 0))
                {
                    var message = FormatErrorMessage(validationContext.DisplayName);
                    return new ValidationResult(message);
                }
            }
            return ValidationResult.Success;
        }//end of IsValid()          

        public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
        {
            var rule = new ModelClientValidationRule
            {
                ErrorMessage = FormatErrorMessage(metadata.DisplayName),
                ValidationType = "requireddpiu"

            };
            rule.ValidationParameters["dpiu"] = this._baasePropertyName;
            yield return rule;
        }
    }

    #endregion

    #region CashPaymentReport


    public class CashPaymentViewModel
    {
        public string SRRDAStateName { get; set; }

        [Display(Name = "SRRDA")]
        public int SRRDA { get; set; }

        public string SRRDA_DPIU { get; set; }

        [Display(Name = "DPIU")]
        [RequiredDPIU("NodalAgency", ErrorMessage = "Please Select DPIU")]
        public int DPIU { get; set; }

        [Display(Name = "NRRDA")]
        public string NRRDA { get; set; }

        [Display(Name = "Month")]
        [RequiredMonthForMonthly("rType", ErrorMessage = "Please select Month")]
        public short Month { get; set; }

        [Display(Name = "Year")]
        [RequiredYearForMonthly("rType", ErrorMessage = "Please select Year")]
        [RequiredYearForYearly("rType", ErrorMessage = "Please select Year")]
        public short Year { get; set; }

        [Display(Name = "Periodic")]
        public string Period { get; set; }


        [Display(Name = "Start Date")]
        [RegularExpression(@"^(((0[1-9]|[12]\d|3[01])\/(0[13578]|1[02])\/((19|[2-9]\d)\d{2}))|((0[1-9]|[12]\d|30)\/(0[13456789]|1[012])\/((19|[2-9]\d)\d{2}))|((0[1-9]|1\d|2[0-8])\/02\/((19|[2-9]\d)\d{2}))|(29\/02\/((1[6-9]|[2-9]\d)(0[48]|[2468][048]|[13579][26])|((16|[2468][048]|[3579][26])00))))$", ErrorMessage = "Start Date is not in valid format")]
        [RequiredStartDateForPeriodic("rType", ErrorMessage = "Start Date is required.")]
        public string StartDate { get; set; }

        [Display(Name = "End Date")]
        [RegularExpression(@"^(((0[1-9]|[12]\d|3[01])\/(0[13578]|1[02])\/((19|[2-9]\d)\d{2}))|((0[1-9]|[12]\d|30)\/(0[13456789]|1[012])\/((19|[2-9]\d)\d{2}))|((0[1-9]|1\d|2[0-8])\/02\/((19|[2-9]\d)\d{2}))|(29\/02\/((1[6-9]|[2-9]\d)(0[48]|[2468][048]|[13579][26])|((16|[2468][048]|[3579][26])00))))$", ErrorMessage = "End Date is not in valid format")]
        [AssetDateValidation("StartDate", ErrorMessage = "End Date must be greater than or equal to start date.")]
        [AssetCurrentDateValidation("StartDate", ErrorMessage = "End date must be less than or equals to todays date.")]
        [RequiredEndDateForPeriodic("rType", ErrorMessage = "End Date is required.")]
        public string EndDate { get; set; }

        public string rType { get; set; }

        public int AdminNdCode { get; set; }

        public string MonthName { get; set; }
        public string DPIUName { get; set; }

        public int levelId { get; set; }

        public string NodalAgency { get; set; }
        public string DPIUByNO { get; set; }

        public int TotalRecords { get; set; }

        public string DPIUBySRRDA { get; set; }

        public List<SelectListItem> ddlDPIU { get; set; }

        public List<SelectListItem> States { get; set; }

        public List<SP_ACC_RPT_DISPALY_Bill_DETAILS_Result> lstAccountBillDetails { get; set; }

        //Added By 
        public List<SelectListItem> MonthList { get; set; }
        public List<SelectListItem> YearList { get; set; }

        [Display(Name = "Module Type")]

        public char Module { get; set; }

        public List<SelectListItem> ModuleType { get; set; }

        public int isAllPiu { get; set; }
        public string YearName { get; set; }
        public string StateName { get; set; }
        public string Selection { get; set; }

        [Display(Name = "Fund Type")]
        [Required(ErrorMessage = "Fund type is required.")]
        public string Fundtype { get; set; }

    }
    #endregion

    #region Remittances and Reconciliation
    public class Schedule
    {
        public Int16 ITEM_ID { get; set; }
        public string ITEM_HEADING { get; set; }
        public decimal? CURRENT_AMT { get; set; }
        public decimal? PREVIOUS_AMT { get; set; }
        public Int16 SORT_ORDER { get; set; }
    }

    public class ScheduleModel
    {
        [DisplayName("Select Month")]
        [Range(1, 12, ErrorMessage = "Please Select Month")]
        public short Month { get; set; }
        public List<SelectListItem> ListMonth { get; set; }

        [DisplayName("Select Year")]
        [Range(2000, short.MaxValue, ErrorMessage = "Please Select Year")]
        public short Year { get; set; }
        public List<SelectListItem> ListYear { get; set; }

        [DisplayName("Select PIU")]
        //Added By Abhishek Kamble 2-dec-2013
        [Range(1, Int32.MaxValue, ErrorMessage = "Please select DPIU")]
        public int Piu { get; set; }
        public List<SelectListItem> ListPiu { get; set; }

        [DisplayName("Select State")]
        public int State { get; set; }
        public List<SelectListItem> ListState { get; set; }

        [DisplayName("Funding Agency")]
        public int FundingAgency { get; set; }
        public List<MASTER_FUNDING_AGENCY> lstFundingAgency { get; set; }

        [DisplayName("Head")]
        [Range(1, Int16.MaxValue, ErrorMessage = "Please Select Head")]
        public int HeadCode { get; set; }
        public List<ACC_MASTER_HEAD> lstHead { get; set; }

        public List<USP_ACC_RPT_SCHEDULE_OF_ROADS_Result> lstSchedule { get; set; }
        public int LevelId { get; set; }
        public string FundType { get; set; }

        public string Header { get; set; }
        public string FormNumber { get; set; }
        public string Paragraph1 { get; set; }
        public string Paragraph2 { get; set; }
        public bool IsYearly { get; set; }
        public bool IsSrrdaPiu { get; set; }
        public string PiuName { get; set; }
        public string HeadName { get; set; }
        public string YearName { get; set; }
        public string AgencyName { get; set; }
        public string MonthName { get; set; }
        public string StateName { get; set; }
        public string ScheduleName { get; set; }

        //added by abhishek kamlbe 20-dec-2013 
        public int ReportType { get; set; }
        public int ReportLevel { get; set; }
        public string ReportName { get; set; }

        public int AdminNdCode { get; set; }
        public int isallPiu { get; set; }
        public string NodalAgencyName { get; set; }
    }
    public class AccountFilterModel
    {
        [DisplayName("Select Month")]
        //[Range(1, 12, ErrorMessage = "Please Select Month")]
        [RequiredMonth("ReportType", ErrorMessage = "Please select Month")]
        public short Month { get; set; }
        public List<SelectListItem> ListMonth { get; set; }

        [DisplayName("Select Year")]
        [Range(2000, short.MaxValue, ErrorMessage = "Please Select Year")]
        public short Year { get; set; }
        public List<SelectListItem> ListYear { get; set; }

        [DisplayName("Select PIU")]
        public int Piu { get; set; }

        public List<SelectListItem> ListPiu { get; set; }

        [DisplayName("Select Agency")]
        [Range(0, int.MaxValue, ErrorMessage = "Please Select Agency")]
        public int Agency { get; set; }
        public List<SelectListItem> ListAgency { get; set; }

        public int LevelId { get; set; }
        public int ReportType { get; set; }
        public int ReportLevel { get; set; }
    }

    //Added By Ashish Markande
    /// <summary>
    /// 
    /// </summary>
    public class ScheduleUtilization
    {
        //public string LineNo { get; set; }
        //public String Description { get; set; }
        //public decimal InnerColumn { get; set; }
        //public decimal OuterColumn { get; set; }

        public string LineNo { get; set; }
        public string Desc { get; set; }
        public Nullable<decimal> InnerCol { get; set; }
        public Nullable<decimal> OuterCol { get; set; }
    }

    public class ScheduleReconciliation
    {
        public int ID { get; set; }
        public string Desc { get; set; }
        public Nullable<decimal> This_Amt { get; set; }
        public Nullable<decimal> Prev_Amt { get; set; }
    }


    //Added by Abhishek kamble
    public class ScheduleCurrentAssets
    {
        public int ID { get; set; }
        public string Particulars { get; set; }
        public string AmountFlag { get; set; }
        public Nullable<decimal> Amount { get; set; }
    }
    //Added by Abhishek kamble
    public class ScheduleDurableAssets
    {
        public string HEAD_CODE { get; set; }
        public string Centarl_State { get; set; }
        public string HEAD_NAME { get; set; }
        public Nullable<decimal> OBAmt { get; set; }
        public Nullable<decimal> MonthlyAmt { get; set; }
        public Nullable<decimal> TotAmt { get; set; }
    }

    public class ScheduleFundReconciliation
    {
        public int Sno { get; set; }
        public string Particulars { get; set; }
        public string IsAmt { get; set; }
        public Nullable<decimal> InnerAmt { get; set; }
        public Nullable<decimal> OuterAmt { get; set; }
    }

    public class ScheduleCurrentLiabilities
    {
        public string GROUP_ID { get; set; }
        public Nullable<int> ITEM_ID { get; set; }
        public string ITEM_HEADING { get; set; }
        public Nullable<decimal> CURRENT_AMT { get; set; }
        public Nullable<decimal> PREVIOUS_AMT { get; set; }
        public string LINK { get; set; }
        public Nullable<int> SORT_ORDER { get; set; }
    }

    public class ScheduleFundTransferred
    {
        public string GROUP_ID { get; set; }
        public Nullable<int> ITEM_ID { get; set; }
        public string ITEM_HEADING { get; set; }
        public Nullable<decimal> CURRENT_AMT { get; set; }
        public Nullable<decimal> PREVIOUS_AMT { get; set; }
        public string LINK { get; set; }
        public Nullable<int> SORT_ORDER { get; set; }
    }


    //Added By Abhishek Kamble   2-dec-2013
    //public class RequiredMonthAttribute : ValidationAttribute, IClientValidatable
    //{
    //    // private const string _defaultErrorMessage = "Start date must be less than end date.";
    //    private string _basePropertyName;

    //    public RequiredMonthAttribute(string basePropertyName)
    //    //: base(_defaultErrorMessage)
    //    {
    //        _basePropertyName = basePropertyName;
    //    }

    //    //Override default FormatErrorMessage Method  
    //    public override string FormatErrorMessage(string name)
    //    {
    //        return string.Format(ErrorMessageString, name, _basePropertyName);
    //    }

    //    //Override IsValid  
    //    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    //    {
    //        //Get PropertyInfo Object  
    //        var basePropertyInfo = validationContext.ObjectType.GetProperty(_basePropertyName);

    //        var IsReportLevelMonthly = basePropertyInfo.GetValue(validationContext.ObjectInstance, null);
    //        //int month = (int)value;

    //        var month = value;

    //        //if (sDate != null && eDate != null)
    //        //{

    //        //Actual Validation 
    //        if ((Convert.ToInt32(IsReportLevelMonthly) == 2) && (Convert.ToInt32(month)==0))
    //        {
    //            var message = FormatErrorMessage(validationContext.DisplayName);
    //            return new ValidationResult(message);
    //        }

    //        return ValidationResult.Success;
    //    }
    //    public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
    //    {
    //        var rule = new ModelClientValidationRule
    //        {
    //            ErrorMessage = FormatErrorMessage(metadata.DisplayName),
    //            ValidationType = "monthvalidator"
    //        };
    //        rule.ValidationParameters["month"] = this._basePropertyName;
    //        yield return rule;
    //    }
    //}
    #endregion

    #region Asset Register

    public class AssetRegisterViewModel
    {
        [Display(Name = "SRRDA")]
        public string SRRDA { get; set; }

        [Display(Name = "DPIU")]
        public int DPIU { get; set; }

        public string SRRDADPIU { get; set; }

        [Display(Name = "Month")]
        public short Month { get; set; }

        [Display(Name = "Year")]
        public short Year { get; set; }

        public string monthlyPeriodicFundWise { get; set; }

        [Display(Name = "Periodic")]
        public string Period { get; set; }

        [Display(Name = "From Date")]
        [RegularExpression(@"^(((0[1-9]|[12]\d|3[01])\/(0[13578]|1[02])\/((19|[2-9]\d)\d{2}))|((0[1-9]|[12]\d|30)\/(0[13456789]|1[012])\/((19|[2-9]\d)\d{2}))|((0[1-9]|1\d|2[0-8])\/02\/((19|[2-9]\d)\d{2}))|(29\/02\/((1[6-9]|[2-9]\d)(0[48]|[2468][048]|[13579][26])|((16|[2468][048]|[3579][26])00))))$", ErrorMessage = "From Date is not in valid format")]
        public string FromDate { get; set; }

        [Display(Name = "To Date")]
        [RegularExpression(@"^(((0[1-9]|[12]\d|3[01])\/(0[13578]|1[02])\/((19|[2-9]\d)\d{2}))|((0[1-9]|[12]\d|30)\/(0[13456789]|1[012])\/((19|[2-9]\d)\d{2}))|((0[1-9]|1\d|2[0-8])\/02\/((19|[2-9]\d)\d{2}))|(29\/02\/((1[6-9]|[2-9]\d)(0[48]|[2468][048]|[13579][26])|((16|[2468][048]|[3579][26])00))))$", ErrorMessage = "To Date is not in valid format")]
        [AssetDateValidation("FromDate", ErrorMessage = "To Date must be greater than or equal to From Date.")]
        [AssetCurrentDateValidation("FromDate", ErrorMessage = "To Date must be less than or equal to Today's Date.")]

        public string ToDate { get; set; }

        [Display(Name = "Fund Type")]
        public string FundCentralState { get; set; }

        [Display(Name = "Name Of DPIU")]
        public string DPIUName { get; set; }

        [Display(Name = "Name Of SRRDA")]
        public string NodalAgencyName { get; set; }

        public string MonthName { get; set; }

        public string FundStateCentralName { get; set; }

        public string AssetPurchaseDetails { get; set; }

        [Display(Name = "Classification Code")]
        public string ClassificationCode { get; set; }

        public string FundDescription { get; set; }

        //lst Asset Register List contains Asset Register Details
        public List<USP_ACC_RPT_REGISTER_DURABLE_ASSETS_Result> lstAssetRegisterDetails { get; set; }

        //lst of Asset Register Classification Code 
        public List<UDF_ACC_GET_ASSET_HEADS_Result> lstAssetRegisterClassificationDetails { get; set; }

        public Decimal? TotalAmount { get; set; }

        //Report Header parameter
        public string FundTypeName { get; set; }
        public string ReportName { get; set; }
        public string ReportParagraphName { get; set; }
        public string ReportFormNumber { get; set; }

        public int AdminNdCode { get; set; }
        public string YearName { get; set; }
        public string SRRDAName { get; set; }
        public string PiuName { get; set; }
        public string FundType { get; set; }
        public string Flag { get; set; }

        public string StartDate { get; set; }
        public string EndDate { get; set; }

    }

    //public class AssetDateValidationAttribute : ValidationAttribute, IClientValidatable
    //{

    //    // private const string _defaultErrorMessage = "Start date must be less than end date.";
    //    private string _basePropertyName;

    //    public AssetDateValidationAttribute(string basePropertyName)
    //    //: base(_defaultErrorMessage)
    //    {
    //        _basePropertyName = basePropertyName;
    //    }

    //    //Override default FormatErrorMessage Method  
    //    public override string FormatErrorMessage(string name)
    //    {
    //        return string.Format(ErrorMessageString, name, _basePropertyName);
    //    }

    //    //Override IsValid  
    //    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    //    {
    //        //Get PropertyInfo Object  
    //        var basePropertyInfo = validationContext.ObjectType.GetProperty(_basePropertyName);

    //        //Get Value of the property  

    //        //var startDate = (DateTime)basePropertyInfo.GetValue(validationContext.ObjectInstance, null);            
    //        //var thisDate = (DateTime)value;  

    //        var sDate = basePropertyInfo.GetValue(validationContext.ObjectInstance, null);
    //        var eDate = value;

    //        if (sDate != null && eDate != null)
    //        {
    //            var startDate = ConvertStringToDate(sDate.ToString());
    //            var thisDate = ConvertStringToDate(eDate.ToString());

    //            //Actual comparision  
    //            if (thisDate < startDate)
    //            {
    //                var message = FormatErrorMessage(validationContext.DisplayName);
    //                return new ValidationResult(message);
    //            }

    //            //System.DateTime toDaysDate = System.DateTime.Now;

    //            //if (thisDate > toDaysDate)
    //            //{
    //            //    //var message = "To date must be less than or equal to today's date.";
    //            //    var message = FormatErrorMessage(validationContext.DisplayName);
    //            //    return new ValidationResult(message);
    //            //}
    //        }

    //        //Default return - This means there were no validation error  
    //        //return null;
    //        return ValidationResult.Success;
    //    }


    //    public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
    //    {
    //        //yield return new ModelClientValidationRule
    //        //{
    //        //    ErrorMessage = FormatErrorMessage(metadata.DisplayName), 
    //        //    ValidationType = "datecomparefieldvalidator"
    //        //};

    //        var rule = new ModelClientValidationRule
    //        {
    //            ErrorMessage = FormatErrorMessage(metadata.DisplayName),
    //            ValidationType = "datecomparefieldvalidator"
    //        };
    //        rule.ValidationParameters["date"] = this._basePropertyName;
    //        yield return rule;

    //    }


    //    public DateTime? ConvertStringToDate(string dateToConvert)
    //    {
    //        DateTime MyDateTime;
    //        MyDateTime = new DateTime();
    //        // MyDateTime = DateTime.Parse(dateToConvert);
    //        MyDateTime = DateTime.ParseExact(dateToConvert, "dd/MM/yyyy", null);
    //        //Convert.ToDateTime(dateToConvert);         //DateTime.ParseExact(dateToConver, "dd/MM/yyyy",null);
    //        return MyDateTime;
    //    }

    //}

    //public class AssetCurrentDateValidationAttribute : ValidationAttribute, IClientValidatable
    //{

    //    // private const string _defaultErrorMessage = "Start date must be less than end date.";
    //    private string _basePropertyName;

    //    public AssetCurrentDateValidationAttribute(string basePropertyName)
    //    //: base(_defaultErrorMessage)
    //    {
    //        _basePropertyName = basePropertyName;
    //    }

    //    //Override default FormatErrorMessage Method  
    //    public override string FormatErrorMessage(string name)
    //    {
    //        return string.Format(ErrorMessageString, name, _basePropertyName);
    //    }

    //    //Override IsValid  
    //    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    //    {
    //        //Get PropertyInfo Object  
    //        var basePropertyInfo = validationContext.ObjectType.GetProperty(_basePropertyName);

    //        //Get Value of the property  

    //        //var startDate = (DateTime)basePropertyInfo.GetValue(validationContext.ObjectInstance, null);            
    //        //var thisDate = (DateTime)value;  

    //        var sDate = basePropertyInfo.GetValue(validationContext.ObjectInstance, null);
    //        var eDate = value;

    //        if (eDate != null)
    //        {
    //            var thisDate = ConvertStringToDate(eDate.ToString());

    //            System.DateTime toDaysDate = System.DateTime.Now;
    //            if (thisDate > toDaysDate)
    //            {
    //                var message = FormatErrorMessage(validationContext.DisplayName);
    //                return new ValidationResult(message);
    //            }
    //        }
    //        return ValidationResult.Success;
    //    }


    //    public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
    //    {
    //        //yield return new ModelClientValidationRule
    //        //{
    //        //    ErrorMessage = FormatErrorMessage(metadata.DisplayName), 
    //        //    ValidationType = "datecomparefieldvalidator"
    //        //};

    //        var rule = new ModelClientValidationRule
    //        {
    //            ErrorMessage = FormatErrorMessage(metadata.DisplayName),
    //            ValidationType = "currentdatefieldvalidator"
    //        };
    //        rule.ValidationParameters["date"] = this._basePropertyName;
    //        yield return rule;

    //    }


    //    public DateTime? ConvertStringToDate(string dateToConvert)
    //    {
    //        DateTime MyDateTime;
    //        MyDateTime = new DateTime();
    //        MyDateTime = DateTime.ParseExact(dateToConvert, "dd/MM/yyyy", null);
    //        return MyDateTime;
    //    }

    //}

    #endregion

    #region Transfer Entry Order

    public class RptTransferEntryOrder
    {
        public RptTransferEntryOrder()
        {
        }
        [Display(Name = "Month")]
        [Range(1, 12, ErrorMessage = "Please Select Month")]
        public short Month { get; set; }
        public List<SelectListItem> MonthList { get; set; }


        [Display(Name = "Year")]
        [Range(1, short.MaxValue, ErrorMessage = "Please Select Year")]
        public short Year { get; set; }
        public List<SelectListItem> YearList { get; set; }

        [Display(Name = "DPIU")]
        [Range(1, short.MaxValue, ErrorMessage = "Please Select DPIU")]
        public short Dpiu { get; set; }
        public List<SelectListItem> DpiuList { get; set; }

        public bool isSRRDA { get; set; }
        public int AdminNDCode { get; set; }

        public string FundType { get; set; }



    }
    public class RptTrnasferEntryOrderList
    {
        public List<SP_ACC_RPT_DISPLAY_TEO_DETAILS_Result> ListTeo { get; set; }
        public double DebitAmt { get; set; }
        public double CreditAmt { get; set; }
        public string FormNumber { get; set; }
        public string ReportName { get; set; }
        public string Paragraph { get; set; }

        public int AdminNDCode { get; set; }
        public int Month { get; set; }
        public int Year { get; set; }
        public string FundType { get; set; }
        public string SrrdaName { get; set; }
        public string PIUName { get; set; }
        public string YearName { get; set; }
    }

    #endregion




    #region Fund Transfer
    public class FundTransferViewModel
    {
        [Required(ErrorMessage = "Please select month.")]
        [Range(1, 12, ErrorMessage = "Please select month.")]
        public string Month { get; set; }

        [Required(ErrorMessage = "Please select year.")]
        [Range(1990, 2099, ErrorMessage = "Please select year.")]
        public string Year { get; set; }

        [Required(ErrorMessage = "Please select head.")]
        [Range(1, Int64.MaxValue, ErrorMessage = "Please select head.")]
        public string HeadCode { get; set; }

        public string StateCode { get; set; }

        public string DPIUCode { get; set; }

        public string LevelId
        {
            get
            {
                return PMGSYSession.Current.LevelId.ToString();
            }
            set
            {
                PMGSYSession.Current.LevelId.ToString();
            }
        }

        public string HeadName { get; set; }

    }
    #endregion

    #region Abstract Fund Transferred
    public class AbstractFundTransferredViewModel
    {
        [Required(ErrorMessage = "Please select year.")]
        [Range(1990, 2099, ErrorMessage = "Please select year.")]
        public string Year { get; set; }

        [Required(ErrorMessage = "Please select head.")]
        [Range(1, Int64.MaxValue, ErrorMessage = "Please select head.")]
        public string Head { get; set; }

        [Required(ErrorMessage = "Please select state.")]
        [Range(1, Int64.MaxValue, ErrorMessage = "Please select state.")]
        public string State { get; set; }

        //[Required(ErrorMessage = "Please select DPIU.")]
        //[Range(1, Int64.MaxValue, ErrorMessage = "Please select DPIU.")]
        public string DPIU { get; set; }

        public List<USP_ACC_RPT_REGISTER_ABSTRACT_PIUWISE_FUND_TRANSFERRED_Result> lstAbstractFund { get; set; }

        public string DPIUName { get; set; }

        public string HeadName { get; set; }

        public string YearName { get; set; }

        public string StateName { get; set; }

        public string ReportNumber { get; set; }

        public string ReportName { get; set; }

        public string ReportPara { get; set; }

        public string FundName { get; set; }

    }
    #endregion


    #region Bank Authorization
    public class BankAuthrizationViewModel
    {
        [Display(Name = "Month")]
        [Required(ErrorMessage = "Please select month")]
        [Range(1, 12, ErrorMessage = "Please select month")]
        public string Month { get; set; }

        [Display(Name = "Year")]
        [Required(ErrorMessage = "Please select year")]
        [Range(2000, int.MaxValue, ErrorMessage = "Please select year")]
        public string Year { get; set; }

        [Display(Name = "State")]
        [Required(ErrorMessage = "Please select state")]
        [Range(1, int.MaxValue, ErrorMessage = "Please select state")]
        public string State { get; set; }

        [Display(Name = "DPIU")]

        public string DPIU { get; set; }

        public List<USP_ACC_RPT_REGISTER_PIUWISE_BANK_AUTHORIZATION_ISSUED_Result> lstBankAuthrization { get; set; }

        public string MonthName { get; set; }

        public string YearName { get; set; }

        public string SRRDAName { get; set; }

        public string DPIUName { get; set; }

        public string ReportNumber { get; set; }

        public string ReportName { get; set; }

        public string ReportPara { get; set; }

        public string FundName { get; set; }


    }
    #endregion

    #region Abstract Bank Authorization
    public class AbstractBankAuthViewModel
    {


        [Display(Name = "Year")]
        [Range(2000, int.MaxValue, ErrorMessage = "Please select Year")]
        public int Year { get; set; }

        [Display(Name = "State")]
        [Range(1, int.MaxValue, ErrorMessage = "Please select State")]
        public int SRRDA { get; set; }

        [Display(Name = "DPIU")]
        public int DPIU { get; set; }

        public string DisplayYear { get; set; }

        public string DisplayState { get; set; }

        public string DisplayDPIU { get; set; }

        //Report Header parameter
        public string FundTypeName { get; set; }
        public string ReportName { get; set; }
        public string ReportParagraphName { get; set; }
        public string ReportFormNumber { get; set; }

    }
    #endregion


    #region Financial Progress of Work

    public class FinancialProgressofWork
    {
        [DisplayName("Select Month")]
        //[RequiredMonth("ReportType", ErrorMessage = "Please select Month")]
        [Range(1, 12, ErrorMessage = "Please Select Month")]
        public short Month { get; set; }
        public List<SelectListItem> ListMonth { get; set; }

        [DisplayName("Select Year")]
        [Range(2000, short.MaxValue, ErrorMessage = "Please Select Year")]
        public short Year { get; set; }
        public List<SelectListItem> ListYear { get; set; }

        [DisplayName("SRRDA")]
        [Range(0, int.MaxValue, ErrorMessage = "Please Select Agency")]
        public int Agency { get; set; }
        public List<SelectListItem> ListAgency { get; set; }


        public string Name { get; set; }

        public int LevelId { get; set; }


    }
    #endregion

    #region Register View Model (Statutory Deduction , Deposites , Miscellaneous Advances  )

    public class RegisterViewModel
    {

        public RegisterViewModel()
        {
            lstFinancialYears = new List<SelectListItem>();
            lstHeads = new List<SelectListItem>();
            lstMonths = new List<SelectListItem>();
            lstYears = new List<SelectListItem>();
            lstFundingAgency = new List<SelectListItem>();
            lstPIU = new List<SelectListItem>();
            lstSRRDA = new List<SelectListItem>();
        }

        [Required(ErrorMessage = "Head is required.")]
        [Display(Name = "Head")]
        [Range(1, Int32.MaxValue, ErrorMessage = "Please select proper head.")]
        public int HeadId { get; set; }

        //[Required(ErrorMessage="Funding Agency is required.")]
        [Display(Name = "Funding Agency")]
        //[Range(1,Int32.MaxValue,ErrorMessage="Funding Agency is required.")]
        public int FundingAgencyCode { get; set; }

        [Required(ErrorMessage = "Month is required.")]
        [RequiredDependentMonthlyField("DurationType", ErrorMessage = "Please select Month.")]
        [Display(Name = "Month")]
        public int Month { get; set; }

        [Display(Name = "Year")]
        [RequiredDependentYearField("DurationType", ErrorMessage = "Please select Year.")]
        public int Year { get; set; }

        [Display(Name = "Year")]
        //[RequiredDependentField("DurationType", ErrorMessage = "Please select Year.")]
        //[Range(2000, 2099, ErrorMessage = "Select proper financial year.")]
        [RequiredDependentYearlyField("FinancialYear", ErrorMessage = "Financial Year is required.")]
        public int FinancialYear { get; set; }

        [Display(Name = "Duration")]
        [Required(ErrorMessage = "Duration Type is required.")]

        public string DurationType { get; set; }

        [Display(Name = "Report Type")]
        public string ReportType { get; set; }

        public string ReportTitle { get; set; }

        public List<SelectListItem> lstMonths { get; set; }

        public List<SelectListItem> lstYears { get; set; }

        public List<SelectListItem> lstFinancialYears { get; set; }

        public List<SelectListItem> lstHeads { get; set; }

        public List<SelectListItem> lstFundingAgency { get; set; }

        public List<SelectListItem> lstPIU { get; set; }

        public List<SelectListItem> lstSRRDA { get; set; }

        public string NodalAgency { get; set; }

        public int AdminCode { get; set; }

        public string ReportFormNumber { get; set; }

        public string FundTypeName { get; set; }

        public string ReportName { get; set; }

        public string ReportParagraphName { get; set; }

        public string StateName { get; set; }

        public string SRRDADPIU { get; set; }

        public string HeadName { get; set; }

        public string DPIUName { get; set; }

        public string MonthName { get; set; }

        public int LevelId { get; set; }

        public int StateCode { get; set; }

        public int Collaboration { get; set; }

        //[RequredDPIU("ReportType", ErrorMessage = "Please select DPIU")]
        public int DPIUCode { get; set; }

        public int HeadCategoryId { get; set; }

        public int SRRDACode { get; set; }

    }
    #endregion

    #region Master Sheet
    public class MasterSheetModel
    {


        public int LvlId { get; set; }


        [Display(Name = "State")]
        [RegularExpression(@"^([0-9]+)$", ErrorMessage = "Please select State.")]
        [Required(ErrorMessage = "Please Select State")]
        public int StateCode { get; set; }
        public String StateName { get; set; }
        public List<SelectListItem> listStates { get; set; }


        [DisplayName("Select Agency")]
        [Range(0, int.MaxValue, ErrorMessage = "Please Select Agency")]
        public int Agency { get; set; }
        public List<SelectListItem> ListAgency { get; set; }

        //[DisplayName("Year")]
        //[Required(ErrorMessage = "Please select year.")]
        //[Range(1990, 2099, ErrorMessage = "Please select year")]
        public Int32 Year { get; set; }

        public SelectList YEAR_LIST { get; set; }

        [Display(Name = "Fund Type")]
        [Required(ErrorMessage = "Please Select Fund Type")]
        [RegularExpression("[PAM]", ErrorMessage = "Please Select Fund Type")]
        public string FundType { get; set; }
        public List<SelectListItem> lstFundType { get; set; }

    }
    #endregion

    #region Authorized Signatory

    public class AuthorizedSignatoryModel
    {
        [Display(Name = "Select DPIU")]
        public int DPIU { set; get; }


        /// <summary>
        /// Populate DPIU
        /// </summary>        
        public SelectList ListDPIU
        {
            get
            {

                List<SelectListItem> lstDPIU = new List<SelectListItem>();
                CommonFunctions objCommonFunction = new CommonFunctions();

                //populate DPIU
                PMGSY.Models.Common.TransactionParams objParam = new PMGSY.Models.Common.TransactionParams();
                objParam.ADMIN_ND_CODE = PMGSYSession.Current.AdminNdCode;

                lstDPIU = objCommonFunction.PopulateDPIU(objParam);
                lstDPIU.RemoveAt(0);
                lstDPIU.Insert(0, new SelectListItem() { Value = "0", Text = "All DPIU" });

                return new SelectList(lstDPIU, "Value", "Text");
            }
        }

        public int LevelID { set; get; }


    }
    #endregion

    #region Reconcilliation
    public class ReconciliationModel
    {
        [Display(Name = "Fund")]
        // [Required(ErrorMessage = "Please Select Fund Type")]
        //[RegularExpression("[PA]", ErrorMessage = "Please Select Fund Type")]
        public string FundType { get; set; }
        //public List<SelectListItem> listFundType { get; set; }


        [Display(Name = "State")]
        [RegularExpression(@"^([0-9]+)$", ErrorMessage = "Please select State.")]
        [Required(ErrorMessage = "Please Select State")]
        public int StateCode { get; set; }
        public String StateName { get; set; }
        public List<SelectListItem> listStates { get; set; }

        [DisplayName("Month")]
        [Range(1, 12, ErrorMessage = "Please Select month")]
        [RegularExpression(@"^([0-9]+)$", ErrorMessage = "Please Select valid Month")]
        public int Month { get; set; }
        public List<SelectListItem> listMonth { get; set; }


        [DisplayName("Year")]
        [Range(2000, Int32.MaxValue, ErrorMessage = "Please Select Year")]
        [RegularExpression(@"^([0-9]+)$", ErrorMessage = "Please Select valid year")]
        public int Year { get; set; }
        public List<SelectListItem> listYear { get; set; }

        // Added by Srishti on 18-05-2023
        //[Required(ErrorMessage = "Please select Account Type.")]
        //[RegularExpression(@"^[SHD]?", ErrorMessage = "Please select valid Account Type.")]
        //[Display(Name = "Account Type")]
        //public string BANK_ACC_TYPE { get; set; }
        //public List<SelectListItem> lstBankAccType { get; set; }


    }
    #endregion Reconcilliation

    #region Running Account

    public class RunningAccountViewModel
    {
        public RunningAccountViewModel()
        {
            ddlMonth = new List<SelectListItem>();
            ddlYear = new List<SelectListItem>();
            ddlDPIU = new List<SelectListItem>();
        }

        [Display(Name = "Month")]
        [Required(ErrorMessage = "Month is required.")]
        [Range(1, 12, ErrorMessage = "Please select Month.")]
        public int Month { get; set; }

        [Display(Name = "Year")]
        [Required(ErrorMessage = "Year is required.")]
        [Range(2000, 2099, ErrorMessage = "Please select Year.")]
        public int Year { get; set; }

        [Display(Name = "Balance Type")]
        [Required(ErrorMessage = "Balance Type is required.")]
        [RegularExpression(@"^([CD]+)$", ErrorMessage = "Please select valid Balance Type")]
        public string Balance { get; set; }

        [Display(Name = "Report Type")]
        //[Required(ErrorMessage = "Report Type is required.")]
        [RegularExpression(@"^([SD]+)$", ErrorMessage = "Please select valid Report Type")]
        public string ReportType { get; set; }


        [Display(Name = "SSRS Report")]
        public string SSRSReport { get; set; }

        public int SRRDANDCode { get; set; }

        public List<SelectListItem> ddlMonth { get; set; }

        public List<SelectListItem> ddlYear { get; set; }

        public List<SelectListItem> ddlDPIU { get; set; }

        public List<SelectListItem> ddlBalance
        {
            get
            {
                List<SelectListItem> lstBalances = new List<SelectListItem>();
                lstBalances.Add(new SelectListItem { Value = "0", Text = "Select Balance Type" });
                lstBalances.Add(new SelectListItem { Value = "C", Text = "Credit" });
                lstBalances.Add(new SelectListItem { Value = "D", Text = "Debit" });
                return lstBalances;
            }
        }

        public int? page;
        public int? rows;
        public string sord;
        public string sidx;

        public string ReportFormNumber { get; set; }

        public string FundTypeName { get; set; }

        public string ReportName { get; set; }

        public string ReportParagraphName { get; set; }

        public string StateName { get; set; }

        public string NodalAgency { get; set; }

        public string SRRDADPIU { get; set; }

        public string BalanceName { get; set; }

        public int DPIUCode { get; set; }

        public string MonthName { get; set; }

        public int AdminCode { get; set; }

        public string DPIUName { get; set; }

        public string PreviousMonthName { get; set; }

    }

    #endregion Running Account


    #region Register of Work

    public class RegisterOfWorksModel
    {
        // -------Filter Specific Info
        [Display(Name = "DPIU")]
        [Range(1, Int64.MaxValue, ErrorMessage = "Please select DPIU")]
        public Int32 ADMIN_ND_CODE { get; set; }
        public List<SelectListItem> DEPARTMENT_LIST { get; set; }

        [Display(Name = "Contractor")]
        [Range(1, Int64.MaxValue, ErrorMessage = "Please select contractor.")]
        public Int32 MAST_CON_ID { get; set; }
        public List<SelectListItem> CONTRACTOR_LIST { get; set; }

        [Display(Name = "Agreement")]
        [Range(1, Int64.MaxValue, ErrorMessage = "Please select agreement.")]
        public Int32 TEND_AGREEMENT_CODE { get; set; }
        public List<SelectListItem> AGREEEMENT_LIST { get; set; }

        public Int32 ParentAdminNdCode { get; set; } //To get nodal agency

        // ------Header specific Info

        [Display(Name = "Name Of Agency")]
        public String StateDepartment { get; set; }

        [Display(Name = "Name Of PIU")]
        public String DistrictDepartment { get; set; }

        [Display(Name = "Agreement Date")]
        public string AGREEMENT_DATE { get; set; }

        [Display(Name = "Agreement Amount (in Lacs)")]
        public decimal AGREEMENT_AMOUNT { get; set; }

        [Display(Name = "Agreement No.")]
        public string AGREEMENT_NUMBER { get; set; }

        public string ContractorName { get; set; }
    }

    #endregion Register of Work


    #region Imprest Register

    public class ImprestSettlementViewModel
    {
        public ImprestSettlementViewModel()
        {
            lstFinancialYears = new List<SelectListItem>();
            lstSrrda = new List<SelectListItem>();
            lstDpiu = new List<SelectListItem>();
        }

        [Display(Name = "Report Type")]
        [IsValidReport("SrrdaAdminCode", "DpiuAdminCode", ErrorMessage = "Please select proper options depending on report type.")]
        public string ReportLevel { get; set; }

        [Required(ErrorMessage = "Please select Financial Year.")]
        [Range(2000, 2099, ErrorMessage = "Please select Financial Year.")]
        [Display(Name = "Financial Year")]
        public short FinancialYear { get; set; }

        [Display(Name = "Nodal Agency")]
        public int SrrdaAdminCode { get; set; }

        [Display(Name = "DPIU")]
        public int DpiuAdminCode { get; set; }

        public int AdminCode { get; set; }

        public int LevelId { get; set; }

        public string DPIUName { get; set; }

        public string NodalAgency { get; set; }

        public List<SelectListItem> lstFinancialYears { get; set; }

        public List<SelectListItem> lstSrrda { get; set; }

        public List<SelectListItem> lstDpiu { get; set; }

    }

    public class IsValidReport : ValidationAttribute//, IClientValidatable
    {
        private string DefaultErrorMessage = "Please select proper choice.";
        private readonly string _srrdaCode;
        private readonly string _dpiuCode;

        public IsValidReport(string srrdaCode, string dpiuCode)
        {
            this._srrdaCode = srrdaCode;
            this._dpiuCode = dpiuCode;
        }

        public override string FormatErrorMessage(string name)
        {
            return string.Format(DefaultErrorMessage, name, _srrdaCode);
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            String amountToCompare = String.Empty;

            var propertyTestedSRRDA = validationContext.ObjectType.GetProperty(this._srrdaCode);
            var PropertyTestedDPIU = validationContext.ObjectType.GetProperty(this._dpiuCode);

            if (propertyTestedSRRDA == null)
            {
                return new ValidationResult(string.Format("unknown property {0}", this._srrdaCode));
            }

            if (PropertyTestedDPIU == null)
            {
                return new ValidationResult(string.Format("unknown property {0}", this._dpiuCode));
            }

            var testedValueSrrda = propertyTestedSRRDA.GetValue(validationContext.ObjectInstance, null);
            var testedValueDpiu = PropertyTestedDPIU.GetValue(validationContext.ObjectInstance, null);

            if (PMGSYSession.Current.LevelId == 5)
            {
                return ValidationResult.Success;
            }

            if (value == null)
            {
                return new ValidationResult("Please select proper report type.");
            }


            if (value.ToString().ToLower() == "s")
            {
                if (!String.IsNullOrEmpty(testedValueSrrda.ToString()) && Convert.ToInt32(testedValueSrrda) != 0)
                {
                    return ValidationResult.Success;
                }
                else
                {
                    return new ValidationResult("Please select SRRDA.");
                }
            }

            if (value.ToString().ToLower() == "d")
            {
                if (!String.IsNullOrEmpty(testedValueSrrda.ToString()) && Convert.ToInt32(testedValueSrrda) != 0)
                {
                    if (!String.IsNullOrEmpty(testedValueDpiu.ToString()) && Convert.ToInt32(testedValueDpiu) != 0)
                    {
                        return ValidationResult.Success;
                    }
                    else
                    {
                        return new ValidationResult("Please select DPIU.");
                    }
                }
                else
                {
                    return new ValidationResult("Please select SRRDA.");
                }
            }

            return ValidationResult.Success;
        }

        public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
        {
            var rule = new ModelClientValidationRule
            {
                ErrorMessage = this.ErrorMessageString,
                ValidationType = "isvalidreport"
            };
            yield return rule;
        }
    }

    #endregion Imprest Register

    #region AbstractSheduleOfRoads

    public class AbstractSheduleOfRoads
    {
        [Display(Name = "Nodal Agency")]
        [Range(1, Int32.MaxValue, ErrorMessage = "Please select Agency")]
        public int AdminNdCode { get; set; }

        [Display(Name = "Month")]
        [Range(1, Int32.MaxValue, ErrorMessage = "Please select Month")]
        public int Month { get; set; }

        [Display(Name = "Year")]
        [Range(1, Int32.MaxValue, ErrorMessage = "Please select Year")]
        public int Year { get; set; }

        [Display(Name = "Year")]
        [Range(1, Int32.MaxValue, ErrorMessage = "Please select Year")]
        public int FinancialYear { get; set; }

        [Display(Name = "Head")]
        [Range(1, Int32.MaxValue, ErrorMessage = "Please select Head")]
        public int HeadId { get; set; }

        public String MonthlyYearly { get; set; }

        public SelectList PopulateAgency
        {
            get
            {
                CommonFunctions commomFuncObj = new CommonFunctions();
                if (PMGSYSession.Current.LevelId == 4)
                {
                    List<SelectListItem> lst = commomFuncObj.PopulateNodalAgencies(PMGSYSession.Current.StateCode);
                    lst = lst.Where(m => m.Value == PMGSYSession.Current.AdminNdCode.ToString()).ToList();
                    return new SelectList(lst, "Value", "Text");
                }
                else if (PMGSYSession.Current.LevelId == 6)
                {
                    return new SelectList(commomFuncObj.PopulateNodalAgencies(), "Value", "Text");
                }
                return null;
            }
        }

        public SelectList PopulateMonth
        {
            get
            {
                CommonFunctions objCom = new CommonFunctions();
                return new SelectList(objCom.PopulateMonths(), "Value", "Text");
            }
        }

        public SelectList PopulateYears
        {
            get
            {
                CommonFunctions objCom = new CommonFunctions();
                return new SelectList(objCom.PopulateYears(), "Value", "Text");
            }
        }


        public SelectList PopulateFinYears
        {
            get
            {
                CommonFunctions objCom = new CommonFunctions();
                return new SelectList(objCom.PopulateFinancialYear(), "Value", "Text");
            }
        }

        public SelectList PopulateHead
        {
            get
            {
                //return new SelectList(objCom.PopulateFinancialYear(), "Value", "Text");
                PMGSYEntities dbContext = new PMGSYEntities();
                List<SelectListItem> lstHead = new List<SelectListItem>();
                //var Heads = dbContext.ACC_MASTER_HEAD.Where(m => m.HEAD_CODE == "11.01" || m.HEAD_CODE == "11.02" || m.HEAD_CODE == "11.03" || m.HEAD_CODE == "11.04" || m.HEAD_CODE == "11.05" || m.HEAD_CODE == "11.06" || m.HEAD_CODE == "11.07").ToList();
              //  var Heads = dbContext.ACC_MASTER_HEAD.Where(m => m.HEAD_ID == 28 || m.HEAD_ID == 29 || m.HEAD_ID == 30 || m.HEAD_ID == 31 || m.HEAD_ID == 385 || m.HEAD_ID == 386 || m.HEAD_ID == 409 || m.HEAD_ID == 427 || m.HEAD_ID == 428 || m.HEAD_ID == 429 || m.HEAD_ID == 430).ToList();
                var Heads = dbContext.ACC_MASTER_HEAD.Where(m => m.HEAD_ID == 28 || m.HEAD_ID == 29 || m.HEAD_ID == 30 || m.HEAD_ID == 31 || m.HEAD_ID == 385 || m.HEAD_ID == 386 || m.HEAD_ID == 409 || m.HEAD_ID == 427 || m.HEAD_ID == 428 || m.HEAD_ID == 429 || m.HEAD_ID == 430 || m.HEAD_ID == 464 || m.HEAD_ID == 465).ToList();
                foreach (var item in Heads)
                {
                    lstHead.Add(new SelectListItem { Text = item.HEAD_CODE + " - " + item.HEAD_NAME, Value = item.HEAD_ID.ToString() });
                }
                lstHead.Insert(0, new SelectListItem { Text = "Select Head", Value = "0" });
                return new SelectList(lstHead, "Value", "Text");
                //return new SelectList(dbContext.ACC_MASTER_HEAD.Where(m => m.HEAD_CODE == "11.01" || m.HEAD_CODE == "11.02" || m.HEAD_CODE == "11.03" || m.HEAD_CODE == "11.04" || m.HEAD_CODE == "11.05" || m.HEAD_CODE == "11.06" || m.HEAD_CODE == "11.07"), "HEAD_ID", "HEAD_NAME");
            }
        }

    }

    #endregion AbstractSheduleOfRoads

    #region Month Revoke Report
    public class MonthRevokeModel
    {
        [DisplayName("From Month")]
        //  [Range(1, 12, ErrorMessage = "Please Select Month")]
        //[RequiredMonth("ReportType", ErrorMessage = "Please select From Month")]

        public short Month { get; set; }
        public List<SelectListItem> ListMonth { get; set; }

        [DisplayName("From Year")]
        // [Range(2000, short.MaxValue, ErrorMessage = "Please Select From Year")]
        public short Year { get; set; }
        public List<SelectListItem> ListYear { get; set; }

        public int UserID;
        public string FundTye;
        public string DurationFlag;
        //
        [DisplayName("To Month")]
        //[Range(1, 12, ErrorMessage = "Please Select Month")]
        //   [RequiredMonth("ReportType", ErrorMessage = "Please select To Month")]
        public short ToMonth { get; set; }
        public List<SelectListItem> ListToMonth { get; set; }

        [DisplayName("To Year")]
        //[Range(2000, short.MaxValue, ErrorMessage = "Please Select To Year")]
        public short ToYear { get; set; }
        public List<SelectListItem> ListToYear { get; set; }
        //


        [DisplayName("PIU")]
        public int Piu { get; set; }
        public List<SelectListItem> ListPiu { get; set; }

        [DisplayName("Agency")]
        [Range(0, int.MaxValue, ErrorMessage = "Please Select Agency")]
        public int Agency { get; set; }
        public List<SelectListItem> ListAgency { get; set; }

        public int LevelId { get; set; }
        public int ReportType { get; set; }
        public int ReportLevel { get; set; }
        public int ReportID { get; set; }

        public string FundType { get; set; }
        public string SRRDA_DPIU { get; set; }
        public int? SrrdaNdCode { get; set; }
        public string RoadReportType { get; set; }

        //Report Header  Used in report (Incidental Fund)
        public string FormNumber { get; set; }
        public string ReportHeader { get; set; }
        public string Refference { get; set; }
        public string ScheduleNumber { get; set; }
    }
    #endregion

    #region PFMS MIS Payment summary
    public class PfmsMisPaymentModel
    {

        [Display(Name = "SRRDA")]
        public int SRRDA { get; set; }

        public string SRRDA_DPIU { get; set; }

        [Display(Name = "DPIU")]
        //[RequiredDPIU("NodalAgency", ErrorMessage = "Please Select DPIU")]
        public int DPIU { get; set; }

        [Display(Name = "NRRDA")]
        public string NRRDA { get; set; }

        [Display(Name = "Month")]
        [RequiredMonthForMonthly("rType", ErrorMessage = "Please select Month")]
        public short Month { get; set; }

        [Display(Name = "Year")]
        [RequiredYearForMonthly("rType", ErrorMessage = "Please select Year")]
        [RequiredYearForYearly("rType", ErrorMessage = "Please select Year")]
        public short Year { get; set; }

        [Display(Name = "Periodic")]
        public string Period { get; set; }

        public string FundType { get; set; }

        [Display(Name = "Start Date")]
        [RegularExpression(@"^(((0[1-9]|[12]\d|3[01])\/(0[13578]|1[02])\/((19|[2-9]\d)\d{2}))|((0[1-9]|[12]\d|30)\/(0[13456789]|1[012])\/((19|[2-9]\d)\d{2}))|((0[1-9]|1\d|2[0-8])\/02\/((19|[2-9]\d)\d{2}))|(29\/02\/((1[6-9]|[2-9]\d)(0[48]|[2468][048]|[13579][26])|((16|[2468][048]|[3579][26])00))))$", ErrorMessage = "Start Date is not in valid format")]
        // [RequiredStartDateForPeriodic("rType", ErrorMessage = "Start Date is required.")]
        public string StartDate { get; set; }

        [Display(Name = "End Date")]
        [RegularExpression(@"^(((0[1-9]|[12]\d|3[01])\/(0[13578]|1[02])\/((19|[2-9]\d)\d{2}))|((0[1-9]|[12]\d|30)\/(0[13456789]|1[012])\/((19|[2-9]\d)\d{2}))|((0[1-9]|1\d|2[0-8])\/02\/((19|[2-9]\d)\d{2}))|(29\/02\/((1[6-9]|[2-9]\d)(0[48]|[2468][048]|[13579][26])|((16|[2468][048]|[3579][26])00))))$", ErrorMessage = "End Date is not in valid format")]
        [AssetDateValidation("StartDate", ErrorMessage = "End Date must be greater than or equal to start date.")]
        [AssetCurrentDateValidation("StartDate", ErrorMessage = "End date must be less than or equals to todays date.")]
        // [RequiredEndDateForPeriodic("rType", ErrorMessage = "End Date is required.")]
        public string EndDate { get; set; }

        public string rType { get; set; }

        public int AdminNdCode { get; set; }

        public string MonthName { get; set; }
        public string DPIUName { get; set; }

        public int levelId { get; set; }

        public string NodalAgency { get; set; }
        public string DPIUByNO { get; set; }

        public int TotalRecords { get; set; }

        public string DPIUBySRRDA { get; set; }

        public List<SelectListItem> ddlDPIU { get; set; }

        public List<SelectListItem> States { get; set; }

        public List<SP_ACC_RPT_DISPALY_Bill_DETAILS_Result> lstAccountBillDetails { get; set; }

        //Added By 
        public List<SelectListItem> MonthList { get; set; }
        public List<SelectListItem> YearList { get; set; }

        public int isAllPiu { get; set; }
        public string YearName { get; set; }
        public string StateName { get; set; }
        public string Selection { get; set; }

        //added by abhinav for redirection parameter.
        public bool isRedirected{ get; set; }

    }

    #endregion

    #region PFMS Pending Bills
    public class PfmsPendingBills
    {

        [Display(Name = "SRRDA")]
        public int SRRDA { get; set; }

        public string SRRDA_DPIU { get; set; }

        [Display(Name = "DPIU")]
        //[RequiredDPIU("NodalAgency", ErrorMessage = "Please Select DPIU")]
        public int DPIU { get; set; }

        [Display(Name = "NRRDA")]
        public string NRRDA { get; set; }

        [Display(Name = "Month")]
        [RequiredMonthForMonthly("rType", ErrorMessage = "Please select Month")]
        public short Month { get; set; }

        [Display(Name = "Year")]
        [RequiredYearForMonthly("rType", ErrorMessage = "Please select Year")]
        [RequiredYearForYearly("rType", ErrorMessage = "Please select Year")]
        public short Year { get; set; }

        [Display(Name = "Periodic")]
        public string Period { get; set; }

        public string FundType { get; set; }

        [Display(Name = "Start Date")]
        [Required(ErrorMessage = "Select Start Date")]
        [RegularExpression(@"^(((0[1-9]|[12]\d|3[01])\/(0[13578]|1[02])\/((19|[2-9]\d)\d{2}))|((0[1-9]|[12]\d|30)\/(0[13456789]|1[012])\/((19|[2-9]\d)\d{2}))|((0[1-9]|1\d|2[0-8])\/02\/((19|[2-9]\d)\d{2}))|(29\/02\/((1[6-9]|[2-9]\d)(0[48]|[2468][048]|[13579][26])|((16|[2468][048]|[3579][26])00))))$", ErrorMessage = "Start Date is not in valid format")]
        // [RequiredStartDateForPeriodic("rType", ErrorMessage = "Start Date is required.")]
        public string StartDate { get; set; }

        [Display(Name = "End Date")]
        [Required(ErrorMessage = "Select End Date")]
        [RegularExpression(@"^(((0[1-9]|[12]\d|3[01])\/(0[13578]|1[02])\/((19|[2-9]\d)\d{2}))|((0[1-9]|[12]\d|30)\/(0[13456789]|1[012])\/((19|[2-9]\d)\d{2}))|((0[1-9]|1\d|2[0-8])\/02\/((19|[2-9]\d)\d{2}))|(29\/02\/((1[6-9]|[2-9]\d)(0[48]|[2468][048]|[13579][26])|((16|[2468][048]|[3579][26])00))))$", ErrorMessage = "End Date is not in valid format")]
        [AssetDateValidation("StartDate", ErrorMessage = "End Date must be greater than or equal to start date.")]
        [AssetCurrentDateValidation("StartDate", ErrorMessage = "End date must be less than or equals to todays date.")]
        // [RequiredEndDateForPeriodic("rType", ErrorMessage = "End Date is required.")]
        public string EndDate { get; set; }

        public string rType { get; set; }

        public int AdminNdCode { get; set; }

        public string MonthName { get; set; }
        public string DPIUName { get; set; }

        public int levelId { get; set; }

        public string NodalAgency { get; set; }
        public string DPIUByNO { get; set; }

        public int TotalRecords { get; set; }

        public string DPIUBySRRDA { get; set; }

        public List<SelectListItem> ddlDPIU { get; set; }

        public List<SelectListItem> States { get; set; }

        public List<SP_ACC_RPT_DISPALY_Bill_DETAILS_Result> lstAccountBillDetails { get; set; }

        //Added By 
        public List<SelectListItem> MonthList { get; set; }
        public List<SelectListItem> YearList { get; set; }

        public int isAllPiu { get; set; }
        public string YearName { get; set; }
        public string StateName { get; set; }
        public string Selection { get; set; }

    }
    #endregion

    public class RoadWiseCompleted
    {

        public RoadWiseCompleted()
        {

            lstscheme = new List<SelectListItem>();


        }
        public int LevelCode { get; set; }
        public string StateName { get; set; }
        public string DistName { get; set; }
        public string BlockName { get; set; }
        public int Mast_State_Code { get; set; }
        public int Mast_Block_Code { get; set; }
        public int Mast_District_Code { get; set; }

        [Display(Name = "State : ")]
        [Required(ErrorMessage = "Please select State. ")]
        [Range(1, int.MaxValue, ErrorMessage = "Please select  State.")]

        public int StateCode { get; set; }
        public List<SelectListItem> StateList { get; set; }

        [Display(Name = "District : ")]
        [Required(ErrorMessage = "Please select District.")]
        [Range(0, int.MaxValue, ErrorMessage = "Please select District.")]

        public int DistrictCode { get; set; }
        public List<SelectListItem> DistrictList { get; set; }

        [Display(Name = "Block : ")]
        [Required(ErrorMessage = "Please select Block.")]
        [Range(0, int.MaxValue, ErrorMessage = "Please select Block.")]

        public int BlockCode { get; set; }
        public List<SelectListItem> BlockList { get; set; }

        [Display(Name = "Completion Year : ")]
        [Range(1, 2090, ErrorMessage = "Please select Year.")]
        [Required(ErrorMessage = "Please select Year.")]
        [RegularExpression("^([0-9]+)$", ErrorMessage = "Year must be valid number.")]
        public int Year { get; set; }
        public List<SelectListItem> YearList { get; set; }

        public List<SelectListItem> lstscheme { get; set; }
        [Display(Name = "Scheme")]
        public int schemeCode { get; set; }

        public int sessionStatecode { get; set; }

        public bool Districtwise { get; set; }

    }

    public class ANNEXURE
    {

        public ANNEXURE()
        {

            lstscheme = new List<SelectListItem>();


        }

        public int sessionStatecode { get; set; }

        public bool Districtwise { get; set; }

        [DisplayName("Select Year")]
        [Range(0, short.MaxValue, ErrorMessage = "Please Select Year")]
        public short Year { get; set; }
        public List<SelectListItem> ListYear { get; set; }


        public List<SelectListItem> lstscheme { get; set; }
        [Display(Name = "Scheme")]
        public int schemeCode { get; set; }

    }

    public class NewCashbookModel
    {
        [Display(Name = "Month")]
        [Required(ErrorMessage = "Please select month.")]
        [Range(1, 12, ErrorMessage = "Please select month.")]
        public Int16 Month { get; set; }

        //public List<SelectListItem> lstMonth { set; get; }

        [Display(Name = "Year")]
        [Required(ErrorMessage = "Please select year.")]
        [Range(1990, 2099, ErrorMessage = "Please select year.")]
        public Int16 Year { get; set; }

        public string SRRDA_DPIU { get; set; }

        [Display(Name = "SRRDA")]
        public int SRRDA { get; set; }

        [Display(Name = "DPIU")]
        //[Range(1,int.MaxValue,ErrorMessage="Please Select DPIU")]
        public int DPIU { get; set; }

        public int ADMIN_ND_CODE { get; set; }
        public int LvlId { get; set; }

        public string CashbookType { get; set; }
    }
}
