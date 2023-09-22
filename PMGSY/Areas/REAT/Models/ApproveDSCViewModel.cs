using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PMGSY.Areas.REAT.Models
{
    public class ApproveDSCViewModel // Added by priyanka 28-05-2020
    {
        [Required(ErrorMessage = "Please select State")]
        [Range(1, 50, ErrorMessage = "Please select a valid State")]
        public int stateCode { get; set; }
        public string stateName { get; set; }
        public List<SelectListItem> lstState { set; get; }

        [Required(ErrorMessage = "Please select Agency")]
        [Range(1, int.MaxValue, ErrorMessage = "Please select a valid Agency")]
        public int agencyCode { get; set; }
        public string agencyName { get; set; }
        public List<SelectListItem> lstAgency { set; get; }

        [Required(ErrorMessage = "Please select Department")]
        [Range(1, int.MaxValue, ErrorMessage = "Please select a valid Department")]
        public int AdminNDCode { get; set; }
        public List<SelectListItem> lstPIU { set; get; }

        public string ACK_DSC_STATUS { get; set; }
        public string REJECTION_NARRATION { get; set; }
        public string Authorised_Signatory_Name { get; set; }
        public int ADMIN_NO_OFFICER_CODE { get; set; }
        public long FileID { get; set; }

       //Added by Ajinkya
        public string REJECTION_CODE { get; set; }

        public int SRRDA { get; set; }
        public List<SelectListItem> SRRDA_LIST { get; set; }

        public short LEVEL { get; set; }

        public string ADMIN_ND_NAME { get; set; }

        public int? MAST_PARENT_ND_CODE { get; set; }

    }
}