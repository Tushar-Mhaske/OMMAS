﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.ComponentModel.DataAnnotations;
using PMGSY.Extensions;

namespace PMGSY.Models.MasterDataEntry
{
    public class MPConstituency
    {
        [UIHint("Hidden")]
        // [Required(ErrorMessage = "District Code is required.")]
        public string EncryptedMpConstituencyCode { get; set; }


        public int MAST_MP_CONST_CODE { get; set; }

        [Display(Name = "MP Constituency Name")]
        [Required(ErrorMessage = "MP Constituency Name is required.")]
        [StringLength(50, ErrorMessage = "MP Constituency Name is not greater than 50 characters.")]
        [RegularExpression(@"^([a-zA-Z ()_+-]+)$", ErrorMessage = "MP Constituency Name is not in valid format.")]
        public string MAST_MP_CONST_NAME { get; set; }

        [Display(Name = "State")]
        [Range(1, 2147483647, ErrorMessage = "Please select state.")]
        public int MAST_STATE_CODE { get; set; }
        
        public string MAST_MP_CONST_ACTIVE { get; set; }


        //To get state Name 
        /// <summary>
        /// Master list of state details
        /// </summary>
        public SelectList States
        {
            get
            {
                List<PMGSY.Models.MASTER_STATE> stateList = new List<PMGSY.Models.MASTER_STATE>();

                if (PMGSYSession.Current.StateCode > 0)
                {
                    stateList.Insert(0, new PMGSY.Models.MASTER_STATE() { MAST_STATE_CODE = PMGSYSession.Current.StateCode, MAST_STATE_NAME = PMGSYSession.Current.StateName.Trim() });
                }
                else
                {
                    PMGSY.DAL.MasterDataEntryDAL masterDataEntryDAL = new PMGSY.DAL.MasterDataEntryDAL();

                    stateList = masterDataEntryDAL.GetAllStates(false);
                    stateList.Insert(0, new PMGSY.Models.MASTER_STATE() { MAST_STATE_CODE = 0, MAST_STATE_NAME = "--Select--" });
                }
                return new SelectList(stateList, "MAST_STATE_CODE", "MAST_STATE_NAME", this.MAST_STATE_CODE);
            }
        }

    }
}