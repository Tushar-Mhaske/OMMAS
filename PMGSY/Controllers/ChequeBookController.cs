using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PMGSY.Models;
using PMGSY.BAL.ChequeBook;
using PMGSY.Common;
using System.Reflection;
using PMGSY.Models.ChequeBook;
using System.Data.Entity.Validation;
using System.Globalization;
using PMGSY.Extensions;



namespace PMGSY.Controllers
{
    [RequiredAuthentication]
    public class ChequeBookController : Controller
    {
        //private PMGSYEntities db = new PMGSYEntities();
        private IChequeBookBAL chequeBookBAL = new ChequeBookBAL();
        private CommonFunctions commomFuncObj = null;

        [GenericAccountValidationFilter(InputParameter = new string[] { "BankDetails" })]
        public ActionResult Index(string id)
        {
            PMGSYEntities db = new PMGSYEntities();
            ChequeBookViewModel chqBookModel = new ChequeBookViewModel();
            try
            {
                using (System.IO.StreamWriter sw = System.IO.File.AppendText(System.IO.Path.Combine("D:\\ErrorLog\\OMMASErrorLog_" + System.DateTime.Now.Date.ToString("dd_MM_yyyy") + ".txt", "")))
                {
                    sw.WriteLine("Date :" + DateTime.Now.ToString());
                    sw.WriteLine("Method : " + "ChequeBook.Index Method test");
                    sw.WriteLine("---------------------------------------------------------------------------------------");
                    sw.Close();
                }

                List<SelectListItem> lstMonths = new SelectList(db.MASTER_MONTH, "MAST_MONTH_CODE", "MAST_MONTH_FULL_NAME").ToList();
                lstMonths.Insert(0, (new SelectListItem { Text = "Select All", Value = "0", Selected = true }));
                ViewBag.ddlMonth = lstMonths;

                List<SelectListItem> lstYears = new SelectList(db.MASTER_YEAR.Where(m => m.MAST_YEAR_CODE < DateTime.Now.Year + 1), "MAST_YEAR_CODE", "MAST_YEAR_CODE").ToList();
                lstYears.Insert(0, (new SelectListItem { Text = "Select All", Value = "0", Selected = true }));
                ViewBag.ddlYear = lstYears;

                return View(chqBookModel);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "ChequeBook.Index()");
                return View(chqBookModel);
            }
            finally
            {
                if (db != null)
                    db.Dispose();
            }
        }

