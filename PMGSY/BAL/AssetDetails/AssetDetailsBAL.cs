using PMGSY.Common;
using PMGSY.DAL;
using PMGSY.Extensions;
using PMGSY.Models;
using PMGSY.Models.AssetDetails;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PMGSY.BAL.AssetDetails
{
    public class AssetDetailsBAL:IAssetDetailsBAL
    {

        IAssetDetailsDAL objAssetDAL = new AssetDetailsDAL();

        public Array GetAssetDetails(int? page, int? rows, string sidx, string sord, out long totalRecords, short? monthCode, short? yearCode, string chequeNo, string billNo, int adminCode, string fundType)
        {
            try
            {
                return objAssetDAL.GetAssetDetails(page, rows, sidx, sord, out totalRecords, monthCode, yearCode, chequeNo, billNo,adminCode,fundType);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                totalRecords = 0;
                return null;
            }
        }

        public Array GetAssetDetailsList(int? page, int? rows, string sidx, string sord, out long totalRecords, int billId, int headId,ref bool isFinalize)
        {
            try
            {
                return objAssetDAL.GetAssetDetailsList(page, rows, sidx, sord, out totalRecords, billId, headId,ref isFinalize);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                totalRecords = 0;
                return null;
            }
        }
        public bool AddAssetDetails(AssetDetailsViewModel model, ref string message)
        {
            try
            {
                return objAssetDAL.AddAssetDetails(model,ref message);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                message = "Error ocurred while processing your request.";
                return false;
            }
        }

        public bool EditAssetDetails(AssetDetailsViewModel model, ref string message)
        {
            try
            {
                return objAssetDAL.EditAssetDetails(model, ref message);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);               
                message = "Error ocurred while processing your request.";
                return false;
            }
        }

        public AssetDetailsViewModel GetAssetInformation(int detailsId)
        {
            try
            {
                return objAssetDAL.GetAssetInformation(detailsId);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                return null;
            }
        }

        public bool DeleteAssetDetails(int detailsId, ref string message)
        {
            try
            {
                return objAssetDAL.DeleteAssetDetails(detailsId, ref message);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                message = "Error occurred while processing your request.";
                return false;
            }
        }

        public bool CheckTotalAmount(int headId,int billId,decimal? totalEnteredAmount,decimal? newTotalAmount,string operation,string detailsId)
        {
            PMGSYEntities dbContext = new PMGSYEntities();
            int detailCode = 0;
            try
            {
                decimal? enteredTotalAmount = null;
                if(dbContext.ACC_ASSET_DETAILS.Any(m=>m.HEAD_ID == headId && m.BILL_ID == billId))
                {
                    enteredTotalAmount = dbContext.ACC_ASSET_DETAILS.Where(m => m.HEAD_ID == headId && m.BILL_ID == billId).Sum(m => m.TOTAL_AMOUNT);
                }
                else
                {
                    enteredTotalAmount = 0;
                }

                if (!string.IsNullOrEmpty(detailsId))
                {
                    String[] encryptedParameters = detailsId.Split('/');
                    Dictionary<string, string> decryptedParameters = URLEncrypt.DecryptParameters1(new string[] { encryptedParameters[0], encryptedParameters[1], encryptedParameters[2] });
                    detailCode = Convert.ToInt32(decryptedParameters["DetailsId"]);
                    ACC_ASSET_DETAILS assetMaster = dbContext.ACC_ASSET_DETAILS.Find(detailCode);
                    enteredTotalAmount = enteredTotalAmount - assetMaster.TOTAL_AMOUNT;
                }
                enteredTotalAmount = newTotalAmount + enteredTotalAmount;
                if (enteredTotalAmount > totalEnteredAmount)
                {
                    return false;
                }
                else
                {
                    return true;
                }
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

        public bool DeleteAssetPaymentDetails(short headId, long billId, ref string message)
        {
            try
            {
                return objAssetDAL.DeleteAssetPaymentDetails(headId, billId, ref message);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);          
                message = "Error occurred while processing your request.";
                return false;
            }
        }

        public bool FinalizeAssetDetails(String id, ref string message)
        {
            try
            {
                return objAssetDAL.FinalizeAssetDetails(id, ref message);
            }
            catch (Exception)
            {
                message = "Error occured while processing your request.";
                return false;
            }
        }

        public bool DefinalizeAssetDetails(long billId, short headId, ref string message)
        {
            try
            {
                return objAssetDAL.DefinalizeAssetDetails(billId,headId, ref message);
            }
            catch (Exception)
            {
                message = "Error occured while processing your request.";
                return false;
            }
        }

    }

    public interface IAssetDetailsBAL
    {

        Array GetAssetDetails(int? page, int? rows, string sidx, string sord,out long totalRecords,short? monthCode,short? yearCode,string chequeNo,string billNo,int adminCode,string fundType);
        Array GetAssetDetailsList(int? page, int? rows, string sidx, string sord, out long totalRecords, int billId, int headId,ref bool isFinalize);
        bool AddAssetDetails(AssetDetailsViewModel model,ref string message);
        bool EditAssetDetails(AssetDetailsViewModel model, ref string message);
        AssetDetailsViewModel GetAssetInformation(int detailsId);
        bool DeleteAssetDetails(int detailsId, ref string message);
        bool CheckTotalAmount(int headId,int billId, decimal? totalEnteredAmount, decimal? newTotalAmount,string operation,string detailsId);
        bool DeleteAssetPaymentDetails(short headId, long billId, ref string message);
        bool FinalizeAssetDetails(String id, ref string message);
        bool DefinalizeAssetDetails(long billId, short headId,ref string message);
    }

}