using System.Web.Mvc;

namespace PMGSY.Areas.RoadList
{
    public class RoadListAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "RoadList";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "RoadList_default",
                "RoadList/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
