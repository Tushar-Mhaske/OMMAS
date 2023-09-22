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
    public class IMSEcFileUploadViewModel
    {
        public IMSEcFileUploadViewModel()
        {
            CommonFunctions commonFunctions = new CommonFunctions();
            MasterDAL objDal = new MasterDAL();
            StateList = new List<SelectListItem>();
            StateName = PMGSYSession.Current.StateCode == 0 ? "0" : PMGSYSession.Current.StateName.Trim();
            StateList = commonFunctions.PopulateStates(true);
            // StateList.Insert(0, (new SelectListItem { Text = "All States", Value = "0"}));
            Mast_State_Code = PMGSYSession.Current.StateCode == 0 ? 0 : PMGSYSession.Current.StateCode;
            StateList.Find(x => x.Value == Mast_State_Code.ToString()).Selected = true;
            BatchList = commonFunctions.PopulateBatch(false);
            BatchList.Find(x => x.Value == "-1").Value = "0";
            Mast_AgencyList = new List<SelectListItem>();
            if (Mast_State_Code > 0)
            {
                Mast_AgencyList = objDal.PopulateAgencies(Mast_State_Code, false);
            }
            else
            {
                Mast_AgencyList.Insert(0, (new SelectListItem { Text = "Select Agency", Value = "0" }));
            }
           // Mast_AgencyList.Find(x => x.Value == "-1").Value = "0";
            PhaseYear = DateTime.Now.Year;
            PhaseYearList = new SelectList(commonFunctions.PopulateFinancialYear(false, false), "Value", "Text").ToList();

            ImsFileTypeList = new List<SelectListItem>();
            ImsFileTypeList.Add(new SelectListItem { Text = "Select File Type", Value = "%" });
            ImsFileTypeList.Add(new SelectListItem { Text = "Audit Report", Value = "A" });
            ImsFileTypeList.Add(new SelectListItem { Text = "Utilization Certificate", Value = "U" });
            ImsFileTypeList.Add(new SelectListItem { Text = "Others", Value = "O" });

        }

        public string StateName { get; set; }
        public string Agency_Name { get; set; }
        public string BatchName { get; set; }
        public string yearName { get; set; }

        [UIHint("hidden")]
        public string EncryptedFileId { get; set; }

        public int MAST_File_ID { get; set; }

        [Display(Name = "State")]
        [Required(ErrorMessage = "Please select State. ")]
        [Range(1, int.MaxValue, ErrorMessage = "Please select  State.")]
        public int Mast_State_Code { get; set; }
        public List<SelectListItem> StateList { get; set; }

        [Display(Name = "Year")]
        [Range(1, 2090, ErrorMessage = "Please select Year.")]
        [Required(ErrorMessage = "Please select Year.")]
        [RegularExpression("^([0-9]+)$", ErrorMessage = "Year must be valid number.")]
        public int PhaseYear { get; set; }
        public List<SelectListItem> PhaseYearList { get; set; }

        [Display(Name = "Batch")]
        [Range(0, 10, ErrorMessage = "Please select Batch.")]
        [Required(ErrorMessage = "Please select Batch.")]
        [RegularExpression("^([0-9]+)$", ErrorMessage = "Batch must be valid number.")]
        public int? Batch { get; set; }
        public List<SelectListItem> BatchList { get; set; }

        [Display(Name = "Agency")]
        [Range(1, int.MaxValue, ErrorMessage = "Please select Agency.")]
        [Required(ErrorMessage = "Please select Agency.")]
        public int Mast_Agency { get; set; }
        public List<SelectListItem> Mast_AgencyList { get; set; }

        [Display(Name = "Type")]
        [RegularExpression(@"^([AUO]+)$", ErrorMessage = "Please select File Type.")]      
        public string ImsFileType { get; set; }
        public List<SelectListItem> ImsFileTypeList { get; set; }

        public string ImsFileName { get; set; }
        public string ImsFilePath { get; set; }
        public string ErrorMessage { get; set; }

     

    }
    public class IMSEcFileUploadSearchViewModel
    
    {
        public IMSEcFileUploadSearchViewModel()
        {
            CommonFunctions commonFunctions = new CommonFunctions();
            MasterDAL objDal = new MasterDAL();
            StateList = new List<SelectListItem>();
            StateName = PMGSYSession.Current.StateCode == 0 ? "0" : PMGSYSession.Current.StateName.Trim();
            StateList = commonFunctions.PopulateStates(false);
            StateList.Insert(0, (new SelectListItem { Text = "All States", Value = "0"}));
            StateCode = PMGSYSession.Current.StateCode == 0 ? 0 : PMGSYSession.Current.StateCode;
            StateList.Find(x => x.Value == StateCode.ToString()).Selected = true;
            BatchList = commonFunctions.PopulateBatch(true);
            Mast_AgencyList = new List<SelectListItem>();
            if (StateCode > 0)
            {
                Mast_AgencyList = objDal.PopulateAgencies(StateCode, true);                
            }
            else
            {
                Mast_AgencyList.Insert(0, (new SelectListItem { Text = "All Agencies", Value = "0" }));
            }
            PhaseYear = DateTime.Now.Year;
            PhaseYearList = new SelectList(commonFunctions.PopulateFinancialYear(true, true), "Value", "Text").ToList();

        }


        public string StateName { get; set; }   

        public int MAST_EC_ID { get; set; }

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

        [Display(Name = "Batch")]
        [Range(0, 10, ErrorMessage = "Please select Batch.")]
        [Required(ErrorMessage = "Please select Batch.")]
        [RegularExpression("^([0-9]+)$", ErrorMessage = "Batch must be valid number.")]
        public int Batch { get; set; }
        public List<SelectListItem> BatchList { get; set; }

        [Display(Name = "Agency")]
        [Range(0, int.MaxValue, ErrorMessage = "Please select Agency.")]
        [Required(ErrorMessage = "Please select Agency.")]
        public int Mast_Agency { get; set; }
        public List<SelectListItem> Mast_AgencyList { get; set; }
      

    }

    public class IMSECFileImageUploadViewModel
    {
        [UIHint("hidden")]
        public string EncryptedFileId { get; set; }
        public int MAST_File_ID { get; set; }

        public int? NumberofFiles { get; set; }

        public string url { get; set; }
        public string thumbnail_url { get; set; }
        public string name { get; set; }


        public string type { get; set; } //n
        public int size { get; set; }
        public string delete_url { get; set; }
        public string delete_type { get; set; } //n


        public string file_type { get; set; }
        public string ImsFileName { get; set; }
        public string ImsFilePath { get; set; }

        //public string QMName { get; set; }
        //public string QMDesignation { get; set; }
        //public string QMType { get; set; }
        //public string QMState { get; set; }

        //Added By Abhishek kamble 26-Apr-2014
        public string ErrorMessage { get; set; }

    }
}