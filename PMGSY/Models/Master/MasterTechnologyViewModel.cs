using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace PMGSY.Models.Master
{
    public class MasterTechnologyViewModel
    {
        public MasterTechnologyViewModel()
        {
            lstTechTypes = new List<SelectListItem>();
            lstTechTypes.Insert(0, new SelectListItem { Value = "0" , Text = "Select Technology Type"});
            lstTechTypes.Insert(1, new SelectListItem { Value = "E", Text = "Technology with IRC Specifications(Mainstreaming of Existing Technology)" });
            lstTechTypes.Insert(2, new SelectListItem { Value = "A", Text = "Technology where IRC Specifications are not available.(IRC accredited Technology)" });
            lstTechTypes.Insert(3, new SelectListItem { Value = "N", Text = "Non Accredited Technology" });
        }

        [UIHint("hidden")]
        public string EncryptedTechCode { get; set; }

        public int MAST_TECH_CODE { get; set; }

        public int MAST_HEAD_CODE { get; set; }

        public string arrLayer { get; set; }
                
        [Display(Name="Name")]        
        [Required(ErrorMessage="Please enter name.")]
        [RegularExpression(@"^([a-zA-Z0-9 ]+)$", ErrorMessage = "Only Alpanumeric Characters are allowed.")]                  
        [StringLength(100,ErrorMessage="Name must be less than 100 characters")]
        public string MAST_TECH_NAME { get; set; }

        [Display(Name = "Description")]
        [Required(ErrorMessage = "Please enter description.")]
        [StringLength(255, ErrorMessage = "Description must be less than 255 characters.")]
        [RegularExpression(@"^([a-zA-Z0-9 ._',\r\n&()-]+)$", ErrorMessage = "Description is not in valid format.")]          
        public string MAST_TECH_DESC { get; set; }
                                                                  
        [Display(Name = "Active")]
        public string MAST_TECH_STATUS { get; set; }

        public bool status{ get; set; }

        [Display(Name = "Type")]
        [RegularExpression("[AEN]", ErrorMessage = "Please Select Aggregated , Existing Technology, orNon Aggregated")]
        [Required(ErrorMessage = "Please select Technology type.")]
        public string TechType { get; set; }
        public List<SelectListItem> lstTechTypes { get; set; }

        [Display(Name = "Layer")]
        [Required(ErrorMessage = "Please select Layer.")]
        [Range(0,int.MaxValue,ErrorMessage = "Please select valid Layer.")]
        public int Layer { get; set; }
        public List<SelectListItem> ListLayers { get; set; }
    }
}