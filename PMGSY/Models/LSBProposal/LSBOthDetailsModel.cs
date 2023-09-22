#region File Header
/*
        * Project Id    :
        * Project Name  :   OMMAS II
        * Name          :   LSBOthDetailsModel.cs
        * Description   :   This View Model is Used in LSB Other Details Views in LSBOtherDetails.cshtml        
        * Author        :   Shyam Yadav
        * Modified By   :   Shivkumar Deshmukh
        * Creation Date :   20-05-2013
 **/
#endregion

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using PMGSY.Models;
using System.Web.Mvc;
using System.Collections;
using System.ComponentModel;
using System.Reflection;

namespace PMGSY.Models.Proposal
{
    public class LSBOthDetailsModel 
    {
        public LSBOthDetailsModel()
        {
           
        }

        public int IMS_PR_ROAD_CODE { get; set; }
        public decimal ROAD_TOP_LEVEL = 0;
        public string OPERATION { get; set; }
        public bool IS_UPDATED { get; set; }
        public string FOUNDATION_TYPE_TEXT { get; set; }
        public string SCOUR_DEPTH_TEXT { get; set; }

        //---------------------- Type of Bridge Details -------------------------//
        [Display(Name = "Road Top level (RTL)")]
        [Required]
        [RegularExpression(@"^\s*(?=.*[0-9])\d*(?:\.\d{1,2})?\s*$", ErrorMessage = "Invalid Road Top level (RTL) value, can only contains Numeric values and 2 digits after decimal place")]
        [Range(0, 99999.99, ErrorMessage = "Road Top level (RTL) value too Large")]
        public decimal IMS_ROAD_TYPE_LEVEL { get; set; }

        [Display(Name = "Highest Flood level (HFL)")]
        [Required]
        [RegularExpression(@"^\s*(?=.*[0-9])\d*(?:\.\d{1,2})?\s*$", ErrorMessage = "Invalid Flood level(HFL) value, can only contains Numeric values and 2 digits after decimal place")]
        [Range(0, 99999.99, ErrorMessage = "Highest Flood level (HFL) value too Large")]
        public decimal IMS_HIGHEST_FLOOD_LEVEL { get; set; }

        [Display(Name = "Ordinary Flood level (OFL)")]
        [Required]
        [RegularExpression(@"^\s*(?=.*[0-9])\d*(?:\.\d{1,2})?\s*$", ErrorMessage = "Invalid Ordinary Flood level(OFL) value, can only contains Numeric values and 2 digits after decimal place")]
        [Range(0, 99999.99, ErrorMessage = "Ordinary Flood level (OFL) value too Large")]
        public decimal IMS_ORDINARY_FLOOD_LEVEL { get; set; }

        [Display(Name = "Average Ground level (AGL)")]
        [Required]
        [RegularExpression(@"^\s*(?=.*[0-9])\d*(?:\.\d{1,2})?\s*$", ErrorMessage = "Invalid Average Ground level(AGL) value, can only contains Numeric values and 2 digits after decimal place")]
        [Range(0, 99999.99, ErrorMessage = "Average Ground level (AGL) value too Large")]
        public decimal IMS_AVERAGE_GROUND_LEVEL { get; set; }

        [Display(Name = "Nala Bed level (NBL)")]
        [Required]
        [RegularExpression(@"^\s*(?=.*[0-9])\d*(?:\.\d{1,2})?\s*$", ErrorMessage = "Invalid Nala Bed level(NBL) value, can only contains Numeric values and 2 digits after decimal place")]
        [Range(0,99999.99,ErrorMessage = "Nala Bed level(NBL) value too Large")]
        public decimal IMS_NALA_BED_LEVEL { get; set; }

        [Display(Name = "Foundation level (FL)")]
        [Required]
        [RegularExpression(@"^\s*(?=.*[0-9])\d*(?:\.\d{1,2})?\s*$", ErrorMessage = "Invalid Foundation level(FL) value, can only contains Numeric values and 2 digits after decimal place")]
        [Range(0,99999.99, ErrorMessage = "Foundation level(FL) value too Large")]
        public decimal IMS_FOUNDATION_LEVEL { get; set; }

