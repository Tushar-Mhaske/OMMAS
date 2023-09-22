#region File Header
/*
        * Project Id    :
        * Project Name  :   OMMAS II
        * Name          :   QMReportsViewModel.cs        
        * Description   :   All filters for Quality Reports will be loaded using this model
        * Author        :   Shyam Yadav 
        * Creation Date :   02/Dec/2013
 **/
#endregion

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PMGSY.Models.QualityMonitoring
{
    public class QMReportsViewModel
    {
        [Display(Name = "From Year")]
        public int FROM_YEAR { get; set; }
        public List<SelectListItem> FROM_YEAR_LIST { set; get; }

        [Display(Name = "To Year")]
        public int TO_YEAR { get; set; }
        public List<SelectListItem> TO_YEAR_LIST { set; get; }

        [Display(Name = "From Month")]
        public int FROM_MONTH { get; set; }
        public List<SelectListItem> FROM_MONTH_LIST { set; get; }

        [Display(Name = "To Month")]
        public int TO_MONTH { get; set; }
        public List<SelectListItem> TO_MONTH_LIST { set; get; }

        [Display(Name = "Year")]
        public int YEAR { get; set; }
        public List<SelectListItem> YEAR_LIST { set; get; }

        [Display(Name = "Month")]
        public int MONTH { get; set; }
        public List<SelectListItem> MONTH_LIST { set; get; }

        [Display(Name = "State")]
        public int STATE { get; set; }
        public List<SelectListItem> STATE_LIST { set; get; }

        [Display(Name = "District")]
        public int DISTRICT { get; set; }
        public List<SelectListItem> DISTRICT_LIST { set; get; }

        [Display(Name = "Grading Item")]
        public int ITEM { get; set; }
        public List<SelectListItem> ITEM_LIST { set; get; }

        public int GRADE { get; set; }

        [Display(Name = "Monitor Type")]
        public string QM_TYPE { get; set; }
        public List<SelectListItem> QM_TYPE_LIST { set; get; }

    }

    public class DefectiveGraphFiltersModel
    {
        [Display(Name = "State")]
        public int StateCode { get; set; }
        public List<SelectListItem> StateList { get; set; }

        [Display(Name = "Year")]
        public int Year { get; set; }
        public List<SelectListItem> YearList { get; set; }

        [Display(Name = "Road Status")]
        public string RdStatus { get; set; }
        public List<SelectListItem> RdStatusList { get; set; }

        public string ValueType { get; set; }
    }

    public class DefectiveGradingLineChartModel
    {
        public string Quarter { get; set; }
        public decimal NQMSRICount { get; set; }
        public decimal NQMUCount { get; set; }
        public decimal SQMSRICount { get; set; }
        public decimal SQMUCount { get; set; }
    }
}