        [HttpPost]
        public ActionResult GetChequeBookList(FormCollection homeFormCollection)
        {
            try
            {
                //Adde By Abhishek kamble 30-Apr-2014 start
                using (CommonFunctions commonFunction = new CommonFunctions())
                {
                    if (!commonFunction.ValidateGridParameters(new GridParams(Convert.ToInt32(homeFormCollection["page"]), Convert.ToInt32(homeFormCollection["rows"]), homeFormCollection["sidx"], homeFormCollection["sord"], Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                    {
                        return null;
                    }
                }
                //Adde By Abhishek kamble 30-Apr-2014 end
                String searchParameters = string.Empty;
                long totalRecords;
                Int16 month = Convert.ToInt16(Request.Params["month"]);
                Int16 year = Convert.ToInt16(Request.Params["year"]);
                Int32 chequeNo = Convert.ToInt32(Request.Params["cheque"]);
                String IsSRRDADpiu = Request.Params["IsSRRDADpiu"];
                
                //Added By Abhishek kamble to Add chq book at SRRDA level start
                int AdminNdCode = 0;
                int LevelId = 0;
                if (PMGSYSession.Current.LevelId == 4)//SRRDA
                {         
                    if (String.IsNullOrEmpty(IsSRRDADpiu))
                    {
                        LevelId = 5;
                        AdminNdCode = Convert.ToInt32(Request.Params["AdminNdCode"]);
                    }
                    else {
                        if (IsSRRDADpiu.Equals("S")) //SRRDA
                        {
                            LevelId = PMGSYSession.Current.LevelId;
                            AdminNdCode = PMGSYSession.Current.AdminNdCode;
                        }
                        else if( IsSRRDADpiu.Equals("D")){  //DPIU
                            LevelId = 5;
                            AdminNdCode = Convert.ToInt32(Request.Params["AdminNdCode"]);
                        }                    
                    }
                }
                else
                {
                    AdminNdCode = PMGSYSession.Current.AdminNdCode;
                    LevelId = PMGSYSession.Current.LevelId;
                }
                //Added By Abhishek kamble to Add chq book at SRRDA level end

                string search = month + "$" + year + "$" + chequeNo;
                var jsonData = new
                {
                    rows = chequeBookBAL.ChequeBookList(Convert.ToInt32(homeFormCollection["page"]) - 1, Convert.ToInt32(homeFormCollection["rows"]), homeFormCollection["sidx"], homeFormCollection["sord"], search, out totalRecords, AdminNdCode, LevelId),
                    total = totalRecords <= Convert.ToInt32(homeFormCollection["rows"]) ? 1 : totalRecords / Convert.ToInt32(homeFormCollection["rows"]) + 1,
                    page = Convert.ToInt32(homeFormCollection["page"]),
                    records = totalRecords
                };
                return Json(jsonData);
            }
            catch (Exception)
            {
                return null;
            }
        }


        public ActionResult Details(int id = 0)
        {
            ACC_CHQ_BOOK_DETAILS acc_chq_book_details = null;
            PMGSYEntities db = new PMGSYEntities();
            try
            {
                acc_chq_book_details = db.ACC_CHQ_BOOK_DETAILS.Find(id);
                if (acc_chq_book_details == null)
                {
                    return HttpNotFound();
                }
                return View(acc_chq_book_details);
            }
            catch (Exception)
            {
                return View(acc_chq_book_details);
            }
            finally
            {
                if (db != null)
                    db.Dispose();
            }
        }

        /// <summary>
        /// to check bank details exist or not 
        /// </summary>
        /// <returns>bool</returns>             
        public ActionResult CheckBankDetailsExist()
        {
            bool exist = true;
            PMGSYEntities db = new PMGSYEntities();
            try
            {
                if (PMGSYSession.Current.LevelId == 4)
                {
                    exist = db.ACC_BANK_DETAILS.Any(bd => bd.ADMIN_ND_CODE == PMGSYSession.Current.AdminNdCode && bd.FUND_TYPE == PMGSYSession.Current.FundType && bd.BANK_ACC_STATUS == true && bd.ACCOUNT_TYPE == "S");
                }
                else
                {
                    int adminNDCode = db.ADMIN_DEPARTMENT.Where(ad => ad.ADMIN_ND_CODE == PMGSYSession.Current.AdminNdCode).Select(ad => (Int32)ad.MAST_PARENT_ND_CODE).FirstOrDefault();

                    exist = db.ACC_BANK_DETAILS.Any(bd => bd.ADMIN_ND_CODE == adminNDCode && bd.FUND_TYPE == PMGSYSession.Current.FundType && bd.BANK_ACC_STATUS == true && bd.ACCOUNT_TYPE == "S");
                }
                return Json(new { exist = exist }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return Json(new { exist = exist }, JsonRequestBehavior.AllowGet);
            }
            finally
            {
                if (db != null)
                    db.Dispose();
            }
        }

        public ActionResult AddEditChequeBook()
        {
            PMGSYEntities db = new PMGSYEntities();
            try
            {
                ACC_BANK_DETAILS bank_detail = new ACC_BANK_DETAILS();

                int AdminNdCode = 0;
                if (PMGSYSession.Current.LevelId == 4)
                {
                    AdminNdCode = Convert.ToInt32(Request.Params["AdminNdCode"]);
                }
                else
                {
                    AdminNdCode = PMGSYSession.Current.AdminNdCode;
                }


                if (PMGSYSession.Current.LevelId == 4)
                {
                    bank_detail = (from item in db.ACC_BANK_DETAILS
                                   where item.ADMIN_ND_CODE == PMGSYSession.Current.AdminNdCode && item.FUND_TYPE == PMGSYSession.Current.FundType && item.BANK_ACC_STATUS == true && item.ACCOUNT_TYPE == "S" //condition added by koustubh nakate on 17/07/2013 for active bank details
                                   select item).FirstOrDefault();
                }
                else
                {
                    //bank_detail = (from item in db.ACC_BANK_DETAILS
                    //               where item.ADMIN_ND_CODE == db.ADMIN_DEPARTMENT.Where(m => m.ADMIN_ND_CODE == PMGSYSession.Current.AdminNdCode).Select(m => m.MAST_PARENT_ND_CODE).FirstOrDefault() && item.FUND_TYPE == PMGSYSession.Current.FundType && item.BANK_ACC_STATUS == true //condition added by koustubh nakate on 17/07/2013 for active bank details
                    //               select item).FirstOrDefault();
                    bank_detail = (from item in db.ACC_BANK_DETAILS
                                   where item.ADMIN_ND_CODE == PMGSYSession.Current.ParentNDCode && item.FUND_TYPE == PMGSYSession.Current.FundType && item.BANK_ACC_STATUS == true && item.ACCOUNT_TYPE == "S" //condition added by koustubh nakate on 17/07/2013 for active bank details
                                   select item).FirstOrDefault();
                }

                ADMIN_DEPARTMENT admin_dept = (from itr in db.ADMIN_DEPARTMENT
                                               where itr.ADMIN_ND_CODE == AdminNdCode
                                               select itr
                                                ).FirstOrDefault();

                if (bank_detail == null)
                {
                    ViewBag.BankName = "Not Available";
                    ViewBag.BankBranch = "Not Available";
                    ViewBag.Name = "Not Available";
                }
                else
                {
                    ViewBag.BankName = bank_detail.BANK_NAME;
                    ViewBag.BankBranch = bank_detail.BANK_BRANCH;
                    ViewBag.Name = admin_dept == null ? "" : admin_dept.ADMIN_ND_NAME;
                }


                //new change done by Vikram on 12-09-2013
                ChequeBookViewModel chqModel = new ChequeBookViewModel();
                commomFuncObj = new CommonFunctions();
                if (bank_detail != null)
                {
                    if (bank_detail.BANK_ACC_OPEN_DATE != null)
                    {
                        chqModel.ACC_OPEN_DATE = commomFuncObj.GetDateTimeToString(bank_detail.BANK_ACC_OPEN_DATE.Value);
                    }
                    else
                    {
                        chqModel.ACC_OPEN_DATE = "01/04/2005";
                    }
                }
                return View("AddEditChequeBook", chqModel);
            }
            catch (Exception ex)
            {
                return null;
            }
            finally
            {
                if (db != null)
                    db.Dispose();
            }
        }

        [HttpPost]
        public ActionResult Create(ChequeBookViewModel chequeBookViewModel)
        {
            String ValidationSummary = String.Empty;
            ACC_BANK_DETAILS bank_detail = new ACC_BANK_DETAILS();
            PMGSYEntities db = new PMGSYEntities();
            try
            {
                if (PMGSYSession.Current.LevelId == 4)
                {
                    bank_detail = db.ACC_BANK_DETAILS.Where(m => m.ADMIN_ND_CODE == PMGSYSession.Current.AdminNdCode && m.FUND_TYPE == PMGSYSession.Current.FundType && m.BANK_ACC_STATUS == true && m.ACCOUNT_TYPE == "S").FirstOrDefault();
                }
                else
                {
                    bank_detail = db.ACC_BANK_DETAILS.Where(m => m.ADMIN_ND_CODE == PMGSYSession.Current.ParentNDCode && m.BANK_ACC_STATUS == true && m.FUND_TYPE == PMGSYSession.Current.FundType && m.ACCOUNT_TYPE == "S").FirstOrDefault();

                    if (ModelState.ContainsKey("ADMIN_ND_CODE"))
                        ModelState["ADMIN_ND_CODE"].Errors.Clear();
                }
                if (PMGSYSession.Current.LevelId == 4)
                {
                    //chequeBookViewModel.LVL_ID = 5;

                    if (String.IsNullOrEmpty(chequeBookViewModel.IsSRRDADpiu))
                    {
                        chequeBookViewModel.LVL_ID = 5;
                    }
                    else
                    {
                        if (chequeBookViewModel.IsSRRDADpiu.Equals("S")) //SRRDA
                        {
                            chequeBookViewModel.LVL_ID = PMGSYSession.Current.LevelId;
                            chequeBookViewModel.ADMIN_ND_CODE = PMGSYSession.Current.AdminNdCode;
                        }
                        else if (chequeBookViewModel.IsSRRDADpiu.Equals("D"))//DPIU
                        {
                            chequeBookViewModel.LVL_ID = 5;
                        }
                    }
                }
                else
                {
                    chequeBookViewModel.LVL_ID = (byte)PMGSYSession.Current.LevelId;
                    chequeBookViewModel.ADMIN_ND_CODE = PMGSYSession.Current.AdminNdCode;
                }

                

                if (ModelState.IsValid)
                {


                    chequeBookViewModel.FUND_TYPE = PMGSYSession.Current.FundType;

                  


                    chequeBookViewModel.BANK_CODE = bank_detail.BANK_CODE;
                    //chequeBookViewModel.BANK_CODE = (from item in db.ACC_BANK_DETAILS
                    //                                 where item.ADMIN_ND_CODE == chequeBookViewModel.ADMIN_ND_CODE && item.FUND_TYPE == PMGSYSession.Current.FundType
                    //                                 select (short)item.BANK_CODE).FirstOrDefault();

                    //Added By Abhishek kamble 10-Mar-2014 start
                    //chequeBookViewModel.CHQ_BOOK_ID = db.ACC_CHQ_BOOK_DETAILS.Max(m => m.CHQ_BOOK_ID) + 1;
                    chequeBookViewModel.CHQ_BOOK_ID = db.ACC_CHQ_BOOK_DETAILS.Any() ? db.ACC_CHQ_BOOK_DETAILS.Max(m => m.CHQ_BOOK_ID) + 1 : 1;
                    //Added By Abhishek kamble 10-Mar-2014 end


                    // ACC_CHQ_BOOK_DETAILS acc_chq_book_details = CloneModel(chequeBookViewModel);
                    ACC_CHQ_BOOK_DETAILS acc_chq_book_details = new ACC_CHQ_BOOK_DETAILS();
                    commomFuncObj = new CommonFunctions();
                    //  ACC_CHQ_BOOK_DETAILS acc_chq_book_details = new ACC_CHQ_BOOK_DETAILS();
                    acc_chq_book_details.CHQ_BOOK_ID = chequeBookViewModel.CHQ_BOOK_ID;
                    acc_chq_book_details.BANK_CODE = chequeBookViewModel.BANK_CODE;
                    acc_chq_book_details.ADMIN_ND_CODE = chequeBookViewModel.ADMIN_ND_CODE;
                    acc_chq_book_details.FUND_TYPE = chequeBookViewModel.FUND_TYPE;
                    acc_chq_book_details.ISSUE_DATE = commomFuncObj.GetStringToDateTime(chequeBookViewModel.ISSUE_DATE);
                    acc_chq_book_details.LEAF_START = chequeBookViewModel.LEAF_START.PadLeft(6, '0');
                    acc_chq_book_details.LEAF_END = chequeBookViewModel.LEAF_END.PadLeft(6, '0');
                    acc_chq_book_details.LVL_ID = chequeBookViewModel.LVL_ID;
                    //added by abhishek kamble 28-nov-2013
                    acc_chq_book_details.USERID = PMGSYSession.Current.UserId;
                    acc_chq_book_details.IPADD = Request.ServerVariables["REMOTE_ADDR"];

                    try
                    {
                        ValidationSummary = chequeBookBAL.ValidateAddEditChequeBookDetails(chequeBookViewModel);
                        if (ValidationSummary == String.Empty)
                        {
                            db.ACC_CHQ_BOOK_DETAILS.Add(acc_chq_book_details);
                            db.SaveChanges();
                            return this.Json(new { success = true, message = string.Empty });
                        }
                        else
                        {
                            return this.Json(new { success = false, message = ValidationSummary });
                        }                     
                    }
                    catch (DbEntityValidationException ex)
                    {
                        // Retrieve the error messages as a list of strings.
                        var errorMessages = ex.EntityValidationErrors
                                .SelectMany(x => x.ValidationErrors)
                                .Select(x => x.ErrorMessage);

                        // Join the list to a single string.
                        var fullErrorMessage = string.Join("; ", errorMessages);

                        // Combine the original exception message with the new one.
                        var exceptionMessage = string.Concat(ex.Message, " The validation errors are: ", fullErrorMessage);

                        // Throw a new DbEntityValidationException with the improved exception message.
                        throw new DbEntityValidationException(exceptionMessage, ex.EntityValidationErrors);
                    }

                    
                }
                ADMIN_DEPARTMENT admin_dept = (from itr in db.ADMIN_DEPARTMENT
                                               where itr.ADMIN_ND_CODE == chequeBookViewModel.ADMIN_ND_CODE //Take this from Session: admin_nd_code
                                               select itr
                                                ).FirstOrDefault();

                ViewBag.BankName = bank_detail.BANK_NAME;
                ViewBag.BankBranch = bank_detail.BANK_BRANCH;
                ViewBag.Name = admin_dept.ADMIN_ND_NAME;
                chequeBookViewModel.ISSUE_DATE = null;
                return PartialView("AddEditChequeBook", chequeBookViewModel);
            }
            catch (Exception ex)
            {
                return this.Json(new { success = false, message = "An Error Occured while processing your request." });

            }
            finally
            {
                if (db != null)
                    db.Dispose();
            }
        }

        public ActionResult Edit(String parameter, String hash, String key)
        {
            PMGSYEntities db = new PMGSYEntities();
            try
            {
                ACC_BANK_DETAILS bank_detail = new ACC_BANK_DETAILS();
                if (PMGSYSession.Current.LevelId == 4)
                {
                    bank_detail = (from item in db.ACC_BANK_DETAILS
                                   where item.ADMIN_ND_CODE == PMGSYSession.Current.AdminNdCode && item.FUND_TYPE == PMGSYSession.Current.FundType && item.BANK_ACC_STATUS == true && item.ACCOUNT_TYPE == "S" //condition added by Vikram on 12/09/2013 for active bank details
                                   select item).FirstOrDefault();
                }
                else
                {
                    bank_detail = (from item in db.ACC_BANK_DETAILS
                                   where item.ADMIN_ND_CODE == PMGSYSession.Current.ParentNDCode && item.FUND_TYPE == PMGSYSession.Current.FundType && item.BANK_ACC_STATUS == true && item.ACCOUNT_TYPE == "S" //condition added by Vikram on 12/09/2013 for active bank details
                                   select item).FirstOrDefault();
                }

                //int AdminNdCode = 0;
                //if (PMGSYSession.Current.LevelId == 4)
                //{
                //    AdminNdCode = Convert.ToInt32(Request.Params["AdminNdCode"]);
                //}
                //else
                //{
                //    AdminNdCode = PMGSYSession.Current.AdminNdCode;
                //}
                //ADMIN_DEPARTMENT admin_dept = (from itr in db.ADMIN_DEPARTMENT
                //                               where itr.ADMIN_ND_CODE == AdminNdCode
                //                               select itr
                //                                ).FirstOrDefault();

                //if (bank_detail == null)
                //{
                //    ViewBag.BankName = "Not Available";
                //    ViewBag.BankBranch = "Not Available";
                //    ViewBag.Name = "Not Available";
                //}
                //else
                //{
                //    ViewBag.BankName = bank_detail.BANK_NAME;
                //    ViewBag.BankBranch = bank_detail.BANK_BRANCH;
                //    ViewBag.Name = admin_dept.ADMIN_ND_NAME;
                //}
                ACC_CHQ_BOOK_DETAILS acc_chq_book_details = null;
                ChequeBookViewModel chequeBookViewModel = null;
                string[] strParameters = URLEncrypt.DecryptParameters(new string[] { parameter, hash, key });
                if (strParameters.Length > 0)
                {
                    Int32 id = Convert.ToInt32(strParameters[0]);
                    //TempData["id"] = id;
                    acc_chq_book_details = db.ACC_CHQ_BOOK_DETAILS.Find(id);
                    //chequeBookViewModel = CloneObject(acc_chq_book_details);

                    chequeBookViewModel = new ChequeBookViewModel();
                    commomFuncObj = new CommonFunctions();
                    //ChequeBookViewModel chequeBookViewModel = new ChequeBookViewModel();
                    chequeBookViewModel.CHQ_BOOK_ID = acc_chq_book_details.CHQ_BOOK_ID;
                    chequeBookViewModel.BANK_CODE = acc_chq_book_details.BANK_CODE;
                    chequeBookViewModel.ADMIN_ND_CODE = acc_chq_book_details.ADMIN_ND_CODE;
                    chequeBookViewModel.FUND_TYPE = acc_chq_book_details.FUND_TYPE;
                    chequeBookViewModel.ISSUE_DATE = commomFuncObj.GetDateTimeToString(acc_chq_book_details.ISSUE_DATE);
                    chequeBookViewModel.LEAF_START = acc_chq_book_details.LEAF_START;
                    chequeBookViewModel.LEAF_END = acc_chq_book_details.LEAF_END;
                    chequeBookViewModel.LVL_ID = acc_chq_book_details.LVL_ID;   

                    ADMIN_DEPARTMENT admin_dept = (from itr in db.ADMIN_DEPARTMENT
                                                   where itr.ADMIN_ND_CODE == acc_chq_book_details.ADMIN_ND_CODE
                                                   select itr
                                                    ).FirstOrDefault();

                    if (bank_detail == null)
                    {
                        ViewBag.BankName = "Not Available";
                        ViewBag.BankBranch = "Not Available";
                        ViewBag.Name = "Not Available";
                    }
                    else
                    {
                        ViewBag.BankName = bank_detail.BANK_NAME;
                        ViewBag.BankBranch = bank_detail.BANK_BRANCH;
                        ViewBag.Name = admin_dept.ADMIN_ND_NAME;
                    }


                }
                if (chequeBookViewModel == null)
                {
                    return HttpNotFound();
                }

                //new change done by Vikram on 12-09-2013
                commomFuncObj = new CommonFunctions();
                if (bank_detail != null)
                {
                    //modified by abhishek kamble 31-oct-2013
                    //old code 
                    //chequeBookViewModel.ACC_OPEN_DATE = commomFuncObj.GetDateTimeToString(bank_detail.BANK_ACC_OPEN_DATE.Value);

                    //new code
                    if (bank_detail.BANK_ACC_OPEN_DATE != null)
                    {
                        chequeBookViewModel.ACC_OPEN_DATE = commomFuncObj.GetDateTimeToString(bank_detail.BANK_ACC_OPEN_DATE.Value);
                    }
                    else
                    {
                        chequeBookViewModel.ACC_OPEN_DATE = "01/04/2005";
                    }
                }
                //end of change
                if (chequeBookViewModel.LVL_ID == 4)
                {
                    chequeBookViewModel.IsSRRDADpiu = "S";
                }
                return PartialView("AddEditChequeBook", chequeBookViewModel);
            }
            catch (Exception)
            {
                return null;
            }
            finally
            {
                if (db != null)
                    db.Dispose();
            }
        }

        [HttpPost]
        public ActionResult Edit(ChequeBookViewModel chequeBookViewModel)
        {
            PMGSYEntities db = new PMGSYEntities();

            try
            {
                String ValidationSummary = String.Empty;
                //chequeBookViewModel.CHQ_BOOK_ID = Convert.ToInt32(TempData["id"]);
                ACC_CHQ_BOOK_DETAILS existingData = db.ACC_CHQ_BOOK_DETAILS.Find(chequeBookViewModel.CHQ_BOOK_ID);
                chequeBookViewModel.ADMIN_ND_CODE = existingData.ADMIN_ND_CODE;
                chequeBookViewModel.BANK_CODE = existingData.BANK_CODE;
                chequeBookViewModel.FUND_TYPE = existingData.FUND_TYPE;
                chequeBookViewModel.LVL_ID = existingData.LVL_ID;
                //ACC_CHQ_BOOK_DETAILS acc_chq_book_details = CloneModel(chequeBookViewModel);

                ACC_CHQ_BOOK_DETAILS acc_chq_book_details = new ACC_CHQ_BOOK_DETAILS();
                commomFuncObj = new CommonFunctions();
                //ACC_CHQ_BOOK_DETAILS acc_chq_book_details = new ACC_CHQ_BOOK_DETAILS();
                acc_chq_book_details.CHQ_BOOK_ID = chequeBookViewModel.CHQ_BOOK_ID;
                acc_chq_book_details.BANK_CODE = chequeBookViewModel.BANK_CODE;
                acc_chq_book_details.ADMIN_ND_CODE = chequeBookViewModel.ADMIN_ND_CODE;
                acc_chq_book_details.FUND_TYPE = chequeBookViewModel.FUND_TYPE;
                acc_chq_book_details.ISSUE_DATE = commomFuncObj.GetStringToDateTime(chequeBookViewModel.ISSUE_DATE);
                acc_chq_book_details.LEAF_START = chequeBookViewModel.LEAF_START.PadLeft(6, '0');
                acc_chq_book_details.LEAF_END = chequeBookViewModel.LEAF_END.PadLeft(6, '0');
                acc_chq_book_details.LVL_ID = chequeBookViewModel.LVL_ID;

                //added by abhishek kamble 28-nov-2013
                acc_chq_book_details.USERID = PMGSYSession.Current.UserId;
                acc_chq_book_details.IPADD = Request.ServerVariables["REMOTE_ADDR"];


                if (PMGSYSession.Current.LevelId == 5)
                {
                    if (ModelState.ContainsKey("ADMIN_ND_CODE"))
                        ModelState["ADMIN_ND_CODE"].Errors.Clear();
                }

                if (ModelState.IsValid)
                {
                    ValidationSummary = chequeBookBAL.ValidateAddEditChequeBookDetails(chequeBookViewModel);
                    if (ValidationSummary == String.Empty)
                    {
                        try
                        {
                            db.Entry(existingData).CurrentValues.SetValues(acc_chq_book_details);
                            //db.Entry(acc_chq_book_details).State = System.Data.Entity.EntityState.Modified;
                            db.SaveChanges();
                            return this.Json(new { success = true, message = string.Empty });
                        }
                        catch (DbEntityValidationException ex)
                        {
                            Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                            // Retrieve the error messages as a list of strings.
                            var errorMessages = ex.EntityValidationErrors
                                    .SelectMany(x => x.ValidationErrors)
                                    .Select(x => x.ErrorMessage);

                            // Join the list to a single string.
                            var fullErrorMessage = string.Join("; ", errorMessages);

                            // Combine the original exception message with the new one.
                            var exceptionMessage = string.Concat(ex.Message, " The validation errors are: ", fullErrorMessage);

                            // Throw a new DbEntityValidationException with the improved exception message.
                            throw new DbEntityValidationException(exceptionMessage, ex.EntityValidationErrors);
                        }
                    }
                    else
                    {
                        return this.Json(new { success = false, message = ValidationSummary });
                    }
                }
                else
                {
                    ACC_BANK_DETAILS bank_detail = new ACC_BANK_DETAILS();
                    if (PMGSYSession.Current.LevelId == 4)
                    {
                         bank_detail = db.ACC_BANK_DETAILS.Where(m => m.ADMIN_ND_CODE == PMGSYSession.Current.AdminNdCode && m.FUND_TYPE == PMGSYSession.Current.FundType && m.BANK_ACC_STATUS == true && m.ACCOUNT_TYPE == "S").FirstOrDefault();
                    }
                    else
                    {
                        bank_detail = db.ACC_BANK_DETAILS.Where(m => m.ADMIN_ND_CODE == PMGSYSession.Current.ParentNDCode && m.BANK_ACC_STATUS == true && m.FUND_TYPE == PMGSYSession.Current.FundType && m.ACCOUNT_TYPE == "S").FirstOrDefault();
                    }
                    ADMIN_DEPARTMENT admin_dept = (from itr in db.ADMIN_DEPARTMENT
                                                   where itr.ADMIN_ND_CODE == existingData.ADMIN_ND_CODE //Take this from Session: admin_nd_code
                                                   select itr
                                              ).FirstOrDefault();

                    ViewBag.BankName = bank_detail.BANK_NAME;
                    ViewBag.BankBranch = bank_detail.BANK_BRANCH;
                    if (admin_dept != null)
                    {
                        ViewBag.Name =admin_dept.ADMIN_ND_NAME;
                    }
                    return PartialView("AddEditChequeBook", chequeBookViewModel);
                }
            }
            catch (Exception)
            {
                return this.Json(new { success = false, message = "An Error occured while proccessing Your request." });
            }
            finally
            {
                if (db != null)
                    db.Dispose();
            }
        }

        [HttpPost]
        public JsonResult Delete(String parameter, String hash, String key)
        {
            try
            {
                string[] strParameters = URLEncrypt.DecryptParameters(new string[] { parameter, hash, key });
                if (strParameters.Length > 0)
                {
                    Int32 id = Convert.ToInt32(strParameters[0]);
                    //Int32 AdminNdCode = 0;
                    IChequeBookBAL chequeBookBAL = new ChequeBookBAL();

                    //if (PMGSYSession.Current.LevelId == 4)
                    //{
                    //    if (!(String.IsNullOrEmpty(Request.Params["AdminNdCode"])))
                    //    {
                    //        AdminNdCode = Convert.ToInt32(Request.Params["AdminNdCode"]);
                    //    }
                    //    else
                    //    {
                    //        return this.Json(new { success = false, message = "Error while processing your request" });
                    //    }
                    //}
                    //else {
                    //    AdminNdCode = PMGSYSession.Current.AdminNdCode;
                    //}

                    String status = chequeBookBAL.DeleteChequeBook(id);
                    if (status == String.Empty)
                    {
                        return this.Json(new { success = true, message = URLEncrypt.EncryptParameters(new string[] { strParameters[0] }) });
                    }
                    else
                    {
                        return this.Json(new { success = false, message = status });
                    }
                }
                else
                {
                    return this.Json(new { success = false, message = "Error while processing your request" });
                }
            }
            catch (Exception)
            {
                return this.Json(new { success = false, message = "Error while processing your request" });
            }
        }


        //[Audit]
        //public ChequeBookViewModel CloneObject(ACC_CHQ_BOOK_DETAILS acc_chq_book_details)
        //{
        //    commomFuncObj = new CommonFunctions();
        //    ChequeBookViewModel chequeBookViewModel = new ChequeBookViewModel();
        //    chequeBookViewModel.CHQ_BOOK_ID = acc_chq_book_details.CHQ_BOOK_ID;
        //    chequeBookViewModel.BANK_CODE = acc_chq_book_details.BANK_CODE;
        //    chequeBookViewModel.ADMIN_ND_CODE = acc_chq_book_details.ADMIN_ND_CODE;
        //    chequeBookViewModel.FUND_TYPE = acc_chq_book_details.FUND_TYPE;
        //    chequeBookViewModel.ISSUE_DATE = commomFuncObj.GetDateTimeToString(acc_chq_book_details.ISSUE_DATE);
        //    chequeBookViewModel.LEAF_START = acc_chq_book_details.LEAF_START;
        //    chequeBookViewModel.LEAF_END = acc_chq_book_details.LEAF_END;
        //    chequeBookViewModel.LVL_ID = acc_chq_book_details.LVL_ID;

        //    return chequeBookViewModel;
        //}


        //[Audit]
        //public ACC_CHQ_BOOK_DETAILS CloneModel(ChequeBookViewModel chequeBookViewModel)
        //{
        //    commomFuncObj = new CommonFunctions();
        //    ACC_CHQ_BOOK_DETAILS acc_chq_book_details = new ACC_CHQ_BOOK_DETAILS();
        //    acc_chq_book_details.CHQ_BOOK_ID = chequeBookViewModel.CHQ_BOOK_ID;
        //    acc_chq_book_details.BANK_CODE = chequeBookViewModel.BANK_CODE;
        //    acc_chq_book_details.ADMIN_ND_CODE = chequeBookViewModel.ADMIN_ND_CODE;
        //    acc_chq_book_details.FUND_TYPE = chequeBookViewModel.FUND_TYPE;
        //    acc_chq_book_details.ISSUE_DATE = commomFuncObj.GetStringToDateTime(chequeBookViewModel.ISSUE_DATE);
        //    acc_chq_book_details.LEAF_START = chequeBookViewModel.LEAF_START.PadLeft(6, '0');
        //    acc_chq_book_details.LEAF_END = chequeBookViewModel.LEAF_END.PadLeft(6, '0');
        //    acc_chq_book_details.LVL_ID = chequeBookViewModel.LVL_ID;

        //    //added by abhishek kamble 28-nov-2013
        //    acc_chq_book_details.USERID = PMGSYSession.Current.UserId;
        //    acc_chq_book_details.IPADD = Request.ServerVariables["REMOTE_ADDR"];

        //    //Added By Abhishek kamble 22-jan-2013
        //    //acc_chq_book_details.IS_CHQBOOK_COMPLETED = true;

        //    return acc_chq_book_details;
        //}


        //protected override void Dispose(bool disposing)
        //{
        //    db.Dispose();
        //    base.Dispose(disposing);
        //}

        #region New Cheque Book Details

        [HttpGet]
        public ActionResult MasterChequeBook()
        {
            try
            {
                return View();
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "MasterChequeBook()");
                return null;
            }
        }

        [HttpGet]
        public ActionResult AddEditCB()
        {
            PMGSYEntities db = new PMGSYEntities();
            try
            {
                ACC_BANK_DETAILS bank_detail = new ACC_BANK_DETAILS();

                int AdminNdCode = 0;
                if (PMGSYSession.Current.LevelId == 4)
                {
                    AdminNdCode = Convert.ToInt32(Request.Params["AdminNdCode"]);
                }
                else
                {
                    AdminNdCode = PMGSYSession.Current.AdminNdCode;
                }

                if (PMGSYSession.Current.LevelId == 4)
                {
                    bank_detail = (from item in db.ACC_BANK_DETAILS
                                   where item.ADMIN_ND_CODE == PMGSYSession.Current.AdminNdCode && item.FUND_TYPE == PMGSYSession.Current.FundType && item.BANK_ACC_STATUS == true && item.ACCOUNT_TYPE == "S"
                                   select item).FirstOrDefault();
                }
                else
                {
                    bank_detail = (from item in db.ACC_BANK_DETAILS
                                   where item.ADMIN_ND_CODE == PMGSYSession.Current.ParentNDCode && item.FUND_TYPE == PMGSYSession.Current.FundType && item.BANK_ACC_STATUS == true && item.ACCOUNT_TYPE == "S"
                                   select item).FirstOrDefault();
                }

                ADMIN_DEPARTMENT admin_dept = (from itr in db.ADMIN_DEPARTMENT
                                               where itr.ADMIN_ND_CODE == AdminNdCode
                                               select itr
                                                ).FirstOrDefault();

                ChequeBookDetailsViewModel chqModel = new ChequeBookDetailsViewModel();
                List<SelectListItem> items = new List<SelectListItem>();
                items.Add(new SelectListItem() { Text = "Saving", Value = "S" });
                items.Add(new SelectListItem() { Text = "Holding", Value = "H" });
                items.Add(new SelectListItem() { Text = "Security Deposit Account (SDA)", Value = "D" });
                items.Insert(0, (new SelectListItem { Text = "Select Account Type", Value = "A", Selected = true }));
                chqModel.lstBankAccType = items;
                // above code added by saurabh

                if (bank_detail == null)
                {
                    ViewBag.BankName = "Not Available";
                    ViewBag.BankBranch = "Not Available";
                    ViewBag.Name = "Not Available";
                }
                else
                {
                    var selectedItem = chqModel.PopulateDPIU.Select(m => m.Text).FirstOrDefault();
                    ViewBag.BankName = bank_detail.BANK_NAME;
                    ViewBag.BankBranch = bank_detail.BANK_BRANCH;
                    ViewBag.Name = admin_dept == null ? selectedItem : admin_dept.ADMIN_ND_NAME;
                }
                commomFuncObj = new CommonFunctions();
                if (bank_detail != null)
                {
                    if (bank_detail.BANK_ACC_OPEN_DATE != null)
                    {
                        chqModel.ACC_OPEN_DATE = commomFuncObj.GetDateTimeToString(bank_detail.BANK_ACC_OPEN_DATE.Value);
                    }
                    else
                    {
                        chqModel.ACC_OPEN_DATE = "01/04/2005";
                    }
                }
                return View("AddEditCB", chqModel);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "AddEditCB()");
                return null;
            }
            finally
            {
                if (db != null)
                    db.Dispose();
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddCB(ChequeBookDetailsViewModel model)
        {
            bool status = false;
            string message = string.Empty;
            try
            {
                if (model.ADMIN_ND_CODE == 0 && model.IsSRRDADpiu.Equals("S"))
                {
                    ModelState["ADMIN_ND_CODE"].Errors.Clear();
                }

                if(PMGSYSession.Current.FundType != "P")
                {
                    ModelState.Remove("BANK_ACC_TYPE");
                    //BANK_ACC_TYPE
                }

                if (ModelState.IsValid)
                {
                    ChequeBookBAL objBAL = new ChequeBookBAL();
                    if (objBAL.AddCBBAL(model, ref message))
                    {
                        message = message == string.Empty ? "Cheque Book details saved successfully." : message;
                        status = true;
                    }
                    else
                    {
                        message = message == string.Empty ? "Cheque Book details not saved." : message;
                    }
                }
                else
                {
                    return PartialView("AddEditCB", model);
                }
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "AddCB()");
                message = "Cheque Book details not saved.";
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }
        }
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult AddCB(ChequeBookDetailsViewModel model)
        //{
        //    bool status = false;
        //    string message = string.Empty;
        //    try
        //    {
        //        if (ModelState.IsValid)
        //        {
        //            ChequeBookBAL objBAL = new ChequeBookBAL();
        //            if (objBAL.AddCBBAL(model, ref message))
        //            {
        //                message = message == string.Empty ? "Cheque Book details saved successfully." : message;
        //                status = true;
        //            }
        //            else
        //            {
        //                message = message == string.Empty ? "Cheque Book details not saved." : message;
        //            }
        //        }
        //        else
        //        {
        //            return PartialView("AddEditCB", model);
        //        }
        //        return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
        //    }
        //    catch (Exception ex)
        //    {
        //        ErrorLog.LogError(ex, "AddCB()");
        //        message = "Cheque Book details not saved.";
        //        return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
        //    }
        //}

        [HttpPost]
        public ActionResult GetCBList(FormCollection homeFormCollection)
        {
            try
            {
                using (CommonFunctions commonFunction = new CommonFunctions())
                {
                    if (!commonFunction.ValidateGridParameters(new GridParams(Convert.ToInt32(homeFormCollection["page"]), Convert.ToInt32(homeFormCollection["rows"]), homeFormCollection["sidx"], homeFormCollection["sord"], Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                    {
                    }
                    else
                    {
                        return null;
                    }
                }

                String searchParameters = string.Empty;
                long totalRecords;
                Int16 month = Convert.ToInt16(Request.Params["month"]);
                Int16 year = Convert.ToInt16(Request.Params["year"]);
                Int32 chequeNo = Convert.ToInt32(Request.Params["cheque"]);
                String IsSRRDADpiu = Request.Params["IsSRRDADpiu"];
                string AccChqType = Request.Params["AccType"];
                int AdminNdCode = 0;
                int LevelId = 0;
                if (PMGSYSession.Current.LevelId == 4)//SRRDA Login
                {
                    if (String.IsNullOrEmpty(IsSRRDADpiu))
                    {
                        LevelId = 5;
                        AdminNdCode = Convert.ToInt32(Request.Params["AdminNdCode"]);
                    }
                    else
                    {
                        if (IsSRRDADpiu.Equals("SRRDA")) //SRRDA Login
                        {
                            LevelId = PMGSYSession.Current.LevelId;
                            AdminNdCode = PMGSYSession.Current.AdminNdCode;
                        }
                        else // if (IsSRRDADpiu.Equals("D"))
                        {  //DPIU Login
                            LevelId = 5;
                            AdminNdCode = Convert.ToInt32(Request.Params["AdminNdCode"]);
                        }
                    }
                }
                else
                { // DPIU Login
                    AdminNdCode = PMGSYSession.Current.AdminNdCode;
                    LevelId = PMGSYSession.Current.LevelId;
                }

                string search = month + "$" + year + "$" + chequeNo;

                var jsonData = new
                {
                    rows = chequeBookBAL.CBList(Convert.ToInt32(homeFormCollection["page"]) - 1, Convert.ToInt32(homeFormCollection["rows"]), homeFormCollection["sidx"], homeFormCollection["sord"], search, out totalRecords, AdminNdCode, LevelId, AccChqType),
                    total = totalRecords <= Convert.ToInt32(homeFormCollection["rows"]) ? 1 : totalRecords / Convert.ToInt32(homeFormCollection["rows"]) + 1,
                    page = Convert.ToInt32(homeFormCollection["page"]),
                    records = totalRecords
                };
                return Json(jsonData);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "GetCBList()");
                return null;
            }
        }

        [HttpGet]
        public ActionResult EditChequeBookDetails(String parameter, String hash, String key)
        {
            PMGSYEntities db = new PMGSYEntities();
            try
            {
                ACC_BANK_DETAILS bank_detail = new ACC_BANK_DETAILS();
                if (PMGSYSession.Current.LevelId == 4)
                {
                    bank_detail = (from item in db.ACC_BANK_DETAILS
                                   where item.ADMIN_ND_CODE == PMGSYSession.Current.AdminNdCode && item.FUND_TYPE == PMGSYSession.Current.FundType && item.BANK_ACC_STATUS == true && item.ACCOUNT_TYPE == "S"
                                   select item).FirstOrDefault();
                }
                else
                {
                    bank_detail = (from item in db.ACC_BANK_DETAILS
                                   where item.ADMIN_ND_CODE == PMGSYSession.Current.ParentNDCode && item.FUND_TYPE == PMGSYSession.Current.FundType && item.BANK_ACC_STATUS == true && item.ACCOUNT_TYPE == "S"
                                   select item).FirstOrDefault();
                }


                ACC_CHQ_BOOK_DETAILS acc_chq_book_details = null;
                ChequeBookDetailsViewModel chequeBookViewModel = null;
                string[] strParameters = URLEncrypt.DecryptParameters(new string[] { parameter, hash, key });
                if (strParameters.Length > 0)
                {
                    Int32 id = Convert.ToInt32(strParameters[0]);
                    acc_chq_book_details = db.ACC_CHQ_BOOK_DETAILS.Find(id);


                    chequeBookViewModel = new ChequeBookDetailsViewModel();
                    commomFuncObj = new CommonFunctions();
                    // changes by saurabh
                    List<SelectListItem> items = new List<SelectListItem>();
                    items.Add(new SelectListItem() { Text = "Saving", Value = "S" });
                    items.Add(new SelectListItem() { Text = "Holding", Value = "H" });
                    items.Add(new SelectListItem() { Text = "Security Deposit Account (SDA)", Value = "D" });
                    items.Insert(0, (new SelectListItem { Text = "Select Account Type", Value = "A", Selected = true }));
                    chequeBookViewModel.lstBankAccType = items;
                    chequeBookViewModel.BANK_ACC_TYPE = acc_chq_book_details.ACC_TYPE; //== "S"? "Saving" : acc_chq_book_details.ACC_TYPE == "H" ? "Holding" : acc_chq_book_details.ACC_TYPE == "D" ? "Security Deposit Account (SDA)" : "-";  // ADDED BY SAURABH
                    // above change end by saurabh

                    chequeBookViewModel.CHQ_BOOK_ID = acc_chq_book_details.CHQ_BOOK_ID;
                    chequeBookViewModel.BANK_CODE = acc_chq_book_details.BANK_CODE;
                    chequeBookViewModel.ADMIN_ND_CODE = acc_chq_book_details.ADMIN_ND_CODE;
                    chequeBookViewModel.FUND_TYPE = acc_chq_book_details.FUND_TYPE;
                    chequeBookViewModel.ISSUE_DATE = commomFuncObj.GetDateTimeToString(acc_chq_book_details.ISSUE_DATE);
                    chequeBookViewModel.LEAF_START = acc_chq_book_details.LEAF_START;
                    chequeBookViewModel.LEAF_END = acc_chq_book_details.LEAF_END;
                    chequeBookViewModel.LVL_ID = acc_chq_book_details.LVL_ID;
                    chequeBookViewModel.EncryptedChequeBookCode = "Not Null";

                    ADMIN_DEPARTMENT admin_dept = (from itr in db.ADMIN_DEPARTMENT
                                                   where itr.ADMIN_ND_CODE == acc_chq_book_details.ADMIN_ND_CODE
                                                   select itr
                                                    ).FirstOrDefault();

                    if (bank_detail == null)
                    {
                        ViewBag.BankName = "Not Available";
                        ViewBag.BankBranch = "Not Available";
                        ViewBag.Name = "Not Available";
                    }
                    else
                    {
                        ViewBag.BankName = bank_detail.BANK_NAME;
                        ViewBag.BankBranch = bank_detail.BANK_BRANCH;
                        ViewBag.Name = admin_dept.ADMIN_ND_NAME;
                    }
                }
                if (chequeBookViewModel == null)
                {
                    return HttpNotFound();
                }

                commomFuncObj = new CommonFunctions();
                if (bank_detail != null)
                {
                    if (bank_detail.BANK_ACC_OPEN_DATE != null)
                    {
                        chequeBookViewModel.ACC_OPEN_DATE = commomFuncObj.GetDateTimeToString(bank_detail.BANK_ACC_OPEN_DATE.Value);
                    }
                    else
                    {
                        chequeBookViewModel.ACC_OPEN_DATE = "01/04/2005";
                    }
                }
                //end of change
                if (chequeBookViewModel.LVL_ID == 4)
                {
                    chequeBookViewModel.IsSRRDADpiu = "S";
                }
                return PartialView("AddEditCB", chequeBookViewModel);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "EditChequeBookDetails()");
                return null;
            }
            finally
            {
                if (db != null)
                    db.Dispose();
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditChequeBookDetails(ChequeBookDetailsViewModel chequeBookViewModel)
        {
            PMGSYEntities db = new PMGSYEntities();

            try
            {
                String ValidationSummary = String.Empty;

                if (PMGSYSession.Current.FundType != "P")
                    chequeBookViewModel.BANK_ACC_TYPE = "S";

                ACC_CHQ_BOOK_DETAILS existingData = db.ACC_CHQ_BOOK_DETAILS.Find(chequeBookViewModel.CHQ_BOOK_ID);
                chequeBookViewModel.ADMIN_ND_CODE = existingData.ADMIN_ND_CODE;
                chequeBookViewModel.BANK_CODE = existingData.BANK_CODE;
                chequeBookViewModel.FUND_TYPE = existingData.FUND_TYPE;
                chequeBookViewModel.LVL_ID = existingData.LVL_ID;

                ACC_CHQ_BOOK_DETAILS acc_chq_book_details = new ACC_CHQ_BOOK_DETAILS();
                commomFuncObj = new CommonFunctions();
                acc_chq_book_details.CHQ_BOOK_ID = chequeBookViewModel.CHQ_BOOK_ID;
                acc_chq_book_details.BANK_CODE = chequeBookViewModel.BANK_CODE;
                acc_chq_book_details.ADMIN_ND_CODE = chequeBookViewModel.ADMIN_ND_CODE;
                acc_chq_book_details.FUND_TYPE = chequeBookViewModel.FUND_TYPE;
                acc_chq_book_details.ISSUE_DATE = commomFuncObj.GetStringToDateTime(chequeBookViewModel.ISSUE_DATE);
                acc_chq_book_details.LEAF_START = chequeBookViewModel.LEAF_START.PadLeft(6, '0');
                acc_chq_book_details.LEAF_END = chequeBookViewModel.LEAF_END.PadLeft(6, '0');
                acc_chq_book_details.LVL_ID = chequeBookViewModel.LVL_ID;
                acc_chq_book_details.ACC_TYPE = chequeBookViewModel.BANK_ACC_TYPE; // change by saurabh
                acc_chq_book_details.USERID = PMGSYSession.Current.UserId;
                acc_chq_book_details.IPADD = Request.ServerVariables["REMOTE_ADDR"];

                if (PMGSYSession.Current.LevelId == 5)
                {
                    if (ModelState.ContainsKey("ADMIN_ND_CODE"))
                        ModelState["ADMIN_ND_CODE"].Errors.Clear();
                }

                if (PMGSYSession.Current.FundType != "P")
                    ModelState.Remove("BANK_ACC_TYPE");
                if (chequeBookViewModel.LVL_ID == 4)
                    ModelState.Remove("ADMIN_ND_CODE");
                

                if (ModelState.IsValid)
                {
                    PMGSYEntities dbContext = null;
                    dbContext = new PMGSYEntities();
                    string returnValue = dbContext.USP_ACC_VALIDATE_CHEQUEBOOK_UPDATE_DETAILS(chequeBookViewModel.ADMIN_ND_CODE, PMGSYSession.Current.FundType, chequeBookViewModel.ISSUE_DATE, chequeBookViewModel.LEAF_START, chequeBookViewModel.LEAF_END, chequeBookViewModel.LVL_ID, chequeBookViewModel.CHQ_BOOK_ID).ToList().FirstOrDefault();
                    if (returnValue != "True")
                    {
                        ValidationSummary = returnValue;

                        if (ValidationSummary == "True")
                        {
                            ValidationSummary = String.Empty;
                        }
                        else
                        {
                            ValidationSummary = returnValue;
                        }
                    }

                    if (ValidationSummary == String.Empty)
                    {
                        try
                        {
                            db.Entry(existingData).CurrentValues.SetValues(acc_chq_book_details);
                            db.SaveChanges();
                            return this.Json(new { success = true, message = string.Empty });
                        }
                        catch (DbEntityValidationException ex)
                        {
                            Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                            var errorMessages = ex.EntityValidationErrors
                                    .SelectMany(x => x.ValidationErrors)
                                    .Select(x => x.ErrorMessage);
                            var fullErrorMessage = string.Join("; ", errorMessages);
                            var exceptionMessage = string.Concat(ex.Message, " The validation errors are: ", fullErrorMessage);
                            throw new DbEntityValidationException(exceptionMessage, ex.EntityValidationErrors);
                        }
                    }
                    else
                    {
                        return this.Json(new { success = false, message = ValidationSummary });
                    }
                }
                else
                {
                    ACC_BANK_DETAILS bank_detail = new ACC_BANK_DETAILS();
                    if (PMGSYSession.Current.LevelId == 4)
                    {
                        bank_detail = db.ACC_BANK_DETAILS.Where(m => m.ADMIN_ND_CODE == PMGSYSession.Current.AdminNdCode && m.FUND_TYPE == PMGSYSession.Current.FundType && m.BANK_ACC_STATUS == true && m.ACCOUNT_TYPE == "S").FirstOrDefault();
                    }
                    else
                    {
                        bank_detail = db.ACC_BANK_DETAILS.Where(m => m.ADMIN_ND_CODE == PMGSYSession.Current.ParentNDCode && m.BANK_ACC_STATUS == true && m.FUND_TYPE == PMGSYSession.Current.FundType && m.ACCOUNT_TYPE == "S").FirstOrDefault();
                    }
                    ADMIN_DEPARTMENT admin_dept = (from itr in db.ADMIN_DEPARTMENT
                                                   where itr.ADMIN_ND_CODE == existingData.ADMIN_ND_CODE //Take this from Session: admin_nd_code
                                                   select itr
                                              ).FirstOrDefault();

                    ViewBag.BankName = bank_detail.BANK_NAME;
                    ViewBag.BankBranch = bank_detail.BANK_BRANCH;
                    if (admin_dept != null)
                    {
                        ViewBag.Name = admin_dept.ADMIN_ND_NAME;
                    }
                    return PartialView("AddEditCB", chequeBookViewModel);
                }
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "EditChequeBookDetails()");
                return this.Json(new { success = false, message = "An Error occured while proccessing Your request." });
            }
            finally
            {
                if (db != null)
                    db.Dispose();
            }
        }

        [HttpPost]
        [Audit]
        //[PMGSY.Filters.ValidateAntiForgeryTokenOnAllPosts]
        public JsonResult DeleteCB(String parameter, String hash, String key)
        {
            try
            {
                string[] strParameters = URLEncrypt.DecryptParameters(new string[] { parameter, hash, key });
                if (strParameters.Length > 0)
                {
                    Int32 id = Convert.ToInt32(strParameters[0]);
                    IChequeBookBAL chequeBookBAL = new ChequeBookBAL();
                    String status = chequeBookBAL.DeleteCBBAL(id);
                    if (status.Equals("1"))
                    {
                        return this.Json(new { success = true, message = URLEncrypt.EncryptParameters(new string[] { strParameters[0] }) });
                    }
                    else
                    {
                        return this.Json(new { success = false, message = status });
                    }
                }
                else
                {
                    return this.Json(new { success = false, message = "Error while processing your request" });
                }
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "DeleteCB()");
                return this.Json(new { success = false, message = "Error while processing your request" });
            }
        }

        [HttpGet]
        public ActionResult SearchCB()
        {
            PMGSYEntities db = new PMGSYEntities();
            ChequeBookDetailsViewModel chqBookModel = new ChequeBookDetailsViewModel();
            try
            {
                List<SelectListItem> lstMonths = new SelectList(db.MASTER_MONTH, "MAST_MONTH_CODE", "MAST_MONTH_FULL_NAME").ToList();
                lstMonths.Insert(0, (new SelectListItem { Text = "Select All", Value = "0", Selected = true }));
                ViewBag.ddlMonth = lstMonths;

                List<SelectListItem> lstYears = new SelectList(db.MASTER_YEAR.Where(m => m.MAST_YEAR_CODE < DateTime.Now.Year + 1), "MAST_YEAR_CODE", "MAST_YEAR_CODE").ToList();
                lstYears.Insert(0, (new SelectListItem { Text = "Select All", Value = "0", Selected = true }));
                ViewBag.ddlYear = lstYears;

                List<SelectListItem> items = new List<SelectListItem>();
                items.Add(new SelectListItem() { Text = "Saving", Value = "S" });
                items.Add(new SelectListItem() { Text = "Holding", Value = "H" });
                items.Add(new SelectListItem() { Text = "Security Deposit Account (SDA)", Value = "D" });
                items.Insert(0, (new SelectListItem { Text = "Select Account Type", Value = "A", Selected = true }));
                chqBookModel.lstBankAccType = items;

                return View(chqBookModel);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "SearchCB()");
                return View(chqBookModel);
            }
            finally
            {
                if (db != null)
                    db.Dispose();
            }
        }

        #endregion


    }

}