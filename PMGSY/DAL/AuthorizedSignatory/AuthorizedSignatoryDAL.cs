using PMGSY.Common;
using PMGSY.Models;
using PMGSY.Models.AuthorizedSignatory;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Transactions;
using System.Web;

using PMGSY.Extensions;

namespace PMGSY.DAL.AuthorizedSignatory
{
    public class AuthorizedSignatoryDAL : IAuthorizedSignatoryDAL
    {

        private PMGSYEntities dbContext = null;

        public AuthorizedSignatoryDAL()
        {
            dbContext = new PMGSYEntities();
        }

        /// <summary>
        /// function to get authorized signatory details as per the state
        /// </summary>
        /// <param name="int_state_code"></param>
        /// <returns> list of authorized signatories </returns>

        //public List<AuthorizedSignatoryModel> GetAuthorizedSignatoryDetails(int? page, int? rows, string sidx, string sord, int int_state_code, string ShowActiveAuthSig, string searchCriteria, out long totalRecords)
        //{
        //    string Level = string.Empty;
        //    string searchStatus = ShowActiveAuthSig;
        //    if (ShowActiveAuthSig == "W")
        //    {
        //        ShowActiveAuthSig = "Y";
        //    }
        //    Level = PMGSYSession.Current.LevelId == 4 ? "S" : "D";

        //    try
        //    {
        //        CommonFunctions objCommon = new CommonFunctions();
        //        List<AuthorizedSignatoryModel> AuthSigList = new List<AuthorizedSignatoryModel>();
        //        String param = string.Empty;
        //        dbContext = new PMGSYEntities();
        //        Nullable<int> noDesig = 517;
        //        var query = (from dept in dbContext.ADMIN_DEPARTMENT
        //                    from officers in dept.ADMIN_NODAL_OFFICERS.Where(m => m.ADMIN_ND_CODE == dept.ADMIN_ND_CODE && m.ADMIN_MODULE == "A").DefaultIfEmpty()
        //                    //into co
        //                    //from x in co.DefaultIfEmpty()
        //                    where
        //                    dept.ADMIN_ND_CODE == PMGSYSession.Current.AdminNdCode || dept.MAST_PARENT_ND_CODE == PMGSYSession.Current.AdminNdCode
        //                    && (ShowActiveAuthSig == "Y" ? "1" : officers.ADMIN_ACTIVE_STATUS) == (ShowActiveAuthSig == "Y" ? "1" : "N")


        //                    select new
        //                    {
        //                        ADMIN_ND_CODE = dept.ADMIN_ND_CODE,
        //                        ADMIN_NO_OFFICER_CODE = officers.ADMIN_NO_OFFICER_CODE == null ? 0 : officers.ADMIN_NO_OFFICER_CODE,
        //                        ADMIN_ND_NAME = dept.ADMIN_ND_NAME,
        //                        ADMIN_NO_FULL_NAME = ShowActiveAuthSig == "Y" ? (officers.ADMIN_ACTIVE_STATUS == "Y" ? (officers.ADMIN_NO_FNAME + " " + (officers.ADMIN_NO_MNAME == null ? string.Empty : officers.ADMIN_NO_MNAME) + " " + officers.ADMIN_NO_LNAME) : string.Empty) : (officers.ADMIN_NO_FNAME + " " + (officers.ADMIN_NO_MNAME == null ? string.Empty : officers.ADMIN_NO_MNAME) + " " + officers.ADMIN_NO_LNAME),
        //                        ADMIN_NO_DESIGNATION = ShowActiveAuthSig == "Y" ? (officers.ADMIN_ACTIVE_STATUS == "N" ? officers.ADMIN_NO_DESIGNATION == null ? 0 : officers.ADMIN_NO_DESIGNATION : officers.ADMIN_NO_DESIGNATION == null ? 0 : officers.ADMIN_NO_DESIGNATION) : officers.ADMIN_NO_DESIGNATION == null ? 0 : officers.ADMIN_NO_DESIGNATION,
        //                        ADMIN_ACTIVE_START_DATE = ShowActiveAuthSig == "Y" ? (officers.ADMIN_ACTIVE_STATUS == "N" ? null : officers.ADMIN_ACTIVE_START_DATE) : officers.ADMIN_ACTIVE_START_DATE,
        //                        ADMIN_ACTIVE_END_DATE = ShowActiveAuthSig == "Y" ? (officers.ADMIN_ACTIVE_STATUS == "N" ? null : officers.ADMIN_ACTIVE_END_DATE) : officers.ADMIN_ACTIVE_END_DATE,
        //                        ADMIN_NO_MOBILE = ShowActiveAuthSig == "Y" ? (officers.ADMIN_ACTIVE_STATUS == "N" ? null : officers.ADMIN_NO_MOBILE) : officers.ADMIN_NO_MOBILE,
        //                        ADMIN_NO_EMAIL = ShowActiveAuthSig == "Y" ? (officers.ADMIN_ACTIVE_STATUS == "N" ? null : officers.ADMIN_NO_EMAIL) : officers.ADMIN_NO_EMAIL,
        //                        ADMIN_NO_ADDRESS1 = ShowActiveAuthSig == "Y" ? (officers.ADMIN_ACTIVE_STATUS == "N" ? null : officers.ADMIN_NO_ADDRESS1) : officers.ADMIN_NO_ADDRESS1,
        //                        ADMIN_NO_ADDRESS2 = ShowActiveAuthSig == "Y" ? (officers.ADMIN_ACTIVE_STATUS == "N" ? null : officers.ADMIN_NO_ADDRESS2) : officers.ADMIN_NO_ADDRESS2,
        //                        MAST_STATE_CODE = dept.MAST_STATE_CODE,
        //                        MAST_DISTRICT_CODE = dept.MAST_DISTRICT_CODE,
        //                        ADMIN_ACTIVE_STATUS = ShowActiveAuthSig == "Y" ? (officers.ADMIN_ACTIVE_STATUS == "N" ? null : officers.ADMIN_ACTIVE_STATUS) : officers.ADMIN_ACTIVE_STATUS,
        //                        MAST_ND_TYPE = dept.MAST_ND_TYPE
        //                    }).Distinct();

