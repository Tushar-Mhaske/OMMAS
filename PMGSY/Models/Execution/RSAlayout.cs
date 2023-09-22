using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PMGSY.Models.Execution
{
    public class RSAlayout
    {
        public string BlockName { get; set; }
        public string Package { get; set; }
        public string RoadName { get; set; }
        public string SanctionYear { get; set; }

        public double Sanction_Cost { get; set; }
        public decimal Sanction_length { get; set; }
        public decimal? AgreementCost { get; set; }

        public string AgreementDate { get; set; }
        public decimal changedLength { get; set; }

        public string EncryptedRoadCode { get; set; }
        public int prRoadCode { get; set; }
        public int cnRoadCode { get; set; }


        [RegularExpression("[PALFXCDNGO]", ErrorMessage = "Please Select valid RSA Stage")] //   DNGO added on 08 Sept 2020
        public string stageCode { get; set; }
        public List<SelectListItem> stageList { get; set; }

      

        public bool isTSC { get; set; }
        public bool isPIC { get; set; }
        public bool isPIURRNMU { get; set; }

        [Required(ErrorMessage = "Please select Inspection Date")]
        [RegularExpression(@"^(((0[1-9]|[12]\d|3[01])\/(0[13578]|1[02])\/((19|[2-9]\d)\d{2}))|((0[1-9]|[12]\d|30)\/(0[13456789]|1[012])\/((19|[2-9]\d)\d{2}))|((0[1-9]|1\d|2[0-8])\/02\/((19|[2-9]\d)\d{2}))|(29\/02\/((1[6-9]|[2-9]\d)(0[48]|[2468][048]|[13579][26])|((16|[2468][048]|[3579][26])00))))$", ErrorMessage = "Date is not in valid format")]
        public string auditDate { get; set; }


        public string RoadStatus { get; set; }
        public string InspectionDate { get; set; }
        public string Operation { get; set; }



    }
}