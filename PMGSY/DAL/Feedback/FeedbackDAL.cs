using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PMGSY.Extensions;
using PMGSY.Models.Feedback;
using PMGSY.Models;
using PMGSY.Common;
using System.IO;
using System.Data.Entity.Validation;

namespace Feedback.DAL
{
    public class FeedbackDAL
    {
        Dictionary<string, string> decryptedParameters = null;

        PMGSY.Models.PMGSYEntities dbContext = null;
        #region Feedback Details
        public List<SelectListItem> PopulateStates(bool flag)
        {
            PMGSY.Models.PMGSYEntities dbContext = new PMGSY.Models.PMGSYEntities();
            List<SelectListItem> habitatList = new List<SelectListItem>();
            SelectListItem item;
            try
            {
                if (PMGSYSession.Current.RoleCode == 25 || PMGSYSession.Current.RoleCode == 5)
                {
                    if (flag)
                    {
                        item = new SelectListItem();
                        item.Text = "All States";
                        item.Value = "0";
                        habitatList.Add(item);
                    }
                    else
                    {
                        item = new SelectListItem();
                        item.Text = "Select";
                        item.Value = "0";
                        habitatList.Add(item);
                    }

                    var q = (from s in dbContext.MASTER_STATE orderby s.MAST_STATE_NAME select s).ToList();
                    foreach (var itm in q)
                    {
                        item = new SelectListItem();
                        item.Text = itm.MAST_STATE_NAME;
                        item.Value = Convert.ToString(itm.MAST_STATE_CODE).Trim();
                        habitatList.Add(item);
                    }
                }
                else
                {
                    //var q = (from s in dbContext.MASTER_STATE orderby s.MAST_STATE_NAME select s).ToList();

                    item = new SelectListItem();
                    item.Text = PMGSYSession.Current.StateName.Trim();
                    item.Value = Convert.ToString(PMGSYSession.Current.StateCode).Trim();
                    habitatList.Add(item);
                }

                return habitatList;
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
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

        public Array FBListDAL(int? page, int? rows, string sidx, string sord, out Int32 totalRecords, int formonth, int foryear, int state, string category, string appr, string status, string fbthrough)
        {
            if (PMGSYSession.Current.RoleCode == 8 || PMGSYSession.Current.RoleCode == 22)
            {
                state = PMGSYSession.Current.StateCode;
            }
            //int formonth=1;
            //int foryear=2014;
            //int state=0;
            //string category="C";
            //string appr = "N";
            //string status="N";
            dbContext = new PMGSY.Models.PMGSYEntities();
            int district = 0;
            if (PMGSYSession.Current.RoleCode == 22)
            {
                district = PMGSYSession.Current.DistrictCode;
            }
            SelectListItem item;
            try
            {
                var itemList = (from s in dbContext.ADMIN_FEEDBACK
                                where (s.FEED_DATE.Month == formonth
                                        //&& s.FEED_DATE.Year.Equals(foryear)
                                        && (foryear == -1 ? 1 : s.FEED_DATE.Year) == (foryear == -1 ? 1 : foryear)
                                        && (category == "0" ? "" : s.FEED_CATEGORY) == (category == "0" ? "" : category)
                                        //&& s.FEED_APPROVAL.Equals(appr.Trim())
                                        && (appr.Trim() == "0" ? 1 == 1 : s.FEED_APPROVAL.Equals(appr.Trim()))
                                        //&& (state == 0 ? (s.MAST_STATE_CODE == null ? 1 : s.MAST_STATE_CODE) : s.MAST_STATE_CODE) == (state == 0 ? (s.MAST_STATE_CODE == null ? 1 : state) : s.MAST_STATE_CODE)
                                        && ((PMGSYSession.Current.RoleCode == 25 || PMGSYSession.Current.RoleCode == 5 || PMGSYSession.Current.RoleCode == 22)
                                            ? (state == 0 ? 1 == 1 : s.MAST_STATE_CODE == state)
                                            : (state == 0 ? s.MAST_STATE_CODE != null : (s.MAST_STATE_CODE != null && s.MAST_STATE_CODE == state))
                                        )
                                        /*For PIU*/
                                        && (PMGSYSession.Current.RoleCode == 22 ? (s.MAST_DISTRICT_CODE == district) : (1 == 1))
                                        && (status == "0" ? "" : s.FEED_STATUS) == (status == "0" ? "" : status)
                                        && (fbthrough == "0" ? 1 == 1 : fbthrough == "M" ? s.CITIZEN_ID != null : s.CITIZEN_ID == null)
                                         && (PMGSYSession.Current.DistrictCode == null || PMGSYSession.Current.DistrictCode == 0) ? 1 == 1 : s.MAST_DISTRICT_CODE == PMGSYSession.Current.DistrictCode
                                      )
                                orderby s.FEED_ID
                                select s).ToList();
                totalRecords = itemList.Count();

                if (sidx.Trim() != null)
                {
                    if (sord.ToString() == "asc")
                    {
                        itemList = itemList.OrderBy(x => x.FEED_ID).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                    }
                    else
                    {
                        itemList = itemList.OrderByDescending(x => x.FEED_ID).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                    }
                }
                else
                {
                    itemList = itemList.OrderBy(x => x.FEED_ID).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                }
                itemList = itemList.OrderBy(x => x.FEED_FOR).ToList();
                return itemList.Select(itemDetails => new
                {
                    cell = new[] {
                                        itemDetails.FEED_ID.ToString().Trim(),
                                        itemDetails.FEED_DATE.ToString("dd/MM/yyyy").Trim(),
                                        itemDetails.FEED_NAME,
                                        itemDetails.FEED_CATEGORY=="F"?"Comment":(itemDetails.FEED_CATEGORY)=="C"?"Complaint":(itemDetails.FEED_CATEGORY)=="Q"?"Query":"General",
                                        itemDetails.FEED_FOR=="H"?"Habitation":itemDetails.FEED_FOR=="R"?"Road":"General",
                                        itemDetails.MASTER_FEEDBACK_CATEGORY.MAST_FEED_NAME,
                                        itemDetails.CITIZEN_ID != null ? "Mobile" : "Web",
                                        //itemDetails.FEED_SUBJECT,
                                        itemDetails.FEED_STATUS=="N"?"Not Replied":(itemDetails.FEED_STATUS=="I")?"Interim Reply":(itemDetails.FEED_STATUS=="O")?"Open Reply": (itemDetails.FEED_STATUS=="F" && itemDetails.ADMIN_FEEDBACK_REPLY.Where(x=>x.FEED_ID == itemDetails.FEED_ID).OrderByDescending(x => x.REP_ID).Select(x => x.UM_User_Master.DefaultRoleID).FirstOrDefault() == 22) ? "Final Reply (PIU)" :"Final Reply",

                                        ((itemDetails.IS_PMGSY_ROAD == "1" && itemDetails.FEED_APPROVAL == "N" && !itemDetails.FEED_APPROVAL_DATE.HasValue)
                                        ||
                                          (itemDetails.IS_PMGSY_ROAD == "1" &&
                                          (itemDetails.FEED_STATUS=="F" && itemDetails.ADMIN_FEEDBACK_REPLY.Where(x=>x.FEED_ID == itemDetails.FEED_ID).OrderByDescending(x => x.REP_ID).Select(x => x.UM_User_Master.DefaultRoleID).FirstOrDefault() == 22))
                                        || (itemDetails.IS_PMGSY_ROAD != "1" && itemDetails.FEED_APPROVAL == "Y" && !itemDetails.FEED_APPROVAL_DATE.HasValue))
                                        ? "SQC"
                                        : (itemDetails.IS_PMGSY_ROAD == "1" && itemDetails.FEED_APPROVAL == "Y" && itemDetails.FEED_APPROVAL_DATE.HasValue &&                                                   itemDetails.FEED_STATUS!="F")
                                        ? "PIU/SQC"
                                        : (itemDetails.IS_PMGSY_ROAD != "1" && itemDetails.FEED_STATUS!="F"/*&& itemDetails.FEED_APPROVAL == "N" && !itemDetails.FEED_APPROVAL_DATE.HasValue*/)
                                        ? "NRRDA"
                                        :"-",

                                        itemDetails.FEED_APPROVAL=="N"? "" + itemDetails.FEED_APPROVAL_DATE == "" ? "Not Accepted" : "Not Accepted (" + Convert.ToDateTime(itemDetails.FEED_APPROVAL_DATE).ToString("dd/MM/yyyy").Trim() +")"
                                                                      :"Accepted (" + Convert.ToDateTime(itemDetails.FEED_APPROVAL_DATE).ToString("dd/MM/yyyy").Trim() +")",
                                        "<a href='#'  class='ui-icon ui-icon-zoomin ui-align-center' onClick='ShowDetails(\""+ PMGSY.Common.URLEncrypt.EncryptParameters1(new string[] { "FeedCode =" +  itemDetails.FEED_ID.ToString().Trim()}) +"\"); return false;'>Show Details</a>"
                                        //URLEncrypt.EncryptParameters1(new string[] { "StateCode =" + stateDetails.MAST_STATE_CODE.ToString().Trim()}),
                               }
                }).ToArray();
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                totalRecords = 0;
                return null;
            }
            finally
            {
                dbContext.Dispose();
            }
        }


        #endregion

        #region Feedback Approval

        public List<SelectListItem> PopulateDistricts(string state)
        {
            PMGSY.Models.PMGSYEntities dbContext = new PMGSY.Models.PMGSYEntities();
            List<SelectListItem> districtList = new List<SelectListItem>();
            SelectListItem item;
            try
            {
                //if (state.Trim() == "-1" || state.Trim() == "0")
                item = new SelectListItem();
                item.Text = "Select District";
                item.Value = "-1";
                districtList.Add(item);

                int i = Convert.ToInt32(state.Trim());
                var q = (from s in dbContext.MASTER_DISTRICT where s.MAST_STATE_CODE == i && s.MAST_DISTRICT_ACTIVE == "Y" orderby s.MAST_DISTRICT_CODE select s).ToList();

                foreach (var itm in q)
                {
                    item = new SelectListItem();
                    item.Text = itm.MAST_DISTRICT_NAME;
                    item.Value = Convert.ToString(itm.MAST_DISTRICT_CODE).Trim();
                    districtList.Add(item);
                }

                return districtList;
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
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

        public int SaveFBCode(PMGSY.Models.Feedback.FBApprovalDetails FbAppr, string C)
        {
            int ret = 0;
            try
            {
                int fId = Convert.ToInt32(C.Trim());
                dbContext = new PMGSY.Models.PMGSYEntities();
                //var q = (from a in dbContext.ADMIN_FEEDBACK where a.FEED_ID == Convert.ToInt32(C) select a);
                PMGSY.Models.ADMIN_FEEDBACK adminFB = dbContext.ADMIN_FEEDBACK.Where(s => s.FEED_ID == fId).FirstOrDefault();

                //PMGSY.Models.ADMIN_FEEDBACK adminFB = new PMGSY.Models.ADMIN_FEEDBACK();
                PMGSY.Common.CommonFunctions comm = new PMGSY.Common.CommonFunctions();

                adminFB.FEED_APPROVAL_DATE = System.DateTime.Now;
                if (FbAppr.Approval == "Y")
                {
                    adminFB.FEED_APPROVAL = "Y";
                    ret = 1;
                }
                else
                {
                    adminFB.FEED_APPROVAL = "N";
                    ret = 2;

                    if (!dbContext.ADMIN_FEEDBACK_REPLY.Where(x => x.FEED_ID == fId && x.REP_ID == 1).Any())
                    {
                        PMGSY.Models.ADMIN_FEEDBACK_REPLY adminFBRep = new PMGSY.Models.ADMIN_FEEDBACK_REPLY();//dbContext.ADMIN_FEEDBACK_REPLY.Where(s => s.FEED_ID == fId).FirstOrDefault();

                        adminFBRep.FEED_ID = fId;
                        adminFBRep.REP_DATE = System.DateTime.Now;
                        adminFBRep.REP_ID = 1;
                        adminFBRep.REP_STATUS = "F";//FbRep.Feed_Reply.Trim();
                        adminFB.FEED_STATUS = "F";
                        if (FbAppr.Rep_ApprComments == null)
                        {
                            adminFBRep.REP_COMMENT = FbAppr.Rep_ApprComments;
                        }
                        else
                        {
                            adminFBRep.REP_COMMENT = FbAppr.Rep_ApprComments.Trim();// == string.Empty ? null : FbAppr.Rep_ApprComments.Trim();
                        }

                        adminFBRep.REP_USER_ID = PMGSYSession.Current.UserId;
                        adminFBRep.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];

                        //dbContext.Entry(adminFBRep).State = System.Data.Entity.EntityState.Modified;
                        dbContext.ADMIN_FEEDBACK_REPLY.Add(adminFBRep);
                        dbContext.SaveChanges();
                    }
                }

                /*Added for Feedback through web*/
                if (FbAppr.CitizenId != null && FbAppr.CitizenId > 0)
                {
                    if (FbAppr.State != null && FbAppr.State > 0)
                    {
                        adminFB.MAST_STATE_CODE = FbAppr.State;
                    }
                    else
                    {
                        adminFB.MAST_STATE_CODE = null;
                    }
                    if (FbAppr.District != null && FbAppr.District > 0)
                    {
                        adminFB.MAST_DISTRICT_CODE = FbAppr.District;
                    }
                    else
                    {
                        adminFB.MAST_DISTRICT_CODE = null;
                    }
                }
                dbContext.Entry(adminFB).State = System.Data.Entity.EntityState.Modified;
                dbContext.SaveChanges();


                return ret;
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                return -1;
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

        #region Feedback Reply Status

        public Array FBRepListtDAL(int? page, int? rows, string sidx, string sord, out Int32 totalRecords, string id)
        {
            //String parameter = null, hash = null, key = null;
            //decryptedParameters = PMGSY.Common.URLEncrypt.DecryptParameters1(new String[] { parameter, hash, key });
            //if (decryptedParameters.Count < 0)
            //{
            //    totalRecords = 0;
            //    return null;
            //}
            int fId = Convert.ToInt32(id.Trim());
            //int formonth=1;
            //int foryear=2014;
            //int state=0;
            //string category="C";
            //string appr = "N";
            //string status="N";
            dbContext = new PMGSY.Models.PMGSYEntities();
            //List<SelectListItem> itemList = new List<SelectListItem>();
            SelectListItem item;
            try
            {
                var max = (from s in dbContext.ADMIN_FEEDBACK_REPLY where (s.FEED_ID == fId) orderby s.FEED_ID select s.REP_ID).DefaultIfEmpty().Max();
                var itemList = (from s in dbContext.ADMIN_FEEDBACK_REPLY where (s.FEED_ID == fId) orderby s.FEED_ID select s).ToList();
                //var itemList = dbContext.ADMIN_FEEDBACK.Where(m => m.FEED_DATE.Month.Equals(1) && m.FEED_DATE.Year.Equals(2014)).ToList();
                totalRecords = itemList.Count();

                if (sidx.Trim() != null)
                {
                    if (sord.ToString() == "desc")
                    {
                        itemList = itemList.OrderBy(x => x.REP_ID).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                    }
                    else
                    {
                        itemList = itemList.OrderByDescending(x => x.REP_ID).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                    }
                }
                else
                {
                    itemList = itemList.OrderBy(x => x.REP_ID).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                }
                //itemList = itemList.OrderByDescending(x => x.REP_DATE).ToList();
                return itemList.Select(itemDetails => new
                {
                    cell = new[] {
                                        itemDetails.REP_ID.ToString().Trim(),
                                        itemDetails.REP_DATE.ToString("dd/MM/yyyy").Trim(),
                                        itemDetails.REP_COMMENT,
                                        itemDetails.REP_STATUS=="N"?"Not Replied":(itemDetails.REP_STATUS=="I")?"Interim Reply":
                                        (itemDetails.REP_STATUS=="F" && itemDetails.UM_User_Master.DefaultRoleID == 22) ? "Final Reply (PIU)" :"Final Reply",                                        //"<a href='#'  class='ui-icon ui-icon-plusthick ui-align-center' onClick='ShowDetails(\""+ itemDetails.FEED_ID.ToString().Trim() +"\"); return false;'>Show Details</a>",
                                        //itemDetails.REP_ID==max && itemDetails.REP_STATUS!="F"?"<a href='#'  class='ui-icon ui-icon-pencil ui-align-center' onClick='UpdateFBRep(\""+ PMGSY.Common.URLEncrypt.EncryptParameters1(new string[] { "FeedCode =" + itemDetails.FEED_ID.ToString().Trim() + "$" + itemDetails.REP_ID.ToString().Trim() }) +"\"); return false;'>Update</a>":"-",
                                        //itemDetails.REP_ID==max && itemDetails.REP_STATUS!="F"?"<a href='#'  class='ui-icon ui-icon-trash ui-align-center' onClick='DeleteFBRep(\""+ PMGSY.Common.URLEncrypt.EncryptParameters1(new string[] { "FeedCode =" +  itemDetails.FEED_ID.ToString().Trim() +"$" + itemDetails.REP_ID.ToString().Trim() }) +"\"); return false;'>Delete</a>":"-",
                                        
                                        ///Edit
                                        /*MRD & CQCAdmin*/
                                        ((itemDetails.ADMIN_FEEDBACK.MAST_BLOCK_CODE!=null && itemDetails.ADMIN_FEEDBACK.MAST_DISTRICT_CODE!=null && itemDetails.ADMIN_FEEDBACK.MAST_STATE_CODE!=null && (PMGSYSession.Current.RoleCode == 25 || PMGSYSession.Current.RoleCode == 5))
                                        /*SQC*/
                                        //|| (itemDetails.ADMIN_FEEDBACK.MAST_STATE_CODE!=null && itemDetails.ADMIN_FEEDBACK.MAST_STATE_CODE !=null && PMGSYSession.Current.RoleCode == 8 && itemDetails.REP_STATUS.Trim() != "F")
                                        /*PIU*/
                                        //|| (itemDetails.ADMIN_FEEDBACK.MAST_DISTRICT_CODE!=null && itemDetails.ADMIN_FEEDBACK.MAST_STATE_CODE==null && PMGSYSession.Current.RoleCode == 22)
                                        )?"-"

                                        : ((itemDetails.REP_ID==max && itemDetails.REP_STATUS!="F") || (PMGSY.Extensions.PMGSYSession.Current.RoleCode == 8 && itemDetails.UM_User_Master.DefaultRoleID == 22 && itemDetails.REP_STATUS.Trim() == "F"))?"<a href='#'  class='ui-icon ui-icon-pencil ui-align-center' onClick='UpdateFBRep(\""+ PMGSY.Common.URLEncrypt.EncryptParameters1(new string[] { "FeedCode =" + itemDetails.FEED_ID.ToString().Trim() + "$" + itemDetails.REP_ID.ToString().Trim() }) +"\"); return false;'>Update</a>"
                                        :"-",
                                        ///Delete
                                        (
                                        /*MRD & CQCAdmin*/
                                        //(itemDetails.ADMIN_FEEDBACK.MAST_BLOCK_CODE!=null && itemDetails.ADMIN_FEEDBACK.MAST_DISTRICT_CODE!=null && itemDetails.ADMIN_FEEDBACK.MAST_STATE_CODE!=null && (PMGSYSession.Current.RoleCode == 25 || PMGSYSession.Current.RoleCode == 5)) || 
                                        /*SQC*/
                                        (itemDetails.ADMIN_FEEDBACK.MAST_STATE_CODE!=null && itemDetails.ADMIN_FEEDBACK.MAST_STATE_CODE !=null && PMGSYSession.Current.RoleCode == 8 /*&& itemDetails.REP_STATUS.Trim() != "F"*/)
                                        /*PIU*/
                                        || (itemDetails.ADMIN_FEEDBACK.MAST_DISTRICT_CODE!=null && itemDetails.ADMIN_FEEDBACK.MAST_STATE_CODE==null && PMGSYSession.Current.RoleCode == 22)
                                        )?"-"
                                        : (/*MRD & CQCAdmin Enabled Deleting for MRD and CQCAdmin*/
                                        ((PMGSYSession.Current.RoleCode == 25 || PMGSYSession.Current.RoleCode == 5) && itemDetails.REP_ID==max && itemDetails.REP_STATUS=="F" ))
                                            ?"<a href='#'  class='ui-icon ui-icon-trash ui-align-center' onClick='DeleteFBRep(\""+ PMGSY.Common.URLEncrypt.EncryptParameters1(new string[] { "FeedCode =" +  itemDetails.FEED_ID.ToString().Trim() +"$" + itemDetails.REP_ID.ToString().Trim() }) +"\"); return false;'>Delete</a>"

                                            : itemDetails.REP_ID==max && itemDetails.REP_STATUS!="F"
                                              ?"<a href='#'  class='ui-icon ui-icon-trash ui-align-center' onClick='DeleteFBRep(\""+ PMGSY.Common.URLEncrypt.EncryptParameters1(new string[] { "FeedCode =" +  itemDetails.FEED_ID.ToString().Trim() +"$" + itemDetails.REP_ID.ToString().Trim() }) +"\"); return false;'>Delete</a>"
                                              :"-",
                               }
                }).ToArray();
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                totalRecords = 0;
                return null;
            }
            finally
            {
                dbContext.Dispose();
            }
            //}
        }



        public bool SaveFBRep(PMGSY.Models.Feedback.FeedbackReply FbRep, string C)
        {
            try
            {
                int repId = 0;
                int fId = Convert.ToInt32(C.Trim());

                dbContext = new PMGSY.Models.PMGSYEntities();
                PMGSY.Models.ADMIN_FEEDBACK_REPLY adminFBRep = new PMGSY.Models.ADMIN_FEEDBACK_REPLY();

                var q = (from f in dbContext.ADMIN_FEEDBACK_REPLY
                         where f.FEED_ID == fId
                         orderby f.FEED_ID
                         select f.REP_ID).DefaultIfEmpty().Max();
                repId = q + 1;

                PMGSY.Common.CommonFunctions comm = new PMGSY.Common.CommonFunctions();

                adminFBRep.FEED_ID = fId;
                adminFBRep.REP_DATE = System.DateTime.Now;
                adminFBRep.REP_ID = repId;
                adminFBRep.REP_STATUS = FbRep.Feed_Reply.Trim();
                if (FbRep.Rep_Comments == null)
                {
                    adminFBRep.REP_COMMENT = FbRep.Rep_Comments;
                }
                else
                {
                    adminFBRep.REP_COMMENT = FbRep.Rep_Comments.Trim();
                }
                adminFBRep.REP_USER_ID = PMGSYSession.Current.UserId;
                adminFBRep.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];

                //dbContext.Entry(adminFBRep).State = System.Data.Entity.EntityState.Modified;
                dbContext.ADMIN_FEEDBACK_REPLY.Add(adminFBRep);
                dbContext.SaveChanges();

                if (FbRep.Feed_Reply.Trim() == "F" || FbRep.Feed_Reply.Trim() == "I")
                {
                    dbContext = new PMGSY.Models.PMGSYEntities();
                    PMGSY.Models.ADMIN_FEEDBACK adminFB = new PMGSY.Models.ADMIN_FEEDBACK();

                    adminFB = dbContext.ADMIN_FEEDBACK.Where(m => m.FEED_ID == fId).FirstOrDefault();
                    if (adminFB != null)
                    {
                        adminFB.FEED_STATUS = FbRep.Feed_Reply.Trim();

                        dbContext.Entry(adminFB).State = System.Data.Entity.EntityState.Modified;
                        dbContext.SaveChanges();
                        return true;
                    }
                    //else
                    //{
                    //    return false;
                    //}
                }
                return true;
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                return false;
            }
            finally
            {
                if (dbContext != null)
                {
                    dbContext.Dispose();
                }
            }
        }

        public bool UpdateFBRep(PMGSY.Models.Feedback.FeedbackReply FbRep, string fid, int repId)
        {
            try
            {
                int feedId = Convert.ToInt32(fid.Trim());
                //int repId = Convert.ToInt32(R.Trim());
                //int fId = Convert.ToInt32(C.Trim());

                dbContext = new PMGSY.Models.PMGSYEntities();
                PMGSY.Models.ADMIN_FEEDBACK_REPLY adminFBRep = new PMGSY.Models.ADMIN_FEEDBACK_REPLY();

                var q = (from f in dbContext.ADMIN_FEEDBACK_REPLY
                         where f.FEED_ID == feedId //&& f.REP_ID == repId
                         orderby f.FEED_ID
                         select f.REP_ID).DefaultIfEmpty().Max();
                if (q == repId)
                {

                    PMGSY.Common.CommonFunctions comm = new PMGSY.Common.CommonFunctions();

                    adminFBRep.FEED_ID = feedId;
                    adminFBRep.REP_DATE = System.DateTime.Now;
                    adminFBRep.REP_ID = repId;
                    adminFBRep.REP_STATUS = FbRep.Feed_Reply.Trim();
                    adminFBRep.REP_COMMENT = FbRep.Rep_Comments;//.Trim();
                    //adminFBRep.REP_USER_ID = PMGSYSession.Current.RoleCode;
                    adminFBRep.REP_USER_ID = PMGSYSession.Current.UserId;
                    adminFBRep.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];

                    //dbContext.Entry(adminFBRep).State = System.Data.Entity.EntityState.Modified;
                    dbContext.Entry(adminFBRep).State = System.Data.Entity.EntityState.Modified;
                    dbContext.SaveChanges();

                    if (FbRep.Feed_Reply.Trim() == "F" || FbRep.Feed_Reply.Trim() == "I")
                    {
                        dbContext = new PMGSY.Models.PMGSYEntities();
                        PMGSY.Models.ADMIN_FEEDBACK adminFB = new PMGSY.Models.ADMIN_FEEDBACK();

                        adminFB = dbContext.ADMIN_FEEDBACK.Where(m => m.FEED_ID == feedId).FirstOrDefault();
                        if (adminFB != null)
                        {
                            adminFB.FEED_STATUS = FbRep.Feed_Reply.Trim();

                            dbContext.Entry(adminFB).State = System.Data.Entity.EntityState.Modified;
                            dbContext.SaveChanges();
                            //return true;
                        }
                        //else
                        //{
                        //    return false;
                        //}
                    }

                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                return false;
            }
            finally
            {
                if (dbContext != null)
                {
                    dbContext.Dispose();
                }
            }
        }

        public bool DeleteFBRep(int feedId, int repId)
        {
            try
            {
                dbContext = new PMGSY.Models.PMGSYEntities();
                PMGSY.Models.ADMIN_FEEDBACK_REPLY adminFBRep = new PMGSY.Models.ADMIN_FEEDBACK_REPLY();
                PMGSY.Models.ADMIN_FEEDBACK adminFB = new PMGSY.Models.ADMIN_FEEDBACK();

                var q = (from f in dbContext.ADMIN_FEEDBACK_REPLY
                         where f.FEED_ID == feedId //&& f.REP_ID == repId
                         orderby f.FEED_ID
                         select f.REP_ID).DefaultIfEmpty().Max();
                if (q == repId)
                {
                    PMGSY.Common.CommonFunctions comm = new PMGSY.Common.CommonFunctions();

                    adminFBRep = dbContext.ADMIN_FEEDBACK_REPLY.Where(m => m.FEED_ID == feedId && m.REP_ID == repId).FirstOrDefault();
                    if (adminFBRep != null)
                    {
                        dbContext.ADMIN_FEEDBACK_REPLY.Remove(adminFBRep);
                        dbContext.SaveChanges();

                        dbContext = new PMGSY.Models.PMGSYEntities();
                        adminFB = dbContext.ADMIN_FEEDBACK.Where(m => m.FEED_ID == feedId).FirstOrDefault();
                        if (adminFB != null)
                        {
                            if (q > 1)
                            {
                                adminFB.FEED_STATUS = "I";
                            }
                            else
                            {
                                adminFB.FEED_STATUS = "N";
                            }
                            dbContext.Entry(adminFB).State = System.Data.Entity.EntityState.Modified;
                            dbContext.SaveChanges();
                        }
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                return false;
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

        #region Advanced Search
        public void searchFBDetailsDAL(ref SearchFeedback sFB)
        {
            string telcode = string.Empty;
            string telNo = string.Empty;
            string details = string.Empty;
            int feedId = 0;
            dbContext = new PMGSY.Models.PMGSYEntities();
            try
            {
                if (sFB.FBToken != null)
                {
                    details = sFB.FBToken.Trim();
                    sFB.searchDetails = "0";
                }
                else
                {
                    if (sFB.searchDetails == "2")
                    {
                        telNo = sFB.contactDetails.Trim();

                    }
                    else
                    {
                        details = sFB.contactDetails.Trim();
                    }
                }

                feedId = sFB.feedbackId;
                var b = (from a in dbContext.ADMIN_FEEDBACK
                         where (a.FEED_ID == feedId
                        )
                         select a).ToList();
                foreach (var itm in b)
                {
                    sFB.feedcomm = itm.FEED_COMMENT;
                    sFB.feedsub = itm.FEED_SUBJECT;
                    sFB.feeddate = itm.FEED_DATE.ToString("dd/MM/yyyy");
                    //feedId = itm.FEED_ID;

                    #region
                    sFB.FState = itm.MAST_STATE_CODE == null ? "-" : itm.MASTER_STATE.MAST_STATE_NAME.Trim();
                    sFB.FDistrict = itm.MAST_DISTRICT_CODE == null ? "-" : itm.MASTER_DISTRICT.MAST_DISTRICT_NAME.Trim();
                    sFB.FBlock = itm.MAST_BLOCK_CODE == null ? "-" : itm.MASTER_BLOCK.MAST_BLOCK_NAME.Trim();
                    sFB.FB_Type = itm.FEED_TYPE == "G" ? "Public" : "";
                    sFB.FName = itm.FEED_NAME.Trim();
                    sFB.FTel = itm.FEED_TELE_CODE + "-" + itm.FEED_TELE_NUMBER;
                    sFB.FMob = itm.FEED_MOBILE;
                    sFB.FEmail = itm.FEED_EMAIL;
                    sFB.FAgainst = itm.MASTER_FEEDBACK_CATEGORY.MAST_FEED_NAME.Trim();
                    sFB.FDate = itm.FEED_DATE.ToString("dd/MM/yyyy");//comm.GetDateTimeToString(itm.FEED_DATE).Trim();

                    sFB.PMGSYRoads = itm.IS_PMGSY_ROAD == null ? "-" : (itm.IS_PMGSY_ROAD.Trim() == "1" ? "Yes" : itm.IS_PMGSY_ROAD.Trim() == "2" ? "No" : "Do Not Know");
                    sFB.RoadName = itm.ROAD_NAME == null ? "-" : itm.ROAD_NAME.Trim();
                    sFB.VillageName = itm.VILLAGE_NAME == null ? "-" : itm.VILLAGE_NAME.Trim();
                    sFB.NearestHabitation = itm.NEAREST_HAB == null ? "-" : itm.NEAREST_HAB.Trim();

                    if (itm.PLAN_CN_ROAD_CODE != null)
                    {
                        sFB.RH_Name = dbContext.PLAN_ROAD.Where(a => a.PLAN_CN_ROAD_CODE == itm.PLAN_CN_ROAD_CODE).Select(a => a.PLAN_RD_NAME).FirstOrDefault();
                    }
                    else if (itm.MAST_HAB_CODE != null)
                    {
                        sFB.RH_Name = dbContext.MASTER_HABITATIONS.Where(a => a.MAST_HAB_CODE == itm.MAST_HAB_CODE).Select(a => a.MAST_HAB_NAME).FirstOrDefault();
                    }
                    switch (itm.FEED_FOR.Trim())
                    {
                        case "R":
                            sFB.FFor = "Road";
                            break;
                        case "H":
                            sFB.FFor = "Habitation";
                            break;
                        case "G":
                            sFB.FFor = "General";
                            break;
                    }
                    sFB.FComments = string.IsNullOrEmpty(itm.FEED_COMMENT) ? "-" : itm.FEED_COMMENT.Trim();
                    switch (itm.FEED_CATEGORY.Trim())
                    {
                        case "C":
                            sFB.FCategory = "Complaint";
                            break;
                        case "F":
                            sFB.FCategory = "Comment";
                            break;
                        case "Q":
                            sFB.FCategory = "Query";
                            break;
                        case "default":
                            sFB.FCategory = "General";
                            break;
                    }
                    #endregion
                }

                var data = (from s in dbContext.ADMIN_FEEDBACK_REPLY
                            where (s.FEED_ID == feedId)
                            select s).ToList();
                foreach (var item in data)
                {
                    sFB.RepStatus = item.REP_STATUS.Trim();
                    if (sFB.statusList == null)
                    {
                        sFB.statusList = new List<PMGSY.Models.Feedback.SearchFeedReply>();
                    }

                    int roleId = 0;
                    roleId = item.UM_User_Master.DefaultRoleID;
                    if (!(sFB.RepStatus == "F" && roleId == 22))
                    {
                        sFB.statusList.Add(new PMGSY.Models.Feedback.SearchFeedReply() { repstat = item.REP_STATUS.Trim(), repdate = item.REP_DATE.ToString("dd/MM/yyyy"), repcomment = string.IsNullOrEmpty(item.REP_COMMENT) ? item.REP_COMMENT : item.REP_COMMENT.Trim() });
                    }
                    else
                    {
                        sFB.RepStatus = null;
                    }
                }
            }
            catch (Exception ex)
            {

            }
            finally
            {
                if (dbContext != null)
                {
                    dbContext.Dispose();
                }
            }
        }

        public void FBListSearchDAL(ref SearchFeedback sFB)
        {
            string telcode = string.Empty;
            string telNo = string.Empty;
            string details = string.Empty;
            string searchDetails = string.Empty;
            string contactDetails = string.Empty;
            int feedId = 0;

            try
            {
                if (sFB.FBToken != null)
                {
                    details = sFB.FBToken.Trim();
                    searchDetails = "0";
                }
                else
                {
                    if (sFB.searchDetails == "2")
                    {
                        telNo = sFB.contactDetails.Trim();
                    }
                    else
                    {
                        details = sFB.contactDetails.Trim();
                    }
                }
                searchDetails = sFB.searchDetails;
                dbContext = new PMGSY.Models.PMGSYEntities();

                int index = 0;
                var b = (from a in dbContext.ADMIN_FEEDBACK
                         where ((searchDetails == "1" ? (a.FEED_MOBILE == details.Trim()) : searchDetails == "2" ? (a.FEED_TELE_NUMBER == telNo.Trim()) : searchDetails == "3" ? (a.FEED_EMAIL == details.Trim())
                         #region Added for Name and Feedback options
 : searchDetails == "4" ? (a.FEED_NAME.Contains(details.Trim())) : searchDetails == "5" ? (a.FEED_COMMENT.Contains(details.Trim()))
                         #endregion
 : (a.FEED_CODE == details.Trim())))
                         select a).ToList();
                foreach (var itm in b)
                {
                    index += 1;
                    sFB.feedbackId = itm.FEED_ID;

                    sFB.feeddate = itm.FEED_DATE.ToString("dd/MM/yyyy");

                    sFB.feedstatus = itm.FEED_APPROVAL == "N" ? "" + itm.FEED_APPROVAL_DATE == "" ? "Not Accepted" : "Not Accepted (" + Convert.ToDateTime(itm.FEED_APPROVAL_DATE).ToString("dd/MM/yyyy").Trim() + ")"
                                                                  : "Accepted (" + Convert.ToDateTime(itm.FEED_APPROVAL_DATE).ToString("dd/MM/yyyy").Trim() + ")";
                    sFB.FComments = itm.FEED_COMMENT ?? "-";
                    if (sFB.feedList == null)
                    {
                        sFB.feedList = new List<PMGSY.Models.Feedback.feedListing>();
                    }
                    sFB.feedList.Add(new PMGSY.Models.Feedback.feedListing()
                    {
                        SrNo = index.ToString().Trim(),
                        name = itm.FEED_NAME.Trim(),
                        feedToken = itm.FEED_CODE.Trim(),
                        fbAgainst = itm.MASTER_FEEDBACK_CATEGORY.MAST_FEED_NAME.Trim(),
                        fbStatus = sFB.feedstatus.Trim(),
                        //feedsubject = string.IsNullOrEmpty(itm.FEED_SUBJECT) ? "--" : itm.FEED_SUBJECT.Trim(),
                        feedComment = sFB.FComments.Trim(),
                        Date = itm.FEED_DATE.ToString("dd/MM/yyyy"),
                        feedId = itm.FEED_ID
                    });
                }
            }
            catch (Exception ex)
            {

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




        #region New Feedback
        public Array FBListDALNew(int? page, int? rows, string sidx, string sord, out Int32 totalRecords, int formonth, int foryear, int state, string category, string appr, string status, string fbthrough)
        {
            if (PMGSYSession.Current.RoleCode == 8 || PMGSYSession.Current.RoleCode == 22)
            {
                state = PMGSYSession.Current.StateCode;
            }

            dbContext = new PMGSY.Models.PMGSYEntities();
            int district = 0;
            if (PMGSYSession.Current.RoleCode == 22)
            {
                district = PMGSYSession.Current.DistrictCode;
            }
            SelectListItem item;
            try
            {
                var itemList = (from s in dbContext.ADMIN_FEEDBACK
                                where (s.FEED_DATE.Month == formonth

                                        && ((foryear == -1 ? 1 : s.FEED_DATE.Year) == (foryear == -1 ? 1 : foryear))
                                        && ((category == "0" ? "1" : s.FEED_CATEGORY) == (category == "0" ? "1" : category))
                                        && ((appr.Trim() == "0" ? "1" : s.FEED_APPROVAL) == (appr.Trim() == "0" ? "1" : appr.Trim()))


                                           && ((PMGSYSession.Current.RoleCode == 25 || PMGSYSession.Current.RoleCode == 5 || PMGSYSession.Current.RoleCode == 22)
                                            ? ((state == 0) ? (1 == 1) : (s.MAST_STATE_CODE == state))
                                            : ((state == 0) ? (s.MAST_STATE_CODE != null) : (s.MAST_STATE_CODE != null && s.MAST_STATE_CODE == state))
                                        )

                                         && ((PMGSYSession.Current.RoleCode == 22) ? (s.MAST_DISTRICT_CODE == district) : (1 == 1))
                                       && ((status == "0") ? "1" : s.FEED_STATUS) == ((status == "0") ? "1" : status)
                                         // && (fbthrough == "0" ? (1 == 1) : (fbthrough == "M" ? (s.CITIZEN_ID != null ): (s.CITIZEN_ID == null)))
                                         // && (fbthrough == "0" ? (1 == 1) : (fbthrough == "M" ? (s.CITIZEN_ID != null) : (s.CITIZEN_ID == null)))
                                         // &&  (fbthrough == "M" ? (s.CITIZEN_ID != null) : (1 == 1))
                                         && ((PMGSYSession.Current.DistrictCode == null || PMGSYSession.Current.DistrictCode == 0) ? 1 == 1 : s.MAST_DISTRICT_CODE == PMGSYSession.Current.DistrictCode)






                                      /*
                                                                            //  && (appr.Trim() == "0" ? 1 == 1 : s.FEED_APPROVAL.Equals(appr.Trim()))

                                                                              && ((PMGSYSession.Current.RoleCode == 25 || PMGSYSession.Current.RoleCode == 5 || PMGSYSession.Current.RoleCode == 22)
                                                                                  ? (state == 0 ? 1 == 1 : s.MAST_STATE_CODE == state)
                                                                                  : (state == 0 ? s.MAST_STATE_CODE != null : (s.MAST_STATE_CODE != null && s.MAST_STATE_CODE == state))
                                                                              )
                                                                         // For PIU
                                                                              && (PMGSYSession.Current.RoleCode == 22 ? (s.MAST_DISTRICT_CODE == district) : (1 == 1))
                                                                              && (status == "0" ? "" : s.FEED_STATUS) == (status == "0" ? "" : status)
                                                                              && (fbthrough == "0" ? 1 == 1 : fbthrough == "M" ? s.CITIZEN_ID != null : s.CITIZEN_ID == null)
                                                                               && (PMGSYSession.Current.DistrictCode == null || PMGSYSession.Current.DistrictCode == 0) ? 1 == 1 : s.MAST_DISTRICT_CODE == PMGSYSession.Current.DistrictCode
                                                                               */
                                      )
                                orderby s.FEED_ID
                                select s).ToList();
                totalRecords = itemList.Count();

                if (sidx.Trim() != null)
                {
                    if (sord.ToString() == "asc")
                    {
                        itemList = itemList.OrderBy(x => x.FEED_ID).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                    }
                    else
                    {
                        itemList = itemList.OrderByDescending(x => x.FEED_ID).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                    }
                }
                else
                {
                    itemList = itemList.OrderBy(x => x.FEED_ID).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                }
                itemList = itemList.OrderBy(x => x.FEED_FOR).ToList();
                return itemList.Select(itemDetails => new
                {
                    cell = new[] {
                                        itemDetails.FEED_ID.ToString().Trim(),
                                        itemDetails.FEED_DATE.ToString("dd/MM/yyyy").Trim(),
                                        itemDetails.FEED_NAME,
                                        itemDetails.FEED_CATEGORY=="F"?"Comment":(itemDetails.FEED_CATEGORY)=="C"?"Complaint":(itemDetails.FEED_CATEGORY)=="Q"?"Query":"General",
                                        itemDetails.FEED_FOR=="H"?"Habitation":itemDetails.FEED_FOR=="R"?"Road":"General",
                                        itemDetails.MASTER_FEEDBACK_CATEGORY.MAST_FEED_NAME,
                                        itemDetails.CITIZEN_ID != null ? "Mobile" : "Web",

                                        itemDetails.FEED_STATUS=="N"?"Not Replied":(itemDetails.FEED_STATUS=="I")?"Interim Reply":(itemDetails.FEED_STATUS=="O")?"Open Reply": (itemDetails.FEED_STATUS=="F" && itemDetails.ADMIN_FEEDBACK_REPLY.Where(x=>x.FEED_ID == itemDetails.FEED_ID).OrderByDescending(x => x.REP_ID).Select(x => x.UM_User_Master.DefaultRoleID).FirstOrDefault() == 22) ? "Final Reply (PIU)" :"Final Reply",


                                        ((itemDetails.IS_PMGSY_ROAD == "1" && itemDetails.FEED_APPROVAL == "N" && !itemDetails.FEED_APPROVAL_DATE.HasValue)
                                        ||
                                          (itemDetails.IS_PMGSY_ROAD == "1" &&
                                          (itemDetails.FEED_STATUS=="F" && itemDetails.ADMIN_FEEDBACK_REPLY.Where(x=>x.FEED_ID == itemDetails.FEED_ID).OrderByDescending(x => x.REP_ID).Select(x => x.UM_User_Master.DefaultRoleID).FirstOrDefault() == 22))
                                        || (itemDetails.IS_PMGSY_ROAD != "1" && itemDetails.FEED_APPROVAL == "Y" && !itemDetails.FEED_APPROVAL_DATE.HasValue))
                                        ? "SQC"
                                        : (itemDetails.IS_PMGSY_ROAD == "1" && itemDetails.FEED_APPROVAL == "Y" && itemDetails.FEED_APPROVAL_DATE.HasValue && itemDetails.FEED_STATUS!="F")
                                        ? "PIU/SQC"
                                        : (itemDetails.IS_PMGSY_ROAD != "1" && itemDetails.FEED_STATUS!="F")
                                        ? "NRRDA"
                                        :"PIU/SQC", //"-"

                                        itemDetails.FEED_APPROVAL=="N"? "" + itemDetails.FEED_APPROVAL_DATE == "" ? "Not Accepted" : "Not Accepted (" + Convert.ToDateTime(itemDetails.FEED_APPROVAL_DATE).ToString("dd/MM/yyyy").Trim() +")"
                                                                      :"Accepted (" + Convert.ToDateTime(itemDetails.FEED_APPROVAL_DATE).ToString("dd/MM/yyyy").Trim() +")",
                                        "<a href='#'  class='ui-icon ui-icon-zoomin ui-align-center' onClick='ShowDetails(\""+ PMGSY.Common.URLEncrypt.EncryptParameters1(new string[] { "FeedCode =" +  itemDetails.FEED_ID.ToString().Trim()}) +"\"); return false;'>Show Details</a>"

                               }
                }).ToArray();
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                totalRecords = 0;
                return null;
            }
            finally
            {
                dbContext.Dispose();
            }
        }

        public Array FBRepListtDALNew(int? page, int? rows, string sidx, string sord, out Int32 totalRecords, string id)
        {

            int fId = Convert.ToInt32(id.Trim());

            dbContext = new PMGSY.Models.PMGSYEntities();

            SelectListItem item;
            try
            {
                if (PMGSYSession.Current.RoleCode == 22 || PMGSYSession.Current.RoleCode == 38 || PMGSYSession.Current.RoleCode == 54)
                {
                    #region PIU


                    var max = (from s in dbContext.ADMIN_FEEDBACK_REPLY where (s.FEED_ID == fId) orderby s.PIU_SQC select s.REP_ID).DefaultIfEmpty().Max();

                    var itemList = (from s in dbContext.ADMIN_FEEDBACK_REPLY where (s.FEED_ID == fId) orderby s.FEED_ID select s).ToList();


                    totalRecords = itemList.Count();

                    if (sidx.Trim() != null)
                    {
                        if (sord.ToString() == "desc")
                        {
                            itemList = itemList.OrderBy(x => x.REP_ID).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                        }
                        else
                        {
                            itemList = itemList.OrderByDescending(x => x.REP_ID).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                        }
                    }
                    else
                    {
                        itemList = itemList.OrderBy(x => x.REP_ID).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                    }

                    return itemList.Select(itemDetails => new
                    {
                        cell = new[] {
                                        itemDetails.REP_ID.ToString().Trim(),
                                        itemDetails.REP_DATE.ToString("dd/MM/yyyy").Trim(),
                                        itemDetails.REP_COMMENT,

                                        // Reply By
                                        itemDetails.PIU_SQC==null ? "PIU":(itemDetails.PIU_SQC.Equals("P")?"PIU":"SQC"),
                                       
                                        // Status
                                        itemDetails.REP_STATUS=="N"?"Not Replied":(itemDetails.REP_STATUS=="I")?"Interim Reply":(itemDetails.REP_STATUS=="F" && itemDetails.UM_User_Master.DefaultRoleID == 22) ? "Final Reply (PIU)" :(itemDetails.REP_STATUS=="F" && itemDetails.UM_User_Master.DefaultRoleID == 38) ? "Final Reply (PIUOA)":(itemDetails.REP_STATUS=="F" && itemDetails.UM_User_Master.DefaultRoleID == 54) ? "Final Reply (PIURCPLWE)":"Final Reply",




                                        itemDetails.PIU_SQC==null?"-":((itemDetails.REP_STATUS=="I" && itemDetails.PIU_SQC.Equals("P") && itemDetails.TENTATIVE_DATE!=null)?(itemDetails.TENTATIVE_DATE.ToString().Substring(0,10)) :"-"),

                                        itemDetails.REP_STATUS=="F"?(string.IsNullOrEmpty(itemDetails.IS_ACTION_TAKEN)?"--":(itemDetails.IS_ACTION_TAKEN=="Y"?"Yes":"No") ):"-",



                                        // Image
                                        itemDetails.PIU_SQC==null?"NA":itemDetails.PIU_SQC=="S"?"<a href='#' title='Click here to View / Upload Image' class='ui-icon ui-icon-zoomin ui-align-center' onClick=UploadPhotoBySQC('" +PMGSY.Common.URLEncrypt.EncryptParameters1(new string[] { "FeedCode =" +  itemDetails.FEED_ID.ToString().Trim() +"$" + itemDetails.REP_ID.ToString().Trim()  })+"'); return false;'>Upload Photo By SQC</a>":
                                       ("<a href='#' title='Click here to View / Upload Image' class='ui-icon ui-icon-image ui-align-center' onClick=UploadPhotoByPIU('" +PMGSY.Common.URLEncrypt.EncryptParameters1(new string[] { "FeedCode =" +  itemDetails.FEED_ID.ToString().Trim() +"$" + itemDetails.REP_ID.ToString().Trim()  })+"'); return false;'>Upload Photo By PIU</a>"),



                                       // PDF
                                           itemDetails.PIU_SQC==null?"NA":itemDetails.PIU_SQC=="S"?"<a href='#' title='Click here to View / Upload PDF' class='ui-icon ui-icon-zoomin ui-align-center' onClick=UploadPDFBySQC('" +PMGSY.Common.URLEncrypt.EncryptParameters1(new string[] { "FeedCode =" +  itemDetails.FEED_ID.ToString().Trim() +"$" + itemDetails.REP_ID.ToString().Trim()  })+"'); return false;'>Upload PDF By SQC</a>":
                                       ("<a href='#' title='Click here to View / Upload PDF' class='ui-icon ui-icon-plusthick ui-align-center' onClick=UploadPDFByPIU('" +PMGSY.Common.URLEncrypt.EncryptParameters1(new string[] { "FeedCode =" +  itemDetails.FEED_ID.ToString().Trim() +"$" + itemDetails.REP_ID.ToString().Trim()  })+"'); return false;'>Upload PDF By PIU</a>"),





                                      (itemDetails.PIU_SQC==null && itemDetails.IS_FORWARD_TO_SQC==null)?"<a href='#' title='Click here to forward reply to SQC' class='ui-icon ui-icon-plusthick ui-align-center' onClick=ForwardToSQC('" +PMGSY.Common.URLEncrypt.EncryptParameters1(new string[] { "FeedCode =" +  itemDetails.FEED_ID.ToString().Trim() +"$" + itemDetails.REP_ID.ToString().Trim()  })+"'); return false;'>Forward To SQC</a>":
                                      ((itemDetails.PIU_SQC!=null && itemDetails.PIU_SQC=="S")?"-":  (itemDetails.PIU_SQC==null?"NA":dbContext.ADMIN_FEEDBACK_REPLY.Any(m=>m.IS_FORWARD_TO_SQC!=null && m.IS_FORWARD_TO_SQC=="Y" && m.FEED_ID==itemDetails.FEED_ID && m.REP_ID==itemDetails.REP_ID)?(itemDetails.PIU_SQC=="P"?"Forwarded to SQC":"Reply From SQC"): "<a href='#' title='Click here to forward reply to SQC' class='ui-icon ui-icon-plusthick ui-align-center' onClick=ForwardToSQC('" +PMGSY.Common.URLEncrypt.EncryptParameters1(new string[] { "FeedCode =" +  itemDetails.FEED_ID.ToString().Trim() +"$" + itemDetails.REP_ID.ToString().Trim()  })+"'); return false;'>Forward To SQC</a>")),












                                         ///Edit
                                         (itemDetails.PIU_SQC!=null && itemDetails.PIU_SQC=="S")?"-":(itemDetails.PIU_SQC==null?"NA":
                                        dbContext.ADMIN_FEEDBACK_REPLY.Any(m=>m.IS_FORWARD_TO_SQC!=null && m.IS_FORWARD_TO_SQC=="Y" && m.FEED_ID==itemDetails.FEED_ID && m.REP_ID==itemDetails.REP_ID)?"--":(


                                        ((itemDetails.ADMIN_FEEDBACK.MAST_BLOCK_CODE!=null && itemDetails.ADMIN_FEEDBACK.MAST_DISTRICT_CODE!=null && itemDetails.ADMIN_FEEDBACK.MAST_STATE_CODE!=null && (PMGSYSession.Current.RoleCode == 25 || PMGSYSession.Current.RoleCode == 5))
                                        )?"-"

                                        : ((itemDetails.REP_ID==max && itemDetails.REP_STATUS!="F") || (PMGSY.Extensions.PMGSYSession.Current.RoleCode == 8 && (itemDetails.UM_User_Master.DefaultRoleID == 22 || itemDetails.UM_User_Master.DefaultRoleID == 38 || itemDetails.UM_User_Master.DefaultRoleID == 54) && itemDetails.REP_STATUS.Trim() == "F"))?"<a href='#'  class='ui-icon ui-icon-pencil ui-align-center' onClick='UpdateFBRep(\""+ PMGSY.Common.URLEncrypt.EncryptParameters1(new string[] { "FeedCode =" + itemDetails.FEED_ID.ToString().Trim() + "$" + itemDetails.REP_ID.ToString().Trim() }) +"\"); return false;'>Update</a>"
                                        :"-")),





                                        ///Delete
                                        ///
                                        (itemDetails.PIU_SQC!=null && itemDetails.PIU_SQC=="S")?"-":  (itemDetails.PIU_SQC==null?"NA":
                                          dbContext.ADMIN_FEEDBACK_REPLY.Any(m=>m.IS_FORWARD_TO_SQC!=null && m.IS_FORWARD_TO_SQC=="Y" && m.FEED_ID==itemDetails.FEED_ID && m.REP_ID==itemDetails.REP_ID)?"--":(
                                        (

                                        (itemDetails.ADMIN_FEEDBACK.MAST_STATE_CODE!=null && itemDetails.ADMIN_FEEDBACK.MAST_STATE_CODE !=null && PMGSYSession.Current.RoleCode == 8 /*&& itemDetails.REP_STATUS.Trim() != "F"*/)

                                        || (itemDetails.ADMIN_FEEDBACK.MAST_DISTRICT_CODE!=null && itemDetails.ADMIN_FEEDBACK.MAST_STATE_CODE==null && (PMGSYSession.Current.RoleCode == 22 || PMGSYSession.Current.RoleCode == 38 || PMGSYSession.Current.RoleCode == 54))
                                        )?"-"
                                        : (
                                        ((PMGSYSession.Current.RoleCode == 25 || PMGSYSession.Current.RoleCode == 5) && itemDetails.REP_ID==max && itemDetails.REP_STATUS=="F" ))
                                            ?"<a href='#'  class='ui-icon ui-icon-trash ui-align-center' onClick='DeleteFBRep(\""+ PMGSY.Common.URLEncrypt.EncryptParameters1(new string[] { "FeedCode =" +  itemDetails.FEED_ID.ToString().Trim() +"$" + itemDetails.REP_ID.ToString().Trim() }) +"\"); return false;'>Delete</a>"

                                            : itemDetails.REP_ID==max && itemDetails.REP_STATUS!="F"
                                              ?"<a href='#'  class='ui-icon ui-icon-trash ui-align-center' onClick='DeleteFBRep(\""+ PMGSY.Common.URLEncrypt.EncryptParameters1(new string[] { "FeedCode =" +  itemDetails.FEED_ID.ToString().Trim() +"$" + itemDetails.REP_ID.ToString().Trim() }) +"\"); return false;'>Delete</a>"
                                              :"-")),








                                        // SQC Method Starts  This is hidden at PIU
                                              
                                       "<a href='#' title='Click here to add replay.' class='ui-icon ui-icon-plusthick ui-align-center' onClick=AddReplayBySQC('" +PMGSY.Common.URLEncrypt.EncryptParameters1(new string[] { "FeedCode =" +  itemDetails.FEED_ID.ToString().Trim() +"$" + itemDetails.REP_ID.ToString().Trim()  })+"'); return false;'>Upload PDF By PIU</a>",








                               }
                    }).ToArray();


                    #endregion PIU Ends here
                }
                else
                {
                    #region SQC


                    var max = (from s in dbContext.ADMIN_FEEDBACK_REPLY where (s.FEED_ID == fId) orderby s.PIU_SQC select s.REP_ID).DefaultIfEmpty().Max();

                    var itemList = (from s in dbContext.ADMIN_FEEDBACK_REPLY where (s.FEED_ID == fId && s.IS_FORWARD_TO_SQC != null && s.IS_FORWARD_TO_SQC == "Y") orderby s.FEED_ID select s).ToList();


                    totalRecords = itemList.Count();

                    if (sidx.Trim() != null)
                    {
                        if (sord.ToString() == "desc")
                        {
                            itemList = itemList.OrderBy(x => x.REP_ID).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                        }
                        else
                        {
                            itemList = itemList.OrderByDescending(x => x.REP_ID).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                        }
                    }
                    else
                    {
                        itemList = itemList.OrderBy(x => x.REP_ID).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                    }

                    return itemList.Select(itemDetails => new
                    {
                        cell = new[] {
                                        itemDetails.REP_ID.ToString().Trim(),
                                        itemDetails.REP_DATE.ToString("dd/MM/yyyy").Trim(),
                                        itemDetails.REP_COMMENT,


                                        itemDetails.PIU_SQC==null ? "PIU":(itemDetails.PIU_SQC.Equals("P")?"PIU":"SQC"),


                                        itemDetails.REP_STATUS=="N"?"Not Replied":(itemDetails.REP_STATUS=="I")?"Interim Reply":(itemDetails.REP_STATUS=="F" && itemDetails.UM_User_Master.DefaultRoleID == 22) ? "Final Reply (PIU)" :(itemDetails.REP_STATUS=="F" && itemDetails.UM_User_Master.DefaultRoleID == 38) ? "Final Reply (PIUOA)":(itemDetails.REP_STATUS=="F" && itemDetails.UM_User_Master.DefaultRoleID == 54) ? "Final Reply (PIURCPLWE)":"Final Reply",



                                        itemDetails.PIU_SQC==null?"-":((itemDetails.REP_STATUS=="I" && itemDetails.PIU_SQC.Equals("P"))?(itemDetails.TENTATIVE_DATE==null?"NA":itemDetails.TENTATIVE_DATE.ToString().Substring(0,10)):"-"),

                                        itemDetails.REP_STATUS=="F"?(string.IsNullOrEmpty(itemDetails.IS_ACTION_TAKEN)?"--":(itemDetails.IS_ACTION_TAKEN=="Y"?"Yes":"No") ):"-",




                                        // Image
                                        itemDetails.PIU_SQC==null?"NA":itemDetails.PIU_SQC=="S"?"<a href='#' title='Click here to View / Upload Image' class='ui-icon ui-icon-image ui-align-center' onClick=UploadPhotoBySQC('" +PMGSY.Common.URLEncrypt.EncryptParameters1(new string[] { "FeedCode =" +  itemDetails.FEED_ID.ToString().Trim() +"$" + itemDetails.REP_ID.ToString().Trim()  })+"'); return false;'>Upload Photo By SQC</a>":

                                        ("<a href='#' title='Click here to View / Upload Image' class='ui-icon ui-icon-zoomin ui-align-center' onClick=UploadPhotoByPIU('" +PMGSY.Common.URLEncrypt.EncryptParameters1(new string[] { "FeedCode =" +  itemDetails.FEED_ID.ToString().Trim() +"$" + itemDetails.REP_ID.ToString().Trim()  })+"'); return false;'>Upload Photo By PIU</a>"),



                                        // PDF
                                         itemDetails.PIU_SQC==null?"NA":itemDetails.PIU_SQC=="S"?"<a href='#' title='Click here to View / Upload PDF' class='ui-icon ui-icon-plusthick ui-align-center' onClick=UploadPDFBySQC('" +PMGSY.Common.URLEncrypt.EncryptParameters1(new string[] { "FeedCode =" +  itemDetails.FEED_ID.ToString().Trim() +"$" + itemDetails.REP_ID.ToString().Trim()  })+"'); return false;'>Upload PDF By SQC</a>":

                                         ("<a href='#' title='Click here to View / Upload PDF' class='ui-icon ui-icon-zoomin ui-align-center' onClick=UploadPDFByPIU('" +PMGSY.Common.URLEncrypt.EncryptParameters1(new string[] { "FeedCode =" +  itemDetails.FEED_ID.ToString().Trim() +"$" + itemDetails.REP_ID.ToString().Trim()  })+"'); return false;'>Upload PDF By PIU</a>"),



                                         // SQC Forward
                                         itemDetails.PIU_SQC==null?"NA":
                                        dbContext.ADMIN_FEEDBACK_REPLY.Any(m=>m.IS_FORWARD_TO_SQC!=null && m.IS_FORWARD_TO_SQC=="Y" && m.FEED_ID==itemDetails.FEED_ID && m.REP_ID==itemDetails.REP_ID)?(itemDetails.PIU_SQC=="P"?"Forwarded to SQC":"Reply From SQC"): "<a href='#' title='Click here to forward reply to SQC' class='ui-icon ui-icon-plusthick ui-align-center' onClick=ForwardToSQC('" +PMGSY.Common.URLEncrypt.EncryptParameters1(new string[] { "FeedCode =" +  itemDetails.FEED_ID.ToString().Trim() +"$" + itemDetails.REP_ID.ToString().Trim()  })+"'); return false;'>Forward To SQC</a>",



                                        // Edit
                                         itemDetails.PIU_SQC==null?"NA":
                                        dbContext.ADMIN_FEEDBACK_REPLY.Any(m=>m.IS_FORWARD_TO_SQC!=null && m.IS_FORWARD_TO_SQC=="Y" && m.FEED_ID==itemDetails.FEED_ID && m.REP_ID==itemDetails.REP_ID)?"--":(

                                        ((itemDetails.ADMIN_FEEDBACK.MAST_BLOCK_CODE!=null && itemDetails.ADMIN_FEEDBACK.MAST_DISTRICT_CODE!=null && itemDetails.ADMIN_FEEDBACK.MAST_STATE_CODE!=null && (PMGSYSession.Current.RoleCode == 25 || PMGSYSession.Current.RoleCode == 5))

                                        )?"-"

                                        : ((itemDetails.REP_ID==max && itemDetails.REP_STATUS!="F") || (PMGSY.Extensions.PMGSYSession.Current.RoleCode == 8 && (itemDetails.UM_User_Master.DefaultRoleID == 22 || itemDetails.UM_User_Master.DefaultRoleID == 38 || itemDetails.UM_User_Master.DefaultRoleID == 54)&& itemDetails.REP_STATUS.Trim() == "F"))?"<a href='#'  class='ui-icon ui-icon-pencil ui-align-center' onClick='UpdateFBRep(\""+ PMGSY.Common.URLEncrypt.EncryptParameters1(new string[] { "FeedCode =" + itemDetails.FEED_ID.ToString().Trim() + "$" + itemDetails.REP_ID.ToString().Trim() }) +"\"); return false;'>Update</a>"
                                        :"-"),





                                        ///Delete
                                        ///
                                          itemDetails.PIU_SQC==null?"NA":
                                          dbContext.ADMIN_FEEDBACK_REPLY.Any(m=>m.IS_FORWARD_TO_SQC!=null && m.IS_FORWARD_TO_SQC=="Y" && m.FEED_ID==itemDetails.FEED_ID && m.REP_ID==itemDetails.REP_ID)?"--":(
                                        (

                                        (itemDetails.ADMIN_FEEDBACK.MAST_STATE_CODE!=null && itemDetails.ADMIN_FEEDBACK.MAST_STATE_CODE !=null && PMGSYSession.Current.RoleCode == 8 /*&& itemDetails.REP_STATUS.Trim() != "F"*/)

                                        || (itemDetails.ADMIN_FEEDBACK.MAST_DISTRICT_CODE!=null && itemDetails.ADMIN_FEEDBACK.MAST_STATE_CODE==null && (PMGSYSession.Current.RoleCode == 22 || PMGSYSession.Current.RoleCode == 38 || PMGSYSession.Current.RoleCode == 54 ))
                                        )?"-"
                                        : (
                                        ((PMGSYSession.Current.RoleCode == 25 || PMGSYSession.Current.RoleCode == 5) && itemDetails.REP_ID==max && itemDetails.REP_STATUS=="F" ))
                                            ?"<a href='#'  class='ui-icon ui-icon-trash ui-align-center' onClick='DeleteFBRep(\""+ PMGSY.Common.URLEncrypt.EncryptParameters1(new string[] { "FeedCode =" +  itemDetails.FEED_ID.ToString().Trim() +"$" + itemDetails.REP_ID.ToString().Trim() }) +"\"); return false;'>Delete</a>"

                                            : itemDetails.REP_ID==max && itemDetails.REP_STATUS!="F"
                                              ?"<a href='#'  class='ui-icon ui-icon-trash ui-align-center' onClick='DeleteFBRep(\""+ PMGSY.Common.URLEncrypt.EncryptParameters1(new string[] { "FeedCode =" +  itemDetails.FEED_ID.ToString().Trim() +"$" + itemDetails.REP_ID.ToString().Trim() }) +"\"); return false;'>Delete</a>"
                                              :"-"),








                                        // SQC Method Starts
                                              
                                      (itemDetails.PIU_SQC!=null && itemDetails.PIU_SQC=="S")?"NA":
                                      (dbContext.ADMIN_FEEDBACK_REPLY.Any(m=>m.PIU_SQC!=null && m.PIU_SQC=="S" &&  m.REP_STATUS=="F" && m.FEED_ID==itemDetails.FEED_ID && m.REP_ID==itemDetails.REP_ID)
                                      ?"-":
                                      ("<a href='#' title='Click here to add replay.' class='ui-icon ui-icon-plusthick ui-align-center' onClick=AddReplayBySQC('" +PMGSY.Common.URLEncrypt.EncryptParameters1(new string[] { "FeedCode =" +  itemDetails.FEED_ID.ToString().Trim() +"$" + itemDetails.REP_ID.ToString().Trim()  })+"'); return false;'>Upload PDF By PIU</a>")),




                                     // Delete by SQC Starts
                                              
                                     dbContext.ADMIN_FEEDBACK_REPLY.Any(m=>m.PIU_SQC!=null && m.PIU_SQC=="S" &&  (m.SQC_FINALIZED=="N"|| m.SQC_FINALIZED==null ) && m.FEED_ID==itemDetails.FEED_ID && m.REP_ID==itemDetails.REP_ID)?"<a href='#' title='Click here to delete details.' class='ui-icon ui-icon-trash ui-align-center' onClick=DelBySQC('" +PMGSY.Common.URLEncrypt.EncryptParameters1(new string[] { "FeedCode =" +  itemDetails.FEED_ID.ToString().Trim() +"$" + itemDetails.REP_ID.ToString().Trim()  })+"'); return false;'>Finalize By SQC</a>":"-",


                                      

                                         // Finalize By SQC
                                       
                                         dbContext.ADMIN_FEEDBACK_REPLY.Any(m=>m.PIU_SQC!=null && m.PIU_SQC=="S" && m.FEED_ID==itemDetails.FEED_ID && m.REP_ID==itemDetails.REP_ID && (m.SQC_FINALIZED==null || m.SQC_FINALIZED=="N") ) ?"<a href='#' title='Click here to Finalize reply' class='ui-icon ui-icon-plusthick ui-align-center' onClick=FinalizeBySQC('" +PMGSY.Common.URLEncrypt.EncryptParameters1(new string[] { "FeedCode =" +  itemDetails.FEED_ID.ToString().Trim() +"$" + itemDetails.REP_ID.ToString().Trim()  })+"'); return false;'>Finalize By SQC</a>" : (dbContext.ADMIN_FEEDBACK_REPLY.Any(m=>m.PIU_SQC!=null && m.PIU_SQC=="S" && m.FEED_ID==itemDetails.FEED_ID && m.REP_ID==itemDetails.REP_ID && m.SQC_FINALIZED!=null && m.SQC_FINALIZED=="Y")?"Replied to complainer.":"-")


                                     //   dbContext.ADMIN_FEEDBACK_REPLY.Any(m=>m.IS_FORWARD_TO_SQC!=null && m.IS_FORWARD_TO_SQC=="Y" && m.FEED_ID==itemDetails.FEED_ID && m.REP_ID==itemDetails.REP_ID)?(itemDetails.PIU_SQC=="P"?"Forwarded to SQC":"Reply From SQC"): "<a href='#' title='Click here to forward reply to SQC' class='ui-icon ui-icon-plusthick ui-align-center' onClick=ForwardToSQC('" +PMGSY.Common.URLEncrypt.EncryptParameters1(new string[] { "FeedCode =" +  itemDetails.FEED_ID.ToString().Trim() +"$" + itemDetails.REP_ID.ToString().Trim()  })+"'); return false;'>Forward To SQC</a>",

                               }
                    }).ToArray();


                    #endregion PIU Ends here
                }
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                totalRecords = 0;
                return null;
            }
            finally
            {
                dbContext.Dispose();
            }
            //}
        }

        public bool SaveFBRepNew(PMGSY.Models.Feedback.FeedbackReply FbRep, string C)
        {
            try
            {
                int repId = 0; // REP_ID
                int fId = Convert.ToInt32(C.Trim()); // FEED_ID

                dbContext = new PMGSY.Models.PMGSYEntities();
                PMGSY.Models.ADMIN_FEEDBACK_REPLY adminFBRep = new PMGSY.Models.ADMIN_FEEDBACK_REPLY();

                var q = (from f in dbContext.ADMIN_FEEDBACK_REPLY
                         where f.FEED_ID == fId
                         orderby f.FEED_ID
                         select f.REP_ID).DefaultIfEmpty().Max();
                repId = q + 1;

                PMGSY.Common.CommonFunctions comm = new PMGSY.Common.CommonFunctions();

                adminFBRep.FEED_ID = fId;
                adminFBRep.REP_DATE = System.DateTime.Now;
                adminFBRep.REP_ID = repId;
                adminFBRep.REP_STATUS = FbRep.Feed_Reply.Trim();



                if (adminFBRep.REP_STATUS.Equals("F"))
                {

                    if (PMGSYSession.Current.RoleCode == 22 || PMGSYSession.Current.RoleCode == 38 || PMGSYSession.Current.RoleCode == 54)
                    {// If PIU and If Final Reply
                        adminFBRep.IS_ACTION_TAKEN = FbRep.Is_Action_Taken.Trim();
                    }


                }
                else
                {

                    adminFBRep.TENTATIVE_DATE = Convert.ToDateTime(FbRep.TIMELINE_DATE);


                }



                if (FbRep.Rep_Comments == null)
                {
                    adminFBRep.REP_COMMENT = FbRep.Rep_Comments;
                }
                else
                {
                    adminFBRep.REP_COMMENT = FbRep.Rep_Comments.Trim();
                }
                adminFBRep.PIU_SQC = "P";
                adminFBRep.SQC_FINALIZED = "N";


                adminFBRep.REP_USER_ID = PMGSYSession.Current.UserId;
                adminFBRep.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];


                dbContext.ADMIN_FEEDBACK_REPLY.Add(adminFBRep);


                // Srinivasa sir told to do not update status in case of PIU Reply 19 Feb 2021

                //if (FbRep.Feed_Reply.Trim() == "F" || FbRep.Feed_Reply.Trim() == "I")
                //{

                //    PMGSY.Models.ADMIN_FEEDBACK adminFB = new PMGSY.Models.ADMIN_FEEDBACK();

                //    adminFB = dbContext.ADMIN_FEEDBACK.Where(m => m.FEED_ID == fId).FirstOrDefault();

                //    if (adminFB != null)
                //    {
                //        adminFB.FEED_STATUS = FbRep.Feed_Reply.Trim();
                //        dbContext.Entry(adminFB).State = System.Data.Entity.EntityState.Modified;


                //    }

                //}



                dbContext.SaveChanges();
                return true;
            }

            catch (DbEntityValidationException e)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(e, HttpContext.Current);
                return false;
            }

            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                return false;
            }
            finally
            {
                if (dbContext != null)
                {
                    dbContext.Dispose();
                }
            }
        }

        public bool UpdateFBRepNew(PMGSY.Models.Feedback.FeedbackReply FbRep, string fid, int repId)
        {
            try
            {
                int feedId = Convert.ToInt32(fid.Trim());
                //int repId = Convert.ToInt32(R.Trim());
                //int fId = Convert.ToInt32(C.Trim());

                dbContext = new PMGSY.Models.PMGSYEntities();
                PMGSY.Models.ADMIN_FEEDBACK_REPLY adminFBRep = new PMGSY.Models.ADMIN_FEEDBACK_REPLY();

                var q = (from f in dbContext.ADMIN_FEEDBACK_REPLY
                         where f.FEED_ID == feedId //&& f.REP_ID == repId
                         orderby f.FEED_ID
                         select f.REP_ID).DefaultIfEmpty().Max();
                if (q == repId)
                {

                    PMGSY.Common.CommonFunctions comm = new PMGSY.Common.CommonFunctions();

                    adminFBRep.FEED_ID = feedId;
                    adminFBRep.REP_DATE = System.DateTime.Now;
                    adminFBRep.REP_ID = repId;
                    adminFBRep.REP_STATUS = FbRep.Feed_Reply.Trim();
                    adminFBRep.REP_COMMENT = FbRep.Rep_Comments;//.Trim();
                    //adminFBRep.REP_USER_ID = PMGSYSession.Current.RoleCode;
                    adminFBRep.REP_USER_ID = PMGSYSession.Current.UserId;
                    adminFBRep.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                    adminFBRep.PIU_SQC = "P";

                    //dbContext.Entry(adminFBRep).State = System.Data.Entity.EntityState.Modified;
                    dbContext.Entry(adminFBRep).State = System.Data.Entity.EntityState.Modified;
                    dbContext.SaveChanges();

                    if (FbRep.Feed_Reply.Trim() == "F" || FbRep.Feed_Reply.Trim() == "I")
                    {
                        dbContext = new PMGSY.Models.PMGSYEntities();
                        PMGSY.Models.ADMIN_FEEDBACK adminFB = new PMGSY.Models.ADMIN_FEEDBACK();

                        adminFB = dbContext.ADMIN_FEEDBACK.Where(m => m.FEED_ID == feedId).FirstOrDefault();
                        if (adminFB != null)
                        {
                            adminFB.FEED_STATUS = FbRep.Feed_Reply.Trim();

                            dbContext.Entry(adminFB).State = System.Data.Entity.EntityState.Modified;
                            dbContext.SaveChanges();
                            //return true;
                        }
                        //else
                        //{
                        //    return false;
                        //}
                    }

                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                return false;
            }
            finally
            {
                if (dbContext != null)
                {
                    dbContext.Dispose();
                }
            }
        }

        public bool DeleteFBRepNew(int feedId, int repId)
        {
            try
            {
                dbContext = new PMGSY.Models.PMGSYEntities();
                PMGSY.Models.ADMIN_FEEDBACK_REPLY adminFBRep = new PMGSY.Models.ADMIN_FEEDBACK_REPLY();
                PMGSY.Models.ADMIN_FEEDBACK adminFB = new PMGSY.Models.ADMIN_FEEDBACK();


                // Check if Image or PDF File is added or not. If added, then dont allow to delete this record.
                if (dbContext.ADMIN_FEEDBACK_REPLY_FILES.Any(m => m.FEED_ID == feedId && m.REP_ID == repId))
                {
                    return false;
                }




                var q = (from f in dbContext.ADMIN_FEEDBACK_REPLY
                         where f.FEED_ID == feedId //&& f.REP_ID == repId
                         orderby f.FEED_ID
                         select f.REP_ID).DefaultIfEmpty().Max();
                if (q == repId)
                {
                    PMGSY.Common.CommonFunctions comm = new PMGSY.Common.CommonFunctions();

                    adminFBRep = dbContext.ADMIN_FEEDBACK_REPLY.Where(m => m.FEED_ID == feedId && m.REP_ID == repId).FirstOrDefault();
                    if (adminFBRep != null)
                    {
                        dbContext.ADMIN_FEEDBACK_REPLY.Remove(adminFBRep);
                        dbContext.SaveChanges();

                        dbContext = new PMGSY.Models.PMGSYEntities();
                        adminFB = dbContext.ADMIN_FEEDBACK.Where(m => m.FEED_ID == feedId).FirstOrDefault();
                        if (adminFB != null)
                        {
                            if (q > 1)
                            {
                                adminFB.FEED_STATUS = "I";
                            }
                            else
                            {
                                adminFB.FEED_STATUS = "N";
                            }
                            dbContext.Entry(adminFB).State = System.Data.Entity.EntityState.Modified;
                            dbContext.SaveChanges();
                        }
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                return false;
            }
            finally
            {
                if (dbContext != null)
                {
                    dbContext.Dispose();
                }
            }
        }

        public Array GetFilesListDALByPIU(int page, int rows, string sidx, string sord, out int totalRecords, int feedID, int REP_ID)
        {
            try
            {
                PMGSYEntities dbContext = new PMGSYEntities();
                CommonFunctions objCommonFunction = new CommonFunctions();

                List<ADMIN_FEEDBACK_REPLY_FILES> listQMFiles = dbContext.ADMIN_FEEDBACK_REPLY_FILES.Where(p => p.FEED_ID == feedID && p.REP_ID == REP_ID && p.FILE_TYPE == "I" && p.PIU_SQC != null && p.PIU_SQC == "P").ToList();

                IQueryable<ADMIN_FEEDBACK_REPLY_FILES> query = listQMFiles.AsQueryable<ADMIN_FEEDBACK_REPLY_FILES>();
                totalRecords = listQMFiles.Count();


                string VirtualDirectoryUrl = string.Empty;
                string PhysicalPath = string.Empty;
                string VirtualDirectoryUrl_OMMAS4 = string.Empty;

                //  VirtualDirectoryUrl = System.IO.Path.Combine(System.Configuration.ConfigurationManager.AppSettings["MERI_SADAK_FEEDBACK_REPLY_FILE_IMAGE_VIRTUAL_DIR_PATH"], "thumbnails");
                VirtualDirectoryUrl = System.IO.Path.Combine(System.Configuration.ConfigurationManager.AppSettings["MERI_SADAK_FEEDBACK_REPLY_FILE_IMAGE_VIRTUAL_DIR_PATH"]);
                PhysicalPath = System.Configuration.ConfigurationManager.AppSettings["MERI_SADAK_FEEDBACK_REPLY_FILE_IMAGE_PHYSICAL_DIR_PATH"];


                //For self Reference
                File.Exists(System.IO.Path.Combine(PhysicalPath, HttpUtility.UrlEncode("1983635_1.jpg")).ToString().Replace(@"\\", @"//").Replace(@"\", @"/"));

                return query.Select(fileDetails => new
                {
                    id = fileDetails.FILE_ID,
                    cell = new[] {   
                                    // Added for look into OMMAS4 also
                                    (Path.Combine(VirtualDirectoryUrl, HttpUtility.UrlEncode(fileDetails.FILE_NAME.ToString())).ToString().Replace(@"\\",@"//").Replace(@"\",@"/") ),


                                    (fileDetails.PIU_SQC==null ||fileDetails.PIU_SQC==string.Empty)?"-":( fileDetails.PIU_SQC.Equals("P")?"PIU":"SQC" ),


                                       (fileDetails.FILE_DESC==null ||fileDetails.FILE_DESC==string.Empty)?"-":(  fileDetails.FILE_DESC.ToString() ),
                                    //fileDetails.EXEC_RSA_START_CHAINAGE.ToString(),
                                    //fileDetails.EXEC_RSA_END_CHAINAGE.ToString(),
                                    //fileDetails.EXEC_RSA_SAFETY_ISSUE.Trim().ToString(),
                                    //fileDetails.EXEC_RSA_RECOMMENDATION.Trim().ToString(),


                                    //fileDetails.EXEC_RSA_GRADE=="S"?"Satisfactory":( fileDetails.EXEC_RSA_GRADE=="R"?"Required Improvement":"Unsatisfactory"),
                                    //(fileDetails.EXEC_RSA_FILE_DESC==null ||fileDetails.EXEC_RSA_FILE_DESC==string.Empty)?"-":(fileDetails.EXEC_RSA_FILE_DESC.ToString().Trim()),
                                    
                                    
                                    "<a href='#' title='Click here to Download an Image' class='ui-icon ui-icon-arrowthickstop-1-s ui-align-center' onClick=DownLoadImage(\"" + URLEncrypt.EncryptParameters(new string[] { fileDetails.FILE_NAME + "$" + fileDetails.FILE_ID.ToString().Trim() }) +"\"); return false;'>Download</a>" ,
                                    "<a href='#' title='Click here to Delete an Image' class='ui-icon ui-icon-trash ui-align-center' onClick=DeleteFileDetails(\"" + URLEncrypt.EncryptParameters(new string[] { fileDetails.FILE_NAME + "$" + fileDetails.FILE_ID.ToString().Trim() }) +"\"); return false;'>Delete</a>"



                    }
                }).ToArray();
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "FeedbackDAL().GetFilesListDALByPIU");
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

        public string AddFileUploadDetailsBALByPIUDAL(List<PMGSY.Models.Feedback.FileUploadViewModel> model1, int FEED_ID, int REP_ID, string FileName, string desc)
        {

            PMGSY.Models.PMGSYEntities dbContext = new PMGSY.Models.PMGSYEntities();
            PMGSY.Models.ADMIN_FEEDBACK_REPLY_FILES masterModel = dbContext.ADMIN_FEEDBACK_REPLY_FILES.Where(m => m.FEED_ID == FEED_ID && m.REP_ID == REP_ID && m.PIU_SQC == "P" && m.FILE_TYPE == "I").FirstOrDefault(); //new PMGSY.Models.EXEC_RSA_INSPECTION_DETAILS();

            PMGSY.Models.ADMIN_FEEDBACK_REPLY_FILES model = new PMGSY.Models.ADMIN_FEEDBACK_REPLY_FILES();
            try
            {


                if (masterModel != null)
                {
                    foreach (var mod in model1)
                    {
                        masterModel.FILE_NAME = FileName;
                        masterModel.FILE_DESC = desc;

                        model.FILE_UPLOAD_DATE = System.DateTime.Now;
                        model.FILE_LAT = mod.Latitude;
                        model.FILE_LONG = mod.Longitude;

                        dbContext.Entry(masterModel).State = System.Data.Entity.EntityState.Modified;

                        dbContext.SaveChanges();
                        return string.Empty;
                    }
                }
                else
                {
                    foreach (var mod in model1)
                    {
                        int PrimaryKey = dbContext.ADMIN_FEEDBACK_REPLY_FILES.Any() ? dbContext.ADMIN_FEEDBACK_REPLY_FILES.Max(m => m.FILE_ID) + 1 : 1;
                        model.FILE_ID = PrimaryKey;

                        model.FEED_ID = FEED_ID;
                        model.REP_ID = REP_ID;

                        model.PIU_SQC = PMGSYSession.Current.RoleCode == 8 ? "S" : "P";  // 8 For SQC and 22 for PIU
                        model.FILE_TYPE = "I";

                        model.FILE_NAME = FileName;
                        model.FILE_DESC = desc;

                        model.FILE_UPLOAD_DATE = System.DateTime.Now;
                        model.FILE_LAT = mod.Latitude;
                        model.FILE_LONG = mod.Longitude;

                        dbContext.ADMIN_FEEDBACK_REPLY_FILES.Add(model);
                        dbContext.SaveChanges();
                        return string.Empty;
                    }



                }


                return ("Error Occurred While Processing Request.");
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "FeedbackDetails().AddFileUploadDetailsBALByPIUDAL");
                return ("Error Occurred While Processing Request.");
            }
            finally
            {
                if (dbContext != null)
                {
                    dbContext.Dispose();
                }
            }
        }


        public string DeleteFileDetailsByPIUDAL(int FileID)
        {
            try
            {
                PMGSYEntities dbContext = new PMGSYEntities();
                ADMIN_FEEDBACK_REPLY_FILES fileDetails = dbContext.ADMIN_FEEDBACK_REPLY_FILES.Where(m => m.FILE_ID == FileID).FirstOrDefault();

                if (fileDetails != null)
                {
                    dbContext.ADMIN_FEEDBACK_REPLY_FILES.Remove(fileDetails);
                    dbContext.SaveChanges();


                    return string.Empty;

                    //fileDetails.FILE_NAME = "NA";
                    //dbContext.Entry(fileDetails).State = System.Data.Entity.EntityState.Modified;
                    //dbContext.SaveChanges();
                    //return string.Empty;
                }
                else
                {
                    return ("Error Occurred While Processing Your Request.");

                }
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "FeedbackDAL().DeleteFileDetailsByPIUDAL");
                return ("Error Occurred While Processing Your Request.");
            }
            finally
            {
                if (dbContext != null)
                {
                    dbContext.Dispose();
                }
            }
        }
        public bool ForwardToSQCDAL(int feedId, int repId)
        {
            try
            {
                dbContext = new PMGSY.Models.PMGSYEntities();
                PMGSY.Models.ADMIN_FEEDBACK_REPLY adminFBRep = new PMGSY.Models.ADMIN_FEEDBACK_REPLY();


                adminFBRep = dbContext.ADMIN_FEEDBACK_REPLY.Where(m => m.FEED_ID == feedId && m.REP_ID == repId).FirstOrDefault();

                if (adminFBRep.PIU_SQC == null)
                {

                }
                else
                {
                    if (dbContext.ADMIN_FEEDBACK_REPLY.Any(m => m.FEED_ID == feedId && m.REP_ID == repId && m.REP_STATUS == "I" && m.PIU_SQC == "P"))
                    {
                        // For Intrim Reply, File Uploading is Optional.
                    }
                    else
                    {
                        String Is_Action_Taken = dbContext.ADMIN_FEEDBACK_REPLY.Where(x => x.FEED_ID == feedId && x.REP_ID == repId).Select(x => x.IS_ACTION_TAKEN).FirstOrDefault();

                        if (!dbContext.ADMIN_FEEDBACK_REPLY_FILES.Any(m => m.FEED_ID == feedId && m.REP_ID == repId && m.PIU_SQC == "P" && (m.FILE_TYPE == "I" || m.FILE_TYPE == "C")) && Is_Action_Taken == "N")
                        {
                            // For Final Reply with Action NOT Taken, File Uploading is Optional. Added on 15 March 2021

                        }

                        if (!dbContext.ADMIN_FEEDBACK_REPLY_FILES.Any(m => m.FEED_ID == feedId && m.REP_ID == repId && m.PIU_SQC == "P" && (m.FILE_TYPE == "I" || m.FILE_TYPE == "C")) && Is_Action_Taken == "Y")
                        {
                            // Check if Image OR pdf is uploaded or not ,when Action taken is YES and Reply is final. 
                            return false;
                        }

                        //if (!dbContext.ADMIN_FEEDBACK_REPLY_FILES.Any(m => m.FEED_ID == feedId && m.REP_ID == repId && m.PIU_SQC == "P" && m.FILE_TYPE == "C") && Is_Action_Taken == "Y")
                        //{
                        //     Check if PDF is uploaded or not, when Action is taken and Reply is final
                        //    return false;
                        //}
                    }
                }




                if (adminFBRep != null)
                {
                    adminFBRep.IS_FORWARD_TO_SQC = "Y";
                    adminFBRep.FORWARD_TO_SQC_DATE = System.DateTime.Now;
                    dbContext.Entry(adminFBRep).State = System.Data.Entity.EntityState.Modified;
                    dbContext.SaveChanges();
                    return true;
                }
                else
                {
                    return false;
                }

            }
            catch (Exception ex)
            {
                /// Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                return false;
            }
            finally
            {
                if (dbContext != null)
                {
                    dbContext.Dispose();
                }
            }
        }








        #region PDF by PIU
        public Array GetPDFFilesListDALByPIU(int page, int rows, string sidx, string sord, out int totalRecords, int feedID, int REP_ID)
        {
            try
            {
                PMGSYEntities dbContext = new PMGSYEntities();
                CommonFunctions objCommonFunction = new CommonFunctions();

                List<ADMIN_FEEDBACK_REPLY_FILES> listQMFiles = dbContext.ADMIN_FEEDBACK_REPLY_FILES.Where(p => p.FEED_ID == feedID && p.REP_ID == REP_ID && p.FILE_TYPE == "C" && p.PIU_SQC != null && p.PIU_SQC == "P").ToList();

                IQueryable<ADMIN_FEEDBACK_REPLY_FILES> query = listQMFiles.AsQueryable<ADMIN_FEEDBACK_REPLY_FILES>();
                totalRecords = listQMFiles.Count();

                string VirtualDirectoryUrl = string.Empty;
                string PhysicalPath = string.Empty;
                string VirtualDirectoryUrl_OMMAS4 = string.Empty;

                VirtualDirectoryUrl = System.IO.Path.Combine(System.Configuration.ConfigurationManager.AppSettings["MERI_SADAK_FEEDBACK_REPLY_FILE_PDF_VIRTUAL_DIR_PATH"], "thumbnails");
                PhysicalPath = System.Configuration.ConfigurationManager.AppSettings["MERI_SADAK_FEEDBACK_REPLY_FILE_PDF_PHYSICAL_DIR_PATH"];



                //For self Reference
                File.Exists(System.IO.Path.Combine(PhysicalPath, HttpUtility.UrlEncode("1983635_1.jpg")).ToString().Replace(@"\\", @"//").Replace(@"\", @"/"));

                return query.Select(fileDetails => new
                {
                    id = fileDetails.FILE_ID,
                    cell = new[] {   
                                    // Added for look into OMMAS4 also
                                    (Path.Combine(VirtualDirectoryUrl, HttpUtility.UrlEncode(fileDetails.FILE_NAME.ToString())).ToString().Replace(@"\\",@"//").Replace(@"\",@"/") ),
                                    (fileDetails.PIU_SQC==null ||fileDetails.PIU_SQC==string.Empty)?"-":( fileDetails.PIU_SQC.Equals("P")?"PIU":"SQC" ),
                                    (fileDetails.FILE_DESC==null ||fileDetails.FILE_DESC==string.Empty)?"-":(  fileDetails.FILE_DESC.ToString() ),
                                    "<a href='#' title='Click here to Download' class='ui-icon ui-icon-arrowthickstop-1-s ui-align-center' onClick=DownLoadPDF(\"" + URLEncrypt.EncryptParameters(new string[] { fileDetails.FILE_NAME + "$" + fileDetails.FILE_ID.ToString().Trim() }) +"\"); return false;'>Download</a>" ,
                                    "<a href='#' title='Click here to Delete' class='ui-icon ui-icon-trash ui-align-center' onClick=DeletePDFFileDetails(\"" + URLEncrypt.EncryptParameters(new string[] { fileDetails.FILE_NAME + "$" + fileDetails.FILE_ID.ToString().Trim() }) +"\"); return false;'>Delete</a>"

                    }
                }).ToArray();
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "FeedbackDAL().GetPDFFilesListDALByPIU");
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


        public string AddPDFFileUploadDetailsBALByPIUDAL(int FEED_ID, int REP_ID, string FileName, string desc)
        {

            PMGSY.Models.PMGSYEntities dbContext = new PMGSY.Models.PMGSYEntities();
            PMGSY.Models.ADMIN_FEEDBACK_REPLY_FILES masterModel = dbContext.ADMIN_FEEDBACK_REPLY_FILES.Where(m => m.FEED_ID == FEED_ID && m.REP_ID == REP_ID && m.PIU_SQC == "P" && m.FILE_TYPE == "C").FirstOrDefault(); //new PMGSY.Models.EXEC_RSA_INSPECTION_DETAILS();
            PMGSY.Models.ADMIN_FEEDBACK_REPLY_FILES model = new PMGSY.Models.ADMIN_FEEDBACK_REPLY_FILES();

            try
            {
                if (masterModel != null)
                {// Update
                    masterModel.FILE_NAME = FileName;
                    masterModel.FILE_DESC = desc;
                    model.FILE_UPLOAD_DATE = System.DateTime.Now;
                    model.FILE_LAT = null;
                    model.FILE_LONG = null;
                    dbContext.Entry(masterModel).State = System.Data.Entity.EntityState.Modified;
                    dbContext.SaveChanges();
                    return string.Empty;
                }
                else
                {// Add
                    int PrimaryKey = dbContext.ADMIN_FEEDBACK_REPLY_FILES.Any() ? dbContext.ADMIN_FEEDBACK_REPLY_FILES.Max(m => m.FILE_ID) + 1 : 1;
                    model.FILE_ID = PrimaryKey;
                    model.FEED_ID = FEED_ID;
                    model.REP_ID = REP_ID;
                    model.PIU_SQC = PMGSYSession.Current.RoleCode == 8 ? "S" : "P";  // 8 For SQC and 22 for PIU
                    model.FILE_TYPE = "C";
                    model.FILE_NAME = FileName;
                    model.FILE_DESC = desc;
                    model.FILE_UPLOAD_DATE = System.DateTime.Now;
                    model.FILE_LAT = null;
                    model.FILE_LONG = null;
                    dbContext.ADMIN_FEEDBACK_REPLY_FILES.Add(model);
                    dbContext.SaveChanges();
                    return string.Empty;
                }
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "FeedbackDetails().AddPDFFileUploadDetailsBALByPIUDAL");
                return ("Error Occurred While Processing Request.");
            }
            finally
            {
                if (dbContext != null)
                {
                    dbContext.Dispose();
                }
            }
        }


        public string DeletePDFFileDetailsByPIUDAL(int FileID)
        {
            try
            {
                PMGSYEntities dbContext = new PMGSYEntities();
                ADMIN_FEEDBACK_REPLY_FILES fileDetails = dbContext.ADMIN_FEEDBACK_REPLY_FILES.Where(m => m.FILE_ID == FileID).FirstOrDefault();

                if (fileDetails != null)
                {
                    dbContext.ADMIN_FEEDBACK_REPLY_FILES.Remove(fileDetails);
                    dbContext.SaveChanges();

                    //fileDetails.FILE_NAME = "NA";
                    //dbContext.Entry(fileDetails).State = System.Data.Entity.EntityState.Modified;
                    dbContext.SaveChanges();
                    return string.Empty;
                }
                else
                {
                    return ("Error Occurred While Processing Your Request.");

                }
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "FeedbackDAL().DeletePDFFileDetailsByPIUDAL");
                return ("Error Occurred While Processing Your Request.");
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




        #region SQC Methods

        public bool SQCSaveFBRepNew(PMGSY.Models.Feedback.FeedbackReply FbRep, string C)
        {
            try
            {
                int repId = 0; // REP_ID
                int fId = Convert.ToInt32(C.Trim()); // FEED_ID


                dbContext = new PMGSY.Models.PMGSYEntities();
                PMGSY.Models.ADMIN_FEEDBACK_REPLY adminFBRep = new PMGSY.Models.ADMIN_FEEDBACK_REPLY();

                var q = (from f in dbContext.ADMIN_FEEDBACK_REPLY
                         where f.FEED_ID == fId
                         orderby f.FEED_ID
                         select f.REP_ID).DefaultIfEmpty().Max();
                repId = q + 1;

                PMGSY.Common.CommonFunctions comm = new PMGSY.Common.CommonFunctions();

                adminFBRep.FEED_ID = fId;
                adminFBRep.REP_DATE = System.DateTime.Now;
                adminFBRep.REP_ID = repId;
                adminFBRep.REP_STATUS = FbRep.Feed_Reply.Trim();
                if (FbRep.Rep_Comments == null)
                {
                    adminFBRep.REP_COMMENT = FbRep.Rep_Comments;
                }
                else
                {
                    adminFBRep.REP_COMMENT = FbRep.Rep_Comments.Trim();
                }
                adminFBRep.PIU_SQC = "S";
                adminFBRep.SQC_FINALIZED = "N";
                adminFBRep.IS_FORWARD_TO_SQC = "Y";
                adminFBRep.FORWARD_TO_SQC_DATE = System.DateTime.Now;
                adminFBRep.REP_USER_ID = PMGSYSession.Current.UserId;
                adminFBRep.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                //Added on 23 March 2021 by Aditi to internally save action taken on feedback
                adminFBRep.IS_ACTION_TAKEN = dbContext.ADMIN_FEEDBACK_REPLY.Where(x => x.REP_ID == q && x.FEED_ID == fId).Select(x => x.IS_ACTION_TAKEN).FirstOrDefault();

                dbContext.ADMIN_FEEDBACK_REPLY.Add(adminFBRep);

                if (FbRep.Feed_Reply.Trim() == "F" || FbRep.Feed_Reply.Trim() == "I")
                {

                    PMGSY.Models.ADMIN_FEEDBACK adminFB = new PMGSY.Models.ADMIN_FEEDBACK();

                    adminFB = dbContext.ADMIN_FEEDBACK.Where(m => m.FEED_ID == fId).FirstOrDefault();

                    if (adminFB != null)
                    {
                        adminFB.FEED_STATUS = FbRep.Feed_Reply.Trim();
                        dbContext.Entry(adminFB).State = System.Data.Entity.EntityState.Modified;

                    }

                }


                dbContext.SaveChanges();
                return true;
            }

            catch (DbEntityValidationException e)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(e, HttpContext.Current);
                return false;
            }

            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                return false;
            }
            finally
            {
                if (dbContext != null)
                {
                    dbContext.Dispose();
                }
            }
        }



        public bool SQCUpdateFBRepNew(PMGSY.Models.Feedback.FeedbackReply FbRep, string fid, int repId)
        {
            try
            {
                int feedId = Convert.ToInt32(fid.Trim());
                //int repId = Convert.ToInt32(R.Trim());
                //int fId = Convert.ToInt32(C.Trim());

                dbContext = new PMGSY.Models.PMGSYEntities();
                PMGSY.Models.ADMIN_FEEDBACK_REPLY adminFBRep = new PMGSY.Models.ADMIN_FEEDBACK_REPLY();

                var q = (from f in dbContext.ADMIN_FEEDBACK_REPLY
                         where f.FEED_ID == feedId //&& f.REP_ID == repId
                         orderby f.FEED_ID
                         select f.REP_ID).DefaultIfEmpty().Max();
                if (q == repId)
                {

                    PMGSY.Common.CommonFunctions comm = new PMGSY.Common.CommonFunctions();

                    adminFBRep.FEED_ID = feedId;
                    adminFBRep.REP_DATE = System.DateTime.Now;
                    adminFBRep.REP_ID = repId;
                    adminFBRep.REP_STATUS = FbRep.Feed_Reply.Trim();
                    adminFBRep.REP_COMMENT = FbRep.Rep_Comments;//.Trim();
                    //adminFBRep.REP_USER_ID = PMGSYSession.Current.RoleCode;
                    adminFBRep.REP_USER_ID = PMGSYSession.Current.UserId;
                    adminFBRep.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                    adminFBRep.PIU_SQC = "S";
                    adminFBRep.IS_FORWARD_TO_SQC = "Y";
                    adminFBRep.FORWARD_TO_SQC_DATE = System.DateTime.Now;
                    //dbContext.Entry(adminFBRep).State = System.Data.Entity.EntityState.Modified;
                    dbContext.Entry(adminFBRep).State = System.Data.Entity.EntityState.Modified;
                    dbContext.SaveChanges();

                    if (FbRep.Feed_Reply.Trim() == "F" || FbRep.Feed_Reply.Trim() == "I")
                    {
                        dbContext = new PMGSY.Models.PMGSYEntities();
                        PMGSY.Models.ADMIN_FEEDBACK adminFB = new PMGSY.Models.ADMIN_FEEDBACK();

                        adminFB = dbContext.ADMIN_FEEDBACK.Where(m => m.FEED_ID == feedId).FirstOrDefault();
                        if (adminFB != null)
                        {
                            adminFB.FEED_STATUS = FbRep.Feed_Reply.Trim();

                            dbContext.Entry(adminFB).State = System.Data.Entity.EntityState.Modified;
                            dbContext.SaveChanges();
                            //return true;
                        }
                        //else
                        //{
                        //    return false;
                        //}
                    }

                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                return false;
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


        #region SQC  Image
        public Array GetFilesListDALBySQC(int page, int rows, string sidx, string sord, out int totalRecords, int feedID, int REP_ID)
        {
            try
            {
                PMGSYEntities dbContext = new PMGSYEntities();
                CommonFunctions objCommonFunction = new CommonFunctions();

                List<ADMIN_FEEDBACK_REPLY_FILES> listQMFiles = dbContext.ADMIN_FEEDBACK_REPLY_FILES.Where(p => p.FEED_ID == feedID && p.REP_ID == REP_ID && p.FILE_TYPE == "I" && p.PIU_SQC != null && p.PIU_SQC == "S").ToList();

                IQueryable<ADMIN_FEEDBACK_REPLY_FILES> query = listQMFiles.AsQueryable<ADMIN_FEEDBACK_REPLY_FILES>();
                totalRecords = listQMFiles.Count();


                string VirtualDirectoryUrl = string.Empty;
                string PhysicalPath = string.Empty;
                string VirtualDirectoryUrl_OMMAS4 = string.Empty;

                //VirtualDirectoryUrl = System.IO.Path.Combine(System.Configuration.ConfigurationManager.AppSettings["MERI_SADAK_FEEDBACK_REPLY_FILE_IMAGE_VIRTUAL_DIR_PATH"], "thumbnails");
                VirtualDirectoryUrl = System.IO.Path.Combine(System.Configuration.ConfigurationManager.AppSettings["MERI_SADAK_FEEDBACK_REPLY_FILE_IMAGE_VIRTUAL_DIR_PATH"]);
                PhysicalPath = System.Configuration.ConfigurationManager.AppSettings["MERI_SADAK_FEEDBACK_REPLY_FILE_IMAGE_PHYSICAL_DIR_PATH"];


                //For self Reference
                File.Exists(System.IO.Path.Combine(PhysicalPath, HttpUtility.UrlEncode("1983635_1.jpg")).ToString().Replace(@"\\", @"//").Replace(@"\", @"/"));

                return query.Select(fileDetails => new
                {
                    id = fileDetails.FILE_ID,
                    cell = new[] {   
                                    // Added for look into OMMAS4 also
                                    (Path.Combine(VirtualDirectoryUrl, HttpUtility.UrlEncode(fileDetails.FILE_NAME.ToString())).ToString().Replace(@"\\",@"//").Replace(@"\",@"/") ),


                                    (fileDetails.PIU_SQC==null ||fileDetails.PIU_SQC==string.Empty)?"-":( fileDetails.PIU_SQC.Equals("P")?"PIU":"SQC" ),


                                       (fileDetails.FILE_DESC==null ||fileDetails.FILE_DESC==string.Empty)?"-":(  fileDetails.FILE_DESC.ToString() ),
                                    //fileDetails.EXEC_RSA_START_CHAINAGE.ToString(),
                                    //fileDetails.EXEC_RSA_END_CHAINAGE.ToString(),
                                    //fileDetails.EXEC_RSA_SAFETY_ISSUE.Trim().ToString(),
                                    //fileDetails.EXEC_RSA_RECOMMENDATION.Trim().ToString(),


                                    //fileDetails.EXEC_RSA_GRADE=="S"?"Satisfactory":( fileDetails.EXEC_RSA_GRADE=="R"?"Required Improvement":"Unsatisfactory"),
                                    //(fileDetails.EXEC_RSA_FILE_DESC==null ||fileDetails.EXEC_RSA_FILE_DESC==string.Empty)?"-":(fileDetails.EXEC_RSA_FILE_DESC.ToString().Trim()),
                                    
                                    
                                    "<a href='#' title='Click here to Download an Image' class='ui-icon ui-icon-arrowthickstop-1-s ui-align-center' onClick=DownLoadImageSQC(\"" + URLEncrypt.EncryptParameters(new string[] { fileDetails.FILE_NAME + "$" + fileDetails.FILE_ID.ToString().Trim() }) +"\"); return false;'>Download</a>" ,
                                    "<a href='#' title='Click here to Delete an Image' class='ui-icon ui-icon-trash ui-align-center' onClick=DeleteFileDetailsSQC(\"" + URLEncrypt.EncryptParameters(new string[] { fileDetails.FILE_NAME + "$" + fileDetails.FILE_ID.ToString().Trim() }) +"\"); return false;'>Delete</a>"



                    }
                }).ToArray();
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "FeedbackDAL().GetFilesListDALBySQC");
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

        public string AddFileUploadDetailsBALBySQCDAL(List<PMGSY.Models.Feedback.FileUploadViewModel> model1, int FEED_ID, int REP_ID, string FileName, string desc)
        {

            PMGSY.Models.PMGSYEntities dbContext = new PMGSY.Models.PMGSYEntities();
            PMGSY.Models.ADMIN_FEEDBACK_REPLY_FILES masterModel = dbContext.ADMIN_FEEDBACK_REPLY_FILES.Where(m => m.FEED_ID == FEED_ID && m.REP_ID == REP_ID && m.PIU_SQC == "P" && m.FILE_TYPE == "I").FirstOrDefault(); //new PMGSY.Models.EXEC_RSA_INSPECTION_DETAILS();

            PMGSY.Models.ADMIN_FEEDBACK_REPLY_FILES model = new PMGSY.Models.ADMIN_FEEDBACK_REPLY_FILES();



            if (dbContext.ADMIN_FEEDBACK_REPLY_FILES.Any(m => m.FEED_ID == FEED_ID && m.FILE_TYPE == "I" && m.REP_ID == REP_ID && m.PIU_SQC == "S"))
            {
                return "Image details are already uploaded.";
            }

            try
            {


                if (masterModel != null)
                {
                    foreach (var mod in model1)
                    {
                        masterModel.FILE_NAME = FileName;
                        masterModel.FILE_DESC = desc;

                        model.FILE_UPLOAD_DATE = System.DateTime.Now;
                        model.FILE_LAT = mod.Latitude;
                        model.FILE_LONG = mod.Longitude;

                        dbContext.Entry(masterModel).State = System.Data.Entity.EntityState.Modified;

                        dbContext.SaveChanges();
                        return string.Empty;
                    }
                }
                else
                {
                    foreach (var mod in model1)
                    {
                        int PrimaryKey = dbContext.ADMIN_FEEDBACK_REPLY_FILES.Any() ? dbContext.ADMIN_FEEDBACK_REPLY_FILES.Max(m => m.FILE_ID) + 1 : 1;
                        model.FILE_ID = PrimaryKey;

                        model.FEED_ID = FEED_ID;
                        model.REP_ID = REP_ID;

                        model.PIU_SQC = "S";  // 8 For SQC and 22 for PIU
                        model.FILE_TYPE = "I";

                        model.FILE_NAME = FileName;
                        model.FILE_DESC = desc;

                        model.FILE_UPLOAD_DATE = System.DateTime.Now;
                        model.FILE_LAT = mod.Latitude;
                        model.FILE_LONG = mod.Longitude;

                        dbContext.ADMIN_FEEDBACK_REPLY_FILES.Add(model);
                        dbContext.SaveChanges();
                        return string.Empty;
                    }



                }


                return ("Error Occurred While Processing Request.");
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "FeedbackDetails().AddFileUploadDetailsBALBySQCDAL");
                return ("Error Occurred While Processing Request.");
            }
            finally
            {
                if (dbContext != null)
                {
                    dbContext.Dispose();
                }
            }
        }


        public string DeleteFileDetailsBySQCDAL(int FileID)
        {
            try
            {
                PMGSYEntities dbContext = new PMGSYEntities();
                ADMIN_FEEDBACK_REPLY_FILES fileDetails = dbContext.ADMIN_FEEDBACK_REPLY_FILES.Where(m => m.FILE_ID == FileID).FirstOrDefault();

                if (fileDetails != null)
                {
                    dbContext.ADMIN_FEEDBACK_REPLY_FILES.Remove(fileDetails);
                    dbContext.SaveChanges();


                    return string.Empty;

                    //fileDetails.FILE_NAME = "NA";
                    //dbContext.Entry(fileDetails).State = System.Data.Entity.EntityState.Modified;
                    //dbContext.SaveChanges();
                    //return string.Empty;
                }
                else
                {
                    return ("Error Occurred While Processing Your Request.");

                }
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "FeedbackDAL().DeleteFileDetailsBySQCDAL");
                return ("Error Occurred While Processing Your Request.");
            }
            finally
            {
                if (dbContext != null)
                {
                    dbContext.Dispose();
                }
            }
        }

        public string UploadSameImageAsPIUDAL(int feedId, int repId)
        {
            try
            {
                dbContext = new PMGSY.Models.PMGSYEntities();
                PMGSY.Models.ADMIN_FEEDBACK_REPLY_FILES model = new PMGSY.Models.ADMIN_FEEDBACK_REPLY_FILES();

                // Get Image Details from PIU
                PMGSY.Models.ADMIN_FEEDBACK_REPLY_FILES getDetails = dbContext.ADMIN_FEEDBACK_REPLY_FILES.Where(m => m.FEED_ID == feedId && m.FILE_TYPE == "I" && m.PIU_SQC == "P").FirstOrDefault();

                if (dbContext.ADMIN_FEEDBACK_REPLY_FILES.Any(m => m.FEED_ID == feedId && m.FILE_TYPE == "I" && m.REP_ID == repId && m.PIU_SQC == "S"))
                {
                    return "Image details are already uploaded.";
                }



                int PrimaryKey = dbContext.ADMIN_FEEDBACK_REPLY_FILES.Any() ? dbContext.ADMIN_FEEDBACK_REPLY_FILES.Max(m => m.FILE_ID) + 1 : 1;
                model.FILE_ID = PrimaryKey;

                model.FEED_ID = feedId;
                model.REP_ID = repId;

                model.PIU_SQC = "S";  // 8 For SQC and 22 for PIU
                model.FILE_TYPE = "I";

                model.FILE_NAME = getDetails.FILE_NAME;
                model.FILE_DESC = getDetails.FILE_DESC;

                model.FILE_UPLOAD_DATE = System.DateTime.Now;
                model.FILE_LAT = getDetails.FILE_LAT;
                model.FILE_LONG = getDetails.FILE_LONG;

                dbContext.ADMIN_FEEDBACK_REPLY_FILES.Add(model);
                dbContext.SaveChanges();
                return string.Empty;


            }
            catch (Exception ex)
            {
                /// Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                return "Image is not Uploaded.";
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



        #region PDF by SQC
        public Array GetPDFFilesListDALBySQC(int page, int rows, string sidx, string sord, out int totalRecords, int feedID, int REP_ID)
        {
            try
            {
                PMGSYEntities dbContext = new PMGSYEntities();
                CommonFunctions objCommonFunction = new CommonFunctions();

                List<ADMIN_FEEDBACK_REPLY_FILES> listQMFiles = dbContext.ADMIN_FEEDBACK_REPLY_FILES.Where(p => p.FEED_ID == feedID && p.REP_ID == REP_ID && p.FILE_TYPE == "C" && p.PIU_SQC != null && p.PIU_SQC == "S").ToList();

                IQueryable<ADMIN_FEEDBACK_REPLY_FILES> query = listQMFiles.AsQueryable<ADMIN_FEEDBACK_REPLY_FILES>();
                totalRecords = listQMFiles.Count();

                string VirtualDirectoryUrl = string.Empty;
                string PhysicalPath = string.Empty;
                string VirtualDirectoryUrl_OMMAS4 = string.Empty;

                VirtualDirectoryUrl = System.IO.Path.Combine(System.Configuration.ConfigurationManager.AppSettings["MERI_SADAK_FEEDBACK_REPLY_FILE_PDF_VIRTUAL_DIR_PATH"], "thumbnails");
                PhysicalPath = System.Configuration.ConfigurationManager.AppSettings["MERI_SADAK_FEEDBACK_REPLY_FILE_PDF_PHYSICAL_DIR_PATH"];



                //For self Reference
                File.Exists(System.IO.Path.Combine(PhysicalPath, HttpUtility.UrlEncode("1983635_1.jpg")).ToString().Replace(@"\\", @"//").Replace(@"\", @"/"));

                return query.Select(fileDetails => new
                {
                    id = fileDetails.FILE_ID,
                    cell = new[] {   
                                    // Added for look into OMMAS4 also
                                    (Path.Combine(VirtualDirectoryUrl, HttpUtility.UrlEncode(fileDetails.FILE_NAME.ToString())).ToString().Replace(@"\\",@"//").Replace(@"\",@"/") ),


                                    (fileDetails.PIU_SQC==null ||fileDetails.PIU_SQC==string.Empty)?"-":( fileDetails.PIU_SQC.Equals("P")?"PIU":"SQC" ),
                                    (fileDetails.FILE_DESC==null ||fileDetails.FILE_DESC==string.Empty)?"-":(  fileDetails.FILE_DESC.ToString() ),
                                    "<a href='#' title='Click here to Download' class='ui-icon ui-icon-arrowthickstop-1-s ui-align-center' onClick=DownLoadPDFSQC(\"" + URLEncrypt.EncryptParameters(new string[] { fileDetails.FILE_NAME + "$" + fileDetails.FILE_ID.ToString().Trim() }) +"\"); return false;'>Download</a>" ,
                                    "<a href='#' title='Click here to Delete' class='ui-icon ui-icon-trash ui-align-center' onClick=DeletePDFFileDetailsSQC(\"" + URLEncrypt.EncryptParameters(new string[] { fileDetails.FILE_NAME + "$" + fileDetails.FILE_ID.ToString().Trim() }) +"\"); return false;'>Delete</a>"

                    }
                }).ToArray();
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "FeedbackDAL().GetPDFFilesListDALBySQC");
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


        public string AddPDFFileUploadDetailsBALBySQCDAL(int FEED_ID, int REP_ID, string FileName, string desc)
        {

            PMGSY.Models.PMGSYEntities dbContext = new PMGSY.Models.PMGSYEntities();
            PMGSY.Models.ADMIN_FEEDBACK_REPLY_FILES masterModel = dbContext.ADMIN_FEEDBACK_REPLY_FILES.Where(m => m.FEED_ID == FEED_ID && m.REP_ID == REP_ID && m.PIU_SQC == "P" && m.FILE_TYPE == "C").FirstOrDefault(); //new PMGSY.Models.EXEC_RSA_INSPECTION_DETAILS();
            PMGSY.Models.ADMIN_FEEDBACK_REPLY_FILES model = new PMGSY.Models.ADMIN_FEEDBACK_REPLY_FILES();


            // PMGSY.Models.ADMIN_FEEDBACK_REPLY_FILES getDetails = dbContext.ADMIN_FEEDBACK_REPLY_FILES.Where(m => m.FEED_ID == feedId && m.FILE_TYPE == "C" && m.PIU_SQC == "P").FirstOrDefault();

            if (dbContext.ADMIN_FEEDBACK_REPLY_FILES.Any(m => m.FEED_ID == FEED_ID && m.FILE_TYPE == "C" && m.REP_ID == REP_ID && m.PIU_SQC == "S"))
            {
                return "PDF details are already uploaded.";
            }


            try
            {
                if (masterModel != null)
                {// Update
                    masterModel.FILE_NAME = FileName;
                    masterModel.FILE_DESC = desc;
                    model.FILE_UPLOAD_DATE = System.DateTime.Now;
                    model.FILE_LAT = null;
                    model.FILE_LONG = null;
                    dbContext.Entry(masterModel).State = System.Data.Entity.EntityState.Modified;
                    dbContext.SaveChanges();
                    return string.Empty;
                }
                else
                {// Add
                    int PrimaryKey = dbContext.ADMIN_FEEDBACK_REPLY_FILES.Any() ? dbContext.ADMIN_FEEDBACK_REPLY_FILES.Max(m => m.FILE_ID) + 1 : 1;
                    model.FILE_ID = PrimaryKey;
                    model.FEED_ID = FEED_ID;
                    model.REP_ID = REP_ID;
                    model.PIU_SQC = "S"; // 8 For SQC and 22 for PIU
                    model.FILE_TYPE = "C";
                    model.FILE_NAME = FileName;
                    model.FILE_DESC = desc;
                    model.FILE_UPLOAD_DATE = System.DateTime.Now;
                    model.FILE_LAT = null;
                    model.FILE_LONG = null;
                    dbContext.ADMIN_FEEDBACK_REPLY_FILES.Add(model);
                    dbContext.SaveChanges();
                    return string.Empty;
                }
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "FeedbackDetails().AddPDFFileUploadDetailsBALBySQCDAL");
                return ("Error Occurred While Processing Request.");
            }
            finally
            {
                if (dbContext != null)
                {
                    dbContext.Dispose();
                }
            }
        }


        public string DeletePDFFileDetailsBySQCDAL(int FileID)
        {
            try
            {
                PMGSYEntities dbContext = new PMGSYEntities();
                ADMIN_FEEDBACK_REPLY_FILES fileDetails = dbContext.ADMIN_FEEDBACK_REPLY_FILES.Where(m => m.FILE_ID == FileID).FirstOrDefault();

                if (fileDetails != null)
                {
                    dbContext.ADMIN_FEEDBACK_REPLY_FILES.Remove(fileDetails);
                    dbContext.SaveChanges();

                    //fileDetails.FILE_NAME = "NA";
                    //dbContext.Entry(fileDetails).State = System.Data.Entity.EntityState.Modified;
                    dbContext.SaveChanges();
                    return string.Empty;
                }
                else
                {
                    return ("Error Occurred While Processing Your Request.");

                }
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "FeedbackDAL().DeletePDFFileDetailsBySQCDAL");
                return ("Error Occurred While Processing Your Request.");
            }
            finally
            {
                if (dbContext != null)
                {
                    dbContext.Dispose();
                }
            }
        }


        public string UploadSamePDFAsPIUDAL(int feedId, int repId)
        {
            try
            {
                dbContext = new PMGSY.Models.PMGSYEntities();
                PMGSY.Models.ADMIN_FEEDBACK_REPLY_FILES model = new PMGSY.Models.ADMIN_FEEDBACK_REPLY_FILES();

                // Get Image Details from PIU
                PMGSY.Models.ADMIN_FEEDBACK_REPLY_FILES getDetails = dbContext.ADMIN_FEEDBACK_REPLY_FILES.Where(m => m.FEED_ID == feedId && m.FILE_TYPE == "C" && m.PIU_SQC == "P").FirstOrDefault();

                if (dbContext.ADMIN_FEEDBACK_REPLY_FILES.Any(m => m.FEED_ID == feedId && m.FILE_TYPE == "C" && m.REP_ID == repId && m.PIU_SQC == "S"))
                {
                    return "PDF details are already uploaded.";
                }



                int PrimaryKey = dbContext.ADMIN_FEEDBACK_REPLY_FILES.Any() ? dbContext.ADMIN_FEEDBACK_REPLY_FILES.Max(m => m.FILE_ID) + 1 : 1;
                model.FILE_ID = PrimaryKey;

                model.FEED_ID = feedId;
                model.REP_ID = repId;

                model.PIU_SQC = "S";  // 8 For SQC and 22 for PIU
                model.FILE_TYPE = "C";

                model.FILE_NAME = getDetails.FILE_NAME;
                model.FILE_DESC = getDetails.FILE_DESC;

                model.FILE_UPLOAD_DATE = System.DateTime.Now;
                model.FILE_LAT = getDetails.FILE_LAT;
                model.FILE_LONG = getDetails.FILE_LONG;

                dbContext.ADMIN_FEEDBACK_REPLY_FILES.Add(model);
                dbContext.SaveChanges();
                return string.Empty;


            }
            catch (Exception ex)
            {
                /// Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                return "PDF is not Uploaded.";
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

        #region Finalize by SQC, Delete by SQC
        public bool SQCFinalizeDAL(int feedId, int repId)
        {
            try
            {
                dbContext = new PMGSY.Models.PMGSYEntities();
                PMGSY.Models.ADMIN_FEEDBACK_REPLY adminFBRep = new PMGSY.Models.ADMIN_FEEDBACK_REPLY();



                if (dbContext.ADMIN_FEEDBACK_REPLY.Any(m => m.FEED_ID == feedId && m.REP_ID == repId && m.REP_STATUS == "I" && m.PIU_SQC == "S"))
                {
                    // For Intrim Reply, File Uploading is Optional.
                }
                else
                {

                    String Is_Action_Taken = dbContext.ADMIN_FEEDBACK_REPLY.Where(x => x.FEED_ID == feedId && x.REP_ID == repId).Select(x => x.IS_ACTION_TAKEN).FirstOrDefault();

                    if (!dbContext.ADMIN_FEEDBACK_REPLY_FILES.Any(m => m.FEED_ID == feedId && m.REP_ID == repId && m.PIU_SQC == "S" && m.PIU_SQC != null && (m.FILE_TYPE == "I" || m.FILE_TYPE == "C")) && Is_Action_Taken == "N")
                    {
                        // For Final Reply with Action NOT Taken, File Uploading is Optional. Added on 22 March 2021

                    }

                    if (!dbContext.ADMIN_FEEDBACK_REPLY_FILES.Any(m => m.FEED_ID == feedId && m.REP_ID == repId && m.PIU_SQC == "S" && m.PIU_SQC != null && (m.FILE_TYPE == "I" || m.FILE_TYPE == "C")) && Is_Action_Taken == "Y")
                    { // Check if Image or PDF is uploaded or not by SQC When Action Taken is YES
                        return false;
                    }

                    //if (!dbContext.ADMIN_FEEDBACK_REPLY_FILES.Any(m => m.FEED_ID == feedId && m.REP_ID == repId && m.PIU_SQC == "S" && m.PIU_SQC != null && m.FILE_TYPE == "C") && Is_Action_Taken == "Y")
                    //{ // Check if PDF is uploaded or not  by SQC
                    //    return false;
                    //}
                }


                adminFBRep = dbContext.ADMIN_FEEDBACK_REPLY.Where(m => m.FEED_ID == feedId && m.REP_ID == repId).FirstOrDefault();


                if (adminFBRep != null)
                {
                    adminFBRep.SQC_FINALIZED = "Y";
                    //  adminFBRep.FORWARD_TO_SQC_DATE = System.DateTime.Now;
                    dbContext.Entry(adminFBRep).State = System.Data.Entity.EntityState.Modified;
                    dbContext.SaveChanges();
                    return true;
                }
                else
                {
                    return false;
                }

            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "FeedbackDAL().SQCFinalizeDAL");
                return false;
            }
            finally
            {
                if (dbContext != null)
                {
                    dbContext.Dispose();
                }
            }
        }



        public bool DelBySQCDAL(int feedId, int repId)
        {
            try
            {
                dbContext = new PMGSY.Models.PMGSYEntities();
                PMGSY.Models.ADMIN_FEEDBACK_REPLY adminFBRep = new PMGSY.Models.ADMIN_FEEDBACK_REPLY();

                if (dbContext.ADMIN_FEEDBACK_REPLY_FILES.Any(m => m.FEED_ID == feedId && m.REP_ID == repId && m.PIU_SQC == "S" && m.PIU_SQC != null && m.FILE_TYPE == "I"))
                { // Check if Image is uploaded or not by SQC
                    return false;
                }

                if (dbContext.ADMIN_FEEDBACK_REPLY_FILES.Any(m => m.FEED_ID == feedId && m.REP_ID == repId && m.PIU_SQC == "S" && m.PIU_SQC != null && m.FILE_TYPE == "C"))
                { // Check if PDF is uploaded or not  by SQC
                    return false;
                }


                adminFBRep = dbContext.ADMIN_FEEDBACK_REPLY.Where(m => m.FEED_ID == feedId && m.REP_ID == repId).FirstOrDefault();


                if (adminFBRep != null)
                {

                    dbContext.ADMIN_FEEDBACK_REPLY.Remove(adminFBRep);
                    dbContext.SaveChanges();
                    return true;
                }
                else
                {
                    return false;
                }

            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "FeedbackDAL().DelBySQCDAL");
                return false;
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

        #endregion
    }
}