using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PMGSY.Models;
using Newtonsoft.Json;
using PMGSY.Models.UserManager;
using PMGSY.Extensions;
using System.Data.Entity.Validation;
using System.Diagnostics;
using System.Web.Script.Serialization;
using PMGSY.Controllers;
using PMGSY.Common;
using System.Data.SqlClient;
using System.Reflection;
using System.Transactions;

namespace PMGSY.DAL.User_Manager
{
    public class UserManagerDAL
    {
        PMGSYEntities db = null;
        public UserManagerDAL()
        {
            //db = new PMGSYEntities();
        }

        public Array ITNOUserListingDAL(int? page, int? rows, string sidx, string sord, out long totalRecords, string filters)
        {
            List<sp_get_user_list_Result> userItemList = new List<sp_get_user_list_Result>();
            JavaScriptSerializer js = null;
            SearchJsonString test = new SearchJsonString();
            string nameSearch = string.Empty;
            string levelSearch = string.Empty;
            string roleSearch = string.Empty;
            string stateSearch = string.Empty;
            string distSearch = string.Empty;
            string departmentSearch = string.Empty;
            try
            {
                using (var dbContext = new PMGSYEntities())
                {

                    if (filters != null)
                    {
                        js = new JavaScriptSerializer();
                        test = js.Deserialize<SearchJsonString>(filters);

                        foreach (SearchRules item in test.rules)
                        {
                            switch (item.field)
                            {
                                case "UserName": nameSearch = item.data;
                                    break;
                                case "LevelName": levelSearch = item.data;
                                    break;
                                case "RoleName": roleSearch = item.data;
                                    break;
                                case "State": stateSearch = item.data;
                                    break;
                                case "District": distSearch = item.data;
                                    break;
                                case "Department": departmentSearch = item.data;
                                    break;
                                default:
                                    break;
                            }
                        }
                    }
                    int role = PMGSYSession.Current.RoleCode;
                    //userItemList = dbContext.sp_get_user_list().Where(x => x.UserName.ToLower().Contains(nameSearch.Equals(string.Empty) ? "" : nameSearch.ToLower()) &&
                    //                                                  x.RoleName.ToLower().Contains(roleSearch.Equals(string.Empty) ? "" : roleSearch.ToLower()) &&
                    //                                                  x.LevelName.ToLower().Contains(levelSearch.Equals(string.Empty) ? "" : levelSearch.ToLower()) &&
                    //                                                  x.StateName.ToLower().Contains(stateSearch.Equals(string.Empty) ? "" : stateSearch.ToLower()) &&
                    //                                                  x.DistrictName.ToLower().Contains(distSearch.Equals(string.Empty) ? "" : distSearch.ToLower()) &&
                    //                                                  x.Department.ToLower().Contains(departmentSearch.Equals(string.Empty) ? "" : departmentSearch.ToLower())
                    //                                                  ).OrderByDescending(x => x.UserID).ToList<sp_get_user_list_Result>();
                    userItemList = dbContext.sp_get_user_list().Where(x => x.UserName.ToLower().Contains(nameSearch.Equals(string.Empty) ? "" : nameSearch.ToLower()) &&
                                                                     x.RoleName.ToLower().Contains(roleSearch.Equals(string.Empty) ? "" : roleSearch.ToLower()) &&
                                                                     x.LevelName.ToLower().Contains(levelSearch.Equals(string.Empty) ? "" : levelSearch.ToLower()) &&
                                                                     x.StateName.ToLower().Contains(stateSearch.Equals(string.Empty) ? "" : stateSearch.ToLower()) &&
                                                                     x.DistrictName.ToLower().Contains(distSearch.Equals(string.Empty) ? "" : distSearch.ToLower()) &&
                                                                     x.Department.ToLower().Contains(departmentSearch.Equals(string.Empty) ? "" : departmentSearch.ToLower())
                                                                     && x.Mast_State_Code == PMGSYSession.Current.StateCode
                                                                     && (role == 36 ?///ITNO
                                                                                  (x.DefaultRoleID == 2 || x.DefaultRoleID == 7 || x.DefaultRoleID == 8 || x.DefaultRoleID == 22 || x.DefaultRoleID == 38 || x.DefaultRoleID == 21 || x.DefaultRoleID == 26
                                                                                  || x.DefaultRoleID == 33 || x.DefaultRoleID == 37 || x.DefaultRoleID == 26 || x.DefaultRoleID == 46 || x.DefaultRoleID == 54 || x.DefaultRoleID == 55)
                                                                                 : role == 56 ? //ITNORCPLWE
                                                                                 (x.DefaultRoleID == 54 || x.DefaultRoleID == 55)
                                                                                 : role == 47 ?
                                                                                 (x.DefaultRoleID == 37 || x.DefaultRoleID == 38)
                                                                                 : (x.DefaultRoleID == 21 || x.DefaultRoleID == 26 || x.DefaultRoleID == 33))).OrderBy(x => x.LevelID).ThenBy(x => x.DefaultRoleID).ToList<sp_get_user_list_Result>();

                    totalRecords = userItemList.Count();

                    if (sidx.Trim() != string.Empty)
                    {
                        if (sord.ToString() == "asc")
                        {
                            userItemList = userItemList.OrderBy(x => x.UserName).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                        }
                        else
                        {
                            userItemList = userItemList.OrderByDescending(x => x.UserName).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                        }
                    }
                    else
                    {
                        userItemList = userItemList.OrderBy(x => x.UserName).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                    }


                    var result = userItemList.Select(model => new
                    {
                        model.UserID,
                        model.UserName,
                        model.DefaultRoleID,
                        model.RoleName,
                        model.LevelName,
                        model.StateName,
                        model.DistrictName,
                        model.Department,
                        model.MappedUserName,
                        model.IsActive,
                        model.IsLocked,
                        model.Mast_State_Code

                    }).ToArray();


                    return result.Select(model => new
                    {
                        id = model.UserID,
                        cell = new[] {
                                         model.UserName,
                                         model.LevelName,
                                         model.RoleName,
                                         model.StateName,
                                         model.DistrictName,
                                         model.Department,
                                         model.MappedUserName,
                                         //model.IsActive.Equals(true)?"<center><a id='aActive-"+ model.UserID +"' class='ui-icon ui-icon-check' href='#'></a></center>":"<center><a id='aActive-"+ model.UserID +"' class='ui-icon ui-icon-close' href='#'></a></center>",
                                         model.IsLocked.Equals(true)?"<center><a id='aLock-"+ model.UserID +"' class='ui-icon ui-icon-locked' href='#'></a></center>":"<center><a id='aLock-"+ model.UserID +"' class='ui-icon ui-icon-unlocked' href='#'></a></center>",
                                         //model.IsActive.Equals(true)?"Yes":"No",
                                         model.IsLocked.Equals(true)?"Yes":"No",
                                         //"<a href='#'title='Click here to map roles to user' class='ui-icon ui-icon-plusthick ui-align-center' onClick='ShowUserRoleMapping(\"" + model.UserID.ToString().Trim()  +"\"); return false;'>Map Role</a>",
                                         //"<a href='#'title='Click here to update menu rights for user' class='ui-icon ui-icon-plusthick ui-align-center' onClick='UpdateMenuRights(\"" + model.UserID.ToString().Trim()  +"\"); return false;'>Map Menu</a>",
                                         
                                         //(model.DefaultRoleID == 6 || model.DefaultRoleID == 7) 
                                         //       ? !(dbContext.ADMIN_QUALITY_MONITORS.Where(c => c.ADMIN_USER_ID == model.UserID).Any())
                                         //           ?  "<a href='#'title='Click here to map independent users' class='ui-icon ui-icon-plusthick ui-align-center' onClick='MapIndependentUsers(\"" + model.UserID.ToString().Trim()+"$"+model.Mast_State_Code.ToString()+"\"); return false;'>Map Independent Users</a>"
                                         //           : "<a href='#'title='Click here to map independent users' class='ui-icon ui-icon-plusthick ui-align-center' onClick='MapIndependentUsers(\"" + model.UserID.ToString().Trim()+"$"+model.Mast_State_Code.ToString()+"\"); return false;'>Map Independent Users</a>"
                                         //           //:  "<a href='#' class='ui-icon ui-icon-locked ui-align-center'>"
                                         //       : (model.DefaultRoleID == 3 || model.DefaultRoleID == 15)
                                         //           ? !(dbContext.ADMIN_TECHNICAL_AGENCY.Where(c => c.ADMIN_USER_ID == model.UserID).Any())
                                         //               ?  "<a href='#'title='Click here to map independent users' class='ui-icon ui-icon-plusthick ui-align-center' onClick='MapIndependentUsers(\"" + model.UserID.ToString().Trim()  +"$"+model.Mast_State_Code.ToString()+"\"); return false;'>Map Independent Users</a>"
                                         //               : "<a href='#'title='Click here to map independent users' class='ui-icon ui-icon-plusthick ui-align-center' onClick='MapIndependentUsers(\"" + model.UserID.ToString().Trim() +"$"+model.Mast_State_Code.ToString()+"\"); return false;'>Map Independent Users</a>"
                                         //               //:  "<a href='#' class='ui-icon ui-icon-locked ui-align-center'>"
                                         //         : (model.DefaultRoleID == 8) 
                                         //       ? !(dbContext.ADMIN_SQC.Where(c => c.ADMIN_USER_ID == model.UserID).Any())
                                         //           ?  "<a href='#'title='Click here to map independent users' class='ui-icon ui-icon-plusthick ui-align-center' onClick='MapIndependentUsers(\"" + model.UserID.ToString().Trim()+"$"+model.Mast_State_Code.ToString()+"\"); return false;'>Map Independent Users</a>"
                                         //           : "<a href='#'title='Click here to map independent users' class='ui-icon ui-icon-plusthick ui-align-center' onClick='MapIndependentUsers(\"" + model.UserID.ToString().Trim()+"$"+model.Mast_State_Code.ToString()+"\"); return false;'>Map Independent Users</a>"
                                         //           //:  "<a href='#' class='ui-icon ui-icon-locked ui-align-center'>"
                                         //           : "<a href='#' class='ui-icon ui-icon-locked ui-align-center'>",

                                         "<a href='#'title='Click here to reset password' class='ui-icon ui-icon-plusthick ui-align-center' onClick='ResetPassword(\"" + model.UserID.ToString().Trim()  +"\"); return false;'>Reset Password</a>",
                                         //"<a href='#'title='Click here to switch role as a User' class='ui-icon ui-icon-plusthick ui-align-center' onClick='SwitchLogin(\"" + model.UserID.ToString().Trim()  +"\"); return false;'>Switch Login</a>",
                                         //"<center><a id='aEdit-"+ model.UserID +"' class='ui-icon ui-icon-pencil' href='#'></a></center>",
                             }
                    }).ToArray();

                }
            }
            catch (Exception Ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(Ex, HttpContext.Current);
                throw Ex;
            }
        }


        #region Admin Home Page


        /// <summary>
        /// Get all Available Roles
        /// </summary>
        /// <param name="objDetails"></param>
        /// <returns></returns>
        public List<LevelRolesListDTO> GetRoles(LevelRolesMappingDTO objDetails)
        {
            List<LevelRolesListDTO> lstMenuItems = new List<LevelRolesListDTO>();
            List<sp_get_roles_Result> List = null;
            try
            {
                using (var dbContext = new PMGSYEntities())
                {
                    if (objDetails.RoleID == null)   // && (objDetails.RoleID.Contains("L") || objDetails.RoleID.Contains("R"))
                    {
                        objDetails.RoleID = "0"; //objDetails.RoleID.Remove(objDetails.RoleID.Length - 1);
                    }
                    //get the list of the menus for selected parent menu and selected role(to get status if menu is already mapped) 
                    List = dbContext.sp_get_roles(objDetails.RoleID).OrderBy(c => c.RoleName).ToList<sp_get_roles_Result>();  // Remove Last character from string

                    foreach (sp_get_roles_Result item in List)
                    {
                        LevelRolesListDTO objListDTO = new LevelRolesListDTO();
                        objListDTO.RoleID = Convert.ToString(item.RoleID);
                        objListDTO.RoleName = item.RoleName.ToString();
                        objListDTO.Level = Convert.ToInt16(item.level);
                        objListDTO.ParentId = item.parentId;
                        objListDTO.IsLeaf = Convert.ToBoolean(item.IsLeaf.ToString());
                        lstMenuItems.Add(objListDTO);
                    }

                    return lstMenuItems;
                }
            }
            catch (Exception Ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(Ex, HttpContext.Current);

                throw Ex;
            }
            finally
            {
                //dbContext.Dispose();
            }

        }


        /// <summary>
        /// Get Details of particular Role
        /// </summary>
        /// <param name="roleId"></param>
        /// <param name="levelId"></param>
        /// <returns></returns>
        public RoleDetailsModel RoleDetails(Int32 roleId, Int16 levelId)
        {
            //List<RoleDetailsModel> roleDetailsList = new List<RoleDetailsModel>();
            RoleDetailsModel roleDetailsModel = new RoleDetailsModel();
            try
            {
                using (var dbContext = new PMGSYEntities())
                {
                    var query = (from ulm in dbContext.UM_Level_Master
                                 join urlm in dbContext.UM_Role_Level_Mapping on ulm.LevelID equals urlm.LevelID
                                 join urm in dbContext.UM_Role_Master on urlm.RoleID equals urm.RoleID
                                 where urlm.LevelID == levelId && urlm.RoleID == roleId
                                 select new
                                 {
                                     urm.RoleID,
                                     urm.RoleName,
                                     ulm.LevelID,
                                     ulm.LevelName,
                                     ulm.IsActive
                                 }).ToList();


                    foreach (var item in query)
                    {
                        roleDetailsModel.RoleId = item.RoleID;
                        roleDetailsModel.RoleName = item.RoleName;
                        roleDetailsModel.LevelId = item.LevelID;
                        roleDetailsModel.LevelName = item.LevelName;
                        if (item.IsActive == true)
                            roleDetailsModel.IsActive = "Yes";
                        else
                            roleDetailsModel.IsActive = "No";
                    }

                    return roleDetailsModel;
                }
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                throw ex;
            }
        }


