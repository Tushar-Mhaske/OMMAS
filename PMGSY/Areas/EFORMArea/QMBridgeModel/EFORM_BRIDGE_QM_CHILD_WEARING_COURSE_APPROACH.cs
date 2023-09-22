using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using static PMGSY.Areas.EFORMArea.Common.CommonEnum;
using System.ComponentModel.DataAnnotations;
using PMGSY.Areas.EFORMArea.Model;
namespace PMGSY.Areas.EFORMArea.QMBridgeModel
{
    public class EFORM_BRIDGE_QM_CHILD_WEARING_COURSE_APPROACH
    {

        [FieldType(PropertyType = PDFFieldType.Skip)]
        public int ROW_ID { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [Required(ErrorMessage = "Page-27: Item 10. APPROACHES: 10.4 Wearing Course  Table- Please enter Location(RD) of row  ")]
        [RegularExpression(@"^(?:\d{0,7}\.\d{1,3})$|^\d{0,7}$", ErrorMessage = "Page-27 Item 10. APPROACHES: 10.4 Wearing Course  Table- Maximum seven digits before decimal and maximum three digits after decimal are allowed in  Location(RD) of row ")]

        public Nullable<decimal> RD_LOC_27_1 { get; set; }

        [FieldType(PropertyType = PDFFieldType.RadioButton)]
        [Required(ErrorMessage = "Page-27: Item 10. APPROACHES: 10.4 Wearing Course  Table- Please select Grading of Coarse Aggregate (S / U) of row  ")]
        [StringLength(3, ErrorMessage = "Page-27 Item 10. APPROACHES: 10.4 Wearing Course  Table- The length must be 3 character or less for Grading of Coarse Aggregate (S / U) of row ")]

        public string GRAD_COARSE_AGGR_27_1 { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [Required(ErrorMessage = "Page-27: Item 10. APPROACHES: 10.4 Wearing Course  Table- Please enter Laying Temperature of the mix as per QCR-I of row  ")]
        [StringLength(20, ErrorMessage = "Page-27 Item 10. APPROACHES: 10.4 Wearing Course  Table- The length must be 20 character or less for Laying Temperature of the mix as per QCR-I of row ")]

        public string TEMP_PER_QCR_27_1 { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [Required(ErrorMessage = "Page-27: Item 10. APPROACHES: 10.4 Wearing Course  Table- Please enter Bitumen content %, as per QCR-I of row  ")]
        [RegularExpression(@"^(?:\d{0,3}\.\d{1,2})$|^\d{0,5}$", ErrorMessage = "Page-27 Item 10. APPROACHES: 10.4 Wearing Course  Table- Maximum three digits before decimal and maximum two digits after decimal are allowed in Bitumen content %, as per QCR-I of row ")]

        public Nullable<decimal> BITUMEN_AGGR_PER_QCR_27_1 { get; set; }

        [FieldType(PropertyType = PDFFieldType.RadioButton)]
        [Required(ErrorMessage = "Page-27: Item 10. APPROACHES: 10.4 Wearing Course  Table- Please select Bitumen content % (S / U) of row  ")]
        [StringLength(3, ErrorMessage = "Page-27 Item 10. APPROACHES: 10.4 Wearing Course  Table- The length must be 3 character or less for Bitumen content % (S / U) of row ")]

        public string BITUMEN_CONTENT_27_1 { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [Required(ErrorMessage = "Page-27: Item 10. APPROACHES: 10.4 Wearing Course  Table- Please enter Observed thickness of layer-As measured by NQM/SQM of row  ")]
        [StringLength(20, ErrorMessage = "Page-27 Item 10. APPROACHES: 10.4 Wearing Course  Table- The length must be 20 character or less for  Observed thickness of layer-As measured by NQM/SQM of row ")]

        public string THICKNESS_BY_QM_27_1 { get; set; }

        [FieldType(PropertyType = PDFFieldType.RadioButton)]
        [Required(ErrorMessage = "Page-27: Item 10. APPROACHES: 10.4 Wearing Course  Table- Please select Observed thickness of layer-S/U of row  ")]
        [StringLength(3, ErrorMessage = "Page-27 Item 10. APPROACHES: 10.4 Wearing Course  Table- The length must be 3 character or less for Observed thickness of layer-S/U of row ")]

        public string THICKNESS_LAYER_GRADE_27_1 { get; set; }

        [FieldType(PropertyType = PDFFieldType.RadioButton)]
        [Required(ErrorMessage = "Page-27: Item 10. APPROACHES: 10.4 Wearing Course  Table- Please select Surface un-evenness (S / U) of row  ")]
        [StringLength(3, ErrorMessage = "Page-27 Item 10. APPROACHES: 10.4 Wearing Course  Table- The length must be 3 character or less for Surface un-evenness (S / U) of row ")]

        public string SURFACE_UNEVENNESS_27_1 { get; set; }
    


    }
}