using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using static PMGSY.Areas.EFORMArea.Common.CommonEnum;
using System.ComponentModel.DataAnnotations;
using PMGSY.Areas.EFORMArea.Model;
namespace PMGSY.Areas.EFORMArea.QMBridgeModel
{
    public class EFORM_BRIDGE_QM_CHILD_BASE_COURSE_APPROACH
    {

        [FieldType(PropertyType = PDFFieldType.Skip)]
        public int ROW_ID { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [Required(ErrorMessage = "Page-26: Item 10. APPROACHES: 10.3 Base Course (Non Bituminous (WMM) and Bituminous Macadam (BM))  Table- Please enter Location(RD) of row  ")]
        [RegularExpression(@"^(?:\d{0,7}\.\d{1,3})$|^\d{0,7}$", ErrorMessage = "Page-26 Item 10. APPROACHES: 10.3 Base Course (Non Bituminous (WMM) and Bituminous Macadam (BM))  Table- Maximum seven digits before decimal and maximum three digits after decimal are allowed in  Location(RD) of row ")]
        public Nullable<decimal> RD_LOC_26_3 { get; set; }

        [FieldType(PropertyType = PDFFieldType.RadioButton)]
        [Required(ErrorMessage = "Page-26: Item 10. APPROACHES: 10.3 Base Course (Non Bituminous (WMM) and Bituminous Macadam (BM))  Table- Please select Grading of Aggregates (S / U) of row  ")]
        [StringLength(3, ErrorMessage = "Page-26 Item 10. APPROACHES: 10.3 Base Course (Non Bituminous (WMM) and Bituminous Macadam (BM))  Table- The length must be 3 character or less for Grading of Aggregates (S / U) of row ")]

        public string GRADING_AGGREGATE_26_3 { get; set; }

        [FieldType(PropertyType = PDFFieldType.RadioButton)]
        [Required(ErrorMessage = "Page-26: Item 10. APPROACHES: 10.3 Base Course (Non Bituminous (WMM) and Bituminous Macadam (BM))  Table- Please select Plasticity of Filler material (S / U) of row  ")]
        [StringLength(3, ErrorMessage = "Page-26 Item 10. APPROACHES: 10.3 Base Course (Non Bituminous (WMM) and Bituminous Macadam (BM))  Table- The length must be 3 character or less for Plasticity of Filler material (S / U) of row ")]

        public string PLASTICITY_FILLER_26_3 { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [Required(ErrorMessage = "Page-26: Item 10. APPROACHES: 10.3 Base Course (Non Bituminous (WMM) and Bituminous Macadam (BM))  Table- Please enter Volume of filler material percent of coarse Aggregate of row  ")]
        [RegularExpression(@"^(?:\d{0,3}\.\d{1,2})$|^\d{0,3}$", ErrorMessage = "Page-26 Item 10. APPROACHES: 10.3 Base Course (Non Bituminous (WMM) and Bituminous Macadam (BM))  Table- Maximum three digits before decimal and maximum two digits after decimal are allowed in Volume of filler material percent of coarse Aggregate of row ")]

        public Nullable<decimal> MATERIAL_VOLUME_AGGR_26_3 { get; set; }

        [FieldType(PropertyType = PDFFieldType.RadioButton)]
        [Required(ErrorMessage = "Page-26: Item 10. APPROACHES: 10.3 Base Course (Non Bituminous (WMM) and Bituminous Macadam (BM))  Table- Please select Compaction based on volumetric analysis/ sand replacement method (S / U) of row  ")]
        [StringLength(3, ErrorMessage = "Page-26 Item 10. APPROACHES: 10.3 Base Course (Non Bituminous (WMM) and Bituminous Macadam (BM))  Table- The length must be 3 character or less for Compaction based on volumetric analysis/ sand replacement method (S / U) of row ")]

        public string COMP_BASED_VOLUMETRIC_26_3 { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [Required(ErrorMessage = "Page-26: Item 10. APPROACHES: 10.3 Base Course (Non Bituminous (WMM) and Bituminous Macadam (BM))  Table- Please enter Design thickness as per DPR (mm) of row  ")]
        [RegularExpression(@"^(?:\d{0,6}\.\d{1,3})$|^\d{0,6}$", ErrorMessage = "Page-26 Item 10. APPROACHES: 10.3 Base Course (Non Bituminous (WMM) and Bituminous Macadam (BM))  Table- Maximum six digits before decimal and maximum three digits after decimal are allowed in Design thickness as per DPR (mm) of row ")]

        public Nullable<decimal> DESIGN_THICKNESS_26_3 { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [Required(ErrorMessage = "Page-26: Item 10. APPROACHES: 10.3 Base Course (Non Bituminous (WMM) and Bituminous Macadam (BM))  Table- Please enter Thickness of each layer of WBM/ WMM (mm) of row  ")]
        [RegularExpression(@"^(?:\d{0,6}\.\d{1,3})$|^\d{0,6}$", ErrorMessage = "Page-26 Item 10. APPROACHES: 10.3 Base Course (Non Bituminous (WMM) and Bituminous Macadam (BM))  Table- Maximum six digits before decimal and maximum three digits after decimal are allowed in Thickness of each layer of WBM/ WMM (mm)  of row ")]

        public Nullable<decimal> WBM_THICKNESS_26_3 { get; set; }

        [FieldType(PropertyType = PDFFieldType.RadioButton)]
        [Required(ErrorMessage = "Page-26: Item 10. APPROACHES: 10.3 Base Course (Non Bituminous (WMM) and Bituminous Macadam (BM))  Table- Please select Thickness adequate (S / U) of row  ")]
        [StringLength(3, ErrorMessage = "Page-26 Item 10. APPROACHES: 10.3 Base Course (Non Bituminous (WMM) and Bituminous Macadam (BM))  Table- The length must be 3 character or less for  Thickness adequate (S / U) of row ")]

        public string IS_THICKNESS_ADEQUATE_26_3 { get; set; }
         

    }
}