        /// <summary>
        /// Get All menu items mapped with particular Role
        /// </summary>
        /// <param name="roleId"></param>
        /// <param name="levelId"></param>
        /// <param name="page"></param>
        /// <param name="rows"></param>
        /// <param name="sidx"></param>
        /// <param name="sord"></param>
        /// <param name="totalRecords"></param>
        /// <returns></returns>
        public Array RoleMenuMappingDetails(Int32 roleId, Int16 levelId, int? page, int? rows, string sidx, string sord, out long totalRecords)
        {
            try
            {
                using (var dbContext = new PMGSYEntities())
                {

                    var roleMenuItemList = dbContext.sp_get_role_menuitems(roleId).ToList<sp_get_role_menuitems_Result>().OrderBy(x => x.MenuID).ToList();  // Remove Last character from string

                    totalRecords = roleMenuItemList.Count();

                    if (sidx.Trim() != string.Empty)
                    {
                        if (sord.ToString() == "asc")
                        {
                            roleMenuItemList = roleMenuItemList.OrderBy(x => x.MenuName).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                        }
                        else
                        {
                            roleMenuItemList = roleMenuItemList.OrderByDescending(x => x.MenuName).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                        }
                    }
                    else
                    {
                        roleMenuItemList = roleMenuItemList.OrderBy(x => x.MenuName).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                    }


                    var result = roleMenuItemList.Select(model => new
                    {
                        model.RoleID,
                        model.RoleName,
                        model.MenuID,
                        model.MenuName,
                        model.ParentID,
                        model.isAdd,
                        model.isEdit,
                        model.isDelete
                    }).ToArray();

                    return result.Select(model => new
                    {
                        id = model.RoleID,
                        cell = new[] {
                                        model.ParentID.ToString(),
                                        model.ParentID.Equals("0")?model.MenuName:model.MenuName,
                                        model.MenuID.ToString(),
                                        model.MenuName,
                                        (model.isAdd.Equals("")?"":model.isAdd+", ")	+ (model.isEdit.Equals("")?"":model.isEdit+", " ) + (model.isDelete.Equals("")?"":model.isDelete),
                                        model.RoleID.ToString(),
                                        model.RoleName
                                    }
                    }).ToArray();
                }
            }
            catch (Exception Ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(Ex, HttpContext.Current);
                throw Ex;
            }


        }



        /// <summary>
        /// User Details for specific Role.
        /// Details from User Profile, User Master, & Mapped Roles
        /// </summary>
        /// <param name="roleId"></param>
        /// <param name="page"></param>
        /// <param name="rows"></param>
        /// <param name="sidx"></param>
        /// <param name="sord"></param>
        /// <param name="totalRecords"></param>
        /// <returns></returns>
        public Array RoleUserMappingDetails(Int32 roleId, int? page, int? rows, string sidx, string sord, out long totalRecords)
        {
            try
            {
                using (var dbContext = new PMGSYEntities())
                {

                    var roleUserItemList = dbContext.sp_get_role_users(roleId).ToList<sp_get_role_users_Result>().ToList();  // Remove Last character from string

                    totalRecords = roleUserItemList.Count();

                    if (sidx.Trim() != string.Empty)
                    {
                        if (sord.ToString() == "asc")
                        {
                            //query = query.OrderBy(x =>x.MAST_STATE_NAME).Skip(Convert.ToInt32(page *rows)).Take(Convert.ToInt32(rows));
                            roleUserItemList = roleUserItemList.OrderBy(x => x.UserName).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                        }
                        else
                        {
                            roleUserItemList = roleUserItemList.OrderByDescending(x => x.UserName).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                        }
                    }
                    else
                    {
                        roleUserItemList = roleUserItemList.OrderBy(x => x.UserName).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                    }


                    var result = roleUserItemList.Select(model => new
                    {
                        model.UserID,
                        //model.Name,
                        model.UserName,
                        //model.City,
                        model.CreationDate,
                        model.IsActive
                    }).ToArray();


                    return result.Select(model => new
                    {
                        id = model.UserID,
                        cell = new[] {
                                        //model.Name,
                                        model.UserName,
                                        //model.City,
                                        model.CreationDate,
                                        model.IsActive.Equals(true)?"Yes":"No"
                             }
                    }).ToArray();
                }
            }
            catch (Exception Ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(Ex, HttpContext.Current);
                throw Ex;
            }
        }


        #endregion


        #region Get List for Roles, Departments, Modules, SubModules

        /// <summary>
        /// Get Role List as per Level ID selected
        /// </summary>
        /// <param name="LevelID"></param>
        /// <returns></returns>
        public List<SelectListItem> GetRoles(int LevelID)
        {
            List<SelectListItem> lstRoles = new List<SelectListItem>();
            db = new PMGSYEntities();
            try
            {
                var query = (from c in db.UM_Role_Master
                             join o in db.UM_Role_Level_Mapping
                             on c.RoleID equals o.RoleID
                             where o.LevelID == LevelID &&
                                   c.IsActive == true
                             select new
                             {
                                 Value = c.RoleID,
                                 Text = c.RoleName
                             }).Distinct().OrderBy(c => c.Text).ToList();


                SelectListItem item = new SelectListItem();
                //item.Text = "Select Role";
                //item.Value = "0";
                //item.Selected = true;
                //lstRoles.Add(item);

                if (LevelID == 0)
                {
                    return lstRoles;
                }
                else
                {
                    foreach (var data in query)
                    {
                        item = new SelectListItem();
                        item.Text = data.Text;
                        item.Value = data.Value.ToString();
                        lstRoles.Add(item);
                    }
                    return lstRoles;
                }
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                return null;
            }
            finally
            {
                db.Dispose();
            }
        }


        /// <summary>
        /// Get Admin departments as per the state selected
        /// </summary>
        /// <param name="stateCode"></param>
        /// <returns></returns>
        public List<SelectListItem> GetNDCode(Int32 stateCode, Int32 districtCode, Int32 levelCode, Int32 roleCode)
        {
            List<SelectListItem> lstNDCode = new List<SelectListItem>();
            db = new PMGSYEntities();
            var query = (IEnumerable<dynamic>)null;
            try
            {
                if (roleCode == 2 || roleCode == 37) //SRRDA =2 and SRRDAOA=37
                {
                    query = (from c in db.ADMIN_DEPARTMENT
                             where c.MAST_STATE_CODE == stateCode &&
                                   c.MAST_ND_TYPE == "S"

                             select new
                             {
                                 Value = c.ADMIN_ND_CODE,
                                 Text = c.ADMIN_ND_NAME
                             }).OrderBy(c => c.Text).ToList();

                    //item.Text = "Select Department";
                    //item.Value = "0";
                    ////item.Selected = true;
                    //lstNDCode.Add(item);


                }
                else if (roleCode == 22 || roleCode == 38) // PIU=22 and PIUOA=38
                {
                    query = (from c in db.ADMIN_DEPARTMENT
                             where c.MAST_STATE_CODE == stateCode &&
                                   c.MAST_ND_TYPE == "D" &&
                             ((districtCode == 0 ? 1 : c.MAST_DISTRICT_CODE) == (districtCode == 0 ? 1 : districtCode))


                             select new
                             {
                                 Value = c.ADMIN_ND_CODE,
                                 Text = c.ADMIN_ND_NAME
                             }).OrderBy(c => c.Text).ToList();

                }
                else if (roleCode == 36) //ITNo=36
                {
                    query = (from c in db.ADMIN_DEPARTMENT
                             join ag in db.MASTER_AGENCY
                              on c.MAST_AGENCY_CODE equals ag.MAST_AGENCY_CODE
                             where c.MAST_STATE_CODE == stateCode &&
                              c.MAST_ND_TYPE == "S" &&
                              ag.MAST_AGENCY_TYPE == "G"
                             select new
                                      {
                                          Value = c.ADMIN_ND_CODE,
                                          Text = c.ADMIN_ND_NAME
                                      }).OrderBy(c => c.Text).ToList();

                }
                else if (roleCode == 47) //ITNoOA=47
                {
                    query = (from c in db.ADMIN_DEPARTMENT
                             join ag in db.MASTER_AGENCY
                              on c.MAST_AGENCY_CODE equals ag.MAST_AGENCY_CODE
                             where c.MAST_STATE_CODE == stateCode &&
                              c.MAST_ND_TYPE == "S" &&
                              ag.MAST_AGENCY_TYPE == "O"
                             select new
                             {
                                 Value = c.ADMIN_ND_CODE,
                                 Text = c.ADMIN_ND_NAME
                             }).OrderBy(c => c.Text).ToList();

                }
                else
                {

                    query = (from c in db.ADMIN_DEPARTMENT
                             where c.MAST_STATE_CODE == stateCode &&
                             ((districtCode == 0 ? 1 : c.MAST_DISTRICT_CODE) == (districtCode == 0 ? 1 : districtCode))
                             select new
                             {
                                 Value = c.ADMIN_ND_CODE,
                                 Text = c.ADMIN_ND_NAME
                             }).OrderBy(c => c.Text).ToList();

                }

                SelectListItem item = new SelectListItem();
                foreach (var data in query)
                {
                    item = new SelectListItem();
                    item.Text = data.Text;
                    item.Value = data.Value.ToString();
                    lstNDCode.Add(item);
                }
                return lstNDCode;
            }

            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                return null;
            }
            finally
            {
                db.Dispose();
            }
        }


