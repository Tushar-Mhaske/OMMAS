/*----------------------------------------------------------------------------------------
 * Project Id             :

 * Project Name           :OMMAS-II

 * File Name              :MasterAgencyViewModel.cs
 
 * Author                 :Rohit Jadhav 

 * Creation Date          :01/May/2013

 * Desc                   :This class is used to declare the variables, lists that are used in the Details form.
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
    public class MasterAgencyViewModel
    {
        
        [UIHint("hidden")]
        public string EncryptedAgencyCode { get; set; }
        public int MAST_AGENCY_CODE { get; set; }
   
        [Display(Name = "Agency Name")]
        [Required(ErrorMessage = "Agency Name is required.")]
        [RegularExpression(@"^[a-zA-Z0-9 ._()&/]+$", ErrorMessage = "Agency Name is not in valid format.")]
        [StringLength(50, ErrorMessage = "Agency Name must be less than 50 characters.")]
        public string MAST_AGENCY_NAME { get; set; }


        [Required(ErrorMessage="Please select Agency Type.")]
        [Display(Name = "Agency Type")]
        public string MAST_AGENCY_TYPE { get; set; }

        /// <summary>
        /// To Get the Types of the Agency 
        /// </summary>
        public SelectList AgencyType
        {
            get
            { 
                List<SelectListItem> classTypeList = new List<SelectListItem>();

                IMasterDAL objDAL = new MasterDAL();

                classTypeList = objDAL.GetAgencyCode();

                return new SelectList(classTypeList, "Value", "Text");
            }
        }
        public virtual ICollection<ADMIN_DEPARTMENT> ADMIN_DEPARTMENT { get; set; }
        public virtual ICollection<MASTER_EXISTING_ROADS> MASTER_EXISTING_ROADS { get; set; }
    }

}