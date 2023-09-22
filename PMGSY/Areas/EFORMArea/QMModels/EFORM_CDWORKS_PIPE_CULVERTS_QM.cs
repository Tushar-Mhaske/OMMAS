using PMGSY.Areas.EFORMArea.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using static PMGSY.Areas.EFORMArea.Common.CommonEnum;

namespace PMGSY.Areas.EFORMArea.QMModels
{ 
    public class EFORM_CDWORKS_PIPE_CULVERTS_QM
    {
        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [Required(ErrorMessage = "Page-23: Item CROSS DRAINAGE WORKS: (Pipe Culverts)- I. Please enter Total number of pipe culverts as per sanctioned DPR ")]
        [RegularExpression(pattern: @"^(?:\d{0,7}\.\d{0,3})$|^\d{0,7}$", ErrorMessage = "Page-23: Item CROSS DRAINAGE WORKS: (Pipe Culverts)-  I. Please Enter Valid number{cant be greater than 9999999.99} in Total number of pipe culverts as per sanctioned DPR ")]
        public decimal TOTAL_PIPE_CULVERTS { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [Required(ErrorMessage = "Page-23: Item CROSS DRAINAGE WORKS: (Pipe Culverts)-  II. Please enter Minimum cushion over pipe culverts as per DPR: ")]
        [RegularExpression(pattern: @"^(?:\d{0,7}\.\d{0,3})$|^\d{0,7}$", ErrorMessage = "Page-23: Item CROSS DRAINAGE WORKS: (Pipe Culverts)-  II. Please Enter Valid number{cant be greater than 9999999.99} in Minimum cushion over pipe culverts as per DPR: ")]

        public decimal MINIMUM_CUSHION { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [Required(ErrorMessage = "Page-23: Item CROSS DRAINAGE WORKS: (Pipe Culverts)-  III. Please enter Class of pipes provided in DPR ")]
        [RegularExpression(pattern: @"^(?:\d{0,7}\.\d{0,3})$|^\d{0,7}$", ErrorMessage = "Page-23: Item CROSS DRAINAGE WORKS: (Pipe Culverts)-  IV. Please Enter Valid number{cant be greater than 9999999.99} in Class of pipes provided in DPR  ")]

        public decimal CLASS_OF_PIPES_NP2 { get; set; }
       
        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [Required(ErrorMessage = "Page-23: Item CROSS DRAINAGE WORKS: (Pipe Culverts)-  III. Please enter Class of pipes provided in DPR ")]
        [RegularExpression(pattern: @"^(?:\d{0,7}\.\d{0,3})$|^\d{0,7}$", ErrorMessage = "Page-23: Item CROSS DRAINAGE WORKS: (Pipe Culverts)-  IV. Please Enter Valid number{cant be greater than 9999999.99} in Class of pipes provided in DPR  ")]

        public decimal CLASS_OF_PIPES_NP3 { get; set; }

        [FieldType(PropertyType = PDFFieldType.RadioButton)]
        [Required(ErrorMessage = "Page-23: Item CROSS DRAINAGE WORKS: (Pipe Culverts)-  IV. Please enter Grade of concrete for headwall as per DPR ")]
        [RegularExpression(@"^[A-Za-z0-9 ]{0,3}$", ErrorMessage = "Page-23: Item CROSS DRAINAGE WORKS: (Pipe Culverts)- V. Maximum three character is allowed in Grade of concrete for headwall as per DPR")]
        public string GRADE_OF_CONCRETE_23 { get; set; }

        [FieldType(PropertyType = PDFFieldType.RadioButton)]
        [Required(ErrorMessage = "Page-23: Item CROSS DRAINAGE WORKS: (Pipe Culverts)-  VI. Please select If cushion over the pipes is inadequate, whether appropriate protection to the pipes as concrete coveror concrete jacketing has been provided ")]
        [RegularExpression(@"^[A-Za-z ]{0,1}$", ErrorMessage = "Page-23: Item CROSS DRAINAGE WORKS: (Pipe Culverts)- Vi. Maximum one character is allowed in If cushion over the pipes is inadequate, whether appropriate protection to the pipes as concrete coveror concrete jacketing has been provided")]


        public string IS_PROTECTION_PROVIDED_23 { get; set; }

        [FieldType(PropertyType = PDFFieldType.RadioButton)]
        [Required(ErrorMessage = "Page-23: Item CROSS DRAINAGE WORKS: (Pipe Culverts)-  VII. Please select Whether invert level of pipe at upstream end has been appropriately placed to avoid silting of pipes ")]
        [RegularExpression(@"^[A-Za-z ]{0,1}$", ErrorMessage = "Page-23: Item CROSS DRAINAGE WORKS: (Pipe Culverts)- VII. Maximum one character is allowed in Whether invert level of pipe at upstream end has been appropriately placed to avoid silting of pipes")]

        public string IS_PIPE_PROPERLY_PLACED_23 { get; set; }

        [FieldType(PropertyType = PDFFieldType.RadioButton)]
        [Required(ErrorMessage = "Page-23: Item CROSS DRAINAGE WORKS: (Pipe Culverts)-  Please select Item Grading-13 ")]
        [RegularExpression(@"^[A-Za-z ]{0,3}$", ErrorMessage = "Page-23: Item CROSS DRAINAGE WORKS: (Pipe Culverts)- Maximum three character is allowed in Item Grading-13")]


        public string ITEM_GRADING_13 { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [Required(ErrorMessage = "Page-23: Item CROSS DRAINAGE WORKS: (Pipe Culverts)-  Please enter suggestions for improvement ")]
        [StringLength(maximumLength: 250, ErrorMessage = "Page-23: Item CROSS DRAINAGE WORKS: (Pipe Culverts)- Only 250 Characters Allowed in suggestions for improvement")]
        public string IMPROVE_SUGGESTIONS_23 { get; set; }

    }
}