        [Display(Name = "Ht.of bridge h=(RTL-NBL)")]
        [Required]
        public decimal IMS_HGT_BIRDGE_NBL { get; set; }

        [Display(Name = "Ht.of bridge h=(RTL-FL)")]
        [Required]
        public decimal IMS_HGT_BRIDGE_FL { get; set; }


        //---------------------- Type of Bridge Details Ends-------------------------//



        //---------------------- Type of Proposed Bridge -------------------------//

        [Display(Name = "Submersible Structures like Vented Causeway or Submersible Bridge")]
        public bool IMS_BRG_SUBMERSIBLE { get; set; }

        [Display(Name = "Box Culvert")]
        public bool IMS_BRG_BOX_CULVERT { get; set; }

        [Display(Name = "Bridge with RCC Piers and Abutments")]
        public bool IMS_BRG_RCC_ABUMENT { get; set; }

        [Display(Name = "High Level Bridge")]
        public bool IMS_BRG_HLB { get; set; }

        [Required]
        [Display(Name = "Type of Foundation")]
        public int IMS_SC_FD_CODE { get; set; }
        public List<SelectListItem> FOUNDATION_CODE { get; set; }

        [Required]
        [Display(Name = "Bearing Capacity at Foundation Level")]
        [Range(0, 99999.99, ErrorMessage="Bearing Capacity at Foundation Level too Large")]
        [RegularExpression(@"^\s*(?=.*[0-9])\d*(?:\.\d{1,2})?\s*$", ErrorMessage = "Invalid Width of Bridge, can only contains Numeric values and 2 digits after decimal place")]
        public decimal IMS_BEARING_CAPACITY { get; set; }

        //---------------------- Type of Proposed Bridge Ends -------------------------//




        //---------------------- Arrangement of Spans -------------------------//
        [Required]
        [Display(Name = "Total Spans")]
        [RegularExpression(@"^[0-9]+$", ErrorMessage = "Invalid Total Spans value, Can only contains numeric values")]
        [Range(0, int.MaxValue, ErrorMessage = "Total Spans value too large")]
        public int IMS_ARG_TOT_SPANS { get; set; }

        [Required]
        [Display(Name = "No.of Vents")]
        [RegularExpression(@"^[0-9]+$", ErrorMessage = "Invalid No.of Vents value, Can only contains numeric values")]
        [Range(0, int.MaxValue, ErrorMessage = "No.of Vents value too large")]
        public int IMS_NO_OF_VENTS { get; set; }

        [Required]
        [Display(Name = "Clear Span of Vent")]
        [RegularExpression(@"^[0-9]+$", ErrorMessage = "Invalid Clear Span of Vent value, Can only contains numeric values")]
        [Range(0, int.MaxValue, ErrorMessage = "Clear Span of Vent value too large")]        
        public int IMS_SPAN_VENT { get; set; }

        [Required]
        [Display(Name = "Maximum Scour Depth")]
        public int IMS_SCOUR_DEPTH { get; set; }
        public List<SelectListItem> SCOUR_DEPTH { get; set; }

        [Required]
        [Display(Name = "Width of Bridge")]
        [RegularExpression(@"^\s*(?=.*[0-9])\d*(?:\.\d{1,2})?\s*$", ErrorMessage = "Invalid Width of Bridge, can only contains Numeric values and 2 digits after decimal place")]
        public decimal IMS_WIDTH_OF_BRIDGE { get; set; }

        public List<SelectListItem> WIDTH_OF_BRIDGE { get; set; }

        //---------------------- Arrangement of Spans Ends-------------------------//



        //---------------------- Total Cost -------------------------//
        [Display(Name = "Approaches(Rs Lakhs)")]
        [Required]
        [RegularExpression(@"^\s*(?=.*[0-9])\d*(?:\.\d{1,4})?\s*$", ErrorMessage = "Invalid total cost for Approaches, can only contains Numeric values and 4 digits after decimal place")]
        [Range(0, 99999, ErrorMessage = "Total cost for Approaches is too Large.")]
        public decimal IMS_APPROACH_COST { get; set; }

