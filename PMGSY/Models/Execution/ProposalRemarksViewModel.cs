#region File Header
/*
        * Project Id    :
        * Project Name  :   OMMAS II
        * Name          :   ProposalFilterViewModel.cs
        * Description   :   This View Model is Used in CBR Views AddProgressRemarks.cshtml
        * Author        :   Vikram Nandanwar        
        * Creation Date :   25/June/2013
 **/
#endregion

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace PMGSY.Models.Execution
{
    public class ProposalRemarksViewModel
    {
        public int IMS_PR_ROAD_CODE { get; set; }

        [Required(ErrorMessage="Please Enter Remarks.")]
        [Display(Name="Remarks")]
        [RegularExpression(@"^([a-zA-Z0-9 ,()]+){1,255}$", ErrorMessage = "Please Enter Valid Text.")]
        public string Remarks { get; set; }

        public string Operation { get; set; }

        public string BlockName { get; set; }

        public string Package { get; set; }

        public int RoadNo { get; set; }

        public string RoadName { get; set; }

        public double Sanction_Cost { get; set; }

        public decimal Sanction_length { get; set; }


    }
}