using PMGSY.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using PMGSY.Extensions;

namespace PMGSY.Controllers
{
    [RequiredAuthentication]
    [RequiredAuthorization]
    public class CommonFiltersController : Controller
    {
        //private PMGSYEntities dbContext = new PMGSYEntities();

        public ActionResult UserFilter()
        {
            ViewBag.FilterCollection = new CommonFilterWrapper().GetCommonFilters(true, true, true, true, true, true, true, true, true, true);
            return View();
        }

        // Accepts model of bool values and returns Partial View with neceesarry Dropdowns
        // GET: /CommonFilters/
        [Audit]
        public ActionResult CommonFilters(CommonFilterCollection comFilterCollObj) //(CommonFiltersModel model)
        {
            ViewBag.StateList = comFilterCollObj.StateMaster;
            ViewBag.DistrictList = comFilterCollObj.DistrictMaster;
            ViewBag.BlockList = comFilterCollObj.BlockMaster;
            ViewBag.VillageList = comFilterCollObj.VillageMaster;
            ViewBag.HabitationList = comFilterCollObj.HabitationMaster;
            ViewBag.StreamList = comFilterCollObj.StreamMaster;
            ViewBag.BatchList = comFilterCollObj.BatchMaster;
            ViewBag.MonthList = comFilterCollObj.MonthMaster;
            ViewBag.YearList = comFilterCollObj.YearMaster;
            ViewBag.ProposalTypeList = comFilterCollObj.ProposalTypeMaster;

            //return Partial View
            return PartialView("_CommonFilters");//, model);
        }


        [HttpPost]
        [Audit]
        public JsonResult GetDistricts(int selectedState, int selectedDistrict = 0)
        {
            List<SelectListItem> districtList = new List<SelectListItem>();
            districtList = new CommonFilterWrapper().GetDistricts(selectedDistrict, selectedState);
            return Json(districtList);
        }

        [HttpPost]
        [Audit]
        public JsonResult GetBlocks(int selectedDistrict, int selectedBlock = 0)
        {
            List<SelectListItem> blockList = new List<SelectListItem>();
            blockList = new CommonFilterWrapper().GetBlocks(selectedBlock, selectedDistrict);
            return Json(blockList);
        }

        [HttpPost]
        [Audit]
        public JsonResult GetVillages(int selectedBlock, int selectedVillage = 0)
        {
            List<SelectListItem> villageList = new List<SelectListItem>();
            villageList = new CommonFilterWrapper().GetVillages(selectedVillage, selectedBlock);
            return Json(villageList);
        }
     
        [HttpPost]
        [Audit]
        public JsonResult GetHabs(int selectedVillage, int selectedHab = 0)
        {
            List<SelectListItem> habList = new List<SelectListItem>();
            habList = new CommonFilterWrapper().GetHabitations(selectedHab, selectedVillage);
            return Json(habList);
        }

    }
}
