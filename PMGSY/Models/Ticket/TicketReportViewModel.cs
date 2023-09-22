using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace PMGSY.Models.Ticket
{
    public class TicketReportViewModel
    {
        [Required(ErrorMessage = "Please select State")]
        [Range(0, int.MaxValue, ErrorMessage = "Please select valid State")]
        [Display(Name = "State")]
        public int state { get; set; }
        public List<SelectListItem> stateList { get; set; }

        [Required(ErrorMessage = "Please select Designation")]
        [Range(0, int.MaxValue, ErrorMessage = "Please select valid Designation")]
        [Display(Name = "Designation")]
        public int Designation { get; set; }
        public List<SelectListItem> DesignationList { get; set; }

        [Display(Name = "Category :")]
        [Required(ErrorMessage = "Please select category")]
        [Range(0, 4, ErrorMessage = "Please select a valid category")]
        public int Category { get; set; }
        public List<SelectListItem> lstCategory { set; get; }


        [Display(Name = "Module")]
        [Required(ErrorMessage = "Please enter module")]
        [Range(0, int.MaxValue, ErrorMessage = "Please select a valid module")]
        public int ModuleID { get; set; }
        public List<SelectListItem> LstModule { set; get; }

        [RegularExpression("[012]", ErrorMessage = "Please select valid level")]
        [Required(ErrorMessage="Please select level")]
        public string Level { get; set; }

        public String TotalTicket { get; set; }

        public String TotalApprovedTicket { get; set; }

        public String TotalClosedTicket { get; set; }

        public String TotalNotClosedTicket { get; set; }

        public String  PartialClosedTicket { get; set; }

        public String InProgressTicket { get; set; }

        public String OpenedTicket { get; set; }

        public String NotOpenedTicket { get; set; }

    }
}