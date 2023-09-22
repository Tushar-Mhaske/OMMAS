/*----------------------------------------------------------------------------------------
 * Project Id       :

 * Project Name     :OMMAS-II

 * File Name        :MasterStreamsViewModel.cs
 
 * Author           :Ashish Markande

 * Creation Date    :01/May/2013

 * Desc             :This class is used to declare the variables, lists that are used in the Details form.
 
 * ---------------------------------------------------------------------------------------*/
using PMGSY.DAL.Master;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.ComponentModel.DataAnnotations;


namespace PMGSY.Models.Master
{
    public class MasterStreamsViewModel
    {   
        public string EncryptecStreamsCode { get; set; }
    
        public int MAST_STREAM_CODE { get; set; }

        [Required(ErrorMessage="Stream Name is required.")]
        [RegularExpression("[A-Za-z ._()]{1,50}", ErrorMessage = "Stream Name is not in valid format.")]
        [StringLength(50, ErrorMessage = "Stream Name must be less than 50 characters.")]
        [Display(Name="Stream Name")]
        public string MAST_STREAM_NAME { get; set; }

      
        [Display(Name="Stream Type")]
        [Required(ErrorMessage = "Please select Stream Type.")]
         public string MAST_STREAM_TYPE { get; set; }
    
        public virtual ICollection<IMS_SANCTIONED_PROJECTS> IMS_SANCTIONED_PROJECTS { get; set; }


              /// <summary>
              /// To Get the Streams Types 
              /// </summary>
        public SelectList StreamsType
        {
            get
            {
                List<SelectListItem> list = new List<SelectListItem>();
                IMasterDAL objDAL = new MasterDAL();
                list = objDAL.GetStreamsCode();
                return new SelectList(list, "Value", "Text");
                      
            }
        }

    }
}