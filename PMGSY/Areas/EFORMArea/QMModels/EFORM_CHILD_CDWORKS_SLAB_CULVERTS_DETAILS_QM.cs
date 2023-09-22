using PMGSY.Areas.EFORMArea.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using static PMGSY.Areas.EFORMArea.Common.CommonEnum;

namespace PMGSY.Areas.EFORMArea.QMModels
{
    public class EFORM_CHILD_CDWORKS_SLAB_CULVERTS_DETAILS_QM
    {
        [FieldType(PropertyType = PDFFieldType.Skip)]
        public int RowID { get; set; }

        [FieldType(PropertyType = PDFFieldType.Skip)]
        public string WORK_TYPE { get; set; }
        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [Required(ErrorMessage = "Page-24: Item 14. CROSS DRAINAGE WORK: (Slab Culverts)- III. Table- Please enter RD at which CD is located of row  ")]
        [RegularExpression(@"^(?:\d{0,7}\.\d{1,3})$|^\d{0,7}$", ErrorMessage = "Page-24: Item 14. CROSS DRAINAGE WORK: (Slab Culverts)- III. Table-  Maximum seven digits before decimal and maximum three digits after decimal are allowed in RD at which CD is located of row ")]

        public decimal LOCATION_RD_24 { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [Required(ErrorMessage = "Page-24: Item 14. CROSS DRAINAGE WORK: (Slab Culverts)- III. Table-  Please enter Thickness of slab As per DPR (mm) of row ")]
        [RegularExpression(@"^(?:\d{0,7}\.\d{1,3})$|^\d{0,7}$", ErrorMessage = "Page-24: Item 14. CROSS DRAINAGE WORK: (Slab Culverts)- III. Table- Maximum seven digits before decimal and maximum three digits after decimal are allowed in Thickness of slab As per DPR (mm) of row ")]

        public decimal SLAB_THICKNESS_ASPER_DPR_24 { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [Required(ErrorMessage = "Page-24: Item 14. CROSS DRAINAGE WORK: (Slab Culverts)- III. Table-  Please enter Thickness of slab As measured by QM (mm) of row ")]
        [RegularExpression(@"^(?:\d{0,7}\.\d{1,3})$|^\d{0,7}$", ErrorMessage = "Page-24: Item 14. CROSS DRAINAGE WORK: (Slab Culverts)- III. Table- Maximum seven digits before decimal and maximum three digits after decimal are allowed in Thickness of slab As measured by QM (mm) of row ")]

        public decimal SLAB_THICKNESS_MEAS_QM_24 { get; set; }

        [FieldType(PropertyType = PDFFieldType.RadioButton)]
        [Required(ErrorMessage = "Page-24: Item 14. CROSS DRAINAGE WORK: (Slab Culverts)- III. Table-  Please select Thickness of slab Grading (S / U) of row ")]
        [RegularExpression(@"^[A-Za-z ]{0,1}$", ErrorMessage = "Page-24: Item 14. CROSS DRAINAGE WORK: (Slab Culverts)- III. Table- Maximum one character is allowed in Thickness of slab Grading (S / U) of row ")]
        public string GRADING_SLAB_THICKNESS_24 { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [Required(ErrorMessage = "Page-24: Item 14. CROSS DRAINAGE WORK: (Slab Culverts)- III. Table-  Please enter Grade of concrete proposed as per DPR of row ")]
        [StringLength(maximumLength: 20, ErrorMessage = "Page-24: Item 14. CROSS DRAINAGE WORK: (Slab Culverts)- III. Table- Only 20 Characters Allowed in Grade of concrete proposed as per DPR of row ")]
        public string CONCRETE_GRADE_ASPER_DPR_24 { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [Required(ErrorMessage = "Page-24: Item 14. CROSS DRAINAGE WORK: (Slab Culverts)- III. Table-  Please enter Strength of concrete used in head walls as per QCR of row ")]
        [StringLength(maximumLength: 20, ErrorMessage = "Page-24: Item 14. CROSS DRAINAGE WORK: (Slab Culverts)- III. Table- Only 20 Characters Allowed in Strength of concrete used in head walls as per QCR of row ")]
        public string STRENGTH_OF_CONCRETE_24 { get; set; }

        [FieldType(PropertyType = PDFFieldType.RadioButton)]
        [Required(ErrorMessage = "Page-24: Item 14. CROSS DRAINAGE WORK: (Slab Culverts)- III. Table-  Please select Quality of material and Quality of workmanship is acceptable(Y / N) of row ")]
        [RegularExpression(@"^[A-Za-z ]{0,1}$", ErrorMessage = "Page-24: Item 14. CROSS DRAINAGE WORK: (Slab Culverts)- III. Table- Maximum one character is allowed in Quality of material and Quality of workmanship is acceptable(Y / N) of row ")]

        public string QOM_24 { get; set; }
    }
}