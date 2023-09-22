using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using PMGSY.DAL.ExistingRoads;
namespace PMGSY.Models.ExistingRoads
{
    public class CdWorksViewModel
    {
        [UIHint("hidden")]
        public string EncryptedCdWorksCode { get; set; }

        public string Operation { get; set; }

        public int MAST_ER_ROAD_CODE { get; set; }
        public int MAST_CD_NUMBER { get; set; }

        [Display(Name = "CD Works Type")]
        [Range(1, 2147483647, ErrorMessage = "Please select CD Works Type.")]        
        public int MAST_CDWORKS_CODE { get; set; }

        [Display(Name = "Length (Mtrs)")]
        [Required (ErrorMessage="Length is required")]
        [RegularExpression(@"^\s*(?=.*[0-9])\d*(?:\.\d{1,2})?\s*$", ErrorMessage = "Invalid Length,Can only contains Numeric values and 2 digits after decimal place")]
        [Range(0.01, 999.99, ErrorMessage = "Invalid Length.")]
        public Nullable<decimal> MAST_CD_LENGTH { get; set; }

        [Display(Name = "Discharge (Cu ms)")]
        [Required(ErrorMessage = "Discharge is required")]
        [RegularExpression(@"^\s*(?=.*[0-9])\d*(?:\.\d{1,2})?\s*$", ErrorMessage = "Invalid Discharge,Can only contains Numeric values and 2 digits after decimal place")]
        [Range(0.01, 999.99, ErrorMessage = "Invalid Carriageway Width.")]
        public Nullable<decimal> MAST_CD_DISCHARGE { get; set; }

        [Display(Name = "Chainage (KMs)")]
        [Required(ErrorMessage = "Chainage is required")]
        [RegularExpression(@"^\s*(?=.*[0-9])\d*(?:\.\d{1,3})?\s*$", ErrorMessage = "Invalid Chainage,Can only contains Numeric values and 3 digits after decimal place")]
        [Range(0.001, 9999.999, ErrorMessage = "Invalid Chainage Length.")]
        public Nullable<decimal> MAST_CD_CHAINAGE { get; set; }
        
        [Display(Name = "Year of Construction")]
        [Range(1950, 2099, ErrorMessage = "Please select Year.")]    
        public Nullable<int> MAST_CONSTRUCT_YEAR { get; set; }

        [CompareValidation("MAST_CONSTRUCT_YEAR", ErrorMessage = "Road Rehabilitation Year should be greater than Road Costruction Year")]
        [Display(Name = "Year of Rehabilitation")]
        [Range(1950, 2099, ErrorMessage = "Please select Year.")]      
        public Nullable<int> MAST_REHAB_YEAR { get; set; }

        [Display(Name = "Span (Mtrs)")]
        [Required(ErrorMessage = "Span is required")]
        [RegularExpression(@"^\s*(?=.*[0-9])\d*(?:\.\d{1,3})?\s*$", ErrorMessage = "Invalid Span,Can only contains Numeric values and 3 digits after decimal place")]
        [Range(0.001, 9999.999, ErrorMessage = "Invalid Span Length.")]
        public Nullable<decimal> MAST_ER_SPAN { get; set; }

        [Display(Name = "Carriage-way (Mtrs)")]
        [Required(ErrorMessage = "Carriage-way is required")]
        [RegularExpression(@"^\s*(?=.*[0-9])\d*(?:\.\d{1,3})?\s*$", ErrorMessage = "Invalid Carriage-way,Can only contains Numeric values and 3 digits after decimal place")]
        [Range(0.001, 9999.999, ErrorMessage = "Invalid Carriageway Length.")]
        public Nullable<decimal> MAST_CARRIAGE_WAY { get; set; }

        [Display(Name = "Foot Path")]
        [Required(ErrorMessage = "Please select Foot Path.")]
        [RegularExpression(@"^([YN]+)$", ErrorMessage = "Please select Foot Path.")]
        public string MAST_IS_FOOTPATH { get; set; }

        [Display(Name = "Road Number")]
        public string RoadNumber { get; set; }

        [Display(Name = "Road Name")]
        public string RoadName { get; set; }

        public SelectList RoadYears
        {
            get
            {
                List<SelectListItem> yearList = new List<SelectListItem>();

                IExistingRoadsDAL objDAL = new ExistingRoadsDAL();

                yearList = objDAL.GetConstructionYears();
                if (PMGSY.Extensions.PMGSYSession.Current.PMGSYScheme == 2)
                {
                    yearList.Find(x => x.Value == "0").Value = "";
                }
                return new SelectList(yearList, "Value", "Text");
            }
        }

        public SelectList CDWorkTypes
        {
            get
            {
                List<MASTER_CDWORKS_TYPE_CONSTRUCTION> cdWorksList = new List<MASTER_CDWORKS_TYPE_CONSTRUCTION>();

                IExistingRoadsDAL objDAL = new ExistingRoadsDAL();

                cdWorksList = objDAL.GetCDWorkTypes();

                cdWorksList.Insert(0, new MASTER_CDWORKS_TYPE_CONSTRUCTION() { MAST_CDWORKS_CODE = 0, MAST_CDWORKS_NAME = "--Select CD Works Type--" });

                return new SelectList(cdWorksList, "MAST_CDWORKS_CODE", "MAST_CDWORKS_NAME");
            }
        }

        public virtual MASTER_CDWORKS_TYPE_CONSTRUCTION MASTER_CDWORKS_TYPE_CONSTRUCTION { get; set; }
        public virtual MASTER_EXISTING_ROADS MASTER_EXISTING_ROADS { get; set; }
    }
}