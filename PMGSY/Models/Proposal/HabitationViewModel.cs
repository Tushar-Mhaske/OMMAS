#region File Header
/*
        * Project Id    :
        * Project Name  :   OMMAS II
        * Name          :   HabitationViewModel.cs
        * Description   :   This View Model is Used in Add, Edit, Disaply Habitation View - AddHabitation.cshtml
        * Author        :   Shivkumar Deshmukh        
        * Creation Date :   17/April/2013
 **/
#endregion

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using PMGSY.Models;
using PMGSY.Extensions;

namespace PMGSY.Models.Proposal
{
    //public class HabitationViewModel
    //{
    //    public string IMS_YEAR { get; set; }
    //    public int IMS_BATCH { get; set; }
    //    public int MAST_STATE_CODE { get; set; }
    //    public int MAST_DISTRICT_CODE { get; set; }       
    //    public int MAST_BLOCK_CODE { get; set; }
    //    public string IMS_PACKAGE_ID { get; set; }

    //    public string MAST_STATE_NAME { get; set; }
    //    public string MAST_DISTRICT_NAME { get; set; }
 
    //    public string PLAN_CN_ROAD_NUMBER { get; set; }
        
    //    public string MAST_BLOCK_NAME { get; set; }
    //    public string MAST_HAB_NAME { get; set; }
    //    public string IMS_ROAD_TO { get; set; }
    //    public string IMS_ROAD_FROM { get; set; }


    //    public int IMS_PR_ROAD_CODE { get; set; }
    //    public int MAST_HAB_CODE { get; set; }
    //    public Nullable<int> MAST_CLUSTER_CODE { get; set; }

    //    public int PLAN_CN_ROAD_CODE { get; set; }

    //    // From IMS_BENIFFITED_HABS.CS
    //    public virtual IMS_SANCTIONED_PROJECTS IMS_SANCTIONED_PROJECTS { get; set; }
    //    public virtual MASTER_HABITATIONS MASTER_HABITATIONS { get; set; }

    //}

    public class HabitationViewModel
    {

        public HabitationViewModel()
        {
            PMGSYScheme = PMGSYSession.Current.PMGSYScheme;
        }

        public byte PMGSYScheme { get; set; }

        [ Display(Name="Year")] 
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

        public bool IsStageTwoProposal { get; set; }

        public virtual MASTER_HABITATIONS MASTER_HABITATIONS { get; set; }


    }

}