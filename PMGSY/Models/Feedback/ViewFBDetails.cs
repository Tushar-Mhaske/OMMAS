using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace PMGSY.Models.Feedback
{
    public class ViewFBDetails
    {

        public string AllowToContact { get; set; }
        //[Display(Name = "Feedback Type")]
        public string FB_Type { set; get; }

        //[Display(Name = "Name")]
        public string FName { get; set; }

        [Display(Name = "Telephone")]
        public string FTel { get; set; }

        [Display(Name = "Mobile")]
        public string FMob { get; set; }

        [Display(Name = "Email")]
        public string FEmail { get; set; }

        [Display(Name = "Date")]
        public string FDate { get; set; }

        [Display(Name = "Category")]
        public string FCategory { get; set; }

        [Display(Name = "Against")]
        public string FAgainst { get; set; }

        [Display(Name = "State")]
        public string FState { get; set; }

        [Display(Name = "District")]
        public string FDistrict { get; set; }

        [Display(Name = "Block")]
        public string FBlock { get; set; }

        [Display(Name = "For")]
        public string FFor { get; set; }

        [Display(Name = "Comments")]
        public string FComments { get; set; }

        [Display(Name = "PMGSY Roads")]
        public string PMGSYRoads { get; set; }

        [Display(Name = "Village Name")]
        public string VillageName { get; set; }

        [Display(Name = "Nearest Habitation")]
        public string NearestHabitation { get; set; }

        [Display(Name = "Road Name")]
        public string RoadName { get; set; }

        public string hdnfeedId { get; set; }

        public string RH_Name { get; set; }
    }
}