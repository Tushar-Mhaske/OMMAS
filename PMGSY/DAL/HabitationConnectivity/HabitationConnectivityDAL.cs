using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PMGSY.Extensions;
using System.Data.Entity;
using PMGSY.Common;
using PMGSY.Models;
using System.Transactions;

namespace PMGSY.DAL.StateLogin
{
    public class HabitationConnectivityDAL : IHabitationConnectivityDAL
    {
        public bool EditHabStatus(int habCode, ref string message)
        {
            PMGSYEntities dbContext = new PMGSYEntities();
            try
            {
                var roadCode = dbContext.IMS_BENEFITED_HABS.Where(x => x.MAST_HAB_CODE == habCode).Select(x=>x.IMS_PR_ROAD_CODE).FirstOrDefault();
                var status = dbContext.IMS_SANCTIONED_PROJECTS.Where(x => x.IMS_PR_ROAD_CODE == roadCode).Select(x=>x.IMS_ISCOMPLETED).FirstOrDefault();

                var updatestatus = dbContext.MASTER_HABITATIONS_DETAILS.Where(x => x.MAST_HAB_CODE == habCode && x.MAST_YEAR == 2001).Select(x=>x.MAST_HAB_CONNECTED).FirstOrDefault();

                MASTER_HABITATIONS habitations = dbContext.MASTER_HABITATIONS.Where(m => m.MAST_HAB_CODE == habCode).FirstOrDefault();
                if (habitations != null)
                {
                    if (status != "C" && status != "X")
                    {
                        updatestatus = updatestatus == "N" ? "U" : updatestatus;
                        habitations.MAST_HAB_STATUS = updatestatus;
                    }
                    else
                    {
                        habitations.MAST_HAB_STATUS = "C";
                    }
                    habitations.USERID = PMGSYSession.Current.UserId;
                    habitations.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];

                    dbContext.Entry(habitations).State = System.Data.Entity.EntityState.Modified;
                    dbContext.SaveChanges();
                    message = "Habitation Details updated successfully";
                    return true;
                }
                else
                {
                    message = "Habitation Details not updated";
                    return false;
                }
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                message = "Habitation Details not updated";
                return false;
            }
            finally
            {
                if (dbContext != null)
                {
                    dbContext.Dispose();
                }
            }
        }

