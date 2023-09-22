using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PMGSY.Common;
using PMGSY.Extensions;
using PMGSY.Models;
using System.Data.Entity;
using System.Web.Mvc;
using PMGSY.Models.ARRR;
using System.Configuration;

namespace PMGSY.DAL.ARRR
{
    public class ARRRDAL : IARRRDAL
    {
        PMGSYEntities dbContext;

        #region Chapter Items


        public Array ListChapterItemsDAL(int page, int rows, string sidx, string sord, out long totalRecords)
        {
            try
            {
                dbContext = new PMGSYEntities();

                var list = from arrrlist in dbContext.MASTER_ARRR_ITEMS_MASTER

                           select new
                           {
                               arrrlist.MAST_ITEM_CODE,
                               arrrlist.MAST_ITEM_NAME,
                               arrrlist.MAST_ITEM_DESC,
                               arrrlist.MAST_ITEM_ACTIVE,
                               arrrlist.MAST_ITEM,
                               arrrlist.MAST_MAJOR_SUBITEM_CODE,
                               arrrlist.MAST_MINOR_SUBITEM_CODE,
                               //arrrlist.MAST_ITEM_PARENT,
                               MAST_ITEM_TYPE = arrrlist.MAST_ITEM_PARENT == 0 ? "Item" : (arrrlist.MAST_MAJOR_SUBITEM_CODE > 0 && arrrlist.MAST_MINOR_SUBITEM_CODE == 0) ? "Major Item" : (arrrlist.MAST_MAJOR_SUBITEM_CODE > 0 && arrrlist.MAST_MINOR_SUBITEM_CODE > 0) ? "Minor Item" : "",
                               arrrlist.MAST_HEAD_CODE,
                               arrrlist.MAST_ITEM_USER_CODE,
                               arrrlist.MAST_MORD_REF,
                               arrrlist.MASTER_ARRR_ITEM_HEAD.MAST_HEAD_NAME,
                           };

                totalRecords = list.Count();

                if (sidx.Trim() != string.Empty)
                {
                    if (sord.ToString() == "asc")
                    {
                        switch (sidx)
                        {
                            //case "MAST_ITEM_CODE":
                            //    list = list.OrderBy(x => x.MAST_ITEM_CODE).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                            //    break;
                            //case "MAST_ITEM_NAME":
                            //    list = list.OrderBy(x => x.MAST_ITEM_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                            //    break;
                            //case "MAST_ITEM_DESC":
                            //    list = list.OrderBy(x => x.MAST_ITEM_DESC).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                            //    break;
                            default:
                                list = list.OrderBy(x => x.MAST_HEAD_CODE).ThenBy(x => x.MAST_ITEM_TYPE).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                                break;
                        }
                    }
                    else
                    {
                        switch (sidx)
                        {
                            //case "MAST_ITEM_CODE":
                            //    list = list.OrderByDescending(x => x.MAST_ITEM_CODE).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                            //    break;
                            //case "MAST_ITEM_NAME":
                            //    list = list.OrderByDescending(x => x.MAST_ITEM_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                            //    break;
                            //case "MAST_ITEM_DESC":
                            //    list = list.OrderByDescending(x => x.MAST_ITEM_DESC).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                            //    break;
                            default:
                                list = list.OrderByDescending(x => x.MAST_HEAD_CODE).ThenByDescending(x => x.MAST_ITEM_TYPE).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                                break;
                        }
                    }
                }
                else
                {
                    list = list.OrderBy(x => x.MAST_ITEM_CODE).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                }

                var result = list.Select(arrrlist => new
                {
                    arrrlist.MAST_ITEM_CODE,
                    arrrlist.MAST_ITEM_NAME,
                    arrrlist.MAST_ITEM_DESC,
                    arrrlist.MAST_ITEM_ACTIVE,
                    arrrlist.MAST_ITEM,
                    arrrlist.MAST_MAJOR_SUBITEM_CODE,
                    arrrlist.MAST_MINOR_SUBITEM_CODE,
                    //arrrlist.MAST_ITEM_PARENT,
                    arrrlist.MAST_ITEM_TYPE,
                    arrrlist.MAST_HEAD_CODE,
                    arrrlist.MAST_ITEM_USER_CODE,
                    arrrlist.MAST_MORD_REF,
                    arrrlist.MAST_HEAD_NAME,
                }).ToArray();

                return result.Select(arrrDetails => new
                {
                    id = arrrDetails.MAST_ITEM_CODE.ToString().Trim(),//URLEncrypt.EncryptParameters1(new string[] { "AdminCode =" + adminDetails.ADMIN_ND_CODE.ToString().Trim() }),

                    cell = new[]{
                    //dbContext.MASTER_ARRR_ITEM_HEAD.Where(m => m.MAST_HEAD_CODE == arrrDetails.MAST_HEAD_CODE).Select(a=>a.MAST_HEAD_NAME).FirstOrDefault(),
                    arrrDetails.MAST_HEAD_NAME,
                 // arrrDetails.MAST_ITEM_TYPE == "Item" ? "<a href='#' class='ui-icon ui-icon-caret-1-e'></a>" + (arrrDetails.MAST_ITEM_NAME == null ? string.Empty : arrrDetails.MAST_ITEM_NAME.ToString()) : (arrrDetails.MAST_ITEM_TYPE == "Major Item") ? "<a href='#'class='ui-icon ui-icon-arrowthick-1-e'></a>" + (arrrDetails.MAST_ITEM_NAME == null ? string.Empty : arrrDetails.MAST_ITEM_NAME.ToString()) :  (arrrDetails.MAST_ITEM_TYPE == "Minor Item") ?  "<a href='#' class='ui-icon ui-icon-arrowthick-1-e'></a>" + (arrrDetails.MAST_ITEM_NAME == null ? string.Empty : arrrDetails.MAST_ITEM_NAME.ToString()):"" ,
                   arrrDetails.MAST_ITEM_TYPE == "Item" ? "<p style='margin-left: 1.5em;'>"+ (arrrDetails.MAST_ITEM_NAME == null ? string.Empty : arrrDetails.MAST_ITEM_NAME.ToString()) + "</p>" : (arrrDetails.MAST_ITEM_TYPE == "Major Item") ? "<p style='margin-left: 3em;'>" + (arrrDetails.MAST_ITEM_NAME == null ? string.Empty : arrrDetails.MAST_ITEM_NAME.ToString()) + "</p>":  (arrrDetails.MAST_ITEM_TYPE == "Minor Item") ?  "<p style='margin-left: 5em;'>" + (arrrDetails.MAST_ITEM_NAME == null ? string.Empty : arrrDetails.MAST_ITEM_NAME.ToString()) + "</p>":"" ,
                    arrrDetails.MAST_ITEM_DESC == null ? string.Empty : arrrDetails.MAST_ITEM_DESC.ToString(),
                    //arrrDetails.MAST_ITEM_PARENT == 0 ? "Item" : (arrrDetails.MAST_MAJOR_SUBITEM_CODE > 0 && arrrDetails.MAST_MINOR_SUBITEM_CODE == 0) ? "Major Item" : (arrrDetails.MAST_MAJOR_SUBITEM_CODE > 0 && arrrDetails.MAST_MINOR_SUBITEM_CODE > 0) ? "Minor Item" : "",
                    arrrDetails.MAST_ITEM_TYPE,
                   arrrDetails.MAST_ITEM.ToString(),
                   arrrDetails.MAST_MAJOR_SUBITEM_CODE.ToString(),
                   arrrDetails.MAST_MINOR_SUBITEM_CODE.ToString(),

                    arrrDetails.MAST_ITEM_USER_CODE.ToString(),
                    (arrrDetails.MAST_MORD_REF == null || arrrDetails.MAST_MORD_REF == string.Empty) ? "-" : arrrDetails.MAST_MORD_REF,
     /*5*/          arrrDetails.MAST_ITEM_ACTIVE == "Y" ? "<center><a href='#' onclick='changeChapterstatus(\"" +  URLEncrypt.EncryptParameters1(new string[]{ "ItemCode=" + arrrDetails.MAST_ITEM_CODE.ToString().Trim() }) + "\"); return false;'>Yes</a></center>"
                    : "<center><a href='#' onclick='changeChapterstatus(\"" +  URLEncrypt.EncryptParameters1(new string[]{ "ItemCode=" + arrrDetails.MAST_ITEM_CODE.ToString().Trim() }) + "\"); return false;'>No</a></center>",

                    dbContext.MASTER_ARRR_ITEMS_MASTER.Where(m => m.MAST_ITEM_CODE == arrrDetails.MAST_ITEM_CODE).Any() ?
                    "<center><a href='#' class='ui-icon ui-icon-pencil' onclick='loadEditChapterDetails(\"" +  URLEncrypt.EncryptParameters1(new string[]{ "ItemCode=" + arrrDetails.MAST_ITEM_CODE.ToString().Trim() }) + "\"); return false;'>Edit</a></center>"
                    :("<center><table><tr><td style='border:none'><a href='#' title='Locked' class='ui-icon ui-icon-locked  ui-align-center'>Locked</a></td></tr></table></center>"),
                    dbContext.MASTER_ARRR_ITEMS_MASTER.Where(m => m.MAST_ITEM_CODE == arrrDetails.MAST_ITEM_CODE).Any() ?
                    "<center><a href='#' class='ui-icon ui-icon-trash' onclick='deleteChapterDetails(\"" +  URLEncrypt.EncryptParameters1(new string[]{ "ItemCode=" + arrrDetails.MAST_ITEM_CODE.ToString().Trim() }) + "\"); return false;'>Delete</a></center>"        
                    :("<center><table><tr><td style='border:none'><a href='#' title='Locked' class='ui-icon ui-icon-locked  ui-align-center'>Locked</a></td></tr></table></center>"),

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
                if (dbContext != null)
                {
                    dbContext.Dispose();
                }
            }

        }

        public List<SelectListItem> PopulateChapterList(bool flg)
        {
            PMGSYEntities dbContext = new PMGSYEntities();
            List<SelectListItem> ChapterList = new List<SelectListItem>();
            SelectListItem item;
            try
            {
                if (flg == true)
                {
                    item = new SelectListItem();
                    item.Text = "All";
                    item.Value = "0";
                    ChapterList.Add(item);
                }
                else
                {
                    item = new SelectListItem();
                    item.Text = "Select";
                    item.Value = "-1";
                    ChapterList.Add(item);
                }
                var s = (from a in dbContext.MASTER_ARRR_ITEM_HEAD orderby a.MAST_HEAD_CODE select a).ToList();

                foreach (var a in s)
                {
                    item = new SelectListItem();
                    item.Text = a.MAST_HEAD_NAME;
                    item.Value = Convert.ToString(a.MAST_HEAD_CODE).Trim();
                    ChapterList.Add(item);
                }

                return ChapterList;
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

        public List<SelectListItem> PopulateItemsList(bool flg, int headCode)
        {
            PMGSYEntities dbContext = new PMGSYEntities();
            List<SelectListItem> ItemList = new List<SelectListItem>();
            SelectListItem item;
            try
            {
                if (flg == true)
                {
                    item = new SelectListItem();
                    item.Text = "All";
                    item.Value = "0";
                    ItemList.Add(item);
                }
                else
                {
                    item = new SelectListItem();
                    item.Text = "Select";
                    item.Value = "-1";
                    ItemList.Add(item);
                }
                var s = (from a in dbContext.MASTER_ARRR_ITEMS_MASTER where a.MAST_ITEM_PARENT == 0 && a.MAST_HEAD_CODE == headCode && a.MAST_MAJOR_SUBITEM_CODE == 0 && a.MAST_MINOR_SUBITEM_CODE == 0 orderby a.MAST_ITEM_CODE select a).ToList();

                foreach (var a in s)
                {
                    item = new SelectListItem();
                    item.Text = a.MAST_ITEM_NAME;
                    item.Value = Convert.ToString(a.MAST_ITEM_CODE).Trim();
                    ItemList.Add(item);
                }
                return ItemList;
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

        public List<SelectListItem> PopulateMajorItemsListItemwise(bool flg, int itemCode)
        {
            PMGSYEntities dbContext = new PMGSYEntities();
            List<SelectListItem> MajorItemList = new List<SelectListItem>();
            SelectListItem item;
            try
            {
                if (flg == true)
                {
                    item = new SelectListItem();
                    item.Text = "All";
                    item.Value = "0";
                    MajorItemList.Add(item);
                }
                else
                {
                    item = new SelectListItem();
                    item.Text = "Select";
                    item.Value = "-1";
                    MajorItemList.Add(item);
                }
                if (itemCode > 0)
                {
                    var s = (from a in dbContext.MASTER_ARRR_ITEMS_MASTER where (a.MAST_MAJOR_SUBITEM_CODE > 0 && a.MAST_MINOR_SUBITEM_CODE == 0 && a.MAST_ITEM_PARENT == itemCode) orderby a.MAST_MAJOR_SUBITEM_CODE select a).ToList();

                    foreach (var a in s)
                    {
                        item = new SelectListItem();
                        item.Text = a.MAST_ITEM_NAME;
                        //item.Value = Convert.ToString(a.MAST_MAJOR_SUBITEM_CODE).Trim();
                        item.Value = Convert.ToString(a.MAST_ITEM_CODE).Trim();
                        MajorItemList.Add(item);
                    }
                }
                return MajorItemList;
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

        public List<SelectListItem> PopulateMajorItemsList(bool flg, int itemCode)
        {
            PMGSYEntities dbContext = new PMGSYEntities();
            List<SelectListItem> MajorItemList = new List<SelectListItem>();
            SelectListItem item;
            try
            {
                if (flg == true)
                {
                    item = new SelectListItem();
                    item.Text = "All";
                    item.Value = "0";
                    MajorItemList.Add(item);
                }
                else
                {
                    item = new SelectListItem();
                    item.Text = "Select";
                    item.Value = "-1";
                    MajorItemList.Add(item);
                }
                if (itemCode > 0)
                {
                    var s = (from a in dbContext.MASTER_ARRR_ITEMS_MASTER where (a.MAST_MAJOR_SUBITEM_CODE > 0 && a.MAST_MINOR_SUBITEM_CODE == 0 && a.MAST_ITEM_CODE == itemCode) orderby a.MAST_MAJOR_SUBITEM_CODE select a).ToList();

                    foreach (var a in s)
                    {
                        item = new SelectListItem();
                        item.Text = a.MAST_ITEM_NAME;
                        //item.Value = Convert.ToString(a.MAST_MAJOR_SUBITEM_CODE).Trim();
                        item.Value = Convert.ToString(a.MAST_ITEM_CODE).Trim();
                        MajorItemList.Add(item);
                    }
                }
                return MajorItemList;
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

        public bool addARRRChaptersDAL(PMGSY.Models.ARRR.ChapterItemsViewModel model, ref string message)
        {
            dbContext = new PMGSYEntities();
            //string message = "";
            try
            {
                int dropId = 0;
                int recordCount = 0;

                PMGSY.Models.MASTER_ARRR_ITEMS_MASTER arrr = new MASTER_ARRR_ITEMS_MASTER();

                dbContext = new PMGSYEntities();
                recordCount = dbContext.MASTER_ARRR_ITEMS_MASTER.Where(m => m.MAST_ITEM_CODE == model.ItemCode && m.MAST_ITEM_NAME == model.ItemName).Count();
                if (recordCount > 0)
                {
                    message = "Item details already exists.";
                    return false;
                }
                arrr.MAST_ITEM = model.ItemType == "I" ? (dbContext.MASTER_ARRR_ITEMS_MASTER.Max(cp => (Int32?)cp.MAST_ITEM) == null ? 1 : (Int32)dbContext.MASTER_ARRR_ITEMS_MASTER.Max(cp => (Int32?)cp.MAST_ITEM) + 1) : model.Chapter;
                arrr.MAST_ITEM_CODE = dbContext.MASTER_ARRR_ITEMS_MASTER.Max(cp => (Int32?)cp.MAST_ITEM_CODE) == null ? 1 : (Int32)dbContext.MASTER_ARRR_ITEMS_MASTER.Max(cp => (Int32?)cp.MAST_ITEM_CODE) + 1;
                arrr.MAST_HEAD_CODE = model.Chapter;
                arrr.MAST_ITEM_DESC = model.ItemDesc;
                arrr.MAST_ITEM_NAME = model.ItemName;

                arrr.MAST_ITEM_ACTIVE = "Y";//model.ItemActive;
                arrr.MAST_ITEM_UNIT = model.ItemUnit;
                arrr.MAST_MAJOR_SUBITEM_CODE = model.ItemType == "M" ? (dbContext.MASTER_ARRR_ITEMS_MASTER.Max(cp => (Int32?)cp.MAST_MAJOR_SUBITEM_CODE) == null ? 1 : (Int32)dbContext.MASTER_ARRR_ITEMS_MASTER.Max(cp => (Int32?)cp.MAST_MAJOR_SUBITEM_CODE) + 1) : model.MajorItem;
                arrr.MAST_MINOR_SUBITEM_CODE = model.ItemType == "N" ? (dbContext.MASTER_ARRR_ITEMS_MASTER.Max(cp => (Int32?)cp.MAST_MINOR_SUBITEM_CODE) == null ? 1 : (Int32)dbContext.MASTER_ARRR_ITEMS_MASTER.Max(cp => (Int32?)cp.MAST_MINOR_SUBITEM_CODE) + 1) : model.MinorItem;

                arrr.MAST_ITEM_PARENT = model.ItemType == "M" ? model.Item : model.ItemType == "N" ? model.MajorItem : model.Parent;

                arrr.MAST_MORD_REF = model.mordRef == null ? null : model.mordRef.Trim();
                arrr.MAST_ITEM_USER_CODE = model.itemUserCode;

                arrr.USERID = PMGSYSession.Current.UserId;
                arrr.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];

                dbContext.MASTER_ARRR_ITEMS_MASTER.Add(arrr);
                dbContext.SaveChanges();

                message = model.ItemType == "I" ? "Item details saved successfully." : model.ItemType == "M" ? "Major Item details saved successfully." : "Minor Item details saved successfully.";
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

        public bool changeChapterstatusDAL(int ItemCode)
        {
            dbContext = new PMGSYEntities();
            try
            {
                PMGSY.Models.MASTER_ARRR_ITEMS_MASTER arrr = dbContext.MASTER_ARRR_ITEMS_MASTER.Where(m => m.MAST_ITEM_CODE == ItemCode).FirstOrDefault();
                //Added by abhishek kamble 27-nov-2013

                arrr.MAST_ITEM_ACTIVE = arrr.MAST_ITEM_ACTIVE == "N" ? "Y" : "N";
                //dbContext.MASTER_ARR_ITEMS_MASTER.Remove(arrr);
                dbContext.Entry(arrr).State = System.Data.Entity.EntityState.Modified;
                dbContext.SaveChanges();
                //message = "Clearance details deleted successfully.";
                return true;
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                return false;
            }
        }

        public bool editARRRChaptersDAL(PMGSY.Models.ARRR.ChapterItemsViewModel model, ref string message)
        {
            dbContext = new PMGSYEntities();
            //string message = "";
            try
            {
                int dropId = 0;
                int recordCount = 0;

                PMGSY.Models.MASTER_ARRR_ITEMS_MASTER arrr = new MASTER_ARRR_ITEMS_MASTER();

                dbContext = new PMGSYEntities();
                var s = dbContext.MASTER_ARRR_ITEMS_MASTER.Where(m => m.MAST_ITEM_CODE == model.ItemCode && m.MAST_ITEM_NAME != model.ItemName).ToList();
                foreach (var itm in s)
                {
                    if (itm.MAST_ITEM_NAME == model.ItemName)
                    {
                        message = "Item details already exists.";
                        return false;
                    }
                }
                arrr = dbContext.MASTER_ARRR_ITEMS_MASTER.Where(m => m.MAST_ITEM_CODE == model.ItemCode).FirstOrDefault();
                //arrr.MAST_ITEM_CODE = model.ItemCode;
                arrr.MAST_ITEM = model.Item;
                arrr.MAST_HEAD_CODE = model.Chapter;
                arrr.MAST_ITEM_DESC = model.ItemDesc;
                arrr.MAST_ITEM_NAME = model.ItemName;
                arrr.MAST_ITEM_PARENT = model.Parent;
                arrr.MAST_ITEM_ACTIVE = model.ItemActive;
                arrr.MAST_ITEM_UNIT = model.ItemUnit;
                arrr.MAST_MAJOR_SUBITEM_CODE = model.hdnMajorItem;
                arrr.MAST_MINOR_SUBITEM_CODE = model.MinorItem;
                arrr.MAST_MORD_REF = model.mordRef == null ? null : model.mordRef.Trim();

                arrr.MAST_ITEM_USER_CODE = model.itemUserCode;

                arrr.USERID = PMGSYSession.Current.UserId;
                arrr.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];

                dbContext.Entry(arrr).State = System.Data.Entity.EntityState.Modified;
                dbContext.SaveChanges();

                message = model.ItemType == "I" ? "Item details edited successfully." : model.ItemType == "M" ? "Major Item details edited successfully." : "Minor Item details edited successfully.";
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

        public bool deleteARRRChaptersDAL(int ItemCode)
        {
            dbContext = new PMGSYEntities();
            try
            {
                PMGSY.Models.MASTER_ARRR_ITEMS_MASTER arrr = dbContext.MASTER_ARRR_ITEMS_MASTER.Where(m => m.MAST_ITEM_CODE == ItemCode).FirstOrDefault();
                //Added by abhishek kamble 27-nov-2013

                dbContext.MASTER_ARRR_ITEMS_MASTER.Remove(arrr);
                dbContext.SaveChanges();
                //message = "Clearance details deleted successfully.";
                return true;
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                return false;
            }
        }
        #endregion

        #region Machinery Master
        public List<SelectListItem> PopulateCategoryDAL(bool flg, int lmmType)
        {
            PMGSYEntities dbContext = new PMGSYEntities();
            List<SelectListItem> CategoryList = new List<SelectListItem>();
            SelectListItem item;
            CategoryList = new List<SelectListItem>();
            try
            {
                if (flg == true)
                {
                    CategoryList.Insert(0, new SelectListItem { Text = "All", Value = "0" });
                }
                else
                {
                    CategoryList.Insert(0, new SelectListItem { Text = "Select", Value = "-1" });
                }
                var s = (from a in dbContext.MASTER_ARRR_LMM_CATEGORY where a.MAST_LMM_TYPE == lmmType orderby a.MAST_LMM_CAT_CODE select a).ToList();

                foreach (var a in s)
                {
                    item = new SelectListItem();
                    item.Text = a.MAST_LMM_CATEGORY;
                    item.Value = Convert.ToString(a.MAST_LMM_CAT_CODE).Trim();
                    CategoryList.Add(item);
                }

                return CategoryList;
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

        public List<SelectListItem> PopulateUsageUnitDAL()
        {
            PMGSYEntities dbContext = new PMGSYEntities();
            List<SelectListItem> UnitList = new List<SelectListItem>();
            SelectListItem item;
            try
            {

                var s = (from a in dbContext.MASTER_UNITS where a.MAST_UNIT_DIMENSION == 4 orderby a.MAST_UNIT_CODE select a).ToList();

                foreach (var a in s)
                {
                    item = new SelectListItem();
                    item.Text = a.MAST_UNIT_NAME;
                    item.Value = Convert.ToString(a.MAST_UNIT_CODE).Trim();
                    UnitList.Add(item);
                }

                return UnitList;
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

        public List<SelectListItem> PopulateOutputUnitDAL()
        {
            PMGSYEntities dbContext = new PMGSYEntities();
            List<SelectListItem> UnitList = new List<SelectListItem>();
            SelectListItem item;
            try
            {

                var s = (from a in dbContext.MASTER_UNITS where a.MAST_UNIT_DIMENSION == 3 orderby a.MAST_UNIT_CODE select a).ToList();

                foreach (var a in s)
                {
                    item = new SelectListItem();
                    item.Text = a.MAST_UNIT_NAME;
                    item.Value = Convert.ToString(a.MAST_UNIT_CODE).Trim();
                    UnitList.Add(item);
                }

                return UnitList;
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

        public Array ListMachineryMasterDAL(int page, int rows, string sidx, string sord, out long totalRecords)
        {
            try
            {
                dbContext = new PMGSYEntities();

                var list = from arrrlist in dbContext.MASTER_ARRR_LMM_TYPES
                           where arrrlist.MAST_LMM_TYPE == 2

                           select new
                           {
                               arrrlist.MAST_LMM_TYPE_CODE,
                               arrrlist.MAST_LMM_DESC,
                               arrrlist.MAST_LMM_TYPE,
                               arrrlist.MAST_LMM_ACTIVE,
                               arrrlist.MAST_LMM_ACTY_CODE,
                               arrrlist.MAST_LMM_ACTIVITY,
                               arrrlist.MAST_LMM_OUTPUT_RATE,
                               arrrlist.MAST_LMM_OUTPUT_UNIT,
                               arrrlist.MAST_LMM_USAGE_UNIT,
                               arrrlist.MAST_LMM_CODE,
                               arrrlist.MAST_LMM_CAT_CODE,
                           };

                totalRecords = list.Count();

                if (sidx.Trim() != string.Empty)
                {
                    if (sord.ToString() == "asc")
                    {
                        switch (sidx)
                        {
                            case "MAST_LMM_TYPE_CODE":
                                list = list.OrderBy(x => x.MAST_LMM_TYPE_CODE).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                                break;
                            case "MAST_LMM_DESC":
                                list = list.OrderBy(x => x.MAST_LMM_DESC).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                                break;
                            case "MAST_LMM_TYPE":
                                list = list.OrderBy(x => x.MAST_LMM_TYPE).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                                break;
                            default:
                                list = list.OrderBy(x => x.MAST_LMM_TYPE_CODE).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                                break;
                        }
                    }
                    else
                    {
                        switch (sidx)
                        {
                            case "MAST_LMM_TYPE_CODE":
                                list = list.OrderByDescending(x => x.MAST_LMM_TYPE_CODE).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                                break;
                            case "MAST_LMM_DESC":
                                list = list.OrderByDescending(x => x.MAST_LMM_DESC).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                                break;
                            case "MAST_LMM_TYPE":
                                list = list.OrderByDescending(x => x.MAST_LMM_TYPE).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                                break;
                            default:
                                list = list.OrderByDescending(x => x.MAST_LMM_TYPE_CODE).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                                break;
                        }
                    }
                }
                else
                {
                    list = list.OrderBy(x => x.MAST_LMM_TYPE_CODE).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                }

                var result = list.Select(arrrlist => new
                {
                    arrrlist.MAST_LMM_TYPE_CODE,
                    arrrlist.MAST_LMM_DESC,
                    arrrlist.MAST_LMM_TYPE,
                    arrrlist.MAST_LMM_ACTIVE,
                    arrrlist.MAST_LMM_ACTY_CODE,
                    arrrlist.MAST_LMM_ACTIVITY,
                    arrrlist.MAST_LMM_OUTPUT_RATE,
                    arrrlist.MAST_LMM_OUTPUT_UNIT,
                    arrrlist.MAST_LMM_USAGE_UNIT,
                    arrrlist.MAST_LMM_CODE,
                    arrrlist.MAST_LMM_CAT_CODE,
                }).ToArray();

                return result.Select(arrrDetails => new
                {
                    id = arrrDetails.MAST_LMM_TYPE_CODE.ToString().Trim(),//URLEncrypt.EncryptParameters1(new string[] { "AdminCode =" + adminDetails.ADMIN_ND_CODE.ToString().Trim() }),

                    cell = new[]{
                    
     /*3*/          arrrDetails.MAST_LMM_DESC == null ? string.Empty : arrrDetails.MAST_LMM_DESC.ToString(),
                    //arrrDetails.MAST_LMM_TYPE == null ? string.Empty : arrrDetails.MAST_LMM_TYPE.ToString(),
     /*5*/          
                    //arrrDetails.MAST_LMM_ACTY_CODE == null ? string.Empty : arrrDetails.MAST_LMM_ACTY_CODE.ToString(),
                    arrrDetails.MAST_LMM_ACTIVITY == null ? string.Empty : arrrDetails.MAST_LMM_ACTIVITY.ToString(),
                    arrrDetails.MAST_LMM_OUTPUT_RATE == null ? string.Empty : arrrDetails.MAST_LMM_OUTPUT_RATE.ToString(),
                    arrrDetails.MAST_LMM_OUTPUT_UNIT == null ? "" : arrrDetails.MAST_LMM_OUTPUT_UNIT.ToString(),
                    //arrrDetails.MAST_LMM_USAGE_UNIT == null ? "" : arrrDetails.MAST_LMM_USAGE_UNIT.ToString(),
                    dbContext.MASTER_UNITS.Where(m => m.MAST_UNIT_CODE == arrrDetails.MAST_LMM_USAGE_UNIT).Select(a=>a.MAST_UNIT_NAME).FirstOrDefault(),
                    arrrDetails.MAST_LMM_CODE,
                    dbContext.MASTER_ARRR_LMM_TYPES.Where(m => m.MAST_LMM_TYPE_CODE == arrrDetails.MAST_LMM_TYPE_CODE).Select(a=>a.MASTER_ARRR_LMM_CATEGORY.MAST_LMM_CATEGORY).FirstOrDefault(),
                    
                    arrrDetails.MAST_LMM_ACTIVE == "Y" ? "<center><a href='#' onclick='changeMachineryMasterstatus(\"" +  URLEncrypt.EncryptParameters1(new string[]{ "lmmCode=" + arrrDetails.MAST_LMM_TYPE_CODE.ToString().Trim() }) + "\"); return false;'>Yes</a></center>" 
                    : "<center><a href='#' onclick='changeMachineryMasterstatus(\"" +  URLEncrypt.EncryptParameters1(new string[]{ "lmmCode=" + arrrDetails.MAST_LMM_TYPE_CODE.ToString().Trim() }) + "\"); return false;'>No</a></center>",
                    dbContext.MASTER_ARRR_LMM_TYPES.Where(m => m.MAST_LMM_TYPE_CODE == arrrDetails.MAST_LMM_TYPE_CODE).Any() ?
                    "<center><a href='#' class='ui-icon ui-icon-pencil' onclick='loadEditMachineryMasterDetails(\"" +  URLEncrypt.EncryptParameters1(new string[]{ "lmmCode=" + arrrDetails.MAST_LMM_TYPE_CODE.ToString().Trim() }) + "\"); return false;'>Edit</a></center>"
                    :("<center><table><tr><td style='border:none'><a href='#' title='Locked' class='ui-icon ui-icon-locked  ui-align-center'>Locked</a></td></tr></table></center>"),
                    
                    dbContext.MASTER_ARRR_LMM_TYPES.Where(m => m.MAST_LMM_TYPE_CODE == arrrDetails.MAST_LMM_TYPE_CODE).Any() ?
                    "<center><a href='#' class='ui-icon ui-icon-trash' onclick='deleteMachineryMaster(\"" +  URLEncrypt.EncryptParameters1(new string[]{ "lmmCode=" + arrrDetails.MAST_LMM_TYPE_CODE.ToString().Trim() }) + "\"); return false;'>Delete</a></center>"        
                    :("<center><table><tr><td style='border:none'><a href='#' title='Locked' class='ui-icon ui-icon-locked  ui-align-center'>Locked</a></td></tr></table></center>"),

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
                if (dbContext != null)
                {
                    dbContext.Dispose();
                }
            }

        }

        public bool addARRRMachineryDAL(PMGSY.Models.ARRR.MachineryMasterViewModel model, ref string message)
        {
            dbContext = new PMGSYEntities();
            //string message = "";
            try
            {
                int dropId = 0;
                int recordCount = 0;

                PMGSY.Models.MASTER_ARRR_LMM_TYPES arrr = new MASTER_ARRR_LMM_TYPES();

                dbContext = new PMGSYEntities();
                //recordCount = dbContext.MASTER_ARRR_LMM_TYPES.Where(m => m.MAST_LMM_DESC == model.Description).Count();
                //if (recordCount > 0)
                //{
                //    message = "Machinery Master details already exists.";
                //    return false;
                //}

                arrr.MAST_LMM_TYPE_CODE = dbContext.MASTER_ARRR_LMM_TYPES.Max(cp => (Int32?)cp.MAST_LMM_TYPE_CODE) == null ? 1 : (Int32)dbContext.MASTER_ARRR_LMM_TYPES.Max(cp => (Int32?)cp.MAST_LMM_TYPE_CODE) + 1;
                arrr.MAST_LMM_DESC = model.Description;

                arrr.MAST_LMM_TYPE = model.lmmType;
                arrr.MAST_LMM_ACTIVE = model.flag;
                arrr.MAST_LMM_ACTIVITY = model.Activity;
                arrr.MAST_LMM_ACTY_CODE = dbContext.MASTER_ARRR_LMM_TYPES.Where(a => a.MAST_LMM_TYPE == 2).Max(cp => (Int32?)cp.MAST_LMM_ACTY_CODE) == null ? 1 : (Int32)dbContext.MASTER_ARRR_LMM_TYPES.Where(a => a.MAST_LMM_TYPE == 2).Max(cp => (Int32?)cp.MAST_LMM_ACTY_CODE) + 1;//model.lmmActyCode;
                arrr.MAST_LMM_OUTPUT_RATE = model.OutputRate;
                arrr.MAST_LMM_OUTPUT_UNIT = model.OutputUnit;
                arrr.MAST_LMM_USAGE_UNIT = model.UsageUnit;

                arrr.MAST_LMM_CODE = model.lCode;
                arrr.MAST_LMM_CAT_CODE = model.Category;

                arrr.USERID = PMGSYSession.Current.UserId;
                arrr.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];

                dbContext.MASTER_ARRR_LMM_TYPES.Add(arrr);
                dbContext.SaveChanges();

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

        public bool changeMachineryMasterstatusDAL(int lmmCode)
        {
            dbContext = new PMGSYEntities();
            try
            {
                PMGSY.Models.MASTER_ARRR_LMM_TYPES arrr = dbContext.MASTER_ARRR_LMM_TYPES.Where(m => m.MAST_LMM_TYPE_CODE == lmmCode).FirstOrDefault();
                //Added by abhishek kamble 27-nov-2013

                arrr.MAST_LMM_ACTIVE = arrr.MAST_LMM_ACTIVE == "N" ? "Y" : "N";
                //dbContext.MASTER_ARR_ITEMS_MASTER.Remove(arrr);
                dbContext.Entry(arrr).State = System.Data.Entity.EntityState.Modified;
                dbContext.SaveChanges();
                //message = "Clearance details deleted successfully.";
                return true;
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                return false;
            }
        }

        public bool editARRRMachineryDAL(PMGSY.Models.ARRR.MachineryMasterViewModel model, ref string message)
        {
            dbContext = new PMGSYEntities();
            //string message = "";
            try
            {
                int dropId = 0;
                int recordCount = 0;

                PMGSY.Models.MASTER_ARRR_LMM_TYPES arrr = new MASTER_ARRR_LMM_TYPES();

                dbContext = new PMGSYEntities();
                //var s = dbContext.MASTER_ARRR_LMM_TYPES.Where(m => m.MAST_LMM_TYPE_CODE != model.lmmCode && m.MAST_LMM_DESC == model.Description).ToList();
                //foreach (var itm in s)
                //{
                //    if (itm.MAST_LMM_DESC == model.Description)
                //    {
                //        message = "Machinery Master details already exists.";
                //        return false;
                //    }
                //}
                arrr = dbContext.MASTER_ARRR_LMM_TYPES.Where(m => m.MAST_LMM_TYPE_CODE == model.lmmCode).FirstOrDefault();
                //arrr.MAST_ITEM_CODE = model.ItemCode;
                arrr.MAST_LMM_DESC = model.Description;
                arrr.MAST_LMM_ACTIVE = model.flag;
                arrr.MAST_LMM_ACTIVITY = model.Activity;

                //arrr.MAST_LMM_ACTY_CODE = model.lmmActyCode;
                arrr.MAST_LMM_OUTPUT_RATE = model.OutputRate;
                arrr.MAST_LMM_OUTPUT_UNIT = model.OutputUnit;
                arrr.MAST_LMM_USAGE_UNIT = model.UsageUnit;

                arrr.MAST_LMM_CODE = model.lCode;
                arrr.MAST_LMM_CAT_CODE = model.Category;

                arrr.USERID = PMGSYSession.Current.UserId;
                arrr.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];

                dbContext.Entry(arrr).State = System.Data.Entity.EntityState.Modified;
                dbContext.SaveChanges();

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

        public bool deleteARRRMachineryDAL(int lmmCode)
        {
            dbContext = new PMGSYEntities();
            try
            {
                PMGSY.Models.MASTER_ARRR_LMM_TYPES arrr = dbContext.MASTER_ARRR_LMM_TYPES.Where(m => m.MAST_LMM_TYPE_CODE == lmmCode).FirstOrDefault();
                //Added by abhishek kamble 27-nov-2013

                dbContext.MASTER_ARRR_LMM_TYPES.Remove(arrr);
                dbContext.SaveChanges();
                //message = "Clearance details deleted successfully.";
                return true;
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                return false;
            }
        }
        #endregion

        #region Material Rates Master

        public List<SelectListItem> PopulateLMMItemsListDAL(bool flg, int lmmtype, int category)
        {
            PMGSYEntities dbContext = new PMGSYEntities();
            List<SelectListItem> ItemList = new List<SelectListItem>();
            SelectListItem item;
            try
            {
                if (flg == true)
                {
                    item = new SelectListItem();
                    item.Text = "All";
                    item.Value = "0";
                    ItemList.Add(item);
                }
                else
                {
                    item = new SelectListItem();
                    item.Text = "Select";
                    item.Value = "-1";
                    ItemList.Add(item);
                }
                var s = (from a in dbContext.MASTER_ARRR_LMM_TYPES where a.MAST_LMM_TYPE == lmmtype /*&& (category == 0 ? 1 == 1 : a.MAST_LMM_CAT_CODE == category)*/ orderby a.MAST_LMM_TYPE_CODE select a).ToList();

                foreach (var a in s)
                {
                    item = new SelectListItem();
                    item.Text = a.MAST_LMM_DESC;
                    item.Value = Convert.ToString(a.MAST_LMM_TYPE_CODE).Trim();
                    ItemList.Add(item);
                }

                return ItemList;
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
        public Array ListMaterialRatesDAL(int page, int rows, string sidx, string sord, int Year, out long totalRecords)
        {
            try
            {
                dbContext = new PMGSYEntities();
                if (Year == 0)
                {
                    Year = dbContext.MASTER_ARRR_STATES_RATES.Max(s => s.MAST_ARRR_YEAR); //To render Year-Wise Listing of existing records
                }
                var list = from arrrlist in dbContext.MASTER_ARRR_STATES_RATES
                           join lmmtype in dbContext.MASTER_ARRR_LMM_TYPES
                           on arrrlist.MAST_LMM_TYPE_CODE equals lmmtype.MAST_LMM_TYPE_CODE
                           where arrrlist.MAST_LMM_TYPE == 3 && arrrlist.MAST_STATE_CODE == PMGSYSession.Current.StateCode && arrrlist.MAST_ARRR_YEAR == Year
                           select new
                           {
                               arrrlist.MAST_ARRR_RATE_CODE,
                               arrrlist.MAST_STATE_CODE,
                               arrrlist.MAST_LMM_TYPE_CODE,
                               arrrlist.MAST_LMM_TYPE,
                               arrrlist.MAST_ARRR_RATE,
                               arrrlist.MAST_ARRR_YEAR,
                               arrrlist.MAST_ARRR_RATE_IS_FINAL,
                               arrrlist.ACTIVE_FLAG,
                               arrrlist.MAST_ARRR_FILENAME,
                               lmmtype.MAST_LMM_OUTPUT_UNIT,
                           };

                totalRecords = list.Count();

                if (sidx.Trim() != string.Empty)
                {
                    if (sord.ToString() == "asc")
                    {
                        switch (sidx)
                        {
                            case "MAST_ARRR_RATE_CODE":
                                list = list.OrderBy(x => x.MAST_ARRR_RATE_CODE).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                                break;
                            case "MAST_STATE_CODE":
                                list = list.OrderBy(x => x.MAST_STATE_CODE).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                                break;
                            case "MAST_LMM_TYPE":
                                list = list.OrderBy(x => x.MAST_LMM_TYPE).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                                break;
                            default:
                                list = list.OrderBy(x => x.MAST_ARRR_RATE_CODE).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                                break;
                        }
                    }
                    else
                    {
                        switch (sidx)
                        {
                            case "MAST_ARRR_RATE_CODE":
                                list = list.OrderByDescending(x => x.MAST_ARRR_RATE_CODE).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                                break;
                            case "MAST_STATE_CODE":
                                list = list.OrderByDescending(x => x.MAST_STATE_CODE).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                                break;
                            case "MAST_LMM_TYPE":
                                list = list.OrderByDescending(x => x.MAST_LMM_TYPE).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                                break;
                            default:
                                list = list.OrderByDescending(x => x.MAST_ARRR_RATE_CODE).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                                break;
                        }
                    }
                }
                else
                {
                    list = list.OrderBy(x => x.MAST_ARRR_RATE_CODE).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                }

                var result = list.Select(arrrlist => new
                {
                    arrrlist.MAST_ARRR_RATE_CODE,
                    arrrlist.MAST_STATE_CODE,
                    arrrlist.MAST_LMM_TYPE_CODE,
                    arrrlist.MAST_LMM_TYPE,
                    arrrlist.MAST_ARRR_RATE,
                    arrrlist.MAST_ARRR_YEAR,
                    arrrlist.MAST_ARRR_RATE_IS_FINAL,
                    arrrlist.ACTIVE_FLAG,
                    arrrlist.MAST_ARRR_FILENAME,
                    arrrlist.MAST_LMM_OUTPUT_UNIT,
                }).ToArray();

                return result.Select(arrrDetails => new
                {
                    id = arrrDetails.MAST_ARRR_RATE_CODE.ToString().Trim(),//URLEncrypt.EncryptParameters1(new string[] { "AdminCode =" + adminDetails.ADMIN_ND_CODE.ToString().Trim() }),

                    cell = new[]{
                    
     /*3*/          //dbContext.MASTER_STATE.Where(m=>m.MAST_STATE_CODE == arrrDetails.MAST_STATE_CODE).Select(m=>m.MAST_STATE_NAME).FirstOrDefault(),
                    dbContext.MASTER_ARRR_STATES_RATES.Where(m => m.MAST_ARRR_RATE_CODE == arrrDetails.MAST_ARRR_RATE_CODE).Select(m => m.MASTER_ARRR_LMM_TYPES.MASTER_ARRR_LMM_CATEGORY.MAST_LMM_CATEGORY).FirstOrDefault(),
                    dbContext.MASTER_ARRR_LMM_TYPES.Where(m=>m.MAST_LMM_TYPE_CODE == arrrDetails.MAST_LMM_TYPE_CODE).Select(m=>m.MAST_LMM_DESC).FirstOrDefault(),
                     dbContext.MASTER_UNITS.Where(m => m.MAST_UNIT_CODE == arrrDetails.MAST_LMM_OUTPUT_UNIT).Select(a=>a.MAST_UNIT_NAME).FirstOrDefault(),
     /*5*/          arrrDetails.MAST_ARRR_RATE == null ? "" : arrrDetails.MAST_ARRR_RATE.ToString(),
                    arrrDetails.MAST_ARRR_YEAR.ToString() + " - " + (arrrDetails.MAST_ARRR_YEAR+1).ToString(),
                    arrrDetails.MAST_ARRR_RATE_IS_FINAL == "N" ? "<a href='#'  onClick='FinalizeMaterialRate(\""+ PMGSY.Common.URLEncrypt.EncryptParameters1(new string[] { "rateCode ="+ arrrDetails.MAST_ARRR_RATE_CODE.ToString().Trim()}) +"\"); return false;'>Finalize</a>" : "Finalized",
                    arrrDetails.MAST_ARRR_RATE_IS_FINAL == "Y" ?
                    ("<center><table><tr><td style='border:none'><a href='#' title='Locked' class='ui-icon ui-icon-locked  ui-align-center'>Locked</a></td></tr></table></center>")
                    : (arrrDetails.ACTIVE_FLAG == "Y" ? "<center><a href='#' onclick='changeMaterialRatestatus(\"" +  URLEncrypt.EncryptParameters1(new string[]{ "rateCode=" + arrrDetails.MAST_ARRR_RATE_CODE.ToString().Trim() }) + "\"); return false;'>Yes</a></center>"
                    : "<center><a href='#' onclick='changeMaterialRatestatus(\"" +  URLEncrypt.EncryptParameters1(new string[]{ "rateCode=" + arrrDetails.MAST_ARRR_RATE_CODE.ToString().Trim() }) + "\"); return false;'>No</a></center>"),

                    //dbContext.MASTER_ARRR_LMM_TYPES.Where(m => m.MAST_LMM_CODE == arrrDetails.MAST_ARRR_RATE_CODE).Any() ?
                    arrrDetails.MAST_ARRR_RATE_IS_FINAL == "N" ?
                    "<center><a href='#' class='ui-icon ui-icon-pencil' onclick='loadEditMaterialRateDetails(\"" +  URLEncrypt.EncryptParameters1(new string[]{ "rateCode=" + arrrDetails.MAST_ARRR_RATE_CODE.ToString().Trim() }) + "\"); return false;'>Edit</a></center>"
                    :("<center><table><tr><td style='border:none'><a href='#' title='Locked' class='ui-icon ui-icon-locked  ui-align-center'>Locked</a></td></tr></table></center>"),
                    //dbContext.MASTER_ARRR_LMM_TYPES.Where(m => m.MAST_LMM_CODE == arrrDetails.MAST_ARRR_RATE_CODE).Any() ?
                    arrrDetails.MAST_ARRR_RATE_IS_FINAL == "N" ?
                    "<center><a href='#' class='ui-icon ui-icon-trash' onclick='deleteMaterialRate(\"" +  URLEncrypt.EncryptParameters1(new string[]{ "rateCode=" + arrrDetails.MAST_ARRR_RATE_CODE.ToString().Trim() }) + "\"); return false;'>Delete</a></center>"        
                    :("<center><table><tr><td style='border:none'><a href='#' title='Locked' class='ui-icon ui-icon-locked  ui-align-center'>Locked</a></td></tr></table></center>"),
               
                     URLEncrypt.EncryptParameters(new string[] { arrrDetails.MAST_ARRR_FILENAME + "$" +  arrrDetails.MAST_ARRR_RATE_CODE }),
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
                if (dbContext != null)
                {
                    dbContext.Dispose();
                }
            }

        }


        public bool addARRRMaterialRatesDAL(PMGSY.Models.ARRR.LMMRateViewModel model, ref string message)
        {
            dbContext = new PMGSYEntities();
            //string message = "";
            try
            {
                int dropId = 0;
                int recordCount = 0;

                PMGSY.Models.MASTER_ARRR_STATES_RATES arrr = new MASTER_ARRR_STATES_RATES();

                dbContext = new PMGSYEntities();



                string StateName = PMGSYSession.Current.StateName;
                int StateCode = PMGSYSession.Current.StateCode;
                int YearCode = model.Year;
                //
                recordCount = dbContext.MASTER_ARRR_STATES_RATES.Where(m => m.MAST_LMM_TYPE_CODE == model.Item && m.MAST_STATE_CODE == StateCode && m.MAST_ARRR_YEAR == YearCode).Count();
                if (recordCount > 0)
                {
                    message = "Material Rate details already exists for " + StateName + ", for Year " + YearCode + " and for selected type.";
                    return false;
                }

                //recordCount = dbContext.MASTER_ARRR_STATES_RATES.Where(m => m.MASTER_ARRR_LMM_TYPES.MASTER_ARRR_LMM_CATEGORY.MAST_LMM_CAT_CODE == model.Category).Count();
                //if (recordCount > 0)
                //{
                //    message = "Material Rate details already exists for selected category.";
                //    return false;
                //}

                arrr.MAST_ARRR_RATE_CODE = dbContext.MASTER_ARRR_STATES_RATES.Max(cp => (Int32?)cp.MAST_ARRR_RATE_CODE) == null ? 1 : (Int32)dbContext.MASTER_ARRR_STATES_RATES.Max(cp => (Int32?)cp.MAST_ARRR_RATE_CODE) + 1;
                arrr.MAST_STATE_CODE = PMGSYSession.Current.StateCode; //model.stateCode;

                arrr.ACTIVE_FLAG = model.flag;

                arrr.MAST_ARRR_RATE = model.Rate;
                arrr.MAST_ARRR_YEAR = model.Year;
                arrr.MAST_LMM_TYPE = model.rateType;
                arrr.MAST_LMM_TYPE_CODE = model.Item;//dbContext.MASTER_ARRR_STATES_RATES.Max(cp => (Int32?)cp.MAST_LMM_TYPE_CODE) == null ? 1 : (Int32)dbContext.MASTER_ARRR_STATES_RATES.Max(cp => (Int32?)cp.MAST_LMM_TYPE_CODE) + 1;
                arrr.MAST_ARRR_RATE_IS_FINAL = model.status;

                arrr.USERID = PMGSYSession.Current.UserId;
                arrr.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];

                dbContext.MASTER_ARRR_STATES_RATES.Add(arrr);
                dbContext.SaveChanges();

                return true;
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                if (ex.InnerException.InnerException.Message == "The INSERT statement conflicted with the FOREIGN KEY constraint \"FK_MASTER_ARRR_STATES_RATES_MASTER_ARRR_LMM_TYPES\". The conflict occurred in database \"OMMAS_DEV\", table \"omms.MASTER_ARRR_LMM_TYPES\", column 'MAST_LMM_CODE'.\r\nThe statement has been terminated.")
                //ex.InnerException.InnerException.Message = "The INSERT statement conflicted with the FOREIGN KEY constraint \"FK_MASTER_ARRR_STATES_RATES_MASTER_ARRR_LMM_TYPES\". The conflict occurred in database \"OMMAS_DEV\", table \"omms.MASTER_ARRR_LMM_TYPES\", column 'MAST_LMM_CODE'.\r\nThe statement has been te...";
                {
                    message = "No details present for LMM Type";
                }
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

        public bool changeLMMRatestatusDAL(int rateCode)
        {
            dbContext = new PMGSYEntities();
            try
            {
                PMGSY.Models.MASTER_ARRR_STATES_RATES arrr = dbContext.MASTER_ARRR_STATES_RATES.Where(m => m.MAST_ARRR_RATE_CODE == rateCode).FirstOrDefault();
                //Added by abhishek kamble 27-nov-2013

                arrr.ACTIVE_FLAG = arrr.ACTIVE_FLAG == "N" ? "Y" : "N";
                //dbContext.MASTER_ARR_ITEMS_MASTER.Remove(arrr);
                dbContext.Entry(arrr).State = System.Data.Entity.EntityState.Modified;
                dbContext.SaveChanges();
                //message = "Clearance details deleted successfully.";
                return true;
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                return false;
            }
        }

        public bool MaterialRateFinalizeDAL(int rateCode)
        {
            bool flg = false;
            try
            {
                dbContext = new PMGSY.Models.PMGSYEntities();

                PMGSY.Models.MASTER_ARRR_STATES_RATES arrr = new PMGSY.Models.MASTER_ARRR_STATES_RATES();

                arrr = dbContext.MASTER_ARRR_STATES_RATES.Where(s => s.MAST_ARRR_RATE_CODE == rateCode).FirstOrDefault();
                PMGSY.Common.CommonFunctions comm = new PMGSY.Common.CommonFunctions();

                arrr.MAST_ARRR_RATE_IS_FINAL = "Y";

                dbContext.Entry(arrr).State = System.Data.Entity.EntityState.Modified;
                dbContext.SaveChanges();
                flg = true;

                return flg;
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

        public bool editARRRMaterialRatesDAL(PMGSY.Models.ARRR.LMMRateViewModel model, ref string message)
        {
            dbContext = new PMGSYEntities();
            //string message = "";
            try
            {
                int dropId = 0;
                int recordCount = 0;

                PMGSY.Models.MASTER_ARRR_STATES_RATES arrr = new MASTER_ARRR_STATES_RATES();

                dbContext = new PMGSYEntities();

                recordCount = dbContext.MASTER_ARRR_STATES_RATES.Where(m => m.MAST_ARRR_RATE_CODE == model.rateCode && m.MAST_LMM_TYPE_CODE == model.hdnItem).Count();
                if (!(recordCount == 1))
                {
                    message = "Material Rate details already exists for selected type.";
                    return false;
                }

                arrr = dbContext.MASTER_ARRR_STATES_RATES.Where(m => m.MAST_ARRR_RATE_CODE == model.rateCode).FirstOrDefault();
                //arrr.MAST_STATE_CODE = model.stateCode;

                arrr.ACTIVE_FLAG = model.flag;
                arrr.MAST_ARRR_YEAR = model.Year;
                arrr.MAST_ARRR_RATE = model.Rate;
                //arrr.MAST_LMM_TYPE = model.rateType;
                //arrr.MAST_LMM_TYPE_CODE = model.Item;
                //arrr.MAST_ARRR_RATE_IS_FINAL = model.status;

                arrr.USERID = PMGSYSession.Current.UserId;
                arrr.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];

                dbContext.Entry(arrr).State = System.Data.Entity.EntityState.Modified;
                dbContext.SaveChanges();

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

        public bool deleteARRRMaterialRatesDAL(int rateCode)
        {
            dbContext = new PMGSYEntities();
            try
            {
                PMGSY.Models.MASTER_ARRR_STATES_RATES arrr = dbContext.MASTER_ARRR_STATES_RATES.Where(m => m.MAST_ARRR_RATE_CODE == rateCode).FirstOrDefault();
                //Added by abhishek kamble 27-nov-2013

                dbContext.MASTER_ARRR_STATES_RATES.Remove(arrr);
                dbContext.SaveChanges();
                //message = "Clearance details deleted successfully.";
                return true;
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                return false;
            }
        }


        public Array ListMaterialFormDAL(int page, int rows, string sidx, string sord, out long totalRecords)
        {
            try
            {
                dbContext = new PMGSYEntities();

                var list = from arrrlist in dbContext.MASTER_ARRR_LMM_TYPES
                           join cat in dbContext.MASTER_ARRR_LMM_CATEGORY
                                     on arrrlist.MAST_LMM_CAT_CODE equals cat.MAST_LMM_CAT_CODE
                           where arrrlist.MAST_LMM_TYPE == 3   /*&& arrrlist.MAST_STATE_CODE == PMGSYSession.Current.StateCode*/
                           select new
                           {
                               arrrlist.MAST_LMM_TYPE_CODE,
                               arrrlist.MAST_LMM_TYPE,
                               arrrlist.MAST_LMM_ACTY_CODE,
                               arrrlist.MAST_LMM_DESC,
                               arrrlist.MAST_LMM_CAT_CODE,
                               arrrlist.MAST_LMM_CODE,
                               arrrlist.MAST_LMM_OUTPUT_UNIT,
                           };

                totalRecords = list.Count();

                if (sidx.Trim() != string.Empty)
                {
                    if (sord.ToString() == "asc")
                    {
                        switch (sidx)
                        {
                            case "MAST_LMM_TYPE_CODE":
                                list = list.OrderBy(x => x.MAST_LMM_TYPE_CODE).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                                break;
                            //case "MAST_STATE_CODE":
                            //    list = list.OrderBy(x => x.MAST_STATE_CODE).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                            //    break;
                            case "MAST_LMM_TYPE":
                                list = list.OrderBy(x => x.MAST_LMM_TYPE).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                                break;
                            default:
                                list = list.OrderBy(x => x.MAST_LMM_TYPE_CODE).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                                break;
                        }
                    }
                    else
                    {
                        switch (sidx)
                        {
                            case "MAST_LMM_TYPE_CODE":
                                list = list.OrderByDescending(x => x.MAST_LMM_TYPE_CODE).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                                break;
                            //case "MAST_STATE_CODE":
                            //    list = list.OrderByDescending(x => x.MAST_STATE_CODE).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                            //    break;
                            case "MAST_LMM_TYPE":
                                list = list.OrderByDescending(x => x.MAST_LMM_TYPE).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                                break;
                            default:
                                list = list.OrderByDescending(x => x.MAST_LMM_TYPE_CODE).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                                break;
                        }
                    }
                }
                else
                {
                    list = list.OrderBy(x => x.MAST_LMM_TYPE_CODE).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                }

                var result = list.Select(arrrlist => new
                {
                    arrrlist.MAST_LMM_TYPE_CODE,
                    arrrlist.MAST_LMM_TYPE,
                    arrrlist.MAST_LMM_ACTY_CODE,
                    arrrlist.MAST_LMM_DESC,
                    arrrlist.MAST_LMM_CAT_CODE,
                    arrrlist.MAST_LMM_CODE,
                    arrrlist.MAST_LMM_OUTPUT_UNIT

                }).ToArray();

                return result.Select(arrrDetails => new
                {
                    id = arrrDetails.MAST_LMM_TYPE_CODE.ToString().Trim(),

                    cell = new[]{
                         dbContext.MASTER_ARRR_LMM_TYPES.Where(m=>m.MAST_LMM_TYPE_CODE == arrrDetails.MAST_LMM_TYPE_CODE).Select(m=>m.MAST_LMM_CODE).FirstOrDefault(),
                         dbContext.MASTER_ARRR_LMM_CATEGORY.Where(m => m.MAST_LMM_CAT_CODE == arrrDetails.MAST_LMM_CAT_CODE).Select(m => m.MAST_LMM_CATEGORY).FirstOrDefault(),
                         dbContext.MASTER_ARRR_LMM_TYPES.Where(m=>m.MAST_LMM_TYPE_CODE == arrrDetails.MAST_LMM_TYPE_CODE).Select(m=>m.MAST_LMM_DESC).FirstOrDefault(),
                         dbContext.MASTER_UNITS.Where(m => m.MAST_UNIT_CODE == arrrDetails.MAST_LMM_OUTPUT_UNIT).Select(a=>a.MAST_UNIT_NAME).FirstOrDefault(),
                         //dbContext.MASTER_ARRR_LMM_TYPES.Where(m=>m.MAST_LMM_TYPE_CODE == arrrDetails.MAST_LMM_TYPE_CODE).Select(m=>m.MAST_LMM_USAGE_UNIT).FirstOrDefault().ToString(),
                       //"<center><table><tr><td  style='border-color:white'><span class='ui-icon ui-icon-plus' title='Upload File' onClick ='UploadLabourPdfFile(\"" + URLEncrypt.EncryptParameters1(new string[]{"ItemCode="+arrrDetails.MAST_LMM_CODE.ToString()}) + "\");' ></span></td></tr></table></center>",
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
                if (dbContext != null)
                {
                    dbContext.Dispose();
                }
            }
        }

        public bool addBulkARRRMaterialRatesDAL(IEnumerable<LabourDetails> materialData, ref string message)
        {
            int recordCount = 0;
            try
            {
                string StateName = PMGSYSession.Current.StateName;
                int StateCode = PMGSYSession.Current.StateCode;
                dbContext = new PMGSYEntities();
                string stateShortCode = dbContext.MASTER_STATE.Where(s => s.MAST_STATE_CODE == PMGSYSession.Current.StateCode).Select(s => s.MAST_STATE_SHORT_CODE).FirstOrDefault();
                foreach (var obj in materialData)
                {
                    LMMRateViewModel model = new LMMRateViewModel();
                    MASTER_ARRR_STATES_RATES arrr = new MASTER_ARRR_STATES_RATES();
                    int YearCode = obj.Year;

                    model.Item = dbContext.MASTER_ARRR_LMM_TYPES.Where(m => m.MAST_LMM_DESC == obj.Labour && m.MAST_LMM_CODE == obj.Mast_Lmm_Code).Select(m => m.MAST_LMM_TYPE_CODE).FirstOrDefault();
                    recordCount = dbContext.MASTER_ARRR_STATES_RATES.Where(m => m.MAST_LMM_TYPE_CODE == model.Item && m.MAST_STATE_CODE == StateCode && m.MAST_ARRR_YEAR == YearCode).Count();
                    if (recordCount > 0)
                    {
                        message = "Material Rate details already exists for " + StateName + ", for Year " + YearCode + " and for selected type " + obj.Labour;
                        return false;
                    }

                    //arrr.MAST_ARRR_RATE_CODE = dbContext.MASTER_ARRR_STATES_RATES.Max(cp => (Int32?)cp.MAST_ARRR_RATE_CODE) == null ? 1 : (Int32)dbContext.MASTER_ARRR_STATES_RATES.Max(cp => (Int32?)cp.MAST_ARRR_RATE_CODE) + 1;
                    arrr.MAST_ARRR_RATE_CODE = dbContext.MASTER_ARRR_STATES_RATES.Any() ? dbContext.MASTER_ARRR_STATES_RATES.Max(s => s.MAST_ARRR_RATE_CODE) + 1 : 1;
                    arrr.MAST_STATE_CODE = PMGSYSession.Current.StateCode;
                    arrr.ACTIVE_FLAG = "Y";                                                 //obj.flag;
                    arrr.MAST_ARRR_YEAR = obj.Year;
                    arrr.MAST_ARRR_RATE = Convert.ToDecimal(obj.Rate);
                    arrr.MAST_LMM_TYPE = 3;                                             //obj.rateType; 1 is for Labour
                    arrr.MAST_LMM_TYPE_CODE = model.Item;              //obj.Item
                    arrr.MAST_ARRR_RATE_IS_FINAL = "N";                     //obj.status;
                    arrr.USERID = PMGSYSession.Current.UserId;
                    arrr.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                    arrr.MAST_ARRR_FILENAME = String.IsNullOrEmpty(obj.File) ? null : ConfigurationManager.AppSettings["MaterialRateFilePath"] + stateShortCode.Trim() + "\\" + obj.File;
                    arrr.MAST_ARRR_FILE_UPLOAD_DATE = String.IsNullOrEmpty(obj.File) ? (DateTime?)null : Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss"));
                    dbContext.MASTER_ARRR_STATES_RATES.Add(arrr);
                    dbContext.SaveChanges();
                }

                return true;
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                if (ex.InnerException.InnerException.Message == "The INSERT statement conflicted with the FOREIGN KEY constraint \"FK_MASTER_ARRR_STATES_RATES_MASTER_ARRR_LMM_TYPES\". The conflict occurred in database \"OMMAS_DEV\", table \"omms.MASTER_ARRR_LMM_TYPES\", column 'MAST_LMM_CODE'.\r\nThe statement has been terminated.")
                {
                    message = "No details present for LMM Type";
                }
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

        public bool FinalizeAllMaterialRatesDAL(int year)
        {
            bool flg = false;
            try
            {
                dbContext = new PMGSY.Models.PMGSYEntities();

                var records = dbContext.MASTER_ARRR_STATES_RATES.Where(s => s.MAST_ARRR_YEAR == year && s.MAST_STATE_CODE == PMGSYSession.Current.StateCode && s.MAST_LMM_TYPE == 3).ToList();
                if (records.Count > 0)
                {
                    foreach (var a in records)
                    {
                        MASTER_ARRR_STATES_RATES arrr = new MASTER_ARRR_STATES_RATES();

                        arrr = dbContext.MASTER_ARRR_STATES_RATES.Where(m => m.MAST_ARRR_RATE_CODE == a.MAST_ARRR_RATE_CODE).FirstOrDefault();
                        arrr.MAST_ARRR_RATE_IS_FINAL = "Y";
                        dbContext.Entry(arrr).State = System.Data.Entity.EntityState.Modified;
                        dbContext.SaveChanges();
                    }
                    flg = true;
                }
                else
                {
                    flg = false;
                }

                return flg;
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

        public bool SaveARRRMaterialFile(string FileName, HttpPostedFileBase filebase, out bool isFileSaved)
        {
            isFileSaved = false;
            dbContext = new PMGSYEntities();
            int recordCount = 0;
            try
            {
                //LMMRateViewModel model = new LMMRateViewModel();
                //model.Item = dbContext.MASTER_ARRR_LMM_TYPES.Where(m => m.MAST_LMM_DESC == labour).Select(m => m.MAST_LMM_TYPE_CODE).FirstOrDefault();
                //recordCount = dbContext.MASTER_ARRR_STATES_RATES.Where(m => m.MAST_LMM_TYPE_CODE == model.Item && m.MAST_STATE_CODE == PMGSYSession.Current.StateCode && m.MAST_ARRR_YEAR == Year).Count();
                //if (recordCount > 0)
                //{
                //    isFileSaved = false;
                //    return false;
                //}
                string stateShortCode = dbContext.MASTER_STATE.Where(s => s.MAST_STATE_CODE == PMGSYSession.Current.StateCode).Select(s => s.MAST_STATE_SHORT_CODE).FirstOrDefault();

                if (!string.IsNullOrEmpty(FileName.Trim()))
                {
                    if (!System.IO.Directory.Exists(System.IO.Path.Combine(@"" + ConfigurationManager.AppSettings["MaterialRateFilePath"] + stateShortCode)))
                        System.IO.Directory.CreateDirectory(System.IO.Path.Combine(@"" + ConfigurationManager.AppSettings["MaterialRateFilePath"] + stateShortCode));

                    filebase.SaveAs(ConfigurationManager.AppSettings["MaterialRateFilePath"] + stateShortCode.Trim() + "\\" + FileName);
                    isFileSaved = true;
                }

                return true;
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "REATDAL.SaveARRRMaterialFile()");
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

        #region Labour Rates Master

        public Array ListLabourRatesDAL(int page, int rows, string sidx, string sord, int Year, out long totalRecords)
        {
            try
            {
                dbContext = new PMGSYEntities();
                if (Year == 0)
                {
                    Year = dbContext.MASTER_ARRR_STATES_RATES.Max(s => s.MAST_ARRR_YEAR); //To render Year-Wise Listing of existing records
                }
                var list = from arrrlist in dbContext.MASTER_ARRR_STATES_RATES
                           join lmmtype in dbContext.MASTER_ARRR_LMM_TYPES
                           on arrrlist.MAST_LMM_TYPE_CODE equals lmmtype.MAST_LMM_TYPE_CODE
                           where arrrlist.MAST_LMM_TYPE == 1 && arrrlist.MAST_STATE_CODE == PMGSYSession.Current.StateCode && arrrlist.MAST_ARRR_YEAR == Year
                           select new
                           {
                               arrrlist.MAST_ARRR_RATE_CODE,
                               arrrlist.MAST_STATE_CODE,
                               arrrlist.MAST_LMM_TYPE_CODE,
                               arrrlist.MAST_LMM_TYPE,
                               arrrlist.MAST_ARRR_RATE,
                               arrrlist.MAST_ARRR_YEAR,
                               arrrlist.MAST_ARRR_RATE_IS_FINAL,
                               arrrlist.ACTIVE_FLAG,
                               arrrlist.MAST_ARRR_FILENAME,
                               lmmtype.MAST_LMM_OUTPUT_UNIT,

                           };

                totalRecords = list.Count();

                if (sidx.Trim() != string.Empty)
                {
                    if (sord.ToString() == "asc")
                    {
                        switch (sidx)
                        {
                            case "MAST_ARRR_RATE_CODE":
                                list = list.OrderBy(x => x.MAST_ARRR_RATE_CODE).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                                break;
                            case "MAST_STATE_CODE":
                                list = list.OrderBy(x => x.MAST_STATE_CODE).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                                break;
                            case "MAST_LMM_TYPE":
                                list = list.OrderBy(x => x.MAST_LMM_TYPE).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                                break;
                            default:
                                list = list.OrderBy(x => x.MAST_ARRR_RATE_CODE).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                                break;
                        }
                    }
                    else
                    {
                        switch (sidx)
                        {
                            case "MAST_ARRR_RATE_CODE":
                                list = list.OrderByDescending(x => x.MAST_ARRR_RATE_CODE).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                                break;
                            case "MAST_STATE_CODE":
                                list = list.OrderByDescending(x => x.MAST_STATE_CODE).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                                break;
                            case "MAST_LMM_TYPE":
                                list = list.OrderByDescending(x => x.MAST_LMM_TYPE).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                                break;
                            default:
                                list = list.OrderByDescending(x => x.MAST_ARRR_RATE_CODE).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                                break;
                        }
                    }
                }
                else
                {
                    list = list.OrderBy(x => x.MAST_ARRR_RATE_CODE).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                }

                var result = list.Select(arrrlist => new
                {
                    arrrlist.MAST_ARRR_RATE_CODE,
                    arrrlist.MAST_STATE_CODE,
                    arrrlist.MAST_LMM_TYPE_CODE,
                    arrrlist.MAST_LMM_TYPE,
                    arrrlist.MAST_ARRR_RATE,
                    arrrlist.MAST_ARRR_YEAR,
                    //arrrlist.MAST_ARRR_EFFECTIVE_FROM,
                    //arrrlist.MAST_ARRR_EFFECTIVE_TILL,
                    arrrlist.MAST_ARRR_RATE_IS_FINAL,
                    arrrlist.ACTIVE_FLAG,
                    arrrlist.MAST_ARRR_FILENAME,
                    arrrlist.MAST_LMM_OUTPUT_UNIT,
                }).ToArray();


                return result.Select(arrrDetails => new
                {
                    id = arrrDetails.MAST_ARRR_RATE_CODE.ToString().Trim(),

                    cell = new[]{
                    
     /*3*/          //dbContext.MASTER_STATE.Where(m=>m.MAST_STATE_CODE == arrrDetails.MAST_STATE_CODE).Select(m=>m.MAST_STATE_NAME).FirstOrDefault(),
                    dbContext.MASTER_ARRR_STATES_RATES.Where(m => m.MAST_ARRR_RATE_CODE == arrrDetails.MAST_ARRR_RATE_CODE).Select(m => m.MASTER_ARRR_LMM_TYPES.MASTER_ARRR_LMM_CATEGORY.MAST_LMM_CATEGORY).FirstOrDefault(),
                    dbContext.MASTER_ARRR_LMM_TYPES.Where(m=>m.MAST_LMM_TYPE_CODE == arrrDetails.MAST_LMM_TYPE_CODE).Select(m=>m.MAST_LMM_DESC).FirstOrDefault(),
                    dbContext.MASTER_UNITS.Where(m => m.MAST_UNIT_CODE == arrrDetails.MAST_LMM_OUTPUT_UNIT).Select(a=>a.MAST_UNIT_NAME).FirstOrDefault(),
     /*5*/      arrrDetails.MAST_ARRR_RATE == null ? "" : arrrDetails.MAST_ARRR_RATE.ToString(),
                    arrrDetails.MAST_ARRR_YEAR.ToString() + " - " + (arrrDetails.MAST_ARRR_YEAR+1).ToString(),
                    //arrrDetails.MAST_ARRR_EFFECTIVE_FROM == null ? string.Empty : arrrDetails.MAST_ARRR_EFFECTIVE_FROM.ToString("dd/MM/yyyy"),
                    //arrrDetails.MAST_ARRR_EFFECTIVE_TILL == null ? "-" : Convert.ToDateTime(arrrDetails.MAST_ARRR_EFFECTIVE_TILL).ToString("dd/MM/yyyy"),
                    arrrDetails.MAST_ARRR_RATE_IS_FINAL == "N" ? "<a href='#'  onClick='FinalizeLabourRate(\""+ PMGSY.Common.URLEncrypt.EncryptParameters1(new string[] { "rateCode ="+ arrrDetails.MAST_ARRR_RATE_CODE.ToString().Trim()}) +"\"); return false;'>Finalize</a>" : "Finalized",
                    arrrDetails.MAST_ARRR_RATE_IS_FINAL == "Y" ?
                    ("<center><table><tr><td style='border:none'><a href='#' title='Locked' class='ui-icon ui-icon-locked  ui-align-center'>Locked</a></td></tr></table></center>")
                    : (arrrDetails.ACTIVE_FLAG == "Y" ? "<center><a href='#' onclick='changeLabourRatestatus(\"" +  URLEncrypt.EncryptParameters1(new string[]{ "rateCode=" + arrrDetails.MAST_ARRR_RATE_CODE.ToString().Trim() }) + "\"); return false;'>Yes</a></center>"
                    : "<center><a href='#' onclick='changeLabourRatestatus(\"" +  URLEncrypt.EncryptParameters1(new string[]{ "rateCode=" + arrrDetails.MAST_ARRR_RATE_CODE.ToString().Trim() }) + "\"); return false;'>No</a></center>"),

                    arrrDetails.MAST_ARRR_RATE_IS_FINAL == "N" ?
                    "<center><a href='#' class='ui-icon ui-icon-pencil' onclick='loadEditLabourRateDetails(\"" +  URLEncrypt.EncryptParameters1(new string[]{ "rateCode=" + arrrDetails.MAST_ARRR_RATE_CODE.ToString().Trim() }) + "\"); return false;'>Edit</a></center>"
                    :("<center><table><tr><td style='border:none'><a href='#' title='Locked' class='ui-icon ui-icon-locked  ui-align-center'>Locked</a></td></tr></table></center>"),

                    arrrDetails.MAST_ARRR_RATE_IS_FINAL == "N" ?
                    "<center><a href='#' class='ui-icon ui-icon-trash' onclick='deleteLabourRate(\"" +  URLEncrypt.EncryptParameters1(new string[]{ "rateCode=" + arrrDetails.MAST_ARRR_RATE_CODE.ToString().Trim() }) + "\"); return false;'>Delete</a></center>"        
                    :("<center><table><tr><td style='border:none'><a href='#' title='Locked' class='ui-icon ui-icon-locked  ui-align-center'>Locked</a></td></tr></table></center>"),
                    
                    URLEncrypt.EncryptParameters(new string[] { arrrDetails.MAST_ARRR_FILENAME + "$" +  arrrDetails.MAST_ARRR_RATE_CODE }),
                   
                    //arrrDetails.MAST_ARRR_RATE_IS_FINAL == "N" ?
                    //"<center><a href='#' class='ui-icon ui-icon-zoomin' onclick='viewUploadedFile(\"" +  URLEncrypt.EncryptParameters1(new string[]{ "rateCode=" + arrrDetails.MAST_ARRR_RATE_CODE.ToString().Trim() }) + "\"); return false;'>View File</a></center>"
                    //:("<center><table><tr><td style='border:none'><a href='#' title='Locked' class='ui-icon ui-icon-locked  ui-align-center'>Locked</a></td></tr></table></center>"),

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
                if (dbContext != null)
                {
                    dbContext.Dispose();
                }
            }
        }


        public bool addARRRLabourRatesDAL(PMGSY.Models.ARRR.LMMRateViewModel model, ref string message)
        {
            int recordCount = 0;
            //string message = "";
            try
            {
                PMGSY.Models.MASTER_ARRR_STATES_RATES arrr = new MASTER_ARRR_STATES_RATES();

                dbContext = new PMGSYEntities();

                string StateName = PMGSYSession.Current.StateName;
                int StateCode = PMGSYSession.Current.StateCode;
                int YearCode = model.Year;



                recordCount = dbContext.MASTER_ARRR_STATES_RATES.Where(m => m.MAST_LMM_TYPE_CODE == model.Item && m.MAST_STATE_CODE == StateCode && m.MAST_ARRR_YEAR == YearCode).Count();
                if (recordCount > 0)
                {
                    message = "Labour Rate details already exists for " + StateName + ", for Year " + YearCode + " and for selected type.";
                    return false;
                }


                arrr.MAST_ARRR_RATE_CODE = dbContext.MASTER_ARRR_STATES_RATES.Max(cp => (Int32?)cp.MAST_ARRR_RATE_CODE) == null ? 1 : (Int32)dbContext.MASTER_ARRR_STATES_RATES.Max(cp => (Int32?)cp.MAST_ARRR_RATE_CODE) + 1;
                arrr.MAST_STATE_CODE = PMGSYSession.Current.StateCode; //model.stateCode;

                arrr.ACTIVE_FLAG = model.flag;

                arrr.MAST_ARRR_YEAR = model.Year;
                arrr.MAST_ARRR_RATE = model.Rate;
                arrr.MAST_LMM_TYPE = model.rateType;
                arrr.MAST_LMM_TYPE_CODE = model.Item;//dbContext.MASTER_ARRR_STATES_RATES.Max(cp => (Int32?)cp.MAST_LMM_TYPE_CODE) == null ? 1 : (Int32)dbContext.MASTER_ARRR_STATES_RATES.Max(cp => (Int32?)cp.MAST_LMM_TYPE_CODE) + 1;
                arrr.MAST_ARRR_RATE_IS_FINAL = model.status;

                arrr.USERID = PMGSYSession.Current.UserId;
                arrr.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];

                dbContext.MASTER_ARRR_STATES_RATES.Add(arrr);
                dbContext.SaveChanges();

                return true;
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                if (ex.InnerException.InnerException.Message == "The INSERT statement conflicted with the FOREIGN KEY constraint \"FK_MASTER_ARRR_STATES_RATES_MASTER_ARRR_LMM_TYPES\". The conflict occurred in database \"OMMAS_DEV\", table \"omms.MASTER_ARRR_LMM_TYPES\", column 'MAST_LMM_CODE'.\r\nThe statement has been terminated.")
                {
                    message = "No details present for LMM Type";
                }
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

        public bool LabourRateFinalizeDAL(int rateCode)
        {
            bool flg = false;
            try
            {
                dbContext = new PMGSY.Models.PMGSYEntities();

                PMGSY.Models.MASTER_ARRR_STATES_RATES arrr = new PMGSY.Models.MASTER_ARRR_STATES_RATES();

                arrr = dbContext.MASTER_ARRR_STATES_RATES.Where(s => s.MAST_ARRR_RATE_CODE == rateCode).FirstOrDefault();
                PMGSY.Common.CommonFunctions comm = new PMGSY.Common.CommonFunctions();

                arrr.MAST_ARRR_RATE_IS_FINAL = "Y";

                dbContext.Entry(arrr).State = System.Data.Entity.EntityState.Modified;
                dbContext.SaveChanges();
                flg = true;

                return flg;
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

        public bool editARRRLabourRatesDAL(PMGSY.Models.ARRR.LMMRateViewModel model, ref string message)
        {
            dbContext = new PMGSYEntities();
            int recordCount = 0;
            try
            {
                PMGSY.Models.MASTER_ARRR_STATES_RATES arrr = new MASTER_ARRR_STATES_RATES();

                dbContext = new PMGSYEntities();

                recordCount = dbContext.MASTER_ARRR_STATES_RATES.Where(m => m.MAST_ARRR_RATE_CODE == model.rateCode && m.MAST_LMM_TYPE_CODE == model.hdnItem).Count();
                if (!(recordCount == 1))
                {
                    message = "Labour Rate details already exists for selected type.";
                    return false;
                }

                arrr = dbContext.MASTER_ARRR_STATES_RATES.Where(m => m.MAST_ARRR_RATE_CODE == model.rateCode).FirstOrDefault();
                //arrr.MAST_STATE_CODE = model.stateCode;

                //arrr.ACTIVE_FLAG = model.flag;

                arrr.MAST_ARRR_RATE = model.Rate;
                arrr.MAST_ARRR_YEAR = model.Year;
                //arrr.MAST_ARRR_RATE_IS_FINAL = model.status;
                //arrr.MAST_LMM_TYPE_CODE = model.Item;
                arrr.USERID = PMGSYSession.Current.UserId;
                arrr.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];

                dbContext.Entry(arrr).State = System.Data.Entity.EntityState.Modified;
                dbContext.SaveChanges();

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

        public bool deleteARRRLabourRatesDAL(int rateCode)
        {
            dbContext = new PMGSYEntities();
            try
            {
                PMGSY.Models.MASTER_ARRR_STATES_RATES arrr = dbContext.MASTER_ARRR_STATES_RATES.Where(m => m.MAST_ARRR_RATE_CODE == rateCode).FirstOrDefault();

                dbContext.MASTER_ARRR_STATES_RATES.Remove(arrr);
                dbContext.SaveChanges();
                //message = "Clearance details deleted successfully.";
                return true;
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                return false;
            }
        }


        public Array ListLabourFormDAL(int page, int rows, string sidx, string sord, out long totalRecords)  //Added by Aditi on 21 Aug 2020
        {
            try
            {
                dbContext = new PMGSYEntities();

                var list = from arrrlist in dbContext.MASTER_ARRR_LMM_TYPES
                           join cat in dbContext.MASTER_ARRR_LMM_CATEGORY
                                     on arrrlist.MAST_LMM_CAT_CODE equals cat.MAST_LMM_CAT_CODE
                           where arrrlist.MAST_LMM_TYPE == 1 /*&& arrrlist.MAST_STATE_CODE == PMGSYSession.Current.StateCode*/
                           select new
                           {
                               arrrlist.MAST_LMM_TYPE_CODE,
                               arrrlist.MAST_LMM_TYPE,
                               arrrlist.MAST_LMM_ACTY_CODE,
                               arrrlist.MAST_LMM_DESC,
                               arrrlist.MAST_LMM_CAT_CODE,
                               arrrlist.MAST_LMM_CODE,
                               arrrlist.MAST_LMM_OUTPUT_UNIT,
                           };

                totalRecords = list.Count();

                if (sidx.Trim() != string.Empty)
                {
                    if (sord.ToString() == "asc")
                    {
                        switch (sidx)
                        {
                            case "MAST_LMM_TYPE_CODE":
                                list = list.OrderBy(x => x.MAST_LMM_TYPE_CODE).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                                break;
                            //case "MAST_STATE_CODE":
                            //    list = list.OrderBy(x => x.MAST_STATE_CODE).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                            //    break;
                            case "MAST_LMM_TYPE":
                                list = list.OrderBy(x => x.MAST_LMM_TYPE).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                                break;
                            default:
                                list = list.OrderBy(x => x.MAST_LMM_TYPE_CODE).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                                break;
                        }
                    }
                    else
                    {
                        switch (sidx)
                        {
                            case "MAST_LMM_TYPE_CODE":
                                list = list.OrderByDescending(x => x.MAST_LMM_TYPE_CODE).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                                break;
                            //case "MAST_STATE_CODE":
                            //    list = list.OrderByDescending(x => x.MAST_STATE_CODE).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                            //    break;
                            case "MAST_LMM_TYPE":
                                list = list.OrderByDescending(x => x.MAST_LMM_TYPE).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                                break;
                            default:
                                list = list.OrderByDescending(x => x.MAST_LMM_TYPE_CODE).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                                break;
                        }
                    }
                }
                else
                {
                    list = list.OrderBy(x => x.MAST_LMM_TYPE_CODE).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                }

                var result = list.Select(arrrlist => new
                {
                    arrrlist.MAST_LMM_TYPE_CODE,
                    arrrlist.MAST_LMM_TYPE,
                    arrrlist.MAST_LMM_ACTY_CODE,
                    arrrlist.MAST_LMM_DESC,
                    arrrlist.MAST_LMM_CAT_CODE,
                    arrrlist.MAST_LMM_CODE,
                    arrrlist.MAST_LMM_OUTPUT_UNIT

                }).ToArray();

                return result.Select(arrrDetails => new
                {
                    id = arrrDetails.MAST_LMM_TYPE_CODE.ToString().Trim(),

                    cell = new[]{
                         dbContext.MASTER_ARRR_LMM_TYPES.Where(m=>m.MAST_LMM_TYPE_CODE == arrrDetails.MAST_LMM_TYPE_CODE).Select(m=>m.MAST_LMM_CODE).FirstOrDefault(),
                         dbContext.MASTER_ARRR_LMM_CATEGORY.Where(m => m.MAST_LMM_CAT_CODE == arrrDetails.MAST_LMM_CAT_CODE).Select(m => m.MAST_LMM_CATEGORY).FirstOrDefault(),
                         dbContext.MASTER_ARRR_LMM_TYPES.Where(m=>m.MAST_LMM_TYPE_CODE == arrrDetails.MAST_LMM_TYPE_CODE).Select(m=>m.MAST_LMM_DESC).FirstOrDefault(),
                         dbContext.MASTER_UNITS.Where(m => m.MAST_UNIT_CODE == arrrDetails.MAST_LMM_OUTPUT_UNIT).Select(a=>a.MAST_UNIT_NAME).FirstOrDefault(),
                         //dbContext.MASTER_ARRR_LMM_TYPES.Where(m=>m.MAST_LMM_TYPE_CODE == arrrDetails.MAST_LMM_TYPE_CODE).Select(m=>m.MAST_LMM_USAGE_UNIT).FirstOrDefault().ToString(),
                       //"<center><table><tr><td  style='border-color:white'><span class='ui-icon ui-icon-plus' title='Upload File' onClick ='UploadLabourPdfFile(\"" + URLEncrypt.EncryptParameters1(new string[]{"ItemCode="+arrrDetails.MAST_LMM_CODE.ToString()}) + "\");' ></span></td></tr></table></center>",
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
                if (dbContext != null)
                {
                    dbContext.Dispose();
                }
            }
        }

        public bool addBulkARRRLabourRatesDAL(IEnumerable<LabourDetails> labourData, ref string message) //Added by Aditi on 27 Aug 2020
        {
            int recordCount = 0;
            try
            {
                string StateName = PMGSYSession.Current.StateName;
                int StateCode = PMGSYSession.Current.StateCode;
                dbContext = new PMGSYEntities();
                string stateShortCode = dbContext.MASTER_STATE.Where(s => s.MAST_STATE_CODE == PMGSYSession.Current.StateCode).Select(s => s.MAST_STATE_SHORT_CODE).FirstOrDefault();
                foreach (var obj in labourData)
                {
                    LMMRateViewModel model = new LMMRateViewModel();
                    MASTER_ARRR_STATES_RATES arrr = new MASTER_ARRR_STATES_RATES();
                    int YearCode = obj.Year;

                    model.Item = dbContext.MASTER_ARRR_LMM_TYPES.Where(m => m.MAST_LMM_DESC == obj.Labour && m.MAST_LMM_CODE == obj.Mast_Lmm_Code).Select(m => m.MAST_LMM_TYPE_CODE).FirstOrDefault();
                    recordCount = dbContext.MASTER_ARRR_STATES_RATES.Where(m => m.MAST_LMM_TYPE_CODE == model.Item && m.MAST_STATE_CODE == StateCode && m.MAST_ARRR_YEAR == YearCode).Count();
                    if (recordCount > 0)
                    {
                        message = "Labour Rate details already exists for " + StateName + ", for Year " + YearCode + " and for selected type " + obj.Labour;
                        return false;
                    }

                    //arrr.MAST_ARRR_RATE_CODE = dbContext.MASTER_ARRR_STATES_RATES.Max(cp => (Int32?)cp.MAST_ARRR_RATE_CODE) == null ? 1 : (Int32)dbContext.MASTER_ARRR_STATES_RATES.Max(cp => (Int32?)cp.MAST_ARRR_RATE_CODE) + 1;
                    arrr.MAST_ARRR_RATE_CODE = dbContext.MASTER_ARRR_STATES_RATES.Any() ? dbContext.MASTER_ARRR_STATES_RATES.Max(s => s.MAST_ARRR_RATE_CODE) + 1 : 1;
                    arrr.MAST_STATE_CODE = PMGSYSession.Current.StateCode;
                    arrr.ACTIVE_FLAG = "Y";                                                 //obj.flag;
                    arrr.MAST_ARRR_YEAR = obj.Year;
                    arrr.MAST_ARRR_RATE = Convert.ToDecimal(obj.Rate);
                    arrr.MAST_LMM_TYPE = 1;                                             //obj.rateType; 1 is for Labour
                    arrr.MAST_LMM_TYPE_CODE = model.Item;              //obj.Item
                    arrr.MAST_ARRR_RATE_IS_FINAL = "N";                     //obj.status;
                    arrr.USERID = PMGSYSession.Current.UserId;
                    arrr.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                    arrr.MAST_ARRR_FILENAME = String.IsNullOrEmpty(obj.File) ? null : ConfigurationManager.AppSettings["LabourRateFilePath"] + stateShortCode.Trim() + "\\" + obj.File;
                    arrr.MAST_ARRR_FILE_UPLOAD_DATE = String.IsNullOrEmpty(obj.File) ? (DateTime?)null : Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss"));
                    dbContext.MASTER_ARRR_STATES_RATES.Add(arrr);
                    dbContext.SaveChanges();
                }

                return true;
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                if (ex.InnerException.InnerException.Message == "The INSERT statement conflicted with the FOREIGN KEY constraint \"FK_MASTER_ARRR_STATES_RATES_MASTER_ARRR_LMM_TYPES\". The conflict occurred in database \"OMMAS_DEV\", table \"omms.MASTER_ARRR_LMM_TYPES\", column 'MAST_LMM_CODE'.\r\nThe statement has been terminated.")
                {
                    message = "No details present for LMM Type";
                }
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

        public bool FinalizeAllLabourRatesDAL(int year) //Added by Aditi on 1 Sept 2020
        {
            bool flg = false;
            try
            {
                dbContext = new PMGSY.Models.PMGSYEntities();

                var records = dbContext.MASTER_ARRR_STATES_RATES.Where(s => s.MAST_ARRR_YEAR == year && s.MAST_STATE_CODE == PMGSYSession.Current.StateCode && s.MAST_LMM_TYPE == 1).ToList();
                if (records.Count > 0)
                {
                    foreach (var a in records)
                    {
                        MASTER_ARRR_STATES_RATES arrr = new MASTER_ARRR_STATES_RATES();

                        arrr = dbContext.MASTER_ARRR_STATES_RATES.Where(m => m.MAST_ARRR_RATE_CODE == a.MAST_ARRR_RATE_CODE).FirstOrDefault();
                        arrr.MAST_ARRR_RATE_IS_FINAL = "Y";
                        dbContext.Entry(arrr).State = System.Data.Entity.EntityState.Modified;
                        dbContext.SaveChanges();
                    }
                    flg = true;
                }
                else
                {
                    flg = false;
                }

                return flg;
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

        public bool SaveLabourCommisionFile(string FileName, HttpPostedFileBase filebase, out bool isFileSaved)// Added by Aditi on 1 Sept 2020
        {
            isFileSaved = false;
            dbContext = new PMGSYEntities();
            int recordCount = 0;
            try
            {
                //LMMRateViewModel model = new LMMRateViewModel();
                //model.Item = dbContext.MASTER_ARRR_LMM_TYPES.Where(m => m.MAST_LMM_DESC == labour).Select(m => m.MAST_LMM_TYPE_CODE).FirstOrDefault();
                //recordCount = dbContext.MASTER_ARRR_STATES_RATES.Where(m => m.MAST_LMM_TYPE_CODE == model.Item && m.MAST_STATE_CODE == PMGSYSession.Current.StateCode && m.MAST_ARRR_YEAR == Year).Count();
                //if (recordCount > 0)
                //{
                //    isFileSaved = false;
                //    return false;
                //}
                string stateShortCode = dbContext.MASTER_STATE.Where(s => s.MAST_STATE_CODE == PMGSYSession.Current.StateCode).Select(s => s.MAST_STATE_SHORT_CODE).FirstOrDefault();

                if (!string.IsNullOrEmpty(FileName.Trim()))
                {
                    if (!System.IO.Directory.Exists(System.IO.Path.Combine(@"" + ConfigurationManager.AppSettings["LabourRateFilePath"] + stateShortCode)))
                        System.IO.Directory.CreateDirectory(System.IO.Path.Combine(@"" + ConfigurationManager.AppSettings["LabourRateFilePath"] + stateShortCode));

                    filebase.SaveAs(ConfigurationManager.AppSettings["LabourRateFilePath"] + stateShortCode.Trim() + "\\" + FileName);
                    isFileSaved = true;
                }

                return true;
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "REATDAL.SaveLabourCommisionFile()");
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

        #region Machinery Rates Master

        public Array ListMachineryRatesDAL(int page, int rows, string sidx, string sord, int Year, out long totalRecords)
        {
            try
            {
                dbContext = new PMGSYEntities();
                if (Year == 0)
                {
                    Year = dbContext.MASTER_ARRR_STATES_RATES.Max(s => s.MAST_ARRR_YEAR); //To render Year-Wise Listing of existing records
                }
                var list = from arrrlist in dbContext.MASTER_ARRR_STATES_RATES
                           join lmmtype in dbContext.MASTER_ARRR_LMM_TYPES
                           on arrrlist.MAST_LMM_TYPE_CODE equals lmmtype.MAST_LMM_TYPE_CODE
                           where arrrlist.MAST_LMM_TYPE == 2 && arrrlist.MAST_STATE_CODE == PMGSYSession.Current.StateCode && arrrlist.MAST_ARRR_YEAR == Year
                           select new
                           {
                               arrrlist.MAST_ARRR_RATE_CODE,
                               arrrlist.MAST_STATE_CODE,
                               arrrlist.MAST_LMM_TYPE_CODE,
                               arrrlist.MAST_LMM_TYPE,
                               arrrlist.MAST_ARRR_RATE,
                               arrrlist.MAST_ARRR_YEAR,
                               arrrlist.MAST_ARRR_RATE_IS_FINAL,
                               arrrlist.ACTIVE_FLAG,
                               arrrlist.MAST_ARRR_FILENAME,
                               lmmtype.MAST_LMM_OUTPUT_UNIT,
                               lmmtype.MAST_LMM_OUTPUT_RATE,
                               lmmtype.MAST_LMM_USAGE_UNIT,
                           };

                totalRecords = list.Count();

                if (sidx.Trim() != string.Empty)
                {
                    if (sord.ToString() == "asc")
                    {
                        switch (sidx)
                        {
                            case "MAST_ARRR_RATE_CODE":
                                list = list.OrderBy(x => x.MAST_ARRR_RATE_CODE).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                                break;
                            case "MAST_STATE_CODE":
                                list = list.OrderBy(x => x.MAST_STATE_CODE).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                                break;
                            case "MAST_LMM_TYPE":
                                list = list.OrderBy(x => x.MAST_LMM_TYPE).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                                break;
                            default:
                                list = list.OrderBy(x => x.MAST_ARRR_RATE_CODE).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                                break;
                        }
                    }
                    else
                    {
                        switch (sidx)
                        {
                            case "MAST_ARRR_RATE_CODE":
                                list = list.OrderByDescending(x => x.MAST_ARRR_RATE_CODE).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                                break;
                            case "MAST_STATE_CODE":
                                list = list.OrderByDescending(x => x.MAST_STATE_CODE).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                                break;
                            case "MAST_LMM_TYPE":
                                list = list.OrderByDescending(x => x.MAST_LMM_TYPE).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                                break;
                            default:
                                list = list.OrderByDescending(x => x.MAST_ARRR_RATE_CODE).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                                break;
                        }
                    }
                }
                else
                {
                    list = list.OrderBy(x => x.MAST_ARRR_RATE_CODE).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                }

                var result = list.Select(arrrlist => new
                {
                    arrrlist.MAST_ARRR_RATE_CODE,
                    arrrlist.MAST_STATE_CODE,
                    arrrlist.MAST_LMM_TYPE_CODE,
                    arrrlist.MAST_LMM_TYPE,
                    arrrlist.MAST_ARRR_RATE,
                    arrrlist.MAST_ARRR_YEAR,
                    arrrlist.MAST_ARRR_RATE_IS_FINAL,
                    arrrlist.ACTIVE_FLAG,
                    arrrlist.MAST_ARRR_FILENAME,
                    arrrlist.MAST_LMM_OUTPUT_UNIT,
                    arrrlist.MAST_LMM_OUTPUT_RATE,
                    arrrlist.MAST_LMM_USAGE_UNIT,
                }).ToArray();

                return result.Select(arrrDetails => new
                {
                    id = arrrDetails.MAST_ARRR_RATE_CODE.ToString().Trim(),

                    cell = new[]{
                    
     /*3*/          //dbContext.MASTER_STATE.Where(m=>m.MAST_STATE_CODE == arrrDetails.MAST_STATE_CODE).Select(m=>m.MAST_STATE_NAME).FirstOrDefault(),
                    dbContext.MASTER_ARRR_STATES_RATES.Where(m => m.MAST_ARRR_RATE_CODE == arrrDetails.MAST_ARRR_RATE_CODE).Select(m => m.MASTER_ARRR_LMM_TYPES.MASTER_ARRR_LMM_CATEGORY.MAST_LMM_CATEGORY).FirstOrDefault(),
                    dbContext.MASTER_ARRR_LMM_TYPES.Where(m=>m.MAST_LMM_TYPE_CODE == arrrDetails.MAST_LMM_TYPE_CODE).Select(m=>m.MAST_LMM_DESC).FirstOrDefault(),
                                             dbContext.MASTER_UNITS.Where(m => m.MAST_UNIT_CODE == arrrDetails.MAST_LMM_OUTPUT_UNIT).Select(m=>m.MAST_UNIT_NAME).FirstOrDefault(),
                        dbContext.MASTER_ARRR_LMM_TYPES.Where(m=>m.MAST_LMM_TYPE_CODE == arrrDetails.MAST_LMM_TYPE_CODE).Select(m=>m.MAST_LMM_OUTPUT_RATE).FirstOrDefault().ToString(),                       
                        dbContext.MASTER_UNITS.Where(m => m.MAST_UNIT_CODE == arrrDetails.MAST_LMM_USAGE_UNIT).Select(m=>m.MAST_UNIT_NAME).FirstOrDefault(),
                    //dbContext.MASTER_ARRR_LMM_TYPES.Where(m=>m.MAST_LMM_TYPE_CODE == arrrDetails.MAST_LMM_TYPE_CODE).Select(m=>m.MASTER_UNITS.MAST_UNIT_NAME).FirstOrDefault().ToString(),
     /*5*/          arrrDetails.MAST_ARRR_RATE == null ? "" : arrrDetails.MAST_ARRR_RATE.ToString(),
                    arrrDetails.MAST_ARRR_YEAR.ToString() + " - " + (arrrDetails.MAST_ARRR_YEAR+1).ToString(),
                    arrrDetails.MAST_ARRR_RATE_IS_FINAL == "N" ? "<a href='#'  onClick='FinalizeMachineryRate(\""+ PMGSY.Common.URLEncrypt.EncryptParameters1(new string[] { "rateCode ="+ arrrDetails.MAST_ARRR_RATE_CODE.ToString().Trim()}) +"\"); return false;'>Finalize</a>" : "Finalized",
                    arrrDetails.MAST_ARRR_RATE_IS_FINAL == "Y" ?
                    ("<center><table><tr><td style='border:none'><a href='#' title='Locked' class='ui-icon ui-icon-locked  ui-align-center'>Locked</a></td></tr></table></center>")
                    : (arrrDetails.ACTIVE_FLAG == "Y" ? "<center><a href='#' onclick='changeMachineryRatestatus(\"" +  URLEncrypt.EncryptParameters1(new string[]{ "rateCode=" + arrrDetails.MAST_ARRR_RATE_CODE.ToString().Trim() }) + "\"); return false;'>Yes</a></center>"
                    : "<center><a href='#' onclick='changeMachineryRatestatus(\"" +  URLEncrypt.EncryptParameters1(new string[]{ "rateCode=" + arrrDetails.MAST_ARRR_RATE_CODE.ToString().Trim() }) + "\"); return false;'>No</a></center>"),
                    arrrDetails.MAST_ARRR_RATE_IS_FINAL == "N" ?
                    "<center><a href='#' class='ui-icon ui-icon-pencil' onclick='loadEditMachineryRateDetails(\"" +  URLEncrypt.EncryptParameters1(new string[]{ "rateCode=" + arrrDetails.MAST_ARRR_RATE_CODE.ToString().Trim() }) + "\"); return false;'>Edit</a></center>"
                    :("<center><table><tr><td style='border:none'><a href='#' title='Locked' class='ui-icon ui-icon-locked  ui-align-center'>Locked</a></td></tr></table></center>"),

                    arrrDetails.MAST_ARRR_RATE_IS_FINAL == "N" ?
                    "<center><a href='#' class='ui-icon ui-icon-trash' onclick='deleteMachineryRate(\"" +  URLEncrypt.EncryptParameters1(new string[]{ "rateCode=" + arrrDetails.MAST_ARRR_RATE_CODE.ToString().Trim() }) + "\"); return false;'>Delete</a></center>"        
                    :("<center><table><tr><td style='border:none'><a href='#' title='Locked' class='ui-icon ui-icon-locked  ui-align-center'>Locked</a></td></tr></table></center>"),
                
                     URLEncrypt.EncryptParameters(new string[] { arrrDetails.MAST_ARRR_FILENAME + "$" +  arrrDetails.MAST_ARRR_RATE_CODE }),
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
                if (dbContext != null)
                {
                    dbContext.Dispose();
                }
            }
        }

        public bool addARRRMachineryRatesDAL(PMGSY.Models.ARRR.LMMRateViewModel model, ref string message)
        {
            int recordCount = 0;
            //string message = "";
            try
            {
                PMGSY.Models.MASTER_ARRR_STATES_RATES arrr = new MASTER_ARRR_STATES_RATES();

                dbContext = new PMGSYEntities();

                string StateName = PMGSYSession.Current.StateName;
                int StateCode = PMGSYSession.Current.StateCode;
                int YearCode = model.Year;

                recordCount = dbContext.MASTER_ARRR_STATES_RATES.Where(m => m.MAST_LMM_TYPE_CODE == model.Item && m.MAST_STATE_CODE == StateCode && m.MAST_ARRR_YEAR == YearCode).Count();
                if (recordCount > 0)
                {
                    message = "Machinery Rate details already exists for " + StateName + ", for Year " + YearCode + " and for selected type.";
                    return false;
                }



                arrr.MAST_ARRR_RATE_CODE = dbContext.MASTER_ARRR_STATES_RATES.Max(cp => (Int32?)cp.MAST_ARRR_RATE_CODE) == null ? 1 : (Int32)dbContext.MASTER_ARRR_STATES_RATES.Max(cp => (Int32?)cp.MAST_ARRR_RATE_CODE) + 1;
                arrr.MAST_STATE_CODE = PMGSYSession.Current.StateCode;//model.stateCode;

                arrr.ACTIVE_FLAG = model.flag;

                arrr.MAST_ARRR_YEAR = model.Year;
                arrr.MAST_ARRR_RATE = model.Rate;
                arrr.MAST_LMM_TYPE = model.rateType;
                arrr.MAST_LMM_TYPE_CODE = model.Item;//dbContext.MASTER_ARRR_STATES_RATES.Max(cp => (Int32?)cp.MAST_LMM_TYPE_CODE) == null ? 1 : (Int32)dbContext.MASTER_ARRR_STATES_RATES.Max(cp => (Int32?)cp.MAST_LMM_TYPE_CODE) + 1;
                arrr.MAST_ARRR_RATE_IS_FINAL = model.status;

                arrr.USERID = PMGSYSession.Current.UserId;
                arrr.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];

                dbContext.MASTER_ARRR_STATES_RATES.Add(arrr);
                dbContext.SaveChanges();

                return true;
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                if (ex.InnerException.InnerException.Message == "The INSERT statement conflicted with the FOREIGN KEY constraint \"FK_MASTER_ARRR_STATES_RATES_MASTER_ARRR_LMM_TYPES\". The conflict occurred in database \"OMMAS_DEV\", table \"omms.MASTER_ARRR_LMM_TYPES\", column 'MAST_LMM_CODE'.\r\nThe statement has been terminated.")
                {
                    message = "No details present for LMM Type";
                }
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

        public bool MachineryRateFinalizeDAL(int rateCode)
        {
            bool flg = false;
            try
            {
                dbContext = new PMGSY.Models.PMGSYEntities();

                PMGSY.Models.MASTER_ARRR_STATES_RATES arrr = new PMGSY.Models.MASTER_ARRR_STATES_RATES();

                arrr = dbContext.MASTER_ARRR_STATES_RATES.Where(s => s.MAST_ARRR_RATE_CODE == rateCode).FirstOrDefault();
                PMGSY.Common.CommonFunctions comm = new PMGSY.Common.CommonFunctions();

                arrr.MAST_ARRR_RATE_IS_FINAL = "Y";

                dbContext.Entry(arrr).State = System.Data.Entity.EntityState.Modified;
                dbContext.SaveChanges();
                flg = true;

                return flg;
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

        public bool editARRRMachineryRatesDAL(PMGSY.Models.ARRR.LMMRateViewModel model, ref string message)
        {
            dbContext = new PMGSYEntities();
            int recordCount = 0;
            try
            {
                PMGSY.Models.MASTER_ARRR_STATES_RATES arrr = new MASTER_ARRR_STATES_RATES();

                dbContext = new PMGSYEntities();

                recordCount = dbContext.MASTER_ARRR_STATES_RATES.Where(m => m.MAST_ARRR_RATE_CODE == model.rateCode && m.MAST_LMM_TYPE_CODE == model.hdnItem).Count();
                if (!(recordCount == 1))
                {
                    message = "Machinery Rate details already exists for selected type.";
                    return false;
                }

                arrr = dbContext.MASTER_ARRR_STATES_RATES.Where(m => m.MAST_ARRR_RATE_CODE == model.rateCode).FirstOrDefault();
                //arrr.MAST_STATE_CODE = model.stateCode;
                //arrr.ACTIVE_FLAG = model.flag;

                arrr.MAST_ARRR_RATE = model.Rate;
                arrr.MAST_ARRR_YEAR = model.Year;
                //arrr.MAST_ARRR_RATE_IS_FINAL = model.status;
                //arrr.MAST_LMM_TYPE_CODE = model.Item;
                arrr.USERID = PMGSYSession.Current.UserId;
                arrr.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];

                dbContext.Entry(arrr).State = System.Data.Entity.EntityState.Modified;
                dbContext.SaveChanges();

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

        public bool deleteARRRMachineryRatesDAL(int rateCode)
        {
            dbContext = new PMGSYEntities();
            try
            {
                PMGSY.Models.MASTER_ARRR_STATES_RATES arrr = dbContext.MASTER_ARRR_STATES_RATES.Where(m => m.MAST_ARRR_RATE_CODE == rateCode).FirstOrDefault();

                dbContext.MASTER_ARRR_STATES_RATES.Remove(arrr);
                dbContext.SaveChanges();
                //message = "Clearance details deleted successfully.";
                return true;
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                return false;
            }
        }

        public Array ListMachineryFormDAL(int page, int rows, string sidx, string sord, out long totalRecords)
        {
            try
            {
                dbContext = new PMGSYEntities();

                var list = from arrrlist in dbContext.MASTER_ARRR_LMM_TYPES
                           join cat in dbContext.MASTER_ARRR_LMM_CATEGORY
                                     on arrrlist.MAST_LMM_CAT_CODE equals cat.MAST_LMM_CAT_CODE
                           where arrrlist.MAST_LMM_TYPE == 2/*&& arrrlist.MAST_STATE_CODE == PMGSYSession.Current.StateCode*/
                           select new
                           {
                               arrrlist.MAST_LMM_TYPE_CODE,
                               arrrlist.MAST_LMM_TYPE,
                               arrrlist.MAST_LMM_ACTY_CODE,
                               arrrlist.MAST_LMM_DESC,
                               arrrlist.MAST_LMM_CAT_CODE,
                               arrrlist.MAST_LMM_CODE,
                               arrrlist.MAST_LMM_USAGE_UNIT,
                               arrrlist.MAST_LMM_OUTPUT_RATE,
                               arrrlist.MAST_LMM_OUTPUT_UNIT,
                           };

                totalRecords = list.Count();

                if (sidx.Trim() != string.Empty)
                {
                    if (sord.ToString() == "asc")
                    {
                        switch (sidx)
                        {
                            case "MAST_LMM_TYPE_CODE":
                                list = list.OrderBy(x => x.MAST_LMM_TYPE_CODE).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                                break;
                            //case "MAST_STATE_CODE":
                            //    list = list.OrderBy(x => x.MAST_STATE_CODE).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                            //    break;
                            case "MAST_LMM_TYPE":
                                list = list.OrderBy(x => x.MAST_LMM_TYPE).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                                break;
                            default:
                                list = list.OrderBy(x => x.MAST_LMM_TYPE_CODE).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                                break;
                        }
                    }
                    else
                    {
                        switch (sidx)
                        {
                            case "MAST_LMM_TYPE_CODE":
                                list = list.OrderByDescending(x => x.MAST_LMM_TYPE_CODE).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                                break;
                            //case "MAST_STATE_CODE":
                            //    list = list.OrderByDescending(x => x.MAST_STATE_CODE).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                            //    break;
                            case "MAST_LMM_TYPE":
                                list = list.OrderByDescending(x => x.MAST_LMM_TYPE).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                                break;
                            default:
                                list = list.OrderByDescending(x => x.MAST_LMM_TYPE_CODE).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                                break;
                        }
                    }
                }
                else
                {
                    list = list.OrderBy(x => x.MAST_LMM_TYPE_CODE).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                }

                var result = list.Select(arrrlist => new
                {
                    arrrlist.MAST_LMM_TYPE_CODE,
                    arrrlist.MAST_LMM_TYPE,
                    arrrlist.MAST_LMM_ACTY_CODE,
                    arrrlist.MAST_LMM_DESC,
                    arrrlist.MAST_LMM_CAT_CODE,
                    arrrlist.MAST_LMM_CODE,
                    arrrlist.MAST_LMM_USAGE_UNIT,
                    arrrlist.MAST_LMM_OUTPUT_RATE,
                    arrrlist.MAST_LMM_OUTPUT_UNIT

                }).ToArray();

                return result.Select(arrrDetails => new
                {
                    id = arrrDetails.MAST_LMM_TYPE_CODE.ToString().Trim(),

                    cell = new[]{
                         dbContext.MASTER_ARRR_LMM_TYPES.Where(m=>m.MAST_LMM_TYPE_CODE == arrrDetails.MAST_LMM_TYPE_CODE).Select(m=>m.MAST_LMM_CODE).FirstOrDefault(),
                         dbContext.MASTER_ARRR_LMM_CATEGORY.Where(m => m.MAST_LMM_CAT_CODE == arrrDetails.MAST_LMM_CAT_CODE).Select(m => m.MAST_LMM_CATEGORY).FirstOrDefault(),
                         dbContext.MASTER_ARRR_LMM_TYPES.Where(m=>m.MAST_LMM_TYPE_CODE == arrrDetails.MAST_LMM_TYPE_CODE).Select(m=>m.MAST_LMM_DESC).FirstOrDefault(),
                         //dbContext.MASTER_ARRR_LMM_TYPES.Where(m=>m.MAST_LMM_TYPE_CODE == arrrDetails.MAST_LMM_TYPE_CODE).Select(m=>m.MAST_LMM_USAGE_UNIT).FirstOrDefault().ToString(),                                          
                         //dbContext.MASTER_ARRR_LMM_TYPES.Where(m=>m.MAST_LMM_TYPE_CODE == arrrDetails.MAST_LMM_TYPE_CODE).Select(m=>m.MASTER_UNITS.MAST_UNIT_NAME).FirstOrDefault().ToString(),
                         dbContext.MASTER_UNITS.Where(m => m.MAST_UNIT_CODE == arrrDetails.MAST_LMM_OUTPUT_UNIT).Select(m=>m.MAST_UNIT_NAME).FirstOrDefault(),
                        dbContext.MASTER_ARRR_LMM_TYPES.Where(m=>m.MAST_LMM_TYPE_CODE == arrrDetails.MAST_LMM_TYPE_CODE).Select(m=>m.MAST_LMM_OUTPUT_RATE).FirstOrDefault().ToString(),                       
                        dbContext.MASTER_UNITS.Where(m => m.MAST_UNIT_CODE == arrrDetails.MAST_LMM_USAGE_UNIT).Select(m=>m.MAST_UNIT_NAME).FirstOrDefault(),
                        //"<center><table><tr><td  style='border-color:white'><span class='ui-icon ui-icon-plus' title='Upload File' onClick ='UploadLabourPdfFile(\"" + URLEncrypt.EncryptParameters1(new string[]{"ItemCode="+arrrDetails.MAST_LMM_CODE.ToString()}) + "\");' ></span></td></tr></table></center>",
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
                if (dbContext != null)
                {
                    dbContext.Dispose();
                }
            }
        }

        public bool addBulkARRRMachineryRatesDAL(IEnumerable<LabourDetails> machineryData, ref string message)
        {
            int recordCount = 0;
            try
            {
                string StateName = PMGSYSession.Current.StateName;
                int StateCode = PMGSYSession.Current.StateCode;
                dbContext = new PMGSYEntities();
                string stateShortCode = dbContext.MASTER_STATE.Where(s => s.MAST_STATE_CODE == PMGSYSession.Current.StateCode).Select(s => s.MAST_STATE_SHORT_CODE).FirstOrDefault();
                foreach (var obj in machineryData)
                {
                    LMMRateViewModel model = new LMMRateViewModel();
                    MASTER_ARRR_STATES_RATES arrr = new MASTER_ARRR_STATES_RATES();
                    int YearCode = obj.Year;

                    model.Item = dbContext.MASTER_ARRR_LMM_TYPES.Where(m => m.MAST_LMM_DESC == obj.Labour && m.MAST_LMM_CODE == obj.Mast_Lmm_Code).Select(m => m.MAST_LMM_TYPE_CODE).FirstOrDefault();
                    recordCount = dbContext.MASTER_ARRR_STATES_RATES.Where(m => m.MAST_LMM_TYPE_CODE == model.Item && m.MAST_STATE_CODE == StateCode && m.MAST_ARRR_YEAR == YearCode).Count();
                    if (recordCount > 0)
                    {
                        message = "Machinery Rate details already exists for " + StateName + ", for Year " + YearCode + " and for selected type " + obj.Labour;
                        return false;
                    }

                    //arrr.MAST_ARRR_RATE_CODE = dbContext.MASTER_ARRR_STATES_RATES.Max(cp => (Int32?)cp.MAST_ARRR_RATE_CODE) == null ? 1 : (Int32)dbContext.MASTER_ARRR_STATES_RATES.Max(cp => (Int32?)cp.MAST_ARRR_RATE_CODE) + 1;
                    arrr.MAST_ARRR_RATE_CODE = dbContext.MASTER_ARRR_STATES_RATES.Any() ? dbContext.MASTER_ARRR_STATES_RATES.Max(s => s.MAST_ARRR_RATE_CODE) + 1 : 1;
                    arrr.MAST_STATE_CODE = PMGSYSession.Current.StateCode;
                    arrr.ACTIVE_FLAG = "Y";                                                 //obj.flag;
                    arrr.MAST_ARRR_YEAR = obj.Year;
                    arrr.MAST_ARRR_RATE = Convert.ToDecimal(obj.Rate);
                    arrr.MAST_LMM_TYPE = 2;                                             //obj.rateType; 1 is for Labour
                    arrr.MAST_LMM_TYPE_CODE = model.Item;              //obj.Item
                    arrr.MAST_ARRR_RATE_IS_FINAL = "N";                     //obj.status;
                    arrr.USERID = PMGSYSession.Current.UserId;
                    arrr.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                    arrr.MAST_ARRR_FILENAME = String.IsNullOrEmpty(obj.File) ? null : ConfigurationManager.AppSettings["MachineryRateFilePath"] + stateShortCode.Trim() + "\\" + obj.File;
                    arrr.MAST_ARRR_FILE_UPLOAD_DATE = String.IsNullOrEmpty(obj.File) ? (DateTime?)null : Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss"));
                    dbContext.MASTER_ARRR_STATES_RATES.Add(arrr);
                    dbContext.SaveChanges();
                }

                return true;
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                if (ex.InnerException.InnerException.Message == "The INSERT statement conflicted with the FOREIGN KEY constraint \"FK_MASTER_ARRR_STATES_RATES_MASTER_ARRR_LMM_TYPES\". The conflict occurred in database \"OMMAS_DEV\", table \"omms.MASTER_ARRR_LMM_TYPES\", column 'MAST_LMM_CODE'.\r\nThe statement has been terminated.")
                {
                    message = "No details present for LMM Type";
                }
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

        public bool FinalizeAllMachineryRatesDAL(int year)
        {
            bool flg = false;
            try
            {
                dbContext = new PMGSY.Models.PMGSYEntities();

                var records = dbContext.MASTER_ARRR_STATES_RATES.Where(s => s.MAST_ARRR_YEAR == year && s.MAST_STATE_CODE == PMGSYSession.Current.StateCode && s.MAST_LMM_TYPE == 2).ToList();
                if (records.Count > 0)
                {
                    foreach (var a in records)
                    {
                        MASTER_ARRR_STATES_RATES arrr = new MASTER_ARRR_STATES_RATES();

                        arrr = dbContext.MASTER_ARRR_STATES_RATES.Where(m => m.MAST_ARRR_RATE_CODE == a.MAST_ARRR_RATE_CODE).FirstOrDefault();
                        arrr.MAST_ARRR_RATE_IS_FINAL = "Y";
                        dbContext.Entry(arrr).State = System.Data.Entity.EntityState.Modified;
                        dbContext.SaveChanges();
                    }
                    flg = true;
                }
                else
                {
                    flg = false;
                }

                return flg;
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

        public bool SaveARRRMachineryFile(string FileName, HttpPostedFileBase filebase, out bool isFileSaved)
        {
            isFileSaved = false;
            dbContext = new PMGSYEntities();
            int recordCount = 0;
            try
            {
                //LMMRateViewModel model = new LMMRateViewModel();
                //model.Item = dbContext.MASTER_ARRR_LMM_TYPES.Where(m => m.MAST_LMM_DESC == labour).Select(m => m.MAST_LMM_TYPE_CODE).FirstOrDefault();
                //recordCount = dbContext.MASTER_ARRR_STATES_RATES.Where(m => m.MAST_LMM_TYPE_CODE == model.Item && m.MAST_STATE_CODE == PMGSYSession.Current.StateCode && m.MAST_ARRR_YEAR == Year).Count();
                //if (recordCount > 0)
                //{
                //    isFileSaved = false;
                //    return false;
                //}
                string stateShortCode = dbContext.MASTER_STATE.Where(s => s.MAST_STATE_CODE == PMGSYSession.Current.StateCode).Select(s => s.MAST_STATE_SHORT_CODE).FirstOrDefault();

                if (!string.IsNullOrEmpty(FileName.Trim()))
                {
                    if (!System.IO.Directory.Exists(System.IO.Path.Combine(@"" + ConfigurationManager.AppSettings["MachineryRateFilePath"] + stateShortCode)))
                        System.IO.Directory.CreateDirectory(System.IO.Path.Combine(@"" + ConfigurationManager.AppSettings["MachineryRateFilePath"] + stateShortCode));

                    filebase.SaveAs(ConfigurationManager.AppSettings["MachineryRateFilePath"] + stateShortCode.Trim() + "\\" + FileName);
                    isFileSaved = true;
                }

                return true;
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "REATDAL.SaveARRRMachineryFile()");
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

        #region Analysis of Rates
        /*
        public List<SelectListItem> PopulateAnalysisYearsDAL(bool flg)
        {
            PMGSYEntities dbContext = new PMGSYEntities();
            List<SelectListItem> YearList = new List<SelectListItem>();
            SelectListItem item;
            try
            {
                if (flg == true)
                {
                    item = new SelectListItem();
                    item.Text = "All";
                    item.Value = "0";
                    YearList.Add(item);
                }
                else
                {
                    item = new SelectListItem();
                    item.Text = "Select";
                    item.Value = "-1";
                    YearList.Add(item);
                }

                //var s = (from a in dbContext.MASTER_ARRR_ITEMS orderby a.MAST_ARRR_CODE select a.MAST_ARRR_DATE).Distinct().ToList();

                //foreach (var a in s)
                //{
                //    item = new SelectListItem();
                //    item.Text = a.Year + "-" + (a.Year + 1);
                //    item.Value = Convert.ToString(a.Year).Trim();
                //    YearList.Add(item);
                //}

                return YearList;
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

        public Array ListAnalysisRatesDAL(int page, int rows, string sidx, string sord, out long totalRecords, int year)
        {
            try
            {
                dbContext = new PMGSYEntities();

                var list = from arrrlist in dbContext.MASTER_ARRR_ITEMS
                           //where arrrlist.MAST_STATE_CODE == PMGSYSession.Current.StateCode //&& arrrlist.MAST_ARRR_DATE.Year == year
                           select new
                           {
                               arrrlist.MAST_ARRR_CODE,
                               //arrrlist.MAST_STATE_CODE,
                               //arrrlist.MAST_ARRR_RATE_CODE,
                               arrrlist.MAST_ITEM_CODE,
                               //arrrlist.MAST_ARRR_RATE,
                               arrrlist.MAST_ARRR_QTY,
                               //arrrlist.MAST_ARRR_AMOUNT,
                               //arrrlist.MAST_ARRR_DATE,
                               arrrlist.MAST_ARRR_FINALIZED,
                               arrrlist.MAST_ARRR_MORD_APPROVED,
                           };

                totalRecords = list.Count();

                if (sidx.Trim() != string.Empty)
                {
                    if (sord.ToString() == "asc")
                    {
                        switch (sidx)
                        {
                            case "MAST_ARRR_CODE":
                                list = list.OrderBy(x => x.MAST_ARRR_CODE).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                                break;
                            //case "MAST_STATE_CODE":
                            //    list = list.OrderBy(x => x.MAST_STATE_CODE).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                            //    break;
                            case "MAST_ITEM_CODE":
                                list = list.OrderBy(x => x.MAST_ITEM_CODE).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                                break;
                            default:
                                list = list.OrderBy(x => x.MAST_ARRR_CODE).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                                break;
                        }
                    }
                    else
                    {
                        switch (sidx)
                        {
                            case "MAST_ARRR_CODE":
                                list = list.OrderByDescending(x => x.MAST_ARRR_CODE).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                                break;
                            //case "MAST_STATE_CODE":
                            //    list = list.OrderByDescending(x => x.MAST_STATE_CODE).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                            //    break;
                            case "MAST_ITEM_CODE":
                                list = list.OrderByDescending(x => x.MAST_ITEM_CODE).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                                break;
                            default:
                                list = list.OrderByDescending(x => x.MAST_ARRR_CODE).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                                break;
                        }
                    }
                }
                else
                {
                    list = list.OrderBy(x => x.MAST_ARRR_CODE).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                }

                var result = list.Select(arrrlist => new
                {
                    arrrlist.MAST_ARRR_CODE,
                    //arrrlist.MAST_STATE_CODE,
                    //arrrlist.MAST_ARRR_RATE_CODE,
                    arrrlist.MAST_ITEM_CODE,
                    //arrrlist.MAST_ARRR_RATE,
                    arrrlist.MAST_ARRR_QTY,
                    //arrrlist.MAST_ARRR_AMOUNT,
                    //arrrlist.MAST_ARRR_DATE,
                    arrrlist.MAST_ARRR_FINALIZED,
                    arrrlist.MAST_ARRR_MORD_APPROVED,
                }).ToArray();

                return result.Select(arrrDetails => new
                {
                    id = arrrDetails.MAST_ARRR_CODE.ToString().Trim(),

                    cell = new[]{
                    
     ///3         //dbContext.MASTER_STATE.Where(m=>m.MAST_STATE_CODE == arrrDetails.MAST_STATE_CODE).Select(m=>m.MAST_STATE_NAME).FirstOrDefault(),
                    //dbContext.MASTER_ARRR_ITEM_HEAD.Where(m=>m.MAST_HEAD_CODE == arrrDetails.MAST_ITEM_CODE).Select(m=>m.MAST_HEAD_NAME).FirstOrDefault(),
                    //dbContext.MASTER_ARR_ITEMS_MASTER.Where(m=>m.MAST_ITEM_CODE == arrrDetails.MAST_ITEM_CODE).Select(m=>m.MAST_ITEM_NAME).FirstOrDefault(),
                    
                    //dbContext.MASTER_ARRR_ITEMS.Where(m=>m.MAST_ARRR_RATE_CODE == arrrDetails.MAST_ARRR_RATE_CODE).Select(m=>m.MASTER_ARRR_ITEMS_MASTER.MASTER_ARRR_ITEM_HEAD.MAST_HEAD_NAME).FirstOrDefault(),
                    //dbContext.MASTER_ARRR_ITEMS.Where(m=>m.MAST_ARRR_RATE_CODE == arrrDetails.MAST_ARRR_RATE_CODE).Select(m=>m.MASTER_ARRR_ITEMS_MASTER.MAST_ITEM_NAME).FirstOrDefault(),
                    //dbContext.MASTER_ARRR_ITEMS.Where(m=>m.MAST_ARRR_RATE_CODE == arrrDetails.MAST_ARRR_RATE_CODE).Select(m=>m.MASTER_ARRR_STATES_RATES.MASTER_ARRR_LMM_TYPES.MAST_LMM_DESC).FirstOrDefault(),
                    arrrDetails.MAST_ARRR_QTY.ToString(),
     ///5          arrrDetails.MAST_ARRR_RATE.ToString(),
     //               arrrDetails.MAST_ARRR_AMOUNT.ToString(),
     //               arrrDetails.MAST_ARRR_DATE.ToString("dd/MM/yyyy"),

                    "<center><table><tr><td style='border-color:white'><a href='#' style='float:left' id='btnSave"+  arrrDetails.MAST_ARRR_CODE.ToString().Trim() +"' title='Click here to Save the Analysis Rates Details' class='ui-icon ui-icon-disk ui-align-center' onClick=SaveAnalysisRatesDetails('" +  arrrDetails.MAST_ARRR_CODE +"');></a><a href='#' style='float:right' id='btnCancel" +  arrrDetails.MAST_ARRR_CODE +"' title='Click here to Cancel the Analysis Rates Edit' class='ui-icon ui-icon-closethick ui-align-center' onClick= CancelSaveDetails('" +  arrrDetails.MAST_ARRR_CODE +"');></a></td></tr></table></center>",

                    arrrDetails.MAST_ARRR_FINALIZED == "N" ?
                    "<center><a href='#' class='ui-icon ui-icon-pencil' onclick='loadEditAnalysisRates(\"" +  URLEncrypt.EncryptParameters1(new string[]{ "analysisCode=" + arrrDetails.MAST_ARRR_CODE.ToString().Trim() }) + "\"); return false;'>Edit</a></center>"
                    //"<center><a href='#' class='ui-icon ui-icon-pencil' onclick='EditAnalysisRatesDetails(\"" +  arrrDetails.MAST_ARRR_CODE.ToString().Trim() + "\"); return false;'>Edit</a></center>"
                    :("<center><table><tr><td style='border:none'><a href='#' title='Locked' class='ui-icon ui-icon-locked  ui-align-center'>Locked</a></td></tr></table></center>"),

                    arrrDetails.MAST_ARRR_FINALIZED == "N" ?
                    "<center><a href='#' class='ui-icon ui-icon-trash' onclick='deleteAnalysisRates(\"" +  URLEncrypt.EncryptParameters1(new string[]{ "analysisCode=" + arrrDetails.MAST_ARRR_CODE.ToString().Trim() }) + "\"); return false;'>Delete</a></center>"
                    :("<center><table><tr><td style='border:none'><a href='#' title='Locked' class='ui-icon ui-icon-locked  ui-align-center'>Locked</a></td></tr></table></center>"),

                    arrrDetails.MAST_ARRR_MORD_APPROVED == "Y" ? "Yes" : arrrDetails.MAST_ARRR_MORD_APPROVED == "N" ? "<a href='#'  onClick='ApproveAnalysisRates(\""+ PMGSY.Common.URLEncrypt.EncryptParameters1(new string[] { "analysisCode ="+ arrrDetails.MAST_ARRR_CODE.ToString().Trim()}) +"\"); return false;'>No</a>" : "",
                    arrrDetails.MAST_ARRR_FINALIZED == "N" ? "Not Finalized" : "Finalized",//"<a href='#'  onClick='FinalizeAnalysisRates(\""+ PMGSY.Common.URLEncrypt.EncryptParameters1(new string[] { "analysisCode ="+ arrrDetails.MAST_ARRR_CODE.ToString().Trim()}) +"\"); return false;'>Finalize</a>" : "Finalized",
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
                if (dbContext != null)
                {
                    dbContext.Dispose();
                }
            }
        }

        public bool addARRRAnalysisRatesDAL(PMGSY.Models.ARRR.AnalysisRatesViewModel model, ref string message)
        {
            dbContext = new PMGSYEntities();
            //string message = "";
            try
            {
                PMGSY.Models.MASTER_ARRR_ITEMS arrr = new MASTER_ARRR_ITEMS();

                dbContext = new PMGSYEntities();

                arrr.MAST_ARRR_CODE = dbContext.MASTER_ARRR_ITEMS.Max(cp => (Int32?)cp.MAST_ARRR_CODE) == null ? 1 : (Int32)dbContext.MASTER_ARRR_ITEMS.Max(cp => (Int32?)cp.MAST_ARRR_CODE) + 1;
                //arrr.MAST_STATE_CODE = model.stateCode;

                arrr.MAST_ITEM_CODE = model.Item;

                //arrr.MAST_ARRR_RATE_CODE = model.rateCode;//dbContext.MASTER_ARRR_ITEMS.Max(cp => (Int32?)cp.MAST_ARRR_RATE_CODE) == null ? 1 : (Int32)dbContext.MASTER_ARRR_ITEMS.Max(cp => (Int32?)cp.MAST_ARRR_RATE_CODE) + 1;

                arrr.MAST_ARRR_QTY = model.quantity;

                //arrr.MAST_ARRR_RATE = model.Rate;
                //arrr.MAST_ARRR_AMOUNT = model.Amount;
                //arrr.MAST_ARRR_DATE = Convert.ToDateTime(model.Date);

                arrr.MAST_ARRR_FINALIZED = "N";

                arrr.MAST_ARRR_MORD_APPROVED = "N";

                arrr.USERID = PMGSYSession.Current.UserId;

                dbContext.MASTER_ARRR_ITEMS.Add(arrr);
                dbContext.SaveChanges();

                return true;
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                if (ex.InnerException.InnerException.Message == "The INSERT statement conflicted with the FOREIGN KEY constraint \"FK_MASTER_ARRR_STATES_RATES_MASTER_ARRR_LMM_TYPES\". The conflict occurred in database \"OMMAS_DEV\", table \"omms.MASTER_ARRR_LMM_TYPES\", column 'MAST_LMM_CODE'.\r\nThe statement has been terminated.")
                {
                    message = "No details present for LMM Type";
                }
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

        public bool AnalysisRateApproveDAL(int rateCode)
        {
            bool flg = false;
            try
            {
                dbContext = new PMGSY.Models.PMGSYEntities();

                PMGSY.Models.MASTER_ARRR_ITEMS arrr = new PMGSY.Models.MASTER_ARRR_ITEMS();

                arrr = dbContext.MASTER_ARRR_ITEMS.Where(s => s.MAST_ARRR_CODE == rateCode).FirstOrDefault();
                PMGSY.Common.CommonFunctions comm = new PMGSY.Common.CommonFunctions();

                arrr.MAST_ARRR_MORD_APPROVED = "Y";

                dbContext.Entry(arrr).State = System.Data.Entity.EntityState.Modified;
                dbContext.SaveChanges();
                flg = true;

                return flg;
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

        public bool AnalysisRateFinalizeDAL(int year)
        {
            bool flg = false;
            try
            {
                dbContext = new PMGSY.Models.PMGSYEntities();

                PMGSY.Models.MASTER_ARRR_ITEMS arrr = new PMGSY.Models.MASTER_ARRR_ITEMS();

                //arrr = dbContext.MASTER_ARRR_ITEMS.Where(s => s.MAST_ARRR_CODE == rateCode).FirstOrDefault();
                arrr = dbContext.MASTER_ARRR_ITEMS.FirstOrDefault();//.Where(s => s.MAST_ARRR_DATE.Year == year && s.MAST_STATE_CODE == PMGSYSession.Current.StateCode).FirstOrDefault();
                PMGSY.Common.CommonFunctions comm = new PMGSY.Common.CommonFunctions();

                arrr.MAST_ARRR_FINALIZED = "Y";

                dbContext.Entry(arrr).State = System.Data.Entity.EntityState.Modified;
                dbContext.SaveChanges();
                flg = true;

                return flg;
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

        public bool editARRRAnalysisRatesDAL(PMGSY.Models.ARRR.AnalysisRatesViewModel model, ref string message)
        {
            dbContext = new PMGSYEntities();
            try
            {
                PMGSY.Models.MASTER_ARRR_ITEMS arrr = new MASTER_ARRR_ITEMS();

                dbContext = new PMGSYEntities();

                arrr = dbContext.MASTER_ARRR_ITEMS.Where(m => m.MAST_ARRR_CODE == model.analysisCode).FirstOrDefault();
                //arrr.MAST_STATE_CODE = model.stateCode;

                //arrr.MAST_ARRR_MORD_APPROVED = model.Approved;
                //arrr.MAST_ARRR_FINALIZED = model.status;

                arrr.MAST_ARRR_QTY = model.quantity;
                //arrr.MAST_ARRR_AMOUNT = model.Amount;
                //arrr.MAST_ARRR_DATE = Convert.ToDateTime(model.Date);

                ///arrr.MAST_ITEM_CODE = model.Item;
                ///arrr.MAST_ARRR_RATE_CODE = dbContext.MASTER_ARRR_ITEMS.Where(m => m.MAST_ARRR_CODE == model.arrrCode).Select(m => m.MAST_ARRR_RATE_CODE).FirstOrDefault();//model.rateCode;
                ///arrr.MAST_ARRR_RATE = model.Rate;

                arrr.USERID = PMGSYSession.Current.UserId;

                dbContext.Entry(arrr).State = System.Data.Entity.EntityState.Modified;
                dbContext.SaveChanges();

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

        public bool deleteARRRAnalysisRatesDAL(int arrrCode)
        {
            dbContext = new PMGSYEntities();
            try
            {
                PMGSY.Models.MASTER_ARRR_ITEMS arrr = dbContext.MASTER_ARRR_ITEMS.Where(m => m.MAST_ARRR_CODE == arrrCode).FirstOrDefault();

                dbContext.MASTER_ARRR_ITEMS.Remove(arrr);
                dbContext.SaveChanges();
                //message = "Clearance details deleted successfully.";
                return true;
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                return false;
            }
        }

        public bool copyARRRAnalysisRatesDAL(int frmyear, int toyear, ref string message)
        {
            dbContext = new PMGSYEntities();
            string date = "";
            int year = 0;
            try
            {
                PMGSY.Models.ARRR.AnalysisRatesViewModel model = new AnalysisRatesViewModel();
                PMGSY.Models.MASTER_ARRR_ITEMS arrr = new MASTER_ARRR_ITEMS();

                dbContext = new PMGSYEntities();

                //var s = dbContext.MASTER_ARRR_ITEMS.Where(m => m.MAST_ARRR_Year == frmyear).ToList();
                var s = dbContext.MASTER_ARRR_ITEMS.ToList();
                if (s.Count > 0)
                {
                    foreach (var a in s)
                    {
                        PMGSY.Models.MASTER_ARRR_ITEMS arrr1 = new MASTER_ARRR_ITEMS();

                        arrr = dbContext.MASTER_ARRR_ITEMS.Where(m => m.MAST_ARRR_CODE == a.MAST_ARRR_CODE).FirstOrDefault();
                        arrr1.MAST_ARRR_CODE = dbContext.MASTER_ARRR_ITEMS.Max(cp => (Int32?)cp.MAST_ARRR_CODE) == null ? 1 : (Int32)dbContext.MASTER_ARRR_ITEMS.Max(cp => (Int32?)cp.MAST_ARRR_CODE) + 1;
                        ///arrr1.MAST_STATE_CODE = arrr.MAST_STATE_CODE;

                        ///arrr1.MAST_ITEM_CODE = arrr.MAST_ITEM_CODE;

                        ///arrr1.MAST_ARRR_RATE_CODE = arrr.MAST_ARRR_RATE_CODE;//dbContext.MASTER_ARRR_ITEMS.Max(cp => (Int32?)cp.MAST_ARRR_RATE_CODE) == null ? 1 : (Int32)dbContext.MASTER_ARRR_ITEMS.Max(cp => (Int32?)cp.MAST_ARRR_RATE_CODE) + 1;

                        ///arrr1.MAST_ARRR_QTY = arrr.MAST_ARRR_QTY;

                        ///arrr1.MAST_ARRR_RATE = arrr.MAST_ARRR_RATE;

                        ///arrr1.MAST_ARRR_AMOUNT = arrr.MAST_ARRR_AMOUNT;

                        ///arrr1.MAST_ARRR_DATE = arrr.MAST_ARRR_DATE;

                        ///date = arrr.MAST_ARRR_DATE.ToString("dd/MM/yyyy");
                        ///date = date.Substring(0, date.LastIndexOf('/') + 1) + toyear;

                        ///arrr1.MAST_ARRR_DATE = Convert.ToDateTime(date);

                        arrr1.MAST_ARRR_FINALIZED = "N";

                        arrr1.MAST_ARRR_MORD_APPROVED = "N";
                        arrr1.USERID = PMGSYSession.Current.UserId;

                        dbContext.MASTER_ARRR_ITEMS.Add(arrr1);
                        //dbContext.Entry(arrr).State = System.Data.Entity.EntityState.Modified;
                        dbContext.SaveChanges();
                    }
                    message = "Analysis Rates details copied successfully.";
                    return true;
                }
                else
                {
                    message = "Analysis Rates details not copied .";
                    return false;
                }
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                if (ex.InnerException.InnerException.Message == "The INSERT statement conflicted with the FOREIGN KEY constraint \"FK_MASTER_ARRR_STATES_RATES_MASTER_ARRR_LMM_TYPES\". The conflict occurred in database \"OMMAS_DEV\", table \"omms.MASTER_ARRR_LMM_TYPES\", column 'MAST_LMM_CODE'.\r\nThe statement has been terminated.")
                {
                    message = "No details present for LMM Type";
                }
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
        */
        #endregion

        #region Schedule of Rates

        public List<SelectListItem> PopulateTaxListDAL(bool flg)
        {
            PMGSYEntities dbContext = new PMGSYEntities();
            List<SelectListItem> taxList = new List<SelectListItem>();
            SelectListItem item;
            try
            {
                if (flg == true)
                {
                    item = new SelectListItem();
                    item.Text = "All";
                    item.Value = "0";
                    taxList.Add(item);
                }
                else
                {
                    item = new SelectListItem();
                    item.Text = "Select";
                    item.Value = "-1";
                    taxList.Add(item);
                }
                //if (itemCode > 0)
                {
                    var s = (from a in dbContext.MASTER_ARRR_TAXES where a.MAST_ARRR_TAX_ISACTIVE == "Y" orderby a.MAST_ARRR_TAX_CODE select a).ToList();

                    foreach (var a in s)
                    {
                        item = new SelectListItem();
                        item.Text = a.MAST_ARRR_TAX_NAME;
                        item.Value = Convert.ToString(a.MAST_ARRR_TAX_CODE).Trim();
                        taxList.Add(item);
                    }
                }
                return taxList;
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


        public List<SelectListItem> PopulateScheduleMajorItemsList(bool flg, int itemCode)
        {
            PMGSYEntities dbContext = new PMGSYEntities();
            List<SelectListItem> MajorItemList = new List<SelectListItem>();
            SelectListItem item;
            try
            {
                if (flg == true)
                {
                    item = new SelectListItem();
                    item.Text = "All";
                    item.Value = "0";
                    MajorItemList.Add(item);
                }
                else
                {
                    item = new SelectListItem();
                    item.Text = "Select";
                    item.Value = "-1";
                    MajorItemList.Add(item);
                }
                if (itemCode > 0)
                {
                    var s = (from a in dbContext.MASTER_ARRR_ITEMS_MASTER where (a.MAST_MAJOR_SUBITEM_CODE > 0 && a.MAST_MINOR_SUBITEM_CODE == 0 && a.MAST_ITEM == itemCode) orderby a.MAST_MAJOR_SUBITEM_CODE select a).ToList();

                    foreach (var a in s)
                    {
                        item = new SelectListItem();
                        item.Text = a.MAST_ITEM_NAME;
                        item.Value = Convert.ToString(a.MAST_ITEM_CODE).Trim();
                        MajorItemList.Add(item);
                    }
                }
                return MajorItemList;
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

        public List<SelectListItem> PopulateMinorItemsList(bool flg, int itemCode)
        {
            PMGSYEntities dbContext = new PMGSYEntities();
            List<SelectListItem> MajorItemList = new List<SelectListItem>();
            SelectListItem item;
            try
            {
                if (flg == true)
                {
                    item = new SelectListItem();
                    item.Text = "All";
                    item.Value = "0";
                    MajorItemList.Add(item);
                }
                else
                {
                    item = new SelectListItem();
                    item.Text = "Select";
                    item.Value = "-1";
                    MajorItemList.Add(item);
                }
                if (itemCode > 0)
                {
                    var s = (from a in dbContext.MASTER_ARRR_ITEMS_MASTER where (a.MAST_MAJOR_SUBITEM_CODE > 0 && a.MAST_MINOR_SUBITEM_CODE > 0 && a.MAST_ITEM == itemCode) orderby a.MAST_MAJOR_SUBITEM_CODE select a).ToList();

                    foreach (var a in s)
                    {
                        item = new SelectListItem();
                        item.Text = a.MAST_ITEM_NAME;
                        item.Value = Convert.ToString(a.MAST_ITEM_CODE).Trim();
                        MajorItemList.Add(item);
                    }
                }
                return MajorItemList;
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
        public Array ListScheduleChapterItemsDAL(int page, int rows, string sidx, string sord, out long totalRecords, int chapter)
        {
            try
            {
                dbContext = new PMGSYEntities();

                var list = from arrrlist in dbContext.MASTER_ARRR_ITEMS_MASTER
                           where arrrlist.MAST_HEAD_CODE == chapter && arrrlist.MAST_MAJOR_SUBITEM_CODE == 0 && arrrlist.MAST_MINOR_SUBITEM_CODE == 0
                           select new
                           {
                               arrrlist.MAST_ITEM_CODE,
                               arrrlist.MAST_ITEM_NAME,
                           };

                totalRecords = list.Count();

                if (sidx.Trim() != string.Empty)
                {
                    if (sord.ToString() == "asc")
                    {
                        switch (sidx)
                        {
                            case "MAST_ITEM_NAME":
                                list = list.OrderBy(x => x.MAST_ITEM_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                                break;
                            //case "MAST_STATE_CODE":
                            //    list = list.OrderBy(x => x.MAST_STATE_CODE).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                            //    break;
                            default:
                                list = list.OrderBy(x => x.MAST_ITEM_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                                break;
                        }
                    }
                    else
                    {
                        switch (sidx)
                        {
                            case "MAST_ITEM_NAME":
                                list = list.OrderByDescending(x => x.MAST_ITEM_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                                break;
                            //case "MAST_STATE_CODE":
                            //    list = list.OrderByDescending(x => x.MAST_STATE_CODE).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                            //    break;
                            default:
                                list = list.OrderByDescending(x => x.MAST_ITEM_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                                break;
                        }
                    }
                }
                else
                {
                    list = list.OrderBy(x => x.MAST_ITEM_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                }

                var result = list.Select(arrrlist => new
                {
                    arrrlist.MAST_ITEM_CODE,
                    arrrlist.MAST_ITEM_NAME,
                }).ToArray();

                return result.Select(arrrDetails => new
                {
                    id = arrrDetails.MAST_ITEM_CODE.ToString().Trim(),

                    cell = new[]{
                    arrrDetails.MAST_ITEM_NAME,

                    "<center><a href='#' class='ui-icon ui-icon-plusthick' onclick='loadScheduleItems(\"" +  URLEncrypt.EncryptParameters1(new string[]{ "itemCode=" + arrrDetails.MAST_ITEM_CODE.ToString().Trim() }) + "\"); return false;'>Add</a></center>",
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
                if (dbContext != null)
                {
                    dbContext.Dispose();
                }
            }
        }

        public Array ListScheduleLMMDAL(int page, int rows, string sidx, string sord, out long totalRecords, int category, int itemCode)
        {
            try
            {
                dbContext = new PMGSYEntities();

                var list = from arrrlist in dbContext.MASTER_ARRR_ITEMS
                           where //arrrlist.MASTER_ARRR_LMM_TYPES.MAST_LMM_TYPE == lmmType &&
                                 (arrrlist.MASTER_ARRR_ITEMS_MASTER.MAST_ITEM_CODE == itemCode || arrrlist.MASTER_ARRR_ITEMS_MASTER.MAST_ITEM_PARENT == itemCode)
                           //&& category == -1 ? 1 == 1 : arrrlist.MASTER_ARRR_LMM_TYPES.MASTER_ARRR_LMM_CATEGORY.MAST_LMM_TYPE == category
                           select new
                           {
                               arrrlist.MAST_ARRR_CODE,
                               arrrlist.MASTER_ARRR_LMM_TYPES.MAST_LMM_DESC,
                               arrrlist.MASTER_ARRR_ITEMS_MASTER.MAST_ITEM_NAME,
                               arrrlist.MAST_ARRR_QTY,
                               MAST_ARRR_TYPE = arrrlist.MASTER_ARRR_LMM_TYPES.MAST_LMM_TYPE == 1 ? "Labour" : arrrlist.MASTER_ARRR_LMM_TYPES.MAST_LMM_TYPE == 2 ? "Machinery" : "Material"
                           };

                totalRecords = list.Count();

                if (sidx.Trim() != string.Empty)
                {
                    if (sord.ToString() == "asc")
                    {
                        switch (sidx)
                        {
                            //case "MAST_ARRR_CODE":
                            //    list = list.OrderBy(x => x.MAST_ARRR_CODE).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                            //    break;
                            //case "MAST_LMM_DESC":
                            //    list = list.OrderBy(x => x.MAST_LMM_DESC).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                            //    break;
                            default:
                                list = list.OrderBy(x => x.MAST_ITEM_NAME).ThenBy(x => x.MAST_ARRR_TYPE).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                                break;
                        }
                    }
                    else
                    {
                        switch (sidx)
                        {
                            //case "MAST_ARRR_CODE":
                            //    list = list.OrderByDescending(x => x.MAST_ARRR_CODE).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                            //    break;
                            //case "MAST_LMM_DESC":
                            //    list = list.OrderByDescending(x => x.MAST_LMM_DESC).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                            //    break;
                            default:
                                list = list.OrderByDescending(x => x.MAST_ITEM_NAME).ThenByDescending(x => x.MAST_ARRR_TYPE).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                                break;
                        }
                    }
                }
                else
                {
                    //list = list.OrderBy(x => x.MAST_ITEM_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                    list = list.OrderBy(x => x.MAST_ITEM_NAME).ThenBy(x => x.MAST_ARRR_TYPE).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                }

                var result = list.Select(arrrlist => new
                {
                    arrrlist.MAST_ARRR_CODE,
                    arrrlist.MAST_LMM_DESC,
                    arrrlist.MAST_ITEM_NAME,
                    arrrlist.MAST_ARRR_QTY,
                    arrrlist.MAST_ARRR_TYPE
                }).ToArray();

                return result.Select(arrrDetails => new
                {
                    id = arrrDetails.MAST_ARRR_CODE.ToString().Trim(),

                    cell = new[]{
                    arrrDetails.MAST_LMM_DESC,
                    arrrDetails.MAST_ITEM_NAME,
                    arrrDetails.MAST_ARRR_QTY.ToString(),
                    arrrDetails.MAST_ARRR_TYPE,
                    //lmmType == 1 ? "<center><table><tr><td style='border-color:white'><a href='#' style='float:left' id='btnSave"+ arrrDetails.MAST_ARRR_CODE.ToString().Trim() +"' title='Click here to Save the Schedule Details' class='ui-icon ui-icon-disk ui-align-center' onClick=SaveScheduleLabour('" + arrrDetails.MAST_ARRR_CODE.ToString().Trim()+"');></a><a href='#' style='float:right' id='btnCancel" +  arrrDetails.MAST_ARRR_CODE.ToString().Trim() +"' title='Click here to Cancel the Schedule Edit' class='ui-icon ui-icon-closethick ui-align-center' onClick= CancelScheduleLabour('" +  arrrDetails.MAST_ARRR_CODE.ToString().Trim() +"');></a></td></tr></table></center>"
                    //: lmmType == 2 ? "<center><table><tr><td style='border-color:white'><a href='#' style='float:left' id='btnSave"+ arrrDetails.MAST_ARRR_CODE.ToString().Trim() +"' title='Click here to Save the Schedule Details' class='ui-icon ui-icon-disk ui-align-center' onClick=SaveScheduleMachinery('" + arrrDetails.MAST_ARRR_CODE.ToString().Trim()+"');></a><a href='#' style='float:right' id='btnCancel" +  arrrDetails.MAST_ARRR_CODE.ToString().Trim() +"' title='Click here to Cancel the Schedule Edit' class='ui-icon ui-icon-closethick ui-align-center' onClick= CancelScheduleMachinery('" +  arrrDetails.MAST_ARRR_CODE.ToString().Trim() +"');></a></td></tr></table></center>" :  
                    "<center><table><tr><td style='border-color:white'><a href='#' style='float:left' id='btnSave"+ arrrDetails.MAST_ARRR_CODE.ToString().Trim() +"' title='Click here to Save the Schedule Details' class='ui-icon ui-icon-disk ui-align-center' onClick=SaveScheduleLMM('" + arrrDetails.MAST_ARRR_CODE.ToString().Trim()+"');></a><a href='#' style='float:right' id='btnCancel" +  arrrDetails.MAST_ARRR_CODE.ToString().Trim() +"' title='Click here to Cancel the Schedule Edit' class='ui-icon ui-icon-closethick ui-align-center' onClick= CancelScheduleLMM('" +  arrrDetails.MAST_ARRR_CODE.ToString().Trim() +"');></a></td></tr></table></center>",
                    //"<center><a href='#' class='ui-icon ui-icon-plusthick' onclick='loadScheduleItems(\"" +  URLEncrypt.EncryptParameters1(new string[]{ "scheduleCode=" + arrrDetails.MAST_ARRR_CODE.ToString().Trim() }) + "\"); return false;'>Add</a></center>",
                    
                    //lmmType == 1 ? "<center><a href='#' class='ui-icon ui-icon-pencil' onclick='EditScheduleLabour(\"" + arrrDetails.MAST_ARRR_CODE.ToString().Trim() + "\"); return false;'>Edit</a></center>"
                    //: lmmType == 2 ? "<center><a href='#' class='ui-icon ui-icon-pencil' onclick='EditScheduleMachinery(\"" + arrrDetails.MAST_ARRR_CODE.ToString().Trim() + "\"); return false;'>Edit</a></center>" : 
                    "<center><a href='#' class='ui-icon ui-icon-pencil' onclick='EditScheduleLMM(\"" + arrrDetails.MAST_ARRR_CODE.ToString().Trim() + "\"); return false;'>Edit</a></center>",
                    
                    //lmmType == 1 ? "<center><a href='#' class='ui-icon ui-icon-trash ui-align-center' onclick='delScheduleLabour(\"" +  URLEncrypt.EncryptParameters1(new string[]{ "scheduleCode=" + arrrDetails.MAST_ARRR_CODE.ToString().Trim() }) + "\"); return false;'>Delete</a></center>"
                    //: lmmType == 2 ? "<center><a href='#' class='ui-icon ui-icon-trash ui-align-center' onclick='delScheduleMachinery(\"" +  URLEncrypt.EncryptParameters1(new string[]{ "scheduleCode=" + arrrDetails.MAST_ARRR_CODE.ToString().Trim() }) + "\"); return false;'>Delete</a></center>" : 
                    "<center><a href='#' class='ui-icon ui-icon-trash ui-align-center' onclick='delScheduleLMM(\"" +  URLEncrypt.EncryptParameters1(new string[]{ "scheduleCode=" + arrrDetails.MAST_ARRR_CODE.ToString().Trim() }) + "\"); return false;'>Delete</a></center>",
                    //URLEncrypt.EncryptParameters1(new string[]{ "scheduleCode=" + arrrDetails.MAST_ARRR_CODE.ToString().Trim() }),
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
                if (dbContext != null)
                {
                    dbContext.Dispose();
                }
            }
        }

        public bool addScheduleLMMDAL(PMGSY.Models.ARRR.ScheduleRatesViewModel model, ref string message)
        {
            dbContext = new PMGSYEntities();
            //string message = "";
            try
            {
                int dropId = 0;
                int recordCount = 0;

                PMGSY.Models.MASTER_ARRR_ITEMS arrr = new MASTER_ARRR_ITEMS();

                dbContext = new PMGSYEntities();

                recordCount = dbContext.MASTER_ARRR_ITEMS.Where(m => m.MAST_ARRR_CODE == model.scheduleCode).Count();
                if (recordCount > 0)
                {
                    message = "Details already exists for selected type.";
                    return false;
                }

                arrr.MAST_ARRR_CODE = dbContext.MASTER_ARRR_ITEMS.Max(cp => (Int32?)cp.MAST_ARRR_CODE) == null ? 1 : (Int32)dbContext.MASTER_ARRR_ITEMS.Max(cp => (Int32?)cp.MAST_ARRR_CODE) + 1;

                arrr.MAST_ARRR_FINALIZED = "N";
                arrr.MAST_ARRR_MORD_APPROVED = "N";
                arrr.MAST_ARRR_ITEM_DEACTIVE_DATE = null;
                arrr.MAST_ARRR_ITEM_DEACTIVE_FROM = null;
                arrr.MAST_ARRR_QTY = model.quantity;
                arrr.MAST_ITEM_CODE = model.hdlmmType == 3 ? model.hdnItemCodeMaterial : model.hdlmmType == 2 ? model.hdnItemCodeMachinery : model.hdnItemTypeCode;
                arrr.MAST_LMM_TYPE_CODE = model.lmmType;

                arrr.USERID = PMGSYSession.Current.UserId;
                arrr.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];

                dbContext.MASTER_ARRR_ITEMS.Add(arrr);
                dbContext.SaveChanges();

                return true;
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                if (ex.InnerException.InnerException.Message == "The INSERT statement conflicted with the FOREIGN KEY constraint \"FK_MASTER_ARRR_STATES_RATES_MASTER_ARRR_LMM_TYPES\". The conflict occurred in database \"OMMAS_DEV\", table \"omms.MASTER_ARRR_LMM_TYPES\", column 'MAST_LMM_CODE'.\r\nThe statement has been terminated.")
                //ex.InnerException.InnerException.Message = "The INSERT statement conflicted with the FOREIGN KEY constraint \"FK_MASTER_ARRR_STATES_RATES_MASTER_ARRR_LMM_TYPES\". The conflict occurred in database \"OMMAS_DEV\", table \"omms.MASTER_ARRR_LMM_TYPES\", column 'MAST_LMM_CODE'.\r\nThe statement has been te...";
                {
                    message = "No details present for LMM Type";
                }
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

        public bool editScheduleLMMDAL(PMGSY.Models.ARRR.ScheduleRatesViewModel model, ref string message)
        {
            dbContext = new PMGSYEntities();
            //string message = "";
            try
            {
                int dropId = 0;
                int recordCount = 0;

                PMGSY.Models.MASTER_ARRR_ITEMS arrr = new MASTER_ARRR_ITEMS();

                dbContext = new PMGSYEntities();

                recordCount = dbContext.MASTER_ARRR_ITEMS.Where(m => m.MAST_ARRR_CODE == model.scheduleCode/* && m.MAST_LMM_TYPE_CODE == model.lmmType*/).Count();
                if (!(recordCount == 1))
                {
                    message = "Details already exists for selected type.";
                    return false;
                }

                arrr = dbContext.MASTER_ARRR_ITEMS.Where(m => m.MAST_ARRR_CODE == model.scheduleCode).FirstOrDefault();
                //arrr.MAST_STATE_CODE = model.stateCode;

                arrr.MAST_ARRR_ITEM_DEACTIVE_DATE = null;
                arrr.MAST_ARRR_ITEM_DEACTIVE_FROM = null;
                arrr.MAST_ARRR_QTY = model.quantity;
                //arrr.MAST_LMM_TYPE_CODE = model.lmmType;

                arrr.USERID = PMGSYSession.Current.UserId;
                arrr.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];

                dbContext.Entry(arrr).State = System.Data.Entity.EntityState.Modified;
                dbContext.SaveChanges();

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

        public bool ScheduleFinalizeDAL(int chapter)
        {
            bool flg = false;
            try
            {
                dbContext = new PMGSY.Models.PMGSYEntities();

                //arrr = dbContext.MASTER_ARRR_ITEMS.Where(s => s.MAST_ARRR_CODE == rateCode).FirstOrDefault();
                //arrr = dbContext.MASTER_ARRR_ITEMS.FirstOrDefault();//.Where(s => s.MAST_ARRR_DATE.Year == year && s.MAST_STATE_CODE == PMGSYSession.Current.StateCode).FirstOrDefault();
                //PMGSY.Common.CommonFunctions comm = new PMGSY.Common.CommonFunctions();
                //arrr.MAST_ARRR_FINALIZED = "Y";

                //dbContext.Entry(arrr).State = System.Data.Entity.EntityState.Modified;
                //dbContext.SaveChanges();

                var s = dbContext.MASTER_ARRR_ITEMS.Where(a => a.MASTER_ARRR_ITEMS_MASTER.MASTER_ARRR_ITEM_HEAD.MAST_HEAD_CODE == chapter).ToList();
                if (s.Count > 0)
                {
                    foreach (var a in s)
                    {
                        PMGSY.Models.MASTER_ARRR_ITEMS arrr = new MASTER_ARRR_ITEMS();

                        arrr = dbContext.MASTER_ARRR_ITEMS.Where(m => m.MAST_ARRR_CODE == a.MAST_ARRR_CODE).FirstOrDefault();

                        arrr.MAST_ARRR_FINALIZED = "Y";

                        dbContext.Entry(arrr).State = System.Data.Entity.EntityState.Modified;
                        dbContext.SaveChanges();
                    }
                }

                flg = true;

                return flg;
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

        public bool deleteScheduleLMMDAL(int scheduleCode)
        {
            dbContext = new PMGSYEntities();
            try
            {
                PMGSY.Models.MASTER_ARRR_ITEMS arrr = dbContext.MASTER_ARRR_ITEMS.Where(m => m.MAST_ARRR_CODE == scheduleCode).FirstOrDefault();
                //Added by abhishek kamble 27-nov-2013

                dbContext.MASTER_ARRR_ITEMS.Remove(arrr);
                dbContext.SaveChanges();
                //message = "Clearance details deleted successfully.";
                return true;
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                return false;
            }
        }

        public Array ListScheduleTaxDAL(int page, int rows, string sidx, string sord, out long totalRecords, int itemCode)
        {
            try
            {
                dbContext = new PMGSYEntities();

                var list = from arrrlist in dbContext.MASTER_ARRR_ITEM_TAXES
                           where //arrrlist.MASTER_ARRR_LMM_TYPES.MAST_LMM_TYPE == lmmType &&
                                 (arrrlist.MASTER_ARRR_ITEMS_MASTER.MAST_ITEM_CODE == itemCode || arrrlist.MASTER_ARRR_ITEMS_MASTER.MAST_ITEM_PARENT == itemCode)
                           select new
                           {
                               arrrlist.MAST_ARRR_ITEM_TAX_CODE,
                               arrrlist.MASTER_ARRR_TAXES.MAST_ARRR_TAX_NAME,
                               arrrlist.MAST_ARRR_TAX_FLAG,
                               arrrlist.MAST_ARRR_TAX_RATE,
                               arrrlist.MASTER_ARRR_ITEMS_MASTER.MAST_ITEM_NAME
                           };

                totalRecords = list.Count();

                if (sidx.Trim() != string.Empty)
                {
                    if (sord.ToString() == "asc")
                    {
                        switch (sidx)
                        {
                            case "MAST_ARRR_ITEM_TAX_CODE":
                                list = list.OrderBy(x => x.MAST_ARRR_ITEM_TAX_CODE).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                                break;
                            case "MAST_ITEM_NAME":
                                list = list.OrderBy(x => x.MAST_ITEM_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                                break;
                            default:
                                list = list.OrderBy(x => x.MAST_ARRR_ITEM_TAX_CODE).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                                break;
                        }
                    }
                    else
                    {
                        switch (sidx)
                        {
                            case "MAST_ARRR_ITEM_TAX_CODE":
                                list = list.OrderByDescending(x => x.MAST_ARRR_ITEM_TAX_CODE).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                                break;
                            case "MAST_ITEM_NAME":
                                list = list.OrderByDescending(x => x.MAST_ITEM_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                                break;
                            default:
                                list = list.OrderByDescending(x => x.MAST_ARRR_ITEM_TAX_CODE).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                                break;
                        }
                    }
                }
                else
                {
                    list = list.OrderBy(x => x.MAST_ITEM_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                }

                var result = list.Select(arrrlist => new
                {
                    arrrlist.MAST_ARRR_ITEM_TAX_CODE,
                    arrrlist.MAST_ARRR_TAX_NAME,
                    arrrlist.MAST_ARRR_TAX_FLAG,
                    arrrlist.MAST_ARRR_TAX_RATE,
                    arrrlist.MAST_ITEM_NAME
                }).ToArray();

                return result.Select(arrrDetails => new
                {
                    id = arrrDetails.MAST_ARRR_ITEM_TAX_CODE.ToString().Trim(),

                    cell = new[]{
                    arrrDetails.MAST_ARRR_TAX_NAME,
                    arrrDetails.MAST_ITEM_NAME,
                    arrrDetails.MAST_ARRR_TAX_RATE.ToString(),
                    arrrDetails.MAST_ARRR_TAX_FLAG == "P" ? "Percentage" : arrrDetails.MAST_ARRR_TAX_FLAG == "F" ? "Fixed" : "Lumsum",

                    "<center><table><tr><td style='border-color:white'><a href='#' style='float:left' id='btnSave"+ arrrDetails.MAST_ARRR_ITEM_TAX_CODE.ToString().Trim() +"' title='Click here to Save the Schedule Details' class='ui-icon ui-icon-disk ui-align-center' onClick=SaveScheduleTax('" + arrrDetails.MAST_ARRR_ITEM_TAX_CODE.ToString().Trim()+"');></a><a href='#' style='float:right' id='btnCancel" +  arrrDetails.MAST_ARRR_ITEM_TAX_CODE.ToString().Trim() +"' title='Click here to Cancel the Schedule Edit' class='ui-icon ui-icon-closethick ui-align-center' onClick= CancelScheduleTax('" +  arrrDetails.MAST_ARRR_ITEM_TAX_CODE.ToString().Trim() +"');></a></td></tr></table></center>",

                    "<center><a href='#' class='ui-icon ui-icon-pencil' onclick='EditScheduleTax(\"" + arrrDetails.MAST_ARRR_ITEM_TAX_CODE.ToString().Trim() + "\"); return false;'>Edit</a></center>",
                    
                    "<center><a href='#' class='ui-icon ui-icon-trash ui-align-center' onclick='delScheduleTax(\"" +  URLEncrypt.EncryptParameters1(new string[]{ "taxCode=" + arrrDetails.MAST_ARRR_ITEM_TAX_CODE.ToString().Trim() }) + "\"); return false;'>Delete</a></center>",
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
                if (dbContext != null)
                {
                    dbContext.Dispose();
                }
            }
        }

        public bool addScheduleTaxDAL(PMGSY.Models.ARRR.TaxScheduleViewModel model, ref string message)
        {
            dbContext = new PMGSYEntities();
            //string message = "";
            try
            {
                int recordCount = 0;

                PMGSY.Models.MASTER_ARRR_ITEM_TAXES arrr = new MASTER_ARRR_ITEM_TAXES();

                dbContext = new PMGSYEntities();

                recordCount = dbContext.MASTER_ARRR_ITEM_TAXES.Where(m => m.MAST_ARRR_TAX_CODE == model.tax && m.MAST_ITEM_CODE == model.itemCode).Count();
                if (recordCount > 0)
                {
                    message = "Details already exists for selected type.";
                    return false;
                }

                arrr.MAST_ARRR_ITEM_TAX_CODE = dbContext.MASTER_ARRR_ITEM_TAXES.Max(cp => (Int32?)cp.MAST_ARRR_ITEM_TAX_CODE) == null ? 1 : (Int32)dbContext.MASTER_ARRR_ITEM_TAXES.Max(cp => (Int32?)cp.MAST_ARRR_ITEM_TAX_CODE) + 1;

                arrr.MAST_ARRR_TAX_CODE = model.tax;
                arrr.MAST_ARRR_TAX_FLAG = model.taxType;
                arrr.MAST_ARRR_TAX_RATE = model.Rate;
                arrr.MAST_ARRR_TAX_SEQ = model.taxType == "P" ? 1 : model.taxType == "F" ? 0 : 2;
                arrr.MAST_ITEM_CODE = model.itemCode;

                arrr.USERID = PMGSYSession.Current.UserId;
                arrr.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];

                dbContext.MASTER_ARRR_ITEM_TAXES.Add(arrr);
                dbContext.SaveChanges();

                return true;
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                if (ex.InnerException.InnerException.Message == "The INSERT statement conflicted with the FOREIGN KEY constraint \"FK_MASTER_ARRR_STATES_RATES_MASTER_ARRR_LMM_TYPES\". The conflict occurred in database \"OMMAS_DEV\", table \"omms.MASTER_ARRR_LMM_TYPES\", column 'MAST_LMM_CODE'.\r\nThe statement has been terminated.")
                //ex.InnerException.InnerException.Message = "The INSERT statement conflicted with the FOREIGN KEY constraint \"FK_MASTER_ARRR_STATES_RATES_MASTER_ARRR_LMM_TYPES\". The conflict occurred in database \"OMMAS_DEV\", table \"omms.MASTER_ARRR_LMM_TYPES\", column 'MAST_LMM_CODE'.\r\nThe statement has been te...";
                {
                    message = "No details present for LMM Type";
                }
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

        public bool editScheduleTaxDAL(PMGSY.Models.ARRR.TaxScheduleViewModel model, ref string message)
        {
            dbContext = new PMGSYEntities();
            //string message = "";
            try
            {
                int dropId = 0;
                int recordCount = 0;

                PMGSY.Models.MASTER_ARRR_ITEM_TAXES arrr = new MASTER_ARRR_ITEM_TAXES();

                dbContext = new PMGSYEntities();

                //recordCount = dbContext.MASTER_ARRR_ITEM_TAXES.Where(m => m.MAST_ARRR_TAX_CODE == model.taxCode/* && m.MAST_LMM_TYPE_CODE == model.lmmType*/).Count();
                //if (!(recordCount == 1))
                //{
                //    message = "Details already exists for selected type.";
                //    return false;
                //}

                arrr = dbContext.MASTER_ARRR_ITEM_TAXES.Where(m => m.MAST_ARRR_ITEM_TAX_CODE == model.taxCode).FirstOrDefault();
                //arrr.MAST_STATE_CODE = model.stateCode;

                //arrr.MAST_ARRR_TAX_CODE = model.tax;
                //arrr.MAST_ARRR_TAX_FLAG = model.taxType;
                arrr.MAST_ARRR_TAX_RATE = model.Rate;
                //arrr.MAST_ARRR_TAX_SEQ = model.taxType == "P" ? 1 : 2;
                //arrr.MAST_ITEM_CODE = model.itemCode;

                arrr.USERID = PMGSYSession.Current.UserId;
                arrr.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];

                dbContext.Entry(arrr).State = System.Data.Entity.EntityState.Modified;
                dbContext.SaveChanges();

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

        public bool deleteScheduleTaxDAL(int taxCode)
        {
            dbContext = new PMGSYEntities();
            try
            {
                PMGSY.Models.MASTER_ARRR_ITEM_TAXES arrr = dbContext.MASTER_ARRR_ITEM_TAXES.Where(m => m.MAST_ARRR_ITEM_TAX_CODE == taxCode).FirstOrDefault();
                //Added by abhishek kamble 27-nov-2013

                dbContext.MASTER_ARRR_ITEM_TAXES.Remove(arrr);
                dbContext.SaveChanges();
                //message = "Clearance details deleted successfully.";
                return true;
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                return false;
            }
        }
        #endregion

        #region Rohit Code

        #region   Master Chapter

        public bool AddChapterDetailsDAL(Chapter ch, ref string message)
        {
            dbContext = new PMGSYEntities();
            CommonFunctions objCommon = new CommonFunctions();
            try
            {
                MASTER_ARRR_ITEM_HEAD master = new MASTER_ARRR_ITEM_HEAD();
                if (dbContext.MASTER_ARRR_ITEM_HEAD.Any())
                {
                    master.MAST_HEAD_CODE = dbContext.MASTER_ARRR_ITEM_HEAD.Max(m => m.MAST_HEAD_CODE) + 1;
                }
                else
                {
                    master.MAST_HEAD_CODE = 1;
                }

                master.MAST_HEAD_NAME = ch.ChapterName;
                master.MAST_HEAD_ACTIVE = "Y";
                dbContext.MASTER_ARRR_ITEM_HEAD.Add(master);
                dbContext.SaveChanges();
                message = "Chapter Description details saved Successfully.";
                return true;

            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                message = "Error occurred while processing your request.";
                return false;
            }
            finally
            {
                dbContext.Dispose();
            }
        }

        public Array GetChapterDetailsListDAL(int? page, int? rows, string sidx, string sord, out long totalRecords)
        {
            dbContext = new PMGSYEntities();
            try
            {
                var chapterDetailsList = dbContext.MASTER_ARRR_ITEM_HEAD.ToList();

                totalRecords = chapterDetailsList.Count;

                if (sidx.Trim() != string.Empty)
                {
                    if (sord.ToString() == "asc")
                    {
                        chapterDetailsList = chapterDetailsList.OrderBy(x => x.MAST_HEAD_CODE).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                    }
                    else
                    {
                        chapterDetailsList = chapterDetailsList.OrderByDescending(x => x.MAST_HEAD_CODE).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                    }
                }
                else
                {
                    chapterDetailsList = chapterDetailsList.OrderBy(x => x.MAST_HEAD_CODE).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                }



                return chapterDetailsList.Select(chapterDetails => new
                {
                    cell = new[]{
                    
                    chapterDetails.MAST_HEAD_NAME == null?string.Empty:chapterDetails.MAST_HEAD_NAME.ToString(),
                
                    URLEncrypt.EncryptParameters1(new string[]{"ChapterCode =" + chapterDetails.MAST_HEAD_CODE.ToString().Trim()}),
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

        public Chapter GetChapterDetailsDAL(int chapterCode)
        {
            dbContext = new PMGSYEntities();
            CommonFunctions objCommon = new CommonFunctions();
            try
            {
                MASTER_ARRR_ITEM_HEAD chMaster = dbContext.MASTER_ARRR_ITEM_HEAD.Find(chapterCode);
                Chapter chapterModel = new Chapter();
                // taxModel.Effective_Date = objCommon.GetDateTimeToString(taxMaster.MAST_EFFECTIVE_DATE);
                chapterModel.ChapterName = chMaster.MAST_HEAD_NAME;

                chapterModel.EncryptedChapterCode = URLEncrypt.EncryptParameters1(new string[] { "ChapterCode = " + chMaster.MAST_HEAD_CODE });
                chapterModel.Operation = "E";
                return chapterModel;
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                return null;
            }
            finally
            {
                dbContext.Dispose();
            }
        }

        public bool EditChapterDetailsDAL(Chapter chapterModel, ref string message)
        {
            dbContext = new PMGSYEntities();
            CommonFunctions objCommon = new CommonFunctions();
            int chapterCode = 0;
            Dictionary<string, string> decryptedParameters = null;
            String[] urlParameters = null;
            try
            {
                urlParameters = chapterModel.EncryptedChapterCode.Split('/');
                decryptedParameters = URLEncrypt.DecryptParameters1(new string[] { urlParameters[0], urlParameters[1], urlParameters[2] });
                chapterCode = Convert.ToInt32(decryptedParameters["ChapterCode"]);

                MASTER_ARRR_ITEM_HEAD chapterMaster = dbContext.MASTER_ARRR_ITEM_HEAD.Find(chapterCode);
                chapterMaster.MAST_HEAD_NAME = chapterModel.ChapterName;
                chapterMaster.MAST_HEAD_ACTIVE = "Y";
                dbContext.Entry(chapterMaster).State = System.Data.Entity.EntityState.Modified;
                dbContext.SaveChanges();
                message = "Chapter details updated successfully.";
                return true;
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                message = "Error occurred while processing your request.";
                return false;
            }
            finally
            {
                dbContext.Dispose();
            }
        }

        public bool DeleteChapterDetailsDAL(int chapterCode)
        {
            dbContext = new PMGSYEntities();
            try
            {
                MASTER_ARRR_ITEM_HEAD chapterMaster = dbContext.MASTER_ARRR_ITEM_HEAD.Find(chapterCode);
                dbContext.MASTER_ARRR_ITEM_HEAD.Remove(chapterMaster);
                dbContext.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                return false;
            }
            finally
            {
                dbContext.Dispose();
            }
        }

        #endregion

        #region Master Material

        // Save
        public bool AddMaterialDetailsDAL(Material ch, ref string message)
        {
            dbContext = new PMGSYEntities();
            CommonFunctions objCommon = new CommonFunctions();
            try
            {
                MASTER_ARRR_LMM_TYPES master = new MASTER_ARRR_LMM_TYPES();
                if (dbContext.MASTER_ARRR_LMM_TYPES.Any())
                {
                    master.MAST_LMM_TYPE_CODE = dbContext.MASTER_ARRR_LMM_TYPES.Max(m => m.MAST_LMM_TYPE_CODE) + 1;
                }
                else
                {
                    master.MAST_LMM_TYPE_CODE = 1;
                }
                master.MAST_LMM_TYPE = 3;
                if (dbContext.MASTER_ARRR_LMM_TYPES.Where(m => m.MAST_LMM_TYPE == 3).Any())
                {
                    master.MAST_LMM_ACTY_CODE = dbContext.MASTER_ARRR_LMM_TYPES.Where(m => m.MAST_LMM_TYPE == 3).Max(m => m.MAST_LMM_ACTY_CODE) + 1;
                }
                else
                {
                    master.MAST_LMM_ACTY_CODE = 1;
                }
                master.MAST_LMM_USAGE_UNIT = ch.UnitCode;
                master.MAST_LMM_DESC = ch.MaterialName;

                master.MAST_LMM_CAT_CODE = ch.TypeCodes1;
                master.MAST_LMM_CODE = ch.LMM_CODE_NAME;


                //Following fields can not be null;
                master.MAST_LMM_OUTPUT_UNIT = 20;
                master.MAST_LMM_ACTIVE = "Y";

                dbContext.MASTER_ARRR_LMM_TYPES.Add(master);
                dbContext.SaveChanges();
                message = "Material Description details saved Successfully.";
                return true;

            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                message = "Error occurred while processing your request.";
                return false;
            }
            finally
            {
                dbContext.Dispose();
            }
        }

        //Get List
        public Array GetMaterialDetailsListDAL(int? page, int? rows, string sidx, string sord, out long totalRecords)
        {
            dbContext = new PMGSYEntities();
            try
            {
                var chapterDetailsList = dbContext.MASTER_ARRR_LMM_TYPES.Where(m => m.MAST_LMM_TYPE == 3).ToList();

                totalRecords = chapterDetailsList.Count;

                if (sidx.Trim() != string.Empty)
                {
                    if (sord.ToString() == "asc")
                    {
                        chapterDetailsList = chapterDetailsList.OrderBy(x => x.MAST_LMM_DESC).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                    }
                    else
                    {
                        chapterDetailsList = chapterDetailsList.OrderByDescending(x => x.MAST_LMM_DESC).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                    }
                }
                else
                {
                    chapterDetailsList = chapterDetailsList.OrderBy(x => x.MAST_LMM_DESC).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                }

                //var result = chapterDetailsList.Select(chapterDetailsList1 => new
                //{
                //    chapterDetailsList1.MAST_HEAD_NAME,
                //    chapterDetailsList1.MAST_HEAD_CODE,


                //}).ToArray();

                return chapterDetailsList.Select(chapterDetails => new
                {
                    cell = new[]{
                    chapterDetails.MAST_LMM_CAT_CODE==null?string.Empty:chapterDetails.MASTER_ARRR_LMM_CATEGORY.MAST_LMM_CATEGORY,
                    chapterDetails.MAST_LMM_DESC == null?string.Empty:chapterDetails.MAST_LMM_DESC.ToString(),
                    chapterDetails.MAST_LMM_USAGE_UNIT==null?string.Empty:chapterDetails.MASTER_UNITS.MAST_UNIT_NAME,
                    chapterDetails.MAST_LMM_CODE==null?string.Empty:chapterDetails.MAST_LMM_CODE.ToString(),
                    
                    URLEncrypt.EncryptParameters1(new string[]{"MaterialCode =" + chapterDetails.MAST_LMM_TYPE_CODE.ToString().Trim()}),
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

        //Edit
        public Material GetMaterialDetailsDAL(int materialCode)
        {
            dbContext = new PMGSYEntities();
            CommonFunctions objCommon = new CommonFunctions();
            try
            {
                MASTER_ARRR_LMM_TYPES chMaster = dbContext.MASTER_ARRR_LMM_TYPES.Find(materialCode);
                Material chapterModel = new Material();
                // taxModel.Effective_Date = objCommon.GetDateTimeToString(taxMaster.MAST_EFFECTIVE_DATE);
                chapterModel.MaterialName = chMaster.MAST_LMM_DESC;
                chapterModel.UnitCode = chMaster.MAST_LMM_USAGE_UNIT;
                chapterModel.TypeCodes1 = chMaster.MAST_LMM_CAT_CODE;
                chapterModel.LMM_CODE_NAME = chMaster.MAST_LMM_CODE;

                chapterModel.EncryptedMaterialCode = URLEncrypt.EncryptParameters1(new string[] { "MaterialCode = " + chMaster.MAST_LMM_TYPE_CODE });
                chapterModel.Operation = "E";
                return chapterModel;
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                return null;
            }
            finally
            {
                dbContext.Dispose();
            }
        }


        //Update
        public bool EditMaterialDetailsDAL(Material chapterModel, ref string message)
        {
            dbContext = new PMGSYEntities();
            CommonFunctions objCommon = new CommonFunctions();
            int materialCode = 0;
            Dictionary<string, string> decryptedParameters = null;
            String[] urlParameters = null;
            try
            {
                urlParameters = chapterModel.EncryptedMaterialCode.Split('/');
                decryptedParameters = URLEncrypt.DecryptParameters1(new string[] { urlParameters[0], urlParameters[1], urlParameters[2] });
                materialCode = Convert.ToInt32(decryptedParameters["MaterialCode"]);

                MASTER_ARRR_LMM_TYPES chapterMaster = dbContext.MASTER_ARRR_LMM_TYPES.Find(materialCode);


                chapterMaster.MAST_LMM_DESC = chapterModel.MaterialName;
                chapterMaster.MAST_LMM_USAGE_UNIT = chapterModel.UnitCode;

                chapterMaster.MAST_LMM_CAT_CODE = chapterModel.TypeCodes1;

                chapterMaster.MAST_LMM_CODE = chapterModel.LMM_CODE_NAME;

                chapterMaster.MAST_LMM_OUTPUT_UNIT = 20;
                chapterMaster.MAST_LMM_ACTIVE = "Y";
                dbContext.Entry(chapterMaster).State = System.Data.Entity.EntityState.Modified;
                dbContext.SaveChanges();
                message = "Material details updated successfully.";
                return true;
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                message = "Error occurred while processing your request.";
                return false;
            }
            finally
            {
                dbContext.Dispose();
            }
        }


        //Delete
        public bool DeleteMaterialDetailsDAL(int materialCode)
        {
            dbContext = new PMGSYEntities();
            try
            {
                MASTER_ARRR_LMM_TYPES chapterMaster = dbContext.MASTER_ARRR_LMM_TYPES.Find(materialCode);
                dbContext.MASTER_ARRR_LMM_TYPES.Remove(chapterMaster);
                dbContext.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                return false;
            }
            finally
            {
                dbContext.Dispose();
            }
        }


        //Get Units 

        public List<MASTER_UNITS> GetAllUnits()
        {
            try
            {
                dbContext = new PMGSYEntities();
                return dbContext.MASTER_UNITS.ToList<MASTER_UNITS>();
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
        //public List<MASTER_UNITS> GetUnitsForMaterialMaster()
        //{
        //    try
        //    {
        //        dbContext = new PMGSYEntities();
        //        return dbContext.MASTER_UNITS.Where(m => m.MAST_UNIT_DIMENSION==3).ToList<MASTER_UNITS>();
        //    }

        //    catch (Exception ex)
        //    {
        //        Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
        //        return null;
        //    }
        //    finally
        //    {
        //        if (dbContext != null)
        //        {
        //            dbContext.Dispose();
        //        }
        //    }
        //}

        public List<MASTER_ARRR_LMM_CATEGORY> GetAllCategoryTypesMaterial()
        {
            try
            {
                dbContext = new PMGSYEntities();
                return dbContext.MASTER_ARRR_LMM_CATEGORY.Where(m => m.MAST_LMM_TYPE == 3).ToList<MASTER_ARRR_LMM_CATEGORY>();


                // var s = (from a in dbContext.MASTER_ARRR_LMM_CATEGORY where a.MAST_LMM_TYPE == lmmType orderby a.MAST_LMM_CAT_CODE select a).ToList();
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

        #endregion

        #region Labour Master

        //Save
        public bool AddLabourDetailsDAL(Labour ch, ref string message)
        {
            dbContext = new PMGSYEntities();
            CommonFunctions objCommon = new CommonFunctions();
            try
            {
                MASTER_ARRR_LMM_TYPES master = new MASTER_ARRR_LMM_TYPES();
                if (dbContext.MASTER_ARRR_LMM_TYPES.Any())
                {
                    master.MAST_LMM_TYPE_CODE = dbContext.MASTER_ARRR_LMM_TYPES.Max(m => m.MAST_LMM_TYPE_CODE) + 1;
                }
                else
                {
                    master.MAST_LMM_TYPE_CODE = 1;
                }
                master.MAST_LMM_TYPE = 1;
                if (dbContext.MASTER_ARRR_LMM_TYPES.Where(m => m.MAST_LMM_TYPE == 1).Any())
                {
                    master.MAST_LMM_ACTY_CODE = dbContext.MASTER_ARRR_LMM_TYPES.Where(m => m.MAST_LMM_TYPE == 1).Max(m => m.MAST_LMM_ACTY_CODE) + 1;
                }
                else
                {
                    master.MAST_LMM_ACTY_CODE = 1;
                }
                master.MAST_LMM_USAGE_UNIT = ch.UnitCode;
                master.MAST_LMM_DESC = ch.MaterialName;

                master.MAST_LMM_CAT_CODE = ch.TypeCodes;
                master.MAST_LMM_CODE = ch.LMM_CODE_NAME;


                //Following fields can not be null;
                master.MAST_LMM_OUTPUT_UNIT = 20;
                master.MAST_LMM_ACTIVE = "Y";

                dbContext.MASTER_ARRR_LMM_TYPES.Add(master);
                dbContext.SaveChanges();
                message = "Labour Description details saved Successfully.";
                return true;

            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                message = "Error occurred while processing your request.";
                return false;
            }
            finally
            {
                dbContext.Dispose();
            }
        }

        // Get List 
        public Array GetLabourDetailsListDAL(int? page, int? rows, string sidx, string sord, out long totalRecords)
        {
            dbContext = new PMGSYEntities();
            try
            {
                var chapterDetailsList = dbContext.MASTER_ARRR_LMM_TYPES.Where(m => m.MAST_LMM_TYPE == 1).ToList();

                totalRecords = chapterDetailsList.Count;

                if (sidx.Trim() != string.Empty)
                {
                    if (sord.ToString() == "asc")
                    {
                        chapterDetailsList = chapterDetailsList.OrderBy(x => x.MAST_LMM_DESC).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                    }
                    else
                    {
                        chapterDetailsList = chapterDetailsList.OrderByDescending(x => x.MAST_LMM_DESC).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                    }
                }
                else
                {
                    chapterDetailsList = chapterDetailsList.OrderBy(x => x.MAST_LMM_DESC).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                }

                return chapterDetailsList.Select(chapterDetails => new
                {
                    cell = new[]{
                    chapterDetails.MAST_LMM_CAT_CODE==null?string.Empty:chapterDetails.MASTER_ARRR_LMM_CATEGORY.MAST_LMM_CATEGORY,
                    chapterDetails.MAST_LMM_DESC == null?string.Empty:chapterDetails.MAST_LMM_DESC.ToString(),
                    chapterDetails.MAST_LMM_USAGE_UNIT==null?string.Empty:chapterDetails.MASTER_UNITS.MAST_UNIT_NAME,
                    chapterDetails.MAST_LMM_CODE==null?string.Empty:chapterDetails.MAST_LMM_CODE.ToString(),

                    
                    URLEncrypt.EncryptParameters1(new string[]{"MaterialCode =" + chapterDetails.MAST_LMM_TYPE_CODE.ToString().Trim()}),
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

        //Edit
        public Labour GetLabourDetailsDAL(int materialCode)
        {
            dbContext = new PMGSYEntities();
            CommonFunctions objCommon = new CommonFunctions();
            try
            {
                MASTER_ARRR_LMM_TYPES chMaster = dbContext.MASTER_ARRR_LMM_TYPES.Find(materialCode);
                Labour chapterModel = new Labour();
                // taxModel.Effective_Date = objCommon.GetDateTimeToString(taxMaster.MAST_EFFECTIVE_DATE);
                chapterModel.MaterialName = chMaster.MAST_LMM_DESC;
                chapterModel.UnitCode = chMaster.MAST_LMM_USAGE_UNIT;
                chapterModel.TypeCodes = chMaster.MAST_LMM_CAT_CODE;
                chapterModel.LMM_CODE_NAME = chMaster.MAST_LMM_CODE;

                chapterModel.EncryptedMaterialCode = URLEncrypt.EncryptParameters1(new string[] { "MaterialCode = " + chMaster.MAST_LMM_TYPE_CODE });
                chapterModel.Operation = "E";
                return chapterModel;
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                return null;
            }
            finally
            {
                dbContext.Dispose();
            }
        }

        //Update
        public bool EditLabourDetailsDAL(Labour chapterModel, ref string message)
        {
            dbContext = new PMGSYEntities();
            CommonFunctions objCommon = new CommonFunctions();
            int materialCode = 0;
            Dictionary<string, string> decryptedParameters = null;
            String[] urlParameters = null;
            try
            {
                urlParameters = chapterModel.EncryptedMaterialCode.Split('/');
                decryptedParameters = URLEncrypt.DecryptParameters1(new string[] { urlParameters[0], urlParameters[1], urlParameters[2] });
                materialCode = Convert.ToInt32(decryptedParameters["MaterialCode"]);

                MASTER_ARRR_LMM_TYPES chapterMaster = dbContext.MASTER_ARRR_LMM_TYPES.Find(materialCode);


                chapterMaster.MAST_LMM_DESC = chapterModel.MaterialName;
                chapterMaster.MAST_LMM_USAGE_UNIT = chapterModel.UnitCode;
                chapterMaster.MAST_LMM_CAT_CODE = chapterModel.TypeCodes;
                chapterMaster.MAST_LMM_CODE = chapterModel.LMM_CODE_NAME;

                chapterMaster.MAST_LMM_OUTPUT_UNIT = 20;
                chapterMaster.MAST_LMM_ACTIVE = "Y";
                dbContext.Entry(chapterMaster).State = System.Data.Entity.EntityState.Modified;
                dbContext.SaveChanges();
                message = "Labour details updated successfully.";
                return true;
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                message = "Error occurred while processing your request.";
                return false;
            }
            finally
            {
                dbContext.Dispose();
            }
        }


        //Delete
        public bool DeleteLabourDetailsDAL(int materialCode)
        {
            dbContext = new PMGSYEntities();
            try
            {
                MASTER_ARRR_LMM_TYPES chapterMaster = dbContext.MASTER_ARRR_LMM_TYPES.Find(materialCode);
                dbContext.MASTER_ARRR_LMM_TYPES.Remove(chapterMaster);
                dbContext.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                return false;
            }
            finally
            {
                dbContext.Dispose();
            }
        }


        //Get Units 

        public List<MASTER_UNITS> GetAllUnitsLabour()
        {
            try
            {
                dbContext = new PMGSYEntities();
                return dbContext.MASTER_UNITS.ToList<MASTER_UNITS>();
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
        // Get Category Types

        public List<MASTER_ARRR_LMM_CATEGORY> GetAllCategoryTypesLabour()
        {
            try
            {
                dbContext = new PMGSYEntities();

                return dbContext.MASTER_ARRR_LMM_CATEGORY.Where(m => m.MAST_LMM_TYPE == 1).ToList<MASTER_ARRR_LMM_CATEGORY>();
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


        #endregion

        #region Category Master

        //Save
        public bool AddCategoryDetailsDAL(CategoryViewModel ch, ref string message)
        {
            dbContext = new PMGSYEntities();
            CommonFunctions objCommon = new CommonFunctions();
            try
            {
                MASTER_ARRR_LMM_CATEGORY master = new MASTER_ARRR_LMM_CATEGORY();

                if (dbContext.MASTER_ARRR_LMM_TYPES.Any())
                {
                    master.MAST_LMM_CAT_CODE = dbContext.MASTER_ARRR_LMM_CATEGORY.Max(m => m.MAST_LMM_CAT_CODE) + 1;
                }
                else
                {
                    master.MAST_LMM_CAT_CODE = 1;
                }
                master.MAST_LMM_TYPE = ch.TypeCode;
                master.MAST_LMM_CATEGORY = ch.CategoryName;
                dbContext.MASTER_ARRR_LMM_CATEGORY.Add(master);
                dbContext.SaveChanges();
                message = "Category Description details saved Successfully.";
                return true;

            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                message = "Error occurred while processing your request.";
                return false;
            }
            finally
            {
                dbContext.Dispose();
            }
        }

        //List
        public Array GetCategoryDetailsListDAL(int? page, int? rows, string sidx, string sord, out long totalRecords)
        {
            dbContext = new PMGSYEntities();
            try
            {
                var chapterDetailsList = dbContext.MASTER_ARRR_LMM_CATEGORY.ToList();

                totalRecords = chapterDetailsList.Count;

                if (sidx.Trim() != string.Empty)
                {
                    if (sord.ToString() == "asc")
                    {
                        chapterDetailsList = chapterDetailsList.OrderBy(x => x.MAST_LMM_TYPE).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                    }
                    else
                    {
                        chapterDetailsList = chapterDetailsList.OrderByDescending(x => x.MAST_LMM_TYPE).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                    }
                }
                else
                {
                    chapterDetailsList = chapterDetailsList.OrderBy(x => x.MAST_LMM_TYPE).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                }


                return chapterDetailsList.Select(chapterDetails => new
                {
                    cell = new[]{
                    
                    chapterDetails.MAST_LMM_CATEGORY == null?string.Empty:chapterDetails.MAST_LMM_CATEGORY.ToString(),
                    
                    chapterDetails.MAST_LMM_TYPE==1?"Labour":(chapterDetails.MAST_LMM_TYPE==2?"Machinery":(chapterDetails.MAST_LMM_TYPE==3?"Material":"")),

                    URLEncrypt.EncryptParameters1(new string[]{"CategoryCode =" + chapterDetails.MAST_LMM_CAT_CODE.ToString().Trim()}),
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

        //Edit
        public CategoryViewModel GetCategoryDetailsBAL(int categoryCode)
        {
            dbContext = new PMGSYEntities();
            CommonFunctions objCommon = new CommonFunctions();
            try
            {
                MASTER_ARRR_LMM_CATEGORY chMaster = dbContext.MASTER_ARRR_LMM_CATEGORY.Find(categoryCode);
                CategoryViewModel chapterModel = new CategoryViewModel();
                chapterModel.CategoryName = chMaster.MAST_LMM_CATEGORY;
                chapterModel.TypeCode = chMaster.MAST_LMM_TYPE;

                chapterModel.EncryptedCategoryCode = URLEncrypt.EncryptParameters1(new string[] { "CategoryCode = " + chMaster.MAST_LMM_CAT_CODE });
                chapterModel.Operation = "E";
                return chapterModel;
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                return null;
            }
            finally
            {
                dbContext.Dispose();
            }
        }

        //Update
        public bool EditCategoryDetailsDAL(CategoryViewModel chapterModel, ref string message)
        {
            dbContext = new PMGSYEntities();
            CommonFunctions objCommon = new CommonFunctions();
            int categoryCode = 0;
            Dictionary<string, string> decryptedParameters = null;
            String[] urlParameters = null;
            try
            {
                urlParameters = chapterModel.EncryptedCategoryCode.Split('/');
                decryptedParameters = URLEncrypt.DecryptParameters1(new string[] { urlParameters[0], urlParameters[1], urlParameters[2] });
                categoryCode = Convert.ToInt32(decryptedParameters["CategoryCode"]);

                MASTER_ARRR_LMM_CATEGORY chapterMaster = dbContext.MASTER_ARRR_LMM_CATEGORY.Find(categoryCode);


                chapterMaster.MAST_LMM_CATEGORY = chapterModel.CategoryName;
                chapterMaster.MAST_LMM_TYPE = chapterModel.TypeCode;



                dbContext.Entry(chapterMaster).State = System.Data.Entity.EntityState.Modified;
                dbContext.SaveChanges();
                message = "Cateogory details updated successfully.";
                return true;
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                message = "Error occurred while processing your request.";
                return false;
            }
            finally
            {
                dbContext.Dispose();
            }
        }


        //Delate
        public bool DeleteCategoryDetailsDAL(int categoryCode)
        {
            dbContext = new PMGSYEntities();
            try
            {
                MASTER_ARRR_LMM_CATEGORY categoryMaster = dbContext.MASTER_ARRR_LMM_CATEGORY.Find(categoryCode);
                dbContext.MASTER_ARRR_LMM_CATEGORY.Remove(categoryMaster);
                dbContext.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                return false;
            }
            finally
            {
                dbContext.Dispose();
            }
        }

        //Get Types Such as Material, Labour and Machinery
        public List<SelectListItem> GetTypesCode()
        {
            List<SelectListItem> list = new List<SelectListItem>();

            list.Add(new SelectListItem
            {
                Text = "Labour",
                Value = "1"
            });

            list.Add(new SelectListItem
            {
                Text = "Machinery",
                Value = "2"
            });

            list.Add(new SelectListItem
            {
                Text = "Material",
                Value = "3"

            });

            return list;
        }

        #endregion

        #region Tax Master

        public bool AddTaxDetailsDAL(TaxViewModel ch, ref string message)
        {
            dbContext = new PMGSYEntities();
            CommonFunctions objCommon = new CommonFunctions();
            try
            {
                MASTER_ARRR_TAXES master = new MASTER_ARRR_TAXES();
                if (dbContext.MASTER_ARRR_TAXES.Any())
                {
                    master.MAST_ARRR_TAX_CODE = dbContext.MASTER_ARRR_TAXES.Max(m => m.MAST_ARRR_TAX_CODE) + 1;
                }
                else
                {
                    master.MAST_ARRR_TAX_CODE = 1;
                }

                master.MAST_ARRR_TAX_NAME = ch.TaxName;
                master.MAST_ARRR_TAX_ISACTIVE = "Y";
                dbContext.MASTER_ARRR_TAXES.Add(master);

                dbContext.SaveChanges();
                message = "Other Charges Description details saved Successfully.";
                return true;

            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                message = "Error occurred while processing your request.";
                return false;
            }
            finally
            {
                dbContext.Dispose();
            }
        }

        public Array GetTaxDetailsListDAL(int? page, int? rows, string sidx, string sord, out long totalRecords)
        {
            dbContext = new PMGSYEntities();
            try
            {
                var chapterDetailsList = dbContext.MASTER_ARRR_TAXES.ToList();

                totalRecords = chapterDetailsList.Count;

                if (sidx.Trim() != string.Empty)
                {
                    if (sord.ToString() == "asc")
                    {
                        chapterDetailsList = chapterDetailsList.OrderBy(x => x.MAST_ARRR_TAX_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                    }
                    else
                    {
                        chapterDetailsList = chapterDetailsList.OrderByDescending(x => x.MAST_ARRR_TAX_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                    }
                }
                else
                {
                    chapterDetailsList = chapterDetailsList.OrderBy(x => x.MAST_ARRR_TAX_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                }
                return chapterDetailsList.Select(chapterDetails => new
                {
                    cell = new[]{
                    
                    chapterDetails.MAST_ARRR_TAX_NAME == null?string.Empty:chapterDetails.MAST_ARRR_TAX_NAME.ToString(),
                    chapterDetails.MAST_ARRR_TAX_ISACTIVE==null?string.Empty:(chapterDetails.MAST_ARRR_TAX_ISACTIVE=="Y"?"Active":""),
                    URLEncrypt.EncryptParameters1(new string[]{"TaxCode =" + chapterDetails.MAST_ARRR_TAX_CODE.ToString().Trim()}),
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

        public TaxViewModel GetTaxDetailsDAL(int taxCode)
        {
            dbContext = new PMGSYEntities();
            CommonFunctions objCommon = new CommonFunctions();
            try
            {
                MASTER_ARRR_TAXES chMaster = dbContext.MASTER_ARRR_TAXES.Find(taxCode);
                TaxViewModel chapterModel = new TaxViewModel();

                chapterModel.TaxName = chMaster.MAST_ARRR_TAX_NAME;
                chapterModel.TaxIsActiveFlag = chMaster.MAST_ARRR_TAX_ISACTIVE;
                chapterModel.EncryptedTaxCode = URLEncrypt.EncryptParameters1(new string[] { "TaxCode = " + chMaster.MAST_ARRR_TAX_CODE });
                chapterModel.Operation = "E";
                return chapterModel;
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                return null;
            }
            finally
            {
                dbContext.Dispose();
            }
        }

        public bool EditTaxDetailsDAL(TaxViewModel chapterModel, ref string message)
        {
            dbContext = new PMGSYEntities();
            CommonFunctions objCommon = new CommonFunctions();
            int chapterCode = 0;
            Dictionary<string, string> decryptedParameters = null;
            String[] urlParameters = null;
            try
            {
                urlParameters = chapterModel.EncryptedTaxCode.Split('/');
                decryptedParameters = URLEncrypt.DecryptParameters1(new string[] { urlParameters[0], urlParameters[1], urlParameters[2] });
                chapterCode = Convert.ToInt32(decryptedParameters["TaxCode"]);


                MASTER_ARRR_TAXES chapterMaster = dbContext.MASTER_ARRR_TAXES.Find(chapterCode);

                chapterMaster.MAST_ARRR_TAX_NAME = chapterModel.TaxName;
                chapterMaster.MAST_ARRR_TAX_ISACTIVE = "Y";
                dbContext.Entry(chapterMaster).State = System.Data.Entity.EntityState.Modified;
                dbContext.SaveChanges();
                message = "Other Charges details updated successfully.";
                return true;
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                message = "Error occurred while processing your request.";
                return false;
            }
            finally
            {
                dbContext.Dispose();
            }
        }

        public bool DeleteTaxDetailsDAL(int taxCode)
        {
            dbContext = new PMGSYEntities();
            try
            {
                MASTER_ARRR_TAXES chapterMaster = dbContext.MASTER_ARRR_TAXES.Find(taxCode);
                dbContext.MASTER_ARRR_TAXES.Remove(chapterMaster);
                dbContext.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                return false;
            }
            finally
            {
                dbContext.Dispose();
            }
        }

        #endregion

        #endregion
    }

    public interface IARRRDAL
    {
        #region Chapter Items
        List<SelectListItem> PopulateChapterList(bool flg);
        List<SelectListItem> PopulateItemsList(bool flg, int headCode);
        List<SelectListItem> PopulateMajorItemsList(bool flg, int ItemCode);
        List<SelectListItem> PopulateMajorItemsListItemwise(bool flg, int itemCode);

        bool addARRRChaptersDAL(PMGSY.Models.ARRR.ChapterItemsViewModel model, ref string message);
        bool changeChapterstatusDAL(int ItemCode);
        bool editARRRChaptersDAL(PMGSY.Models.ARRR.ChapterItemsViewModel model, ref string message);

        bool deleteARRRChaptersDAL(int ItemCode);

        Array ListChapterItemsDAL(int page, int rows, string sidx, string sord, out long totalRecords);
        #endregion

        #region Machinery Master
        List<SelectListItem> PopulateCategoryDAL(bool flg, int lmmType);
        List<SelectListItem> PopulateUsageUnitDAL();
        List<SelectListItem> PopulateOutputUnitDAL();

        bool addARRRMachineryDAL(PMGSY.Models.ARRR.MachineryMasterViewModel model, ref string message);

        bool changeMachineryMasterstatusDAL(int lmmCode);

        bool editARRRMachineryDAL(PMGSY.Models.ARRR.MachineryMasterViewModel model, ref string message);

        bool deleteARRRMachineryDAL(int ItemCode);

        Array ListMachineryMasterDAL(int page, int rows, string sidx, string sord, out long totalRecords);
        #endregion

        #region Material Rates
        List<SelectListItem> PopulateLMMItemsListDAL(bool flg, int lmmtype, int category);
        bool MaterialRateFinalizeDAL(int rateCode);
        bool addARRRMaterialRatesDAL(PMGSY.Models.ARRR.LMMRateViewModel model, ref string message);
        bool changeLMMRatestatusDAL(int rateCode);
        bool editARRRMaterialRatesDAL(PMGSY.Models.ARRR.LMMRateViewModel model, ref string message);

        bool deleteARRRMaterialRatesDAL(int rateCode);



        Array ListMaterialRatesDAL(int page, int rows, string sidx, string sord, int Year, out long totalRecords);




        Array ListMaterialFormDAL(int page, int rows, string sidx, string sord, out long totalRecords);
        bool addBulkARRRMaterialRatesDAL(IEnumerable<LabourDetails> materialData, ref string message);
        bool FinalizeAllMaterialRatesDAL(int year);
        bool SaveARRRMaterialFile(string FileName, HttpPostedFileBase filebase, out bool isFileSaved); 



        #endregion

        #region Labour Rates
        bool LabourRateFinalizeDAL(int rateCode);
        bool addARRRLabourRatesDAL(PMGSY.Models.ARRR.LMMRateViewModel model, ref string message);
        bool editARRRLabourRatesDAL(PMGSY.Models.ARRR.LMMRateViewModel model, ref string message);

        bool deleteARRRLabourRatesDAL(int rateCode);



        Array ListLabourRatesDAL(int page, int rows, string sidx, string sord, int Year, out long totalRecords);


        Array ListLabourFormDAL(int page, int rows, string sidx, string sord, out long totalRecords);  //Added by Aditi on 21 Aug 2020
        bool addBulkARRRLabourRatesDAL(IEnumerable<LabourDetails> labourData, ref string message); //Added by Aditi on 27 Aug 2020
        bool FinalizeAllLabourRatesDAL(int year);  //Added by Aditi on 1 Sept 2020
        bool SaveLabourCommisionFile(string FileName, HttpPostedFileBase filebase, out bool isFileSaved); //Added by Aditi on 4 Sept 2020
        #endregion

        #region Machinery Rates
        bool MachineryRateFinalizeDAL(int rateCode);
        bool addARRRMachineryRatesDAL(PMGSY.Models.ARRR.LMMRateViewModel model, ref string message);
        bool editARRRMachineryRatesDAL(PMGSY.Models.ARRR.LMMRateViewModel model, ref string message);

        bool deleteARRRMachineryRatesDAL(int rateCode);

        Array ListMachineryRatesDAL(int page, int rows, string sidx, string sord, int Year, out long totalRecords);


        Array ListMachineryFormDAL(int page, int rows, string sidx, string sord, out long totalRecords);
        bool addBulkARRRMachineryRatesDAL(IEnumerable<LabourDetails> machineryData, ref string message);
        bool FinalizeAllMachineryRatesDAL(int year);
        bool SaveARRRMachineryFile(string FileName, HttpPostedFileBase filebase, out bool isFileSaved); 
        
        #endregion

        #region Analysis of Rates
        /*
        List<SelectListItem> PopulateAnalysisYearsDAL(bool flg);
        
        bool AnalysisRateApproveDAL(int rateCode);
        bool AnalysisRateFinalizeDAL(int rateCode);
        bool addARRRAnalysisRatesDAL(PMGSY.Models.ARRR.AnalysisRatesViewModel model, ref string message);
        bool editARRRAnalysisRatesDAL(PMGSY.Models.ARRR.AnalysisRatesViewModel model, ref string message);

        bool deleteARRRAnalysisRatesDAL(int rateCode);

        Array ListAnalysisRatesDAL(int page, int rows, string sidx, string sord, out long totalRecords, int year);
        bool copyARRRAnalysisRatesDAL(int frmyear, int toyear, ref string message);
        */
        #endregion

        #region Schedule of Rates
        List<SelectListItem> PopulateScheduleMajorItemsList(bool flg, int itemCode);
        List<SelectListItem> PopulateMinorItemsList(bool flg, int ItemCode);
        Array ListScheduleChapterItemsDAL(int page, int rows, string sidx, string sord, out long totalRecords, int chapter);
        Array ListScheduleLMMDAL(int page, int rows, string sidx, string sord, out long totalRecords, int lmmType, int itemCode);
        bool addScheduleLMMDAL(PMGSY.Models.ARRR.ScheduleRatesViewModel model, ref string message);
        bool editScheduleLMMDAL(PMGSY.Models.ARRR.ScheduleRatesViewModel model, ref string message);
        bool ScheduleFinalizeDAL(int chapter);
        bool deleteScheduleLMMDAL(int scheduleCode);

        List<SelectListItem> PopulateTaxListDAL(bool flg);
        Array ListScheduleTaxDAL(int page, int rows, string sidx, string sord, out long totalRecords, int itemCode);
        bool addScheduleTaxDAL(PMGSY.Models.ARRR.TaxScheduleViewModel model, ref string message);
        bool editScheduleTaxDAL(PMGSY.Models.ARRR.TaxScheduleViewModel model, ref string message);
        bool deleteScheduleTaxDAL(int taxCode);
        #endregion

        #region Rohit Code

        #region Chapter Master
        //RJ
        bool AddChapterDetailsDAL(Chapter ch, ref string message);
        Array GetChapterDetailsListDAL(int? page, int? rows, string sidx, string sord, out long totalRecords);
        Chapter GetChapterDetailsDAL(int chapterCode);
        bool EditChapterDetailsDAL(Chapter chaptersModel, ref string message);
        bool DeleteChapterDetailsDAL(int chapterCode);

        #endregion

        #region Material Master

        bool AddMaterialDetailsDAL(Material ch, ref string message);
        Array GetMaterialDetailsListDAL(int? page, int? rows, string sidx, string sord, out long totalRecords);
        Material GetMaterialDetailsDAL(int chapterCode);
        bool EditMaterialDetailsDAL(Material chaptersModel, ref string message);
        bool DeleteMaterialDetailsDAL(int materialCode);

        #endregion

        #region Labour Master

        bool AddLabourDetailsDAL(Labour ch, ref string message);
        Array GetLabourDetailsListDAL(int? page, int? rows, string sidx, string sord, out long totalRecords);
        Labour GetLabourDetailsDAL(int chapterCode);
        bool EditLabourDetailsDAL(Labour chaptersModel, ref string message);
        bool DeleteLabourDetailsDAL(int materialCode);

        #endregion

        #region Category Master

        bool AddCategoryDetailsDAL(CategoryViewModel ch, ref string message);
        Array GetCategoryDetailsListDAL(int? page, int? rows, string sidx, string sord, out long totalRecords);
        CategoryViewModel GetCategoryDetailsBAL(int materialCode);
        bool EditCategoryDetailsDAL(CategoryViewModel chaptersModel, ref string message);
        bool DeleteCategoryDetailsDAL(int categoryCode);

        #endregion

        #region Tax Master

        bool AddTaxDetailsDAL(TaxViewModel ch, ref string message);
        Array GetTaxDetailsListDAL(int? page, int? rows, string sidx, string sord, out long totalRecords);
        TaxViewModel GetTaxDetailsDAL(int taxCode);
        bool EditTaxDetailsDAL(TaxViewModel chaptersModel, ref string message);
        bool DeleteTaxDetailsDAL(int taxCode);

        #endregion

        #endregion
    }
}