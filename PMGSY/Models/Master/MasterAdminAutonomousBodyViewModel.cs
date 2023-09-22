/*----------------------------------------------------------------------------------------
 * Project Id          :

 * Project Name        :OMMAS-II

 * File Name           :MasterAdminAutonomousBodyViewModel.cs
 
 * Author              :Abhishek Kamble.

 * Creation Date       :01/May/2013

 * Desc                : This class is used to declare the variables, lists that are used in the Details form.
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
    public class MasterAdminAutonomousBodyViewModel
    {
        [UIHint("hidden")]
        public string EncryptedStateCode { get; set; }

        [Required(ErrorMessage = "Please select state.")]      
        [Display(Name = "State")]
        public int MAST_STATE_CODE { get; set; }        

        [Display(Name = "Autonomous Body")]
        [Required(ErrorMessage = "Autonomous Body is required.")]
        [RegularExpression("^([a-zA-Z0-9 ._,&()-]+)", ErrorMessage = "Autonomous Body is not in valid format.")]
        [StringLength(255, ErrorMessage = "Autonomous Body must be less than 255 characters.")]         
        public string ADMIN_AUTONOMOUS_BODY1 { get; set; }

        public SelectList lstState { get; set; }

        
        /// <summary>
        ///  To get the States 
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



        public virtual MASTER_STATE MASTER_STATE { get; set; }
    }
}