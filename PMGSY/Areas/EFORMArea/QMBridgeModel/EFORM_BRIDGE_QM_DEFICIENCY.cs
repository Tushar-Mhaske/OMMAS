using PMGSY.Areas.EFORMArea.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using static PMGSY.Areas.EFORMArea.Common.CommonEnum;

namespace PMGSY.Areas.EFORMArea.QMBridgeModel
{
    public class EFORM_BRIDGE_QM_DEFICIENCY
    {
        [FieldType(PropertyType = PDFFieldType.Skip)]
        public int? DEF_ID { get; set; }
        
        [FieldType(PropertyType = PDFFieldType.RadioButton)]
        [Required(ErrorMessage = "Page-30: Item 12. A. I or II Please Select Observations about deficiency in project preparation :")]
        public string IS_NO_DEFICIENCY { get; set; }

        [FieldType(PropertyType = PDFFieldType.CheckBox)]
        [StringLength(3, ErrorMessage = "Page-30: Item 12. A.  II The length must be 3 character or less for Nomenclature of BOQ Items is not clearly stated such as what type of binder (VGGrade/Emulsion) has to be used and the quantity of such items.")]

        public string IS_BOQ_NOT_CLEAR { get; set; }


        [FieldType(PropertyType = PDFFieldType.CheckBox)]
        [StringLength(3, ErrorMessage = "Page-30: Item 12. A.  II The length must be 3 character or less for Number of spans are insufficient as per the site's hydrological condition")]

        public string IS_NO_SPANS_INSUFFICIENT { get; set; }

        [FieldType(PropertyType = PDFFieldType.CheckBox)]
        [StringLength(3, ErrorMessage = "Page-30: Item 12. A.  II The length must be 3 character or less for No provision of protective work in DPR but as per site conditions it is required")]

        public string IS_NO_PROVISION_PROTECTIVE_WORK { get; set; }

        [FieldType(PropertyType = PDFFieldType.CheckBox)]
        [StringLength(3, ErrorMessage = "Page-30: Item 12. A.  II The length must be 3 character or less for Hydraulic Design & calculation for bridge design not provisioned in DPR")]

        public string IS_HYDROLIC_DESIGN_DPR { get; set; }

        [FieldType(PropertyType = PDFFieldType.CheckBox)]
        [StringLength(3, ErrorMessage = "Page-30: Item 12. A.  II The length must be 3 character or less for Hydraulic Design & calculation for Guard stone/ Crash barrier/ Road studs on approches are required to be provisioned inDPR.")]

        public string IS_GUARD_STONE_IN_DPR { get; set; }

        [FieldType(PropertyType = PDFFieldType.CheckBox)]
        [StringLength(3, ErrorMessage = "Page-30: Item 12. A.  II The length must be 3 character or less for Deviation from proposed Alignment")]

        public string IS_DEVIATION_ALIGNMENT { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [StringLength(250, ErrorMessage = "Page-30: Item 12. A.- The length must be 250 character or less for suggestions for improvement  ")]
        public string OTHR_COMMENT { get; set; }
       
    }
}