using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PMGSY.DAL.OB;
using PMGSY.Models.OB;
using PMGSY.Models.Receipts;
using PMGSY.Models;
using PMGSY.Models.Common;

namespace PMGSY.BAL.OB
{
    public class OpeningBalanceBAL : IOpeningBalanceBAL
    {
        private IOpeningBalanceDAL objOBDAL = new OpeningBalanceDAL();

        public String GetOBMasterIds(Int64 billId)
        {
           return objOBDAL.GetOBMasterIds(billId);
        }
        
        public OBMasterModel GetOBMasterById(String billId)
        {
            return objOBDAL.GetOBMasterById(billId);
        }

        public ACC_BILL_MASTER GetOBMaster(Int64 billId)
        {
            return objOBDAL.GetOBMaster(billId);
        }

        public OBDetailsModel GetOBDetailsByTransId(Int64 billId, Int16 transId)
        {
            return objOBDAL.GetOBDetailsByTransId(billId, transId);
        }

        public Int64 AddOBMaster(OBMasterModel obMasterModel)
        {
            return objOBDAL.AddOBMaster(obMasterModel);
        }

        public String EditOBMaster(OBMasterModel obMasterModel)
        {
            return objOBDAL.EditOBMaster(obMasterModel);
        }

        public String AddOBDetails(OBDetailsModel obDetailsModel)
        {
            return objOBDAL.AddOBDetails(obDetailsModel);
        }

        public String EditOBDetails(OBDetailsModel obDetailsModel)
        {
            return objOBDAL.EditOBDetails(obDetailsModel);
        }

        public Array GetOBMasterList(ReceiptFilterModel objFilter, out long totalRecords)
        {
            totalRecords = 0;
            return objOBDAL.GetOBMasterList(objFilter, out totalRecords);
        }       

        public String DeleteOBMaster(String billId)
        {
            return objOBDAL.DeleteOBMaster(billId);
        }

        public String DeleteOBDetails(Int64 billId, Int16 transNo)
        {
            return objOBDAL.DeleteOBDetails(billId, transNo);
        }

        public Array GetOBDetailsList(ReceiptFilterModel objFilter, out long totalRecords, out decimal AssetTotal, out decimal LibTotal, out decimal GrossAmount, out string Finalized)
        {
            totalRecords = 0;
            AssetTotal = 0;
            LibTotal = 0;
            GrossAmount = 0;
            Finalized = "N";
            return objOBDAL.GetOBDetailsList(objFilter, out totalRecords, out AssetTotal, out LibTotal, out GrossAmount, out Finalized);
        }

        public String FinalizeOB(Int64 assetBillId, Int64 libBillId)
        {
            return objOBDAL.FinalizeOB(assetBillId,libBillId);
        }

        public String GetOBStatus(TransactionParams objParam)
        {
            return objOBDAL.GetOBStatus(objParam);
        }

        public String ValidateOBDetails(OBDetailsModel obDetailsModel)
        {
            return objOBDAL.ValidateOBDetails(obDetailsModel);
        }

        public JsonCollection GetAssetLiabilityDetails(TransactionParams objParam)
        {
            JsonCollection jsonCol = new JsonCollection();
            List<Dictionary<String, String>> lstAssetJson = new List<Dictionary<string, string>>();
            List<Dictionary<String, String>> lstLibJson = new List<Dictionary<string, string>>();
            List<OBChart> lstParams = new List<OBChart>();
            lstParams = objOBDAL.GetAssetLiabilityDetails(objParam);
            foreach (var record in lstParams)
            {
                Dictionary<String, String> jsonObject = new Dictionary<String, String>();
                jsonObject.Add("transname", record.TransDesc);
                jsonObject.Add("amount", record.Amount.ToString());
                if (record.Id == 1)
                {
                    lstAssetJson.Add(jsonObject);
                }
                else
                {
                    lstLibJson.Add(jsonObject);
                }
            }
            jsonCol.assetList = lstAssetJson;
            jsonCol.libList = lstLibJson;

            return jsonCol;
        }

        public String GetAccountStatus(TransactionParams objParam)
        {
            return objOBDAL.GetAccountStatus(objParam);
        }

        public bool IsFinalPayment(int billId, int roadCode)
        {
            return objOBDAL.IsFinalPayment(billId,roadCode);
        }

    }

    public interface IOpeningBalanceBAL
    {
        String GetOBMasterIds(Int64 billId);
        OBMasterModel GetOBMasterById(String billId);
        ACC_BILL_MASTER GetOBMaster(Int64 billId);
        OBDetailsModel GetOBDetailsByTransId(Int64 billId, Int16 TransId);
        Int64 AddOBMaster(OBMasterModel obMasterModel);
        String EditOBMaster(OBMasterModel obMasterModel);
        Array GetOBMasterList(ReceiptFilterModel objFilter, out long totalRecords);
        Array GetOBDetailsList(ReceiptFilterModel objFilter, out long totalRecords, out decimal AssetTotal, out decimal LibTotal, out decimal GrossAmount, out string Finalized);
        String DeleteOBMaster(String billId);
        String AddOBDetails(OBDetailsModel obDetailsModel);
        String EditOBDetails(OBDetailsModel obDetailsModel);
        String DeleteOBDetails(Int64 billId, Int16 transNo);
        String FinalizeOB(Int64 assetBillId, Int64 libBillId);
        String GetOBStatus(TransactionParams objParam);
        String ValidateOBDetails(OBDetailsModel obDetailsModel);
        JsonCollection GetAssetLiabilityDetails(TransactionParams objParam);
        String GetAccountStatus(TransactionParams objParam);
        bool IsFinalPayment(int billId,int roadCode);
    }
}