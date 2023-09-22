using System.Web.Mvc;

namespace PMGSY.Areas.ContractorGrievances
{
    public class ContractorGrievancesAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "ContractorGrievances";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "ContractorGrievances_default",
                "ContractorGrievances/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
