using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PMGSY.Common;
using PMGSY.Extensions;
using System.Web.Mvc;
using System.ComponentModel.DataAnnotations;
using PMGSY.DAL.Master;

namespace PMGSY.Models.Master
{
    public class IMSEcTrainingViewModel
    {
        public IMSEcTrainingViewModel()
        {
            CommonFunctions commonFunctions = new CommonFunctions();
            MasterDAL objDal = new MasterDAL();
            StateList = new List<SelectListItem>();
            StateName = PMGSYSession.Current.StateCode == 0 ? "0" : PMGSYSession.Current.StateName.Trim();
            StateList = commonFunctions.PopulateStates(true);
            // StateList.Insert(0, (new SelectListItem { Text = "All States", Value = "0"}));
            StateCode = PMGSYSession.Current.StateCode == 0 ? 0 : PMGSYSession.Current.StateCode;
            StateList.Find(x => x.Value == StateCode.ToString()).Selected = true;
            Mast_DesignationList = new List<SelectListItem>();
            Mast_DesignationList = commonFunctions.PopulateDesignationSpecific(false);
            //Mast_DesignationList.Find(x => x.Value == "-1").Value = "0";
            PhaseYear = DateTime.Now.Year;
            PhaseYearList = new SelectList(commonFunctions.PopulateFinancialYear(false, false), "Value", "Text").ToList();
        }

        public string StateName { get; set; }
        public string Designation_Name { get; set; }     
       
        [UIHint("hidden")]
        public string EncryptedMastTrainingId { get; set; }

        public int MAST_Training_ID { get; set; }

        [Display(Name = "State")]
        [Required(ErrorMessage = "Please select State. ")]
        [Range(1, int.MaxValue, ErrorMessage = "Please select  State.")]
        public int StateCode { get; set; }
        public List<SelectListItem> StateList { get; set; }

        [Display(Name = "Year")]
        [Range(1, 2090, ErrorMessage = "Please select Year.")]
        [Required(ErrorMessage = "Please select Year.")]
        [RegularExpression("^([0-9]+)$", ErrorMessage = "Year must be valid number.")]
        public int PhaseYear { get; set; }
        public List<SelectListItem> PhaseYearList { get; set; }

      

        [Display(Name = "Designation")]
        [Range(1, int.MaxValue, ErrorMessage = "Please select Designation.")]
        [Required(ErrorMessage = "Please select Designation.")]
        public int Mast_Designation { get; set; }
        public List<SelectListItem> Mast_DesignationList { get; set; }

        [Display(Name = "Total Person")]
        [Range(1, 100000, ErrorMessage = "Total Person must be valid number.")]
        [Required(ErrorMessage = "Please enter Total Person.")]
        [RegularExpression(@"^([0-9]+)$", ErrorMessage = "Total Person must be valid number.")]
     
        public int IMS_TOTAL_PERSON { get; set; }
     

      
    }

    public class IMSEcTrainingSearchViewModel
        
    {
        public IMSEcTrainingSearchViewModel()
        {
            CommonFunctions commonFunctions = new CommonFunctions();
            MasterDAL objDal = new MasterDAL();
            StateList = new List<SelectListItem>();
            StateName = PMGSYSession.Current.StateCode == 0 ? "0" : PMGSYSession.Current.StateName.Trim();
            StateList = commonFunctions.PopulateStates(false);
            StateList.Insert(0, (new SelectListItem { Text = "All States", Value = "0"}));
            StateCode = PMGSYSession.Current.StateCode == 0 ? 0 : PMGSYSession.Current.StateCode;
            StateList.Find(x => x.Value == StateCode.ToString()).Selected = true;
            Mast_DesignationList = new List<SelectListItem>();
            Mast_DesignationList = commonFunctions.PopulateDesignationSpecific(true);   
            PhaseYear = DateTime.Now.Year;
            PhaseYearList = new SelectList(commonFunctions.PopulateFinancialYear(true, true), "Value", "Text").ToList();
        }


        public string StateName { get; set; }   

        public int MAST_Training_ID { get; set; }

        [Display(Name = "State")]
        [Required(ErrorMessage = "Please select State. ")]
        [Range(0, int.MaxValue, ErrorMessage = "Please select  State.")]
        public int StateCode { get; set; }
        public List<SelectListItem> StateList { get; set; }

        [Display(Name = "Year")]
        [Range(0, 2090, ErrorMessage = "Please select Year.")]
        [Required(ErrorMessage = "Please select Year.")]
        [RegularExpression("^([0-9]+)$", ErrorMessage = "Year must be valid number.")]
        public int PhaseYear { get; set; }
        public List<SelectListItem> PhaseYearList { get; set; }

        [Display(Name = "Designation")]
        [Range(0, int.MaxValue, ErrorMessage = "Please select Designation.")]
        [Required(ErrorMessage = "Please select Designation.")]
        public int Mast_Designation { get; set; }
        public List<SelectListItem> Mast_DesignationList { get; set; }

      

    }
}