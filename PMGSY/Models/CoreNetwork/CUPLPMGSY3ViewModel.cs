using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PMGSY.Models.CoreNetwork
{
    public class CUPLPMGSY3ViewModel
    {
        [Required(ErrorMessage = "Please select valid District.")]
        [Range(1, int.MaxValue, ErrorMessage = "Please select valid District.")]
        public int districtCode { get; set; }
        public List<SelectListItem> lstDistricts { get; set; }

        [Required(ErrorMessage = "Please select valid Date.")]
        [RegularExpression(@"^(((0[1-9]|[12]\d|3[01])\/(0[13578]|1[02])\/((19|[2-9]\d)\d{2}))|((0[1-9]|[12]\d|30)\/(0[13456789]|1[012])\/((19|[2-9]\d)\d{2}))|((0[1-9]|1\d|2[0-8])\/02\/((19|[2-9]\d)\d{2}))|(29\/02\/((1[6-9]|[2-9]\d)(0[48]|[2468][048]|[13579][26])|((16|[2468][048]|[3579][26])00))))$", ErrorMessage = "Generation Date must be in dd/mm/yyyy format.")]
        public string generationDate { get; set; }

        [Range(2000, 2099, ErrorMessage = "Please select valid Year.")]
        public int Year { get; set; }
        public List<SelectListItem> lstYears { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Please select valid Batch.")]
        public int Batch { get; set; }
        public List<SelectListItem> lstBatch { get; set; }

        public int stateCode { get; set; }
        public int distCode { get; set; }
        public int blockCode { get; set; }

        public string stateName { get; set; }
        public string districtName { get; set; }
        public string blockName { get; set; }
    }
}