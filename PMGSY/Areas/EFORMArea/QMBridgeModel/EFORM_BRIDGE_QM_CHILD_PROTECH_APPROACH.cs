using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using static PMGSY.Areas.EFORMArea.Common.CommonEnum;
using System.ComponentModel.DataAnnotations;
using PMGSY.Areas.EFORMArea.Model;
namespace PMGSY.Areas.EFORMArea.QMBridgeModel
{
    public class EFORM_BRIDGE_QM_CHILD_PROTECH_APPROACH
    {

        [FieldType(PropertyType = PDFFieldType.RadioButton)]
        [Required(ErrorMessage = "Page-27: Item 10. APPROACHES: 10.5 Protection Work- I. Please select Whether sanctioned DPR has the provision of protection works ")]
        [StringLength(3, ErrorMessage = "Page-27 Item 10. APPROACHES: 10.5 Protection Work- I. The length must be 3 character or less for Whether sanctioned DPR has the provision of protection works")]

        public string IS_DPR_PROV_PROTE_WORKS { get; set; }

        [FieldType(PropertyType = PDFFieldType.CheckBox)]
        [RegularExpression(pattern: @"^[Y,N]{1}$", ErrorMessage = "Page-27: Item 10. APPROACHES 10.5 Protection Work- II. only (Y/N) is allowed in Type of protection work- Retaining wall checkbox ")]

        public string IS_RETAINING_WALL { get; set; }

        [FieldType(PropertyType = PDFFieldType.CheckBox)]
        [RegularExpression(pattern: @"^[Y,N]{1}$", ErrorMessage = "Page-27: Item 10. APPROACHES 10.5 Protection Work- II. only (Y/N) is allowed in Type of protection work-Breast wall checkbox ")]

        public string IS_BREAST_WALL { get; set; }

        [FieldType(PropertyType = PDFFieldType.CheckBox)]
        [RegularExpression(pattern: @"^[Y,N]{1}$", ErrorMessage = "Page-27: Item 10. APPROACHES 10.5 Protection Work- II. only (Y/N) is allowed in	Type of protection work-Parapet wall checkbox ")]

        public string IS_PARAPET_WALL { get; set; }

        [FieldType(PropertyType = PDFFieldType.CheckBox)]
        [RegularExpression(pattern: @"^[Y,N]{1}$", ErrorMessage = "Page-27: Item 10. APPROACHES 10.5 Protection Work- II. only (Y/N) is allowed in	Type of protection work-Any other type of Protection work checkbox ")]

        public string IS_ANY_OTHER_PROT_WORK { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [Required(ErrorMessage = "Page-27: Item 10. APPROACHES: 10.5 Protection Work-II. Please enter Any other type of Protection work (a) ")]
        [StringLength(30, ErrorMessage = "Page-27 Item 10. APPROACHES: 10.5 Protection Work- II. The length must be 30 character or less for Any other type of Protection work (a) ")]

        public string IS_ANY_OTHER_PROT_WORK_A { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
     //   [Required(ErrorMessage = "Page-27: Item 10. APPROACHES: 10.5 Protection Work- Please select Grading of Coarse Aggregate (S / U) of row  ")]
        [StringLength(30, ErrorMessage = "Page-27 Item 10. APPROACHES: 10.5 Protection Work-II. The length must be 30 character or less for Any other type of Protection work (b) ")]

        public string IS_ANY_OTHER_PROT_WORK_B { get; set; }

        [FieldType(PropertyType = PDFFieldType.RadioButton)]
        [Required(ErrorMessage = "Page-28: Item 10. APPROACHES: 10.5 Protection Work- V. a) Please select Workmanship of stone masonry is acceptable  ")]
        [StringLength(3, ErrorMessage = "Page-28 Item 10. APPROACHES: 10.5 Protection Work-V. a) The length must be 3 character or less for Workmanship of stone masonry is acceptable  ")]

        public string WORKMENSHIP_STONE_ACCEP { get; set; }

        [FieldType(PropertyType = PDFFieldType.RadioButton)]
        [Required(ErrorMessage = "Page-28: Item 10. APPROACHES: 10.5 Protection Work-V. b) Please select Bond stone has been provided in stone masonry  ")]
        [StringLength(3, ErrorMessage = "Page-28 Item 10. APPROACHES: 10.5 Protection Work-V. b) The length must be 3 character or less for Bond stone has been provided in stone masonry ")]

        public string IS_BOND_STONE_PROVIDED { get; set; }
    

    }
}