using PMGSY.Areas.EFORMArea.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using static PMGSY.Areas.EFORMArea.Common.CommonEnum;

namespace PMGSY.Areas.EFORMArea.QMBridgeModel
{
    public class EFORM_BRIDGE_QM_CHILD_STEEL_SUPERSTRUCTURE_COMPLETED
    {
        [FieldType(PropertyType = PDFFieldType.RadioButton)]
        [Required(ErrorMessage = "Page-20: Item 6. (ii)Steel Superstructure-B)Completed Work: I. Please select Presence of water spray or moisture in the vicinity of bridge such as waterfall or marshy wet land ")]
        [RegularExpression(pattern: @"^[A-Za-z]{0,3}$", ErrorMessage = "Page-20: Item 6. (ii)Steel Superstructure-B)Completed Work:- I. Maximum three character is allowed in Presence of water spray or moisture in the vicinity of bridge such as waterfall or marshy wet land ")]
        public string IS_PRESENCE_MOISTURE { get; set; }


        [FieldType(PropertyType = PDFFieldType.RadioButton)]
        [Required(ErrorMessage = "Page-20: Item 6. (ii)Steel Superstructure-B)Completed Work:- II. Please select Presence of industrial units nearby, which emit corrosive fumes or discharge chemical effluents ")]
        [RegularExpression(pattern: @"^[A-Za-z]{0,3}$", ErrorMessage = "Page-20: Item 6. (ii)Steel Superstructure-B)Completed Work:- II. Maximum three character is allowed in Presence of industrial units nearby, which emit corrosive fumes or discharge chemical effluents ")]
        public string IS_INDUSTRIAL_UNIT_NEARBY { get; set; }


        [FieldType(PropertyType = PDFFieldType.RadioButton)]
        [Required(ErrorMessage = "Page-20: Item 6. (ii)Steel Superstructure-B)Completed Work:- III. Please select Presence of salinity in the atmosphere ")]
        [RegularExpression(pattern: @"^[A-Za-z]{0,3}$", ErrorMessage = "Page-20: Item 6. (ii)Steel Superstructure-B)Completed Work:- III. Maximum three character is allowed in Presence of salinity in the atmosphere ")]
        public string IS_SALINITY_IN_ATMOSPHERE { get; set; }  //


    }
}