        public Array GetHabitationDetails(int districtCode, int blockCode, int page, int rows, string sidx, string sord, out long totalRecords)
        {
            PMGSYEntities dbContext = new PMGSYEntities();
            try
            {
                //var BenHabs = (from beniffitedHab in dbContext.IMS_BENEFITED_HABS select new { beniffitedHab.MAST_HAB_CODE }).Distinct();
                var BenHabs = (from beniffitedHab in dbContext.IMS_BENEFITED_HABS
                               join ISP in dbContext.IMS_SANCTIONED_PROJECTS on beniffitedHab.IMS_PR_ROAD_CODE equals ISP.IMS_PR_ROAD_CODE
                               where ISP.MAST_PMGSY_SCHEME == 1
                               select new { beniffitedHab.MAST_HAB_CODE }).Distinct();

                var villageCodes = (blockCode == 0 ? (from villageDetails in dbContext.MASTER_VILLAGE select new { villageDetails.MAST_VILLAGE_CODE }) : (from villageDetails in dbContext.MASTER_VILLAGE where villageDetails.MAST_BLOCK_CODE == blockCode select new { villageDetails.MAST_VILLAGE_CODE })).Distinct();

                var list = (from item in dbContext.MASTER_HABITATIONS
                            join habDetails in dbContext.MASTER_HABITATIONS_DETAILS
                            on item.MAST_HAB_CODE equals habDetails.MAST_HAB_CODE
                            where !BenHabs.Any(s => s.MAST_HAB_CODE == item.MAST_HAB_CODE) && (habDetails.MAST_HAB_CONNECTED == "N" || item.MAST_HAB_STATUS == "F" || item.MAST_HAB_STATUS == "S") && item.MAST_HABITATION_ACTIVE == "Y" &&
                            habDetails.MAST_YEAR == 2001 &&
                            villageCodes.Any(v => v.MAST_VILLAGE_CODE == item.MAST_VILLAGE_CODE)
                            select new
                            {
                                item.MAST_HAB_NAME,
                                item.MAST_HAB_CODE,
                                habDetails.MAST_HAB_TOT_POP,
                                habDetails.MAST_HAB_SCST_POP,
                                habDetails.MAST_HAB_CONNECTED,
                                item.MAST_HAB_STATUS
                            }).ToList();

                totalRecords = list.Count();

                if (sidx.Trim() != string.Empty)
                {
                    if (sord.ToString() == "asc")
                    {
                        switch (sidx)
                        {
                            case "HabitationName":
                                list = list.OrderBy(m => m.MAST_HAB_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                                break;
                            case "TotalPopulation":
                                list = list.OrderBy(m => m.MAST_HAB_TOT_POP).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                                break;
                            case "STPopulation":
                                list = list.OrderBy(m => m.MAST_HAB_SCST_POP).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                                break;
                            case "Connected":
                                list = list.OrderBy(m => m.MAST_HAB_CONNECTED).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                                break;
                            default:
                                list = list.OrderBy(m => m.MAST_HAB_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                                break;
                        }

                    }
                    else
                    {
                        switch (sidx)
                        {
                            case "HabitationName":
                                list = list.OrderByDescending(m => m.MAST_HAB_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                                break;
                            case "TotalPopulation":
                                list = list.OrderByDescending(m => m.MAST_HAB_TOT_POP).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                                break;
                            case "STPopulation":
                                list = list.OrderByDescending(m => m.MAST_HAB_SCST_POP).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                                break;
                            case "Connected":
                                list = list.OrderByDescending(m => m.MAST_HAB_CONNECTED).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                                break;
                            default:
                                list = list.OrderByDescending(m => m.MAST_HAB_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                                break;
                        }

                    }
                }


                return list.Select(habs => new
                {
                    id = URLEncrypt.EncryptParameters1(new string[] { "HabCode=" + habs.MAST_HAB_CODE.ToString().Trim() }),
                    cell = new[]
                     {
                         habs.MAST_HAB_NAME,
                         habs.MAST_HAB_TOT_POP.ToString(),
                         habs.MAST_HAB_SCST_POP.ToString(),
                         habs.MAST_HAB_STATUS.ToString()=="U"?"Unconnected":(habs.MAST_HAB_STATUS=="S"?"State Connected":(habs.MAST_HAB_STATUS=="F"?"Not Feasible":""))
                      
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
                if (dbContext != null)
                {
                    dbContext.Dispose();
                }
            }
        }

        //public List<SelectListItem> GetAllStates()
        //{
        //    PMGSYEntities dbContext = new PMGSYEntities();
        //    try
        //    {

        //        List<SelectListItem> list = new SelectList(dbContext.MASTER_STATE, "MAST_STATE_CODE", "MAST_STATE_NAME").ToList();
        //        list.Insert(0,(new SelectListItem { Text = "--Select State--", Value = "0", Selected = true }));
        //        return list;
        //    }

        //    catch (Exception)
        //    {
        //        return null;
        //    }
        //    finally
        //    {
        //        if (dbContext != null)
        //        {
        //            dbContext.Dispose();
        //        }
        //    }

        //}

        public List<SelectListItem> GetAllDistricts()
        {
            PMGSYEntities dbContext = new PMGSYEntities();
            try
            {
                int stateCode = PMGSYSession.Current.StateCode;

                List<SelectListItem> list = new SelectList(dbContext.MASTER_DISTRICT.Where(m => m.MAST_STATE_CODE == stateCode && m.MAST_DISTRICT_ACTIVE == "Y"), "MAST_DISTRICT_CODE", "MAST_DISTRICT_NAME").ToList();
                list.Insert(0, (new SelectListItem { Text = "--Select District--", Value = "0", Selected = true }));
                return list;
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                return null;

            }
            finally
            {
                if (dbContext != null)
                {
                    dbContext.Dispose();
                }
            }
        }

        public List<SelectListItem> GetAllBlocks(int districtCode)
        {
            PMGSYEntities dbContext = new PMGSYEntities();
            try
            {
                List<SelectListItem> listItem = new List<SelectListItem>();

                var list = (from item in dbContext.MASTER_BLOCK
                            where item.MAST_DISTRICT_CODE == districtCode && item.MAST_BLOCK_ACTIVE == "Y"
                            select new
                            {
                                item.MAST_BLOCK_CODE,
                                item.MAST_BLOCK_NAME
                            });

                foreach (var item in list)
                {
                    listItem.Add(new SelectListItem { Text = item.MAST_BLOCK_NAME, Value = item.MAST_BLOCK_CODE.ToString().Trim() });

                }
                listItem.Insert(0, new SelectListItem { Text = "--Select Block--", Value = "0", Selected = true });

                return listItem;
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                return null;
            }
            finally
            {
                if (dbContext != null)
                {
                    dbContext.Dispose();
                }
            }
            //return list;
        }

        public bool ChangeStatusOfHabitation(string encryptedHabCode, string status)
        {
            PMGSYEntities dbContext = new PMGSYEntities();
            string[] encryptedParameters = null;
            Dictionary<string, string> decryptedParameters = null;
            try
            {

                String[] habCodes = null;

                int habCode = 0;

                habCodes = encryptedHabCode.Split(',');
                if (habCodes.Count() == 0)
                {
                    return false;
                }
                foreach (String item in habCodes)
                {
                    encryptedParameters = null;
                    encryptedParameters = item.Split('/');
                    decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { encryptedParameters[0], encryptedParameters[1], encryptedParameters[2] });
                    habCode = Convert.ToInt32(decryptedParameters["HabCode"].ToString());

                    MASTER_HABITATIONS master = dbContext.MASTER_HABITATIONS.Where(hb => hb.MAST_HAB_CODE == habCode).FirstOrDefault();

                    if (status == "NF")
                    {
                        master.MAST_HAB_STATUS = "F";
                    }
                    else if (status == "SC")
                    {
                        master.MAST_HAB_STATUS = "S";
                    }
                    else if (status == "UC")
                    {
                        master.MAST_HAB_STATUS = "U";
                    }
                    master.USERID = PMGSYSession.Current.UserId;
                    master.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                    dbContext.Entry(master).State = System.Data.Entity.EntityState.Modified;
                    dbContext.SaveChanges();
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
                if (dbContext != null)
                {
                    dbContext.Dispose();
                }
            }
        }

        public bool ChangeStatusAsPerCensusYearOfHabitation(string encryptedHabCode, string status)
        {
            PMGSYEntities dbContext = new PMGSYEntities();
            string[] encryptedParameters = null;
            Dictionary<string, string> decryptedParameters = null;
            using (TransactionScope objScope = new TransactionScope())
            {
                try
                {

                    String[] habCodes = null;

                    int habCode = 0;

                    habCodes = encryptedHabCode.Split(',');
                    if (habCodes.Count() == 0)
                    {
                        return false;
                    }
                    foreach (String item in habCodes)
                    {
                        encryptedParameters = null;
                        encryptedParameters = item.Split('/');
                        decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { encryptedParameters[0], encryptedParameters[1], encryptedParameters[2] });
                        habCode = Convert.ToInt32(decryptedParameters["HabCode"].ToString());

                        var habDetail = dbContext.MASTER_HABITATIONS_DETAILS.Where(hb => hb.MAST_HAB_CODE == habCode && hb.MAST_YEAR == 2001).Select(m => m.MAST_HAB_CONNECTED).FirstOrDefault();

                        MASTER_HABITATIONS master = dbContext.MASTER_HABITATIONS.Where(hb => hb.MAST_HAB_CODE == habCode).FirstOrDefault();


                        if (habDetail == "Y")
                        {
                            master.MAST_HAB_STATUS = "C";
                        }
                        else
                        {
                            master.MAST_HAB_STATUS = "U";
                        }
                        master.USERID = PMGSYSession.Current.UserId;
                        master.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                        dbContext.Entry(master).State = System.Data.Entity.EntityState.Modified;
                        dbContext.SaveChanges();
                    }
                    objScope.Complete();
                    return true;
                }
                catch (Exception ex)
                {
                    Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                    return false;
                }
                finally
                {
                    if (dbContext != null)
                    {
                        dbContext.Dispose();
                    }
                }
            }
        }
        public Array GetAllDetails_ByBlockCode(int blockCode, int page, int rows, string sidx, string sord, out long totalRecords)
        {
            PMGSYEntities dbContext = new PMGSYEntities();
            try
            {
                int PMGSY = PMGSYSession.Current.PMGSYScheme;
                var TotalHabitation = dbContext.get_Habitation_Details(blockCode, "TH", PMGSY).ToList();//New
                var TotalProposals = dbContext.get_Habitation_Details(blockCode, "BH", PMGSY).ToList();
                var ConnectedHabitation = dbContext.get_Habitation_Details(blockCode, "HC", PMGSY).ToList();
                var UnconnectedHabitation = dbContext.get_Habitation_Details(blockCode, "TU", PMGSY).ToList(); //new
                var NotConnectedHabitation = dbContext.get_Habitation_Details(blockCode, "NC", PMGSY).ToList();
                var FeasibleHabs = dbContext.get_Habitation_Details(blockCode, "IF", PMGSY).ToList();
                var StateConnectedHabs = dbContext.get_Habitation_Details(blockCode, "SC", PMGSY).ToList();
                var BenifitedHabs = dbContext.get_Habitation_Details(blockCode, "UB", PMGSY).ToList();//BH
                int balanceHabs = UnconnectedHabitation.Select(s => s.Value).FirstOrDefault() - BenifitedHabs.Select(s => s.Value).FirstOrDefault();
                string BlockName = dbContext.MASTER_BLOCK.Where(m => m.MAST_BLOCK_CODE == blockCode).Select(m => m.MAST_BLOCK_NAME).FirstOrDefault();


                totalRecords = 1;

                return TotalHabitation.Select(habs => new
                {

                    cell = new[]
                {   
                    BlockName.ToString(),
           
                    "<a href='#' title='Click here to get total habitation details'  onClick=GetTotalHabsDetails('" + URLEncrypt.EncryptParameters1(new string[]{"BlockCode="+blockCode.ToString().Trim()}) +"'); return false;'>"+ TotalHabitation.FirstOrDefault().ToString()+"</a>",
                    "<a href='#' title='Click here to get total Prposal habitation details'  onClick=GetTotalProposalDetails('" + URLEncrypt.EncryptParameters1(new string[]{"BlockCode="+blockCode.ToString().Trim()}) +"'); return false;'>"+ TotalProposals.FirstOrDefault().ToString()+"</a>",
                    "<a href='#' title='Click here to get connected habitation details'  onClick=GetConnectedHabsDetails('" + URLEncrypt.EncryptParameters1(new string[]{"BlockCode="+blockCode.ToString().Trim()}) +"'); return false;'>"+ ConnectedHabitation.FirstOrDefault().ToString()+"</a>",
                    "<a href='#' title='Click here to get UnConnected habitation details'  onClick=GetUnConnectedHabsDetails('" + URLEncrypt.EncryptParameters1(new string[]{"BlockCode="+blockCode.ToString().Trim()}) +"'); return false;'>"+ UnconnectedHabitation.FirstOrDefault().ToString()+"</a>",
                   "<a href='#' title='Click here to get UnConnected habitation details'  onClick=GetNotConnectHabsDetails('" + URLEncrypt.EncryptParameters1(new string[]{"BlockCode="+blockCode.ToString().Trim()}) +"'); return false;'>"+NotConnectedHabitation.FirstOrDefault().ToString()+"</a>",
                    "<a href='#' title='Click here to get not feasible habitation details'  onClick=GetNotFeasibleHabsDetails('" + URLEncrypt.EncryptParameters1(new string[]{"BlockCode="+blockCode.ToString().Trim()}) +"'); return false;'>"+FeasibleHabs.FirstOrDefault().ToString()+"</a>",
                    "<a href='#' title='Click here to get state connected habitation details'  onClick=GetStateConnectedHabsDetails('" + URLEncrypt.EncryptParameters1(new string[]{"BlockCode="+blockCode.ToString().Trim()}) +"'); return false;'>"+StateConnectedHabs.FirstOrDefault().ToString()+"</a>",
                    "<a href='#' title='Click here to get benifited habitation details'  onClick=GetBenifitedHabsDetails('" + URLEncrypt.EncryptParameters1(new string[]{"BlockCode="+blockCode.ToString().Trim()}) +"'); return false;'>"+BenifitedHabs.FirstOrDefault().ToString()+"</a>",
                    "<a href='#' title='Click here to get Change Status habitation details'  onClick=GetBalancedHabsDetails('" + URLEncrypt.EncryptParameters1(new string[]{"BlockCode="+blockCode.ToString().Trim()}) +"'); return false;'>"+balanceHabs.ToString()+"</a>",
          
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
                if (dbContext != null)
                {
                    dbContext.Dispose();
                }
            }
        }

        public Array GetTotalHabsDetails(int blockCode, int page, int rows, string sidx, string sord, out long totalRecords)
        {
            PMGSYEntities dbContext = new PMGSYEntities();
            try
            {
                int PMGSY = PMGSYSession.Current.PMGSYScheme;
                var list = dbContext.get_Habitation_Details_Info(blockCode, "TH", PMGSY).ToList();


                totalRecords = list.Count();

                if (sidx.Trim() != string.Empty)
                {
                    if (sord.Trim() == "asc")
                    {
                        switch (sidx)
                        {
                            case "Block":
                                list = list.OrderBy(m => m.MAST_HAB_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                                break;
                            case "Village":
                                list = list.OrderBy(m => m.MAST_VILLAGE_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                                break;
                            case "TotalPopulation":
                                list = list.OrderBy(m => m.MAST_HAB_TOT_POP).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                                break;
                            case "STPopulation":
                                list = list.OrderBy(m => m.MAST_HAB_SCST_POP).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                                break;

                        }
                    }
                    else
                    {
                        switch (sidx)
                        {
                            case "Block":
                                list = list.OrderByDescending(m => m.MAST_HAB_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                                break;
                            case "Village":
                                list = list.OrderByDescending(m => m.MAST_VILLAGE_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                                break;
                            case "TotalPopulation":
                                list = list.OrderByDescending(m => m.MAST_HAB_TOT_POP).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                                break;
                            case "STPopulation":
                                list = list.OrderByDescending(m => m.MAST_HAB_SCST_POP).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                                break;

                        }
                    }


                }

                return list.Select(total => new
                {
                    cell = new[]
                    {
                        total.MAST_HAB_NAME.ToString(),
                        total.MAST_VILLAGE_NAME.ToString(),
                        total.MAST_HAB_TOT_POP.ToString(),
                        total.MAST_HAB_SCST_POP.ToString(),
                      //  total.MAST_HAB_STATUS.ToString()=="U"?"Unconnected":(total.MAST_HAB_STATUS=="S"?"State Connected":(total.MAST_HAB_STATUS=="F"?"Not Feasible":""))
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
                if (dbContext != null)
                {
                    dbContext.Dispose();
                }
            }
        }

        public Array GetConnectedHabsDetails(int blockCode, int page, int rows, string sidx, string sord, out long totalRecords)
        {
            PMGSYEntities dbContext = new PMGSYEntities();
            try
            {
                int PMGSY = PMGSYSession.Current.PMGSYScheme;
                var list = dbContext.get_Habitation_Details_Info(blockCode, "HC", PMGSY).ToList();
                totalRecords = list.Count();

                if (sidx.Trim() != string.Empty)
                {
                    if (sord.Trim() == "asc")
                    {
                        switch (sidx)
                        {
                            case "Block":
                                list = list.OrderBy(m => m.MAST_HAB_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                                break;
                            case "Village":
                                list = list.OrderBy(m => m.MAST_VILLAGE_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                                break;
                            case "TotalPopulation":
                                list = list.OrderBy(m => m.MAST_HAB_TOT_POP).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                                break;
                            case "STPopulation":
                                list = list.OrderBy(m => m.MAST_HAB_SCST_POP).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                                break;

                        }
                    }
                    else
                    {
                        switch (sidx)
                        {
                            case "Block":
                                list = list.OrderByDescending(m => m.MAST_HAB_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                                break;
                            case "Village":
                                list = list.OrderByDescending(m => m.MAST_VILLAGE_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                                break;
                            case "TotalPopulation":
                                list = list.OrderByDescending(m => m.MAST_HAB_TOT_POP).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                                break;
                            case "STPopulation":
                                list = list.OrderByDescending(m => m.MAST_HAB_SCST_POP).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                                break;

                        }
                    }
                }

                return list.Select(total => new
                {
                    cell = new[]
                    {
                        total.MAST_HAB_NAME.ToString(),
                        total.MAST_VILLAGE_NAME.ToString(),
                        total.MAST_HAB_TOT_POP.ToString(),
                        total.MAST_HAB_SCST_POP.ToString(),
                    //   total.MAST_HAB_STATUS.ToString()=="U"?"Unconnected":(total.MAST_HAB_STATUS=="S"?"State Connected":(total.MAST_HAB_STATUS=="F"?"Not Feasible":""))
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
                if (dbContext != null)
                {
                    dbContext.Dispose();
                }
            }
        }

        public Array GetNotConnectedHabsDetails(int blockCode, int page, int rows, string sidx, string sord, out long totalRecords)
        {
            PMGSYEntities dbContext = new PMGSYEntities();
            try
            {
                int PMGSY = PMGSYSession.Current.PMGSYScheme;
                var list = dbContext.get_Habitation_Details_Info(blockCode, "NC", PMGSY).ToList();
                totalRecords = list.Count();


                if (sidx.Trim() != string.Empty)
                {
                    if (sord.Trim() == "asc")
                    {
                        switch (sidx)
                        {
                            case "Block":
                                list = list.OrderBy(m => m.MAST_HAB_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                                break;
                            case "Village":
                                list = list.OrderBy(m => m.MAST_VILLAGE_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                                break;
                            case "TotalPopulation":
                                list = list.OrderBy(m => m.MAST_HAB_TOT_POP).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                                break;
                            case "STPopulation":
                                list = list.OrderBy(m => m.MAST_HAB_SCST_POP).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                                break;

                        }
                    }
                    else
                    {
                        switch (sidx)
                        {
                            case "Block":
                                list = list.OrderByDescending(m => m.MAST_HAB_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                                break;
                            case "Village":
                                list = list.OrderByDescending(m => m.MAST_VILLAGE_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                                break;
                            case "TotalPopulation":
                                list = list.OrderByDescending(m => m.MAST_HAB_TOT_POP).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                                break;
                            case "STPopulation":
                                list = list.OrderByDescending(m => m.MAST_HAB_SCST_POP).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                                break;

                        }
                    }
                }

                return list.Select(total => new
                {
                    cell = new[]
                    {
                        total.MAST_HAB_NAME.ToString(),
                        total.MAST_VILLAGE_NAME.ToString(),
                        total.MAST_HAB_TOT_POP.ToString(),
                        total.MAST_HAB_SCST_POP.ToString(),
                     //   total.MAST_HAB_STATUS.ToString()=="U"?"Unconnected":(total.MAST_HAB_STATUS=="S"?"State Connected":(total.MAST_HAB_STATUS=="F"?"Not Feasible":""))
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
                if (dbContext != null)
                {
                    dbContext.Dispose();
                }
            }
        }

        public Array GetNotFeasibleHabsDetails(int blockCode, int page, int rows, string sidx, string sord, out long totalRecords)
        {
            PMGSYEntities dbContext = new PMGSYEntities();
            try
            {
                int PMGSY = PMGSYSession.Current.PMGSYScheme;
                var list = dbContext.get_Habitation_Details_Info(blockCode, "IF", PMGSY).ToList();
                totalRecords = list.Count();


                if (sidx.Trim() != string.Empty)
                {
                    if (sord.Trim() == "asc")
                    {
                        switch (sidx)
                        {
                            case "Block":
                                list = list.OrderBy(m => m.MAST_HAB_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                                break;
                            case "Village":
                                list = list.OrderBy(m => m.MAST_VILLAGE_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                                break;
                            case "TotalPopulation":
                                list = list.OrderBy(m => m.MAST_HAB_TOT_POP).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                                break;
                            case "STPopulation":
                                list = list.OrderBy(m => m.MAST_HAB_SCST_POP).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                                break;

                        }
                    }
                    else
                    {
                        switch (sidx)
                        {
                            case "Block":
                                list = list.OrderByDescending(m => m.MAST_HAB_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                                break;
                            case "Village":
                                list = list.OrderByDescending(m => m.MAST_VILLAGE_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                                break;
                            case "TotalPopulation":
                                list = list.OrderByDescending(m => m.MAST_HAB_TOT_POP).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                                break;
                            case "STPopulation":
                                list = list.OrderByDescending(m => m.MAST_HAB_SCST_POP).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                                break;

                        }
                    }
                }
                return list.Select(total => new
                {
                    cell = new[]
                    {
                        total.MAST_HAB_NAME.ToString(),
                        total.MAST_VILLAGE_NAME.ToString(),
                        total.MAST_HAB_TOT_POP.ToString(),
                        total.MAST_HAB_SCST_POP.ToString(),
                     //  total.MAST_HAB_STATUS.ToString()=="U"?"Unconnected":(total.MAST_HAB_STATUS=="S"?"State Connected":(total.MAST_HAB_STATUS=="F"?"Not Feasible":""))
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
                if (dbContext != null)
                {
                    dbContext.Dispose();
                }
            }
        }

        public Array GetStateConnectedHabsDetails(int blockCode, int page, int rows, string sidx, string sord, out long totalRecords)
        {
            PMGSYEntities dbContext = new PMGSYEntities();
            try
            {
                int PMGSY = PMGSYSession.Current.PMGSYScheme;
                var list = dbContext.get_Habitation_Details_Info(blockCode, "SC", PMGSY).ToList();
                totalRecords = list.Count();


                if (sidx.Trim() != string.Empty)
                {
                    if (sord.Trim() == "asc")
                    {
                        switch (sidx)
                        {
                            case "Block":
                                list = list.OrderBy(m => m.MAST_HAB_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                                break;
                            case "Village":
                                list = list.OrderBy(m => m.MAST_VILLAGE_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                                break;
                            case "TotalPopulation":
                                list = list.OrderBy(m => m.MAST_HAB_TOT_POP).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                                break;
                            case "STPopulation":
                                list = list.OrderBy(m => m.MAST_HAB_SCST_POP).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                                break;

                        }
                    }
                    else
                    {
                        switch (sidx)
                        {
                            case "Block":
                                list = list.OrderByDescending(m => m.MAST_HAB_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                                break;
                            case "Village":
                                list = list.OrderByDescending(m => m.MAST_VILLAGE_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                                break;
                            case "TotalPopulation":
                                list = list.OrderByDescending(m => m.MAST_HAB_TOT_POP).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                                break;
                            case "STPopulation":
                                list = list.OrderByDescending(m => m.MAST_HAB_SCST_POP).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                                break;

                        }
                    }
                }

                return list.Select(total => new
                {
                    cell = new[]
                    {
                        total.MAST_HAB_NAME.ToString(),
                        total.MAST_VILLAGE_NAME.ToString(),
                        total.MAST_HAB_TOT_POP.ToString(),
                        total.MAST_HAB_SCST_POP.ToString(),
                     //   total.MAST_HAB_STATUS.ToString()=="U"?"Unconnected":(total.MAST_HAB_STATUS=="S"?"State Connected":(total.MAST_HAB_STATUS=="F"?"Not Feasible":""))
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
                if (dbContext != null)
                {
                    dbContext.Dispose();
                }
            }
        }

        public Array GetBenifitedHabsDetails(int blockCode, int page, int rows, string sidx, string sord, out long totalRecords)
        {
            PMGSYEntities dbContext = new PMGSYEntities();

            try
            {
                int PMGSY = PMGSYSession.Current.PMGSYScheme;
                var list = dbContext.get_Habitation_Details_Info(blockCode, "BH", PMGSY).ToList();
                totalRecords = list.Count();


                if (sidx.Trim() != string.Empty)
                {
                    if (sord.Trim() == "asc")
                    {
                        switch (sidx)
                        {
                            case "Block":
                                list = list.OrderBy(m => m.MAST_HAB_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                                break;
                            case "Village":
                                list = list.OrderBy(m => m.MAST_VILLAGE_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                                break;
                            case "TotalPopulation":
                                list = list.OrderBy(m => m.MAST_HAB_TOT_POP).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                                break;
                            case "STPopulation":
                                list = list.OrderBy(m => m.MAST_HAB_SCST_POP).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                                break;

                        }
                    }
                    else
                    {
                        switch (sidx)
                        {
                            case "Block":
                                list = list.OrderByDescending(m => m.MAST_HAB_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                                break;
                            case "Village":
                                list = list.OrderByDescending(m => m.MAST_VILLAGE_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                                break;
                            case "TotalPopulation":
                                list = list.OrderByDescending(m => m.MAST_HAB_TOT_POP).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                                break;
                            case "STPopulation":
                                list = list.OrderByDescending(m => m.MAST_HAB_SCST_POP).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                                break;

                        }
                    }
                }

                return list.Select(total => new
                {
                    cell = new[]
                    {
                        total.MAST_HAB_NAME.ToString(),
                        total.MAST_VILLAGE_NAME.ToString(),
                        total.MAST_HAB_TOT_POP.ToString(),
                        total.MAST_HAB_SCST_POP.ToString(),
                      //  total.MAST_HAB_STATUS.ToString()=="U"?"Unconnected":(total.MAST_HAB_STATUS=="S"?"State Connected":(total.MAST_HAB_STATUS=="F"?"Not Feasible":""))
                      
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
                if (dbContext != null)
                {
                    dbContext.Dispose();
                }
            }
        }

        public Array GetHabsDetailsByStatusDAL(string flagParam, int blockCode, int page, int rows, string sidx, string sord, out long totalRecords)
        {
            PMGSYEntities dbContext = new PMGSYEntities();
            try
            {
                int PMGSY = PMGSYSession.Current.PMGSYScheme;
                var list = dbContext.get_Habitation_Details_Info(blockCode, flagParam, PMGSY).ToList();
                totalRecords = list.Count();

                if (sidx.Trim() != string.Empty)
                {
                    if (sord.Trim() == "asc")
                    {
                        switch (sidx)
                        {
                            case "Block":
                                list = list.OrderBy(m => m.MAST_HAB_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                                break;
                            case "Village":
                                list = list.OrderBy(m => m.MAST_VILLAGE_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                                break;
                            case "TotalPopulation":
                                list = list.OrderBy(m => m.MAST_HAB_TOT_POP).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                                break;
                            case "STPopulation":
                                list = list.OrderBy(m => m.MAST_HAB_SCST_POP).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                                break;

                        }
                    }
                    else
                    {
                        switch (sidx)
                        {
                            case "Block":
                                list = list.OrderByDescending(m => m.MAST_HAB_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                                break;
                            case "Village":
                                list = list.OrderByDescending(m => m.MAST_VILLAGE_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                                break;
                            case "TotalPopulation":
                                list = list.OrderByDescending(m => m.MAST_HAB_TOT_POP).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                                break;
                            case "STPopulation":
                                list = list.OrderByDescending(m => m.MAST_HAB_SCST_POP).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                                break;

                        }
                    }
                }

                return list.Select(total => new
                {
                    cell = new[]
                    {
                        total.MAST_HAB_NAME.ToString(),
                        total.MAST_VILLAGE_NAME.ToString(),
                        total.MAST_HAB_TOT_POP.ToString(),
                        total.MAST_HAB_SCST_POP.ToString(),
                    //   total.MAST_HAB_STATUS.ToString()=="U"?"Unconnected":(total.MAST_HAB_STATUS=="S"?"State Connected":(total.MAST_HAB_STATUS=="F"?"Not Feasible":""))
                        "<a href='#' title='Click here to Remove State Connected' class='ui-icon ui-icon-pencil ui-align-center' onClick=EditStatus('" + URLEncrypt.EncryptParameters1(new string[]{"HabCode=" +  total.MAST_HAB_CODE.ToString().Trim()}) +"'); return false;>Edit</a>",
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
                if (dbContext != null)
                {
                    dbContext.Dispose();
                }
            }
        }

    }
    public interface IHabitationConnectivityDAL
    {
        bool EditHabStatus(int habCode, ref string message);

        Array GetHabitationDetails(int districtCode, int blockCode, int page, int rows, string sidx, string sord, out long totalRecords);
        //List<SelectListItem> GetAllStates(); 
        List<SelectListItem> GetAllDistricts();
        List<SelectListItem> GetAllBlocks(int districtCode);
        bool ChangeStatusOfHabitation(string encryptedHabCode, string status);
        bool ChangeStatusAsPerCensusYearOfHabitation(string encryptedHabCode, string status);
        Array GetAllDetails_ByBlockCode(int blockCode, int page, int rows, string sidx, string sord, out long totalRecords);
        Array GetTotalHabsDetails(int blockCode, int page, int rows, string sidx, string sord, out long totalRecords);
        Array GetConnectedHabsDetails(int blockCode, int page, int rows, string sidx, string sord, out long totalRecords);
        Array GetNotConnectedHabsDetails(int blockCode, int page, int rows, string sidx, string sord, out long totalRecords);
        Array GetNotFeasibleHabsDetails(int blockCode, int page, int rows, string sidx, string sord, out long totalRecords);
        Array GetStateConnectedHabsDetails(int blockCode, int page, int rows, string sidx, string sord, out long totalRecords);
        Array GetBenifitedHabsDetails(int blockCode, int page, int rows, string sidx, string sord, out long totalRecords);
        Array GetHabsDetailsByStatusDAL(string flagParam, int blockCode, int page, int rows, string sidx, string sord, out long totalRecords);
    }
}