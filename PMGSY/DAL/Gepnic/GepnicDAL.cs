using PMGSY.Common;
using PMGSY.Extensions;
using PMGSY.Models;
using PMGSY.Models.Gepnic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Transactions;

using System.Data.Entity.Validation;
using System.Diagnostics;
using PMGSY.BAL.Menu;
using PMGSY.Models.Login;
using System.Text;
using System.Security.Cryptography;
using System.Xml;
using PMGSY.Models;
using PMGSY.Controllers;

namespace PMGSY.DAL.Gepnic
{
    public class GepnicDAL
    {
        PMGSYEntities dbContext = null;


        /// <summary>
        /// Populate PFMS Bank Names
        /// </summary>
        /// <param name="bankName"></param>
        /// <returns></returns>
        public List<SelectListItem> PopulatePackageRefNoDAL()
        {
            try
            {
                dbContext = new PMGSYEntities();
                List<SelectListItem> lstRefNos = new SelectList(dbContext.OMMAS_GEPNIC_INTEGRATION.Where(x=>x.MAST_STATE_CODE == PMGSYSession.Current.StateCode).Select(x=>x.PACKAGE_NUMBER).Distinct()).ToList();

                //lstRefNos.Insert(0, (new SelectListItem { Text = "Select Ifsc Code", Value = "" }));

                return lstRefNos;
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "GepnicDAL.PopulatePackageRefNoDAL()");
                return null;
            }
            finally
            {
                dbContext.Dispose();
            }
        }

        // 

