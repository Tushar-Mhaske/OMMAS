/*----------------------------------------------------------------------------------------
 * Project Id       :

 * Project Name     :OMMAS-II

 * File Name        :MasterRegionViewModel.cs
 
 * Author           :Abhishek Kamble.

 * Creation Date    :01/May/2013

 * Desc             :This class is used to declare the variables, lists that are used in the Details form.
 
 * ---------------------------------------------------------------------------------------*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using PMGSY.DAL.Master;

namespace PMGSY.Models.Master
{
    public class MasterRegionViewModel
    {
        [UIHint("hidden")]
        public string EncryptedRegionCode { get; set; }
        
        public int MAST_REGION_CODE { get; set; }
        [Display(Name = "Region Name")]
        [Required(ErrorMessage = "Region Name is required.")]
        [RegularExpression(@"^([a-zA-Z0-9 ]+)$", ErrorMessage = "Region Name is not in valid format.")]
        [StringLength(50, ErrorMessage = "Region Name must be less than 50 characters.")]
        public string MAST_REGION_NAME { get; set; }

        [Required(ErrorMessage = "Please select State")]
        [Display(Name = "State")]     
        public int MAST_STATE_CODE { get; set; }

        /// <summary>
        /// To Get The State Names 
        /// </summary>
        public SelectList States
        {
            get
            {
                List<MASTER_STATE> stateList = new List<MASTER_STATE>();
                

                IMasterDAL objDAL = new MasterDAL();



                stateList = objDAL.GetAllStateNames();

                return new SelectList(stateList, "MAST_STATE_CODE", "MAST_STATE_NAME");
            }

        }

    
        public virtual ICollection<MASTER_REGION_DISTRICT_MAP> MASTER_REGION_DISTRICT_MAP { get; set; }
        public virtual MASTER_STATE MASTER_STATE { get; set; }
    }
}