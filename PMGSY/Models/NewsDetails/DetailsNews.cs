using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace PMGSY.Models.NewsDetails
{
    public class DetailsNews
    {
        [Display(Name = "Month")]
        [Range(1, int.MaxValue, ErrorMessage = "Please Select Month")]
        public int MONTHs { get; set; }
        public List<SelectListItem> Months_List { set; get; }

        [Display(Name = "Year")]
        public int YEARs { get; set; }
        public List<SelectListItem> Years_List { set; get; }

        [Display(Name = "State")]
        public string State { get; set; }
        public List<SelectListItem> State_List { set; get; }

        [Display(Name = "Category")]
        public string Category { get; set; }
        public List<SelectListItem> Category_List { set; get; }

        [Display(Name = "Approved")]
        public string Approved { get; set; }
        public List<SelectListItem> Approved_List { set; get; }

        [Display(Name = "Status")]
        public string Status { get; set; }
        public List<SelectListItem> Status_List { set; get; }

        [Display(Name = "NRRDA")]
        public string NRRDA { get; set; }
        public List<SelectListItem> Nrrda_List { set; get; }

        //[Display(Name = "SRRDA")]
        public string rdbNS { get; set; }
        
        [Display(Name = "SRRDA")]
        public string SRRDA { get; set; }
        public List<SelectListItem> Srrda_List { set; get; }

        [Display(Name = "DPIU")]
        public string DPIU { get; set; }
        public List<SelectListItem> Dpiu_List { set; get; }

        public int hdnRole { get; set; }

    }
}