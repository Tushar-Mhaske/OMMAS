using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PMGSY.Models.CoreNetwork
{
    public class PCIFinalizationViewModel
    {
        [Required(ErrorMessage = "Please select valid District.")]
        [Range(1, int.MaxValue, ErrorMessage = "Please select valid District.")]
        public int districtCode { get; set; }
        public List<SelectListItem> lstDistricts { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Please select valid Block.")]
        public int blockCode { get; set; }
        public List<SelectListItem> lstBlocks { get; set; }

        [RegularExpression(@"^(((0[1-9]|[12]\d|3[01])\/(0[13578]|1[02])\/((19|[2-9]\d)\d{2}))|((0[1-9]|[12]\d|30)\/(0[13456789]|1[012])\/((19|[2-9]\d)\d{2}))|((0[1-9]|1\d|2[0-8])\/02\/((19|[2-9]\d)\d{2}))|(29\/02\/((1[6-9]|[2-9]\d)(0[48]|[2468][048]|[13579][26])|((16|[2468][048]|[3579][26])00))))$", ErrorMessage = "Final Payment Date must be in dd/mm/yyyy format.")]
        public string finalizationDate { get; set; }

        public bool isFinalized { get; set; }
    }
}