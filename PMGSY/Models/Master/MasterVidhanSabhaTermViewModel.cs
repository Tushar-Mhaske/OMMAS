/*----------------------------------------------------------------------------------------
 * Project Id       :

 * Project Name     :OMMAS-II

 * File Name        :MasterVidhanSabhaTermViewModel.cs
 
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
    public class MasterVidhanSabhaTermViewModel
    {
        [UIHint("hidden")]
        public string EncryptedVSTermCode { get; set; }

        [Required(ErrorMessage = "Please select State.")]
        [Display(Name = "State")]
        public int MAST_STATE_CODE { get; set; }

        public int MAST_VS_TERM { get; set; }

        [RegularExpression(@"^(((0[1-9]|[12]\d|3[01])\/(0[13578]|1[02])\/((19|[2-9]\d)\d{2}))|((0[1-9]|[12]\d|30)\/(0[13456789]|1[012])\/((19|[2-9]\d)\d{2}))|((0[1-9]|1\d|2[0-8])\/02\/((19|[2-9]\d)\d{2}))|(29\/02\/((1[6-9]|[2-9]\d)(0[48]|[2468][048]|[13579][26])|((16|[2468][048]|[3579][26])00))))$", ErrorMessage = "Start Date is not in valid format")]
        [Display(Name = "Start Date")]
        [Required(ErrorMessage="Start Date is required.")]
        public string MAST_VS_START_DATE { get; set; }

        [RegularExpression(@"^(((0[1-9]|[12]\d|3[01])\/(0[13578]|1[02])\/((19|[2-9]\d)\d{2}))|((0[1-9]|[12]\d|30)\/(0[13456789]|1[012])\/((19|[2-9]\d)\d{2}))|((0[1-9]|1\d|2[0-8])\/02\/((19|[2-9]\d)\d{2}))|(29\/02\/((1[6-9]|[2-9]\d)(0[48]|[2468][048]|[13579][26])|((16|[2468][048]|[3579][26])00))))$", ErrorMessage = "End Date is not in valid format")]
        [Display(Name = "End Date")]
        [DateValidationVST("MAST_VS_START_DATE", ErrorMessage = "End Date must be greater than or equal to Start Date.")]
        public string MAST_VS_END_DATE { get; set; }
        
        /// <summary>
        /// TO Get the State Names
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

        public virtual ICollection<MASTER_MLA_MEMBERS> MASTER_MLA_MEMBERS { get; set; }
        public virtual MASTER_STATE MASTER_STATE { get; set; }

    }
}