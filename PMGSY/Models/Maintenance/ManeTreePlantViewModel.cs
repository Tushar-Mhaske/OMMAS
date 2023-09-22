using PMGSY.Model.Maintenance;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace PMGSY.Model.Maintenance
{
    public class ManeTreePlantHeaderViewModel
    {
        public int RoadCode { get; set; }
        public string RoadName { get; set; }
        public string StateName { get; set; }
        public string DistrictName { get; set; }
        public string BlockName { get; set; }
        public string SanctionYear { get; set; }
        public string Package { get; set; }

        public int roleCode { get; set; }
        public int obsId { get; set; }
        public int verifyCount { get; set; }
    }

    public class ManeTreePlantViewModel
    {

        public ManeTreePlantViewModel()
        {
            TreePLant = new ManeTreePlantModel();
        }
        public ManeTreePlantModel TreePLant { get; set; }
        public List<SelectListItem> YEAR_LIST { get; set; }
        public List<SelectListItem> MONTH_LIST { get; set; }
        public int hdRoleCode { get; set; }
    }

    public class ManeTreePlantVerifyViewModel
    {
        public int scheduleCode { get; set; }
        public int observationId { get; set; }
        public int roadCode { get; set; }

        [Required(ErrorMessage = "Please Select Yes or No")]
        [RegularExpression("[YN]", ErrorMessage = "Invalid selection Please Select Yes or No")]
        public string Verify { get; set; }

        [RegularExpression(@"^([-0-9a-zA-Z,.@()/ ]+)$", ErrorMessage = "Remarks has some invalid characters.")]
        public string Remarks { get; set; }
    }
}
