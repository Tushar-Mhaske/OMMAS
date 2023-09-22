using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PMGSY.Models.Login
{
    public class StateAsPIUModel
    {
        public int STATE_CODE { get; set; }
        public int DISTRICT_CODE { get; set; }

        [Required]
        [Display(Name = "Select Department")]
        public int ADMIN_ND_CODE { get; set; }


        public List<SelectListItem> DEPARTMENT_LIST { set; get; }

        public int LEVEL_ID { get; set; }
        public int ROLE_ID { get; set; }

        [Display(Name = "User name")]
        public string USER_NAME { get; set; }
        public string STATE_NAME { get; set; }
        public string DISTRICT_NAME { get; set; }
        public string DEPARTMENT_NAME { get; set; }
        public string ROLE_NAME { get; set; }

    }
}