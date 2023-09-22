using PMGSY.Common;
using PMGSY.Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PMGSY.Areas.MaintenanceSSRSReport.Models
{
    public class MaintenanceStatewise
    {
        public MaintenanceStatewise()
            {
                CommonFunctions commonFunctions = new CommonFunctions();              
                StateList = new List<SelectListItem>();
                    

                StateName = PMGSYSession.Current.StateCode == 0 ? "0" : PMGSYSession.Current.StateName.Trim();
              

                Mast_State_Code = PMGSYSession.Current.StateCode;
           

                //LevelCode = PMGSYSession.Current.BlockCode > 0 ? 3 : PMGSYSession.Current.DistrictCode > 0 ? 2 : 1;
                LevelCode = PMGSYSession.Current.DistrictCode > 0 ? 2 : 1;

                StateList = commonFunctions.PopulateStates(false);
               // StateList.Insert(0, (new SelectListItem { Text = "All States", Value = "0"}));
                StateCode = PMGSYSession.Current.StateCode == 0 ? 0 : PMGSYSession.Current.StateCode;
                StateList.Find(x => x.Value == StateCode.ToString()).Selected = true;

            
             

             
      
            }
          
            public int LevelCode { get; set; }
            public string StateName { get; set; }
                  
            public int Mast_State_Code { get; set; }
        
          
            [Display(Name = "State")]
            [Required(ErrorMessage = "Please select State. ")]
            [Range(0, int.MaxValue, ErrorMessage = "Please select  State.")]
            public int StateCode { get; set; }
            public List<SelectListItem> StateList { get; set; }

  
    }
}