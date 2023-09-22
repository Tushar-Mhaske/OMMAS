using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace PMGSY.Models.Gepnic
{
    public class GepnicTenderDetailsViewModel
    {
       // [StringLength(15,ErrorMessage="Maximum length for Package Number is 15 only. ")]
       // [RegularExpression(@"^[a-zA-Z0-9 -/]+$", ErrorMessage = "Invalid Package Number.")]
       // [Required(ErrorMessage = "Package No. field is required.")]
        public string packageRefNo { get; set; }
    }

    public class XMLCreation
    {
        [StringLength(15, ErrorMessage = "Maximum length for Date is 8 only. ")]
        [RegularExpression(@"^[0-9]+$", ErrorMessage = "Invalid Date Number.")]
        [Required(ErrorMessage = "Date field is required.")]
        public string DateStringNumber { get; set; }
    }


    public class AOCInfoByPublishedDate
    {
        public int AOCID { get; set; } // Primary Key
        public string A_TENDERID { get; set; }
        public string A_TENDERREFNO { get; set; }
        public string A_WORKITEMID { get; set; }
        public string A_TENDER_DESC { get; set; }
        public string A_TENDER_TYPE { get; set; }
        public DateTime A_PUBLISHED_DATE { get; set; }
        public string A_NO_OF_BIDS { get; set; }
        public string A_NO_BIDS_RECD { get; set; }
        public string A_L1BIDDER_NAME { get; set; }
        public string A_L1BIDDER_ADDRESS { get; set; }
        public DateTime A_CONTRACT_DATE { get; set; }
        public string A_CONTRACT_NO { get; set; }
        public string A_CURRENCY { get; set; }
        public string A_CONTRACT_VALUE { get; set; }
        public string A_DATE_COMPLETION { get; set; }
        public string A_CONT_VALID_FROM { get; set; }
        public string A_CONT_VALID_TO { get; set; }
        public string A_PARTIES_QUALIFIED { get; set; }
        public string A_PARTIES_NOTQUALIFIED { get; set; }
        public string A_ORGID { get; set; }
        public string A_ORGNAME { get; set; }
        public string A_REMARKS { get; set; }
       
        public DateTime GENERATED_DATE { get; set; }

    }

    public class CorriInfoByPublishedDate
    {
         public int CORRID { get; set; } // Primary key
         public string C_ID { get; set; }
         public DateTime C_DATE { get; set; }
         public string C_TYPE { get; set; }
         public string C_STATUS { get; set; }
         public string T_REF_NO { get; set; }
         public string T_ID { get; set; }
         public string C_TITLE { get; set; }
         public string C_DESC { get; set; }
         public string T_PRE_QUAL { get; set; }
         public string T_LOCATION { get; set; }
         public string T_PINCODE { get; set; }
         public string T_CURRENCY { get; set; }
         public decimal? T_FEE { get; set; }
         public decimal? T_VALUE { get; set; }
         public decimal? T_EMD { get; set; }
         public DateTime T_PUB_DATE { get; set; }
         public DateTime T_PREBID_DATE { get; set; }
         public DateTime T_DOC_START_DATE { get; set; }
         public DateTime T_DOC_END_DATE { get; set; }
         public DateTime T_BIDSUB_START_DATE { get; set; }
         public DateTime T_BIDSUB_END_DATE { get; set; }
         public DateTime T_BID_OPEN_DATE { get; set; }
         public string T_INVITING_OFFICER { get; set; }
         public string T_INVITING_OFF_ADDRESS { get; set; }
         public string T_PROD_CAT { get; set; }
         public string T_PROD_SUB_CAT { get; set; }
         public string T_TENDER_TYPE { get; set; }
         public string T_TENDER_CATEGORY { get; set; }
         public string T_FORM_CONTRACT { get; set; }
         public string T_RETURN_URL { get; set; }
         public string T_REMARKS { get; set; }
         public string TENDER_CORRIGENDUM { get; set; }
         public DateTime GENERATED_DATE { get; set; }
        
    }


    public class AocInfoByCreatedDate
    {

        public int AOCID { get; set; } // Primary Key
        public string A_TENDERID { get; set; }
        public string A_TENDERREFNO { get; set; }
        public string A_WORKITEMID { get; set; }
        public string A_TENDER_DESC { get; set; }
        public string A_TENDER_TYPE { get; set; }
        public DateTime A_PUBLISHED_DATE { get; set; }
        public string A_NO_OF_BIDS { get; set; }
        public string A_NO_BIDS_RECD { get; set; }
        public string A_L1BIDDER_NAME { get; set; }
        public string A_L1BIDDER_ADDRESS { get; set; }
        public DateTime A_CONTRACT_DATE { get; set; }
        public string A_CONTRACT_NO { get; set; }
        public string A_CURRENCY { get; set; }
        public string A_CONTRACT_VALUE { get; set; }
        public string A_DATE_COMPLETION { get; set; }
        public string A_CONT_VALID_FROM { get; set; }
        public string A_CONT_VALID_TO { get; set; }
        public string A_PARTIES_QUALIFIED { get; set; }
        public string A_PARTIES_NOTQUALIFIED { get; set; }
        public string A_ORGID { get; set; }
        public string A_ORGNAME { get; set; }
        public string A_REMARKS { get; set; }
        public string A_RETURN_URL { get; set; }
        public DateTime GENERATED_DATE { get; set; }
    }


    public class WORKITEMDETAILS
    {
        public string WORKITEM_REF_NO { get; set; }
        public string ORG_CHAIN { get; set; }
        public string T_REF_NO { get; set; }

        public string T_TENDER_TYPE { get; set; }
        public string T_FORM_OF_CONTRACT { get; set; }
        public string T_TENDER_CATEGORY { get; set; }

        public string W_TITLE { get; set; }
        public string W_DESC { get; set; }
        public string W_PRE_QUAL { get; set; }

        public string W_PROD_CAT { get; set; }
        public string W_PROD_SUB_CAT { get; set; }
        public string W_VALUE { get; set; }

        public string W_BIDVALIDITY { get; set; }
        public string W_LOCATION { get; set; }
        public int W_PINCODE { get; set; }

        public string W_PREBID_MEET_PLACE { get; set; }
        public string W_PREBID_ADDRESS { get; set; }
        public string W_BID_OPENING_PLACE { get; set; }

        public string W_INVITING_OFFICER { get; set; }
        public string W_INVITING_OFF_ADDRESS { get; set; }
        public Decimal? W_TENDER_FEE { get; set; }

        public string W_TF_PAYABLE_TO { get; set; }
        public string W_TF_PAYABLE_AT { get; set; }
        public Decimal? W_EMD_FEE { get; set; }

        public string W_EMD_PAYABLE_TO { get; set; }
        public string W_EMD_PAYABLE_AT { get; set; }
        public string W_PUB_DATE { get; set; }


        public string W_DOC_START_DATE { get; set; }
        public string W_DOC_END_DATE { get; set; }
        public string W_SEEK_CLAR_START_DATE { get; set; }

        public string W_SEEK_CLAR_END_DATE { get; set; }
        public string W_PREBID_DATE { get; set; }
        public string W_BIDSUB_START_DATE { get; set; }

        public string W_BIDSUB_END_DATE { get; set; }
        public string W_BID_OPEN_DATE { get; set; }
        public string W_FIN_BID_OPEN_DATE { get; set; }

        public string W_BID_OPENERS { get; set; }
        public string W_RETURN_URL { get; set; }
        public string W_NO_OF_BIDS { get; set; }

    }

    public class Corrigendum
    {
        public string CORR_NAME { get; set; }
        public string CORR_TYPE { get; set; }
        public DateTime CORR_PUB_DATE { get; set; }
        public string CORR_PUB_BY { get; set; }
    }

    public class Bidders
    {
        public int BID_ID { get; set; }
        public string BIDDER_NAME { get; set; }
        public DateTime BID_PLACED_DATE { get; set; }
        public string BID_IP_ADDRESS { get; set; }
    }


    public class TendersByPublishDate
    {
        public int T_ID { get; set; }
        public string T_REF_NO { get; set; }
        public string T_TITLE { get; set; }
        public string T_DESC { get; set; }
        public string T_PRE_QUAL { get; set; }
        public string T_LOCATION { get; set; }
        public string T_PINCODE { get; set; }
        public string T_CURRENCY { get; set; }
        public string T_FEE { get; set; }
        public string T_VALUE { get; set; }
        public string T_EMD { get; set; }
        public string T_PUB_DATE { get; set; }
        public string T_PREBID_DATE { get; set; }
        public string T_DOC_START_DATE { get; set; }
        public string T_DOC_END_DATE { get; set; }
        public string T_BIDSUB_START_DATE { get; set; }
        public string T_BIDSUB_END_DATE { get; set; }
        public string T_BID_OPEN_DATE { get; set; }
        public string T_INVITING_OFFICER { get; set; }
        public string T_INVITING_OFF_ADDRESS { get; set; }
        public string T_PROD_CAT { get; set; }
        public string T_PROD_SUB_CAT { get; set; }
        public string T_TENDER_TYPE { get; set; }
        public string T_TENDER_CATEGORY { get; set; }
        public string T_FORM_CONTRACT { get; set; }
        public string T_RETURN_URL { get; set; }
        public string T_ORGNAME { get; set; }    
    }

    public class BIDOPEN
    {
        public int BID_ID { get; set; }
        public string BIDDER_NAME { get; set; }
        public DateTime BID_OPENED_DATE { get; set; }
        public string BID_OPENED_BY { get; set; }
    }

    public class BIDEVAL
    {
        public int BID_ID { get; set; }
        public string BIDDER_NAME { get; set; }
        public DateTime BID_EVAL_DATE { get; set; }
        public string BID_EVAL_BY { get; set; }
    }

    public class AOC
    {
        public int GEPNIC_BID_ID { get; set; }
        public string BIDDER_NAME { get; set; }
        public DateTime CONTRACT_DATE { get; set; }
        public int CONTRACT_NO { get; set; }
        public decimal AWARDED_VALUE { get; set; }
    }


    public class TenderXMLByPublishdate
    {
        public int TINFOID { get; set; } // Autoincreamented Primary Key
        public string T_ID { get; set; }
        public string T_REF_NO { get; set; }
        public string T_TITLE { get; set; }

        public string T_DESC { get; set; }// Added on 5 Oct 2018


        public string T_PRE_QUAL { get; set; }
        public string T_LOCATION { get; set; }
        public string T_PINCODE { get; set; }
        public string T_CURRENCY { get; set; }

        public decimal? T_FEE { get; set; }
        public decimal? T_VALUE { get; set; }
        public decimal? T_EMD { get; set; }

        public DateTime? T_PUB_DATE { get; set; }
        public DateTime? T_PREBID_DATE { get; set; }
        public DateTime? T_DOC_START_DATE { get; set; }
        public DateTime? T_DOC_END_DATE { get; set; }
        public DateTime? T_BIDSUB_START_DATE { get; set; }
        public DateTime? T_BIDSUB_END_DATE { get; set; }
        public DateTime? T_BID_OPEN_DATE { get; set; }
        public string T_INVITING_OFFICER { get; set; }
        public string T_INVITING_OFF_ADDRESS { get; set; }

        public string T_PROD_CAT { get; set; }

        public string T_PROD_SUB_CAT { get; set; } 

        public string T_TENDER_TYPE { get; set; }
        public string T_TENDER_CATEGORY { get; set; }
        public string T_FORM_CONTRACT { get; set; }
        public string T_RETURN_URL { get; set; }
        public string T_REMARKS { get; set; }
        public string T_ORGNAME { get; set; }
        public DateTime GENERATED_DATE { get; set; }


    }

}