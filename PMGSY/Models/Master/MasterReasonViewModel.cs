/*----------------------------------------------------------------------------------------
 * Project Id       :

 * Project Name     :OMMAS-II

 * File Name        :MasterReasonViewModel.cs
 
 * Author           :Rohit Jadhav 
 
 * Creation Date    :01/May/2013

 * Desc             :This class is used to declare the variables, lists that are used in the Details form.
 
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
    public class MasterReasonViewModel
    {       
        [UIHint("hidden")]
        public string EncryptedReasonCode { get; set;}
        public int MAST_REASON_CODE { get; set; }

        [Display(Name = "Reason Name")]
        [RegularExpression("^([a-zA-Z0-9 -._,=;:/\"?'\r\n&()]+)", ErrorMessage = "Name must be Alpanumberic and these -._,=;:/\"?'\r\n&() special symbols allowed.")]
        [StringLength(100,ErrorMessage="Reason Name  must be less than 100 characters.")]
        [Required(ErrorMessage = "Reason Name is required.")]
        public string MAST_REASON_NAME { get; set; }

        [Required(ErrorMessage="Please select Reason Type.")]
        [Display(Name = "Reason Type")]
        public string MAST_REASON_TYPE { get; set;}
    
        public virtual ICollection<IMS_MLA_PROPOSAL_STATUS> IMS_MLA_PROPOSAL_STATUS { get; set; }
        public virtual ICollection<IMS_MLA_PROPOSAL_STATUS> IMS_MLA_PROPOSAL_STATUS1 { get; set; }
        public virtual ICollection<IMS_MP_PROPOSAL_STATUS> IMS_MP_PROPOSAL_STATUS { get; set; }
        public virtual ICollection<IMS_MP_PROPOSAL_STATUS> IMS_MP_PROPOSAL_STATUS1 { get; set; }
        public virtual ICollection<IMS_SANCTIONED_PROJECTS> IMS_SANCTIONED_PROJECTS { get; set; }
        public virtual ICollection<IMS_SANCTIONED_PROJECTS> IMS_SANCTIONED_PROJECTS1 { get; set; }
        public virtual ICollection<MASTER_EXISTING_ROADS> MASTER_EXISTING_ROADS { get; set; }

        /// <summary>
        /// To Get the Types of the Reason 
        /// </summary>
        public SelectList ReasonType
        {
            get
            {
                List<SelectListItem> classTypeList = new List<SelectListItem>();

                IMasterDAL objDAL = new MasterDAL();

                classTypeList = objDAL.GetReasonCode();

                return new SelectList(classTypeList, "Value", "Text");
           }
       }
    }
}