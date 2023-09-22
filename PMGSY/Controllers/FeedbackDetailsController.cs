using ExifLib;
using PMGSY.Common;
using PMGSY.Extensions;
using PMGSY.Models;
using PMGSY.Models.Feedback;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace PMGSY.Controllers
{
    [RequiredAuthentication]
    [RequiredAuthorization]
    public class FeedbackDetailsController : Controller
    {
        Dictionary<string, string> decryptedParameters = null;

        Feedback.DAL.FeedbackDAL fbDAL = new Feedback.DAL.FeedbackDAL();
        private PMGSY.Models.PMGSYEntities dbContext = new Models.PMGSYEntities();
        Common.CommonFunctions comm = new Common.CommonFunctions();
        int outParam = 0;
        //
        // GET: /Feedback/FeedbackDetails/
        #region DetailsFB
        public ActionResult DetailsFB()
        {
            Common.CommonFunctions com = new Common.CommonFunctions();
            PMGSY.Models.Feedback.FeedbackDetails.DetailsFB Fb = new Models.Feedback.FeedbackDetails.DetailsFB();
            Fb.Years_List = PopulateYear(System.DateTime.Now.Year, true, true);
            Fb.State_List = fbDAL.PopulateStates(true);

            Fb.Months_List = com.PopulateMonths(System.DateTime.Now.Month);

            Fb.Status_List = fillStatus("N");

            Fb.MONTHs = System.DateTime.Now.Month;
            Fb.YEARs = System.DateTime.Now.Year;

            Fb.Category = "0";
            Fb.Approved = PMGSY.Extensions.PMGSYSession.Current.RoleCode == 25 ? "0" : "Y";
            Fb.Status = "0";

            Fb.hdnRoleId = PMGSY.Extensions.PMGSYSession.Current.RoleCode;

            return View("DetailsFB", Fb);
        }

        public ActionResult fillDDLStatus(string approval)
        {
            try
            {
                List<SelectListItem> statusLst = new List<SelectListItem>();
                statusLst = fillStatus(approval);

                return Json(statusLst);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return Json(string.Empty);
            }
        }

        public List<SelectListItem> fillStatus(string appr)
        {
            List<SelectListItem> statusLst = new List<SelectListItem>();
            SelectListItem item;
            if (appr == "N")
            {
                statusLst.Insert(0, new SelectListItem() { Text = "Final Reply", Value = "F" });
                statusLst.Insert(0, new SelectListItem() { Text = "Interim Reply", Value = "I" });
                statusLst.Insert(0, new SelectListItem() { Text = "No Reply", Value = "N" });
                statusLst.Insert(0, new SelectListItem() { Text = "All", Value = "0" });
            }
            else
            {
                statusLst.Insert(0, new SelectListItem() { Text = "Final Reply", Value = "F" });
                statusLst.Insert(0, new SelectListItem() { Text = "Interim Reply", Value = "I" });
                statusLst.Insert(0, new SelectListItem() { Text = "All", Value = "0" });
            }
            return statusLst;
        }

        [HttpPost]
        public ActionResult FBList(FormCollection formCollection)
        {
            Feedback.BAL.IDetailsFBBAL qualityBAL = new Feedback.BAL.DetailsFBBAL();
            int totalRecords;
            try
            {
                using (PMGSY.Common.CommonFunctions commonFunction = new PMGSY.Common.CommonFunctions())
                {
                    if (!commonFunction.ValidateGridParameters(new PMGSY.Common.GridParams(Convert.ToInt32(formCollection["page"]), Convert.ToInt32(formCollection["rows"]), formCollection["sidx"], formCollection["sord"], Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                    {
                        return null;
                    }
                }
                var jsonData = new
                {
                    rows = qualityBAL.FBListBAL(Convert.ToInt32(formCollection["page"]) - 1, Convert.ToInt32(formCollection["rows"]),
                                                            formCollection["sidx"], formCollection["sord"], out totalRecords,
                                                            Convert.ToInt32(formCollection["FOR_MONTH"]), Convert.ToInt32(formCollection["FOR_YEAR"]),
                                                            Convert.ToInt32(formCollection["FOR_STATES"]), Convert.ToString(formCollection["FOR_CATEGORY"]),
                                                            Convert.ToString(formCollection["APPR"]), Convert.ToString(formCollection["STATUS"]), Convert.ToString(formCollection["fbThrough"])),
                    total = totalRecords <= Convert.ToInt32(formCollection["rows"]) ? 1 : (totalRecords % Convert.ToInt32(formCollection["rows"]) == 0 ? totalRecords / Convert.ToInt32(formCollection["rows"]) : (totalRecords / Convert.ToInt32(formCollection["rows"]) + 1)),
                    page = Convert.ToInt32(formCollection["page"]),
                    records = totalRecords
                };
                return Json(jsonData, JsonRequestBehavior.AllowGet);
                //return null;
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return Json(String.Empty);
            }
        }

        public List<SelectListItem> PopulateYear(int SelectedYear, bool populateFirstItem = true, bool isAllYearsSelected = false)
        {
            List<SelectListItem> lstYears = new List<SelectListItem>();
            SelectListItem item = new SelectListItem();
            if (populateFirstItem && isAllYearsSelected == false)
            {
                item.Text = "Select Year";
                item.Value = "0";
                item.Selected = true;
                lstYears.Add(item);
            }
            if (populateFirstItem && isAllYearsSelected)
            {
                item.Text = "All Years";
                item.Value = "-1";
                item.Selected = true;
                lstYears.Add(item);
            }
            for (int i = 2000; i < DateTime.Now.Year + 1; i++)
            {
                item = new SelectListItem();
                item.Text = i.ToString();// +" - " + (i + 1);
                item.Value = i.ToString();
                lstYears.Add(item);
            }
            return lstYears;
        }
        #endregion

        #region Feedback Details
        public ActionResult ViewFBDetails(String parameter, String hash, String key/*string fId */)
        {
            decryptedParameters = PMGSY.Common.URLEncrypt.DecryptParameters1(new String[] { parameter, hash, key });
            PMGSY.Models.Feedback.ViewFBDetails vFB = new Models.Feedback.ViewFBDetails();
            if (decryptedParameters.Count > 0)
            {
                string fId = Convert.ToString(decryptedParameters["FeedCode"]).Trim();
                if (!int.TryParse(fId, out outParam))
                {
                    return Json(false);
                }
                int id = Convert.ToInt32(fId);


                vFB.hdnfeedId = PMGSY.Common.URLEncrypt.EncryptParameters1(new string[] { "FeedCode =" + fId.Trim() });//Convert.ToInt32(fId.Trim());

                var data = (from feedbak in dbContext.ADMIN_FEEDBACK
                            where
                            feedbak.FEED_ID == id
                            select new
                            {
                                feedbak.FEED_PREFIX,
                                feedbak.FEED_NAME,
                                feedbak.FEED_TYPE,
                                feedbak.FEED_CODE,
                                feedbak.FEED_TELE_CODE,
                                feedbak.FEED_TELE_NUMBER,
                                feedbak.FEED_MOBILE,
                                feedbak.FEED_EMAIL,
                                feedbak.FEED_DATE,
                                feedbak.FEED_CATEGORY,
                                feedbak.MASTER_FEEDBACK_CATEGORY.MAST_FEED_NAME,
                                feedbak.FEED_FOR,
                                feedbak.FEED_COMMENT,
                                feedbak.MASTER_STATE.MAST_STATE_NAME,
                                feedbak.MASTER_DISTRICT.MAST_DISTRICT_NAME,
                                feedbak.MASTER_BLOCK.MAST_BLOCK_NAME,
                                feedbak.PLAN_CN_ROAD_CODE,
                                feedbak.MAST_HAB_CODE,
                                feedbak.IS_PMGSY_ROAD,
                                feedbak.ROAD_NAME,
                                feedbak.VILLAGE_NAME,
                                feedbak.NEAREST_HAB,
                            }).ToList();

                //List<PMGSY.Models.ADMIN_FEEDBACK> lst= new List<Models.ADMIN_FEEDBACK>();
                //lst=dbContext.ADMIN_FEEDBACK.Where(M=>M.FEED_ID==id).ToList<Models.ADMIN_FEEDBACK>();
                //lst.Select(S=>S.PLAN_ROAD.PLAN_RD_NAME).FirstOrDefault();


                foreach (var item in data)
                {
                    if (item.MAST_STATE_NAME != null)
                    {
                        vFB.FState = item.MAST_STATE_NAME.Trim();
                    }
                    else
                    {
                        vFB.FState = string.Empty;
                    }
                    if (item.MAST_DISTRICT_NAME != null)
                    {
                        vFB.FDistrict = item.MAST_DISTRICT_NAME.Trim();
                    }
                    else
                    {
                        vFB.FDistrict = string.Empty;
                    }
                    if (item.MAST_BLOCK_NAME != null)
                    {
                        vFB.FBlock = item.MAST_BLOCK_NAME.Trim();
                    }
                    else
                    {
                        vFB.FBlock = string.Empty;
                    }

                    if (item.FEED_TYPE == "G")
                    {
                        vFB.FB_Type = "Public";
                    }
                    else
                    {
                        vFB.FB_Type = "Private";
                    }
                    vFB.FName = item.FEED_NAME.Trim();
                    vFB.FTel = item.FEED_TELE_CODE + "-" + item.FEED_TELE_NUMBER;
                    vFB.FMob = item.FEED_MOBILE;
                    vFB.FEmail = item.FEED_EMAIL;
                    vFB.FAgainst = item.MAST_FEED_NAME.Trim();
                    vFB.FDate = comm.GetDateTimeToString(item.FEED_DATE).Trim();

                    vFB.PMGSYRoads = item.IS_PMGSY_ROAD == null ? "-" : (item.IS_PMGSY_ROAD.Trim() == "1" ? "Yes" : item.IS_PMGSY_ROAD.Trim() == "2" ? "No" : "Do Not Know");
                    vFB.RoadName = item.ROAD_NAME == null ? "-" : item.ROAD_NAME.Trim();
                    vFB.VillageName = item.VILLAGE_NAME == null ? "-" : item.VILLAGE_NAME.Trim();
                    vFB.NearestHabitation = item.NEAREST_HAB == null ? "-" : item.NEAREST_HAB.Trim();

                    if (item.PLAN_CN_ROAD_CODE != null)
                    {
                        vFB.RH_Name = dbContext.PLAN_ROAD.Where(a => a.PLAN_CN_ROAD_CODE == item.PLAN_CN_ROAD_CODE).Select(a => a.PLAN_RD_NAME).FirstOrDefault();
                    }
                    else if (item.MAST_HAB_CODE != null)
                    {
                        vFB.RH_Name = dbContext.MASTER_HABITATIONS.Where(a => a.MAST_HAB_CODE == item.MAST_HAB_CODE).Select(a => a.MAST_HAB_NAME).FirstOrDefault();
                    }

                    switch (item.FEED_FOR.Trim())
                    {
                        case "R":
                            vFB.FFor = "Road";
                            break;
                        case "H":
                            vFB.FFor = "Habitation";
                            break;
                        case "G":
                            vFB.FFor = "General";
                            break;
                    }
                    vFB.FComments = item.FEED_COMMENT;

                    switch (item.FEED_CATEGORY.Trim())
                    {
                        case "C":
                            vFB.FCategory = "Complaint";
                            break;
                        case "F":
                            vFB.FCategory = "Comment";
                            break;
                        case "Q":
                            vFB.FCategory = "Query";
                            break;
                        case "default":
                            vFB.FCategory = "General";
                            break;
                    }
                }
            }
            return PartialView("ViewFBDetails", vFB);
        }
        #endregion

        #region Feedback Approval
        public ActionResult FBApprovalDetails(String parameter, String hash, String key/*string fId*/)
        {
            int? state = 0;
            int? district = 0;
            PMGSY.Models.Feedback.FBApprovalDetails FBAppr = new Models.Feedback.FBApprovalDetails();
            try
            {
                decryptedParameters = PMGSY.Common.URLEncrypt.DecryptParameters1(new String[] { parameter, hash, key });
                if (decryptedParameters.Count > 0)
                {
                    string fId = Convert.ToString(decryptedParameters["FeedCode"]).Trim();
                    if (!int.TryParse(fId, out outParam))
                    {
                        return Json(false);
                    }
                    int id = Convert.ToInt32(fId);


                    FBAppr.State_List = fbDAL.PopulateStates(false);
                    if (PMGSYSession.Current.RoleCode == 25 || PMGSYSession.Current.RoleCode == 5)
                    {
                        FBAppr.State_List.Find(x => x.Value == "0").Value = "-1";
                    }

                    state = dbContext.ADMIN_FEEDBACK.Where(x => x.FEED_ID == id).Select(x => x.MAST_STATE_CODE).FirstOrDefault();
                    if (state > 0)
                    {
                        FBAppr.State = (int)state;
                    }

                    FBAppr.District_List = new List<SelectListItem>();
                    FBAppr.District_List.Insert(0, new SelectListItem() { Text = "Select District", Value = "-1" });

                    district = dbContext.ADMIN_FEEDBACK.Where(x => x.FEED_ID == id).Select(x => x.MAST_DISTRICT_CODE).FirstOrDefault();
                    if (district > 0)
                    {
                        FBAppr.District_List = fbDAL.PopulateDistricts(Convert.ToString(state).Trim());
                        FBAppr.District = (int)district;
                    }

                    FBAppr.hdnfeedId = Convert.ToInt32(fId.Trim());

                    var data = (from feedbak in dbContext.ADMIN_FEEDBACK
                                where
                                feedbak.FEED_ID == id
                                select new
                                {
                                    feedbak.FEED_APPROVAL_DATE,
                                    feedbak.FEED_APPROVAL,
                                    feedbak.FEED_STATUS,
                                    feedbak.CITIZEN_ID,
                                    feedbak.IS_PMGSY_ROAD
                                }).ToList();

                    foreach (var item in data)
                    {
                        if (item.FEED_APPROVAL == "Y")
                        {
                            //DateTime dt = ;
                            FBAppr.Appr_Date = comm.GetDateTimeToString((DateTime)item.FEED_APPROVAL_DATE);
                            FBAppr.Approval = item.FEED_APPROVAL.Trim();
                            FBAppr.ApprovalDisplay = "Feedback Accepted on Date=" + comm.GetDateTimeToString((DateTime)item.FEED_APPROVAL_DATE);
                        }
                        else
                        {
                            //FBAppr.Appr_Date = System.DateTime.Now.ToString("dd/MM/yyyy").Trim();
                            //FBAppr.Approval = "N";
                            if (item.FEED_APPROVAL_DATE != null)
                                FBAppr.ApprovalDisplay = "Feedback Not Accepted on Date=" + comm.GetDateTimeToString((DateTime)item.FEED_APPROVAL_DATE);
                            FBAppr.Approval = "N";
                        }
                        FBAppr.Repstat = item.FEED_STATUS.Trim();
                        FBAppr.CitizenId = item.CITIZEN_ID;

                        FBAppr.IS_PMGSY_ROAD = item.IS_PMGSY_ROAD == "1" ? true : false;
                    }

                    var query = (from fbRep in dbContext.ADMIN_FEEDBACK_REPLY
                                 where
                                 fbRep.FEED_ID == id
                                 select new
                                 {
                                     fbRep.FEED_ID,
                                     fbRep.REP_COMMENT,
                                     fbRep.REP_STATUS,
                                     fbRep.REP_DATE
                                 }).ToList();
                    if (query.Count > 0)
                    {
                        foreach (var itm in query)
                        {
                            FBAppr.Rep_ApprComments = itm.REP_COMMENT == null ? itm.REP_COMMENT : itm.REP_COMMENT.Trim();
                        }
                    }
                }
                return PartialView("FBApprovalDetails", FBAppr);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
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

        public JsonResult updateFBApproval(PMGSY.Models.Feedback.FBApprovalDetails apprFB)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    int flag = fbDAL.SaveFBCode(apprFB, Convert.ToString(apprFB.hdnfeedId));

                    return Json(new { message = flag });
                }
                else
                {
                    string message = ModelState.Values.Where(x => x.Errors.Count == 1).FirstOrDefault().Errors[0].ErrorMessage;

                    return Json(string.Empty);
                }

            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return null;
            }
        }

        public JsonResult fillDDLDistricts(string Code)
        {
            PMGSY.Models.Feedback.FBApprovalDetails apprFB = new PMGSY.Models.Feedback.FBApprovalDetails();

            //int outParam = 0;
            try
            {
                if (!int.TryParse(Code, out outParam))
                {
                    return Json(false);
                }
                List<SelectListItem> districtList = new List<SelectListItem>();

                districtList = fbDAL.PopulateDistricts(Code.Trim());

                //return Json(new SelectList(vwNwDtls.Srrda_List, "ADMIN_ND_NAME", "ADMIN_ND_CODE"));
                return Json(districtList);

            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return Json(string.Empty);
            }
        }
        #endregion

        #region Feedback Reply Status
        public ActionResult FBRepStatus(String parameter, String hash, String key/*string fId*/)
        {
            int id = 0;
            dbContext = new Models.PMGSYEntities();
            PMGSY.Models.Feedback.FBRepStatus fbRepStat = new Models.Feedback.FBRepStatus();
            decryptedParameters = PMGSY.Common.URLEncrypt.DecryptParameters1(new String[] { parameter, hash, key });
            if (decryptedParameters.Count > 0)
            {
                string fId = Convert.ToString(decryptedParameters["FeedCode"]).Trim();
                if (!int.TryParse(fId, out outParam))
                {
                    return Json(false);
                }
                id = Convert.ToInt32(fId.Trim());


                var data = (from f in dbContext.ADMIN_FEEDBACK where f.FEED_ID == id select f);

                fbRepStat.hdnfeedId = PMGSY.Common.URLEncrypt.EncryptParameters1(new string[] { "FeedCode =" + fId.Trim() });//id;
                fbRepStat.interimList = new List<string>();

                var q = dbContext.ADMIN_FEEDBACK_REPLY.Where(s => s.FEED_ID == id && s.REP_STATUS == "I").OrderBy(s => s.REP_ID).ToList();
                if (q.Count > 0)
                {
                    foreach (var item in q)
                    {
                        if (item.REP_COMMENT != null)
                        {
                            fbRepStat.interimList.Add(item.REP_COMMENT.Trim());
                        }
                        else
                        {
                            fbRepStat.interimList.Add(item.REP_COMMENT);
                        }
                    }
                }

                foreach (var itm in data)
                {
                    if (itm.FEED_EMAIL != null)
                    {
                        fbRepStat.ToAdd = itm.FEED_EMAIL.Trim();
                    }
                    else if (itm.FEED_TELE_CODE != null && itm.FEED_TELE_NUMBER != null)
                    {
                        if (itm.FEED_TELE_CODE != null)
                        {
                            fbRepStat.ToAdd = itm.FEED_TELE_CODE.Trim() + "-" + itm.FEED_TELE_NUMBER.Trim();
                        }
                    }
                    else
                    {
                        fbRepStat.ToAdd = itm.FEED_MOBILE;
                    }

                    fbRepStat.Date = comm.GetDateTimeToString((DateTime)itm.FEED_DATE).Trim();
                    fbRepStat.Comment = itm.FEED_COMMENT;
                    fbRepStat.RepStat = itm.FEED_STATUS.Trim();

                    fbRepStat.FBState = itm.MAST_STATE_CODE;

                    fbRepStat.fbapprDate = itm.FEED_APPROVAL_DATE == null ? "" : comm.GetDateTimeToString((DateTime)itm.FEED_APPROVAL_DATE).Trim();
                }

                var r = dbContext.ADMIN_FEEDBACK_REPLY.Where(s => s.FEED_ID == id && s.REP_STATUS == "F").ToList();
                if (r.Count > 0)
                {
                    foreach (var item in r)
                        fbRepStat.Final = item.REP_STATUS.Trim();
                }

                var a = dbContext.ADMIN_FEEDBACK.Where(s => s.FEED_ID == id && s.FEED_APPROVAL == "Y").ToList();
                if (a.Count > 0)
                {
                    foreach (var itm in a)
                    {
                        fbRepStat.hdnappr = itm.FEED_APPROVAL.Trim();
                        fbRepStat.hdnstate = Convert.ToString(itm.MAST_STATE_CODE).Trim();
                        fbRepStat.hdndist = Convert.ToString(itm.MAST_DISTRICT_CODE).Trim();
                        fbRepStat.hdnblock = Convert.ToString(itm.MAST_BLOCK_CODE).Trim();

                        fbRepStat.hdnStateCode = Convert.ToInt32(itm.MAST_STATE_CODE);
                        fbRepStat.hdnDistrictCode = Convert.ToInt32(itm.MAST_DISTRICT_CODE);
                        fbRepStat.hdnBlockCode = Convert.ToInt32(itm.MAST_BLOCK_CODE);
                        fbRepStat.IS_PMGSY_ROAD = itm.IS_PMGSY_ROAD == "1" ? true : false;
                    }
                }
                else
                {
                    fbRepStat.hdnappr = "N";
                }
            }
            var query = dbContext.ADMIN_FEEDBACK_REPLY.Where(x => x.FEED_ID == id).OrderByDescending(x => x.REP_DATE).FirstOrDefault();
            if (query != null)
            {
                fbRepStat.hdRole = query.UM_User_Master.DefaultRoleID;
                fbRepStat.hdRepStatus = query.REP_STATUS;
            }

            var feedback = dbContext.ADMIN_FEEDBACK.Where(s => s.FEED_ID == id).ToList();
            if (feedback.Count > 0)
            {
                foreach (var itm in feedback)
                {
                    fbRepStat.hdnStateCode = Convert.ToInt32(itm.MAST_STATE_CODE);
                    fbRepStat.hdnDistrictCode = Convert.ToInt32(itm.MAST_DISTRICT_CODE);
                    fbRepStat.hdnBlockCode = Convert.ToInt32(itm.MAST_BLOCK_CODE);
                    fbRepStat.IS_PMGSY_ROAD = itm.IS_PMGSY_ROAD == "1" ? true : false;
                }
            }

            return PartialView("FBRepStatus", fbRepStat);
        }

        [HttpPost]
        public ActionResult FBRepStatList(FormCollection formCollection)
        {
            Feedback.DAL.FeedbackDAL qualityDAL = new Feedback.DAL.FeedbackDAL();
            int totalRecords;
            try
            {
                using (PMGSY.Common.CommonFunctions commonFunction = new PMGSY.Common.CommonFunctions())
                {
                    if (!commonFunction.ValidateGridParameters(new PMGSY.Common.GridParams(Convert.ToInt32(formCollection["page"]), Convert.ToInt32(formCollection["rows"]), formCollection["sidx"], formCollection["sord"], Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                    {
                        return null;
                    }
                }

                String parameter = null, hash = null, key = null;
                string[] encoded = Convert.ToString(formCollection["id"]).Trim().Split('/');
                decryptedParameters = PMGSY.Common.URLEncrypt.DecryptParameters1(new String[] { encoded[0], encoded[1], encoded[2] });
                if (decryptedParameters.Count < 0)
                {
                    //totalRecords = 0;
                    return null;
                }
                string fId = decryptedParameters["FeedCode"].ToString();
                var jsonData = new
                {
                    rows = qualityDAL.FBRepListtDAL(Convert.ToInt32(formCollection["page"]) - 1, Convert.ToInt32(formCollection["rows"]),
                                                            formCollection["sidx"], formCollection["sord"], out totalRecords,
                        /*Convert.ToString(formCollection["id"])*/fId.Trim()),
                    //"1"),
                    total = totalRecords <= Convert.ToInt32(formCollection["rows"]) ? 1 : (totalRecords % Convert.ToInt32(formCollection["rows"]) == 0 ? totalRecords / Convert.ToInt32(formCollection["rows"]) : (totalRecords / Convert.ToInt32(formCollection["rows"]) + 1)),
                    page = Convert.ToInt32(formCollection["page"]),
                    records = totalRecords
                };
                return Json(jsonData, JsonRequestBehavior.AllowGet);
                //return null;
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return Json(String.Empty);
            }
        }

        public ActionResult FeedbackReply(String parameter, String hash, String key/*string fId*/)
        {
            PMGSY.Models.Feedback.FeedbackReply FBRep = new Models.Feedback.FeedbackReply();
            decryptedParameters = PMGSY.Common.URLEncrypt.DecryptParameters1(new String[] { parameter, hash, key });
            if (decryptedParameters.Count > 0)
            {
                string fId = Convert.ToString(decryptedParameters["FeedCode"]).Trim();

                if (!int.TryParse(fId, out outParam))
                {
                    return Json(false);
                }
                //fId = "1";
                int feedId = Convert.ToInt32(fId.Trim());


                dbContext = new Models.PMGSYEntities();

                var a = dbContext.ADMIN_FEEDBACK.Where(m => m.FEED_ID == feedId).OrderBy(s => s.FEED_ID).ToList();
                foreach (var itm in a)
                {
                    if (itm.MAST_STATE_CODE != null)
                    {
                        FBRep.hdnRepStateCode = (int)itm.MAST_STATE_CODE;
                    }
                    else
                    {
                        FBRep.hdnRepStateCode = 0;
                    }
                    FBRep.hdnRepDistrictCode = itm.MAST_DISTRICT_CODE.HasValue ? itm.MAST_DISTRICT_CODE.Value : 0;
                    FBRep.hdnStateType = itm.MAST_STATE_CODE > 0 ? itm.MASTER_STATE.MAST_STATE_UT.Trim() : null;
                }

                string Rep_Stat = string.Empty;
                var q = dbContext.ADMIN_FEEDBACK_REPLY.Where(m => m.FEED_ID == feedId).OrderBy(s => s.REP_ID).ToList();
                foreach (var item in q)
                {
                    Rep_Stat = item.REP_STATUS.Trim();
                    FBRep.hdnRole = item.UM_User_Master.DefaultRoleID;
                }

                FBRep.hdnRepStatus = Rep_Stat.Trim();
                FBRep.hdnFBId = PMGSY.Common.URLEncrypt.EncryptParameters1(new string[] { "FeedCode =" + fId.Trim() });//Convert.ToInt32(fId);

                FBRep.hdnDBOpr = "I";
            }
            return View("FeedbackReply", FBRep);
        }

        public ActionResult DisplayFBFiles(String parameter, String hash, String key/*string nId*/)
        {
            //PMGSY.Models.NewsDetails.DisplayNewsFiles dsplnewsfiles = new Models.NewsDetails.DisplayNewsFiles();
            PMGSY.Models.Feedback.DisplayFBFiles dsplFBfiles = new Models.Feedback.DisplayFBFiles();

            dbContext = new Models.PMGSYEntities();

            decryptedParameters = PMGSY.Common.URLEncrypt.DecryptParameters1(new String[] { parameter, hash, key });
            if (decryptedParameters.Count > 0)
            {
                string nId = Convert.ToString(decryptedParameters["FeedCode"]).Trim();
                if (!int.TryParse(nId, out outParam))
                {
                    return Json(false);
                }
                string FullfilePhysicalPath = string.Empty;

                SelectListItem item;
                //if (nId == null)
                //{
                //    nId = "1";
                //}
                int FeedId = Convert.ToInt32(nId.Trim());



                var s = dbContext.ADMIN_FEEDBACK_FILES.Where(a => a.FEED_ID == FeedId).OrderBy(a => a.FILE_ID).ToList();

                if (s.Count > 0)
                {
                    dsplFBfiles.FileLat = new decimal[s.Count];
                    dsplFBfiles.FileLong = new decimal[s.Count];

                    int ct = 0;

                    dsplFBfiles.path = new List<SelectListItem>();
                    foreach (var a in s)
                    {
                        //switch(a.ADMIN_NEWS.ADMIN_DEPARTMENT.)
                        {
                            //  case 
                            //dsplFBfiles.IssuedBy = a.ADMIN_FEEDBACK.UM_User_Master.UserName.Trim();//a.ADMIN_NEWS.NEWS_USER_ID.ToString();
                            dsplFBfiles.IssuedBy = "";
                            dsplFBfiles.IssuedDate = a.ADMIN_FEEDBACK.FEED_DATE.ToString("dd/MM/yyyy hh:mm tt");
                            dsplFBfiles.Title = a.ADMIN_FEEDBACK.FEED_NAME.Trim();
                            dsplFBfiles.Description = a.ADMIN_FEEDBACK.FEED_SUBJECT;
                        }
                        item = new SelectListItem();
                        if (a.FILE_TYPE == "P")
                        {
                            //FullFileLogicalPath = Path.Combine(ConfigurationManager.AppSettings["NEWS_PDF_FILE_UPLOAD_VIRTUAL_DIR_PATH"], FileName);
                            //FullfilePhysicalPath = Path.Combine(ConfigurationManager.AppSettings["NEWS_PDF_FILE_UPLOAD"], a.FILE_NAME.Trim());
                            FullfilePhysicalPath = Path.Combine(ConfigurationManager.AppSettings["FEEDBACK_PDF_FILE_UPLOAD_VIRTUAL_DIR_PATH"], a.FILE_NAME.Trim());
                        }
                        else
                        {
                            //FullFileLogicalPath = Path.Combine(ConfigurationManager.AppSettings["NEWS_IMAGE_FILE_UPLOAD_VIRTUAL_DIR_PATH"], FileName);
                            //FullfilePhysicalPath = Path.Combine(ConfigurationManager.AppSettings["NEWS_IMAGE_FILE_UPLOAD"], a.FILE_NAME.Trim());
                            FullfilePhysicalPath = Path.Combine(ConfigurationManager.AppSettings["FEEDBACK_IMAGE_FILE_UPLOAD_VIRTUAL_DIR_PATH"], a.FILE_NAME.Trim());
                        }
                        item.Text = FullfilePhysicalPath.Trim();
                        item.Value = a.FILE_TYPE.Trim();
                        dsplFBfiles.path.Add(item);

                        //dsplFBfiles.FileLat[ct] = (decimal)a.FILE_LAT;
                        //dsplFBfiles.FileLong[ct] = (decimal)a.FILE_LONG;
                        dsplFBfiles.IsLatLongAvailable = (a.FILE_LAT != null && a.FILE_LONG != null) ? true : false;
                        if (ct == 0)
                        {
                            dsplFBfiles.LatLong += a.FILE_LAT == null ? "" : a.FILE_LAT.ToString() + "$$";
                            dsplFBfiles.LatLong += a.FILE_LONG == null ? "" : a.FILE_LONG.ToString();
                        }
                        else
                        {
                            dsplFBfiles.LatLong += "$$$";
                            dsplFBfiles.LatLong += (a.FILE_LAT == null ? "" : a.FILE_LAT.ToString()) + "$$";
                            dsplFBfiles.LatLong += a.FILE_LONG == null ? "" : a.FILE_LONG.ToString();
                        }
                        Dictionary<string, string> lstPaths = new Dictionary<string, string>();
                        lstPaths.Add(Path.Combine(FullfilePhysicalPath, HttpUtility.UrlEncode(a.FILE_NAME.ToString())).ToString().Replace(@"\\", @"//").Replace(@"\", @"/") +
                                            "$$$" + (a.FILE_LAT == null ? "0" : a.FILE_LAT.ToString()) + "$$$" +
                                            (a.FILE_LONG == null ? "0" : a.FILE_LONG.ToString()), a.FILE_NAME.ToString());

                        ViewBag.FileDetails = lstPaths;

                        ct++;
                    }
                }
            }
            return View("DisplayFBFiles", dsplFBfiles);
        }


        public ActionResult FeedbackReplyUpdate(String parameter, String hash, String key/*string fId*/)
        {
            PMGSY.Models.Feedback.FeedbackReply FBRep = new Models.Feedback.FeedbackReply();
            decryptedParameters = PMGSY.Common.URLEncrypt.DecryptParameters1(new String[] { parameter, hash, key });
            if (decryptedParameters.Count > 0)
            {
                string fId = Convert.ToString(decryptedParameters["FeedCode"]).Trim();
                string[] ID = fId.Split('$');

                if (!int.TryParse(ID[0], out outParam))
                {
                    return Json(false);
                }
                if (!int.TryParse(ID[1], out outParam))
                {
                    return Json(false);
                }
                int feed_Id = Convert.ToInt32(ID[0].Trim());
                int rep_Id = Convert.ToInt32(ID[1].Trim());

                dbContext = new Models.PMGSYEntities();

                var a = dbContext.ADMIN_FEEDBACK.Where(m => m.FEED_ID == feed_Id).OrderBy(s => s.FEED_ID).ToList();
                foreach (var itm in a)
                {
                    if (itm.MAST_STATE_CODE != null)
                    {
                        FBRep.hdnRepStateCode = (int)itm.MAST_STATE_CODE;
                    }
                    else
                    {
                        FBRep.hdnRepStateCode = 0;
                    }
                    FBRep.hdnRepDistrictCode = itm.MAST_DISTRICT_CODE.HasValue ? itm.MAST_STATE_CODE.Value : 0;
                }

                string Rep_Stat = string.Empty;
                var q = dbContext.ADMIN_FEEDBACK_REPLY.Where(m => m.FEED_ID == feed_Id && m.REP_ID == rep_Id).OrderBy(s => s.REP_ID).ToList();
                foreach (var item in q)
                {
                    Rep_Stat = item.REP_STATUS.Trim();
                    FBRep.Rep_Comments = item.REP_COMMENT;
                    FBRep.hdnRole = item.UM_User_Master.DefaultRoleID;
                }

                FBRep.hdnDBOpr = "M";

                FBRep.hdnRepStatus = Rep_Stat.Trim();
                FBRep.hdnFBId = PMGSY.Common.URLEncrypt.EncryptParameters1(new string[] { "FeedCode =" + feed_Id });//Convert.ToString(feed_Id).Trim();
                FBRep.hdnRepId = rep_Id;
            }
            return View("FeedbackReply", FBRep);
        }

        public JsonResult saveFeedBackRep(PMGSY.Models.Feedback.FeedbackReply fbRep)
        {
            try
            {
                ModelState.Remove("TIMELINE_DATE");
                if (ModelState.IsValid)
                {
                    string[] encoded = fbRep.hdnFBId.Trim().Split('/');
                    decryptedParameters = PMGSY.Common.URLEncrypt.DecryptParameters1(new String[] { encoded[0], encoded[1], encoded[2] });
                    if (decryptedParameters.Count < 0)
                    {
                        //totalRecords = 0;
                        return Json(String.Empty);
                    }
                    //bool flag = fbDAL.SaveFBRep(fbRep, Convert.ToString(fbRep.hdnFBId));
                    bool flag = fbDAL.SaveFBRep(fbRep, Convert.ToString(decryptedParameters["FeedCode"]));

                    return Json(new { status = flag });
                }
                else
                {
                    return Json(string.Empty);
                }
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return null;
            }
        }

        public JsonResult updateFeedBackRep(PMGSY.Models.Feedback.FeedbackReply fbRep)
        {
            try
            {
                ModelState.Remove("TIMELINE_DATE");
                if (ModelState.IsValid)
                {
                    string[] encoded = fbRep.hdnFBId.Trim().Split('/');
                    decryptedParameters = PMGSY.Common.URLEncrypt.DecryptParameters1(new String[] { encoded[0], encoded[1], encoded[2] });
                    if (decryptedParameters.Count < 0)
                    {
                        //totalRecords = 0;
                        return Json(String.Empty);
                    }
                    //bool flag = fbDAL.UpdateFBRep(fbRep, fbRep.hdnFBId, fbRep.hdnRepId);
                    bool flag = fbDAL.UpdateFBRep(fbRep, Convert.ToString(decryptedParameters["FeedCode"]).Trim(), fbRep.hdnRepId);

                    return Json(new { status = flag });
                }
                else
                {
                    return Json(string.Empty);
                }
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return null;
            }
        }

        public JsonResult deleteFeedBackRep(String parameter, String hash, String key/*string id*/)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    bool flag = false;
                    decryptedParameters = PMGSY.Common.URLEncrypt.DecryptParameters1(new String[] { parameter, hash, key });
                    if (decryptedParameters.Count > 0)
                    {
                        string id = Convert.ToString(decryptedParameters["FeedCode"]).Trim();
                        string[] fId = id.Trim().Split('$');

                        if (!int.TryParse(fId[0], out outParam))
                        {
                            return Json(false);
                        }
                        if (!int.TryParse(fId[1], out outParam))
                        {
                            return Json(false);
                        }

                        int feedId = Convert.ToInt32(fId[0].Trim());
                        int repId = Convert.ToInt32(fId[1].Trim());
                        flag = fbDAL.DeleteFBRep(feedId, repId);

                    }
                    return Json(new { status = flag }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    string message = ModelState.Values.Where(x => x.Errors.Count == 1).FirstOrDefault().Errors[0].ErrorMessage;
                    return Json(string.Empty);
                }
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return null;
            }
        }
        #endregion

        #region Advanced Search
        [HttpGet]
        public ActionResult SearchFeedback()
        {
            if (PMGSYSession.Current == null)
            {
                Response.Redirect("/Login/Login");
            }
            try
            {
                PMGSY.Models.Feedback.SearchFeedback srFB = new Models.Feedback.SearchFeedback();

                return View("SearchFeedback", srFB);
            }
            catch (Exception ex)
            {
                return null;
            }
            finally
            { 
                
            }
        }
        
        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult searchFBDetails(PMGSY.Models.Feedback.SearchFeedback sFB)
        {
            if (PMGSYSession.Current == null)
            {
                Response.Redirect("/Login/Login");
            }
            CommonFunctions comm = new CommonFunctions();
            try
            {
                if (ModelState.IsValid)
                {
                    fbDAL.searchFBDetailsDAL(ref sFB);

                    return PartialView("searchFBDetails", sFB);
                }
                else
                {
                    string message = "";
                    message = ModelState.Values.Where(x => x.Errors.Count == 1).FirstOrDefault().Errors[0].ErrorMessage;

                    return null;
                }
            }
            catch (Exception ex)
            {
                //Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return null;
            }
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult FBListSearch(PMGSY.Models.Feedback.SearchFeedback sFB)
        {
            if (PMGSYSession.Current == null)
            {
                Response.Redirect("/Login/Login");
            }
            try
            {
                if (ModelState.IsValid)
                {
                    fbDAL.FBListSearchDAL(ref sFB);

                    return PartialView("FBList", sFB);
                }
                else
                {
                    string message = string.Empty;
                    message = ModelState.Values.Where(x => x.Errors.Count == 1).FirstOrDefault().Errors[0].ErrorMessage;
                    return null;
                }
            }
            catch (Exception ex)
            {
                //Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return null;
            }
        }
        #endregion



        #region New Feedback

        #region Other Methods
        // Filters
        public ActionResult DetailsFBNew()
        {
            try
            {
                Common.CommonFunctions com = new Common.CommonFunctions();
                PMGSY.Models.Feedback.FeedbackDetails.DetailsFB Fb = new Models.Feedback.FeedbackDetails.DetailsFB();
                Fb.Years_List = PopulateYear(System.DateTime.Now.Year, true, true);
                Fb.State_List = fbDAL.PopulateStates(true);

                Fb.Months_List = com.PopulateMonths(System.DateTime.Now.Month);

                Fb.Status_List = fillStatus("N");

                Fb.MONTHs = System.DateTime.Now.Month;
                Fb.YEARs = System.DateTime.Now.Year;

                Fb.Category = "0";
                Fb.Approved = PMGSY.Extensions.PMGSYSession.Current.RoleCode == 25 ? "0" : "Y";
                Fb.Status = "0";

                Fb.hdnRoleId = PMGSY.Extensions.PMGSYSession.Current.RoleCode;

                return View("DetailsFBNew", Fb);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "FeedbackDetailsController.DetailsFBNew()");
                return null;
            }
        }

        // Get List
        [HttpPost]
        public ActionResult FBListNew(FormCollection formCollection)
        {
            Feedback.BAL.IDetailsFBBAL qualityBAL = new Feedback.BAL.DetailsFBBAL();
            int totalRecords;
            Feedback.DAL.FeedbackDAL fbDAL = new Feedback.DAL.FeedbackDAL();
            try
            {
                using (PMGSY.Common.CommonFunctions commonFunction = new PMGSY.Common.CommonFunctions())
                {
                    if (!commonFunction.ValidateGridParameters(new PMGSY.Common.GridParams(Convert.ToInt32(formCollection["page"]), Convert.ToInt32(formCollection["rows"]), formCollection["sidx"], formCollection["sord"], Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                    {
                        return null;
                    }
                }
                var jsonData = new
                {

                    rows = fbDAL.FBListDALNew(Convert.ToInt32(formCollection["page"]) - 1, Convert.ToInt32(formCollection["rows"]),
                                                            formCollection["sidx"], formCollection["sord"], out totalRecords,
                                                            Convert.ToInt32(formCollection["FOR_MONTH"]), Convert.ToInt32(formCollection["FOR_YEAR"]),
                                                            Convert.ToInt32(formCollection["FOR_STATES"]), Convert.ToString(formCollection["FOR_CATEGORY"]),
                                                            Convert.ToString(formCollection["APPR"]), Convert.ToString(formCollection["STATUS"]), Convert.ToString(formCollection["fbThrough"])),
                    total = totalRecords <= Convert.ToInt32(formCollection["rows"]) ? 1 : (totalRecords % Convert.ToInt32(formCollection["rows"]) == 0 ? totalRecords / Convert.ToInt32(formCollection["rows"]) : (totalRecords / Convert.ToInt32(formCollection["rows"]) + 1)),
                    page = Convert.ToInt32(formCollection["page"]),
                    records = totalRecords
                };
                return Json(jsonData, JsonRequestBehavior.AllowGet);
                //return null;
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "FeedbackDetailsController.FBListNew()");
                // Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return Json(String.Empty);
            }
        }

        // Show Citizen Feedback Details
        public ActionResult ViewFBDetailsNew(String parameter, String hash, String key/*string fId */)
        {
            try
            {
                PMGSYEntities dbContext = new PMGSYEntities();
                decryptedParameters = PMGSY.Common.URLEncrypt.DecryptParameters1(new String[] { parameter, hash, key });
                PMGSY.Models.Feedback.ViewFBDetails vFB = new Models.Feedback.ViewFBDetails();
                if (decryptedParameters.Count > 0)
                {
                    string fId = Convert.ToString(decryptedParameters["FeedCode"]).Trim();
                    if (!int.TryParse(fId, out outParam))
                    {
                        return Json(false);
                    }
                    int id = Convert.ToInt32(fId);


                    string ContactMe = dbContext.ADMIN_FEEDBACK.Where(m => m.FEED_ID == id).Select(m => m.CONTACT_ME).FirstOrDefault();

                    if (string.IsNullOrEmpty(ContactMe))
                    { // Show
                        vFB.AllowToContact = "Y";
                    }
                    else if (ContactMe.Equals("N"))
                    {// Hide
                        vFB.AllowToContact = "N";
                    }
                    else
                    { // Show
                        vFB.AllowToContact = "Y";
                    }




                    vFB.hdnfeedId = PMGSY.Common.URLEncrypt.EncryptParameters1(new string[] { "FeedCode =" + fId.Trim() });//Convert.ToInt32(fId.Trim());

                    var data = (from feedbak in dbContext.ADMIN_FEEDBACK
                                where
                                feedbak.FEED_ID == id
                                select new
                                {
                                    feedbak.FEED_PREFIX,
                                    feedbak.FEED_NAME,
                                    feedbak.FEED_TYPE,
                                    feedbak.FEED_CODE,
                                    feedbak.FEED_TELE_CODE,
                                    feedbak.FEED_TELE_NUMBER,
                                    feedbak.FEED_MOBILE,
                                    feedbak.FEED_EMAIL,
                                    feedbak.FEED_DATE,
                                    feedbak.FEED_CATEGORY,
                                    feedbak.MASTER_FEEDBACK_CATEGORY.MAST_FEED_NAME,
                                    feedbak.FEED_FOR,
                                    feedbak.FEED_COMMENT,
                                    feedbak.MASTER_STATE.MAST_STATE_NAME,
                                    feedbak.MASTER_DISTRICT.MAST_DISTRICT_NAME,
                                    feedbak.MASTER_BLOCK.MAST_BLOCK_NAME,
                                    feedbak.PLAN_CN_ROAD_CODE,
                                    feedbak.MAST_HAB_CODE,
                                    feedbak.IS_PMGSY_ROAD,
                                    feedbak.ROAD_NAME,
                                    feedbak.VILLAGE_NAME,
                                    feedbak.NEAREST_HAB,
                                }).ToList();

                    //List<PMGSY.Models.ADMIN_FEEDBACK> lst= new List<Models.ADMIN_FEEDBACK>();
                    //lst=dbContext.ADMIN_FEEDBACK.Where(M=>M.FEED_ID==id).ToList<Models.ADMIN_FEEDBACK>();
                    //lst.Select(S=>S.PLAN_ROAD.PLAN_RD_NAME).FirstOrDefault();


                    foreach (var item in data)
                    {
                        if (item.MAST_STATE_NAME != null)
                        {
                            vFB.FState = item.MAST_STATE_NAME.Trim();
                        }
                        else
                        {
                            vFB.FState = string.Empty;
                        }
                        if (item.MAST_DISTRICT_NAME != null)
                        {
                            vFB.FDistrict = item.MAST_DISTRICT_NAME.Trim();
                        }
                        else
                        {
                            vFB.FDistrict = string.Empty;
                        }
                        if (item.MAST_BLOCK_NAME != null)
                        {
                            vFB.FBlock = item.MAST_BLOCK_NAME.Trim();
                        }
                        else
                        {
                            vFB.FBlock = string.Empty;
                        }

                        if (item.FEED_TYPE == "G")
                        {
                            vFB.FB_Type = "Public";
                        }
                        else
                        {
                            vFB.FB_Type = "Private";
                        }
                        vFB.FName = item.FEED_NAME.Trim();
                        vFB.FTel = item.FEED_TELE_CODE + "-" + item.FEED_TELE_NUMBER;
                        vFB.FMob = item.FEED_MOBILE;
                        vFB.FEmail = item.FEED_EMAIL;
                        vFB.FAgainst = item.MAST_FEED_NAME.Trim();
                        vFB.FDate = comm.GetDateTimeToString(item.FEED_DATE).Trim();

                        vFB.PMGSYRoads = item.IS_PMGSY_ROAD == null ? "-" : (item.IS_PMGSY_ROAD.Trim() == "1" ? "Yes" : item.IS_PMGSY_ROAD.Trim() == "2" ? "No" : "Do Not Know");
                        vFB.RoadName = item.ROAD_NAME == null ? "-" : item.ROAD_NAME.Trim();

                        int VillageCode;
                        bool isNumericVillage = int.TryParse(item.VILLAGE_NAME, out VillageCode);


                        int HabCode;
                        bool isNumericHab = int.TryParse(item.NEAREST_HAB, out HabCode);

                        if (isNumericVillage)
                        {
                          //  Int32 VillageCode = item.VILLAGE_NAME == null ? 0 : Convert.ToInt32(item.VILLAGE_NAME);
                            vFB.VillageName = item.VILLAGE_NAME == null ? "-" : dbContext.MASTER_VILLAGE.Where(m => m.MAST_VILLAGE_CODE == VillageCode).Select(m => m.MAST_VILLAGE_NAME).FirstOrDefault();
                        }
                        else
                        { 
                           vFB.VillageName=item.VILLAGE_NAME == null ? "-" : item.VILLAGE_NAME.Trim();
                        }

                        if (isNumericHab)
                        {
                          //  Int32 HabCode = item.NEAREST_HAB == null ? 0 : Convert.ToInt32(item.NEAREST_HAB);
                            vFB.NearestHabitation = item.NEAREST_HAB == null ? "-" : dbContext.MASTER_HABITATIONS.Where(m => m.MAST_HAB_CODE == HabCode).Select(m => m.MAST_HAB_NAME).FirstOrDefault();
                        }
                        else
                        {
                            if (!string.IsNullOrEmpty(item.NEAREST_HAB))
                            {

                                if (item.NEAREST_HAB.Trim().Contains(','))
                                {
                                    string[] subs = item.NEAREST_HAB.Trim().Split(',');

                                    string abc = string.Empty;

                                    foreach (string i in subs)
                                    {
                                        int HabIdFromList;

                                        bool isHab = int.TryParse(i, out HabIdFromList);

                                        if (isHab)
                                        {
                                            vFB.NearestHabitation = dbContext.MASTER_HABITATIONS.Where(m => m.MAST_HAB_CODE == HabIdFromList).Select(m => m.MAST_HAB_NAME).FirstOrDefault();
                                            abc = abc+"," + vFB.NearestHabitation;
                                        }
                                    }
                                    vFB.NearestHabitation = abc;
                                }
                                else 
                                {
                                    vFB.NearestHabitation = item.NEAREST_HAB == null ? "-" : item.NEAREST_HAB.Trim();
                                
                                }


                            }
                            else
                            {
                                vFB.NearestHabitation = item.NEAREST_HAB == null ? "-" : item.NEAREST_HAB.Trim();
                            }
                        
                        }
                       
                       


                       
                     

                        //vFB.VillageName = item.VILLAGE_NAME == null ? "-" : item.VILLAGE_NAME.Trim();
                        //vFB.NearestHabitation = item.NEAREST_HAB == null ? "-" : item.NEAREST_HAB.Trim();

                        if (item.PLAN_CN_ROAD_CODE != null)
                        {
                            vFB.RH_Name = dbContext.PLAN_ROAD.Where(a => a.PLAN_CN_ROAD_CODE == item.PLAN_CN_ROAD_CODE).Select(a => a.PLAN_RD_NAME).FirstOrDefault();
                        }
                        else if (item.MAST_HAB_CODE != null)
                        {
                            vFB.RH_Name = dbContext.MASTER_HABITATIONS.Where(a => a.MAST_HAB_CODE == item.MAST_HAB_CODE).Select(a => a.MAST_HAB_NAME).FirstOrDefault();
                        }

                        switch (item.FEED_FOR.Trim())
                        {
                            case "R":
                                vFB.FFor = "Road";
                                break;
                            case "H":
                                vFB.FFor = "Habitation";
                                break;
                            case "G":
                                vFB.FFor = "General";
                                break;
                        }
                        vFB.FComments = item.FEED_COMMENT;

                        switch (item.FEED_CATEGORY.Trim())
                        {
                            case "C":
                                vFB.FCategory = "Complaint";
                                break;
                            case "F":
                                vFB.FCategory = "Comment";
                                break;
                            case "Q":
                                vFB.FCategory = "Query";
                                break;
                            case "default":
                                vFB.FCategory = "General";
                                break;
                        }
                    }
                }
                return PartialView("ViewFBDetailsNew", vFB);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "FeedbackDetailsController.ViewFBDetailsNew()");
                return null;
            }
        }

        // Show Map and Pics
        public ActionResult DisplayFBFilesNew(String parameter, String hash, String key/*string nId*/)
        {
            //PMGSY.Models.NewsDetails.DisplayNewsFiles dsplnewsfiles = new Models.NewsDetails.DisplayNewsFiles();
            try
            {
                PMGSY.Models.Feedback.DisplayFBFiles dsplFBfiles = new Models.Feedback.DisplayFBFiles();

                dbContext = new Models.PMGSYEntities();

                decryptedParameters = PMGSY.Common.URLEncrypt.DecryptParameters1(new String[] { parameter, hash, key });
                if (decryptedParameters.Count > 0)
                {
                    string nId = Convert.ToString(decryptedParameters["FeedCode"]).Trim();
                    if (!int.TryParse(nId, out outParam))
                    {
                        return Json(false);
                    }
                    string FullfilePhysicalPath = string.Empty;

                    SelectListItem item;
                    //if (nId == null)
                    //{
                    //    nId = "1";
                    //}
                    int FeedId = Convert.ToInt32(nId.Trim());



                    var s = dbContext.ADMIN_FEEDBACK_FILES.Where(a => a.FEED_ID == FeedId).OrderBy(a => a.FILE_ID).ToList();

                    if (s.Count > 0)
                    {
                        dsplFBfiles.FileLat = new decimal[s.Count];
                        dsplFBfiles.FileLong = new decimal[s.Count];

                        int ct = 0;

                        dsplFBfiles.path = new List<SelectListItem>();
                        foreach (var a in s)
                        {
                            //switch(a.ADMIN_NEWS.ADMIN_DEPARTMENT.)
                            {
                                //  case 
                                //dsplFBfiles.IssuedBy = a.ADMIN_FEEDBACK.UM_User_Master.UserName.Trim();//a.ADMIN_NEWS.NEWS_USER_ID.ToString();
                                dsplFBfiles.IssuedBy = "";
                                dsplFBfiles.IssuedDate = a.ADMIN_FEEDBACK.FEED_DATE.ToString("dd/MM/yyyy hh:mm tt");
                                dsplFBfiles.Title = a.ADMIN_FEEDBACK.FEED_NAME.Trim();
                                dsplFBfiles.Description = a.ADMIN_FEEDBACK.FEED_SUBJECT;
                            }
                            item = new SelectListItem();
                            if (a.FILE_TYPE == "P")
                            {
                                //FullFileLogicalPath = Path.Combine(ConfigurationManager.AppSettings["NEWS_PDF_FILE_UPLOAD_VIRTUAL_DIR_PATH"], FileName);
                                //FullfilePhysicalPath = Path.Combine(ConfigurationManager.AppSettings["NEWS_PDF_FILE_UPLOAD"], a.FILE_NAME.Trim());
                                FullfilePhysicalPath = Path.Combine(ConfigurationManager.AppSettings["FEEDBACK_PDF_FILE_UPLOAD_VIRTUAL_DIR_PATH"], a.FILE_NAME.Trim());
                            }
                            else
                            {
                                //FullFileLogicalPath = Path.Combine(ConfigurationManager.AppSettings["NEWS_IMAGE_FILE_UPLOAD_VIRTUAL_DIR_PATH"], FileName);
                                //FullfilePhysicalPath = Path.Combine(ConfigurationManager.AppSettings["NEWS_IMAGE_FILE_UPLOAD"], a.FILE_NAME.Trim());
                                FullfilePhysicalPath = Path.Combine(ConfigurationManager.AppSettings["FEEDBACK_IMAGE_FILE_UPLOAD_VIRTUAL_DIR_PATH"], a.FILE_NAME.Trim());
                            }
                            item.Text = FullfilePhysicalPath.Trim();
                            item.Value = a.FILE_TYPE.Trim();
                            dsplFBfiles.path.Add(item);

                            //dsplFBfiles.FileLat[ct] = (decimal)a.FILE_LAT;
                            //dsplFBfiles.FileLong[ct] = (decimal)a.FILE_LONG;
                            dsplFBfiles.IsLatLongAvailable = (a.FILE_LAT != null && a.FILE_LONG != null) ? true : false;
                            if (ct == 0)
                            {
                                dsplFBfiles.LatLong += a.FILE_LAT == null ? "" : a.FILE_LAT.ToString() + "$$";
                                dsplFBfiles.LatLong += a.FILE_LONG == null ? "" : a.FILE_LONG.ToString();
                            }
                            else
                            {
                                dsplFBfiles.LatLong += "$$$";
                                dsplFBfiles.LatLong += (a.FILE_LAT == null ? "" : a.FILE_LAT.ToString()) + "$$";
                                dsplFBfiles.LatLong += a.FILE_LONG == null ? "" : a.FILE_LONG.ToString();
                            }
                            Dictionary<string, string> lstPaths = new Dictionary<string, string>();
                            lstPaths.Add(Path.Combine(FullfilePhysicalPath, HttpUtility.UrlEncode(a.FILE_NAME.ToString())).ToString().Replace(@"\\", @"//").Replace(@"\", @"/") +
                                                "$$$" + (a.FILE_LAT == null ? "0" : a.FILE_LAT.ToString()) + "$$$" +
                                                (a.FILE_LONG == null ? "0" : a.FILE_LONG.ToString()), a.FILE_NAME.ToString());

                            ViewBag.FileDetails = lstPaths;

                            ct++;
                        }
                    }
                }
                return View("DisplayFBFilesNew", dsplFBfiles);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "FeedbackDetailsController.DisplayFBFilesNew()");
                return null;
            }
        }

        // Replay by PIU
        public ActionResult FBRepStatusNew(String parameter, String hash, String key/*string fId*/)
        {
            try
            {
                int id = 0;
                dbContext = new Models.PMGSYEntities();
                PMGSY.Models.Feedback.FBRepStatusNew fbRepStat = new Models.Feedback.FBRepStatusNew();
                decryptedParameters = PMGSY.Common.URLEncrypt.DecryptParameters1(new String[] { parameter, hash, key });
                if (decryptedParameters.Count > 0)
                {
                    string fId = Convert.ToString(decryptedParameters["FeedCode"]).Trim();

                    if (!int.TryParse(fId, out outParam))
                    {
                        return Json(false);
                    }
                    id = Convert.ToInt32(fId.Trim());

                    fbRepStat.RoleCode = PMGSYSession.Current.RoleCode;
                    // 8 SQC 
                    // 22 PIU


                    var data = (from f in dbContext.ADMIN_FEEDBACK where f.FEED_ID == id select f);

                    fbRepStat.hdnfeedId = PMGSY.Common.URLEncrypt.EncryptParameters1(new string[] { "FeedCode =" + fId.Trim() });//id;
                    fbRepStat.interimList = new List<string>();

                    var q = dbContext.ADMIN_FEEDBACK_REPLY.Where(s => s.FEED_ID == id && s.REP_STATUS == "I").OrderBy(s => s.REP_ID).ToList();



                    if (dbContext.ADMIN_FEEDBACK_REPLY.Any(m => m.PIU_SQC == "P" && m.FEED_ID == id && m.REP_STATUS == "F"))
                    {
                        fbRepStat.isFinalReplyByPIU = "Y";
                    }
                    else
                    {
                        fbRepStat.isFinalReplyByPIU = "N";

                    }

                    //if (dbContext.ADMIN_FEEDBACK_REPLY.Any(m => m.FEED_ID == id && m.REP_STATUS == "F" && m.PIU_SQC != null && m.PIU_SQC == "P"))
                    //{
                    //    fbRepStat.AddButtonShow = "N";
                    //}
                    //else
                    //{
                    //    fbRepStat.AddButtonShow = "N";
                    //}



                    if (q.Count > 0)
                    {
                        foreach (var item in q)
                        {
                            if (item.REP_COMMENT != null)
                            {
                                fbRepStat.interimList.Add(item.REP_COMMENT.Trim());
                            }
                            else
                            {
                                fbRepStat.interimList.Add(item.REP_COMMENT);
                            }
                        }
                    }

                    foreach (var itm in data)
                    {
                        if (itm.FEED_EMAIL != null)
                        {
                            fbRepStat.ToAdd = itm.FEED_EMAIL.Trim();
                        }
                        else if (itm.FEED_TELE_CODE != null && itm.FEED_TELE_NUMBER != null)
                        {
                            if (itm.FEED_TELE_CODE != null)
                            {
                                fbRepStat.ToAdd = itm.FEED_TELE_CODE.Trim() + "-" + itm.FEED_TELE_NUMBER.Trim();
                            }
                        }
                        else
                        {
                            fbRepStat.ToAdd = itm.FEED_MOBILE;
                        }

                        fbRepStat.Date = comm.GetDateTimeToString((DateTime)itm.FEED_DATE).Trim();
                        fbRepStat.Comment = itm.FEED_COMMENT;
                        fbRepStat.RepStat = itm.FEED_STATUS.Trim();

                        fbRepStat.FBState = itm.MAST_STATE_CODE;

                        fbRepStat.fbapprDate = itm.FEED_APPROVAL_DATE == null ? "" : comm.GetDateTimeToString((DateTime)itm.FEED_APPROVAL_DATE).Trim();
                    }

                    var r = dbContext.ADMIN_FEEDBACK_REPLY.Where(s => s.FEED_ID == id && s.REP_STATUS == "F").ToList();
                    if (r.Count > 0)
                    {
                        foreach (var item in r)
                            fbRepStat.Final = item.REP_STATUS.Trim();
                    }

                    var a = dbContext.ADMIN_FEEDBACK.Where(s => s.FEED_ID == id && s.FEED_APPROVAL == "Y").ToList();
                    if (a.Count > 0)
                    {
                        foreach (var itm in a)
                        {
                            fbRepStat.hdnappr = itm.FEED_APPROVAL.Trim();
                            fbRepStat.hdnstate = Convert.ToString(itm.MAST_STATE_CODE).Trim();
                            fbRepStat.hdndist = Convert.ToString(itm.MAST_DISTRICT_CODE).Trim();
                            fbRepStat.hdnblock = Convert.ToString(itm.MAST_BLOCK_CODE).Trim();

                            fbRepStat.hdnStateCode = Convert.ToInt32(itm.MAST_STATE_CODE);
                            fbRepStat.hdnDistrictCode = Convert.ToInt32(itm.MAST_DISTRICT_CODE);
                            fbRepStat.hdnBlockCode = Convert.ToInt32(itm.MAST_BLOCK_CODE);
                            fbRepStat.IS_PMGSY_ROAD = itm.IS_PMGSY_ROAD == "1" ? true : false;
                        }
                    }
                    else
                    {
                        fbRepStat.hdnappr = "N";
                    }
                }
                var query = dbContext.ADMIN_FEEDBACK_REPLY.Where(x => x.FEED_ID == id).OrderByDescending(x => x.REP_DATE).FirstOrDefault();
                if (query != null)
                {
                    fbRepStat.hdRole = query.UM_User_Master.DefaultRoleID;
                    fbRepStat.hdRepStatus = query.REP_STATUS;
                }

                var feedback = dbContext.ADMIN_FEEDBACK.Where(s => s.FEED_ID == id).ToList();
                if (feedback.Count > 0)
                {
                    foreach (var itm in feedback)
                    {
                        fbRepStat.hdnStateCode = Convert.ToInt32(itm.MAST_STATE_CODE);
                        fbRepStat.hdnDistrictCode = Convert.ToInt32(itm.MAST_DISTRICT_CODE);
                        fbRepStat.hdnBlockCode = Convert.ToInt32(itm.MAST_BLOCK_CODE);
                        fbRepStat.IS_PMGSY_ROAD = itm.IS_PMGSY_ROAD == "1" ? true : false;
                    }
                }

                return PartialView("FBRepStatusNew", fbRepStat);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "FeedbackDetailsController.FBRepStatusNew()");
                return null;
            }
        }


        [HttpPost]
        public ActionResult FBRepStatListNew(FormCollection formCollection)
        {
            Feedback.DAL.FeedbackDAL qualityDAL = new Feedback.DAL.FeedbackDAL();
            int totalRecords;
            try
            {
                using (PMGSY.Common.CommonFunctions commonFunction = new PMGSY.Common.CommonFunctions())
                {
                    if (!commonFunction.ValidateGridParameters(new PMGSY.Common.GridParams(Convert.ToInt32(formCollection["page"]), Convert.ToInt32(formCollection["rows"]), formCollection["sidx"], formCollection["sord"], Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                    {
                        return null;
                    }
                }

                String parameter = null, hash = null, key = null;
                string[] encoded = Convert.ToString(formCollection["id"]).Trim().Split('/');
                decryptedParameters = PMGSY.Common.URLEncrypt.DecryptParameters1(new String[] { encoded[0], encoded[1], encoded[2] });
                if (decryptedParameters.Count < 0)
                {
                    //totalRecords = 0;
                    return null;
                }
                string fId = decryptedParameters["FeedCode"].ToString();
                var jsonData = new
                {
                    rows = qualityDAL.FBRepListtDALNew(Convert.ToInt32(formCollection["page"]) - 1, Convert.ToInt32(formCollection["rows"]),
                                                            formCollection["sidx"], formCollection["sord"], out totalRecords,
                        /*Convert.ToString(formCollection["id"])*/fId.Trim()),
                    //"1"),
                    total = totalRecords <= Convert.ToInt32(formCollection["rows"]) ? 1 : (totalRecords % Convert.ToInt32(formCollection["rows"]) == 0 ? totalRecords / Convert.ToInt32(formCollection["rows"]) : (totalRecords / Convert.ToInt32(formCollection["rows"]) + 1)),
                    page = Convert.ToInt32(formCollection["page"]),
                    records = totalRecords
                };
                return Json(jsonData, JsonRequestBehavior.AllowGet);
                //return null;
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "FeedbackDetailsController.FBRepStatListNew()");
                //Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return Json(String.Empty);
            }
        }

        public ActionResult FeedbackReplyUpdateNew(String parameter, String hash, String key/*string fId*/)
        {
            try
            {
                PMGSY.Models.Feedback.FeedbackReply FBRep = new Models.Feedback.FeedbackReply();
                decryptedParameters = PMGSY.Common.URLEncrypt.DecryptParameters1(new String[] { parameter, hash, key });
                if (decryptedParameters.Count > 0)
                {
                    string fId = Convert.ToString(decryptedParameters["FeedCode"]).Trim();
                    string[] ID = fId.Split('$');

                    if (!int.TryParse(ID[0], out outParam))
                    {
                        return Json(false);
                    }
                    if (!int.TryParse(ID[1], out outParam))
                    {
                        return Json(false);
                    }
                    int feed_Id = Convert.ToInt32(ID[0].Trim());
                    int rep_Id = Convert.ToInt32(ID[1].Trim());

                    dbContext = new Models.PMGSYEntities();

                    var a = dbContext.ADMIN_FEEDBACK.Where(m => m.FEED_ID == feed_Id).OrderBy(s => s.FEED_ID).ToList();
                    foreach (var itm in a)
                    {
                        if (itm.MAST_STATE_CODE != null)
                        {
                            FBRep.hdnRepStateCode = (int)itm.MAST_STATE_CODE;
                        }
                        else
                        {
                            FBRep.hdnRepStateCode = 0;
                        }
                        FBRep.hdnRepDistrictCode = itm.MAST_DISTRICT_CODE.HasValue ? itm.MAST_STATE_CODE.Value : 0;
                    }

                    string Rep_Stat = string.Empty;
                    var q = dbContext.ADMIN_FEEDBACK_REPLY.Where(m => m.FEED_ID == feed_Id && m.REP_ID == rep_Id).OrderBy(s => s.REP_ID).ToList();
                    foreach (var item in q)
                    {
                        Rep_Stat = item.REP_STATUS.Trim();
                        FBRep.Rep_Comments = item.REP_COMMENT;
                        FBRep.hdnRole = item.UM_User_Master.DefaultRoleID;
                    }

                    FBRep.hdnDBOpr = "M";

                    FBRep.hdnRepStatus = Rep_Stat.Trim();
                    FBRep.hdnFBId = PMGSY.Common.URLEncrypt.EncryptParameters1(new string[] { "FeedCode =" + feed_Id });//Convert.ToString(feed_Id).Trim();
                    FBRep.hdnRepId = rep_Id;
                }
                return View("FeedbackReplyNew", FBRep);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "FeedbackDetailsController.FeedbackReplyNew()");
                //Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return null;
                // return Json(String.Empty);
            }
        }


        public ActionResult FeedbackReplyNew(String parameter, String hash, String key/*string fId*/)
        {
            PMGSY.Models.Feedback.FeedbackReply FBRep = new Models.Feedback.FeedbackReply();
            decryptedParameters = PMGSY.Common.URLEncrypt.DecryptParameters1(new String[] { parameter, hash, key });
            if (decryptedParameters.Count > 0)
            {
                string fId = Convert.ToString(decryptedParameters["FeedCode"]).Trim();

                if (!int.TryParse(fId, out outParam))
                {
                    return Json(false);
                }
                //fId = "1";
                int feedId = Convert.ToInt32(fId.Trim());


                dbContext = new Models.PMGSYEntities();

                var a = dbContext.ADMIN_FEEDBACK.Where(m => m.FEED_ID == feedId).OrderBy(s => s.FEED_ID).ToList();
                foreach (var itm in a)
                {
                    if (itm.MAST_STATE_CODE != null)
                    {
                        FBRep.hdnRepStateCode = (int)itm.MAST_STATE_CODE;
                    }
                    else
                    {
                        FBRep.hdnRepStateCode = 0;
                    }
                    FBRep.hdnRepDistrictCode = itm.MAST_DISTRICT_CODE.HasValue ? itm.MAST_DISTRICT_CODE.Value : 0;
                    FBRep.hdnStateType = itm.MAST_STATE_CODE > 0 ? itm.MASTER_STATE.MAST_STATE_UT.Trim() : null;
                }

                string Rep_Stat = string.Empty;
                var q = dbContext.ADMIN_FEEDBACK_REPLY.Where(m => m.FEED_ID == feedId).OrderBy(s => s.REP_ID).ToList();
                foreach (var item in q)
                {
                    Rep_Stat = item.REP_STATUS.Trim();
                    FBRep.hdnRole = item.UM_User_Master.DefaultRoleID;
                }

                FBRep.hdnRepStatus = Rep_Stat.Trim();
                FBRep.hdnFBId = PMGSY.Common.URLEncrypt.EncryptParameters1(new string[] { "FeedCode =" + fId.Trim() });//Convert.ToInt32(fId);

                FBRep.hdnDBOpr = "I";
            }
            return View("FeedbackReplyNew", FBRep);
        }


        public JsonResult saveFeedBackRepNew(PMGSY.Models.Feedback.FeedbackReply fbRep)
        {
            try
            {
                if (fbRep.Feed_Reply.Equals("F"))
                {
                    ModelState.Remove("TIMELINE_DATE");
                }

                //if (PMGSYSession.Current.RoleCode != 22)
                if (!(PMGSYSession.Current.RoleCode == 22 || PMGSYSession.Current.RoleCode == 38 || PMGSYSession.Current.RoleCode == 54))

                {
                    ModelState.Remove("TIMELINE_DATE");
                }


                if (ModelState.IsValid)
                {
                    string[] encoded = fbRep.hdnFBId.Trim().Split('/');
                    decryptedParameters = PMGSY.Common.URLEncrypt.DecryptParameters1(new String[] { encoded[0], encoded[1], encoded[2] });
                    if (decryptedParameters.Count < 0)
                    {
                        //totalRecords = 0;
                        return Json(String.Empty);
                    }
                    //bool flag = fbDAL.SaveFBRep(fbRep, Convert.ToString(fbRep.hdnFBId));
                    bool flag = fbDAL.SaveFBRepNew(fbRep, Convert.ToString(decryptedParameters["FeedCode"]));

                    return Json(new { status = flag });
                }
                else
                {
                    return Json(string.Empty);
                }
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "FeedbackDetailsController.saveFeedBackRepNew()");
                // Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return null;
            }
        }

        public JsonResult updateFeedBackRepNew(PMGSY.Models.Feedback.FeedbackReply fbRep)
        {
            try
            {

                if (ModelState.IsValid)
                {
                    string[] encoded = fbRep.hdnFBId.Trim().Split('/');
                    decryptedParameters = PMGSY.Common.URLEncrypt.DecryptParameters1(new String[] { encoded[0], encoded[1], encoded[2] });
                    if (decryptedParameters.Count < 0)
                    {
                        //totalRecords = 0;
                        return Json(String.Empty);
                    }
                    //bool flag = fbDAL.UpdateFBRep(fbRep, fbRep.hdnFBId, fbRep.hdnRepId);
                    bool flag = fbDAL.UpdateFBRepNew(fbRep, Convert.ToString(decryptedParameters["FeedCode"]).Trim(), fbRep.hdnRepId);

                    return Json(new { status = flag });
                }
                else
                {
                    return Json(string.Empty);
                }
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "FeedbackDetailsController.updateFeedBackRepNew()");

                //  Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return null;
            }
        }

        public JsonResult deleteFeedBackRepNew(String parameter, String hash, String key/*string id*/)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    bool flag = false;
                    decryptedParameters = PMGSY.Common.URLEncrypt.DecryptParameters1(new String[] { parameter, hash, key });
                    if (decryptedParameters.Count > 0)
                    {
                        string id = Convert.ToString(decryptedParameters["FeedCode"]).Trim();
                        string[] fId = id.Trim().Split('$');

                        if (!int.TryParse(fId[0], out outParam))
                        {
                            return Json(false);
                        }
                        if (!int.TryParse(fId[1], out outParam))
                        {
                            return Json(false);
                        }

                        int feedId = Convert.ToInt32(fId[0].Trim());
                        int repId = Convert.ToInt32(fId[1].Trim());

                        flag = fbDAL.DeleteFBRepNew(feedId, repId);

                    }
                    return Json(new { status = flag }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    string message = ModelState.Values.Where(x => x.Errors.Count == 1).FirstOrDefault().Errors[0].ErrorMessage;
                    return Json(string.Empty);
                }
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "FeedbackDetailsController.deleteFeedBackRepNew()");
                //  Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return null;
            }
        }

        public JsonResult ForwardToSQC(String parameter, String hash, String key/*string id*/)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    bool flag = false;

                    decryptedParameters = PMGSY.Common.URLEncrypt.DecryptParameters1(new String[] { parameter, hash, key });
                    if (decryptedParameters.Count > 0)
                    {
                        string id = Convert.ToString(decryptedParameters["FeedCode"]).Trim();
                        string[] fId = id.Trim().Split('$');

                        if (!int.TryParse(fId[0], out outParam))
                        {
                            return Json(false);
                        }
                        if (!int.TryParse(fId[1], out outParam))
                        {
                            return Json(false);
                        }

                        int feedId = Convert.ToInt32(fId[0].Trim());
                        int repId = Convert.ToInt32(fId[1].Trim());



                        flag = fbDAL.ForwardToSQCDAL(feedId, repId);

                    }
                    return Json(new { status = flag }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    string message = ModelState.Values.Where(x => x.Errors.Count == 1).FirstOrDefault().Errors[0].ErrorMessage;
                    return Json(string.Empty);
                }
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "FeedbackDetailsController.ForwardToSQC()");

                // Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return null;
            }
        }


        #endregion

        #region Image Upload PIU
        //1
        [HttpGet]
        public ActionResult ImageUploadByPIU(String parameter, String hash, String key)
        {
            try
            {
                PMGSY.Models.PMGSYEntities dbContext = new PMGSY.Models.PMGSYEntities();
                FileUploadViewModel fileUploadViewModel = new FileUploadViewModel();
                int feedId = 0;
                int repId = 0;

                bool flag = false;
                decryptedParameters = PMGSY.Common.URLEncrypt.DecryptParameters1(new String[] { parameter, hash, key });
                if (decryptedParameters.Count > 0)
                {
                    string id = Convert.ToString(decryptedParameters["FeedCode"]).Trim();
                    string[] fId = id.Trim().Split('$');

                    if (!int.TryParse(fId[0], out outParam))
                    {
                        return Json(false);
                    }
                    if (!int.TryParse(fId[1], out outParam))
                    {
                        return Json(false);
                    }

                    feedId = Convert.ToInt32(fId[0].Trim());
                    repId = Convert.ToInt32(fId[1].Trim());


                }

                fileUploadViewModel.FEED_ID = feedId;
                fileUploadViewModel.REP_ID = repId;
                fileUploadViewModel.FinalizedByPIU = dbContext.ADMIN_FEEDBACK_REPLY.Where(m => m.IS_FORWARD_TO_SQC == "Y" && m.IS_FORWARD_TO_SQC != null && m.FEED_ID == feedId && m.REP_ID == repId).Select(m => m.IS_FORWARD_TO_SQC).FirstOrDefault();
                fileUploadViewModel.FILE_ID = dbContext.ADMIN_FEEDBACK_REPLY_FILES.Where(m => m.FEED_ID == feedId && m.REP_ID == repId).Select(m => m.FILE_ID).FirstOrDefault();

                // File ID gete it
                if (string.IsNullOrEmpty(fileUploadViewModel.FinalizedByPIU))
                {
                    fileUploadViewModel.FinalizedByPIU = "N";
                }

                if (dbContext.ADMIN_FEEDBACK_REPLY_FILES.Where(m => m.FEED_ID == feedId && m.REP_ID == repId).Any())
                {
                    string FileName = dbContext.ADMIN_FEEDBACK_REPLY_FILES.Where(m => m.FEED_ID == feedId && m.REP_ID == repId).Select(m => m.FILE_NAME).FirstOrDefault().ToString();

                    if (FileName.Equals("NA"))
                    {
                        fileUploadViewModel.NumberofFiles = 0;
                    }
                    else if (String.IsNullOrEmpty(FileName))
                    {
                        fileUploadViewModel.NumberofFiles = 0;
                    }
                    else
                    {
                        fileUploadViewModel.NumberofFiles = 1;
                    }
                }
                else
                {
                    fileUploadViewModel.NumberofFiles = 0;
                }


                return View("ImageUploadByPIU", fileUploadViewModel);
                //  return View("ImageUploadByPIU", fileUploadViewModel);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "FeedbbackDetailsController.ImageUploadByPIU()");
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


        //2
        [HttpPost]
        public JsonResult ListFilesByPIU(FormCollection formCollection)
        {
            // IExecutionBAL objExecutionBAL = new ExecutionBAL();
            try
            {

                using (CommonFunctions commonFunction = new CommonFunctions())
                {
                    if (!commonFunction.ValidateGridParameters(new GridParams(Convert.ToInt32(formCollection["page"]), Convert.ToInt32(formCollection["rows"]), formCollection["sidx"], formCollection["sord"], Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                    {
                        return null;
                    }
                }
                Int32 FEED_ID = Convert.ToInt32(Request.Params["FEED_ID"]);
                Int32 REP_ID = Convert.ToInt32(Request.Params["REP_ID"]);

                int totalRecords;
                var jsonData = new
                {
                    rows = fbDAL.GetFilesListDALByPIU(Convert.ToInt32(formCollection["page"]) - 1, Convert.ToInt32(formCollection["rows"]), formCollection["sidx"], formCollection["sord"], out totalRecords, FEED_ID, REP_ID),
                    total = totalRecords <= Convert.ToInt32(formCollection["rows"]) ? 1 : (totalRecords % Convert.ToInt32(formCollection["rows"]) == 0 ? totalRecords / Convert.ToInt32(formCollection["rows"]) : (totalRecords / Convert.ToInt32(formCollection["rows"]) + 1)),
                    page = Convert.ToInt32(formCollection["page"]),
                    records = totalRecords
                };
                return Json(jsonData);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "FeedbackDetails.ListFilesByPIU()");
                return null;
            }

        }


        //3 
        [HttpPost]
        public ActionResult UploadsByPIU(PMGSY.Models.Feedback.FileUploadViewModel fileUploadViewModel)
        {
            try
            {
                ModelState.Remove("PdfDescription");
                CommonFunctions objCommonFunc = new CommonFunctions();
                // HttpRequestBase Request;
                String[] fileTypes = new String[] { "jpeg", String.Empty, String.Empty };
                String StorageRoot = string.Empty;
                StorageRoot = System.Configuration.ConfigurationManager.AppSettings["MERI_SADAK_FEEDBACK_REPLY_FILE_IMAGE_PHYSICAL_DIR_PATH"];

                if (!(objCommonFunc.IsValidImageFile(StorageRoot, Request, fileTypes)))
                {
                    fileUploadViewModel.ErrorMessage = "File Type is Not Allowed.";
                    return View("ImageUploadByPIU", fileUploadViewModel.ErrorMessage);
                }


                foreach (string file in Request.Files)
                {
                    string status = ValidateImageFileofProgress(Request.Files[0].ContentLength, System.IO.Path.GetExtension(Request.Files[0].FileName));
                    if (status != string.Empty)
                    {
                        fileUploadViewModel.ErrorMessage = status;
                        return View("ImageUploadByPIU", fileUploadViewModel.ErrorMessage);
                    }
                }

                var fileData = new List<PMGSY.Models.Feedback.FileUploadViewModel>();

                //
                foreach (string file in Request.Files)
                {
                    UploadImageFileByPIU(Request, fileData, fileUploadViewModel.FEED_ID, fileUploadViewModel.REP_ID);
                }
                var serializer = new System.Web.Script.Serialization.JavaScriptSerializer();
                serializer.MaxJsonLength = Int32.MaxValue;

                var result = new ContentResult
                {
                    Content = "{\"files\":" + serializer.Serialize(fileData) + "}",
                };
                return result;
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "FeedbackDetails.UploadsByPIU()");
                return null;
            }
        }



        // 4
        [HttpPost]
        public void UploadImageFileByPIU(HttpRequestBase request, List<PMGSY.Models.Feedback.FileUploadViewModel> statuses, int FEED_ID, int REP_ID)
        {
            dbContext = new PMGSYEntities();
            // IExecutionBAL objExecutionBAL = new ExecutionBAL();
            String StorageRoot = string.Empty;
            ModelState.Remove("PdfDescription");



            try
            {
                string errorMessage = string.Empty;
                string lati = string.Empty;
                string longi = string.Empty;

                for (int i = 0; i < request.Files.Count; i++)
                {
                    HttpPostedFileBase file = request.Files[i];
                    // var fileName = IMS_PR_ROAD_CODE + System.IO.Path.GetExtension(request.Files[i].FileName).ToString();

                    //#region LAT_LONG_CALCULATION

                    //Double[] latitudeRef = null;
                    //Double[] longitudeRef = null;
                    //StringBuilder strLat = new StringBuilder();
                    //StringBuilder strLong = new StringBuilder();

                    //using (ExifReader reader = new ExifReader(file.InputStream, true))
                    //{
                    //    reader.GetTagValue<Double[]>(ExifTags.GPSLatitude, out latitudeRef);
                    //    reader.GetTagValue<Double[]>(ExifTags.GPSLongitude, out longitudeRef);

                    //    if (latitudeRef != null && longitudeRef != null)
                    //    {
                    //        //return Json(new { success = true, latitude = latitude, longitude = longitude });

                    //        for (int value = 0; value < latitudeRef.Count(); value++)
                    //        {
                    //            if (value == 0)
                    //            {
                    //                strLat.Append(latitudeRef[value].ToString() + ".");
                    //            }
                    //            else
                    //            {
                    //                if (latitudeRef[value].ToString().Contains("."))
                    //                {
                    //                    strLat.Append(latitudeRef[value].ToString().Replace(".", string.Empty).ToString());
                    //                }
                    //                else
                    //                {
                    //                    strLat.Append(latitudeRef[value].ToString());
                    //                }
                    //            }
                    //        }

                    //        for (int value = 0; value < longitudeRef.Count(); value++)
                    //        {
                    //            if (value == 0)
                    //            {
                    //                strLong.Append(longitudeRef[value].ToString() + ".");
                    //            }
                    //            else
                    //            {
                    //                if (longitudeRef[value].ToString().Contains("."))
                    //                {
                    //                    strLong.Append(longitudeRef[value].ToString().Replace(".", string.Empty).ToString());
                    //                }
                    //                else
                    //                {
                    //                    strLong.Append(longitudeRef[value].ToString());
                    //                }
                    //            }
                    //        }
                    //    }
                    //    else
                    //    {
                    //        errorMessage = "Image does not contain the Geo location information.";
                    //        //return Json(new { success = false, message = "Image does not contain the Geo location information." });
                    //    }
                    //}
                    //#endregion


                    #region LAT_LONG_CALCULATION

                    Double[] latitudeRef = null;
                    Double[] longitudeRef = null;
                    StringBuilder strLat = new StringBuilder();
                    StringBuilder strLong = new StringBuilder();

                    using (ExifReader reader = new ExifReader(file.InputStream, true))
                    {
                        reader.GetTagValue<Double[]>(ExifTags.GPSLatitude, out latitudeRef);
                        reader.GetTagValue<Double[]>(ExifTags.GPSLongitude, out longitudeRef);

                        if (latitudeRef != null && longitudeRef != null)
                        {
                            //return Json(new { success = true, latitude = latitude, longitude = longitude });

                            for (int value = 0; value < latitudeRef.Count(); value++)
                            {
                                if (value == 0)
                                {
                                    strLat.Append(latitudeRef[value].ToString() + ".");
                                }
                                else
                                {
                                    if (latitudeRef[value].ToString().Contains("."))
                                    {
                                        strLat.Append(latitudeRef[value].ToString().Replace(".", string.Empty).ToString());
                                    }
                                    else
                                    {
                                        strLat.Append(latitudeRef[value].ToString());
                                    }

                                }
                            }


                            for (int value = 0; value < longitudeRef.Count(); value++)
                            {
                                if (value == 0)
                                {
                                    strLong.Append(longitudeRef[value].ToString() + ".");
                                }
                                else
                                {
                                    if (longitudeRef[value].ToString().Contains("."))
                                    {
                                        strLong.Append(longitudeRef[value].ToString().Replace(".", string.Empty).ToString());
                                    }
                                    else
                                    {
                                        strLong.Append(longitudeRef[value].ToString());
                                    }
                                }
                            }
                        }
                    }

                    #endregion



                    var timestampArray = System.DateTime.Now.ToString("MM/dd/yyyy hh:mm:ss.fff").Replace('.', '_').Split(' ');
                    string timestamp = timestampArray[0].ToString() + "_" + timestampArray[1].ToString();
                    var fileName = "IMG_" + timestamp.Replace('/', '_').Replace(':', '_') + "_" + FEED_ID.ToString() + "_" + REP_ID.ToString() + System.IO.Path.GetExtension(request.Files[i].FileName).ToString();

                    StorageRoot = System.Configuration.ConfigurationManager.AppSettings["MERI_SADAK_FEEDBACK_REPLY_FILE_IMAGE_PHYSICAL_DIR_PATH"];
                    var fullPath = System.IO.Path.Combine(StorageRoot, fileName);

                    var ThumbnailPath = System.IO.Path.Combine(StorageRoot, "thumbnails");
                    var FullThumbnailPath = System.IO.Path.Combine(ThumbnailPath, fileName);

                    if (!(System.IO.Directory.Exists(ThumbnailPath)))
                    {
                        System.IO.Directory.CreateDirectory(ThumbnailPath);
                    }

                    statuses.Add(new PMGSY.Models.Feedback.FileUploadViewModel()
                    {
                        url = fullPath,
                        thumbnail_url = fullPath,
                        name = fileName,
                        type = file.ContentType,
                        size = file.ContentLength,
                        delete_url = "",
                        delete_type = "DELETE",
                        Image_Description = request.Params["remark[]"],  //.Split(',')[i]
                        FEED_ID = FEED_ID,
                        REP_ID = REP_ID,
                        Latitude = Convert.ToDecimal(strLat.ToString()),
                        Longitude = Convert.ToDecimal(strLong.ToString()),


                    });
                    string status = fbDAL.AddFileUploadDetailsBALByPIUDAL(statuses, FEED_ID, REP_ID, fileName, request.Params["remark[]"]);

                    if (status == string.Empty)
                    {
                        //if (fileName.Contains("pdf") || fileName.Contains("xls"))
                        //{
                        HttpPostedFileBase postBasefile = request.Files[0];
                        postBasefile.SaveAs(Path.Combine(StorageRoot, fileName));

                        //   }
                        //else
                        //{
                        new PMGSY.BAL.Proposal.ProposalBAL().CompressImage(request.Files[0], fullPath, FullThumbnailPath);
                        //CompressImage(request.Files[0], fullPath, FullThumbnailPath);
                        // }
                    }
                    else
                    {
                        // show an error over here
                    }
                    // Commented on 26 Nov 2020

                    //if (status == string.Empty)
                    //{
                    //    new PMGSY.BAL.Proposal.ProposalBAL().CompressImage(request.Files[0], fullPath, FullThumbnailPath);
                    //}
                    //else
                    //{
                    //    // show an error over here
                    //}
                }
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "FeedbackDetails.UploadImageFileByPIU()");
            }
            finally
            {
                if (dbContext != null)
                {
                    dbContext.Dispose();
                }
            }
        }

        // 5
        public string ValidateImageFileofProgress(int FileSize, string FileExtension)
        {
            try
            {
                string ValidExtensions = System.Configuration.ConfigurationManager.AppSettings["MERI_SADAK_FEEDBACK_REPLY_FILE_IMAGE_VALID_FORMAT"];
                string[] arrValidFormats = ValidExtensions.Split('$');


                if (!arrValidFormats.Contains(FileExtension.ToLower()))
                {
                    return "File is not Valid Image File";
                }
                if (FileSize > Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["MERI_SADAK_FEEDBACK_REPLY_FILE_IMAGE_FILE_MAX_SIZE"]))
                {
                    return "File Size Exceed the Maximum File Limit";
                }

                return string.Empty;
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "FeedbackDetails.ValidateImageFileofProgress()");
                return null;
            }
        }

        //6
        [HttpGet]
        public ActionResult DownloadFileUploadedByPIU(String parameter, String hash, String key)
        {
            string FileName = string.Empty;
            string FullFileLogicalPathName = string.Empty;
            string FullfilePhysicalPathName = string.Empty;
            string FullFileLogicalPathId = string.Empty;
            string FullfilePhysicalPathId = string.Empty;
            string FileExtension = string.Empty;
            Int32 FileID = 0;
            dbContext = new PMGSYEntities();
            try
            {
                if (!String.IsNullOrEmpty(parameter) && !String.IsNullOrEmpty(hash) && !String.IsNullOrEmpty(key))
                {
                    String[] urlParams = URLEncrypt.DecryptParameters(new String[] { parameter, hash, key });
                    if (urlParams.Length >= 1)
                    {
                        String[] urlSplitParams = urlParams[0].Split('$');
                        FileName = (urlSplitParams[0]);
                        FileID = Convert.ToInt32(urlSplitParams[1]);
                    }
                }
                FileExtension = System.IO.Path.GetExtension(FileName).ToLower();

                if (FileExtension == ".jpg" || FileExtension == ".jpeg" || FileExtension == ".bmp" || FileExtension == ".png" || FileExtension == ".gif" || FileExtension == ".tiff")
                {
                    FullFileLogicalPathName = System.IO.Path.Combine(System.Configuration.ConfigurationManager.AppSettings["MERI_SADAK_FEEDBACK_REPLY_FILE_IMAGE_VIRTUAL_DIR_PATH"], FileName);
                    FullfilePhysicalPathName = System.IO.Path.Combine(System.Configuration.ConfigurationManager.AppSettings["MERI_SADAK_FEEDBACK_REPLY_FILE_IMAGE_PHYSICAL_DIR_PATH"], FileName);

                }

                string name = System.IO.Path.GetFileName(FileName);
                string ext = System.IO.Path.GetExtension(FileName);

                string type = string.Empty;

                if (ext != null)
                {
                    switch (ext.ToLower())
                    {
                        case ".pdf":
                            type = "Application/pdf";
                            break;
                        case ".doc":
                        case ".docx":
                            type = "Application/msword";
                            break;
                        case ".jpg":
                        case ".bmp":
                        case ".tiff":
                        case ".png":
                        case ".gif":
                        case ".jpeg":
                            type = "image/png";
                            break;
                        default:
                            type = "Application";
                            break;
                    }
                }

                if (System.IO.File.Exists(FullfilePhysicalPathName))
                {
                    return File(FullfilePhysicalPathName, type, DateTime.Now.ToShortDateString() + "_" + new Random().Next(1000000000) + FileExtension);
                }
                else if (System.IO.File.Exists(FullfilePhysicalPathId))
                {
                    return File(FullfilePhysicalPathId, type, DateTime.Now.ToShortDateString() + "_" + new Random().Next(1000000000) + FileExtension);
                }
                else
                {
                    return Json(new { Error = "File Not Exist" }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "FeedbackDetailsController.DownloadFileUploadedByPIU()");
                return Json(new { Success = "false" }, JsonRequestBehavior.AllowGet);
            }
            finally
            {
                if (dbContext != null)
                {
                    dbContext.Dispose();
                }
            }
        }


        // 7
        [HttpPost]
        public JsonResult DeleteFileDetailsByPIU(String parameter, String hash, String key)
        {
            String PhysicalPath = string.Empty;
            String ThumbnailPath = string.Empty;
            dbContext = new PMGSYEntities();


            try
            {
                String[] urlParams = URLEncrypt.DecryptParameters(new String[] { parameter, hash, key });

                String[] urlSplitParams = urlParams[0].Split('$');
                string QM_FILE_NAME = (urlSplitParams[0]);
                int FileID = Convert.ToInt32(urlSplitParams[1]); // This is Primary Key

                PhysicalPath = System.Configuration.ConfigurationManager.AppSettings["MERI_SADAK_FEEDBACK_REPLY_FILE_IMAGE_PHYSICAL_DIR_PATH"];
                ThumbnailPath = System.IO.Path.Combine(System.IO.Path.Combine(PhysicalPath, "thumbnails"), QM_FILE_NAME);
                PhysicalPath = System.IO.Path.Combine(PhysicalPath, QM_FILE_NAME);
                if (!System.IO.File.Exists(PhysicalPath) && !System.IO.File.Exists(ThumbnailPath))
                {
                    return Json(new { Success = false, ErrorMessage = "File Not Found." });
                }
                string status = fbDAL.DeleteFileDetailsByPIUDAL(FileID);
                if (status == string.Empty)
                {
                    try
                    {
                        System.IO.File.Delete(PhysicalPath);
                        System.IO.File.Delete(ThumbnailPath);
                    }
                    catch (Exception ex)
                    {
                        Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                        return Json(new { Success = true, ErrorMessage = ex.Message });
                    }
                    return Json(new { Success = true, ErrorMessage = "Error Ocurred While Processing Your Request." });
                }
                else
                {
                    return Json(new { Success = false, ErrorMessage = status });
                }
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "FeedbackDetails.DeleteFileDetailsByPIU()");
                return Json(new { Success = false, ErrorMessage = "Error Ocurred While Processing Your Request." });
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


        #region PDF Upload PIU

        //1
        [HttpGet]
        public ActionResult PDFFileUploadByPIU(String parameter, String hash, String key)
        {
            try
            {
                PMGSY.Models.PMGSYEntities dbContext = new PMGSY.Models.PMGSYEntities();
                FileUploadViewModel fileUploadViewModel = new FileUploadViewModel();
                int feedId = 0;
                int repId = 0;

                bool flag = false;
                decryptedParameters = PMGSY.Common.URLEncrypt.DecryptParameters1(new String[] { parameter, hash, key });
                if (decryptedParameters.Count > 0)
                {
                    string id = Convert.ToString(decryptedParameters["FeedCode"]).Trim();
                    string[] fId = id.Trim().Split('$');

                    if (!int.TryParse(fId[0], out outParam))
                    {
                        return Json(false);
                    }
                    if (!int.TryParse(fId[1], out outParam))
                    {
                        return Json(false);
                    }

                    feedId = Convert.ToInt32(fId[0].Trim());
                    repId = Convert.ToInt32(fId[1].Trim());


                }

                fileUploadViewModel.FEED_ID = feedId;
                fileUploadViewModel.REP_ID = repId;
                fileUploadViewModel.FinalizedByPIU = dbContext.ADMIN_FEEDBACK_REPLY.Where(m => m.IS_FORWARD_TO_SQC == "Y" && m.IS_FORWARD_TO_SQC != null && m.FEED_ID == feedId && m.REP_ID == repId).Select(m => m.IS_FORWARD_TO_SQC).FirstOrDefault();
                fileUploadViewModel.FILE_ID = dbContext.ADMIN_FEEDBACK_REPLY_FILES.Where(m => m.FEED_ID == feedId && m.REP_ID == repId).Select(m => m.FILE_ID).FirstOrDefault();

                // File ID gete it
                if (string.IsNullOrEmpty(fileUploadViewModel.FinalizedByPIU))
                {
                    fileUploadViewModel.FinalizedByPIU = "N";
                }

                if (dbContext.ADMIN_FEEDBACK_REPLY_FILES.Where(m => m.FEED_ID == feedId && m.REP_ID == repId).Any())
                {
                    string FileName = dbContext.ADMIN_FEEDBACK_REPLY_FILES.Where(m => m.FEED_ID == feedId && m.REP_ID == repId).Select(m => m.FILE_NAME).FirstOrDefault().ToString();

                    if (FileName.Equals("NA"))
                    {
                        fileUploadViewModel.NumberofFiles = 0;
                    }
                    else if (String.IsNullOrEmpty(FileName))
                    {
                        fileUploadViewModel.NumberofFiles = 0;
                    }
                    else
                    {
                        fileUploadViewModel.NumberofFiles = 1;
                    }
                }
                else
                {
                    fileUploadViewModel.NumberofFiles = 0;
                }


                return View("PDFFileUploadByPIU", fileUploadViewModel);
                //  return View("ImageUploadByPIU", fileUploadViewModel);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "FeedbbackDetailsController.PDFFileUploadByPIU()");
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



        //2
        [HttpPost]
        public JsonResult ListPDFFilesByPIU(FormCollection formCollection)
        {
            // IExecutionBAL objExecutionBAL = new ExecutionBAL();
            try
            {

                using (CommonFunctions commonFunction = new CommonFunctions())
                {
                    if (!commonFunction.ValidateGridParameters(new GridParams(Convert.ToInt32(formCollection["page"]), Convert.ToInt32(formCollection["rows"]), formCollection["sidx"], formCollection["sord"], Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                    {
                        return null;
                    }
                }
                Int32 FEED_ID = Convert.ToInt32(Request.Params["FEED_ID"]);
                Int32 REP_ID = Convert.ToInt32(Request.Params["REP_ID"]);

                int totalRecords;
                var jsonData = new
                {
                    rows = fbDAL.GetPDFFilesListDALByPIU(Convert.ToInt32(formCollection["page"]) - 1, Convert.ToInt32(formCollection["rows"]), formCollection["sidx"], formCollection["sord"], out totalRecords, FEED_ID, REP_ID),
                    total = totalRecords <= Convert.ToInt32(formCollection["rows"]) ? 1 : (totalRecords % Convert.ToInt32(formCollection["rows"]) == 0 ? totalRecords / Convert.ToInt32(formCollection["rows"]) : (totalRecords / Convert.ToInt32(formCollection["rows"]) + 1)),
                    page = Convert.ToInt32(formCollection["page"]),
                    records = totalRecords
                };
                return Json(jsonData);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "FeedbackDetails.ListPDFFilesByPIU()");
                return null;
            }

        }




        //3 
        [HttpPost]
        public ActionResult UploadsPDFDetailsByPIU(PMGSY.Models.Feedback.FileUploadViewModel fileUploadViewModel)
        {
            try
            {
                CommonFunctions objCommonFunc = new CommonFunctions();
                String[] fileTypes = new String[] { "pdf" };
                String StorageRoot = string.Empty;
                StorageRoot = System.Configuration.ConfigurationManager.AppSettings["MERI_SADAK_FEEDBACK_REPLY_FILE_PDF_PHYSICAL_DIR_PATH"];
                if (!(objCommonFunc.IsValidImageFileForContractor(ConfigurationManager.AppSettings["MERI_SADAK_FEEDBACK_REPLY_FILE_PDF_PHYSICAL_DIR_PATH"], Request, fileTypes)))
                {
                    fileUploadViewModel.ErrorMessage = "File Type is Not Allowed.";
                    return View("PDFFileUploadByPIU", fileUploadViewModel.ErrorMessage);
                }

                var fileData = new List<PMGSY.Models.Feedback.FileUploadViewModel>();
                foreach (string file in Request.Files)
                {
                    UploadsPDFDetailsByPIUDetails(Request, fileData, fileUploadViewModel.FEED_ID, fileUploadViewModel.REP_ID);
                }
                var serializer = new System.Web.Script.Serialization.JavaScriptSerializer();
                serializer.MaxJsonLength = Int32.MaxValue;

                var result = new ContentResult
                {
                    Content = "{\"files\":" + serializer.Serialize(fileData) + "}",
                };
                return result;
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "FeedbackDetails.UploadsPDFDetailsByPIU()");
                return null;
            }
        }


        // 4
        [HttpPost]
        public void UploadsPDFDetailsByPIUDetails(HttpRequestBase request, List<PMGSY.Models.Feedback.FileUploadViewModel> statuses, int FEED_ID, int REP_ID)
        {
            dbContext = new PMGSYEntities();
            // IExecutionBAL objExecutionBAL = new ExecutionBAL();
            String StorageRoot = string.Empty;
            ModelState.Remove("PdfDescription");
            try
            {
                for (int i = 0; i < request.Files.Count; i++)
                {
                    HttpPostedFileBase file = request.Files[i];

                    var timestampArray = System.DateTime.Now.ToString("MM/dd/yyyy hh:mm:ss.fff").Replace('.', '_').Split(' ');
                    string timestamp = timestampArray[0].ToString() + "_" + timestampArray[1].ToString();
                    var fileName = "PDF_" + timestamp.Replace('/', '_').Replace(':', '_') + "_" + FEED_ID.ToString() + "_" + REP_ID.ToString() + System.IO.Path.GetExtension(request.Files[i].FileName).ToString();
                    StorageRoot = System.Configuration.ConfigurationManager.AppSettings["MERI_SADAK_FEEDBACK_REPLY_FILE_PDF_PHYSICAL_DIR_PATH"];
                    var fullPath = System.IO.Path.Combine(StorageRoot, fileName);

                    var ThumbnailPath = System.IO.Path.Combine(StorageRoot, "thumbnails");
                    var FullThumbnailPath = System.IO.Path.Combine(ThumbnailPath, fileName);

                    if (!(System.IO.Directory.Exists(ThumbnailPath)))
                    {
                        System.IO.Directory.CreateDirectory(ThumbnailPath);
                    }

                    statuses.Add(new PMGSY.Models.Feedback.FileUploadViewModel()
                    {
                        url = fullPath,
                        thumbnail_url = fullPath,
                        name = fileName,
                        type = file.ContentType,
                        size = file.ContentLength,
                        delete_url = "",
                        delete_type = "DELETE",
                        Image_Description = request.Params["remark[]"],  //.Split(',')[i]
                        FEED_ID = FEED_ID,
                        REP_ID = REP_ID

                    });
                    string status = fbDAL.AddPDFFileUploadDetailsBALByPIUDAL(FEED_ID, REP_ID, fileName, request.Params["remark[]"]);

                    if (status == string.Empty)
                    {
                        if (fileName.Contains("pdf") || fileName.Contains("xls"))
                        {
                            HttpPostedFileBase postBasefile = request.Files[0];
                            postBasefile.SaveAs(Path.Combine(StorageRoot, fileName));

                        }
                        else
                        {
                            CompressPDF(request.Files[0], fullPath, FullThumbnailPath);
                        }
                    }
                    else
                    {
                        // show an error over here
                    }

                }
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "FeedbackDetails.UploadImageFileByPIU()");
            }
            finally
            {
                if (dbContext != null)
                {
                    dbContext.Dispose();
                }
            }
        }


        //5
        public void CompressPDF(HttpPostedFileBase httpPostedFileBase, string DestinitionPath, string ThumbnailPath)
        {
            try
            {
                // For Thumbnail Image    
                ImageResizer.ImageJob ThumbnailJob = new ImageResizer.ImageJob(httpPostedFileBase, ThumbnailPath,
                    new ImageResizer.ResizeSettings("width=100;height=100;format=jpg;mode=max"));

                ThumbnailJob.Build();

                // For Original Image
                ImageResizer.ImageJob job = new ImageResizer.ImageJob(httpPostedFileBase, DestinitionPath,
                    new ImageResizer.ResizeSettings("width=1024;height=768;format=jpg;mode=max"));

                job.Build();
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "FeedbackDetailsController().CompressPDF (PIU case)");
                //ErrorLog.LogError(ex, "Execution.CompressImage()");
            }
        }

        // 6
        [HttpGet]
        public ActionResult DownloadPDFFileUploadedByPIU(String parameter, String hash, String key)
        {
            string FileName = string.Empty;
            string FullFileLogicalPathName = string.Empty;
            string FullfilePhysicalPathName = string.Empty;
            string FullFileLogicalPathId = string.Empty;
            string FullfilePhysicalPathId = string.Empty;
            string FileExtension = string.Empty;
            Int32 FileID = 0;
            dbContext = new PMGSYEntities();
            try
            {
                if (!String.IsNullOrEmpty(parameter) && !String.IsNullOrEmpty(hash) && !String.IsNullOrEmpty(key))
                {
                    String[] urlParams = URLEncrypt.DecryptParameters(new String[] { parameter, hash, key });
                    if (urlParams.Length >= 1)
                    {
                        String[] urlSplitParams = urlParams[0].Split('$');
                        FileName = (urlSplitParams[0]);
                        FileID = Convert.ToInt32(urlSplitParams[1]);
                    }
                }
                FileExtension = System.IO.Path.GetExtension(FileName).ToLower();

                if (FileExtension == ".pdf" || FileExtension == ".jpg" || FileExtension == ".jpeg" || FileExtension == ".bmp" || FileExtension == ".png" || FileExtension == ".gif" || FileExtension == ".tiff")
                {
                    FullFileLogicalPathName = System.IO.Path.Combine(System.Configuration.ConfigurationManager.AppSettings["MERI_SADAK_FEEDBACK_REPLY_FILE_PDF_VIRTUAL_DIR_PATH"], FileName);
                    FullfilePhysicalPathName = System.IO.Path.Combine(System.Configuration.ConfigurationManager.AppSettings["MERI_SADAK_FEEDBACK_REPLY_FILE_PDF_PHYSICAL_DIR_PATH"], FileName);

                }

                string name = System.IO.Path.GetFileName(FileName);
                string ext = System.IO.Path.GetExtension(FileName);

                string type = string.Empty;

                if (ext != null)
                {
                    switch (ext.ToLower())
                    {
                        case ".pdf":
                            type = "Application/pdf";
                            break;
                        case ".doc":
                        case ".docx":
                            type = "Application/msword";
                            break;
                        case ".jpg":
                        case ".bmp":
                        case ".tiff":
                        case ".png":
                        case ".gif":
                        case ".jpeg":
                            type = "image/png";
                            break;
                        default:
                            type = "Application";
                            break;
                    }
                }

                if (System.IO.File.Exists(FullfilePhysicalPathName))
                {
                    return File(FullfilePhysicalPathName, type, DateTime.Now.ToShortDateString() + "_" + new Random().Next(1000000000) + FileExtension);
                }
                else if (System.IO.File.Exists(FullfilePhysicalPathId))
                {
                    return File(FullfilePhysicalPathId, type, DateTime.Now.ToShortDateString() + "_" + new Random().Next(1000000000) + FileExtension);
                }
                else
                {
                    return Json(new { Error = "File Not Exist" }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "FeedbackDetails.DownloadPDFFileUploadedByPIU()");
                return Json(new { Success = "false" }, JsonRequestBehavior.AllowGet);
            }
            finally
            {
                if (dbContext != null)
                {
                    dbContext.Dispose();
                }
            }
        }

        // 7
        [HttpPost]
        public JsonResult DeletePDFFileDetailsByPIU(String parameter, String hash, String key)
        {
            String PhysicalPath = string.Empty;
            String ThumbnailPath = string.Empty;
            dbContext = new PMGSYEntities();

            try
            {
                String[] urlParams = URLEncrypt.DecryptParameters(new String[] { parameter, hash, key });

                String[] urlSplitParams = urlParams[0].Split('$');
                string QM_FILE_NAME = (urlSplitParams[0]);
                int FileID = Convert.ToInt32(urlSplitParams[1]); // This is Primary Key

                PhysicalPath = System.Configuration.ConfigurationManager.AppSettings["MERI_SADAK_FEEDBACK_REPLY_FILE_PDF_PHYSICAL_DIR_PATH"];
                ThumbnailPath = System.IO.Path.Combine(System.IO.Path.Combine(PhysicalPath, "thumbnails"), QM_FILE_NAME);
                PhysicalPath = System.IO.Path.Combine(PhysicalPath, QM_FILE_NAME);
                if (!System.IO.File.Exists(PhysicalPath) && !System.IO.File.Exists(ThumbnailPath))
                {
                    return Json(new { Success = false, ErrorMessage = "File Not Found." });
                }
                string status = fbDAL.DeletePDFFileDetailsByPIUDAL(FileID);
                if (status == string.Empty)
                {
                    try
                    {
                        System.IO.File.Delete(PhysicalPath);
                        System.IO.File.Delete(ThumbnailPath);
                    }
                    catch (Exception ex)
                    {
                        Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                        return Json(new { Success = true, ErrorMessage = ex.Message });
                    }
                    return Json(new { Success = true, ErrorMessage = "Error Ocurred While Processing Your Request." });
                }
                else
                {
                    return Json(new { Success = false, ErrorMessage = status });
                }
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "FeedbackDetails.DeletePDFFileDetailsByPIU()");
                return Json(new { Success = false, ErrorMessage = "Error Ocurred While Processing Your Request." });
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

        public ActionResult SQCFeedbackReplyNew(String parameter, String hash, String key/*string fId*/)
        {
            try
            {
                PMGSY.Models.Feedback.FeedbackReply FBRep = new Models.Feedback.FeedbackReply();
                decryptedParameters = PMGSY.Common.URLEncrypt.DecryptParameters1(new String[] { parameter, hash, key });
                if (decryptedParameters.Count > 0)
                {
                    string fId = Convert.ToString(decryptedParameters["FeedCode"]).Trim();

                    if (!int.TryParse(fId, out outParam))
                    {
                        return Json(false);
                    }
                    //fId = "1";
                    int feedId = Convert.ToInt32(fId.Trim());


                    dbContext = new Models.PMGSYEntities();

                    var a = dbContext.ADMIN_FEEDBACK.Where(m => m.FEED_ID == feedId).OrderBy(s => s.FEED_ID).ToList();
                    foreach (var itm in a)
                    {
                        if (itm.MAST_STATE_CODE != null)
                        {
                            FBRep.hdnRepStateCode = (int)itm.MAST_STATE_CODE;
                        }
                        else
                        {
                            FBRep.hdnRepStateCode = 0;
                        }
                        FBRep.hdnRepDistrictCode = itm.MAST_DISTRICT_CODE.HasValue ? itm.MAST_DISTRICT_CODE.Value : 0;
                        FBRep.hdnStateType = itm.MAST_STATE_CODE > 0 ? itm.MASTER_STATE.MAST_STATE_UT.Trim() : null;
                    }

                    string Rep_Stat = string.Empty;

                    var q = dbContext.ADMIN_FEEDBACK_REPLY.Where(m => m.FEED_ID == feedId && m.PIU_SQC != null && m.PIU_SQC == "S").OrderBy(s => s.REP_ID).ToList();

                    foreach (var item in q)
                    {
                        Rep_Stat = item.REP_STATUS.Trim();
                        FBRep.hdnRole = item.UM_User_Master.DefaultRoleID;
                    }

                    FBRep.hdnRepStatus = Rep_Stat.Trim();
                    FBRep.hdnFBId = PMGSY.Common.URLEncrypt.EncryptParameters1(new string[] { "FeedCode =" + fId.Trim() });//Convert.ToInt32(fId);

                    FBRep.hdnDBOpr = "I";
                }
                return View("SQCFeedbackReplyNew", FBRep);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "FeedbackDetailsController().SQCFeedbackReplyNew");
                return null;
            }
        }



        public JsonResult SQCsaveFeedBackRepNew(PMGSY.Models.Feedback.FeedbackReply fbRep)
        {
            try
            {
                ModelState.Remove("TIMELINE_DATE");

                if (ModelState.IsValid)
                {
                    string[] encoded = fbRep.hdnFBId.Trim().Split('/');
                    decryptedParameters = PMGSY.Common.URLEncrypt.DecryptParameters1(new String[] { encoded[0], encoded[1], encoded[2] });
                    if (decryptedParameters.Count < 0)
                    {
                        //totalRecords = 0;
                        return Json(String.Empty);
                    }
                    //bool flag = fbDAL.SaveFBRep(fbRep, Convert.ToString(fbRep.hdnFBId));
                    bool flag = fbDAL.SQCSaveFBRepNew(fbRep, Convert.ToString(decryptedParameters["FeedCode"]));

                    return Json(new { status = flag });
                }
                else
                {
                    return Json(string.Empty);
                }
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "FeedbackDetailsController().SQCsaveFeedBackRepNew");
                //Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return null;
            }
        }


        public JsonResult SQCupdateFeedBackRepNew(PMGSY.Models.Feedback.FeedbackReply fbRep)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    string[] encoded = fbRep.hdnFBId.Trim().Split('/');
                    decryptedParameters = PMGSY.Common.URLEncrypt.DecryptParameters1(new String[] { encoded[0], encoded[1], encoded[2] });
                    if (decryptedParameters.Count < 0)
                    {
                        //totalRecords = 0;
                        return Json(String.Empty);
                    }
                    //bool flag = fbDAL.UpdateFBRep(fbRep, fbRep.hdnFBId, fbRep.hdnRepId);
                    bool flag = fbDAL.SQCUpdateFBRepNew(fbRep, Convert.ToString(decryptedParameters["FeedCode"]).Trim(), fbRep.hdnRepId);

                    return Json(new { status = flag });
                }
                else
                {
                    return Json(string.Empty);
                }
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "FeedbackDetailsController().SQCsaveFeedBackRepNew");
                //Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return null;
            }
        }
        #endregion



        #region Image Upload SQC
        //1
        [HttpGet]
        public ActionResult ImageUploadBySQC(String parameter, String hash, String key)
        {
            try
            {
                PMGSY.Models.PMGSYEntities dbContext = new PMGSY.Models.PMGSYEntities();
                FileUploadViewModel fileUploadViewModel = new FileUploadViewModel();
                int feedId = 0;
                int repId = 0;

                bool flag = false;
                decryptedParameters = PMGSY.Common.URLEncrypt.DecryptParameters1(new String[] { parameter, hash, key });
                if (decryptedParameters.Count > 0)
                {
                    string id = Convert.ToString(decryptedParameters["FeedCode"]).Trim();
                    string[] fId = id.Trim().Split('$');

                    if (!int.TryParse(fId[0], out outParam))
                    {
                        return Json(false);
                    }
                    if (!int.TryParse(fId[1], out outParam))
                    {
                        return Json(false);
                    }

                    feedId = Convert.ToInt32(fId[0].Trim());
                    repId = Convert.ToInt32(fId[1].Trim());


                }

                fileUploadViewModel.FEED_ID = feedId;
                fileUploadViewModel.REP_ID = repId;
                fileUploadViewModel.FinalizedByPIU = dbContext.ADMIN_FEEDBACK_REPLY.Where(m => m.IS_FORWARD_TO_SQC == "Y" && m.IS_FORWARD_TO_SQC != null && m.FEED_ID == feedId && m.REP_ID == repId).Select(m => m.IS_FORWARD_TO_SQC).FirstOrDefault();
                fileUploadViewModel.FILE_ID = dbContext.ADMIN_FEEDBACK_REPLY_FILES.Where(m => m.FEED_ID == feedId && m.REP_ID == repId).Select(m => m.FILE_ID).FirstOrDefault();

                // File ID gete it


                fileUploadViewModel.FinalizedBySQC = dbContext.ADMIN_FEEDBACK_REPLY.Where(m => m.FEED_ID == feedId && m.REP_ID == repId).Select(m => m.SQC_FINALIZED).FirstOrDefault();



                if (PMGSYSession.Current.RoleCode == 22)
                {
                    fileUploadViewModel.FinalizedByPIU = "Y";


                }
                else
                {

                    fileUploadViewModel.FinalizedByPIU = "N";
                }

                //if (string.IsNullOrEmpty(fileUploadViewModel.FinalizedByPIU))
                //{

                //}




                if (dbContext.ADMIN_FEEDBACK_REPLY_FILES.Where(m => m.FEED_ID == feedId && m.REP_ID == repId).Any())
                {
                    string FileName = dbContext.ADMIN_FEEDBACK_REPLY_FILES.Where(m => m.FEED_ID == feedId && m.REP_ID == repId).Select(m => m.FILE_NAME).FirstOrDefault().ToString();

                    if (FileName.Equals("NA"))
                    {
                        fileUploadViewModel.NumberofFiles = 0;
                    }
                    else if (String.IsNullOrEmpty(FileName))
                    {
                        fileUploadViewModel.NumberofFiles = 0;
                    }
                    else
                    {
                        fileUploadViewModel.NumberofFiles = 1;
                    }
                }
                else
                {
                    fileUploadViewModel.NumberofFiles = 0;
                }


                return View("ImageUploadBySQC", fileUploadViewModel);
                //  return View("ImageUploadByPIU", fileUploadViewModel);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "FeedbbackDetailsController.ImageUploadBySQC()");
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


        //2
        [HttpPost]
        public JsonResult ListFilesBySQC(FormCollection formCollection)
        {
            // IExecutionBAL objExecutionBAL = new ExecutionBAL();
            try
            {

                using (CommonFunctions commonFunction = new CommonFunctions())
                {
                    if (!commonFunction.ValidateGridParameters(new GridParams(Convert.ToInt32(formCollection["page"]), Convert.ToInt32(formCollection["rows"]), formCollection["sidx"], formCollection["sord"], Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                    {
                        return null;
                    }
                }
                Int32 FEED_ID = Convert.ToInt32(Request.Params["FEED_ID"]);
                Int32 REP_ID = Convert.ToInt32(Request.Params["REP_ID"]);

                int totalRecords;
                var jsonData = new
                {
                    rows = fbDAL.GetFilesListDALBySQC(Convert.ToInt32(formCollection["page"]) - 1, Convert.ToInt32(formCollection["rows"]), formCollection["sidx"], formCollection["sord"], out totalRecords, FEED_ID, REP_ID),
                    total = totalRecords <= Convert.ToInt32(formCollection["rows"]) ? 1 : (totalRecords % Convert.ToInt32(formCollection["rows"]) == 0 ? totalRecords / Convert.ToInt32(formCollection["rows"]) : (totalRecords / Convert.ToInt32(formCollection["rows"]) + 1)),
                    page = Convert.ToInt32(formCollection["page"]),
                    records = totalRecords
                };
                return Json(jsonData);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "FeedbackDetails.ListFilesBySQC()");
                return null;
            }

        }


        //3 
        [HttpPost]
        public ActionResult UploadsBySQC(PMGSY.Models.Feedback.FileUploadViewModel fileUploadViewModel)
        {
            try
            {
                ModelState.Remove("PdfDescription");
                CommonFunctions objCommonFunc = new CommonFunctions();
                // HttpRequestBase Request;
                String[] fileTypes = new String[] { "jpeg", String.Empty, String.Empty };
                String StorageRoot = string.Empty;
                StorageRoot = System.Configuration.ConfigurationManager.AppSettings["MERI_SADAK_FEEDBACK_REPLY_FILE_IMAGE_PHYSICAL_DIR_PATH"];

                if (!(objCommonFunc.IsValidImageFile(StorageRoot, Request, fileTypes)))
                {
                    fileUploadViewModel.ErrorMessage = "File Type is Not Allowed.";
                    return View("ImageUploadBySQC", fileUploadViewModel.ErrorMessage);
                }


                foreach (string file in Request.Files)
                {
                    string status = SQCValidateImageFileofProgress(Request.Files[0].ContentLength, System.IO.Path.GetExtension(Request.Files[0].FileName));
                    if (status != string.Empty)
                    {
                        fileUploadViewModel.ErrorMessage = status;
                        return View("ImageUploadBySQC", fileUploadViewModel.ErrorMessage);
                    }
                }

                var fileData = new List<PMGSY.Models.Feedback.FileUploadViewModel>();

                //
                foreach (string file in Request.Files)
                {
                    UploadImageFileBySQC(Request, fileData, fileUploadViewModel.FEED_ID, fileUploadViewModel.REP_ID);
                }
                var serializer = new System.Web.Script.Serialization.JavaScriptSerializer();
                serializer.MaxJsonLength = Int32.MaxValue;

                var result = new ContentResult
                {
                    Content = "{\"files\":" + serializer.Serialize(fileData) + "}",
                };
                return result;
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "FeedbackDetails.UploadsBySQC()");
                return null;
            }
        }



        // 4
        [HttpPost]
        public void UploadImageFileBySQC(HttpRequestBase request, List<PMGSY.Models.Feedback.FileUploadViewModel> statuses, int FEED_ID, int REP_ID)
        {
            dbContext = new PMGSYEntities();
            // IExecutionBAL objExecutionBAL = new ExecutionBAL();
            String StorageRoot = string.Empty;
            ModelState.Remove("PdfDescription");



            try
            {
                string errorMessage = string.Empty;
                string lati = string.Empty;
                string longi = string.Empty;

                for (int i = 0; i < request.Files.Count; i++)
                {
                    HttpPostedFileBase file = request.Files[i];
                    // var fileName = IMS_PR_ROAD_CODE + System.IO.Path.GetExtension(request.Files[i].FileName).ToString();

                    //#region LAT_LONG_CALCULATION

                    //Double[] latitudeRef = null;
                    //Double[] longitudeRef = null;
                    //StringBuilder strLat = new StringBuilder();
                    //StringBuilder strLong = new StringBuilder();

                    //using (ExifReader reader = new ExifReader(file.InputStream, true))
                    //{
                    //    reader.GetTagValue<Double[]>(ExifTags.GPSLatitude, out latitudeRef);
                    //    reader.GetTagValue<Double[]>(ExifTags.GPSLongitude, out longitudeRef);

                    //    if (latitudeRef != null && longitudeRef != null)
                    //    {
                    //        //return Json(new { success = true, latitude = latitude, longitude = longitude });

                    //        for (int value = 0; value < latitudeRef.Count(); value++)
                    //        {
                    //            if (value == 0)
                    //            {
                    //                strLat.Append(latitudeRef[value].ToString() + ".");
                    //            }
                    //            else
                    //            {
                    //                if (latitudeRef[value].ToString().Contains("."))
                    //                {
                    //                    strLat.Append(latitudeRef[value].ToString().Replace(".", string.Empty).ToString());
                    //                }
                    //                else
                    //                {
                    //                    strLat.Append(latitudeRef[value].ToString());
                    //                }
                    //            }
                    //        }

                    //        for (int value = 0; value < longitudeRef.Count(); value++)
                    //        {
                    //            if (value == 0)
                    //            {
                    //                strLong.Append(longitudeRef[value].ToString() + ".");
                    //            }
                    //            else
                    //            {
                    //                if (longitudeRef[value].ToString().Contains("."))
                    //                {
                    //                    strLong.Append(longitudeRef[value].ToString().Replace(".", string.Empty).ToString());
                    //                }
                    //                else
                    //                {
                    //                    strLong.Append(longitudeRef[value].ToString());
                    //                }
                    //            }
                    //        }
                    //    }
                    //    else
                    //    {
                    //        errorMessage = "Image does not contain the Geo location information.";
                    //        //return Json(new { success = false, message = "Image does not contain the Geo location information." });
                    //    }
                    //}
                    //#endregion
                    #region LAT_LONG_CALCULATION

                    Double[] latitudeRef = null;
                    Double[] longitudeRef = null;
                    StringBuilder strLat = new StringBuilder();
                    StringBuilder strLong = new StringBuilder();

                    using (ExifReader reader = new ExifReader(file.InputStream, true))
                    {
                        reader.GetTagValue<Double[]>(ExifTags.GPSLatitude, out latitudeRef);
                        reader.GetTagValue<Double[]>(ExifTags.GPSLongitude, out longitudeRef);

                        if (latitudeRef != null && longitudeRef != null)
                        {
                            //return Json(new { success = true, latitude = latitude, longitude = longitude });

                            for (int value = 0; value < latitudeRef.Count(); value++)
                            {
                                if (value == 0)
                                {
                                    strLat.Append(latitudeRef[value].ToString() + ".");
                                }
                                else
                                {
                                    if (latitudeRef[value].ToString().Contains("."))
                                    {
                                        strLat.Append(latitudeRef[value].ToString().Replace(".", string.Empty).ToString());
                                    }
                                    else
                                    {
                                        strLat.Append(latitudeRef[value].ToString());
                                    }

                                }
                            }


                            for (int value = 0; value < longitudeRef.Count(); value++)
                            {
                                if (value == 0)
                                {
                                    strLong.Append(longitudeRef[value].ToString() + ".");
                                }
                                else
                                {
                                    if (longitudeRef[value].ToString().Contains("."))
                                    {
                                        strLong.Append(longitudeRef[value].ToString().Replace(".", string.Empty).ToString());
                                    }
                                    else
                                    {
                                        strLong.Append(longitudeRef[value].ToString());
                                    }
                                }
                            }
                        }
                    }

                    #endregion

                    var timestampArray = System.DateTime.Now.ToString("MM/dd/yyyy hh:mm:ss.fff").Replace('.', '_').Split(' ');
                    string timestamp = timestampArray[0].ToString() + "_" + timestampArray[1].ToString();
                    var fileName = "IMG_" + timestamp.Replace('/', '_').Replace(':', '_') + "_" + FEED_ID.ToString() + "_" + REP_ID.ToString() + System.IO.Path.GetExtension(request.Files[i].FileName).ToString();

                    StorageRoot = System.Configuration.ConfigurationManager.AppSettings["MERI_SADAK_FEEDBACK_REPLY_FILE_IMAGE_PHYSICAL_DIR_PATH"];
                    var fullPath = System.IO.Path.Combine(StorageRoot, fileName);

                    var ThumbnailPath = System.IO.Path.Combine(StorageRoot, "thumbnails");
                    var FullThumbnailPath = System.IO.Path.Combine(ThumbnailPath, fileName);

                    if (!(System.IO.Directory.Exists(ThumbnailPath)))
                    {
                        System.IO.Directory.CreateDirectory(ThumbnailPath);
                    }

                    statuses.Add(new PMGSY.Models.Feedback.FileUploadViewModel()
                    {
                        url = fullPath,
                        thumbnail_url = fullPath,
                        name = fileName,
                        type = file.ContentType,
                        size = file.ContentLength,
                        delete_url = "",
                        delete_type = "DELETE",
                        Image_Description = request.Params["remark[]"],  //.Split(',')[i]
                        FEED_ID = FEED_ID,
                        REP_ID = REP_ID,
                        Latitude = Convert.ToDecimal(strLat.ToString()),
                        Longitude = Convert.ToDecimal(strLong.ToString())


                    });
                    string status = fbDAL.AddFileUploadDetailsBALBySQCDAL(statuses, FEED_ID, REP_ID, fileName, request.Params["remark[]"]);


                    if (status == string.Empty)
                    {
                        HttpPostedFileBase postBasefile = request.Files[0];
                        postBasefile.SaveAs(Path.Combine(StorageRoot, fileName));

                        new PMGSY.BAL.Proposal.ProposalBAL().CompressImage(request.Files[0], fullPath, FullThumbnailPath);

                    }
                    else
                    {
                        // show an error over here
                    }

                    //if (status == string.Empty)
                    //{
                    //    new PMGSY.BAL.Proposal.ProposalBAL().CompressImage(request.Files[0], fullPath, FullThumbnailPath);
                    //}
                    //else
                    //{
                    //    // show an error over here
                    //}
                }
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "FeedbackDetails.UploadImageFileBySQC()");
            }
            finally
            {
                if (dbContext != null)
                {
                    dbContext.Dispose();
                }
            }
        }

        // 5
        public string SQCValidateImageFileofProgress(int FileSize, string FileExtension)
        {
            string ValidExtensions = System.Configuration.ConfigurationManager.AppSettings["MERI_SADAK_FEEDBACK_REPLY_FILE_IMAGE_VALID_FORMAT"];
            string[] arrValidFormats = ValidExtensions.Split('$');


            if (!arrValidFormats.Contains(FileExtension.ToLower()))
            {
                return "File is not Valid Image File";
            }
            if (FileSize > Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["MERI_SADAK_FEEDBACK_REPLY_FILE_IMAGE_FILE_MAX_SIZE"]))
            {
                return "File Size Exceed the Maximum File Limit";
            }

            return string.Empty;
        }

        //6
        [HttpGet]
        public ActionResult DownloadFileUploadedBySQC(String parameter, String hash, String key)
        {
            string FileName = string.Empty;
            string FullFileLogicalPathName = string.Empty;
            string FullfilePhysicalPathName = string.Empty;
            string FullFileLogicalPathId = string.Empty;
            string FullfilePhysicalPathId = string.Empty;
            string FileExtension = string.Empty;
            Int32 FileID = 0;
            dbContext = new PMGSYEntities();
            try
            {
                if (!String.IsNullOrEmpty(parameter) && !String.IsNullOrEmpty(hash) && !String.IsNullOrEmpty(key))
                {
                    String[] urlParams = URLEncrypt.DecryptParameters(new String[] { parameter, hash, key });
                    if (urlParams.Length >= 1)
                    {
                        String[] urlSplitParams = urlParams[0].Split('$');
                        FileName = (urlSplitParams[0]);
                        FileID = Convert.ToInt32(urlSplitParams[1]);
                    }
                }
                FileExtension = System.IO.Path.GetExtension(FileName).ToLower();

                if (FileExtension == ".jpg" || FileExtension == ".jpeg" || FileExtension == ".bmp" || FileExtension == ".png" || FileExtension == ".gif" || FileExtension == ".tiff")
                {
                    FullFileLogicalPathName = System.IO.Path.Combine(System.Configuration.ConfigurationManager.AppSettings["MERI_SADAK_FEEDBACK_REPLY_FILE_IMAGE_VIRTUAL_DIR_PATH"], FileName);
                    FullfilePhysicalPathName = System.IO.Path.Combine(System.Configuration.ConfigurationManager.AppSettings["MERI_SADAK_FEEDBACK_REPLY_FILE_IMAGE_PHYSICAL_DIR_PATH"], FileName);

                }

                string name = System.IO.Path.GetFileName(FileName);
                string ext = System.IO.Path.GetExtension(FileName);

                string type = string.Empty;

                if (ext != null)
                {
                    switch (ext.ToLower())
                    {
                        case ".pdf":
                            type = "Application/pdf";
                            break;
                        case ".doc":
                        case ".docx":
                            type = "Application/msword";
                            break;
                        case ".jpg":
                        case ".bmp":
                        case ".tiff":
                        case ".png":
                        case ".gif":
                        case ".jpeg":
                            type = "image/png";
                            break;
                        default:
                            type = "Application";
                            break;
                    }
                }

                if (System.IO.File.Exists(FullfilePhysicalPathName))
                {
                    return File(FullfilePhysicalPathName, type, DateTime.Now.ToShortDateString() + "_" + new Random().Next(1000000000) + FileExtension);
                }
                else if (System.IO.File.Exists(FullfilePhysicalPathId))
                {
                    return File(FullfilePhysicalPathId, type, DateTime.Now.ToShortDateString() + "_" + new Random().Next(1000000000) + FileExtension);
                }
                else
                {
                    return Json(new { Error = "File Not Exist" }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "FeedbackDetails.DownloadFileUploadedBySQC()");
                return Json(new { Success = "false" }, JsonRequestBehavior.AllowGet);
            }
            finally
            {
                if (dbContext != null)
                {
                    dbContext.Dispose();
                }
            }
        }


        // 7
        [HttpPost]
        public JsonResult DeleteFileDetailsBySQC(String parameter, String hash, String key)
        {
            String PhysicalPath = string.Empty;
            String ThumbnailPath = string.Empty;
            dbContext = new PMGSYEntities();


            try
            {
                String[] urlParams = URLEncrypt.DecryptParameters(new String[] { parameter, hash, key });

                String[] urlSplitParams = urlParams[0].Split('$');
                string QM_FILE_NAME = (urlSplitParams[0]);
                int FileID = Convert.ToInt32(urlSplitParams[1]); // This is Primary Key

                PhysicalPath = System.Configuration.ConfigurationManager.AppSettings["MERI_SADAK_FEEDBACK_REPLY_FILE_IMAGE_PHYSICAL_DIR_PATH"];
                ThumbnailPath = System.IO.Path.Combine(System.IO.Path.Combine(PhysicalPath, "thumbnails"), QM_FILE_NAME);
                PhysicalPath = System.IO.Path.Combine(PhysicalPath, QM_FILE_NAME);

                // We are not allowing SQC to delete file physically as it may have copied from PIU and we dont want PIU Image file to be deleted.
                //if (!System.IO.File.Exists(PhysicalPath) && !System.IO.File.Exists(ThumbnailPath))
                //{
                //    return Json(new { Success = false, ErrorMessage = "File Not Found." });
                //}

                string status = fbDAL.DeleteFileDetailsBySQCDAL(FileID);
                if (status == string.Empty)
                {
                    try
                    {                // We are not allowing SQC to delete file physically as it may have copied from PIU and we dont want PIU Image file to be deleted.
                        //System.IO.File.Delete(PhysicalPath);
                        //System.IO.File.Delete(ThumbnailPath);
                    }
                    catch (Exception ex)
                    {
                        Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                        return Json(new { Success = true, ErrorMessage = ex.Message });
                    }
                    return Json(new { Success = true, ErrorMessage = "Error Ocurred While Processing Your Request." });
                }
                else
                {
                    return Json(new { Success = false, ErrorMessage = status });
                }
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "FeedbackDetails.DeleteFileDetailsBySQC()");
                return Json(new { Success = false, ErrorMessage = "Error Ocurred While Processing Your Request." });
            }
            finally
            {
                if (dbContext != null)
                {
                    dbContext.Dispose();
                }
            }
        }


        [HttpPost]
        public JsonResult UploadSameImageAsPIUDetails(FormCollection formCollection)
        {
            String PhysicalPath = string.Empty;
            String ThumbnailPath = string.Empty;
            dbContext = new PMGSYEntities();


            try
            {
                Int32 FEED_ID = Convert.ToInt32(Request.Params["FEED_ID"]);
                Int32 REP_ID = Convert.ToInt32(Request.Params["REP_ID"]);



                string status = fbDAL.UploadSameImageAsPIUDAL(FEED_ID, REP_ID);
                if (status == string.Empty)
                {
                    return Json(new { Success = true, ErrorMessage = "Error Ocurred While Processing Your Request." });
                }
                else
                {
                    return Json(new { Success = false, ErrorMessage = status });
                }
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "FeedbackDetails.UploadSameImageAsPIUDetails()");
                return Json(new { Success = false, ErrorMessage = "Error Ocurred While Processing Your Request." });
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



        #region PDF Upload SQC

        //1
        [HttpGet]
        public ActionResult PDFFileUploadBySQC(String parameter, String hash, String key)
        {
            try
            {
                PMGSY.Models.PMGSYEntities dbContext = new PMGSY.Models.PMGSYEntities();
                FileUploadViewModel fileUploadViewModel = new FileUploadViewModel();
                int feedId = 0;
                int repId = 0;

                bool flag = false;
                decryptedParameters = PMGSY.Common.URLEncrypt.DecryptParameters1(new String[] { parameter, hash, key });
                if (decryptedParameters.Count > 0)
                {
                    string id = Convert.ToString(decryptedParameters["FeedCode"]).Trim();
                    string[] fId = id.Trim().Split('$');

                    if (!int.TryParse(fId[0], out outParam))
                    {
                        return Json(false);
                    }
                    if (!int.TryParse(fId[1], out outParam))
                    {
                        return Json(false);
                    }

                    feedId = Convert.ToInt32(fId[0].Trim());
                    repId = Convert.ToInt32(fId[1].Trim());


                }

                fileUploadViewModel.FEED_ID = feedId;
                fileUploadViewModel.REP_ID = repId;
                fileUploadViewModel.FinalizedByPIU = dbContext.ADMIN_FEEDBACK_REPLY.Where(m => m.IS_FORWARD_TO_SQC == "Y" && m.IS_FORWARD_TO_SQC != null && m.FEED_ID == feedId && m.REP_ID == repId).Select(m => m.IS_FORWARD_TO_SQC).FirstOrDefault();
                fileUploadViewModel.FILE_ID = dbContext.ADMIN_FEEDBACK_REPLY_FILES.Where(m => m.FEED_ID == feedId && m.REP_ID == repId).Select(m => m.FILE_ID).FirstOrDefault();

                // File ID gete it
                //if (string.IsNullOrEmpty(fileUploadViewModel.FinalizedByPIU))
                //{
                //    fileUploadViewModel.FinalizedByPIU = "N";
                //}


                fileUploadViewModel.FinalizedBySQC = dbContext.ADMIN_FEEDBACK_REPLY.Where(m => m.FEED_ID == feedId && m.REP_ID == repId).Select(m => m.SQC_FINALIZED).FirstOrDefault();



                if (PMGSYSession.Current.RoleCode == 22)
                {
                    fileUploadViewModel.FinalizedByPIU = "Y";

                }
                else
                {

                    fileUploadViewModel.FinalizedByPIU = "N";
                }

                if (dbContext.ADMIN_FEEDBACK_REPLY_FILES.Where(m => m.FEED_ID == feedId && m.REP_ID == repId).Any())
                {
                    string FileName = dbContext.ADMIN_FEEDBACK_REPLY_FILES.Where(m => m.FEED_ID == feedId && m.REP_ID == repId).Select(m => m.FILE_NAME).FirstOrDefault().ToString();

                    if (FileName.Equals("NA"))
                    {
                        fileUploadViewModel.NumberofFiles = 0;
                    }
                    else if (String.IsNullOrEmpty(FileName))
                    {
                        fileUploadViewModel.NumberofFiles = 0;
                    }
                    else
                    {
                        fileUploadViewModel.NumberofFiles = 1;
                    }
                }
                else
                {
                    fileUploadViewModel.NumberofFiles = 0;
                }


                return View("PDFFileUploadBySQC", fileUploadViewModel);
                //  return View("ImageUploadByPIU", fileUploadViewModel);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "FeedbbackDetailsController.PDFFileUploadBySQC()");
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



        //2
        [HttpPost]
        public JsonResult ListPDFFilesBySQC(FormCollection formCollection)
        {
            // IExecutionBAL objExecutionBAL = new ExecutionBAL();
            try
            {

                using (CommonFunctions commonFunction = new CommonFunctions())
                {
                    if (!commonFunction.ValidateGridParameters(new GridParams(Convert.ToInt32(formCollection["page"]), Convert.ToInt32(formCollection["rows"]), formCollection["sidx"], formCollection["sord"], Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                    {
                        return null;
                    }
                }
                Int32 FEED_ID = Convert.ToInt32(Request.Params["FEED_ID"]);
                Int32 REP_ID = Convert.ToInt32(Request.Params["REP_ID"]);

                int totalRecords;
                var jsonData = new
                {
                    rows = fbDAL.GetPDFFilesListDALBySQC(Convert.ToInt32(formCollection["page"]) - 1, Convert.ToInt32(formCollection["rows"]), formCollection["sidx"], formCollection["sord"], out totalRecords, FEED_ID, REP_ID),
                    total = totalRecords <= Convert.ToInt32(formCollection["rows"]) ? 1 : (totalRecords % Convert.ToInt32(formCollection["rows"]) == 0 ? totalRecords / Convert.ToInt32(formCollection["rows"]) : (totalRecords / Convert.ToInt32(formCollection["rows"]) + 1)),
                    page = Convert.ToInt32(formCollection["page"]),
                    records = totalRecords
                };
                return Json(jsonData);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "FeedbackDetails.ListPDFFilesBySQC()");
                return null;
            }

        }




        //3 
        [HttpPost]
        public ActionResult UploadsPDFDetailsBySQC(PMGSY.Models.Feedback.FileUploadViewModel fileUploadViewModel)
        {
            try
            {
                CommonFunctions objCommonFunc = new CommonFunctions();
                String[] fileTypes = new String[] { "pdf" };
                String StorageRoot = string.Empty;
                StorageRoot = System.Configuration.ConfigurationManager.AppSettings["MERI_SADAK_FEEDBACK_REPLY_FILE_PDF_PHYSICAL_DIR_PATH"];
                if (!(objCommonFunc.IsValidImageFileForContractor(ConfigurationManager.AppSettings["MERI_SADAK_FEEDBACK_REPLY_FILE_PDF_PHYSICAL_DIR_PATH"], Request, fileTypes)))
                {
                    fileUploadViewModel.ErrorMessage = "File Type is Not Allowed.";
                    return View("PDFFileUploadBySQC", fileUploadViewModel.ErrorMessage);
                }

                var fileData = new List<PMGSY.Models.Feedback.FileUploadViewModel>();
                foreach (string file in Request.Files)
                {
                    UploadsPDFDetailsBySQCDetails(Request, fileData, fileUploadViewModel.FEED_ID, fileUploadViewModel.REP_ID);
                }
                var serializer = new System.Web.Script.Serialization.JavaScriptSerializer();
                serializer.MaxJsonLength = Int32.MaxValue;

                var result = new ContentResult
                {
                    Content = "{\"files\":" + serializer.Serialize(fileData) + "}",
                };
                return result;
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "FeedbackDetails.UploadsPDFDetailsBySQC()");
                return null;
            }
        }


        // 4
        [HttpPost]
        public void UploadsPDFDetailsBySQCDetails(HttpRequestBase request, List<PMGSY.Models.Feedback.FileUploadViewModel> statuses, int FEED_ID, int REP_ID)
        {
            dbContext = new PMGSYEntities();
            // IExecutionBAL objExecutionBAL = new ExecutionBAL();
            String StorageRoot = string.Empty;
            ModelState.Remove("PdfDescription");
            try
            {
                for (int i = 0; i < request.Files.Count; i++)
                {
                    HttpPostedFileBase file = request.Files[i];

                    var timestampArray = System.DateTime.Now.ToString("MM/dd/yyyy hh:mm:ss.fff").Replace('.', '_').Split(' ');
                    string timestamp = timestampArray[0].ToString() + "_" + timestampArray[1].ToString();
                    var fileName = "PDF_" + timestamp.Replace('/', '_').Replace(':', '_') + "_" + FEED_ID.ToString() + "_" + REP_ID.ToString() + System.IO.Path.GetExtension(request.Files[i].FileName).ToString();
                    StorageRoot = System.Configuration.ConfigurationManager.AppSettings["MERI_SADAK_FEEDBACK_REPLY_FILE_PDF_PHYSICAL_DIR_PATH"];
                    var fullPath = System.IO.Path.Combine(StorageRoot, fileName);

                    var ThumbnailPath = System.IO.Path.Combine(StorageRoot, "thumbnails");
                    var FullThumbnailPath = System.IO.Path.Combine(ThumbnailPath, fileName);

                    if (!(System.IO.Directory.Exists(ThumbnailPath)))
                    {
                        System.IO.Directory.CreateDirectory(ThumbnailPath);
                    }

                    statuses.Add(new PMGSY.Models.Feedback.FileUploadViewModel()
                    {
                        url = fullPath,
                        thumbnail_url = fullPath,
                        name = fileName,
                        type = file.ContentType,
                        size = file.ContentLength,
                        delete_url = "",
                        delete_type = "DELETE",
                        Image_Description = request.Params["remark[]"],  //.Split(',')[i]
                        FEED_ID = FEED_ID,
                        REP_ID = REP_ID

                    });
                    string status = fbDAL.AddPDFFileUploadDetailsBALBySQCDAL(FEED_ID, REP_ID, fileName, request.Params["remark[]"]);

                    if (status == string.Empty)
                    {
                        if (fileName.Contains("pdf") || fileName.Contains("xls"))
                        {
                            HttpPostedFileBase postBasefile = request.Files[0];
                            postBasefile.SaveAs(Path.Combine(StorageRoot, fileName));

                        }
                        else
                        {
                            CompressPDFSQC(request.Files[0], fullPath, FullThumbnailPath);
                        }
                    }
                    else
                    {
                        // show an error over here
                    }

                }
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "FeedbackDetails.UploadsPDFDetailsBySQCDetails()");
            }
            finally
            {
                if (dbContext != null)
                {
                    dbContext.Dispose();
                }
            }
        }


        //5
        public void CompressPDFSQC(HttpPostedFileBase httpPostedFileBase, string DestinitionPath, string ThumbnailPath)
        {
            try
            {
                // For Thumbnail Image    
                ImageResizer.ImageJob ThumbnailJob = new ImageResizer.ImageJob(httpPostedFileBase, ThumbnailPath,
                    new ImageResizer.ResizeSettings("width=100;height=100;format=jpg;mode=max"));

                ThumbnailJob.Build();

                // For Original Image
                ImageResizer.ImageJob job = new ImageResizer.ImageJob(httpPostedFileBase, DestinitionPath,
                    new ImageResizer.ResizeSettings("width=1024;height=768;format=jpg;mode=max"));

                job.Build();
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "FeedbackController.CompressPDFSQC()");
            }
        }

        // 6
        [HttpGet]
        public ActionResult DownloadPDFFileUploadedBySQC(String parameter, String hash, String key)
        {
            string FileName = string.Empty;
            string FullFileLogicalPathName = string.Empty;
            string FullfilePhysicalPathName = string.Empty;
            string FullFileLogicalPathId = string.Empty;
            string FullfilePhysicalPathId = string.Empty;
            string FileExtension = string.Empty;
            Int32 FileID = 0;
            dbContext = new PMGSYEntities();
            try
            {
                if (!String.IsNullOrEmpty(parameter) && !String.IsNullOrEmpty(hash) && !String.IsNullOrEmpty(key))
                {
                    String[] urlParams = URLEncrypt.DecryptParameters(new String[] { parameter, hash, key });
                    if (urlParams.Length >= 1)
                    {
                        String[] urlSplitParams = urlParams[0].Split('$');
                        FileName = (urlSplitParams[0]);
                        FileID = Convert.ToInt32(urlSplitParams[1]);
                    }
                }
                FileExtension = System.IO.Path.GetExtension(FileName).ToLower();

                if (FileExtension == ".pdf" || FileExtension == ".jpg" || FileExtension == ".jpeg" || FileExtension == ".bmp" || FileExtension == ".png" || FileExtension == ".gif" || FileExtension == ".tiff")
                {
                    FullFileLogicalPathName = System.IO.Path.Combine(System.Configuration.ConfigurationManager.AppSettings["MERI_SADAK_FEEDBACK_REPLY_FILE_PDF_VIRTUAL_DIR_PATH"], FileName);
                    FullfilePhysicalPathName = System.IO.Path.Combine(System.Configuration.ConfigurationManager.AppSettings["MERI_SADAK_FEEDBACK_REPLY_FILE_PDF_PHYSICAL_DIR_PATH"], FileName);

                }

                string name = System.IO.Path.GetFileName(FileName);
                string ext = System.IO.Path.GetExtension(FileName);

                string type = string.Empty;

                if (ext != null)
                {
                    switch (ext.ToLower())
                    {
                        case ".pdf":
                            type = "Application/pdf";
                            break;
                        case ".doc":
                        case ".docx":
                            type = "Application/msword";
                            break;
                        case ".jpg":
                        case ".bmp":
                        case ".tiff":
                        case ".png":
                        case ".gif":
                        case ".jpeg":
                            type = "image/png";
                            break;
                        default:
                            type = "Application";
                            break;
                    }
                }

                if (System.IO.File.Exists(FullfilePhysicalPathName))
                {
                    return File(FullfilePhysicalPathName, type, DateTime.Now.ToShortDateString() + "_" + new Random().Next(1000000000) + FileExtension);
                }
                else if (System.IO.File.Exists(FullfilePhysicalPathId))
                {
                    return File(FullfilePhysicalPathId, type, DateTime.Now.ToShortDateString() + "_" + new Random().Next(1000000000) + FileExtension);
                }
                else
                {
                    return Json(new { Error = "File Not Exist" }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "FeedbackDetails.DownloadPDFFileUploadedBySQC()");
                return Json(new { Success = "false" }, JsonRequestBehavior.AllowGet);
            }
            finally
            {
                if (dbContext != null)
                {
                    dbContext.Dispose();
                }
            }
        }

        // 7
        [HttpPost]
        public JsonResult DeletePDFFileDetailsBySQC(String parameter, String hash, String key)
        {
            String PhysicalPath = string.Empty;
            String ThumbnailPath = string.Empty;
            dbContext = new PMGSYEntities();

            try
            {
                String[] urlParams = URLEncrypt.DecryptParameters(new String[] { parameter, hash, key });

                String[] urlSplitParams = urlParams[0].Split('$');
                string QM_FILE_NAME = (urlSplitParams[0]);
                int FileID = Convert.ToInt32(urlSplitParams[1]); // This is Primary Key

                PhysicalPath = System.Configuration.ConfigurationManager.AppSettings["MERI_SADAK_FEEDBACK_REPLY_FILE_PDF_PHYSICAL_DIR_PATH"];
                ThumbnailPath = System.IO.Path.Combine(System.IO.Path.Combine(PhysicalPath, "thumbnails"), QM_FILE_NAME);
                PhysicalPath = System.IO.Path.Combine(PhysicalPath, QM_FILE_NAME);
                if (!System.IO.File.Exists(PhysicalPath) && !System.IO.File.Exists(ThumbnailPath))
                {
                    return Json(new { Success = false, ErrorMessage = "File Not Found." });
                }
                string status = fbDAL.DeletePDFFileDetailsBySQCDAL(FileID);
                if (status == string.Empty)
                {
                    try
                    {
                        // PDF Physically Deletion by SQC is restrited as it may delete it from PIU if Same as PIU option is selected.
                        //System.IO.File.Delete(PhysicalPath);
                        //System.IO.File.Delete(ThumbnailPath);
                    }
                    catch (Exception ex)
                    {
                        Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                        return Json(new { Success = true, ErrorMessage = ex.Message });
                    }
                    return Json(new { Success = true, ErrorMessage = "Error Ocurred While Processing Your Request." });
                }
                else
                {
                    return Json(new { Success = false, ErrorMessage = status });
                }
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "FeedbackDetails.DeletePDFFileDetailsBySQC()");
                return Json(new { Success = false, ErrorMessage = "Error Ocurred While Processing Your Request." });
            }
            finally
            {
                if (dbContext != null)
                {
                    dbContext.Dispose();
                }
            }
        }





        [HttpPost]
        public JsonResult UploadSamePDFAsPIUDetails(FormCollection formCollection)
        {
            String PhysicalPath = string.Empty;
            String ThumbnailPath = string.Empty;
            dbContext = new PMGSYEntities();


            try
            {
                Int32 FEED_ID = Convert.ToInt32(Request.Params["FEED_ID"]);
                Int32 REP_ID = Convert.ToInt32(Request.Params["REP_ID"]);



                string status = fbDAL.UploadSamePDFAsPIUDAL(FEED_ID, REP_ID);
                if (status == string.Empty)
                {
                    return Json(new { Success = true, ErrorMessage = "Error Ocurred While Processing Your Request." });
                }
                else
                {
                    return Json(new { Success = false, ErrorMessage = status });
                }
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "FeedbackDetails.UploadSamePDFAsPIUDetails()");
                return Json(new { Success = false, ErrorMessage = "Error Ocurred While Processing Your Request." });
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


        #region Finalize by SQC, delete by SQC

        public JsonResult SQCFinalize(String parameter, String hash, String key/*string id*/)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    bool flag = false;

                    decryptedParameters = PMGSY.Common.URLEncrypt.DecryptParameters1(new String[] { parameter, hash, key });
                    if (decryptedParameters.Count > 0)
                    {
                        string id = Convert.ToString(decryptedParameters["FeedCode"]).Trim();
                        string[] fId = id.Trim().Split('$');

                        if (!int.TryParse(fId[0], out outParam))
                        {
                            return Json(false);
                        }
                        if (!int.TryParse(fId[1], out outParam))
                        {
                            return Json(false);
                        }

                        int feedId = Convert.ToInt32(fId[0].Trim());
                        int repId = Convert.ToInt32(fId[1].Trim());

                        flag = fbDAL.SQCFinalizeDAL(feedId, repId);

                    }
                    return Json(new { status = flag }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    string message = ModelState.Values.Where(x => x.Errors.Count == 1).FirstOrDefault().Errors[0].ErrorMessage;
                    return Json(string.Empty);
                }
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "FeedbackDetailsController().SQCFinalize");
                //Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return null;
            }
        }


        public JsonResult DelBySQC(String parameter, String hash, String key/*string id*/)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    bool flag = false;

                    decryptedParameters = PMGSY.Common.URLEncrypt.DecryptParameters1(new String[] { parameter, hash, key });
                    if (decryptedParameters.Count > 0)
                    {
                        string id = Convert.ToString(decryptedParameters["FeedCode"]).Trim();
                        string[] fId = id.Trim().Split('$');

                        if (!int.TryParse(fId[0], out outParam))
                        {
                            return Json(false);
                        }
                        if (!int.TryParse(fId[1], out outParam))
                        {
                            return Json(false);
                        }

                        int feedId = Convert.ToInt32(fId[0].Trim());
                        int repId = Convert.ToInt32(fId[1].Trim());

                        flag = fbDAL.DelBySQCDAL(feedId, repId);

                    }
                    return Json(new { status = flag }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    string message = ModelState.Values.Where(x => x.Errors.Count == 1).FirstOrDefault().Errors[0].ErrorMessage;
                    return Json(string.Empty);
                }
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "FeedbackDetailsController().DelBySQC");
                //Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return null;
            }
        }
        #endregion

        #region Geo Positions
        private bool ReadGeoPositions(HttpPostedFileBase file, out string errorMessage, out string latitude, out string longitude)
        {
            try
            {
                Double[] latitudeRef = null;
                Double[] longitudeRef = null;
                //HttpPostedFileBase file = formCollection["files"];
                using (ExifReader reader = new ExifReader(file.InputStream))
                {
                    reader.GetTagValue<Double[]>(ExifTags.GPSLatitude, out latitudeRef);
                    reader.GetTagValue<Double[]>(ExifTags.GPSLongitude, out longitudeRef);

                    if (latitudeRef != null && longitudeRef != null)
                    {
                        //return Json(new { success = true, latitude = latitude, longitude = longitude });
                        StringBuilder strLat = new StringBuilder();
                        for (int value = 0; value < latitudeRef.Count(); value++)
                        {
                            if (value == 0)
                            {
                                strLat.Append(latitudeRef[value].ToString() + ".");
                            }
                            else
                            {
                                if (latitudeRef[value].ToString().Contains("."))
                                {
                                    strLat.Append(latitudeRef[value].ToString().Replace(".", string.Empty).ToString());
                                }
                                else
                                {
                                    strLat.Append(latitudeRef[value].ToString());
                                }

                            }
                        }

                        StringBuilder strLong = new StringBuilder();
                        for (int value = 0; value < longitudeRef.Count(); value++)
                        {
                            if (value == 0)
                            {
                                strLong.Append(longitudeRef[value].ToString() + ".");
                            }
                            else
                            {
                                if (longitudeRef[value].ToString().Contains("."))
                                {
                                    strLong.Append(longitudeRef[value].ToString().Replace(".", string.Empty).ToString());
                                }
                                else
                                {
                                    strLong.Append(longitudeRef[value].ToString());
                                }
                            }
                        }

                        errorMessage = string.Empty;
                        latitude = strLat.ToString();
                        longitude = strLong.ToString();
                        return true;
                    }
                    else
                    {
                        errorMessage = "Image does not contain the Geo location information.";
                        //return Json(new { success = false, message = "Image does not contain the Geo location information." });
                        latitude = latitudeRef.ToString();
                        longitude = longitudeRef.ToString();
                        return false;
                    }

                }
            }
            catch (Exception)
            {
                errorMessage = "Please select file to upload";
                latitude = string.Empty;
                longitude = string.Empty;
                return false;
                //return Json(new { success = false , message = "Error occurred while processing your request."});
            }
        }


        [HttpPost]
        public ActionResult ReadGeoPositions(FormCollection formCollection)
        {
            try
            {
                Double[] latitudeRef = null;
                Double[] longitudeRef = null;

                Stream stream = Request.InputStream;

                if (stream != null)
                {
                    using (ExifReader reader = new ExifReader(stream))
                    {
                        reader.GetTagValue<Double[]>(ExifTags.GPSLatitude, out latitudeRef);
                        reader.GetTagValue<Double[]>(ExifTags.GPSLongitude, out longitudeRef);

                        if (latitudeRef != null && longitudeRef != null)
                        {
                            StringBuilder strLatitude = new StringBuilder();
                            for (int item = 0; item < latitudeRef.Count(); item++)
                            {
                                if (item == 0)
                                {
                                    strLatitude.Append(latitudeRef[item].ToString() + (char)176 + " ");
                                }
                                if (item == 1)
                                {
                                    strLatitude.Append(latitudeRef[item].ToString() + "' ");
                                }
                                if (item == 2)
                                {
                                    strLatitude.Append(Math.Round(latitudeRef[item], 1).ToString());
                                }
                            }

                            StringBuilder strLongitude = new StringBuilder();
                            for (int item = 0; item < longitudeRef.Count(); item++)
                            {
                                if (item == 0)
                                {
                                    strLongitude.Append(longitudeRef[item].ToString() + (char)176 + " ");
                                }
                                if (item == 1)
                                {
                                    strLongitude.Append(longitudeRef[item].ToString() + "' ");
                                }
                                if (item == 2)
                                {
                                    strLongitude.Append(Math.Round(longitudeRef[item], 1).ToString());
                                }
                                //strLongitude.Append(item.ToString());
                            }

                            return Json(new { success = true, latitude = strLatitude.ToString(), longitude = strLongitude.ToString() });
                        }
                        else
                        {
                            return Json(new { success = false, message = "Image does not contain the Geo location information." });
                        }

                    }
                }
                else
                {
                    return Json(new { success = false, message = "Please select image to upload." });
                }

                //return Json(new { success = true, latitude = latitudeRef.ToString(), longitude = longitudeRef.ToString() });
            }
            catch (Exception ex)
            {
                if (ex.Message == "Unable to locate EXIF content")
                {
                    return Json(new { success = false, message = "Image does not contain Longitude and Latitude." });
                }
                else if (ex.Message == "File is not a valid JPEG")
                {
                    return Json(new { success = false, message = "Please select a valid Image File." });
                }
                else
                {
                    return Json(new { success = false, message = "Error occurred while processing your request." });
                }
            }
        }

        #endregion

        #endregion
    }
}
