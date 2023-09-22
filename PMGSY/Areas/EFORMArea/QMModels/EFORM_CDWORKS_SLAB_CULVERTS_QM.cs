using PMGSY.Areas.EFORMArea.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using static PMGSY.Areas.EFORMArea.Common.CommonEnum;

namespace PMGSY.Areas.EFORMArea.QMModels
{
    public class EFORM_CDWORKS_SLAB_CULVERTS_QM
    {
        [FieldType(PropertyType = PDFFieldType.RadioButton)]
        [Required(ErrorMessage = "Page-24: Item 14. CROSS DRAINAGE WORK: (Slab Culverts)- I. Please select Grade of concrete for slab culvert as per DPR ")]
        [RegularExpression(@"^[A-Za-z0-9 ]{0,3}$", ErrorMessage = "Page-24: Item 14. CROSS DRAINAGE WORK: (Slab Culverts)-  I. Maximum three character is allowed in Grade of concrete for slab culvert as per DPR ")]
        public string CONCRTE_GRADE_24 { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [Required(ErrorMessage = "Page-24: Item 14. CROSS DRAINAGE WORK: (Slab Culverts)- II. Please enter Total number of slab culverts as per sanctioned DPR ")]
        [RegularExpression(@"^(?:\d{0,7}\.\d{1,3})$|^\d{0,7}$", ErrorMessage = "Page-24: Item 14. CROSS DRAINAGE WORK: (Slab Culverts)-  II. Maximum seven digits before decimal and maximum three digits after decimal are allowed in Total number of slab culverts as per sanctioned DPR")]

        public decimal TOTAL_SLAB_CULVERTS { get; set; }

        [FieldType(PropertyType = PDFFieldType.RadioButton)]
        [Required(ErrorMessage = "Page-24: Item 14. CROSS DRAINAGE WORK: (Slab Culverts)-  Please select Item Grading-14 ")]
        [RegularExpression(@"^[A-Za-z ]{0,3}$", ErrorMessage = "Page-24: Item 14. CROSS DRAINAGE WORK: (Slab Culverts)-  Maximum three character is allowed in Item Grading-14")]

        public string ITEM_GRADING_14 { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [Required(ErrorMessage = "Page-24: Item 14. CROSS DRAINAGE WORK: (Slab Culverts)-  Please enter suggestions for improvement ")]
        [StringLength(maximumLength: 250, ErrorMessage = "Page-24: Item 14. CROSS DRAINAGE WORK: (Slab Culverts)-  Only 250 Characters Allowed in suggestions for improvement")]
        public string IMPROVE_SUGGESTIONS_24 { get; set; }
    }
}