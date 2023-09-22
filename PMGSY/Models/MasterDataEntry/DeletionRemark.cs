using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace PMGSY.Models.MasterDataEntry
{
    public class DeletionRemark
    {
        [Required(ErrorMessage = "Please enter remark")]
        [Display( Name = "Remark")]
        public string Remark { get; set; }
        [Required]
        public string FacilityID { get; set; }
        public string HabName { get; set; }
        public string facilityname { get; set; }
        public string blockname { get; set; }
    }
}