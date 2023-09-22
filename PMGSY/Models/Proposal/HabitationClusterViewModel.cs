#region File Header
/*
        * Project Name  :   OMMAS II
        * Name          :   HabitationClusterViewModel.cs
        * Description   :   This View Model is Used for mapping the habitation along with cluster in Proposal
        * Author        :   Vikram Nandanwar        
        * Creation Date :   08/Sept/2014
 **/
#endregion

using PMGSY.Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace PMGSY.Models.Proposal
{
    public class HabitationClusterViewModel
    {
        public HabitationClusterViewModel()
        {
            PMGSYScheme = PMGSYSession.Current.PMGSYScheme;
        }

        public byte PMGSYScheme { get; set; }

        [Display(Name="Year")] 
        public int IMS_YEAR { get; set;}
        [Display(Name = "Batch")]
        public int IMS_BATCH { get; set; }
        [Display(Name = "Package No.")]
        public string IMS_PACKAGE_ID { get; set; }
        [Display(Name="Road Name")]
        public string PLAN_RD_NAME { get; set; }
        [Display(Name = "Pavement Length")]
        public decimal IMS_PAV_LENGTH { get; set; }

        public string PLAN_CN_ROAD_CODE { get; set; }
        public int IMS_PR_ROAD_CODE { get; set; }
        public string MAST_STATE_TYPE { get; set; }
        public string MAST_IAP_DISTRICT { get; set; }
        public string MAST_IS_TRIBAL { get; set; }

        public string IMS_ISCOMPLETED { get; set; }

        //Taken for is Mord Unlocked Status
        public string IMS_LOCK_STATUS { get; set; }

        [Display(Name = "Habitaion")]
        [Required(ErrorMessage = "Please Select Habitation")]
        public int MAST_HAB_CODE { get; set; }


        [Display(Name = "Population")]
        public int MAST_HAB_TOT_POP { get; set; }

        public string MAST_IAP_BLOCK { get; set; }

        public string MAST_BLOCK_SCHEDULE5 { get; set; }

        public string MAST_BLOCK_IS_DESERT { get; set; }

        public string HAB_CODES_LIST { get; set; }

        public string CLUSTER_CODES_LIST { get; set; }

        public bool IsStageTwoProposal { get; set; }

        public string SelectionType { get; set; }

        public virtual MASTER_HABITATIONS MASTER_HABITATIONS { get; set; }
    }
}