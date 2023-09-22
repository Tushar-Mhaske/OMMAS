using PMGSY.Common;
using PMGSY.Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PMGSY.Areas.AnalysisSSRSReport.Models
{
    public class RodsUpgradeMaintModel
    {

        public RodsUpgradeMaintModel()
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
                    // BlockList.Find(x => x.Value == "-1").Value = "0";
                    //BlockCode = PMGSYSession.Current.BlockCode == 0 ? 0 : PMGSYSession.Current.BlockCode;
                    //BlockList.Find(x => x.Value == BlockCode.ToString()).Selected = true;
                }

                RoadWise = true;
      
            }
          
            public int LevelCode { get; set; }
            public string StateName { get; set; }
            public string DistName { get; set; }
            public string BlockName { get; set; }
            public int Mast_State_Code { get; set; }
            public int Mast_Block_Code { get; set; }
            public int Mast_District_Code { get; set; }
          
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

            [Display(Name = "Roadwise")]
            public bool RoadWise { get; set; }

         
        
    }
}