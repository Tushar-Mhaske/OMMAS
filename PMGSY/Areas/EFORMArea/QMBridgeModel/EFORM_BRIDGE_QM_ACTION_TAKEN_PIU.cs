using PMGSY.Areas.EFORMArea.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using static PMGSY.Areas.EFORMArea.Common.CommonEnum;

namespace PMGSY.Areas.EFORMArea.QMBridgeModel
{
    public class EFORM_BRIDGE_QM_ACTION_TAKEN_PIU
    {

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [StringLength(100, ErrorMessage = "Page-31: Item 12. D.- The length must be 100 character or less for Previous QM's Designation(NQM /SQM /)DO in row ")]

        public string PREV_QM_DESIG { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [StringLength(250, ErrorMessage = "Page-31: Item 12. D.- The length must be 250 character or less for Previous QM's Observation in row  .  ")]
        public string PREV_QM_OBSERVATION { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [StringLength(250, ErrorMessage = "Page-31: Item 12. D.- The length must be 250 character or less for Action taken By PIU in row  .  ")]
        public string ACTION_TAKEN_PIU { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [StringLength(250, ErrorMessage = "Page-31: Item 12. D.- The length must be 250 character or less for Your observation about PIU's action in row  .  ")]
        public string OBSERVATION_ON_PIU_ACTION { get; set; }

        [FieldType(PropertyType = PDFFieldType.Skip)]
        public int ROW_ID { get; set; }

    }
}