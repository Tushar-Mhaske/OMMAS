using PMGSY.Areas.FeedbackReport.Models;
using PMGSY.Controllers;
using PMGSY.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PMGSY.Areas.FeedbackReport.Controllers
{
    [RequiredAuthentication]
    [RequiredAuthorization]
    public class FeedbackController : Controller
    {
        //
        // GET: /FeedbackReport/Feedback/

        public ActionResult StateWiseStatus()
        {
            List<SelectListItem>  fbThroughList = new List<SelectListItem>();

            fbThroughList.Insert(0, new SelectListItem() { Text = "All", Value = "A" });
            fbThroughList.Insert(1, new SelectListItem() { Text = "Mobile", Value = "M" });
            fbThroughList.Insert(2, new SelectListItem() { Text = "Web", Value = "W" });

            ViewBag.fbThroughList = fbThroughList;

            ViewBag.FBThrough = "A";

            return View();
        }
        public PartialViewResult StateWiseStatusReport(string fromDate, string toDate, string fbThrough)
        {
            ViewBag.FromDate = fromDate;
            ViewBag.ToDate = toDate;
            ViewBag.fbThrough = fbThrough;
            return PartialView();
        }

        #region Feedback Pendency Report
        [HttpGet]
        public ActionResult FeedbackPendencyReport()
        {
            SearchModelForReport model = new SearchModelForReport();
            try
            {
                List<MASTER_STATE> StateList = new PMGSYEntities().MASTER_STATE.ToList();
                ViewBag.StateList = StateList;

                model.fbThroughList = new List<SelectListItem>();
                model.fbThroughList.Insert(0, new SelectListItem() { Text = "Both", Value = "%" });
                model.fbThroughList.Insert(1, new SelectListItem() { Text = "Mobile", Value = "M" });
                model.fbThroughList.Insert(2, new SelectListItem() { Text = "Web", Value = "W" });
                //model.fbThrough = "%";
                return View(model);
            }
            catch (Exception ex)
            {
                return null;
            }
            finally
            {

            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LoadFBPendencyReport(SearchModelForReport model)
        {
            try
            {
                int state = model.stateId;
                if (state > 36 || state < 0)
                {
                    return Json(new { success = false, message = "Please select a valid state" });
                }
                else
                {
                    List<string> throughList = new List<string>();
                    throughList.Add("%");
                    throughList.Add("W");
                    throughList.Add("M");

                    if (!throughList.Contains(model.fbThrough))
                    {
                        return Json(new { success = false, message = "Please select a valid Through Option" });
                    }
                }
                using (var dbContext = new PMGSYEntities())
                {
                    if (model.stateId != 0)
                    {
                        model.stateName = dbContext.MASTER_STATE.Where(m => m.MAST_STATE_CODE == model.stateId).SingleOrDefault().MAST_STATE_NAME;
                    }
                    else
                    {
                        model.stateName = "All States";
                    }
                }
                return View(model);
            }
            catch
            {
                return Json(new { success = false, message = "Something went wrong. Please check the selected options" });
            }
        }
        #endregion
    }
}
