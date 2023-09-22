using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PMGSY.Areas.EFORMArea.QMBridgeViewPdfModel
{
    public class EFORM_BRIDGE_QM_QC_TEST_DETAILS_VIEW
    {
        public int ITEM_ID { get; set; }

        public string DPR_QUANTITY { get; set; }
        public string EXECUTED_QUANTITY { get; set; }
        public string TEST_NAME { get; set; }
        public string REQD_TEST_NUMBER { get; set; }
        public string CONDUCTED_TEST_NUMBER { get; set; }
        public string IS_TESTING_ADEQUATE { get; set; }

    }
}