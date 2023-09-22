using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PMGSY.DAL.MPMLAProposal;
using PMGSY.Models.MPMLAProposal;
namespace PMGSY.BAL.MPMLAProposal
{
    public class MPMLAProposalBAL:IMPMLAProposalBAL
    {

        #region MP Proposed Road Details

            public Array ListMPProposedRoadList(int yearCode, int constituencyCode,int? page, int? rows, string sidx, string sord, out long totalRecords)
            {
                try
                {
                    IMPMLAProposalDAL objDAL = new MPMLAProposalDAL();
                    return objDAL.ListMPProposedRoadList(yearCode, constituencyCode, page, rows, sidx, sord, out totalRecords);
                }
                catch (Exception Ex)
                {
                    Elmah.ErrorSignal.FromCurrentContext().Raise(Ex, HttpContext.Current);
        
                    totalRecords = 0;
                    return null;            
                }
            }

            public bool AddMPProposedRoadDetails(MPProposalViewModel mpProposalViewModel, ref string message)
            {
                try
                {
                   IMPMLAProposalDAL objDAL = new MPMLAProposalDAL();
                   return objDAL.AddMPProposedRoadDetails(mpProposalViewModel, ref message);
                }
                catch (Exception Ex)
                {
                    Elmah.ErrorSignal.FromCurrentContext().Raise(Ex, HttpContext.Current);
        
                    message = "An error Occured while proccessing your request.";
                    return false;
                }
            }

            public bool EditMPProposedRoadDetails(MPProposalViewModel mpProposalViewModel, ref string message)
            {
                try
                {
                    IMPMLAProposalDAL objDAL = new MPMLAProposalDAL();
                    return objDAL.EditMPProposedRoadDetails(mpProposalViewModel, ref message);
                }
                catch (Exception Ex)
                {
                    Elmah.ErrorSignal.FromCurrentContext().Raise(Ex, HttpContext.Current);
        
                    message = "An error Occured while proccessing your request.";
                    return false;
                }   
            }

            public bool DeleteMPProposedRoadDetails(int imsYear, int mpConstituencyCode, int ImsRoadId, ref string message)
            {
                try
                {
                    IMPMLAProposalDAL objDAL = new MPMLAProposalDAL();
                    return objDAL.DeleteMPProposedRoadDetails(imsYear,mpConstituencyCode,ImsRoadId,ref message);
                }
                catch (Exception Ex)
                {
                    Elmah.ErrorSignal.FromCurrentContext().Raise(Ex, HttpContext.Current);
        
                    message = "An error Occured while proccessing your request.";
                    return false;
                }   
            }

            public MPProposalViewModel GetMPProposedRoadDetails(int imsYear, int constCode, int roadId)
            {
                try
                {
                    IMPMLAProposalDAL objDAL = new MPMLAProposalDAL();
                    return objDAL.GetMPProposedRoadDetails(imsYear,constCode,roadId);
                }
                catch (Exception Ex)
                {
                    Elmah.ErrorSignal.FromCurrentContext().Raise(Ex, HttpContext.Current);
         
                    return null;
                }
            }

        #endregion  MP Proposed Road Details

        #region MP Proposed Road Inclusion Details

            public bool AddMPProposalRoadInclusionDetails(MPRoadProposalInclusionViewModel mpProposalInclusionViewModel, ref string message)
            {
                try
                {
                    IMPMLAProposalDAL objDAL = new MPMLAProposalDAL();
                    return objDAL.AddMPProposalRoadInclusionDetails(mpProposalInclusionViewModel, ref message);
                }
                catch (Exception Ex)
                {
                    Elmah.ErrorSignal.FromCurrentContext().Raise(Ex, HttpContext.Current);
                   message = "An error Occured while proccessing your request.";
                    return false;
                }
            }


            public MPRoadProposalInclusionViewModel GetMPProposalRoadInclusionDetails(int imsYear, int constCode, int roadId)
            {
                try
                {
                    IMPMLAProposalDAL objDAL = new MPMLAProposalDAL();
                    return objDAL.GetMPProposalRoadInclusionDetails(imsYear, constCode, roadId);
                }
                catch (Exception Ex)
                {
                    Elmah.ErrorSignal.FromCurrentContext().Raise(Ex, HttpContext.Current);
                    return null;
                }
            }


        #endregion MP Proposed Road Inclusion Details


        #region MLA Proposed Road Details

            public Array ListMLAProposedRoadList(int yearCode, int constituencyCode, int? page, int? rows, string sidx, string sord, out long totalRecords)
            {
                try
                {
                    IMPMLAProposalDAL objDAL = new MPMLAProposalDAL();
                    return objDAL.ListMLAProposedRoadList(yearCode, constituencyCode, page, rows, sidx, sord, out totalRecords);
                }
                catch (Exception Ex)
                {
                    Elmah.ErrorSignal.FromCurrentContext().Raise(Ex, HttpContext.Current);
                    totalRecords = 0;
                    return null;
                }
            }

            public bool AddMLAProposedRoadDetails(MLAProposalViewModel mlaProposalViewModel, ref string message)
            {
                try
                {
                    IMPMLAProposalDAL objDAL = new MPMLAProposalDAL();
                    return objDAL.AddMLAProposedRoadDetails(mlaProposalViewModel, ref message);
                }
                catch (Exception Ex)
                {
                    Elmah.ErrorSignal.FromCurrentContext().Raise(Ex, HttpContext.Current);
                     message = "An error Occured while proccessing your request.";
                    return false;
                }
            }

