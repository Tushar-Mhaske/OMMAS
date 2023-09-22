//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace PMGSY.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class REAT_RECEIPT_DETAILS
    {
        public int RECEIPT_ID { get; set; }
        public long FILE_ID { get; set; }
        public int ADMIN_ND_CODE { get; set; }
        public string BATCH_ID { get; set; }
        public string BILL_NO { get; set; }
        public System.DateTime BILL_DATE { get; set; }
        public string CHQ_NO { get; set; }
        public System.DateTime CHQ_DATE { get; set; }
        public decimal CHQ_AMOUNT { get; set; }
        public decimal CASH_AMOUNT { get; set; }
        public decimal GROSS_AMOUNT { get; set; }
        public long BILL_ID { get; set; }
        public string REQ_FILENAME { get; set; }
        public System.DateTime REQ_FILE_GEN_DATE { get; set; }
        public string ACK_RECEIVED_FILENAME { get; set; }
        public string ACK_STATUS { get; set; }
        public string REJECTION_CODE { get; set; }
        public string REJECTION_NARRATION { get; set; }
        public Nullable<System.DateTime> ACK_RECEIVED_DATE { get; set; }
        public Nullable<bool> PLACED_ON_SFTP { get; set; }
        public Nullable<System.DateTime> DATE_OF_PLACING { get; set; }
        public Nullable<bool> COPIED_TO_BACKUP { get; set; }
        public Nullable<bool> DELETED_FROM_SOURCE { get; set; }
        public Nullable<System.DateTime> DATE_OF_DELETION { get; set; }
        public string CHKSUM { get; set; }
    
        public virtual ACC_BILL_MASTER ACC_BILL_MASTER { get; set; }
        public virtual ADMIN_DEPARTMENT ADMIN_DEPARTMENT { get; set; }
        public virtual REAT_DATA_SEND_DETAILS REAT_DATA_SEND_DETAILS { get; set; }
    }
}
