using PMGSY.Areas.EFORMArea.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using static PMGSY.Areas.EFORMArea.Common.CommonEnum;

namespace PMGSY.Areas.EFORMArea.QMBridgeModel
{
    public class EFORM_BRIDGE_QM_DIFFERENCE_IN_OBSERVATION
    {

        [FieldType(PropertyType = PDFFieldType.RadioButton)]
        [Required(ErrorMessage = "Page-32: Item 12. E. Please Select Whether any difference found in previous observations QMs:")]
        [StringLength(250, ErrorMessage = "Page-32: Item 12. E. The length must be 250 character or less for Whether any difference found in previous observations QMs ")]

        public string IS_DIFFERENCE_FOUND { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [StringLength(250, ErrorMessage = "Page-32: Item 12. E.- The length must be 250 character or less for comment on difference in Observation  ")]
        [Required(ErrorMessage = "Page-32: Item 12. E. Please enter your comment on difference in Observation")]
        public string COMMENT_ON_DIFFERENCE { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [StringLength(250, ErrorMessage = "Page-32: Item 12. E.- The length must be 250 character or less for Other observations, if any ")]
        public string OTHER_OBSERVATION { get; set; }
        

    }
}