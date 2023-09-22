using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;


namespace PMGSY.Models.ExistingRoads
{
    public class MapDRRPUnderPMGSY3
    {
            public MapDRRPUnderPMGSY3()
            {
                lstBlocks = new List<SelectListItem>();
                lstCoreNetworks = new List<SelectListItem>();
            }

            public string WorkName { get; set; }

            public string PackageName { get; set; }

            public int ProposalCode { get; set; }

            public string ProposalType { get; set; }

            public string UpgradeConnect { get; set; }

            
            [Display(Name = "District")]
            [Range(0, Int32.MaxValue, ErrorMessage = "Please select District.")]
            [Required(ErrorMessage = "Please select District.")]
            public int District { get; set; }
            public List<SelectListItem> lstDistrict { get; set; }
                

            [Display(Name = "Block")]
            [Range(0, Int32.MaxValue, ErrorMessage = "Please select Block.")]
            [Required(ErrorMessage = "Please select Block.")]
            public int Block { get; set; }
            public List<SelectListItem> lstBlocks { get; set; }

            [Display(Name = "DRRP")]
           // [Required(ErrorMessage = "Please select DRRP.")]
            [Range(1, Int32.MaxValue, ErrorMessage = "Please select DRRP.")]
            public int CnCode { get; set; }
            public List<SelectListItem> lstCoreNetworks { get; set; }

    }  
}
