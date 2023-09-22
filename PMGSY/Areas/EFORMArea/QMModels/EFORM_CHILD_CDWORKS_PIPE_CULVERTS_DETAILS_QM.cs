using PMGSY.Areas.EFORMArea.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using static PMGSY.Areas.EFORMArea.Common.CommonEnum;

namespace PMGSY.Areas.EFORMArea.QMModels
{
    public class EFORM_CHILD_CDWORKS_PIPE_CULVERTS_DETAILS_QM
    {

        [FieldType(PropertyType = PDFFieldType.Skip)]
        public int RowID { get; set; }

        [FieldType(PropertyType = PDFFieldType.Skip)]
        public string WORK_TYPE { get; set; }
        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [Required(ErrorMessage = "Page-23: Item 13. CROSS DRAINAGE WORKS: (Pipe Culverts)- III. Table- Please enter RD at which CD is located of row ")]
        [RegularExpression(@"^[0-9]{0,7}(?:\.[0-9]{1,3})?$", ErrorMessage = "Page-23: Item 13. CROSS DRAINAGE WORKS: (Pipe Culverts)- III. Table-  Maximum seven digits before decimal and maximum three digits after decimal are allowed in RD at which CD is located of row ")]

        public decimal LOCATION_RD_23 { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [Required(ErrorMessage = "Page-23: Item 13. CROSS DRAINAGE WORKS: (Pipe Culverts)- III. Table-  Please enter Class of pipe used at site of row  ")]
        [StringLength(maximumLength: 20, ErrorMessage = "Page-23: Item 13. CROSS DRAINAGE WORKS: (Pipe Culverts)- III. Table-  Only 250 Characters Allowed in Class of pipe used at site of row ")]

        public string CLASS_OF_PIPE_23 { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [Required(ErrorMessage = "Page-23: Item 13. CROSS DRAINAGE WORKS: (Pipe Culverts)- III. Table-  Please enter Measured cushion over pipes (cm) of row ")]
        [RegularExpression(@"^[0-9]{0,7}(?:\.[0-9]{1,3})?$", ErrorMessage = "Page-23: Item 13. CROSS DRAINAGE WORKS: (Pipe Culverts)- III. Table-  Maximum seven digits before decimal and maximum three digits after decimal are allowed in Measured cushion over pipes (cm) of row ")]


        public decimal MEAS_CUSHION_23 { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [Required(ErrorMessage = "Page-23: Item 13. CROSS DRAINAGE WORKS: (Pipe Culverts)- III. Table-  Please enter Strength of concrete used in head walls as per QCR of row  ")]
        [StringLength(maximumLength: 20, ErrorMessage = "Page-23: Item 13. CROSS DRAINAGE WORKS: (Pipe Culverts)- III. Table-  Only 250 Characters Allowed in Strength of concrete used in head walls as per QCR of row ")]


        public string STRENGTH_OF_CONCRETE_23 { get; set; }

        [FieldType(PropertyType = PDFFieldType.RadioButton)]
        [Required(ErrorMessage = "Page-23: Item 13. CROSS DRAINAGE WORKS: (Pipe Culverts)- III. Table-  Please select Quality of material and workmanship (S / U) of row  ")]
        [RegularExpression(@"^[A-Za-z ]{0,1}$", ErrorMessage = "Page-23: Item 13. CROSS DRAINAGE WORKS: (Pipe Culverts)- III. Table-  Maximum one character is allowed Quality of material and workmanship (S / U) of row  ")]
        public string QOM_23 { get; set; }
    }
}