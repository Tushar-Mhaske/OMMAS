using System.Web.Mvc;

namespace PMGSY.Areas.PRAYASNEW
{
    public class PRAYASNEWAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "PRAYASNEW";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "PRAYASNEW_default",
                "PRAYASNEW/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