        /// <summary>
        /// Populate all modules from Module Master
        /// </summary>
        /// <returns></returns>
        public List<SelectListItem> GetModules()
        {
            List<SelectListItem> lstModules = new List<SelectListItem>();
            db = new PMGSYEntities();
            try
            {
                var query = (from c in db.UM_Module_Master
                             where c.ParentID == 0
                             select new
                             {
                                 Value = c.ModuleID,
                                 Text = c.ModuleName
                             }).OrderBy(c => c.Text).ToList();


                SelectListItem item = new SelectListItem();
                //item.Text = "Select Module";
                //item.Value = "0";
                //item.Selected = true;
                //lstModules.Add(item);

                foreach (var data in query)
                {
                    item = new SelectListItem();
                    item.Text = data.Text;
                    item.Value = data.Value.ToString();
                    lstModules.Add(item);
                }
                return lstModules;
            }

            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                return null;
            }
            finally
            {
                db.Dispose();
            }
        }


        /// <summary>
        /// Populate Modules based on selected Parent Module
        /// </summary>
        /// <param name="moduleId"></param>
        /// <returns></returns>
        public List<SelectListItem> GetSubModules(Int32 moduleId)
        {
            List<SelectListItem> lstSubModules = new List<SelectListItem>();
            db = new PMGSYEntities();
            try
            {
                var query = (from c in db.UM_Module_Master
                             where c.ParentID == moduleId
                             select new
                             {
                                 Value = c.ModuleID,
                                 Text = c.ModuleName
                             }).OrderBy(c => c.Text).ToList();


                SelectListItem item = new SelectListItem();
                //if (moduleId != -1)
                //{
                //    item.Text = "Select Sub Module";
                //    item.Value = "0";
                //    item.Selected = true;
                //    lstSubModules.Add(item);
                //}

                foreach (var data in query)
                {
                    item = new SelectListItem();
                    item.Text = data.Text;
                    item.Value = data.Value.ToString();
                    lstSubModules.Add(item);
                }
                return lstSubModules;
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                return null;
            }
            finally
            {
                db.Dispose();
            }
        }


        /// <summary>
        /// Get  Independent Roles (Set only selected roles)
        /// </summary>
        /// <param name="stateCode"></param>
        /// <returns></returns>
        //public List<SelectListItem> GetIndependentRoles()
        //{
        //    List<SelectListItem> lstIndependentRoles = new List<SelectListItem>();
        //    try
        //    {
        //        var query = (from c in db.UM_Role_Master
        //                     where (c.RoleName.Contains("NQM") || c.RoleName.Contains("SQM") || c.RoleName.Contains("SQC") || c.RoleName.Contains("STA") || c.RoleName.Contains("PTA"))
        //                     && c.IsActive == true
        //                     select new
        //                     {
        //                         Value = c.RoleID,
        //                         Text = c.RoleName 
        //                     }).OrderBy(c => c.Text).ToList();


        //        SelectListItem item = new SelectListItem();
        //        foreach (var data in query)
        //        {
        //            item = new SelectListItem();
        //            item.Text = data.Text;
        //            item.Value = data.Value.ToString();
        //            lstIndependentRoles.Add(item);
        //        }
        //        return lstIndependentRoles;
        //    }

        //    catch (Exception ex)
        //    {
        //        return null;
        //    }
        //}

        /// <summary>
        /// Get User Profile Names
        /// </summary>
        /// <returns></returns>
        public List<SelectListItem> GetUserProfileNames(int roleID, int stateCode)
        {
            List<SelectListItem> lstProfileNames = new List<SelectListItem>();
            CommonFunctions objCommonFunctions = new CommonFunctions();
            var dbContext = new PMGSYEntities();
            try
            {
                SelectListItem item = new SelectListItem();

                if (roleID == 6) //NQM
                {
                    //item.Text = "Select Monitor";
                    //item.Value = "0";
                    //item.Selected = true;
                    //lstProfileNames.Add(item);

                    var existingUserNames = (from aqm in dbContext.ADMIN_QUALITY_MONITORS
                                             where aqm.ADMIN_USER_ID != null
                                             && aqm.ADMIN_QM_TYPE == "I"
                                             select aqm.ADMIN_QM_CODE).ToList();

                    var query = (from c in dbContext.ADMIN_QUALITY_MONITORS
                                 join s in dbContext.MASTER_STATE
                                   on c.MAST_STATE_CODE equals s.MAST_STATE_CODE into joinedState
                                 from stateDetails in joinedState.DefaultIfEmpty()
                                 join d in dbContext.MASTER_DISTRICT
                                   on c.MAST_DISTRICT_CODE equals d.MAST_DISTRICT_CODE into joinedDistrict
                                 from districtDetails in joinedDistrict.DefaultIfEmpty()
                                 where c.ADMIN_QM_TYPE == "I"
                                 && !existingUserNames.Contains(c.ADMIN_QM_CODE)
                                 && c.ADMIN_QM_EMPANELLED == "Y"
                                 && ((stateCode == 0 ? 1 : c.MAST_STATE_CODE) == (stateCode == 0 ? 1 : stateCode)) //new change by deepak 17/06/2014

                                 select new
                                 {
                                     Value = c.ADMIN_QM_CODE,
                                     Text = (c.ADMIN_QM_LNAME.Equals(null) ? "" : c.ADMIN_QM_LNAME) + " " + (c.ADMIN_QM_FNAME.Equals(null) ? " " : c.ADMIN_QM_FNAME) + " " + (c.ADMIN_QM_MNAME.Equals(null) ? " " : c.ADMIN_QM_MNAME) + (stateDetails.MAST_STATE_NAME.Equals(null) ? " " : " , " + stateDetails.MAST_STATE_NAME) + (districtDetails.MAST_DISTRICT_NAME.Equals(null) ? " " : " , " + districtDetails.MAST_DISTRICT_NAME)
                                 }).OrderBy(c => c.Text).ToList();

                    foreach (var data in query)
                    {
                        item = new SelectListItem();
                        item.Text = data.Text;
                        item.Value = data.Value.ToString();
                        lstProfileNames.Add(item);
                    }

                    //lstProfileNames = objCommonFunctions.PopulateMonitors("true", "I", 0).Where(!c.Value.Contains(existingUserNames.Select(c => c.ADMIN_QM_CODE)) );      //NQM
                }
                else if (roleID == 7) //SQM
                {
                    //lstProfileNames = objCommonFunctions.PopulateMonitors("true", "S", 0);      

                    var existingUserNames = (from aqm in dbContext.ADMIN_QUALITY_MONITORS
                                             where aqm.ADMIN_USER_ID != null
                                             && aqm.ADMIN_QM_TYPE == "S" && aqm.MAST_STATE_CODE == stateCode
                                             select aqm.ADMIN_QM_CODE).ToList();

                    var query = (from c in dbContext.ADMIN_QUALITY_MONITORS
                                 join s in dbContext.MASTER_STATE
                                 on c.MAST_STATE_CODE equals s.MAST_STATE_CODE into joinedState
                                 from stateDetails in joinedState.DefaultIfEmpty()
                                 join d in dbContext.MASTER_DISTRICT
                                 on c.MAST_DISTRICT_CODE equals d.MAST_DISTRICT_CODE into joinedDistrict
                                 from districtDetails in joinedDistrict.DefaultIfEmpty()
                                 where c.ADMIN_QM_TYPE == "S"
                                 && !existingUserNames.Contains(c.ADMIN_QM_CODE)
                                 && c.ADMIN_QM_EMPANELLED == "Y"
                                 && ((stateCode == 0 ? 1 : c.MAST_STATE_CODE) == (stateCode == 0 ? 1 : stateCode)) //new change by deepak 17/06/2014

                                 select new
                                 {
                                     Value = c.ADMIN_QM_CODE,
                                     Text = (c.ADMIN_QM_LNAME.Equals(null) ? "" : c.ADMIN_QM_LNAME) + " " + (c.ADMIN_QM_FNAME.Equals(null) ? " " : c.ADMIN_QM_FNAME) + " " + (c.ADMIN_QM_MNAME.Equals(null) ? " " : c.ADMIN_QM_MNAME) + (stateDetails.MAST_STATE_NAME.Equals(null) ? " " : " , " + stateDetails.MAST_STATE_NAME) + (districtDetails.MAST_DISTRICT_NAME.Equals(null) ? " " : " , " + districtDetails.MAST_DISTRICT_NAME)
                                 }).OrderBy(c => c.Text).ToList();

                    foreach (var data in query)
                    {
                        item = new SelectListItem();
                        item.Text = data.Text;
                        item.Value = data.Value.ToString();
                        lstProfileNames.Add(item);
                    }
                }
                else if (roleID == 3) //STA
                {
                    var existingUserNames = (from aqm in dbContext.ADMIN_TECHNICAL_AGENCY
                                             where aqm.ADMIN_USER_ID != null
                                             && aqm.ADMIN_TA_TYPE == "S" && aqm.MAST_STATE_CODE == stateCode
                                             select aqm.ADMIN_TA_CODE).ToList();

                    var query = (from c in dbContext.ADMIN_TECHNICAL_AGENCY
                                 join s in dbContext.MASTER_STATE
                                 on c.MAST_STATE_CODE equals s.MAST_STATE_CODE into joinedState
                                 from stateDetails in joinedState.DefaultIfEmpty()
                                 join d in dbContext.MASTER_DISTRICT
                                 on c.MAST_DISTRICT_CODE equals d.MAST_DISTRICT_CODE into joinedDistrict
                                 from districtDetails in joinedDistrict.DefaultIfEmpty()
                                 where c.ADMIN_TA_TYPE == "S"
                                 && !existingUserNames.Contains(c.ADMIN_TA_CODE)
                                 && ((stateCode == 0 ? 1 : c.MAST_STATE_CODE) == (stateCode == 0 ? 1 : stateCode)) //new change by deepak 17/06/2014

                                 select new
                                 {
                                     Value = c.ADMIN_TA_CODE,
                                     Text = (c.ADMIN_TA_NAME.Equals(null) ? "NA" : c.ADMIN_TA_NAME) + " " +
                                             (c.ADMIN_TA_CONTACT_NAME.Equals(null) ? " " : " (" + c.ADMIN_TA_CONTACT_NAME + ")") +
                                             (stateDetails.MAST_STATE_NAME.Equals(null) ? " " : " , " + stateDetails.MAST_STATE_NAME) +
                                             (districtDetails.MAST_DISTRICT_NAME.Equals(null) ? " " : " , " + districtDetails.MAST_DISTRICT_NAME)


                                 }).OrderBy(c => c.Text).ToList();

                    foreach (var taItems in query)
                    {
                        item = new SelectListItem();
                        item.Text = taItems.Text;
                        item.Value = taItems.Value.ToString();
                        lstProfileNames.Add(item);
                    }
                }
                else if (roleID == 15) //PTA
                {
                    var existingUserNames = (from aqm in dbContext.ADMIN_TECHNICAL_AGENCY
                                             where aqm.ADMIN_USER_ID != null
                                             && aqm.ADMIN_TA_TYPE == "P"
                                             select aqm.ADMIN_TA_CODE).ToList();

                    var query = (from c in dbContext.ADMIN_TECHNICAL_AGENCY
                                 join s in dbContext.MASTER_STATE
                                 on c.MAST_STATE_CODE equals s.MAST_STATE_CODE into joinedState
                                 from stateDetails in joinedState.DefaultIfEmpty()
                                 join d in dbContext.MASTER_DISTRICT
                                 on c.MAST_DISTRICT_CODE equals d.MAST_DISTRICT_CODE into joinedDistrict
                                 from districtDetails in joinedDistrict.DefaultIfEmpty()
                                 where c.ADMIN_TA_TYPE == "P"
                                 && !existingUserNames.Contains(c.ADMIN_TA_CODE)
                                 && ((stateCode == 0 ? 1 : c.MAST_STATE_CODE) == (stateCode == 0 ? 1 : stateCode)) //new change by deepak 17/06/2014

                                 select new
                                 {
                                     Value = c.ADMIN_TA_CODE,
                                     Text = (c.ADMIN_TA_NAME.Equals(null) ? "NA" : c.ADMIN_TA_NAME) + " " +
                                             (c.ADMIN_TA_CONTACT_NAME.Equals(null) ? " " : " (" + c.ADMIN_TA_CONTACT_NAME + ")") +
                                             (stateDetails.MAST_STATE_NAME.Equals(null) ? " " : " , " + stateDetails.MAST_STATE_NAME) +
                                             (districtDetails.MAST_DISTRICT_NAME.Equals(null) ? " " : " , " + districtDetails.MAST_DISTRICT_NAME)

                                 }).OrderBy(c => c.Text).ToList();

                    foreach (var taItems in query)
                    {
                        item = new SelectListItem();
                        item.Text = taItems.Text;
                        item.Value = taItems.Value.ToString();
                        lstProfileNames.Add(item);
                    }
                }
                else if (roleID == 8) //SQC
                {
                    var existingUserNames = (from aqm in dbContext.ADMIN_SQC
                                             where aqm.ADMIN_USER_ID != null
                                             && aqm.ADMIN_QC_TYPE == "S"
                                             select aqm.ADMIN_QC_CODE).ToList();

                    var query = (from c in dbContext.ADMIN_SQC
                                 join s in dbContext.MASTER_STATE
                                 on c.MAST_STATE_CODE equals s.MAST_STATE_CODE into joinedState
                                 from stateDetails in joinedState.DefaultIfEmpty()
                                 join d in dbContext.MASTER_DISTRICT
                                 on c.MAST_DISTRICT_CODE equals d.MAST_DISTRICT_CODE into joinedDistrict
                                 from districtDetails in joinedDistrict.DefaultIfEmpty()
                                 where c.ADMIN_QC_TYPE == "S"
                                 && !existingUserNames.Contains(c.ADMIN_QC_CODE)
                                 && ((stateCode == 0 ? 1 : c.MAST_STATE_CODE) == (stateCode == 0 ? 1 : stateCode)) //new change by deepak 17/06/2014

                                 select new
                                 {
                                     Value = c.ADMIN_QC_CODE,
                                     Text = (c.ADMIN_QC_NAME.Equals(null) ? "NA" : c.ADMIN_QC_NAME) +
                                             (stateDetails.MAST_STATE_NAME.Equals(null) ? " " : " , " + stateDetails.MAST_STATE_NAME) +
                                             (districtDetails.MAST_DISTRICT_NAME.Equals(null) ? " " : " , " + districtDetails.MAST_DISTRICT_NAME)


                                 }).OrderBy(c => c.Text).ToList();

                    foreach (var taItems in query)
                    {
                        item = new SelectListItem();
                        item.Text = taItems.Text;
                        item.Value = taItems.Value.ToString();
                        lstProfileNames.Add(item);
                    }
                }

                return lstProfileNames;
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
        #endregion


        #region Create User, Roles, Menus

        /// <summary>
        /// save new user Entry
        /// </summary>
        /// <returns></returns>
        public bool CreateUser(User_Master um_user_master)
        {
            UM_User_Master obj_User_Master = new UM_User_Master();
            var context = new PMGSYEntities();
            try
            {
                obj_User_Master.UserID = ((from userid in context.UM_User_Master select userid.UserID).Max()) + 1;
                obj_User_Master.UserName = um_user_master.UserName;
                obj_User_Master.LevelID = Convert.ToByte(um_user_master.LevelID);
                obj_User_Master.DefaultRoleID = um_user_master.RoleID;
                obj_User_Master.CreatedBy = PMGSYSession.Current.UserId;
                obj_User_Master.CreationDate = DateTime.Now;
                obj_User_Master.IsActive = true;
                obj_User_Master.IsFirstLogin = true;
                obj_User_Master.IsLocked = false;

                if (um_user_master.Mast_State_Code == 0)
                    obj_User_Master.Mast_State_Code = null;
                else
                    obj_User_Master.Mast_State_Code = um_user_master.Mast_State_Code;

                if (um_user_master.Mast_District_Code == 0)
                    obj_User_Master.Mast_District_Code = null;
                else
                    obj_User_Master.Mast_District_Code = um_user_master.Mast_District_Code;

                if (um_user_master.Admin_ND_Code == 0)
                    obj_User_Master.Admin_ND_Code = null;
                else
                    obj_User_Master.Admin_ND_Code = um_user_master.Admin_ND_Code;

                obj_User_Master.Password = new Login().EncodePassword(obj_User_Master.UserName);
                obj_User_Master.FailedPasswordAttempts = 0;
                obj_User_Master.FailedPasswordAnswerAttempts = 0;
                obj_User_Master.PreferedLanguageID = um_user_master.PreferedLanguageID;
                obj_User_Master.PreferedCssID = um_user_master.PreferedCssID;
                obj_User_Master.MaxConcurrentLoginsAllowed = um_user_master.MaxConcurrentLoginsAllowed;
                obj_User_Master.Remarks = um_user_master.Remarks;


                //Assign userRoleMapping details
                UM_User_Role_Mapping userRoleMapping = new UM_User_Role_Mapping();
                userRoleMapping.ID = ((from uurm in context.UM_User_Role_Mapping select uurm.ID).Max()) + 1;
                userRoleMapping.UserId = obj_User_Master.UserID;
                userRoleMapping.RoleId = um_user_master.RoleID;


                //Assign Security Question Answer
                UM_Security_Question_Answer secQuestionAnswer = new UM_Security_Question_Answer();
                secQuestionAnswer.UserID = obj_User_Master.UserID;
                secQuestionAnswer.PasswordQuestionID = 44;              //Default Question is What is your name?
                secQuestionAnswer.Answer = obj_User_Master.UserName;    //Default Answer is value of UserName
                secQuestionAnswer.SetDate = DateTime.Now;
                secQuestionAnswer.LastUpdatedDate = DateTime.Now;


                //Add all entities
                context.UM_User_Role_Mapping.Add(userRoleMapping);
                context.UM_User_Master.Add(obj_User_Master);
                context.UM_Security_Question_Answer.Add(secQuestionAnswer);
                context.SaveChanges();

                return true;
            }
            catch (Exception ex)
            {
                //Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                ErrorLog.LogError(ex, "UserManager.CreateDAL()");
                return false;
            }
            finally
            {
                if (context != null)
                    context.Dispose();
            }
        }



        /// <summary>
        /// Save newly created roles to the UM_Role_Master
        /// </summary>
        /// <param name="um_user_master"></param>
        /// <returns></returns>
        public bool CreateRole(Role_Master um_role_master)
        {
            UM_Role_Master obj_Role_Master = new UM_Role_Master();
            var context = new PMGSYEntities();
            try
            {
                obj_Role_Master.RoleID = Convert.ToInt16(((from role in context.UM_Role_Master select role.RoleID).Max()) + 1);
                obj_Role_Master.RoleName = um_role_master.RoleName;
                obj_Role_Master.IsActive = true;
                obj_Role_Master.Remark = um_role_master.Remark;

                //Add obj_Role_Master entity
                context.UM_Role_Master.Add(obj_Role_Master);
                context.SaveChanges();

                //Assign roleLevelapping details, for one Role multiple levels may be there.
                UM_Role_Level_Mapping roleLevelapping = new UM_Role_Level_Mapping();
                var levelArr = um_role_master.LevelId.Split(',');
                foreach (var item in levelArr)
                {
                    //roleLevelapping.ID = ((from rlm in context.UM_Role_Level_Mapping select rlm.ID).Max()) + 1;
                    roleLevelapping.LevelID = Convert.ToByte(item);
                    roleLevelapping.RoleID = obj_Role_Master.RoleID;        //current max role Id 

                    //Add UM_Role_Level_Mapping entity
                    context.UM_Role_Level_Mapping.Add(roleLevelapping);
                    context.SaveChanges();
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
                if (context != null)
                    context.Dispose();
            }
        }



        /// <summary>
        /// Update User details
        /// </summary>
        /// <param name="um_user_master"></param>
        /// <returns></returns>
        public bool EditUser(User_Master um_user_master)
        {
            // UM_User_Master obj_User_Master = new UM_User_Master();
            var context = new PMGSYEntities();
            try
            {
                UM_User_Master obj_User_Master = context.UM_User_Master.Find(Convert.ToInt32(um_user_master.UserID));
                obj_User_Master.LevelID = Convert.ToByte(um_user_master.LevelID);
                obj_User_Master.DefaultRoleID = um_user_master.RoleID;
                obj_User_Master.PreferedLanguageID = um_user_master.PreferedLanguageID;
                obj_User_Master.PreferedCssID = um_user_master.PreferedCssID;
                obj_User_Master.MaxConcurrentLoginsAllowed = um_user_master.MaxConcurrentLoginsAllowed;
                obj_User_Master.Remarks = um_user_master.Remarks;

                if (um_user_master.Mast_State_Code == 0)
                    obj_User_Master.Mast_State_Code = null;
                else
                    obj_User_Master.Mast_State_Code = um_user_master.Mast_State_Code;

                if (um_user_master.Mast_District_Code == 0)
                    obj_User_Master.Mast_District_Code = null;
                else
                    obj_User_Master.Mast_District_Code = um_user_master.Mast_District_Code;

                if (um_user_master.Admin_ND_Code == 0)
                    obj_User_Master.Admin_ND_Code = null;
                else
                    obj_User_Master.Admin_ND_Code = um_user_master.Admin_ND_Code;



                context.Entry(obj_User_Master).State = System.Data.Entity.EntityState.Modified;
                context.SaveChanges();


                ///Temporarily Commmented
                ////Delete alrady mapped roles for user
                //List<UM_User_Role_Mapping> userRoleMappingEntity = (from uurm in context.UM_User_Role_Mapping
                //                                                    where uurm.UserId == um_user_master.UserID
                //                                                    select uurm).ToList();

                ////If selected role id not already mapped, then only save new one
                //foreach (var data in userRoleMappingEntity)
                //{
                //    //If alrady Exists then update it, else add new
                //    if (um_user_master.RoleID != data.RoleId)
                //    {
                //        //Assign userRoleMapping details
                //        UM_User_Role_Mapping userRoleMapping = new UM_User_Role_Mapping();
                //        userRoleMapping.UserId = obj_User_Master.UserID;
                //        userRoleMapping.RoleId = um_user_master.RoleID;
                //        context.UM_User_Role_Mapping.Add(userRoleMapping);
                //        context.SaveChanges();
                //    }
                //}

                return true;
            }
            catch (DbEntityValidationException dbEx)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(dbEx, HttpContext.Current);

                foreach (var validationErrors in dbEx.EntityValidationErrors)
                {
                    foreach (var validationError in validationErrors.ValidationErrors)
                    {
                        Trace.TraceInformation("Property: {0} Error: {1}", validationError.PropertyName, validationError.ErrorMessage);
                    }
                }
                return false;
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                return false;
            }
            finally
            {
                if (context != null)
                    context.Dispose();
            }
        }


        /// <summary>
        /// Update Role Details
        /// </summary>
        /// <param name="um_role_master"></param>
        /// <returns></returns>
        public bool EditRole(Role_Master um_role_master)
        {
            var context = new PMGSYEntities();
            try
            {
                UM_Role_Master obj_role_master = context.UM_Role_Master.Find(Convert.ToInt32(um_role_master.RoleID));
                obj_role_master.RoleName = um_role_master.RoleName;
                //obj_role_master.IsActive = um_role_master.IsActive;
                obj_role_master.Remark = um_role_master.Remark;

                context.Entry(obj_role_master).State = System.Data.Entity.EntityState.Modified;


                var levelArr = um_role_master.LevelId.Split(',');

                //Get already mapped levels for user
                List<UM_Role_Level_Mapping> roleLevelMappingEntity = (from urlm in context.UM_Role_Level_Mapping
                                                                      where urlm.RoleID == um_role_master.RoleID
                                                                      select urlm).ToList();



                List<Int16> existingLevelList = new List<Int16>();
                foreach (var existingLevels in roleLevelMappingEntity)
                {
                    existingLevelList.Add(existingLevels.LevelID);
                }

                List<Int16> newLevelList = new List<Int16>();
                foreach (var level in levelArr)
                {
                    newLevelList.Add(Convert.ToInt16(level));
                }

                List<Int16> existingDiffLevelList = existingLevelList.Except(newLevelList).ToList();      //items in existingLevelList that are not in newLevelList
                List<Int16> newDiffLevelList = newLevelList.Except(existingLevelList).ToList();           //items in newLevelList that are not in existingLevelList

                //In Existing But not in new List
                if (existingDiffLevelList.Count > 0)
                {
                    foreach (var item in existingDiffLevelList)
                    {
                        //remove difference items
                        Int16 levelToRemove = item;
                        UM_Role_Level_Mapping roleLevelMapping = context.UM_Role_Level_Mapping.Where(c => c.RoleID == um_role_master.RoleID && c.LevelID == levelToRemove).First();
                        context.UM_Role_Level_Mapping.Remove(roleLevelMapping);
                    }
                }

                //In New But not in Existing
                if (newDiffLevelList.Count > 0)
                {
                    foreach (var item in newDiffLevelList)
                    {
                        UM_Role_Level_Mapping roleLevelMapping = new UM_Role_Level_Mapping();
                        roleLevelMapping.LevelID = item;
                        roleLevelMapping.RoleID = obj_role_master.RoleID;

                        //Update UM_Role_Level_Mapping entity
                        context.UM_Role_Level_Mapping.Add(roleLevelMapping);
                    }
                }

                context.SaveChanges();
                return true;
            }
            catch (DbEntityValidationException dbEx)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(dbEx, HttpContext.Current);

                foreach (var validationErrors in dbEx.EntityValidationErrors)
                {
                    foreach (var validationError in validationErrors.ValidationErrors)
                    {
                        Trace.TraceInformation("Property: {0} Error: {1}", validationError.PropertyName, validationError.ErrorMessage);
                    }
                }
                return false;
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                return false;
            }
            finally
            {
                if (context != null)
                    context.Dispose();
            }
        }

        #endregion


        /// <summary>
        /// Method returnss all details for particular user for displaying Profile
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public UserProfileModel GetUserProfile(Int32 userId)
        {
            UserProfileModel UserProfModel = new UserProfileModel();
            List<sp_get_user_profile_Result> userItemList = null;
            try
            {
                using (var dbContext = new PMGSYEntities())
                {
                    userItemList = dbContext.sp_get_user_profile(userId).ToList<sp_get_user_profile_Result>();  // Remove Last character from string

                    foreach (sp_get_user_profile_Result item in userItemList)
                    {
                        UserProfModel.UserName = item.UserName;
                        UserProfModel.Name = item.Name;
                        UserProfModel.LevelName = item.LevelName;
                        UserProfModel.State = item.StateName;
                        UserProfModel.District = item.DistrictName;

                        if (item.Department == null)
                            UserProfModel.Department = "--";
                        else
                            UserProfModel.Department = item.Department;
                        UserProfModel.PreferredCss = item.CSS;
                        UserProfModel.PreferredLanguage = item.PrefLanguage;
                        UserProfModel.IsActive = item.IsActive;
                    }

                    return UserProfModel;
                }
            }
            catch (Exception Ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(Ex, HttpContext.Current);
                throw Ex;
            }
        }


        /// <summary>
        /// Populate User List in Grid
        /// </summary>
        /// <param name="page"></param>
        /// <param name="rows"></param>
        /// <param name="sidx"></param>
        /// <param name="sord"></param>
        /// <param name="totalRecords"></param>
        /// <returns></returns>
        public Array UserList(int? page, int? rows, string sidx, string sord, out long totalRecords, string filters)
        {
            List<sp_get_user_list_Result> userItemList = new List<sp_get_user_list_Result>();
            JavaScriptSerializer js = null;
            SearchJsonString test = new SearchJsonString();
            string nameSearch = string.Empty;
            string levelSearch = string.Empty;
            string roleSearch = string.Empty;
            string stateSearch = string.Empty;
            string distSearch = string.Empty;
            string departmentSearch = string.Empty;
            try
            {
                using (var dbContext = new PMGSYEntities())
                {

                    if (filters != null)
                    {
                        js = new JavaScriptSerializer();
                        test = js.Deserialize<SearchJsonString>(filters);

                        foreach (SearchRules item in test.rules)
                        {
                            switch (item.field)
                            {
                                case "UserName": nameSearch = item.data;
                                    break;
                                case "LevelName": levelSearch = item.data;
                                    break;
                                case "RoleName": roleSearch = item.data;
                                    break;
                                case "State": stateSearch = item.data;
                                    break;
                                case "District": distSearch = item.data;
                                    break;
                                case "Department": departmentSearch = item.data;
                                    break;
                                default:
                                    break;
                            }
                        }
                    }

                    userItemList = dbContext.sp_get_user_list().Where(x => x.UserName.ToLower().Contains(nameSearch.Equals(string.Empty) ? "" : nameSearch.ToLower()) &&
                                                                      x.RoleName.ToLower().Contains(roleSearch.Equals(string.Empty) ? "" : roleSearch.ToLower()) &&
                                                                      x.LevelName.ToLower().Contains(levelSearch.Equals(string.Empty) ? "" : levelSearch.ToLower()) &&
                                                                      x.StateName.ToLower().Contains(stateSearch.Equals(string.Empty) ? "" : stateSearch.ToLower()) &&
                                                                      x.DistrictName.ToLower().Contains(distSearch.Equals(string.Empty) ? "" : distSearch.ToLower()) &&
                                                                      x.Department.ToLower().Contains(departmentSearch.Equals(string.Empty) ? "" : departmentSearch.ToLower())
                                                                      ).OrderByDescending(x => x.UserID).ToList<sp_get_user_list_Result>();

                    totalRecords = userItemList.Count();

                    if (sidx.Trim() != string.Empty)
                    {
                        if (sord.ToString() == "asc")
                        {
                            userItemList = userItemList.OrderBy(x => x.UserName).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                        }
                        else
                        {
                            userItemList = userItemList.OrderByDescending(x => x.UserName).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                        }
                    }
                    else
                    {
                        userItemList = userItemList.OrderBy(x => x.UserName).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                    }


                    var result = userItemList.Select(model => new
                    {
                        model.UserID,
                        model.UserName,
                        model.DefaultRoleID,
                        model.RoleName,
                        model.LevelName,
                        model.StateName,
                        model.DistrictName,
                        model.Department,
                        model.MappedUserName,
                        model.IsActive,
                        model.IsLocked,
                        model.Mast_State_Code

                    }).ToArray();


                    return result.Select(model => new
                    {
                        id = model.UserID,
                        cell = new[] {
                                         model.UserName,
                                         model.LevelName,
                                         model.RoleName,
                                         model.StateName,
                                         model.DistrictName,
                                         model.Department,
                                         model.MappedUserName,
                                         "<a href='#'title='Click here to switch role as a User' class='ui-icon ui-icon-plusthick ui-align-center' onClick='SwitchLogin(\"" + model.UserID.ToString().Trim()  +"\"); return false;'>Switch Login</a>",
                                         model.IsActive.Equals(true)?"<center><a id='aActive-"+ model.UserID +"' class='ui-icon ui-icon-check' href='#'></a></center>":"<center><a id='aActive-"+ model.UserID +"' class='ui-icon ui-icon-close' href='#'></a></center>",
                                         model.IsLocked.Equals(true)?"<center><a id='aLock-"+ model.UserID +"' class='ui-icon ui-icon-locked' href='#'></a></center>":"<center><a id='aLock-"+ model.UserID +"' class='ui-icon ui-icon-unlocked' href='#'></a></center>",
                                         model.IsActive.Equals(true)?"Yes":"No",
                                         model.IsLocked.Equals(true)?"Yes":"No",
                                         "<a href='#'title='Click here to map roles to user' class='ui-icon ui-icon-plusthick ui-align-center' onClick='ShowUserRoleMapping(\"" + model.UserID.ToString().Trim()  +"\"); return false;'>Map Role</a>",
                                         "<a href='#'title='Click here to update menu rights for user' class='ui-icon ui-icon-plusthick ui-align-center' onClick='UpdateMenuRights(\"" + model.UserID.ToString().Trim()  +"\"); return false;'>Map Menu</a>",
                                         
                                         (model.DefaultRoleID == 6 || model.DefaultRoleID == 7) 
                                                ? !(dbContext.ADMIN_QUALITY_MONITORS.Where(c => c.ADMIN_USER_ID == model.UserID).Any())
                                                    ?  "<a href='#'title='Click here to map independent users' class='ui-icon ui-icon-plusthick ui-align-center' onClick='MapIndependentUsers(\"" + model.UserID.ToString().Trim()+"$"+model.Mast_State_Code.ToString()+"\"); return false;'>Map Independent Users</a>"
                                                    : "<a href='#'title='Click here to map independent users' class='ui-icon ui-icon-plusthick ui-align-center' onClick='MapIndependentUsers(\"" + model.UserID.ToString().Trim()+"$"+model.Mast_State_Code.ToString()+"\"); return false;'>Map Independent Users</a>"
                                                    //:  "<a href='#' class='ui-icon ui-icon-locked ui-align-center'>"
                                                : (model.DefaultRoleID == 3 || model.DefaultRoleID == 15)
                                                    ? !(dbContext.ADMIN_TECHNICAL_AGENCY.Where(c => c.ADMIN_USER_ID == model.UserID).Any())
                                                        ?  "<a href='#'title='Click here to map independent users' class='ui-icon ui-icon-plusthick ui-align-center' onClick='MapIndependentUsers(\"" + model.UserID.ToString().Trim()  +"$"+model.Mast_State_Code.ToString()+"\"); return false;'>Map Independent Users</a>"
                                                        : "<a href='#'title='Click here to map independent users' class='ui-icon ui-icon-plusthick ui-align-center' onClick='MapIndependentUsers(\"" + model.UserID.ToString().Trim() +"$"+model.Mast_State_Code.ToString()+"\"); return false;'>Map Independent Users</a>"
                                                        //:  "<a href='#' class='ui-icon ui-icon-locked ui-align-center'>"
                                                  : (model.DefaultRoleID == 8) 
                                                ? !(dbContext.ADMIN_SQC.Where(c => c.ADMIN_USER_ID == model.UserID).Any())
                                                    ?  "<a href='#'title='Click here to map independent users' class='ui-icon ui-icon-plusthick ui-align-center' onClick='MapIndependentUsers(\"" + model.UserID.ToString().Trim()+"$"+model.Mast_State_Code.ToString()+"\"); return false;'>Map Independent Users</a>"
                                                    : "<a href='#'title='Click here to map independent users' class='ui-icon ui-icon-plusthick ui-align-center' onClick='MapIndependentUsers(\"" + model.UserID.ToString().Trim()+"$"+model.Mast_State_Code.ToString()+"\"); return false;'>Map Independent Users</a>"
                                                    //:  "<a href='#' class='ui-icon ui-icon-locked ui-align-center'>"
                                                    : "<a href='#' class='ui-icon ui-icon-locked ui-align-center'>",

                                         "<a href='#'title='Click here to reset password' class='ui-icon ui-icon-arrowrefresh-1-w ui-align-center' onClick='ResetPassword(\"" + model.UserID.ToString().Trim()  +"\"); return false;'>Reset Password</a>",
                                         "<center><a id='aEdit-"+ model.UserID +"' class='ui-icon ui-icon-pencil' href='#'></a></center>",
                             }
                    }).ToArray();

                }
            }
            catch (Exception Ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(Ex, HttpContext.Current);
                throw Ex;
            }
        }


        /// <summary>
        /// Populate Role List in Grid
        /// </summary>
        /// <param name="page"></param>
        /// <param name="rows"></param>
        /// <param name="sidx"></param>
        /// <param name="sord"></param>
        /// <param name="totalRecords"></param>
        /// <returns></returns>
        public Array RoleList(int? page, int? rows, string sidx, string sord, out long totalRecords, string filters)
        {
            List<sp_get_role_list_Result> roleItemList = new List<sp_get_role_list_Result>();

            JavaScriptSerializer js = null;
            SearchJsonString test = new SearchJsonString();
            string nameSearch = string.Empty;

            try
            {
                if (filters != null)
                {
                    js = new JavaScriptSerializer();
                    test = js.Deserialize<SearchJsonString>(filters);

                    foreach (SearchRules item in test.rules)
                    {
                        switch (item.field)
                        {
                            case "RoleName": nameSearch = item.data;
                                break;
                            default:
                                break;
                        }
                    }
                }

                using (var dbContext = new PMGSYEntities())
                {
                    roleItemList = dbContext.sp_get_role_list().Where(x => x.RoleName.ToLower().Contains(nameSearch.Equals(string.Empty) ? "" : nameSearch.ToLower())
                                                                        ).OrderByDescending(x => x.RoleID).ToList<sp_get_role_list_Result>();

                    totalRecords = roleItemList.Count();

                    if (sidx.Trim() != string.Empty)
                    {
                        if (sord.ToString() == "asc")
                        {
                            roleItemList = roleItemList.OrderBy(x => x.RoleName).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                        }
                        else
                        {
                            roleItemList = roleItemList.OrderByDescending(x => x.RoleName).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                        }
                    }
                    else
                    {
                        roleItemList = roleItemList.OrderBy(x => x.RoleName).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                    }


                    var result = roleItemList.Select(model => new
                    {
                        model.RoleID,
                        model.RoleName,
                        model.LevelID,
                        model.LevelName,
                        model.IsActive,
                        model.Remark
                    }).ToArray();


                    return result.Select(model => new
                    {
                        id = model.RoleID,
                        cell = new[] {
                                         model.RoleName,
                                         model.LevelID.ToString(),
                                         model.LevelName,
                                         model.IsActive,
                                         model.Remark,
                                         "<a href='#'title='Click here to map menu with role' class='ui-icon ui-icon-plusthick ui-align-center' onClick='ShowRoleMenuMapping(\"" + model.RoleID.ToString().Trim()  +"\"); return false;'>Map Menu</a>",
                                         "<center><a id='aEdit-"+ model.RoleID +"' class='ui-icon ui-icon-pencil' href='#'></a></center>"
                             }
                    }).ToArray();
                }
            }
            catch (Exception Ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(Ex, HttpContext.Current);
                throw Ex;
            }
        }


        /// <summary>
        /// Populate Menu List in Grid
        /// </summary>
        /// <param name="page"></param>
        /// <param name="rows"></param>
        /// <param name="sidx"></param>
        /// <param name="sord"></param>
        /// <param name="totalRecords"></param>
        /// <returns></returns>
        public Array MenuList(int? page, int? rows, string sidx, string sord, out long totalRecords, string filters)
        {
            List<sp_get_menu_list_Result> menuItemList = new List<sp_get_menu_list_Result>();

            JavaScriptSerializer js = null;
            SearchJsonString test = new SearchJsonString();
            string nameSearch = string.Empty;

            try
            {
                if (filters != null)
                {
                    js = new JavaScriptSerializer();
                    test = js.Deserialize<SearchJsonString>(filters);

                    foreach (SearchRules item in test.rules)
                    {
                        switch (item.field)
                        {
                            case "MenuName": nameSearch = item.data;
                                break;
                            default:
                                break;
                        }
                    }
                }

                using (var dbContext = new PMGSYEntities())
                {
                    //var menuItemList = dbContext.sp_get_menu_list().ToList<sp_get_menu_list_Result>().ToList();  // Remove Last character from string
                    menuItemList = dbContext.sp_get_menu_list().Where(x => x.MenuName.ToLower().Contains(nameSearch.Equals(string.Empty) ? "" : nameSearch.ToLower())
                                                                        ).OrderByDescending(x => x.MenuID).ToList<sp_get_menu_list_Result>();

                    totalRecords = menuItemList.Count();

                    if (sidx.Trim() != string.Empty)
                    {
                        if (sord.ToString() == "asc")
                        {
                            menuItemList = menuItemList.OrderBy(x => x.MenuName).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                        }
                        else
                        {
                            menuItemList = menuItemList.OrderByDescending(x => x.MenuName).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                        }
                    }
                    else
                    {
                        menuItemList = menuItemList.OrderBy(x => x.MenuName).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                    }


                    var result = menuItemList.Select(model => new
                    {
                        model.MenuID,
                        model.MenuName,
                        model.ParentID,
                        model.ParentName,
                        model.Sequence,
                        model.VerticalLevel,
                        model.IsActive,
                        model.MenucombinationCode
                    }).ToArray();


                    return result.Select(model => new
                    {
                        id = model.MenuID,
                        cell = new[] {
                                        model.MenuID.ToString(),
                                        model.MenuName,
                                        model.ParentID.ToString(),
                                        model.ParentName,
                                        model.Sequence.ToString(),
                                        model.VerticalLevel.ToString(),
                                        model.IsActive,
                                        model.MenucombinationCode,
                                        "<a href='#'title='Click here to map menu with levels' class='ui-icon ui-icon-plusthick ui-align-center' onClick='ShowMenuLevelMapping(\"" + model.MenuID.ToString().Trim()  +"\"); return false;'>Map Levels</a>",
                                        "<center><a id='aEdit-"+ model.MenuID +"' class='ui-icon ui-icon-pencil' href='#'></a></center>"
                             }
                    }).ToArray();
                }
            }
            catch (Exception Ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(Ex, HttpContext.Current);
                throw Ex;
            }
        }


        /// <summary>
        /// Populate Roles with corresponding home pages in Grid
        /// </summary>
        /// <param name="page"></param>
        /// <param name="rows"></param>
        /// <param name="sidx"></param>
        /// <param name="sord"></param>
        /// <param name="totalRecords"></param>
        /// <returns></returns>
        public Array RoleHomePageList(int? page, int? rows, string sidx, string sord, out long totalRecords, string filters)
        {
            try
            {
                using (var dbContext = new PMGSYEntities())
                {
                    JavaScriptSerializer js = null;
                    SearchJsonString test = new SearchJsonString();
                    string nameSearch = string.Empty;

                    if (filters != null)
                    {
                        js = new JavaScriptSerializer();
                        test = js.Deserialize<SearchJsonString>(filters);

                        foreach (SearchRules item in test.rules)
                        {
                            switch (item.field)
                            {
                                case "RoleName": nameSearch = item.data;
                                    break;
                                default:
                                    break;
                            }
                        }
                    }

                    var homePageItemList = dbContext.sp_get_role_homepage_list().Where(x => x.RoleName.ToLower().Contains(nameSearch.Equals(string.Empty) ? "" : nameSearch.ToLower())
                                                                        ).OrderByDescending(x => x.RoleID).ToList<sp_get_role_homepage_list_Result>();  // Remove Last character from string

                    totalRecords = homePageItemList.Count();

                    if (sidx.Trim() != string.Empty)
                    {
                        if (sord.ToString() == "asc")
                        {
                            homePageItemList = homePageItemList.OrderBy(x => x.RoleName).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                        }
                        else
                        {
                            homePageItemList = homePageItemList.OrderByDescending(x => x.RoleName).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                        }
                    }
                    else
                    {
                        homePageItemList = homePageItemList.OrderBy(x => x.RoleName).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                    }


                    var result = homePageItemList.Select(model => new
                    {
                        model.Id,
                        model.RoleID,
                        model.RoleName,
                        model.HomePageId,
                        model.ModuleName,
                        model.ParentID
                    }).ToArray();


                    return result.Select(model => new
                    {
                        id = model.Id,
                        cell = new[] {
                                        model.RoleID.ToString(),
                                        model.RoleName,
                                        model.HomePageId.ToString(),
                                        model.ModuleName,
                                        model.ParentID.ToString(),
                                         "<center><a id='aEdit-"+ model.HomePageId +"' class='ui-icon ui-icon-pencil' href='#'></a></center>"
                             }
                    }).ToArray();
                }
            }
            catch (Exception Ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(Ex, HttpContext.Current);
                throw Ex;
            }
        }

        /// <summary>
        /// Set Home Page for specific Role
        /// </summary>
        /// <param name="roleHomePageModel"></param>
        /// <returns></returns>
        public string RoleHomePage(RoleHomePageModel roleHomePageModel)
        {
            var context = new PMGSYEntities();
            try
            {
                var homePageMaster = (from uhm in context.UM_HomePage_Master
                                      select uhm).ToList();

                foreach (var item in homePageMaster)
                {
                    if (item.RoleId == roleHomePageModel.RoleID)
                    {
                        return "0"; //duplicate entry,(already present)
                    }
                }

                //Assign Home Page details
                var actionMaster = (from uam in context.UM_Action_Master
                                    where uam.ModuleID == roleHomePageModel.SubModuleID
                                    select uam).First();

                UM_HomePage_Master obj_HomePage_Master = new UM_HomePage_Master();
                obj_HomePage_Master.Id = ((from uhm in context.UM_HomePage_Master select uhm.Id).Max()) + 1;
                obj_HomePage_Master.RoleId = Convert.ToInt32(roleHomePageModel.RoleID);
                obj_HomePage_Master.HomePageId = actionMaster.ActionID;            //action mapped to Home Page Id

                //Add all entities
                context.UM_HomePage_Master.Add(obj_HomePage_Master);
                context.SaveChanges();

                return "1";
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                throw ex;
            }
            finally
            {
                if (context != null)
                    context.Dispose();
            }
        }


        /// <summary>
        /// Update Role Details
        /// </summary>
        /// <param name="um_role_master"></param>
        /// <returns></returns>
        public bool EditRoleHomePage(RoleHomePageModel obj_homepage_master)
        {
            var context = new PMGSYEntities();
            try
            {
                UM_HomePage_Master um_homepage_master = context.UM_HomePage_Master.Find(obj_homepage_master.ID);
                um_homepage_master.RoleId = obj_homepage_master.RoleID;

                var actionMaster = (from uam in context.UM_Action_Master
                                    where uam.ModuleID == obj_homepage_master.SubModuleID
                                    select uam).First();

                um_homepage_master.HomePageId = actionMaster.ActionID;
                context.Entry(um_homepage_master).State = System.Data.Entity.EntityState.Modified;
                context.SaveChanges();

                return true;
            }
            catch (DbEntityValidationException dbEx)
            {

                Elmah.ErrorSignal.FromCurrentContext().Raise(dbEx, HttpContext.Current);
                foreach (var validationErrors in dbEx.EntityValidationErrors)
                {
                    foreach (var validationError in validationErrors.ValidationErrors)
                    {
                        Trace.TraceInformation("Property: {0} Error: {1}", validationError.PropertyName, validationError.ErrorMessage);
                    }
                }
                return false;
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                return false;
            }
            finally
            {
                if (context != null)
                    context.Dispose();
            }
        }



        /// <summary>
        /// Populate User List in Grid
        /// </summary>
        /// <param name="page"></param>
        /// <param name="rows"></param>
        /// <param name="sidx"></param>
        /// <param name="sord"></param>
        /// <param name="totalRecords"></param>
        /// <returns></returns>
        //public Array IndependentUsersList(int? page, int? rows, string sidx, string sord, out long totalRecords, string filters)
        //{
        //    List<um_get_independent_user_list_Result> userItemList = new List<um_get_independent_user_list_Result>();
        //    JavaScriptSerializer js = null;
        //    SearchJsonString test = new SearchJsonString();
        //    string roleSearch = string.Empty;
        //    string nameSearch = string.Empty;
        //    string userNameSearch = string.Empty;

        //    try
        //    {
        //        using (var dbContext = new PMGSYEntities())
        //        {
        //            if (filters != null)
        //            {
        //                js = new JavaScriptSerializer();
        //                test = js.Deserialize<SearchJsonString>(filters);

        //                foreach (SearchRules item in test.rules)
        //                {
        //                    switch (item.field)
        //                    {
        //                        case "RoleName": roleSearch = item.data;
        //                            break;
        //                        case "UserName": userNameSearch = item.data;
        //                            break;
        //                        case "FullName": nameSearch = item.data;
        //                            break;
        //                        default:
        //                            break;
        //                    }
        //                }
        //            }

        //            userItemList = dbContext.um_get_independent_user_list().Where(x => x.RoleName.Contains(roleSearch.Equals(string.Empty) ? "" : roleSearch) &&
        //                                                              x.UserName.Contains(userNameSearch.Equals(string.Empty) ? "" : userNameSearch) &&
        //                                                              x.Name.Contains(nameSearch.Equals(string.Empty) ? "" : nameSearch) 
        //                                                              ).ToList<um_get_independent_user_list_Result>();

        //            totalRecords = userItemList.Count();

        //            if (sidx.Trim() != string.Empty)
        //            {
        //                if (sord.ToString() == "asc")
        //                {
        //                    //query = query.OrderBy(x =>x.MAST_STATE_NAME).Skip(Convert.ToInt32(page *rows)).Take(Convert.ToInt32(rows));
        //                    userItemList = userItemList.OrderBy(x => x.UserName).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
        //                }
        //                else
        //                {
        //                    userItemList = userItemList.OrderByDescending(x => x.UserName).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
        //                }
        //            }
        //            else
        //            {
        //                userItemList = userItemList.OrderBy(x => x.UserName).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
        //            }


        //            var result = userItemList.Select(model => new
        //            {
        //                model.Id,
        //                model.Name,
        //                model.UserID,
        //                model.UserName,
        //                model.RoleID,
        //                model.RoleName

        //            }).ToArray();


        //            return result.Select(model => new
        //            {
        //                id = model.UserID,
        //                cell = new[] {
        //                                  model.RoleID.ToString(),
        //                                  model.RoleName,
        //                                  model.Id.ToString(),
        //                                  model.Name,
        //                                  model.UserName,
        //                                 "<center><a id='aEdit-"+ model.UserID +"' class='ui-icon ui-icon-pencil' href='#'></a></center>",
        //                     }
        //            }).ToArray();


        //        }
        //    }
        //    catch (Exception Ex)
        //    {
        //        throw Ex;
        //    }
        //}




        /// <summary>
        /// map usernames for Independent Monitors
        /// </summary>
        /// <param name="um_user_master"></param>
        /// <returns></returns>
        public string IndependentUsersMapping(IndependentUsersModel independentUsersModel)
        {
            var context = new PMGSYEntities();
            try
            {
                if (independentUsersModel.UserRoleID == 6 || independentUsersModel.UserRoleID == 7) //NQM, SQM
                {
                    using (TransactionScope ts = new TransactionScope())
                    {
                        ADMIN_QUALITY_MONITORS obj_admin_quality_monitors = null;

                        var query = (from aqm in context.ADMIN_QUALITY_MONITORS
                                     where aqm.ADMIN_USER_ID == independentUsersModel.UserID
                                     select aqm).ToList();

                        if (query.Count > 0)
                        {
                            //return "duplicate";
                            int QMCode = query.Select(a => a.ADMIN_QM_CODE).FirstOrDefault();

                            obj_admin_quality_monitors = context.ADMIN_QUALITY_MONITORS.Find(QMCode);
                            obj_admin_quality_monitors.ADMIN_USER_ID = null;
                            context.Entry(obj_admin_quality_monitors).State = System.Data.Entity.EntityState.Modified;
                            context.SaveChanges();
                        }

                        obj_admin_quality_monitors = context.ADMIN_QUALITY_MONITORS.Find(independentUsersModel.UserProfileID);

                        obj_admin_quality_monitors.ADMIN_USER_ID = independentUsersModel.UserID;
                        context.Entry(obj_admin_quality_monitors).State = System.Data.Entity.EntityState.Modified;
                        context.SaveChanges();
                        ts.Complete();
                        return "mapped";
                    }

                }
                else if (independentUsersModel.UserRoleID == 3 || independentUsersModel.UserRoleID == 15) // Block for STA / PTA
                {
                    using (TransactionScope ts = new TransactionScope())
                    {
                        ADMIN_TECHNICAL_AGENCY obj_admin_technical_agency = null;

                        var query = (from aqm in context.ADMIN_TECHNICAL_AGENCY
                                     where aqm.ADMIN_USER_ID == independentUsersModel.UserID &&
                                     aqm.ADMIN_TA_TYPE == (independentUsersModel.UserRoleID == 3 ? "S" : "P")
                                     select aqm).ToList();

                        if (query.Count > 0)
                        {
                            int TACode = query.Select(a => a.ADMIN_TA_CODE).FirstOrDefault();
                            obj_admin_technical_agency = context.ADMIN_TECHNICAL_AGENCY.Find(TACode);
                            obj_admin_technical_agency.ADMIN_USER_ID = null;
                            context.Entry(obj_admin_technical_agency).State = System.Data.Entity.EntityState.Modified;
                            context.SaveChanges();

                        }
                        obj_admin_technical_agency = context.ADMIN_TECHNICAL_AGENCY.Find(independentUsersModel.UserProfileID);
                        obj_admin_technical_agency.ADMIN_USER_ID = independentUsersModel.UserID;
                        context.Entry(obj_admin_technical_agency).State = System.Data.Entity.EntityState.Modified;
                        context.SaveChanges();
                        ts.Complete();
                        return "mapped";
                    }
                }
                else if (independentUsersModel.UserRoleID == 8) //SQC
                {
                    using (TransactionScope ts = new TransactionScope())
                    {
                        ADMIN_SQC obj_admin_sqc = null;

                        var query = (from aqm in context.ADMIN_SQC
                                     where aqm.ADMIN_USER_ID == independentUsersModel.UserID
                                     select aqm).ToList();

                        if (query.Count > 0)
                        {
                            int SQCCode = query.Select(a => a.ADMIN_QC_CODE).FirstOrDefault();
                            obj_admin_sqc = context.ADMIN_SQC.Find(SQCCode);
                            obj_admin_sqc.ADMIN_USER_ID = null;
                            context.Entry(obj_admin_sqc).State = System.Data.Entity.EntityState.Modified;
                            context.SaveChanges();
                        }
                        obj_admin_sqc = context.ADMIN_SQC.Find(independentUsersModel.UserProfileID);
                        obj_admin_sqc.ADMIN_USER_ID = independentUsersModel.UserID;
                        context.Entry(obj_admin_sqc).State = System.Data.Entity.EntityState.Modified;
                        context.SaveChanges();
                        ts.Complete();
                        return "mapped";

                    }


                }
                return "mapped";
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                return "Error Ocurred While Processing Your Request.";
            }
            finally
            {
                if (context != null)
                    context.Dispose();
            }
        }

        #region User Log Report
        public Array UserLogListingDAL(int roleCode, int stateCode, int userCode, int year, int month, int page, int rows, string sidx, string sord, out int totalRecords, string filters)
        {
            List<USP_UM_GET_LOGIN_DETAILS_Report_Result> UserLogReportsList;
            var dbContext = new PMGSY.Models.PMGSYEntities();
            try
            {
                int RoleCode = roleCode;
                int StateCode = stateCode;
                int UserCode = userCode;
                int Month = month;
                int Year = year;

                UserLogReportsList = dbContext.Database.SqlQuery<USP_UM_GET_LOGIN_DETAILS_Report_Result>("EXEC [omms].[USP_UM_GET_LOGIN_DETAILS]@RoleCode,@StateCode,@UserCode, @Year,@Month",
                   new SqlParameter("@RoleCode", RoleCode),
                   new SqlParameter("@StateCode", StateCode),
                   new SqlParameter("@UserCode", UserCode),
                   new SqlParameter("@Year", Year),
                   new SqlParameter("@Month", Month)
               ).ToList<USP_UM_GET_LOGIN_DETAILS_Report_Result>();

                totalRecords = UserLogReportsList.Count();
                PropertyInfo propertyInfo = typeof(USP_UM_GET_LOGIN_DETAILS_Report_Result).GetProperty(sidx.Trim());
                if (sidx.Trim() != string.Empty)
                {
                    if (sord.ToString() == "asc")
                    {
                        UserLogReportsList = UserLogReportsList.OrderBy(x => propertyInfo.GetValue(x)).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                    }
                    else
                    {
                        UserLogReportsList = UserLogReportsList.OrderByDescending(x => propertyInfo.GetValue(x)).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                    }
                }
                else
                {
                    UserLogReportsList = UserLogReportsList.OrderBy(x => x.UserName).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                }

                return UserLogReportsList.Select(x => new
                {

                    id = x.UserID.ToString(),
                    cell = new[]{
                        x.UserName.ToString(),
                        x.LoginDateTime==null?"-":  x.LoginDateTime.ToString(),	
                        x.LogoutDateTime==null?"-":x.LogoutDateTime.ToString(),	
                        x.IpAddress==null?"-": x.IpAddress.ToString(),	
                        x.Duration==null?"-":x.Duration.ToString()
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


        public List<SelectListItem> GetUserRoles()
        {
            List<SelectListItem> lstRoles = new List<SelectListItem>();
            db = new PMGSYEntities();
            try
            {
                var query = (from c in db.UM_Role_Master
                             where
                                   c.IsActive == true
                             select new
                             {
                                 Value = c.RoleID,
                                 Text = c.RoleName
                             }).Distinct().OrderBy(c => c.Text).ToList();
                return new SelectList(query.ToList(), "Value", "Text").ToList();

            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                return null;
            }
            finally
            {
                db.Dispose();
            }
        }


        public List<SelectListItem> GetRolewiseUser(int RoleId, int StateCode)
        {
            List<SelectListItem> lstRoles = new List<SelectListItem>();
            db = new PMGSYEntities();
            try
            {
                var query = (from c in db.UM_User_Master
                             join o in db.UM_Role_Master
                             on c.DefaultRoleID equals o.RoleID
                             where c.DefaultRoleID == RoleId &&
                                  (StateCode == 0 ? 1 : c.Mast_State_Code) == (StateCode == 0 ? 1 : StateCode) &&
                                   c.IsActive == true

                             select new
                             {
                                 Value = c.UserID,
                                 Text = c.UserName
                             }).Distinct().OrderBy(c => c.Text).ToList();
                return new SelectList(query.ToList(), "Value", "Text").ToList();

            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                return null;
            }
            finally
            {
                db.Dispose();
            }
        }

        #endregion


        #region LogAccess
        public List<SelectListItem> PopulateLoginYears(bool isPopulateFirstItem = true)
        {
            var dbContext = new PMGSYEntities();

            try
            {
                var years = dbContext.USP_UM_GET_USER_LOGIN_YEAR();
                List<SelectListItem> lstYears = new List<SelectListItem>();


                foreach (var year in years)
                {
                    lstYears.Add(new SelectListItem { Text = year.Value.ToString(), Value = year.Value.ToString() });

                }
                if (isPopulateFirstItem)
                {
                    lstYears.Insert(0, (new SelectListItem { Text = "Select Year", Value = "0" }));
                }
                return lstYears;

            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                dbContext.Dispose();
            }
        }

        public Array UserLogAccessListingDAL(int moduleCode, int year, int month, int page, int rows, string sidx, string sord, out int totalRecords, string filters)
        {
            List<USP_UM_GET_USER_ACCESS_REPORT_DETAILS_Result> UserLogAccessReportsList;
            var dbContext = new PMGSY.Models.PMGSYEntities();
            try
            {
                int ModuleCode = moduleCode;
                int Month = month;
                int Year = year;
                UserLogAccessReportsList = dbContext.Database.SqlQuery<USP_UM_GET_USER_ACCESS_REPORT_DETAILS_Result>("EXEC [omms].[USP_UM_GET_USER_ACCESS_DETAILS]@ModuleId, @Year,@Month",
                   new SqlParameter("@ModuleId", ModuleCode),
                   new SqlParameter("@Year", Year),
                   new SqlParameter("@Month", Month)
               ).ToList<USP_UM_GET_USER_ACCESS_REPORT_DETAILS_Result>();

                totalRecords = UserLogAccessReportsList.Count();

                PropertyInfo propertyInfo = typeof(USP_UM_GET_USER_ACCESS_REPORT_DETAILS_Result).GetProperty(sidx.Trim());
                if (sidx.Trim() != string.Empty)
                {
                    if (sord.ToString() == "asc")
                    {
                        UserLogAccessReportsList = UserLogAccessReportsList.OrderBy(x => propertyInfo.GetValue(x)).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                    }
                    else
                    {
                        UserLogAccessReportsList = UserLogAccessReportsList.OrderByDescending(x => propertyInfo.GetValue(x)).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                    }
                }
                else
                {
                    UserLogAccessReportsList = UserLogAccessReportsList.OrderBy(x => x.UserName).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                }

                return UserLogAccessReportsList.Select(x => new
                {

                    id = x.UserID.ToString(),
                    cell = new[]{
                        x.UserName.ToString(),
                        x.ModuleName==null?"-":  x.ModuleName.ToString(),	
                        x.RequestType==null?"-":x.RequestType.ToString(),	
                        x.URLAccessed==null?"-": x.URLAccessed.ToString(),	
                        x.IPAddress==null?"-":x.IPAddress.ToString(),
                        x.TimeStamp==null?"-":x.TimeStamp.ToString(),
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

        #region PROPOSAL_PIU_UPDATE

        public Array GetDistrictUserListDAL(int? page, int? rows, string sidx, string sord, out long totalRecords, int state, int district, int agency)
        {
            using (var dbContext = new PMGSYEntities())
            {
                try
                {
                    //var lstUsers = dbContext.UM_User_Master.Where(m => m.Mast_State_Code == state && m.Mast_District_Code == district).ToList();

                    var lstUsers = (from item in dbContext.UM_User_Master
                                    join p in dbContext.ADMIN_DEPARTMENT
                                    on item.Admin_ND_Code equals p.ADMIN_ND_CODE
                                    where item.Mast_State_Code == state
                                    && item.Mast_District_Code == district
                                    && p.MAST_AGENCY_CODE == agency
                                    select new
                                    {
                                        item.UserID,
                                        item.UserName,
                                        item.Admin_ND_Code,
                                        p.ADMIN_ND_NAME
                                    }).ToList();

                    totalRecords = lstUsers.Count();

                    if (sidx.Trim() != string.Empty)
                    {
                        if (sord.ToString() == "asc")
                        {
                            lstUsers = lstUsers.OrderBy(x => x.UserName).Skip(Convert.ToInt32((page - 1) * rows)).Take(Convert.ToInt32(rows)).ToList();
                        }
                        else
                        {
                            lstUsers = lstUsers.OrderByDescending(x => x.UserName).Skip(Convert.ToInt32((page - 1) * rows)).Take(Convert.ToInt32(rows)).ToList();
                        }
                    }
                    else
                    {
                        lstUsers = lstUsers.OrderBy(x => x.UserName).Skip(Convert.ToInt32((page - 1) * rows)).Take(Convert.ToInt32(rows)).ToList();
                    }


                    var result = lstUsers.Select(model => new
                    {
                        model.UserID,
                        model.UserName,
                        model.Admin_ND_Code,
                        model.ADMIN_ND_NAME,
                    }).ToArray();


                    return result.Select(model => new
                    {
                        id = model.UserID,
                        cell = new[] {
                                        model.UserID.ToString(),
                                        model.UserName,
                                        model.Admin_ND_Code.ToString(),
                                        model.ADMIN_ND_NAME,
                                        "<a href='#'title='Click here to map menu with levels' class='ui-icon ui-icon-plusthick ui-align-center' onClick='ShowMenuLevelMapping(\"" + model.UserID.ToString().Trim()  +"\"); return false;'>Map Levels</a>"
                             }
                    }).ToArray();
                }
                catch (Exception)
                {
                    totalRecords = 0;
                    return null;
                }
            }
        }

        public Array GetProposalPIUListDAL(int? page, int? rows, string sidx, string sord, out long totalRecords, int state, int district, int agency)
        {
            using (var dbContext = new PMGSYEntities())
            {
                try
                {
                    //var lstUsers = dbContext.UM_User_Master.Where(m => m.Mast_State_Code == state && m.Mast_District_Code == district).ToList();

                    var lstPIUProposal = (from item in dbContext.IMS_SANCTIONED_PROJECTS
                                          join p in dbContext.ADMIN_DEPARTMENT
                                          on item.MAST_DPIU_CODE equals p.ADMIN_ND_CODE
                                          where item.MAST_STATE_CODE == state
                                          && item.MAST_DISTRICT_CODE == district
                                          && p.MAST_AGENCY_CODE == agency
                                          group item by new { item.MAST_DPIU_CODE, p.ADMIN_ND_NAME } into pgroup
                                          select new
                                          {
                                              MAST_DPIU_CODE = pgroup.Key.MAST_DPIU_CODE,
                                              ADMIN_ND_NAME = pgroup.Key.ADMIN_ND_NAME,
                                              PROPOSAL_COUNT = pgroup.Count()
                                          }).Distinct().ToList();

                    totalRecords = lstPIUProposal.Count();

                    if (sidx.Trim() != string.Empty)
                    {
                        if (sord.ToString() == "asc")
                        {
                            lstPIUProposal = lstPIUProposal.OrderBy(x => x.ADMIN_ND_NAME).Skip(Convert.ToInt32((page - 1) * rows)).Take(Convert.ToInt32(rows)).ToList();
                        }
                        else
                        {
                            lstPIUProposal = lstPIUProposal.OrderByDescending(x => x.ADMIN_ND_NAME).Skip(Convert.ToInt32((page - 1) * rows)).Take(Convert.ToInt32(rows)).ToList();
                        }
                    }
                    else
                    {
                        lstPIUProposal = lstPIUProposal.OrderBy(x => x.ADMIN_ND_NAME).Skip(Convert.ToInt32((page - 1) * rows)).Take(Convert.ToInt32(rows)).ToList();
                    }


                    var result = lstPIUProposal.Select(model => new
                    {
                        model.MAST_DPIU_CODE,
                        model.ADMIN_ND_NAME,
                        model.PROPOSAL_COUNT
                    }).ToArray();


                    return result.Select(model => new
                    {
                        cell = new[] {
                                        model.MAST_DPIU_CODE.ToString(),
                                        model.ADMIN_ND_NAME,
                                        model.PROPOSAL_COUNT.ToString(),
                                        "<a href='#'title='Click here to map menu with levels' class='ui-icon ui-icon-plusthick ui-align-center' onClick='PIUMapping(\"" + model.MAST_DPIU_CODE.ToString().Trim()  +"\"); return false;'>Map Levels</a>"
                             }
                    }).ToArray();
                }
                catch (Exception)
                {
                    totalRecords = 0;
                    return null;
                }
            }
        }

        public List<SelectListItem> PopulateDPIUOfDistrict(int district, int agency)
        {
            using (var dbContext = new PMGSYEntities())
            {
                List<SelectListItem> lstDPIU = null;
                try
                {
                    lstDPIU = new SelectList(dbContext.ADMIN_DEPARTMENT.Where(m => m.MAST_DISTRICT_CODE == district && m.MAST_AGENCY_CODE == agency), "ADMIN_ND_CODE", "ADMIN_ND_NAME").ToList();
                    return lstDPIU;
                }
                catch (Exception)
                {
                    return null;
                }
            }
        }

        public bool ChangeProposalPIUMapping(int oldAdminCode, int newAdminCode, int state, int district)
        {
            using (PMGSYEntities dbContext = new PMGSYEntities())
            {
                try
                {
                    var lstProposal = dbContext.IMS_SANCTIONED_PROJECTS.Where(m => m.MAST_DPIU_CODE == oldAdminCode && m.MAST_STATE_CODE == state && m.MAST_DISTRICT_CODE == district).ToList();
                    if (lstProposal.Count() != 0)
                    {
                        lstProposal.ForEach(m => { m.MAST_DPIU_CODE = newAdminCode; m.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"]; m.USERID = PMGSYSession.Current.UserId; });
                        dbContext.SaveChanges();
                    }
                    return true;
                }
                catch (Exception)
                {
                    return false;
                }
            }
        }


        #endregion


        //-----------Error Elmah Section Starts---
        //Added By Rohit Jadhav 9-May-2014

        public Array ErrorDal(int year, int month, int page, int rows, string sidx, string sord, out int totalRecords, string filters)
        {
            List<USP_GET_ERROR_DETAILS_ELMAH_Result> ErrorList;
            var dbContext = new PMGSY.Models.PMGSYEntities();
            try
            {
                int Month = month;
                int Year = year;
                ErrorList = dbContext.Database.SqlQuery<USP_GET_ERROR_DETAILS_ELMAH_Result>("EXEC [omms].[USP_GET_ERROR_DETAILS_ELMAH] @Year,@Month",
                   new SqlParameter("@Year", Year),
                   new SqlParameter("@Month", Month)
               ).ToList<USP_GET_ERROR_DETAILS_ELMAH_Result>();

                totalRecords = ErrorList.Count();

                PropertyInfo propertyInfo = typeof(USP_GET_ERROR_DETAILS_ELMAH_Result).GetProperty(sidx.Trim());
                if (sidx.Trim() != string.Empty)
                {
                    if (sord.ToString() == "asc")
                    {
                        ErrorList = ErrorList.OrderBy(x => propertyInfo.GetValue(x)).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                    }
                    else
                    {
                        ErrorList = ErrorList.OrderByDescending(x => propertyInfo.GetValue(x)).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                    }
                }
                else
                {
                    ErrorList = ErrorList.OrderBy(x => x.User).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                }

                //DateTime previous =DateTime.Now;
                DateTime previousDate = DateTime.Now.AddMonths(-6);

                return ErrorList.Select(x => new
                {

                    id = x.ErrorId.ToString(),
                    cell = new[]{
                        x.User.ToString(),
                        x.Message==null?"-":  x.Message.ToString(),	
                        x.Source==null?"-":x.Source.ToString(),	
                        x.Type==null?"-": x.Type.ToString(),	
                        x.Host==null?"-":x.Host.ToString(),
                        x.StatusCode==null?"-":x.StatusCode.ToString(),
                        x.TimeUtc<previousDate?"<a href='#' title='Click here to delete this record' class='ui-icon ui-icon-trash ui-align-center' onClick=DeleteUserErrorLog('" + x.ErrorId.ToString().Trim() +"'); return false;'>Delete Block</a>":"<span class='ui-icon ui-icon-locked ui-align-center' title='You can not delete this record.' ></span>"
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
        public bool DeleteErrorRecord(String id)
        {
            PMGSYEntities dbContext = new PMGSYEntities();
            //var key = id;
            //System.Guid key = new Guid(id);
            DateTime previousDate = DateTime.Now.AddMonths(-6);
            try
            {
                using (TransactionScope ts = new TransactionScope())
                {

                    if (id.Contains(','))
                    {
                        String[] ids = id.Split(',');

                        foreach (var item in ids)
                        {
                            System.Guid key = new Guid(item);
                            ELMAH_Error errorElmah = dbContext.ELMAH_Error.Find(key);

                            if (errorElmah.TimeUtc < previousDate)
                            {

                                dbContext.ELMAH_Error.Remove(errorElmah);
                                dbContext.SaveChanges();
                            }
                            else
                            {
                                return false;
                            }
                        }
                    }
                    else
                    {
                        System.Guid key = new Guid(id);
                        ELMAH_Error errorElmah = dbContext.ELMAH_Error.Find(key);
                        if (errorElmah.TimeUtc < previousDate)
                        {

                            dbContext.ELMAH_Error.Remove(errorElmah);
                            dbContext.SaveChanges();
                        }
                        else
                        {
                            return false;
                        }
                    }
                    ts.Complete();
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
                dbContext.Dispose();
            }
        }
        //---------Error Ends----

        #region Refresh Account Data
        public bool ExecuteQueryToRefreshAccountData(bool id)
        {
            var context = new PMGSYEntities();
            try
            {
                if (id)
                {
                    ((System.Data.Entity.Infrastructure.IObjectContextAdapter)context).ObjectContext.CommandTimeout = 1200;
                    context.Configuration.LazyLoadingEnabled = false;
                    context.USP_NSP_ACCOUNT_AUTOMATE_QUERY(0, 0, System.DateTime.Now.Month, System.DateTime.Now.Year);
                }
                else
                {
                    ((System.Data.Entity.Infrastructure.IObjectContextAdapter)context).ObjectContext.CommandTimeout = 1200;
                    context.Configuration.LazyLoadingEnabled = false;
                    context.USP_ACC_FPP_AUTOMATE_QUERY(0, 0);

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
                if (context != null)
                    context.Dispose();
            }


        }

        #endregion Refresh Account Data


        #region DATA_DIFFERENCE

        /// <summary>
        /// returns the list of proposals
        /// </summary>
        /// <returns></returns>
        public Array GetProposalDataGapDetailsList(int? page, int? rows, string sidx, string sord, out long totalRecords, int reportNo, FormCollection formCollection)
        {
            try
            {
                using (db = new PMGSYEntities())
                {
                    var lstProposals = db.USP_PROPDATAGAP_REPORT_DETAILS(reportNo, 0, 0, 0, 0, 0, 0, 0, "%", "%", "%", 1).ToList();

                    string stateName = string.Empty;
                    string districtName = string.Empty;
                    string blockName = string.Empty;
                    string dpiuName = string.Empty;
                    int year = 0;
                    int batch = 0;
                    string package = string.Empty;

                    if (formCollection["_search"] == "true")
                    {
                        stateName = formCollection["StateName"] ?? "";
                        districtName = formCollection["DistrictName"] ?? "";
                        blockName = formCollection["BlockName"] ?? "";
                        dpiuName = formCollection["DPIUName"] ?? "";
                        year = Convert.ToInt32(formCollection["Year"]) == null ? 0 : Convert.ToInt32(formCollection["Year"]);
                        batch = Convert.ToInt32(formCollection["Batch"]) == null ? 0 : Convert.ToInt32(formCollection["Batch"]);
                        package = formCollection["PackageId"] ?? "";

                        lstProposals = lstProposals.Where(m => m.MAST_STATE_NAME.StartsWith(stateName) && m.MAST_DISTRICT_NAME.StartsWith(districtName) && m.MAST_BLOCK_NAME.StartsWith(blockName) && m.DPIU_CODE.Contains(dpiuName) && m.IMS_YEAR == (year == 0 ? m.IMS_YEAR : year) && m.IMS_BATCH == (batch == 0 ? m.IMS_BATCH : batch) && m.IMS_PACKAGE_ID.Contains(package)).ToList();
                    }



                    totalRecords = lstProposals.Count();

                    if (sidx.Trim() != string.Empty)
                    {
                        if (sord.ToString() == "asc")
                        {
                            lstProposals = lstProposals.OrderBy(x => x.IMS_PR_ROAD_CODE).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                        }
                        else
                        {
                            lstProposals = lstProposals.OrderByDescending(x => x.IMS_PR_ROAD_CODE).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                        }
                    }
                    else
                    {
                        lstProposals = lstProposals.OrderBy(x => x.IMS_PR_ROAD_CODE).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                    }


                    var result = lstProposals.Select(m => new
                    {
                        m.BRIDGE_LENGTH,
                        m.BRIDGE_NAME,
                        m.BS_AMT,
                        m.BT_LENGTH,
                        m.BW_AMT,
                        m.CARRIAGE_CODE,
                        m.CARRIAGED_WIDTH,
                        m.CC_LENGTH,
                        m.CD_AMT,
                        m.CD_WORKS,
                        m.COMP_STATUS,
                        m.CONNECTIVITY_TYPE,
                        m.DPIU_CODE,
                        m.DPR_STATUS,
                        m.EXISTING_SURFACE,
                        m.EXISTING_SURFACE_CODE,
                        m.FC_AMT,
                        m.FINANCIAL_DATE,
                        m.FREEZE_STATUS,
                        m.HAB_REASON,
                        m.HAB_REASON_CODE,
                        m.HAB_STATUS,
                        m.HIGHER_SPECS,
                        m.HIGHER_STATUS,
                        m.HS_AMT,
                        m.IMS_BATCH,
                        m.IMS_COLLABORATION,
                        m.IMS_DPR_STATUS,
                        m.IMS_EXEC_REMARKS,
                        m.IMS_EXISTING_PACKAGE,
                        m.IMS_FINAL_PAYMENT_FLAG,
                        m.IMS_FREEZE_STATUS,
                        m.IMS_IS_STAGED,
                        m.IMS_ISBENEFITTED_HABS,
                        m.IMS_ISCOMPLETED,
                        m.IMS_LOCK_STATUS,
                        m.IMS_PACKAGE_ID,
                        m.IMS_PARTIAL_LEN,
                        m.IMS_PAV_LENGTH,
                        m.IMS_PR_ROAD_CODE,
                        m.IMS_PROG_REMARKS,
                        m.IMS_PROPOSAL_TYPE,
                        m.IMS_REMARKS,
                        m.IMS_ROAD_FROM,
                        m.IMS_ROAD_NAME,
                        m.IMS_ROAD_TO,
                        m.IMS_SANCTIONED,
                        m.IMS_SHIFT_STATUS,
                        m.IMS_UPGRADE_CONNECT,
                        m.IMS_YEAR,
                        m.IMS_ZP_RESO_OBTAINED,
                        m.LOCK_STATUS,
                        m.LSB_COUNT,
                        m.MAN_AMT1,
                        m.MAN_AMT2,
                        m.MAN_AMT3,
                        m.MAN_AMT4,
                        m.MAN_AMT5,
                        m.MAST_BLOCK_CODE,
                        m.MAST_BLOCK_NAME,
                        m.MAST_DISTRICT_CODE,
                        m.MAST_DISTRICT_NAME,
                        m.MAST_DPIU_CODE,
                        m.MAST_PMGSY_SCHEME,
                        m.MAST_STATE_CODE,
                        m.MAST_STATE_NAME,
                        m.MLA_CONST_CODE,
                        m.MLA_CONST_NAME,
                        m.MP_CONST_CODE,
                        m.MP_CONST_NAME,
                        m.MRD_SANCTION_DATE,
                        m.MRD_SANCTIONED_BY,
                        m.MRD_STATUS,
                        m.OLD_BLOCK_CODE,
                        m.OLD_BLOCK_NAME,
                        m.OLD_PACKAGE,
                        m.OLD_ROAD,
                        m.OW_AMT,
                        m.PACKAGE_STATUS,
                        m.PARTIAL_LEN,
                        m.PAV_AMT,
                        m.PAYMENT_DATE,
                        m.PAYMENT_MADE,
                        m.PAYMENT_STATUS,
                        m.PHYSICAL_DATE,
                        m.PLAN_CN_ROAD_CODE,
                        m.PLAN_CN_ROAD_NAME,
                        m.PROP_REASON,
                        m.PROP_REASON_CODE,
                        m.PROPORSAL_TYPE,
                        m.PROPOSED_STATUS,
                        m.PROPOSED_SURFACE,
                        m.PTA_NAME,
                        m.PTA_REMARKS,
                        m.PTA_SANCTION_DATE,
                        m.PTA_SANCTIONED,
                        m.PTA_SANCTIONED_BY,
                        m.PTA_STATUS,
                        m.PW_AMT,
                        m.RENEWAL_AMT,
                        m.RS_AMT,
                        m.SHARE_PERCENT,
                        m.SHARE_STATUS,
                        m.SHIFT_STATUS,
                        m.STA_REMARKS,
                        m.STA_SANCTION_DATE,
                        m.STA_SANCTIONED,
                        m.STA_SANCTIONED_BY,
                        m.STA_STATUS,
                        m.STAGE_PACKAGE,
                        m.STAGE_PHASE,
                        m.STAGE_ROAD,
                        m.STAGE_STATUS,
                        m.STAGE_YEAR,
                        m.STREAM,
                        m.STREAM_CODE,
                        m.TRAFFIC_CODE,
                        m.TRAFFIC_TYPE,
                        m.VALUEOFWORK_DONE,
                        m.ZP_RESO
                    }).ToArray();

                    return result.Select(m => new
                    {

                        id = m.IMS_PR_ROAD_CODE.ToString(),
                        cell = new[]
                        {
                            m.MAST_STATE_CODE.ToString(),
                            m.MAST_STATE_NAME.ToString(),
                            m.MAST_DISTRICT_CODE.ToString(),
                            m.MAST_DISTRICT_NAME.ToString(),
                            m.MAST_BLOCK_CODE.ToString(),
                            m.MAST_BLOCK_NAME.ToString(),
                            m.IMS_PR_ROAD_CODE.ToString(),
                            m.MAST_DPIU_CODE.ToString(),
                            m.DPIU_CODE.ToString(),
                            m.IMS_YEAR.ToString(),
                            m.IMS_BATCH.ToString(),
                            m.PACKAGE_STATUS.ToString(),
                            m.IMS_PACKAGE_ID.ToString(),
                            m.PROPORSAL_TYPE.ToString(),
                            m.CONNECTIVITY_TYPE.ToString(),
                            m.IMS_COLLABORATION.ToString(),
                            m.IMS_ROAD_NAME.ToString(),
                            m.IMS_ROAD_FROM.ToString(),
                            m.IMS_ROAD_TO.ToString(),
                            m.IMS_PARTIAL_LEN.ToString(),
                            m.PARTIAL_LEN.ToString(),
                            m.BT_LENGTH.ToString(),
                            m.CC_LENGTH.ToString(),
                            m.LSB_COUNT.ToString(),
                            m.BRIDGE_NAME.ToString(),
                            m.BRIDGE_LENGTH.ToString(),
                            m.PLAN_CN_ROAD_CODE.ToString(),
                            m.PLAN_CN_ROAD_NAME.ToString(),
                            m.EXISTING_SURFACE.ToString(),
                            m.PROPOSED_SURFACE.ToString(),
                            m.CARRIAGED_WIDTH.ToString(),
                            m.TRAFFIC_TYPE.ToString(),
                            m.STREAM.ToString(),
                            m.ZP_RESO.ToString(),
                            m.CD_WORKS.ToString(),
                            m.HIGHER_SPECS.ToString(),
                            m.SHARE_PERCENT.ToString(),
                            m.STAGE_STATUS.ToString(),
                            m.STAGE_PHASE.ToString(),
                            m.STAGE_YEAR.ToString(),
                            m.STAGE_PACKAGE.ToString(),
                            m.STAGE_ROAD.ToString(),
                            m.OLD_BLOCK_CODE.ToString(),
                            m.OLD_BLOCK_NAME.ToString(),
                            m.OLD_PACKAGE.ToString(),
                            m.OLD_ROAD.ToString(),
                            m.MP_CONST_NAME.ToString(),
                            m.MLA_CONST_NAME.ToString(),
                            m.HAB_STATUS.ToString(),
                            m.HAB_REASON.ToString(),
                            m.STA_STATUS.ToString(),
                            m.MAST_STATE_NAME.ToString(),
                            m.STA_SANCTION_DATE.ToString(),
                            m.STA_REMARKS.ToString(),
                            m.PTA_STATUS.ToString(),
                            m.PTA_NAME.ToString(),
                            m.PTA_SANCTION_DATE.ToString(),
                            m.PTA_REMARKS.ToString(),
                            m.MRD_STATUS.ToString(),
                            m.PROP_REASON.ToString(),
                            m.MRD_SANCTIONED_BY.ToString(),
                            m.MRD_SANCTION_DATE.ToString(),
                            m.PAV_AMT.ToString(),
                            m.CD_AMT.ToString(),
                            m.PW_AMT.ToString(),
                            m.OW_AMT.ToString(),
                            m.HS_AMT.ToString(),
                            m.FC_AMT.ToString(),
                            m.BW_AMT.ToString(),
                            m.RS_AMT.ToString(),
                            m.BS_AMT.ToString(),
                            m.MAN_AMT1.ToString(),
                            m.MAN_AMT2.ToString(),
                            m.MAN_AMT3.ToString(),
                            m.MAN_AMT4.ToString(),
                            m.MAN_AMT5.ToString(),
                            m.RENEWAL_AMT.ToString(),
                            m.IMS_REMARKS.ToString(),
                            m.IMS_EXEC_REMARKS.ToString(),
                            m.IMS_PROG_REMARKS.ToString(),
                            m.VALUEOFWORK_DONE.ToString(),
                            m.PAYMENT_MADE.ToString(),
                            m.PAYMENT_STATUS.ToString(),
                            m.PAYMENT_DATE.ToString(),
                            m.FINANCIAL_DATE.ToString(),
                            m.PHYSICAL_DATE.ToString(),
                            m.LOCK_STATUS.ToString(),
                            m.FREEZE_STATUS.ToString(),
                            m.SHIFT_STATUS.ToString(),
                            m.DPR_STATUS.ToString(),
                            m.COMP_STATUS.ToString(),
                            m.MAST_PMGSY_SCHEME.ToString()
                        }

                    }).ToArray();


                }
            }
            catch (Exception)
            {
                totalRecords = 0;
                return null;
            }
        }

        #endregion

    }
}