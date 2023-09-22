using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace PMGSY.Models.QualityMonitoring
{
    public class QMTourViewModel
    {
        public int TourId { get; set; }
        public string Operation { get; set; }

        [Display(Name = "Monitor")]
        public string MonitorName { get; set; }

        [Display(Name = "Inspection Month and Year")]
        public string InspMonthYear { get; set; }

        [Display(Name = "State To be visited")]
        public string StateName { get; set; }

        public string DistrictName1 { get; set; }
        public string DistrictName2 { get; set; }
        public string DistrictName3 { get; set; }

        public int AdminScheduleCode { get; set; }

        [Display(Name = "Arrival Date in State")]
        [Required]
        public string FlightArrivalDate { get; set; }

        [Display(Name = "Arrival Time in State")]
        [Required]
        public string FlightArrivalTime { get; set; }

        [Display(Name = "Departure Date in State")]
        [Required]
        public string FlightDepartureDate { get; set; }

        [Display(Name = "Departure Time in State")]
        [Required]
        public string FlightDepartureTime { get; set; }

        public bool IsAlreadyEntered { get; set; }

        public string ScheduleMonthYearStartDate { get; set; }
        public string CurrentDate { get; set; }

        public int RoleCode { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Please enter valid expenditure")]
        [Display(Name="Total Expenditure")]
        public int totExpenditure { get; set; }

        [Required(ErrorMessage = "Report is required.")]
        public string tourReport { get; set; }

        [Required(ErrorMessage = "Submission Date is required.")]
        public string tourSubmissionDate { get; set; }
    }
}