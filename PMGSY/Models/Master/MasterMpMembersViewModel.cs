/*----------------------------------------------------------------------------------------
 * Project Id       :

 * Project Name     :OMMAS-II

 * File Name        :MasterMpMembersViewModel.cs
 
 * Author           :Abhishek Kamble.

 * Creation Date    :14/May/2013

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
    public class MasterMpMembersViewModel
    {
        [UIHint("hidden")]
        public string EncryptedMpMemberId { get; set; }

        [Required(ErrorMessage = "Please select Lok Sabha Term.")]
        [Display(Name = "Lok Sabha Term")]
        public int MAST_LS_TERM { get; set; }

        [Range(1, 2147483647, ErrorMessage = "Please select MP Constituency.")]
        [Required(ErrorMessage = "Please select MP Constituency.")]
        [Display(Name = "MP Constituency")]
        public int MAST_MP_CONST_CODE { get; set; }

        public int MAST_MEMBER_ID { get; set; }

        [Display(Name = "Member Name")]
        [Required(ErrorMessage = "Member Name is required.")]
        [RegularExpression(@"^([a-zA-Z _.]+)$", ErrorMessage = "Member Name is not in valid format.")]
        [StringLength(50, ErrorMessage = "Member Name must be less than 50 characters.")]
        public string MAST_MEMBER { get; set; }

        [Display(Name = "Party Name")]
        [RegularExpression(@"^([a-zA-Z -()._]+)$", ErrorMessage = "Party Name is not in valid format.")]
        [StringLength(50, ErrorMessage = "Party Name must be less than 50 characters.")]
        public string MAST_MEMBER_PARTY { get; set; }


        [RegularExpression(@"^(((0[1-9]|[12]\d|3[01])\/(0[13578]|1[02])\/((19|[2-9]\d)\d{2}))|((0[1-9]|[12]\d|30)\/(0[13456789]|1[012])\/((19|[2-9]\d)\d{2}))|((0[1-9]|1\d|2[0-8])\/02\/((19|[2-9]\d)\d{2}))|(29\/02\/((1[6-9]|[2-9]\d)(0[48]|[2468][048]|[13579][26])|((16|[2468][048]|[3579][26])00))))$", ErrorMessage = "Start Date is not in valid format")]
        [Display(Name = "Start Date")]
        [Required(ErrorMessage = "Start Date is required.")]
        [DataType(DataType.Date, ErrorMessage = "Start date is not in valid format.")]
        public string MAST_MEMBER_START_DATE { get; set; }



        [RegularExpression(@"^(((0[1-9]|[12]\d|3[01])\/(0[13578]|1[02])\/((19|[2-9]\d)\d{2}))|((0[1-9]|[12]\d|30)\/(0[13456789]|1[012])\/((19|[2-9]\d)\d{2}))|((0[1-9]|1\d|2[0-8])\/02\/((19|[2-9]\d)\d{2}))|(29\/02\/((1[6-9]|[2-9]\d)(0[48]|[2468][048]|[13579][26])|((16|[2468][048]|[3579][26])00))))$", ErrorMessage = "End Date is not in valid format")]
        [Display(Name = "End Date")]
        [DataType(DataType.Date, ErrorMessage = "End Date is not in valid format.")]
        [DateValidationVST("MAST_MEMBER_START_DATE", ErrorMessage = "End Date must be greater than or equal to Start Date.")]
        public string MAST_MEMBER_END_DATE { get; set; }

        /// <summary>
        /// To get the Lok Sabha Term count 
        /// </summary>
        public SelectList LockSabhaTerms
        {
            get
            {
                List<MASTER_LOK_SABHA_TERM> list = new List<MASTER_LOK_SABHA_TERM>();
                IMasterDAL objDAL = new MasterDAL();
                list = objDAL.GetAllLockSabhaTerms();
                return new SelectList(list, "MAST_LS_TERM", "MAST_LS_TERM");
            }
        }
        /// <summary>
        /// To get the MP Constituency Names
        /// </summary>
        public SelectList MpConstituencyNames
        {
            get
            {
                List<MASTER_MP_CONSTITUENCY> list = new List<MASTER_MP_CONSTITUENCY>();
                IMasterDAL objDAL = new MasterDAL();

                list = objDAL.GetAllMpConstituencyNames();
                return new SelectList(list, "MAST_MP_CONST_CODE", "MAST_MP_CONST_NAME");
            }
        }

        public virtual MASTER_LOK_SABHA_TERM MASTER_LOK_SABHA_TERM { get; set; }
        public virtual MASTER_MP_CONSTITUENCY MASTER_MP_CONSTITUENCY { get; set; }

    }
}