        public bool AddDetails(GepnicTenderDetailsViewModel model, ref string message, XmlDocument xmlDoc, XmlNodeList workItemNodes, XmlNodeList CorrigendumNodes, XmlNodeList biddersNodes, XmlNodeList techOpenNodes, XmlNodeList techEvalNodes, XmlNodeList finOpenNodes, XmlNodeList finEvalNodes, XmlNodeList aocNodes)
        {
            dbContext = new PMGSYEntities();   
            try
            {
                using (TransactionScope scope = new TransactionScope())
                {
                    #region  Work Item Details
                    OMMAS_GEPNIC_TENDER_WORKITEM_DETAILS master = new OMMAS_GEPNIC_TENDER_WORKITEM_DETAILS();
         
                    WORKITEMDETAILS workItems = new WORKITEMDETAILS();
                    int TenderID = dbContext.OMMAS_GEPNIC_TENDER_WORKITEM_DETAILS.Max(cp => (Int32?)cp.TENDER_ID) == null ? 1 : (Int32)dbContext.OMMAS_GEPNIC_TENDER_WORKITEM_DETAILS.Max(cp => (Int32?)cp.TENDER_ID) + 1;
                    master.TENDER_ID = TenderID;
                    master.GENERATED_DATE = System.DateTime.Now;
                    foreach (XmlNode nodeWorkItems in workItemNodes)
                    {
                        foreach (XmlNode childWorkItems in nodeWorkItems.ChildNodes)
                        {
                            if (childWorkItems.Name.ToUpper().Trim() == "WORKITEM_REF_NO")
                            {
                                workItems.WORKITEM_REF_NO = childWorkItems.InnerXml;
                                master.WORKITEM_REF_NO = workItems.WORKITEM_REF_NO;

                                if (dbContext.OMMAS_GEPNIC_TENDER_WORKITEM_DETAILS.Where(m => m.WORKITEM_REF_NO.ToUpper() == workItems.WORKITEM_REF_NO.ToUpper()).Any())
                                {
                                    message = "This Tender Work Item Details are already added. Dup";
                                    return false;
                                }
                            }

                            if (childWorkItems.Name.ToUpper().Trim() == "ORG_CHAIN")
                            {
                                workItems.ORG_CHAIN = childWorkItems.InnerXml;
                                master.ORG_CHAIN = workItems.ORG_CHAIN;
                            }
                            if (childWorkItems.Name.ToUpper().Trim() == "T_REF_NO")
                            {
                                workItems.T_REF_NO = childWorkItems.InnerXml;
                                master.T_REF_NO = workItems.T_REF_NO;
                            }

                            if (childWorkItems.Name.ToUpper().Trim() == "T_TENDER_TYPE")
                            {
                                workItems.T_TENDER_TYPE = childWorkItems.InnerXml;
                                master.T_TENDER_TYPE = workItems.T_TENDER_TYPE;
                            }

                            if (childWorkItems.Name.ToUpper().Trim() == "T_FORM_OF_CONTRACT")
                            {
                                workItems.T_FORM_OF_CONTRACT = childWorkItems.InnerXml;
                                master.T_FORM_OF_CONTRACT = workItems.T_FORM_OF_CONTRACT;
                            }
                            if (childWorkItems.Name.ToUpper().Trim() == "T_TENDER_CATEGORY")
                            {
                                workItems.T_TENDER_CATEGORY = childWorkItems.InnerXml;
                                master.T_TENDER_CATEGORY = workItems.T_TENDER_CATEGORY;
                            }

                            if (childWorkItems.Name.ToUpper().Trim() == "W_TITLE")
                            {
                                workItems.W_TITLE = childWorkItems.InnerXml;
                                master.W_TITLE = workItems.W_TITLE;
                            }
                            if (childWorkItems.Name.ToUpper().Trim() == "W_DESC")
                            {
                                workItems.W_DESC = childWorkItems.InnerXml;
                                master.W_DESC = workItems.W_DESC;
                            }

                            if (childWorkItems.Name.ToUpper().Trim() == "W_PRE_QUAL")
                            {
                                workItems.W_PRE_QUAL = childWorkItems.InnerXml;
                                master.W_PRE_QUAL = workItems.W_PRE_QUAL;
                            }

                            if (childWorkItems.Name.ToUpper().Trim() == "W_PROD_CAT")
                            {
                                workItems.W_PROD_CAT = childWorkItems.InnerXml;
                                master.W_PROD_CAT = workItems.W_PROD_CAT;
                            }
                            if (childWorkItems.Name.ToUpper().Trim() == "W_PROD_SUB_CAT")
                            {
                                workItems.W_PROD_SUB_CAT = childWorkItems.InnerXml;
                                master.W_PROD_SUB_CAT = workItems.W_PROD_SUB_CAT;
                            }
                            if (childWorkItems.Name.ToUpper().Trim() == "W_VALUE")
                            {
                                workItems.W_VALUE = childWorkItems.InnerXml;
                                master.W_VALUE = (workItems.W_VALUE == String.Empty ? (decimal?)null : Convert.ToDecimal(workItems.W_VALUE));
                            }

                            if (childWorkItems.Name.ToUpper().Trim() == "W_BIDVALIDITY")
                            {
                                workItems.W_BIDVALIDITY = childWorkItems.InnerXml;
                                master.W_BIDVALIDITY = (workItems.W_BIDVALIDITY == String.Empty ? (int?)null : Convert.ToInt16(workItems.W_BIDVALIDITY));
                            }
                            if (childWorkItems.Name.ToUpper().Trim() == "W_LOCATION")
                            {
                                workItems.W_LOCATION = childWorkItems.InnerXml;
                                master.W_LOCATION = workItems.W_LOCATION;
                            }

                            if (childWorkItems.Name.ToUpper().Trim() == "W_PINCODE")
                            {
                                workItems.W_PINCODE = Convert.ToInt32(childWorkItems.InnerXml);
                                master.W_PINCODE = Convert.ToString(workItems.W_PINCODE);
                            }

                            if (childWorkItems.Name.ToUpper().Trim() == "W_PREBID_MEET_PLACE")
                            {
                                workItems.W_PREBID_MEET_PLACE = childWorkItems.InnerXml;
                                master.W_PREBID_MEET_PLACE = workItems.W_PREBID_MEET_PLACE;
                            }
                            if (childWorkItems.Name.ToUpper().Trim() == "W_PREBID_ADDRESS")
                            {
                                workItems.W_PREBID_ADDRESS = childWorkItems.InnerXml;
                                master.W_PREBID_ADDRESS = workItems.W_PREBID_ADDRESS;
                            }
                            if (childWorkItems.Name.ToUpper().Trim() == "W_BID_OPENING_PLACE")
                            {
                                workItems.W_BID_OPENING_PLACE = childWorkItems.InnerXml;
                                master.W_BID_OPENING_PLACE = workItems.W_BID_OPENING_PLACE;
                            }
                            if (childWorkItems.Name.ToUpper().Trim() == "W_INVITING_OFFICER")
                            {
                                workItems.W_INVITING_OFFICER = childWorkItems.InnerXml;
                                master.W_INVITING_OFFICER = workItems.W_INVITING_OFFICER;
                            }
                            if (childWorkItems.Name.ToUpper().Trim() == "W_INVITING_OFF_ADDRESS")
                            {
                                workItems.W_INVITING_OFF_ADDRESS = childWorkItems.InnerXml;
                                master.W_INVITING_OFF_ADDRESS = workItems.W_INVITING_OFF_ADDRESS;
                            }
                            if (childWorkItems.Name.ToUpper().Trim() == "W_TENDER_FEE")
                            {
                                workItems.W_TENDER_FEE = Convert.ToDecimal(childWorkItems.InnerXml);
                                master.W_TENDER_FEE = workItems.W_TENDER_FEE;
                            }
                            if (childWorkItems.Name.ToUpper().Trim() == "W_TF_PAYABLE_TO")
                            {
                                workItems.W_TF_PAYABLE_TO = childWorkItems.InnerXml;
                                master.W_TF_PAYABLE_TO = workItems.W_TF_PAYABLE_TO;
                            }
                            if (childWorkItems.Name.ToUpper().Trim() == "W_TF_PAYABLE_AT")
                            {
                                workItems.W_TF_PAYABLE_AT = childWorkItems.InnerXml;
                                master.W_TF_PAYABLE_AT = workItems.W_TF_PAYABLE_AT;
                            }
                            if (childWorkItems.Name.ToUpper().Trim() == "W_EMD_FEE")
                            {
                                workItems.W_EMD_FEE = Convert.ToDecimal(childWorkItems.InnerXml);
                                master.W_EMD_FEE = workItems.W_EMD_FEE;
                            }
                            if (childWorkItems.Name.ToUpper().Trim() == "W_EMD_PAYABLE_TO")
                            {
                                workItems.W_EMD_PAYABLE_TO = childWorkItems.InnerXml;
                                master.W_EMD_PAYABLE_TO = workItems.W_EMD_PAYABLE_TO;
                            }
                            if (childWorkItems.Name.ToUpper().Trim() == "W_EMD_PAYABLE_AT")
                            {
                                workItems.W_EMD_PAYABLE_AT = childWorkItems.InnerXml;
                                master.W_EMD_PAYABLE_AT = workItems.W_EMD_PAYABLE_AT;
                            }
                            if (childWorkItems.Name.ToUpper().Trim() == "W_PUB_DATE")
                            {
                                workItems.W_PUB_DATE = (childWorkItems.InnerXml);
                                master.W_PUB_DATE = (workItems.W_PUB_DATE == String.Empty ? (DateTime?)null : Convert.ToDateTime(workItems.W_PUB_DATE));
                            }
                            if (childWorkItems.Name.ToUpper().Trim() == "W_DOC_START_DATE")
                            {
                                workItems.W_DOC_START_DATE = childWorkItems.InnerXml;
                                master.W_DOC_START_DATE = (workItems.W_DOC_START_DATE == String.Empty ? (DateTime?)null : Convert.ToDateTime(workItems.W_DOC_START_DATE));
                            }

                            if (childWorkItems.Name.ToUpper().Trim() == "W_DOC_END_DATE")
                            {
                                workItems.W_DOC_END_DATE = childWorkItems.InnerXml;
                                master.W_DOC_END_DATE = (workItems.W_DOC_END_DATE == String.Empty ? (DateTime?)null : Convert.ToDateTime(workItems.W_DOC_END_DATE));
                            }
                            if (childWorkItems.Name.ToUpper().Trim() == "W_SEEK_CLAR_START_DATE")
                            {
                                workItems.W_SEEK_CLAR_START_DATE = childWorkItems.InnerXml;
                                master.W_SEEK_CLAR_START_DATE = (workItems.W_SEEK_CLAR_START_DATE == String.Empty ? (DateTime?)null : Convert.ToDateTime(workItems.W_SEEK_CLAR_START_DATE));
                            }
                            if (childWorkItems.Name.ToUpper().Trim() == "W_SEEK_CLAR_END_DATE")
                            {
                                workItems.W_SEEK_CLAR_END_DATE = childWorkItems.InnerXml;
                                master.W_SEEK_CLAR_END_DATE = (workItems.W_SEEK_CLAR_END_DATE == String.Empty ? (DateTime?)null : Convert.ToDateTime(workItems.W_SEEK_CLAR_END_DATE));
                            }
                            if (childWorkItems.Name.ToUpper().Trim() == "W_PREBID_DATE")
                            {
                                workItems.W_PREBID_DATE = childWorkItems.InnerXml;
                                master.W_PREBID_DATE = (workItems.W_PREBID_DATE == String.Empty ? (DateTime?)null : Convert.ToDateTime(workItems.W_PREBID_DATE));
                            }
                            if (childWorkItems.Name.ToUpper().Trim() == "W_BIDSUB_START_DATE")
                            {
                                workItems.W_BIDSUB_START_DATE = childWorkItems.InnerXml;
                                master.W_BIDSUB_START_DATE = (workItems.W_BIDSUB_START_DATE == String.Empty ? (DateTime?)null : Convert.ToDateTime(workItems.W_BIDSUB_START_DATE));
                            }
                            if (childWorkItems.Name.ToUpper().Trim() == "W_BIDSUB_END_DATE")
                            {
                                workItems.W_BIDSUB_END_DATE = childWorkItems.InnerXml;
                                master.W_BIDSUB_END_DATE = (workItems.W_BIDSUB_END_DATE == String.Empty ? (DateTime?)null : Convert.ToDateTime(workItems.W_BIDSUB_END_DATE));
                            }
                            if (childWorkItems.Name.ToUpper().Trim() == "W_BID_OPEN_DATE")
                            {
                                workItems.W_BID_OPEN_DATE = childWorkItems.InnerXml;
                                master.W_BID_OPEN_DATE = (workItems.W_BID_OPEN_DATE == String.Empty ? (DateTime?)null : Convert.ToDateTime(workItems.W_BID_OPEN_DATE));
                            }
                            if (childWorkItems.Name.ToUpper().Trim() == "W_FIN_BID_OPEN_DATE")
                            {
                                workItems.W_FIN_BID_OPEN_DATE = childWorkItems.InnerXml;
                                master.W_FIN_BID_OPEN_DATE = (workItems.W_FIN_BID_OPEN_DATE == String.Empty ? (DateTime?)null : Convert.ToDateTime(workItems.W_FIN_BID_OPEN_DATE));
                            }

                            if (childWorkItems.Name.ToUpper().Trim() == "W_BID_OPENERS")
                            {
                                workItems.W_BID_OPENERS = childWorkItems.InnerXml;
                                master.W_BID_OPENERS = workItems.W_BID_OPENERS;
                            }
                            if (childWorkItems.Name.ToUpper().Trim() == "W_RETURN_URL")
                            {
                                workItems.W_RETURN_URL = childWorkItems.InnerXml;
                                master.W_RETURN_URL = workItems.W_RETURN_URL;
                            }
                            if (childWorkItems.Name.ToUpper().Trim() == "W_NO_OF_BIDS")
                            {
                                workItems.W_NO_OF_BIDS = childWorkItems.InnerXml;
                                master.W_NO_OF_BIDS = (workItems.W_NO_OF_BIDS == String.Empty ? (int?)null : Convert.ToInt32(workItems.W_NO_OF_BIDS));
                            }

                        }

                    }
                    dbContext.OMMAS_GEPNIC_TENDER_WORKITEM_DETAILS.Add(master);
                    dbContext.SaveChanges();


                    #endregion

                    #region Corrigendum Details
                    foreach (XmlNode nodeCorrigendum in CorrigendumNodes)
                    {
                        foreach (XmlNode childCorrigendum in nodeCorrigendum.ChildNodes)
                        {

                            if (childCorrigendum.Name == "CORR_ID")
                            {
                                Corrigendum corrItems = new Corrigendum();
                                OMMAS_GEPNIC_TENDER_CORRIGENDUM_DETAILS masterCorrigendum = new OMMAS_GEPNIC_TENDER_CORRIGENDUM_DETAILS();
                                masterCorrigendum.TENDER_ID = master.TENDER_ID;
                                int corrigendumID = dbContext.OMMAS_GEPNIC_TENDER_CORRIGENDUM_DETAILS.Max(cp => (Int32?)cp.CORR_ID) == null ? 1 : (Int32)dbContext.OMMAS_GEPNIC_TENDER_CORRIGENDUM_DETAILS.Max(cp => (Int32?)cp.CORR_ID) + 1;
                                masterCorrigendum.CORR_ID = corrigendumID;
                                masterCorrigendum.GENERATED_DATE = System.DateTime.Now;
                                foreach (XmlNode childCORRID in childCorrigendum.ChildNodes)
                                {

                                    if (childCORRID.Name.ToUpper().Trim() == "CORR_NAME")
                                    {
                                        corrItems.CORR_NAME = childCORRID.InnerXml;
                                        masterCorrigendum.CORR_NAME = corrItems.CORR_NAME;
                                    }

                                    if (childCORRID.Name.ToUpper().Trim() == "CORR_TYPE")
                                    {
                                        corrItems.CORR_TYPE = childCORRID.InnerXml;
                                        masterCorrigendum.CORR_TYPE = corrItems.CORR_TYPE;
                                    }
                                    if (childCORRID.Name.ToUpper().Trim() == "CORR_PUB_DATE")
                                    {
                                        corrItems.CORR_PUB_DATE = Convert.ToDateTime(childCORRID.InnerXml);
                                        masterCorrigendum.CORR_PUB_DATE = corrItems.CORR_PUB_DATE;
                                    }

                                    if (childCORRID.Name.ToUpper().Trim() == "CORR_PUB_BY")
                                    {
                                        corrItems.CORR_PUB_BY = childCORRID.InnerXml;
                                        masterCorrigendum.CORR_PUB_BY = corrItems.CORR_PUB_BY;
                                    }
                                }
                                dbContext.OMMAS_GEPNIC_TENDER_CORRIGENDUM_DETAILS.Add(masterCorrigendum);
                                dbContext.SaveChanges();

                            }

                        }
                    }
                    #endregion

                    #region Bidders Details
                    foreach (XmlNode nodeBidders in biddersNodes)
                    {
                        foreach (XmlNode childBidders in nodeBidders.ChildNodes)
                        {
                            if (childBidders.Name == "BID_INFO")
                            {
                                Bidders bidtems = new Bidders();
                                OMMAS_GEPNIC_TENDER_BIDDER_DETAILS masterBidder = new OMMAS_GEPNIC_TENDER_BIDDER_DETAILS();
                                masterBidder.TENDER_ID = master.TENDER_ID;
                                int bidID = dbContext.OMMAS_GEPNIC_TENDER_BIDDER_DETAILS.Max(cp => (Int32?)cp.BID_ID) == null ? 1 : (Int32)dbContext.OMMAS_GEPNIC_TENDER_BIDDER_DETAILS.Max(cp => (Int32?)cp.BID_ID) + 1;
                                masterBidder.BID_ID = bidID;
                                masterBidder.GENERATED_DATE = System.DateTime.Now;
                                foreach (XmlNode childBidInfo in childBidders.ChildNodes)
                                {
                                    if (childBidInfo.Name.ToUpper().Trim() == "BID_ID")
                                    {
                                        bidtems.BID_ID = Convert.ToInt32(childBidInfo.InnerXml);
                                        masterBidder.GEPNIC_BID_ID = bidtems.BID_ID;
                                    }

                                    if (childBidInfo.Name.ToUpper().Trim() == "BIDDER_NAME")
                                    {
                                        bidtems.BIDDER_NAME = childBidInfo.InnerXml.ToString().Trim();
                                        masterBidder.BIDDER_NAME = bidtems.BIDDER_NAME;
                                    }

                                    if (childBidInfo.Name.ToUpper().Trim() == "BID_PLACED_DATE")
                                    {
                                        bidtems.BID_PLACED_DATE = Convert.ToDateTime(childBidInfo.InnerXml.ToString().Trim());
                                        masterBidder.BID_PLACED_DATE = bidtems.BID_PLACED_DATE;
                                    }

                                    if (childBidInfo.Name.ToUpper().Trim() == "BID_IP_ADDRESS")
                                    {
                                        bidtems.BID_IP_ADDRESS = childBidInfo.InnerXml.ToString().Trim();
                                        masterBidder.BID_IP_ADDRESS = bidtems.BID_IP_ADDRESS;
                                    }
                                }
                                dbContext.OMMAS_GEPNIC_TENDER_BIDDER_DETAILS.Add(masterBidder);
                                dbContext.SaveChanges();
                            }

                        }
                    }
                    #endregion

                    #region Tech Open Details
                    foreach (XmlNode nodeTechOpen in techOpenNodes)
                    {
                        foreach (XmlNode childTechOpen in nodeTechOpen.ChildNodes)
                        {
                            if (childTechOpen.Name == "BID_INFO")
                            {
                                BIDOPEN bidTechtems = new BIDOPEN();

                                OMMAS_GEPNIC_TENDER_BID_OPENEVAL_DETAILS masterBidOpen = new OMMAS_GEPNIC_TENDER_BID_OPENEVAL_DETAILS();
                                masterBidOpen.TENDER_ID = master.TENDER_ID;
                                int bidIDOpen = dbContext.OMMAS_GEPNIC_TENDER_BID_OPENEVAL_DETAILS.Max(cp => (Int32?)cp.DETAIL_ID) == null ? 1 : (Int32)dbContext.OMMAS_GEPNIC_TENDER_BID_OPENEVAL_DETAILS.Max(cp => (Int32?)cp.DETAIL_ID) + 1;
                                masterBidOpen.DETAIL_ID = bidIDOpen;
                                masterBidOpen.BID_OPEN_EVAL_TYPE = "TO";
                                masterBidOpen.GENERATED_DATE = System.DateTime.Now;
                                foreach (XmlNode childTechBidInfo in childTechOpen.ChildNodes)
                                {
                                    if (childTechBidInfo.Name.ToUpper().Trim() == "BID_ID")
                                    {
                                        bidTechtems.BID_ID = Convert.ToInt32(childTechBidInfo.InnerXml);
                                        masterBidOpen.GEPNIC_BID_ID = bidTechtems.BID_ID;
                                    }

                                    if (childTechBidInfo.Name.ToUpper().Trim() == "BIDDER_NAME")
                                    {
                                        bidTechtems.BIDDER_NAME = childTechBidInfo.InnerXml.ToString().Trim();
                                        masterBidOpen.BIDDER_NAME = bidTechtems.BIDDER_NAME;
                                    }

                                    if (childTechBidInfo.Name.ToUpper().Trim() == "BID_OPENED_DATE")
                                    {
                                        bidTechtems.BID_OPENED_DATE = Convert.ToDateTime(childTechBidInfo.InnerXml.ToString().Trim());
                                        masterBidOpen.BID_OPENED_DATE = bidTechtems.BID_OPENED_DATE;
                                    }

                                    if (childTechBidInfo.Name.ToUpper().Trim() == "BID_OPENED_BY")
                                    {
                                        bidTechtems.BID_OPENED_BY = childTechBidInfo.InnerXml.ToString().Trim();
                                        masterBidOpen.BID_OPENED_BY = bidTechtems.BID_OPENED_BY;
                                    }
                                }
                                dbContext.OMMAS_GEPNIC_TENDER_BID_OPENEVAL_DETAILS.Add(masterBidOpen);
                                dbContext.SaveChanges();
                            }

                        }
                    }

                    #endregion

                    #region Tech Eval Details
                    foreach (XmlNode nodeTechEval in techEvalNodes)
                    {
                        foreach (XmlNode childTechEval in nodeTechEval.ChildNodes)
                        {
                            if (childTechEval.Name == "BID_INFO")
                            {
                                BIDEVAL bidTechEval = new BIDEVAL();
                                OMMAS_GEPNIC_TENDER_BID_OPENEVAL_DETAILS masterBidEval = new OMMAS_GEPNIC_TENDER_BID_OPENEVAL_DETAILS();
                                masterBidEval.TENDER_ID = master.TENDER_ID;
                                int bidIDOpen = dbContext.OMMAS_GEPNIC_TENDER_BID_OPENEVAL_DETAILS.Max(cp => (Int32?)cp.DETAIL_ID) == null ? 1 : (Int32)dbContext.OMMAS_GEPNIC_TENDER_BID_OPENEVAL_DETAILS.Max(cp => (Int32?)cp.DETAIL_ID) + 1;
                                masterBidEval.DETAIL_ID = bidIDOpen;
                                masterBidEval.BID_OPEN_EVAL_TYPE = "TE";
                                masterBidEval.GENERATED_DATE = System.DateTime.Now;
                                foreach (XmlNode childTechBidInfo in childTechEval.ChildNodes)
                                {
                                    if (childTechBidInfo.Name.ToUpper().Trim() == "BID_ID")
                                    {
                                        bidTechEval.BID_ID = Convert.ToInt32(childTechBidInfo.InnerXml);
                                        masterBidEval.GEPNIC_BID_ID = bidTechEval.BID_ID;
                                    }

                                    if (childTechBidInfo.Name.ToUpper().Trim() == "BIDDER_NAME")
                                    {
                                        bidTechEval.BIDDER_NAME = childTechBidInfo.InnerXml.ToString().Trim();
                                        masterBidEval.BIDDER_NAME = bidTechEval.BIDDER_NAME;
                                    }

                                    if (childTechBidInfo.Name.ToUpper().Trim() == "BID_EVAL_DATE")
                                    {
                                        bidTechEval.BID_EVAL_DATE = Convert.ToDateTime(childTechBidInfo.InnerXml.ToString().Trim());
                                        masterBidEval.BID_OPENED_DATE = bidTechEval.BID_EVAL_DATE;
                                    }

                                    if (childTechBidInfo.Name.ToUpper().Trim() == "BID_EVAL_BY")
                                    {
                                        bidTechEval.BID_EVAL_BY = childTechBidInfo.InnerXml.ToString().Trim();
                                        masterBidEval.BID_OPENED_BY = bidTechEval.BID_EVAL_BY;
                                    }

                                }
                                dbContext.OMMAS_GEPNIC_TENDER_BID_OPENEVAL_DETAILS.Add(masterBidEval);
                                dbContext.SaveChanges();
                            }

                        }
                    }
                    #endregion

                    #region FIN Open Details
                    foreach (XmlNode nodeFinOpen in finOpenNodes)
                    {
                        foreach (XmlNode childTechOpen in nodeFinOpen.ChildNodes)
                        {
                            if (childTechOpen.Name == "BID_INFO")
                            {
                                BIDOPEN bidFinitems = new BIDOPEN();
                                OMMAS_GEPNIC_TENDER_BID_OPENEVAL_DETAILS masterBidOpen = new OMMAS_GEPNIC_TENDER_BID_OPENEVAL_DETAILS();
                                masterBidOpen.TENDER_ID = master.TENDER_ID;
                                int bidIDOpen = dbContext.OMMAS_GEPNIC_TENDER_BID_OPENEVAL_DETAILS.Max(cp => (Int32?)cp.DETAIL_ID) == null ? 1 : (Int32)dbContext.OMMAS_GEPNIC_TENDER_BID_OPENEVAL_DETAILS.Max(cp => (Int32?)cp.DETAIL_ID) + 1;
                                masterBidOpen.DETAIL_ID = bidIDOpen;
                                masterBidOpen.BID_OPEN_EVAL_TYPE = "FO";
                                masterBidOpen.GENERATED_DATE = System.DateTime.Now;
                                foreach (XmlNode childTechBidInfo in childTechOpen.ChildNodes)
                                {
                                    if (childTechBidInfo.Name.ToUpper().Trim() == "BID_ID")
                                    {
                                        bidFinitems.BID_ID = Convert.ToInt32(childTechBidInfo.InnerXml);
                                        masterBidOpen.GEPNIC_BID_ID = bidFinitems.BID_ID;
                                    }

                                    if (childTechBidInfo.Name.ToUpper().Trim() == "BIDDER_NAME")
                                    {
                                        bidFinitems.BIDDER_NAME = childTechBidInfo.InnerXml.ToString().Trim();
                                        masterBidOpen.BIDDER_NAME = bidFinitems.BIDDER_NAME;
                                    }

                                    if (childTechBidInfo.Name.ToUpper().Trim() == "BID_OPENED_DATE")
                                    {
                                        bidFinitems.BID_OPENED_DATE = Convert.ToDateTime(childTechBidInfo.InnerXml.ToString().Trim());
                                        masterBidOpen.BID_OPENED_DATE = bidFinitems.BID_OPENED_DATE;
                                    }

                                    if (childTechBidInfo.Name.ToUpper().Trim() == "BID_OPENED_BY")
                                    {
                                        bidFinitems.BID_OPENED_BY = childTechBidInfo.InnerXml.ToString().Trim();
                                        masterBidOpen.BID_OPENED_BY = bidFinitems.BID_OPENED_BY;
                                    }

                                }
                                dbContext.OMMAS_GEPNIC_TENDER_BID_OPENEVAL_DETAILS.Add(masterBidOpen);
                                dbContext.SaveChanges();
                            }

                        }
                    }
                    #endregion

                    #region FIN Eval Details
                    foreach (XmlNode nodeFinEval in finEvalNodes)
                    {
                        foreach (XmlNode childTechEval in nodeFinEval.ChildNodes)
                        {
                            if (childTechEval.Name == "BID_INFO")
                            {
                                BIDEVAL bidFinEval = new BIDEVAL();
                                OMMAS_GEPNIC_TENDER_BID_OPENEVAL_DETAILS masterBidEval = new OMMAS_GEPNIC_TENDER_BID_OPENEVAL_DETAILS();
                                masterBidEval.TENDER_ID = master.TENDER_ID;

                                int bidIDOpen = dbContext.OMMAS_GEPNIC_TENDER_BID_OPENEVAL_DETAILS.Max(cp => (Int32?)cp.DETAIL_ID) == null ? 1 : (Int32)dbContext.OMMAS_GEPNIC_TENDER_BID_OPENEVAL_DETAILS.Max(cp => (Int32?)cp.DETAIL_ID) + 1;
                                masterBidEval.DETAIL_ID = bidIDOpen;
                                masterBidEval.BID_OPEN_EVAL_TYPE = "FE";
                                masterBidEval.GENERATED_DATE = System.DateTime.Now;
                                foreach (XmlNode childTechBidInfo in childTechEval.ChildNodes)
                                {
                                    if (childTechBidInfo.Name.ToUpper().Trim() == "BID_ID")
                                    {
                                        bidFinEval.BID_ID = Convert.ToInt32(childTechBidInfo.InnerXml);
                                        masterBidEval.GEPNIC_BID_ID = bidFinEval.BID_ID;
                                    }

                                    if (childTechBidInfo.Name.ToUpper().Trim() == "BIDDER_NAME")
                                    {
                                        bidFinEval.BIDDER_NAME = childTechBidInfo.InnerXml.ToString().Trim();
                                        masterBidEval.BIDDER_NAME = bidFinEval.BIDDER_NAME;
                                    }

                                    if (childTechBidInfo.Name.ToUpper().Trim() == "BID_EVAL_DATE")
                                    {
                                        bidFinEval.BID_EVAL_DATE = Convert.ToDateTime(childTechBidInfo.InnerXml.ToString().Trim());
                                        masterBidEval.BID_OPENED_DATE = bidFinEval.BID_EVAL_DATE;
                                    }

                                    if (childTechBidInfo.Name.ToUpper().Trim() == "BID_EVAL_BY")
                                    {
                                        bidFinEval.BID_EVAL_BY = childTechBidInfo.InnerXml.ToString().Trim();
                                        masterBidEval.BID_OPENED_BY = bidFinEval.BID_EVAL_BY;
                                    }

                                }
                                dbContext.OMMAS_GEPNIC_TENDER_BID_OPENEVAL_DETAILS.Add(masterBidEval);
                                dbContext.SaveChanges();
                            }

                        }
                    }
                    #endregion


                    #region AOC Details
                    foreach (XmlNode nodeFinEval in aocNodes)
                    {
                        foreach (XmlNode childTechEval in nodeFinEval.ChildNodes)
                        {
                            if (childTechEval.Name == "BID_INFO")
                            {
                                AOC aocObject = new AOC();
                                OMMAS_GEPNIC_AOC_DETAILS aocMaster = new OMMAS_GEPNIC_AOC_DETAILS();
                                aocMaster.TENDER_ID = master.TENDER_ID;

                                int aocID = dbContext.OMMAS_GEPNIC_AOC_DETAILS.Max(cp => (Int32?)cp.AOC_ID) == null ? 1 : (Int32)dbContext.OMMAS_GEPNIC_AOC_DETAILS.Max(cp => (Int32?)cp.AOC_ID) + 1;
                                aocMaster.AOC_ID = aocID;
                                aocMaster.GENERATED_DATE = System.DateTime.Now;

                                foreach (XmlNode childTechBidInfo in childTechEval.ChildNodes)
                                {
                                    if (childTechBidInfo.Name.ToUpper().Trim() == "BID_ID")
                                    {
                                        aocObject.GEPNIC_BID_ID = Convert.ToInt32(childTechBidInfo.InnerXml);
                                        aocMaster.GEPNIC_BID_ID = aocObject.GEPNIC_BID_ID;
                                    }

                                    if (childTechBidInfo.Name.ToUpper().Trim() == "BIDDER_NAME")
                                    {
                                        aocObject.BIDDER_NAME = childTechBidInfo.InnerXml.ToString().Trim();
                                        aocMaster.BIDDER_NAME = aocObject.BIDDER_NAME;
                                    }

                                    if (childTechBidInfo.Name.ToUpper().Trim() == "CONTRACT_DATE")
                                    {
                                        aocObject.CONTRACT_DATE = Convert.ToDateTime(childTechBidInfo.InnerXml.ToString().Trim());
                                        aocMaster.CONTRACT_DATE = aocObject.CONTRACT_DATE;
                                    }

                                    if (childTechBidInfo.Name.ToUpper().Trim() == "CONTRACT_NO")
                                    {
                                        aocObject.CONTRACT_NO = Convert.ToInt32(childTechBidInfo.InnerXml);
                                        aocMaster.CONTRACT_NUMBER = Convert.ToString(aocObject.CONTRACT_NO);
                                    }
                                    if (childTechBidInfo.Name.ToUpper().Trim() == "AWARDED_VALUE")
                                    {
                                        aocObject.AWARDED_VALUE = Convert.ToDecimal(childTechBidInfo.InnerXml);
                                        aocMaster.AWARDED_VALUE = Convert.ToDecimal(aocObject.AWARDED_VALUE);
                                    }

                                }
                                dbContext.OMMAS_GEPNIC_AOC_DETAILS.Add(aocMaster);
                                dbContext.SaveChanges();
                            }

                        }
                    }
                    #endregion

                 //   ("TN2059");  
                   scope.Complete();
               }
           

                return true;
            }
            catch (TransactionException ex)
            {
                using (System.IO.StreamWriter sw = System.IO.File.AppendText(System.IO.Path.Combine("D:\\ErrorLog\\OMMASErrorLog_" + System.DateTime.Now.Date.ToString("dd_MM_yyyy") + ".txt", "")))
                {
                    sw.WriteLine("Date :" + DateTime.Now.ToString());
                    sw.WriteLine("Method : " + "GepenicDAL.AddDetails");
                    sw.WriteLine("---------------------------------------------------------------------------------------");
                    sw.Close();
                }

                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                message = "Error occurred while processing your request.";
                return false;

            }
            catch (Exception ex)
            {
                using (System.IO.StreamWriter sw = System.IO.File.AppendText(System.IO.Path.Combine("D:\\ErrorLog\\OMMASErrorLog_" + System.DateTime.Now.Date.ToString("dd_MM_yyyy") + ".txt", "")))
                {
                    sw.WriteLine("Date :" + DateTime.Now.ToString());
                    sw.WriteLine("Method : " + "GepenicDAL.AddDetails");
                    sw.WriteLine("---------------------------------------------------------------------------------------");
                    sw.Close();
                }
                
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                message = "Error occurred while processing your request.";
                return false;
            }
            finally
            {
                dbContext.Dispose();
            }
        }

