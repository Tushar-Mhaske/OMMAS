using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PMGSY.BAL.Maintenance;
using PMGSY.Model.Maintenance;
//using PMGSY.ViewModel.Maintenance;
using PMGSY.Common;

namespace PMGSY.Controllers
{
    public class TreePlantController : Controller
    {
        IManeTreePlantBAL maintenanceBAL;
        CommonFunctions common = new CommonFunctions();

        public TreePlantController()
        {
            maintenanceBAL = new ManeTreePlantBAL();
        }
        //
        // GET: /TreePlant/
        [HttpGet]
        public PartialViewResult Index(int id)
        {
            try
            {
                var PlantHeader = maintenanceBAL.GetHeader(id);
                PlantHeader.RoadCode = id;
                PlantHeader.roleCode = PMGSY.Extensions.PMGSYSession.Current.RoleCode;
                return PartialView(PlantHeader);
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
        public JsonResult TreePlantJson(int id)
        {
            try
            {
                var PlantHeader = maintenanceBAL.GetAll(id);
                return Json(PlantHeader);
            }
            catch (Exception ex)
            {
                return null;
            }
            finally
            {
            }
        }

        [HttpGet]
        public PartialViewResult TreePlantAdd(int id)
        {
            try
            {
                var PlantToCreate = new ManeTreePlantViewModel();
                PlantToCreate.TreePLant.IMS_PR_ROAD_CODE = id;
                PlantToCreate.TreePLant.IsFirst = maintenanceBAL.GetAll(id).Count == 0 ? true : false;
                PlantToCreate.MONTH_LIST = common.PopulateMonths(true);
                PlantToCreate.YEAR_LIST = common.PopulateYears(true);
                PlantToCreate.hdRoleCode = PMGSY.Extensions.PMGSYSession.Current.RoleCode;
                return PartialView(PlantToCreate);
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
        public JsonResult TreePlantAdd(ManeTreePlantViewModel treeModel)
        {
            bool flag = false;
            string message = string.Empty;
            try
            {
                if (ModelState.IsValid)
                {
                    flag = maintenanceBAL.Add(treeModel.TreePLant);
                }
                var listTreePlant = maintenanceBAL.GetAll(treeModel.TreePLant.IMS_PR_ROAD_CODE);

                message = flag == true ? "Tree Plant Added Successfully" : "Error occured on Tree Plant Add";
                //return Json(listTreePlant);
                return Json(new { message = message, recordList = listTreePlant }, JsonRequestBehavior.AllowGet);
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
        public JsonResult TreePlantDelete(int id)
        {
            string message = string.Empty;
            try
            {
                string sTREE_PLANT_ID = Request.Params["TREE_PLANT_ID"];
                int roadid = maintenanceBAL.GetPlant(id).IMS_PR_ROAD_CODE;
                message = maintenanceBAL.Delete(id);
                var listTreePlant = maintenanceBAL.GetAll(roadid);
                //return Json(listTreePlant);
                message = message == "" ? "Tree Plant Deleted Successfully" : "Error occured on Tree Plant Deletion";
                return Json(new { message = message, recordList = listTreePlant }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                //return null;
                return Json(new { message = "Error occured on Tree Plant Delete", recordList = (dynamic)null }, JsonRequestBehavior.AllowGet);
            }
            finally
            {

            }
        }

        [HttpGet]
        public PartialViewResult VerifyIndex(string id)
        {
            try
            {
                string[] param = id.Split('$');

                var PlantHeader = maintenanceBAL.GetHeader(Convert.ToInt32(param[0]));
                PlantHeader.RoadCode = Convert.ToInt32(param[0]);
                PlantHeader.roleCode = PMGSY.Extensions.PMGSYSession.Current.RoleCode;
                PlantHeader.obsId = Convert.ToInt32(param[1]);
                return PartialView("Index", PlantHeader);
            }
            catch (Exception ex)
            {
                return null;
            }
            finally
            {
            }
        }

        [HttpGet]
        public ActionResult TreePlantVerify(string id)
        {
            string[] param = null;
            try
            {
                param = id.Split('$');
                ManeTreePlantVerifyViewModel model = new ManeTreePlantVerifyViewModel();

                model.observationId = Convert.ToInt32(param[0]);
                model.roadCode = Convert.ToInt32(param[1]);

                //model.scheduleCode = dbContext.QUALITY_QM_SCHEDULE_DETAILS.Where(x => x.IMS_PR_ROAD_CODE == id && x.UM_User_Master.UserID == PMGSY.Extensions.PMGSYSession.Current.UserId).Select(x => x.QUALITY_QM_SCHEDULE.ADMIN_QM_CODE).FirstOrDefault();
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
        public JsonResult TreePlantVerifyAdd(ManeTreePlantVerifyViewModel model)
        {
            string message = string.Empty;
            bool success = false;
            try
            {
                if (ModelState.IsValid)
                {
                    message = maintenanceBAL.TreePlantVerifyAddBAL(model);
                    if (message == string.Empty)
                    {
                        return Json(new { success = true, message = "Tree Plant Verified successfully" }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        return Json(new { success = true, message = "Error occured on Tree Plant Verify" }, JsonRequestBehavior.AllowGet);
                    }
                }
                else
                {
                    return Json(new { success = true, message = "Error occured on Tree Plant Verify" }, JsonRequestBehavior.AllowGet);
                }

            }
            catch (Exception ex)
            {
                return Json(new { success = true, message = "Error occured on Tree Plant Verify" }, JsonRequestBehavior.AllowGet);
            }
            finally
            {
            }
        }
    }
}
