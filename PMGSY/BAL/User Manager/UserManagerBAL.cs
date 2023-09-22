using System;
using System.Collections.Generic;
using System.Linq;

using PMGSY.Models;
using PMGSY.DAL;
using PMGSY.DAL.User_Manager;
using PMGSY.Models.UserManager;
using System.Web;
using System.Web.Mvc;


namespace PMGSY.BAL.User_Manager
{
    public class UserManagerBAL
    {
        //public List<User_Master> GetUserListBAL()
        //{
        //    UserManagerDAL userManagerDAL = new UserManagerDAL();
        //    return userManagerDAL.GetUserListDAL();
        //}
        public Array ITNOUserListingBAL(int? page, int? rows, string sidx, string sord, out long totalRecords, string filters)
        {
            UserManagerDAL umDAL = new UserManagerDAL();
            return umDAL.ITNOUserListingDAL(page, rows, sidx, sord, out totalRecords, filters);
        }


        //Array ITNOUserListingBAL(int year, int month, int collaboration, int page, int rows, string sidx, string sord, out int totalRecords);

        #region Admin Home Page

        /// <summary>
        ///  Get Roles
        /// </summary>
        /// <param name="objDetails"></param>
        /// <returns></returns>
        public List<LevelRolesListDTO> GetRoleItems(LevelRolesMappingDTO objDetails)
        {
            List<LevelRolesListDTO> lstRoleItems = null;
            try
            {
                UserManagerDAL umDAL = new UserManagerDAL();
                lstRoleItems = umDAL.GetRoles(objDetails);
                return lstRoleItems;
            }

            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);

                throw ex;
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="listMenuItems"></param>
        /// <returns></returns>
        public object GetJSONRoleItemCollection(List<LevelRolesListDTO> listRoleItems)
        {
            int level = 0;
            if (HttpContext.Current.Request.Form.AllKeys.Contains("nodeid"))
            {
                if (!HttpContext.Current.Request.Form["nodeid"].Equals(""))
                {
                    level = Convert.ToInt32(HttpContext.Current.Request.Form["n_level"]) + 1;
                }
            }
            try
            {
                var data = (listRoleItems.Select(c => new
                {
                    id = c.RoleID,
                    cell = new[]
                            {
                                c.RoleName,
                                c.Level.ToString(),
                                //c.ParentId.ToString(),
                                (c.ParentId.ToString() == "")? null : c.ParentId.ToString(),
                                c.IsLeaf.ToString(),
                                //(c.RoleID == "1L")? true.ToString() : false.ToString()
                                false.ToString()
                            }
                })).ToArray();

                return data;
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);

                throw ex;
            }
        }