            public bool EditMLAProposedRoadDetails(MLAProposalViewModel mlaProposalViewModel, ref string message)
            {
                try
                {
                    IMPMLAProposalDAL objDAL = new MPMLAProposalDAL();
                    return objDAL.EditMLAProposedRoadDetails(mlaProposalViewModel, ref message);
                }
                catch (Exception Ex)
                {
                    Elmah.ErrorSignal.FromCurrentContext().Raise(Ex, HttpContext.Current);
                    message = "An error Occured while proccessing your request.";
                    return false;
                }
            }

            public bool DeleteMLAProposedRoadDetails(int imsYear, int mlaConstituencyCode, int ImsRoadId, ref string message)
            {
                try
                {
                    IMPMLAProposalDAL objDAL = new MPMLAProposalDAL();
                    return objDAL.DeleteMLAProposedRoadDetails(imsYear, mlaConstituencyCode, ImsRoadId, ref message);
                }
                catch (Exception Ex)
                {
                    Elmah.ErrorSignal.FromCurrentContext().Raise(Ex, HttpContext.Current);
                    message = "An error Occured while proccessing your request.";
                    return false;
                }
            }

            public MLAProposalViewModel GetMLAProposedRoadDetails(int imsYear, int constCode, int roadId)
            {
                try
                {
                    IMPMLAProposalDAL objDAL = new MPMLAProposalDAL();
                    return objDAL.GetMLAProposedRoadDetails(imsYear, constCode, roadId);
                }
                catch (Exception Ex)
                {
                    Elmah.ErrorSignal.FromCurrentContext().Raise(Ex, HttpContext.Current);
                    return null;
                }
            }

            #endregion  MLA Proposed Road Details

        #region MLA Proposed Road Inclusion Details

            public bool AddMLAProposalRoadInclusionDetails(MLARoadProposalInclusionViewModel mlaProposalInclusionViewModel, ref string message)
            {
                try
                {
                    IMPMLAProposalDAL objDAL = new MPMLAProposalDAL();
                    return objDAL.AddMLAProposalRoadInclusionDetails(mlaProposalInclusionViewModel, ref message);
                }
                catch (Exception Ex)
                {
                    Elmah.ErrorSignal.FromCurrentContext().Raise(Ex, HttpContext.Current);
                     message = "An error Occured while proccessing your request.";
                    return false;
                }
            }


            public MLARoadProposalInclusionViewModel GetMLAProposalRoadInclusionDetails(int imsYear, int constCode, int roadId)
            {
                try
                {
                    IMPMLAProposalDAL objDAL = new MPMLAProposalDAL();
                    return objDAL.GetMLAProposalRoadInclusionDetails(imsYear, constCode, roadId);
                }
                catch (Exception Ex)
                {
                    Elmah.ErrorSignal.FromCurrentContext().Raise(Ex, HttpContext.Current);
                    return null;
                }
            }


            #endregion MLA Proposed Road Inclusion Details



    }


    public interface IMPMLAProposalBAL
    {
        #region MP Proposed Road Details
            Array ListMPProposedRoadList(int yearCode, int constituencyCode, int? page, int? rows, string sidx, string sord, out long totalRecords);
            bool AddMPProposedRoadDetails(MPProposalViewModel mpProposalViewModel, ref string message);
            bool EditMPProposedRoadDetails(MPProposalViewModel mpProposalViewModel, ref string message);
            bool DeleteMPProposedRoadDetails(int imsYear, int mpConstituencyCode, int ImsRoadId, ref string message);
            MPProposalViewModel GetMPProposedRoadDetails(int imsYear, int constCode, int roadId);
        #endregion MP Proposed Road Details

        #region MP Proposed Road Inclusion Details

            bool AddMPProposalRoadInclusionDetails(MPRoadProposalInclusionViewModel mpProposalInclusionViewModel, ref string message);
            MPRoadProposalInclusionViewModel GetMPProposalRoadInclusionDetails(int imsYear, int constCode, int roadId);

        #endregion MP Proposed Road Inclusion Details
                
        
        #region MLA Proposed Road Details

            Array ListMLAProposedRoadList(int yearCode, int constituencyCode, int? page, int? rows, string sidx, string sord, out long totalRecords);
            bool AddMLAProposedRoadDetails(MLAProposalViewModel mlaProposalViewModel, ref string message);
            bool EditMLAProposedRoadDetails(MLAProposalViewModel mlaProposalViewModel, ref string message);
            bool DeleteMLAProposedRoadDetails(int imsYear, int mlaConstituencyCode, int ImsRoadId, ref string message);
            MLAProposalViewModel GetMLAProposedRoadDetails(int imsYear, int constCode, int roadId);

        #endregion MLA Proposed Road Details

        #region MLA Proposal Inclusion

            bool AddMLAProposalRoadInclusionDetails(MLARoadProposalInclusionViewModel mlaProposalInclusionViewModel, ref string message);
            MLARoadProposalInclusionViewModel GetMLAProposalRoadInclusionDetails(int imsYear, int constCode, int roadId);

        #endregion MLA Proposal Inclusion

    }
}