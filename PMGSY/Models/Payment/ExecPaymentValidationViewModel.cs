using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PMGSY.Models.Payment
{
    public class ExecPaymentValidationViewModel
    {

        [Range(1, int.MaxValue, ErrorMessage = "Please select Sanction Year.")]
        public int Year { get; set; }
        public List<SelectListItem> lstYear { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Please select Road.")]
        public int roadCode { get; set; }
        public List<SelectListItem> lstRoadCode { get; set; }

        [Range(1, Int16.MaxValue, ErrorMessage = "Please Select SRRDA")]
        public int SRRDA { get; set; }
        public List<SelectListItem> lstSRRDA { get; set; }

        //[Range(0, Int16.MaxValue, ErrorMessage = "Please Select DPIU")]
        public int DPIU { get; set; }
        public List<SelectListItem> lstDPIU { get; set; }

        [Required(ErrorMessage = "SRRDA/DPIU Selection is required.")]
        [RegularExpression("[SD]", ErrorMessage = "Please Select SRRDA/DPIU")]
        //[Range(0, int.MaxValue, ErrorMessage = "Please Select feedback through")]
        public string srrda_Dpiu { get; set; }

        [Required(ErrorMessage = "Road Selection is required.")]
        [RegularExpression("[01]", ErrorMessage = "Please Select All/Road")]
        //[Range(0, int.MaxValue, ErrorMessage = "Please Select feedback through")]
        public string roadSelection { get; set; }

        [Required(ErrorMessage = "Please select From Date.")]
        [RegularExpression(@"^(((0[1-9]|[12]\d|3[01])\/(0[13578]|1[02])\/((19|[2-9]\d)\d{2}))|((0[1-9]|[12]\d|30)\/(0[13456789]|1[012])\/((19|[2-9]\d)\d{2}))|((0[1-9]|1\d|2[0-8])\/02\/((19|[2-9]\d)\d{2}))|(29\/02\/((1[6-9]|[2-9]\d)(0[48]|[2468][048]|[13579][26])|((16|[2468][048]|[3579][26])00))))$", ErrorMessage = "From Date is not in valid format")]
        public string fromDate { get; set; }

        [Required(ErrorMessage = "Please select To Date.")]
        [RegularExpression(@"^(((0[1-9]|[12]\d|3[01])\/(0[13578]|1[02])\/((19|[2-9]\d)\d{2}))|((0[1-9]|[12]\d|30)\/(0[13456789]|1[012])\/((19|[2-9]\d)\d{2}))|((0[1-9]|1\d|2[0-8])\/02\/((19|[2-9]\d)\d{2}))|(29\/02\/((1[6-9]|[2-9]\d)(0[48]|[2468][048]|[13579][26])|((16|[2468][048]|[3579][26])00))))$", ErrorMessage = "To Date is not in valid format")]
        public string toDate { get; set; }

    }
}