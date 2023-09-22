using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace PMGSY.Models.MasterDataEntry
{
    public class PanchayatMaster
    {
        [UIHint("Hidden")]      
      //  [Required(ErrorMessage = "Panchayat Code is required.")]
        public string EncryptedPanchayatCode { get; set; }

        public int MAST_PANCHAYAT_CODE { get; set; }

        [Display(Name = "Panchayat Name")]
        [Required(ErrorMessage = "Panchayat Name is required.")]
        [StringLength(50, ErrorMessage = "Panchayat Name is not greater than 50 characters.")]
        [RegularExpression(@"^([a-zA-Z ]+)$", ErrorMessage = "Panchayat Name is not in valid format.")]
        public string MAST_PANCHAYAT_NAME { get; set; }
         
        public string MAST_PANCHAYAT_ACTIVE { get; set; }

        [Display(Name = "State")]
        [Range(1, 2147483647, ErrorMessage = "Please select state.")]
        public int MAST_STATE_CODE { get; set; }

        [Display(Name = "District")]
        [Range(1, 2147483647, ErrorMessage = "Please select district.")]
        public int MAST_DISTRICT_CODE { get; set; }


        [Display(Name = "Block")]
        [Range(1, 2147483647, ErrorMessage = "Please select block.")]
        public int MAST_BLOCK_CODE { get; set; }

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
    }

    //public class CreateFacility
    //{
    //    [UIHint("Hidden")]
    //    public string EncryptedFacilityCode { get; set; }

    //    [Display(Name = "State")]
    //    [Range(1, 2147483647, ErrorMessage = "Please select state.")]
    //    public int MAST_STATE_CODE { get; set; }

    //    [Display(Name = "District")]
    //    //[Range(1, 2147483647, ErrorMessage = "Please select district.")]
    //    public int MAST_DISTRICT_CODE { get; set; }

    //    public IEnumerable<SelectListItem> DistrictList { get; set; }


    //    [Display(Name = "Block")]
    //    [Range(1, 2147483647, ErrorMessage = "Please Select Block.")]
    //    public int MAST_BLOCK_CODE { get; set; }

    //    public IEnumerable<SelectListItem> BlockList { get; set; }

    //    public IEnumerable<SelectListItem> facilityList { get; set; }

    //    [Display(Name = "Facility")]
    //    [Range(1, 2147483647, ErrorMessage = "Please Select Facility.")]
    //    public int facilityCode { get; set; }

    //    [Display(Name = "Facility Name")]
    //    //[Required(AllowEmptyStrings=false,ErrorMessage="Please Enter Facility Name")]
    //    [Range(1, 2147483647, ErrorMessage = "Please Select facility Name.")]
    //    public int FacilityName { get; set; }
    //    public IEnumerable<SelectListItem> facilityNameList { get; set; }

    //    [Display(Name = "Address")]
    //    [MaxLength(50, ErrorMessage = "Maximum 50 Charcters")]
    //    public string address { get; set; }

    //    [Range(1, 2147483647, ErrorMessage = "Please Select Habitation.")]
    //    [Display(Name = "Habitation")]
    //    public int HabitationCode { get; set; }

    //    public IEnumerable<SelectListItem> habitationList { get; set; }

    //    [Display(Name = "PIN Code")]
    //    [Required(ErrorMessage = "Please Enter PIN Code")]
    //    //[StringLength(6,ErrorMessage="Enter 6 Digit Pin Code")]
    //    public int pincode { get; set; }
    //}
}