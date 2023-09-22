#region File Header
/*
        * Project Id    :
        * Project Name  :   OMMAS II
        * Name          :   QMFIlterViewModel.cs        
        * Description   :   All filters will be loaded using this model
        * Author        :   Shyam Yadav 
        * Creation Date :   10/Jun/2013
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
    public class QMFilterViewModel
    {
        public QMFilterViewModel()
        {
            QM_TYPES = new List<SelectListItem>();
            STATES = new List<SelectListItem>();
            MONITORS = new List<SelectListItem>();
        }

        public int UserLevelID { get; set; }
        public int RoleID { get; set; }

        [Display(Name = "Type")]
        public string QM_TYPE_CODE { set; get; }
        public List<SelectListItem> QM_TYPES { set; get; }

        [Display(Name = "State")]
        public int MAST_STATE_CODE { get; set; }
        public List<SelectListItem> STATES { get; set; }

        [Display(Name = "Monitor")]
        public int ADMIN_QM_CODE { get; set; }
        public List<SelectListItem> MONITORS { set; get; }

        [Display(Name = "Month")]
        public int FROM_MONTH { get; set; }
        public List<SelectListItem> FROM_MONTHS_LIST { set; get; }

        [Display(Name = "Month")]
        public int TO_MONTH { get; set; }
        public List<SelectListItem> TO_MONTHS_LIST { set; get; }

        [Display(Name = "Year")]
        public int FROM_YEAR { get; set; }
        public List<SelectListItem> FROM_YEARS_LIST { set; get; }

        [Display(Name = "Year")]
        public int TO_YEAR { get; set; }
        public List<SelectListItem> TO_YEARS_LIST { set; get; }

        [Display(Name = "ATR Status")]
        public string ATR_STATUS { get; set; }
        public List<SelectListItem> ATR_STATUS_LIST { set; get; }

        [Display(Name = "Work Status ")]      //    30 - 06 - 2022  vikky 
        public string ROAD_STATUS { get; set; }
        public List<SelectListItem> ROAD_STATUS_LIST { get; set; }

        [Display(Name = "Duration")]
        public int ATR_SUBMIT_DURATION { get; set; }
        public List<SelectListItem> ATR_SUBMIT_DURATION_LIST { get; set; }

        [Display(Name = "Empanelled")]
        public string IsEmpanelled { get; set; }
        public List<SelectListItem> EmpanelledList { get; set; }


        [Display(Name = "Scheme")]
        [Range(0, 4, ErrorMessage = "Invalid Scheme is Selected.")]
        [RegularExpression("^([0-9]+)$", ErrorMessage = "Scheme is Invalid.")]
        public int schemeType { get; set; }
        public List<SelectListItem> schemeList { get; set; } //ATR_Change

        public string imsSanctioned { get; set; }
        public List<SelectListItem> imsSanctionedList { get; set; }

        //    30 - 06 - 2022  vikky 

        [Display(Name = "Road/Bridge")]
        public string roadOrBridge { get; set; }
        public List<SelectListItem> roadOrBridgeList { get; set; }

        [Display(Name = "QM Type ")]
        public string qmType { get; set; }



        [Display(Name = "Overall Grade")]
        public string gradeType { get; set; }
        public List<SelectListItem> gradeTypeList { get; set; }

        [Display(Name = "e-Form Status")]
        public string eFormStatusType { get; set; }
        public List<SelectListItem> eFormStatusTypeList { get; set; }
        //    30 - 06 - 2022  vikky 

        [Display(Name = "Techinal Expert")]
        public int TechnicalExpertID { get; set; }
        public List<SelectListItem> TechnicalExpertList { get; set; } //add by Shreyas 23-03-2023
        public string MONITOR_NAME { get; set; } // added by Srishti on 11-04-2023

        public string PIU_OR_SQC { get; set; }// ADDED BY VIKKY
    }

}