        //        //remove state entry
        //        query = query.Where(m => m.MAST_ND_TYPE == "D");


        //        //Remove the unwanted rows 
        //        if (ShowActiveAuthSig == "Y")
        //        {
        //            var removelistDistrict =
        //            (from item in query
        //             where item.ADMIN_ACTIVE_STATUS == "Y" && item.MAST_DISTRICT_CODE != null
        //             select item.MAST_DISTRICT_CODE).Distinct();//.ToList<int?>();


        //            var removelistState =
        //            (from item in query
        //             where item.ADMIN_ACTIVE_STATUS == "Y" && item.MAST_DISTRICT_CODE == null
        //             select item.MAST_STATE_CODE).Distinct();//.ToList<int?>();


        //            //get  state list to remove 
        //            var removedState = query.Where(item => removelistState.Contains(item.MAST_STATE_CODE) && item.ADMIN_ACTIVE_STATUS == null && item.MAST_DISTRICT_CODE == null);

        //            //get district List to remove
        //            // following code is removing the district(DPIU from list) where only one or none nodal officers are present
        //            var removedDistrict = query.Where(item => removelistDistrict.Contains(item.MAST_DISTRICT_CODE) && item.ADMIN_ACTIVE_STATUS == null && item.MAST_DISTRICT_CODE != null);

        //            //remove unwanted rows
        //            query = query.Except(removedState);
        //            query = query.Except(removedDistrict);

        //        }
        //        else
        //        {

        //            query = query.Where(c => c.ADMIN_ACTIVE_STATUS.ToLower() == "n");
        //            //query = query.Where(c => c.ADMIN_ACTIVE_STATUS.ToLower() == "n" && (c.ADMIN_ACTIVE_STATUS.ToUpper() == "Y" && c.ADMIN_ACTIVE_START_DATE == null));                                                      
        //        }

        //        if (searchStatus == "W") //search only currently working records
        //        {
        //            query = query.Where(c => c.ADMIN_ACTIVE_STATUS.ToLower() == "y");
        //        }

        //        // apply search criteria
        //        if (searchCriteria.Trim() != "")
        //        {
        //            string[] names = searchCriteria.Split(' ');

        //            if (names.Count() == 1)
        //            {
        //                String firstname = names[0];


        //                query =
        //                       from result in query
        //                       where
        //                           result.ADMIN_NO_FULL_NAME.Contains(firstname)
        //                       select result;
        //            }

        //            else if (names.Count() == 2)
        //            {
        //                String firstname = names[0];
        //                String lastName = names[1];

        //                query =
        //                       from result in query
        //                       where
        //                           result.ADMIN_NO_FULL_NAME.Contains(firstname) && result.ADMIN_NO_FULL_NAME.Contains(lastName)
        //                       select result;
        //            }
        //            else if (names.Count() == 3)
        //            {
        //                String firstname = names[0];
        //                String middleName = names[1];
        //                String lastName = names[2];

        //                query = from result in query
        //                        where
        //                            result.ADMIN_NO_FULL_NAME.Contains(firstname) && result.ADMIN_NO_FULL_NAME.Contains(middleName) && result.ADMIN_NO_FULL_NAME.Contains(lastName)
        //                        select result;
        //            }
        //        }




        //        //find out total rows
        //        totalRecords = query == null ? 0 : query.Count();
        //        if (sidx.Trim() != string.Empty)
        //        {
        //            //ascending order and get rows of the page
        //            if (sord.ToString() == "asc")
        //            {
        //                // query = query.OrderBy(x => x.ADMIN_ND_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows));
        //                //query = query.OrderByDescending(x => x.MAST_ND_TYPE == "S").ThenBy(x => x.ADMIN_ND_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows));

        //                //added by abhishek
        //                query = query.OrderByDescending(x => x.MAST_ND_TYPE == "S").ThenBy(x => x.ADMIN_ND_NAME);

