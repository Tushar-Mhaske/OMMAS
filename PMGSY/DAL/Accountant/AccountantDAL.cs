using PMGSY.Controllers;
using PMGSY.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;

//Added by Pradip Patil 30-12-2016
namespace PMGSY.DAL.Accountant
{
    public class AccountantDAL
    {
        public Array UserList(int? page, int? rows, string sidx, string sord, out long totalRecords, string filters, int Role, int State, int PIU)
        {
            List<usp_acc_get_user_list_Result> userItemList = new List<usp_acc_get_user_list_Result>();
            
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

                    userItemList = dbContext.usp_acc_get_user_list(Role,State,PIU).Where(x => x.UserName.ToLower().Contains(nameSearch.Equals(string.Empty) ? "" : nameSearch.ToLower()) &&
                                                                      x.RoleName.ToLower().Contains(roleSearch.Equals(string.Empty) ? "" : roleSearch.ToLower()) &&
                                                                      x.LevelName.ToLower().Contains(levelSearch.Equals(string.Empty) ? "" : levelSearch.ToLower()) &&
                                                                      x.StateName.ToLower().Contains(stateSearch.Equals(string.Empty) ? "" : stateSearch.ToLower()) &&
                                                                      x.DistrictName.ToLower().Contains(distSearch.Equals(string.Empty) ? "" : distSearch.ToLower()) &&
                                                                      x.Department.ToLower().Contains(departmentSearch.Equals(string.Empty) ? "" : departmentSearch.ToLower())
                                                                      ).OrderByDescending(x => x.UserID).ToList<usp_acc_get_user_list_Result>();

                    //userItemList = dbContext.usp_acc_get_user_list(Role, State, PIU).ToList<usp_acc_get_user_list_Result>();

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
                        model.LevelName,
                        model.DefaultRoleID,
                        model.RoleName,
                        model.StateName,
                        model.DistrictName,
                        model.Department,
                        model.MappedUserName,
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
                                         "<a href='#'title='Click here to switch role as a User' class='ui-icon ui-icon-plusthick ui-align-center' onClick='switchUserLogin(\"" + model.UserID.ToString().Trim()  +"\"); return false;'>Switch Login</a>",
                             }
                    }).ToArray();

                }
            }
            catch (Exception Ex)
            {
                throw Ex;
            }
        }
    }
}