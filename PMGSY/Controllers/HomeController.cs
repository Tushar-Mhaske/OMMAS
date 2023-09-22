using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PMGSY.Extensions;
using PMGSY.Models;
using System.Data.Entity;
using System.Text;
using System.ComponentModel;
using System.Reflection;
using System.Web.Script.Serialization;
using System.Collections;
using System.Data.SqlClient;
using Newtonsoft.Json;
using System.Text.RegularExpressions;
using PMGSY.Common;
using System.Web.UI.WebControls;
using Newtonsoft.Json.Linq;
using PMGSY.BAL.Menu;
using System.Web.Routing;
using PMGSY.BAL.Accounts;
using PMGSY.BAL.Home;
using PMGSY.Models.Home;

namespace PMGSY.Controllers
{
    [RequiredAuthentication]
    public class HomeController : Controller
    {
        IAccountsBAL objAccountsBAL = null;

        #region Home

        public HomeController()
        {
            PMGSYSession.Current.ModuleName = "Home";
        }

        [Audit]
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        public ActionResult Index(String parameter, String hash, String key)
        {
            try
            {
                // If Role is Accountant || Authorized Signatory || Empowered Officer || Account Reports Then redirect to account specific page 
                // Else Redirect to Home Page for each Role
                if (PMGSYSession.Current.RoleCode == 21 || PMGSYSession.Current.RoleCode == 33 || PMGSYSession.Current.RoleCode == 26 || PMGSYSession.Current.RoleCode == 53 || PMGSYSession.Current.RoleCode == 66)
                {
                    objAccountsBAL = new AccountsBAL();
                    if (!String.IsNullOrEmpty(parameter) && !String.IsNullOrEmpty(hash) && !String.IsNullOrEmpty(key))
                    {
                        string[] strParameters = URLEncrypt.DecryptParameters(new string[] { parameter, hash, key });
                        PMGSYSession.Current.FundType = strParameters[0];
                    }
                    List<string> notificationList = objAccountsBAL.GetNotificationsBAL();

                    ViewBag.NotificationList = notificationList;
                    ViewBag.LevelId = PMGSYSession.Current.LevelId;

                    return View();
                }
                else
                {
                    if (PMGSYSession.Current.RoleCode == 25)
                    {
                        //old code to redirect QM dashboard.
                        //return RedirectToAction("DashboardLayout", "Dashboard");
                        //new code 26Nov2014 to disp on fund type change on mrd user for acc reports.
                        return RedirectToAction("ListProposal", "Proposal");                         
                    }
                    else
                    {
                        return Redirect(PMGSYSession.Current.HomePageURL);
                    }
                }
               
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return Redirect("login/error");
            }
        }


        /// <summary>
        /// Default page
        /// If no url is mapped to particular role as Home Page then redirected to this action.
        /// </summary>
        /// <returns></returns>
        [Audit]
        public ActionResult Default()
        {
            return View();
        }


        /// <summary>
        /// action to chage the fund type
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        [Audit]
        public ActionResult ChangeFundType(String id)
        {
            try
            {
                if (!String.IsNullOrEmpty(id))
                {
                    if (id != "M" && id != "A" && id != "P")
                    {
                        return Json(new
                        {
                            Status = "0"
                        });
                    }
                    else 
                    {
                        PMGSYSession.Current.FundType = id;//set value of fundtype
                    }
                }

                return Json(new
                {
                    Status = "1", //sucess
                    encryptedurl = URLEncrypt.EncryptParameters(new string[] { id })
                });

            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return Redirect("login/error");
            }
        }


        #region for encryption starts here

        [AcceptVerbs(HttpVerbs.Post)]
        [RequiredAuthentication]
        [Audit]
        public JsonResult encryptUrl(string id1, string id2)
        {

            try
            {
                string[] idlist = id1.ToString().Trim().Split(',');
                string strEncrypted = URLEncrypt.EncryptParameters(idlist);
                return new JsonResult
                {
                    Data = Server.HtmlEncode(strEncrypted)
                };

            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                throw ex;
            }

        }

