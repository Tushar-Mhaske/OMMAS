using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace PMGSY.Models.MasterDataEntry
{
    public class BlockMaster
    {
        [UIHint("Hidden")]
      //  [Required(ErrorMessage = "Block Code is required.")]
        public string EncryptedBlockCode { get; set; }


        public int MAST_BLOCK_CODE { get; set; }

        [Display(Name = "Block Name")]
        [Required(ErrorMessage = "Block Name is required.")]
        [StringLength(50, ErrorMessage = "Block Name is not greater than 50 characters.")]
        [RegularExpression(@"^([a-zA-Z ]+)$", ErrorMessage = "Block Name is not in valid format.")]
        public string MAST_BLOCK_NAME { get; set; }

        [Display(Name = "Block Id")]
        [Required(ErrorMessage = "Block Id is required.")]
        [Range(1, 2147483647, ErrorMessage = " Please enter Block Id in valid format.")]
        [RegularExpression(@"^([0-9]+)$", ErrorMessage = "Block Id not in valid format.")]
        public int? Max_Mast_Block_Id { get; set; }

        [Display(Name = "State")]
        [Range(1, 2147483647, ErrorMessage = "Please select state.")]
        public int MAST_STATE_CODE { get; set; }

        [Display(Name = "District")]
        [Range(1, 2147483647, ErrorMessage = "Please select district.")]
        public int MAST_DISTRICT_CODE { get; set; }

        [Display(Name = "Is Desert")]
        public string MAST_IS_DESERT { get; set; }

        [Display(Name = "Is Tribal")]
        public string MAST_IS_TRIBAL { get; set; }

        [Display(Name = "Is Included In PMGSY")]
        public string MAST_PMGSY_INCLUDED { get; set; }

        [Display(Name = "Is Schedule5")]
        public string MAST_SCHEDULE5 { get; set; }

        [Display(Name = "Is Most Affected IAP")]
        public string MAST_IS_IAP { get; set; }

        public string MAST_BLOCK_ACTIVE { get; set; }
        public int MAST_NIC_STATE_CODE { get; set; }

        [Display(Name = "Is Border Area")]
        public string MAST_IS_BADB { get; set; }

        [IsBooleanValidator(ErrorMessage = "Please select valid option")]
        public bool IsDESERT { get; set; }
        [IsBooleanValidator(ErrorMessage = "Please select valid option")]
        public bool IsTRIBAL { get; set; }
        [IsBooleanValidator(ErrorMessage = "Please select valid option")]
        public bool IsPMGSYIncluded { get; set; }
        [IsBooleanValidator(ErrorMessage = "Please select valid option")]
        public bool IsSchedule5 { get; set; }
        [IsBooleanValidator(ErrorMessage = "Please select valid option")]
        public bool IsBADB { get; set; }

        [IsBooleanValidator(ErrorMessage = "Please select valid option")]
        public bool IsIAP { get; set; }

        [Display(Name="Active")]
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
    }
}