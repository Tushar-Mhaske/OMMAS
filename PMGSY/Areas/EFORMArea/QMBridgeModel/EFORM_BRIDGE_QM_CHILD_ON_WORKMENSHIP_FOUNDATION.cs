using PMGSY.Areas.EFORMArea.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using static PMGSY.Areas.EFORMArea.Common.CommonEnum;

namespace PMGSY.Areas.EFORMArea.QMBridgeModel
{
    public class EFORM_BRIDGE_QM_CHILD_ON_WORKMENSHIP_FOUNDATION
    {
        [FieldType(PropertyType = PDFFieldType.Skip)]
        public int LOC_ID { get; set; }

        [FieldType(PropertyType = PDFFieldType.Skip)]
        public int EFORM_ID { get; set; }

        [FieldType(PropertyType = PDFFieldType.Skip)]
        public int FD_ON_ID { get; set; }

        [FieldType(PropertyType = PDFFieldType.Skip)]
        public int PR_ROAD_CODE { get; set; }

        [FieldType(PropertyType = PDFFieldType.Skip)]
        public int ROW_ID { get; set; }
        

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [Required(ErrorMessage = "Page-13: Item 4. FOUNDATION- A) Ongoing - Workmanship Table- Please enter Location/ Item of work of row ")]
        [StringLength(20, ErrorMessage = "Page-13: Item 4. FOUNDATION- A) Ongoing - Workmanship Table- The length must be 20 character or less for Location/ Item of work of row ")]
        public string WORK_ITEM_LOC_2 { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [Required(ErrorMessage = "Page-13: Item 4. FOUNDATION- A) Ongoing - Workmanship Table- Please enter Grade of concrete of row ")]
        [StringLength(20, ErrorMessage = "Page-13: Item 4. FOUNDATION- A) Ongoing - Workmanship Table- The length must be 20 character or less for Grade of concrete of row ")]
        public string CONCRETE_GRADE_2 { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [Required(ErrorMessage = "Page-13: Item 4. FOUNDATION- A) Ongoing - Workmanship Table- Please enter Form work quality and arrangements of row")]
        [StringLength(20, ErrorMessage = "Page-13: Item 4. FOUNDATION- A) Ongoing - Workmanship Table- The length must be 20 character or less for Form work quality and arrangements of row")]
        public string WORK_QUALITY_AGG_2 { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [Required(ErrorMessage = "Page-13: Item 4. FOUNDATION- A) Ongoing - Workmanship Table- Please enter Reinforcement checked by competent authority of row")]
        [StringLength(20, ErrorMessage = "Page-13: Item 4. FOUNDATION- A) Ongoing - Workmanship Table- The length must be 20 character or less for Reinforcement checked by competent authority of row")]
        public string REINFORCEMENT_CHECK_AUTH_2 { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [Required(ErrorMessage = "Page-13: Item 4. FOUNDATION- A) Ongoing - Workmanship Table- Please enter Workability Of Concrete of row")]
        [StringLength(20, ErrorMessage = "Page-13: Item 4. FOUNDATION- A) Ongoing - Workmanship Table- The length must be 20 character or less for Workability Of Concrete of row")]
        public string CONCRETE_WORKABILITY_2 { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [Required(ErrorMessage = "Page-13: Item 4. FOUNDATION- A) Ongoing - Workmanship Table- Please enter Compaction arrangement of row ")]
        [StringLength(20, ErrorMessage = "Page-13: Item 4. FOUNDATION- A) Ongoing - Workmanship Table- The length must be 20 character or less for Compaction arrangement of row ")]
        public string COMPACTION_AGG_2 { get; set; }
        
        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [Required(ErrorMessage = "Page-13: Item 4. FOUNDATION- A) Ongoing - Workmanship Table- Please enter Curing arrangements of row ")]
        [StringLength(20, ErrorMessage = "Page-13: Item 4. FOUNDATION- A) Ongoing - Workmanship Table- The length must be 20 character or less for Curing arrangements of row ")]
        public string CURING_AGG_2 { get; set; }

        [FieldType(PropertyType = PDFFieldType.RadioButton)]
        [Required(ErrorMessage = "Page-13: Item 4. FOUNDATION:- A) Ongoing - Workmanship Table- Please select Grading of row ")]
        [RegularExpression(pattern: @"^[A-Za-z]{0,1}$", ErrorMessage = "Page-13: Item 4. FOUNDATION:- A) Ongoing - Workmanship Table- Maximum one character is allowed in Grading of row ")]
        public string GRADE_2 { get; set; }

        [FieldType(PropertyType = PDFFieldType.Skip)]
        public string IPADD { get; set; }

        [FieldType(PropertyType = PDFFieldType.Skip)]
        public int USER_ID { get; set; }
    }
}