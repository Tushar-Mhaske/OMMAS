using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PMGSY.Extensions;
using System.Web.Mvc;
using System.Data.Entity.Infrastructure;
using System.Data.Entity;
using System.Configuration;
using System.IO;
using PMGSY.Common;
using System.Transactions;
using System.Data.Entity.Core;

namespace PMGSY.DAL.NewsDetails
{
    public class NewsDAL
    {
        PMGSY.Models.PMGSYEntities dbContext = new Models.PMGSYEntities();
        PMGSY.Common.CommonFunctions comm = new PMGSY.Common.CommonFunctions();

        public List<SelectListItem> PopulateNRRDAList(string check)
        {
            PMGSY.Models.PMGSYEntities dbContext = new PMGSY.Models.PMGSYEntities();
            List<SelectListItem> NRRDAList = new List<SelectListItem>();
            SelectListItem item;
            try
            {
                if (check.Trim() == "0")
                {
                    item = new SelectListItem();
                    item.Text = PMGSYSession.Current.UserName.Trim();
                    item.Value = "0";
                    NRRDAList.Add(item);
                }
                else
                {
                    item = new SelectListItem();
                    item.Text = "Select";
                    item.Value = "-1";
                    NRRDAList.Add(item);

                    //var q = (from s in dbContext.MASTER_STATE orderby s.MAST_STATE_NAME select s).ToList();

                    //foreach (var itm in q)
                    //{
                    //    item = new SelectListItem();
                    //    item.Text = itm.MAST_STATE_NAME;
                    //    item.Value = Convert.ToString(itm.MAST_STATE_CODE).Trim();
                    //    NRRDAList.Add(item);
                    //}
                }
                return NRRDAList;
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

        public List<SelectListItem> PopulateNrrdaStates(string state)
        {
            PMGSY.Models.PMGSYEntities dbContext = new PMGSY.Models.PMGSYEntities();
            List<SelectListItem> stateList = new List<SelectListItem>();
            SelectListItem item;
            try
            {
                //if (PMGSYSession.Current.RoleCode == 25)
                //{
                if (state.Trim() == "-1")
                {
                    if (PMGSYSession.Current.RoleCode == 25)
                    {
                        item = new SelectListItem();
                        item.Text = "Select";
                        item.Value = "-1";
                        stateList.Add(item);
                    }
                    else
                    {
                        item = new SelectListItem();
                        item.Text = PMGSYSession.Current.StateName;
                        item.Value = Convert.ToString(PMGSYSession.Current.StateCode).Trim();
                        stateList.Add(item);
                    }
                }
                else
                {
                    item = new SelectListItem();
                    item.Text = "All States";
                    item.Value = "0";
                    stateList.Add(item);

                    var q = (from s in dbContext.MASTER_STATE orderby s.MAST_STATE_NAME select s).ToList();

                    foreach (var itm in q)
                    {
                        item = new SelectListItem();
                        item.Text = itm.MAST_STATE_NAME;
                        item.Value = Convert.ToString(itm.MAST_STATE_CODE).Trim();
                        stateList.Add(item);
                    }
                }


                //}
                //else
                //{
                //    //var q = (from s in dbContext.MASTER_STATE orderby s.MAST_STATE_NAME select s).ToList();

                //    item = new SelectListItem();
                //    item.Text = PMGSYSession.Current.StateName.Trim();
                //    item.Value = Convert.ToString(PMGSYSession.Current.StateCode).Trim();
                //    habitatList.Add(item);
                //}

                return stateList;
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

        public List<SelectListItem> PopulateSRRDAList(string state)
        {
            PMGSY.Models.PMGSYEntities dbContext = new PMGSY.Models.PMGSYEntities();
            List<SelectListItem> SRRDAList = new List<SelectListItem>();
            SelectListItem item;
            try
            {
                if (state.Trim() == "-1")
                {
                    item = new SelectListItem();
                    item.Text = "Select";
                    item.Value = "-1";
                    SRRDAList.Add(item);
                }
                else if (state.Trim() == "0")
                {
                    item = new SelectListItem();
                    if (PMGSYSession.Current.RoleCode == 25)
                    {
                        item.Text = "All SRRDA";
                        item.Value = "0";
                    }
                    else
                    {
                        item.Text = PMGSYSession.Current.DepartmentName.Trim();
                        item.Value = Convert.ToString(PMGSYSession.Current.AdminNdCode).Trim();
                    }

                    SRRDAList.Add(item);
                }
                else
                {
                    item = new SelectListItem();
                    item.Text = "All SRRDA";
                    item.Value = "0";
                    SRRDAList.Add(item);

                    int i = Convert.ToInt32(state.Trim());
                    var q = (from s in dbContext.ADMIN_DEPARTMENT where s.MAST_STATE_CODE == i && s.MAST_ND_TYPE == "S" orderby s.ADMIN_ND_CODE select s).ToList();

                    foreach (var itm in q)
                    {
                        item = new SelectListItem();
                        item.Text = itm.ADMIN_ND_NAME;
                        item.Value = Convert.ToString(itm.ADMIN_ND_CODE).Trim();
                        SRRDAList.Add(item);
                    }
                }
                return SRRDAList;
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

        public List<SelectListItem> PopulateDPIUList(string srrda)
        {
            PMGSY.Models.PMGSYEntities dbContext = new PMGSY.Models.PMGSYEntities();
            List<SelectListItem> DPIUList = new List<SelectListItem>();
            SelectListItem item;
            try
            {
                if (srrda.Trim() == "-1")
                {
                    item = new SelectListItem();
                    item.Text = "Select";
                    item.Value = "-1";
                    DPIUList.Add(item);
                }
                else if (srrda.Trim() == "0")
                {
                    item = new SelectListItem();
                    //if (PMGSYSession.Current.RoleCode == 2)
                    //{
                    item.Text = "All DPIU";
                    //}
                    //else
                    //{
                    //    item.Text = PMGSYSession.Current.UserName.Trim();
                    //}
                    item.Value = "0";
                    DPIUList.Add(item);
                }
                else
                {
                    if (PMGSYSession.Current.RoleCode == 22)
                    {
                        item = new SelectListItem();
                        item.Text = PMGSYSession.Current.DepartmentName.Trim();
                        item.Value = Convert.ToString(PMGSYSession.Current.AdminNdCode).Trim();
                        DPIUList.Add(item);
                    }
                    else
                    {
                        item = new SelectListItem();
                        item.Text = "All DPIU";
                        item.Value = "0";
                        DPIUList.Add(item);

                        int i = Convert.ToInt32(srrda.Trim());
                        var q = (from s in dbContext.ADMIN_DEPARTMENT where s.MAST_STATE_CODE == i && s.MAST_ND_TYPE == "D" && s.MAST_PARENT_ND_CODE == PMGSYSession.Current.ParentNDCode orderby s.ADMIN_ND_CODE select s).ToList();

                        foreach (var itm in q)
                        {
                            item = new SelectListItem();
                            item.Text = itm.ADMIN_ND_NAME;
                            item.Value = Convert.ToString(itm.ADMIN_ND_CODE).Trim();
                            DPIUList.Add(item);
                        }
                    }
                }
                return DPIUList;
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

        public Array NewsDetailsReportDAL(int? page, int? rows, string sidx, string sord, out Int32 totalRecords, int formonth, int foryear, string appr, string status, string NRRDA, string state, string SRRDA, string DPIU)
        {
            string agency = "";
            int selstate = -2;
            if (state != null)
            {
                selstate = Convert.ToInt32(state.Trim());
            }
            int selsrrda = Convert.ToInt32(SRRDA.Trim());
            int seldpiu = Convert.ToInt32(DPIU.Trim());
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
                var itemList = (from s in dbContext.ADMIN_NEWS
                                where (s.NEWS_UPLOAD_DATE.Month == formonth
                                    //&& s.NEWS_UPLOAD_DATE.Year.Equals(foryear)
                                    && (foryear == -1 ? 1 : s.NEWS_UPLOAD_DATE.Year) == (foryear == -1 ? 1 : foryear)
                                    //&& s.NEWS_APPROVAL.Equals(appr.Trim())
                                    && (appr.Trim() == "0" ? 1 == 1 : s.NEWS_APPROVAL.Equals(appr.Trim()))
                                    && (status == "0" ? "" : s.NEWS_STATUS) == (status == "0" ? "" : status)
                                    /*PIU*/         && (PMGSYSession.Current.RoleCode == 22 ? s.MAST_DISTRICT_CODE : 1) == (PMGSYSession.Current.RoleCode == 22 ? PMGSYSession.Current.DistrictCode : 1)
                                    /*SRRDA*/        && (

  /*Working Previous Logic*/          //     (PMGSYSession.Current.RoleCode == 2 ? s.MAST_STATE_CODE : 1) == (PMGSYSession.Current.RoleCode == 2 ? PMGSYSession.Current.StateCode : 1)
                                    //  && (PMGSYSession.Current.RoleCode == 2 ? (PMGSYSession.Current.DistrictCode != 0 ? s.MAST_DISTRICT_CODE : 1) : 1) == (PMGSYSession.Current.RoleCode == 2 ? (PMGSYSession.Current.DistrictCode != 0 ? PMGSYSession.Current.DistrictCode : 1) : 1)

  /*Working Modified Logic*/            //PMGSYSession.Current.RoleCode == 2
                                    //? (s.MAST_STATE_CODE == PMGSYSession.Current.StateCode && (s.MAST_DISTRICT_CODE != null || s.MAST_DISTRICT_CODE == null)&& s.ADMIN_ND_CODE != null)
                                    //:1==1

                                        PMGSYSession.Current.RoleCode == 2
                                        ? (selsrrda == PMGSYSession.Current.AdminNdCode && DPIU == "-1"
                                           ? (s.MAST_STATE_CODE == PMGSYSession.Current.StateCode && s.MAST_DISTRICT_CODE == null && s.ADMIN_ND_CODE == PMGSYSession.Current.AdminNdCode)
                                           : (selsrrda == PMGSYSession.Current.AdminNdCode && DPIU == "0"
                                             ? (s.MAST_STATE_CODE == PMGSYSession.Current.StateCode && (/*s.MAST_DISTRICT_CODE == null ||*/ s.MAST_DISTRICT_CODE != null) && s.ADMIN_ND_CODE != null && s.NEWS_STATUS != "N")
                                             : (selsrrda == PMGSYSession.Current.AdminNdCode && seldpiu > 0
                                               ? (s.MAST_STATE_CODE == PMGSYSession.Current.StateCode && s.MAST_DISTRICT_CODE != null && s.ADMIN_ND_CODE == seldpiu && s.NEWS_STATUS != "N")
                                               : 1 == 1
                                               )
                                            )
                                          )
                                        : 1 == 1

                                       )
                                    /*NRRDA*/         && (

  /*Working Previous Logic*/               //PMGSYSession.Current.RoleCode == 25
                                    //     ?((s.MAST_STATE_CODE != null && s.MAST_DISTRICT_CODE == null) || (s.MAST_STATE_CODE == null && s.MAST_DISTRICT_CODE == null))
                                    //: 1 == 1

                                           PMGSYSession.Current.RoleCode == 25
                                           ? ((state == "-1" && SRRDA == "-1")
                                                ? (s.MAST_STATE_CODE == null && s.MAST_DISTRICT_CODE == null && s.ADMIN_ND_CODE == null)
                                                : ((state == "0" && SRRDA == "0")
                                                    ? (s.MAST_STATE_CODE != null && s.MAST_DISTRICT_CODE == null && s.ADMIN_ND_CODE != null && s.NEWS_STATUS != "N")
                                                    : ((selstate > 0 && SRRDA == "0")
                                                        ? (s.MAST_STATE_CODE == selstate && s.MAST_DISTRICT_CODE == null && s.ADMIN_ND_CODE != null && s.NEWS_STATUS != "N")
                                                        : ((selstate > 0 && selsrrda > 0)
                                                            ? (s.MAST_STATE_CODE == selstate && s.MAST_DISTRICT_CODE == null && s.ADMIN_ND_CODE == selsrrda && s.NEWS_STATUS != "N")
                                                            : 1 == 1
                                                          )
                                                       )
                                                   )
                                             )
                                           : 1 == 1
                                        )
                                    )
                                orderby s.NEWS_ID
                                select s).ToList();

                totalRecords = itemList.Count();

                if (sidx.Trim() != null)
                {
                    if (sord.ToString() == "asc")
                    {
                        itemList = itemList.OrderBy(x => x.NEWS_ID).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                    }
                    else
                    {
                        itemList = itemList.OrderByDescending(x => x.NEWS_ID).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                    }
                }
                else
                {
                    itemList = itemList.OrderBy(x => x.NEWS_ID).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                }

                string pubEndDate = string.Empty;
                
                itemList = itemList.OrderBy(x => x.UM_User_Master.UserName).ToList();
                return itemList.Select(itemDetails => new
                {
                    cell = new[] {           
                        
                                        //itemDetails.NEWS_ID.ToString().Trim(),
                                        itemDetails.NEWS_UPLOAD_DATE.ToString("dd/MM/yyyy").Trim(),
                                        itemDetails.NEWS_TITLE,
                                        itemDetails.NEWS_PUB_START_DATE.ToString("dd/MM/yyyy").Trim(),
                                        itemDetails.NEWS_PUB_END_DATE == null?"": pubEndDate = comm.GetDateTimeToString((DateTime)itemDetails.NEWS_PUB_END_DATE),
                                        (itemDetails.NEWS_STATUS=="N" || itemDetails.NEWS_STATUS=="F")?"Not Published":(itemDetails.NEWS_STATUS=="P")?"Published":"Archived",
                         
                         /*Edit*/       (PMGSYSession.Current.RoleCode==itemDetails.UM_User_Master.DefaultRoleID
                                            ?(itemDetails.NEWS_APPROVAL == "N" && itemDetails.NEWS_STATUS=="N")
                                            ?"<a href='#'  class='ui-icon ui-icon-pencil ui-align-center' onClick='loadCreateNews(\""+ PMGSY.Common.URLEncrypt.EncryptParameters1(new string[] { "NewsCode =" +  itemDetails.NEWS_ID.ToString().Trim() }) +"\"); return false;'>Edit</a>"
                                            :"-"
                                            :"-"),


                        /*Del*/         (PMGSYSession.Current.RoleCode==itemDetails.UM_User_Master.DefaultRoleID
                                            ?(itemDetails.NEWS_APPROVAL == "N" && itemDetails.NEWS_STATUS=="N")
                                                ?"<a href='#'  class='ui-icon ui-icon-trash ui-align-center' onClick='DeleteNews(\""+ PMGSY.Common.URLEncrypt.EncryptParameters1(new string[] { "NewsCode =" +  itemDetails.NEWS_ID.ToString().Trim()}) + "\"); return false;'>Delete</a>"
                                                :(itemDetails.NEWS_APPROVAL == "Y" && itemDetails.NEWS_STATUS=="A")
    /*Del enabled on Archive*/                      ?"-"//?"<a href='#'  class='ui-icon ui-icon-trash ui-align-center' onClick='DeleteNews(\""+ PMGSY.Common.URLEncrypt.EncryptParameters1(new string[] { "NewsCode =" +  itemDetails.NEWS_ID.ToString().Trim()}) + "\"); return false;'>Show Details</a>"
                                                    :"-"
                                            :"-"),
                       
                                        (PMGSYSession.Current.RoleCode==itemDetails.UM_User_Master.DefaultRoleID?
                       /*Upload*/       ((itemDetails.NEWS_APPROVAL == "N" && itemDetails.NEWS_STATUS=="N")?"<a href='#'  class='ui-icon ui-icon-plusthick ui-align-center' onClick='UploadNews(\""+ PMGSY.Common.URLEncrypt.EncryptParameters1(new string[] { "NewsCode ="+ itemDetails.NEWS_ID.ToString().Trim()}) +"\"); return false;'>Upload</a>":"-"):"-"),

                                        itemDetails.UM_User_Master.DefaultRoleID != 25 ? itemDetails.MASTER_STATE.MAST_STATE_NAME : "National",
                                        itemDetails.UM_User_Master.DefaultRoleID == 25 ? "NRRDA" : itemDetails.ADMIN_DEPARTMENT.ADMIN_ND_NAME,
                                        //itemDetails.UM_User_Master.UserName,itemDetails.UM_User_Master.DefaultRoleID == 25 ? itemDetails.ADMIN_DEPARTMENT.MASTER_AGENCY.MAST_AGENCY_NAME : itemDetails.ADMIN_DEPARTMENT.ADMIN_ND_NAME,
                                        //agency = itemDetails.UM_User_Master.MASTER_AGENCY.Where(a=>a.MAST_AGENCY_CODE == dbContext.ADMIN_DEPARTMENT.Where(s=>s.ADMIN_ND_CODE == itemDetails.ADMIN_ND_CODE).Select(m=>m.MAST_AGENCY_CODE).FirstOrDefault()).Select(a=>a.MAST_AGENCY_NAME).FirstOrDefault(),
                      
                       /*Pub?Arch*/      (PMGSYSession.Current.RoleCode != 22 
                                          ? (
                                                (itemDetails.NEWS_APPROVAL == "Y" && itemDetails.NEWS_STATUS=="F")
                                                ?(PMGSYSession.Current.RoleCode==itemDetails.UM_User_Master.DefaultRoleID?"<a href='#'  onClick='PublishArchNews(\""+ PMGSY.Common.URLEncrypt.EncryptParameters1(new string[] { "NewsCode ="+ itemDetails.NEWS_ID.ToString().Trim()}) +"\"); return false;'>Publish</a>":"Not Published")
                                                :(itemDetails.NEWS_APPROVAL == "Y" && itemDetails.NEWS_STATUS=="P")
                                                //?"<a href='#' onClick='PublishArchNews(\""+ itemDetails.NEWS_ID.ToString().Trim() +"\"); return false;'>Archive</a>"
                                                ?(PMGSYSession.Current.RoleCode==itemDetails.UM_User_Master.DefaultRoleID?"<a href='#' onClick='PublishArchNews(\""+ PMGSY.Common.URLEncrypt.EncryptParameters1(new string[] { "NewsCode ="+ itemDetails.NEWS_ID.ToString().Trim() }) +"\"); return false;'>Archive</a>":"Not Archived")
                                                :"-"
                                            )
                                            :(itemDetails.NEWS_APPROVAL == "Y" && itemDetails.NEWS_STATUS=="F")
                                                ?(PMGSYSession.Current.RoleCode==itemDetails.UM_User_Master.DefaultRoleID?"<a href='#'  onClick='PublishArchNews(\""+ PMGSY.Common.URLEncrypt.EncryptParameters1(new string[] { "NewsCode ="+ itemDetails.NEWS_ID.ToString().Trim() }) +"\"); return false;'>Publish</a>": "Not Published")
                                                :(itemDetails.NEWS_APPROVAL == "Y" && itemDetails.NEWS_STATUS=="P")
                                                //?"Archived"
                                                ?(PMGSYSession.Current.RoleCode==itemDetails.UM_User_Master.DefaultRoleID?"<a href='#' onClick='PublishArchNews(\""+ PMGSY.Common.URLEncrypt.EncryptParameters1(new string[] { "NewsCode ="+ itemDetails.NEWS_ID.ToString().Trim() }) +"\"); return false;'>Archive</a>":"Not Archived")
                                                :(itemDetails.NEWS_APPROVAL == "N" && itemDetails.NEWS_STATUS=="F")
                                          ?"Not Published":"Published"),

                     /*Finalize*/       (itemDetails.NEWS_STATUS == "N" && itemDetails.NEWS_APPROVAL == "N") 
                                            ? "<a href='#'  onClick='FinalizeNews(\""+ PMGSY.Common.URLEncrypt.EncryptParameters1(new string[] { "NewsCode ="+ itemDetails.NEWS_ID.ToString().Trim()}) +"\"); return false;'>Finalize</a>"
                                            :(itemDetails.NEWS_STATUS == "F" && itemDetails.NEWS_APPROVAL == "N" && (PMGSYSession.Current.UserId == itemDetails.NEWS_USER_ID))
                                                ?"<a href='#' onClick='UnFinalizeNews(\""+ PMGSY.Common.URLEncrypt.EncryptParameters1(new string[] { "NewsCode ="+ itemDetails.NEWS_ID.ToString().Trim()}) +"\"); return false;'>Unfinalize</a>"
                                                :"Finalized",
                            
                                        
                    /*Approve*/         (PMGSYSession.Current.RoleCode == 25 ?(itemDetails.NEWS_APPROVAL=="N"?itemDetails.NEWS_STATUS=="F"?"<a href='#' onClick='ApproveNews(\""+ PMGSY.Common.URLEncrypt.EncryptParameters1(new string[] { "NewsCode ="+ itemDetails.NEWS_ID.ToString().Trim() }) +"\"); return false;'>Not Approved</a>":"Not Approved":"Approved"):
                                        (PMGSYSession.Current.RoleCode == 2? (itemDetails.NEWS_APPROVAL =="N"?(itemDetails.MAST_DISTRICT_CODE> 0 ?"<a href='#' onClick='ApproveNews(\""+ PMGSY.Common.URLEncrypt.EncryptParameters1(new string[] { "NewsCode ="+ itemDetails.NEWS_ID.ToString().Trim() }) +"\"); return false;'>Not Approved</a>":"Not Approved"):"Approved"): (itemDetails.NEWS_APPROVAL=="N"?"Not Approved</a>":"Approved"))),
                                        
                   /*View*/             "<a href='#'  class='ui-icon ui-icon-zoomin ui-align-center' onClick='DisplayNewsDetails(\""+ PMGSY.Common.URLEncrypt.EncryptParameters1(new string[] { "NewsCode ="+ itemDetails.NEWS_ID.ToString().Trim() }) +"\"); return false;'>Display News Details</a>"
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

        public int FinalizeNews(string id)
        {
            int ret = 0;
            try
            {
                int newsId = Convert.ToInt32(id.Trim());

                dbContext = new PMGSY.Models.PMGSYEntities();

                PMGSY.Models.ADMIN_NEWS adminNews = new PMGSY.Models.ADMIN_NEWS();

                adminNews = dbContext.ADMIN_NEWS.Where(s => s.NEWS_ID == newsId).FirstOrDefault();

                PMGSY.Common.CommonFunctions comm = new PMGSY.Common.CommonFunctions();

                adminNews.NEWS_STATUS = "F";
                //adminNews.NEWS_APPROVAL_DATE = comm.GetStringToDateTime(System.DateTime.Now.ToString("dd/MM/yyyy"));

                dbContext.Entry(adminNews).State = System.Data.Entity.EntityState.Modified;
                dbContext.SaveChanges();
                ret = 1;

                return ret;
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                return -1;
            }
        }

        public int UnfinalizeNews(string id)
        {
            int ret = 0;
            try
            {
                int newsId = Convert.ToInt32(id.Trim());

                dbContext = new PMGSY.Models.PMGSYEntities();

                PMGSY.Models.ADMIN_NEWS adminNews = new PMGSY.Models.ADMIN_NEWS();

                adminNews = dbContext.ADMIN_NEWS.Where(s => s.NEWS_ID == newsId).FirstOrDefault();

                PMGSY.Common.CommonFunctions comm = new PMGSY.Common.CommonFunctions();

                adminNews.NEWS_STATUS = "N";
                //adminNews.NEWS_APPROVAL_DATE = comm.GetStringToDateTime(System.DateTime.Now.ToString("dd/MM/yyyy"));

                dbContext.Entry(adminNews).State = System.Data.Entity.EntityState.Modified;
                dbContext.SaveChanges();
                ret = 1;

                return ret;
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                return -1;
            }
        }

        public bool saveNews(PMGSY.Models.NewsDetails.CreateNews crNews)
        {
            try
            {
                int newsId = 0;

                dbContext = new PMGSY.Models.PMGSYEntities();
                PMGSY.Models.ADMIN_NEWS adminNews = new PMGSY.Models.ADMIN_NEWS();

                var q = (from n in dbContext.ADMIN_NEWS
                         orderby crNews.newsId
                         select n.NEWS_ID).DefaultIfEmpty().Max();
                if (q > 0)
                {
                    newsId = q + 1;
                }
                else
                {
                    newsId = 1;
                }

                PMGSY.Common.CommonFunctions comm = new PMGSY.Common.CommonFunctions();

                if (PMGSYSession.Current.RoleCode == 2)//State
                {
                    crNews.mast_state_code = PMGSYSession.Current.StateCode;
                }
                else if (PMGSYSession.Current.RoleCode == 22)//PIU
                {
                    crNews.mast_state_code = PMGSYSession.Current.StateCode;
                    crNews.mast_dist_code = PMGSYSession.Current.DistrictCode;
                }


                adminNews.NEWS_ID = newsId;
                adminNews.NEWS_UPLOAD_DATE = System.DateTime.Now;
                adminNews.NEWS_TITLE = crNews.newsTitle.Trim();
                adminNews.NEWS_DESCRIPTION = crNews.news_Desc.Trim();
                adminNews.NEWS_PUB_START_DATE = comm.GetStringToDateTime(crNews.newa_Pub_Start_Date.Trim());

                if (crNews.newa_Pub_End_Date != null)
                {
                    adminNews.NEWS_PUB_END_DATE = comm.GetStringToDateTime(crNews.newa_Pub_End_Date.Trim());
                }
                else
                {
                    adminNews.NEWS_PUB_END_DATE = null;
                }

                if (crNews.mast_state_code != null && crNews.mast_state_code != 0)
                {
                    adminNews.MAST_STATE_CODE = crNews.mast_state_code;
                }
                else
                {
                    adminNews.MAST_STATE_CODE = null;
                }

                if (crNews.mast_dist_code != null && crNews.mast_dist_code != 0)
                {
                    adminNews.MAST_DISTRICT_CODE = crNews.mast_dist_code;
                }
                else
                {
                    adminNews.MAST_DISTRICT_CODE = null;
                }
                adminNews.NEWS_APPROVAL = "N";
                adminNews.NEWS_APPROVAL_DATE = null;
                adminNews.NEWS_ARCHIVED_DATE = null;
                adminNews.NEWS_STATUS = "N";
                adminNews.NEWS_USER_ID = PMGSYSession.Current.UserId;
                if (PMGSYSession.Current.AdminNdCode != 0)
                {
                    adminNews.ADMIN_ND_CODE = PMGSYSession.Current.AdminNdCode;
                }
                else
                {
                    adminNews.ADMIN_ND_CODE = null;
                }
                adminNews.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];

                //dbContext.Entry(adminFBRep).State = System.Data.Entity.EntityState.Modified;
                dbContext.ADMIN_NEWS.Add(adminNews);
                dbContext.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                return false;
            }
        }

        public int updateNews(PMGSY.Models.NewsDetails.CreateNews crNews)
        {
            int ret = 0;
            try
            {
                int newsId = 0;

                dbContext = new PMGSY.Models.PMGSYEntities();
                PMGSY.Models.PMGSYEntities dbContext1 = new PMGSY.Models.PMGSYEntities();
                PMGSY.Models.ADMIN_NEWS adminNews = new PMGSY.Models.ADMIN_NEWS();

                var q = (from n in dbContext.ADMIN_NEWS
                         where n.NEWS_ID == crNews.hdnewsId //&& f.REP_ID == repId
                         orderby n.NEWS_ID
                         select n.NEWS_ID).DefaultIfEmpty().Max();
                if (q == crNews.hdnewsId)
                {
                    var a = dbContext1.ADMIN_NEWS.Where(s => s.NEWS_ID == crNews.hdnewsId).ToList();
                    foreach (var s in a)
                    {
                        if ((s.NEWS_APPROVAL == "Y" && s.NEWS_STATUS == "P") || (s.NEWS_APPROVAL == "N" && s.NEWS_STATUS == "N"))
                        {
                            PMGSY.Common.CommonFunctions comm = new PMGSY.Common.CommonFunctions();

                            if (PMGSYSession.Current.RoleCode == 2)//State
                            {
                                crNews.mast_state_code = PMGSYSession.Current.StateCode;
                            }
                            else if (PMGSYSession.Current.RoleCode == 22)//PIU
                            {
                                crNews.mast_state_code = PMGSYSession.Current.StateCode;
                                crNews.mast_dist_code = PMGSYSession.Current.DistrictCode;
                            }

                            adminNews.NEWS_ID = crNews.hdnewsId;
                            adminNews.NEWS_UPLOAD_DATE = System.DateTime.Now;
                            adminNews.NEWS_TITLE = crNews.newsTitle.Trim();
                            adminNews.NEWS_DESCRIPTION = crNews.news_Desc.Trim();
                            adminNews.NEWS_PUB_START_DATE = comm.GetStringToDateTime(crNews.newa_Pub_Start_Date.Trim());

                            if (crNews.newa_Pub_End_Date != null)
                            {
                                adminNews.NEWS_PUB_END_DATE = comm.GetStringToDateTime(crNews.newa_Pub_End_Date.Trim());
                            }
                            else
                            {
                                adminNews.NEWS_PUB_END_DATE = null;
                            }

                            if (crNews.mast_state_code != null && crNews.mast_state_code != 0)
                            {
                                adminNews.MAST_STATE_CODE = crNews.mast_state_code;
                            }
                            else
                            {
                                adminNews.MAST_STATE_CODE = null;
                            }

                            if (crNews.mast_dist_code != null && crNews.mast_dist_code != 0)
                            {
                                adminNews.MAST_DISTRICT_CODE = crNews.mast_dist_code;
                            }
                            else
                            {
                                adminNews.MAST_DISTRICT_CODE = null;
                            }
                            adminNews.NEWS_APPROVAL = "N";
                            adminNews.NEWS_APPROVAL_DATE = null;
                            adminNews.NEWS_ARCHIVED_DATE = null;
                            adminNews.NEWS_STATUS = "N";
                            adminNews.NEWS_USER_ID = PMGSYSession.Current.UserId;

                            if (PMGSYSession.Current.AdminNdCode != 0)
                            {
                                adminNews.ADMIN_ND_CODE = PMGSYSession.Current.AdminNdCode;
                            }
                            else
                            {
                                adminNews.ADMIN_ND_CODE = null;
                            }
                            adminNews.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];

                            dbContext.Entry(adminNews).State = System.Data.Entity.EntityState.Modified;
                            //dbContext.ADMIN_NEWS.Add(adminNews);
                            dbContext.SaveChanges();
                            ret = 1;
                        }
                        else
                        {
                            ret = -2;
                        }
                    }
                }
                else
                {
                    ret = 0;
                }
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                ret = -1;
            }
            return ret;
        }

        public int deleteNews(string id)
        {
            using (TransactionScope ts = new TransactionScope())
            {
                try
                {
                    int newsId = Convert.ToInt32(id.Trim());

                    dbContext = new PMGSY.Models.PMGSYEntities();
                    PMGSY.Models.PMGSYEntities dbContext1 = new Models.PMGSYEntities();
                    PMGSY.Models.ADMIN_NEWS adminNews = new PMGSY.Models.ADMIN_NEWS();
                    PMGSY.Models.ADMIN_NEWS_FILES admnNewsfiles = new PMGSY.Models.ADMIN_NEWS_FILES();

                    var q = (from n in dbContext.ADMIN_NEWS
                             where n.NEWS_ID == newsId //&& f.REP_ID == repId
                             orderby n.NEWS_ID
                             select n.NEWS_ID).DefaultIfEmpty().Max();
                    if (q == newsId)
                    {
                        adminNews = dbContext.ADMIN_NEWS.Where(m => m.NEWS_ID == newsId).FirstOrDefault();
                        if (adminNews != null)
                        {
                            if ((adminNews.NEWS_APPROVAL == "Y" && adminNews.NEWS_STATUS == "P") || (adminNews.NEWS_APPROVAL == "N" && adminNews.NEWS_STATUS == "N") || (adminNews.NEWS_APPROVAL == "Y" && adminNews.NEWS_STATUS == "A"))
                            {
                                PMGSY.Common.CommonFunctions comm = new PMGSY.Common.CommonFunctions();

                                var data = dbContext.ADMIN_NEWS_FILES.Where(m => m.NEWS_ID == newsId).ToList();

                                foreach (PMGSY.Models.ADMIN_NEWS_FILES item in data)
                                {
                                    dbContext.ADMIN_NEWS_FILES.Remove(item);
                                    dbContext.SaveChanges();
                                }

                                //if (admnNewsfiles != null)
                                //{
                                //    dbContext.ADMIN_NEWS_FILES.Remove(admnNewsfiles);
                                //    dbContext.SaveChanges();
                                //}

                                adminNews = dbContext.ADMIN_NEWS.Where(m => m.NEWS_ID == newsId).FirstOrDefault();
                                dbContext.ADMIN_NEWS.Remove(adminNews);
                                dbContext.SaveChanges();
                                ts.Complete();
                                return 1;
                            }
                            else
                            {
                                //if (!(adminNews.NEWS_APPROVAL == "Y" && adminNews.NEWS_STATUS == "P") || (adminNews.NEWS_APPROVAL == "N" && adminNews.NEWS_STATUS == "N"))
                                {
                                    return -2;
                                }
                            }
                        }
                        else
                        {
                            return 0;
                        }
                    }
                    else
                    {
                        return 0;
                    }
                }
                catch (Exception ex)
                {
                    Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                    ts.Dispose();
                    if (ex.InnerException.InnerException.Message == "The DELETE statement conflicted with the REFERENCE constraint \"FK_ADMIN_NEWS_ADMIN_NEWS_FILES\". The conflict occurred in database \"OMMAS_DEV\", table \"omms.ADMIN_NEWS_FILES\", column 'NEWS_ID'.\r\nThe statement has been terminated.")
                    {
                        return -3;
                    }
                    else
                    {
                        return -1;
                    }
                }
            }
        }

        public int PublishArchiveNews(string id)
        {
            PMGSY.Models.ADMIN_NEWS adminNews = new PMGSY.Models.ADMIN_NEWS();
            int ret = 0;
            try
            {
                int newsId = Convert.ToInt32(id.Trim());

                dbContext = new PMGSY.Models.PMGSYEntities();
                PMGSY.Models.PMGSYEntities dbContext1 = new PMGSY.Models.PMGSYEntities();


                var q = (from n in dbContext.ADMIN_NEWS
                         where n.NEWS_ID == newsId //&& f.REP_ID == repId
                         orderby n.NEWS_ID
                         select n.NEWS_ID).DefaultIfEmpty().Max();
                //if (q == newsId)
                {
                    adminNews = dbContext.ADMIN_NEWS.Where(s => s.NEWS_ID == newsId).FirstOrDefault();

                    PMGSY.Common.CommonFunctions comm = new PMGSY.Common.CommonFunctions();

                    //foreach (var item in a)
                    //{
                    if (adminNews.NEWS_APPROVAL == "Y")
                    {
                        if (adminNews.NEWS_STATUS == "F")
                        {
                            adminNews.NEWS_STATUS = "P";
                            adminNews.NEWS_PUB_START_DATE = comm.GetStringToDateTime(System.DateTime.Now.ToString("dd/MM/yyyy"));
                        }
                        else if (adminNews.NEWS_STATUS == "P")
                        {
                            adminNews.NEWS_STATUS = "A";
                            adminNews.NEWS_ARCHIVED_DATE = comm.GetStringToDateTime(System.DateTime.Now.ToString("dd/MM/yyyy"));
                        }
                        //adminNews.NEWS_ID = newsId;
                        //adminNews.NEWS_USER_ID = PMGSYSession.Current.RoleCode;

                        dbContext.Entry(adminNews).State = System.Data.Entity.EntityState.Modified;
                        dbContext.SaveChanges();
                        if (adminNews.NEWS_STATUS == "P")
                        {
                            ret = 1;
                        }
                        else if (adminNews.NEWS_STATUS == "A")
                        {
                            ret = 2;
                        }
                    }
                    else
                    {
                        ret = -3;
                    }
                    //}
                    return ret;
                }

                //else
                //{
                //    return 0;
                //}
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                if (adminNews.NEWS_STATUS == "P")
                {
                    return -1;
                }
                else if (adminNews.NEWS_STATUS == "A")
                {
                    return -2;
                }
                else
                {
                    return -1;
                }
            }
        }

        public int ApproveNews(string id)
        {
            int ret = 0;
            try
            {
                int newsId = Convert.ToInt32(id.Trim());

                dbContext = new PMGSY.Models.PMGSYEntities();
                PMGSY.Models.PMGSYEntities dbContext1 = new PMGSY.Models.PMGSYEntities();
                PMGSY.Models.ADMIN_NEWS adminNews = new PMGSY.Models.ADMIN_NEWS();

                var q = (from n in dbContext.ADMIN_NEWS
                         where n.NEWS_ID == newsId //&& f.REP_ID == repId
                         orderby n.NEWS_ID
                         select n.NEWS_ID).DefaultIfEmpty().Max();
                //if (q == newsId)
                {
                    adminNews = dbContext.ADMIN_NEWS.Where(s => s.NEWS_ID == newsId).FirstOrDefault();

                    PMGSY.Common.CommonFunctions comm = new PMGSY.Common.CommonFunctions();

                    adminNews.NEWS_APPROVAL = "Y";
                    adminNews.NEWS_APPROVAL_DATE = comm.GetStringToDateTime(System.DateTime.Now.ToString("dd/MM/yyyy"));

                    dbContext.Entry(adminNews).State = System.Data.Entity.EntityState.Modified;
                    dbContext.SaveChanges();
                    ret = 1;
                    //    }
                    //    else
                    //    {
                    //        ret = -2;
                    //    }
                    //}
                    return ret;
                }
                //else
                //{
                //    return 0;
                //}
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                return -1;
            }
        }

        #region Upload News
        public Array NewsFileList(int? page, int? rows, string sidx, string sord, out Int32 totalRecords, int NewsId)
        {
            string filters = string.Empty;
            string nameSearch = string.Empty;
            PMGSY.Models.PMGSYEntities dbContext = new PMGSY.Models.PMGSYEntities();
            try
            {
                // new change with data from stored procedure
                //if (roadType == "0")
                //{
                //    roadType = null;
                //}
                var lstPlanRoads = dbContext.ADMIN_NEWS_FILES.Where(s => s.NEWS_ID == NewsId && s.FILE_TYPE == "P").ToList();

                totalRecords = lstPlanRoads.Count();

                if (sidx.Trim() != string.Empty)
                {
                    if (sord.ToString() == "asc")
                    {
                        switch (sidx)
                        {
                            case "News_Id":
                                lstPlanRoads = lstPlanRoads.OrderBy(x => x.FILE_ID).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                        }
                    }
                    else
                    {
                        switch (sidx)
                        {
                            case "News_Id":
                                lstPlanRoads = lstPlanRoads.OrderByDescending(x => x.FILE_ID).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                        }
                    }
                }
                else
                {
                    lstPlanRoads = lstPlanRoads.OrderBy(x => x.FILE_ID).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                }

                string pubEndDate = string.Empty;
                lstPlanRoads = lstPlanRoads.OrderBy(x => x.NEWS_ID).ToList();
                return lstPlanRoads.Select(itemDetails => new
                {
                    id = itemDetails.NEWS_ID.ToString().Trim() + "$" + itemDetails.FILE_ID.ToString().Trim(),// + "$" + fileDetails.IMS_PR_ROAD_CODE,
                    cell = new[] {           
                        
                                        //itemDetails.FILE_ID.ToString().Trim(),
                                        URLEncrypt.EncryptParameters(new string[] { itemDetails.FILE_NAME  }),
                                        itemDetails.FILE_DESC,
                                        "<center><table><tr><td style='border-color:white'><a href='#' style='float:left' id='btnSave"+  itemDetails.NEWS_ID.ToString().Trim()+ "$" + itemDetails.FILE_ID.ToString().Trim() +"' title='Click here to Save the File Details' class='ui-icon ui-icon-disk ui-align-center' onClick=SavePDFDetails('" +  itemDetails.NEWS_ID + "$" + itemDetails.FILE_ID.ToString().Trim()+"');></a><a href='#' style='float:right' id='btnCancel" +  itemDetails.NEWS_ID  + "$" + itemDetails.FILE_ID.ToString().Trim() +"' title='Click here to Cancel the File Edit' class='ui-icon ui-icon-closethick ui-align-center' onClick= CancelSavePDFDetails('" +  itemDetails.NEWS_ID + "$" + itemDetails.FILE_ID.ToString().Trim() +"');></a></td></tr></table></center>",
                                        "<a href='#' title='Click here to Edit the File Details' class='ui-icon ui-icon-pencil ui-align-center' onClick='EditPDFDetails(\"" +  itemDetails.NEWS_ID.ToString().Trim() + "$" + itemDetails.FILE_ID.ToString().Trim() + "\"); return false;'>Edit</a>",
                        /*Del*/         //"<a href='#'  class='ui-icon ui-icon-trash ui-align-center' onClick='DeleteFileDetails(\"" +  itemDetails.NEWS_ID.ToString().Trim() /*+ "$" + itemDetails.FILE_ID.ToString().Trim() + "','" + itemDetails.FILE_NAME.Trim()*/ +"\"); return false;'>Delete News Details</a>",
                                        //"<a href='#' title='Click here to Delete the File Details' class='ui-icon ui-icon-trash ui-align-center' onClick=DeleteFileDetails('"+ itemDetails.NEWS_ID.ToString().Trim()  + "$" + itemDetails.FILE_ID.ToString().Trim() + "','" + itemDetails.FILE_NAME.Trim() + "'); return false;>Delete</a>",
                                        "<a href='#' title='Click here to Delete the File Details' class='ui-icon ui-icon-trash ui-align-center' onClick=DeleteFileDetails('"+ PMGSY.Common.URLEncrypt.EncryptParameters1(new string[] { "NewsCode =" + itemDetails.NEWS_ID.ToString().Trim()  + "$" + itemDetails.FILE_ID.ToString().Trim() + "','" + itemDetails.FILE_NAME.Trim() }) + "'); return false;>Delete</a>",
                                        "<span class='ui-icon ui-icon-locked ui-align-center'>Action</span></center>"
                               }
                }).ToArray();

                //end of change

            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                totalRecords = 0;
                dbContext.Dispose();
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

        public Array NewsImageFileList(int? page, int? rows, string sidx, string sord, out Int32 totalRecords, int NewsId)
        {
            string filters = string.Empty;
            string nameSearch = string.Empty;
            PMGSY.Models.PMGSYEntities dbContext = new PMGSY.Models.PMGSYEntities();
            try
            {
                // new change with data from stored procedure
                //if (roadType == "0")
                //{
                //    roadType = null;
                //}
                var query = dbContext.ADMIN_NEWS_FILES.Where(s => s.NEWS_ID == NewsId && s.FILE_TYPE == "I").ToList();

                totalRecords = query.Count();

                if (sidx.Trim() != string.Empty)
                {
                    if (sord.ToString() == "asc")
                    {
                        switch (sidx)
                        {
                            case "News_Id":
                                query = query.OrderBy(x => x.FILE_ID).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                        }
                    }
                    else
                    {
                        switch (sidx)
                        {
                            case "News_Id":
                                query = query.OrderByDescending(x => x.FILE_ID).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                        }
                    }
                }
                else
                {
                    query = query.OrderBy(x => x.FILE_ID).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                }

                string VirtualDirectoryUrl = Path.Combine(ConfigurationManager.AppSettings["NEWS_IMAGE_FILE_UPLOAD_VIRTUAL_DIR_PATH"], "thumbnails");
                string PhysicalPath = ConfigurationManager.AppSettings["NEWS_IMAGE_FILE_UPLOAD"];

                return query.Select(fileDetails => new
                {
                    id = fileDetails.NEWS_ID + "$" + fileDetails.FILE_ID.ToString().Trim(),
                    cell = new[] {   
                                   // @"file/://"  + Path.Combine(PhysicalPath, fileDetails.IMS_FILE_NAME.ToString()).ToString().Replace(@"\\",@"//").Replace(@"\",@"/"),
                                    Path.Combine(VirtualDirectoryUrl, fileDetails.FILE_NAME.ToString()).ToString().Replace(@"\\",@"//").Replace(@"\",@"/"),
                                    //fileDetails..ToString(),
                                    //"",
                                    fileDetails.FILE_DESC,
                                    //"<a href='#' title='Click here to Download an Image' class='ui-icon ui-icon-arrowthickstop-1-s ui-align-center' onClick=DownLoadImage(\"" + fileDetails.NEWS_ID.ToString().Trim()  + "$" + fileDetails.FILE_ID.ToString().Trim()   +"\"); return false;'>Download</a>" ,
                                    "<a href='#' title='Click here to Download the Image File' class='ui-icon ui-icon-arrowthickstop-1-s ui-align-center' onClick=DownLoadImage(\"" + URLEncrypt.EncryptParameters(new string[] { fileDetails.FILE_NAME  }) +"\"); return false;'>Download</a>" ,
                                    "<a href='#' title='Click here to Edit the File Details' class='ui-icon ui-icon-pencil ui-align-center' onClick=EditImageDetails('" +  fileDetails.NEWS_ID.ToString().Trim()  + "$" + fileDetails.FILE_ID.ToString().Trim() +"'); return false;>Edit</a>",
                                    //"<a href='#' title='Click here to delete the File Details' class='ui-icon ui-icon-trash ui-align-center' onClick=DeleteFileDetails('" + fileDetails.NEWS_ID.ToString().Trim()  + "$" + fileDetails.FILE_ID.ToString().Trim() + "','" + fileDetails.FILE_NAME.Trim() + "'); return false;>Delete</a>",                                    
                                    "<a href='#' title='Click here to Delete the File Details' class='ui-icon ui-icon-trash ui-align-center' onClick=DeleteFileDetails('"+ PMGSY.Common.URLEncrypt.EncryptParameters1(new string[] { "NewsCode =" + fileDetails.NEWS_ID.ToString().Trim()  + "$" + fileDetails.FILE_ID.ToString().Trim() + "','" + fileDetails.FILE_NAME.Trim() }) + "'); return false;>Delete</a>",
                                    "<center><table><tr><td style='border-color:white'><a href='#' style='float:left' id='btnSave"+  fileDetails.NEWS_ID.ToString().Trim()  + "$" + fileDetails.FILE_ID.ToString().Trim()  +"' title='Click here to Save the File Details' class='ui-icon ui-icon-disk ui-align-center' onClick=SaveFileDetails('" +  fileDetails.NEWS_ID.ToString().Trim()  + "$" + fileDetails.FILE_ID.ToString().Trim() + "');></a><a href='#' style='float:right' id='btnCancel" +  fileDetails.NEWS_ID.ToString().Trim()  + "$" + fileDetails.FILE_ID.ToString().Trim()  +"' title='Click here to Cancel the File Edit' class='ui-icon ui-icon-closethick ui-align-center' onClick= CancelSaveFileDetails('" +  fileDetails.NEWS_ID.ToString().Trim()  + "$" + fileDetails.FILE_ID.ToString().Trim() + "');></a></td></tr></table></center>"
                    }
                }).ToArray();
                //end of change

            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                totalRecords = 0;
                dbContext.Dispose();
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


        public string AddFileUploadDetailsDAL(List<PMGSY.Models.ADMIN_NEWS_FILES> lst_ims_news_files)
        {
            try
            {
                dbContext = new Models.PMGSYEntities();
                Int32? MaxID;
                foreach (PMGSY.Models.ADMIN_NEWS_FILES fileModel in lst_ims_news_files)
                {
                    if (!(dbContext.ADMIN_NEWS_FILES.Where(m => m.NEWS_ID == fileModel.NEWS_ID).Any()))
                    {
                        MaxID = 0;
                    }
                    else
                    {
                        MaxID = (from c in dbContext.ADMIN_NEWS_FILES where c.NEWS_ID == fileModel.NEWS_ID select (Int32?)c.FILE_ID ?? 0).Max();
                    }
                    ++MaxID;
                    fileModel.FILE_ID = Convert.ToInt32(MaxID);
                    //fileModel.USERID = PMGSYSession.Current.UserId;
                    //fileModel.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                    dbContext.ADMIN_NEWS_FILES.Add(fileModel);
                }
                dbContext.SaveChanges();
                return string.Empty;
            }
            catch (DbUpdateException ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                return ("An Error Occurred While Processing Your Request.");
            }
            catch (OptimisticConcurrencyException ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                return ("An Error Occurred While Processing Your Request.");
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                return ("Error Occurred While Processing Request.");
            }
            finally
            {
                dbContext.Dispose();
            }
        }

        public string UpdateImageDetailsDAL(PMGSY.Models.ADMIN_NEWS_FILES news_files)
        {
            try
            {
                dbContext = new Models.PMGSYEntities();
                PMGSY.Models.ADMIN_NEWS_FILES obj_news_files = dbContext.ADMIN_NEWS_FILES.Where(
                    a => a.NEWS_ID == news_files.NEWS_ID &&
                    a.FILE_ID == news_files.FILE_ID &&
                    a.FILE_TYPE == news_files.FILE_TYPE
                    ).FirstOrDefault();

                //db_ims_proposal_files.CHAINAGE = ims_proposal_files.CHAINAGE;
                //db_ims_proposal_files.ISPF_FILE_REMARK = ims_proposal_files.ISPF_FILE_REMARK;
                obj_news_files.FILE_DESC = news_files.FILE_DESC.Trim();
                dbContext.Entry(obj_news_files).State = System.Data.Entity.EntityState.Modified;
                dbContext.SaveChanges();

                return string.Empty;
            }
            catch (DbUpdateException ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                return ("An Error Occurred While Processing Your Request.");
            }
            catch (OptimisticConcurrencyException ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                return ("An Error Occurred While Processing Your Request.");
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                return ("Error Occurred While Processing Request.");
            }
            finally
            {
                dbContext.Dispose();
            }
        }

        public string UpdatePDFDetailsDAL(PMGSY.Models.ADMIN_NEWS_FILES news_files)
        {
            try
            {
                dbContext = new Models.PMGSYEntities();
                PMGSY.Models.ADMIN_NEWS_FILES obj_news_files = dbContext.ADMIN_NEWS_FILES.Where(
                   a => a.NEWS_ID == news_files.NEWS_ID &&
                    a.FILE_ID == news_files.FILE_ID
                    //a.ISPF_TYPE == ims_proposal_files.ISPF_TYPE
                    ).FirstOrDefault();

                obj_news_files.FILE_DESC = news_files.FILE_DESC.Trim();

                dbContext.Entry(obj_news_files).State = System.Data.Entity.EntityState.Modified;
                dbContext.SaveChanges();

                return string.Empty;
            }
            catch (DbUpdateException ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                return ("An Error Occurred While Processing Your Request.");
            }
            catch (OptimisticConcurrencyException ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                return ("An Error Occurred While Processing Your Request.");
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                return ("Error Occurred While Processing Request.");
            }
            finally
            {
                dbContext.Dispose();
            }
        }

        public string DeleteFileDetailsDAL(PMGSY.Models.ADMIN_NEWS_FILES news_files)
        {
            try
            {
                dbContext = new Models.PMGSYEntities();
                PMGSY.Models.ADMIN_NEWS_FILES obj_news_files = dbContext.ADMIN_NEWS_FILES.Where(
                    a => a.NEWS_ID == news_files.NEWS_ID &&
                    a.FILE_ID == news_files.FILE_ID
                    //a.ISPF_TYPE == ims_proposal_files.ISPF_TYPE &&
                    ).FirstOrDefault();

                //obj_ims_propopsal_files.USERID = PMGSYSession.Current.UserId;
                //obj_ims_propopsal_files.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                dbContext.Entry(obj_news_files).State = System.Data.Entity.EntityState.Modified;
                dbContext.SaveChanges();

                dbContext.ADMIN_NEWS_FILES.Remove(obj_news_files);
                dbContext.SaveChanges();
                return string.Empty;
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                return "There is an error while processing request.";
            }
            finally
            {
                dbContext.Dispose();
            }
        }
        #endregion

    }
}