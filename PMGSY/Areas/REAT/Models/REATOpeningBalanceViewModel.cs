using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace PMGSY.Areas.REAT.Models
{
    public class REATOpeningBalanceViewModel
    {
        public string BANK_ACC_NO { get; set; }
        public string BANK_NAME { get; set; }
        public string BANK_BRANCH { get; set; }
        public string BANK_ADDRESS1 { get; set; }
        public string BANK_PHONE1 { get; set; }
        public string BANK_EMAIL { get; set; }

        [DataType(DataType.Date)]
        public Nullable<System.DateTime> BANK_ACC_OPEN_DATE { get; set; }

        public string MAST_IFSC_CODE { get; set; }
        public string OB_STATUS { get; set; }

        [Required(ErrorMessage = "Please enter opening balance")]
        public decimal OB_AMOUNT { get; set; }

        public string OB_BALANCE { get; set; }

        [Required(ErrorMessage = "Please select opening date")]
        [DataType(DataType.Date)]
        public System.DateTime OB_DATE { get; set; }
    }
}