/*----------------------------------------------------------------------------------------

 * Project Id:

 * Project Name:OMMAS2

 * File Name: SplitWorkBAL.cs

 * Author : Koustubh Nakate

 * Creation Date :02/July/2013

 * Desc : This class is used to call methods from data access layer class.  
 * ---------------------------------------------------------------------------------------*/


using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PMGSY.DAL.SplitWork;
using PMGSY.Models.SplitWork;

namespace PMGSY.BAL.SplitWork
{
    public class SplitWorkBAL:ISplitWorkBAL
    {
        ISplitWorkDAL splitWorkDAL = new SplitWorkDAL();

        public Array GetSplitWorkDetailsListBAL(int IMSPRRoadCode, int page, int rows, string sidx, string sord, out long totalRecords)
        {
            return splitWorkDAL.GetSplitWorkDetailsListDAL(IMSPRRoadCode, page, rows, sidx, sord, out totalRecords);
        }


        public bool SaveSplitWorkDetailsBAL(SplitWorkDetails splitWorkDetails, ref string message)
        {
            return splitWorkDAL.SaveSplitWorkDetailsDAL(splitWorkDetails, ref message);
        }


        public SplitWorkDetails GetSplitWorkDetailsBAL(int IMSPRRoadCode, int IMSWorkCode)
        {
            return splitWorkDAL.GetSplitWorkDetailsDAL(IMSPRRoadCode, IMSWorkCode);
        }

        public bool UpdateSplitWorkDetailsBAL(SplitWorkDetails splitWorkDetails, ref string message)
        {
            return splitWorkDAL.UpdateSplitWorkDetailsDAL(splitWorkDetails, ref message);
        }


        public bool DeleteSplitWorkDetailsBAL(int IMSPRRoadCode, int IMSWorkCode, ref string message)
        {
            return splitWorkDAL.DeleteSplitWorkDetailsDAL(IMSPRRoadCode,IMSWorkCode, ref message);
        }


        public bool CheckAgreementExistBAL(int IMSPRRoadCode,ref bool isAgreementExist, ref bool isSplitWorkExist,ref bool isSplitCountExist)
        {
            return splitWorkDAL.CheckAgreementExistBAL(IMSPRRoadCode, ref isAgreementExist, ref isSplitWorkExist,ref isSplitCountExist);
        }


        public bool AddSplitCountBAL(SplitCount splitCount, ref string message)
        {
            return splitWorkDAL.AddSplitCountBAL(splitCount, ref message);
        }


        public string GetSanctionedCostDetailsBAL(int IMSPRRoadCode,int IMSWorkCode)
        {
            return splitWorkDAL.GetSanctionedCostDetailsDAL(IMSPRRoadCode, IMSWorkCode);
        }


        public bool FinalizedSplitWorkDetailsBAL(int IMSPRRoadCode, ref string message)
        {
            return splitWorkDAL.FinalizedSplitWorkDetailsDAL(IMSPRRoadCode, ref message);
        }
    }

    public interface ISplitWorkBAL
    {

        Array GetSplitWorkDetailsListBAL(int IMSPRRoadCode, int page, int rows, string sidx, string sord, out long totalRecords);

        bool SaveSplitWorkDetailsBAL(SplitWorkDetails splitWorkDetails, ref string message);

        SplitWorkDetails GetSplitWorkDetailsBAL(int IMSPRRoadCode, int IMSWorkCode);

        bool UpdateSplitWorkDetailsBAL(SplitWorkDetails splitWorkDetails, ref string message);

        bool DeleteSplitWorkDetailsBAL(int IMSPRRoadCode, int IMSWorkCode, ref string message);

        bool CheckAgreementExistBAL(int IMSPRRoadCode,ref bool isAgreementExist, ref bool isSplitWorkExist, ref bool isSplitCountExist);

        bool AddSplitCountBAL(SplitCount splitCount, ref string message);

        string GetSanctionedCostDetailsBAL(int IMSPRRoadCode, int IMSWorkCode);

        bool FinalizedSplitWorkDetailsBAL(int IMSPRRoadCode, ref string message);
    }
}