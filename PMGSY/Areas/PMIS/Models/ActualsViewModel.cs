using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PMGSY.Areas.PMIS.Models
{
    public class AddActualsViewModel
    {
        public int IMS_PR_ROAD_CODE { get; set; }
        public string ACTIVITY_DESC { get; set; }
        public string ACTIVITY_UNIT { get; set; }
        public Nullable<decimal> QUANTITY { get; set; }
        public Nullable<decimal> ACTUAL_QUANTITY { get; set; }
        public Nullable<decimal> AGREEMENT_COST { get; set; }
        //[DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public Nullable<DateTime> PLANNED_START_DATE { get; set; }
        //[DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public Nullable<DateTime> PLANNED_COMPLETION_DATE { get; set; }
        public string STARTED { get; set; }
        // [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public Nullable<DateTime> STARTED_DATE { get; set; }
        public string FINISHED { get; set; }
        //[DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public Nullable<DateTime> FINISHED_DATE { get; set; }
        public string SCHEDULE { get; set; }
        public decimal CompletedRoadLength { get; set; }
        public char ProjectStatus { get; set; }
        public Nullable<DateTime> Date_of_progress_entry { get; set; }

        public string QUANTITY_APPL { get; set; }
        public string AGRCOST_APPL { get; set; }
        public string PLANNED_START_DATE_APPL { get; set; }
        public string PLANNED_COMPLETION_DATE_APPL { get; set; }
        public string Remarks { get; set; }
        //[Display(Name = "ProjectStatus")]
        //[Required(ErrorMessage = "Please Select Project Status")]
        //[RegularExpression(@"^([0-9]+)$", ErrorMessage = "Please select Project Status.")]
        // public int ProjectStatusCode { get; set; }
        // public List<SelectListItem> ProjectStatus { get; set; }
        //public string ProjectStatus { get; set; }
        //        Dictionary<char, string> ProjectStatus = new Dictionary<char, string>(){
        //    {'P', "In Progress"},
        //    {'C', "Completed"},
        //    {'A', "Pending: Land Acquisition"},
        //    {'A', "Pending: Land Acquisition"},
        //    {'F', "Pending: Forest Clearance"},
        //    {'L', "Pending:Legal Cases"}
        //};
    }

    public class AddChainageViewModel
    {
        public int SanctionedLength { get; set; }
        public int IMS_PR_ROAD_CODE { get; set; }
       // public string Date_of_Chainage_entry { get; set; }
        [Required] //(ErrorMessage = "Date of Chainage details is required.")
        public Nullable<DateTime> Date_of_Chainage_entry { get; set; }
        public List<SelectListItem> earthworklist { get; set; }
        public List<SelectListItem> subgradelist { get; set; }
        public List<SelectListItem> granularsubbaselist { get; set; }
        public List<SelectListItem> wbmgrading2list { get; set; }
        public List<SelectListItem> wbmgrading3list { get; set; }
        public List<SelectListItem> wetmixmacadamlist { get; set; }
        public List<SelectListItem> bituminousmacadamlist { get; set; }
        public List<SelectListItem> surfacecourselist { get; set; }
    }


    // New Class Added by Saurabh on 08-06-2023 for FDR Development
    public class AddFDRStabilizeModel
    {
        public int IMS_PR_ROAD_CODE { get; set; }

        public decimal Sanction_length { get; set; }
        public int ROW_LENGTH { get; set; }

        [RegularExpression(pattern: @"^(?:\d{0,3}\.\d{1,2})$|^\d{0,3}$", ErrorMessage = "Chainage cant be greater than 999.99")]

        public List<SelectListItem> START_CHAINAGE_1 { get; set; }

        [RegularExpression(pattern: @"^(?:\d{0,3}\.\d{1,2})$|^\d{0,3}$", ErrorMessage = "Chainage cant be greater than 999.99")]
        public List<SelectListItem> END_CHAINAGE_1 { get; set; }

        [RegularExpression(pattern: @"^(?:\d{0,3}\.\d{1,2})$|^\d{0,3}$", ErrorMessage = "Chainage cant be greater than 999.99")]
        public List<SelectListItem> START_CHAINAGE_2 { get; set; }


        [RegularExpression(pattern: @"^(?:\d{0,3}\.\d{1,2})$|^\d{0,3}$", ErrorMessage = "Chainage cant be greater than 999.99")]
        public List<SelectListItem> END_CHAINAGE_2 { get; set; }

        public Nullable<DateTime> Entry_Date { get; set; }

        //public List<SelectListItem> Chainage_Date_FirstChainage { get; set; }

        //public List<SelectListItem> Chainage_Date_SecondChainage { get; set; }

        public string IS_SUBMIT { get; set; }

        public List<AddFDRStabilizeListModel> AddFDRStabilizeListModelObj { get; set; }

    }

    public class AddFDRStabilizeListModel
    {
        public Nullable<decimal> CHAINAGE_FROM { get; set; }

        public Nullable<decimal> CHAINAGE_TO { get; set; }

        [RegularExpression(pattern: @"^(?:\d{0,3}\.\d{1,2})$|^\d{0,3}$", ErrorMessage = "Chainage cant be greater than 999.99")]
        public Nullable<decimal> START_CHAINAGE_1 { get; set; }


        [RegularExpression(pattern: @"^(?:\d{0,3}\.\d{1,2})$|^\d{0,3}$", ErrorMessage = "Chainage cant be greater than 999.99")]
        public Nullable<decimal> END_CHAINAGE_1 { get; set; }


        [RegularExpression(pattern: @"^(?:\d{0,3}\.\d{1,2})$|^\d{0,3}$", ErrorMessage = "Chainage cant be greater than 999.99")]
        public Nullable<decimal> START_CHAINAGE_2 { get; set; }


        [RegularExpression(pattern: @"^(?:\d{0,3}\.\d{1,2})$|^\d{0,3}$", ErrorMessage = "Chainage cant be greater than 999.99")]
        public Nullable<decimal> END_CHAINAGE_2 { get; set; }


        // [RegularExpression(pattern: @"^([0]?[1-9]|[12][0-9]|[3][01])\/([0]?[1-9]|[1][0-2])\/([0-9]{4})$", ErrorMessage = "Please Enter Valid date{in dd/mm/yyyy format}")]
        public string Chainage_Date_FirstChainage { get; set; }

        // [RegularExpression(pattern: @"^([0]?[1-9]|[12][0-9]|[3][01])\/([0]?[1-9]|[1][0-2])\/([0-9]{4})$", ErrorMessage = "Please Enter Valid date{in dd/mm/yyyy format}")]
        public string Chainage_Date_SecondChainage { get; set; }




    }

    // Changes Ended by Saurabh..
}