using PMGSY.Areas.WronglyMappedHabs.BAL;
using PMGSY.Areas.WronglyMappedHabs.DAL;
using PMGSY.Common;
using PMGSY.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;


namespace PMGSY.Areas.WronglyMappedHabs.Controllers
{
    [RequiredAuthentication]
    [RequiredAuthorization]
    public class WronglyMappedHabitationsController : Controller
    {
        CommonFunctions common = new CommonFunctions();

        
        string message = String.Empty;
        private IWronglyMappedHabsDAL objDAL = null;
        private IWronglyMappedHabsBAL objBAL = null;
        Dictionary<string, string> decryptedParameters = null;
        String[] encryptedParameters = null;
        int outParam = 0;
       // CommonFunctions common = new CommonFunctions();
        //
        // GET: /WronglyMappedHabs/WronglyMappedHabs/

        public ActionResult GetView()
        {
            return View();
        }

        //GetListofWronglyMappedHabs
        public ActionResult GetListofWronglyMappedHabs(int? page, int? rows, string sidx, string sord)
        {

            String searchParameters = String.Empty;
            long totalRecords;
            string agencyType = String.Empty;
            objBAL = new WronglyMappedHabsBAL();

            using (CommonFunctions commonFunction = new CommonFunctions())
            {
                if (!commonFunction.ValidateGridParameters(new GridParams(page, rows, sidx, sord, Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))


                {
                    return null;
                }
            }

            //if (!string.IsNullOrEmpty(Request.Params["AgencyType"]))
            //{
            //    agencyType = Request.Params["AgencyType"].Replace('+', ' ').Trim();
            ////}



            var jsonData = new
            {
                rows = objBAL.ListHabs(PMGSY.Extensions.PMGSYSession.Current.StateCode, Convert.ToInt32(page) - 1, Convert.ToInt32(rows), sidx, sord, out totalRecords),


                total = totalRecords <= Convert.ToInt32(rows) ? 1 : totalRecords / Convert.ToInt32(rows) + 1,
                page = Convert.ToInt32(page),
                records = totalRecords
            };

            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }

       [HttpPost]
        public ActionResult DeleteHabs(String id)
        {
            Dictionary<string, string> decryptedParameters = null;

            try
            {
                objBAL = new WronglyMappedHabsBAL();
                string [] param=id.Split('/');
                decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { param[0], param[1], param[2] });

                if (decryptedParameters.Count() > 0)
                {
                    if (!objBAL.DeleteHabsBAL(Convert.ToInt32(decryptedParameters["habCode"].ToString()), Convert.ToInt32(decryptedParameters["roadCode"].ToString())))
                    {
                        ModelState.AddModelError(string.Empty, "Habs details not deleted.");
                        return Json(new { success = false, message = "You can not delete this Habs details." }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        return Json(new { success = true, message = "Habs  details deleted successfully." }, JsonRequestBehavior.AllowGet);
                    }
                }
                return Json(new { success = false, message = "You can not delete this Habs  details." }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);

                return Json(new { success = false, message = "You can not delete this Habs  details." }, JsonRequestBehavior.AllowGet);
            }
        }





    }
}
