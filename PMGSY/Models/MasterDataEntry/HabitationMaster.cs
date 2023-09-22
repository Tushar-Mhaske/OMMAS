using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace PMGSY.Models.MasterDataEntry
{
    public class HabitationMaster
    {
        [UIHint("Hidden")]
        //[Required(ErrorMessage = "Habitation Code is required.")]
        public string EncryptedHabitationCode { get; set; }


        public int MAST_HAB_CODE { get; set; }

        [Display(Name = "Habitation Name")]
        [Required(ErrorMessage = "Habitation Name is required.")]
        [StringLength(50, ErrorMessage = "Habitation Name is not greater than 50 characters.")]
        [RegularExpression(@"^([a-zA-Z0-9 ._-]+)$", ErrorMessage = "Habitation Name is not in valid format.")]

        //[RegularExpression(@"^[a-zA-Z][a-zA-Z0-9 ._-]+$", ErrorMessage = "Habitation Name is not in valid format.")]
        public string MAST_HAB_NAME { get; set; }


        [Display(Name = "State")]
        [Range(1, 2147483647, ErrorMessage = "Please select state.")]
        public int MAST_STATE_CODE { get; set; }

        [Display(Name = "District")]
        [Range(1, 2147483647, ErrorMessage = "Please select district.")]
        public int MAST_DISTRICT_CODE { get; set; }


        [Display(Name = "Block")]
        [Range(1, 2147483647, ErrorMessage = "Please select block.")]
        public int MAST_BLOCK_CODE { get; set; }

        [Display(Name = "Village")]
        [Range(1, 2147483647, ErrorMessage = "Please select village.")]
        public int MAST_VILLAGE_CODE { get; set; }

        [Display(Name = "MLA Constituency")]
        //[Range(1, 2147483647, ErrorMessage = "Please select MLA contituency.")]
        public int MAST_MLA_CONST_CODE { get; set; }

        [Display(Name = "MP Constituency")]
        //[Range(1, 2147483647, ErrorMessage = "Please select MP contituency.")]
        public int MAST_MP_CONST_CODE { get; set; }

        [Display(Name = "Is Schedule5")]
        public string MAST_SCHEDULE5 { get; set; }

        [IsBooleanValidator(ErrorMessage = "Please select valid option")]
        public bool IsSchedule5 { get; set; }

        [Display(Name = "Active")]
        [IsBooleanValidator(ErrorMessage = "Please select valid Status")]
        public bool IsActive { get; set; }

        //To get state Name 
        /// <summary>
        /// Master list of state details
        /// </summary>
        public SelectList States
        {
            get
            {
                List<PMGSY.Models.MASTER_STATE> stateList = new List<PMGSY.Models.MASTER_STATE>();

                PMGSY.DAL.MasterDataEntryDAL masterDataEntryDAL = new PMGSY.DAL.MasterDataEntryDAL();

                stateList = masterDataEntryDAL.GetAllStates(false);

                stateList.Insert(0, new PMGSY.Models.MASTER_STATE() { MAST_STATE_CODE = 0, MAST_STATE_NAME = "--Select--" });

                return new SelectList(stateList, "MAST_STATE_CODE", "MAST_STATE_NAME", this.MAST_STATE_CODE);
            }
        }
     
        public SelectList Districts
        {
            get
            {
                List<PMGSY.Models.MASTER_DISTRICT> districtList = new List<PMGSY.Models.MASTER_DISTRICT>();

                PMGSY.DAL.MasterDataEntryDAL masterDataEntryDAL = new PMGSY.DAL.MasterDataEntryDAL();

                districtList = masterDataEntryDAL.GetAllDistrictsByStateCode(this.MAST_STATE_CODE,false);

                return new SelectList(districtList, "MAST_DISTRICT_CODE", "MAST_DISTRICT_NAME", this.MAST_DISTRICT_CODE);
            }
        }


        public SelectList Blocks
        {
            get
            {
                List<MASTER_BLOCK> blockList = new List<MASTER_BLOCK>();

                PMGSY.DAL.MasterDataEntryDAL masterDataEntryDAL = new PMGSY.DAL.MasterDataEntryDAL();

                blockList = masterDataEntryDAL.GetAllBlocksByDistrictCode(this.MAST_DISTRICT_CODE,false);

                return new SelectList(blockList, "MAST_BLOCK_CODE", "MAST_BLOCK_NAME", this.MAST_BLOCK_CODE);
            }
        }


        public SelectList Villages
        {
            get
            {
                List<MASTER_VILLAGE> villageList = new List<MASTER_VILLAGE>();

                PMGSY.DAL.MasterDataEntryDAL masterDataEntryDAL = new PMGSY.DAL.MasterDataEntryDAL();

                villageList = masterDataEntryDAL.GetAllVillagesByBlockCode(this.MAST_BLOCK_CODE,false);

                return new SelectList(villageList, "MAST_VILLAGE_CODE", "MAST_VILLAGE_NAME", this.MAST_VILLAGE_CODE);
            }
        }

        public SelectList MPContituency
        {
            get
            {
                List<MASTER_MP_CONSTITUENCY> mpContituencyList = new List<MASTER_MP_CONSTITUENCY>();

                PMGSY.DAL.MasterDataEntryDAL masterDataEntryDAL = new PMGSY.DAL.MasterDataEntryDAL();

                mpContituencyList = masterDataEntryDAL.GetAllMPContituencyByBlockCode(this.MAST_BLOCK_CODE);

                return new SelectList(mpContituencyList, "MAST_MP_CONST_CODE", "MAST_MP_CONST_NAME", this.MAST_MP_CONST_CODE);
            }
        }

        public SelectList MLAContituency
        {
            get
            {
                List<MASTER_MLA_CONSTITUENCY> mlaContituencyList = new List<MASTER_MLA_CONSTITUENCY>();

                PMGSY.DAL.MasterDataEntryDAL masterDataEntryDAL = new PMGSY.DAL.MasterDataEntryDAL();

                mlaContituencyList = masterDataEntryDAL.GetAllMLAContituencyByBlockCode(this.MAST_BLOCK_CODE);

                return new SelectList(mlaContituencyList, "MAST_MLA_CONST_CODE", "MAST_MLA_CONST_NAME", this.MAST_MP_CONST_CODE);
            }
        }

        

    }
}