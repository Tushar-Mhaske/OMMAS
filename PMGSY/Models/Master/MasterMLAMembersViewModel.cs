/*----------------------------------------------------------------------------------------
 * Project Id       :

 * Project Name     :OMMAS-II

 * File Name        :MasterMLAMembersViewModel.cs
 
 * Author           : Vikram Nandanwar

 * Creation Date    :14/May/2013

 * Desc             : This class is used to declare the variables, lists that are used in the Details form.
 
 * ---------------------------------------------------------------------------------------*/
using PMGSY.DAL.Master;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PMGSY.Models.Master
{
    public class MasterMLAMembersViewModel
    {
        [UIHint("Hidden")]
        public string EncryptedMembersCode { get; set; }
        
        [Required(ErrorMessage="Please select State.")]
        [Display(Name="State")]
        [Range(1, 2147483647, ErrorMessage = "Please select State.")]
        public int MAST_STATE_CODE { get; set; }
        
        [Required(ErrorMessage="Please select Vidhan Sabha Term.")]
        [Range(1, 2147483647, ErrorMessage = "Please select Vidhan Sabha Term.")]
        [Display(Name="Vidhan Sabha Term")]
        public int MAST_VS_TERM { get; set; }
        
        [Required(ErrorMessage="Please select MLA Constituency.")]
        [Range(1, 2147483647, ErrorMessage = "Please select MLA Constituency.")]
        [Display(Name="MLA Constituency")]
        public int MAST_MLA_CONST_CODE { get; set; }
        
        public int MAST_MEMBER_ID { get; set; }

        [Required(ErrorMessage="Member Name is required.")]
        [Display(Name="Member Name")]
        [RegularExpression(@"^([a-zA-Z _.]+)$", ErrorMessage = "Member Name is not in valid format.")]
        [StringLength(50,ErrorMessage="Member Name must be less than 50 characters.")]
        public string MAST_MEMBER { get; set; }

        [Required(ErrorMessage = "Party Name is required.")]
        [Display(Name="Party Name")]
        [RegularExpression(@"^([a-zA-Z -()._]+)$", ErrorMessage = "Party Name is not in valid format.")]
        [StringLength(50,ErrorMessage="Party Name must be less than 50 characters.")]
        public string MAST_MEMBER_PARTY { get; set; }

        [Display(Name="Start Date")]
        [RegularExpression(@"^(((0[1-9]|[12]\d|3[01])\/(0[13578]|1[02])\/((19|[2-9]\d)\d{2}))|((0[1-9]|[12]\d|30)\/(0[13456789]|1[012])\/((19|[2-9]\d)\d{2}))|((0[1-9]|1\d|2[0-8])\/02\/((19|[2-9]\d)\d{2}))|(29\/02\/((1[6-9]|[2-9]\d)(0[48]|[2468][048]|[13579][26])|((16|[2468][048]|[3579][26])00))))$", ErrorMessage = "Start Date is not in valid format.")]
        [DataType(DataType.Date)]
        public string MAST_MEMBER_START_DATE { get; set; }

        [Display(Name="End Date")]
        [RegularExpression(@"^(((0[1-9]|[12]\d|3[01])\/(0[13578]|1[02])\/((19|[2-9]\d)\d{2}))|((0[1-9]|[12]\d|30)\/(0[13456789]|1[012])\/((19|[2-9]\d)\d{2}))|((0[1-9]|1\d|2[0-8])\/02\/((19|[2-9]\d)\d{2}))|(29\/02\/((1[6-9]|[2-9]\d)(0[48]|[2468][048]|[13579][26])|((16|[2468][048]|[3579][26])00))))$", ErrorMessage = "End Date is not in valid format.")]
        [DataType(DataType.Date)]
        [DateValidationVST("MAST_MEMBER_START_DATE", ErrorMessage = "End Date must be greater than or equal to Start Date.")]
        public string MAST_MEMBER_END_DATE { get; set; }

        public virtual MASTER_MLA_CONSTITUENCY MASTER_MLA_CONSTITUENCY { get; set; }
        public virtual MASTER_VIDHAN_SABHA_TERM MASTER_VIDHAN_SABHA_TERM { get; set; }

        /// <summary>
        /// To get the State names 
        /// </summary>
        public SelectList States
        {
            get
            {
                List<MASTER_STATE> stateList = new List<MASTER_STATE>();

                MasterDAL objMaster = new MasterDAL();

                stateList = objMaster.GetAllStates();

                stateList.Insert(0, new PMGSY.Models.MASTER_STATE() { MAST_STATE_CODE = 0, MAST_STATE_NAME = "--Select--" });

                return new SelectList(stateList, "MAST_STATE_CODE", "MAST_STATE_NAME", this.MAST_STATE_CODE);
            }
        }
        /// <summary>
        /// To get the Constituency Name 
        /// </summary>
        public SelectList Constituency
        {
            get 
            {
                List<MASTER_MLA_CONSTITUENCY> constituencyList = new List<MASTER_MLA_CONSTITUENCY>();

                MasterDAL objMaster = new MasterDAL();

                constituencyList = objMaster.GetAllConstituency(this.MAST_STATE_CODE);

                constituencyList.Insert(0, new PMGSY.Models.MASTER_MLA_CONSTITUENCY() { MAST_MLA_CONST_CODE = 0, MAST_MLA_CONST_NAME= "--Select--" });

                return new SelectList(constituencyList, "MAST_MLA_CONST_CODE", "MAST_MLA_CONST_NAME", this.MAST_MLA_CONST_CODE);
            }
        }
        /// <summary>
        /// To get the Vidhan Sabha Term count 
        /// </summary>
        public SelectList VidhanSabhaTerm
        {
            get
            {
                List<MASTER_VIDHAN_SABHA_TERM> termList = new List<MASTER_VIDHAN_SABHA_TERM>();
                

                MasterDAL objMaster = new MasterDAL();

                termList = objMaster.GetAllVidhanSabhaTerms(this.MAST_STATE_CODE);

                return new SelectList(termList, "MAST_VS_TERM", "MAST_VS_TERM", this.MAST_VS_TERM);
            }
        }

    }
}