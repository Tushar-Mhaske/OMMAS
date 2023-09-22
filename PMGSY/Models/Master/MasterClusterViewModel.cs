using PMGSY.Common;
using PMGSY.Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PMGSY.Models.Master
{
    public class MasterClusterViewModel
    {

        public MasterClusterViewModel()
        {
            CommonFunctions commonFunctions = new CommonFunctions();
            StateList = new List<SelectListItem>();
            DistrictList = new List<SelectListItem>();
            BlockList = new List<SelectListItem>();

            StateName = PMGSYSession.Current.StateCode == 0 ? "0" : PMGSYSession.Current.StateName.Trim();
            DistName = PMGSYSession.Current.DistrictCode == 0 ? "0" : PMGSYSession.Current.DistrictName.Trim();

            Mast_State_Code = PMGSYSession.Current.StateCode;
            Mast_District_Code = PMGSYSession.Current.DistrictCode;

            //LevelCode = PMGSYSession.Current.BlockCode > 0 ? 3 : PMGSYSession.Current.DistrictCode > 0 ? 2 : 1;
            LevelCode = PMGSYSession.Current.DistrictCode > 0 ? 2 : 1;

            StateList = commonFunctions.PopulateStates(true);
            // StateList.Insert(0, (new SelectListItem { Text = "All States", Value = "0"}));
            StateCode = PMGSYSession.Current.StateCode == 0 ? 0 : PMGSYSession.Current.StateCode;
            StateList.Find(x => x.Value == StateCode.ToString()).Selected = true;

            DistrictList = new List<SelectListItem>();
            if (StateCode == 0)
            {
                DistrictList.Insert(0, (new SelectListItem { Text = "Select District", Value = "0", Selected = true }));
            }
            else
            {
                DistrictList = commonFunctions.PopulateDistrict(StateCode, false);
                DistrictCode = PMGSYSession.Current.DistrictCode == 0 ? 0 : PMGSYSession.Current.DistrictCode;
                // DistrictList.Find(x => x.Value == "-1").Value = "0";
                DistrictList.Find(x => x.Value == DistrictCode.ToString()).Selected = true;

            }
            BlockList = new List<SelectListItem>();
            if (DistrictCode == 0)
            {
                BlockList.Insert(0, (new SelectListItem { Text = "Select Block", Value = "0", Selected = true }));
            }
            else
            {
                BlockList = commonFunctions.PopulateBlocks(DistrictCode, false);
                //BlockList.Find(x => x.Value == "-1").Value = "0";
            }



        }

        public int LevelCode { get; set; }
        public string StateName { get; set; }
        public string DistName { get; set; }
        public string BlockName { get; set; }
        public int Mast_State_Code { get; set; }
        public int Mast_Block_Code { get; set; }
        public int Mast_District_Code { get; set; }

        [UIHint("hidden")]
        public string EncryptedClusterCode { get; set; }

        [Display(Name = "State")]
        [Required(ErrorMessage = "Please select State. ")]
        [Range(1, int.MaxValue, ErrorMessage = "Please select  State.")]
        public int StateCode { get; set; }
        public List<SelectListItem> StateList { get; set; }

        [Display(Name = "District")]
        [Required(ErrorMessage = "Please select District.")]
        [Range(1, int.MaxValue, ErrorMessage = "Please select District.")]
        public int DistrictCode { get; set; }
        public List<SelectListItem> DistrictList { get; set; }

        [Display(Name = "Block")]
        [Required(ErrorMessage = "Please select Block.")]
        [Range(1, int.MaxValue, ErrorMessage = "Please select Block.")]
        public int BlockCode { get; set; }
        public List<SelectListItem> BlockList { get; set; }


    }
    public class MasterClusterSearchModel
    {
        public MasterClusterSearchModel()
        {
            CommonFunctions commonFunctions = new CommonFunctions();
            StateList = new List<SelectListItem>();
            DistrictList = new List<SelectListItem>();
            BlockList = new List<SelectListItem>();

            StateName = PMGSYSession.Current.StateCode == 0 ? "0" : PMGSYSession.Current.StateName.Trim();
            DistName = PMGSYSession.Current.DistrictCode == 0 ? "0" : PMGSYSession.Current.DistrictName.Trim();

            Mast_State_Code = PMGSYSession.Current.StateCode;
            Mast_District_Code = PMGSYSession.Current.DistrictCode;

            //LevelCode = PMGSYSession.Current.BlockCode > 0 ? 3 : PMGSYSession.Current.DistrictCode > 0 ? 2 : 1;
            LevelCode = PMGSYSession.Current.DistrictCode > 0 ? 2 : 1;

            StateList = commonFunctions.PopulateStates(true);
            // StateList.Insert(0, (new SelectListItem { Text = "All States", Value = "0"}));
            StateCode = PMGSYSession.Current.StateCode == 0 ? 1 : PMGSYSession.Current.StateCode;
            StateList.Find(x => x.Value == StateCode.ToString()).Selected = true;

            DistrictList = new List<SelectListItem>();
            if (StateCode == 0)
            {
                DistrictList.Insert(0, (new SelectListItem { Text = "All District", Value = "0", Selected = true }));
            }
            else
            {
                DistrictList = commonFunctions.PopulateDistrict(StateCode, true);
                DistrictCode = PMGSYSession.Current.DistrictCode == 0 ? 0 : PMGSYSession.Current.DistrictCode;
                DistrictList.Find(x => x.Value == "-1").Value = "0";
               // DistrictList.Find(x => x.Value == DistrictCode.ToString()).Selected = true;
             

            }
            BlockList = new List<SelectListItem>();
            if (DistrictCode == 0)
            {
                BlockList.Insert(0, (new SelectListItem { Text = "All Block", Value = "0", Selected = true }));
            }
            else
            {
                BlockList = commonFunctions.PopulateBlocks(DistrictCode, true);
                BlockList.Find(x => x.Value == "-1").Value = "0";
            }

            StatusList = new List<SelectListItem>();
            StatusList.Insert(0, (new SelectListItem { Text = "All", Value = "%"}));
            StatusList.Insert(1, (new SelectListItem { Text = "Active", Value = "Y" ,Selected=true}));
            StatusList.Insert(1, (new SelectListItem { Text = "In Active", Value = "N" }));
            Status = "Y";
   
        }

        public int LevelCode { get; set; }
        public string StateName { get; set; }
        public string DistName { get; set; }
        public string BlockName { get; set; }
        public int Mast_State_Code { get; set; }
        public int Mast_Block_Code { get; set; }
        public int Mast_District_Code { get; set; }

        [UIHint("hidden")]
        public string EncryptedClusterCode { get; set; }

        [Display(Name = "State")]
        [Required(ErrorMessage = "Please select State. ")]
        [Range(1, int.MaxValue, ErrorMessage = "Please select  State.")]
        public int StateCode { get; set; }
        public List<SelectListItem> StateList { get; set; }

        [Display(Name = "District")]
        [Required(ErrorMessage = "Please select District.")]
        [Range(0, int.MaxValue, ErrorMessage = "Please select District.")]
        public int DistrictCode { get; set; }
        public List<SelectListItem> DistrictList { get; set; }

        [Display(Name = "Block")]
        [Required(ErrorMessage = "Please select Block.")]
        [Range(0, int.MaxValue, ErrorMessage = "Please select Block.")]
        public int BlockCode { get; set; }
        public List<SelectListItem> BlockList { get; set; }

        [Display(Name = "Status")]
        public string Status { get; set; }
        public List<SelectListItem> StatusList { get; set; }

    }


    public class MasterClusterEditViewModel
    {
        public MasterClusterEditViewModel()
        {
        }
        [UIHint("hidden")]
        public string EncryptedClusterCode { get; set; }

        public int ClusterCode{get;set;}
        public string ClusterName{get;set;}
    
        [Display(Name = "Cluster")]
        [Required(ErrorMessage = "Please select Habitation.")]
        [Range(1, int.MaxValue, ErrorMessage = "Please select Habitation.")]
        public int Hab_Code { get; set; }
        public List<SelectListItem> HabitationList { get; set; }

    }

    public class MasterClusterViewEditHabiationModel
    {
        public MasterClusterViewEditHabiationModel()
        {
        }
        [UIHint("hidden")]
        public string EncryptedClusterCode { get; set; }

        public int ClusterCode { get; set; }
        public string ClusterName { get; set; }

        public int StatCode { get; set; }
        public string StatName { get; set; }
        public int DistrictCode { get; set; }
        public string DistrictName { get; set; }
        public int BlockCode { get; set; }
        public string BlockName { get; set; }
        public string FinalizeLockStatus { get; set; }
        public string IMS_UNLOCK_Status { get; set; }

     
    }
}