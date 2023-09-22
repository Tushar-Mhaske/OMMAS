/*----------------------------------------------------------------------------------------
 * Project Id       :

 * Project Name     :OMMAS-II

 * File Name        :MasterLokSabhaTermViewModel.cs
 
 * Author           : Vikram Nandanwar

 * Creation Date    :01/May/2013

 * Desc             : This class is used to declare the variables, lists that are used in the Details form.
 
 * ---------------------------------------------------------------------------------------*/
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace PMGSY.Models.Master
{
    public class MasterLokSabhaTermViewModel
    {

        [UIHint("Hidden")]
        public string EncryptedLokSabhaTermCode { get; set; }

        [Display(Name="Lok Sabha Term")]
        public int MAST_LS_TERM { get; set; }

        [Required(ErrorMessage="Start Date is required.")]
        [Display(Name="Start Date")]
        [RegularExpression(@"^(((0[1-9]|[12]\d|3[01])\/(0[13578]|1[02])\/((19|[2-9]\d)\d{2}))|((0[1-9]|[12]\d|30)\/(0[13456789]|1[012])\/((19|[2-9]\d)\d{2}))|((0[1-9]|1\d|2[0-8])\/02\/((19|[2-9]\d)\d{2}))|(29\/02\/((1[6-9]|[2-9]\d)(0[48]|[2468][048]|[13579][26])|((16|[2468][048]|[3579][26])00))))$", ErrorMessage = "Start Date is not in valid format.")]
        public string MAST_LS_START_DATE { get; set; }

        [Display(Name="End Date")]
        [RegularExpression(@"^(((0[1-9]|[12]\d|3[01])\/(0[13578]|1[02])\/((19|[2-9]\d)\d{2}))|((0[1-9]|[12]\d|30)\/(0[13456789]|1[012])\/((19|[2-9]\d)\d{2}))|((0[1-9]|1\d|2[0-8])\/02\/((19|[2-9]\d)\d{2}))|(29\/02\/((1[6-9]|[2-9]\d)(0[48]|[2468][048]|[13579][26])|((16|[2468][048]|[3579][26])00))))$", ErrorMessage = "End Date is not in valid format.")]
        [DateValidationVST("MAST_LS_START_DATE",ErrorMessage="End Date must be greater than or equal to Start Date.")]
        public string MAST_LS_END_DATE { get; set; }
    
        public virtual ICollection<MASTER_MP_MEMBERS> MASTER_MP_MEMBERS { get; set; }
    }
}