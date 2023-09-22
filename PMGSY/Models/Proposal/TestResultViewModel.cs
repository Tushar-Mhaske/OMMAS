using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using PMGSY.DAL.Proposal;

namespace PMGSY.Models.Proposal
{
    public class TestResultViewModel
    {

        [UIHint("Hidden")]
        public string EncryptedResultCode { get; set; }

        public int hidden_ims_pr_road_code { get; set; }
        
        public int IMS_RESULT_CODE { get; set; }

        public int IMS_PR_ROAD_CODE { get; set; }

        

        [Display(Name = "Test Name")]
        [Range(1, int.MaxValue, ErrorMessage = "Test Name is required")]
        public int IMS_TEST_CODE { get; set; }

        [Display(Name = "Chainage")]
        [Required(ErrorMessage = "Chainage is required")]
        [RegularExpression(@"^\s*(?=.*[0-9])\d*(?:\.\d{1,3})?\s*$", ErrorMessage = "Invalid Chainage, Can only contains Numeric values and 3 digits after decimal palce")]
        [Range(0.001, 999.999, ErrorMessage = "Invalid Chainage")]
        public decimal IMS_CHAINAGE { get; set; }


        [Display(Name = "Sample")]
        [Range(1, int.MaxValue, ErrorMessage = "Sample is required")]
        public int IMS_SAMPLE_ID { get; set; }

        [Display(Name = "Value")]
        [Required(ErrorMessage = "Value is required")]
        [RegularExpression(@"^\s*(?=.*[0-9])\d*(?:\.\d{1,2})?\s*$", ErrorMessage = "Invalid Value, Can only contains Numeric values and 2 digits after decimal palce")]
        [Range(0.01, 999999.99, ErrorMessage = "Invalid Value")]
        public decimal IMS_TEST_RESULT1 { get; set; }


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

        public bool RoadStatus { get; set; }

        public SelectList TestNames {

            get {
                List<SelectListItem> lstTestNames = null;
                IProposalDAL objProposalDAL = new ProposalDAL();

                lstTestNames = objProposalDAL.PopulateTestNames();

                return new SelectList(lstTestNames, "Value", "Text");
            }
        }

        public SelectList Samples {
            get {
                List<SelectListItem> lstSamples = null;
                IProposalDAL objProposalDAL = new ProposalDAL();
                lstSamples = objProposalDAL.PopulateSamples();
                return new SelectList(lstSamples, "Value", "Text");
            }
        }


        public virtual IMS_SANCTIONED_PROJECTS IMS_SANCTIONED_PROJECTS { get; set; }
        public virtual MASTER_TEST MASTER_TEST { get; set; }
    }
}