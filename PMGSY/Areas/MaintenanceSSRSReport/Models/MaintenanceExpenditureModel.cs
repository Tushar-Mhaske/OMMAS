using PMGSY.Common;
using PMGSY.Extensions;
using PMGSY.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PMGSY.Areas.MaintenanceSSRSReport.Models
{
    public class MaintenanceExpenditureModel
    {
        public MaintenanceExpenditureModel()
        {
            CommonFunctions commonFunctions = new CommonFunctions();
            StateList = new List<SelectListItem>();
            DistrictList = new List<SelectListItem>();
            BlockList = new List<SelectListItem>();

            StateName = PMGSYSession.Current.StateCode == 0 ? "0" : PMGSYSession.Current.StateName.Trim();
            DistName = PMGSYSession.Current.DistrictCode == 0 ? "0" : PMGSYSession.Current.DistrictName.Trim();

            Mast_State_Code = PMGSYSession.Current.StateCode;
            Mast_District_Code = PMGSYSession.Current.DistrictCode;

            LevelCode = PMGSYSession.Current.DistrictCode > 0 ? 2 : 1;

            StateList = PopulateStatesEXP();//commonFunctions.PopulateStates(false);
            StateCode = PMGSYSession.Current.StateCode == 0 ? 0 : PMGSYSession.Current.StateCode;
            StateList.Find(x => x.Value == StateCode.ToString()).Selected = true;

            DistrictList = new List<SelectListItem>();
            DistrictList.Insert(0, (new SelectListItem { Text = "All District", Value = "0", Selected = true }));

            //if (StateCode == 0)
            //{
                
            //}
            //else
            //{
            //    DistrictList = commonFunctions.PopulateDistrict(StateCode, false);
            //    DistrictCode = PMGSYSession.Current.DistrictCode == 0 ? 0 : PMGSYSession.Current.DistrictCode;
            //    DistrictList.Find(x => x.Value == DistrictCode.ToString()).Selected = true;

            //}
            BlockList = new List<SelectListItem>();
            BlockList.Insert(0, (new SelectListItem { Text = "All Block", Value = "0", Selected = true }));
            //if (DistrictCode == 0)
            //{
              
            //}
            //else
            //{
            //    BlockList = commonFunctions.PopulateBlocks(DistrictCode, true);
            //    BlockList.Find(x => x.Value == "-1").Value = "0";
            //}

            PhaseYear = DateTime.Now.Year;
            PhaseYearList = new SelectList(PopulateFinancialYear(true, false), "Value", "Text").ToList();

            RoadWise = true;

            lstscheme = PopulateScheme();


        
        }

        public SelectList PopulateFinancialYear(bool populateFirstItem = true, bool isAllYearsSelected = false)
        {
            try
            {
                PMGSYEntities dbContext = new PMGSYEntities();

                int month = DateTime.Now.Month;

                if (month > 3)
                {
                    List<MASTER_YEAR> lstYears = dbContext.MASTER_YEAR.Where(y => y.MAST_YEAR_CODE <= DateTime.Now.Year).OrderByDescending(y => y.MAST_YEAR_CODE).ToList<MASTER_YEAR>();
                    if (populateFirstItem && isAllYearsSelected)
                    {
                        lstYears.Insert(0, new MASTER_YEAR() { MAST_YEAR_CODE = 0, MAST_YEAR_TEXT = "All Years" });
                    }
                    else
                    {
                        lstYears.Insert(0, new MASTER_YEAR() { MAST_YEAR_CODE = 0, MAST_YEAR_TEXT = "Select Year " });
                    }

                    return new SelectList(lstYears, "MAST_YEAR_CODE", "MAST_YEAR_TEXT", DateTime.Now.Year);
                }
                else {
                    List<MASTER_YEAR> lstYears = dbContext.MASTER_YEAR.Where(y => y.MAST_YEAR_CODE < DateTime.Now.Year).OrderByDescending(y => y.MAST_YEAR_CODE).ToList<MASTER_YEAR>();
                    if (populateFirstItem && isAllYearsSelected)
                    {
                        lstYears.Insert(0, new MASTER_YEAR() { MAST_YEAR_CODE = 0, MAST_YEAR_TEXT = "All Years" });
                    }
                    else
                    {
                        lstYears.Insert(0, new MASTER_YEAR() { MAST_YEAR_CODE = 0, MAST_YEAR_TEXT = "Select Year " });
                    }

                    return new SelectList(lstYears, "MAST_YEAR_CODE", "MAST_YEAR_TEXT", DateTime.Now.Year);
                }
            }
            catch
            {
                return null;
            }
            finally
            {
               // dbContext.Dispose();
            }
        }


        public List<SelectListItem> PopulateStatesEXP(bool isPopulateFirstItem = true)
        {
            List<SelectListItem> StatesList = new List<SelectListItem>();
            SelectListItem item;

           
                item = new SelectListItem();
                item.Text = "All States";
                item.Value = "0";
                item.Selected = true;
                StatesList.Add(item);

            

            try
            {
                PMGSYEntities dbContext = new PMGSYEntities();
                var query = (from c in dbContext.MASTER_STATE
                             where c.MAST_STATE_ACTIVE == "Y"
                             select new
                             {
                                 Text = c.MAST_STATE_NAME,
                                 Value = c.MAST_STATE_CODE
                             }).OrderBy(c => c.Text).ToList();

                foreach (var data in query)
                {
                    item = new SelectListItem();
                    item.Text = data.Text;
                    item.Value = data.Value.ToString();
                    StatesList.Add(item);
                }
                return StatesList;
            }
            catch
            {
                return StatesList;
            }
            finally
            {
                //dbContext.Dispose();
            }
        }

        public List<SelectListItem> PopulateScheme()
        {
            List<SelectListItem> populateScheme = new List<SelectListItem>();
            try
            {
                populateScheme.Insert(0, (new SelectListItem { Text = "All Schemes", Value = "0", Selected = true }));
                populateScheme.Insert(1, (new SelectListItem { Text = "PMGSY 1", Value = "1" }));
                populateScheme.Insert(2, (new SelectListItem { Text = "PMGSY 2", Value = "2" }));
                populateScheme.Insert(3, (new SelectListItem { Text = "RCPLWE", Value = "3" }));
                populateScheme.Insert(4, (new SelectListItem { Text = "PMGSY 3", Value = "4" }));

                return populateScheme;
            }
            catch
            {
                return populateScheme;
            }
        }

        public int LevelCode { get; set; }
        public string StateName { get; set; }
        public string DistName { get; set; }
        public string BlockName { get; set; }
        public int Mast_State_Code { get; set; }
        public int Mast_Block_Code { get; set; }
        public int Mast_District_Code { get; set; }

        [Display(Name = "State")]
        [Required(ErrorMessage = "Please select State. ")]
        [Range(0, int.MaxValue, ErrorMessage = "Please select  State.")]
        public int StateCode { get; set; }
        public List<SelectListItem> StateList { get; set; }

        [Display(Name = "District")]
        [Required(ErrorMessage = "Please select District.")]
        [Range(0, int.MaxValue, ErrorMessage = "Please select District.")]
        public int DistrictCode { get; set; }
        public List<SelectListItem> DistrictList { get; set; }

        [Display(Name = "Block")]
        [Required(ErrorMessage = "Please select Block.")]
        [Range(0, int.MaxValue, ErrorMessage = "Please select Block.")]
        public int BlockCode { get; set; }
        public List<SelectListItem> BlockList { get; set; }

        [Display(Name = "Year")]
        [Range(2000, 2090, ErrorMessage = "Please select Year.")]
        [Required(ErrorMessage = "Please select Year.")]
        [RegularExpression("^([0-9]+)$", ErrorMessage = "Year must be valid number.")]
        public int PhaseYear { get; set; }
        public List<SelectListItem> PhaseYearList { get; set; }

        [Display(Name = "Roadwise")]
        public bool RoadWise { get; set; }

        [Display(Name = "Scheme")]
        public int schemeCode { get; set; }
        public List<SelectListItem> lstscheme { get; set; }

    }
}