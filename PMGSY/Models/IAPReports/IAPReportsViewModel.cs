using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PMGSY.Models.IAPReports
{
    public class IAPReportsViewModel
    {
        [Display(Name = "Month")]
        [Range(1, 12, ErrorMessage = "Please select Month.")]
        public int MONTH { get; set; }
        public List<SelectListItem> MONTHS_LIST { set; get; }



        [Display(Name = "Year")]
        [Range(2000, 2020, ErrorMessage = "Please select Year.")]
        public int YEAR { get; set; }
        public List<SelectListItem> YEARS_LIST { set; get; }



        [Display(Name = "State")]
        //[Range(1, 2147483647, ErrorMessage = " Please select state.")]
        public int MAST_STATE_CODE { get; set; }
        public SelectList States
        {
            get
            {
                List<PMGSY.Models.MASTER_STATE> stateList = new List<PMGSY.Models.MASTER_STATE>();

                PMGSY.DAL.MasterDataEntryDAL masterDataEntryDAL = new PMGSY.DAL.MasterDataEntryDAL();

                stateList = masterDataEntryDAL.GetAllStates(false);

                stateList.Insert(0, new PMGSY.Models.MASTER_STATE() { MAST_STATE_CODE = 0, MAST_STATE_NAME = "--All--" });

                return new SelectList(stateList, "MAST_STATE_CODE", "MAST_STATE_NAME", this.MAST_STATE_CODE);
            }
        }

        public string Type  { get;  set; }
    }


}