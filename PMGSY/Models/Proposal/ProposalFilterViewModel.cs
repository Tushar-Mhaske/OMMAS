#region File Header
/*
        * Project Id    :
        * Project Name  :   OMMAS II
        * Name          :   ProposalFilterViewModel.cs
        * Description   :   This View Model is Used in Filtering Proposal Listing View - ListProposal.cshtml
        * Author        :   Shivkumar Deshmukh        
        * Creation Date :   09/May/2013
 **/
#endregion

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
    public class ProposalFilterViewModel
    {
        public ProposalFilterViewModel()
        {
            BLOCKS = new List<SelectListItem>();
            Years = new List<SelectListItem>();
            BATCHS = new List<SelectListItem>();
            PROPOSAL_TYPES = new List<SelectListItem>();
            DISTRICTS = new List<SelectListItem>();
            STATES = new List<SelectListItem>();
            MORD_STATUS = new List<SelectListItem>();
            COLLABORATIONS = new List<SelectListItem>();
            SCHEMELIST = new List<SelectListItem>();
            SCHEMELIST.Insert(0, new SelectListItem { Value = "1", Text = "PMGSY-I" });
            SCHEMELIST.Insert(1, new SelectListItem { Value = "2", Text = "PMGSY-II" });
            SCHEMELIST.Insert(2, new SelectListItem { Value = "3", Text = "RCPLWE" });
            SCHEMELIST.Insert(3, new SelectListItem { Value = "4", Text = "PMGSY-III" });
        }

        public int UserLevelID { get; set; }
        public int RoleID { get; set; }

        [Display(Name = "Year")]
        public int IMS_YEAR { get; set; }
        public List<SelectListItem> Years { set; get; }

        [Display(Name = "Batch")]
        public int IMS_BATCH { get; set; }
        public List<SelectListItem> BATCHS { set; get; }

        [Display(Name = "Block")]
        public int MAST_BLOCK_CODE { get; set; }
        public List<SelectListItem> BLOCKS { get; set; }

        [Display(Name = "Proposal Type")]
        public string IMS_PROPOSAL_TYPE { get; set; }
        public List<SelectListItem> PROPOSAL_TYPES { get; set; }

        //[Display(Name = "Technology Proposed")]        
        //public Nullable<int> IMS_STREAMS { get; set; }
        //public List<SelectListItem> STREAMS { get; set; }

        [Display(Name = "District")]
        public Nullable<int> MAST_DISTRICT_CODE { get; set; }
        public List<SelectListItem> DISTRICTS { get; set; }

        [Display(Name = "Proposal Status")]
        public string IMS_PROPOSAL_STATUS { get; set; }
        public List<SelectListItem> PROPOSAL_STATUS { get; set; }

        [Display(Name = "State Name")]
        public Nullable<int> MAST_STATE_CODE { get; set; }
        public List<SelectListItem> STATES { get; set; }

        public List<SelectListItem> MORD_STATUS { get; set; }

        [Display(Name = "Funding Agency")]
        public Nullable<int> IMS_COLLABORATION { get; set; }
        public List<SelectListItem> COLLABORATIONS { get; set; }

        [Display(Name = "Agency")]
        public Nullable<int> IMS_AGENCY { get; set; }
        public List<SelectListItem> AGENCIES { get; set; }

        [Display(Name = "New / Upgradation")]
        public string IMS_UPGRADE_COONECT { get; set; }
        public List<SelectListItem> CONNECTIVITYLIST { get; set; }

        [Display(Name = "Scheme")]
        public int PMGSY_SCHEME { get; set; }
        public List<SelectListItem> SCHEMELIST { get; set; }

        //Added by Pradip Patil [21-04-2017]
        [Display(Name = "Batch")]
        [Range(1, int.MaxValue, ErrorMessage = "Please Select Batch")]
        public int DROP_IMS_BATCH { get; set; }

        [Display(Name = "Funding Agency")]
        [Range(1, int.MaxValue, ErrorMessage = "Please Select Collaboration")]
        public int DROP_IMS_COLLABORATION { get; set; }

        public string StateName { get; set; }

    }

    public class ProposalColumnsTotal
    {
        public int TOT_HAB1000 { get; set; }

        public int TOT_HAB999 { get; set; }

        public int TOT_HAB499 { get; set; }

        public int TOT_HAB250 { get; set; }

        public int TOT_HABS { get; set; }

        public decimal TOT_PAV_LENGTH { get; set; }

        public decimal TOT_STATE_COST { get; set; }

        public decimal? TOT_MORD_COST { get; set; }

        public decimal TOT_COST { get; set; }

        public decimal TOT_MANE_COST { get; set; }

        public decimal TOT_RENEWAL_COST { get; set; }

        // decimal change to nullable
        //public decimal TOT_HIGHER_SPEC { get; set; }
        public decimal? TOT_HIGHER_SPEC { get; set; }

        public decimal? TOTAL_COST { get; set; }
        public decimal? STATE_SHARE_COST { get; set; }
        public decimal? MORD_SHARE_COST { get; set; }
        public decimal? TOTAL_STATE_SHARE { get; set; }
        public decimal? TOTAL_SHARE_COST { get; set; }
    }

    public class ProposalDPRFilterViewModel
    {
        public ProposalDPRFilterViewModel()
        {

            CommonFunctions commonFunctions = new CommonFunctions();
            ProposalDAL objDAL = new ProposalDAL();
            SCHEMELIST = new List<SelectListItem>();
            SCHEMELIST.Insert(0, new SelectListItem { Value = "1", Text = "PMGSY-I" });
            SCHEMELIST.Insert(1, new SelectListItem { Value = "2", Text = "PMGSY-II" });


            STATES_List = new List<SelectListItem>();

            STATES_List = commonFunctions.PopulateStates(false);
            //STATES_List.Insert(0, (new SelectListItem { Text = "All States", Value = "0"}));
            STATES_List.Insert(0, (new SelectListItem { Text = "Select State", Value = "-1" }));
            MAST_STATE_CODE = PMGSYSession.Current.StateCode == 0 ? 0 : PMGSYSession.Current.StateCode;
            if (MAST_DISTRICT_CODE > 0)
            {
                STATES_List.Find(x => x.Value == MAST_STATE_CODE.ToString()).Selected = true;
            }
            else
            {
                STATES_List.Find(x => x.Value == "-1").Selected = true;
            }

            DISTRICTS_List = new List<SelectListItem>();
            if (MAST_STATE_CODE == 0)
            {
                //DISTRICTS_List.Insert(0, (new SelectListItem { Text = "All Districts", Value = "0", Selected = true }));
                DISTRICTS_List.Insert(0, (new SelectListItem { Text = "Select District", Value = "-1", Selected = true }));
            }
            else
            {
                DISTRICTS_List = commonFunctions.PopulateDistrict(MAST_STATE_CODE, true);
                MAST_DISTRICT_CODE = PMGSYSession.Current.DistrictCode == 0 ? 0 : PMGSYSession.Current.DistrictCode;
                //DISTRICTS_List.Find(x => x.Value == "-1").Value = "0";
                if (PMGSYSession.Current.RoleCode == 22)
                {
                    DISTRICTS_List.Find(x => x.Value == "-1").Text = "Select District"; 
                }
                else 
                { 
                    DISTRICTS_List.Find(x => x.Value == "-1").Value = "0"; 
                }
                if (MAST_DISTRICT_CODE > 0)
                {
                    DISTRICTS_List.Find(x => x.Value == MAST_DISTRICT_CODE.ToString()).Selected = true;
                }
                else
                {
                    //DISTRICTS_List.Find(x => x.Value == "-1").Selected = true;
                    DISTRICTS_List.Find(x => x.Value == MAST_DISTRICT_CODE.ToString()).Selected = true;
                }
            }
            BLOCKS_List = new List<SelectListItem>();
            if (MAST_DISTRICT_CODE == 0)
            {
                BLOCKS_List.Insert(0, (new SelectListItem { Text = "All Blocks", Value = "0", Selected = true }));
            }
            else
            {
                BLOCKS_List = commonFunctions.PopulateBlocks(MAST_DISTRICT_CODE, true);
                BLOCKS_List.Find(x => x.Value == "-1").Value = "0";
                //BlockCode = PMGSYSession.Current.BlockCode == 0 ? 0 : PMGSYSession.Current.BlockCode;
                //BlockList.Find(x => x.Value == BlockCode.ToString()).Selected = true;
            }

            IMS_YEAR = DateTime.Now.Year;
            // FromYearList =new SelectList(commonFunctions.PopulateFinancialYear(true,true),"Value","Text").ToList();
            Years_List = commonFunctions.PopulateYears(false);
            Years_List.Insert(0, (new SelectListItem { Text = "All Years", Value = "0" }));



            BATCHS_List = commonFunctions.PopulateBatch(true);
            COLLABORATIONS_List = commonFunctions.PopulateFundingAgency(true);
            PROPOSAL_TYPES_List = new List<SelectListItem>();
            PROPOSAL_TYPES_List = commonFunctions.PopulateProposalTypes();

            PROPOSAL_STATUS_List = new List<SelectListItem>();

            PROPOSAL_STATUS_List.Add(new SelectListItem { Text = "All Proposals", Value = "%" });
            PROPOSAL_STATUS_List.Add(new SelectListItem { Text = "Pending", Value = "P" });
            PROPOSAL_STATUS_List.Add(new SelectListItem { Text = "Srutinized", Value = "S" });
            PROPOSAL_STATUS_List.Add(new SelectListItem { Text = "UnScrutinized", Value = "U" });
            IMS_PROPOSAL_STATUS = "%";

            CONNECTIVITYLIST = new List<SelectListItem>();
            CONNECTIVITYLIST.Insert(0, new SelectListItem { Value = "0", Text = "All" });
            CONNECTIVITYLIST.Insert(1, new SelectListItem { Value = "N", Text = "New Connectivity" });
            CONNECTIVITYLIST.Insert(2, new SelectListItem { Value = "U", Text = "Upgradation" });

            Package_List = new List<SelectListItem>();
            //Package_List = objDAL.PopulatePackagesForRepackaging(0, 0, 0);
            Package_List.Add(new SelectListItem { Text = "All Packages", Value = "0" });
        }

        public int UserLevelID { get; set; }
        public int RoleID { get; set; }

        [Display(Name = "Year")]
        public int IMS_YEAR { get; set; }
        public List<SelectListItem> Years_List { set; get; }

        [Display(Name = "Batch")]
        public int IMS_BATCH { get; set; }
        public List<SelectListItem> BATCHS_List { set; get; }

        [Display(Name = "Block")]
        public int MAST_BLOCK_CODE { get; set; }
        public List<SelectListItem> BLOCKS_List { get; set; }

        [Display(Name = "Proposal Type")]
        public string IMS_PROPOSAL_TYPE { get; set; }
        public List<SelectListItem> PROPOSAL_TYPES_List { get; set; }

        //[Display(Name = "Technology Proposed")]        
        //public Nullable<int> IMS_STREAMS { get; set; }
        //public List<SelectListItem> STREAMS { get; set; }

        [Display(Name = "District")]
        [Range(0, int.MaxValue, ErrorMessage = "Please Select District")]
        public int MAST_DISTRICT_CODE { get; set; }
        public List<SelectListItem> DISTRICTS_List { get; set; }

        [Display(Name = "Proposal Status")]
        public string IMS_PROPOSAL_STATUS { get; set; }
        public List<SelectListItem> PROPOSAL_STATUS_List { get; set; }

        [Display(Name = "State")]
        [Range(1, int.MaxValue, ErrorMessage = "Please Select State")]
        public int MAST_STATE_CODE { get; set; }
        public List<SelectListItem> STATES_List { get; set; }

        public List<SelectListItem> MORD_STATUS { get; set; }

        [Display(Name = "Funding Agency")]
        public Nullable<int> IMS_COLLABORATION { get; set; }
        public List<SelectListItem> COLLABORATIONS_List { get; set; }

        [Display(Name = "Agency")]
        public int IMS_AGENCY { get; set; }
        public List<SelectListItem> AGENCIES_List { get; set; }

        [Display(Name = "New / Upgradation")]
        public string IMS_UPGRADE_COONECT { get; set; }
        public List<SelectListItem> CONNECTIVITYLIST { get; set; }

        [Display(Name = "Scheme")]
        public int PMGSY_SCHEME { get; set; }
        public List<SelectListItem> SCHEMELIST { get; set; }

        [Display(Name = "Package")]
        public string Package_Id { get; set; }
        public List<SelectListItem> Package_List { get; set; }
    }
}