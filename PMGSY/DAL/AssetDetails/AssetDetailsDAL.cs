using PMGSY.Common;
using PMGSY.Extensions;
using PMGSY.Models;
using PMGSY.Models.AssetDetails;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Transactions;
using System.Web;
using System.Web.Mvc;

namespace PMGSY.DAL
{
    public class AssetDetailsDAL:IAssetDetailsDAL
    {

        private PMGSYEntities dbContext = null;

        public Array GetAssetDetails(int? page, int? rows, string sidx, string sord, out long totalRecords, short? monthCode, short? yearCode, string chequeNo, string billNo, int adminCode, string fundType)
        {
            dbContext = new PMGSYEntities();
            try
            {
                if (chequeNo == string.Empty)
                {
                    chequeNo = "-1";
                }

                if (billNo == string.Empty)
                {
                    billNo = "-1";
                }

                var lstAssetDetails = dbContext.SP_ACC_ASSET_LIST_DETAILS(adminCode, monthCode, yearCode, "A", chequeNo, billNo).Distinct().ToList();

                totalRecords = lstAssetDetails.Count();

                if (sidx.Trim() != string.Empty)
                {
                    if (sord.ToString() == "asc")
                    {
                        switch (sidx)
                        {
                            case "Voucher_No":
                                lstAssetDetails = lstAssetDetails.OrderBy(m => m.Voucher_No).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                            case "Voucher_date":
                                lstAssetDetails = lstAssetDetails.OrderBy(m => m.Voucher_date).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                            case "Head_Desc":
                                lstAssetDetails = lstAssetDetails.OrderBy(m => m.Head_Desc).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                            case "Cheque_No":
                                lstAssetDetails = lstAssetDetails.OrderBy(m => m.Cheque_No).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                            case "Cheque_Date":
                                lstAssetDetails = lstAssetDetails.OrderBy(m => m.Cheque_Date).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                            case "Asset_Amount":
                                lstAssetDetails = lstAssetDetails.OrderBy(m => m.Asset_Amount).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                            case "Entered_Asset_Amount":
                                lstAssetDetails = lstAssetDetails.OrderBy(m => m.Entered_Asset_Amount).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                            case "Payee_Name":
                                lstAssetDetails = lstAssetDetails.OrderBy(m => m.Payee_Name).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                            default:
                                lstAssetDetails = lstAssetDetails.OrderBy(m => m.Voucher_No).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                        }
                    }
                    else
                    {
                        switch (sidx)
                        {
                            case "Voucher_No":
                                lstAssetDetails = lstAssetDetails.OrderByDescending(m => m.Voucher_No).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                            case "Voucher_date":
                                lstAssetDetails = lstAssetDetails.OrderByDescending(m => m.Voucher_date).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                            case "Head_Desc":
                                lstAssetDetails = lstAssetDetails.OrderByDescending(m => m.Head_Desc).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                            case "Cheque_No":
                                lstAssetDetails = lstAssetDetails.OrderByDescending(m => m.Cheque_No).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                            case "Cheque_Date":
                                lstAssetDetails = lstAssetDetails.OrderByDescending(m => m.Cheque_Date).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                            case "Asset_Amount":
                                lstAssetDetails = lstAssetDetails.OrderByDescending(m => m.Asset_Amount).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                            case "Entered_Asset_Amount":
                                lstAssetDetails = lstAssetDetails.OrderByDescending(m => m.Entered_Asset_Amount).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                            case "Payee_Name":
                                lstAssetDetails = lstAssetDetails.OrderByDescending(m => m.Payee_Name).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                            default:
                                lstAssetDetails = lstAssetDetails.OrderByDescending(m => m.Voucher_No).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                        }
                    }
                }
                else
                {
                    lstAssetDetails = lstAssetDetails.OrderBy(m => m.Voucher_No).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                }

                var gridData = lstAssetDetails.Select(lstAsset =>
                    new
                    {
                        lstAsset.Voucher_No,
                        lstAsset.Voucher_date,
                        lstAsset.Head_Desc,
                        lstAsset.Cheque_No,
                        lstAsset.Cheque_Date,
                        lstAsset.Asset_Amount,
                        lstAsset.Entered_Asset_Amount,
                        lstAsset.Payee_Name,
                        lstAsset.Head_Id,
                        lstAsset.Bill_ID
                    }).ToArray();

                return gridData.Select(m => new
                {
                    cell = new[]
                    {
                        m.Voucher_No.ToString(),
                        m.Voucher_date.ToString(),
                        m.Head_Desc.ToString(),
                        m.Cheque_No==null?string.Empty:m.Cheque_No.ToString(),
                        m.Cheque_Date == null?string.Empty:m.Cheque_Date.ToString(),
                        m.Asset_Amount.ToString(),
                        m.Entered_Asset_Amount.ToString(),
                        m.Payee_Name == null? string.Empty:m.Payee_Name.ToString(), 
                        dbContext.ACC_ASSET_DETAILS.Any(a=>a.BILL_ID == m.Bill_ID && a.HEAD_ID == m.Head_Id && a.ISFINALIZED == true)?"<span class='ui-icon ui-icon-locked ui-align-center'></span>":"<a href='#' title='Click here to add Asset Details' class='ui-icon ui-icon-plusthick ui-align-center' onClick=AddAssetDetails('" +URLEncrypt.EncryptParameters1(new string[]{"BillId="+m.Bill_ID.ToString().Trim(),"HeadId="+m.Head_Id.ToString().Trim(),"AssetAmount="+m.Asset_Amount.ToString().Trim(),"HeadDesc="+m.Head_Desc.ToString().Trim(),"VoucherDate="+m.Voucher_date.ToString().Trim().Replace('/','-'),"VoucherNo="+m.Voucher_No.ToString().Trim()})+"'); return false;'>Add Asset Details</a>",//:"<a href='#' title='Click here to add Edit Acknowledged Voucher Details' class='ui-icon ui-icon-pencil ui-align-center' onClick=AddVoucherDetails('" +URLEncrypt.EncryptParameters1(new string[]{"BillId="+m.Bill_id.ToString().Trim()})+"'); return false;'>Add Acknowledged Voucher Details</a>"
                        dbContext.ACC_ASSET_DETAILS.Any(a=>a.BILL_ID == m.Bill_ID && a.HEAD_ID == m.Head_Id && a.ISFINALIZED == true)?"<span class='ui-icon ui-icon-locked ui-align-center'></span>":m.Entered_Asset_Amount>0?"<a href='#' title='Click here to delete Asset Payment Details' class='ui-icon ui-icon-trash ui-align-center' onClick=DeleteAssetDetails('" +URLEncrypt.EncryptParameters1(new string[]{"BillId="+m.Bill_ID.ToString().Trim(),"HeadId="+m.Head_Id.ToString().Trim(),"HeadDesc="+m.Head_Desc.ToString().Trim(),"VoucherDate="+m.Voucher_date.ToString().Trim().Replace('/','-'),"VoucherNo="+m.Voucher_No.ToString().Trim()})+"'); return false;'>Delete Asset Details</a>":"-",
                        dbContext.ACC_ASSET_DETAILS.Any(a=>a.BILL_ID == m.Bill_ID && a.HEAD_ID == m.Head_Id && a.ISFINALIZED == true)?"<a href='#' title='Click here to definalize details' class='ui-icon ui-icon-unlocked ui-align-center' onClick=DefinalizeAssetDetails('" +URLEncrypt.EncryptParameters1(new string[]{"BillId="+m.Bill_ID.ToString().Trim(),"HeadId="+m.Head_Id.ToString().Trim()})+"'); return false;'>Definalize Asset Details</a>":"<span>-</span>"
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

        public List<SelectListItem> GetAssets()
        {
            dbContext = new PMGSYEntities();
            List<SelectListItem> lstAssets = new List<SelectListItem>();
            SelectListItem item = null;
            try
            {
                var query = (from asset in dbContext.ACC_MASTER_ASSET
                                  select new
                                  {
                                      Text = asset.ASSET_NAME,
                                      Value = asset.ASSET_ID
                                  }).ToList();

                item = new SelectListItem();
                item.Text = "--Select Asset--";
                item.Value = "0";
                item.Selected = true;
                lstAssets.Add(item);

                foreach (var data in query)
                {
                    item = new SelectListItem();
                    item.Text = data.Text;
                    item.Value = data.Value.ToString();
                    lstAssets.Add(item);
                }
                return lstAssets;
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);         
                
                return lstAssets;
            }
            finally
            {
                if (dbContext != null)
                {
                    dbContext.Dispose();
                }
            }
        }

        public Array GetAssetDetailsList(int? page, int? rows, string sidx, string sord, out long totalRecords, int billId, int headId,ref bool isFinalize)
        {
            dbContext = new PMGSYEntities();
            try
            {
                var detailsList = (from item in dbContext.ACC_ASSET_DETAILS
                                   join asset in dbContext.ACC_MASTER_ASSET on item.ASSET_ID equals asset.ASSET_ID
                                   where item.BILL_ID == billId &&
                                   item.HEAD_ID == headId
                                   select new
                                   {
                                       asset.ASSET_NAME,
                                       item.ASSIGNED_ID,
                                       item.BILL_ID,
                                       item.DETAIL_ID,
                                       item.DISPOSAL_DATE,
                                       item.HEAD_ID,
                                       item.MODEL_NO,
                                       item.QUANTITY,
                                       item.RATE,
                                       item.SERIAL_NO,
                                       item.TOTAL_AMOUNT,
                                       item.ISFINALIZED
                                   });

                if(detailsList.Any(m=>m.ISFINALIZED == true))
                {
                    isFinalize = true;
                }

                totalRecords = detailsList.Count();

                var gridData = detailsList.Select(m => new 
                {
                    m.ASSET_NAME,
                    m.ASSIGNED_ID,
                    m.BILL_ID,
                    m.DETAIL_ID,
                    m.DISPOSAL_DATE,
                    m.HEAD_ID,
                    m.MODEL_NO,
                    m.QUANTITY,
                    m.RATE,
                    m.SERIAL_NO,
                    m.TOTAL_AMOUNT,
                    m.ISFINALIZED
                }).ToArray();

                return gridData.Select(details => new 
                {
                    cell=new[]
                    {
                        details.ASSET_NAME == null?string.Empty:details.ASSET_NAME.ToString(),
                        details.SERIAL_NO==null?string.Empty:details.SERIAL_NO.ToString(),
                        details.MODEL_NO == null?string.Empty:details.MODEL_NO.ToString(),
                        details.RATE == null?string.Empty:details.RATE.ToString(),
                        details.TOTAL_AMOUNT == null?string.Empty:details.TOTAL_AMOUNT.ToString(),
                        details.ASSIGNED_ID == null?string.Empty:details.ASSIGNED_ID.ToString(),
                        details.DISPOSAL_DATE == null?string.Empty:Convert.ToDateTime(details.DISPOSAL_DATE).ToString("dd/MM/yyyy"),
                        details.ISFINALIZED==true?"<span class='ui-icon ui-icon-locked ui-align-center'></span>":"<a href='#' title='Click here to Edit Asset Details' class='ui-icon ui-icon-pencil ui-align-center' onClick=EditAsset('" +URLEncrypt.EncryptParameters1(new string[]{"DetailsId="+details.DETAIL_ID.ToString().Trim(),"BillId="+details.BILL_ID.ToString().Trim(),"HeadId="+details.HEAD_ID.ToString().Trim()})+"'); return false;'>Add Asset Details</a>",
                        details.ISFINALIZED==true?"<span class='ui-icon ui-icon-locked ui-align-center'></span>":"<a href='#' title='Click here to Delete Asset Details' class='ui-icon ui-icon-trash ui-align-center' onClick=DeleteAsset('" +URLEncrypt.EncryptParameters1(new string[]{"DetailsId="+details.DETAIL_ID.ToString().Trim(),"BillId="+details.BILL_ID.ToString().Trim(),"HeadId="+details.HEAD_ID.ToString().Trim()})+"'); return false;'>Delete Asset Details</a>",
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

        public bool AddAssetDetails(AssetDetailsViewModel model, ref string message)
        {
            dbContext = new PMGSYEntities();
            CommonFunctions objCommon = new CommonFunctions();
            try
            {
                ACC_ASSET_DETAILS assetMaster = new ACC_ASSET_DETAILS();
                if (dbContext.ACC_ASSET_DETAILS.Any())
                {
                    assetMaster.DETAIL_ID = dbContext.ACC_ASSET_DETAILS.Max(m => m.DETAIL_ID) + 1;
                }
                else
                {
                    assetMaster.DETAIL_ID = 1;
                }
                assetMaster.ASSET_ID = model.Asset_Id;
                assetMaster.ASSIGNED_ID = model.Assigned_Id;
                assetMaster.BILL_ID = model.Bill_Id;
                if (model.Disposal_Date != null)
                {
                    assetMaster.DISPOSAL_DATE = objCommon.GetStringToDateTime(model.Disposal_Date);
                }
                assetMaster.HEAD_ID = model.Head_Id;
                assetMaster.MODEL_NO = model.Model_No;
                assetMaster.QUANTITY = model.Quantity;
                assetMaster.RATE = model.Rate;
                assetMaster.SERIAL_NO = model.Serial_No;
                assetMaster.TOTAL_AMOUNT = model.Total_Amount;

                //Added By Abhishek Kamble 28-nov-2013
                assetMaster.USERID = PMGSYSession.Current.UserId;
                assetMaster.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                assetMaster.ISFINALIZED = false;
                dbContext.ACC_ASSET_DETAILS.Add(assetMaster);
                dbContext.SaveChanges();
                message = "Asset Details Saved Successfully.";
                return true;
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                message = "Error occurred while processing your request.";
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

        public bool EditAssetDetails(AssetDetailsViewModel model, ref string message)
        {
            dbContext = new PMGSYEntities();
            CommonFunctions objCommon = new CommonFunctions();
            String[] encryptedParameters = null;
            Dictionary<string, string> decryptedParameters = null;
            try
            {
                encryptedParameters = model.EncryptedDetailsId.Split('/');
                decryptedParameters = URLEncrypt.DecryptParameters1(new string[] { encryptedParameters[0],encryptedParameters[1],encryptedParameters[2]});
                int detailsID = Convert.ToInt32(decryptedParameters["DetailsId"]);
                ACC_ASSET_DETAILS assetMaster = dbContext.ACC_ASSET_DETAILS.Find(detailsID);
                assetMaster.ASSET_ID = model.Asset_Id;
                assetMaster.ASSIGNED_ID = model.Assigned_Id;
                assetMaster.BILL_ID = model.Bill_Id;
                if (model.Disposal_Date != null)
                {
                    assetMaster.DISPOSAL_DATE = objCommon.GetStringToDateTime(model.Disposal_Date);
                }
                assetMaster.HEAD_ID = model.Head_Id;
                assetMaster.MODEL_NO = model.Model_No;
                assetMaster.QUANTITY = model.Quantity;
                assetMaster.RATE = model.Rate;
                assetMaster.SERIAL_NO = model.Serial_No;
                assetMaster.TOTAL_AMOUNT = model.Total_Amount;

                //Added By Abhishek Kamble 28-nov-2013
                assetMaster.USERID = PMGSYSession.Current.UserId;
                assetMaster.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                assetMaster.ISFINALIZED = false;
                dbContext.Entry(assetMaster).State = System.Data.Entity.EntityState.Modified;
                dbContext.SaveChanges();
                message = "Asset Details Updated Successfully.";
                return true;
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                message = "Error occurred while processing your request.";
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

        public AssetDetailsViewModel GetAssetInformation(int detailsId)
        {
            dbContext = new PMGSYEntities();
            CommonFunctions objCommon = new CommonFunctions();
            try
            {
                ACC_ASSET_DETAILS assetMaster = dbContext.ACC_ASSET_DETAILS.Find(detailsId);
                AssetDetailsViewModel model = new AssetDetailsViewModel();
                model.EncryptedDetailsId = URLEncrypt.EncryptParameters1(new string[] {"DetailsId="+assetMaster.DETAIL_ID.ToString().Trim() });
                model.Asset_Id = assetMaster.ASSET_ID;
                model.Assigned_Id = assetMaster.ASSIGNED_ID;
                model.Bill_Id = assetMaster.BILL_ID;
                if (assetMaster.DISPOSAL_DATE != null)
                {
                    model.Disposal_Date = objCommon.GetDateTimeToString(assetMaster.DISPOSAL_DATE.Value);
                }
                model.Head_Id = assetMaster.HEAD_ID;
                model.Model_No = assetMaster.MODEL_NO;
                model.Operation = "E";
                model.Quantity = assetMaster.QUANTITY;
                model.Rate = assetMaster.RATE;
                model.Serial_No = assetMaster.SERIAL_NO;
                model.Total_Amount = assetMaster.TOTAL_AMOUNT;
                return model;
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

        public bool DeleteAssetDetails(int detailsId, ref string message)
        {
            using (TransactionScope ts = new TransactionScope())
            {
                dbContext = new PMGSYEntities();
                try
                {
                    ACC_ASSET_DETAILS assetMaster = dbContext.ACC_ASSET_DETAILS.Find(detailsId);

                    //Added By Abhishek Kamble 28-nov-2013
                    assetMaster.USERID = PMGSYSession.Current.UserId;
                    assetMaster.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                    dbContext.Entry(assetMaster).State = System.Data.Entity.EntityState.Modified;
                    dbContext.SaveChanges();

                    dbContext.ACC_ASSET_DETAILS.Remove(assetMaster);
                    dbContext.SaveChanges();
                    message = "Asset details deleted successfully.";
                    ts.Complete();
                    return true;
                }
                catch (Exception ex)
                {
                    ts.Dispose();
                    Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                    message = "Error occurred while processing your request.";
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

        public decimal? GetRemainingAmount(decimal totalAssetAmount,short headId,long billId)
        {
            dbContext = new PMGSYEntities();
            try
            {
                decimal? enteredAssetAmount = dbContext.ACC_ASSET_DETAILS.Where(m => m.BILL_ID == billId && m.HEAD_ID == headId).Sum(m => m.TOTAL_AMOUNT);
                if (enteredAssetAmount != null)
                {
                    return (totalAssetAmount - enteredAssetAmount);
                }
                else
                {
                    return totalAssetAmount;
                }
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

        public bool DeleteAssetPaymentDetails(short headId, long billId,ref string message)
        {
            using(TransactionScope ts=new TransactionScope())
            {
                dbContext = new PMGSYEntities();
                try
                {
                    //added by abhishek kamble 28-nov-2013
                    ACC_ASSET_DETAILS assetDetails = dbContext.ACC_ASSET_DETAILS.Where(m => m.BILL_ID == billId && m.HEAD_ID == headId).FirstOrDefault();

                    if (assetDetails != null)
                    {
                        assetDetails.USERID = PMGSYSession.Current.UserId;
                        assetDetails.IPADD=HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                        dbContext.Entry(assetDetails).State = System.Data.Entity.EntityState.Modified;
                        dbContext.SaveChanges();
                    }
                    
                    int status = dbContext.SP_ACC_DELETE_ASSET_DETAILS(billId, headId);
                    message = "Payment details deleted successfully.";
                    ts.Complete();
                    return true;
                }
                catch (Exception ex)
                {
                    ts.Dispose();
                    Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                    message = "Error occurred while processing your request.";
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

        public bool FinalizeAssetDetails(String id, ref string message)
        {
            try
            {
                using (dbContext = new PMGSYEntities())
                {
                    string[] parameters = id.Split('$');
                    long billId = Convert.ToInt64(parameters[0]);
                    short headId = Convert.ToInt16(parameters[1]);
                    List<ACC_ASSET_DETAILS> lstAssets = dbContext.ACC_ASSET_DETAILS.Where(m => m.BILL_ID == billId && m.HEAD_ID == headId).ToList();
                    lstAssets.ForEach(m => m.ISFINALIZED = true);
                    dbContext.SaveChanges();
                    message = "Finalized successfully.";
                    return true;
                }
            }
            catch (Exception)
            {
                message = "Error occurred while finalizing the details.";
                return false;
            }
        }

        public bool DefinalizeAssetDetails(long billId, short headId, ref string message)
        {
            using(dbContext = new PMGSYEntities())
            {
                try
                {
                    var lstDetails = dbContext.ACC_ASSET_DETAILS.Where(m=>m.BILL_ID == billId && m.HEAD_ID == headId).ToList();
                    lstDetails.ForEach(m => m.ISFINALIZED = false);
                    dbContext.SaveChanges();
                    message = "Details definalized successfully.";
                    return true;
                }
                catch (Exception)
                {
                    message = "Error occurred while definalizing the details.";
                    return false;
                }
            }
        }


    }

    public interface IAssetDetailsDAL
    {

        Array GetAssetDetails(int? page, int? rows, string sidx, string sord, out long totalRecords, short? monthCode, short? yearCode, string chequeNo, string billNo, int adminCode, string fundType);
        List<SelectListItem> GetAssets();
        Array GetAssetDetailsList(int? page, int? rows, string sidx, string sord, out long totalRecords, int billId, int headId,ref bool isFinalize);             
        bool AddAssetDetails(AssetDetailsViewModel model, ref string message);
        bool EditAssetDetails(AssetDetailsViewModel model, ref string message);
        AssetDetailsViewModel GetAssetInformation(int detailsId);
        bool DeleteAssetDetails(int detailsId, ref string message);
        decimal? GetRemainingAmount(decimal totalAssetAmount, short headId, long billId);
        bool DeleteAssetPaymentDetails(short headId, long billId, ref string message);
        bool FinalizeAssetDetails(String id, ref string message);
        bool DefinalizeAssetDetails(long billId, short headId, ref string message);
    }

}