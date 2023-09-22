using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace PMGSY.Models.Proposal
{
    public class AdditionalLengthViewModel
    {
        public int IMS_PR_ROAD_CODE { get; set; }

        public int IMS_TRANSACTION_CODE { get; set; }

        public string EncProposalCode { get; set; }

        [Required(ErrorMessage = "Please select Increase or Decrease")]
        [RegularExpression(@"[ID]",ErrorMessage="Please select Increase or Decrease")]
        [Display(Name="Please select Increase or Decrease")]
        public string IMS_CHANGE_TYPE { get; set; }

        [Required]
        [Display(Name="New Length (in Kms)")]
        public decimal IMS_CHANGED_LENGTH { get; set; }

        [Display(Name="Change in Percentage")]
        [Required(ErrorMessage="Please enter change in percentage.")]
        public decimal IMS_PERCENTAGE_CHANGE { get; set; }

        [Display(Name="Current Sanction Length")]
        public decimal IMS_CURRENT_LENGTH { get; set; }

        public string IMS_LENGTH_CHANGE_REQUEST_DATE { get; set; }

        public string IMS_IS_MRD_APPROVED { get; set; }

        public string IMS_MRD_APPROVED_DATE { get; set; }

        [Display(Name = "Year")]
        public int IMS_YEAR { get; set; }

        [Display(Name = "Batch")]
        public int IMS_BATCH { get; set; }

        [Display(Name = "Package Number")]
        public string IMS_PACKAGE_ID { get; set; }

        [Display(Name = "Road Name")]
        public string IMS_ROAD_NAME { get; set; }

        [Display(Name = "Pavement Length")]
        public Nullable<decimal> IMS_PAV_LENGTH { get; set; }

        [Display(Name = "State Amount")]
        public Nullable<decimal> IMS_STATE_AMOUNT_TEXT { get; set; }

        [Display(Name = "Mord Amount")]
        public Nullable<decimal> IMS_MORD_AMOUNT_TEXT { get; set; }

        public string ProposalType { get; set; }
    }
}