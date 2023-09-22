using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using PMGSY.Common;

namespace PMGSY.Models.Bank
{
    public class BankPdfKeyViewModel
    {
        [Display(Name = "Fund Type")]
        [RegularExpression("[PMA]", ErrorMessage = "Invalid Fund Type")]
        [Required(ErrorMessage = "Please select Fund Type.")]
        public String FundType { get; set; }

        [Display(Name = "Agency")]
        //[Range(1,int.MaxValue,ErrorMessage="Please select Agency")]
        public int AdminNdCode { get; set; }

        public String hdnBankCode { get; set; }
        public string hdnGeneratedKey { get; set; }
        public string StateAgencyName { get; set; }

        public SelectList PopulateFundType
        {

            get
            {
                CommonFunctions comFunc = new CommonFunctions();
                List<SelectListItem> lstFundTypes = comFunc.PopulateFundTypes(false);
                lstFundTypes.Insert(0, new SelectListItem { Text = "--Select Fund Type--", Value = "" });
                return new SelectList(lstFundTypes, "Value", "Text");
            }
        }

        public SelectList PopulateAgency
        {
            get
            {
                CommonFunctions comFunc = new CommonFunctions();
                List<SelectListItem> lstAgency = comFunc.PopulateNodalAgencies();
                lstAgency.Insert(0, new SelectListItem { Text = "--All--", Value = "0" });
                return new SelectList(lstAgency, "Value", "Text");
            }
        }
    }
}