/*----------------------------------------------------------------------------------------
 * Project Id         :

 * Project Name       :OMMAS-II

 * File Name          :MasterDesignationViewModel.cs
 
 * Author             :Vikram Nandanwar.

 * Creation Date      :01/May/2013

 * Desc               :This class is used to declare the variables, lists that are used in the Details form.
 
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
    public class MasterDesignationViewModel
    {

        [UIHint("Hidden")]
        public string EncryptedDesignationCode { get; set; }
        public int MAST_DESIG_CODE { get; set; }
        
        [Required(ErrorMessage="Designation Name is required.")]
        [Display(Name="Designation Name")]
        [RegularExpression(@"^([a-zA-Z .,&()-]+)$", ErrorMessage ="Designation Name is not in valid format.")]
        public string MAST_DESIG_NAME { get; set; }

        [Required(ErrorMessage="Please select Designation Type.")]
        [Display(Name = "Designation Type")]
        [RegularExpression(@"^([a-zA-Z1-9]+)$", ErrorMessage = "Please select Designation Type.")]
        public string MAST_DESIG_TYPE { get; set; }
    
        public virtual ICollection<ADMIN_NODAL_OFFICERS> ADMIN_NODAL_OFFICERS { get; set; }
        public virtual ICollection<ADMIN_QUALITY_MONITORS> ADMIN_QUALITY_MONITORS { get; set; }
        public virtual ICollection<ADMIN_SQC> ADMIN_SQC { get; set; }
        public virtual ICollection<ADMIN_TECHNICAL_AGENCY> ADMIN_TECHNICAL_AGENCY { get; set; }

        /// <summary>
        /// To Get Designation Type
        /// </summary>
        public SelectList DesigType
        {

            get
            {
                List<SelectListItem> list = new List<SelectListItem>();

                IMasterDAL objDAL = new MasterDAL();

                list = objDAL.GetDesigType();

                return new SelectList(list, "Value", "Text");
            }

        }

    }
}