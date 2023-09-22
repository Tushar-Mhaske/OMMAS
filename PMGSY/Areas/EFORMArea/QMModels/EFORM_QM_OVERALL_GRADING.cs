using PMGSY.Areas.EFORMArea.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using static PMGSY.Areas.EFORMArea.Common.CommonEnum;

namespace PMGSY.Areas.EFORMArea.QMModels
{
    public class EFORM_QM_OVERALL_GRADING
    {

        public EFORM_QM_OVERALL_GRADING(bool RoadStatusIsCompleted)
        {
            this.TemplateStatus = RoadStatusIsCompleted;
        }

        [FieldType(PropertyType = PDFFieldType.Skip)]
        public bool TemplateStatus { get; set; }

        [FieldType(PropertyType = PDFFieldType.Skip)]
        public int GRADING_ID { get; set; }

        [FieldType(PropertyType = PDFFieldType.Skip)]
        public int EFORM_ID { get; set; }

        [FieldType(PropertyType = PDFFieldType.Skip)]
        public int IMS_PR_ROAD_CODE { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [RoadStatusDependable]
        // [Required(ErrorMessage = "Page-38: Please enter ")]
        //[RegularExpression(@"^[A-Za-z ]{0,3}$", ErrorMessage = "Page-38: Maximum three characters are allowed ")]
        [StringLength(3, ErrorMessage = "Page-38: Item 23. OVERALL GRADING OF WORK- Maximum three characters are allowed in 2. Quality Control Arrangements Awarded Grade")]
        public string QC_ARRANGEMENTS_38 { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [RoadStatusDependable]
        // [Required(ErrorMessage = "Page-38: Please enter ")]
        //[RegularExpression(@"^[A-Za-z ]{0,3}$", ErrorMessage = "Page-38: Maximum three characters are allowed ")]
        [StringLength(3, ErrorMessage = "Page-38: Item 23. OVERALL GRADING OF WORK-  Maximum three characters are allowed in 3. Attention to Quality Awarded Grade ")]
        public string ATTN_TO_QTY_38 { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        // [Required(ErrorMessage = "Page-38: Please enter ")]
        //[RegularExpression(@"^[A-Za-z ]{0,3}$", ErrorMessage = "Page-38: Maximum three characters are allowed ")]
        [StringLength(3, ErrorMessage = "Page-38: Item 23. OVERALL GRADING OF WORK-  Maximum three characters are allowed in 4. Geometrics Awarded Grade ")]
        public string GEOMMETRICS_38 { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        //[Required(ErrorMessage = "Page-38: Please enter ")]
        //[RegularExpression(@"^[A-Za-z ]{0,3}$", ErrorMessage = "Page-38: Maximum three characters are allowed ")]
        [StringLength(3, ErrorMessage = "Page-38: Item 23. OVERALL GRADING OF WORK-  Maximum three characters are allowed in 5. Earthwork and Sub-grade in Embankment/Cutting Awarded Grade ")]
        public string EW_SG_IN_EMBANKME_38 { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        // [Required(ErrorMessage = "Page-38: Please enter ")]
        //[RegularExpression(@"^[A-Za-z ]{0,3}$", ErrorMessage = "Page-38: Maximum three characters are allowed ")]
        [StringLength(3, ErrorMessage = "Page-38: Item 23. OVERALL GRADING OF WORK-  Maximum three characters are allowed in 6. Granular Sub-base Awarded Grade ")]
        public string GRANULAR_SUBBASE_38 { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        //[Required(ErrorMessage = "Page-38: Please enter ")]
        //[RegularExpression(@"^[A-Za-z ]{0,3}$", ErrorMessage = "Page-38: Maximum three characters are allowed ")]
        [StringLength(3, ErrorMessage = "Page-38: Item 23. OVERALL GRADING OF WORK-  Maximum three characters are allowed in 7. Base Course (WBM-II) Awarded Grade ")]
        public string BASE_COURSE_WBMII_38 { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        //  [Required(ErrorMessage = "Page-38: Please enter ")]
        //[RegularExpression(@"^[A-Za-z ]{0,3}$", ErrorMessage = "Page-38: Maximum three characters are allowed ")]
        [StringLength(3, ErrorMessage = "Page-38: Item 23. OVERALL GRADING OF WORK-  Maximum three characters are allowed in 8. Base Course (WBM-III) Awarded Grade ")]
        public string BASE_COURSE_WBMIII_38 { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        // [Required(ErrorMessage = "Page-38: Please enter ")]
        //[RegularExpression(@"^[A-Za-z ]{0,3}$", ErrorMessage = "Page-38: Maximum three characters are allowed ")]
        [StringLength(3, ErrorMessage = "Page-38: Item 23. OVERALL GRADING OF WORK-  Maximum three characters are allowed in 9. Base Course (WMM) Awarded Grade ")]
        public string BASE_COURSE_WMM_38 { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [Required(ErrorMessage = "Page-38: Please enter ")]
        //[RegularExpression(@"^[A-Za-z ]{0,3}$", ErrorMessage = "Page-38: Maximum three characters are allowed ")]
        [StringLength(3, ErrorMessage = "Page-38: Item 23. OVERALL GRADING OF WORK-  Maximum three characters are allowed in 10. Bituminous Base Course (BM and DBM) Awarded Grade ")]
        public string BITUMINOUS_BASE_COURSE_38 { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        //  [Required(ErrorMessage = "Page-38: Please enter ")]
        //[RegularExpression(@"^[A-Za-z ]{0,3}$", ErrorMessage = "Page-38: Maximum three characters are allowed ")]
        [StringLength(3, ErrorMessage = "Page-38: Item 23. OVERALL GRADING OF WORK-  Maximum three characters are allowed in Awarded 11.. Bituminous Surface Course (OGPC and Seal coat/ SD/SDBC)  Grade ")]
        public string BITUMINOUS_SURF_COURSE_38 { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        // [Required(ErrorMessage = "Page-38: Please enter ")]
        //[RegularExpression(@"^[A-Za-z ]{0,3}$", ErrorMessage = "Page-38: Maximum three characters are allowed ")]
        [StringLength(3, ErrorMessage = "Page-38: Item 23. OVERALL GRADING OF WORK-  Maximum three characters are allowed in Awarded 12. Shoulders Grade ")]
        public string SLOULDERS_38 { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        // [Required(ErrorMessage = "Page-38: Please enter ")]
        //[RegularExpression(@"^[A-Za-z ]{0,3}$", ErrorMessage = "Page-38: Maximum three characters are allowed ")]
        [StringLength(3, ErrorMessage = "Page-38: Item 23. OVERALL GRADING OF WORK-  Maximum three characters are allowed in Awarded 13. Cross Drainage Work (Pipe Culvert)  Grade ")]
        public string CD_WORK_PIPE_38 { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        //  [Required(ErrorMessage = "Page-38: Please enter ")]
        //[RegularExpression(@"^[A-Za-z ]{0,3}$", ErrorMessage = "Page-38: Maximum three characters are allowed ")]
        [StringLength(3, ErrorMessage = "Page-38: Item 23. OVERALL GRADING OF WORK-  Maximum three characters are allowed in 14. Cross Drainage Work (Slab Culvert) Awarded Grade ")]
        public string CD_WORK_SLAB_38 { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        // [Required(ErrorMessage = "Page-38: Please enter ")]
        //[RegularExpression(@"^[A-Za-z ]{0,3}$", ErrorMessage = "Page-38: Maximum three characters are allowed ")]
        [StringLength(3, ErrorMessage = "Page-38: Item 23. OVERALL GRADING OF WORK-  Maximum three characters are allowed in 15. Protection Work (Retaining wall /Breast wall/Parapets Awarded Grade ")]
        public string PROTECTION_WORK_38 { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        // [Required(ErrorMessage = "Page-38: Please enter ")]
        //[RegularExpression(@"^[A-Za-z ]{0,3}$", ErrorMessage = "Page-38: Maximum three characters are allowed ")]
        [StringLength(3, ErrorMessage = "Page-38: Item 23. OVERALL GRADING OF WORK-  Maximum three characters are allowed in 16. Crash Barriers and Road Safety Sign Boards Awarded Grade ")]
        public string CRASH_BARRIERS_38 { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        // [Required(ErrorMessage = "Page-38: Please enter ")]
        //[RegularExpression(@"^[A-Za-z ]{0,3}$", ErrorMessage = "Page-38: Maximum three characters are allowed ")]
        [StringLength(3, ErrorMessage = "Page-38: Item 23. OVERALL GRADING OF WORK-  Maximum three characters are allowed in 17. Side Drains and Catch Water Drains Awarded Grade ")]
        public string SIDE_DRAINS_38 { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        //  [Required(ErrorMessage = "Page-38: Please enter ")]
        //[RegularExpression(@"^[A-Za-z ]{0,3}$", ErrorMessage = "Page-38: Maximum three characters are allowed ")]
        [StringLength(3, ErrorMessage = "Page-38: Item 23. OVERALL GRADING OF WORK-  Maximum three characters are allowed in 18. Cement Concrete / Semi Rigid Pavements Awarded Grade ")]
        public string CEMENT_CONCRETE_SRP_38 { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        // [Required(ErrorMessage = "Page-38: Please enter ")]
        //[RegularExpression(@"^[A-Za-z ]{0,3}$", ErrorMessage = "Page-38: Maximum three characters are allowed ")]
        [StringLength(3, ErrorMessage = "Page-38: Item 23. OVERALL GRADING OF WORK-  Maximum three characters are allowed in 19. Cement Concrete Pucca Drains Awarded Grade ")]
        public string CEMENT_CONCRETE_PUCCA_DRAINS_38 { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        //   [Required(ErrorMessage = "Page-38: Please enter ")]
        //[RegularExpression(@"^[A-Za-z ]{0,3}$", ErrorMessage = "Page-38: Maximum three characters are allowed ")]
        [StringLength(3, ErrorMessage = "Page-38: Item 23. OVERALL GRADING OF WORK-  Maximum three characters are allowed in 20. Road Furniture and Markings Awarded Grade ")]
        public string ROAD_FURNITURE_38 { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        //  [Required(ErrorMessage = "Page-38: Please enter ")]
        //[RegularExpression(@"^[A-Za-z ]{0,3}$", ErrorMessage = "Page-38: Maximum three characters are allowed ")]
        [StringLength(3, ErrorMessage = "Page-38: Item 23. OVERALL GRADING OF WORK-  Maximum three characters are allowed in Overall Grading Awarded Grade ")]
        public string OVERALL_GRADING_38 { get; set; }

        [FieldType(PropertyType = PDFFieldType.RadioButton)]
        //  [Required(ErrorMessage = "Page-38: Please select ")]
        //[RegularExpression(@"^[A-Za-z ]{0,1}$", ErrorMessage = "Page-38: Maximum one characters is allowed ")]
        [StringLength(3, ErrorMessage = "Page-38: Item 23. OVERALL GRADING OF WORK-  Maximum three characters are allowed Whether the work can be considered as excellent based on the test results and visual observations made by the quality monitor")]
        public string IS_WORK_EXCELLENT_38 { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [Required(ErrorMessage = "Page-38: Item 23. OVERALL GRADING OF WORK- Please enter Name Of QM ")]
        //[RegularExpression(@"^[A-Za-z\s]{0,50}$", ErrorMessage = "Page-38: Maximum fifty characters are allowed ")]
        [StringLength(100, ErrorMessage = "Page-38: Item 23. OVERALL GRADING OF WORK-  Maximum 100 characters are allowed in Name Of QM ")]
        public string NAME_OF_QM_38 { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
          [Required(ErrorMessage = "Page-38: Item 23. OVERALL GRADING OF WORK-  Please enter Date of uploading the report on OMMAS")]
        [RegularExpression(pattern: @"^([0]?[0-9]|[12][0-9]|[3][01])/([0]?[1-9]|[1][0-2])/([0-9]{4})$", ErrorMessage = "Page-38: Item 23. OVERALL GRADING OF WORK-  Please Enter Valid date{in dd/mm/yyyy format} in Date of uploading the report on OMMAS ")]

        public string UPLOADING_DATE_38 { get; set; }

        [FieldType(PropertyType = PDFFieldType.Skip)]
        public int QM_USER_ID { get; set; }

        [FieldType(PropertyType = PDFFieldType.Skip)]
        public string IPADD { get; set; }

    }
}