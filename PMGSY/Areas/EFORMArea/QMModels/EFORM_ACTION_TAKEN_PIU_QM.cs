using PMGSY.Areas.EFORMArea.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using static PMGSY.Areas.EFORMArea.Common.CommonEnum;

namespace PMGSY.Areas.EFORMArea.QMModels
{
    public class EFORM_ACTION_TAKEN_PIU_QM
    {

        public EFORM_ACTION_TAKEN_PIU_QM()
        {
            this.ROW_ID = 3;

        }

        [FieldType(PropertyType = PDFFieldType.Skip)]
        public int ROW_ID { get; set; }


        [FieldType(PropertyType = PDFFieldType.Skip)]
        public int ACTION_ID { get; set; }

        [FieldType(PropertyType = PDFFieldType.Skip)]
        public int EFORM_ID { get; set; }

        [FieldType(PropertyType = PDFFieldType.Skip)]
        public int IMS_PR_ROAD_CODE { get; set; }



        [FieldType(PropertyType = PDFFieldType.TextBox)]
       // [Required(ErrorMessage = "Page-33: Please Enter ")]
        [StringLength(100, ErrorMessage = "Page-33: ITEM 21. General Observations of QM- D. The length must be 100 character or less for previous QM's Name & Designation NQM/SQM/DO of row ")]
        public string PREV_QM_DESIG_33 { get; set; }



        [FieldType(PropertyType = PDFFieldType.TextBox)]
       // [Required(ErrorMessage = "Page-33: Please Enter  ")]
        [StringLength(250, ErrorMessage = "Page-33: ITEM 21. General Observations of QM- D.  The length must be 250 character or less for Previous QM's observation of row ")]
        public string PREV_QM_OBSERVATION_33 { get; set; }


        [FieldType(PropertyType = PDFFieldType.TextBox)]
       // [Required(ErrorMessage = "Page-33: ITEM 21. General Observations of QM- D.  Please Enter  ")]
        [StringLength(250, ErrorMessage = "Page-33: ITEM 21. General Observations of QM- D.  The length must be 250 character or less for Action taken by PIU of row ")]
        public string ACTION_TAKEN_PIU_33 { get; set; }


        [FieldType(PropertyType = PDFFieldType.TextBox)]
       // [Required(ErrorMessage = "Page-33: ITEM 21. General Observations of QM- D.  Please Enter  ")]
        [StringLength(250, ErrorMessage = "Page-33: ITEM 21. General Observations of QM- D.  The length must be 250 character or less for Your observation about PIU's action of row ")]
        public string OBSERVATION_ON_PIU_ACTION_33 { get; set; }




        [FieldType(PropertyType = PDFFieldType.Skip)]
        public int QM_USER_ID { get; set; }

        [FieldType(PropertyType = PDFFieldType.Skip)]
        public string IPADD { get; set; }

    }
}