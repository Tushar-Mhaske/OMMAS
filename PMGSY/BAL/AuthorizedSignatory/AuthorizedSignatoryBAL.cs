using PMGSY.DAL.AuthorizedSignatory;
using PMGSY.Models.AuthorizedSignatory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PMGSY.BAL.AuthorizedSignatory
{
    public class AuthorizedSignatoryBAL : IAuthorizedSignatoryBAL
    {
        IAuthorizedSignatoryDAL AuthSigDAL = null;

        public AuthorizedSignatoryBAL()
        {
            AuthSigDAL = new AuthorizedSignatoryDAL();
        }

      
        /// <summary>
        /// function to get the authorized signatory details
        /// </summary>
        /// <param name="int_state_code"> state code of which authorized signatory details to be listed</param>
        /// <returns></returns>
        public List<AuthorizedSignatoryModel> GetAuthorizedSignatoryDetails(int? page, int? rows, string sidx, string sord, int int_state_code, string ShowActiveAuthSig, string searchCriteria, out long totalRecords)
        {
            try
            {


                return AuthSigDAL.GetAuthorizedSignatoryDetails(page, rows, sidx, sord, int_state_code, ShowActiveAuthSig,searchCriteria, out totalRecords);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
               throw new Exception("Error While listing authorized Signatories");
            }
        }

        public AuthorizedSignatoryModel GetAuthorizedSignatoryDetails(int adminNdCode, int adminNoOfficerCode,string status)
        {
            try
            {
                return AuthSigDAL.GetAuthorizedSignatoryDetails(adminNdCode,adminNoOfficerCode, status);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                throw new Exception("Error While getting authorized Signatories to view or edit");
            }
        }

        public String AddEditAuthorizedSignatoryDetails(int adminNoOfficerCode,int adminNdCode, int officerCode, int DistrictCode, string Operation, AuthorizedSignatoryModel model, ref string message)
        {
            try
            {
                return AuthSigDAL.AddEditAuthorizedSignatoryDetails(adminNoOfficerCode,adminNdCode, officerCode, DistrictCode, Operation, model, ref message);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                throw new Exception("Error While getting authorized Signatories to view or edit");
            }
        }


        //public AuthorizedSignatoryModel GetAuthorizedSignatoryDetails(int adminNdCode)
        //{
        //    try
        //    {
        //        return AuthSigDAL.GetAuthorizedSignatoryDetails(adminNdCode);
        //    }
        //    catch (Exception ex)
        //    {
        //        throw new Exception("Error While getting authorized Signatories details");
        //    }
        //}

        public int DeRegisterDSC(int adminNoOfficerCode)
        {
            try
            {
                return AuthSigDAL.DeRegisterDSC(adminNoOfficerCode);
            }

            catch (Exception Ex)
            {
                throw new Exception("Error while deleting certificate registration details...");
            }

        }

    
        
    }
    public interface IAuthorizedSignatoryBAL
    {
        List<AuthorizedSignatoryModel> GetAuthorizedSignatoryDetails(int? page, int? rows, string sidx, string sord, int int_state_code, string ShowActiveAuthSig, string searchCriteria, out long totalRecords);
        AuthorizedSignatoryModel GetAuthorizedSignatoryDetails(int adminNdCode, int adminNoOfficerCode, string status);
        String AddEditAuthorizedSignatoryDetails(int adminNoOfficerCode, int adminNdCode, int officerCode, int DistrictCode, string Operation, AuthorizedSignatoryModel model,  ref string message);
        //AuthorizedSignatoryModel GetAuthorizedSignatoryDetails(int adminNdCode);

        int DeRegisterDSC(int adminNoOfficerCode);

    }
}