        //            }
        //            else
        //            {
        //                // query = query.OrderByDescending(x => x.MAST_ND_TYPE).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows));
        //                //query = query.OrderByDescending(x => x.MAST_ND_TYPE == "S").ThenByDescending(x => x.ADMIN_ND_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows));

        //                //added by abhishek
        //                query = query.OrderByDescending(x => x.MAST_ND_TYPE == "S").ThenByDescending(x => x.ADMIN_ND_NAME);
        //            }
        //        }
        //        else
        //        {
        //            query = query.OrderByDescending(x => x.MAST_ND_TYPE == "S").ThenBy(x => x.ADMIN_ND_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows));
        //            //added by abhishek
        //            //query = query.OrderByDescending(x => x.MAST_ND_TYPE == "S").ThenBy(x => x.ADMIN_ND_NAME);
        //        }


        //        foreach (var item in query)
        //        {
        //            AuthorizedSignatoryModel objAuthSig = new AuthorizedSignatoryModel();
        //            objAuthSig.ND_CODE = item.ADMIN_ND_CODE;
        //            objAuthSig.ADMIN_NO_OFFICER_CODE = item.ADMIN_NO_OFFICER_CODE;
        //            objAuthSig.ADMIN_ND_NAME = item.ADMIN_ND_NAME;
        //            objAuthSig.ADMIN_FULL_NAME = item.ADMIN_NO_FULL_NAME;
        //            objAuthSig.ADMIN_NO_DESIGNATION = item.ADMIN_NO_DESIGNATION;
        //            if (item.ADMIN_ACTIVE_START_DATE.HasValue)
        //            {
        //                objAuthSig.START_DATE = objCommon.GetDateTimeToString(item.ADMIN_ACTIVE_START_DATE.Value);
        //            }
        //            if (item.ADMIN_ACTIVE_END_DATE.HasValue)
        //            {
        //                objAuthSig.END_DATE = objCommon.GetDateTimeToString(item.ADMIN_ACTIVE_END_DATE.Value);
        //            }

        //            objAuthSig.ADMIN_MOBILE = item.ADMIN_NO_MOBILE;
        //            objAuthSig.ADMIN_EMAIL = item.ADMIN_NO_EMAIL;
        //            objAuthSig.ADMIN_ADDRESS1 = item.ADMIN_NO_ADDRESS1;
        //            objAuthSig.ADMIN_ADDRESS2 = item.ADMIN_NO_ADDRESS2;
        //            objAuthSig.STATE_CODE = item.MAST_STATE_CODE;
        //            objAuthSig.MAST_DISTRICT_CODE = item.MAST_DISTRICT_CODE;
        //            objAuthSig.ADMIN_ACTIVE_STATUS = item.ADMIN_ACTIVE_STATUS;
        //            AuthSigList.Add(objAuthSig);
        //        }

        //        return AuthSigList;

        //    }
        //    catch (Exception ex)
        //    {
        //        Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
        //        dbContext.Dispose();
        //        throw new Exception("Error while getting authorized signatories....");

        //    }
        //    finally
        //    {
        //        dbContext.Dispose();
        //    }
        //}


