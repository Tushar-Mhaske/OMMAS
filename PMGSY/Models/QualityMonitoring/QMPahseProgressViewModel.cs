#region File Header
/*
        * Project Id    :
        * Project Name  :   OMMAS II
        * Name          :   QMPahseProgressViewModel.cs        
        * Description   :   All filters will be loaded using this model
        * Author        :   Rohit Jadhav.
        * Creation Date :   29/Aug/2014
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
    public class QMPahseProgressViewModel
    {

        [Range(1, 2147483647, ErrorMessage = "Please select state.")]
        [RegularExpression("^([0-9]+)$", ErrorMessage = "State must be valid number.")]
        [Display(Name = "State : ")]
        public int StateCode { get; set; }
        public List<SelectListItem> lstStates { get; set; }

        public String StateName { get; set; }

        public int LevelID { get; set; }
        public int year { get; set; }
    }
}