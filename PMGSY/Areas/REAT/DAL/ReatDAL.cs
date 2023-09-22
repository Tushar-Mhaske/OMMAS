using PMGSY.Areas.REAT.Models;
using PMGSY.Common;
using PMGSY.Extensions;
using PMGSY.Models;
using PMGSY.Models.Payment;
using PMGSY.Models.PFMS;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Core;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Validation;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Transactions;
using System.Web;
using System.Web.Mvc;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace PMGSY.Areas.REAT.DAL
{
    public class ReatDAL
    {
        PMGSYEntities dbContext = null;
        #region REAT Vendor Registration

        public Array GetBeneficiaryDetailsDAL(int stateCode, int districtCode, int agencyCode, int? page, int? rows, string sidx, string sord, out Int32 totalRecords)
        {
            string date = string.Empty;
            try
            {
                dbContext = new PMGSYEntities();
                var contractorDetails = (from con in dbContext.MASTER_CONTRACTOR
                                         join reg in dbContext.MASTER_CONTRACTOR_BANK on con.MAST_CON_ID equals reg.MAST_CON_ID
                                         join CON_REG in dbContext.MASTER_CONTRACTOR_REGISTRATION on con.MAST_CON_ID equals CON_REG.MAST_CON_ID
                                         join MS in dbContext.MASTER_STATE on CON_REG.MAST_REG_STATE equals MS.MAST_STATE_CODE
                                         join MD in dbContext.MASTER_DISTRICT on reg.MAST_DISTRICT_CODE equals MD.MAST_DISTRICT_CODE
                                         join LST in dbContext.OMMAS_LDG_STATE_MAPPING on MS.MAST_STATE_CODE equals LST.MAST_STATE_CODE
                                         join LDT in dbContext.OMMAS_LDG_DISTRICT_MAPPING on MD.MAST_DISTRICT_CODE equals LDT.MAST_DISTRICT_CODE
                                         where
                                         CON_REG.MAST_REG_STATUS == "A"
                                         && reg.MAST_ACCOUNT_STATUS == "A"
                                         && (reg.MAST_IFSC_CODE).Length > 10
                                         && !string.IsNullOrEmpty(reg.MAST_IFSC_CODE)
                                         && !(dbContext.REAT_CONTRACTOR_DETAILS.Any(x => x.MAST_CON_ID == con.MAST_CON_ID && x.MAST_ACCOUNT_ID == reg.MAST_ACCOUNT_ID && x.REAT_CON_ID != null && x.ommas_STATUS == "A" && x.MAST_LGD_STATE_CODE == LST.MAST_STATE_LDG_CODE_REAT && (x.MAST_AGENCY_CODE == agencyCode || x.MAST_AGENCY_CODE == null)))
                                         && MS.MAST_STATE_CODE == stateCode//PMGSYSession.Current.StateCode
                                         && MD.MAST_DISTRICT_CODE == (districtCode == 0 ? MD.MAST_DISTRICT_CODE : districtCode)
                                         && reg.MAST_LOCK_STATUS == "Y"
                                         select new
                                         {
                                             con.MAST_CON_ID,
                                             con.MAST_CON_PAN,
                                             reg.MAST_ACCOUNT_ID,
                                             reg.MASTER_DISTRICT.MASTER_STATE.OMMAS_LDG_STATE_MAPPING.MAST_STATE_LDG_CODE_REAT,
                                             reg.MASTER_DISTRICT.OMMAS_LDG_DISTRICT_MAPPING.MAST_DISTRICT_CODE,
                                             con.MAST_CON_COMPANY_NAME,
                                             reg.MAST_IFSC_CODE,
                                             reg.MAST_ACCOUNT_NUMBER,
                                             con.MAST_CON_FNAME,
                                             con.MAST_CON_MNAME,
                                             con.MAST_CON_LNAME,
                                             BANK_NAME = reg.MAST_BANK_NAME,//.Trim().Replace("\n", "").Replace("\r", ""),
                                             BATCH_ID = dbContext.REAT_CONTRACTOR_DETAILS.Where(z => z.MAST_CON_ID == con.MAST_CON_ID && z.MAST_ACCOUNT_ID == reg.MAST_ACCOUNT_ID && z.MAST_LGD_STATE_CODE == LST.MAST_STATE_LDG_CODE_REAT && z.MAST_AGENCY_CODE == agencyCode).OrderByDescending(a => a.DETAIL_ID).Select(v => v.BATCH_ID).FirstOrDefault(),
                                             status = dbContext.REAT_CONTRACTOR_DETAILS.Any(z => z.MAST_CON_ID == con.MAST_CON_ID && z.MAST_ACCOUNT_ID == reg.MAST_ACCOUNT_ID && z.MAST_LGD_STATE_CODE == LST.MAST_STATE_LDG_CODE_REAT && (z.MAST_AGENCY_CODE == agencyCode || z.MAST_AGENCY_CODE == null) && z.ommas_STATUS == "A" && z.REAT_CON_ID != null) ? "Accepted"
                                                    : dbContext.REAT_CONTRACTOR_DETAILS.Any(z => z.MAST_CON_ID == con.MAST_CON_ID && z.MAST_ACCOUNT_ID == reg.MAST_ACCOUNT_ID && z.MAST_LGD_STATE_CODE == LST.MAST_STATE_LDG_CODE_REAT && (z.MAST_AGENCY_CODE == agencyCode || z.MAST_AGENCY_CODE == null) && z.ommas_STATUS == "R" && z.REAT_CON_ID == null) ? "Rejected"
                                                    : "Processing at REAT",
                                         }).Distinct().ToList();
                totalRecords = contractorDetails.Count();

                if (sidx.Trim() != string.Empty)
                {
                    if (sord.ToString() == "asc")
                    {
                        switch (sidx)
                        {
                            case "ContractorName":
                                contractorDetails = contractorDetails.OrderBy(x => x.MAST_CON_ID).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                            default:
                                break;
                        }
                    }
                    else
                    {
                        switch (sidx)
                        {
                            case "ContractorName":
                                contractorDetails = contractorDetails.OrderByDescending(x => x.MAST_CON_ID).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                            default:
                                break;
                        }
                    }
                }
                else
                {
                    contractorDetails = contractorDetails.OrderByDescending(x => x.MAST_CON_ID).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                }

                var result = contractorDetails.Select(conDetails => new
                {
                    conDetails.MAST_CON_FNAME,
                    conDetails.MAST_CON_MNAME,
                    conDetails.MAST_CON_LNAME,
                    conDetails.MAST_CON_PAN,
                    conDetails.MAST_CON_ID,
                    conDetails.MAST_CON_COMPANY_NAME,
                    conDetails.BANK_NAME,
                    conDetails.MAST_ACCOUNT_ID,
                    conDetails.MAST_IFSC_CODE,
                    conDetails.MAST_ACCOUNT_NUMBER,
                    conDetails.status,
                    //Below Condition is modified on 13-06-2022 
                    //flag = checkDate(conDetails.BATCH_ID, out date),
                    flag = conDetails.status== "Rejected" ? true: checkDate(conDetails.BATCH_ID, out date),
                    date
                }).ToArray();

                return result.Select(lstcontractorDetails => new
                {
                    id = lstcontractorDetails.MAST_CON_ID.ToString(),
                    cell = new[] {      
                                    lstcontractorDetails.flag == true ? "<input id='cbx_'"+ lstcontractorDetails.MAST_CON_ID +" class='cbxCon' type='checkbox' title='Proposal is dropped.' name='cbContractor' value='" + lstcontractorDetails.MAST_CON_ID.ToString() + "$" + lstcontractorDetails.MAST_ACCOUNT_ID.ToString() + "$" + lstcontractorDetails.flag
                                    +"'>"
                                    : "<input id='cbx_'"+ lstcontractorDetails.MAST_CON_ID +" class='cbxCon' type='checkbox' title='Proposal is dropped.' name='cbContractor' disabled=disabled value='" + lstcontractorDetails.MAST_CON_ID.ToString()  + "$" + lstcontractorDetails.MAST_ACCOUNT_ID.ToString() + "'>",
                                    Convert.ToString(lstcontractorDetails.MAST_CON_FNAME).Trim() + " " + Convert.ToString(lstcontractorDetails.MAST_CON_MNAME) + " " + Convert.ToString(lstcontractorDetails.MAST_CON_LNAME) + " (" + lstcontractorDetails.MAST_CON_ID.ToString() + ")",
                                    string.IsNullOrEmpty(lstcontractorDetails.MAST_CON_PAN) ? "-" : lstcontractorDetails.MAST_CON_PAN,
                                    lstcontractorDetails.MAST_CON_COMPANY_NAME,
                                    lstcontractorDetails.BANK_NAME.Trim().Replace("\n", "").Replace("\r", ""),
                                    lstcontractorDetails.MAST_ACCOUNT_ID.ToString(),
                                    lstcontractorDetails.MAST_IFSC_CODE,
                                    lstcontractorDetails.MAST_ACCOUNT_NUMBER.ToString(),

                                    lstcontractorDetails.status.Trim(),
                                    lstcontractorDetails.date
                    }
                }).ToArray();
            }
            catch (Exception ex)
            {
                totalRecords = 0;
                ErrorLog.LogError(ex, "REATDAL.GetBeneficiaryDetailsDAL()");
                return null;
            }
        }

        public bool checkDate(string genDate, out string date)
        {
            date = "";
            if (string.IsNullOrEmpty(genDate))
            {
                return true;
            }
            PMGSYEntities dbContext1 = new PMGSYEntities();
            DateTime currDate = DateTime.Now;
            string[] strArr = new string[3];
            try
            {
                strArr[0] = genDate.Substring(6, 2);
                strArr[1] = genDate.Substring(8, 2);
                strArr[2] = "20" + genDate.Substring(10, 2);

                date = strArr[0] + "/" + strArr[1] + "/" + strArr[2];

                DateTime dt = new DateTime(Convert.ToInt32(strArr[2]), Convert.ToInt32(strArr[1]), Convert.ToInt32(strArr[0]));

                //Below Condition is modified on 13-06-2022 
                // if ((currDate - dt).TotalDays <= 5)
                if ((currDate - dt).TotalDays <= 2)
                {
                    return false;
                }
                else
                {
                    return true;
                }

                return false;
            }
            catch (Exception ex)
            {
                date = "";
                ErrorLog.LogError(ex, "REATDAL.checkDate()");
                return false;
            }
            finally
            {
                dbContext1.Dispose();
            }
        }

        public string GetStateShortName(int StateCode)
        {
            dbContext = new PMGSYEntities();
            try
            {
                return dbContext.MASTER_STATE.FirstOrDefault(s => s.MAST_STATE_CODE == StateCode).MAST_STATE_SHORT_CODE;
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "REATDAL.GetStateShortName");
                return String.Empty;
            }
        }

        //public bool GenerateXMLDAL(REATDownloadXMLViewModel model, out string xmlFName, out int recCount)
        public bool GenerateXMLDAL(REATDownloadXMLViewModel model, ref string message)
        {
            bool flg = false;
            string xmlFName = string.Empty;
            int recCount = 0;
            string xmlString = string.Empty;
            string xmlHeader = string.Empty;
            string xmlBody = string.Empty;
            string xmlFileName = string.Empty;
            int recordCount = 0;

            var outParam = new System.Data.Entity.Core.Objects.ObjectParameter("XmlFileName", xmlFileName);
            var outPrmCount = new System.Data.Entity.Core.Objects.ObjectParameter("RecordCount", recordCount);
            #region SqlParameter (for reference)
            #endregion
            try
            {
                dbContext = new PMGSYEntities();

                #region Call SP (for reference)
                #endregion

                DataTable ContractorIds = new DataTable();
                ContractorIds.Columns.Add("MAST_CON_ID", typeof(int));
                ContractorIds.Columns.Add("MAST_ACCOUNT_ID", typeof(int));
                if (model.mastContractorIds != null)
                {
                    foreach (string conId in model.mastContractorIds)
                    {
                        if (conId.Split('$').Length > 2)
                        {
                            if (conId.Split('$')[2].ToUpper() == "TRUE")
                            {
                                ContractorIds.Rows.Add(new object[] { Convert.ToInt32(conId.Split('$')[0]), Convert.ToInt32(conId.Split('$')[1]) });
                            }
                        }
                    }
                }


                Object[] parameters = { model.stateCode, model.agencyCode, model.districtCode, outParam, outPrmCount, ContractorIds };

                Object[] bparameters = { model.stateCode, model.agencyCode, model.districtCode, ContractorIds };
                //var bdy = dbContext.Database.SqlQuery<string>("exec omms.PFMS_Generate_Conctractor_XML", bparameters).ToList();

                var levelParam = new SqlParameter("@Level", SqlDbType.Int);
                levelParam.Value = model.Level;

                var stateParam = new SqlParameter("@stateCode", SqlDbType.Int);
                stateParam.Value = model.stateCode;
                //stateParam.TypeName = "int";

                var agencyParam = new SqlParameter("@agencyCode", SqlDbType.Int);
                agencyParam.Value = model.agencyCode;
                //agencyParam.TypeName = "int";

                var distParam = new SqlParameter("@DistrictC", SqlDbType.Int);
                distParam.Value = model.districtCode;
                //distParam.TypeName = "int";

                //var xmlParam = new SqlParameter {
                //                   ParameterName = "@XmlFileName",
                //                   Value = 0,
                //                   Direction = ParameterDirection.Output };

                //var xmlParam = new SqlParameter("@XmlFileName", SqlDbType.VarChar);
                //xmlParam.Value = "";
                ////xmlParam.TypeName = "varchar";
                //xmlParam.Direction = ParameterDirection.Output;

                var xmlParam = new SqlParameter
                {
                    ParameterName = "XmlFileName",
                    DbType = System.Data.DbType.String,
                    Size = 30,
                    Direction = System.Data.ParameterDirection.Output
                };

                var recParam = new SqlParameter("@RecordCount", SqlDbType.Int);
                recParam.Value = 0;
                //recParam.TypeName = "int";
                recParam.Direction = ParameterDirection.Output;

                var conParam = new SqlParameter("@Contractors", SqlDbType.Structured);
                conParam.Value = ContractorIds;
                conParam.TypeName = "omms.ContractorList";

                var levelParam1 = new SqlParameter("@Level", SqlDbType.Int);
                levelParam1.Value = model.Level;

                var stateParam1 = new SqlParameter("@stateCode", SqlDbType.Int);
                stateParam1.Value = model.stateCode;
                //stateParam.TypeName = "int";

                var agencyParam1 = new SqlParameter("@agencyCode", SqlDbType.Int);
                agencyParam1.Value = model.agencyCode;
                //agencyParam.TypeName = "int";

                var distParam1 = new SqlParameter("@DistrictC", SqlDbType.Int);
                distParam1.Value = model.districtCode;

                var conParam1 = new SqlParameter("@Contractors", SqlDbType.Structured);
                conParam1.Value = ContractorIds;
                conParam1.TypeName = "omms.ContractorList";

                //using (DbContextTransaction transaction = dbContext.Database.BeginTransaction())
                using (TransactionScope ts = new TransactionScope())
                {
                    try
                    {
                        //var results = context.Database.SqlQuery<Person>("GetPersonAndVoteCount @id, @voteCount out", idParam,votesParam);
                        var hdr = dbContext.Database.SqlQuery<string>("exec omms.REAT_Generate_Conctractor_Header_XML @Level,@stateCode,@agencyCode,@DistrictC,@XmlFileName out,@RecordCount out,@Contractors", levelParam, stateParam, agencyParam, distParam, xmlParam, recParam, conParam).ToList();
                        var bdy = dbContext.Database.SqlQuery<string>("exec omms.REAT_Generate_Conctractor_XML @Level,@stateCode,@agencyCode,@DistrictC,@Contractors", levelParam1, stateParam1, agencyParam1, distParam1, conParam1).ToList();

                        xmlFName = Convert.ToString(xmlParam.Value);
                        recCount = Convert.ToInt32(recParam.Value);

                        if (recCount == 0)
                        {
                            message = "No Records to generate XML";
                            return false;
                        }

                        xmlHeader = string.Join("", hdr);
                        xmlBody = string.Join("", bdy);

                        xmlString = xmlHeader.Trim() + xmlBody.Trim();

                        XmlSerializer XmlS = new XmlSerializer(xmlString.GetType());

                        StringWriter sw = new PMGSY.DAL.PFMS.PFMSDAL1.StringWriterUtf8();
                        XmlTextWriter tw = new XmlTextWriter(sw);
                        tw.WriteProcessingInstruction("xml", "version=\"1.0\" encoding=\"UTF-8\" standalone=\"yes\"");

                        XmlS.Serialize(tw, xmlString);

                        xmlString = sw.ToString().Replace("&lt;", "<").Replace("&gt;", ">").Replace("<VendorRegRequest>", "").Replace("</VendorRegRequest>", "").Replace("<string>", @"<VendorRegRequest xmlns=""http://cpsms.nic.in/VendorRegistrationRequest""><CstmrDtls>").Replace("</string>", "</CstmrDtls></VendorRegRequest>");

                        dbContext = new PMGSYEntities();

                        string stateShortCode = GetStateShortName(model.stateCode);
                        string[] paths = { @"" + ConfigurationManager.AppSettings["REATBeneficiaryRequest"], stateShortCode.Trim(), "\\" + xmlFName.Trim(), ".xml" };

                        REAT_DATA_SEND_DETAILS Reat_data_send_details = new REAT_DATA_SEND_DETAILS();
                        Reat_data_send_details.FILE_ID = !(dbContext.REAT_DATA_SEND_DETAILS.Any()) ? 1 : (dbContext.REAT_DATA_SEND_DETAILS.Max(x => x.FILE_ID) + 1);
                        Reat_data_send_details.ADMIN_ND_CODE = dbContext.ADMIN_DEPARTMENT.Where(x => x.MAST_STATE_CODE == model.stateCode && x.MAST_AGENCY_CODE == model.agencyCode && x.MAST_ND_TYPE == "S").Select(x => x.ADMIN_ND_CODE).FirstOrDefault();
                        Reat_data_send_details.FUND_TYPE = "P";//null;
                        Reat_data_send_details.FILE_GENERATION_DATE = DateTime.Now;
                        Reat_data_send_details.GENERATED_FILE_PATH = Path.Combine(@"" + ConfigurationManager.AppSettings["REATBeneficiaryRequest"]  + stateShortCode + "\\" + xmlFName.Trim() + ".xml");
                        Reat_data_send_details.FILE_TYPE = "C";
                        Reat_data_send_details.RESPONSE_RECEIVED_DATE = null;
                        Reat_data_send_details.RESPONSE_RECEIVED_DATE = null;
                        Reat_data_send_details.GENERATED_FILE_NAME = xmlFName.Trim();
                        Reat_data_send_details.RECEIVED_FILE_NAME = null;
                        Reat_data_send_details.USER_ID = PMGSYSession.Current.UserId;
                        Reat_data_send_details.IPADDR = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];

                        dbContext.REAT_DATA_SEND_DETAILS.Add(Reat_data_send_details);
                        dbContext.SaveChanges();

                        //string str = string.Format("{0:000}", Reat_data_send_details.GENERATED_FILE_NAME.Substring(21).PadLeft(4, '0')).Trim();
                        //REAT_CONTRACTOR_DETAILS reat_contractor_details = dbContext.REAT_CONTRACTOR_DETAILS.Where(z => z.BATCH_ID == "CB0037" + Reat_data_send_details.GENERATED_FILE_NAME.Substring(13, 4) + Reat_data_send_details.GENERATED_FILE_NAME.Substring(19, 2) + str).FirstOrDefault();

                        string str = "CS0037" + Reat_data_send_details.GENERATED_FILE_NAME.Substring(13, 4) + Reat_data_send_details.GENERATED_FILE_NAME.Substring(19, 2) + string.Format("{0:000}", Reat_data_send_details.GENERATED_FILE_NAME.Substring(21).PadLeft(4, '0')).Trim();
                        if (dbContext.REAT_CONTRACTOR_DETAILS.Any())
                        {
                            //dbContext.REAT_DATA_SEND_DETAILS.FirstOrDefault();
                            //var query = dbContext.REAT_CONTRACTOR_DETAILS.ToList();
                            //REAT_CONTRACTOR_DETAILS reat_contractor_details = dbContext.REAT_CONTRACTOR_DETAILS.Where(z => z.BATCH_ID == str).FirstOrDefault();
                            // REAT_CONTRACTOR_DETAILS reat_contractor_details = dbContext.REAT_CONTRACTOR_DETAILS.FirstOrDefault();

                            //reat_contractor_details.FILE_ID = Reat_data_send_details.FILE_ID;
                            //dbContext.Entry(reat_contractor_details).State = System.Data.Entity.EntityState.Modified;

                            var lst_Reat_Contractor_Details = dbContext.REAT_CONTRACTOR_DETAILS.Where(z => z.BATCH_ID == str).ToList();
                            if (lst_Reat_Contractor_Details != null)
                            {
                                lst_Reat_Contractor_Details.ForEach(m => m.FILE_ID = Reat_data_send_details.FILE_ID);
                                dbContext.SaveChanges();
                            }
                        }
                        
                        byte[] bytes = Encoding.ASCII.GetBytes(xmlString.Trim());

                        if (!string.IsNullOrEmpty(xmlFName.Trim()))
                        {

                            if (!Directory.Exists(Path.Combine(@"" + ConfigurationManager.AppSettings["REATBeneficiaryRequest"] + stateShortCode)))
                                Directory.CreateDirectory(Path.Combine(@"" + ConfigurationManager.AppSettings["REATBeneficiaryRequest"] + stateShortCode));


                            //stateShortCode = objDAL.GetStateShortName(model.stateCode);
                            System.IO.File.WriteAllBytes(Path.Combine(@"" + ConfigurationManager.AppSettings["REATBeneficiaryRequest"] + stateShortCode + "\\" + xmlFName.Trim() + ".xml"), bytes);
                            //return Json(new { success = true, message = "REAT XML generated successfully" }, JsonRequestBehavior.AllowGet);

                            message = "REAT XML generated successfully";
                        }
                        else
                        {
                            message = "Error in PFMS XML generation";
                            return false;
                        }
                    }
                    catch (DbEntityValidationException ex)
                    {
                        message = "Error in PFMS XML generation";
                        ErrorLog.LogError(ex, "REAT.GenerateXMLDAL");
                        return false;
                    }
                    catch (Exception ex)
                    {
                        message = "Error in PFMS XML generation";
                        ErrorLog.LogError(ex, "REAT.GenerateXMLDAL");
                        return false;
                    }
                    ts.Complete();
                }
                return true;
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "REAT.GenerateXMLDAL()");
                xmlFName = string.Empty;
                recCount = -1;
                //return string.Empty;
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

        #endregion

        #region Map Contractor for REAT

        public bool EditREATContractorDetails(List<ContractorMappingREAT> lstModel, string xmlFileName)
        {
            CommonFunctions comm = new CommonFunctions();
            dbContext = new PMGSYEntities();
            int lgdStateCode = 0;
            string responseDate = string.Empty;
            try
            {
                //using (TransactionScope ts = new TransactionScope())
                {
                    REAT_CONTRACTOR_DETAILS reat_contractor_details = null;
                    string genFileName = string.Empty;
                    genFileName = xmlFileName.Trim().Contains('_') ? xmlFileName.Trim().Substring(0, xmlFileName.IndexOf('_')) : xmlFileName.Trim();
                    genFileName = genFileName.Replace('S', 'Q');
                    //if ( !(genFileName.Contains(".xml") || genFileName.Contains(".XML") )) 
                    //{
                    //    genFileName = genFileName + ".xml";
                    //}
                    REAT_DATA_SEND_DETAILS reat_data_send_details = dbContext.REAT_DATA_SEND_DETAILS.Where(x => x.GENERATED_FILE_NAME == genFileName).FirstOrDefault();
                    //if (reat_data_send_details != null)
                    //{
                    //    lgdStateCode = reat_data_send_details.ADMIN_DEPARTMENT.MASTER_STATE.OMMAS_LDG_STATE_MAPPING.MAST_STATE_LDG_CODE;
                    //}
                    foreach (var itm in lstModel)
                    {
                        if (lgdStateCode == 0)
                        {
                            //Get LGD State Code from file
                            lgdStateCode = dbContext.REAT_INITIATING_PARTY_MASTER.Where(q => q.REAT_INIT_PARTY_UNIQUE_CODE == itm.pfmsStateCode).Select(w => w.ADMIN_DEPARTMENT.MASTER_STATE.OMMAS_LDG_STATE_MAPPING.MAST_STATE_LDG_CODE_REAT).FirstOrDefault();
                        }
                        if (lgdStateCode == 0)
                        {
                            //Get LGD State Code from PFMS Data Send Details if unique code is changed
                            lgdStateCode = reat_data_send_details.ADMIN_DEPARTMENT.MASTER_STATE.OMMAS_LDG_STATE_MAPPING.MAST_STATE_LDG_CODE_REAT;
                        }
                        if (itm.acceptStatus == "ACCP" || itm.acceptStatus == "ACPT")
                        {
                            //PFMS_OMMAS_CONTRACTOR_MAPPING pfms_ommas_contractor_mapping = dbContext.PFMS_OMMAS_CONTRACTOR_MAPPING.Where(x => x.MAST_CON_ID == itm.contractorID && x.MAST_LGD_STATE_CODE == itm.lgdStateCode && x.MAST_LGD_DISTRICT_CODE == itm.lgdDistrictCode && x.MAST_ACCOUNT_NUMBER == itm.accountNumber.Trim() && x.MAST_BANK_NAME == itm.bankName.Trim() && x.MAST_IFSC_CODE == itm.branchName.Trim()).FirstOrDefault();
                            reat_contractor_details = dbContext.REAT_CONTRACTOR_DETAILS.Where(x => x.MAST_CON_ID == itm.contractorID && x.MAST_ACCOUNT_NUMBER.Contains(itm.accountNumber.Trim())
                                && x.MAST_BANK_NAME.Contains(itm.bankName.Trim().Replace("\n", "").Replace("\r", ""))
                                && x.REAT_CON_ID == null
                                && x.BATCH_ID == itm.batchId).FirstOrDefault();
                        }
                        else
                        {
                            reat_contractor_details = dbContext.REAT_CONTRACTOR_DETAILS.Where(x => x.MAST_CON_ID == itm.contractorID && x.REAT_CON_ID == null && x.BATCH_ID == itm.batchId).FirstOrDefault();
                        }
                        responseDate = itm.pfmsResponseDate.Substring(0, 10).Split('-')[2] + "/" + itm.pfmsResponseDate.Substring(0, 10).Split('-')[1] + "/" + itm.pfmsResponseDate.Substring(0, 10).Split('-')[0];

                        if (reat_contractor_details != null)
                        {
                            if (itm.cpsmsID != null)
                            {
                                reat_contractor_details.REAT_CON_ID = itm.cpsmsID;
                                reat_contractor_details.ommas_STATUS = (itm.acceptStatus.Trim() == "ACCP" || itm.acceptStatus.Trim() == "ACPT") ? "A" : null;
                            }
                            if (itm.lstRejectCode != null && itm.lstRejectCode.Count > 0)
                            {
                                reat_contractor_details.ommas_STATUS = (itm.acceptStatus.Trim() == "ACCP" || itm.acceptStatus.Trim() == "ACPT") ? "A" : null;
                                reat_contractor_details.REJECTION_CODE = string.Join(",", itm.lstRejectCode);
                                //dbContext.Entry(reat_contractor_details).State = System.Data.Entity.EntityState.Modified;
                            }

                            reat_contractor_details.ACK_FILE_NAME = xmlFileName.Trim().Substring(0, xmlFileName.IndexOf('.'));
                            reat_contractor_details.ACK_RECV_DATE = new CommonFunctions().GetStringToDateTime(responseDate.Trim());//DateTime.Now;
                            reat_contractor_details.reat_CON_NAME = itm.pfmsConName;
                            reat_contractor_details.reat_IFSC_CODE = itm.branchName;
                            reat_contractor_details.reat_STATUS = (itm.acceptStatus.Trim() == "ACCP" || itm.acceptStatus.Trim() == "ACPT") ? "A" : null;

                            dbContext.Entry(reat_contractor_details).State = System.Data.Entity.EntityState.Modified;

                            #region Definalize for Contractor correction after rejection from REAT commented
                            //if (reat_contractor_details.STATUS == "R")
                            //{
                            //    MASTER_CONTRACTOR_BANK bank = dbContext.MASTER_CONTRACTOR_BANK.Where(x => x.MAST_CON_ID == itm.contractorID && x.MAST_ACCOUNT_ID == reat_contractor_details.MAST_ACCOUNT_ID && x.MASTER_DISTRICT.OMMAS_LDG_DISTRICT_MAPPING.MAST_DISTRICT_LDG_CODE == reat_contractor_details.MAST_LGD_DISTRICT_CODE).FirstOrDefault();
                            //    if (bank != null)
                            //    {
                            //        bank.MAST_LOCK_STATUS = "N";
                            //        dbContext.Entry(bank).State = System.Data.Entity.EntityState.Modified;
                            //    }
                            //}
                            #endregion
                        }
                    }

                    if (reat_data_send_details != null)
                    {
                        reat_data_send_details.RESPONSE_RECEIVED_DATE = new CommonFunctions().GetStringToDateTime(responseDate.Trim());
                        reat_data_send_details.RECEIVED_FILE_NAME = xmlFileName.Trim().Substring(0, xmlFileName.IndexOf('.'));
                        reat_data_send_details.RECEIVED_FILE_PATH = null;//To be changed in Windows service
                        dbContext.Entry(reat_data_send_details).State = System.Data.Entity.EntityState.Modified;
                    }
                    if (reat_contractor_details != null || reat_data_send_details != null)
                    {
                        //dbContext.SaveChanges();
                        REATLog("Beneficiary", ConfigurationManager.AppSettings["PFMSBeneficiaryLog"].ToString(), "Beneficiary acknowledgement successful", xmlFileName);
                    }
                    else
                    {
                        REATLog("Beneficiary", ConfigurationManager.AppSettings["PFMSBeneficiaryLog"].ToString(), "No Records updated for Beneficiary acknowledgement", xmlFileName);
                    }
                    //ts.Complete();
                    dbContext.SaveChanges();
                }
                return true;
            }
            catch (DbEntityValidationException e)
            {
                //Elmah.ErrorSignal.FromCurrentContext().Raise(e, HttpContext.Current);
                ErrorLog.LogError(e, "SaveRoadProposalDAL(DbEntityValidationException ex).DAL");
                ModelStateDictionary modelstate = new ModelStateDictionary();

                foreach (var eve in e.EntityValidationErrors)
                {
                    foreach (var ve in eve.ValidationErrors)
                    {
                        modelstate.AddModelError(ve.ErrorMessage, ve.ErrorMessage);
                    }
                }
                ErrorLog.LogError(e, "REATDAL.EditREATContractorDetails()");
                using (System.IO.StreamWriter sw = System.IO.File.AppendText(System.IO.Path.Combine("D:\\ErrorLog\\OMMASErrorLog_" + System.DateTime.Now.Date.ToString("dd_MM_yyyy") + ".txt", "")))
                {
                    sw.WriteLine("Date :" + DateTime.Now.ToString());
                    sw.WriteLine("REATDAL.EditREATContractorDetails() : " + "Application_Error()");

                    sw.WriteLine("Exception : " + new CommonFunctions().FormatErrorMessage(modelstate));
                    sw.WriteLine("---------------------------------------------------------------------------------------");
                    sw.Close();
                }

                //return new CommonFunctions().FormatErrorMessage(modelstate);
                return false;
            }

            catch (DbUpdateException ex)
            {
                //Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                ErrorLog.LogError(ex, "REATDAL.EditREATContractorDetails()");
                return false;
            }
            catch (OptimisticConcurrencyException ex)
            {
                //Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                ErrorLog.LogError(ex, "REATDAL.EditREATContractorDetails()");
                return false;
            }
            catch (Exception ex)
            {
                REATLog("Beneficiary", ConfigurationManager.AppSettings["REATBeneficiaryLog"].ToString(), "Error on Beneficiary acknowledgement", xmlFileName);
                ErrorLog.LogError(ex, "REATDAL.EditREATContractorDetails()");
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
        //public bool EditREATContractorDetails(List<ContractorMappingREAT> lstModel, string xmlFileName)
        //{
        //    CommonFunctions comm = new CommonFunctions();
        //    dbContext = new PMGSYEntities();
        //    int lgdStateCode = 0;
        //    string responseDate = string.Empty;
        //    try
        //    {
        //        //using (TransactionScope ts = new TransactionScope())
        //        {
        //            REAT_CONTRACTOR_DETAILS reat_contractor_details = null;
        //            string genFileName = string.Empty;
        //            genFileName = xmlFileName.Trim().Contains('_') ? xmlFileName.Trim().Substring(0, xmlFileName.IndexOf('_')) : xmlFileName.Trim();
        //            genFileName = genFileName.Replace('S', 'Q');
        //            REAT_DATA_SEND_DETAILS reat_data_send_details = dbContext.REAT_DATA_SEND_DETAILS.Where(x => x.GENERATED_FILE_NAME == genFileName).FirstOrDefault();
        //            //if (reat_data_send_details != null)
        //            //{
        //            //    lgdStateCode = reat_data_send_details.ADMIN_DEPARTMENT.MASTER_STATE.OMMAS_LDG_STATE_MAPPING.MAST_STATE_LDG_CODE;
        //            //}
        //            foreach (var itm in lstModel)
        //            {
        //                if (lgdStateCode == 0)
        //                {
        //                    ///Get LGD State Code from file
        //                    lgdStateCode = dbContext.REAT_INITIATING_PARTY_MASTER.Where(q => q.REAT_INIT_PARTY_UNIQUE_CODE == itm.pfmsStateCode).Select(w => w.ADMIN_DEPARTMENT.MASTER_STATE.OMMAS_LDG_STATE_MAPPING.MAST_STATE_LDG_CODE_REAT).FirstOrDefault();
        //                }

        //                if (lgdStateCode == 0)
        //                {
        //                    ///Get LGD State Code from PFMS Data Send Details if unique code is changed
        //                    lgdStateCode = reat_data_send_details.ADMIN_DEPARTMENT.MASTER_STATE.OMMAS_LDG_STATE_MAPPING.MAST_STATE_LDG_CODE_REAT;
        //                }


        //                //PFMS_OMMAS_CONTRACTOR_MAPPING pfms_ommas_contractor_mapping = dbContext.PFMS_OMMAS_CONTRACTOR_MAPPING.Where(x => x.MAST_CON_ID == itm.contractorID && x.MAST_LGD_STATE_CODE == itm.lgdStateCode && x.MAST_LGD_DISTRICT_CODE == itm.lgdDistrictCode && x.MAST_ACCOUNT_NUMBER == itm.accountNumber.Trim() && x.MAST_BANK_NAME == itm.bankName.Trim() && x.MAST_IFSC_CODE == itm.branchName.Trim()).FirstOrDefault();
        //                reat_contractor_details = dbContext.REAT_CONTRACTOR_DETAILS.Where(x => x.MAST_CON_ID == itm.contractorID && x.MAST_ACCOUNT_NUMBER.Contains(itm.accountNumber.Trim())
        //                    && x.MAST_BANK_NAME.Contains(itm.bankName.Trim().Replace("\n", "").Replace("\r", ""))
        //                    //&& x.MAST_IFSC_CODE == itm.branchName.Trim() 
        //                    && x.MAST_LGD_STATE_CODE == lgdStateCode && x.REAT_CON_ID == null
        //                    && x.BATCH_ID == itm.batchId).FirstOrDefault();
                        
        //                responseDate = itm.pfmsResponseDate.Substring(0, 10).Split('-')[2] + "/" + itm.pfmsResponseDate.Substring(0, 10).Split('-')[1] + "/" + itm.pfmsResponseDate.Substring(0, 10).Split('-')[0];
        //                if (reat_contractor_details != null)
        //                {
        //                    if (itm.cpsmsID != null)
        //                    {
        //                        reat_contractor_details.REAT_CON_ID = itm.cpsmsID;
        //                        reat_contractor_details.ommas_STATUS = (itm.acceptStatus.Trim() == "ACCP" || itm.acceptStatus.Trim() == "ACPT") ? "A" : "R";
        //                    }
        //                    if (itm.lstRejectCode != null && itm.lstRejectCode.Count > 0)
        //                    {
        //                        reat_contractor_details.ommas_STATUS = (itm.acceptStatus.Trim() == "ACCP" || itm.acceptStatus.Trim() == "ACPT") ? "A" : "R";
        //                        reat_contractor_details.REJECTION_CODE = string.Join(",", itm.lstRejectCode);
        //                        //dbContext.Entry(reat_contractor_details).State = System.Data.Entity.EntityState.Modified;
        //                    }

        //                    reat_contractor_details.ACK_FILE_NAME = xmlFileName.Trim().Substring(0, xmlFileName.IndexOf('.'));

        //                    reat_contractor_details.ACK_RECV_DATE = new CommonFunctions().GetStringToDateTime(responseDate.Trim());//DateTime.Now;

        //                    reat_contractor_details.reat_CON_NAME = itm.pfmsConName;
        //                    reat_contractor_details.reat_IFSC_CODE = itm.branchName;
        //                    reat_contractor_details.reat_STATUS = (itm.acceptStatus.Trim() == "ACCP" || itm.acceptStatus.Trim() == "ACPT") ? "A" : "R";

        //                    dbContext.Entry(reat_contractor_details).State = System.Data.Entity.EntityState.Modified;

        //                    #region Definalize for Contractor correction after rejection from REAT commented
        //                    //if (reat_contractor_details.STATUS == "R")
        //                    //{
        //                    //    MASTER_CONTRACTOR_BANK bank = dbContext.MASTER_CONTRACTOR_BANK.Where(x => x.MAST_CON_ID == itm.contractorID && x.MAST_ACCOUNT_ID == reat_contractor_details.MAST_ACCOUNT_ID && x.MASTER_DISTRICT.OMMAS_LDG_DISTRICT_MAPPING.MAST_DISTRICT_LDG_CODE == reat_contractor_details.MAST_LGD_DISTRICT_CODE).FirstOrDefault();
        //                    //    if (bank != null)
        //                    //    {
        //                    //        bank.MAST_LOCK_STATUS = "N";
        //                    //        dbContext.Entry(bank).State = System.Data.Entity.EntityState.Modified;
        //                    //    }
        //                    //}
        //                    #endregion
        //                }
        //            }

        //            if (reat_data_send_details != null)
        //            {
        //                reat_data_send_details.RESPONSE_RECEIVED_DATE = new CommonFunctions().GetStringToDateTime(responseDate.Trim());
        //                reat_data_send_details.RECEIVED_FILE_NAME = xmlFileName.Trim().Substring(0, xmlFileName.IndexOf('.'));
        //                reat_data_send_details.RECEIVED_FILE_PATH = null;//To be changed in Windows service
        //                dbContext.Entry(reat_data_send_details).State = System.Data.Entity.EntityState.Modified;
        //            }
        //            if (reat_contractor_details != null || reat_contractor_details != null)
        //            {
        //                //dbContext.SaveChanges();
        //                REATLog("Beneficiary", ConfigurationManager.AppSettings["PFMSBeneficiaryLog"].ToString(), "Beneficiary acknowledgement successful", xmlFileName);
        //            }
        //            else
        //            {
        //                REATLog("Beneficiary", ConfigurationManager.AppSettings["PFMSBeneficiaryLog"].ToString(), "No Records updated for Beneficiary acknowledgement", xmlFileName);
        //            }
        //            //ts.Complete();
        //            dbContext.SaveChanges();
        //        }
        //        return true;
        //    }
        //    catch (DbEntityValidationException e)
        //    {
        //        //Elmah.ErrorSignal.FromCurrentContext().Raise(e, HttpContext.Current);
        //        ErrorLog.LogError(e, "SaveRoadProposalDAL(DbEntityValidationException ex).DAL");
        //        ModelStateDictionary modelstate = new ModelStateDictionary();

        //        foreach (var eve in e.EntityValidationErrors)
        //        {
        //            foreach (var ve in eve.ValidationErrors)
        //            {
        //                modelstate.AddModelError(ve.ErrorMessage, ve.ErrorMessage);
        //            }
        //        }
        //        ErrorLog.LogError(e, "REATDAL.EditREATContractorDetails()");
        //        using (System.IO.StreamWriter sw = System.IO.File.AppendText(System.IO.Path.Combine("D:\\ErrorLog\\OMMASErrorLog_" + System.DateTime.Now.Date.ToString("dd_MM_yyyy") + ".txt", "")))
        //        {
        //            sw.WriteLine("Date :" + DateTime.Now.ToString());
        //            sw.WriteLine("REATDAL.EditREATContractorDetails() : " + "Application_Error()");

        //            sw.WriteLine("Exception : " + new CommonFunctions().FormatErrorMessage(modelstate));
        //            sw.WriteLine("---------------------------------------------------------------------------------------");
        //            sw.Close();
        //        }

        //        //return new CommonFunctions().FormatErrorMessage(modelstate);
        //        return false;
        //    }

        //    catch (DbUpdateException ex)
        //    {
        //        //Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
        //        ErrorLog.LogError(ex, "REATDAL.EditREATContractorDetails()");
        //        return false;
        //    }
        //    catch (OptimisticConcurrencyException ex)
        //    {
        //        //Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
        //        ErrorLog.LogError(ex, "REATDAL.EditREATContractorDetails()");
        //        return false;
        //    }
        //    catch (Exception ex)
        //    {
        //        REATLog("Beneficiary", ConfigurationManager.AppSettings["REATBeneficiaryLog"].ToString(), "Error on Beneficiary acknowledgement", xmlFileName);
        //        ErrorLog.LogError(ex, "REATDAL.EditREATContractorDetails()");
        //        return false;
        //    }
        //    finally
        //    {
        //        if (dbContext != null)
        //        {
        //            dbContext.Dispose();
        //        }
        //    }
        //}
        #endregion

        #region REAT DSC

        public DSCREATModel ValidateDscREATDetails()
        {
            dbContext = new PMGSYEntities();
            try
            {

                ADMIN_NODAL_OFFICERS officer = dbContext.ADMIN_NODAL_OFFICERS.FirstOrDefault(x => x.ADMIN_MODULE == "A" && x.ADMIN_NO_DESIGNATION == 26 && x.ADMIN_ACTIVE_STATUS == "Y" && x.ADMIN_ND_CODE == PMGSYSession.Current.AdminNdCode);  //bill admin code used b4

                ADMIN_DEPARTMENT AdminDeptPIU = dbContext.ADMIN_DEPARTMENT.FirstOrDefault(s => s.ADMIN_ND_CODE == PMGSYSession.Current.AdminNdCode); //bill

                //ADMIN_DEPARTMENT AdminState = dbContext.ADMIN_DEPARTMENT.FirstOrDefault(s => s.MAST_STATE_CODE == AdminDeptPIU.MAST_STATE_CODE && s.MAST_ND_TYPE == "S");
                //ACC_BANK_DETAILS BankDetails = dbContext.ACC_BANK_DETAILS.FirstOrDefault(s => s.FUND_TYPE == "P" && s.ADMIN_ND_CODE == AdminState.ADMIN_ND_CODE && s.BANK_ACC_STATUS == true);
                //REAT_INITIATING_PARTY_MASTER initparty = dbContext.REAT_INITIATING_PARTY_MASTER.FirstOrDefault(s => s.ADMIN_ND_CODE == AdminState.ADMIN_ND_CODE);

                ACC_BANK_DETAILS BankDetails = dbContext.ACC_BANK_DETAILS.FirstOrDefault(s => s.FUND_TYPE == "P" && s.ADMIN_ND_CODE == AdminDeptPIU.MAST_PARENT_ND_CODE && s.BANK_ACC_STATUS == true && s.ACCOUNT_TYPE == "S");
                REAT_INITIATING_PARTY_MASTER initparty = dbContext.REAT_INITIATING_PARTY_MASTER.FirstOrDefault(s => s.ADMIN_ND_CODE == AdminDeptPIU.MAST_PARENT_ND_CODE );

                DSCREATModel model = new DSCREATModel();
                model.IsEmailAvailable = (officer == null) ? false : !String.IsNullOrEmpty(officer.ADMIN_NO_EMAIL.Trim());
                model.IsAccountNumberAvailable = (BankDetails == null) ? false : !String.IsNullOrEmpty(BankDetails.BANK_ACC_NO);
                model.IsIFSCAvailable = (BankDetails == null) ? false : !String.IsNullOrEmpty(BankDetails.MAST_IFSC_CODE);
                if (initparty != null)
                {
                    if (!(string.IsNullOrEmpty(initparty.REAT_INIT_PARTY_UNIQUE_CODE) || string.IsNullOrEmpty(initparty.SCHEME_CODE)))
                    {
                        model.IsInitPartyAvailable = true;
                    }
                    else
                    {
                        model.IsInitPartyAvailable = false;
                    }

                }

                return model;
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "PFMSDAL.ValidateDscREATDetails()");
                return null;
            }
        }

        public DSCREATModel ValidateDscREATDetailsforDelete(int officerCode)
        {
            dbContext = new PMGSYEntities();
            try
            {

                ADMIN_NODAL_OFFICERS officer = dbContext.ADMIN_NODAL_OFFICERS.FirstOrDefault(x => x.ADMIN_MODULE == "A" && x.ADMIN_NO_DESIGNATION == 26 && x.ADMIN_ACTIVE_STATUS == "Y" && x.ADMIN_ND_CODE == PMGSYSession.Current.AdminNdCode && x.ADMIN_NO_OFFICER_CODE == officerCode);  //bill admin code used b4

                ADMIN_DEPARTMENT AdminDeptPIU = dbContext.ADMIN_DEPARTMENT.FirstOrDefault(s => s.ADMIN_ND_CODE == PMGSYSession.Current.AdminNdCode); //bill

                
                ACC_BANK_DETAILS BankDetails = dbContext.ACC_BANK_DETAILS.FirstOrDefault(s => s.FUND_TYPE == "P" && s.ADMIN_ND_CODE == AdminDeptPIU.MAST_PARENT_ND_CODE && s.BANK_ACC_STATUS == true && s.ACCOUNT_TYPE == "S");
                REAT_INITIATING_PARTY_MASTER initparty = dbContext.REAT_INITIATING_PARTY_MASTER.FirstOrDefault(s => s.ADMIN_ND_CODE == AdminDeptPIU.MAST_PARENT_ND_CODE);

                DSCREATModel model = new DSCREATModel();
                model.IsEmailAvailable = (officer == null) ? false : !String.IsNullOrEmpty(officer.ADMIN_NO_EMAIL.Trim());
                model.IsAccountNumberAvailable = (BankDetails == null) ? false : !String.IsNullOrEmpty(BankDetails.BANK_ACC_NO);
                model.IsIFSCAvailable = (BankDetails == null) ? false : !String.IsNullOrEmpty(BankDetails.MAST_IFSC_CODE);
                if (initparty != null)
                    model.IsInitPartyAvailable = true;
                else
                    model.IsInitPartyAvailable = false;

                return model;
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "ReatDAL.ValidateDscREATDetailsforDelete()");
                return null;
            }
        }



        public string GenerateDSCXml(int adminNdCode, out string fileName, string operation, string AdminNdName)
        {
            try
            {
                //Xml code To get List of SRRDA Offices  .Below Code added on 30-12-2021
                XDocument doc_xml = XDocument.Load(System.Web.HttpContext.Current.Server.MapPath("~/OfficeList.xml"));
                List<OfficeDetails> SRRDA_Office_List = new List<OfficeDetails>();
                foreach (XElement element in doc_xml.Descendants("officeList")
                    .Descendants("office"))
                {
                    OfficeDetails data = new OfficeDetails();
                    data.srrda_Code = Convert.ToInt32(element.Element("srrdaCode").Value);
                    data.state_Code = Convert.ToInt32(element.Element("stateCode").Value);
                    data.District_Code = Convert.ToInt32(element.Element("DistrictCode").Value);
                    data.DistrictLDG_Code = Convert.ToInt32(element.Element("DistrictLDGCode").Value);

                    SRRDA_Office_List.Add(data);
                }
                //xml Code end

                dbContext = new PMGSYEntities();

                // ACC_BILL_MASTER bill = dbContext.ACC_BILL_MASTER.FirstOrDefault(s => s.BILL_ID == 2990803);

                ADMIN_NODAL_OFFICERS officer = dbContext.ADMIN_NODAL_OFFICERS.FirstOrDefault(x => x.ADMIN_MODULE == "A" && x.ADMIN_NO_DESIGNATION == 26 && x.ADMIN_ACTIVE_STATUS == "Y" && x.ADMIN_ND_CODE == adminNdCode);  //bill admin code used b4

                ACC_CERTIFICATE_DETAILS cert = dbContext.ACC_CERTIFICATE_DETAILS.FirstOrDefault(s => s.ADMIN_NO_OFFICER_CODE == officer.ADMIN_NO_OFFICER_CODE);

                ADMIN_DEPARTMENT AdminDeptPIU = dbContext.ADMIN_DEPARTMENT.FirstOrDefault(s => s.ADMIN_ND_CODE == adminNdCode); //bill

                //ADMIN_DEPARTMENT AdminState = dbContext.ADMIN_DEPARTMENT.FirstOrDefault(s => s.MAST_STATE_CODE == AdminDeptPIU.MAST_STATE_CODE && s.MAST_ND_TYPE == "S");

                //ACC_BANK_DETAILS BankDetails = dbContext.ACC_BANK_DETAILS.FirstOrDefault(s => s.FUND_TYPE == "P" && s.ADMIN_ND_CODE == AdminState.ADMIN_ND_CODE && s.BANK_ACC_STATUS == true);
                ACC_BANK_DETAILS BankDetails = dbContext.ACC_BANK_DETAILS.FirstOrDefault(s => s.FUND_TYPE == "P" && s.ADMIN_ND_CODE == AdminDeptPIU.MAST_PARENT_ND_CODE && s.BANK_ACC_STATUS == true && s.ACCOUNT_TYPE == "S");

                X509Certificate2 certifiate = new X509Certificate2(Convert.FromBase64String(cert.CERTIFICATE));

                String FileName = string.Empty;

                int runningCount = dbContext.REAT_DATA_SEND_DETAILS.Any(s => s.FILE_TYPE == "D" && s.FUND_TYPE == "P") //bill {{&& s.ADMIN_ND_CODE == adminNdCode}}
                                   ? (dbContext.REAT_DATA_SEND_DETAILS.Where(s => s.FILE_TYPE == "D" && s.FUND_TYPE == "P" && EntityFunctions.TruncateTime(s.FILE_GENERATION_DATE) == DateTime.Today).Count()) + 1 //{{&& s.ADMIN_ND_CODE == adminNdCode}}
                                   : 1;
                FileName = "0037DSCENRREQ" + DateTime.Now.ToString("ddMMyyyy") + "" + runningCount.ToString("D3");

                StringBuilder startstring = new StringBuilder("<DscEnrolmentRequest xmlns=\"http://pfms.nic.in/EnrolmentRequest\"><AcctMndtMntncReq>");
                // string EndString = "</AcctMndtMntncReq></DscEnrolmentRequest>";

                REAT_INITIATING_PARTY_MASTER initparty = dbContext.REAT_INITIATING_PARTY_MASTER.FirstOrDefault(s => s.ADMIN_ND_CODE == /*AdminState.ADMIN_ND_CODE*/ AdminDeptPIU.MAST_PARENT_ND_CODE);
                XDocument doc = new XDocument(new XElement("GrpHdr",
                                                     new XElement("MsgID", "0037DSCENRREQ" + DateTime.Now.ToString("ddMMyyyy") + "" + runningCount.ToString("D3")),
                                                     new XElement("CreDtTm", DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss")),
                                                     new XElement("OrgId",
                                                           new XElement("SchmeCd", initparty.SCHEME_CODE) // new XElement("SchmeCd", "9179")
                                                                 ),
                                                      new XElement("InitgPty",
                                                              new XElement("Nm", initparty.REAT_INIT_PARTYNAME),
                                                              new XElement("Id", initparty.REAT_INIT_PARTY_UNIQUE_CODE)
                                                                  ),
                                                      new XElement("PrcId", "DSC"),
                                                      operation == "A"
                                                      ? new XElement("Prtry",
                                                                  new XElement("TxTp", "ADDN")
                                                                )
                                                      : new XElement("Prtry",
                                                                  new XElement("TxTp", "CANC")
                                                                )
                                                     ));
                string xmlstring = doc.ToString();

                string middleString = "<RptAcct><Accts>";
                XDocument middle_1 = new XDocument(new XElement("Acct",
                                                         new XElement("AcctId",
                                                              new XElement("Id",
                                                                   new XElement("BBAN", BankDetails.BANK_ACC_NO)),
                                                             new XElement("Nm", initparty.REAT_INIT_PARTYNAME),
                                                             new XElement("Tp", "SVGS")))); //saving

                XDocument middle_2 = new XDocument(new XElement("AcctSvcrId",
                                                             new XElement("FinInstnNm", BankDetails.BANK_NAME),
                                                             new XElement("BrnchId", BankDetails.MAST_IFSC_CODE) //to be changed
                                                             )
                                                   );

                int fileId = dbContext.REAT_OMMAS_DSC_MAPPING.Any() ? dbContext.REAT_OMMAS_DSC_MAPPING.Max(s => s.ID) + 1 : 1;
                var endstring = "<Mndt><ID>" + officer.ADMIN_NO_OFFICER_CODE + "-" + fileId.ToString() + "</ID><SgntrOrdrInd>" + 1 + "</SgntrOrdrInd><MndtHldr>";

                //below part repetative

                var stateLgd = dbContext.OMMAS_LDG_STATE_MAPPING.FirstOrDefault(s => s.MAST_STATE_CODE == AdminDeptPIU.MAST_STATE_CODE);
                var districtLgd = dbContext.OMMAS_LDG_DISTRICT_MAPPING.FirstOrDefault(s => s.MAST_DISTRICT_CODE == AdminDeptPIU.MAST_DISTRICT_CODE);

                //Below Code Added on 30-12-2021 to get SRRDA office location Code
                var SRRDA_MAST_DISTRICT_LDG_CODE = "";
                foreach (OfficeDetails element in SRRDA_Office_List)
                {
                    if (element.srrda_Code == AdminDeptPIU.ADMIN_ND_CODE)
                    {
                        SRRDA_MAST_DISTRICT_LDG_CODE = element.DistrictLDG_Code.ToString();
                        break;
                    }
                }
                //Added on 16-11-2021 
                //var MAST_DISTRICT_LDG_CODE = PMGSYSession.Current.LevelId == 4 ? "" : districtLgd.MAST_DISTRICT_LDG_CODE.ToString();
                //Below Code Added on 30-12-=2021
                var MAST_DISTRICT_LDG_CODE = PMGSYSession.Current.LevelId == 4 ? SRRDA_MAST_DISTRICT_LDG_CODE : districtLgd.MAST_DISTRICT_LDG_CODE.ToString();
                

                XDocument LastCert = new XDocument(new XElement("DgtlSgntr",
                                                              new XElement("StartDt", Convert.ToDateTime(certifiate.GetEffectiveDateString()).ToString("yyyy-MM-dd")),
                                                              new XElement("EndDt", Convert.ToDateTime(certifiate.GetExpirationDateString()).ToString("yyyy-MM-dd")),
                                                    new XElement("Pty",
                                                             new XElement("NM", officer.ADMIN_NO_FNAME + " " + (officer.ADMIN_NO_MNAME ?? "") + " " + officer.ADMIN_NO_LNAME),
                                                             new XElement("Cert", certifiate.SerialNumber),
                                                             new XElement("ThumbPrint", certifiate.Thumbprint),
                                                             new XElement("Issr", certifiate.IssuerName.Name),
                                                             new XElement("ID",
                                                                   new XElement("PrvtId",
                    //  new XElement("SOSE",officer.ADMIN_AADHAR_NO)
                    //  new XElement("PAN","")
                                                                     "")
                                                                     ),
                                                             new XElement("Prtry",
                                                                     new XElement("Nm", officer.MASTER_DESIGNATION.MAST_DESIG_NAME)
                                                                     ),
                                                                  new XElement("LglAdr",
                                                                      new XElement("Dept", "Rural Department"),
                                                                      new XElement("SubDept", "Rural Department"),
                    //new XElement("PstCd","?"),
                                                                      //new XElement("DstCd", districtLgd.MAST_DISTRICT_LDG_CODE),
                                                                      new XElement("DstCd", MAST_DISTRICT_LDG_CODE),
                                                                      new XElement("PrvcCd", (stateLgd.MAST_STATE_LDG_CODE_REAT))
                                                                              ),
                                                             new XElement("CtctDtls",
                    //new XElement("MobNb",""),
                                                                 new XElement("EmailAdr", officer.ADMIN_NO_EMAIL))
                                                                ),
                                                     new XElement("Authstn",
                    // new XElement("MinAmtPerTx", "0"),
                                                                  new XElement("MaxAmtPerTx", "10000000")
                                                                 )
                                                                )
                                                              );

                string EndStringRepeatSection = "</MndtHldr>";

                String DscDeclaretion = "I " + officer.ADMIN_NO_FNAME + " " + (officer.ADMIN_NO_MNAME ?? "") + " " + officer.ADMIN_NO_LNAME + " from Department, confirm that account mentioned in this message is operated by me and I shall be using this digital signature for payment signing as appended below with this message within the amount mentioned above through PFMS.";
                XDocument Declartion = new XDocument(new XElement("BkOpr",
                                                              new XElement("Domn",
                                                              new XElement("Cd", "PMNT")
                                                              )
                                                              )
                    //new XElement("MemFld",DscDeclaretion)//+I officer.ADMIN_NO_FNAME +" " +officer.ADMIN_NO_MNAME?""+officer.ADMIN_NO_LNAME +" from Department, confirm that account mentioned in this message is operated by me and I shall be using this digital signature for payment signing as appended below with this message within the amount mentioned above through PFMS.",
                                                        );

                String declartionendString = "<MemFld>" + DscDeclaretion + "</MemFld></Mndt></Accts></RptAcct></AcctMndtMntncReq></DscEnrolmentRequest>";

                startstring.Append(doc.ToString());
                startstring.Append(middleString);
                startstring.Append(middle_1.ToString());
                startstring.Append(middle_2.ToString());
                startstring.Append(endstring);
                startstring.Append(LastCert);
                startstring.Append(EndStringRepeatSection);
                startstring.Append(Declartion.ToString());
                startstring.Append(declartionendString);

                XDocument finaldoc = XDocument.Parse(startstring.ToString());

                //finaldoc.Save(@"d:\LINQ.XML");
                //finaldoc.Save(ConfigurationManager.AppSettings["FinalDSCXmlFilePath"]);

                //REAT_DATA_SEND_DETAILS Datasenddetails = new REAT_DATA_SEND_DETAILS();
                //Datasenddetails.FILE_ID = dbContext.REAT_DATA_SEND_DETAILS.Any() ? dbContext.REAT_DATA_SEND_DETAILS.Max(s => s.FILE_ID) + 1 : 1;
                //Datasenddetails.ADMIN_ND_CODE = adminNdCode;
                //Datasenddetails.FUND_TYPE = "P";
                //Datasenddetails.GENERATED_FILE_NAME = FileName.Trim() + ".xml";
                //Datasenddetails.FILE_GENERATION_DATE = DateTime.Now;
                //Datasenddetails.GENERATED_FILE_PATH = Path.Combine(ConfigurationManager.AppSettings["REATDSCFilePath"].ToString(), FileName.Trim());
                //Datasenddetails.FILE_TYPE = "D";
                //Datasenddetails.RESPONSE_RECEIVED_DATE = null;
                //Datasenddetails.RECEIVED_FILE_NAME = null;
                //Datasenddetails.RECEIVED_FILE_PATH = null;
                //Datasenddetails.ERR_RECEIVED_RESPONSE = null;

                //Datasenddetails.USER_ID = PMGSYSession.Current.UserId;
                //Datasenddetails.IPADDR = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];

                //dbContext.REAT_DATA_SEND_DETAILS.Add(Datasenddetails);

                //REAT_OMMAS_DSC_MAPPING DscMapping = new REAT_OMMAS_DSC_MAPPING();
                //DscMapping.ID = dbContext.REAT_OMMAS_DSC_MAPPING.Any() ? dbContext.REAT_OMMAS_DSC_MAPPING.Max(s => s.ID) + 1 : 1;
                //DscMapping.FILE_ID = Datasenddetails.FILE_ID;
                //DscMapping.ADMIN_ND_CODE = AdminDeptPIU.ADMIN_ND_CODE;//PMGSYSession.Current.AdminNdCode;
                //DscMapping.ADMIN_NO_OFFICER_CODE = officer.ADMIN_NO_OFFICER_CODE;
                //DscMapping.DSC_REQ_FILENAME = FileName.Trim() + ".xml";
                //DscMapping.IS_ACTIVE = true;
                //dbContext.REAT_OMMAS_DSC_MAPPING.Add(DscMapping);

                //dbContext.SaveChanges();

                fileName = FileName.Trim() + ".xml";
                return startstring.ToString();
            }
            catch (OptimisticConcurrencyException ex)
            {
                ErrorLog.LogError(ex, "REATDAL.GenerateDSCXml().OptimisticConcurrencyException");
                fileName = "";
                return String.Empty;
            }
            catch (UpdateException ex)
            {
                ErrorLog.LogError(ex, "REATDAL.GenerateDSCXml().UpdateException");
                fileName = "";
                return String.Empty;
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "REATDAL.GenerateDSCXml()");
                fileName = "";
                return String.Empty;
            }
        }



        #region VIKKY
        public bool IsSecondlevelSuccessPaymentExists(long billId)
        {
            dbContext = new PMGSYEntities();
            try
            {
                return (dbContext.REAT_OMMAS_HOLDING_ACCOUNT_PAYMENTS.Any(x => x.BILL_ID == billId));
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "REATDAL.IsPaymentExists()");
                return false;
            }
        }
        public string GenerateSecondLevelSuccessPaymentXMLDAL(PFMSDownloadPaymentXMLViewModel model, out string fileName, out int runningCnt)
        {
            string xmlString = string.Empty;
            string xmlHeader = string.Empty;
            string xmlBody = string.Empty;

            string genDate = string.Empty;
            int runningCount = 0;

            CommonFunctions comm = new CommonFunctions();
            try
            {
                dbContext = new PMGSYEntities();

                String xmlFileName = String.Empty;
                //int AdminNdCode = dbContext.ADMIN_DEPARTMENT.FirstOrDefault(s => s.MAST_STATE_CODE == model.stateCode && s.MAST_AGENCY_CODE == model.agencyCode && s.MAST_ND_TYPE == "S").ADMIN_ND_CODE;
                int AdminNdCode = PMGSYSession.Current.AdminNdCode;
                DateTime GenrationBillDate = Convert.ToDateTime(model.generationDate);
                model.agencyCode = dbContext.ADMIN_DEPARTMENT.Where(x => x.ADMIN_ND_CODE == PMGSYSession.Current.AdminNdCode).Select(x => x.MAST_AGENCY_CODE).FirstOrDefault();
                model.generationDate = comm.GetDateTimeToString(dbContext.ACC_BILL_MASTER.Where(x => x.BILL_ID == model.billId).Select(x => x.BILL_DATE).FirstOrDefault());
                Int64 BillId = model.billId;
                ObjectParameter outfilename = new System.Data.Entity.Core.Objects.ObjectParameter("XmlFileName", xmlFileName);
                ObjectParameter outParam = new System.Data.Entity.Core.Objects.ObjectParameter("RunningCount", runningCount);

                var hdr = dbContext.REAT_HOLDING_ACCOUNT_PAYMENT_HEADER_XML(model.stateCode, model.agencyCode, Convert.ToDateTime(model.generationDate), model.FileType, BillId, outfilename, outParam).ToList();
                var bdy = dbContext.REAT_GENERATE_HOLDING_ACCOUNT_PAYEMENT_XML(model.stateCode, model.agencyCode, Convert.ToDateTime(model.generationDate), BillId, model.FileType, Convert.ToString(outfilename.Value), Convert.ToInt32(outParam.Value)).ToList();
                xmlHeader = string.Join("", hdr);
                xmlBody = string.Join("", bdy);
                xmlString = xmlHeader.Trim() + xmlBody.Trim();
                XmlSerializer XmlS = new XmlSerializer(xmlString.GetType());
                StringWriter sw = new PMGSY.DAL.PFMS.PFMSDAL1.StringWriterUtf8();
                XmlTextWriter tw = new XmlTextWriter(sw);
                tw.WriteProcessingInstruction("xml", "version=\"1.0\" encoding=\"UTF-8\" standalone=\"yes\"");
                XmlS.Serialize(tw, xmlString);

                xmlString = sw.ToString().Replace("&lt;", "<").Replace("&gt;", ">").Replace("<DbtBeneficiaries>", "").Replace("</DbtBeneficiaries>", "").Replace("<string>", @"<HOLPaymentRequest xmlns=""http://pfms.nic.in/HOLPaymentRequest""><HOLPayReqDetails>").Replace("</string>", "</HOLPayReqDetails></HOLPaymentRequest>");


                //   xmlString = xmlString + xmlHeader.Trim() + xmlBody.Trim();
                fileName = outfilename.Value.ToString();
                runningCnt = Convert.ToInt32(outParam.Value);
                return xmlString;
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "REAT.GenerateSecondLevelSuccessPaymentXMLDAL()");
                fileName = "";
                runningCnt = 0;
                return string.Empty;
            }
        }

        public DSCREATModel ValidateDscREATDetailsForSnaToHolding()
        {
            dbContext = new PMGSYEntities();
            try
            {

                ADMIN_NODAL_OFFICERS officer = dbContext.ADMIN_NODAL_OFFICERS.FirstOrDefault(x => x.ADMIN_MODULE == "A" && x.ADMIN_NO_DESIGNATION == 26 && x.ADMIN_ACTIVE_STATUS == "Y" && x.ADMIN_ND_CODE == PMGSYSession.Current.AdminNdCode);  //bill admin code used b4

                ADMIN_DEPARTMENT AdminDeptPIU = dbContext.ADMIN_DEPARTMENT.FirstOrDefault(s => s.ADMIN_ND_CODE == PMGSYSession.Current.AdminNdCode); //bill

                ACC_BANK_DETAILS BankDetailsC = dbContext.ACC_BANK_DETAILS.FirstOrDefault(s => s.FUND_TYPE == "P" && s.ADMIN_ND_CODE == AdminDeptPIU.MAST_PARENT_ND_CODE && s.BANK_ACC_STATUS == true && s.ACCOUNT_TYPE == "S");
                ACC_BANK_DETAILS BankDetailsD = dbContext.ACC_BANK_DETAILS.FirstOrDefault(s => s.FUND_TYPE == "P" && s.ADMIN_ND_CODE == AdminDeptPIU.MAST_PARENT_ND_CODE && s.BANK_ACC_STATUS == true && s.ACCOUNT_TYPE == "H");
                REAT_INITIATING_PARTY_MASTER initparty = dbContext.REAT_INITIATING_PARTY_MASTER.FirstOrDefault(s => s.ADMIN_ND_CODE == AdminDeptPIU.MAST_PARENT_ND_CODE);

                DSCREATModel model = new DSCREATModel();
                model.IsEmailAvailable = (officer == null) ? false : !String.IsNullOrEmpty(officer.ADMIN_NO_EMAIL.Trim());
                model.IsAccountNumberAvailableC = (BankDetailsC == null) ? false : !String.IsNullOrEmpty(BankDetailsC.BANK_ACC_NO);
                model.IsAccountNumberAvailableD = (BankDetailsD == null) ? false : !String.IsNullOrEmpty(BankDetailsD.BANK_ACC_NO);
                model.IsIFSCAvailableC = (BankDetailsC == null) ? false : !String.IsNullOrEmpty(BankDetailsC.MAST_IFSC_CODE);
                model.IsIFSCAvailableD = (BankDetailsD == null) ? false : !String.IsNullOrEmpty(BankDetailsD.MAST_IFSC_CODE);
                if (initparty != null)
                {
                    if (!(string.IsNullOrEmpty(initparty.REAT_INIT_PARTY_UNIQUE_CODE) || string.IsNullOrEmpty(initparty.SCHEME_CODE)))
                    {
                        model.IsInitPartyAvailable = true;
                    }
                    else
                    {
                        model.IsInitPartyAvailable = false;
                    }

                }

                return model;
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "PFMSDAL.ValidateDscREATDetailsForSnaToHolding()");
                return null;
            }
        }

        #endregion





        public Boolean ValidXmlGenerateSetFlag()
        {
            dbContext = new PMGSYEntities();
            try
            {
                if (dbContext.ADMIN_NODAL_OFFICERS.Where(c => c.ADMIN_ND_CODE == PMGSYSession.Current.AdminNdCode && c.ADMIN_ACTIVE_STATUS.Equals("Y") && c.ADMIN_MODULE.Equals("A")).Any())
                {
                    var nodalOfficerDetails = dbContext.ADMIN_NODAL_OFFICERS.Where(c => c.ADMIN_ND_CODE == PMGSYSession.Current.AdminNdCode && c.ADMIN_ACTIVE_STATUS.Equals("Y") && c.ADMIN_MODULE.Equals("A")).First();
                    //dbContext.ADMIN_NODAL_OFFICERS.Attach(nodalOfficerDetails);
                    //nodalOfficerDetails.IS_VALID_XML = true;
                    //nodalOfficerDetails.XML_FINALIZATION_DATE = DateTime.Now;
                    //dbContext.Entry<ADMIN_NODAL_OFFICERS>(nodalOfficerDetails).State = System.Data.Entity.EntityState.Modified;

                    REAT_OMMAS_DSC_MAPPING DscMapping = dbContext.REAT_OMMAS_DSC_MAPPING.Where(x => x.ADMIN_ND_CODE == PMGSYSession.Current.AdminNdCode).OrderByDescending(c => c.FILE_PROCESS_DATE).FirstOrDefault();
                    if (DscMapping != null)
                    {
                        DscMapping.IS_ACTIVE = true;
                        dbContext.Entry(DscMapping).State = System.Data.Entity.EntityState.Modified;
                    }
                    dbContext.SaveChanges();
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "REATDAL.ValidXmlGenerateSetFlag()");
                return false;
            }
        }

        public bool SaveDscEnrollmentAcknowlegement(XElement doc, string FileName, out bool isRecordExists)
        {
            isRecordExists = false;
            dbContext = new PMGSYEntities();
            try
            {
                XNamespace ns = doc.GetDefaultNamespace();//"";

                var xmlfiledata = (from ack in doc.Element(ns + "DSCEnrolmentAckRpt").Elements(ns + "OrgnlGrpInfAndSts")
                                   select new
                                   {
                                       FileName = FileName,
                                       MesageID = ack.Element(ns + "OrgnlMsgId").Value,
                                       FileStatus = ack.Element(ns + "GrpSts").Value,
                                       //FileStatusReasonCode = ack.Element(ns + "GrpStsRsnInf").Element(ns + "Rsn").Element(ns + "Cd").Value,
                                       //FileStatusReason = ack.Element(ns + "GrpStsRsnInf").Element(ns + "Rsn").Element(ns + "AddtlInf").Value,

                                       FileStatusReasonCode = ((ack.Element(ns + "GrpStsRsnInf").Element(ns + "Rsn").Descendants(ns + "Cd").Count() > 0) ? ack.Element(ns + "GrpStsRsnInf").Element(ns + "Rsn").Element(ns + "Cd").Value : null),
                                       FileStatusReason = ((ack.Element(ns + "GrpStsRsnInf").Element(ns + "Rsn").Descendants(ns + "AddtlInf").Count() > 0) ? ack.Element(ns + "GrpStsRsnInf").Element(ns + "Rsn").Element(ns + "AddtlInf").Value : null),

                                       RecordStatus = ack.Element(ns + "OrgnlSigInfAndSts").Element(ns + "TxSts").Value,

                                       #region
                                       //RejectionCode = ack.Element(ns + "OrgnlSigInfAndSts").Element(ns + "StsRsnInf").Element(ns + "Rsn").Element(ns + "Cd").Value,
                                       //RejectionReason = ack.Element(ns + "OrgnlSigInfAndSts").Element(ns + "StsRsnInf").Element(ns + "Rsn").Element(ns + "AddtlInf").Value

                                       //RejectionCode = ((ack.Element(ns + "OrgnlSigInfAndSts").Element(ns + "StsRsnInf").Element(ns + "Rsn").Descendants(ns + "Cd").Count() > 0) ?
                                       //                     ack.Element(ns + "OrgnlSigInfAndSts").Element(ns + "StsRsnInf").Element(ns + "Rsn").Element(ns + "Cd").Value : null),

                                       //RejectionCode = ((ack.Element(ns + "GrpStsRsnInf").Descendants(ns + "Rsn").Count() > 0) ?
                                       //                     ack.Element(ns + "GrpStsRsnInf").Element(ns + "Rsn").Element(ns + "Cd").Value : null),
                                       //RejectionReason = ((ack.Element(ns + "OrgnlSigInfAndSts").Element(ns + "StsRsnInf").Element(ns + "Rsn").Descendants(ns + "AddtlInf").Count() > 0) ?
                                       //                     ack.Element(ns + "OrgnlSigInfAndSts").Element(ns + "StsRsnInf").Element(ns + "Rsn").Element(ns + "AddtlInf").Value : null)
                                       #endregion
                                       RejectionCode = ((ack.Element(ns + "GrpStsRsnInf").Element(ns + "Rsn").Descendants(ns + "Cd").Count() > 0) ?
                                                 (from cd in ack.Element(ns + "GrpStsRsnInf").Elements(ns + "Rsn") //.Element(ns + "Cd").Value : null,
                                                  select cd.Element(ns + "Cd").Value).ToList() : null),

                                       RejectionReason = ((ack.Element(ns + "GrpStsRsnInf").Element(ns + "Rsn").Descendants(ns + "AddtlInf").Count() > 0) ?
                                                 (from cd in ack.Element(ns + "GrpStsRsnInf").Elements(ns + "Rsn") //.Element(ns + "Cd").Value : null,
                                                  select cd.Element(ns + "AddtlInf").Value).ToList() : null),
                                       pfmsResponseDate = (from cd in doc.Element(ns + "DSCEnrolmentAckRpt").Elements(ns + "GrpHdr") select cd.Element(ns + "CreDtTm").Value).FirstOrDefault(),
                                   }).ToList();

                REAT_OMMAS_DSC_MAPPING DscMapping = null;


                for (int i = 0; i < xmlfiledata.Count; i++)
                {
                    String originalFile = xmlfiledata[i].MesageID + ".xml";
                    REAT_DATA_SEND_DETAILS OrigMsg = dbContext.REAT_DATA_SEND_DETAILS.SingleOrDefault(s => s.GENERATED_FILE_NAME == originalFile);

                    OrigMsg.RECEIVED_FILE_NAME = FileName;
                    OrigMsg.RESPONSE_RECEIVED_DATE = DateTime.Now;

                    //if (dbContext.PFMS_OMMAS_DSC_MAPPING.Any(x => x.ADMIN_ND_CODE == OrigMsg.ADMIN_ND_CODE && x.DSC_REQ_FILENAME == DscMapping.DSC_REQ_FILENAME))
                    //{
                    //    isRecordExists = true;
                    //    return false;
                    //    PFMSLog("DSC", ConfigurationManager.AppSettings["PFMSDSCLog"].ToString(), "DSC already acknowledged", FileName);
                    //}

                    if (OrigMsg != null)
                    {
                        if (dbContext.REAT_OMMAS_DSC_MAPPING.Any(x => x.ADMIN_ND_CODE == OrigMsg.ADMIN_ND_CODE && x.DSC_REQ_FILENAME == originalFile))
                        {
                            ///If Previous status is rejected and current status is accepted then change status to accept else do nothing
                            DscMapping = dbContext.REAT_OMMAS_DSC_MAPPING.Where(x => x.ADMIN_ND_CODE == OrigMsg.ADMIN_ND_CODE && x.DSC_REQ_FILENAME == originalFile).FirstOrDefault();
                            if (/*DscMapping.ACK_DSC_STATUS.Trim() == "RJCT" &&*/ (xmlfiledata[i].FileStatus == "ACPT" || xmlfiledata[i].FileStatus == "ACCP"))
                            {
                                string responseDate = xmlfiledata[i].pfmsResponseDate.Substring(0, 10).Split('-')[2] + "/" + xmlfiledata[i].pfmsResponseDate.Substring(0, 10).Split('-')[1] + "/" + xmlfiledata[i].pfmsResponseDate.Substring(0, 10).Split('-')[0];
                                DscMapping.ACK_RECEIVED_DATE = new CommonFunctions().GetStringToDateTime(responseDate.Trim());//DateTime.Now;
                                DscMapping.ACK_RECEVIED_FILENAME = FileName.Trim();            // Added by Aditi 9 April 2020
                                DscMapping.ACK_DSC_STATUS = xmlfiledata[i].FileStatus.Trim();
                                DscMapping.REJECTION_CODE = null;
                                DscMapping.REJECTION_NARRATION = null;
                                DscMapping.FILE_PROCESS_DATE = DateTime.Now;
                                DscMapping.IS_ACTIVE = true;                                                                    // Added by Aditi 9 April 2020

                                dbContext.Entry(DscMapping).State = System.Data.Entity.EntityState.Modified;

                                //if (DscMapping.IS_ACTIVE == true)
                                /*{
                                    var nodalOfficerDetails = dbContext.ADMIN_NODAL_OFFICERS.Where(c => c.ADMIN_ND_CODE == OrigMsg.ADMIN_ND_CODE && c.ADMIN_ACTIVE_STATUS.Equals("Y") && c.ADMIN_MODULE.Equals("A")).FirstOrDefault();
                                    if (nodalOfficerDetails != null)
                                    {
                                        nodalOfficerDetails.IS_VALID_XML = true;
                                        nodalOfficerDetails.XML_FINALIZATION_DATE = DateTime.Now;
                                        dbContext.Entry(nodalOfficerDetails).State = System.Data.Entity.EntityState.Modified;
                                    }
                                }*/
                            }
                            else if (xmlfiledata[i].FileStatus == "RJCT")              // Added by Aditi Shree on 9 April 2020
                            {
                                string responseDate = xmlfiledata[i].pfmsResponseDate.Substring(0, 10).Split('-')[2] + "/" + xmlfiledata[i].pfmsResponseDate.Substring(0, 10).Split('-')[1] + "/" + xmlfiledata[i].pfmsResponseDate.Substring(0, 10).Split('-')[0];
                                DscMapping.ACK_RECEIVED_DATE = new CommonFunctions().GetStringToDateTime(responseDate.Trim());
                                DscMapping.ACK_RECEVIED_FILENAME = FileName.Trim();
                                DscMapping.ACK_DSC_STATUS = xmlfiledata[i].FileStatus.Trim();
                                DscMapping.REJECTION_CODE = xmlfiledata[i].RejectionCode == null ? null : string.Join(",", xmlfiledata[i].RejectionCode).Trim();
                                DscMapping.REJECTION_NARRATION = xmlfiledata[i].RejectionReason == null ? null : string.Join(",", xmlfiledata[i].RejectionReason).Trim();
                                DscMapping.FILE_PROCESS_DATE = DateTime.Now;
                                DscMapping.IS_ACTIVE = (DscMapping.ACK_DSC_STATUS.Trim() == "RJCT") ? false : true;

                                dbContext.Entry(DscMapping).State = System.Data.Entity.EntityState.Modified;
                            }
                            else
                            {
                                isRecordExists = true;
                                REATLog("DSC", ConfigurationManager.AppSettings["REATDSCLog"].ToString(), "DSC already acknowledged", FileName);
                                return false;
                            }
                        }
                        else
                        {
                            /* DscMapping = new REAT_OMMAS_DSC_MAPPING();

                             DscMapping.ID = dbContext.REAT_OMMAS_DSC_MAPPING.Any() ? dbContext.REAT_OMMAS_DSC_MAPPING.Max(s => s.ID) + 1 : 1;
                             DscMapping.ACK_RECEVIED_FILENAME = FileName.Trim();
                             DscMapping.DSC_REQ_FILENAME = xmlfiledata[i].MesageID + ".xml";
                             DscMapping.ACK_RECEIVED_DATE = DateTime.Now;
                             DscMapping.ACK_DSC_STATUS = xmlfiledata[i].FileStatus.Trim();
                             //DscMapping.REJECTION_CODE = String.IsNullOrEmpty(xmlfiledata[i].RejectionCode) ? null : string.Join(",", xmlfiledata[i].RejectionCode);//xmlfiledata[i].FileStatusReasonCode;
                             //DscMapping.REJECTION_NARRATION = String.IsNullOrEmpty(xmlfiledata[i].RejectionReason) ? null : string.Join(",", xmlfiledata[i].RejectionReason);//xmlfiledata[i].FileStatusReason;

                             DscMapping.REJECTION_CODE = xmlfiledata[i].RejectionCode == null ? null : string.Join(",", xmlfiledata[i].RejectionCode).Trim();//xmlfiledata[i].FileStatusReasonCode;
                             DscMapping.REJECTION_NARRATION = xmlfiledata[i].RejectionReason == null ? null : string.Join(",", xmlfiledata[i].RejectionReason).Trim();//xmlfiledata[i].FileStatusReason;

                             DscMapping.IS_ACTIVE = (DscMapping.ACK_DSC_STATUS.Trim() == "RJCT") ? false : true;

                             DscMapping.ADMIN_ND_CODE = OrigMsg.ADMIN_ND_CODE;
                             DscMapping.FILE_PROCESS_DATE = DateTime.Now;
                             dbContext.REAT_OMMAS_DSC_MAPPING.Add(DscMapping);

                             #region on Rejection of DSC
                             if (DscMapping.IS_ACTIVE == false)
                             {
                                 var nodalOfficerDetails = dbContext.ADMIN_NODAL_OFFICERS.Where(c => c.ADMIN_ND_CODE == OrigMsg.ADMIN_ND_CODE && c.ADMIN_ACTIVE_STATUS.Equals("Y") && c.ADMIN_MODULE.Equals("A")).FirstOrDefault();
                                 if (nodalOfficerDetails != null)
                                 {
                                     nodalOfficerDetails.IS_VALID_XML = false;
                                     nodalOfficerDetails.XML_FINALIZATION_DATE = null;
                                     dbContext.Entry(nodalOfficerDetails).State = System.Data.Entity.EntityState.Modified;
                                 }
                             }
                             #endregion
                            */
                        }
                    }
                    else
                    {
                        REATLog("DSC", ConfigurationManager.AppSettings["REATDSCLog"].ToString(), "No Records to update for DSC acknowledgement", FileName);
                        return false;
                    }
                }
                if (DscMapping != null)
                {
                    dbContext.SaveChanges();
                    REATLog("DSC", ConfigurationManager.AppSettings["REATDSCLog"].ToString(), "DSC acknowledged successful", FileName);
                }
                return true;
            }
            catch (Exception ex)
            {
                REATLog("DSC", ConfigurationManager.AppSettings["REATDSCLog"].ToString(), "Error on DSC acknowledgement", FileName);
                ErrorLog.LogError(ex, "REATDAL.SaveDscEnrollmentAcknowlegement()");
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

        //public bool SaveDscEnrollmentAcknowlegement(XElement doc, string FileName, out bool isRecordExists)
        //{
        //    isRecordExists = false;
        //    dbContext = new PMGSYEntities();
        //    try
        //    {
        //        XNamespace ns = doc.GetDefaultNamespace();//"";

        //        var xmlfiledata = (from ack in doc.Element(ns + "DSCEnrolmentAckRpt").Elements(ns + "OrgnlGrpInfAndSts")
        //                           select new
        //                           {
        //                               FileName = FileName,
        //                               MesageID = ack.Element(ns + "OrgnlMsgId").Value,
        //                               FileStatus = ack.Element(ns + "GrpSts").Value,
        //                               //FileStatusReasonCode = ack.Element(ns + "GrpStsRsnInf").Element(ns + "Rsn").Element(ns + "Cd").Value,
        //                               //FileStatusReason = ack.Element(ns + "GrpStsRsnInf").Element(ns + "Rsn").Element(ns + "AddtlInf").Value,

        //                               FileStatusReasonCode = ((ack.Element(ns + "GrpStsRsnInf").Element(ns + "Rsn").Descendants(ns + "Cd").Count() > 0) ? ack.Element(ns + "GrpStsRsnInf").Element(ns + "Rsn").Element(ns + "Cd").Value : null),
        //                               FileStatusReason = ((ack.Element(ns + "GrpStsRsnInf").Element(ns + "Rsn").Descendants(ns + "AddtlInf").Count() > 0) ? ack.Element(ns + "GrpStsRsnInf").Element(ns + "Rsn").Element(ns + "AddtlInf").Value : null),

        //                               RecordStatus = ack.Element(ns + "OrgnlSigInfAndSts").Element(ns + "TxSts").Value,

        //                               #region
        //                               //RejectionCode = ack.Element(ns + "OrgnlSigInfAndSts").Element(ns + "StsRsnInf").Element(ns + "Rsn").Element(ns + "Cd").Value,
        //                               //RejectionReason = ack.Element(ns + "OrgnlSigInfAndSts").Element(ns + "StsRsnInf").Element(ns + "Rsn").Element(ns + "AddtlInf").Value

        //                               //RejectionCode = ((ack.Element(ns + "OrgnlSigInfAndSts").Element(ns + "StsRsnInf").Element(ns + "Rsn").Descendants(ns + "Cd").Count() > 0) ?
        //                               //                     ack.Element(ns + "OrgnlSigInfAndSts").Element(ns + "StsRsnInf").Element(ns + "Rsn").Element(ns + "Cd").Value : null),

        //                               //RejectionCode = ((ack.Element(ns + "GrpStsRsnInf").Descendants(ns + "Rsn").Count() > 0) ?
        //                               //                     ack.Element(ns + "GrpStsRsnInf").Element(ns + "Rsn").Element(ns + "Cd").Value : null),
        //                               //RejectionReason = ((ack.Element(ns + "OrgnlSigInfAndSts").Element(ns + "StsRsnInf").Element(ns + "Rsn").Descendants(ns + "AddtlInf").Count() > 0) ?
        //                               //                     ack.Element(ns + "OrgnlSigInfAndSts").Element(ns + "StsRsnInf").Element(ns + "Rsn").Element(ns + "AddtlInf").Value : null)
        //                               #endregion
        //                               RejectionCode = ((ack.Element(ns + "GrpStsRsnInf").Element(ns + "Rsn").Descendants(ns + "Cd").Count() > 0) ?
        //                                         (from cd in ack.Element(ns + "GrpStsRsnInf").Elements(ns + "Rsn") //.Element(ns + "Cd").Value : null,
        //                                          select cd.Element(ns + "Cd").Value).ToList() : null),

        //                               RejectionReason = ((ack.Element(ns + "GrpStsRsnInf").Element(ns + "Rsn").Descendants(ns + "AddtlInf").Count() > 0) ?
        //                                         (from cd in ack.Element(ns + "GrpStsRsnInf").Elements(ns + "Rsn") //.Element(ns + "Cd").Value : null,
        //                                          select cd.Element(ns + "AddtlInf").Value).ToList() : null),
        //                               pfmsResponseDate = (from cd in doc.Element(ns + "DSCEnrolmentAckRpt").Elements(ns + "GrpHdr") select cd.Element(ns + "CreDtTm").Value).FirstOrDefault(),
        //                           }).ToList();

        //        REAT_OMMAS_DSC_MAPPING DscMapping = null;


        //        for (int i = 0; i < xmlfiledata.Count; i++)
        //        {
        //            String originalFile = xmlfiledata[i].MesageID + ".xml";
        //            REAT_DATA_SEND_DETAILS OrigMsg = dbContext.REAT_DATA_SEND_DETAILS.SingleOrDefault(s => s.GENERATED_FILE_NAME == originalFile);

        //            OrigMsg.RECEIVED_FILE_NAME = FileName;
        //            OrigMsg.RESPONSE_RECEIVED_DATE = DateTime.Now;

        //            //if (dbContext.PFMS_OMMAS_DSC_MAPPING.Any(x => x.ADMIN_ND_CODE == OrigMsg.ADMIN_ND_CODE && x.DSC_REQ_FILENAME == DscMapping.DSC_REQ_FILENAME))
        //            //{
        //            //    isRecordExists = true;
        //            //    return false;
        //            //    PFMSLog("DSC", ConfigurationManager.AppSettings["PFMSDSCLog"].ToString(), "DSC already acknowledged", FileName);
        //            //}

        //            if (OrigMsg != null)
        //            {
        //                if (dbContext.REAT_OMMAS_DSC_MAPPING.Any(x => x.ADMIN_ND_CODE == OrigMsg.ADMIN_ND_CODE && x.DSC_REQ_FILENAME == originalFile))
        //                {
        //                    ///If Previous status is rejected and current status is accepted then change status to accept else do nothing
        //                    DscMapping = dbContext.REAT_OMMAS_DSC_MAPPING.Where(x => x.ADMIN_ND_CODE == OrigMsg.ADMIN_ND_CODE && x.DSC_REQ_FILENAME == originalFile).FirstOrDefault();
        //                    if (/*DscMapping.ACK_DSC_STATUS.Trim() == "RJCT" &&*/ (xmlfiledata[i].FileStatus == "ACPT" || xmlfiledata[i].FileStatus == "ACCP"))
        //                    {
        //                        string responseDate = xmlfiledata[i].pfmsResponseDate.Substring(0, 10).Split('-')[2] + "/" + xmlfiledata[i].pfmsResponseDate.Substring(0, 10).Split('-')[1] + "/" + xmlfiledata[i].pfmsResponseDate.Substring(0, 10).Split('-')[0];
        //                    DscMapping.ACK_RECEIVED_DATE = new CommonFunctions().GetStringToDateTime(responseDate.Trim());//DateTime.Now;

        //                        DscMapping.ACK_DSC_STATUS = xmlfiledata[i].FileStatus.Trim();
        //                        DscMapping.REJECTION_CODE = null;
        //                        DscMapping.REJECTION_NARRATION = null;

        //                        DscMapping.FILE_PROCESS_DATE = DateTime.Now;

        //                        //DscMapping.IS_ACTIVE = true;
        //                        dbContext.Entry(DscMapping).State = System.Data.Entity.EntityState.Modified;

        //                        //if (DscMapping.IS_ACTIVE == true)
        //                        /*{
        //                            var nodalOfficerDetails = dbContext.ADMIN_NODAL_OFFICERS.Where(c => c.ADMIN_ND_CODE == OrigMsg.ADMIN_ND_CODE && c.ADMIN_ACTIVE_STATUS.Equals("Y") && c.ADMIN_MODULE.Equals("A")).FirstOrDefault();
        //                            if (nodalOfficerDetails != null)
        //                            {
        //                                nodalOfficerDetails.IS_VALID_XML = true;
        //                                nodalOfficerDetails.XML_FINALIZATION_DATE = DateTime.Now;
        //                                dbContext.Entry(nodalOfficerDetails).State = System.Data.Entity.EntityState.Modified;
        //                            }
        //                        }*/
        //                    }
        //                    else
        //                    {
        //                        isRecordExists = true;
        //                        REATLog("DSC", ConfigurationManager.AppSettings["REATDSCLog"].ToString(), "DSC already acknowledged", FileName);
        //                        return false;
        //                    }
        //                }
        //                else
        //                {
        //                   /* DscMapping = new REAT_OMMAS_DSC_MAPPING();

        //                    DscMapping.ID = dbContext.REAT_OMMAS_DSC_MAPPING.Any() ? dbContext.REAT_OMMAS_DSC_MAPPING.Max(s => s.ID) + 1 : 1;
        //                    DscMapping.ACK_RECEVIED_FILENAME = FileName.Trim();
        //                    DscMapping.DSC_REQ_FILENAME = xmlfiledata[i].MesageID + ".xml";
        //                    DscMapping.ACK_RECEIVED_DATE = DateTime.Now;
        //                    DscMapping.ACK_DSC_STATUS = xmlfiledata[i].FileStatus.Trim();
        //                    //DscMapping.REJECTION_CODE = String.IsNullOrEmpty(xmlfiledata[i].RejectionCode) ? null : string.Join(",", xmlfiledata[i].RejectionCode);//xmlfiledata[i].FileStatusReasonCode;
        //                    //DscMapping.REJECTION_NARRATION = String.IsNullOrEmpty(xmlfiledata[i].RejectionReason) ? null : string.Join(",", xmlfiledata[i].RejectionReason);//xmlfiledata[i].FileStatusReason;

        //                    DscMapping.REJECTION_CODE = xmlfiledata[i].RejectionCode == null ? null : string.Join(",", xmlfiledata[i].RejectionCode).Trim();//xmlfiledata[i].FileStatusReasonCode;
        //                    DscMapping.REJECTION_NARRATION = xmlfiledata[i].RejectionReason == null ? null : string.Join(",", xmlfiledata[i].RejectionReason).Trim();//xmlfiledata[i].FileStatusReason;

        //                    DscMapping.IS_ACTIVE = (DscMapping.ACK_DSC_STATUS.Trim() == "RJCT") ? false : true;

        //                    DscMapping.ADMIN_ND_CODE = OrigMsg.ADMIN_ND_CODE;
        //                    DscMapping.FILE_PROCESS_DATE = DateTime.Now;
        //                    dbContext.REAT_OMMAS_DSC_MAPPING.Add(DscMapping);

        //                    #region on Rejection of DSC
        //                    if (DscMapping.IS_ACTIVE == false)
        //                    {
        //                        var nodalOfficerDetails = dbContext.ADMIN_NODAL_OFFICERS.Where(c => c.ADMIN_ND_CODE == OrigMsg.ADMIN_ND_CODE && c.ADMIN_ACTIVE_STATUS.Equals("Y") && c.ADMIN_MODULE.Equals("A")).FirstOrDefault();
        //                        if (nodalOfficerDetails != null)
        //                        {
        //                            nodalOfficerDetails.IS_VALID_XML = false;
        //                            nodalOfficerDetails.XML_FINALIZATION_DATE = null;
        //                            dbContext.Entry(nodalOfficerDetails).State = System.Data.Entity.EntityState.Modified;
        //                        }
        //                    }
        //                    #endregion
        //                    * */
        //                }
        //            }
        //            else
        //            {
        //                REATLog("DSC", ConfigurationManager.AppSettings["REATDSCLog"].ToString(), "No Records to update for DSC acknowledgement", FileName);
        //                return false;
        //            }
        //        }
        //        if (DscMapping != null)
        //        {
        //            dbContext.SaveChanges();
        //            REATLog("DSC", ConfigurationManager.AppSettings["REATDSCLog"].ToString(), "DSC acknowledged successful", FileName);
        //        }
        //        return true;
        //    }
        //    catch (Exception ex)
        //    {
        //        REATLog("DSC", ConfigurationManager.AppSettings["REATDSCLog"].ToString(), "Error on DSC acknowledgement", FileName);
        //        ErrorLog.LogError(ex, "REATDAL.SaveDscEnrollmentAcknowlegement()");
        //        return false;
        //    }
        //    finally
        //    {
        //        if (dbContext != null)
        //        {
        //            dbContext.Dispose();
        //        }
        //    }
        //}

        #endregion

        #region Temporary Code to Test Sign Xml on LIVE
        public string GeneratePayXmlDALTEST(out string xmlFName)
        {
            int stateCode = 0, agencyCode = 0;
            string xmlString = string.Empty;
            string xmlHeader = string.Empty;
            string xmlBody = string.Empty;
            string xmlFileName = string.Empty;

            int runningCount = 0;

            var outParam = new System.Data.Entity.Core.Objects.ObjectParameter("XmlFileName", xmlFileName);
            var outParam1 = new System.Data.Entity.Core.Objects.ObjectParameter("RunningCount", runningCount);
            try
            {
                dbContext = new PMGSYEntities();

                agencyCode = dbContext.ADMIN_DEPARTMENT.Where(x => x.ADMIN_ND_CODE == PMGSYSession.Current.AdminNdCode).Select(x => x.MAST_AGENCY_CODE).FirstOrDefault();

                //Below lines commented on 25-11-2021
                //var hdr = dbContext.REAT_PAYMENT_HEADER_XML_TEMP(PMGSYSession.Current.StateCode, agencyCode, PMGSYSession.Current.AdminNdCode, outParam, outParam1).ToList();
                //var bdy = dbContext.REAT_GENERATE_PAYEMENT_XML_TEMP(PMGSYSession.Current.StateCode, agencyCode, PMGSYSession.Current.AdminNdCode, Convert.ToString(outParam.Value), Convert.ToInt32(outParam1.Value)).ToList();
                    
                //Below code added on 25-11-2021
                if (PMGSYSession.Current.FundType == "P")
                {
                    var hdr = dbContext.REAT_PAYMENT_HEADER_XML_TEMP(PMGSYSession.Current.StateCode, agencyCode, PMGSYSession.Current.AdminNdCode, outParam, outParam1).ToList();
                    var bdy = dbContext.REAT_GENERATE_PAYEMENT_XML_TEMP(PMGSYSession.Current.StateCode, agencyCode, PMGSYSession.Current.AdminNdCode, Convert.ToString(outParam.Value), Convert.ToInt32(outParam1.Value)).ToList();
                    xmlHeader = string.Join("", hdr);
                    xmlBody = string.Join("", bdy);
                }
                else if (PMGSYSession.Current.FundType == "A")
                {
                    var hdr = dbContext.REAT_PAYMENT_HEADER_XML_TEMP_Admin(PMGSYSession.Current.StateCode, agencyCode, PMGSYSession.Current.AdminNdCode, outParam, outParam1, PMGSYSession.Current.FundType, PMGSYSession.Current.LevelId).ToList();
                    var bdy = dbContext.REAT_GENERATE_PAYEMENT_XML_TEMP_Admin(PMGSYSession.Current.StateCode, agencyCode, PMGSYSession.Current.AdminNdCode, Convert.ToString(outParam.Value), Convert.ToInt32(outParam1.Value), PMGSYSession.Current.FundType, PMGSYSession.Current.LevelId).ToList();
                    xmlHeader = string.Join("", hdr);
                    xmlBody = string.Join("", bdy);
                }
                
               // xmlFName = Convert.ToString(outParam.Value);
                xmlFName = outParam.Value.ToString().Split('$')[0].ToString(); 

                //Below lines commented on 25-11-2021

                //xmlHeader = string.Join("", hdr);
                //xmlBody = string.Join("", bdy);
                xmlString = xmlHeader.Trim() + xmlBody.Trim();

                XmlSerializer XmlS = new XmlSerializer(xmlString.GetType());

                StringWriter sw = new PMGSY.DAL.PFMS.PFMSDAL1.StringWriterUtf8();
                XmlTextWriter tw = new XmlTextWriter(sw);
                tw.WriteProcessingInstruction("xml", "version=\"1.0\" encoding=\"UTF-8\" standalone=\"yes\"");

                XmlS.Serialize(tw, xmlString);

                xmlString = sw.ToString().Replace("&lt;", "<").Replace("&gt;", ">").Replace("<DbtBeneficiaries>", "").Replace("</DbtBeneficiaries>", "").Replace("<string>", @"<EATPaymentRequest xmlns=""http://cpsms.nic.in/EATPaymentRequest""><CstmrCdtTrfInitn>").Replace("</string>", "</CstmrCdtTrfInitn></EATPaymentRequest>");

                return xmlString;
            }
            catch (Exception ex)
            {
                xmlFName = "";
                ErrorLog.LogError(ex, "GeneratePayXmlDAL()");
                //fileName = "";
                return String.Empty;
            }
        }

        public bool ValidateSamplePayment()
        {
            try
            {
                dbContext = new PMGSYEntities();
                return (dbContext.REAT_PAYMENT_CONFIGURATION_TABLE.Any(x => x.ADMIN_ND_CODE == PMGSYSession.Current.AdminNdCode && x.IS_ENABLE == true && x.FUND_TYPE == PMGSYSession.Current.FundType && (x.IS_XML_GENERATED == false || x.IS_XML_GENERATED == null)));
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "ReatDAL.ValidateSamplePayment()");
                return false;
            }
            //return true;
        }
        #endregion



        #region REAT Payment

        public bool SavePaymentAcknowlegementDetails(XElement doc, string FileName, out bool isRecordExists)// Added by Aditi Shree on 2 April 2020
        {
            isRecordExists = false;
            dbContext = new PMGSYEntities();
            try
            {
                XNamespace ns = doc.GetDefaultNamespace();
                #region code for reference
                //var xmlfiledata = (from ack in doc.Element(ns + "CstmrCdtTrfInitn").Elements(ns + "CdtTrfTxInf")
                //                   select new
                //                   {
                //                       FileName = FileName,
                //                       MessageId = (from cd in doc.Element(ns + "CstmrCdtTrfInitn").Elements(ns + "GrpHdr") select cd.Element(ns + "MsgId").Value).FirstOrDefault(),
                //                       PartyId = (from cd in doc.Element(ns + "CstmrCdtTrfInitn").Elements(ns + "GrpHdr").Elements(ns + "InitgPty").Elements(ns + "PrTry") select cd.Element(ns + "Id").Value).FirstOrDefault(),
                //                       SchemeCd = (from cd in doc.Element(ns + "CstmrCdtTrfInitn").Elements(ns + "GrpHdr").Elements(ns + "PmtInfDt").Elements(ns + "OrgId") select cd.Element(ns + "SchmeCd").Value).FirstOrDefault(),
                //                       ResponseDate = (from cd in doc.Element(ns + "CstmrCdtTrfInitn").Element(ns + "GrpHdr").Element(ns + "PmtInfDt").Elements(ns + "SrcSystmIdntfr") select cd.Element(ns + "PmtInfDt").Value).FirstOrDefault(),
                //                       //BillId =
                //                       //BatchId=
                //                       PFMSId = ack.Element(ns + "Cdtr").Element(ns + "PFMSId").Value,
                //                       InstAmount = ack.Element(ns + "Cdtr").Element(ns + "Amt").Element(ns + "InstdAmt").Value,
                //                       PaymentMode = ack.Element(ns + "Cdtr").Element(ns + "PmtMd").Value,
                //                       StatusReason = ((ack.Element(ns + "Cdtr").Element(ns + "StsRsnInf").Element(ns + "Rsn").Descendants(ns + "AddtlInf").Count() > 0) ? ack.Element(ns + "Cdtr").Element(ns + "StsRsnInf").Element(ns + "Rsn").Element(ns + "AddtlInf").Value : null),
                //                       BranchId = ack.Element(ns + "Cdtr").Element(ns + "CdtrAgt").Element(ns + "FinInstnId").Element(ns + "BrnchId").Value,
                //                       AccountNumber = ack.Element(ns + "Cdtr").Element(ns + "CdtrAcct").Element(ns + "Id").Element(ns + "Othr").Element(ns + "BBAN").Value,
                //                       Id = ack.Element(ns + "Cdtr").Element(ns + "PymntHd").Element(ns + "Id").Value,
                //                       Amount = ack.Element(ns + "Cdtr").Element(ns + "PymntHd").Element(ns + "Amt").Value,
                //                       ExpTp = ack.Element(ns + "Cdtr").Element(ns + "PymntHd").Element(ns + "ExpTp").Value,
                //                       Response = ack.Element(ns + "Cdtr").Element(ns + "Ddctn").Element(ns + "DdctnApplcbl").Element(ns + "Rspns").Value,
                //                       DeductionAmount = ack.Element(ns + "Cdtr").Element(ns + "Ddctn").Element(ns + "DcdctnDtls").Element(ns + "Amt").Value,
                //                       Tp = ack.Element(ns + "Cdtr").Element(ns + "Ddctn").Element(ns + "DcdctnDtls").Element(ns + "Tp").Value, 
                //                   }).ToList();
                #endregion
                var xmlfiledata = (from ack in doc.Element(ns + "CstmrPmtAckRpt").Elements(ns + "OrgnlPmtInfAndSts")
                                   select new
                                   {
                                       FileName = FileName,
                                       MessageId = (from cd in doc.Element(ns + "CstmrPmtAckRpt").Elements(ns + "GrpHdr").Elements(ns + "OrgnlGrpInfAndSts") select cd.Element(ns + "OrgnlMsgId").Value).FirstOrDefault(),
                                       AckReceivedDate = (from cd in doc.Element(ns + "CstmrPmtAckRpt").Elements(ns + "GrpHdr") select cd.Element(ns + "CreDtTm").Value).FirstOrDefault(),
                                       BatchId = (from cd in doc.Element(ns + "CstmrPmtAckRpt").Elements(ns + "GrpHdr").Elements(ns + "OrgnlGrpInfAndSts") select cd.Element(ns + "OrgnlPmtInfId").Value).FirstOrDefault(),
                                       BillId = ack.Element(ns + "TxInfAndSts").Element(ns + "OrgnlEndToEndId").Value,
                                       Status = ack.Element(ns + "TxInfAndSts").Element(ns + "TxSts").Value,
                                       GrpStatus = (from cd in doc.Element(ns + "CstmrPmtAckRpt").Elements(ns + "GrpHdr").Elements(ns + "OrgnlGrpInfAndSts") select cd.Element(ns + "GrpSts").Value).FirstOrDefault(),
                                       #region Condition working for Rejection file but not for Acceptance file
                                       //RejectionCode = ((ack.Element(ns + "TxInfAndSts").Element(ns + "StsRsnInf").Element(ns + "Rsn").Descendants(ns + "Cd").Count() > 0) ?
                                       //          (from cd in ack.Element(ns + "TxInfAndSts").Element(ns + "StsRsnInf").Elements(ns + "Rsn")
                                       //           select cd.Element(ns + "Cd").Value).ToList() : null),                                     
                                       //RejectionReason = ((ack.Element(ns + "TxInfAndSts").Element(ns + "StsRsnInf").Element(ns + "Rsn").Descendants(ns + "AddtlInf").Count() > 0) ?
                                       //(from cd in ack.Element(ns + "TxInfAndSts").Element(ns + "StsRsnInf").Elements(ns + "Rsn")
                                       // select cd.Element(ns + "AddtlInf").Value).ToList() : null),
                                       //GrpRejectionCode = ((doc.Element(ns + "CstmrPmtAckRpt").Elements(ns + "GrpHdr").Elements(ns + "GrpStsRsnInf").Elements(ns + "Rsn").Descendants(ns + "Cd").Count() > 0)
                                       //                     ? (from cd in doc.Element(ns + "CstmrPmtAckRpt").Elements(ns + "GrpHdr").Elements(ns + "GrpStsRsnInf").Elements(ns + "Rsn") select cd.Element(ns + "Cd").Value).ToList()
                                       //                     : null),
                                       //GrpRejectionReason = ((doc.Element(ns + "CstmrPmtAckRpt").Elements(ns + "GrpHdr").Elements(ns + "GrpStsRsnInf").Elements(ns + "Rsn").Descendants(ns + "AddtlInf").Count() > 0)
                                       //                    ? (from cd in doc.Element(ns + "CstmrPmtAckRpt").Elements(ns + "GrpHdr").Elements(ns + "GrpStsRsnInf").Elements(ns + "Rsn") select cd.Element(ns + "AddtlInf").Value).ToList()
                                       //                    : null),
                                       #endregion
                                       RejectionCode = ((ack.Element(ns + "TxInfAndSts").Element(ns + "StsRsnInf").Descendants(ns + "Rsn").Count() > 0) ?
                                      (from cd in ack.Element(ns + "TxInfAndSts").Element(ns + "StsRsnInf").Elements(ns + "Rsn")
                                       select cd.Element(ns + "Cd").Value).ToList() : null),
                                       RejectionReason = ((ack.Element(ns + "TxInfAndSts").Element(ns + "StsRsnInf").Descendants(ns + "Rsn").Count() > 0) ?
                                       (from cd in ack.Element(ns + "TxInfAndSts").Element(ns + "StsRsnInf").Elements(ns + "Rsn")
                                        select cd.Element(ns + "AddtlInf").Value).ToList() : null),
                                       GrpRejectionCode = ((doc.Element(ns + "CstmrPmtAckRpt").Elements(ns + "GrpHdr").Elements(ns + "GrpStsRsnInf").Descendants(ns + "Rsn").Count() > 0)
                                                             ? (from cd in doc.Element(ns + "CstmrPmtAckRpt").Elements(ns + "GrpHdr").Elements(ns + "GrpStsRsnInf").Elements(ns + "Rsn") select cd.Element(ns + "Cd").Value).ToList()
                                                            : null),
                                       GrpRejectionReason = ((doc.Element(ns + "CstmrPmtAckRpt").Elements(ns + "GrpHdr").Elements(ns + "GrpStsRsnInf").Descendants(ns + "Rsn").Count() > 0)
                                                            ? (from cd in doc.Element(ns + "CstmrPmtAckRpt").Elements(ns + "GrpHdr").Elements(ns + "GrpStsRsnInf").Elements(ns + "Rsn") select cd.Element(ns + "AddtlInf").Value).ToList()
                                                           : null),
                                   }).ToList();
                REAT_OMMAS_PAYMENT_MAPPING PaymentMapping = null;

                for (int i = 0; i < xmlfiledata.Count; i++)
                {
                    String originalFile = xmlfiledata[i].MessageId.ToString();
                    long BillId = Convert.ToInt64(xmlfiledata[i].BillId);
                    String BatchId = xmlfiledata[i].BatchId.ToString();
                    REAT_DATA_SEND_DETAILS OrigMsg = dbContext.REAT_DATA_SEND_DETAILS.SingleOrDefault(s => s.GENERATED_FILE_NAME == originalFile || s.GENERATED_FILE_NAME == originalFile + ".xml");

                    if (OrigMsg != null)
                    {
                        OrigMsg.RECEIVED_FILE_NAME = FileName;
                        OrigMsg.RESPONSE_RECEIVED_DATE = DateTime.Now;

                        if (dbContext.REAT_OMMAS_PAYMENT_MAPPING.Any(x => x.PAYMENT_REQ_FILENAME == originalFile || x.PAYMENT_REQ_FILENAME == originalFile + ".xml"))
                        {
                            PaymentMapping = dbContext.REAT_OMMAS_PAYMENT_MAPPING.Where(x => x.BILL_ID == BillId /*&& x.BATCH_ID == BatchId*/).FirstOrDefault();
                            if (PaymentMapping != null)
                            {
                                string ReceiveDate = xmlfiledata[i].AckReceivedDate.Substring(0, 10).Split('-')[2] + "/" + xmlfiledata[i].AckReceivedDate.Substring(0, 10).Split('-')[1] + "/" + xmlfiledata[i].AckReceivedDate.Substring(0, 10).Split('-')[0];
                                PaymentMapping.ACK_RECEIVED_DATE = new CommonFunctions().GetStringToDateTime(ReceiveDate.Trim());
                                PaymentMapping.ACK_RECEVIED_FILENAME = FileName;
                                PaymentMapping.ACK_BILL_STATUS = (xmlfiledata[i].GrpStatus == "ACCP" || xmlfiledata[i].GrpStatus == "ACPT") ? "A" : "R";

                                if (PaymentMapping.ACK_BILL_STATUS == "R")
                                {
                                    PaymentMapping.REJECTION_CODE = xmlfiledata[i].GrpRejectionCode == null ? null : string.Join(",", xmlfiledata[i].GrpRejectionCode);
                                    PaymentMapping.REJECTION_CODE += xmlfiledata[i].RejectionCode == null ? null : (!string.IsNullOrEmpty(PaymentMapping.REJECTION_CODE)) ? "," + string.Join(",", xmlfiledata[i].RejectionCode) : string.Join(",", xmlfiledata[i].RejectionCode);

                                    PaymentMapping.REJECTION_NARRATION = xmlfiledata[i].GrpRejectionReason == null ? null : string.Join(",", xmlfiledata[i].GrpRejectionReason);
                                    PaymentMapping.REJECTION_NARRATION += xmlfiledata[i].RejectionReason == null ? null : (!string.IsNullOrEmpty(PaymentMapping.REJECTION_NARRATION)) ? "," + string.Join(",", xmlfiledata[i].RejectionReason) : string.Join(",", xmlfiledata[i].RejectionReason);
                                }
                                else
                                {
                                    PaymentMapping.REJECTION_CODE = null;
                                    PaymentMapping.REJECTION_NARRATION = null;
                                }
                                dbContext.Entry(PaymentMapping).State = System.Data.Entity.EntityState.Modified;
                            }
                            else
                            {
                                REATLog("REAT Payment", ConfigurationManager.AppSettings["REATPaymentLog"].ToString(), "No Records to update for Payment acknowledgement", FileName);
                                return false;
                            }
                        }
                        else
                        {
                            REATLog("REAT Payment", ConfigurationManager.AppSettings["REATPaymentLog"].ToString(), "No Records to update for Payment acknowledgement", FileName);
                            return false;
                        }
                    }
                    else
                    {
                        REATLog("REAT Payment", ConfigurationManager.AppSettings["REATPaymentLog"].ToString(), "No Original file found for this Acknowledgement file", FileName);
                        return false;
                    }
                }
                if (PaymentMapping != null)
                {
                    dbContext.SaveChanges();
                    REATLog("REAT Payment", ConfigurationManager.AppSettings["REATPaymentLog"].ToString(), "Payment acknowledgement successful", FileName);
                }
                return true;
            }
            catch (Exception ex)
            {
                REATLog("REAT Payment", ConfigurationManager.AppSettings["REATPaymentLog"].ToString(), "Error on Payment acknowledgement", FileName);
                ErrorLog.LogError(ex, "REATDAL.SavePaymentAcknowlegementDetails()");
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
      
        
        #endregion


        #region REAT Bank


        public bool SaveBankAcknowlegementDetails(XElement doc, string FileName, out bool isRecordExists)// Added by Aditi Shree on 15 April 2020
        {
            isRecordExists = false;
            dbContext = new PMGSYEntities();
            try
            {
                XNamespace ns = doc.GetDefaultNamespace();
                var xmlfiledata = (from ack in doc.Element(ns + "CstmrCdtTrfInitn").Elements(ns + "CdtTrfTxInf")
                                   select new
                                   {
                                       FileName = FileName,
                                       AckReceivedDate = (from cd in doc.Element(ns + "CstmrCdtTrfInitn").Elements(ns + "GrpHdr") select cd.Element(ns + "CreDtTm").Value).FirstOrDefault(),
                                       BatchId = (from cd in doc.Element(ns + "CstmrCdtTrfInitn").Element(ns + "GrpHdr").Element(ns + "PmtInf").Elements(ns + "SrcSystmIdntfr") select cd.Element(ns + "PmtInfId").Value).FirstOrDefault(),
                                       BillId = ack.Element(ns + "PmtId").Element(ns + "EndToEndId").Value,
                                       Status = ack.Element(ns + "TxSts").Value,
                                       InstrId = ack.Element(ns + "PmtId").Element(ns + "InstrId").Value,
                                       CreTxnId = ack.Element(ns + "PmtId").Element(ns + "CrTxID").Value,
                                       TxnDate = ack.Element(ns + "TxDt").Value,

                                       RejectionCode = ((ack.Element(ns + "StsRsnInf").Element(ns + "Rsn").Descendants(ns + "Cd").Count() > 0) ?
                                                 (from cd in ack.Element(ns + "StsRsnInf").Elements(ns + "Rsn")
                                                  select cd.Element(ns + "Cd").Value).ToList() : null),

                                       RejectionReason = ((ack.Element(ns + "StsRsnInf").Element(ns + "Rsn").Descendants(ns + "AddtlInf").Count() > 0) ?
                                                 (from cd in ack.Element(ns + "StsRsnInf").Elements(ns + "Rsn")
                                                  select cd.Element(ns + "AddtlInf").Value).ToList() : null),
                                   }).ToList();
                REAT_OMMAS_PAYMENT_MAPPING BankMapping = null;
                for (int i = 0; i < xmlfiledata.Count; i++)
                {

                    long BillId = Convert.ToInt64(xmlfiledata[i].BillId);
                    String BatchId = xmlfiledata[i].BatchId.ToString();

                    BankMapping = dbContext.REAT_OMMAS_PAYMENT_MAPPING.Where(x => x.BILL_ID == BillId /*&& x.BATCH_ID == BatchId*/).FirstOrDefault();

                    if (BankMapping != null)
                    {
                        string ReceiveDate = xmlfiledata[i].AckReceivedDate.Substring(0, 10).Split('-')[2] + "/" + xmlfiledata[i].AckReceivedDate.Substring(0, 10).Split('-')[1] + "/" + xmlfiledata[i].AckReceivedDate.Substring(0, 10).Split('-')[0];
                        BankMapping.BANK_ACK_RECEIVED_DATE = new CommonFunctions().GetStringToDateTime(ReceiveDate.Trim());
                        BankMapping.BANK_ACK_RECEVIED_FILENAME = FileName;
                        BankMapping.BANK_ACK_BILL_STATUS = (xmlfiledata[i].Status == "ACCP" || xmlfiledata[i].Status == "ACPT") ? "A" : "R";
                        BankMapping.BANK_ACK_INSTRID = xmlfiledata[i].InstrId.Trim();
                        BankMapping.BANK_ACK_CRETXNID = xmlfiledata[i].CreTxnId.Trim();

                        string AckTxnDate = xmlfiledata[i].TxnDate.Substring(0, 10).Split('-')[2] + "/" + xmlfiledata[i].TxnDate.Substring(0, 10).Split('-')[1] + "/" + xmlfiledata[i].TxnDate.Substring(0, 10).Split('-')[0];
                        BankMapping.BANK_ACK_TXNDATE = new CommonFunctions().GetStringToDateTime(AckTxnDate);

                        if (BankMapping.BANK_ACK_BILL_STATUS == "R")
                        {
                            BankMapping.BANK_ACK_REJECTION_CODE += xmlfiledata[i].RejectionCode == null ? null : (!string.IsNullOrEmpty(BankMapping.BANK_ACK_REJECTION_CODE)) ? "," + string.Join(",", xmlfiledata[i].RejectionCode) : string.Join(",", xmlfiledata[i].RejectionCode);
                            BankMapping.BANK_ACK_REJECTION_NARRATION += xmlfiledata[i].RejectionReason == null ? null : (!string.IsNullOrEmpty(BankMapping.BANK_ACK_REJECTION_NARRATION)) ? "," + string.Join(",", xmlfiledata[i].RejectionReason) : string.Join(",", xmlfiledata[i].RejectionReason);
                        }
                        else
                        {
                            BankMapping.BANK_ACK_REJECTION_CODE = null;
                            BankMapping.BANK_ACK_REJECTION_NARRATION = null;
                        }
                        dbContext.Entry(BankMapping).State = System.Data.Entity.EntityState.Modified;
                    }
                    else
                    {
                        REATLog("REAT Payment", ConfigurationManager.AppSettings["REATPaymentLog"].ToString(), "No Records to update for Payment acknowledgement", FileName);
                        return false;
                    }

                }
                if (BankMapping != null)
                {
                    dbContext.SaveChanges();
                    REATLog("REAT Payment", ConfigurationManager.AppSettings["REATBankLog"].ToString(), "Bank acknowledgement successful", FileName);
                }

                return true;
            }
            catch (Exception ex)
            {
                REATLog("REAT Bank", ConfigurationManager.AppSettings["REATBankLog"].ToString(), "Error on Bank acknowledgement", FileName);
                ErrorLog.LogError(ex, "REATDAL.SaveBankAcknowlegementDetails()");
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
      

        #endregion

        #region Add IFSC code

        public AddIfscCodeModel AddIFSCcode(Models.AddIfscCodeModel mIFSC, ref String oprnType)
        {
            PMGSYEntities db = new PMGSYEntities();
            PFMS_BANK_BRANCHMASTER modObj = new PFMS_BANK_BRANCHMASTER();
            AddIfscCodeModel custModel = new AddIfscCodeModel();
            var varstate = db.MASTER_STATE.Where(x => x.MAST_STATE_CODE == mIFSC.stateCode).Select(u => new { state_Name = u.MAST_STATE_NAME }).Single();
            try
            {
                using (var scope = new TransactionScope())
                {
                    if (db.PFMS_BANK_BRANCHMASTER.Any(x => x.IFSCCode == mIFSC.IfscCode))
                    {
                        //   status = false;
                        System.Diagnostics.Debug.WriteLine("Duplicate ifsc code is =" + mIFSC.IfscCode);

                        modObj = db.PFMS_BANK_BRANCHMASTER.Where(x => x.IFSCCode == mIFSC.IfscCode).FirstOrDefault<PFMS_BANK_BRANCHMASTER>();
                        if (modObj != null)
                        {
                            custModel.IfscCode = modObj.IFSCCode;
                            custModel.BankName = modObj.BankName;
                            custModel.BranchName = modObj.BranchName;
                            custModel.BankAddress = modObj.BranchAdress1;
                            custModel.City = modObj.City;
                            custModel.stateName = modObj.State;
                            oprnType = "D";  //D: Duplicate IFSC code

                        }

                    }
                    else
                    {
                        if (mIFSC != null)
                        {
                            if (db.PFMS_BANK_MASTER.Any(x => x.PFMS_BANK_NAME.Equals(mIFSC.BankName)))
                            {
                                modObj.BankName = mIFSC.BankName;
                                modObj.BranchName = mIFSC.BranchName;
                                modObj.BranchAdress1 = mIFSC.BankAddress;
                                modObj.BranchAdress2 = mIFSC.BankAddress;
                                modObj.City = mIFSC.City;
                                modObj.State = varstate.state_Name;
                                modObj.IFSCCode = mIFSC.IfscCode;
                                modObj.CBSStatusId = "CBS";

                                db.PFMS_BANK_BRANCHMASTER.Add(modObj);
                                db.SaveChanges();

                                custModel.IfscCode = mIFSC.IfscCode;
                                custModel.BankName = mIFSC.BankName;
                                custModel.BranchName = mIFSC.BranchName;
                                custModel.BankAddress = mIFSC.BankAddress;
                                custModel.City = mIFSC.City;
                                custModel.stateName = varstate.state_Name;
                                oprnType = "I";    //I:Insert operation
                            }
                            else
                            {

                                oprnType = "InvBank";
                            }

                        }

                        scope.Complete();

                    }
                }

            }
            catch (Exception ex)
            {

                System.Diagnostics.Debug.WriteLine("exception=" + ex.Data);
            }
            return custModel;
        }


        #endregion

        public bool ValidateREATContractor(int mastConId, int conAccountId)
        {
            try
            {
                int lgdStateCode = dbContext.OMMAS_LDG_STATE_MAPPING.Where(x => x.MAST_STATE_CODE == PMGSYSession.Current.StateCode).Select(x => x.MAST_STATE_LDG_CODE).FirstOrDefault();
                REAT_CONTRACTOR_DETAILS rcmodel = dbContext.REAT_CONTRACTOR_DETAILS.FirstOrDefault(x => x.MAST_CON_ID == mastConId && x.MAST_ACCOUNT_ID == conAccountId && x.REAT_CON_ID != null && x.reat_STATUS == "A");
                if (rcmodel != null)
                   return true;
                else
              return  false;
                
              }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "REATDAL.ValidateREATContractor");
                return false;
            }
        }

        public bool IsPaymentExists(long billId)
        {
            dbContext = new PMGSYEntities();
            try
            {
                return (dbContext.REAT_OMMAS_PAYMENT_MAPPING.Any(x => x.BILL_ID == billId));
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "REATDAL.IsPaymentExists()");
                return false;
            }
        }

        //public string GeneratePaymentXMLDAL(PFMSDownloadPaymentXMLViewModel model, out string fileName, out int runningCnt)
        //{
        //    string xmlString = string.Empty;
        //    string xmlHeader = string.Empty;
        //    string xmlBody = string.Empty;
          
        //    string genDate = string.Empty;
        //    int runningCount = 0;

        //    CommonFunctions comm = new CommonFunctions();
        //    try
        //    {
        //        dbContext = new PMGSYEntities();
        //        String xmlFileName = String.Empty;

        //        int AdminNdCode = PMGSYSession.Current.AdminNdCode;
        //        DateTime GenrationBillDate = Convert.ToDateTime(model.generationDate);
                
        //        model.agencyCode = dbContext.ADMIN_DEPARTMENT.Where(x => x.ADMIN_ND_CODE == PMGSYSession.Current.AdminNdCode).Select(x => x.MAST_AGENCY_CODE).FirstOrDefault();
        //        model.generationDate = comm.GetDateTimeToString(dbContext.ACC_BILL_MASTER.Where(x => x.BILL_ID == model.billId).Select(x => x.BILL_DATE).FirstOrDefault());

        //        //int BillId = 2990803;  //this billid for which payrequest is generated 
        //        Int64 BillId = model.billId;
        //        var outParam = new System.Data.Objects.ObjectParameter("XmlFileName", xmlFileName);
        //        var outParam1 = new System.Data.Objects.ObjectParameter("RunningCount", runningCount);
               

        //        var hdr = dbContext.REAT_PAYMENT_HEADER_XML_TEMP(PMGSYSession.Current.StateCode, model.agencyCode, PMGSYSession.Current.AdminNdCode, outParam, outParam1).ToList();
        //        var bdy = dbContext.REAT_GENERATE_PAYEMENT_XML_TEMP (PMGSYSession.Current.StateCode, model.agencyCode, PMGSYSession.Current.AdminNdCode, Convert.ToString(outParam.Value), Convert.ToInt32(outParam1.Value)).ToList();

              

        //        xmlHeader = string.Join("", hdr);
        //        xmlBody = string.Join("", bdy);
        //        xmlString = xmlHeader.Trim() + xmlBody.Trim();

        //        XmlSerializer XmlS = new XmlSerializer(xmlString.GetType());

        //        StringWriter sw = new PMGSY.DAL.PFMS.PFMSDAL1.StringWriterUtf8();
        //        XmlTextWriter tw = new XmlTextWriter(sw);
        //        tw.WriteProcessingInstruction("xml", "version=\"1.0\" encoding=\"UTF-8\" standalone=\"yes\"");

        //        XmlS.Serialize(tw, xmlString);

        //        xmlString = sw.ToString().Replace("&lt;", "<").Replace("&gt;", ">").Replace("<DbtBeneficiaries>", "").Replace("</DbtBeneficiaries>", "").Replace("<string>", @"<EATPaymentRequest xmlns=""http://cpsms.nic.in/EATPaymentRequest""><CstmrCdtTrfInitn>").Replace("</string>", "</CstmrCdtTrfInitn></EATPaymentRequest>");


        //        fileName = Convert.ToString(outParam.Value);
        //        runningCnt = Convert.ToInt32(outParam1.Value);

        //        return xmlString;
        //    }
        //    catch (Exception ex)
        //    {
        //        ErrorLog.LogError(ex, "REAT.GeneratePaymentXMLDAL()");
        //        fileName = "";
        //        runningCnt = 0;
        //        return string.Empty;
        //    }
        //}



        public string GeneratePaymentXMLDAL(PFMSDownloadPaymentXMLViewModel model, out string fileName, out int runningCnt)
        {
            string xmlString = string.Empty;
            string xmlHeader = string.Empty;
            string xmlBody = string.Empty;
         
            string genDate = string.Empty;
            int runningCount = 0;

            CommonFunctions comm = new CommonFunctions();
            try
            {
                dbContext = new PMGSYEntities();

                String xmlFileName = String.Empty;
                //int AdminNdCode = dbContext.ADMIN_DEPARTMENT.FirstOrDefault(s => s.MAST_STATE_CODE == model.stateCode && s.MAST_AGENCY_CODE == model.agencyCode && s.MAST_ND_TYPE == "S").ADMIN_ND_CODE;
                int AdminNdCode = PMGSYSession.Current.AdminNdCode;
                DateTime GenrationBillDate = Convert.ToDateTime(model.generationDate);       
                model.agencyCode = dbContext.ADMIN_DEPARTMENT.Where(x => x.ADMIN_ND_CODE == PMGSYSession.Current.AdminNdCode).Select(x => x.MAST_AGENCY_CODE).FirstOrDefault();
                model.generationDate = comm.GetDateTimeToString(dbContext.ACC_BILL_MASTER.Where(x => x.BILL_ID == model.billId).Select(x => x.BILL_DATE).FirstOrDefault());        
                Int64 BillId = model.billId;
                ObjectParameter outfilename = new System.Data.Entity.Core.Objects.ObjectParameter("XmlFileName", xmlFileName);
                ObjectParameter outParam = new System.Data.Entity.Core.Objects.ObjectParameter("RunningCount", runningCount);
                var hdr = dbContext.REAT_PAYMENT_HEADER_XML(model.stateCode, model.agencyCode, Convert.ToDateTime(model.generationDate), model.FileType, BillId, outfilename, outParam).ToList();
                var bdy = dbContext.REAT_GENERATE_PAYEMENT_XML(model.stateCode, model.agencyCode, Convert.ToDateTime(model.generationDate), BillId, model.FileType, Convert.ToString(outfilename.Value), Convert.ToInt32(outParam.Value)).ToList();
                xmlHeader = string.Join("", hdr);
                xmlBody = string.Join("", bdy);
                xmlString = xmlHeader.Trim() + xmlBody.Trim();
                XmlSerializer XmlS = new XmlSerializer(xmlString.GetType());
                StringWriter sw = new PMGSY.DAL.PFMS.PFMSDAL1.StringWriterUtf8();
                XmlTextWriter tw = new XmlTextWriter(sw);
                tw.WriteProcessingInstruction("xml", "version=\"1.0\" encoding=\"UTF-8\" standalone=\"yes\"");
                XmlS.Serialize(tw, xmlString);

                xmlString = sw.ToString().Replace("&lt;", "<").Replace("&gt;", ">").Replace("<DbtBeneficiaries>", "").Replace("</DbtBeneficiaries>", "").Replace("<string>", @"<EATPaymentRequest xmlns=""http://cpsms.nic.in/EATPaymentRequest""><CstmrCdtTrfInitn>").Replace("</string>", "</CstmrCdtTrfInitn></EATPaymentRequest>");


             //   xmlString = xmlString + xmlHeader.Trim() + xmlBody.Trim();
                fileName = outfilename.Value.ToString();            
                runningCnt = Convert.ToInt32(outParam.Value);
                return xmlString;
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "REAT.GeneratePaymentXMLDAL()");
                fileName = "";
                runningCnt = 0;
                return string.Empty;
            }
        }


        #region Insert Epayment Details
        public string InsertEpaymentMailDetailsREAT(Int64 bill_ID, String FileName, int mastConId, int conAccountId, int runningCount)
        {
            CommonFunctions commomFuncObj = new CommonFunctions();
            dbContext = new PMGSYEntities();
            try
            {
                using (var scope = new TransactionScope())
                {
                    int? parentNdVode = dbContext.ADMIN_DEPARTMENT.Where(b => b.ADMIN_ND_CODE == PMGSYSession.Current.AdminNdCode).Select(d => d.MAST_PARENT_ND_CODE).First();

                    //get payment master details
                    ACC_BILL_MASTER masterDetails = new ACC_BILL_MASTER();

                    masterDetails = dbContext.ACC_BILL_MASTER.Where(x => x.BILL_ID == bill_ID).First();

                    //get package details
                    List<Int32> lstBillDetails = new List<Int32>();

                    lstBillDetails = dbContext.ACC_BILL_DETAILS.Where(c => c.BILL_ID == bill_ID && c.CASH_CHQ == "Q" && c.CREDIT_DEBIT == "D" && c.IMS_PR_ROAD_CODE != null).Select(f => f.IMS_PR_ROAD_CODE.Value).Distinct().ToList<Int32>();

                    String Packages = String.Empty;

                    if (lstBillDetails != null && lstBillDetails.Count() != 0)
                    {
                        foreach (int item in lstBillDetails)
                        {
                            Packages = Packages + "," + dbContext.IMS_SANCTIONED_PROJECTS.Where(c => c.IMS_PR_ROAD_CODE == item).Select(x => x.IMS_PACKAGE_ID).First();
                        }

                        if (Packages != string.Empty)
                        {
                            if (Packages[0] == ',')
                            {
                                Packages = Packages.Substring(1);
                            }

                        }
                    }
                    //get the agreement details 
                    int? agreementCode = dbContext.ACC_BILL_DETAILS.Where(c => c.BILL_ID == bill_ID && c.CREDIT_DEBIT == "D" && c.CASH_CHQ == "Q").First().IMS_AGREEMENT_CODE;
                    String AgreementNo = masterDetails.FUND_TYPE == "M" ? (dbContext.MANE_IMS_CONTRACT.Where(c => c.MANE_CONTRACT_ID == (agreementCode.HasValue ? agreementCode.Value : -1)).Select(x => x.MANE_AGREEMENT_NUMBER).FirstOrDefault()) : (dbContext.TEND_AGREEMENT_MASTER.Where(c => c.TEND_AGREEMENT_CODE == (agreementCode.HasValue ? agreementCode.Value : -1)).Select(y => y.TEND_AGREEMENT_NUMBER).FirstOrDefault());

                    //get contractor details
                    //MASTER_CONTRACTOR_BANK con = new MASTER_CONTRACTOR_BANK();
                    REAT_CONTRACTOR_DETAILS con = null;
                    ADMIN_NO_BANK admNoBank = new ADMIN_NO_BANK();
                    //if (masterDetails.TXN_ID == 472 || masterDetails.TXN_ID == 415)
                    //{
                    //    admNoBank = dbContext.ADMIN_NO_BANK.Where(v => v.ADMIN_NO_OFFICER_CODE == masterDetails.ADMIN_NO_OFFICER_CODE && v.MAST_ACCOUNT_STATUS == "A").FirstOrDefault();
                    //}
                    //else
                    //{
                        //if (PMGSYSession.Current.FundType == "P")
                        //{
                            int lgdStateCode = dbContext.OMMAS_LDG_STATE_MAPPING.Where(x => x.MAST_STATE_CODE == PMGSYSession.Current.StateCode).Select(x => x.MAST_STATE_LDG_CODE).FirstOrDefault();

                            con = dbContext.REAT_CONTRACTOR_DETAILS.Where(v => v.MAST_CON_ID == masterDetails.MAST_CON_ID && v.REAT_CON_ID != null && v.MAST_ACCOUNT_ID == conAccountId && v.reat_STATUS == "A").FirstOrDefault();
                            if (con != null)
                            {
                                con.MAST_ACCOUNT_NUMBER = con.MAST_ACCOUNT_NUMBER;
                                con.MAST_ACCOUNT_ID = con.MAST_ACCOUNT_ID;
                                con.MAST_IFSC_CODE = con.MAST_IFSC_CODE;
                                con.MAST_BANK_NAME = con.MAST_BANK_NAME;
                            }
                        //}
                    //}

                    //get authorized bank details
                    ACC_BANK_DETAILS bankDetails = new ACC_BANK_DETAILS();

                    if (parentNdVode.HasValue)
                    {
                        bankDetails = dbContext.ACC_BANK_DETAILS.Where(f => f.ADMIN_ND_CODE == parentNdVode.Value && f.FUND_TYPE == PMGSYSession.Current.FundType && f.BANK_ACC_STATUS == true && f.ACCOUNT_TYPE == "S").First();
                    }

                    //get state /district information
                    MASTER_DISTRICT district = new MASTER_DISTRICT();

                    district = dbContext.MASTER_DISTRICT.Where(v => v.MAST_DISTRICT_CODE == PMGSYSession.Current.DistrictCode).First();

                    int stateCode = district.MAST_STATE_CODE;

                    string StateName = dbContext.MASTER_STATE.Where(v => v.MAST_STATE_CODE == stateCode).Select(c => c.MAST_STATE_NAME).First();
                    string PiuName = dbContext.ADMIN_DEPARTMENT.Where(v => v.ADMIN_ND_CODE == PMGSYSession.Current.AdminNdCode).Select(c => c.ADMIN_ND_NAME).First();

                    ACC_EPAY_MAIL_MASTER EpayModel = new ACC_EPAY_MAIL_MASTER();

                    long maxEPAY_ID = 0;

                    if (dbContext.ACC_EPAY_MAIL_MASTER.Any())
                    {
                        maxEPAY_ID = dbContext.ACC_EPAY_MAIL_MASTER.Max(c => c.EPAY_ID);
                    }

                    maxEPAY_ID = maxEPAY_ID + 1;

                    EpayModel.EPAY_ID = maxEPAY_ID;

                    EpayModel.BILL_ID = bill_ID;

                    EpayModel.EPAY_NO = masterDetails.CHQ_NO;

                    EpayModel.EPAY_MONTH = (byte)masterDetails.BILL_MONTH;

                    EpayModel.EPAY_YEAR = masterDetails.BILL_YEAR;

                    if (masterDetails.CHQ_DATE.HasValue)
                    {
                        EpayModel.EPAY_DATE = masterDetails.CHQ_DATE.Value;
                    }

                    EpayModel.EMAIL_FROM = "omms.pmgsy@nic.in";

                    EpayModel.EMAIL_TO = bankDetails.BANK_EMAIL;

                    EpayModel.EMAIL_SUBJECT = " An Epayment transaction is made by DPIU of " + PiuName + " ( " + district.MAST_DISTRICT_NAME + " ) of " + StateName + "on https://omms.nic.in,Epayment No: " + masterDetails.CHQ_NO;

                    EpayModel.EMAIL_CC = "";

                    //EpayModel.EMAIL_CC = dbContext.ADMIN_DEPARTMENT.Where(m => m.MAST_STATE_CODE == PMGSYSession.Current.StateCode && m.MAST_ND_TYPE == "S").Select(m=>m.ADMIN_ND_EMAIL).FirstOrDefault();

                    EpayModel.EMAIL_BCC = "omms.pmgsy@nic.in";

                    EpayModel.EMAIL_SENT_DATE = DateTime.Now;

                    EpayModel.IS_EPAY_VALID = true;

                    EpayModel.REQUEST_IP = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];

                    EpayModel.ADMIN_ND_CODE = PMGSYSession.Current.AdminNdCode;

                    EpayModel.LVL_ID = (byte)PMGSYSession.Current.LevelId;

                    EpayModel.EPAY_EREMITTANCE = "E";

                    EpayModel.DEPT_BANK_ACC_NO = String.Empty;

                    EpayModel.DPIU_TAN_NO = String.Empty;

                    //File Name Added By Abhishek kamble for Resend/Reject mail Details 25 Sep 2014
                    EpayModel.FILE_NAME = FileName;

                    //Added By Abhishek kamble 29-nov-2013
                    EpayModel.USERID = PMGSYSession.Current.UserId;
                    EpayModel.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];

                    dbContext.ACC_EPAY_MAIL_MASTER.Add(EpayModel);

                    //insert the details into [ACC_EPAY_MAIL_DETAILS]

                    long maxDetail_ID = 0;

                    if (dbContext.ACC_EPAY_MAIL_MASTER.Any())
                    {
                        maxDetail_ID = dbContext.ACC_EPAY_MAIL_DETAILS.Max(c => c.DETAIL_ID);
                    }

                    ACC_EPAY_MAIL_DETAILS EpayDetailsModel = new ACC_EPAY_MAIL_DETAILS();

                    maxDetail_ID = maxDetail_ID + 1;

                    EpayDetailsModel.DETAIL_ID = maxDetail_ID;

                    EpayDetailsModel.EPAY_ID = maxEPAY_ID;

                    EpayDetailsModel.BANK_ACC_NO = bankDetails.BANK_ACC_NO;

                    EpayDetailsModel.BILL_NO = masterDetails.BILL_NO;

                    EpayDetailsModel.BILL_DATE = masterDetails.BILL_DATE;

                    EpayDetailsModel.AGREEMENT_NO = AgreementNo;

                    EpayDetailsModel.PKG_NO = Packages;

                    EpayDetailsModel.CON_NAME = masterDetails.PAYEE_NAME;


                  
                    //if (masterDetails.TXN_ID == 472 || masterDetails.TXN_ID == 415)
                    //{
                    //    EpayDetailsModel.CON_ACC_NO = (admNoBank == null ? "" : admNoBank.MAST_ACCOUNT_NUMBER);
                    //    EpayDetailsModel.CON_BANK_NAME = (admNoBank == null ? "" : admNoBank.MAST_BANK_NAME);
                    //    EpayDetailsModel.CON_BANK_IFS_CODE = (admNoBank == null ? "" : admNoBank.MAST_IFSC_CODE);
                    //}
                    //else
                    //{
                        EpayDetailsModel.CON_ACC_NO = (con == null ? "" : con.MAST_ACCOUNT_NUMBER);
                        EpayDetailsModel.CON_BANK_NAME = (con == null ? "" : con.MAST_BANK_NAME);
                        EpayDetailsModel.CON_BANK_IFS_CODE = (con == null ? "" : con.MAST_IFSC_CODE);
                    //}
                   
                    EpayDetailsModel.EPAY_AMOUNT = masterDetails.CHQ_AMOUNT;


                    EpayDetailsModel.ADMIN_ND_CODE = PMGSYSession.Current.AdminNdCode;

                    EpayDetailsModel.LVL_ID = (byte)PMGSYSession.Current.LevelId;

                    EpayDetailsModel.CON_PAN_NO = dbContext.MASTER_CONTRACTOR.Where(c => c.MAST_CON_ID == masterDetails.MAST_CON_ID).Select(d => d.MAST_CON_PAN).FirstOrDefault();

                   
                    EpayDetailsModel.USERID = PMGSYSession.Current.UserId;
                    EpayDetailsModel.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];

                    dbContext.ACC_EPAY_MAIL_DETAILS.Add(EpayDetailsModel);

                    
                    ACC_BILL_MASTER acc_bill_master = dbContext.ACC_BILL_MASTER.Find(bill_ID);

                    if (acc_bill_master.CHQ_EPAY == "E")
                    {
                       
                        acc_bill_master.BILL_FINALIZED = "Y";
                      
                    }
                    else
                    {
                        acc_bill_master.BILL_FINALIZED = "Y";
                    }
                    acc_bill_master.ACTION_REQUIRED = "N";

                   
                    acc_bill_master.USERID = PMGSYSession.Current.UserId;
                    acc_bill_master.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];

                    dbContext.Entry(acc_bill_master).State = System.Data.Entity.EntityState.Modified;

                    
                    string FName = FileName.Trim().Contains(".xml") ? FileName.Trim().Substring(0, FileName.Trim().IndexOf('.')) : FileName.Trim();
                    if (dbContext.REAT_OMMAS_PAYMENT_MAPPING .Any(z => z.PAYMENT_REQ_FILENAME.Trim() == FName.Trim() || z.PAYMENT_REQ_FILENAME.Trim() == FName.Trim() + ".xml"))
                    {
                        return "-1";
                    }

                    #region Insert PFMS Details


                    REAT_OMMAS_PAYMENT_MAPPING rt_ommas_payment_mapping = new REAT_OMMAS_PAYMENT_MAPPING();

                    rt_ommas_payment_mapping.BILL_ID = bill_ID;
                    rt_ommas_payment_mapping.BATCH_ID = "CP0037" + DateTime.Now.Day.ToString("D2") + DateTime.Now.Month.ToString("D2") + DateTime.Now.Year.ToString().Substring(2, 2) + runningCount.ToString("D4");
                    rt_ommas_payment_mapping.BILL_DATE = acc_bill_master.BILL_DATE;
                    rt_ommas_payment_mapping.PAYMENT_REQ_FILENAME = FileName.Trim();
                    rt_ommas_payment_mapping.PAYMENT_REQ_FILE_GEN_DATE = DateTime.Now;

                    dbContext.REAT_OMMAS_PAYMENT_MAPPING.Add(rt_ommas_payment_mapping);
                    #endregion

                    dbContext.SaveChanges();

                    scope.Complete();

                    return "1";

                }
            }
            catch (DbEntityValidationException e)
            {
                //Elmah.ErrorSignal.FromCurrentContext().Raise(e, HttpContext.Current);
                //ErrorLog.LogError(e, "PFMSDAL.InsertEpaymentMailDetailsPFMS(DbEntityValidationException ex)");
                ModelStateDictionary modelstate = new ModelStateDictionary();

                foreach (var eve in e.EntityValidationErrors)
                {
                    foreach (var ve in eve.ValidationErrors)
                    {
                        //modelstate.AddModelError(ve.ErrorMessage, ve.ErrorMessage);
                        using (StreamWriter sw = System.IO.File.AppendText(System.IO.Path.Combine("D:\\ErrorLog\\OMMASErrorLog_" + System.DateTime.Now.Date.ToString("dd_MM_yyyy") + ".txt", "")))
                        {
                            sw.WriteLine("Date :" + DateTime.Now.ToString() + " UserName -" + PMGSYSession.Current.UserName);
                            sw.WriteLine("Method : " + "REATDAL.InsertEpaymentMailDetailsPFMS(DbEntityValidationException ex)");
                            sw.WriteLine("Exception : " + e.ToString());
                            sw.WriteLine("Exception Message : " + ve.ErrorMessage);
                            sw.WriteLine("____________________________________________________");
                            sw.Close();
                        }
                    }
                }

                //return new CommonFunctions().FormatErrorMessage(modelstate);



                throw new Exception("Error while Finalizing Epayment");
            }
            catch (DbUpdateException ex)
            {
                //Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                ErrorLog.LogError(ex, "REATDAL.InsertEpaymentMailDetailsPFMS(DbUpdateException ex)");

                throw new Exception("Error while Finalizing Epayment");
            }
            catch (OptimisticConcurrencyException ex)
            {
                //Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                ErrorLog.LogError(ex, "REATDAL.InsertEpaymentMailDetailsPFMS(OptimisticConcurrencyException ex)");
                throw new Exception("Error while Finalizing Epayment");
            }
            catch (Exception ex)
            {
                //Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                ErrorLog.LogError(ex, "REATDAL.InsertEpaymentMailDetailsPFMS()");
                throw new Exception("Error while Finalizing Epayment");
            }
            finally
            {
                dbContext.Dispose();
            }

        }
        #endregion

        public void REATLog(string module, string logPath, string message, string fileName)
        {
            try
            {
                if (!Directory.Exists(logPath))
                {
                    Directory.CreateDirectory(logPath);
                }
                using (System.IO.StreamWriter sw = System.IO.File.AppendText(System.IO.Path.Combine(logPath + "\\REAT" + module + "Log_" + System.DateTime.Now.Date.ToString("dd_MM_yyyy") + ".txt", "")))
                {
                    sw.WriteLine("---------------------------------------------------------------------------------------");
                    sw.WriteLine("Date : " + DateTime.Now.ToString());
                    sw.WriteLine("FileName : " + fileName);
                    sw.WriteLine("status : " + message);
                    sw.WriteLine("---------------------------------------------------------------------------------------");
                    sw.Close();
                }
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "PFMS.PFMSLog");
            }
        }


        public string SaveDSCREAT(string prmfileName , int prmAdminNdCode , string opType)
        {

            dbContext = new PMGSY.Models.PMGSYEntities();
            try
            {
                ADMIN_NODAL_OFFICERS officer = dbContext.ADMIN_NODAL_OFFICERS.FirstOrDefault(x => x.ADMIN_MODULE == "A" && x.ADMIN_NO_DESIGNATION == 26 && x.ADMIN_ACTIVE_STATUS == "Y" && x.ADMIN_ND_CODE == prmAdminNdCode);  //bill admin code used b4

                REAT_DATA_SEND_DETAILS Datasenddetails = new REAT_DATA_SEND_DETAILS();
                Datasenddetails.FILE_ID = dbContext.REAT_DATA_SEND_DETAILS.Any() ? dbContext.REAT_DATA_SEND_DETAILS.Max(s => s.FILE_ID) + 1 : 1;
                Datasenddetails.ADMIN_ND_CODE = prmAdminNdCode;
                Datasenddetails.FUND_TYPE = "P";
                Datasenddetails.GENERATED_FILE_NAME = prmfileName.Trim();//+ ".xml";
                Datasenddetails.FILE_GENERATION_DATE = DateTime.Now;
                Datasenddetails.GENERATED_FILE_PATH = Path.Combine(ConfigurationManager.AppSettings["REATDSCFilePath"].ToString(), prmfileName.Trim());
                Datasenddetails.FILE_TYPE = "D";
                Datasenddetails.RESPONSE_RECEIVED_DATE = null;
                Datasenddetails.RECEIVED_FILE_NAME = null;
                Datasenddetails.RECEIVED_FILE_PATH = null;
                Datasenddetails.ERR_RECEIVED_RESPONSE = null;

                Datasenddetails.USER_ID = PMGSYSession.Current.UserId;
                Datasenddetails.IPADDR = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];

                dbContext.REAT_DATA_SEND_DETAILS.Add(Datasenddetails);

                REAT_OMMAS_DSC_MAPPING DscMapping = new REAT_OMMAS_DSC_MAPPING();
                DscMapping.ID = dbContext.REAT_OMMAS_DSC_MAPPING.Any() ? dbContext.REAT_OMMAS_DSC_MAPPING.Max(s => s.ID) + 1 : 1;
                DscMapping.FILE_ID = Datasenddetails.FILE_ID;
                DscMapping.ADMIN_ND_CODE = prmAdminNdCode;//PMGSYSession.Current.AdminNdCode;
                DscMapping.ADMIN_NO_OFFICER_CODE = officer.ADMIN_NO_OFFICER_CODE;
                DscMapping.DSC_REQ_FILENAME = prmfileName; //+ ".xml";
                DscMapping.IS_ACTIVE = true;
                if (opType.Equals("A"))
                {
                    DscMapping.FUND_TYPE = "P"; // "P" for new dsc 
                }
                else if (opType.Equals("D"))
                {
                     DscMapping.FUND_TYPE = "D"; //FOR Dsc Deletion
                }

                    dbContext.REAT_OMMAS_DSC_MAPPING.Add(DscMapping);
                dbContext.SaveChanges();
                return "1"; 
            }
            catch (Exception ex)
            {
                ex.Message.ToString();
                return "0" ;
            }
        }





        public REATOpeningBalanceViewModel GetAccBankDetails()   //Added by Aditi on 24 April 2020
        {
            dbContext = new PMGSYEntities();
            try
            {
                REATOpeningBalanceViewModel model = new REATOpeningBalanceViewModel();
                ACC_BANK_DETAILS BankDetails = dbContext.ACC_BANK_DETAILS.FirstOrDefault(s => s.FUND_TYPE == "P" && s.ADMIN_ND_CODE == PMGSYSession.Current.AdminNdCode && s.BANK_ACC_STATUS == true && s.ACCOUNT_TYPE == "S");

                var latestDate = dbContext.REAT_OB_DETAILS.Where(s => s.ADMIN_ND_CODE == PMGSYSession.Current.AdminNdCode).Max(s => s.OB_FILE_DATE);

                REAT_OB_DETAILS ObDetails = dbContext.REAT_OB_DETAILS.FirstOrDefault(s => s.ADMIN_ND_CODE == PMGSYSession.Current.AdminNdCode && s.OB_FILE_DATE == latestDate);

                if (BankDetails != null && ObDetails == null)
                {
                    model.BANK_NAME = BankDetails.BANK_NAME;
                    model.MAST_IFSC_CODE = BankDetails.MAST_IFSC_CODE;
                    model.BANK_BRANCH = BankDetails.BANK_BRANCH;
                    model.BANK_ACC_NO = BankDetails.BANK_ACC_NO;
                    model.BANK_ADDRESS1 = BankDetails.BANK_ADDRESS1;
                    model.BANK_PHONE1 = BankDetails.BANK_PHONE1;
                    model.BANK_EMAIL = BankDetails.BANK_EMAIL;
                    model.BANK_ACC_OPEN_DATE = BankDetails.BANK_ACC_OPEN_DATE;
                }
                else if (BankDetails != null && ObDetails != null)
                {
                    model.BANK_NAME = BankDetails.BANK_NAME;
                    model.MAST_IFSC_CODE = BankDetails.MAST_IFSC_CODE;
                    model.BANK_BRANCH = BankDetails.BANK_BRANCH;
                    model.BANK_ACC_NO = BankDetails.BANK_ACC_NO;
                    model.BANK_ADDRESS1 = BankDetails.BANK_ADDRESS1;
                    model.BANK_PHONE1 = BankDetails.BANK_PHONE1;
                    model.BANK_EMAIL = BankDetails.BANK_EMAIL;
                    model.BANK_ACC_OPEN_DATE = BankDetails.BANK_ACC_OPEN_DATE;
                    model.OB_STATUS = ObDetails.OB_STATUS;
                    decimal obAmount = ObDetails.OB_AMOUNT;
                    string obAmt = string.Format("{0:#,#.00}", obAmount);
                    model.OB_BALANCE = obAmt;
                    model.OB_DATE = ObDetails.OB_DATE;
                }
                else
                {
                    REATLog("REAT OB", ConfigurationManager.AppSettings["REATOBLog"].ToString(), "No records found to populate", "");
                    return null;
                }
                return model;
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "REAT.GetAccBankDetails()");
                return null;
            }
        }






        public string GenerateXMLOpeningBalanceDAL(double OpBalance, DateTime OpDate, out string fileName)
        {
            try
            {

                dbContext = new PMGSYEntities();
                String FileName = string.Empty;
                ADMIN_DEPARTMENT AdminDeptPIU = dbContext.ADMIN_DEPARTMENT.FirstOrDefault(s => s.ADMIN_ND_CODE == PMGSYSession.Current.AdminNdCode);
                ACC_BANK_DETAILS BankDetails = dbContext.ACC_BANK_DETAILS.FirstOrDefault(s => s.FUND_TYPE == "P" && s.ADMIN_ND_CODE == AdminDeptPIU.MAST_PARENT_ND_CODE && s.BANK_ACC_STATUS == true && s.ACCOUNT_TYPE == "S");
                REAT_INITIATING_PARTY_MASTER initparty = dbContext.REAT_INITIATING_PARTY_MASTER.FirstOrDefault(s => s.ADMIN_ND_CODE == AdminDeptPIU.MAST_PARENT_ND_CODE);

                int runningCount = dbContext.REAT_DATA_SEND_DETAILS.Any(s => s.FUND_TYPE == PMGSYSession.Current.FundType && s.FILE_TYPE == "O")
                                   ? (dbContext.REAT_DATA_SEND_DETAILS.Where(s => s.FUND_TYPE == PMGSYSession.Current.FundType && s.FILE_TYPE == "O" && EntityFunctions.TruncateTime(s.FILE_GENERATION_DATE) == DateTime.Today).Count()) + 1
                                   : 1;
                #region XML Generation

                FileName = "0037OPBUPDREQ" + DateTime.Now.ToString("ddMMyyyy") + "" + runningCount.ToString();
                StringBuilder startString = new StringBuilder("<EATOpeningBalance  xmlns=\"http://cpsms.nic.in/EATOpeningBalance\"><CstmrCdtTrfInitn>");

                XDocument doc = new XDocument(new XElement("GrpHdr",
                                                    new XElement("MsgId", "0037OPBUPDREQ" + DateTime.Now.ToString("ddMMyyyy") + "" + runningCount.ToString()),
                                                    new XElement("Src", "0037"),

                                                     new XElement("InitgPty",
                                                             new XElement("PrTry",
                                                                    new XElement("Id", initparty.REAT_INIT_PARTY_UNIQUE_CODE)
                                                                 )),
                                                     new XElement("PmtInf",
                                                              new XElement("PmtInfId", "P0037" + DateTime.Now.ToString("ddMMyyyy") + "" + runningCount.ToString()),
                                                              new XElement("PmtInfDt", DateTime.Now.ToString("yyyy-MM-dd"))
                                                                  ),
                                                     new XElement("OrgId",
                                                          new XElement("SchmeCd", initparty.SCHEME_CODE)
                                                                )
                                                    ));
                string xmlstring = doc.ToString();
                int OBId = !(dbContext.REAT_OB_DETAILS.Any()) ? 1 : (dbContext.REAT_OB_DETAILS.Max(x => x.OB_ID) + 1);
                string SrcSysTranNo = PMGSYSession.Current.AdminNdCode + "/" + PMGSYSession.Current.FundType + "/" + OBId;

                XDocument middleString = new XDocument(new XElement("CdtTrfTxInf",
                                                         new XElement("EndtoEndId", SrcSysTranNo),
                                                         new XElement("CdtrAcct",
                                                              new XElement("Id",
                                                                   new XElement("BBAN", BankDetails.BANK_ACC_NO)),
                                                              new XElement("FinInstnId",
                                                                   new XElement("BrnchId", BankDetails.MAST_IFSC_CODE))),
                                                          new XElement("Amt", OpBalance.ToString()),
                                                         new XElement("CreDtTm", OpDate.ToString("yyyy-MM-dd")),
                                                          new XElement("PmtTxDtls",
                                                                  new XElement("Txsts", "1"),
                                                                  new XElement("TxTp", "B")),
                    /* new XElement("RmtInf",
                         new XElement("RltdInf",
                             new XElement("Rem", "Opening Balance")))),*/
                    /*new XElement("AcctngEntty",
                         new XElement("Id", "Project Id")),*/
                                                          new XElement("TxEffct",
                                                               new XElement("Stts", "1"))
                    /*new XElement("TPymntHd",
                          new XElement("Id", "Scheme Component ID"),
                           new XElement("Amt", "Component Amount"))*/
                                                             ));

                String endString = "</CstmrCdtTrfInitn></EATOpeningBalance>";

                startString.Append(doc.ToString());
                startString.Append(middleString.ToString());
                startString.Append(endString);

                XDocument finaldoc = XDocument.Parse(startString.ToString());
                fileName = FileName.Trim() + ".xml";
                #endregion
                finaldoc.Save(ConfigurationManager.AppSettings["REATOBXmlFilePath"] + fileName);
                //finaldoc.Save(@"D:\OmmasImages\REAT\OpeningBalance\"+fileName);

                #region Call to Stored Procedure
                var prmNDCode = new SqlParameter("@prmNDCode", SqlDbType.Int);
                prmNDCode.Value = PMGSYSession.Current.AdminNdCode;
                var prmFundType = new SqlParameter("@prmFundType", SqlDbType.Char);
                prmFundType.Value = PMGSYSession.Current.FundType;
                var prmBatch = new SqlParameter("@prmBatch", SqlDbType.VarChar);
                prmBatch.Value = "P0037" + DateTime.Now.ToString("ddMMyyyy") + "" + runningCount.ToString();
                var prmFileName = new SqlParameter("@prmFileName", SqlDbType.VarChar);
                prmFileName.Value = fileName;
                var prmOBAmount = new SqlParameter("@prmOBAmount", SqlDbType.Decimal);
                prmOBAmount.Value = OpBalance;
                var prmOBDate = new SqlParameter("@prmOBDate", SqlDbType.Date);
                prmOBDate.Value = OpDate.Date;
                var prmUserID = new SqlParameter("@prmUserID", SqlDbType.Int);
                prmUserID.Value = PMGSYSession.Current.UserId;
                var prmIPADDR = new SqlParameter("@prmIPADDR", SqlDbType.VarChar);
                prmIPADDR.Value = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];

                using (TransactionScope ts = new TransactionScope())
                {
                    try
                    {
                        var Records = dbContext.Database.SqlQuery<Int32>("exec omms.REAT_INSERT_OB_DETAILS @prmNDCode,@prmFundType,@prmBatch,@prmFileName,@prmOBAmount,@prmOBDate,@prmUserID,@prmIPADDR", prmNDCode, prmFundType, prmBatch, prmFileName, prmOBAmount, prmOBDate, prmUserID, prmIPADDR);
                        if (Records.Count() < 1)
                        {
                            REATLog("REAT OB", ConfigurationManager.AppSettings["REATOBLog"].ToString(), "Error occured while inserting in REAT_OB_DETAILS", FileName);
                            return String.Empty;
                        }
                    }
                    catch (Exception ex)
                    {
                        ErrorLog.LogError(ex, "REATDAL.GenerateXMLOpeningBalanceDAL");
                        return String.Empty;
                    }
                    ts.Complete();
                }
                #endregion
                return startString.ToString();
            }
            catch (OptimisticConcurrencyException ex)
            {
                ErrorLog.LogError(ex, "REATDAL.GenerateXMLOpeningBalanceDAL.OptimisticConcurrencyException");
                fileName = "";
                return String.Empty;
            }
            catch (UpdateException ex)
            {
                ErrorLog.LogError(ex, "REATDAL.GenerateXMLOpeningBalanceDAL.UpdateException");
                fileName = "";
                return String.Empty;
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "REATDAL.GenerateXMLOpeningBalanceDAL");
                fileName = "";
                return String.Empty;
            }
            finally
            {
                if (dbContext != null)
                {
                    dbContext.Dispose();
                }
            }
        }
        #region DSC Approve

        public int? PopulatePIUbystatecodeDAL(int Statecode, int Agencycode) // Added by Priyanka 14-05-2020
        {

            dbContext = new PMGSYEntities();
            try
            {

                return dbContext.ADMIN_DEPARTMENT.Where(m => m.MAST_STATE_CODE == Statecode && m.MAST_AGENCY_CODE == Agencycode).Select(s => s.MAST_PARENT_ND_CODE).FirstOrDefault();


            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "REAT.PopulatePIUbystatecodeDAL()");
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

        public List<SelectListItem> PopulateSRRDA(int Statecode, int Agencycode)
        {
            List<SelectListItem> getsrrda = new List<SelectListItem>();

            dbContext = new PMGSYEntities();

            try
            {

                var result = dbContext.ADMIN_DEPARTMENT.Where(m => m.MAST_STATE_CODE == Statecode && m.MAST_AGENCY_CODE == Agencycode && m.MAST_ND_TYPE == "S").Select(s => new ApproveDSCViewModel { ADMIN_ND_NAME = s.ADMIN_ND_NAME, MAST_PARENT_ND_CODE = s.MAST_PARENT_ND_CODE }).ToList();

                getsrrda.Add(new SelectListItem { Value = "0", Text = "---Select SRRDA---" });
                foreach (var item in result)
                {
                    getsrrda.Add(new SelectListItem { Value = item.MAST_PARENT_ND_CODE.ToString(), Text = item.ADMIN_ND_NAME });
                }
                return getsrrda;
            }

            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "REAT.PopulateSRRDA()");
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

        public ApproveDSCViewModel GetAuthoriseSignatoryDetailsDAL(int admin_ND_code)
        {

            dbContext = new PMGSYEntities();
            try
            {
                ApproveDSCViewModel modelobj = new ApproveDSCViewModel();

                List<ADMIN_NODAL_OFFICERS> adminNoOfficers = dbContext.ADMIN_NODAL_OFFICERS.Where(ano => ano.ADMIN_MODULE == "A" && ano.ADMIN_NO_DESIGNATION == 26 && ano.ADMIN_ACTIVE_STATUS == "Y" && ano.ADMIN_ND_CODE == admin_ND_code).ToList(); //.Select(ad => ad.ADMIN_NO_OFFICER_CODE).ToList();
                List<REAT_OMMAS_DSC_MAPPING> dscMappingList = new List<REAT_OMMAS_DSC_MAPPING>();
                adminNoOfficers.ForEach(ano =>
                {
                    var result = dbContext.REAT_OMMAS_DSC_MAPPING.Where(dsc => dsc.ADMIN_ND_CODE == ano.ADMIN_ND_CODE && dsc.ADMIN_NO_OFFICER_CODE == ano.ADMIN_NO_OFFICER_CODE && dsc.FUND_TYPE == "P").OrderByDescending(s => s.FILE_ID).FirstOrDefault();
                    if (result != null)
                    {
                        dscMappingList.Add(result);
                    }
                });
                var record = dscMappingList.OrderByDescending(list => list.FILE_ID).FirstOrDefault();

                modelobj.ACK_DSC_STATUS = record.ACK_DSC_STATUS;
                modelobj.REJECTION_NARRATION = record.REJECTION_NARRATION;
                modelobj.AdminNDCode = record.ADMIN_ND_CODE;
                modelobj.ADMIN_NO_OFFICER_CODE = record.ADMIN_NO_OFFICER_CODE;
                modelobj.FileID = record.FILE_ID;
                modelobj.lstAgency = new List<SelectListItem>();
                modelobj.lstPIU = new List<SelectListItem>();
                modelobj.lstState = new List<SelectListItem>();
                //modelobj.REJECTION_CODE = record.REJECTION_CODE;
                //added by ajinkya
                String[] array = record.REJECTION_CODE.Split(',');

                for (int i = 0; i < array.Length; i++)
                {
                    if (array[i] == "CDE0029")
                    {
                        modelobj.REJECTION_CODE = "CDE0029";
                    }
                    else
                    {
                        modelobj.REJECTION_CODE = record.REJECTION_CODE;
                        break;
                    }
                }
                modelobj.Authorised_Signatory_Name = adminNoOfficers.Where(s => s.ADMIN_ND_CODE == record.ADMIN_ND_CODE && s.ADMIN_NO_OFFICER_CODE == record.ADMIN_NO_OFFICER_CODE).Select(n => n.ADMIN_NO_FNAME + " " + n.ADMIN_NO_MNAME + " " + n.ADMIN_NO_LNAME).FirstOrDefault();

                return modelobj;

            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "REAT.GetAuthoriseSignatoryDetails()");
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


        //public ApproveDSCViewModel GetAuthoriseSignatoryDetailsDAL(int admin_ND_code)
        //{

        //    dbContext = new PMGSYEntities();
        //    try
        //    {
        //        ApproveDSCViewModel modelobj = new ApproveDSCViewModel();

        //        List<ADMIN_NODAL_OFFICERS> adminNoOfficers = dbContext.ADMIN_NODAL_OFFICERS.Where(ano => ano.ADMIN_MODULE == "A" && ano.ADMIN_NO_DESIGNATION == 26 && ano.ADMIN_ACTIVE_STATUS == "Y" && ano.ADMIN_ND_CODE == admin_ND_code).ToList(); //.Select(ad => ad.ADMIN_NO_OFFICER_CODE).ToList();
        //        List<REAT_OMMAS_DSC_MAPPING> dscMappingList = new List<REAT_OMMAS_DSC_MAPPING>();
        //        adminNoOfficers.ForEach(ano =>
        //        {
        //            var result = dbContext.REAT_OMMAS_DSC_MAPPING.Where(dsc => dsc.ADMIN_ND_CODE == ano.ADMIN_ND_CODE && dsc.ADMIN_NO_OFFICER_CODE == ano.ADMIN_NO_OFFICER_CODE).OrderByDescending(s => s.FILE_ID).FirstOrDefault();
        //            if (result != null)
        //            {
        //                dscMappingList.Add(result);
        //            }
        //        });
        //        var record = dscMappingList.OrderByDescending(list => list.FILE_ID).FirstOrDefault();

        //        modelobj.ACK_DSC_STATUS = record.ACK_DSC_STATUS;
        //        modelobj.REJECTION_NARRATION = record.REJECTION_NARRATION;
        //        modelobj.AdminNDCode = record.ADMIN_ND_CODE;
        //        modelobj.ADMIN_NO_OFFICER_CODE = record.ADMIN_NO_OFFICER_CODE;
        //        modelobj.FileID = record.FILE_ID;
        //        modelobj.lstAgency = new List<SelectListItem>();
        //        modelobj.lstPIU = new List<SelectListItem>();
        //        modelobj.lstState = new List<SelectListItem>();
        //        modelobj.Authorised_Signatory_Name = adminNoOfficers.Where(s => s.ADMIN_ND_CODE == record.ADMIN_ND_CODE && s.ADMIN_NO_OFFICER_CODE == record.ADMIN_NO_OFFICER_CODE).Select(n => n.ADMIN_NO_FNAME + " " + n.ADMIN_NO_MNAME + " " + n.ADMIN_NO_LNAME).FirstOrDefault();

        //        return modelobj;

        //    }
        //    catch (Exception ex)
        //    {
        //        ErrorLog.LogError(ex, "REAT.GetAuthoriseSignatoryDetails()");
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



        public String ApproveDSCDAL(int ADMIN_NO_OFFICER_CODE, int ADMINNDCODE, long FileID)
        {

            dbContext = new PMGSYEntities();
            try
            {

                int updatedrecord = 0;
                List<REAT_OMMAS_DSC_MAPPING> DSCResultList = new List<REAT_OMMAS_DSC_MAPPING>();

                var cerdetails = dbContext.ACC_CERTIFICATE_DETAILS.Where(c => c.ADMIN_NO_OFFICER_CODE == ADMIN_NO_OFFICER_CODE).FirstOrDefault();
                if (cerdetails != null)
                {
                    //List<int> certList = dbContext.ACC_CERTIFICATE_DETAILS.Where(s => s.ADMIN_NO_OFFICER_CODE != ADMIN_NO_OFFICER_CODE && s.CERTIFICATE == cerdetails.CERTIFICATE
                    //                                                       && s.CERTIFICATE_CHAIN == cerdetails.CERTIFICATE_CHAIN && s.PUBLIC_KEY == cerdetails.PUBLIC_KEY).
                    //                                                       Select(x => x.ADMIN_NO_OFFICER_CODE).Distinct().ToList();
                    //List<int> adminnooffcode = dbContext.ACC_CERTIFICATE_DETAILS_SHADOW.Where(s => s.ADMIN_NO_OFFICER_CODE != ADMIN_NO_OFFICER_CODE && s.CERTIFICATE == cerdetails.CERTIFICATE
                    //                                                       && s.CERTIFICATE_CHAIN == cerdetails.CERTIFICATE_CHAIN && s.PUBLIC_KEY == cerdetails.PUBLIC_KEY).
                    //                                                       Select(x => x.ADMIN_NO_OFFICER_CODE).Distinct().ToList();

                    //certList.AddRange(adminnooffcode);

                    //certList.Distinct().ToList().ForEach(adn =>
                    //{
                    //    var result = dbContext.REAT_OMMAS_DSC_MAPPING.Where(a => a.ADMIN_NO_OFFICER_CODE == adn && a.ACK_DSC_STATUS == "ACCP").ToList();
                    //    if (result != null && result.Count() > 0)
                    //    {
                    //        DSCResultList.AddRange(result);
                    //    }
                    //});

                    //if (DSCResultList.Count() > 0)
                    //{
                    var obj = dbContext.REAT_OMMAS_DSC_MAPPING.SingleOrDefault(u => u.ADMIN_NO_OFFICER_CODE == ADMIN_NO_OFFICER_CODE && u.ADMIN_ND_CODE == ADMINNDCODE && u.FILE_ID == FileID);
                    if (obj != null)
                    {
                        obj.ACK_DSC_STATUS = "ACCP";
                        obj.IS_ACTIVE = true;
                        updatedrecord = dbContext.SaveChanges();
                    }



                    if (updatedrecord > 0)
                    {
                        return "DSC Approved Sucessfully!";
                    }
                    else
                    {
                        return "Status not get updated!";
                    }
                    //}
                    //else
                    //{
                    //    return "Already accepted DSC not found!";
                    //}
                }
                else
                {
                    return "Certificate not found against this Authorized Signatory!";
                }


            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "REAT.ApproveDSCDAL()");
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

        //public String ApproveDSCDAL(int ADMIN_NO_OFFICER_CODE, int ADMINNDCODE, long FileID)
        //{

        //    dbContext = new PMGSYEntities();
        //    try
        //    {

        //        int updatedrecord = 0;
        //        List<REAT_OMMAS_DSC_MAPPING> DSCResultList = new List<REAT_OMMAS_DSC_MAPPING>();

        //        var cerdetails = dbContext.ACC_CERTIFICATE_DETAILS.Where(c => c.ADMIN_NO_OFFICER_CODE == ADMIN_NO_OFFICER_CODE).FirstOrDefault();
        //        if (cerdetails != null)
        //        {
        //            List<int> certList = dbContext.ACC_CERTIFICATE_DETAILS.Where(s => s.ADMIN_NO_OFFICER_CODE != ADMIN_NO_OFFICER_CODE && s.CERTIFICATE == cerdetails.CERTIFICATE
        //                                                                   && s.CERTIFICATE_CHAIN == cerdetails.CERTIFICATE_CHAIN && s.PUBLIC_KEY == cerdetails.PUBLIC_KEY).
        //                                                                   Select(x => x.ADMIN_NO_OFFICER_CODE).Distinct().ToList();
        //            List<int> adminnooffcode = dbContext.ACC_CERTIFICATE_DETAILS_SHADOW.Where(s => s.ADMIN_NO_OFFICER_CODE != ADMIN_NO_OFFICER_CODE && s.CERTIFICATE == cerdetails.CERTIFICATE
        //                                                                   && s.CERTIFICATE_CHAIN == cerdetails.CERTIFICATE_CHAIN && s.PUBLIC_KEY == cerdetails.PUBLIC_KEY).
        //                                                                   Select(x => x.ADMIN_NO_OFFICER_CODE).Distinct().ToList();

        //            certList.AddRange(adminnooffcode);

        //            certList.Distinct().ToList().ForEach(adn =>
        //            {
        //                var result = dbContext.REAT_OMMAS_DSC_MAPPING.Where(a => a.ADMIN_NO_OFFICER_CODE == adn && a.ACK_DSC_STATUS == "ACCP").ToList();
        //                if (result != null && result.Count() > 0)
        //                {
        //                    DSCResultList.AddRange(result);
        //                }
        //            });

        //            if (DSCResultList.Count() > 0)
        //            {
        //                var obj = dbContext.REAT_OMMAS_DSC_MAPPING.SingleOrDefault(u => u.ADMIN_NO_OFFICER_CODE == ADMIN_NO_OFFICER_CODE && u.ADMIN_ND_CODE == ADMINNDCODE && u.FILE_ID == FileID);
        //                if (obj != null)
        //                {
        //                    obj.ACK_DSC_STATUS = "ACCP";
        //                    obj.IS_ACTIVE = true;
        //                    updatedrecord = dbContext.SaveChanges();
        //                }



        //                if (updatedrecord > 0)
        //                {
        //                    return "DSC Approved Sucessfully!";
        //                }
        //                else
        //                {
        //                    return "Status not get updated!";
        //                }
        //            }
        //            else
        //            {
        //                return "Already accepted DSC not found!";
        //            }
        //        }
        //        else
        //        {
        //            return "Certificate not found against this Authorized Signatory!";
        //        }


        //    }
        //    catch (Exception ex)
        //    {
        //        ErrorLog.LogError(ex, "REAT.ApproveDSCDAL()");
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





        
        
        #endregion


        #region REAT OB Acknowledgement
        public bool SaveOBAcknowlegement(XElement doc, string FileName, out bool isRecordExists)// Added by Aditi Shree on 8 May 2020
        {
            isRecordExists = false;
            dbContext = new PMGSYEntities();
            try
            {
                #region File reading code
                XNamespace ns = doc.GetDefaultNamespace();
                var xmlfiledata = (from ack in doc.Element(ns + "CstmrPmtAckRpt").Elements(ns + "OrgnlGrpInfAndSts")
                                   select new
                                   {
                                       FileName = FileName,
                                       MessageId = ack.Element(ns + "OrgnlMsgId").Value,
                                       AckReceivedDate = (from cd in doc.Element(ns + "CstmrPmtAckRpt").Elements(ns + "GrpHdr") select cd.Element(ns + "CreDtTm").Value).FirstOrDefault(),
                                       BatchId = ack.Element(ns + "OrgnlPmtInfId").Value,
                                       Status = ack.Element(ns + "GrpSts").Value,
                                       TxnDate = ack.Element(ns + "OrgnlCreDtTm").Value,

                                       RejectionCode = ((ack.Element(ns + "TxInfAndSts").Element(ns + "StsRsnInf").Descendants(ns + "Rsn").Count() > 0) ?
                                                 (from cd in ack.Element(ns + "TxInfAndSts").Element(ns + "StsRsnInf").Elements(ns + "Rsn")
                                                  select cd.Element(ns + "Cd").Value).ToList() : null),

                                       RejectionReason = ((ack.Element(ns + "TxInfAndSts").Element(ns + "StsRsnInf").Descendants(ns + "Rsn").Count() > 0) ?
                                                 (from cd in ack.Element(ns + "TxInfAndSts").Element(ns + "StsRsnInf").Elements(ns + "Rsn")
                                                  select cd.Element(ns + "AddtlInf").Value).ToList() : null),
                                   }).ToList();
                #endregion

                #region Data saving code

                REAT_OB_DETAILS OBMapping = null;
                for (int i = 0; i < xmlfiledata.Count; i++)
                {
                    String originalFile = xmlfiledata[i].MessageId + ".xml";
                    REAT_DATA_SEND_DETAILS OrigMsg = dbContext.REAT_DATA_SEND_DETAILS.SingleOrDefault(s => s.GENERATED_FILE_NAME == originalFile);

                    String BatchId = xmlfiledata[i].BatchId.ToString();
                    OBMapping = dbContext.REAT_OB_DETAILS.Where(x => x.BATCH_ID == BatchId && x.ADMIN_ND_CODE == OrigMsg.ADMIN_ND_CODE && x.OB_FILE_NAME == originalFile).FirstOrDefault();

                    if (OBMapping != null && OrigMsg != null)
                    {
                        OrigMsg.RECEIVED_FILE_NAME = FileName;
                        OrigMsg.RESPONSE_RECEIVED_DATE = DateTime.Now;
                        string ReceiveDate = xmlfiledata[i].AckReceivedDate.Substring(0, 10).Split('-')[2] + "/" + xmlfiledata[i].AckReceivedDate.Substring(0, 10).Split('-')[1] + "/" + xmlfiledata[i].AckReceivedDate.Substring(0, 10).Split('-')[0];
                        OBMapping.RESPONSE_RECEIVED_DATE = new CommonFunctions().GetStringToDateTime(ReceiveDate.Trim());
                        OBMapping.RECEIVED_FILE_NAME = FileName;
                        OBMapping.REAT_RESPONSE = (xmlfiledata[i].Status == "ACCP" || xmlfiledata[i].Status == "ACPT") ? "A" : "R";

                        if (OBMapping.REAT_RESPONSE == "R")
                        {
                            OBMapping.OB_STATUS = "I";  //Set OB_Status as Inactive in case of Rejected response to allow re-entry of OB.
                            OBMapping.RECEIVED_RESPONSE += xmlfiledata[i].RejectionReason == null ? null : (!string.IsNullOrEmpty(OBMapping.RECEIVED_RESPONSE)) ? "," + string.Join(",", xmlfiledata[i].RejectionReason) : string.Join(",", xmlfiledata[i].RejectionReason);
                        }
                        else
                        {
                            OBMapping.RECEIVED_RESPONSE = null;
                        }
                        dbContext.Entry(OBMapping).State = System.Data.Entity.EntityState.Modified;
                    }
                    else
                    {
                        REATLog("REAT OB", ConfigurationManager.AppSettings["REATOBLog"].ToString(), "No Records to update for OB acknowledgement", FileName);
                        return false;
                    }
                }
                #endregion
                if (OBMapping != null)
                {
                    dbContext.SaveChanges();
                    REATLog("REAT OB", ConfigurationManager.AppSettings["REATOBLog"].ToString(), "OB Acknowledgement successful", FileName);
                }
                return true;
            }
            catch (Exception ex)
            {
                REATLog("REAT OB", ConfigurationManager.AppSettings["REATOBLog"].ToString(), "Error in OB Acknowledgement", FileName);
                ErrorLog.LogError(ex, "REATDAL.SaveOBAcknowlegement()");
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
        #endregion



        #region REAT Fund Receipt Acknowledgement
        public bool SaveFRAcknowlegement(XElement doc, string FileName, out bool isRecordExists)
        {
            isRecordExists = false;
            dbContext = new PMGSYEntities();
            try
            {
                #region File reading code
                XNamespace ns = doc.GetDefaultNamespace();
                var xmlfiledata = (from ack in doc.Element(ns + "CstmrPmtAckRpt").Elements(ns + "OrgnlGrpInfAndSts")
                                   select new
                                   {
                                       FileName = FileName,
                                       MessageId = ack.Element(ns + "OrgnlMsgId").Value,
                                       AckReceivedDate = (from cd in doc.Element(ns + "CstmrPmtAckRpt").Elements(ns + "GrpHdr") select cd.Element(ns + "CreDtTm").Value).FirstOrDefault(),
                                       BatchId = ack.Element(ns + "OrgnlPmtInfId").Value,
                                       Status = ack.Element(ns + "GrpSts").Value,
                                       TxnDate = ack.Element(ns + "OrgnlCreDtTm").Value,

                                       RejectionCode = ((ack.Element(ns + "TxInfAndSts").Element(ns + "StsRsnInf").Descendants(ns + "Rsn").Count() > 0) ?
                                                 (from cd in ack.Element(ns + "TxInfAndSts").Element(ns + "StsRsnInf").Elements(ns + "Rsn")
                                                  select cd.Element(ns + "Cd").Value).ToList() : null),

                                       RejectionReason = ((ack.Element(ns + "TxInfAndSts").Element(ns + "StsRsnInf").Descendants(ns + "Rsn").Count() > 0) ?
                                                 (from cd in ack.Element(ns + "TxInfAndSts").Element(ns + "StsRsnInf").Elements(ns + "Rsn")
                                                  select cd.Element(ns + "AddtlInf").Value).ToList() : null),
                                   }).ToList();
                #endregion

                #region Data saving code

                REAT_RECEIPT_DETAILS FRMapping = null;
                for (int i = 0; i < xmlfiledata.Count; i++)
                {
                    String originalFile = xmlfiledata[i].MessageId + ".xml";
                    REAT_DATA_SEND_DETAILS OrigMsg = dbContext.REAT_DATA_SEND_DETAILS.SingleOrDefault(s => s.GENERATED_FILE_NAME == originalFile);

                    String BatchId = xmlfiledata[i].BatchId.ToString();
                    FRMapping = dbContext.REAT_RECEIPT_DETAILS.Where(x => x.BATCH_ID == BatchId && x.ADMIN_ND_CODE == OrigMsg.ADMIN_ND_CODE && x.REQ_FILENAME == originalFile).FirstOrDefault();

                    if (FRMapping != null && OrigMsg != null)
                    {
                        OrigMsg.RECEIVED_FILE_NAME = FileName;
                        OrigMsg.RESPONSE_RECEIVED_DATE = DateTime.Now;
                        string ReceiveDate = xmlfiledata[i].AckReceivedDate.Substring(0, 10).Split('-')[2] + "/" + xmlfiledata[i].AckReceivedDate.Substring(0, 10).Split('-')[1] + "/" + xmlfiledata[i].AckReceivedDate.Substring(0, 10).Split('-')[0];
                        FRMapping.ACK_RECEIVED_DATE = new CommonFunctions().GetStringToDateTime(ReceiveDate.Trim());
                        FRMapping.ACK_RECEIVED_FILENAME = FileName;
                        FRMapping.ACK_STATUS = (xmlfiledata[i].Status == "ACCP" || xmlfiledata[i].Status == "ACPT") ? "A" : "R";

                        if (FRMapping.ACK_STATUS == "R")
                        {
                            FRMapping.REJECTION_CODE += xmlfiledata[i].RejectionCode == null ? null : (!string.IsNullOrEmpty(FRMapping.REJECTION_CODE)) ? "," + string.Join(",", xmlfiledata[i].RejectionCode) : string.Join(",", xmlfiledata[i].RejectionCode);
                            FRMapping.REJECTION_NARRATION += xmlfiledata[i].RejectionReason == null ? null : (!string.IsNullOrEmpty(FRMapping.REJECTION_NARRATION)) ? "," + string.Join(",", xmlfiledata[i].RejectionReason) : string.Join(",", xmlfiledata[i].RejectionReason);
                        }
                        else
                        {
                            FRMapping.REJECTION_CODE = null;
                            FRMapping.REJECTION_NARRATION = null;
                        }
                        dbContext.Entry(FRMapping).State = System.Data.Entity.EntityState.Modified;
                    }
                    else
                    {
                        REATLog("REAT FR", ConfigurationManager.AppSettings["REATFRLog"].ToString(), "No Records to update for  acknowledgement", FileName);
                        return false;
                    }
                }
                #endregion
                if (FRMapping != null)
                {
                    dbContext.SaveChanges();
                    REATLog("REAT FR", ConfigurationManager.AppSettings["REATFRLog"].ToString(), "FR Acknowledgement successful", FileName);
                }
                return true;
            }
            catch (Exception ex)
            {
                REATLog("REAT FR", ConfigurationManager.AppSettings["REATFRLog"].ToString(), "Error in FR Acknowledgement", FileName);
                ErrorLog.LogError(ex, "REATDAL.SaveFRAcknowlegement()");
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

        #endregion


        #region REAT Fund Receipt Generation

        public Array FundReceiptList(FundReceiptModel objModel, int? page, int? rows, string sidx, string sord, out long totalRecords)
        {
            try
            {
                CommonFunctions comObj = null;
                PMGSYEntities dbContext = new PMGSYEntities();

                var lstBillMaster = (from bm in dbContext.ACC_BILL_MASTER
                                     join txn in dbContext.ACC_MASTER_TXN on bm.TXN_ID equals txn.TXN_ID
                                     where
                                     //bm.FUND_TYPE == "P"//commeneted on 15-03-2023
                                     bm.FUND_TYPE == PMGSYSession.Current.FundType //Added on 15-03-2023
                                     && bm.BILL_TYPE == "R"
                                     && bm.BILL_FINALIZED == "Y"
                                     && bm.ADMIN_ND_CODE == PMGSYSession.Current.AdminNdCode
                                     && bm.BILL_MONTH + (bm.BILL_YEAR * 12) == objModel.Month + (objModel.Year * 12)
                                     && bm.CHQ_EPAY == "Q"
                                     select new
                                     {
                                         bm.BILL_ID,
                                         bm.BILL_NO,
                                         bm.BILL_DATE,
                                         bm.CHQ_NO,
                                         bm.CHQ_DATE,
                                         txn.TXN_DESC,
                                         bm.CHQ_AMOUNT,
                                         bm.CASH_AMOUNT,
                                         bm.GROSS_AMOUNT,
                                     }).OrderBy(x => x.BILL_NO).ToList();

                totalRecords = lstBillMaster.Count();

                lstBillMaster = lstBillMaster.OrderBy(x => x.BILL_NO).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();

                return lstBillMaster.Select(item => new
                {
                    id = URLEncrypt.EncryptParameters(new string[] { item.BILL_ID.ToString().Trim() }),
                    cell = new[] { 
                         item.BILL_NO,
                         Convert.ToDateTime(item.BILL_DATE).ToString("dd/MM/yyyy"),
                         item.CHQ_NO,
                         item.CHQ_DATE == null ? String.Empty : Convert.ToDateTime(item.CHQ_DATE).ToString("dd/MM/yyyy"),
                         item.TXN_DESC.Trim(),
                         item.CHQ_AMOUNT.ToString(), 
                         item.CASH_AMOUNT.ToString(),
                         item.GROSS_AMOUNT.ToString(),  
                         (dbContext.REAT_RECEIPT_DETAILS.Where(z=>z.BILL_ID == item.BILL_ID).Any())? "Receipt Generated" :
                         "<center><a href='#' class='ui-icon ui-icon-plusthick' onClick='GenerateFundXMLFile(\"" +URLEncrypt.EncryptParameters(new string[] {"BillID =" + item.BILL_ID.ToString().Trim()})+ "\"); return false;'>Generate Fund Receipt</a></center>"                                                                                       
                       
                    }
                }).ToArray();
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "REATDAL.FundReceiptList()");
                totalRecords = 0;
                return null;
            }

        }

        public string GenerateXMLFundReceiptDAL(Int64 billId, out string fileName)
        {
            try
            {
                dbContext = new PMGSYEntities();
                String FileName = string.Empty;
                ADMIN_DEPARTMENT AdminDeptPIU = dbContext.ADMIN_DEPARTMENT.FirstOrDefault(s => s.ADMIN_ND_CODE == PMGSYSession.Current.AdminNdCode);
               // ACC_BANK_DETAILS BankDetails = dbContext.ACC_BANK_DETAILS.FirstOrDefault(s => s.FUND_TYPE == "P" && s.ADMIN_ND_CODE == AdminDeptPIU.MAST_PARENT_ND_CODE && s.BANK_ACC_STATUS == true);//Commented on 15-03-2023 
                ACC_BANK_DETAILS BankDetails = dbContext.ACC_BANK_DETAILS.FirstOrDefault(s => s.FUND_TYPE == PMGSYSession.Current.FundType && s.ADMIN_ND_CODE == AdminDeptPIU.MAST_PARENT_ND_CODE && s.BANK_ACC_STATUS == true && s.ACCOUNT_TYPE == "S");//Added on 15-03-2023 
                REAT_INITIATING_PARTY_MASTER initparty = dbContext.REAT_INITIATING_PARTY_MASTER.FirstOrDefault(s => s.ADMIN_ND_CODE == AdminDeptPIU.MAST_PARENT_ND_CODE);
                decimal GrossAmount = dbContext.ACC_BILL_MASTER.Where(x => x.BILL_ID == billId).Select(x => x.GROSS_AMOUNT).FirstOrDefault();

                //Below Code Commented on 15-03-2023
                //int runningCount = dbContext.REAT_DATA_SEND_DETAILS.Any(s => s.FUND_TYPE == PMGSYSession.Current.FundType && s.FILE_TYPE == "R")
                //                   ? (dbContext.REAT_DATA_SEND_DETAILS.Where(s => s.FUND_TYPE == PMGSYSession.Current.FundType && s.FILE_TYPE == "R" && EntityFunctions.TruncateTime(s.FILE_GENERATION_DATE) == DateTime.Today).Count()) + 1
                //                   : 1;

                //Below Code Added on 15-03-2023
                int runningCount = dbContext.REAT_DATA_SEND_DETAILS.Any(s => s.FILE_TYPE == "R")
                                   ? (dbContext.REAT_DATA_SEND_DETAILS.Where(s =>  s.FILE_TYPE == "R" && EntityFunctions.TruncateTime(s.FILE_GENERATION_DATE) == DateTime.Today).Count()) + 1
                                   : 1;
                #region XML Generation

                FileName = "0037FDSRCPTREQ" + DateTime.Now.ToString("ddMMyyyy") + "" + runningCount.ToString();
                StringBuilder startString = new StringBuilder("<EATIncomeFromOtherSources  xmlns=\"http://cpsms.nic.in/EATIncomeFromOtherSources\"><CstmrCdtTrfInitn>");

                XDocument doc = new XDocument(new XElement("GrpHdr",
                                                    new XElement("MsgId", "0037FDSRCPTREQ" + DateTime.Now.ToString("ddMMyyyy") + "" + runningCount.ToString()),
                                                    new XElement("Src", "0037"),

                                                     new XElement("InitgPty",
                                                             new XElement("PrTry",
                                                                    new XElement("Id", initparty.REAT_INIT_PARTY_UNIQUE_CODE)
                                                                 )),
                                                     new XElement("PmtInf",
                                                              new XElement("PmtInfId", "P0037" + DateTime.Now.ToString("ddMMyy") + "" + runningCount.ToString("D5")),
                                                              new XElement("PmtInfDt", DateTime.Now.ToString("yyyy-MM-dd"))
                                                                  ),
                                                     new XElement("OrgId",
                                                          new XElement("SchmeCd", initparty.SCHEME_CODE)
                                                                )
                                                    ));
                string xmlstring = doc.ToString();
                int FRId = !(dbContext.REAT_RECEIPT_DETAILS.Any()) ? 1 : (dbContext.REAT_RECEIPT_DETAILS.Max(x => x.RECEIPT_ID) + 1); //new change addedy on 14 July 2020
                string SrcSysTranNo = PMGSYSession.Current.AdminNdCode + "/" + PMGSYSession.Current.FundType + "/" + FRId;
                int month = DateTime.Now.Month;
                int year = DateTime.Now.Year;
                int financialYear;
                if (month != 1 && month != 2 && month != 3)
                {
                    financialYear = year + 1;
                }
                else
                {
                    financialYear = year;
                }
                XDocument middleString = new XDocument(new XElement("CdtTrfTxInf",
                                                         new XElement("EndToEndId", SrcSysTranNo),
                                                         new XElement("CdtrAcct",
                                                              new XElement("Id",
                                                                   new XElement("Othr",
                                                                        new XElement("BBAN", BankDetails.BANK_ACC_NO))),
                                                              new XElement("FinInstnId",
                                                                   new XElement("BrnchId", BankDetails.MAST_IFSC_CODE))),
                                                                    new XElement("FndSrc",
                                                                         new XElement("Id", "8"),
                                                                              new XElement("Typ", "1"),
                                                                           new XElement("Amt", GrossAmount)),
                                                                 new XElement("AccntngEntty",
                                                                      new XElement("Id", "")),
                                                                 new XElement("PmtMd", "4"),
                                                                 new XElement("PmtTxDtls",
                                                                      new XElement("Txsts", "2"),
                                                                       new XElement("FinYr", financialYear),
                                                                        new XElement("ReqdExctnDt", DateTime.Now.ToString("yyyy-MM-dd")),
                                                                         new XElement("TxPrps", "Remarks")),
                                                                  new XElement("PmtId",
                                                                      new XElement("InstrId", "100000"),
                                                                      new XElement("CreDtTm", DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss")))
                                                             ));

                String endString = "</CstmrCdtTrfInitn></EATIncomeFromOtherSources>";

                startString.Append(doc.ToString());
                startString.Append(middleString.ToString());
                startString.Append(endString);

                XDocument finaldoc = XDocument.Parse(startString.ToString());
                fileName = FileName.Trim() + ".xml";
                #endregion

                string stateShortCode = GetStateShortName(PMGSYSession.Current.StateCode);
                if (!string.IsNullOrEmpty(fileName.Trim()))
                {
                    if (!Directory.Exists(Path.Combine(@"" + ConfigurationManager.AppSettings["REATFundXmlFilePath"] + stateShortCode)))
                        Directory.CreateDirectory(Path.Combine(@"" + ConfigurationManager.AppSettings["REATFundXmlFilePath"] + stateShortCode));

                    finaldoc.Save(ConfigurationManager.AppSettings["REATFundXmlFilePath"] + stateShortCode.Trim() + "\\" + fileName);
                }
                //finaldoc.Save(@"D:\OmmasImages\REAT\FundReceipt"+fileName);

                #region Save Receipt details in DB

                using (TransactionScope ts = new TransactionScope())
                {
                    try
                    {
                        REAT_DATA_SEND_DETAILS dataSendDetails = new REAT_DATA_SEND_DETAILS();
                        dataSendDetails.FILE_ID = !(dbContext.REAT_DATA_SEND_DETAILS.Any()) ? 1 : (dbContext.REAT_DATA_SEND_DETAILS.Max(x => x.FILE_ID) + 1);
                        dataSendDetails.ADMIN_ND_CODE = PMGSYSession.Current.AdminNdCode;
                        dataSendDetails.FUND_TYPE = PMGSYSession.Current.FundType;
                        dataSendDetails.GENERATED_FILE_NAME = fileName;
                        dataSendDetails.FILE_GENERATION_DATE = DateTime.Now;
                        dataSendDetails.GENERATED_FILE_PATH = Path.Combine(@"" + ConfigurationManager.AppSettings["REATFundXmlFilePath"] + fileName.Trim() + ".xml");
                        dataSendDetails.FILE_TYPE = "R";
                        dataSendDetails.USER_ID = PMGSYSession.Current.UserId;
                        dataSendDetails.IPADDR = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                        dbContext.REAT_DATA_SEND_DETAILS.Add(dataSendDetails);

                        ACC_BILL_MASTER accBillMaster = new ACC_BILL_MASTER();
                        accBillMaster = dbContext.ACC_BILL_MASTER.Where(m => m.BILL_ID == billId).FirstOrDefault();

                        REAT_RECEIPT_DETAILS reatReceipt = new REAT_RECEIPT_DETAILS();
                        reatReceipt.RECEIPT_ID = !(dbContext.REAT_RECEIPT_DETAILS.Any()) ? 1 : (dbContext.REAT_RECEIPT_DETAILS.Max(x => x.RECEIPT_ID) + 1);
                        reatReceipt.FILE_ID = dataSendDetails.FILE_ID;
                        reatReceipt.ADMIN_ND_CODE = PMGSYSession.Current.AdminNdCode;
                        reatReceipt.BATCH_ID = "P0037" + DateTime.Now.ToString("ddMMyy") + "" + runningCount.ToString("D5");
                        reatReceipt.BILL_NO = accBillMaster.BILL_NO;
                        reatReceipt.BILL_DATE = accBillMaster.BILL_DATE;
                        reatReceipt.CHQ_NO = accBillMaster.CHQ_NO;
                        reatReceipt.CHQ_DATE = Convert.ToDateTime(accBillMaster.CHQ_DATE);
                        reatReceipt.CHQ_AMOUNT = accBillMaster.CHQ_AMOUNT;
                        reatReceipt.CASH_AMOUNT = accBillMaster.CASH_AMOUNT;
                        reatReceipt.GROSS_AMOUNT = accBillMaster.GROSS_AMOUNT;
                        reatReceipt.BILL_ID = accBillMaster.BILL_ID;
                        reatReceipt.REQ_FILENAME = fileName;
                        reatReceipt.REQ_FILE_GEN_DATE = DateTime.Now;
                        dbContext.REAT_RECEIPT_DETAILS.Add(reatReceipt);

                        dbContext.SaveChanges();
                    }
                    catch (Exception ex)
                    {
                        ErrorLog.LogError(ex, "REATDAL.GenerateXMLFundReceiptDAL");
                        return String.Empty;
                    }
                    ts.Complete();
                }
                #endregion

                return startString.ToString();

            }
            catch (OptimisticConcurrencyException ex)
            {
                ErrorLog.LogError(ex, "REATDAL.GenerateXMLFundReceiptDAL.OptimisticConcurrencyException");
                fileName = "";
                return String.Empty;
            }
            catch (UpdateException ex)
            {
                ErrorLog.LogError(ex, "REATDAL.GenerateXMLFundReceiptDAL.UpdateException");
                fileName = "";
                return String.Empty;
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "REATDAL.GenerateXMLFundReceiptDAL");
                fileName = "";
                return String.Empty;
            }
            finally
            {
                if (dbContext != null)
                {
                    dbContext.Dispose();
                }
            }
        }




        
        
        
        #endregion



        #region REAT Beneficiary Updation
        public Array GetBeneficiaryUpdationDAL(int stateCode, int districtCode, int agencyCode, int? page, int? rows, string sidx, string sord, out Int32 totalRecords)
        {
            string date = string.Empty;
            try
            {
                using (StreamWriter sw = System.IO.File.AppendText(System.IO.Path.Combine("D:\\ErrorLog\\OMMASErrorLog_" + System.DateTime.Now.Date.ToString("dd_MM_yyyy") + ".txt", "")))
                {
                    sw.WriteLine("__________________STEP 1 Reat Beneficiary Update______________________________");
                    sw.WriteLine("Date :" + DateTime.Now.ToString() + " UserName -" + PMGSYSession.Current.UserName);
                    sw.WriteLine("stateCode : " + stateCode);
                    sw.WriteLine("districtCode : " + districtCode);
                    sw.WriteLine("agencyCode : " + agencyCode);
                    sw.WriteLine("____________________________________________________");
                    sw.Close();
                }

                dbContext = new PMGSYEntities();
                List<USP_GET_CONTRACTOR_FOR_UPDATION_Result> lstcontractorDetails = new List<USP_GET_CONTRACTOR_FOR_UPDATION_Result>();
                lstcontractorDetails = dbContext.USP_GET_CONTRACTOR_FOR_UPDATION(stateCode, agencyCode).ToList<USP_GET_CONTRACTOR_FOR_UPDATION_Result>(); ;

                //using (StreamWriter sw = System.IO.File.AppendText(System.IO.Path.Combine("D:\\ErrorLog\\OMMASErrorLog_" + System.DateTime.Now.Date.ToString("dd_MM_yyyy") + ".txt", "")))
                //{
                //    sw.WriteLine("__________________STEP 2 Reat Beneficiary Update______________________________");
                //    sw.WriteLine("Date :" + DateTime.Now.ToString() + " UserName -" + PMGSYSession.Current.UserName);

                //    sw.Close();
                //}

                totalRecords = lstcontractorDetails.Count();

                //using (StreamWriter sw = System.IO.File.AppendText(System.IO.Path.Combine("D:\\ErrorLog\\OMMASErrorLog_" + System.DateTime.Now.Date.ToString("dd_MM_yyyy") + ".txt", "")))
                //{
                //    sw.WriteLine("__________________STEP 3 Reat Beneficiary Update______________________________");
                //    sw.WriteLine("Date :" + DateTime.Now.ToString() + " UserName -" + PMGSYSession.Current.UserName);
                //    sw.WriteLine("totalRecords : " + totalRecords);
                //    sw.Close();
                //}

                if (sidx.Trim() != string.Empty)
                {
                    if (sord.ToString() == "asc")
                    {
                        switch (sidx)
                        {
                            case "ContractorName":
                                lstcontractorDetails = lstcontractorDetails.OrderBy(x => x.MAST_CON_ID).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                            default:
                                break;
                        }
                    }
                    else
                    {
                        switch (sidx)
                        {
                            case "ContractorName":
                                lstcontractorDetails = lstcontractorDetails.OrderByDescending(x => x.MAST_CON_ID).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                            default:
                                break;
                        }
                    }
                }
                else
                {
                    lstcontractorDetails = lstcontractorDetails.OrderByDescending(x => x.MAST_CON_ID).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                }


                //using (StreamWriter sw = System.IO.File.AppendText(System.IO.Path.Combine("D:\\ErrorLog\\OMMASErrorLog_" + System.DateTime.Now.Date.ToString("dd_MM_yyyy") + ".txt", "")))
                //{
                //    sw.WriteLine("__________________STEP 4 Reat Beneficiary Update______________________________");
                //    sw.WriteLine("Date :" + DateTime.Now.ToString() + " UserName -" + PMGSYSession.Current.UserName);
                //    sw.WriteLine("Before Return : ");
                //    sw.Close();
                //}

                return lstcontractorDetails.Select(contractor => new
                {
                    id = contractor.MAST_CON_ID.ToString(),
                    cell = new[] {      
                                    contractor.MAST_CON_ID==null?"-":contractor.MAST_CON_ID.ToString(),
                                    string.IsNullOrEmpty(contractor.reat_CON_NAME)?"-":contractor.reat_CON_NAME.Trim() ,
                                    string.IsNullOrEmpty(contractor.MAST_CON_PAN) ? "-" : contractor.MAST_CON_PAN,
                                    contractor.MAST_CON_COMPANY_NAME,
                                    string.IsNullOrEmpty(contractor.MAST_BANK_NAME)?"-": contractor.MAST_BANK_NAME.Trim().Replace("\n", "").Replace("\r", ""),
                                    (contractor.MAST_ACCOUNT_ID==null)?"-":contractor.MAST_ACCOUNT_ID.ToString(),
                                    contractor.MAST_IFSC_CODE,
                                    contractor.MAST_ACCOUNT_NUMBER,

                                      dbContext.REAT_CONTRACTOR_DETAILS.Any(m=>m.DETAIL_ID==contractor.DETAIL_ID && m.UPDATED_VENDOR_ID==null)?"<a href='#' title='Click here to Edit' class='ui-icon ui-icon-plusthick ui-align-center' onClick='EditContractor(\"" + contractor.MAST_CON_ID.ToString().Trim() + "," + contractor.DETAIL_ID.ToString().Trim() + ","+stateCode.ToString().Trim() + "," +agencyCode.ToString().Trim() + "\"); return false;'>update</a>"
                                    :((contractor.reat_STATUS=="R")?"<a href='#' title='Click here to Edit' class='ui-icon ui-icon-plusthick ui-align-center' onClick='EditContractor(\"" + contractor.MAST_CON_ID.ToString().Trim() + "," + contractor.DETAIL_ID.ToString().Trim() + ","+stateCode.ToString().Trim() + "," +agencyCode.ToString().Trim() +"\"); return false;'>update</a>":
                                    (contractor.reat_STATUS=="A")?"--":(contractor.ALLOW_OR_NOT.Equals("Y")?"<a href='#' title='Click here to Edit' class='ui-icon ui-icon-plusthick ui-align-center' onClick='EditContractor(\"" + contractor.MAST_CON_ID.ToString().Trim() + "," + contractor.DETAIL_ID.ToString().Trim() + ","+stateCode.ToString().Trim() + "," +agencyCode.ToString().Trim() +"\"); return false;'>update</a>":"-" ))

                                  //  dbContext.REAT_CONTRACTOR_DETAILS.Any(m=>m.DETAIL_ID==contractor.DETAIL_ID && m.UPDATED_VENDOR_ID!=null) ?"<span class='ui-icon ui-icon-locked ui-align-center'></span>" : "<a href='#' title='Click here to Edit' class='ui-icon ui-icon-plusthick ui-align-center' onClick='EditContractor(\"" + contractor.MAST_CON_ID.ToString().Trim() + ","+stateCode.ToString().Trim() + "," +agencyCode.ToString().Trim() +"\"); return false;'>update</a>"

                                
                    }
                }).ToArray();
            }
            catch (Exception ex)
            {
                totalRecords = 0;
                ErrorLog.LogError(ex, "REATDAL.GetBeneficiaryDetailsDAL()");
                return null;
            }
        }


        public bool GenerateXMLForBeneficiaryUpdationDAL(REATDownloadBeneficiaryUpdateXML model, ref string message)
        {

            bool flg = false;
            //string xmlFName = string.Empty;
            // int recCount = 0;
            string xmlString = string.Empty;
            string xmlHeader = string.Empty;
            string xmlBody = string.Empty;
            string xmlFileName = string.Empty;

            //   int recordCount = 0;

            try
            {
                dbContext = new PMGSYEntities();

                using (TransactionScope ts = new TransactionScope())
                {
                    try
                    {

                        // var vl = dbContext.REAT_Generate_Conctractor_XML_Updation(4,17,33787,16976,"SBIN0014617","Mr. ABDUR  RAHMAN",99,"1.2.3.4").ToList();

                        var vl = dbContext.REAT_Generate_Conctractor_XML_Updation(model.StateCode, model.AgencyCode, model.ContractorID, model.AccountID, model.reatIFSC, model.reatContractorName, PMGSYSession.Current.UserId, System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"], model.DETAIL_ID,model.BankName).ToList();
                        foreach (var item in vl)
                        {
                            xmlFileName = item.xmlFileName;
                            xmlHeader = item.XML_HEADER_OUTPUT;
                            xmlBody = item.XML_DETAILED_OUTPUT;
                        }


                        //StringBuilder startString = new StringBuilder("<EATOpeningBalance  xmlns=\"http://cpsms.nic.in/EATOpeningBalance\"><CstmrCdtTrfInitn>");
                        //String endString = "</CstmrCdtTrfInitn></EATOpeningBalance>";

                        //startString.Append(doc.ToString()); // Header
                        //startString.Append(middleString.ToString()); // Body
                        //startString.Append(endString); // End



                        //XDocument finaldoc = XDocument.Parse(startString.ToString());

                        //fileName = xmlFileName.Trim() + ".xml";

                        //finaldoc.Save(ConfigurationManager.AppSettings["REATBeneficiaryRequest"] + fileName);


                        xmlString = xmlHeader.Trim() + xmlBody.Trim();

                        XmlSerializer XmlS = new XmlSerializer(xmlString.GetType());

                        StringWriter sw = new PMGSY.DAL.PFMS.PFMSDAL1.StringWriterUtf8();
                        XmlTextWriter tw = new XmlTextWriter(sw);
                        tw.WriteProcessingInstruction("xml", "version=\"1.0\" encoding=\"UTF-8\" standalone=\"yes\"");

                        XmlS.Serialize(tw, xmlString);

                        xmlString = sw.ToString().Replace("&lt;", "<").Replace("&gt;", ">").Replace("<VendorRegRequest>", "").Replace("</VendorRegRequest>", "").Replace("<string>", @"<VendorRegRequest xmlns=""http://cpsms.nic.in/VendorRegistrationRequest""><CstmrDtls>").Replace("</string>", "</CstmrDtls></VendorRegRequest>");

                        dbContext = new PMGSYEntities();

                        string stateShortCode = GetStateShortName(model.StateCode);
                        string[] paths = { @"" + ConfigurationManager.AppSettings["REATBeneficiaryRequest"], stateShortCode.Trim(), "\\" + xmlFileName.Trim(), ".xml" };


                        byte[] bytes = Encoding.ASCII.GetBytes(xmlString.Trim());

                        if (!string.IsNullOrEmpty(xmlFileName.Trim()))
                        {

                            if (!Directory.Exists(Path.Combine(@"" + ConfigurationManager.AppSettings["REATBeneficiaryRequest"] + stateShortCode)))
                                Directory.CreateDirectory(Path.Combine(@"" + ConfigurationManager.AppSettings["REATBeneficiaryRequest"] + stateShortCode));



                            System.IO.File.WriteAllBytes(Path.Combine(@"" + ConfigurationManager.AppSettings["REATBeneficiaryRequest"] + stateShortCode + "\\" + xmlFileName.Trim() + ".xml"), bytes);


                            message = "REAT XML generated successfully";
                        }
                        else
                        {
                            message = "Error in REAT XML generation";
                            return false;
                        }
                    }
                    catch (Exception ex)
                    {
                        message = "Error in REAT XML generation..";
                        ErrorLog.LogError(ex, "REAT.GenerateXMLForBeneficiaryUpdationDAL");
                        return false;
                    }
                    ts.Complete();
                }
                return true;
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "REAT.GenerateXMLForBeneficiaryUpdationDAL()");
                xmlFileName = string.Empty;
                //  recCount = -1;

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
        #endregion



    }
}