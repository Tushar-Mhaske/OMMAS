using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.Collections;
using System.Web.Mvc;


namespace PMGSY.Models.MasterDataEntry
{
    public class MASTER_DISTRICT
    {

        [UIHint("Hidden")]
       // [Required(ErrorMessage = "District Code is required.")]
        public string EncryptedDistrictCode { get; set; }

        public int MAST_DISTRICT_CODE { get; set; }

        [Display(Name = "District Name")]
        [Required(ErrorMessage = "District Name is required.")]
        [StringLength(50, ErrorMessage = "District Name is not greater than 50 characters.")]
        [RegularExpression(@"^([a-zA-Z ]+)$", ErrorMessage = "District Name is not in valid format.")]
        public string MAST_DISTRICT_NAME { get; set; }

        [Display(Name = "District Id")]
        [Required(ErrorMessage = "District Id is required.")]
        [Range(1, 2147483647, ErrorMessage = " Please enter District Id in valid format.")]
        [RegularExpression(@"^([0-9]+)$", ErrorMessage = "District Id not in valid format.")]
        public int Max_Mast_District_Id { get; set; }

        [Display(Name="State")]
        [Range(1, 2147483647, ErrorMessage = " Please select state.")]
        public int MAST_STATE_CODE { get; set; }

         [Display(Name = "Is Included In PMGSY")]
        public string MAST_PMGSY_INCLUDED { get; set; }

         [Display(Name = "Is IAP District")]
        public string MAST_IAP_DISTRICT { get; set; }
        
        public string MAST_DISTRICT_ACTIVE { get; set; }
        public int MAST_NIC_STATE_CODE { get; set; }
        public Nullable<int> MAST_NIC_DISTRICT_CODE { get; set; }
        public string DUMMY_STATE_CODE { get; set; }
        public Nullable<int> DUMMY_DISTRICT_CODE { get; set; }

        [IsBooleanValidator(ErrorMessage="Please select valid option")]
        public bool IsPMGSYIncluded { get; set; }

        [IsBooleanValidator(ErrorMessage = "Please select valid option")]
        public bool IsIAPDistrict { get; set; }
        
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
                
                stateList.Insert(0,new PMGSY.Models.MASTER_STATE() { MAST_STATE_CODE = 0, MAST_STATE_NAME = "--Select--" });

               return new SelectList(stateList, "MAST_STATE_CODE", "MAST_STATE_NAME",this.MAST_STATE_CODE);  
            }
        }

    }



}