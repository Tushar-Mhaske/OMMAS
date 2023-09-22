using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PMGSY.Areas.EFORMArea.Model
{
    public class PdfRoadListModel
    {

        public int EFORM_ID { get; set; }
        public int MAST_PMGSY_SCHEME { get; set; }
        public string PIUfinalize_status { get; set; }
        public string District { get; set; }
        public string Block { get; set; }

        public string SANCTION_YEAR { get; set; }

        public Nullable<decimal> SANCTION_length { get; set; }
        public Nullable<decimal> EXECUTED_length { get; set; }

        public bool UserStatus { get; set; }

        public bool UserStatusTR { get; set; }
        public string PIU_Name { get; set; }

        public string Completion_Date { get; set; }

        public string Work_Status { get; set; }
        public string IMS_PROPOSAL_TYPE { get; set; }
        public int IMS_YEAR { get; set; }
        public int IMS_BATCH { get; set; }
        public string IMS_PACKAGE_ID { get; set; }
        public string IMS_ROAD_NAME { get; set; }
        public int IMS_PR_ROAD_CODE { get; set; }
        public string IMS_BRIDGE_NAME { get; set; }
        public string PIU_Status { get; set; }
        public string isfileupload { get; set; }
        public string isPIUfileupload { get; set; }

        public string isQMfileupload { get; set; }
        public string QM_NAME { get; set; }
        public string ADMIN_QM_CODE { get; set; }
        public string QM_TYPE { get; set; }

        public int ADMIN_SCHEDULE_CODE { get; set; }

        public int ADMIN_SCHEDULE_MONTH { get; set; }

        public int ADMIN_SCHEDULE_YEAR { get; set; }

        public string isQMfinalised { get; set; }


        //Ajinkya
        //   public string isTRfileuploaded { get; set; }
        //Ajinkya
        //   public string isTrEform { get; set; }
        //Ajinkya
        //  public string isTRfinalised { get; set; }

        public string isAllPdfUpload { get; set; }

        public string isETRuploaded { get; set; }


        public string isScanTRuploaded { get; set; }

        public string isETRfinalised { get; set; }

        public string isScanTRfinalised { get; set; }

        public string isItem3Uploaded { get; set; }


    }
}