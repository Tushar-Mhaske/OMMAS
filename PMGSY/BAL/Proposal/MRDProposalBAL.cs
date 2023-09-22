using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PMGSY.DAL.Proposal;
using PMGSY.Models.Proposal;

namespace PMGSY.BAL.Proposal
{
    public class MRDProposalBAL : IProposalBAL1
    {
        IProposalDAL1 objProposalDAL = new MRDProposalDAL();

        public Array ListMrdDroppedBAL(int clrId,int stateCode, int year, int batch, int agencyCode, int collaboration, int page, int rows, string sidx, string sord, out long totalRecords)
        {
            return objProposalDAL.ListMrdDroppedDAL(clrId, stateCode, year, batch, agencyCode, collaboration, page, rows, sidx, sord, out totalRecords);
        }

        public Array ListMrdClearanceBAL(int stateCode, int year, int batch, int agencyCode, int collaboration, int page, int rows, string sidx, string sord, out long totalRecords)
        {
            return objProposalDAL.ListMrdClearanceDAL(stateCode, year, batch, agencyCode, collaboration, page, rows, sidx, sord, out totalRecords);
        }

        //public MrdDroppedViewModel GetMrdDroppedDetailsBAL(int clearanceCode)
        //{
        //    return objProposalDAL.GetMrdDroppedDetailsDAL(clearanceCode);
        //}
        public bool saveDroppedLettersBAL(PMGSY.Models.Proposal.MrdDroppedViewModel crMrdDroppedLetters, ref string msg)
        {
            return objProposalDAL.saveDroppedLettersDAL(crMrdDroppedLetters, ref msg);
        }

        public bool editDroppedLettersBAL(PMGSY.Models.Proposal.MrdDroppedViewModel crMrdDroppedLetters)
        {
            return objProposalDAL.editDroppedLettersDAL(crMrdDroppedLetters);
        }

        public bool deleteDroppedLettersBAL(int dropCode)
        {
            return objProposalDAL.deleteDroppedLettersDAL(dropCode);
        }
    }

    public interface IProposalBAL1
    {
        Array ListMrdClearanceBAL(int stateCode, int year, int batch, int agencyCode, int collaboration, int page, int rows, string sidx, string sord, out long totalRecords);
        Array ListMrdDroppedBAL(int clrId, int stateCode, int year, int batch, int agencyCode, int collaboration, int page, int rows, string sidx, string sord, out long totalRecords);
        //MrdDroppedViewModel GetMrdDroppedDetailsBAL(int clearanceCode);
        bool saveDroppedLettersBAL(PMGSY.Models.Proposal.MrdDroppedViewModel crMrdDroppedLetters, ref string msg);
        
        bool editDroppedLettersBAL(PMGSY.Models.Proposal.MrdDroppedViewModel crMrdDroppedLetters);

        bool deleteDroppedLettersBAL(int dropCode);
    }
}