using PMGSY.Areas.EFORMArea.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using static PMGSY.Areas.EFORMArea.Common.CommonEnum;

namespace PMGSY.Areas.EFORMArea.QMBridgeModel
{
    public class EFORM_BRIDGE_QM_CHILD_ON_WORKMENSHIP_SUBSTRUCTURE
    {

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [Required(ErrorMessage = "Page-15: Item 5. SUBSTRUCTURE (A)-Table No. 02: Please enter Location/ Item of work  of row ")]
        [StringLength(20, ErrorMessage = "Page-15: Item 5. SUBSTRUCTURE (A)- Table No. 02: The length must be 20 character or less for Location/ Item of work of row ")]
        public string WORK_ITEM_LOC_15_2 { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [Required(ErrorMessage = "Page-15: Item 5. SUBSTRUCTURE (A)- Table No. 02: Please enter Grade of Concrete  of row ")]
        [StringLength(20, ErrorMessage = "Page-15: Item 5. SUBSTRUCTURE (A)- Table No. 02: The length must be 20 character or less for Grade of Concrete  of row ")]
        public string CONCRETE_GRADE_15_2 { get; set; }


        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [Required(ErrorMessage = "Page-15: Item 5. SUBSTRUCTURE (A)- Table No. 02: Please enter Form work quality and arrangements of row ")]
        [StringLength(20, ErrorMessage = "Page-15: Item 5. SUBSTRUCTURE (A)- Table No. 02: The length must be 20 character or less for Form work quality and arrangements of row ")]
        public string WORK_QUALITY_ARRG_15_2 { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [Required(ErrorMessage = "Page-15: Item 5. SUBSTRUCTURE (A)- Table No. 02: Please enter Reinforcement checked by competent authority of row ")]
        [StringLength(20, ErrorMessage = "Page-15: Item 5. SUBSTRUCTURE (A)- Table No. 02: The length must be 20 character or less for Reinforcement checked by competent authority of row ")]
        public string REINFORCEMENT_CHECK_AUTH_15_2 { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [Required(ErrorMessage = "Page-15: Item 5. SUBSTRUCTURE (A)- Table No. 02: Please enter Workability of Concrete  of row ")]
        [StringLength(20, ErrorMessage = "Page-15: Item 5. SUBSTRUCTURE (A)- Table No. 02: The length must be 20 character or less for Workability of Concrete  of row ")]
        public string CONCRETE_WORKABILITY_15_2 { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [Required(ErrorMessage = "Page-15: Item 5. SUBSTRUCTURE (A)- Table No. 02: Please enter Compaction Arrangements  of row ")]
        [StringLength(20, ErrorMessage = "Page-15: Item 5. SUBSTRUCTURE (A)- Table No. 02: The length must be 20 character or less for Compaction Arrangements  of row ")]
        public string COMPACTION_ARRG_15_2 { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [Required(ErrorMessage = "Page-15: Item 5. SUBSTRUCTURE (A)- Table No. 02: Please enter Curing Arrangements  of row ")]
        [StringLength(20, ErrorMessage = "Page-15: Item 5. SUBSTRUCTURE (A)- Table No. 02: The length must be 20 character or less for Curing Arrangements  of row ")]
        public string CURING_ARRG_15_2 { get; set; }


        [FieldType(PropertyType = PDFFieldType.RadioButton)]
        [Required(ErrorMessage = "Page-15: Item 5. SUBSTRUCTURE (A)- Table No. 02: Please enter Grade(S/U)  of row ")]
        [StringLength(3, ErrorMessage = "Page-15: Item 5. SUBSTRUCTURE (A)- Table No. 02: The length must be 3 character or less for  Grade(S/U)  of row ")]
        public string GRADE_15_2 { get; set; }


       

        [FieldType(PropertyType = PDFFieldType.Skip)]
        public int RowID { get; set; }

        [FieldType(PropertyType = PDFFieldType.Skip)]
        public string SubstructureType { get; set; }

    }
}