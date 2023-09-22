using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PMGSY.Models.Gepnic
{
    public class GepnicNregaModel
    {
        [Display(Name = "Year")]
        public int IMS_YEAR { get; set; }
        public List<SelectListItem> Years { set; get; }

        [Display(Name = "State Name")]
        public int MAST_STATE_CODE { get; set; }
        public List<SelectListItem> STATES { get; set; }

        [Display(Name = "Integration Type")]
        public string IntegrationType { get; set; }
        public List<SelectListItem> IntegrationTypeList { get; set; }
    }

    public class GepnicRoadDetailsModel
    {
        public int ROAD_CODE { get; set; }
        public string ROAD_NAME { get; set; }
        public decimal ROAD_LENGTH { get; set; }
        public int BLOCK_CODE { get; set; }
        public string BLOCK_NAME { get; set; }
    }


}