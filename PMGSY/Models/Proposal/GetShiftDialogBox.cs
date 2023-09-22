using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using PMGSY.Models;
using System.Web.Mvc;
using PMGSY.Common;
using PMGSY.Extensions;
using PMGSY.DAL.Proposal;


namespace PMGSY.Models.Proposal
{
    public class GetShiftDialogBox
    {

        public GetShiftDialogBox()
        {
            STATES = new List<SelectListItem>();
            DISTRICTS = new List<SelectListItem>();
            BLOCKS = new List<SelectListItem>();
        }

        [Display(Name = "State")]
        public Nullable<int> MAST_STATE_CODE { get; set; }
        public List<SelectListItem> STATES { get; set; }

        [Display(Name = "District")]
        public Nullable<int> MAST_DISTRICT_CODE { get; set; }
        public List<SelectListItem> DISTRICTS { get; set; }

        [Display(Name = "Block")]
        public int MAST_BLOCK_CODE { get; set; }
        public List<SelectListItem> BLOCKS { get; set; }


        public string EncryptedVillageCode { get; set; }
        public Int32 ProposalCode { get; set; }
    }
}