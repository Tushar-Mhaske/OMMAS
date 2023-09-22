using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using static PMGSY.Areas.EFORMArea.Common.CommonEnum;
using System.ComponentModel.DataAnnotations;
using PMGSY.Areas.EFORMArea.Model;
namespace PMGSY.Areas.EFORMArea.QMBridgeModel
{
    public class EFORM_BRIDGE_QM_CHILD_QOM_APPROACH
    {

        [FieldType(PropertyType = PDFFieldType.Skip)]
        public int ROW_ID { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [Required(ErrorMessage = "Page-27: Item 10. APPROACHES: 10.5 Protection Work- III. Quality of Materials- Please enter Location(RD) of row  ")]
        [RegularExpression(@"^(?:\d{0,7}\.\d{1,3})$|^\d{0,7}$", ErrorMessage = "Page-27 Item 10. APPROACHES: 10.5 Protection Work- III. Quality of Materials- Maximum seven digits before decimal and maximum three digits after decimal are allowed in  Location(RD) of row ")]

        public Nullable<decimal> RD_LOC_27_2 { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [Required(ErrorMessage = "Page-27: Item 10. APPROACHES: 10.5 Protection Work- III. Quality of Materials- Please enter Structure Type (Retaining Wall / Breast Wall / Parapets) of row  ")]
        [StringLength(30, ErrorMessage = "Page-27 Item 10. APPROACHES: 10.5 Protection Work- III. Quality of Materials- The length must be 30 character or less for Structure Type (Retaining Wall / Breast Wall / Parapets) of row ")]

        public string STRUCTURE_TYPE_27_2 { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [Required(ErrorMessage = "Page-27: Item 10. APPROACHES: 10.5 Protection Work- III. Quality of Materials- Please enter Type of Protection work (RCC/CC/ Masonry/ wire crate) of row  ")]
        [StringLength(30, ErrorMessage = "Page-27 Item 10. APPROACHES: 10.5 Protection Work- III. Quality of Materials- The length must be 30 character or less for Type of Protection work (RCC/CC/ Masonry/ wire crate) of row ")]

        public string PROTECTION_TYPE_27_2 { get; set; }

        [FieldType(PropertyType = PDFFieldType.RadioButton)]
        [Required(ErrorMessage = "Page-27: Item 10. APPROACHES: 10.5 Protection Work- III. Quality of Materials- Please select General quality of material conforms to specifications (Yes / No) of row  ")]
        [StringLength(3, ErrorMessage = "Page-27 Item 10. APPROACHES: 10.5 Protection Work- III. Quality of Materials- The length must be 3 character or less for General quality of material conforms to specifications (Yes / No) of row ")]

        public string QOM_SPECIFI_27_2 { get; set; }

        [FieldType(PropertyType = PDFFieldType.RadioButton)]
        [Required(ErrorMessage = "Page-27: Item 10. APPROACHES: 10.5 Protection Work- III. Quality of Materials- Please select Size and shape as per design (Yes / No) of row  ")]
        [StringLength(3, ErrorMessage = "Page-27 Item 10. APPROACHES: 10.5 Protection Work- III. Quality of Materials- The length must be 3 character or less for Size and shape as per design (Yes / No) of row ")]

        public string DESIGN_SHAPE_27_2 { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [Required(ErrorMessage = "Page-27: Item 10. APPROACHES: 10.5 Protection Work- III. Quality of Materials- Please enter Quality assessment by personal judgement of row  ")]
        [StringLength(25, ErrorMessage = "Page-27 Item 10. APPROACHES: 10.5 Protection Work- III. Quality of Materials- The length must be 25 character or less for Quality assessment by personal judgement of row ")]

        public string QUALITY_ASSESMENT_27_2 { get; set; }
        
    }
}