using PMGSY.Areas.EFORMArea.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using static PMGSY.Areas.EFORMArea.Common.CommonEnum;

namespace PMGSY.Areas.EFORMArea.QMModels
{
    public class EFORM_QM_QUALITY_GRADING
    {

        public EFORM_QM_QUALITY_GRADING(bool RoadStatusIsCompleted)
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
       // [Required(ErrorMessage = "Page-34: Please enter ")]
        // [RegularExpression(@"^[A-Za-z ]{0,3}$", ErrorMessage = "Page-34: Maximum three characters are allowed ")]
        [StringLength(3, ErrorMessage = "Page-34: Item 22. QUALITY GRADING OF ITEMS AND SUB-ITEMS OF WORK- Maximum three characters are allowed in Item 2 - Quality Arrangements- a. Quality Arrangements- Awarded Grades")]
        public string I2_QUALITY_ARRANGEMENTS { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [RoadStatusDependable]
        // [Required(ErrorMessage = "Page-34: Please enter ")]
        // [RegularExpression(@"^[A-Za-z ]{0,3}$", ErrorMessage = "Page-34: Maximum three characters are allowed ")]
        [StringLength(3, ErrorMessage = "Page-34: Item 22. QUALITY GRADING OF ITEMS AND SUB-ITEMS OF WORK-  Maximum three characters are allowed in Item 2 - Quality Arrangements- Item Grading Awarded Grades")]
        public string I2_ITEM_GRADING { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [RoadStatusDependable]
        //  [Required(ErrorMessage = "Page-34: Item 22. QUALITY GRADING OF ITEMS AND SUB-ITEMS OF WORK-  Please enter ")]
        //[RegularExpression(@"^[A-Za-z ]{0,3}$", ErrorMessage = "Page-34: Item 22. QUALITY GRADING OF ITEMS AND SUB-ITEMS OF WORK-  Maximum three characters are allowed ")]
        [StringLength(3, ErrorMessage = "Page-34: Item 22. QUALITY GRADING OF ITEMS AND SUB-ITEMS OF WORK-  Maximum three characters are allowed in Item 3 - Attention to Quality- a. Maintenance of QC Registers Awarded Grades")]
        public string I3_MANE_QC_REGISTERS { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [RoadStatusDependable]
        //  [Required(ErrorMessage = "Page-34: Item 22. QUALITY GRADING OF ITEMS AND SUB-ITEMS OF WORK-  Please enter ")]
        // [RegularExpression(@"^[A-Za-z ]{0,3}$", ErrorMessage = "Page-34: Item 22. QUALITY GRADING OF ITEMS AND SUB-ITEMS OF WORK-  Maximum three characters are allowed ")]
        [StringLength(3, ErrorMessage = "Page-34: Item 22. QUALITY GRADING OF ITEMS AND SUB-ITEMS OF WORK-  Maximum three characters are allowed  in Item 3 - Attention to Quality- b. Verification of test results Awarded Grades")]
        public string I3_VERIFICATION_OF_RESULTS { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [RoadStatusDependable]
        // [Required(ErrorMessage = "Page-34: Item 22. QUALITY GRADING OF ITEMS AND SUB-ITEMS OF WORK-  Please enter ")]
        // [RegularExpression(@"^[A-Za-z ]{0,3}$", ErrorMessage = "Page-34: Item 22. QUALITY GRADING OF ITEMS AND SUB-ITEMS OF WORK-  Maximum three characters are allowed ")]
        [StringLength(3, ErrorMessage = "Page-34: Item 22. QUALITY GRADING OF ITEMS AND SUB-ITEMS OF WORK-  Maximum three characters are allowed in Item 3 - Attention to Quality- Item Grading Awarded Grades")]
        public string I3_ITEM_GRADING { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        // [Required(ErrorMessage = "Page-34: Item 22. QUALITY GRADING OF ITEMS AND SUB-ITEMS OF WORK-  Please enter ")]
        // [RegularExpression(@"^[A-Za-z ]{0,3}$", ErrorMessage = "Page-34: Item 22. QUALITY GRADING OF ITEMS AND SUB-ITEMS OF WORK-  Maximum three characters are allowed ")]
        [StringLength(3, ErrorMessage = "Page-34: Item 22. QUALITY GRADING OF ITEMS AND SUB-ITEMS OF WORK-  Maximum three characters are allowed in Item 4 – Geometrics- a. Road way width Awarded Grades")]
        public string I4_ROADWAY_WIDTH { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        // [Required(ErrorMessage = "Page-34: Item 22. QUALITY GRADING OF ITEMS AND SUB-ITEMS OF WORK-  Please enter ")]
        //[RegularExpression(@"^[A-Za-z ]{0,3}$", ErrorMessage = "Page-34: Item 22. QUALITY GRADING OF ITEMS AND SUB-ITEMS OF WORK-  Maximum three characters are allowed ")]
        [StringLength(3, ErrorMessage = "Page-34: Item 22. QUALITY GRADING OF ITEMS AND SUB-ITEMS OF WORK-  Maximum three characters are allowed in Item 4 – Geometrics- b. Carriageway width Awarded Grades")]
        public string I4_CARRIAGE_WAY_WIDTH { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        //  [Required(ErrorMessage = "Page-34: Item 22. QUALITY GRADING OF ITEMS AND SUB-ITEMS OF WORK-  Please enter ")]
        //[RegularExpression(@"^[A-Za-z ]{0,3}$", ErrorMessage = "Page-34: Item 22. QUALITY GRADING OF ITEMS AND SUB-ITEMS OF WORK-  Maximum three characters are allowed ")]
        [StringLength(3, ErrorMessage = "Page-34: Item 22. QUALITY GRADING OF ITEMS AND SUB-ITEMS OF WORK-  Maximum three characters are in Item 4 – Geometrics- allowed c. Camber Awarded Grades")]
        public string I4_CAMBER { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        // [Required(ErrorMessage = "Page-34: Item 22. QUALITY GRADING OF ITEMS AND SUB-ITEMS OF WORK-  Please enter ")]
        //[RegularExpression(@"^[A-Za-z ]{0,3}$", ErrorMessage = "Page-34: Item 22. QUALITY GRADING OF ITEMS AND SUB-ITEMS OF WORK-  Maximum three characters are allowed ")]
        [StringLength(3, ErrorMessage = "Page-34: Item 22. QUALITY GRADING OF ITEMS AND SUB-ITEMS OF WORK-  Maximum three characters are allowed in Item 4 – Geometrics-  d. Super elevation Awarded Grades")]
        public string I4_SUPER_ELEVATION { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        //  [Required(ErrorMessage = "Page-34: Item 22. QUALITY GRADING OF ITEMS AND SUB-ITEMS OF WORK-  Please enter ")]
        //[RegularExpression(@"^[A-Za-z ]{0,3}$", ErrorMessage = "Page-34: Item 22. QUALITY GRADING OF ITEMS AND SUB-ITEMS OF WORK-  Maximum three characters are allowed ")]
        [StringLength(3, ErrorMessage = "Page-34: Item 22. QUALITY GRADING OF ITEMS AND SUB-ITEMS OF WORK-  Maximum three characters are allowed in Item 4 – Geometrics- e. Extra Widening at Curves Awarded Grades")]
        public string I4_EXTRA_WIDENING { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        // [Required(ErrorMessage = "Page-34: Item 22. QUALITY GRADING OF ITEMS AND SUB-ITEMS OF WORK-  Please enter ")]
        //[RegularExpression(@"^[A-Za-z ]{0,3}$", ErrorMessage = "Page-34: Item 22. QUALITY GRADING OF ITEMS AND SUB-ITEMS OF WORK-  Maximum three characters are allowed ")]
        [StringLength(3, ErrorMessage = "Page-34: Item 22. QUALITY GRADING OF ITEMS AND SUB-ITEMS OF WORK-  Maximum three characters are allowed in Item 4 – Geometrics- f. Longitudinal Gradient in case of road in hilly/ rolling terrain Awarded Grades")]
        public string I4_LG_FOR_HILLY { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        //  [Required(ErrorMessage = "Page-34: Item 22. QUALITY GRADING OF ITEMS AND SUB-ITEMS OF WORK-  Please enter ")]
        //[RegularExpression(@"^[A-Za-z ]{0,3}$", ErrorMessage = "Page-34: Item 22. QUALITY GRADING OF ITEMS AND SUB-ITEMS OF WORK-  Maximum three characters are allowed ")]
        [StringLength(3, ErrorMessage = "Page-34: Item 22. QUALITY GRADING OF ITEMS AND SUB-ITEMS OF WORK-  Maximum three characters are allowed in Item 4 – Geometrics- Item Grading Awarded Grades")]
        public string I4_ITEM_GRADING { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        // [Required(ErrorMessage = "Page-34: Item 22. QUALITY GRADING OF ITEMS AND SUB-ITEMS OF WORK-  Please enter ")]
        //[RegularExpression(@"^[A-Za-z ]{0,3}$", ErrorMessage = "Page-34: Item 22. QUALITY GRADING OF ITEMS AND SUB-ITEMS OF WORK-  Maximum three characters are allowed ")]
        [StringLength(3, ErrorMessage = "Page-34: Item 22. QUALITY GRADING OF ITEMS AND SUB-ITEMS OF WORK-  Maximum three characters are allowed in Item 5 - Earth Work and Sub-grade in Embankment/ Cutting- a. Assessment of New Technology section Awarded Grades")]
        public string I5_ASSESS_NEW_TECH { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        // [Required(ErrorMessage = "Page-34: Item 22. QUALITY GRADING OF ITEMS AND SUB-ITEMS OF WORK-  Please enter ")]
        //[RegularExpression(@"^[A-Za-z ]{0,3}$", ErrorMessage = "Page-34: Item 22. QUALITY GRADING OF ITEMS AND SUB-ITEMS OF WORK-  Maximum three characters are allowed ")]
        [StringLength(3, ErrorMessage = "Page-34: Item 22. QUALITY GRADING OF ITEMS AND SUB-ITEMS OF WORK-  Maximum three characters are allowed in Item 5 - Earth Work and Sub-grade in Embankment/ Cutting- b. Quality of Material for Embankment/ Sub - grade Awarded Grades")]
        public string I5_QOM_EMBANKMENT { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        // [Required(ErrorMessage = "Page-34: Item 22. QUALITY GRADING OF ITEMS AND SUB-ITEMS OF WORK-  Please enter ")]
        //[RegularExpression(@"^[A-Za-z ]{0,3}$", ErrorMessage = "Page-34: Item 22. QUALITY GRADING OF ITEMS AND SUB-ITEMS OF WORK-  Maximum three characters are allowed ")]
        [StringLength(3, ErrorMessage = "Page-34: Item 22. QUALITY GRADING OF ITEMS AND SUB-ITEMS OF WORK-  Maximum three characters are allowed in Item 5 - Earth Work and Sub-grade in Embankment/ Cutting- c. Compaction Awarded Grades")]
        public string I5_COMPACTION { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        // [Required(ErrorMessage = "Page-34: Item 22. QUALITY GRADING OF ITEMS AND SUB-ITEMS OF WORK-  Please enter ")]
        //[RegularExpression(@"^[A-Za-z ]{0,3}$", ErrorMessage = "Page-34: Item 22. QUALITY GRADING OF ITEMS AND SUB-ITEMS OF WORK-  Maximum three characters are allowed ")]
        [StringLength(3, ErrorMessage = "Page-34: Item 22. QUALITY GRADING OF ITEMS AND SUB-ITEMS OF WORK-  Maximum three characters are allowed in Item 5 - Earth Work and Sub-grade in Embankment/ Cutting- d. Side Slopes Awarded Grades")]
        public string I5_SIDE_SLOPES { get; set; }

        [FieldType(PropertyType = PDFFieldType.RadioButton)]
        // [Required(ErrorMessage = "Page-34: Item 22. QUALITY GRADING OF ITEMS AND SUB-ITEMS OF WORK-  Please enter ")]
        //[RegularExpression(@"^[A-Za-z ]{0,3}$", ErrorMessage = "Page-34: Item 22. QUALITY GRADING OF ITEMS AND SUB-ITEMS OF WORK-  Maximum three characters are allowed ")]
        [StringLength(3, ErrorMessage = "Page-34: Item 22. QUALITY GRADING OF ITEMS AND SUB-ITEMS OF WORK-  Maximum three characters are allowed in Item 5 - Earth Work and Sub-grade in Embankment/ Cutting- e. Profile Awarded Grades")]
        public string I5_PROFILE { get; set; }

        [FieldType(PropertyType = PDFFieldType.RadioButton)]
        //[Required(ErrorMessage = "Page-34: Item 22. QUALITY GRADING OF ITEMS AND SUB-ITEMS OF WORK-  Please enter ")]
        //[RegularExpression(@"^[A-Za-z ]{0,3}$", ErrorMessage = "Page-34: Item 22. QUALITY GRADING OF ITEMS AND SUB-ITEMS OF WORK-  Maximum three characters are allowed ")]
        [StringLength(3, ErrorMessage = "Page-34: Item 22. QUALITY GRADING OF ITEMS AND SUB-ITEMS OF WORK-  Maximum three characters are allowed in Item 5 - Earth Work and Sub-grade in Embankment/ Cutting- f. Adequacy of Slope Protection (in case of high embankments / hilly / rolling terrain) Awarded Grades")]
        public string I5_ADEQUACY { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        //[Required(ErrorMessage = "Page-34: Item 22. QUALITY GRADING OF ITEMS AND SUB-ITEMS OF WORK-  Please enter ")]
        //[RegularExpression(@"^[A-Za-z ]{0,3}$", ErrorMessage = "Page-34: Item 22. QUALITY GRADING OF ITEMS AND SUB-ITEMS OF WORK-  Maximum three characters are allowed ")]
        [StringLength(3, ErrorMessage = "Page-34: Item 22. QUALITY GRADING OF ITEMS AND SUB-ITEMS OF WORK-  Maximum three characters are allowed in Item 5 - Earth Work and Sub-grade in Embankment/ Cutting- Item Grading Awarded Grades")]
        public string I5_ITEM_GRADING { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        // [Required(ErrorMessage = "Page-35: Please enter ")]
        //[RegularExpression(@"^[A-Za-z ]{0,3}$", ErrorMessage = "Page-35: Maximum three characters are allowed ")]
        [StringLength(3, ErrorMessage = "Page-35: Maximum three characters are allowed in Item 6 – Granular Sub-Base (GSB)- a. Assessment of New Technology section Awarded Grades")]
        public string I6_ASSESS_NEW_TECH { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        // [Required(ErrorMessage = "Page-35: Please enter ")]
        //[RegularExpression(@"^[A-Za-z ]{0,3}$", ErrorMessage = "Page-35: Maximum three characters are allowed ")]
        [StringLength(3, ErrorMessage = "Page-35: Maximum three characters are allowed in Item 6 – Granular Sub-Base (GSB)- b. Grain Size Awarded Grades")]
        public string I6_GRAIN_SIZE { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        // [Required(ErrorMessage = "Page-35: Please enter ")]
        //[RegularExpression(@"^[A-Za-z ]{0,3}$", ErrorMessage = "Page-35: Maximum three characters are allowed ")]
        [StringLength(3, ErrorMessage = "Page-35: Maximum three characters are allowed in Item 6 – Granular Sub-Base (GSB)- c. Plasticity Awarded Grades")]
        public string I6_PLASTISITY { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        // [Required(ErrorMessage = "Page-35: Please enter ")]
        //[RegularExpression(@"^[A-Za-z ]{0,3}$", ErrorMessage = "Page-35: Maximum three characters are allowed ")]
        [StringLength(3, ErrorMessage = "Page-35: Maximum three characters are allowed in Item 6 – Granular Sub-Base (GSB)- d. Compaction Awarded Grades")]
        public string I6_COMPACTION { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        // [Required(ErrorMessage = "Page-35: Please enter ")]
        //[RegularExpression(@"^[A-Za-z ]{0,3}$", ErrorMessage = "Page-35: Maximum three characters are allowed ")]
        [StringLength(3, ErrorMessage = "Page-35: Maximum three characters are allowed in Item 6 – Granular Sub-Base (GSB)- e. Total Thickness of Layer Awarded Grades")]
        public string I6_THICKNESS { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        // [Required(ErrorMessage = "Page-35: Please enter ")]
        //[RegularExpression(@"^[A-Za-z ]{0,3}$", ErrorMessage = "Page-35: Maximum three characters are allowed ")]
        [StringLength(3, ErrorMessage = "Page-35: Maximum three characters are allowed in Item 6 – Granular Sub-Base (GSB)- Item Grading Awarded Grades")]
        public string I6_ITEM_GRADING { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        // [Required(ErrorMessage = "Page-35: Please enter ")]
        //[RegularExpression(@"^[A-Za-z ]{0,3}$", ErrorMessage = "Page-35: Maximum three characters are allowed ")]
        [StringLength(3, ErrorMessage = "Page-35: Maximum three characters are allowed in Item 7 - Base Course – Water Bound Macadam (WBM-Grade-II)- a. Assessment of New Technology section Awarded Grades")]
        public string I7_ASSESS_NEW_TECH { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        // [Required(ErrorMessage = "Page-35: Please enter ")]
        //[RegularExpression(@"^[A-Za-z ]{0,3}$", ErrorMessage = "Page-35: Maximum three characters are allowed ")]
        [StringLength(3, ErrorMessage = "Page-35: Maximum three characters are allowed in Item 7 - Base Course – Water Bound Macadam (WBM-Grade-II)- b. Grain Size of Course Aggregate Awarded Grades")]
        public string I7_GRAIN_SIZE { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        // [Required(ErrorMessage = "Page-35: Please enter ")]
        //[RegularExpression(@"^[A-Za-z ]{0,3}$", ErrorMessage = "Page-35: Maximum three characters are allowed ")]
        [StringLength(3, ErrorMessage = "Page-35: Maximum three characters are allowed in Item 7 - Base Course – Water Bound Macadam (WBM-Grade-II)- c. Plasticity of Crushable Aggregate used as fillers Awarded Grades")]
        public string I7_PLASTISITY { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        //[Required(ErrorMessage = "Page-35: Please enter ")]
        //[RegularExpression(@"^[A-Za-z ]{0,3}$", ErrorMessage = "Page-35: Maximum three characters are allowed ")]
        [StringLength(3, ErrorMessage = "Page-35: Maximum three characters are allowed in Item 7 - Base Course – Water Bound Macadam (WBM-Grade-II)- d. Adequacy of Compaction through Volumetric analysis. Awarded Grades")]
        public string I7_COMPACTION { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        // [Required(ErrorMessage = "Page-35: Please enter ")]
        //[RegularExpression(@"^[A-Za-z ]{0,3}$", ErrorMessage = "Page-35: Maximum three characters are allowed ")]
        [StringLength(3, ErrorMessage = "Page-35: Maximum three characters are allowed in Item 7 - Base Course – Water Bound Macadam (WBM-Grade-II)- e. Thickness of every layer of WBM. Awarded Grades")]
        public string I7_THICKNESS { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        // [Required(ErrorMessage = "Page-35: Please enter ")]
        //[RegularExpression(@"^[A-Za-z ]{0,3}$", ErrorMessage = "Page-35: Maximum three characters are allowed ")]
        [StringLength(3, ErrorMessage = "Page-35: Maximum three characters are allowed in Item 7 - Base Course – Water Bound Macadam (WBM-Grade-II)- Item Grading Awarded Grades")]
        public string I7_ITEM_GRADING { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        // [Required(ErrorMessage = "Page-35: Please enter ")]
        //[RegularExpression(@"^[A-Za-z ]{0,3}$", ErrorMessage = "Page-35: Maximum three characters are allowed ")]
        [StringLength(3, ErrorMessage = "Page-35: Maximum three characters are allowed in Item 8 - Base Course – Water Bound Macadam (WBM-Grade-III)- a. Assessment of New Technology section Awarded Grades")]
        public string I8_ASSESS_NEW_TECH { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        // [Required(ErrorMessage = "Page-35: Please enter ")]
        //[RegularExpression(@"^[A-Za-z ]{0,3}$", ErrorMessage = "Page-35: Maximum three characters are allowed ")]
        [StringLength(3, ErrorMessage = "Page-35: Maximum three characters are allowed in Item 8 - Base Course – Water Bound Macadam (WBM-Grade-III)- b. Grain Size of Course Aggregate Awarded Grades")]
        public string I8_GRAIN_SIZE { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        // [Required(ErrorMessage = "Page-35: Please enter ")]
        //[RegularExpression(@"^[A-Za-z ]{0,3}$", ErrorMessage = "Page-35: Maximum three characters are allowed ")]
        [StringLength(3, ErrorMessage = "Page-35: Maximum three characters are allowed in Item 8 - Base Course – Water Bound Macadam (WBM-Grade-III)- c. Plasticity of Crushable Aggregate used as fillers Awarded Grades")]
        public string I8_PLASTISITY { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        // [Required(ErrorMessage = "Page-35: Please enter ")]
        //[RegularExpression(@"^[A-Za-z ]{0,3}$", ErrorMessage = "Page-35: Maximum three characters are allowed ")]
        [StringLength(3, ErrorMessage = "Page-35: Maximum three characters are allowed in Item 8 - Base Course – Water Bound Macadam (WBM-Grade-III)- d. Adequacy of Compaction through Volumetric analysis. Awarded Grades")]
        public string I8_COMPACTION { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        // [Required(ErrorMessage = "Page-35: Please enter ")]
        //[RegularExpression(@"^[A-Za-z ]{0,3}$", ErrorMessage = "Page-35: Maximum three characters are allowed ")]
        [StringLength(3, ErrorMessage = "Page-35: Maximum three characters are allowed in Item 8 - Base Course – Water Bound Macadam (WBM-Grade-III)- e. Thickness of every layer of WBM. Awarded Grades")]
        public string I8_THICKNESS { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        // [Required(ErrorMessage = "Page-35: Please enter ")]
        //[RegularExpression(@"^[A-Za-z ]{0,3}$", ErrorMessage = "Page-35: Maximum three characters are allowed ")]
        [StringLength(3, ErrorMessage = "Page-35: Maximum three characters are allowed in Item 8 - Base Course – Water Bound Macadam (WBM-Grade-III)- Item Grading Awarded Grades")]
        public string I8_ITEM_GRADING { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        // [Required(ErrorMessage = "Page-35: Please enter ")]
        //[RegularExpression(@"^[A-Za-z ]{0,3}$", ErrorMessage = "Page-35: Maximum three characters are allowed ")]
        [StringLength(3, ErrorMessage = "Page-35: Maximum three characters are allowed in Item 9 - Base Course – Wet Mix Macadam (WMM)- a. Assessment of New Technology section Awarded Grades")]
        public string I9_ASSESS_NEW_TECH { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        // [Required(ErrorMessage = "Page-35: Please enter ")]
        //[RegularExpression(@"^[A-Za-z ]{0,3}$", ErrorMessage = "Page-35: Maximum three characters are allowed ")]
        [StringLength(3, ErrorMessage = "Page-35: Maximum three characters are allowed in Item 9 - Base Course – Wet Mix Macadam (WMM)- b. Grain Size of Course Aggregate Awarded Grades")]
        public string I9_GRAIN_SIZE { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        //[Required(ErrorMessage = "Page-35: Please enter ")]
        //[RegularExpression(@"^[A-Za-z ]{0,3}$", ErrorMessage = "Page-35: Maximum three characters are allowed ")]
        [StringLength(3, ErrorMessage = "Page-35: Maximum three characters are allowed in Item 9 - Base Course – Wet Mix Macadam (WMM)- c. Plasticity of Crushable Aggregate used as fillers Awarded Grades")]
        public string I9_PLASTISITY { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        // [Required(ErrorMessage = "Page-35: Please enter ")]
        //[RegularExpression(@"^[A-Za-z ]{0,3}$", ErrorMessage = "Page-35: Maximum three characters are allowed ")]
        [StringLength(3, ErrorMessage = "Page-35: Maximum three characters are allowed in Item 9 - Base Course – Wet Mix Macadam (WMM)- d. Adequacy of Compaction through Volumetric analysis Awarded Grades")]
        public string I9_COMPACTION { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        //[Required(ErrorMessage = "Page-35: Please enter ")]
        //[RegularExpression(@"^[A-Za-z ]{0,3}$", ErrorMessage = "Page-35: Maximum three characters are allowed ")]
        [StringLength(3, ErrorMessage = "Page-35: Maximum three characters are allowed in Item 9 - Base Course – Wet Mix Macadam (WMM)- e. Thickness of every layer of WMM. Awarded Grades")]
        public string I9_THICKNESS { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        // [Required(ErrorMessage = "Page-35: Please enter ")]
        //[RegularExpression(@"^[A-Za-z ]{0,3}$", ErrorMessage = "Page-35: Maximum three characters are allowed ")]
        [StringLength(3, ErrorMessage = "Page-35: Maximum three characters are allowed in Item 9 - Base Course – Wet Mix Macadam (WMM)- Item Grading Awarded Grades")]
        public string I9_ITEM_GRADING { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        // [Required(ErrorMessage = "Page-35: Please enter ")]
        //[RegularExpression(@"^[A-Za-z ]{0,3}$", ErrorMessage = "Page-35: Maximum three characters are allowed ")]
        [StringLength(3, ErrorMessage = "Page-35: Maximum three characters are allowed in Item 10- Bituminous Base Course: Bituminous Macadam (BM) and Dense BM- a. Assessment of New Technology section Awarded Grades")]
        public string I10_ASSESS_NEW_TECH { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        //[Required(ErrorMessage = "Page-35: Please enter ")]
        //[RegularExpression(@"^[A-Za-z ]{0,3}$", ErrorMessage = "Page-35: Maximum three characters are allowed ")]
        [StringLength(3, ErrorMessage = "Page-35: Maximum three characters are allowed in Item 10- Bituminous Base Course: Bituminous Macadam (BM) and Dense BM- b. Grading of Coarse Aggregate Awarded Grades")]
        public string I10_GRADING_COURSE_AGG { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        // [Required(ErrorMessage = "Page-35: Please enter ")]
        //[RegularExpression(@"^[A-Za-z ]{0,3}$", ErrorMessage = "Page-35: Maximum three characters are allowed ")]
        [StringLength(3, ErrorMessage = "Page-35: Maximum three characters are allowed in Item 10- Bituminous Base Course: Bituminous Macadam (BM) and Dense BM- c. Bitumen Content Awarded Grades")]
        public string I10_BITUMEN_CONTENT { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        //[Required(ErrorMessage = "Page-35: Please enter ")]
        //[RegularExpression(@"^[A-Za-z ]{0,3}$", ErrorMessage = "Page-35: Maximum three characters are allowed ")]
        [StringLength(3, ErrorMessage = "Page-35: Maximum three characters are allowed in Item 10- Bituminous Base Course: Bituminous Macadam (BM) and Dense BM- d. Thickness of Layer Awarded Grades")]
        public string I10_THICKNESS_LAYER { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        // [Required(ErrorMessage = "Page-35: Please enter ")]
        //[RegularExpression(@"^[A-Za-z ]{0,3}$", ErrorMessage = "Page-35: Maximum three characters are allowed ")]
        [StringLength(3, ErrorMessage = "Page-35: Maximum three characters are allowed in Item 10- Bituminous Base Course: Bituminous Macadam (BM) and Dense BM- Item Grading Awarded Grades")]
        public string I10_ITEM_GRADING { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        // [Required(ErrorMessage = "Page-36: Please enter ")]
        //[RegularExpression(@"^[A-Za-z ]{0,3}$", ErrorMessage = "Page-36: Maximum three characters are allowed ")]
        [StringLength(3, ErrorMessage = "Page-36: Maximum three characters are allowed in Item 11 - Bituminous Surface Course: – OGPC and Seal coat/ Surface Dressing (SD) / SDBC- a. Assessment of New Technology section Awarded Grades")]
        public string I11_ASSESS_NEW_TECH { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        // [Required(ErrorMessage = "Page-36: Please enter ")]
        //[RegularExpression(@"^[A-Za-z ]{0,3}$", ErrorMessage = "Page-36: Maximum three characters are allowed ")]
        [StringLength(3, ErrorMessage = "Page-36: Maximum three characters are allowed in Item 11 - Bituminous Surface Course: – OGPC and Seal coat/ Surface Dressing (SD) / SDBC- b. Gradation of Aggregate Awarded Grades")]
        public string I11_GRADATION_AGG { get; set; }

        [FieldType(PropertyType = PDFFieldType.RadioButton)]
        // [Required(ErrorMessage = "Page-36: Please enter ")]
        //[RegularExpression(@"^[A-Za-z ]{0,3}$", ErrorMessage = "Page-36: Maximum three characters are allowed ")]
        [StringLength(3, ErrorMessage = "Page-36: Maximum three characters are allowed in Item 11 - Bituminous Surface Course: – OGPC and Seal coat/ Surface Dressing (SD) / SDBC- c. Laying Temperature of Mix Awarded Grades")]
        public string I11_LAYING_TEMP { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        // [Required(ErrorMessage = "Page-36: Please enter ")]
        //[RegularExpression(@"^[A-Za-z ]{0,3}$", ErrorMessage = "Page-36: Maximum three characters are allowed ")]
        [StringLength(3, ErrorMessage = "Page-36: Maximum three characters are allowed in Item 11 - Bituminous Surface Course: – OGPC and Seal coat/ Surface Dressing (SD) / SDBC- d. Bitumen content Awarded Grades")]
        public string I11_BITUMEN_CONTENT { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        // [Required(ErrorMessage = "Page-36: Please enter ")]
        //[RegularExpression(@"^[A-Za-z ]{0,3}$", ErrorMessage = "Page-36: Maximum three characters are allowed ")]
        [StringLength(3, ErrorMessage = "Page-36: Maximum three characters are allowed in Item 11 - Bituminous Surface Course: – OGPC and Seal coat/ Surface Dressing (SD) / SDBC- e. Thickness of layer Awarded Grades")]
        public string I11_THICKNESS_LAYER { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        //[Required(ErrorMessage = "Page-36: Please enter ")]
        //[RegularExpression(@"^[A-Za-z ]{0,3}$", ErrorMessage = "Page-36: Maximum three characters are allowed ")]
        [StringLength(3, ErrorMessage = "Page-36: Maximum three characters are allowed in Item 11 - Bituminous Surface Course: – OGPC and Seal coat/ Surface Dressing (SD) / SDBC- f. Surface Evenness Awarded Grades")]
        public string I11_SURFACE_EVENNESS { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        // [Required(ErrorMessage = "Page-36: Please enter ")]
        //[RegularExpression(@"^[A-Za-z ]{0,3}$", ErrorMessage = "Page-36: Maximum three characters are allowed ")]
        [StringLength(3, ErrorMessage = "Page-36: Maximum three characters are allowed in Item 11 - Bituminous Surface Course: – OGPC and Seal coat/ Surface Dressing (SD) / SDBC- Item Grading Awarded Grades")]
        public string I11_ITEM_GRADING { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        //[Required(ErrorMessage = "Page-36: Please enter ")]
        //[RegularExpression(@"^[A-Za-z ]{0,3}$", ErrorMessage = "Page-36: Maximum three characters are allowed ")]
        [StringLength(3, ErrorMessage = "Page-36: Maximum three characters are allowed in Item 12 – Shoulders- a. Assessment of New Technology section Awarded Grades")]
        public string I12_ASSESS_NEW_TECH { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        // [Required(ErrorMessage = "Page-36: Please enter ")]
        //[RegularExpression(@"^[A-Za-z ]{0,3}$", ErrorMessage = "Page-36: Maximum three characters are allowed ")]
        [StringLength(3, ErrorMessage = "Page-36: Maximum three characters are allowed in Item 12 – Shoulders- b. Quality of material for shoulders Awarded Grades")]
        public string I12_QOM_SHOULDERS { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        // [Required(ErrorMessage = "Page-36: Please enter ")]
        //[RegularExpression(@"^[A-Za-z ]{0,3}$", ErrorMessage = "Page-36: Maximum three characters are allowed ")]
        [StringLength(3, ErrorMessage = "Page-36: Maximum three characters are allowed in Item 12 – Shoulders- c. Degree of compaction Awarded Grades")]
        public string I12_DOC { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [Required(ErrorMessage = "Page-36: Please enter ")]
        //[RegularExpression(@"^[A-Za-z ]{0,3}$", ErrorMessage = "Page-36: Maximum three characters are allowed ")]
        [StringLength(3, ErrorMessage = "Page-36: Maximum three characters are allowed in Item 12 – Shoulders- d. Camber. Awarded Grades")]
        public string I12_CAMBER { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        // [Required(ErrorMessage = "Page-36: Please enter ")]
        //[RegularExpression(@"^[A-Za-z ]{0,3}$", ErrorMessage = "Page-36: Maximum three characters are allowed ")]
        [StringLength(3, ErrorMessage = "Page-36: Maximum three characters are allowed in Item 12 – Shoulders- Item Grading Awarded Grades")]
        public string I12_ITEM_GRADING { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        // [Required(ErrorMessage = "Page-36: Please enter ")]
        //[RegularExpression(@"^[A-Za-z ]{0,3}$", ErrorMessage = "Page-36: Maximum three characters are allowed ")]
        [StringLength(3, ErrorMessage = "Page-36: Maximum three characters are allowed in Item 13 - Cross Drainage Works (Pipe Culvert)- a. Cushion over Hume pipes including size etc. Awarded Grades")]
        public string I13_CUSHION { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        // [Required(ErrorMessage = "Page-36: Please enter ")]
        //[RegularExpression(@"^[A-Za-z ]{0,3}$", ErrorMessage = "Page-36: Maximum three characters are allowed ")]
        [StringLength(3, ErrorMessage = "Page-36: Maximum three characters are allowed in Item 13 - Cross Drainage Works (Pipe Culvert)- b. Quality of Workmanship such as positioning of pipes, wing walls, cushion over Hume Pipes etc Awarded Grades")]
        public string I13_QOW { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        // [Required(ErrorMessage = "Page-36: Please enter ")]
        //[RegularExpression(@"^[A-Za-z ]{0,3}$", ErrorMessage = "Page-36: Maximum three characters are allowed ")]
        [StringLength(3, ErrorMessage = "Page-36: Maximum three characters are allowed in Item 13 - Cross Drainage Works (Pipe Culvert)- Item Grading Awarded Grades")]
        public string I13_ITEM_GRADING { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        //   [Required(ErrorMessage = "Page-36: Please enter ")]
        //[RegularExpression(@"^[A-Za-z ]{0,3}$", ErrorMessage = "Page-36: Maximum three characters are allowed ")]
        [StringLength(3, ErrorMessage = "Page-36: Maximum three characters are allowed in Item 14 - Cross Drainage Works (Slab Culvert)- a. Thickness of Slab Awarded Grades")]
        public string I14_THICKNESS_OF_SLAB { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [Required(ErrorMessage = "Page-36: Please enter ")]
        //[RegularExpression(@"^[A-Za-z ]{0,3}$", ErrorMessage = "Page-36: Maximum three characters are allowed ")]
        [StringLength(3, ErrorMessage = "Page-36: Maximum three characters are allowed in Item 14 - Cross Drainage Works (Slab Culvert)- b. Quality of material & workmanship Awarded Grades")]
        public string I14_QOM { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [Required(ErrorMessage = "Page-36: Please enter ")]
        //[RegularExpression(@"^[A-Za-z ]{0,3}$", ErrorMessage = "Page-36: Maximum three characters are allowed ")]
        [StringLength(3, ErrorMessage = "Page-36: Maximum three characters are allowed in Item 14 - Cross Drainage Works (Slab Culvert)- Item Grading Awarded Grades")]
        public string I14_ITEM_GRADING { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        //  [Required(ErrorMessage = "Page-36: Please enter ")]
        //[RegularExpression(@"^[A-Za-z ]{0,3}$", ErrorMessage = "Page-36: Maximum three characters are allowed ")]
        [StringLength(3, ErrorMessage = "Page-36: Maximum three characters are allowed in Item 15- Protection Work (Retaining wall /Breast wall/Parapets- a. Quality of Material Awarded Grades")]
        public string I15_QOM { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        // [Required(ErrorMessage = "Page-36: Please enter ")]
        //[RegularExpression(@"^[A-Za-z ]{0,3}$", ErrorMessage = "Page-36: Maximum three characters are allowed ")]
        [StringLength(3, ErrorMessage = "Page-36: Maximum three characters are allowed in Item 15- Protection Work (Retaining wall /Breast wall/Parapets- b. Workmanship of retaining structure Awarded Grades")]
        public string I15_W_RETAINING_STRUCT { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        // [Required(ErrorMessage = "Page-36: Please enter ")]
        //[RegularExpression(@"^[A-Za-z ]{0,3}$", ErrorMessage = "Page-36: Maximum three characters are allowed ")]
        [StringLength(3, ErrorMessage = "Page-36: Maximum three characters are allowed in Item 15- Protection Work (Retaining wall /Breast wall/Parapets- Item Grading Awarded Grades")]
        public string I15_ITEM_GRADING { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [Required(ErrorMessage = "Page-36: Please enter ")]
        //[RegularExpression(@"^[A-Za-z ]{0,3}$", ErrorMessage = "Page-36: Maximum three characters are allowed ")]
        [StringLength(3, ErrorMessage = "Page-36: Maximum three characters are allowed in Item 16- Crash Barriers and Road Safety Sign Boards- a. Overall quality of safety measures in road Awarded Grades")]
        public string I16_OVERALL_QOS { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        // [Required(ErrorMessage = "Page-36: Please enter ")]
        //[RegularExpression(@"^[A-Za-z ]{0,3}$", ErrorMessage = "Page-36: Maximum three characters are allowed ")]
        [StringLength(3, ErrorMessage = "Page-36: Maximum three characters are allowed in Item 16- Crash Barriers and Road Safety Sign Boards- b. Fixing of mandatory and cautionary sign boards Awarded Grades")]
        public string I16_FIXING_SIGN_BOARDS { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [Required(ErrorMessage = "Page-36: Please enter ")]
        //[RegularExpression(@"^[A-Za-z ]{0,3}$", ErrorMessage = "Page-36: Maximum three characters are allowed ")]
        [StringLength(3, ErrorMessage = "Page-36: Maximum three characters are allowed in Item 16- Crash Barriers and Road Safety Sign Boards- Item Grading Awarded Grades")]
        public string I16_ITEM_GRADING { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        // [Required(ErrorMessage = "Page-37: Please enter ")]
        //[RegularExpression(@"^[A-Za-z ]{0,3}$", ErrorMessage = "Page-37: Maximum three characters are allowed ")]
        [StringLength(3, ErrorMessage = "Page-37: Maximum three characters are allowed in Item 17 - Side Drain and Catch Water Drain (Earthen)- a. General quality of Side Drains/ Catch Water Drains and their integration with CDs Awarded Grades")]
        public string I17_QOS_DRAINS { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        // [Required(ErrorMessage = "Page-37: Please enter ")]
        //[RegularExpression(@"^[A-Za-z ]{0,3}$", ErrorMessage = "Page-37: Maximum three characters are allowed ")]
        [StringLength(3, ErrorMessage = "Page-37: Maximum three characters are allowed in Item 17 - Side Drain and Catch Water Drain (Earthen)- Item Grading Awarded Grades")]
        public string I17_ITEM_GRADING { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        //  [Required(ErrorMessage = "Page-37: Please enter ")]
        //[RegularExpression(@"^[A-Za-z ]{0,3}$", ErrorMessage = "Page-37: Maximum three characters are allowed ")]
        [StringLength(3, ErrorMessage = "Page-37: Maximum three characters are allowed in Item 18 – Cement Concrete / Semi Rigid Pavements- a. Quality of Material – Concrete, Stone/ Concrete Block Pavement etc Awarded Grades")]
        public string I18_QOM_CONCREATE { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        //  [Required(ErrorMessage = "Page-37: Please enter ")]
        //[RegularExpression(@"^[A-Za-z ]{0,3}$", ErrorMessage = "Page-37: Maximum three characters are allowed ")]
        [StringLength(3, ErrorMessage = "Page-37: Maximum three characters are allowed in Item 18 – Cement Concrete / Semi Rigid Pavements- b. Strength of CC in Concrete Pavement/ Concrete Block Pavement Awarded Grades")]
        public string I18_STRENGTH_OF_CC { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        //  [Required(ErrorMessage = "Page-37: Please enter ")]
        //[RegularExpression(@"^[A-Za-z ]{0,3}$", ErrorMessage = "Page-37: Maximum three characters are allowed ")]
        [StringLength(3, ErrorMessage = "Page-37: Maximum three characters are allowed in Item 18 – Cement Concrete / Semi Rigid Pavements- c. Quality of Workmanship – Wearing surface texture, Adequacy of setting of concrete, Joints, Edges etc. Awarded Grades")]
        public string I18_QOW { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [Required(ErrorMessage = "Page-37: Please enter ")]
        //[RegularExpression(@"^[A-Za-z ]{0,3}$", ErrorMessage = "Page-37: Maximum three characters are allowed ")]
        [StringLength(3, ErrorMessage = "Page-37: Maximum three characters are allowed in Item 18 – Cement Concrete / Semi Rigid Pavements- d. Thickness of Layer Awarded Grades")]
        public string I18_THICKNESS_OF_LAYER { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        //  [Required(ErrorMessage = "Page-37: Please enter ")]
        //[RegularExpression(@"^[A-Za-z ]{0,3}$", ErrorMessage = "Page-37: Maximum three characters are allowed ")]
        [StringLength(3, ErrorMessage = "Page-37: Maximum three characters are allowed in Item 18 – Cement Concrete / Semi Rigid Pavements- Item Grading Awarded Grades")]
        public string I18_ITEM_GRADING { get; set; }

        [FieldType(PropertyType = PDFFieldType.RadioButton)]
        // [Required(ErrorMessage = "Page-37: Please enter ")]
        //[RegularExpression(@"^[A-Za-z ]{0,3}$", ErrorMessage = "Page-37: Maximum three characters are allowed ")]
        [StringLength(3, ErrorMessage = "Page-37: Maximum three characters are allowed in Item 19- Cement Concrete Pucca Drains- a. Thickness of concrete layer Awarded Grades")]
        public string I19_THICKNESS_CONCREATE_LAYER { get; set; }

        [FieldType(PropertyType = PDFFieldType.RadioButton)]
        // [Required(ErrorMessage = "Page-37: Please enter ")]
        //[RegularExpression(@"^[A-Za-z ]{0,3}$", ErrorMessage = "Page-37: Maximum three characters are allowed ")]
        [StringLength(3, ErrorMessage = "Page-37: Maximum three characters are allowed in Item 19- Cement Concrete Pucca Drains- b. Strength of concrete Awarded Grades")]
        public string I19_STRENGTH_OF_CONCREATE { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        //[Required(ErrorMessage = "Page-37: Please enter ")]
        //[RegularExpression(@"^[A-Za-z ]{0,3}$", ErrorMessage = "Page-37: Maximum three characters are allowed ")]
        [StringLength(3, ErrorMessage = "Page-37: Maximum three characters are allowed in Item 19- Cement Concrete Pucca Drains- c. General Quality of material and Workmanship Awarded Grades")]
        public string I19_GENERAL_QOM { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        // [Required(ErrorMessage = "Page-37: Please enter ")]
        //[RegularExpression(@"^[A-Za-z ]{0,3}$", ErrorMessage = "Page-37: Maximum three characters are allowed ")]
        [StringLength(3, ErrorMessage = "Page-37: Maximum three characters are allowed in Item 19- Cement Concrete Pucca Drains- Item Grading Awarded Grades")]
        public string I19_ITEM_GRADING { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [Required(ErrorMessage = "Page-37: Please enter ")]
        //[RegularExpression(@"^[A-Za-z ]{0,3}$", ErrorMessage = "Page-37: Maximum three characters are allowed ")]
        [StringLength(3, ErrorMessage = "Page-37: Maximum three characters are allowed in Item 20 - Road Furniture and Markings- a. Citizen Information Board, Main Informatory Board, Quality and whether fixed during construction Awarded Grades")]
        public string I20_CITIZEN_INFO_BOARD { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        //  [Required(ErrorMessage = "Page-37: Please enter ")]
        //[RegularExpression(@"^[A-Za-z ]{0,3}$", ErrorMessage = "Page-37: Maximum three characters are allowed ")]
        [StringLength(3, ErrorMessage = "Page-37: Maximum three characters are allowed in Item 20 - Road Furniture and Markings- b. Logo boards, 200 m stones and Km stones, quality and whether fixed after completion Awarded Grades")]
        public string I20_LOGO_BOARDS { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        //  [Required(ErrorMessage = "Page-37: Please enter ")]
        //[RegularExpression(@"^[A-Za-z ]{0,3}$", ErrorMessage = "Page-37: Maximum three characters are allowed ")]
        [StringLength(3, ErrorMessage = "Page-37: Maximum three characters are allowed in Item 20 - Road Furniture and Markings- c. Whether the information in boards is given in local language Awarded Grades")]
        public string I20_LOCAL_LANG { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        //[Required(ErrorMessage = "Page-37: Please enter ")]
        //[RegularExpression(@"^[A-Za-z ]{0,3}$", ErrorMessage = "Page-37: Maximum three characters are allowed ")]
        [StringLength(3, ErrorMessage = "Page-37: Maximum three characters are allowed in Item 20 - Road Furniture and Markings- Item Grading Awarded Grades")]
        public string I20_ITEM_GRADING { get; set; }

        [FieldType(PropertyType = PDFFieldType.Skip)]
        public int QM_USER_ID { get; set; }

        [FieldType(PropertyType = PDFFieldType.Skip)]
        public string IPADD { get; set; }

    }
}