        public List<AuthorizedSignatoryModel> GetAuthorizedSignatoryDetails(int? page, int? rows, string sidx, string sord, int int_state_code, string ShowActiveAuthSig, string searchCriteria, out long totalRecords)
        {
            string Level = string.Empty;
            string searchStatus = ShowActiveAuthSig;
            if (ShowActiveAuthSig == "W")
            {
                ShowActiveAuthSig = "Y";
            }
            Level = PMGSYSession.Current.LevelId == 4 ? "S" : "D";

            try
            {
                CommonFunctions objCommon = new CommonFunctions();
                List<AuthorizedSignatoryModel> AuthSigList = new List<AuthorizedSignatoryModel>();
                String param = string.Empty;
                dbContext = new PMGSYEntities();
                //Nullable<int> noDesig = 517;

                int stateAdminCode = 0;
                int dpiuAdminCode = 0;

                if(PMGSYSession.Current.LevelId == 4)
                {
                    stateAdminCode = PMGSYSession.Current.AdminNdCode;
                }

                if(PMGSYSession.Current.LevelId == 5)
                {
                    stateAdminCode = PMGSYSession.Current.ParentNDCode.Value;
                    dpiuAdminCode = PMGSYSession.Current.AdminNdCode;
                }

                List<USP_ACC_DISPLAY_AUTHORISE_SIGNATORY_DETAILS_Result> query = dbContext.USP_ACC_DISPLAY_AUTHORISE_SIGNATORY_DETAILS (stateAdminCode,dpiuAdminCode,PMGSYSession.Current.LevelId,ShowActiveAuthSig).ToList();

                //remove state entry
                //query = query.Where(m => m.mast_nd_type == "DPIU").ToList();
                //To add State Entry
                query = query.Where(m => m.mast_nd_type == "DPIU" || m.mast_nd_type == "SRRDA").ToList();

                // apply search criteria
                if (searchCriteria.Trim() != "")
                {
                    string[] names = searchCriteria.Split(' ');

                    if (names.Count() == 1)
                    {
                        String firstname = names[0];
                        query =
                               (from result in query
                               where
                                   result.ADMIN_NO_NAME.Contains(firstname)
                               select result).ToList();
                    }

                    else if (names.Count() == 2)
                    {
                        String firstname = names[0];
                        String lastName = names[1];
                        query =
                               (from result in query
                               where
                                   result.ADMIN_NO_NAME.Contains(firstname) && result.ADMIN_NO_NAME.Contains(lastName)
                               select result).ToList();
                    }
                    else if (names.Count() == 3)
                    {
                        String firstname = names[0];
                        String middleName = names[1];
                        String lastName = names[2];

                        query = (from result in query
                                where
                                    result.ADMIN_NO_NAME.Contains(firstname) && result.ADMIN_NO_NAME.Contains(middleName) && result.ADMIN_NO_NAME.Contains(lastName)
                                select result).ToList();
                    }
                }

                //find out total rows
                totalRecords = query == null ? 0 : query.Count();
                if (sidx.Trim() != string.Empty)
                {
                    //ascending order and get rows of the page
                    if (sord.ToString() == "asc")
                    {
                        query = query.OrderByDescending(x => x.mast_nd_type == "SRRDA").ThenBy(x => x.ADMIN_ND_NAME).ToList();
                    }
                    else
                    {
                        query = query.OrderByDescending(x => x.mast_nd_type == "SRRDA").ThenByDescending(x => x.ADMIN_ND_NAME).ToList();
                    }
                }
                else
                {
                    query = query.OrderByDescending(x => x.mast_nd_type == "SRRDA").ThenBy(x => x.ADMIN_ND_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                    //added by abhishek
                    //query = query.OrderByDescending(x => x.MAST_ND_TYPE == "S").ThenBy(x => x.ADMIN_ND_NAME);
                }


                foreach (var item in query)
                {
                    AuthorizedSignatoryModel objAuthSig = new AuthorizedSignatoryModel();
                    objAuthSig.ND_CODE = item.ADMIN_ND_CODE;
                    objAuthSig.ADMIN_NO_OFFICER_CODE = item.ADMIN_NO_OFFICER_CODE;
                    objAuthSig.ADMIN_ND_NAME = item.ADMIN_ND_NAME;
                    objAuthSig.ADMIN_FULL_NAME = item.ADMIN_NO_NAME;
                    objAuthSig.ADMIN_NO_DESIGNATION = 26;
                    if (!(String.IsNullOrEmpty(item.ADMIN_ACTIVE_START_DATE)))
                    {
                        objAuthSig.START_DATE = item.ADMIN_ACTIVE_START_DATE;
                    }
                    if (!(String.IsNullOrEmpty(item.ADMIN_ACTIVE_END_DATE)))
                    {
                        objAuthSig.END_DATE = item.ADMIN_ACTIVE_END_DATE;
                    }

                    objAuthSig.ADMIN_MOBILE = item.ADMIN_NO_MOBILE;
                    objAuthSig.ADMIN_EMAIL = item.ADMIN_NO_EMAIL;
                    objAuthSig.ADMIN_ADDRESS1 = item.ADMIN_NO_ADDRESS1;
                    objAuthSig.ADMIN_ADDRESS2 = item.ADMIN_NO_ADDRESS2;
                    objAuthSig.STATE_CODE = dbContext.MASTER_STATE.Where(m=>m.MAST_STATE_NAME == item.MAST_STATE_NAME).Select(m=>m.MAST_STATE_CODE).FirstOrDefault();
                    objAuthSig.MAST_DISTRICT_CODE = dbContext.MASTER_DISTRICT.Where(m => m.MAST_DISTRICT_NAME == item.MAST_DISTRICT_NAME).Select(m => m.MAST_DISTRICT_CODE).FirstOrDefault();
                    objAuthSig.ADMIN_ACTIVE_STATUS = item.ADMIN_ACTIVE_STATUS;
                    objAuthSig.DSCRegistered = item.IsRegistered;
                    
                    AuthSigList.Add(objAuthSig);
                }

                return AuthSigList;

            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                dbContext.Dispose();
                throw new Exception("Error while getting authorized signatories....");

            }
            finally
            {
                dbContext.Dispose();
            }
        }




        //public AuthorizedSignatoryModel GetAuthorizedSignatoryDetails(int adminNdCode)
        //{
        //    AuthorizedSignatoryModel authModel = new AuthorizedSignatoryModel();
        //    CommonFunctions objCommon = new CommonFunctions();

        //    dbContext = new PMGSYEntities();

        //    var query = ( from result in dbContext.ADMIN_NODAL_OFFICERS
        //                  where result.ADMIN_ND_CODE == adminNdCode && result.ADMIN_NO_DESIGNATION ==26 &&  result.ADMIN_ACTIVE_STATUS == "Y" && result.ADMIN_MODULE == "A"
        //                  select result).FirstOrDefault();

        //    authModel.ADMIN_OFFICER_CODE = query.ADMIN_NO_OFFICER_CODE;
        //    authModel.ADMIN_ADDRESS1 = query.ADMIN_NO_ADDRESS1;
        //    authModel.ADMIN_ADDRESS2 = query.ADMIN_NO_ADDRESS2;
        //    authModel.ADMIN_FNAME = query.ADMIN_NO_FNAME;
        //    authModel.ADMIN_MNAME = query.ADMIN_NO_MNAME == null ? string.Empty : query.ADMIN_NO_MNAME;
        //    authModel.ADMIN_LNAME = query.ADMIN_NO_LNAME;
        //    authModel.ADMIN_NO_DESIGNATION = query.ADMIN_NO_DESIGNATION;
        //    authModel.MAST_DISTRICT_CODE = query.MAST_DISTRICT_CODE;
        //    authModel.ADMIN_PIN = query.ADMIN_NO_PIN;
        //    authModel.ADMIN_RESIDENCE_STD = query.ADMIN_NO_RESIDENCE_STD;
        //    authModel.ADMIN_RESIDENCE_PHONE = query.ADMIN_NO_RESIDENCE_PHONE;
        //    authModel.ADMIN_OFFICE_STD = query.ADMIN_NO_OFFICE_STD;
        //    authModel.ADMIN_OFFICE_PHONE = query.ADMIN_NO_OFFICE_PHONE;
        //    authModel.ADMIN_STD_FAX = query.ADMIN_NO_STD_FAX;
        //    authModel.ADMIN_FAX = query.ADMIN_NO_FAX;
        //    authModel.ADMIN_MOBILE = query.ADMIN_NO_MOBILE;
        //    authModel.ADMIN_EMAIL = query.ADMIN_NO_EMAIL;
        //    if (query.ADMIN_ACTIVE_START_DATE.HasValue)
        //    {
        //        authModel.startDate = objCommon.GetDateTimeToString(query.ADMIN_ACTIVE_START_DATE.Value);
        //    }
        //    if (query.ADMIN_ACTIVE_START_DATE.HasValue)
        //    {
        //        authModel.START_DATE = objCommon.GetDateTimeToString(query.ADMIN_ACTIVE_START_DATE.Value);
        //    }
        //    authModel.EndDate = query.ADMIN_ACTIVE_END_DATE != null ? objCommon.GetDateTimeToString(query.ADMIN_ACTIVE_END_DATE.Value) : string.Empty;
        //    authModel.ADMIN_REMARKS = query.ADMIN_NO_REMARKS;
        //    if (query.ADMIN_AADHAR_PAN_FLAG == "A")
        //    {
        //        authModel.ADMIN_AADHAR_NO = query.ADMIN_AADHAR_NO;
        //    }
        //    else if (query.ADMIN_AADHAR_PAN_FLAG == "P")
        //    {
        //        authModel.ADMIN_PAN_NO = query.ADMIN_AADHAR_NO;
        //    }
        //    authModel.ADMIN_AADHAR_PAN_FLAG = query.ADMIN_AADHAR_PAN_FLAG;
        //    return authModel;

        //}


        /// <summary>
        /// BAL function to get  the authorized signatory details based on admin_nd_code
        /// </summary>
        /// <param name="adminNdCode">Admin_ND_Code</param>
        /// <param name="status"> Active or non active</param>
        /// <returns></returns>
        public AuthorizedSignatoryModel GetAuthorizedSignatoryDetails(int adminNdCode, int adminNoOfficerCode, string status)
        {
            AuthorizedSignatoryModel model = new AuthorizedSignatoryModel();
            CommonFunctions objCommon = new CommonFunctions();
            dbContext = new PMGSYEntities();
            try
            {

                //get the details of the authorized signatory
                var query = (from result in dbContext.ADMIN_NODAL_OFFICERS
                             where result.ADMIN_NO_OFFICER_CODE==adminNoOfficerCode && result.ADMIN_ND_CODE == adminNdCode && result.ADMIN_ACTIVE_STATUS == status && result.ADMIN_MODULE == "A"
                             select result).FirstOrDefault();

                model.ADMIN_OFFICER_CODE = query.ADMIN_NO_OFFICER_CODE;
                model.ADMIN_ADDRESS1 = query.ADMIN_NO_ADDRESS1;
                model.ADMIN_ADDRESS2 = query.ADMIN_NO_ADDRESS2;
                model.ADMIN_FNAME = query.ADMIN_NO_FNAME;
                model.ADMIN_MNAME = query.ADMIN_NO_MNAME == null ? string.Empty : query.ADMIN_NO_MNAME;
                model.ADMIN_LNAME = query.ADMIN_NO_LNAME;
                model.ADMIN_NO_DESIGNATION = query.ADMIN_NO_DESIGNATION;
                model.ADMIN_ADDRESS1 = query.ADMIN_NO_ADDRESS1;
                model.ADMIN_ADDRESS2 = query.ADMIN_NO_ADDRESS2;
                model.MAST_DISTRICT_CODE = query.MAST_DISTRICT_CODE;
                model.ADMIN_PIN = query.ADMIN_NO_PIN;
                model.ADMIN_RESIDENCE_STD = query.ADMIN_NO_RESIDENCE_STD;
                model.ADMIN_RESIDENCE_PHONE = query.ADMIN_NO_RESIDENCE_PHONE;
                model.ADMIN_OFFICE_STD = query.ADMIN_NO_OFFICE_STD;
                model.ADMIN_OFFICE_PHONE = query.ADMIN_NO_OFFICE_PHONE;
                model.ADMIN_STD_FAX = query.ADMIN_NO_STD_FAX;
                model.ADMIN_FAX = query.ADMIN_NO_FAX;
                model.ADMIN_MOBILE = query.ADMIN_NO_MOBILE;
                model.ADMIN_EMAIL = query.ADMIN_NO_EMAIL;

                if (query.ADMIN_ACTIVE_START_DATE.HasValue)
                {
                    model.startDate = objCommon.GetDateTimeToString(query.ADMIN_ACTIVE_START_DATE.Value);
                }
                if (query.ADMIN_ACTIVE_START_DATE.HasValue)
                {
                    model.START_DATE = objCommon.GetDateTimeToString(query.ADMIN_ACTIVE_START_DATE.Value);
                }

                model.EndDate = query.ADMIN_ACTIVE_END_DATE != null ? objCommon.GetDateTimeToString(query.ADMIN_ACTIVE_END_DATE.Value) : string.Empty;

                model.END_DATE = model.EndDate;

                model.ADMIN_REMARKS = query.ADMIN_NO_REMARKS;


                if (query.ADMIN_AADHAR_PAN_FLAG == "A")
                {
                    model.ADMIN_AADHAR_NO = query.ADMIN_AADHAR_NO;
                }
                else if (query.ADMIN_AADHAR_PAN_FLAG == "P")
                {
                    model.ADMIN_PAN_NO = query.ADMIN_AADHAR_NO;
                }
                model.ADMIN_AADHAR_PAN_FLAG = query.ADMIN_AADHAR_PAN_FLAG;

                return model;
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                throw new Exception("Error While getting authorized signatory details to view or edit");

            }
            finally
            {
                dbContext.Dispose();
            }


        }


        /// <summary>
        /// function to add edit the authorized signatory details
        /// </summary>
        /// <param name="adminNdCode"></param>
        /// <param name="Operation">Add (A) or Edit (E)</param>
        /// <param name="model"> model to save /update</param>
        /// <returns></returns>
        public String AddEditAuthorizedSignatoryDetails(int adminNoOfficerCode, int adminNdCode, int officerCode, int DistrictCode, string Operation, AuthorizedSignatoryModel model, ref string message)
        {
            CommonFunctions objCommon = new CommonFunctions();
            //save the details in the database
            try
            {
                dbContext = new PMGSYEntities();
                using (var scope = new TransactionScope())
                {
                    ADMIN_NODAL_OFFICERS newmodel = new ADMIN_NODAL_OFFICERS();

                    if (Operation == "E")
                    {
                        newmodel = dbContext.ADMIN_NODAL_OFFICERS.Where(m => m.ADMIN_NO_OFFICER_CODE == adminNoOfficerCode).FirstOrDefault();                    
                    }

                    if (model.END_DATE != String.Empty && model.END_DATE != null)
                    {
                        newmodel.ADMIN_ACTIVE_END_DATE = objCommon.GetStringToDateTime(model.END_DATE);
                    }


                    if (dbContext.ADMIN_NODAL_OFFICERS.Any(no=>no.ADMIN_NO_OFFICER_CODE==adminNoOfficerCode && no.ADMIN_ACTIVE_STATUS == "N" && no.ADMIN_ND_CODE == adminNdCode && no.ADMIN_MODULE == "A"))
                    {
                        DateTime? endDate = dbContext.ADMIN_NODAL_OFFICERS.Where(no => no.ADMIN_ACTIVE_STATUS == "N" && no.ADMIN_ND_CODE == adminNdCode && no.ADMIN_MODULE == "A").Max(no => no.ADMIN_ACTIVE_END_DATE);

                        if (objCommon.GetStringToDateTime(model.START_DATE) < endDate)
                        {
                            message = "Authorized Signatory start date should be greater than or equal to previous authorized signatory end date.";
                            return "0";
                        }

                    }
                    if (model.END_DATE != null)
                    {

                        if (objCommon.GetStringToDateTime(model.END_DATE) > System.DateTime.Now)
                        {
                            message = "Authorized Signatory end date should not be greater than current date.";
                            return "0";
                        }
                    }

                    int max = 0;

                    if (Operation == "A")
                    {
                        if (dbContext.ADMIN_NODAL_OFFICERS.Any())
                        {
                            max = dbContext.ADMIN_NODAL_OFFICERS.Max(c => c.ADMIN_NO_OFFICER_CODE);
                        }
                        newmodel.ADMIN_NO_OFFICER_CODE = max + 1;
                    }
                    else
                    {
                        newmodel.ADMIN_NO_OFFICER_CODE = officerCode;
                    }


                    newmodel.ADMIN_ND_CODE = adminNdCode;
                    newmodel.ADMIN_NO_FNAME = model.ADMIN_FNAME;
                    newmodel.ADMIN_NO_MNAME = model.ADMIN_MNAME == null ? string.Empty : model.ADMIN_MNAME;
                    newmodel.ADMIN_NO_LNAME = model.ADMIN_LNAME;
                    newmodel.ADMIN_NO_DESIGNATION = model.ADMIN_NO_DESIGNATION;
                    newmodel.ADMIN_NO_ADDRESS1 = model.ADMIN_ADDRESS1;
                    newmodel.ADMIN_NO_ADDRESS2 = model.ADMIN_ADDRESS2;
                    newmodel.MAST_DISTRICT_CODE = model.MAST_DISTRICT_CODE;
                    newmodel.ADMIN_NO_PIN = model.ADMIN_PIN;
                    newmodel.ADMIN_NO_RESIDENCE_STD = model.ADMIN_RESIDENCE_STD;
                    newmodel.ADMIN_NO_RESIDENCE_PHONE = model.ADMIN_RESIDENCE_PHONE;
                    newmodel.ADMIN_NO_OFFICE_STD = model.ADMIN_OFFICE_STD;
                    newmodel.ADMIN_NO_OFFICE_PHONE = model.ADMIN_OFFICE_PHONE;
                    newmodel.ADMIN_NO_STD_FAX = model.ADMIN_STD_FAX;
                    newmodel.ADMIN_NO_FAX = model.ADMIN_FAX;
                    newmodel.ADMIN_NO_MOBILE = model.ADMIN_MOBILE;
                    newmodel.ADMIN_NO_EMAIL = model.ADMIN_EMAIL;
                    newmodel.ADMIN_NO_MAIL_FLAG = model.ADMIN_EMAIL == string.Empty ? "N" : "Y";
                    //newmodel.ADMIN_NO_TYPE = 1;
                    //Modified by Abhishek Sugested By Dev Sir 24-June-2014
                    newmodel.ADMIN_NO_TYPE = 8;
                    newmodel.ADMIN_NO_LEVEL = DistrictCode == 0 ? "S" : "D";
                    newmodel.ADMIN_NO_REMARKS = model.ADMIN_REMARKS;
                    newmodel.ADMIN_MODULE = "A";
                    if (model.START_DATE != String.Empty && model.START_DATE != null)
                    {
                        newmodel.ADMIN_ACTIVE_START_DATE = objCommon.GetStringToDateTime(model.START_DATE);
                    }
                    if (model.END_DATE != String.Empty && model.END_DATE != null)
                    {
                        newmodel.ADMIN_ACTIVE_END_DATE = objCommon.GetStringToDateTime(model.END_DATE);
                    }

                    //Modified 6May2015 for Aadhaar / Pan Number
                    if (model.ADMIN_AADHAR_PAN_FLAG.Equals("A"))
                    {
                        newmodel.ADMIN_AADHAR_NO = model.ADMIN_AADHAR_NO;
                    }
                    else if (model.ADMIN_AADHAR_PAN_FLAG.Equals("P"))
                    {
                        newmodel.ADMIN_AADHAR_NO = model.ADMIN_PAN_NO;
                    }
                    newmodel.ADMIN_AADHAR_PAN_FLAG = model.ADMIN_AADHAR_PAN_FLAG;

                    if (Operation == "A")
                    {
                        //newmodel.ADMIN_AUTH_CODE = null;
                        //added by PP
                        newmodel.IS_VALID_XML = false; //dsc registration is pending
                        newmodel.XML_FINALIZATION_DATE = null;
                        //end

                        newmodel.ADMIN_ACTIVE_STATUS = newmodel.ADMIN_ACTIVE_END_DATE == null ? "Y" : "N";
                        dbContext.ADMIN_NODAL_OFFICERS.Add(newmodel);
                        if (newmodel.ADMIN_ACTIVE_STATUS.Equals("N"))
                        {
                            UM_User_Master umMaster = dbContext.UM_User_Master.Where(m => m.Admin_ND_Code == newmodel.ADMIN_ND_CODE && m.DefaultRoleID == 26).FirstOrDefault();
                            if (umMaster != null)
                            {
                                umMaster.IsLocked = true;

                            }

                        }
                        dbContext.SaveChanges();
                    }
                    else
                    {         
                        newmodel.ADMIN_ACTIVE_STATUS = newmodel.ADMIN_ACTIVE_END_DATE == null ? "Y" : "N";
                        //added by PP
                        if (model.IsDscDetailChanged == "Y")
                        {
                            newmodel.IS_VALID_XML = false;
                            newmodel.XML_FINALIZATION_DATE = null;
                        }
                        //end
                        dbContext.Entry(newmodel).State = System.Data.Entity.EntityState.Modified;
                       
                        if (newmodel.ADMIN_ACTIVE_STATUS.Equals("N"))
                        {
                            UM_User_Master umMaster = dbContext.UM_User_Master.Where(m => m.Admin_ND_Code == newmodel.ADMIN_ND_CODE && m.DefaultRoleID == 
                               26).FirstOrDefault();
                            if (umMaster != null)
                            {
                                umMaster.IsLocked = true;
                            }
                        }
                        dbContext.SaveChanges();
                    }

                    scope.Complete();

                    return "1";
                }
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                throw ex;
            }
            finally
            {
                dbContext.Dispose();
            }

        }

        public int DeRegisterDSC(int adminNoOfficerCode)
        {
             PMGSYEntities cerDbContext = new PMGSYEntities();
            try
            {
                int returnFlag = 0;
                using (var scope = new TransactionScope())
                {
                    ACC_CERTIFICATE_DETAILS  cerDetails = cerDbContext.ACC_CERTIFICATE_DETAILS.SingleOrDefault(p => p.ADMIN_NO_OFFICER_CODE == adminNoOfficerCode);
                    if (cerDetails != null)
                    {
                        cerDetails.USERID = PMGSYSession.Current.UserId;
                        cerDetails.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                        cerDbContext.Entry(cerDetails).State = System.Data.Entity.EntityState.Modified;
                        cerDbContext.SaveChanges();
                        cerDbContext.Database.ExecuteSqlCommand
                       ("DELETE [omms].ACC_CERTIFICATE_DETAILS Where ADMIN_NO_OFFICER_CODE = {0}", adminNoOfficerCode);

                        #region PFMS
                        ADMIN_NODAL_OFFICERS nodalOfficer = cerDbContext.ADMIN_NODAL_OFFICERS.Where(x => x.ADMIN_NO_OFFICER_CODE == adminNoOfficerCode).FirstOrDefault();
                        if (nodalOfficer != null)
                        {
                            nodalOfficer.IS_VALID_XML = false;
                            nodalOfficer.XML_FINALIZATION_DATE = null;
                            cerDbContext.Entry(nodalOfficer).State = System.Data.Entity.EntityState.Modified;
                        }

                        //PFMS_OMMAS_DSC_MAPPING pfms = cerDbContext.PFMS_OMMAS_DSC_MAPPING.Where(x => x.ADMIN_ND_CODE == cerDetails.ADMIN_NODAL_OFFICERS.ADMIN_ND_CODE).OrderByDescending(c=>c.FILE_PROCESS_DATE).FirstOrDefault();
                        //if (pfms != null)
                        //{
                        //    pfms.IS_ACTIVE = false;
                        //    cerDbContext.Entry(pfms).State = System.Data.Entity.EntityState.Modified;
                        //}


                        //REAT Deregister
                        REAT_OMMAS_DSC_MAPPING DscMapping = cerDbContext.REAT_OMMAS_DSC_MAPPING.Where(x => x.ADMIN_ND_CODE == cerDetails.ADMIN_NODAL_OFFICERS.ADMIN_ND_CODE && x.ADMIN_NO_OFFICER_CODE == adminNoOfficerCode && x.IS_ACTIVE == true && x.FUND_TYPE =="P").OrderByDescending(c => c.FILE_PROCESS_DATE).FirstOrDefault();
                        if (DscMapping != null)
                        {
                            DscMapping.IS_ACTIVE = false;
                            cerDbContext.Entry(DscMapping).State = System.Data.Entity.EntityState.Modified;
                        }
                        #endregion

                        cerDbContext.SaveChanges();
                        scope.Complete();
                        returnFlag = 1;
                    }
                    else
                    {
                        returnFlag = 0; 
                    }
                    return returnFlag;
                }

            
            }
            catch (System.Data.Entity.Validation.DbEntityValidationException e)
            {
                string rs = "";
                foreach (var eve in e.EntityValidationErrors)
                {
                    rs = string.Format("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:", eve.Entry.Entity.GetType().Name, eve.Entry.State);
                    Console.WriteLine(rs);

                    foreach (var ve in eve.ValidationErrors)
                    {
                        rs += "<br />" + string.Format("- Property: \"{0}\", Error: \"{1}\"", ve.PropertyName, ve.ErrorMessage);
                    }
                }
               
                ErrorLog.LogError(new Exception(rs), "Authorized Signatoty.DeRegisterDSC111()");
                throw new Exception(rs);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "Authorized Signatoty.DeRegisterDSC()");
                throw new Exception("Error while deleting registered certificate details.");
            }
            finally
            {
               // cerDbContext.SaveChanges();
                cerDbContext.Dispose();
            }

        }


    }

    public interface IAuthorizedSignatoryDAL
    {
        List<AuthorizedSignatoryModel> GetAuthorizedSignatoryDetails(int? page, int? rows, string sidx, string sord, int int_state_code, string ShowActiveAuthSig, string searchCriteria, out long totalRecords);
        AuthorizedSignatoryModel GetAuthorizedSignatoryDetails(int adminNdCode, int adminNoOfficerCode, string status);
        String AddEditAuthorizedSignatoryDetails(int adminNoOfficerCode,int adminNdCode, int officerCode, int DistrictCode, string Operation, AuthorizedSignatoryModel model, ref string message);
        //AuthorizedSignatoryModel GetAuthorizedSignatoryDetails(int adminNdCode);

        int DeRegisterDSC(int adminNoOfficerCode);

    }
}