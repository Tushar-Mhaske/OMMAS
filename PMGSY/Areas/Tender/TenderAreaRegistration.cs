using System.Web.Mvc;

namespace PMGSY.Areas.Tender
{
    public class TenderAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "Tender";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "Tender_default",
                "Tender/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