        public RoleDetailsModel RoleDetails(string roleId, string levelId)
        {
            //List<RoleDetailsModel> roleDetailsList = new List<RoleDetailsModel>();
            RoleDetailsModel roleDetailsModel = new RoleDetailsModel();
            UserManagerDAL umDAL = new UserManagerDAL();
            try
            {
                if (roleId != null)
                {
                    if (roleId.Contains("R"))
                    {
                        roleId = roleId.Remove(roleId.Length - 1);
                    }

                    if (levelId.Contains("L"))
                    {
                        levelId = levelId.Remove(levelId.Length - 1);
                    }
                }
                else
                {
                    return roleDetailsModel;     //if roleId & levelId 0  then send Empty List
                }

                roleDetailsModel = umDAL.RoleDetails(Convert.ToInt32(roleId), Convert.ToInt16(levelId));
                return roleDetailsModel;

            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);

                throw ex;
            }
        }



        public Array RoleMenuMappingDetails(string roleId, string levelId, int? page, int? rows, string sidx, string sord, out long totalRecords)
        {
            UserManagerDAL umDAL = new UserManagerDAL();
            try
            {

                if (roleId.Contains("R"))
                {
                    roleId = roleId.Remove(roleId.Length - 1);
                }

                if (levelId.Contains("L"))
                {
                    levelId = levelId.Remove(levelId.Length - 1);
                }

                return umDAL.RoleMenuMappingDetails(Convert.ToInt32(roleId), Convert.ToInt16(levelId), page, rows, sidx, sord, out totalRecords);
                //roleMenuMappingList = umDAL.RoleMenuMappingDetails(Convert.ToInt32(roleId), Convert.ToInt16(levelId));
                //return roleMenuMappingList;

            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);

                throw ex;
            }
        }


        //public object GetJSONRoleMenuCollection(List<RoleMenuMappingModel> listRoleMenuItems)
        //{
        //    try
        //    {
        //        var data = (listRoleMenuItems.Select(c => new
        //        {
        //            id = c.RoleId,
        //            cell = new[]
        //                    {
        //                        c.ParentId.ToString(),
        //                        c.ParentName,
        //                        c.MenuId.ToString(),
        //                        c.MenuName,
        //                        c.RightsPermitted,
        //                        c.RoleId.ToString(),
        //                        c.RoleName
        //                    }
        //        })).ToArray();

        //        return data;
        //    }
        //    catch (Exception Ex)
        //    {
        //        throw Ex;
        //    }
        //}




        public Array RoleUserMappingDetails(string roleId, int? page, int? rows, string sidx, string sord, out long totalRecords)
        {
            UserManagerDAL umDAL = new UserManagerDAL();
            try
            {
                if (roleId.Contains("R"))
                {
                    roleId = roleId.Remove(roleId.Length - 1);
                }

                return umDAL.RoleUserMappingDetails(Convert.ToInt32(roleId), page, rows, sidx, sord, out totalRecords);

            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                throw ex;
            }
        }


        //public object GetJSONRoleUserCollection(List<RoleUserDetailsModel> listRoleUserItems)
        //{
        //    try
        //    {
        //        var data = (listRoleUserItems.Select(c => new
        //        {
        //            id = c.UserId,
        //            cell = new[]
        //                    {
        //                        c.UserId.ToString(),
        //                        c.FullName,
        //                        c.UserName,
        //                        c.City,
        //                        c.CreationDate,
        //                        c.IsActive
        //                    }
        //        })).ToArray();

        //        return data;
        //    }
        //    catch (Exception Ex)
        //    {
        //        throw Ex;
        //    }
        //}


        #endregion

        #region Get Role, NDCode, Modules, SubModules

        public List<SelectListItem> GetRoles(int LevelID)
        {
            UserManagerDAL umDAL = new UserManagerDAL();
            return umDAL.GetRoles(LevelID);
        }

        public List<SelectListItem> GetNDCode(Int32 stateCode, Int32 districtCode, Int32 levelCode, Int32 roleCode)
        {
            UserManagerDAL umDAL = new UserManagerDAL();
            return umDAL.GetNDCode(stateCode, districtCode,levelCode,roleCode);
        }

        public List<SelectListItem> GetModules()
        {
            UserManagerDAL umDAL = new UserManagerDAL();
            return umDAL.GetModules();
        }

        public List<SelectListItem> GetSubModules(Int32 moduleId)
        {
            UserManagerDAL umDAL = new UserManagerDAL();
            return umDAL.GetSubModules(moduleId);
        }

        //public List<SelectListItem> GetIndependentRoles()
        //{
        //    UserManagerDAL umDAL = new UserManagerDAL();
        //    return umDAL.GetIndependentRoles();
        //}

        public List<SelectListItem> GetUserProfileNames(int roleID,int stateCode)
        {
            UserManagerDAL umDAL = new UserManagerDAL();
            return umDAL.GetUserProfileNames(roleID,stateCode);
        }



        #endregion



        #region Create User,Roles, Menus

        /// <summary>
        /// Create User
        /// </summary>
        /// <param name="um_user_master"></param>
        /// <returns></returns>
        public bool CreateUser(User_Master um_user_master)
        {
            UserManagerDAL umDAL = new UserManagerDAL();
            return umDAL.CreateUser(um_user_master);
        }


        /// <summary>
        /// Create ROle
        /// </summary>
        /// <param name="um_role_master"></param>
        /// <returns></returns>
        public bool CreateRole(Role_Master um_role_master)
        {
            UserManagerDAL umDAL = new UserManagerDAL();
            return umDAL.CreateRole(um_role_master);
        }


        /// <summary>
        /// Edit User
        /// </summary>
        /// <param name="um_user_master"></param>
        /// <returns></returns>
        public bool EditUser(User_Master um_user_master)
        {
            UserManagerDAL umDAL = new UserManagerDAL();
            return umDAL.EditUser(um_user_master);
        }


        /// <summary>
        /// Edit Role
        /// </summary>
        /// <param name="um_user_master"></param>
        /// <returns></returns>
        public bool EditRole(Role_Master um_role_master)
        {
            UserManagerDAL umDAL = new UserManagerDAL();
            return umDAL.EditRole(um_role_master);
        }

        #endregion


        /// <summary>
        /// Get User Profile details
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public UserProfileModel GetUserProfile(int userId)
        {
            UserManagerDAL umDAL = new UserManagerDAL();
            try
            {
                return umDAL.GetUserProfile(userId);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                throw ex;
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
            List<User_Master> userList = new List<User_Master>();
            UserManagerDAL umDAL = new UserManagerDAL();
            try
            {
                //userList = umDAL.UserList(page, rows, sidx, sord, out totalRecords);
                //return userList;
                return umDAL.UserList(page, rows, sidx, sord, out totalRecords, filters);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                throw ex;
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
            UserManagerDAL umDAL = new UserManagerDAL();
            try
            {
                return umDAL.RoleList(page, rows, sidx, sord, out totalRecords, filters);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                throw ex;
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
            UserManagerDAL umDAL = new UserManagerDAL();
            try
            {
                return umDAL.MenuList(page, rows, sidx, sord, out totalRecords, filters);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                throw ex;
            }
        }


        /// <summary>
        /// Populate Roles with corresponding home Pages in Grid
        /// </summary>
        /// <param name="page"></param>
        /// <param name="rows"></param>
        /// <param name="sidx"></param>
        /// <param name="sord"></param>
        /// <param name="totalRecords"></param>
        /// <returns></returns>
        public Array RoleHomePageList(int? page, int? rows, string sidx, string sord, out long totalRecords, string filters)
        {
            UserManagerDAL umDAL = new UserManagerDAL();
            try
            {
                return umDAL.RoleHomePageList(page, rows, sidx, sord, out totalRecords, filters);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                throw ex;
            }
        }


        /// <summary>
        /// Map role Home Page
        /// </summary>
        /// <param name="roleHomePageModel"></param>
        /// <returns></returns>
        public string RoleHomePage(RoleHomePageModel roleHomePageModel)
        {
            UserManagerDAL umDAL = new UserManagerDAL();
            return umDAL.RoleHomePage(roleHomePageModel);
        }



        /// <summary>
        /// Update RoleHomePage
        /// </summary>
        /// <param name="um_user_master"></param>
        /// <returns></returns>
        public bool EditRoleHomePage(RoleHomePageModel obj_homepage_master)
        {
            UserManagerDAL umDAL = new UserManagerDAL();
            return umDAL.EditRoleHomePage(obj_homepage_master);
        }

        #region PROPOSAL_PIU_UPDATE

        public Array GetDistrictUserListBAL(int? page, int? rows, string sidx, string sord, out long totalRecords, int state, int district, int agency)
        {
            UserManagerDAL umDAL = new UserManagerDAL();
            return umDAL.GetDistrictUserListDAL(page, rows, sidx, sord, out totalRecords, state, district, agency);
        }

        public Array GetProposalPIUListBAL(int? page, int? rows, string sidx, string sord, out long totalRecords, int state, int district, int agency)
        {
            UserManagerDAL umDAL = new UserManagerDAL();
            return umDAL.GetProposalPIUListDAL(page,rows,sidx,sord, out totalRecords,state, district,agency);
        }

        public bool ChangeProposalPIUMapping(int oldAdminCode, int newAdminCode,int state,int district)
        {
            UserManagerDAL umDAL = new UserManagerDAL();
            return umDAL.ChangeProposalPIUMapping(oldAdminCode,newAdminCode,state,district);
        }

        #endregion




        /// <summary>
        /// Map user name for Independent Users
        /// </summary>
        /// <param name="um_role_master"></param>
        /// <returns></returns>
        public string IndependentUsersMapping(IndependentUsersModel independentUsersModel)
        {
            UserManagerDAL umDAL = new UserManagerDAL();
            return umDAL.IndependentUsersMapping(independentUsersModel);
        }
        /// <summary>
        /// 
        /// User Login Listing
        /// </summary>
        /// <param name="year"></param>
        /// <param name="month"></param>
        /// <param name="page"></param>
        /// <param name="rows"></param>
        /// <param name="sidx"></param>
        /// <param name="sord"></param>
        /// <param name="totalRecords"></param>
        /// <param name="filters"></param>
        /// <returns></returns>
        public Array UserLogListingBAL(int roleCode,int stateCode,int userCode,int year, int month, int page, int rows, string sidx, string sord, out int totalRecords, string filters)
        {
            UserManagerDAL umDAL = new UserManagerDAL();
            return umDAL.UserLogListingDAL(roleCode,stateCode,userCode,year, month, page, rows, sidx, sord, out totalRecords, filters);
        }
        public Array UserLogAccessListingBAL(int moduleCode,int year, int month, int page, int rows, string sidx, string sord, out int totalRecords, string filters)
        {
            UserManagerDAL umDAL = new UserManagerDAL();
            return umDAL.UserLogAccessListingDAL(moduleCode,year, month, page, rows, sidx, sord, out totalRecords, filters);      
        }
        //Added By Rohit Jadhav 9-May-2014
        public Array ErrorBal(int year, int month, int page, int rows, string sidx, string sord, out int totalRecords, string filters)
        {
            UserManagerDAL umDAL = new UserManagerDAL();
            return umDAL.ErrorDal(year, month, page, rows, sidx, sord, out totalRecords, filters); 
        
        }
        //Added By Rohit Jadhav 9-May-2014
        public Boolean DeleteErrorRecord(string id)
        {
            UserManagerDAL umDAL = new UserManagerDAL();
            return umDAL.DeleteErrorRecord(id);
        }

        public bool RefreshAccountData(bool a)
        {
            UserManagerDAL umDAL = new UserManagerDAL();
            return umDAL.ExecuteQueryToRefreshAccountData(a);
        
        }
        /// <summary>
        /// Populate Independent Users List in Grid
        /// </summary>
        /// <param name="page"></param>
        /// <param name="rows"></param>
        /// <param name="sidx"></param>
        /// <param name="sord"></param>
        /// <param name="totalRecords"></param>
        /// <returns></returns>
        //public Array IndependentUsersList(int? page, int? rows, string sidx, string sord, out long totalRecords, string filters)
        //{
        //    UserManagerDAL umDAL = new UserManagerDAL();
        //    try
        //    {
        //        return umDAL.IndependentUsersList(page, rows, sidx, sord, out totalRecords, filters);
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}

        //public object GetJSONUserListCollection(List<User_Master> userListItems)
        //{
        //    try
        //    {
        //        var data = (userListItems.Select(c => new
        //        {
        //            id = c.UserID,
        //            cell = new[]
        //                    {
        //                        c.UserName,
        //                        c.LevelName,
        //                        c.Mast_State_Name,
        //                        c.Mast_District_Name,
        //                        c.Admin_ND_Name,
        //                        c.IsLocked.Equals(true)?"Yes":"No",
        //                        c.IsActive.Equals(true)?"Yes":"No",
        //                        c.Edit = "<center><a id='aEdit-"+ c.UserID +"' class='ui-icon ui-icon-pencil' href='#'></a></center>"
        //                    }
        //        })).ToArray();

        //        return data;
        //    }
        //    catch (Exception Ex)
        //    {
        //        throw Ex;
        //    }
        //}


        //public String AddLevelCombination(RoleLevelMapping roleLevelMapping)
        //{
        //    UserManagerDAL umDAL = new UserManagerDAL();
        //    try
        //    {
        //        return umDAL.AddLevelCombination(roleLevelMapping);
        //    }
        //    catch (Exception Ex)
        //    {
        //        throw Ex;
        //    }
        //}

        //User Login Listing

    }

}
