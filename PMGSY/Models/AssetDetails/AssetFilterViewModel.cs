using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PMGSY.Models.AssetDetails
{
    public class AssetFilterViewModel
    {

        public AssetFilterViewModel()
        {
            ddlMonth = new List<SelectListItem>();
            ddlYear = new List<SelectListItem>();
        }

        [Range(1,12,ErrorMessage="Please Select Month.")]
        public short Month { get; set; }

        [Range(2000,2099,ErrorMessage="Please Select Year.")]
        public short Year { get; set; }

        public string ChequeNo { get; set; }

        public string BillNo { get; set; }

        public List<SelectListItem> ddlMonth { get; set; }

        public List<SelectListItem> ddlYear { get; set; }

        public string Urlparameter { get; set; }

    }
}