        [Display(Name = "Super Structure(Rs Lakhs)")]
        [Required]
        [RegularExpression(@"^\s*(?=.*[0-9])\d*(?:\.\d{1,4})?\s*$", ErrorMessage = "Invalid total cost for Super Structure, can only contains Numeric values and 4 digits after decimal place")]
        [Range(0, 99999, ErrorMessage = "Total cost for super structure is too large.")]
        public decimal IMS_BRGD_STRUCTURE_COST { get; set; }

        [Display(Name = "Sub Structure(Rs Lakhs)")]
        [Required]
        [RegularExpression(@"^\s*(?=.*[0-9])\d*(?:\.\d{1,4})?\s*$", ErrorMessage = "Invalid total cost for Sub Structure, can only contains Numeric values and 4 digits after decimal place")]
        [Range(0, 99999, ErrorMessage = "Total cost for sub structure is too large.")]
        public decimal IMS_STRUCTURE_COST { get; set; }

        [Display(Name = "Others(Rs Lakhs)")]
        [Required]
        [RegularExpression(@"^\s*(?=.*[0-9])\d*(?:\.\d{1,4})?\s*$", ErrorMessage = "Invalid total cost for Others, can only contains Numeric values and 4 digits after decimal place")]
        [Range(0, 99999, ErrorMessage = "Total cost for others is too large.")]
        public decimal IMS_BRGD_OTHER_COST { get; set; }

        //---------------------- Total Cost -------------------------//

        //----------------------Cost per KM. -------------------------//
        [Display(Name = "Cost per km. for Approaches(Rs Lakhs)")]
        [Required]
        [RegularExpression(@"^\s*(?=.*[0-9])\d*(?:\.\d{1,4})?\s*$", ErrorMessage = "Invalid cost per km. for Approaches, can only contains Numeric values and 4 digits after decimal place")]
        [Range(0, 99999, ErrorMessage = "Cost per km. for Approaches is too Large.")]
        public decimal IMS_APPROACH_PER_MTR { get; set; }

        [Display(Name = "Cost per km. for Super Structure(Rs Lakhs)")]
        [Required]
        [RegularExpression(@"^\s*(?=.*[0-9])\d*(?:\.\d{1,4})?\s*$", ErrorMessage = "Invalid cost per km. for Super Structure, can only contains Numeric values and 4 digits after decimal place")]
        [Range(0, 99999, ErrorMessage = "Cost per km. for super structure is too large.")]
        public decimal IMS_BRGD_STRUCTURE_PER_MTR { get; set; }

        [Display(Name = "Cost per km. for Sub Structure(Rs Lakhs)")]
        [Required]
        [RegularExpression(@"^\s*(?=.*[0-9])\d*(?:\.\d{1,4})?\s*$", ErrorMessage = "Invalid cost per km. for Sub Structure, can only contains Numeric values and 4 digits after decimal place")]
        [Range(0, 99999, ErrorMessage = "Cost per km. for sub structure is too large.")]
        public decimal IMS_STRUCTURE_PER_MTR { get; set; }

        [Display(Name = "Cost per km. for Others(Rs Lakhs)")]
        [Required]
        [RegularExpression(@"^\s*(?=.*[0-9])\d*(?:\.\d{1,4})?\s*$", ErrorMessage = "Invalid cost per km. for Others, can only contains Numeric values and 4 digits after decimal place")]
        [Range(0, 99999, ErrorMessage = "Cost per km. for others is too large.")]
        public decimal IMS_BRGD_OTHER_PER_MTR { get; set; }
       
        //----------------------Cost per KM. -------------------------//

        [Display(Name = "Total(Rs Lakhs)")]
        public decimal TotalEstimatedCost { get; set; }


        public virtual IMS_SANCTIONED_PROJECTS IMS_SANCTIONED_PROJECTS { get; set; }
        public virtual MASTER_SCOUR_FOUNDATION_TYPE MASTER_SCOUR_FOUNDATION_TYPE { get; set; }
        public virtual MASTER_SCOUR_FOUNDATION_TYPE MASTER_SCOUR_FOUNDATION_TYPE1 { get; set; }
       
    }
}
