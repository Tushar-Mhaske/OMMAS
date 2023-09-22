using System.Web.Mvc;

namespace PMGSY.Areas.EmargDataPull
{
    public class EmargDataPullAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "EmargDataPull";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "EmargDataPull_default",
                "EmargDataPull/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
