using System.Web.Mvc;

namespace PMGSY.Areas.ProposalSSRSReports
{
    public class ProposalSSRSReportsAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "ProposalSSRSReports";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "ProposalSSRSReports_default",
                "ProposalSSRSReports/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
