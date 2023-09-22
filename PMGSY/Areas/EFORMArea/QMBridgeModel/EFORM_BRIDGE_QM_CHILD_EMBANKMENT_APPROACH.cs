using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using static PMGSY.Areas.EFORMArea.Common.CommonEnum;
using System.ComponentModel.DataAnnotations;
using PMGSY.Areas.EFORMArea.Model;
namespace PMGSY.Areas.EFORMArea.QMBridgeModel
{
    public class EFORM_BRIDGE_QM_CHILD_EMBANKMENT_APPROACH
    {
        [FieldType(PropertyType = PDFFieldType.Skip)]
        public int ROW_ID { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [Required(ErrorMessage = "Page-26: Item 10. APPROACHES: 10.1 Embankment  Table- Please enter Location of row  ")]
        [RegularExpression(@"^(?:\d{0,7}\.\d{1,3})$|^\d{0,7}$", ErrorMessage = "Page-26 Item 10. APPROACHES: 10.1 Embankment  Table- Maximum seven digits before decimal and maximum three digits after decimal are allowed in  Location of row ")]

        public Nullable<decimal> RD_LOC_26_1 { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [Required(ErrorMessage = "Page-26: Item 10. APPROACHES: 10.1 Embankment  Table- Please enter suitability of soil of row ")]
        [StringLength(20, ErrorMessage = "Page-26 Item 10. APPROACHES: 10.1 Embankment  Table- The length must be 20 character or less for suitability of soil of row ")]
       
        public string SOIL_SUITABILITY_26_1 { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [Required(ErrorMessage = "Page-26: Item 10. APPROACHES: 10.1 Embankment  Table- Please enter Compaction of row  ")]
        [StringLength(20, ErrorMessage = "Page-26 Item 10. APPROACHES: 10.1 Embankment  Table- The length must be 20 character or less for Compaction of row ")]
       
        public string COMPACTION_26_1 { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [Required(ErrorMessage = "Page-26: Item 10. APPROACHES: 10.1 Embankment  Table- Please enter Side slopes of row  ")]
        [StringLength(20, ErrorMessage = "Page-26 Item 10. APPROACHES: 10.1 Embankment  Table- The length must be 20 character or less for Side slopes of row ")]
       
        public string SIDE_SLOPES_26_1 { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [Required(ErrorMessage = "Page-26: Item 10. APPROACHES: 10.1 Embankment  Table- Please enter Shoulders of row  ")]
        [StringLength(20, ErrorMessage = "Page-26 Item 10. APPROACHES: 10.1 Embankment  Table- The length must be 20 character or less for Shoulders of row ")]
       
        public string SHOULDERS_26_1 { get; set; }

        [FieldType(PropertyType = PDFFieldType.RadioButton)]
        [Required(ErrorMessage = "Page-26: Item 10. APPROACHES: 10.1 Embankment  Table- Please select Grade(S/ SRI/ U) of row  ")]
        [StringLength(3, ErrorMessage = "Page-26 Item 10. APPROACHES: 10.1 Embankment  Table- The length must be 3 character or less for Grade(S/ SRI/ U) of row ")]
 

        public string GRADE_26_1 { get; set; }
        

    }
}