using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PMGSY.Areas.EFORMArea.QMBridgeViewPdfModel
{
    public class EFORM_BRIDGE_QM_CHILD_ON_QOM_STEEL_SUPERSTRUCTURE_VIEW
    {


       
        public string RD_LOC { get; set; }
        public string STEEL_GRADE { get; set; }
       
        public string TENSILE_STRENGTH { get; set; }
       
        public string YIELD_STRESS_PER_QC { get; set; }
       
        public string ELONGATION_PER_QC { get; set; }  
       
        public string IS_CHEMICAL_ANALYSIS_DONE { get; set; }
       
        public string IS_CHEM_ANALYSIS_ADEQUATE { get; set; }
       
        public string IS_NDT_WELDING_DONE { get; set; }
        
        public string IS_TESTING_OF_STEEL_BOLT { get; set; }
        
        public string IS_PAINTING_PROPER { get; set; }
        
        public string IS_HFL_CLEARANCE_ADEQUATE { get; set; }
    }
}