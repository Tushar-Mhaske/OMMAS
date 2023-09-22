using PMGSY.Common;
using PMGSY.DAL.MPMLAProposal;
using PMGSY.Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PMGSY.Models.MPMLAProposal
{
    public class MLARoadProposalInclusionViewModel:IValidatableObject
    {
        [UIHint("hidden")]
        public string EncryptedCode { get; set; }

        public string display_Year { get; set; }

        [Display(Name = "Constituency: ")]
        public string display_Constituency { get; set; }

        [Display(Name = "Whether included in core network ?")]
        [RegularExpression("[YN]",ErrorMessage="Please select Y or N")]
        public string IMS_INCLUDED_IN_CN { get; set; }

        [Display(Name = "Reasons for non-inclusion in core network ?")]
        public Nullable<int> IMS_REASON_ID_1 { get; set; }

        [Display(Name = "Road Name")]
        public Nullable<int> PLAN_CN_ROAD_CODE { get; set; }

        [Display(Name = "Whether included in proposal ?")]
        [RegularExpression("[YN]", ErrorMessage = "Please select Y or N")]
        public string IMS_INCLUDED_IN_PROPOSAL { get; set; }

        [Display(Name = "Reasons for non-inclusion in proposals ?")]
        public Nullable<int> IMS_REASON_ID_2 { get; set; }

        [Display(Name = "Road Name")]
        public Nullable<int> IMS_PR_ROAD_CODE { get; set; }

        [Display(Name = "Block")]
        public int IMS_BLOCK { get; set; }

        [Display(Name = "Block")]
        public int DRRP_BLOCK { get; set; }

        [Display(Name = "Year")]
        public int inclusion_year { get; set; }

        [Display(Name = "Road Details: ")]
        public string IMS_ROAD_DETAILS { get; set; }

        public string Operation { get; set; }


        /// <summary>
        /// Populate Year Drop down for Road Incusion Details 
        /// </summary>
        public SelectList Years
        {
            get
            {
                List<SelectListItem> lstYears = new List<SelectListItem>();
                IMPMLAProposalDAL objDAL = new MPMLAProposalDAL();
                lstYears = objDAL.PopulateYear(0, true, false);
                lstYears.RemoveAt(0);
                return new SelectList(lstYears, "Value", "Text");
            }
        }

        /// <summary>
        /// Populate Block Drop down for Road Incusion Details 
        /// </summary>
        public SelectList Blocks
        {
            get {
                List<SelectListItem> lstBlocks = new List<SelectListItem>();
                CommonFunctions objCommonFunction = new CommonFunctions();
                lstBlocks = objCommonFunction.PopulateBlocks(PMGSYSession.Current.DistrictCode, false);
                lstBlocks.RemoveAt(0);
                return new SelectList(lstBlocks, "Value", "Text");
            }
        }

        /// <summary>
        /// Populate Reason of type 'I' for Road Incusion Details 
        /// </summary>
        public SelectList Reasons
        {
            get
            {
                IMPMLAProposalDAL objDAL = new MPMLAProposalDAL();
                List<SelectListItem> lstReasons = new List<SelectListItem>();
                lstReasons = objDAL.PopulateReasons();
                return new SelectList(lstReasons, "Value", "Text");
            }
        }

        /// <summary>
        /// Populate Core Network Road Names for Road Incusion Details 
        /// </summary>
        public SelectList DrrpRoadNames
        {
            get
            {
                IMPMLAProposalDAL objDAL = new MPMLAProposalDAL();
                List<SelectListItem> lstDrrpRoadNames = new List<SelectListItem>();
                lstDrrpRoadNames = objDAL.PopulateDrrpRoads(0);
                return new SelectList(lstDrrpRoadNames, "Value", "Text");
            }
        }
        /// <summary>
        /// Populate Ims Road Names for Road Incusion Details 
        /// </summary>
        public SelectList ImsRoadNames
        {
            get
            {
                IMPMLAProposalDAL objDAL = new MPMLAProposalDAL();
                List<SelectListItem> lstImsRoadNames = new List<SelectListItem>();
                lstImsRoadNames = objDAL.PopulateImsSanctionedRoads(0, 0);
                return new SelectList(lstImsRoadNames, "Value", "Text");
            }
        }

        public virtual IMS_SANCTIONED_PROJECTS IMS_SANCTIONED_PROJECTS { get; set; }
        public virtual MASTER_MP_CONSTITUENCY MASTER_MP_CONSTITUENCY { get; set; }
        public virtual MASTER_REASON MASTER_REASON { get; set; }
        public virtual MASTER_REASON MASTER_REASON1 { get; set; }
        public virtual PLAN_ROAD PLAN_ROAD { get; set; }


        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if ((this.IMS_INCLUDED_IN_CN == "Y") && (this.PLAN_CN_ROAD_CODE == 0))
            {
                yield return new ValidationResult("Please select road name", new[] { "PLAN_CN_ROAD_CODE" });
            }

            if ((this.IMS_INCLUDED_IN_CN == "N") && (this.IMS_REASON_ID_1 == 0))
            {
                yield return new ValidationResult("Please select reason", new[] { "IMS_REASON_ID_1" });
            }

            if ((this.IMS_INCLUDED_IN_PROPOSAL == "Y") && (this.IMS_PR_ROAD_CODE == 0))
            {
                yield return new ValidationResult("Please select road name", new[] { "IMS_PR_ROAD_CODE" });
            }

            if ((this.IMS_INCLUDED_IN_PROPOSAL == "N") && (this.IMS_REASON_ID_2 == 0))
            {
                yield return new ValidationResult("Please select reason", new[] { "IMS_REASON_ID_2" });
            }
        }
    }
}