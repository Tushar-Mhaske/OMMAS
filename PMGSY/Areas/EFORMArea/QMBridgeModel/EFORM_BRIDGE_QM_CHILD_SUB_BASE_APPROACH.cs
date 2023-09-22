using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using static PMGSY.Areas.EFORMArea.Common.CommonEnum;
using System.ComponentModel.DataAnnotations;
using PMGSY.Areas.EFORMArea.Model;
namespace PMGSY.Areas.EFORMArea.QMBridgeModel
{
    public class EFORM_BRIDGE_QM_CHILD_SUB_BASE_APPROACH
    {
        

        [FieldType(PropertyType = PDFFieldType.Skip)]   
        public int ROW_ID { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [Required(ErrorMessage = "Page-26: Item 10. APPROACHES: 10.2 Sub Base (G.S.B.)  Table- Please enter Location(RD) of row  ")]
        [RegularExpression(@"^(?:\d{0,7}\.\d{1,3})$|^\d{0,7}$", ErrorMessage = "Page-26 Item 10. APPROACHES: 10.2 Sub Base (G.S.B.)  Table- Maximum seven digits before decimal and maximum three digits after decimal are allowed in  Location(RD) of row ")]

        public Nullable<decimal> RD_LOC_26_2 { get; set; }

        [FieldType(PropertyType = PDFFieldType.RadioButton)]
        [Required(ErrorMessage = "Page-26: Item 10. APPROACHES: 10.2 Sub Base (G.S.B.)  Table- Please select Conforms to Grading (Y/N) of row  ")]
        [StringLength(3, ErrorMessage = "Page-26 Item 10. APPROACHES: 10.2 Sub Base (G.S.B.)  Table- The length must be 3 character or less for Conforms to Grading (Y/N) of row ")]

        public string IS_GRADING_CONFORM_26_2 { get; set; }

        [FieldType(PropertyType = PDFFieldType.RadioButton)]
        [Required(ErrorMessage = "Page-26: Item 10. APPROACHES: 10.2 Sub Base (G.S.B.)  Table- Please select Material Suitable from plasticity angle of row  ")]
        [StringLength(3, ErrorMessage = "Page-26 Item 10. APPROACHES: 10.2 Sub Base (G.S.B.)  Table- The length must be 3 character or less for Material Suitable from plasticity angle of row ")]

        public string IS_MATERIAL_SUITABLE_26_2 { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [Required(ErrorMessage = "Page-26: Item 10. APPROACHES: 10.2 Sub Base (G.S.B.)  Table- Please enter Dry density kN / m3 of row  ")]
        [RegularExpression(@"^(?:\d{0,6}\.\d{1,3})$|^\d{0,6}$", ErrorMessage = "Page-26 Item 10. APPROACHES: 10.2 Sub Base (G.S.B.)  Table- Maximum six digits before decimal and maximum three digits after decimal are allowed in Dry density kN / m3 of row ")]
        public Nullable<decimal> IS_DRY_DENSITY_26_2 { get; set; }


        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [Required(ErrorMessage = "Page-26: Item 10. APPROACHES: 10.2 Sub Base (G.S.B.)  Table- Please enter % Compaction of row  ")]
        [RegularExpression(@"^(?:\d{0,3}\.\d{1,2})$|^\d{0,3}$", ErrorMessage = "Page-26 Item 10. APPROACHES: 10.2 Sub Base (G.S.B.)  Table- Maximum three digits before decimal and maximum two digits after decimal are allowed in  % Compaction of row ")]

        public Nullable<decimal> COMPACTION_26_2 { get; set; }

        [FieldType(PropertyType = PDFFieldType.RadioButton)]
        [Required(ErrorMessage = "Page-26: Item 10. APPROACHES: 10.2 Sub Base (G.S.B.)  Table- Please select Whether compaction is adequate (Y / N) of row  ")]
        [StringLength(3, ErrorMessage = "Page-26 Item 10. APPROACHES: 10.2 Sub Base (G.S.B.)  Table- The length must be 3 character or less for Whether compaction is adequate (Y / N) of row ")]

        public string IS_COMPACTION_INADEQUATE_26_2 { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [Required(ErrorMessage = "Page-26: Item 10. APPROACHES: 10.2 Sub Base (G.S.B.)  Table- Please enter Thickness as per DPR (in mm) of row  ")]
        [RegularExpression(@"^(?:\d{0,6}\.\d{1,3})$|^\d{0,6}$", ErrorMessage = "Page-26 Item 10. APPROACHES: 10.2 Sub Base (G.S.B.)  Table- Maximum six digits before decimal and maximum three digits after decimal are allowed in  Thickness as per DPR (in mm) of row ")]

        public Nullable<decimal> THICKNESS_PER_DPR_26_2 { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [Required(ErrorMessage = "Page-26: Item 10. APPROACHES: 10.2 Sub Base (G.S.B.)  Table- Please enter Measured Thickness (in mm) of row  ")]
        [RegularExpression(@"^(?:\d{0,6}\.\d{1,3})$|^\d{0,6}$", ErrorMessage = "Page-26 Item 10. APPROACHES: 10.2 Sub Base (G.S.B.)  Table- Maximum six digits before decimal and maximum three digits after decimal are allowed in Measured Thickness (in mm) of row ")]
        public Nullable<decimal> MEASURED_THICKNESS_26_2 { get; set; }

        [FieldType(PropertyType = PDFFieldType.RadioButton)]
        [Required(ErrorMessage = "Page-26: Item 10. APPROACHES: 10.2 Sub Base (G.S.B.)  Table- Please select Prescribed Thickness provided (Y/N) of row  ")]
        [StringLength(3, ErrorMessage = "Page-26 Item 10. APPROACHES: 10.2 Sub Base (G.S.B.)  Table- The length must be 3 character or less for Prescribed Thickness provided (Y/N) of row ")]

        public string IS_PRESCRIBED_THICKNESS_PROV_26_2 { get; set; }
       


    }
}