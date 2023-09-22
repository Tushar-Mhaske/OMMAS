using PMGSY.Areas.EFORMArea.Model;
using System.ComponentModel.DataAnnotations;
using static PMGSY.Areas.EFORMArea.Common.CommonEnum;

namespace PMGSY.Areas.EFORMArea.QMModels
{
    public class EFORM_QM_QOM_EMBANKMENT
    {
        [FieldType(PropertyType = PDFFieldType.Skip)]
        public int QOM_ID { get; set; }

        [FieldType(PropertyType = PDFFieldType.Skip)]
        public int EFORM_ID { get; set; }

        [FieldType(PropertyType = PDFFieldType.Skip)]
        public int IMS_PR_ROAD_CODE { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [Required(ErrorMessage = "Page-11: Item 5.EARTHWORK & SUB GRADE- II. a) Please enter Name and location of source ")]
        [StringLength(100, ErrorMessage = "Page-11: Item 5.EARTHWORK & SUB GRADE- II. a) Maximum 100 characters are allowed in Name and location of source ")]
        public string NAME_LOCATION_SRC { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [Required(ErrorMessage = "Page-11: Item 5.EARTHWORK & SUB GRADE- II. b) Please enter Distance of source of earth (lead) ")]
        [RegularExpression(@"^[0-9]{0,7}(?:\.[0-9]{1,3})?$", ErrorMessage = "Page-11: Item 5.EARTHWORK & SUB GRADE- II. b) Maximum seven digits before decimal and maximum three digits after decimal are allowed Distance of source of earth (lead)")]
        public decimal DIST_SOE { get; set; }

        [FieldType(PropertyType = PDFFieldType.RadioButton)]
        [Required(ErrorMessage = "Page-11: Item 5.EARTHWORK & SUB GRADE- II. c) Please select Whether long lead distance for the transportation of material is justified or not ")]
        [StringLength(1, ErrorMessage = "Page-11: Item 5.EARTHWORK & SUB GRADE- II. c) Maximum 1 charater is allowed in Whether long lead distance for the transportation of material is justified or not")]
        public string IS_JUSTIFIED { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [Required(ErrorMessage = "Page-12: Item 5.EARTHWORK & SUB GRADE- II. d) Please enter comment on the quality of local earth ")]
        [StringLength(250, ErrorMessage = "Page-12: Item 5.EARTHWORK & SUB GRADE- II. d) Maximum 250 characters are allowed in comment on the quality of local earth")]
        public string APPROVED_SRC_REMARKS { get; set; }

        [FieldType(PropertyType = PDFFieldType.RadioButton)]
        [Required(ErrorMessage = "Page-12: Item 5.EARTHWORK & SUB GRADE- II. Please select Sub-Item Grading 5-II ")]
        [StringLength(3, ErrorMessage = "Page-12: Item 5.EARTHWORK & SUB GRADE- II. Maximum 3 characters are allowed Sub-Item Grading 5-II")]
        public string SUBITEM_GRADING_5_II { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [Required(ErrorMessage = "Page-12: Item 5.EARTHWORK & SUB GRADE- II.  Please enter suggestions for improvement ")]
        [StringLength(250, ErrorMessage = "Page-12: Item 5.EARTHWORK & SUB GRADE- II. d) Maximum 250 characters are allowed in suggestions for improvement")]
        public string IMPROVEMENT_REMARK_12_1 { get; set; }

        [FieldType(PropertyType = PDFFieldType.Skip)]
        public int QM_USER_ID { get; set; }

        [FieldType(PropertyType = PDFFieldType.Skip)]
        public string IPADD { get; set; }
    }
}