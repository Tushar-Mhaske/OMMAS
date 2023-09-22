using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using PMGSY.DAL.MPMLAProposal;

namespace PMGSY.Models.MPMLAProposal
{
    public class MPProposalViewModel
    {   
        [UIHint("hidden")]
        public string EncryptedCode { get; set; }
        public string display_Year { get; set; }
        public string display_Constituency { get; set; }

        [Display(Name = "MP Constituency:")]
        [Range(1, int.MaxValue, ErrorMessage = "Please Select Constituency")]
        public int MAST_MP_CONST_CODE { get; set; }

        [Display(Name = "Year")]
        [Range(2000, int.MaxValue, ErrorMessage = "Please Select Year")]
        public int IMS_YEAR { get; set; }

        public int IMS_ROAD_ID { get; set; }
        [Display(Name="Road Details")]
        [RegularExpression(@"^([a-zA-Z0-9 ._',\r\n&()-]+)$", ErrorMessage = "Road details is not in valid format.")]
        [Required(ErrorMessage="Please enter road details")]
        public string IMS_ROAD_DETAILS { get; set; }

        [Display(Name = "Whether included in DRRP ?")]
        public string IMS_INCLUDED_IN_CN { get; set; }

        [Display(Name = "Reasons for non-inclusion in DRRP ?")]
        public Nullable<int> IMS_REASON_ID_1 { get; set; }

        [Display(Name = "Road Name")]
        public Nullable<int> PLAN_CN_ROAD_CODE { get; set; }

        [Display(Name = "Whether included in proposal ?")]
        public string IMS_INCLUDED_IN_PROPOSAL { get; set; }

        [Display(Name = "Reasons for non-inclusion in current years proposals ?")]
        public Nullable<int> IMS_REASON_ID_2 { get; set; }

        [Display(Name = "Road Name")]
        public Nullable<int> IMS_PR_ROAD_CODE { get; set; }


        /// <summary>
        /// Populate Year for Search criteria dropdown
        /// </summary>
        public SelectList Years
        {
            get
            {
                List<SelectListItem> lstYears = new List<SelectListItem>();
                IMPMLAProposalDAL objDAL = new MPMLAProposalDAL();
                lstYears = objDAL.PopulateYear(0, true, false);
                return new SelectList(lstYears, "Value", "Text");
            }
        }

        /// <summary>
        /// Populate MP Constituency for Search criteria dropdown
        /// </summary>
        public SelectList Constituency
        {
            get
            {
                List<SelectListItem> lstConstituency = new List<SelectListItem>();
                IMPMLAProposalDAL objDAL = new MPMLAProposalDAL();
                lstConstituency = objDAL.PopulateMPConstatuency();
                return new SelectList(lstConstituency, "Value", "Text");
            }
        }
        
        public virtual IMS_SANCTIONED_PROJECTS IMS_SANCTIONED_PROJECTS { get; set; }
        public virtual MASTER_MP_CONSTITUENCY MASTER_MP_CONSTITUENCY { get; set; }
        public virtual MASTER_REASON MASTER_REASON { get; set; }
        public virtual MASTER_REASON MASTER_REASON1 { get; set; }
        public virtual PLAN_ROAD PLAN_ROAD { get; set; }
    }
}