        // GetTenderList


        public Array GetTenderList(int? page, int? rows, string sidx, string sord, out long totalRecords)
        {
            dbContext = new PMGSYEntities();
            try
            {
                //var lstDPRProposals = dbContext.IMS_SANCTIONED_PROJECTS.Where(m => m.IMS_DPR_STATUS == "Y" && m.MAST_DPIU_CODE == PMGSYSession.Current.AdminNdCode).ToList();
                var lstDPRProposals = (from m in dbContext.OMMAS_GEPNIC_TENDER_WORKITEM_DETAILS
                                       where m.T_REF_NO == m.T_REF_NO && m.TENDER_ID == dbContext.OMMAS_GEPNIC_TENDER_WORKITEM_DETAILS.Where(b=>m.T_REF_NO==b.T_REF_NO).Select(a=>a.TENDER_ID).Max()
                                        //(context.Users.Select(u1 => u1.Id).Max())
                                       select new
                                       {
                                           m.WORKITEM_REF_NO,
                                           m.ORG_CHAIN,
                                           m.T_REF_NO,
                                           m.T_TENDER_TYPE,
                                           m.T_FORM_OF_CONTRACT,
                                           m.T_TENDER_CATEGORY,
                                           m.W_TITLE,
                                           m.W_DESC,
                                           m.W_PRE_QUAL,
                                           m.W_PROD_CAT,
                                           m.W_PROD_SUB_CAT,
                                           m.W_VALUE,
                                           m.W_BIDVALIDITY,
                                           m.W_LOCATION,
                                           m.W_PINCODE,
                                           m.W_PREBID_MEET_PLACE,
                                           m.W_PREBID_ADDRESS,
                                           m.W_BID_OPENING_PLACE,
                                           m.W_INVITING_OFFICER,
                                           m.W_INVITING_OFF_ADDRESS,
                                           m.W_TENDER_FEE,
                                           m.W_TF_PAYABLE_TO,
                                           m.W_TF_PAYABLE_AT,
                                           m.W_EMD_FEE,
                                           m.W_EMD_PAYABLE_TO,
                                           m.W_EMD_PAYABLE_AT,
                                           m.W_PUB_DATE,
                                           m.W_DOC_START_DATE,
                                           m.W_DOC_END_DATE,
                                           m.W_SEEK_CLAR_START_DATE,
                                           m.W_SEEK_CLAR_END_DATE,
                                           m.W_PREBID_DATE,
                                           m.W_BIDSUB_START_DATE,
                                           m.W_BIDSUB_END_DATE,
                                           m.W_BID_OPEN_DATE,
                                           m.W_FIN_BID_OPEN_DATE,
                                           m.W_BID_OPENERS,
                                           m.W_NO_OF_BIDS,
                                           m. W_RETURN_URL,
                                           m.TENDER_ID


                                       }).ToList();
                totalRecords = lstDPRProposals.Count();

                if (sidx.Trim() != string.Empty)
                {
                    if (sord.ToString() == "asc")
                    {
                        switch (sidx)
                        {
                            case "WORKITEM_REF_NO":
                                lstDPRProposals = lstDPRProposals.OrderBy(m => m.WORKITEM_REF_NO).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                                break;
                            default:
                                lstDPRProposals = lstDPRProposals.OrderBy(m => m.WORKITEM_REF_NO).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                                break;
                        }
                    }
                    else
                    {
                        switch (sidx)
                        {
                            case "WORKITEM_REF_NO":
                                lstDPRProposals = lstDPRProposals.OrderByDescending(m => m.WORKITEM_REF_NO).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                                break;
                            default:
                                lstDPRProposals = lstDPRProposals.OrderByDescending(m => m.WORKITEM_REF_NO).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                                break;
                        }
                    }
                }
                else
                {
                    lstDPRProposals = lstDPRProposals.OrderBy(m => m.WORKITEM_REF_NO).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                }

                var result = lstDPRProposals.Select(m => new
                {
                    m.WORKITEM_REF_NO,
                    m.ORG_CHAIN,
                    m.T_REF_NO,
                    m.T_TENDER_TYPE,
                    m.T_FORM_OF_CONTRACT,
                    m.T_TENDER_CATEGORY,
                    m.W_TITLE,
                    m.W_DESC,
                    m.W_PRE_QUAL,
                    m.W_PROD_CAT,
                    m.W_PROD_SUB_CAT,
                    m.W_VALUE,
                    m.W_BIDVALIDITY,
                    m.W_LOCATION,
                    m.W_PINCODE,
                    m.W_PREBID_MEET_PLACE,
                    m.W_PREBID_ADDRESS,
                    m.W_BID_OPENING_PLACE,
                    m.W_INVITING_OFFICER,
                    m.W_INVITING_OFF_ADDRESS,
                    m.W_TENDER_FEE,
                    m.W_TF_PAYABLE_TO,
                    m.W_TF_PAYABLE_AT,
                    m.W_EMD_FEE,
                    m.W_EMD_PAYABLE_TO,
                    m.W_EMD_PAYABLE_AT,
                    m.W_PUB_DATE,
                    m.W_DOC_START_DATE,
                    m.W_DOC_END_DATE,
                    m.W_SEEK_CLAR_START_DATE,
                    m.W_SEEK_CLAR_END_DATE,
                    m.W_PREBID_DATE,
                    m.W_BIDSUB_START_DATE,
                    m.W_BIDSUB_END_DATE,
                    m.W_BID_OPEN_DATE,
                    m.W_FIN_BID_OPEN_DATE,
                    m.W_BID_OPENERS,
                    m.W_NO_OF_BIDS,
                    m.W_RETURN_URL,
                    m.TENDER_ID
                }).ToArray();

                return result.Select(m => new
                {
                    cell = new[] 
                    {
                        
                                           m.WORKITEM_REF_NO,
                                           m.ORG_CHAIN.ToString(), 
                                           m.T_REF_NO.ToString(), 
                                           m.T_TENDER_TYPE.ToString(), 
                                           m.T_FORM_OF_CONTRACT.ToString(), 
                                           m.T_TENDER_CATEGORY.ToString(),
                                           m.W_TITLE==null?"-":m.W_TITLE.ToString(),
                                           m.W_DESC==null?"-":m.W_DESC.ToString(), 
                                           m.W_PRE_QUAL==null?"-":m.W_PRE_QUAL.ToString(), 
                                           m.W_PROD_CAT==null?"-":m.W_PROD_CAT.ToString(), 
                                           m.W_PROD_SUB_CAT==null?"-":m.W_PROD_SUB_CAT.ToString(),
                                           m.W_VALUE==null?"-": m.W_VALUE.ToString(),
                                           m.W_BIDVALIDITY==null?"-":m.W_BIDVALIDITY.ToString(),
                                           m.W_LOCATION==null?"-":m.W_LOCATION.ToString(),
                                           m.W_PINCODE==null?"-":m.W_PINCODE.ToString(),
                                           m.W_PREBID_MEET_PLACE==null?"-": m.W_PREBID_MEET_PLACE.ToString(),
                                           m.W_PREBID_ADDRESS==null?"-": m.W_PREBID_ADDRESS.ToString(),
                                           m.W_BID_OPENING_PLACE==null?"-":  m.W_BID_OPENING_PLACE.ToString(),
                                           m.W_INVITING_OFFICER==null?"-":m.W_INVITING_OFFICER.ToString(),
                                           m.W_INVITING_OFF_ADDRESS==null?"-":  m.W_INVITING_OFF_ADDRESS.ToString(),
                                           m.W_TENDER_FEE==null?"-":m.W_TENDER_FEE.ToString(),
                                           m.W_TF_PAYABLE_TO==null?"-":  m.W_TF_PAYABLE_TO.ToString(),
                                           m.W_TF_PAYABLE_AT==null?"-": m.W_TF_PAYABLE_AT.ToString(),
                                           m.W_EMD_FEE==null?"-":m.W_EMD_FEE.ToString(),
                                           m.W_EMD_PAYABLE_TO==null?"-": m.W_EMD_PAYABLE_TO.ToString(),
                                           m.W_EMD_PAYABLE_AT==null?"-": m.W_EMD_PAYABLE_AT.ToString(),
                                           m.W_PUB_DATE==null?"-":m.W_PUB_DATE.ToString(),
                                           m.W_DOC_START_DATE==null?"-": m.W_DOC_START_DATE.ToString(),
                                           m.W_DOC_END_DATE==null?"-": m.W_DOC_END_DATE.ToString(),
                                           m.W_SEEK_CLAR_START_DATE==null?"-": m.W_SEEK_CLAR_START_DATE.ToString(),
                                           m.W_SEEK_CLAR_END_DATE==null?"-":m.W_SEEK_CLAR_END_DATE.ToString(),
                                           m.W_PREBID_DATE==null?"-":m.W_PREBID_DATE.ToString(),
                                           m.W_BIDSUB_START_DATE==null?"-":m.W_BIDSUB_START_DATE.ToString(),
                                           m.W_BIDSUB_END_DATE==null?"-":m.W_BIDSUB_END_DATE.ToString(),
                                           m.W_BID_OPEN_DATE==null?"-":m.W_BID_OPEN_DATE.ToString(),
                                           m.W_FIN_BID_OPEN_DATE==null?"-": m.W_FIN_BID_OPEN_DATE.ToString(),
                                           m.W_BID_OPENERS==null?"-":m.W_BID_OPENERS.ToString(),
                                           m.W_NO_OF_BIDS==null?"-": m.W_NO_OF_BIDS.ToString(),
                                           m. W_RETURN_URL==null?"-":m. W_RETURN_URL.ToString(),
                                           m.TENDER_ID.ToString()
                    }
                }).ToArray();

            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "GetTenderList().DAL");
                totalRecords = 0;
                return null;
            }
            finally
            {
                if (dbContext != null)
                {
                    dbContext.Dispose();
                }
            }
        }


        public Array GetCorrigendumList(int? page, int? rows, string sidx, string sord, out long totalRecords, int TENDER_CODE)
        {
            dbContext = new PMGSYEntities();
            try
            {
               
                var lstDPRProposals = (from m in dbContext.OMMAS_GEPNIC_TENDER_CORRIGENDUM_DETAILS
                                       where m.TENDER_ID == TENDER_CODE
                                       select new
                                       {
                                           m.CORR_NAME,
                                           m.CORR_TYPE,
                                           m.CORR_PUB_DATE,
                                           m.CORR_PUB_BY
                                


                                       }).ToList();
                totalRecords = lstDPRProposals.Count();

                if (sidx.Trim() != string.Empty)
                {
                    if (sord.ToString() == "asc")
                    {
                        switch (sidx)
                        {
                            case "CORR_NAME":
                                lstDPRProposals = lstDPRProposals.OrderBy(m => m.CORR_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                                break;
                            default:
                                lstDPRProposals = lstDPRProposals.OrderBy(m => m.CORR_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                                break;
                        }
                    }
                    else
                    {
                        switch (sidx)
                        {
                            case "CORR_NAME":
                                lstDPRProposals = lstDPRProposals.OrderByDescending(m => m.CORR_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                                break;
                            default:
                                lstDPRProposals = lstDPRProposals.OrderByDescending(m => m.CORR_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                                break;
                        }
                    }
                }
                else
                {
                    lstDPRProposals = lstDPRProposals.OrderBy(m => m.CORR_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                }

                var result = lstDPRProposals.Select(m => new
                {
                    m.CORR_NAME,
                    m.CORR_TYPE,
                    m.CORR_PUB_DATE,
                    m.CORR_PUB_BY
      
                }).ToArray();

                return result.Select(m => new
                {
                    cell = new[] 
                    {
                                           m.CORR_NAME,
                                           m.CORR_TYPE,
                                           m.CORR_PUB_DATE.ToString(),
                                           m.CORR_PUB_BY
                             
                    }
                }).ToArray();

            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "GetCorrigendumList().DAL");
                totalRecords = 0;
                return null;
            }
            finally
            {
                if (dbContext != null)
                {
                    dbContext.Dispose();
                }
            }
        }

        public Array GetBidderList(int? page, int? rows, string sidx, string sord, out long totalRecords, int TENDER_CODE)
        {
            dbContext = new PMGSYEntities();
            try
            {

                var lstDPRProposals = (from m in dbContext.OMMAS_GEPNIC_TENDER_BIDDER_DETAILS
                                       where m.TENDER_ID == TENDER_CODE
                                       select new
                                       {
                                           m.GEPNIC_BID_ID,
                                           m.BIDDER_NAME,
                                           m.BID_PLACED_DATE,
                                           m.BID_IP_ADDRESS



                                       }).ToList();
                totalRecords = lstDPRProposals.Count();

                if (sidx.Trim() != string.Empty)
                {
                    if (sord.ToString() == "asc")
                    {
                        switch (sidx)
                        {
                            case "GEPNIC_BID_ID":
                                lstDPRProposals = lstDPRProposals.OrderBy(m => m.GEPNIC_BID_ID).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                                break;
                            default:
                                lstDPRProposals = lstDPRProposals.OrderBy(m => m.GEPNIC_BID_ID).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                                break;
                        }
                    }
                    else
                    {
                        switch (sidx)
                        {
                            case "GEPNIC_BID_ID":
                                lstDPRProposals = lstDPRProposals.OrderByDescending(m => m.GEPNIC_BID_ID).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                                break;
                            default:
                                lstDPRProposals = lstDPRProposals.OrderByDescending(m => m.GEPNIC_BID_ID).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                                break;
                        }
                    }
                }
                else
                {
                    lstDPRProposals = lstDPRProposals.OrderBy(m => m.GEPNIC_BID_ID).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                }

                var result = lstDPRProposals.Select(m => new
                {
                    m.GEPNIC_BID_ID,
                    m.BIDDER_NAME,
                    m.BID_PLACED_DATE,
                    m.BID_IP_ADDRESS

                }).ToArray();

                return result.Select(m => new
                {
                    cell = new[] 
                    {
                                           m.GEPNIC_BID_ID.ToString(),
                                           m.BIDDER_NAME,
                                           m.BID_PLACED_DATE.ToString(),
                                           m.BID_IP_ADDRESS
                             
                    }
                }).ToArray();

            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "GetBidderList().DAL");
                totalRecords = 0;
                return null;
            }
            finally
            {
                if (dbContext != null)
                {
                    dbContext.Dispose();
                }
            }
        }

        //
        public Array AOCListDAL(int? page, int? rows, string sidx, string sord, out long totalRecords, int TENDER_CODE)
        {
            dbContext = new PMGSYEntities();
            try
            {

                var lstDPRProposals = (from m in dbContext.OMMAS_GEPNIC_AOC_DETAILS
                                       where m.TENDER_ID == TENDER_CODE
                                       select new
                                       {
                                           m.GEPNIC_BID_ID,
                                           m.BIDDER_NAME,
                                           m.CONTRACT_DATE,
                                           m.CONTRACT_NUMBER,
                                           m.AWARDED_VALUE

                                       }).ToList();
                totalRecords = lstDPRProposals.Count();

                if (sidx.Trim() != string.Empty)
                {
                    if (sord.ToString() == "asc")
                    {
                        switch (sidx)
                        {
                            case "GEPNIC_BID_ID":
                                lstDPRProposals = lstDPRProposals.OrderBy(m => m.GEPNIC_BID_ID).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                                break;
                            default:
                                lstDPRProposals = lstDPRProposals.OrderBy(m => m.GEPNIC_BID_ID).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                                break;
                        }
                    }
                    else
                    {
                        switch (sidx)
                        {
                            case "GEPNIC_BID_ID":
                                lstDPRProposals = lstDPRProposals.OrderByDescending(m => m.GEPNIC_BID_ID).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                                break;
                            default:
                                lstDPRProposals = lstDPRProposals.OrderByDescending(m => m.GEPNIC_BID_ID).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                                break;
                        }
                    }
                }
                else
                {
                    lstDPRProposals = lstDPRProposals.OrderBy(m => m.GEPNIC_BID_ID).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                }

                var result = lstDPRProposals.Select(m => new
                {
                    m.GEPNIC_BID_ID,
                    m.BIDDER_NAME,
                    m.CONTRACT_DATE,
                    m.CONTRACT_NUMBER,
                    m.AWARDED_VALUE

                }).ToArray();

                return result.Select(m => new
                {
                    cell = new[] 
                    {
                                           m.GEPNIC_BID_ID.ToString(),
                                           m.BIDDER_NAME,
                                           m.CONTRACT_DATE.ToString(),
                                           m.CONTRACT_NUMBER,
                                            m.AWARDED_VALUE.ToString()
                              
                    }
                }).ToArray();

            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "AOCListDAL().DAL");
                totalRecords = 0;
                return null;
            }
            finally
            {
                if (dbContext != null)
                {
                    dbContext.Dispose();
                }
            }
        }

        public Array GetOpenEvalDAL(int? page, int? rows, string sidx, string sord, out long totalRecords, int TENDER_CODE)
        {
            dbContext = new PMGSYEntities();
            try
            {

                var lstDPRProposals = (from m in dbContext.OMMAS_GEPNIC_TENDER_BID_OPENEVAL_DETAILS
                                       where m.TENDER_ID == TENDER_CODE
                                       select new
                                       {
                                           m.GEPNIC_BID_ID,
                                           m.BIDDER_NAME,
                                           m.BID_OPENED_DATE,
                                           m.BID_OPENED_BY,
                                           m.BID_OPEN_EVAL_TYPE



                                       }).ToList();
                totalRecords = lstDPRProposals.Count();

                if (sidx.Trim() != string.Empty)
                {
                    if (sord.ToString() == "asc")
                    {
                        switch (sidx)
                        {
                            case "GEPNIC_BID_ID":
                                lstDPRProposals = lstDPRProposals.OrderBy(m => m.GEPNIC_BID_ID).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                                break;
                            default:
                                lstDPRProposals = lstDPRProposals.OrderBy(m => m.GEPNIC_BID_ID).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                                break;
                        }
                    }
                    else
                    {
                        switch (sidx)
                        {
                            case "GEPNIC_BID_ID":
                                lstDPRProposals = lstDPRProposals.OrderByDescending(m => m.GEPNIC_BID_ID).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                                break;
                            default:
                                lstDPRProposals = lstDPRProposals.OrderByDescending(m => m.GEPNIC_BID_ID).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                                break;
                        }
                    }
                }
                else
                {
                    lstDPRProposals = lstDPRProposals.OrderBy(m => m.GEPNIC_BID_ID).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                }

                var result = lstDPRProposals.Select(m => new
                {

                    m.GEPNIC_BID_ID,
                    m.BIDDER_NAME,
                    m.BID_OPENED_DATE,
                    m.BID_OPENED_BY,
                    m.BID_OPEN_EVAL_TYPE

                }).ToArray();

                return result.Select(m => new
                {
                    cell = new[] 
                    {
                    m.GEPNIC_BID_ID.ToString(),
                    m.BIDDER_NAME,
                    m.BID_OPENED_DATE.ToString(),
                    m.BID_OPENED_BY,
                    m.BID_OPEN_EVAL_TYPE=="TO"?"Technical Open":(  m.BID_OPEN_EVAL_TYPE=="TE"?"Technical Eval":(  m.BID_OPEN_EVAL_TYPE=="FO"?"Financial Open":"Financial Eval"))
                             
                    }
                }).ToArray();

            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "GetBidderList().DAL");
                totalRecords = 0;
                return null;
            }
            finally
            {
                if (dbContext != null)
                {
                    dbContext.Dispose();
                }
            }
        }


        #region Tender Info By Publish Date

        public bool AddDetailsTender(XMLCreation model, ref string message, XmlDocument xmlDoc, XmlNodeList workItemNodes)
        {
            dbContext = new PMGSYEntities();
            try
            {
                using (TransactionScope scope = new TransactionScope())
                {

                    #region Tech Eval Details
                    foreach (XmlNode nodeTechEval in workItemNodes)
                    {
                        foreach (XmlNode childTechEval in nodeTechEval.ChildNodes)
                        {
                            if (childTechEval.Name == "TENDERS")
                            {
                                TenderXMLByPublishdate localModel = new TenderXMLByPublishdate();

                                OMMAS_GEPNIC_TENDERINFO_BY_PDATE master = new OMMAS_GEPNIC_TENDERINFO_BY_PDATE();

                                int tenderPrimaryKey = dbContext.OMMAS_GEPNIC_TENDERINFO_BY_PDATE.Max(cp => (Int32?)cp.TINFOID) == null ? 1 : (Int32)dbContext.OMMAS_GEPNIC_TENDERINFO_BY_PDATE.Max(cp => (Int32?)cp.TINFOID) + 1;
                                localModel.TINFOID = tenderPrimaryKey;

                                master.GENERATED_DATE = System.DateTime.Now;
                                master.TINFOID = localModel.TINFOID;


                                foreach (XmlNode childTechBidInfo in childTechEval.ChildNodes)
                                {
                                    if (childTechBidInfo.Name.ToUpper().Trim() == "T_ID")
                                    {
                                        localModel.T_ID = childTechBidInfo.InnerXml.ToString().Trim();
                                        master.T_ID = localModel.T_ID;
                                    }

                                    if (childTechBidInfo.Name.ToUpper().Trim() == "T_REF_NO")
                                    {
                                        localModel.T_REF_NO = childTechBidInfo.InnerXml.ToString().Trim();
                                        master.T_REF_NO = localModel.T_REF_NO;

                                        if (dbContext.OMMAS_GEPNIC_TENDERINFO_BY_PDATE.Where(m => m.T_REF_NO.ToUpper() == localModel.T_REF_NO.ToUpper()).Any())
                                        {
                                            message = "Tender Referance Number is already Exists. Duplicate Entry.";
                                            return false;
                                        }


                                    }

                                    if (childTechBidInfo.Name.ToUpper().Trim() == "T_TITLE")
                                    {
                                        localModel.T_TITLE = childTechBidInfo.InnerXml.ToString().Trim();
                                        master.T_TITLE = localModel.T_TITLE;
                                    }


                                    // Added on 05 Oct 2018
                                    if (childTechBidInfo.Name.ToUpper().Trim() == "T_DESC")
                                    {
                                        localModel.T_DESC = childTechBidInfo.InnerXml.ToString().Trim();
                                        master.T_DESC = localModel.T_DESC;
                                    }




                                    if (childTechBidInfo.Name.ToUpper().Trim() == "T_PRE_QUAL")
                                    {
                                        localModel.T_PRE_QUAL = childTechBidInfo.InnerXml.ToString().Trim();
                                        master.T_PRE_QUAL = localModel.T_PRE_QUAL;
                                    }

                                    //
                                    if (childTechBidInfo.Name.ToUpper().Trim() == "T_LOCATION")
                                    {
                                        localModel.T_LOCATION = childTechBidInfo.InnerXml.ToString().Trim();
                                        master.T_LOCATION = localModel.T_LOCATION;
                                    }

                                    if (childTechBidInfo.Name.ToUpper().Trim() == "T_PINCODE")
                                    {
                                        localModel.T_PINCODE = childTechBidInfo.InnerXml.ToString().Trim();
                                        master.T_PINCODE = localModel.T_PINCODE;
                                    }

                                    if (childTechBidInfo.Name.ToUpper().Trim() == "T_CURRENCY")
                                    {
                                        localModel.T_CURRENCY = childTechBidInfo.InnerXml.ToString().Trim();
                                        master.T_CURRENCY = localModel.T_CURRENCY;
                                    }

                                    if (childTechBidInfo.Name.ToUpper().Trim() == "T_FEE")
                                    {
                                        localModel.T_FEE = Convert.ToDecimal(childTechBidInfo.InnerXml);
                                        master.T_FEE = localModel.T_FEE;
                                    }
                                    if (childTechBidInfo.Name.ToUpper().Trim() == "T_VALUE")
                                    {
                                        localModel.T_VALUE = Convert.ToDecimal(childTechBidInfo.InnerXml);
                                        master.T_VALUE = localModel.T_VALUE;
                                    }

                                    if (childTechBidInfo.Name.ToUpper().Trim() == "T_EMD")
                                    {
                                        localModel.T_EMD = Convert.ToDecimal(childTechBidInfo.InnerXml);
                                        master.T_EMD = localModel.T_EMD;
                                    }

                                    if (childTechBidInfo.Name.ToUpper().Trim() == "T_PUB_DATE")
                                    {
                                        localModel.T_PUB_DATE = childTechBidInfo.InnerXml == string.Empty ? (DateTime?)null : Convert.ToDateTime(childTechBidInfo.InnerXml.ToString().Trim());
                                        master.T_PUB_DATE = localModel.T_PUB_DATE;

                                      //  workItems.W_PUB_DATE = (childWorkItems.InnerXml);
                                      //  master.W_PUB_DATE = (workItems.W_PUB_DATE == String.Empty ? (DateTime?)null : Convert.ToDateTime(workItems.W_PUB_DATE));

                                    }

                                    if (childTechBidInfo.Name.ToUpper().Trim() == "T_PREBID_DATE")
                                    {
                                        localModel.T_PREBID_DATE = childTechBidInfo.InnerXml == string.Empty ? (DateTime?)null : Convert.ToDateTime(childTechBidInfo.InnerXml.ToString().Trim()); // Convert.ToDateTime(childTechBidInfo.InnerXml.ToString().Trim());
                                        master.T_PREBID_DATE = localModel.T_PREBID_DATE;
                                    }

                                    if (childTechBidInfo.Name.ToUpper().Trim() == "T_DOC_START_DATE")
                                    {
                                        localModel.T_DOC_START_DATE = childTechBidInfo.InnerXml == string.Empty ? (DateTime?)null : Convert.ToDateTime(childTechBidInfo.InnerXml.ToString().Trim()); //Convert.ToDateTime(childTechBidInfo.InnerXml.ToString().Trim());
                                        master.T_DOC_START_DATE = localModel.T_DOC_START_DATE;
                                    }

                                    if (childTechBidInfo.Name.ToUpper().Trim() == "T_DOC_END_DATE")
                                    {
                                        localModel.T_DOC_END_DATE = childTechBidInfo.InnerXml == string.Empty ? (DateTime?)null : Convert.ToDateTime(childTechBidInfo.InnerXml.ToString().Trim()); //Convert.ToDateTime(childTechBidInfo.InnerXml.ToString().Trim());
                                        master.T_DOC_END_DATE = localModel.T_DOC_END_DATE;
                                    }

                                    if (childTechBidInfo.Name.ToUpper().Trim() == "T_BIDSUB_START_DATE")
                                    {
                                        localModel.T_BIDSUB_START_DATE = childTechBidInfo.InnerXml == string.Empty ? (DateTime?)null : Convert.ToDateTime(childTechBidInfo.InnerXml.ToString().Trim()); // Convert.ToDateTime(childTechBidInfo.InnerXml.ToString().Trim());
                                        master.T_BIDSUB_START_DATE = localModel.T_BIDSUB_START_DATE;
                                    }

                                    if (childTechBidInfo.Name.ToUpper().Trim() == "T_BIDSUB_END_DATE")
                                    {
                                        localModel.T_BIDSUB_END_DATE = childTechBidInfo.InnerXml == string.Empty ? (DateTime?)null : Convert.ToDateTime(childTechBidInfo.InnerXml.ToString().Trim()); // Convert.ToDateTime(childTechBidInfo.InnerXml.ToString().Trim());
                                        master.T_BIDSUB_END_DATE = localModel.T_BIDSUB_END_DATE;
                                    }
                                    if (childTechBidInfo.Name.ToUpper().Trim() == "T_BID_OPEN_DATE")
                                    {
                                        localModel.T_BID_OPEN_DATE = childTechBidInfo.InnerXml == string.Empty ? (DateTime?)null : Convert.ToDateTime(childTechBidInfo.InnerXml.ToString().Trim()); // Convert.ToDateTime(childTechBidInfo.InnerXml.ToString().Trim());
                                        master.T_BID_OPEN_DATE = localModel.T_BID_OPEN_DATE;
                                    }

                                    if (childTechBidInfo.Name.ToUpper().Trim() == "T_INVITING_OFFICER")
                                    {
                                        localModel.T_INVITING_OFFICER = childTechBidInfo.InnerXml.ToString().Trim();
                                        master.T_INVITING_OFFICER = localModel.T_INVITING_OFFICER;
                                    }

                                    if (childTechBidInfo.Name.ToUpper().Trim() == "T_INVITING_OFF_ADDRESS")
                                    {
                                        localModel.T_INVITING_OFF_ADDRESS = childTechBidInfo.InnerXml.ToString().Trim();
                                        master.T_INVITING_OFF_ADDRESS = localModel.T_INVITING_OFF_ADDRESS;
                                    }

                                    if (childTechBidInfo.Name.ToUpper().Trim() == "T_PROD_CAT")
                                    {
                                        localModel.T_PROD_CAT = childTechBidInfo.InnerXml.ToString().Trim();
                                        master.T_PROD_CAT = localModel.T_PROD_CAT;
                                    }
                                    if (childTechBidInfo.Name.ToUpper().Trim() == "T_PROD_SUB_CAT")
                                    {
                                        localModel.T_PROD_SUB_CAT = childTechBidInfo.InnerXml.ToString().Trim();
                                        master.T_PROD_SUB_CAT = localModel.T_PROD_SUB_CAT;
                                    }

                                    if (childTechBidInfo.Name.ToUpper().Trim() == "T_TENDER_TYPE")
                                    {
                                        localModel.T_TENDER_TYPE = childTechBidInfo.InnerXml.ToString().Trim();
                                        master.T_TENDER_TYPE = localModel.T_TENDER_TYPE;
                                    }

                                    if (childTechBidInfo.Name.ToUpper().Trim() == "T_TENDER_CATEGORY")
                                    {
                                        localModel.T_TENDER_CATEGORY = childTechBidInfo.InnerXml.ToString().Trim();
                                        master.T_TENDER_CATEGORY = localModel.T_TENDER_CATEGORY;
                                    }

                                    if (childTechBidInfo.Name.ToUpper().Trim() == "T_FORM_CONTRACT")
                                    {
                                        localModel.T_FORM_CONTRACT = childTechBidInfo.InnerXml.ToString().Trim();
                                        master.T_FORM_CONTRACT = localModel.T_FORM_CONTRACT;
                                    }
                                    if (childTechBidInfo.Name.ToUpper().Trim() == "T_RETURN_URL")
                                    {
                                        localModel.T_RETURN_URL = childTechBidInfo.InnerXml.ToString().Trim();
                                        master.T_RETURN_URL = localModel.T_RETURN_URL;
                                    }

                                    if (childTechBidInfo.Name.ToUpper().Trim() == "T_REMARKS")
                                    {
                                        localModel.T_REMARKS = childTechBidInfo.InnerXml.ToString().Trim();
                                        master.T_REMARKS = localModel.T_REMARKS;
                                    }

                                    if (childTechBidInfo.Name.ToUpper().Trim() == "T_ORGNAME")
                                    {
                                        localModel.T_ORGNAME = childTechBidInfo.InnerXml.ToString().Trim();
                                        master.T_ORGNAME = localModel.T_ORGNAME;
                                    }

                                    if (childTechBidInfo.Name.ToUpper().Trim() == "GENERATED_DATE")
                                    {
                                        localModel.GENERATED_DATE = Convert.ToDateTime(childTechBidInfo.InnerXml.ToString().Trim());
                                        master.GENERATED_DATE = localModel.GENERATED_DATE;
                                    }
                                }
                                dbContext.OMMAS_GEPNIC_TENDERINFO_BY_PDATE.Add(master);
                                dbContext.SaveChanges();
                            }
                            else
                            {
                                return false;
                            }

                        }
                    }
                    #endregion



                    //   ("TN2059");  
                    scope.Complete();
                }


                return true;
            }
            catch (TransactionException ex)
            {
                using (System.IO.StreamWriter sw = System.IO.File.AppendText(System.IO.Path.Combine("D:\\ErrorLog\\OMMASErrorLog_" + System.DateTime.Now.Date.ToString("dd_MM_yyyy") + ".txt", "")))
                {
                    sw.WriteLine("Date :" + DateTime.Now.ToString());
                    sw.WriteLine("Method : " + "GepenicDAL.AddDetails");
                    sw.WriteLine("---------------------------------------------------------------------------------------");
                    sw.Close();
                }

                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                message = "Error occurred while processing your request.";
                return false;

            }
            catch (Exception ex)
            {
                using (System.IO.StreamWriter sw = System.IO.File.AppendText(System.IO.Path.Combine("D:\\ErrorLog\\OMMASErrorLog_" + System.DateTime.Now.Date.ToString("dd_MM_yyyy") + ".txt", "")))
                {
                    sw.WriteLine("Date :" + DateTime.Now.ToString());
                    sw.WriteLine("Method : " + "GepenicDAL.AddDetails");
                    sw.WriteLine("---------------------------------------------------------------------------------------");
                    sw.Close();
                }

                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                message = "Error occurred while processing your request.";
                return false;
            }
            finally
            {
                dbContext.Dispose();
            }
        }
        // GetTenderListByPublishedDate

        public Array GetTenderListByPublishedDate(int? page, int? rows, string sidx, string sord, out long totalRecords)
        {
            dbContext = new PMGSYEntities();
            try
            {
                List<OMMAS_GEPNIC_TENDERINFO_BY_PDATE> listWorkItemDetails = dbContext.OMMAS_GEPNIC_TENDERINFO_BY_PDATE.ToList<OMMAS_GEPNIC_TENDERINFO_BY_PDATE>();


                var lstDPRProposals = (from m in dbContext.OMMAS_GEPNIC_TENDERINFO_BY_PDATE

                                       select new
                                       {
                                           m.T_ID,
                                           m.T_REF_NO,
                                           m.T_TITLE,
                                           m.T_PRE_QUAL,
                                           m.T_LOCATION,
                                           m.T_PINCODE,
                                           m.T_CURRENCY,
                                           m.T_FEE,
                                           m.T_VALUE,
                                           m.T_EMD,
                                           m.T_PUB_DATE,
                                           m.T_PREBID_DATE,
                                           m.T_DOC_START_DATE,
                                           m.T_DOC_END_DATE,
                                           m.T_BIDSUB_START_DATE,
                                           m.T_BIDSUB_END_DATE,
                                           m.T_BID_OPEN_DATE,
                                           m.T_INVITING_OFFICER,
                                           m.T_INVITING_OFF_ADDRESS,
                                           m.T_PROD_CAT,
                                           m.T_PROD_SUB_CAT,
                                           m.T_TENDER_TYPE,
                                           m.T_TENDER_CATEGORY,
                                           m.T_FORM_CONTRACT,
                                           m.T_RETURN_URL,
                                           m.T_REMARKS,
                                           m.T_ORGNAME,
                                           m.GENERATED_DATE

                                       }).ToList();
                totalRecords = lstDPRProposals.Count();
                return lstDPRProposals.ToArray();

                if (sidx.Trim() != string.Empty)
                {
                    if (sord.ToString() == "asc")
                    {
                        switch (sidx)
                        {
                            case "WORKITEM_REF_NO":
                                lstDPRProposals = lstDPRProposals.OrderBy(m => m.T_REF_NO).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                                break;
                            default:
                                lstDPRProposals = lstDPRProposals.OrderBy(m => m.T_REF_NO).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                                break;
                        }
                    }
                    else
                    {
                        switch (sidx)
                        {
                            case "WORKITEM_REF_NO":
                                lstDPRProposals = lstDPRProposals.OrderByDescending(m => m.T_REF_NO).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                                break;
                            default:
                                lstDPRProposals = lstDPRProposals.OrderByDescending(m => m.T_REF_NO).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                                break;
                        }
                    }
                }
                else
                {
                    lstDPRProposals = lstDPRProposals.OrderBy(m => m.T_REF_NO).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                }

                var result = lstDPRProposals.Select(m => new
                {
                    m.T_ID,
                    m.T_REF_NO,
                    m.T_TITLE,
                    m.T_PRE_QUAL,
                    m.T_LOCATION,
                    m.T_PINCODE,
                    m.T_CURRENCY,
                    m.T_FEE,
                    m.T_VALUE,
                    m.T_EMD,
                    m.T_PUB_DATE,
                    m.T_PREBID_DATE,
                    m.T_DOC_START_DATE,
                    m.T_DOC_END_DATE,
                    m.T_BIDSUB_START_DATE,
                    m.T_BIDSUB_END_DATE,
                    m.T_BID_OPEN_DATE,
                    m.T_INVITING_OFFICER,
                    m.T_INVITING_OFF_ADDRESS,
                    m.T_PROD_CAT,
                    m.T_PROD_SUB_CAT,
                    m.T_TENDER_TYPE,
                    m.T_TENDER_CATEGORY,
                    m.T_FORM_CONTRACT,
                    m.T_RETURN_URL,
                    m.T_REMARKS,
                    m.T_ORGNAME,
                    m.GENERATED_DATE
                }).ToArray();

                return result.Select(m => new
                {
                    cell = new[] 
                    {
                        
                                           m.T_ID,
                                           m.T_REF_NO,
                                           m.T_TITLE,
                                           m.T_PRE_QUAL,
                                           m.T_LOCATION,
                                           m.T_PINCODE,
                                           m.T_CURRENCY,
                                           m.T_FEE.ToString(),
                                           m.T_VALUE.ToString(),
                                           m.T_EMD.ToString(),
                                           m.T_PUB_DATE==null?"":Convert.ToDateTime(m.T_PUB_DATE).ToString("dd/MM/yyyy"),
                                           m.T_PREBID_DATE==null?"":Convert.ToDateTime(m.T_PREBID_DATE).ToString("dd/MM/yyyy"),
                                           m.T_DOC_START_DATE==null?"":Convert.ToDateTime(m.T_DOC_START_DATE).ToString("dd/MM/yyyy"),
                                           m.T_DOC_END_DATE==null?"":Convert.ToDateTime(m.T_DOC_END_DATE).ToString("dd/MM/yyyy"),
                                           m.T_BIDSUB_START_DATE==null?"":Convert.ToDateTime(m.T_BIDSUB_START_DATE).ToString("dd/MM/yyyy"),
                                           m.T_BIDSUB_END_DATE==null?"":Convert.ToDateTime(m.T_BIDSUB_END_DATE).ToString("dd/MM/yyyy"),
                                           m.T_BID_OPEN_DATE==null?"":Convert.ToDateTime(m.T_BID_OPEN_DATE).ToString("dd/MM/yyyy"),
                                           m.T_INVITING_OFFICER,
                                           m.T_INVITING_OFF_ADDRESS,
                                           m.T_PROD_CAT,
                                           m.T_PROD_SUB_CAT,
                                           m.T_TENDER_TYPE,
                                           m.T_TENDER_CATEGORY,
                                           m.T_FORM_CONTRACT,
                                           m.T_RETURN_URL,
                                           m.T_REMARKS,
                                           m.T_ORGNAME,
                                           m.GENERATED_DATE==null?"":Convert.ToDateTime(m.GENERATED_DATE).ToString("dd/MM/yyyy")
                    }
                }).ToArray();

            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "GetTenderListByPublishedDate().DAL");
                totalRecords = 0;
                return null;
            }
            finally
            {
                if (dbContext != null)
                {
                    dbContext.Dispose();
                }
            }
        }

        #endregion


        #region Corr Info Details By Published Date
        public bool AddDetailsCorrInfoByPublishedDate(XMLCreation model, ref string message, XmlDocument xmlDoc, XmlNodeList CorrigendumNodes)
        {
            dbContext = new PMGSYEntities();
            try
            {
                using (TransactionScope scope = new TransactionScope())
                {
                    #region Corrigendum Details
                    foreach (XmlNode nodeCorrigendum in CorrigendumNodes)
                    {
                        foreach (XmlNode childCorrigendum in nodeCorrigendum.ChildNodes)
                        {

                            if (childCorrigendum.Name == "TENDER_CORRIGENDUM")
                            {
                                CorriInfoByPublishedDate corrItems = new CorriInfoByPublishedDate();
                                OMMAS_GEPNIC_CORRINFO_BY_PDATE masterCorrigendum = new OMMAS_GEPNIC_CORRINFO_BY_PDATE();
                                masterCorrigendum.GENERATED_DATE = System.DateTime.Now;
                                int corrigendumID = dbContext.OMMAS_GEPNIC_CORRINFO_BY_PDATE.Max(cp => (Int32?)cp.CORRID) == null ? 1 : (Int32)dbContext.OMMAS_GEPNIC_CORRINFO_BY_PDATE.Max(cp => (Int32?)cp.CORRID) + 1;
                                masterCorrigendum.CORRID = corrigendumID;

                                foreach (XmlNode childCORRID in childCorrigendum.ChildNodes)
                                {

                                    if (childCORRID.Name.ToUpper().Trim() == "C_ID")
                                    {
                                        corrItems.C_ID = childCORRID.InnerXml;
                                        masterCorrigendum.C_ID = corrItems.C_ID;
                                    }

                                    if (childCORRID.Name.ToUpper().Trim() == "C_DATE")
                                    {
                                        corrItems.C_DATE = Convert.ToDateTime(childCORRID.InnerXml);
                                        masterCorrigendum.C_DATE = corrItems.C_DATE;


                                    }
                                    if (childCORRID.Name.ToUpper().Trim() == "C_TYPE")
                                    {
                                        corrItems.C_TYPE = childCORRID.InnerXml;
                                        masterCorrigendum.C_TYPE = corrItems.C_TYPE;
                                    }

                                    if (childCORRID.Name.ToUpper().Trim() == "C_STATUS")
                                    {
                                        corrItems.C_STATUS = childCORRID.InnerXml;
                                        masterCorrigendum.C_STATUS = corrItems.C_STATUS;
                                    }

                                    if (childCORRID.Name.ToUpper().Trim() == "T_REF_NO")
                                    {
                                        corrItems.T_REF_NO = childCORRID.InnerXml;
                                        masterCorrigendum.T_REF_NO = corrItems.T_REF_NO;

                                        if (dbContext.OMMAS_GEPNIC_CORRINFO_BY_PDATE.Where(m => m.T_REF_NO.ToUpper() == corrItems.T_REF_NO.ToUpper()).Any())
                                        {
                                            message = "Tender Referance Number is already Exists. Duplicate Entry.";
                                            return false;
                                        }
                                    }

                                    if (childCORRID.Name.ToUpper().Trim() == "T_ID")
                                    {
                                        corrItems.T_ID = childCORRID.InnerXml;
                                        masterCorrigendum.T_ID = corrItems.T_ID;
                                    }

                                    if (childCORRID.Name.ToUpper().Trim() == "C_TITLE")
                                    {
                                        corrItems.C_TITLE = childCORRID.InnerXml;
                                        masterCorrigendum.C_TITLE = corrItems.C_TITLE;
                                    }

                                    if (childCORRID.Name.ToUpper().Trim() == "C_DESC")
                                    {
                                        corrItems.C_DESC = childCORRID.InnerXml;
                                        masterCorrigendum.C_DESC = corrItems.C_DESC;
                                    }
                                    if (childCORRID.Name.ToUpper().Trim() == "T_PRE_QUAL")
                                    {
                                        corrItems.T_PRE_QUAL = childCORRID.InnerXml;
                                        masterCorrigendum.T_PRE_QUAL = corrItems.T_PRE_QUAL;
                                    }

                                    if (childCORRID.Name.ToUpper().Trim() == "T_LOCATION")
                                    {
                                        corrItems.T_LOCATION = childCORRID.InnerXml;
                                        masterCorrigendum.T_LOCATION = corrItems.T_LOCATION;
                                    }

                                    if (childCORRID.Name.ToUpper().Trim() == "T_PINCODE")
                                    {
                                        corrItems.T_PINCODE = childCORRID.InnerXml;
                                        masterCorrigendum.T_PINCODE = corrItems.T_PINCODE;
                                    }

                                    if (childCORRID.Name.ToUpper().Trim() == "T_CURRENCY")
                                    {
                                        corrItems.T_CURRENCY = childCORRID.InnerXml;
                                        masterCorrigendum.T_CURRENCY = corrItems.T_CURRENCY;
                                    }
                                    if (childCORRID.Name.ToUpper().Trim() == "T_FEE")
                                    {
                                        corrItems.T_FEE = Convert.ToDecimal(childCORRID.InnerXml);
                                        masterCorrigendum.T_FEE = corrItems.T_FEE;
                                    }

                                    if (childCORRID.Name.ToUpper().Trim() == "T_VALUE")
                                    {
                                        corrItems.T_VALUE = Convert.ToDecimal(childCORRID.InnerXml);
                                        masterCorrigendum.T_VALUE = corrItems.T_VALUE;
                                    }

                                    if (childCORRID.Name.ToUpper().Trim() == "T_EMD")
                                    {
                                        corrItems.T_EMD = Convert.ToDecimal(childCORRID.InnerXml);
                                        masterCorrigendum.T_EMD = corrItems.T_EMD;
                                    }
                                    if (childCORRID.Name.ToUpper().Trim() == "T_PUB_DATE")
                                    {
                                        corrItems.T_PUB_DATE = Convert.ToDateTime(childCORRID.InnerXml);
                                        masterCorrigendum.T_PUB_DATE = corrItems.T_PUB_DATE;



                                         
                                    }

                                    if (childCORRID.Name.ToUpper().Trim() == "T_PREBID_DATE")
                                    {
                                        corrItems.T_PREBID_DATE = Convert.ToDateTime(childCORRID.InnerXml);
                                        masterCorrigendum.T_PREBID_DATE = corrItems.T_PREBID_DATE;
                                    }

                                    if (childCORRID.Name.ToUpper().Trim() == "T_DOC_START_DATE")
                                    {
                                        corrItems.T_DOC_START_DATE = Convert.ToDateTime(childCORRID.InnerXml);
                                        masterCorrigendum.T_DOC_START_DATE = corrItems.T_DOC_START_DATE;
                                    }

                                    if (childCORRID.Name.ToUpper().Trim() == "T_DOC_END_DATE")
                                    {
                                        corrItems.T_DOC_END_DATE = Convert.ToDateTime(childCORRID.InnerXml);
                                        masterCorrigendum.T_DOC_END_DATE = corrItems.T_DOC_END_DATE;
                                    }

                                    if (childCORRID.Name.ToUpper().Trim() == "T_BIDSUB_START_DATE")
                                    {
                                        corrItems.T_BIDSUB_START_DATE = Convert.ToDateTime(childCORRID.InnerXml);
                                        masterCorrigendum.T_BIDSUB_START_DATE = corrItems.T_BIDSUB_START_DATE;
                                    }

                                    if (childCORRID.Name.ToUpper().Trim() == "T_BIDSUB_END_DATE")
                                    {
                                        corrItems.T_BIDSUB_END_DATE = Convert.ToDateTime(childCORRID.InnerXml);
                                        masterCorrigendum.T_BIDSUB_END_DATE = corrItems.T_BIDSUB_END_DATE;
                                    }

                                    if (childCORRID.Name.ToUpper().Trim() == "T_BID_OPEN_DATE")
                                    {
                                        corrItems.T_BID_OPEN_DATE = Convert.ToDateTime(childCORRID.InnerXml);
                                        masterCorrigendum.T_BID_OPEN_DATE = corrItems.T_BID_OPEN_DATE;
                                    }

                                    if (childCORRID.Name.ToUpper().Trim() == "T_INVITING_OFFICER")
                                    {
                                        corrItems.T_INVITING_OFFICER = childCORRID.InnerXml;
                                        masterCorrigendum.T_INVITING_OFFICER = corrItems.T_INVITING_OFFICER;
                                    }

                                    if (childCORRID.Name.ToUpper().Trim() == "T_INVITING_OFF_ADDRESS")
                                    {
                                        corrItems.T_INVITING_OFF_ADDRESS = childCORRID.InnerXml;
                                        masterCorrigendum.T_INVITING_OFF_ADDRESS = corrItems.T_INVITING_OFF_ADDRESS;
                                    }

                                    if (childCORRID.Name.ToUpper().Trim() == "T_PROD_CAT")
                                    {
                                        corrItems.T_PROD_CAT = childCORRID.InnerXml;
                                        masterCorrigendum.T_PROD_CAT = corrItems.T_PROD_CAT;
                                    }

                                    if (childCORRID.Name.ToUpper().Trim() == "T_PROD_SUB_CAT")
                                    {
                                        corrItems.T_PROD_SUB_CAT = childCORRID.InnerXml;
                                        masterCorrigendum.T_PROD_SUB_CAT = corrItems.T_PROD_SUB_CAT;
                                    }

                                    if (childCORRID.Name.ToUpper().Trim() == "T_TENDER_TYPE")
                                    {
                                        corrItems.T_TENDER_TYPE = childCORRID.InnerXml;
                                        masterCorrigendum.T_TENDER_TYPE = corrItems.T_TENDER_TYPE;
                                    }

                                    if (childCORRID.Name.ToUpper().Trim() == "T_TENDER_CATEGORY")
                                    {
                                        corrItems.T_TENDER_CATEGORY = childCORRID.InnerXml;
                                        masterCorrigendum.T_TENDER_CATEGORY = corrItems.T_TENDER_CATEGORY;
                                    }
                                    if (childCORRID.Name.ToUpper().Trim() == "T_FORM_CONTRACT")
                                    {
                                        corrItems.T_FORM_CONTRACT = childCORRID.InnerXml;
                                        masterCorrigendum.T_FORM_CONTRACT = corrItems.T_FORM_CONTRACT;
                                    }
                                    if (childCORRID.Name.ToUpper().Trim() == "T_RETURN_URL")
                                    {
                                        corrItems.T_RETURN_URL = childCORRID.InnerXml;
                                        masterCorrigendum.T_RETURN_URL = corrItems.T_RETURN_URL;
                                    }
                                    if (childCORRID.Name.ToUpper().Trim() == "T_REMARKS")
                                    {
                                        corrItems.T_REMARKS = childCORRID.InnerXml;
                                        masterCorrigendum.T_REMARKS = corrItems.T_REMARKS;
                                    }
                                    if (childCORRID.Name.ToUpper().Trim() == "TENDER_CORRIGENDUM")
                                    {
                                        corrItems.TENDER_CORRIGENDUM = childCORRID.InnerXml;
                                        masterCorrigendum.TENDER_CORRIGENDUM = corrItems.TENDER_CORRIGENDUM;
                                    }
                                    if (childCORRID.Name.ToUpper().Trim() == "GENERATED_DATE")
                                    {
                                        corrItems.GENERATED_DATE = Convert.ToDateTime(childCORRID.InnerXml);
                                        masterCorrigendum.GENERATED_DATE = corrItems.GENERATED_DATE;
                                    }
                                }
                                dbContext.OMMAS_GEPNIC_CORRINFO_BY_PDATE.Add(masterCorrigendum);
                                dbContext.SaveChanges();
                            }
                            else
                            {
                                return false;
                            }

                        }
                    }
                    #endregion
                    //   ("TN2059");  
                    scope.Complete();
                }


                return true;
            }
            catch (TransactionException ex)
            {
                using (System.IO.StreamWriter sw = System.IO.File.AppendText(System.IO.Path.Combine("D:\\ErrorLog\\OMMASErrorLog_" + System.DateTime.Now.Date.ToString("dd_MM_yyyy") + ".txt", "")))
                {
                    sw.WriteLine("Date :" + DateTime.Now.ToString());
                    sw.WriteLine("Method : " + "GepenicDAL.AddDetails");
                    sw.WriteLine("---------------------------------------------------------------------------------------");
                    sw.Close();
                }

                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                message = "Error occurred while processing your request.";
                return false;

            }
            catch (Exception ex)
            {
                using (System.IO.StreamWriter sw = System.IO.File.AppendText(System.IO.Path.Combine("D:\\ErrorLog\\OMMASErrorLog_" + System.DateTime.Now.Date.ToString("dd_MM_yyyy") + ".txt", "")))
                {
                    sw.WriteLine("Date :" + DateTime.Now.ToString());
                    sw.WriteLine("Method : " + "GepenicDAL.AddDetails");
                    sw.WriteLine("---------------------------------------------------------------------------------------");
                    sw.Close();
                }

                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                message = "Error occurred while processing your request.";
                return false;
            }
            finally
            {
                dbContext.Dispose();
            }
        }

        public Array GetCorrListByPublishedDate(int? page, int? rows, string sidx, string sord, out long totalRecords)
        {
            dbContext = new PMGSYEntities();
            try
            {
                List<OMMAS_GEPNIC_CORRINFO_BY_PDATE> listWorkItemDetails = dbContext.OMMAS_GEPNIC_CORRINFO_BY_PDATE.ToList<OMMAS_GEPNIC_CORRINFO_BY_PDATE>();

                var lstDPRProposals = (from m in dbContext.OMMAS_GEPNIC_CORRINFO_BY_PDATE

                                       select new
                                       {
                                           m.C_ID,
                                           m.C_DATE,
                                           m.C_TYPE,
                                           m.C_STATUS,

                                           m.T_REF_NO,
                                           m.T_ID,
                                           m.C_TITLE,
                                           m.C_DESC,

                                           m.T_PRE_QUAL,
                                           m.T_LOCATION,
                                           m.T_PINCODE,
                                           m.T_CURRENCY,
                                           m.T_FEE,

                                           m.T_VALUE,
                                           m.T_EMD,
                                           m.T_PUB_DATE,
                                           m.T_PREBID_DATE,

                                           m.T_DOC_START_DATE,
                                           m.T_DOC_END_DATE,
                                           m.T_BIDSUB_START_DATE,
                                           m.T_BIDSUB_END_DATE,

                                           m.T_BID_OPEN_DATE,
                                           m.T_INVITING_OFFICER,
                                           m.T_INVITING_OFF_ADDRESS,
                                           m.T_PROD_CAT,

                                           m.T_PROD_SUB_CAT,
                                           m.T_TENDER_TYPE,
                                           m.T_TENDER_CATEGORY,
                                           m.T_FORM_CONTRACT,
                                           m.T_RETURN_URL,
                                           m.T_REMARKS,
                                           m.TENDER_CORRIGENDUM,
                                           m.GENERATED_DATE

                                       }).ToList();
                totalRecords = lstDPRProposals.Count();
                return lstDPRProposals.ToArray();

                if (sidx.Trim() != string.Empty)
                {
                    if (sord.ToString() == "asc")
                    {
                        switch (sidx)
                        {
                            case "WORKITEM_REF_NO":
                                lstDPRProposals = lstDPRProposals.OrderBy(m => m.T_REF_NO).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                                break;
                            default:
                                lstDPRProposals = lstDPRProposals.OrderBy(m => m.T_REF_NO).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                                break;
                        }
                    }
                    else
                    {
                        switch (sidx)
                        {
                            case "WORKITEM_REF_NO":
                                lstDPRProposals = lstDPRProposals.OrderByDescending(m => m.T_REF_NO).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                                break;
                            default:
                                lstDPRProposals = lstDPRProposals.OrderByDescending(m => m.T_REF_NO).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                                break;
                        }
                    }
                }
                else
                {
                    lstDPRProposals = lstDPRProposals.OrderBy(m => m.T_REF_NO).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                }

                var result = lstDPRProposals.Select(m => new
                {
                    m.C_ID,
                    m.C_DATE,
                    m.C_TYPE,
                    m.C_STATUS,
                    m.T_REF_NO,
                    m.T_ID,
                    m.C_TITLE,
                    m.C_DESC,
                    m.T_PRE_QUAL,
                    m.T_LOCATION,
                    m.T_PINCODE,
                    m.T_CURRENCY,
                    m.T_FEE,
                    m.T_VALUE,
                    m.T_EMD,
                    m.T_PUB_DATE,
                    m.T_PREBID_DATE,
                    m.T_DOC_START_DATE,
                    m.T_DOC_END_DATE,
                    m.T_BIDSUB_START_DATE,
                    m.T_BIDSUB_END_DATE,
                    m.T_BID_OPEN_DATE,
                    m.T_INVITING_OFFICER,
                    m.T_INVITING_OFF_ADDRESS,
                    m.T_PROD_CAT,
                    m.T_PROD_SUB_CAT,
                    m.T_TENDER_TYPE,
                    m.T_TENDER_CATEGORY,
                    m.T_FORM_CONTRACT,
                    m.T_RETURN_URL,
                    m.T_REMARKS,
                    m.TENDER_CORRIGENDUM,
                    m.GENERATED_DATE
                }).ToArray();

                return result.Select(m => new
                {
                    cell = new[] 
                    {
                        
                                           m.C_ID,
                                           m.C_DATE== null ?"":Convert.ToDateTime(m.C_DATE).ToString("dd/MM/yyyy"),
                                           m.C_TYPE,
                                           m.C_STATUS,
                                           m.T_REF_NO,
                                           m.T_ID,
                                           m.C_TITLE,
                                           m.C_DESC,
                                           m.T_PRE_QUAL,
                                           m.T_LOCATION,
                                           m.T_PINCODE,
                                           m.T_CURRENCY,
                                           m.T_FEE.ToString(),
                                           m.T_VALUE.ToString(),
                                           m.T_EMD.ToString(),
                                           m.T_PUB_DATE== null ?"":Convert.ToDateTime(m.T_PUB_DATE).ToString("dd/MM/yyyy"),
                                           m.T_PREBID_DATE== null ?"":Convert.ToDateTime(m.T_PREBID_DATE).ToString("dd/MM/yyyy"),
                                           m.T_DOC_START_DATE== null ?"":Convert.ToDateTime(m.T_DOC_START_DATE).ToString("dd/MM/yyyy"),
                                           m.T_DOC_END_DATE== null ?"":Convert.ToDateTime(m.T_DOC_END_DATE).ToString("dd/MM/yyyy"),
                                           m.T_BIDSUB_START_DATE== null ?"":Convert.ToDateTime(m.T_BIDSUB_START_DATE).ToString("dd/MM/yyyy"),
                                           m.T_BIDSUB_END_DATE== null ?"":Convert.ToDateTime(m.T_BIDSUB_END_DATE).ToString("dd/MM/yyyy"),
                                           m.T_BID_OPEN_DATE== null ?"":Convert.ToDateTime(m.T_BID_OPEN_DATE).ToString("dd/MM/yyyy"),
                                           m.T_INVITING_OFFICER,
                                           m.T_INVITING_OFF_ADDRESS,
                                           m.T_PROD_CAT,
                                           m.T_PROD_SUB_CAT,
                                           m.T_TENDER_TYPE,
                                           m.T_TENDER_CATEGORY,
                                           m.T_FORM_CONTRACT,
                                           m.T_RETURN_URL,
                                           m.T_REMARKS,
                                           m.TENDER_CORRIGENDUM,
                                           m.GENERATED_DATE== null ?"":Convert.ToDateTime(m.GENERATED_DATE).ToString("dd/MM/yyyy")
                                          
                    }
                }).ToArray();

            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "GetTenderList().DAL");
                totalRecords = 0;
                return null;
            }
            finally
            {
                if (dbContext != null)
                {
                    dbContext.Dispose();
                }
            }
        }
        #endregion


        #region AOC Details By Published Date

        public bool AddDetailsAOCInfoByPublishedDate(XMLCreation model, ref string message, XmlDocument xmlDoc, XmlNodeList CorrigendumNodes)
        {
            dbContext = new PMGSYEntities();
            try
            {
                using (TransactionScope scope = new TransactionScope())
                {
                    #region Corrigendum Details
                    foreach (XmlNode nodeCorrigendum in CorrigendumNodes)
                    {
                        foreach (XmlNode childCorrigendum in nodeCorrigendum.ChildNodes)
                        {

                            if (childCorrigendum.Name == "AOCS")
                            {
                                AOCInfoByPublishedDate corrItems = new AOCInfoByPublishedDate();
                                OMMAS_GEPNIC_AOCINFO_BY_CDATE masterCorrigendum = new OMMAS_GEPNIC_AOCINFO_BY_CDATE();
                                masterCorrigendum.GENERATED_DATE = System.DateTime.Now;
                                int AOCID = dbContext.OMMAS_GEPNIC_AOCINFO_BY_CDATE.Max(cp => (Int32?)cp.AOCID) == null ? 1 : (Int32)dbContext.OMMAS_GEPNIC_AOCINFO_BY_CDATE.Max(cp => (Int32?)cp.AOCID) + 1;

                                //  int aocID =dbContext.OMMAS_GEPNIC_AOCINFO_BY_CDATE.Max(a => a.AOCID) == null ? 1 : dbContext.OMMAS_GEPNIC_AOCINFO_BY_CDATE.Max(a => a.AOCID) + 1;
                                masterCorrigendum.AOCID = AOCID;

                                foreach (XmlNode childCORRID in childCorrigendum.ChildNodes)
                                {

                                    if (childCORRID.Name.ToUpper().Trim() == "A_TENDERID")
                                    {
                                        corrItems.A_TENDERID = childCORRID.InnerXml;
                                        masterCorrigendum.A_TENDERID = corrItems.A_TENDERID;
                                    }

                                    if (childCORRID.Name.ToUpper().Trim() == "A_TENDERREFNO")
                                    {
                                        corrItems.A_TENDERREFNO = childCORRID.InnerXml;
                                        masterCorrigendum.A_TENDERREFNO = corrItems.A_TENDERREFNO;

                                        if (dbContext.OMMAS_GEPNIC_AOCINFO_BY_CDATE.Where(m => m.A_TENDERREFNO.ToUpper() == corrItems.A_TENDERREFNO.ToUpper()).Any())
                                        {
                                            message = "Tender Referance Number is already Exists. Duplicate Entry.";
                                            return false;
                                        }
                                    }

                                    if (childCORRID.Name.ToUpper().Trim() == "A_WORKITEMID")
                                    {
                                        corrItems.A_WORKITEMID = childCORRID.InnerXml;
                                        masterCorrigendum.A_WORKITEMID = corrItems.A_WORKITEMID;
                                    }

                                    if (childCORRID.Name.ToUpper().Trim() == "A_TENDER_DESC")
                                    {
                                        corrItems.A_TENDER_DESC = childCORRID.InnerXml;
                                        masterCorrigendum.A_TENDER_DESC = corrItems.A_TENDER_DESC;
                                    }
                                    if (childCORRID.Name.ToUpper().Trim() == "A_TENDER_TYPE")
                                    {
                                        corrItems.A_TENDER_TYPE = childCORRID.InnerXml;
                                        masterCorrigendum.A_TENDER_TYPE = corrItems.A_TENDER_TYPE;
                                    }

                                    if (childCORRID.Name.ToUpper().Trim() == "A_PUBLISHED_DATE")
                                    {
                                        corrItems.A_PUBLISHED_DATE = Convert.ToDateTime(childCORRID.InnerXml);
                                        masterCorrigendum.A_PUBLISHED_DATE = corrItems.A_PUBLISHED_DATE;
                                    }


                                    if (childCORRID.Name.ToUpper().Trim() == "A_NO_OF_BIDS")
                                    {
                                        corrItems.A_NO_OF_BIDS = childCORRID.InnerXml;

                                        masterCorrigendum.A_NO_OF_BIDS = corrItems.A_NO_OF_BIDS;
                                    }

                                    if (childCORRID.Name.ToUpper().Trim() == "A_NO_BIDS_RECD")
                                    {
                                        corrItems.A_NO_BIDS_RECD = childCORRID.InnerXml;
                                        masterCorrigendum.A_NO_BIDS_RECD = corrItems.A_NO_BIDS_RECD;
                                    }
                                    if (childCORRID.Name.ToUpper().Trim() == "A_L1BIDDER_NAME")
                                    {
                                        corrItems.A_L1BIDDER_NAME = childCORRID.InnerXml;
                                        masterCorrigendum.A_L1BIDDER_NAME = corrItems.A_L1BIDDER_NAME;
                                    }

                                    if (childCORRID.Name.ToUpper().Trim() == "A_L1BIDDER_ADDRESS")
                                    {
                                        corrItems.A_L1BIDDER_ADDRESS = childCORRID.InnerXml;
                                        masterCorrigendum.A_L1BIDDER_ADDRESS = corrItems.A_L1BIDDER_ADDRESS;
                                    }
                                    if (childCORRID.Name.ToUpper().Trim() == "A_CONTRACT_DATE")
                                    {
                                        corrItems.A_CONTRACT_DATE = Convert.ToDateTime(childCORRID.InnerXml);
                                        masterCorrigendum.A_CONTRACT_DATE = corrItems.A_CONTRACT_DATE;
                                    }

                                    if (childCORRID.Name.ToUpper().Trim() == "A_CONTRACT_NO")
                                    {
                                        corrItems.A_CONTRACT_NO = childCORRID.InnerXml;
                                        masterCorrigendum.A_CONTRACT_NO = corrItems.A_CONTRACT_NO;
                                    }
                                    if (childCORRID.Name.ToUpper().Trim() == "A_CURRENCY")
                                    {
                                        corrItems.A_CURRENCY = childCORRID.InnerXml;
                                        masterCorrigendum.A_CURRENCY = corrItems.A_CURRENCY;
                                    }

                                    if (childCORRID.Name.ToUpper().Trim() == "A_CONTRACT_VALUE")
                                    {
                                        corrItems.A_CONTRACT_VALUE = childCORRID.InnerXml;
                                        masterCorrigendum.A_CONTRACT_VALUE = corrItems.A_CONTRACT_VALUE;
                                    }


                                    if (childCORRID.Name.ToUpper().Trim() == "A_DATE_COMPLETION")
                                    {
                                        corrItems.A_DATE_COMPLETION = childCORRID.InnerXml;
                                        masterCorrigendum.A_DATE_COMPLETION = corrItems.A_DATE_COMPLETION;
                                    }

                                    if (childCORRID.Name.ToUpper().Trim() == "A_CONT_VALID_FROM")
                                    {
                                        corrItems.A_CONT_VALID_FROM = childCORRID.InnerXml;
                                        masterCorrigendum.A_CONT_VALID_FROM = corrItems.A_CONT_VALID_FROM;
                                    }

                                    if (childCORRID.Name.ToUpper().Trim() == "A_CONT_VALID_TO")
                                    {
                                        corrItems.A_CONT_VALID_TO = childCORRID.InnerXml;
                                        masterCorrigendum.A_CONT_VALID_TO = corrItems.A_CONT_VALID_TO;
                                    }
                                    if (childCORRID.Name.ToUpper().Trim() == "A_PARTIES_QUALIFIED")
                                    {
                                        corrItems.A_PARTIES_QUALIFIED = childCORRID.InnerXml;
                                        masterCorrigendum.A_PARTIES_QUALIFIED = corrItems.A_PARTIES_QUALIFIED;
                                    }
                                    if (childCORRID.Name.ToUpper().Trim() == "A_PARTIES_NOTQUALIFIED")
                                    {
                                        corrItems.A_PARTIES_NOTQUALIFIED = childCORRID.InnerXml;
                                        masterCorrigendum.A_PARTIES_NOTQUALIFIED = corrItems.A_PARTIES_NOTQUALIFIED;
                                    }

                                    if (childCORRID.Name.ToUpper().Trim() == "A_ORGID")
                                    {
                                        corrItems.A_ORGID = childCORRID.InnerXml;
                                        masterCorrigendum.A_ORGID = corrItems.A_ORGID;
                                    }
                                    if (childCORRID.Name.ToUpper().Trim() == "A_ORGNAME")
                                    {
                                        corrItems.A_ORGNAME = childCORRID.InnerXml;
                                        masterCorrigendum.A_ORGNAME = corrItems.A_ORGNAME;
                                    }

                                    if (childCORRID.Name.ToUpper().Trim() == "A_REMARKS")
                                    {
                                        corrItems.A_REMARKS = childCORRID.InnerXml;
                                        masterCorrigendum.A_REMARKS = corrItems.A_REMARKS;
                                    }
                                    if (childCORRID.Name.ToUpper().Trim() == "GENERATED_DATE")
                                    {
                                        corrItems.GENERATED_DATE = Convert.ToDateTime(childCORRID.InnerXml);
                                        masterCorrigendum.GENERATED_DATE = corrItems.GENERATED_DATE;
                                    }
                                }
                                dbContext.OMMAS_GEPNIC_AOCINFO_BY_CDATE.Add(masterCorrigendum);
                                dbContext.SaveChanges();
                            }
                            else
                            {
                                return false;
                            }

                        }
                    }
                    #endregion
                    //   ("TN2059");  
                    scope.Complete();
                }


                return true;
            }
            catch (TransactionException ex)
            {
                using (System.IO.StreamWriter sw = System.IO.File.AppendText(System.IO.Path.Combine("D:\\ErrorLog\\OMMASErrorLog_" + System.DateTime.Now.Date.ToString("dd_MM_yyyy") + ".txt", "")))
                {
                    sw.WriteLine("Date :" + DateTime.Now.ToString());
                    sw.WriteLine("Method : " + "GepenicDAL.AddDetails");
                    sw.WriteLine("---------------------------------------------------------------------------------------");
                    sw.Close();
                }

                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                message = "Error occurred while processing your request.";
                return false;

            }
            catch (Exception ex)
            {
                using (System.IO.StreamWriter sw = System.IO.File.AppendText(System.IO.Path.Combine("D:\\ErrorLog\\OMMASErrorLog_" + System.DateTime.Now.Date.ToString("dd_MM_yyyy") + ".txt", "")))
                {
                    sw.WriteLine("Date :" + DateTime.Now.ToString());
                    sw.WriteLine("Method : " + "GepenicDAL.AddDetails");
                    sw.WriteLine("---------------------------------------------------------------------------------------");
                    sw.Close();
                }

                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                message = "Error occurred while processing your request.";
                return false;
            }
            finally
            {
                dbContext.Dispose();
            }
        }

        public Array GetAOCListByPublishedDate(int? page, int? rows, string sidx, string sord, out long totalRecords)
        {
            dbContext = new PMGSYEntities();
            try
            {
                List<OMMAS_GEPNIC_AOCINFO_BY_CDATE> listWorkItemDetails = dbContext.OMMAS_GEPNIC_AOCINFO_BY_CDATE.ToList<OMMAS_GEPNIC_AOCINFO_BY_CDATE>();

                var lstDPRProposals = (from m in dbContext.OMMAS_GEPNIC_AOCINFO_BY_CDATE

                                       select new
                                       {

                                           m.A_TENDERID,
                                           m.A_TENDERREFNO,
                                           m.A_WORKITEMID,
                                           m.A_TENDER_DESC,
                                           m.A_TENDER_TYPE,
                                           m.A_PUBLISHED_DATE,
                                           m.A_NO_OF_BIDS,
                                           m.A_NO_BIDS_RECD,
                                           m.A_L1BIDDER_NAME,
                                           m.A_L1BIDDER_ADDRESS,
                                           m.A_CONTRACT_DATE,
                                           m.A_CONTRACT_NO,
                                           m.A_CURRENCY,
                                           m.A_CONTRACT_VALUE,
                                           m.A_DATE_COMPLETION,
                                           m.A_CONT_VALID_FROM,
                                           m.A_CONT_VALID_TO,
                                           m.A_PARTIES_QUALIFIED,
                                           m.A_PARTIES_NOTQUALIFIED,
                                           m.A_ORGID,
                                           m.A_ORGNAME,
                                           m.A_REMARKS,
                                           m.A_RETURN_URL,
                                           m.GENERATED_DATE,

                                       }).ToList();

                totalRecords = lstDPRProposals.Count();
                return lstDPRProposals.ToArray();

                if (sidx.Trim() != string.Empty)
                {
                    if (sord.ToString() == "asc")
                    {
                        switch (sidx)
                        {
                            case "A_TENDERREFNO":
                                lstDPRProposals = lstDPRProposals.OrderBy(m => m.A_TENDERREFNO).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                                break;
                            default:
                                lstDPRProposals = lstDPRProposals.OrderBy(m => m.A_TENDERREFNO).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                                break;
                        }
                    }
                    else
                    {
                        switch (sidx)
                        {
                            case "A_TENDERREFNO":
                                lstDPRProposals = lstDPRProposals.OrderByDescending(m => m.A_TENDERREFNO).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                                break;
                            default:
                                lstDPRProposals = lstDPRProposals.OrderByDescending(m => m.A_TENDERREFNO).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                                break;
                        }
                    }
                }
                else
                {
                    lstDPRProposals = lstDPRProposals.OrderBy(m => m.A_TENDERREFNO).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                }

                var result = lstDPRProposals.Select(m => new
                {

                    m.A_TENDERID,
                    m.A_TENDERREFNO,
                    m.A_WORKITEMID,
                    m.A_TENDER_DESC,
                    m.A_TENDER_TYPE,
                    m.A_PUBLISHED_DATE,
                    m.A_NO_OF_BIDS,
                    m.A_NO_BIDS_RECD,
                    m.A_L1BIDDER_NAME,
                    m.A_L1BIDDER_ADDRESS,
                    m.A_CONTRACT_DATE,
                    m.A_CONTRACT_NO,
                    m.A_CURRENCY,
                    m.A_CONTRACT_VALUE,
                    m.A_DATE_COMPLETION,
                    m.A_CONT_VALID_FROM,
                    m.A_CONT_VALID_TO,
                    m.A_PARTIES_QUALIFIED,
                    m.A_PARTIES_NOTQUALIFIED,
                    m.A_ORGID,
                    m.A_ORGNAME,
                    m.A_REMARKS,
                    m.A_RETURN_URL,
                    m.GENERATED_DATE,
                }).ToArray();

                return result.Select(m => new
                {
                    cell = new[] 
                    {
                        
                                         
                                           m.A_TENDERID,
                                           m.A_TENDERREFNO,
                                           m.A_WORKITEMID,
                                           m.A_TENDER_DESC,

                                           m.A_TENDER_TYPE,
                                           m.A_PUBLISHED_DATE== null ?"":Convert.ToDateTime(m.A_PUBLISHED_DATE).ToString("dd/MM/yyyy"), 
                                           m.A_NO_OF_BIDS,
                                           m.A_NO_BIDS_RECD,

                                           m.A_L1BIDDER_NAME,
                                           m.A_L1BIDDER_ADDRESS,
                                           m.A_CONTRACT_DATE== null ?"":Convert.ToDateTime(m.A_CONTRACT_DATE).ToString("dd/MM/yyyy"), 
                                           m.A_CONTRACT_NO,

                                           m.A_CURRENCY,
                                           m.A_CONTRACT_VALUE,
                                           m.A_DATE_COMPLETION,
                                           m.A_CONT_VALID_FROM,

                                           m.A_CONT_VALID_TO,
                                           m.A_PARTIES_QUALIFIED,
                                           m.A_PARTIES_NOTQUALIFIED,
                                           m.A_ORGID,

                                           m.A_ORGNAME,
                                           m.A_REMARKS,
                                           m.A_RETURN_URL,
                                           m.GENERATED_DATE== null ?"":Convert.ToDateTime(m.GENERATED_DATE).ToString("dd/MM/yyyy")
                                      
                    }
                }).ToArray();

            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "GetAOCListByPublishedDate().DAL");
                totalRecords = 0;
                return null;
            }
            finally
            {
                if (dbContext != null)
                {
                    dbContext.Dispose();
                }
            }
        }

        #endregion

    }
}