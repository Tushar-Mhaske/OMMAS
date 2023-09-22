using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PMGSY.Areas.EFORMArea.QM_ViewPdf_Model
{
    public class QM_SHOULDERS_MATERIAL_DETAILS
    {
        public decimal LOCATION_RD { get; set; }
        public string GRADING_QOM_HAND_FEEL { get; set; }
        public decimal COMPAQ_DEGREE_ASPER_QCR1 { get; set; }
        public decimal COMPAQ_DEGREE_MEAS_QM { get; set; }
        public string GRADING_COMPAQ_DEGREE { get; set; }
        public decimal CAMBER_ASPER_DPR { get; set; }
        public decimal CAMBER_MEAS_QM { get; set; }
        public string GRADING_CAMBER { get; set; }
        public decimal SECTIONAL_WIDTH { get; set; }
        public decimal SECTIONAL_THICKNESS { get; set; }
        public string GRADING_SECTIONAL { get; set; }
    }
}