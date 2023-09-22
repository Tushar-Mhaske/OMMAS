#region File Header
/*
        * Project Id    :
        * Project Name  :   OMMAS II
        * Name          :   ExecutingOfficerViewModel.cs
        * Description   :   This View Model is Used in CBR Views AddExecutingOfficerDetails.cshtml
        * Author        :   Vikram Nandanwar        
        * Creation Date :   28/June/2013
 **/
#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
namespace PMGSY.Models.Execution
{
    public class ExecutingOfficerViewModel
    {
        public string Operation { get; set; }

        public int EXEC_OFFICER_ID { get; set; }
        public int IMS_PR_ROAD_CODE { get; set; }

        [Display(Name = "Year")]
        [Range(2000, 3000, ErrorMessage = "Please Select Year")]
        [CompareAgrementYear("AgreementYear", "AgreementDate", ErrorMessage = "Year must be greater than or equal to Agreement Year")]
        public int EXEC_YEAR { get; set; }

        [Display(Name="Month")]
        [Range(1, 12, ErrorMessage = "Please Select Month")]
        [CompareAgrementMonth("EXEC_YEAR", "AgreementDate", ErrorMessage = "Month must be greater than or equal to Agreement Month")]
        public int EXEC_MONTH { get; set; }

        [Display(Name = "Executing Officer")]
        [Range(1, int.MaxValue, ErrorMessage = "Please Select Executing Officer")]
        public int MAST_OFFICER_CODE { get; set; }

        [Display(Name = "Designation")]
        [Range(1, int.MaxValue, ErrorMessage = "Please Select Designation")]
        public Nullable<int> MAST_DESIG_CODE { get; set; }
        
        public int ExecutingOfficerId{ get; set; }

        public string DistrictName { get; set; }

        public string BlockName { get; set; }

        public string Package { get; set; }

        public string RoadName { get; set; }

        public double Sanction_Cost { get; set; }
        
        public decimal Sanction_length { get; set; }

        public string Year { get; set; }

        public int AgreementYear { get; set; }

        public string AgreementDate { get; set; }

        public int AgreementMonth { get; set; }



        public virtual ADMIN_NODAL_OFFICERS ADMIN_NODAL_OFFICERS { get; set; }
        public virtual IMS_SANCTIONED_PROJECTS IMS_SANCTIONED_PROJECTS { get; set; }
    }
}