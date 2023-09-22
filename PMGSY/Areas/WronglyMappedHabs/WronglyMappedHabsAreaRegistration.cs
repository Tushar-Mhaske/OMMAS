using System.Web.Mvc;

namespace PMGSY.Areas.WronglyMappedHabs
{
    public class WronglyMappedHabsAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "WronglyMappedHabs";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "WronglyMappedHabs_default",
                "WronglyMappedHabs/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