        [AcceptVerbs(HttpVerbs.Post)]
        [Audit]
        public JsonResult encryptUrlReqAuthExcluded(string id1, string id2)
        {

            try
            {
                string[] idlist = id1.ToString().Trim().Split(',');
                string strEncrypted = URLEncrypt.EncryptParameters(idlist);
                return new JsonResult
                {
                    Data = Server.HtmlEncode(strEncrypted)
                };

            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                throw ex;
            }


        }


        #endregion of encryption

        [RequiredAuthentication(Order = 1)]
        [Audit]
        public JsonResult DecryptUrl(String parameter, String hash, String key)
        {
            try
            {
                String[] decryptedParams = URLEncrypt.DecryptParameters(new String[] { parameter, hash, key });
                String strReturnVal = String.Empty;
                foreach (String item in decryptedParams)
                {
                    strReturnVal += "$" + item;
                }
                strReturnVal = strReturnVal.Trim(new Char[] { '$' });
                return new JsonResult
                {
                    Data = Server.HtmlEncode(strReturnVal)
                };
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                throw ex;
            }

        }


        /// <summary>
        /// On click of menu, Set ModuleName in session
        /// Called from Layout.js -- LoadPage() Method
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        [Audit]
        public JsonResult SetModuleName(string id)
        {
            if (id == null)
            {
                return Json(new { Success = false, Data = "" });
            }
            else
            {
                PMGSYSession.Current.ModuleName = id;
                return Json(new { Success = true, Data = PMGSYSession.Current.ModuleName });
            }

            
        }


        /// <summary>
        /// On check of Scheme, set scheme selected to Session
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        [Audit]
        public JsonResult SetScheme(string id)
        {
            if (id == null)
            {
                return Json(new { Success = false, Data = "" });
            }
            else
            {
                PMGSYSession.Current.PMGSYScheme = Convert.ToByte(id);
                return Json(new { Success = true, Data = PMGSYSession.Current.PMGSYScheme });
            }


        } 


//        public Object CreateGenericList(Type typeX)
//        {
//            Type listType = typeof(List<>);
//            Type[] typeArgs = { typeX };
//            Type genericType = listType.MakeGenericType(typeArgs);
//            object o = Activator.CreateInstance(genericType);
//            return o;
//        } 
        
//        public ActionResult TestGrid(GridSettings objGridSetting)
//        {
//            GridDataBiind objGridDataBind = new GridDataBiind();
//            string spname = Request.Params["storedprocedurename"];
//            string columnNames = Request.Params["columnlist"];
            
//            objGridSetting.SpParameters = objGridDataBind.ConvertToIDictionary(Request.Params["storedprocedureparameters"]);
//            objGridSetting.SPName = spname;
//            objGridSetting.Columns = columnNames;

            
//            #region Generic Code
//            //PMGSYEntities _context = new PMGSYEntities();
//            //Type type = Type.GetType(string.Format("PMGSY.Models.{0}", spname));
//            //var method = _context.GetType().GetMember("Set")
//            //      .Cast<MethodInfo>()
//            //      .Where(x => x.IsGenericMethodDefinition)
//            //      .FirstOrDefault();
//            //var genericMethod = method.MakeGenericMethod(type);
//            //dynamic invokeSet = genericMethod.Invoke(_context, null);                
//            //var varUserProfiles = Enumerable.ToList(invokeSet);
//            #endregion
//            return Content(objGridDataBind.GridData(objGridSetting).ToString(), "application/json");             
//        }

//        public ActionResult Customer()
//        {
//            PMGSYEntities context = new PMGSYEntities();

//            ViewData["ComapnyName"] = (context.Customers.Select(r => r.CompanyName).Distinct()).ToArray<string>();

//            return View("Grid");
//        }

//        public ActionResult GetCustomers( GridSettings objGridSetting )
//        {
//            //PMGSYEntities context = new PMGSYEntities();
//            //ViewData["ComapnyName"] = (context.Customers.Select(r => r.CompanyName).Distinct()).ToArray<string>();

//            GridDataBiind objGridDataBind = new GridDataBiind();
//            string spname = Request.Params["storedprocedurename"];
//            string columnNames = Request.Params["columnlist"];
            
            
            
//            //objGridSetting.SpParameters = objGridDataBind.ConvertToIDictionary(Request.Params["storedprocedureparameters"]);
//            objGridSetting.SPName = spname;
//            objGridSetting.Columns = columnNames;

//            return Content(objGridDataBind.GridData(objGridSetting).ToString(), "application/json");             
//        }

//        public ActionResult EditUser(string id)
//        {
//            return null;
//        }

//        #region ConvertListToDataTable 
//        public static T CreateItem<T>(DataRow row)
//        {
//            T obj = default(T);
//            if (row != null)
//            {
//                obj = Activator.CreateInstance<T>();

//                foreach (DataColumn column in row.Table.Columns)
//                {
//                    PropertyInfo prop = obj.GetType().GetProperty(column.ColumnName);
//                    try
//                    {
//                        object value = row[column.ColumnName];
//                        prop.SetValue(obj, value, null);
//                    }
//                    catch
//                    {
//                        // You can log something here
//                        throw;
//                    }
//                }
//            }

//            return obj;
//        }

//        public static DataTable CreateTable<T>()
//        {
//            Type entityType = typeof(T);
//            DataTable table = new DataTable(entityType.Name);
//            PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(entityType);

//            foreach (PropertyDescriptor prop in properties)
//            {
//                table.Columns.Add(prop.Name, prop.PropertyType);
//            }

//            return table;
//        }
     
//        public static DataTable ConvertTo<T>(IList<T> list)
//        {
//            DataTable table = CreateTable<T>();
//            Type entityType = typeof(T);
//            PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(entityType);

//            foreach (T item in list)
//            {
//                DataRow row = table.NewRow();

//                foreach (PropertyDescriptor prop in properties)
//                {
//                    row[prop.Name] = prop.GetValue(item);
//                }

//                table.Rows.Add(row);
//            }

//            return table;
//        }
//        #endregion

//        public ActionResult About()
//        {
//            string userName = PMGSYSession.Current.UserName;
//            ViewBag.Message = "Your app description page.";
            
//            return View();
//        }

//        public ActionResult Contact()
//        {
//            ViewBag.Message = "Your contact page.";

//            return View();
//        }


//        public ActionResult Trouble()
//        {
//            return View();
//            //return View("Error");
//        }

//        public class test
//        {
//            public int a;
//            public string b;
//        }

//        public ActionResult TestCss()
//        {
//            return View();
//        }

