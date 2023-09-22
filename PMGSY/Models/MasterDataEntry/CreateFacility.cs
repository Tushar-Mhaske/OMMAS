using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PMGSY.Models.MasterDataEntry
{
    public class CreateFacility
    {
        // Added by Rohit on 4 Sept 2019
        public string IsFinalized { get; set; }
        public string FinalizedDate { get; set; }
        [UIHint("Hidden")]
        public string EncryptedCode { get; set; }

        public string Errmessage { get; set; }
        [UIHint("Hidden")]
        public string EncryptedFacilityCode { get; set; }

        [Display(Name = "District")]
        //[Range(1, 2147483647, ErrorMessage = "Please select district.")]
        public int MAST_DISTRICT_CODE { get; set; }

        public IEnumerable<SelectListItem> DistrictList { get; set; }


        [Display(Name = "Block")]
        [Range(1, 2147483647, ErrorMessage = "Please Select Block.")]
        public int MAST_BLOCK_CODE { get; set; }

        //public IEnumerable<SelectListItem> BlockList { get; set; }
        public List<SelectListItem> BlockList { get; set; }

        public IEnumerable<SelectListItem> facilityList { get; set; }

        [Display(Name = "Facility Category")]
        [Range(1, 2147483647, ErrorMessage = "Please Select Facility Category.")]
        public int facilityCode { get; set; }

        [Display(Name = "Facility Type")]
        //[Required(AllowEmptyStrings=false,ErrorMessage="Please Enter Facility Name")]
        [Range(1, 2147483647, ErrorMessage = "Please Select facility Type.")]
        public int FacilityName { get; set; }
        public IEnumerable<SelectListItem> facilityNameList { get; set; }

        [Display(Name = "Address")]
        [Required(ErrorMessage = "Address is required.")]
        [MaxLength(255, ErrorMessage = "Maximum 255 Charcters allowed")]
        [RegularExpression(@"^[0-9a-zA-Z''-'\s]{1,254}$", ErrorMessage = "special characters are not allowed")]
        public string address { get; set; }
        //\.\-\,
        [Range(1, 2147483647, ErrorMessage = "Please Select Habitation.")]
        [Display(Name = "Habitation")]
        public int HabitationCode { get; set; }

        public IEnumerable<SelectListItem> habitationList { get; set; }

        //[Display(Name = "PIN Code")]
        //[Required(ErrorMessage="Please Enter PIN Code")]
        //public string pincode { get; set; }

        [Display(Name = "PIN Code")]
        [Required(ErrorMessage = "PIN Code is required.")]
        [RegularExpression("^([0-9]{6})?$", ErrorMessage = "Invalid PIN Code")]
        public string pincode { get; set; }

    


        [Display(Name = "Facility Name")]
        [Required(ErrorMessage = "Facility Description is required.")]
        [MaxLength(200, ErrorMessage = "Maximum 200 Charcters allowed")]
        [RegularExpression(@"^[0-9a-zA-Z''-'\s]{1,199}$", ErrorMessage = "special characters are not allowed.")]
        public string FacilityDescription { get; set; }


        [Display(Name = "DISTRICT")]
        public string DistrictName { get; set; }

        [Display(Name = "BLOCK")]
        public string blockName { get; set; }

        [Display(Name = "HABITATION")]
        public string habName { get; set; }

        [Display(Name = "FACILITY TYPE")]
        public string FacilityParentCategory { get; set; }

        [Display(Name = "FACILITY CATEGORY")]
        public string FacilityCategory { get; set; }

        [Display(Name = "FACILITY DESCRIPTION")]
        public string FacilityDesc { get; set; }

        [Display(Name = "ADDRESS")]
        public string DisplayAddress { get; set; }

        [Display(Name = "PIN CODE")]
        public string DisplayPIN { get; set; }

        public List<string> Photos { get; set; }

        public double Latitude { get; set; }

        public double Longitude { get; set; }

        public string UploadDate { get; set; }

        public int FacilityID { get; set; }

        public string FileName { get; set; }

        public string FileNameReport { get; set; }

        public bool isReportDisplay { get; set; }

        public bool isPMGSY3Finalized { get; set; }

    }
}