using PMGSY.Areas.EFORMArea.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using static PMGSY.Areas.EFORMArea.Common.CommonEnum;

namespace PMGSY.Areas.EFORMArea.QMModels
{
    public class EFORM_QM_QUALITY_ATTENTION
    {

        public EFORM_QM_QUALITY_ATTENTION(bool RoadStatusIsCompleted)
        {
            this.TemplateStatus = RoadStatusIsCompleted;
        }


        [FieldType(PropertyType = PDFFieldType.Skip)]
        public bool TemplateStatus { get; set; }


        [FieldType(PropertyType = PDFFieldType.Skip)]
        public int INFO_ID { get; set; }

        [FieldType(PropertyType = PDFFieldType.Skip)]
        public int EFORM_ID { get; set; }

        [FieldType(PropertyType = PDFFieldType.Skip)]
        public int QM_USER_ID { get; set; }

        [FieldType(PropertyType = PDFFieldType.Skip)]
        public int PR_ROAD_CODE { get; set; }



        [FieldType(PropertyType = PDFFieldType.RadioButton)]
        [Required(ErrorMessage = "Page-7: Item 3. ATTENTION TO QUALITY- I. (a). Please select Based on executed quantities, whether all mandatory tests conducted ")]
        [RegularExpression(pattern: @"^[A-Za-z]{0,1}$", ErrorMessage = "Page-7: Item 3. ATTENTION TO QUALITY- I. (a) Maximum one character is allowed in Based on executed quantities, whether all mandatory tests conducted ")]
        [RoadStatusDependable]
        public string IS_ALL_TEST_CONDUCTED { get; set; }

        [FieldType(PropertyType = PDFFieldType.RadioButton)]
        [Required(ErrorMessage = "Page-7: Item 3. ATTENTION TO QUALITY- I. (b) Please select Whether QC Register Part I maintained as per provisions ")]
        [RegularExpression(pattern: @"^[A-Za-z]{0,1}$", ErrorMessage = "Page-7: Item 3. ATTENTION TO QUALITY- I. (b) Maximum one character is allowed in Whether QC Register Part I maintained as per provisions ")]
        [RoadStatusDependable]
        public string IS_QC_REG_P1_MAINTAINED { get; set; }

        [FieldType(PropertyType = PDFFieldType.RadioButton)]
        [Required(ErrorMessage = "Page-7: Item 3. ATTENTION TO QUALITY- I. (c) Please select Whether QC Register Part II maintained and test results monitored as per provisions ")]
        [RegularExpression(pattern: @"^[A-Za-z]{0,1}$", ErrorMessage = "Page-7: Item 3. ATTENTION TO QUALITY- I. (c) Maximum one character is allowed in Whether QC Register Part II maintained and test results monitored as per provisions ")]
        [RoadStatusDependable]
        public string IS_QC_REG_P2_MAINTAINED { get; set; }


        [FieldType(PropertyType = PDFFieldType.CheckBox)]
        [RoadStatusDependable]
        public string IS_NEGLIGENCE { get; set; }

        [FieldType(PropertyType = PDFFieldType.CheckBox)]
        [RoadStatusDependable]
        public string IS_LOE { get; set; }

        [FieldType(PropertyType = PDFFieldType.CheckBox)]
        [RoadStatusDependable]
        public string IS_LOK { get; set; }

        [FieldType(PropertyType = PDFFieldType.CheckBox)]
        [RoadStatusDependable]
        public string IS_OTHER { get; set; }


        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [StringLength(250, ErrorMessage = "Page-9: Item 3. ATTENTION TO QUALITY- II. (b) The length must be 250 character or less for any other reason field")]
        [RoadStatusDependable]
        public string OTHER_REASON { get; set; }


        [FieldType(PropertyType = PDFFieldType.RadioButton)]
        [Required(ErrorMessage = "Page-9: Item 3. ATTENTION TO QUALITY- II. (d) Please select Whether non-conformities recorded in QCR-II by AE, have been rectified and recorded in QCR-I again as conformities, after conducting necessary tests ")]
        [RegularExpression(pattern: @"^[A-Za-z]{0,1}$", ErrorMessage = "Page-9: Item 3. ATTENTION TO QUALITY- II. (d) Maximum one character is allowed in Whether non-conformities recorded in QCR-II by AE, have been rectified and recorded in QCR-I again as conformities, after conducting necessary tests: ")]
        [RoadStatusDependable]
        public string IS_NON_CONFORMITIES_QCR2 { get; set; }


        [FieldType(PropertyType = PDFFieldType.RadioButton)]
        [Required(ErrorMessage = "Page-9: Item 3. ATTENTION TO QUALITY- Please select Item Grading-3 ")]
        [RegularExpression(pattern: @"^[A-Za-z]{0,3}$", ErrorMessage = "Page-9: Item 3. ATTENTION TO QUALITY- Maximum three character is allowed in Item Grading-3 ")]
        [RoadStatusDependable]
        public string ITEM_GRADING_3 { get; set; }



        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [Required(ErrorMessage = "Page-9: Item 3. ATTENTION TO QUALITY- Please enter suggestions for improvement ")]
        [StringLength(250, ErrorMessage = "Page-9: Item 3. ATTENTION TO QUALITY- The length must be 250 character or less for  suggestions for improvement")]
        [RoadStatusDependable]
        public string IMPROVEMENT_REMARK_9 { get; set; }


        [FieldType(PropertyType = PDFFieldType.Skip)]
        public string IPADD { get; set; }

    }
}