        public ActionResult Test()
        {
            return View();
        }


//        public ActionResult Test()
//        {
//            var abc = Request.IsAjaxRequest();
//            return View();

//        }

//        [HttpPost]
//        public ActionResult Test(LoginModel model, string returnUrl, FormCollection frm)
//        {


//            // If we got this far, something failed, redisplay form
//            ModelState.AddModelError("", "The user name or password provided is incorrect.");
//            return View(model);
//        }


//        public ActionResult TableAsJqgrid()
//        {
//            return View();

//        }


        [Audit]
        public ActionResult SetFundType(String parameter, String hash, String key)
        {
            try
            {
                string[] strParameters = URLEncrypt.DecryptParameters(new string[] { parameter, hash, key });
                PMGSYSession.Current.FundType = strParameters[0];
                int roleCode = PMGSYSession.Current.RoleCode;
                string url = string.Empty;


                if (roleCode == 10)
                {
                    url = "/Bank/BankReconciliation";
                }
                else if (roleCode == 46)
                {

                    url = "/Definalization/GetDefinalizationView";
                }

                return Json(new { status = true, url = url }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return Redirect("login/error");
            }

        }


        /// <summary>
        /// View to render User Manual Pdf
        /// </summary>
        /// <returns></returns>
        public ActionResult PdfViewer()
        {
            return View();
        }


        /// <summary>
        /// Render Video For User Manual
        /// </summary>
        /// <returns></returns>
        public ActionResult UMVideoStream()
        {
            return View();
        }

#endregion

        #region CBR Map Data

        /// <summary>
        /// Render Highmap with CBR Details 
        /// </summary>
        /// <returns></returns>
        public ActionResult CBRDetailsMap()
        {
            return View();
        }


        /// <summary>
        /// Get CBR Min Max Details
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public JsonResult GetCBRData()
        {
            PMGSYEntities dbContext = new PMGSYEntities();
            try
            {
                HomeBAL objBAL = new HomeBAL();
                List<USP_HIGHMAP_CBR_DETAILS_Result> lstResult = objBAL.GetCBRDataBAL(Convert.ToInt32(Request.Params["state"]), Convert.ToInt32(Request.Params["district"]), Convert.ToInt32(Request.Params["block"]));
                return Json(lstResult, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                //using (System.IO.StreamWriter sw = System.IO.File.AppendText(System.IO.Path.Combine("D:\\Highmap\\ErrorLog_" + System.DateTime.Now.Date.ToString("dd_MM_yyyy") + ".txt", "")))
                //{
                //    sw.WriteLine("Date :" + DateTime.Now.ToString());
                //    sw.WriteLine("Method : " + "GetData()");
                //    sw.WriteLine("Exception : " + ex.StackTrace.ToString());

                //    if (ex.InnerException != null)
                //        sw.WriteLine("Exception : " + ex.InnerException.StackTrace.ToString());
                //    sw.WriteLine("---------------------------------------------------------------------------------------");
                //    sw.Close();
                //}
                return new JsonResult
                {
                    Data = "Error"
                };
            }
        }


        /// <summary>
        /// Get Column Chart Data
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public JsonResult GetCBRColumnChartData()
        {
            PMGSYEntities dbContext = new PMGSYEntities();
            try
            {
                HomeBAL objBAL = new HomeBAL();
                List<CBRColumnChartModel> lstChart = objBAL.GetCBRColumnChartDataBAL(Convert.ToInt32(Request.Params["state"]), Convert.ToInt32(Request.Params["district"]), Convert.ToInt32(Request.Params["block"]));

                return new JsonResult
                {
                    Data = lstChart
                };
            }
            catch (Exception ex)
            {
                //using (System.IO.StreamWriter sw = System.IO.File.AppendText(System.IO.Path.Combine("D:\\Highmap\\ErrorLog_" + System.DateTime.Now.Date.ToString("dd_MM_yyyy") + ".txt", "")))
                //{
                //    sw.WriteLine("Date :" + DateTime.Now.ToString());
                //    sw.WriteLine("Method : " + "GetColumnChartData()");
                //    sw.WriteLine("Exception : " + ex.StackTrace.ToString());

                //    if (ex.InnerException != null)
                //        sw.WriteLine("Exception : " + ex.InnerException.StackTrace.ToString());
                //    sw.WriteLine("---------------------------------------------------------------------------------------");
                //    sw.Close();
                //}
                return new JsonResult
                {
                    Data = "Error"
                };
            }
        }


        /// <summary>
        /// Get CBR Grid Data
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public JsonResult GetCBRGridData()
        {
            PMGSYEntities dbContext = new PMGSYEntities();
            try
            {
                HomeBAL objBAL = new HomeBAL();
                List<CBRColumnChartModel> lstChart = objBAL.GetCBRGridDataBAL(Convert.ToInt32(Request.Params["state"]), Convert.ToInt32(Request.Params["district"]), Convert.ToInt32(Request.Params["block"]));
                return Json(lstChart, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                //using (System.IO.StreamWriter sw = System.IO.File.AppendText(System.IO.Path.Combine("D:\\Highmap\\ErrorLog_" + System.DateTime.Now.Date.ToString("dd_MM_yyyy") + ".txt", "")))
                //{
                //    sw.WriteLine("Date :" + DateTime.Now.ToString());
                //    sw.WriteLine("Method : " + "GetGridData()");
                //    sw.WriteLine("Exception : " + ex.StackTrace.ToString());

                //    if (ex.InnerException != null)
                //        sw.WriteLine("Exception : " + ex.InnerException.StackTrace.ToString());
                //    sw.WriteLine("---------------------------------------------------------------------------------------");
                //    sw.Close();
                //}
                return new JsonResult
                {
                    Data = "Error"
                };
            }
        }

        #endregion
    }
}
