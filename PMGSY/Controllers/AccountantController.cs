using PMGSY.Areas.Accountant.Models;
using PMGSY.BAL.Accountant;
using PMGSY.BAL.User_Manager;
using PMGSY.Common;
using PMGSY.Controllers;
using PMGSY.DAL;
using PMGSY.Extensions;
using PMGSY.Models.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PMGSY.Areas.Accountant.Controllers
{   [RequiredAuthentication]
    [RequiredAuthorization]
    public class AccountantController : Controller
    {
        //
        // GET: /Accountant/
        //Added by Pradip Patil 30-12-2016 start 
        [HttpGet]
        public ActionResult  AccUserList()
    {
        ACCModel Accmodel = new ACCModel();
        CommonFunctions commomFuncObj = new CommonFunctions();
        List<SelectListItem> RoleList = new List<SelectListItem>();

       // RoleList.Add(new SelectListItem { Value = "10", Text = "Bank" });
        RoleList.Add(new SelectListItem { Value = "26", Text = "Authorized Signatory" });
        RoleList.Add(new SelectListItem { Value = "21", Text = "Accountant" });
      //  RoleList.Add(new SelectListItem { Value = "33", Text = "Empowered Officer" });
        RoleList.Add(new SelectListItem { Value = "46", Text = "Finance" });
        RoleList.Insert(0, new SelectListItem { Text = "All", Value = "0" });
        #region Commented
        { //RoleList.Add(Bank);
            //Bank.Text = "Accountant"; Bank.Value = "21";
            //RoleList.Add(Bank);
            //Bank.Text = "Authorized Signatory"; Bank.Value = "26";
            //RoleList.Add(Bank);
            //Bank.Text = "Empowered Officer"; Bank.Value = "33";
            //RoleList.Add(Bank);
            //Bank.Text = "Finance"; Bank.Value = "46";

            //SelectListItem Accountant = new SelectListItem { Value = "21", Text = "Accountant" };
            //SelectListItem AuthorizedSignatory = new SelectListItem { Value = "26", Text = "Authorized Signatory" };
            //SelectListItem EmpoweredOfficer = new SelectListItem { Value = "33", Text = "Empowered Officer" };
            //SelectListItem Finance = new SelectListItem { Value = "46", Text = "Finance" };

            //RoleList.Add(Bank);
            //RoleList.Add(Accountant);
            //RoleList.Add(AuthorizedSignatory);
            //RoleList.Add(EmpoweredOfficer);
            //RoleList.Add(Finance);
        } 
        #endregion

        Accmodel.RoleList = RoleList;

        //populate SRRDA
        List<SelectListItem> lstSRRDA = new List<SelectListItem>();
        lstSRRDA = commomFuncObj.PopulateNodalAgencies();
        lstSRRDA.Insert(0, (new SelectListItem { Text = "All", Value = "0", Selected = true }));
        Accmodel.SRRDAList = lstSRRDA;

        //Populate DPIU
        List<SelectListItem> lstDPIU = new List<SelectListItem>();
        lstDPIU.Insert(0, new SelectListItem { Text = "All", Value = "0" });
        Accmodel.DPIUList = lstDPIU;


        return View("UserList", Accmodel);

    }

       [Audit]
       [HttpPost]
        public JsonResult PopulateDPIU(string id)
        {
            try
            {
                TransactionParams objParam = new TransactionParams();
                CommonFunctions objCommonFunction = new CommonFunctions();

                int AdminNdCode = Convert.ToInt32(id);
                objParam.ADMIN_ND_CODE = AdminNdCode;


                if (AdminNdCode == 0)
                {
                    List<SelectListItem> lstDpiu = new List<SelectListItem>();
                    lstDpiu.Insert(0, new SelectListItem { Text = "All", Value = "0" });
                    return Json(lstDpiu, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    List<SelectListItem> lstDPIU = objCommonFunction.PopulateDPIU(objParam);
                    lstDPIU.RemoveAt(0);
                    lstDPIU.Insert(0, new SelectListItem { Text = "All", Value = "0" });

                    return Json(new SelectList(lstDPIU, "Value", "Text"), JsonRequestBehavior.AllowGet);
                }
            }
            catch
            {
                return Json(false);
            }
        }


       [Audit]
       [HttpPost]
       public ActionResult AccUserList(FormCollection homeFormCollection)
       {
           AccountantBAL accBAL = new AccountantBAL();
           long totalRecords;

           int Role = Convert.ToInt32(homeFormCollection["roleCode"]);
           int State = Convert.ToInt32(homeFormCollection["stateCode"]);
           int PIU = Convert.ToInt32(homeFormCollection["PIUCode"]);
 
           try
           {
                
               using (CommonFunctions commonFunction = new CommonFunctions())
               {
                   if (!commonFunction.ValidateGridParameters(new GridParams(Convert.ToInt32(homeFormCollection["page"]), Convert.ToInt32(homeFormCollection["rows"]), homeFormCollection["sidx"], homeFormCollection["sord"], Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                   {
                       return null;
                   }
               }
               
               var jsonData = new
               {
                   rows = accBAL.UserList(Convert.ToInt32(homeFormCollection["page"]) - 1,
                                           Convert.ToInt32(homeFormCollection["rows"]),
                                           homeFormCollection["sidx"],
                                           homeFormCollection["sord"], out totalRecords, homeFormCollection["filters"],Role,State,PIU),
                   total = totalRecords <=
                   Convert.ToInt32(homeFormCollection["rows"]) ? 1 : totalRecords /
                   Convert.ToInt32(homeFormCollection["rows"]) + 1,
                   page = Convert.ToInt32(homeFormCollection["page"]),
                   records = totalRecords
               };
               return Json(jsonData);
           }
           catch (Exception ex)
           {
               throw ex;
           }
       }

     
    }
}
