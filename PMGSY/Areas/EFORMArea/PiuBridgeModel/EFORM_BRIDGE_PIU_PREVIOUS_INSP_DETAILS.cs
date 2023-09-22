using PMGSY.Areas.EFORMArea.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using static PMGSY.Areas.EFORMArea.Common.CommonEnum;

namespace PMGSY.Areas.EFORMArea.PiuBridgeModel
{
    public class EFORM_BRIDGE_PIU_PREVIOUS_INSP_DETAILS
    {

        [FieldType(PropertyType = PDFFieldType.Skip)]
        public int RowID { get; set; }

        [FieldType(PropertyType = PDFFieldType.Skip)]
        public string Inspection_TYPE { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [Required(ErrorMessage = "Page-8: Item 6. EARLIER INSPECTIONS-Insp Id is blank of ")]

        public string INSP_ID { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [RegularExpression(pattern: @"^([0]?[0-9]|[12][0-9]|[3][01])/([0]?[1-9]|[1][0-2])/([0-9]{4})$", ErrorMessage = "Page-8 Item 6. EARLIER INSPECTIONS- Please Enter Valid date{in dd/mm/yyyy format} in  ")]
        public string VISIT_DATE { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [StringLength(maximumLength: 1000, ErrorMessage = "Page-8 Item 6. EARLIER INSPECTIONS-Only 1000 Characters Allowed in Name and Designation of inspecting officer (NQM/SQM/CE/SE)(2) of ")]
        public string VISITOR_NAME_DESG { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [RegularExpression(@"^(?:\d{0,4}\.\d{1,3})$|^\d{0,4}$", ErrorMessage = "Page-8 Item 6. EARLIER INSPECTIONS-Maximum four digits before decimal and maximum three digits after decimal are allowed in Road distance(RD) From(3) of ")]

        public Nullable<decimal> ROAD_FROM { get; set; }


        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [RegularExpression(@"^(?:\d{0,4}\.\d{1,3})$|^\d{0,4}$", ErrorMessage = "Page-8 Item 6. EARLIER INSPECTIONS -Maximum four digits before decimal and maximum three digits after decimal are allowed in Road distance(RD) To(4) of ")]

        public Nullable<decimal> ROAD_TO { get; set; }

        [Required(ErrorMessage = "Page-8: Item 6. EARLIER INSPECTIONS-Please select Level of work at the time of inspection (5) of ")]
        [FieldType(PropertyType = PDFFieldType.ComboBox)]
          [StringLength(maximumLength: 250, ErrorMessage = "Page-8 Item 6. EARLIER INSPECTIONS-Only 250 Characters Allowed in Level of work at the time of inspection (5) of ")]

        public string INSP_LEVEL { get; set; }

        [Required(ErrorMessage = "Page-8: Item 6. EARLIER INSPECTIONS-Please enter Major observations (6) of  ")]
        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [StringLength(maximumLength: 250, ErrorMessage = "Page-8 Item 6. EARLIER INSPECTIONS-Only 250 Characters Allowed in Major observations (6) of ")]
        public string OBSERVATIONS { get; set; }

        [Required(ErrorMessage = "Page-8: Item 6. EARLIER INSPECTIONS-Please enter Action taken by PIU with date(7) of ")]
        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [StringLength(maximumLength: 250, ErrorMessage = "Page-8 Item 6. EARLIER INSPECTIONS-Only 250 Characters Allowed in Action taken by PIU with date(7) of ")]
        public string ACTION { get; set; }

        [FieldType(PropertyType = PDFFieldType.Skip)]

        public string IINSERT_OR_UPDATE { get; set; }
    }
}