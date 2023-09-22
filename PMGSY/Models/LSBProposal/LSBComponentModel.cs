#region File Header
/*
        * Project Id    :
        * Project Name  :   OMMAS II
        * Name          :   LSBComponentModel.cs
        * Description   :   This View Model is Used in LSB Component Views in AddLSBComponentDetails        
        * Author        :   Shyam Yadav
        * Modified By   :   Shivkumar Deshmukh
        * Creation Date :   20-05-2013
 **/
#endregion

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PMGSY.Models.Proposal
{
    public class LSBComponentModel
    {
        public string OPERATION { get; set; }

        [Required]
        public int IMS_PR_ROAD_CODE { get; set; }

        [Required]
        [Display(Name = "Component Description")]
        [RegularExpression(@"^[a-zA-Z0-9 ]+$", ErrorMessage = "Invalid Component Description, Can only contains AlphaNumeric values")]
        public int IMS_COMPONENT_CODE { get; set; }

        public List<SelectListItem> COMPONENT_TYPE { get; set; }

        [Required]
        [Display(Name = "Quantity ")]
        [Range(0, int.MaxValue, ErrorMessage = "Quantity too Large")]
        [RegularExpression(@"^[0-9]+$", ErrorMessage = "Invalid Quantity value, Can only contains numeric values")]
        public int IMS_QUANTITY { get; set; }

        [Required]
        [Display(Name = "Cost (Rs Lakhs)")]
        [Range(0, 99999, ErrorMessage = "Cost too Large")]
        [RegularExpression(@"^\s*(?=.*[0-9])\d*(?:\.\d{1,2})?\s*$", ErrorMessage = "Invalid Cost, can only contains Numeric values and 2 digits after decimal place")]
        public Nullable<decimal> IMS_TOTAL_COST { get; set; }

        [Required]
        [Display(Name = "Grade concrete (Rs Lakhs)")]
        [RegularExpression(@"^\s*(?=.*[0-9])\d*(?:\.\d{1,2})?\s*$", ErrorMessage = "Invalid Grade concrete cost, can only contains Numeric values and 2 digits after decimal place")]
        [Range(0, 99999, ErrorMessage = "Grade concrete (in lakhs) too Large")]
        public decimal IMS_GRADE_CONCRETE { get; set; }

        public virtual IMS_SANCTIONED_PROJECTS IMS_SANCTIONED_PROJECTS { get; set; }
        public virtual MASTER_COMPONENT_TYPE MASTER_COMPONENT_TYPE { get; set; }
    }
}